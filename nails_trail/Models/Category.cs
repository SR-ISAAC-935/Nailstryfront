using System;
using System.Collections.Generic;

namespace nails_trail.Models;

public partial class Category
{
    public int IdCategory { get; set; }

    public string CategoryName { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
