using Moq;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using FCG.Api.Controllers;
using FCG.Business.Models;
using FCG.Business.Services.Interfaces;
using System.Collections.Generic;

namespace FCG.Tests.Controllers
{
    public class AccountControllerTests
    {
        private readonly Mock<UserManager<Usuario>> _userManagerMock;
        private readonly Mock<SignInManager<Usuario>> _signInManagerMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly AccountController _controller;

        public AccountControllerTests()
        {
            var userStoreMock = new Mock<IUserStore<Usuario>>();
            _userManagerMock = new Mock<UserManager<Usuario>>(userStoreMock.Object, null, null, null, null, null, null, null, null);

            var contextAccessor = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
            var userPrincipalFactory = new Mock<IUserClaimsPrincipalFactory<Usuario>>();
            _signInManagerMock = new Mock<SignInManager<Usuario>>(_userManagerMock.Object, contextAccessor.Object, userPrincipalFactory.Object, null, null, null, null);

            _tokenServiceMock = new Mock<ITokenService>();
            _configurationMock = new Mock<IConfiguration>();

            _controller = new AccountController(
                _userManagerMock.Object,
                _signInManagerMock.Object,
                _tokenServiceMock.Object,
                _configurationMock.Object
            );
        }

        [Fact]
        public async Task RegistrarRetornaOkQuandoUsuarioCriado()
        {
            var model = new RegisterModel
            {
                Email = "usuario@exemplo.com",
                Password = "Senha123!",
                Nome = "Usuário Teste"
            };

            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<Usuario>(), model.Password))
                .ReturnsAsync(IdentityResult.Success);

            var result = await _controller.Register(model);

            var okResult = Assert.IsType<Microsoft.AspNetCore.Mvc.OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task RegistrarRetornaBadRequestQuandoFalhaNaCriacao()
        {
            var model = new RegisterModel
            {
                Email = "usuario@exemplo.com",
                Password = "123",
                Nome = "Nome"
            };

            var identityErrors = new List<IdentityError> { new IdentityError { Description = "Erro" } };

            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<Usuario>(), model.Password))
                .ReturnsAsync(IdentityResult.Failed(identityErrors.ToArray()));

            var result = await _controller.Register(model);

            var badRequestResult = Assert.IsType<Microsoft.AspNetCore.Mvc.BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task LoginRetornaOkQuandoCredenciaisValidas()
        {
            var model = new LoginModel
            {
                Email = "usuario@exemplo.com",
                Password = "Senha123!"
            };

            var user = new Usuario
            {
                Id = "1",
                Email = model.Email,
                UserName = model.Email,
                Nome = "Teste"
            };

            _userManagerMock.Setup(x => x.FindByNameAsync(model.Email)).ReturnsAsync(user);
            _signInManagerMock.Setup(x => x.CheckPasswordSignInAsync(user, model.Password, false))
                .ReturnsAsync(SignInResult.Success);
            _tokenServiceMock.Setup(x => x.GenerateToken(user)).ReturnsAsync("fake-jwt-token");
            _configurationMock.Setup(x => x["Jwt:DurationInMinutes"]).Returns("60");

            var result = await _controller.Login(model);

            var okResult = Assert.IsType<Microsoft.AspNetCore.Mvc.OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task LoginRetornaNaoAutorizadoQuandoUsuarioNaoEncontrado()
        {
            var model = new LoginModel
            {
                Email = "naoencontrado@exemplo.com",
                Password = "Senha123"
            };

            _userManagerMock.Setup(x => x.FindByNameAsync(model.Email)).ReturnsAsync((Usuario)null);

            var result = await _controller.Login(model);

            var unauthorizedResult = Assert.IsType<Microsoft.AspNetCore.Mvc.UnauthorizedObjectResult>(result);
            Assert.Equal(401, unauthorizedResult.StatusCode);
        }

        [Fact]
        public async Task LoginRetornaNaoAutorizadoQuandoSenhaInvalida()
        {
            var model = new LoginModel
            {
                Email = "usuario@exemplo.com",
                Password = "SenhaErrada"
            };

            var user = new Usuario
            {
                Id = "1",
                Email = model.Email,
                UserName = model.Email,
                Nome = "Nome"
            };

            _userManagerMock.Setup(x => x.FindByNameAsync(model.Email)).ReturnsAsync(user);
            _signInManagerMock.Setup(x => x.CheckPasswordSignInAsync(user, model.Password, false))
                .ReturnsAsync(SignInResult.Failed);

            var result = await _controller.Login(model);

            var unauthorizedResult = Assert.IsType<Microsoft.AspNetCore.Mvc.UnauthorizedObjectResult>(result);
            Assert.Equal(401, unauthorizedResult.StatusCode);
        }
    }
}
