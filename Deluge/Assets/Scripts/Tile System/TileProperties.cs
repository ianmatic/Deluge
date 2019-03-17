using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileProperties : MonoBehaviour
{
    //used for pathfinding
    public GameObject previous;
    public int heuristic;
    public int pathDistance;
    public bool isParent = false;

    // Start is called before the first frame update
    void Start()
    {
        previous = null;
        pathDistance = 0;
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

    //cost to reach this vertex from the start plus heuristic vlaue
    public int GetWeightedDistance()
    {
        return pathDistance + heuristic;
    }
}
