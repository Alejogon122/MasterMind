using Mastermind;
using Xunit;

namespace MasterMind.Tests;

public sealed class EvaluadorIntentosTests
{
    [Theory]
    [InlineData(" RbGy ", "RBGY")]
    [InlineData("wcyb", "WCYB")]
    public void ValidarPatron_NormalizaEspaciosYMayusculas(string patron, string esperado)
    {
        string resultado = EvaluadorIntentos.ValidarPatron(patron, nameof(patron));

        Assert.Equal(esperado, resultado);
    }

    [Theory]
    [InlineData("")]
    [InlineData("RGB")]
    [InlineData("RBGYW")]
    public void ValidarPatron_RechazaLongitudDistintaDeCuatro(string patron)
    {
        ArgumentException exception = Assert.Throws<ArgumentException>(
            () => EvaluadorIntentos.ValidarPatron(patron, "patron"));

        Assert.Equal("patron", exception.ParamName);
    }

    [Fact]
    public void ValidarPatron_RechazaColorNoDisponible()
    {
        ArgumentException exception = Assert.Throws<ArgumentException>(
            () => EvaluadorIntentos.ValidarPatron("RGBX", "patron"));

        Assert.Equal("patron", exception.ParamName);
    }

    [Fact]
    public void ValidarPatron_RechazaValorNulo()
    {
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(
            () => EvaluadorIntentos.ValidarPatron(null!, "patron"));

        Assert.Equal("patron", exception.ParamName);
    }

    [Fact]
    public void EvaluarIntento_DevuelveCoincidenciasExactasYDesplazadas()
    {
        ResultadoIntento resultado = EvaluadorIntentos.EvaluarIntento("RBGY", "RYBG");

        Assert.Equal("R", resultado.BienPosicionado);
        Assert.Equal("YBG", resultado.ColorCorrecto);
    }

    [Fact]
    public void EvaluarIntento_NoCuentaColoresRepetidosMasDeUnaVez()
    {
        ResultadoIntento resultado = EvaluadorIntentos.EvaluarIntento("RRBG", "RRRB");

        Assert.Equal("RR", resultado.BienPosicionado);
        Assert.Equal("B", resultado.ColorCorrecto);
    }

    [Fact]
    public void EvaluarIntento_DevuelveCadenasVaciasCuandoNoHayCoincidencias()
    {
        ResultadoIntento resultado = EvaluadorIntentos.EvaluarIntento("RBGY", "WWCC");

        Assert.Equal(string.Empty, resultado.BienPosicionado);
        Assert.Equal(string.Empty, resultado.ColorCorrecto);
    }

    [Fact]
    public void EvaluarIntento_DevuelveTodoElPatronCuandoEsCorrecto()
    {
        ResultadoIntento resultado = EvaluadorIntentos.EvaluarIntento("WCRB", "WCRB");

        Assert.Equal("WCRB", resultado.BienPosicionado);
        Assert.Equal(string.Empty, resultado.ColorCorrecto);
    }

    [Fact]
    public void EvaluarIntento_ConCheatCodeRevelaUnaPosicionIncorrectaDelIntento()
    {
        ResultadoIntento resultado = EvaluadorIntentos.EvaluarIntento(
            "RBGY",
            "WWWW",
            "THISISACHEATCODE");

        Assert.False(resultado.SinSuerte);
        Assert.NotNull(resultado.PosicionRevelada);
        Assert.NotNull(resultado.ColorRevelado);
        Assert.Equal(
            "RBGY"[resultado.PosicionRevelada!.Value - 1],
            resultado.ColorRevelado!.Value);
    }

    [Fact]
    public void EvaluarIntento_ConCheatCodeDevuelveSinSuerteSiLaPosicionYaEstabaAcertada()
    {
        ResultadoIntento resultado = EvaluadorIntentos.EvaluarIntento(
            "RBGY",
            "RBGY",
            "THISISACHEATCODE");

        Assert.True(resultado.SinSuerte);
        Assert.Null(resultado.PosicionRevelada);
        Assert.Null(resultado.ColorRevelado);
    }

    [Theory]
    [InlineData(null, "RBGY", "patronSecreto")]
    [InlineData("RBGY", null, "intentoJugador")]
    public void EvaluarIntento_RechazaPatronesNulos(
        string? patronSecreto,
        string? intentoJugador,
        string parametroEsperado)
    {
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(
            () => EvaluadorIntentos.EvaluarIntento(patronSecreto!, intentoJugador!));

        Assert.Equal(parametroEsperado, exception.ParamName);
    }
}
