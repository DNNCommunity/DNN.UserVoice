// MIT License
// Copyright DNN Community

namespace DNN.Modules.UserVoice.Services.Ideas.Mappers
{
    using DNN.Modules.UserVoice.Data.Entities;
    using DNN.Modules.UserVoice.Services.Ideas.DTOs;
    using DNN.Modules.UserVoice.Services.Ideas.ViewModels;
    using DotNetNuke.Abstractions.Users;
    using Humanizer;
    using System.Linq;

    /// <summary>
    /// Provides static methods for mapping and transforming idea-related data.
    /// </summary>
    public static class IdeaMapper
    {
        /// <summary>
        /// Creates a new Idea instance using the data from the specified SaveIdeaDto and the provided module
        /// identifier.
        /// </summary>
        /// <param name="dto">The data transfer object containing the idea's information.</param>
        /// <returns>A new Idea object populated with values from the DTO.</returns>
        public static Idea ToNewIdea(this SaveIdeaDtoWithContext dto)
        {
            return new Idea
            {
                ModuleId = dto.ModuleId,
                Title = dto.Dto.Title.Trim(),
                Description = dto.Dto.Description.Trim(),
            };
        }

        /// <summary>
        /// Updates an idea with new information.
        /// </summary>
        /// <param name="idea">The idea to update.</param>
        /// <param name="dto">The data to update.</param>
        public static void UpdateIdeaFromDto(this Idea idea, SaveIdeaDtoWithContext dto)
        {
            idea.Title = dto.Dto.Title.Trim();
            idea.Description = dto.Dto.Description.Trim();
        }

        /// <summary>
        /// Creates a new IdeaViewModel instance that represents the specified Idea.
        /// </summary>
        /// <param name="idea">The Idea object to convert to an IdeaViewModel. Cannot be null.</param>
        /// <param name="actingUserId">The ID of the user for whom the view model is being created.</param>
        /// <returns>An IdeaViewModel containing the Id and Title from the specified Idea.</returns>
        public static IdeaViewModel ToIdeaViewModel(this Idea idea, int actingUserId)
        {
            return new IdeaViewModel
            {
                Id = idea.Id,
                Title = idea.Title,
                Description = idea.Description,
                Votes = idea.UserVotes.Count,
                IsVotedByUser = idea.UserVotes.Any(v => v.UserId == actingUserId),
            };
        }

        /// <summary>
        /// Creates a new instance of <see cref="IdeaDetailsViewModel"/> populated with details from the specified <see
        /// cref="Idea"/>.
        /// </summary>
        /// <param name="idea">The <see cref="Idea"/> object containing the data to be mapped. Cannot be null.</param>
        /// <param name="actingUser">Information about the user performing the action.</param>
        /// <param name="author">Information about the author of the idea.</param>
        /// <returns>An <see cref="IdeaDetailsViewModel"/> initialized with provided <see cref="Idea"/>
        /// <paramref name="idea"/>.</returns>
        public static IdeaDetailsViewModel ToIdeaDetailsViewModel(this Idea idea, IUserInfo actingUser, IUserInfo author)
        {
            return new IdeaDetailsViewModel
            {
                Id = idea.Id,
                Title = idea.Title,
                Description = idea.Description,
                CanEdit = actingUser is null ? false : (actingUser.IsAdmin || actingUser.UserID == idea.CreatedByUserId),
                CreatedByUserDisplayName = author?.DisplayName,
                CreatedAt = idea.CreatedAt,
                CreatedSince = idea.CreatedAt.Humanize(),
                Votes = idea.UserVotes.Count,
            };
        }
    }
}
