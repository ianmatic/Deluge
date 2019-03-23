using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileProperties : MonoBehaviour
{
    //used for pathfinding
    public GameObject previous;

    public float heuristic3D;

    public float path3DDistance;

    public bool isParent = false;
    public bool isWall = false;

    public GameObject manager;

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

    public float Manhattan3DDistance(Vector3 start, Vector3 end)
    {
        //estimate of how far this is from the end point
        return Vector3.Distance(start,end);
    }

    //cost to reach this vertex from the start plus heuristic vlaue
    public float GetWeightedDistance()
    {
        return path3DDistance + heuristic3D;
    }

    /// <summary>
    /// Based on surrounding tiles, determines if wall
    /// </summary>
    /// <returns></returns>
    public bool IsWall()
    {
        float height = GetComponent<Renderer>().bounds.size.y;

        Vector3 pos = transform.position;
        pos.y += height;

        GameObject aboveOne = manager.GetComponent<TileManager>().GetTileAtPosition(pos);

        pos.y += height;

        GameObject aboveTwo = manager.GetComponent<TileManager>().GetTileAtPosition(pos);

        //tiles found above, so this is a wall
        if (aboveOne != null && aboveTwo != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
