using System.Collections;
using UnityEngine;
using UHFPS.Runtime;

/// <summary>
/// GuardianGameManager — El Lamento del Guardián
/// Gestiona el progreso por zonas y controla el acceso al ascensor.
/// 
/// DISTRIBUCIÓN DE PISOS:
/// Piso 3 (arriba)   = Zona 1 — punto de inicio
/// Piso 2 (medio)    = Zona 2
/// Piso 1 (abajo)    = Zona 3
/// 
/// LÓGICA: el jugador BAJA desde Zona 1 hacia Zona 3.
/// Al inicio solo puede bajar al Piso 2.
/// Al completar Zona 2 puede bajar al Piso 1.
/// </summary>
public class GuardianGameManager : MonoBehaviour
{
    [Header("=== ESTADO DE ZONAS ===")]
    [SerializeField] private bool zona1Completada = false;
    [SerializeField] private bool zona2Completada = false;
    [SerializeField] private bool zona3Completada = false;

    [Header("=== CALL BUTTONS EXTERNOS (fuera del ascensor) ===")]
    [Tooltip("CallButton externo del Piso 3 (Zona 1) — inicio del juego")]
    [SerializeField] private ElevatorInteract callButtonExterno_Piso3;

    [Tooltip("CallButton externo del Piso 2 (Zona 2) — se activa al completar Zona 1")]
    [SerializeField] private ElevatorInteract callButtonExterno_Piso2;

    [Tooltip("CallButton externo del Piso 1 (Zona 3) — se activa al completar Zona 2")]
    [SerializeField] private ElevatorInteract callButtonExterno_Piso1;

    [Header("=== BOTONES INTERNOS (dentro de la cabina) ===")]
    [Tooltip("Botón interno que va al Piso 3 (Zona 1) — bloqueado")]
    [SerializeField] private ElevatorInteract botonInterno_Piso3;

    [Tooltip("Botón interno que va al Piso 2 (Zona 2) — activo desde el inicio")]
    [SerializeField] private ElevatorInteract botonInterno_Piso2;

    [Tooltip("Botón interno que va al Piso 1 (Zona 3) — bloqueado hasta completar Zona 2")]
    [SerializeField] private ElevatorInteract botonInterno_Piso1;

    [Header("=== PUERTA DE SALIDA (Escena 2) ===")]
    [SerializeField] private GameObject puertaSalida;

    [Header("=== NOMBRE DE LA SIGUIENTE ESCENA ===")]
    [SerializeField] private string nombreEscena2 = "Escena2";

    public static GuardianGameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        InicializarEstado();
    }

    private void InicializarEstado()
    {
        SetBtn(callButtonExterno_Piso3, true);
        SetBtn(callButtonExterno_Piso2, false);
        SetBtn(callButtonExterno_Piso1, false);

        SetBtn(botonInterno_Piso3, false);
        SetBtn(botonInterno_Piso2, true);
        SetBtn(botonInterno_Piso1, false);

        if (puertaSalida != null) puertaSalida.SetActive(false);

        Debug.Log("[Guardian] Estado inicial: solo Zona 1 accesible.");
    }

    /// <summary>Llamar desde OnAccessGranted del Keypad de Zona 1.</summary>
    public void CompletarZona1()
    {
        if (zona1Completada) return;
        zona1Completada = true;

        SetBtn(callButtonExterno_Piso2, true);

        // Detener cronómetro Zona 1 e iniciar Zona 2
        ScoreManager.Instance?.TerminarZona(1);
        ScoreManager.Instance?.IniciarCronometroZona(2);

        Debug.Log("[Guardian] Zona 1 completada. Piso 2 desbloqueado.");
    }

    /// <summary>Llamar desde OnLeversCorrect del LeversChain de Zona 2.</summary>
    public void CompletarZona2()
    {
        if (zona2Completada) return;
        zona2Completada = true;

        SetBtn(callButtonExterno_Piso1, true);
        SetBtn(botonInterno_Piso1, true);

        // Detener cronómetro Zona 2 e iniciar Zona 3
        ScoreManager.Instance?.TerminarZona(2);
        ScoreManager.Instance?.IniciarCronometroZona(3);

        Debug.Log("[Guardian] Zona 2 completada. Piso 1 desbloqueado.");
    }

    /// <summary>Llamar desde OnConnected del ElectricCircuit de Zona 3.</summary>
    public void CompletarZona3()
    {
        if (zona3Completada) return;
        zona3Completada = true;

        if (puertaSalida != null) puertaSalida.SetActive(true);

        // Detener cronómetro Zona 3 (Zona 4 inicia automáticamente al cargar Escena2)
        ScoreManager.Instance?.TerminarZona(3);

        Debug.Log("[Guardian] Zona 3 completada. Puerta de salida activada.");
    }

    /// <summary>Carga la Escena 2 con fade a negro.</summary>
    public void CargarEscena2()
    {
        Debug.Log("[Guardian] Cargando Escena2...");
        StartCoroutine(TransicionEscena2());
    }

    private IEnumerator TransicionEscena2()
    {
        yield return new WaitForSeconds(1f);
        UnityEngine.SceneManagement.SceneManager.LoadScene(nombreEscena2);
    }

    public bool EsZona1Completada() => zona1Completada;
    public bool EsZona2Completada() => zona2Completada;
    public bool EsZona3Completada() => zona3Completada;

    private void SetBtn(ElevatorInteract btn, bool activo)
    {
        if (btn != null) btn.gameObject.SetActive(activo);
    }
}