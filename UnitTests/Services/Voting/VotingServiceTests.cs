using AutoFixture;
using DNN.Modules.UserVoice.Data.Repositories;
using DNN.Modules.UserVoice.Entities.Settings;
using DNN.Modules.UserVoice.Services.Voting;
using DNN.Modules.UserVoice.Services.Voting.DTOs;
using FluentValidation;
using NSubstitute;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.Services.Voting
{
    public class VotingServiceTests
    {
        private readonly CancellationToken token;
        private readonly Fixture fixture;
        
        private readonly InlineValidator<VoteDtoWithContext> upvoteDtoValidator;
        private readonly IUserVoteRepository userVoteRepository;

        private readonly IVotingService votingService;

        public VotingServiceTests()
        {
            this.token = new CancellationToken();
            this.fixture = new Fixture();

            this.upvoteDtoValidator = new InlineValidator<VoteDtoWithContext>();
            this.userVoteRepository = Substitute.For<IUserVoteRepository>();

            this.votingService = new VotingService(
                this.upvoteDtoValidator,
                this.userVoteRepository);
        }

        [Fact]
        public async Task Upvote_Validates()
        {
            // Arrange
            var dto = fixture
                .Build<VoteDtoWithContext>()
                .Without(d => d.ActingUserId)
                .Create();
            var message = fixture.Create<string>();
            this.upvoteDtoValidator
                .RuleFor(x => x.ActingUserId)
                .GreaterThan(0)
                .WithMessage(message);

            // Act
            var result = await this.votingService.UpvoteAsync(dto, token);

            // Assert
            result.Switch(
                success => Assert.Fail("Excpected a failure"),
                error =>
                {
                    var receivedMessage = Assert.Single(error.Value);
                    Assert.Equal(message, receivedMessage);
                });
        }

        [Fact]
        public async Task Upvote_SavesVote()
        {
            // Arrange
            var dto = fixture.Create<VoteDtoWithContext>();
            
            // Act
            var result = await this.votingService.UpvoteAsync(dto, token);
            
            // Assert
            result.Switch(
                success => this.userVoteRepository
                .Received(1)
                .AddVoteAsync(dto.ActingUserId, dto.Dto.IdeaId, this.token),
                error => Assert.Fail("Expected a success"));
        }

        [Fact]
        public async Task RemoveVote_Removes()
        {
            // Arrange
            var dto = fixture.Create<VoteDtoWithContext>();

            // Act
            await this.votingService.RemoveVoteAsync(dto, token);

            // Assert
            await this.userVoteRepository
                .Received(1)
                .RemoveVoteAsync(dto.ActingUserId, dto.Dto.IdeaId, this.token);
        }

        [Fact]
        public async Task GetsVoteStats()
        {
            // Arrange
            var moduleId = fixture.Create<int>();
            var userId = fixture.Create<int>();
            this.userVoteRepository
                .CountVotesForUserAsync(moduleId, userId, this.token)
                .Returns(5);
            var settings = fixture.Create<UserVoiceSettings>();

            // Act
            var result = await this.votingService
                .GetVoteStatsAsync(moduleId, userId, settings, token);

            // Assert
            Assert.Equal(5, result.CurrentVotes);
            Assert.Equal(settings.BaseVotesPerUser, result.MaxVotes);
        }
    }
}
