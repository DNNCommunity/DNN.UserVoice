// MIT License
// Copyright DNN Community

namespace DNN.Modules.UserVoice
{
    using DNN.Modules.UserVoice.Extensions;
    using DotNetNuke.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Implements the IDnnStartup interface to run at application start.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class Startup : IDnnStartup
    {
        /// <summary>
        /// Registers the dependencies for injection.
        /// </summary>
        /// <param name="services">The services collection.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddModuleData();
            services.AddModuleServices();
            services.AddValidators();
            services.AddAdapters();
        }
    }
}