using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AquariumBuilderPrototype : MonoBehaviour
{
    public GameObject invisCube;
    public GameObject cubeAsEdgePrefab;
    public GameObject plane;
    public Vector3 cubeBoundaries;
    public Bounds boundsCached;

    private void Start()
    {
        invisCube.GetComponent<MeshRenderer>().enabled = true;
        SpawnBoundaries();
        invisCube.GetComponent<MeshRenderer>().enabled = false;
    }

    private void OnValidate()
    {
        if (invisCube == null) Debug.LogWarning("cube not set", this);
        invisCube.transform.localScale = cubeBoundaries;

        Bounds bounds = GetWorldBounds(invisCube);
        plane.transform.position = bounds.center + new Vector3(0f, -bounds.extents.y, 0f);
        plane.transform.localScale = new Vector3(bounds.size.x / 10, 1f, bounds.size.z / 10);
    }

    private void SpawnBoundaries()
    {
        Bounds bounds = GetWorldBounds(invisCube);

        // Spawn box colliders at each face of the cube
        SpawnBoxCollider(bounds.center + new Vector3(bounds.extents.x, 0f, 0f), new Vector3(0.1f, bounds.size.y, bounds.size.z)); // Right
        SpawnBoxCollider(bounds.center + new Vector3(-bounds.extents.x, 0f, 0f), new Vector3(0.1f, bounds.size.y, bounds.size.z)); // Left
        SpawnBoxCollider(bounds.center + new Vector3(0f, bounds.extents.y, 0f), new Vector3(bounds.size.x, 0.1f, bounds.size.z)); // Top
        SpawnBoxCollider(bounds.center + new Vector3(0f, -bounds.extents.y, 0f), new Vector3(bounds.size.x, 0.1f, bounds.size.z)); // Bottom
        SpawnBoxCollider(bounds.center + new Vector3(0f, 0f, bounds.extents.z), new Vector3(bounds.size.x, bounds.size.y, 0.1f)); // Front
        SpawnBoxCollider(bounds.center + new Vector3(0f, 0f, -bounds.extents.z), new Vector3(bounds.size.x, bounds.size.y, 0.1f)); // Back


        SpawnCubeEdge(bounds.center + new Vector3(bounds.extents.x, 0f, bounds.extents.z), new Vector3(0.1f, bounds.size.y, 0.1f));
        SpawnCubeEdge(bounds.center + new Vector3(-bounds.extents.x, 0f, -bounds.extents.z), new Vector3(0.1f, bounds.size.y, 0.1f));
        SpawnCubeEdge(bounds.center + new Vector3(-bounds.extents.x, 0f, bounds.extents.z), new Vector3(0.1f, bounds.size.y, 0.1f));
        SpawnCubeEdge(bounds.center + new Vector3(bounds.extents.x, 0f, -bounds.extents.z), new Vector3(0.1f, bounds.size.y, 0.1f));


        SpawnCubeEdge(bounds.center + new Vector3(bounds.extents.x, bounds.extents.y, 0), new Vector3(0.1f, 0.1f, bounds.size.z));
        SpawnCubeEdge(bounds.center + new Vector3(-bounds.extents.x, bounds.extents.y, 0), new Vector3(0.1f, 0.1f, bounds.size.z));

        SpawnCubeEdge(bounds.center + new Vector3(0, bounds.extents.y, bounds.extents.z), new Vector3(bounds.size.x, 0.1f, 0.1f));
        SpawnCubeEdge(bounds.center + new Vector3(0, bounds.extents.y, -bounds.extents.z), new Vector3(bounds.size.x, 0.1f, 0.1f));
    }

    BoxCollider SpawnBoxCollider(Vector3 position, Vector3 size)
    {
        GameObject boxColliderObject = new GameObject("BoundaryCollider");
        boxColliderObject.transform.localPosition = position;
        var boxCollider = boxColliderObject.AddComponent<BoxCollider>();
        boxCollider.size = size;

        boxColliderObject.transform.parent = transform; // Make it a child of this object for organization
        return boxCollider;
    }

    void SpawnCubeEdge(Vector3 position, Vector3 size)
    {
        GameObject cube = Instantiate(cubeAsEdgePrefab, transform);
        cube.transform.localPosition = position;
        cube.transform.localScale = size;
    }



    Bounds GetWorldBounds(GameObject go)
    {
        Bounds bounds = new Bounds(go.transform.position, Vector3.zero);
        Renderer[] renderers = go.GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            bounds.Encapsulate(renderer.bounds);
        }
        boundsCached = bounds;
        return bounds;
    }

}
