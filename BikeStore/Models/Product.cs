using System;
using System.Collections.Generic;

namespace BikeStore.Models;

public partial class Product
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public decimal? Price { get; set; }

    public int? Stock { get; set; }

    public int? Sold { get; set; }

    public int? ReorderLevel { get; set; }

    public int? CategoryId { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<FeaturedProduct> FeaturedProducts { get; set; } = new List<FeaturedProduct>();
}
