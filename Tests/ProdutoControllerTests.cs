using FCG.Api.Controllers.v1;
using FCG.Api.Dto;
using FCG.Business.Models;
using FCG.Business.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace FCG.Tests.Controllers;

public class ProdutoControllerTests
{
    private readonly ProdutoController _controller;
    private readonly Mock<IProdutoService> _produtoServiceMock;
    private readonly Mock<ILogger<ProdutoController>> _loggerMock;

    public ProdutoControllerTests()
    {
        _produtoServiceMock = new Mock<IProdutoService>();
        _loggerMock = new Mock<ILogger<ProdutoController>>();
        _controller = new ProdutoController(_produtoServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task ObterTodosRetornaOkComListaDeProdutos()
    {
        var produtos = new List<Produto>
        {
            new Produto(Guid.NewGuid(), "Produto 1", "Descrição 1", "Categoria 1", 10, DateTime.UtcNow),
            new Produto(Guid.NewGuid(), "Produto 2", "Descrição 2", "Categoria 2", 20, DateTime.UtcNow)
        };

        _produtoServiceMock.Setup(s => s.ListarTodos()).ReturnsAsync(produtos);

        var result = await _controller.GetTodos();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnList = Assert.IsAssignableFrom<IEnumerable<ProdutoDto>>(okResult.Value);
        Assert.Equal(2, returnList.Count());
    }

    [Fact]
    public async Task ObterPorIdRetornaOkComProdutoQuandoEncontrado()
    {
        var id = Guid.NewGuid();
        var produto = new Produto(id, "Produto Teste", "Descrição Teste", "Categoria Teste", 50, DateTime.UtcNow);

        _produtoServiceMock.Setup(s => s.ObterPorId(id)).ReturnsAsync(produto);

        var result = await _controller.GetPorId(id);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnDto = Assert.IsType<ProdutoDto>(okResult.Value);
        Assert.Equal(id, returnDto.Id);
    }

    [Fact]
    public async Task ObterPorIdRetornaNotFoundQuandoNaoEncontrado()
    {
        var id = Guid.NewGuid();

        _produtoServiceMock.Setup(s => s.ObterPorId(id)).ReturnsAsync((Produto)null);

        var result = await _controller.GetPorId(id);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task AdicionarRetornaCreatedAtActionComProdutoCriado()
    {
        var produtoDto = new ProdutoDto
        {
            Id = Guid.NewGuid(),
            Nome = "Produto Teste",
            Descricao = "Descrição do Produto Teste",
            Categoria = "Categoria Teste",
            Preco = 100m,
            DataLancamento = DateTime.UtcNow
        };

        var produtoCriado = new Produto(
            Guid.NewGuid(),
            produtoDto.Nome,
            produtoDto.Descricao,
            produtoDto.Categoria,
            produtoDto.Preco,
            produtoDto.DataLancamento
        );

        _produtoServiceMock.Setup(s => s.Adicionar(It.IsAny<Produto>())).ReturnsAsync(produtoCriado);

        var result = await _controller.Adicionar(produtoDto);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnDto = Assert.IsType<ProdutoDto>(createdResult.Value);
        Assert.Equal(produtoCriado.Id, returnDto.Id);
    }

    [Fact]
    public async Task AdicionarRetornaStatus500QuandoErro()
    {
        var produtoDto = new ProdutoDto
        {
            Id = Guid.Empty,
            Nome = "Produto Teste",
            Descricao = "Descrição do Produto Teste",
            Categoria = "Categoria Teste",
            Preco = 100m,
            DataLancamento = DateTime.UtcNow
        };

        _produtoServiceMock.Setup(s => s.Adicionar(It.IsAny<Produto>())).ThrowsAsync(new Exception("Erro"));

        var result = await _controller.Adicionar(produtoDto);

        var objectResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, objectResult.StatusCode);
    }

    [Fact]
    public async Task AtualizarRetornaNoContentQuandoSucesso()
    {
        var produtoDto = new ProdutoDto
        {
            Id = Guid.NewGuid(),
            Nome = "Produto Atualizado",
            Descricao = "Descrição Atualizada",
            Categoria = "Categoria Atualizada",
            Preco = 150m,
            DataLancamento = DateTime.UtcNow
        };

        _produtoServiceMock.Setup(s => s.Atualizar(It.IsAny<Produto>())).Returns(Task.CompletedTask);

        var result = await _controller.Atualizar(produtoDto.Id, produtoDto);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task AtualizarRetornaBadRequestQuandoIdDiferente()
    {
        var produtoDto = new ProdutoDto
        {
            Id = Guid.NewGuid(),
            Nome = "Produto Atualizado",
            Descricao = "Descrição Atualizada",
            Categoria = "Categoria Atualizada",
            Preco = 150m,
            DataLancamento = DateTime.UtcNow
        };

        var result = await _controller.Atualizar(Guid.NewGuid(), produtoDto);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("IDs não coincidem.", badRequestResult.Value);
    }

    [Fact]
    public async Task AtualizarRetornaStatus500QuandoErro()
    {
        var produtoDto = new ProdutoDto
        {
            Id = Guid.NewGuid(),
            Nome = "Produto Atualizado",
            Descricao = "Descrição Atualizada",
            Categoria = "Categoria Atualizada",
            Preco = 150m,
            DataLancamento = DateTime.UtcNow
        };

        _produtoServiceMock.Setup(s => s.Atualizar(It.IsAny<Produto>())).ThrowsAsync(new Exception("Erro"));

        var result = await _controller.Atualizar(produtoDto.Id, produtoDto);

        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, objectResult.StatusCode);
    }

    [Fact]
    public async Task RemoverRetornaNoContentQuandoSucesso()
    {
        var id = Guid.NewGuid();

        _produtoServiceMock.Setup(s => s.Remover(id)).Returns(Task.CompletedTask);

        var result = await _controller.Remover(id);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task RemoverRetornaStatus500QuandoErro()
    {
        var id = Guid.NewGuid();

        _produtoServiceMock.Setup(s => s.Remover(id)).ThrowsAsync(new Exception("Erro"));

        var result = await _controller.Remover(id);

        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, objectResult.StatusCode);
    }






}
