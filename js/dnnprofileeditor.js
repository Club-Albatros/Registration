(function ($, Sys) {
 $(document).ready(function () {
  hookupNameBoxes();
 }); // doc ready
} (jQuery, window.Sys));

function hookupNameBoxes() {
 if (typeof dnnProfileEditorId !== 'undefined') {
  if ($('#' + dnnProfileEditorId + '_DisplayName_DisplayName_TextBox') && $('#' + dnnProfileEditorId + '_FirstName_FirstName_Control') && $('#' + dnnProfileEditorId + '_LastName_LastName_Control')) {
   $('#' + dnnProfileEditorId + '_FirstName_FirstName_Control').keyup(function () {
    $('#' + dnnProfileEditorId + '_DisplayName_DisplayName_TextBox').val($('#' + dnnProfileEditorId + '_FirstName_FirstName_Control').val() + ' ' + $('#' + dnnProfileEditorId + '_LastName_LastName_Control').val());
   });
   $('#' + dnnProfileEditorId + '_LastName_LastName_Control').keyup(function () {
    $('#' + dnnProfileEditorId + '_DisplayName_DisplayName_TextBox').val($('#' + dnnProfileEditorId + '_FirstName_FirstName_Control').val() + ' ' + $('#' + dnnProfileEditorId + '_LastName_LastName_Control').val());
   });
  }
 }
}

Sys.WebForms.PageRequestManager.getInstance().add_endRequest(hookupNameBoxes());

