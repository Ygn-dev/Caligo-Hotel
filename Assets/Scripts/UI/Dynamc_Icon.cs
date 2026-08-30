using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;


#if UNITY_EDITOR
using UnityEditor;
#endif

public enum ObjectType
{
    Image,
    SpriteRenderer
}

public class Dynamc_Icon : MonoBehaviour
{
    public InputActionReference actionReference;
    public ObjectType tipoDeObjeto;

    public Image image;
    public SpriteRenderer spriteRenderer;

    private bool imageMode;
    private bool initialized;

    void Awake()
    {
        if (image != null)
        {
            imageMode = true;
            initialized = true;
        }
        else if (spriteRenderer != null)
        {
            imageMode = false;
            initialized = true;
        }
        else
        {
            Debug.LogError("No se ha asignado ni Image ni SpriteRenderer en el inspector.");
            enabled = false;
        }
    }

    private void OnEnable()
    {
        if (!initialized) return;

        // Evita suscribirse dos veces por accidente
        Input_Schema_Manager.ChangedSchema -= UpdateIcon;
        Input_Schema_Manager.ChangedSchema += UpdateIcon;

        if (Input_Schema_Manager.Instance != null)
        {
            UpdateIcon(Input_Schema_Manager.Instance.currentSchema);
        }
    }

    private void OnDisable()
    {
        Input_Schema_Manager.ChangedSchema -= UpdateIcon;
    }

    private void OnDestroy()
    {
        Input_Schema_Manager.ChangedSchema -= UpdateIcon;
    }

    private void UpdateIcon(string newSchema)
    {
        if (actionReference == null || actionReference.action == null)
        {
            Debug.LogWarning("No se ha asignado un InputActionReference.");
            return;
        }

        var action = actionReference.action;

        for (int i = 0; i < action.bindings.Count; i++)
        {
            var binding = action.bindings[i];

            string path = binding.effectivePath;
            string grupos = binding.groups;

            // Caso especial: composite, por ejemplo Move con WASD
            if (binding.isComposite)
            {
                string actionMap = action.actionMap.name;
                string resultado;

                if (newSchema == "Gamepad")
                {
                    resultado = "Sprites_Icons_UI/" + newSchema + "/" + Input_Schema_Manager.Instance.currentGamepad + "/" + actionMap + "/" + action.name;
                }
                else
                {
                    resultado = "Sprites_Icons_UI/" + newSchema + "/" + actionMap + "/" + action.name;
                }

                Sprite nuevoSprite = Resources.Load<Sprite>(resultado);

                if (nuevoSprite == null)
                {
                    Debug.LogWarning($"No se pudo cargar el composite: {resultado}");
                    continue;
                }

                SetSprite(nuevoSprite);
                return;
            }

            // Caso normal
            if (!string.IsNullOrEmpty(grupos) && grupos.Contains(newSchema))
            {
                if (string.IsNullOrEmpty(path) || !path.Contains("/"))
                {
                    Debug.LogWarning($"Binding path inválido: {path}");
                    continue;
                }

                string actionMap = action.actionMap.name;
                string resultado;

                if (newSchema == "Gamepad")
                {
                    resultado = "Sprites_Icons_UI/" + newSchema + "/" + Input_Schema_Manager.Instance.currentGamepad + "/" + actionMap + "/" + action.name;
                }
                else
                {
                    resultado = "Sprites_Icons_UI/" + newSchema + "/" + actionMap + "/" + action.name;
                }

                
                Sprite nuevoSprite = Resources.Load<Sprite>(resultado);

                if (nuevoSprite == null)
                {
                    Debug.LogWarning($"No se pudo cargar la imagen desde el path: {resultado}");
                    continue;
                }

                SetSprite(nuevoSprite);
                return;
            }
        }
        Debug.LogWarning($"Asegúrate de que la ruta 'Sprites_Icons_UI/{newSchema}/' contenga las imágenes correspondientes a las acciones.");
    }

    private void SetSprite(Sprite nuevoSprite)
    {
        if (imageMode)
        {
            if (image != null)
                image.sprite = nuevoSprite;
        }
        else
        {
            if (spriteRenderer != null)
                spriteRenderer.sprite = nuevoSprite;
        }
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(Dynamc_Icon))]
public class Dynamc_Icon_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedProperty actionReference = serializedObject.FindProperty("actionReference");
        SerializedProperty tipoDeObjeto = serializedObject.FindProperty("tipoDeObjeto");
        SerializedProperty image = serializedObject.FindProperty("image");
        SerializedProperty spriteRenderer = serializedObject.FindProperty("spriteRenderer");

        EditorGUILayout.PropertyField(actionReference, new GUIContent("Action Reference"));
        
        EditorGUILayout.Space();

        EditorGUILayout.BeginVertical("box");

        EditorGUILayout.PropertyField(tipoDeObjeto, new GUIContent("Tipo de Objeto"));

        EditorGUI.indentLevel++;

        ObjectType selectedType = (ObjectType)tipoDeObjeto.enumValueIndex;

        switch (selectedType)
        {
            case ObjectType.Image:
                EditorGUILayout.PropertyField(image, new GUIContent("Image"));
                break;

            case ObjectType.SpriteRenderer:
                EditorGUILayout.PropertyField(spriteRenderer, new GUIContent("Sprite Renderer"));
                break;
        }

        EditorGUI.indentLevel--;

        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }
}
#endif