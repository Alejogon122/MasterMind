# Mastermind

Juego de consola creado con C# y .NET 10. El sistema genera un patrón secreto de cuatro colores; el jugador dispone de hasta 12 intentos para adivinarlo.

## Ejecutar

Desde esta carpeta:

```powershell
dotnet run
```

Ingresa cuatro iniciales, sin espacios: `R` (rojo), `B` (negro), `G` (verde), `Y` (amarillo), `W` (blanco) y `C` (cian). Se permiten colores repetidos y las letras minúsculas también son válidas.

El patrón secreto permanece oculto hasta que se adivine o se agoten los 12 intentos. La función `EvaluadorIntentos.EvaluarIntento(patronSecreto, intentoJugador)` devuelve:

- `BienPosicionado`: colores correctos en la posición exacta.
- `ColorCorrecto`: colores presentes en el secreto, pero en otra posición.

Primero se contabilizan las coincidencias exactas; después se comparan las posiciones restantes, para evitar contar un color más veces de las disponibles en el patrón secreto.
