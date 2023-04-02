using System;
using System.Collections.Generic;

namespace dionizos_backend_app.Models;

public partial class Organizer
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int Status { get; set; }

    public virtual ICollection<Emailcode> Emailcodes { get; } = new List<Emailcode>();

    public virtual ICollection<Event> Events { get; } = new List<Event>();

    public virtual ICollection<Session> Sessions { get; } = new List<Session>();
}
