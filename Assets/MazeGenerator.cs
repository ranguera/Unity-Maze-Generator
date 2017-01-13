// Roger Anguera, 01/2016 - ranguera@gmail.com
// modified from https://forum.unity3d.com/threads/quick-maze-generator.173370/
// -

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MazeGenerator : MonoBehaviour
{
    public int width, height;
    public Material brick;
    private int[,] Maze;
    private GameObject[,] MazeObjects;
    private List<Vector3> pathMazes = new List<Vector3>();
    private Stack<Vector2> _tiletoTry = new Stack<Vector2>();
    private Stack<Vector2> _pathToExit = new Stack<Vector2>();
    private List<Vector2> offsets = new List<Vector2> { new Vector2(0, 1), new Vector2(0, -1), new Vector2(1, 0), new Vector2(-1, 0) };
    private System.Random rnd = new System.Random();
    private int _width, _height;
    private Vector2 _currentTile;

    public Vector2 CurrentTile
    {
        get { return _currentTile; }
        private set
        {
            if (value.x < 1 || value.x >= this.width - 1 || value.y < 1 || value.y >= this.height - 1)
            {
                throw new ArgumentException("CurrentTile must be within the one tile border all around the maze");
            }
            if (true)//(value.x % 2 == 1 || value.y % 2 == 1)
            { _currentTile = value; }
            else
            {
                throw new ArgumentException("The current square must not be both on an even X-axis and an even Y-axis, to ensure we can get walls around all tunnels");
            }
        }
    }

    private Vector2 previousTile;

    /*
    private static MazeGenerator instance;
    public static MazeGenerator Instance
    {
        get
        {
            return instance;
        }
    }

    private Vector2 

    void Awake()
    {
        instance = this;
    }
    */

    void Start()
    {
        GenerateMaze();
    }

    void GenerateMaze()
    {
        Maze = new int[width, height];
        MazeObjects = new GameObject[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject ptype = null;
                Maze[x, y] = 1;
                ptype = GameObject.CreatePrimitive(PrimitiveType.Cube);
                ptype.transform.position = new Vector3(x * ptype.transform.localScale.x, y * ptype.transform.localScale.y, 0);
                ptype.GetComponent<Renderer>().material.color = Color.red;
                ptype.transform.parent = transform;
                MazeObjects[x, y] = ptype;
            }
        }
        CurrentTile = Vector2.one;
        _tiletoTry.Push(CurrentTile);
        StartCoroutine(CreateMaze(0));
        

        //RenderMaze();
    }

    private void RenderMaze()
    {
        GameObject ptype = null;
        for (int i = 0; i <= Maze.GetUpperBound(0); i++)
        {
            for (int j = 0; j <= Maze.GetUpperBound(1); j++)
            {
                //print("ij: " + i + "-" + j);
                ptype = MazeObjects[i, j];
                if (Maze[i, j] == 1)
                {
                    if (i % 2 == 0 && j % 2 == 0)
                        ptype.GetComponent<Renderer>().material.color = Color.gray;
                    else
                        ptype.GetComponent<Renderer>().material.color = Color.white;
                }
                else if (Maze[i, j] == 2)
                    ptype.GetComponent<Renderer>().material.color = Color.magenta;
                else if (Maze[i, j] == 0)
                    ptype.GetComponent<Renderer>().material.color = Color.cyan;

                    pathMazes.Add(new Vector3(i, j, 0));
            }
        }

                /*
                for (int i = 0; i <= Maze.GetUpperBound(0); i++)
                {
                    for (int j = 0; j <= Maze.GetUpperBound(1); j++)
                    {
                        if (Maze[i, j] == 1)
                        {
                            ptype = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            ptype.transform.position = new Vector3(i * ptype.transform.localScale.x, j * ptype.transform.localScale.y, 0);
                            ptype.transform.parent = transform;
                        }
                        else if( Maze[i,j] == 2 )
                        {
                            ptype = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            ptype.transform.position = new Vector3(i * ptype.transform.localScale.x, j * ptype.transform.localScale.y, 0);
                            ptype.transform.parent = transform;
                            ptype.GetComponent<Renderer>().material.color = Color.red;
                        }
                        else if (Maze[i, j] == 0)
                        {
                            if (i % 2 == 0 && j % 2 == 0)
                            {
                                ptype.GetComponent<Renderer>().material.color = Color.gray;
                                ptype.transform.parent = transform;
                            }
                            pathMazes.Add(new Vector3(i, j, 0));
                        }

                    }
                }
                */
    }


    // idees: crear un random segons turns directe a exit complet i despres completar (no cridant lo de next neighbor)
    // permetre mes forats ?
    public IEnumerator CreateMaze(int turns)
    {
        bool pathCompleted = false;
        int iteration = 1;
        int moves_made = 1; // 1 because it starts at (1,1)
        Vector2 previousTile = CurrentTile;

        int x = Mathf.FloorToInt((width - 2) / 2);
        int num_cols = Mathf.RoundToInt(Mathf.Pow(x, 2));
        int fastest_route = ((width - 2) * 2) - 1;
        int slowest_route = 0;
        int empty_spaces = Mathf.RoundToInt(Mathf.Pow(x, 2));
        int num_routes = 0;

        if (x % 2 == 1)
            empty_spaces += 2;

        slowest_route = Mathf.RoundToInt(Mathf.Pow((width - 2), 2)) - num_cols - empty_spaces;
        num_routes = ((slowest_route - fastest_route) / 4) - 1;

        // First pass - path to exit with N turns

        //local variable to store neighbors to the current square
        //as we work our way through the maze
        List<Vector2> neighbors;
        //as long as there are still tiles to try
        while (!pathCompleted)
        {
            //excavate the square we are on
            Maze[(int)CurrentTile.x, (int)CurrentTile.y] = 0;

            //get all valid neighbors for the new tile
            neighbors = GetValidNeighbors(CurrentTile);

            //if there are any interesting looking neighbors
            if (neighbors.Count > 0 && moves_made < slowest_route)
            {
                //remember this tile, by putting it on the stack
                _tiletoTry.Push(CurrentTile);
                //move on to a random of the neighboring tiles

                //CurrentTile = NextNeighborToExit(CurrentTile);
                //print("nc: " + neighbors.Count);
                int kk = rnd.Next(neighbors.Count);
                //print("kk: " + kk);
                CurrentTile = neighbors[kk];
                print(CurrentTile);
                moves_made++;
                //print(moves_made);
                /*if (CurrentTile.x != previousTile.x && CurrentTile.y != previousTile.y)
                    turnsMade++;*/

                if (CurrentTile == new Vector2(width - 2, height - 2))
                {
                    Maze[(int)CurrentTile.x, (int)CurrentTile.y] = 0;
                    pathCompleted = true;
                }
            }
            else
            {
                //if there were no neighbors to try, we are at a dead-end
                //toss this tile out
                //(thereby returning to a previous tile in the list to check).

                Maze[(int)CurrentTile.x, (int)CurrentTile.y] = 2; //mark as visited
                moves_made--;

                if ( _tiletoTry.Count >0 )
                    CurrentTile = _tiletoTry.Pop();
                //print(CurrentTile);
                // unexcavate to continue finding the path to exit
                
                //print(moves_made);
            }
            iteration++;

            RenderMaze();
            yield return new WaitForSeconds(.2f);

            //yield return null;
        }


        /*
        // Second pass - complete maze

        //local variable to store neighbors to the current square
        //as we work our way through the maze
        neighbors = new List<Vector2>();
        //as long as there are still tiles to try
        while (_tiletoTry.Count > 0)
        {
            if (CurrentTile == new Vector2(width - 2, height - 2))
                pathCompleted = true;

            //excavate the square we are on
            Maze[(int)CurrentTile.x, (int)CurrentTile.y] = 0;

            //get all valid neighbors for the new tile
            neighbors = GetValidNeighbors(CurrentTile);

            //if there are any interesting looking neighbors
            if (neighbors.Count > 0)
            {
                //remember this tile, by putting it on the stack
                _tiletoTry.Push(CurrentTile);
                //move on to a random of the neighboring tiles
                
                CurrentTile = neighbors[rnd.Next(neighbors.Count)];
            }
            else
            {
                //if there were no neighbors to try, we are at a dead-end
                //toss this tile out
                //(thereby returning to a previous tile in the list to check).
                CurrentTile = _tiletoTry.Pop();
            }
            iteration++;
        }
        */

        // Make sure doors of perception are open
        Maze[1, 0] = 0;
        Maze[1, 1] = 0;
        Maze[width - 2, height - 1] = 0;
        Maze[width - 2, height - 2] = 0;
    }


    /// <summary>
    /// Get all the prospective neighboring tiles
    /// </summary>
    /// <param name="centerTile">The tile to test</param>
    /// <returns>All and any valid neighbors</returns>
    private List<Vector2> GetValidNeighbors(Vector2 centerTile)
    {

        List<Vector2> validNeighbors = new List<Vector2>();

        //Check all four directions around the tile
        foreach (var offset in offsets)
        {
            //find the neighbor's position
            Vector2 toCheck = new Vector2(centerTile.x + offset.x, centerTile.y + offset.y);

            //make sure the tile is not on both an even X-axis and an even Y-axis
            //to ensure we can get walls around all tunnels
            if (toCheck.x % 2 == 1 || toCheck.y % 2 == 1)
            {
                //if the potential neighbor is unexcavated (==1)
                //and still has three walls intact (new territory)
                if (Maze[(int)toCheck.x, (int)toCheck.y] == 1 && HasThreeWallsIntact(toCheck))
                {
                    //add the neighbor
                    validNeighbors.Add(toCheck);
                }
            }
        }

        return validNeighbors;
    }

    /// <summary>
    /// Finds the next neighbour to the straight exit route. goes straight up, then right.
    /// </summary>
    /// <returns></returns>
    private Vector2 NextNeighborToExit(Vector2 current)
    {
        if (current.y < height-2 )
        {
            return new Vector2(current.x, current.y + 1);
        }
        else if ( current.y < 2 )
        {
            return new Vector2(current.x + 1, current.y);
        }
        else if ( current.x > width-2 )
        {
            return new Vector2(current.x, current.y + 1);
        }
        else
        {
            return new Vector2(current.x + 1, current.y);
        }
    }


    /// <summary>
    /// Counts the number of intact walls around a tile
    /// </summary>
    /// <param name="Vector2ToCheck">The coordinates of the tile to check</param>
    /// <returns>Whether there are three intact walls (the tile has not been dug into earlier.</returns>
    private bool HasThreeWallsIntact(Vector2 Vector2ToCheck)
    {
        int intactWallCounter = 0;

        //Check all four directions around the tile
        foreach (var offset in offsets)
        {
            //find the neighbor's position
            Vector2 neighborToCheck = new Vector2(Vector2ToCheck.x + offset.x, Vector2ToCheck.y + offset.y);

            //make sure it is inside the maze, and it hasn't been dug out yet
            if (IsInside(neighborToCheck) && Maze[(int)neighborToCheck.x, (int)neighborToCheck.y] == 1)
            {
                intactWallCounter++;
            }
        }

        //tell whether three walls are intact
        return intactWallCounter == 3;

    }

    private bool IsInside(Vector2 p)
    {
        return p.x >= 0 && p.y >= 0 && p.x < width && p.y < height;
    }
}