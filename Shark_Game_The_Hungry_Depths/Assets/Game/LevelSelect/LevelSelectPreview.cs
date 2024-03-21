using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class LevelSelectPreview : MonoBehaviour, IPointerClickHandler
{
    public TMP_Text title;
    public Image levelImage;

    public UnityEvent OnClick;

    private void Awake()
    {
        SignalBus.OnLevelSelected += OnLevelSelected;
        OnLevelSelected(GameManager.instance.gameData.selectedLevel);
    }

    void OnLevelSelected(string levelName)
    {
        var level = GameManager.instance.levelSelectItemScriptableObjects.Find((so) => so.name == levelName);
        if (level == null)
        {
            title.text = "Error: please reselect a level";
        }

        title.text = level.levelTitle;
        levelImage.sprite = level.levelSprite;
    }

    private void OnDestroy()
    {
        SignalBus.OnLevelSelected -= OnLevelSelected;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick?.Invoke();
    }
}
