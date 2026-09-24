namespace AccidentesMadrid.Errors;

public record DomainError(string Code, string Message)
{
    public override string ToString() => $"[{Code}] {Message}";
}