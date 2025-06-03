using FCG.Business.Models;
using Microsoft.AspNetCore.Identity;
using System;
using Xunit;

namespace FCG.Tests;

public class UsuarioTests
{
    #region Testes do Construtor - Casos de Sucesso

    [Fact]
    public void Construtor_ComDadosValidos_DeveCriarUsuarioCorretamente()
    {
        // Arrange
        var email = "usuario123@exemplo.com";
        var nome = "João Silva";

        // Act
        var usuario = new Usuario(email, nome);

        // Assert
        Assert.Equal(email, usuario.Email);
        Assert.Equal(email, usuario.UserName);
        Assert.Equal(nome, usuario.Nome);
        Assert.False(usuario.EmailConfirmed);
    }

    [Fact]
    public void Construtor_ComEmailConfirmado_DeveCriarUsuarioComEmailConfirmado()
    {
        // Arrange
        var email = "usuario123@exemplo.com";
        var nome = "João Silva";
        var emailConfirmado = true;

        // Act
        var usuario = new Usuario(email, nome, emailConfirmado);

        // Assert
        Assert.Equal(email, usuario.Email);
        Assert.Equal(email, usuario.UserName);
        Assert.Equal(nome, usuario.Nome);
        Assert.True(usuario.EmailConfirmed);
    }

    #endregion

    #region Testes do Construtor - Validação de Email

    [Fact]
    public void Construtor_ComEmailNulo_DeveLancarArgumentException()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Usuario(null, "João Silva"));

        Assert.Equal("E-mail é obrigatório.", exception.Message);
    }

    [Fact]
    public void Construtor_ComEmailVazio_DeveLancarArgumentException()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Usuario("", "João Silva"));

        Assert.Equal("E-mail é obrigatório.", exception.Message);
    }

    [Fact]
    public void Construtor_ComEmailApenasEspacos_DeveLancarArgumentException()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Usuario("   ", "João Silva"));

        Assert.Equal("E-mail é obrigatório.", exception.Message);
    }

    [Fact]
    public void Construtor_ComEmailSemArroba_DeveLancarArgumentException()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Usuario("usuarioemail.com", "João Silva"));

        Assert.Equal("E-mail inválido. Deve ter pelo menos 8 caracteres antes do @ e um domínio válido.", exception.Message);
    }

    [Fact]
    public void Construtor_ComEmailSemDominio_DeveLancarArgumentException()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Usuario("usuario@", "João Silva"));

        Assert.Equal("E-mail inválido. Deve ter pelo menos 8 caracteres antes do @ e um domínio válido.", exception.Message);
    }

    [Fact]
    public void Construtor_ComEmailSemExtensao_DeveLancarArgumentException()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Usuario("usuario@exemplo", "João Silva"));

        Assert.Equal("E-mail inválido. Deve ter pelo menos 8 caracteres antes do @ e um domínio válido.", exception.Message);
    }

    [Fact]
    public void Construtor_ComEmailMenosDe8CaracteresAntesDoArroba_DeveLancarArgumentException()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Usuario("user123@exemplo.com", "João Silva"));

        Assert.Equal("E-mail inválido. Deve ter pelo menos 8 caracteres antes do @ e um domínio válido.", exception.Message);
    }

    [Fact]
    public void Construtor_ComEmailExatamente8CaracteresAntesDoArroba_DeveAceitar()
    {
        // Arrange
        var email = "12345678@exemplo.com";

        // Act
        var usuario = new Usuario(email, "João Silva");

        // Assert
        Assert.Equal(email, usuario.Email);
    }

    [Fact]
    public void Construtor_ComEmailComEspacos_DeveLancarArgumentException()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Usuario("usuario teste@exemplo.com", "João Silva"));

        Assert.Equal("E-mail inválido. Deve ter pelo menos 8 caracteres antes do @ e um domínio válido.", exception.Message);
    }

    #endregion

    #region Testes do Construtor - Validação de Nome

    [Fact]
    public void Construtor_ComNomeNulo_DeveLancarArgumentException()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Usuario("usuario123@exemplo.com", null));

        Assert.Equal("Nome é obrigatório.", exception.Message);
    }

    [Fact]
    public void Construtor_ComNomeVazio_DeveLancarArgumentException()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Usuario("usuario123@exemplo.com", ""));

        Assert.Equal("Nome é obrigatório.", exception.Message);
    }

    [Fact]
    public void Construtor_ComNomeApenasEspacos_DeveLancarArgumentException()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Usuario("usuario123@exemplo.com", "   "));

        Assert.Equal("Nome é obrigatório.", exception.Message);
    }

    [Fact]
    public void Construtor_ComNomeMaisDe50Caracteres_DeveLancarArgumentException()
    {
        // Arrange
        var nomeGrande = new string('A', 51);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Usuario("usuario123@exemplo.com", nomeGrande));

        Assert.Equal("Nome não pode ter mais que 50 caracteres.", exception.Message);
    }

    [Fact]
    public void Construtor_ComNomeExatamente50Caracteres_DeveAceitar()
    {
        // Arrange
        var nome = new string('A', 50);

        // Act
        var usuario = new Usuario("usuario123@exemplo.com", nome);

        // Assert
        Assert.Equal(nome, usuario.Nome);
    }

    #endregion

    #region Testes do Método SetNome

    [Fact]
    public void SetNome_ComNomeValido_DeveAtualizarNome()
    {
        // Arrange
        var usuario = new Usuario("usuario123@exemplo.com", "Nome Original");
        var novoNome = "Novo Nome";

        // Act
        usuario.SetNome(novoNome);

        // Assert
        Assert.Equal(novoNome, usuario.Nome);
    }

    [Fact]
    public void SetNome_ComNomeNulo_DeveLancarArgumentException()
    {
        // Arrange
        var usuario = new Usuario("usuario123@exemplo.com", "Nome Original");

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            usuario.SetNome(null));

        Assert.Equal("Nome é obrigatório.", exception.Message);
    }

    [Fact]
    public void SetNome_ComNomeVazio_DeveLancarArgumentException()
    {
        // Arrange
        var usuario = new Usuario("usuario123@exemplo.com", "Nome Original");

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            usuario.SetNome(""));

        Assert.Equal("Nome é obrigatório.", exception.Message);
    }

    [Fact]
    public void SetNome_ComNomeMaisDe50Caracteres_DeveLancarArgumentException()
    {
        // Arrange
        var usuario = new Usuario("usuario123@exemplo.com", "Nome Original");
        var nomeGrande = new string('A', 51);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            usuario.SetNome(nomeGrande));

        Assert.Equal("Nome não pode ter mais que 50 caracteres.", exception.Message);
    }

    #endregion

    #region Testes do Método SetEmail

    [Fact]
    public void SetEmail_ComEmailValido_DeveAtualizarEmailEUserName()
    {
        // Arrange
        var usuario = new Usuario("original123@exemplo.com", "João Silva");
        var novoEmail = "novoemail@exemplo.com";

        // Act
        usuario.SetEmail(novoEmail);

        // Assert
        Assert.Equal(novoEmail, usuario.Email);
        Assert.Equal(novoEmail, usuario.UserName);
    }

    [Fact]
    public void SetEmail_ComEmailNulo_DeveLancarArgumentException()
    {
        // Arrange
        var usuario = new Usuario("usuario123@exemplo.com", "João Silva");

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            usuario.SetEmail(null));

        Assert.Equal("E-mail é obrigatório.", exception.Message);
    }

    [Fact]
    public void SetEmail_ComEmailInvalido_DeveLancarArgumentException()
    {
        // Arrange
        var usuario = new Usuario("usuario123@exemplo.com", "João Silva");

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            usuario.SetEmail("emailinvalido"));

        Assert.Equal("E-mail inválido. Deve ter pelo menos 8 caracteres antes do @ e um domínio válido.", exception.Message);
    }

    #endregion

    #region Testes do Método ValidarSenha

    [Fact]
    public void ValidarSenha_ComSenhaValida_NaoDeveLancarExcecao()
    {
        // Arrange
        var senha = "Senha@123";

        // Act & Assert
        var exception = Record.Exception(() => Usuario.ValidarSenha(senha));
        Assert.Null(exception);
    }

    [Fact]
    public void ValidarSenha_ComSenhaNula_DeveLancarArgumentException()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            Usuario.ValidarSenha(null));

        Assert.Equal("Senha é obrigatória.", exception.Message);
    }

    [Fact]
    public void ValidarSenha_ComSenhaVazia_DeveLancarArgumentException()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            Usuario.ValidarSenha(""));

        Assert.Equal("Senha é obrigatória.", exception.Message);
    }

    [Fact]
    public void ValidarSenha_ComSenhaApenasEspacos_DeveLancarArgumentException()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            Usuario.ValidarSenha("   "));

        Assert.Equal("Senha é obrigatória.", exception.Message);
    }

    [Fact]
    public void ValidarSenha_ComSenhaMenosDe8Caracteres_DeveLancarArgumentException()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            Usuario.ValidarSenha("Abc@123"));

        Assert.Equal("Senha deve ter no mínimo 8 caracteres.", exception.Message);
    }

    [Fact]
    public void ValidarSenha_ComSenhaSemNumero_DeveLancarArgumentException()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            Usuario.ValidarSenha("SenhaSemNumero@"));

        Assert.Equal("Senha deve conter pelo menos um número.", exception.Message);
    }

    [Fact]
    public void ValidarSenha_ComSenhaSemLetra_DeveLancarArgumentException()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            Usuario.ValidarSenha("12345678@"));

        Assert.Equal("Senha deve conter pelo menos uma letra.", exception.Message);
    }

    [Fact]
    public void ValidarSenha_ComSenhaSemLetraMaiuscula_DeveLancarArgumentException()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            Usuario.ValidarSenha("senhasemcapital123@"));

        Assert.Equal("Senha deve conter pelo menos uma letra maiúscula.", exception.Message);
    }

    [Fact]
    public void ValidarSenha_ComSenhaSemCaractereEspecial_DeveLancarArgumentException()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            Usuario.ValidarSenha("SenhaSemEspecial123"));

        Assert.Equal("Senha deve conter pelo menos um caractere especial.", exception.Message);
    }

    [Fact]
    public void ValidarSenha_ComSenhasValidasVariadas_NaoDeveLancarExcecao()
    {
        // Arrange
        var senhasValidas = new[]
        {
            "Senha@123",
            "MinhaSenha!999",
            "Teste#2024",
            "Complexa$Senha1",
            "P@ssw0rd",
            "Admin!234"
        };

        // Act & Assert
        foreach (var senha in senhasValidas)
        {
            var exception = Record.Exception(() => Usuario.ValidarSenha(senha));
            Assert.Null(exception);
        }
    }

    #endregion

    #region Testes de Cenários Específicos

    [Fact]
    public void Usuario_AlterarEmailMultiplasVezes_DeveManterConsistencia()
    {
        // Arrange
        var usuario = new Usuario("primeiro12@exemplo.com", "João Silva");
        var emails = new[] { "segundo12@exemplo.com", "terceiro1@exemplo.com", "quarto123@exemplo.com" };

        // Act & Assert
        foreach (var email in emails)
        {
            usuario.SetEmail(email);
            Assert.Equal(email, usuario.Email);
            Assert.Equal(email, usuario.UserName);
        }
    }

    [Fact]
    public void Usuario_AlterarNomeMultiplasVezes_DeveManterUltimoValor()
    {
        // Arrange
        var usuario = new Usuario("usuario123@exemplo.com", "Nome Original");
        var nomes = new[] { "João", "Maria", "Pedro", "Ana" };

        // Act & Assert
        foreach (var nome in nomes)
        {
            usuario.SetNome(nome);
            Assert.Equal(nome, usuario.Nome);
        }
    }

    [Fact]
    public void Usuario_ComEmailComplexo_DeveAceitar()
    {
        // Arrange
        var emailsComplexos = new[]
        {
            "usuario.teste@exemplo.com",
            "usuario+tag@exemplo.com",
            "usuario_teste@exemplo.com.br",
            "usuario123@sub.dominio.com",
            "nome.sobrenome@empresa.org"
        };

        // Act & Assert
        foreach (var email in emailsComplexos)
        {
            var usuario = new Usuario(email, "Nome Teste");
            Assert.Equal(email, usuario.Email);
            Assert.Equal(email, usuario.UserName);
        }
    }

    #endregion
}