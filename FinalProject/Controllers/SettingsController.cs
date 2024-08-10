using FinalProject.Context;
using FinalProject.Models;
using FinalProject.ViewModels.Settings;
using FinalProject.ViewModels.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.Controllers;

public class SettingsController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IConfiguration _configuration;
    public SettingsController(AppDbContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IConfiguration configuration)
    {
        _context = context;
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
    }

    public async Task<IActionResult> Index()
    {
        var CurrentUser = await _userManager.GetUserAsync(User);
        if (CurrentUser == null)
        {
            return NotFound();
        }

        UserUpdateViewModel model = new()
        {
            UserName = CurrentUser.UserName,
            Email = CurrentUser.Email,

        };

        return View(model);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> UpdateProfile(string type,UserUpdateViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return NotFound();


        if (type == "UserName")
        {
            if (user.UserName != model.UserName && _userManager.Users.Any(u => u.UserName == model.UserName))
            {
                TempData["ErrorMessage"] = "This Username already taken";
                return View(nameof(Index), model);
            }
            if (model.UserName == null)
            {
                TempData["ErrorMessage"] = "Username field required";
                return View(nameof(Index), model);
            }
            user.UserName = model.UserName;
        }
        else if (type == "Email")
        {
            if (user.Email != model.Email && _userManager.Users.Any(u => u.Email == model.Email))
            {
                TempData["ErrorMessage"] = "This Email already taken";
                return View(nameof(Index), model);
            }
            if (string.IsNullOrEmpty(model.Email))
            {
                TempData["ErrorMessage"] = "Email field required";
                return View(nameof(Index), model);
            }
            user.Email = model.Email;
        }
        else if (type == "Password")
        {
            if (model.CurrentPassword == null)
            {
                TempData["ErrorMessage"] = "Current Password field required";
                return View(nameof(Index), model);
            }
            if (model.CurrentPassword != null)
            {
                if (model.NewPassword == null)
                {
                    TempData["ErrorMessage"] = "New Password field required";
                    return View(nameof(Index), model);
                }
                if (model.ConfirmNewPassword == null)
                {
                    TempData["ErrorMessage"] = "Confirm Password field required";
                    return View(nameof(Index), model);
                }

                await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            }
        }

        await _userManager.UpdateAsync(user);

        await _signInManager.RefreshSignInAsync(user);

        TempData["SuccessMessage"] = "Your profile updated successfully";

        return RedirectToAction(nameof(Index));
    }

    [Authorize]
    public async Task<IActionResult> Deactivate(string type, UserUpdateViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        user.IsDeleted = true;

         await _userManager.UpdateAsync(user);
        return RedirectToAction("Logout", "Auth");
    }

}