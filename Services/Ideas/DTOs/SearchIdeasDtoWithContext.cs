// MIT License
// Copyright DNN Community

namespace DNN.Modules.UserVoice.Services.Ideas.DTOs
{
    /// <summary>
    /// Details about ideas search with context.
    /// </summary>
    public class SearchIdeasDtoWithContext
    {
        /// <summary>
        /// Gets or sets the unique identifier for the module to search for.
        /// </summary>
        public int ModuleId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user performing the current action.
        /// </summary>
        public int ActingUserId { get; set; }

        /// <summary>
        /// Gets or sets the details about the search to perform.
        /// </summary>
        public SearchIdeasDto Dto { get; set; }
    }
}
