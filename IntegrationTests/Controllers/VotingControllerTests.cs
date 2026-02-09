using AutoFixture;
using DNN.Modules.UserVoice.Controllers;
using DNN.Modules.UserVoice.Data.Entities;
using DNN.Modules.UserVoice.Entities.ProblemDetails;
using DNN.Modules.UserVoice.Services.Voting.DTOs;
using DNN.Modules.UserVoice.Services.Voting.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Results;
using Xunit;

namespace IntegrationTests.Controllers
{
    public class VotingControllerTests : FakeControllerContext
    {
        private readonly VotingController votingController;

        public VotingControllerTests()
        {
            this.votingController = this.GetController<VotingController>();
        }

        [Fact]
        public async Task GetVotingStats()
        {
            // Arrange
            var ideas = this.fixture
                .Build<Idea>()
                .With(i => i.ModuleId, this.RequestContext.Module.ModuleID)
                .Without(i => i.UserVotes)
                .CreateMany(5);
            foreach (var idea in ideas)
            {
                var vote = new UserVote
                {
                    UserId = this.RequestContext.User.UserID,
                };
                idea.UserVotes.Add(vote);
            }
            this.dataContext.Ideas.AddRange(ideas);
            await this.dataContext.SaveChangesAsync(this.token);

            // Act
            var response = await this.votingController.GetVotingStats(this.token);

            // Assert
            var result = Assert.IsType<OkNegotiatedContentResult<VoteStatsViewModel>>(response);
            var vm = result.Content;
            Assert.Equal(5, vm.CurrentVotes);
            Assert.Equal(10, vm.MaxVotes);
        }

        [Fact]
        public async Task UpvoteIdea_FailsProperly()
        {
            // Arrange
            var ideas = this.fixture
                .Build<Idea>()
                .With(i => i.ModuleId, this.RequestContext.Module.ModuleID)
                .Without(i => i.UserVotes)
                .CreateMany(10);
            this.dataContext.Ideas.AddRange(ideas);
            foreach (var idea in ideas)
            {
                var vote = new UserVote
                {
                    UserId = this.RequestContext.User.UserID,
                };
                idea.UserVotes.Add(vote);
            }

            var newIdea = this.fixture
                .Build<Idea>()
                .With(i => i.ModuleId, this.RequestContext.Module.ModuleID)
                .Without(i => i.UserVotes)
                .Create();
            this.dataContext.Ideas.Add(newIdea);
            await this.dataContext.SaveChangesAsync(token);

            var dto = new VoteDto
            {
                IdeaId = newIdea.Id,
            };

            // Act
            var response = await this.votingController.UpvoteIdea(dto, this.token);

            // Assert
            var result = Assert.IsType<NegotiatedContentResult<ProblemDetails>>(response);
            var problemDetails = result.Content;
            Assert.Equal(this.localizationService.ViewModel.ModelValidation.ValidationErrorTitle, problemDetails.Title);
            var message = problemDetails.Errors.FirstOrDefault();
            Assert.Equal(this.localizationService.ViewModel.ModelValidation.VotesExhausted, message);
        }

        [Fact]
        public async Task Upvote_Saves()
        {
            // Arrange
            var idea = this.fixture
                .Build<Idea>()
                .With(i => i.ModuleId, this.RequestContext.Module.ModuleID)
                .Without(i => i.UserVotes)
                .Create();
            this.dataContext.Ideas.Add(idea);
            await this.dataContext.SaveChangesAsync(token);
            var dto = new VoteDto
            {
                IdeaId = idea.Id,
            };

            // Act
            var response = await this.votingController.UpvoteIdea(dto, this.token);

            // Assert
            var result = Assert.IsType<OkResult>(response);
            var savedIdea = this.dataContext.Ideas.Find(idea.Id);
            var vote = Assert.Single(savedIdea.UserVotes);
            Assert.Equal(this.RequestContext.User.UserID, vote.UserId);
        }

        [Fact]
        public async Task RemoveVote_Removes()
        {
            var idea = this.fixture
                .Build<Idea>()
                .With(i => i.ModuleId, this.RequestContext.Module.ModuleID)
                .Without(i => i.UserVotes)
                .Create();
            var vote = new UserVote
            {
                UserId = this.RequestContext.User.UserID,
            };
            idea.UserVotes.Add(vote);
            this.dataContext.Ideas.Add(idea);
            await this.dataContext.SaveChangesAsync(token);
            var dto = new VoteDto
            {
                IdeaId = idea.Id,
            };

            // Act
            var response = await this.votingController.RemoveVote(dto, this.token);

            // Assert
            Assert.IsType<OkResult>(response);
            var savedIdea = this.dataContext.Ideas.Find(idea.Id);
            Assert.Empty(savedIdea.UserVotes);
        }
    }
}
