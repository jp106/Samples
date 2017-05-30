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
                // TODO : Fix: Doesn't return any state features from whereclause 
                // upper(STATE_NAME) in ('Florida','California','Oklahoma','Alabama','Georgia','Mississippi','New Mexico','Washington','Connecticut','Massachusetts','Rhode Island','Utah','North Carolina','Nevada','Missouri','Arkansas','Texas','Kentucky','West Virginia','Tennessee','Maine','New Hampshire','Vermont','Ohio','Michigan','New York','Oregon')
                var results = await layer.FeatureTable.QueryFeaturesAsync(queryparams);
                var features = results.ToList();
                if (features.Any())
                {
                    var feature = features[0];
                    //ResultName = feature.Attributes?["STATE_NAME"]?.ToString();
                    OID = Convert.ToInt16(feature.Attributes["FID"]);
                    QueryLayer.DefinitionExpression = queryparams.WhereClause;
                    Geometries.Add(feature.Geometry);
                    //QueryLayer.SelectFeature(feature);
                    //await myMapView.SetViewpointGeometryAsync(feature.Geometry.Extent);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("query featurelayer failed");
            }

            //var featCollection = new FeatureCollection();
            //featCollection.Tables.Add(new FeatureCollectionTable(results));

            //// Create a layer to display the feature collection, add it to the map's operational layers
            //ResultLayer = new FeatureCollectionLayer(featCollection);
            //MyMapView.Map.OperationalLayers.Add(featCollectionTable);
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