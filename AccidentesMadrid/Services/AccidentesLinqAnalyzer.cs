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
}