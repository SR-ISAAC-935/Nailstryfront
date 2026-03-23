using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace nails_trail.Models;

[Table("Services")]
public partial class Service
{
    public int IdService { get; set; }

    public string ServiceName { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int Duration { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<DateServiceDetail> DateServiceDetails { get; set; } = new List<DateServiceDetail>();
}
