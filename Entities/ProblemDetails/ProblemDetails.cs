// MIT License
// Copyright DNN Community

using System;
using System.Collections.Generic;

namespace DNN.Modules.UserVoice.Entities.ProblemDetails
{
    /// <summary>
    /// Implements RFC 7807 Problem Details for HTTP APIs.
    /// </summary>
    public class ProblemDetails
    {
        /// <summary>
        /// Gets or sets a URI that describes the type of problem.
        /// </summary>
        public string Type { get; set; } = "https://httpstatuses.com/400";

        /// <summary>
        /// Gets or sets the title of the problem.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the status code.
        /// </summary>
        public int Status { get; set; } = 400;

        /// <summary>
        /// Gets or sets a human readable string explaining the problem in more details.
        /// </summary>
        public string Detail { get; set; }

        /// <summary>
        /// Gets or sets the url that caused the instance of this problem.
        /// </summary>
        public string Instance { get; set; }

        /// <summary>
        /// Gets or sets the collection of error messages associated with the current operation or object.
        /// </summary>
        public IEnumerable<string> Errors { get; set; } = Array.Empty<string>();
    }
}
