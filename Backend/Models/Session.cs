using System;
using System.Collections.Generic;

namespace dionizos_backend_app.Models;

public partial class Session
{
    public long Id { get; set; }

    public long OrganizerId { get; set; }

    public string Token { get; set; } = null!;

    public DateTime Time { get; set; }

    public virtual Organizer Organizer { get; set; } = null!;
}
