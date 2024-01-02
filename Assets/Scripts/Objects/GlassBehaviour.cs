using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshCollider))]
public class GlassBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject sensorPrefab;
    [SerializeField] private Vector3 splitNormal = Vector3.up; // Normal of the plane for splitting
    private MeshRenderer _MeshRenderer;

    private void Start()
    {
        _MeshRenderer = GetComponent<MeshRenderer>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"GLASS BEHAVIOUR - Triggered with {collision.gameObject.name}");
        // Check if the collision involves an object that can split the mesh
        if (collision.gameObject.name.ToLower().Contains("bullet"))
        {
            // Call the function to split the mesh
            SplitMesh(collision.contacts[0].point);
        }
    }

    void SplitMesh(Vector3 splitPoint)
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            Mesh mesh = meshFilter.mesh;

            // Split the mesh into two based on the cutting plane
            Mesh[] newMeshes = SplitMeshByPlane(mesh, splitPoint, splitNormal);

            // Create game objects for each submesh
            foreach (Mesh submesh in newMeshes)
            {
                GameObject submeshObject = new GameObject("Submesh");
                submeshObject.transform.position = transform.position;
                submeshObject.transform.rotation = transform.rotation;

                MeshFilter submeshFilter = submeshObject.AddComponent<MeshFilter>();
                submeshFilter.mesh = submesh;

                MeshRenderer submeshRenderer = submeshObject.AddComponent<MeshRenderer>();
                submeshRenderer.material = _MeshRenderer.material;

                // Optional: Add a collider to each submesh if needed
                submeshObject.AddComponent<MeshCollider>();
            }

            // Destroy the original object
            Destroy(gameObject);
        }
    }

    Mesh[] SplitMeshByPlane(Mesh originalMesh, Vector3 splitPoint, Vector3 splitNormal)
    {
        // Get the vertices, triangles, and other mesh data
        Vector3[] vertices = originalMesh.vertices;
        int[] triangles = originalMesh.triangles;
        Vector3[] normals = originalMesh.normals;
        Vector2[] uv = originalMesh.uv;

        // Lists to store the vertices and triangles for each side of the cut
        var leftVertices = new System.Collections.Generic.List<Vector3>();
        var leftTriangles = new System.Collections.Generic.List<int>();
        var rightVertices = new System.Collections.Generic.List<Vector3>();
        var rightTriangles = new System.Collections.Generic.List<int>();

        // Determine which side of the cutting plane each vertex is on
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 vertex = vertices[i];
            Vector3 vertexWorldPos = transform.TransformPoint(vertex);
            Vector3 toVertex = vertexWorldPos - splitPoint;
            bool isOnLeftSide = Vector3.Dot(toVertex, splitNormal) > 0;

            if (isOnLeftSide)
            {
                leftVertices.Add(vertexWorldPos);
            }
            else
            {
                rightVertices.Add(vertexWorldPos);
            }
        }

        // Update triangle indices for each side
        for (int i = 0; i < triangles.Length; i += 3)
        {
            int i0 = triangles[i];
            int i1 = triangles[i + 1];
            int i2 = triangles[i + 2];

            if (IsOnSameSide(vertices[i0], vertices[i1], vertices[i2], splitPoint, splitNormal))
            {
                // Triangle is on the left side
                leftTriangles.Add(i0);
                leftTriangles.Add(i1);
                leftTriangles.Add(i2);
            }
            else
            {
                // Triangle is on the right side
                rightTriangles.Add(i0);
                rightTriangles.Add(i1);
                rightTriangles.Add(i2);
            }
        }

        // Create new meshes for each side
        Mesh leftMesh = new Mesh();
        leftMesh.vertices = leftVertices.ToArray();
        leftMesh.triangles = leftTriangles.ToArray();
        leftMesh.normals = normals;
        leftMesh.uv = uv;

        Mesh rightMesh = new Mesh();
        rightMesh.vertices = rightVertices.ToArray();
        rightMesh.triangles = rightTriangles.ToArray();
        rightMesh.normals = normals;
        rightMesh.uv = uv;

        return new Mesh[] { leftMesh, rightMesh };
    }

    bool IsOnSameSide(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 planePoint, Vector3 planeNormal)
    {
        // Determine if all three points are on the same side of the plane
        float side0 = Vector3.Dot(planeNormal, p0 - planePoint);
        float side1 = Vector3.Dot(planeNormal, p1 - planePoint);
        float side2 = Vector3.Dot(planeNormal, p2 - planePoint);

        return (side0 > 0 && side1 > 0 && side2 > 0) || (side0 <= 0 && side1 <= 0 && side2 <= 0);
    }
}
