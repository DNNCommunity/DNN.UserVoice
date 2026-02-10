// MIT License
// Copyright DNN Community

namespace DNN.Modules.UserVoice.Data.Entities
{
    using DNN.Modules.UserVoice.Common;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Represents an idea.
    /// </summary>
    [Table(Globals.ModulePrefix + "Ideas")]
    public class Idea : BaseEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Idea"/> class.
        /// </summary>
        public Idea()
        {
            this.UserVotes = new HashSet<UserVote>();
        }

        /// <summary>
        /// Gets or sets the unique identifier for the module this idea belongs to.
        /// </summary>
        [Required]
        [Index]
        public int ModuleId { get; set; }

        /// <summary>
        /// Gets or sets the title of the idea.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [StringLength(200)]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the description of the idea.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [StringLength(2000)]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the entity was soft-deleted.
        /// </summary>
        public DateTime? DeletedOn { get; set; }

        /// <summary>
        /// Gets or sets the collection of votes associated with the idea.
        /// </summary>
        public virtual ICollection<UserVote> UserVotes { get; set; }
    }
}
