using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class SwarmBoidsManager : MonoBehaviour
{
    public SwarmBoidsRulesComputer swarmBoidsRulesComputer;
    public List<BoidFactionPart> boidFactions;

    [HideInInspector]
    public GameObject[] boids;
    private AquariumBuilder aquariumBuilder;

    private bool start = false;

    NativeArray<int> boidFactionIndex;
    NativeArray<BoidFactionData> boidFactionDatas;

    int framesUntilRulesCalculate = 1;
    int framesCounter = 0;

    List<Transform> possibleSpawnPoints;
    Queue<int> respawnQueue;


    public void Setup(int maxBoidsAtOnce, int framesUntilRulesCalculate, List<Transform> possibleSpawnPoints, AquariumBuilder aquariumBuilder, List<Collider> colliders)
    {
        this.framesUntilRulesCalculate = framesUntilRulesCalculate;
        this.aquariumBuilder = aquariumBuilder;
        this.possibleSpawnPoints = possibleSpawnPoints;

        boids = new GameObject[maxBoidsAtOnce];
        respawnQueue = new Queue<int>(maxBoidsAtOnce);

        // Init boidfactionIndex and datas
        boidFactionDatas = new NativeArray<BoidFactionData>(boidFactions.Count, Allocator.Persistent);

        float totalRatio = 0;
        for (int i = 0; i < boidFactions.Count; i++)
        {
            totalRatio += boidFactions[i].totalAmountRatio;
        }

        int startIndex = 0;
        for (int i = 0; i < boidFactions.Count; i++)
        {
            BoidFactionData boidFactionData = BoidFactionData.FromScriptableObject(boidFactions[i].boidFaction);

            float percantage = boidFactions[i].totalAmountRatio / totalRatio;

            int endIndex = startIndex + (int)(percantage * boids.Length);

            boidFactionData.startIndex = startIndex;
            boidFactionData.endIndex = endIndex;


            if (i == boidFactions.Count - 1)
            {
                boidFactionData.endIndex = boids.Length;
            }

            boidFactionDatas[i] = boidFactionData;
            startIndex = endIndex;
        }

        boidFactionIndex = new NativeArray<int>(boids.Length, Allocator.Persistent);
        for (int i = 0; i < boids.Length; i++)
        {
            for (int j = 0; j < boidFactions.Count; j++)
            {
                if (i >= boidFactionDatas[j].startIndex && i < boidFactionDatas[j].endIndex)
                {
                    boidFactionIndex[i] = j;
                    boids[i] = Instantiate(boidFactions[j].boidFaction.boidPrefab, transform);
                    boids[i].transform.position = possibleSpawnPoints[Random.Range(0, possibleSpawnPoints.Count)].transform.position;

                    break;
                }
            }
        }

        swarmBoidsRulesComputer.Init(boids, boidFactionIndex, boidFactionDatas, colliders);
        swarmBoidsRulesComputer.CalculateRules(aquariumBuilder.boundsCached, LevelManager.instance.player.transform.position, framesUntilRulesCalculate);
        start = true;
    }

    private void FixedUpdate()
    {
        if (!start)
            return;

        bool calculateThisFrame = framesCounter % framesUntilRulesCalculate == 0;

        swarmBoidsRulesComputer.FinishCalculateRules();

        if (calculateThisFrame)
            RespawnBoids();


        NativeArray<Vector3> positions = swarmBoidsRulesComputer.GetPositions();
        NativeArray<Vector3> velocities = swarmBoidsRulesComputer.GetVelocities();

        for (int i = 0; i < boids.Length; i++)
        {
            boids[i].transform.position = positions[i];
            boids[i].transform.LookAt(positions[i] + velocities[i]);
        }


        if (calculateThisFrame)
            swarmBoidsRulesComputer.CalculateRules(aquariumBuilder.boundsCached, LevelManager.instance.player.transform.position, framesUntilRulesCalculate);
        
        framesCounter++;
    }

    public BoidFaction KillBoid(int boid)
    {
        respawnQueue.Enqueue(boid);
        boids[boid].SetActive(false);
        return boidFactions[boidFactionIndex[boid]].boidFaction;
    }

    private void RespawnBoids()
    {
        Transform furthestSpawnPoint = possibleSpawnPoints[0];
        Vector3 dir = LevelManager.instance.player.transform.position - furthestSpawnPoint.position;
        float distSquared = Vector3.Dot(dir, dir);
        for (int i = 1; i < possibleSpawnPoints.Count; i++)
        {
            dir = LevelManager.instance.player.transform.position - possibleSpawnPoints[i].position;
            float thisDistSquared = Vector3.Dot(dir, dir);
            if (thisDistSquared > distSquared)
            {
                furthestSpawnPoint = possibleSpawnPoints[i];
                distSquared = thisDistSquared;
            }
        }


        NativeArray<Vector3> positions = swarmBoidsRulesComputer.GetPositions();
        while (respawnQueue.Count > 0)
        {
            int boid = respawnQueue.Dequeue();
            positions[boid] = furthestSpawnPoint.position;
            boids[boid].SetActive(true);
        }
    }


    private void OnDestroy()
    {
        swarmBoidsRulesComputer.Deinit();
    }

}

[System.Serializable]
public struct BoidFactionPart
{
    /// <summary>
    /// All BoidFactions Ratio get added together and each ratio is the percentage
    /// of the total amount. This is how many of all maxBoidsAtOnce should be always
    /// this boid faction
    /// </summary>
    public float totalAmountRatio;
    public BoidFaction boidFaction;
}