﻿var translations = {
    seagull: {
        buttons: {
            BACK: "Back",
            SAVE: "Save",
            FinalSAVE: "Submit",
            SAVE_CONTINUE: "Save & Continue",
            Project_MainPage: "Project Main Page",
            SAVE_DRAFT: "Save As Draft",
            UNDO: "Undo",
            ADD: "Add New",
            EDIT: "Edit",
            DELETE: "Delete",
            REFRESH: "Refresh",
            CANCEL: "Cancel",
            SUBMIT: "Submit",
            NEXT: "Next Page",
            PREVIOUS: "Previous Page",
            FIRST: "First Page",
            LAST: "Last Page"
        },
        validation: {
            REQUIRED: 'The Field {{param}} is Required',
            MINLENGTH: "Must be at least {{param}} characters",
            MAXLENGTH: "Must be at most {{param}} characters",
            MIN: "Value Must be less than ",
            MAX: "Value Must be more than ",
            EMAIL: "Invalid Email format",
            DATE: "Invalid Date format",
        },
        operators: {
            NQ: 'Not Match',
            EQ: 'Exact Match',
            IN: 'Contain'
        },
        messages: {
            DELETE_CONFIRM: "Confirm Delete",
            DELETE_CONFIRM_MSG: "Are you sure that you want to delete this row?",
            NO_DATA: "No data to display.",
            LOADING: "Loading Data..."
        },
        labels: {
            PAGE: 'Page',
            PAGE_ROWS: 'Rows per page: ',
            OF: 'Of',
            TOTAL_ROWS: 'Total Rows:',
            OPERATION: 'Operation'
        }
    }
};
var ListOfValues = {
    Quarters: {
        1: "Q1",
        2: "Q2",
        3: "Q3",
        4: "Q4",
    }
}
angular.extend(angular, { translations: translations, ListOfValues: ListOfValues });