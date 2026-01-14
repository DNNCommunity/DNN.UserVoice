using DNN.Modules.UserVoice.Data.Repositories;
using DNN.Modules.UserVoice.Providers;
using NSubstitute;
using Xunit;

namespace UnitTests.Data.Repositories
{
    public class IdeaRepositoryTests : FakeDataContext
    {
        private readonly IIdeaRepository ideaRepository;
        private readonly IDateTimeProvider dateTimeProvider;

        public IdeaRepositoryTests()
        {
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
    }
}
