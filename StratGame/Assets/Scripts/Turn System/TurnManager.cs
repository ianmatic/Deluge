using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    //all enemies in the scene
    private List<GameObject> enemies;

    private GameObject player;

    //enemies that are active and cause the turn system to activate
    private List<GameObject> nearbyEnemies;

    //which enemy is having its turn
    private int counter;

    // Start is called before the first frame update
    void Start()
    {
        enemies = new List<GameObject>();
        nearbyEnemies = new List<GameObject>();

        player = GameObject.FindGameObjectWithTag("Player");

        enemies.AddRange(GameObject.FindGameObjectsWithTag("enemy"));

        nearbyEnemies = GetNearbyEnemies(player, enemies);

        counter = -1;

    }

    // Update is called once per frame
    void Update()
    {

        //First determine which enemies are close (in combat)
        nearbyEnemies = GetNearbyEnemies(player, enemies);

        //in combat
        if (nearbyEnemies.Count > 0)
        {
            player.GetComponent<PlayerData>().inCombat = true;

            //player's turn
            if (counter == -1)
            {
                GetComponent<CameraManager>().target = player;
                player.GetComponent<Entity>().doingTurn = true;


                //out of time
                if (player.GetComponent<Timer>().remainingTime < 0.05)
                {
                    counter++;
                }
            }
            //go through each enemy
            else
            {
                player.GetComponent<Entity>().doingTurn = false;

                for (int i = 0; i < nearbyEnemies.Count; i++)
                {
                    //handle the current enemy
                    if (counter == i)
                    {
                        //update camera
                        GetComponent<CameraManager>().target = nearbyEnemies[i];
                        nearbyEnemies[i].GetComponent<Entity>().doingTurn = true;

                        //end turn
                        if (nearbyEnemies[i].GetComponent<Timer>().remainingTime <= 0)
                        {
                            foreach (GameObject tile in nearbyEnemies[i].GetComponent<EnemyData>().pathToPlayer)
                            {
                                GetComponent<ShaderManager>().Untint(tile);
                            }

                            //A* pathfinding
                            nearbyEnemies[i].GetComponent<EnemyData>().pathToPlayer = 
                                GetComponent<TileManager>().FindPath(nearbyEnemies[i], player);

                            foreach (GameObject tile in nearbyEnemies[i].GetComponent<EnemyData>().pathToPlayer)
                            {
                                GetComponent<ShaderManager>().TintRed(tile);
                            }

                            nearbyEnemies[i].GetComponent<EnemyData>().ProcessTurn();

                            nearbyEnemies[i].GetComponent<Entity>().doingTurn = false;
                            counter++;
                        }
                    }
                }

                //last enemy ended turn
                if (counter == nearbyEnemies.Count)
                {
                    //go back to player
                    counter = -1;
                }
            }

        }
        else
        {
            //for now, have the player go first every time
            counter = -1;
            GetComponent<CameraManager>().target = player;
            player.GetComponent<PlayerData>().inCombat = false;
        }
    }


    /// <summary>
    /// Returns a list of enemies that are 3 tiles or less from the player
    /// </summary>
    /// <param name="player"></param>
    /// <param name="enemies"></param>
    /// <returns></returns>
    public List<GameObject> GetNearbyEnemies(GameObject player, List<GameObject> enemies)
    {
        List<GameObject> nearbyEnemies = new List<GameObject>();

        //determine which enemies are close
        foreach (GameObject enemy in enemies)
        {
            //n tiles proximity
            if (Vector3.Distance(player.transform.position, enemy.transform.position) < 5)
            {
                nearbyEnemies.Add(enemy);
            }
        }

        return nearbyEnemies;
    }

    public void AttackNearbyEnemies()
    {
        List<GameObject> hittableEnemies = new List<GameObject>();

        //find enemies that are in target area
        foreach (GameObject enemy in nearbyEnemies)
        {
            foreach (GameObject tile in player.GetComponent<PlayerData>().actionTiles)
            {
                //enemy is located on a tile that the player can attack
                if (enemy.GetComponent<Entity>().parentTile == tile)
                {
                    hittableEnemies.Add(enemy);
                }
            }
        }

        foreach (GameObject enemy in hittableEnemies)
        {
            player.GetComponent<Entity>().Attack(enemy);
        }

    }
}
