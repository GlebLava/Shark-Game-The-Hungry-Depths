using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreenAquarium : MonoBehaviour
{
    List<GameObject> spawnedSharks = new List<GameObject>();

    private void Awake()
    {
        SignalBus.OnNewSharkBought += NewSharkBought;
    }

    private void Start()
    {
        SpawnAllOwnedSharks();
    }

    private void OnDestroy()
    {
        SignalBus.OnNewSharkBought -= NewSharkBought;
    }

    private void Update()
    {
        RotateAquarium();
    }

    private void NewSharkBought(string sharkName)
    {
        SharkSO sharkSO = GameManager.instance.sharkScriptableObjects.Find((s) => s.name == sharkName);
        AddShark(sharkSO.sharkModelPrefab.gameObject);
    }

    private void SpawnAllOwnedSharks()
    {
        foreach (string sharkName in GameManager.instance.gameData.sharksOwned)
        {
            SharkSO sharkSO = GameManager.instance.sharkScriptableObjects.Find((s) => s.name == sharkName);
            if (sharkSO == null) continue; // Could be because of rename or anything like that

            AddShark(sharkSO.sharkModelPrefab.gameObject);
        }
    }

    private void AddShark(GameObject sharkPrefab)
    {
        var sharkGo = Instantiate(sharkPrefab, transform);
        sharkGo.AddComponent<IndividualBoidManager>();
        sharkGo.AddComponent<SphereCollider>().radius = 2f;

        spawnedSharks.Add(sharkGo);
    }

    private void RotateAquarium()
    {
        transform.Rotate(0, 3 * Time.deltaTime, 0);
    }
}
