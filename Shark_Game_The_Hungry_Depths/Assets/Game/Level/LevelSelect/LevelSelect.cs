using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
    public LevelSelectItem levelSelectItemPrefab;

    public Color inactiveColor;
    public Color activeColor;

    public List<Button> depthButtons;
    public List<GameObject> depthScreens;

    private List<LevelSelectItem> spawnedLevelSelectItems = new List<LevelSelectItem>();

    public void Awake()
    {
        HideAllDepthScreens();

        for (int i = 0; i < depthButtons.Count; i++)
        {
            // Need to recapture the iterator in lambda function
            // because c# optimizes it away or something like that
            int index = i;
            depthButtons[i].onClick.AddListener(() =>
            {
                ClickDepth(index);
            });
        }

        for (int i = 0; i < depthScreens.Count; i++)
        {
            var content = depthScreens[i].GetComponentInChildren<GridLayoutGroup>().gameObject;

            GameManager.instance.levelSelectItemScriptableObjects.Sort((a, b) => a.levelPrice.CompareTo(b.levelPrice));
            foreach (LevelSelectItemSO levelSelectItemSO in GameManager.instance.levelSelectItemScriptableObjects)
            {
                if (levelSelectItemSO.depthIndex != i) continue;
                var levelSelectItem = Instantiate(levelSelectItemPrefab, content.transform);
                levelSelectItem.AssignLevelSelectItemSO(levelSelectItemSO);
                spawnedLevelSelectItems.Add(levelSelectItem);
            }
        }

        SignalBus.OnNewLevelBought += OnNewLevelBought;

        EnsureRightLevelsAreLocked();
        ClickDepth(0);
    }

    private void OnDestroy()
    {
        SignalBus.OnNewLevelBought -= OnNewLevelBought;
    }


    private void ClickDepth(int index)
    {
        HideAllDepthScreens();
        AssignEachButtonInActiveColor();

        depthScreens[index].SetActive(true);
        depthButtons[index].image.color = activeColor;
    }

    private void AssignEachButtonInActiveColor()
    {
        foreach (var button in depthButtons)
        {
            button.image.color = inactiveColor;
        }
    }

    private void HideAllDepthScreens()
    {
        foreach (var depthScreen in depthScreens)
            depthScreen.SetActive(false);
    }

    private void OnNewLevelBought(string s)
    {
        EnsureRightLevelsAreLocked();
    }


    /// <summary>
    /// Not only does it lock the right levels but it also
    /// displays the price on the right level. (The price is only
    /// displayed on the next locked level after the last unlocked
    /// level)
    ///
    /// </summary>
    private void EnsureRightLevelsAreLocked()
    {
        GameManager.instance.levelSelectItemScriptableObjects.Sort((a, b) => a.levelPrice.CompareTo(b.levelPrice));

        // First we lock all levels
        foreach (var spawnedLevel in spawnedLevelSelectItems)
        {
            spawnedLevel.LockLevel();
            spawnedLevel.HidePrice();
        }

        // Then we unlock all owned levels
        foreach (var spawnedLevel in spawnedLevelSelectItems)
        {
            if (GameManager.instance.gameData.levelsOwned.Contains(spawnedLevel.levelSelectItemSO.name))
            {
                spawnedLevel.UnlockLevel();
            }
        }

        // Now we search for the lowest priced locked level and show the price there
        LevelSelectItem nextUnlockableLevel = spawnedLevelSelectItems.Where((item) => item.locked).OrderByDescending((item) => item.levelSelectItemSO.levelPrice).Last();
        if (nextUnlockableLevel != null) nextUnlockableLevel.ShowPrice();
    }
}
