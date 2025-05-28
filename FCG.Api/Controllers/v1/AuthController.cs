using Asp.Versioning;
using FCG.Api.Dto;
using FCG.Business.Models;
using FCG.Business.Services.Interfaces;
using FCG.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using IdentitySignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace FCG.Api.Controllers.v1;



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

    [HttpPost("registro")]
    public async Task<IActionResult> Registro([FromBody] RegistroModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await using var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            var user = new Usuario
            {
                UserName = model.Email,
                Email = model.Email,
                Nome = model.Nome
            };

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
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return StatusCode(500, "Ocorreu um erro interno ao registrar o usuário.");
        }
    }
    [HttpPost("login")]
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
}
