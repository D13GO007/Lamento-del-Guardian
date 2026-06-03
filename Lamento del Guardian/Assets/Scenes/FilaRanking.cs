using UnityEngine;

/// <summary>
/// FilaRanking — El Lamento del Guardián
/// Controla una fila del leaderboard en la pantalla Victory.
///
/// SETUP:
///   1. Crear un Prefab con este script
///   2. Asignar los textos en el Inspector
///   3. Arrastrar el Prefab al campo "Prefab Fila Ranking" del VictoryManager
/// </summary>
public class FilaRanking : MonoBehaviour
{
    [Header("Textos")]
    public TMPro.TMP_Text textPosicion;
    public TMPro.TMP_Text textUsername;
    public TMPro.TMP_Text textPuntaje;
    public TMPro.TMP_Text textTiempo;
    public TMPro.TMP_Text textMuertes;

    [Header("Highlight (fila del jugador actual)")]
    public UnityEngine.UI.Image fondoFila;
    public Color colorNormal   = new Color(0f, 0f, 0f, 0.3f);
    public Color colorDestacado = new Color(1f, 0.8f, 0f, 0.3f);

    public void Inicializar(int posicion, string username, string puntaje, string tiempo, string muertes, bool esMiPuntaje)
    {
        if (textPosicion != null) textPosicion.text = $"#{posicion}";
        if (textUsername != null) textUsername.text  = username;
        if (textPuntaje  != null) textPuntaje.text   = int.TryParse(puntaje, out int p) ? p.ToString("N0") : puntaje;
        if (textTiempo   != null) textTiempo.text    = tiempo;
        if (textMuertes  != null) textMuertes.text   = muertes;

        // Destacar la fila del jugador actual
        if (fondoFila != null)
            fondoFila.color = esMiPuntaje ? colorDestacado : colorNormal;
    }
}