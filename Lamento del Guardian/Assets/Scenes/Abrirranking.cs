using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// AbrirRanking — El Lamento del Guardián
/// Guarda la escena actual antes de cargar el Ranking,
/// para que el botón VOLVER del ranking sepa a dónde regresar.
///
/// SETUP:
///   1. Agregar este script al mismo GameObject que tenga el botón VER RANKING
///      (tanto en Victory como en MainMenu)
///   2. Botón VER RANKING → OnClick → AbrirRanking → IrAlRanking()
/// </summary>
public class AbrirRanking : MonoBehaviour
{
    [Header("Nombre exacto de la escena de Ranking")]
    public string escenaRanking = "Ranking";

    /// <summary>
    /// Guarda la escena actual y carga la escena de Ranking.
    /// Conectar al botón VER RANKING.
    /// </summary>
    public void IrAlRanking()
    {
        // Guardar la escena actual para que el botón VOLVER regrese aquí
        SavePathManager.EscenaAnteriorRanking = SceneManager.GetActiveScene().name;
        Debug.Log($"[AbrirRanking] Escena anterior guardada: {SavePathManager.EscenaAnteriorRanking}");

        SceneManager.LoadScene(escenaRanking);
    }
}