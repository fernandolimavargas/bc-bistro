public class CatalogoService
{
    private readonly CatalogoRepository _catalogoRepository;

    public CatalogoService(CatalogoRepository catalogoRepository)
    {
        _catalogoRepository = catalogoRepository;
    }
    
    public List<Catalogo> MostrarCatalogo(int filtro)
    {
        return _catalogoRepository.MostrarCatalogo(filtro); 
    }
}