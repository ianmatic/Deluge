using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyData : MonoBehaviour
{
    public List<GameObject> pathToPlayer;
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        pathToPlayer = new List<GameObject>();
        GetComponent<Entity>().time = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        
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
        //approach the player
        if (pathToPlayer.Count > 2)
        {
            GetComponent<Entity>().SetTileAsParentTile(pathToPlayer[1]);
        }
        //attack the player
        else if (pathToPlayer.Count <= 2)
        {
            GetComponent<Entity>().Attack(player);
        }
    }

    
}
