using FinalProject.Context;
using FinalProject.Models;
using FinalProject.ViewModels.HomeViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace FinalProject.ViewComponents;

public class CustomUserStatusViewComponent : ViewComponent
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;


    public CustomUserStatusViewComponent(AppDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    [HttpPost]
    public async Task<IViewComponentResult> InvokeAsync([FromQuery] string? userId, [FromQuery] int? postId)
    {
        if (userId == null)
        {
            userId = HttpContext.Session.GetString("CustomUserId");
        }
        var user = await _userManager.FindByIdAsync(userId);

        if (true)
        {
            postId = HttpContext.Session.GetInt32("CustomPostId");
        }

        var CurrentUser = await _userManager.GetUserAsync((System.Security.Claims.ClaimsPrincipal)User);
        var MainPost = await _context.Posts.FirstOrDefaultAsync(p => p.Id == postId);

        var postQuery = _context.Posts.AsQueryable();

        var bookmarks = await _context.Bookmarks.Where(b => b.UserId == CurrentUser.Id).ToListAsync();
        var likes = await _context.Likes.Where(b => b.UserId == CurrentUser.Id).ToListAsync();
        var reposts = await _context.Reposts.Where(b => b.UserId == CurrentUser.Id).ToListAsync();
        var postImages = await _context.PostImages.ToListAsync();

        var posts = await postQuery.Where(p => p.IsActive)
                                       .Where(p => p.User.IsActive == true)
                                       .Where(p => p.IsReply)
                                       .Where(p => p.CommentedPostId == postId)
                                       .Include(p => p.User)
                                       .OrderBy(post => post.CreatedDate)
                                       .ToListAsync();

      

        PostViewModel model = new()
        {
            CurrentUser = CurrentUser,
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
