using AccidentesMadrid.Models;
using CSharpFunctionalExtensions;

namespace AccidentesMadrid.Repositories;

public interface IAccidentesRepository
{
    Result<IEnumerable<Accidente>, string> Cargar(string path);
    Result<IEnumerable<Accidente>, string> CargarTodos(IEnumerable<string> paths);
    Task<Result<IEnumerable<Accidente>, string>> CargarTodosParaleloAsync(IEnumerable<string> paths);
}