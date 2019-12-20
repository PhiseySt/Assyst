// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

getFieldByKey = function (key, index) {
    if (key == null) return null;
    var fields = key.split("|");
    return fields[index];
};

ckEditorToTextArea = function (elementId) {
    var elem = document.getElementById('richRemarks.content');
    if (elem !== undefined && elem !== null) {
        var content = CKEDITOR.instances['richRemarks.content'].getData();
        elem.innerHTML = content;
    }
}

clearDelimer = function(text, delimiter) {
    var lengthDelimiter = delimiter.length;
    var firstPosDelimiter = text.indexOf(delimiter);
    var secondPosDelimiter = text.indexOf(delimiter, firstPosDelimiter + 1);
    var lengthErrorText = secondPosDelimiter - firstPosDelimiter - lengthDelimiter;
    var errorText = text.substr(firstPosDelimiter + lengthDelimiter, lengthErrorText);

    var elem = document.createElement('textarea');
    elem.innerHTML = errorText;
    return elem.value;
};

