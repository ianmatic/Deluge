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

    // Setting up item container overlays
    public GameObject itemContainer;
    public GameObject defaultItemSlot;
    List<GameObject> itemSlots;

    public GameObject itemIconContainer;
    public GameObject defaultItemIcon;
    List<GameObject> itemIcons;

    // Setting up left container
    public GameObject panelItemIcon;
    public GameObject panelNameText;
    public List<GameObject> panelStatText;
    public List<GameObject> panelItemLevelMods;
    public List<GameObject> panelItemLevelIndicators;
    public GameObject panelItemExperienceBar;
    public GameObject panelItemSpellText;
    public GameObject panelItemSpellIcon;
    public GameObject panelFlavorText;

    // Pulling the bg images
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
        mainUI.GetComponent<UI_Manager>().ToggleAssets();

        itemSlots = new List<GameObject>();
        itemSlots.Add(defaultItemSlot);

        itemIcons = new List<GameObject>();
        itemIcons.Add(defaultItemIcon);

        displayVec = Vector3.one;

        // Add all the items as children of the itemContainer
        for (int i = 0; i < 19; i++)
        {   
            GameObject newItemSlot = Instantiate(defaultItemSlot, itemContainer.transform);
            itemSlots.Add(newItemSlot);
            newItemSlot.transform.localScale = displayVec;
        }

        for (int i = 0; i < 19; i++)
        {
            GameObject newItemIcon = Instantiate(defaultItemIcon, itemIconContainer.transform);
            itemIcons.Add(newItemIcon);
            newItemIcon.transform.localScale = displayVec;
        }

        // Move the bits off the screen
        ToggleAssets();
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

        // Update the displaying denoter
        displaying = !displaying;
    }

    // Function to pull from player inventory any time the UI is toggled on
    void UpdateFromInventory()
    {
        // Pull the current version of the player's pack
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
        // Find the slot to put the item into
        int slotNumber = (yPos * xPos) + xPos;

        // Pull the item's type
        switch (itemData.ItemType)
        {
            case Item.itemType.hat:
                itemSlots[slotNumber].GetComponent<RawImage>().texture = pack_frame_hat;
                break;
            case Item.itemType.offhand:
                itemSlots[slotNumber].GetComponent<RawImage>().texture = pack_frame_offhand;
                break;
            case Item.itemType.torso:
                itemSlots[slotNumber].GetComponent<RawImage>().texture = pack_frame_torso;
                break;
            case Item.itemType.weapon:
                itemSlots[slotNumber].GetComponent<RawImage>().texture = pack_frame_weapon;
                break;
            case Item.itemType.useable:
                itemSlots[slotNumber].GetComponent<RawImage>().texture = pack_frame_useable;
                break;
        }

        // Pull the item's sprite
        itemIcons[slotNumber].GetComponent<RawImage>().texture = itemData.sprite;
    }

    /// <summary>
    /// Updates the left side info panel with information from a given item
    /// </summary>
    void UpdateInfoPanel(GameObject itemGO)
    {
        // Get the Item script so we don't have to keep calling GetComponent<whatever>()
        Item item = itemGO.GetComponent<Item>();

        // Update icon
        panelItemIcon.GetComponent<RawImage>().texture = item.sprite;

        // Update name text & flavor text
        panelNameText.GetComponent<Text>().text = item.itemName;
        panelFlavorText.GetComponent<Text>().text = item.itemFlavor;

        // Update stats text
        panelStatText[0].GetComponent<Text>().text = "+" + item.bonusMaxHP.ToString();
        panelStatText[1].GetComponent<Text>().text = "+" + item.bonusAtk.ToString();
        panelStatText[2].GetComponent<Text>().text = "+" + item.bonusDef.ToString();
        panelStatText[3].GetComponent<Text>().text = "+" + item.bonusVamp.ToString() + "%";

        // Update item level xp info


        // TODO: Create spells to update spell info from
    }
}
