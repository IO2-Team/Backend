﻿using System;
using System.Collections.Generic;

namespace dionizos_backend_app.Models;

public partial class Event
{
    public long Id { get; set; }

    public long Owner { get; set; }

    public string Title { get; set; } = null!;

    public string? Name { get; set; }

    public DateTime Starttime { get; set; }

    public DateTime Endtime { get; set; }

    public string Latitude { get; set; } = null!;

    public string Longitude { get; set; } = null!;

    public int Status { get; set; }

    public int Placecapacity { get; set; }

    public string? Placeschema { get; set; }

    public virtual ICollection<Eventincategory> Eventincategories { get; } = new List<Eventincategory>();

    public virtual Organizer OwnerNavigation { get; set; } = null!;

    public virtual ICollection<Path> Paths { get; } = new List<Path>();

    public virtual ICollection<Reservaton> Reservatons { get; } = new List<Reservaton>();
}
