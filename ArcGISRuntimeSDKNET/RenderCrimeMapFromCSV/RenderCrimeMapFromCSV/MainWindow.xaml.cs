using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.UI;
using RenderCrimeMapFromCSV.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace RenderCrimeMapFromCSV
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MapViewModel mapViewModel;
        private HashSet<string> UniqueCrimeType;

        public MainWindow()
        {
            InitializeComponent();
            Initialize();
        }

        private void AddGraphicsLayertoMap(GraphicsOverlay layer)
        {
            layer.SelectionColor = Colors.Red;
            mapViewModel.AddGraphicsOverlay(layer);
        }

        private void ButtonClearGraphics_Click(object sender, RoutedEventArgs e)
        {
            ResetSelection();
        }

        private void ButtonFullExtent_Click(object sender, RoutedEventArgs e)
        {
            MainMapView.SetViewpoint(mapViewModel.Map.InitialViewpoint);
        }

        private void CrimeTypeList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var gl = mapViewModel.GraphicsOverlays.First();
            if (gl.Graphics?.Count == 0)
            {
                mapViewModel.SelectedGraphicsCount = "Graphics layer is empty";
                return;
            }
            FilterGraphicsBasedonSelection(sender, gl);
        }

        private void FilterGraphicsBasedonSelection(object sender, GraphicsOverlay gl)
        {
            var selectedvalue = ((System.Windows.Controls.Primitives.Selector)sender).SelectedValue;
            gl.ClearSelection();
            gl.Graphics.Where(x =>
                        (x.Attributes.Keys.Contains("Primary Type") == true) &&
                        (x.Attributes["Primary Type"].ToString() == selectedvalue.ToString()))
                        .ToList().ForEach(selectGraphic);

            mapViewModel.SelectedGraphicsCount = String.Format("Found {0} crimes ",
                gl.SelectedGraphics.Count().ToString());
        }

        private void Initialize()
        {
            // Create new Map with basemap
            mapViewModel = this.FindResource("MapViewModel") as MapViewModel;
            mapViewModel.SelectedGraphicsCount = "Second label";
        }

        private void LoadPointDatatoMapSetMapExtent()
        {
            try
            {
                var filepath = @"data\crimedata.csv";
                //Read CSV File
                var readcsv = new CSVFileFromTextFieldParser(filepath);
                var primartytypeindex = Array.IndexOf(readcsv.RowList.First(), "Primary Type");
                var getgraphicslist = new GraphicsList(readcsv.RowList);
                // Read geometry from csv file and construct graphics list with attributes
                UniqueCrimeType = new HashSet<string>(readcsv.RowList
                                                             .Skip(1)
                                                             .Select(x => x[primartytypeindex]).Distinct());
                // Use readcsv.CSVRowList to use regex expression
                // var graphicslist = ConstructGraphicsList(readcsv.CSVRowList);

                // Use readcsv.RowList to use TextFieldParser
                var graphicslist = getgraphicslist.ListofGraphics;

                SetCrimeTypeList();
                var goverlay = new NewGraphicsOverlayFromGraphicsList(graphicslist);
                AddGraphicsLayertoMap(goverlay.NewGraphicsOverlay);
                setMapInitialExtent(goverlay.GraphicsExtent);
            }
            catch (Exception)
            {
                MessageBox.Show("Failed to load points from csv file.", "Error loading data.");
                Console.WriteLine("Failed to load points.");
            }
        }

        
        private void ResetSelection()
        {
            mapViewModel.SelectedGraphicsCount = String.Empty;
            mapViewModel.GraphicsOverlays.First().ClearSelection();
        }

        private void selectGraphic(Graphic g)
        {
            g.IsSelected = true;
        }

        private void SetCrimeTypeList()
        {
            mapViewModel.CrimeTypeList = UniqueCrimeType;
        }

        private async void setMapInitialExtent(Viewpoint viewpoint)
        {
            await MainMapView.SetViewpointAsync(viewpoint);

            //await MainMapView.SetViewpointCenterAsync(viewpoint.GetCenter());
            mapViewModel.Map.InitialViewpoint = viewpoint;
        }

        private void ButtonLoadCrimeData_Click(object sender, RoutedEventArgs e)
        {
            LoadPointDatatoMapSetMapExtent();

        }

        private void ButtonLoadOutageDAta_Click(object sender, RoutedEventArgs e)
        {
            LoadOutageDatatoMapSetMapExtent();
        }

        private void LoadOutageDatatoMapSetMapExtent()
        {
            //ReadExcel();
            //LoadGeometryToGraphicsList();
            //CreateGraphicsLayerFromList();
            //AddGraphicsToLayer();
        }
    }
}