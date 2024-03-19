using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreen : MonoBehaviour
{
    public GameObject titleScreen;
    public GameObject optionsScreen;
    public GameObject gadgetShopScreen;
    public GameObject sharkShopScreen;

    public GameObject returnToTitleScreenButton;


    [Header ("Cam and Transitions")]
    public Camera cam;
    public Transform cameraLookAtAquariumPos;
    public Transform cameraLookAtSharkShopPos;
    public AnimationCurve camAnimationCurve;

    private void Awake()
    {
        returnToTitleScreenButton.SetActive(false);
        AllDisable();
        titleScreen.SetActive(true);
        CopyTransformToCam(cameraLookAtAquariumPos);
    }

    public void ClickOptionsButton()
    {
        AllDisable();

        titleScreen.SetActive(optionsScreen.activeSelf); // Because its a toggle
        optionsScreen.SetActive(!optionsScreen.activeSelf); // Because its a toggle
        returnToTitleScreenButton.SetActive(true);
    }

    public void ClickReturnToTitleScreen()
    {
        AllDisable();
        titleScreen.SetActive(true);
        returnToTitleScreenButton.SetActive(false);
        FlyCamTo(cameraLookAtAquariumPos);
    }

    public void ClickGadgetShop()
    {
        AllDisable();
        gadgetShopScreen.SetActive(true);
        returnToTitleScreenButton.SetActive(true);
    }

    public void ClickSharkShop()
    {
        AllDisable();
        sharkShopScreen.SetActive(true);
        returnToTitleScreenButton.SetActive(true);
        FlyCamTo(cameraLookAtSharkShopPos);
    }

    private void AllDisable()
    {
        titleScreen.SetActive(false);
        optionsScreen.SetActive(false);
        gadgetShopScreen.SetActive(false);
        sharkShopScreen.SetActive(false);
    }

    private void FlyCamTo(Transform transform)
    {
        StopAllCoroutines();
        StartCoroutine(FlyCamToCo(transform));
    }

   
    private IEnumerator FlyCamToCo(Transform transform)
    {
        float jumpingTime = 0.5f;
        float timer = 0f;

        Vector3 camPosStart = cam.transform.position;
        Quaternion camRotStart = cam.transform.rotation;

        while (timer < jumpingTime)
        {
            cam.transform.position = Vector3.Lerp(camPosStart, transform.position, camAnimationCurve.Evaluate(timer / jumpingTime));
            cam.transform.rotation = Quaternion.Lerp(camRotStart, transform.rotation, camAnimationCurve.Evaluate(timer / jumpingTime));
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
        }
    }

    private void CopyTransformToCam(Transform transform)
    {
        cam.transform.position = transform.position;
        cam.transform.rotation = transform.rotation;
    }
}
