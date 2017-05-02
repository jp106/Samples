using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.UI;
using RenderCrimeMapFromCSV.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;

namespace RenderCrimeMapFromCSV
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private HashSet<string> UniqueCrimeType;
        private MapViewModel mapViewModel;

        public MainWindow()
        {
            InitializeComponent();
            Initialize();
            LoadPointDatatoMapSetMapExtent();
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

        private IList<Graphic> ConstructGraphicsList(IList<string> rowlist)
        {
            var graphics = new List<Graphic>();
            var columns = rowlist[0].Split(',');
            rowlist.Skip(1)
                .ToList()
                .ForEach(x => graphics.Add(ConstructNewGraphicfromRow(x, columns)));
            return graphics;
        }

        private Graphic ConstructNewGraphic(string latitude, string longitude, List<KeyValuePair<string, object>> attributes)
        {
            IList<Graphic> graphics = new List<Graphic>();
            double parse;
            if (Double.TryParse(latitude, out parse) && double.TryParse(longitude.ToString(), out parse))
            {
                var longi = Convert.ToDouble(longitude);
                var lat = Convert.ToDouble(latitude);
                MapPoint p = new MapPoint(longi, lat, SpatialReferences.Wgs84);
                return new Graphic(p, attributes);
            }
            return new Graphic();
        }

        private Graphic ConstructNewGraphicfromRow(string row, IList<string> columns)
        {
            var attributesKeyValue = new List<KeyValuePair<string, object>>();
            // find indices of geometry columns
            int latindex = columns.IndexOf("Latitude");
            int longindex = columns.IndexOf("Longitude");
            int crimetypeindex = columns.IndexOf("Primary Type");

            //clear attributes list
            attributesKeyValue.Clear();
            var attributesList = UseRegextoSplitrow(row);
            Console.WriteLine(attributesList[0]);

            //TODO: Handle empty values in UseRegextoSplitrow
            if (columns.Count > attributesList.Count)
                return new Graphic();

            //build attribute objects
            attributesList.Select((x, i) => new { Name = (i < columns.Count) ? columns[i] : "Uknown", Value = x })
                .ToList()
                .ForEach(x => attributesKeyValue.Add(new KeyValuePair<string, object>(x.Name, x.Value)));

            UniqueCrimeType.Add(attributesList[crimetypeindex].ToString());

            var latitude = attributesList[latindex];
            var longitude = attributesList[longindex];
            return ConstructNewGraphic(latitude, longitude, attributesKeyValue);
        }

        private GraphicsOverlay CreateGraphicsOverlayfromGraphicsList(IList<Graphic> graphics)
        {
            var graphicslayer = new GraphicsOverlay();
            graphicslayer.IsPopupEnabled = true;
            graphics.ToList().ForEach(x => graphicslayer.Graphics.Add(x));
            graphicslayer.Renderer = setsymbology();
            setMapGraphicsLayerExtent(graphics.Where(x => x.Geometry != null).Select(x => (MapPoint)(x.Geometry)).ToList());
            return graphicslayer;
        }

        private void CrimeTypeList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

            var gl = mapViewModel.GraphicsOverlays.First();
            if (gl.Graphics.Count == 0)
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
            mapViewModel.Map.Basemap = Basemap.CreateImageryWithLabels();
            mapViewModel.SelectedGraphicsCount = "Second label";

        }

        private void LoadPointDatatoMapSetMapExtent()
        {
            try
            {
                UniqueCrimeType = new HashSet<string>();
                var filepath = @"data\crimedata.csv";
                //Read CSV File
                var readcsv = new ReadCSVFile(filepath);

                //Read geometry from csv file and construct graphics list with attributes
                var graphicslist = ConstructGraphicsList(readcsv.CSVRowList);
                SetCrimeTypeList();
                AddGraphicsLayertoMap(CreateGraphicsOverlayfromGraphicsList(graphicslist));
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
            CrimeTypeList.ItemsSource = UniqueCrimeType;
        }

        private async void setMapGraphicsLayerExtent(IEnumerable<MapPoint> points)
        {
            PolygonBuilder pb = new PolygonBuilder(points, SpatialReferences.Wgs84);
            var viewpoint = new Viewpoint(pb.Extent);
            await MainMapView.SetViewpointAsync(viewpoint);

            await MainMapView.SetViewpointCenterAsync(pb.Extent.GetCenter());
            mapViewModel.Map.InitialViewpoint = viewpoint;
        }

        private SimpleRenderer setsymbology()
        {
            SimpleMarkerSymbol pointSymbol = new SimpleMarkerSymbol()
            {
                Color = Colors.Yellow,
                Size = 5,
                Style = SimpleMarkerSymbolStyle.Square,
            };

            SimpleRenderer renderer = new SimpleRenderer(pointSymbol);
            return renderer;
        }

        private IList<string> UseRegextoSplitrow(string row)
        {
            //first split at ", include quotes, then split at , and exclude " and ,
            var pattern4 = @""".+?""|[^"",]+?(?=,)|(?<=,)[^""]+";
            //
            var pattern5 = @""".+?""|([^\d\w""])[,]([^\d\w""])|[^"",]+?(?=,)|(?<=,)[^""]+";
            //split recursive commas(,,) to include empty strings as missing value
            //however, if last column is missing doesn't include the last empty string missing value
            //(?<=,),|".+?"|[^",]+?(?=,)|(?<=,)[^"]+
            var pattern6 = @"(?<=,),|" + pattern4;
            var regx = new Regex(pattern6);
            IList<string> attributes = new List<string>();
            foreach (Match m in regx.Matches(row))
            {
                attributes.Add(m.ToString());
            }

            return attributes;
        }
    }
}