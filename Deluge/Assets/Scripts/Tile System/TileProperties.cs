using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileProperties : MonoBehaviour
{
    //used for pathfinding
    public GameObject previous;

    public int heuristic;
    public float heuristic3D;

    public int pathDistance;
    public float path3DDistance;

    public bool isParent = false;
    public bool isWall = false;

    public GameObject manager;

    // Start is called before the first frame update
    void Start()
    {
        previous = null;
        pathDistance = 0;

        manager = GameObject.FindGameObjectWithTag("manager");

        isWall = IsWall();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int ManhattanDistance(int xPos, int zPos, int endX, int endZ)
    {
        //estimate of how far this is from the end point
        return Mathf.Abs(xPos - endX) + Mathf.Abs(zPos - endZ);
    }

    public float Manhattan3DDistance(float xPos, float yPos, float zPos, float endX, float endY, float endZ)
    {
        //estimate of how far this is from the end point
        return Mathf.Abs(xPos - endX) + Mathf.Abs(yPos - endY) + Mathf.Abs(zPos - endZ);
    }

    //cost to reach this vertex from the start plus heuristic vlaue
    public int GetWeightedDistance()
    {
        return pathDistance + heuristic;
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
