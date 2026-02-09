using AutoFixture;
using DNN.Modules.UserVoice.Data.Entities;
using DNN.Modules.UserVoice.Data.Repositories;
using DNN.Modules.UserVoice.Providers;
using NSubstitute;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.Data.Repositories
{
    public class UserVoteRepositoryTests : FakeDataContext
    {
        private readonly CancellationToken token;
        private readonly Fixture fixture;
        private readonly IDateTimeProvider dateTimeProvider;

        private readonly IUserVoteRepository userVoteRepository;

        public UserVoteRepositoryTests()
        {
            this.token = new CancellationToken();
            this.fixture = new Fixture();
            this.dateTimeProvider = Substitute.For<IDateTimeProvider>();

            this.userVoteRepository = new UserVoteRepository(
                this.dataContext,
                this.dateTimeProvider);
        }

        [Fact]
        public async Task AddVote_Adds()
        {
            // Arrange
            var userId = fixture.Create<int>();
            var idea = new Idea
            {
                Title = fixture.Create<string>(),
                Description = fixture.Create<string>(),
            };
            this.dataContext.Ideas.Add(idea);
            await this.dataContext.SaveChangesAsync(token);

            // Act
            await this.userVoteRepository.AddVoteAsync(userId, idea.Id, token);

            // Assert
            Assert.Equal(1, this.dataContext.UserVotes.Count());
            var vote = this.dataContext.UserVotes.FirstOrDefault();
            Assert.NotNull(vote);
            Assert.Equal(userId, vote.UserId);
            Assert.Equal(idea.Id, vote.Idea.Id);
        }

        [Fact]
        public async Task AddVote_MissingIdeaDoesNothing()
        {
            // Arrange
            var userId = fixture.Create<int>();
            var ideaId = fixture.Create<int>();

            // Act
            await this.userVoteRepository.AddVoteAsync(userId, ideaId, token);

            // Assert
            Assert.Empty(this.dataContext.UserVotes);
        }

        [Fact]
        public async Task AddVote_PreventsDuplicates()
        {
            // Arrange
            var userId = fixture.Create<int>();
            var idea = new Idea
            {
                Title = fixture.Create<string>(),
                Description = fixture.Create<string>(),
            };
            this.dataContext.Ideas.Add(idea);
            await this.dataContext.SaveChangesAsync(token);
            var existingVote = new UserVote
            {
                UserId = userId,
                Idea = idea
            };
            this.dataContext.UserVotes.Add(existingVote);
            await this.dataContext.SaveChangesAsync(token);

            // Act
            await this.userVoteRepository.AddVoteAsync(userId, idea.Id, token);

            // Assert
            Assert.Equal(1, this.dataContext.UserVotes.Count());
        }

        [Fact]
        public async Task RemoveVote_Removes()
        {
            // Arrange
            var userId = fixture.Create<int>();
            var idea = new Idea
            {
                Title = fixture.Create<string>(),
                Description = fixture.Create<string>(),
            };
            this.dataContext.Ideas.Add(idea);
            await this.dataContext.SaveChangesAsync(token);
            var vote = new UserVote
            {
                UserId = userId,
                Idea = idea
            };
            this.dataContext.UserVotes.Add(vote);
            await this.dataContext.SaveChangesAsync(token);

            // Act
            await this.userVoteRepository.RemoveVoteAsync(userId, idea.Id, token);

            // Assert
            Assert.Empty(this.dataContext.UserVotes);
        }

        [Fact]
        public async Task Remove_WithMissing_DoesNothing()
        {
            // Arrange
            var userId = fixture.Create<int>();
            var ideaId = fixture.Create<int>();
            
            // Act
            await this.userVoteRepository.RemoveVoteAsync(userId, ideaId, token);
            
            // Assert
            Assert.Empty(this.dataContext.UserVotes);
        }
    }
}
