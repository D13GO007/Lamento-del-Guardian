using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

/// <summary>
/// AuthManager — El Lamento del Guardián
/// Maneja login, registro y recuperación de contraseña con Supabase Auth nativo.
/// Garantiza que el perfil siempre exista en la tabla perfiles antes de continuar.
/// </summary>
public class AuthManager : MonoBehaviour
{
    private const string SUPABASE_URL = "https://tisltxhjrvsfmvwpxvku.supabase.co";
    private const string SUPABASE_ANON_KEY = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InRpc2x0eGhqcnZzZm12d3B4dmt1Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3ODAzNjQ0MjUsImV4cCI6MjA5NTk0MDQyNX0.aw8iMn3KudPXu9_mWsAtNJAUz6fPpVFu5Y2BGAqZPMc";

    public static string JwtToken  => SavePathManager.JwtToken;
    public static string UserId    => SavePathManager.UserId;
    public static string Username  => SavePathManager.Username;

    [Header("Paneles")]
    public GameObject panelLogin;
    public GameObject panelRegistro;
    public GameObject panelRecuperar;

    [Header("Campos Login")]
    public TMPro.TMP_InputField loginEmail;
    public TMPro.TMP_InputField loginPassword;

    [Header("Campos Registro")]
    public TMPro.TMP_InputField registroEmail;
    public TMPro.TMP_InputField registroUsername;
    public TMPro.TMP_InputField registroPassword;
    public TMPro.TMP_InputField registroConfirmarPassword;

    [Header("Campos Recuperar")]
    public TMPro.TMP_InputField recuperarEmail;
    public TMPro.TMP_InputField recuperarCodigoOTP;
    public TMPro.TMP_InputField recuperarNuevaPassword;
    public TMPro.TMP_InputField recuperarConfirmarPassword;

    [Header("Mensajes")]
    public TMPro.TMP_Text textoMensajeLogin;
    public TMPro.TMP_Text textoMensajeRegistro;
    public TMPro.TMP_Text textoMensajeRecuperar;

    [Header("Escenas")]
    public string escenaMenuPrincipal = "MainMenu";

    private bool codigoEnviado = false;

    private void Start()
    {
        MostrarPanelLogin();
    }

    // =============================================
    // NAVEGACIÓN
    // =============================================
    public void MostrarPanelLogin()
    {
        if (panelLogin != null)     panelLogin.SetActive(true);
        if (panelRegistro != null)  panelRegistro.SetActive(false);
        if (panelRecuperar != null) panelRecuperar.SetActive(false);
        codigoEnviado = false;
        LimpiarMensajeLogin();
    }

    public void MostrarPanelRegistro()
    {
        if (panelLogin != null)     panelLogin.SetActive(false);
        if (panelRegistro != null)  panelRegistro.SetActive(true);
        if (panelRecuperar != null) panelRecuperar.SetActive(false);
        LimpiarMensajeRegistro();
    }

    public void MostrarPanelRecuperar()
    {
        if (panelLogin != null)     panelLogin.SetActive(false);
        if (panelRegistro != null)  panelRegistro.SetActive(false);
        if (panelRecuperar != null) panelRecuperar.SetActive(true);
        codigoEnviado = false;
        LimpiarMensajeRecuperar();
    }

    // =============================================
    // LOGIN
    // =============================================
    public void BotonLogin()
    {
        string email    = loginEmail?.text.Trim();
        string password = loginPassword?.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            MostrarErrorLogin("Por favor completa todos los campos.");
            return;
        }

        StartCoroutine(LoginCoroutine(email, password));
    }

    private IEnumerator LoginCoroutine(string email, string password)
    {
        MostrarMensajeLogin("Iniciando sesión...", Color.yellow);

        string url  = $"{SUPABASE_URL}/auth/v1/token?grant_type=password";
        string json = $"{{\"email\":\"{email}\",\"password\":\"{password}\"}}";

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            request.uploadHandler   = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("apikey", SUPABASE_ANON_KEY);

            yield return request.SendWebRequest();

            Debug.Log($"[AuthManager] LOGIN - Código: {request.responseCode} | Respuesta: {request.downloadHandler.text}");

            if (request.result == UnityWebRequest.Result.Success)
            {
                AuthResponse response = JsonUtility.FromJson<AuthResponse>(request.downloadHandler.text);
                string tempToken  = response.access_token;
                string tempUserId = response.user.id;
                string tempEmail  = response.user.email;

                Debug.Log($"[AuthManager] LOGIN exitoso — UserId: {tempUserId} | Email: {tempEmail}");

                // Buscar perfil existente
                string tempUsername = "";
                yield return StartCoroutine(ObtenerUsernameCoroutine(tempUserId, tempToken, (u) => tempUsername = u));

                Debug.Log($"[AuthManager] Username obtenido de perfiles: '{tempUsername}'");

                // Si no tiene perfil, crearlo con el username del registro
                // Usamos la parte antes del @ como fallback
                if (string.IsNullOrEmpty(tempUsername))
                {
                    Debug.LogWarning($"[AuthManager] Perfil no encontrado para {tempUserId}. Buscando username en PlayerPrefs...");
                    // Recuperar el username que el jugador escribió al registrarse
                    tempUsername = PlayerPrefs.GetString("PendingUsername_" + tempEmail, tempEmail.Split('@')[0]);
                    PlayerPrefs.DeleteKey("PendingUsername_" + tempEmail);
                    Debug.Log($"[AuthManager] Username recuperado: '{tempUsername}'");

                    bool perfilCreado = false;
                    yield return StartCoroutine(CrearPerfilCoroutine(tempUserId, tempUsername, tempToken, (ok) => perfilCreado = ok));

                    if (perfilCreado)
                        Debug.Log($"[AuthManager] Perfil creado exitosamente con username: '{tempUsername}'");
                    else
                    {
                        Debug.LogError("[AuthManager] Error al crear perfil.");
                        MostrarErrorLogin("Error al crear el perfil. Intenta de nuevo.");
                        yield break;
                    }
                }

                Debug.Log($"[AuthManager] Iniciando sesión con username: '{tempUsername}'");

                // Guardar sesión en SavePathManager
                if (SavePathManager.Instance != null)
                    SavePathManager.Instance.IniciarSesion(tempToken, tempUserId, tempUsername);
                else
                    Debug.LogError("[AuthManager] SavePathManager no encontrado.");

                Debug.Log($"[AuthManager] SavePathManager.Username: '{SavePathManager.Username}'");

                MostrarExitoLogin($"¡Bienvenido, {tempUsername}!");
                yield return new WaitForSeconds(1f);
                SceneManager.LoadScene(escenaMenuPrincipal);
            }
            else
            {
                Debug.LogError($"[AuthManager] LOGIN fallido — {request.downloadHandler.text}");
                MostrarErrorLogin(ObtenerMensajeError(request.downloadHandler.text));
            }
        }
    }

    // =============================================
    // REGISTRO
    // =============================================
    public void BotonRegistro()
    {
        string email     = registroEmail?.text.Trim();
        string username  = registroUsername?.text.Trim();
        string password  = registroPassword?.text;
        string confirmar = registroConfirmarPassword?.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(username) ||
            string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmar))
        {
            MostrarErrorRegistro("Por favor completa todos los campos.");
            return;
        }

        if (password != confirmar)
        {
            MostrarErrorRegistro("Las contraseñas no coinciden.");
            return;
        }

        if (password.Length < 6)
        {
            MostrarErrorRegistro("La contraseña debe tener al menos 6 caracteres.");
            return;
        }

        if (username.Length < 3)
        {
            MostrarErrorRegistro("El nombre de usuario debe tener al menos 3 caracteres.");
            return;
        }

        StartCoroutine(RegistroCoroutine(email, username, password));
    }

    private IEnumerator RegistroCoroutine(string email, string username, string password)
    {
        MostrarMensajeRegistro("Creando cuenta...", Color.yellow);

        // Guardar username temporalmente en PlayerPrefs para recuperarlo al confirmar
        PlayerPrefs.SetString("PendingUsername_" + email, username);
        PlayerPrefs.Save();
        Debug.Log($"[AuthManager] Username '{username}' guardado en PlayerPrefs para email '{email}'");

        string url  = $"{SUPABASE_URL}/auth/v1/signup";
        string json = $"{{\"email\":\"{email}\",\"password\":\"{password}\"}}";

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            request.uploadHandler   = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("apikey", SUPABASE_ANON_KEY);

            yield return request.SendWebRequest();

            Debug.Log($"[AuthManager] REGISTRO - Código: {request.responseCode} | Respuesta: {request.downloadHandler.text}");

            if (request.result == UnityWebRequest.Result.Success)
            {
                AuthResponse response = JsonUtility.FromJson<AuthResponse>(request.downloadHandler.text);

                // Sin token = requiere confirmación de email
                if (string.IsNullOrEmpty(response.access_token))
                {
                    Debug.Log($"[AuthManager] Registro exitoso — requiere confirmación de email. Username '{username}' guardado en PlayerPrefs.");
                    MostrarExitoRegistro("¡Cuenta creada! Revisa tu correo para confirmar.");
                    yield break;
                }

                // Con token = crear perfil inmediatamente
                Debug.Log($"[AuthManager] Registro con token inmediato. Creando perfil con username: '{username}'");
                MostrarMensajeRegistro("Guardando perfil...", Color.yellow);

                bool perfilCreado = false;
                yield return StartCoroutine(CrearPerfilCoroutine(response.user.id, username, response.access_token, (ok) => perfilCreado = ok));

                if (perfilCreado)
                {
                    Debug.Log($"[AuthManager] Perfil creado con username: '{username}'");
                    PlayerPrefs.DeleteKey("PendingUsername_" + email);
                    MostrarExitoRegistro("¡Cuenta creada exitosamente!");
                    yield return new WaitForSeconds(1.5f);
                    MostrarPanelLogin();
                }
                else
                {
                    Debug.LogError("[AuthManager] Error al crear perfil tras registro.");
                    MostrarErrorRegistro("Error al guardar el perfil. Intenta iniciar sesión.");
                }
            }
            else
            {
                Debug.LogError($"[AuthManager] REGISTRO fallido — {request.downloadHandler.text}");
                MostrarErrorRegistro(ObtenerMensajeError(request.downloadHandler.text));
            }
        }
    }

    // =============================================
    // CREAR PERFIL — con callback de éxito
    // =============================================
    private IEnumerator CrearPerfilCoroutine(string userId, string username, string token, Action<bool> callback)
    {
        Debug.Log($"[AuthManager] Intentando crear perfil — UserId: {userId} | Username: '{username}'");

        string url  = $"{SUPABASE_URL}/rest/v1/perfiles";
        string json = $"{{\"id\":\"{userId}\",\"username\":\"{username}\"}}";

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            request.uploadHandler   = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("apikey", SUPABASE_ANON_KEY);
            request.SetRequestHeader("Authorization", $"Bearer {token}");
            request.SetRequestHeader("Prefer", "return=minimal");

            yield return request.SendWebRequest();

            Debug.Log($"[AuthManager] CREAR PERFIL - Código: {request.responseCode} | Respuesta: {request.downloadHandler.text}");

            // 201 = creado, 409 = ya existe (ambos son OK)
            bool exito = request.responseCode == 201 || request.responseCode == 409;
            Debug.Log($"[AuthManager] Perfil creado: {exito}");
            callback?.Invoke(exito);
        }
    }

    // =============================================
    // RECUPERAR CONTRASEÑA — PASO 1: ENVIAR OTP
    // =============================================
    public void BotonEnviarCodigo()
    {
        string email = recuperarEmail?.text.Trim();

        if (string.IsNullOrEmpty(email))
        {
            MostrarErrorRecuperar("Escribe tu email para recibir el código.");
            return;
        }

        StartCoroutine(EnviarOTPCoroutine(email));
    }

    private IEnumerator EnviarOTPCoroutine(string email)
    {
        MostrarMensajeRecuperar("Enviando código a tu correo...", Color.yellow);

        string url  = $"{SUPABASE_URL}/auth/v1/otp";
        string json = $"{{\"email\":\"{email}\",\"create_user\":false}}";

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            request.uploadHandler   = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("apikey", SUPABASE_ANON_KEY);

            yield return request.SendWebRequest();

            Debug.Log($"[AuthManager] OTP - Código: {request.responseCode} | Respuesta: {request.downloadHandler.text}");

            if (request.result == UnityWebRequest.Result.Success)
            {
                codigoEnviado = true;
                MostrarExitoRecuperar("¡Código enviado! Revisa tu correo e ingrésalo abajo.");
            }
            else
            {
                MostrarErrorRecuperar(ObtenerMensajeError(request.downloadHandler.text));
            }
        }
    }

    // =============================================
    // RECUPERAR CONTRASEÑA — PASO 2: VERIFICAR Y CAMBIAR
    // =============================================
    public void BotonCambiarPassword()
    {
        if (!codigoEnviado)
        {
            MostrarErrorRecuperar("Primero solicita el código de verificación.");
            return;
        }

        string email         = recuperarEmail?.text.Trim();
        string codigo        = recuperarCodigoOTP?.text.Trim();
        string nuevaPassword = recuperarNuevaPassword?.text;
        string confirmar     = recuperarConfirmarPassword?.text;

        if (string.IsNullOrEmpty(codigo))
        {
            MostrarErrorRecuperar("Ingresa el código que recibiste en tu correo.");
            return;
        }

        if (string.IsNullOrEmpty(nuevaPassword) || string.IsNullOrEmpty(confirmar))
        {
            MostrarErrorRecuperar("Por favor completa todos los campos.");
            return;
        }

        if (nuevaPassword != confirmar)
        {
            MostrarErrorRecuperar("Las contraseñas no coinciden.");
            return;
        }

        if (nuevaPassword.Length < 6)
        {
            MostrarErrorRecuperar("La contraseña debe tener al menos 6 caracteres.");
            return;
        }

        StartCoroutine(VerificarOTPyCambiarCoroutine(email, codigo, nuevaPassword));
    }

    private IEnumerator VerificarOTPyCambiarCoroutine(string email, string codigo, string nuevaPassword)
    {
        MostrarMensajeRecuperar("Verificando código...", Color.yellow);

        string url  = $"{SUPABASE_URL}/auth/v1/verify";
        string json = $"{{\"email\":\"{email}\",\"token\":\"{codigo}\",\"type\":\"email\"}}";

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            request.uploadHandler   = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("apikey", SUPABASE_ANON_KEY);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                AuthResponse response = JsonUtility.FromJson<AuthResponse>(request.downloadHandler.text);
                yield return StartCoroutine(ActualizarPasswordCoroutine(response.access_token, nuevaPassword));
            }
            else
            {
                MostrarErrorRecuperar("Código incorrecto o expirado. Solicita uno nuevo.");
            }
        }
    }

    private IEnumerator ActualizarPasswordCoroutine(string token, string nuevaPassword)
    {
        MostrarMensajeRecuperar("Actualizando contraseña...", Color.yellow);

        string url  = $"{SUPABASE_URL}/auth/v1/user";
        string json = $"{{\"password\":\"{nuevaPassword}\"}}";

        using (UnityWebRequest request = new UnityWebRequest(url, "PUT"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            request.uploadHandler   = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("apikey", SUPABASE_ANON_KEY);
            request.SetRequestHeader("Authorization", $"Bearer {token}");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                MostrarExitoRecuperar("¡Contraseña actualizada! Ya puedes iniciar sesión.");
                yield return new WaitForSeconds(2f);
                MostrarPanelLogin();
            }
            else
            {
                MostrarErrorRecuperar("Error al actualizar la contraseña. Intenta de nuevo.");
            }
        }
    }

    // =============================================
    // OBTENER USERNAME
    // =============================================
    private IEnumerator ObtenerUsernameCoroutine(string userId, string token, Action<string> callback)
    {
        string url = $"{SUPABASE_URL}/rest/v1/perfiles?id=eq.{userId}&select=username";

        Debug.Log($"[AuthManager] Consultando perfil para userId: {userId}");

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("apikey", SUPABASE_ANON_KEY);
            request.SetRequestHeader("Authorization", $"Bearer {token}");

            yield return request.SendWebRequest();

            Debug.Log($"[AuthManager] OBTENER USERNAME - Código: {request.responseCode} | Respuesta: {request.downloadHandler.text}");

            string username = "";
            if (request.result == UnityWebRequest.Result.Success)
            {
                string raw = request.downloadHandler.text;
                int idx = raw.IndexOf("\"username\":\"");
                if (idx >= 0)
                {
                    int start = idx + 12;
                    int end   = raw.IndexOf("\"", start);
                    username  = raw.Substring(start, end - start);
                    Debug.Log($"[AuthManager] Username encontrado: '{username}'");
                }
                else
                {
                    Debug.LogWarning($"[AuthManager] Username no encontrado en respuesta: {raw}");
                }
            }
            callback?.Invoke(username);
        }
    }

    // =============================================
    // LOGOUT
    // =============================================
    public static void CerrarSesion()
    {
        if (SavePathManager.Instance != null)
            SavePathManager.Instance.CerrarSesion();
    }

    // =============================================
    // MENSAJES LOGIN
    // =============================================
    private void MostrarMensajeLogin(string mensaje, Color color)
    {
        if (textoMensajeLogin != null) { textoMensajeLogin.text = mensaje; textoMensajeLogin.color = color; }
        Debug.Log($"[AuthManager] {mensaje}");
    }
    private void MostrarErrorLogin(string msg)  => MostrarMensajeLogin(msg, Color.red);
    private void MostrarExitoLogin(string msg)  => MostrarMensajeLogin(msg, Color.green);
    private void LimpiarMensajeLogin()          { if (textoMensajeLogin != null) textoMensajeLogin.text = ""; }

    // =============================================
    // MENSAJES REGISTRO
    // =============================================
    private void MostrarMensajeRegistro(string mensaje, Color color)
    {
        if (textoMensajeRegistro != null) { textoMensajeRegistro.text = mensaje; textoMensajeRegistro.color = color; }
        Debug.Log($"[AuthManager] {mensaje}");
    }
    private void MostrarErrorRegistro(string msg)  => MostrarMensajeRegistro(msg, Color.red);
    private void MostrarExitoRegistro(string msg)  => MostrarMensajeRegistro(msg, Color.green);
    private void LimpiarMensajeRegistro()          { if (textoMensajeRegistro != null) textoMensajeRegistro.text = ""; }

    // =============================================
    // MENSAJES RECUPERAR
    // =============================================
    private void MostrarMensajeRecuperar(string mensaje, Color color)
    {
        if (textoMensajeRecuperar != null) { textoMensajeRecuperar.text = mensaje; textoMensajeRecuperar.color = color; }
        Debug.Log($"[AuthManager] {mensaje}");
    }
    private void MostrarErrorRecuperar(string msg)  => MostrarMensajeRecuperar(msg, Color.red);
    private void MostrarExitoRecuperar(string msg)  => MostrarMensajeRecuperar(msg, Color.green);
    private void LimpiarMensajeRecuperar()          { if (textoMensajeRecuperar != null) textoMensajeRecuperar.text = ""; }

    // =============================================
    // ERRORES
    // =============================================
    private string ObtenerMensajeError(string responseText)
    {
        if (responseText.Contains("Invalid login credentials"))   return "Email o contraseña incorrectos.";
        if (responseText.Contains("User already registered"))     return "Este email ya está registrado.";
        if (responseText.Contains("Password should be at least")) return "La contraseña debe tener al menos 6 caracteres.";
        if (responseText.Contains("Unable to validate email"))    return "El formato del email no es válido.";
        if (responseText.Contains("over_email_send_rate_limit"))  return "Demasiados intentos. Espera un momento.";
        if (responseText.Contains("Token has expired") || responseText.Contains("otp_expired")) return "El código expiró. Solicita uno nuevo.";
        if (responseText.Contains("email_not_confirmed"))         return "Debes confirmar tu correo antes de iniciar sesión.";
        return "Error de conexión. Intenta de nuevo.";
    }

    // =============================================
    // CLASES JSON
    // =============================================
    [Serializable]
    private class AuthResponse
    {
        public string access_token;
        public string refresh_token;
        public UserData user;
    }

    [Serializable]
    private class UserData
    {
        public string id;
        public string email;
    }
}