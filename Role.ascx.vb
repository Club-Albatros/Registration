Imports Albatros.DNN.Modules.Registration.Entities.Roles
Imports Albatros.DNN.Modules.Registration.Entities.RoleLocalizations

Public Class Role
 Inherits ModuleBase

 Public Property Role As RegistrationRoleInfo = Nothing

 Private Sub Page_Init(sender As Object, e As System.EventArgs) Handles Me.Init

 End Sub

 Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

  rpLocalizations.DataSource = RoleLocalizationsController.GetRoleLocalizationsByRole(PortalId, Role.RoleID)
  rpLocalizations.DataBind()

 End Sub

 Private Sub rpLocalizations_ItemDataBound(sender As Object, e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rpLocalizations.ItemDataBound

  If (e.Item.ItemType = ListItemType.Item) Or (e.Item.ItemType = ListItemType.AlternatingItem) Then
   Dim rl As RoleLocalizationInfo = CType(e.Item.DataItem, RoleLocalizationInfo)
   Dim lblLocale As Label = CType(e.Item.FindControl("lblLocale"), Label)
   Dim txtName As TextBox = CType(e.Item.FindControl("txtName"), TextBox)
   Dim txtPresentation As TextBox = CType(e.Item.FindControl("txtPresentation"), TextBox)
   lblLocale.Text = rl.Locale
   txtName.Text = rl.RoleName
   txtPresentation.Text = rl.Presentation
  End If

 End Sub

 Public Sub Update()

  Data.DataProvider.Instance.RemoveRoleLocalizations(Role.RoleID)

  For Each itm As RepeaterItem In rpLocalizations.Items

   Dim locale As String = CType(itm.FindControl("lblLocale"), Label).Text
   Dim name As String = CType(itm.FindControl("txtName"), TextBox).Text.Trim
   Dim presentation As String = CType(itm.FindControl("txtPresentation"), TextBox).Text.Trim

   If Not (String.IsNullOrEmpty(name) And String.IsNullOrEmpty(presentation)) Then
    Data.DataProvider.Instance.SetRoleLocalization("", "", locale, presentation, Role.RoleID, name)
   End If

  Next

 End Sub
End Class