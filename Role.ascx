<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="Role.ascx.vb" Inherits="Albatros.DNN.Modules.Registration.Role" %>


<h2><%= Role.Rolename%></h2>

<div>
 <asp:Repeater runat="server" ID="rpLocalizations">
  <ItemTemplate>
   <div>
    <div class="ar_editrowdiv"><asp:Label runat="server" ID="lblLocale" /></div>
    <div class="ar_editrowdiv"><asp:TextBox runat="server" ID="txtName" Width="250" /></div>
    <div class="ar_editrowdiv"><asp:TextBox runat="server" ID="txtPresentation" Width="400" Height="150" TextMode="MultiLine" /></div>
   </div>
  </ItemTemplate>
 </asp:Repeater>
</div>