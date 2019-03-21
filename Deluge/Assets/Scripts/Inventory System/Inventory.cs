using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    // FIELDS
    // Main Pack
    public Item[,] mainPack;
    public GameObject testItem;

    // Current Items
    public main_item currentMain;
    public enum main_item
    {
        spear,
        axe,
        bow
    }

    public Item currentHat;
    public Item currentTorso;
    public Item currentOffHand;

    // Runs all startup instantiation/connection code
    void Start()
    {
        mainPack = new Item[4, 5];
        AddItem(testItem.GetComponent<Item>());

        Debug.Log("Main Pack:" + mainPack);

        currentMain = main_item.spear;

    }

    /// <summary>
    /// Returns a boolean base on if the inventory was full
    /// If true is returned, the item was successfully added
    /// </summary>
    /// <param name="newItem">The item being added to the inventory</param>
    public bool AddItem(Item newItem)
    {
        // Cycle through the whole inventory pack looking for an open slot
        for (int i = 0; i < mainPack.GetLength(0); i++)
        {
            for (int j = 0; j < mainPack.GetLength(1); j++)
            {
                // If a slot is open, set the item in that slot and return true
                if (mainPack[j, i] != null)
                {
                    mainPack[j, i] = newItem;
                    return true;
                }
            }
        }

        // No spots open, return false
        return false;
    }

    /// <summary>
    /// Used for adding items to a specific slot - use in save loading
    /// </summary>
    /// <param name="newItem">The item being added to the inventory</param>
    /// <param name="xPos">X position of the item in the inventory</param>
    /// <param name="yPos">Y position of the item in the inventory</param>
    /// <returns></returns>
    public bool AddItem(Item newItem, int xPos, int yPos)
    {
        // Check if the slot is occupied & add item to that slot if it's not
        if (mainPack[xPos, yPos] != null)
        {
            mainPack[xPos, yPos] = newItem;
            return true;
        }

        // If the slot is already filled, return false;
        return false;
    }


}
