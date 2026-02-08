// MIT License
// Copyright DNN Community

namespace DNN.Modules.UserVoice.Controllers.Context
{
    using DNN.Modules.UserVoice.Entities.Settings;
    using DotNetNuke.Abstractions.Portals;
    using DotNetNuke.Abstractions.Users;
    using DotNetNuke.Entities.Modules;
    using DotNetNuke.Entities.Tabs;
    using DotNetNuke.Entities.Users;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Web;

    /// <inheritdoc cref="IDnnRequestContext" />
    [ExcludeFromCodeCoverage] // This class is mostly about orchestrating calls to DNN APIs, which are well-tested. The logic in this class is simple and low-risk, and the DNN APIs it calls are already tested.
    internal class DnnRequestContext : IDnnRequestContext
    {
        private readonly IModuleController moduleController;
        private readonly ITabController tabController;
        private readonly IUserController userController;
        private readonly IUserVoiceSettingsRepository userVoiceSettingsRepository;
        private readonly HttpContextBase httpContext;

        private readonly int moduleId;
        private readonly int tabId;
        private readonly Lazy<ModuleInfo> moduleInfo;
        private readonly Lazy<TabInfo> tabInfo;
        private readonly Lazy<IPortalSettings> portalSettings;
        private readonly Lazy<IUserInfo> userInfo;
        private readonly Lazy<UserVoiceSettings> userVoiceSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="DnnRequestContext"/> class.
        /// </summary>
        /// <param name="moduleController">Provides services related to modules.</param>
        /// <param name="tabController">Provides services related to tabs (pages).</param>
        /// <param name="userController">Provides services related to users.</param>
        /// <param name="userVoiceSettingsRepository">Provides access to the UserVoice module settings.</param>
        /// <param name="httpContext">Provides access to the current HTTP context.</param>
        public DnnRequestContext(
            IModuleController moduleController,
            ITabController tabController,
            IUserController userController,
            IUserVoiceSettingsRepository userVoiceSettingsRepository,
            HttpContextBase httpContext)
        {
            this.moduleController = moduleController;
            this.tabController = tabController;
            this.userController = userController;
            this.userVoiceSettingsRepository = userVoiceSettingsRepository;
            this.httpContext = httpContext;

            this.moduleId = this.GetRequestIntParam("ModuleId");
            this.moduleInfo = new Lazy<ModuleInfo>(this.InitModuleInfo);
            this.tabId = this.GetRequestIntParam("TabId");
            this.portalSettings = new Lazy<IPortalSettings>(this.InitPortalSettings);
            this.tabInfo = new Lazy<TabInfo>(this.InitTabInfo);
            this.userInfo = new Lazy<IUserInfo>(this.InitUserInfo);
            this.userVoiceSettings = new Lazy<UserVoiceSettings>(() => this.userVoiceSettingsRepository.GetSettings(this.Module));
        }

        /// <inheritdoc/>
        public IPortalSettings PortalSettings => this.portalSettings.Value;

        /// <inheritdoc/>
        public IUserInfo User => this.userInfo.Value;

        /// <inheritdoc/>
        public TabInfo Tab => this.tabInfo.Value;

        /// <inheritdoc/>
        public ModuleInfo Module => this.moduleInfo.Value;

        /// <inheritdoc/>
        public UserVoiceSettings UserVoiceSettings => this.userVoiceSettings.Value;

        private int GetRequestIntParam(string key)
        {
            var request = this.httpContext?.Request;
            if (request == null)
            {
                return -1;
            }

            string value = request.Headers[key] ?? request.QueryString[key];
            return int.TryParse(value, out var result) ? result : -1;
        }

        private ModuleInfo InitModuleInfo()
        {
            if (this.moduleId == -1)
            {
                return null;
            }

            if (this.tabId == -1)
            {
                return null;
            }

            var localModuleInfo = this.moduleController.GetModule(this.moduleId, this.tabId, false);
            if (localModuleInfo != null && localModuleInfo.ModuleID > 0)
            {
                return localModuleInfo;
            }

            return null;
        }

        private IPortalSettings InitPortalSettings()
        {
            if (this.httpContext == null)
            {
                return null;
            }

            return this.httpContext.Items["PortalSettings"] as IPortalSettings;
        }

        private TabInfo InitTabInfo()
        {
            if (this.tabId == -1)
            {
                return null;
            }

            if (this.portalSettings.Value == null)
            {
                return null;
            }

            return this.tabController.GetTab(this.tabId, this.portalSettings.Value.PortalId);
        }

        private IUserInfo InitUserInfo()
        {
            if (this.portalSettings.Value == null)
            {
                return null;
            }

            return this.userController.GetUser(
                this.portalSettings.Value.PortalId,
                this.portalSettings.Value.UserId);
        }
    }
}
