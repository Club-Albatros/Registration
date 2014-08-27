<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="Settings.ascx.vb" Inherits="Albatros.DNN.Modules.Registration.Settings" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<fieldset>
 <div class="dnnFormItem">
  <dnn:label id="plMultiSelect" runat="server" controlname="chkMultiSelect" suffix=":" />
  <asp:CheckBox runat="server" ID="chkMultiSelect" />
 </div>
 <div class="dnnFormItem">
  <dnn:label id="plRoles" runat="server" controlname="cblRoles" suffix=":" />
  <asp:CheckBoxList runat="server" ID="cblRoles" DataTextField="RoleName" DataValueField="RoleID" />
 </div>
</fieldset>
