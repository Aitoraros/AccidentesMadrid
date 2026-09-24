using AccidentesMadrid.Models;
using Microsoft.Data.Analysis;

namespace AccidentesMadrid.Services;

public class AccidentesDataFrameAnalyzer
{
    public DataFrame ConstruirDataFrame(IEnumerable<Accidente> datos)
    {
        var lista = datos.ToList();

        var numExpediente = new StringDataFrameColumn("num_expediente", lista.Select(a => a.NumExpediente));
        var anio = new PrimitiveDataFrameColumn<int>("anio", lista.Select(a => a.Anio));
        var mes = new PrimitiveDataFrameColumn<int>("mes", lista.Select(a => a.Mes));
        var dia = new PrimitiveDataFrameColumn<int>("dia", lista.Select(a => a.Fecha.Day));
        var diaSemana = new StringDataFrameColumn("dia_semana", lista.Select(a => a.DiaSemana.ToString()));
        var hora = new PrimitiveDataFrameColumn<int>("hora", lista.Select(a => a.Hora.Hour));
        var distrito = new StringDataFrameColumn("distrito", lista.Select(a => a.Distrito));
        var codDistrito = new PrimitiveDataFrameColumn<int>("cod_distrito", lista.Select(a => a.CodDistrito));
        var tipoAccidente = new StringDataFrameColumn("tipo_accidente", lista.Select(a => a.TipoAccidente));
        var estadoMeteo = new StringDataFrameColumn("estado_meteorologico", lista.Select(a => a.EstadoMeteorologico));
        var tipoVehiculo = new StringDataFrameColumn("tipo_vehiculo", lista.Select(a => a.TipoVehiculo));
        var tipoPersona = new StringDataFrameColumn("tipo_persona", lista.Select(a => a.TipoPersona.ToString()));
        var rangoEdad = new StringDataFrameColumn("rango_edad", lista.Select(a => a.RangoEdad));
        var sexo = new StringDataFrameColumn("sexo", lista.Select(a => a.Sexo.ToString()));
        var lesividad = new StringDataFrameColumn("lesividad", lista.Select(a => a.Lesividad ?? ""));
        var finDeSemana = new PrimitiveDataFrameColumn<bool>("es_fin_de_semana", lista.Select(a => a.EsFinDeSemana));
        var posAlcohol = new PrimitiveDataFrameColumn<bool>("positiva_alcohol", lista.Select(a => a.PositivaAlcohol));
        var posDroga = new PrimitiveDataFrameColumn<bool>("positiva_droga", lista.Select(a => a.PositivaDroga));
        var esPeaton = new PrimitiveDataFrameColumn<bool>("es_peaton", lista.Select(a => a.TipoPersona == TipoPersona.Peaton));

        return new DataFrame(numExpediente, anio, mes, dia, diaSemana, hora, distrito, codDistrito,
            tipoAccidente, estadoMeteo, tipoVehiculo, tipoPersona, rangoEdad, sexo, lesividad,
            finDeSemana, posAlcohol, posDroga, esPeaton);
    }

    private static List<(string Key, int Total)> ContarPorColumna(DataFrame df, string columna)
    {
        var conteo = new Dictionary<string, int>();

        for (long i = 0; i < df.Rows.Count; i++)
        {
            var valor = df[columna][i]?.ToString() ?? "";

            if (conteo.ContainsKey(valor))
                conteo[valor]++;
            else
                conteo[valor] = 1;
        }

        return conteo
            .Select(kv => (kv.Key, kv.Value))
            .OrderByDescending(x => x.Value)
            .ToList();
    }

    private static DataFrame FiltrarPorValor(DataFrame df, string columna, object valor)
    {
        var filas = new List<long>();

        for (long i = 0; i < df.Rows.Count; i++)
        {
            var valorFila = df[columna][i];
            if (valorFila is not null && valorFila.Equals(valor))
            {
                filas.Add(i);
            }
        }

        return df[filas.ToArray()];
    }

    // 1
    public int Total(DataFrame df) => (int)df.Rows.Count;

    // 2
    public List<(string Distrito, int Total)> Top5Distritos(DataFrame df) =>
        ContarPorColumna(df, "distrito").Take(5).ToList();

    // 3
    public List<(string Tipo, int Total)> PorTipoAccidente(DataFrame df) =>
        ContarPorColumna(df, "tipo_accidente");

    // 4
    public List<(string Estado, int Total)> PorEstadoMeteorologico(DataFrame df) =>
        ContarPorColumna(df, "estado_meteorologico");

    // 5
    public List<(string Sexo, int Total)> PorSexo(DataFrame df) =>
        ContarPorColumna(df, "sexo");

    // 6
    public List<(string RangoEdad, int Total)> PorRangoEdad(DataFrame df) =>
        ContarPorColumna(df, "rango_edad");

    // 7
    public int PositivosAlcohol(DataFrame df) => (int)FiltrarPorValor(df, "positiva_alcohol", true).Rows.Count;

    // 8
    public int PositivosDroga(DataFrame df) => (int)FiltrarPorValor(df, "positiva_droga", true).Rows.Count;

    // 9
    public List<(string Dia, int Total)> PorDiaSemana(DataFrame df) =>
        ContarPorColumna(df, "dia_semana");

    // 10
    public List<(string Mes, int Total)> PorMes(DataFrame df) =>
        ContarPorColumna(df, "mes").OrderBy(x => int.Parse(x.Key)).ToList();

    // 11
    public (string Hora, int Total) HoraConMasAccidentes(DataFrame df) =>
        ContarPorColumna(df, "hora").First();

    // 12
    public List<(string Lesividad, int Total)> LesionesMasFrecuentes(DataFrame df) =>
        ContarPorColumna(df, "lesividad").Where(x => x.Key != "").ToList();

    // 13
    public (string TipoVehiculo, int Total) TipoVehiculoMasImplicado(DataFrame df) =>
        ContarPorColumna(df, "tipo_vehiculo").First();

    // 14
    public int AccidentesConPeatones(DataFrame df) => (int)FiltrarPorValor(df, "es_peaton", true).Rows.Count;

    // 15
    public (double PctHombre, double PctMujer) ProporcionHombreMujer(DataFrame df)
    {
        var total = (int)FiltrarPorValor(df, "sexo", "Hombre").Rows.Count + (int)FiltrarPorValor(df, "sexo", "Mujer").Rows.Count;
        if (total == 0) return (0, 0);
        var hombres = (int)FiltrarPorValor(df, "sexo", "Hombre").Rows.Count;
        var mujeres = (int)FiltrarPorValor(df, "sexo", "Mujer").Rows.Count;
        return (hombres * 100.0 / total, mujeres * 100.0 / total);
    }

    // 16
    public List<(string Distrito, int Total)> Top5DistritosPeatones(DataFrame df) =>
        ContarPorColumna(FiltrarPorValor(df, "es_peaton", true), "distrito").Take(5).ToList();

    // 17
    public (int FinDeSemana, int EntreSemana) FinDeSemanaVsEntreSemana(DataFrame df) =>
        ((int)FiltrarPorValor(df, "es_fin_de_semana", true).Rows.Count,
         (int)FiltrarPorValor(df, "es_fin_de_semana", false).Rows.Count);

    // 18
    public double MediaAccidentesPorDia(DataFrame df)
    {
        var diasUnicos = new HashSet<(int Anio, int Mes, int Dia)>();

        for (long i = 0; i < df.Rows.Count; i++)
        {
            var anio = Convert.ToInt32(df["anio"][i]);
            var mes = Convert.ToInt32(df["mes"][i]);
            var dia = Convert.ToInt32(df["dia"][i]);
            diasUnicos.Add((anio, mes, dia));
        }

        return diasUnicos.Count == 0 ? 0 : df.Rows.Count / (double)diasUnicos.Count;
    }

    // 19
    public int AccidentesConAlcoholYDroga(DataFrame df)
    {
        var conAlcohol = FiltrarPorValor(df, "positiva_alcohol", true);
        return (int)FiltrarPorValor(conAlcohol, "positiva_droga", true).Rows.Count;
    }

    // 20
    public List<(string RangoEdad, int Total)> RangosEdadVulnerablesPeatones(DataFrame df) =>
        ContarPorColumna(FiltrarPorValor(df, "es_peaton", true), "rango_edad");

    // 21
    public List<(string Distrito, int Total)> Top5DistritosAlcohol(DataFrame df) =>
        ContarPorColumna(FiltrarPorValor(df, "positiva_alcohol", true), "distrito").Take(5).ToList();

    // 22
    public List<(string CodDistrito, int Total)> PorCodigoDistrito(DataFrame df) =>
        ContarPorColumna(df, "cod_distrito").OrderBy(x => int.Parse(x.Key)).ToList();

    // 23
    public List<(string Anio, int Total)> PorAnio(DataFrame df) =>
        ContarPorColumna(df, "anio").OrderBy(x => x.Key).ToList();

    private IEnumerable<int> AniosDisponibles(DataFrame df) =>
        ContarPorColumna(df, "anio").Select(x => int.Parse(x.Key)).OrderBy(x => x);

    // 24
    public List<(int Anio, string Mes, int Total)> EvolucionMensualPorAnio(DataFrame df)
    {
        var resultado = new List<(int, string, int)>();
        foreach (var anio in AniosDisponibles(df))
        {
            var dfAnio = FiltrarPorValor(df, "anio", anio);
            foreach (var (mes, total) in ContarPorColumna(dfAnio, "mes").OrderBy(x => int.Parse(x.Key)))
                resultado.Add((anio, mes, total));
        }
        return resultado;
    }

    // 25
    public List<(int Anio, string Distrito, int Total)> DistritoConMasAccidentesPorAnio(DataFrame df) =>
        AniosDisponibles(df)
            .Select(anio => (anio, ContarPorColumna(FiltrarPorValor(df, "anio", anio), "distrito").First()))
            .Select(x => (x.anio, x.Item2.Key, x.Item2.Total))
            .ToList();

    // 26
    public List<(int Anio, int Total)> TendenciaAlcoholPorAnio(DataFrame df) =>
        AniosDisponibles(df)
            .Select(anio => (anio, (int)FiltrarPorValor(FiltrarPorValor(df, "anio", anio), "positiva_alcohol", true).Rows.Count))
            .ToList();

    // 27
    public List<(int Anio, int FinDeSemana, int EntreSemana)> FinDeSemanaVsEntreSemanaPorAnio(DataFrame df) =>
        AniosDisponibles(df)
            .Select(anio =>
            {
                var dfAnio = FiltrarPorValor(df, "anio", anio);
                return (anio,
                    (int)FiltrarPorValor(dfAnio, "es_fin_de_semana", true).Rows.Count,
                    (int)FiltrarPorValor(dfAnio, "es_fin_de_semana", false).Rows.Count);
            })
            .ToList();

    // 28
    public List<(int Anio, string Hora, int Total)> HoraPicoPorAnio(DataFrame df) =>
        AniosDisponibles(df)
            .Select(anio => (anio, ContarPorColumna(FiltrarPorValor(df, "anio", anio), "hora").First()))
            .Select(x => (x.anio, x.Item2.Key, x.Item2.Total))
            .ToList();

    // 29
    public List<(int Anio, string Lesividad, int Total)> LesionMasFrecuentePorAnio(DataFrame df) =>
        AniosDisponibles(df)
            .Select(anio =>
            {
                var top = ContarPorColumna(FiltrarPorValor(df, "anio", anio), "lesividad")
                          .Where(x => x.Key != "").FirstOrDefault();
                return (anio, top.Key ?? "", top.Total);
            })
            .ToList();

    // 30
    public List<(int Anio, int Total)> EvolucionPeatonesPorAnio(DataFrame df) =>
        AniosDisponibles(df)
            .Select(anio => (anio, (int)FiltrarPorValor(FiltrarPorValor(df, "anio", anio), "es_peaton", true).Rows.Count))
            .ToList();

    public void ImprimirResultados(DataFrame df)
{
    Console.WriteLine($"1. Total accidentes: {Total(df)}");

    Console.WriteLine("2. Top 5 distritos:");
    foreach (var (d, t) in Top5Distritos(df)) Console.WriteLine($"   {d}: {t}");

    Console.WriteLine("3. Por tipo de accidente:");
    foreach (var (tipo, t) in PorTipoAccidente(df)) Console.WriteLine($"   {tipo}: {t}");

    Console.WriteLine("4. Por estado meteorológico:");
    foreach (var (estado, t) in PorEstadoMeteorologico(df)) Console.WriteLine($"   {estado}: {t}");

    Console.WriteLine("5. Por sexo:");
    foreach (var (sexo, t) in PorSexo(df)) Console.WriteLine($"   {sexo}: {t}");

    Console.WriteLine("6. Por rango de edad:");
    foreach (var (rango, t) in PorRangoEdad(df)) Console.WriteLine($"   {rango}: {t}");

    Console.WriteLine($"7. Positivos en alcohol: {PositivosAlcohol(df)}");
    Console.WriteLine($"8. Positivos en drogas: {PositivosDroga(df)}");

    Console.WriteLine("9. Por día de la semana:");
    foreach (var (dia, t) in PorDiaSemana(df)) Console.WriteLine($"   {dia}: {t}");

    Console.WriteLine("10. Por mes:");
    foreach (var (mes, t) in PorMes(df)) Console.WriteLine($"   {mes}: {t}");

    var horaTop = HoraConMasAccidentes(df);
    Console.WriteLine($"11. Hora con más accidentes: {horaTop.Hora}h ({horaTop.Total})");

    Console.WriteLine("12. Lesiones más frecuentes:");
    foreach (var (lesividad, t) in LesionesMasFrecuentes(df)) Console.WriteLine($"   {lesividad}: {t}");

    var vehiculoTop = TipoVehiculoMasImplicado(df);
    Console.WriteLine($"13. Tipo de vehículo más implicado: {vehiculoTop.TipoVehiculo} ({vehiculoTop.Total})");

    Console.WriteLine($"14. Accidentes con peatones: {AccidentesConPeatones(df)}");

    var prop = ProporcionHombreMujer(df);
    Console.WriteLine($"15. Proporción hombre/mujer: {prop.PctHombre:F1}% / {prop.PctMujer:F1}%");

    Console.WriteLine("16. Top 5 distritos con más peatones:");
    foreach (var (d, t) in Top5DistritosPeatones(df)) Console.WriteLine($"   {d}: {t}");

    var finde = FinDeSemanaVsEntreSemana(df);
    Console.WriteLine($"17. Fin de semana: {finde.FinDeSemana} | Entre semana: {finde.EntreSemana}");

    Console.WriteLine($"18. Media accidentes/día: {MediaAccidentesPorDia(df):F2}");

    Console.WriteLine($"19. Accidentes con alcohol y droga: {AccidentesConAlcoholYDroga(df)}");

    Console.WriteLine("20. Rangos de edad vulnerables (peatones):");
    foreach (var (rango, t) in RangosEdadVulnerablesPeatones(df)) Console.WriteLine($"   {rango}: {t}");

    Console.WriteLine("21. Top 5 distritos con más positivos en alcohol:");
    foreach (var (d, t) in Top5DistritosAlcohol(df)) Console.WriteLine($"   {d}: {t}");

    Console.WriteLine("22. Por código de distrito:");
    foreach (var (cod, t) in PorCodigoDistrito(df)) Console.WriteLine($"   Distrito {cod}: {t}");

    Console.WriteLine("23. Por año:");
    foreach (var (anio, t) in PorAnio(df)) Console.WriteLine($"   {anio}: {t}");

    Console.WriteLine("24. Evolución mensual por año:");
    foreach (var (anio, mes, t) in EvolucionMensualPorAnio(df)) Console.WriteLine($"   {anio}-{mes}: {t}");

    Console.WriteLine("25. Distrito con más accidentes por año:");
    foreach (var (anio, d, t) in DistritoConMasAccidentesPorAnio(df)) Console.WriteLine($"   {anio}: {d} ({t})");

    Console.WriteLine("26. Tendencia de alcohol por año:");
    foreach (var (anio, t) in TendenciaAlcoholPorAnio(df)) Console.WriteLine($"   {anio}: {t}");

    Console.WriteLine("27. Fin de semana vs entre semana por año:");
    foreach (var (anio, fs, es) in FinDeSemanaVsEntreSemanaPorAnio(df))
        Console.WriteLine($"   {anio}: finde={fs}, entresemana={es}");

    Console.WriteLine("28. Hora pico por año:");
    foreach (var (anio, hora, t) in HoraPicoPorAnio(df)) Console.WriteLine($"   {anio}: {hora}h ({t})");

    Console.WriteLine("29. Lesión más frecuente por año:");
    foreach (var (anio, lesividad, t) in LesionMasFrecuentePorAnio(df)) Console.WriteLine($"   {anio}: {lesividad} ({t})");

    Console.WriteLine("30. Evolución de peatones por año:");
    foreach (var (anio, t) in EvolucionPeatonesPorAnio(df)) Console.WriteLine($"   {anio}: {t}");
}
}