// MIT License
// Copyright DNN Community

namespace DNN.Modules.UserVoice.Services.Ideas
{
    using DNN.Modules.UserVoice.Data.Repositories;
    using DNN.Modules.UserVoice.Services.Ideas.DTOs;
    using DotNetNuke.Abstractions.Users;
    using OneOf;
    using OneOf.Types;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides services related to ideas.
    /// </summary>
    public interface IIdeaService
    {
        /// <summary>
        /// Asynchronously retrieves detailed information for the specified idea.
        /// </summary>
        /// <param name="id">The unique identifier of the idea to retrieve details for.</param>
        /// <param name="actingUser">The user requesting the idea details.</param>
        /// <param name="token">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see
        /// cref="IdeaDetailsViewModel"/> with the details of the specified idea.</returns>
        Task<IdeaDetailsViewModel> GetIdeaDetailsAsync(int id, IUserInfo actingUser, CancellationToken token);

        /// <summary>
        /// Saves (creates or updates) an idea asynchronously.
        /// </summary>
        /// <param name="dtoWithContext">An object containing the idea details and contextual information required for saving the idea. Cannot be
        /// null.</param>
        /// <param name="token">A cancellation token that can be used to cancel the save operation.</param>
        /// <returns>A task that represents the asynchronous operation. The result is a union type containing either a Success
        /// value if the idea was saved successfully, or an Error with a list of validation or processing error messages
        /// if the save failed.</returns>
        Task<OneOf<Success, Error<IEnumerable<string>>>> SaveIdeaAsync(SaveIdeaDtoWithContext dtoWithContext, CancellationToken token);

        /// <summary>
        /// Asynchronously searches for ideas that match the specified criteria and returns a paged list of results.
        /// </summary>
        /// <param name="dto">An object containing the search criteria and context information used to filter and sort the ideas. Cannot
        /// be null.</param>
        /// <param name="token">A cancellation token that can be used to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a paged list of idea view models
        /// that match the search criteria. The list may be empty if no ideas are found.</returns>
        Task<PagedList<IdeaViewModel>> SearchIdeasAsync(SearchIdeasDtoWithContext dto, CancellationToken token);

        /// <summary>
        /// Marks an idea as deleted (soft delete) asynchronously.
        /// </summary>
        /// <param name="dto">An object containing the details of the idea to be deleted along with contextual information. Cannot be null.</param>
        /// <param name="token">A cancellation token that can be used to cancel the delete operation.</param>
        /// <returns>A task that represents the asynchronous operation. The result is a union type containing either a Success or a list of errors.</returns>
        Task<OneOf<Success, Error<IEnumerable<string>>>> SoftDeleteIdea(RequestIdeaDeletionDtoWithContext dto, CancellationToken token);
    }
}
