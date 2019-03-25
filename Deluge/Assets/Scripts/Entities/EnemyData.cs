using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;

public class EnemyData : MonoBehaviour
{
    [HideInInspector]
    public List<GameObject> pathToPlayer;
    [HideInInspector]
    public GameObject manager;
    [HideInInspector]
    public List<GameObject> wanderTiles;

    private GameObject player;

    private float wanderTimer = 1.0f;

    GameObject currentStartTile;
    GameObject currentEndTile;

    //set in inspector
    public EnemyType type;

    public enum EnemyType
    {
        melee,
        archer
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        manager = GameObject.FindGameObjectWithTag("manager");
        pathToPlayer = new List<GameObject>();
        wanderTiles = new List<GameObject>();
        GetComponent<Entity>().maxTime = 1.0f;
        GetComponent<Entity>().type = entityType.enemy;
        GetComponent<Entity>().health = 10;
    }

    // Update is called once per frame
    void Update()
    {
        //no combat when paused
        if (!GameData.GameplayPaused && !GameData.FullPaused)
        {
            //wander
            if (!GetComponent<Entity>().inCombat)
            {
                wanderTimer -= Time.deltaTime;

                //timer ends
                if (wanderTimer < 0)
                {
                    //only cardinal directions
                    wanderTiles = manager.GetComponent<TileManager>().FindAdjacentTiles(gameObject, false);

                    //tiles to wander to
                    if (wanderTiles.Count > 0)
                    {
                        //the + 1 is so that there is a chance the entity does nothing
                        int randomNumber = Random.Range(0, wanderTiles.Count + 1);

                        //wander to random tile if number is not beyond list indices
                        if (randomNumber < wanderTiles.Count)
                        {
                            //don't go through walls
                            if (!wanderTiles[randomNumber].GetComponent<TileProperties>().isWall)
                            {
                                //update direction and parentTile
                                GetComponent<Entity>().direction = UpdateDirectionBasedOnTiles(
                                    GetComponent<Entity>().parentTile, wanderTiles[randomNumber]);

                                GetComponent<Entity>().SetTileAsParentTile(wanderTiles[randomNumber]);
                            }

                        }


                    }


                    //reset timer
                    wanderTimer = Random.Range(.8f, 1.2f);
                }
            }
            else
            {
                //reset timer
                wanderTimer = Random.Range(.8f, 1.2f);
            }


            //A* Pathfinding
            //if the parent tiles have changed since last path was found is the other part
            if (currentStartTile != GetComponent<Entity>().parentTile || currentEndTile != player.GetComponent<Entity>().parentTile)
            {
                //try to find a path
                if (Vector3.Distance(player.transform.position, transform.position) < 10)
                {
                    pathToPlayer = manager.GetComponent<TileManager>().FindPath(gameObject, player);
                }
                //set tiles regardless of success of A*
                currentStartTile = GetComponent<Entity>().parentTile;
                currentEndTile = player.GetComponent<Entity>().parentTile;
            }
        }
    }

    public void ProcessTurn()
    {
        //Determine action
        CalculateAction();

        Debug.Log(name + " is ending its turn");


    }

    /// <summary>
    /// Helper function to determine what action the object will take
    /// </summary>
    public void CalculateAction()
    {
        //Actual pathfinding is calculated in Turn Manager
        switch (type)
        {
            case EnemyType.melee:
                //approach the player
                if (pathToPlayer.Count > 2)
                {
                    //update directionality
                    GetComponent<Entity>().direction = UpdateDirectionBasedOnTiles(
                        GetComponent<Entity>().parentTile, pathToPlayer[1]);

                    GetComponent<Entity>().SetTileAsParentTile(pathToPlayer[1]);
                }
                //attack the player (2 because includes enemy's and player's tiles)
                else if (pathToPlayer.Count == 2)
                {
                    GetComponent<Entity>().direction = UpdateDirectionBasedOnTiles(
                        GetComponent<Entity>().parentTile, player.GetComponent<Entity>().parentTile);

                    GetComponent<Entity>().Attack(player);
                }
                break;
            case EnemyType.archer:
                //approach the player
                if (pathToPlayer.Count > 5)
                {
                    //update directionality
                    GetComponent<Entity>().direction = UpdateDirectionBasedOnTiles(
                        GetComponent<Entity>().parentTile, pathToPlayer[1]);

                    GetComponent<Entity>().SetTileAsParentTile(pathToPlayer[1]);
                }
                //attack the player (2 because includes enemy's and player's tiles)
                else if (pathToPlayer.Count > 1)
                {
                    GetComponent<Entity>().direction = UpdateDirectionBasedOnTiles(
                        GetComponent<Entity>().parentTile, player.GetComponent<Entity>().parentTile);

                    GetComponent<Entity>().Attack(player);
                }
                break;
        }
        
    }

    /// <summary>
    /// Based on differences between futureParent and currentParent, determines direction
    /// For movement: futureParent is tile entity wants to move to
    /// For attacking: futureParent is parentTile of entity beign targeted
    /// </summary>
    /// <param name="currentParent"></param>
    /// <param name="futureParent"></param>
    /// <returns></returns>
    public FaceDirection UpdateDirectionBasedOnTiles(GameObject currentParent, GameObject futureParent)
    {
        //z + 1
        if (currentParent.transform.position.z < futureParent.transform.position.z)
        {
            return FaceDirection.backward;
        }
        //z - 1
        else if (currentParent.transform.position.z > futureParent.transform.position.z)
        {
            return FaceDirection.forward;
        }
        //x + 1
        else if (currentParent.transform.position.x < futureParent.transform.position.x)
        {
            return FaceDirection.right;                                                                       
        }
        //x - 1
        else if (currentParent.transform.position.x > futureParent.transform.position.x)
        {
            return FaceDirection.left;
        }
        //none
        else
        {
            return GetComponent<Entity>().direction;
        }
    }

    
}
