// MIT License
// Copyright DNN Community

namespace DNN.Modules.UserVoice.Services.Ideas.DTOs
{
    /// <summary>
    /// The details about ideas search to perform.
    /// </summary>
    public class SearchIdeasDto
    {
        /// <summary>
        /// Gets or sets the current page number for paginated results.
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// Gets or sets the number of items to include on each page of results.
        /// </summary>
        public int PageSize { get; set; } = 20;

        /// <summary>
        /// Gets or sets the string to search for.
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether only ideas created by the current user are included.
        /// </summary>
        public bool OnlyMyIdeas { get; set; }
    }
}
