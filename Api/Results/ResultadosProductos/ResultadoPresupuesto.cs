namespace Api.Results.ResultadosProductos;

public class ResultadoPresupuesto:ResultadoListaProductos
{
    public string FechaPresupuesto { get; set; }
    public int TotalPresupuesto { get; set; }
}

