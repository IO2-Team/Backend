using System;
using System.Collections.Generic;

namespace dionizos_backend_app.Models;

public partial class Emailcode
{
    public long Id { get; set; }

    public long OrganizerId { get; set; }

    public string Code { get; set; } = null!;

    public DateTime Time { get; set; }

    public virtual Organizer Organizer { get; set; } = null!;
}
