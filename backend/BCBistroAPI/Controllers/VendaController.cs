using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]

public class VendaController : ControllerBase
{
    private readonly ConexaoDapper _connectionFactory;
    private readonly VendaService _vendaService; 

    public VendaController(ConexaoDapper connectionFactory, VendaService vendaService)
    {
        _connectionFactory = connectionFactory;
        _vendaService = vendaService;
    }

    [HttpPost("finalizar_venda")]
    public async Task<IActionResult> FinalizarVenda(Venda venda)
    {
        try
        {
            await _vendaService.FinalizarVenda(venda);
            return Ok(new
            {
                sucesso = true,
                mensagem = "Venda Finalizada"
            });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("buscar_vendas")]
    public async Task<IActionResult> BuscarVendas()
    {
        return Ok(await _vendaService.BuscarVendas()); 
    }
    

}

