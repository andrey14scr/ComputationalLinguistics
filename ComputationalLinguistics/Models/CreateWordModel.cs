using System;
using System.ComponentModel.DataAnnotations;

namespace ComputationalLinguistics.Models
{
    public class CreateWordModel
    {
        [Required]
        public string Content { get; set; }
    }
}