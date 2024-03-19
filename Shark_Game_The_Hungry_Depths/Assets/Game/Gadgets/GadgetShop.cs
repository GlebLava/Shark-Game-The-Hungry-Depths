using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GadgetShop : MonoBehaviour
{
    public GadgetShopItem gadgetShopItemPrefab;

    public GameObject content;

    List<GameObject> spawnedGadgetShopItems = new List<GameObject>();

    private void OnEnable()
    {
        foreach (GadgetSO gadgetSO in GameManager.instance.gadgetScriptableObjects)
        {
            var gadgetShopItem = Instantiate(gadgetShopItemPrefab, content.transform);
            gadgetShopItem.AssignGadgetSO(gadgetSO);
            spawnedGadgetShopItems.Add(gadgetShopItem.gameObject);
        }
    }

    
    private void OnDisable()
    {
        foreach (var go in spawnedGadgetShopItems)
            Destroy(go);

        spawnedGadgetShopItems.Clear();
    }
}
