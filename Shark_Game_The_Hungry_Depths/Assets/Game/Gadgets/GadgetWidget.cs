using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GadgetWidget : MonoBehaviour
{
    [Header("Button slots")]
    public Button slot0;
    public Button slot1;
    public Button slot2;
    private Button[] slotsAsArray;

    [Header("To setup")]
    public TitleScreen titleScreen;
    public GameObject gadgetsOwnedPanel;
    // This background is used for detecting whether a click or
    // tap happened outside the gadgets owned widget. 
    // So its important that this background does not encapsulate
    // the actual slot buttons
    public Image gadgetsOwnedBackground;
    public GameObject gadgetsOwnedContent;
    public Sprite emptySlot;

    [Header("Prefabs")]
    public Button buttonGadgetWidgetPrefab;

    int currentSlotSelecting = 0;

    private void Awake()
    {
        gadgetsOwnedPanel.SetActive(false);

        slotsAsArray = new Button[3];
        slotsAsArray[0] = slot0;
        slotsAsArray[1] = slot1;
        slotsAsArray[2] = slot2;

        slot0.onClick.AddListener(() => ClickSlot(0));
        slot1.onClick.AddListener(() => ClickSlot(1));
        slot2.onClick.AddListener(() => ClickSlot(2));

        for (int i = 0; i < GameManager.instance.gameData.selectedGadgets.Length; i++)
        {
            string gadgetName = GameManager.instance.gameData.selectedGadgets[i];
            GadgetSO gadgetSO = GameManager.instance.gadgetScriptableObjects.Find((go) => go.name == gadgetName);

            if (gadgetSO == null)
            {
                slotsAsArray[i].image.sprite = emptySlot;
            }
            else
            {
                slotsAsArray[i].image.sprite = gadgetSO.icon;
            }
        }
    }

    void ClickSlot(int slot)
    {
        currentSlotSelecting = slot;
        DisplayGadgetsOwnedPanel();
    }

    void DisplayGadgetsOwnedPanel()
    {
        gadgetsOwnedPanel.SetActive(true);

        foreach (GadgetSO gadget in GameManager.instance.gadgetScriptableObjects)
        {
            Button buttonGadget = Instantiate(buttonGadgetWidgetPrefab, gadgetsOwnedContent.transform);
            if (GameManager.instance.gameData.gadgetsOwned.Find((go) => go.name == gadget.name) != null)
            {
                buttonGadget.image.sprite = gadget.icon;
                buttonGadget.onClick.AddListener(() =>
                {
                    OnDisplayGadgetClick(gadget);
                    DisableGadgetsOwnedPanel();
                });
            }
            else
            {
                buttonGadget.onClick.AddListener(() =>
                {
                    titleScreen.ClickGadgetShop();
                    DisableGadgetsOwnedPanel();
                }); ;
            }
        }
    }

    void OnDisplayGadgetClick(GadgetSO gadget)
    {
        GameManager.instance.gameData.selectedGadgets[currentSlotSelecting] = gadget.name;
        slotsAsArray[currentSlotSelecting].image.sprite = gadget.icon;

        // First check that no other slots have the same gadget, if so remove them from these slots
        for (int i = 0; i < GameManager.instance.gameData.selectedGadgets.Length; i++)
        {
            if (i == currentSlotSelecting) continue;

            if (GameManager.instance.gameData.selectedGadgets[i] == gadget.name)
            {
                // Remove it from this slot
                GameManager.instance.gameData.selectedGadgets[i] = "";
                slotsAsArray[i].image.sprite = emptySlot;
            }
        }

        SignalBus.OnGadgetSelectedInvoke(currentSlotSelecting, gadget.name);
    }

    void DisableGadgetsOwnedPanel()
    {
        gadgetsOwnedPanel.SetActive(false);

        foreach (Transform child in gadgetsOwnedContent.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !RectTransformUtility.RectangleContainsScreenPoint(gadgetsOwnedBackground.rectTransform, Input.mousePosition))
        {
            DisableGadgetsOwnedPanel();
        }
    }
}
