using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidManagerPrototype : MonoBehaviour
{
    public GameObject boidPrefab;

    [Header("Boid Preconfig")]
    public LayerMask boidLayer;
    public GameObject player;
    public AquariumBuilderPrototype aquariumBuilder;
    public BoidManagerComputerPrototype boidManagerComputer;
    public List<Collider> staticColliders;

    [Header("Boid Config")]
    public float maxSpeed = 7f;
    public float maxSteerForce = 1f;

    public float radiusBorder = 0.25f;
    public float radiusColliders = 0.75f;
    public float radiusPlayerDetection = 1.5f;
    public float radiusBoidsAvoidance = 1.25f;
    public float radiusVelocityMatching = 4f;
    public float radiusCentring = 3f;

    [Header("Weights")]
    public float collidersAvoidanceWeight = 7f;
    public float playerAvoidanceWeight = 5f;
    public float boidsAvoidanceWeight = 1f;
    public float velocityMatchingWeight = 1f;
    public float flockCenteringWeight = 1f;

    private float allWeightsCombined = 1f;

    [HideInInspector]
    public GameObject[] boids;
    bool gameStarted = false;

    Coroutine currentCalculateRoulesCo;
    Coroutine currentSlideWeightsCo;

    Vector3[] collidersAvoidVelocityCache;
    Vector3[] playerAvoidVelocityCache;

    public void StartGame(int boidsCount)
    {
        DestroyBoids();
        GetAllStaticColliders();

        SpawnBoids(boidsCount);

        if (gameStarted)
        {
            boidManagerComputer.Deinit();
        }

        boidManagerComputer.Init(boids);

        gameStarted = true;

        if (currentCalculateRoulesCo != null)
        {
            StopCoroutine(currentCalculateRoulesCo);
        }

        if (currentSlideWeightsCo != null)
        {
            StopCoroutine(currentSlideWeightsCo);
        }


        collidersAvoidVelocityCache = new Vector3[boidsCount];
        playerAvoidVelocityCache = new Vector3[boidsCount];

        currentCalculateRoulesCo = StartCoroutine(CalculateRoulesCo());
        currentSlideWeightsCo = StartCoroutine(SlideWeightsCo());

        allWeightsCombined = collidersAvoidanceWeight + playerAvoidanceWeight + boidsAvoidanceWeight + velocityMatchingWeight + flockCenteringWeight;
    }

    private void FixedUpdate()
    {
        if (!gameStarted)
        {
            return;
        }

        for (int i = 0; i < boids.Length; i++)
        {
            GameObject boid = boids[i];
            if (boid == null) continue;

            RespawnBoidIfOutOfBounds(i);
            Vector3 acceleration = Vector3.zero;
            acceleration += SteerTowards(GetBorderAvoidVelocity(boid), i) * allWeightsCombined;
            acceleration += SteerTowards(collidersAvoidVelocityCache[i], i) * collidersAvoidanceWeight;
            acceleration += SteerTowards(playerAvoidVelocityCache[i], i) * playerAvoidanceWeight;
            acceleration += SteerTowards(boidManagerComputer.avoidance[i], i) * boidsAvoidanceWeight;
            acceleration += SteerTowards(boidManagerComputer.velocityMatching[i], i) * velocityMatchingWeight;
            acceleration += SteerTowards(boidManagerComputer.flockCentring[i], i) * flockCenteringWeight;

            boidManagerComputer.velocities[i] += acceleration * Time.fixedDeltaTime;
            float speed = boidManagerComputer.velocities[i].magnitude;
            Vector3 dir = speed == 0 ? Vector3.zero : boidManagerComputer.velocities[i] / speed;
            speed = Mathf.Clamp(speed, 0.1f, maxSpeed);
            boidManagerComputer.velocities[i] = dir * speed;

            boids[i].transform.position += boidManagerComputer.velocities[i];
            boids[i].transform.LookAt(boids[i].transform.position + boidManagerComputer.velocities[i]);
        }
    }

    IEnumerator CalculateRoulesCo()
    {
        // At the beginnning we want to calculate all rules each time for every frame to ensure no chaos
        for (int i = 0; i < 60; i++)
        {
            boidManagerComputer.CalculateRules(
                radiusBoidsAvoidance * radiusBoidsAvoidance,
                radiusVelocityMatching * radiusVelocityMatching,
                radiusCentring * radiusCentring);
            for (int j = 0; j < boids.Length; j++)
            {
                if (boids[j] == null) continue;

                collidersAvoidVelocityCache[j] = GetCollidersAvoidVelocity(boids[j]);
                playerAvoidVelocityCache[j] = GetPlayerAvoidVelocity(boids[j]);
            }
            yield return new WaitForFixedUpdate();
        }

        // Now we slow down the calculations
        while (true)
        {
            boidManagerComputer.CalculateRules(
                radiusBoidsAvoidance * radiusBoidsAvoidance,
                radiusVelocityMatching * radiusVelocityMatching,
                radiusCentring * radiusCentring);
            yield return new WaitForSeconds(0.001f);

            
            for (int i = 0; i < boids.Length; i++)
            {
                if (boids[i] == null) continue;

                collidersAvoidVelocityCache[i] = GetCollidersAvoidVelocity(boids[i]);
                playerAvoidVelocityCache[i] = GetPlayerAvoidVelocity(boids[i]);

                if (i % 32 == 0)
                {
                    yield return new WaitForSeconds(0.001f);
                }
            }
        }
    }

    IEnumerator SlideWeightsCo()
    {
        float weightsRange = 1.5f;
        float avoidance = boidsAvoidanceWeight;
        float velocityMatch = velocityMatchingWeight;
        float centring = flockCenteringWeight;
        float timer = 0;

        float aRandomStart = Random.Range(-1, 1);
        float vRandomStart = Random.Range(-1, 1);
        float cRandomStart = Random.Range(-1, 1);

        float aSpeed = Random.Range(0, 3);
        float vSpeed = Random.Range(0, 3);
        float cSpeed = Random.Range(0, 3);

        while (true)
        {
            boidsAvoidanceWeight = avoidance + (weightsRange * Mathf.Sin((timer + aRandomStart) * aSpeed));
            velocityMatchingWeight = velocityMatch + (weightsRange * Mathf.Sin((timer + vRandomStart) * vSpeed));
            flockCenteringWeight = centring + (weightsRange * Mathf.Sin((timer + cRandomStart) * cSpeed));

            yield return new WaitForFixedUpdate();
            timer += Time.fixedDeltaTime;
        }
    }



    #region velocity methods

    private Vector3 GetBorderAvoidVelocity(GameObject boid)
    {
        Vector3 oppositeDir = Vector3.zero;
        Vector3 pos = boid.transform.position;

        Bounds bounds = aquariumBuilder.boundsCached;

        // x
        if (pos.x >= bounds.center.x + bounds.extents.x - radiusBorder) { oppositeDir.x = -1; }
        else if (pos.x <= bounds.center.x - bounds.extents.x + radiusBorder) { oppositeDir.x = 1; }

        // y
        if (pos.y >= bounds.center.y + bounds.extents.y - radiusBorder) { oppositeDir.y = -1; }
        else if (pos.y <= bounds.center.y - bounds.extents.y + radiusBorder) { oppositeDir.y = 1; }

        // z
        if (pos.z >= bounds.center.z + bounds.extents.z - radiusBorder) { oppositeDir.z = -1; }
        else if (pos.z <= bounds.center.z - bounds.extents.z + radiusBorder) { oppositeDir.z = 1; }


        return oppositeDir;
    }


    private Vector3 GetCollidersAvoidVelocity(GameObject boid)
    {
        Vector3 oppositeDir = Vector3.zero;

        int collsCount = 0;
        foreach (Collider coll in staticColliders)
        {
            Vector3 closestPoint = coll.ClosestPoint(boid.transform.position);
            float distanceToCollider = Vector3.Distance(closestPoint, boid.transform.position);
            if (distanceToCollider > radiusColliders) continue;

            float distanceWeight = 1 - (distanceToCollider / radiusColliders);
            oppositeDir += (boid.transform.position - closestPoint) * distanceWeight;
            collsCount++;
        }

        if (collsCount > 0)
        {
            return (oppositeDir / collsCount).normalized;
        }
        else
        {
            return oppositeDir;
        }
    }

    private Vector3 GetPlayerAvoidVelocity(GameObject boid)
    {
        Vector3 oppositeDir = Vector3.zero;
        if (player != null)
        {
            Vector3 closestPoint = player.transform.position;
            float distanceToCollider = Vector3.Distance(closestPoint, boid.transform.position);
            if (distanceToCollider <= radiusPlayerDetection)
            {
                float distanceWeight = 1 - (distanceToCollider / radiusPlayerDetection);
                oppositeDir += (boid.transform.position - closestPoint) * distanceWeight;
            }
        }

        return oppositeDir;
    }


    #endregion

    #region helper methods

    void DestroyBoids()
    {
        if (boids == null) return;

        for (int i = 0; i < boids.Length; i++)
        {
            if (boids[i] != null)
            {
                Destroy(boids[i]);
                boids[i] = null;
            }
        }
    }

    void SpawnBoids(int boidsCount)
    {
        boids = new GameObject[boidsCount];

        for (int i = 0; i < boidsCount; i++)
        {
            boids[i] = GameObject.Instantiate(boidPrefab, transform);
            boids[i].transform.position = transform.position;

            float variation = 0.05f;
            boids[i].transform.localScale += new Vector3(Random.Range(-variation, variation), Random.Range(-variation, variation), Random.Range(-variation, variation));
        }
    }

    void GetAllStaticColliders()
    {
        staticColliders = new List<Collider>();
        Collider[] colliders = Physics.OverlapSphere(transform.position, 100000f, ~boidLayer);

        foreach (Collider coll in colliders)
        {
            if (coll.gameObject.CompareTag("Player")) continue;
            staticColliders.Add(coll);
        }
    }


    Vector3 SteerTowards(Vector3 vector, int boidIndex)
    {
        Vector3 v = vector.normalized * maxSpeed - boidManagerComputer.velocities[boidIndex];
        return Vector3.ClampMagnitude(v, maxSteerForce);
    }

    private void RespawnBoidIfOutOfBounds(int boid)
    {
        if (!aquariumBuilder.boundsCached.Contains(boids[boid].transform.position))
        {
            boids[boid].transform.position = transform.position;
            boidManagerComputer.velocities[boid] = Random.insideUnitSphere;
        }
    }
    #endregion
}
