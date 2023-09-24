﻿using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace TradeHubApp.Models;

public class UserBalance
{
    [Key]
    public string UserId { get; set; }
    public IdentityUser User { get; set; }
    public decimal Balance { get; set; }
}
