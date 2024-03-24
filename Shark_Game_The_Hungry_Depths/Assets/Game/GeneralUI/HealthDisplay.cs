using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    public Image healthBarBackground;
    public Image healthBar;

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

        Vector3 scale = healthBarBackground.transform.localScale;
        // The initial width of the health display represents a health of 10
        scale.x = LevelManager.instance.playerController.sharkScriptableObject.maxHealth / 10;
        healthBarBackground.transform.localScale = scale;


        Vector2 leftestMiddlePosBackground = healthBarBackground.rectTransform.rect.center;
        leftestMiddlePosBackground.x -= (healthBar.rectTransform.rect.width / 2) * scale.x;

        healthBar.transform.localPosition = leftestMiddlePosBackground;

        Vector3 scaleHealth = healthBar.transform.localScale;
        // The initial width of the health display represents a health of 10
        scaleHealth.x = LevelManager.instance.playerController.currentHealth / 10;
        healthBar.transform.localScale = scaleHealth;
    }

    
}
