﻿using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace RenderCrimeMapFromCSV.Model
{
    public class GraphicsList
    {
        public IList<Graphic> ListofGraphics { get; } = new List<Graphic>();

        public GraphicsList(IList<string[]> rows)
        {
            ListofGraphics = ConstructGraphicsList(rows);
        }

        public GraphicsList(DataTable dt)
        {
            ListofGraphics = ConstructGraphicsList(dt);
        }

        private IList<Graphic> ConstructGraphicsList(DataTable dt)
        {
            var graphics = new List<Graphic>();            
            var rows = dt.AsEnumerable();
            rows.ToList().ForEach(r => graphics.Add(ConstructNewGraphicfromDataRow(r)));
            return graphics;
        }

        private Graphic ConstructNewGraphicfromDataRow(DataRow r)
        {
            var attributes = new List<KeyValuePair<string, object>>();
            var columns = r.Table.Columns;
            var locationindex = columns.IndexOf("Area affected");
            var rowlocation = r.ItemArray[locationindex];
            r.ItemArray.Select((x, i) => new { X = x, i = i })
                .ToList()
                .ForEach(x => attributes
                .Add(new KeyValuePair<string, object>(columns[x.i].ColumnName, x.X)));
            var g = new Graphic();
            return new Graphic();
        }

        private IList<Graphic> ConstructGraphicsList(IList<string[]> rowList)
        {
            var graphics = new List<Graphic>();
            var columns = rowList[0];
            rowList?.Skip(1)
                   .ToList()
                   .ForEach(x => graphics.Add(ConstructNewGraphicfromRow(x, columns)));
            return graphics;
        }

        private Graphic ConstructNewGraphic(string latitude, string longitude,
                                            List<KeyValuePair<string, object>> attributes)
        {
            IList<Graphic> graphics = new List<Graphic>();
            double parse;
            if (double.TryParse(latitude, out parse) &&
                double.TryParse(longitude.ToString(), out parse))
            {
                var p = new MapPoint(Convert.ToDouble(longitude),
                                          Convert.ToDouble(latitude),
                                          SpatialReferences.Wgs84);
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

            // build attribute objects
            attributes.Select((x, i) => new { Name = columns?[i], Value = x })
                      .ToList()
                      .ForEach(x => attributesKeyValue.Add(
                              new KeyValuePair<string, object>(x.Name, x.Value)));
            return (string.IsNullOrEmpty((string)attributes?[latindex])) ?
                    new Graphic() :
                    ConstructNewGraphic(attributes[latindex],
                                        attributes[longindex],
                                        attributesKeyValue);
        }
    }
}