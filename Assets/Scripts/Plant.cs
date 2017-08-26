using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour {

    public WorldPoint point;
    public int numTilesBlocked = 0;

    float energy = 0.1f;
    public float size = 0f;
    Vector3 targetScale = Vector3.zero;
    float maxSize = 3f;
    float growthRate = 0.1f;

    float growCycle = 0f;
    float maxGrowCycle = 1f;

    float initialEnergyUse = 0.01f;
    float initialLightNeeds = 0.01f;
    float energyUse = 0.001f;
    float waterAbsorption = 0.01f;
    float lightNeeds = 0.1f;
    float lightAbsorption = 0.01f;
    float growthNeedsAjust = 300f;

    // Use this for initialization
    void Start () {
        point = Game.control.world.map[(int)transform.position.x, (int)transform.position.z];
        transform.localScale = Vector3.one * size;
    }

    // Update is called once per frame
    void Update () {
		if (growCycle < maxGrowCycle)
        {
            growCycle += Time.deltaTime;
        }
        else
        {
            Grow();
            growCycle = 0f;
        }

        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime);

    }

    void Grow()
    {
        //absorb water
        if (point.water > waterAbsorption)
        {
            point.AddWater(-waterAbsorption);
            energy += waterAbsorption;
        }

        //photosynthesize
        if (point.light > lightNeeds)
        {
            energy += lightAbsorption;
        }

        //grow
        if (energy > energyUse)
        {
            energy -= energyUse;
            size += growthRate;
            if (size > maxSize) size = maxSize;
            targetScale = Vector3.one * size;

            //recalculate needs and energy use
            energyUse = initialEnergyUse + (size / growthNeedsAjust);
            lightNeeds = initialLightNeeds + (size / growthNeedsAjust);

            //change light blocking
            numTilesBlocked = Mathf.RoundToInt(size);
        }
        else
        {
            Die();
        }
    }

    void Die()
    {
        point.plant = null;
        Destroy(gameObject);
    }
}
