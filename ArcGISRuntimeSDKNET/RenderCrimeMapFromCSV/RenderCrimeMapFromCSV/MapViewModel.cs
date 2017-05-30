using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RenderCrimeMapFromCSV
{
    internal class MapViewModel : INotifyPropertyChanged
    {
        private HashSet<string> crimeTypeList = new HashSet<string>();

        private GraphicsOverlayCollection graphicsOverlays = new GraphicsOverlayCollection();

        private string selectedGraphicsCount = string.Empty;

        public event PropertyChangedEventHandler PropertyChanged;

        public HashSet<string> CrimeTypeList
        {
            get { return crimeTypeList; }
            internal set { crimeTypeList = value; OnPropertyChanged(); }
        }

        public GraphicsOverlayCollection GraphicsOverlays { get { return graphicsOverlays; } }

        internal void AddGraphicsOverlay(GraphicsOverlay layer, bool replace = false) =>
                                                        graphicsOverlays.Insert(0, layer);

        /// <summary>
        /// Gets the map
        /// </summary>
        public Map Map { get; } = new Map(Basemap.CreateDarkGrayCanvasVector());

        public string SelectedGraphicsCount
        {
            get { return selectedGraphicsCount; }
            set { selectedGraphicsCount = value; OnPropertyChanged(); }
        }

        public void SetOperationalLayers() => Map.OperationalLayers.Add(new FeatureLayer(
                                      new ServiceFeatureTable(
        //new Uri("https://services.arcgis.com/P3ePLMYs2RVChkJx/ArcGIS/rest/services/USA_Counties/FeatureServer/0"))));
        new Uri("https://services.arcgis.com/P3ePLMYs2RVChkJx/ArcGIS/rest/services/USA_States_Generalized/FeatureServer/0"))));

        public Viewpoint InitialViewpoint
        {
            get { return Map.InitialViewpoint; }
            set { Map.InitialViewpoint = value; }
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged" /> event
        /// </summary>
        /// <param name="propertyName">The name of the property that has changed</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) => 
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}