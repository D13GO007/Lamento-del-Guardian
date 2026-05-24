# Guión de Diálogos — El Guardián

El Guardián es una entidad alienígena antigua, omnisciente y despiadada. Su voz transmite superioridad intelectual, burla contenida y amenaza latente.

**Tono:** Grave, pausado, con eco leve. Nunca grita — susurra con autoridad.

---

## Zona 1 — La Antecámara

### Al entrar por primera vez
> *"Bienvenido... aunque esa palabra no aplica aquí."*

> *"Llevas siglos de evolución a tus espaldas... y aun así, cruzas mi umbral como si nada te esperara."*

### Timeout — 3 minutos sin resolver
> *"El tiempo no existe para mí. Para ti... es otra historia."*

### Timeout — 6 minutos
> *"Interesante. Sigues aquí. La mayoría ya habrían huido."*

### Al resolver el Keypad
> *"Ah. Entonces sí tienes cerebro."*

> *"El primer sello ha caído. Cuatro más te esperan. Cada uno... más exigente que el anterior."*

---

## Zona 2 — La Sala de Energía

### Al llegar al piso 2
> *"La energía que duerme aquí... la desperté hace eones. Ahora te toca a ti restaurarla."*

### Al recoger cada fusible
> *"Uno."* / *"Dos."* / *"Tres."* / *"El último... no será tan obvio."*

### Al completar el Fusebox
> *"La energía fluye de nuevo. Pero la prueba apenas comienza."*

### Al resolver las palancas
> *"La energía que despiertas... también me despierta a mí. Continúa. Me divierte verte intentarlo."*

---

## Zona 3 — La Cámara de Ecos

### Al llegar al piso 3
> *"Esta sala... es donde los anteriores perdieron la esperanza."*

> *"No están solos aquí. Nunca lo estuvieron."*

### Con zombies activos
> *"Ah. Veo que ya los conociste. No te preocupes por ellos. Preocúpate por el circuito."*

### Al resolver el circuito
> *"Cada cable que conectas... desconectas algo de ti."*

---

## Zona 4 — El Laberinto de Sombras

### Al entrar a Escena 2
> *"Llevas tres sellos. Pocos han llegado tan lejos. Ninguno llegó más allá de lo que sigue."*

### Al resolver el laberinto
> *"La combinación está ante ti. Úsala bien."*

### Al abrir el candado
> *"Llevas cuatro sellos. El último... te costará todo lo que queda de ti. Te lo prometo."*

---

## Zona 5 — El Núcleo Divino

### Al entrar
> *"El núcleo. Nadie había llegado aquí. Nadie."*

### Final — Al resolver el último puzzle
> *"Imposible."*

> *"Nadie... nadie había llegado hasta aquí."*

> *"Toma tu premio, intruso. Has ganado los cinco tesoros de la eternidad."*

> *"Has ganado... por ahora."*

---

## Tabla de Audios

| ID Audio | Trigger | Zona | Tipo |
|----------|---------|------|------|
| AUDIO\_Z1\_ENTRADA\_01 | Al entrar por primera vez | Zona 1 | Narrativa |
| AUDIO\_Z1\_TIMEOUT\_01 | 3 min sin resolver | Zona 1 | Timeout |
| AUDIO\_Z1\_TIMEOUT\_02 | 6 min sin resolver | Zona 1 | Timeout |
| AUDIO\_Z1\_TIMEOUT\_03 | 10 min sin resolver | Zona 1 | Timeout |
| AUDIO\_Z1\_RESUELTO | Código correcto | Zona 1 | Recompensa |
| AUDIO\_Z2\_ENTRADA\_01 | Llegar al piso 2 | Zona 2 | Narrativa |
| AUDIO\_Z2\_FUSIBLE\_01-04 | Recoger cada fusible | Zona 2 | Progreso |
| AUDIO\_Z2\_FUSEBOX\_COMPLETO | Todos los fusibles | Zona 2 | Progreso |
| AUDIO\_Z2\_RESUELTO | Palancas resueltas | Zona 2 | Recompensa |
| AUDIO\_Z3\_ENTRADA\_01 | Llegar al piso 3 | Zona 3 | Narrativa |
| AUDIO\_Z3\_ENTRADA\_02 | Zombies activos | Zona 3 | Narrativa |
| AUDIO\_Z3\_RESUELTO | Circuito completo | Zona 3 | Recompensa |
| AUDIO\_Z4\_ENTRADA\_01 | Entrar Escena 2 | Zona 4 | Narrativa |
| AUDIO\_Z4\_CANDADO\_RESUELTO | Candado abierto | Zona 4 | Recompensa |
| AUDIO\_Z5\_ENTRADA\_01 | Entrar Zona 5 | Zona 5 | Narrativa |
| AUDIO\_Z5\_FINAL\_01-02 | Puzzle final resuelto | Zona 5 | Final |

**Total: 38 audios grabados**
