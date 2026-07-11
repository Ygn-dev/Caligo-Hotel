using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.EventSystems;

public class Extras_Func : MonoBehaviour
{
    [Header("Canvas Groups")]
    public CanvasGroup canvasGroup1; // Menú principal
    public CanvasGroup canvasGroup2; // Menú Extras

    [Header("Input")]
    public InputActionAsset inputActionAsset;

    [Header("Configuración")]
    public float fadeDuration = 1f;

    [Header("Botones")]
    public GameObject primerBoton;
    public GameObject botonDefault;
    public GameObject botonInicial2;
    public GameObject botonDefault2;

    public Animator LogoAnimator; 
    public Animator LeyendaAnimator;

    private InputActionMap mainMenuMap;
    private InputAction backAction;

    private void Awake()
    {
        mainMenuMap = inputActionAsset.FindActionMap("MainMenu");
        backAction = mainMenuMap.FindAction("Back");
    }

    public void ActivarExtras()
    {
        backAction.performed -= OnBackActionPerformed;

        mainMenuMap.Disable();
        StartCoroutine(FadeToExtras());
    }

    private IEnumerator FadeToExtras()
    {
        canvasGroup2.gameObject.SetActive(true);

        canvasGroup1.alpha = 1f;
        canvasGroup2.alpha = 0f;

        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fadeDuration;

            canvasGroup1.alpha = Mathf.Lerp(1f, 0f, t);
            canvasGroup2.alpha = Mathf.Lerp(0f, 1f, t);

            yield return null;
        }

        canvasGroup1.alpha = 0f;
        canvasGroup2.alpha = 1f;

        canvasGroup1.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.6f);

        EventSystem.current.SetSelectedGameObject(primerBoton);

        mainMenuMap.Enable();
        Input_Schema_Manager.Instance.defaultCovered = botonDefault;

        backAction.performed += OnBackActionPerformed;
    }

    private void OnBackActionPerformed(InputAction.CallbackContext context)
    {
        backAction.performed -= OnBackActionPerformed;
        StartCoroutine(FadeBackToMainMenu());
    }

    private IEnumerator FadeBackToMainMenu()
    {
        mainMenuMap.Disable();

        canvasGroup1.gameObject.SetActive(true);

        canvasGroup1.alpha = 0f;
        canvasGroup2.alpha = 1f;

        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fadeDuration;

            canvasGroup1.alpha = Mathf.Lerp(0f, 1f, t);
            canvasGroup2.alpha = Mathf.Lerp(1f, 0f, t);

            yield return null;
        }

        canvasGroup1.alpha = 1f;
        canvasGroup2.alpha = 0f;

        canvasGroup2.gameObject.SetActive(false);

        LogoAnimator.Play("Idle_Visible", 0, 1f);
        LeyendaAnimator.Play("Visible", 0, 1f);

        yield return new WaitForSeconds(0.6f);

        EventSystem.current.SetSelectedGameObject(botonInicial2);
        Input_Schema_Manager.Instance.defaultCovered = botonDefault2;

        mainMenuMap.Enable();
    }
}