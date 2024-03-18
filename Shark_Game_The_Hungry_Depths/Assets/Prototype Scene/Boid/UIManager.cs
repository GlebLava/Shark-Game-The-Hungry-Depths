using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TMP_InputField inputField;
    public TMP_Text fpsText;
    private float deltaTime = 0.0f;

    public Slider avoidanceSlider;
    public Slider velocitySlider;
    public Slider centerSlider;

    public BoidManager boidManager;

    private void Start()
    {
        avoidanceSlider.SetValueWithoutNotify(boidManager.boidsAvoidanceWeight);
        velocitySlider.SetValueWithoutNotify(boidManager.velocityMatchingWeight);
        centerSlider.SetValueWithoutNotify(boidManager.flockCenteringWeight);
    }

    private void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        fpsText.text = $"FPS: {Mathf.Round(fps)}";
    }

    public void SpawnBoids()
    {
        int boidsCount = int.Parse(inputField.text);
        boidManager.StartGame(boidsCount);
    }

    public void OnAvoidanceSlider(float value)
    {
        boidManager.boidsAvoidanceWeight = value;
    }

    public void OnVelocitySlider(float value)
    {
        boidManager.velocityMatchingWeight = value;
    }

    public void OnCenterSlider(float value)
    {
        boidManager.flockCenteringWeight = value;
    }
}
