// MIT License
// Copyright DNN Community

using DotNetNuke.Web.Api;

namespace DNN.Modules.DnnUserVoice.Controllers
{
    /// <summary>
    /// Implements the Dnn IServiceRouteMapper to register this module routes.
    /// </summary>
    public class ServiceRouteMapper : IServiceRouteMapper
    {
        /// <inheritdoc/>
        public void RegisterRoutes(IMapRoute mapRouteManager)
        {
            mapRouteManager?.MapHttpRoute("DNN_UserVoice", "default", "{controller}/{action}", new[] { typeof(ServiceRouteMapper).Namespace });
        }
    }
}