Imports DotNetNuke.Web.UI.WebControls
Imports DotNetNuke.Security.Membership
Imports DotNetNuke.Entities.Host
Imports DotNetNuke.Entities.Profile
Imports Albatros.DNN.Modules.Registration.Entities.RoleProperties
Imports DotNetNuke.Common.Lists
Imports DotNetNuke.UI.WebControls
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Users

<ParseChildren(True)>
Public Class DnnProfileEditor
 Inherits DnnFormEditor

#Region " Constants "
 Protected Const PasswordStrengthTextBoxCssClass As String = "password-strength"
 Protected Const ConfirmPasswordTextBoxCssClass As String = "password-confirm"
#End Region

#Region " Properties "
 Private AddedFields As New List(Of String)

 Protected ReadOnly Property PortalSettings As DotNetNuke.Entities.Portals.PortalSettings
  Get
   Return DotNetNuke.Entities.Portals.PortalSettings.Current
  End Get
 End Property

 Protected ReadOnly Property PortalId As Integer
  Get
   Return DotNetNuke.Entities.Portals.PortalSettings.Current.PortalId
  End Get
 End Property

 Public Property SelectedRoles As String = ""

#Region " Portal Settings "
 Protected ReadOnly Property UseEmailAsUserName() As Boolean
  Get
   Return Convert.ToBoolean(GetSetting(PortalId, "Registration_UseEmailAsUserName"))
  End Get
 End Property

 Protected ReadOnly Property EmailValidator() As String
  Get
   Return GetSettingValue("Security_EmailValidation")
  End Get
 End Property

 Protected ReadOnly Property RandomPassword() As Boolean
  Get
   Return Convert.ToBoolean(GetSetting(PortalId, "Registration_RandomPassword"))
  End Get
 End Property

 Protected ReadOnly Property UserNameValidator() As String
  Get
   Return GetSettingValue("Security_UserNameValidation")
  End Get
 End Property

 Protected ReadOnly Property ExcludeTerms() As String
  Get
   Dim excludeTerms__1 As String = GetSettingValue("Registration_ExcludeTerms")
   Dim regex As String = [String].Empty
   If Not [String].IsNullOrEmpty(excludeTerms__1) Then
    regex = "^(?:(?!" & excludeTerms__1.Replace(" ", "").Replace(",", "|") & ").)*$\r?\n?"
   End If
   Return regex
  End Get
 End Property

 Protected ReadOnly Property RequirePasswordConfirm() As Boolean
  Get
   Return Convert.ToBoolean(GetSetting(PortalId, "Registration_RequireConfirmPassword"))
  End Get
 End Property

 Protected ReadOnly Property DisplayNameFormat() As String
  Get
   Return GetSettingValue("Security_DisplayNameFormat")
  End Get
 End Property
#End Region

#End Region

#Region " Overrides "
 Public Overrides Sub DataBind()

  ' Clear Form
  Items.Clear()
  Sections.Clear()

  If PortalSettings.UserId = -1 Then
   Dim mainSection As New DnnFormSection
   mainSection.ResourceKey = "secLogin"
   Sections.Add(mainSection)

   'UserName
   If UseEmailAsUserName Then
    AddField(mainSection, "Email", [String].Empty, True, EmailValidator, TextBoxMode.SingleLine)
   Else
    AddField(mainSection, "Username", [String].Empty, True, If([String].IsNullOrEmpty(UserNameValidator), ExcludeTerms, UserNameValidator), TextBoxMode.SingleLine)
   End If

   'Password
   If Not RandomPassword Then
    AddPasswordStrengthField(mainSection, "Password", "Membership", True)

    If RequirePasswordConfirm Then
     AddPasswordConfirmField(mainSection, "PasswordConfirm", "Membership", True)
    End If
   End If

   'Password Q&A
   If MembershipProviderConfig.RequiresQuestionAndAnswer Then
    AddField(mainSection, "PasswordQuestion", "Membership", True, [String].Empty, TextBoxMode.SingleLine)
    AddField(mainSection, "PasswordAnswer", "Membership", True, [String].Empty, TextBoxMode.SingleLine)
   End If
  End If


  Dim nameSection As New DnnFormSection
  nameSection.ResourceKey = "secName"
  Sections.Add(nameSection)

  'DisplayName
  If [String].IsNullOrEmpty(DisplayNameFormat) Then
   AddField(nameSection, "DisplayName", [String].Empty, True, [String].Empty, TextBoxMode.SingleLine)
  Else
   AddField(nameSection, "FirstName", [String].Empty, True, [String].Empty, TextBoxMode.SingleLine)
   AddField(nameSection, "LastName", [String].Empty, True, [String].Empty, TextBoxMode.SingleLine)
  End If

  'Email
  If Not UseEmailAsUserName Then
   AddField(nameSection, "Email", [String].Empty, True, EmailValidator, TextBoxMode.SingleLine)
  End If

  Dim allProperties As List(Of ProfilePropertyDefinition) = RolePropertiesController.GetProfileProperties(PortalId, SelectedRoles)

  Dim otherSections As New Dictionary(Of String, DnnFormSection)
  For Each [property] As ProfilePropertyDefinition In allProperties
   If [property].Required And Not AddedFields.Contains([property].PropertyName.ToLower) Then
    If Not otherSections.ContainsKey([property].PropertyCategory) Then
     otherSections.Add([property].PropertyCategory, New DnnFormSection With {.ResourceKey = [property].PropertyCategory})
    End If
   End If
  Next

  For Each [property] As ProfilePropertyDefinition In allProperties
   If [property].Required And Not AddedFields.Contains([property].PropertyName.ToLower) Then
    AddProperty(otherSections([property].PropertyCategory), [property])
    AddedFields.Add([property].PropertyName.ToLower)
   End If
  Next

  For Each section As DnnFormSection In otherSections.Values
   Sections.Add(section)
  Next

  MyBase.DataBind()
 End Sub

 Protected Overrides Sub OnPreRender(e As EventArgs)
  MyBase.OnPreRender(e)

  Dim confirmPasswordOptions As New DnnConfirmPasswordOptions With {
   .FirstElementSelector = "." & PasswordStrengthTextBoxCssClass,
   .SecondElementSelector = "." & ConfirmPasswordTextBoxCssClass,
   .ContainerSelector = ".dnnRegistrationForm",
   .UnmatchedCssClass = "unmatched",
   .MatchedCssClass = "matched"
  }

  Dim optionsAsJsonString As String = Json.Serialize(confirmPasswordOptions)
  Dim script As String = String.Format("dnn.initializePasswordComparer({0});{1}", optionsAsJsonString, Environment.NewLine)

  If ScriptManager.GetCurrent(Page) IsNot Nothing Then
   ' respect MS AJAX
   ScriptManager.RegisterStartupScript(Page, [GetType](), "ConfirmPassword", script, True)
  Else
   Page.ClientScript.RegisterStartupScript([GetType](), "ConfirmPassword", script, True)
  End If

 End Sub
#End Region

#Region " Private Methods "
 Private Sub AddField(section As DnnFormSection, dataField As String, dataMember As String, required As Boolean, regexValidator As String, textMode As TextBoxMode)
  If AddedFields.Contains(dataField.ToLower) Then Exit Sub
  Dim formItem As New DnnFormTextBoxItem With {
   .ID = dataField,
   .DataField = dataField,
   .DataMember = dataMember,
   .Visible = True,
   .Required = required,
   .TextMode = textMode,
   .ResourceKey = dataField,
   .LocalResourceFile = "~/DesktopModules/Admin/Security/App_LocalResources/Profile.ascx.resx"
  }
  If Not [String].IsNullOrEmpty(regexValidator) Then
   formItem.ValidationExpression = regexValidator
  End If
  If section Is Nothing Then
   Items.Add(formItem)
  Else
   section.Items.Add(formItem)
  End If
  AddedFields.Add(dataField.ToLower)
 End Sub

 Private Sub AddPasswordStrengthField(section As DnnFormSection, dataField As String, dataMember As String, required As Boolean)
  If AddedFields.Contains(dataField.ToLower) Then Exit Sub
  Dim formItem As DnnFormItemBase

  If Host.EnableStrengthMeter Then
   formItem = New DnnFormPasswordItem() With {
    .TextBoxCssClass = PasswordStrengthTextBoxCssClass,
    .ContainerCssClass = "password-strength-container"
   }
  Else
   formItem = New DnnFormTextBoxItem() With {
    .TextMode = TextBoxMode.Password,
    .TextBoxCssClass = PasswordStrengthTextBoxCssClass
   }
  End If

  formItem.ID = dataField
  formItem.DataField = dataField
  formItem.DataMember = dataMember
  formItem.Visible = True
  formItem.Required = required
  formItem.ResourceKey = dataField
  formItem.LocalResourceFile = "~/DesktopModules/Admin/Security/App_LocalResources/Profile.ascx.resx"

  If section Is Nothing Then
   Items.Add(formItem)
  Else
   section.Items.Add(formItem)
  End If
  AddedFields.Add(dataField.ToLower)

 End Sub

 Private Sub AddPasswordConfirmField(section As DnnFormSection, dataField As String, dataMember As String, required As Boolean)
  If AddedFields.Contains(dataField.ToLower) Then Exit Sub
  Dim formItem As New DnnFormTextBoxItem() With {
   .ID = dataField,
   .DataField = dataField,
   .DataMember = dataMember,
   .Visible = True,
   .Required = required,
   .TextMode = TextBoxMode.Password,
   .TextBoxCssClass = ConfirmPasswordTextBoxCssClass,
   .ResourceKey = dataField,
   .LocalResourceFile = "~/DesktopModules/Admin/Security/App_LocalResources/Profile.ascx.resx"
  }

  If section Is Nothing Then
   Items.Add(formItem)
  Else
   section.Items.Add(formItem)
  End If
  AddedFields.Add(dataField.ToLower)

 End Sub

 Private Sub AddProperty(section As DnnFormSection, [property] As ProfilePropertyDefinition)
  Dim controller As New ListController()
  Dim imageType As ListEntryInfo = controller.GetListEntryInfo("DataType", "Image")
  If [property].DataType <> imageType.EntryID Then
   Dim formItem As New DnnFormEditControlItem With {
    .ID = [property].PropertyName,
    .ResourceKey = [String].Format("ProfileProperties_{0}", [property].PropertyName),
    .LocalResourceFile = "~/DesktopModules/Admin/Security/App_LocalResources/Profile.ascx.resx",
    .ValidationMessageSuffix = ".Validation",
    .ControlType = EditorInfo.GetEditor([property].DataType),
    .DataMember = "Profile",
    .DataField = [property].PropertyName,
    .Visible = [property].Visible,
    .Required = [property].Required
   }
   'To check if the property has a deafult value
   If Not [String].IsNullOrEmpty([property].DefaultValue) Then
    formItem.Value = [property].DefaultValue
   End If
   If Not [String].IsNullOrEmpty([property].ValidationExpression) Then
    formItem.ValidationExpression = [property].ValidationExpression
   End If

   If section Is Nothing Then
    Items.Add(formItem)
   Else
    section.Items.Add(formItem)
   End If

  End If

 End Sub
#End Region

#Region " Portal Settings Methods "
 Private Shared Function GetSetting(portalId As Integer, settingKey As String) As Object
  Dim settings As Hashtable = UserController.GetUserSettings(portalId)
  Return settings(settingKey)
 End Function

 Private Function GetSettingValue(key As String) As String
  Dim value As String = [String].Empty
  Dim setting As Object = GetSetting(UserPortalID, key)
  If (setting IsNot Nothing) AndAlso (Not [String].IsNullOrEmpty(Convert.ToString(setting))) Then
   value = Convert.ToString(setting)
  End If
  Return value
 End Function

 Protected ReadOnly Property UserPortalID() As Integer
  Get
   Return If(DotNetNuke.Common.Globals.IsHostTab(PortalSettings.ActiveTab.TabID), Null.NullInteger, PortalId)
  End Get
 End Property
#End Region

End Class
