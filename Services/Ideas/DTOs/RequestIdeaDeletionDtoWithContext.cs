// MIT License
// Copyright DNN Community

namespace DNN.Modules.UserVoice.Services.Ideas.DTOs
{
    /// <summary>
    /// The DTO for requesting idea deletion, including contextual information.
    /// </summary>
    public class RequestIdeaDeletionDtoWithContext
    {
        /// <summary>
        /// Gets or sets the ID of the acting user.
        /// </summary>
        public int ActingUserId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the portal.
        /// </summary>
        public int PortalId { get; set; }

        /// <summary>
        /// Gets or sets the information required to request idea deletion.
        /// </summary>
        public RequestIdeaDeletionDto Dto { get; set; }
    }
}
