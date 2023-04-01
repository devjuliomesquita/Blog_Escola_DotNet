using Blog_Escola.Models;
using System.ComponentModel.DataAnnotations;

namespace Blog_Escola.ViewModels
{
    public class CreatePostVM
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "O Título é Obrigatório.")]
        public string? Title { get; set; }
        [Required(ErrorMessage = "Uma Pequena Descrição é Obrigatória.")]
        public string? ShortDescription { get; set; }
        public string? ApplicationUserId { get; set; }
        [Required(ErrorMessage = "A descrição (TEXTO) é Obrigatória.")]
        public string? Description { get; set; }
        public string? ThumbnailUrl { get; set; }
        public IFormFile? Thumbnail { get; set; }
    }
}
