declare var angular: any;

var app = angular.module("app", ["ngRoute", "templates-main"]);

app.config(['$routeProvider', function ($routeProvider) {
    $routeProvider
        .when("/", {
            templateUrl: "main.html",
            controller: "mainCtrl as ctrl"
        })
        .when("/other", {
            templateUrl: "other.html",
            controller: "otherCtrl as ctrl"
        })
        .otherwise({
            redirectTo: "/"
        });
}]);

app.run(["$templateCache", "$compile", "$rootScope", function ($templateCache, $compile, $rootScope) {
    var templatesHTML = $templateCache.get('demo-templates');
    $compile(templatesHTML)($rootScope);

    console.log('app is running');
}]);