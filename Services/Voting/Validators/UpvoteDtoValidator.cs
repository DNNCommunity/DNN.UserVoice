// MIT License
// Copyright DNN Community

namespace DNN.Modules.UserVoice.Services.Voting.Validators
{
    using DNN.Modules.UserVoice.Data.Entities;
    using DNN.Modules.UserVoice.Data.Repositories;
    using DNN.Modules.UserVoice.Services.Localization;
    using DNN.Modules.UserVoice.Services.Voting.DTOs;
    using FluentValidation;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides validation rules for instances of the <see cref="VoteDtoWithContext"/> object.
    /// </summary>
    internal class UpvoteDtoValidator : AbstractValidator<VoteDtoWithContext>
    {
        private readonly LocalizationViewModel localization;
        private readonly IIdeaRepository ideaRepository;
        private readonly IUserVoteRepository userVoteRepository;

        private Idea idea;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpvoteDtoValidator"/> class.
        /// </summary>
        /// <param name="localizationService">Provides localization services.</param>
        /// <param name="ideaRepository">Provides data-access methods for ideas.</param>
        /// <param name="userVoteRepository">Provides data-access methods for user votes.</param>
        public UpvoteDtoValidator(
            ILocalizationService localizationService,
            IIdeaRepository ideaRepository,
            IUserVoteRepository userVoteRepository)
        {
            this.localization = localizationService.ViewModel;
            this.ideaRepository = ideaRepository;
            this.userVoteRepository = userVoteRepository;

            this.RuleFor(x => x.ActingUserId)
                .GreaterThan(0);
            this.RuleFor(x => x.Dto)
                .NotNull()
                .DependentRules(() =>
                {
                    this.RuleFor(x => x.Dto.IdeaId)
                    .GreaterThan(0)
                    .WithMessage(this.localization.ModelValidation.IdeaNotFound)
                    .DependentRules(() =>
                    {
                        this.RuleFor(x => x.Dto.IdeaId)
                            .MustAsync(this.IdeaExists)
                            .WithMessage(this.localization.ModelValidation.IdeaNotFound)
                            .DependentRules(() =>
                            {
                                this.RuleFor(x => x)
                                    .MustAsync(this.NotExceedVotingLimit)
                                    .WithMessage(this.localization.ModelValidation.VotesExhausted);
                            });
                    });
                });
        }

        private async Task<bool> NotExceedVotingLimit(VoteDtoWithContext context, CancellationToken token)
        {
            var userVoteCount = await this.userVoteRepository
                .CountVotesForUserAsync(context.ModuleId, context.ActingUserId, token);
            return userVoteCount < context.Settings.BaseVotesPerUser;
        }

        private async Task<bool> IdeaExists(int ideaId, CancellationToken token)
        {
            this.idea = await this.ideaRepository.GetByIdAsync(ideaId, token);
            return this.idea != null;
        }
    }
}
