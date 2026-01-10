// MIT License
// Copyright DNN Community

using System;

namespace DNN.Modules.UserVoice.Providers
{
    /// <summary>
    /// Provides a testable version of DateTime.
    /// </summary>
    public interface IDateTimeProvider
    {
        /// <summary>
        /// Gets the current UTC date and time.
        /// </summary>
        /// <returns>The current UTC date and time.</returns>
        DateTime GetUtcNow();
    }
}
