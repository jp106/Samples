using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace RenderCrimeMapFromCSV.Model
{
    public class FeatureLayerQuery
    {
        private string query;
        private string url;

        public FeatureLayerQuery(string url, string query)
        {
            this.url = url;
            this.query = query;
            Geometries = new List<Geometry>();
            CreateFeatureLayer();
            QueryFeatureLayer(QueryLayer, queryparams());
        }

        public int OID { get; private set; }

        public FeatureLayer QueryLayer { get; private set; }

        public string ResultName { get; private set; }

        public IList<Geometry> Geometries { get; } = new List<Geometry>();

        private void CreateFeatureLayer() => QueryLayer = new FeatureLayer(new ServiceFeatureTable(new Uri(url)));

        
        private async void QueryFeatureLayer(FeatureLayer layer, QueryParameters queryparams)
        {
            try
            {
                var serviceft = new ServiceFeatureTable(new Uri(url));
                var newresults = await serviceft.QueryFeaturesAsync(queryparams, QueryFeatureFields.LoadAll);
                var results = await layer.FeatureTable.QueryFeaturesAsync(queryparams);
                if (results.ToList().Any())
                {
                    var feature = results.ToList()[0];
                    //ResultName = feature.Attributes?["STATE_NAME"]?.ToString();
                    OID = Convert.ToInt16(feature.Attributes["FID"]);
                    QueryLayer.DefinitionExpression = queryparams.WhereClause;
                    Geometries.Add(feature.Geometry);
                    //QueryLayer.SelectFeature(feature);
                    //await myMapView.SetViewpointGeometryAsync(feature.Geometry.Extent);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("query featurelayer failed");
            }
            
        }

        private QueryParameters queryparams()
        {
            return new QueryParameters()
            {
                //ReturnGeometry = false,
                WhereClause = query,
                MaxFeatures = 50
            };
        }
    }
}