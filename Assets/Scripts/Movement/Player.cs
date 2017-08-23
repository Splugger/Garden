using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    Camera cam;
    GameObject selectionObj;

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
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 5f, LayerMask.NameToLayer("Terrain")))
        {
            MeshCollider meshCollider = hit.collider as MeshCollider;
            if (meshCollider == null || meshCollider.sharedMesh == null)
                return;

            Mesh mesh = meshCollider.sharedMesh;
            Vector3[] vertices = mesh.vertices;
            int[] triangles = mesh.triangles;

            Vector3 p = vertices[triangles[hit.triangleIndex * 3 + 0]];

            selectionObj.SetActive(true);
            selectionObj.transform.position = p;

            if (Input.GetButton("Fire1"))
            {
                Game.control.world.map[(int)p.x, (int)p.z].SetDug(true);
            }
            if (Input.GetButton("Fire2"))
            {
                Game.control.world.map[(int)p.x, (int)p.z].SetDug(false);
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
