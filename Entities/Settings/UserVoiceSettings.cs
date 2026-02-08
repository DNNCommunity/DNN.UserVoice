// MIT License
// Copyright DNN Community

namespace DNN.Modules.UserVoice.Entities.Settings
{
    using DotNetNuke.Entities.Modules.Settings;

    /// <summary>
    /// Represents the UserVoice module settings.
    /// </summary>
    public class UserVoiceSettings
    {
        /// <summary>
        /// Gets or sets the default number of votes allocated to each user.
        /// </summary>
        [ModuleSetting]
        public int BaseVotesPerUser { get; set; } = 10;
    }
}
