using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    // FIELDS
    public bool inCombat;
    public Inventory inventory;
    private UI_Manager ui_manager;

    //temporary
    string direction;

    // Inventory
    public int invWidth;
    public int invHeight;

    int counter = 0;

    /// <summary>
    /// Called on the first frame of existance
    /// </summary>
    void Start()
    {
        GetComponent<Entity>().time = 1.5f;
        inCombat = false;
        inventory = new Inventory();

        ui_manager = GameObject.Find("UI Canvas Manager").GetComponent<UI_Manager>();
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
        // Checking for main weapon swaps, can swap both in and out of combat
        if (Input.GetKeyDown(KeyCode.Q))
        {
            switch (inventory.currentMain)
            {
                case Inventory.main_item.bow:
                    inventory.currentMain = Inventory.main_item.axe;
                    break;
                case Inventory.main_item.axe:
                    inventory.currentMain = Inventory.main_item.spear;
                    break;
                case Inventory.main_item.spear:
                    inventory.currentMain = Inventory.main_item.bow;
                    break;

                default:
                    Debug.Log("Enum System Broken");
                    break;
            }

            ui_manager.cycleIconMain();
        }

        if (inCombat)
        {
            //player's turn
            if (GetComponent<Entity>().doingTurn)
            {
                //temporary hardcoded solution to test turns
                if (Input.GetKeyDown(KeyCode.W))
                {
                    direction = "up";
                }
                else if (Input.GetKeyDown(KeyCode.A))
                {
                    direction = "left";
                }
                else if (Input.GetKeyDown(KeyCode.D))
                {
                    direction = "right";
                }
                else if (Input.GetKeyDown(KeyCode.S))
                {
                    direction = "down";
                }

                //handle input/execute turn
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    //didn't pick a direction
                    if (direction != null)
                    {
                        //don't move
                        GetComponent<Entity>().MoveDirection(direction);
                    }
                    GetComponent<Timer>().remainingTime = 0;
                }
            }
            else
            {
                //reset movement direction each turn
                direction = null;
            }

        }
        else
        {
            //reset counter on first frame of key press
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A)
                || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.S))
            {
                counter = 0;
            }

            //able to hold key in free roam mode
            if (Input.GetKey(KeyCode.W) && counter % 10 == 0)
            {
                GetComponent<Entity>().MoveDirection("up");
            }
            if (Input.GetKey(KeyCode.A) && counter % 10 == 0)
            {
                GetComponent<Entity>().MoveDirection("left");
            }
            if (Input.GetKey(KeyCode.D) && counter % 10 == 0)
            {
                GetComponent<Entity>().MoveDirection("right");
            }
            if (Input.GetKey(KeyCode.S) && counter % 10 == 0)
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
