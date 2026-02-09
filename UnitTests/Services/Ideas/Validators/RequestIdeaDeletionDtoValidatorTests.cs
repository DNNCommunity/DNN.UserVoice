using AutoFixture;
using DNN.Modules.UserVoice.Adapters;
using DNN.Modules.UserVoice.Data.Entities;
using DNN.Modules.UserVoice.Data.Repositories;
using DNN.Modules.UserVoice.Services.Ideas.DTOs;
using DNN.Modules.UserVoice.Services.Ideas.Validators;
using DNN.Modules.UserVoice.Services.Localization;
using DotNetNuke.Abstractions.Users;
using FluentValidation;
using FluentValidation.TestHelper;
using NSubstitute;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.Services.Ideas.Validators
{
    public class RequestIdeaDeletionDtoValidatorTests
    {
        private readonly Fixture fixture;
        private readonly ILocalizationService localizationService;
        private readonly LocalizationViewModel localization;
        private readonly IIdeaRepository ideaRepository;
        private readonly IUserControllerAdapter userController;

        private readonly IValidator<RequestIdeaDeletionDtoWithContext> validator;

        public RequestIdeaDeletionDtoValidatorTests()
        {
            this.fixture = new Fixture();
            this.localizationService = Substitute.For<ILocalizationService>();
            this.localization = this.fixture.Create<LocalizationViewModel>();
            this.localizationService.ViewModel.Returns(this.localization);
            this.ideaRepository = Substitute.For<IIdeaRepository>();
            this.userController = Substitute.For<IUserControllerAdapter>();
            this.validator = new RequestIdeaDeletionDtoValidator(
                this.localizationService,
                this.ideaRepository,
                this.userController);
        }

        [Fact]
        public async Task RequiresActingUserId()
        {
            // Arrange
            var dto = this.fixture
                .Build<RequestIdeaDeletionDtoWithContext>()
                .With(x => x.ActingUserId, 0)
                .Create();

            // Act
            var result = await this.validator.TestValidateAsync(dto);
            
            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ActingUserId)
                .WithErrorMessage(this.localization.ModelValidation.UserRequired);
        }

        [Fact]
        public async Task RequiresPortalId()
        {
            // Arrange
            var dto = this.fixture
                .Build<RequestIdeaDeletionDtoWithContext>()
                .With(x => x.PortalId, -1)
                .Create();
            
            // Act
            var result = await this.validator.TestValidateAsync(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.PortalId);
        }

        [Fact]
        public async Task RequiresDto()
        {
            // Arrange
            var dto = this.fixture
                .Build<RequestIdeaDeletionDtoWithContext>()
                .With(x => x.Dto, default(RequestIdeaDeletionDto))
                .Create();

            // Act
            var result = await this.validator.TestValidateAsync(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Dto);
        }

        [Fact]
        public async Task IdeaIdRequired()
        {
            // Arrange
            var dto = new RequestIdeaDeletionDtoWithContext
            {
                ActingUserId = 1,
                PortalId = 1,
                Dto = new RequestIdeaDeletionDto
                {
                    Id = 0
                }
            };

            // Act
            var result = await this.validator.TestValidateAsync(dto);
            
            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Dto.Id)
                .WithErrorMessage(this.localization.ModelValidation.IdGreaterThanZero);
        }

        [Fact]
        public async Task IdeaMustExist()
        {
            // Arrange
            var dto = fixture.Create<RequestIdeaDeletionDtoWithContext>();
            this.ideaRepository.GetByIdAsync(dto.Dto.Id)
                .Returns(default(Idea));

            // Act
            var result = await this.validator.TestValidateAsync(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Dto.Id)
                .WithErrorMessage(this.localization.ModelValidation.IdeaNotFound);
        }

        [Fact]
        public async Task CannotDeleteIdeasOfOtherUsers()
        {
            // Arrange
            var userId1 = 123;
            var userId2 = 234;
            var idea = this.fixture
                .Build<Idea>()
                .With(x => x.CreatedByUserId, userId1)
                .Without(x => x.UserVotes)
                .Create();
            this.ideaRepository.GetByIdAsync(idea.Id)
                .Returns(idea);
            var dto = new RequestIdeaDeletionDtoWithContext
            {
                ActingUserId = userId2,
                PortalId = 1,
                Dto = new RequestIdeaDeletionDto
                {
                    Id = idea.Id
                }
            };
            var user = Substitute.For<IUserInfo>();
            user.UserID.Returns(dto.ActingUserId);
            user.IsAdmin.Returns(false);
            this.userController.GetUserById(dto.PortalId, dto.ActingUserId)
                .Returns(user);

            // Act
            var result = await this.validator.TestValidateAsync(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ActingUserId)
                .WithErrorMessage(this.localization.ModelValidation.CannotEditIdeaNotYours);
        }

        [Fact]
        public async Task UserCanDeleteHisOwnIdeas()
        {
            // Arrange
            var userId = 123;
            var idea = this.fixture
                .Build<Idea>()
                .With(x => x.CreatedByUserId, userId)
                .Without(x => x.UserVotes)
                .Create();
            this.ideaRepository.GetByIdAsync(idea.Id)
                .Returns(idea);
            var dto = new RequestIdeaDeletionDtoWithContext
            {
                ActingUserId = userId,
                PortalId = 1,
                Dto = new RequestIdeaDeletionDto
                {
                    Id = idea.Id
                }
            };
            var user = Substitute.For<IUserInfo>();
            user.UserID.Returns(dto.ActingUserId);
            user.IsAdmin.Returns(false);
            this.userController.GetUserById(dto.PortalId, dto.ActingUserId)
                .Returns(user);

            // Act
            var result = await this.validator.TestValidateAsync(dto);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.ActingUserId);
        }

        [Fact]
        public async Task AdminsCanAlwaysDeleteIdea()
        {
            // Arrange
            var userId = 123;
            var adminId = 234;
            var idea = this.fixture
                .Build<Idea>()
                .With(x => x.CreatedByUserId, userId)
                .Without(x => x.UserVotes)
                .Create();
            this.ideaRepository.GetByIdAsync(idea.Id)
                .Returns(idea);
            var admin = Substitute.For<IUserInfo>();
            admin.UserID.Returns(adminId);
            admin.IsAdmin.Returns(true);
            this.userController.GetUserById(Arg.Any<int>(), adminId)
                .Returns(admin);
            var dto = new RequestIdeaDeletionDtoWithContext
            {
                ActingUserId = admin.UserID,
                PortalId = 1,
                Dto = new RequestIdeaDeletionDto
                {
                    Id = idea.Id,
                }
            };

            // Act
            var result = await this.validator.TestValidateAsync(dto);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public async Task NoUserCantDelete()
        {
            // Arrange
            var userId = 123;
            var idea = this.fixture
                .Build<Idea>()
                .With(x => x.CreatedByUserId, userId)
                .Without(x => x.UserVotes)
                .Create();
            this.ideaRepository.GetByIdAsync(idea.Id)
                .Returns(idea);
            this.userController.GetUserById(Arg.Any<int>(), userId)
                .Returns((IUserInfo)null);
            var dto = new RequestIdeaDeletionDtoWithContext
            {
                ActingUserId = userId,
                PortalId = 1,
                Dto = new RequestIdeaDeletionDto
                {
                    Id = idea.Id,
                }
            };

            // Act
            var result = await this.validator.TestValidateAsync(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ActingUserId);
        }
    }
}
