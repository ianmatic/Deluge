using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory_UI_Manager : MonoBehaviour
{
    // FIELDS
    // Pulling data from other places
    public Inventory playerInventory;

    // Pulling gameobject UI data
    public GameObject itemContainer;
    public GameObject[,] mainPack;

    // Pulling the pack bg images
    public Texture2D pack_frame_hat;
    public Texture2D pack_frame_torso;
    public Texture2D pack_frame_weapon;
    public Texture2D pack_frame_offhand;
    public Texture2D pack_frame_useable;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // TODO: Set up function to pull from player inventory any time the UI is toggled on
    void UpdateFromInventory()
    {
    }

    /// <summary>
    /// Places an item in a specific position
    /// </summary>
    void PlaceItem(GameObject item, int xPos, int yPos)
    {
        // Pull the item's general data
        Item itemData = item.GetComponent<Item>();

        // Pull the item's type & sprite
        switch (itemData.ItemType)
        {
            case Item.itemType.hat:
                mainPack[xPos, yPos].GetComponent<RawImage>().texture = pack_frame_hat;
                break;
            case Item.itemType.offhand:
                mainPack[xPos, yPos].GetComponent<RawImage>().texture = pack_frame_offhand;
                break;
            case Item.itemType.torso:
                mainPack[xPos, yPos].GetComponent<RawImage>().texture = pack_frame_torso;
                break;
            case Item.itemType.weapon:
                mainPack[xPos, yPos].GetComponent<RawImage>().texture = pack_frame_weapon;
                break;
            case Item.itemType.useable:
                mainPack[xPos, yPos].GetComponent<RawImage>().texture = pack_frame_useable;
                break;
        }

    }

    /// <summary>
    /// Updates the left side info panel with information from a given item
    /// </summary>
    void UpdateInfoPanel(GameObject item)
    {

    }
}
