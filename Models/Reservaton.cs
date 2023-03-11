using System;
using System.Collections.Generic;

namespace dionizos_backend_app.Models;

public partial class Reservaton
{
    public int EventId { get; set; }

    public int PlaceId { get; set; }

    public int Token { get; set; }

    public virtual Event Event { get; set; } = null!;
}
