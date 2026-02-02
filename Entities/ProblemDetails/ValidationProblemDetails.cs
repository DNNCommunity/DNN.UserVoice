// MIT License
// Copyright DNN Community

namespace DNN.Modules.UserVoice.Entities.ProblemDetails
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents details about a validation error, including information about the specific validation problems.
    /// </summary>
    public class ValidationProblemDetails : ProblemDetails
    {
        /// <summary>
        /// Gets or sets the collection of validation error messages associated with the current operation.
        /// </summary>
        public IEnumerable<string> Errors { get; set; } = new HashSet<string>();
    }
}
