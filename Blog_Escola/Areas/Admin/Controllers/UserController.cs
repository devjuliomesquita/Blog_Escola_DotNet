﻿using AspNetCoreHero.ToastNotification.Abstractions;
using Blog_Escola.Models;
using Blog_Escola.Utilites;
using Blog_Escola.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

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
                Email = u.Email,
            }).ToList();

            foreach (var user in usersVM)
            {
                var singleUser = await _userManager.FindByIdAsync(user.Id);
                var role = await _userManager.GetRolesAsync(singleUser);
                user.Role = role.FirstOrDefault();
            }
            return View(usersVM);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> ResetPassword(string id)
        {
            //Encontrar o Usuário
            var existingUser = await _userManager.FindByIdAsync(id);
            if (existingUser == null)
            {
                _iNotyfService.Error("Usuário não encontrado.");
                return View();
            }
            var resetPasswordUser = new ResetPasswordIM() 
            { 
                Id = existingUser.Id,
                UserName = existingUser.UserName,
            };
            return View(resetPasswordUser);

        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordIM resetPasswordIM)
        {
            //Checar se o formulário é válido
            if (!ModelState.IsValid) { return View(resetPasswordIM); }
            //Checar a existência desse usuário
            var existingUser = await _userManager.FindByIdAsync(resetPasswordIM.Id);
            if(existingUser == null)
            {
                _iNotyfService.Error("Usuário não encontrado.");
                return View(resetPasswordIM);
            }
            //Criando as variáveis para realizar o update
            var token = await _userManager.GeneratePasswordResetTokenAsync(existingUser);
            var resultResetPassword = await _userManager.ResetPasswordAsync(existingUser, token, resetPasswordIM.NewPassword);
            if(resultResetPassword.Succeeded)
            {
                _iNotyfService.Success("Senha Alterada com Sucesso.");
                return RedirectToAction(nameof(Index));
            }
            return View(resetPasswordIM);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Register()
        {
            return View( new RegisterVM());
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            //Checar se o formulário é válido
            if(!ModelState.IsValid) return View(registerVM);

            //Checando se já existe email ou usuário
            var checkUserByEmail = await _userManager.FindByEmailAsync(registerVM.Email);
            if(checkUserByEmail != null)
            {
                _iNotyfService.Error("Este e-mail já existe.");
                return View(registerVM);
            }
            var checkUserByUsername = await _userManager.FindByNameAsync(registerVM.UserName);
            if(checkUserByUsername != null)
            {
                _iNotyfService.Error("Este usuário já existe.");
                return View(registerVM);
            }
            //Criando o objeto ApplicatiomUser
            var applicationUser = new ApplicationUser()
            {
                Email = registerVM.Email,
                UserName = registerVM.UserName,
                FirstName = registerVM.FirstName,
                LastName = registerVM.LastName,
            };
            var resultUser = await _userManager.CreateAsync(applicationUser, registerVM.Password);
            //teste do resultado para admin ou não
            if (resultUser.Succeeded)
            {
                if (registerVM.IsAdmin)
                {
                    await _userManager.AddToRoleAsync(applicationUser, WebSiteRoles.WebSiteAdmin);
                }
                else
                {
                    await _userManager.AddToRoleAsync(applicationUser, WebSiteRoles.WebSiteAuthor);
                }
                _iNotyfService.Success("Usuário criado com sucesso.");
                return RedirectToAction("Index", "User", new {area = "Admin"});
            }
            return View(registerVM);
           
        }
        public async Task<IActionResult> Delete(string id)
        {
            //Encontrar o Usuário
            var existingUser = await _userManager.FindByIdAsync(id);
            //Teste
            if (existingUser == null)
            {
                _iNotyfService.Error("Usuário não encontrado.");
                return View();
            }

            var loggedInUser = await _userManager.Users.FirstOrDefaultAsync(l => l.UserName == User.Identity!.Name);
            var loggedInUserRole = await _userManager.GetRolesAsync(loggedInUser!);
            if (loggedInUserRole[0] == WebSiteRoles.WebSiteAdmin)
            {
                await _userManager.DeleteAsync(existingUser);
                _iNotyfService.Warning("Usuário Apagado.");
                return RedirectToAction("Index", "User", new { area = "Admin" });
            }
            return View();
        }
        [HttpGet("Login")]
        public IActionResult Login()
        {
            if (!HttpContext.User.Identity!.IsAuthenticated)
            {
                return View(new LoginVM());
            }
            return RedirectToAction("Index", "Post", new {area = "Admin"});
            
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
            return RedirectToAction("Index", "Post", new { area = "Admin"});
        }
        [Authorize]
        [HttpPost]
        public IActionResult Logout()
        {
            _signInManager.SignOutAsync();
            _iNotyfService.Success("Usuário deslogado.");
            return RedirectToAction("Index", "Home", new {area = ""});
        }
        [HttpGet("AccessDenied")]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
