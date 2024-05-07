using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using blogAppApi.Models;
using blogAppApi.Data;
using Microsoft.EntityFrameworkCore;
namespace blogApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AdminController : ControllerBase
{
    private readonly AppDbContext _context;
    public AdminController(AppDbContext context)
    {
        _context = context;
    }


    
   
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Admin>>> GetAdmins()
    {
       var admins = await _context.Admins.ToListAsync();
        return Ok(admins);
    }

    
    [HttpGet("{id}")]
    public async Task<ActionResult<Admin>> GetAdmin([FromRoute] int id)
    {
        var admin = await _context.Admins.FindAsync(id);

        if (admin == null)
        {
            return NotFound();
        }

        return Ok(admin);
    }

    
    [HttpPut("{id}")]
    public async Task<IActionResult> PutAdmin([FromRoute] int id, Admin admin)
    {
        if (id != admin.AdminId)
        {
            return BadRequest();
        }

        _context.Entry(admin).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!AdminExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    [HttpPost]
    public async Task<ActionResult<Admin>> PostAdmin(Admin admin)
    {
        if (AdminEmailExists(admin.Email))
        {
            return Conflict("Email is already taken");
        }

        _context.Admins.Add(admin);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetAdmin", new { id = admin.AdminId }, admin);
    }

   
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAdmin([FromRoute] int id)
    {
        var admin = await _context.Admins.FindAsync(id);
        if (admin == null)
        {
            return NotFound();
        }

        _context.Admins.Remove(admin);
        await _context.SaveChangesAsync();

        return NoContent();
    }
    private bool AdminExists(int id)
    {
        return _context.Admins.Any(e => e.AdminId == id);
    }
    private bool AdminEmailExists(string email)
    {
        return _context.Admins.Any(e => e.Email == email);
    }

 
    [HttpPost("Login")]
public async Task<ActionResult<Admin>> AdminLogin([FromBody] AdminLoginModel model)
{
    var adminInDb = await _context.Admins.FirstOrDefaultAsync(a => a.Email == model.Email);
    if (adminInDb == null)
    {
        return NotFound("Invalid email");
    }
    if (adminInDb.Password != model.Password)
    {
        return BadRequest("Invalid password");
    }
    
    return Ok(adminInDb);
}

public class AdminLoginModel
{
    public string Email { get; set; }
    public string Password { get; set; }
}

}
