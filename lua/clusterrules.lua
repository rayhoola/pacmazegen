ClusterDirection = {
    North = "North",
    South = "South",
    East = "East",
    West = "West"
}

-- list all tiles by name/id
tileIDs = {
    -- in our current case, it's just archetypes, not specific tiles, but this should be extendable to more specific tiles
}

ClusterRule = {}
ClusterRule.__index = ClusterRule

function ClusterRule:new()
    local instance = {
        north = {},
        east = {},
        south = {},
        west = {}
    }
    setmetatable(instance, ClusterRule)
    return instance
end

ClusterRules = {}

ClusterRules.ConnectsNorth = { "Plus", "TeeNorth", "TeeEast", "TeeWest", "CornerNorthEast", "CornerNorthWest", "VerticalLine" }
ClusterRules.NotConnectsNorth = { "TeeSouth", "CornerSouthEast", "CornerSouthWest", "HorizontalLine", "Empty" }

ClusterRules.ConnectsSouth = { "Plus", "TeeSouth", "TeeEast", "TeeWest", "CornerSouthEast", "CornerSouthWest", "VerticalLine" }
ClusterRules.NotConnectsSouth = { "TeeNorth", "CornerNorthEast", "CornerNorthWest", "HorizontalLine", "Empty" }

ClusterRules.ConnectsEast = { "Plus", "TeeNorth", "TeeSouth", "TeeEast", "CornerNorthEast", "CornerSouthEast", "HorizontalLine" }
ClusterRules.NotConnectsEast = { "TeeWest", "CornerNorthWest", "CornerSouthWest", "VerticalLine", "Empty" }

ClusterRules.ConnectsWest = { "Plus", "TeeNorth", "TeeSouth", "TeeWest", "CornerNorthWest", "CornerSouthWest", "HorizontalLine" }
ClusterRules.NotConnectsWest = { "TeeEast", "CornerNorthEast", "CornerSouthEast", "VerticalLine", "Empty" }

ClusterRules.Empty = { "Empty" }

function ClusterRules.Contains(shapes, target)
    local isFound = false

    for _, shape in ipairs(shapes) do
        if shape == target then
            isFound = true
            break
        end
    end

    return isFound
end

ClusterRules.validNeighbors = {
    -- Plus
    { {"Plus", ClusterDirection.North}, ClusterRules.ConnectsSouth },
    { {"Plus", ClusterDirection.South}, ClusterRules.ConnectsNorth },
    { {"Plus", ClusterDirection.East}, ClusterRules.ConnectsWest },
    { {"Plus", ClusterDirection.West}, ClusterRules.ConnectsEast },

    -- TeeUp
    { {"TeeNorth", ClusterDirection.North}, ClusterRules.ConnectsSouth },
    { {"TeeNorth", ClusterDirection.South}, ClusterRules.NotConnectsNorth },
    { {"TeeNorth", ClusterDirection.East}, ClusterRules.ConnectsWest },
    { {"TeeNorth", ClusterDirection.West}, ClusterRules.ConnectsEast },

    -- TeeDown
    { {"TeeSouth", ClusterDirection.North}, ClusterRules.NotConnectsSouth },
    { {"TeeSouth", ClusterDirection.South}, ClusterRules.ConnectsNorth },
    { {"TeeSouth", ClusterDirection.East}, ClusterRules.ConnectsWest },
    { {"TeeSouth", ClusterDirection.West}, ClusterRules.ConnectsEast },

    -- TeeRight
    { {"TeeEast", ClusterDirection.North}, ClusterRules.ConnectsSouth },
    { {"TeeEast", ClusterDirection.South}, ClusterRules.ConnectsNorth },
    { {"TeeEast", ClusterDirection.East}, ClusterRules.ConnectsWest },
    { {"TeeEast", ClusterDirection.West}, ClusterRules.NotConnectsEast },

    -- TeeLeft
    { {"TeeWest", ClusterDirection.North}, ClusterRules.ConnectsSouth },
    { {"TeeWest", ClusterDirection.South}, ClusterRules.ConnectsNorth },
    { {"TeeWest", ClusterDirection.East}, ClusterRules.NotConnectsWest },
    { {"TeeWest", ClusterDirection.West}, ClusterRules.ConnectsEast },

    -- CornerNorthEast
    { {"CornerNorthEast", ClusterDirection.North}, ClusterRules.ConnectsSouth },
    { {"CornerNorthEast", ClusterDirection.South}, ClusterRules.NotConnectsNorth },
    { {"CornerNorthEast", ClusterDirection.East}, ClusterRules.ConnectsWest },
    { {"CornerNorthEast", ClusterDirection.West}, ClusterRules.NotConnectsEast },

    -- CornerSouthEast
    { {"CornerSouthEast", ClusterDirection.North}, ClusterRules.NotConnectsSouth },
    { {"CornerSouthEast", ClusterDirection.South}, ClusterRules.ConnectsNorth },
    { {"CornerSouthEast", ClusterDirection.East}, ClusterRules.ConnectsWest },
    { {"CornerSouthEast", ClusterDirection.West}, ClusterRules.NotConnectsEast },

    -- CornerNorthWest
    { {"CornerNorthWest", ClusterDirection.North}, ClusterRules.ConnectsSouth },
    { {"CornerNorthWest", ClusterDirection.South}, ClusterRules.NotConnectsNorth },
    { {"CornerNorthWest", ClusterDirection.East}, ClusterRules.NotConnectsWest },
    { {"CornerNorthWest", ClusterDirection.West}, ClusterRules.ConnectsEast },

    -- CornerSouthWest
    { {"CornerSouthWest", ClusterDirection.North}, ClusterRules.NotConnectsSouth },
    { {"CornerSouthWest", ClusterDirection.South}, ClusterRules.ConnectsNorth },
    { {"CornerSouthWest", ClusterDirection.East}, ClusterRules.NotConnectsWest },
    { {"CornerSouthWest", ClusterDirection.West}, ClusterRules.ConnectsEast },

    -- VerticalLine
    { {"VerticalLine", ClusterDirection.North}, ClusterRules.ConnectsSouth },
    { {"VerticalLine", ClusterDirection.South}, ClusterRules.ConnectsNorth },
    { {"VerticalLine", ClusterDirection.East}, ClusterRules.NotConnectsWest },
    { {"VerticalLine", ClusterDirection.West}, ClusterRules.NotConnectsEast },

    -- HorizontalLine
    { {"HorizontalLine", ClusterDirection.North}, ClusterRules.NotConnectsSouth },
    { {"HorizontalLine", ClusterDirection.South}, ClusterRules.NotConnectsNorth },
    { {"HorizontalLine", ClusterDirection.East}, ClusterRules.ConnectsWest },
    { {"HorizontalLine", ClusterDirection.West}, ClusterRules.ConnectsEast },

    -- Empty
    { {"Empty", ClusterDirection.North}, ClusterRules.NotConnectsSouth },
    { {"Empty", ClusterDirection.South}, ClusterRules.NotConnectsNorth },
    { {"Empty", ClusterDirection.East}, ClusterRules.NotConnectsWest },
    { {"Empty", ClusterDirection.West}, ClusterRules.NotConnectsEast },

}
