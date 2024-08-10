﻿using FinalProject.Helpers;
using FinalProject.Helpers.Enums;
using FinalProject.Models;
using FinalProject.ViewModels.AuthViewModels;
using FinalProject.ViewModels.HelperViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public AuthController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IConfiguration configuration, RoleManager<IdentityRole> roleManager, IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _roleManager = roleManager;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
            {
                return View();
            }
            var user = await _userManager.FindByNameAsync(model.UsernameOrEmail);
            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(model.UsernameOrEmail);
                if (user == null)
                {
                    ModelState.AddModelError("", "Username/Email or Password incorrect");
                    return View();
                }
            }
            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                ModelState.AddModelError("", "Please confirm your email address");
                return View();
            }

            if (!user.IsActive)
            {
                ModelState.AddModelError("", "This account has been suspended");
                return View(model);
            }
            if (user.IsDeleted)
            {
                ModelState.AddModelError("", "This account doesn't exist");
                return View(model);
            }
            var signInResault = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, true, false);
            if (!signInResault.Succeeded)
            {
                ModelState.AddModelError("", "Username/Email or Password incorrect");
                return View();
            }

            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> Logout()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return BadRequest();
            }
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Auth");
        }

        public async Task<IActionResult> ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel forgotPasswordViewModel)
        {
            if (!ModelState.IsValid)
                return View();

            var user = await _userManager.FindByEmailAsync(forgotPasswordViewModel.Email);
            if (user == null)
            {
                ModelState.AddModelError("Email", "Email not found");
                return View();
            }

            string token = await _userManager.GeneratePasswordResetTokenAsync(user);
            string link = Url.Action("ResetPassword", "Auth", new { email = user.Email, token }, HttpContext.Request.Scheme, HttpContext.Request.Host.Value);

            string path = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "templates", "ResetPassword.html");
            using StreamReader streamReader = new StreamReader(path);
            string content = await streamReader.ReadToEndAsync();
            string body = content.Replace("[link]", link);

            EmailHelper emailHelper = new EmailHelper(_configuration);
            await emailHelper.SendEmailAsync(new MailRequest { ToEmail = user.Email, Subject = "ResetPassword", Body = body });

            return RedirectToAction(nameof(Login));
        }

        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel resetPasswordViewModel)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordViewModel.Email);
            if (user == null)
                return NotFound();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(SubmitResetPasswordViewModel submitResetPasswordViewModel, string email, string token)
        {
            if (!ModelState.IsValid)
                return View();

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return NotFound();

            IdentityResult identityResult = await _userManager.ResetPasswordAsync(user, token, submitResetPasswordViewModel.Password);
            if (!identityResult.Succeeded)
            {
                foreach (var error in identityResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View();
            }

            return RedirectToAction(nameof(Login));
        }
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return NotFound();

            if (await _userManager.IsEmailConfirmedAsync(user))
                return BadRequest();

            IdentityResult identityResult = await _userManager.ConfirmEmailAsync(user, model.Token);
            if (identityResult.Succeeded)
            {
                TempData["ConfirmationMessage"] = "Your email successfully confirmed";
                return RedirectToAction("Login", "Auth");
            }

            return BadRequest();
        }

        public async Task<IActionResult> CreateRole()
        {
            foreach (var roleName in Enum.GetNames(typeof(Roles)))
            {
                await _roleManager.CreateAsync(new IdentityRole { Name = roleName });
            }

            await _roleManager.CreateAsync(new IdentityRole { Name = "Admin" });
            await _roleManager.CreateAsync(new IdentityRole { Name = "User" });
            await _roleManager.CreateAsync(new IdentityRole { Name = "Moderator" });

            return Content("Butun rollar yarandi");
        }
    }
}
