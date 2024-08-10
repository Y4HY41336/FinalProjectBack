using FinalProject.Context;
using FinalProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.Controllers;

public class PremiumController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public PremiumController(AppDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }
        return View(user);
    }
    public async Task<IActionResult> SubscribePremium()
    {
        var user = await _userManager.GetUserAsync(User);
        user.IsPremium = true;
        user.IsVerified = false;
        user.IsGovermentVerified = false;
        _context.SaveChanges();
        return RedirectToAction("Index", "Home");
    }
    
    public async Task<IActionResult> VerifiedOrg()
    {
        var user = await _userManager.GetUserAsync(User);
        user.IsVerified = true;
        user.IsGovermentVerified = false;
        user.IsPremium = false;
        _context.SaveChanges();
        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> CancelSubscription()
    {
        var user = await _userManager.GetUserAsync(User);
        user.IsPremium = false;
        user.IsVerified = false;
        user.IsGovermentVerified = false;
        _context.SaveChanges();
        return RedirectToAction("Index", "Home");
    }
}
