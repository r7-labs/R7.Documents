function r7d_selectDocument(documentId, checked, value) {
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
}
function r7d_getModuleId(target) {
	var c = $(target).closest("div.DnnModule").attr("class");
	var moduleId = c.match("DnnModule-(\\d+)")[1];
	return moduleId;
}
function r7d_selectDocument2(target) {
	var documentId = $(target).data("document-id");
	var moduleId = r7d_getModuleId(target);
	var field = document.getElementById("dnn_ctr" + moduleId + "_ViewDocuments_hiddenSelectedDocuments");
	field.value = r7d_selectDocument(documentId, target.checked, field.value);
}
function r7d_selectDeselectAll(target) {
	var moduleId = r7d_getModuleId(target);
	$("div.DnnModule-" + moduleId + " .EditCell input[type='checkbox']").prop("checked", target.checked).trigger("change");
}
