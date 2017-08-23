using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorBySeason : MonoBehaviour
{

    public Color[] colors;
    public Material mat;

    int season = 0;

    void Start()
    {
        mat = GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        mat.color = colors[Game.control.season];
    }
}
