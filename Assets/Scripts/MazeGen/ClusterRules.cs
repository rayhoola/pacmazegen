using System;
using System.Collections.Generic;

public enum ClusterDirection
{
    North,
    South,
    East,
    West
}

// list all tiles by name/id
public static class tileIDs
{
    // in our current case, it's just archetypes, not specific tiles, but this should be extendable to more specific tiles

}

public class ClusterRule
{
    ClusterShape[] north;
    ClusterShape[] east;
    ClusterShape[] south;
    ClusterShape[] west;
}

// list all valid tile id's that connect to the north south east west for each tile


public static class ClusterRules
{
    public static ClusterShape[] ConnectsNorth = { ClusterShape.Plus, ClusterShape.TeeNorth, ClusterShape.TeeEast, ClusterShape.TeeWest, ClusterShape.CornerNorthEast, ClusterShape.CornerNorthWest, ClusterShape.VerticalLine };
    public static ClusterShape[] NotConnectsNorth = { ClusterShape.TeeSouth, ClusterShape.CornerSouthEast, ClusterShape.CornerSouthWest, ClusterShape.HorizontalLine, ClusterShape.Empty };

    public static ClusterShape[] ConnectsSouth = { ClusterShape.Plus, ClusterShape.TeeSouth, ClusterShape.TeeEast, ClusterShape.TeeWest, ClusterShape.CornerSouthEast, ClusterShape.CornerSouthWest, ClusterShape.VerticalLine };
    public static ClusterShape[] NotConnectsSouth = { ClusterShape.TeeNorth, ClusterShape.CornerNorthEast, ClusterShape.CornerNorthWest, ClusterShape.HorizontalLine, ClusterShape.Empty };

    public static ClusterShape[] ConnectsEast = { ClusterShape.Plus, ClusterShape.TeeNorth, ClusterShape.TeeSouth, ClusterShape.TeeEast, ClusterShape.CornerNorthEast, ClusterShape.CornerSouthEast, ClusterShape.HorizontalLine };
    public static ClusterShape[] NotConnectsEast = { ClusterShape.TeeWest, ClusterShape.CornerNorthWest, ClusterShape.CornerSouthWest, ClusterShape.VerticalLine, ClusterShape.Empty };

    public static ClusterShape[] ConnectsWest = { ClusterShape.Plus, ClusterShape.TeeNorth, ClusterShape.TeeSouth, ClusterShape.TeeWest, ClusterShape.CornerNorthWest, ClusterShape.CornerSouthWest, ClusterShape.HorizontalLine };
    public static ClusterShape[] NotConnectsWest = { ClusterShape.TeeEast, ClusterShape.CornerNorthEast, ClusterShape.CornerSouthEast, ClusterShape.VerticalLine, ClusterShape.Empty };

    public static ClusterShape[] Empty = { ClusterShape.Empty };


    public static bool Contains(ClusterShape[] shapes, ClusterShape target)
    {
        bool isFound = false;

        foreach (ClusterShape shape in shapes)
        {
            if (shape == target)
            {
                isFound = true;
            }
        }

        return isFound;
    }


    public static Dictionary<Tuple<ClusterShape, ClusterDirection>, ClusterShape[]> validNeighbors = new()
    {
        //Plus
        { new Tuple<ClusterShape, ClusterDirection>(ClusterShape.Plus, ClusterDirection.North), ConnectsSouth },
        { new Tuple<ClusterShape, ClusterDirection>(ClusterShape.Plus, ClusterDirection.South), ConnectsNorth },
        { new Tuple<ClusterShape, ClusterDirection>(ClusterShape.Plus, ClusterDirection.East), ConnectsWest },
        { new Tuple<ClusterShape, ClusterDirection>(ClusterShape.Plus, ClusterDirection.West), ConnectsEast },

        //TeeUp
        { new Tuple<ClusterShape, ClusterDirection>(ClusterShape.TeeNorth, ClusterDirection.North), ConnectsSouth },
        { new Tuple<ClusterShape, ClusterDirection>(ClusterShape.TeeNorth, ClusterDirection.South), NotConnectsNorth },
        { new Tuple<ClusterShape, ClusterDirection>(ClusterShape.TeeNorth, ClusterDirection.East), ConnectsWest },
        { new Tuple<ClusterShape, ClusterDirection>(ClusterShape.TeeNorth, ClusterDirection.West), ConnectsEast },

        //TeeDown
        { new Tuple<ClusterShape, ClusterDirection>(ClusterShape.TeeSouth, ClusterDirection.North), NotConnectsSouth },
        { new Tuple<ClusterShape, ClusterDirection>(ClusterShape.TeeSouth, ClusterDirection.South), ConnectsNorth },
        { new Tuple<ClusterShape, ClusterDirection>(ClusterShape.TeeSouth, ClusterDirection.East), ConnectsWest },
        { new Tuple<ClusterShape, ClusterDirection>(ClusterShape.TeeSouth, ClusterDirection.West), ConnectsEast },

        //TeeRight
        { new Tuple<ClusterShape, ClusterDirection>(ClusterShape.TeeEast, ClusterDirection.North), ConnectsSouth },
        { new Tuple<ClusterShape, ClusterDirection>(ClusterShape.TeeEast, ClusterDirection.South), ConnectsNorth },
        { new Tuple<ClusterShape, ClusterDirection>(ClusterShape.TeeEast, ClusterDirection.East), ConnectsWest },
        { new Tuple<ClusterShape, ClusterDirection>(ClusterShape.TeeEast, ClusterDirection.West), NotConnectsEast },

        //TeeLeft
        { new Tuple<ClusterShape, ClusterDirection>(ClusterShape.TeeWest, ClusterDirection.North), ConnectsSouth },
        { new Tuple<ClusterShape, ClusterDirection>(ClusterShape.TeeWest, ClusterDirection.South), ConnectsNorth },
        { new Tuple<ClusterShape, ClusterDirection>(ClusterShape.TeeWest, ClusterDirection.East), NotConnectsWest },
        { new Tuple<ClusterShape, ClusterDirection>(ClusterShape.TeeWest, ClusterDirection.West), ConnectsEast },

        //CornerNorthEast
        { new Tuple<ClusterShape, ClusterDirection>(ClusterShape.CornerNorthEast, ClusterDirection.North), ConnectsSouth },
        { new Tuple<ClusterShape, ClusterDirection>(ClusterShape.CornerNorthEast, ClusterDirection.South), NotConnectsNorth },
        { new Tuple<ClusterShape, ClusterDirection>(ClusterShape.CornerNorthEast, ClusterDirection.East), ConnectsWest },
        { new Tuple<ClusterShape, ClusterDirection>(ClusterShape.CornerNorthEast, ClusterDirection.West), NotConnectsEast },

        //CornerSouthEast
        { new Tuple<ClusterShape, ClusterDirection>(ClusterShape.CornerSouthEast, ClusterDirection.North), NotConnectsSouth },
        { new Tuple<ClusterShape, ClusterDirection>(ClusterShape.CornerSouthEast, ClusterDirection.South), ConnectsNorth },
        { new Tuple<ClusterShape, ClusterDirection>(ClusterShape.CornerSouthEast, ClusterDirection.East), ConnectsWest },
        { new Tuple<ClusterShape, ClusterDirection>(ClusterShape.CornerSouthEast, ClusterDirection.West), NotConnectsEast },

        //CornerNorthWest
        { new Tuple<ClusterShape, ClusterDirection>(ClusterShape.CornerNorthWest, ClusterDirection.North), ConnectsSouth },
        { new Tuple<ClusterShape, ClusterDirection>(ClusterShape.CornerNorthWest, ClusterDirection.South), NotConnectsNorth },
        { new Tuple<ClusterShape, ClusterDirection>(ClusterShape.CornerNorthWest, ClusterDirection.East), NotConnectsWest },
        { new Tuple<ClusterShape, ClusterDirection>(ClusterShape.CornerNorthWest, ClusterDirection.West), ConnectsEast },

        //CornerSouthWest
        { new Tuple<ClusterShape, ClusterDirection>(ClusterShape.CornerSouthWest, ClusterDirection.North), NotConnectsSouth },
        { new Tuple<ClusterShape, ClusterDirection>(ClusterShape.CornerSouthWest, ClusterDirection.South), ConnectsNorth },
        { new Tuple<ClusterShape, ClusterDirection>(ClusterShape.CornerSouthWest, ClusterDirection.East), NotConnectsWest },
        { new Tuple<ClusterShape, ClusterDirection>(ClusterShape.CornerSouthWest, ClusterDirection.West), ConnectsEast },

        //VerticalLine
        { new Tuple<ClusterShape, ClusterDirection>(ClusterShape.VerticalLine, ClusterDirection.North), ConnectsSouth },
        { new Tuple<ClusterShape, ClusterDirection>(ClusterShape.VerticalLine, ClusterDirection.South), ConnectsNorth },
        { new Tuple<ClusterShape, ClusterDirection>(ClusterShape.VerticalLine, ClusterDirection.East), NotConnectsWest },
        { new Tuple<ClusterShape, ClusterDirection>(ClusterShape.VerticalLine, ClusterDirection.West), NotConnectsEast },

        //HorizontalLine
        { new Tuple<ClusterShape, ClusterDirection>(ClusterShape.HorizontalLine, ClusterDirection.North), NotConnectsSouth },
        { new Tuple<ClusterShape, ClusterDirection>(ClusterShape.HorizontalLine, ClusterDirection.South), NotConnectsNorth },
        { new Tuple<ClusterShape, ClusterDirection>(ClusterShape.HorizontalLine, ClusterDirection.East), ConnectsWest },
        { new Tuple<ClusterShape, ClusterDirection>(ClusterShape.HorizontalLine, ClusterDirection.West), ConnectsEast },

        //Empty
        { new Tuple<ClusterShape, ClusterDirection>(ClusterShape.Empty, ClusterDirection.North), NotConnectsSouth },
        { new Tuple<ClusterShape, ClusterDirection>(ClusterShape.Empty, ClusterDirection.South), NotConnectsNorth },
        { new Tuple<ClusterShape, ClusterDirection>(ClusterShape.Empty, ClusterDirection.East), NotConnectsWest },
        { new Tuple<ClusterShape, ClusterDirection>(ClusterShape.Empty, ClusterDirection.West), NotConnectsEast },
    };

   


}

