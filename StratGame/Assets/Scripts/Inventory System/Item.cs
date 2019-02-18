using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    // FIELDS

    // Identifiers
    public int itemID;
    public GameObject itemHolder;

    // Basic Stats
    public int bonusAtk = 0;
    public int bonusDef = 0;
    public int bonusHP = 0;
    public int bonusMaxHP = 0;

    // Special Stats
    public int bonusTime = 0;
    public float bonusVamp = 0.0f;
    public int healthPerTurn = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
