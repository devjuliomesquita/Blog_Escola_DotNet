using System.ComponentModel.DataAnnotations;


namespace Blog_Escola.ViewModels
{
    public class LoginVM
    {
        [Required(ErrorMessage ="O Nome do Usuário é Obrigatório.")]
        public string? Username { get; set; }
        [Required(ErrorMessage = "Sua senha é obrigatória.")]
        public string? Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
