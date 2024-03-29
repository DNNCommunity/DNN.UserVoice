﻿// MIT License
// Copyright DNN Community

using System.ComponentModel.DataAnnotations;

namespace DNN.Modules.DnnUserVoice.DTO
{
    /// <summary>
    /// Data transfer object to create a new item.
    /// </summary>
    public class CreateItemDTO
    {
        /// <summary>
        /// Gets or sets the name for the item.
        /// </summary>
        [Required(ErrorMessage = "NameRequired")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the item.
        /// </summary>
        public string Description { get; set; }
    }
}
