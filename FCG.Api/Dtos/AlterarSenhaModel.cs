using System.ComponentModel.DataAnnotations;

namespace FCG.Api.Dtos;

/// <summary>
/// Modelo para alteração de senha do usuário
/// </summary>
public class AlterarSenhaModel
{
    [Required(ErrorMessage = "A senha atual é obrigatória")]
    [DataType(DataType.Password)]
    public string SenhaAtual { get; set; }

    [Required(ErrorMessage = "A nova senha é obrigatória")]
    [StringLength(100, ErrorMessage = "A senha deve ter pelo menos {2} caracteres.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    public string NovaSenha { get; set; }

    [Required(ErrorMessage = "A confirmação da nova senha é obrigatória")]
    [DataType(DataType.Password)]
    [Compare("NovaSenha", ErrorMessage = "A nova senha e a confirmação não coincidem.")]
    public string ConfirmaNovaSenha { get; set; }
}
