<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="Registration.ascx.vb" Inherits="Albatros.DNN.Modules.Registration.Registration" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" %>
<%@ Register TagPrefix="ar" Assembly="Albatros.DNN.Modules.Registration" Namespace="Albatros.DNN.Modules.Registration" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls" %>

<asp:Panel runat="server" ID="pnlSocialLogin">
 <h2 id="H2" class="dnnFormSectionHead">
  <a href="#" class="dnnSectionExpanded">
   <%=LocalizeString("secConnect")%></a></h2>
 <div class="dnnSocialRegistration">
  <ul class="buttonList">
   <asp:PlaceHolder ID="socialLoginControls" runat="server" />
  </ul>
 </div>
</asp:Panel>

<asp:Panel runat="server" ID="pnlRoles">
 <div class="dnnForm dnnRegistrationForm">
  <h2 id="H1" class="dnnFormSectionHead">
   <a href="#" class="dnnSectionExpanded">
    <%=LocalizeString("secRoles")%></a></h2>
  <fieldset>
   <asp:Repeater runat="server" ID="rpRoles">
    <ItemTemplate>
     <div class="ar_rs">
      <asp:CheckBox runat="server" ID="chkActive" AutoPostBack="true" />
      <asp:RadioButton runat="server" ID="rdoActive" AutoPostBack="true" />
      <div class="ar_rs_name"><%# Eval("RoleName") %></div>
      <div class="ar_rs_presentation"><%# Eval("Presentation")%></div>
     </div>
     <asp:HiddenField runat="server" ID="hidRoleID" Value='<%# Eval("RoleId") %>' />
    </ItemTemplate>
   </asp:Repeater>
  </fieldset>
 </div>
</asp:Panel>

<div class="dnnClear">
 <ar:dnnprofileeditor runat="server" id="profileForm" />
</div>

<div id="captchaRow" runat="server" visible="false" class="dnnFormItem dnnCaptcha">
 <dnn:label id="captchaLabel" controlname="ctlCaptcha" runat="server" />
 <dnn:captchacontrol id="ctlCaptcha" captchawidth="130" captchaheight="40" ErrorStyle-CssClass="dnnFormMessage dnnFormError dnnCaptcha" runat="server" />
</div>

<div id="humanQuestionRow" runat="server" visible="false" class="dnnFormItem">
    <div class="dnnLabel"><label><asp:Label runat="server" ID="humanQuestion" /></label></div>
    <div id="humanAnswerRow" runat="server"></div>
    <asp:HiddenField runat="server" ID="humanAnswer" />
</div>

<ul id="actionsRow" runat="server" class="dnnActions dnnClear">
 <li>
  <asp:LinkButton ID="cmdUpdate" runat="server" CssClass="dnnPrimaryAction" resourcekey="cmdUpdate" Visible="false" />
 </li>
 <li>
  <asp:LinkButton ID="registerButton" runat="server" CssClass="dnnPrimaryAction" resourcekey="cmdRegister" Visible="false" />
 </li>
 <li>
  <asp:LinkButton ID="cancelButton" runat="server" CssClass="dnnSecondaryAction" resourcekey="cmdCancel" CausesValidation="false" />
 </li>
</ul>
</div>

<asp:HiddenField runat="server" ID="hidVerToken" />

<script type="text/javascript">
 (function ($, Sys) {
  $(document).ready(function () {

   crservice = new CountryRegionService($, {
    serverErrorText: '',
    serverErrorWithDescriptionText: '',
    errorBoxId: ''
   }, <%= ModuleId %>);

   setupCountryAutoComplete();
   setRegionList();
   setupCityAutoComplete();

   $('.dnnFormItem .dnnLabel').each(function () {
    var next = $(this).next();
    if (next.hasClass('dnnFormRequired'))
     $(this).find('span').addClass('dnnFormRequired');
   });

   $('.ar_rs input[type="radio"]').click(function() {
    $('.ar_rs input[type="radio"]').prop("checked", false);
    $(this).prop("checked", true);
   });

  }); // doc ready
 } (jQuery, window.Sys));
</script>
