using System.Globalization;
using AccidentesMadrid.Models;

namespace AccidentesMadrid.Mappers;

public static class AccidenteMapper
{
    public static Accidente ToAccidente(this IReadOnlyDictionary<string, string> campos)
    {
        return new Accidente
        {
            NumExpediente = campos["num_expediente"],
            Fecha = DateOnly.ParseExact(campos["fecha"], "dd/MM/yyyy", CultureInfo.InvariantCulture),
            Hora = TimeOnly.TryParse(campos["hora"], CultureInfo.InvariantCulture, out var h) ? h : TimeOnly.MinValue,
            Localizacion = campos["localizacion"],
            Numero = string.IsNullOrWhiteSpace(campos["numero"]) ? null : campos["numero"],
            CodDistrito = int.TryParse(campos["cod_distrito"], out var cd) ? cd : 0,
            Distrito = campos["distrito"],
            TipoAccidente = campos["tipo_accidente"],
            EstadoMeteorologico = campos["estado_meteorológico"],
            TipoVehiculo = campos["tipo_vehiculo"],
            TipoPersona = ParseTipoPersona(campos["tipo_persona"]),
            RangoEdad = campos["rango_edad"],
            Sexo = ParseSexo(campos["sexo"]),
            CodLesividad = string.IsNullOrWhiteSpace(campos["cod_lesividad"]) ? null : campos["cod_lesividad"],
            Lesividad = string.IsNullOrWhiteSpace(campos["lesividad"]) ? null : campos["lesividad"],
            CoordenadaX = ParseCoord(campos, "coordenada_x_utm"),
            CoordenadaY = ParseCoord(campos, "coordenada_y_utm"),
            PositivaAlcohol = EsS(campos, "positiva_alcohol"),
            PositivaDroga = EsS(campos, "positiva_droga")
        };
    }

    private static TipoPersona ParseTipoPersona(string valor) => valor.ToUpperInvariant() switch
    {
        "CONDUCTOR" => TipoPersona.Conductor,
        "PASAJERO" => TipoPersona.Pasajero,
        "PEATÓN" or "PEATON" => TipoPersona.Peaton,
        _ => TipoPersona.Desconocido
    };

    private static Sexo ParseSexo(string valor) => valor.ToUpperInvariant() switch
    {
        "HOMBRE" => Sexo.Hombre,
        "MUJER" => Sexo.Mujer,
        _ => Sexo.Desconocido
    };

    private static bool EsS(IReadOnlyDictionary<string, string> campos, string campo) =>
        campos.TryGetValue(campo, out var v) && v.Trim().Equals("S", StringComparison.OrdinalIgnoreCase);

    private static double? ParseCoord(IReadOnlyDictionary<string, string> campos, string campo) =>
        campos.TryGetValue(campo, out var v) && !string.IsNullOrWhiteSpace(v)
            && double.TryParse(v, NumberStyles.Any, CultureInfo.InvariantCulture, out var d)
            ? d : null;
}