using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FaceDirection
{
    left,
    right,
    forward,
    backward,
    none
}

public enum entityType
{
    player,
    enemy,
    npc,
    chest
}

public class Entity : MonoBehaviour
{
    // FIELDS

    // Basic Stats
    public int health;
    public int maxHealth;
    public int attack;
    public int defense;
    public float vamp;

    public FaceDirection direction = FaceDirection.none;
    public entityType type;

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
    public float maxTime;


    //smooth Movement
    public float smoothSpeed = 0.25f;

    public Vector3 velocity;

    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.FindGameObjectWithTag("manager");

        //temporary until proper parent tile is found
        Vector3 temporaryParent = transform.position;

        //move y to below entity
        float height = GetComponent<Renderer>().bounds.size.y;

        temporaryParent.y -= .75f;

        parentTile = manager.GetComponent<TileManager>().UpdateParentTile(temporaryParent, null);

        inventory = new GameObject[invWidth, invHeight];

        doingTurn = false;
        //TODO: Populate inventory from save data or generation
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameData.GameplayPaused)
        {
            //Smooth movement of GO
            Vector3 desiredPos = new Vector3(parentTile.transform.position.x,
                                     parentTile.transform.position.y + .75f, parentTile.transform.position.z);

            velocity = Vector3.Lerp(transform.position, desiredPos, smoothSpeed) - transform.position;


            //clamp speed
            velocity = Vector3.ClampMagnitude(velocity, .15f);


            //then update the position of GO
            transform.position += velocity;

            //snap to the tile if close enough to it
            Vector3 snapPosition = transform.position;
            if (Mathf.Abs(transform.position.x - parentTile.transform.position.x) < .05f)
            {
                snapPosition.x = parentTile.transform.position.x;
            }
            if (Mathf.Abs(transform.position.z - parentTile.transform.position.z) < .05f)
            {
                snapPosition.z = parentTile.transform.position.z;
            }
            transform.position = snapPosition;
        }
    }

    /// <summary>
    /// This function only handles 2d movement, vertical movement is handled by UpdateParentTile
    /// </summary>
    /// <param name="direction"></param>
    public void MoveDirection(FaceDirection direction)
    {
        //where the object wants to go
        Vector3 targetPosition = parentTile.transform.position;
        //handle 2d movement
        if (direction == FaceDirection.forward)
        { 
            targetPosition.z += 1;
        }
        else if (direction == FaceDirection.right)
        {
            targetPosition.x += 1;
        }
        else if (direction == FaceDirection.backward)
        {
            targetPosition.z -= 1;
        }
        else if (direction == FaceDirection.left)
        {
            targetPosition.x -= 1;
        }

        //update the parent tile
        if (Vector3.Distance(new Vector3(transform.position.x, transform.position.y - .75f, transform.position.z), parentTile.transform.position) < 1.75)
        {
            parentTile = manager.GetComponent<TileManager>().UpdateParentTile(targetPosition, parentTile);
        }
    }

    public void SetTileAsParentTile(GameObject tile)
    {
        parentTile = manager.GetComponent<TileManager>().UpdateParentTile(tile.transform.position, parentTile);
    }

    /// <summary>
    /// Handles attacking GAMEOBJECTS
    /// </summary>
    /// <param name="GameObject"></param>
    public void Attack(GameObject target)
    {
        // Resolve Damage
        int dmgCalc = attack - target.GetComponent<Entity>().defense;
        target.GetComponent<Entity>().health -= dmgCalc;

        // Resolve Life Vamp

        //health += (int)(dmgCalc * vamp);

        health += dmgCalc;

        print(name + " attacked " + target.name);
        // TODO: Tricker Attack Animation

    }
}
