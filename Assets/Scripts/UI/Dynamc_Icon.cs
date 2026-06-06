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


    void Awake() {
        if(image != null)
            imageMode = true;
        else if(spriteRenderer != null)
            imageMode = false;
        else
        {
            Debug.LogError("No se ha asignado ni Image ni SpriteRenderer en el inspector.");
            return;
        }

        Input_Schema_Manager.ChangedSchema += UpdateIcon;
        UpdateIcon(Input_Schema_Manager.Instance.currentSchema);
    }


    private void UpdateIcon(string newSchema)
    {
        //Debug.Log($"Actualizando icono para el esquema: {newSchema}");
        var action = actionReference.action;

        for (int i = 0; i < action.bindings.Count; i++)
        {
            var binding = action.bindings[i];

            string path = binding.effectivePath;
            string grupos = binding.groups; // Aquí están los control schemes

            // Caso especial: composite (ej: Move con WASD/Flechas)
            if (binding.isComposite)
            {
                string resultado = "Sprites_Icons_UI/" + newSchema + "/" + action.name;
                Sprite nuevoSprite = Resources.Load<Sprite>(resultado);
                if (nuevoSprite == null)
                {
                    Debug.LogWarning($"No se pudo cargar el composite: {resultado}");
                    continue;
                }
                if(imageMode)
                    image.sprite = nuevoSprite;
                else
                    spriteRenderer.sprite = nuevoSprite;
                return;
            }

            // Caso normal
            if (grupos.Contains(newSchema))
            {
                string resultado = "Sprites_Icons_UI/" + newSchema + "/" + path.Split('/')[1];// "SpritesIconsUI/Keyboard/enter"
                Sprite nuevoSprite = Resources.Load<Sprite>(resultado);
                if (nuevoSprite == null)
                {
                    Debug.LogWarning($"No se pudo cargar la imagen desde el path: {resultado}");
                    continue;
                }
                else
                {
                    if(imageMode)
                        image.sprite = nuevoSprite;
                    else
                        spriteRenderer.sprite = nuevoSprite;
                    return; 
                }
            }
        }

        Debug.LogWarning($"No se encontró una imagen para el esquema: {newSchema}");
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