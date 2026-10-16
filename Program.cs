using Mastermind;

const int longitudPatron = 4;
const int maximoIntentos = 12;
const string coloresDisponibles = "RBGYWC";
string directorioProyecto = RegistroIntentos.ObtenerDirectorioProyecto();
RegistroIntentos.ActualizarRegistrosAnteriores(directorioProyecto);
ControladorCheatCode controladorCheatCode = new(directorioProyecto);

char[] patronSecreto = Enumerable.Range(0, longitudPatron)
    .Select(_ => coloresDisponibles[Random.Shared.Next(coloresDisponibles.Length)])
    .ToArray();

Console.WriteLine("Mastermind");
Console.WriteLine($"Adivina el patrón de {longitudPatron} colores.");
Console.WriteLine("Colores disponibles: R (rojo), B (negro), G (verde), Y (amarillo), W (blanco), C (cian).");
Console.WriteLine("Puedes repetir colores. Escribe las cuatro iniciales sin espacios.");
Console.Write("Cheat code opcional (presiona Enter para omitir): ");
string? codigoIngresado = Console.ReadLine();
bool cheatCodeActivo = controladorCheatCode.ActivarAlInicio(codigoIngresado);
if (cheatCodeActivo)
{
    Console.WriteLine($"Cheat code activado. Usos globales restantes: {controladorCheatCode.UsosRestantes}.");
}
else if (string.Equals(codigoIngresado?.Trim(), ControladorCheatCode.Codigo, StringComparison.Ordinal))
{
    Console.WriteLine("El cheat code ya no tiene usos disponibles. La partida continúa.");
}
else if (!string.IsNullOrWhiteSpace(codigoIngresado))
{
    Console.WriteLine("Cheat code inválido. La partida continúa.");
}

for (int numeroIntento = 1; numeroIntento <= maximoIntentos;)
{
    Console.Write($"Intento {numeroIntento}/{maximoIntentos}: ");
    string? intentoIngresado = Console.ReadLine();

    if (intentoIngresado is null)
    {
        Console.WriteLine();
        Console.WriteLine("Entrada finalizada.");
        break;
    }

    if (string.Equals(
        intentoIngresado.Trim(),
        ControladorCheatCode.Codigo,
        StringComparison.Ordinal))
    {
        Console.WriteLine(controladorCheatCode.FueUsadoEnPartida
            ? "El cheat code ya fue usado en esta partida. Continúa con tu intento."
            : "El cheat code solo se puede ingresar al inicio de la partida. Continúa con tu intento.");
        continue;
    }

    try
    {
        string intentoJugador = EvaluadorIntentos.ValidarPatron(intentoIngresado, nameof(intentoIngresado));
        ResultadoIntento resultado = EvaluadorIntentos.EvaluarIntento(
            new string(patronSecreto),
            intentoJugador,
            numeroIntento == 1 && cheatCodeActivo ? ControladorCheatCode.Codigo : null);

        string lineaRegistro = RegistroIntentos.Registrar(
            directorioProyecto,
            intentoJugador,
            resultado);
        Console.WriteLine(lineaRegistro);

        if (resultado.SinSuerte)
        {
            Console.WriteLine("Sin suerte: la posición aleatoria ya estaba acertada.");
        }
        else if (resultado.PosicionRevelada is int posicion && resultado.ColorRevelado is char color)
        {
            Console.WriteLine($"Pista: la posición {posicion} contiene el color {color}.");
        }

        Console.WriteLine(
            $"bienPosicionado = {resultado.BienPosicionado.Length}: [{resultado.BienPosicionado}]");
        Console.WriteLine(
            $"colorCorrecto = {resultado.ColorCorrecto.Length}: [{resultado.ColorCorrecto}]");

        numeroIntento++;

        if (resultado.BienPosicionado.Length == longitudPatron)
        {
            Console.WriteLine($"¡Adivinaste! El patrón secreto era: {new string(patronSecreto)}");
            break;
        }

        if (numeroIntento > maximoIntentos)
        {
            Console.WriteLine($"Se agotaron los intentos. El patrón secreto era: {new string(patronSecreto)}");
            break;
        }
    }
    catch (ArgumentException exception)
    {
        Console.WriteLine($"Intento inválido: {exception.Message}");
    }
}
