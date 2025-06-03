using Asp.Versioning;
using FCG.Api.Dto;
using FCG.Api.MappingDtos;
using FCG.Business.Models;
using FCG.Business.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FCG.Api.Controllers.v1;

/// <summary>
/// Controller para gerenciamento de jogos
/// </summary>
[Authorize]
[ApiController]
[ApiVersion(1.0, Deprecated = true)]
[Route("api/v{version:apiVersion}/[controller]")]
public class JogoController : ControllerBase
{
    private readonly IJogoService _produtoService;
    private readonly ILogger<JogoController> _logger;

    public JogoController(IJogoService produtoService, ILogger<JogoController> logger)
    {
        _produtoService = produtoService;
        _logger = logger;
    }

    /// <summary>
    /// Lista todos os jogos cadastrados no sistema
    /// </summary>
    /// <remarks>
    /// Retorna uma lista com todos os jogos ativos cadastrados no sistema FCG.
    /// 
    /// **⚠️ ATENÇÃO:** Este endpoint está obsoleto e será removido na versão 2.0
    /// 
    /// **Processo de listagem:**
    /// 1. 🔐 **Autenticação:** Verifica se o usuário está autenticado
    /// 2. 📋 **Consulta:** Busca todos os jogos ativos no banco de dados
    /// 3. 🔄 **Transformação:** Converte os dados para DTO
    /// 4. ✅ **Retorno:** Envia a lista de jogos
    /// 
    /// **Filtros aplicados:**
    /// - Somente jogos ativos (não removidos)
    /// 
    /// **Exemplo de resposta de sucesso:**
    /// ```json
    /// [
    ///   {
    ///     "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///     "nome": "The Last of Us Part II",
    ///     "descricao": "Jogo de ação e aventura pós-apocalíptico",
    ///     "categoria": "Ação/Aventura",
    ///     "preco": 199.90,
    ///     "dataLancamento": "2020-06-19"
    ///   },
    ///   {
    ///     "id": "7cb85f64-5717-4562-b3fc-2c963f66afa8",
    ///     "nome": "God of War Ragnarök",
    ///     "descricao": "Continuação da saga nórdica de Kratos",
    ///     "categoria": "Ação/RPG",
    ///     "preco": 299.90,
    ///     "dataLancamento": "2022-11-09"
    ///   }
    /// ]
    /// ```
    /// 
    /// **Exemplo de resposta vazia:**
    /// ```json
    /// []
    /// ```
    /// </remarks>
    /// <returns>Lista de jogos cadastrados</returns>
    /// <response code="200">Lista de jogos retornada com sucesso</response>
    /// <response code="401">Usuário não autenticado - Token JWT inválido ou ausente</response>
    /// <response code="500">Erro interno do servidor</response>    
    [HttpGet("ObterTodos")]
    [Obsolete("Este endpoint está obsoleto. Use a versão mais atualizada da API.")]
    [ProducesResponseType(typeof(IEnumerable<JogoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<JogoDto>>> GetTodos()
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

    /// <summary>
    /// Obtém os detalhes de um jogo específico
    /// </summary>
    /// <remarks>
    /// Busca e retorna as informações detalhadas de um jogo através do seu identificador único.
    /// 
    /// **⚠️ ATENÇÃO:** Este endpoint está obsoleto e será removido na versão 2.0
    /// 
    /// **Processo de busca:**
    /// 1. 🔐 **Autenticação:** Verifica se o usuário está autenticado
    /// 2. 🔍 **Validação:** Valida o formato do GUID fornecido
    /// 3. 📋 **Consulta:** Busca o jogo pelo ID no banco de dados
    /// 4. 🔄 **Verificação:** Confirma se o jogo existe e está ativo
    /// 5. ✅ **Retorno:** Envia os dados do jogo ou erro 404
    /// 
    /// **Regras de negócio:**
    /// - O ID deve ser um GUID válido
    /// - Somente retorna jogos ativos (não deletados)
    /// - Retorna 404 se o jogo não existir ou estiver inativo
    /// 
    /// **Exemplo de requisição:**
    /// ```
    /// GET /api/v1/jogo/ObterPorId/3fa85f64-5717-4562-b3fc-2c963f66afa6
    /// ```
    /// 
    /// **Exemplo de resposta de sucesso:**
    /// ```json
    /// {
    ///   "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///   "nome": "The Last of Us Part II",
    ///   "descricao": "Jogo de ação e aventura pós-apocalíptico",
    ///   "categoria": "Ação/Aventura",
    ///   "preco": 199.90,
    ///   "dataLancamento": "2020-06-19"
    /// }
    /// ```
    /// 
    /// **Exemplo de resposta de erro (não encontrado):**
    /// ```json
    /// {
    ///   "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
    ///   "title": "Not Found",
    ///   "status": 404
    /// }
    /// ```
    /// </remarks>
    /// <param name="id">Identificador único do jogo (GUID)</param>
    /// <returns>Dados detalhados do jogo</returns>
    /// <response code="200">Jogo encontrado e retornado com sucesso</response>
    /// <response code="401">Usuário não autenticado - Token JWT inválido ou ausente</response>
    /// <response code="404">Jogo não encontrado ou inativo</response>
    /// <response code="500">Erro interno do servidor</response>    
    [HttpGet("ObterPorId/{id:guid}")]
    [Obsolete("Este endpoint está obsoleto. Use a versão mais atualizada da API.")]
    [ProducesResponseType(typeof(JogoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<JogoDto>> GetPorId(Guid id)
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

    /// <summary>
    /// Adiciona um novo jogo ao catálogo
    /// </summary>
    /// <remarks>
    /// Cria um novo registro de jogo no sistema FCG com as informações fornecidas.
    /// 
    /// **⚠️ ATENÇÃO:** Este endpoint está obsoleto e será removido na versão 2.0
    /// 
    /// **Processo de criação:**
    /// 1. 🔐 **Autenticação:** Verifica se o usuário está autenticado
    /// 2. 🛡️ **Autorização:** Confirma se o usuário tem role de Administrador
    /// 3. 🔍 **Validação:** Valida todos os campos obrigatórios
    /// 4. 💾 **Criação:** Salva o novo jogo no banco de dados
    /// 5. 🆔 **Geração:** Cria um novo ID único para o jogo
    /// 6. ✅ **Retorno:** Envia o jogo criado com status 201
    /// 
    /// **Requisitos de segurança:**
    /// - Usuário deve estar autenticado (token JWT válido)
    /// - Usuário deve ter role "Administrador"
    /// - Todos os campos obrigatórios devem ser preenchidos
    /// 
    /// **Exemplo de payload:**
    /// ```json
    /// {
    ///   "nome": "Horizon Forbidden West",
    ///   "descricao": "Sequência do aclamado Horizon Zero Dawn",
    ///   "categoria": "Ação/RPG",
    ///   "preco": 249.90,
    ///   "dataLancamento": "2022-02-18"
    /// }
    /// ```
    /// 
    /// **Exemplo de resposta de sucesso:**
    /// ```json
    /// {
    ///   "id": "8fa85f64-5717-4562-b3fc-2c963f66afa9",
    ///   "nome": "Horizon Forbidden West",
    ///   "descricao": "Sequência do aclamado Horizon Zero Dawn",
    ///   "categoria": "Ação/RPG",
    ///   "preco": 249.90,
    ///   "dataLancamento": "2022-02-18"
    /// }
    /// ```
    /// 
    /// **Exemplo de resposta de erro (validação):**
    /// ```json
    /// {
    ///   "errors": {
    ///     "Nome": ["O campo Nome é obrigatório."],
    ///     "Preco": ["O campo Preço deve ser maior que zero."]
    ///   }
    /// }
    /// ```
    /// </remarks>
    /// <param name="produtoDto">Dados do jogo a ser criado</param>
    /// <returns>Jogo criado com ID gerado</returns>
    /// <response code="201">Jogo criado com sucesso - Retorna o jogo criado</response>
    /// <response code="400">Dados inválidos - Erro de validação</response>
    /// <response code="401">Usuário não autenticado - Token JWT inválido ou ausente</response>
    /// <response code="403">Acesso negado - Usuário não possui permissão de Administrador</response>
    /// <response code="500">Erro interno do servidor</response>    
    [HttpPost("Adicionar")]
    [Obsolete("Este endpoint está obsoleto. Use a versão mais atualizada da API.")]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(typeof(JogoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<JogoDto>> Adicionar(JogoDto produtoDto)
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

    /// <summary>
    /// Atualiza as informações de um jogo existente
    /// </summary>
    /// <remarks>
    /// Permite a atualização completa dos dados de um jogo já cadastrado no sistema.
    /// 
    /// **⚠️ ATENÇÃO:** Este endpoint está obsoleto e será removido na versão 2.0
    /// 
    /// **Processo de atualização:**
    /// 1. 🔐 **Autenticação:** Verifica se o usuário está autenticado
    /// 2. 🛡️ **Autorização:** Confirma se o usuário tem role de Administrador
    /// 3. 🔍 **Validação:** Valida os dados e verifica se IDs coincidem
    /// 4. 📋 **Busca:** Localiza o jogo no banco de dados
    /// 5. 🔄 **Atualização:** Aplica as alterações no registro
    /// 6. 💾 **Persistência:** Salva as mudanças no banco
    /// 7. ✅ **Confirmação:** Retorna status 204 (No Content)
    /// 
    /// **Requisitos de segurança:**
    /// - Usuário deve estar autenticado (token JWT válido)
    /// - Usuário deve ter role "Administrador"
    /// - ID na URL deve corresponder ao ID no corpo da requisição
    /// 
    /// 
    /// **⚠️ Importante:** Esta operação substitui todos os dados do jogo
    /// 
    /// **Exemplo de requisição:**
    /// ```
    /// PUT /api/v1/jogo/Alterar/3fa85f64-5717-4562-b3fc-2c963f66afa6
    /// ```
    /// 
    /// **Exemplo de payload:**
    /// ```json
    /// {
    ///   "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///   "nome": "The Last of Us Part II - Director's Cut",
    ///   "descricao": "Versão definitiva com conteúdo adicional",
    ///   "categoria": "Ação/Aventura",
    ///   "preco": 249.90,
    ///   "dataLancamento": "2020-06-19"
    /// }
    /// ```
    /// 
    /// **Exemplo de resposta de erro (IDs não coincidem):**
    /// ```json
    /// {
    ///   "message": "IDs não coincidem."
    /// }
    /// ```
    /// 
    /// **Exemplo de resposta de erro (não encontrado):**
    /// ```json
    /// {
    ///   "message": "Jogo não encontrado."
    /// }
    /// ```
    /// </remarks>
    /// <param name="id">ID único do jogo a ser atualizado</param>
    /// <param name="produtoDto">Novos dados completos do jogo</param>
    /// <returns>Confirmação da atualização sem conteúdo</returns>
    /// <response code="204">Jogo atualizado com sucesso - Sem conteúdo no retorno</response>
    /// <response code="400">Dados inválidos ou IDs não coincidem</response>
    /// <response code="401">Usuário não autenticado - Token JWT inválido ou ausente</response>
    /// <response code="403">Acesso negado - Usuário não possui permissão de Administrador</response>
    /// <response code="404">Jogo não encontrado</response>
    /// <response code="500">Erro interno do servidor</response>    
    [HttpPut("Alterar/{id:guid}")]
    [Obsolete("Este endpoint está obsoleto. Use a versão mais atualizada da API.")]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Atualizar(Guid id, JogoDto produtoDto)
    {
        if (id != produtoDto.Id)
            return BadRequest("IDs não coincidem.");

        try
        {
            Jogo? produto = await _produtoService.ObterPorId(id);
            if (produto == null)
                return NotFound("Jogo não encontrado.");

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

    /// <summary>
    /// Desativa um jogo do catálogo
    /// </summary>
    /// <remarks>
    /// Realiza a desativação lógica (soft delete) de um jogo, mantendo o registro no banco para auditoria.
    /// 
    /// **⚠️ ATENÇÃO:** Este endpoint está obsoleto e será removido na versão 2.0
    /// 
    /// **Processo de desativação:**
    /// 1. 🔐 **Autenticação:** Verifica se o usuário está autenticado
    /// 2. 🛡️ **Autorização:** Confirma se o usuário tem role de Administrador
    /// 3. 🔍 **Validação:** Valida o formato do GUID fornecido
    /// 4. 📋 **Busca:** Localiza o jogo no banco de dados
    /// 5. 🚫 **Desativação:** Marca o jogo como inativo
    /// 6. 💾 **Persistência:** Salva a alteração no banco
    /// 7. ✅ **Confirmação:** Retorna status 204 (No Content)
    /// 
    /// **Requisitos de segurança:**
    /// - Usuário deve estar autenticado (token JWT válido)
    /// - Usuário deve ter role "Administrador"
    /// 
    /// **Comportamento da desativação:**
    /// - O jogo não é fisicamente removido do banco
    /// - O registro é marcado como "Excluido = true"
    /// - O jogo não aparece mais em listagens
    /// - O jogo não pode mais ser consultado por ID
    /// - Dados são mantidos para auditoria e histórico
    /// 
    /// **📝 Nota importante:** 
    /// Esta operação é definitiva. Para reativar um jogo desativado, 
    /// é necessário intervenção direta no banco de dados.
    /// 
    /// **Exemplo de requisição:**
    /// ```
    /// DELETE /api/v1/jogo/Remover/3fa85f64-5717-4562-b3fc-2c963f66afa6
    /// ```
    /// 
    /// **Resposta de sucesso:**
    /// ```
    /// HTTP 204 No Content
    /// (Sem corpo na resposta)
    /// ```
    /// 
    /// **Exemplo de resposta de erro (não encontrado):**
    /// ```json
    /// {
    ///   "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
    ///   "title": "Not Found",
    ///   "status": 404
    /// }
    /// ```
    /// </remarks>
    /// <param name="id">ID único do jogo a ser desativado</param>
    /// <returns>Confirmação da desativação sem conteúdo</returns>
    /// <response code="204">Jogo desativado com sucesso - Sem conteúdo no retorno</response>
    /// <response code="401">Usuário não autenticado - Token JWT inválido ou ausente</response>
    /// <response code="403">Acesso negado - Usuário não possui permissão de Administrador</response>
    /// <response code="404">Jogo não encontrado ou já desativado</response>
    /// <response code="500">Erro interno do servidor</response>    
    [HttpDelete("Remover/{id:guid}")]
    [Obsolete("Este endpoint está obsoleto. Use a versão mais atualizada da API.")]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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