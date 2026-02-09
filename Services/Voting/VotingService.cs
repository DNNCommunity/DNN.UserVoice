// MIT License
// Copyright DNN Community

namespace DNN.Modules.UserVoice.Services.Voting
{
    using DNN.Modules.UserVoice.Data.Repositories;
    using DNN.Modules.UserVoice.Entities.Settings;
    using DNN.Modules.UserVoice.Services.Voting.DTOs;
    using DNN.Modules.UserVoice.Services.Voting.ViewModels;
    using FluentValidation;
    using OneOf;
    using OneOf.Types;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    /// <inheritdoc cref="IVotingService"/>
    internal class VotingService : IVotingService
    {
        private readonly IValidator<VoteDtoWithContext> upvoteDtoValidator;
        private readonly IUserVoteRepository userVoteRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="VotingService"/> class.
        /// </summary>
        /// <param name="upvoteDtoValidator">Validates <see cref="VoteDtoWithContext"/>.</param>
        /// <param name="userVoteRepository">Provides access to user votes in the data store.</param>
        public VotingService(
            IValidator<VoteDtoWithContext> upvoteDtoValidator,
            IUserVoteRepository userVoteRepository)
        {
            this.upvoteDtoValidator = upvoteDtoValidator;
            this.userVoteRepository = userVoteRepository;
        }

        /// <inheritdoc/>
        public async Task<VoteStatsViewModel> GetVoteStatsAsync(int moduleId, int userId, UserVoiceSettings settings, CancellationToken token)
        {
            var currentVotes = await this.userVoteRepository
                .CountVotesForUserAsync(moduleId, userId, token);
            var maxVotes = settings.BaseVotesPerUser;

            return new VoteStatsViewModel
            {
                CurrentVotes = currentVotes,
                MaxVotes = maxVotes,
            };
        }

        /// <inheritdoc/>
        public async Task RemoveVoteAsync(VoteDtoWithContext dto, CancellationToken token)
        {
            await this.userVoteRepository.RemoveVoteAsync(dto.ActingUserId, dto.Dto.IdeaId, token);
        }

        /// <inheritdoc/>
        public async Task<OneOf<Success, Error<IEnumerable<string>>>> UpvoteAsync(VoteDtoWithContext dto, CancellationToken token)
        {
            var validationResult = await this.upvoteDtoValidator.ValidateAsync(dto, token);

            if (!validationResult.IsValid)
            {
                return new Error<IEnumerable<string>>(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            await this.userVoteRepository.AddVoteAsync(dto.ActingUserId, dto.Dto.IdeaId, token);

            return default(Success);
        }
    }
}
