// MIT License
// Copyright DNN Community

namespace DNN.Modules.UserVoice.Entities.Settings
{
    using DotNetNuke.Entities.Modules.Settings;

    /// <summary>
    /// Provides settings management for the UserVoice module, allowing for retrieval and storage of module-specific settings.
    /// </summary>
    public interface IUserVoiceSettingsRepository : ISettingsRepository<UserVoiceSettings>
    {
    }
}
