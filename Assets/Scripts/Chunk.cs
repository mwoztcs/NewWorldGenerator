using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Chunk : MonoBehaviour {

    private Mesh mesh;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;
    private Material material;

    public void SetUp(Material materialRef) {
        meshFilter = gameObject.GetComponent<MeshFilter>();
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
        meshCollider = gameObject.GetComponent<MeshCollider>();

        if (meshFilter == null)
            meshFilter = gameObject.AddComponent<MeshFilter>();
        if (meshRenderer == null)
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
        if (meshCollider == null)
            meshCollider = gameObject.AddComponent<MeshCollider>();

        mesh = meshFilter.sharedMesh;
        if (mesh == null) {
            mesh = new Mesh();
            meshFilter.sharedMesh = mesh;
        }

        if (meshCollider.sharedMesh == null)
            meshCollider.sharedMesh = mesh;

        material = materialRef;
    }

    public void UpdateMesh(ref Vector3[] vertices, ref int[] triangles) {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
        Vector2[] uvs = new Vector2[vertices.Length];
        
        for (int i = 0; i < triangles.Length; i += 3) {
            Vector3[] v = {vertices[triangles[i]], vertices[triangles[i + 1]], vertices[triangles[i + 2]]};
            Vector3 normal = Vector3.Cross(v[1] - v[0], v[2] - v[0]).normalized;

            Vector3 vDir = new Vector3(0, 0, 1);
            if (Mathf.Abs(normal.y) < 0.99f) {
                vDir = (new Vector3(0, 1, 0) - normal.y * normal).normalized;
            }

            Vector3 uDir = Vector3.Cross(normal, vDir).normalized;
            
            for (int j = 0; j < 3; j++) {
                uvs[triangles[i + j]] = new Vector2(Vector3.Dot(v[j], uDir), Vector3.Dot(v[j], vDir));
            }
        }

        mesh.uv = uvs;

        // force collider update
        meshCollider.enabled = false;
        meshCollider.enabled = true;
        meshRenderer.material = material;
    }
}
