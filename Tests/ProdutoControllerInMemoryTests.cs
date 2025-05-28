using FCG.Api.Controllers.v1;
using FCG.Api.Dto;
using FCG.Business.Interfaces;
using FCG.Business.Models;
using FCG.Business.Services;
using FCG.Business.Services.Interfaces;
using FCG.Data;
using FCG.Data.Repository;
using FCG.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FCG.Tests.Controllers
{
    public class ProdutoControllerInMemoryTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly IProdutoRepository _produtoRepository;
        private readonly IProdutoService _produtoService;
        private readonly ProdutoController _controller;
        private readonly Mock<ILogger<ProdutoController>> _loggerMock;

        public ProdutoControllerInMemoryTests()
        {
            // Configurar o contexto InMemory
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);

            // Configurar as dependências reais
            _produtoRepository = new ProdutoRepository(_context);
            _produtoService = new ProdutoService(_produtoRepository);
            _loggerMock = new Mock<ILogger<ProdutoController>>();
            _controller = new ProdutoController(_produtoService, _loggerMock.Object);

            // Garantir que o banco seja limpo antes de cada teste
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
        }

        [Fact]
        public async Task ObterTodos_ComProdutosNoBanco_RetornaOkComListaDeProdutos()
        {
            // Arrange
            var produtos = new List<Produto>
            {
                new Produto(Guid.NewGuid(), "Produto 1", "Descrição 1", "Categoria 1", 10m, DateTime.UtcNow),
                new Produto(Guid.NewGuid(), "Produto 2", "Descrição 2", "Categoria 2", 20m, DateTime.UtcNow)
            };

            // Adicionar produtos ao banco
            _context.Produto.AddRange(produtos);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetTodos();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnList = Assert.IsAssignableFrom<IEnumerable<ProdutoDto>>(okResult.Value);
            Assert.Equal(2, returnList.Count());

            // Verificar se os dados estão corretos
            var produtosList = returnList.ToList();
            Assert.Contains(produtosList, p => p.Nome == "Produto 1");
            Assert.Contains(produtosList, p => p.Nome == "Produto 2");
        }

        [Fact]
        public async Task ObterTodos_ComBancoVazio_RetornaOkComListaVazia()
        {
            // Act
            var result = await _controller.GetTodos();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnList = Assert.IsAssignableFrom<IEnumerable<ProdutoDto>>(okResult.Value);
            Assert.Empty(returnList);
        }

        [Fact]
        public async Task ObterPorId_ComProdutoExistente_RetornaOkComProduto()
        {
            // Arrange
            var id = Guid.NewGuid();
            var produto = new Produto(id, "Produto Teste", "Descrição Teste", "Categoria Teste", 50m, DateTime.UtcNow);

            _context.Produto.Add(produto);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetPorId(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnDto = Assert.IsType<ProdutoDto>(okResult.Value);
            Assert.Equal(id, returnDto.Id);
            Assert.Equal("Produto Teste", returnDto.Nome);
            Assert.Equal("Descrição Teste", returnDto.Descricao);
            Assert.Equal("Categoria Teste", returnDto.Categoria);
            Assert.Equal(50m, returnDto.Preco);
        }

        [Fact]
        public async Task ObterPorId_ComProdutoInexistente_RetornaNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();

            // Act
            var result = await _controller.GetPorId(id);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Adicionar_ComDadosValidos_SalvaNoBancoERetornaCreated()
        {
            // Arrange
            var produtoDto = new ProdutoDto
            {
                Id = Guid.NewGuid(),
                Nome = "Produto Teste",
                Descricao = "Descrição do Produto Teste",
                Categoria = "Categoria Teste",
                Preco = 100m,
                DataLancamento = DateTime.UtcNow
            };

            // Act
            var result = await _controller.Adicionar(produtoDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnDto = Assert.IsType<ProdutoDto>(createdResult.Value);

            // Verificar se retornou os dados corretos
            Assert.Equal(produtoDto.Nome, returnDto.Nome);
            Assert.Equal(produtoDto.Descricao, returnDto.Descricao);
            Assert.Equal(produtoDto.Categoria, returnDto.Categoria);
            Assert.Equal(produtoDto.Preco, returnDto.Preco);

            // Verificar se foi realmente salvo no banco
            var produtoNoBanco = await _context.Produto.FindAsync(returnDto.Id);
            Assert.NotNull(produtoNoBanco);
            Assert.Equal(produtoDto.Nome, produtoNoBanco.Nome);
            Assert.Equal(produtoDto.Descricao, produtoNoBanco.Descricao);
            Assert.Equal(produtoDto.Categoria, produtoNoBanco.Categoria);
            Assert.Equal(produtoDto.Preco, produtoNoBanco.Preco);

            // Verificar se existe apenas um produto no banco
            var totalProdutos = await _context.Produto.CountAsync();
            Assert.Equal(1, totalProdutos);
        }

        [Fact]
        public async Task Adicionar_ComPrecoNegativo_DeveLancarExcecao()
        {
            // Arrange
            var produtoDto = new ProdutoDto
            {
                Id = Guid.NewGuid(),
                Nome = "Produto Inválido",
                Descricao = "Descrição",
                Categoria = "Categoria",
                Preco = -10m, // Preço negativo
                DataLancamento = DateTime.UtcNow
            };

            // Act & Assert
            var result = await _controller.Adicionar(produtoDto);

            // Verificar se retornou erro 500
            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, objectResult.StatusCode);

            // Verificar se nenhum produto foi salvo no banco
            var totalProdutos = await _context.Produto.CountAsync();
            Assert.Equal(0, totalProdutos);
        }

        [Fact]
        public async Task Atualizar_ComDadosValidos_AtualizaNoBancoERetornaNoContent()
        {
            // Arrange
            var id = Guid.NewGuid();
            var produtoOriginal = new Produto(id, "Produto Original", "Descrição Original", "Categoria Original", 50m, DateTime.UtcNow);

            _context.Produto.Add(produtoOriginal);
            await _context.SaveChangesAsync();
            _context.Entry(produtoOriginal).State = EntityState.Detached;

            var produtoDto = new ProdutoDto
            {
                Id = id,
                Nome = "Produto Atualizado",
                Descricao = "Descrição Atualizada",
                Categoria = "Categoria Atualizada",
                Preco = 150m,
                DataLancamento = DateTime.UtcNow
            };

            // Act
            var result = await _controller.Atualizar(id, produtoDto);

            // Assert
            Assert.IsType<NoContentResult>(result);

            // Verificar se foi realmente atualizado no banco
            var produtoAtualizado = await _context.Produto.FindAsync(id);
            Assert.NotNull(produtoAtualizado);
            Assert.Equal("Produto Atualizado", produtoAtualizado.Nome);
            Assert.Equal("Descrição Atualizada", produtoAtualizado.Descricao);
            Assert.Equal("Categoria Atualizada", produtoAtualizado.Categoria);
            Assert.Equal(150m, produtoAtualizado.Preco);

            // Verificar se ainda existe apenas um produto
            var totalProdutos = await _context.Produto.CountAsync();
            Assert.Equal(1, totalProdutos);
        }

        [Fact]
        public async Task Atualizar_ComIdsDiferentes_RetornaBadRequest()
        {
            // Arrange
            var produtoDto = new ProdutoDto
            {
                Id = Guid.NewGuid(),
                Nome = "Produto Atualizado",
                Descricao = "Descrição Atualizada",
                Categoria = "Categoria Atualizada",
                Preco = 150m,
                DataLancamento = DateTime.UtcNow
            };

            var idDiferente = Guid.NewGuid();

            // Act
            var result = await _controller.Atualizar(idDiferente, produtoDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("IDs não coincidem.", badRequestResult.Value);
        }

        [Fact]
        public async Task Atualizar_ComProdutoInexistente_DeveLancarExcecao()
        {
            // Arrange
            var id = Guid.NewGuid();
            var produtoDto = new ProdutoDto
            {
                Id = id,
                Nome = "Produto Inexistente",
                Descricao = "Descrição",
                Categoria = "Categoria",
                Preco = 100m,
                DataLancamento = DateTime.UtcNow
            };

            // Act
            var result = await _controller.Atualizar(id, produtoDto);

            // Assert
            var badRequest = Assert.IsType<BadRequestResult>(result);
            Assert.Equal(400, badRequest.StatusCode);
        }

        [Fact]
        public async Task Remover_ComProdutoExistente_RemoveNoBancoERetornaNoContent()
        {
            // Arrange
            var id = Guid.NewGuid();
            var produto = new Produto(id, "Produto para Remover", "Descrição com tamanho correto", "Categoria", 100m, DateTime.UtcNow);

            _context.Produto.Add(produto);
            await _context.SaveChangesAsync();

            // Verificar que o produto existe antes da remoção
            var produtoAntes = await _context.Produto.FindAsync(id);
            Assert.NotNull(produtoAntes);

            // Act
            var result = await _controller.Remover(id);

            // Assert
            Assert.IsType<NoContentResult>(result);

            // Verificar se foi realmente removido do banco
            var produtoDepois = await _context.Produto.FirstOrDefaultAsync(p => p.Id == id && !p.Excluido);
            Assert.Null(produtoDepois);
        }

        [Fact]
        public async Task Remover_ComProdutoInexistente_DeveLancarExcecao()
        {
            // Arrange
            var id = Guid.NewGuid();

            // Act
            var result = await _controller.Remover(id);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
        }

        [Fact]
        public async Task Fluxo_Completo_CRUD_DeveExecutarCorretamente()
        {
            // 1. Criar produto
            var produtoDto = new ProdutoDto
            {
                Id = Guid.NewGuid(),
                Nome = "Produto Completo",
                Descricao = "Descrição Completa",
                Categoria = "Categoria Completa",
                Preco = 200m,
                DataLancamento = DateTime.UtcNow
            };

            var createResult = await _controller.Adicionar(produtoDto);
            var createdResult = Assert.IsType<CreatedAtActionResult>(createResult.Result);
            var produtoCriado = Assert.IsType<ProdutoDto>(createdResult.Value);

            // 2. Listar todos (deve ter 1)
            var listResult = await _controller.GetTodos();
            var okListResult = Assert.IsType<OkObjectResult>(listResult.Result);
            var produtos = Assert.IsAssignableFrom<IEnumerable<ProdutoDto>>(okListResult.Value);
            Assert.Single(produtos);

            // 3. Buscar por ID
            var getResult = await _controller.GetPorId(produtoCriado.Id);
            var okGetResult = Assert.IsType<OkObjectResult>(getResult.Result);
            var produtoEncontrado = Assert.IsType<ProdutoDto>(okGetResult.Value);
            Assert.Equal(produtoCriado.Id, produtoEncontrado.Id);

            // 4. Atualizar produto
            var entidadeProduto = await _context.Produto.FindAsync(produtoCriado.Id);
            _context.Entry(entidadeProduto!).State = EntityState.Detached;

            produtoEncontrado.Nome = "Produto Atualizado Completo";
            produtoEncontrado.Preco = 300m;
            var updateResult = await _controller.Atualizar(produtoEncontrado.Id, produtoEncontrado);
            Assert.IsType<NoContentResult>(updateResult);

            // 5. Verificar atualização
            var getUpdatedResult = await _controller.GetPorId(produtoCriado.Id);
            var okUpdatedResult = Assert.IsType<OkObjectResult>(getUpdatedResult.Result);
            var produtoAtualizado = Assert.IsType<ProdutoDto>(okUpdatedResult.Value);
            Assert.Equal("Produto Atualizado Completo", produtoAtualizado.Nome);
            Assert.Equal(300m, produtoAtualizado.Preco);

            // 6. Remover produto
            var deleteResult = await _controller.Remover(produtoCriado.Id);
            Assert.IsType<NoContentResult>(deleteResult);

            // 7. Verificar remoção
            var getDeletedResult = await _controller.GetPorId(produtoCriado.Id);
            Assert.IsType<NotFoundResult>(getDeletedResult.Result);

            // 8. Listar todos (deve estar vazio)
            var finalListResult = await _controller.GetTodos();
            var okFinalListResult = Assert.IsType<OkObjectResult>(finalListResult.Result);
            var produtosFinais = Assert.IsAssignableFrom<IEnumerable<ProdutoDto>>(okFinalListResult.Value);
            Assert.Empty(produtosFinais);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}