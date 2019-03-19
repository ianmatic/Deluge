using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Attach this to the manager object

public class TileManager : MonoBehaviour
{
    public List<GameObject> tiles;

    //do this before the other GOs need the tiles, so use Awake
    private void Awake()
    {
        tiles = new List<GameObject>();
        tiles.AddRange(GameObject.FindGameObjectsWithTag("tile"));
        tiles.AddRange(GameObject.FindGameObjectsWithTag("spawn"));
    }


    /// <summary>
    /// Finds a tile to move to based on the target position (where the ParentTile wants to go)
    /// </summary>
    /// <param name="gameObject"></param>
    /// <returns></returns>
    public GameObject UpdateParentTile(Vector3 targetPosition, GameObject oldParent)
    {

        /*Summary of Method*/
        /*  The method first checks if it should be checking for vertical tiles (which it will unless it is the first frame, when oldParent is null)
         *  Then it moves to an applicable tile
         * 
         *  For first frame, this is just the tile the GO is on top of
         *  
         *  For the rest of the frames, this is more complicated, the order of checking is:
         *  1. Check for horizontal
         *  2. If horizontal found, check for above (and move to if possible)
         *  3. If horizontal not found, check for above, THEN bottom (and move to if possible)
         *
         *  If a applicable tile is not found, then the GO stays in place (oldParent is returned)
         */


        GameObject newParentTile = null;

        Vector3 horizontalPotentialPosition = targetPosition;

        //look for tile one above the horizontal
        Vector3 abovePotentialPosition = targetPosition;
        abovePotentialPosition.y += .5f;

        //look for tile two below (can fall 2 tiles)
        Vector3 secondBelowPotentialPosition = targetPosition;
        secondBelowPotentialPosition.y -= 1;

        //look for tile one below
        Vector3 belowPotentialPosition = targetPosition;
        belowPotentialPosition.y -= .5f;

        //first frame, just set the beginning parentTile (will always be directly underneath)
        if (oldParent == null)
        {
            newParentTile = GetTileAtPosition(horizontalPotentialPosition);
        }
        //also check for vertical movement
        else
        {
            //found horizontal tile (no vertical movement)
            if (GetTileAtPosition(horizontalPotentialPosition) != null)
            {
                //found a tile one above
                if (GetTileAtPosition(abovePotentialPosition) != null)
                {
                    //check if there is yet another tile on top of the previous one (can only move up by 1)
                    abovePotentialPosition.y += .5f;
                    //no tile too high, able to move up
                    if (GetTileAtPosition(abovePotentialPosition) == null)
                    {
                        abovePotentialPosition.y -= .5f;
                        newParentTile = GetTileAtPosition(abovePotentialPosition);

                    }

                    //reset
                    abovePotentialPosition = targetPosition;
                }
                else
                {
                    //default to this unless a tile is found for vertical movement
                    newParentTile = GetTileAtPosition(horizontalPotentialPosition);
                }

            }
            else
            {
                //check for tiles above
                //found a tile one above
                //going up has priority over going down
                if (GetTileAtPosition(abovePotentialPosition) != null)
                {
                    //check if there is yet another tile on top of the previous one (can only move up by 1)
                    abovePotentialPosition.y += .5f;
                    //no tile too high, able to move up
                    if (GetTileAtPosition(abovePotentialPosition) == null)
                    {
                        //move gameobject to one tile up
                        abovePotentialPosition.y -= .5f;
                        newParentTile = GetTileAtPosition(abovePotentialPosition);
                    }
                }
                //check for tiles below
                else
                {
                    //move 2 down if missing first block
                    if (GetTileAtPosition(secondBelowPotentialPosition) != null
                        && GetTileAtPosition(belowPotentialPosition) == null)
                    {
                        newParentTile = GetTileAtPosition(secondBelowPotentialPosition);
                    }
                    //move 1 down if missing 2nd block or if both present
                    if ((GetTileAtPosition(belowPotentialPosition) != null
                        && GetTileAtPosition(secondBelowPotentialPosition) == null)
                        ||
                        (GetTileAtPosition(belowPotentialPosition) != null
                        && GetTileAtPosition(secondBelowPotentialPosition) != null))

                    {
                        newParentTile = GetTileAtPosition(belowPotentialPosition);
                    }


                }

            }

        }

        //no suitable tile found or the suitable tile is taken, no movement
        if (newParentTile == null || newParentTile.GetComponent<TileProperties>().isParent)
        {
            return oldParent;
        }
        //found suitable tile, movement
        else
        {
            //update parenting of tiles
            if (oldParent != null)
            {
                oldParent.GetComponent<TileProperties>().isParent = false;
            }
            newParentTile.GetComponent<TileProperties>().isParent = true;

            return newParentTile;
        }

    }

    /// <summary>
    /// Returns a tile at position if it exists, otherwise returns null
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public GameObject GetTileAtPosition(Vector3 position)
    {
        foreach (GameObject tile in tiles)
        {
            //found a tile
            if (tile.transform.position == position)
            {
                return tile;
            }
        }

        //no tile found
        return null;
    }


    /// <summary>
    /// Given start GO and end GO, find shortest path (currently only works horizontally)
    /// </summary>
    /// <param name="player"></param>
    /// <param name="enemy"></param>
    /// <param name="tiles"></param>
    /// <returns></returns>
    public List<GameObject> FindPath(GameObject start, GameObject end)
    {
        //get end positions
        int endXPos = (int)end.GetComponent<Entity>().parentTile.transform.position.x;
        int endZPos = (int)end.GetComponent<Entity>().parentTile.transform.position.z;

        //calculate each heuristic and reset the previous
        foreach (GameObject tile in tiles)
        {
            tile.GetComponent<TileProperties>().heuristic =
                tile.GetComponent<TileProperties>().ManhattanDistance((int)tile.transform.position.x, (int)tile.transform.position.z, endXPos, endZPos);

            tile.GetComponent<TileProperties>().previous = null;
        }

        //get startTile
        GameObject startTile = start.GetComponent<Entity>().parentTile;

        //track which tiles have been visited
        List<GameObject> openTiles = new List<GameObject>();
        List<GameObject> closedTiles = new List<GameObject>();

        //returned path
        List<GameObject> path = new List<GameObject>();

        //start at the GO's position
        GameObject currentTile = startTile;

        //Begin algorithm

        //exit loop when this is false
        bool findingPath = true;

        //put the start tile in the closed list
        closedTiles.Add(startTile);

        //search until we find the path, or determine there is no possible path
        while (findingPath)
        {
            //check the neighbors of each tile
            for (int currentX = (int)currentTile.transform.position.x - 1; currentX <= currentTile.transform.position.x + 1; currentX++)
            {
                for (int currentZ = (int)currentTile.transform.position.z - 1; currentZ <= currentTile.transform.position.z + 1; currentZ++)
                {
                    //if it's this cell, ignore it
                    if (currentX == currentTile.transform.position.x && currentZ == currentTile.transform.position.z)
                    {
                        continue;
                    }

                    //ignore diagonals
                    if ((currentX == currentTile.transform.position.x - 1 && currentZ == currentTile.transform.position.z - 1) ||
                        (currentX == currentTile.transform.position.x - 1 && currentZ == currentTile.transform.position.z + 1) ||
                        (currentX == currentTile.transform.position.x + 1 && currentZ == currentTile.transform.position.z - 1) ||
                        (currentX == currentTile.transform.position.x + 1 && currentZ == currentTile.transform.position.z + 1))
                    {
                        continue;
                    }

                    //don't check tiles that don't exist
                    if (GetTileAtPosition(new Vector3(currentX, startTile.transform.position.y, currentZ)) == null)
                    {
                        continue;
                    }

                    //don't check cells that have already been checked (in the closed list)
                    bool inClosed = false;
                    //loop through closed list
                    for (int i = 0; i < closedTiles.Count; i++)
                    {
                        //found the current Tile in closed tiles
                        if (closedTiles[i].transform.position.x == GetTileAtPosition(new Vector3(currentX, startTile.transform.position.y, currentZ)).transform.position.x
                            && closedTiles[i].transform.position.z == GetTileAtPosition(new Vector3(currentX, startTile.transform.position.y, currentZ)).transform.position.z)
                        {
                            inClosed = true;
                            break;
                        }
                    }

                    //since we found a cell in closed list, don't check this iteration
                    if (inClosed)
                    {
                        continue;
                    }

                    //found the end Tile
                    if (GetTileAtPosition(new Vector3(currentX, startTile.transform.position.y, currentZ)).transform.position.x == endXPos
                        && GetTileAtPosition(new Vector3(currentX, startTile.transform.position.y, currentZ)).transform.position.z == endZPos)
                    {
                        //set the end's parent
                        GetTileAtPosition(new Vector3(currentX, startTile.transform.position.y, currentZ)).GetComponent<TileProperties>().previous = currentTile;

                        //add the current Tile to the visited list
                        closedTiles.Add(GetTileAtPosition(new Vector3(currentX, startTile.transform.position.y, currentZ)));

                        //set the current Tile to the end so that we traverse from the end to the beginning
                        currentTile = GetTileAtPosition(new Vector3(currentX, startTile.transform.position.y, currentZ));

                        //add the end to the path, add ends to the end in c#
                        path.Add(currentTile);

                        //while current isn't the start Tile
                        while (currentTile.GetComponent<TileProperties>().previous != null)
                        {
                            //add parent to the front of the list
                            path.Insert(0, currentTile.GetComponent<TileProperties>().previous);

                            //setup Tile for next iteration (singly linked list essentially)
                            currentTile = currentTile.GetComponent<TileProperties>().previous;
                        }

                        //no longer need to keep searching
                        findingPath = false;

                        //return the path we've found
                        return path;
                    }

                    //at this point, the current Tile is valid

                    int xDistance = (int)(GetTileAtPosition(new Vector3(currentX, startTile.transform.position.y, currentZ)).transform.position.x - currentTile.transform.position.x);
                    int zDistance = (int)(GetTileAtPosition(new Vector3(currentX, startTile.transform.position.y, currentZ)).transform.position.z - currentTile.transform.position.z);

                    //calculate movement distance using distance formula + path distance
                    int moveCost = currentTile.GetComponent<TileProperties>().pathDistance + (int)Mathf.Sqrt(Mathf.Pow(xDistance, 2) + Mathf.Pow(zDistance, 2));

                    //no parent Tile or the current Tile's path distance is shorter
                    if (GetTileAtPosition(new Vector3(currentX, startTile.transform.position.y, currentZ)).GetComponent<TileProperties>().previous == null
                        || moveCost < GetTileAtPosition(new Vector3(currentX, startTile.transform.position.y, currentZ)).GetComponent<TileProperties>().pathDistance)
                    {
                        //that means we should use this Tile
                        //so assign it as a parent
                        //and update the pathDistance
                        GetTileAtPosition(new Vector3(currentX, startTile.transform.position.y, currentZ)).GetComponent<TileProperties>().previous = currentTile;
                        GetTileAtPosition(new Vector3(currentX, startTile.transform.position.y, currentZ)).GetComponent<TileProperties>().pathDistance = moveCost;

                        //check if the Tile is on the open list, if not, then add it

                        //nothing in open tiles, which means there's no way the Tile is already inside
                        if (openTiles.Count == 0)
                        {
                            openTiles.Add(GetTileAtPosition(new Vector3(currentX, startTile.transform.position.y, currentZ)));
                        }
                        //open tiles has contents
                        else
                        {

                            bool inOpen = false;

                            //loop through open tiles and check each Tile
                            for (int i = 0; i < openTiles.Count; i++)
                            {
                                //Tile already inside open tiles
                                if (openTiles[i].transform.position.x == GetTileAtPosition(new Vector3(currentX, startTile.transform.position.y, currentZ)).transform.position.x
                                    && openTiles[i].transform.position.z == GetTileAtPosition(new Vector3(currentX, startTile.transform.position.y, currentZ)).transform.position.z)
                                {
                                    inOpen = true;

                                    //no reason to traverse anymore, so break
                                    break;
                                }
                            }

                            //Tile not in openTiles
                            if (!inOpen)
                            {
                                //add the Tile to open tiles
                                openTiles.Add(GetTileAtPosition(new Vector3(currentX, startTile.transform.position.y, currentZ)));
                            }
                        }

                    }

                }
            }

            //at this point, we've checked the neighbors of the current Tile

            //if open tiles has tiles inside it, that means we have more tiles to check
            if (openTiles.Count != 0)
            {
                //find the shortest distance in the open list

                //so start from the beginning
                int shortestDistance = openTiles[0].GetComponent<TileProperties>().GetWeightedDistance();
                int shortestIndex = 0;

                //check each Tile, start at 1 since already set to beginning
                for (int i = 1; i < openTiles.Count; i++)
                {
                    //found a shorter distance
                    if (openTiles[i].GetComponent<TileProperties>().GetWeightedDistance() < shortestDistance)
                    {
                        //set to new shortest distance
                        shortestDistance = openTiles[i].GetComponent<TileProperties>().GetWeightedDistance();
                        shortestIndex = i;
                    }
                }

                //set the current Tile to the one with the shortest distance
                currentTile = openTiles[shortestIndex];

                //since we've checked it, add it to the closed list
                closedTiles.Add(currentTile);

                //remove it from the open list
                openTiles.Remove(currentTile);
            }
            else //no open tiles means no path was found
            {
                findingPath = false;
            }
        }

        //should NOT hit this if the maze is solvable
        return path;
    }

    /// <summary>
    /// Finds the tiles that the player can interact with in combat
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public List<GameObject> FindActionTiles(GameObject player, FaceDirection direction)
    {

        List<GameObject> tiles = new List<GameObject>();
        Vector3 parentTilePosition = player.GetComponent<Entity>().parentTile.transform.position;

        if (direction == FaceDirection.none)
        {
            return tiles;
        }
        else
        {
            //determine tiles based on weapon
            if (player.GetComponent<PlayerData>().weaponSelected == "axe")
            {
                //By default, sword hits top right and adjacent, relative to direction
                //z + 1
                if (direction == FaceDirection.forward)
                {
                    tiles.Add(GetTileAtPosition(
                        new Vector3(parentTilePosition.x,
                                    parentTilePosition.y,
                                    parentTilePosition.z + 1
                                    )));
                    tiles.Add(GetTileAtPosition(
                        new Vector3(parentTilePosition.x + 1,
                                    parentTilePosition.y,
                                    parentTilePosition.z + 1
                                    )));
                    tiles.Add(GetTileAtPosition(
                        new Vector3(parentTilePosition.x + 1,
                                    parentTilePosition.y,
                                    parentTilePosition.z
                                    )));
                }
                //x + 1                                     
                else if (direction == FaceDirection.right)
                {
                    tiles.Add(GetTileAtPosition(
                        new Vector3(parentTilePosition.x + 1,
                                    parentTilePosition.y,
                                    parentTilePosition.z
                                    )));
                    tiles.Add(GetTileAtPosition(
                        new Vector3(parentTilePosition.x + 1,
                                    parentTilePosition.y,
                                    parentTilePosition.z - 1
                                    )));
                    tiles.Add(GetTileAtPosition(
                        new Vector3(parentTilePosition.x,
                                    parentTilePosition.y,
                                    parentTilePosition.z - 1
                                    )));
                }
                //z - 1
                else if (direction == FaceDirection.backward)
                {
                    tiles.Add(GetTileAtPosition(
                         new Vector3(parentTilePosition.x,
                                     parentTilePosition.y,
                                     parentTilePosition.z - 1
                                     )));
                    tiles.Add(GetTileAtPosition(
                        new Vector3(parentTilePosition.x - 1,
                                    parentTilePosition.y,
                                    parentTilePosition.z - 1
                                    )));
                    tiles.Add(GetTileAtPosition(
                        new Vector3(parentTilePosition.x - 1,
                                    parentTilePosition.y,
                                    parentTilePosition.z
                                    )));
                }
                //x - 1
                else if (direction == FaceDirection.left)
                {
                    tiles.Add(GetTileAtPosition(
                    new Vector3(parentTilePosition.x - 1,
                                parentTilePosition.y,
                                parentTilePosition.z
                                )));
                    tiles.Add(GetTileAtPosition(
                        new Vector3(parentTilePosition.x - 1,
                                    parentTilePosition.y,
                                    parentTilePosition.z + 1
                                    )));
                    tiles.Add(GetTileAtPosition(
                        new Vector3(parentTilePosition.x,
                                    parentTilePosition.y,
                                    parentTilePosition.z + 1
                                    )));
                }
            }
            else if (player.GetComponent<PlayerData>().weaponSelected == "spear")
            {
                if (direction == FaceDirection.forward)
                {
                    tiles.Add(GetTileAtPosition(
                        new Vector3(parentTilePosition.x,
                                    parentTilePosition.y,
                                    parentTilePosition.z + 1
                                    )));
                    tiles.Add(GetTileAtPosition(
                        new Vector3(parentTilePosition.x,
                                    parentTilePosition.y,
                                    parentTilePosition.z + 2
                                    )));
                }
                else if (direction == FaceDirection.right)
                {
                    tiles.Add(GetTileAtPosition(
                         new Vector3(parentTilePosition.x + 1,
                                    parentTilePosition.y,
                                    parentTilePosition.z
                                    )));
                    tiles.Add(GetTileAtPosition(
                        new Vector3(parentTilePosition.x + 2,
                                    parentTilePosition.y,
                                    parentTilePosition.z
                                    )));
                }
                else if (direction == FaceDirection.backward)
                {
                    tiles.Add(GetTileAtPosition(
                         new Vector3(parentTilePosition.x,
                                    parentTilePosition.y,
                                    parentTilePosition.z - 2
                                    )));
                    tiles.Add(GetTileAtPosition(
                        new Vector3(parentTilePosition.x,
                                    parentTilePosition.y,
                                    parentTilePosition.z - 1
                                    )));
                }
                else if (direction == FaceDirection.left)
                {
                    tiles.Add(GetTileAtPosition(
                        new Vector3(parentTilePosition.x - 1,
                                   parentTilePosition.y,
                                   parentTilePosition.z
                                   )));
                    tiles.Add(GetTileAtPosition(
                        new Vector3(parentTilePosition.x - 2,
                                    parentTilePosition.y,
                                    parentTilePosition.z
                                    )));
                }
            }
            else if (player.GetComponent<PlayerData>().weaponSelected == "bow")
            {
                if (direction == FaceDirection.forward)
                {
                    tiles.Add(GetTileAtPosition(
                        new Vector3(parentTilePosition.x,
                                    parentTilePosition.y,
                                    parentTilePosition.z + 2
                                    )));
                    tiles.Add(GetTileAtPosition(
                        new Vector3(parentTilePosition.x,
                                    parentTilePosition.y,
                                    parentTilePosition.z + 3
                                    )));
                }
                else if (direction == FaceDirection.right)
                {
                    tiles.Add(GetTileAtPosition(
                         new Vector3(parentTilePosition.x + 2,
                                    parentTilePosition.y,
                                    parentTilePosition.z
                                    )));
                    tiles.Add(GetTileAtPosition(
                        new Vector3(parentTilePosition.x + 3,
                                    parentTilePosition.y,
                                    parentTilePosition.z
                                    )));
                }
                else if (direction == FaceDirection.backward)
                {
                    tiles.Add(GetTileAtPosition(
                         new Vector3(parentTilePosition.x,
                                    parentTilePosition.y,
                                    parentTilePosition.z - 3
                                    )));
                    tiles.Add(GetTileAtPosition(
                        new Vector3(parentTilePosition.x,
                                    parentTilePosition.y,
                                    parentTilePosition.z - 2
                                    )));
                }
                else if (direction == FaceDirection.left)
                {
                    tiles.Add(GetTileAtPosition(
                        new Vector3(parentTilePosition.x - 3,
                                   parentTilePosition.y,
                                   parentTilePosition.z
                                   )));
                    tiles.Add(GetTileAtPosition(
                        new Vector3(parentTilePosition.x - 2,
                                    parentTilePosition.y,
                                    parentTilePosition.z
                                    )));
                }
            }
        }


        //remove any null tiles
        tiles.RemoveAll(tile => tile == null);

        return tiles;
    }

    /// <summary>
    /// Finds the tile that the player can interact with outside of combat (based on direction)
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public GameObject FindInteractTile(GameObject player, FaceDirection direction)
    {
        //get the position
        Vector3 parentTilePosition = player.GetComponent<Entity>().parentTile.transform.position;

        //choose appropriate tile based on direction
        //z + 1
        if (direction == FaceDirection.forward)
        {
            parentTilePosition.z += 1;
        }
        //z - 1
        else if (direction == FaceDirection.backward)
        {
            parentTilePosition.z -= 1;
        }
        //x - 1
        else if (direction == FaceDirection.left)
        {
            parentTilePosition.x -= 1;
        }
        //x + 1
        else if (direction == FaceDirection.right)
        {
            parentTilePosition.x += 1;
        }
        else
        {
            return null;
        }

        return GetTileAtPosition(parentTilePosition);
    }

    /// <summary>
    /// Finds tiles around the entity
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public List<GameObject> FindAdjacentTiles(GameObject entity, bool corners)
    {
        List<GameObject> nearbyTiles = new List<GameObject>();
        GameObject parentTile = entity.GetComponent<Entity>().parentTile;

        if (corners)
        {
            //find all tiles in 3x3 grid around parentTile (not including parentTile)
            for (float x = parentTile.transform.position.x - 1; x < parentTile.transform.position.x + 1; x++)
            {
                for (float z = parentTile.transform.position.z - 1; z < parentTile.transform.position.z + 1; z++)
                {
                    //ignore parent tile
                    if (parentTile.transform.position.x == x && parentTile.transform.position.z == z)
                    {
                        continue;
                    }

                    //add the tiles
                    nearbyTiles.Add(GetTileAtPosition(new Vector3(x, parentTile.transform.position.y, z)));
                }
            }
        }
        else
        {
            Vector3 parentPos = parentTile.transform.position;

            //only cardinal direction
            nearbyTiles.Add(GetTileAtPosition(new Vector3(parentPos.x - 1, parentPos.y, parentPos.z)));
            nearbyTiles.Add(GetTileAtPosition(new Vector3(parentPos.x + 1, parentPos.y, parentPos.z)));

            nearbyTiles.Add(GetTileAtPosition(new Vector3(parentPos.x, parentPos.y, parentPos.z - 1)));
            nearbyTiles.Add(GetTileAtPosition(new Vector3(parentPos.x, parentPos.y, parentPos.z + 1)));
        }

        //remove any null tiles
        nearbyTiles.RemoveAll(tile => tile == null);

        return nearbyTiles;
    }
}
