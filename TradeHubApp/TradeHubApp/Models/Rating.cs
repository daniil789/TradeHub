using Microsoft.AspNetCore.Identity;

namespace TradeHubApp.Models;

public class Rating
{
    public int Id { get; set; }
    public int AdvertisementId { get; set; }
    public Advertisement Advertisement { get; set; }
    public string UserId { get; set; }
    public IdentityUser User { get; set; }
    public int Value { get; set; } // значение оценки
    public DateTime Timestamp { get; set; }
}
