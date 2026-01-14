// MIT License
// Copyright DNN Community

namespace DNN.Modules.UserVoice.Extensions
{
    using DNN.Modules.UserVoice.Data;
    using DNN.Modules.UserVoice.Providers;
    using DNN.Modules.UserVoice.Services.Localization;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Provides extension methods for configuring and managing services in an <see cref="IServiceCollection"/>.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds module-specific data layer (dbcontext, repositories, etc.)
        /// </summary>
        /// <param name="services">The service collection to configure.</param>
        public static void AddModuleData(this IServiceCollection services)
        {
            services.AddScoped<ModuleDbContext, ModuleDbContext>();
        }

        /// <summary>
        /// Adds module-specific services to the specified <see cref="IServiceCollection"/> for dependency injection.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to which the module services will be added. Cannot be null.</param>
        public static void AddModuleServices(this IServiceCollection services)
        {
            services.AddScoped<ILocalizationService, LocalizationService>();
            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        }
    }
}
