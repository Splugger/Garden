using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldPoint
{
    public Vector3 position;
    public float height;
    public float water;
    public float waterRetention = 0.01f;
    public float surfaceWaterHeight = 0.5f;
    public float waterWeight;
    float light;
    public bool isDug = false;
    GameObject holeObj;
    GameObject waterObj;
    MeshFilter mf;

    public WorldPoint(float height, Vector3 position)
    {
        this.height = height;
        this.position = position;

        waterObj = GameObject.Instantiate(Resources.Load("Water") as GameObject);
        waterObj.transform.position = position;
    }
    public void SetDug(bool state)
    {
        isDug = state;
        SetTile(true);
    }

    public void AddWater(float amount)
    {
        water += amount;
        if (water > 0 && (water > surfaceWaterHeight || isDug))
        {
            float height = water - surfaceWaterHeight;
            //if (isDug && height < surfaceWaterHeight) height = 0.1f;
            waterObj.SetActive(true);
            waterObj.transform.localScale = new Vector3(1f, height, 1f);
        }
        else
        {
            waterObj.SetActive(false);
        }

    }

    public void SetObjActive(bool state)
    {
        if (state)
        {
            SetTile(true);
        }
        else
        {
            SetTile(false);
        }
    }

    public void SetTile(bool setNeighbors = false)
    {
        GameObject.Destroy(holeObj);

        WorldPoint[] neighbors = new WorldPoint[4];
        neighbors[0] = Game.control.world.map[(int)position.x, (int)position.z + 1]; // up
        neighbors[1] = Game.control.world.map[(int)position.x + 1, (int)position.z]; // right
        neighbors[2] = Game.control.world.map[(int)position.x, (int)position.z - 1]; // down
        neighbors[3] = Game.control.world.map[(int)position.x - 1, (int)position.z]; // left

        if (setNeighbors)
        {
            foreach (WorldPoint neighbor in neighbors)
            {
                neighbor.SetTile();
            }
        }
        bool[] neighborDugStates = new bool[4];
        neighborDugStates[0] = neighbors[0].isDug; // up
        neighborDugStates[1] = neighbors[1].isDug; // right
        neighborDugStates[2] = neighbors[2].isDug; // down
        neighborDugStates[3] = neighbors[3].isDug; // left


        int neighborCount = neighborDugStates.Where(c => c).Count();

        if (isDug)
        {
            float rotation = 0f;
            string holeName = "";
            RaycastHit hit;
            Physics.Raycast(position + Vector3.up, Vector3.down, out hit);

            switch (neighborCount)
            {
                case 0:
                    holeName = "Hole_Closed";
                    break;
                case 1:
                    holeName = "Hole_End";
                    if (neighborDugStates[0])
                    {
                        rotation = 90f;
                    }
                    if (neighborDugStates[1])
                    {
                        rotation = 180f;
                    }
                    if (neighborDugStates[2])
                    {
                        rotation = 270f;
                    }
                    break;
                case 2:
                    //corners
                    if (neighborDugStates[3] && neighborDugStates[0])
                    {
                        holeName = "Hole_Corner";
                    }
                    if (neighborDugStates[0] && neighborDugStates[1])
                    {
                        holeName = "Hole_Corner";
                        rotation = 90f;
                    }
                    if (neighborDugStates[1] && neighborDugStates[2])
                    {
                        holeName = "Hole_Corner";
                        rotation = 180f;
                    }
                    if (neighborDugStates[2] && neighborDugStates[3])
                    {
                        holeName = "Hole_Corner";
                        rotation = 270f;
                    }

                    // straights
                    if (neighborDugStates[1] && neighborDugStates[3])
                    {
                        holeName = "Hole_Straight";
                    }
                    if (neighborDugStates[0] && neighborDugStates[2])
                    {
                        holeName = "Hole_Straight";
                        rotation = 90f;
                    }
                    break;
                case 3:
                    holeName = "Hole_Edge";
                    if (!neighborDugStates[0])
                    {
                        rotation = 90f;
                    }
                    if (!neighborDugStates[1])
                    {
                        rotation = 180f;
                    }
                    if (!neighborDugStates[2])
                    {
                        rotation = 270f;
                    }
                    break;
                case 4:
                    holeName = "Hole_Open";
                    break;
            }
            holeObj = GameObject.Instantiate(Resources.Load(holeName) as GameObject);
            holeObj.transform.up = hit.normal;
            holeObj.transform.Rotate(new Vector3(0f, rotation, 0f));
            holeObj.transform.position = position;
        }
    }
}
