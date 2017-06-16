
# coding: utf-8

# ## Based on 15 years (2002-2017) of power outage data, predict highest outage event type in 2018/2017, before an electric disturbance is reported so as to prevent aftermath effects and prepared and have minimal effects. 
# 
# Develop a prediction model in Python for power outage detection using supervised learning. 

# 
# ### 1. Create empty dataframe object
# 
# 

# In[164]:

import pandas as pd
import calendar

combined = pd.DataFrame()


# ## 2. Download Electric disturbances annual summaries from [here](http://www.oe.netl.doe.gov/OE417_annual_summary.aspx)
# 
# Note: You can download clean data from [here (Google Spreadsheet)](http://insideenergy.org/2014/08/18/data-explore-15-years-of-power-outages/) for 2002-2014 or [CSV](https://docs.google.com/spreadsheets/d/1AdxhulfM9jeqviIZihuODqk7HoS1kRUlM_afIKXAjXQ/edit#gid=595041757). 

# ## 3. Check for data inconsistencies
# 
# 

# #### Note: When reading file with read_excel this message is displayed:  WARNING:  *** file size (48323) not 512 + multiple of sector size (512).
# To avoid this warning, open downloaded files in MS Excel, enable editing and save.

# In[133]:

for year in range(2002, 2018):
        df = ""        
        file_path = "C:\downloadedfrompython\{0}_Annual_Summary.xls".format(year)
        try:
            df = pd.read_excel(file_path)
            
            print ("{0} : {1}, isnull: {2}".format(year, len(df.columns), 
                                                   df.isnull().values.any()))
        except Exception as e:
            print(e)        


# Note: The column number and null data varies by year. So group files by same number of columns and prepare excel file accordingly.

# # 3. Clean Data to make it conistent

# Based on the number of columns from above files will be grouped into :
# * 2002 to 2010
# * 2011 to 2014
# * 2015 to 2017
# 
# 
# 
# 

# ### 3.1 Clean data from 2002 - 2010

# ### Observe data

# In[359]:

for year in range(2002, 2011):
        df = ""        
        file_path = "C:\downloadedfrompython\{0}_Annual_Summary.xls".format(year)
        try:
            df = pd.read_excel(file_path, header=1)
            
            print ("{0} : {1}, isnull: {2}".format(year, len(df.columns), 
                                                   df.isnull().values.any()))
            print (df.columns)
        except Exception as e:
            print(e)        


# ### Observations
# 1. Column 'NERC Region' is consistent among all years. However, in 2007 the column name is  ' NERC Region', notice the trailing space at the beginning. This will be avoided by setting 'names' property to a standard list of column names. 
# 2. In 2008 the first two rows must be skipped because of unnecessary text. 
# 3. Rest of the years only first row is skipped hence header index - 1

# ### Steps to clean data:
# 
# 1. Read excel file with the following attributes 
#     1. Set header to 2 if year = 2008 otherwise 1.  
#     2. Set names  to a list of 8 column names  
# 2. Drop rows that don't have 'Type of Disturbance'
# 3. Drop columns that have all NaN  
# 4. Optional: add new column 'Restoration Date' to split 'Restoration Time' into date and time
# 

# In[335]:

combined_1 = pd.DataFrame()
for year in range(2002, 2011):
        df1 = ""        
        file_path = "C:\downloadedfrompython\{0}_Annual_Summary.xls".format(year)
        try:
            h = 1
            if year == 2008:
                h = 2
            df1 = pd.read_excel(file_path,header=h, 
                               names=['Date Event Began', 'NERC Region',
                                     'Time Event Began', 'Area affected', 
                                     'Disturbance Type', 'Demand Loss(MW)',
                                     'Customers affected', 'Restoration Time'])
            # Drop rows where column 'Type of Disturbance' is NaN
            df1 = df1.dropna(axis=0,how='all',subset=['Type of Disturbance'])
            print (df1.shape)
            df1 = df1.dropna(axis=1,how='all')
            print (df1.shape)
            print ("{0} : {1}, isnull: {2}".format(year, len(df1.columns), 
                                                   df1.isnull().values.any()))
            # Add column Restoration Date to the dataframe
            df1['Restoration Date'] = ''
            print(df1.columns)
        except Exception as e:
            print(e)        
        combined_1 = combined_1.append(df1, ignore_index=True)        


# In[336]:

combined_1.shape


# In[289]:

combined_1.head()


# ### 3.2 Clean data from 2011 - 2014

# ### Observe data

# In[360]:

for year in range(2011, 2015):
        df = ""        
        file_path = "C:\downloadedfrompython\{0}_Annual_Summary.xls".format(year)
        try:
            df = pd.read_excel(file_path, header=1)
            
            print ("{0} : {1}, isnull: {2}".format(year, len(df.columns), 
                                                   df.isnull().values.any()))
            print (df.columns)
        except Exception as e:
            print(e)        


# ### Observations
# 
# 1. This group has 9 columns, which is what we're aiming for
# 2. No missing or unecessary columns
# 3. Data looks clean

# ## Steps to clean data:
# 
# 1. Read excel file with the following attributes 
#     1. Set header to 1  
#     2. Set names  to a list of 9 column names  
# 2. Drop rows that don't have 'Type of Disturbance'
# 3. Drop columns that have all NaN  

# In[333]:

combined_2 = pd.DataFrame()
for year in range(2011, 2015):
        df2 = ""        
        path = "C:\downloadedfrompython\{0}_Annual_Summary.xls".format(year)        
        try:
            df2 = pd.read_excel(path, header=1,
                               names=['Date Event Began', 
                                     'Time Event Began',
                                      'Restoration Date','Restoration Time',
                                      'Area affected', 'NERC Region',
                                     'Disturbance Type', 'Demand Loss(MW)',
                                     'Customers affected'])
            # Drop rows where column 'Type of Disturbance' is NaN
            df2 = df2.dropna(axis=0,how='all',subset=['Type of Disturbance'])
            print (df2.shape)
            df2 = df2.dropna(axis=1,how='all')
            print (df2.shape)
            print ("{0} : {1}, isnull: {2}".format(year, len(df2.columns), 
                                                   df2.isnull().values.any()))
            print(df2.columns)
        except Exception as e:
            print(e)        
        combined_2 = combined_2.append(df2, ignore_index=True)        


# In[334]:

combined_2.shape


# In[308]:

combined_2.tail()


# ### 3.3 Clean data from 2015 - 2017

# #### Observe data

# In[355]:

for year in range(2015, 2018):
        df = ""        
        file_path = "C:\downloadedfrompython\{0}_Annual_Summary.xls".format(year)
        try:
            df = pd.read_excel(file_path,header=1)
            
            print ("{0} : {1}, isnull: {2}".format(year, len(df.columns), 
                                                   df.isnull().values.any()))
            print(df.columns)
        except Exception as e:
            print(e)        


# ### Observations 
# 
# 1. This group has 11 columns. 2 additional columns 'Month' and 'Alert Criteria' can be dropped.

# ### Steps to clean data:
# 
# 1. Read excel file with the following attributes 
#     1. Set header to 1
#     2. Parse columns other than 'Month' and 'Alert Criteria' 
#     3. Set names  to a list of 9 column names
# 2. Drop rows that don't have 'Type of Disturbance'
# 3. Drop columns that have all NaN

# In[337]:

combined_3 = pd.DataFrame()
for year in range(2015, 2018):
        df3 = ""        
        path = "C:\downloadedfrompython\{0}_Annual_Summary.xls".format(year)        
        try:
            df3 = pd.read_excel(path, header=1,parse_cols='B:G,I:K',
                               names=['Date Event Began', 
                                     'Time Event Began',
                                      'Restoration Date','Restoration Time',
                                      'Area affected', 'NERC Region',
                                     'Disturbance Type', 'Demand Loss(MW)',
                                     'Customers affected'])
            # Drop rows where column 'Type of Disturbance' is NaN
            df3 = df3.dropna(axis=0,how='all',subset=['Type of Disturbance'])
            print (df3.shape)
            df2 = df3.dropna(axis=1,how='all')
            print (df3.shape)
            print ("{0} : {1}, isnull: {2}".format(year, len(df3.columns), 
                                                   df3.isnull().values.any()))
            print(df3.columns)
        except Exception as e:
            print(e)        
        combined_3 = combined_3.append(df3, ignore_index=True)        


# In[340]:

combined_3.shape


# In[341]:

combined_3.head(1)


# ## 4. Merge data from 2002-2017

# In[347]:

combined = pd.DataFrame()
combined = combined.append(combined_1,ignore_index=True)
combined = combined.append(combined_2,ignore_index=True)
combined = combined.append(combined_3,ignore_index=True)
combined.shape


# In[350]:

combined.head()

