// MIT License
// Copyright DNN Community

namespace DNN.Modules.UserVoice.Extensions
{
    using DNN.Modules.UserVoice.Adapters;
    using DNN.Modules.UserVoice.Controllers.Context;
    using DNN.Modules.UserVoice.Data;
    using DNN.Modules.UserVoice.Data.Repositories;
    using DNN.Modules.UserVoice.Providers;
    using DNN.Modules.UserVoice.Services.Ideas;
    using DNN.Modules.UserVoice.Services.Ideas.DTOs;
    using DNN.Modules.UserVoice.Services.Ideas.Validators;
    using DNN.Modules.UserVoice.Services.Localization;
    using FluentValidation;
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
            services.AddScoped<IIdeaRepository, IdeaRepository>();
        }

        /// <summary>
        /// Adds module-specific services to the specified <see cref="IServiceCollection"/> for dependency injection.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to which the module services will be added. Cannot be null.</param>
        public static void AddModuleServices(this IServiceCollection services)
        {
            services.AddScoped<ILocalizationService, LocalizationService>();
            services.AddScoped<IIdeaService, IdeaService>();
        }

        /// <summary>
        /// Registers validators required for the module validation.
        /// </summary>
        /// <param name="services">The service collection to which the validators will be added. Cannot be null.</param>
        public static void AddValidators(this IServiceCollection services)
        {
            services.AddScoped<IValidator<SaveIdeaDtoWithContext>, SaveIdeaDtoValidator>();
            services.AddScoped<IValidator<RequestIdeaDeletionDtoWithContext>, RequestIdeaDeletionDtoValidator>();
        }

        /// <summary>
        /// Registers adapter services.
        /// </summary>
        /// <param name="services">The service collection to which the adapter services will be added. Cannot be null.</param>
        public static void AddAdapters(this IServiceCollection services)
        {
            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
            services.AddScoped<IUserControllerAdapter, UserControllerAdapter>();
            services.AddScoped<IDnnRequestContext, DnnRequestContext>();
        }
    }
}
