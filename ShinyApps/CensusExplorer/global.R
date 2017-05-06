library('readxl')

#list of statecodes 
codes<-1:35
selectedTable<-function(filepath){read_excel(filepath)}