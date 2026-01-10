// MIT License
// Copyright DNN Community

namespace DNN.Modules.UserVoice.Services.Localization
{
    /// <summary>
    /// Provides strongly typed localization services for this module.
    /// </summary>
    public interface ILocalizationService
    {
        /// <summary>
        /// Gets viewmodel that strongly types all resource keys.
        /// </summary>
        LocalizationViewModel ViewModel { get; }
    }
}