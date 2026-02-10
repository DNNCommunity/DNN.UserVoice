// MIT License
// Copyright DNN Community

namespace DNN.Modules.UserVoice.Services.Ideas.ViewModels
{
    /// <summary>
    /// Basic information about an idea.
    /// </summary>
    public class IdeaViewModel
    {
        /// <summary>
        /// Gets or sets the unique identifier for the idea.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the title associated with the idea.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the description associated with the object.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the number of votes associated with the idea.
        /// </summary>
        public int Votes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the current user has voted for this item.
        /// </summary>
        public bool IsVotedByUser { get; set; }
    }
}
