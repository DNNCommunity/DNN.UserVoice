// MIT License
// Copyright DNN Community

namespace DNN.Modules.UserVoice.Services.Voting
{
    using DNN.Modules.UserVoice.Entities.Settings;
    using DNN.Modules.UserVoice.Services.Voting.DTOs;
    using DNN.Modules.UserVoice.Services.Voting.ViewModels;
    using OneOf;
    using OneOf.Types;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides services related to voting.
    /// </summary>
    public interface IVotingService
    {
        /// <summary>
        /// Asynchronously retrieves voting statistics for a user.
        /// </summary>
        /// <param name="moduleId">The unique identifier of the module for which to retrieve vote statistics.</param>
        /// <param name="userId">The unique identifier of the user for whom to retrieve vote statistics.</param>
        /// <param name="settings">The current module settings.</param>
        /// <param name="token">A cancellation token that can be used to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a VoteStatsViewModel with the
        /// user's voting statistics.</returns>
        Task<VoteStatsViewModel> GetVoteStatsAsync(int moduleId, int userId, UserVoiceSettings settings, CancellationToken token);

        /// <summary>
        /// Removes a vote if it exists.
        /// </summary>
        /// <param name="dto">Details about the vote to remove.</param>
        /// <param name="token">A token that can be used to abort the request early.</param>
        /// <returns>An awaitable task.</returns>
        Task RemoveVoteAsync(VoteDtoWithContext dto, CancellationToken token);

        /// <summary>
        /// Upvotes an idea.
        /// </summary>
        /// <param name="dto">The details of the vote with the context.</param>
        /// <param name="token">A token that can be used to abort the request early.</param>
        /// <returns>Either a Success or a list of validation errors.</returns>
        Task<OneOf<Success, Error<IEnumerable<string>>>> UpvoteAsync(VoteDtoWithContext dto, CancellationToken token);
    }
}
