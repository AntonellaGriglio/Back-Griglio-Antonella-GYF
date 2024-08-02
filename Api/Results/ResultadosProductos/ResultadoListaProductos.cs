using Api.Models;

namespace Api.Results.ResultadosProductos;

public class ResultadoListaProductos: ResultBase
{
    public List<producto> listaProductos { get; set; } = new List<producto>();
}
public class producto {
    public int Id { get; set; }
    public int? Precio { get; set; }
    public string? FechaDeCarga { get; set; }
    public string? Categoria { get; set; }


}
