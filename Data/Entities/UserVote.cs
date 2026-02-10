// MIT License
// Copyright DNN Community

namespace DNN.Modules.UserVoice.Data.Entities
{
    using DNN.Modules.UserVoice.Common;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Represents a single vote from a user on an idea.
    /// </summary>
    [Table(Globals.ModulePrefix + "UserVotes")]
    public class UserVote : BaseEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier for the user.
        /// </summary>
        [Index]
        [Index("IX_UserVote_Idea_User", 2, IsUnique = true)]
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the associated idea identifier.
        /// </summary>
        [Index("IX_UserVote_Idea_User", 1, IsUnique = true)]
        [ForeignKey(nameof(Idea))]
        public int IdeaId { get; set; }

        /// <summary>
        /// Gets or sets the associated idea entity.
        /// </summary>
        [Required]
        public virtual Idea Idea { get; set; }
    }
}
