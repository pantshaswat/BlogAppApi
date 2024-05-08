

using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using blogAppApi.Models;
using blogAppApi.Data;
using Microsoft.EntityFrameworkCore;
namespace blogApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NotificationController : ControllerBase
{
    private readonly AppDbContext _context;
    public NotificationController(AppDbContext context)
    {
        _context = context;
    }


    
    // GET: api/User
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Notification>>> GetNotifications()
    {
        var notifications = await _context.Notifications.ToListAsync();
        return Ok(notifications);
    }
    [HttpGet("{userId}")]
    public async Task<ActionResult<IEnumerable<Notification>>> GetNotificationsByUserId([FromRoute] int userId)
    {
        var notifications = await _context.Notifications.Where(n => n.UserId == userId).ToListAsync();
        return Ok(notifications);
    }
}
