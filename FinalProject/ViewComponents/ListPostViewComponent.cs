using FinalProject.Context;
using FinalProject.Models;
using FinalProject.ViewModels.List;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace FinalProject.ViewComponents;

public class ListPostViewComponent : ViewComponent
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;


    public ListPostViewComponent(AppDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    [HttpPost]
    public async Task<IViewComponentResult> InvokeAsync([FromQuery] int? listId)
    {
        var CurrentUser = await _userManager.GetUserAsync((System.Security.Claims.ClaimsPrincipal)User);

        var list = await _context.UserLists.FirstOrDefaultAsync(l => l.Id == listId);
        if (list == null)
        {
            listId = HttpContext.Session.GetInt32("listId"); ;

            list = await _context.UserLists.FirstOrDefaultAsync(l => l.Id == listId);
        }

        var Owner = await _userManager.FindByIdAsync(list.OwnerId);
        var postQuery = _context.Posts.AsQueryable();

        var bookmarks = await _context.Bookmarks.Where(b => b.UserId == CurrentUser.Id).ToListAsync();
        var likes = await _context.Likes.Where(b => b.UserId == CurrentUser.Id).ToListAsync();
        var reposts = await _context.Reposts.Where(b => b.UserId == CurrentUser.Id).ToListAsync();
        var postImages = await _context.PostImages.ToListAsync();

        var Members = await _context.Members.Where(m => m.ListId == listId).ToListAsync();

        var MembersUserIds = Members.Select(f => f.UserId).ToList();



        var posts = await postQuery.Where(p => p.IsActive)
                                   .Where(p => p.User.IsActive == true)
                                   .Where(p => !p.IsReply)
                                   .Where(p => p.IsActive && MembersUserIds.Contains(p.UserId))
                                   .Include(p => p.User)
                                   .OrderByDescending(post => post.CreatedDate)
                                   .ToListAsync();     

        ListPostViewModel model = new()
        {
            Posts = posts,
            Members = Members,
            Bookmarks = bookmarks,
            Likes = likes,
            Reposts = reposts,
            PostImages = postImages,

        };



        return View(model);
    }


}
