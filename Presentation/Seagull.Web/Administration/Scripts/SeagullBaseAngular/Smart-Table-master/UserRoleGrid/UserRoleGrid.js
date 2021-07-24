SeagullApp.controller('UserRoleGridAngular', ['Resource', '$http', '$scope', function (service, $http, $scope) {
    var ctrl = this;
    this.displayed = [];
    this.getdataFromServer = function callServer(tableState) {
        ctrl.isLoading = true;
        var pagination = tableState.pagination;
        var start = pagination.start || 0;     // This is NOT the page number, but the index of item in the list that you want to use to display the table.
        var number = pagination.number || 10;  // Number of entries showed per page.
        service.getPage(start, number, tableState).then(function (result) {
            ctrl.displayed = [];
            ctrl.displayed = result.data;
            tableState.pagination.totalItemCount = result.totalItemCount
            tableState.pagination.numberOfPages = result.numberOfPages;//set the number of pages so the pagination can update
            ctrl.isLoading = false;
        });
        $scope.EditRow = function (row) {
            var _url = EditRowUrl();
            document.location.href = _url + "/" + row.Id;
        };
        $scope.RemoveRow = function (row) {

            //$http service that send or receive data from the remote server
            $http({
                method: 'POST',
                url: RemoveRowUrl(),
                data: { id: row.Id },
            }).then(function successCallback(data, status, headers, config) {
                // this callback will be called asynchronously
                // when the response is available
                if (data.data.success) {
                    $("#header").html("Success");
                    $("#divbody").html("Deleted Sucssesfully");
                    $("#UserRoleModal").modal('show');
                    document.location.href = data.data.url;
                }
                else {
                    var error = "";
                    $.each(data.data.errors, function (index, value) {
                        error += "," + value;
                    });
                    $("#header").html("Error");
                    $("#divbody").html(error);
                    $("#UserRoleModal").modal('show');
                }
            }, function errorCallback(data, status, headers, config) {
                // called asynchronously if an error occurs
                // or server returns response with an error status.

            });
        };
    };
}]).directive('pageSelect', function () {
    return {
        restrict: 'E',
        template: '<input type="text" class="select-page" ng-model="inputPage" ng-change="selectPage(inputPage)">',
        link: function (scope, element, attrs) {
            scope.$watch('currentPage', function (c) {
                scope.inputPage = c;
            });
        }
    }
}).config(['$httpProvider', function ($httpProvider) {
    var antiForgeryToken = document.getElementById('antiForgeryToken_UserRoleGrid').value;
    $httpProvider.defaults.headers.post['__RequestVerificationToken'] = antiForgeryToken;
}]).factory('Resource', ['$q', '$filter', '$timeout', '$http', function ($q, $filter, $timeout, $http) {

    //this would be the service to call your server, a standard bridge between your model an $http
    var randomsItems = [];
    // the database (normally on your server)
    //fake call to the server, normally this service would serialize table state to send it to the server (with query parameters for example) and parse the response
    //in our case, it actually performs the logic which would happened in the server
    function getPage(start, number, params) {
        var deferred = $q.defer();
        $http({
            method: 'POST',
            url: '/Admin/UserRole/ListAngular',
            data: { pagination: params.pagination, sort: params.sort, search: JSON.stringify(params.search.predicateObject) }
        }).then(function successCallback(data, status, headers, config) {
            // this callback will be called asynchronously
            // when the response is available
            randomsItems = [];
            angular.forEach(data.data.data, function (value, key) {
                randomsItems.push({ Id: value.Id, Name: value.Name, Active: value.Active, IsSystemRole: value.IsSystemRole });
            });
            deferred.resolve({
                data: randomsItems,
                numberOfPages: data.data.numberOfPages,
                totalItemCount: data.data.totalItemCount

            });
        }, function errorCallback(data, status, headers, config) {
            // called asynchronously if an error occurs
            // or server returns response with an error status.
        });
        return deferred.promise;
    }
    return {
        getPage: getPage
    };
}]).config(['$qProvider', function ($qProvider) {
    $qProvider.errorOnUnhandledRejections(false);
}]);