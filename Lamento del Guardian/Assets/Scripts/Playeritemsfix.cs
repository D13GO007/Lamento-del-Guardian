using System.Collections;
using UnityEngine;
using UHFPS.Runtime;

/// <summary>
/// PlayerItemsFix — El Lamento del Guardián
/// Fuerza la reinicialización de los Player Items al cargar una nueva escena.
/// Adjuntar al HEROPLAYER en la escena Raining.
/// </summary>
public class PlayerItemsFix : MonoBehaviour
{
    private IEnumerator Start()
    {
        // Espera 2 frames para que todo cargue
        yield return null;
        yield return null;

        // Busca el PlayerItemsManager en el HEROPLAYER
        PlayerItemsManager itemsManager = GetComponentInChildren<PlayerItemsManager>();

        if (itemsManager != null)
        {
            // Usa SendMessage para llamar métodos internos
            itemsManager.SendMessage("OnSwitchItem", SendMessageOptions.DontRequireReceiver);
            yield return new WaitForSeconds(0.1f);
            itemsManager.SendMessage("OnSwitchItem", SendMessageOptions.DontRequireReceiver);
            Debug.Log("[Guardian] Player Items reinicializados.");
        }
        else
        {
            // Alternativa: buscar el componente por tipo en toda la jerarquía
            var allManagers = FindObjectsOfType<PlayerItemsManager>();
            foreach (var manager in allManagers)
            {
                manager.gameObject.SetActive(false);
                yield return null;
                manager.gameObject.SetActive(true);
            }
            Debug.Log("[Guardian] Player Items reinicializados via SetActive.");
        }
    }
}