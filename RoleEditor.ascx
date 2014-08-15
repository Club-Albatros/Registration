<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="RoleEditor.ascx.vb" Inherits="Albatros.DNN.Modules.Registration.RoleEditor" %>
<%@ Register TagPrefix="albatros" TagName="role" Src="Role.ascx" %>

<ul class="ar_roles">
<asp:Repeater runat="server" ID="rpRoles">
 <ItemTemplate>
  <li id="role_<%# Eval("RoleId")%>" class="ui-state-default ar_role">
   <albatros:role id="ctrRole" runat="server" />
  </li>
 </ItemTemplate>
</asp:Repeater>
</ul>

<div class="ar_buttonbar">
 <asp:Button runat="server" ID="cmdUpdate" resourcekey="cmdUpdate" class="dnnPrimaryAction" />
 <asp:Button runat="server" ID="cmdCancel" resourcekey="cmdCancel" class="dnnSecondaryAction" />
</div>

<script type="text/javascript">
 var roleService

 (function ($, Sys) {
 $(document).ready(function () {

  roleService = new RolesService($, {
   serverErrorText: '<%= LocalizeJSString("ServerError") %>',
   serverErrorWithDescriptionText: '<%= LocalizeJSString("ServerErrorWithDescription") %>',
   errorBoxId: '#serviceErrorBox<%= ModuleId %>'
  },
  <%= ModuleId %>);

  $('.ar_roles').sortable({
   update: function (event, ui) {
    roleService.reorder($('.ar_roles').sortable('serialize'), null)
   }
  });

 });
} (jQuery, window.Sys));


</script>
