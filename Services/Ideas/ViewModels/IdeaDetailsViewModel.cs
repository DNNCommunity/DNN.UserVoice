// MIT License
// Copyright DNN Community

using System;

namespace DNN.Modules.UserVoice.Services.Ideas.ViewModels
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

        /// <summary>
        /// Gets or sets the display name of the user who created the entity.
        /// </summary>
        public string CreatedByUserDisplayName { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the idea was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets a value indicating how long ago the idea was created, in a human-readable format (e.g., "2 hours ago").
        /// </summary>
        public string CreatedSince { get; set; }

        /// <summary>
        /// Gets or sets the number of votes associated with the idea.
        /// </summary>
        public int Votes { get; set; }
    }
}
