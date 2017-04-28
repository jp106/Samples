(function () {

    var weather = function ($http) {

        var weatherService = {};

        weatherService.getPointWeatherDetails = function (requestUrl) {

            return $http.get(requestUrl).then(function (response) {
                console.log("Nearest Radar Station : ");
                console.log(response.data.properties.radarStation);
                return response.data.properties;
            });
        };

        weatherService.getforecastOffice = function (forecastOfficeUrl) {

            return $http.get(forecastOfficeUrl)
                .then(function (response) {
                    return response.data;
                });
        };

        weatherService.getweatherforecast = function (forecasturl) {

            return $http.get(forecasturl)
                .then(function (response) {
                    return response.data.properties.periods[0];
                });
        };
        return weatherService
    };

    var appmodule = angular.module("weather-map");
    appmodule.factory('weatherFactory', weather);

}());