using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/GadgetSO")]
public class GadgetSO : ScriptableObject
{
    // Name of the gadget
    // We use the name of the scriptable object as the id
    // so we can change the title of the gadget at any time
    public string title;
    public Sprite icon;
    public string description;
    public int price;
}
