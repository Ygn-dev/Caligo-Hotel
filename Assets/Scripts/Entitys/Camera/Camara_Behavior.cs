using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.Universal;

#if UNITY_EDITOR
using UnityEditor;
#endif

public enum ModoCamara
{
    Idle,
    Paneo
}

public enum TipoAccionPalanca
{
    Simple,
    Toggle,
    UnaVez
}

public enum AccionPalanca
{
    Girar,
    Apagar,
    Encender
}

public class Camara_Behavior : MonoBehaviour
{
    // No Editable
    public GameObject lente;
    public Light2D luzCamara;
    public Collider2D triggerDeathZone;

    // Editable
    public ModoCamara modoCamara;

    public bool tienePalanca;
    [HideInInspector] public TipoAccionPalanca tipoAccionPalanca;
    [HideInInspector] public AccionPalanca accionPalanca;

    // MODO PANEO
    [HideInInspector] public float tiempoEsperaPaneo;
    [HideInInspector] public float gradosRotacionPaneo;
    [HideInInspector] public float duracionRotacionPaneo;
    [HideInInspector] public AnimationCurve curvaRotacionPaneo = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    // MODO GIRAR POR PALANCA
    [HideInInspector] public float gradosDeGiro;
    [HideInInspector] public float duracionGiro;
    [HideInInspector] public AnimationCurve curvaGiro = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    // MODO ENCENDER / APAGAR
    [HideInInspector] public float duracionParpadeo = 0.5f;
    [HideInInspector] public float frecuenciaParpadeo = 10f;

    // Variables privadas
    private Player_Respawn playerRespawn;

    private Quaternion rotacionInicialLente;

    private Coroutine corrutinaGiro;
    private Coroutine corrutinaPaneo;
    private Coroutine corrutinaParpadeo;

    private bool estaGirando;
    private bool estaPaneando;

    private bool giroPendiente;
    private bool giroPendienteEsAbsoluto;
    private float gradosGiroPendiente;
    private Quaternion rotacionFinalGiroPendiente;

    private bool toggleSiguienteEncender;
    private bool toggleSiguienteGirar;

    private bool estadoInicialLuz;
    private bool estadoInicialCollider;

    private void Awake()
    {
        if (lente != null)
        {
            rotacionInicialLente = lente.transform.localRotation;
        }

        if (luzCamara != null)
        {
            estadoInicialLuz = luzCamara.enabled;
        }

        if (triggerDeathZone != null)
        {
            estadoInicialCollider = triggerDeathZone.enabled;
        }

        InicializarToggles();
    }

    private void OnEnable()
    {
        InicializarToggles();
        StartCoroutine(WaitForPlayer());
        IniciarModoCamara();
    }

    private IEnumerator WaitForPlayer()
    {
        GameObject player = null;

        while (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            yield return null;
        }

        playerRespawn = player.GetComponent<Player_Respawn>();

        if (playerRespawn != null)
        {
            playerRespawn.DeathEvent += OnPlayerDeath;
            playerRespawn.RespawnEvent += OnPlayerRespawn;
        }
    }

    private void OnDisable()
    {
        if (playerRespawn != null)
        {
            playerRespawn.DeathEvent -= OnPlayerDeath;
            playerRespawn.RespawnEvent -= OnPlayerRespawn;
        }

        DetenerModoYAcciones();
    }

    private void IniciarModoCamara()
    {
        switch (modoCamara)
        {
            case ModoCamara.Idle:
                break;

            case ModoCamara.Paneo:
                IniciarPaneo();
                break;
        }
    }

    private void IniciarPaneo()
    {
        if (lente == null) return;
        if (estaPaneando) return;

        corrutinaPaneo = StartCoroutine(PaneoCamara());
    }

    private IEnumerator PaneoCamara()
    {
        estaPaneando = true;

        while (true)
        {
            // IDA
            Quaternion rotacionInicial = lente.transform.localRotation;
            yield return RotarLente(rotacionInicial, gradosRotacionPaneo, duracionRotacionPaneo, curvaRotacionPaneo);        
            if (tiempoEsperaPaneo > 0f) yield return EsperarPaneo(tiempoEsperaPaneo);
            
            // VUELTA
            rotacionInicial = lente.transform.localRotation;
            yield return RotarLente(rotacionInicial, -gradosRotacionPaneo, duracionRotacionPaneo, curvaRotacionPaneo);
            yield return EjecutarGiroPendienteSiExiste();

            // TIEMPO DE ESPERA
            if (tiempoEsperaPaneo > 0f) yield return EsperarPaneo(tiempoEsperaPaneo);
            yield return EjecutarGiroPendienteSiExiste();
        }
    }

    private IEnumerator EjecutarGiroPendienteSiExiste()
    {
        if (!giroPendiente) yield break;
        if (estaGirando) yield break;
        if (lente == null) yield break;

        bool usarRotacionFinalAbsoluta = giroPendienteEsAbsoluto;
        float grados = gradosGiroPendiente;
        Quaternion rotacionFinalAbsoluta = rotacionFinalGiroPendiente;

        giroPendiente = false;
        giroPendienteEsAbsoluto = false;
        gradosGiroPendiente = 0f;

        estaGirando = true;

        Quaternion rotacionInicial = lente.transform.localRotation;

        Quaternion rotacionFinal = usarRotacionFinalAbsoluta
            ? rotacionFinalAbsoluta
            : rotacionInicial * Quaternion.Euler(0f, 0f, grados);

        float gradosCalculados = usarRotacionFinalAbsoluta 
            ? (rotacionFinalAbsoluta.eulerAngles.z - lente.transform.localRotation.eulerAngles.z) 
            : grados;
        yield return RotarLente(lente.transform.localRotation, gradosCalculados, duracionGiro, curvaGiro);


        estaGirando = false;
    }

    private IEnumerator EsperarPaneo(float duracion)
    {
        float tiempo = 0f;

        while (tiempo < duracion)
        {
            if (giroPendiente) yield break;

            tiempo += Time.deltaTime;
            yield return null;
        }
    }

    private void OnPlayerDeath()
    {
        DetenerModoYAcciones();
    }

    private void OnPlayerRespawn()
    {
        DetenerModoYAcciones();

        if (lente != null)
        {
            lente.transform.localRotation = rotacionInicialLente;
        }

        if (luzCamara != null)
        {
            luzCamara.enabled = estadoInicialLuz;
        }

        if (triggerDeathZone != null)
        {
            triggerDeathZone.enabled = estadoInicialCollider;
        }

        InicializarToggles();
        IniciarModoCamara();
    }

    private void DetenerModoYAcciones()
    {
        if (corrutinaPaneo != null)
        {
            StopCoroutine(corrutinaPaneo);
            corrutinaPaneo = null;
        }

        estaPaneando = false;

        if (corrutinaGiro != null)
        {
            StopCoroutine(corrutinaGiro);
            corrutinaGiro = null;
        }

        estaGirando = false;

        if (corrutinaParpadeo != null)
        {
            StopCoroutine(corrutinaParpadeo);
            corrutinaParpadeo = null;
        }

        giroPendiente = false;
        giroPendienteEsAbsoluto = false;
        gradosGiroPendiente = 0f;
    }

    public void EjecutarAccion()
    {
        if (!tienePalanca) return;

        switch (tipoAccionPalanca)
        {
            case TipoAccionPalanca.Simple:
                EjecutarModoSimple();
                break;

            case TipoAccionPalanca.Toggle:
                EjecutarModoToggle();
                break;

            case TipoAccionPalanca.UnaVez:
                // EjecutarModoUnaVez();
                break;
        }
    }

    private void EjecutarModoSimple()
    {
        switch (accionPalanca)
        {
            case AccionPalanca.Girar:
                IniciarGiro(gradosDeGiro);
                break;

            case AccionPalanca.Encender:
                IniciarCambioEncendido(true);
                break;

            case AccionPalanca.Apagar:
                IniciarCambioEncendido(false);
                break;
        }
    }

    private void EjecutarModoToggle()
    {
        switch (accionPalanca)
        {
            case AccionPalanca.Girar:
                EjecutarToggleGirar();
                break;

            case AccionPalanca.Encender:
            case AccionPalanca.Apagar:
                IniciarCambioEncendido(toggleSiguienteEncender);
                toggleSiguienteEncender = !toggleSiguienteEncender;
                break;
        }
    }

    private void InicializarToggles()
    {
        InicializarToggleEncendidoApagado();

        // En Toggle Girar, la primera activación siempre gira hacia los grados configurados.
        toggleSiguienteGirar = true;
    }

    private void InicializarToggleEncendidoApagado()
    {
        switch (accionPalanca)
        {
            case AccionPalanca.Apagar:
                // Si la acción base es Apagar, la primera activación apaga.
                toggleSiguienteEncender = false;
                break;

            case AccionPalanca.Encender:
                // Si la acción base es Encender, la primera activación enciende.
                toggleSiguienteEncender = true;
                break;
        }
    }

    private void EjecutarToggleGirar()
    {
        if (lente == null) return;

        Quaternion rotacionFinal = toggleSiguienteGirar
            ? rotacionInicialLente * Quaternion.Euler(0f, 0f, gradosDeGiro)
            : rotacionInicialLente;

        bool giroAceptado = IniciarGiroHacia(rotacionFinal);

        if (giroAceptado)
        {
            toggleSiguienteGirar = !toggleSiguienteGirar;
        }
    }

    private void IniciarCambioEncendido(bool encender)
    {
        if (corrutinaParpadeo != null)
        {
            StopCoroutine(corrutinaParpadeo);
            corrutinaParpadeo = null;
        }

        corrutinaParpadeo = StartCoroutine(ParpadearCamara(encender));
    }

    private IEnumerator ParpadearCamara(bool estadoFinalEncendido)
    {
        if (luzCamara == null)
        {
            AplicarEstadoCamara(estadoFinalEncendido);
            corrutinaParpadeo = null;
            yield break;
        }

        if (duracionParpadeo <= 0f || frecuenciaParpadeo <= 0f)
        {
            AplicarEstadoCamara(estadoFinalEncendido);
            corrutinaParpadeo = null;
            yield break;
        }

        float tiempo = 0f;
        float intervalo = 1f / frecuenciaParpadeo;
        float siguienteCambio = 0f;

        while (tiempo < duracionParpadeo)
        {
            if (tiempo >= siguienteCambio)
            {
                luzCamara.enabled = !luzCamara.enabled;
                siguienteCambio += intervalo;
            }

            tiempo += Time.deltaTime;
            yield return null;
        }

        AplicarEstadoCamara(estadoFinalEncendido);

        corrutinaParpadeo = null;
    }

    private void AplicarEstadoCamara(bool encendida)
    {
        if (luzCamara != null)
        {
            luzCamara.enabled = encendida;
        }

        if (triggerDeathZone != null)
        {
            triggerDeathZone.enabled = encendida;
        }
    }

    private void IniciarGiro(float grados)
    {
        if (lente == null) return;

        if (estaPaneando)
        {
            giroPendiente = true;
            giroPendienteEsAbsoluto = false;
            gradosGiroPendiente += grados;
            return;
        }

        if (estaGirando) return;

        corrutinaGiro = StartCoroutine(GirarCamara(grados));
    }

    private bool IniciarGiroHacia(Quaternion rotacionFinal)
    {
        if (lente == null) return false;

        if (estaPaneando)
        {
            giroPendiente = true;
            giroPendienteEsAbsoluto = true;
            rotacionFinalGiroPendiente = rotacionFinal;
            gradosGiroPendiente = 0f;
            return true;
        }

        if (estaGirando) return false;

        corrutinaGiro = StartCoroutine(GirarCamaraHacia(rotacionFinal));
        return true;
    }

    private IEnumerator GirarCamara(float grados)
    {
        estaGirando = true;

        Quaternion rotacionInicial = lente.transform.localRotation;
        Quaternion rotacionFinal = rotacionInicial * Quaternion.Euler(0f, 0f, grados);

        yield return RotarLente(rotacionInicial, grados, duracionGiro, curvaGiro);

        estaGirando = false;
        corrutinaGiro = null;
    }

    private IEnumerator GirarCamaraHacia(Quaternion rotacionFinal)
    {
        estaGirando = true;

        Quaternion rotacionInicial = lente.transform.localRotation;
        float deltaZ = rotacionFinal.eulerAngles.z - rotacionInicial.eulerAngles.z;
        yield return RotarLente(rotacionInicial, deltaZ, duracionGiro, curvaGiro);

        estaGirando = false;
        corrutinaGiro = null;
    }

private IEnumerator RotarLente(
    Quaternion rotacionInicial,
    float gradosARotar,
    float duracion,
    AnimationCurve curva
)
{
    if (lente == null) yield break;

    if (duracion <= 0f)
    {
        lente.transform.localRotation = rotacionInicial * Quaternion.Euler(0f, 0f, gradosARotar);
        yield break;
    }

    float anguloInicial = rotacionInicial.eulerAngles.z;
    float tiempo = 0f;

    while (tiempo < duracion)
    {
        tiempo += Time.deltaTime;

        float progreso = Mathf.Clamp01(tiempo / duracion);
        float progresoConCurva = curva != null ? curva.Evaluate(progreso) : progreso;

        lente.transform.localRotation = Quaternion.Euler(0f, 0f, anguloInicial + gradosARotar * progresoConCurva);

        yield return null;
    }

    lente.transform.localRotation = rotacionInicial * Quaternion.Euler(0f, 0f, gradosARotar);
}
}

#if UNITY_EDITOR
[CustomEditor(typeof(Camara_Behavior))]
public class Camara_Behavior_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedProperty modoCamara = serializedObject.FindProperty("modoCamara");

        SerializedProperty tienePalanca = serializedObject.FindProperty("tienePalanca");
        SerializedProperty tipoAccionPalanca = serializedObject.FindProperty("tipoAccionPalanca");
        SerializedProperty accionPalanca = serializedObject.FindProperty("accionPalanca");

        SerializedProperty gradosRotacionPaneo = serializedObject.FindProperty("gradosRotacionPaneo");
        SerializedProperty duracionRotacionPaneo = serializedObject.FindProperty("duracionRotacionPaneo");
        SerializedProperty curvaRotacionPaneo = serializedObject.FindProperty("curvaRotacionPaneo");
        SerializedProperty tiempoEsperaPaneo = serializedObject.FindProperty("tiempoEsperaPaneo");

        SerializedProperty gradosDeGiro = serializedObject.FindProperty("gradosDeGiro");
        SerializedProperty duracionGiro = serializedObject.FindProperty("duracionGiro");
        SerializedProperty curvaGiro = serializedObject.FindProperty("curvaGiro");

        SerializedProperty duracionParpadeo = serializedObject.FindProperty("duracionParpadeo");
        SerializedProperty frecuenciaParpadeo = serializedObject.FindProperty("frecuenciaParpadeo");

        SerializedProperty lente = serializedObject.FindProperty("lente");
        SerializedProperty luzCamara = serializedObject.FindProperty("luzCamara");
        SerializedProperty triggerDeathZone = serializedObject.FindProperty("triggerDeathZone");

        EditorGUILayout.LabelField("Editable", EditorStyles.boldLabel);

        EditorGUI.indentLevel++;

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(
            modoCamara,
            new GUIContent("Modo cámara")
        );

        if ((ModoCamara)modoCamara.enumValueIndex == ModoCamara.Paneo)
        {
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Configuración de paneo", EditorStyles.boldLabel);

            EditorGUI.indentLevel++;

            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.PropertyField(
                gradosRotacionPaneo,
                new GUIContent("Grados de rotación")
            );

            EditorGUILayout.PropertyField(
                duracionRotacionPaneo,
                new GUIContent("Duración de rotación")
            );

            EditorGUILayout.PropertyField(
                curvaRotacionPaneo,
                new GUIContent("Curva de rotación")
            );

            EditorGUILayout.PropertyField(
                tiempoEsperaPaneo,
                new GUIContent("Tiempo de espera")
            );

            EditorGUILayout.EndVertical();

            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Configuración de palanca", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(
            tienePalanca,
            new GUIContent("¿Tiene palanca?")
        );

        if (tienePalanca.boolValue)
        {
            EditorGUI.indentLevel++;

            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.PropertyField(
                tipoAccionPalanca,
                new GUIContent("Tipo de acción")
            );

            EditorGUILayout.PropertyField(
                accionPalanca,
                new GUIContent("Acción")
            );

            AccionPalanca accionSeleccionada = (AccionPalanca)accionPalanca.enumValueIndex;
            TipoAccionPalanca tipoSeleccionado = (TipoAccionPalanca)tipoAccionPalanca.enumValueIndex;

            if (accionSeleccionada == AccionPalanca.Girar)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(
                    gradosDeGiro,
                    new GUIContent("Grados de giro")
                );

                EditorGUILayout.PropertyField(
                    duracionGiro,
                    new GUIContent("Duración de giro")
                );

                EditorGUILayout.PropertyField(
                    curvaGiro,
                    new GUIContent("Curva de giro")
                );

                if (tipoSeleccionado == TipoAccionPalanca.Toggle)
                {
                    EditorGUILayout.HelpBox(
                        "En Toggle, la primera activación girará la cámara los grados configurados. La siguiente activación volverá a la rotación inicial, como un paneo manual.",
                        MessageType.Info
                    );
                }

                EditorGUI.indentLevel--;
            }

            if (
                accionSeleccionada == AccionPalanca.Encender ||
                accionSeleccionada == AccionPalanca.Apagar
            )
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.Space();

                EditorGUILayout.LabelField(
                    "Configuración de parpadeo",
                    EditorStyles.boldLabel
                );

                EditorGUILayout.PropertyField(
                    duracionParpadeo,
                    new GUIContent("Duración de parpadeo")
                );

                EditorGUILayout.PropertyField(
                    frecuenciaParpadeo,
                    new GUIContent("Frecuencia de parpadeo")
                );

                if (tipoSeleccionado == TipoAccionPalanca.Toggle)
                {
                    string primeraAccion = accionSeleccionada == AccionPalanca.Apagar
                        ? "Apagar"
                        : "Encender";

                    string segundaAccion = accionSeleccionada == AccionPalanca.Apagar
                        ? "Encender"
                        : "Apagar";

                    EditorGUILayout.HelpBox(
                        "En Toggle, la primera activación será: " + primeraAccion + ". La siguiente será: " + segundaAccion + ". Luego seguirá alternando.",
                        MessageType.Info
                    );
                }

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndVertical();

            EditorGUI.indentLevel--;
        }

        EditorGUI.indentLevel--;

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("No editable", EditorStyles.boldLabel);

        EditorGUI.indentLevel++;

        EditorGUILayout.PropertyField(
            lente,
            new GUIContent("Lente")
        );

        EditorGUILayout.PropertyField(
            luzCamara,
            new GUIContent("Luz cámara")
        );

        EditorGUILayout.PropertyField(
            triggerDeathZone,
            new GUIContent("Trigger del DeathZone")
        );

        EditorGUI.indentLevel--;

        serializedObject.ApplyModifiedProperties();
    }
}
#endif