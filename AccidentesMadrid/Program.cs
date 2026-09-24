using System.Diagnostics;
using AccidentesMadrid.Config;
using AccidentesMadrid.Repositories;
using AccidentesMadrid.Services;

Console.WriteLine("=== Análisis de Accidentes de Madrid ===\n");

var repository = new AccidentesRepository();
var linqAnalyzer = new AccidentesLinqAnalyzer();
var dfAnalyzer = new AccidentesDataFrameAnalyzer();

var swCarga = Stopwatch.StartNew();
var resultado = await repository.CargarTodosParaleloAsync(AppConfig.RutasCompletas);
swCarga.Stop();

if (resultado.IsFailure)
{
    Console.WriteLine($"Error al cargar los datos: {resultado.Error}");
    return;
}

var datos = resultado.Value.ToList();
Console.WriteLine($"Cargados {datos.Count} accidentes en {swCarga.ElapsedMilliseconds} ms\n");

Console.WriteLine("--- Consultas LINQ ---");
var swLinq = Stopwatch.StartNew();
linqAnalyzer.ImprimirResultados(datos);
swLinq.Stop();
Console.WriteLine($"Tiempo LINQ: {swLinq.ElapsedMilliseconds} ms\n");

Console.WriteLine("--- Consultas DataFrame ---");
var swDfBuild = Stopwatch.StartNew();
var df = dfAnalyzer.ConstruirDataFrame(datos);
swDfBuild.Stop();

var swDf = Stopwatch.StartNew();
dfAnalyzer.ImprimirResultados(df);
swDf.Stop();
Console.WriteLine($"Tiempo construcción DataFrame: {swDfBuild.ElapsedMilliseconds} ms");
Console.WriteLine($"Tiempo consultas DataFrame: {swDf.ElapsedMilliseconds} ms\n");

Console.WriteLine("=== Comparativa de tiempos ===");
Console.WriteLine($"Carga de ficheros    : {swCarga.ElapsedMilliseconds} ms");
Console.WriteLine($"LINQ (30 consultas)  : {swLinq.ElapsedMilliseconds} ms");
Console.WriteLine($"DataFrame (build+30) : {swDfBuild.ElapsedMilliseconds + swDf.ElapsedMilliseconds} ms");