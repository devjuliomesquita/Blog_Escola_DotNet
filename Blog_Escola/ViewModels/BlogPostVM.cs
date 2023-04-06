namespace Blog_Escola.ViewModels
{
    public class BlogPostVM
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? ShortDescription { get; set; }
        public string? Description { get; set; }
        public string? AuthorName { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ThumbnailUrl { get; set; }


    }
}
