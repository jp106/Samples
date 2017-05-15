using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.UI;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace RenderCrimeMapFromCSV.Model
{
    internal class GraphicsOverlayCreator
    {
        private Viewpoint graphicsExtent;

        public GraphicsOverlayCreator(IList<Graphic> graphics)
        {
            NewGraphicsOverlay = CreateGraphicsOverlayfromGraphicsList(graphics);
        }

        public Viewpoint GraphicsExtent { get { return graphicsExtent; } }

        public GraphicsOverlay NewGraphicsOverlay { get; }

        private SimpleRenderer defaultRenderer => new SimpleRenderer(new SimpleMarkerSymbol()
        {
            Color = Colors.Yellow,
            Size = 5,
            Style = SimpleMarkerSymbolStyle.Square,
        });

        private GraphicsOverlay CreateGraphicsOverlayfromGraphicsList(IList<Graphic> graphics)
        {
            var graphicslayer = new GraphicsOverlay();
            graphicslayer.IsPopupEnabled = true;
            graphics.ToList().ForEach(x => graphicslayer.Graphics.Add(x));
            graphicslayer.Renderer = defaultRenderer;
            graphicsExtent = setGraphicsOverlayExtent(graphics.Where(x => x.Geometry != null)
                                             .Select(x => (MapPoint)(x.Geometry)).ToList());
            return graphicslayer;
        }

        private Viewpoint setGraphicsOverlayExtent(IEnumerable<MapPoint> points) => new Viewpoint(
            new PolygonBuilder(points, SpatialReferences.Wgs84).Extent);
    }
}