function showNotification(scope, _timeout, message, result, isView) {
    scope.isView = !isView;
    scope.message = message;
    scope.classnotify = result;
    scope.isView = isView;
    _timeout(function () {
        hideNotification(scope);
    }, 5000);
}
function hideNotification(scope) {
    scope.isView = false;
}
function DisableButton(_buttons) {
    $.each(_buttons, function (index, value) {
        $("#" + value).attr("disabled", true);
    });
}
function EnableButton(_buttons) {
    $.each(_buttons, function (index, value) {
        $("#" + value).attr("disabled", false);
    });


   
}
function DynamicSurveyReport() {
	$.ajax({
		url: GetDynamicSurveyyUrl,
		type: "POST",
		cache: false,
		data: { __RequestVerificationToken: requestVerificationTokenAdminLayout },
		contentType: "application/json",
		success: function (data) {
		}
	});
}