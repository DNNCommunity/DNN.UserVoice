using DNN.Modules.UserVoice.Adapters;
using DNN.Modules.UserVoice.Controllers.Context;
using DNN.Modules.UserVoice.Data;
using DNN.Modules.UserVoice.Data.Repositories;
using DNN.Modules.UserVoice.Providers;
using DNN.Modules.UserVoice.Services.Ideas;
using DNN.Modules.UserVoice.Services.Ideas.DTOs;
using DNN.Modules.UserVoice.Services.Localization;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Web;
using Xunit;

namespace UnitTests
{
    public class StartupTests
    {
        [Fact]
        public void Startup_RegistersAllRequiredServices()
        {
            // Arrange
            IServiceCollection services = new ServiceCollection();
            var startup = new DNN.Modules.UserVoice.Startup();
            var types = new List<Type>()
            {
                // Data
                typeof(ModuleDbContext),
                typeof(IIdeaRepository),
                
                // Services
                typeof(ILocalizationService),
                typeof(IIdeaService),

                // Validators
                typeof(IValidator<SaveIdeaDtoWithContext>),
                typeof(IValidator<RequestIdeaDeletionDtoWithContext>),

                // Adapters
                typeof(IDateTimeProvider),
                typeof(IUserControllerAdapter),
                typeof(HttpContextBase),
                typeof(IDnnRequestContext)
            };

            // Act
            startup.ConfigureServices(services);

            // Assert
            types.ForEach(type =>
            {
                Assert.Contains(services, s => s.ServiceType == type);
            });

            Assert.Equal(types.Count, services.Count);
        }
    }
}