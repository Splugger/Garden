using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class World : MonoBehaviour
{

    public WorldPoint[,] map;
    public GameObject worldObj;
    public MeshFilter worldMesh;

    int width = 100;
    int height = 100;
    float[,] heightMap;
    float heightScale = 10f;
    int seed;
    float scale = 100;
    int octaves = 5;
    float persistence = 0.4f;
    float lacunarity = 2.2f;
    public Material material;

    float maxWaterFlowRate = 2f;
    float dugWaterOffset = 0.1f;


    public Color[] seasonalColors;

    // Use this for initialization
    void Start()
    {
        GenerateWorld();

        ColorBySeason colorBySeason = worldObj.AddComponent<ColorBySeason>();
        colorBySeason.colors = seasonalColors;

        InvokeRepeating("SimulateTiles", 0f, 0.1f);
        InvokeRepeating("Evaporate", 0f, 1f);
    }

    public void SetMeshVertex(int index, Vector3 offset)
    {
        List<Vector3> vertices = worldMesh.mesh.vertices.ToList();
        vertices[index] += offset;
        worldMesh.mesh.SetVertices(vertices);
    }

    void GenerateWorld()
    {
        map = new WorldPoint[width, height];
        seed = DateTime.Now.Millisecond;
        heightMap = Noise.GenerateNoiseMap(width, height, seed, scale, octaves, persistence, lacunarity, Noise.NormalizeMode.global, false);

        worldObj = MeshGenerator.Generate(width, height, heightMap, heightScale, material);
        worldObj.layer = LayerMask.NameToLayer("Terrain");
        worldMesh = worldObj.GetComponent<MeshFilter>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                map[x, y] = new WorldPoint(heightMap[x, y], new Vector3(x, heightMap[x, y] * heightScale, y));
            }
        }

        //Rain(0.4f);
    }

    void Rain(float amount)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                map[x, y].AddWater(amount);
            }
        }
    }

    void Evaporate()
    {
        float amount = 0.001f;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                WorldPoint point = map[x, y];
                float addAmount = -Mathf.Min(point.water, amount);
                if (point.water > point.surfaceWaterHeight)
                    point.AddWater(addAmount);
                else
                    point.AddWater(addAmount / 5);
            }
        }
    }

    /*private void OnDrawGizmos()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Gizmos.color = new Color(map[x, y].light, map[x, y].light, map[x, y].light, 1f);
                Gizmos.DrawCube(map[x, y].position, Vector3.one);
            }
        }
    }*/

    void SimulateTiles()
    {
        List<WorldPoint> points = new List<WorldPoint>();
        float[,] waterChange = new float[width, height];

        //first pass
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                WorldPoint point = map[x, y];
                points.Add(point);
                //water
                point.waterWeight = CalculateWaterWeight(map[x, y]);
            }
        }

        //light
        WorldPoint[] pointsByTreeHeight = points.Where(q => q.plant != null).OrderBy(q => q.plant.size).ToArray();
        foreach (WorldPoint point in pointsByTreeHeight)
        {
            int x = (int)point.position.x;
            int y = (int)point.position.z;

            //set light to sun level
            point.light = Game.control.cycle.intensityMultiplier;

            //block neighbor tiles
            for (int neighborX = x - point.plant.numTilesBlocked; neighborX < x + point.plant.numTilesBlocked; neighborX++)
            {
                for (int neighborY = y - point.plant.numTilesBlocked; neighborY < y + point.plant.numTilesBlocked; neighborY++)
                {
                    Game.control.world.map[neighborX, neighborY].light *= 0.2f;
                }
            }
        }

        //second pass
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                SimulateWaterTile(x, y, ref waterChange);
            }
        }

        //third pass
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                map[x, y].AddWater(waterChange[x, y]);
            }
        }
    }

    void SimulateWaterTile(int mapX, int mapY, ref float[,] waterChange)
    {
        WorldPoint point = map[mapX, mapY];
        if (point.water <= point.waterRetention && !point.isDug) return;

        var neighbors = GetNeighbors(point);

        WorldPoint destination = neighbors.OrderBy(q => q.waterWeight).First();
        float minWeight = destination.waterWeight;

        if (point.waterWeight < minWeight || point.water <= point.waterRetention) return;

        float difference = point.waterWeight - minWeight;
        float waterFlowRate = (difference < maxWaterFlowRate) ? difference : maxWaterFlowRate;

        float flow = 0;

        if (point.water > waterFlowRate)
        {
            flow = waterFlowRate;
        }
        else
        {
            flow = point.water;
        }

        waterChange[mapX, mapY] -= flow;
        waterChange[(int)destination.position.x, (int)destination.position.z] += flow;

    }

    List<WorldPoint> GetNeighbors(WorldPoint point)
    {
        int x = (int)point.position.x;
        int y = (int)point.position.z;
        List<WorldPoint> neighbors = new List<WorldPoint>();

        if (y < height - 1)
        {
            WorldPoint up = map[x, y + 1];
            neighbors.Add(up);
        }
        if (x < width - 1)
        {
            WorldPoint right = map[x + 1, y];
            neighbors.Add(right);
        }
        if (y > 0)
        {
            WorldPoint down = map[x, y - 1];
            neighbors.Add(down);
        }
        if (x > 0)
        {
            WorldPoint left = map[x - 1, y];
            neighbors.Add(left);
        }

        return neighbors;
    }

    float CalculateWaterWeight(WorldPoint point)
    {
        float weight = point.height + point.water / 4;
        weight += point.isDug ? 0 : dugWaterOffset;

        return weight;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
