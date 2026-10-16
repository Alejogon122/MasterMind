using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Mastermind;

internal sealed class ControladorCheatCode
{
    public const string Codigo = "THISISACHEATCODE";
    private const int UsosIniciales = 10;
    private readonly string rutaContador;
    private int usosRestantes;

    public ControladorCheatCode(string directorioProyecto)
    {
        rutaContador = Path.Combine(directorioProyecto, "cheatcode-uses.json");
        if (File.Exists(rutaContador))
        {
            EstadoUsos estado = JsonSerializer.Deserialize<EstadoUsos>(
                File.ReadAllText(rutaContador))
                ?? throw new InvalidDataException("El archivo del contador del cheat code está vacío.");
            if (estado.UsosRestantes < 0 || estado.UsosRestantes > UsosIniciales)
            {
                throw new InvalidDataException("El contador del cheat code contiene un valor inválido.");
            }

            usosRestantes = estado.UsosRestantes;
        }
        else
        {
            usosRestantes = UsosIniciales;
            GuardarContador(usosRestantes);
        }
    }

    public bool FueUsadoEnPartida { get; private set; }

    public int UsosRestantes => usosRestantes;

    public bool ActivarAlInicio(string? codigoIngresado)
    {
        if (!string.Equals(codigoIngresado?.Trim(), Codigo, StringComparison.Ordinal))
        {
            return false;
        }

        if (FueUsadoEnPartida || usosRestantes == 0)
        {
            return false;
        }

        GuardarContador(usosRestantes - 1);
        usosRestantes--;
        FueUsadoEnPartida = true;
        return true;
    }

    private void GuardarContador(int usos)
    {
        string contenido = JsonSerializer.Serialize(
            new EstadoUsos(usos),
            new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(rutaContador, contenido, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
    }

    private sealed record EstadoUsos(int UsosRestantes);
}

internal static class RegistroIntentos
{
    private static readonly Regex PatronRegistroAnterior = new(
        @"(En la fecha \d{2}-\d{2}-\d{4} \d{2}:\d{2}:\d{2} se evaluó el intento [A-Z]{4}) contra el patrón [A-Z]{4}\.",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public static string ObtenerDirectorioProyecto()
    {
        DirectoryInfo? directorio = new(AppContext.BaseDirectory);
        while (directorio is not null)
        {
            if (File.Exists(Path.Combine(directorio.FullName, "Mastermind.csproj")))
            {
                return directorio.FullName;
            }

            directorio = directorio.Parent;
        }

        return Directory.GetCurrentDirectory();
    }

    public static void ActualizarRegistrosAnteriores(string directorioProyecto)
    {
        string rutaRegistro = Path.Combine(directorioProyecto, "intentos.log");
        if (!File.Exists(rutaRegistro))
        {
            return;
        }

        string contenido = File.ReadAllText(rutaRegistro);
        string contenidoActualizado = PatronRegistroAnterior.Replace(contenido, "$1.");
        if (!string.Equals(contenido, contenidoActualizado, StringComparison.Ordinal))
        {
            File.WriteAllText(rutaRegistro, contenidoActualizado, new UTF8Encoding(false));
        }
    }

    public static string Registrar(
        string directorioProyecto,
        string intento,
        ResultadoIntento resultado)
    {
        string fecha = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
        string linea =
            $"En la fecha {fecha} se evaluó el intento {intento}. " +
            $"Resultado: {resultado.BienPosicionado.Length} bien posicionados – " +
            $"{resultado.ColorCorrecto.Length} con color correcto.";
        string rutaRegistro = Path.Combine(directorioProyecto, "intentos.log");
        File.AppendAllText(rutaRegistro, linea + Environment.NewLine, new UTF8Encoding(false));
        return linea;
    }
}
