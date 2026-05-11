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
    public async Task<IActionResult> BuscarVendas([FromQuery] DateTime dataInicial, [FromQuery] DateTime dataFinal)
    {
        return Ok(await _vendaService.BuscarVendas(dataInicial, dataFinal));
    }

    [HttpGet("download-excel-vendas")]
    public async Task<IActionResult> DownloadExcelVendas([FromQuery] DateTime dataInicial, [FromQuery] DateTime dataFinal)
    {
        try
        {
            var arquivo = await _vendaService
                .DownloadExcelVendas(dataInicial, dataFinal);

            return File(
                arquivo,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"relatorio-vendas-{dataInicial:dd-MM-yyyy} - {dataFinal:dd-MM-yyyy}.xlsx"
            );
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                sucesso = false,
                mensagem = ex.Message
            });
        }
    }
    

}

