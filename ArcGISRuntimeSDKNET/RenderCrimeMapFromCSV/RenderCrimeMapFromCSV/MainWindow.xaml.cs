using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.UI;
using System;
using System.Collections.Generic;
using System.IO;
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

        public MainWindow()
        {
            InitializeComponent();
            Initialize();
            LoadPointDatatoMapSetMapExtent();
        }

        private void AddGraphicsLayertoMap(GraphicsOverlay layer)
        {
            MainMapView.GraphicsOverlays.Add(layer);
            layer.SelectionColor = Colors.Red;
        }

        private void ButtonClearGraphics_Click(object sender, RoutedEventArgs e)
        {
            ResetSelection();
        }

        private void ButtonFullExtent_Click(object sender, RoutedEventArgs e)
        {
            MainMapView.SetViewpoint(MainMapView.Map.InitialViewpoint);
        }

        private Graphic ConstructGraphicsListfromRow(string row, IList<string> columns)
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
            var gl = MainMapView.GraphicsOverlays.First();
            var selectedvalue = ((System.Windows.Controls.Primitives.Selector)sender).SelectedValue;
            gl.ClearSelection();
            gl.Graphics
                .Where(x => (x.Attributes.Keys.Contains("Primary Type") == true)
                && (x.Attributes["Primary Type"].ToString() == selectedvalue.ToString()))
                .ToList().ForEach(selectGraphics);
            CountLabel.Content = String.Format("Found {0} crimes.",
                gl.SelectedGraphics.Count().ToString());
        }

        private void Initialize()
        {
            // Create new Map with basemap
            MainMapView.Map = new Esri.ArcGISRuntime.Mapping.Map(Esri.ArcGISRuntime.Mapping.BasemapType.ImageryWithLabels, 34.056295, -117.195800, 10);
        }

        private void LoadPointDatatoMapSetMapExtent()
        {
            try
            {
                //Open CSV File
                var csvfile = OpenCSVFile();
                if (csvfile == null)
                {
                    return;
                }
                //Read geometry from csv file and construct graphics list with attributes
                var graphicslist = ReadCSVFiletoConstructGraphicsList(csvfile);
                SetCrimeTypeList();
                AddGraphicsLayertoMap(CreateGraphicsOverlayfromGraphicsList(graphicslist));
            }
            catch (Exception)
            {
                MessageBox.Show("Failed to load points from csv file.", "Error loading data.");
                Console.WriteLine("Failed to load points.");
                throw;
            }
        }

        private FileStream OpenCSVFile()
        {
            var filepath = @"Data\crimedata.csv";
            Console.Write(File.Exists(filepath));
            if (!File.Exists(filepath)) return null;

            return File.OpenRead(filepath);
        }

        private IList<Graphic> ReadCSVFiletoConstructGraphicsList(FileStream csvfile)
        {
            using (csvfile)
            using (var reader = new StreamReader(csvfile))
            {
                var graphics = new List<Graphic>();
                UniqueCrimeType = new HashSet<string>();
                var columnsstring = reader.ReadLine();
                IList<String> columns = columnsstring.Split(',');
                while (!reader.EndOfStream)
                {
                    graphics.Add(ConstructGraphicsListfromRow(reader.ReadLine(), columns));
                }
                return graphics;
            }
        }

        private void ResetSelection()
        {
            CountLabel.Content = String.Empty;
            MainMapView.GraphicsOverlays.First().ClearSelection();
        }

        private void selectGraphics(Graphic g)
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
            MainMapView.Map.InitialViewpoint = viewpoint;
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

            var regx = new Regex(pattern4);
            IList<string> attributes = new List<string>();
            foreach (Match m in regx.Matches(row))
            {
                attributes.Add(m.ToString());
            }

            return attributes;
        }
    }
}