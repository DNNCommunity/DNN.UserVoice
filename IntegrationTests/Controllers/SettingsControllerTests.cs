using DNN.Modules.UserVoice.Controllers;
using DNN.Modules.UserVoice.Entities.Settings;
using DotNetNuke.Entities.Modules;
using NSubstitute;
using System.Web.Http.Results;
using Xunit;

namespace IntegrationTests.Controllers
{
    public class SettingsControllerTests : FakeControllerContext
    {
        private readonly SettingsController settingsController;

        public SettingsControllerTests()
        {
            this.settingsController = this.GetController<SettingsController>();
        }

        [Fact]
        public void GetSettings_ReturnsOk()
        {
            // Act
            var response = this.settingsController.GetSettings();

            // Assert
            var result = Assert.IsType<OkNegotiatedContentResult<UserVoiceSettings>>(response);
            var settings = result.Content;
            Assert.Equal(10, settings.BaseVotesPerUser); // default setting.
        }

        [Fact]
        public void UpdateSettings_Saves()
        {
            // Arrange
            var newSettings = new UserVoiceSettings
            {
                BaseVotesPerUser = 20
            };

            // Act
            var response = this.settingsController.UpdateSettings(newSettings);

            // Assert
            Assert.IsType<OkResult>(response);
            this.userVoiceSettingsRepository
                .Received(1)
                .SaveSettings(
                    Arg.Is<ModuleInfo>(m => m.ModuleID == this.RequestContext.Module.ModuleID),
                    newSettings);
        }
    }
}
