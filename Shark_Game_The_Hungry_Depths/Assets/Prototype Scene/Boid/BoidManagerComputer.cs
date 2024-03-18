using UnityEngine;
using UnityEngine.Rendering;

public class BoidManagerComputer : MonoBehaviour
{
    public ComputeShader csBoidRules;

    [HideInInspector]
    public Vector3[] velocities;
    public ComputeBuffer cbVelocities;

    [HideInInspector]
    public Vector3[] positions;
    private ComputeBuffer cbPositions;

    [HideInInspector]
    public GameObject[] boids;

    [HideInInspector]
    public Vector3[] avoidance;
    private ComputeBuffer cbAvoidance;

    [HideInInspector]
    public Vector3[] velocityMatching;
    private ComputeBuffer cbVelocityMatching;

    [HideInInspector]
    public Vector3[] flockCentring;
    private ComputeBuffer cbFlockCentring;

    public void Init(GameObject[] boids)
    {
        this.boids = boids;

        velocities = new Vector3[boids.Length];
        for (int i = 0; i < velocities.Length; i++)
            velocities[i] = Random.insideUnitSphere;
        cbVelocities = new ComputeBuffer(boids.Length, sizeof(float) * 3);

        positions = new Vector3[boids.Length];
        cbPositions = new ComputeBuffer(boids.Length, sizeof(float) * 3);

        avoidance = new Vector3[boids.Length];
        cbAvoidance = new ComputeBuffer(boids.Length, sizeof(float) * 3);

        velocityMatching = new Vector3[boids.Length];
        cbVelocityMatching = new ComputeBuffer(boids.Length, sizeof(float) * 3);

        flockCentring = new Vector3[boids.Length];
        cbFlockCentring = new ComputeBuffer(boids.Length, sizeof(float) * 3);
    }

    public void Deinit()
    {
        cbVelocities.Dispose();
        cbPositions.Dispose();
        cbAvoidance.Dispose();
        cbVelocityMatching.Dispose();
        cbFlockCentring.Dispose();
    }

    public void CalculateRules(float radiusSquaredAvoidance, float radiusSquaredMatching, float radiusSquaredCentring)
    {
        for (int i = 0; i < boids.Length; i++)
        {
            if (boids[i] == null)
            {
                positions[i] = new Vector3(-10000, -10000, -10000);
            }
            else
            {
                positions[i] = boids[i].transform.position;
            }
        }

        // Inputs
        csBoidRules.SetInt("amountOfBoids", boids.Length);
        csBoidRules.SetFloat("radiusAvoidanceSquared", radiusSquaredAvoidance);
        csBoidRules.SetFloat("radiusVelocitySquared", radiusSquaredMatching);
        csBoidRules.SetFloat("radiusCentringSquared", radiusSquaredCentring);

        cbVelocities.SetData(velocities);
        csBoidRules.SetBuffer(0, "velocities", cbVelocities);

        cbPositions.SetData(positions);
        csBoidRules.SetBuffer(0, "positions", cbPositions);

        // Outputs
        csBoidRules.SetBuffer(0, "avoidance", cbAvoidance);
        csBoidRules.SetBuffer(0, "velocityMatching", cbVelocityMatching);
        csBoidRules.SetBuffer(0, "flockCentring", cbFlockCentring);




        // Dispatch

        /*
            We might need to dispatch multiple times and this is what the
            startingIndexOffset is for. We are only using the x workers size.

            So if we have 200 boids but a x workers thread size of 50, we need
            to dispatch 4 dimes (200/50).
         */

        int dispatchTimes = (int)(boids.Length / SystemInfo.maxComputeWorkGroupSizeX) + 1;

        for (int i = 0; i < dispatchTimes; i++)
        {
            csBoidRules.SetInt("startingIndexOffset", i * SystemInfo.maxComputeWorkGroupSizeX);

            int xWorkSize = boids.Length > SystemInfo.maxComputeWorkGroupSizeX ? (SystemInfo.maxComputeWorkGroupSizeX / 8) : (boids.Length / 8) + 1;
            csBoidRules.Dispatch(0, xWorkSize, 1, 1);
        }

        // Get Data
        cbAvoidance.GetData(avoidance);
        cbVelocityMatching.GetData(velocityMatching);
        cbFlockCentring.GetData(flockCentring);
    }

    private void OnDisable()
    {
        if (cbVelocities != null) // stupid hack, dont keep this
            Deinit();
    }


}
