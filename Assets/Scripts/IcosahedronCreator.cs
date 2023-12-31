using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class IcosahedronCreator
{
    // Private fields to store necessary references
    private readonly Transform _transform;       // Transform of the icosahedron
    private readonly MeshFilter _meshFilter;     // MeshFilter component for the icosahedron
    private readonly MeshRenderer _meshRenderer; // MeshRenderer component for the icosahedron
    private readonly Material _material;         // Material for the icosahedron

    // Constructor for initializing the creator with required references
    public IcosahedronCreator(Transform transform, MeshFilter meshFilter, MeshRenderer meshRenderer, Material material)
    {
        // Ensure that the material and references are not null
        _material = material ?? throw new Exception("Error when creating IcosahedronCreator (Material - null)");
        _transform = transform ?? throw new Exception("Error when creating IcosahedronCreator (Transform - null)");
        _meshFilter = meshFilter ?? throw new Exception("Error when creating IcosahedronCreator (MeshFilter - null)");
        _meshRenderer = meshRenderer ?? throw new Exception("Error when creating IcosahedronCreator (MeshRenderer - null)");
    }

    // Method to create the icosahedron mesh and associated TextMesh components
    public void CreateIcosahedron(ref List<TextMeshPro> TextMeshPros)
    {
        // Phi value for icosahedron construction
        float phi = (1f + Mathf.Sqrt(5f)) / 2f;

        // Vertices for the icosahedron
        Vector3[] vertices = new Vector3[12]
        {
            new Vector3(-1, phi, 0),
            new Vector3(1, phi, 0),
            new Vector3(-1, -phi, 0),
            new Vector3(1, -phi, 0),

            new Vector3(0, -1, phi),
            new Vector3(0, 1, phi),
            new Vector3(0, -1, -phi),
            new Vector3(0, 1, -phi),

            new Vector3(phi, 0, -1),
            new Vector3(phi, 0, 1),
            new Vector3(-phi, 0, -1),
            new Vector3(-phi, 0, 1),
        };

        // Triangles for the icosahedron
        int[] triangles = {
            0, 11, 5,
            0, 5, 1,
            0, 1, 7,
            0, 7, 10,
            0, 10, 11,

            1, 5, 9,
            5, 11, 4,
            11, 10, 2,
            10, 7, 6,
            7, 1, 8,

            3, 9, 4,
            3, 4, 2,
            3, 2, 6,
            3, 6, 8,
            3, 8, 9,

            4, 9, 5,
            2, 4, 11,
            6, 2, 10,
            8, 6, 7,
            9, 8, 1
        };

        // Create a new mesh and set its vertices and triangles
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        SetNormals(mesh);
        _meshFilter.mesh = mesh;

        // Set material and generate TextMeshPro components for each face
        _meshRenderer.material = _material;
        SetText(vertices, triangles, ref TextMeshPros);
    }

    // Method to set TextMeshPro components on each face of the icosahedron
    private void SetText(Vector3[] vertices, int[] triangles, ref List<TextMeshPro> TextMeshPros)
    {
        // Iterate through the triangles to create TextMeshPro components
        for (int i = 0; i < triangles.Length; i += 3)
        {
            // Create a new GameObject for text
            GameObject textObject = new GameObject("Text" + i / 3);
            textObject.transform.parent = _transform;
            textObject.transform.localPosition = Vector3.zero;
            textObject.layer = 6;

            // Add TextMeshPro component to the GameObject
            TextMeshPro textMesh = textObject.AddComponent<TextMeshPro>();
            TextMeshPros.Add(textMesh);

            // Set RectTransform size for the text
            RectTransform textObjecRect = textObject.GetComponent<RectTransform>();
            textObjecRect.sizeDelta = new Vector2(1.5f, 1.5f);

            // Set text, font size, and alignment
            textMesh.text = (i / 3 + 1).ToString();
            textMesh.fontSize = 10;
            textMesh.alignment = TextAlignmentOptions.Center;

            // Get vertex indices of the current triangle
            int vertexIndex1 = triangles[i];
            int vertexIndex2 = triangles[i + 1];
            int vertexIndex3 = triangles[i + 2];

            // Calculate the center of the triangle
            Vector3 center = (vertices[vertexIndex1] + vertices[vertexIndex2] + vertices[vertexIndex3]) / 3f * 1.0001f + _transform.position;
            center = new Vector3(center.x * _transform.localScale.x, center.y * _transform.localScale.y, center.z * _transform.localScale.z);
            textObject.transform.position = center;

            // Make the textObject face towards the center of the icosahedron
            textObject.transform.LookAt(_transform.position, vertices[vertexIndex3]);
        }
    }

    // Method to set normals for the mesh
    private void SetNormals(Mesh mesh)
    {
        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = new Vector3[vertices.Length];

        for (int i = 0; i < normals.Length; i++)
        {
            normals[i] = vertices[i].normalized;
        }

        mesh.normals = normals;
    }
}
