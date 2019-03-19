﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public float remainingTime;

    // Start is called before the first frame update
    void Start()
    {
        //set the start time
        remainingTime = GetComponent<Entity>().maxTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameData.GameplayPaused && !GameData.FullPaused)
        {
            //entity's turn
            if (GetComponent<Entity>().doingTurn)
            {
                remainingTime -= Time.deltaTime;
            }
            //otherwise reset time
            else
            {
                remainingTime = GetComponent<Entity>().maxTime;
            }
        }
    }
}
