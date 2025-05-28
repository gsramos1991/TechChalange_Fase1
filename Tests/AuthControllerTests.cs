using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using FluentAssertions;
using FCG.Api.Controllers.v1;
using FCG.Api.Dto;
using FCG.Business.Models;
using FCG.Business.Services.Interfaces;
using FCG.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FCG.Api.Tests.Controllers.v1
{
    public class AuthControllerTests : IDisposable
    {
        private readonly Mock<UserManager<Usuario>> _mockUserManager;
        private readonly Mock<SignInManager<Usuario>> _mockSignInManager;
        private readonly Mock<ITokenService> _mockTokenService;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly ApplicationDbContext _dbContext;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            // Setup UserManager mock
            var userStore = new Mock<IUserStore<Usuario>>();
            _mockUserManager = new Mock<UserManager<Usuario>>(
                userStore.Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<IPasswordHasher<Usuario>>().Object,
                new IUserValidator<Usuario>[0],
                new IPasswordValidator<Usuario>[0],
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<IServiceProvider>().Object,
                new Mock<ILogger<UserManager<Usuario>>>().Object);

            // Setup SignInManager mock
            var contextAccessor = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
            var claimsFactory = new Mock<IUserClaimsPrincipalFactory<Usuario>>();
            _mockSignInManager = new Mock<SignInManager<Usuario>>(
                _mockUserManager.Object,
                contextAccessor.Object,
                claimsFactory.Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<ILogger<SignInManager<Usuario>>>().Object,
                new Mock<Microsoft.AspNetCore.Authentication.IAuthenticationSchemeProvider>().Object,
                new Mock<IUserConfirmation<Usuario>>().Object);

            _mockTokenService = new Mock<ITokenService>();
            _mockConfiguration = new Mock<IConfiguration>();

            // Criar um ApplicationDbContext usando SQLite InMemory database para os testes (suporta transações)
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite("DataSource=:memory:")
                .Options;
            _dbContext = new ApplicationDbContext(options);

            // Garante que o banco seja criado
            _dbContext.Database.OpenConnection();
            _dbContext.Database.EnsureCreated();

            _controller = new AuthController(
                _mockUserManager.Object,
                _mockSignInManager.Object,
                _mockTokenService.Object,
                _mockConfiguration.Object,
                _dbContext);
        }

        public void Dispose()
        {
            _dbContext?.Database?.CloseConnection();
            _dbContext?.Dispose();
        }

        [Fact]
        public async Task Registro_ComModeloValido_DeveRetornarOk()
        {
            // Arrange
            var registroModel = new RegistroModel
            {
                Email = "teste@exemplo.com",
                Nome = "Usuário Teste",
                Password = "MinhaSenh@123"
            };

            var usuario = new Usuario
            {
                UserName = registroModel.Email,
                Email = registroModel.Email,
                Nome = registroModel.Nome
            };

            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<Usuario>(), registroModel.Password))
                .ReturnsAsync(IdentityResult.Success);

            _mockUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<Usuario>(), "Usuario"))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.Registro(registroModel);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Value.Should().NotBeNull();

            var response = okResult.Value.ToString();
            response.Should().Contain("Usuário criado com sucesso");
        }

        [Fact]
        public async Task Registro_ComModeloInvalido_DeveRetornarBadRequest()
        {
            // Arrange
            var registroModel = new RegistroModel(); // Modelo vazio/inválido
            _controller.ModelState.AddModelError("Email", "O campo Email é obrigatório");

            // Act
            var result = await _controller.Registro(registroModel);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Registro_QuandoCriacaoUsuarioFalha_DeveRetornarBadRequest()
        {
            // Arrange
            var registroModel = new RegistroModel
            {
                Email = "teste@exemplo.com",
                Nome = "Usuário Teste",
                Password = "senha123"
            };

            var identityErrors = new List<IdentityError>
            {
                new IdentityError { Code = "PasswordTooShort", Description = "Senha muito curta" }
            };

            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<Usuario>(), registroModel.Password))
                .ReturnsAsync(IdentityResult.Failed(identityErrors.ToArray()));

            // Act
            var result = await _controller.Registro(registroModel);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Value.Should().BeEquivalentTo(identityErrors);
        }

        [Fact]
        public async Task Registro_QuandoAssociacaoRoleFalha_DeveRetornarBadRequest()
        {
            // Arrange
            var registroModel = new RegistroModel
            {
                Email = "teste@exemplo.com",
                Nome = "Usuário Teste",
                Password = "MinhaSenh@123"
            };

            var roleErrors = new List<IdentityError>
            {
                new IdentityError { Code = "RoleNotFound", Description = "Role não encontrada" }
            };

            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<Usuario>(), registroModel.Password))
                .ReturnsAsync(IdentityResult.Success);

            _mockUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<Usuario>(), "Usuario"))
                .ReturnsAsync(IdentityResult.Failed(roleErrors.ToArray()));

            // Act
            var result = await _controller.Registro(registroModel);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Value.Should().BeEquivalentTo(roleErrors);
        }

        [Fact]
        public async Task Registro_QuandoOcorreExcecao_DeveRetornarStatusCode500()
        {
            // Arrange
            var registroModel = new RegistroModel
            {
                Email = "teste@exemplo.com",
                Nome = "Usuário Teste",
                Password = "MinhaSenh@123"
            };

            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<Usuario>(), registroModel.Password))
                .ThrowsAsync(new Exception("Erro interno"));

            // Act
            var result = await _controller.Registro(registroModel);

            // Assert
            result.Should().BeOfType<ObjectResult>();
            var objectResult = result as ObjectResult;
            objectResult.StatusCode.Should().Be(500);
            objectResult.Value.Should().Be("Ocorreu um erro interno ao registrar o usuário.");
        }

        [Theory]
        [InlineData("", "Nome Teste", "Senha123", "Email")]
        [InlineData("email@teste.com", "", "Senha123", "Nome")]
        [InlineData("email@teste.com", "Nome Teste", "", "Password")]
        public async Task Registro_ComCamposObrigatoriosVazios_DeveRetornarBadRequest(
            string email, string nome, string password, string campoInvalido)
        {
            // Arrange
            var registroModel = new RegistroModel
            {
                Email = email,
                Nome = nome,
                Password = password
            };

            _controller.ModelState.AddModelError(campoInvalido, $"O campo {campoInvalido} é obrigatório");

            // Act
            var result = await _controller.Registro(registroModel);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Registro_DeveConfigurarUsuarioCorretamente()
        {
            // Arrange
            var registroModel = new RegistroModel
            {
                Email = "teste@exemplo.com",
                Nome = "Usuário Teste",
                Password = "MinhaSenh@123"
            };

            Usuario usuarioCriado = null;

            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<Usuario>(), registroModel.Password))
                .Callback<Usuario, string>((user, password) => usuarioCriado = user)
                .ReturnsAsync(IdentityResult.Success);

            _mockUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<Usuario>(), "Usuario"))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            await _controller.Registro(registroModel);

            // Assert
            usuarioCriado.Should().NotBeNull();
            usuarioCriado.UserName.Should().Be(registroModel.Email);
            usuarioCriado.Email.Should().Be(registroModel.Email);
            usuarioCriado.Nome.Should().Be(registroModel.Nome);
        }

        [Fact]
        public async Task Registro_DeveAdicionarUsuarioARole()
        {
            // Arrange
            var registroModel = new RegistroModel
            {
                Email = "teste@exemplo.com",
                Nome = "Usuário Teste",
                Password = "MinhaSenh@123"
            };

            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<Usuario>(), registroModel.Password))
                .ReturnsAsync(IdentityResult.Success);

            _mockUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<Usuario>(), "Usuario"))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            await _controller.Registro(registroModel);

            // Assert
            _mockUserManager.Verify(x => x.AddToRoleAsync(It.IsAny<Usuario>(), "Usuario"), Times.Once);
        }
    }
}