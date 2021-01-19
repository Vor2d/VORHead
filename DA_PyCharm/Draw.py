import numpy
import math
from PIL import Image
import matplotlib.pyplot as plt
import sys

color = 255 #pixel color
address = "./landc/"    #save file address folder (!Folder needs to exsit before running the program)
post = ".png"   #post file name
noise_range = int(3)    #used to fill the landC


def point_dist(x1,y1,x2,y2):
    deltx = int(abs(x1-x2))
    delty = int(abs(y1-y2))
    return math.sqrt(deltx*deltx + delty*delty)


def cir_gener(r,res_arr,g,diag):
    arr = numpy.zeros((2*r+1,2*r+1))
    cr,cc = r,r #Relative center
    for row in range(0,2*r+1):
        for col in range(0,2*r+1):
            arr[row][col] = point_dist(row,col,cr,cc)
    arr = numpy.abs((arr-r)*1000)
    arr = arr.astype(int)
    minrv = arr.min(axis=1)
    mincv = arr.min(axis=0) #min value calculate
    res_rows,res_cols = res_arr.shape
    rcr,rcc = int(res_rows/2),int(res_cols/2) #True center of the large matrix
    if(diag):
        diag_arr = diag_points(rcr)
        #print(diag_arr)
    for row in range(0,res_rows):
        for col in range(0,res_cols):
            if(not diag and (col > rcc and abs(row-rcr) <= g/2)): #leave gap
                continue
            elif(abs(row - rcr) <= r and abs(col-rcc) <= r):
                if(arr[row-rcr+r][col-rcc+r] == minrv[row-rcr+r]):
                    if(diag and (row < rcr and col > rcc) and point_diag_cal(row,col,diag_arr,g/2)):
                        continue
                    res_arr[row][col] = color
                if(arr[row-rcr+r][col-rcc+r] == mincv[col-rcc+r]):
                    if(diag and (row < rcr and col > rcc) and point_diag_cal(row,col,diag_arr,g/2)):
                        continue
                    res_arr[row][col] = color


def diag_points(center):
    diag_arr = numpy.zeros((center+1,2))
    row,col = center,center
    for i in range(0,center+1):
        diag_arr[i][0] = row
        diag_arr[i][1] = col
        row -= 1
        col += 1
    return diag_arr


def point_diag_cal(x,y,diag_arr,dist):
    size = diag_arr.shape[0]
    minv = int(diag_arr[0][0] * 2 + 1)
    for i in range(0,size):
        temp = point_dist(x,y,diag_arr[i][0],diag_arr[i][1])
        if(minv > temp):
            minv = temp
        else:
            break
    return minv <= dist


#draw landC with radius:
#r(int): radius in pixels, diag(bool): whether to draw diagonal landC.
def landc_draw(r,diag):
    d = 2*r+1
    w = int(d/5)
    g = int(d/5)
    hw = int(w/2)
    print(d,w,g)
    res_arr = numpy.zeros((2*(hw+r)+1,2*(hw+r)+1),dtype = (numpy.uint8))
    for i in range(-hw,hw+1):
        cir_gener(r+i,res_arr,g,diag)
    de_noise(res_arr)
    plt.imshow(res_arr)
    plt.show()
    return res_arr

def de_noise(res_arr):
    rows,cols = res_arr.shape
    for r in range(0,rows):
        for c in range(0,cols):
            if(noise_check(res_arr,r,c,noise_range)):
                res_arr[r][c] = color


def noise_check(res_arr,r,c,count):
    rows,cols = res_arr.shape
    cou = 0
    if(r == 0 or c == 0 or r == rows-1 or c == cols-1):
        return False
    if(res_arr[r][c] == 0):
        if(res_arr[r-1][c] != 0):
            cou += 1
        if(res_arr[r+1][c] != 0):
            cou += 1
        if(res_arr[r][c-1] != 0):
            cou += 1
        if(res_arr[r][c+1] != 0):
            cou += 1
        return (cou >= count)
    return False


#Draw the landC with gap pixels, g(int): gap, diag(bool): draw diagonal landC if true.
def land_draw_g(g,diag):
    r = (g*5-1)/2
    return landc_draw(int(r),diag)

#Save iamge to local file, res_arr(object[]): image array, g(int): gap size, used to rename the file.
def save_image(res_arr,g):
    image = Image.fromarray(res_arr,mode = "L")
    image.save(address+str(g)+post)


#Change your parameters here:
#i(int): gap size (!Gaps need to be odd numbers!)
for i in [1,3,5,7,9]:
    res_arr = land_draw_g(i,False)
    save_image(res_arr,i)






