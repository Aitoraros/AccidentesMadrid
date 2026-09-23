using System.Globalization;
using AccidentesMadrid.Mappers;
using AccidentesMadrid.Models;
using CsvHelper;
using CsvHelper.Configuration;

namespace AccidentesMadrid.Repositories;

public class AccidentesRepository
{
    public IReadOnlyList<Accidente> Cargar(string carpetaData)
    {
        var ficheros = new[] { "2024_Accidentalidad.csv", "2025_Accidentalidad.csv", "2026_Accidentalidad.csv" };
        var resultado = new List<Accidente>();

        var config = new CsvConfiguration(CultureInfo.GetCultureInfo("es-ES"))
        {
            Delimiter = ";",
            HasHeaderRecord = true,
            MissingFieldFound = null,
            BadDataFound = null
        };

        foreach (var fichero in ficheros)
        {
            using var reader = new StreamReader(Path.Combine(carpetaData, fichero));
            using var csv = new CsvReader(reader, config);
            csv.Read();
            csv.ReadHeader();
            while (csv.Read())
            {
                var campos = csv.HeaderRecord!.ToDictionary(h => h, h => csv.GetField(h) ?? "");
                resultado.Add(campos.ToAccidente());
            }
        }
        return resultado;
    }
}