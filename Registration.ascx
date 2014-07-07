<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="Registration.ascx.vb" Inherits="Albatros.DNN.Modules.Registration.Registration" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" %>
<%@ Register TagPrefix="ar" Assembly="Albatros.DNN.Modules.Registration" Namespace="Albatros.DNN.Modules.Registration" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls"%>

<div class="dnnForm dnnRegistrationForm">
<h2 id="H1" class="dnnFormSectionHead">
 <a href="#" class="dnnSectionExpanded">
  <%=LocalizeString("secRoles")%></a></h2>
<fieldset>
 <asp:Repeater runat="server" ID="rpRoles">
  <ItemTemplate>
   <div>
    <asp:CheckBox runat="server" ID="chkActive" AutoPostBack="true" />
    <asp:HiddenField runat="server" ID="hidRoleID" Value='<%# Eval("RoleId") %>' />
   </div>
   <div>
    <div><%# Eval("RoleName") %></div>
    <div><%# Eval("Description") %></div>
   </div>
  </ItemTemplate>
 </asp:Repeater>
</fieldset>

    <div class="dnnFormItem dnnClear">
        <ar:DnnProfileEditor runat="server" id="profileForm" />
        <div class="dnnSocialRegistration">
            <div id="mainContainer">
                <ul class="buttonList">
                    <asp:PlaceHolder ID="socialLoginControls" runat="server"/>
                </ul>
            </div>
        </div>
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
