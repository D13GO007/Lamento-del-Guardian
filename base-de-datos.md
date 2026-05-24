# Base de Datos

## Plataforma

**PostgreSQL** alojado en **Supabase**, accedido mediante API REST autogenerada. La aplicación Unity consume esta API mediante `UnityWebRequest`.

---

## Modelo Relacional

```
perfiles
├── id (UUID PK)
├── username (UNIQUE)
├── password_hash (bcrypt)
└── created_at

partidas_guardadas
├── id (UUID PK)
├── perfil_id (FK → perfiles.id)
├── zona_actual (SmallInt 1-5)
├── tesoros_obtenidos (SmallInt 0-5)
├── contador_muertes (Integer)
├── estado_puzles (JSONB)
└── es_activa (Boolean)

leaderboard
├── id (UUID PK)
├── perfil_id (FK → perfiles.id)
├── puntaje_final (Integer)
├── tiempo_juego (Integer)
└── fecha_registro (Timestamp)
```

---

## Tablas

### perfiles

Representa al jugador registrado en el sistema.

| Columna | Tipo | Descripción |
|---------|------|-------------|
| `id` | UUID | Autogenerado con `gen_random_uuid()` |
| `username` | VARCHAR | Nombre único del jugador |
| `password_hash` | VARCHAR | Contraseña cifrada con bcrypt |
| `created_at` | TIMESTAMP | Fecha de registro |

### partidas_guardadas

Representa un ciclo de juego. Solo puede existir **una partida activa** por perfil a la vez.

| Columna | Tipo | Descripción |
|---------|------|-------------|
| `id` | UUID | Autogenerado |
| `perfil_id` | UUID FK | Referencia a `perfiles.id` |
| `zona_actual` | SMALLINT | Zona activa: valores 1 a 5 |
| `tesoros_obtenidos` | SMALLINT | Conteo de tesoros: 0 a 5 |
| `contador_muertes` | INTEGER | Total de muertes acumuladas |
| `estado_puzles` | JSONB | Estado dinámico de acertijos por zona |
| `es_activa` | BOOLEAN | `true` = partida en curso actualmente |

### leaderboard

Registra la puntuación final de cada intento completado.

| Columna | Tipo | Descripción |
|---------|------|-------------|
| `id` | UUID | Autogenerado |
| `perfil_id` | UUID FK | Referencia a `perfiles.id` |
| `puntaje_final` | INTEGER | Puntuación obtenida al finalizar |
| `tiempo_juego` | INTEGER | Duración total en segundos |
| `fecha_registro` | TIMESTAMP | Timestamp de la puntuación |

---

## Decisiones de Diseño Físico

### UUID como clave primaria
Evita colisiones y oculta la secuencialidad, mejorando la seguridad frente a enumeración de registros.

### JSONB para estado_puzles
Permite guardar estructuras jerárquicas dinámicas (puertas abiertas, posición de objetos, glifos activados) sin alterar el esquema relacional con cada nuevo puzzle añadido.

### Índice B-Tree en leaderboard
```sql
CREATE INDEX idx_leaderboard_puntaje 
ON leaderboard(puntaje_final DESC);
```
Garantiza que la consulta Top 10 responda en ≤ 2 segundos bajo carga normal.

### Row Level Security (RLS)
```sql
CREATE POLICY "usuarios_solo_su_partida"
ON partidas_guardadas
FOR ALL USING (
  perfil_id = auth.uid()
);
```
Cada cliente Unity solo puede modificar los registros cuyo `perfil_id` coincida con el `sub` del token JWT activo. Previene trampas y modificaciones no autorizadas.

---

## Requisitos de Rendimiento

| Requisito | Meta | Implementación |
|-----------|------|----------------|
| RNF-001 | Leaderboard ≤ 2 seg | Índice B-Tree sobre `puntaje_final` |
| RNF-002 | Seguridad de partidas | RLS en PostgreSQL |
