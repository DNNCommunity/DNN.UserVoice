// MIT License
// Copyright DNN Community

using DNN.Modules.UserVoice.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace DNN.Modules.UserVoice.Services.Items
{
    /// <summary>
    /// Represents the basic information about an item.
    /// </summary>
    public class ItemViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemViewModel"/> class.
        /// </summary>
        public ItemViewModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemViewModel"/> class.
        /// </summary>
        /// <param name="item">An <see cref="Item"/> entity.</param>
        public ItemViewModel(Item item)
        {
            this.Id = item.Id;
            this.Name = item.Name;
            this.Description = item.Description;
        }

        /// <summary>
        /// Gets or sets the id of the item.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the item.
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the item description.
        /// </summary>
        public string Description { get; set; }
    }
}
