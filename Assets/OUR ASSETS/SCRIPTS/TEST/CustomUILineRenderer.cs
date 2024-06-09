using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class CustomUILineRenderer : MonoBehaviour
{
    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    public void DrawLine(Vector3 start, Vector3 end, Color color, float width)
    {
        vertices = new Vector3[4];
        triangles = new int[6];

        // Define vertices
        Vector3 perpendicular = Vector3.Cross((end - start).normalized, Vector3.forward).normalized * width / 2f;
        vertices[0] = start + perpendicular;
        vertices[1] = start - perpendicular;
        vertices[2] = end + perpendicular;
        vertices[3] = end - perpendicular;

        // Define triangles
        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;

        triangles[3] = 2;
        triangles[4] = 1;
        triangles[5] = 3;

        // Assign vertices and triangles to the mesh
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        // Set color and width
        GetComponent<MeshRenderer>().material.SetColor("_Color", color);
        GetComponent<MeshRenderer>().material.SetFloat("_Width", width);
    }
}
