using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targeted : MonoBehaviour
{
    //set in inspector->spell
    public string targetedName;

    [HideInInspector]
    public int length; //how many turns the targeted spell lasts

    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        //setup functionality
        Setup();
    }

    /// <summary>
    /// Every single spell needs to be individually programmed (overlap will exist), do so here if you add a spell,
    /// setup Start() stuff in here
    /// </summary>
    void Setup()
    {
        switch (name)
        {
            //deflects damage from 
            case "iceShard":

                break;
        }
    }

    /// <summary>
    /// Every single spell needs to be individually programmed (overlap will exist), do so here if you add a spell
    /// setup Update() stuff in here
    /// </summary>
    void ActiveState()
    {

    }
}
