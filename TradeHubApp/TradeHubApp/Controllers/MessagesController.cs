using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TradeHubApp.Data;
using TradeHubApp.Models;

namespace TradeHubApp.Controllers;

public class MessagesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    
    public MessagesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string userId)
    {
        var user = await _userManager.GetUserAsync(User);
        var messages = await _context.PrivateMessages
            .Where(pm => pm.SenderId == user.Id || pm.ReceiverId == user.Id)
            .Include(pm => pm.Sender)
            .Include(pm => pm.Receiver)
            .OrderBy(pm => pm.Timestamp)
            .ToListAsync();

        return View(messages);
    }
    
    [HttpPost]
    public async Task<IActionResult> Send(string receiverId, string content)
    {
        var sender = await _userManager.GetUserAsync(User);
        var receiver = await _userManager.FindByIdAsync("3ffb74a6-7f57-409f-a3e3-c999e709de6b");

        if (receiver == null) return NotFound("Receiver not found");

        var message = new PrivateMessage
        {
            SenderId = sender.Id,
            ReceiverId = receiver.Id,
            Content = content,
            Timestamp = DateTime.UtcNow
        };
        
        _context.PrivateMessages.Add(message);
        await _context.SaveChangesAsync();
        
        return RedirectToAction("Index");
    }
}
