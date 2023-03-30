using Microsoft.AspNetCore.Mvc;

namespace Blog_Escola.Controllers
{
    public class PageController : Controller
    {
        public IActionResult About()
        {
            return View();
        }
        public IActionResult Contact()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Teste()
        {
            return View();
        }
    }
}
