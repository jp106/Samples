using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.UI;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System;

namespace RenderCrimeMapFromCSV
{
    internal class MapViewModel : INotifyPropertyChanged
    {
        private HashSet<string> crimeTypeList = new HashSet<string>();

        private GraphicsOverlayCollection graphicsOverlays = new GraphicsOverlayCollection();

        private Map map = new Map(Basemap.CreateDarkGrayCanvasVector());

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
        /// Gets or sets the map
        /// </summary>
        public Map Map { get { return map; } }

        public string SelectedGraphicsCount
        {
            get { return selectedGraphicsCount; }
            set { selectedGraphicsCount = value; OnPropertyChanged(); }
        }


        public void SetViewPoint(Viewpoint viewpoint)
        {
        }

        /// <summary>
        /// Raises the <see cref="MapViewModel.PropertyChanged" /> event
        /// </summary>
        /// <param name="propertyName">The name of the property that has changed</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var propertyChangedHandler = PropertyChanged;
            if (propertyChangedHandler != null)
                propertyChangedHandler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}