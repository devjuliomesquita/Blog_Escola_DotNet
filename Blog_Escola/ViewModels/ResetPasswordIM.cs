using System.ComponentModel.DataAnnotations;

namespace Blog_Escola.ViewModels
{
    public class ResetPasswordIM
    {
        public string? Id { get; set; }
        public string? UserName { get; set; }
        [Required(ErrorMessage = "O Preenchimento Obrigatório.")]
        public string? NewPassword { get; set; }
        [Required(ErrorMessage = "O Preenchimento Obrigatório.")]
        [Compare(nameof(NewPassword))]
        public string? ConfirmPassword { get; set; }

    }
}
