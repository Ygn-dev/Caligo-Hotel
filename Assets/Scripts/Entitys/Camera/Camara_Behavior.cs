using UnityEngine;
using System.Collections;

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
    [HideInInspector] public AnimationCurve curvaRotacionPaneo;

    // MODO GIRAR POR PALANCA
    [HideInInspector] public float gradosDeGiro;
    [HideInInspector] public float duracionGiro;
    [HideInInspector] public AnimationCurve curvaGiro;

    // Variables privadas
    private Player_Respawn playerRespawn;
    private Quaternion rotacionInicialLente;
    private Coroutine corrutinaGiro;
    private Coroutine corrutinaPaneo;
    private bool estaGirando;
    private bool estaPaneando;
    private bool giroPendiente;
    private float gradosGiroPendiente;


    private void Awake()
    {
        if (lente == null) return;
        rotacionInicialLente = lente.transform.localRotation;
    }

    private void OnEnable()
    {
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
            Quaternion rotacionFinal = rotacionInicial * Quaternion.Euler(0f, 0f, gradosRotacionPaneo);
            yield return RotarLente(rotacionInicial, rotacionFinal, duracionRotacionPaneo, curvaRotacionPaneo);
            yield return EjecutarGiroPendienteSiExiste();
            
            // TIEMPO DE ESPERA
            if (tiempoEsperaPaneo > 0f) yield return EsperarPaneo(tiempoEsperaPaneo);
            yield return EjecutarGiroPendienteSiExiste();
            
            // VUELTA
            rotacionInicial = lente.transform.localRotation;
            rotacionFinal = rotacionInicial * Quaternion.Euler(0f, 0f, -gradosRotacionPaneo);
            yield return RotarLente(rotacionInicial, rotacionFinal, duracionRotacionPaneo, curvaRotacionPaneo);
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

        float grados = gradosGiroPendiente;

        giroPendiente = false;
        gradosGiroPendiente = 0f;

        estaGirando = true;

        Quaternion rotacionInicial = lente.transform.localRotation;
        Quaternion rotacionFinal = rotacionInicial * Quaternion.Euler(0f, 0f, grados);

        yield return RotarLente(
            rotacionInicial,
            rotacionFinal,
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
            // Si durante la espera se activa la palanca,
            // cortamos la espera para ejecutar el giro pendiente.
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
        if (lente != null)
            lente.transform.localRotation = rotacionInicialLente;

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

        giroPendiente = false;
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
                // EjecutarModoToggle();
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

            case AccionPalanca.Apagar:
                // Apagar();
                break;
        }
    }

    private void IniciarGiro(float grados)
    {
        if (lente == null) return;

        // Si está paneando, no giramos inmediatamente.
        // Guardamos el giro para ejecutarlo cuando termine el tramo actual del paneo.
        if (estaPaneando)
        {
            giroPendiente = true;
            gradosGiroPendiente += grados;
            return;
        }

        if (estaGirando) return;

        corrutinaGiro = StartCoroutine(GirarCamara(grados));
    }

    private IEnumerator GirarCamara(float grados)
    {
        estaGirando = true;

        Quaternion rotacionInicial = lente.transform.localRotation;
        Quaternion rotacionFinal = rotacionInicial * Quaternion.Euler(0f, 0f, grados);

        yield return RotarLente(
            rotacionInicial,
            rotacionFinal,
            duracionGiro,
            curvaGiro
        );

        estaGirando = false;
        corrutinaGiro = null;
    }

    private IEnumerator RotarLente(
        Quaternion rotacionInicial,
        Quaternion rotacionFinal,
        float duracion,
        AnimationCurve curva
    )
    {
        if (lente == null) yield break;

        if (duracion <= 0f)
        {
            lente.transform.localRotation = rotacionFinal;
            yield break;
        }

        float tiempo = 0f;

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;

            float progreso = Mathf.Clamp01(tiempo / duracion);

            float progresoConCurva = curva != null
                ? curva.Evaluate(progreso)
                : progreso;

            lente.transform.localRotation = Quaternion.Lerp(
                rotacionInicial,
                rotacionFinal,
                progresoConCurva
            );

            yield return null;
        }

        lente.transform.localRotation = rotacionFinal;
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

            if ((AccionPalanca)accionPalanca.enumValueIndex == AccionPalanca.Girar)
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