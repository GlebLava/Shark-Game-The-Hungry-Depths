using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/LevelSelectItemSO")]
public class LevelSelectItemSO : ScriptableObject
{
    public string levelTitle;
    public int levelPrice;
    public int depthIndex;
    public Sprite levelSprite;
}
