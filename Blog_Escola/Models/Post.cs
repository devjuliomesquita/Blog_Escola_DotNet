namespace Blog_Escola.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? ShortDescription { get; set; }
        //Relacionamentos
        public string? ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string? Description { get; set; }
        public string? Slug { get; set; }
    }
}
