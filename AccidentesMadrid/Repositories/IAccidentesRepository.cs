using AccidentesMadrid.Errors;
using AccidentesMadrid.Models;
using CSharpFunctionalExtensions;

namespace AccidentesMadrid.Repositories;

public interface IAccidentesRepository
{
    Result<IEnumerable<Accidente>, DomainError> Cargar(string path);
    Result<IEnumerable<Accidente>, DomainError> CargarTodos(IEnumerable<string> paths);
}