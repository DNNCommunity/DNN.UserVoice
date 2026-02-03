using AutoFixture;
using DNN.Modules.UserVoice.Controllers;
using DNN.Modules.UserVoice.Data.Entities;
using DNN.Modules.UserVoice.Data.Repositories;
using DNN.Modules.UserVoice.Entities.ProblemDetails;
using DNN.Modules.UserVoice.Services.Ideas.DTOs;
using DotNetNuke.Abstractions.Users;
using DotNetNuke.ExtensionPoints;
using NSubstitute;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http.Results;
using Xunit;

namespace IntegrationTests.Controllers
{
    public class IdeaControllerTests : FakeControllerContext
    {
        private readonly IdeaController ideaController;

        public IdeaControllerTests()
        {
            this.ideaController = this.GetController<IdeaController>();
        }

        [Fact]
        public async Task SaveIdea_ReportsErrorsCorrectly()
        {
            // Arrange
            var dto = new SaveIdeaDto
            {
                Title = string.Empty,
                Description = "This is a test idea description.",
            };

            // Act
            var response = await this.ideaController.SaveIdea(dto, this.token);

            // Assert
            var contentResult = Assert.IsType<NegotiatedContentResult<ProblemDetails>>(response);
            Assert.Equal(HttpStatusCode.BadRequest, contentResult.StatusCode);
            var result = Assert.IsType<ProblemDetails>(contentResult.Content);
            var error = Assert.Single(result.Errors);
            Assert.Equal(this.localizationService.ViewModel.ModelValidation.TitleRequired, error);
        }

        [Fact]
        public async Task SaveIdea_PreventsEditingOtherUserIdea()
        {
            // Arrange
            var originalUserId = this.RequestContext.User.UserID + 1;
            this.userController
                .GetUserById(RequestContext.PortalSettings.PortalId, RequestContext.User.UserID)
                .Returns(this.RequestContext.User);
            var idea = new Idea
            {
                Description = "Original Description",
                Title = "Original Title",
                ModuleId = this.RequestContext.Module.ModuleID,
                CreatedByUserId = originalUserId,
            };
            this.dataContext.Ideas.Add(idea);
            await this.dataContext.SaveChangesAsync(this.token);
            var dto = new SaveIdeaDto
            {
                Id = idea.Id,
                Title = "Updated Title",
                Description = "Updated Description",
            };

            // Act
            var response = await this.ideaController.SaveIdea(dto, this.token);

            // Assert
            var contentResult = Assert.IsType<NegotiatedContentResult<ProblemDetails>>(response);
            Assert.Equal(HttpStatusCode.BadRequest, contentResult.StatusCode);
            var result = Assert.IsType<ProblemDetails>(contentResult.Content);
            var error = Assert.Single(result.Errors);
            Assert.Equal(this.localizationService.ViewModel.ModelValidation.CannotEditIdeaNotYours, error);
        }

        [Fact]
        public async Task SaveIdea_Creates()
        {
            // Arrange
            var dto = new SaveIdeaDto
            {
                Title = "Title",
                Description = "Description",
            };

            // Act
            var response = await this.ideaController.SaveIdea(dto, this.token);

            // Assert
            Assert.IsType<OkResult>(response);
            var savedIdea = Assert.Single(this.dataContext.Ideas);
            Assert.Equal(dto.Title, savedIdea.Title);
            Assert.Equal(dto.Description, savedIdea.Description);
            Assert.Equal(this.RequestContext.Module.ModuleID, savedIdea.ModuleId);
            Assert.Equal(this.RequestContext.User.UserID, savedIdea.CreatedByUserId);
            Assert.Equal(this.dateTimeProvider.GetUtcNow(), savedIdea.CreatedAt);
        }

        [Fact]
        public async Task Save_Updates()
        {
            // Arrange
            var yesterdayIdea = this.fixture
                .Build<Idea>()
                .With(i => i.CreatedByUserId, this.RequestContext.User.UserID)
                .Create();
            this.dataContext.Ideas.Add(yesterdayIdea);
            await this.dataContext.SaveChangesAsync(this.token);
            var dto = new SaveIdeaDto
            {
                Id = yesterdayIdea.Id,
                Title = "Updated Title",
                Description = "Updated Description",
            };

            // Act
            var response = await this.ideaController.SaveIdea(dto, this.token);

            // Assert
            Assert.IsType<OkResult>(response);
            var savedIdea = Assert.Single(this.dataContext.Ideas);
            Assert.Equal(dto.Title, savedIdea.Title);
            Assert.Equal(dto.Description, savedIdea.Description);
            Assert.Equal(yesterdayIdea.CreatedAt, savedIdea.CreatedAt);
            Assert.Equal(dateTimeProvider.GetUtcNow(), savedIdea.UpdatedAt);
            Assert.Equal(this.RequestContext.User.UserID, savedIdea.UpdatedByUserId);
        }

        [Fact]
        public async Task Search_WithBlankTerm_ReturnsAll_ExceptDeleted()
        {
            // Arrange
            var ideas = this.fixture
                .Build<Idea>()
                .With(i => i.ModuleId, this.RequestContext.Module.ModuleID)
                .With(i => i.DeletedOn, default(DateTime?))
                .CreateMany(20);
            var deletedIdeas = this.fixture
                .Build<Idea>()
                .With(i => i.ModuleId, this.RequestContext.Module.ModuleID)
                .With(i => i.DeletedOn, this.dateTimeProvider.GetUtcNow().AddDays(-5))
                .CreateMany(5);
            var otherModuleIdeas = this.fixture
                .Build<Idea>()
                .With(i => i.ModuleId, this.RequestContext.Module.ModuleID + 10)
                .CreateMany(5);
            var allIdeas = ideas
                .Concat(deletedIdeas)
                .Concat(otherModuleIdeas)
                .OrderBy(_ => fixture.Create<int>()) // Randomize
                .ToList();
            this.dataContext.Ideas.AddRange(allIdeas);
            await this.dataContext.SaveChangesAsync(this.token);
            var dto = new SearchIdeasDto
            {
                OnlyMyIdeas = false,
                Page = 1,
                PageSize = 10,
                Query = string.Empty,
            };

            // Act
            var response = await this.ideaController.SearchIdeas(dto, this.token);

            // Assert
            var result = Assert.IsType<OkNegotiatedContentResult<PagedList<IdeaViewModel>>>(response);
            var vm = result.Content;
            Assert.Equal(20, vm.ResultCount);
            Assert.Equal(10, vm.Items.Count());
            Assert.Equal(2, vm.PageCount);
        }

        [Fact]
        public async Task Search_Respects_Query()
        {
            // Arrange
            var seoIdeas = fixture.Build<Idea>()
                .With(i => i.ModuleId, this.RequestContext.Module.ModuleID)
                .With(i => i.DeletedOn, default(DateTime?))
                .With(i => i.Title, () => $"SEO {fixture.Create<string>()}")
                .With(i => i.Description, () => $"SEO {fixture.Create<string>()}")
                .CreateMany(5);
            var seoDescriptions = fixture.Build<Idea>()
                .With(i => i.ModuleId, this.RequestContext.Module.ModuleID)
                .With(i => i.DeletedOn, default(DateTime?))
                .With(i => i.Title, () =>
                {
                    // Ensure it doesn't contain "SEO"
                    string t;
                    do t = fixture.Create<string>();
                    while (t.IndexOf("SEO", StringComparison.OrdinalIgnoreCase) >= 0);

                    return t;
                })
                .With(i => i.Description, () => $"{fixture.Create<string>()} seo")
                .CreateMany(5);
            var nonSeoIdeas = fixture.Build<Idea>()
                .With(i => i.ModuleId, this.RequestContext.Module.ModuleID)
                .With(i => i.DeletedOn, default(DateTime?))
                .With(i => i.Title, () =>
                {
                    // Ensure it doesn't contain "SEO"
                    string t;
                    do t = fixture.Create<string>();
                    while (t.IndexOf("SEO", StringComparison.OrdinalIgnoreCase) >= 0);

                    return t;
                })
                .With(i => i.Description, () =>
                {
                    // Ensure it doesn't contain "SEO"
                    string t;
                    do t = fixture.Create<string>();
                    while (t.IndexOf("SEO", StringComparison.OrdinalIgnoreCase) >= 0);

                    return t;
                })
                .CreateMany(5);
            var allIdeas = seoIdeas
                .Concat(seoDescriptions)
                .Concat(nonSeoIdeas)
                .OrderBy(_ => fixture.Create<int>()) // Randomize them
                .ToList();
            this.dataContext.Ideas.AddRange(allIdeas);
            await this.dataContext.SaveChangesAsync(this.token);
            var dto = new SearchIdeasDto
            {
                OnlyMyIdeas = false,
                Page = 1,
                PageSize = 10,
                Query = "SEO",
            };

            // Act
            var response = await this.ideaController.SearchIdeas(dto, this.token);


            // Assert
            var result = Assert.IsType<OkNegotiatedContentResult<PagedList<IdeaViewModel>>>(response);
            var vm = result.Content;
            Assert.Equal(10, vm.ResultCount);
            Assert.Equal(10, vm.Items.Count());
            Assert.Equal(1, vm.PageCount);
        }

        [Fact]
        public async Task GetsDetails()
        {
            // Arrange
            var idea = this.fixture.Build<Idea>()
                .With(i => i.ModuleId, this.RequestContext.Module.ModuleID)
                .With(i => i.CreatedByUserId, this.RequestContext.User.UserID + 1)
                .Create();
            this.dataContext.Ideas.Add(idea);
            await this.dataContext.SaveChangesAsync(this.token);

            // Act
            var response = await this.ideaController.GetIdeaDetails(idea.Id, this.token);

            // Assert
            var result = Assert.IsType<OkNegotiatedContentResult<IdeaDetailsViewModel>>(response);
            var vm = result.Content;
            Assert.Equal(idea.Title, vm.Title);
            Assert.Equal(idea.Description, vm.Description);
            Assert.False(vm.CanEdit);
        }

        [Fact]
        public async Task CanEditMyOwnQuestion()
        {
            // Arrange
            var idea = this.fixture.Build<Idea>()
                .With(i => i.ModuleId, this.RequestContext.Module.ModuleID)
                .With(i => i.CreatedByUserId, this.RequestContext.User.UserID)
                .Create();
            this.dataContext.Ideas.Add(idea);
            await this.dataContext.SaveChangesAsync(this.token);

            // Act
            var response = await this.ideaController.GetIdeaDetails(idea.Id, this.token);

            // Assert
            var result = Assert.IsType<OkNegotiatedContentResult<IdeaDetailsViewModel>>(response);
            var vm = result.Content;
            Assert.Equal(idea.Title, vm.Title);
            Assert.Equal(idea.Description, vm.Description);
            Assert.True(vm.CanEdit);
        }

        [Fact]
        public async Task AdminsCanAlwaysEdit()
        {
            // Arrange
            var idea = this.fixture.Build<Idea>()
                .With(i => i.ModuleId, this.RequestContext.Module.ModuleID)
                .With(i => i.CreatedByUserId, this.RequestContext.User.UserID + 1)
                .Create();
            this.dataContext.Ideas.Add(idea);
            var admin = Substitute.For<IUserInfo>();
            admin.UserID.Returns(this.RequestContext.User.UserID);
            admin.IsAdmin.Returns(true);
            this.RequestContext.User.Returns(admin);
            this.userController
                .GetUserById(this.RequestContext.PortalSettings.PortalId, this.RequestContext.User.UserID)
                .Returns(admin);
            await this.dataContext.SaveChangesAsync(this.token);

            // Act
            var response = await this.ideaController.GetIdeaDetails(idea.Id, this.token);

            // Assert
            var result = Assert.IsType<OkNegotiatedContentResult<IdeaDetailsViewModel>>(response);
            var vm = result.Content;
            Assert.Equal(idea.Title, vm.Title);
            Assert.Equal(idea.Description, vm.Description);
            Assert.True(vm.CanEdit);
        }

        [Fact]
        public async Task CannotDeleteIdeasIfNotYours()
        {
            // Arrange
            var idea = fixture.Build<Idea>()
                .With(i => i.CreatedByUserId, this.RequestContext.User.UserID + 1)
                .With(i => i.DeletedOn, default(DateTime?))
                .Create();
            this.dataContext.Ideas.Add(idea);
            await this.dataContext.SaveChangesAsync();
            var dto = new RequestIdeaDeletionDto
            {
                Id = idea.Id,
            };

            // Act
            var response = await this.ideaController.RequestIdeaDeletion(dto, token);

            // Assert
            var result = Assert.IsType<NegotiatedContentResult<ProblemDetails>>(response);
            var error = Assert.Single(result.Content.Errors);
            Assert.Equal(this.localizationService.ViewModel.ModelValidation.CannotEditIdeaNotYours, error);
        }

        [Fact]
        public async Task CanDeleteOwnIdea()
        {
            // Arrange
            var idea = fixture.Build<Idea>()
                .With(i => i.CreatedByUserId, this.RequestContext.User.UserID)
                .With(i => i.DeletedOn, default(DateTime?))
                .Create();
            this.dataContext.Ideas.Add(idea);
            await this.dataContext.SaveChangesAsync();
            var dto = new RequestIdeaDeletionDto
            {
                Id = idea.Id,
            };

            // Act
            var response = await this.ideaController.RequestIdeaDeletion(dto, token);

            // Assert
            var result = Assert.IsType<OkResult>(response);
        }

        [Fact]
        public async Task AdminsCanAlwaysDelete()
        {
            // Arrange
            var adminUserId = 64;
            var idea = this.fixture.Build<Idea>()
                .With(i => i.ModuleId, this.RequestContext.Module.ModuleID)
                .With(i => i.CreatedByUserId, this.RequestContext.User.UserID + 1)
                .Create();
            this.dataContext.Ideas.Add(idea);
            var admin = Substitute.For<IUserInfo>();
            admin.UserID.Returns(adminUserId);
            admin.IsAdmin.Returns(true);
            this.RequestContext.User.Returns(admin);
            this.userController
                .GetUserById(this.RequestContext.PortalSettings.PortalId, adminUserId)
                .Returns(admin);
            await this.dataContext.SaveChangesAsync(this.token);
            var dto = new RequestIdeaDeletionDto
            {
                Id = idea.Id,
            };

            // Act
            var response = await this.ideaController.RequestIdeaDeletion(dto, this.token);

            // Assert
            var result = Assert.IsType<OkResult>(response);
        }
    }
}
