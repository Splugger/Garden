using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{

    public static GameObject Generate(int width, int height, float[,] heightMap, float heightScale, Material material)
    {
        GameObject go = new GameObject();
        go.transform.position = Vector3.zero;
        go.name = "Ground";
        MeshRenderer mr = go.AddComponent<MeshRenderer>();
        MeshFilter mf = go.AddComponent<MeshFilter>();
        MeshCollider mc = go.AddComponent<MeshCollider>();

        Mesh m = new Mesh();

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                vertices.Add(new Vector3(x, heightMap[x,y] * heightScale, y));
                uvs.Add(new Vector2(x, y));

                // Don't generate a triangle if it would be out of bounds.
                if (x - 1 <= 0 || y <= 0 || x >= width)
                {
                    continue;
                }
                // Generate the triangle north of you.
                triangles.Add((x - 1) + (y - 1) * width);
                triangles.Add(x + (y - 1) * width);
                triangles.Add(x + y * width);

                // Generate the triangle northwest of you.
                if (x - 1 <= 0 || y <= 0)
                {
                    continue;
                }
                triangles.Add((x - 1) + y * width);
                triangles.Add((x - 1) + (y - 1) * width);
                triangles.Add(x + y * width);
            }
        }

        m.SetVertices(vertices);
        m.uv = uvs.ToArray();
        m.triangles = triangles.ToArray();
        m.RecalculateNormals();

        mf.mesh = m;
        mr.material = material;
        mc.sharedMesh = m;

        return go;
    }
}