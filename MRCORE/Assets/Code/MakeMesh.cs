using UnityEngine;

public class MakeMesh : MonoBehaviour
{
    public void MakeFill(Vector3[] points)
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.Clear();

        int[] triangles = new int[(points.Length - 2) * 3];

        int triIndex = 0;
        for (int i = 1; i < points.Length - 1; i++)
        {
            triangles[triIndex++] = 0;
            triangles[triIndex++] = i;
            triangles[triIndex++] = i + 1;
        }

        mesh.vertices = points;
        mesh.triangles = triangles;
    }
}