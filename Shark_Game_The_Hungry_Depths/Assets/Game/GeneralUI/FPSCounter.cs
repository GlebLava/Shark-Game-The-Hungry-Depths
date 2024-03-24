using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FPSCounter : MonoBehaviour
{
    private float deltaTime = 0.0f;
    public TMP_Text fpsCounter;

    private void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        fpsCounter.text = $"FPS: {Mathf.Round(fps)}";
    }
}
