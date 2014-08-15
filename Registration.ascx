<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="Registration.ascx.vb" Inherits="Albatros.DNN.Modules.Registration.Registration" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" %>
<%@ Register TagPrefix="ar" Assembly="Albatros.DNN.Modules.Registration" Namespace="Albatros.DNN.Modules.Registration" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls"%>

<asp:Panel runat="server" ID="pnlSocialLogin">
<h2 id="H2" class="dnnFormSectionHead">
 <a href="#" class="dnnSectionExpanded">
  <%=LocalizeString("secConnect")%></a></h2>
<div class="dnnSocialRegistration">
 <ul class="buttonList">
  <asp:PlaceHolder ID="socialLoginControls" runat="server"/>
 </ul>
</div>
</asp:Panel>

<div class="dnnForm dnnRegistrationForm">
<h2 id="H1" class="dnnFormSectionHead">
 <a href="#" class="dnnSectionExpanded">
  <%=LocalizeString("secRoles")%></a></h2>
<fieldset>
 <asp:Repeater runat="server" ID="rpRoles">
  <ItemTemplate>
   <div class="ar_rs">
    <asp:CheckBox runat="server" ID="chkActive" AutoPostBack="true" />
    <div class="ar_rs_name"><%# Eval("RoleName") %></div>
    <div class="ar_rs_presentation"><%# Eval("Presentation")%></div>
   </div>
   <asp:HiddenField runat="server" ID="hidRoleID" Value='<%# Eval("RoleId") %>' />
  </ItemTemplate>
 </asp:Repeater>
</fieldset>

 <div class="dnnFormItem dnnClear">
  <ar:DnnProfileEditor runat="server" id="profileForm" />
 </div>
 <div id="captchaRow" runat="server" visible="false" class="dnnFormItem dnnCaptcha">
  <dnn:label id="captchaLabel" controlname="ctlCaptcha" runat="server" />
  <dnn:captchacontrol id="ctlCaptcha" captchawidth="130" captchaheight="40" ErrorStyle-CssClass="dnnFormMessage dnnFormError dnnCaptcha" runat="server" />
 </div>
 <ul id="actionsRow" runat="server" class="dnnActions dnnClear">
  <li><asp:LinkButton id="registerButton" runat="server" CssClass="dnnPrimaryAction" resourcekey="cmdRegister" /></li>
  <li><asp:LinkButton id="cancelButton" runat="server" CssClass="dnnSecondaryAction" resourcekey="cmdCancel" CausesValidation="false" /></li>
 </ul>
</div>


<script type="text/javascript">
 $(function () {
  $('.dnnFormItem .dnnLabel').each(function () {
   var next = $(this).next();
   if (next.hasClass('dnnFormRequired'))
    $(this).find('span').addClass('dnnFormRequired');
  });
 });
</script>
