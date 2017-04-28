(function () {

    var weather = function ($http) {

        var weatherService = {};
        //Get Weather Details from a point geometry
        weatherService.getPointWeatherDetails = function (requestUrl) {

            return $http.get(requestUrl).then(function (response) {
                console.log("Nearest Radar Station : ");
                console.log(response.data.properties.radarStation);
                return response.data.properties;
            });
        };
        //Get forecast office name from office code 
        weatherService.getforecastOffice = function (forecastOfficeUrl) {

            return $http.get(forecastOfficeUrl)
                .then(function (response) {
                    return response.data;
                });
        };
        //Get weather forecast from point geometry
        weatherService.getweatherforecast = function (forecasturl) {

            return $http.get(forecasturl)
                .then(function (response) {
                    return response.data.properties.periods[0];
                });
        };
        return weatherService
    };

    var appmodule = angular.module("weather-service-map");
    appmodule.factory('weatherFactory', weather);

}());