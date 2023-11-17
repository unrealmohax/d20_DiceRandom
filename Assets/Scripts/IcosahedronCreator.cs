using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class IcosahedronCreator
{
    private readonly Transform _transform;
    private readonly MeshFilter _meshFilter;
    private readonly MeshRenderer _meshRenderer;
    private readonly Material _material;
    private readonly ParticleSystem _particleSystem;
    
    public IcosahedronCreator(Transform transform, MeshFilter meshFilter, MeshRenderer meshRenderer, Material material, ParticleSystem particleSystem)
    {
        _material = material ?? throw new Exception("Error when creating IcosahedronCreator (Material - null)");
        _transform = transform ?? throw new Exception("Error when creating IcosahedronCreator (Transform - null)");
        _meshFilter = meshFilter ?? throw new Exception("Error when creating IcosahedronCreator (MeshFilter - null)");
        _meshRenderer = meshRenderer ?? throw new Exception("Error when creating IcosahedronCreator (MeshRenderer - null)");
        _particleSystem = particleSystem ?? throw new Exception("Error when creating IcosahedronCreator (ParticleSystem - null)");
    }

    public void CreateIcosahedron(ref List<TextMeshPro> TextMeshPros)
    {
        float phi = (1f + Mathf.Sqrt(5f)) / 2f;

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

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        SetNormals(mesh);
        _meshFilter.mesh = mesh;

        SerParticleMesh(_particleSystem, mesh);

        _meshRenderer.material = _material;
        SetText(vertices, triangles, ref TextMeshPros);
    }

    private void SerParticleMesh(ParticleSystem particleSystem, Mesh mesh)
    {
        var shape = particleSystem.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Mesh;
        shape.mesh = mesh;
    }

    private void SetText(Vector3[] vertices, int[] triangles, ref List<TextMeshPro> TextMeshPros)
    {
        for (int i = 0; i < triangles.Length; i += 3)
        {
            GameObject textObject = new GameObject("Text" + i / 3);
            textObject.transform.parent = _transform;
            textObject.transform.localPosition = Vector3.zero;
            textObject.layer = 6;

            TextMeshPro textMesh = textObject.AddComponent<TextMeshPro>();
            TextMeshPros.Add(textMesh);

            RectTransform textObjecRect = textObject.GetComponent<RectTransform>();
            textObjecRect.sizeDelta = new Vector2(1, 1);

            textMesh.text = (i / 3 + 1).ToString();
            textMesh.fontSize = 8;
            textMesh.alignment = TextAlignmentOptions.Center;

            int vertexIndex1 = triangles[i];
            int vertexIndex2 = triangles[i + 1];
            int vertexIndex3 = triangles[i + 2];

            Vector3 center = (vertices[vertexIndex1] + vertices[vertexIndex2] + vertices[vertexIndex3]) / 3f * 1.0001f + _transform.position;
            center = new Vector3(center.x * _transform.localScale.x, center.y * _transform.localScale.y, center.z * _transform.localScale.z);
            textObject.transform.position = center;

            textObject.transform.LookAt(_transform.position);
        }
    }

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
