using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FaceDirection
{
    left,
    right,
    forward,
    backward,
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
    public int outGoingDamage = 0;

    [HideInInspector]
    public FaceDirection direction = FaceDirection.forward;
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


    //Spell variables
    [HideInInspector]
    public int deflection = 0;

    // Start is called before the first frame update
    void Start()
    {
        //default values
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

        //set intial direction
        transform.rotation = SetOrientation(direction);

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
            //also update animations if not paused
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
                    UpdateRotationSmoothly();
                }
                else
                {
                    //only respawn for 1 frame
                    respawning = false;
                    transform.position = new Vector3(parentTile.transform.position.x,
                                     parentTile.transform.position.y + .75f, parentTile.transform.position.z);
                    transform.rotation = SetOrientation(direction);
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
    /// smooths the rotation of the go
    /// </summary>
    public void UpdateRotationSmoothly()
    {
        Quaternion currentOrientation = gameObject.transform.rotation;

        //default value, should be update in direction
        Quaternion targetOrientation = Quaternion.Euler(0, 0, 0);

        //find what orientation should be used
        switch (direction)
        {
            case FaceDirection.forward:
                targetOrientation = Quaternion.Euler(0, 0, 0);
                break;
            case FaceDirection.backward:
                targetOrientation = Quaternion.Euler(0, 180, 0);
                break;
            case FaceDirection.right:
                targetOrientation = Quaternion.Euler(0, 270, 0);
                break;
            case FaceDirection.left:
                targetOrientation = Quaternion.Euler(0, 90, 0);
                break;
        }

        //update orientation
        transform.rotation = Quaternion.Lerp(currentOrientation, targetOrientation, .1f);
    }

    /// <summary>
    /// This function only handles 2d movement, vertical movement is handled by UpdateParentTile
    /// returns true if moving to new tile, false otherwise
    /// </summary>
    /// <param name="direction"></param>
    public bool MoveDirection(FaceDirection direction)
    {
        GameObject oldParent = parentTile;

        //where the object wants to go
        Vector3 targetPosition = parentTile.transform.position;
        //handle 2d movement
        if (direction == FaceDirection.forward)
        {
            targetPosition.z -= 1;
        }
        else if (direction == FaceDirection.right)
        {
            targetPosition.x += 1;
        }
        else if (direction == FaceDirection.backward)
        {
            targetPosition.z += 1;
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

        //if they are the same tile, movement failed
        if (parentTile == oldParent)
        {
            return false;
        }
        else
        {
            //successfully moved to new tile
            return true;
        }
    }

    /// <summary>
    /// Sets the selected tile as the parent tile :O
    /// </summary>
    /// <param name="tile"></param>
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
        outGoingDamage = attack - target.GetComponent<Entity>().defense;

        target.GetComponent<Entity>().health -= outGoingDamage;

        health -= target.GetComponent<Entity>().deflection;

        print(gameObject.name + " lost " + target.GetComponent<Entity>().deflection + " health due to deflection");



        //GO being attacked
        switch (target.GetComponent<Entity>().type)
        {
            case entityType.enemy:
                if (target.GetComponent<Entity>().health > 0)
                {
                    FindObjectOfType<AudioManager>().PlaySound("enemyHurtSound");
                }
                else
                {
                    FindObjectOfType<AudioManager>().PlaySound("enemyDeathSound");
                }

                break;
            case entityType.player:
                if (target.GetComponent<Entity>().health > 0)
                {
                    FindObjectOfType<AudioManager>().PlaySound("playerHurtSound");
                }
                else
                {
                    FindObjectOfType<AudioManager>().PlaySound("playerDeathSound");
                }
                break;
        }

        //limit health
        if (health + outGoingDamage > maxHealth)
        {
            health = maxHealth;
        }
        else
        {
            health += Mathf.RoundToInt((float)outGoingDamage / 2);
        }

        print(name + " attacked " + target.name);
        // TODO: Trigger Attack Animation

    }


    /// <summary>
    /// Call this when killing a target, it handles cleanup and actually destroys the GO
    /// </summary>
    /// <param name="entity"></param>
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

    /// <summary>
    /// Returns an orientation based on the direction of the GO
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public Quaternion SetOrientation(FaceDirection direction)
    {
        //find what orientation should be used
        switch (direction)
        {
            case FaceDirection.forward:
                return Quaternion.Euler(0, 180, 0);
            case FaceDirection.backward:
                return Quaternion.Euler(0, 0, 0);
            case FaceDirection.right:
                return Quaternion.Euler(0, 270, 0);
            case FaceDirection.left:
                return Quaternion.Euler(0, 90, 0);

        }

        //somehow no orientation found (something's broken)
        return Quaternion.Euler(0, 0, 0);
    }

    /// <summary>
    /// Gets the current spell, returns either a spell if found or null if not
    /// </summary>
    /// <returns></returns>
    public Spell DetermineActiveSpell()
    {
        //has an inventory
        if (GetComponent<Inventory>() != null)
        {
            //has an offhand equipped
            if (GetComponent<Inventory>().currentOffHand != null)
            {
                //has a spell equipped
                if (GetComponent<Inventory>().currentOffHand.GetComponent<Spell>() != null)
                {
                    return GetComponent<Inventory>().currentOffHand.GetComponent<Spell>();
                }
            }
        }

        return null;
    }
}
