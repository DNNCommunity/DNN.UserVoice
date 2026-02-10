// MIT License
// Copyright DNN Community

namespace DNN.Modules.UserVoice.Services.Voting.ViewModels
{
    /// <summary>
    /// Represents voting statistics for a user.
    /// </summary>
    public class VoteStatsViewModel
    {
        /// <summary>
        /// Gets or sets the total number of votes already cast by the user.
        /// </summary>
        public int CurrentVotes { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of votes allowed for the user.
        /// </summary>
        public int MaxVotes { get; set; }
    }
}
