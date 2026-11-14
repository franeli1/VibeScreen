# VibeScreen

Gestor de fondos de escritorio para Windows 11 (biblioteca, perfiles, varios monitores, GIF y vídeo).
Requisitos completos: [`docs/Requisitos_y_Hoja_de_Ruta.md`](docs/Requisitos_y_Hoja_de_Ruta.md).

## Arquitectura

| Proceso | Tecnología | Responsabilidad |
|---|---|---|
| `src/desktop` (renderer) | React + Vite + TS | Interfaz |
| `src/desktop/electron` (main) | Electron / Node.js | Ventana, bandeja, arranque y supervisión del backend |
| `src/backend` | C# / .NET 10 (Kestrel en 127.0.0.1) | Biblioteca, perfiles, monitores, persistencia, validación, logs |
| `src/wallpaper-host` | C# / .NET 10 (Win32) | Ventana tras los iconos, reproducción, pausa |

Electron no contiene lógica de negocio. El backend solo escucha en loopback y exige un token por sesión
(cabecera `X-Vibe-Token`) que genera Electron al arrancar.

## Requisitos de desarrollo

- Windows 11, .NET 10 SDK, Node.js 22+

## Primeros pasos

```powershell
npm install
npm run dev                                   # Vite + Electron + backend
npm run host:proto -- --image C:\ruta\foto.jpg --monitor 0
```

En el prototipo del host se pueden escribir por consola `pause`, `resume` y `exit`.
