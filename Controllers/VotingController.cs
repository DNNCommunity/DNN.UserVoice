// MIT License
// Copyright DNN Community

namespace DNN.Modules.UserVoice.Controllers
{
    using DNN.Modules.UserVoice.Controllers.Context;
    using DNN.Modules.UserVoice.Entities.ProblemDetails;
    using DNN.Modules.UserVoice.Extensions;
    using DNN.Modules.UserVoice.Services.Localization;
    using DNN.Modules.UserVoice.Services.Voting;
    using DNN.Modules.UserVoice.Services.Voting.DTOs;
    using DNN.Modules.UserVoice.Services.Voting.ViewModels;
    using DotNetNuke.Web.Api;
    using NSwag.Annotations;
    using System;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http;

    /// <summary>
    /// REST APIs to manage voting features.
    /// </summary>
    public class VotingController : ModuleApiController
    {
        private readonly IDnnRequestContext dnnRequestContext;
        private readonly IVotingService votingService;
        private readonly LocalizationViewModel localization;

        /// <summary>
        /// Initializes a new instance of the <see cref="VotingController"/> class.
        /// </summary>
        /// <param name="dnnRequestContext">Provides information about the web request.</param>
        /// <param name="votingService">Provides services to manage voting features.</param>
        /// <param name="localizationService">Provides localization services.</param>
        public VotingController(
            IDnnRequestContext dnnRequestContext,
            IVotingService votingService,
            ILocalizationService localizationService)
        {
            this.dnnRequestContext = dnnRequestContext;
            this.votingService = votingService;
            this.localization = localizationService.ViewModel;
        }

        /// <summary>
        /// Retrieves aggregated voting statistics for the current user.
        /// </summary>
        /// <param name="token">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>An HTTP response containing the voting statistics if the operation is successful.</returns>
        [HttpGet]
        [DnnAuthorize]
        [SwaggerResponse(HttpStatusCode.OK, typeof(VoteStatsViewModel), Description = "The voting statistics were retrieved successfully.")]
        public async Task<IHttpActionResult> GetVotingStats(CancellationToken token)
        {
            var vm = await this.votingService.GetVoteStatsAsync(
                this.dnnRequestContext.Module.ModuleID,
                this.dnnRequestContext.User.UserID,
                this.dnnRequestContext.UserVoiceSettings,
                token);

            return this.Ok(vm);
        }

        /// <summary>
        /// Registers an upvote for the specified idea on behalf of the current user.
        /// </summary>
        /// <param name="dto">An object containing the details of the idea to upvote. Must not be null.</param>
        /// <param name="token">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>An HTTP response indicating the result of the upvote operation. Returns 200 OK if the upvote is successful;
        /// otherwise, returns 400 Bad Request with problem details.</returns>
        [HttpPost]
        [DnnAuthorize]
        [SwaggerDefaultResponse]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(ProblemDetails))]
        public async Task<IHttpActionResult> UpvoteIdea(VoteDto dto, CancellationToken token)
        {
            var dtoWithContext = new VoteDtoWithContext
            {
                ActingUserId = this.dnnRequestContext.User.UserID,
                ModuleId = this.dnnRequestContext.Module.ModuleID,
                Settings = this.dnnRequestContext.UserVoiceSettings,
                Dto = dto,
            };
            var result = await this.votingService.UpvoteAsync(dtoWithContext, token);

            return result.Match<IHttpActionResult>(
                ok => this.Ok(),
                error => this.BadRequest(error.Value, this.localization.ModelValidation.ValidationErrorTitle));
        }

        /// <summary>
        /// Removes a user's vote from the specified idea.
        /// </summary>
        /// <param name="dto">An object containing the details of the vote to be removed. Cannot be null.</param>
        /// <param name="token">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>An HTTP response indicating the result of the remove vote operation.</returns>
        [HttpPost]
        [DnnAuthorize]
        [SwaggerDefaultResponse]
        public async Task<IHttpActionResult> RemoveVote(VoteDto dto, CancellationToken token)
        {
            var dtoWithContext = new VoteDtoWithContext
            {
                ActingUserId = this.dnnRequestContext.User.UserID,
                ModuleId = this.dnnRequestContext.Module.ModuleID,
                Settings = this.dnnRequestContext.UserVoiceSettings,
                Dto = dto,
            };
            await this.votingService.RemoveVoteAsync(dtoWithContext, token);

            return this.Ok();
        }
    }
}
