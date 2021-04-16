"use strict";

function R7_DocumentSelector () {
}

R7_DocumentSelector.prototype.selectDocument = function(documentId, checked, value) {
    var values = JSON.parse(value);
    var index = values.indexOf(documentId);
    if (checked) {
        if (index < 0) {
            values.push(documentId);
        }
    } else {
        if (index >= 0) {
            values.splice(index, 1);
        }
    }
    return JSON.stringify(values);
};

R7_DocumentSelector.prototype.getModuleId = function(target) {
    var c = $(target).closest("div.DnnModule").attr("class");
    var moduleId = c.match("DnnModule-(\\d+)")[1];
    return moduleId;
};

R7_DocumentSelector.prototype.selectDocument2 = function(target) {
    var documentId = $(target).data("document-id");
    var moduleId = this.getModuleId(target);
    var field = document.getElementById("dnn_ctr" + moduleId + "_ViewDocuments_hiddenSelectedDocuments");
    field.value = this.selectDocument(documentId, target.checked, field.value);
};

R7_DocumentSelector.prototype.toggleAll = function(target) {
    var moduleId = this.getModuleId(target);
    $(".DnnModule-" + moduleId + " .edit-cell input[type='checkbox']").prop("checked", target.checked).trigger("change");
};

window.r7_documentSelector = new R7_DocumentSelector();

