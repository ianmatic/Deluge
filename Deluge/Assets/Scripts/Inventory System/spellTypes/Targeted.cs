using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targeted : MonoBehaviour
{
    //set in inspector->spell
    [HideInInspector]
    public string targetedName;

    [HideInInspector]
    public int length; //how many turns the targeted spell lasts
    [HideInInspector]
    public bool active;

    private GameObject player;

    private int turnsRemaining; //how many turns the effect has left



    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        active = false;
        turnsRemaining = length;

        EventManager.OnPlayerTurn += UpdateTurn;

        //setup functionality
        Setup();
    }

    void Update()
    {
        //active spell, so do purpose
        if (active)
        {
            ActiveState();
        }
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
        turnsRemaining -= 1;

        if (turnsRemaining == -1)
        {
            active = false;
            turnsRemaining = length;
        }
    }

    /// <summary>
    /// Activates the uses of the spell
    /// </summary>
    public void Activate()
    {
        active = true;
    }

    /// <summary>
    /// Updates the amount of turns left
    /// </summary>
    public void UpdateTurn()
    {
        turnsRemaining -= 1;

        if (turnsRemaining == -1)
        {
            active = false;
            turnsRemaining = length;
        }
    }

    private void OnDestroy()
    {
        EventManager.OnPlayerTurn -= UpdateTurn;
    }
}
