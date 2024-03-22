using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoadingScreen : MonoBehaviour
{
    public TMP_Text loadingDots;

    private void Awake()
    {
        StartCoroutine(LoadingDots());
    }

    IEnumerator LoadingDots()
    {
        while (true)
        {
            loadingDots.text = "";
            yield return new WaitForSeconds(0.2f);
            loadingDots.text = ".";
            yield return new WaitForSeconds(0.2f);
            loadingDots.text = "..";
            yield return new WaitForSeconds(0.2f);
            loadingDots.text = "...";
            yield return new WaitForSeconds(0.2f);
        }
    }


}
