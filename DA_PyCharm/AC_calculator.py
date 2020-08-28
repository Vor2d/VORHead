#!/usr/bin/env python
# coding: utf-8

# In[5]:


import numpy
import math


# In[21]:


#size_data = numpy.array([1,2,3,4,5,6,7,8,9,10,11,12,13])
size_data = numpy.array(range(1,100))

H_reso = 1920.0 #Horizontal resolution
p_dist = 900.0 #player distance (mm)
s_width = 20.0 #screen width (mm)
DegPArcm = 1.0/60.0
#FeetsPArcm = 20.0


# In[22]:


PPMM = H_reso/s_width #pixels per mm
print(PPMM)
size_hf = size_data/2.0 #half the gap
size_MM = size_hf/PPMM #half gap mm 
ratio = size_MM / p_dist #tan ratio
angle = numpy.degrees(numpy.arctan(ratio)) * 2.0 #gap angle
arcm = angle / DegPArcm #arcminute
#feets = arcm * FeetsPArcm
logMar = numpy.log10(arcm) #logMAR


# In[ ]:





# In[25]:


#print(feets)
#print(logMar)
for i in range(0,99):
    print(logMar[i],i+1)


# In[ ]:




