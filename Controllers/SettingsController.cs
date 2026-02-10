// MIT License
// Copyright DNN Community

namespace DNN.Modules.UserVoice.Controllers
{
    using DNN.Modules.UserVoice.Controllers.Context;
    using DNN.Modules.UserVoice.Entities.Settings;
    using DotNetNuke.Entities.Modules.Settings;
    using DotNetNuke.Security;
    using DotNetNuke.Web.Api;
    using NSwag.Annotations;
    using System.Web.Http;

    /// <summary>
    /// REST APIs to manage the UserVoice module settings.
    /// </summary>
    public class SettingsController : ModuleApiController
    {
        private readonly IDnnRequestContext requestContext;
        private readonly IUserVoiceSettingsRepository userVoiceSettingsRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsController"/> class using the specified user voice settings.
        /// repository.
        /// </summary>
        /// <param name="requestContext">The request context providing access to the current HTTP request and related information. Cannot be null.</param>
        /// <param name="userVoiceSettingsRepository">The repository used to access and manage user voice settings. Cannot be null.</param>
        public SettingsController(
            IDnnRequestContext requestContext,
            IUserVoiceSettingsRepository userVoiceSettingsRepository)
        {
            this.requestContext = requestContext;
            this.userVoiceSettingsRepository = userVoiceSettingsRepository;
        }

        /// <summary>
        /// Retrieves the current configuration settings for the UserVoice module.
        /// </summary>
        /// <remarks>This method is accessible only to users with administrative privileges. Use this
        /// endpoint to obtain the module's settings for review or configuration purposes.</remarks>
        /// <returns>An <see cref="IHttpActionResult"/> containing the <see cref="UserVoiceSettings"/> for the module. The result
        /// has HTTP status code 200 (OK) if the settings are successfully retrieved.</returns>
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        [SwaggerResponse(System.Net.HttpStatusCode.OK, typeof(UserVoiceSettings), Description = "The settings of the UserVoice module.")]
        public IHttpActionResult GetSettings()
        {
            var module = this.requestContext.Module;
            var settings = this.userVoiceSettingsRepository.GetSettings(module);

            return this.Ok(settings);
        }

        /// <summary>
        /// Updates the module settings with the specified values.
        /// </summary>
        /// <remarks>Requires administrative access to update settings.</remarks>
        /// <param name="settings">The settings to apply to the UserVoice module. Cannot be null.</param>
        /// <returns>An HTTP response indicating the result of the update operation.</returns>
        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        [SwaggerDefaultResponse]
        public IHttpActionResult UpdateSettings(UserVoiceSettings settings)
        {
            this.userVoiceSettingsRepository.SaveSettings(this.requestContext.Module, settings);
            return this.Ok();
        }
    }
}
