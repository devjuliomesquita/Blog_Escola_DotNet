using System.ComponentModel.DataAnnotations;

namespace Blog_Escola.ViewModels
{
    public class PageVM
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Um Título é necessário.")]
        public string? Title { get; set; }
        public string? ShortDescription { get; set; }
        public string? Description { get; set; }
        public string? ThumbnailUrl { get; set; }
        public IFormFile? Thumbnail { get; set; }
    }
}
