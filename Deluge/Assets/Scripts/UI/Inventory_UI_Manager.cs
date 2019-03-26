using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory_UI_Manager : MonoBehaviour
{
    // FIELDS
    // Pulling data from other places           //TODO: Continue redoing inventory system starting
    public Inventory playerInventory;           //      from the PlaceItem function
    public GameObject mainUI;
    Item[,] mainPack;

    // Pulling gameobject UI data
    public GameObject parentContainer;

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

    // Indicator Info
    int[] currentItemPos = new int[] { 0, 0 };          //{ x, y }
    public GameObject itemIndicator;

    // Setting up item container overlays
    private const int WIDTH = 4;
    private const int HEIGHT = 5;
    GameObject[,] itemSlots = new GameObject[WIDTH, HEIGHT];
    public GameObject slotContainer;
    public GameObject defaultItemSlot;
    public GameObject defaultItemIcon;  // Get to item icons by finding the zeroth child of a given slot
    public List<GameObject> itemIcons;

    // Empty Item for mainPack
    public GameObject emptyItem;

    enum direction
    {
        up,
        down,
        left,
        right
    }

    // Display data
    bool displaying = true;
    Vector3 displayVec;

    // Start is called before the first frame update
    void Start()
    {
        mainUI.GetComponent<UI_Manager>().ToggleAssets();
        itemIcons = new List<GameObject>();

        #region old code (gross)
        /*  REDOING MAIN PACK DISPLAYING
        // Add all the items sprite and background holders as children of their containers
        // Slot Bg's
        for (int i = 0; i < 19; i++)
        {   
            GameObject newItemSlot = Instantiate(defaultItemSlot, itemContainer.transform);
            itemSlots.Add(newItemSlot);
            newItemSlot.transform.localScale = displayVec;

            // Add all the item icons and set them as children of the containers
            //GameObject newItemIcon = Instantiate(defaultItemIcon, newItemSlot.transform);
            //newItemIcon.transform.localScale = displayVec;
            itemIcons.Add(itemSlots[i].transform.GetChild(0).gameObject);
        }
        */
        #endregion

        // Instantiate all containers and add item icons as their children
        InstantiateContainers();

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

        // Update the item indicator and left panel based on which input the player gave
        if (displaying)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                MoveIndicator(direction.up);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                MoveIndicator(direction.down);
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                MoveIndicator(direction.left);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                MoveIndicator(direction.right);
            }
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

        // Update the displaying denoter
        displaying = !displaying;

        if (displaying)
        {
            // Update the items currently held
            UpdateFromInventory();
        }

        // Pause the main game loop
        //GameData.ToggleFullPaused();
    }

    /// <summary>
    /// Instantiates all the containers on startup
    /// </summary>
    void InstantiateContainers()
    {
        // Set the default container into the first slot
        itemSlots[0, 0] = defaultItemSlot;

        // Create an object to base all others on
        GameObject itemSlotNoIcon = defaultItemSlot;
        itemSlotNoIcon.transform.DetachChildren();

        // Populate all other slots with containers based on the above defined one
        for (int y = 0; y < HEIGHT; y++)
        {
            for (int x = 0; x < WIDTH; x++)
            {
                if (itemSlots[x, y] == null)
                {
                    itemSlots[x, y] = Instantiate(itemSlotNoIcon, slotContainer.transform);
                }
            }
        }
    }

    /// <summary>
    /// Moves the indicator based on a given direction passed in
    /// </summary>
    /// <param name="d"></param>
    void MoveIndicator(direction d)
    {
        // Update the position in code
        switch (d)
        {
            case direction.up:
                if (currentItemPos[1] == 0)
                {
                    currentItemPos[1] = HEIGHT - 1;
                }
                else
                {
                    currentItemPos[1]--;
                }
                break;

            case direction.down:
                if (currentItemPos[1] == HEIGHT - 1)
                {
                    currentItemPos[1] = 0;
                }
                else
                {
                    currentItemPos[1]++;
                }
                break;

            case direction.left:
                if (currentItemPos[0] == 0)
                {
                    currentItemPos[0] = WIDTH - 1;
                }
                else
                {
                    currentItemPos[0]--;
                }
                break;

            case direction.right:
                if (currentItemPos[0] == WIDTH - 1)
                {
                    currentItemPos[0] = 0;
                }
                else
                {
                    currentItemPos[0]++;
                }
                break;

            default:
                break;
        }

        // Move to the position of where the indicator should be
        itemIndicator.transform.position = itemSlots[currentItemPos[0], currentItemPos[1]].transform.position;

        // Update the left panel based on the new item
        UpdateInfoPanel(mainPack[currentItemPos[0], currentItemPos[1]]);
    }

    /// <summary>
    /// Pulls the current inventory informaiton from the player inventory item and adds to mainPack
    /// </summary>
    void UpdateFromInventory()
    {
        // Pull the current version of the player's pack
        mainPack = playerInventory.mainPack;

        // Cycle through every slot in the player's backpack and pull the items
        for (int y = 0; y < mainPack.GetLength(1); y++) 
        {
            for (int x = 0; x < mainPack.GetLength(0); x++)
            {
                // Make sure the inventory slot has something in it
                if(playerInventory.mainPack[x, y] != null)
                {
                    PlaceItem(mainPack[x, y], x, y);
                }
                // If there's no item, fill the slot with an empty one
                else
                {
                    mainPack[x, y] = emptyItem.GetComponent<Item>();
                    PlaceItem(emptyItem.GetComponent<Item>(), x, y);
                }
            }
        }
    }

    /// <summary>
    /// Places an item in a specific position
    /// </summary>
    void PlaceItem(Item itemData, int xPos, int yPos)
    {
        // Pull the item's type
        switch (itemData.ItemType)
        {
            case Item.itemType.hat:
                itemSlots[xPos, yPos].GetComponent<RawImage>().texture = pack_frame_hat;
                break;
            case Item.itemType.offhand:
                itemSlots[xPos, yPos].GetComponent<RawImage>().texture = pack_frame_offhand;
                break;
            case Item.itemType.torso:
                itemSlots[xPos, yPos].GetComponent<RawImage>().texture = pack_frame_torso;
                break;
            case Item.itemType.weapon:
                itemSlots[xPos, yPos].GetComponent<RawImage>().texture = pack_frame_weapon;
                break;
            case Item.itemType.empty:
            case Item.itemType.useable:
                itemSlots[xPos, yPos].GetComponent<RawImage>().texture = pack_frame_useable;
                break;
        }

        // Pull the item's sprite
        if (itemData.sprite != null)
        {
            if (itemSlots[xPos,yPos].transform.childCount != 0)
            {
                GameObject child = itemSlots[xPos, yPos].transform.GetChild(0).gameObject;
                child.GetComponent<RawImage>().texture = itemData.sprite;
            }
            else
            {
                // Instantiate the object and add the sprite to it
                GameObject child = Instantiate(defaultItemIcon, itemSlots[xPos, yPos].transform);
                child.GetComponent<RawImage>().texture = itemData.sprite;
                
                // Properly set the size
                child.transform.localScale = new Vector3(0.16f, 0.16f, 0.16f);
                child.GetComponent<RectTransform>().sizeDelta = new Vector2(128, 128);

                // Adjust the anchorpoints and position
                child.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
                child.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
                child.transform.localPosition = Vector3.zero;
                itemIcons.Add(child);
            }
        }
    }

    /// <summary>
    /// Goes through and zeros out all the item icon positions relative to their parents
    /// </summary>
    void ResetItemIconZeros()
    {
        foreach(GameObject icon in itemIcons)
        {
            icon.transform.localPosition = Vector3.zero;
        }
    }

    /// <summary>
    /// Calls UpdateInfoPanel with the given game object's item script
    /// </summary>
    /// <param name="itemGO"></param>
    void UpdateInfoPanelByGO(GameObject itemGO)
    {
        // Get the Item script so we don't have to keep calling GetComponent<whatever>()
        Item item = itemGO.GetComponent<Item>();
        UpdateInfoPanel(item);
    }

    /// <summary>
    /// Updates the left side info panel with information from a given item
    /// </summary>
    void UpdateInfoPanel(Item item)
    {
        Debug.Log(item);
        if (item != null)
        {
            // Update icon
            if (item.sprite != null)
            {
                panelItemIcon.GetComponent<RawImage>().texture = item.sprite;
            }

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
        // For when the item passed in is null, default everything out
        else
        {
            // TODO: Default out all values
        }

    }




}
