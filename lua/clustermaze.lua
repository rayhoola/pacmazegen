ClusterMaze = {}
ClusterMaze.__index = ClusterMaze

function ClusterMaze:new(dimensionX, dimensionY)
    local instance = setmetatable({}, ClusterMaze)
    instance.width = dimensionX
    instance.height = dimensionY
    instance.grid = {}
    for x = 1, dimensionX do
        instance.grid[x] = {}
        for y = 1, dimensionY do
            instance.grid[x][y] = nil
        end
    end
    return instance
end

function ClusterMaze:GetLength()
    return self.width * self.height
end

function ClusterMaze:SetCluster(coordinate, cluster)
    if self.grid == nil then
        print("Warning: Trying to set cluster on an empty grid")
    end
    self.grid[coordinate.x + 1][coordinate.y + 1] = cluster
end

function ClusterMaze:TryGetCluster(coordinate)
    local outOfBounds = coordinate.x < 0 or coordinate.x >= #self.grid or coordinate.y < 0 or coordinate.y >= #self.grid[1]
    if outOfBounds then
        return nil, false
    end
    return self.grid[coordinate.x + 1][coordinate.y + 1], true
end

function ClusterMaze:CanGetCluster(coordinate)
    local outOfBounds = coordinate.x < 0 or coordinate.x >= #self.grid or coordinate.y < 0 or coordinate.y >= #self.grid[1]
    return not outOfBounds
end

function ClusterMaze:GetLengthByDimension(dimension)
    local length = 0
    if dimension == 0 then
        length = #self.grid
    elseif dimension == 1 then
        length = #self.grid[1]
    else
        print("Warning: Invalid dimension requested")
    end
    return length
end

function ClusterMaze:GetLowestEntropy()
    local retval = {}
    local minEntropy = math.huge

    for x = 1, self.width do
        for y = 1, self.height do
            local cluster = self.grid[x][y]
            if cluster and cluster.Entropy > 1 then
                local entropy = cluster.Entropy
                if entropy < minEntropy then
                    minEntropy = entropy
                    retval = {cluster}
                elseif entropy == minEntropy then
                    table.insert(retval, cluster)
                end
            end
        end
    end

    return retval
end

function ClusterMaze:DestroyMaze()
    self.grid = nil
end
