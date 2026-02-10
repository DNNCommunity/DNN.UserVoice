// MIT License
// Copyright DNN Community

namespace DNN.Modules.UserVoice.Services.Ideas.Validators
{
    using DNN.Modules.UserVoice.Adapters;
    using DNN.Modules.UserVoice.Data.Entities;
    using DNN.Modules.UserVoice.Data.Repositories;
    using DNN.Modules.UserVoice.Services.Ideas.DTOs;
    using DNN.Modules.UserVoice.Services.Localization;
    using FluentValidation;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides validation logic for instances of the RequestIdeaDeletionDtoWithContext class.
    /// </summary>
    internal class RequestIdeaDeletionDtoValidator : AbstractValidator<RequestIdeaDeletionDtoWithContext>
    {
        private readonly LocalizationViewModel localization;
        private readonly IIdeaRepository ideaRepository;
        private readonly IUserControllerAdapter userController;
        private Idea idea;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestIdeaDeletionDtoValidator"/> class.
        /// </summary>
        /// <param name="localizationService">The service used to provide localized validation messages.</param>
        /// <param name="ideaRepository">The repository used to access and validate idea data.</param>
        /// <param name="userController">The adapter used to interact with user-related operations during validation.</param>
        public RequestIdeaDeletionDtoValidator(
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
            this.RuleFor(x => x.PortalId)
                .GreaterThan(-1);
            this.RuleFor(x => x.Dto)
                .NotNull()
                .DependentRules(() =>
                {
                    this.RuleFor(x => x.Dto.Id)
                        .GreaterThan(0)
                            .WithMessage(this.localization.ModelValidation.IdGreaterThanZero)
                        .DependentRules(() =>
                        {
                            this.RuleFor(x => x.Dto.Id)
                                .MustAsync(this.IdeaExists)
                                    .WithMessage(this.localization.ModelValidation.IdeaNotFound)
                                .DependentRules(() =>
                                {
                                    this.RuleFor(x => x.ActingUserId)
                                        .Must(this.ActingUserCanDeleteIdea)
                                        .WithMessage(this.localization.ModelValidation.CannotEditIdeaNotYours);
                                });
                        });
                });
        }

        private async Task<bool> IdeaExists(int ideaId, CancellationToken token)
        {
            this.idea = await this.ideaRepository.GetByIdAsync(ideaId, token);
            return this.idea != null;
        }

        private bool ActingUserCanDeleteIdea(RequestIdeaDeletionDtoWithContext dto, int actingUserId)
        {
            var actingUser = this.userController.GetUserById(dto.PortalId, actingUserId);

            if (actingUser is null)
            {
                return false;
            }

            if (actingUser.IsAdmin)
            {
                return true;
            }

            return this.idea.CreatedByUserId == actingUserId;
        }
    }
}
