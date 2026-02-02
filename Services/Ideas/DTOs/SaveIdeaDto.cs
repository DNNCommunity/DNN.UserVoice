// MIT License
// Copyright DNN Community

namespace DNN.Modules.UserVoice.Services.Ideas.DTOs
{
    /// <summary>
    /// Represents the data transfer object used to submit or update an idea for saving operations.
    /// </summary>
    public class SaveIdeaDto
    {
        /// <summary>
        /// Gets or sets the unique identifier of the idea (only required for updates).
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the title associated with the idea.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the descriptive text associated with the idea.
        /// </summary>
        public string Description { get; set; }
    }
}