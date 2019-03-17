﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    // FIELDS
    public bool inCombat;
    public Inventory inventory;
    private UI_Manager ui_manager;
    private GameObject manager;

    //which way the player is facing
    bool moveInCombat = true;
    bool newInput = false;

    public string weaponSelected;
    public List<GameObject> actionTiles;
    public GameObject interactTile;
    public List<GameObject> interactiveObjects;
    public bool interacting = false;


    // Inventory
    public int invWidth;
    public int invHeight;

    public int counter = 0;

    public int divisor = 10;

    /// <summary>
    /// Called on the first frame of existance
    /// </summary>
    void Start()
    {
        GetComponent<Entity>().maxTime = 1.5f;
        inCombat = false;
        actionTiles = new List<GameObject>();
        interactiveObjects = new List<GameObject>();

        ui_manager = GameObject.Find("main_ui").GetComponent<UI_Manager>();
        manager = GameObject.FindGameObjectWithTag("manager");

        weaponSelected = "axe";

        GetComponent<Entity>().health = 20;
        GetComponent<Entity>().maxHealth = 34;
        GetComponent<Entity>().type = entityType.player;
    }

    /// <summary>
    /// Called every frame
    /// </summary>
    void Update()
    {
        interactiveObjects = FindInteractiveObjects();

        CheckDebugInputs();

        //Handle keyboard input and end turn
        CheckPlayerInputs(inCombat);
    }

    /// <summary>
    /// Checking the player's main inputs
    /// </summary>
    void CheckPlayerInputs(bool inCombat)
    {

        if (inCombat)
        {
            //player's turn
            if (GetComponent<Entity>().doingTurn)
            {
                //Weapon toggling during turn
                SelectWeapon();

                //toggle moving and attacking
                if (Input.GetKeyDown(KeyCode.LeftShift))
                {
                    moveInCombat = !moveInCombat;
                }

                //select direction via WASD
                if (Input.GetKey(KeyCode.W))
                {
                    GetComponent<Entity>().direction = FaceDirection.forward;
                    newInput = true;
                }
                else if (Input.GetKey(KeyCode.A))
                {
                    GetComponent<Entity>().direction = FaceDirection.left;
                    newInput = true;
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    GetComponent<Entity>().direction = FaceDirection.right;
                    newInput = true;
                }
                else if (Input.GetKey(KeyCode.S))
                {
                    GetComponent<Entity>().direction = FaceDirection.backward;
                    newInput = true;
                }
                else
                {
                    newInput = false;
                }

                if (!moveInCombat)
                {
                    //untint all old tiles
                    foreach (GameObject tile in actionTiles)
                    {
                        manager.GetComponent<ShaderManager>().Untint(tile);
                    }

                    //find action tiles
                    actionTiles = manager.GetComponent<TileManager>().
                         FindActionTiles(gameObject, GetComponent<Entity>().direction);

                    //tint new tiles
                    foreach (GameObject tile in actionTiles)
                    {
                        manager.GetComponent<ShaderManager>().TintBlue(tile);
                    }
                }

                //handle input/execute turn
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    //picked a direction
                    if (GetComponent<Entity>().direction != FaceDirection.none)
                    {
                        //move when new input given
                        if (moveInCombat && newInput)
                        {
                            GetComponent<Entity>().MoveDirection(GetComponent<Entity>().direction);
                        }
                        //attack an enemy if possible
                        else
                        {
                            manager.GetComponent<TurnManager>().AttackNearbyEnemies();
                        }

                    }
                    GetComponent<Timer>().remainingTime = 0;
                }
            }
            else
            {

            }

        }
        else if (!interacting)
        {
            SelectWeapon();

            //moving diagonally
            if (Mathf.Abs(GetComponent<Entity>().velocity.x) > 0 && Mathf.Abs(GetComponent<Entity>().velocity.z) > 0)
            {
                GetComponent<Entity>().smoothSpeed = .175f;
                divisor = 14;
            }
            else
            {
                GetComponent<Entity>().smoothSpeed = .25f;
                divisor = 10;
            }

            //reset counter on first frame of key press
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A)
                || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.S))
            {
                counter = 0;
            }

            //able to hold key in free roam mode
            if (Input.GetKey(KeyCode.W) && counter % divisor == 0)
            {
                GetComponent<Entity>().direction = FaceDirection.forward;
                GetComponent<Entity>().MoveDirection(GetComponent<Entity>().direction);
            }
            if (Input.GetKey(KeyCode.A) && counter % divisor == 0)
            {
                GetComponent<Entity>().direction = FaceDirection.left;
                GetComponent<Entity>().MoveDirection(GetComponent<Entity>().direction);
            }
            if (Input.GetKey(KeyCode.D) && counter % divisor == 0)
            {
                GetComponent<Entity>().direction = FaceDirection.right;
                GetComponent<Entity>().MoveDirection(GetComponent<Entity>().direction);
            }
            if (Input.GetKey(KeyCode.S) && counter % divisor == 0)
            {
                GetComponent<Entity>().direction = FaceDirection.backward;
                GetComponent<Entity>().MoveDirection(GetComponent<Entity>().direction);
            }

            #region temporarilyHighlightInteractTile
            if (interactTile != null)
            {
                manager.GetComponent<ShaderManager>().Untint(interactTile);
            }

            //update interact tile after direction is set
            interactTile = manager.GetComponent<TileManager>().FindInteractTile(gameObject, GetComponent<Entity>().direction);

            if (interactTile != null)
            {
                manager.GetComponent<ShaderManager>().TintGreen(interactTile);
            }
            #endregion

            //increment counter if a key is being held
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A)
                || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.S))
            {
                counter++;
            }

            //environment interaction
            if (Input.GetKeyDown(KeyCode.E))
            {
                foreach (GameObject interactiveObject in interactiveObjects)
                {
                    //the player found an object to interact with
                    if (interactTile == interactiveObject.GetComponent<Entity>().parentTile)
                    {
                        entityType temp = interactiveObject.GetComponent<Entity>().type;

                        //check different kinds of objects
                        switch (temp)
                        {
                            case entityType.npc:
                                interactiveObject.GetComponent<npcData>().OnPlayerPrompt();
                                interacting = true;
                                break;
                        }
                    }
                }
            }
        }
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
                    break;
                case Inventory.main_item.axe:
                    inventory.currentMain = Inventory.main_item.spear;
                    weaponSelected = "spear";
                    break;
                case Inventory.main_item.spear:
                    inventory.currentMain = Inventory.main_item.bow;
                    weaponSelected = "bow";
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

        return objects;
    }
}
