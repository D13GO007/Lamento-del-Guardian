using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// GuardianAudioManager — El Lamento del Guardián
/// 
/// SETUP (una sola vez por escena):
///   1. Crear GameObject vacío → renombrar "GuardianAudioManager"
///   2. Add Component → GuardianAudioManager
///   3. Arrastrar todos los clips de audio al array "Clips" en el Inspector
///      siguiendo el orden del enum GuardianClip abajo
///   4. Conectar los métodos públicos a los eventos de cada puzzle en el Inspector
/// 
/// USO DESDE OTROS SCRIPTS:
///   GuardianAudioManager.Instance.Play(GuardianClip.Z1_RESUELTO);
/// </summary>
public class GuardianAudioManager : MonoBehaviour
{
    // ─── Singleton ───────────────────────────────────────────────
    public static GuardianAudioManager Instance { get; private set; }

    // ─── Enum con todos los audios (orden = índice en el array) ──
    public enum GuardianClip
    {
        // Zona 1
        Z1_ENTRADA_01       = 0,
        Z1_ENTRADA_02       = 1,
        Z1_KEYPAD_INTENTO   = 2,
        Z1_RESUELTO         = 3,

        // Zona 2
        Z2_ENTRADA_01       = 4,
        Z2_FUSEBOX_COMPLETO = 5,
        Z2_RESUELTO         = 6,

        // Zona 3
        Z3_ENTRADA_01       = 7,
        Z3_ENTRADA_02       = 8,
        Z3_RESUELTO         = 9,

        // Zona 4
        Z4_ENTRADA_01       = 10,
        Z4_LABERINTO_01     = 11,
        Z4_CANDADO_RESUELTO = 12,

        // Zona 5 — Raining
        Z5_ENTRADA_01       = 13,
        Z5_COMBATE_INICIO   = 14,
        Z5_VICTORIA         = 15,
    }

    // ─── Inspector ───────────────────────────────────────────────
    [Header("Clips — respetar el orden del enum GuardianClip")]
    public AudioClip[] clips;

    [Header("Configuración")]
    [Range(0f, 1f)] public float volume = 1f;

    // Delay fijo para audios de entrada secundaria (Z1_ENTRADA_02, Z3_ENTRADA_02)
    private const float ENTRADA_DELAY = 5f;

    // ─── Privados ────────────────────────────────────────────────
    private AudioSource _source;
    private bool _z1EntradaPlayed;
    private bool _z3EntradaPlayed;

    // ─── Unity ───────────────────────────────────────────────────
    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        _source = gameObject.AddComponent<AudioSource>();
        _source.playOnAwake = false;
        _source.volume = volume;
    }

    void Start()
    {
        string scene = SceneManager.GetActiveScene().name;
        if (scene == "Escena 1")       OnZona1Entrada();
        else if (scene == "Escena2")   OnZona4Entrada();
        else if (scene == "Raining")   OnZona5Entrada();
    }

    // ─── API pública principal ───────────────────────────────────

    /// <summary>Reproducir un clip por enum. Interrumpe el audio actual.</summary>
    public void Play(GuardianClip clip)
    {
        int idx = (int)clip;
        if (idx < 0 || idx >= clips.Length || clips[idx] == null)
        {
            Debug.LogWarning($"[Guardian] Clip {clip} no asignado en el Inspector.");
            return;
        }
        _source.Stop();
        _source.clip = clips[idx];
        _source.Play();
    }

    /// <summary>Reproducir con delay en segundos.</summary>
    public void PlayDelayed(GuardianClip clip, float delay)
    {
        StartCoroutine(PlayAfterDelay(clip, delay));
    }

    private IEnumerator PlayAfterDelay(GuardianClip clip, float delay)
    {
        yield return new WaitForSeconds(delay);
        Play(clip);
    }

    // ─── Métodos para conectar directo desde eventos Unity ───────
    // (UnityEvent solo acepta métodos sin parámetros o con un parámetro simple)
    // Conecta estos al Inspector de cada puzzle con un clic.

    // ZONA 1 ──────────────────────────────────────────────────────

    /// <summary>Conectar al Start() de la escena o a un trigger de entrada.</summary>
    public void OnZona1Entrada()
    {
        if (_z1EntradaPlayed) return;
        _z1EntradaPlayed = true;
        Play(GuardianClip.Z1_ENTRADA_01);
        PlayDelayed(GuardianClip.Z1_ENTRADA_02, ENTRADA_DELAY);
    }

    /// <summary>Conectar al OnTriggerEnter del collider frente al Keypad.</summary>
    public void OnZona1KeypadIntento()
    {
        Play(GuardianClip.Z1_KEYPAD_INTENTO);
    }

    /// <summary>Conectar al OnAccessGranted del Puzzle_Keypad.</summary>
    public void OnZona1Resuelto()
    {
        Play(GuardianClip.Z1_RESUELTO);
    }

    // ZONA 2 ──────────────────────────────────────────────────────

    /// <summary>Conectar al evento de llegada al Piso 2 del ascensor.</summary>
    public void OnZona2Entrada()
    {
        Play(GuardianClip.Z2_ENTRADA_01);
    }

    /// <summary>Conectar al OnAllFusesConnected del Puzzle_Fusebox.</summary>
    public void OnZona2FuseboxCompleto()
    {
        Play(GuardianClip.Z2_FUSEBOX_COMPLETO);
    }

    /// <summary>Conectar al OnLeversCorrect del Puzzle_LeversChain.</summary>
    public void OnZona2Resuelto()
    {
        Play(GuardianClip.Z2_RESUELTO);
    }

    // ZONA 3 ──────────────────────────────────────────────────────

    /// <summary>Conectar al evento de llegada al Piso 1 del ascensor.</summary>
    public void OnZona3Entrada()
    {
        if (_z3EntradaPlayed) return;
        _z3EntradaPlayed = true;
        Play(GuardianClip.Z3_ENTRADA_01);
        PlayDelayed(GuardianClip.Z3_ENTRADA_02, ENTRADA_DELAY);
    }

    /// <summary>Conectar al OnConnected del Puzzle_ElectricCircuit.</summary>
    public void OnZona3Resuelto()
    {
        Play(GuardianClip.Z3_RESUELTO);
    }

    // ZONA 4 ──────────────────────────────────────────────────────

    /// <summary>Conectar al Start() de Escena2 o trigger de entrada.</summary>
    public void OnZona4Entrada()
    {
        Play(GuardianClip.Z4_ENTRADA_01);
    }

    /// <summary>Conectar al OnTriggerEnter del collider frente al MazePuzzle.</summary>
    public void OnZona4LaberintoIntento()
    {
        Play(GuardianClip.Z4_LABERINTO_01);
    }

    /// <summary>Conectar al OnUnlocked del Puzzle_Padlock.</summary>
    public void OnZona4CandadoResuelto()
    {
        Play(GuardianClip.Z4_CANDADO_RESUELTO);
    }

    // ZONA 5 — Raining ────────────────────────────────────────────

    /// <summary>Conectar al Start() de la escena Raining.</summary>
    public void OnZona5Entrada()
    {
        Play(GuardianClip.Z5_ENTRADA_01);
    }

    /// <summary>Conectar al primer RegistrarZombieMuerto() o al spawn del primer zombie.</summary>
    public void OnZona5CombateInicio()
    {
        Play(GuardianClip.Z5_COMBATE_INICIO);
    }

    /// <summary>Conectar al ZombieWaveManager cuando llega a 5 kills, antes de cargar Victory.</summary>
    public void OnZona5Victoria()
    {
        Play(GuardianClip.Z5_VICTORIA);
    }
}