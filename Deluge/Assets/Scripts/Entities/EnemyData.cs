using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyData : MonoBehaviour
{
    public List<GameObject> pathToPlayer;
    private GameObject player;
    public GameObject manager;
    public List<GameObject> wanderTiles;

    private float wanderTimer = 1.0f;

    enum EnemyType
    {
        melee
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
                            //update direction
                            GetComponent<Entity>().direction = UpdateDirectionBasedOnTiles(
                                GetComponent<Entity>().parentTile, wanderTiles[randomNumber]);

                            GetComponent<Entity>().SetTileAsParentTile(wanderTiles[randomNumber]);
                        }


                    }


                    //reset timer
                    wanderTimer = 1.0f;
                }
            }
            else
            {
                wanderTimer = 1.0f;
            }

            #region testing directionality of enemies with tinting
            if (GetComponent<Entity>().direction == FaceDirection.forward)
            {
                manager.GetComponent<ShaderManager>().TintBlue(gameObject);
            }
            else if (GetComponent<Entity>().direction == FaceDirection.backward)
            {
                manager.GetComponent<ShaderManager>().TintRed(gameObject);
            }
            else if (GetComponent<Entity>().direction == FaceDirection.right)
            {
                manager.GetComponent<ShaderManager>().TintGreen(gameObject);
            }
            else if (GetComponent<Entity>().direction == FaceDirection.left)
            {
                manager.GetComponent<ShaderManager>().Untint(gameObject);
            }
            #endregion

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
            return FaceDirection.forward;
        }
        //z - 1
        else if (currentParent.transform.position.z > futureParent.transform.position.z)
        {
            return FaceDirection.backward;
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
