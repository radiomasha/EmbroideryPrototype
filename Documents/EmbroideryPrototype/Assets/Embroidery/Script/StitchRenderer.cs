using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StitchRenderer : MonoBehaviour
{
    public Material _lineMaterial;
    [SerializeField] private float _lineThickness = 0.0005f;
    [SerializeField] private float stitchAngle = 90f; 
    [SerializeField] private ColorPicker _colorPicker;

    public GameObject CreateStitch(Vector3 start, Vector3 end, Color color, Collider planeCollider, float offset = 0.0005f)
    {
        start = planeCollider.ClosestPoint(start);
        end = planeCollider.ClosestPoint(end);
        Vector3 normal = planeCollider.transform.up; 
        start += normal * offset;
        end += normal * offset;
        GameObject lineObject = new GameObject("Stitch");
        MeshFilter meshFilter = lineObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = lineObject.AddComponent<MeshRenderer>();
        
        Mesh mesh = new Mesh();
        Vector3 lineDirection =(end - start).normalized;
        Vector3 vertDir = Quaternion.AngleAxis(stitchAngle, lineDirection) * 
            Vector3.Cross(lineDirection, Vector3.up).normalized * (_lineThickness / 2);
        Vector3[] vertices = new Vector3[]
        {
            start - vertDir,
            start + vertDir,
            end - vertDir,
            end + vertDir
        };
        int[] triangles = new int[]
        {
            0, 1, 2,
            1, 3, 2
        };
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        

        Color[] colors = new Color[]
        {
            color, color, color, color
        };
        mesh.colors = colors;
        meshFilter.mesh = mesh;
        meshRenderer.material = _lineMaterial;
        meshRenderer.material.color = _colorPicker._sphere.GetComponent<Renderer>().material.color;
        return lineObject;
    }
}
