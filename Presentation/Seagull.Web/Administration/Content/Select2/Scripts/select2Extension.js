function buildSelect2For(elementId, url, placeholder) {
    $(elementId).select2(
    {
        dir: 'rtl',
        placeholder: placeholder,
        minimumInputLength: 0,
        multiple: false,
        allowClear: true,
        ajax: {
            quietMillis: 150,
            url: url,
            dataType: 'json',
            data: function (term, page) {
                return {
                    pageSize: 20,
                    pageNum: page,
                    searchTerm: term,
                };
            },
            results: function (data, page) {
                return { results: data.Results };
            }
        }
    });
}

function buildSelect2WithCondition(elementId, url, condition) {
    $(elementId).select2(
    {
        placeholder: '',
        minimumInputLength: 0,
        multiple: false,
        allowClear: true,
        ajax: {
            quietMillis: 150,
            url: url,
            dataType: 'json',
            data: function (term, page) {
                return {
                    searchTerm: term,
                    pageSize: pageSize,
                    pageNum: page,
                };
            },
            results: function (data, page) {
                return { results: data.Results };
            }
        }
    }).on('select2-open', function (e) {
        if (condition) {
            $(this).select2('close');
        }
    });
}

function buildMultiSelect2For(elementId, url) {
    $(elementId).select2(
    {
        placeholder: '',
        minimumInputLength: 0,
        multiple: true,
        allowClear: true,
        ajax: {
            quietMillis: 150,
            url: url,
            dataType: 'json',
            data: function (term, page) {
                return {
                    pageSize: 20,
                    pageNum: page,
                    searchTerm: term,
                };
            },
            results: function (data, page) {
                return { results: data.Results };
            }
        }
    });
}

function buildSelect2WithParameters(elementId, url, data, condition) {
    $(elementId).select2(
    {
        placeholder: '',
        minimumInputLength: 0,
        multiple: false,
        allowClear: true,
        ajax: {
            quietMillis: 150,
            url: url,
            dataType: 'json',
            data: function (term, page) {
                return {
                    searchTerm: term,
                    pageSize: pageSize,
                    pageNum: page,
                };
            },
            results: function (data, page) {
                return { results: data.Results };
            }
        }
    }).on('select2-open', function (e) {
        if (condition) {
            $(this).select2('close');
        }
    });
}

function fillDropDown(fieldId, IdValue, textValue) {
    $(fieldId).select2('data', { id: IdValue, text: textValue })

}

function buildSelect2For(elementId, url, placeholder, SearchId) {
    $(elementId).select2(
    {
        dir: 'rtl',
        placeholder: placeholder,
        minimumInputLength: 0,
        multiple: false,
        allowClear: true,
        ajax: {
            quietMillis: 150,
            url: url,
            dataType: 'json',
            data: function (term, page) {
                return {
                    pageSize: 20,
                    pageNum: page,
                    searchTerm: term,
                    SearchId: $("#" + SearchId).val(),

                };
            },
            results: function (data, page) {
                return { results: data.Results };
            }
        }
    });
}

function buildSelect2For(elementId, url, placeholder, caseId, TableName) {
    $(elementId).select2(
    {
        dir: 'rtl',
        placeholder: placeholder,
        minimumInputLength: 0,
        multiple: false,
        allowClear: true,
        ajax: {
            quietMillis: 150,
            url: url,
            dataType: 'json',
            data: function (term, page) {
                return {
                    pageSize: 20,
                    pageNum: page,
                    searchTerm: term,
                    caseId: caseId,
                    TableName: TableName,

                };
            },
            results: function (data, page) {
                return { results: data.Results };
            }
        }
    });
}

function buildMultiSelect2For(elementId, url, placeholder, caseId, TableName) {
    $(elementId).select2(
    {
        dir: 'rtl',
        placeholder: placeholder,
        minimumInputLength: 0,
        multiple: true,
        allowClear: true,
        ajax: {
            quietMillis: 150,
            url: url,
            dataType: 'json',
            data: function (term, page) {
                return {
                    pageSize: 20,
                    pageNum: page,
                    searchTerm: term,
                    caseId: caseId,
                    userId: $("#DefendantName").select2('data').id,

                    TableName: TableName,

                };
            },
            results: function (data, page) {
                return { results: data.Results };
            }
        }
    });
}

function buildSelect2WithConditionAndChaneEvent(elementId, url, Placeholder, condition, ChangeEvent) {
    $(elementId).select2(
    {
        placeholder: '',
        minimumInputLength: 0,
        multiple: false,
        allowClear: true,
        ajax: {
            quietMillis: 150,
            url: url,
            dataType: 'json',
            data: function (term, page) {
                return {
                    searchTerm: term,
                    pageSize: 20,
                    pageNum: page,
                };
            },
            results: function (data, page) {
                return { results: data.Results };
            }
        }
    }).on('select2-open', function (e) {
        if (condition) {
            $(this).select2('close');
        }
    }).on("change", function (e) {
        // mostly used event, fired to the original element when the value changes
        window[ChangeEvent]();
    });
}

function buildMultiSelect2For(elementId, url, placeholder) {
    $(elementId).select2(
    {
        dir: 'rtl',
        placeholder: placeholder,
        minimumInputLength: 0,
        multiple: true,
        allowClear: true,
        ajax: {
            quietMillis: 150,
            url: url,
            dataType: 'json',
            data: function (term, page) {
                return {
                    pageSize: 20,
                    pageNum: page,
                    searchTerm: term,
                };
            },
            results: function (data, page) {
                //Remove Loading Ajax
                $('#ajaxBusy').hide();
                return { results: data.Results };
            }
        }
    });
}