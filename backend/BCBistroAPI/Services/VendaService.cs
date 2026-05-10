public class VendaService
{
    private readonly VendaRepository _vendaRepository;

    public VendaService(VendaRepository vendaRepository)
    {
        _vendaRepository = vendaRepository;
    }

    public async Task FinalizarVenda(Venda venda)
    {
        await _vendaRepository.FinalizarVenda(venda);
    }
    
    public async Task<List<VendasHistoricos>> BuscarVendas()
    {
        return await _vendaRepository.BuscarVendas(); 
    }
}