using Microsoft.AspNetCore.Identity;

namespace TradeHubApp.Models;

public class PrivateMessage
{
    public int Id { get; set; }
    public string SenderId { get; set; }
    public IdentityUser Sender { get; set; }
    public string ReceiverId { get; set; }
    public IdentityUser Receiver { get; set; }
    public string Content { get; set; }
    public DateTime Timestamp { get; set; }
}
