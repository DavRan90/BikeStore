using System;
using System.Collections.Generic;

namespace BikeStore.Models;

public partial class Cart
{
    public int Id { get; set; }

    public string? ProductId { get; set; }

    public int Amount { get; set; }

    public int? UserId { get; set; }

    public virtual Customer? User { get; set; }
}
