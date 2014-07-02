<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="FieldChooser.ascx.vb" Inherits="Albatros.DNN.Modules.Registration.FieldChooser" %>

<table id="fieldchooser">
 <tr>
  <th></th>
<asp:Repeater runat="server" ID="rpRoles">
 <ItemTemplate>
  <th data-id="<%# Eval("RoleId")%>" class="ar_role"><%# Eval("RoleName")%></th>
 </ItemTemplate>
</asp:Repeater>
 </tr>
<asp:Repeater runat="server" ID="rpFields">
 <ItemTemplate>
  <tr data-id="<%# Eval("PropertyDefinitionId")%>" class="ar_property">
   <td><%# DotNetNuke.Services.Localization.Localization.GetString("ProfileProperties_" & Eval("PropertyName"),"DesktopModules\Admin\Security\App_LocalResources\Profile.ascx.resx").TrimEnd(":")%></td>
  </tr>
 </ItemTemplate>
</asp:Repeater>
</table>

<div class="ar_buttonbar">
 <asp:Button runat="server" ID="cmdReturn" resourcekey="cmdReturn" class="dnnPrimaryAction" />
</div>

<script type="text/javascript">
(function ($, Sys) {

 $(document).ready(function () {

  service = new RegistrationService($, {
   serverErrorText: '<%= LocalizeJSString("ServerError") %>',
   serverErrorWithDescriptionText: '<%= LocalizeJSString("ServerErrorWithDescription") %>',
   errorBoxId: '#pUrlsServiceErrorBox<%= ModuleId %>'
  }, <%= ModuleId %>);

  var roles = [];
  roles = $('.ar_role').map(function() {
   return $(this).attr('data-id');
  });

  var checkboxes = ''
  $.each(roles, function(index, roleId) {
   checkboxes += '<td><img src="<%= ResolveUrl("~/DesktopModules/Albatros/Registration/images/check.png") %>" width="24" height="24" data-value="' + roleId + '" data-state="notselected" class="ar_checkbox" /></td>'
  });

  $('.ar_property').append(checkboxes);

  service.getRoleProperties(function (data) {
   $.each(data, function(index, roleprop) {
    if (roleprop.Required) {
     $('tr[data-id=' + roleprop.PropertyDefinitionID + '] td img.ar_checkbox[data-value=' + roleprop.RoleId + ']').attr('src', '<%= ResolveUrl("~/DesktopModules/Albatros/Registration/images/check_required.png") %>').attr('data-state', 'required');
    } else {
     $('tr[data-id=' + roleprop.PropertyDefinitionID + '] td img.ar_checkbox[data-value=' + roleprop.RoleId + ']').attr('src', '<%= ResolveUrl("~/DesktopModules/Albatros/Registration/images/check_selected.png") %>').attr('data-state', 'selected');
    }
   });
  });

  $('.ar_checkbox').click(function() {
   var switchTo = ''
   switch ($(this).attr('data-state')) {
    case 'required':
     switchTo = 'notselected';
     break;
    case 'selected':
     switchTo = 'required';
     break;
    default:
     switchTo = 'selected';
   }

   var that = $(this);

   service.setRoleProperty($(this).parent().parent().attr('data-id'), $(this).attr('data-value'), switchTo, function() {
    $(that).attr('data-state', switchTo);
    switch (switchTo) {
     case 'required':
      $(that).attr('src', '<%= ResolveUrl("~/DesktopModules/Albatros/Registration/images/check_required.png") %>');
      break;
     case 'selected':
      $(that).attr('src', '<%= ResolveUrl("~/DesktopModules/Albatros/Registration/images/check_selected.png") %>');
      break;
     default:
      $(that).attr('src', '<%= ResolveUrl("~/DesktopModules/Albatros/Registration/images/check.png") %>');
    }
   });

  });

 }); // doc ready

} (jQuery, window.Sys));
</script>
