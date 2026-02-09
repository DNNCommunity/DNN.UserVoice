using AutoFixture;
using DNN.Modules.UserVoice.Adapters;
using DNN.Modules.UserVoice.Data;
using DNN.Modules.UserVoice.Data.Entities;
using DNN.Modules.UserVoice.Data.Repositories;
using DNN.Modules.UserVoice.Providers;
using DNN.Modules.UserVoice.Services.Ideas;
using DNN.Modules.UserVoice.Services.Ideas.DTOs;
using DotNetNuke.Abstractions.Users;
using Effort;
using FluentValidation;
using NSubstitute;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.Services.Ideas
{
    public class IdeasServiceTests
    {
        private readonly Fixture fixture;
        private readonly CancellationToken token;

        private readonly InlineValidator<SaveIdeaDtoWithContext> saveIdeaDtoValidator;
        private readonly IIdeaRepository ideaRepository;
        private readonly IDateTimeProvider dateTimeProvider;
        private readonly InlineValidator<RequestIdeaDeletionDtoWithContext> requestIdeaDeletionValidator;
        private readonly IUserControllerAdapter userController;

        private readonly IIdeaService ideaService;

        public IdeasServiceTests()
        {
            this.fixture = new Fixture();
            this.token = new CancellationToken();
            this.saveIdeaDtoValidator = new InlineValidator<SaveIdeaDtoWithContext>();
            this.ideaRepository = Substitute.For<IIdeaRepository>();
            this.dateTimeProvider = Substitute.For<IDateTimeProvider>();
            this.requestIdeaDeletionValidator = new InlineValidator<RequestIdeaDeletionDtoWithContext>();
            this.userController = Substitute.For<IUserControllerAdapter>();

            this.ideaService = new IdeaService(
                this.saveIdeaDtoValidator,
                this.ideaRepository,
                this.requestIdeaDeletionValidator,
                this.dateTimeProvider,
                this.userController);
        }

        [Fact]
        public async Task Save_ValidatesDto()
        {
            // Arrange
            var dtoWithContext = fixture.Create<SaveIdeaDtoWithContext>();
            dtoWithContext.ModuleId = -1;
            this.saveIdeaDtoValidator
                .RuleFor(x => x.ModuleId)
                .GreaterThan(0);

            // Act
            var result = await this.ideaService.SaveIdeaAsync(dtoWithContext, this.token);

            // Assert
            result.Switch(
                success => Assert.Fail("Expected an error."),
                error => Assert.Single(error.Value));
        }

        [Fact]
        public async Task Save_Creates()
        {
            // Arrange
            var dto = fixture.Create<SaveIdeaDtoWithContext>();

            // Act
            var result = await this.ideaService.SaveIdeaAsync(dto, this.token);

            // Assert
            result.Switch(
                success =>
                {
                    this.ideaRepository
                        .Received(1)
                        .CreateAsync(
                            Arg.Is<Idea>(i =>
                                i.ModuleId == dto.ModuleId &&
                                i.Title == dto.Dto.Title.Trim() &&
                                i.Description == dto.Dto.Description.Trim() &&
                                i.Id == 0
                            ),
                            dto.ActingUserId,
                            this.token);
                },
                error => Assert.Fail("Expected success."));
        }

        [Fact]
        public async Task Save_Updates()
        {
            // Arrange
            var dto = fixture.Create<SaveIdeaDtoWithContext>();
            var existingIdea = fixture
                .Build<Idea>()
                .Without(x => x.UserVotes)
                .Create();
            existingIdea.Id = dto.Dto.Id;
            this.ideaRepository
                .GetByIdAsync(dto.Dto.Id, this.token)
                .Returns(existingIdea);

            // Act
            var result = await this.ideaService.SaveIdeaAsync(dto, this.token);
            
            // Assert
            result.Switch(
                success =>
                {
                    this.ideaRepository
                        .DidNotReceive()
                        .CreateAsync(Arg.Any<Idea>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
                    this.ideaRepository
                        .Received(1)
                        .UpdateAsync(
                            Arg.Is<Idea>(i =>
                                i.Id == existingIdea.Id &&
                                i.ModuleId == existingIdea.ModuleId &&
                                i.Title == dto.Dto.Title &&
                                i.Description == dto.Dto.Description
                            ),
                            dto.ActingUserId,
                            this.token);
                },
                error => Assert.Fail("Expected success."));
        }

        [Fact]
        public async Task SearchesIdeas()
        {
            DbConnection connection = DbConnectionFactory.CreateTransient();
            using (var ctx = new ModuleDbContext(connection))
            {
                // Arrange
                var moduleId = 123;

                var idea1 = fixture.Build<Idea>()
                    .With(i => i.ModuleId, moduleId)
                    .With(i => i.Title, "An Idea about Performance")
                    .With(i => i.Description, "This idea aims to enhance website performance, but the description mentions seo.")
                    .With(i => i.DeletedOn, default(DateTime?))
                    .Without(x => x.UserVotes)
                    .Create();
                var idea2 = fixture.Build<Idea>()
                    .With(i => i.ModuleId, moduleId)
                    .With(i => i.Title, "Some Idea about SEO")
                    .With(i => i.Description, "This idea is focused on improving search engine optimization.")
                    .With(i => i.DeletedOn, default(DateTime?))
                    .Without(x => x.UserVotes)
                    .Create();
                var idea3 = fixture.Build<Idea>()
                    .With(i => i.ModuleId, 999)
                    .With(i => i.Title, "Mentions SEO but for wrong module")
                    .With(i => i.Description, "Also mentions SEO for still for wrong module")
                    .With(i => i.DeletedOn, default(DateTime?))
                    .Without(x => x.UserVotes)
                    .Create();
                var idea4 = fixture.Build<Idea>()
                    .With(i => i.ModuleId, moduleId)
                    .With(i => i.Title, "Deleted SEO idea")
                    .With(i => i.Description, "This idea mentions SEO but is deleted and should not be included.")
                    .With(i => i.DeletedOn, this.dateTimeProvider.GetUtcNow())
                    .Without(x => x.UserVotes)
                    .Create();
                var ideas = new[] { idea1, idea2, idea3, idea4 };

                ctx.Ideas.AddRange(ideas);
                ctx.SaveChanges();
                var ideaRepository = new IdeaRepository(ctx, this.dateTimeProvider);
                IIdeaService ideaService = new IdeaService(
                    this.saveIdeaDtoValidator,
                    ideaRepository,
                    this.requestIdeaDeletionValidator,
                    this.dateTimeProvider,
                    this.userController);
                var dto = new SearchIdeasDtoWithContext
                {
                    ModuleId = moduleId,
                    ActingUserId = 234,
                    Dto = new SearchIdeasDto
                    {
                        Query = "seo",
                        OnlyMyIdeas = false,
                    },
                };

                // Act
                var result = await ideaService.SearchIdeasAsync(dto, this.token);
                
                // Assert
                Assert.Equal(2, result.ResultCount);
                Assert.Collection(result.Items,
                    item => Assert.Equal(idea2.Id, item.Id),
                    item => Assert.Equal(idea1.Id, item.Id));
            }
        }

        [Fact]
        public async Task GetsIdeaDetails()
        {
            // Arrange
            var idea = fixture.Build<Idea>()
                .With(x => x.ModuleId, fixture.Create<int>())
                .With(x => x.CreatedByUserId, fixture.Create<int>())
                .With(x => x.CreatedAt, DateTime.UtcNow.AddDays(-1))
                .Without(x => x.UserVotes)
                .Create();
            this.ideaRepository
                .GetByIdAsync(idea.Id, this.token)
                .Returns(idea);

            var actingUser = Substitute.For<IUserInfo>();
            actingUser.DisplayName.Returns("Acting User");
            actingUser.UserID.Returns(fixture.Create<int>());
            actingUser.IsAdmin.Returns(false);
            
            var authorUser = Substitute.For<IUserInfo>();
            authorUser.DisplayName.Returns("Author User");
            authorUser.UserID.Returns(idea.CreatedByUserId);
            var portalId = fixture.Create<int>();
            this.userController
                .GetUserById(portalId, idea.CreatedByUserId)
                .Returns(authorUser);

            // Act
            var result = await this.ideaService.GetIdeaDetailsAsync(idea.Id, actingUser, portalId, this.token);

            // Assert
            Assert.Equal(idea.Id, result.Id);
            Assert.Equal(idea.Title, result.Title);
            Assert.Equal(idea.Description, result.Description);
            Assert.False(result.CanEdit);
            Assert.Equal(authorUser.DisplayName, result.CreatedByUserDisplayName);
            Assert.Equal(idea.CreatedAt, result.CreatedAt);
            Assert.Equal("yesterday", result.CreatedSince);
        }

        [Fact]
        public async Task GetsIdeaDetails_WhenUserCanEdit()
        {
            // Arrange
            var idea = fixture
                .Build<Idea>()
                .Without(x => x.UserVotes)
                .Create();
            this.ideaRepository
                .GetByIdAsync(idea.Id, this.token)
                .Returns(idea);
            var user = Substitute.For<IUserInfo>();
            user.UserID.Returns(idea.CreatedByUserId);
            user.IsAdmin.Returns(false);
            var portalId = fixture.Create<int>();

            // Act
            var result = await this.ideaService.GetIdeaDetailsAsync(idea.Id, user, portalId, this.token);

            // Assert
            Assert.Equal(idea.Id, result.Id);
            Assert.Equal(idea.Title, result.Title);
            Assert.Equal(idea.Description, result.Description);
            Assert.True(result.CanEdit);
        }

        [Fact]
        public async Task GetsIdeaDetails_WhenUserIsAdmin()
        {
            // Arrange
            var idea = fixture.Build<Idea>()
                .With(x => x.CreatedByUserId, 123)
                .Without(x => x.UserVotes)
                .Create();
            this.ideaRepository
                .GetByIdAsync(idea.Id, this.token)
                .Returns(idea);
            var user = Substitute.For<IUserInfo>();
            user.UserID.Returns(234);
            user.IsAdmin.Returns(true);
            var portalId = fixture.Create<int>();

            // Act
            var result = await this.ideaService.GetIdeaDetailsAsync(idea.Id, user, portalId, this.token);

            // Assert
            Assert.Equal(idea.Id, result.Id);
            Assert.Equal(idea.Title, result.Title);
            Assert.Equal(idea.Description, result.Description);
            Assert.True(result.CanEdit);
        }

        [Fact]
        public async Task SoftDelete_Validates()
        {
            // Arrange
            var dto = new RequestIdeaDeletionDtoWithContext
            {
                ActingUserId = 123,
                PortalId = -1,
                Dto = new RequestIdeaDeletionDto
                {
                    Id = 789,
                },
            };
            this.requestIdeaDeletionValidator
                .RuleFor(x => x.PortalId)
                .GreaterThan(-1);

            // Act
            var result = await this.ideaService.SoftDeleteIdea(dto, this.token);
            
            // Assert
            result.Switch(
                success => Assert.Fail("Expected an error."),
                error => Assert.Single(error.Value));
        }

        [Fact]
        public async Task SoftDelete_Deletes()
        {
            // Arrange
            this.dateTimeProvider
                .GetUtcNow()
                .Returns(new DateTime(2026, 2, 1));
            var dto = fixture.Create<RequestIdeaDeletionDtoWithContext>();
            var existingIdea = fixture
                .Build<Idea>()
                .Without(x => x.UserVotes)
                .Create();
            existingIdea.DeletedOn = null;
            existingIdea.Id = dto.Dto.Id;
            this.ideaRepository
                .GetByIdAsync(dto.Dto.Id, this.token)
                .Returns(existingIdea);

            // Act
            var result = await this.ideaService.SoftDeleteIdea(dto, this.token);
            
            // Assert
            result.Switch(
                success =>
                {
                    this.ideaRepository
                        .Received(1)
                        .UpdateAsync(
                            Arg.Is<Idea>(i =>
                                i.DeletedOn == this.dateTimeProvider.GetUtcNow()
                            ),
                            dto.ActingUserId,
                            this.token);
                },
                error => Assert.Fail("Expected success."));
        }
    }
}
