Imports DotNetNuke.Web.UI.WebControls
Imports DotNetNuke.Security.Membership
Imports DotNetNuke.Entities.Host
Imports DotNetNuke.Entities.Profile
Imports Albatros.DNN.Modules.Registration.Entities.RoleProperties
Imports DotNetNuke.Common.Lists
Imports DotNetNuke.UI.WebControls
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Users
Imports DotNetNuke.Services
Imports DotNetNuke.Security
Imports DotNetNuke.Entities.Users.Membership
Imports DotNetNuke.Entities.Users.Internal

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

 Protected Shadows ReadOnly Property IsValid() As Boolean
  Get
   Return Validate()
  End Get
 End Property

 Private _User As UserInfo = Nothing
 Public Property User() As UserInfo
  Get
   If _User Is Nothing Then
    If PortalSettings.UserId = -1 Then
     _User = InitialiseUser()
    Else
     _User = PortalSettings.UserInfo
    End If
   End If
   Return _User
  End Get
  Set(value As UserInfo)
   _User = value
  End Set
 End Property

 Protected Property CreateStatus() As UserCreateStatus

 Protected Property AuthenticationType() As String
  Get
   Return ViewState.GetValue("AuthenticationType", Null.NullString)
  End Get
  Set(value As String)
   ViewState.SetValue("AuthenticationType", value, Null.NullString)
  End Set
 End Property

 Private Structure DataType
  Public Name As String
  Public Editor As String
 End Structure
 Private Property DataTypes As New Dictionary(Of Integer, DataType)

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

 Protected ReadOnly Property UseProfanityFilter() As Boolean
  Get
   Return Convert.ToBoolean(GetSetting(PortalId, "Registration_UseProfanityFilter"))
  End Get
 End Property

 Protected ReadOnly Property RequireUniqueDisplayName() As Boolean
  Get
   Return Convert.ToBoolean(GetSetting(PortalId, "Registration_RequireUniqueDisplayName"))
  End Get
 End Property
#End Region

#End Region

#Region " Events "
 Private Sub DnnProfileEditor_Init(sender As Object, e As System.EventArgs) Handles Me.Init
  For Each t As ListEntryInfo In (New ListController).GetListEntryInfoItems("DataType")
   DataTypes.Add(t.EntryID, New DataType With {.Name = t.Value, .Editor = t.Text})
  Next
 End Sub
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

  Me.DataSource = Me.User

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

  Dim formItem As DnnFormEditControlItem = Nothing
  Select Case DataTypes([property].DataType).Name
   Case "Image"
   Case "Country"
    formItem = New DnnFormEditControlItem With {
     .ControlType = "Albatros.DNN.Modules.Registration.DnnCountryRegionControl, ALBATROS.DNN.MODULES.REGISTRATION",
     .DataMember = "Country"
    }
   Case "Region"
    formItem = New DnnFormEditControlItem With {
     .ControlType = "Albatros.DNN.Modules.Registration.DnnCountryRegionControl, ALBATROS.DNN.MODULES.REGISTRATION",
     .DataMember = "Region"
    }
   Case Else
    formItem = New DnnFormEditControlItem With {
     .ControlType = DataTypes([property].DataType).Editor,
     .DataMember = "Profile"
    }
  End Select

  If formItem Is Nothing Then Exit Sub

  With formItem
   .ID = [property].PropertyName
   .ResourceKey = String.Format("ProfileProperties_{0}", [property].PropertyName)
   .LocalResourceFile = "~/DesktopModules/Admin/Security/App_LocalResources/Profile.ascx.resx"
   .ValidationMessageSuffix = ".Validation"
   .DataField = [property].PropertyName
   .Visible = [property].Visible
   .Required = [property].Required
  End With

  'To check if the property has a default value
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

 End Sub

 Private Function Validate() As Boolean
  CreateStatus = UserCreateStatus.AddUser
  Dim portalSecurity__1 As New PortalSecurity

  'Check User Editor
  Dim _IsValid As Boolean = MyBase.IsValid

  'Update UserName
  If UseEmailAsUserName Then
   User.Username = User.Email
   If [String].IsNullOrEmpty(User.DisplayName) Then
    User.DisplayName = User.Email.Substring(0, User.Email.IndexOf("@", StringComparison.Ordinal))
   End If
  End If

  'Check Password is valid
  If Not RandomPassword Then
   'Check Password is Valid
   If CreateStatus = UserCreateStatus.AddUser AndAlso Not UserController.ValidatePassword(User.Membership.Password) Then
    CreateStatus = UserCreateStatus.InvalidPassword
   End If

   If RequirePasswordConfirm AndAlso [String].IsNullOrEmpty(AuthenticationType) Then
    If User.Membership.Password <> User.Membership.PasswordConfirm Then
     CreateStatus = UserCreateStatus.PasswordMismatch
    End If
   End If
  Else
   'Generate a random password for the user
   User.Membership.Password = UserController.GeneratePassword()
   User.Membership.PasswordConfirm = User.Membership.Password

  End If

  'Validate banned password
  Dim settings As New MembershipPasswordSettings(User.PortalID)

  If settings.EnableBannedList Then
   Dim m As New MembershipPasswordController
   If m.FoundBannedPassword(User.Membership.Password) OrElse User.Username = User.Membership.Password Then
    CreateStatus = UserCreateStatus.BannedPasswordUsed

   End If
  End If
  'Validate Profanity
  If UseProfanityFilter Then
   If Not portalSecurity__1.ValidateInput(User.Username, PortalSecurity.FilterFlag.NoProfanity) Then
    CreateStatus = UserCreateStatus.InvalidUserName
   End If
   If Not [String].IsNullOrEmpty(User.DisplayName) Then
    If Not portalSecurity__1.ValidateInput(User.DisplayName, PortalSecurity.FilterFlag.NoProfanity) Then
     CreateStatus = UserCreateStatus.InvalidDisplayName
    End If
   End If
  End If

  'Validate Unique User Name
  Dim user__2 As UserInfo = UserController.GetUserByName(PortalId, User.Username)
  If user__2 IsNot Nothing Then
   If UseEmailAsUserName Then
    CreateStatus = UserCreateStatus.DuplicateEmail
   Else
    CreateStatus = UserCreateStatus.DuplicateUserName
    Dim i As Integer = 1
    Dim userName As String = Nothing
    While user__2 IsNot Nothing
     userName = User.Username + "0" & i.ToString(Globalization.CultureInfo.InvariantCulture)
     user__2 = UserController.GetUserByName(PortalId, userName)
     i += 1
    End While
    User.Username = userName
   End If
  End If

  'Validate Unique Display Name
  If CreateStatus = UserCreateStatus.AddUser AndAlso RequireUniqueDisplayName Then
   user__2 = TestableUserController.Instance.GetUserByDisplayname(PortalId, User.DisplayName)
   If user__2 IsNot Nothing Then
    CreateStatus = UserCreateStatus.DuplicateDisplayName
    Dim i As Integer = 1
    Dim displayName As String = Nothing
    While user__2 IsNot Nothing
     displayName = User.DisplayName + " 0" & i.ToString(Globalization.CultureInfo.InvariantCulture)
     user__2 = TestableUserController.Instance.GetUserByDisplayname(PortalId, displayName)
     i += 1
    End While
    User.DisplayName = displayName
   End If
  End If

  'Check Question/Answer
  If CreateStatus = UserCreateStatus.AddUser AndAlso MembershipProviderConfig.RequiresQuestionAndAnswer Then
   If String.IsNullOrEmpty(User.Membership.PasswordQuestion) Then
    'Invalid Question
    CreateStatus = UserCreateStatus.InvalidQuestion
   End If
   If CreateStatus = UserCreateStatus.AddUser Then
    If String.IsNullOrEmpty(User.Membership.PasswordAnswer) Then
     'Invalid Question
     CreateStatus = UserCreateStatus.InvalidAnswer
    End If
   End If
  End If

  If CreateStatus <> UserCreateStatus.AddUser Then
   _IsValid = False
  End If
  Return _IsValid
 End Function
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

#Region " Private User Methods "
 Private Function InitialiseUser() As UserInfo
  Dim newUser As New UserInfo()
  newUser.PortalID = PortalId

  'Initialise the ProfileProperties Collection
  Dim lc As String = New DotNetNuke.Services.Localization.Localization().CurrentUICulture

  newUser.Profile.InitialiseProfile(PortalId)
  newUser.Profile.PreferredTimeZone = PortalSettings.TimeZone

  newUser.Profile.PreferredLocale = lc

  'Set default countr
  Dim country As String = Null.NullString
  country = LookupCountry()
  If Not [String].IsNullOrEmpty(country) Then
   newUser.Profile.Country = country
  End If
  'Set AffiliateId
  Dim AffiliateId As Integer = Null.NullInteger
  If HttpContext.Current.Request.Cookies("AffiliateId") IsNot Nothing Then
   AffiliateId = Integer.Parse(HttpContext.Current.Request.Cookies("AffiliateId").Value)
  End If
  newUser.AffiliateID = AffiliateId
  Return newUser
 End Function

 Private Function LookupCountry() As String
  Dim IP As String
  Dim IsLocal As Boolean = False
  Dim _CacheGeoIPData As Boolean = True
  Dim _GeoIPFile As String
  _GeoIPFile = "controls/CountryListBox/Data/GeoIP.dat"
  If Page.Request.UserHostAddress = "127.0.0.1" Then
   ''The country cannot be detected because the user is local.
   IsLocal = True
   'Set the IP address in case they didn't specify LocalhostCountryCode
   IP = Page.Request.UserHostAddress
  Else
   'Set the IP address so we can find the country
   IP = Page.Request.UserHostAddress
  End If
  'Check to see if we need to generate the Cache for the GeoIPData file
  If Context.Cache.[Get]("GeoIPData") Is Nothing AndAlso _CacheGeoIPData Then
   'Store it as	well as	setting	a dependency on	the	file
   Context.Cache.Insert("GeoIPData", CountryLookup.FileToMemory(Context.Server.MapPath(_GeoIPFile)), New CacheDependency(Context.Server.MapPath(_GeoIPFile)))
  End If

  'Check to see if the request is a localhost request
  'and see if the LocalhostCountryCode is specified
  If IsLocal Then
   Return Null.NullString
  End If

  'Either this is a remote request or it is a local
  'request with no LocalhostCountryCode specified
  Dim _CountryLookup As CountryLookup

  'Check to see if we are using the Cached
  'version of the GeoIPData file
  If _CacheGeoIPData Then
   'Yes, get it from cache
   _CountryLookup = New CountryLookup(DirectCast(Context.Cache.[Get]("GeoIPData"), IO.MemoryStream))
  Else
   'No, get it from file
   _CountryLookup = New CountryLookup(Context.Server.MapPath(_GeoIPFile))
  End If
  'Get the country code based on the IP address
  Dim country As String = Null.NullString
  Try
   country = _CountryLookup.LookupCountryName(IP)
  Catch ex As Exception
   Exceptions.LogException(ex)
  End Try
  Return country
 End Function
#End Region

End Class
