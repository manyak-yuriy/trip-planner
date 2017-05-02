﻿angular.module('mainApp').controller('searchController', function ($scope, $http) {

    $scope.settings = {};

    $http.get("search/GetAvailableCultures")
    .then(function (response) {
        $scope.settings.locales = response.data;
        console.log(JSON.stringify(response.data), false);
    });

    $http.get("search/GetAvailableCurrencies")
    .then(function (response) {
        $scope.settings.currencies = response.data;
        console.log(JSON.stringify(response.data), false);
    });

    $scope.loadCountries = function () {
        $http.get("search/GetAvailableCountries", {
            params: { locale: $scope.settings.selectedLocale }
        })
        .then(function (response) {
            $scope.settings.countries = response.data;
            console.log(JSON.stringify(response.data), false);
        });
    }

    $scope.person = {};
    $scope.people = [
      { name: 'Adam', email: 'adam@email.com', age: 10 },
      { name: 'Amalie', email: 'amalie@email.com', age: 12 },
      { name: 'Wladimir', email: 'wladimir@email.com', age: 30 },
      { name: 'Samantha', email: 'samantha@email.com', age: 31 },
      { name: 'Estefanía', email: 'estefanía@email.com', age: 16 },
      { name: 'Natasha', email: 'natasha@email.com', age: 54 },
      { name: 'Nicole', email: 'nicole@email.com', age: 43 },
      { name: 'Adrian', email: 'adrian@email.com', age: 21 }
    ];
});