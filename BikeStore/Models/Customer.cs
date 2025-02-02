using System;
using System.Collections.Generic;

namespace BikeStore.Models;

public partial class Customer : User
{
    //public int Id { get; set; }

    public string? PersonNummer { get; set; }

    //public string? Name { get; set; }

    public string? City { get; set; }

    public string? Street { get; set; }

    public string? ZipCode { get; set; }

    public string? PhoneNumber { get; set; }

    //public string? Email { get; set; }

    //public string? Password { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
