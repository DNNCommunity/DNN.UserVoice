// MIT License
// Copyright DNN Community

namespace DNN.Modules.UserVoice.Controllers
{
    using DNN.Modules.UserVoice.Controllers.Context;
    using DNN.Modules.UserVoice.Data.Repositories;
    using DNN.Modules.UserVoice.Entities.ProblemDetails;
    using DNN.Modules.UserVoice.Extensions;
    using DNN.Modules.UserVoice.Services.Ideas;
    using DNN.Modules.UserVoice.Services.Ideas.DTOs;
    using DNN.Modules.UserVoice.Services.Localization;
    using DotNetNuke.Security;
    using DotNetNuke.Web.Api;
    using NSwag.Annotations;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http;

    /// <summary>
    /// REST APIs to manage ideas.
    /// </summary>
    public class IdeaController : ModuleApiController
    {
        private readonly IDnnRequestContext requestContext;
        private readonly IIdeaService ideaService;
        private readonly LocalizationViewModel localization;

        /// <summary>
        /// Initializes a new instance of the <see cref="IdeaController"/> class.
        /// </summary>
        /// <param name="requestContext">Provides information about the current HTTP request.</param>
        /// <param name="ideaService">Provides services to manage ideas.</param>
        /// <param name="localizationService">Provides localization services.</param>
        public IdeaController(
            IDnnRequestContext requestContext,
            IIdeaService ideaService,
            ILocalizationService localizationService)
        {
            this.requestContext = requestContext;
            this.ideaService = ideaService;
            this.localization = localizationService.ViewModel;
        }

        /// <summary>
        /// Saves (creates or updates) an idea.
        /// </summary>
        /// <param name="dto">The information to save.</param>
        /// <param name="token">A token that can be used to abort the request early.</param>
        /// <returns>Either a success or an error with details.</returns>
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, typeof(void), Description = "The idea was saved successfully.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(ProblemDetails), Description = "An error has occured.")]
        public async Task<IHttpActionResult> SaveIdea(SaveIdeaDto dto, CancellationToken token)
        {
            var dtoWithContext = new SaveIdeaDtoWithContext
            {
                ModuleId = this.requestContext.Module.ModuleID,
                ActingUserId = this.requestContext.User.UserID,
                PortalId = this.requestContext.PortalSettings.PortalId,
                Dto = dto,
            };

            var result = await this.ideaService.SaveIdeaAsync(dtoWithContext, token);

            return result.Match<IHttpActionResult>(
                success => this.Ok(),
                error => this.BadRequest(error.Value, this.localization.ModelValidation.ValidationErrorTitle));
        }

        /// <summary>
        /// Searches for ideas that match the specified criteria and returns a paged list of results.
        /// </summary>
        /// <param name="dto">An object containing the search criteria to filter ideas. Cannot be null.</param>
        /// <param name="token">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>An HTTP response containing a paged list of ideas that match the search criteria. The list may be empty if
        /// no ideas are found.</returns>
        [AllowAnonymous]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, typeof(PagedList<IdeaViewModel>), Description = "The ideas were retrieved successfully.")]
        public async Task<IHttpActionResult> SearchIdeas(SearchIdeasDto dto, CancellationToken token)
        {
            var dtoWithContext = new SearchIdeasDtoWithContext
            {
                ModuleId = this.requestContext.Module.ModuleID,
                ActingUserId = this.requestContext.User.UserID,
                Dto = dto,
            };
            var result = await this.ideaService.SearchIdeasAsync(dtoWithContext, token);
            return this.Ok(result);
        }

        /// <summary>
        /// Submits a request to delete an idea based on the provided deletion details.
        /// </summary>
        /// <param name="dto">An object containing the details required to request deletion of an idea. Cannot be null.</param>
        /// <param name="token">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>An HTTP response indicating the result of the deletion request. Returns status 200 (OK) if the request was
        /// submitted successfully; otherwise, returns status 400 (Bad Request) with error details.</returns>
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, typeof(void), Description = "The idea deletion request was submitted successfully.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(ProblemDetails), Description = "An error has occured.")]
        public async Task<IHttpActionResult> RequestIdeaDeletion(RequestIdeaDeletionDto dto, CancellationToken token)
        {
            var dtoWithContext = new RequestIdeaDeletionDtoWithContext
            {
                ActingUserId = this.requestContext.User.UserID,
                PortalId = this.requestContext.PortalSettings.PortalId,
                Dto = dto,
            };

            var result = await this.ideaService.SoftDeleteIdea(dtoWithContext, token);
            return result.Match<IHttpActionResult>(
                success => this.Ok(),
                error => this.BadRequest(error.Value, this.localization.ModelValidation.ValidationErrorTitle));
        }

        /// <summary>
        /// Retrieves the details of a specific idea by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the idea to retrieve. Must be a valid idea ID.</param>
        /// <param name="token">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>An action result containing the details of the requested idea if found; otherwise, an appropriate error
        /// response.</returns>
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        [SwaggerResponse(HttpStatusCode.OK, typeof(IdeaDetailsViewModel), Description = "The idea details were retrieved successfully.")]
        public async Task<IHttpActionResult> GetIdeaDetails(int id, CancellationToken token)
        {
            var vm = await this.ideaService.GetIdeaDetailsAsync(id, this.requestContext.User, token);
            return this.Ok(vm);
        }
    }
}
