public class VendasHistoricos
{
    public int Id { get; set; }
    public DateTime DataVenda { get; set; }
    public decimal ValorVendidoGeral { get; set; }
    public decimal ValorVendidoHoje { get; set; }
    public int VendasTotaisHoje { get; set; }
}
