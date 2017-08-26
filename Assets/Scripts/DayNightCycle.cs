using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{

    public Light sun;
    public float secondsInFullDay = 120f;
    public float intensityMultiplier = 1;

    [Range(0, 1)]
    public float currentTimeOfDay = 0;
    [HideInInspector]
    public float timeMultiplier = 1f;
    public GameObject stars;
    Material starsMat;
    float targetStarAlpha;

    float sunInitialIntensity;

    void Start()
    {
        starsMat = stars.GetComponent<MeshRenderer>().material;

        sunInitialIntensity = sun.intensity;
    }

    void Update()
    {
        UpdateSun();

        currentTimeOfDay += (Time.deltaTime / secondsInFullDay) * timeMultiplier;

        if (currentTimeOfDay >= 1)
        {
            Game.control.IncrementDays();
            currentTimeOfDay = 0;
        }

        stars.transform.position = Game.control.playerObj.transform.position;
    }

    void UpdateSun()
    {
        sun.transform.localRotation = Quaternion.Euler((currentTimeOfDay * 360f) - 90, 170, 0);
        if (currentTimeOfDay <= 0.23f || currentTimeOfDay >= 0.75f)
        {
            intensityMultiplier = 0;
        }
        else if (currentTimeOfDay <= 0.25f)
        {
            intensityMultiplier = Mathf.Clamp01((currentTimeOfDay - 0.23f) * (1 / 0.02f));
        }
        else if (currentTimeOfDay >= 0.73f)
        {
            intensityMultiplier = Mathf.Clamp01(1 - ((currentTimeOfDay - 0.73f) * (1 / 0.02f)));
        }

        sun.intensity = sunInitialIntensity * intensityMultiplier;

        //stars
        starsMat.color = Color.Lerp(starsMat.color, new Color(starsMat.color.r, starsMat.color.g, starsMat.color.b, 1 - intensityMultiplier), Time.deltaTime);
    }
}
