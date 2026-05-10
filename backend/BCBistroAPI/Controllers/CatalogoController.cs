using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]

public class CatalogoController: ControllerBase 
{

    private readonly CatalogoService _catalogoService;

    public CatalogoController (CatalogoService catalogoService)
    {
        _catalogoService = catalogoService;
    }

    [HttpGet("mostrar_catalogo/{filtro:int}")]
    public IActionResult MostrarCatalogo([FromRoute] int filtro = 0)
    {
        try
        {
            return Ok(_catalogoService.MostrarCatalogo(filtro));
        } catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}