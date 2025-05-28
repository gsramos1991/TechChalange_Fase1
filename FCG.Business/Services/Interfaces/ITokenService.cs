using FCG.Business.Models;

namespace FCG.Business.Services.Interfaces;

public interface ITokenService
{
    Task<string> GenerateToken(Usuario user);
}
