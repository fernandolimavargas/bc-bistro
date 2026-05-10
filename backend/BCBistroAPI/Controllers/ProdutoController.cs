using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]

public class ProdutoController : ControllerBase
{
    
    private readonly ProdutoService _produtoService;

    public ProdutoController(ProdutoService produtoService)
    {
        _produtoService = produtoService;
    }

    [HttpGet("buscar_produtos")]
    public IActionResult BuscarProdutos()
    {
        try
        {
            return Ok(_produtoService.BuscarProdutos());
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("adicionar_produto")]
    public async Task<IActionResult> AdicionarProduto(Produtos produto)
    {
        try
        {
            await _produtoService.AdicionarProduto(produto);
            return Ok(new
            {
                sucesso = true,
                mensagem = "Produto adicionado com sucesso"
            }); 
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("alterar_produto")]
    public IActionResult AlterarProduto(Produtos produtos)
    {
        try
        {
            _produtoService.AlterarProduto(produtos);
            return Ok("Produto Alterado"); 
        } catch (Exception ex)
        {
            return BadRequest(ex.Message); 
        }
    } 
}