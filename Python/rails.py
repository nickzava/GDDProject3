from PIL import Image
import os, sys
import math

size = (400,400)
baseHeight = 127
railHeight = 110
im = 0
raw = 0

def setRange(width, height, xOffset, yOffset, valueFunction, valueParams):
    val = -1
    xMax = (im.size[0], xOffset + width)[im.size[0] > xOffset + width]
    yMax = (im.size[1], yOffset + height)[im.size[1] > yOffset + height]
    xMin = (0,xOffset)[xOffset > 0]
    yMin = (0,yOffset)[yOffset > 0]
    for x in range(xMin, xMax):
        for y in range(yMin, yMax):
            val = valueFunction(x-xOffset,y-yOffset,raw[x,y],valueParams)
            if (type(val) is float or type(val) is int)  and val != -1:
                raw[x,y] = math.floor(val);
            elif type(val) is tuple:
                raw[x,y] = val;

def drawColor(x,y, params):
    return params[0]

#rail vals
railWidth = math.floor(.05 * size[0])
railChannelWidth = math.floor(.12 *size[0])
railChannelSpacing = .5 * size[0]

railOffset = (railChannelWidth - railWidth)/2
railChannelCenter = math.floor((size[0]-railChannelSpacing)/2)

#percentage of the rail that is curved
railCurvePercent = .5
#percentage of the channel that is curved
channelCurvePercent = .15
railStartPercentage = railOffset/railChannelWidth
railTotalPercent = .5 - railStartPercentage

#percentage measured from the start of the channel to the end
def railDepth(percentage, current):
    if percentage > .5:
        percentage = 1-percentage
    if percentage <= channelCurvePercent:
        return min(math.cos(percentage/channelCurvePercent *math.pi/2) * baseHeight,current)
    if percentage >= railStartPercentage:
        percentIntoRail = (percentage - railStartPercentage) / railTotalPercent
        if percentIntoRail <= railCurvePercent:
            return min(math.sin(percentIntoRail/railCurvePercent *math.pi/2) * railHeight,current)
        return min(railHeight,current)
    return 0


#### normal maps ####


def straightRailVert(x,y,current,params):
    return railDepth(x/railChannelWidth,current)

def straightRailHorz(x,y,current,params):
    return railDepth(y/railChannelWidth,current)

#params (pivotX,pivotY,channelInnerRad)
def curvedRail(x,y,current,params):
    #early return for linear distance
    if abs(x-params[0]) > params[2]+railChannelWidth or abs(y-params[1]) > params[2]+railChannelWidth:
        return -1
    dist = math.sqrt(math.pow(x-params[0],2)+math.pow(y-params[1],2))
    percentage = (dist - params[2])/railChannelWidth
    if percentage >= 0 and percentage <=1:
        return railDepth(percentage,current)
    return -1

#curve vals
areaSize = (railChannelSpacing + railChannelWidth) * 1.5
areaSize = math.floor(areaSize)
areaOffset = (size[0] - areaSize)/2
areaOffset = math.floor(areaOffset)
innerRad = areaSize/2 - railChannelSpacing/2 - railChannelWidth/2
innerRad = math.floor(innerRad)
outerRad = innerRad + railChannelSpacing
outerRad = math.floor(outerRad)
shortRailLength = math.floor(size[0]-areaSize-areaOffset)


#i
im = Image.new("L", size, (baseHeight))
raw = im.load()
setRange(railChannelWidth, size[0],math.floor(railChannelCenter-railChannelWidth/2),0,straightRailVert,())
setRange(railChannelWidth, size[0],math.floor(size[0]-railChannelCenter-railChannelWidth/2),0,straightRailVert,()) 
im.save("I-HMap.png","PNG")

#cross
im = Image.new("L", size, (baseHeight))
raw = im.load()

setRange(railChannelWidth, size[0],math.floor(railChannelCenter-railChannelWidth/2),0,straightRailVert,())
setRange(railChannelWidth, size[0],math.floor(size[0]-railChannelCenter-railChannelWidth/2),0,straightRailVert,())
setRange(size[0],railChannelWidth,0,math.floor(railChannelCenter-railChannelWidth/2),straightRailHorz,())
setRange(size[0],railChannelWidth,0,math.floor(size[0]-railChannelCenter-railChannelWidth/2),straightRailHorz,())

params = (areaSize,areaSize, innerRad)
setRange(areaSize,areaSize,areaOffset,areaOffset,curvedRail,params)
params = (areaSize,areaSize, outerRad)
setRange(areaSize,areaSize,areaOffset,areaOffset,curvedRail,params)

params = (0,areaSize, innerRad)
setRange(areaSize,areaSize,areaOffset,areaOffset,curvedRail,params)
params = (0,areaSize, outerRad)
setRange(areaSize,areaSize,areaOffset,areaOffset,curvedRail,params)

params = (areaSize,0, innerRad)
setRange(areaSize,areaSize,areaOffset,areaOffset,curvedRail,params)
params = (areaSize,0, outerRad)
setRange(areaSize,areaSize,areaOffset,areaOffset,curvedRail,params)

params = (0,0, innerRad)
setRange(areaSize,areaSize,areaOffset,areaOffset,curvedRail,params)
params = (0,0, outerRad)
setRange(areaSize,areaSize,areaOffset,areaOffset,curvedRail,params)

im.save("Cross-HMap.png","PNG")

#l
im = Image.new("L", size, (baseHeight))
raw = im.load()

setRange(railChannelWidth, shortRailLength,math.floor(railChannelCenter-railChannelWidth/2),size[0] - shortRailLength,straightRailVert,())
setRange(railChannelWidth, shortRailLength,math.floor(size[0]-railChannelCenter-railChannelWidth/2),size[0] - shortRailLength,straightRailVert,())
setRange(shortRailLength,railChannelWidth,size[0] - shortRailLength,math.floor(railChannelCenter-railChannelWidth/2),straightRailHorz,())
setRange(shortRailLength,railChannelWidth,size[0] - shortRailLength,math.floor(size[0]-railChannelCenter-railChannelWidth/2),straightRailHorz,())

params = (areaSize,areaSize, innerRad)
setRange(areaSize,areaSize,areaOffset,areaOffset,curvedRail,params)
params = (areaSize,areaSize,outerRad)
setRange(areaSize,areaSize,areaOffset,areaOffset,curvedRail,params)

im.save("L-HMap.png","PNG")

#t

im = Image.new("L", size, (baseHeight))
raw = im.load()

setRange(railChannelWidth, size[0],math.floor(railChannelCenter-railChannelWidth/2),0,straightRailVert,())
setRange(railChannelWidth, size[0],math.floor(size[0]-railChannelCenter-railChannelWidth/2),0,straightRailVert,()) 
setRange(shortRailLength,railChannelWidth,size[0] - shortRailLength,math.floor(railChannelCenter-railChannelWidth/2),straightRailHorz,())
setRange(shortRailLength,railChannelWidth,size[0] - shortRailLength,math.floor(size[0]-railChannelCenter-railChannelWidth/2),straightRailHorz,())

params = (areaSize,areaSize, innerRad)
setRange(areaSize,areaSize,areaOffset,areaOffset,curvedRail,params)
params = (areaSize,areaSize, outerRad)
setRange(areaSize,areaSize,areaOffset,areaOffset,curvedRail,params)

params = (areaSize,0, innerRad)
setRange(areaSize,areaSize,areaOffset,areaOffset,curvedRail,params)
params = (areaSize,0, outerRad)
setRange(areaSize,areaSize,areaOffset,areaOffset,curvedRail,params)

im.save("T-HMap.png","PNG")

#### Color ####

concColor = (176,178,184)
darkConcColor = (121,124,134)
railCenterColor = (29,51,62)

#used for all rail drawing
def colorMin(c1,c2):
    if c1[0] == darkConcColor[0] or c2[0] == darkConcColor[0]:
        return darkConcColor
    if c1[0] == railCenterColor[0] or c2[0] == railCenterColor[0]:
        return railCenterColor
    return concColor

def railColor(percentage, current):
    if percentage > .5:
        percentage = 1-percentage
    if percentage <= channelCurvePercent:
        return colorMin(concColor,current)
    if percentage >= railStartPercentage:
        return colorMin(railCenterColor,current)
    return darkConcColor

def colorStraightRailVert(x,y,current,params):
    return railColor(x/railChannelWidth,current)

def colorStraightRailHorz(x,y,current,params):
    return railColor(y/railChannelWidth,current)

def colorScurvedRail(x,y,current,params):
    #early return for linear distance
    if abs(x-params[0]) > params[2]+railChannelWidth or abs(y-params[1]) > params[2]+railChannelWidth:
        return colorMin(current, concColor)
    dist = math.sqrt(math.pow(x-params[0],2)+math.pow(y-params[1],2))
    percentage = (dist - params[2])/railChannelWidth
    if percentage >= 0 and percentage <=1:
        return railColor(percentage,current)
    return colorMin(current, concColor)

#i
im = Image.new('RGB', size, concColor)
raw = im.load()
setRange(railChannelWidth, size[0],math.floor(railChannelCenter-railChannelWidth/2),0,colorStraightRailVert,())
setRange(railChannelWidth, size[0],math.floor(size[0]-railChannelCenter-railChannelWidth/2),0,colorStraightRailVert,()) 
im.save("I-Color.png","PNG")

#t
im = Image.new("RGB", size, concColor)
raw = im.load()

setRange(railChannelWidth, size[0],math.floor(railChannelCenter-railChannelWidth/2),0,colorStraightRailVert,())
setRange(railChannelWidth, size[0],math.floor(size[0]-railChannelCenter-railChannelWidth/2),0,colorStraightRailVert,())
setRange(size[0],railChannelWidth,0,math.floor(railChannelCenter-railChannelWidth/2),colorStraightRailHorz,())
setRange(size[0],railChannelWidth,0,math.floor(size[0]-railChannelCenter-railChannelWidth/2),colorStraightRailHorz,())

params = (areaSize,areaSize, innerRad)
setRange(areaSize,areaSize,areaOffset,areaOffset,colorScurvedRail,params)
params = (areaSize,areaSize, outerRad)
setRange(areaSize,areaSize,areaOffset,areaOffset,colorScurvedRail,params)

params = (0,areaSize, innerRad)
setRange(areaSize,areaSize,areaOffset,areaOffset,colorScurvedRail,params)
params = (0,areaSize, outerRad)
setRange(areaSize,areaSize,areaOffset,areaOffset,colorScurvedRail,params)

params = (areaSize,0, innerRad)
setRange(areaSize,areaSize,areaOffset,areaOffset,colorScurvedRail,params)
params = (areaSize,0, outerRad)
setRange(areaSize,areaSize,areaOffset,areaOffset,colorScurvedRail,params)

params = (0,0, innerRad)
setRange(areaSize,areaSize,areaOffset,areaOffset,colorScurvedRail,params)
params = (0,0, outerRad)
setRange(areaSize,areaSize,areaOffset,areaOffset,colorScurvedRail,params)

im.save("Cross-Color.png","PNG")

#l
im = Image.new("RGB", size, concColor)
raw = im.load()

setRange(railChannelWidth, shortRailLength,math.floor(railChannelCenter-railChannelWidth/2),size[0] - shortRailLength,colorStraightRailVert,())
setRange(railChannelWidth, shortRailLength,math.floor(size[0]-railChannelCenter-railChannelWidth/2),size[0] - shortRailLength,colorStraightRailVert,())
setRange(shortRailLength,railChannelWidth,size[0] - shortRailLength,math.floor(railChannelCenter-railChannelWidth/2),colorStraightRailHorz,())
setRange(shortRailLength,railChannelWidth,size[0] - shortRailLength,math.floor(size[0]-railChannelCenter-railChannelWidth/2),colorStraightRailHorz,())

params = (areaSize,areaSize, innerRad)
setRange(areaSize,areaSize,areaOffset,areaOffset,colorScurvedRail,params)
params = (areaSize,areaSize,outerRad)
setRange(areaSize,areaSize,areaOffset,areaOffset,colorScurvedRail,params)

im.save("L-Color.png","PNG")

#cross

im = Image.new("RGB", size, concColor)
raw = im.load()

setRange(railChannelWidth, size[0],math.floor(railChannelCenter-railChannelWidth/2),0,colorStraightRailVert,())
setRange(railChannelWidth, size[0],math.floor(size[0]-railChannelCenter-railChannelWidth/2),0,colorStraightRailVert,()) 
setRange(shortRailLength,railChannelWidth,size[0] - shortRailLength,math.floor(railChannelCenter-railChannelWidth/2),colorStraightRailHorz,())
setRange(shortRailLength,railChannelWidth,size[0] - shortRailLength,math.floor(size[0]-railChannelCenter-railChannelWidth/2),colorStraightRailHorz,())

params = (areaSize,areaSize, innerRad)
setRange(areaSize,areaSize,areaOffset,areaOffset,colorScurvedRail,params)
params = (areaSize,areaSize, outerRad)
setRange(areaSize,areaSize,areaOffset,areaOffset,colorScurvedRail,params)

params = (areaSize,0, innerRad)
setRange(areaSize,areaSize,areaOffset,areaOffset,colorScurvedRail,params)
params = (areaSize,0, outerRad)
setRange(areaSize,areaSize,areaOffset,areaOffset,colorScurvedRail,params)

im.save("T-Color.png","PNG")






    
