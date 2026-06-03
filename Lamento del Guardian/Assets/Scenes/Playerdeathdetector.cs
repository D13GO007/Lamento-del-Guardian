using UnityEngine;
using UHFPS.Runtime;

/// <summary>
/// PlayerDeathDetector — El Lamento del Guardián
/// Detecta cuando el jugador muere monitoreando el estado Death del UHFPS
/// y registra la muerte en el ScoreManager.
///
/// SETUP:
///   1. Seleccionar el GameObject HEROPLAYER en la Hierarchy
///   2. Add Component → PlayerDeathDetector
///   3. Repetir en Escena1, Escena2 y Raining
/// </summary>
public class PlayerDeathDetector : MonoBehaviour
{
    private PlayerHealth playerHealth;
    private bool muerteRegistrada = false;

    private void Awake()
    {
        playerHealth = GetComponent<PlayerHealth>();

        if (playerHealth == null)
            Debug.LogError("[PlayerDeathDetector] PlayerHealth no encontrado en este GameObject.");
    }

    private void Update()
    {
        if (playerHealth == null || muerteRegistrada) return;

        if (playerHealth.IsDead)
        {
            muerteRegistrada = true;
            ScoreManager.Instance?.RegistrarMuerte();
            Debug.Log("[PlayerDeathDetector] Muerte registrada en ScoreManager.");
        }
    }

    /// <summary>
    /// Resetea el detector cuando el jugador respawnea.
    /// Llamar desde el sistema de respawn si existe.
    /// </summary>
    public void ResetearDetector()
    {
        muerteRegistrada = false;
    }
}