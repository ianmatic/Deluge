using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    // FIELDS
    public enum itemType
    {
        weapon,
        hat,
        torso,
        offhand,
        useable
    }

    // Identifiers
    public int itemID;
    public Texture2D sprite;
    public itemType ItemType;

    // Basic Stats
    public int bonusAtk = 0;
    public int bonusDef = 0;
    public int bonusHP = 0;             //use with consumables
    public int bonusMaxHP = 0;          //use with equippables

    // Item level info
    public int experience = 0;
    public int experienceCap = 100;
    public int level = 0;               //start at lvl 0 with no bonuses, increase to lvl 3 w/ max bonus
    public int levelCap = 3;
    public int levelOneBonus = 3;
    public int levelTwoBonus = 3;
    public int levelThreeBonus = 3;

    // Special Stats
    public int bonusTime = 0;
    public float bonusVamp = 0.0f;
    public int healthPerTurn = 0;

    // Flavor
    public string itemName = "Default";
    public string itemFlavor = "Welcome to Flavor Text Town";

    // Questing
    public bool questItem;

    /// <summary>
    /// Add experience to a given item
    /// </summary>
    /// <param name="xpEarned"></param>
    public void AddXP(int xpEarned)
    {
        // Add experience
        experience += xpEarned;

        // If experience exceeds the experience cap and item is below max level, increase lvl & reset xp
        if (experienceCap < experience && level < levelCap)
        {
            experience -= experienceCap;
            level++;
        }
        // If experience exceeds xp cap and item is max level, just set to max xp and level
        else if (experienceCap < experience && level > levelCap)
        {
            level = levelCap;
            experience = experienceCap;
        }
    }
}
