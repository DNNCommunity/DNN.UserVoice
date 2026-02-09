using DNN.Modules.UserVoice.Adapters;
using DNN.Modules.UserVoice.Controllers.Context;
using DNN.Modules.UserVoice.Data;
using DNN.Modules.UserVoice.Data.Repositories;
using DNN.Modules.UserVoice.Entities.Settings;
using DNN.Modules.UserVoice.Providers;
using DNN.Modules.UserVoice.Services.Ideas;
using DNN.Modules.UserVoice.Services.Ideas.DTOs;
using DNN.Modules.UserVoice.Services.Localization;
using DNN.Modules.UserVoice.Services.Voting;
using DNN.Modules.UserVoice.Services.Voting.DTOs;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
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
                typeof(IUserVoiceSettingsRepository),
                typeof(IUserVoteRepository),
                
                // Services
                typeof(ILocalizationService),
                typeof(IIdeaService),
                typeof(IVotingService),

                // Validators
                typeof(IValidator<SaveIdeaDtoWithContext>),
                typeof(IValidator<RequestIdeaDeletionDtoWithContext>),
                typeof(IValidator<VoteDtoWithContext>),

                // Adapters
                typeof(IDateTimeProvider),
                typeof(IUserControllerAdapter),
                typeof(HttpContextBase),
                typeof(IDnnRequestContext)
            };

            // Act
            startup.ConfigureServices(services);

            // Assert
            var registeredTypes = services.Select(s => s.ServiceType).ToList();
            var missingServices = types.Where(t => !registeredTypes.Contains(t)).ToList();
            var extraServices = registeredTypes.Where(t => !types.Contains(t)).ToList();

            // Check for missing services
            Assert.True(
                missingServices.Count == 0,
                $"Missing service registrations:{Environment.NewLine}" +
                string.Join(Environment.NewLine, missingServices.Select(t => $"  - {t.FullName}")));

            // Check for extra services
            Assert.True(
                extraServices.Count == 0,
                $"Extra service registrations not in expected list:{Environment.NewLine}" +
                string.Join(Environment.NewLine, extraServices.Select(t => $"  - {t.FullName}")));

            // Verify exact count match
            Assert.Equal(types.Count, services.Count);
        }
    }
}