# Requisitos del Software

Especificación basada en el estándar **IEEE 830-1998**.

---

## Requisitos Funcionales

| ID | Descripción | Script Responsable |
|----|-------------|-------------------|
| RF-001 | Almacenar credenciales cifradas y perfiles únicos de jugador | `AuthManager.cs` |
| RF-002 | Guardar estado de partida: zona (1-5), tesoros (0-5), puzles, muertes | `SaveManager.cs` |
| RF-003 | Registrar puntuaciones finales y tiempo de juego para el Top 10 global | `LeaderboardManager.cs` |
| RF-004 | Sistema de navegación en primera persona con colisiones | `PlayerController.cs` |
| RF-005 | Sistema de interacción y resolución de puzles | `PuzzleController.cs` |
| RF-006 | Gestión de inventario con 5 tesoros rastreados | `SaveManager.cs` |
| RF-007 | Permadeath Lite — reinicio de zona conservando tesoros | `GuardianGameManager.cs` |
| RF-008 | Narrativa reactiva — diálogos del Guardián por trigger | `AudioManager.cs` |
| RF-009 | Sistema de combate con pistola y enemigos | `ZombieWaveManager.cs` |
| RF-010 | Control secuencial del ascensor por zonas completadas | `GuardianGameManager.cs` |

---

## Requisitos No Funcionales

| ID | Descripción | Implementación |
|----|-------------|---------------|
| RNF-001 | Tiempo de respuesta del leaderboard ≤ 2 seg bajo carga normal | Índice B-Tree en PostgreSQL |
| RNF-002 | Seguridad: solo el jugador propietario puede modificar su partida | Row Level Security (RLS) |
| RNF-003 | Fluidez de imagen a 60 FPS en resolución 1080p | Optimización HDRP |
| RNF-004 | Tiempo de carga inicial ≤ 30 segundos | Sistema modular de escenas |
| RNF-005 | Latencia de entrada ≤ 100 milisegundos | Input System nativo de Unity |
| RNF-006 | Funcionamiento en equipos con mínimo 8GB RAM | Assets optimizados UHFPS |

---

## Restricciones de Diseño

- **Plataforma:** Windows 10/11 exclusivamente
- **Controles:** Teclado + Ratón únicamente
- **Motor:** Unity 6 con HDRP
- **Idioma:** Español en toda la interfaz y diálogos
- **Sin multijugador en tiempo real** en la versión inicial

---

## Matriz de Trazabilidad

| Requisito | Componente Unity | Tabla BD | Criterio de Aceptación |
|-----------|-----------------|----------|----------------------|
| RF-001 | `AuthManager.cs` | `perfiles` | Login exitoso con token JWT válido |
| RF-002 | `SaveManager.cs` | `partidas_guardadas` | Estado persiste entre sesiones |
| RF-003 | `LeaderboardManager.cs` | `leaderboard` | Top 10 carga en ≤ 2 seg |
| RF-007 | `GuardianGameManager.cs` | `partidas_guardadas` | Muerte reinicia posición, conserva tesoros |
| RF-010 | `GuardianGameManager.cs` | — | Ascensor bloqueado hasta completar zona |
