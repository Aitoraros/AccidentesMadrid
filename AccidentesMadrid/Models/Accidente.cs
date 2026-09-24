namespace AccidentesMadrid.Models;

public record Accidente
{
    public required string NumExpediente { get; init; }
    public required DateOnly Fecha { get; init; }
    public required TimeOnly Hora { get; init; }
    public required string Localizacion { get; init; }
    public string? Numero { get; init; }        // puede no ser numerico = string
    public int CodDistrito { get; init; }
    public required string Distrito { get; init; }
    public required string TipoAccidente { get; init; }
    public required string EstadoMeteorologico { get; init; }
    public required string TipoVehiculo { get; init; }
    public TipoPersona TipoPersona { get; init; }
    public required string RangoEdad { get; init; }
    public Sexo Sexo { get; init; }
    public string? CodLesividad { get; init; }
    public string? Lesividad { get; init; }
    public double? CoordenadaX { get; init; }
    public double? CoordenadaY { get; init; }
    public bool PositivaAlcohol { get; init; }
    public bool PositivaDroga { get; init; }

    public int Anio => Fecha.Year;
    public int Mes => Fecha.Month;
    public DayOfWeek DiaSemana => Fecha.DayOfWeek;
    public bool EsFinDeSemana => DiaSemana is DayOfWeek.Saturday or DayOfWeek.Sunday;
}