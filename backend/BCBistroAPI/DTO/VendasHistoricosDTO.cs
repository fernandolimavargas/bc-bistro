using System.Numerics;

public class VendasHistoricosDTO
{
    public int Id { get; set; }
    public DateTime HoraVenda { get; set; }
    public decimal TotalVenda { get; set; }
    public decimal TotalDoDia { get; set; }
    public string Produto { get; set; }
    public string Categoria { get; set; }
    public decimal ValorUnidade { get; set; }
    public int Quantidade { get; set; }
    public decimal ValorCalculado { get; set; }
}

