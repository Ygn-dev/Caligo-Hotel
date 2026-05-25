using UnityEngine;
using UnityEngine.UI;
using Unity.Cinemachine;
using System.Collections;

public static class DevTools
{
    public static IEnumerator Animar(Image image, float targetAlpha, float duration, AnimationCurve curve)
    {
        float startAlpha = image.color.a;
        float time = 0f;

        while (time < duration)
        {
            float t = Mathf.Clamp01(time / duration);
            float curveValue = curve.Evaluate(t);

            float alpha = Mathf.LerpUnclamped(startAlpha, targetAlpha, curveValue);
            image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);

            time += Time.unscaledDeltaTime;
            yield return null;
        }

        image.color = new Color(image.color.r, image.color.g, image.color.b, targetAlpha);
        yield return null;
    }

    public static IEnumerator SetupCamara(CinemachineCamera camera, ScriptableObject levelDataSO, GameObject character)
    {
        //camera.Follow = character.transform;

        // Obtener componentes necesarios
        Level_Data_Base nivelData = (Level_Data_Base)levelDataSO;
        camera.Follow = character.transform;


        CinemachinePositionComposer camPosCom =  camera.GetComponent<CinemachinePositionComposer>();
        CinemachineConfiner2D camConfiner = camera.GetComponent<CinemachineConfiner2D>();

        camConfiner.SlowingDistance = nivelData.slowingDistance;
        camConfiner.Damping = nivelData.damping;

        // cambiar screen position composer
        camera.Lens.OrthographicSize = nivelData.camaraZoom;

        if (nivelData.esDeadZone)
        {
            camPosCom.Composition.DeadZone.Enabled = true;
            camPosCom.Composition.DeadZone.Size = nivelData.deadZoneWidthHeight;
        }
        else
        {
            camPosCom.Composition.DeadZone.Enabled = false;
        }

        // cambiar screen position composer 
        camPosCom.Composition.ScreenPosition = nivelData.screenPositionComposer;
        
        // Cambiar confiner
        GameObject confinerInst = Object.Instantiate(nivelData.confiner);

        camConfiner.BoundingShape2D = confinerInst.GetComponentInChildren<Collider2D>();
        camConfiner.InvalidateBoundingShapeCache();

        yield return null;
    }
    
    public static void SetupCinematicManager()
    {
        GameObject cinematicManagerPrefab = Resources.Load<GameObject>("Cinematic_Manager/Cinematic_Manager_Prefab");
        Object.Instantiate(cinematicManagerPrefab);
    }

    public static void SetupDialogueManager()
    {
        GameObject dialogueManagerPrefab = Resources.Load<GameObject>("Dialogue_Manager");
        Object.Instantiate(dialogueManagerPrefab);
    }
}
