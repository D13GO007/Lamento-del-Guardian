using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

/// <summary>
/// VictoryManager — El Lamento del Guardián
/// Muestra el puntaje final del jugador, sube el resultado al leaderboard
/// de Supabase y permite ir al menú principal o ver el ranking.
/// </summary>
public class VictoryManager : MonoBehaviour
{
    private const string SUPABASE_URL = "https://tisltxhjrvsfmvwpxvku.supabase.co";
    private const string SUPABASE_ANON_KEY = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InRpc2x0eGhqcnZzZm12d3B4dmt1Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3ODAzNjQ0MjUsImV4cCI6MjA5NTk0MDQyNX0.aw8iMn3KudPXu9_mWsAtNJAUz6fPpVFu5Y2BGAqZPMc";

    [Header("Textos de Stats")]
    public TMPro.TMP_Text textPuntaje;
    public TMPro.TMP_Text textTiempo;
    public TMPro.TMP_Text textMuertes;
    public TMPro.TMP_Text textBonus;

    [Header("Mensajes")]
    public TMPro.TMP_Text textMensaje;

    [Header("Escenas")]
    public string escenaMenuPrincipal = "MainMenu";

    private void Start()
    {
        // Liberar cursor al entrar a Victory
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;

        // Asegurar que el timeScale esté en 1
        Time.timeScale = 1f;

        MostrarStats();
        StartCoroutine(SubirPuntajeCoroutine());
    }

    // =============================================
    // MOSTRAR STATS EN PANTALLA
    // =============================================
    private void MostrarStats()
    {
        int puntaje     = ScoreManager.PuntajeFinal;
        int muertes     = ScoreManager.Muertes;
        string tiempo   = ScoreManager.TiempoTotalFormateado();
        int bonusTiempo = CalcularBonusTiempoTotal();

        if (textPuntaje != null) textPuntaje.text = puntaje.ToString("N0");
        if (textTiempo  != null) textTiempo.text  = tiempo;
        if (textMuertes != null) textMuertes.text  = muertes.ToString();
        if (textBonus   != null) textBonus.text    = $"+{bonusTiempo}";

        Debug.Log($"[VictoryManager] Puntaje: {puntaje} | Tiempo: {tiempo} | Muertes: {muertes} | Bonus: {bonusTiempo}");
    }

    private int CalcularBonusTiempoTotal()
    {
        int puntosBase      = 10000;
        int penalizacion    = ScoreManager.Muertes * 300;
        int bonusZombies    = ScoreManager.ZombiesEliminados * 20;
        int bonusSinMuertes = ScoreManager.Muertes == 0 ? 2000 : 0;
        int bonusTiempo     = ScoreManager.PuntajeFinal - puntosBase - bonusZombies - bonusSinMuertes + penalizacion;
        return Mathf.Max(0, bonusTiempo);
    }

    // =============================================
    // SUBIR PUNTAJE AL LEADERBOARD
    // =============================================
    private IEnumerator SubirPuntajeCoroutine()
    {
        if (string.IsNullOrEmpty(SavePathManager.JwtToken) || string.IsNullOrEmpty(SavePathManager.UserId))
        {
            Debug.LogWarning("[VictoryManager] No hay sesión activa. Puntaje no subido.");
            yield break;
        }

        MostrarMensaje("Guardando puntaje...", Color.yellow);

        string url  = $"{SUPABASE_URL}/rest/v1/leaderboard?on_conflict=perfil_id";
        string json = $"{{" +
            $"\"perfil_id\":\"{SavePathManager.UserId}\"," +
            $"\"username\":\"{SavePathManager.Username}\"," +
            $"\"puntaje_final\":{ScoreManager.PuntajeFinal}," +
            $"\"tiempo_juego\":{Mathf.RoundToInt(ScoreManager.TiempoTotalJuego())}," +
            $"\"zombies_eliminados\":{ScoreManager.ZombiesEliminados}," +
            $"\"muertes\":{ScoreManager.Muertes}," +
            $"\"zonas_completadas\":5" +
            $"}}";

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            request.uploadHandler   = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("apikey", SUPABASE_ANON_KEY);
            request.SetRequestHeader("Authorization", $"Bearer {SavePathManager.JwtToken}");
            request.SetRequestHeader("Prefer", "resolution=merge-duplicates,return=minimal");

            yield return request.SendWebRequest();

            Debug.Log($"[VictoryManager] LEADERBOARD - Código: {request.responseCode} | Respuesta: {request.downloadHandler.text}");

            if (request.result == UnityWebRequest.Result.Success)
            {
                MostrarMensaje("¡Puntaje guardado en el ranking!", Color.green);
                yield return new WaitForSeconds(2f);
                LimpiarMensaje();
            }
            else
            {
                MostrarMensaje("Error al guardar el puntaje.", Color.red);
            }
        }
    }

    // =============================================
    // NAVEGACIÓN
    // =============================================
    public void BotonMenuPrincipal()
    {
        ScoreManager.Instance?.ReiniciarEstado();
        SceneManager.LoadScene(escenaMenuPrincipal);
    }

    public void BotonVerRanking()
    {
        // Guardar que venimos de Victory
        SavePathManager.EscenaAnteriorRanking = "Victory";
        SceneManager.LoadScene("Ranking");
    }

    // =============================================
    // MENSAJES
    // =============================================
    private void MostrarMensaje(string mensaje, Color color)
    {
        if (textMensaje != null)
        {
            textMensaje.text  = mensaje;
            textMensaje.color = color;
        }
        Debug.Log($"[VictoryManager] {mensaje}");
    }

    private void LimpiarMensaje()
    {
        if (textMensaje != null)
            textMensaje.text = "";
    }
}