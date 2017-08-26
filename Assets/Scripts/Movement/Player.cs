using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    Camera cam;
    GameObject selectionObj;

    enum ToolType { shovel, seeds };
    ToolType tool = ToolType.seeds;

    public GameObject plantObj;

    // Use this for initialization
    void Start()
    {
        cam = GetComponentInChildren<Camera>();
        selectionObj = Instantiate(Resources.Load("Selection") as GameObject);
    }

    // Update is called once per frame
    void Update()
    {

        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 5f))
        {
            MeshCollider meshCollider = hit.collider as MeshCollider;
            if (meshCollider == null || meshCollider.sharedMesh == null)
                return;

            Mesh mesh = meshCollider.sharedMesh;
            Vector3[] vertices = mesh.vertices;
            int[] triangles = mesh.triangles;

            int index = triangles[hit.triangleIndex * 3 + 0];
            Vector3 p = vertices[index];

            selectionObj.SetActive(true);
            selectionObj.transform.position = p;

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                tool = ToolType.shovel;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                tool = ToolType.seeds;
            }

            if (Input.GetButton("Fire1"))
            {
                switch (tool)
                {
                    case ToolType.shovel:
                        Game.control.world.map[(int)p.x, (int)p.z].SetDug(true);
                        break;
                    case ToolType.seeds:
                        Game.control.world.map[(int)p.x, (int)p.z].Plant(plantObj);
                        break;
                    default:
                        break;
                }
            }
            if (Input.GetButton("Fire2"))
            {
                switch (tool)
                {
                    case ToolType.shovel:
                        Game.control.world.map[(int)p.x, (int)p.z].SetDug(false);
                        break;
                    default:
                        break;
                }
            }
            if (Input.GetKeyDown(KeyCode.P))
            {
                Game.control.world.map[(int)p.x, (int)p.z].AddWater(1);
            }
            if (Input.GetKeyDown(KeyCode.O))
            {
                print(Game.control.world.map[(int)p.x, (int)p.z].water);
            }
        }
        else
        {
            selectionObj.SetActive(false);
        }
    }
}
