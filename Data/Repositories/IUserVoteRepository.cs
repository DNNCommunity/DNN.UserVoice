// MIT License
// Copyright DNN Community

namespace DNN.Modules.UserVoice.Data.Repositories
{
    using DNN.Modules.UserVoice.Data.Entities;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides data-access methods for user votes.
    /// </summary>
    public interface IUserVoteRepository : IRepository<UserVote>
    {
        /// <summary>
        /// Asynchronously records a vote for the specified idea by the given user within the specified portal.
        /// </summary>
        /// <param name="userId">The identifier of the user submitting the vote. Must be a valid user ID.</param>
        /// <param name="ideaId">The identifier of the idea to receive the vote. Must correspond to an existing idea within the portal.</param>
        /// <param name="token">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation of recording the vote.</returns>
        Task AddVoteAsync(int userId, int ideaId, CancellationToken token);

        /// <summary>
        /// Asynchronously removes a user's vote from the specified idea.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose vote is to be removed.</param>
        /// <param name="ideaId">The unique identifier of the idea from which the vote will be removed.</param>
        /// <param name="token">A cancellation token that can be used to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous remove operation.</returns>
        Task RemoveVoteAsync(int userId, int ideaId, CancellationToken token);
    }
}
