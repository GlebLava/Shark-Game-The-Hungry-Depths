using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public delegate void Notify();
public delegate void NotifyInt(int i);
public delegate void NotifyString(string s);
public static class SignalBus
{
    public static event NotifyString OnSharkPlayerChosen;
    public static void OnSharkPlayerChosenInvoke(string s)
    {
        OnSharkPlayerChosen?.Invoke(s);
    }

    public static event NotifyInt OnCoinsAmountChanged;
    public static void OnCoinsAmountChangedInvoke(int amount)
    {
        OnCoinsAmountChanged?.Invoke(amount);
    }

    public static event NotifyString OnNewSharkBought;
    public static void OnNewSharkBoughtInvoke(string s)
    {
        OnNewSharkBought?.Invoke(s);
    }

    public static event NotifyString OnNewGadgetBought;
    public static void OnNewGadgetBoughtInvoke(string s)
    {
        OnNewGadgetBought?.Invoke(s);
    }

    public static event NotifyString OnLevelSelected;
    public static void OnLevelSelectedInvoke(string s)
    {
        OnLevelSelected?.Invoke(s);
    }

    public static event NotifyString OnNewLevelBought;
    public static void OnNewLevelBoughtInvoke(string s)
    {
        OnNewLevelBought?.Invoke(s);
    }

}
