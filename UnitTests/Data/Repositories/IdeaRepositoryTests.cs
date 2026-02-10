using AutoFixture;
using DNN.Modules.UserVoice.Data.Entities;
using DNN.Modules.UserVoice.Data.Repositories;
using DNN.Modules.UserVoice.Providers;
using NSubstitute;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.Data.Repositories
{
    public class IdeaRepositoryTests : FakeDataContext
    {
        private readonly CancellationToken token;
        private readonly Fixture fixture;
        private readonly IDateTimeProvider dateTimeProvider;

        private readonly IIdeaRepository ideaRepository;

        public IdeaRepositoryTests()
        {
            this.token = new CancellationToken();
            this.fixture = new Fixture();
            this.dateTimeProvider = Substitute.For<IDateTimeProvider>();
            this.ideaRepository = new IdeaRepository(
                this.dataContext,
                this.dateTimeProvider);
        }

        [Fact]
        public void Constructs()
        {
            Assert.NotNull(this.ideaRepository);
        }

        [Fact]
        public async Task IsTitleUnique_WhenExistsButForOtherModule()
        {
            // Arrange
            var title = fixture.Create<string>();
            var otherModuleId = fixture.Create<int>();
            var thisModuleId = fixture.Create<int>();
            var otherIdea = fixture.Build<DNN.Modules.UserVoice.Data.Entities.Idea>()
                .With(x => x.Title, title)
                .With(x => x.ModuleId, otherModuleId)
                .Without(x => x.UserVotes)
                .Create();
            this.dataContext.Ideas.Add(otherIdea);
            await this.dataContext.SaveChangesAsync();

            // Act
            bool isUnique = await this.ideaRepository.IsTitleUniqueAsync(title, thisModuleId, 123, this.token);

            // Assert
            Assert.True(isUnique);
        }

        [Fact]
        public async Task IsTitleUnique_WhenExistsForSameModule()
        {
            // Arrange
            var title = fixture.Create<string>();
            var moduleId = fixture.Create<int>();
            var existingIdea = fixture.Build<DNN.Modules.UserVoice.Data.Entities.Idea>()
                .With(x => x.Title, title)
                .With(x => x.ModuleId, moduleId)
                .Without(x => x.UserVotes)
                .Create();
            this.dataContext.Ideas.Add(existingIdea);
            await this.dataContext.SaveChangesAsync();
            
            // Act
            bool isUnique = await this.ideaRepository.IsTitleUniqueAsync(title, moduleId, 123, this.token);
            
            // Assert
            Assert.False(isUnique);
        }

        [Fact]
        public async Task IsTitleUnique_WhenItExistsForTheIdeaBeingEdited()
        {
            // Arrange
            var title = fixture.Create<string>();
            var moduleId = fixture.Create<int>();
            var existingIdea = fixture.Build<DNN.Modules.UserVoice.Data.Entities.Idea>()
                .With(x => x.Title, title)
                .With(x => x.ModuleId, moduleId)
                .Without(x => x.UserVotes)
                .Create();
            this.dataContext.Ideas.Add(existingIdea);
            await this.dataContext.SaveChangesAsync();
            
            // Act
            bool isUnique = await this.ideaRepository.IsTitleUniqueAsync(title, moduleId, existingIdea.Id, this.token);
            
            // Assert
            Assert.True(isUnique);

        }
    }
}
