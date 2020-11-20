from PIL import Image as im
import os, sys
import math

dH = {
    "T" : im.open("T-HMap.png"),
    "L" : im.open("L-HMap.png"),
    "Cross" :im.open("Cross-HMap.png"),
    "I" :im.open("I-HMap.png")
}

dC = {
    "T" : im.open("T-Color.png"),
    "L" : im.open("L-Color.png"),
    "Cross":im.open("Cross-Color.png"),
    "I": im.open("I-Color.png")
}

##t im.open("T-HMap.png")
##l = im.open("L-HMap.png")
##cross = im.open("Cross-HMap.png")
##i = im.open("I-HMap.png")
##tC = im.open("T-Color.png")
##lC = im.open("L-Color.png")
##crossC = im.open("Cross-Color.png")
##iC = im.open("I-Color.png")


for r in range(1,4):
    rotation = r * 90
    for key in dH.keys():
        dH[key] = dH[key].rotate(90)
        dH[key].save(key + str(rotation) + "HM.png","PNG")
    for key in dC.keys():
        dC[key] = dC[key].rotate(90)
        dC[key].save(key + str(rotation) + "Main.png","PNG")
    

