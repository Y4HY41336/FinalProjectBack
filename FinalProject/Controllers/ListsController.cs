using FinalProject.Context;
using FinalProject.Helpers.Extencions;
using FinalProject.Models;
using FinalProject.Services;
using FinalProject.ViewModels.HomeViewModels;
using FinalProject.ViewModels.List;
using FinalProject.ViewModels.ProfileViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

namespace FinalProject.Controllers;

public class ListsController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly NotificationService _notificationService;

    public ListsController(AppDbContext context, UserManager<AppUser> userManager, IWebHostEnvironment webHostEnvironment, NotificationService notificationService)
    {
        _context = context;
        _userManager = userManager;
        _webHostEnvironment = webHostEnvironment;
        _notificationService = notificationService;
    }

    [Authorize]
    public async Task<IActionResult> Index()
    {
        return View();
    }
    [Authorize]
    public async Task<IActionResult> Status(int listId)
    {
        HttpContext.Session.SetInt32("listId", listId);

        var CurrentUser = await _userManager.GetUserAsync(User);
        if (CurrentUser == null)
        {
            return NotFound();
        }
        var list = await _context.UserLists.FirstOrDefaultAsync(l => l.Id == listId);
        if (list == null)
        {
            return NotFound();
        }
        var owner = await _userManager.FindByIdAsync(list.OwnerId);
        if (owner == null)
        {
            return NotFound();
        }
        var follows = await _context.ListFollowers.FirstOrDefaultAsync(f => f.UserId == CurrentUser.Id && f.ListId == listId);
        ListInfoViewModel model = new()
        {
            CurrentUser = CurrentUser,
            Owner = owner,
            List = list,
            Follows = follows,
        };

        return View(model);
    }

    [Authorize]
    public async Task<IActionResult> AddMember(string userId, int listId)
    {
        var CurrentUser = await _userManager.GetUserAsync(User);
        var Member = await _userManager.FindByIdAsync(userId);

        var List = await _context.UserLists.FirstOrDefaultAsync(l => l.Id == listId);
        if (List == null)
        {
            return NotFound();
        }

        var Members = await _context.Members.FirstOrDefaultAsync(m => m.UserId == Member.Id && m.ListId == listId);

        if (Members == null)
        {
            Member newMember = new()
            {
                ListId = listId,
                UserId = userId,
            };
            List.MemberCount++;            
            await _context.Members.AddAsync(newMember);
            await _context.SaveChangesAsync();
        }
        else
        {
            List.MemberCount--;
            _context.Members.Remove(Members);
            await _context.SaveChangesAsync();
        }

        
        return RedirectToAction("Status", new { listId = listId });
    }


    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateList(CreateListViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        UserList list = new()
        {
            Name = model.Name,
            Description = model.Description,
            IsPrivate = model.IsPrivate,
            OwnerId = user.Id,
            CreatedDate = DateTime.UtcNow,
        };

        await _context.UserLists.AddAsync(list);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> UpdateList(int listId ,CreateListViewModel model)
    {
        var list = await _context.UserLists.FirstOrDefaultAsync(l => l.Id == listId);
        if (list == null)
        {
            return NotFound();
        }

        list.Name = model.Name;
        list.Description = model.Description;
        list.IsPrivate = model.IsPrivate;

        var image = model.ProfilePhoto;
        if (image != null)
        {
            if (!image.CheckFileType("image/"))
            {
                return RedirectToAction("Status", new { listId = listId });
            }
            string fileName = $"{Guid.NewGuid()}-{image.FileName}";
            string path = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "images", "ProfileImages", "ListPhotos", fileName);
            FileStream stream = new FileStream(path, FileMode.Create);
            await image.CopyToAsync(stream);
            stream.Dispose();

            list.ProfilePhoto = fileName;
        }


        _context.UserLists.Update(list);

        await _context.SaveChangesAsync();

        return RedirectToAction("Status", new { listId = listId });
    }

    [Authorize]
    public async Task<IActionResult> DeleteList(int listId)
    {
        var list = await _context.UserLists.FirstOrDefaultAsync(l => l.Id == listId);
        if (list == null)
        {
            return NotFound();
        }

        list.IsDeleted = true;

        _context.UserLists.Update(list);

        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }


    [Authorize]
    public async Task<IActionResult> FollowList(int listId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }
        var list = await _context.UserLists.FirstOrDefaultAsync(p => p.Id == listId);
        if (list == null)
        {
            return NotFound();
        }

        var listFollower = await _context.ListFollowers.FirstOrDefaultAsync(p => p.ListId == listId && p.UserId == user.Id);
        if (listFollower == null)
        {
            ListFollower newListFollower = new()
            {
                ListId = listId,
                UserId = user.Id,
            };
            list.FollowerCount++;
            await _context.ListFollowers.AddAsync(newListFollower);
        }
        else
        {
            list.FollowerCount--;
            if(list.FollowerCount <= 0)
            {
                list.FollowerCount = 0;
            }
            _context.Remove(listFollower);
        }

        
        await _context.SaveChangesAsync();

        return ViewComponent("List");
    }

    [Authorize]
    public async Task<IActionResult> Follow(string userId)
    {
        var CurrentUser = await _userManager.GetUserAsync(User);
        var Followed = await _userManager.FindByIdAsync(userId);

        var follow = await _context.Follows.FirstOrDefaultAsync(f => f.FollowerId == CurrentUser.Id && f.FollowedId == Followed.Id);

        if (follow == null)
        {
            Follow newFollow = new()
            {
                FollowedId = Followed.Id,
                FollowerId = CurrentUser.Id,
                CreatedDate = DateTime.Now,
            };
            Followed.FollowerCount++;
            CurrentUser.FollowingCount++;
            await _context.Follows.AddAsync(newFollow);
            await _context.SaveChangesAsync();
        }
        else
        {
            Followed.FollowerCount--;
            CurrentUser.FollowingCount--;
            _context.Follows.Remove(follow);
            await _context.SaveChangesAsync();
        }

        var listId = HttpContext.Session.GetInt32("listId");
        return RedirectToAction("Status", new { listId = listId });
    }

    [Authorize]
    public async Task<IActionResult> Repost(int postId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var post = await _context.Posts.Where(p => p.IsActive).FirstOrDefaultAsync(p => p.Id == postId);
        if (post == null)
        {
            return NotFound();
        }

        var repost = await _context.Reposts.FirstOrDefaultAsync(r => r.PostId == postId && r.UserId == user.Id);
        if (repost == null)
        {
            Repost newRepost = new()
            {
                PostId = postId,
                UserId = user.Id,
                CreatedDate = DateTime.UtcNow,
            };
            post.RepostCount++;
            await _context.Reposts.AddAsync(newRepost);
            if (user.Id != post.UserId)
            {
                await _notificationService.SendNotificationAsync(user.Id, post.UserId, postId, "Repost", $"{user.Name} reposted your post");
            }
        }
        else
        {
            post.RepostCount--;
            _context.Remove(repost);
        }

        await _context.SaveChangesAsync();

        return ViewComponent("ListPost");
    }

    [Authorize]
    public async Task<IActionResult> Like(int postId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var post = await _context.Posts.Where(p => p.IsActive).FirstOrDefaultAsync(p => p.Id == postId);
        if (post == null)
        {
            return NotFound();
        }

        var like = await _context.Likes.FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == user.Id);
        if (like == null)
        {
            Like newlike = new()
            {
                PostId = postId,
                UserId = user.Id,
                CreatedDate = DateTime.UtcNow,
            };
            post.LikeCount++;
            await _context.Likes.AddAsync(newlike);
            if (user.Id != post.UserId)
            {
                await _notificationService.SendNotificationAsync(user.Id, post.UserId, postId, "Like", $"{user.Name} liked your post");
            }
        }
        else
        {
            post.LikeCount--;
            _context.Remove(like);
        }

        await _context.SaveChangesAsync();

        return ViewComponent("ListPost");
    }

    [Authorize]
    public async Task<IActionResult> Bookmark(int postId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var post = await _context.Posts.Where(p => p.IsActive).FirstOrDefaultAsync(p => p.Id == postId);
        if (post == null)
        {
            return NotFound();
        }

        var bookMark = await _context.Bookmarks.FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == user.Id);
        if (bookMark == null)
        {
            Bookmark newBookmark = new()
            {
                PostId = postId,
                UserId = user.Id,
                CreatedDate = DateTime.UtcNow,
            };
            post.BookmarkCount++;
            await _context.Bookmarks.AddAsync(newBookmark);

        }
        else
        {
            post.BookmarkCount--;
            _context.Remove(bookMark);
        }

        await _context.SaveChangesAsync();

        return ViewComponent("ListPost");
    }

    [Authorize]
    public async Task<IActionResult> DeletePost(int postId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }
        var post = await _context.Posts.Where(p => p.IsActive).FirstOrDefaultAsync(p => p.Id == postId);
        if (post == null)
        {
            return NotFound();
        }
        var postTrends = await _context.PostTrends.Where(p => p.PostId == postId).ToListAsync();
        if (postTrends != null)
        {
            foreach (var postTrend in postTrends)
            {
                var trend = await _context.Trends.FirstOrDefaultAsync(p => p.Id == postTrend.TrendId);
                trend.PostCount--;
            }


        }

        post.IsActive = false;
        await _context.SaveChangesAsync();

        return ViewComponent("ListPost");
    }


    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreatePost(CreatePostViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        Post newPost = new()
        {
            Content = model.Content,
            Image = model.Image,
            CreatedDate = DateTime.UtcNow,
            UserId = user.Id,
            IsActive = true,
        };
        await _context.Posts.AddAsync(newPost);
        await _context.SaveChangesAsync();


        model.PostId = newPost.Id;
        return RedirectToAction("CreateTrend", new { PostId = newPost.Id, TrendName = model.Content });
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> ReplyPost(CreatePostViewModel model, int postId)
    {
        var user = await _userManager.GetUserAsync(User);
        var post = await _context.Posts.Where(p => p.IsActive).FirstOrDefaultAsync(p => p.Id == postId);
        if (post == null)
        {
            return NotFound();
        }
        Post newPost = new()
        {
            Content = model.Content,
            Image = model.Image,
            CreatedDate = DateTime.UtcNow,
            UserId = user.Id,
            IsActive = true,
            CommentedPostId = post.Id,
            IsReply = true,
        };
        post.ReplyCount++;
        await _context.Posts.AddAsync(newPost);
        await _context.SaveChangesAsync();

        if (user.Id != post.UserId)
        {
            await _notificationService.SendNotificationAsync(user.Id, post.UserId, postId, "Reply", newPost.Content);
        }

        model.PostId = newPost.Id;
        return RedirectToAction("CreateTrend", new { PostId = newPost.Id, TrendName = model.Content });
    }

    [Authorize]
    public async Task<IActionResult> CreateTrend(CreatePostViewModel model)
    {
        string pattern = @"#\w+";

        var trendNames = Regex.Matches(model.TrendName, pattern);

        foreach (var name in trendNames)
        {
            var trend = await _context.Trends.FirstOrDefaultAsync(t => t.TrendName == name.ToString());

            if (trend == null)
            {
                Trend newTrend = new()
                {
                    TrendName = name.ToString(),
                    CreatedDate = DateTime.Now,
                };
                newTrend.PostCount++;
                await _context.Trends.AddAsync(newTrend);
                await _context.SaveChangesAsync();

                PostTrend newPostTrend = new()
                {
                    PostId = model.PostId,
                    TrendId = newTrend.Id,
                };

                await _context.PostTrends.AddAsync(newPostTrend);
                await _context.SaveChangesAsync();

            }
            else
            {
                trend.PostCount++;
                PostTrend newPostTrend = new()
                {
                    PostId = model.PostId,
                    TrendId = trend.Id,
                };

                await _context.PostTrends.AddAsync(newPostTrend);
            }
        }
        await _context.SaveChangesAsync();
        var listId = HttpContext.Session.GetInt32("listId");
        return RedirectToAction("Status", new {listId = listId});
    }
    
    [Authorize]
    public async Task<IActionResult> Filter(string? TabId, string? userId)
    {
        if (TabId == "Members" || TabId == "Suggested")
        {
            userId = HttpContext.Session.GetString("FollowersUserId");
            return ViewComponent("Followers", new { TabId = TabId, userId = userId });
        }

        return ViewComponent("CustomUserPost", new { TabId = TabId, userId = userId });

    }


}
