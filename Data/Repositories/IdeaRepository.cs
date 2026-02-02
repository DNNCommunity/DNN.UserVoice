// MIT License
// Copyright DNN Community

namespace DNN.Modules.UserVoice.Data.Repositories
{
    using DNN.Modules.UserVoice.Data.Entities;
    using DNN.Modules.UserVoice.Providers;
    using System.Data.Entity;
    using System.Threading;
    using System.Threading.Tasks;

    /// <inheritdoc cref="IIdeaRepository"/>
    internal class IdeaRepository : Repository<Idea>, IIdeaRepository
    {
        private readonly ModuleDbContext dataContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="IdeaRepository"/> class.
        /// provider.
        /// </summary>
        /// <param name="context">The database context to be used for data access operations. Cannot be null.</param>
        /// <param name="dateTimeProvider">The provider used to obtain the current date and time. Cannot be null.</param>
        public IdeaRepository(ModuleDbContext context, IDateTimeProvider dateTimeProvider)
            : base(context, dateTimeProvider)
        {
            this.dataContext = context;
        }

        /// <inheritdoc/>
        public async Task<bool> IsTitleUniqueAsync(string title, int moduleId, int ideaId, CancellationToken token)
        {
            var exists = await this.dataContext.Ideas
                .AnyAsync(
                    x =>
                        x.Title.Trim() == title.Trim() &&
                        x.ModuleId == moduleId &&
                        x.Id != ideaId,
                    token);
            return !exists;
        }
    }
}
