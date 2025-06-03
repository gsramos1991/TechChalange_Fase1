using Asp.Versioning;
using FCG.Api.Dto;
using FCG.Api.Dtos;
using FCG.Business.Models;
using FCG.Business.Services.Interfaces;
using FCG.Infra.Data;
using FCG.Infra.Token;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using IdentitySignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace FCG.Api.Controllers.v1;

/// <summary>
/// Controller para autenticação e autorização de usuários
/// </summary>
/// <remarks>
/// Gerencia o registro de novos usuários e autenticação no sistema FCG.
/// </remarks>
[ApiController]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<Usuario> _userManager;
    private readonly SignInManager<Usuario> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _configuration;
    private readonly ApplicationDbContext _dbContext;

    public AuthController(
        UserManager<Usuario> userManager,
        SignInManager<Usuario> signInManager,
        ITokenService tokenService,
        IConfiguration configuration,
        ApplicationDbContext dbContext)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _configuration = configuration;
        _dbContext = dbContext;
    }


    /// <summary>
    /// Registra um novo usuário no sistema
    /// </summary>
    /// <remarks>
    /// Cria uma nova conta de usuário no sistema FCG com as credenciais fornecidas.
    /// 
    /// **Processo de registro:**
    /// 1. 🔐 **Validação:** Verifica se os dados fornecidos são válidos
    /// 2. 👤 **Criação do usuário:** Cria o usuário com email e senha
    /// 3. 🏷️ **Atribuição de role:** Associa automaticamente à role "Usuario"
    /// 
    /// **Regras de negócio:**
    /// - Email deve ser único no sistema
    /// - Senha deve atender aos critérios de segurança configurados
    /// - Todos os novos usuários recebem automaticamente a role "Usuario"
    /// - Se houver falha na atribuição da role, o usuário não é criado
    /// 
    /// **Exemplo de payload:**
    /// ```json
    /// {
    ///   "nome": "João Silva",
    ///   "email": "joao.silva@email.com",
    ///   "password": "MinhaSenh@123",
    ///   "confirmPassword": "MinhaSenh@123"
    /// }
    /// ```
    /// 
    /// **Exemplo de resposta de sucesso:**
    /// ```json
    /// {
    ///   "message": "Usuário criado com sucesso e associado à role Usuario!"
    /// }
    /// ```
    /// </remarks>
    /// <param name="model">Dados para registro do novo usuário</param>
    /// <returns>Confirmação do registro do usuário</returns>
    /// <response code="200">Usuário registrado com sucesso</response>
    /// <response code="400">Dados inválidos ou erro de validação</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPost("registro")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Registro([FromBody] RegistroModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await using var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            // Criação do usuário com validações de domínio
            var user = new Usuario(model.Email, model.Nome);

            // Valida a senha antes de prosseguir
            Usuario.ValidarSenha(model.Password);

            // Verifica se o e-mail já está em uso
            var emailExistente = await _userManager.FindByEmailAsync(model.Email);
            if (emailExistente != null)
            {
                return BadRequest(new { error = "E-mail já está em uso." });
            }

            // 1. Cria o usuário
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            // 2. Associa o usuário à role
            var roleResult = await _userManager.AddToRoleAsync(user, "Usuario");
            if (!roleResult.Succeeded)
            {
                // Se falhar, a transação será revertida e o usuário NÃO será criado
                await transaction.RollbackAsync();
                return BadRequest(roleResult.Errors);
            }

            await transaction.CommitAsync();
            return Ok(new { message = "Usuário criado com sucesso e associado à role Usuario!" });
        }
        catch (ArgumentException ex)
        {
            await transaction.RollbackAsync();
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return StatusCode(500, "Ocorreu um erro interno ao registrar o usuário.");
        }
    }

    /// <summary>
    /// Autentica um usuário no sistema
    /// </summary>
    /// <remarks>
    /// Realiza a autenticação do usuário e retorna um token JWT para acesso aos recursos protegidos.
    /// 
    /// **Processo de login:**
    /// 1. 🔍 **Validação:** Verifica se os dados fornecidos são válidos
    /// 2. 👤 **Busca do usuário:** Localiza o usuário pelo email
    /// 3. 🔐 **Verificação da senha:** Valida as credenciais fornecidas
    /// 4. 🎫 **Geração do token:** Cria um token JWT com as informações do usuário
    /// 5. ✅ **Retorno dos dados:** Envia token e informações básicas do usuário
    /// 
    /// **Segurança:**
    /// - Senhas são verificadas usando hash seguro
    /// - Não expõe informações sensíveis em caso de erro
    /// 
    /// **Exemplo de payload:**
    /// ```json
    /// {
    ///   "email": "joao.silva@email.com",
    ///   "password": "MinhaSenh@123"
    /// }
    /// ```
    /// 
    /// **Exemplo de resposta de sucesso:**
    /// ```json
    /// {
    ///   "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    ///   "user": {
    ///     "id": "123e4567-e89b-12d3-a456-426614174000",
    ///     "userName": "joao.silva@email.com",
    ///     "email": "joao.silva@email.com",
    ///     "nome": "João Silva"
    ///   },
    ///   "expiration": "2025-06-01T18:30:00.000Z"
    /// }
    /// ```
    /// 
    /// **Como usar o token:**
    /// - Incluir o token no cabeçalho de todas as requisições protegidas
    /// - Formato: `Authorization: Bearer {token}`
    /// - Token é válido até a data de expiração retornada
    /// </remarks>
    /// <param name="model">Credenciais de login (email e senha)</param>
    /// <returns>Token JWT e informações do usuário autenticado</returns>
    /// <response code="200">Login realizado com sucesso</response>
    /// <response code="400">Dados de entrada inválidos</response>
    /// <response code="401">Credenciais inválidas (usuário ou senha incorretos)</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _userManager.FindByNameAsync(model.Email);
        if (user == null)
        {
            return Unauthorized(new { message = "Usuário ou senha inválidos." });
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
        if (result.Succeeded)
        {
            var token = await _tokenService.GenerateToken(user);
            return Ok(new
            {
                token,
                user = new
                {
                    id = user.Id,
                    userName = user.UserName,
                    email = user.Email,
                    nome = user.Nome
                },
                expiration = DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:DurationInMinutes"]))
            });
        }

        return Unauthorized(new { message = "Usuário ou senha inválidos." });
    }


    /// <summary>
    /// Altera a senha do usuário autenticado
    /// </summary>
    /// <remarks>
    /// Permite que o usuário autenticado altere sua senha fornecendo a senha atual e a nova senha.
    /// 
    /// **Processo de alteração de senha:**
    /// 1. 🔐 **Autenticação:** Verifica se o usuário está autenticado
    /// 2. 🔍 **Validação:** Valida os dados fornecidos
    /// 3. 👤 **Busca do usuário:** Localiza o usuário pelo ID do token
    /// 4. 🔑 **Verificação:** Confirma se a senha atual está correta
    /// 5. 🔄 **Alteração:** Atualiza para a nova senha
    /// 6. ✅ **Confirmação:** Retorna sucesso ou erro
    /// 
    /// **Requisitos de segurança:**
    /// - Usuário deve estar autenticado (token JWT válido)
    /// - Deve fornecer a senha atual correta
    /// - Nova senha deve atender aos critérios de segurança configurados
    /// 
    /// **Exemplo de payload:**
    /// ```json
    /// {
    ///   "senhaAtual": "MinhaSenh@123",
    ///   "novaSenha": "NovaSenha@456",
    ///   "confirmaNovaSenha": "NovaSenha@456"
    /// }
    /// ```
    /// 
    /// **Exemplo de resposta de sucesso:**
    /// ```json
    /// {
    ///   "message": "Senha alterada com sucesso!"
    /// }
    /// ```
    /// 
    /// **Exemplo de resposta de erro:**
    /// ```json
    /// {
    ///   "message": "Senha atual incorreta."
    /// }
    /// ```
    /// </remarks>
    /// <param name="model">Dados para alteração de senha</param>
    /// <returns>Confirmação da alteração de senha</returns>
    /// <response code="200">Senha alterada com sucesso</response>
    /// <response code="400">Dados inválidos ou senha atual incorreta</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="500">Erro interno do servidor</response>
    [Authorize]
    [HttpPost("alterar-senha")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AlterarSenha([FromBody] AlterarSenhaModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            // Obtém o ID do usuário do token JWT
            string? userLogin = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(userLogin))
            {
                return Unauthorized(new { message = "Usuário não autenticado." });
            }

            // Busca o usuário
            var user = await _userManager.FindByNameAsync(userLogin);
            if (user == null)
            {
                return Unauthorized(new { message = "Usuário não encontrado." });
            }

            // Verifica se a senha atual está correta
            var senhaCorreta = await _userManager.CheckPasswordAsync(user, model.SenhaAtual);
            if (!senhaCorreta)
            {
                return BadRequest(new { message = "Senha atual incorreta." });
            }

            // Altera a senha
            var result = await _userManager.ChangePasswordAsync(user, model.SenhaAtual, model.NovaSenha);
            if (result.Succeeded)
            {
                return Ok(new { message = "Senha alterada com sucesso!" });
            }

            // Retorna os erros se houver
            return BadRequest(result.Errors);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Ocorreu um erro interno ao alterar a senha.");
        }
    }

}