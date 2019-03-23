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
    [HideInInspector]
    public int health;
    [HideInInspector]
    public int maxHealth;
    [HideInInspector]
    public int attack;
    [HideInInspector]
    public int defense;
    [HideInInspector]
    public float vamp;
    [HideInInspector]
    public bool respawning = false;
    [HideInInspector]
    public bool inCombat;

    [HideInInspector]
    public FaceDirection direction = FaceDirection.none;
    [HideInInspector]
    public entityType type;

    // Inventory
    public int invWidth;
    public int invHeight;
    public GameObject[,] inventory;

    //Tile system
    [HideInInspector]
    public GameObject manager;

    //this is the tile that the entity snaps to
    [HideInInspector]
    public GameObject parentTile;

    //Turn system
    [HideInInspector]
    public bool doingTurn;
    [HideInInspector]
    public float maxTime;



    //smooth Movement
    [HideInInspector]
    public float smoothSpeed = 0.25f;
    [HideInInspector]
    public Vector3 velocity;

    // Start is called before the first frame update
    void Start()
    {
        //default health values
        health = 10;
        maxHealth = 20;
        attack = 4;

        manager = GameObject.FindGameObjectWithTag("manager");

        //temporary until proper parent tile is found
        Vector3 temporaryParent = transform.position;

        //move y to below entity
        float height = GetComponent<Renderer>().bounds.size.y;

        temporaryParent.y -= .75f;

        parentTile = manager.GetComponent<TileManager>().UpdateParentTile(temporaryParent, null);

        inventory = new GameObject[invWidth, invHeight];

        doingTurn = false;
        inCombat = false;
        //TODO: Populate inventory from save data or generation
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameData.GameplayPaused)
        {
            //handle death
            if (health <= 0)
            {
                KillEntity(gameObject);
            }
            else
            {
                //respawning should be instant, no smooth movement
                if (!respawning)
                {
                    UpdateMovementSmoothly();
                }
                else
                {
                    //only respawn for 1 frame
                    respawning = false;
                    transform.position = new Vector3(parentTile.transform.position.x,
                                     parentTile.transform.position.y + .75f, parentTile.transform.position.z);
                }
            }



        }
    }

    /// <summary>
    /// smooths the translation of the GO
    /// </summary>
    public void UpdateMovementSmoothly()
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

        //limit health
        if (health + dmgCalc > maxHealth)
        {
            health = maxHealth;
        }
        else
        {
            health += dmgCalc;
        }


        print(name + " attacked " + target.name);
        // TODO: Trigger Attack Animation

    }

    public void KillEntity(GameObject entity)
    {
        switch (entity.GetComponent<Entity>().type)
        {
            case entityType.player:
                entity.GetComponent<PlayerData>().Respawn();
                break;
            case entityType.enemy:
                //update combat order and parent tile
                entity.GetComponent<Entity>().doingTurn = false;
                entity.GetComponent<Entity>().parentTile.GetComponent<TileProperties>().isParent = false;
                entity.GetComponent<Entity>().parentTile = null;


                //remove from list
                manager.GetComponent<TurnManager>().enemies.Remove(entity);

                //finally, eliminate the enemy
                Destroy(entity);            
                break;
        }

    }
}
