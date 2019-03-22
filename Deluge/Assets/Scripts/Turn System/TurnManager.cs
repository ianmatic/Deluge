using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    //all enemies in the scene
    public List<GameObject> enemies;

    private GameObject player;

    //which enemy is having its turn
    private int counter;

    public List<GameObject> combatEntities;

    private float timer = 0.35f;

    // Start is called before the first frame update
    void Start()
    {
        enemies = new List<GameObject>();
        combatEntities = new List<GameObject>();

        player = GameObject.FindGameObjectWithTag("Player");

        enemies.AddRange(GameObject.FindGameObjectsWithTag("enemy"));

        counter = 0;

    }

    // Update is called once per frame
    void Update()
    {

        if (!GameData.GameplayPaused && !GameData.FullPaused)
        {
            //Get all entities that want to fight
            combatEntities = GetCombatEntities();

            //in combat when more than just player in list
            if (combatEntities.Count > 1)
            {
                player.GetComponent<Entity>().inCombat = true;

                GetComponent<CameraManager>().target = combatEntities[counter];

                //timer has been triggered
                if (timer < .35f)
                {
                    timer -= Time.deltaTime;
                }
                else
                {
                    //timer hasn't been triggered yet, so doing turn
                    combatEntities[counter].GetComponent<Entity>().doingTurn = true;

                    //do specific code for different entities, player handled in playerData
                    switch (combatEntities[counter].GetComponent<Entity>().type)
                    {
                        case entityType.enemy:
                            //end turn
                            if (combatEntities[counter].GetComponent<Timer>().remainingTime < 0.35f)
                            {
                                foreach (GameObject tile in combatEntities[counter].GetComponent<EnemyData>().pathToPlayer)
                                {
                                    GetComponent<ShaderManager>().Untint(tile);
                                }

                                //A* pathfinding
                                combatEntities[counter].GetComponent<EnemyData>().pathToPlayer =
                                    GetComponent<TileManager>().Find3DPath(combatEntities[counter], player);

                                foreach (GameObject tile in combatEntities[counter].GetComponent<EnemyData>().pathToPlayer)
                                {
                                    GetComponent<ShaderManager>().TintRed(tile);
                                }

                                combatEntities[counter].GetComponent<EnemyData>().ProcessTurn();

                                //trigger end of turn timer
                                timer = combatEntities[counter].GetComponent<Timer>().remainingTime;
                                combatEntities[counter].GetComponent<Entity>().doingTurn = false;
                            }
                            break;
                        case entityType.player:
                            
                            //trigger end of turn timer 
                            if (combatEntities[counter].GetComponent<Timer>().remainingTime < 0.05f)
                            {
                                timer = 0.349f;
                                combatEntities[counter].GetComponent<Entity>().doingTurn = false;
                            }


                            break;
                    }
                }
            
                //timer has ended
                if (timer <= 0.0f)
                {
                    //advance to next entity
                    combatEntities[counter].GetComponent<Entity>().doingTurn = false;
                    counter++;
                    timer = 0.35f;
                }

                //last entity ended turn
                if (counter == combatEntities.Count)
                {
                    //go back to player
                    counter = 0;
                }
            }
            //end combat
            else
            {
                //for now, have the player go first every time
                counter = 0;
                GetComponent<CameraManager>().target = player;
                player.GetComponent<Entity>().inCombat = false;
                player.GetComponent<Entity>().doingTurn = false;
            }
        }
    }


    /// <summary>
    /// Removes any null (dead) entities from multiple lists
    /// </summary>
    public void RemoveNullAndUpdateEntities()
    {
        for (int i = 0; i < combatEntities.Count; i++)
        {
            if (combatEntities[i] == null)
            {
                combatEntities.Remove(combatEntities[i]);
            }
        }
    }

    public List<GameObject> GetCombatEntities()
    {
        List<GameObject> entities = new List<GameObject>();

        entities.Add(player);

        entities.AddRange(GetNearbyEnemies(player, enemies));

        return entities;
    }


    /// <summary>
    /// Returns a list of enemies that are 3 tiles or less from the player, also sets all enemy inCombat value
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
                enemy.GetComponent<Entity>().inCombat = true;
                nearbyEnemies.Add(enemy);
            }
            else
            {
                enemy.GetComponent<Entity>().inCombat = false;
            }
        }

        return nearbyEnemies;
    }

    public void AttackNearbyEnemies()
    {
        List<GameObject> hittableEnemies = new List<GameObject>();

        //find entities that are in target area
        foreach (GameObject entity in combatEntities)
        {
            switch (entity.GetComponent<Entity>().type)
            {
                case entityType.enemy:
                    foreach (GameObject tile in player.GetComponent<PlayerData>().actionTiles)
                    {
                        //enemy is located on a tile that the player can attack
                        if (entity.GetComponent<Entity>().parentTile == tile)
                        {
                            hittableEnemies.Add(entity);
                        }
                    }
                    break;
            }

        }

        foreach (GameObject enemy in hittableEnemies)
        {
            player.GetComponent<Entity>().Attack(enemy);
        }

    }
}
