﻿using Microsoft.AspNetCore.Mvc;

namespace Blog_Escola.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet("Login")]
        public IActionResult Login()
        {
            return View();
        }

    }
}
