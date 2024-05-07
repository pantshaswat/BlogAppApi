
namespace blogAppApi.Models;
using System.ComponentModel.DataAnnotations;


public class Notification
{
    [Key]
    public int NotificationId { get; set; }

    public string Content { get; set; }

    public DateTime NotificationDate { get; set; }

    public int UserId { get; set; }
    public User? User { get; set; }
}