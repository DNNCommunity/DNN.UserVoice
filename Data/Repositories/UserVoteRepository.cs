// MIT License
// Copyright DNN Community

namespace DNN.Modules.UserVoice.Data.Repositories
{
    using DNN.Modules.UserVoice.Data.Entities;
    using DNN.Modules.UserVoice.Providers;
    using System.Data.Entity;
    using System.Threading;
    using System.Threading.Tasks;

    /// <inheritdoc cref="IUserVoteRepository"/>
    internal class UserVoteRepository : Repository<UserVote>, IUserVoteRepository
    {
        private readonly ModuleDbContext dataContext;
        private readonly IDateTimeProvider dateTimeProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserVoteRepository"/> class.
        /// </summary>
        /// <param name="context">The underlying data-context to use.</param>
        /// <param name="dateTimeProvider">Provides access to system time.</param>
        public UserVoteRepository(ModuleDbContext context, IDateTimeProvider dateTimeProvider)
            : base(context, dateTimeProvider)
        {
            this.dataContext = context;
            this.dateTimeProvider = dateTimeProvider;
        }

        /// <inheritdoc/>
        public async Task AddVoteAsync(int userId, int ideaId, CancellationToken token)
        {
            var idea = await this.dataContext.Ideas
                .SingleOrDefaultAsync(i => i.Id == ideaId, token);
            if (idea == null)
            {
                return;
            }

            var exists = await this.dataContext.UserVotes.AnyAsync(
                v => v.UserId == userId && v.Idea.Id == ideaId,
                token);

            if (exists)
            {
                return;
            }

            var now = this.dateTimeProvider.GetUtcNow();
            var vote = new UserVote
            {
                UserId = userId,
                Idea = idea,
                CreatedAt = now,
                UpdatedAt = now,
                CreatedByUserId = userId,
                UpdatedByUserId = userId,
            };
            this.dataContext.UserVotes.Add(vote);
            await this.dataContext.SaveChangesAsync(token);
        }

        /// <inheritdoc/>
        public async Task RemoveVoteAsync(int userId, int ideaId, CancellationToken token)
        {
            var vote = await this.dataContext.UserVotes
                .FirstOrDefaultAsync(v => v.UserId == userId && v.Idea.Id == ideaId, token);

            if (vote == null)
            {
                return;
            }

            this.dataContext.UserVotes.Remove(vote);
            await this.dataContext.SaveChangesAsync(token);
        }
    }
}
