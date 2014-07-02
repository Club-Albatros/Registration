function RegistrationService($, settings, mid) {
 var moduleId = mid;
 var baseServicepath = $.dnnSF(moduleId).getServiceRoot('Albatros/Registration');

 this.getRoleProperties = function (success) {
  $.ajax({
   type: "GET",
   url: baseServicepath + 'Props/GetRoleProperties',
   beforeSend: $.dnnSF(moduleId).setModuleHeaders,
   data: {}
  }).done(function (data) {
   if (success != undefined) {
    success(data);
   }
  }).fail(function (xhr, status) {
   displayMessage(settings.errorBoxId, settings.serverErrorWithDescription + eval("(" + xhr.responseText + ")").ExceptionMessage, "dnnFormWarning");
  });
 };

 this.setRoleProperty = function (propertyDefinitionID, roleId, state, success) {
  $.ajax({
   type: "POST",
   url: baseServicepath + 'Props/SetRoleProperty',
   beforeSend: $.dnnSF(moduleId).setModuleHeaders,
   data: { propertyDefinitionID: propertyDefinitionID, roleId: roleId, state: state }
  }).done(function (data) {
   if (success != undefined) {
    success(data);
   }
  }).fail(function (xhr, status) {
   displayMessage(settings.errorBoxId, settings.serverErrorWithDescription + eval("(" + xhr.responseText + ")").ExceptionMessage, "dnnFormWarning");
  });
 };

}