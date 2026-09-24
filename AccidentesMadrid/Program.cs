using System.Diagnostics;
using AccidentesMadrid.Config;
using AccidentesMadrid.Repositories;
using AccidentesMadrid.Services;

Console.WriteLine("=== Análisis de Accidentes de Madrid ===\n");

var repository = new AccidentesRepository();
var analyzer = new AccidentesLinqAnalyzer();

// --- Carga de datos ---
var swCarga = Stopwatch.StartNew();
var resultado = repository.CargarTodos(AppConfig.RutasCompletas);
swCarga.Stop();

if (resultado.IsFailure)
{
    Console.WriteLine($"Error al cargar los datos: {resultado.Error}");
    return;
}

var datos = resultado.Value.ToList();
Console.WriteLine($"Cargados {datos.Count} accidentes en {swCarga.ElapsedMilliseconds} ms\n");

// --- Consultas LINQ ---
var swLinq = Stopwatch.StartNew();

var total = analyzer.Total(datos);
var top5Distritos = analyzer.Top5Distritos(datos).ToList();
var porTipo = analyzer.PorTipoAccidente(datos).ToList();
var porEstadoMeteo = analyzer.PorEstadoMeteorologico(datos).ToList();
var porSexo = analyzer.PorSexo(datos).ToList();
var porRangoEdad = analyzer.PorRangoEdad(datos).ToList();
var positivosAlcohol = analyzer.PositivosAlcohol(datos);
var positivosDroga = analyzer.PositivosDroga(datos);
var porDiaSemana = analyzer.PorDiaSemana(datos).ToList();
var porMes = analyzer.PorMes(datos).ToList();
var horaConMasAccidentes = analyzer.HoraConMasAccidentes(datos);
var lesionesMasFrecuentes = analyzer.LesionesMasFrecuentes(datos).ToList();
var tipoVehiculoMasImplicado = analyzer.TipoVehiculoMasImplicado(datos);
var accidentesConPeatones = analyzer.AccidentesConPeatones(datos);
var proporcionHM = analyzer.ProporcionHombreMujer(datos);
var top5DistritosPeatones = analyzer.Top5DistritosPeatones(datos).ToList();
var finDeSemanaVsEntreSemana = analyzer.FinDeSemanaVsEntreSemana(datos);
var mediaAccidentesPorDia = analyzer.MediaAccidentesPorDia(datos);
var accidentesConAlcoholYDroga = analyzer.AccidentesConAlcoholYDroga(datos);
var rangosEdadVulnerables = analyzer.RangosEdadVulnerablesPeatones(datos).ToList();
var top5DistritosAlcohol = analyzer.Top5DistritosAlcohol(datos).ToList();
var porCodigoDistrito = analyzer.PorCodigoDistrito(datos).ToList();
var porAnio = analyzer.PorAnio(datos).ToList();
var evolucionMensualPorAnio = analyzer.EvolucionMensualPorAnio(datos).ToList();
var distritoConMasAccidentesPorAnio = analyzer.DistritoConMasAccidentesPorAnio(datos).ToList();
var tendenciaAlcoholPorAnio = analyzer.TendenciaAlcoholPorAnio(datos).ToList();
var finSemanaVsEntreSemanaPorAnio = analyzer.FinDeSemanaVsEntreSemanaPorAnio(datos).ToList();
var horaPicoPorAnio = analyzer.HoraPicoPorAnio(datos).ToList();
var lesionMasFrecuentePorAnio = analyzer.LesionMasFrecuentePorAnio(datos).ToList();
var evolucionPeatonesPorAnio = analyzer.EvolucionPeatonesPorAnio(datos).ToList();

swLinq.Stop();

// --- Resultados ---
Console.WriteLine($"--- Consultas LINQ ({swLinq.ElapsedMilliseconds} ms) ---\n");

Console.WriteLine($"1. Total accidentes: {total}");

Console.WriteLine("2. Top 5 distritos:");
foreach (var (distrito, count) in top5Distritos)
    Console.WriteLine($"   {distrito}: {count}");

Console.WriteLine("3. Por tipo de accidente:");
foreach (var (tipo, count) in porTipo)
    Console.WriteLine($"   {tipo}: {count}");

Console.WriteLine("4. Por estado meteorológico:");
foreach (var (estado, count) in porEstadoMeteo)
    Console.WriteLine($"   {estado}: {count}");

Console.WriteLine("5. Por sexo:");
foreach (var (sexo, count) in porSexo)
    Console.WriteLine($"   {sexo}: {count}");

Console.WriteLine("6. Por rango de edad:");
foreach (var (rango, count) in porRangoEdad)
    Console.WriteLine($"   {rango}: {count}");

Console.WriteLine($"7. Positivos en alcohol: {positivosAlcohol}");
Console.WriteLine($"8. Positivos en drogas: {positivosDroga}");

Console.WriteLine("9. Por día de la semana:");
foreach (var (dia, count) in porDiaSemana)
    Console.WriteLine($"   {dia}: {count}");

Console.WriteLine("10. Por mes:");
foreach (var (mes, count) in porMes)
    Console.WriteLine($"   {mes}: {count}");

Console.WriteLine($"11. Hora con más accidentes: {horaConMasAccidentes.Hora}h ({horaConMasAccidentes.Total} accidentes)");

Console.WriteLine("12. Lesiones más frecuentes:");
foreach (var (lesion, count) in lesionesMasFrecuentes)
    Console.WriteLine($"   {lesion}: {count}");

Console.WriteLine($"13. Tipo de vehículo más implicado: {tipoVehiculoMasImplicado}");
Console.WriteLine($"14. Accidentes con peatones: {accidentesConPeatones}");
Console.WriteLine($"15. Proporción hombre/mujer: {proporcionHM.PctHombre:F1}% / {proporcionHM.PctMujer:F1}%");

Console.WriteLine("16. Top 5 distritos con peatones implicados:");
foreach (var (distrito, count) in top5DistritosPeatones)
    Console.WriteLine($"   {distrito}: {count}");

Console.WriteLine($"17. Fin de semana: {finDeSemanaVsEntreSemana.FinDeSemana} | Entre semana: {finDeSemanaVsEntreSemana.EntreSemana}");
Console.WriteLine($"18. Media accidentes/día: {mediaAccidentesPorDia:F2}");
Console.WriteLine($"19. Accidentes con alcohol y droga: {accidentesConAlcoholYDroga}");

Console.WriteLine("20. Rangos de edad vulnerables (peatones):");
foreach (var (rango, count) in rangosEdadVulnerables)
    Console.WriteLine($"   {rango}: {count}");

Console.WriteLine("21. Top 5 distritos con positivos en alcohol:");
foreach (var (distrito, count) in top5DistritosAlcohol)
    Console.WriteLine($"   {distrito}: {count}");

Console.WriteLine("22. Por código de distrito:");
foreach (var (codigo, count) in porCodigoDistrito)
    Console.WriteLine($"   Código {codigo}: {count}");

Console.WriteLine("23. Por año:");
foreach (var (anio, count) in porAnio)
    Console.WriteLine($"   {anio}: {count}");

Console.WriteLine("24. Evolución mensual por año:");
foreach (var (anio, mes, count) in evolucionMensualPorAnio)
    Console.WriteLine($"   Año {anio} - Mes {mes}: {count}");

Console.WriteLine("25. Distrito con más accidentes por año:");
foreach (var (anio, distrito, count) in distritoConMasAccidentesPorAnio)
    Console.WriteLine($"   Año {anio}: {distrito} ({count} accidentes)");

Console.WriteLine("26. Tendencia de positivos en alcohol por año:");
foreach (var (anio, count) in tendenciaAlcoholPorAnio)
    Console.WriteLine($"   Año {anio}: {count} positivos");

Console.WriteLine("27. Fin de semana vs Entre semana por año:");
foreach (var item in finSemanaVsEntreSemanaPorAnio)
    Console.WriteLine($"   Año {item.Anio} -> Fin de semana: {item.FinDeSemana} | Entre semana: {item.EntreSemana}");

Console.WriteLine("28. Hora pico por año:");
foreach (var (anio, hora, count) in horaPicoPorAnio)
    Console.WriteLine($"   Año {anio}: {hora}h ({count} accidentes)");

Console.WriteLine("29. Lesión más frecuente por año:");
foreach (var (anio, lesion, count) in lesionMasFrecuentePorAnio)
    Console.WriteLine($"   Año {anio}: {lesion} ({count} casos)");

Console.WriteLine("30. Evolución de accidentes con peatones por año:");
foreach (var (anio, count) in evolucionPeatonesPorAnio)
    Console.WriteLine($"   Año {anio}: {count} accidentes");


Console.WriteLine($"\nTiempo total consultas LINQ: {swLinq.ElapsedMilliseconds} ms");