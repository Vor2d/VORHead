import numpy
from matplotlib.pyplot import *
from matplotlib import pyplot


x = [0.0,0.08,0.16,0.24,0.32,0.40]
y1 = []
logMAR = [-0.182, -0.036, 0.072, 0.232, 0.294, 0.397, 0.516, 0.610, 0.709, 0.808, 0.903, 1.016]

def test1():
    y2 = []
    for ind in y1:
        y2.append(logMAR[ind])
    pyplot.plot(x,y2)
    pyplot.show()

#test1()

xgsl = [0.0,0.08,0.16,0.24,0.32,0.40,0.48]
xgsr = [0.0,0.08,0.16,0.24,0.32,0.40,0.48]
ygsl = [5,4,2,1,2,2,2]
ygsr = [2,1,1,1,1,1,1]

def test2():
    y2l = []
    for ind in ygsl:
        y2l.append(logMAR[ind])
    y2r = []
    for ind in ygsr:
        y2r.append(logMAR[ind])
    tpl1, = pyplot.plot(xgsl, y2l, label = "left-turn")
    tpl2, = pyplot.plot(xgsr, y2r, label = "right-turn")
    pyplot.legend(handles = [tpl1,tpl2])
    pyplot.show()

test2()