
var adminApp = angular.module("adminApp", ["ngRoute", "ui", "ngTagsInput", "chart.js", "ui.bootstrap", ]);

adminApp.config(function ($routeProvider) {

    $routeProvider
        .when('/users',
           {
               templateUrl: '/public/angular-templates/adminapp/users.html',
               controller: 'usersController'
           }
         )
        .when('/issues',
           {
               templateUrl: '/public/angular-templates/adminapp/issues.html',
               controller: 'issuesController'
           }
         )
        .when('/products',
           {
               templateUrl: '/public/angular-templates/adminapp/products.html',
               controller: 'productsController'
           }
         )
        .when('/analytics',
            {
                templateUrl: '/public/angular-templates/adminapp/analytics.html',
                controller : 'analyticsController'
            }
        )
        .when('/roles',
            {
                templateUrl: '/public/angular-templates/adminapp/roles.html',
                controller: 'rolesController'
            }
        )
        .when('/',
           {
               templateUrl: '/public/angular-templates/adminapp/productDetail.html',
               controller: 'dashboardController'
           }
         )
    

});

adminApp.run(function ($rootScope) {
    $rootScope.areIssuesPending = false;
    $rootScope.areUsersPending = false;
});

adminApp.controller('rolesController', ['$scope', '$http',
    function ($scope, $http) {
        $scope.errorMessage = {
            description: null,
            show: false
        };
        $scope.$watch("errorMessage.show", function (newVal) {
            $('#errorModal').modal(newVal ? 'show' : 'hide');
        }, false)

        $http.get('/api/roles').success(function (response) {
            $scope.roles = response;
            console.log(response);
        })
        $scope.putRole = function (role) {
            $http.put('/api/roles/' + role.Id, JSON.stringify(role)).success(function (response) {
                role = response;
            }).error(function (response) {
                $scope.errorMessage.description = "Role name exists."
                $scope.errorMessage.show = true;
            });
        }
        $scope.deleteRole = function (role) {
            if (role.Name.toLowerCase() == "admin") {
                $scope.errorMessage.description = "Cannot delete admin role";
                $scope.errorMessage.show = true;
                return;
            }
            if (role.Users && role.Users.length > 0) {
                $scope.errorMessage.description = "Role currently has users attached so role cannot be deleted."
                $scope.errorMessage.show = true;
                return;
            }
            $http.get('/api/roles/'+role.Id).success(function (response) {
                role = response;
                if (role.Users && role.Users.length > 0) {
                    $scope.errorMessage.description = "Role currently has users attached so role cannot be deleted."
                    $scope.errorMessage.show = true;
                    return;
                }
                $http.delete('/api/roles/'+role.Id).success(function (response) {
                    var index = $scope.roles.indexOf(findObj($scope.roles,role.Id));
                    $scope.roles.splice(index, 1);
                });
            });
        }
        $scope.postRole = function (role) {
            $http.post('/api/roles' , JSON.stringify(role)).success(function (response) {
                $scope.roles.push(response);
                role = {
                    Name: ''
                };
                $('#addRoleModal').modal('hide');
                console.log(response);
            }).error(function(response){
                $scope.errorMessage.description = "Role name exists."
                $scope.errorMessage.show = true;
            });
        }
    }
]);

adminApp.controller('navController', ['$scope', '$rootScope', '$http',

    function ($scope, $rootScope, $http) {

        $http.get('/api/issues').success(function (response) {
            
            $rootScope.areIssuesPending = areIssuesPending(response);
        }).error(function(response){});

        $http.get('/api/users').success(function (response) {
            $rootScope.areUsersPending = areUsersPending(response);
        });

    
    }
    
]);

adminApp.controller('analyticsController', ['$scope', '$rootScope', '$http',

    function ($scope, $rootScope, $http) {
        var self = this;
        var timeIter = {
            "ms": 1
        };
        timeIter.sec = timeIter.ms * 1000;
        timeIter.min = timeIter.sec * 60;
        timeIter.hour = timeIter.min * 60;
        timeIter.day = timeIter.hour * 24;
        timeIter.week = timeIter.hour * 7;
        
        var entityEnum = {
            products: 'products',
            modules: 'modules',
            activities: 'activities'
        }
        $scope.showDetail = false;
        $scope.showMaster = true;
        $scope.users = {};
        $scope.entityEnum = entityEnum;

        var maxDate = new Date();
        $scope.dateOptions = {
            maxDate : maxDate
        }
        $scope.date = {
            start: new Date(),
            end: new Date()
        }
        $scope.date.start.setDate($scope.date.start.getDate() - 7);

        $scope.date.start.setHours(0, 0, 0);

        $scope.date.end.setHours(11, 59, 59);

        $scope.options = {
            scaleLabel: "<%=Math.abs(value).toFixed(1)%>",
            labelsFilter: function(label, index){
                return false;
            },
            tooltips: {
                enabled:true
            }
            //scales: {
            //    yAxes: [
            //      {
            //          id: 'y-axis-1',
            //          type: 'linear',
            //          display: true,
            //          position: 'left'
            //      }
            //    ]
            //}
        };
        $scope.datasetOverride = {
            lineTension: 0,
        };
        
        $scope.pie = {
            labels: [],
            values: []
        }
        $scope.detail = {
            labels: [],
            values: []
        }
        $scope.userGraph = {
            labels: [],
            values: []
        }
        $scope.pie.colors = [
            '#1482CC',
            '#f1595f',
            '#B33B3A',
            '#FFFD19',
            '#79c36a',
            '#599ad3',
            '#f9a65a',
            '#9e66ab',
            '#cd7058',
            '#d77fb3'
        ];
        $scope.detail.colors = [
            '#B33B3A',
        ]


       
        $scope.pieClick = function (pts, evt) {
            var selectedEntityArray = $scope[$scope.entityName];
            var selectedEntity = selectedEntityArray[pts[0]._index];
            switch ($scope.entityName) {
                case entityEnum.products:
                    $scope.showMaster = true;
                    $scope.showDetail = true;
                    $scope.getProductModuleData(selectedEntity.Id);
                    break;
                case entityEnum.modules:
                    $scope.showMaster = true;
                    $scope.showDetail = true;
                    $scope.getModuleActivityData(selectedEntity.Id);
                    break;
                case entityEnum.activities:
                    $scope.showMaster = false;
                    $scope.showDetail = true;
                    $scope.getActivityData(selectedEntity.Id);
                    break;
                default:
                    $scope.showMaster = true;
                    $scope.showDetail = false;
                    $scope.getProductData();
                    return;
            }
        }

      
        $scope.startDateChange = function (date) {

            $scope.date.start = new Date(date);
            $scope.date.start.setHours(0, 0, 0);
            $scope.date.end = new Date($scope.date.end);
            $scope.date.end = new Date($scope.date.end.setHours(11, 59, 59));

            $http.get('/api/views/' + $scope.entityName + "/" + $scope.selected.Id + "?" + self.dateUrlSelector($scope.date.start, $scope.date.end)).then(function (response) {

                var diffInDays =  Math.floor(($scope.date.end.getTime() - $scope.date.start.getTime()) / (timeIter.day))
                var timeOption = "day";
                if (diffInDays < 3) {
                    timeOption = "hour";
                } if (diffInDays > 30) {
                    timeOption = "week";
                }
                self.getDetailGraphData(response.data, timeOption);
            });
        }
        $scope.endDateChange = function (date) {
            $scope.date.end = new Date(date);
            $scope.date.start = new Date($scope.date.start);
            $scope.date.start.setHours(0, 0, 0);
            $scope.date.end = new Date($scope.date.end);
            $scope.date.end = new Date($scope.date.end.setHours(11, 59, 59));
            

            $http.get('/api/views/' + $scope.entityName + "/" + $scope.selected.Id + "?" + self.dateUrlSelector($scope.date.start, $scope.date.end)).then(function (response) {
               
                var diffInDays = Math.floor(($scope.date.end.getTime() - $scope.date.start.getTime()) / (timeIter.day))
                var timeOption = "day";
                if (diffInDays < 3) {
                    timeOption = "hour";
                } if (diffInDays > 30) {
                    timeOption = "week";
                }
                self.getDetailGraphData(response.data, timeOption);

            });
        }
        

        $scope.goBack = function (name) {
            switch (name) {
                case entityEnum.activities:
                    $scope.getProductModuleData($scope.product.Id);
                    break;
                default:
                    $scope.getProductData();
                    return;
            }
        }
       
        $scope.getProductData = function () {
            $scope.entityName = entityEnum.products;
            $scope.selected = null;
            $http.get('/api/products').then(function (response) {
                self.getPieGraphData(response.data, $scope.entityName);
               
            });
            $scope.showDetail = false;
        }
        $scope.getProductModuleData = function (productId) {
            $scope.entityName = entityEnum.modules;
            /*
            * Get product info
            */
            $http.get('/api/products/' + productId).then(function (response) {
                $scope.product = response.data;
                $scope.selected = response.data;

                // get product's modules with views to separate into per module on pie graph
                self.getPieGraphData(response.data.Modules, $scope.entityName);
            });
            
            $http.get('/api/views/products/' + productId + "?" + self.dateUrlSelector($scope.date.start, $scope.date.end)).then(function (response) {
                self.getDetailGraphData(response.data, "day");
            });
        }
        $scope.getModuleActivityData = function (moduleId) {
            $scope.entityName = entityEnum.activities;
            $http.get('/api/modules/' + moduleId).then(function (response) {
                $scope.module = response.data;
                $scope.selected = response.data;
                self.getPieGraphData(response.data.Activities, $scope.entityName);
            });
            $http.get('/api/views/modules/' + moduleId + "?" + self.dateUrlSelector($scope.date.start, $scope.date.end)).then(function (response) {
                
                self.getDetailGraphData(response.data, "day");
            });
        }

        self.getPieGraphData = function (entities, entityName) {
            ;
            $scope.pie.labels = entities.map(function (x) {
                return x.Name;
            });
            $scope.pie.values = Array.apply(null, Array(entities.length)).map(Number.prototype.valueOf, 0);
            $scope[entityName] = entities;
            $scope.users = [];
            var users = [];
            angular.forEach($scope[entityName], function (entity) {
                $http.get('/api/views/' + entityName + '/' + entity.Id + '?with=users').then(function (response) {
                    for (var i = 0; i < response.data.length; i++) {
                        var view = response.data[i];
                        if (view.UserId == null) {
                            continue;
                        }
                        var user = findObj($scope.users, view.UserId);
                        
                        if(!user){
                            user = $scope.users.push({
                                Id : view.UserId,
                                FirstName: view.User.FirstName,
                                LastName: view.User.LastName,
                                Roles : view.User.Roles,
                                count : 0
                            });
                        }
                        user.count++;
                        
                    }                  
                    entity = findObj(entities, entity.Id);
                    $scope.pie.values[$scope.pie.labels.indexOf(entity.Name)] = response.data.length;
                })
                
            });
            users = $scope.users;
            ;
            $scope.userGraph.labels = users.map(function (user) {
                return user.FirstName + " " + user.LastName
            })
            $scope.userGraph.values = users.map(function (user) {
                
                return user.count
            })
        }

        self.getDetailGraphData = function (entities, timeOption) {
            $scope.detail.labels = [];
            $scope.detail.values = [];
            
            var xValues = self.getXValues(entities, timeIter[timeOption]);
            var yValues = self.getYValues(entities, xValues);
            
            //$scope.xValues = xValuesUtc;
            $scope.detail.labels = self.toTimeStrings(xValues, timeOption);
            $scope.detail.values = yValues;
            $scope.showDetail = true;

            $scope.options.scaleStartValue = $scope.detail.labels[0];
            
        }


        $scope.getProductData();

        $scope.getActivityData = function (activityId) {
            $scope.entityName = entityEnum.activities;
            $http.get('/api/views/activities/' + activityId + "?" + self.dateUrlSelector($scope.date.start, $scope.date.end)).then(function (response) {

                self.getDetailGraphData(response.data, "day");
            })
        }

        self.getYValues = function (views, xValues) {
            var yValues = Array.apply(null, Array(xValues.length)).map(Number.prototype.valueOf, 0);
            for (var i = 0; i < views.length ; i++) {
                var view = views[i];
                view.ViewedAt = new Date(view.ViewedAt);
                for (var j = 0; j < xValues.length; j++) {
                    var currTime = new Date(xValues[j]);
                    var nextTime = new Date(xValues[j + 1]);
                    if (view.ViewedAt.getTime() >= currTime.getTime() && (view.ViewedAt.getTime() < nextTime.getTime() || j + 1 == xValues.length)) {
                        yValues[j]++;
                        
                        
                        
                        
                        break;
                    }
                }
            }

            return yValues;
        }

        self.getXValues = function (views, timeIterOption) {

            //var maxTime = Math.max.apply(Math, views.map(function (o) { return new Date(o.ViewedAt); }));
            //var minTime = Math.min.apply(Math, views.map(function (o) { return new Date(o.ViewedAt); }));
            var minTime = $scope.date.start.getTime();
            var endTime = new Date($scope.date.end);
            endTime.setHours(23, 59, 59);
            var maxTime = endTime.getTime();
            console.log(new Date(minTime), new Date(maxTime))
            
            var xValues = [];
            for (var i = minTime; i <= maxTime; i += timeIterOption) {
                xValues.push(i);
            }
            return xValues;

        }
        self.dateUrlSelector = function (startDate,endDate) {
            return "startDate=" + self.dateToString(new Date(startDate)) + "&endDate=" + self.dateToString(new Date(endDate));
        }
        self.dateToString = function (date) {
            return date.getMonth() + 1 + "/" + date.getDate() + "/" + date.getFullYear();
        }
        self.mapToUtc = function (times) {
            return times.map(function (x) {
                var dateObj = new Date(x);
                return new Date(Date.UTC(dateObj.getFullYear(), dateObj.getMonth(), dateObj.getDate(), dateObj.getHours() , 0, 0));

            })
        }

        self.toTimeStrings = function (xValues, timeOption) {
            var newValues;
            switch (timeOption) {

                case "hour":
                    newValues = xValues.map(function (date) {
                        var dateObj = new Date(date);
                        //
                        //
                        //dateObj.setMinutes(dateObj.getMinutes() + dateObj.getTimezoneOffset());
                        //
                        //
                        return self.getMonth(dateObj) + " " + dateObj.getDate() + " " + self.getHour(dateObj);
                        
                    })
                    break;
                case "day":
                    newValues = xValues.map(function (dateObj) {
                        dateObj = new Date(dateObj);
                        return self.getMonth(dateObj) + " " + dateObj.getDate();
                    });
                    break;
                case "week":
                    newValues = xValues.map(function (date) {
                        var dateObj = new Date(date);
                        var lastWeek = new Date(date);
                        lastWeek.setDate(lastWeek.getDate() - 7);
                        return self.getMonth(lastWeek) + " " + lastWeek.getDate() + "-" + self.getMonth(dateObj) + " " + dateObj.getDate();
                    });
                    break;
                default:
                    newValues = xValues.map(function (dateObj) {
                        return dateObj;
                    });

            }
            return newValues;
        }


        self.getHour = function (date) {
            var hours = date.getHours() + 1;
            var ampm = hours >= 12 && hours != 24 ? "pm" : "am";
            var hours = hours % 12;
            if (hours == 0) {
                hours = 12;
            }
            return hours + " " + ampm;
        }
        
        self.getMonth = function (date) {
            var locale = "en-us",
            month = date.toLocaleString(locale, { month: "short" });
            return month;
        }
    }
    
]);


adminApp.controller('usersController', ['$scope', '$rootScope', '$http',

    function ($scope, $rootScope, $http) {
        $http.get('/api/users').success(function (response) {
            $scope.users = response;
        })

        $scope.authenticateUser = function (user) {
            $http.post('/api/users/' + user.Id + '/authenticate').success(function (response) {
                user.IsAuthenticated = true;
                $rootScope.areUsersPending = areUsersPending($scope.users);
            });
        }

        $scope.deauthenticateUser = function (user) {
            $http.post('/api/users/' + user.Id + '/deauthenticate').success(function (response) {
                user.IsAuthenticated = false;
                $rootScope.areUsersPending = areUsersPending($scope.users);
            })
        }

        $scope.deleteUser = function (user) {
            $http.delete('/api/users/' + user.Id ).success(function (response) {
                var index = $scope.users.indexOf(user);
                $scope.users.splice(index, 1);
            })
        }
    }

]);

adminApp.controller('issuesController', ['$scope', '$rootScope', '$http',

    function ($scope, $rootScope, $http) {
        $http.get('/api/issues').success(function (response) {
            $scope.issues = response;
            
        })

        $scope.getIssue = function (issue) {
            $scope.issue = issue;
        };

        $scope.getSolution = function (issue) {
            $scope.getIssue(issue);
            $scope.solution = issue.Solutions[0];
        }

        $scope.postSolution = function () {
            var issue = findObj($scope.issues, $scope.issue.Id);
            var data = $scope.postSolutionObj;
            data.issueId = $scope.issue.Id;
            $http.post('/api/solutions', JSON.stringify(data)).success(function (response) {
                $scope.postSolutionObj = {
                    Name: '',
                    Description:''
                }
                issue.Solutions.push(response);
                $('#postSolutionModal').modal('hide');
                $rootScope.areIssuesPending = areIssuesPending($scope.issues);
            }).error(function(reposnse){
            })
        }
    }

]);

adminApp.controller('productsController', ['$scope', '$http',

    function ($scope, $http) {

        $http.get('/api/products').success(function (response) {
            $scope.products = response;
        })
        $scope.postProduct = function () {
            $http.post('/api/products', JSON.stringify($scope.product)).success(function (response) {
                $scope.products.push(response);
                $scope.product = {
                    Name: '',
                    Description: ''
                };
                $('#addProductModal').modal('hide');
            }).error(function (error) {
                
            });
        }
        $scope.deleteProduct = function (product) {
            $http.delete('/api/products/' + product.Id).success(function (response) {
                var index = $scope.products.indexOf(product);
                $scope.products.splice(index, 1);

            }).error(function (error) {
                
            })
        }
        //$http.get('/api/modules').success(function (response) {
        //    $scope.modules = response;
        //})

        //$http.get('/api/activities').success(function (response) {
        //    $scope.activities = response;
        //    
        //})

        //$scope.postActivity = function () {
        //    $http.post('/api/activities').success(function (response) {
        //        
        //    }).error(function (error) {
        //        
        //    })
        //}
    }

]);

adminApp.controller('dashboardController', ['$scope', '$http', '$q',

function ($scope, $http, $q) {
    var self = this;
    $scope.httpDone = false;
    $scope.errorMessage = {
        description: null,
        show: false
    };
    $scope.$watch("errorMessage.show", function (newVal) {
        $('#errorModal').modal(newVal ? 'show' : 'hide');
    }, false)
    $http.get('/api/products').success(function (response) {
        $scope.products = response;
    })

    $http.get('/api/tags').success(function (response) {
        $scope.tags = response;
    });

    $scope.getProduct = function (product) {
        $http.get('/api/products/' + product.Id).success(function (response) {
            $scope.product = response;
            
            delete $scope.module;
            delete $scope.activity;
        })
    }

    $scope.postProduct = function () {
        var data = $scope.postProductObj;
        $http.post('/api/products', JSON.stringify(data)).success(function (response) {
            $scope.products.push(response);
            $scope.postProductObj = {
                Name: '',
                Description: ''
            };
            $('#addProductModal').modal('hide');
        }).error(function (error) {
            
        });
    }
    $scope.putProduct = function (product) {
        $http.put('/api/products/' + product.Id, JSON.stringify(product)).success(function (response) {
            product = response;
            
        }).error(function (error) {
            
        })
    }

    $scope.deleteProduct = function (product) {
        $http.get('/api/products/' + product.Id).success(function (response) {
            if (response.Modules.length > 0) {
                //error message
                $scope.errorMessage.show = true;
                $scope.errorMessage.description = 'This product cannot be deleted because it has dependent activities!';
                return;
            }
            $http.delete('/api/products/' + product.Id).success(function (response) {
                var index = $scope.products.indexOf(product);
                $scope.products.splice(index, 1);

            }).error(function (error) {

            })
        });
    }

    $scope.getModule = function (module) {
        $http.get('/api/modules/' + module.Id).success(function (response) {
            $scope.module = response;
            delete $scope.activity;
        })
    }

    $scope.postModule = function () {
        var data = $scope.postModuleObj;
        data.productId = $scope.product.Id;
        
        $http.post('/api/modules', JSON.stringify(data)).success(function (response) {
            $scope.product.Modules.push(response);
            $scope.postModuleObj = {
                Name: ''
            }
            $('#addModuleModal').modal('hide');
        }).error(function (error) {
            
        })
    }
    $scope.putModule = function (module) {
        $http.put('/api/modules/' + module.Id, JSON.stringify(module)).success(function (response) {
            module = response;
            
        }).error(function (error) {
            
        })
    }
    $scope.deleteModule = function (module) {
        //call to get object to see if it has dependencies
        $http.get('/api/modules/' + module.Id).success(function (response) {
            if (response.Activities.length > 0) {
                //error message
                $scope.errorMessage.show = true;
                $scope.errorMessage.description = 'This module cannot be deleted because it has dependent topics!';
                return;
            }
            $http.delete('/api/modules/' + module.Id).success(function (response) {
                var index = $scope.product.Modules.indexOf(module);
                $scope.product.Modules.splice(index, 1);

            }).error(function (error) {
                
            })
        });
    }

    $scope.getActivity = function (activity) {
        $http.get('/api/activities/' + activity.Id + '?include=steps,tags').success(function (response) {
            $scope.activity = response;
            
        })
    }


    $scope.postActivity = function () {
        var data = $scope.postActivityObj;
        data.moduleId = $scope.module.Id;
        
        $http.post('/api/activities', JSON.stringify(data)).success(function (response) {
            $scope.module.Activities.push(response);
            $scope.postActivityObj = {
                Name: '',
                Description : ''
            }
            $('#addActivityModal').modal('hide');
        }).error(function (error) {
            
        })
    }


    $scope.putActivity = function (activity) {
        $http.put('/api/activities/' + activity.Id, JSON.stringify(activity)).success(function (response) {
            activity = response;
            
        }).error(function (error) {
            
        })
    }

    $scope.deleteActivity = function (activity) {
        //call to get object to see if it has dependencies
       
            $http.delete('/api/activities/' + activity.Id).success(function (response) {
                var index = $scope.module.Activities.indexOf(activity);
                $scope.module.Activities.splice(index, 1);

            }).error(function (error) {

                
            })
    }

    $scope.postStep = function () {
        var data = $scope.postStepObj;
        data.activityId = $scope.activity.Id;
        data.order = $scope.activity.Steps.length == null ? 1 : $scope.activity.Steps.length + 1;
        $http.post('/api/steps', JSON.stringify(data)).success(function (response) {
            $scope.activity.Steps.push(response);
            $scope.postStepObj = {
                Name: '',
                Description: ''
            }
            $('#addStepModal').modal('hide');
        }).error(function (error) {
            console.log(error)
        })
    }

    $scope.deleteStep = function (step) {
        $http.delete('/api/steps/' + step.Id).success(function (response) {
            var index = $scope.activity.Steps.indexOf(step);
            $scope.activity.Steps.splice(index, 1);

        }).error(function (error) {
            
        })
    }

    $scope.putStep = function (step) {
        $http.put('/api/steps/'+step.Id, JSON.stringify(step)).success(function (response) {
            step = response;
            
        }).error(function (error) {
            
        })
    }

    $scope.manageTags = function (activity, tags) {
        for (var i = 0; i < tags.length; i++) {
            var tmpTag = tags[i];
            var tag = self.getTagByName(tmpTag.Name);
            if (tag) {
                self.attachTagToActivity(activity, tag);
            }
            else {
                self.postTagAndAttach(activity, tmpTag)
            }
        }
    }

    $scope.addTag = function (activity, tag) {
        var existingTag = self.getTagByName(tag.Name);
        if (existingTag) {
            self.attachTagToActivity(activity, existingTag);
        }
        else {
            self.postTagAndAttach(activity, tag);
        }
    }

    this.attachTagToActivity = function (activity, tag) {
        $http.put('/api/activities/' + activity.Id + '/tags/' + tag.Id).success(function (response) {
            activity = response;
            
        }).error(function (error) {
            
        })
    }

    $scope.detachTagFromActivity = function (activity, tag) {
        ;
        var tag = self.getTagByName(tag.Name);
        ;
        $http.delete('/api/activities/' + activity.Id + '/tags/' + tag.Id + '/detach').success(function (response) {
            activity = response;
            ;
        }).error(function (error) {
            
        })
    }

    this.postTagAndAttach = function (activity,tag) {
        ;
        //var data = { name: tagName };
        return $http({
            url: '/api/tags',
            method: 'POST',
            async: false,
            headers: {
                'Content-Type': 'application/json'
            },
            data:(JSON.stringify(tag))
        })
            .then(function (response) {
                ;
                $scope.tags.push(response.data);
                self.attachTagToActivity(activity, response.data)
                return response;
            })
            //.success(function (response) {
            //;
            //return response;
            //})
            //.error(function (response) {  });
    }

    this.getTagByName= function(tagName){
        for(var i=0; i<$scope.tags.length;i++){
            if ($scope.tags[i].Name.toLowerCase() == tagName.toLowerCase()) {
                return $scope.tags[i];
            }
        }
        return false;
    }

    $scope.sortOptions = {    
        update: function (e, ui) {
        },
        stop: function (e, ui) {
            $scope.updateStepOrder($scope.activity.Steps);
        }
    }
    $scope.updateStepOrder = function (steps) {
        for (var i = 0; i < steps.length; i++) {
            steps[i].Order = i + 1;
            $scope.putStep(steps[i]);
        }
    }
}

]);
function areIssuesPending(issues) {
    for (var i = 0; i < issues.length; i++) {
        var issue = issues[i];
        if (issue.Solutions.length == 0) {
            return true;
        }
        else {
            if (!issue.Solutions[0].IsCorrect) {
                return true;
            }
        }
    }
    return false;
}

function areUsersPending(users) {

    for (var i = 0; i < users.length; i++) {
        if (!users[i].IsAuthenticated) {
            return true;
        }
    }
    return false;
}
function findObj(list, id) {
    for (var i = 0; i < list.length; i++) {
        if (list[i].Id == id) {
            return list[i];
        }
        
    }
    return null;
}
String.prototype.capitalize = function () {
    return this.charAt(0).toUpperCase() + this.slice(1);
}
