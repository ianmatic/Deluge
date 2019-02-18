using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    // FIELDS

    // Basic Stats
    public int health;
    public int maxHealth;
    public int attack;
    public int defense;
    public float vamp;

    // Inventory
    public int invWidth;
    public int invHeight;
    public GameObject[,] inventory;

    //Tile system
    public GameObject manager;

    //this is the tile that the entity snaps to
    public GameObject parentTile;

    //Turn system
    public bool doingTurn;
    public float time;


    //smooth Movement
    private float smoothSpeed = 0.25f;

    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.FindGameObjectWithTag("manager");

        //temporary until proper parent tile is found
        Vector3 temporaryParent = transform.position;
        temporaryParent.y -= 1;

        parentTile = manager.GetComponent<TileManager>().UpdateParentTile(temporaryParent, null);

        inventory = new GameObject[invWidth, invHeight];

        doingTurn = false;
        //TODO: Populate inventory from save data or generation
    }

    // Update is called once per frame
    void Update()
    {
        //Smooth movement of GO
        Vector3 desiredPos = new Vector3(parentTile.transform.position.x,
                                 parentTile.transform.position.y + 1, parentTile.transform.position.z);


        //then update the position of GO
        transform.position = Vector3.Lerp(transform.position, desiredPos, smoothSpeed);

        //TODO: Apply attack function when neccesary


    }

    /// <summary>
    /// This function only handles 2d movement, vertical movement is handled by UpdateParentTile
    /// </summary>
    /// <param name="direction"></param>
    public void MoveDirection(string direction)
    {
        //where the object wants to go
        Vector3 targetPosition = parentTile.transform.position;
        //handle 2d movement
        if (direction == "up")
        {
            targetPosition.z += 1;
        }
        else if (direction == "right")
        {
            targetPosition.x += 1;
        }
        else if (direction == "down")
        {
            targetPosition.z -= 1;
        }
        else if (direction == "left")
        {
            targetPosition.x -= 1;
        }

        //update the parent tile
        parentTile = manager.GetComponent<TileManager>().UpdateParentTile(targetPosition, parentTile);
    }
}
