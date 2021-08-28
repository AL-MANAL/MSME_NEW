/* ------------------------------------------------------------------------------
 *
 *  # CKEditor editor
 *
 *  Demo JS code for editor_ckeditor.html page
 *
 * ---------------------------------------------------------------------------- */


// Setup module
// ------------------------------

var CKEditor = function() {


    //
    // Setup module components
    //

    // CKEditor
    var _componentCKEditor = function() {
        if (typeof CKEDITOR == 'undefined') {
            console.warn('Warning - ckeditor.js is not loaded.');
            return;
        }
        

        // Full featured editor
        // ------------------------------

        // Setup
        CKEDITOR.replace('editor-full', {
            height: 400,
            extraPlugins: 'forms'
        });


     
    };

    // Select2
    var _componentSelect2 = function() {
        if (!$().select2) {
            console.warn('Warning - select2.min.js is not loaded.');
            return;
        };

        // Default initialization
        $('.form-control-select2').select2({
            minimumResultsForSearch: Infinity
        });
    };


    //
    // Return objects assigned to module
    //

    return {
        init: function() {
            _componentCKEditor();
            _componentSelect2();
        }
    }
}();


// Initialize module
// ------------------------------

document.addEventListener('DOMContentLoaded', function() {
    CKEditor.init();
});
