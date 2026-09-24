using System.Globalization;
using System.Text;
using AccidentesMadrid.Config;
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

    public Result<IEnumerable<Accidente>, string> Cargar(string path)
    {
        _logger.Debug("Cargando accidentes desde el archivo '{path}'", path);

        if (!Path.Exists(path))
        {
            _logger.Warning("El archivo '{path}' no existe.", path);
            return Result.Failure<IEnumerable<Accidente>, string>($"No se ha encontrado el archivo '{path}'.");
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

            return Result.Success<IEnumerable<Accidente>, string>(accidentes);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Ha sucedido un error al cargar los accidentes desde el archivo '{path}'.", path);
            return Result.Failure<IEnumerable<Accidente>, string>($"Formato inválido: {ex.Message}");
        }
    }

    public Result<IEnumerable<Accidente>, string> CargarTodos(IEnumerable<string> paths)
    {
        var todos = new List<Accidente>();

        foreach (var path in paths)
        {
            var resultado = Cargar(path);
            if (resultado.IsFailure)
            {
                return Result.Failure<IEnumerable<Accidente>, string>(resultado.Error);
            }
            todos.AddRange(resultado.Value);
        }

        _logger.Debug("Cargados {Total} accidentes en total.", todos.Count);
        return Result.Success<IEnumerable<Accidente>, string>(todos);
    }

    public async Task<Result<IEnumerable<Accidente>, string>> CargarTodosParaleloAsync(IEnumerable<string> paths)
    {
        var tareas = paths.Select(path => Task.Run(() => Cargar(path))).ToList();
        var resultados = await Task.WhenAll(tareas);

        var fallo = resultados.FirstOrDefault(r => r.IsFailure);
        if (fallo.IsFailure)
        {
            return Result.Failure<IEnumerable<Accidente>, string>(fallo.Error);
        }

        var todos = resultados.SelectMany(r => r.Value).ToList();
        _logger.Debug("Cargados {Total} accidentes en total (paralelo).", todos.Count);
        return Result.Success<IEnumerable<Accidente>, string>(todos);
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