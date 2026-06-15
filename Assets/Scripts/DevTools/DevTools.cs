using UnityEngine;
using UnityEngine.UI;
using Unity.Cinemachine;
using System.Collections;

public static class DevTools
{
    //ANIMACIONES
    public static IEnumerator AnimarImage(Image image, float targetAlpha, float duration, AnimationCurve curve)
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

    public static IEnumerator AnimarCamaraYBackground(CinemachineCamera camera, float zoomCamara, float camPosX, float camPosY, float duracionZoom, AnimationCurve curvaZoom,
                                            GameObject InstanceBlackBackground, AnimationCurve curvaBlackBackground)
    {
        //debe haber un previo cinemachineCamera.Follow = null;
        float prevZoom = camera.Lens.OrthographicSize;
        Vector3 prevPosCam = camera.transform.position;
        Vector3 prevPosBlack = InstanceBlackBackground.transform.localPosition;

        float tiempoTranscurrido = 0f;

        while (tiempoTranscurrido < duracionZoom)
        {
            float t = Mathf.Clamp01(tiempoTranscurrido / duracionZoom);
            float curveZoom = curvaZoom.Evaluate(t);
            float curveBlack = curvaBlackBackground.Evaluate(t);

            camera.Lens.OrthographicSize = Mathf.LerpUnclamped(prevZoom, zoomCamara, curveZoom);
            camera.transform.position = Vector3.LerpUnclamped(prevPosCam, new Vector3(camPosX, camPosY, prevPosCam.z), curveZoom);
            InstanceBlackBackground.transform.localPosition = Vector3.LerpUnclamped(prevPosBlack, new Vector3(260, 0, 0), curveBlack);

            //if (interact.action.triggered) break; //interrumpir animacion

            tiempoTranscurrido += Time.deltaTime;
            yield return null;
        }

        camera.Lens.OrthographicSize = zoomCamara;
        camera.transform.position = new Vector3(camPosX, camPosY, prevPosCam.z);
        InstanceBlackBackground.transform.localPosition = new Vector3(260, 0, 0);

        yield return null;
    }

    //SETUPS
    public static IEnumerator SetupCamara(CinemachineCamera camera, ScriptableObject levelDataSO, GameObject character)
    {
        camera.Follow = null;

        // Obtener componentes necesarios
        Level_Data_Base nivelData = (Level_Data_Base)levelDataSO;

        CinemachineConfiner2D camConfiner = camera.GetComponent<CinemachineConfiner2D>();
        CinemachinePositionComposer camPosCom =  camera.GetComponent<CinemachinePositionComposer>();     

        camConfiner.Damping = nivelData.damping;
        camConfiner.SlowingDistance = nivelData.slowingDistance;

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
        GameObject confiner = GameObject.FindGameObjectWithTag("Confiner");

        camConfiner.BoundingShape2D = confiner.GetComponentInChildren<Collider2D>();
        camConfiner.InvalidateBoundingShapeCache();
        
        camera.Follow = character.transform;

        yield return null;
    }


    public static IEnumerator SetupCharacter(GameObject character, Level_Data_Base nivelData, System.Action<GameObject> setCharacter)
    {
        if (character != null) yield break;
            
        GameObject newCharacter = Object.Instantiate(Resources.Load<GameObject>("Prefabs/Entitys/Character"),nivelData.spawnPoint,Quaternion.identity);
        newCharacter.GetComponent<Player_Respawn>().nivelData = nivelData;
        setCharacter?.Invoke(newCharacter);
        yield return null;
    }
    
    public static void SetupCinematicManager()
    {
        GameObject cinematicManagerPrefab = Resources.Load<GameObject>("Prefabs/Cinematic_System/Cinematic_Manager");
        Object.Instantiate(cinematicManagerPrefab);
    }

    public static void SetupDialogueManager()
    {
        GameObject dialogueManagerPrefab = Resources.Load<GameObject>("Prefabs/Dialogue_System/Dialogue_Manager");
        Object.Instantiate(dialogueManagerPrefab);
    }
}
