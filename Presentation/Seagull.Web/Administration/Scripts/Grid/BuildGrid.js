﻿//SearchPrint//$.jgrid.defaults.width = //$("#" + idwidth).width();
$.jgrid.defaults.responsive = true;
$.jgrid.defaults.styleUI = 'Bootstrap';
function buildTreeGrid(Model, divname, urlp, rowNump, pagerdivname, searchp, addp, editp, delp, refreshp, multipleSearchp, direction, cansearch, idwidth, ActionType, ReportType, ExportToExcellFunction, ExportToPdfFunction) {
    $("#" + divname).jqGrid({
        width: null,//idwidth == "" ? 450 : $("." + idwidth).width() - 40,
        shrinkToFit: true,
        autowidth: true,
        url: urlp,
        mtype: "Get",
        styleUI: 'Bootstrap',
        datatype: "json",
        colModel: Model,
        loadonce: true,
        //grouping: true,
        //groupingView: {
        //    minusicon: "ui-icon-arrowstop-1-n", // "ui-icon-circlesmall-minus"
        //    plusicon: "ui-icon-arrowstop-1-s", 	// "ui-icon-circlesmall-plus",
        //    groupField: ["SectorName"],
        //    groupCollapse: true
        //},
        viewrecords: true,
        height: null,
        rowNum: rowNump,
        subGrid: true, // set the subGrid property to true to show expand buttons for each row
        subGridRowExpanded: // mostly used event, fired to the original element when the value changes
        showChildGrid, // javascript function that will take care of showing the child grid
        subGridOptions: {
            // expand all rows on load
            minusicon: "glyphicon glyphicon-minus redIcon", // "ui-icon-circlesmall-minus"
            plusicon: "glyphicon glyphicon-plus blueIcon", 	// "ui-icon-circlesmall-plus",
            expandOnLoad: false
        },
        pager: "#" + pagerdivname,
        direction: direction,
        loadComplete: function (data) {
            var trFirstElementForReport = jQuery("#" + 1, jQuery('#' + divname));
            var trLastElementForReport = jQuery("#" + (data.rows.length - 1), jQuery('#' + divname));
            var trTotalElementForReport = jQuery("#" + data.rows.length, jQuery('#' + divname));
            switch (divname) {
                case "jqGridPrePaidAndPostPaidCountForOperatorsReport":
                    ApplyTotalColor(trTotalElementForReport);
                    //for (var i = 1; i <= data.rows.length; i = i + 1) {
                    //    var trElement = jQuery("#" + i, jQuery('#' + divname));
                    //    trElement[0].children[9].className = 'rowColorBlue';
                    //}
                    break;
                case "jqGridSectorFundAndProgressPerYearReport":
                    ApplyPreYearColor(trFirstElementForReport)
                    ApplyPostYearColor(trLastElementForReport);
                    ApplyTotalColor(trTotalElementForReport);
                    for (var i = 1; i <= data.rows.length; i = i + 1) {
                        var trElement = jQuery("#" + i, jQuery('#' + divname));
                        trElement[0].children[12].className = 'rowColorBlue';
                    }
                    break;
                case "jqGridFixedLineSubscribersCount":
                   // ApplyPreYearColor(trFirstElementForReport)
                    ApplyPostYearColor(trLastElementForReport);
                    ApplyTotalColor(trTotalElementForReport);
                    //for (var i = 1; i <= data.rows.length; i = i + 1) {
                    //    var trElement = jQuery("#" + i, jQuery('#' + divname));
                    //    trElement[0].children[1].className = 'rowColorBlue';
                    //}
                    break;
                case "jqGridInternetSubscribersCount":
                    // ApplyPreYearColor(trFirstElementForReport)
                    // ApplyPostYearColor(trLastElementForReport);
                    ApplyPostYearColor(trLastElementForReport);
                    ApplyTotalColor(trTotalElementForReport);
                    //for (var i = 1; i <= data.rows.length; i = i + 1) {
                    //    var trElement = jQuery("#" + i, jQuery('#' + divname));
                    //    trElement[0].children[1].className = 'rowColorBlue';
                    //}
                    break;
                default:
                    break;
            }
        },
        //shrinkToFit: false
    });
    //$('#' + divname).jqGrid('filterToolbar');
    if (cansearch) {
        $('#' + divname).jqGrid('filterToolbar', {
            // JSON stringify all data from search, including search toolbar operators
            stringResult: true,
            // instuct the grid toolbar to show the search options
            searchOperators: true,
            defaultSearch: 'cn'
        });
    }
    $('#' + divname).navGrid("#" + pagerdivname, {
        search: searchp, // show search button on the toolbar
        add: addp,
        edit: editp,
        del: delp,
        refresh: refreshp
    },
         {}, // edit options
         {}, // add options
         {}, // delete options
         { multipleSearch: multipleSearchp } // search options - define multiple search
         ).navButtonAdd('#' + pagerdivname, {
             caption: "Excel",
             title: "Excel",
             buttonicon: "glyphicon-file",
             onClickButton: function () {
                 if (ExportToExcellFunction != undefined && ExportToExcellFunction != "") {
                     var _tempFunction = ExportToExcellFunction.split('**');
                     // mostly used event, fired to the original element when you calling export Function
                     var input = "#" + _tempFunction[0];
                     var _function = "angular.element($('" + input + "')).scope()." + _tempFunction[1];
                     var fire = new Function("return (" + _function + ")")();
                 }
             },
             position: "first"
         }).navButtonAdd('#' + pagerdivname, {
             caption: "PDF",
             title: "PDF",
             buttonicon: "glyphicon-list-alt",
             onClickButton: function () {
                 if (ExportToPdfFunction != undefined && ExportToPdfFunction != "") {
                     var _tempFunction = ExportToPdfFunction.split('**');
                     // mostly used event, fired to the original element when you calling export Function
                     var input = "#" + _tempFunction[0];
                     var _function = "angular.element($('" + input + "')).scope()." + _tempFunction[1];
                     var fire = new Function("return (" + _function + ")")();
                 }
             },
             position: "first"
         });

}

function buildGrid(Model, divname, urlp, rowNump, pagerdivname, searchp, addp, editp, delp, refreshp, multipleSearchp, direction, cansearch, idwidth, ActionType, ReportType, dynamicTableId, ExportToExcellFunction, ExportToPdfFunction) {
    $("#" + divname).jqGrid({
        width: null,//idwidth == "" ? 450 : $("." + idwidth).width() - 40,
        shrinkToFit: true,
        autowidth: true,
        url: urlp,
        mtype: "Get",
        styleUI: 'Bootstrap',
        datatype: "json",
        colModel: Model,
        viewrecords: true,
        height: null,
        rowNum: rowNump,
        pager: "#" + pagerdivname,
        direction: direction,
        loadComplete: function (data) {
            //if (jQuery("#" + divname).getGridParam("records") == 0) {
            //    jQuery("#" + divname).addRowData("blankRow", { "firstCol": "No data was found", "secondCol": "", "thirdCol": "" });
            //    return;
            //}
            if (dynamicTableId != undefined && dynamicTableId != "") {
                var trFirstElementForReportDynamic = jQuery("#" + 1, jQuery('#' + dynamicTableId));
                var trLastElementForReportDynamic = jQuery("#" + (data.rows.length), jQuery('#' + dynamicTableId));
                //ApplyPreYearColor(trFirstElementForReportDynamic);
                //ApplyPostYearColor(trLastElementForReportDynamic);
                //for (var i = 1; i <= data.rows.length; i = i + 1) {
                //var trElement = jQuery("#" + i, jQuery('#' + dynamicTableId));
                //trElement[0].children[6].className = 'rowColorBlue';
                //}
                return;
            }
            var trFirstElementForReport = jQuery("#" + 1, jQuery('#' + divname));
            var trLastElementForReport = jQuery("#" + (data.rows.length - 1), jQuery('#' + divname));
            var trTotalElementForReport = jQuery("#" + data.rows.length, jQuery('#' + divname));
            switch (divname) {
                case "jqGridTest":
                    //Functions
                    //ApplyPreYearColor(trFirstElementForReport)
                    //ApplyPostYearColor(trLastElementForReport);
                    //ApplyTotalColor(trTotalElementForReport);
                    ColorCellDynamic("jqGridTest", data)
                    break;

                case "jqGridAverageIndicatorsForMailsReport":
                    if (data.rows.length > 0) {
                        trFirstElementForReport.prevObject[0].children[0].children[1].children[0].style.backgroundColor = "turquoise";
                        trFirstElementForReport.prevObject[0].children[0].children[1].children[0].style.color = "black";
                        trFirstElementForReport.prevObject[0].children[0].children[1].children[1].style.backgroundColor = "turquoise";
                        trFirstElementForReport.prevObject[0].children[0].children[1].children[1].style.color = "black";
                        trFirstElementForReport.prevObject[0].children[0].children[1].children[2].style.backgroundColor = "turquoise";
                        trFirstElementForReport.prevObject[0].children[0].children[1].children[2].style.color = "black";
                        trFirstElementForReport.prevObject[0].children[0].children[1].children[3].style.backgroundColor = "turquoise";
                        trFirstElementForReport.prevObject[0].children[0].children[1].children[3].style.color = "black";
                        trFirstElementForReport.prevObject[0].children[0].children[2].children[0].style.backgroundColor = "turquoise";
                        trFirstElementForReport.prevObject[0].children[0].children[2].children[0].style.color = "black";
                        trFirstElementForReport.prevObject[0].children[0].children[2].children[1].style.backgroundColor = "turquoise";
                        trFirstElementForReport.prevObject[0].children[0].children[2].children[1].style.color = "black";
                        trFirstElementForReport.prevObject[0].children[0].children[2].children[2].style.backgroundColor = "turquoise";
                        trFirstElementForReport.prevObject[0].children[0].children[2].children[2].style.color = "black";
                        trFirstElementForReport.prevObject[0].children[0].children[2].children[3].style.backgroundColor = "turquoise";
                        trFirstElementForReport.prevObject[0].children[0].children[2].children[3].style.color = "black";
                    }
                    break;
                
                default:
                    break;
            }
            //Color Your Cell
            //for (var i = 0; i < data.rows.length; i = i + 1) {
            //    var trElement = jQuery("#" + data.rows[i].Id, jQuery('#' + divname));
            //    switch (data.rows[i].Test) {
            //        case 0:
            //            trElement[0].children[4].className = 'rowColorRed';
            //            break;
            //        case 1:
            //            trElement[0].children[4].className = 'rowColorGreen';
            //            break;
            //    }
            //    switch (data.rows[i].ProgressStatus) {
            //        case 0:
            //            trElement[0].children[7].className = 'rowColorRed';
            //            break;
            //        case 1:
            //            trElement[0].children[7].className = 'rowColorGreen';
            //            break;
            //    }
            //}
        },
        //shrinkToFit: false
    });
    //$('#' + divname).jqGrid('filterToolbar');
    if (cansearch) {
        $('#' + divname).jqGrid('filterToolbar', {
            // JSON stringify all data from search, including search toolbar operators
            stringResult: true,
            // instuct the grid toolbar to show the search options
            searchOperators: true,
            defaultSearch: 'cn'
        });
    }
    $('#' + divname).navGrid("#" + pagerdivname, {
        search: searchp, // show search button on the toolbar
        add: addp,
        edit: editp,
        del: delp,
        refresh: refreshp
    },
         {}, // edit options
         {}, // add options
         {}, // delete options
         { multipleSearch: multipleSearchp } // search options - define multiple search
         ).navButtonAdd('#' + pagerdivname, {
             caption: "Excel",
             title: "Excel",
             buttonicon: "glyphicon-file",
             onClickButton: function () {
                 if (ExportToExcellFunction != undefined && ExportToExcellFunction != "") {
                     var _tempFunction = ExportToExcellFunction.split('**');
                     // mostly used event, fired to the original element when you calling export Function
                     var input = "#" + _tempFunction[0];
                     var _function = "angular.element($('" + input + "')).scope()." + _tempFunction[1];
                     var fire = new Function("return (" + _function + ")")();
                 }
             },
             position: "first"
         }).navButtonAdd('#' + pagerdivname, {
             caption: "PDF",
             title: "PDF",
             buttonicon: "glyphicon-list-alt",
             onClickButton: function () {
                 if (ExportToPdfFunction != undefined && ExportToPdfFunction != "") {
                     var _tempFunction = ExportToPdfFunction.split('**');
                     // mostly used event, fired to the original element when you calling export Function
                     var input = "#" + _tempFunction[0];
                     var _function = "angular.element($('" + input + "')).scope()." + _tempFunction[1];
                     var fire = new Function("return (" + _function + ")")();
                 }
             },
             position: "first"
         });
    
}

function buildDashboardGrid(Model, divname, urlp, rowNump, pagerdivname, searchp, addp, editp, delp, refreshp, multipleSearchp, direction, cansearch, idwidth, urlPrint, chartname) {
    $("#" + divname).jqGrid({
        width: idwidth == "" ? 450 : $("#" + idwidth).width() - 40,
        shrinkToFit: true,
        url: urlp,
        mtype: "GET",
        styleUI: 'Bootstrap',
        datatype: "json",
        colModel: Model,
        viewrecords: true,
        height: null,
        rowNum: rowNump,
        pager: "#" + pagerdivname,
        direction: direction,
        loadComplete: function (data) {
            $(this).find(">tbody>tr.jqgrow:odd").addClass("myAltRowClassEven");
        },
        //shrinkToFit: false
    });
    //$('#' + divname).jqGrid('filterToolbar');
    if (cansearch) {
        $('#' + divname).jqGrid('filterToolbar', {
            // JSON stringify all data from search, including search toolbar operators
            stringResult: true,
            // instuct the grid toolbar to show the search options
            searchOperators: true,
            defaultSearch: 'cn'
        });
    }
    $('#' + divname).navGrid("#" + pagerdivname, {
        search: searchp, // show search button on the toolbar
        add: addp,
        edit: editp,
        del: delp,
        refresh: refreshp
    },
         {}, // edit options
         {}, // add options
         {}, // delete options
         { multipleSearch: multipleSearchp } // search options - define multiple search
         ).navButtonAdd('#' + pagerdivname, {
             caption: "Excel",
             title: "Excel",
             buttonicon: "glyphicon-file",
             onClickButton: function () {
                 //alert("Adding Row");
                 GetDashBoardReport(urlPrint, 1, chartname)
             },
             position: "first"
         }).navButtonAdd('#' + pagerdivname, {
             caption: "PDF",
             title: "PDF",
             buttonicon: "glyphicon-list-alt",
             onClickButton: function () {
                 //alert("Adding Row");
                 GetDashBoardReport(urlPrint, 2, chartname)
             },
             position: "first"
         });

}

function buildLink(id, url) {
    var row = id.split("=");
    var row_ID = row[1];
    url = url + "/" + row_ID;
    window.open(url);
}

function ApplyPreYearColor(trFirstElementForReport) {
    if (trFirstElementForReport.length > 0 && trFirstElementForReport[0].textContent.split(preplan).length > 1) {
        trFirstElementForReport.removeClass('ui-widget-content');
        trFirstElementForReport.addClass('rowColorOrange');
    }
    return;
}

function ApplyPostYearColor(trLastElementForReport) {
    if (trLastElementForReport.length > 0 ) {
        trLastElementForReport.removeClass('ui-widget-content');
        trLastElementForReport.addClass('rowColorOrange');
    }
    return;
}

function ApplyTotalColor(trTotalElementForReport) {
    trTotalElementForReport.removeClass('ui-widget-content');
    trTotalElementForReport.addClass('rowColorGreen');
    return;
}

function ColorCellDynamic(tablename, data) {
    for (var i = 0; i < data.rows.length; i = i + 1) {
        var id = i + 1;
        var trElement = jQuery("#" + id, jQuery('#' + tablename));
        switch (tablename) {
            case "jqGridPopUpTestReport":
                trElement[0].children[7].className = 'rowColorBlue';
                if (data.rows[i].IsDone == notDone)
                    trElement[0].children[4].className = 'cellColorRed';
                else
                    trElement[0].children[4].className = 'cellColorGreen';
                break;
        }
    }
}

function BuildReportDiv(divname, jqGridReport, jqGridReportPager) {
    $("#" + divname).html("");
    $("#" + divname).html("<div class='col-xs-12'><table id='" + jqGridReport + "'></table><div id='" + jqGridReportPager + "'></div></div>");
}

function buildSearchCriteriaUrlDynamicForReport(url, canPrint, canPrintPdf, $scope, tableName, jqPostData) {
    var countSearchGridUrl = 0;
    var countBuildingUrl = 0;
    if (canPrint || canPrintPdf) {
        angular.forEach(jqPostData, function (element, name) {
            url += (countSearchGridUrl == 0 ? "?grid=" : "&") + name + "=" + jqPostData[name];
            countSearchGridUrl += 1;
        });
    }
   
    // Array which will store the form data
    var formValues = [];
    // Get form elements
    angular.forEach($scope.form, function (element, name) {
        if (!name.startsWith('$')) {
            var value = ($scope.form[name] == undefined || $scope.form[name].$viewValue == undefined ? "" : $scope.form[name].$viewValue);
            if (canPrintPdf || canPrint)
                url += "&" + name + "=" + value;
            else
                url += (countBuildingUrl == 0 ? "?" : "&") + name + "=" + value;
            countBuildingUrl += 1;
        }

    });

    if (url.indexOf("?") >= 0)
        url += "&CanPrint=" + canPrint;
    else
        url += "?CanPrint=" + canPrint;
    url += "&canPrintPdf=" + canPrintPdf;
    
    return url;
}
