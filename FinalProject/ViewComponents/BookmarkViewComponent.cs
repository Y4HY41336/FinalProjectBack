using FinalProject.Context;
using FinalProject.Models;
using FinalProject.ViewModels.HomeViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace FinalProject.ViewComponents;

public class BookmarkViewComponent : ViewComponent
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    public BookmarkViewComponent(AppDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var user = await _userManager.GetUserAsync((System.Security.Claims.ClaimsPrincipal)User);
     
        var likes = await _context.Likes.Where(b => b.UserId == user.Id).ToListAsync();
        var reposts = await _context.Reposts.Where(b => b.UserId == user.Id).ToListAsync();
        var bookmarks = await _context.Bookmarks.Where(b => b.UserId == user.Id).ToListAsync();

        var bookmakedPostIds = bookmarks.Select(f => f.PostId).ToList();

        var postImages = await _context.PostImages.ToListAsync();

        var posts = await _context.Posts.Where(p => p.IsActive)
                                        .Where(p => p.User.IsActive == true)
                                        .Where(p => bookmakedPostIds.Contains(p.Id))
                                        .Include(p => p.User)
                                        .OrderByDescending(post => post.CreatedDate)
                                        .ToListAsync();



        PostViewModel model = new()
        {
            Posts = posts,
            User = user,
            Bookmarks = bookmarks,
            Likes = likes,
            Reposts = reposts,
            PostImages = postImages,

        };

        return View(model);


    }
}
