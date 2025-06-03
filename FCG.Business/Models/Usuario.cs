using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FCG.Business.Models;

public class Usuario : IdentityUser
{
    private string _nome;

    protected Usuario() { } 

    public Usuario(string email, string nome)
    {
        SetEmail(email);
        SetNome(nome);
    }

    public Usuario(string email, string nome, bool emailConfirmed) : this(email, nome)
    {
        EmailConfirmed = emailConfirmed;
    }

    public string Nome
    {
        get => _nome;
        private set => _nome = value;
    }

    public void SetNome(string nome)
    {
        ValidarNome(nome);
        _nome = nome;
    }

    public void SetEmail(string email)
    {
        ValidarEmail(email);
        Email = email;
        UserName = email;
    }

    public static void ValidarSenha(string senha)
    {
        if (string.IsNullOrWhiteSpace(senha))
            throw new ArgumentException("Senha é obrigatória.");

        if (senha.Length < 8)
            throw new ArgumentException("Senha deve ter no mínimo 8 caracteres.");

        if (!Regex.IsMatch(senha, @"[0-9]"))
            throw new ArgumentException("Senha deve conter pelo menos um número.");

        if (!Regex.IsMatch(senha, @"[a-zA-Z]"))
            throw new ArgumentException("Senha deve conter pelo menos uma letra.");

        if (!Regex.IsMatch(senha, @"[A-Z]"))
            throw new ArgumentException("Senha deve conter pelo menos uma letra maiúscula.");

        if (!Regex.IsMatch(senha, @"[^a-zA-Z0-9]"))
            throw new ArgumentException("Senha deve conter pelo menos um caractere especial.");
    }

    private static void ValidarNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome é obrigatório.");

        if (nome.Length > 50)
            throw new ArgumentException("Nome não pode ter mais que 50 caracteres.");
    }

    private static void ValidarEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("E-mail é obrigatório.");

        string pattern = @"^[^@\s]{8,}@[^@\s]+\.[^@\s]+$";
        if (!Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase))
            throw new ArgumentException("E-mail inválido. Deve ter pelo menos 8 caracteres antes do @ e um domínio válido.");
    }
}