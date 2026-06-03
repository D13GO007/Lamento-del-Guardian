using UnityEngine;
using UnityEngine.SceneManagement;
using UHFPS.Runtime;

/// <summary>
/// SceneLoader — El Lamento del Guardián
/// Carga escenas usando la pantalla de carga nativa del UHFPS (LoadingScene).
/// Si no hay escena de carga configurada, hace fallback directo con SceneManager.
///
/// SETUP:
///   1. Seleccionar el botón en la Hierarchy
///   2. Add Component → SceneLoader
///   3. En el Inspector escribir el nombre exacto de la escena destino en "Nombre Escena"
///   4. (Opcional) En "Nombre Escena Carga" escribir el nombre de la escena de carga del UHFPS
///   5. En el componente Button → OnClick → + → arrastrar este GameObject
///      → seleccionar SceneLoader → Cargar()
///
/// IMPORTANTE: Todas las escenas deben estar en File → Build Settings → Scenes In Build
/// </summary>
public class SceneLoader : MonoBehaviour
{
    [Header("Nombre exacto de la escena destino (como aparece en Build Settings)")]
    public string nombreEscena;

    [Header("Nombre de la escena de carga del UHFPS (ej: LoadingScreen). Dejar vacío para carga directa)")]
    public string nombreEscenaCarga = "";

    public void Cargar()
    {
        if (string.IsNullOrEmpty(nombreEscena))
        {
            Debug.LogWarning("[SceneLoader] No hay ninguna escena asignada.");
            return;
        }

        // Si hay escena de carga configurada, usarla como intermediario (pantalla nativa UHFPS)
        if (!string.IsNullOrEmpty(nombreEscenaCarga))
        {
            SaveGameManager.LoadSceneName = nombreEscena;
            SceneManager.LoadScene(nombreEscenaCarga);
        }
        else
        {
            // Carga directa sin pantalla de carga
            Debug.Log($"[SceneLoader] Cargando '{nombreEscena}' directamente.");
            SceneManager.LoadScene(nombreEscena);
        }
    }
}