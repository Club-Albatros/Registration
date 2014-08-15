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

 this.listRegions = function (country, success) {
  $.ajax({
   type: "GET",
   url: baseServicepath + 'Country/' + country + '/Regions',
   beforeSend: $.dnnSF(moduleId).setModuleHeaders,
   data: { }
  }).done(function (data) {
   if (success != undefined) {
    success(data);
   }
  }).fail(function (xhr, status) {
   displayMessage(settings.errorBoxId, settings.serverErrorWithDescription + eval("(" + xhr.responseText + ")").ExceptionMessage, "dnnFormWarning");
  });
 };

 this.listSiblingRegions = function (region, success) {
  $.ajax({
   type: "GET",
   url: baseServicepath + 'Region/' + region + '/Siblings',
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

 this.listCities = function (searchString, success) {
  $.ajax({
   type: "GET",
   url: baseServicepath + 'Cities',
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

function setRegionList() {
 $('#' + dnnRegionBoxId + '_dropdown').hide();
 $('#' + dnnRegionBoxId + '_text').show();
 if (typeof dnnRegionBoxId !== 'undefined') {
  var initVal = $('#' + dnnRegionBoxId + '_value').attr('value');
  if (typeof dnnCountryBoxId !== 'undefined') {
   crservice.listRegions($('#' + dnnCountryBoxId + '_code').val(), function (data) {
    setRegionDropdown(data);
   });
  } else {
   if (initVal != '') {
    crservice.listSiblingRegions(initVal, function (data) {
     setRegionDropdown(data);
    });
   }
  }
  $('#' + dnnRegionBoxId + '_dropdown').change(function () {
   $('#' + dnnRegionBoxId + '_value').val($('#' + dnnRegionBoxId + '_dropdown option:selected').val());
  });
  $('#' + dnnRegionBoxId + '_text').change(function () {
   $('#' + dnnRegionBoxId + '_value').val($('#' + dnnRegionBoxId + '_text').val());
  });
 }
}

function setRegionDropdown(data) {
 var dd = $('#' + dnnRegionBoxId + '_dropdown');
 $(dd).empty();
 $.each(data, function (index, value) {
  $(dd).append($('<option>').text(value.Text).attr('value', value.Value));
 });
 if ($(dd).children().length == 0) {
  $(dd).hide();
  $('#' + dnnRegionBoxId + '_text').show();
  $('#' + dnnRegionBoxId + '_text').val($('#' + dnnRegionBoxId + '_value').val());
 } else {
  $(dd).show();
  $('#' + dnnRegionBoxId + '_text').hide();
  $(dd).children('option[value="' + $('#' + dnnRegionBoxId + '_value').val() + '"]').attr('selected', '1');
 }
}

function setupCountryAutoComplete() {
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
   setRegionList();
  }
 })
}

function setupCityAutoComplete() {
 if (typeof dnnCityBoxId !== 'undefined') {
  $('#' + dnnCityBoxId).autocomplete({
   minLength: 2,
   source: function (request, response) {
    crservice.listCities(request.term, function (data) {
     response($.map(data, function (item) {
      return {
       label: item,
       value: item,
       name: item
      };
     }))
    })
   }
  })
 }
}

function foobar() {
 setupCountryAutoComplete();
 setRegionList();
 setupCityAutoComplete();
}

Sys.WebForms.PageRequestManager.getInstance().add_endRequest(foobar);
