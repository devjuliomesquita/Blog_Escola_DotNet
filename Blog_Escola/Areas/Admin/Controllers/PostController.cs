using AspNetCoreHero.ToastNotification.Abstractions;
using Blog_Escola.Data;
using Blog_Escola.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog_Escola.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class PostController : Controller
    {
        //Inicializado o construtor
        private readonly INotyfService _iNotyfService;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        public PostController(
                                INotyfService notyfService,
                                ApplicationDbContext context,
                                IWebHostEnvironment environment
            )
        {
            _iNotyfService = notyfService;
            _context = context;
            _environment = environment;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreatePostVM());
        }
        //Fazendo o Upload do Post
        [HttpPost]
        public async Task<IActionResult> Create(CreatePostVM createPostVM)
        {
            if (!ModelState.IsValid) { return View(createPostVM); }

            return View(createPostVM);
        }
            //Upload da foto
        private string UploadImage(IFormFile formFile)
        {
            string uniqueFileName = "";
            var folderPath = Path.Combine(_environment.WebRootPath, "Thumbnails");
            uniqueFileName = new Guid().ToString() + "_" + formFile.FileName;
            var filePath = Path.Combine(folderPath, uniqueFileName);
            using (FileStream fileStream = System.IO.File.Create(filePath))
            {
                formFile.CopyTo(fileStream);
            }
            return uniqueFileName;
        }
    }
}
