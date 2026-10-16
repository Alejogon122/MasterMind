namespace Mastermind;

public sealed record ResultadoIntento(
    string BienPosicionado,
    string ColorCorrecto,
    int? PosicionRevelada = null,
    char? ColorRevelado = null,
    bool SinSuerte = false);

public static class EvaluadorIntentos
{
    private const int LongitudPatron = 4;
    private const string ColoresDisponibles = "RBGYWC";
    private const string CodigoCheat = "THISISACHEATCODE";

    public static ResultadoIntento EvaluarIntento(
        string patronSecreto,
        string intentoJugador,
        string? cheatCode = null)
    {
        string secretoValidado = ValidarPatron(patronSecreto, nameof(patronSecreto));
        string intentoValidado = ValidarPatron(intentoJugador, nameof(intentoJugador));
        char[] bienPosicionado = new char[LongitudPatron];
        bool[] secretoContado = new bool[LongitudPatron];
        bool[] intentoContado = new bool[LongitudPatron];

        for (int indice = 0; indice < LongitudPatron; indice++)
        {
            if (secretoValidado[indice] == intentoValidado[indice])
            {
                bienPosicionado[indice] = intentoValidado[indice];
                secretoContado[indice] = true;
                intentoContado[indice] = true;
            }
        }

        List<char> colorCorrecto = [];
        for (int indiceIntento = 0; indiceIntento < LongitudPatron; indiceIntento++)
        {
            if (intentoContado[indiceIntento])
            {
                continue;
            }

            for (int indiceSecreto = 0; indiceSecreto < LongitudPatron; indiceSecreto++)
            {
                if (!secretoContado[indiceSecreto]
                    && intentoValidado[indiceIntento] == secretoValidado[indiceSecreto])
                {
                    colorCorrecto.Add(intentoValidado[indiceIntento]);
                    secretoContado[indiceSecreto] = true;
                    intentoContado[indiceIntento] = true;
                    break;
                }
            }
        }

        int? posicionRevelada = null;
        char? colorRevelado = null;
        bool sinSuerte = false;
        if (string.Equals(cheatCode?.Trim(), CodigoCheat, StringComparison.Ordinal))
        {
            int indiceRevelado = Random.Shared.Next(LongitudPatron);
            if (intentoValidado[indiceRevelado] == secretoValidado[indiceRevelado])
            {
                sinSuerte = true;
            }
            else
            {
                posicionRevelada = indiceRevelado + 1;
                colorRevelado = secretoValidado[indiceRevelado];
            }
        }

        return new ResultadoIntento(
            new string(bienPosicionado.Where(color => color != '\0').ToArray()),
            new string(colorCorrecto.ToArray()),
            posicionRevelada,
            colorRevelado,
            sinSuerte);
    }

    public static string ValidarPatron(string patron, string nombreParametro)
    {
        ArgumentNullException.ThrowIfNull(patron, nombreParametro);

        string patronNormalizado = patron.Trim().ToUpperInvariant();
        if (patronNormalizado.Length != LongitudPatron)
        {
            throw new ArgumentException(
                $"Debe contener exactamente {LongitudPatron} iniciales de color.",
                nombreParametro);
        }

        foreach (char color in patronNormalizado)
        {
            if (!ColoresDisponibles.Contains(color))
            {
                throw new ArgumentException(
                    $"El color '{color}' no es válido. Usa únicamente {ColoresDisponibles}.",
                    nombreParametro);
            }
        }

        return patronNormalizado;
    }
}
