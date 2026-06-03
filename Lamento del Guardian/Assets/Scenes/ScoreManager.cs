using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ScoreManager — El Lamento del Guardián
/// Mide el tiempo por zona, cuenta muertes y zombies eliminados,
/// y calcula el puntaje final al terminar la Zona 5.
///
/// SETUP:
///   1. Crear un GameObject vacío → renombrar "ScoreManager"
///   2. Add Component → ScoreManager
///   3. Mover al objeto DontDestroyOnLoad (junto al SavePathManager)
///   4. Conectar en GuardianGameManager:
///      - CompletarZona1() → también llama ScoreManager.Instance.TerminarZona(1)
///      - CompletarZona2() → también llama ScoreManager.Instance.TerminarZona(2)
///      - CompletarZona3() → también llama ScoreManager.Instance.TerminarZona(3)
///   5. En ZombieWaveManager al matar zombie 47 → ScoreManager.Instance.TerminarZona(5)
/// </summary>
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    // =============================================
    // TIEMPOS POR ZONA (en segundos)
    // =============================================
    public static float TiempoZona1 { get; private set; }
    public static float TiempoZona2 { get; private set; }
    public static float TiempoZona3 { get; private set; }
    public static float TiempoZona4 { get; private set; }
    public static float TiempoZona5 { get; private set; }

    // =============================================
    // ESTADÍSTICAS
    // =============================================
    public static int Muertes            { get; private set; }
    public static int ZombiesEliminados  { get; private set; }
    public static int PuntajeFinal       { get; private set; }

    // =============================================
    // PUNTOS BASE POR ZONA
    // =============================================
    private const int PUNTOS_ZONA1 = 1000;
    private const int PUNTOS_ZONA2 = 1500;
    private const int PUNTOS_ZONA3 = 2000;
    private const int PUNTOS_ZONA4 = 2500;
    private const int PUNTOS_ZONA5 = 3000;

    private const int PUNTOS_POR_ZOMBIE   = 20;
    private const int PENALIZACION_MUERTE = 300;
    private const int BONUS_SIN_MUERTES   = 2000;

    // Tiempos límite para bonus máximo por zona (en segundos)
    private const float TIEMPO_OPTIMO_ZONA1 = 120f;
    private const float TIEMPO_OPTIMO_ZONA2 = 180f;
    private const float TIEMPO_OPTIMO_ZONA3 = 180f;
    private const float TIEMPO_OPTIMO_ZONA4 = 240f;
    private const float TIEMPO_OPTIMO_ZONA5 = 300f;

    // Estado interno
    private int zonaActual = 0;
    private float tiempoInicioZona = 0f;
    private bool midiendo = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        ReiniciarEstado();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnScenaCargada;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnScenaCargada;
    }

    private void Update()
    {
        // El tiempo se mide en Update para mayor precisión
    }

    // =============================================
    // DETECTAR CAMBIOS DE ESCENA AUTOMÁTICAMENTE
    // =============================================
    private void OnScenaCargada(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "Escena 1":
                // Zona 1 empieza al cargar Escena1
                IniciarCronometroZona(1);
                break;

            case "Escena2":
                // Zona 3 termina, Zona 4 empieza
                TerminarZona(3);
                IniciarCronometroZona(4);
                break;

            case "Raining":
                // Zona 4 termina, Zona 5 empieza
                TerminarZona(4);
                IniciarCronometroZona(5);
                break;
        }
    }

    // =============================================
    // CONTROL DE CRONÓMETRO
    // =============================================
    public void IniciarCronometroZona(int zona)
    {
        zonaActual      = zona;
        tiempoInicioZona = Time.realtimeSinceStartup;
        midiendo        = true;
        Debug.Log($"[ScoreManager] Cronómetro iniciado — Zona {zona}");
    }

    /// <summary>
    /// Termina el cronómetro de la zona indicada y guarda el tiempo.
    /// Conectar desde GuardianGameManager.CompletarZona1/2/3() y ZombieWaveManager.
    /// </summary>
    public void TerminarZona(int zona)
    {
        if (!midiendo) return;

        float tiempoTranscurrido = Time.realtimeSinceStartup - tiempoInicioZona;
        midiendo = false;

        switch (zona)
        {
            case 1: TiempoZona1 = tiempoTranscurrido; break;
            case 2: TiempoZona2 = tiempoTranscurrido; break;
            case 3: TiempoZona3 = tiempoTranscurrido; break;
            case 4: TiempoZona4 = tiempoTranscurrido; break;
            case 5:
                TiempoZona5 = tiempoTranscurrido;
                CalcularPuntajeFinal();
                break;
        }

        Debug.Log($"[ScoreManager] Zona {zona} completada en {tiempoTranscurrido:F1}s");
    }

    // =============================================
    // MUERTES Y ZOMBIES
    // =============================================

    /// <summary>Llamar desde el sistema de muerte del jugador.</summary>
    public void RegistrarMuerte()
    {
        Muertes++;
        Debug.Log($"[ScoreManager] Muerte registrada. Total: {Muertes}");
    }

    /// <summary>Llamar desde ZombieWaveManager.RegistrarZombieMuerto().</summary>
    public void RegistrarZombieEliminado()
    {
        ZombiesEliminados++;
        Debug.Log($"[ScoreManager] Zombie eliminado. Total: {ZombiesEliminados}");
    }

    // =============================================
    // CÁLCULO DE PUNTAJE FINAL
    // =============================================
    private void CalcularPuntajeFinal()
    {
        // Puntos base por completar cada zona
        int puntosBase = PUNTOS_ZONA1 + PUNTOS_ZONA2 + PUNTOS_ZONA3 + PUNTOS_ZONA4 + PUNTOS_ZONA5;

        // Bonus por tiempo en cada zona
        int bonusTiempo = 0;
        bonusTiempo += CalcularBonusTiempo(TiempoZona1, TIEMPO_OPTIMO_ZONA1, PUNTOS_ZONA1);
        bonusTiempo += CalcularBonusTiempo(TiempoZona2, TIEMPO_OPTIMO_ZONA2, PUNTOS_ZONA2);
        bonusTiempo += CalcularBonusTiempo(TiempoZona3, TIEMPO_OPTIMO_ZONA3, PUNTOS_ZONA3);
        bonusTiempo += CalcularBonusTiempo(TiempoZona4, TIEMPO_OPTIMO_ZONA4, PUNTOS_ZONA4);
        bonusTiempo += CalcularBonusTiempo(TiempoZona5, TIEMPO_OPTIMO_ZONA5, PUNTOS_ZONA5);

        // Bonus por zombies eliminados
        int bonusZombies = ZombiesEliminados * PUNTOS_POR_ZOMBIE;

        // Bonus por no morir
        int bonusSinMuertes = (Muertes == 0) ? BONUS_SIN_MUERTES : 0;

        // Penalización por muertes
        int penalizacion = Muertes * PENALIZACION_MUERTE;

        // Puntaje final (mínimo 0)
        PuntajeFinal = Mathf.Max(0, puntosBase + bonusTiempo + bonusZombies + bonusSinMuertes - penalizacion);

        Debug.Log($"[ScoreManager] ====== PUNTAJE FINAL ======");
        Debug.Log($"[ScoreManager] Base:          {puntosBase}");
        Debug.Log($"[ScoreManager] Bonus tiempo:  {bonusTiempo}");
        Debug.Log($"[ScoreManager] Bonus zombies: {bonusZombies}");
        Debug.Log($"[ScoreManager] Bonus perfecto:{bonusSinMuertes}");
        Debug.Log($"[ScoreManager] Penalización:  -{penalizacion}");
        Debug.Log($"[ScoreManager] TOTAL:         {PuntajeFinal}");
    }

    /// <summary>
    /// Calcula el bonus de tiempo para una zona.
    /// Si terminó antes del tiempo óptimo → hasta 50% del puntaje base de la zona.
    /// Si tardó el doble del óptimo o más → 0 bonus.
    /// </summary>
    private int CalcularBonusTiempo(float tiempoReal, float tiempoOptimo, int puntosZona)
    {
        if (tiempoReal <= 0f) return 0;

        // Factor entre 0 y 1 según qué tan rápido fue
        float factor = Mathf.Clamp01(1f - (tiempoReal / (tiempoOptimo * 2f)));
        int bonus = Mathf.RoundToInt(puntosZona * 0.5f * factor);
        return bonus;
    }

    // =============================================
    // TIEMPO TOTAL DE JUEGO
    // =============================================
    public static float TiempoTotalJuego()
    {
        return TiempoZona1 + TiempoZona2 + TiempoZona3 + TiempoZona4 + TiempoZona5;
    }

    public static string TiempoTotalFormateado()
    {
        float total = TiempoTotalJuego();
        int horas   = Mathf.FloorToInt(total / 3600);
        int minutos = Mathf.FloorToInt((total % 3600) / 60);
        int segundos = Mathf.FloorToInt(total % 60);
        return horas > 0
            ? $"{horas:D2}:{minutos:D2}:{segundos:D2}"
            : $"{minutos:D2}:{segundos:D2}";
    }

    // =============================================
    // REINICIAR (nueva partida)
    // =============================================
    public void ReiniciarEstado()
    {
        TiempoZona1 = 0f;
        TiempoZona2 = 0f;
        TiempoZona3 = 0f;
        TiempoZona4 = 0f;
        TiempoZona5 = 0f;
        Muertes           = 0;
        ZombiesEliminados = 0;
        PuntajeFinal      = 0;
        zonaActual        = 0;
        midiendo          = false;
        Debug.Log("[ScoreManager] Estado reiniciado.");
    }
}