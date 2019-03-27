using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : MonoBehaviour
{
    [HideInInspector]
    public enum SpellType
    {
        enhancement, //sword does more vamp damage, bigger range etc.
        effect,  //healing, more resistance, etc.
        targeted //fireball, ice vortex, etc.
    }

    //set in inspector
    public SpellType type;

    //set in inspector
    public string spellName;

    //set in inspector, how many turns this spell lasts when cast
    public int length;

    // Start is called before the first frame update
    void Start()
    {
        switch (type)
        {
            //add the appropriate capability to the player
            case SpellType.enhancement:
                GameObject.FindGameObjectWithTag("Player").AddComponent<Enhancement>();
                GameObject.FindGameObjectWithTag("Player").GetComponent<Enhancement>().enhancementName = spellName;
                GameObject.FindGameObjectWithTag("Player").GetComponent<Enhancement>().length = length;
                break;
            case SpellType.effect:
                GameObject.FindGameObjectWithTag("Player").AddComponent<Effect>();
                GameObject.FindGameObjectWithTag("Player").GetComponent<Effect>().effectName = spellName;
                GameObject.FindGameObjectWithTag("Player").GetComponent<Effect>().length = length;
                break;
            case SpellType.targeted:
                GameObject.FindGameObjectWithTag("Player").AddComponent<Targeted>();
                GameObject.FindGameObjectWithTag("Player").GetComponent<Targeted>().targetedName = spellName;
                GameObject.FindGameObjectWithTag("Player").GetComponent<Targeted>().length = length;
                break;
        }

    }

    //remove capability from player
    void OnDestroy()
    {
        switch (type)
        {
            //remove the appropriate capability from the player
            case SpellType.enhancement:
                Destroy(GameObject.FindGameObjectWithTag("Player").AddComponent<Enhancement>());
                break;
            case SpellType.effect:
                Destroy(GameObject.FindGameObjectWithTag("Player").AddComponent<Effect>());
                break;
            case SpellType.targeted:
                Destroy(GameObject.FindGameObjectWithTag("Player").AddComponent<Targeted>());
                break;


        }
    }
}
