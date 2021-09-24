using System;
using System.ComponentModel.DataAnnotations;

namespace ComputationalLinguistics.Models
{
    public class CreateWordModel
    {
        [Required(ErrorMessage="Слово обязательно для заполнения.")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Должны быть только латинские буквы.")]
        public string Content { get; set; }
    }
}