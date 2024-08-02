using System;
using System.Collections.Generic;

namespace Api.Models;

public partial class Producto
{
    public int Id { get; set; }

    public int? Precio { get; set; }

    public DateTime? FechaDeCarga { get; set; }

    public string? Categoria { get; set; }
    public DateTime? fechaBaja { get; set; }

}
