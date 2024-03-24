using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class SwarmBoidsManager : MonoBehaviour
{
    public SwarmBoidsRulesComputer swarmBoidsRulesComputer;
    public List<BoidFactionPart> boidFactions;

    private GameObject[] boids;
    private AquariumBuilder aquariumBuilder;

    private bool start = false;

    NativeArray<int> boidFactionIndex;
    NativeArray<BoidFactionData> boidFactionDatas;


    public void Setup(int maxBoidsAtOnce, List<Transform> possibleSpawnPoints, AquariumBuilder aquariumBuilder)
    {
        this.aquariumBuilder = aquariumBuilder;
        boids = new GameObject[maxBoidsAtOnce];


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
                    boids[i].transform.position = possibleSpawnPoints[0].transform.position;

                    break;
                }
            }
        }


        swarmBoidsRulesComputer.Init(boids, boidFactionIndex, boidFactionDatas);
        swarmBoidsRulesComputer.CalculateRules(aquariumBuilder.boundsCached, LevelManager.instance.player.transform.position);
        start = true;
    }

    private void FixedUpdate()
    {
        if (!start)
            return;

        swarmBoidsRulesComputer.FinishCalculateRules();

        NativeArray<Vector3> positions = swarmBoidsRulesComputer.GetPositions();
        NativeArray<Vector3> velocities = swarmBoidsRulesComputer.GetVelocities();

        for (int i = 0; i < boids.Length; i++)
        {
            boids[i].transform.position = positions[i];
            boids[i].transform.LookAt(positions[i] + velocities[i]);
        }

        swarmBoidsRulesComputer.CalculateRules(aquariumBuilder.boundsCached, LevelManager.instance.player.transform.position);
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