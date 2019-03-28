using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : MonoBehaviour
{
    [HideInInspector]
    //not going to use for now
    public enum SpellType
    {
        enhancement, //sword does more vamp damage, bigger range etc.
        effect,  //healing, more resistance, etc.
        targeted //fireball, ice vortex, etc.
    }

    [HideInInspector]
    public bool active;

    //set in inspector
    [HideInInspector] //not going to use for now
    public SpellType type;

    //set in inspector
    public string spellName;

    //set in inspector, how many turns this spell lasts when cast
    public int length;

    protected int turnsRemaining; //how many turns the effect has left

    private GameObject parent;

    // Start is called before the first frame update
    void Start()
    {
        turnsRemaining = length;

        parent = GameObject.FindGameObjectWithTag("Player");

        EventManager.OnPlayerTurn += UpdateTurn;

        Setup();
    }

    void Update()
    {
        if (!parent.GetComponent<Entity>().inCombat)
        {
            turnsRemaining = length;
            active = false;
        }

        //active spell, so do purpose
        if (active)
        {
            ActiveState();
        }
        //unactive spell, so reset
        else
        {
            UnActiveState();
        }
    }

    //remove capability from player
    void OnDestroy()
    {
        EventManager.OnPlayerTurn -= Update;
    }

    /// <summary>
    /// Every single spell needs to be individually programmed (overlap will exist), do so here if you add a spell,
    /// setup Start() stuff in here
    /// </summary>
    protected virtual void Setup()
    {
        switch (spellName)
        {
            //deflects damage from enemy attacks, lasts 3 turns
            case "deflection":
                break;
            //deflects damage from 
            case "blazingWeapon":
                break;
            //deflects damage from 
            case "iceShard":
                break;
        }
    }

    /// <summary>
    /// Every single spell needs to be individually programmed (overlap will exist), do so here
    /// setup Update() stuff in here
    /// Runs when spell active
    /// </summary>
    void ActiveState()
    {
        switch (spellName)
        {
            case "deflection":
                parent.GetComponent<Entity>().deflection = 3;
                break;
            case "Ice Shard":

                break;
        }
    }

    /// <summary>
    /// Every single spell needs to be individually programmed (overlap will exist), do so here
    /// setup Update() stuff in here
    /// Runs when spell not active
    /// </summary>
    void UnActiveState()
    {
        switch (spellName)
        {
            case "deflection":
                parent.GetComponent<Entity>().deflection = 0;
                FindObjectOfType<ShaderManager>().Untint(parent);
                break;
            case "Ice Shard":

                break;
        }
    }

    /// <summary>
    /// Activates the uses of the spell
    /// </summary>
    public void Activate()
    {
        active = true;

        switch (spellName)
        {
            //visual and audio cues
            case "deflection":
                FindObjectOfType<ShaderManager>().TintGreenPulse(parent);
                FindObjectOfType<AudioManager>().PlaySound("magicShieldSound");
                break;
        }
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

            switch (spellName)
            {
                case "deflection":
                    FindObjectOfType<ShaderManager>().Untint(parent);
                    break;
            }
        }
    }
}
