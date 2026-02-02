// MIT License
// Copyright DNN Community

namespace DNN.Modules.UserVoice.Services.Ideas.Validators
{
    using DNN.Modules.UserVoice.Adapters;
    using DNN.Modules.UserVoice.Common.Utilities;
    using DNN.Modules.UserVoice.Data.Entities;
    using DNN.Modules.UserVoice.Data.Repositories;
    using DNN.Modules.UserVoice.Services.Ideas.DTOs;
    using DNN.Modules.UserVoice.Services.Localization;
    using FluentValidation;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides validation logic for SaveIdeaDtoWithContext instances when saving asset data.
    /// </summary>
    internal class SaveIdeaDtoValidator : AbstractValidator<SaveIdeaDtoWithContext>
    {
        private readonly LocalizationViewModel localization;
        private readonly IIdeaRepository ideaRepository;
        private readonly IUserControllerAdapter userController;

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveIdeaDtoValidator"/> class.
        /// </summary>
        /// <param name="localizationService">Provides localization service.</param>
        /// <param name="ideaRepository">Provides data-access to ideas..</param>
        /// <param name="userController">Provides services related to users.</param>
        public SaveIdeaDtoValidator(
            ILocalizationService localizationService,
            IIdeaRepository ideaRepository,
            IUserControllerAdapter userController)
        {
            this.localization = localizationService.ViewModel;
            this.ideaRepository = ideaRepository;
            this.userController = userController;

            this.RuleFor(x => x.ActingUserId)
                .GreaterThan(0)
                    .WithMessage(this.localization.ModelValidation.UserRequired);
            this.RuleFor(x => x.ModuleId)
                .GreaterThan(0)
                    .WithMessage(this.localization.ModelValidation.ModuleRequired);
            this.RuleFor(x => x.Dto)
                .NotNull()
                .DependentRules(() =>
                {
                    var titleMaxLength = MaxLength.Of<Idea>(x => x.Title);
                    this.RuleFor(x => x.Dto.Title)
                        .NotEmpty()
                            .WithMessage(this.localization.ModelValidation.TitleRequired)
                        .MaximumLength(titleMaxLength)
                            .WithMessage(string.Format(this.localization.ModelValidation.TitleTooLong, titleMaxLength))
                        .MustAsync(this.TitleIsUnique)
                            .WithMessage(this.localization.ModelValidation.TitleUnique);
                    this.RuleFor(x => x.Dto.Description)
                        .NotEmpty()
                            .WithMessage(this.localization.ModelValidation.DescriptionRequired)
                        .MaximumLength(MaxLength.Of<Idea>(x => x.Description))
                            .WithMessage(string.Format(this.localization.ModelValidation.DescriptionTooLong, MaxLength.Of<Idea>(x => x.Description)));
                    this.RuleFor(x => x.ActingUserId)
                        .MustAsync(this.IsAllowedToEdit);
                });
        }

        private async Task<bool> IsAllowedToEdit(SaveIdeaDtoWithContext dto, int userId, CancellationToken token)
        {
            var existingIdea = await this.ideaRepository.GetByIdAsync(dto.Dto.Id, token);
            if (existingIdea == null)
            {
                // New idea, everyone is allowed to create.
                return true;
            }

            var user = this.userController.GetUserById(dto.PortalId, userId);
            if (user.IsAdmin)
            {
                // Admins are allowed to edit any idea.
                return true;
            }

            // Only the original author is allowed to edit their idea.
            return existingIdea.CreatedByUserId == userId;
        }

        private async Task<bool> TitleIsUnique(SaveIdeaDtoWithContext dto, string title, CancellationToken token)
        {
            var isUnique = await this.ideaRepository.IsTitleUniqueAsync(title, dto.ModuleId, dto.Dto.Id, token);
            return isUnique;
        }
    }
}
