using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GadgetShopItem : MonoBehaviour
{
    public TMP_Text title;
    public Image icon;
    public TMP_Text description;
    public TMP_Text price;
    public TMP_Text ownedAmount;

    public Button buyButton;

    private GadgetSO gadgetSO;

    private void Awake()
    {
        buyButton.onClick.AddListener(ClickedBuy);
        SignalBus.OnCoinsAmountChanged += OnCoinsAmountChanged;
    }

    private void GetAmountOwned()
    {
        GadgetData gd = GameManager.instance.gameData.gadgetsOwned.Find((g) => g.name == gadgetSO.name);
        if (gd == null)
        {
            ownedAmount.text = "0";
        }
        else
        {
            ownedAmount.text = gd.amountOwned.ToString();
        }
    }

    public void AssignGadgetSO(GadgetSO gadgetSO)
    {
        this.gadgetSO = gadgetSO;

        title.text = gadgetSO.title;
        icon.sprite = gadgetSO.icon;
        description.text = gadgetSO.description;
        price.text = gadgetSO.price.ToString();

        GetAmountOwned();

        if (GameManager.instance.gameData.coinsOwned < gadgetSO.price)
        {
            buyButton.interactable = false;
        }
    }

    private void ClickedBuy()
    {
        int currentAmount = int.Parse(ownedAmount.text);
        currentAmount++;
        ownedAmount.text = currentAmount.ToString();

        SignalBus.OnCoinsAmountChangedInvoke(GameManager.instance.gameData.coinsOwned - gadgetSO.price);
        SignalBus.OnNewGadgetBoughtInvoke(gadgetSO.name);
    }

    private void OnCoinsAmountChanged(int amountOfCoins)
    {
        if (amountOfCoins < gadgetSO.price)
        {
            buyButton.interactable = false;
        }
        else
        {
            buyButton.interactable = true;
        }
    }


    private void OnDestroy()
    {
        buyButton.onClick.RemoveListener(ClickedBuy);
        SignalBus.OnCoinsAmountChanged -= OnCoinsAmountChanged;
    }

}

