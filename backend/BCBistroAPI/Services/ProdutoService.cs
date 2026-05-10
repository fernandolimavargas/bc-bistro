using System.Threading.Tasks;

public class ProdutoService
{
    private readonly ProdutoRepository _produtoRepository;

    public ProdutoService(ProdutoRepository produtoRepository)
    {
        _produtoRepository = produtoRepository;
    }

    public List<Produtos> BuscarProdutos()
    {
        return _produtoRepository.BuscarProdutos();
    }

    public async Task AdicionarProduto(Produtos produto)
    {
        await _produtoRepository.AdicionarProduto(produto);
    }
    
    public async Task AlterarProduto(Produtos produto)
    {
        await _produtoRepository.AlterarProduto(produto); 
    }
}