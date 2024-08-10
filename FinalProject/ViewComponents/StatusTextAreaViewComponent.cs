using FinalProject.Context;
using FinalProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.ViewComponents;

public class StatusTextAreaViewComponent : ViewComponent
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;


    public StatusTextAreaViewComponent(AppDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    [HttpPost]
    public async Task<IViewComponentResult> InvokeAsync()
    {
        ViewBag.postId = HttpContext.Session.GetInt32("CustomPostId");

        ViewBag.CurrentUser = await _userManager.GetUserAsync((System.Security.Claims.ClaimsPrincipal)User);

        return View();
    }
}
