// MIT License
// Copyright DNN Community

using DNN.Modules.UserVoice.Data;
using DNN.Modules.UserVoice.Data.Entities;
using DNN.Modules.UserVoice.Data.Repositories;
using DNN.Modules.UserVoice.Providers;
using DNN.Modules.UserVoice.Services.Items;
using DNN.Modules.UserVoice.Services.Localization;
using DotNetNuke.DependencyInjection;
using DotNetNuke.Instrumentation;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace DNN.Modules.UserVoice
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
            services.AddScoped<IItemRepository, ItemRepository>();
            services.AddScoped<IItemService, ItemService>();
            services.TryAddScoped(x => LoggerSource.Instance);
            services.AddScoped<ILocalizationService, LocalizationService>();
            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
            services.AddScoped<IValidator<CreateItemDTO>, CreateItemDtoValidator>();
            services.AddScoped<IValidator<UpdateItemDTO>, UpdateItemDtoValidator>();
        }
    }
}