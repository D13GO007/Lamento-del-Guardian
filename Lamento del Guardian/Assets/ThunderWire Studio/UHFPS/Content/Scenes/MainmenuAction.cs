using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// MainMenuActions — El Lamento del Guardián
/// Acciones del menú principal que requieren lógica personalizada.
///
/// SETUP:
///   1. Add Component → MainMenuActions al GameObject MAINMENU
///   2. Botón Quit → OnClick → MAINMENU → MainMenuActions → CerrarSesionYVolverLogin()
/// </summary>
public class MainMenuActions : MonoBehaviour
{
    [Header("Escenas")]
    public string escenaLogin = "Login";

    /// <summary>
    /// Cierra la sesión del jugador y regresa a la pantalla de Login.
    /// Conectar al botón Quit del menú principal.
    /// </summary>
    public void CerrarSesionYVolverLogin()
    {
        AuthManager.CerrarSesion();
        SceneManager.LoadScene(escenaLogin);
    }
}