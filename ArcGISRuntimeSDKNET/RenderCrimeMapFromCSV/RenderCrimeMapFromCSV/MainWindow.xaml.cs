using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.UI;
using RenderCrimeMapFromCSV.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace RenderCrimeMapFromCSV
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string CSVFILEPATH = @"data\crimedata.csv";
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

        private void ButtonClearGraphics_Click(object sender, RoutedEventArgs e) => ResetSelection();

        private void ButtonFullExtent_Click(object sender, RoutedEventArgs e) => MainMapView.SetViewpoint(mapViewModel.Map.InitialViewpoint);

        private void ButtonLoadCrimeData_Click(object sender, RoutedEventArgs e) => LoadPointDatatoMapSetMapExtent();

        private void ButtonLoadOutageDAta_Click(object sender, RoutedEventArgs e) => LoadOutageDatatoMapSetMapExtent();

        private void CrimeTypeList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var gl = mapViewModel?.GraphicsOverlays?.FirstOrDefault();
            if (gl?.Graphics?.Count == 0)
            {
                mapViewModel.SelectedGraphicsCount = "Graphics layer is empty";
                return;
            }
            FilterGraphicsBasedonSelection(sender, gl);
        }

        private void FilterGraphicsBasedonSelection(object sender, GraphicsOverlay gl)
        {
            var selectedvalue = ((Selector)sender).SelectedValue;
            gl.ClearSelection();
            gl.Graphics.Where(x =>
                        (x.Attributes.Keys.Contains("Primary Type") == true) &&
                        (x.Attributes["Primary Type"].ToString() == selectedvalue.ToString()))
                        .ToList().ForEach(selectGraphic);

            mapViewModel.SelectedGraphicsCount = $"Found {gl.SelectedGraphics.Count().ToString()} crimes ";
        }

        private void getValuesFromCSV(string filepath, out CSVFileFromTextFieldParser readcsv, out int primartytypeindex, out GraphicsList getgraphicslist)
        {
            readcsv = new CSVFileFromTextFieldParser(filepath);
            primartytypeindex = Array.IndexOf(readcsv.RowList.First(), "Primary Type");
            getgraphicslist = new GraphicsList(readcsv.RowList);
        }

        private void Initialize()
        {
            // Create new Map with basemap
            mapViewModel = this.FindResource("MapViewModel") as MapViewModel;
            mapViewModel.SelectedGraphicsCount = "Graphics Count";
        }

        private void LoadOutageDatatoMapSetMapExtent()
        {
            //ReadExcel();
            //LoadGeometryToGraphicsList();
            //CreateGraphicsLayerFromList();
            //AddGraphicsToLayer();
        }

        private void LoadPointDatatoMapSetMapExtent()
        {
            try
            {
                //Read CSV File
                CSVFileFromTextFieldParser readcsv;
                int primartytypeindex;
                GraphicsList getgraphicslist;
                getValuesFromCSV(CSVFILEPATH, out readcsv, out primartytypeindex, out getgraphicslist);
                // Read geometry from csv file and construct graphics list with attributes
                UniqueCrimeType = new HashSet<string>(readcsv.RowList
                                                             .Skip(1)
                                                             .Select(x => x[primartytypeindex]).Distinct());
                SetCrimeTypeList();
                var goverlay = new NewGraphicsOverlayFromGraphicsList(getgraphicslist.ListofGraphics);
                AddGraphicsLayertoMap(goverlay.NewGraphicsOverlay);
                setMapInitialExtent(goverlay.GraphicsExtent);
            }
            catch (Exception)
            {
                MessageBox.Show("Failed to load points from csv file.", "Error loading data.");
                Debug.WriteLine("Failed to load points.");
            }
        }
        private void ResetSelection()
        {
            mapViewModel.SelectedGraphicsCount = String.Empty;
            mapViewModel.GraphicsOverlays.FirstOrDefault()?.ClearSelection();
        }

        private void selectGraphic(Graphic g) => g.IsSelected = true;

        private void SetCrimeTypeList() => mapViewModel.CrimeTypeList = UniqueCrimeType;

        private async void setMapInitialExtent(Viewpoint viewpoint)
        {
            await MainMapView.SetViewpointAsync(viewpoint);
            // TODO: Center
            //await MainMapView.SetViewpointCenterAsync(viewpoint.GetCenter());
            mapViewModel.Map.InitialViewpoint = viewpoint;
        }
    }
}