using System.Globalization;

namespace FCG.Business.Models;

public class Produto
{
    #region "Propriedades"
    public Guid Id { get; private set; }
    public string Nome { get; private set; }
    public string Descricao { get; private set; }
    public string Categoria { get; private set; }
    public decimal Preco { get; private set; }
    public DateTime DataLancamento { get; private set; }
    public DateTime DataCriacao { get; private set; }
    public DateTime? DataAtualizacao { get; private set; }
    public bool Excluido { get; private set; }
    #endregion

    #region "Constructors"
    protected Produto() { }

    public Produto(Guid id, string nome, string descricao, string categoria, decimal preco, DateTime dataLancamento)
    {
        ValidarEntidade(id, nome, descricao, categoria, preco, dataLancamento);

        Id = id;
        Nome = nome;
        Descricao = descricao;
        Categoria = categoria;
        Preco = preco;
        DataLancamento = dataLancamento;
        DataCriacao = DateTime.UtcNow;
        DataAtualizacao = null;
        Excluido = false; // Produto criado não deve estar excluído por padrão
    }
    #endregion

    #region "Métodos de domínio"
    public void Atualizar(string novoNome, string novaDescricao, string novaCategoria, decimal novoPreco, DateTime dataLancamento)
    {
        ValidarEntidade(Id, novoNome, novaDescricao, novaCategoria, novoPreco, dataLancamento);

        Nome = novoNome;
        Descricao = novaDescricao;
        Categoria = novaCategoria;
        Preco = novoPreco;
        DataLancamento = dataLancamento;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void Excluir()
    {
        Excluido = true;
    }

    public void Reativar()
    {
        Excluido = false;
        DataAtualizacao = DateTime.UtcNow;
    }
    #endregion

    #region "Métodos de validação"
    private static void ValidarEntidade(Guid id, string nome, string descricao, string categoria, decimal preco, DateTime dataLancamento)
    {
        ValidarId(id);
        ValidarNome(nome);
        ValidarDescricao(descricao);
        ValidarCategoria(categoria);
        ValidarPreco(preco);
        ValidarDataLancamento(dataLancamento);
    }
            
    private static void ValidarId(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Id não pode ser vazio");
    }

    private static void ValidarNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException( "Nome não pode ser nulo ou vazio");

        nome = nome.Trim();

        if (nome.Length < 3)
            throw new ArgumentException("Nome deve ter mais de 3 caracteres");

        if (nome.Length > 200)
            throw new ArgumentException("Nome deve ter no máximo 200 caracteres");
    }

    private static void ValidarDescricao(string descricao)
    {
        if (string.IsNullOrWhiteSpace(descricao))
            throw new ArgumentException("Descrição não pode ser nula ou vazia");

        descricao = descricao.Trim();

        if (descricao.Length < 10)
            throw new ArgumentException("Descrição deve ter mais de 10 caracteres");

        if (descricao.Length > 1000)
            throw new ArgumentException("Descrição deve ter no máximo 1000 caracteres");
    }

    private static void ValidarCategoria(string categoria)
    {
        if (string.IsNullOrWhiteSpace(categoria))
            throw new ArgumentException("Categoria não pode ser nula ou vazia");

        categoria = categoria.Trim();

        if (categoria.Length < 3)
            throw new ArgumentException("Categoria deve ter mais de 3 caracteres");

        if (categoria.Length > 50)
            throw new ArgumentException("Categoria deve ter no máximo 50 caracteres");
    }

    private static void ValidarPreco(decimal preco)
    {
        if (preco <= 0)
            throw new ArgumentException("Preço deve ser maior que zero");

        // Validação adicional para evitar preços muito altos (opcional)
        if (preco > 999999.99m)
            throw new ArgumentException("Preço não pode exceder R$ 999.999,99");
    }

    private static void ValidarDataLancamento(DateTime dataLancamento)
    {
        if (dataLancamento == DateTime.MinValue)
            throw new ArgumentException("Data lançamento inválida (muito antiga)");

        if (dataLancamento == DateTime.MaxValue)
            throw new ArgumentException("Data lançamento inválida (muito distante)");
    }
    #endregion

    #region "Métodos de consulta"
    public bool EstaAtivo() => !Excluido;

    public bool FoiLancado() => DataLancamento <= DateTime.UtcNow;

    public int DiasDesdeUltimaAtualizacao()
    {
        if (!DataAtualizacao.HasValue)
            return (DateTime.UtcNow - DataCriacao).Days;

        return (DateTime.UtcNow - DataAtualizacao.Value).Days;
    }
    #endregion

    public override string ToString()
    {
        return $"Produto: {Nome} - {Categoria} - R$ {Preco.ToString("F2", new CultureInfo("pt-BR"))}";
    }
}