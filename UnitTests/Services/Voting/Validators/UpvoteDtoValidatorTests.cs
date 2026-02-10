using AutoFixture;
using DNN.Modules.UserVoice.Data.Entities;
using DNN.Modules.UserVoice.Data.Repositories;
using DNN.Modules.UserVoice.Entities.Settings;
using DNN.Modules.UserVoice.Services.Localization;
using DNN.Modules.UserVoice.Services.Voting.DTOs;
using DNN.Modules.UserVoice.Services.Voting.Validators;
using FluentValidation.TestHelper;
using NSubstitute;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.Services.Voting.Validators
{
    public class UpvoteDtoValidatorTests
    {
        private readonly Fixture fixture;
        private readonly CancellationToken token;
        private readonly ILocalizationService localizationService;
        private readonly IIdeaRepository ideaRepository;
        private readonly IUserVoteRepository userVoteRepository;

        private readonly UpvoteDtoValidator upvoteDtoValidator;

        public UpvoteDtoValidatorTests()
        {
            this.fixture = new Fixture();
            this.token = new CancellationToken();
            this.localizationService = Substitute.For<ILocalizationService>();
            var localizationViewModel = new LocalizationViewModel();
            this.localizationService.ViewModel.Returns(localizationViewModel);
            this.ideaRepository = Substitute.For<IIdeaRepository>();
            this.userVoteRepository = Substitute.For<IUserVoteRepository>();
            this.upvoteDtoValidator = new UpvoteDtoValidator(
                this.localizationService,
                this.ideaRepository,
                this.userVoteRepository);
        }

        [Fact]
        public async Task RequiresUserId()
        {
            // Arrange
            var dtoWithContext = new VoteDtoWithContext
            {
                Dto = new VoteDto
                {
                    IdeaId = this.fixture.Create<int>()
                },
            };

            // Act
            var result = await this.upvoteDtoValidator.TestValidateAsync(dtoWithContext);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ActingUserId);
        }

        [Fact]
        public async Task RequiresDto()
        {
            // Arrange
            var dtoWithContext = new VoteDtoWithContext
            {
                ActingUserId = this.fixture.Create<int>()
            };

            // Act
            var result = await this.upvoteDtoValidator.TestValidateAsync(dtoWithContext);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Dto);
        }

        [Fact]
        public async Task RequiresIdeaId()
        {
            // Arrange
            var dtoWithContext = new VoteDtoWithContext
            {
                ActingUserId = this.fixture.Create<int>(),
                Dto = new VoteDto()
            };

            // Act
            var result = await this.upvoteDtoValidator.TestValidateAsync(dtoWithContext);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Dto.IdeaId)
                .WithErrorMessage(this.localizationService.ViewModel.ModelValidation.IdeaNotFound);
        }

        [Fact]
        public async Task IdeaMustExist()
        {
            // Arrange
            var dto = new VoteDtoWithContext
            {
                ActingUserId = this.fixture.Create<int>(),
                Dto = new VoteDto
                {
                    IdeaId = this.fixture.Create<int>()
                }
            };

            // Act
            var result = await this.upvoteDtoValidator.TestValidateAsync(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Dto.IdeaId)
                .WithErrorMessage(this.localizationService.ViewModel.ModelValidation.IdeaNotFound);
        }

        [Fact]
        public async Task MustRespectVoteLimits()
        {
            // Arrange
            var dto = new VoteDtoWithContext
            {
                ActingUserId = this.fixture.Create<int>(),
                Settings = new UserVoiceSettings
                {
                    BaseVotesPerUser = 10
                },
                ModuleId = this.fixture.Create<int>(),
                Dto = new VoteDto
                {
                    IdeaId = this.fixture.Create<int>()
                }
            };
            var existingIdea = this.fixture
                .Build<Idea>()
                .With(i => i.Id, dto.Dto.IdeaId)
                .Without(i => i.UserVotes)
                .Create();
            this.ideaRepository
                .GetByIdAsync(dto.Dto.IdeaId)
                .Returns(existingIdea);
            this.userVoteRepository
                .CountVotesForUserAsync(dto.ModuleId, dto.ActingUserId, this.token)
                .Returns(10);

            // Act
            var result = await this.upvoteDtoValidator.TestValidateAsync(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x)
                .WithErrorMessage(this.localizationService.ViewModel.ModelValidation.VotesExhausted);
        }
    }
}
