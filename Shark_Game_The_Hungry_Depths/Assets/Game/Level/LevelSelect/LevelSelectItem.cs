using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LevelSelectItem : MonoBehaviour, IPointerClickHandler
{
    public TMP_Text title; // Set by Scriptable Object

    public GameObject priceObject; // Preset
    public TMP_Text textPrice; // Set by Scriptable Object

    public Image levelImage; // Set by Scriptable Object
    public Image lockImage; // Preset
    public Image selectedLevelImage; // Preset

    public bool locked = false;
    private bool lockedForBuy = false;

    public LevelSelectItemSO levelSelectItemSO; // Set by Scriptable Object

    private void Awake()
    {
        SignalBus.OnLevelSelected += OnLevelSelected;
    }

    private void OnDestroy()
    {
        SignalBus.OnLevelSelected -= OnLevelSelected;
    }

    public void AssignLevelSelectItemSO(LevelSelectItemSO levelSelectItemSO)
    {
        this.levelSelectItemSO = levelSelectItemSO;
        title.text = levelSelectItemSO.levelTitle;
        levelImage.sprite = levelSelectItemSO.levelSprite;

        textPrice.text = levelSelectItemSO.levelPrice.ToString() + " x";

        priceObject.SetActive(false);
        lockImage.gameObject.SetActive(false);

        OnLevelSelected(GameManager.instance.gameData.selectedLevel);
    }


    public void LockLevel()
    {
        lockImage.gameObject.SetActive(true);
        locked = true;
    }

    public void UnlockLevel() 
    {
        lockImage.gameObject.SetActive(false);
        locked = false;
    }


    public void ShowPrice()
    {
        priceObject.SetActive(true);
        lockedForBuy = false;
    }

    public void HidePrice()
    {
        priceObject.SetActive(false);
        lockedForBuy = true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (GameManager.instance.gameData.levelsOwned.Contains(levelSelectItemSO.name))
        {
            // Select this level, since the player already owns it
            SignalBus.OnLevelSelectedInvoke(levelSelectItemSO.name);
        }
        else if (!lockedForBuy)
        {
            // Buy this level if player has enough money
            // since the locked will prevent us from coming here if the level
            // cant be buyable yet
            if (GameManager.instance.gameData.coinsOwned < levelSelectItemSO.levelPrice)
                return;

            SignalBus.OnCoinsAmountChangedInvoke(GameManager.instance.gameData.coinsOwned - levelSelectItemSO.levelPrice);
            SignalBus.OnNewLevelBoughtInvoke(levelSelectItemSO.name);
            SignalBus.OnLevelSelectedInvoke(levelSelectItemSO.name);
        }
    }

    private void OnLevelSelected(string name)
    {
        if (name == levelSelectItemSO.name)
        {
            selectedLevelImage.gameObject.SetActive(true);
        }
        else
        {
            selectedLevelImage.gameObject.SetActive(false);
        }
    }

}
