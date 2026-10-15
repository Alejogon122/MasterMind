using Mastermind;

const int longitudPatron = 4;
const int maximoIntentos = 12;
const string coloresDisponibles = "RBGYWC";

char[] patronSecreto = Enumerable.Range(0, longitudPatron)
    .Select(_ => coloresDisponibles[Random.Shared.Next(coloresDisponibles.Length)])
    .ToArray();

Console.WriteLine("Mastermind");
Console.WriteLine($"Adivina el patrón de {longitudPatron} colores.");
Console.WriteLine("Colores disponibles: R (rojo), B (negro), G (verde), Y (amarillo), W (blanco), C (cian).");
Console.WriteLine("Puedes repetir colores. Escribe las cuatro iniciales sin espacios.");

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

    try
    {
        string intentoJugador = EvaluadorIntentos.ValidarPatron(intentoIngresado, nameof(intentoIngresado));
        ResultadoIntento resultado = EvaluadorIntentos.EvaluarIntento(
            new string(patronSecreto),
            intentoJugador);

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
