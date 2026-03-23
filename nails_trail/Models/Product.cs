using System;
using System.Collections.Generic;

namespace nails_trail.Models;

public partial class Product
{
    public int IdProduct { get; set; }

    public string ProductName { get; set; } = null!;

    public int IdCategory { get; set; }

    public int Stock { get; set; }

    public decimal BuyedAt { get; set; }

    public decimal SellsAt { get; set; }

    public virtual Category IdCategoryNavigation { get; set; } = null!;
}
