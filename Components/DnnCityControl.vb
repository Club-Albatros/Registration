Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.UI.WebControls
Imports DotNetNuke.Entities.Portals

<ToolboxData("<{0}:DnnCityControl runat=server></{0}:DnnCityControl>")>
Public Class DnnCityControl
 Inherits TextEditControl

#Region " Page Events "
 Private Sub DnnCountryRegionControl_Init(sender As Object, e As System.EventArgs) Handles Me.Init
  DotNetNuke.Web.Client.ClientResourceManagement.ClientResourceManager.RegisterScript(Page, ResolveUrl("~/DesktopModules/Albatros/Registration/js/countryregionbox.js"), 70)
  DotNetNuke.Framework.jQuery.RequestRegistration()
  DotNetNuke.Framework.jQuery.RequestUIRegistration()
 End Sub

 Private Sub DnnCountryRegionControl_Load(sender As Object, e As System.EventArgs) Handles Me.Load

  Dim script As String = String.Format(
  <![CDATA[
  <script type='text/javascript'>
   var dnnCityBoxId = '{0}';
  </script>
  ]]>.Value, Me.ClientID)
  DotNetNuke.UI.Utilities.ClientAPI.RegisterStartUpScript(Page, "DnnCityControl", script)

 End Sub
#End Region

End Class
