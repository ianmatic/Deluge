using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    // FIELDS
    // Main Pack
    Item[,] mainPack;

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

    void Start()
    {
        mainPack = new Item[ 5, 8 ];
        currentMain = main_item.spear;
    }
}
