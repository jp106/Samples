angular.module('weather-map', ['esri.map'])
    .controller('MapController', function (esriLoader, weatherFactory, $scope) {
        var self = this;
        esriLoader.require([
            'esri/Map',
            'esri/layers/FeatureLayer',
            'esri/widgets/ScaleBar',
            'esri/widgets/Search',
            'esri/widgets/LayerList',
            "esri/symbols/SimpleMarkerSymbol",
            "esri/layers/Layer",
            "esri/Graphic",
            "esri/geometry/support/webMercatorUtils",
            "dojo/_base/array",
            "dojo/on",
            "dojo/dom",
            "dojo/domReady!"
        ], function (Map, FeatureLayer, ScaleBar, Search,
            LayerList, SimpleMarkerSymbol, Layer, Graphic, webMercatorUtils, arrayUtils, on, dom) {

            self.map = new Map({
                basemap: 'streets'
            });

            self.newlayer = new FeatureLayer({
                url: "http://services1.arcgis.com/0MSEUqKaxRlEPj5g/ArcGIS/rest/services/Redlands_CA/FeatureServer/0"
            });

            var scale = new ScaleBar();
            var search = new Search();
            var toc = new LayerList();
            var selectionSymbol = SimpleMarkerSymbol({
                color: [0, 0, 0, 0],
                style: "circle",
                size: '40px',
                outline: {
                    color: [0, 255, 255, 1],
                    width: "3px"
                }
            });

            $scope.radatastation = "new";
            var template = {
                title: "Weather Forecast",
                content: "Feature is a {LocationType} with ID {FID}"
            }
            self.newlayer.popupTemplate = template;

            self.onViewCreated = function (view) {
                self.mapview = view;
                scale.view = self.mapview;
                search.view = self.mapview;

                self.mapview.ui.add(scale, "bottom-left");
                self.mapview.ui.add(search, 'top-right');
                self.mapview.then(function () {
                    self.mapview.on("click", getAllFeatures);
                });


                function getAllFeatures(event) {
                    clearmessages();
                    self.mapview.hitTest(event.screenPoint)
                        .then(function (response) {
                            if (response.results.length == 0) {
                                clearmessages();
                                return;
                            }
                            var featurefound = response.results[0].graphic;
                            $scope.statusmessage = "Feature is a " + featurefound.attributes.LocationType + " with ID " + featurefound.attributes.FID;
                            var newgraphic = featurefound.clone();
                            newgraphic.symbol = selectionSymbol;
                            self.mapview.graphics.add(newgraphic);


                            var latitude = response.results[0].mapPoint.latitude;
                            var longitude = response.results[0].mapPoint.longitude;
                            var clickpoint = latitude + "," + longitude;
                            console.log("Map point clicked: ");
                            console.log(clickpoint);
                            var requesturl = "https://api.weather.gov/points/" + clickpoint;
                            var requestforecast = "https://api.weather.gov/points/" + clickpoint + "/forecast";
                            weatherFactory.getPointWeatherDetails(requesturl)
                                .then(function (response) {
                                    $scope.radarstation = response.radarStation;
                                    weatherFactory.getforecastOffice(response.forecastOffice)
                                        .then(function (response) {
                                            $scope.forecastoffice = response.name;
                                        });

                                    weatherFactory.getweatherforecast(requestforecast)
                                        .then(function (response) {
                                            $scope.forecast = response.shortForecast;
                                        });
                                }, function (reason) {
                                    $scope.error = "Could not find weather data.";
                                });
                        });
                }

                var clearmessages = function () {
                    self.mapview.graphics.removeAll();
                    $scope.statusmessage = "No features found.";
                    $scope.radarstation = " ";
                    $scope.forecastoffice = " ";
                    $scope.forecast = " ";
                };
            };
            self.map.add(self.newlayer);
        });
    });