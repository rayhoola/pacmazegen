ClusterShape = {
    Plus = "Plus",
    TeeNorth = "TeeNorth",
    TeeSouth = "TeeSouth",
    TeeEast = "TeeEast",
    TeeWest = "TeeWest",
    CornerNorthEast = "CornerNorthEast",
    CornerSouthEast = "CornerSouthEast",
    CornerNorthWest = "CornerNorthWest",
    CornerSouthWest = "CornerSouthWest",
    VerticalLine = "VerticalLine",
    HorizontalLine = "HorizontalLine",
    Empty = "Empty"
}

Cluster = {}
Cluster.__index = Cluster

function Cluster:new()
    local instance = setmetatable({}, Cluster)
    instance.Location = nil
    instance.PossibleShapes = {}
    for _, shape in pairs(ClusterShape) do
        table.insert(instance.PossibleShapes, shape)
    end
    instance.DefiniteShape = nil
    instance.neighbors = {}
    instance.IsDefiniteShapeSet = false
    return instance
end

function Cluster:SetNeighbor(cluster, direction)
    self.neighbors[direction] = cluster
end

function Cluster:Neighbor(direction)
    return self.neighbors[direction]
end

function Cluster:ConstrainDirection(shapes)
    if #self.PossibleShapes == 1 then
        return
    end
    for _, shape in ipairs(shapes) do
        for i = #self.PossibleShapes, 1, -1 do
            if self.PossibleShapes[i] == shape then
                table.remove(self.PossibleShapes, i)
            end
        end
    end
end

function Cluster:SetDefiniteShape(shape)
    self.DefiniteShape = shape
    self.PossibleShapes = {shape}
    self.IsDefiniteShapeSet = true
end

function Cluster:SetPossibleShapes(shapes)
    self.PossibleShapes = {}
    for _, shape in ipairs(shapes) do
        table.insert(self.PossibleShapes, shape)
    end
end

function Cluster:FilterToCompatibleShapes(shapes, dir)
    local isReduced = false
    -- Logic for filtering compatible shapes would go here
    return isReduced
end

function Cluster:ReducePossibleShapes(shapes)
    local isReduced = false
    if shapes then
        for _, shape in ipairs(shapes) do
            for i = #self.PossibleShapes, 1, -1 do
                if self.PossibleShapes[i] == shape then
                    table.remove(self.PossibleShapes, i)
                    isReduced = true
                end
            end
        end
    end
    if #self.PossibleShapes == 1 then
        self:SetDefiniteShape(self.PossibleShapes[1])
    end
    return isReduced
end

function Cluster:SetRandomDefiniteShape()
    local index = math.random(1, #self.PossibleShapes)
    local shape = self.PossibleShapes[index]
    self:SetDefiniteShape(shape)
end

-- returns clusters to north, east, south, west, in that order.
-- nil if no neighbor
