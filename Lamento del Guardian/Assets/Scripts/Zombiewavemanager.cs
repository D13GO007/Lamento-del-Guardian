using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ZombieWaveManager — El Lamento del Guardián
/// Cuenta las muertes de zombies en la escena Raining.
/// Al matar el último zombie dispara la secuencia épica del final.
/// </summary>
public class ZombieWaveManager : MonoBehaviour
{
    [Header("Configuración")]
    public int zombiesParaGanar = 47;
    public string escenaVictoria = "Victory";

    [Header("Secuencia final épica")]
    public GameObject techoCentral;
    public Light luzFinal;
    public AudioSource audioFinal;
    public float duracionDiscurso = 8f;
    public float velocidadFade = 1.5f;

    [Header("Canvas de fade (opcional)")]
    public CanvasGroup panelFadeNegro;

    private int zombiesMuertos = 0;
    private bool finalActivado = false;

    private void Start()
    {
        if (luzFinal != null)
            luzFinal.enabled = false;

        Debug.Log($"[ZombieWaveManager] Inicializado. Esperando {zombiesParaGanar} zombies.");
    }

    /// <summary>
    /// Conectar este método al evento OnDeath de cada zombie.
    /// </summary>
    public void RegistrarZombieMuerto()
    {
        Debug.Log($"[ZombieWaveManager] RegistrarZombieMuerto() llamado. Final activado: {finalActivado}");

        if (finalActivado)
        {
            Debug.Log("[ZombieWaveManager] Final ya activado, ignorando.");
            return;
        }

        zombiesMuertos++;
        ScoreManager.Instance?.RegistrarZombieEliminado();

        Debug.Log($"[ZombieWaveManager] Zombies muertos: {zombiesMuertos}/{zombiesParaGanar}");

        if (zombiesMuertos >= zombiesParaGanar)
        {
            Debug.Log("[ZombieWaveManager] ¡Todos los zombies eliminados! Iniciando secuencia final...");
            finalActivado = true;
            ScoreManager.Instance?.TerminarZona(5);
            StartCoroutine(SecuenciaFinalEpica());
        }
    }

    private IEnumerator SecuenciaFinalEpica()
    {
        Debug.Log("[ZombieWaveManager] Secuencia épica iniciada — pausa dramática...");
        yield return new WaitForSeconds(1.5f);

        // Iluminar techo central
        if (luzFinal != null)
        {
            Debug.Log("[ZombieWaveManager] Encendiendo luz final...");
            luzFinal.enabled = true;
            float intensidadMax = 5f;
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * 0.8f;
                luzFinal.intensity = Mathf.Lerp(0f, intensidadMax, t);
                yield return null;
            }
        }
        else
        {
            Debug.LogWarning("[ZombieWaveManager] Luz final no asignada.");
        }

        if (techoCentral != null)
        {
            Debug.Log("[ZombieWaveManager] Activando techo central...");
            techoCentral.SetActive(true);
        }

        yield return new WaitForSeconds(1f);

        // Reproducir audio final
        if (audioFinal != null)
        {
            Debug.Log("[ZombieWaveManager] Reproduciendo audio final...");
            audioFinal.Play();
        }
        else
        {
            Debug.LogWarning("[ZombieWaveManager] Audio final no asignado.");
        }

        Debug.Log($"[ZombieWaveManager] Esperando {duracionDiscurso} segundos de discurso...");
        yield return new WaitForSeconds(duracionDiscurso);

        Debug.Log("[ZombieWaveManager] Iniciando fade y carga de Victory...");
        yield return StartCoroutine(FadeYCargar());
    }

    private IEnumerator FadeYCargar()
    {
        if (panelFadeNegro != null)
        {
            panelFadeNegro.gameObject.SetActive(true);
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * velocidadFade;
                panelFadeNegro.alpha = Mathf.Clamp01(t);
                yield return null;
            }
        }
        else
        {
            Debug.Log("[ZombieWaveManager] Sin panel fade — cargando Victory directamente.");
            yield return new WaitForSeconds(1f);
        }

        Debug.Log($"[ZombieWaveManager] Cargando escena: {escenaVictoria}");
        SceneManager.LoadScene(escenaVictoria);
    }
}