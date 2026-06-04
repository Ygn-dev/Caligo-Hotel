using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public enum ModoCamara
{
    Idle
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
    //No Editable
    public GameObject lente;


    //Editable
    public ModoCamara modoCamara;
    public bool tienePalanca;
    [HideInInspector] public TipoAccionPalanca tipoAccionPalanca;
    [HideInInspector] public AccionPalanca accionPalanca;

    //MODO GIRAR
    [HideInInspector] public float gradosDeGiro;
    [HideInInspector] public float duracionGiro;
    [HideInInspector] public AnimationCurve curvaGiro;
    
    //Variables privadas
    private Player_Respawn playerRespawn;
    
    // Valores iniciales
    private Quaternion rotacionInicialLente;
    private Coroutine corrutinaGiro;
    
    //MODO GIRAR
    private bool estaGirando;


    //Funciones
    private void Awake()
    {
        if (lente == null) return;
        rotacionInicialLente = lente.transform.localRotation;
    }
    private void OnEnable()
    {
        StartCoroutine(WaitForPlayer());
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
        playerRespawn.ReiniciarNivelEvent += OnPlayerRespawn;
    }

    private void OnDisable()
    {
        if (playerRespawn != null)
            playerRespawn.RespawnEvent -= OnPlayerRespawn;
    }

    private void OnPlayerRespawn()
    {
        if (estaGirando)
        {  
            StopCoroutine(corrutinaGiro);
            corrutinaGiro = null;
        }

        estaGirando = false; 
        lente.transform.localRotation = rotacionInicialLente;
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
                //EjecutarModoToggle();
                break;

            case TipoAccionPalanca.UnaVez:
                //EjecutarModoUnaVez();
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
                //Apagar();
                break;
        }
    }

    private void IniciarGiro(float grados)
    {
        if (estaGirando) return;
        corrutinaGiro = StartCoroutine(GirarCamara(grados));
    }
    
    private IEnumerator GirarCamara(float grados)
    {
        estaGirando = true;

        Quaternion rotacionInicial = lente.transform.localRotation;
        Quaternion rotacionFinal = rotacionInicial * Quaternion.Euler(0f, 0f, grados);

        float tiempo = 0f;

        while (tiempo < duracionGiro)
        {
            tiempo += Time.deltaTime;

            float progreso = tiempo / duracionGiro;
            float progresoConCurva = curvaGiro.Evaluate(progreso);

            lente.transform.localRotation = Quaternion.Lerp(
                rotacionInicial,
                rotacionFinal,
                progresoConCurva
            );

            yield return null;
        }

        lente.transform.localRotation = rotacionFinal;
        estaGirando = false;
        yield return null;
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

            EditorGUI.indentLevel++;

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

            EditorGUI.indentLevel--;

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