using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TradeHubApp.Data;
using TradeHubApp.Models;

namespace TradeHubApp.Controllers;

public class PaymentController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public PaymentController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Index(decimal amount)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var userBalance = await _context.UserBalances
            .FirstOrDefaultAsync(ub => ub.UserId == user.Id) ?? new UserBalance { UserId = user.Id };
        
        userBalance.Balance += amount;

        if (_context.UserBalances.Any(ub => ub.UserId == user.Id))
        {
            _context.UserBalances.Update(userBalance);
        }
        else
        {
            await _context.UserBalances.AddAsync(userBalance);
        }

        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Home");
    }
}
