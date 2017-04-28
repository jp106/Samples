angular.module('weather-service-map', ['esri.map'])
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
            "esri/widgets/Expand",
            "dojo/on",
            "dojo/dom",
            "dojo/domReady!"
        ], function (Map, FeatureLayer, ScaleBar, Search,
            LayerList, SimpleMarkerSymbol, Layer, Graphic, Expand, on, dom) {
            //setup the map viewer
            self.map = new Map({
                basemap: 'streets'
            });

            self.redlandspointfeaturelayer = new FeatureLayer({
                url: "http://services1.arcgis.com/0MSEUqKaxRlEPj5g/ArcGIS/rest/services/Redlands_CA/FeatureServer/0"
            });
            //setup widgets
            var weatherapiurl = "https://api.weather.gov/points/"
            var scale = new ScaleBar();
            var search = new Search();
            var infoContainer, expandWidget;
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
                title: "Feature ID",
                content: "Feature is a {LocationType} with ID {FID}"
            }
            infoContainer = dom.byId("infoWidget");
            expandWidget = new Expand({
                expandIconClass: "esri-icon-edit",
                expandTooltip: "Expand Edit",
                expanded: true,
                view: self.mapview,
                content: infoContainer
            });

            self.redlandspointfeaturelayer.popupTemplate = template;

            self.onViewCreated = function (view) {
                self.mapview = view;
                scale.view = self.mapview;
                search.view = self.mapview;

                self.mapview.ui.add(scale, "bottom-left");
                self.mapview.ui.add(search, "bottom-right");
                self.mapview.ui.add(expandWidget, "top-right");
                self.mapview.then(function () {
                    self.mapview.on("click", getAllFeatures);
                });

                //get feature from map click event
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
                            addFeaturetomap(featurefound.clone());
                            //construct request url
                            var requesturl = setRequestUrl(response.results[0].mapPoint);
                            //get data from weather service
                            requestWeatherData(requesturl);
                        });
                }

                function setRequestUrl(point) {
                    //get feature geometry to get weather data
                    var latitude = point.latitude;
                    var longitude = point.longitude;
                    var clickpoint = latitude + "," + longitude;
                    console.log("Map point clicked: ");
                    console.log(clickpoint);
                    var requesturl = weatherapiurl + clickpoint;
                    return requesturl
                }

                //Add selected feature to map
                function addFeaturetomap(featuregraphic) {
                    featuregraphic.symbol = selectionSymbol;
                    self.mapview.graphics.add(featuregraphic);
                }

                //weather
                function requestWeatherData(requesturl) {
                    var requestforecast = requesturl + "/forecast";
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
                }

                //reset controls and graphics on map
                var clearmessages = function () {
                    self.mapview.graphics.removeAll();
                    $scope.statusmessage = "Features not found.";
                    $scope.radarstation = " ";
                    $scope.forecastoffice = " ";
                    $scope.forecast = " ";
                };
            };
            self.map.add(self.redlandspointfeaturelayer);
        });
    });