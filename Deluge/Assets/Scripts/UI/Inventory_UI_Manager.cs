using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory_UI_Manager : MonoBehaviour
{
    // FIELDS
    // Pulling data from other places
    public Inventory playerInventory;
    public GameObject mainUI;
    Item[,] mainPack;

    // Pulling gameobject UI data
    public GameObject parentContainer;
    public GameObject itemContainer;

    // Pulling the pack bg images
    public Texture2D pack_frame_hat;
    public Texture2D pack_frame_torso;
    public Texture2D pack_frame_weapon;
    public Texture2D pack_frame_offhand;
    public Texture2D pack_frame_useable;

    // Display daya
    bool displaying = true;
    Vector3 displayVec;

    // Start is called before the first frame update
    void Start()
    {
        // Move the assets off-screen before the game loads
        ToggleAssets();
        mainUI.GetComponent<UI_Manager>().ToggleAssets();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleAssets();
        }
    }
    
    /// <summary>
    /// Puts the assets from this scene on screen and puts main UI assets off
    /// </summary>
    void ToggleAssets()
    {
        // Toggle the main UI's assets
        mainUI.GetComponent<UI_Manager>().ToggleAssets();

        // Toggle the assets for this scene
        int positionShift = 10000;
        if (!displaying)            
        {
            positionShift *= -1;
        }

        // Get and replace the transform of the parent with one 1000 to the left or right
        displayVec = parentContainer.transform.position;
        displayVec.x += positionShift;
        parentContainer.transform.position = displayVec;

        // Update the items currently held
        UpdateFromInventory();
    }

    // Function to pull from player inventory any time the UI is toggled on
    void UpdateFromInventory()
    {
        Debug.Log(playerInventory);
        Debug.Log(playerInventory.mainPack);

        mainPack = playerInventory.mainPack;

        // Cycle through every slot in the player's backpack
        for (int y = 0; y < mainPack.GetLength(1); y++) 
        {
            for (int x = 0; x < mainPack.GetLength(0); x++)
            {
                // Make sure the inventory slot has something in it
                if(playerInventory.mainPack[x, y] != null)
                {
                    PlaceItem(mainPack[x, y], x, y);
                }
            }
        }
    }

    /// <summary>
    /// Places an item in a specific position
    /// </summary>
    void PlaceItem(Item itemData, int xPos, int yPos)
    {
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
