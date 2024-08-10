using FinalProject.Context;
using FinalProject.Models;
using FinalProject.ViewModels.CustomUser;
using FinalProject.ViewModels.HomeViewModels;
using FinalProject.ViewModels.ProfileViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace FinalProject.ViewComponents;

public class StatusMainPostViewComponent : ViewComponent
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;


    public StatusMainPostViewComponent(AppDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    [HttpPost]
    public async Task<IViewComponentResult> InvokeAsync([FromQuery] int? postId)
    {
        //HttpContext.Session.SetInt32("CustomPostId", postId);

        HttpContext.Session.SetString("PreviousAction", "Status");

        if (postId == null)
        {
            postId = HttpContext.Session.GetInt32("CustomPostId");
        }

        var currentUser = await _userManager.GetUserAsync((System.Security.Claims.ClaimsPrincipal)User);

        var mainpost = await _context.Posts.FirstOrDefaultAsync(p => p.Id == postId);

        var user = await _userManager.FindByIdAsync(mainpost.UserId);

        HttpContext.Session.SetString("CustomUserId", user.Id);

        var replies = await _context.Posts.Where(p => p.IsReply && p.CommentedPostId == postId).ToListAsync();
        var follows = await _context.Follows.Where(f => f.FollowerId == currentUser.Id).ToListAsync();

        var bookmarks = await _context.Bookmarks.Where(b => b.UserId == user.Id).ToListAsync();
        var likes = await _context.Likes.Where(b => b.UserId == user.Id).ToListAsync();
        var reposts = await _context.Reposts.Where(b => b.UserId == user.Id).ToListAsync();
        var postImages = await _context.PostImages.Where(b => b.PostId == postId).ToListAsync();
        
        StatusMainPostViewModel model = new()
        {
            PostOwner = user,
            CurrentUser = currentUser,
            Replies = replies,
            Follows = follows,
            MainPost = mainpost,
            Bookmarks = bookmarks,
            Likes = likes,
            Reposts = reposts,
            PostImages = postImages,
        };

        return View(model);

    }
}
