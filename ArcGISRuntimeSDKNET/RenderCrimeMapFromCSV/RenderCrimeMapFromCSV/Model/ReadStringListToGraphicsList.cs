using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RenderCrimeMapFromCSV.Model
{
    internal class ReadStringListToGraphicsList
    {
        private IList<Graphic> graphicsList;

        public IList<Graphic> GraphicsList
        {
            get { return graphicsList; }
            set { graphicsList = value; }
        }

        public ReadStringListToGraphicsList(IList<string[]> rows)
        {
           graphicsList = ConstructGraphicsList(rows);
        }

        private IList<Graphic> ConstructGraphicsList(IList<string[]> rowList)
        {
            var graphics = new List<Graphic>();
            var columns = rowList[0];

            rowList.Skip(1)
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

        private Graphic ConstructNewGraphicfromRow(string[] attributes, string[] columns)
        {
            var attributesKeyValue = new List<KeyValuePair<string, object>>();
            // find indices of geometry and crimte type columns
            int latindex = Array.FindIndex(columns, r => r == "Latitude");
            int longindex = Array.FindIndex(columns, r => r == "Longitude");
            int crimetypeindex = Array.FindIndex(columns, r => r == "Primary Type");

            //clear attributes list
            attributesKeyValue.Clear();

            //TODO: Handle empty values in UseRegextoSplitrow
            if (string.IsNullOrEmpty((string)attributes[latindex]))
                return new Graphic();

            //build attribute objects
            attributes.Select((x, i) => new { Name = columns[i], Value = x })
                .ToList()
                .ForEach(x => attributesKeyValue.Add(new KeyValuePair<string, object>(x.Name, x.Value)));

            //UniqueCrimeType.Add(attributes[crimetypeindex].ToString());

            var latitude = attributes[latindex];
            var longitude = attributes[longindex];
            return ConstructNewGraphic(latitude, longitude, attributesKeyValue);
        }
    }
}