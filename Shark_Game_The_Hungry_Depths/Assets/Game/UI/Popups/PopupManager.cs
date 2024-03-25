using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupManager : MonoBehaviour
{
    public static PopupManager instance;

    public PopupOkCancel popupOkCancelPrefab;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void DisplayPopupOkCancel(Canvas canvasParent, string text, Action OnOkAction, Action OnCancelAction, bool deletePopupOkCancel = true)
    {
        PopupOkCancel popup = Instantiate(popupOkCancelPrefab, canvasParent.transform);
        popup.Setup(text, OnOkAction, OnCancelAction, deletePopupOkCancel);
    }
}
