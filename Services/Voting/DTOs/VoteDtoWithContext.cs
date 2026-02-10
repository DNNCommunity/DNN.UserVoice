// MIT License
// Copyright DNN Community

namespace DNN.Modules.UserVoice.Services.Voting.DTOs
{
    using DNN.Modules.UserVoice.Entities.Settings;

    /// <summary>
    /// Information about casting or removing a vote, including context about the user and item.
    /// </summary>
    public class VoteDtoWithContext
    {
        /// <summary>
        /// Gets or sets the ID of the acting user.
        /// </summary>
        public int ActingUserId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the module.
        /// </summary>
        public int ModuleId { get; set; }

        /// <summary>
        /// Gets or sets the user voice settings for the current session.
        /// </summary>
        public UserVoiceSettings Settings { get; set; }

        /// <summary>
        /// Gets or sets the information about the vote.
        /// </summary>
        public VoteDto Dto { get; set; }
    }
}
