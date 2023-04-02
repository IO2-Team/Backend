using System;
using System.Collections.Generic;

namespace dionizos_backend_app.Models;

public partial class Eventincategory
{
    public long Id { get; set; }

    public long EventId { get; set; }

    public long CategoriesId { get; set; }

    public virtual Category Categories { get; set; } = null!;

    public virtual Event Event { get; set; } = null!;
}
