using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.Universal;

#if UNITY_EDITOR
using UnityEditor;
#endif

public enum ModoCamara
{
    Idle,
    Paneo,
    GiroConstante
}

public enum TipoAccionPalanca
{
    Normal,
    Toggle,
    UnaVez
}

public enum AccionPalanca
{
    Girar,
    Apagar,
    Encender,
    InvertirGiro,
    ApagarTemporal,
    EncenderTemporal
}

public class Camara_Behavior : MonoBehaviour
{
    // No Editable
    public GameObject lente;

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

    // MODO GIRO CONSTANTE
    [HideInInspector] public bool giroConstanteHaciaDerecha = true;
    [HideInInspector] public float duracionVueltaGiroConstante = 2f;
    [HideInInspector] public AnimationCurve curvaGiroConstante = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    // MODO GIRAR POR PALANCA
    [HideInInspector] public float gradosDeGiro;
    [HideInInspector] public float duracionGiro;
    [HideInInspector] public AnimationCurve curvaGiro = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    // MODO ENCENDER / APAGAR
    [HideInInspector] public float duracionParpadeo = 0.5f;
    [HideInInspector] public float frecuenciaParpadeo = 10f;

    // MODO ENCENDER / APAGAR TEMPORAL
    [HideInInspector] public float duracionEncendidoTemporal = 2f;

    // Variables privadas
    private Light2D luzCamara;
    private Collider2D triggerDeathZone;

    private Player_Respawn playerRespawn;
    private AudioSource sonidoGirando;

    private Quaternion rotacionInicialLente;

    private Coroutine corrutinaGiro;
    private Coroutine corrutinaPaneo;
    private Coroutine corrutinaGiroConstante;
    private Coroutine corrutinaParpadeo;
    private Coroutine corrutinaCambioTemporal;

    private bool estaGirando;
    private bool estaPaneando;
    private bool estaGirandoConstante;

    private bool giroPendiente;
    private bool giroPendienteEsAbsoluto;
    private float gradosGiroPendiente;
    private Quaternion rotacionFinalGiroPendiente;

    private bool invertirGiroPaneoPendiente;

    private bool toggleSiguienteEncender;
    private bool toggleSiguienteGirar;

    private bool accionUnaVezUsada;

    private bool estadoInicialLuz;
    private bool estadoInicialCollider;
    private bool giroConstanteInicial;
    private float gradosRotacionPaneoInicial;

    private void Awake()
    {
        luzCamara = GetComponentInChildren<Light2D>();
        triggerDeathZone = luzCamara.GetComponentInChildren<Collider2D>();

        giroConstanteInicial = giroConstanteHaciaDerecha;
        gradosRotacionPaneoInicial = gradosRotacionPaneo;

        if (lente != null)
        {
            rotacionInicialLente = lente.transform.localRotation;
            estadoInicialLuz = luzCamara.enabled;
            estadoInicialCollider = triggerDeathZone.enabled;
            InicializarEstadosAcciones();
        }
        else
        {
            Debug.LogWarning("No se ha asignado la lente de la cámara en " + gameObject.name + ". La cámara no funcionará correctamente.");
        }
    }

    private void OnEnable()
    {
        InicializarEstadosAcciones();
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

            case ModoCamara.GiroConstante:
                IniciarGiroConstante();
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
            EjecutarInversionPaneoPendienteSiExiste();

            // IDA
            Quaternion rotacionInicial = lente.transform.localRotation;

            IniciarSonidoGiro();

            yield return RotarLente(
                rotacionInicial,
                gradosRotacionPaneo,
                duracionRotacionPaneo,
                curvaRotacionPaneo
            );

            DetenerSonidoGiro();

            if (tiempoEsperaPaneo > 0f)
                yield return EsperarPaneo(tiempoEsperaPaneo);

            // VUELTA
            rotacionInicial = lente.transform.localRotation;

            IniciarSonidoGiro();

            yield return RotarLente(
                rotacionInicial,
                -gradosRotacionPaneo,
                duracionRotacionPaneo,
                curvaRotacionPaneo
            );

            DetenerSonidoGiro();

            yield return EjecutarGiroPendienteSiExiste();

            if (tiempoEsperaPaneo > 0f)
                yield return EsperarPaneo(tiempoEsperaPaneo);

            yield return EjecutarGiroPendienteSiExiste();
        }
    }

    private void EjecutarInversionPaneoPendienteSiExiste()
    {
        if (!invertirGiroPaneoPendiente) return;

        gradosRotacionPaneo *= -1f;
        invertirGiroPaneoPendiente = false;
    }

    private void IniciarGiroConstante()
    {
        if (lente == null) return;
        if (estaGirandoConstante) return;

        corrutinaGiroConstante = StartCoroutine(GiroConstanteCamara());
    }

    private IEnumerator GiroConstanteCamara()
    {
        estaGirandoConstante = true;
        IniciarSonidoGiro(true);

        while (true)
        {
            Quaternion rotacionInicial = lente.transform.localRotation;

            // En Unity 2D:
            // -360 normalmente se ve como giro horario / derecha.
            //  360 normalmente se ve como giro antihorario / izquierda.
            float grados = giroConstanteHaciaDerecha ? -360f : 360f;

            yield return RotarLente(
                rotacionInicial,
                grados,
                duracionVueltaGiroConstante,
                curvaGiroConstante
            );
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
            ? Mathf.DeltaAngle(lente.transform.localRotation.eulerAngles.z, rotacionFinal.eulerAngles.z)
            : grados;

        yield return RotarLente(
            lente.transform.localRotation,
            gradosCalculados,
            duracionGiro,
            curvaGiro
        );

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

        InicializarEstadosAcciones();
        giroConstanteHaciaDerecha = giroConstanteInicial;
        gradosRotacionPaneo = gradosRotacionPaneoInicial;
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

        if (corrutinaGiroConstante != null)
        {
            StopCoroutine(corrutinaGiroConstante);
            corrutinaGiroConstante = null;
        }

        estaGirandoConstante = false;

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

        if (corrutinaCambioTemporal != null)
        {
            StopCoroutine(corrutinaCambioTemporal);
            corrutinaCambioTemporal = null;
        }

        giroPendiente = false;
        giroPendienteEsAbsoluto = false;
        gradosGiroPendiente = 0f;
        invertirGiroPaneoPendiente = false;
        DetenerSonidoGiro();
    }

    public void EjecutarAccion()
    {
        if (!tienePalanca) return;

        switch (tipoAccionPalanca)
        {
            case TipoAccionPalanca.Normal:
                EjecutarModoNormal();
                break;

            case TipoAccionPalanca.Toggle:
                EjecutarModoToggle();
                break;

            case TipoAccionPalanca.UnaVez:
                EjecutarModoUnaVez();
                break;
        }
    }

    private void EjecutarModoNormal()
    {
        EjecutarAccionNormal();
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

            case AccionPalanca.InvertirGiro:
                InvertirGiro();
                break;

            case AccionPalanca.EncenderTemporal:
            case AccionPalanca.ApagarTemporal:
                // Las acciones temporales son incompatibles con Toggle.
                break;
        }
    }

    private void EjecutarModoUnaVez()
    {
        if (accionUnaVezUsada) return;

        accionUnaVezUsada = true;
        EjecutarAccionNormal();
    }

    private void EjecutarAccionNormal()
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

            case AccionPalanca.InvertirGiro:
                InvertirGiro();
                break;

            case AccionPalanca.EncenderTemporal:
                IniciarCambioTemporal(true);
                break;

            case AccionPalanca.ApagarTemporal:
                IniciarCambioTemporal(false);
                break;
        }
    }

    private void InicializarEstadosAcciones()
    {
        InicializarToggleEncendidoApagado();

        // En Toggle Girar, la primera activación siempre gira hacia los grados configurados.
        toggleSiguienteGirar = true;

        // En UnaVez, se vuelve a permitir usar la acción al iniciar o al hacer respawn.
        accionUnaVezUsada = false;
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
        if (corrutinaCambioTemporal != null)
        {
            StopCoroutine(corrutinaCambioTemporal);
            corrutinaCambioTemporal = null;
        }

        if (corrutinaParpadeo != null)
        {
            StopCoroutine(corrutinaParpadeo);
            corrutinaParpadeo = null;
        }

        corrutinaParpadeo = StartCoroutine(ParpadearCamara(encender));
    }

    private void IniciarCambioTemporal(bool encenderTemporalmente)
    {
        if (corrutinaCambioTemporal != null)
        {
            StopCoroutine(corrutinaCambioTemporal);
            corrutinaCambioTemporal = null;
        }

        if (corrutinaParpadeo != null)
        {
            StopCoroutine(corrutinaParpadeo);
            corrutinaParpadeo = null;
        }

        corrutinaCambioTemporal = StartCoroutine(CambioTemporalCamara(encenderTemporalmente));
    }

    private IEnumerator CambioTemporalCamara(bool encenderTemporalmente)
    {
        bool estadoTemporal = encenderTemporalmente;
        bool estadoFinal = !encenderTemporalmente;

        yield return ParpadearCamara(estadoTemporal);

        if (duracionEncendidoTemporal > 0f)
        {
            yield return new WaitForSeconds(duracionEncendidoTemporal);
        }

        yield return ParpadearCamara(estadoFinal);

        corrutinaCambioTemporal = null;
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

    private void IniciarSonidoGiro(bool volumenReducido = false)
    {
        if (sonidoGirando != null)
            return;

        sonidoGirando = SoundFX_Manager.Instance.GetRandomClip(SoundType.CAMARA_SE_MUEVE);

        if (sonidoGirando == null)
            return;

        sonidoGirando.loop = true;
        sonidoGirando.volume = volumenReducido ? 0.4f : 1f;
        sonidoGirando.enabled = true;
        sonidoGirando.Play();
    }

    private void DetenerSonidoGiro()
    {
        if (sonidoGirando == null)
            return;

        sonidoGirando.Stop();
        sonidoGirando.enabled = false;
        sonidoGirando = null;

        SoundFX_Manager.Instance.PlaySound(SoundType.CAMARA_SE_DETUVO);
    }

    private void InvertirGiro()
    {
        switch (modoCamara)
        {
            case ModoCamara.Paneo:
                InvertirGiroPaneo();
                break;

            case ModoCamara.GiroConstante:
                InvertirGiroConstante();
                break;

            case ModoCamara.Idle:
                break;
        }
    }

    private void InvertirGiroPaneo()
    {
        invertirGiroPaneoPendiente = true;
    }

    private void InvertirGiroConstante()
    {
        giroConstanteHaciaDerecha = !giroConstanteHaciaDerecha;

        if (corrutinaGiroConstante != null)
        {
            StopCoroutine(corrutinaGiroConstante);
            corrutinaGiroConstante = null;
        }

        estaGirandoConstante = false;

        if (modoCamara == ModoCamara.GiroConstante)
        {
            IniciarGiroConstante();
        }
    }

    private void IniciarGiro(float grados)
    {
        // En modo Giro Constante, la acción Girar no afecta a la cámara.
        if (modoCamara == ModoCamara.GiroConstante) return;
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
        // En modo Giro Constante, la acción Girar no afecta a la cámara.
        if (modoCamara == ModoCamara.GiroConstante) return false;
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
        IniciarSonidoGiro();
        Quaternion rotacionInicial = lente.transform.localRotation;

        yield return RotarLente(
            rotacionInicial,
            grados,
            duracionGiro,
            curvaGiro
        );

        DetenerSonidoGiro();

        estaGirando = false;
        corrutinaGiro = null;
    }

    private IEnumerator GirarCamaraHacia(Quaternion rotacionFinal)
    {
        estaGirando = true;
        IniciarSonidoGiro();
        Quaternion rotacionInicial = lente.transform.localRotation;
        float deltaZ = Mathf.DeltaAngle(rotacionInicial.eulerAngles.z, rotacionFinal.eulerAngles.z);

        yield return RotarLente(
            rotacionInicial,
            deltaZ,
            duracionGiro,
            curvaGiro
        );

        DetenerSonidoGiro();

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

            lente.transform.localRotation = Quaternion.Euler(
                0f,
                0f,
                anguloInicial + gradosARotar * progresoConCurva
            );

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

        SerializedProperty giroConstanteHaciaDerecha = serializedObject.FindProperty("giroConstanteHaciaDerecha");
        SerializedProperty duracionVueltaGiroConstante = serializedObject.FindProperty("duracionVueltaGiroConstante");
        SerializedProperty curvaGiroConstante = serializedObject.FindProperty("curvaGiroConstante");

        SerializedProperty gradosDeGiro = serializedObject.FindProperty("gradosDeGiro");
        SerializedProperty duracionGiro = serializedObject.FindProperty("duracionGiro");
        SerializedProperty curvaGiro = serializedObject.FindProperty("curvaGiro");

        SerializedProperty duracionParpadeo = serializedObject.FindProperty("duracionParpadeo");
        SerializedProperty frecuenciaParpadeo = serializedObject.FindProperty("frecuenciaParpadeo");
        SerializedProperty duracionEncendidoTemporal = serializedObject.FindProperty("duracionEncendidoTemporal");

        SerializedProperty lente = serializedObject.FindProperty("lente");

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

        if ((ModoCamara)modoCamara.enumValueIndex == ModoCamara.GiroConstante)
        {
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Configuración de giro constante", EditorStyles.boldLabel);

            EditorGUI.indentLevel++;

            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.PropertyField(
                giroConstanteHaciaDerecha,
                new GUIContent("Girar hacia la derecha")
            );

            EditorGUILayout.PropertyField(
                duracionVueltaGiroConstante,
                new GUIContent("Duración de una vuelta")
            );

            EditorGUILayout.PropertyField(
                curvaGiroConstante,
                new GUIContent("Curva de giro")
            );

            EditorGUILayout.HelpBox(
                "En este modo, la cámara gira constantemente. La acción Girar no tendrá efecto, pero Encender, Apagar e Invertir Giro sí pueden seguir funcionando.",
                MessageType.Info
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
            ModoCamara modoSeleccionado = (ModoCamara)modoCamara.enumValueIndex;

            if (tipoSeleccionado == TipoAccionPalanca.UnaVez)
            {
                EditorGUILayout.HelpBox(
                    "En modo Una Vez, esta acción solo se podrá ejecutar una vez. Después no volverá a hacer nada hasta que la cámara se reinicie o el jugador haga respawn.",
                    MessageType.Info
                );
            }

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

                if (modoSeleccionado == ModoCamara.GiroConstante)
                {
                    EditorGUILayout.HelpBox(
                        "La acción Girar no tendrá efecto mientras el modo de cámara sea Giro Constante.",
                        MessageType.Warning
                    );
                }
                else if (tipoSeleccionado == TipoAccionPalanca.Toggle)
                {
                    EditorGUILayout.HelpBox(
                        "En Toggle, la primera activación girará la cámara los grados configurados. La siguiente activación volverá a la rotación inicial, como un paneo manual.",
                        MessageType.Info
                    );
                }

                EditorGUI.indentLevel--;
            }

            if (accionSeleccionada == AccionPalanca.InvertirGiro)
            {
                EditorGUILayout.HelpBox(
                    "Invertir Giro cambia la dirección del paneo o del giro constante. En Paneo, la inversión se aplica al inicio del siguiente ciclo completo. En Toggle se comporta igual que Normal porque cada activación vuelve a invertir la dirección.",
                    MessageType.Info
                );

                if (modoSeleccionado == ModoCamara.Idle)
                {
                    EditorGUILayout.HelpBox(
                        "Esta acción no tendrá efecto en modo Idle. Solo funciona en Paneo o Giro Constante.",
                        MessageType.Warning
                    );
                }
            }

            if (
                accionSeleccionada == AccionPalanca.Encender ||
                accionSeleccionada == AccionPalanca.Apagar ||
                accionSeleccionada == AccionPalanca.EncenderTemporal ||
                accionSeleccionada == AccionPalanca.ApagarTemporal
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

                if (
                    accionSeleccionada == AccionPalanca.EncenderTemporal ||
                    accionSeleccionada == AccionPalanca.ApagarTemporal
                )
                {
                    EditorGUILayout.Space();

                    EditorGUILayout.LabelField(
                        "Configuración temporal",
                        EditorStyles.boldLabel
                    );

                    EditorGUILayout.PropertyField(
                        duracionEncendidoTemporal,
                        new GUIContent("Duración temporal")
                    );

                    if (tipoSeleccionado == TipoAccionPalanca.Toggle)
                    {
                        EditorGUILayout.HelpBox(
                            "Las acciones temporales son incompatibles con Toggle. Usa Normal o Una Vez.",
                            MessageType.Warning
                        );
                    }
                    else
                    {
                        string descripcion = accionSeleccionada == AccionPalanca.EncenderTemporal
                            ? "Encender Temporal: enciende la cámara durante la duración indicada y luego la apaga."
                            : "Apagar Temporal: apaga la cámara durante la duración indicada y luego la enciende.";

                        EditorGUILayout.HelpBox(
                            descripcion,
                            MessageType.Info
                        );
                    }
                }
                else if (tipoSeleccionado == TipoAccionPalanca.Toggle)
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

        EditorGUI.indentLevel--;

        serializedObject.ApplyModifiedProperties();
    }
}
#endif