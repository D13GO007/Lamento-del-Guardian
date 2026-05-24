# Arquitectura del Sistema

## Vista General

El Lamento del Guardián sigue una **arquitectura monolítica en el cliente** y una **arquitectura de microservicios simplificada en el backend**, comunicadas por una capa REST/WebSocket.

```
┌─────────────────────────────────────┐
│         CLIENTE — Unity 3D          │
│         Aplicación monolítica       │
│                                     │
│  GameManager ←→ AuthManager         │
│       ↕              ↕              │
│  LevelManager    SaveManager        │
│       ↕              ↕              │
│  PuzzleController  ScoreManager     │
│       ↕              ↕              │
│  LeaderboardMgr  GuardianGameMgr    │
└──────────────┬──────────────────────┘
               │ UnityWebRequest
               │ API REST / WebSocket
┌──────────────▼──────────────────────┐
│         BACKEND — Supabase          │
│                                     │
│  Auth Service   │  Game Service     │
│  (JWT/bcrypt)   │  (Partidas)       │
│                 │                   │
│  Score Service  │  Online Service   │
│  (Leaderboard)  │  (Sync)          │
│                 │                   │
│        PostgreSQL Database          │
└─────────────────────────────────────┘
```

---

## Cliente — Unity 3D (Monolítico)

Toda la lógica del cliente reside en un único proyecto Unity compilado a `.exe`. Los scripts C# se comunican directamente entre sí en memoria.

### Scripts del Proyecto

| Script | Responsabilidad |
|--------|----------------|
| `GuardianGameManager.cs` | Gestión de progreso por zonas · Control del ascensor |
| `ZombieWaveManager.cs` | Contador de kills · Activación de victoria |
| `AuthManager.cs` | Login · Registro · Token JWT con Supabase |
| `RegisterManager.cs` | Formulario de registro de usuario |
| `SaveManager.cs` | Serialización del estado de la partida |
| `LevelManager.cs` | Control de las 5 zonas y sus transiciones |
| `PuzzleController.cs` | Lógica de acertijos extendida |
| `LeaderboardManager.cs` | Consulta y visualización del Top 10 global |
| `ScoreManager.cs` | Cálculo de puntaje en tiempo real |
| `GameReportManager.cs` | Reporte post-partida |
| `ReviewManager.cs` | Reseñas del jugador |
| `UIManager.cs` | HUD · Menús · Canvas |
| `AudioManager.cs` | Música ambiental · Efectos de sonido |
| `PlayerController.cs` | Movimiento 3D en primera persona |
| `VideoSettingsManager.cs` | Configuración gráfica |
| `OnlineManager.cs` | Modo online · Sincronización |

### Asset Principal — UHFPS

El kit **Ultimate Horror FPS** de ThunderWire Studio provee:
- Player Controller completo (caminar, correr, agacharse, inclinarse)
- Sistema de Save Game
- 11 puzzles prefabricados y funcionales
- Sistema de inventario estilo Resident Evil
- Sistema de narrativa con audio reactivo
- IA de enemigos con NavMesh
- Sistema de armas (pistola, hacha, cuchillo)
- Menús y HUD completos
- Sistema de ocultamiento del jugador

---

## Backend — Supabase

Plataforma BaaS (Backend as a Service) que expone PostgreSQL a través de una API REST autogenerada.

### Servicios

| Servicio | Función |
|----------|---------|
| Auth Service | Autenticación JWT · Cifrado bcrypt |
| Game Service | Gestión de partidas y progreso |
| Score Service | Puntuaciones y leaderboard global |
| Online Service | Sincronización en tiempo real |

### Flujo de Autenticación

```
1. Jugador ingresa credenciales en Unity
2. AuthManager.cs → POST /auth/v1/token
3. Supabase valida contra hash bcrypt
4. Retorna token JWT
5. JWT se adjunta en Authorization: Bearer <token>
```

### Flujo de Guardado

```
1. SaveManager serializa estado como JSON
2. PATCH /rest/v1/partidas_guardadas
3. RLS verifica perfil_id == sub del JWT
4. DB actualiza registro → HTTP 200
```

---

## Escenas del Juego

| Escena | Contenido |
|--------|-----------|
| `Escena1.unity` | Zonas 1, 2 y 3 · Edificio de 3 pisos · Ascensor funcional |
| `Escena2.unity` | Zonas 4 y 5 · Sala de preparación |
| `Raining.unity` | Zona de combate · Bosque con lluvia · Oleadas de zombies |
