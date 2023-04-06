using Blog_Escola.Data;
using Blog_Escola.ViewModels;
using Blog_Escola.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace Blog_Escola.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index(int? page)
        {
            var vm = new HomeVM();
            var setting = _context.Settings!.ToList();
            vm.Title = setting[0].Title;
            vm.ShortDescription = setting[0].ShortDescription;
            vm.ThumbnailUrl = setting[0].ThumbnailUrl;
            int pageSize = 4;
            int pageNumber = (page ?? 1);
            vm.Posts = await _context.Posts!.Include(vmp => vmp.ApplicationUser )
                                            .OrderByDescending(vmp => vmp.CreatedAt)
                                            .ToPagedListAsync(pageNumber, pageSize);



            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}