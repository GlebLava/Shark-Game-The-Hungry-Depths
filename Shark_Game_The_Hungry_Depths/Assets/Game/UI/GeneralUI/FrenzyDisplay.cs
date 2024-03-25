using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FrenzyDisplay : MonoBehaviour
{
    public Image frenzyBarBackground;
    public Image frenzyBar;

    private bool start = false;

    private void Awake()
    {
        SignalBus.OnLevelFinishedLoading += OnLevelFinishedLoading;
    }

    private void OnDestroy()
    {
        SignalBus.OnLevelFinishedLoading -= OnLevelFinishedLoading;
    }

    private void OnLevelFinishedLoading()
    {
        start = true;
    }

    private void Update()
    {
        if (!start) return;

        Vector3 scale = frenzyBarBackground.transform.localScale;
        // The initial width of the frenzy display represents a frenzy amount of 10
        scale.x = LevelManager.instance.playerController.sharkScriptableObject.maxFrenzy / 10;
        frenzyBarBackground.transform.localScale = scale;


        Vector2 leftestMiddlePosBackground = frenzyBarBackground.rectTransform.rect.center;
        leftestMiddlePosBackground.x -= (frenzyBar.rectTransform.rect.width / 2) * scale.x;

        frenzyBar.transform.localPosition = leftestMiddlePosBackground;

        Vector3 scaleFrenzy = frenzyBar.transform.localScale;
        // The initial width of the frenzy display represents a frenzy amount of 10
        scaleFrenzy.x = LevelManager.instance.playerController.currentFrenzy / 10;
        frenzyBar.transform.localScale = scaleFrenzy;
    }

}
