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
        attack //fireball, ice vortex, etc., probably rename this when i think of better name
    }

    //set in inspector
    public SpellType type;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
