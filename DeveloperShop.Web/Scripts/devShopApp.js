// define an angular module for our app
var devShopApp = angular.module('devShopApp', ['ngRoute', 'ngResource']);

// define Routing for app
devShopApp.config(['$routeProvider', function ($routeProvider) {
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
      when('/cart/orderFinished', {
          templateUrl: 'Templates/orderFinished.html',
          controller: 'CartController'
      }).
      otherwise({
          redirectTo: '/'
      });
}]);

// factories
devShopApp.factory('AppApi', ['$resource', function ($resource) {
    return {
        Developers: $resource('api/developers/:id', { id: '@id' },
        {
            getFromGithub: {
                url: 'api/developers/fromGithub/:query',
                method: 'GET'
            },
            getFromGithubOrganization: {
                url: 'api/developers/fromGithubOrganization/:query',
                method: 'GET',
                isArray: true
            }
        }),
        Cart: $resource('api/cart/:id', { id: '@id' },
        {
            applyDiscount: {
                url: 'api/cart/applyDiscount/:coupon',
                method: 'PUT'
            },
            finishOrder: {
                url: 'api/cart/finishOrder',
                method: 'GET'
            }
        })
    };
}]);


// controllers
devShopApp.controller('DevelopersController', ['$scope', '$resource', 'AppApi', '$location', function ($scope, $resource, AppApi, $location) {

    $scope.developers = AppApi.Developers.query();
    $scope.searchKey = "";

    // actions
    $scope.searchDeveloperOnGithub = function () {

        if ($scope.searchKey === "") return false;

        var resourceDev = AppApi.Developers.getFromGithub({ query: $scope.searchKey });

        resourceDev.$promise.then(function (response) {
            $scope.developers.push(resourceDev);
            $scope.searchKey = "";
        }, function (failureResponse) {
            if (failureResponse.status === 500) {
                alert("Internal Server Error. " + failureResponse.data.ExceptionMessage);
            } else {
                alert("Developer was not found.");
            }
        });

        return false;
    }

    $scope.searchOnGithubByOrganization = function () {

        if ($scope.organization === "") return false;

        var resource = AppApi.Developers.getFromGithubOrganization({ query: $scope.searchKey });
        resource.$promise.then(function (response) {
            $scope.developers = AppApi.Developers.query(); // update developers list
            $scope.searchKey = "";
            alert("Developer(s) found: " + response.length);
        });

        return false;
    }

    $scope.addNewItem = function (developerId) {

        $location.path("/cart/addingItem/" + developerId);
        return false;
    }

}]);

devShopApp.controller('CartController', ['$scope', '$resource', '$routeParams', '$location', 'AppApi', function ($scope, $resource, $routeParams, $location, AppApi) {

    $scope.devId = $routeParams.devId;

    if ($scope.devId > 0) {
        $scope.developer = AppApi.Developers.get({ id: $scope.devId });
        $scope.hours = 8;
    }
    else {
        $scope.cart = AppApi.Cart.get();
        $scope.cart.$promise.then(function (cart) {
            $scope.hasItems = cart.Items.length > 0;
            updateCouponVariables();
        });
    }

    // actions
    $scope.addToCart = function () {

        var data = { DeveloperId: $scope.devId, AmountOfHours: $scope.hours };
        var response = AppApi.Cart.save(data);

        navigateAfterCartOperation(response);

        return false;
    }

    $scope.removeFromCart = function (developerId) {

        var data = { id: developerId };
        var response = AppApi.Cart.delete(data);

        navigateAfterCartOperation(response);

        return false;
    }

    $scope.applyDiscount = function () {

        $scope.invalidCoupon = false;

        if ($scope.coupon === "") return false;

        var data = { couponKey: $scope.coupon };
        var response = AppApi.Cart.applyDiscount(data);

        response.$promise.then(function (cart) {
            $scope.cart = cart;
            updateCouponVariables();
        }, function (failureResponse) {
            $scope.invalidCoupon = true; // probably 404 or 400 request
            updateCouponVariables();
        });

        return false;
    }

    $scope.finishTheOrder = function () {

        AppApi.Cart.finishOrder()
            .$promise.then(function (response) {
                $location.path("/cart/orderFinished");
            });
        return false;
    }


    var updateCouponVariables = function () {
        $scope.hasDiscount = $scope.cart.Coupon != null && $scope.hasItems;
        $scope.coupon = "";
    }

    var navigateAfterCartOperation = function (resource) {

        resource.$promise.then(function (cart) {

            var hasItems = cart != null && cart.Items.length > 0;
            $scope.hasItems = hasItems;

            if (hasItems) {
                $scope.cart = cart;
                $location.path("/cart");
            }
            else
                $location.path("/");
        });

        return false;
    }
}]);