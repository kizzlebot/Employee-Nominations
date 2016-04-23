var app = angular.module('nominateApp', ['ui.router', 'ngAnimate', 'ui.bootstrap', 'angular-loading-bar']);



app.run(function ($rootScope, $http, $window, $location) {

    var url = $window.location.pathname; 
    var protocol = $location.protocol();
    var host = $location.host();
    var port = $location.port();
    $rootScope.root = ($window.VirtualPath != '/') ? $window.VirtualPath : "";


    $rootScope.root = protocol + "://" + host + ':' + port + url;
    $rootScope.employees = null;
    $rootScope.teams = null;
    $rootScope.names = null;
    $rootScope.empId = $window.empid;
    $rootScope.full_name = $window.full_name;
    $rootScope.all = null;

    var root = $location.url();
    $http.post($rootScope.root + '/Nomination/GetEmployees/all?group=NON-ATTORNEY STAFF', {}).then(function (response) {
        $rootScope.employees = response.data.data;
        $rootScope.names = response.data.data.map(function (entry) {
            return entry.full_name;
        });
        $http.post($rootScope.root + '/Nomination/GetEmployees/teams', {}).then(function (response) {
            $rootScope.teams = response.data.data;
            $rootScope.all = $rootScope.names.concat($rootScope.teams);
        });
    });


    $rootScope.formChoice = 0;
    $rootScope.$on('formchoice', function (val) {
        console.log('formchoice recieved ' + val);
        $rootScope.formChoice = val;
    });
});




app.config(function ($stateProvider, $urlRouterProvider) {

    $urlRouterProvider.otherwise('/');


    var root = window.location.pathname;

    $stateProvider
      // Main form 
      .state('main', {
          url: '/',
          //templateUrl: root + '/Content/partials/form.html',
          templateProvider: function ($rootScope, $location, $http, $window) {
              return $http.get($rootScope.root + '/Content/partials/form.html').then(function (response) {
                  return response.data;
              });
          },
          controller: function ($scope, $http, $filter, $state, $rootScope, $q, $window) {
              $scope.empId = window.empid;
              $scope.sortKey = 'full_name';
              $scope.formChoice = 0;
              $scope.nominationtype = "Employee Recognition Nomination";
              $scope.offset = 'col-md-offset-3';


              // Determine which subform to display in ui-view
              $scope.updatePartial = function (val) {
                  $state.go((($state.is('main')) ? '.form' : '^.form') + val);
                  $scope.$emit('formchoice', val);
                  $scope.formChoice = val;
                  $scope.nominationtype = ($scope.formChoice == 1) ? "Applause, Applause" : "Teamwork";
                  $scope.offset = 'col-md-offset-1';
              };







              // On Success signal, transistion state to Success page
              $scope.$on('Success', function () {
                  console.log("Success Signal Received");
                  $state.go('success');
              });
              // What to do on failure
              $scope.$on('Failure', function () {
                  console.log("Failure Signal Received");
              });
          }
      })

      // Subform: Employee Nomination
      .state('main.form1', {
          url: 'form1',
          //templateUrl: root + '/Content/partials/employeeform.html',
          templateProvider: function ($rootScope, $location, $http, $window) {
              return $http.get($rootScope.root + '/Content/partials/employeeform.html').then(function (response) {
                  return response.data;
              });
          },
          controller: function ($scope, $filter, $state, $rootScope, $q, $http, $location, $window) {
              $scope.employees = $rootScope.employees;
              $scope.selected = undefined;
              $scope.nominated = [];
              $scope.onSelect = function ($item, $model, $label) {
                  console.log($item);
              };
              $scope.submit = function () {
                  if ($scope.selected.employee_number === undefined) return;
                  var param = {
                      Nomination_Type: 0,
                      Nominator_Employee_Number: window.empid.toString(),
                      Nominator_Employee_Full_Name: window.full_name,
                      Nomination_Reason: $scope.reason,
                      Nominee_Employee_Number: $scope.selected.employee_number,
                      Nominee_Emp_or_Team_Name: $scope.selected.full_name
                  };

                  $http.post($rootScope.root + '/Nomination/Create', param).then(function (response) {
                      if (response.data.Message === "Success") $scope.$emit('Success');
                      else $scope.$emit('Failure');
                  });
              }
          }
      })
      // Subform: Team Nomination
      .state('main.form2', {
          url: 'form2',
          templateProvider: function ($rootScope, $location, $http, $window) {
              return $http.get($rootScope.root + '/Content/partials/teamform.html').then(function (response) {
                  return response.data;
              });
          },
          controller: function ($scope, $rootScope, $filter, $http, $location, $window, $state) {
              $scope.inputs = [{}];
              $scope.disabled = []; 
              $scope.selected = [];
              $scope.names = $rootScope.names;
              $scope.teams = $rootScope.teams;

              $scope.all = $rootScope.all;

              $scope.onSelect = function ($item, $model, $label, $index) {
                  $scope.disabled[$index] = true;
              };

              $scope.addInput = function () {
                  if ($scope.inputs.length === $scope.selected.length) $scope.inputs.push({});
              }


              $scope.submit = function (selects) {
                  // Validate all nominations
                  var selects = $scope.selected.map(function (data) {
                      var rtn = {                                                          // Item to append to 'selects' array
                          Nomination_Type: 1,
                          Nominator_Employee_Number: $scope.$root.empId,
                          Nominator_Employee_Full_Name: $scope.$root.full_name,
                          Nomination_Reason: $scope.reason
                      };


                      var src = $filter('filter')($scope.employees, { full_name: data });  // Try to find a employee of exact match of entry
                      if (src.length > 0) {                                                // If match found, then add attributes to rtn and add to selects array
                          rtn.Nominee_Employee_Number = src[0].employee_number;
                          rtn.Nominee_Emp_or_Team_Name = src[0].full_name;
                          return rtn; 
                      }
                      else if ($scope.all.indexOf(data) !== -1) {                          // If match found in $scope.all instead, add attributes to rtn and add to selects array
                          rtn.Nominee_Employee_Number = '';
                          rtn.Nominee_Emp_or_Team_Name = data;
                          return rtn; 
                      }
                      else return;                                                         // Otherwise skip
                  });


                  // Submit to Server
                  $http.post($rootScope.root + '/Nomination/CreateTeam', selects).then(function (response) {
                      if (response.data.Message === "Success") $scope.$emit("Success");
                      else $scope.$emit("Failure");
                  });
              }
              console.log($state.current);
          }
      })

      // Success form.
      .state('success', {
          url: '/success',
          templateProvider: function ($rootScope, $location, $http, $window){
              return $http.get($rootScope.root + '/Content/partials/success.html').then(function (response) {
                  return response.data;
              });
          },
          controller: function ($scope, $window) {
              $scope.exitWindow = function () {
                  $window.close();
              };
          }
      });
});


app.filter('exclude', function () {
    return function (emps, excludes) {
        angular.forEach(excludes, function (ex) {
            if (emps.indexOf(ex) !== -1) emps.splice(emps.indexOf(ex), 1);
        });
        return emps;
    };
});

