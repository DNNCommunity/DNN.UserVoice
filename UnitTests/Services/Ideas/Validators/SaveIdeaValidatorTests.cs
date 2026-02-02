using AutoFixture;
using DNN.Modules.UserVoice.Adapters;
using DNN.Modules.UserVoice.Common.Utilities;
using DNN.Modules.UserVoice.Data.Entities;
using DNN.Modules.UserVoice.Data.Repositories;
using DNN.Modules.UserVoice.Services.Ideas.DTOs;
using DNN.Modules.UserVoice.Services.Ideas.Validators;
using DNN.Modules.UserVoice.Services.Localization;
using DotNetNuke.Abstractions.Users;
using FluentValidation;
using FluentValidation.TestHelper;
using NSubstitute;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.Services.Ideas.Validators
{
    public class SaveIdeaValidatorTests
    {
        // Utilities
        private Fixture fixture;
        private CancellationToken token;

        // Dependencies
        private readonly LocalizationViewModel localizationViewModel;
        private readonly ILocalizationService localizationService;
        private readonly IIdeaRepository ideaRepository;
        private readonly IUserControllerAdapter userController;

        // System Under Test
        private readonly IValidator<SaveIdeaDtoWithContext> saveAssetDtoValidator;

        public SaveIdeaValidatorTests()
        {
            this.fixture = new Fixture();
            this.token = new CancellationToken();

            this.localizationViewModel = new LocalizationViewModel();
            this.localizationService = Substitute.For<ILocalizationService>();
            this.localizationService.ViewModel.Returns(this.localizationViewModel);
            this.ideaRepository = Substitute.For<IIdeaRepository>();
            this.userController = Substitute.For<IUserControllerAdapter>();

            this.saveAssetDtoValidator = new SaveIdeaDtoValidator(
                this.localizationService,
                this.ideaRepository,
                this.userController);
        }

        [Fact]
        public async Task RequiresUserId()
        {
            // Arrange
            var dto = this.fixture.Build<SaveIdeaDtoWithContext>()
                .With(x => x.ActingUserId, 0)
                .Create();

            // Act
            var result = await this.saveAssetDtoValidator.TestValidateAsync(dto, cancellationToken: this.token);

            // Assert
            result
                .ShouldHaveValidationErrorFor(x => x.ActingUserId)
                .WithErrorMessage(this.localizationViewModel.ModelValidation.UserRequired);
        }

        [Fact]
        public async Task CannotEditIdeaThatIsNotYours()
        {
            // Arrange
            var existingIdea = new Idea
            {
                CreatedByUserId = 2,
                Description = "Existing Description",
                Title = "Existing Title",
                Id = 123,
                ModuleId = 10,
            };
            this.ideaRepository
                .GetByIdAsync(existingIdea.Id, this.token)
                .Returns(existingIdea);
            var dto = new SaveIdeaDtoWithContext
            {
                ActingUserId = 234,
                ModuleId = 10,
                Dto = new SaveIdeaDto
                {
                    Id = existingIdea.Id,
                    Title = "New Title",
                    Description = "New Description",
                },
            };
            this.ideaRepository
                .IsTitleUniqueAsync(
                    dto.Dto.Title,
                    dto.ModuleId,
                    Arg.Any<int>(),
                    this.token)
                .Returns(true);

            // Act
            var result = await this.saveAssetDtoValidator.TestValidateAsync(dto, cancellationToken: this.token);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ActingUserId);
        }

        [Fact]
        public async Task CanEditOwnIdea()
        {
            // Arrange
            var userId = 111;
            var existingIdea = new Idea
            {
                CreatedByUserId = userId,
                Description = "Existing Description",
                Title = "Existing Title",
                Id = 123,
                ModuleId = 10,
            };
            this.ideaRepository
                .GetByIdAsync(existingIdea.Id, this.token)
                .Returns(existingIdea);
            var dto = new SaveIdeaDtoWithContext
            {
                ActingUserId = userId,
                ModuleId = 10,
                Dto = new SaveIdeaDto
                {
                    Id = existingIdea.Id,
                    Title = "New Title",
                    Description = "New Description",
                },
            };
            this.ideaRepository
                .IsTitleUniqueAsync(
                    dto.Dto.Title,
                    dto.ModuleId,
                    Arg.Any<int>(),
                    this.token)
                .Returns(true);

            // Act
            var result = await this.saveAssetDtoValidator.TestValidateAsync(dto, cancellationToken: this.token);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.ActingUserId);
        }

        [Fact]
        public async Task AdminCanAlwaysEdit()
        {
            // Arrange
            var existingIdea = new Idea
            {
                CreatedByUserId = 123,
                Description = "Existing Description",
                Title = "Existing Title",
                Id = 123,
                ModuleId = 10,
            };
            this.ideaRepository
                .GetByIdAsync(existingIdea.Id, this.token)
                .Returns(existingIdea);
            var dto = new SaveIdeaDtoWithContext
            {
                ActingUserId = 234,
                ModuleId = 10,
                Dto = new SaveIdeaDto
                {
                    Id = existingIdea.Id,
                    Title = "New Title",
                    Description = "New Description",
                },
            };
            this.ideaRepository
                .IsTitleUniqueAsync(
                    dto.Dto.Title,
                    dto.ModuleId,
                    Arg.Any<int>(),
                    this.token)
                .Returns(true);
            var user = Substitute.For<IUserInfo>();
            user.IsAdmin.Returns(true);
            this.userController
                .GetUserById(Arg.Any<int>(), dto.ActingUserId)
                .Returns(user);

            // Act
            var result = await this.saveAssetDtoValidator.TestValidateAsync(dto, cancellationToken: this.token);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.ActingUserId);
        }

        [Fact]
        public async Task RequiresModuleId()
        {
            // Arrange
            var dto = this.fixture.Build<SaveIdeaDtoWithContext>()
                .With(x => x.ModuleId, 0)
                .Create();

            // Act
            var result = await this.saveAssetDtoValidator.TestValidateAsync(dto, cancellationToken: this.token);

            // Assert
            result
                .ShouldHaveValidationErrorFor(x => x.ModuleId)
                .WithErrorMessage(this.localizationViewModel.ModelValidation.ModuleRequired);
        }

        [Fact]
        public async Task RequiresTitle()
        {
            // Arrange
            var dto = this.fixture.Build<SaveIdeaDto>()
                .With(x => x.Title, string.Empty)
                .Create();
            var dtoWithContext = this.fixture.Build<SaveIdeaDtoWithContext>()
                .With(x => x.Dto, dto)
                .Create();

            // Act
            var result = await this.saveAssetDtoValidator.TestValidateAsync(dtoWithContext, cancellationToken: this.token);

            // Assert
            result
                .ShouldHaveValidationErrorFor(x => x.Dto.Title)
                .WithErrorMessage(this.localizationViewModel.ModelValidation.TitleRequired);
        }

        [Fact]
        public async Task TitleTooLongFails()
        {
            // Arrange
            var maxLength = MaxLength.Of<Idea>(x => x.Title);
            var longTitle = new string('A', maxLength + 1);
            var dto = this.fixture.Build<SaveIdeaDto>()
                .With(x => x.Title, longTitle)
                .Create();
            var dtoWithContext = this.fixture.Build<SaveIdeaDtoWithContext>()
                .With(x => x.Dto, dto)
                .Create();

            // Act
            var result = await this.saveAssetDtoValidator.TestValidateAsync(dtoWithContext, cancellationToken: this.token);

            // Assert
            var expectedMessage = string.Format(
                this.localizationViewModel.ModelValidation.TitleTooLong,
                maxLength);
            result
                .ShouldHaveValidationErrorFor(x => x.Dto.Title)
                .WithErrorMessage(expectedMessage);
        }

        [Fact]
        public async Task TitleMustBeUnique()
        {
            // Arrange
            var dto = this.fixture.Build<SaveIdeaDto>()
                .With(x => x.Title, "Unique Title")
                .Create();
            var dtoWithContext = this.fixture.Build<SaveIdeaDtoWithContext>()
                .With(x => x.Dto, dto)
                .Create();
            this.ideaRepository
                .IsTitleUniqueAsync(
                    dto.Title,
                    dtoWithContext.ModuleId,
                    Arg.Any<int>(),
                    this.token)
                .Returns(false);

            // Act
            var result = await this.saveAssetDtoValidator.TestValidateAsync(dtoWithContext, cancellationToken: this.token);

            // Assert
            result
                .ShouldHaveValidationErrorFor(x => x.Dto.Title)
                .WithErrorMessage(this.localizationViewModel.ModelValidation.TitleUnique);
        }

        [Fact]
        public async Task RequiresDescription()
        {
            // Arrange
            var dto = this.fixture.Build<SaveIdeaDto>()
                .With(x => x.Description, string.Empty)
                .Create();
            var dtoWithContext = this.fixture.Build<SaveIdeaDtoWithContext>()
                .With(x => x.Dto, dto)
                .Create();

            // Act
            var result = await this.saveAssetDtoValidator.TestValidateAsync(dtoWithContext, cancellationToken: this.token);

            // Assert
            result
                .ShouldHaveValidationErrorFor(x => x.Dto.Description)
                .WithErrorMessage(this.localizationViewModel.ModelValidation.DescriptionRequired);
        }

        [Fact]
        public async Task DescriptionTooLongFails()
        {
            // Arrange
            var maxLength = MaxLength.Of<Idea>(x => x.Description);
            var longDescription = new string('A', maxLength + 1);
            var dto = this.fixture.Build<SaveIdeaDto>()
                .With(x => x.Description, longDescription)
                .Create();
            var dtoWithContext = this.fixture.Build<SaveIdeaDtoWithContext>()
                .With(x => x.Dto, dto)
                .Create();

            // Act
            var result = await this.saveAssetDtoValidator.TestValidateAsync(dtoWithContext, cancellationToken: this.token);

            // Assert
            var expectedMessage = string.Format(
                this.localizationViewModel.ModelValidation.DescriptionTooLong,
                maxLength);
            result
                .ShouldHaveValidationErrorFor(x => x.Dto.Description)
                .WithErrorMessage(expectedMessage);
        }

        [Fact]
        public async Task ValidDtoPassesValidation()
        {
            // Arrange
            var dtoWithContext = new SaveIdeaDtoWithContext
            {
                ActingUserId = 1,
                ModuleId = 2,
                Dto = new SaveIdeaDto
                {
                    Title = "Valid Title",
                    Description = "Valid Description",
                }
            };
            this.ideaRepository
                .IsTitleUniqueAsync(
                    dtoWithContext.Dto.Title,
                    dtoWithContext.ModuleId,
                    Arg.Any<int>(),
                    this.token)
                .Returns(true);
            
            // Act
            var result = await this.saveAssetDtoValidator.TestValidateAsync(dtoWithContext, cancellationToken: this.token);
            
            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
