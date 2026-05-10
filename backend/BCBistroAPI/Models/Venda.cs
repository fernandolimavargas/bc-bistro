public class Venda
{
    public DateTime HoraVenda { get; set; }
    public decimal Total { get; set; }
    public List<Comanda> Produtos { get; set; }
}