namespace blogAppApi.Models;
using System.ComponentModel.DataAnnotations;

   public class Admin
{
    [Key]
    public int AdminId { get; set; }

    [Required]
    public string Username { get; set; }

    [Required]
    [MaxLength(20)]
    public string Email { get; set; }

    [Required]
    [MaxLength(20)]
    public string Password { get; set; }
}