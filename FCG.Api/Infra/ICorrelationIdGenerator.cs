namespace FCG.Api.Infra;

public interface ICorrelationIdGenerator
{
    string Get();
    void Set(string correlationId);
}
