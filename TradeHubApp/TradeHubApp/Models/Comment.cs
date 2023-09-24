﻿using Microsoft.AspNetCore.Identity;

namespace TradeHubApp.Models;

public class Comment
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public IdentityUser User { get; set; }
    public int AdvertisementId { get; set; }
    public Advertisement Advertisement { get; set; }
    public string Content { get; set; }
    public DateTime Timestamp { get; set; }
}
