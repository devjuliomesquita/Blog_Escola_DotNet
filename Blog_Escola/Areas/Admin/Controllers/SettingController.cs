using AspNetCoreHero.ToastNotification.Abstractions;
using Blog_Escola.Data;
using Blog_Escola.ViewModels;
using Blog_Escola.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Dynamic;

namespace Blog_Escola.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SettingController : Controller
    {
        //Instanciado no construtor
        private readonly ApplicationDbContext _context;
        private readonly INotyfService _notyfService;
        private readonly IWebHostEnvironment _environment;
        public SettingController(
            ApplicationDbContext context,
            INotyfService notyfService,
            IWebHostEnvironment environment
            )
        {
            _context = context;
            _notyfService = notyfService;
            _environment = environment;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            //retornar uma lista de Settings
            var settings = _context.Settings!.ToList();
            if(settings.Count > 0)
            {
                //Criando o objeto
                var model = new SettingVM()
                {
                    Id = settings[0].Id,
                    SiteName = settings[0].SiteName,
                    Title = settings[0].Title,
                    ShortDescription = settings[0].ShortDescription,
                    ThumbnailUrl = settings[0].ThumbnailUrl,
                    GitHub = settings[0].GitHub,
                    LinkedIn = settings[0].LinkedIn,
                    Gmail = settings[0].Gmail,
                    Portifolio = settings[0].Portifolio,
                    WhatsApp = settings[0].WhatsApp,
                };
                return View(model);
            }
            //Alocando um nome no site
            var setting = new Setting()
            {
                SiteName = "Demo Site",
            };
            //Adicionando no contexto de dados
            await _context.Settings!.AddAsync(setting);
            await _context.SaveChangesAsync(); 

            var createSettings = _context.Settings?.ToList();
            var createVM = new SettingVM()
            {
                Id = createSettings[0].Id,
                SiteName = createSettings[0].SiteName,
                Title = createSettings[0].Title,
                ShortDescription = createSettings[0].ShortDescription,
                ThumbnailUrl = createSettings[0].ThumbnailUrl,
                GitHub = createSettings[0].GitHub,
                LinkedIn = createSettings[0].LinkedIn,
                Gmail = createSettings[0].Gmail,
                Portifolio = createSettings[0].Portifolio,
                WhatsApp = createSettings[0].WhatsApp,
            };
            return View(createVM);
        }
        [HttpPost]
        public async Task<IActionResult> Index(SettingVM settingVM)
        {
            //Veirificar se o model é válido
            if (!ModelState.IsValid) { return View(settingVM); }
            //Encontrar as configurações no contexto de dados
            var setting = await _context.Settings!.FirstOrDefaultAsync(s => s.Id == settingVM.Id);
            if(setting == null)
            {
                _notyfService.Warning("Temos algum problema no contexto de dados.");
                return View(settingVM);
            }
            //Mapeando o objeto
            setting.SiteName = settingVM.SiteName;
            setting.Title = settingVM.Title;
            setting.ShortDescription = settingVM.ShortDescription;
            setting.GitHub = settingVM.GitHub;
            setting.LinkedIn = settingVM.LinkedIn;
            setting.Gmail = settingVM.Gmail;
            setting.WhatsApp = settingVM.WhatsApp;
            setting.Portifolio = settingVM.Portifolio;
            if(settingVM.Thumbnail != null)
            {
                setting.ThumbnailUrl = UploadImage(settingVM.Thumbnail);
            }
            //Entity
            await _context.SaveChangesAsync();
            _notyfService.Success("Configurações salvas com sucesso.");
            return RedirectToAction("Index", "User", new { area = "Admin"});
        }

        //=>Upload da foto
        private string UploadImage(IFormFile formFile)
        {
            string uniqueFileName = "";
            var folderPath = Path.Combine(_environment.WebRootPath, "Thumbnail");
            uniqueFileName = Guid.NewGuid().ToString() + "_" + formFile.FileName;
            var filePath = Path.Combine(folderPath, uniqueFileName);
            using (FileStream fileStream = System.IO.File.Create(filePath))
            {
                formFile.CopyTo(fileStream);
            }
            return uniqueFileName;
        }
    }
}
