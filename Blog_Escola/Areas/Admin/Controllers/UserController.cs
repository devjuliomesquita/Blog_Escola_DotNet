using AspNetCoreHero.ToastNotification.Abstractions;
using Blog_Escola.Models;
using Blog_Escola.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog_Escola.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly INotyfService _iNotyfService;
        public UserController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            INotyfService iNotyfService
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _iNotyfService = iNotyfService;
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var usersVM = users.Select(u => new UserVM()
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                UserName = u.UserName,
            }).ToList();
            return View(usersVM);
        }
        [HttpGet("Login")]
        public IActionResult Login()
        {
            if (!HttpContext.User.Identity!.IsAuthenticated)
            {
                return View(new LoginVM());
            }
            return RedirectToAction("Index", "User", new {area = "Admin"});
            
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid) { return View(loginVM); }
            var existinUser = await _userManager.Users.FirstOrDefaultAsync(e => e.UserName == loginVM.Username);
            if(existinUser == null)
            {
                _iNotyfService.Error("Usuário não existente.");
                return View(loginVM);
            }
            var verifyPassword = await _userManager.CheckPasswordAsync(existinUser, loginVM.Password);
            if(!verifyPassword)
            {
                _iNotyfService.Error("Senha inválida.");
                return View(loginVM);
            }
            await _signInManager.PasswordSignInAsync(loginVM.Username, loginVM.Password, loginVM.RememberMe, true);
            _iNotyfService.Success("Login realizado com sucesso");
            return RedirectToAction("Index", "User", new { area = "Admin"});
        }
        [HttpPost]
        public IActionResult Logout()
        {
            _signInManager.SignOutAsync();
            _iNotyfService.Success("Usuário deslogado.");
            return RedirectToAction("Index", "Home", new {area = ""});
        }
    }
}
