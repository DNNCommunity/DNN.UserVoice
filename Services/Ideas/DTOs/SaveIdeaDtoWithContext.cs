// MIT License
// Copyright DNN Community

namespace DNN.Modules.UserVoice.Services.Ideas.DTOs
{
    /// <summary>
    /// Represents a data transfer object for saving an idea along with its associated context information.
    /// </summary>
    public class SaveIdeaDtoWithContext
    {
        /// <summary>
        /// Gets or sets the identifier of the user performing the current action.
        /// </summary>
        public int ActingUserId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the module.
        /// </summary>
        public int ModuleId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the portal.
        /// </summary>
        public int PortalId { get; set; }

        /// <summary>
        /// Gets or sets the data transfer object containing the details required to save an idea.
        /// </summary>
        public SaveIdeaDto Dto { get; set; }
    }
}
