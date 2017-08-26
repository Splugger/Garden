using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorBySeason : MonoBehaviour
{

    public Color[] colors;
    public Material mat;
    Color targetColor;
    float randomOffset = 5f;

    int season = 0;

    void Start()
    {
        mat = GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        targetColor = colors[Game.control.season];
        mat.color = Color.Lerp(mat.color, targetColor, Time.deltaTime * Random.Range(0f, randomOffset));
    }
}
