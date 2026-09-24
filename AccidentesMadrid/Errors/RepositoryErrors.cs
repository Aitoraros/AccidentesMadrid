namespace AccidentesMadrid.Errors;

public static class RepositoryErrors
{
    public static DomainError FileNotFound(string path) =>
        new("Repository.FileNotFound", $"No se ha encontrado el archivo '{path}'.");

    public static DomainError InvalidFormat(string detalle) =>
        new("Repository.InvalidFormat", $"El formato del archivo no es válido: {detalle}");

    public static DomainError WriteError(string detalle) =>
        new("Repository.WriteError", $"Error al escribir el archivo: {detalle}");
}