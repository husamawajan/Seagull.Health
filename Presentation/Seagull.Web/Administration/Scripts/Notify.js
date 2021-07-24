
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
function NotifyForDelayReport() {
    $.ajax({
        url: GetNotifyUrl,
        type: "POST",
        cache: false,
        data: { __RequestVerificationToken: requestVerificationTokenAdminLayout },
        contentType: "application/json",
        success: function (data) {
            if (data.length > 0) {
                if (data.length > 0) {
                    $("#countBadge").text(data[0].CountNotification);
                    $("#notifyHeader").text($("#notifyHeader").text() + data[0]._notifyForDelayReportData.length);
                    $.each(data[0]._notifyForDelayReportData, function (index, value) {
                        //alert(index + ": " + value);
                        var notifyText = "<li><a href='#'><div class='clearfix' ><span class='pull-left'>" +
                                          "<i class='btn btn-xs no-hover btn-pink fa fa-comment'></i> " + value.EntityName + " " + value.Msg  + " " + value.Year + " : " + value.MonthName + " " +

                                          "</span><span class='pull-right badge badge-info'>....</span>" +
                                          "</div></a></li>";
                        $("#notifyList").append(notifyText);
                    });
                }
            }
        }
    });
}

//jQuery(document).ready(function () {
//    // Start document 
//    removeAnimation();
//    // Declare a proxy to reference the hub.
//    var notifications = $.connection.signalRMethods;

//    // Create a function that the hub can call to broadcast messages.
//    notifications.client.updateNotification = function () {
//        pushData()
//        GetPriorityNotify();
//    };
//    // Start the connection.
//    $.connection.hub.start().done(function () {
//        //alert("connection started")
//        pushData();
//        GetPriorityNotify();
//    }).fail(function (e) {
//        alert(e);
//    });

//    function removeAnimation() {
//        // Remove Animation
//        setTimeout(function () {
//        jQuery('.bell_number').removeClass('animating')
//        jQuery('.CustomBell').removeClass('shake')
//        }, 1000);
//    }
//    function pushData() {
//        $.ajax({
//            url: GetNotify(),
//            type: "POST",
//            contentType: "application/json",
//            success: function (data) {
//                $("div.append-con").empty();
//                jQuery('.bell_number').html(data.Count)
//                jQuery('.number').html(data.Count);
//                jQuery.each(data.data, function (i, val) {
//                    contentText = jQuery(".notification-struct").clone(true);
//                    contentText.removeClass('notification-struct');
//                    contentText.find(".face-noti").html("<i onclick ='GoToNotify(" + val.Id + " , " + val.NotificationId + ");' class='fa fa-pencil-square-o fa-lg CustomNotifyIco' aria-hidden='true'></i>");
//                    contentText.find(".name").html(val.Status);
//                    contentText.find(".notify-con").append(val.Serial);
//                    contentText.prependTo('.append-con');
//                });
//                if (data.Count > 0) {
//                    jQuery('.CustomBell').addClass('shake');
//                    jQuery('.bell_number').addClass('animating').html(parseInt(jQuery('.number').html()));
//                }
//                var length = jQuery('.row-noti').length;
//                if (length == 6) {
//                    jQuery('.append-con').css('max-height', '360px');
//                }
//            }
//        });        
//    }
    
   
//    jQuery('.notification').on('click', function () { //on click notification open notify pop
//        jQuery('.row-noti').animate({
//            left: '0px',
//            opacity: 1
//        }, 500);
//        jQuery('.notification-wrapper').fadeToggle(100).animate({
//            top: '47px',
//            opacity: 1
//        }, 500);
//        return false;
//    });
//    jQuery('.notification-wrapper').on('click', function () {
//        return false;
//    });
//    jQuery('body').not('.notification').click(function () { //on click notification close notify pop
//        jQuery('.row-noti').animate({
//            left: '200px',
//            opacity: 0
//        }, 500);
//        jQuery('.notification-wrapper').animate({
//            top: '34px',
//            opacity: 0
//        }, 500).fadeOut(100);

//        //jQuery('.number').html(0);
//    });
//    jQuery('.email').on('click', function () {
//        jQuery('.notification-wrapper').animate({
//            top: '34px',
//            opacity: 0
//        }, 500).fadeOut(100);
//    });
//});
//function GoToNotify(i, o) {
//    var url = GoToUrl();
//    url += "?id=" + i + "&NotifyId=" + o + "";
//    window.location = url
//}