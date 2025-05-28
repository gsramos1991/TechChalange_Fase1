using Asp.Versioning;
using FCG.Api.Dto;
using FCG.Api.Extensions;
using FCG.Business.Models;
using FCG.Business.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FCG.Api.Controllers.v1;


[Authorize]
[ApiController]
[ApiVersion(1.0, Deprecated = true)]
[Route("api/v{version:apiVersion}/[controller]")]
public class ProdutoController : ControllerBase
{
    private readonly IProdutoService _produtoService;
    private readonly ILogger<ProdutoController> _logger;

    public ProdutoController(IProdutoService produtoService, ILogger<ProdutoController> logger)
    {
        _produtoService = produtoService;
        _logger = logger;
    }


    [HttpGet("ObterTodos")]
    public async Task<ActionResult<IEnumerable<ProdutoDto>>> GetTodos()
    {
        try
        {
            var produtos = await _produtoService.ListarTodos();
            return Ok(produtos.Select(p => p.ToDto()));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar todos os produtos.");
            return StatusCode(500, "Erro interno no servidor.");
        }
    }


    [HttpGet("ObterPorId/{id:guid}")]
    public async Task<ActionResult<ProdutoDto>> GetPorId(Guid id)
    {
        try
        {
            var produto = await _produtoService.ObterPorId(id);

            if (produto == null)
            {
                _logger.LogWarning("Produto com ID {Id} não encontrado.", id);
                return NotFound();
            }

            return Ok(produto.ToDto());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter produto por ID {Id}.", id);
            return StatusCode(500, "Erro interno no servidor.");
        }
    }



    [HttpPost("Adicionar")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<ProdutoDto>> Adicionar(ProdutoDto produtoDto)
    {
        try
        {

            var produto = produtoDto.ToDomain();
            var produtoCriado = await _produtoService.Adicionar(produto);
            return CreatedAtAction(nameof(GetPorId), new { id = produtoCriado.Id }, produtoCriado.ToDto());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao adicionar novo produto.");
            return StatusCode(500, "Erro interno ao adicionar produto.");
        }
    }


    [HttpPut("Alterar{id:guid}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Atualizar(Guid id, ProdutoDto produtoDto)
    {
        if (id != produtoDto.Id)
            return BadRequest("IDs não coincidem.");

        try
        {
            Produto? produto = await _produtoService.ObterPorId(id);
            if (produto == null)
                return BadRequest();

            produto.Atualizar(
                                novoNome: produtoDto.Nome,
                                novaDescricao: produtoDto.Descricao,
                                novaCategoria: produtoDto.Categoria,
                                novoPreco: produtoDto.Preco,
                                dataLancamento: produtoDto.DataLancamento
            );

            await _produtoService.Atualizar(produto);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar produto com ID {Id}.", id);
            return StatusCode(500, "Erro interno ao atualizar produto.");
        }
    }


    [HttpDelete("Remover{id:guid}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Remover(Guid id)
    {
        try
        {
            await _produtoService.Remover(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao remover produto com ID {Id}.", id);
            return StatusCode(500, "Erro interno ao remover produto.");
        }
    }
}