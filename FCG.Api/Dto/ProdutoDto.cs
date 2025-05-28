namespace FCG.Api.Dto;
public class ProdutoDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
    public string Descricao { get; set; }
    public string Categoria { get; set; }
    public decimal Preco { get; set; }
    public DateTime DataLancamento { get; set; }
}

