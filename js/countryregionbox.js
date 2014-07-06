(function ($, Sys) {
 $(document).ready(function () {
  // alert($('#' + dnnCountryBoxId).length);

  crservice = new CountryRegionService($, {
   serverErrorText: '',
   serverErrorWithDescriptionText: '',
   errorBoxId: ''
  }, 500);

  setupAutoComplete();

  if (typeof dnnRegionBoxId === 'undefined') {
   // alert('yes');
  }


 }); // doc ready
} (jQuery, window.Sys));

function CountryRegionService($, settings, mid) {
 var moduleId = mid;
 var baseServicepath = $.dnnSF(moduleId).getServiceRoot('Albatros/Registration');

 this.listCountries = function (searchString, success) {
  $.ajax({
   type: "GET",
   url: baseServicepath + 'Countries',
   beforeSend: $.dnnSF(moduleId).setModuleHeaders,
   data: { searchString: searchString }
  }).done(function (data) {
   if (success != undefined) {
    success(data);
   }
  }).fail(function (xhr, status) {
   displayMessage(settings.errorBoxId, settings.serverErrorWithDescription + eval("(" + xhr.responseText + ")").ExceptionMessage, "dnnFormWarning");
  });
 };


}

function setupAutoComplete() {
 $('#' + dnnCountryBoxId + '_name').autocomplete({
  minLength: 2,
  source: function (request, response) {
   crservice.listCountries(request.term, function (data) {
    response($.map(data, function (item) {
     return {
      label: item.FullName,
      id: item.Code,
      value: item.Code,
      name: item.Name
     };
    }))
   })
  },
  select: function (event, ui) {
   $('#' + dnnCountryBoxId + '_code').val(ui.item.id);
   $('#' + dnnCountryBoxId + '_name').attr('data-text', ui.item.name);
  },
  close: function () {
   $('#' + dnnCountryBoxId + '_name').val($('#' + dnnCountryBoxId + '_name').attr('data-text'));
  }
 })
}


function foobar() {
 // alert(crservice);
 setupAutoComplete();
}

Sys.WebForms.PageRequestManager.getInstance().add_endRequest(foobar);
