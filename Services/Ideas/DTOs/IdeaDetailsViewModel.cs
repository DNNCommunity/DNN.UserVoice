// MIT License
// Copyright DNN Community

namespace DNN.Modules.UserVoice.Services.Ideas.DTOs
{
    /// <summary>
    /// Represents the view model for displaying details related to an idea.
    /// </summary>
    public class IdeaDetailsViewModel
    {
        /// <summary>
        /// Gets or sets the unique identifier for the entity.
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

        /// <summary>
        /// Gets or sets a value indicating whether the current user has permission to edit the idea.
        /// </summary>
        public bool CanEdit { get; set; }
    }
}
