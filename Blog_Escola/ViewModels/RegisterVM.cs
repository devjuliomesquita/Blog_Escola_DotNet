
using System.ComponentModel.DataAnnotations;

namespace Blog_Escola.ViewModels
{
    public class RegisterVM
    {
        [Required(ErrorMessage = "O Nome é Obrigatório.")]
        public string? FirstName { get; set; }
        [Required(ErrorMessage = "O Sobrenome é Obrigatório.")]
        public string? LastName { get; set; }
        [Required(ErrorMessage = "O E-mail é Obrigatório.")]
        [EmailAddress]
        public string? Email { get; set; }
        [Required(ErrorMessage = "O Nome de Usuário é Obrigatório.")]
        public string? UserName { get; set; }
        [Required(ErrorMessage = "O Senha é Obrigatório.")]
        public string? Password { get; set; }
        public bool IsAdmin { get; set; }

    }
}
