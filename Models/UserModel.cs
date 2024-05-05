namespace blogAppApi.Models;
using System.ComponentModel.DataAnnotations;

   public class User
{
    [Key]
    public int UserId { get; set; }

    [Required]
    public string Username { get; set; }

    [Required]
    [MaxLength(20)]
    public string Email { get; set; }

    [Required]
    [MaxLength(20)]
    public string? Password { get; set; }

    public ICollection<Blog>? Blogs { get; set; }
    public ICollection<Comment>? Comments { get; set; }
    public ICollection<Reaction>? Reactions { get; set; }
    public ICollection<Notification>? Notifications { get; set; }
}