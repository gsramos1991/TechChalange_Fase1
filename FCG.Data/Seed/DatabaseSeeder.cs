using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FCG.Business.Models;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using FCG.Business.Models;


namespace FCG.Data.Seed;

public class DatabaseSeeder
{
    private readonly UserManager<Usuario> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(
        UserManager<Usuario> userManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration,
        ILogger<DatabaseSeeder> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        await SeedRolesAsync();
        await SeedAdminUserAsync();
    }

    private async Task SeedRolesAsync()
    {
        string[] roles = { "Administrador", "Usuario" };

        foreach (string role in roles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                var identityRole = new IdentityRole(role);
                var result = await _roleManager.CreateAsync(identityRole);

                if (result.Succeeded)
                {
                    _logger.LogInformation($"Role '{role}' criada com sucesso");
                }
                else
                {
                    _logger.LogError($"Erro ao criar role '{role}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
        }
    }

    private async Task SeedAdminUserAsync()
    {
        var adminEmail = _configuration["AdminUser:Email"];
        var adminPassword = _configuration["AdminUser:Password"];
        var adminNome = _configuration["AdminUser:Nome"];

        var adminUser = await _userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            adminUser = new Usuario
            {
                UserName = adminEmail,
                Email = adminEmail,
                Nome = adminNome,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(adminUser, adminPassword);

            if (result.Succeeded)
            {
                var roleResult = await _userManager.AddToRoleAsync(adminUser, "Administrador");

                if (roleResult.Succeeded)
                {
                    _logger.LogInformation($"Usuário administrador criado com sucesso: {adminEmail}");
                }
                else
                {
                    _logger.LogError($"Erro ao adicionar role ao usuário administrador: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                }
            }
            else
            {
                _logger.LogError($"Erro ao criar usuário administrador: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }
        else
        {
            // Verificar se já tem a role de administrador
            if (!await _userManager.IsInRoleAsync(adminUser, "Administrador"))
            {
                var roleResult = await _userManager.AddToRoleAsync(adminUser, "Administrador");

                if (roleResult.Succeeded)
                {
                    _logger.LogInformation($"Role 'Administrador' adicionada ao usuário existente: {adminEmail}");
                }
            }
            else
            {
                _logger.LogInformation($"Usuário administrador já existe: {adminEmail}");
            }
        }
    }
}