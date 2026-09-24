using System.Globalization;
using System.Text;
using AccidentesMadrid.Config;
using AccidentesMadrid.Errors;
using AccidentesMadrid.Mappers;
using AccidentesMadrid.Models;
using CSharpFunctionalExtensions;
using CsvHelper;
using CsvHelper.Configuration;
using Serilog;

namespace AccidentesMadrid.Repositories;

public class AccidentesRepository : IAccidentesRepository
{
    private readonly ILogger _logger = Log.ForContext<AccidentesRepository>();

    public AccidentesRepository() : this(AppConfig.DataFolder) { }

    public AccidentesRepository(string dataFolder)
    {
        _logger.Debug("Inicializando AccidentesRepository...");
        InitStorage(dataFolder);
    }

    public Result<IEnumerable<Accidente>, DomainError> Cargar(string path)
    {
        _logger.Debug("Cargando accidentes desde el archivo '{path}'", path);

        if (!Path.Exists(path))
        {
            _logger.Warning("El archivo '{path}' no existe.", path);
            return Result.Failure<IEnumerable<Accidente>, DomainError>(RepositoryErrors.FileNotFound(path));
        }

        try
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
                HasHeaderRecord = true,
                MissingFieldFound = null,
                BadDataFound = null,
                TrimOptions = TrimOptions.Trim
            };

            using var reader = new StreamReader(path, Encoding.UTF8);
            using var csv = new CsvReader(reader, config);

            csv.Read();
            csv.ReadHeader();
            var cabecera = csv.HeaderRecord!;

            var accidentes = new List<Accidente>();
            while (csv.Read())
            {
                var campos = cabecera.ToDictionary(h => h, h => csv.GetField(h) ?? "");
                accidentes.Add(campos.ToAccidente());
            }

            return Result.Success<IEnumerable<Accidente>, DomainError>(accidentes);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Ha sucedido un error al cargar los accidentes desde el archivo '{path}'.", path);
            return Result.Failure<IEnumerable<Accidente>, DomainError>(RepositoryErrors.InvalidFormat(ex.Message));
        }
    }

    /// <inheritdoc cref="IAccidentesRepository.CargarTodos"/>
    public Result<IEnumerable<Accidente>, DomainError> CargarTodos(IEnumerable<string> paths)
    {
        var todos = new List<Accidente>();

        foreach (var path in paths)
        {
            var resultado = Cargar(path);
            if (resultado.IsFailure)
            {
                return Result.Failure<IEnumerable<Accidente>, DomainError>(resultado.Error);
            }
            todos.AddRange(resultado.Value);
        }

        _logger.Debug("Cargados {Total} accidentes en total.", todos.Count);
        return Result.Success<IEnumerable<Accidente>, DomainError>(todos);
    }

    private void InitStorage(string folder)
    {
        if (Directory.Exists(folder))
        {
            return;
        }
        _logger.Debug("El directorio '{Folder}' no existe. Creándolo...", folder);
        Directory.CreateDirectory(folder);
    }
}