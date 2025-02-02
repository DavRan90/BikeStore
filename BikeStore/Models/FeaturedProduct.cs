using System;
using System.Collections.Generic;

namespace BikeStore.Models;

public partial class FeaturedProduct
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public int Amount { get; set; }

    public int ProductId { get; set; }

    public virtual Product? Product { get; set; }
}
