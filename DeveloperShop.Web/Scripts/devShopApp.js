// define angular module
var devShopApp = angular.module('devShopApp', ['ngRoute', 'ngResource', 'ngCookies']);

// routes
devShopApp.config(['$routeProvider', '$httpProvider', function ($routeProvider, $httpProvider) {
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

    $httpProvider.interceptors.push('CartIdInterceptor');
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
                method: 'POST'
            }
        })
    };
}]);
devShopApp.factory('CartIdInterceptor', ['$q', '$cookies', function ($q, $cookies) {
    return {
        request: function (config) {
            config.headers = config.headers || {};
            var cartId = $cookies.get('cartID') ? $cookies.get('cartID') : "";
            config.headers['auth_cart_id'] = cartId;

            return config;
        },
        response: function (response) {
            return response || $q.when(response);
        }
    };
}]);


// controllers
devShopApp.controller('DevelopersController', ['$scope', '$resource', '$cookies', '$location', 'AppApi', function ($scope, $resource, $cookies, $location, AppApi) {

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

    $scope.showDetails = function (developerId) {

        $scope.devDetail = AppApi.Developers.get({ id: developerId });
        $("#popupDevDetails").modal("show");
        return false;
    }

    $scope.addNewItem = function (developerId) {

        $("#popupDevDetails").removeClass("fade").modal("hide").addClass("fade"); // bug when trying to hide bootstrap modal, had to remove fase class manually
        $location.path("/cart/addingItem/" + developerId);
        return false;
    }

}]);

devShopApp.controller('CartController', ['$scope', '$resource', '$routeParams', '$cookies', '$location', 'AppApi', function ($scope, $resource, $routeParams, $cookies, $location, AppApi) {

    $scope.devId = $routeParams.devId;

    if ($scope.devId > 0) {
        $scope.developer = AppApi.Developers.get({ id: $scope.devId });
        $scope.hours = 8;
    }
    else {
        $scope.cart = AppApi.Cart.get();
        $scope.cart.$promise.then(function (cart) {
            $cookies.put('cartID', cart.Id);
            $scope.hasItems = cart.Items != null && cart.Items.length > 0;
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
            .$promise.then(function (cart) {
                $cookies.remove('cartID', cart.Id);
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
            $cookies.put('cartID', cart.Id);

            var hasItems = cart.Items != null && cart.Items.length > 0;
            $scope.hasItems = hasItems;

            if (hasItems) {
                $scope.cart = cart;
                $location.path("/cart");
            }
            else
                $location.path("/");
        }, function (failureResponse) {
            if (failureResponse.status === 500) {
                alert("Internal Server Error. " + failureResponse.data.ExceptionMessage);
            } else if (failureResponse.status === 409) {
                alert("Error: Item already added in cart.");
            } else {
                alert("Error: " + failureResponse.message);
            }
        });

        return false;
    }
}]);


// directives
devShopApp.directive('myDevDetails', function () {
    return {
        templateUrl: '/Templates/Directives/devDetails.html',
        replace: true,
        scope: {
            devDetail: '=ngModel',
            showBackButton: '=showBackButton'
        }
    };
});
//devShopApp.directive('myDevDetails', function () {
//    return {
//        templateUrl: '/Templates/Directives/devDetails.html',
//        replace: true,
//        scope: {
//            devDetail: '=ngModel',
//            showBackButton: '=showBackButton'
//        }
//    };
//});