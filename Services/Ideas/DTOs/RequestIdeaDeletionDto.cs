// MIT License
// Copyright DNN Community

namespace DNN.Modules.UserVoice.Services.Ideas.DTOs
{
    /// <summary>
    /// Information required to soft-delete an idea.
    /// </summary>
    public class RequestIdeaDeletionDto
    {
        /// <summary>
        /// Gets or sets the ID of the idea to be deleted.
        /// </summary>
        public int Id { get; set; }
    }
}