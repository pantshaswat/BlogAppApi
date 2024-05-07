
namespace blogAppApi.Data;
using Microsoft.EntityFrameworkCore;
using blogAppApi.Models;
public class AppDbContext: DbContext{

public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
{
}
        public DbSet<User> Users { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Reaction> Reactions { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Admin> Admins { get; set; }

         protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(u => u.Blogs)
                .WithOne(b => b.Author)
                .HasForeignKey(b => b.UserId);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Comments)
                .WithOne(c => c.Commenter)
                .HasForeignKey(c => c.UserId);

            modelBuilder.Entity<Blog>()
                .HasMany(b => b.Reactions)
                .WithOne(r => r.Blog)
                .HasForeignKey(r => r.BlogId);

            modelBuilder.Entity<Blog>()
                .HasMany(b => b.Comments)
                .WithOne(c => c.Blog)
                .HasForeignKey(c => c.BlogId);

            modelBuilder.Entity<Comment>()
                .HasMany(c => c.Reactions)
                .WithOne(r => r.Comment)
                .HasForeignKey(r => r.CommentId);

            modelBuilder.Entity<Reaction>()
                .HasOne(r => r.Reactor)
                .WithMany(u => u.Reactions)
                .HasForeignKey(r => r.UserId);

            modelBuilder.Entity<Reaction>()
                .HasOne(r => r.Blog)
                .WithMany(b => b.Reactions)
                .HasForeignKey(r => r.BlogId);

            modelBuilder.Entity<Reaction>()
                .HasOne(r => r.Comment)
                .WithMany(c => c.Reactions)
                .HasForeignKey(r => r.CommentId);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId);
        }
}