using AspNetCoreHero.ToastNotification.Abstractions;
using Blog_Escola.Data;
using Blog_Escola.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog_Escola.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class PageController : Controller
    {
        //Inicializando o construtor
        private readonly INotyfService _iNotyfService;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        public PageController(
            INotyfService notyfService,
            ApplicationDbContext context,
            IWebHostEnvironment environment)
        {
            _iNotyfService = notyfService;
            _context = context;
            _environment = environment;
        }

        [HttpGet]
        public async Task<IActionResult> About()
        {
            //Encontrar a página
            var page = await _context.Pages!.FirstOrDefaultAsync(p => p.Slug == "about");
            //Teste
            if(page == null) 
            {
                _iNotyfService.Warning("Página não encontrada.");
                return View(); 
            }
            //Mapear o objeto
            var viewModel = new PageVM()
            {
                Id = page.Id,
                Title = page.Title,
                ShortDescription = page.ShortDescription,
                Description = page.Description,
                ThumbnailUrl = page.ThumbnailUrl
            };
            return View(viewModel);   
        }
        [HttpPost]
        public async Task<IActionResult> About(PageVM pageVM)
        {
            //Validar o formulário
            if (!ModelState.IsValid) { return View(pageVM); }
            //Encontrar a página
            var page = await _context.Pages!.FirstOrDefaultAsync(p => p.Slug == "about");
            //Teste
            if (page == null)
            {
                _iNotyfService.Warning("Página não encontrada.");
                return View();
            }
            //Mapeando o objeto para realizar o Upload
            page.Title = pageVM.Title;
            page.ShortDescription = pageVM.ShortDescription;
            page.Description = pageVM.Description;
            if (pageVM.Thumbnail != null)
            {
                page.ThumbnailUrl = UploadImage(pageVM.Thumbnail);
            }
            //Entity
            await _context.SaveChangesAsync();
            _iNotyfService.Success("Página Atualizada Com Sucesso.");
            return RedirectToAction("Index", "User", new {area = "Admin"});
        }

        [HttpGet]
        public async Task<IActionResult> Contact()
        {
            //Encontrar a página
            var page = await _context.Pages!.FirstOrDefaultAsync(p => p.Slug == "contact");
            //Teste
            if (page == null)
            {
                _iNotyfService.Warning("Página não encontrada.");
                return View();
            }
            //Mapear o objeto
            var viewModel = new PageVM()
            {
                Id = page.Id,
                Title = page.Title,
                ShortDescription = page.ShortDescription,
                Description = page.Description,
                ThumbnailUrl = page.ThumbnailUrl
            };
            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> Contact(PageVM pageVM)
        {
            //Validar o formulário
            if (!ModelState.IsValid) { return View(pageVM); }
            //Encontrar a página
            var page = await _context.Pages!.FirstOrDefaultAsync(p => p.Slug == "contact");
            //Teste
            if (page == null)
            {
                _iNotyfService.Warning("Página não encontrada.");
                return View();
            }
            //Mapeando o objeto para realizar o Upload
            page.Title = pageVM.Title;
            page.ShortDescription = pageVM.ShortDescription;
            page.Description = pageVM.Description;
            if (pageVM.Thumbnail != null)
            {
                page.ThumbnailUrl = UploadImage(pageVM.Thumbnail);
            }
            //Entity
            await _context.SaveChangesAsync();
            _iNotyfService.Success("Página Atualizada Com Sucesso.");
            return RedirectToAction("Index", "User", new { area = "Admin" });
        }

        [HttpGet]
        public async Task<IActionResult> Privacy()
        {
            //Encontrar a página
            var page = await _context.Pages!.FirstOrDefaultAsync(p => p.Slug == "privacy");
            //Teste
            if (page == null)
            {
                _iNotyfService.Warning("Página não encontrada.");
                return View();
            }
            //Mapear o objeto
            var viewModel = new PageVM()
            {
                Id = page.Id,
                Title = page.Title,
                ShortDescription = page.ShortDescription,
                Description = page.Description,
                ThumbnailUrl = page.ThumbnailUrl
            };
            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> Privacy(PageVM pageVM)
        {
            //Validar o formulário
            if (!ModelState.IsValid) { return View(pageVM); }
            //Encontrar a página
            var page = await _context.Pages!.FirstOrDefaultAsync(p => p.Slug == "privacy");
            //Teste
            if (page == null)
            {
                _iNotyfService.Warning("Página não encontrada.");
                return View();
            }
            //Mapeando o objeto para realizar o Upload
            page.Title = pageVM.Title;
            page.ShortDescription = pageVM.ShortDescription;
            page.Description = pageVM.Description;
            if (pageVM.Thumbnail != null)
            {
                page.ThumbnailUrl = UploadImage(pageVM.Thumbnail);
            }
            //Entity
            await _context.SaveChangesAsync();
            _iNotyfService.Success("Página Atualizada Com Sucesso.");
            return RedirectToAction("Index", "User", new { area = "Admin" });
        }

        private string UploadImage(IFormFile file)
        {
            string uniqueFileName = "";
            var folderPath = Path.Combine(_environment.WebRootPath, "thumbnails");
            uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            var filePath = Path.Combine(folderPath, uniqueFileName);
            using (FileStream fileStream = System.IO.File.Create(filePath))
            {
                file.CopyTo(fileStream);
            }
            return uniqueFileName;
        }
    }
}
