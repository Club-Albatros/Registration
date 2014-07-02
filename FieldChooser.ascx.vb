Public Class FieldChooser
 Inherits ModuleBase

 Private Sub Page_Init(sender As Object, e As System.EventArgs) Handles Me.Init

  rpRoles.DataSource = Settings.RolesToShow.Values
  rpRoles.DataBind()

  rpFields.DataSource = DotNetNuke.Entities.Profile.ProfileController.GetPropertyDefinitionsByPortal(PortalId)
  rpFields.DataBind()

 End Sub

 Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

 End Sub

 Private Sub cmdReturn_Click(sender As Object, e As System.EventArgs) Handles cmdReturn.Click
  Me.Response.Redirect(DotNetNuke.Common.NavigateURL(), False)
 End Sub
End Class