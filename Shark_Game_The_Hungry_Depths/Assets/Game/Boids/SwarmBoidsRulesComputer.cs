using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

public class SwarmBoidsRulesComputer : MonoBehaviour
{
    private GameObject[] boids;

    public struct Holder
    {
        public NativeArray<Vector3> positions;
        public NativeArray<Vector3> velocities;
    }

    private Holder[] doubleBuffer;
    private int currentDoubleBufferIndex;

    private NativeArray<int> boidFactionIndex; // holds the index into boidFactionDatas for each boid
    private NativeArray<BoidFactionData> boidFactionDatas; // hold data about the faction


    JobHandle currentJob;

    public void Init(GameObject[] boids, NativeArray<int> boidFactionIndex, NativeArray<BoidFactionData> boidFactionDatas)
    {
        this.boids = boids;
        this.boidFactionIndex = boidFactionIndex;
        this.boidFactionDatas = boidFactionDatas;

        doubleBuffer = new Holder[2];


        // Init arrays
        doubleBuffer[0].positions = new NativeArray<Vector3>(boids.Length, Allocator.Persistent);
        doubleBuffer[1].positions = new NativeArray<Vector3>(boids.Length, Allocator.Persistent);

        doubleBuffer[0].velocities = new NativeArray<Vector3>(boids.Length, Allocator.Persistent);
        doubleBuffer[1].velocities = new NativeArray<Vector3>(boids.Length, Allocator.Persistent);

        for (int i = 0; i < boids.Length; i++)
        {
            doubleBuffer[0].positions[i] = boids[i].transform.position;
            doubleBuffer[1].positions[i] = boids[i].transform.position;

            doubleBuffer[0].velocities[i] = Random.insideUnitSphere;
            doubleBuffer[1].velocities[i] = Random.insideUnitSphere;
        }
    }

    public void CalculateRules(Bounds bounds, Vector3 playerPosition)
    {
        int frontBuffer = currentDoubleBufferIndex;
        int backBuffer = (currentDoubleBufferIndex + 1) % 2;


        var calculateRulesJob = new CalculateRulesJob
        {
            positionsIn = doubleBuffer[frontBuffer].positions,
            velocitiesIn = doubleBuffer[frontBuffer].velocities,

            positionsOut = doubleBuffer[backBuffer].positions,
            velocitiesOut = doubleBuffer[backBuffer].velocities,

            boidFactionDatas = boidFactionDatas,
            boidFactionIndex = boidFactionIndex,

            bounds = bounds,
            deltaTime = Time.fixedDeltaTime,
            playerPosition = playerPosition
        };

        currentJob = calculateRulesJob.Schedule(boids.Length, 64);
    }

    public void FinishCalculateRules()
    {
        currentJob.Complete();
        currentDoubleBufferIndex++;
        currentDoubleBufferIndex %= 2;
    }

    public NativeArray<Vector3> GetPositions()
    {
        return doubleBuffer[currentDoubleBufferIndex].positions;
    }

    public NativeArray<Vector3> GetVelocities()
    {
        return doubleBuffer[currentDoubleBufferIndex].velocities;
    }

    public void Deinit()
    {
        currentJob.Complete();

        doubleBuffer[0].positions.Dispose();
        doubleBuffer[1].positions.Dispose();

        doubleBuffer[0].velocities.Dispose();
        doubleBuffer[1].velocities.Dispose();

        boidFactionDatas.Dispose();
        boidFactionIndex.Dispose();
    }


    [BurstCompile]
    private struct CalculateRulesJob : IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<Vector3> positionsIn;
        [ReadOnly]
        public NativeArray<Vector3> velocitiesIn;

        [WriteOnly]
        public NativeArray<Vector3> positionsOut;
        [WriteOnly]
        public NativeArray<Vector3> velocitiesOut;

        [ReadOnly]
        public NativeArray<BoidFactionData> boidFactionDatas;
        [ReadOnly]
        public NativeArray<int> boidFactionIndex;



        public Bounds bounds;
        public float deltaTime;
        public Vector3 playerPosition;

        public void Execute(int i)
        {
            BoidFactionData factionData = boidFactionDatas[boidFactionIndex[i]];

            Vector3 accelaration = Vector3.zero;
            accelaration += SteerTowards(GetBorderAvoidVelocity(i), i) * 10f;
            accelaration += SteerTowards(GetPlayerAvoidVelocity(i, factionData), i) * factionData.playerAvoidanceWeight;

            accelaration += SteerTowards(GetBoidsAvoidVelocity(i, factionData), i) * factionData.boidsAvoidanceWeight;
            accelaration += SteerTowards(GetVelocityMatch(i, factionData), i) * factionData.velocityMatchingWeight;
            accelaration += SteerTowards(GetFlockCentringVelocity(i, factionData), i) * factionData.flockCenteringWeight;

            Vector3 velocity = velocitiesIn[i] + (accelaration * deltaTime);
            float speed = velocity.magnitude;
            Vector3 dir = speed == 0 ? Vector3.zero : velocity / speed;
            speed = Mathf.Clamp(speed, 0.1f, 5);
            velocity = dir * speed;

            velocitiesOut[i] = velocity;
            positionsOut[i] = positionsIn[i] + velocity;
        }

        private Vector3 GetBorderAvoidVelocity(int boid)
        {
            Vector3 oppositeDir = Vector3.zero;
            Vector3 pos = positionsIn[boid];
            float radiusBorder = 0.5f;

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

        private Vector3 GetPlayerAvoidVelocity(int boid, BoidFactionData factionData)
        {
            Vector3 oppositeDir = Vector3.zero;

            Vector3 dir = positionsIn[boid] - playerPosition;
            float distSqr = Vector3.Dot(dir, dir);

            if (distSqr <= factionData.radiusPlayerDetection)
            {
                float distanceWeight = 1 - (distSqr / factionData.radiusPlayerDetection);
                oppositeDir += dir * distanceWeight;
            }

            return oppositeDir;
        }

        private Vector3 GetBoidsAvoidVelocity(int boid, BoidFactionData factionData)
        {
            Vector3 oppositeDir = Vector3.zero;
            Vector3 pos = positionsIn[boid];
            float radiusSquared = factionData.radiusBoidsAvoidance;

            for (int i = 0; i < positionsIn.Length; i++)
            {
                Vector3 dir = pos - positionsIn[i];
                float distSqr = Vector3.Dot(dir, dir);

                if (distSqr >= radiusSquared) continue;

                oppositeDir += dir;
            }

            return oppositeDir;
        }

        private Vector3 GetVelocityMatch(int boid, BoidFactionData factionData)
        {
            Vector3 velocity = Vector3.zero;
            Vector3 pos = positionsIn[boid];
            float radiusSquared = factionData.velocityMatchingWeight;

            int count = 0;
            for (int i = factionData.startIndex; i < factionData.endIndex; i++)
            {
                if (i == boid) continue;

                Vector3 dir = pos - positionsIn[i];
                float distSqr = Vector3.Dot(dir, dir);

                if (distSqr >= radiusSquared) continue;

                velocity += velocitiesIn[i];
                count++;
            }

            if (count != 0)
                velocity /= count;

            return velocity;
        }

        private Vector3 GetFlockCentringVelocity(int boid, BoidFactionData factionData)
        {
            Vector3 center = Vector3.zero;
            Vector3 pos = positionsIn[boid];
            float radiusSquared = factionData.radiusCentring;

            int count = 0;
            for (int i = factionData.startIndex; i < factionData.endIndex; i++)
            {
                if (i == boid) continue;

                Vector3 dir = pos - positionsIn[i];
                float distSqr = Vector3.Dot(dir, dir);

                if (distSqr >= radiusSquared) continue;

                center += positionsIn[i];
                count++;
            }

            if (count != 0)
            {
                center /= count;
                return center - pos;
            }
            else
            {
                return Vector3.zero;
            }
        }

        Vector3 SteerTowards(Vector3 vector, int boidIndex)
        {
            BoidFactionData factionData = boidFactionDatas[boidFactionIndex[boidIndex]];

            Vector3 v = vector.normalized * factionData.maxSpeed - velocitiesIn[boidIndex];
            return Vector3.ClampMagnitude(v, factionData.maxSteerForce);
        }
    }
}


public struct BoidFactionData
{
    public int startIndex;
    public int endIndex;


    public float maxSpeed;
    public float maxSteerForce;

    public float radiusPlayerDetection;
    public float radiusBoidsAvoidance;
    public float radiusVelocityMatching;
    public float radiusCentring;

    public float collidersAvoidanceWeight;
    public float playerAvoidanceWeight;
    public float boidsAvoidanceWeight;
    public float velocityMatchingWeight;
    public float flockCenteringWeight;

    public static BoidFactionData FromScriptableObject(BoidFaction boidFaction)
    {
        return new BoidFactionData
        {
            maxSpeed = boidFaction.maxSpeed,
            maxSteerForce = boidFaction.maxSteerForce,

            radiusPlayerDetection = boidFaction.radiusPlayerDetection,
            radiusBoidsAvoidance = boidFaction.radiusBoidsAvoidance,
            radiusVelocityMatching = boidFaction.radiusVelocityMatching,
            radiusCentring = boidFaction.radiusCentring,

            collidersAvoidanceWeight = boidFaction.collidersAvoidanceWeight,
            playerAvoidanceWeight = boidFaction.playerAvoidanceWeight,
            boidsAvoidanceWeight = boidFaction.boidsAvoidanceWeight,
            velocityMatchingWeight = boidFaction.velocityMatchingWeight,
            flockCenteringWeight = boidFaction.flockCenteringWeight
        };
    }

}

