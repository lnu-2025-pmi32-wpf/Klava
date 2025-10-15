﻿namespace Klava.DataSeeder.Models;

public class Team
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int? OwnerId { get; set; }
}

