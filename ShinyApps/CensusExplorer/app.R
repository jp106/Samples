library('shiny')
library('readxl')
library('leaflet')


# TODO - When state is selected in the dropdown zoom to that state in map
# DONE - replace statecode in dropdown with statename
# TODO - use http://www.devinfo.org/indiacensuspopulationtotals2011/libraries/aspx/RegDataQuery.aspx
#        rest api to get data
state_codes <- read.csv("www/StateCodes.csv")
ui<-fluidPage(
  titlePanel("Indian Census Explorer by State"),
  navbarPage(title = "Explorer",
             tabPanel(title = "Data Explorer",
                      fluidRow(
                        #column(3,sliderInput(inputId = "slide1",label = "find your number", value = 50, min = 1,max = 200)),
                        #column(3,actionButton(inputId = "btnload",label = "Load")),
                        #column(3,selectInput(inputId = "selinput",label = "Select a State",choices = 1:35)),
                        column(3,selectInput(inputId = "selinput1",label = "Select a State",choices = state_codes$StateName))
                      ),
                      hr(),
                      dataTableOutput(outputId = "statestable"),
                      textOutput(outputId = "path")  
             ),
             tabPanel(title = "Map Explorer",
                      leafletOutput("map", width = "100%", height = 600))
  )
)

server<-function(input,output){
  getStateCode <- reactive({
    state_codes[state_codes$StateName==input$selinput1,1]
  })
  #xlpath<-reactive(paste("~/Documents/GithubRepos/DataDownload/Downloadedfrompython/",input$selinput,".xlsx",sep = ""))
  xlpath<-reactive(paste("www/",input$selinput,".xlsx",sep = ""))
  
  #output$path<-reactive(getStateCode())
  output$statestable<-renderDataTable(read_excel(paste("www/",getStateCode(),".xlsx",sep = "")),
                                      options = list(pageLength=10),escape = c('State Code'))
  output$map<-renderLeaflet({
    leaflet() %>% 
      addTiles() %>% 
      setView(lng = -93.85, lat = 37.45, zoom = 5)
  })
  
  #output$statemap<-renderPlot(indiadistrds)
}

shinyApp(ui = ui,server = server)
