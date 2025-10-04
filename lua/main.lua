require 'ClusterMazeGen'
require 'Coordinate'

function init()
	print ("init here")
	mazegen = ClusterMazeGen:new(10,10)
	coords = Coordinate:new(42,0)
	coords:move(1,2)
	mazegen:foo()
	mazegen:InitMaze(10,10)
end

function love.load()
	
	if arg[#arg] == "vsc_debug" then require("lldebugger").start() end

	init()
	-- Load map file

end


function love.update(dt)
	require("lovebird").update()
end

function love.joystickaxis(joystick, axis, value )
end

function love.joystickpressed(joystick, button)
	print("button pressed"..button)
end

function love.keypressed(key)
end

-- clear color from map png 140c1c
local function drawFn()
--	titleScreen.Draw()
	
  -- <Your drawing logic goes here.>

	local r, g, b = love.math.colorFromBytes(20, 12, 28)
	love.graphics.setBackgroundColor(r, g, b)
	
	love.graphics.clear(r,g,b)

end

function love.draw()
end

