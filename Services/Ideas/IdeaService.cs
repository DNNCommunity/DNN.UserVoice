// MIT License
// Copyright DNN Community

namespace DNN.Modules.UserVoice.Services.Ideas
{
    using DNN.Modules.UserVoice.Adapters;
    using DNN.Modules.UserVoice.Common.Extensions;
    using DNN.Modules.UserVoice.Data.Repositories;
    using DNN.Modules.UserVoice.Providers;
    using DNN.Modules.UserVoice.Services.Ideas.DTOs;
    using DNN.Modules.UserVoice.Services.Ideas.Mappers;
    using DNN.Modules.UserVoice.Services.Ideas.ViewModels;
    using DotNetNuke.Abstractions.Users;
    using FluentValidation;
    using OneOf;
    using OneOf.Types;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    /// <inheritdoc cref="IIdeaService"/>
    internal class IdeaService : IIdeaService
    {
        private readonly IValidator<SaveIdeaDtoWithContext> saveIdeaDtoValidator;
        private readonly IIdeaRepository ideaRepository;
        private readonly IValidator<RequestIdeaDeletionDtoWithContext> requestIdeaDeletionDtoValidator;
        private readonly IDateTimeProvider datetimeProvider;
        private readonly IUserControllerAdapter userController;

        /// <summary>
        /// Initializes a new instance of the <see cref="IdeaService"/> class.
        /// </summary>
        /// <param name="saveIdeaDtoValidator">Validator for saving an idea.</param>
        /// <param name="ideaRepository">Provides data-access to ideas.</param>
        /// <param name="requestIdeaDeletionDtoValidator">Validator for requesting idea deletion.</param>
        /// <param name="datetimeProvider">Provider for current date and time.</param>
        /// <param name="userController">Provides services related to users.</param>
        public IdeaService(
            IValidator<SaveIdeaDtoWithContext> saveIdeaDtoValidator,
            IIdeaRepository ideaRepository,
            IValidator<RequestIdeaDeletionDtoWithContext> requestIdeaDeletionDtoValidator,
            IDateTimeProvider datetimeProvider,
            IUserControllerAdapter userController)
        {
            this.saveIdeaDtoValidator = saveIdeaDtoValidator;
            this.ideaRepository = ideaRepository;
            this.requestIdeaDeletionDtoValidator = requestIdeaDeletionDtoValidator;
            this.datetimeProvider = datetimeProvider;
            this.userController = userController;
        }

        /// <inheritdoc/>
        public async Task<IdeaDetailsViewModel> GetIdeaDetailsAsync(
            int id,
            IUserInfo actingUser,
            int portalId,
            CancellationToken token)
        {
            var idea = await this.ideaRepository.GetByIdAsync(id, token);
            var author = this.userController.GetUserById(portalId, idea.CreatedByUserId);
            return idea.ToIdeaDetailsViewModel(actingUser, author);
        }

        /// <inheritdoc/>
        public async Task<OneOf<Success, Error<IEnumerable<string>>>> SaveIdeaAsync(SaveIdeaDtoWithContext dtoWithContext, CancellationToken token)
        {
            var validationResult = await this.saveIdeaDtoValidator.ValidateAsync(dtoWithContext, token);
            if (!validationResult.IsValid)
            {
                return new Error<IEnumerable<string>>(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            var existingIdea = await this.ideaRepository.GetByIdAsync(dtoWithContext.Dto.Id, token);
            if (existingIdea is null)
            {
                var idea = dtoWithContext.ToNewIdea();

                await this.ideaRepository.CreateAsync(idea, dtoWithContext.ActingUserId, token);
                return default(Success);
            }

            existingIdea.UpdateIdeaFromDto(dtoWithContext);
            await this.ideaRepository.UpdateAsync(existingIdea, dtoWithContext.ActingUserId, token);

            return default(Success);
        }

        /// <inheritdoc/>
        public async Task<PagedList<IdeaViewModel>> SearchIdeasAsync(SearchIdeasDtoWithContext dto, CancellationToken token)
        {
            var query = this.ideaRepository
                .Get()
                .Where(i =>
                    i.ModuleId == dto.ModuleId &&
                    i.DeletedOn == null);
            if (dto.Dto.OnlyMyIdeas)
            {
                query = query
                    .Where(i => i.CreatedByUserId == dto.ActingUserId);
            }

            var words = (dto.Dto.Query ?? string.Empty)
                .Split([' '], StringSplitOptions.RemoveEmptyEntries)
                .Select(w => w.Trim())
                .Where(w => w.Length >= 3) // Drop very short words
                .Distinct()
                .Take(8) // Limit number of words to prevent query bloat
                .ToArray();

            if (words.Length == 0)
            {
                var paged = await query
                    .OrderByDescending(i => i.CreatedAt)
                    .ToPagedListAsync(dto.Dto.Page, dto.Dto.PageSize, token);
                return new PagedList<IdeaViewModel>(
                    paged.Items.Select(i => i.ToIdeaViewModel()),
                    paged.Page,
                    paged.PageSize,
                    paged.ResultCount,
                    paged.PageCount);
            }

            const int TitleWeight = 10;
            const int DescriptionWeight = 1;

            var scored = query
                .Select(i => new
                {
                    Idea = i,
                    Score = words.Sum(w =>
                        (i.Title.ToUpper().Contains(w.ToUpper()) ? TitleWeight : 0) +
                        (i.Description.ToUpper().Contains(w.ToUpper()) ? DescriptionWeight : 0)),
                })
                .Where(x => x.Score > 0)
                .OrderByDescending(x => x.Score)
                .ThenByDescending(x => x.Idea.CreatedAt);

            var pagedScored = await scored
                .ToPagedListAsync(dto.Dto.Page, dto.Dto.PageSize, token);

            return new PagedList<IdeaViewModel>(
                pagedScored.Items.Select(x => x.Idea.ToIdeaViewModel()),
                pagedScored.Page,
                pagedScored.PageSize,
                pagedScored.ResultCount,
                pagedScored.PageCount);
        }

        /// <inheritdoc/>
        public async Task<OneOf<Success, Error<IEnumerable<string>>>> SoftDeleteIdea(RequestIdeaDeletionDtoWithContext dto, CancellationToken token)
        {
            var validationResult = await this.requestIdeaDeletionDtoValidator.ValidateAsync(dto, token);

            if (!validationResult.IsValid)
            {
                return new Error<IEnumerable<string>>(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            var existing = await this.ideaRepository.GetByIdAsync(dto.Dto.Id, token);
            if (existing == null)
            {
                return default(Success);
            }

            existing.DeletedOn = this.datetimeProvider.GetUtcNow();
            await this.ideaRepository.UpdateAsync(existing, dto.ActingUserId, token);

            return default(Success);
        }
    }
}
