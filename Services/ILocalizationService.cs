// MIT License
// Copyright DNN Community
using DNN.Modules.DnnUserVoice.ViewModels;

namespace DNN.Modules.DnnUserVoice.Services
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