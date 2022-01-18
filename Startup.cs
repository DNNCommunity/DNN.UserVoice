// MIT License
// Copyright DNN Community

using DNN.Modules.DnnUserVoice.Data;
using DNN.Modules.DnnUserVoice.Data.Entities;
using DNN.Modules.DnnUserVoice.Data.Repositories;
using DNN.Modules.DnnUserVoice.Services;
using DotNetNuke.DependencyInjection;
using DotNetNuke.Services.Localization;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace DNN.Modules.DnnUserVoice
{
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
            services.AddScoped<ModuleDbContext, ModuleDbContext>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IItemService>(provider => new ItemService(provider.GetService<IRepository<Item>>()));
            services.AddScoped<ILoggingService, LoggingService>();
            services.AddScoped<ILocalizationService, LocalizationService>();
        }
    }
}