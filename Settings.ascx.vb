﻿Imports DotNetNuke.Security.Roles

Public Class Settings
 Inherits DotNetNuke.Entities.Modules.ModuleSettingsBase

#Region " Properties "
 Private _settings As Albatros.DNN.Modules.Registration.ModuleSettings
 Public Shadows Property Settings() As Albatros.DNN.Modules.Registration.ModuleSettings
  Get

   If _settings Is Nothing Then
    _settings = Albatros.DNN.Modules.Registration.ModuleSettings.GetSettings(ModuleId, PortalId)
   End If
   Return _settings

  End Get
  Set(ByVal Value As Albatros.DNN.Modules.Registration.ModuleSettings)
   _settings = Value
  End Set
 End Property
#End Region

#Region " Page Events "
 Private Sub Page_Init(sender As Object, e As System.EventArgs) Handles Me.Init

  cblRoles.DataSource = (New RoleController).GetPortalRoles(PortalId)
  cblRoles.DataBind()

  For Each itm As ListItem In cblRoles.Items
   If itm.Value = PortalSettings.RegisteredRoleId.ToString Then
    itm.Enabled = False
   End If
  Next

 End Sub
#End Region

#Region " Overrides "
 Public Overrides Sub LoadSettings()

  For Each itm As ListItem In cblRoles.Items
   itm.Selected = Settings.RolesToShow.ContainsKey(Integer.Parse(itm.Value))
  Next
  chkMultiSelect.Checked = Settings.MultiSelect
  chkShowHumanQuestion.Checked = Settings.ShowHumanQuestion

 End Sub

 Public Overrides Sub UpdateSettings()

  Dim res As New List(Of String)
  For Each itm As ListItem In cblRoles.Items
   If itm.Selected AndAlso itm.Value <> PortalSettings.RegisteredRoleId.ToString Then
    res.Add(itm.Value)
   End If
  Next
  If Not chkMultiSelect.Checked Then
   res.Add(PortalSettings.RegisteredRoleId.ToString)
  End If
  Settings.RolesToShow = Globals.GetRoleDictionary(PortalId, String.Join(";", res), Threading.Thread.CurrentThread.CurrentCulture.Name)
  Settings.MultiSelect = chkMultiSelect.Checked
  Settings.ShowHumanQuestion = chkShowHumanQuestion.Checked
  Settings.SaveSettings()

 End Sub
#End Region

End Class