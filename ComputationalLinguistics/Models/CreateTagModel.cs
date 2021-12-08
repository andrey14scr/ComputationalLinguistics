using System.ComponentModel.DataAnnotations;

namespace ComputationalLinguistics.Models
{
    public class CreateTagModel
    {
        [Required(ErrorMessage = "Это поле обязательно для заполнения.")]
        [StringLength(5)]
        public string TagName { get; set; }

        [Required(ErrorMessage = "Это поле обязательно для заполнения.")]
        public string Info { get; set; }
    }
}
