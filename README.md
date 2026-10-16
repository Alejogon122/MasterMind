# Mastermind

Juego de consola creado con C# y .NET 10. El sistema genera un patrón secreto de cuatro colores; el jugador dispone de hasta 12 intentos para adivinarlo.

## Ejecutar

Desde esta carpeta:

```powershell
dotnet run
```

## Pruebas

Desde la carpeta raíz del repositorio, ejecuta las pruebas unitarias con:

```powershell
dotnet test .\MasterMind.Tests\MasterMind.Tests.csproj
```

Ingresa cuatro iniciales, sin espacios: `R` (rojo), `B` (negro), `G` (verde), `Y` (amarillo), `W` (blanco) y `C` (cian). Se permiten colores repetidos y las letras minúsculas también son válidas.

El patrón secreto permanece oculto hasta que se adivine o se agoten los 12 intentos. La función `EvaluadorIntentos.EvaluarIntento(string patronSecreto, string intentoJugador, string? cheatCode = null)` devuelve:

- `BienPosicionado`: colores correctos en la posición exacta.
- `ColorCorrecto`: colores presentes en el secreto, pero en otra posición.

Primero se contabilizan las coincidencias exactas; después se comparan las posiciones restantes, para evitar contar un color más veces de las disponibles en el patrón secreto.

## Registro y cheat code

Cada intento válido se agrega a `intentos.log` con fecha, combinación evaluada y resultado. El patrón secreto no se incluye en el registro.

Al iniciar cada partida puedes ingresar el cheat code `THISISACHEATCODE`. Una vez activado, revela un color en una posición aleatoria al evaluar el primer intento; si esa posición ya estaba acertada en el intento, se muestra “Sin suerte”. Solo puede usarse una vez por partida.

El contador global comienza con 10 usos y se guarda en `cheatcode-uses.json` en la carpeta del proyecto. Cada activación consume un uso, incluso si la posición seleccionada ya estaba acertada. Al llegar a cero, el código queda agotado.
