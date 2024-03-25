using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PopupOkCancel : MonoBehaviour
{
    public TMP_Text text;
    public Button buttonOk;
    public Button buttonCancle;

    public void Setup(string text, Action OnOkAction, Action OnCancelAction, bool deletePopupOkCancel = true)
    {
        this.text.text = text;
        buttonOk.onClick.AddListener(() => OnOkAction?.Invoke());
        buttonCancle.onClick.AddListener(() => OnCancelAction?.Invoke());

        if (deletePopupOkCancel)
        {
            buttonOk.onClick.AddListener(() => Destroy(gameObject));
            buttonCancle.onClick.AddListener(() => Destroy(gameObject));
        }

    }
}
