using UnityEngine;

/// <summary>
/// GuardianTrigger — El Lamento del Guardián
///
/// SETUP:
///   1. Crear GameObject vacío en la zona deseada
///   2. Add Component → Box Collider → marcar "Is Trigger" → ajustar tamaño
///   3. Add Component → GuardianTrigger
///   4. En el Inspector seleccionar el método correspondiente en "Metodo"
///
/// Se reproduce una sola vez (no vuelve a disparar si el jugador sale y entra de nuevo).
/// </summary>
public class GuardianTrigger : MonoBehaviour
{
    public enum Metodo
    {
        Z1_KEYPAD_INTENTO,
        Z4_LABERINTO_01,
    }

    [Header("Método a llamar al entrar al trigger")]
    public Metodo metodo;

    private bool _played;

    void OnTriggerEnter(Collider other)
    {
        if (_played || !other.CompareTag("Player")) return;
        _played = true;

        if (GuardianAudioManager.Instance == null)
        {
            Debug.LogWarning("[GuardianTrigger] No se encontró GuardianAudioManager en la escena.");
            return;
        }

        switch (metodo)
        {
            case Metodo.Z1_KEYPAD_INTENTO:
                GuardianAudioManager.Instance.OnZona1KeypadIntento();
                break;
            case Metodo.Z4_LABERINTO_01:
                GuardianAudioManager.Instance.OnZona4LaberintoIntento();
                break;
        }
    }
}