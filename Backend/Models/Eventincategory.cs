using System;
using System.Collections.Generic;

namespace dionizos_backend_app.Models;

public partial class Eventincategory
{
    public int Id { get; set; }

    public int EventId { get; set; }

    public int CategoriesId { get; set; }

    public virtual Category Categories { get; set; } = null!;

    public virtual Event Event { get; set; } = null!;
}
