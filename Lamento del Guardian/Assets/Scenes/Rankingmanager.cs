using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

/// <summary>
/// RankingManager — El Lamento del Guardián
/// Carga el Top 8 del leaderboard desde Supabase y lo muestra en pantalla.
///
/// SETUP:
///   1. Crear GameObject vacío → renombrar "RankingManager"
///   2. Add Component → RankingManager
///   3. Asignar los 8 grupos de textos en el Inspector
///   4. Botón VOLVER → OnClick → RankingManager → BotonVolver()
/// </summary>
public class RankingManager : MonoBehaviour
{
    private const string SUPABASE_URL = "https://tisltxhjrvsfmvwpxvku.supabase.co";
    private const string SUPABASE_ANON_KEY = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InRpc2x0eGhqcnZzZm12d3B4dmt1Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3ODAzNjQ0MjUsImV4cCI6MjA5NTk0MDQyNX0.aw8iMn3KudPXu9_mWsAtNJAUz6fPpVFu5Y2BGAqZPMc";

    [Header("Fila 1")]
    public TMPro.TMP_Text textJugador1;
    public TMPro.TMP_Text textPuntaje1;
    public TMPro.TMP_Text textTiempo1;
    public TMPro.TMP_Text textMuertes1;

    [Header("Fila 2")]
    public TMPro.TMP_Text textJugador2;
    public TMPro.TMP_Text textPuntaje2;
    public TMPro.TMP_Text textTiempo2;
    public TMPro.TMP_Text textMuertes2;

    [Header("Fila 3")]
    public TMPro.TMP_Text textJugador3;
    public TMPro.TMP_Text textPuntaje3;
    public TMPro.TMP_Text textTiempo3;
    public TMPro.TMP_Text textMuertes3;

    [Header("Fila 4")]
    public TMPro.TMP_Text textJugador4;
    public TMPro.TMP_Text textPuntaje4;
    public TMPro.TMP_Text textTiempo4;
    public TMPro.TMP_Text textMuertes4;

    [Header("Fila 5")]
    public TMPro.TMP_Text textJugador5;
    public TMPro.TMP_Text textPuntaje5;
    public TMPro.TMP_Text textTiempo5;
    public TMPro.TMP_Text textMuertes5;

    [Header("Fila 6")]
    public TMPro.TMP_Text textJugador6;
    public TMPro.TMP_Text textPuntaje6;
    public TMPro.TMP_Text textTiempo6;
    public TMPro.TMP_Text textMuertes6;

    [Header("Fila 7")]
    public TMPro.TMP_Text textJugador7;
    public TMPro.TMP_Text textPuntaje7;
    public TMPro.TMP_Text textTiempo7;
    public TMPro.TMP_Text textMuertes7;

    [Header("Fila 8")]
    public TMPro.TMP_Text textJugador8;
    public TMPro.TMP_Text textPuntaje8;
    public TMPro.TMP_Text textTiempo8;
    public TMPro.TMP_Text textMuertes8;

    [Header("Mensajes")]
    public TMPro.TMP_Text textMensaje;

    [Header("Escena anterior")]
    public string escenaAnterior = "MainMenu";

    // Arrays internos para facilitar el llenado
    private TMPro.TMP_Text[] jugadores;
    private TMPro.TMP_Text[] puntajes;
    private TMPro.TMP_Text[] tiempos;
    private TMPro.TMP_Text[] muertes;

    private void Start()
    {
        // Inicializar arrays
        jugadores = new TMPro.TMP_Text[] { textJugador1, textJugador2, textJugador3, textJugador4, textJugador5, textJugador6, textJugador7, textJugador8 };
        puntajes  = new TMPro.TMP_Text[] { textPuntaje1, textPuntaje2, textPuntaje3, textPuntaje4, textPuntaje5, textPuntaje6, textPuntaje7, textPuntaje8 };
        tiempos   = new TMPro.TMP_Text[] { textTiempo1,  textTiempo2,  textTiempo3,  textTiempo4,  textTiempo5,  textTiempo6,  textTiempo7,  textTiempo8  };
        muertes   = new TMPro.TMP_Text[] { textMuertes1, textMuertes2, textMuertes3, textMuertes4, textMuertes5, textMuertes6, textMuertes7, textMuertes8 };

        LimpiarFilas();
        StartCoroutine(CargarRankingCoroutine());
    }

    // =============================================
    // CARGAR RANKING DESDE SUPABASE
    // =============================================
    private IEnumerator CargarRankingCoroutine()
    {
        MostrarMensaje("Cargando ranking...", Color.yellow);

        string url = $"{SUPABASE_URL}/rest/v1/leaderboard?select=username,puntaje_final,tiempo_juego,muertes&order=puntaje_final.desc&limit=8";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("apikey", SUPABASE_ANON_KEY);

            // Si hay sesión activa la usamos, si no cargamos igual (ranking es público)
            if (!string.IsNullOrEmpty(SavePathManager.JwtToken))
                request.SetRequestHeader("Authorization", $"Bearer {SavePathManager.JwtToken}");

            yield return request.SendWebRequest();

            Debug.Log($"[RankingManager] Código: {request.responseCode} | Respuesta: {request.downloadHandler.text}");

            if (request.result == UnityWebRequest.Result.Success)
            {
                LimpiarMensaje();
                PoblarRanking(request.downloadHandler.text);
            }
            else
            {
                MostrarMensaje("Error al cargar el ranking.", Color.red);
            }
        }
    }

    // =============================================
    // POBLAR FILAS CON DATOS
    // =============================================
    private void PoblarRanking(string json)
    {
        json = json.Trim();
        if (json == "[]" || string.IsNullOrEmpty(json))
        {
            MostrarMensaje("Aún no hay puntajes registrados.", Color.white);
            return;
        }

        // Normalizar saltos de línea antes de dividir
        json = json.Replace("\n", "").Replace("\r", "").Replace(" ", "");
        string[] entradas = json.Split(new string[] { "},{" }, StringSplitOptions.None);

        for (int i = 0; i < entradas.Length && i < 8; i++)
        {
            string limpio = entradas[i]
                .Replace("[", "").Replace("]", "")
                .Replace("{", "").Replace("}", "");

            string username  = ExtraerValor(limpio, "username");
            string puntaje   = ExtraerValor(limpio, "puntaje_final");
            string tiempoSeg = ExtraerValor(limpio, "tiempo_juego");
            string muerte    = ExtraerValor(limpio, "muertes");

            // Formatear tiempo
            string tiempoFormateado = "--:--";
            if (int.TryParse(tiempoSeg, out int seg))
            {
                int min = seg / 60;
                int s   = seg % 60;
                tiempoFormateado = $"{min:D2}:{s:D2}";
            }

            // Formatear puntaje con separador de miles
            string puntajeFormateado = puntaje;
            if (int.TryParse(puntaje, out int p))
                puntajeFormateado = p.ToString("N0");

            // Asignar a la fila correspondiente
            if (jugadores[i] != null) jugadores[i].text = username;
            if (puntajes[i]  != null) puntajes[i].text  = puntajeFormateado;
            if (tiempos[i]   != null) tiempos[i].text   = tiempoFormateado;
            if (muertes[i]   != null) muertes[i].text   = muerte;

            // Destacar fila del jugador actual
            if (!string.IsNullOrEmpty(SavePathManager.Username) &&
                username == SavePathManager.Username)
            {
                DestacatFila(i);
            }
        }
    }

    // =============================================
    // DESTACAR FILA DEL JUGADOR ACTUAL
    // =============================================
    private void DestacatFila(int indice)
    {
        if (jugadores[indice] != null) jugadores[indice].color = Color.yellow;
        if (puntajes[indice]  != null) puntajes[indice].color  = Color.yellow;
        if (tiempos[indice]   != null) tiempos[indice].color   = Color.yellow;
        if (muertes[indice]   != null) muertes[indice].color   = Color.yellow;
    }

    // =============================================
    // LIMPIAR FILAS
    // =============================================
    private void LimpiarFilas()
    {
        for (int i = 0; i < 8; i++)
        {
            if (jugadores[i] != null) jugadores[i].text = "---";
            if (puntajes[i]  != null) puntajes[i].text  = "---";
            if (tiempos[i]   != null) tiempos[i].text   = "--:--";
            if (muertes[i]   != null) muertes[i].text   = "-";
        }
    }

    // =============================================
    // NAVEGACIÓN
    // =============================================
    public void BotonVolver()
    {
        SceneManager.LoadScene(SavePathManager.EscenaAnteriorRanking);
    }

    // =============================================
    // PARSEO JSON
    // =============================================
    private string ExtraerValor(string json, string clave)
    {
        string buscar = $"\"{clave}\":";
        int idx = json.IndexOf(buscar);
        if (idx < 0) return "";

        int inicio  = idx + buscar.Length;
        bool esString = inicio < json.Length && json[inicio] == '"';

        if (esString)
        {
            inicio++;
            int fin = json.IndexOf('"', inicio);
            return fin >= 0 ? json.Substring(inicio, fin - inicio) : "";
        }
        else
        {
            int fin = json.IndexOfAny(new char[] { ',', '}' }, inicio);
            return fin >= 0 ? json.Substring(inicio, fin - inicio).Trim() : "";
        }
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
        Debug.Log($"[RankingManager] {mensaje}");
    }

    private void LimpiarMensaje()
    {
        if (textMensaje != null)
            textMensaje.text = "";
    }
}