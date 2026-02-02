// MIT License
// Copyright DNN Community

namespace DNN.Modules.UserVoice.Data.Repositories
{
    using DNN.Modules.UserVoice.Data.Entities;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides data-access methods for ideas.
    /// </summary>
    public interface IIdeaRepository : IRepository<Idea>
    {
        /// <summary>
        /// Checks if a title is unique for the module.
        /// </summary>
        /// <param name="title">The title to check.</param>
        /// <param name="moduleId">The module identifier.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>True if the title is unique for the module, false otherwise.</returns>
        Task<bool> IsTitleUniqueAsync(string title, int moduleId, CancellationToken token);
    }
}
