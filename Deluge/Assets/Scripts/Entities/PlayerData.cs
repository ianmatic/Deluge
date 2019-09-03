using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    // FIELDS
    public Inventory inventory;
    private UI_Manager ui_manager;
    private GameObject manager;

    //update player directionality
    bool newInput = false;

    [HideInInspector]
    public string weaponSelected;
    [HideInInspector]
    public List<GameObject> actionTiles;
    [HideInInspector]
    public GameObject interactTile;
    [HideInInspector]
    public List<GameObject> interactiveObjects;
    [HideInInspector]
    AudioManager audioManager;


    // Inventory
    public int invWidth;
    public int invHeight;

    [HideInInspector]
    public double counter = 0.0f;

    [HideInInspector]
    public double divisor;

    /// <summary>
    /// Called on the first frame of existance
    /// </summary>
    void Start()
    {
        GetComponent<Entity>().maxTime = 1.5f;
        actionTiles = new List<GameObject>();
        interactiveObjects = new List<GameObject>();

        ui_manager = GameObject.Find("main_ui").GetComponent<UI_Manager>();
        manager = GameObject.FindGameObjectWithTag("manager");

        weaponSelected = "axe";

        GetComponent<Entity>().health = 18;
        GetComponent<Entity>().maxHealth = 34;
        GetComponent<Entity>().type = entityType.player;
        GetComponent<Entity>().attack = 5;
        GetComponent<Entity>().vamp = 2.0f;

        audioManager = FindObjectOfType<AudioManager>();
    }

    /// <summary>
    /// Called every frame
    /// </summary>
    void Update()
    {
        interactiveObjects = FindInteractiveObjects();

        CheckDebugInputs();

        //Handle keyboard input and end turn
        CheckPlayerInputs(GetComponent<Entity>().inCombat);

        //toggle pausing
        if (Input.GetKeyDown(KeyCode.P))
        {
            GameData.ToggleFullPaused();
        }
    }

    /// <summary>
    /// Checking the player's main inputs
    /// </summary>
    void CheckPlayerInputs(bool inCombat)
    {

        //press tab to speedup
        if (Input.GetKey(KeyCode.Tab))
        {
            Time.timeScale = 10.0f;
        }
        else
        {
            Time.timeScale = 1.0f;
        }

        //can't move, fight, or interact when paused
        if (!GameData.FullPaused)
        {
            //untint all old targeting
            foreach (GameObject tile in actionTiles)
            {
                manager.GetComponent<ShaderManager>().Untint(tile);
            }

            if (inCombat)
            {
                //while the current song isn't set, transition
                audioManager.TransitionToSong("combatTheme");

                //player's turn
                if (GetComponent<Entity>().doingTurn)
                {
                    //Weapon toggling during turn
                    SelectWeapon();

                    //select direction via Arrow Keys
                    if (Input.GetKey(KeyCode.DownArrow))
                    {
                        GetComponent<Entity>().direction = FaceDirection.forward;
                        newInput = true;
                    }
                    else if (Input.GetKey(KeyCode.LeftArrow))
                    {
                        GetComponent<Entity>().direction = FaceDirection.left;
                        newInput = true;
                    }
                    else if (Input.GetKey(KeyCode.RightArrow))
                    {
                        GetComponent<Entity>().direction = FaceDirection.right;
                        newInput = true;
                    }
                    else if (Input.GetKey(KeyCode.UpArrow))
                    {
                        GetComponent<Entity>().direction = FaceDirection.backward;
                        newInput = true;
                    }
                    else
                    {
                        newInput = false;
                    }

                    //cast spell
                    if (Input.GetKeyDown(KeyCode.F))
                    {

                        //has a spell equiped
                        if (GetComponent<Inventory>().currentOffHand != null && 
                            GetComponent<Inventory>().currentOffHand.GetComponent<Spell>() != null)
                        {
                            Spell spell = GetComponent<Inventory>().currentOffHand.GetComponent<Spell>();

                            //activate the spell if not already active 
                            if (!spell.active)
                            {
                                spell.Activate();
                            }
                        } 
                    }

                    //find action tiles
                    actionTiles = manager.GetComponent<TileManager>().
                         FindActionTiles(gameObject, GetComponent<Entity>().direction);

                    //tint new tiles
                    foreach (GameObject tile in actionTiles)
                    {
                        manager.GetComponent<ShaderManager>().TintBlue(tile);
                    }

                    //movement
                    if (Input.GetKeyDown(KeyCode.W))
                    {
                        if (GetComponent<Entity>().MoveDirection(FaceDirection.backward))
                        {
                            //successful movement
                            GetComponent<Timer>().remainingTime = 0;
                        }

                    }
                    if (Input.GetKeyDown(KeyCode.A))
                    {
                        if (GetComponent<Entity>().MoveDirection(FaceDirection.left))
                        {
                            //successful movement
                            GetComponent<Timer>().remainingTime = 0;
                        }
                    }
                    if (Input.GetKeyDown(KeyCode.D))
                    {
                        if (GetComponent<Entity>().MoveDirection(FaceDirection.right))
                        {
                            //successful movement
                            GetComponent<Timer>().remainingTime = 0;
                        }

                    }
                    if (Input.GetKeyDown(KeyCode.S))
                    {
                        if (GetComponent<Entity>().MoveDirection(FaceDirection.forward))
                        {
                            //successful movement
                            GetComponent<Timer>().remainingTime = 0;
                        }
                    }

                    //attack
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        manager.GetComponent<TurnManager>().AttackNearbyEnemies();
                        GetComponent<Timer>().remainingTime = 0;
                    }
                }
            }
            else
            {
                //while the current song isn't set, transition
                audioManager.TransitionToSong("exploreTheme");

                //no movement when in dialogue
                if (!GameData.GameplayPaused)
                {
                    SelectWeapon();

                    //moving diagonally
                    if (Mathf.Abs(GetComponent<Entity>().velocity.x) > 0 && Mathf.Abs(GetComponent<Entity>().velocity.z) > 0)
                    {
                        GetComponent<Entity>().smoothSpeed = 7.175f;
                        divisor = 2.85f;
                    }
                    else
                    {
                        GetComponent<Entity>().smoothSpeed = 10.25f;
                        divisor = 4.0f;
                    }

                    bool moved = false;
                    //able to hold key in free roam mode, paced movement unless initial key press, then immediate movement
                    if ((Input.GetKey(KeyCode.S) && counter > (1.0f / divisor)) || (Input.GetKeyDown(KeyCode.S) && counter == 0.0f))
                    {
                        GetComponent<Entity>().direction = FaceDirection.forward;
                        GetComponent<Entity>().MoveDirection(GetComponent<Entity>().direction);
                        moved = true;
                    }
                    if ((Input.GetKey(KeyCode.A) && counter > (1.0f / divisor)) || (Input.GetKeyDown(KeyCode.A) && counter == 0.0f))
                    {
                        GetComponent<Entity>().direction = FaceDirection.left;
                        GetComponent<Entity>().MoveDirection(GetComponent<Entity>().direction);
                        moved = true;
                    }
                    if ((Input.GetKey(KeyCode.D) && counter > (1.0f / divisor)) || (Input.GetKeyDown(KeyCode.D) && counter == 0.0f))
                    {
                        GetComponent<Entity>().direction = FaceDirection.right;
                        GetComponent<Entity>().MoveDirection(GetComponent<Entity>().direction);
                        moved = true;
                    }
                    if ((Input.GetKey(KeyCode.W) && counter > (1.0f / divisor)) || (Input.GetKeyDown(KeyCode.W) && counter == 0.0f))
                    {
                        GetComponent<Entity>().direction = FaceDirection.backward;
                        GetComponent<Entity>().MoveDirection(GetComponent<Entity>().direction);
                        moved = true;
                    }

                    if (moved)
                    {
                        counter = 0.0f;
                    }

                    #region temporarilyHighlightInteractTile
                    //if (interactTile != null)
                    //{
                    //    manager.GetComponent<ShaderManager>().Untint(interactTile);
                    //}

                    //update interact tile after direction is set
                    interactTile = manager.GetComponent<TileManager>().FindInteractTile(gameObject, GetComponent<Entity>().direction);

                    //if (interactTile != null)
                    //{
                    //    manager.GetComponent<ShaderManager>().TintGreen(interactTile);
                    //}
                    #endregion
                    //increment timer if key being held
                    if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A)
                        || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.S))
                    {
                        counter += Time.deltaTime;
                    } else
                    {
                        counter = 0.0f;
                    }

                }


                //environment interaction
                if (Input.GetKeyDown(KeyCode.E))
                {
                    foreach (GameObject interactiveObject in interactiveObjects)
                    {
                        //the player found an object to interact with
                        if (interactTile == interactiveObject.GetComponent<Entity>().parentTile)
                        {
                            //check different kinds of objects
                            switch (interactiveObject.GetComponent<Entity>().type)
                            {
                                case entityType.npc:
                                    //begin dialogue
                                    if (!GameData.GameplayPaused)
                                    {
                                        EventManager.DialogueBegin();
                                    }
                                    //end dialogue
                                    else
                                    {
                                        EventManager.DialogueExit();
                                    }
                                    break;
                                case entityType.chest:
                                    EventManager.ChestOpen();
                                    break;
                            }


                        }
                    }
                }
                //continue through dialogue
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if (GameData.GameplayPaused)
                    {
                        EventManager.DialogueContinue();
                    }
                }
            }
        }

        //menu navigation goes here
    }

    /// <summary>
    /// Helper function that checks which weapon is selected in inventory
    /// </summary>
    void SelectWeapon()
    {
        // Checking for main weapon swaps, can swap both in and out of combat
        if (Input.GetKeyDown(KeyCode.Q))
        {
            switch (inventory.currentMain)
            {
                case Inventory.main_item.bow:
                    inventory.currentMain = Inventory.main_item.axe;
                    weaponSelected = "axe";
                    audioManager.PlaySound("axeEquipSound");
                    break;
                case Inventory.main_item.axe:
                    inventory.currentMain = Inventory.main_item.spear;
                    weaponSelected = "spear";
                    audioManager.PlaySound("spearEquipSound");
                    break;
                case Inventory.main_item.spear:
                    inventory.currentMain = Inventory.main_item.bow;
                    weaponSelected = "bow";
                    audioManager.PlaySound("bowEquipSound");
                    break;

                default:
                    Debug.Log("Enum System Broken");
                    break;
            }

            ui_manager.CycleIconMain();
        }
    }

    /// <summary>
    /// Checking for debug inputs
    /// </summary>
    void CheckDebugInputs()
    {
        // Checking for bugtesting inputs
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            GetComponent<Entity>().health++;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            GetComponent<Entity>().health--;
        }
    }

    /// <summary>
    /// Finds all npcs, chests, items, etc that the player can interact with outside of combat
    /// </summary>
    /// <returns></returns>
    List<GameObject> FindInteractiveObjects()
    {
        List<GameObject> objects = new List<GameObject>();

        //add all npcs
        objects.AddRange(GameObject.FindGameObjectsWithTag("npc"));

        //add all chests
        objects.AddRange(GameObject.FindGameObjectsWithTag("chest"));

        return objects;
    }

    /// <summary>
    /// Finds respawn point and puts player at it
    /// </summary>
    public void Respawn()
    {
        //move the player
        GameObject respawn = GameObject.FindGameObjectWithTag("spawn");
        GetComponent<Entity>().SetTileAsParentTile(respawn);

        //reset health
        GetComponent<Entity>().health = 14;

        //indicate to Entity that the player is respawning
        GetComponent<Entity>().respawning = true;
    }
}
