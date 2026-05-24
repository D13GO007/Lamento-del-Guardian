using System.Collections;
using UnityEngine;
using UHFPS.Runtime;

/// <summary>
/// GuardianGameManager — El Lamento del Guardián
/// Gestiona el progreso por zonas y controla el acceso al ascensor.
/// Adjuntar este script al GameObject GAMEMANAGER en la escena.
/// </summary>
public class GuardianGameManager : MonoBehaviour
{
    [Header("=== ESTADO DE ZONAS ===")]
    [SerializeField] private bool zona1Completada = false;
    [SerializeField] private bool zona2Completada = false;
    [SerializeField] private bool zona3Completada = false;

    [Header("=== CALL BUTTONS EXTERNOS (fuera del ascensor) ===")]
    [Tooltip("CallButton externo del Piso 1 (Floor0_2) — inicio")]
    [SerializeField] private ElevatorInteract callButtonExterno_Piso1;

    [Tooltip("CallButton externo del Piso 2 (Floor1) — se activa al completar Zona 1")]
    [SerializeField] private ElevatorInteract callButtonExterno_Piso2;

    [Tooltip("CallButton externo del Piso 3 (Floor2_0) — se activa al completar Zona 2")]
    [SerializeField] private ElevatorInteract callButtonExterno_Piso3;

    [Header("=== BOTONES INTERNOS (dentro de la cabina) ===")]
    [Tooltip("Botón interno que va al Piso 1 — SIEMPRE desactivado (no se puede regresar)")]
    [SerializeField] private ElevatorInteract botonInterno_Piso1;

    [Tooltip("Botón interno que va al Piso 2")]
    [SerializeField] private ElevatorInteract botonInterno_Piso2;

    [Tooltip("Botón interno que va al Piso 3")]
    [SerializeField] private ElevatorInteract botonInterno_Piso3;

    [Header("=== PUERTA DE SALIDA (Escena 2) ===")]
    [Tooltip("Puerta o trigger que lleva a la Escena 2 — se activa al completar Zona 3")]
    [SerializeField] private GameObject puertaSalida;

    [Header("=== NOMBRE DE LA SIGUIENTE ESCENA ===")]
    [SerializeField] private string nombreEscena2 = "Escena2";

    // ── Singleton ──────────────────────────────────────────────
    public static GuardianGameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        InicializarEstado();
    }

    /// <summary>
    /// Configura el estado inicial: solo Piso 1 accesible.
    /// </summary>
    private void InicializarEstado()
    {
        // Externos: solo Piso 1 activo al inicio
        SetCallButton(callButtonExterno_Piso1, true);
        SetCallButton(callButtonExterno_Piso2, false);
        SetCallButton(callButtonExterno_Piso3, false);

        // Internos: Piso 1 siempre bloqueado (no se regresa)
        SetCallButton(botonInterno_Piso1, false);
        SetCallButton(botonInterno_Piso2, true);   // puede bajar al 2 desde el inicio
        SetCallButton(botonInterno_Piso3, false);  // bloqueado hasta completar Zona 1

        // Puerta de salida desactivada
        if (puertaSalida != null)
            puertaSalida.SetActive(false);

        Debug.Log("[Guardian] Estado inicial configurado. Solo Zona 1 accesible.");
    }

    // ══════════════════════════════════════════════════════════
    //  MÉTODOS PÚBLICOS — conectar en los eventos OnPuzzleSolved
    // ══════════════════════════════════════════════════════════

    /// <summary>
    /// Llamar desde el evento OnPuzzleSolved del Keypad (Zona 1).
    /// </summary>
    public void CompletarZona1()
    {
        if (zona1Completada) return;
        zona1Completada = true;

        // Desbloquear acceso al Piso 2
        SetCallButton(callButtonExterno_Piso2, true);
        SetCallButton(botonInterno_Piso3, false); // aún no puede ir al 3

        Debug.Log("[Guardian] Zona 1 completada. Piso 2 desbloqueado.");
    }

    /// <summary>
    /// Llamar desde el evento OnPuzzleSolved del último puzzle de Zona 2 (LeversChain).
    /// </summary>
    public void CompletarZona2()
    {
        if (zona2Completada) return;
        zona2Completada = true;

        // Desbloquear acceso al Piso 3
        SetCallButton(callButtonExterno_Piso3, true);
        SetCallButton(botonInterno_Piso3, true);

        Debug.Log("[Guardian] Zona 2 completada. Piso 3 desbloqueado.");
    }

    /// <summary>
    /// Llamar desde el evento OnPuzzleSolved del último puzzle de Zona 3 (ChestLockpick).
    /// </summary>
    public void CompletarZona3()
    {
        if (zona3Completada) return;
        zona3Completada = true;

        // Activar puerta de salida hacia Escena 2
        if (puertaSalida != null)
            puertaSalida.SetActive(true);

        Debug.Log("[Guardian] Zona 3 completada. Puerta de salida activada.");
    }

    /// <summary>
    /// Carga la Escena 2 con pantalla negra.
    /// Conectar al trigger de la puerta de salida.
    /// </summary>
   public void CargarEscena2()
{
    Debug.Log("[Guardian] Cargando Escena2...");
    StartCoroutine(TransicionEscena2());
}
    private IEnumerator TransicionEscena2()
    {
        // Fade a negro — usa el sistema de UHFPS si está disponible
        yield return new WaitForSeconds(1f);
        UnityEngine.SceneManagement.SceneManager.LoadScene(nombreEscena2);
    }

    // ══════════════════════════════════════════════════════════
    //  GETTERS — para consultar estado desde otros scripts
    // ══════════════════════════════════════════════════════════

    public bool EsZona1Completada() => zona1Completada;
    public bool EsZona2Completada() => zona2Completada;
    public bool EsZona3Completada() => zona3Completada;

    // ── Helper ─────────────────────────────────────────────────
    private void SetCallButton(ElevatorInteract btn, bool activo)
    {
        if (btn != null)
            btn.enabled = activo;
    }
}