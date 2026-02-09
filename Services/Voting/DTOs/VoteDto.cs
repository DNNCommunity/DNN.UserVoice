// MIT License
// Copyright DNN Community

namespace DNN.Modules.UserVoice.Services.Voting.DTOs
{
    /// <summary>
    /// Information about the vote to cast or remove.
    /// </summary>
    public class VoteDto
    {
        /// <summary>
        /// Gets or sets the ID of the idea being voted on.
        /// </summary>
        public int IdeaId { get; set; }
    }
}