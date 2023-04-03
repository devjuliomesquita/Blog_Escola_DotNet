using Blog_Escola.Data;
using Blog_Escola.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog_Escola.Controllers
{
    public class PageController : Controller
    {
        //Feito a instância do banco de dados
        private readonly ApplicationDbContext _context;
        public PageController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> About()
        {
            //Encontrar a página correta
            var page = await _context.Pages!.FirstOrDefaultAsync(p => p.Slug == "about");
            //Mapear o objeto
            var viewModel = new PageVM()
            {
                Title = page!.Title,
                ShortDescription = page.ShortDescription,
                Description = page.Description,
                ThumbnailUrl = page.ThumbnailUrl
            };
            return View(viewModel);
        }
        public async Task<IActionResult> Contact()
        {
            //Encontrar a página correta
            var page = await _context.Pages!.FirstOrDefaultAsync(p => p.Slug == "contact");
            //Mapear o objeto
            var viewModel = new PageVM()
            {
                Title = page!.Title,
                ShortDescription = page.ShortDescription,
                Description = page.Description,
                ThumbnailUrl = page.ThumbnailUrl
            };
            return View(viewModel);
        }
        public async Task<IActionResult> Privacy()
        {
            //Encontrar a página correta
            var page = await _context.Pages!.FirstOrDefaultAsync(p => p.Slug == "privacy");
            //Mapear o objeto
            var viewModel = new PageVM()
            {
                Title = page!.Title,
                ShortDescription = page.ShortDescription,
                Description = page.Description,
                ThumbnailUrl = page.ThumbnailUrl
            };
            return View(viewModel);
        }
        public IActionResult Teste()
        {
            return View();
        }
    }
}
