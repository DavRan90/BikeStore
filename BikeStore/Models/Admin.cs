using System;
using System.Collections.Generic;

namespace BikeStore.Models;

public partial class Admin : User
{
    //public int Id { get; set; }

    //public string? Name { get; set; }

    public int? AuthorityLevel { get; set; }

    //public string? Email { get; set; }

    //public string? Password { get; set; }
}
