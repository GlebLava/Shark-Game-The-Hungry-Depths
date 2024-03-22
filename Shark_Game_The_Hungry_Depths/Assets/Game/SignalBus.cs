using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public delegate void Notify();
public delegate void NotifyInt(int i);
public delegate void NotifyString(string s);
public delegate void NotifyVector2(Vector2 v);
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


    /// <summary>
    /// Used by each level. The level is loaded async. But async loading does not include all
    /// awake and start calls. To not lag the game to much each level handles the creation of the 
    /// level and spawning of boids in coroutines. This is called by the level to announce that it finished loading completly
    /// and the game can be started
    /// </summary>
    public static event Notify OnLevelFinishedLoading;
    public static void OnLevelFinishedLoadingInvoke()
    {
        OnLevelFinishedLoading?.Invoke();
    }

    public static event Notify OnPauseGame;
    public static void OnPauseGameInvoke()
    {
        OnPauseGame?.Invoke();
    }

    public static event Notify OnUnpauseGame;
    public static void OnUnpauseGameInvoke()
    {
        OnUnpauseGame?.Invoke();
    }

    public static event Notify OnLevelFinishedReturnToMainMenu;
    public static void OnLevelFinishedReturnToMainMenuInvoke()
    {
        OnLevelFinishedReturnToMainMenu?.Invoke();
    }



}
