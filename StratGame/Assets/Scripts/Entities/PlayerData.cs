using System.Collections;
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
    public string direction;
    bool moveInCombat = true;
    bool newInput = false;
    public string weaponSelected;
    public List<GameObject> actionTiles;


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
        GetComponent<Entity>().time = 1.5f;
        inCombat = false;
        actionTiles = new List<GameObject>();

        ui_manager = GameObject.Find("UI Canvas Manager").GetComponent<UI_Manager>();
        manager = GameObject.FindGameObjectWithTag("manager");

        weaponSelected = "axe";
    }

    /// <summary>
    /// Called every frame
    /// </summary>
    void Update()
    {
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
                    direction = "up";
                    newInput = true;
                }
                else if (Input.GetKey(KeyCode.A))
                {
                    direction = "left";
                    newInput = true;
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    direction = "right";
                    newInput = true;
                }
                else if (Input.GetKey(KeyCode.S))
                {
                    direction = "down";
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
                         FindActionTiles(gameObject, direction);

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
                    if (direction != null)
                    {
                        //move when new input given
                        if (moveInCombat && newInput)
                        {
                            GetComponent<Entity>().MoveDirection(direction);
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
        else
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
                GetComponent<Entity>().MoveDirection("up");
            }
            if (Input.GetKey(KeyCode.A) && counter % divisor == 0)
            {
                GetComponent<Entity>().MoveDirection("left");
            }
            if (Input.GetKey(KeyCode.D) && counter % divisor == 0)
            {
                GetComponent<Entity>().MoveDirection("right");
            }
            if (Input.GetKey(KeyCode.S) && counter % divisor == 0)
            {
                GetComponent<Entity>().MoveDirection("down");
            }

            //increment counter if a key is being held
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A)
                || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.S))
            {
                counter++;
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
}
