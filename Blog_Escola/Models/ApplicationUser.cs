using Microsoft.AspNetCore.Identity;

namespace Blog_Escola.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        //Relacionamentos
        public List<Post>? Posts { get; set; }
    }
}
