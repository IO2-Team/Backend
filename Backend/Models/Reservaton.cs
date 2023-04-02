using System;
using System.Collections.Generic;

namespace dionizos_backend_app.Models;

public partial class Reservaton
{
    public long EventId { get; set; }

    public long PlaceId { get; set; }

    public string Token { get; set; } = null!;

    public virtual Event Event { get; set; } = null!;
}
