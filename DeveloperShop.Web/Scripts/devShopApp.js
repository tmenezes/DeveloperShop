//Define an angular module for our app
var devShopApp = angular.module('devShopApp', ['ngRoute', 'ngResource']);

//Define Routing for app
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
      otherwise({
          redirectTo: '/'
      });
});

//devShopApp.factory('Developers', ['$resource', function ($resource) {
//    return $resource('/developers/:id', null,
//        {
//            getFromGithub: {
//                url: '/developers/fromGithub/:query',
//                method: 'GET'
//                //params: {
//                //    query: '@query'
//                //}
//            }
//        });
//}]);



devShopApp.controller('DevelopersController', function ($scope, $resource) {

    var Developers = $resource('api/developers/:id', { id: '@id' }, {
        getFromGithub: {
            url: 'api/developers/fromGithub/:query',
            method: 'GET'
            //params: {
            //    query: '@query'
            //}
        }
    });

    $scope.developers = Developers.query();
    $scope.developerUsername = "";

    $scope.searchDeveloperOnGithub = function () {

        if ($scope.developerUsername === "") return false;

        var dev = Developers.getFromGithub({ query: $scope.developerUsername });
        if (dev != null) {
            $scope.developers.push(dev);
            $scope.developerUsername = "";
        }
        else {
            alert("Developer was not found.");
        }

        return false;
    }
});


devShopApp.controller('CartController', function ($scope) {

    $scope.message = 'This is Show orders screen';

});