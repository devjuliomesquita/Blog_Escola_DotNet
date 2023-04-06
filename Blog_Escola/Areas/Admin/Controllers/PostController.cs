using AspNetCoreHero.ToastNotification.Abstractions;
using Blog_Escola.Data;
using Blog_Escola.Models;
using Blog_Escola.Utilites;
using Blog_Escola.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

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
        private readonly UserManager<ApplicationUser> _manager;
        public PostController(
                                INotyfService notyfService,
                                ApplicationDbContext context,
                                IWebHostEnvironment environment,
                                UserManager<ApplicationUser> manager
            )
        {
            _iNotyfService = notyfService;
            _context = context;
            _environment = environment;
            _manager = manager;
        }
        [HttpGet]
        public async Task<IActionResult> Index(int? page)
        {
            //Inicializar uma lista de Posts
            var listOfPosts = new List<Post>();
            //Pegar o Id do usuário
            var loggedInUser = await _manager.Users.FirstOrDefaultAsync(l => l.UserName == User.Identity!.Name);
            var loggedInUserRole = await _manager.GetRolesAsync( loggedInUser! );
            //=> Testes
            if (loggedInUserRole[0] == WebSiteRoles.WebSiteAdmin)
            {
                listOfPosts = await _context.Posts!.Include(l => l.ApplicationUser).ToListAsync();
            }
            else
            {
                listOfPosts = await _context.Posts! .Include(l => l.ApplicationUser)
                                                    .Where(l => l.ApplicationUser.Id == loggedInUser!.Id)
                                                    .ToListAsync();
            }

            //Mapeando
            var listOfPostVM = listOfPosts.Select(l => new PostVM() 
            { 
                Id = l.Id,
                Title = l.Title,
                ShortDescription = l.ShortDescription,
                CreatedAt = l.CreatedAt,
                Thumbnail = l.ThumbnailUrl,
                AuthorName = l.ApplicationUser.FirstName + " " + l.ApplicationUser.LastName
            }).ToList();

            int pageSize = 5;
            int pageNumber = (page ?? 1);

            return View(await listOfPostVM.OrderByDescending(x => x.CreatedAt).ToPagedListAsync(pageNumber, pageSize));

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
            //Confere se o Model está válido para a requisição
            if (!ModelState.IsValid) { return View(createPostVM); }
            //Pegar o Id do usuário
            var loggedInUser = await _manager.Users.FirstOrDefaultAsync(l => l.UserName == User.Identity!.Name);
            //Criar o mapeamento do POST
            var post = new Post();
            post.Title = createPostVM.Title;
            post.ShortDescription = createPostVM.ShortDescription;
            post.Description = createPostVM.Description;
            post.ApplicationUserId = loggedInUser!.Id;
            //==>Testes
            if (post.Title != null)
            {
                string slug = createPostVM.Title!.Trim();
                slug = slug.Replace(" ", "-");
                post.Slug = slug + Guid.NewGuid();
            }
            if (createPostVM.Thumbnail!= null)
            {
                post.ThumbnailUrl = UploadImage(createPostVM.Thumbnail);
            }
            //Criar na Base
            await _context.Posts!.AddAsync(post);
            await _context.SaveChangesAsync();
            //Notificações
            _iNotyfService.Success("Post criado com sucesso.");

            return RedirectToAction("Index", "Post", new {area="Admin"});
        }
        //Método Delete
        //[HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            //Encontrar o post
            var post = await _context.Posts!.FirstOrDefaultAsync(p => p.Id == id);
            //Pegar o Id do usuário
            var loggedInUser = await _manager.Users.FirstOrDefaultAsync(l => l.UserName == User.Identity!.Name);
            var loggedInUserRole = await _manager.GetRolesAsync(loggedInUser!);
            //Testes
            if (loggedInUserRole[0] == WebSiteRoles.WebSiteAdmin || loggedInUser.Id == post.ApplicationUserId)
            {
                _context.Posts!.Remove(post!);
                await _context.SaveChangesAsync();
                _iNotyfService.Warning("Post Apagado.");
                return RedirectToAction("Index", "Post", new { area = "Admin" });
            }
            return View();
        }
        //Editar um post
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            //Encontrar o post
            var post = await _context.Posts!.FirstOrDefaultAsync(p => p.Id==id);
            //Teste se existe
            if(post == null)
            {
                _iNotyfService.Warning("Post não encontrado");
                return View();
            }
            //Pegar o Id do usuário e o tipo de usuário
            var loggedInUser = await _manager.Users.FirstOrDefaultAsync(l => l.UserName == User.Identity!.Name);
            var loggedInUserRole = await _manager.GetRolesAsync(loggedInUser!);
            if (loggedInUserRole[0] != WebSiteRoles.WebSiteAdmin && loggedInUser.Id != post.ApplicationUserId )
            {
                _iNotyfService.Information("Sem altorização.");
                return RedirectToAction("Index");
            }
            //Sem erros - mapear o objeto
            var edit = new CreatePostVM()
            {
                Id = post.Id,
                Title = post.Title,
                ShortDescription = post.ShortDescription,
                Description = post.Description,
                ThumbnailUrl = post.ThumbnailUrl,
            };
            return View(edit);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(CreatePostVM createPostVM)
        {
            //Vrificar se o formulário é válido
            if (!ModelState.IsValid) { return View(createPostVM); }
            //Encontrar o post
            var post = await _context.Posts!.FirstOrDefaultAsync(p => p.Id == createPostVM.Id);
            //Teste se existe
            if (post == null)
            {
                _iNotyfService.Warning("Post não encontrado");
                return View();
            }
            post.Title = createPostVM.Title;
            post.ShortDescription = createPostVM.ShortDescription;
            post.Description = createPostVM.Description;
            //post.ThumbnailUrl = createPostVM.ThumbnailUrl;
            if (createPostVM.Thumbnail != null)
            {
                post.ThumbnailUrl = UploadImage(createPostVM.Thumbnail);
            }
            //Carregamento no bando de dados do Entity
            await _context.SaveChangesAsync();
            _iNotyfService.Success("Post Atualizado.");
            return RedirectToAction("Index", "Post", new {area="Admin"});
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
