// MIT License
// Copyright DNN Community

namespace DNN.Modules.UserVoice.Data.Entities
{
    using DNN.Modules.UserVoice.Common;
    using DotNetNuke.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Represents an idea.
    /// </summary>
    [TableName(Globals.ModulePrefix + "Ideas")]
    public class Idea : BaseEntity
    {
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
    }
}
