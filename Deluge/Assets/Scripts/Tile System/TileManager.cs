using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Attach this to the manager object

public class TileManager : MonoBehaviour
{
    [HideInInspector]
    public Dictionary<Vector3, GameObject> tiles;

    //do this before the other GOs need the tiles, so use Awake
    private void Awake()
    {
        tiles = new Dictionary<Vector3, GameObject>();

        foreach (GameObject tile in GameObject.FindGameObjectsWithTag("tile"))
        {
            tiles.Add(tile.transform.position, tile);
        }

        foreach (GameObject tile in GameObject.FindGameObjectsWithTag("spawn"))
        {
            tiles.Add(tile.transform.position, tile);
        }
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
        GameObject tryGO;

        //try to get a tile at value and output it in tryGO
        if (tiles.TryGetValue(position, out tryGO))
        {
            return tryGO;
        }
        else
        {
            return null;
        }

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
        //shouldn't be hardcoded since tile height may change

        //get end positions
        Vector3 endPos = end.GetComponent<Entity>().parentTile.transform.position;

        //calculate each heuristic and reset the previous
        foreach (KeyValuePair<Vector3, GameObject> tile in tiles)
        {
            tile.Value.GetComponent<TileProperties>().heuristic =
                Vector3.Distance(tile.Value.transform.position, endPos);

            tile.Value.GetComponent<TileProperties>().previous = null;
        }

        //get startTile
        GameObject startTile = start.GetComponent<Entity>().parentTile;

        float tileHeight = startTile.GetComponent<Renderer>().bounds.size.y;

        //track which tiles have been visited
        List<GameObject> openTiles = new List<GameObject>();
        List<GameObject> closedTiles = new List<GameObject>();

        //returned path
        List<GameObject> path = new List<GameObject>();

        //start at the GO's position
        GameObject currentTile = startTile;
        Vector3 currentPos = start.transform.position;

        //Begin algorithm

        //exit loop when this is false
        bool findingPath = true;

        //put the start tile in the closed list
        closedTiles.Add(startTile);

        //search until we find the path, or determine there is no possible path
        while (findingPath)
        {
            //check the neighbors of each tile
            for (float currentX = currentTile.transform.position.x - 1; currentX <= currentTile.transform.position.x + 1; currentX++)
            {
                for (float currentZ = currentTile.transform.position.z - 1; currentZ <= currentTile.transform.position.z + 1; currentZ++)
                {
                    //check each vertical layer of tiles
                    for (float currentY = currentTile.transform.position.y - (tileHeight * 2); currentY <= currentTile.transform.position.y + tileHeight; currentY += tileHeight)
                    {
                        currentPos = new Vector3(currentX, currentY, currentZ);

                        GameObject loopTile = GetTileAtPosition(currentPos);

                        //if it's this cell, ignore it
                        if (currentPos == currentTile.transform.position)
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
                        if (loopTile == null)
                        {
                            continue;
                        }

                        //don't check tiles occupied already unless it's the players one
                        if (loopTile.GetComponent<TileProperties>().isParent && loopTile.transform.position != endPos)
                        {
                            continue;
                        }

                        //don't check walls (tiles that have 2 tiles above it)
                        if (loopTile.GetComponent<TileProperties>().isWall)
                        {
                            continue;
                        }

                        //don't check tiles with 1 tile above
                        if (GetTileAtPosition(new Vector3(currentX, currentY + tileHeight, currentZ)) != null)
                        {
                            continue;
                        }

                        //don't check tiles in closed list
                        if (closedTiles.Contains(loopTile))
                        {
                            continue;
                        }

                        //found the end Tile
                        if (loopTile.transform.position == endPos)
                        {
                            //set the end's parent
                            loopTile.GetComponent<TileProperties>().previous = currentTile;

                            //add the current Tile to the visited list
                            closedTiles.Add(loopTile);

                            //set the current Tile to the end so that we traverse from the end to the beginning
                            currentTile = loopTile;

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

                        float xDistance = loopTile.transform.position.x - currentTile.transform.position.x;
                        float yDistance = loopTile.transform.position.y - currentTile.transform.position.y;
                        float zDistance = loopTile.transform.position.z - currentTile.transform.position.z;

                        //calculate movement distance using distance formula + path distance
                        float moveCost = currentTile.GetComponent<TileProperties>().pathDistance + Mathf.Sqrt(Mathf.Pow(xDistance, 2) + Mathf.Pow(yDistance, 2) + Mathf.Pow(zDistance, 2));

                        //no parent Tile or the current Tile's path distance is shorter
                        if (loopTile.GetComponent<TileProperties>().previous == null
                            || moveCost < loopTile.GetComponent<TileProperties>().pathDistance)
                        {
                            //that means we should use this Tile
                            //so assign it as a parent
                            //and update the path3DDistance
                            loopTile.GetComponent<TileProperties>().previous = currentTile;
                            loopTile.GetComponent<TileProperties>().pathDistance = moveCost;

                            //check if the Tile is on the open list, if not, then add it

                            //nothing in open tiles, which means there's no way the Tile is already inside
                            if (openTiles.Count == 0)
                            {
                                openTiles.Add(loopTile);
                            }
                            //open tiles has contents
                            else
                            {

                                bool inOpen = false;

                                //loop through open tiles and check each Tile
                                for (int i = 0; i < openTiles.Count; i++)
                                {
                                    //Tile already inside open tiles
                                    if (openTiles[i].transform.position == loopTile.transform.position)
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
                                    openTiles.Add(loopTile);
                                }
                            }

                        }
                    }
                }
            }

            //at this pofloat, we've checked the neighbors of the current Tile

            //if open tiles has tiles inside it, that means we have more tiles to check
            if (openTiles.Count != 0)
            {
                //find the shortest distance in the open list

                //so start from the beginning
                float shortestDistance = openTiles[0].GetComponent<TileProperties>().GetWeightedDistance();
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
        float tileHeight = player.GetComponent<Entity>().parentTile.GetComponent<Renderer>().bounds.size.y;

        for (float y = parentTilePosition.y + tileHeight; y >= parentTilePosition.y - tileHeight; y -= tileHeight)
        {
            //determine tiles based on weapon
            if (player.GetComponent<PlayerData>().weaponSelected == "axe")
            {
                //By default, sword hits top right and adjacent, relative to direction
                //z + 1
                if (direction == FaceDirection.backward)
                {

                    tiles.Add(GetHighestTile(parentTilePosition + new Vector3(0, 0, 1), tileHeight));

                    tiles.Add(GetHighestTile(parentTilePosition + new Vector3(1, 0, 1), tileHeight));

                    tiles.Add(GetHighestTile(parentTilePosition + new Vector3(1, 0, 0), tileHeight));
                }
                //x + 1                                     
                else if (direction == FaceDirection.right)
                {
                    tiles.Add(GetHighestTile(parentTilePosition + new Vector3(1, 0, 0), tileHeight));

                    tiles.Add(GetHighestTile(parentTilePosition + new Vector3(1, 0, -1), tileHeight));

                    tiles.Add(GetHighestTile(parentTilePosition + new Vector3(0, 0, -1), tileHeight));
                }
                //z - 1
                else if (direction == FaceDirection.forward)
                {
                    tiles.Add(GetHighestTile(parentTilePosition + new Vector3(0, 0, -1), tileHeight));

                    tiles.Add(GetHighestTile(parentTilePosition + new Vector3(-1, 0, -1), tileHeight));

                    tiles.Add(GetHighestTile(parentTilePosition + new Vector3(-1, 0, 0), tileHeight));
                }
                //x - 1
                else if (direction == FaceDirection.left)
                {
                    tiles.Add(GetHighestTile(parentTilePosition + new Vector3(-1, 0, 0), tileHeight));

                    tiles.Add(GetHighestTile(parentTilePosition + new Vector3(-1, 0, 1), tileHeight));

                    tiles.Add(GetHighestTile(parentTilePosition + new Vector3(0, 0, 1), tileHeight));
                }
            }
            else if (player.GetComponent<PlayerData>().weaponSelected == "spear")
            {
                if (direction == FaceDirection.backward)
                {
                    tiles.Add(GetHighestTile(parentTilePosition + new Vector3(0, 0, 1), tileHeight));

                    tiles.Add(GetHighestTile(parentTilePosition + new Vector3(0, 0, 2), tileHeight));
                }
                else if (direction == FaceDirection.right)
                {
                    tiles.Add(GetHighestTile(parentTilePosition + new Vector3(1, 0, 0), tileHeight));

                    tiles.Add(GetHighestTile(parentTilePosition + new Vector3(2, 0, 0), tileHeight));
                }
                else if (direction == FaceDirection.forward)
                {
                    tiles.Add(GetHighestTile(parentTilePosition + new Vector3(0, 0, -2), tileHeight));

                    tiles.Add(GetHighestTile(parentTilePosition + new Vector3(0, 0, -1), tileHeight));
                }
                else if (direction == FaceDirection.left)
                {
                    tiles.Add(GetHighestTile(parentTilePosition + new Vector3(-1, 0, 0), tileHeight));

                    tiles.Add(GetHighestTile(parentTilePosition + new Vector3(-2, 0, 0), tileHeight));
                }
            }
            else if (player.GetComponent<PlayerData>().weaponSelected == "bow")
            {
                if (direction == FaceDirection.backward)
                {
                    tiles.Add(GetHighestTile(parentTilePosition + new Vector3(0, 0, 2), tileHeight));

                    tiles.Add(GetHighestTile(parentTilePosition + new Vector3(0, 0, 3), tileHeight));
                }
                else if (direction == FaceDirection.right)
                {
                    tiles.Add(GetHighestTile(parentTilePosition + new Vector3(2, 0, 0), tileHeight));

                    tiles.Add(GetHighestTile(parentTilePosition + new Vector3(3, 0, 0), tileHeight));
                }
                else if (direction == FaceDirection.forward)
                {
                    tiles.Add(GetHighestTile(parentTilePosition + new Vector3(0, 0, -3), tileHeight));

                    tiles.Add(GetHighestTile(parentTilePosition + new Vector3(0, 0, -2), tileHeight));
                }
                else if (direction == FaceDirection.left)
                {
                    tiles.Add(GetHighestTile(parentTilePosition + new Vector3(-3, 0, 0), tileHeight));

                    tiles.Add(GetHighestTile(parentTilePosition + new Vector3(-2, 0, 0), tileHeight));
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
        //needed to find vertically highest tile

        float tileHeight = player.GetComponent<Entity>().parentTile.GetComponent<Renderer>().bounds.size.y;

        //get the position
        Vector3 parentTilePosition = player.GetComponent<Entity>().parentTile.transform.position;

        //choose appropriate tile based on direction
        //z + 1
        if (direction == FaceDirection.backward)
        {
            parentTilePosition.z += 1;
        }
        //z - 1
        else if (direction == FaceDirection.forward)
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

        //check for vertical interactivity
        return GetHighestTile(parentTilePosition, tileHeight);
    }

    /// <summary>
    /// Finds tiles around the entity
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public List<GameObject> FindAdjacentTiles(GameObject entity, bool corners)
    {
        //used to determine vertical selection
        float tileHeight = entity.GetComponent<Entity>().parentTile.GetComponent<Renderer>().bounds.size.y;

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

                    //add the tiles, also account for verticallity
                    nearbyTiles.Add(GetHighestTile(new Vector3(x, parentTile.transform.position.y, z), tileHeight));
                }
            }
        }
        else
        {
            Vector3 parentPos = parentTile.transform.position;

            //only cardinal direction
            nearbyTiles.Add(GetHighestTile(new Vector3(parentPos.x - 1, parentPos.y, parentPos.z), tileHeight));
            nearbyTiles.Add(GetHighestTile(new Vector3(parentPos.x + 1, parentPos.y, parentPos.z), tileHeight));

            nearbyTiles.Add(GetHighestTile(new Vector3(parentPos.x, parentPos.y, parentPos.z - 1), tileHeight));
            nearbyTiles.Add(GetHighestTile(new Vector3(parentPos.x, parentPos.y, parentPos.z + 1), tileHeight));
        }

        //remove any null tiles
        nearbyTiles.RemoveAll(tile => tile == null);

        return nearbyTiles;
    }

    /// <summary>
    /// Finds the highest tile of 3, starts from top and goes down
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="tileHeight"></param>
    /// <returns></returns>
    public GameObject GetHighestTile(Vector3 pos, float tileHeight)
    {
        GameObject tile;

        //loop from top to bottom
        for (float y = pos.y + tileHeight; y >= pos.y - tileHeight; y -= tileHeight)
        {
            tile = GetTileAtPosition(new Vector3(pos.x, y, pos.z));

            //found tile, no need to keep checking
            if (tile != null)
            {
                return tile;
            }
        }

        //no tile found
        return null;

    }
}
