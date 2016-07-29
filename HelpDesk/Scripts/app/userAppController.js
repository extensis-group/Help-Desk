
var userApp = angular.module("userApp", ["ngRoute"]);

userApp.config(function ($routeProvider) {

    $routeProvider

        .when('/topics/:activityId',
           {
               templateUrl: '/public/angular-templates/userapp/activity.html',
               controller: 'activityController'
           }
         )

        .when('/steps/:stepId/issues',
           {
               templateUrl: '/public/angular-templates/userapp/issues.html',
               controller: 'issuesController'
           }
         )

        .when('/issues/:issueId',
           {
               templateUrl: '/public/angular-templates/userapp/issueDetail.html',
               controller: 'issueDetailController'
           }
         )

        .when('/search/:searchTerm',
           {
               templateUrl: '/public/angular-templates/userapp/search.html',
               controller: 'searchController'
           }
         )

        .when('/',
           {
               templateUrl: '/public/angular-templates/userapp/search.html',
               controller: 'searchController'
           }
         )

});

userApp.run(function ($rootScope) {

});

userApp.controller('activityController', ['$scope', '$routeParams', '$http',

        function ($scope, $routeParams, $http) {
            var activityId = $routeParams.activityId;
            console.log(activityId);
            $http.get('/api/activities/'+activityId+'?include=steps').then(function (response) {
                $scope.activity = response.data;
            });

        }
]);
userApp.controller('issuesController', ['$scope', '$routeParams', '$http',

        function ($scope, $routeParams, $http) {
            var stepId = $routeParams.stepId;
            $http.get('/api/steps/' + stepId).then(function (response) {
                $scope.step = response.data;
                console.log(response);
                $http.get('/api/activities/' + $scope.step.ActivityId + '?include=steps').then(function (response) {
                    $scope.activity = response.data;
                });
            });

            $scope.postIssue = function () {
                var data = $scope.issue;
                data.stepId = stepId;
                $http.post('/api/issues', data).success(function (response) {
                    $scope.step.Issues.push(response);
                    $scope.issue = {
                        Name: '',
                        Description:''
                    }
                    $('#postIssueModal').modal('hide');
                    $('#issueSuccessModal').modal('show');
                }).error(function(error){
                    console.log(error);
                });
            };

        }
]);
userApp.controller('issueDetailController', ['$scope', '$routeParams', '$http',

        function ($scope, $routeParams, $http) {
            var issueId = $routeParams.issueId;
            $http.get('/api/issues/' + issueId).then(function (response) {
                console.log(response);
                $scope.issue = response.data;
            });

        }
]);

userApp.controller('searchController', ['$scope', '$routeParams', '$http', '$location',

    function ($scope, $routeParams, $http, $location) {
        var self = this;
        $scope.results = {
            activities: [],
            issues : []
        }
        $scope.searchTerm = $routeParams.searchTerm;

        $scope.searchRefresh = function (searchTerm) {
            //self.searchTags(searchTerm);
            if (searchTerm.length < 3) {
                return;
            }
            $location.path("/search/" + searchTerm);
        }
        //this.searchTags = function (term) {
        //    $http.get('/api/tags?search='+term).success(function (response) {
        //        console.log(response);
        //    });
        //}
        self.searchActivities = function (term) {
            $http.get('/api/activities?search=' + term).success(function (response) {
                $scope.results.activities = response;
            });
        }
        self.searchIssues = function (term) {
            $http.get('/api/issues?search=' + term).success(function (response) {
                $scope.results.issues = response;
            });
        }
        $scope.search = function (term) {

            self.searchActivities($scope.searchTerm);
            self.searchIssues($scope.searchTerm);
        }
        $scope.search($scope.searchTerm);
        $scope.isEmptyPage = function () {
            return $scope.searchTerm == null || $scope.searchTerm.trim() === '' ;
        }
    }
    
]);
userApp.controller('sidebarController', ['$scope', '$routeParams', '$http', '$location',

        function ($scope, $routeParams, $http, $location) {
            $http.get('/api/products').then(function (response) {
                $scope.products = response.data;
            });

            $scope.getProductDetails = function (productId) {
                var product = findObj($scope.products, productId);
                $http.get('/api/products/' + productId).then(function (response) {
                    product.Modules = response.data.Modules;
                });
            };

            $scope.getModuleDetails = function (moduleId, productId) {
                var product = findObj($scope.products, productId);
                var module = findObj(product.Modules, moduleId);
                $http.get('/api/modules/' + moduleId).then(function (response) {

                    module.Activities = response.data.Activities;
                });
            };

            $scope.getActivityDetails = function (activityId, moduleId, productId) {
                var product = findObj($scope.products, productId);
                var module = findObj(product.Modules, moduleId);
                var activity = findObj(module.Activities, activityId);
                $http.get('/api/activities/' + activityId + '?include=steps,tags').then(function (response) {
                    activity.Steps = response.data.Steps;
                    console.log(response.data);
                });
            }

            $scope.searchActivities = function (term) {
                $location.path("/search/" + term);
                $scope.searchTerm = "";
            }

            function updateObjectDetail(oldArray, newObject) {
                for (var i = 0; i < oldArray.length; i++) {
                    if (oldArray[i].Id == newObject.Id) {
                        oldArray[i] = newObject;
                        break;
                    }
                }
            }
            function findObj(list, id) {
                for (var i = 0; i < list.length; i++) {
                    if (list[i].Id == id) {
                        return list[i];
                    }
                }
            }
        }
]);
