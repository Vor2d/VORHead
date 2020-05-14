#!/usr/bin/env python
# coding: utf-8

# In[1]:

from matplotlib.pyplot import *
from matplotlib import pyplot
import math
from scipy.optimize import *
import numpy
import os
import datetime
from scipy.io import loadmat


# In[2]:

pyplot.rcParams["figure.figsize"] = (6,6)
pyplot.rcParams["figure.dpi"] = 300
pyplot.rcParams["font.size"] = 8


# In[3]:

xlab1 = "Optotype logMAR"
xlab2 = "Time (seconds) from start of head-turn"
xlab3 = "Time (seconds) from head completely stops"
xlab4 = "Simulink samples"
ylab1 = "Acuity correctness percentage"
ylab2 = "Orientation (degrees)"
ylab3 = "Speed (degrees/second)"


# In[4]:

maxfitv = 80000
#Percentage plot limit
C_limit1 = -0.1
C_limit2 = 1.1
#Wide data trial range
eye_left = -800
eye_right = 500
#Narrow data trial range
S_eye_left = -200
S_eye_right = 800
#Wide data trial speed threshold
speed_TH = 40
#Subplot info
subplot_col_num = 5
#Subplots for all trials
subplot_row_width = 3
#Subplots for mean plot
SP_row_width2 = 5
#Head turn start speed threshold
start_TH = 30
#Simulink-time ratio
SS_Ratio = 960.0
SM_range = 3000
SM_inter_range = 500
SD_range = 77 #stop_delay range (SS)
logistic_base = 0.125
fit_precise = 0.001
AC_threshold = (1.0 + 0.125) / 2.0
DL_threshold = 7.0 / 9.0
quant_low = 0.25
quant_high = 0.75
logMAR = [-0.16939196,0.3077292,0.52957791,0.52957791,0.67570581,0.67570581,0.7848501,0.7848501,           0.87200005,0.87200005,0.94455045,0.94455045]


# In[5]:

def sigmoid3(x,a,b,c):
	return c / (1.0 + np.exp(-a * (x - b))) + logistic_base


# $$f(x) = \frac {L}{1+e^{-k(x-x_0)}} + Base$$

# In[6]:

def write_to_file(path,restr):
	file = open(path + ".txt","w")
	file.write(restr)
	file.close


# In[7]:

file_dir = "./data/"
currentDT = datetime.datetime.now()
resdata = "./resdata/" + currentDT.strftime("%Y_%m_%d_%H_%M_%S") + "/"
SavePath = "./results/" + currentDT.strftime("%Y_%m_%d_%H_%M_%S") + "/"
os.mkdir(SavePath)
os.mkdir(resdata)


# In[8]:


#Subject index
S_index = -1
#All sections and mat collection
StaticSections = []
DynamicSections = []
DY_delaySections = []
GA_delaySections = []
DY_EYSections = []
GA_EYSections = []
MatDatas = []


# In[9]:

class Section:
	
	def __init__(self,mode):
		self.sub_index = 0
		self.mode = mode #Section name
		self.AZ_RW_dir = [] #arr[arr[Acuity Size, right-wrong, direction, sample]]
		self.delay_RW = [] #arr[arr[Delay time, right-wrong, sample]]
		self.Ldelay_RW = []
		self.Rdelay_RW = []
		self.SSSS_RW_sam = [] #arr[arr[start_sample,stop_sample,delay_time,right-wrong,sample]]
		self.LSSSS_RW_sam = []
		self.RSSSS_RW_sam = []
		self.move_target = []
		self.AZ_percent = {} #dict{Acuity size : percentage}
		self.delay_percent = {} #dict{Delay time : percentage}
		self.eye_samples = []
		self.Ldelay_percent = {}
		self.Rdelay_percent = {}
		self.Ldelay_counter = {}
		self.Rdelay_counter = {}
		self.SD_percent = {} #dict{stop_delay_time : arr[right,total,percentage]}
		self.LSD_percent = {}
		self.RSD_percent = {}
	
	#Calculate acuity percentage
	def cal_AC(self):
		AZ_RI_TO = {} #dict{size:arr[right,total]}
		for ARD in self.AZ_RW_dir:
			if(logMAR[ARD[0]] in AZ_RI_TO):
				AZ_RI_TO[logMAR[ARD[0]]][1] += 1
			else:
				AZ_RI_TO[logMAR[ARD[0]]] = [0,1]
			if(ARD[1] == 1):
				AZ_RI_TO[logMAR[ARD[0]]][0] += 1
		for ART in AZ_RI_TO.keys():
			self.AZ_percent[ART] = float(AZ_RI_TO[ART][0]) / float(AZ_RI_TO[ART][1])
	
	#Calculatge delay percentage
	def cal_delay(self):
		DE_RI_TO = {}
		for DR in self.delay_RW:
			if(DR[0] in DE_RI_TO):
				DE_RI_TO[DR[0]][1] += 1
			else:
				DE_RI_TO[DR[0]] = [0,1]
			if(DR[1] == 1):
				DE_RI_TO[DR[0]][0] += 1
		for DRT in DE_RI_TO.keys():
			self.delay_percent[DRT] = float(DE_RI_TO[DRT][0]) / float(DE_RI_TO[DRT][1])
	
	def _CDD_LR(self,sample,mat_data):
		start_sample = sample - SM_range
		index = start_sample - mat_data.head_init_sample
		direction = -1
		turning_index = 0
		i = index
		while(i < index + SM_range):
			if(abs(mat_data.head_SA_HS[i][1]) > start_TH):
				if(mat_data.head_SA_HS[i][1] > 0):
					direction = 0
				else:
					direction = 1
				turning_index = i
				i += SM_inter_range
			i += 1
		return (direction,turning_index + mat_data.head_init_sample)
	
	def _cal_SD(self,mat_data,DR):
		sample = DR[2]
		end = sample - mat_data.head_init_sample
		i = end
		while(i >= end - SM_range):
			if(abs(mat_data.head_SA_HS[i][1]) > speed_TH):
				stop_sample = i + mat_data.head_init_sample
				return stop_sample
			i -= 1
		return -1
	
	def cal_delay_dir(self,mat_data):
		LDE_RI_TO = {}
		RDE_RI_TO = {}
		for DR in self.delay_RW:
			sample = DR[2]
			stop_sample = self._cal_SD(mat_data,DR)
			LR,T_sample = self._CDD_LR(sample,mat_data)
			self.SSSS_RW_sam.append([T_sample,stop_sample,DR[0],DR[1],DR[2]])
			if(LR == 0):
				self.LSSSS_RW_sam.append([T_sample,stop_sample,DR[0],DR[1],DR[2]])
				self.Ldelay_RW.append(DR)
				if(DR[0] in LDE_RI_TO):
					LDE_RI_TO[DR[0]][1] += 1
				else:
					LDE_RI_TO[DR[0]] = [0,1]
				if(DR[1] == 1):
					LDE_RI_TO[DR[0]][0] += 1
			elif(LR == 1):
				self.RSSSS_RW_sam.append([T_sample,stop_sample,DR[0],DR[1],DR[2]])
				self.Rdelay_RW.append(DR)
				if(DR[0] in RDE_RI_TO):
					RDE_RI_TO[DR[0]][1] += 1
				else:
					RDE_RI_TO[DR[0]] = [0,1]
				if(DR[1] == 1):
					RDE_RI_TO[DR[0]][0] += 1
		self.SSSS_RW_sam = numpy.array(self.SSSS_RW_sam)
		self.LSSSS_RW_sam = numpy.array(self.LSSSS_RW_sam)
		self.RSSSS_RW_sam = numpy.array(self.RSSSS_RW_sam)
		for LDRT in LDE_RI_TO.keys():
			self.Ldelay_percent[LDRT] = float(LDE_RI_TO[LDRT][0]) / float(LDE_RI_TO[LDRT][1])
		for RDRT in RDE_RI_TO.keys():
			self.Rdelay_percent[RDRT] = float(RDE_RI_TO[RDRT][0]) / float(RDE_RI_TO[RDRT][1])
		self.Ldelay_counter = LDE_RI_TO
		self.Rdelay_counter = RDE_RI_TO
		
	def _cal_SD_dir(self,source,target):
		DS_time = 0
		for SD in source:
			D_time_S = int(float(SD[2]) * 960.0) + SD[0]
			DS_time_R = D_time_S - SD[1]
			if(DS_time_R > SM_range or DS_time_R < -SM_range):
				continue
			DS_time = int(DS_time_R / SD_range)
			if(DS_time_R > 0):
				DS_time += 0.5
			else:
				DS_time -= 0.5
			if(DS_time in target):
				target[DS_time][1] += 1
			else:
				target[DS_time] = [0,1,0]
			if(SD[3] == 1):
				target[DS_time][0] += 1
		for DS_time in target.keys():
			target[DS_time][2] = float(target[DS_time][0]) / float(target[DS_time][1])
	
	#Calculate and bin all stop delay times
	def cal_SD_dir(self):
		self._cal_SD_dir(self.SSSS_RW_sam,self.SD_percent)
		self._cal_SD_dir(self.LSSSS_RW_sam,self.LSD_percent)
		self._cal_SD_dir(self.RSSSS_RW_sam,self.RSD_percent)
			
		
	def print_DEL_DIR_TO(self):
		print("Left",self.Ldelay_counter)
		print("Right",self.Rdelay_counter)
		
	def test_plot(self,mat_data,start_sample,end_sample):
		x_data = range(start_sample,end_sample)
		start = start_sample - mat_data.head_init_sample
		end = end_sample - mat_data.head_init_sample
		y_data = mat_data.head_SA_HS[start:end,1]
		plot(x_data,y_data)
		#pyplot.show()
		pyplot.clf()
		
	def log_to_file(self,prefix,mode=0):
		#self.AZ_percent
		if(mode == 0):
			restr = "LogMAR\tPercent\n"
			for size in self.AZ_percent:
				restr += str(size) + "\t" + str(self.AZ_percent[size]) + "\n"
		#self.delay_percent
		if(mode == 1):
			restr = "DelayTime\tPercent\n"
			for DT in self.delay_percent:
				restr += str(DT) + "\t" + str(self.delay_percent[DT]) + "\n"
		#self.SD_percent
		if(mode == 2):
			restr = "DelayTimeFromStop\tRight\tTotal\tPercent\n"
			for DTFS in self.SD_percent:
				restr += str(DTFS) + "\t" + str(self.SD_percent[DTFS][0]) + "\t" + str(self.SD_percent[DTFS][1]) + "\t" + str(self.SD_percent[DTFS][2]) + "\n"
		path = resdata + prefix
		write_to_file(path,restr)
		print("!!!!!! ",path)


# In[10]:

class MatData:
	
	def __init__(self):
		self.init_sample = 0 #Very start sample numbet for mat_data
		self.head_init_sample = 0 #Very start sample numbet for head mat_data
		self.SA_HS_LES_RES = [] #arr[arr[Sample number, head speed, left-eye speed, right-eye speed]]
		self.head_SA_HS = [] #head_arr[arr[Sample number, head speed]]
		self.gaze_speed = [] #arr[gaze speed]
		self.eye_index = 0 #0 as left, 1 as right
		self.DYTR_HS_ES_GSs = [] #Dynamic_arr[arr[arr[head speed, eye speed, gaze speed], left-right, trial
								 #index]]
		self.GATR_HS_ES_GSs = [] #Gaze_arr[arr[arr[head speed, eye speed, gaze speed], left-right, trial
								 #index]]
		self.DYS_mean = [] #Dynamic_speed_mean_arr[arr[Head Speed, ES, GS],arr[Head variance, EV, GV]]
		self.GAS_mean = [] #Gaze_speed_mean_arr[arr[Head Speed, ES, GS],arr[Head variance, EV, GV]]
		self.DYLS_mean = [] #Left dynamic_speed_mean_arr[arr[Head Speed, ES, GS],arr[Head variance, EV,
							#GV]]
		self.DYRS_mean = [] #Right dynamic_speed_mean_arr[arr[Head Speed, ES, GS],arr[Head variance, EV,
							#GV]]
		self.GALS_mean = [] #Left gaze_speed_mean_arr[arr[Head Speed, ES, GS],arr[Head variance, EV, GV]]
		self.GARS_mean = [] #Right gaze_speed_mean_arr[arr[Head Speed, ES, GS],arr[Head variance, EV, GV]]
		self.DY_delete_list = [] #DDL_arr[index], trial indexes to delete
		self.GA_delete_list = [] #GDL_arr[index], trial indexes to delete
		self.SA_HP_LEP_REP = [] #arr[arr[Sample number, head position, left-eye position, right-eye position]]
		self.head_SA_HP = [] #head_arr[arr[Sample number, head position]]
		self.gaze_position = [] #arr[gaze position]
		self.DYTR_HP_EP_GPs = [] #Dynamic_arr[arr[arr[head position, eye position, gaze position], left-right,
								 #trial index]]
		self.GATR_HP_EP_GPs = [] #Gaze_arr[arr[arr[head position, eye position, gaze position], left-right,
								 #trial index]]
		self.DYP_mean = [] #Dynamic_position_mean_arr[arr[Head Position, EP, GP],arr[Head variance, EV,
						   #GV]]
		self.GAP_mean = [] #Gaze_position_mean_arr[arr[Head Position, EP, GP],arr[Head variance, EV, GV]]
		self.DYLP_mean = [] #Left dynamic_position_mean_arr[arr[Head Position, EP, GP],arr[Head variance,
							#EV, GV]]
		self.DYRP_mean = [] #Right dynamic_position_mean_arr[arr[Head Position, EP, GP],arr[Head variance,
							#EV, GV]]
		self.GALP_mean = [] #Left gazeshift_position_mean_arr[arr[Head Position, EP, GP],arr[Head
							#variance, EV, GV]]
		self.GARP_mean = [] #Right gazeshift_position_mean_arr[arr[Head Position, EP, GP],arr[Head
							#variance, EV, GV]]
		self.DY_mean_start = 0 #Dynamic head start point
		self.GA_mean_start = 0 #Gaze head start point
		self.DYL_mean_start = 0 #Left dynamic head start point
		self.GAL_mean_start = 0 #Right gaze head start point
		self.DYR_mean_start = 0 #Left dynamic head start point
		self.GAR_mean_start = 0 #Right gaze head start point
	
	#Calculate gaze data
	def gaze_cal(self):
		self.gaze_speed = self.SA_HS_LES_RES[:,self.eye_index + 2] + self.SA_HS_LES_RES[:,1]
		self.gaze_position = self.SA_HP_LEP_REP[:,self.eye_index + 2] + self.SA_HP_LEP_REP[:,1]

	#Get each individual trials
	def get_eye_trials(self,section,sectionN=0):
		trial_index = 0
		for sample in section.eye_samples:
			left = 0
			index = sample - self.init_sample
			start = int(index + eye_left)
			end = int(index + eye_right)
			T_index = -1
			invert = False
			for i in range(start,end):
				if(abs(self.SA_HS_LES_RES[i,1]) > speed_TH):
					if(self.SA_HS_LES_RES[i,1] < 0):
						invert = True
						left = 1
					T_index = i
					break
			T_start = T_index + S_eye_left
			T_end = T_index + S_eye_right

			matrix = numpy.append([self.SA_HS_LES_RES[int(T_start):int(T_end),1]],                        [self.SA_HS_LES_RES[int(T_start):int(T_end),self.eye_index + 2]],                        axis=0)
			matrix = numpy.append(matrix,[self.gaze_speed[int(T_start):int(T_end)]],axis=0)
			matrix = matrix.transpose()
			if(invert):
				matrix *= -1
			if(sectionN == 0):
				self.DYTR_HS_ES_GSs.append([matrix,left,trial_index])
			elif(sectionN == 1):
				self.GATR_HS_ES_GSs.append([matrix,left,trial_index])
				
			matrix2 = numpy.append([self.SA_HP_LEP_REP[int(T_start):int(T_end),1]],                        [self.SA_HP_LEP_REP[int(T_start):int(T_end),self.eye_index + 2]],                        axis=0)
			matrix2 = numpy.append(matrix2,[self.gaze_position[int(T_start):int(T_end)]],axis=0)
			matrix2 = matrix2.transpose()
			if(invert):
				matrix2 *= -1
			if(sectionN == 0):
				self.DYTR_HP_EP_GPs.append([matrix2,left,trial_index])
			elif(sectionN == 1):
				self.GATR_HP_EP_GPs.append([matrix2,left,trial_index])
			trial_index += 1
			
	#Private fun, calculate mean
	def _self_mean_cal(self,source):        
		left_flag = False
		sample_num = len(source[0][0])
		trial_num = len(source)
		sum_head = numpy.zeros((sample_num,1))
		sum_eye = numpy.zeros((sample_num,1))
		sum_gaze = numpy.zeros((sample_num,1))
		Lsum_head = numpy.zeros((sample_num,1))
		Lsum_eye = numpy.zeros((sample_num,1))
		Lsum_gaze = numpy.zeros((sample_num,1))
		Rsum_head = numpy.zeros((sample_num,1))
		Rsum_eye = numpy.zeros((sample_num,1))
		Rsum_gaze = numpy.zeros((sample_num,1))
		
		for i in range(0,len(source)):
			trial = source[i,0]
			sum_head += numpy.transpose([trial[:,0]])
			sum_eye += numpy.transpose([trial[:,1]])
			sum_gaze += numpy.transpose([trial[:,2]])
			if(source[i,1] == 0):
				left_flag = True
				Lsum_head += numpy.transpose([trial[:,0]])
				Lsum_eye += numpy.transpose([trial[:,1]])
				Lsum_gaze += numpy.transpose([trial[:,2]])
			else:
				left_flag = False
				Rsum_head += numpy.transpose([trial[:,0]])
				Rsum_eye += numpy.transpose([trial[:,1]])
				Rsum_gaze += numpy.transpose([trial[:,2]])
		
		sum_3 = numpy.column_stack((sum_head,sum_eye,sum_gaze))
		Lsum_3 = numpy.column_stack((Lsum_head,Lsum_eye,Lsum_gaze))
		Rsum_3 = numpy.column_stack((Rsum_head,Rsum_eye,Rsum_gaze))
		
		mean3 = sum_3 / float(trial_num)
		Lmean3 = Lsum_3 / float(trial_num)
		Rmean3 = Rsum_3 / float(trial_num)
		
		square_sum3 = numpy.zeros((sample_num,3))
		Lsquare_sum3 = numpy.zeros((sample_num,3))
		Rsquare_sum3 = numpy.zeros((sample_num,3))
		
		for i in range(0,len(source)):
			square_sum3 += numpy.power(numpy.absolute(mean3 - source[i,0]),2)
			if(source[i,1] == 0):
				Lsquare_sum3 += numpy.power(numpy.absolute(Lmean3 - source[i,0]),2)
			else:
				Rsquare_sum3 += numpy.power(numpy.absolute(Rmean3 - source[i,0]),2)
		variance3 = square_sum3 / float(trial_num)
		Lvariance3 = Lsquare_sum3 / float(trial_num)
		Rvariance3 = Rsquare_sum3 / float(trial_num)
		
		return ([mean3,variance3],[Lmean3,Lvariance3],[Rmean3,Rsquare_sum3])
	
	#Private fun, calculate start point
	def _mean_start_cal(self,speed_arr):
		index = 0
		var = -1
		for speed in speed_arr:
			if(speed[0] > start_TH):
				var = index
				break
			index += 1
		return var
	
	#Mean calculation for all situations
	def mean_cal(self):
		self.DYS_mean,self.DYLS_mean,self.DYRS_mean = self._self_mean_cal(self.DYTR_HS_ES_GSs)
		self.GAS_mean,self.GALS_mean,self.GARS_mean = self._self_mean_cal(self.GATR_HS_ES_GSs)
		self.DYP_mean,self.DYLP_mean,self.DYRP_mean = self._self_mean_cal(self.DYTR_HP_EP_GPs)
		self.GAP_mean,self.GALP_mean,self.GARP_mean = self._self_mean_cal(self.GATR_HP_EP_GPs)
		
		self.DY_mean_start = self._mean_start_cal(self.DYS_mean[0])
		self.GA_mean_start = self._mean_start_cal(self.GAS_mean[0])
		self.DYL_mean_start = self._mean_start_cal(self.DYLS_mean[0])
		self.GAL_mean_start = self._mean_start_cal(self.GALS_mean[0])
		self.DYR_mean_start = self._mean_start_cal(self.DYRS_mean[0])
		self.GAR_mean_start = self._mean_start_cal(self.GARS_mean[0])
	
	#Private fun, drop out bad trials
	def _self_drop_out(self,source,target):
		target = numpy.delete(target,source,0)
		return target
	
	#Drop out bad trials
	def drop_out(self):
		self.DYTR_HS_ES_GSs = self._self_drop_out(self.DY_delete_list,self.DYTR_HS_ES_GSs)
		self.GATR_HS_ES_GSs = self._self_drop_out(self.GA_delete_list,self.GATR_HS_ES_GSs)
		self.DYTR_HP_EP_GPs = self._self_drop_out(self.DY_delete_list,self.DYTR_HP_EP_GPs)
		self.GATR_HP_EP_GPs = self._self_drop_out(self.GA_delete_list,self.GATR_HP_EP_GPs)


# In[11]:


#Read section files(Acuity file name, Delay file name, Section number in the
#file)
def read_file(AF_name, DF_name,sectionN=0):
	try:
		file = open(file_dir + AF_name,"r")
		lines = file.readlines()
	except:
		lines = []
	
	section = Section("")
	
	start_flag = False
	S_index = 0
	last_delay = 0.0
	sample = 0
	for line in lines:
		strings = line.split()
		if(len(strings) < 1):
			continue
		if(strings[1] == "start"):
			if(S_index == sectionN):
				start_flag = True
				section.mode = strings[4]
			else:
				start_flag = False
			S_index += 1
		if(start_flag):
			try:
				AZ = int(strings[3])
			except:
				AZ = -100
			if(AZ >= 0):
				try:
					sample = int(strings[1])
				except:
					sample = -1
				RW = 1 if (strings[4] == "True") else 0
				section.AZ_RW_dir.append((AZ,RW,strings[5],sample))
				section.delay_RW.append((last_delay,RW,sample))
			if(AZ == -2):
				try:
					last_delay = float(strings[4])
				except:
					last_delay = -1.0
				try:
					sample = int(strings[1])
				except:
					sample = -1
				section.eye_samples.append(sample)
	file.close()
	
	try:
		file = open(file_dir + DF_name,"r")
		lines = file.readlines()
	except:
		lines = []
	
	for line in lines:
		strings = line.split()
		if(len(strings) < 1):
			continue
		if(strings[4] == "MoveTarget"):
			try:
				simS = int(strings[2])
				degree = float(strings[5]) if (strings[6] == "1") else -(float(strings[5]))
			except:
				simS = int(-1)
				degree = 0.0
			section.move_target.append((simS,degree))
			
			
	return section


# In[12]:


#MAT read fun(mat file name)
def read_mat(filename,shift):
	MD = MatData()
	file = loadmat(file_dir + filename)
	MD.init_sample = file['sampleNo'][0][0]
	samples = file['sampleNo']
	if(shift != 0):
		samples = samples[:shift]
	MD.SA_HS_LES_RES = numpy.column_stack((samples,file['head'][0][0][1][:,2],file['leftEye'][0][0][1][:,2],file['rightEye'][0][0][1][:,2]))
	MD.SA_HP_LEP_REP = numpy.column_stack((samples,file['head'][0][0][0][:,2],file['leftEye'][0][0][0][:,2],file['rightEye'][0][0][0][:,2]))
	return MD


# In[13]:

def read_head_mat(matdata,filename,shift):
	file = loadmat(file_dir + filename)
	matdata.head_init_sample = file['sampleNo'][0][0]
	samples = file['sampleNo']
	if(shift != 0):
		samples = samples[:shift]
	matdata.head_SA_HS = numpy.column_stack((samples,file['head'][0][0][1][:,2]))
	matdata.head_SA_HP = numpy.column_stack((samples,file['head'][0][0][0][:,2]))
	return matdata


# In[14]:


#Plot percentage plots(x_data, y_data, legends, low_y_limit, high_y_limit)
def plotplot(x_data,y_data,legends,limit1,limit2):
	ylim(limit1, limit2)
	tpl, = plot(x_data,y_data,label = legends)
	return tpl


# In[15]:


#Plot acuity plots(section data,lable by mode/lable by subjects)
def plot_ac(section,lab_mode=0,dots=False):
	x_data = sorted(section.AZ_percent.keys())
	y_data = []
	for x in x_data:
		y_data.append(section.AZ_percent[x])
	if(lab_mode == 0):
		tpl = plotplot(x_data,y_data,section.mode,C_limit1,C_limit2)
	elif(lab_mode == 1):
		tpl = plotplot(x_data,y_data,section.sub_index,C_limit1,C_limit2)
	if(dots):
		pyplot.scatter(x_data,y_data)
	return tpl


# In[16]:


#Plot statc and dynamic acuity plots(static data,dynamic data)
def plot_ST_DY(section1,section2):
	#pyplot.rcParams["figure.figsize"] = (20,10)
	#pyplot.title("Acuities")
	tpl1 = plot_ac(section1)
	tpl2 = plot_ac(section2)
	pyplot.legend(handles=[tpl1, tpl2])
	pyplot.savefig(SavePath + "AcuityResult_S" + str(S_index) + ".png",dpi = 300)
	#pyplot.show()
	pyplot.clf()

# In[17]:

def plot_static_dynamic_acuity(section1,lab=""):
	#pyplot.title(lab)
	tpl1 = plot_ac(section1,dots = True)
	x_data = sorted(list(section1.AZ_percent.keys()))
	line = pyplot.hlines(AC_threshold,x_data[0],x_data[-1],linestyles = 'dashed',label = "Threshold: 0.5625")
	pyplot.legend(handles=[tpl1,line])
	pyplot.xlabel(xlab1)
	pyplot.ylabel(ylab1)
	pyplot.savefig(SavePath + lab + str(S_index) + ".png",dpi = 300)
	#pyplot.show()
	pyplot.clf()

# In[18]:


#Plot delay data(delay data, label by mode/label by subjects)
def plot_delay(section,lab_mode=0):
	x_data = sorted(section.delay_percent.keys())
	y_data = []
	for x in x_data:
		y_data.append(section.delay_percent[x])
	if(lab_mode == 0):
		tpl = plotplot(x_data,y_data,section.mode,C_limit1,C_limit2)
	elif(lab_mode == 1):
		tpl = plotplot(x_data,y_data,section.sub_index,C_limit1,C_limit2)
	return tpl


# In[19]:


#Plot dynamic delay and gaze shift delay plots(dynamic delay data, gaze shift
#data)
def plot_DYD_GAD(section1,section2):
	section2.mode = "GazeShift"
	#pyplot.rcParams["figure.figsize"] = (20,10)
	#pyplot.title("Delays")
	tpl1 = plot_delay(section1)
	tpl2 = plot_delay(section2)
	pyplot.legend(handles=[tpl1, tpl2])
	pyplot.savefig(SavePath + "DelayResult_" + str(S_index) + ".png",dpi = 300)
	#pyplot.show()
	pyplot.clf()

# In[20]:

def plot_delay_dir(section,title):
	#pyplot.rcParams["figure.figsize"] = (20,10)
	#pyplot.title(title)
	
	x_data = sorted(section.delay_percent.keys())
	y_data = []
	for x in x_data:
		y_data.append(section.delay_percent[x])
	tpl0 = plotplot(x_data,y_data,"Total",C_limit1,C_limit2)
	
	x_data = sorted(section.Ldelay_percent.keys())
	y_data = []
	for x in x_data:
		y_data.append(section.Ldelay_percent[x])
	tpl1 = plotplot(x_data,y_data,"Left",C_limit1,C_limit2)
	
	x_data = sorted(section.Rdelay_percent.keys())
	y_data = []
	for x in x_data:
		y_data.append(section.Rdelay_percent[x])
	tpl2 = plotplot(x_data,y_data,"Right",C_limit1,C_limit2)
	
	pyplot.legend(handles=[tpl0,tpl1,tpl2])
	pyplot.xlabel(xlab2)
	pyplot.ylabel(ylab1)
	pyplot.savefig(SavePath + title + ".png",dpi = 300)
	#pyplot.show()
	pyplot.clf()

# In[21]:


#Plot dynamic delay and gaze shift delay with direction plots(dynamic delay
#data, gaze shift data)
def plot_DYD_GAD_dir(section1,section2):
	section1.print_DEL_DIR_TO()
	plot_delay_dir(section1,"Dynamic Delays")
	section2.print_DEL_DIR_TO()
	plot_delay_dir(section2,"GazeShift Delays")


# In[22]:


#Plot single trial speed data(trial index, mat_data)
def plot_eye_single(index, mat_data):
	#pyplot.rcParams["figure.figsize"] = (20,10)
	labels = ["HeadSpeed","EyeSpeed","GazeSpeed"]
	x_data = range(0,S_eye_right - S_eye_left)
	y_data = []
	tpls = []
	start = int(index + eye_left)
	end = int(index + eye_right)
	T_index = -1
	for i in range(start,end):
		if(abs(mat_data.SA_HS_LES_RES[i,1]) > speed_TH):
			T_index = i
			break
	T_start = T_index + S_eye_left
	T_end = T_index + S_eye_right
	
	y_data = mat_data.SA_HS_LES_RES[int(T_start):int(T_end),1]
	tpl, = plot(x_data,y_data,label = labels[0])
	tpls.append(tpl)
	
	y_data = mat_data.SA_HS_LES_RES[int(T_start):int(T_end),mat_data.eye_index + 2]
	tpl, = plot(x_data,y_data,label = labels[1])
	tpls.append(tpl)
	
	y_data = mat_data.gaze_speed[int(T_start):int(T_end)]
	tpl, = plot(x_data,y_data,label = labels[2])
	tpls.append(tpl)
	return tpls


# In[23]:


#Plot single trial speed data(trial data)
def plot_eye_trial(trial):
	#pyplot.rcParams["figure.figsize"] = (20,10)
	labels = ["HeadSpeed","EyeSpeed","GazeSpeed"]
	x_data = range(0,len(trial))
	y_data = []
	tpls = []
	
	y_data = trial[:,0]
	tpl, = plot(x_data,y_data,label = labels[0])
	tpls.append(tpl)
	
	y_data = trial[:,1]
	tpl, = plot(x_data,y_data,label = labels[1])
	tpls.append(tpl)
	
	y_data = trial[:,2]
	tpl, = plot(x_data,y_data,label = labels[2])
	tpls.append(tpl)
	
	return tpls


# In[24]:


#Plot single trial position data(trial data)
def plot_eye_trial_pos(trial):
	#pyplot.rcParams["figure.figsize"] = (20,10)
	labels = ["HeadOrientation","EyeOrientation","GazeOrientation"]
	x_data = range(0,len(trial))
	y_data = []
	tpls = []
	
	y_data = trial[:,0]
	tpl, = plot(x_data,y_data,label = labels[0])
	tpls.append(tpl)
	
	y_data = trial[:,1]
	tpl, = plot(x_data,y_data,label = labels[1])
	tpls.append(tpl)
	
	y_data = trial[:,2]
	tpl, = plot(x_data,y_data,label = labels[2])
	tpls.append(tpl)
	
	return tpls


# In[25]:


#Plot all trials speed data(mat data, 0 for DD/1 for GD)
def plot_eye(mat_data,sectionN=0):
	#pyplot.rcParams["figure.figsize"] = (20,10)
	trials = []
	if(sectionN == 0):
		trials = mat_data.DYTR_HS_ES_GSs
		#pyplot.title("DynamicSpeed")
	if(sectionN == 1):
		trials = mat_data.GATR_HS_ES_GSs
		#pyplot.title("GazeShiftSpeed")
	for trial in trials:
		tpl = plot_eye_trial(trial)
		pyplot.legend(handles = tpl)
		#pyplot.show()
		pyplot.clf()
	return tpl


# In[26]:


#Plot all trials position data(tmat data, 0 for DD/1 for GD)
def plot_eye_pos(mat_data,dir_mode,title = "",sectionN=0):
	#pyplot.rcParams["figure.figsize"] = (20,10)
	trials = []
	trials_select = []
	if(sectionN == 0):
		trials_select = mat_data.DYTR_HP_EP_GPs
		#pyplot.title("DynamicPosition,Direction:" + str(dir_mode))
	if(sectionN == 1):
		trials_select = mat_data.GATR_HP_EP_GPs
		#pyplot.title("GazeShiftPosition,Direction:" + str(dir_mode))
	dir_flag = dir_mode - 1
	if(dir_flag == -1):
		trials = trials_select
	else:
		for piece in trials_select:
			if(piece[1] == dir_flag):
				trials.append(piece)
	index = 0
	for trial in trials:
		print(index)
		tpl = plot_eye_trial_pos(trial[0])
		pyplot.legend(handles = tpl)
		pyplot.xlabel(xlab4)
		pyplot.ylabel(ylab2)
		pyplot.savefig(SavePath + title + str(index) + "_HeadEye" + ".png",dpi = 300)
		#pyplot.show()
		pyplot.clf()
		index += 1


# In[27]:


#Plot mean plot(mean arrays, 0 for data plot/1 for variance plot)
def plot_eye_mean(mean_arrs,mode=0):
	#pyplot.rcParams["figure.figsize"] = (20,10)
	#pyplot.title("MeanPlot")
	mean_arr = mean_arrs[mode]
	tpls = plot_eye_trial_pos(mean_arr)
	pyplot.legend(handles = tpls)
	#pyplot.show()
	pyplot.clf()
	return tpls


# In[28]:


#Plot single subplot position data(plot position, trial data)
def subplotplot_pos(ax,trial):
	labels = ["HeadPosition","EyePosition","GazePosition"]
	x_data = range(0,len(trial))
	y_data = []
	tpls = []
	
	y_data = trial[:,0]
	tpl, = ax.plot(x_data,y_data,label = labels[0])
	tpls.append(tpl)
	
	y_data = trial[:,1]
	tpl, = ax.plot(x_data,y_data,label = labels[1])
	tpls.append(tpl)
	
	y_data = trial[:,2]
	tpl, = ax.plot(x_data,y_data,label = labels[2])
	tpls.append(tpl)
	
	return tpls


# In[29]:


#Plot all subplots position plots(mat data, 0 for DD/1 for GD)
def subplot_eye(mat_data,dir_mode,sectionN=0,title = ""):
	data = []
	data_select = []
	if(sectionN == 0):
		title += "Dynamic_"
		data_select = mat_data.DYTR_HP_EP_GPs
		if(dir_mode == 0):
			print("DynamicAcuity")
		elif(dir_mode == 1):
			print("LeftDynamicAcuity")
		elif(dir_mode == 2):
			print("RightDynamicAcuity")
	elif(sectionN == 1):
		title += "Gazeshift_"
		data_select = mat_data.GATR_HP_EP_GPs
		if(dir_mode == 0):
			print("GazeShift")
		elif(dir_mode == 1):
			print("LeftGazeShift")
		elif(dir_mode == 2):
			print("RightGazeShift")
	dir_flag = dir_mode - 1
	if(dir_flag == -1):
		data = data_select
	else:
		for piece in data_select:
			if(piece[1] == dir_flag):
				data.append(piece)
	row_n = len(data) // subplot_col_num + 1
	#pyplot.rcParams["figure.figsize"] = (20,subplot_row_width * row_n)
	fig, axes = pyplot.subplots(row_n,subplot_col_num,squeeze = False)
	index = 0
	finished = False
	for i in range(0,row_n):
		if(finished):
			break
		for j in range(0,subplot_col_num):
			if(index >= len(data)):
				finished = True
				break
			subplotplot_pos(axes[i,j],data[index][0])
			index += 1
	pyplot.savefig(SavePath + title + "EyeSubplots" + ".png",dpi = 300)
	#pyplot.show()
	pyplot.clf()
	


# In[30]:


#Plot subplots mean plots(mar data, section data, 0 for DD/1 for GD)
def subplot_mean(mat_data,section,sectionN,dir_mode):
	#pyplot.rcParams["figure.figsize"] = (20,4*SP_row_width2)
	start = 0
	mean_arr = []
	mean_arr_speed = []
	mean_var = []
	section_dic = {}
	if(sectionN == 0):
		if(dir_mode == 0):
			print("DynamicMeanPlot")
			start = mat_data.DY_mean_start
			mean_arr = mat_data.DYP_mean[0]
			mean_arr_speed = mat_data.DYS_mean[0]
			mean_var = mat_data.DYP_mean[1]
			section_dic = section.delay_percent
		elif(dir_mode == 1):
			print("LeftDynamicMeanPlot")
			start = mat_data.DYL_mean_start
			mean_arr = mat_data.DYLP_mean[0]
			mean_arr_speed = mat_data.DYLS_mean[0]
			mean_var = mat_data.DYLP_mean[1]
			section_dic = section.Ldelay_percent
		elif(dir_mode == 2):
			print("RightDynamicMeanPlot")
			start = mat_data.DYR_mean_start
			mean_arr = mat_data.DYRP_mean[0]
			mean_arr_speed = mat_data.DYRS_mean[0]
			mean_var = mat_data.DYRP_mean[1]
			section_dic = section.Rdelay_percent
	elif(sectionN == 1):
		if(dir_mode == 0):
			print("GazeShiftMeanPlot")
			start = mat_data.GA_mean_start
			mean_arr = mat_data.GAP_mean[0]
			mean_arr_speed = mat_data.GAS_mean[0]
			mean_var = mat_data.GAP_mean[1]
			section_dic = section.delay_percent
		elif(dir_mode == 1):
			print("LeftGazeShiftMeanPlot")
			start = mat_data.GAL_mean_start
			mean_arr = mat_data.GALP_mean[0]
			mean_arr_speed = mat_data.GALS_mean[0]
			mean_var = mat_data.GALP_mean[1]
			section_dic = section.Ldelay_percent
		elif(dir_mode == 2):
			print("RightGazeShiftMeanPlot")
			start = mat_data.GAR_mean_start
			mean_arr = mat_data.GARP_mean[0]
			mean_arr_speed = mat_data.GARS_mean[0]
			mean_var = mat_data.GARP_mean[1]
			section_dic = section.Rdelay_percent
	arr = []
	arr.append(mean_arr) #mean data position array
	arr.append(mean_arr_speed) #mean data speed array
	arr.append(mean_var) #mean variance array
	
	fig, axes = pyplot.subplots(4,1,squeeze = False,sharex = True)
	
	x_data = sorted(section_dic.keys())
	y_data = []
	for x in x_data:
		y_data.append(section_dic[x])
	ratio_x_data = []
	for x in x_data:
		ratio_x_data.append(x * SS_Ratio + start)
	axes[0,0].plot(ratio_x_data,y_data)
	axes[0,0].set_ylim(C_limit1,C_limit2)
	
	for rx in ratio_x_data:   
		axes[0,0].axvline(x=rx)
	
	for i in range(0,3):
		x_data = range(0,len(arr[i]))
		y_data = arr[i][:,0]
		tpl1, = axes[i + 1,0].plot(x_data,y_data,label = "head")

		y_data = arr[i][:,1]
		tpl2, = axes[i + 1,0].plot(x_data,y_data,label = "eye")

		y_data = arr[i][:,2]
		tpl3, = axes[i + 1,0].plot(x_data,y_data,label = "gaze")
		
		for rx in ratio_x_data:
			axes[i + 1,0].axvline(x=rx)
		axes[i + 1,0].axhline(y=0)
		axes[i + 1,0].legend([tpl1,tpl2,tpl3])
	
	#pyplot.show()
	pyplot.clf()


# In[31]:

def plot_SD_percent(x_data,y_data,labels):
	tpl, = plot(x_data,y_data,label = labels)
	return tpl


# In[32]:

def plot_SD_percent3(section,title,S_index=0):
	#pyplot.rcParams["figure.figsize"] = (20,10)
	ylim(C_limit1, C_limit2)
	tpls = []
	
	y_data = []
	x_data = numpy.asarray(list(section.SD_percent.keys()))
	x_data.sort()
	x_data_S = x_data * SD_range / 1000.0
	for x in x_data:
		y_data.append(section.SD_percent[x][2])
	tpls.append(plot_SD_percent(x_data_S,y_data,"Total"))
	
	y_data = []
	x_data = numpy.asarray(list(section.LSD_percent.keys()))
	x_data.sort()
	x_data_S = x_data * SD_range / 1000.0
	for x in x_data:
		y_data.append(section.LSD_percent[x][2])
	tpls.append(plot_SD_percent(x_data_S,y_data,"Left"))
	
	y_data = []
	x_data = numpy.asarray(list(section.RSD_percent.keys()))
	x_data.sort()
	x_data_S = x_data * SD_range / 1000.0
	for x in x_data:
		y_data.append(section.RSD_percent[x][2])
	tpls.append(plot_SD_percent(x_data_S,y_data,"Right"))
	
	pyplot.xlabel(xlab3)
	pyplot.ylabel(ylab1)
	pyplot.legend(handles = tpls)
	pyplot.savefig(SavePath + title + str(S_index) + ".png",dpi = 300)
	#pyplot.show()
	pyplot.clf()


# In[33]:

def print_CT(section):
	print("Total:\n")
	print(section.SD_percent)
	print("\n")
	
	print("Left:\n")
	print(section.LSD_percent)
	print("\n")
	
	print("Right:\n")
	print(section.RSD_percent)
	print("\n")

"""
def plot_sin_head(sta_sample,sto_sample,mat_data,mode = 0):
	if(mode == 0):
		#speed
		initsta = int(sta_sample - mat_data.head_init_sample)
		initsto = int(sto_sample - mat_data.head_init_sample)
		x_data = range(initsta,initsto)
		y_data = mat_data.head_SA_HS[initsta:initsto,1]
		tpl = plot(x_data,y_data)
		return tpl

"""

def find_max_gaze_pos(mat_data,)



# In[34]:


#Run a single subject through(parameters, Dynamic delay delete list, Gazeshift
#delay delete list)
def run_subject(para,sub_index,DY_DL=[], GA_DL=[],plot_detail=False,shift1=0,shift2=0):
	AC_log1 = para[0]
	AC_log2 = para[1]
	JU_log1 = para[2]
	JU_log2 = para[3]
	JU_log3 = para[4]
	EY_log = para[5]
	EY_JU_log = para[6]
	Mat_log = para[7]
	eye_index = int(para[8])
	Mat_log_head = para[9]
	
	section1 = read_file(AC_log1,JU_log1,sectionN = 0) #Static Acuity
	section2 = read_file(AC_log1,JU_log1,sectionN = 1) #Dynamic Acuity
	section3 = read_file(AC_log2,JU_log2,sectionN = 0) #Dynamic Delay
	section4 = read_file(AC_log2,JU_log3,sectionN = 1) #Gazeshift Delay
	section5 = read_file(EY_log,EY_JU_log,sectionN = 0) #DD eye data
	section6 = read_file(EY_log,EY_JU_log,sectionN = 1) #GD eye data
	section1.sub_index = sub_index
	section2.sub_index = sub_index
	section3.sub_index = sub_index
	section4.sub_index = sub_index
	section5.sub_index = sub_index
	section6.sub_index = sub_index
	mat_data = None
	try:
		mat_data = read_mat(Mat_log,shift1) #MAT data
		mat_data = read_head_mat(mat_data,Mat_log_head,shift2) #MAT data
	except Exception as e:
		mat_data = MatData()
		print("mat data error!",e)
	mat_data.eye_index = eye_index
	mat_data.DY_delete_list = DY_DL
	mat_data.GA_delete_list = GA_DL
	
	section1.cal_AC()
	section2.cal_AC()
	section3.cal_delay()
	section4.cal_delay()
	section3.cal_delay_dir(mat_data)
	section4.cal_delay_dir(mat_data)
	section5.mode = "DynamicEye"
	section6.mode = "GazeShiftEye"
	mat_data.gaze_cal()
	mat_data.get_eye_trials(section5,0)
	mat_data.get_eye_trials(section6,1)
	mat_data.drop_out()
	mat_data.mean_cal()
	section3.cal_SD_dir()
	section4.cal_SD_dir()
	
	#plot_head(section3,mat_data,mode = 0,title = "Head_Position")
	print_CT(section3)
	plot_delay_dir(section3,"DynamicDelay_" + str(sub_index))
	plot_SD_percent3(section3,"DynamicStopDelay",S_index = sub_index)
	print_CT(section4)
	plot_delay_dir(section4,"GazeShiftDelay_" + str(sub_index))
	plot_SD_percent3(section4,"GazeShiftStopDelay",S_index = sub_index)
	print("Static Acuity: ",section1.AZ_percent)
	print("Dynamic Acuity: ",section2.AZ_percent)
	plot_static_dynamic_acuity(section1,"Static_Acuity")
	plot_static_dynamic_acuity(section2,"Dynamic_Acuity")
	plot_ST_DY(section1,section2)
	#plot_DYD_GAD(section3,section4)
	plot_DYD_GAD_dir(section3,section4)
	subplot_mean(mat_data,section3,0,0)
	subplot_mean(mat_data,section3,0,1)
	subplot_mean(mat_data,section3,0,2)
	subplot_mean(mat_data,section4,1,0)
	subplot_mean(mat_data,section4,1,1)
	subplot_mean(mat_data,section4,1,2)
	subplot_eye(mat_data,1,sectionN = 0,title = str(sub_index))
	subplot_eye(mat_data,2,sectionN = 0,title = str(sub_index))
	subplot_eye(mat_data,1,sectionN = 1,title = str(sub_index))
	subplot_eye(mat_data,2,sectionN = 1,title = str(sub_index))
	if(plot_detail):
		plot_eye_pos(mat_data,1,title = str(sub_index) + "_left_DD_",sectionN = 0)
		plot_eye_pos(mat_data,2,title = str(sub_index) + "_right_DD_",sectionN = 0)
		plot_eye_pos(mat_data,1,title = str(sub_index) + "_left_GD_",sectionN = 1)
		plot_eye_pos(mat_data,2,title = str(sub_index) + "_right_GD_",sectionN = 1)
	
	StaticSections.append(section1)
	DynamicSections.append(section2)
	DY_delaySections.append(section3)
	GA_delaySections.append(section4)
	DY_EYSections.append(section5)
	GA_EYSections.append(section6)
	MatDatas.append(mat_data)


# In[35]:

S_index += 1
sub_para = ["AcuityLog_2019_08_19_10_20_29.txt",        "AcuityLog_2019_08_19_10_35_08.txt",        "JumpLog__2019_08_19_10_20_29.txt",        "JumpLog__2019_08_19_10_35_08.txt",        "JumpLog__2019_08_19_10_35_08_2.txt",        "AcuityLog_2019_08_19_10_50_10.txt",        "",        "Mdata.mat",        "0",        "MdataH.mat"]
DY_DL = []
GA_DL = [17]
run_subject(sub_para,S_index, DY_DL = DY_DL, GA_DL = GA_DL, plot_detail = True)


# In[36]:

S_index += 1
sub_para = ["AcuityLog_2019_08_30_01_05_31.txt",        "AcuityLog_2019_08_30_01_19_07.txt",        "JumpLog__2019_08_30_01_05_31.txt",        "JumpLog__2019_08_30_01_19_07.txt",        "JumpLog__2019_08_30_01_19_07_2.txt",        "AcuityLog_2019_08_30_01_33_17.txt",        "",        "Qdata.mat",        "0",        "QdataH.mat"]
DY_DL = [0,5,8,9,10,11,12,13,14,15,16,17,18,19]
GA_DL = [0,1,2,4,6,7,8,9,10,11,12,13,14,15,16,17,18,19]
run_subject(sub_para,S_index,DY_DL = DY_DL, GA_DL = GA_DL, plot_detail = True)


# In[37]:

S_index += 1
sub_para = ["AcuityLog_2019_08_30_02_09_54.txt",        "AcuityLog_2019_08_30_02_25_56.txt",        "JumpLog__2019_08_30_02_09_54.txt",        "JumpLog__2019_08_30_02_25_56.txt",        "JumpLog__2019_08_30_02_25_56_2.txt",        "AcuityLog_2019_08_30_02_48_29.txt",        "",        "MIdata.mat",        "0",        "MIdataH.mat"]
DY_DL = [0]
GA_DL = [0,7,17]
run_subject(sub_para,S_index,DY_DL = DY_DL, GA_DL = GA_DL, plot_detail = True)


# In[38]:

S_index += 1
sub_para = ["AcuityLog_2019_08_30_03_30_50.txt",            "AcuityLog_2019_08_30_03_55_04.txt",            "JumpLog__2019_08_30_03_30_50.txt",            "JumpLog__2019_08_30_03_55_04.txt",            "JumpLog__2019_08_30_03_55_04_2.txt",            "AcuityLog_2019_08_30_04_11_56.txt",            "",            "S1data.mat",            "1",            "S1dataH.mat"]
DY_DL = [7,10]
GA_DL = [0,7,8,9,10,11]
run_subject(sub_para,S_index,DY_DL = DY_DL, GA_DL = GA_DL, plot_detail = True)


# In[39]:

S_index += 1
sub_para = ["AcuityLog_2019_09_18_05_23_42.txt",            "AcuityLog_2019_09_18_05_36_44.txt",            "JumpLog__2019_09_18_05_23_42.txt",            "JumpLog__2019_09_18_05_36_44.txt",            "JumpLog__2019_09_18_05_36_44_2.txt",            "AcuityLog_2019_09_18_05_53_13.txt",            "",            "Hdata.mat",            "0",            "HdataH.mat"]
DY_DL = []
GA_DL = [0,2,5,8,10,11,12,13,14,16,17,18]
run_subject(sub_para,S_index,DY_DL = DY_DL, GA_DL = GA_DL, plot_detail = True)


# In[40]:

S_index += 1
sub_para = ["AcuityLog_2019_10_18_01_43_44.txt",            "AcuityLog_2019_10_18_01_59_54.txt",            "JumpLog__2019_10_18_01_43_44.txt",            "JumpLog__2019_10_18_01_59_54.txt",            "JumpLog__2019_10_18_01_59_54_2.txt",            "AcuityLog_2019_10_18_02_22_03.txt",            "",            "Cdata.mat",            "0",            "CdataH.mat"]
DY_DL = [2]
GA_DL = [0,15,16,18]
run_subject(sub_para,S_index,DY_DL = DY_DL, GA_DL = GA_DL, plot_detail = True,shift2 = -1)


# In[41]:

def plot_fit(x_data,popt,index,lab_mode=0):
	x_data = np.arange(x_data[0],x_data[-1],fit_precise)
	y_data = sigmoid3(x_data, *popt)
	if(lab_mode == 1):
		tpl = plotplot(x_data,y_data,index,C_limit1,C_limit2)
	return tpl


# In[42]:

class TotalMean:
	def __init__(self):
		self.sa_dict = {} #SA mean -> dict{size : percentage}
		self.da_dict = {} #DA mean -> dict{size : percentage}
		self.dda_dict = {} #Delayed DA mean -> dict{time : percentage}
		self.dga_dict = {} #Delayed GA mean -> dict{time : percentage}
		self.sdda_dict = {} #Stop Delayed DA mean -> dict{time_index : arr[right,total,percentage]}
		self.sdga_dict = {} #Stop Delayed GA mean -> dict{time_index : arr[right,total,percentage]}
		self.sa_fit = [] #SA mean fit POPT,PCOV -> arr[arr[a,b,c],POCV]
		self.da_fit = [] #DA mean fit POPT,PCOV -> arr[arr[a,b,c],POCV]
		self.dda_fit = [] #Delayed DA mean fit POPT,PCOV -> arr[arr[a,b,c],POCV]
		self.dga_fit = [] #Delayed GA mean fit POPT,PCOV -> arr[arr[a,b,c],POCV]
		self.sdda_fit = [] #Stop Delayed DA mean fit POPT,PCOV -> arr[arr[a,b,c],POCV]
		self.sdga_fit = [] #Stop Delayed GA mean fit POPT,PCOV -> arr[arr[a,b,c],POCV]
		self.sa_TH = 0.0 #SA threshold -> size
		self.da_TH = 0.0 #DA threshold -> size
		self.dda_TH = 0.0 #Delayed DA threshold -> time
		self.dga_TH = 0.0 #Delayed GA threshold -> time
		self.sdda_TH = 0.0 #Stop Delayed DA threshold -> time
		self.sdga_TH = 0.0 #Stop Delayed GA threshold -> time
		self.sa_total_dict = {} #SA all threshold -> dict{subject : size}
		self.da_total_dict = {} #DA all threshold -> dict{subject : size}
		self.dda_total_dict = {}  #Delayed DA threshold -> dict{subject : size}
		self.dga_total_dict = {} #Delayed GA threshold -> dict{subject : size}
		self.sdda_total_dict = {} #Stop Delayed DA threshold -> dict{subject : size}
		self.sdga_total_dict = {} #Stop Delayed GA threshold -> dict{subject : size}
		self.sa_fit_total = {} #SA all fit POPTs -> dict{subject : POPT}
		self.da_fit_total = {} #DA all fit POPTs -> dict{subject : POPT}
		self.dda_fit_total = {} #Delayed DA all fit POPTs -> dict{subject : POPT}
		self.dga_fit_total = {} #Delayed GA all fit POPTs -> dict{subject : POPT}
		self.sdda_fit_total = {} #Stop Delayed DA all fit POPTs -> dict{subject : POPT}
		self.sdga_fit_total = {} #Stop Delayed GA all fit POPTs -> dict{subject : POPT}
		self.sa_quant = [] #Satic Acuity Quantile -> arr[x,low,med,up]
		self.da_quant = [] #Dynamic Acuity Quantile -> arr[x,low,med,up]
		self.dda_quant = [] #Delayed Dynamic Acuity Quantile -> arr[x,low,med,up]
		self.dga_quant = [] #Delayed Gazeshift Acuity Quantile -> arr[x,low,med,up]
		self.sdda_quant = [] #Stop Delayed Gazeshift Acuity Quantile -> arr[x,low,med,up]
		self.sdga_quant = [] #Stop Delayed Gazeshift Acuity Quantile -> arr[x,low,med,up]
		self.sa_raw_quant = [] #Raw Static Acuity Quantile -> arr[x,low,med,up]
		self.da_raw_quant = [] #Raw Dynamic Acuity Quantile -> arr[x,low,med,up]
		self.dda_raw_quant = [] #Raw Delayed Dynamic Acuity Quantile -> arr[x,low,med,up]
		self.dga_raw_quant = [] #Raw Delayed Gaze-shift Quantile -> arr[x,low,med,up]
		self.sdda_raw_quant = [] #Raw Stop Delayed Dynmaic Acuity Quantile -> arr[x,low,med,up]
		self.sdga_raw_quant = [] #Raw Stop Delayed Gaze-shift Acuity Quantile -> arr[x,low,med,up]
		self.sa_raw_quant_fit = [] #Raw Static Acuity Quantile Fit POPTs -> arr[low POPTS,med POPTS,up POPTS]
		self.da_raw_quant_fit = [] #Raw Dynamic Acuity Quantile Fit POPTs -> arr[low POPTS,med POPTS,up POPTS]
		self.dda_raw_quant_fit = [] #Raw Delayed Dynamic Acuity Quantile POPTs -> arr[low POPTS,med POPTS,up
									#POPTS]
		self.dga_raw_quant_fit = [] #Raw Delayed Gaze-shift Quantile POPTs -> arr[low POPTS,med POPTS,up POPTS]
		self.sdda_raw_quant_fit = [] #Raw Stop Delayed Dynmaic Acuity Quantile POPTs -> arr[low POPTS,med POPTS,up
									 #POPTS]
		self.sdga_raw_quant_fit = [] #Raw Stop Delayed Gaze-shift Acuity Quantile POPTs -> arr[low POPTS,med
									 #POPTS,up POPTS]
	
	def _mean_cal(self,sections, mode=0):
		res_dict = {}
		count = 0
		if(mode == 0):
			for SS in sections:
				for k in SS.AZ_percent.keys():
					if k in res_dict:
						res_dict[k] += SS.AZ_percent[k]
					else:
						res_dict[k] = SS.AZ_percent[k]
				count += 1
		elif(mode == 1):
			for DS in sections:
				for k in DS.AZ_percent.keys():
					if k in res_dict:
						res_dict[k] += DS.AZ_percent[k]
					else:
						res_dict[k] = DS.AZ_percent[k]
				count += 1
		elif(mode == 2):
			for DDS in sections:
				for k in DDS.delay_percent.keys():
					if k in res_dict:
						res_dict[k] += DDS.delay_percent[k]
					else:
						res_dict[k] = DDS.delay_percent[k]
				count += 1
		elif(mode == 3):
			for GSS in sections:
				for k in GSS.delay_percent.keys():
					if k in res_dict:
						res_dict[k] += GSS.delay_percent[k]
					else:
						res_dict[k] = GSS.delay_percent[k]
				count += 1
		elif(mode == 4):
			for DSS in sections:
				for k in DSS.SD_percent.keys():
					if k in res_dict:
						res_dict[k] = numpy.add(res_dict[k],DSS.SD_percent[k])
					else:
						res_dict[k] = DSS.SD_percent[k]
		elif(mode == 5):
			for GSS in sections:
				for k in GSS.SD_percent.keys():
					if k in res_dict:
						res_dict[k] = numpy.add(res_dict[k],GSS.SD_percent[k])
					else:
						res_dict[k] = GSS.SD_percent[k]
		for k in res_dict.keys():
			if(mode == 0 or mode == 1 or mode == 2 or mode == 3):
				res_dict[k] /= float(count)
			elif(mode == 4 or mode == 5):
				res_dict[k][2] = float(res_dict[k][0]) / float(res_dict[k][1])
		return res_dict
	
	def _test(self,sections):
		for GSS in sections:
			print(" ---------------------- ")
			for k in GSS.SD_percent.keys():
				print(" !!! ",k," | ",GSS.SD_percent[k])
	
	def mean_cal_tot(self,SSs,DSs,DADSs,GSDSs):
		self.sa_dict = self._mean_cal(SSs,mode = 0)
		self.da_dict = self._mean_cal(DSs,mode = 1)
		self.dda_dict = self._mean_cal(DADSs,mode = 2)
		self.dga_dict = self._mean_cal(GSDSs,mode = 3)
		self.sdda_dict = self._mean_cal(DADSs,mode = 4)
		self.sdga_dict = self._mean_cal(GSDSs,mode = 5)
		print("sdda_dict ",self.sdda_dict)
		print("sdga_dict ",self.sdga_dict)

	def _log_fit(self,mode=0):
		if(mode == 0):
			x_data = sorted(numpy.asarray(list(self.sa_dict.keys())))
			y_data = []
			for k in x_data:
				y_data.append(self.sa_dict[k])
			self.sa_fit = curve_fit(sigmoid3, x_data, y_data)
		elif (mode == 1):
			x_data = sorted(numpy.asarray(list(self.da_dict.keys())))
			y_data = []
			for k in x_data:
				y_data.append(self.da_dict[k])
			self.da_fit = curve_fit(sigmoid3, x_data, y_data)
		elif (mode == 2):
			return
		elif (mode == 3):
			x_data = sorted(numpy.asarray(list(self.dga_dict.keys())))
			y_data = []
			for k in x_data:
				y_data.append(self.dga_dict[k])
			self.dga_fit = curve_fit(sigmoid3, x_data, y_data)
		elif (mode == 4):
			return
		elif (mode == 5):
			x_data = sorted(numpy.asarray(list(self.sdga_dict.keys())))
			y_data = []
			for k in x_data:
				y_data.append(self.sdga_dict[k][2])
			self.sdga_fit = curve_fit(sigmoid3, x_data, y_data)
		
	def _TH_cal(self,mode=0):
		if(mode == 0):
			x_data = sorted(numpy.asarray(list(self.sa_dict.keys())))
			fp = float(x_data[-1] - x_data[0]) * fit_precise
			x_data = np.arange(x_data[0],x_data[-1],fp)
			for x in x_data:
				if(sigmoid3(x,*(self.sa_fit[0])) >= AC_threshold):
					self.sa_TH = x
					break
		elif(mode == 1):
			x_data = sorted(numpy.asarray(list(self.da_dict.keys())))
			fp = float(x_data[-1] - x_data[0]) * fit_precise
			x_data = np.arange(x_data[0],x_data[-1],fp)
			for x in x_data:
				if(sigmoid3(x,*(self.da_fit[0])) >= AC_threshold):
					self.da_TH = x
					break
		elif(mode == 2):
			x_data = sorted(numpy.asarray(list(self.dda_dict.keys())))
			for x in x_data:
				if(self.dda_dict[x] >= DL_threshold):
					self.dda_TH = x
					break
		elif(mode == 3):
			x_data = sorted(numpy.asarray(list(self.dga_dict.keys())))
			for x in x_data:
				if(self.dga_dict[x] >= DL_threshold):
					self.dga_TH = x
					break
		elif(mode == 4):
			x_data = sorted(numpy.asarray(list(self.sdda_dict.keys())))
			for x in x_data:
				if(self.sdda_dict[x][2] >= DL_threshold):
					self.sdda_TH = x
					break     
		elif(mode == 5):
			x_data = sorted(numpy.asarray(list(self.sdga_dict.keys())))
			for x in x_data:
				if(self.sdga_dict[x][2] >= DL_threshold):
					self.sdga_TH = x
					break   
		
	def log_fit(self):
		self._log_fit(mode = 0)
		self._log_fit(mode = 1)
		self._log_fit(mode = 2)
		self._log_fit(mode = 3)
		self._log_fit(mode = 4)
		self._log_fit(mode = 5)
		self._TH_cal(mode = 0)
		self._TH_cal(mode = 1)
		print("sa_TH",self.sa_TH)
		print("da_TH",self.da_TH)
		self._TH_cal(mode = 2)
		self._TH_cal(mode = 3)
		print("dda_TH",self.dda_TH)
		print("dga_TH",self.dga_TH)
		self._TH_cal(mode = 4)
		self._TH_cal(mode = 5)
		print("sdda_TH",self.sdda_TH)
		print("sdga_TH",self.sdga_TH)
		
	def mean_plot(self):
		#pyplot.rcParams["figure.figsize"] = (20,10)
		
		#SA Mean Plot
		x_data = numpy.asarray(list(self.sa_dict.keys()))
		x_data.sort()
		y_data = []
		for k in x_data:
			y_data.append(self.sa_dict[k])
		tpl1 = plotplot(x_data,y_data,"Static Acuity Mean",C_limit1,C_limit2)
		#SA Fit Plot
		fp = float(x_data[-1] - x_data[0]) * fit_precise
		x_data = np.arange(x_data[0],x_data[-1],fp)
		y_data = sigmoid3(x_data, *(self.sa_fit[0]))
		tpl2 = plotplot(x_data,y_data,"Static Acuity Fit",C_limit1,C_limit2)
		#Threshold line
		pyplot.hlines(AC_threshold,x_data[0],x_data[-1],                      linestyles = 'dashed')
		pyplot.legend(handles=[tpl1,tpl2])
		pyplot.xlabel(xlab1)
		pyplot.ylabel(ylab1)
		#pyplot.show()
		pyplot.clf()
		
		#DA Mean Plot
		x_data = numpy.asarray(list(self.da_dict.keys()))
		x_data.sort()
		y_data = []
		for k in x_data:
			y_data.append(self.da_dict[k])
		tpl1 = plotplot(x_data,y_data,"Dynamic Acuity Mean",C_limit1,C_limit2)
		#DA Fit Plot
		fp = float(x_data[-1] - x_data[0]) * fit_precise
		x_data = np.arange(x_data[0],x_data[-1],fp)
		y_data = sigmoid3(x_data, *(self.da_fit[0]))
		tpl2 = plotplot(x_data,y_data,"Dynamic Acuity Fit",C_limit1,C_limit2)
		#Threshold line
		pyplot.hlines(AC_threshold,x_data[0],x_data[-1],                     linestyles = 'dashed')
		pyplot.legend(handles=[tpl1, tpl2])
		pyplot.xlabel(xlab1)
		pyplot.ylabel(ylab1)
		#pyplot.show()
		pyplot.clf()
		
		#Delayed Dynamic Acuity Plot
		x_data = numpy.asarray(list(self.dda_dict.keys()))
		x_data.sort()
		y_data = []
		for k in x_data:
			y_data.append(self.dda_dict[k])
		tpl = plotplot(x_data,y_data,"Delayed Dynamic Time Mean",C_limit1,C_limit2)
		#Threshold line
		pyplot.hlines(DL_threshold,x_data[0],x_data[-1],                      linestyles = 'dashed')
		pyplot.legend(handles=[tpl])
		pyplot.xlabel(xlab2)
		pyplot.ylabel(ylab1)
		#pyplot.show()
		pyplot.clf()
		
		#Delayed Gazeshift Acuity Plot
		x_data = numpy.asarray(list(self.dga_dict.keys()))
		x_data.sort()
		y_data = []
		for k in x_data:
			y_data.append(self.dga_dict[k])
		tpl = plotplot(x_data,y_data,"Delayed Gazeshift Time Mean",C_limit1,C_limit2)
		#Threshold line
		pyplot.hlines(DL_threshold,x_data[0],x_data[-1],                      linestyles = 'dashed')
		pyplot.legend(handles=[tpl])
		pyplot.xlabel(xlab2)
		pyplot.ylabel(ylab1)
		#pyplot.show()
		pyplot.clf()
		
		#Stop Delayed Dynamic Acuity Plot
		x_data = numpy.asarray(list(self.sdda_dict.keys()))
		x_data.sort()
		y_data = []
		for k in x_data:
			y_data.append(self.sdda_dict[k][2])
		tpl = plotplot(x_data,y_data,"Delayed Dynamic Time From Stop Mean",C_limit1,C_limit2)
		#Threshold line
		pyplot.hlines(DL_threshold,x_data[0],x_data[-1],                      linestyles = 'dashed')
		pyplot.legend(handles=[tpl])
		pyplot.xlabel(xlab3)
		pyplot.ylabel(ylab1)
		#pyplot.show()
		pyplot.clf()
		
		#Stop Delayed Gazeshift Acuity Plot
		x_data = numpy.asarray(list(self.sdga_dict.keys()))
		x_data.sort()
		y_data = []
		for k in x_data:
			y_data.append(self.sdga_dict[k][2])
		tpl = plotplot(x_data,y_data,"Delayed Gazeshift Time From Stop Mean",C_limit1,C_limit2)
		#Threshold line
		pyplot.hlines(DL_threshold,x_data[0],x_data[-1],                      linestyles = 'dashed')
		pyplot.legend(handles=[tpl])
		pyplot.xlabel(xlab3)
		pyplot.ylabel(ylab1)
		#pyplot.show()
		pyplot.clf()
	
	def _total_TH_cal(self,popt,x_data):
		fp = float(x_data[-1] - x_data[0]) * fit_precise
		x_data = np.arange(x_data[0],x_data[-1],fp)
		for x in x_data:
			if(sigmoid3(x,*popt) >= AC_threshold):
				return x
	
	#Fit and calculate thresholds for all subjects
	def total_fit(self,sections,mode=0):
		count = 0
		if(mode == 0):
			for SSs in sections:
				x_data = sorted(numpy.asarray(list(SSs.AZ_percent.keys())))
				y_data = []
				popt = []
				for k in x_data:
					y_data.append(SSs.AZ_percent[k])
				try:
					popt,pcov = curve_fit(sigmoid3, x_data, y_data)
					self.sa_total_dict[count] = _total_TH_cal(popt,x_data)
				except:
					y_data[1] -= 0.01
					try:
						popt,pcov = curve_fit(sigmoid3, x_data, y_data,maxfev=maxfitv)
						self.sa_total_dict[count] = self._total_TH_cal(popt,x_data)
					except Exception as e:
						print("fit error !!! ",mode, " | ",count, " | ",e)
						self.sa_total_dict[count] = 0
						popt = [0.0,0.0,0.0]
				self.sa_fit_total[count] = popt
				count += 1
		elif(mode == 1):
			for DSs in sections:
				x_data = sorted(numpy.asarray(list(DSs.AZ_percent.keys())))
				y_data = []
				popt = []
				for k in x_data:
					y_data.append(DSs.AZ_percent[k])
				try:
					popt,pcov = curve_fit(sigmoid3, x_data, y_data)
					self.da_total_dict[count] = _total_TH_cal(popt,x_data)
				except:
					y_data[1] -= 0.01
					try:
						popt,pcov = curve_fit(sigmoid3, x_data, y_data,maxfev=maxfitv)
						self.da_total_dict[count] = self._total_TH_cal(popt,x_data)
					except Exception as e:
						print("fit error !!! ",mode, " | ",count, " | ",e)
						self.da_total_dict[count] = 0
						popt = [0.0,0.0,0.0]
				self.da_fit_total[count] = popt
				count += 1
		elif(mode == 2):
			for DDSs in sections:
				x_data = sorted(numpy.asarray(list(DDSs.delay_percent.keys())))
				for x in x_data:
					if(DDSs.delay_percent[x] >= DL_threshold):
						self.dda_total_dict[count] = x
						break
				if not count in self.dda_total_dict:
					self.dda_total_dict[count] = x_data[-1]
				count += 1
		elif(mode == 3):
			for DGSs in sections:
				x_data = sorted(numpy.asarray(list(DGSs.delay_percent.keys())))
				y_data = []
				popt = []
				#Threshold
				for x in x_data:
					if(DGSs.delay_percent[x] >= DL_threshold):
						self.dga_total_dict[count] = x
						break
				if not count in self.dga_total_dict:
					self.dga_total_dict[count] = x_data[-1]
				#Fit
				for x in x_data:
					y_data.append(DGSs.delay_percent[x])
				try:
					popt,pcov = curve_fit(sigmoid3, x_data, y_data)
				except:
					y_data[1] -= 0.01
					try:
						popt,pcov = curve_fit(sigmoid3, x_data, y_data,maxfev=maxfitv)
					except Exception as e:
						print("fit error !!! ",mode, " | ",count, " | ",e)
				self.dga_fit_total[count] = popt
				count += 1
		elif(mode == 4): #From stop
			for DDSs in sections:
				x_data = sorted(numpy.asarray(list(DDSs.SD_percent.keys())))
				#Threshold
				for x in x_data:
					if(DDSs.SD_percent[x][2] >= DL_threshold):
						self.sdda_total_dict[count] = x
						break
				if not count in self.sdda_total_dict:
					self.sdda_total_dict[count] = x_data[-1]
				count += 1
		elif(mode == 5): #From stop
			for DGSs in sections:
				x_data = sorted(numpy.asarray(list(DGSs.SD_percent.keys())))
				y_data = []
				popt = []
				#Threshold
				for x in x_data:
					if(DGSs.SD_percent[x][2] >= DL_threshold):
						self.sdga_total_dict[count] = x
						break
				if not count in self.sdga_total_dict:
					self.sdga_total_dict[count] = x_data[-1]
				#Fit
				for x in x_data:
					y_data.append(DGSs.SD_percent[x][2])
				try:
					popt,pcov = curve_fit(sigmoid3, x_data, y_data)
				except:
					y_data[1] -= 0.01
					try:
						popt,pcov = curve_fit(sigmoid3, x_data, y_data,maxfev=maxfitv)
					except Exception as e:
						print("fit error !!! ",mode, " | ",count, " | ",e)
				self.sdga_fit_total[count] = popt
				count += 1
	
	#Calculation for quantiles
	def _quantile_cal(self,sections=None,mode=0):
		if(mode == 0):
			x_data = sorted(list(self.sa_dict.keys()))
			fp = float(x_data[-1] - x_data[0]) * fit_precise
			x_data = np.arange(x_data[0],x_data[-1],fp)
			for x in x_data:
				sample = []
				sample.append(x)
				temp = []
				for popt in self.sa_fit_total.values():
					val = sigmoid3(x,*popt)
					temp.append(val)
				temp.sort()
				sample.append(temp[int(len(temp) * quant_low)])
				sample.append(temp[int(len(temp) * 0.5)])
				sample.append(temp[int(len(temp) * quant_high)])
				self.sa_quant.append(sample)
			self.sa_quant = numpy.array(self.sa_quant)
		if(mode == 1):
			x_data = sorted(list(self.da_dict.keys()))
			fp = float(x_data[-1] - x_data[0]) * fit_precise
			x_data = np.arange(x_data[0],x_data[-1],fp)
			for x in x_data:
				sample = []
				sample.append(x)
				temp = []
				for popt in self.da_fit_total.values():
					val = sigmoid3(x,*popt)
					temp.append(val)
				temp.sort()
				sample.append(temp[int(len(temp) * quant_low)])
				sample.append(temp[int(len(temp) * 0.5)])
				sample.append(temp[int(len(temp) * quant_high)])
				self.da_quant.append(sample)
			self.da_quant = numpy.array(self.da_quant)
		if(mode == 2 and sections is not None):
			x_data = sorted(list(self.dda_dict.keys()))
			for x in x_data:
				sample = []
				sample.append(x)
				temp = []
				for sec in sections:
					temp.append(sec.delay_percent[x])
				temp.sort()
				sample.append(temp[int(len(temp) * quant_low)])
				sample.append(temp[int(len(temp) * 0.5)])
				sample.append(temp[int(len(temp) * quant_high)])
				self.dda_quant.append(sample)
			self.dda_quant = numpy.array(self.dda_quant)
		if(mode == 3):
			x_data = sorted(list(self.dga_dict.keys()))
			fp = float(x_data[-1] - x_data[0]) * fit_precise
			x_data = np.arange(x_data[0],x_data[-1],fp)
			for x in x_data:
				sample = []
				sample.append(x)
				temp = []
				for popt in self.dga_fit_total.values():
					val = sigmoid3(x,*popt)
					temp.append(val)
				temp.sort()
				sample.append(temp[int(len(temp) * quant_low)])
				sample.append(temp[int(len(temp) * 0.5)])
				sample.append(temp[int(len(temp) * quant_high)])
				self.dga_quant.append(sample)
			self.dga_quant = numpy.array(self.dga_quant)
		if(mode == 4):
			return
		if(mode == 5):
			x_data = sorted(list(self.sdga_dict.keys()))
			fp = float(x_data[-1] - x_data[0]) * fit_precise
			x_data = np.arange(x_data[0],x_data[-1],fp)
			for x in x_data:
				sample = []
				sample.append(x)
				temp = []
				for popt in self.sdga_fit_total.values():
					val = sigmoid3(x,*popt)
					temp.append(val)
				temp.sort()
				sample.append(temp[int(len(temp) * quant_low)])
				sample.append(temp[int(len(temp) * 0.5)])
				sample.append(temp[int(len(temp) * quant_high)])
				self.sdga_quant.append(sample)
			self.sdga_quant = numpy.array(self.sdga_quant)                    
			
	def _raw_qt_fit(self,x_data,y_data,mode):
		try:
			popt,pcov = curve_fit(sigmoid3, x_data, y_data)
			return popt
		except:
			y_data[1] -= 0.01
			try:
				popt,pcov = curve_fit(sigmoid3, x_data, y_data,maxfev=maxfitv)
				return popt
			except Exception as e:
				print("fit error !!! ",mode,"|",e)
				return [0.0,0.0,0.0]
	
	def _raw_quantile_cal(self,sections=None,mode=0):
		if(mode == 0): #Staic Acuity
			x_data = sorted(list(self.sa_dict.keys()))
			for x in x_data:
				sample = []
				sample.append(x)
				temp = []
				for sec in StaticSections:
					temp.append(sec.AZ_percent[x])
				temp.sort()
				sample.append(temp[int(len(temp) * quant_low)])
				sample.append(temp[int(len(temp) * 0.5)])
				sample.append(temp[int(len(temp) * quant_high)])
				self.sa_raw_quant.append(sample)
			self.sa_raw_quant = numpy.array(self.sa_raw_quant)
			f_x_data = x_data
			for i in range(0,3):
				f_y_data = self.sa_raw_quant[:,i + 1]
				popt = self._raw_qt_fit(f_x_data,f_y_data,mode)
				self.sa_raw_quant_fit.append(popt)
		
	def total_cal(self,SS,DS,DDS,DGS):
		self.total_fit(SS,mode = 0)
		print("sa_total_dict ",self.sa_total_dict," ------------ ")
		self.total_fit(DS,mode = 1)
		print("da_total_dict ",self.da_total_dict, " ------------ ")
		self.total_fit(DDS,mode = 2)
		print("dda_total_dict ",self.dda_total_dict, " ------------ ")
		self.total_fit(DGS,mode = 3)
		print("dga_total_dict ",self.dga_total_dict, " ------------ ")
		self.total_fit(DDS,mode = 4)
		print("dga_total_dict ",self.sdda_total_dict, " ------------ ")
		self.total_fit(DGS,mode = 5)
		print("dga_total_dict ",self.sdga_total_dict, " ------------ ")
		self._quantile_cal(mode = 0)
		self._quantile_cal(mode = 1)
		self._quantile_cal(sections = DDS,mode = 2)
		self._quantile_cal(mode = 3)
		self._quantile_cal(mode = 4)
		self._quantile_cal(mode = 5)
		self._raw_quantile_cal(mode = 0)
		
	def _total_plot(self,mode=0):
		if(mode == 0):
			#pyplot.title("Static")
			x_data = list(self.sa_total_dict.values())
			y_data = [0.5] * len(x_data)
			tpl1 = plot(x_data,y_data,".")
			pyplot.xlabel(xlab1)
			pyplot.ylabel(ylab1)
			#pyplot.show()
			pyplot.clf()
		elif(mode == 1):
			#pyplot.title("Dynamic")
			x_data = list(self.da_total_dict.values())
			y_data = [0.5] * len(x_data)
			tpl1 = plot(x_data,y_data,".")
			pyplot.xlabel(xlab1)
			pyplot.ylabel(ylab1)
			#pyplot.show()
			pyplot.clf()
		elif(mode == 2):
			#pyplot.title("Delayed Dynamic")
			x_data = list(self.dda_total_dict.values())
			y_data = [0.5] * len(x_data)
			tpl1 = plot(x_data,y_data,".")
			pyplot.xlabel(xlab2)
			pyplot.ylabel(ylab1)
			#pyplot.show()
			pyplot.clf()
		elif(mode == 3):
			#pyplot.title("Delayed Gazeshift")
			x_data = list(self.dga_total_dict.values())
			y_data = [0.5] * len(x_data)
			tpl1 = plot(x_data,y_data,".")
			pyplot.xlabel(xlab2)
			pyplot.ylabel(ylab1)
			#pyplot.show()
			pyplot.clf()
		elif(mode == 4):
			#pyplot.title("Stop Delayed Dynamic")
			x_data = list(self.sdda_total_dict.values())
			y_data = [0.5] * len(x_data)
			tpl1 = plot(x_data,y_data,".")
			pyplot.xlabel(xlab3)
			pyplot.ylabel(ylab1)
			#pyplot.show()
			pyplot.clf()
		elif(mode == 5):
			#pyplot.title("Stop Delayed Gazeshift")
			x_data = list(self.sdga_total_dict.values())
			y_data = [0.5] * len(x_data)
			tpl1 = plot(x_data,y_data,".")
			pyplot.xlabel(xlab3)
			pyplot.ylabel(ylab1)
			#pyplot.show()
			pyplot.clf()
		elif(mode == 6):
			#pyplot.title("Static VS Dynamic")
			y_data = list(self.sa_total_dict.values())
			tpl1, = plot(y_data,".",label = "Static Acuity")
			y_data = list(self.da_total_dict.values())
			tpl2, = plot(y_data,"*",label = "Dynamic Acuity")
			pyplot.legend(handles = [tpl1,tpl2])
			pyplot.xlabel(xlab1)
			pyplot.ylabel(ylab1)
			#pyplot.show()
			pyplot.clf()
	
	def total_plot(self):
		self._total_plot(mode = 0)
		self._total_plot(mode = 1)
		self._total_plot(mode = 2)
		self._total_plot(mode = 3)
		self._total_plot(mode = 4)
		self._total_plot(mode = 5)
		self._total_plot(mode = 6)
		
	def _find_vlinev(self,y_data):
		count = 0
		for y in y_data:
			if(y >= AC_threshold):
				return count
			count += 1
		
	def _quant_plot(self,mode=0):
		if(mode == 0):
			#self.sa_quant
			print("self.sa_quant")
			vlinev = []
			x_data = self.sa_quant[:,0]
			y_data = self.sa_quant[:,1]
			vlinev.append(x_data[self._find_vlinev(y_data)])
			tpl1 = plotplot(x_data,y_data,str(quant_low),C_limit1,C_limit2)
			y_data = self.sa_quant[:,2]
			vlinev.append(x_data[self._find_vlinev(y_data)])
			tpl2 = plotplot(x_data,y_data,str(0.5),C_limit1,C_limit2)
			y_data = self.sa_quant[:,3]
			vlinev.append(x_data[self._find_vlinev(y_data)])
			tpl3 = plotplot(x_data,y_data,str(quant_high),C_limit1,C_limit2)
			y_data = sigmoid3(x_data, *(self.sa_fit[0]))
			vlinev.append(x_data[self._find_vlinev(y_data)])
			tpl4 = plotplot(x_data,y_data,"mean",C_limit1,C_limit2)
			line = pyplot.hlines(AC_threshold,x_data[0],x_data[-1],linestyles = 'dashed',label = "Threshold: 0.5625")
			#pyplot.vlines(vlinev[3],C_limit1,AC_threshold,linestyles =
			#'dashed')
			print("Threshold 25,50,75,m ",vlinev)
			#pyplot.xticks(list(pyplot.xticks()[0]) + [vlinev[3]])
			#for vv in vlinev:
			#    pyplot.vlines(vv,0,AC_threshold,linestyles = 'dashed')
			pyplot.legend(handles = [tpl1,tpl2,tpl3,tpl4,line])
			pyplot.xlabel(xlab1)
			pyplot.ylabel(ylab1)
			pyplot.savefig(SavePath + "StaticAcuityTile" + ".png",dpi = 300)
			#pyplot.show()
			pyplot.clf()
		if(mode == 1):
			#self.da_quant
			print("self.da_quant")
			vlinev = []
			x_data = self.da_quant[:,0]
			y_data = self.da_quant[:,1]
			vlinev.append(x_data[self._find_vlinev(y_data)])
			tpl1 = plotplot(x_data,y_data,str(quant_low),C_limit1,C_limit2)
			y_data = self.da_quant[:,2]
			vlinev.append(x_data[self._find_vlinev(y_data)])
			tpl2 = plotplot(x_data,y_data,str(0.5),C_limit1,C_limit2)
			y_data = self.da_quant[:,3]
			vlinev.append(x_data[self._find_vlinev(y_data)])
			tpl3 = plotplot(x_data,y_data,str(quant_high),C_limit1,C_limit2)
			y_data = sigmoid3(x_data, *(self.da_fit[0]))
			vlinev.append(x_data[self._find_vlinev(y_data)])
			tpl4 = plotplot(x_data,y_data,"mean",C_limit1,C_limit2)
			line = pyplot.hlines(AC_threshold,x_data[0],x_data[-1],linestyles = 'dashed',label = "Threshold: 0.5625")
			#pyplot.vlines(vlinev[3],C_limit1,AC_threshold,linestyles =
			#'dashed')
			print("Threshold 25,50,75,m ",vlinev)
			pyplot.legend(handles = [tpl1,tpl2,tpl3,tpl4,line])
			pyplot.xlabel(xlab1)
			pyplot.ylabel(ylab1)
			pyplot.savefig(SavePath + "DynamicAcuityTile" + ".png",dpi = 300)
			#pyplot.show()
			pyplot.clf()
		if(mode == 2): #Dealyed Dyanmic Acuity
			#dda_quant
			print("self.dda_quant")
			vlinev = []
			x_data = self.dda_quant[:,0]
			y_data = self.dda_quant[:,1]
			vlinev.append(x_data[self._find_vlinev(y_data)])
			tpl1 = plotplot(x_data,y_data,str(quant_low),C_limit1,C_limit2)
			y_data = self.dda_quant[:,2]
			vlinev.append(x_data[self._find_vlinev(y_data)])
			tpl2 = plotplot(x_data,y_data,str(0.5),C_limit1,C_limit2)
			y_data = self.dda_quant[:,3]
			vlinev.append(x_data[self._find_vlinev(y_data)])
			tpl3 = plotplot(x_data,y_data,str(quant_high),C_limit1,C_limit2)
			#line = pyplot.hlines(AC_threshold,x_data[0],x_data[-1],linestyles
			#= 'dashed',label = "Threshold: 0.5625")
			#pyplot.vlines(vlinev[1],C_limit1,AC_threshold,linestyles =
			#'dashed')
			#pyplot.legend(handles = [tpl1,tpl2,tpl3,line])
			pyplot.legend(handles = [tpl1,tpl2,tpl3])
			pyplot.xlabel(xlab2)
			pyplot.ylabel(ylab1)
			pyplot.savefig(SavePath + "TemporalDynamicAcuityTileFromStart" + ".png",dpi = 300)
			#pyplot.show()
			pyplot.clf()
		if(mode == 3): #Delayed Gazeshift Acuity
			#dga_quant
			print("self.dga_quant")
			vlinev = []
			x_data = self.dga_quant[:,0]
			y_data = self.dga_quant[:,1]
			vlinev.append(x_data[self._find_vlinev(y_data)])
			tpl1 = plotplot(x_data,y_data,str(quant_low),C_limit1,C_limit2)
			y_data = self.dga_quant[:,2]
			vlinev.append(x_data[self._find_vlinev(y_data)])
			tpl2 = plotplot(x_data,y_data,str(0.5),C_limit1,C_limit2)
			y_data = self.dga_quant[:,3]
			vlinev.append(x_data[self._find_vlinev(y_data)])
			tpl3 = plotplot(x_data,y_data,str(quant_high),C_limit1,C_limit2)
			y_data = sigmoid3(x_data, *(self.dga_fit[0]))
			vlinev.append(x_data[self._find_vlinev(y_data)])
			tpl4 = plotplot(x_data,y_data,"mean",C_limit1,C_limit2)
			line = pyplot.hlines(AC_threshold,x_data[0],x_data[-1],linestyles = 'dashed',label = "Threshold: 0.5625")
			#pyplot.vlines(vlinev[3],C_limit1,AC_threshold,linestyles =
			#'dashed')
			print("Threshold 25,50,75,m ",vlinev)
			pyplot.legend(handles = [tpl1,tpl2,tpl3,tpl4,line])
			pyplot.xlabel(xlab2)
			pyplot.ylabel(ylab1)
			pyplot.savefig(SavePath + "TemporalGazeShiftTileFromStart" + ".png",dpi = 300)
			#pyplot.show()
			pyplot.clf()
		if(mode == 4):
			return
		if(mode == 5): #Stop Delayed Gazeshift Acuity
			#sdga_quant
			print("self.sdga_quant")
			vlinev = []
			x_data = self.sdga_quant[:,0]
			y_data = self.sdga_quant[:,1]
			vlinev.append(x_data[self._find_vlinev(y_data)])
			tpl1 = plotplot(x_data,y_data,str(quant_low),C_limit1,C_limit2)
			y_data = self.sdga_quant[:,2]
			vlinev.append(x_data[self._find_vlinev(y_data)])
			tpl2 = plotplot(x_data,y_data,str(0.5),C_limit1,C_limit2)
			y_data = self.sdga_quant[:,3]
			vlinev.append(x_data[self._find_vlinev(y_data)])
			tpl3 = plotplot(x_data,y_data,str(quant_high),C_limit1,C_limit2)
			y_data = sigmoid3(x_data, *(self.sdga_fit[0]))
			vlinev.append(x_data[self._find_vlinev(y_data)])
			tpl4 = plotplot(x_data,y_data,"mean",C_limit1,C_limit2)
			line = pyplot.hlines(AC_threshold,x_data[0],x_data[-1],linestyles = 'dashed',label = "Threshold: 0.5625")
			#pyplot.vlines(vlinev[3],C_limit1,AC_threshold,linestyles =
			#'dashed')
			print("Threshold 25,50,75,m ",vlinev)
			pyplot.legend(handles = [tpl1,tpl2,tpl3,tpl4,line])
			pyplot.xlabel(xlab3)
			pyplot.ylabel(ylab1)
			pyplot.savefig(SavePath + "TemporalGazeShiftTileFromStop" + ".png",dpi = 300)
			#pyplot.show()
			pyplot.clf()
			
	def _quant_fit_plot(self,mode=0):
		leg = ["0.25","0.5","0.75"]
		if(mode == 0):
			vlinev = []
			x_data = self.sa_raw_quant[:,0]
			count = 0
			tpls = []
			for popt in self.sa_raw_quant_fit:
				tpl = plot_fit(x_data,popt,leg[count],lab_mode = 1)
				tpls.append(tpl)
				count += 1
			p_x_data = np.arange(x_data[0],x_data[-1],fit_precise)
			y_data = sigmoid3(p_x_data, *(self.sa_fit[0]))
			tpl = plotplot(p_x_data,y_data,"mean",C_limit1,C_limit2)
			tpls.append(tpl)
			vlinev.append(p_x_data[self._find_vlinev(y_data)])
			line = pyplot.hlines(AC_threshold,x_data[0],x_data[-1],linestyles = 'dashed',label = "Threshold: 0.5625")
			pyplot.vlines(vlinev[0],C_limit1,AC_threshold,linestyles = 'dashed')
			tpls.append(line)
			pyplot.legend(handles = tpls)
			pyplot.xlabel(xlab1)
			pyplot.ylabel(ylab1)
			pyplot.savefig(SavePath + "SA_quant_fit" + ".png",dpi = 300)
			#pyplot.show()
			pyplot.clf()
		
	def quant_plot(self):
		self._quant_plot(mode = 0)
		self._quant_plot(mode = 1)
		self._quant_plot(mode = 2)
		self._quant_plot(mode = 3)
		self._quant_plot(mode = 4)
		self._quant_plot(mode = 5)
		self._quant_fit_plot(mode = 0)
		
	def write_to_file(self,path,rstr):
		file = open(path + ".txt","w")
		file.write(rstr)
		file.close
		
	def log_to_file(self):
		#sa_dict
		rstr = "LogMAR\tpercentage\n"
		for sa in self.sa_dict:
			rstr += str(sa) + "\t" + str(self.sa_dict[sa]) + "\n"
		self.write_to_file(resdata + "mean_static_acuity",rstr)
		#self.da_dict
		rstr = "LogMAR\tpercentage\n"
		for da in self.da_dict:
			rstr += str(da) + "\t" + str(self.da_dict[sa]) + "\n"
		self.write_to_file(resdata + "mean_dynamic_acuity",rstr)
		#self.dda_dict
		rstr = "TimeStamp\tpercentage\n"
		for dda in self.dda_dict:
			rstr += str(dda) + "\t" + str(self.dda_dict[dda]) + "\n"
		self.write_to_file(resdata + "mean_delayed_dynamic_acuity",rstr)
		#self.dga_dict
		rstr = "TimeStamp\tpercentage\n"
		for dga in self.dga_dict:
			rstr += str(dga) + "\t" + str(self.dga_dict[dga]) + "\n"
		self.write_to_file(resdata + "mean_delayed_gazeshift_acuity",rstr)
		#self.sdda_dict
		rstr = "TimeStamp\tright\ttotal\tpercentage\n"
		for sdda in self.sdda_dict:
			rstr += str(sdda)
			for da in self.sdda_dict[sdda]:
				rstr += "\t"
				rstr += str(da)
			rstr += "\n"
		self.write_to_file(resdata + "mean_delayed_dynamic_acuity_fromheadstop",rstr)
		#self.sdga_dict
		rstr = "TimeStamp\tright\ttotal\tpercentage\n"
		for sdga in self.sdga_dict:
			rstr += str(sdga)
			for da in self.sdga_dict[sdga]:
				rstr += "\t"
				rstr += str(da)
			rstr += "\n"
		self.write_to_file(resdata + "mean_delayed_gazeshift_acuity_fromheadstop",rstr)
		#self.sa_fit
		rstr = "POPT1\tPOPT2\tPOPT3\tPOCV\n"
		for POPT in self.sa_fit[0]:
			rstr += str(POPT) + "\t"
		rstr += str(self.sa_fit[1]) + "\n"
		self.write_to_file(resdata + "mean_static_acuity_fit",rstr)
		#self.da_fit
		rstr = "POPT1\tPOPT2\tPOPT3\tPOCV\n"
		for POPT in self.da_fit[0]:
			rstr += str(POPT) + "\t"
		rstr += str(self.da_fit[1]) + "\n"
		self.write_to_file(resdata + "mean_dynamic_acuity_fit",rstr)
		"""
		#self.dda_fit
		rstr = "POPT1\tPOPT2\tPOPT3\tPOCV\n"
		for POPT in self.dda_fit[0]:
			rstr += POPT + "\t"
		rstr += self.dda_fit[1] + "\n"
		self.write_to_file(resdata+"mean_delayed_dynamic_acuity_fit",rstr)
		"""
		#self.dga_fit
		rstr = "POPT1\tPOPT2\tPOPT3\tPOCV\n"
		for POPT in self.dga_fit[0]:
			rstr += str(POPT) + "\t"
		rstr += str(self.dga_fit[1]) + "\n"
		self.write_to_file(resdata + "mean_delayed_gazeshift_acuity_fit",rstr)
		"""
		#self.sdda_fit
		rstr = "POPT1\tPOPT2\tPOPT3\tPOCV\n"
		for POPT in self.sdda_fit[0]:
			rstr += POPT + "\t"
		rstr += self.sdda_fit[1] + "\n"
		self.write_to_file(resdata+"mean_delayed_gazeshift_acuity_fit",rstr)
		"""
		#self.sdga_fit
		rstr = "POPT1\tPOPT2\tPOPT3\tPOCV\n"
		for POPT in self.sdga_fit[0]:
			rstr += str(POPT) + "\t"
		rstr += str(self.sdga_fit[1]) + "\n"
		self.write_to_file(resdata + "mean_delayed_gazeshift_acuity_fit_fromheadstop",rstr)
		#self.sa_fit_total
		rstr = "Subject\tPOPT1\tPOPT2\tPOPT3\n"
		for sub in self.sa_fit_total:
			rstr += str(sub)
			for POPT in self.sa_fit_total[sub]:
				rstr += "\t" + str(POPT)
			rstr += "\n"
		self.write_to_file(resdata + "subjects_static_acuity_fit",rstr)
		#self.da_fit_total
		rstr = "Subject\tPOPT1\tPOPT2\tPOPT3\n"
		for sub in self.da_fit_total:
			rstr += str(sub)
			for POPT in self.da_fit_total[sub]:
				rstr += "\t" + str(POPT)
			rstr += "\n"
		self.write_to_file(resdata + "subjects_dynamic_acuity_fit",rstr)
		"""
		#self.dda_fit_total
		rstr = "Subject\tPOPT1\tPOPT2\tPOPT3\n"
		for sub in self.dda_fit_total:
			rstr += sub
			for POPT in self.dda_fit_total[sub]:
				rstr += "\t" + POPT
			rstr += "\n"
		self.write_to_file(resdata+"subjects_dynamic_acuity_fit",rstr)
		"""
		#self.dga_fit_total
		rstr = "Subject\tPOPT1\tPOPT2\tPOPT3\n"
		for sub in self.dga_fit_total:
			rstr += str(sub)
			for POPT in self.dga_fit_total[sub]:
				rstr += "\t" + str(POPT)
			rstr += "\n"
		self.write_to_file(resdata + "subjects_gazeshift_acuity_fit",rstr)
		#self.sdga_fit_total
		rstr = "Subject\tPOPT1\tPOPT2\tPOPT3\n"
		for sub in self.sdga_fit_total:
			rstr += str(sub)
			for POPT in self.sdga_fit_total[sub]:
				rstr += "\t" + str(POPT)
			rstr += "\n"
		self.write_to_file(resdata + "subjects_gazeshift_acuity_fit_fromheadstop",rstr)
		#self.sa_quant
		rstr = "x\tlowy\tmediany\thighy\n"
		for da in self.sa_quant:
			for dada in da:
				rstr += str(dada) + "\t"
			rstr += "\n"
		self.write_to_file(resdata + "static_acuity_fitthenquantile",rstr)
		#self.da_quant
		rstr = "x\tlowy\tmediany\thighy\n"
		for da in self.da_quant:
			for dada in da:
				rstr += str(dada) + "\t"
			rstr += "\n"
		self.write_to_file(resdata + "dynamic_acuity_fitthenquantile",rstr)
		#self.dga_quant
		rstr = "x\tlowy\tmediany\thighy\n"
		for da in self.dga_quant:
			for dada in da:
				rstr += str(dada) + "\t"
			rstr += "\n"
		self.write_to_file(resdata + "delayed_gazeshift_acuity_fitthenquantile",rstr)
		#self.sdga_quant
		rstr = "x\tlowy\tmediany\thighy\n"
		for da in self.sdga_quant:
			for dada in da:
				rstr += str(dada) + "\t"
			rstr += "\n"
		self.write_to_file(resdata + "delayed_gazeshift_acuity_fromheadstop_fitthenquantile",rstr)
		#self.sa_raw_quant
		rstr = "x\tlowy\tmediany\thighy\n"
		for da in self.sa_raw_quant:
			for dada in da:
				rstr += str(dada) + "\t"
			rstr += "\n"
		self.write_to_file(resdata + "static_acuity_raw_quantile",rstr)
		"""
		#self.da_raw_quant
		rstr = "x\tlowy\tmediany\thighy\n"
		for da in self.da_raw_quant:
			for dada in da:
				rstr += str(dada) + "\t"
			rstr += "\n"
		self.write_to_file(resdata+"dynamic_acuity_raw_quantile",rstr)
		"""
		"""
		#self.dda_raw_quant
		rstr = "x\tlowy\tmediany\thighy\n"
		for da in self.dda_raw_quant:
			for dada in da:
				rstr += str(dada) + "\t"
			rstr += "\n"
		self.write_to_file(resdata+"delayed_dynamic_acuity_raw_quantile",rstr)
		"""
		"""
		#self.dga_raw_quant
		rstr = "x\tlowy\tmediany\thighy\n"
		for da in self.dga_raw_quant:
			for dada in da:
				rstr += str(dada) + "\t"
			rstr += "\n"
		self.write_to_file(resdata+"delayed_gazeshift_acuity_raw_quantile",rstr)
		"""
		"""
		#self.sdda_raw_quant
		rstr = "x\tlowy\tmediany\thighy\n"
		for da in self.sdda_raw_quant:
			for dada in da:
				rstr += str(dada) + "\t"
			rstr += "\n"
		self.write_to_file(resdata+"delayed_dynamic_acuity_raw_quantile_fromheadstop",rstr)
		"""
		"""
		#self.sdga_raw_quant
		rstr = "x\tlowy\tmediany\thighy\n"
		for da in self.sdga_raw_quant:
			for dada in da:
				rstr += str(dada) + "\t"
			rstr += "\n"
		self.write_to_file(resdata+"delayed_gazeshift_acuity_raw_quantile_fromheadstop",rstr)
		"""
	
	def plot_subjects(self,mode=0):
		#self.sa_fit_total
		print("Static fit")
		counter = 0
		for sub in self.sa_fit_total:
			popt = self.sa_fit_total[sub]
			temp = list(self.sa_dict.keys())
			maxx = max(temp)
			minx = min(temp)
			step = (maxx - minx) * fit_precise
			x_data = numpy.arange(minx,maxx,step)
			y_data = sigmoid3(x_data, *(popt))
			tpl = plotplot(x_data,y_data,"Logistic Curve",C_limit1,C_limit2)
			line = pyplot.hlines(AC_threshold,x_data[0],x_data[-1],linestyles = 'dashed',                                  label = "Threshold: 0.5625")
			pyplot.legend(handles=[tpl,line])
			pyplot.xlabel(xlab1)
			pyplot.ylabel(ylab1)
			pyplot.savefig(SavePath + str(counter) + "_StaticAcuityFit" + ".png",dpi = 300)
			#pyplot.show()
			pyplot.clf()
			counter += 1
		#self.da_fit_total
		print("Dynamic fit")
		counter = 0
		for sub in self.da_fit_total:
			popt = self.da_fit_total[sub]
			temp = list(self.da_dict.keys())
			maxx = max(temp)
			minx = min(temp)
			step = (maxx - minx) * fit_precise
			x_data = numpy.arange(minx,maxx,step)
			y_data = sigmoid3(x_data, *(popt))
			tpl = plotplot(x_data,y_data,"Logistic Curve",C_limit1,C_limit2)
			line = pyplot.hlines(AC_threshold,x_data[0],x_data[-1],linestyles = 'dashed',                                  label = "Threshold: 0.5625")
			pyplot.legend(handles=[tpl,line])
			pyplot.xlabel(xlab1)
			pyplot.ylabel(ylab1)
			pyplot.savefig(SavePath + str(counter) + "_DynamicACuityFit" + ".png",dpi = 300)
			#pyplot.show()
			pyplot.clf()
			counter += 1
			
	def plot_static_dynamic_fit(self,sub):
		print("Static-Dynamic")
		popt = self.sa_fit_total[sub]
		temp = list(self.sa_dict.keys())
		maxx = max(temp)
		minx = min(temp)
		step = (maxx - minx) * fit_precise
		x_data = numpy.arange(minx,maxx,step)
		y_data = sigmoid3(x_data, *(popt))
		tpl1 = plotplot(x_data,y_data,"Static Acuity",C_limit1,C_limit2)
		popt = self.da_fit_total[sub]
		temp = list(self.da_dict.keys())
		maxx = max(temp)
		minx = min(temp)
		step = (maxx - minx) * fit_precise
		x_data = numpy.arange(minx,maxx,step)
		y_data = sigmoid3(x_data, *(popt))
		tpl2 = plotplot(x_data,y_data,"Dynamic Acuity",C_limit1,C_limit2)
		line = pyplot.hlines(AC_threshold,x_data[0],x_data[-1],linestyles = 'dashed',                              label = "Threshold: 0.5625")
		pyplot.legend(handles=[tpl1,tpl2,line])
		pyplot.xlabel(xlab1)
		pyplot.ylabel(ylab1)
		pyplot.savefig(SavePath + str(sub) + "_SDFit" + ".png",dpi = 300)
		#pyplot.show()
		pyplot.clf()
		
		


# In[43]:

def do_total_mean():
	total_mean = TotalMean()
	total_mean.mean_cal_tot(StaticSections,DynamicSections,DY_delaySections,GA_delaySections)
	total_mean.log_fit()
	total_mean.mean_plot()
	total_mean.total_cal(StaticSections,DynamicSections,DY_delaySections,GA_delaySections)
	total_mean.total_plot()
	total_mean.quant_plot()
	total_mean.plot_subjects(mode = 0)
	total_mean.plot_static_dynamic_fit(2)
	return total_mean


# In[44]:

total_mean = do_total_mean()


# In[45]:

total_mean.log_to_file()


# x_data = sorted(list(total_mean.dga_dict.keys()))
# x_data = np.arange(x_data[0],x_data[-1],fit_precise)
# for x in x_data:
#     sample = []
#     sample.append(x)
#     temp = []
#     for popt in total_mean.dga_fit_total.values():
#         val = sigmoid3(x,*popt)
#         temp.append(val)
#     temp.sort()
#     print(x,temp)

# %matplotlib notebook
# total_mean._quant_plot(mode = 3)

# In[46]:

def plot_stop_delay(section,lab_mode=0):
	x_data = sorted(list(section.SD_percent.keys()))
	y_data = []
	for x in x_data:
		y_data.append(section.SD_percent[x][2])
	if(lab_mode == 0):
		tpl = plotplot(x_data,y_data,section.mode,C_limit1,C_limit2)
	elif(lab_mode == 1):
		tpl = plotplot(x_data,y_data,section.sub_index,C_limit1,C_limit2)
	return tpl
	


# In[47]:


#Plots for all subjects
def mix_plot():
	handles = []
	for SS in StaticSections:
		tpl = plot_ac(SS,lab_mode = 1)
		handles.append(tpl)
	#pyplot.legend(handles=handles)
	pyplot.xlabel(xlab1)
	pyplot.ylabel(ylab1)
	pyplot.savefig(SavePath + "StaticMix" + ".png",dpi = 300)
	#pyplot.title("Static Acuity")
	#pyplot.show()
	pyplot.clf()
	
	handles = []
	for DS in DynamicSections:
		tpl = plot_ac(DS,lab_mode = 1)
		handles.append(tpl)
	#pyplot.legend(handles=handles)
	pyplot.xlabel(xlab1)
	pyplot.ylabel(ylab1)
	pyplot.savefig(SavePath + "DynamicMix" + ".png",dpi = 300)
	#pyplot.title("Dynamic Acuity")
	#pyplot.show()
	pyplot.clf()
	
	handles = []
	for DDS in DY_delaySections:
		tpl = plot_delay(DDS,lab_mode = 1)
		handles.append(tpl)
	#pyplot.legend(handles=handles)
	pyplot.xlabel(xlab2)
	pyplot.ylabel(ylab1)
	pyplot.savefig(SavePath + "DynamicDelayMix" + ".png",dpi = 300)
	#pyplot.title("Dynamic Delay")
	#pyplot.show()
	pyplot.clf()
	
	handles = []
	for GDS in GA_delaySections:
		tpl = plot_delay(GDS,lab_mode = 1)
		handles.append(tpl)
	#pyplot.legend(handles=handles)
	pyplot.xlabel(xlab2)
	pyplot.ylabel(ylab1)
	pyplot.savefig(SavePath + "GazeDelayMix" + ".png",dpi = 300)
	#pyplot.title("Gazeshift Delay")
	#pyplot.show()
	pyplot.clf()
	
	#Stop Delayed Dynamic Acuity
	handles = []
	for DDS in DY_delaySections:
		tpl = plot_stop_delay(DDS,lab_mode = 1)
		handles.append(tpl)
	#pyplot.legend(handles=handles)
	pyplot.xlabel(xlab3)
	pyplot.ylabel(ylab1)
	pyplot.savefig(SavePath + "StopDelayDynamic" + ".png",dpi = 300)
	#pyplot.title("Stop Delay Dynamic")
	#pyplot.show()
	pyplot.clf()
	
	#Stop Delayed Gazeshift Acuity
	handles = []
	for GDS in GA_delaySections:
		tpl = plot_stop_delay(GDS,lab_mode = 1)
		handles.append(tpl)
	#pyplot.legend(handles=handles)
	pyplot.xlabel(xlab3)
	pyplot.ylabel(ylab1)
	pyplot.savefig(SavePath + "StopDelayGazeshift" + ".png",dpi = 300)
	#pyplot.title("Stop Delay Gazeshift")
	#pyplot.show()
	pyplot.clf()
	
	#Static Acuity Fit
	handles = []
	x_min = 0.0
	x_max = 0.0
	for SS in StaticSections:
		x_data = sorted(list(SS.AZ_percent.keys()))
		x_min = min(x_min,x_data[0])
		x_max = max(x_max,x_data[-1])
		tpl = plot_fit(x_data,total_mean.sa_fit_total[SS.sub_index],SS.sub_index,lab_mode = 1)
		handles.append(tpl)
	line = pyplot.hlines(AC_threshold,x_min,x_max,linestyles = 'dashed',label = "Threshold: 0.5625")
	pyplot.legend(handles = [line])
	pyplot.xlabel(xlab1)
	pyplot.ylabel(ylab1)
	pyplot.savefig(SavePath + "Static Acuity Fit" + ".png",dpi = 300)
	#pyplot.title("Static Acuity Fit")
	#pyplot.show()
	pyplot.clf()
	
	#Dynamic Acuity Fit
	handles = []
	x_min = 0.0
	x_max = 0.0
	for DS in DynamicSections:
		x_data = sorted(list(DS.AZ_percent.keys()))
		x_min = min(x_min,x_data[0])
		x_max = max(x_max,x_data[-1])
		tpl = plot_fit(x_data,total_mean.da_fit_total[DS.sub_index],DS.sub_index,lab_mode = 1)
		handles.append(tpl)
	line = pyplot.hlines(AC_threshold,x_min,x_max,linestyles = 'dashed',label = "Threshold: 0.5625")
	pyplot.legend(handles=[line])
	pyplot.xlabel(xlab1)
	pyplot.ylabel(ylab1)
	pyplot.savefig(SavePath + "Dynamic Acuity Fit" + ".png",dpi = 300)
	#pyplot.title("Dynamic Acuity Fit")
	#pyplot.show()
	pyplot.clf()
	
	#Gazeshift Acuity Fit
	handles = []
	x_min = 0.0
	x_max = 0.0
	for GDS in GA_delaySections:
		x_data = sorted(list(GDS.delay_percent.keys()))
		x_min = min(x_min,x_data[0])
		x_max = max(x_max,x_data[-1])
		tpl = plot_fit(x_data,total_mean.dga_fit_total[GDS.sub_index],GDS.sub_index,lab_mode = 1)
		handles.append(tpl)
	line = pyplot.hlines(AC_threshold,x_min,x_max,linestyles = 'dashed',label = "Threshold: 0.5625")
	pyplot.legend(handles=[line])
	pyplot.xlabel(xlab2)
	pyplot.ylabel(ylab1)
	pyplot.savefig(SavePath + "Gazeshift Acuity Fit" + ".png",dpi = 300)
	#pyplot.title("Gazeshift Acuity Fit")
	#pyplot.show()
	pyplot.clf()
	
	#Stop Gazesift Acuity Fit
	handles = []
	x_min = 0.0
	x_max = 0.0
	for GDS in GA_delaySections:
		x_data = sorted(list(GDS.SD_percent.keys()))
		x_min = min(x_min,x_data[0])
		x_max = max(x_max,x_data[-1])
		tpl = plot_fit(x_data,total_mean.sdga_fit_total[GDS.sub_index],GDS.sub_index,lab_mode = 1)
		handles.append(tpl)
	line = pyplot.hlines(AC_threshold,x_min,x_max,linestyles = 'dashed',label = "Threshold: 0.5625")
	pyplot.legend(handles=[line])
	pyplot.xlabel(xlab3)
	pyplot.ylabel(ylab1)
	pyplot.savefig(SavePath + "Stop Gazesift Acuity Fit" + ".png",dpi = 300)
   # pyplot.title("Stop Gazesift Acuity Fit")
	#pyplot.show()
	pyplot.clf()


# In[48]:

mix_plot()


# In[49]:

def subjects_log_to_file():
	#StaticSections
	counter = 0
	for SS in StaticSections:
		SS.log_to_file(str(counter) + "_StaticAcuity",mode = 0)
		counter += 1
	#DynamicSections
	counter = 0
	for DS in DynamicSections:
		DS.log_to_file(str(counter) + "_DynamicAcuity",mode = 0)
		counter += 1
	#DY_delaySections
	counter = 0
	for DDS in DY_delaySections:
		DDS.log_to_file(str(counter) + "_DelayedDynamicAcuity",mode = 1)
		counter += 1
	#DY_delaySections_stop
	counter = 0
	for DDS in DY_delaySections:
		DDS.log_to_file(str(counter) + "_DelayedDynamicAcuityFromHeadStop",mode = 2)
		counter += 1
	#GA_delaySections
	counter = 0
	for GDS in GA_delaySections:
		GDS.log_to_file(str(counter) + "_DelayedGazeshiftAcuity",mode = 1)
		counter += 1
	#GA_delaySections_stop
	counter = 0
	for GDS in GA_delaySections:
		GDS.log_to_file(str(counter) + "_DelayedGazeshiftAcuityFromHeadStop",mode = 2)
		counter += 1


# In[50]:

subjects_log_to_file()


pyplot.close("all")

# In[ ]:




