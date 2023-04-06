using AspNetCoreHero.ToastNotification.Abstractions;
using Blog_Escola.Data;
using Blog_Escola.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog_Escola.Controllers
{
    public class BlogController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly INotyfService _notyfService;
        public BlogController(
            ApplicationDbContext context,
            INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }
        [HttpGet("[controller]/{slug}")]
        public async Task<IActionResult> Post(string slug)
        {
            //Encontrar esse post
            var post = await _context.Posts!.Include(p => p.ApplicationUser).FirstOrDefaultAsync(p => p.Slug == slug);
            //Teste
            if (post == null || slug == "")
            {
                _notyfService.Error("Post não encontrado.");
                return RedirectToAction("Index", "Home", new {area = ""});
            }
            //Criar o objeto de Post
            var vm = new BlogPostVM()
            {
                Id = post.Id,
                Title = post.Title,
                ShortDescription = post.ShortDescription,
                AuthorName = post.ApplicationUser.FirstName + " " + post.ApplicationUser.LastName,
                CreatedAt = post.CreatedAt,
                ThumbnailUrl = post.ThumbnailUrl,
                Description = post.Description
            };
            return View(vm);
        }
    }
}
