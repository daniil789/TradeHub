using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TradeHubApp.Data;
using TradeHubApp.Models;

public class AdvertisementsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public AdvertisementsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    //[ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Advertisement advertisement)
    {
        if (ModelState.IsValid)
        {
            advertisement.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Задаем ID пользователя, который создает объявление
            _context.Add(advertisement);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(advertisement);
    }
    
    [Authorize]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var advertisement = await _context.Advertisements.FindAsync(id);
        if (advertisement == null) return NotFound();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (advertisement.UserId != userId) return Forbid(); // Пользователь может редактировать только свои объявления

        return View(advertisement);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Edit(int id, Advertisement advertisement)
    {
        if (id != advertisement.Id) return NotFound();
        if (!UserOwnsAdvertisement(id)) return Forbid(); // Пользователь может редактировать только свои объявления

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(advertisement);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
              return NotFound();
            }
            return RedirectToAction(nameof(Index));
        }
        return View(advertisement);
    }

    private bool UserOwnsAdvertisement(int id)
    {
        var advertisement = _context.Advertisements.Find(id);
        if (advertisement == null) return false;
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return advertisement.UserId == userId;
    }

    [Authorize]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var advertisement = await _context.Advertisements
            .FirstOrDefaultAsync(m => m.Id == id);
        if (advertisement == null) return NotFound();

        if (advertisement.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            return Forbid(); // Пользователь может удалять только свои объявления

        return View(advertisement);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var advertisement = await _context.Advertisements.FindAsync(id);
        if (advertisement.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            return Forbid(); // Пользователь может удалять только свои объявления

        _context.Advertisements.Remove(advertisement);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [Authorize]
    public async Task<IActionResult> MyAdvertisements(int? pageNumber)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var advertisements = _context.Advertisements
            .Where(a => a.UserId == userId)
            .OrderBy(a => a.Title);

        int pageSize = 5;
        return View(await PaginatedList<Advertisement>.CreateAsync(advertisements.AsNoTracking(), pageNumber ?? 1, pageSize));
    }


    public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
    {
        ViewData["CurrentSort"] = sortOrder;
        ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
        ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";

        if (searchString != null)
        {
            pageNumber = 1;
        }
        else
        {
            searchString = currentFilter;
        }

        ViewData["CurrentFilter"] = searchString;

        var advertisements = from a in _context.Advertisements
            select a;

        if (!String.IsNullOrEmpty(searchString))
        {
            advertisements = advertisements.Where(s => s.Title.Contains(searchString));
        }

        switch (sortOrder)
        {
            case "name_desc":
                advertisements = advertisements.OrderByDescending(s => s.Title);
                break;
            case "Date":
                advertisements = advertisements.OrderBy(s => s.PublicationDate);
                break;
            case "date_desc":
                advertisements = advertisements.OrderByDescending(s => s.PublicationDate);
                break;
            default:
                advertisements = advertisements.OrderBy(s => s.Title);
                break;
        }

        int pageSize = 5;
        return View(await PaginatedList<Advertisement>.CreateAsync(advertisements.AsNoTracking(), pageNumber ?? 1, pageSize));
    }
    
    [HttpPost]
    public async Task<IActionResult> AddComment(int advertisementId, string content)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var comment = new Comment
        {
            UserId = userId,
            AdvertisementId = advertisementId,
            Content = content,
            Timestamp = DateTime.UtcNow
        };
    
        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();
    
        return RedirectToAction("Details", new { id = advertisementId });
    }
    
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var advertisement = await _context.Advertisements
            .Include(a => a.Comments)
            .ThenInclude(c => c.User)
            .FirstOrDefaultAsync(m => m.Id == id);
        
        if (advertisement == null)
        {
            return NotFound();
        }
        
        return View(advertisement);
    }
    
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddRating(int advertisementId, int value)
    {
        var user = await _userManager.GetUserAsync(User);
        var hasPurchased = await _context.Purchases.AnyAsync(p => p.AdvertisementId == advertisementId && p.BuyerId == user.Id);

        if (!hasPurchased)
        {
            return Forbid("You can only rate products you have purchased.");
        }

        var rating = new Rating
        {
            AdvertisementId = advertisementId,
            UserId = user.Id,
            Value = value,
            Timestamp = DateTime.UtcNow
        };
    
        _context.Ratings.Add(rating);
        await _context.SaveChangesAsync();
    
        return RedirectToAction(nameof(Details), new { id = advertisementId });
    }


}