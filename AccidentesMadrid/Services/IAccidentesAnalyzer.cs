using AccidentesMadrid.Models;

namespace AccidentesMadrid.Services;

public interface IAccidentesAnalyzer
{
    int Total(IEnumerable<Accidente> datos);
    IEnumerable<(string Distrito, int Total)> Top5Distritos(IEnumerable<Accidente> datos);
    IEnumerable<(string Tipo, int Total)> PorTipoAccidente(IEnumerable<Accidente> datos);
    IEnumerable<(string Estado, int Total)> PorEstadoMeteorologico(IEnumerable<Accidente> datos);
    IEnumerable<(Sexo Sexo, int Total)> PorSexo(IEnumerable<Accidente> datos);
    IEnumerable<(string RangoEdad, int Total)> PorRangoEdad(IEnumerable<Accidente> datos);
    int PositivosAlcohol(IEnumerable<Accidente> datos);
    int PositivosDroga(IEnumerable<Accidente> datos);
    IEnumerable<(DayOfWeek Dia, int Total)> PorDiaSemana(IEnumerable<Accidente> datos);
    IEnumerable<(int Mes, int Total)> PorMes(IEnumerable<Accidente> datos);
    (int Hora, int Total) HoraConMasAccidentes(IEnumerable<Accidente> datos);
    IEnumerable<(string Lesividad, int Total)> LesionesMasFrecuentes(IEnumerable<Accidente> datos);
    (string TipoVehiculo, int Total) TipoVehiculoMasImplicado(IEnumerable<Accidente> datos);
    int AccidentesConPeatones(IEnumerable<Accidente> datos);
    (double PctHombre, double PctMujer) ProporcionHombreMujer(IEnumerable<Accidente> datos);
    IEnumerable<(string Distrito, int Total)> Top5DistritosPeatones(IEnumerable<Accidente> datos);
    (int FinDeSemana, int EntreSemana) FinDeSemanaVsEntreSemana(IEnumerable<Accidente> datos);
    double MediaAccidentesPorDia(IEnumerable<Accidente> datos);
    int AccidentesConAlcoholYDroga(IEnumerable<Accidente> datos);
    IEnumerable<(string RangoEdad, int Total)> RangosEdadVulnerablesPeatones(IEnumerable<Accidente> datos);
    IEnumerable<(string Distrito, int Total)> Top5DistritosAlcohol(IEnumerable<Accidente> datos);
    IEnumerable<(int CodDistrito, int Total)> PorCodigoDistrito(IEnumerable<Accidente> datos);
    IEnumerable<(int Anio, int Total)> PorAnio(IEnumerable<Accidente> datos);
    IEnumerable<(int Anio, int Mes, int Total)> EvolucionMensualPorAnio(IEnumerable<Accidente> datos);
    IEnumerable<(int Anio, string Distrito, int Total)> DistritoConMasAccidentesPorAnio(IEnumerable<Accidente> datos);
    IEnumerable<(int Anio, int Total)> TendenciaAlcoholPorAnio(IEnumerable<Accidente> datos);
    IEnumerable<(int Anio, int FinDeSemana, int EntreSemana)> FinDeSemanaVsEntreSemanaPorAnio(IEnumerable<Accidente> datos);
    IEnumerable<(int Anio, int Hora, int Total)> HoraPicoPorAnio(IEnumerable<Accidente> datos);
    IEnumerable<(int Anio, string Lesividad, int Total)> LesionMasFrecuentePorAnio(IEnumerable<Accidente> datos);
    IEnumerable<(int Anio, int Total)> EvolucionPeatonesPorAnio(IEnumerable<Accidente> datos);
}