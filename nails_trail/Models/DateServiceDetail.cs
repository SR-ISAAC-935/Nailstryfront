using System;
using System.Collections.Generic;

namespace nails_trail.Models;

public partial class DateServiceDetail
{
    public int IdDetail { get; set; }

    public int IdDate { get; set; }

    public int IdService { get; set; }

    public decimal ServicePrice { get; set; }

    public int ServiceDuration { get; set; }

    public int? Quantity { get; set; }

    public string? Notes { get; set; }

    public virtual DateAgendedResume IdDateNavigation { get; set; } = null!;

    public virtual Service IdServiceNavigation { get; set; } = null!;
}
