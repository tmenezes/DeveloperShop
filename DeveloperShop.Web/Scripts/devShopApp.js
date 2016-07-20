// define an angular module for our app
var devShopApp = angular.module('devShopApp', ['ngRoute', 'ngResource']);

// define Routing for app
devShopApp.config(function ($routeProvider) {
    $routeProvider.
      when('/', {
          templateUrl: 'Templates/developers.html',
          controller: 'DevelopersController'
      }).
      when('/cart', {
          templateUrl: 'Templates/cart.html',
          controller: 'CartController'
      }).
      when('/cart/addingItem/:devId', {
          templateUrl: 'Templates/addingToCart.html',
          controller: 'CartController'
      }).
      otherwise({
          redirectTo: '/'
      });
});

// factories
devShopApp.factory('AppApi', function ($resource) {

    return {
        Developers: $resource('api/developers/:id', { id: '@id' },
        {
            getFromGithub: {
                url: 'api/developers/fromGithub/:query',
                method: 'GET'
            }
        }),
        Cart: $resource('api/cart/:id', { id: '@id' })
    };
});


// controllers
devShopApp.controller('DevelopersController', function ($scope, $resource, AppApi, $location) {

    $scope.developers = AppApi.Developers.query();
    $scope.developerUsername = "";

    // actions
    $scope.searchDeveloperOnGithub = function () {

        if ($scope.developerUsername === "") return false;

        var dev = AppApi.Developers.getFromGithub({ query: $scope.developerUsername });
        if (dev != null) {
            $scope.developers.push(dev);
            $scope.developerUsername = "";
        }
        else {
            alert("Developer was not found.");
        }

        return false;
    }

    $scope.addNewItem = function (developerId) {

        $location.path("/cart/addingItem/" + developerId);
        return false;
    }

});

devShopApp.controller('CartController', function ($scope, $resource, $routeParams, $location, AppApi) {

    $scope.devId = $routeParams.devId;

    if ($scope.devId > 0) {
        $scope.developer = AppApi.Developers.get({ id: $scope.devId });
        $scope.hours = 8;
    }
    else {
        $scope.cart = AppApi.Cart.get();
    }

    // actions
    $scope.addToCart = function () {

        var data = { DeveloperId: $scope.devId, AmountOfHours: $scope.hours };
        var response = AppApi.Cart.save(data);

        navigateAfterCartOperation(response);

        return false;
    }

    $scope.removeFromCart = function (developerId) {

        var data = { DeveloperId: developerId };
        var response = AppApi.Cart.delete(data);

        navigateAfterCartOperation(response);

        return false;
    }

    var navigateAfterCartOperation = function (resource) {

        resource.$promise.then(function (cart) {
            var hasItems = cart != null && cart.Items.length > 0;
            if (hasItems) {
                $scope.cart = cart;
                $location.path("/cart");
            }
            else
                $location.path("/");
        });

        return false;
    }

});