using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum ClusterShape
{
    Plus,
    TeeNorth,
    TeeSouth,
    TeeEast,
    TeeWest,
    CornerNorthEast,
    CornerSouthEast,
    CornerNorthWest,
    CornerSouthWest,
    VerticalLine,
    HorizontalLine,
    Empty
}
public class Cluster
{
    public Coordinate Location { get; set; }
    public List<ClusterShape> PossibleShapes { get; private set; }
    public ClusterShape DefiniteShape { get; private set; }

    Dictionary<ClusterDirection, Cluster> neighbors;

    
    public bool IsDefiniteShapeSet { get; private set; }

    public int Entropy { get { return PossibleShapes.Count; } }   

    // Start is called before the first frame update
    public Cluster()
    {
        // initialize by adding all possible shapes
        PossibleShapes = new List<ClusterShape>((ClusterShape[])Enum.GetValues(typeof(ClusterShape)));
        IsDefiniteShapeSet = false;
        neighbors = new Dictionary<ClusterDirection, Cluster>();
    }

    public void SetNeighbor(Cluster cluster, ClusterDirection direction)
    {
        neighbors[direction] = cluster;
    }

    public Cluster Neighbor(ClusterDirection direction)
    {
        return neighbors[direction];
    }

    public void ConstrainDirection(ClusterShape[] shapes)
    {
        if (PossibleShapes.Count == 1)
            return;
        // if the shapes are in this group's list of available, or possible, shapes, remove it/them
        foreach (ClusterShape shape in shapes)
        {
            if (PossibleShapes.Contains(shape))
            {
                PossibleShapes.Remove(shape);
            }
        }
    }

    public void SetDefiniteShape(ClusterShape shape)
    {
        DefiniteShape = shape;

        if (PossibleShapes == null)
            PossibleShapes = new List<ClusterShape>();

        PossibleShapes.Clear();
        PossibleShapes.Add(shape);
        IsDefiniteShapeSet = true;
        DefiniteShape = shape;
    }

    public void SetPossibleShapes(List<ClusterShape> shapes)
    {
        if (PossibleShapes == null)
        {
            PossibleShapes = new List<ClusterShape>();
        }
        PossibleShapes.Clear();
        PossibleShapes.AddRange(shapes);
    }

    public bool FilterToCompatibleShapes(ClusterShape[] shapes, ClusterDirection dir)
    {
        // for each of our existing shapes, check if ANY of the incoming shapes are compatible.
        // otherwise remove 
        bool isReduced = false;

        foreach (ClusterShape myShape in shapes)
        {
            bool isCompatible = false;
            // is this shape compatible with any of the incoming shapes?
            // 
            
        }

        if (shapes != null)
        {
            foreach (ClusterShape shape in shapes)
            {

            }
        }
        return isReduced;
    }


    public bool ReducePossibleShapes(ClusterShape[] shapes)
    {
        bool isReduced = false;

        if (shapes != null)
        {
            foreach (ClusterShape shape in shapes)
            {
                if (PossibleShapes.Contains(shape))
                {
                    PossibleShapes.Remove(shape);
                    isReduced = true;
                }
            }
        }

        if (Entropy == 1)
        {
            SetDefiniteShape(PossibleShapes[0]);
        }

        return isReduced;
    }

    public void SetRandomDefiniteShape()
    {
        ClusterShape shape = PossibleShapes[UnityEngine.Random.Range(0, PossibleShapes.Count)];

        SetDefiniteShape(shape);
        PossibleShapes.Clear();
        PossibleShapes.Add(shape);
    }

    // returns clusers to north, east, south, west, in that order.
    // null if no neighbor
}
