using FinalProject.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Context;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions options) : base(options)
    {
    }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Repost> Reposts { get; set; }
    public DbSet<Like> Likes { get; set; }
    public DbSet<Trend> Trends { get; set; }
    public DbSet<PostTrend> PostTrends { get; set; }
    public DbSet<Follow> Follows { get; set; }
    public DbSet<Bookmark> Bookmarks { get; set; }
    public DbSet<PostImage> PostImages { get; set; }
    public DbSet<UserList> UserLists { get; set; }
    public DbSet<Member> Members { get; set; }
    public DbSet<ListFollower> ListFollowers { get; set; }
    public DbSet<Notification> Notifications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Post>()
            .HasOne(p => p.User)
            .WithMany(u => u.Posts)
            .HasForeignKey(p => p.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Post>()
            .HasOne(p => p.CommentedPost)
            .WithMany(p => p.Comments)
            .HasForeignKey(p => p.CommentedPostId)
            .OnDelete(DeleteBehavior.Restrict);


        modelBuilder.Entity<Repost>()
            .HasOne(r => r.User)
            .WithMany(r => r.Reposts)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);


        modelBuilder.Entity<Repost>()
            .HasOne(r => r.Post)
            .WithMany(r => r.Reposts)
            .HasForeignKey(r => r.PostId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Like>()
            .HasOne(l => l.Post)
            .WithMany(l => l.Likes)
            .HasForeignKey(l => l.PostId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Like>()
            .HasOne(l => l.User)
            .WithMany(l => l.Likes)
            .HasForeignKey(l => l.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Follow>()
            .HasKey(f => f.Id);

        modelBuilder.Entity<Follow>()
            .HasOne(f => f.Follower)
            .WithMany(u => u.Followed)
            .HasForeignKey(f => f.FollowerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Follow>()
            .HasOne(f => f.Followed)
            .WithMany(u => u.Follower)
            .HasForeignKey(f => f.FollowedId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Bookmark>()
            .HasOne(b => b.User)
            .WithMany(u => u.Bookmarks)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Bookmark>()
            .HasOne(b => b.Post)
            .WithMany(p => p.Bookmarks)
            .HasForeignKey(b => b.PostId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PostImage>()
            .HasOne(pi => pi.Post)
            .WithMany(p => p.PostImages)
            .HasForeignKey(pi => pi.PostId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserList>()
            .HasMany(l => l.Members)
            .WithOne(m => m.List)
            .HasForeignKey(m => m.ListId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserList>()
            .HasMany(l => l.ListFollowers)
            .WithOne(lf => lf.List)
            .HasForeignKey(lf => lf.ListId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Member>()
            .HasOne(m => m.User)
            .WithMany(u => u.Members)
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ListFollower>()
            .HasOne(lf => lf.User)
            .WithMany(u => u.ListFollowers)
            .HasForeignKey(lf => lf.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Notification>()
            .HasOne(n => n.User)
            .WithMany(u => u.SentNotifications)
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Notification>()
            .HasOne(n => n.Receiver)
            .WithMany(u => u.ReceivedNotifications)
            .HasForeignKey(n => n.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Notification>()
            .HasOne(n => n.Post)
            .WithMany(p => p.Notifications)
            .HasForeignKey(n => n.PostId)
            .OnDelete(DeleteBehavior.Restrict);

    }


}
