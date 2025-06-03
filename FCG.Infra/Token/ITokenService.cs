using FCG.Business.Models;

namespace FCG.Infra.Token;

public interface ITokenService
{
    Task<string> GenerateToken(Usuario user);
}
