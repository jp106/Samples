using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace RenderCrimeMapFromCSV.Model
{
    internal class ReadCSVWithRegexToGraphicsList
    {
        public ReadCSVWithRegexToGraphicsList()
        {

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

            var latitude = attributesList[latindex];
            var longitude = attributesList[longindex];
            return ConstructNewGraphic(latitude, longitude, attributesKeyValue);
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
    }
}