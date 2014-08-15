Imports System.Linq
Imports Albatros.DNN.Modules.Registration.Entities.Roles

Public Class RoleEditor
 Inherits ModuleBase

 Private Sub Page_Init(sender As Object, e As System.EventArgs) Handles Me.Init

  AddJavascriptFile("albatros.roles.services.js", 70)
  rpRoles.DataSource = Settings.RolesToShow.Values
  rpRoles.DataBind()

 End Sub

 Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

  DotNetNuke.Framework.jQuery.RequestUIRegistration()

 End Sub

 Private Sub rpRoles_ItemDataBound(sender As Object, e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rpRoles.ItemDataBound
  If (e.Item.ItemType = ListItemType.Item) Or (e.Item.ItemType = ListItemType.AlternatingItem) Then
   Dim ctr As Role = CType(e.Item.FindControl("ctrRole"), Role)
   ctr.ModuleConfiguration = Me.ModuleConfiguration
   ctr.Role = CType(e.Item.DataItem, RegistrationRoleInfo)
  End If
 End Sub

 Private Sub cmdUpdate_Click(sender As Object, e As System.EventArgs) Handles cmdUpdate.Click

  For Each itm As RepeaterItem In rpRoles.Items
   Dim ctrRole As Role = CType(itm.FindControl("ctrRole"), Role)
   ctrRole.Update()
  Next
  Globals.ClearRolesCache(PortalId)
  Settings.ClearCache()

  Me.Response.Redirect(DotNetNuke.Common.NavigateURL(), False)

 End Sub

 Private Sub cmdCancel_Click(sender As Object, e As System.EventArgs) Handles cmdCancel.Click
  Globals.ClearRolesCache(PortalId)
  Settings.ClearCache()
  Me.Response.Redirect(DotNetNuke.Common.NavigateURL(), False)
 End Sub
End Class