# Diseño de Zonas

## Distribución de los 11 Puzzles

Todos los puzzles son prefabs nativos del **UHFPS (ThunderWire Studio)**. La dificultad escala progresivamente de Zona 1 a Zona 5.

| Zona | Nombre | Puzzles | Dificultad |
|------|--------|---------|-----------|
| Zona 1 | La Antecámara | Keypad | ⭐ |
| Zona 2 | Sala de Energía | Fusebox + LeversChain | ⭐⭐ |
| Zona 3 | Cámara de Ecos | ElectricCircuit | ⭐⭐⭐ |
| Zona 4 | Laberinto de Sombras | MazePuzzle + Padlock + LeversOrder | ⭐⭐⭐⭐ |
| Zona 5 | El Núcleo Divino | SafeWhell + Keycard + LeversState | ⭐⭐⭐⭐⭐ |

---

## Zona 1 — La Antecámara

**Floor:** Floor0\_2 · **Escena:** Escena1

### Puzzle: Keypad
Código de 4 dígitos: `0451`. El jugador debe encontrar 3 pistas distribuidas en la sala:

| Pista | Objeto | Cómo revela el dígito |
|-------|--------|----------------------|
| Dígito 1 → `0` | Paper en el suelo | "¿Cuántas luces están apagadas en esta sala?" → 0 apagadas |
| Dígitos 2 y 3 → `45` | Radio en una mesa | Sintonizado en frecuencia 4.5 · Audio de voz |
| Dígito 4 → `1` | Paper en la pared | Operación matemática: `-2 × (-3) + 4 ÷ 2 - 7 = ?` → respuesta: 1 |

### Al resolver
El CallButton del ascensor se activa automáticamente → el jugador sube a Zona 2.

---

## Zona 2 — La Sala de Energía

**Floor:** Floor1 · **Escena:** Escena1

### Puzzle 1: Fusebox
El jugador debe encontrar **4 fusibles** escondidos en la sala:

| Fusible | Dónde | Mecánica |
|---------|-------|---------|
| Fusible 1 | Storage\_Trunk | Abrir el baúl directamente |
| Fusible 2 | Puzzle\_ChestLockpick | Encontrar ganzúa en Storage\_Trunk → abrir cofre |
| Fusible 3 | Bajo una caja movible | Empujar la caja con MovableObject |
| Fusible 4 | Puzzle\_SafeWhell | Combinación en Paper escondido: `3×3-4=5` |

### Puzzle 2: LeversChain (se activa al completar Fusebox)
El jugador activa las palancas en el orden correcto siguiendo las reacciones en cadena.

### Al resolver
Las puertas del ascensor se abren automáticamente → el jugador sube a Zona 3.

---

## Zona 3 — La Cámara de Ecos

**Floor:** Floor2\_0 · **Escena:** Escena1

### Puzzle: ElectricCircuit
Conectar los cables del circuito eléctrico para completar el camino de corriente. Las pistas están fragmentadas en papers escondidos por la sala.

### Mecánica especial: Zombies
**2 zombies activos** persiguen al jugador mientras resuelve el circuito. El jugador no tiene armas — debe esquivarlos.

### Al resolver
Aparece una llave → el jugador abre la puerta de salida → transición a Escena2 con fade a negro.

---

## Zona 4 — El Laberinto de Sombras

**Floor:** Floor0\_2 · **Escena:** Escena2

### Puzzle 1: MazePuzzle
Guiar la bola a través del laberinto físico. Al completarlo aparece la combinación numérica del candado.

### Puzzle 2: Padlock
Usar la combinación obtenida del laberinto para abrir el candado.

### Puzzle 3: LeversOrder
Dentro del cofre del candado hay una nota con el orden correcto de las palancas. Activarlas en orden correcto desbloquea el ascensor.

---

## Zona 5 — El Núcleo Divino

**Floor:** Floor1 · **Escena:** Escena2

### Sala de preparación
Antes de entrar a la zona de combate el jugador encuentra:
- Pistola (`PlayerItem_Pistol`)
- 3 cargadores de munición
- 2 botiquines de primeros auxilios
- 2 pociones de curación

### Puzzle 1: SafeWhell
Combinar símbolos alienígenas esparcidos por la sala para abrir la caja fuerte.

### Puzzle 2: Keycard
La keycard obtenida activa el lector que revela el estado correcto de las palancas.

### Puzzle 3: LeversState
Activar las palancas en el estado correcto indicado en el suelo.

### Zona de combate — Raining
Al completar los puzzles el jugador accede al bosque bajo lluvia donde debe eliminar **5 zombies** con la pistola para completar el juego.

---

## Sistema de Ascensor

El ascensor conecta los 3 pisos de cada escena. El acceso está bloqueado secuencialmente:

```
Inicio → Solo Piso 1 accesible
Resolver Zona 1 → Se desbloquea Piso 2
Resolver Zona 2 → Se desbloquea Piso 3
Resolver Zona 3 → Se activa puerta de salida a Escena2
```

El script `GuardianGameManager.cs` gestiona todo este flujo habilitando/deshabilitando los `ElevatorInteract` de cada piso.
