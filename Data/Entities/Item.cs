// MIT License
// Copyright DNN Community

using DNN.Modules.DnnUserVoice.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DNN.Modules.DnnUserVoice.Data.Entities
{
    /// <summary>
    /// Represents an item entity.
    /// </summary>
    [Table(Globals.ModulePrefix + "Items")]
    public class Item : BaseEntity
    {
        /// <summary>
        /// Gets or sets the item name.
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the item description.
        /// </summary>
        [StringLength(250)]
        public string Description { get; set; }
    }
}