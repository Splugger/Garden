using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{

    public static Game control;

    public World world;

    public GameObject playerObj;
    public DayNightCycle cycle;

    public float timeInDay = 720f;
    int day;
    public int dayOfYear;
    public int daysInYear = 365;

    public int season = 0;
    int seasons = 6;

    // Use this for initialization
    void Awake()
    {
        if (control == null)
        {
            control = this;
        }
        else if (control != this)
        {
            Destroy(gameObject);
        }
        playerObj = GameObject.FindWithTag("Player");
        world = GetComponent<World>();
        cycle = GetComponent<DayNightCycle>();
        cycle.secondsInFullDay = timeInDay;
    }

    // Update is called once per frame
    void Update()
    {
        //seasons
        float percentThroughYear = (float)Game.control.dayOfYear / (float)Game.control.daysInYear;

        if (percentThroughYear == 0)
            season = 0;

        if (seasons - 1 > season)
        {
            if (percentThroughYear > (float)(season + 1) / (float)(seasons))
            {
                season++;
            }
        }
    }

    public void IncrementDays()
    {
        day++;
        dayOfYear = day % daysInYear;
    }
}
