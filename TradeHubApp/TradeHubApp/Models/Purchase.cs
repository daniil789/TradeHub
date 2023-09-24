using Microsoft.AspNetCore.Identity;

namespace TradeHubApp.Models;

public class Purchase
{
    public int Id { get; set; }
    public int AdvertisementId { get; set; }
    public Advertisement Advertisement { get; set; }
    public string BuyerId { get; set; }
    public IdentityUser Buyer { get; set; }
    public DateTime Timestamp { get; set; }
}
