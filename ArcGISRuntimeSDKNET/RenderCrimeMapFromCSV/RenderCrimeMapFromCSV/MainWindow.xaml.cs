﻿using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.UI;
using RenderCrimeMapFromCSV.Model;
using System;
using System.Collections.Generic;
using System.Data;
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
        private const string EXCELFILEPATH = @"data\2017_Annual_Summary.xls";
        private const string USSTATESURL = "https://services.arcgis.com/P3ePLMYs2RVChkJx/ArcGIS/rest/services/USA_States_Generalized/FeatureServer/0";
        private MapViewModel mapViewModel;
        private HashSet<string> UniqueCrimeType;

        public MainWindow()
        {
            InitializeComponent();
            Initialize();
        }

        private void AddGraphicsLayertoMap(Esri.ArcGISRuntime.UI.GraphicsOverlay layer)
        {
            layer.SelectionColor = Colors.Red;
            mapViewModel.AddGraphicsOverlay(layer);
        }

        private void ButtonClearGraphics_Click(object sender, RoutedEventArgs e) => ResetSelection();

        private void ButtonFullExtent_Click(object sender, RoutedEventArgs e) => MainMapView.SetViewpoint(mapViewModel.InitialViewpoint);

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

        private void FilterGraphicsBasedonSelection(object sender, Esri.ArcGISRuntime.UI.GraphicsOverlay gl)
        {
            var selectedvalue = ((Selector)sender).SelectedValue;
            gl.ClearSelection();
            gl.Graphics.Where(x =>
                        (x.Attributes.Keys.Contains("Primary Type") == true) &&
                        (x.Attributes["Primary Type"].ToString() == selectedvalue.ToString()))
                        .ToList().ForEach(selectGraphic);

            mapViewModel.SelectedGraphicsCount = $"Found {gl.SelectedGraphics.Count().ToString()} crimes ";
        }

        private void getValuesFromCSV(string filepath, out CSVFileTextFieldParser readcsv, out int primartytypeindex, out GraphicsList getgraphicslist)
        {
            readcsv = new CSVFileTextFieldParser(filepath);
            primartytypeindex = Array.IndexOf(readcsv.RowList.First(), "Primary Type");
            getgraphicslist = new GraphicsList(readcsv.RowList);
        }

        private void Initialize()
        {
            // Create new Map with basemap
            mapViewModel = this.FindResource("MapViewModel") as MapViewModel;
            mapViewModel.SelectedGraphicsCount = "Graphics Count";
            //mapViewModel.SetOperationalLayers();
        }

        private void LoadOutageDatatoMapSetMapExtent()
        {
            //LoadGeometryToGraphicsList();
            //CreateGraphicsLayerFromList();
            //AddGraphicsToLayer();
            var dtexcel = new DataTableFromExcel(EXCELFILEPATH);
            //var graphics = new GraphicsList(dtexcel.DataTableExcel);
            var statesr = new StatesReader();
            var states = statesr.ConstructStatesString(dtexcel.DataTableExcel);

            var geometryquery = new FeatureLayerQuery(USSTATESURL,
            $"upper(STATE_NAME) in ({states})");
            //"upper(STATE_NAME) in ('FLORIDA', 'GEORGIA')");
            //"upper(STATE_NAME) = 'FLORIDA'");
            mapViewModel.Map.OperationalLayers.Add(geometryquery.QueryLayer);
        }

        private void LoadPointDatatoMapSetMapExtent()
        {
            try
            {
                //Read CSV File
                CSVFileTextFieldParser readcsv;
                int primartytypeindex;
                GraphicsList getgraphicslist;
                getValuesFromCSV(CSVFILEPATH, out readcsv, out primartytypeindex, out getgraphicslist);
                // Read geometry from csv file and construct graphics list with attributes
                UniqueCrimeType = new HashSet<string>(readcsv.RowList
                                                             .Skip(1)
                                                             .Select(x => x[primartytypeindex]).Distinct());
                SetCrimeTypeList();
                var goverlay = new Model.GraphicsOverlayCreator(getgraphicslist.ListofGraphics);
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
            // TODO: Set Center
            //await MainMapView.SetViewpointCenterAsync(viewpoint.GetCenter());
            mapViewModel.InitialViewpoint = viewpoint;
        }
    }
}