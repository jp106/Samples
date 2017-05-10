using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.UI;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace RenderCrimeMapFromCSV.Model
{
    internal class NewGraphicsOverlayFromGraphicsList
    {
        private Viewpoint graphicsExtent;

        private GraphicsOverlay newGraphicsOverlay;

        public NewGraphicsOverlayFromGraphicsList(IList<Graphic> graphics)
        {
            CreateGraphicsOverlayfromGraphicsList(graphics);
        }
        public Viewpoint GraphicsExtent
        {
            get { return graphicsExtent; }
            set { graphicsExtent = value; }
        }

        public GraphicsOverlay NewGraphicsOverlay
        {
            get { return newGraphicsOverlay; }
            set { newGraphicsOverlay = value; }
        }
        private void CreateGraphicsOverlayfromGraphicsList(IList<Graphic> graphics)
        {
            var graphicslayer = new GraphicsOverlay();
            graphicslayer.IsPopupEnabled = true;
            graphics.ToList().ForEach(x => graphicslayer.Graphics.Add(x));
            graphicslayer.Renderer = setsymbology();
            getGraphicsOverlayExtent(graphics.Where(x => x.Geometry != null).Select(x => (MapPoint)(x.Geometry)).ToList());
            newGraphicsOverlay = graphicslayer;
        }

        private void getGraphicsOverlayExtent(IEnumerable<MapPoint> points)
        {
            PolygonBuilder pb = new PolygonBuilder(points, SpatialReferences.Wgs84);

            graphicsExtent = new Viewpoint(pb.Extent);
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
    }
}