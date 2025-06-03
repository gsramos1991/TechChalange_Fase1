namespace FCG.Core.Services.Interfaces;

public interface ICorrelationIdGenerator
{
    string Get();
    void Set(string correlationId);
}
