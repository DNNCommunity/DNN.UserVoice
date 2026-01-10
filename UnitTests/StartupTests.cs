using DNN.Modules.UserVoice.Data;
using DNN.Modules.UserVoice.Data.Repositories;
using DNN.Modules.UserVoice.Providers;
using DNN.Modules.UserVoice.Services.Items;
using DNN.Modules.UserVoice.Services.Localization;
using DotNetNuke.Instrumentation;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
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
                typeof(ModuleDbContext),
                typeof(IItemRepository),
                typeof(IItemService),
                typeof(ILoggerSource),
                typeof(ILocalizationService),
                typeof(IDateTimeProvider),
                typeof(IValidator<CreateItemDTO>),
                typeof(IValidator<UpdateItemDTO>),
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