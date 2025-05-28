using FCG.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCG.Tests;

public class ProdutoTests
{
    #region Testes do Construtor - Casos de Sucesso

    [Fact]
    public void Construtor_ComDadosValidos_DeveCriarProdutoCorretamente()
    {
        // Arrange
        var id = Guid.NewGuid();
        var nome = "Produto Teste";
        var descricao = "Esta é uma descrição válida do produto";
        var categoria = "Categoria";
        var preco = 100.50m;
        var dataLancamento = DateTime.UtcNow.AddDays(30);

        // Act
        var produto = new Produto(id, nome, descricao, categoria, preco, dataLancamento);

        // Assert
        Assert.Equal(id, produto.Id);
        Assert.Equal(nome, produto.Nome);
        Assert.Equal(descricao, produto.Descricao);
        Assert.Equal(categoria, produto.Categoria);
        Assert.Equal(preco, produto.Preco);
        Assert.Equal(dataLancamento, produto.DataLancamento);
        Assert.False(produto.Excluido);
        Assert.Null(produto.DataAtualizacao);
        Assert.True(produto.DataCriacao <= DateTime.UtcNow);
    }

    #endregion

    #region Testes do Construtor - Validação de ID

    [Fact]
    public void Construtor_ComIdVazio_DeveLancarArgumentException()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Produto(Guid.Empty, "Produto Teste", "Descrição válida do produto", "Categoria", 100m, DateTime.UtcNow));

        Assert.Equal("Id não pode ser vazio", exception.Message);
    }

    #endregion

    #region Testes do Construtor - Validação de Nome

    [Fact]
    public void Construtor_ComNomeNulo_DeveLancarArgumentException()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Produto(Guid.NewGuid(), null, "Descrição válida do produto", "Categoria", 100m, DateTime.UtcNow));

        Assert.Equal("Nome não pode ser nulo ou vazio", exception.Message);
    }

    [Fact]
    public void Construtor_ComNomeVazio_DeveLancarArgumentException()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Produto(Guid.NewGuid(), "", "Descrição válida do produto", "Categoria", 100m, DateTime.UtcNow));

        Assert.Equal("Nome não pode ser nulo ou vazio", exception.Message);
    }

    [Fact]
    public void Construtor_ComNomeComMenos3Caracteres_DeveLancarArgumentException()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Produto(Guid.NewGuid(), "Ab", "Descrição válida do produto", "Categoria", 100m, DateTime.UtcNow));

        Assert.Equal("Nome deve ter mais de 3 caracteres", exception.Message);
    }

    [Fact]
    public void Construtor_ComNomeCom201Caracteres_DeveLancarArgumentException()
    {
        // Arrange
        var nomeGrande = new string('A', 201);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Produto(Guid.NewGuid(), nomeGrande, "Descrição válida do produto", "Categoria", 100m, DateTime.UtcNow));

        Assert.Equal("Nome deve ter no máximo 200 caracteres", exception.Message);
    }

    [Fact]
    public void Construtor_ComNomeCom200Caracteres_DeveAceitar()
    {
        // Arrange
        var nome = new string('A', 200);

        // Act
        var produto = new Produto(Guid.NewGuid(), nome, "Descrição válida do produto", "Categoria", 100m, DateTime.UtcNow);

        // Assert
        Assert.Equal(nome, produto.Nome);
    }

    #endregion

    #region Testes do Construtor - Validação de Descrição

    [Fact]
    public void Construtor_ComDescricaoNula_DeveLancarArgumentException()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Produto(Guid.NewGuid(), "Produto", null, "Categoria", 100m, DateTime.UtcNow));

        Assert.Equal("Descrição não pode ser nula ou vazia", exception.Message);
    }

    [Fact]
    public void Construtor_ComDescricaoComMenos10Caracteres_DeveLancarArgumentException()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Produto(Guid.NewGuid(), "Produto", "Desc", "Categoria", 100m, DateTime.UtcNow));

        Assert.Equal("Descrição deve ter mais de 10 caracteres", exception.Message);
    }

    [Fact]
    public void Construtor_ComDescricaoCom1001Caracteres_DeveLancarArgumentException()
    {
        // Arrange
        var descricaoGrande = new string('A', 1001);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Produto(Guid.NewGuid(), "Produto", descricaoGrande, "Categoria", 100m, DateTime.UtcNow));

        Assert.Equal("Descrição deve ter no máximo 1000 caracteres", exception.Message);
    }

    #endregion

    #region Testes do Construtor - Validação de Categoria

    [Fact]
    public void Construtor_ComCategoriaNula_DeveLancarArgumentException()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Produto(Guid.NewGuid(), "Produto", "Descrição válida", null, 100m, DateTime.UtcNow));

        Assert.Equal("Categoria não pode ser nula ou vazia", exception.Message);
    }

    [Fact]
    public void Construtor_ComCategoriaComMenos3Caracteres_DeveLancarArgumentException()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Produto(Guid.NewGuid(), "Produto", "Descrição válida", "Ab", 100m, DateTime.UtcNow));

        Assert.Equal("Categoria deve ter mais de 3 caracteres", exception.Message);
    }

    [Fact]
    public void Construtor_ComCategoriaCom51Caracteres_DeveLancarArgumentException()
    {
        // Arrange
        var categoriaGrande = new string('A', 51);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Produto(Guid.NewGuid(), "Produto", "Descrição válida", categoriaGrande, 100m, DateTime.UtcNow));

        Assert.Equal("Categoria deve ter no máximo 50 caracteres", exception.Message);
    }

    #endregion

    #region Testes do Construtor - Validação de Preço

    [Fact]
    public void Construtor_ComPrecoZero_DeveLancarArgumentException()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Produto(Guid.NewGuid(), "Produto", "Descrição válida", "Categoria", 0m, DateTime.UtcNow));

        Assert.Equal("Preço deve ser maior que zero", exception.Message);
    }

    [Fact]
    public void Construtor_ComPrecoNegativo_DeveLancarArgumentException()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Produto(Guid.NewGuid(), "Produto", "Descrição válida", "Categoria", -10m, DateTime.UtcNow));

        Assert.Equal("Preço deve ser maior que zero", exception.Message);
    }

    [Fact]
    public void Construtor_ComPrecoMuitoAlto_DeveLancarArgumentException()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Produto(Guid.NewGuid(), "Produto", "Descrição válida", "Categoria", 1000000m, DateTime.UtcNow));

        Assert.Equal("Preço não pode exceder R$ 999.999,99", exception.Message);
    }

    #endregion

    #region Testes do Construtor - Validação de Data

    [Fact]
    public void Construtor_ComDataMuitoAntiga_DeveLancarArgumentException()
    {
        // Arrange
        var dataAntiga = DateTime.MinValue;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Produto(Guid.NewGuid(), "Produto", "Descrição válida", "Categoria", 100m, dataAntiga));

        Assert.Contains("Data lançamento inválida (muito antiga)", exception.Message);
    }

    [Fact]
    public void Construtor_ComDataMuitoDistante_DeveLancarArgumentException()
    {
        // Arrange
        var dataAntiga = DateTime.MaxValue;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Produto(Guid.NewGuid(), "Produto", "Descrição válida", "Categoria", 100m, dataAntiga));

        Assert.Contains("Data lançamento inválida (muito distante)", exception.Message);
    }
    #endregion

    #region Testes do Método Atualizar

    [Fact]
    public void Atualizar_ComDadosValidos_DeveAtualizarProduto()
    {
        // Arrange
        var produto = new Produto(Guid.NewGuid(), "Produto", "Descrição original", "Categoria", 100m, DateTime.UtcNow);
        var novoNome = "Produto Atualizado";
        var novaDescricao = "Nova descrição do produto";
        var novaCategoria = "Nova Categoria";
        var novoPreco = 150m;
        var novaDataLancamento = DateTime.UtcNow.AddDays(60);

        // Act
        produto.Atualizar(novoNome, novaDescricao, novaCategoria, novoPreco, novaDataLancamento);

        // Assert
        Assert.Equal(novoNome, produto.Nome);
        Assert.Equal(novaDescricao, produto.Descricao);
        Assert.Equal(novaCategoria, produto.Categoria);
        Assert.Equal(novoPreco, produto.Preco);
        Assert.Equal(novaDataLancamento, produto.DataLancamento);
        Assert.NotNull(produto.DataAtualizacao);
        Assert.True(produto.DataAtualizacao <= DateTime.UtcNow);
    }

    [Fact]
    public void Atualizar_ComNomeInvalido_DeveLancarArgumentException()
    {
        // Arrange
        var produto = new Produto(Guid.NewGuid(), "Produto", "Descrição original", "Categoria", 100m, DateTime.UtcNow);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            produto.Atualizar("AB", "Nova descrição", "Categoria", 100m, DateTime.UtcNow));

        Assert.Equal("Nome deve ter mais de 3 caracteres", exception.Message);
    }

    #endregion

    #region Testes dos Métodos Excluir e Reativar

    [Fact]
    public void Excluir_DeveMarcarProdutoComoExcluido()
    {
        // Arrange
        var produto = new Produto(Guid.NewGuid(), "Produto", "Descrição com 10 caracteres", "Categoria", 100m, DateTime.UtcNow);

        // Act
        produto.Excluir();

        // Assert
        Assert.True(produto.Excluido);
    }

    [Fact]
    public void Reativar_DeveMarcarProdutoComoNaoExcluido()
    {
        // Arrange
        var produto = new Produto(Guid.NewGuid(), "Produto", "Descrição com 10 caracteres", "Categoria", 100m, DateTime.UtcNow);
        produto.Excluir();

        // Act
        produto.Reativar();

        // Assert
        Assert.False(produto.Excluido);
        Assert.NotNull(produto.DataAtualizacao);
    }

    #endregion

    #region Testes dos Métodos Auxiliares

    [Fact]
    public void EstaAtivo_ComProdutoNaoExcluido_DeveRetornarTrue()
    {
        // Arrange
        var produto = new Produto(Guid.NewGuid(), "Produto", "Descrição com 10 caracteres", "Categoria", 100m, DateTime.UtcNow);

        // Act & Assert
        Assert.True(produto.EstaAtivo());
    }

    [Fact]
    public void EstaAtivo_ComProdutoExcluido_DeveRetornarFalse()
    {
        // Arrange
        var produto = new Produto(Guid.NewGuid(), "Produto", "Descrição com 10 caracteres", "Categoria", 100m, DateTime.UtcNow);
        produto.Excluir();

        // Act & Assert
        Assert.False(produto.EstaAtivo());
    }

    [Fact]
    public void FoiLancado_ComDataLancamentoPassada_DeveRetornarTrue()
    {
        // Arrange
        var produto = new Produto(Guid.NewGuid(), "Produto", "Descrição com 10 caracteres", "Categoria", 100m, DateTime.UtcNow.AddDays(-1));

        // Act & Assert
        Assert.True(produto.FoiLancado());
    }

    [Fact]
    public void FoiLancado_ComDataLancamentoFutura_DeveRetornarFalse()
    {
        // Arrange
        var produto = new Produto(Guid.NewGuid(), "Produto", "Descrição com 10 caracteres", "Categoria", 100m, DateTime.UtcNow.AddDays(1));

        // Act & Assert
        Assert.False(produto.FoiLancado());
    }

    [Fact]
    public void DiasDesdeUltimaAtualizacao_SemAtualizacao_DeveRetornarDiasDesdeCreacao()
    {
        // Arrange
        var produto = new Produto(Guid.NewGuid(), "Produto", "Descrição com 10 caracteres", "Categoria", 100m, DateTime.UtcNow);

        // Act
        var dias = produto.DiasDesdeUltimaAtualizacao();

        // Assert
        Assert.True(dias >= 0);
    }

    [Fact]
    public void DiasDesdeUltimaAtualizacao_ComAtualizacao_DeveRetornarDiasDesdeAtualizacao()
    {
        // Arrange
        var produto = new Produto(Guid.NewGuid(), "Produto", "Descrição com 10 caracteres", "Categoria", 100m, DateTime.UtcNow);
        produto.Atualizar("Novo Produto", "Nova descrição", "Nova categoria", 200m, DateTime.UtcNow);

        // Act
        var dias = produto.DiasDesdeUltimaAtualizacao();

        // Assert
        Assert.True(dias >= 0);
    }

    #endregion

    #region Teste do ToString

    [Fact]
    public void ToString_DeveRetornarFormatoCorreto()
    {
        // Arrange
        var produto = new Produto(Guid.NewGuid(), "Produto Teste", "Descrição com 10 caracteres", "Eletrônicos", 299.99m, DateTime.UtcNow);

        // Act
        var resultado = produto.ToString();

        // Assert
        Assert.Equal("Produto: Produto Teste - Eletrônicos - R$ 299,99", resultado);
    }

    #endregion
}