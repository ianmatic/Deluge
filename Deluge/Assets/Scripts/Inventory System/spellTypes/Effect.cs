using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{
    //set in inspector->spell
    [HideInInspector]
    public string effectName;

    private GameObject player;

    [HideInInspector]
    public bool active;

    [HideInInspector]
    public int length; //how many turns the effect lasts

    private int turnsRemaining; //how many turns the effect has left

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        EventManager.OnPlayerTurn += UpdateTurn;

        //setup functionality
        Setup();
        active = false;
        turnsRemaining = length;
    }

    // Update is called once per frame
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
            //deflects damage from enemy attacks, lasts 
            case "deflection":

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
