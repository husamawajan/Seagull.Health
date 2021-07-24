SeagullApp.controller('UserRoleController', function ($scope, $http, $timeout) {
    $scope.userRole = {};
    UserRoleId != '0' ? $("#userrole-delete").show() : $("#userrole-delete").hide();
    showNotification($scope, $timeout, 'Loading', "succes", true);
    $http({
        method: 'POST',
        url: GetUserRoleUrl("Prepare"),
        data: { Id: UserRoleId, }
    }).then(function successCallback(data, status, headers, config) {
        // this callback will be called asynchronously
        // when the response is available
        $scope.userRole = data.data;
        showNotification($scope, $timeout, 'Loading Succesfully', "succes", true)
    }, function errorCallback(data, status, headers, config) {
        // called asynchronously if an error occurs
        // or server returns response with an error status.
        showNotification($scope, $timeout, 'Unexpected Error while get data!!', "error", true);
    });
    //get called when user submits the form
    $scope.submitForm = function (e) {
        DisableButton(["save-continue", "save"]);
        showNotification($scope, $timeout, 'Please wait -- Posting Data', "succes", true)
        var model = {
            "Id": $scope.userRole.Id,
            "Name": $scope.userRole.Name,
            "SystemName": $scope.userRole.SystemName,
            "FreeShipping": $scope.userRole.FreeShipping,
            "TaxExempt": $scope.userRole.TaxExempt,
            "EnablePasswordLifetime": $scope.userRole.EnablePasswordLifetime,
            "Active": $scope.userRole.Active,
            "IsSystemRole": $scope.userRole.IsSystemRole
        }
        //$http service that send or receive data from the remote server
        $http({
            method: 'POST',
            url: GetUserRoleUrl("get"),
            data: { model: model, continueEditing: e },
        }).then(function successCallback(data, status, headers, config) {
            // this callback will be called asynchronously
            // when the response is available
            $scope.errors = [];
            EnableButton(["save-continue", "save"]);
            if (data.data.success) {
                $scope.userRole = {};
                $scope.userRole = data.data;
                showNotification($scope, $timeout, 'Save Succesfully', "succes", true);
                if (data.data.url != "") {
                    document.location.href = data.data.url;
                }
            }
            else {
                var error = "";
                $.each(data.data.errors, function (index, value) {
                    error += "," + value;
                });
                showNotification($scope, $timeout, error, "error", true);
            }
        }, function errorCallback(data, status, headers, config) {
            // called asynchronously if an error occurs
            // or server returns response with an error status.
            showNotification($scope, $timeout, 'Unexpected Error while saving data!!', "error", true);
            EnableButton(["save-continue", "save"]);
        });
        //$scope.isViewLoading = false;
    }
}).config(['$httpProvider', function ($httpProvider) {
    var antiForgeryToken = document.getElementById('UserRoleForm').childNodes[1].value;
    $httpProvider.defaults.headers.post['__RequestVerificationToken'] = antiForgeryToken;
}]);
