using Api.Comandos.Productos;
using Api.Models;
using Api.Results.ResultadosProductos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]

public class ProductoController : ControllerBase
{
    private readonly ProductosContext context;

    public ProductoController(ProductosContext Context)
    {
        this.context = Context;
    }

    [HttpGet]
    [Route("consultarProductos")]
    
    public ActionResult<ResultadoListaProductos> consultarProductos()
    {
        var result = new ResultadoListaProductos();
        try
        {
            var products =  context.Productos.Where(p => p.fechaBaja == null).ToList();
            if (products != null)
            {
                foreach (var product in products)
                {
                    var rta = new producto()
                    {
                        Id = product.Id,
                        FechaDeCarga = product.FechaDeCarga?.ToString("dd/MM/yyyy"),
                        Precio = product.Precio,
                        Categoria = product.Categoria,
                    };
                    result.listaProductos.Add(rta);
                }
                result.setConfirm("Productos encontrados con exito");
                return result;
            }
            else
            {
                result.StatusCode = 404;
                result.setError("No se encontraron productos registrados vigentes");
                return result;
            }
        }
        catch (Exception ex)
        {
            result.setError(ex.Message);
            return BadRequest("Error al consultar los productos");
        }
    }

    [HttpPost]
    [Route("registraProducto")]

    public async Task<ActionResult<ResultadoPostProducto>> registrarProducto([FromForm] ProductoCmd producto)
    {
        var result = new ResultadoPostProducto();
        try
        {
            var product = new Producto
            {
                FechaDeCarga = DateTime.Now,
                Precio = producto.Precio,
                Categoria = producto.Categoria,
            };
            context.Productos.Add(product);
            await context.SaveChangesAsync();
            result.setConfirm("Producto registrado correctamente");
            result.IdProducto = product.Id;

            return Ok(result);
        }
        catch (Exception ex)
        {
            result.setError(ex.Message);
            return result;
        }
    }

    [HttpPut]
    [Route("modificarProducto")]
    public async Task<ActionResult<ResultadoPostProducto>> modificarProducto([FromForm] ProductoUpdateCmd product)
    {
        var result = new ResultadoPostProducto();
        try
        {
            var productBd = await context.Productos.Where(p => p.Id == product.Id && p.fechaBaja == null).FirstOrDefaultAsync();
            if (productBd != null)
            {
                productBd.Precio = product.Precio;
                productBd.Categoria = product.Categoria;

                context.Productos.Update(productBd);
                await context.SaveChangesAsync();

                result.IdProducto = productBd.Id;
                result.setConfirm("Producto modificado con exito");
                return Ok(result);
            }
            else
            {
                result.StatusCode = 400;
                result.setError("El producto que desea modificar no se encuentra");
                return result;
            }
        }
        catch (Exception ex)
        {
            result.setError(ex.Message);
            return result;
        }
    }

    [HttpDelete]
    [Route("eliminarProducto/{id}")]
    public async Task<ActionResult<ResultadoPostProducto>> eliminarProducto(int id)
    {
        var result = new ResultadoPostProducto();
        try
        {
            var productBd = await context.Productos.Where(p => p.Id == id).FirstOrDefaultAsync();
            if (productBd != null)
            {
                productBd.fechaBaja = DateTime.Now;

                context.Productos.Update(productBd);
                await context.SaveChangesAsync();

                result.IdProducto = productBd.Id;
                result.setConfirm("Producto eliminado con exito");
                return Ok(result);
            }
            else
            {
                result.StatusCode = 400;
                result.setError("No se encuentra el producto que desea eliminar");
                return result;
            }
        }
        catch (Exception ex)
        {
            result.setError(ex.Message);
            return result;
        }
    }
[HttpGet]
[Route("presupuesto")]
[Authorize]
public async Task<ActionResult<ResultadoPresupuesto>> presupuesto(int presupuesto)
{
    if (presupuesto < 1 || presupuesto > 1000000)
    {
        return BadRequest("El presupuesto debe estar entre 1 y 1.000.000.");
    }

    var productosPorCategoria = await context.Productos
     .Where(p => p.Precio <= presupuesto && p.fechaBaja == null)
    .GroupBy(p => p.Categoria ?? "Sin Categoría")
    .ToDictionaryAsync(g => g.Key, g => g.OrderByDescending(p => p.Precio).ToList());
    if (productosPorCategoria.Count == 0){
        return BadRequest("No hay productos disponibles para el presupuesto dado");
    }
    var combinaciones = new List<List<Producto>>();
    CrearCombinaciones(productosPorCategoria.Values.ToList(), 0, new List<Producto>(), combinaciones);

    var combinacionesValidas = combinaciones
        .Where(c => c.Sum(p => p.Precio) <= presupuesto)
        .ToList();

    if (combinacionesValidas.Count == 0)
    {
        return Ok("No hay combinación de productos que se ajuste al presupuesto.");
    }

    var mejorCombinacion = combinacionesValidas
        .OrderByDescending(c => c.Sum(p => p.Precio))
        .FirstOrDefault();
        if (mejorCombinacion.Count <= 1){
            return Ok("No hay combinación de productos que se ajuste al presupuesto.");
        }
    var resultadoPresupuesto = new ResultadoPresupuesto
    {
        FechaPresupuesto = DateTime.Now.ToString("dd/MM/yyyy"),
        TotalPresupuesto = mejorCombinacion.Sum(p => p.Precio ?? 0),
        listaProductos = mejorCombinacion.Select(p => new producto
        {
            Id = p.Id,
            Precio = p.Precio,
            FechaDeCarga = p.FechaDeCarga?.ToString("dd/MM/yyyy"),
            Categoria = p.Categoria
        }).ToList(),
        
    };
    resultadoPresupuesto.setConfirm("Presupuesto dado con exito");

    return Ok(resultadoPresupuesto);
}

private void CrearCombinaciones(List<List<Producto>> listas, int nivel, List<Producto> combinacionActual, List<List<Producto>> combinaciones)
{
    if (nivel == listas.Count)
    {
        combinaciones.Add(new List<Producto>(combinacionActual));
        return;
    }

    foreach (var producto in listas[nivel])
    {
        combinacionActual.Add(producto);
        CrearCombinaciones(listas, nivel + 1, combinacionActual, combinaciones);
        combinacionActual.RemoveAt(combinacionActual.Count - 1);
    }
}

}
