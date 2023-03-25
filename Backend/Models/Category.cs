using System;
using System.Collections.Generic;

namespace dionizos_backend_app.Models;

public partial class Category
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Eventincategory> Eventincategories { get; } = new List<Eventincategory>();
}
