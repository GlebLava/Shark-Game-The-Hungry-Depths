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

}
