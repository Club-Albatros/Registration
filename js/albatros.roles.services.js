function RolesService($, settings, mid) {
 var moduleId = mid;
 var baseServicepath = $.dnnSF(moduleId).getServiceRoot('Albatros/Registration');

 this.reorder = function (order, success) {
  $.ajax({
   type: "POST",
   url: baseServicepath + "Reorder",
   beforeSend: $.dnnSF(moduleId).setModuleHeaders,
   data: { order: order }
  }).done(function (data) {
   if (success != undefined) {
    success();
   }
  }).fail(function (xhr, status) {
   displayMessage(settings.errorBoxId, settings.serverErrorWithDescription + eval("(" + xhr.responseText + ")").ExceptionMessage, "dnnFormWarning");
  });
 };

}