using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ClusterMazeGen
{

    int dimensionX;
    int dimensionY;

    ClusterMaze maze;
    private List<Cluster> lowEntropyList;

    public ClusterMazeGen()
    {
        // Default constructor
    }

    public ClusterMazeGen(int dimensionX = 10, int dimensionY = 10)
    {
        this.dimensionX = dimensionX;
        this.dimensionY = dimensionY;
    }


    public void InitMaze(int width, int height)
    {
        Debug.Log("Initializing maze with dimensions: " + width + "x" + height);
        string debugoutput;

        lowEntropyList = new List<Cluster>();

        this.dimensionX = width;
        this.dimensionY = height;

        maze = new ClusterMaze(dimensionX, dimensionY);

        // create clusters - add all possible shapes
        for (int x = 0; x < dimensionX; x++)
        {
            debugoutput = "";
            for (int y = 0; y < dimensionY; y++)
            {
                Cluster cluster = new Cluster();
                maze.SetCluster(new Coordinate(x, y), cluster);
                cluster.Location = new Coordinate(x, y);
                debugoutput += cluster.Entropy.ToString() + " ";
            }
            Debug.Log(debugoutput);
        }
        Debug.Log("=================");

        for (int x = 0; x < dimensionX; x++)
        {
            for (int y = 0; y < dimensionY; y++)
            {
                Cluster neighbor;
                Cluster cluster;
                maze.TryGetCluster(new Coordinate(x, y), out cluster);

                maze.TryGetCluster(new Coordinate(cluster.Location.X, cluster.Location.Y+1), out neighbor);
                cluster.SetNeighbor(neighbor, ClusterDirection.North);
                maze.TryGetCluster(new Coordinate(cluster.Location.X + 1, cluster.Location.Y), out neighbor);
                cluster.SetNeighbor(neighbor, ClusterDirection.East);
                maze.TryGetCluster(new Coordinate(cluster.Location.X, cluster.Location.Y-1), out neighbor);
                cluster.SetNeighbor(neighbor, ClusterDirection.South);
                maze.TryGetCluster(new Coordinate(cluster.Location.X - 1, cluster.Location.Y), out neighbor);
                cluster.SetNeighbor(neighbor, ClusterDirection.West);
            }
        }
        // start by creating a boundary around the outer edge of the maze
        // and decide where the exists will be



        //  InitializeBoundaryClusters(); bugbug maybe lets just try setting a list of concrete constraints
        // place exits and collapse
        // place any empty cells and collapse (this forced dead areas in the map - useful for making non-rectangular mazes, or areas for other visual props to exist?)
        PlaceExits();

        InitializeBoundaryClusters();

        // now pick one of the nodes with lowest entropy and collapse it
        // and propagate the constraints    
        // repeat until all nodes are collapsed or a contradiction is found
        // if a contradiction is found, restart the process
        // if the process takes too long, restart the process
        // once the maze is fully collapsed, return the maze


        UpdateLowEntropyList();

        while (UpdateLowEntropyList() == true)
        {
            // randomly pick a low entropy cell, and collapse it
            int idx = Random.Range(0, lowEntropyList.Count);
            Debug.Log("collapse at " + idx);
            Cluster target = lowEntropyList[idx];
            target.SetRandomDefiniteShape();
            PropagateConstraints(target);
        }


        Debug.Log("Final maze:");
        for (int x = 0; x < dimensionX; x++)
        {
            debugoutput = "";
            for (int y = 0; y < dimensionY; y++)
            {
                Cluster debugcluster;
                maze.TryGetCluster(new Coordinate(x, y), out debugcluster);
                debugoutput += debugcluster.DefiniteShape.ToString() + " ";
            }
            Debug.Log(debugoutput);
        }

    }

    void PropagateConstraints(Cluster cluster)
    {
        /*
        while (stack.is_empty() == False):
            tile = stack.pop()
            tilePossibilities = tile.getPossibilities()
            directions = tile.getDirections()

            for direction in directions:
                neighbour = tile.getNeighbour(direction)
                if neighbour.entropy != 0:
                    reduced = neighbour.constrain(tilePossibilities, direction)
                    if reduced == True:
                        stack.push(neighbour)    # When possibilities were reduced need to propagate further
        */

        Stack<Cluster> stack = new Stack<Cluster>();
        stack.Push(cluster);

        while (stack.Count > 0)
        {

            Cluster targetCluster = stack.Pop();

            foreach (ClusterDirection dir in ClusterDirection.GetValues(typeof(ClusterDirection)))
            {
                // do each of 4 directions in order

                Cluster neighbor = targetCluster.Neighbor(dir);
                bool modified = false;

                if (neighbor == null)
                    continue;

                // 1. get list of all current possible shapes in this cluster, 
                // 2. and for each of those shapes, get the list of compatible neighbor shapes in that direction
                // 3. then, subtract this list from the list of all shapes
                // 4. this will leave just the list of shapes that are incompatible with ANY of our possible shapes, in that direction

                List<ClusterShape> compatibleShapes = new List<ClusterShape>();
                foreach (ClusterShape possibleShape in targetCluster.PossibleShapes)
                {
                    ClusterShape[] validNeigborShapes = ClusterRules.validNeighbors[new Tuple<ClusterShape, ClusterDirection>(possibleShape, dir)];
                    if (validNeigborShapes == null)
                    {
                        continue;
                    }

                    foreach (ClusterShape neighborShape in validNeigborShapes)
                    {
                        if (!compatibleShapes.Contains(neighborShape))
                        {
                            compatibleShapes.Add(neighborShape);
                        }
                    }
                }

                // now we have all compatible shapes in this direction
                // so now remove these from the total list of shapes to reduce down to just incompatible shapes
                List<ClusterShape> remainderShapes = new List<ClusterShape>();
                foreach (ClusterShape candidateShape in ClusterShape.GetValues(typeof(ClusterShape)))
                {
                    if (!remainderShapes.Contains(candidateShape))
                    {
                        remainderShapes.Add(candidateShape);
                    }
                }

                // and remove any shape from the total list if it's a compatible shape (i.e. we done want to remove those shapes from the neighbor)
                foreach (ClusterShape currentShape in compatibleShapes)
                {
                    if (remainderShapes.Contains(currentShape))
                    {
                        remainderShapes.Remove(currentShape);
                    }

                }

                // now remainder is just the list of incompatible shapes

                if (neighbor.Entropy > 1) // > 2 since we need 1 concrete shape and the "empty" entry
                {
                    modified = neighbor.ReducePossibleShapes(remainderShapes.ToArray());
                }

                if (modified)
                {
                    stack.Push(neighbor);
                }
            }
        }

    }

   

    private bool UpdateLowEntropyList()
    {
        bool isList = false;

        int lowest = int.MaxValue;
        lowEntropyList.Clear();

        for (int i = 0; i < dimensionX; i++)
        {
            for (int j = 0; j < dimensionY; j++)
            {
                Cluster cluster;
                
                bool foundCluster = maze.TryGetCluster(new Coordinate(i, j), out cluster);

                if (foundCluster)
                {
                    if ((cluster.Entropy == 1) || (cluster.Entropy > lowest))  // tile is done or entropy is greater
                        continue;


                    if (cluster.Entropy < lowest)
                    {

                        lowest = cluster.Entropy;
                        lowEntropyList.Clear();
                        lowEntropyList.Add(cluster);
                        isList = true;

                    }
                    else if (cluster.Entropy == lowest)
                    {
                        lowEntropyList.Add(cluster);
                        isList = true;
                    }
                }

            }
        }

        return isList;
    }



    // bugbug make reference counts of each incompatible shape?
    private void CompareShapes(Cluster updatedGroup, Coordinate nextCoordinate, ClusterDirection directionToNextGroup, Cluster[,] tileGroupMatrix)
    {
      
    }

    void PlaceExits()
    {
        int exitFlags = Random.Range(1, 16);
        Debug.Log("Exit flags: " + exitFlags);

        if ((exitFlags & 1) == 1)
        { 
            // place an exit
            Cluster exitCluster;
            maze.TryGetCluster(new Coordinate(0, dimensionY / 2), out exitCluster);
            if (exitCluster == null)
            {
                Debug.LogError("Cluster at " + 0 + "," + dimensionY / 2 + " is null");
            }
            else
            {
                exitCluster.SetDefiniteShape(ClusterShape.HorizontalLine);
            }
            PropagateConstraints(exitCluster);

        }

        if ((exitFlags & 2) == 2)
        {
            // place an exit
            Cluster exitCluster;
            maze.TryGetCluster(new Coordinate(dimensionX - 1, dimensionY / 2), out exitCluster);
            if (exitCluster == null)
            {
                Debug.LogError("Cluster at " + (dimensionX - 1) + "," + dimensionY / 2 + " is null");
            }
            else
            {
                exitCluster.SetDefiniteShape(ClusterShape.HorizontalLine);
            }
            PropagateConstraints(exitCluster);

        }

        if ((exitFlags & 4) == 4)
        {
            // place an exit
            Cluster exitCluster;
            maze.TryGetCluster(new Coordinate(dimensionX / 2, 0), out exitCluster);
            if (exitCluster == null)
            {
                Debug.LogError("Cluster at " + (dimensionX / 2) + "," + 0 + " is null");
            }
            else
            {
                exitCluster.SetDefiniteShape(ClusterShape.VerticalLine);
            }
            PropagateConstraints(exitCluster);

        }

        if ((exitFlags & 8) == 8)
        {
            // place an exit
            Cluster exitCluster;
            maze.TryGetCluster(new Coordinate(dimensionX / 2, dimensionY - 1), out exitCluster);
            if (exitCluster == null)
            {
                Debug.LogError("Cluster at " + dimensionX / 2 + "," + (dimensionY - 1) + " is null");
            }
            else
            {
                exitCluster.SetDefiniteShape(ClusterShape.VerticalLine);
            }
            PropagateConstraints(exitCluster);

        }
    }

    void InitializeBoundaryClusters()
    {
        string debugoutput;
        Cluster cluster;
        maze.TryGetCluster(new Coordinate(0, 0), out cluster); // bottom left
        {
            cluster.ReducePossibleShapes(ClusterRules.ConnectsWest);
            cluster.ReducePossibleShapes(ClusterRules.ConnectsSouth);
            cluster.ReducePossibleShapes(ClusterRules.Empty);
        }
        PropagateConstraints(cluster);
        maze.TryGetCluster(new Coordinate(0, dimensionY-1), out cluster); // top left
        {
            cluster.ReducePossibleShapes(ClusterRules.ConnectsNorth);
            cluster.ReducePossibleShapes(ClusterRules.ConnectsWest);
            cluster.ReducePossibleShapes(ClusterRules.Empty);
        }
        PropagateConstraints(cluster);
        maze.TryGetCluster(new Coordinate(dimensionX-1, 0), out cluster); // bottom right
        {
            cluster.ReducePossibleShapes(ClusterRules.ConnectsSouth);
            cluster.ReducePossibleShapes(ClusterRules.ConnectsEast);
            cluster.ReducePossibleShapes(ClusterRules.Empty);
        }
        PropagateConstraints(cluster);
        maze.TryGetCluster(new Coordinate(dimensionX - 1, dimensionY - 1), out cluster); // top right
        {
            cluster.ReducePossibleShapes(ClusterRules.ConnectsNorth);
            cluster.ReducePossibleShapes(ClusterRules.ConnectsEast);
            cluster.ReducePossibleShapes(ClusterRules.Empty);
        }
        PropagateConstraints(cluster);

        for (int x = 0; x < dimensionX; x++)
        {
            for (int y = 0; y < dimensionY; y++)
            {
                // if we're on an edge, place a wall
                bool isEdge = x == 0 || x == dimensionX - 1 || y == 0 || y == dimensionY - 1;
                if (isEdge)
                {
                   
                    maze.TryGetCluster(new Coordinate(x, y), out cluster);

                    if (cluster == null)
                    {
                        Debug.LogError("Cluster at " + x + "," + y + " is null");
                        continue;
                    }

                    // place a wall
                    if (cluster.Entropy ==1)
                    {
                        continue;
                    }

                    if (x == 0 && y == 0) // bottom left corner
                    {
                        cluster.ReducePossibleShapes(ClusterRules.ConnectsWest);
                        cluster.ReducePossibleShapes(ClusterRules.ConnectsSouth);
                        cluster.ReducePossibleShapes(ClusterRules.Empty);
                    }
                    else if (x == dimensionX - 1 && y == 0) // bottom right corner
                    {
                        cluster.ReducePossibleShapes(ClusterRules.ConnectsSouth);
                        cluster.ReducePossibleShapes(ClusterRules.ConnectsEast);
                        cluster.ReducePossibleShapes(ClusterRules.Empty);
                    }
                    else if (x == 0 && y == dimensionY - 1) // Top Left corner
                    {
                        cluster.ReducePossibleShapes(ClusterRules.ConnectsNorth);
                        cluster.ReducePossibleShapes(ClusterRules.ConnectsWest);
                        cluster.ReducePossibleShapes(ClusterRules.Empty);
                    }
                    else if (x == dimensionX - 1 && (y == dimensionY - 1)) // top right corner
                    {
                        cluster.ReducePossibleShapes(ClusterRules.ConnectsNorth);
                        cluster.ReducePossibleShapes(ClusterRules.ConnectsEast);
                        cluster.ReducePossibleShapes(ClusterRules.Empty);
                    }
                    else if (x == 0) // left edge
                    {
                        cluster.ReducePossibleShapes(ClusterRules.ConnectsWest);
                        cluster.ReducePossibleShapes(ClusterRules.Empty);
                    }
                    else if (x == dimensionX - 1) // right edge
                    {
                        cluster.ReducePossibleShapes(ClusterRules.ConnectsEast);
                        cluster.ReducePossibleShapes(ClusterRules.Empty);
                    }
                    else if (y == 0) // bottom edge
                    {
                        cluster.ReducePossibleShapes(ClusterRules.ConnectsSouth);
                        cluster.ReducePossibleShapes(ClusterRules.Empty);
                    }
                    else if (y == dimensionY - 1) // top edge
                    {
                        cluster.ReducePossibleShapes(ClusterRules.ConnectsNorth);
                        cluster.ReducePossibleShapes(ClusterRules.Empty);
                    }

                    PropagateConstraints(cluster);

                }

                // randomly pick 1 to 4 possible exit sides on the boundary




                // if we're at a corner, make it a corner wall
                // if we're on an edge but not a corner, make it a straight wall    
                // if we're on an edge and at a designated exit point, make it an exit

            }
        }


        


        Debug.Log("Afer setting boundaries:");

        for (int x = 0; x < dimensionX; x++)
        {
            debugoutput = "";
            for (int y = 0; y < dimensionY; y++)
            {
                Cluster debugcluster;
                maze.TryGetCluster(new Coordinate(x, y), out debugcluster);
                debugoutput += debugcluster.Entropy.ToString() + " ";
            }
            Debug.Log(debugoutput);
        }
        Debug.Log("=================");

    }

    void CollapseAtLocation(Coordinate coordinate)
    {
        Cluster cluster;
        if (maze.TryGetCluster(coordinate, out cluster))
        {
            if (cluster.Entropy > 1)
            {
                SetRandomDefiniteShape(cluster);
                // propagate constraints to neighbors
            }
            else
            {
                // propagate constraints to neighbors
            }

        }
        else
        {
            Debug.LogError("Cannot collapse at " + coordinate + " - out of bounds");
        }
    }

    void SetRandomDefiniteShape(Cluster cluster)
    {
        if (cluster == null)
        {
            Debug.LogError("Cannot set random definite shape for null cluster");
            return;
        }

        if (cluster.IsDefiniteShapeSet)
        {
            Debug.LogWarning("Cluster already has definite shape set");
            return;
        }

        int shapeidx = Random.Range(0, cluster.PossibleShapes.Count);
        cluster.SetDefiniteShape(cluster.PossibleShapes[shapeidx]);
    }

    void UpdateNeighbors(Cluster parentCluster, Coordinate coordinate)
    {

    }

    void UpdateLowestEntropy()
    {
    }
}
