// MIT License
// Copyright DNN Community

namespace DNN.Modules.UserVoice.Extensions
{
    using DNN.Modules.UserVoice.Entities.ProblemDetails;
    using FluentValidation.Results;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Web.Http;
    using System.Web.Http.Results;

    /// <summary>
    /// Extensions methods for <see cref="ApiController"/>.
    /// </summary>
    public static class ApiControllerExtensions
    {
        /// <summary>
        /// Creates a BadRequest response with a list of validation failures.
        /// </summary>
        /// <param name="controller">The controller in use.</param>
        /// <param name="failures">The failures.</param>
        /// <param name="title">The title of the problem (optional).</param>
        /// <returns>A BadRequest response message.</returns>
        public static IHttpActionResult BadRequest(
            this ApiController controller,
            IEnumerable<string> failures,
            string title = "Validation Error")
        {
            var message = string.Join(Environment.NewLine, failures);
            var problem = new ProblemDetails
            {
                Title = title,
                Instance = controller.Request?.RequestUri?.ToString(),
                Status = 400,
                Type = "https://httpstatuses.com/400",
                Detail = message,
                Errors = failures,
            };

            return new NegotiatedContentResult<ProblemDetails>(
                HttpStatusCode.BadRequest,
                problem,
                controller);
        }
    }
}
