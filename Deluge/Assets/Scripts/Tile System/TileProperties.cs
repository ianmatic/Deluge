using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileProperties : MonoBehaviour
{
    [HideInInspector]
    public GameObject manager;

    //used for pathfinding
    [HideInInspector]
    public GameObject previous;

    [HideInInspector]
    public float heuristic;

    [HideInInspector]
    public float pathDistance;

    [HideInInspector]
    public bool isParent = false;
    [HideInInspector]
    public bool isWall = false;

    // Start is called before the first frame update
    void Start()
    {
        previous = null;

        manager = GameObject.FindGameObjectWithTag("manager");

        isWall = IsWall();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float ManhattanDistance(Vector3 start, Vector3 end)
    {
        //estimate of how far this is from the end point
        return Vector3.Distance(start,end);
    }

    //cost to reach this vertex from the start plus heuristic vlaue
    public float GetWeightedDistance()
    {
        return pathDistance + heuristic;
    }

    /// <summary>
    /// Based on surrounding tiles, determines if wall
    /// </summary>
    /// <returns></returns>
    public bool IsWall()
    {
        //get tile height
        float height = GetComponent<Renderer>().bounds.size.y;

        //Find tiles above the current one
        Vector3 pos = transform.position;
        pos.y += height;

        GameObject aboveOne = manager.GetComponent<TileManager>().GetTileAtPosition(pos);

        pos.y += height;

        GameObject aboveTwo = manager.GetComponent<TileManager>().GetTileAtPosition(pos);

        //go down 3 tiles (1 below)
        pos.y -= height * 3;

        GameObject oneBelow = manager.GetComponent<TileManager>().GetTileAtPosition(pos);

        //tiles found above or tile found below and above, so this is a wall, 
        if (aboveOne != null && aboveTwo != null || (oneBelow != null && aboveOne != null))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
