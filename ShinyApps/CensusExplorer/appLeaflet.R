library('shiny')
library('leaflet')

ui <- fluidPage(
  titlePanel("LEAFLET MAP"),
  leafletOutput("map", width = "100%", height = "100%")
)

server <- function(input,output){
  output$map <- renderLeaflet({
    leaflet() %>%
      addTiles() %>%
      setView(lng = -93.85, lat = 37.45, zoom = 6)
  })
}


shinyApp(ui = ui,server = server)