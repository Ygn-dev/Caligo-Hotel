using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class Change_Camera_Var : MonoBehaviour
{
    public float NewPosCompX;
    public float NewPosCompY;
    public float duration;
    public AnimationCurve curve;
    public CinemachineCamera camera;

    private IEnumerator TriggerCameraChange()
    {
        CinemachinePositionComposer camPosCom =  camera.GetComponent<CinemachinePositionComposer>();
        Vector2 initialPos = camPosCom.Composition.ScreenPosition;
        Vector2 newPosComp = new Vector2(NewPosCompX != 0 ? NewPosCompX : initialPos.x, NewPosCompY != 0 ? NewPosCompY : initialPos.y);


        float time = 0f;
        while(time < duration)
        {
            float t = Mathf.Clamp01(time / duration);
            float curveValue = curve.Evaluate(t);

            // Interpolar entre la posición inicial y la nueva
            camPosCom.Composition.ScreenPosition = Vector2.Lerp(initialPos, newPosComp, curveValue);
            time += Time.deltaTime;
            
            yield return null;
        }
        camPosCom.Composition.ScreenPosition = newPosComp;
        yield return null;
    }

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(TriggerCameraChange());
            GetComponent<Collider2D>().enabled = false; // Desactivar el trigger para que no se vuelva a activar
        }
    }
}
