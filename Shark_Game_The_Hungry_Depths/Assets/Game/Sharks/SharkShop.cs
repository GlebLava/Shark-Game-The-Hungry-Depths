using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SharkShop : MonoBehaviour
{
    readonly float DISTANCE_BETWEEN_SHARKS = 5;

    public GameObject sharksHolder;
    private List<GameObject> spawnedSharkDisplays = new List<GameObject>();

    [Header("Buttons")]
    public GameObject upButton;
    public GameObject downButton;

    public GameObject chooseButton;
    public GameObject buyButton;

    [Header("Effects")]
    public AnimationCurve swapCurve;

    private int focusIndex = 0;

    private void OnEnable()
    {
        for (int i = 0; i < GameManager.instance.sharkScriptableObjects.Count; i++)
        {
            var shark = Instantiate(GameManager.instance.sharkScriptableObjects[i].sharkModelPrefab, sharksHolder.transform);
            shark.transform.position = new Vector3(0, i * DISTANCE_BETWEEN_SHARKS, 0);
            spawnedSharkDisplays.Add(shark);

            if (GameManager.instance.gameData.currentShark == GameManager.instance.sharkScriptableObjects[i].name)
            {
                focusIndex = i;
            }

        }

        DisableRightButtonUpDown();
        DisableRightButtonBuyChoose();
        JumpToFocusIndex(0, focusIndex);
    }

    private void OnDisable()
    {
        foreach (var shark in spawnedSharkDisplays)
        {
            Destroy(shark);
        }

        spawnedSharkDisplays.Clear();
    }


    public void ClickUp()
    {
        focusIndex++;
        DisableRightButtonUpDown();
        DisableRightButtonBuyChoose();
        JumpToFocusIndex(focusIndex - 1, focusIndex);
    }

    public void ClickDown()
    {
        focusIndex--;
        DisableRightButtonUpDown();
        DisableRightButtonBuyChoose();
        JumpToFocusIndex(focusIndex + 1, focusIndex);
    }

    public void ClickChooseShark()
    {
        SignalBus.OnSharkPlayerChosenInvoke(GameManager.instance.sharkScriptableObjects[focusIndex].name);
    }

    private void DisableRightButtonUpDown()
    {
        upButton.SetActive(true);
        downButton.SetActive(true);

        if (focusIndex == 0) downButton.SetActive(false);
        if (focusIndex == GameManager.instance.sharkScriptableObjects.Count - 1) upButton.SetActive(false);
    }

    private void DisableRightButtonBuyChoose()
    {
        if (GameManager.instance.gameData.sharksOwned.Contains(GameManager.instance.sharkScriptableObjects[focusIndex].name))
        {
            buyButton.SetActive(false);
            chooseButton.SetActive(true);
        }
        else
        {
            buyButton.SetActive(true);
            chooseButton.SetActive(false);
            buyButton.GetComponent<Button>().interactable = GameManager.instance.sharkScriptableObjects[focusIndex].cost <= GameManager.instance.gameData.coinsOwned;
        }
    }

    private void JumpToFocusIndex(int indexFrom, int indexTo)
    {
        StopAllCoroutines();
        StartCoroutine(JumpToCurrentFocusIDCo(indexFrom, indexTo));
    }

    private IEnumerator JumpToCurrentFocusIDCo(int indexFrom, int indexTo)
    {
        float jumpingTime = 0.5f;
        float timer = 0f;

        float fromSharkY = spawnedSharkDisplays[indexFrom].transform.position.y;
        float wantedSharkY = spawnedSharkDisplays[indexTo].transform.position.y;
        float currentSharksHolderY = sharksHolder.transform.position.y;

        while (timer < jumpingTime)
        {
            Vector3 sharkHolderPos = sharksHolder.transform.position;
            sharkHolderPos.y = Mathf.Lerp(currentSharksHolderY - fromSharkY, currentSharksHolderY - wantedSharkY, swapCurve.Evaluate(timer / jumpingTime));
            sharksHolder.transform.position = sharkHolderPos;

            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
        }
    }

}
