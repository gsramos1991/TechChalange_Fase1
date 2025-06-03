using System.ComponentModel.DataAnnotations;

namespace FCG.Api.Dto;

public class RegistroModel
{
    [Required(ErrorMessage = "E-mail é obrigatório")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Nome é obrigatório")]
    public string Nome { get; set; }

    [Required(ErrorMessage = "Senha é obrigatória")]
    [MinLength(8, ErrorMessage = "Senha deve ter no mínimo 8 caracteres")]
    public string Password { get; set; }
}

