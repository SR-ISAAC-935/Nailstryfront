using System;
using System.Collections.Generic;

namespace nails_trail.Models;

public partial class DateAgendedResume
{
    public int IdDate { get; set; }

    public string Idissuer { get; set; } = null!;

    public DateTime Datereserved { get; set; }

    public decimal? TotalPrice { get; set; }

    public int? TotalDuration { get; set; }

    public string? Status { get; set; }

    public string? Notes { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<DateServiceDetail> DateServiceDetails { get; set; } = new List<DateServiceDetail>();

    public virtual AspNetUser IdissuerNavigation { get; set; } = null!;
}
