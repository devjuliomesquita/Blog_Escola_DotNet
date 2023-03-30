using AspNetCoreHero.ToastNotification.Abstractions;
using Blog_Escola.Models;
using Blog_Escola.ViewModels;
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
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet("Login")]
        public IActionResult Login()
        {
            return View(new LoginVM());
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

    }
}
