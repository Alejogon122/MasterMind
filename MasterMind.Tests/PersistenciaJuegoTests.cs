using Mastermind;
using Xunit;

namespace MasterMind.Tests;

public sealed class PersistenciaJuegoTests
{
    [Fact]
    public void ControladorCheatCode_ConsumeUnaVezYPersisteLosUsos()
    {
        string directorio = CrearDirectorioTemporal();
        try
        {
            ControladorCheatCode controlador = new(directorio);

            Assert.Equal(10, controlador.UsosRestantes);
            Assert.False(controlador.ActivarAlInicio("incorrecto"));
            Assert.True(controlador.ActivarAlInicio(ControladorCheatCode.Codigo));
            Assert.Equal(9, controlador.UsosRestantes);
            Assert.False(controlador.ActivarAlInicio(ControladorCheatCode.Codigo));

            ControladorCheatCode recargado = new(directorio);
            Assert.Equal(9, recargado.UsosRestantes);
        }
        finally
        {
            Directory.Delete(directorio, recursive: true);
        }
    }

    [Fact]
    public void ControladorCheatCode_CheatRepetido_NoConsumeOtroUsoEnLaPartida()
    {
        string directorio = CrearDirectorioTemporal();
        try
        {
            ControladorCheatCode controlador = new(directorio);

            Assert.True(controlador.ActivarAlInicio(ControladorCheatCode.Codigo));
            Assert.Equal(9, controlador.UsosRestantes);
            Assert.False(controlador.ActivarAlInicio(ControladorCheatCode.Codigo));
            Assert.Equal(9, controlador.UsosRestantes);
        }
        finally
        {
            Directory.Delete(directorio, recursive: true);
        }
    }

    [Fact]
    public void ControladorCheatCode_NoPermiteUsosCuandoElContadorLlegaACero()
    {
        string directorio = CrearDirectorioTemporal();
        try
        {
            File.WriteAllText(
                Path.Combine(directorio, "cheatcode-uses.json"),
                """{"UsosRestantes":0}""");
            ControladorCheatCode controlador = new(directorio);

            Assert.False(controlador.ActivarAlInicio(ControladorCheatCode.Codigo));
            Assert.Equal(0, controlador.UsosRestantes);
        }
        finally
        {
            Directory.Delete(directorio, recursive: true);
        }
    }

    [Fact]
    public void RegistroIntentos_GuardaLaLineaConFormatoYFechaRequeridos()
    {
        string directorio = CrearDirectorioTemporal();
        try
        {
            ResultadoIntento resultado = new("R", "BG");

            string linea = RegistroIntentos.Registrar(directorio, "RYBG", resultado);

            Assert.Matches(
                @"^En la fecha \d{2}-\d{2}-\d{4} \d{2}:\d{2}:\d{2} se evaluó el intento RYBG\. Resultado: 1 bien posicionados – 2 con color correcto\.$",
                linea);
            Assert.DoesNotContain("RBGY", linea);
            Assert.Equal(linea + Environment.NewLine, File.ReadAllText(Path.Combine(directorio, "intentos.log")));
        }
        finally
        {
            Directory.Delete(directorio, recursive: true);
        }
    }

    [Fact]
    public void RegistroIntentos_EliminaElPatronSecretoDeRegistrosAnteriores()
    {
        string directorio = CrearDirectorioTemporal();
        try
        {
            string rutaRegistro = Path.Combine(directorio, "intentos.log");
            File.WriteAllText(
                rutaRegistro,
                "En la fecha 15-10-2026 23:07:18 se evaluó el intento RBGY contra el patrón GRCR. Resultado: 0 bien posicionados – 2 con color correcto." +
                Environment.NewLine);

            RegistroIntentos.ActualizarRegistrosAnteriores(directorio);

            string contenido = File.ReadAllText(rutaRegistro);
            Assert.DoesNotContain("GRCR", contenido);
            Assert.Equal(
                "En la fecha 15-10-2026 23:07:18 se evaluó el intento RBGY. Resultado: 0 bien posicionados – 2 con color correcto." +
                Environment.NewLine,
                contenido);
        }
        finally
        {
            Directory.Delete(directorio, recursive: true);
        }
    }

    private static string CrearDirectorioTemporal()
    {
        string directorio = Path.Combine(Path.GetTempPath(), $"Mastermind-tests-{Guid.NewGuid():N}");
        Directory.CreateDirectory(directorio);
        return directorio;
    }
}
