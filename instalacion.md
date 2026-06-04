# Cómo Ejecutar el Proyecto

## Requisitos del Sistema

| Componente | Mínimo |
|-----------|--------|
| Sistema Operativo | Windows 10/11 (64-bit) |
| RAM | 8 GB |
| GPU | Compatible con DirectX 11 · Drivers actualizados |
| Almacenamiento | 5 GB libres |
| Controles | Teclado + Ratón |

---

## Opción 1 — Ejecutar el Build (.exe)

### Descargar el ejecutable

📦 **[Descargar Lamento del Guardián v1.0 (Google Drive)](https://drive.google.com/file/d/1RWSPArHp91VTxHiLkEXUa6FBZkjofrGr/view?usp=sharing)**

> Tamaño: ~2.45 GB · Solo Windows 64-bit

### Pasos

1. Descarga el `.zip` desde el link de arriba
2. Extrae el archivo `.zip`
3. Ejecuta `LamentoDelGuardian.exe`
4. No requiere instalación adicional

---

## Opción 2 — Abrir en Unity (Desarrollo)

### Prerrequisitos

- [Unity Hub](https://unity.com/download)
- Unity **6000.3.14f1** o superior
- Git instalado

### Pasos

```bash
# 1. Clonar el repositorio
git clone https://github.com/D13GO007/Lamento-del-Guardian.git

# 2. Abrir Unity Hub
# Add → seleccionar la carpeta clonada

# 3. Esperar que Unity importe todos los assets
# (puede tardar 5-10 minutos la primera vez)

# 4. Abrir la escena principal
# Assets/Scenes/Escena1.unity

# 5. Presionar Play para probar
```

### Escenas del Proyecto

| Escena | Ruta | Descripción |
|--------|------|-------------|
| Escena1 | `Assets/Scenes/Escena1.unity` | Zonas 1, 2 y 3 |
| Escena2 | `Assets/Scenes/Escena2.unity` | Zonas 4 y 5 |
| Raining | `Assets/Scenes/Raining.unity` | Zona de combate final |

---

## Controles

| Acción | Tecla |
|--------|-------|
| Moverse | `W A S D` |
| Mirar | Ratón |
| Interactuar | `E` |
| Correr | `Shift` |
| Agacharse | `Ctrl` |
| Inventario | `Tab` |
| Pausa | `Escape` |
| Disparar | `Click izquierdo` |
| Apuntar | `Click derecho` |
| Recargar | `R` |

---

## Configuración de Supabase (Backend)

Para conectar el backend propio:

1. Crear cuenta en [supabase.com](https://supabase.com)
2. Crear nuevo proyecto
3. Ejecutar el SQL de creación de tablas (ver sección Base de Datos)
4. Copiar la URL y API Key del proyecto
5. Actualizar las constantes en `AuthManager.cs`:

```csharp
private const string SUPABASE_URL = "https://tu-proyecto.supabase.co";
private const string SUPABASE_KEY = "tu-api-key";
```

---

## Solución de Problemas Comunes

| Problema | Solución |
|----------|---------|
| Pantalla negra al iniciar | Verificar drivers de GPU actualizados |
| Zombies no se mueven | Verificar que el NavMesh está horneado en la escena |
| Pistola no dispara al cambiar de escena | El script `PlayerItemsFix.cs` debe estar en el HEROPLAYER |
| Error de compilación UHFPS | Verificar que Unity AI Navigation package está instalado |
