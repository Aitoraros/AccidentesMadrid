using AccidentesMadrid.Models;

namespace AccidentesMadrid.Services;

public class AccidentesLinqAnalyzer : IAccidentesAnalyzer
{
    public int Total(IEnumerable<Accidente> datos) => datos.Count();

    public IEnumerable<(string Distrito, int Total)> Top5Distritos(IEnumerable<Accidente> datos) =>
        datos.GroupBy(a => a.Distrito)
             .Select(g => (g.Key, g.Count()))
             .OrderByDescending(x => x.Item2)
             .Take(5);

    public IEnumerable<(string Tipo, int Total)> PorTipoAccidente(IEnumerable<Accidente> datos) =>
        datos.GroupBy(a => a.TipoAccidente)
             .Select(g => (g.Key, g.Count()))
             .OrderByDescending(x => x.Item2);

    public IEnumerable<(string Estado, int Total)> PorEstadoMeteorologico(IEnumerable<Accidente> datos) =>
        datos.GroupBy(a => a.EstadoMeteorologico)
             .Select(g => (g.Key, g.Count()))
             .OrderByDescending(x => x.Item2);

    public IEnumerable<(Sexo Sexo, int Total)> PorSexo(IEnumerable<Accidente> datos) =>
        datos.GroupBy(a => a.Sexo)
             .Select(g => (g.Key, g.Count()));

    public IEnumerable<(string RangoEdad, int Total)> PorRangoEdad(IEnumerable<Accidente> datos) =>
        datos.GroupBy(a => a.RangoEdad)
             .Select(g => (g.Key, g.Count()))
             .OrderByDescending(x => x.Item2);

    public int PositivosAlcohol(IEnumerable<Accidente> datos) =>
        datos.Count(a => a.PositivaAlcohol);

    public int PositivosDroga(IEnumerable<Accidente> datos) =>
        datos.Count(a => a.PositivaDroga);

    public IEnumerable<(DayOfWeek Dia, int Total)> PorDiaSemana(IEnumerable<Accidente> datos) =>
        datos.GroupBy(a => a.DiaSemana)
             .Select(g => (g.Key, g.Count()));

    public IEnumerable<(int Mes, int Total)> PorMes(IEnumerable<Accidente> datos) =>
        datos.GroupBy(a => a.Mes)
             .Select(g => (g.Key, g.Count()))
             .OrderBy(x => x.Item1);

    public (int Hora, int Total) HoraConMasAccidentes(IEnumerable<Accidente> datos) =>
        datos.GroupBy(a => a.Hora.Hour)
             .Select(g => (g.Key, g.Count()))
             .OrderByDescending(x => x.Item2)
             .First();

    public IEnumerable<(string Lesividad, int Total)> LesionesMasFrecuentes(IEnumerable<Accidente> datos) =>
        datos.Where(a => a.Lesividad is not null)
             .GroupBy(a => a.Lesividad!)
             .Select(g => (g.Key, g.Count()))
             .OrderByDescending(x => x.Item2);

    public (string TipoVehiculo, int Total) TipoVehiculoMasImplicado(IEnumerable<Accidente> datos) =>
        datos.GroupBy(a => a.TipoVehiculo)
             .Select(g => (g.Key, g.Count()))
             .OrderByDescending(x => x.Item2)
             .First();

    public int AccidentesConPeatones(IEnumerable<Accidente> datos) =>
        datos.Count(a => a.TipoPersona == TipoPersona.Peaton);

    public (double PctHombre, double PctMujer) ProporcionHombreMujer(IEnumerable<Accidente> datos)
    {
        var total = datos.Count(a => a.Sexo != Sexo.Desconocido);
        if (total == 0) return (0, 0);
        var hombres = datos.Count(a => a.Sexo == Sexo.Hombre);
        var mujeres = datos.Count(a => a.Sexo == Sexo.Mujer);
        return (hombres * 100.0 / total, mujeres * 100.0 / total);
    }

    public IEnumerable<(string Distrito, int Total)> Top5DistritosPeatones(IEnumerable<Accidente> datos) =>
        datos.Where(a => a.TipoPersona == TipoPersona.Peaton)
             .GroupBy(a => a.Distrito)
             .Select(g => (g.Key, g.Count()))
             .OrderByDescending(x => x.Item2)
             .Take(5);

    public (int FinDeSemana, int EntreSemana) FinDeSemanaVsEntreSemana(IEnumerable<Accidente> datos)
    {
        var lista = datos.ToList();
        return (lista.Count(a => a.EsFinDeSemana), lista.Count(a => !a.EsFinDeSemana));
    }

    public double MediaAccidentesPorDia(IEnumerable<Accidente> datos)
    {
        var lista = datos.ToList();
        var dias = lista.Select(a => a.Fecha).Distinct().Count();
        return dias == 0 ? 0 : lista.Count / (double)dias;
    }

    public int AccidentesConAlcoholYDroga(IEnumerable<Accidente> datos) =>
        datos.Count(a => a.PositivaAlcohol && a.PositivaDroga);

    public IEnumerable<(string RangoEdad, int Total)> RangosEdadVulnerablesPeatones(IEnumerable<Accidente> datos) =>
        datos.Where(a => a.TipoPersona == TipoPersona.Peaton)
             .GroupBy(a => a.RangoEdad)
             .Select(g => (g.Key, g.Count()))
             .OrderByDescending(x => x.Item2);

    public IEnumerable<(string Distrito, int Total)> Top5DistritosAlcohol(IEnumerable<Accidente> datos) =>
        datos.Where(a => a.PositivaAlcohol)
             .GroupBy(a => a.Distrito)
             .Select(g => (g.Key, g.Count()))
             .OrderByDescending(x => x.Item2)
             .Take(5);

    public IEnumerable<(int CodDistrito, int Total)> PorCodigoDistrito(IEnumerable<Accidente> datos) =>
        datos.GroupBy(a => a.CodDistrito)
             .Select(g => (g.Key, g.Count()))
             .OrderBy(x => x.Item1);

    public IEnumerable<(int Anio, int Total)> PorAnio(IEnumerable<Accidente> datos) =>
        datos.GroupBy(a => a.Anio)
             .Select(g => (g.Key, g.Count()))
             .OrderBy(x => x.Item1);

    public IEnumerable<(int Anio, int Mes, int Total)> EvolucionMensualPorAnio(IEnumerable<Accidente> datos) =>
        datos.GroupBy(a => (a.Anio, a.Mes))
             .Select(g => (g.Key.Anio, g.Key.Mes, g.Count()))
             .OrderBy(x => x.Anio).ThenBy(x => x.Mes);

    public IEnumerable<(int Anio, string Distrito, int Total)> DistritoConMasAccidentesPorAnio(IEnumerable<Accidente> datos) =>
        datos.GroupBy(a => a.Anio)
             .Select(g =>
             {
                 var top = g.GroupBy(a => a.Distrito)
                            .Select(gd => (gd.Key, gd.Count()))
                            .OrderByDescending(x => x.Item2)
                            .First();
                 return (g.Key, top.Key, top.Item2);
             })
             .OrderBy(x => x.Item1);

    public IEnumerable<(int Anio, int Total)> TendenciaAlcoholPorAnio(IEnumerable<Accidente> datos) =>
        datos.Where(a => a.PositivaAlcohol)
             .GroupBy(a => a.Anio)
             .Select(g => (g.Key, g.Count()))
             .OrderBy(x => x.Item1);

    public IEnumerable<(int Anio, int FinDeSemana, int EntreSemana)> FinDeSemanaVsEntreSemanaPorAnio(IEnumerable<Accidente> datos) =>
        datos.GroupBy(a => a.Anio)
             .Select(g => (g.Key, g.Count(a => a.EsFinDeSemana), g.Count(a => !a.EsFinDeSemana)))
             .OrderBy(x => x.Item1);

    public IEnumerable<(int Anio, int Hora, int Total)> HoraPicoPorAnio(IEnumerable<Accidente> datos) =>
        datos.GroupBy(a => a.Anio)
             .Select(g =>
             {
                 var top = g.GroupBy(a => a.Hora.Hour)
                            .Select(gh => (gh.Key, gh.Count()))
                            .OrderByDescending(x => x.Item2)
                            .First();
                 return (g.Key, top.Key, top.Item2);
             })
             .OrderBy(x => x.Item1);

    public IEnumerable<(int Anio, string Lesividad, int Total)> LesionMasFrecuentePorAnio(IEnumerable<Accidente> datos) =>
        datos.Where(a => a.Lesividad is not null)
             .GroupBy(a => a.Anio)
             .Select(g =>
             {
                 var top = g.GroupBy(a => a.Lesividad!)
                            .Select(gl => (gl.Key, gl.Count()))
                            .OrderByDescending(x => x.Item2)
                            .First();
                 return (g.Key, top.Key, top.Item2);
             })
             .OrderBy(x => x.Item1);

    public IEnumerable<(int Anio, int Total)> EvolucionPeatonesPorAnio(IEnumerable<Accidente> datos) =>
        datos.Where(a => a.TipoPersona == TipoPersona.Peaton)
             .GroupBy(a => a.Anio)
             .Select(g => (g.Key, g.Count()))
             .OrderBy(x => x.Item1);
    
    public void ImprimirResultados(IEnumerable<Accidente> datos)
    {
        var lista = datos.ToList();

        Console.WriteLine($"1. Total accidentes: {Total(lista)}");

        Console.WriteLine("2. Top 5 distritos:");
        foreach (var (d, t) in Top5Distritos(lista)) Console.WriteLine($"   {d}: {t}");

        Console.WriteLine("3. Por tipo de accidente:");
        foreach (var (tipo, t) in PorTipoAccidente(lista)) Console.WriteLine($"   {tipo}: {t}");

        Console.WriteLine("4. Por estado meteorológico:");
        foreach (var (estado, t) in PorEstadoMeteorologico(lista)) Console.WriteLine($"   {estado}: {t}");

        Console.WriteLine("5. Por sexo:");
        foreach (var (sexo, t) in PorSexo(lista)) Console.WriteLine($"   {sexo}: {t}");

        Console.WriteLine("6. Por rango de edad:");
        foreach (var (rango, t) in PorRangoEdad(lista)) Console.WriteLine($"   {rango}: {t}");

        Console.WriteLine($"7. Positivos en alcohol: {PositivosAlcohol(lista)}");
        Console.WriteLine($"8. Positivos en drogas: {PositivosDroga(lista)}");

        Console.WriteLine("9. Por día de la semana:");
        foreach (var (dia, t) in PorDiaSemana(lista)) Console.WriteLine($"   {dia}: {t}");

        Console.WriteLine("10. Por mes:");
        foreach (var (mes, t) in PorMes(lista)) Console.WriteLine($"   {mes}: {t}");

        var horaTop = HoraConMasAccidentes(lista);
        Console.WriteLine($"11. Hora con más accidentes: {horaTop.Hora}h ({horaTop.Total})");

        Console.WriteLine("12. Lesiones más frecuentes:");
        foreach (var (lesividad, t) in LesionesMasFrecuentes(lista)) Console.WriteLine($"   {lesividad}: {t}");

        var vehiculoTop = TipoVehiculoMasImplicado(lista);
        Console.WriteLine($"13. Tipo de vehículo más implicado: {vehiculoTop.TipoVehiculo} ({vehiculoTop.Total})");

        Console.WriteLine($"14. Accidentes con peatones: {AccidentesConPeatones(lista)}");

        var prop = ProporcionHombreMujer(lista);
        Console.WriteLine($"15. Proporción hombre/mujer: {prop.PctHombre:F1}% / {prop.PctMujer:F1}%");

        Console.WriteLine("16. Top 5 distritos con más peatones:");
        foreach (var (d, t) in Top5DistritosPeatones(lista)) Console.WriteLine($"   {d}: {t}");

        var finde = FinDeSemanaVsEntreSemana(lista);
        Console.WriteLine($"17. Fin de semana: {finde.FinDeSemana} | Entre semana: {finde.EntreSemana}");

        Console.WriteLine($"18. Media accidentes/día: {MediaAccidentesPorDia(lista):F2}");

        Console.WriteLine($"19. Accidentes con alcohol y droga: {AccidentesConAlcoholYDroga(lista)}");

        Console.WriteLine("20. Rangos de edad vulnerables (peatones):");
        foreach (var (rango, t) in RangosEdadVulnerablesPeatones(lista)) Console.WriteLine($"   {rango}: {t}");

        Console.WriteLine("21. Top 5 distritos con más positivos en alcohol:");
        foreach (var (d, t) in Top5DistritosAlcohol(lista)) Console.WriteLine($"   {d}: {t}");

        Console.WriteLine("22. Por código de distrito:");
        foreach (var (cod, t) in PorCodigoDistrito(lista)) Console.WriteLine($"   Distrito {cod}: {t}");

        Console.WriteLine("23. Por año:");
        foreach (var (anio, t) in PorAnio(lista)) Console.WriteLine($"   {anio}: {t}");

        Console.WriteLine("24. Evolución mensual por año:");
        foreach (var (anio, mes, t) in EvolucionMensualPorAnio(lista)) Console.WriteLine($"   {anio}-{mes}: {t}");

        Console.WriteLine("25. Distrito con más accidentes por año:");
        foreach (var (anio, d, t) in DistritoConMasAccidentesPorAnio(lista)) Console.WriteLine($"   {anio}: {d} ({t})");

        Console.WriteLine("26. Tendencia de alcohol por año:");
        foreach (var (anio, t) in TendenciaAlcoholPorAnio(lista)) Console.WriteLine($"   {anio}: {t}");

        Console.WriteLine("27. Fin de semana vs entre semana por año:");
        foreach (var (anio, fs, es) in FinDeSemanaVsEntreSemanaPorAnio(lista))
            Console.WriteLine($"   {anio}: finde={fs}, entresemana={es}");

        Console.WriteLine("28. Hora pico por año:");
        foreach (var (anio, hora, t) in HoraPicoPorAnio(lista)) Console.WriteLine($"   {anio}: {hora}h ({t})");

        Console.WriteLine("29. Lesión más frecuente por año:");
        foreach (var (anio, lesividad, t) in LesionMasFrecuentePorAnio(lista)) Console.WriteLine($"   {anio}: {lesividad} ({t})");

        Console.WriteLine("30. Evolución de peatones por año:");
        foreach (var (anio, t) in EvolucionPeatonesPorAnio(lista)) Console.WriteLine($"   {anio}: {t}");
    }
}