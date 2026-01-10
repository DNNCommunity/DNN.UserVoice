// MIT License
// Copyright DNN Community

using System;

namespace DNN.Modules.UserVoice.Providers
{
    /// <summary>
    /// Provides a testable version of DateTime.
    /// </summary>
    internal class DateTimeProvider : IDateTimeProvider
    {
        /// <inheritdoc/>
        public DateTime GetUtcNow() => DateTime.UtcNow;
    }
}
