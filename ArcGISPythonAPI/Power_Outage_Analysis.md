
## Based on 15 years (2002-2017) of power outage data, predict highest outage event type in 2018/2017, before an electric disturbance is reported so as to prevent aftermath effects and prepared and have minimal effects. 

Develop a prediction model in Python for power outage detection using supervised learning. 


### 1. Create empty dataframe object




```python
import pandas as pd
import calendar

combined = pd.DataFrame()
```

## 2. Download Electric disturbances annual summaries from [here](http://www.oe.netl.doe.gov/OE417_annual_summary.aspx)

Note: You can download clean data from [here (Google Spreadsheet)](http://insideenergy.org/2014/08/18/data-explore-15-years-of-power-outages/) for 2002-2014 or [CSV](https://docs.google.com/spreadsheets/d/1AdxhulfM9jeqviIZihuODqk7HoS1kRUlM_afIKXAjXQ/edit#gid=595041757). 

## 3. Check for data inconsistencies



#### Note: When reading file with read_excel this message is displayed:  WARNING:  *** file size (48323) not 512 + multiple of sector size (512).
To avoid this warning, open downloaded files in MS Excel, enable editing and save.


```python
for year in range(2002, 2018):
        df = ""        
        file_path = "C:\downloadedfrompython\{0}_Annual_Summary.xls".format(year)
        try:
            df = pd.read_excel(file_path)
            
            print ("{0} : {1}, isnull: {2}".format(year, len(df.columns), 
                                                   df.isnull().values.any()))
        except Exception as e:
            print(e)        
```

    2002 : 8, isnull: True
    2003 : 8, isnull: True
    2004 : 8, isnull: True
    2005 : 8, isnull: True
    2006 : 8, isnull: True
    2007 : 8, isnull: True
    2008 : 8, isnull: True
    2009 : 8, isnull: True
    2010 : 8, isnull: True
    2011 : 9, isnull: True
    2012 : 9, isnull: True
    2013 : 9, isnull: True
    2014 : 9, isnull: True
    2015 : 11, isnull: True
    2016 : 11, isnull: True
    2017 : 11, isnull: False
    

Note: The column number and null data varies by year. So group files by same number of columns and prepare excel file accordingly.

# 3. Clean Data to make it conistent

Based on the number of columns from above files will be grouped into :
* 2002 to 2010
* 2011 to 2014
* 2015 to 2017





### 3.1 Clean data from 2002 - 2010

### Observe data


```python
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
```

    2002 : 8, isnull: True
    Index(['Date', 'NERC Region', 'Time', 'Area', 'Type of Disturbance',
           'Loss (megawatts)', 'Number of Customers Affected', 'Restoration Time'],
          dtype='object')
    2003 : 8, isnull: True
    Index(['Date', 'NERC Region', 'Time', 'Area Affected', 'Type of Disturbance',
           'Loss (megawatts)', 'Number of Customers Affected 1', 'Restoration'],
          dtype='object')
    2004 : 8, isnull: True
    Index(['Date', 'NERC Region', 'Time', 'Area Affected', 'Type of Disturbance',
           'Loss (megawatts)', 'Number of Customers Affected 1', 'Restoration'],
          dtype='object')
    2005 : 8, isnull: True
    Index(['Date', 'NERC Region', 'Time', 'Area Affected', 'Type of Disturbance',
           'Loss (megawatts)', 'Number of Customers Affected 1', 'Restoration'],
          dtype='object')
    2006 : 8, isnull: True
    Index(['Date', 'NERC Region', 'Time', 'Area Affected', 'Type of Disturbance',
           'Loss (megawatts)', 'Number of Customers Affected 1', 'Restoration'],
          dtype='object')
    2007 : 8, isnull: True
    Index(['Date', ' NERC Region', 'Time', 'Area Affected', 'Type of Disturbance',
           'Loss (megawatts)', 'Number of Customers Affected 1[1]', 'Restoration'],
          dtype='object')
    2008 : 8, isnull: True
    Index(['Table B.2.  Major Disturbances and Unusual Occurrences, Year-to-Date through December 2008',
           'Unnamed: 1', 'Unnamed: 2', 'Unnamed: 3', 'Unnamed: 4', 'Unnamed: 5',
           'Unnamed: 6', 'Unnamed: 7'],
          dtype='object')
    2009 : 8, isnull: True
    Index(['Date', 'NERC Region', 'Time', 'Area Affected', 'Type of Disturbance',
           'Loss (megawatts)', 'Number of Customers Affected 1', 'Restoration'],
          dtype='object')
    2010 : 8, isnull: True
    Index(['Date', 'NERC Region', 'Time', 'Area Affected', 'Type of Disturbance',
           'Loss (megawatts)', 'Number of Customers Affected 1', 'Restoration'],
          dtype='object')
    

### Observations
1. Column 'NERC Region' is consistent among all years. However, in 2007 the column name is  ' NERC Region', notice the trailing space at the beginning. This will be avoided by setting 'names' property to a standard list of column names. 
2. In 2008 the first two rows must be skipped because of unnecessary text. 
3. Rest of the years only first row is skipped hence header index - 1

### Steps to clean data:

1. Read excel file with the following attributes 
    1. Set header to 2 if year = 2008 otherwise 1.  
    2. Set names  to a list of 8 column names  
2. Drop rows that don't have 'Type of Disturbance'
3. Drop columns that have all NaN  
4. Optional: add new column 'Restoration Date' to split 'Restoration Time' into date and time



```python
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
```

    (24, 8)
    (24, 8)
    2002 : 8, isnull: True
    Index(['Date Event Began', 'NERC Region', 'Time Event Began', 'Area affected',
           'Type of Disturbance', 'Demand Loss(MW)', 'Customers affected',
           'Restoration Time'],
          dtype='object')
    (64, 8)
    (64, 8)
    2003 : 8, isnull: True
    Index(['Date Event Began', 'NERC Region', 'Time Event Began', 'Area affected',
           'Type of Disturbance', 'Demand Loss(MW)', 'Customers affected',
           'Restoration Time'],
          dtype='object')
    (97, 8)
    (97, 8)
    2004 : 8, isnull: False
    Index(['Date Event Began', 'NERC Region', 'Time Event Began', 'Area affected',
           'Type of Disturbance', 'Demand Loss(MW)', 'Customers affected',
           'Restoration Time'],
          dtype='object')
    (89, 8)
    (89, 8)
    2005 : 8, isnull: True
    Index(['Date Event Began', 'NERC Region', 'Time Event Began', 'Area affected',
           'Type of Disturbance', 'Demand Loss(MW)', 'Customers affected',
           'Restoration Time'],
          dtype='object')
    (94, 8)
    (94, 8)
    2006 : 8, isnull: True
    Index(['Date Event Began', 'NERC Region', 'Time Event Began', 'Area affected',
           'Type of Disturbance', 'Demand Loss(MW)', 'Customers affected',
           'Restoration Time'],
          dtype='object')
    (80, 8)
    (80, 8)
    2007 : 8, isnull: True
    Index(['Date Event Began', 'NERC Region', 'Time Event Began', 'Area affected',
           'Type of Disturbance', 'Demand Loss(MW)', 'Customers affected',
           'Restoration Time'],
          dtype='object')
    (154, 8)
    (154, 8)
    2008 : 8, isnull: True
    Index(['Date Event Began', 'NERC Region', 'Time Event Began', 'Area affected',
           'Type of Disturbance', 'Demand Loss(MW)', 'Customers affected',
           'Restoration Time'],
          dtype='object')
    (99, 8)
    (99, 8)
    2009 : 8, isnull: True
    Index(['Date Event Began', 'NERC Region', 'Time Event Began', 'Area affected',
           'Type of Disturbance', 'Demand Loss(MW)', 'Customers affected',
           'Restoration Time'],
          dtype='object')
    (126, 8)
    (126, 8)
    2010 : 8, isnull: True
    Index(['Date Event Began', 'NERC Region', 'Time Event Began', 'Area affected',
           'Type of Disturbance', 'Demand Loss(MW)', 'Customers affected',
           'Restoration Time'],
          dtype='object')
    


```python
combined_1.shape
```




    (827, 8)




```python
combined_1.head()

```




<div>
<style>
    .dataframe thead tr:only-child th {
        text-align: right;
    }

    .dataframe thead th {
        text-align: left;
    }

    .dataframe tbody tr th {
        vertical-align: top;
    }
</style>
<table border="1" class="dataframe">
  <thead>
    <tr style="text-align: right;">
      <th></th>
      <th>Date Event Began</th>
      <th>NERC Region</th>
      <th>Time Event Began</th>
      <th>Area affected</th>
      <th>Type of Disturbance</th>
      <th>Demand Loss(MW)</th>
      <th>Customers affected</th>
      <th>Restoration Time</th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <th>0</th>
      <td>2002-01-30 00:00:00</td>
      <td>SPP</td>
      <td>06:00:00</td>
      <td>Oklahoma</td>
      <td>Ice Storm</td>
      <td>500</td>
      <td>1881134</td>
      <td>2002-02-07 12:00:00</td>
    </tr>
    <tr>
      <th>1</th>
      <td>2002-01-29 00:00:00</td>
      <td>SPP</td>
      <td>Evening</td>
      <td>Metropolitan Kansas City Area</td>
      <td>Ice Storm</td>
      <td>500-600</td>
      <td>270000</td>
      <td>NaN</td>
    </tr>
    <tr>
      <th>2</th>
      <td>2002-01-30 00:00:00</td>
      <td>SPP</td>
      <td>16:00:00</td>
      <td>Missouri</td>
      <td>Ice Storm</td>
      <td>210</td>
      <td>95000</td>
      <td>2002-02-10 21:00:00</td>
    </tr>
    <tr>
      <th>3</th>
      <td>2002-02-27 00:00:00</td>
      <td>WSCC</td>
      <td>10:48:00</td>
      <td>California</td>
      <td>Interruption of Firm Load</td>
      <td>300</td>
      <td>255000</td>
      <td>2002-02-27 11:35:00</td>
    </tr>
    <tr>
      <th>4</th>
      <td>2002-03-09 00:00:00</td>
      <td>ECAR</td>
      <td>00:00:00</td>
      <td>Lower Peninsula of Michigan</td>
      <td>Severe Weather</td>
      <td>190</td>
      <td>190000</td>
      <td>2002-03-11 12:00:00</td>
    </tr>
  </tbody>
</table>
</div>



### 3.2 Clean data from 2011 - 2014

### Observe data


```python
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
```

    2011 : 9, isnull: True
    Index(['Date Event Began', 'Time Event Began', 'Date of Restoration',
           'Time of Restoration', 'Area Affected', 'NERC Region', 'Event Type',
           'Demand Loss (MW)', 'Number of Customers Affected'],
          dtype='object')
    2012 : 9, isnull: True
    Index(['Date Event Began', 'Time Event Began', 'Date of Restoration',
           'Time of Restoration', 'Area Affected', 'NERC Region', 'Event Type',
           'Demand Loss (MW)', 'Number of Customers Affected'],
          dtype='object')
    2013 : 9, isnull: True
    Index(['Date Event Began', 'Time Event Began', 'Date of Restoration',
           'Time of Restoration', 'Area Affected', 'NERC Region', 'Event Type',
           'Demand Loss (MW)', 'Number of Customers Affected'],
          dtype='object')
    2014 : 9, isnull: True
    Index(['Date Event Began', 'Time Event Began', 'Date of Restoration',
           'Time of Restoration', 'Area Affected', 'NERC Region', 'Event Type',
           'Demand Loss (MW)', 'Number of Customers Affected'],
          dtype='object')
    

### Observations

1. This group has 9 columns, which is what we're aiming for
2. No missing or unecessary columns
3. Data looks clean

## Steps to clean data:

1. Read excel file with the following attributes 
    1. Set header to 1  
    2. Set names  to a list of 9 column names  
2. Drop rows that don't have 'Type of Disturbance'
3. Drop columns that have all NaN  


```python
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
```

    (307, 9)
    (307, 9)
    2011 : 9, isnull: True
    Index(['Date Event Began', 'Time Event Began', 'Restoration Date',
           'Restoration Time', 'Area affected', 'NERC Region',
           'Type of Disturbance', 'Demand Loss(MW)', 'Customers affected'],
          dtype='object')
    (203, 9)
    (203, 9)
    2012 : 9, isnull: True
    Index(['Date Event Began', 'Time Event Began', 'Restoration Date',
           'Restoration Time', 'Area affected', 'NERC Region',
           'Type of Disturbance', 'Demand Loss(MW)', 'Customers affected'],
          dtype='object')
    (174, 9)
    (174, 9)
    2013 : 9, isnull: True
    Index(['Date Event Began', 'Time Event Began', 'Restoration Date',
           'Restoration Time', 'Area affected', 'NERC Region',
           'Type of Disturbance', 'Demand Loss(MW)', 'Customers affected'],
          dtype='object')
    (214, 9)
    (214, 9)
    2014 : 9, isnull: True
    Index(['Date Event Began', 'Time Event Began', 'Restoration Date',
           'Restoration Time', 'Area affected', 'NERC Region',
           'Type of Disturbance', 'Demand Loss(MW)', 'Customers affected'],
          dtype='object')
    


```python
combined_2.shape
```




    (898, 9)




```python
combined_2.tail()
```




<div>
<style>
    .dataframe thead tr:only-child th {
        text-align: right;
    }

    .dataframe thead th {
        text-align: left;
    }

    .dataframe tbody tr th {
        vertical-align: top;
    }
</style>
<table border="1" class="dataframe">
  <thead>
    <tr style="text-align: right;">
      <th></th>
      <th>Date Event Began</th>
      <th>Time Event Began</th>
      <th>Restoration Date</th>
      <th>Restoration Time</th>
      <th>Area affected</th>
      <th>NERC Region</th>
      <th>Type of Disturbance</th>
      <th>Demand Loss(MW)</th>
      <th>Customers affected</th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <th>893</th>
      <td>2014-12-11 00:00:00</td>
      <td>16:05:00</td>
      <td>2014-12-11 00:00:00</td>
      <td>21:00:00</td>
      <td>Portland, Oregon</td>
      <td>WECC</td>
      <td>Severe Weather- High Winds</td>
      <td>250</td>
      <td>85470</td>
    </tr>
    <tr>
      <th>894</th>
      <td>2014-12-11 00:00:00</td>
      <td>17:00:00</td>
      <td>2014-12-12 00:00:00</td>
      <td>10:00:00</td>
      <td>Kitsap, Thurston, Whatcom counties Washington</td>
      <td>WECC</td>
      <td>Severe Weather- High Winds</td>
      <td>116</td>
      <td>264000</td>
    </tr>
    <tr>
      <th>895</th>
      <td>2014-12-17 00:00:00</td>
      <td>11:00:00</td>
      <td>2014-12-17 00:00:00</td>
      <td>12:15:00</td>
      <td>Washington</td>
      <td>WECC</td>
      <td>Suspected Physical Attack</td>
      <td>Unknown</td>
      <td>Unknown</td>
    </tr>
    <tr>
      <th>896</th>
      <td>2014-12-30 00:00:00</td>
      <td>15:50:00</td>
      <td>2014-12-31 00:00:00</td>
      <td>11:00:00</td>
      <td>New Hampshire, Massachusetts, Maine, Rhode Isl...</td>
      <td>NPCC</td>
      <td>Suspected Cyber Attack</td>
      <td>Unknown</td>
      <td>Unknown</td>
    </tr>
    <tr>
      <th>897</th>
      <td>2014-12-30 00:00:00</td>
      <td>13:08:00</td>
      <td>2015-01-01 00:00:00</td>
      <td>16:50:00</td>
      <td>Northern California</td>
      <td>WECC</td>
      <td>Severe Weather- High Winds</td>
      <td>127</td>
      <td>84500</td>
    </tr>
  </tbody>
</table>
</div>



### 3.3 Clean data from 2015 - 2017

#### Observe data


```python
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
```

    2015 : 11, isnull: True
    Index(['Month', 'Date Event Began', 'Time Event Began', 'Date of Restoration',
           'Time of Restoration', 'Area Affected', 'NERC Region', 'Alert Criteria',
           'Event Type', 'Demand Loss (MW)', 'Number of Customers Affected'],
          dtype='object')
    2016 : 11, isnull: True
    Index(['Month', 'Date Event Began', 'Time Event Began', 'Date of Restoration',
           'Time of Restoration', 'Area Affected', 'NERC Region', 'Alert Criteria',
           'Event Type', 'Demand Loss (MW)', 'Number of Customers Affected'],
          dtype='object')
    2017 : 11, isnull: False
    Index(['Month', 'Date Event Began', 'Time Event Began', 'Date of Restoration',
           'Time of Restoration', 'Area Affected', 'NERC Region', 'Alert Criteria',
           'Event Type', 'Demand Loss (MW)', 'Number of Customers Affected'],
          dtype='object')
    

### Observations 

1. This group has 11 columns. 2 additional columns 'Month' and 'Alert Criteria' can be dropped.

### Steps to clean data:

1. Read excel file with the following attributes 
    1. Set header to 1
    2. Parse columns other than 'Month' and 'Alert Criteria' 
    3. Set names  to a list of 9 column names
2. Drop rows that don't have 'Type of Disturbance'
3. Drop columns that have all NaN


```python
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
```

    (143, 9)
    (143, 9)
    2015 : 9, isnull: True
    Index(['Date Event Began', 'Time Event Began', 'Restoration Date',
           'Restoration Time', 'Area affected', 'NERC Region',
           'Type of Disturbance', 'Demand Loss(MW)', 'Customers affected'],
          dtype='object')
    (141, 9)
    (141, 9)
    2016 : 9, isnull: True
    Index(['Date Event Began', 'Time Event Began', 'Restoration Date',
           'Restoration Time', 'Area affected', 'NERC Region',
           'Type of Disturbance', 'Demand Loss(MW)', 'Customers affected'],
          dtype='object')
    (49, 9)
    (49, 9)
    2017 : 9, isnull: False
    Index(['Date Event Began', 'Time Event Began', 'Restoration Date',
           'Restoration Time', 'Area affected', 'NERC Region',
           'Type of Disturbance', 'Demand Loss(MW)', 'Customers affected'],
          dtype='object')
    


```python
combined_3.shape
```




    (333, 9)




```python
combined_3.head(1)
```




<div>
<style>
    .dataframe thead tr:only-child th {
        text-align: right;
    }

    .dataframe thead th {
        text-align: left;
    }

    .dataframe tbody tr th {
        vertical-align: top;
    }
</style>
<table border="1" class="dataframe">
  <thead>
    <tr style="text-align: right;">
      <th></th>
      <th>Date Event Began</th>
      <th>Time Event Began</th>
      <th>Restoration Date</th>
      <th>Restoration Time</th>
      <th>Area affected</th>
      <th>NERC Region</th>
      <th>Type of Disturbance</th>
      <th>Demand Loss(MW)</th>
      <th>Customers affected</th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <th>0</th>
      <td>2015-01-07 00:00:00</td>
      <td>17:00:00</td>
      <td>2015-01-08 00:00:00</td>
      <td>08:35:00</td>
      <td>Tennessee</td>
      <td>SERC</td>
      <td>Severe Weather - Winter</td>
      <td>Unknown</td>
      <td>Unknown</td>
    </tr>
  </tbody>
</table>
</div>



## 4. Merge data from 2002-2017


```python
combined = pd.DataFrame()
combined = combined.append(combined_1,ignore_index=True)
combined = combined.append(combined_2,ignore_index=True)
combined = combined.append(combined_3,ignore_index=True)
combined.shape
```




    (2058, 9)




```python
combined.head()
```




<div>
<style>
    .dataframe thead tr:only-child th {
        text-align: right;
    }

    .dataframe thead th {
        text-align: left;
    }

    .dataframe tbody tr th {
        vertical-align: top;
    }
</style>
<table border="1" class="dataframe">
  <thead>
    <tr style="text-align: right;">
      <th></th>
      <th>Area affected</th>
      <th>Customers affected</th>
      <th>Date Event Began</th>
      <th>Demand Loss(MW)</th>
      <th>NERC Region</th>
      <th>Restoration Date</th>
      <th>Restoration Time</th>
      <th>Time Event Began</th>
      <th>Type of Disturbance</th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <th>0</th>
      <td>Oklahoma</td>
      <td>1881134</td>
      <td>2002-01-30 00:00:00</td>
      <td>500</td>
      <td>SPP</td>
      <td>NaN</td>
      <td>2002-02-07 12:00:00</td>
      <td>06:00:00</td>
      <td>Ice Storm</td>
    </tr>
    <tr>
      <th>1</th>
      <td>Metropolitan Kansas City Area</td>
      <td>270000</td>
      <td>2002-01-29 00:00:00</td>
      <td>500-600</td>
      <td>SPP</td>
      <td>NaN</td>
      <td>NaN</td>
      <td>Evening</td>
      <td>Ice Storm</td>
    </tr>
    <tr>
      <th>2</th>
      <td>Missouri</td>
      <td>95000</td>
      <td>2002-01-30 00:00:00</td>
      <td>210</td>
      <td>SPP</td>
      <td>NaN</td>
      <td>2002-02-10 21:00:00</td>
      <td>16:00:00</td>
      <td>Ice Storm</td>
    </tr>
    <tr>
      <th>3</th>
      <td>California</td>
      <td>255000</td>
      <td>2002-02-27 00:00:00</td>
      <td>300</td>
      <td>WSCC</td>
      <td>NaN</td>
      <td>2002-02-27 11:35:00</td>
      <td>10:48:00</td>
      <td>Interruption of Firm Load</td>
    </tr>
    <tr>
      <th>4</th>
      <td>Lower Peninsula of Michigan</td>
      <td>190000</td>
      <td>2002-03-09 00:00:00</td>
      <td>190</td>
      <td>ECAR</td>
      <td>NaN</td>
      <td>2002-03-11 12:00:00</td>
      <td>00:00:00</td>
      <td>Severe Weather</td>
    </tr>
  </tbody>
</table>
</div>


