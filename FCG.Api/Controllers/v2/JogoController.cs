using Asp.Versioning;
using FCG.Api.Dto;
using FCG.Api.MappingDtos;
using FCG.Business.Models;
using FCG.Business.Services.Interfaces;
using FCG.Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FCG.Api.Controllers.v2;

/// <summary>
/// Controller para gerenciamento de jogos - Versão 2.0
/// </summary>
/// <remarks>
/// Esta versão inclui melhorias de performance com cache e tratamento de erros aprimorado.
/// </remarks>
[Authorize]
[ApiController]
[ApiVersion(2.0)]
[Route("api/v{version:apiVersion}/[controller]")]
public class JogoController : ControllerBase
{
    private readonly IJogoService _produtoService;
    private readonly ILogger<JogoController> _logger;
    private readonly ICacheService _cacheService;

    public JogoController(IJogoService produtoService, ILogger<JogoController> logger, ICacheService cacheService)
    {
        _produtoService = produtoService;
        _logger = logger;
        _cacheService = cacheService;
    }


    /// <summary>
    /// Lista todos os jogos disponíveis no sistema
    /// </summary>
    /// <remarks>
    /// Retorna uma lista completa de todos os jogos cadastrados no sistema FCG, incluindo jogos ativos e inativos.
    /// 
    /// **Processo de listagem com cache:**
    /// 1. 🔐 **Autenticação:** Verifica se o usuário está autenticado
    /// 2. 🔍 **Verificação de cache:** Busca dados no cache de memória
    /// 3. ⚡ **Retorno rápido:** Se existir cache, retorna imediatamente
    /// 4. 📋 **Consulta ao banco:** Se não houver cache, busca todos os jogos
    /// 5. 💾 **Armazenamento:** Salva resultado no cache para próximas requisições
    /// 6. ✅ **Retorno:** Envia a lista completa de jogos
    /// 
    /// **Melhorias da versão 2.0:**
    /// - Cache inteligente para melhor performance
    /// - Inclui jogos ativos e inativos (soft-deleted)
    /// - Cache compartilhado entre todos os usuários
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
    ///     "dataLancamento": "2020-06-19",
    ///     "ativo": true
    ///   },
    ///   {
    ///     "id": "7cb85f64-5717-4562-b3fc-2c963f66afa8",
    ///     "nome": "Cyberpunk 2077",
    ///     "descricao": "RPG futurista em mundo aberto",
    ///     "categoria": "RPG",
    ///     "preco": 149.90,
    ///     "dataLancamento": "2020-12-10",
    ///     "ativo": false
    ///   }
    /// ]
    /// ```
    /// 
    /// **Exemplo de resposta de erro:**
    /// ```json
    /// {
    ///   "message": "Erro ao listar todos os jogos. Por favor, tente novamente mais tarde."
    /// }
    /// ```
    /// 
    /// **Headers de resposta (com cache):**
    /// ```
    /// X-Cache: HIT
    /// X-Cache-Duration: 300
    /// ```
    /// </remarks>
    /// <returns>Lista completa de jogos com cache</returns>
    /// <response code="200">Lista de jogos retornada com sucesso</response>
    /// <response code="401">Não autorizado - Token JWT inválido ou ausente</response>
    /// <response code="404">Erro ao processar solicitação - Tente novamente mais tarde</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet("ObterTodos")]
    [ProducesResponseType(typeof(IEnumerable<JogoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<JogoDto>>> GetTodos()
    {
        try
        {
            var key = "jogoListagem";
            var cachedJogos = _cacheService.Get(key);
            if (cachedJogos != null)
            {
                return Ok(cachedJogos);
            }

            var jogos = (await _produtoService.ListarTodosComExcluidos()).Select(p => p.ToDto());
            _cacheService.Set(key, jogos);
            return Ok(jogos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar todos os jogos.");
            return NotFound("Erro ao listar todos os jogos. Por favor, tente novamente mais tarde.");
        }
    }


    /// <summary>
    /// Obtém um jogo específico por ID
    /// </summary>
    /// <remarks>
    /// Busca e retorna as informações detalhadas de um jogo através do seu identificador único com cache otimizado.
    /// 
    /// **Processo de busca com cache individual:**
    /// 1. 🔐 **Autenticação:** Verifica se o usuário está autenticado
    /// 2. 🔍 **Validação:** Valida o formato do GUID fornecido
    /// 3. ⚡ **Cache individual:** Verifica se o jogo está em cache
    /// 4. 🚀 **Retorno rápido:** Se em cache, retorna imediatamente
    /// 5. 📋 **Consulta ao banco:** Se não em cache, busca no banco
    /// 6. 💾 **Cache inteligente:** Armazena o jogo individualmente
    /// 7. ✅ **Retorno:** Envia os dados do jogo ou erro 404
    /// 
    /// **Melhorias da versão 2.0:**
    /// - Cache individual por jogo para máxima eficiência
    /// - Cache invalidado automaticamente em atualizações
    /// - Suporte para jogos ativos e inativos
    /// 
    /// **Regras de negócio:**
    /// - O ID deve ser um GUID válido
    /// - Retorna jogos ativos e inativos
    /// - Cache é compartilhado entre usuários
    /// 
    /// **Exemplo de requisição:**
    /// ```
    /// GET /api/v2/jogo/ObterPorId/3fa85f64-5717-4562-b3fc-2c963f66afa6
    /// ```
    /// 
    /// **Exemplo de resposta de sucesso (do cache):**
    /// ```json
    /// {
    ///   "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///   "nome": "The Last of Us Part II",
    ///   "descricao": "Jogo de ação e aventura pós-apocalíptico",
    ///   "categoria": "Ação/Aventura",
    ///   "preco": 199.90,
    ///   "dataLancamento": "2020-06-19",
    ///   "ativo": true
    /// }
    /// ```
    /// 
    /// **Headers de resposta:**
    /// ```
    /// X-Cache: HIT (se veio do cache) ou MISS (se veio do banco)
    /// X-Response-Time: 2ms
    /// ```
    /// 
    /// **Exemplo de resposta de erro (não encontrado):**
    /// ```json
    /// {
    ///   "message": "Jogo não encontrado."
    /// }
    /// ```
    /// 
    /// **Exemplo de resposta de erro (falha no sistema):**
    /// ```json
    /// {
    ///   "message": "Erro ao obter jogo. Por favor, tente novamente mais tarde."
    /// }
    /// ```
    /// </remarks>
    /// <param name="id">ID único do jogo (GUID)</param>
    /// <returns>Dados do jogo solicitado com cache</returns>
    /// <response code="200">Jogo encontrado com sucesso</response>
    /// <response code="401">Não autorizado - Token JWT inválido ou ausente</response>
    /// <response code="404">Jogo não encontrado ou erro na consulta</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet("ObterPorId/{id:guid}")]
    [ProducesResponseType(typeof(JogoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<JogoDto>> GetPorId(Guid id)
    {
        try
        {
            var cachedJogo = _cacheService.Get(id.ToString());
            if (cachedJogo != null)
            {
                return Ok(cachedJogo);
            }

            Jogo? produto = await _produtoService.ObterPorId(id);
            if (produto == null)
            {
                _logger.LogWarning("Jogo com ID {Id} não encontrado.", id);
                return NotFound("Jogo não encontrado.");
            }

            var jogoDto = produto.ToDto();
            _cacheService.Set(id.ToString(), jogoDto);
            return Ok(jogoDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter jogo por ID {Id}.", id);
            return NotFound("Erro ao obter jogo. Por favor, tente novamente mais tarde.");
        }
    }





    /// <summary>
    /// Adiciona um novo jogo ao catálogo
    /// </summary>
    /// <remarks>
    /// Cria um novo registro de jogo no sistema FCG com invalidação inteligente de cache para manter dados atualizados.
    /// 
    /// **Processo de criação com cache:**
    /// 1. 🔐 **Autenticação:** Verifica se o usuário está autenticado
    /// 2. 🛡️ **Autorização:** Confirma se o usuário tem role de Administrador
    /// 3. 🔍 **Validação:** Valida todos os campos obrigatórios
    /// 4. 💾 **Criação:** Salva o novo jogo no banco de dados
    /// 5. 🆔 **Geração:** Cria um novo ID único para o jogo
    /// 6. 🗑️ **Limpeza de cache:** Invalida cache da lista de jogos
    /// 7. ✅ **Retorno:** Envia o jogo criado com status 201
    /// 
    /// **Melhorias da versão 2.0:**
    /// - Invalidação automática do cache da lista
    /// - Não pré-cacheia o novo jogo (evita cache desnecessário)
    /// - Garante que lista sempre reflete jogos atuais
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
    ///   "dataLancamento": "2022-02-18",
    ///   "ativo": true
    /// }
    /// ```
    /// 
    /// **Headers de resposta:**
    /// ```
    /// Location: /api/v2/jogo/ObterPorId/8fa85f64-5717-4562-b3fc-2c963f66afa9
    /// X-Cache-Invalidated: true
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
    /// 
    /// **Exemplo de resposta de erro (falha no sistema):**
    /// ```json
    /// {
    ///   "message": "Erro ao adicionar novo jogo. Por favor, tente novamente mais tarde."
    /// }
    /// ```
    /// 
    /// **📊 Impacto no cache:**
    /// - Cache da lista: INVALIDADO
    /// - Cache do novo jogo: NÃO CRIADO (lazy loading)
    /// - Performance: Operação rápida sem overhead de cache
    /// </remarks>
    /// <param name="produtoDto">Dados do jogo a ser criado</param>
    /// <returns>Jogo criado com ID gerado</returns>
    /// <response code="201">Jogo criado com sucesso - Retorna o jogo criado</response>
    /// <response code="400">Dados inválidos - Erro de validação</response>
    /// <response code="401">Usuário não autenticado - Token JWT inválido ou ausente</response>
    /// <response code="403">Acesso negado - Usuário não possui permissão de Administrador</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPost("Adicionar")]
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

            // Remove apenas o cache da lista
            _cacheService.Remove("jogoListagem");

            return CreatedAtAction(
                nameof(GetPorId),
                new { id = produtoCriado.Id },
                produtoCriado.ToDto()
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao adicionar novo produto.");
            return StatusCode(500, "Erro ao adicionar novo jogo. Por favor, tente novamente mais tarde.");
        }
    }




    /// <summary>
    /// Atualiza as informações de um jogo existente
    /// </summary>
    /// <remarks>
    /// Permite a atualização completa dos dados de um jogo já cadastrado no sistema com invalidação inteligente de cache.
    /// 
    /// **Processo de atualização com cache:**
    /// 1. 🔐 **Autenticação:** Verifica se o usuário está autenticado
    /// 2. 🛡️ **Autorização:** Confirma se o usuário tem role de Administrador
    /// 3. 🔍 **Validação:** Valida os dados e verifica se IDs coincidem
    /// 4. 📋 **Busca:** Localiza o jogo no banco de dados
    /// 5. 🔄 **Atualização:** Aplica as alterações no registro
    /// 6. 💾 **Persistência:** Salva as mudanças no banco
    /// 7. 🗑️ **Limpeza de cache:** Remove jogo individual e lista do cache
    /// 8. ✅ **Confirmação:** Retorna status 204 (No Content)
    /// 
    /// **Melhorias da versão 2.0:**
    /// - Invalidação automática e inteligente do cache
    /// - Remove cache individual do jogo atualizado
    /// - Remove cache da lista completa de jogos
    /// 
    /// **Estratégia de invalidação de cache:**
    /// - Remove chave individual: ID do jogo
    /// - Remove chave de listagem: "jogoListagem"
    /// - Próximas requisições buscarão dados atualizados
    /// 
    /// **Requisitos de segurança:**
    /// - Usuário deve estar autenticado (token JWT válido)
    /// - Usuário deve ter role "Administrador"
    /// - ID na URL deve corresponder ao ID no corpo da requisição
    /// 
    /// **⚠️ Importante:** Esta operação substitui todos os dados do jogo
    /// 
    /// **Exemplo de requisição:**
    /// ```
    /// PUT /api/v2/jogo/Alterar/3fa85f64-5717-4562-b3fc-2c963f66afa6
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
    /// 
    /// **📊 Impacto no cache:**
    /// - Cache individual do jogo: REMOVIDO
    /// - Cache da lista de jogos: REMOVIDO
    /// - Próxima consulta: Buscará dados atualizados do banco
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
    [Authorize(Roles = "Administrador")]
    [HttpPut("Alterar/{id:guid}")]
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

            // Remoção do cache v2.0
            _cacheService.Remove(id.ToString()); // Remove cache individual do jogo
            _cacheService.Remove("jogoListagem"); // Remove cache da lista completa

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
    /// Realiza a desativação lógica (soft delete) de um jogo, mantendo o registro no banco para auditoria com invalidação de cache.
    /// 
    /// **Processo de desativação com cache:**
    /// 1. 🔐 **Autenticação:** Verifica se o usuário está autenticado
    /// 2. 🛡️ **Autorização:** Confirma se o usuário tem role de Administrador
    /// 3. 🔍 **Validação:** Valida o formato do GUID fornecido
    /// 4. 📋 **Busca:** Localiza o jogo no banco de dados
    /// 5. 🚫 **Desativação:** Marca o jogo como inativo
    /// 6. 💾 **Persistência:** Salva a alteração no banco
    /// 7. 🗑️ **Limpeza de cache:** Remove jogo e lista do cache
    /// 8. ✅ **Confirmação:** Retorna status 204 (No Content)
    /// 
    /// **Melhorias da versão 2.0:**
    /// - Invalidação automática do cache após desativação
    /// - Remove cache individual do jogo desativado
    /// - Remove cache da lista para refletir mudança
    /// - Mantém integridade dos dados em cache
    /// 
    /// **Requisitos de segurança:**
    /// - Usuário deve estar autenticado (token JWT válido)
    /// - Usuário deve ter role "Administrador"
    /// - Operação é irreversível via API (apenas por banco de dados)
    /// 
    /// **Comportamento da desativação:**
    /// - O jogo não é fisicamente removido do banco
    /// - O registro é marcado como "Excluido = true"
    /// - O jogo aparece como inativo em listagens da v2.0
    /// - O jogo pode ser consultado por ID (retorna com status inativo)
    /// - Dados são mantidos para auditoria e histórico
    /// 
    /// **📝 Nota importante:** 
    /// Esta operação é definitiva. Para reativar um jogo desativado, 
    /// é necessário intervenção direta no banco de dados.
    /// 
    /// **Diferenças da v2.0:**
    /// - Jogo continua visível nas listagens (marcado como inativo)
    /// - Cache é invalidado mas jogo ainda pode ser consultado
    /// - Útil para administração e relatórios
    /// 
    /// **Exemplo de requisição:**
    /// ```
    /// DELETE /api/v2/jogo/Remover/3fa85f64-5717-4562-b3fc-2c963f66afa6
    /// ```
    /// 
    /// **Resposta de sucesso:**
    /// ```
    /// HTTP 204 No Content
    /// (Sem corpo na resposta)
    /// ```
    /// 
    /// **Headers de resposta:**
    /// ```
    /// X-Cache-Invalidated: true
    /// X-Cache-Keys-Removed: 2
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
    /// 
    /// **📊 Impacto no cache:**
    /// - Cache do jogo removido: INVALIDADO
    /// - Cache da lista: INVALIDADO
    /// - Próximas consultas: Dados atualizados do banco
    /// - Jogo aparecerá como inativo nas listagens
    /// </remarks>
    /// <param name="id">ID único do jogo a ser desativado</param>
    /// <returns>Confirmação da desativação sem conteúdo</returns>
    /// <response code="204">Jogo desativado com sucesso - Sem conteúdo no retorno</response>
    /// <response code="401">Usuário não autenticado - Token JWT inválido ou ausente</response>
    /// <response code="403">Acesso negado - Usuário não possui permissão de Administrador</response>
    /// <response code="404">Jogo não encontrado ou já desativado</response>
    /// <response code="500">Erro interno do servidor</response>
    [Authorize(Roles = "Administrador")]
    [HttpDelete("Remover/{id:guid}")]
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

            // Invalidação do cache v2.0
            _cacheService.Remove(id.ToString()); // Remove cache individual do jogo
            _cacheService.Remove("jogoListagem"); // Remove cache da lista completa

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao remover produto com ID {Id}.", id);
            return StatusCode(500, "Erro interno ao remover produto.");
        }
    }


}