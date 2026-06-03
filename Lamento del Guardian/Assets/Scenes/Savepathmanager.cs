using UnityEngine;
using UHFPS.Scriptable;

/// <summary>
/// SavePathManager — El Lamento del Guardián
/// Persiste entre escenas. Guarda la sesión del jugador, configura
/// la ruta de guardado del UHFPS y maneja la navegación al ranking.
/// </summary>
public class SavePathManager : MonoBehaviour
{
    public static SavePathManager Instance { get; private set; }

    [Header("Arrastrar desde Assets/ThunderWire Studio/UHFPS/Content/Scriptable/")]
    public SerializationAsset serializationAsset;

    // =============================================
    // SESIÓN GLOBAL
    // =============================================
    public static string JwtToken  { get; set; }
    public static string UserId    { get; set; }
    public static string Username  { get; set; }

    // =============================================
    // NAVEGACIÓN RANKING
    // =============================================
    public static string EscenaAnteriorRanking { get; set; } = "MainMenu";

    private const string BASE_SAVES_PATH = "SavedGame";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Guarda la sesión del jugador y configura la ruta de guardado.
    /// </summary>
    public void IniciarSesion(string jwtToken, string userId, string username)
    {
        JwtToken = jwtToken;
        UserId   = userId;
        Username = username;
        ConfigurarRutaUsuario(userId);
    }

    /// <summary>
    /// Cierra la sesión y restablece la ruta de guardado.
    /// </summary>
    public void CerrarSesion()
    {
        JwtToken = null;
        UserId   = null;
        Username = null;
        RestablecerRuta();
    }

    /// <summary>
    /// Configura la ruta de guardado para el usuario logueado.
    /// </summary>
    public void ConfigurarRutaUsuario(string userId)
    {
        if (serializationAsset == null)
        {
            Debug.LogError("[SavePathManager] SerializationAsset no asignado.");
            return;
        }

        if (string.IsNullOrEmpty(userId))
        {
            Debug.LogError("[SavePathManager] UserId vacío.");
            return;
        }

        string rutaUsuario = $"{BASE_SAVES_PATH}/user_{userId}";
        serializationAsset.SavesPath = rutaUsuario;
        Debug.Log($"[SavePathManager] Ruta configurada: {rutaUsuario}");
    }

    /// <summary>
    /// Restaura la ruta por defecto al cerrar sesión.
    /// </summary>
    public void RestablecerRuta()
    {
        if (serializationAsset == null) return;
        serializationAsset.SavesPath = BASE_SAVES_PATH;
        Debug.Log("[SavePathManager] Ruta restablecida a default.");
    }

    /// <summary>
    /// Verifica si hay una sesión activa.
    /// </summary>
    public static bool HaySesionActiva()
    {
        return !string.IsNullOrEmpty(JwtToken) && !string.IsNullOrEmpty(UserId);
    }
}