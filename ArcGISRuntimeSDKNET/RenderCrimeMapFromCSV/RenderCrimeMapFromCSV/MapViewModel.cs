using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.UI;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RenderCrimeMapFromCSV
{
    internal class MapViewModel : INotifyPropertyChanged
    {
        public MapViewModel()
        {

        }


        private Map _map = new Map(Basemap.CreateDarkGrayCanvasVector());

        /// <summary>
        /// Gets or sets the map
        /// </summary>
        public Map Map
        {
            get { return _map; }
            set { _map = value; OnPropertyChanged(); }
        }

        private GraphicsOverlayCollection graphicsOverlays = new GraphicsOverlayCollection();

        public GraphicsOverlayCollection GraphicsOverlays
        {
            get { return graphicsOverlays; }
            set { graphicsOverlays = value; OnPropertyChanged(); }
        }

        public void AddGraphicsOverlay(GraphicsOverlay layer)
        {
            graphicsOverlays.Add(layer);
        }

        public void SetViewPoint(Viewpoint viewpoint)
        {
            
        }

        private string selectedGraphicsCount = string.Empty;

        public string SelectedGraphicsCount
        {
            get { return selectedGraphicsCount; }
            set { selectedGraphicsCount = value; OnPropertyChanged(); }
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

        public event PropertyChangedEventHandler PropertyChanged;

    }
}