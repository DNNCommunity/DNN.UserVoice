// MIT License
// Copyright DNN Community

using DotNetNuke.Abstractions.Portals;
using DotNetNuke.Abstractions.Users;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Tabs;

namespace DNN.Modules.UserVoice.Controllers.Context
{
    /// <summary>
    /// Provides information about the current request context.
    /// </summary>
    public interface IDnnRequestContext
    {
        /// <summary>
        /// Gets the current portal settings.
        /// </summary>
        IPortalSettings PortalSettings { get; }

        /// <summary>
        /// Gets the current user information.
        /// </summary>
        IUserInfo User { get; }

        /// <summary>
        /// Gets the current page information.
        /// </summary>
        TabInfo Tab { get; }

        /// <summary>
        /// Gets the current module information.
        /// <remarks>Will be null unless the module and tab IDs are provided in the request.</remarks>
        /// </summary>
        ModuleInfo Module { get; }
    }
}
