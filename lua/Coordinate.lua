Coordinate = {}
Coordinate.__index = Coordinate -- Set __index to itself for method lookup

-- Constructor function
function Coordinate:new(x, y)
    local o = {} -- Create a new empty table for the instance
    setmetatable(o, self) -- Set the metatable of the instance to the Coordinate table
    o.x = x or 0 -- Initialize x, default to 0 if not provided
    o.y = y or 0 -- Initialize y, default to 0 if not provided
    return o
end

-- Instance method to get coordinates as a string
function Coordinate:toString()
    return "X: " .. self.x .. ", Y: " .. self.y
end

-- Instance method to move the coordinate
function Coordinate:move(dx, dy)
    self.x = self.x + dx
    self.y = self.y + dy
end