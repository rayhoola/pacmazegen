using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClusterMaze
{
    int width, height;
    Cluster[,] grid;
    // Start is called before the first frame update

    public int Length { get { return width * height; } }

    public ClusterMaze(int dimensionX, int dimensionY)
    {
        width = dimensionX;
        height = dimensionY;
        grid = new Cluster[dimensionX, dimensionY];
    }

    public void SetCluster(Coordinate coordinate, Cluster cluster)
    {
        if (grid == null)
            Debug.LogWarning("Trying to set cluster on an empty grid");

        grid[coordinate.X, coordinate.Y] = cluster;
    }
    public bool TryGetCluster(Coordinate coordinate, out Cluster cluster)
    {
        bool outOfBounds = coordinate.X < 0 || coordinate.X > grid.GetLength(0) - 1 || coordinate.Y < 0 || coordinate.Y > grid.GetLength(1) - 1;
        if (outOfBounds)
        {
            cluster = null;
            return false;
        }

        cluster = grid[coordinate.X, coordinate.Y];
        return true;
    }

    public bool CanGetCluster(Coordinate coordinate)
    {
        bool outOfBounds = coordinate.X < 0 || coordinate.X > grid.GetLength(0) - 1 || coordinate.Y < 0 || coordinate.Y > grid.GetLength(1) - 1;
        if (outOfBounds)
        {
            return false;
        }

        return true;
    }

    public int GetLength(int dimension)
    {
        int length = 0;

        switch (dimension)
        {
            case 0:
                length = grid.GetLength(0);
                break;
            case 1:
                length = grid.GetLength(1);
                break;
            default:
                Debug.LogWarning("Invalid dimension requested");
                break;
        }

        return length;
    }

    public List<Cluster> GetLowestEntropy()
    {
        List<Cluster> retval = new List<Cluster>();
        retval.Clear();
        int minEntropy = int.MaxValue;

        // Find tile(s) with lowest entropy (>1)
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cluster cluster = grid[x, y];
                int entropy = cluster.Entropy;
                if (entropy > 1)
                {
                    if (entropy < minEntropy)
                    {
                        minEntropy = entropy;
                        retval.Clear();
                        retval.Add(cluster);
                    }
                    else if (entropy == minEntropy)
                    {
                        retval.Add(cluster);
                    }
                }
            }
        }


        return retval;
    }

    public void DestroyMaze()
    {
        grid = null;
    }
    
}

