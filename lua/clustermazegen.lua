require 'Cluster'
require 'Coordinate'
require 'ClusterRules'
require 'ClusterMaze'

ClusterMazeGen = {}
ClusterMazeGen.__index = ClusterMazeGen


function ClusterMazeGen:new(dX, dY)
    local instance = setmetatable({}, ClusterMazeGen)
    print("new GameMazeGen")
    instance.dimX = dX
    instance.dimY = dY
    instance.lowEntropyList = {}
    return instance
end

function ClusterMazeGen:foo()
    print("foo called")
end

function ClusterMazeGen:InitMaze(width, height)
    self.lowEntropyList = {}
    self.dimX = width
    self.dimY = height
    self.maze = ClusterMaze:new(self.dimX, self.dimY)

    for x = 0, self.dimX - 1 do
        for y = 0, self.dimY - 1 do
            local cluster = Cluster:new()
            self.maze:SetCluster(Coordinate:new(x, y), cluster)
            cluster.Location = Coordinate:new(x, y)
        end
    end

    for x = 0, self.dimX - 1 do
        for y = 0, self.dimY - 1 do
            local neighbor
            local cluster
            cluster = self.maze:TryGetCluster(Coordinate:new(x, y))

            neighbor = self.maze:TryGetCluster(Coordinate:new(cluster.Location.x, cluster.Location.y + 1))
            cluster:SetNeighbor(neighbor, ClusterDirection.North)
            neighbor = self.maze:TryGetCluster(Coordinate:new(cluster.Location.x + 1, cluster.Location.y))
            cluster:SetNeighbor(neighbor, ClusterDirection.East)
            neighbor = self.maze:TryGetCluster(Coordinate:new(cluster.Location.x, cluster.Location.y - 1))
            cluster:SetNeighbor(neighbor, ClusterDirection.South)
            neighbor = self.maze:TryGetCluster(Coordinate:new(cluster.Location.x - 1, cluster.Location.y))
            cluster:SetNeighbor(neighbor, ClusterDirection.West)
        end
    end

    self:PlaceExits()
    self:InitializeBoundaryClusters()
    self:UpdateLowEntropyList()

    while self:UpdateLowEntropyList() do
        local idx = math.random(1, #self.lowEntropyList)
        local target = self.lowEntropyList[idx]
        target:SetRandomDefiniteShape()
        self:PropagateConstraints(target)
    end
end

function ClusterMazeGen:PropagateConstraintsRefCountMethod(cluster)
    local stack = {}
    table.insert(stack, cluster)

    while #stack > 0 do
        local targetCluster = table.remove(stack)

        for _, dir in ipairs(ClusterDirection:GetValues()) do
            local neighbor = targetCluster:Neighbor(dir)
            local modified = false

            if neighbor == nil then
                goto continue
            end

            local refCounts = {}
            local refs = 0
            ::continue::
        end
    end
end

function ClusterMazeGen:PropagateConstraints(cluster)
    local stack = {}
    table.insert(stack, cluster)

    while #stack > 0 do
        local targetCluster = table.remove(stack)

        for _, dir in ipairs(ClusterDirection:GetValues()) do
            local neighbor = targetCluster:Neighbor(dir)
            local modified = false

            if neighbor == nil then
                goto continue
            end

            local compatibleShapes = {}
            for _, possibleShape in ipairs(targetCluster.PossibleShapes) do
                local validNeighborShapes = ClusterRules.validNeighbors[Tuple:new(possibleShape, dir)]
                if validNeighborShapes == nil then
                    goto continue
                end

                for _, neighborShape in ipairs(validNeighborShapes) do
                    if not table.contains(compatibleShapes, neighborShape) then
                        table.insert(compatibleShapes, neighborShape)
                    end
                end
            end

            local remainderShapes = {}
            for _, candidateShape in ipairs(ClusterShape:GetValues()) do
                if not table.contains(remainderShapes, candidateShape) then
                    table.insert(remainderShapes, candidateShape)
                end
            end

            for _, currentShape in ipairs(compatibleShapes) do
                if table.contains(remainderShapes, currentShape) then
                    table.remove(remainderShapes, currentShape)
                end
            end

            if neighbor.Entropy > 1 then
                modified = neighbor:ReducePossibleShapes(remainderShapes)
            end

            if modified then
                table.insert(stack, neighbor)
            end
            ::continue::
        end
    end
end

function ClusterMazeGen:UpdateLowEntropyList()
    local isList = false
    local lowest = math.huge
    self.lowEntropyList = {}

    for i = 0, self.dimX - 1 do
        for j = 0, self.dimY - 1 do
            local cluster
            local cluster = self.maze:TryGetCluster(Coordinate:new(i, j))

            if cluster then
                if cluster.Entropy == 1 or cluster.Entropy > lowest then
                    goto continue
                end

                if cluster.Entropy < lowest then
                    lowest = cluster.Entropy
                    self.lowEntropyList = {cluster}
                    isList = true
                elseif cluster.Entropy == lowest then
                    table.insert(self.lowEntropyList, cluster)
                    isList = true
                end
            end
            ::continue::
        end
    end

    return isList
end

function ClusterMazeGen:PlaceExits()
    local exitFlags = math.random(1, 16)

    if bit.band(exitFlags, 1) == 1 then
        local exitCluster
        exitCluster = self.maze:TryGetCluster(Coordinate:new(0, self.dimY / 2))
        if exitCluster then
            exitCluster:SetDefiniteShape(ClusterShape.HorizontalLine)
            self:PropagateConstraints(exitCluster)
        end
    end

    if bit.band(exitFlags, 2) == 2 then
        local exitCluster
        exitCluster = self.maze:TryGetCluster(Coordinate:new(self.dimX - 1, self.dimY / 2))
        if exitCluster then
            exitCluster:SetDefiniteShape(ClusterShape.HorizontalLine)
            self:PropagateConstraints(exitCluster)
        end
    end

    if bit.band(exitFlags, 4) == 4 then
        local exitCluster
        exitCluster =  self.maze:TryGetCluster(Coordinate:new(self.dimX / 2, 0))
        if exitCluster then
            exitCluster:SetDefiniteShape(ClusterShape.VerticalLine)
            self:PropagateConstraints(exitCluster)
        end
    end

    if bit.band(exitFlags, 8) == 8 then
        local exitCluster
        exitCluster =  self.maze:TryGetCluster(Coordinate:new(self.dimX / 2, self.dimY - 1))
        if exitCluster then
            exitCluster:SetDefiniteShape(ClusterShape.VerticalLine)
            self:PropagateConstraints(exitCluster)
        end
    end
end

function ClusterMazeGen:InitializeBoundaryClusters()
    local debugoutput
    local cluster

    cluster = self.maze:TryGetCluster(Coordinate:new(0, 0)) -- bottom left
    cluster:ReducePossibleShapes(ClusterRules.ConnectsWest)
    cluster:ReducePossibleShapes(ClusterRules.ConnectsSouth)
    cluster:ReducePossibleShapes(ClusterRules.Empty)
    self:PropagateConstraints(cluster)

    cluster = self.maze:TryGetCluster(Coordinate:new(0, self.dimY - 1)) -- top left
    cluster:ReducePossibleShapes(ClusterRules.ConnectsNorth)
    cluster:ReducePossibleShapes(ClusterRules.ConnectsWest)
    cluster:ReducePossibleShapes(ClusterRules.Empty)
    self:PropagateConstraints(cluster)

    cluster = self.maze:TryGetCluster(Coordinate:new(self.dimX - 1, 0)) -- bottom right
    cluster:ReducePossibleShapes(ClusterRules.ConnectsSouth)
    cluster:ReducePossibleShapes(ClusterRules.ConnectsEast)
    cluster:ReducePossibleShapes(ClusterRules.Empty)
    self:PropagateConstraints(cluster)

    cluster = self.maze:TryGetCluster(Coordinate:new(self.dimX - 1, self.dimY - 1)) -- top right
    cluster:ReducePossibleShapes(ClusterRules.ConnectsNorth)
    cluster:ReducePossibleShapes(ClusterRules.ConnectsEast)
    cluster:ReducePossibleShapes(ClusterRules.Empty)
    self:PropagateConstraints(cluster)

    for x = 0, self.dimX - 1 do
        for y = 0, self.dimY - 1 do
            local isEdge = (x == 0 or x == self.dimX - 1 or y == 0 or y == self.dimY - 1)
            if isEdge then
                cluster = self.maze:TryGetCluster(Coordinate:new(x, y))

                if cluster == nil then
                    print("Cluster at " .. x .. "," .. y .. " is null")
                    goto continue
                end

                if cluster.Entropy == 1 then
                    goto continue
                end

                if x == 0 and y == 0 then -- bottom left corner
                    cluster:ReducePossibleShapes(ClusterRules.ConnectsWest)
                    cluster:ReducePossibleShapes(ClusterRules.ConnectsSouth)
                    cluster:ReducePossibleShapes(ClusterRules.Empty)
                elseif x == self.dimX - 1 and y == 0 then -- bottom right corner
                    cluster:ReducePossibleShapes(ClusterRules.ConnectsSouth)
                    cluster:ReducePossibleShapes(ClusterRules.ConnectsEast)
                    cluster:ReducePossibleShapes(ClusterRules.Empty)
                elseif x == 0 and y == self.dimY - 1 then -- Top Left corner
                    cluster:ReducePossibleShapes(ClusterRules.ConnectsNorth)
                    cluster:ReducePossibleShapes(ClusterRules.ConnectsWest)
                    cluster:ReducePossibleShapes(ClusterRules.Empty)
                elseif x == self.dimX - 1 and y == self.dimY - 1 then -- top right corner
                    cluster:ReducePossibleShapes(ClusterRules.ConnectsNorth)
                    cluster:ReducePossibleShapes(ClusterRules.ConnectsEast)
                    cluster:ReducePossibleShapes(ClusterRules.Empty)
                elseif x == 0 then -- left edge
                    cluster:ReducePossibleShapes(ClusterRules.ConnectsWest)
                    cluster:ReducePossibleShapes(ClusterRules.Empty)
                elseif x == self.dimX - 1 then -- right edge
                    cluster:ReducePossibleShapes(ClusterRules.ConnectsEast)
                    cluster:ReducePossibleShapes(ClusterRules.Empty)
                elseif y == 0 then -- bottom edge
                    cluster:ReducePossibleShapes(ClusterRules.ConnectsSouth)
                    cluster:ReducePossibleShapes(ClusterRules.Empty)
                elseif y == self.dimY - 1 then -- top edge
                    cluster:ReducePossibleShapes(ClusterRules.ConnectsNorth)
                    cluster:ReducePossibleShapes(ClusterRules.Empty)
                end

                self:PropagateConstraints(cluster)
            end
            ::continue::
        end
    end

    print("After setting boundaries:")

    for x = 0, self.dimX - 1 do
        debugoutput = ""
        for y = 0, self.dimY - 1 do
            local debugcluster
            debugcluster = self.maze:TryGetCluster(Coordinate:new(x, y))
            debugoutput = debugoutput .. debugcluster.Entropy .. " "
        end
        print(debugoutput)
    end
    print("=================")
end

function ClusterMazeGen:CollapseAtLocation(coordinate)
    local cluster
    cluster = self.maze:TryGetCluster(coordinate)
    if cluster ~= nil then
        if cluster.Entropy > 1 then
            self:SetRandomDefiniteShape(cluster)
            -- propagate constraints to neighbors
        else
            -- propagate constraints to neighbors
        end
    else
        print("Cannot collapse at " .. tostring(coordinate) .. " - out of bounds")
    end
end

function ClusterMazeGen:SetRandomDefiniteShape(cluster)
    if cluster == nil then
        print("Cannot set random definite shape for null cluster")
        return
    end

    if cluster.IsDefiniteShapeSet then
        print("Cluster already has definite shape set")
        return
    end

    local shapeidx = math.random(1, #cluster.PossibleShapes)
    cluster:SetDefiniteShape(cluster.PossibleShapes[shapeidx])
end
