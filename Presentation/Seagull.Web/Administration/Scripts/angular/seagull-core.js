(function () {
    var _templatesUrl = '/Administration/Scripts/angular/templates/';
    var _apiPostUrl = '/Admin/';
    var _apiGetUrl = '/Admin/';
    var modelToFromData = function (obj, formData, preindex, attachOnly) {
        var index;
        for (var prop in obj) {
            index = preindex ? preindex + "[" + prop + "]" : prop;
            if (obj[prop] instanceof File) {
                formData.append(prop + "[]", obj[prop]);
            }
            else if (angular.isObject(obj[prop]))
                modelToFromData(obj[prop], formData, index);
            else if (!attachOnly) {
                formData.append(index, obj[prop]);
            }
        } 
        return formData;
    };
    var app = angular.module('ngSeagull', ['pascalprecht.translate', 'ngSanitize']);
    app.run(function ($rootScope, $timeout) {
        $rootScope.messages = [];
        $rootScope.notifications = [];
        $rootScope.showbox = {};

        $rootScope.setMessages = function (messages) {
            angular.forEach(messages, function (message) {
                if (message.message_type === 2) {
                    $rootScope.notifications.push(message);
                    $timeout(function () {
                        $rootScope.notifications.splice($rootScope.notifications.indexOf(message), 1);
                    }, 5000);
                } else if (message.message_type === 1) {
                    $rootScope.messages.push(message);
                }

            });
        };


    });

    app.config(['$locationProvider', function ($locationProvider) {
        $locationProvider.html5Mode({ enabled: true, requireBase: false, rewriteLinks: false });
    }]);
    app.config(['$translateProvider', function ($translateProvider) {
        $translateProvider.useSanitizeValueStrategy('escape', 'sanitizeParameters');
        $translateProvider.translations('all', angular.translations);
        $translateProvider.preferredLanguage('all');

    }]);
    app.config(['$httpProvider', function ($httpProvider) {
        $httpProvider.defaults.transformResponse.push(function (responseData) {
            var regexIso8601 = /^(\d\d\d\d)-(\d?\d)-(\d?\d)T(\d\d):(\d\d):(\d\d)$/g;
            function getLocaleDate(input) {
                if (typeof input !== "object") return input;
                for (var key in input) {
                    if (!input.hasOwnProperty(key)) continue;
                    var value = input[key];
                    var match;
                    if (typeof value === "string" && (match = value.match(regexIso8601))) {
                        input[key] = moment(match[0], moment.ISO_8601).format('L');
                    } else if (typeof value === "object") {
                        getLocaleDate(value);
                    }
                }
            }
            getLocaleDate(responseData);
            return responseData;
        });
        $httpProvider.interceptors.push(function ($q, $rootScope) {

            return {
                'request': function (config) {
                    if (!config.url.split('GenericGets').length > 1)
                        $('#ajaxBusy').fadeIn('slow');
                    if (config.data && config.data.token) {
                        config.headers.__RequestVerificationToken = config.data.token;
                        delete config.data.token;
                    } else if (config.data && config.data.__RequestVerificationToken) {
                        config.headers.__RequestVerificationToken = config.data.__RequestVerificationToken;
                    } else if ($rootScope.token) {
                        config.headers.__RequestVerificationToken = $rootScope.token;
                    }
                    return config;
                },
                'response': function (response) {
                    RemoveLoading()
                    if (response.data.success && angular.isArray(response.data.Msg) && response.data.Msg.length > 0) {
                        $rootScope.setMessages([{ message_type: 2, message_header: "Notification", message_body: response.data.Msg[0] }]);
                    }
                    if (response.data.success === false && angular.isArray(response.data.Msg) && response.data.Msg.length > 0) {
                        $rootScope.setMessages([{ message_type: 1, message_header: "Business Validation Messages", message_body: response.data.Msg }]);
                    }

                    if (response.data.url && response.data.url != "") {
                        GoToUrl(response.data.url);
                        return;
                    }
                    return response;
                },
                'responseError': function (rejection) {
                    RemoveLoading()
                    //console.error("request error",rejection);
                    $rootScope.setMessages([{ message_type: 1, message_header: "Request Error: " + rejection.status + " " + rejection.statusText, message_body: [rejection.config.url] }]);
                    return $q.resolve({ data: { success: false }, rejection: rejection });
                }
            };
        });
    }]);

    app.controller('null', function () {/*dummy ctrl*/ });

    var sgFormDirective = function ($http, $timeout, $parse) {

        var link = function (scope, elem, attrs, ctrl, transclude) {
            elem.addClass('sg-form');
            //elem.ready(function () {
            //    scope.$apply(function () {
            //        var func = $parse(attrs.sgReady);
            //        func(scope);
            //    })
            //})
            //scope.RemoveLoading = function () {
            //    RemoveLoading();
            //}
            scope.moment = moment;
            transclude(scope, function (clone) {
                elem.append(clone);
            });
            scope.form = ctrl;
            scope.token = attrs.token;
            var IsEdit =
			scope.vm = {};
            scope.vm.ctrl = attrs.ctrl;

            if (attrs.view != "False") {

                scope.vm.getData = scope.getData || function () {
                    scope.vm.editLock = true;
                    if (typeof scope.beforeGet === "function") scope.beforeGet();
                    $http.post(_apiGetUrl + attrs.ctrl + "/CreateOrEditModel", { Id: scope.id, token: attrs.token }).then(function (response) {
                        if (response.data.success == true) {
                            scope.model = response.data.data;
                            if (typeof scope.afterGet === "function") scope.afterGet(response);
                            $timeout(function () {
                                ctrl.$setPristine();
                                ctrl.$setUntouched();
                                scope.vm.editLock = false;
                                scope.FormErrors = {};
                            }, 10);
                        }
                    });

                };


                scope.vm.validateForm = function () {
                    scope.FormErrors = {};
                    ctrl.$setSubmitted();

                    if (ctrl.$invalid) {
                        var elemClass = elem.find(".sg-input-container.ng-invalid [name]:first");
                        var errorsList = [];


                        $(elemClass).closest('sg-tabs').find('ul.nav-tabs:first>li').removeClass("active");
                        $(elemClass).closest('sg-tabs').find('ul.nav-tabs:first>li[tab-toggle=' + $(elemClass).closest('.tab-pane').attr("id") + ']').addClass("active");
                        $(elemClass).closest('sg-tabs').find('.tab-content:first>.tab-pane').css("display", "none");
                        $(elemClass).closest('.tab-pane').css("display", "block");

                        if (elemClass.is(":visible")) {
                            try {
                                elem.find(elemClass)[0].focus();
                                elem.find('.sg-input-container.ng-invalid:visible')[0].scrollIntoView({ behavior: "smooth", viewPadding: { y: 10 } });
                                //$('.sg-input-container.ng-invalid:visible')[0].scrollIntoView({duration: 2500, direction: "vertical",viewPadding: { y: 10 }});
                                //scope.$root.messages.push({ 'message_type': 'error', message_header: 'Form Error', 'message_body': ["لم يتم حفظ البيانات بسبب وجود أخطاء , قم بالتأكد من البيانات المدخلة والحفظ مجددا"] });
                            } catch (e) { }
                        } else {

                            angular.forEach(ctrl.$error, function (errors, errorType) {
                                angular.forEach(errors, function (error) {
                                    if (error.messages) {
                                        errorsList.push(error.messages[errorType]);
                                    }
                                });
                            });
                            if (errorsList.length == 0) return true;
                            scope.$root.messages.push({ 'message_type': 'error', message_header: 'Form Error', 'message_body': errorsList });
                        }
                        return false;
                    }
                    return true;
                };


                scope.vm.submit = function (continueEditing, saveAsDraft, submit) {
                    if (!scope.vm.validateForm())
                        return;
                    scope.vm.editLock = true;
                    //Loading Ajax Busy
                    $('#ajaxBusy').fadeIn('slow');
                    var postData = angular.copy(scope.model);
                    if (typeof scope.afterGet === "function") postData = scope.beforeSubmit(scope.model);
                    json = { Model: postData, continueEditing: continueEditing, token: attrs.token };
                    if (saveAsDraft) json['saveAsDraft'] = true;
                    if (submit) json['submit'] = true;
                    $http.post(_apiPostUrl + attrs.ctrl + "/CreateOrEdit", json).then(function (response) {
                        debugger
                        var y = response.rejection.data;
                        debugger
                    	if (response.data.success) {
                            if (scope.model._upload) {
                                var formData = new FormData();
                                formData = modelToFromData(scope.model._upload, formData, null, true);
                                formData.append("Id", response.data.data.Id);
                                formData.append("__RequestVerificationToken", $('input[name=__RequestVerificationToken]:first').val());
                                $http.post(_apiPostUrl + attrs.ctrl + "/uploadFiles", formData, {
                                    transformRequest: angular.identity,
                                    headers: { 'Content-Type': undefined, 'Process-Data': false }
                                }).then(function () {
                                    scope._upload = [{}];
                                });
                            }

                            scope.model = response.data.data;
                            //Remove Loading Ajax Busy
                            RemoveLoading();
                            $timeout(function () {
                                ctrl.$setPristine();
                            }, 10);

                        } else if (response.data.success === false && response.data.FormErrors) {
                            ctrl.$setUntouched();
                            ctrl.$setPristine();
                            scope.FormErrors = response.data.FormErrors;
                        }
                        scope.vm.editLock = false;
                    });
                };
                scope.$watch('id', function (val) {
                    scope.vm.getData();
                })
            }

            scope.sum = function (items, prop1, prop2) {
                if (items)
                    return items.reduce(function (a, b) {
                        if (b[prop1]) a = a + b[prop1];
                        if (b[prop2]) a = a + b[prop2];
                        return a;
                    }, 0);
            };
        };
        return {
            restrict: 'EA',
            name: 'formController',
            require: '^form',
            template: '',
            scope: { model: '=?formModel', id: '=' },
            transclude: true,
            link: link,
            controller: '@'
        };
    };
    var sgTableDirective = function ($filter) {
        var templateUrl = _templatesUrl + 'sg-table.html';
        var ctrlName;
        var controller = function ($scope, $http, $element) {
            var vm = this;
            $scope.currentRow = {};
            $scope.col_search = {};
            $scope.col_search_operator = {};
            $scope.cso_color = {};
            $scope.tableParams = {};
            vm.binding = [];

            vm.transaction = { meta: [], data: [] };
            vm.cols = [];
            vm.sg_cols = [];
            vm.fields = [];
            vm.showEditForm = false;
            vm.defualtValues = { Id: 0 };
            vm.limit_list = [{ 'title': 10, 'val': 10 }, { 'title': 25, 'val': 25 }, { 'title': 50, 'val': 50 }, { title: 100, val: 100 }, { 'title': 'all', val: 10000000 }];
            vm.filter_operators = [
				{ 'title': 'mdi-code-not-equal', 'color': 'text-danger', 'val': 'nq', filter: 'text,number', tooltip: $filter('translate')('seagull.operators.NQ') },
				{ 'title': 'mdi-code-equal', 'color': 'text-success', 'val': 'eq', filter: 'text,number', tooltip: $filter('translate')('seagull.operators.EQ') },
				{ 'title': 'mdi-code-array', 'color': 'text-primary', 'val': 'in', filter: 'text', tooltip: $filter('translate')('seagull.operators.IN') }

            ];
            vm.Date_filter_operators = [
				{ 'title': 'mdi-code-greater-than-or-equal', 'color': 'text-danger', 'val': 'gr', filter: 'date', tooltip: $filter('translate')('seagull.operators.greater') },
                { 'title': 'mdi-code-equal', 'color': 'text-success', 'val': 'eq', filter: 'date', tooltip: $filter('translate')('seagull.operators.EQ') },
				{ 'title': 'mdi-code-less-than-or-equal', 'color': 'text-primary', 'val': 'ls', filter: 'date', tooltip: $filter('translate')('seagull.operators.less') }

            ];
            $scope.page_no = 1;
            vm.limit = $scope.limit;
            $scope.rowClass = function (row) {

                // return "red";
            }

            $scope.rowReOpen = function (row) {
                if (row.Reopen)
                    return true
                else
                    return false;

            }

            vm.reopen_msg = {
                header: "Reopen confirm",
                bodyMsg: "Are you sure you want to reopen the report?"
            };

            $scope.rowClosed = function (row) {
                if (row.Closed)
                    return true
                else
                    return false;

            }

            vm.closed_msg = {
                header: "Close confirm",
                bodyMsg: "Are you sure you want to close the report?"
            };

            $scope.$watch(function () {
                return [$scope.where, $scope.col_search, vm.limit, $scope.initialData];
            }, function (newVal, oldVal) {
                if (newVal != oldVal) {
                    $scope.page_no = 1;
                    vm.undo();
                }
            }, true);

            vm.pick_filter = function (col_name, operator) {
                $scope.col_search_operator[col_name] = operator.val;
                $scope.cso_color[col_name] = operator.color;
                if ($scope.col_search[col_name] != null && $scope.col_search[col_name] != "") {
                    $scope.page_no = 1;
                    vm.undo();
                }
            };

            vm.selectRow = function (row) {
                $scope.currentRow = row;
            };

            vm.editRow = function (row) {
                if ($scope.createAction) {
                    var url = $scope.createAction.replace("seagull=qHH63IV%2AyBM%3D", row.EncId);
                    window.location = url;
                    return;
                }
            };

            vm.addRow = function () {
                if ($scope.createAction) {
                    window.location = $scope.createAction;
                    return;
                }
            };

            vm.deleteRow = function (row) {
                //Loading Ajax Busy
                $('#ajaxBusy').fadeIn('slow');
                $http.post(_apiPostUrl + ctrlName + '/Delete', { Id: row.Id, token: $scope.token }).then(function (response) {
                    if (response.data.success === true) {
                        row.$operation = 'delete';
                    } else {
                        //Remove Loading Ajax Busy
                        RemoveLoading();
                    }
                });
            };

            vm.reopenRow = function (row) {
                //Loading Ajax Busy
                $('#ajaxBusy').fadeIn('slow');
                $http.post(_apiPostUrl + ctrlName + '/Reopen', { Id: row.Id, token: $scope.token }).then(function (response) {
                    if (response.data.success === true) {
                        row.$operation = 'reopen';
                    } else {
                        //Remove Loading Ajax Busy
                        RemoveLoading();
                    }
                }); 
            };

            vm.closedRow = function (row) {
                //Loading Ajax Busy
                $('#ajaxBusy').fadeIn('slow');
                $http.post(_apiPostUrl + ctrlName + '/Closed', { Id: row.Id, token: $scope.token }).then(function (response) {
                    if (response.data.success === true) {
                        row.$operation = 'closed';
                    } else {
                        //Remove Loading Ajax Busy
                        RemoveLoading();
                    }
                });
            };



            vm.sort = function (col) {
                if (col.name) {
                    vm.reverse = col.name === vm.orderby ? !vm.reverse : false;
                    vm.orderby = col.name;
                    vm.undo();
                }
            };

            vm.paging = function (action) {
                switch (action) {
                    case 'next':
                        if ($scope.page_no < vm.page_count) {
                            $scope.page_no = $scope.page_no + 1;
                        }
                        break;
                    case 'previous':
                        if ($scope.page_no > 1) {
                            $scope.page_no = $scope.page_no - 1;
                        }
                        break;
                    case 'first':
                        $scope.page_no = 1;
                        break;
                    case 'last':
                        $scope.page_no = vm.page_count;
                        break;
                }
                vm.undo();
            }

            vm.undo = function () {
                vm.showEditForm = false;
                vm.isLoading = true;
                var Id = $scope.initialData;
                var URl = _apiGetUrl + $scope.datasrc;
                var paging = {
                    "token": $scope.token,
                    "pagination": {
                        "start": ($scope.page_no - 1) * vm.limit,
                        "Count": vm.limit
                    },
                    "sort": {
                        "predicate": vm.orderby,
                        "reverse": vm.reverse
                    },
                    "search": JSON.stringify($scope.col_search),
                    "search_operator": JSON.stringify($scope.col_search_operator),
                    "where": JSON.stringify($scope.where),
                    "id": Id
                };
                $http.post(URl, paging).then(function (data) {
                    if (typeof $scope.onDataFetch != "undefined") $scope.onDataFetch()(data);
                    vm.binding = data.data;
                    vm.page_count = data.data.page_count;
                    vm.data_count = data.data.data_count;
                    vm.transaction = { data: [] };

                    if ($scope.page_no > vm.page_count && vm.page_count > 0) {
                        $scope.page_no = angular.copy(vm.page_count);
                    }
                    //if ($scope.where) {
                    //    for (var key in $scope.where) {
                    //        vm.defualtValues[key] = $scope.where[key];
                    //    }
                    //}
                    //$scope.currentRow = vm.binding.data[0]; proplem when refresh then add 
                    vm.isLoading = false;
                    setTimeout(function () {
                        $element.find('.sg-table table.table').floatThead({
                            position: 'absolute',
                            top: 0
                        });
                    }, 100);
                });
            };

            $scope.$on('Import', function () {
                vm.undo();
            });
        }, link = function (scope, elem, attrs, ctrl, transclude) {
            if (typeof attrs.hasFilter == "undefined") scope.hasFilter = true;
            if (typeof attrs.ctrl != "undefined") ctrlName = attrs.ctrl;
            if (typeof attrs.hasOperation == "undefined") scope.hasOperation = { 'add': true, 'edit': true, 'delete': false, 'reopen': false, 'closed': false };
            if (typeof attrs.hasImport == "undefined") scope.hasImport = false;
            if (typeof attrs.limit == "undefined") ctrl.limit = 10;
            if (typeof attrs.canView == "undefined") scope.hasImport = false;
            if (typeof attrs.initialData == "undefined") scope.initialData = 0;
            ctrl.undo();
        };
        return {
            restrict: 'E',
            transclude: true,
            scope: {
                datasrc: '@',
                token: '@',
                createAction: '@',
                limit: '=?',
                hasFilter: '=?',
                hasOperation: '=?',
                hasImport: '=?',
                currentRow: '=?',
                initialData: '=?',
                where: '=?',
                onDataFetch: '&?'
            },
            controller: controller,
            controllerAs: 'vm',
            link: link,
            templateUrl: templateUrl
        };
    };
    var sgColDirective = function () {
        var link = function (scope, elem, attrs, sgTableController, transclude) {
            transclude(scope, function (content) {
                if (content[0]) {
                    var html = "";
                    angular.forEach(content, function (node) {
                        if (node.outerHTML) html = html + node.outerHTML;
                        else if (node.textContent) html = html + node.textContent;
                    });
                    attrs.tag = "<span>" + html + "</span>";
                }
            });
            sgTableController.cols.push(attrs);
            sgTableController.orderby = sgTableController.cols[0].name;
            if (attrs.name && attrs.type) {
                sgTableController.pick_filter(attrs.name, attrs.type == 'text' ? sgTableController.filter_operators[2] : attrs.type == 'date' ? sgTableController.Date_filter_operators[2] : sgTableController.filter_operators[1]);
            }
            //console.log("attrs", attrs);
        };
        return {
            restrict: 'E',
            transclude: true,
            require: '^sgTable',
            link: link
        };
    };
    var sgOutputDirective = function ($compile) {
        var link = function (scope, element, attrs, sgTableCtrl) {
            var filter = "";
            if (scope.meta.tag) {
                var el = angular.element(scope.meta.tag);
                $compile(el)(scope.$parent);
                element.replaceWith(el);
                return;
            }

            if (scope.meta.type && scope.meta.type == "checkbox") {
                if (scope.value === true || scope.value == "true" || scope.value == 1)
                    element.html('<div class="text-center"><i class="fa fa-check text-primary"></i></div>');
                else
                    element.html('<div class="text-center"><i class="fa fa-times text-warning"></i></div>');
                return;
            }
            element.html('<span ng-bind="::row[col.name] ' + filter + '"/>');

            scope.output = scope.value;
            $compile(element.contents())(scope.$parent);

            element.replaceWith(element.contents());

        };
        return {
            restrict: 'E',
            //template : '<span ng-bind="::value">',
            require: '^sgTable',
            scope: { value: '=', meta: '=' },
            link: link
        };
    };
    var sgInputDirective = function ($parse, $filter, $translate, $sanitize) {
        link = function (scope, element, attrs, ctrl) {
            element.addClass('sg-input-container');
            scope.required = attrs.ngRequired;

            if (!attrs.label) {
                element.find('.control-label').remove();
                element.find('.form-control-wrapper').css({ width: "100%" });
            }
            //element.find('input').bind('change', function () {
            //    ctrl.$setDirty();
            //    scope.$apply(ctrl);
            //});

            element.find('[name].form-control').bind('blur', function () {
                ctrl.$setTouched();
                scope.$apply(ctrl);
            });
            var reqMessage = attrs.label || attrs.hiddenLabel || null;
            if (reqMessage)
                reqMessage = '"' + reqMessage + '"';

            $translate('seagull.validation.REQUIRED', { param: reqMessage }).then(function (d) {
                ctrl.messages.required = d;
            });
            $translate('seagull.validation.MINLENGTH', { param: attrs.ngMinlength }).then(function (d) {
                ctrl.messages.minlength = d;
            });
            $translate('seagull.validation.MAXLENGTH', { param: attrs.ngMaxlength }).then(function (d) {
                ctrl.messages.maxlength = d;
            });
            ctrl.messages = {
                required: "",
                minlength: "",
                maxlength: "",
                min: $filter('translate')('seagull.validation.MIN') + attrs.ngMin,
                max: $filter('translate')('seagull.validation.MAX') + attrs.ngMin,
                email: $filter('translate')('seagull.validation.EMAIL'),
                date: $filter('translate')('seagull.validation.DATE'),
            };

            function validateEmail(email) {
                var re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
                return re.test(email);
            }

            scope.getMessage = function () {
                for (var i in ctrl.messages) {
                    if (ctrl.$error[i]) return ctrl.messages[i];
                }
            };


            var validator = function (value) {
                if (attrs.type == 'date') ctrl.$setValidity('date', value == "" || value == null || moment(value, "L", true).isValid() && moment(value).format("L") == value);
                if (attrs.ngMin) ctrl.$setValidity('min', value >= attrs.ngMin || value == "" || value == null);
                if (attrs.ngMax) ctrl.$setValidity('max', value <= attrs.ngMax || value == "" || value == null);
                if (attrs.ngMaxlength) ctrl.$setValidity('maxlength', value == "" || value == null || value.toString().length <= attrs.ngMaxlength);
                if (attrs.ngMinlength) ctrl.$setValidity('minlength', value == "" || value == null || value.toString().length >= attrs.ngMinlength);
                if (typeof attrs.ngEmail != "undefined") ctrl.$setValidity('email', value == "" || value == null || validateEmail(value));
                return value;
            };
            //ctrl.$formatters.push(validator);
            //ctrl.$parsers.push(validator);
            scope.$watch('ngModel', function (newVal, oldVal) {
                if (newVal != oldVal) {
                    validator(newVal);
                    //if (attrs.type == "percentage")
                    //    scope.ngModel = newVal.replace(/[\%,]/, '') + "%";
                    ctrl.$setDirty(); //if we set name for both directive and input $dirty can be read from input
                }
            });
        };
        return {
            restrict: 'E',
            require: 'ngModel',
            transclude: true,
            templateUrl: _templatesUrl + 'sg-input.html',
            scope: {
                ngModel: '=', change: '@', name: '@', type: '@', label: '@', disable: '@', keyup: '@', value: '@', checked: '@', class: '@', id: '@'
            },
            link: link

        };
    };
    var sgTypeDirective = function ($compile) {
        return {
            link: function (scope, elem, attrs) {
                var html;
                switch (attrs.sgType) {
                    case 'checkbox':
                        scope.ngModel = (scope.ngModel == "true" || scope.ngModel == "True") ? true : (scope.ngModel == "false" || scope.ngModel == "False") ? false : attrs.value;
                        html = '<input value="' + attrs.value + '" type="checkbox" name="' + attrs.name + '"  ng-model="ngModel" ng-change="' + attrs.change + '" ng-disabled="' + attrs.disable + '" ng-attr-id="' + attrs.id + '" ng-click="' + attrs.checked + '" />';
                        break;
                    case 'radio':
                        html = '<input value="' + attrs.value + '" type="radio" name="' + attrs.name + '" ng-model="ngModel"  ng-disabled="' + attrs.disable + + '" ng-value="' + attrs.value + '" ng-checked="' + attrs.checked + '" />';
                        break;
                    case 'textarea':
                        html = '<div><textarea  value="' + attrs.value + '" style="' + attrs.style + '" name="' + attrs.name + '" class="form-control ' + attrs.class + '" ng-model="ngModel" ng-change="' + attrs.change + '" ng-disabled="' + attrs.disable + '"  tooltipview><textarea/></div>';
                        break;
                    case 'currency':
                    case 'number':
                        scope.ngModel = parseFloat(scope.ngModel);
                        html = '<input type="number" value="' + attrs.value + '" name="' + attrs.name + '" class="form-control" ng-model="ngModel" ng-disabled="' + attrs.disable + '" ng-change="' + attrs.change + '" ng-keyup="' + attrs.keyup + '"  />';
                        break;
                    case 'float':
                        scope.ngModel = parseFloat(scope.ngModel);
                        html = '<input type="text" value="' + attrs.value + '" name="' + attrs.name + '" class="form-control" ng-model="ngModel" ng-disabled="' + attrs.disable + '" pattern="[0-9]+([\.,][0-9]+)*" format>';
                        break;
                    case 'date':
                        html = '<input type="text" ng-value="' + attrs.value + '" name="' + attrs.name + '" k-datepicker class="form-control k-datepicker" ng-change="' + attrs.change + '" ng-keyup="' + attrs.keyup + '" ng-model="ngModel" ng-disabled="' + attrs.disable + '" />';
                        break;
                    case 'percentage':
                        html = '<div><input type="text" value="' + attrs.value + '" name="' + attrs.name + '" class="form-control" ng-model="ngModel" tooltipview/></div>';
                        break;
                    case 'diffraction':
                        scope.ngModel = parseFloat(scope.ngModel);
                        html = '<div><input type="text" placeholder="' + (attrs.placeholder == undefined || attrs.placeholder == "" ? '' : attrs.placeholder) + '" value="' + attrs.value + '" name="' + attrs.name + '" class="form-control" ng-model="ngModel" ng-disabled="' + attrs.disable + '" ng-pattern="/^[0-9]+(\.[0-9]{1,2})?$/" step="' + attrs.step + '" tooltipview/></div>';
                        break;
                        //here
                    case 'Decimal':
                        html = '<input type="text" value="' + attrs.value + '" name="' + attrs.name + '" class="form-control" ng-model="ngModel" ng-disabled="' + attrs.disable + '" pattern="[0-9]+([\.,][0-1]+)*" format>';
                        break;
                    default:
                        html = '<div><input type="' + attrs.sgType + '" value="' + attrs.value + '" name="' + attrs.name + '" class="form-control" ng-model="ngModel" ng-change="' + attrs.change + '" ng-disabled="' + attrs.disable + '"  tooltipview/></div>';
                }
                elem.replaceWith($compile(html)(scope));
            }
        }
    }

    var sgSelectDirective = function ($http, $filter, $translate, $timeout) {
        var link = function (scope, elem, attrs, ctrl) {
            elem.addClass('sg-input-container');
            scope.vm = {};
            scope.isExternal = true;
            scope.attrs = attrs;
            scope.vm.search = {};
            scope.vm.currentRow = {};
            scope.vm.selectedRows = [];
            scope.vm.selectedIds = [];
            scope.vm.binding = [];
            if (!attrs.label) {
                elem.find('.control-label').remove();
                elem.find('.form-control-wrapper').css({ width: "100%" });
            }
            if (typeof attrs.multi !== "undefined") {
                attrs.multi = true;
            }
            if (typeof attrs.returnCol === "undefined") {
                attrs.returnCol = "id";
            }
            if (typeof attrs.returnAs === "undefined") {
                attrs.returnAs = "name";
            }



            scope.vm.selectRow = function (row) {
                scope.isExternal = false;
                scope.vm.closeModal();
                if (attrs.multi && row[attrs.returnCol]) {
                    scope.vm.selectedRows.push(row);
                } else {
                    scope.vm.currentRow = row;
                    ctrl.$setViewValue(row[attrs.returnCol]);
                }
            };
            scope.vm.unSelectRow = function (row) {
                scope.isExternal = false;
                scope.vm.selectedRows.splice(scope.vm.selectedRows.indexOf(row), 1);
            };
            scope.$watch('vm.selectedRows', function (newVal, oldVal) {
                if (attrs.multi && newVal !== oldVal) {
                    scope.vm.selectedIds = "";
                    angular.forEach(newVal, function (row) {
                        scope.vm.selectedIds += "," + row[attrs.returnCol];
                        //scope.vm.selectedIds.push(row[attrs.returnCol]);
                    });
                    scope.isExternal = false;
                    ctrl.$setViewValue(scope.vm.selectedIds);
                }
            }, true);

            elem.find('.search-box').bind('blur', function () {
                $timeout(function () {
                    ctrl.$setTouched();
                }, 100);
            });

            elem.find('.select-box').keydown(function (e) {
                if (e.keyCode == 9 || e.keyCode == 16 || e.keyCode == 27)
                    return;

                if (e.keyCode == 32) { // when space ignore keydown
                    e.preventDefault();
                }

                elem.find('.search-box').val("");
                elem.find('.search-box').keydown()[0].focus();
                openDropList();

            });
            elem.find('.sg-select').bind('click keydown keyup', function (e) {
                if (e.keyCode == 27 || e.keyCode == 9)
                    scope.vm.closeModal();
                e.stopPropagation();
            });
            elem.find('.search-box').bind('keydown', function (e) {
                if (e.keyCode == 13) {
                    scope.vm.selectRow(scope.vm.selectedRow);
                    scope.vm.closeModal();
                    scope.$apply();
                }
                if ((e.keyCode == 40 || e.keyCode == 38) && scope.vm.binding.length > 0) {
                    try {
                        if (!scope.vm.selectedRow[attrs.returnCol]) {
                            scope.vm.selectedRow = scope.vm.binding[0];
                            scope.$apply();
                            return;
                        }
                        var row = elem.find('tr.selected')[0];
                        var container = elem.find('.sg-table')[0];
                        var scrollHeight = container.scrollHeight;
                        var offset = 0;

                        var currIndex = scope.vm.binding.indexOf(scope.vm.selectedRow);
                        var length = scope.vm.binding.length;
                        if (row) {
                            offset = (row.rowIndex + 1) * row.clientHeight;
                        }
                        if (e.keyCode == 40) {
                            if (currIndex + 1 >= length)
                                return;
                            scope.vm.selectedRow = scope.vm.binding[currIndex + 1];
                            scope.$apply();
                            offset += row.clientHeight;
                        }
                        if (e.keyCode == 38) {
                            scope.vm.selectedRow = scope.vm.binding[currIndex - 1 < 0 ? 0 : currIndex - 1];
                            scope.$apply();
                            offset -= row.clientHeight;

                        }

                        if (offset > container.clientHeight + container.scrollTop) {
                            $(container).animate({ scrollTop: offset - container.clientHeight }, 100);
                        }
                        if (offset < container.scrollTop + row.clientHeight) {
                            $(container).animate({ scrollTop: offset - row.clientHeight }, 100);
                        }
                    } catch (e) {
                    }

                }

            });

            var openDropList = function () {
                $('.sg-select').removeClass('open');

                elem.find('.sg-table').scrollTop(0);
                var bodyRect = document.body.getBoundingClientRect(),
                        elemRect = elem.find('.sg-select')[0].getBoundingClientRect(),
                        offset = bodyRect.height - elemRect.bottom - window.scrollY;
                if (offset < 250)
                    elem.find('#droplist').css({ bottom: elemRect.height, top: "", display: "none" }).slideDown(200);
                else {
                    elem.find('#droplist').css({ bottom: "", top: elemRect.height, display: "none" }).slideDown(200);
                    //elem.find('#droplist').css({bottom: "", top: 0, display: "none"}).slideDown(200);
                }
                elem.find('.sg-select').addClass('open');
                elem.find('input:first').focus();
                scope.vm.selectedRow = {};
                scope.vm.search = {};
                scope.vm.undo();
                //scope.$apply();
            };

            scope.vm.showModal = function () {
                if (elem.find('.sg-select').hasClass('open')) {
                    scope.vm.closeModal();
                    return;
                }
                openDropList();
            };
            scope.vm.closeModal = function () {
                elem.find('#droplist').slideUp(200, function () {
                    elem.find('.sg-select').removeClass('open');
                    $(this).css({ display: "block" });
                    //elem.find('.select-box').focus();
                });

            };

            scope.vm.undo = function (ids, callback) {
                var Id = 0;
                scope.vm.binding = [];
                var Url = _apiGetUrl + attrs.datasrc;
                if (attrs.datasrc.includes("=")) {
                    Id = parseInt(attrs.datasrc.split('=')[1]);
                    Url = _apiGetUrl + attrs.datasrc.split('?')[0];
                }

                var search = scope.vm.search;
                if (ids) search = ids;
                var paging = {
                    "token": attrs.token || scope.$parent.token,
                    "pagination": {
                        "start": 0,
                        "Count": 20
                    },
                    "sort": {
                        "predicate": attrs.returnCol
                    },
                    "search": JSON.stringify(search),
                    "filter": JSON.stringify(scope.filter),
                    "search_operator": '{"' + attrs.returnAs + '":"in","' + attrs.returnCol + '":"eq"}',
                    "id": Id
                };
                $http.post(Url, paging).then(function (response) {
                    if (response.data.data) {
                        scope.vm.binding = response.data.data;
                        //if (scope.vm.binding.length < 20) {
                        //	scope.vm.undo = function (search,callback) {
                        //		if (typeof callback === "function") callback(scope.vm.binding);
                        //	}
                        //}
                    }

                    if (typeof callback === "function") callback(scope.vm.binding);
                });
            };
            if (attrs.datasrc.search('ListOfValues') !== -1) {
                scope.vm.undo = function (ids, callback) {
                    var listId = attrs.datasrc.split("/")[1];
                    var listOfValues = [];
                    angular.forEach(angular.ListOfValues[listId], function (value, key) {
                        listOfValues.push({ Id: key, Name: value });
                    });
                    scope.vm.binding = listOfValues;
                    if (typeof callback === "function") callback(scope.vm.binding);
                }
            }

            scope.$watch(function () {
                var status = true;
                angular.forEach(scope.filter, function (val, key) {
                    if (!val)
                        status = false;
                });
                if (!status)
                    return false;
                return scope.filter;
            }, function (newVal, oldVal) {
                if (oldVal !== false && newVal != oldVal) {
                    scope.ngModel = null;
                    scope.vm.currentRow = {};
                }
            }, true);
            scope.$watch('ngModel', function (newVal, oldVal) {
                if (angular.isFunction(scope.sgChange) && newVal !== oldVal) { scope.sgChange(); }
                if (newVal && scope.isExternal) {
                    var obj = {};
                    if (attrs.multi)
                        scope.vm.selectedRows = [];
                    if (angular.isArray(scope.ngModel) && scope.ngModel.length > 0) {
                        obj[attrs.returnCol] = scope.ngModel;

                    } else if (angular.isNumber(scope.ngModel)) {
                        obj[attrs.returnCol] = scope.ngModel;
                    } else {
                        obj[attrs.returnCol] = scope.ngModel;
                    }

                    scope.vm.undo(obj, function (data) {

                        angular.forEach(data, function (row) {
                            if (attrs.multi) {
                                scope.vm.selectedRows.push(row);
                            } else
                                scope.vm.currentRow = row;
                        });
                    });
                } else if (!newVal) {
                    scope.vm.currentRow = {};
                    if (attrs.multi)
                        scope.vm.selectedRows = [];
                } else {
                    scope.isExternal = true;
                }
            }, true);


            ctrl.messages = {};
            var reqMessage = attrs.label || attrs.hiddenLabel || null;
            if (reqMessage)
                reqMessage = '"' + reqMessage + '"';
            $translate('seagull.validation.REQUIRED', { param: reqMessage }).then(function (d) {
                ctrl.messages.required = d;
            });

            scope.getMessage = function () {
                for (var i in ctrl.messages) {
                    if (ctrl.$error[i]) {
                        return ctrl.messages[i];
                    }
                }
            };
            scope.getMessage();
            if (typeof attrs.disabled !== "undefined" && attrs.disabled != "false" && attrs.disabled) {
                scope.disabled = true;
                scope.vm.openDropList = null;
                scope.vm.closeModal = null;
                scope.vm.showModal = null;

                elem.find('.form-control').attr('disabled', true);
            }

        };
        return {
            restrict: 'E',
            require: 'ngModel',
            templateUrl: function (elem, attrs) {
                if (typeof attrs.multi !== "undefined") return _templatesUrl + "sg-select-multi.html"
                return _templatesUrl + "sg-select.html"
            },
            scope: {
                ngModel: '=',
                returnModel: '=?',
                filter: '=?',
                sgChange: '&?'
            },
            link: link
        };
    }
    var sgValidateDirective = function ($parse) {
        var link = function (scope, element, attrs, sgInputCtrl) {
            //if (attrs.condition && attrs.condition.includes('moment')) scope.moment = moment;
            attrs.$observe('message', function (value) { sgInputCtrl.messages[attrs.name] = value; });
            attrs.$observe('condition', function (value) {
                sgInputCtrl.$setValidity(attrs.name, value == "true" ? true : false);
            });

            //scope.$watch(function () { return scope.$eval(attrs.condition); console.log("zz", scope.$interpolate(attrs.condition)) }, function (newVal, oldVal) {
            //    sgInputCtrl.$setValidity(attrs.name, newVal);
            //    if (newVal !== oldVal) {
            //        sgInputCtrl.$setTouched();
            //    }
            //});
        };
        return {
            restrict: 'E',
            require: "^^ngModel",
            link: link
        };
    };
    var sgConfirmDirective = function ($compile) {
        return {
            restrict: 'AE',
            scope: {
                sgConfirm: '@',
                modalHeader: '=',
                modalBody: '='
            },
            link: function (scope, element, attrs) {

                scope.modalHeader = scope.modalHeader == undefined ? attrs.modalHeader : scope.modalHeader;
                scope.modalBody = scope.modalBody == undefined ? attrs.modalBody : scope.modalBody;
                scope.sgConfirm = scope.sgConfirm == undefined ? attrs.sgConfirm : scope.sgConfirm;

                $(element).on('click', function (e) {

                    $(element).after($compile('<sg-modal modal-action="' + scope.sgConfirm + '" modal-header="' + scope.modalHeader + '" modal-body="' + scope.modalBody + '"></sg-modal>')(scope.$parent));
                });
            }
        };
    };

    var sgModalDirective = function () {
        return {
            templateUrl: _templatesUrl + 'sgmodal.html',
            link: function (scope, element, attrs, sgModalCtrl) {
                $('#SgModal').modal('show');
                element.find('[data-dismiss]').bind('click', function () {
                    $('#SgModal').modal('hide');
                    setTimeout(function () {
                        $('sg-modal').remove();
                    }, 300);
                })
                scope.action = function () {
                    scope.$parent.$eval(scope.modalAction);
                }
            },
            scope: {
                modalAction: '@',
                modalHeader: '@',
                modalBody: '@'
            }
        }
    };
    var sgFieldTypeDirective = function () {
        return {
            link: function (scope, element, attrs, modelCtrl) {
                element.bind("keypress", function ($event) {
                    var char = String.fromCharCode($event.keyCode);
                    if (!(char.match(/[\d.,-]/g) || $event.keyCode === 13 || $event.keyCode === 188 || $event.keyCode === 189 || $event.keyCode === 190) && attrs.sgFieldType == 'number') {
                        $event.preventDefault();
                    }
                });

            }
        };
    }
    var sgFormToolbarDirective = function () {
        return {
            template: function (elem, attrs) {
                var perms = {
                    save: true,
                    finalsave: false,
                    savec: false,
                    submit: false, // Submit Button
                    back: true,
                    savedraft: false,
                    undo: false,
                    backtoproject: false
                }
                if (typeof attrs.save != "undefined") {
                    perms.save = attrs.save;
                }
                if (typeof attrs.finalsave != "undefined") {
                    perms.finalsave = attrs.finalsave;
                }
                if (typeof attrs.savec != "undefined") {
                    perms.savec = attrs.savec;
                }
                if (typeof attrs.backtoproject != "undefined") {
                    perms.backtoproject = attrs.backtoproject;
                }
                if (typeof attrs.back != "undefined") {
                    perms.back = attrs.back;
                }
                if (typeof attrs.undo != "undefined") {
                    perms.undo = attrs.undo;
                }
                if (typeof attrs.savedraft != "undefined") {
                    perms.savedraft = attrs.savedraft;
                }
                //Submit Button
                if (typeof attrs.submit != "undefined") {
                    perms.submit = attrs.submit;
                }
                if (typeof attrs.reopenmsg != "undefined") {
                    attrs.reopenmsg = JSON.parse(attrs.reopenmsg);
                } else {
                    attrs.reopenmsg = {
                        header: "header",
                        bodyMsg: "bodyMsg"
                    };
                }
                if (typeof attrs.closedmsg != "undefined") {
                    attrs.closedmsg = JSON.parse(attrs.closedmsg);
                } else {
                    attrs.closedmsg = {
                        header: "header",
                        bodyMsg: "bodyMsg"
                    };
                }
                return '<div class="toolbar"><div class="btn-group">' +
							'<button ng-if ="' + perms.save + '" type="button" class="btn btn-primary btn-sm" ng-click="vm.submit(false,false,false)" ng-disabled="vm.editLock"><i class="fa fa-save"></i> <span class="hidden-md-down" translate="seagull.buttons.SAVE">Save</span></button>' +
                    		'<button ng-if ="' + perms.finalsave + '" type="button" class="btn btn-primary btn-sm" ng-click="vm.submit(false,false,false)" ng-disabled="vm.editLock"><i class="fa fa-save"></i> <span class="hidden-md-down" translate="seagull.buttons.FinalSAVE">Finalsave</span></button>' +

                    //Submit Button
                            '<button ng-if ="' + perms.submit + '" type="button" class="btn btn-primary btn-sm"  sg-confirm="vm.submit(false,false,true)" modal-header="' + attrs.reopenmsg.header + '" modal-body="' + attrs.reopenmsg.bodyMsg + '" ng-disabled="vm.editLock"><i class="fa fa-submit"></i> <span class="hidden-md-down" translate="seagull.buttons.SUBMIT">Submit</span></button>' +
							'<button ng-if ="' + perms.savec + '" type="button" class="btn btn-primary btn-sm" ng-click="vm.submit(true,false,false)" ng-disabled="vm.editLock"><i class="fa fa-save"></i> <span class="hidden-md-down" translate="seagull.buttons.SAVE_CONTINUE">Save & Continue</span></button>' +
                    	    '<button ng-if ="' + perms.backtoproject + '" type="button" class="btn btn-primary btn-sm" ng-click="vm.submit(true,false,false)" ng-disabled="vm.editLock"> <span class="hidden-md-down" translate="seagull.buttons.Project_MainPage">back To Project</span></button>' +
                            '<button ng-if ="' + perms.savedraft + '" type="button" class="btn btn-primary btn-sm" ng-click="vm.submit(false,true,false)" ng-disabled="vm.editLock"><i class="fa fa-save"></i> <span class="hidden-md-down" translate="seagull.buttons.SAVE_DRAFT">Save As Draft</span></button>' +
							'<button ng-if ="' + perms.undo + '"type="button" class="btn btn-warning btn-sm" ng-click="vm.getData()" ng-disabled="vm.editLock"><i class="fa fa-undo"></i> <span class="hidden-md-down" translate="seagull.buttons.UNDO">Undo</span></button>' +
							'<a  ng-if ="' + perms.back + '" href="/Admin/{{vm.ctrl}}/List" class="btn btn-warning btn-sm"><i class="fa fa-arrow-left"></i> <span class="hidden-md-down" translate="seagull.buttons.BACK">Back</span></a>' +
					  '</div></div>'
            }
        }
    }

    var kDatepickerDirective = function () {
        return {
            require: 'ngModel',
            scope: { ngModel: '=' },
            link: function (scope, elem, attrs, ctrl) {
                $(elem).on('change', function () {
                    ctrl.$setViewValue($(this).val());
                    scope.$apply();
                });
                setTimeout(function () { elem.kendoDatePicker(); }, 0)
            }
        }
    };
    var bodyDirective = function () {
        return {
            link: function (scope, elem, attrs) {
                elem.bind('click keydown keyup', function (e) {
                    elem.find('.sg-select').removeClass('open');
                    scope.$root.showbox = {};
                    scope.$apply();
                });
            }
        };
    };
    var tooltipDirective = function ($timeout) {
        return {
            link: function (scope, elem, attrs) {
                var tooltip = $("");
                var status = "";
                elem.bind('mouseenter ', function () {
                    status = "mouseenter"
                    $timeout(function () {
                        if (status == "mouseenter") {
                            tooltip = $('<div class="tooltip-text">' + attrs.tooltip + '</div>');
                            $('body').prepend(tooltip);
                            $timeout(function () {
                                elemRect = elem[0].getBoundingClientRect();
                                tooltipRect = tooltip[0].getBoundingClientRect();
                                tooltip.css({ visibility: 'visible', opacity: 1, top: elemRect.top - tooltipRect.height - 5 + $(window).scrollTop(), left: elemRect.left - tooltipRect.width / 2 + elemRect.width / 2 });
                            }, 0);
                        }
                    }, 300);

                });

                elem.bind('mouseleave ', function () {
                    status = "mouseleave";
                    $('.tooltip-text').fadeOut(100, function () { $(this).remove(); });
                });
            }
        };
    };
    var sgPageDirective = function () {
        return {
            restrict: 'E',
            link: function (scope, elem, attrs, ctrl, transclude) {
                elem.wrapInner('<div class="sg-page-content"></div>');
                if (typeof attrs.title !== "undefined") {
                    elem.prepend('<h1 class="sg-page-title"><i class="fa fa-th-large"></i> <span>' + attrs.title + '</span></h1>');
                }
            }
        }
    }
    var sgTabsDirective = function () {
        var link = function (scope, elem, attrs, ctrl) {
            elem.find('.tab-content:first>.tab-pane#tab-1').css('display', "block");
            elem.find('ul.nav-tabs:first>li:first').addClass("active");
            elem.find('ul.nav-tabs:first>li').bind('click', function () {
                elem.find('ul.nav-tabs:first>li').removeClass('active');
                $(this).addClass("active");
                elem.find('.tab-content:first>.tab-pane').css("display", "none");
                elem.find('.tab-content:first>.tab-pane#' + $(this).attr('tab-toggle')).css({ display: "block" });
            });
        }, controller = function ($element) {
            var vm = this;
            var count = 0;
            $element.addClass("nav-tabs-custom").css("display", "block");
            $element.append('<ul class="nav nav-tabs"></ul>');
            $element.append('<div class="tab-content"/>');
            vm.addTab = function (clone, title, show) {
                show = show == undefined ? 'true' : show;
                if (show == 'true') {
                    count++;
                    var c = $('<div class="tab-pane" id="tab-' + count + '" style="display:none;"></div>');
                    c.append(clone);
                    $element.find("ul.nav-tabs:first").append('<li tab-toggle="tab-' + count + '"><a class="nav-link">' + title + '</a></li>');
                    $element.find(".tab-content:first").append(c);
                }
            };
        };
        return {
            link: link,
            controller: controller,
            controllerAs: "vm"
        };
    };
    var sgTabDirective = function () {
        var link = function (scope, elem, attrs, sgTabsCtrl, transclude) {
            transclude(scope, function (clone) {
                console.log("sgTab", sgTabsCtrl);
                sgTabsCtrl.addTab(clone, attrs.title, attrs.show);
            });
            elem.remove();
        };
        return {
            require: '^sgTabs',
            transclude: true,
            link: link,
            template: ""
        };
    };
    var sgBoxDirective = function () {
        var link = function (scope, elem, attrs) {
            elem.wrapInner('<div class="box-body"/>');

            var header = $('<div class="box-header with-border clearfix">' +
                    '<div class="box-title"><i class="' + attrs.icon + '"></i> <span>' + attrs.label + '</span></div>' +
                    '<div class="box-tools pull-right"><button type="button" class="btn btn-box-tool" data-widget="collapsePanel"><text><i class="fa fa-plus"></i></text></button></div>' +
                    '</div>');
            elem.prepend(header);
            elem.wrapInner('<div class="box box-info"/>');
            elem.addClass('open');
            var body = elem.find('.box-body');
            elem.find("[data-widget='collapsePanel']").bind('click', function () {
                if (elem.hasClass('open')) {
                    body.slideUp();
                    elem.removeClass('open');
                    elem.find("[data-widget='collapsePanel'] .fa-plus").addClass('fa-minus');
                } else {
                    body.slideDown();
                    elem.addClass('open');
                    elem.find("[data-widget='collapsePanel'] .fa-plus").removeClass('fa-minus');
                }
            });

        };
        return {
            link: link
        };
    };


    var fileDirective = function () {
        return {
            scope: {
                file: '='
            },
            link: function (scope, el, attrs) {
                el.bind('change', function (event) {
                    var file = event.target.files[0];
                    scope.file = file ? file : undefined;
                    scope.$apply();
                });
            }
        };
    };


    app.directive('sgTable', sgTableDirective);
    app.directive('sgCol', sgColDirective);
    app.directive('sgOutput', sgOutputDirective);

    app.directive('sgForm', sgFormDirective);
    app.directive('sgFormToolbar', sgFormToolbarDirective);

    app.directive('sgInput', sgInputDirective);
    app.directive('sgValidate', sgValidateDirective);
    app.directive('sgFieldType', sgFieldTypeDirective);
    app.directive('sgType', sgTypeDirective);

    app.directive('sgSelect', sgSelectDirective);
    app.directive('sgMultiSelect', sgSelectDirective);
    app.directive('kDatepicker', kDatepickerDirective);

    app.directive('sgPage', sgPageDirective);
    app.directive('sgTabs', sgTabsDirective);
    app.directive('sgTab', sgTabDirective);
    app.directive('sgBox', sgBoxDirective);
    app.directive('sgModal', sgModalDirective);

    app.directive('body', bodyDirective);
    app.directive('file', fileDirective);
    app.directive('tooltip', tooltipDirective);
    app.directive('sgConfirm', sgConfirmDirective);

    app.directive('format', function ($filter, $browser) {
        'use strict';

        return {
            require: '?ngModel',
            link: function (scope, elem, attrs, ctrl) {
                if (!ctrl) {
                    return;
                }

                //ctrl.$formatters.unshift(function () {
                //    return $filter('number')(parseFloat(ctrl.$modelValue), 0);
                //});
                //ctrl.$parsers.push(function (viewValue) {
                //    return viewValue.replace(/,/g, '');
                //})
                //ctrl.$parsers.unshift(function (viewValue) {
                //    var checkText = "";
                //    var plainNumber = viewValue.match(/\d/g);
                //    plainNumber = plainNumber.join("");
                //    plainNumber = plainNumber.replace(/[\,\.]/g, '');
                //    var b = $filter('number')(plainNumber);
                //    if (b == ""){
                //        b = 0;
                //    }
                //    elem.val(b);
                //    return plainNumber;
                //});

                var listener = function () {
                    var _value = elem.val();
                    var value = _value.match(/\d/g);
                    if (value != null) {
                        value = value.join("");
                        value = value.replace(/[\,\.]/g, '');
                        if (value.length > 16) {
                            value = value.substring(0, 16);
                        }
                    }
                    else
                        value = "0";
                    elem.val($filter('number')(value, false));
                }

                // This runs when we update the text field
                ctrl.$parsers.push(function (viewValue) {
                    var _value = elem.val();
                    var value = _value.match(/\d/g);
                    if (value != null) {
                        value = value.join("");
                        value = value.replace(/[\,\.]/g, '');
                        if (value.length > 16) {
                            value = value.substring(0, 16);
                        }
                    }
                    else
                        value = "0";
                    return value;
                })

                // This runs when the model gets updated on the scope directly and keeps our view in sync
                ctrl.$render = function () {
                    elem.val($filter('number')(ctrl.$viewValue, false))
                }

                elem.bind('change', listener)
                elem.bind('keydown', function (event) {
                    var key = event.keyCode
                    // If the keys include the CTRL, SHIFT, ALT, or META keys, or the arrow keys, do nothing.
                    // This lets us support copy and paste too
                    if (key == 91 || (15 < key && key < 19) || (37 <= key && key <= 40))
                        return
                    $browser.defer(listener) // Have to do this or changes don't get picked up properly
                })

                elem.bind('paste cut', function () {
                    $browser.defer(listener)
                })
            }
        };
    });

    app.directive('tooltipview', function ($filter, $browser, $timeout) {
        'use strict';
        return {
            require: '?ngModel',
            link: function (scope, elem, attrs, ctrl) {
                var divelem = elem.parent().closest('div');
                divelem.bind('mouseenter ', function () {
                    status = "mouseenter"
                    $timeout(function () {
                        if (status == "mouseenter") {
                            var tooltip = $('<div class="tooltip-text">' + elem.val() + '</div>');
                            $('body').prepend(tooltip);
                            $timeout(function () {
                                var elemRect = elem[0].getBoundingClientRect();
                                var tooltipRect = tooltip[0].getBoundingClientRect();
                                tooltip.css({ visibility: 'visible', opacity: 1, zIndex: 1100, top: elemRect.top - tooltipRect.height - 5 + $(window).scrollTop(), left: elemRect.left - tooltipRect.width / 2 + elemRect.width / 2 });
                            }, 300);
                        }
                    }, 300);
                });
                divelem.bind('mouseleave ', function () {
                    status = "mouseleave";
                    $('.tooltip-text').fadeOut(100, function () { $(this).remove(); });
                });
            }
        };
    });
    app.directive('tooltipforlabel', function ($filter, $browser, $timeout) {
        'use strict';
        return {
            require: '?ngModel',
            link: function (scope, elem, attrs, ctrl) {
                var divelem = elem.parent().closest('div');
                divelem.bind('mouseenter ', function () {
                    status = "mouseenter";
                    var tooltip = $('<div class="tooltip-text">' + attrs.tooltip + '</div>');
                    $('body').prepend(tooltip);
                    var elemRect = elem[0].getBoundingClientRect();
                    var tooltipRect = tooltip[0].getBoundingClientRect();
                    tooltip.css({ visibility: 'visible', opacity: 1, zIndex: 1100, top: elemRect.top - tooltipRect.height - 5 + $(window).scrollTop(), left: elemRect.left - tooltipRect.width / 2 + elemRect.width / 2 });

                });
                //divelem.bind('mouseleave ', function () {
                //    status = "mouseleave";
                //    $('.tooltip-text').fadeOut(100, function () { $(this).remove(); });
                //});
            }
        };
    });

    app.directive('tooltipviewdrop', function ($filter, $browser, $timeout) {
        'use strict';
        return {
            require: '?ngModel',
            link: function (scope, elem, attrs, ctrl) {
                var divelem = elem;
                divelem.bind('mouseenter ', function () {
                    status = "mouseenter"
                    $timeout(function () {
                        if (status == "mouseenter") {
                            if (scope.row.SubObjectiveName != undefined) {
                                var tooltip = $('<div class="tooltip-text" id="' + scope.row.Id + '">' + scope.row.SubObjectiveName + '</div>');
                                $('body').prepend(tooltip);
                                $timeout(function () {
                                    var elemRect = elem[0].getBoundingClientRect();
                                    var tooltipRect = tooltip[0].getBoundingClientRect();
                                    tooltip.css({ visibility: 'visible', opacity: 1, zIndex: 1100, top: elemRect.top - tooltipRect.height - 5 + $(window).scrollTop(), left: elemRect.left - tooltipRect.width / 2 + elemRect.width / 2 });
                                }, 300);
                            }
                        }
                    }, 300);
                });
                divelem.bind('mouseleave ', function () {
                    status = "mouseleave";
                    $('.tooltip-text').fadeOut(100, function () { $(this).remove(); });
                });
            }
        };
    });

    app.directive('sgRepeatDirective', function () {
        //Loading Ajax Busy
        $('#ajaxBusy').fadeIn('slow');
        return function (scope, element, attrs) {
            if (scope.$last) {
                // iteration is complete, do whatever post-processing

                //Remove Loading Ajax Busy
                //RemoveLoading();
            }
        };
    })
    app.directive('sgReady', function ($parse) {
        return {
            restrict: 'A',
            link: function ($scope, elem, attrs) {

            }
        }
    })

})();

function isDate(value) {
    if (/^\d{1,2}\/\d{1,2}\/\d{4}$/.test(value))
        return true;
    else
        return false;
}
function RemoveLoading() {
    setTimeout(function () {
        $('#ajaxBusy').fadeOut('slow');
    }, 500);
}
function GoToUrl(url) {
    setTimeout(function () {
        window.location = url;
    }, 500);
}

