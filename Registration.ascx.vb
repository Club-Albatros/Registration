Imports DotNetNuke.Common
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Security.Membership
Imports DotNetNuke.Services.Authentication
Imports DotNetNuke.Web.UI.WebControls
Imports DotNetNuke.Entities.Host
Imports DotNetNuke.Entities.Profile
Imports DotNetNuke.Common.Lists
Imports DotNetNuke.UI.WebControls
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Entities.Users
Imports DotNetNuke.UI.Skins.Controls
Imports DotNetNuke.Security
Imports DotNetNuke.Entities.Users.Membership
Imports System.Globalization
Imports System.Linq
Imports DotNetNuke.Entities.Users.Internal
Imports DotNetNuke.Framework
Imports DotNetNuke.Web.Client.ClientResourceManagement
Imports DotNetNuke.Security.Permissions
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Modules.Actions

Partial Public Class Registration
 Inherits ModuleBase
 Implements IActionable

 Protected Const PasswordStrengthTextBoxCssClass As String = "password-strength"
 Protected Const ConfirmPasswordTextBoxCssClass As String = "password-confirm"

 Protected WithEvents userForm As DnnFormEditor

 Private ReadOnly _loginControls As New List(Of AuthenticationLoginBase)()
 Private AddedFields As New List(Of String)

#Region "Protected Properties"

 Protected Property AuthenticationType() As String
  Get
   Return ViewState.GetValue("AuthenticationType", Null.NullString)
  End Get
  Set(value As String)
   ViewState.SetValue("AuthenticationType", value, Null.NullString)
  End Set
 End Property

 Protected Property CreateStatus() As UserCreateStatus
  Get
   Return m_CreateStatus
  End Get
  Set(value As UserCreateStatus)
   m_CreateStatus = value
  End Set
 End Property
 Private m_CreateStatus As UserCreateStatus

 Protected ReadOnly Property DisplayNameFormat() As String
  Get
   Return GetSettingValue("Security_DisplayNameFormat")
  End Get
 End Property

 Protected ReadOnly Property EmailValidator() As String
  Get
   Return GetSettingValue("Security_EmailValidation")
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

 Protected ReadOnly Property IsValid() As Boolean
  Get
   Return Validate()
  End Get
 End Property

 Protected ReadOnly Property RandomPassword() As Boolean
  Get
   Return Convert.ToBoolean(GetSetting(PortalId, "Registration_RandomPassword"))
  End Get
 End Property

 Protected ReadOnly Property RedirectURL() As String
  Get
   Dim _RedirectURL As String = ""

   Dim setting As Object = GetSetting(PortalId, "Redirect_AfterRegistration")

   If Convert.ToInt32(setting) > 0 Then
    'redirect to after registration page
    _RedirectURL = DotNetNuke.Common.Globals.NavigateURL(Convert.ToInt32(setting))
   Else

    If Convert.ToInt32(setting) <= 0 Then
     If Request.QueryString("returnurl") IsNot Nothing Then
      'return to the url passed to register
      _RedirectURL = HttpUtility.UrlDecode(Request.QueryString("returnurl"))
      'redirect url should never contain a protocol ( if it does, it is likely a cross-site request forgery attempt )
      If _RedirectURL.Contains("://") AndAlso Not _RedirectURL.StartsWith(DotNetNuke.Common.Globals.AddHTTP(PortalSettings.PortalAlias.HTTPAlias), StringComparison.InvariantCultureIgnoreCase) Then
       _RedirectURL = ""
      End If
      If _RedirectURL.Contains("?returnurl") Then
       Dim baseURL As String = _RedirectURL.Substring(0, _RedirectURL.IndexOf("?returnurl", StringComparison.Ordinal))
       Dim returnURL As String = _RedirectURL.Substring(_RedirectURL.IndexOf("?returnurl", StringComparison.Ordinal) + 11)

       _RedirectURL = String.Concat(baseURL, "?returnurl", HttpUtility.UrlEncode(returnURL))
      End If
     End If
     If [String].IsNullOrEmpty(_RedirectURL) Then
      'redirect to current page 
      _RedirectURL = DotNetNuke.Common.Globals.NavigateURL()
     End If
    Else
     'redirect to after registration page
     _RedirectURL = DotNetNuke.Common.Globals.NavigateURL(Convert.ToInt32(setting))
    End If
   End If

   Return _RedirectURL
  End Get
 End Property


 Protected ReadOnly Property RegistrationFields() As String
  Get
   Return GetSettingValue("Registration_RegistrationFields")
  End Get
 End Property

 Protected ReadOnly Property RegistrationFormType() As Integer
  Get
   Return Convert.ToInt32(GetSettingValue("Registration_RegistrationFormType"))
  End Get
 End Property

 Protected ReadOnly Property RequirePasswordConfirm() As Boolean
  Get
   Return Convert.ToBoolean(GetSetting(PortalId, "Registration_RequireConfirmPassword"))
  End Get
 End Property

 Protected ReadOnly Property RequireUniqueDisplayName() As Boolean
  Get
   Return Convert.ToBoolean(GetSetting(PortalId, "Registration_RequireUniqueDisplayName"))
  End Get
 End Property

 Protected ReadOnly Property RequireValidProfile() As Boolean
  Get
   Return Convert.ToBoolean(GetSetting(PortalId, "Security_RequireValidProfile"))
  End Get
 End Property

 Protected ReadOnly Property UseAuthProviders() As Boolean
  Get
   Return Convert.ToBoolean(GetSetting(PortalId, "Registration_UseAuthProviders"))
  End Get
 End Property

 Protected ReadOnly Property UseCaptcha() As Boolean
  Get
   Return Convert.ToBoolean(GetSetting(PortalId, "Security_CaptchaRegister"))
  End Get
 End Property

 Protected ReadOnly Property UseEmailAsUserName() As Boolean
  Get
   Return Convert.ToBoolean(GetSetting(PortalId, "Registration_UseEmailAsUserName"))
  End Get
 End Property

 Protected ReadOnly Property UserNameValidator() As String
  Get
   Return GetSettingValue("Security_UserNameValidation")
  End Get
 End Property

 Protected ReadOnly Property UseProfanityFilter() As Boolean
  Get
   Return Convert.ToBoolean(GetSetting(PortalId, "Registration_UseProfanityFilter"))
  End Get
 End Property

 Protected Property UserToken() As String
  Get
   Return ViewState.GetValue("UserToken", String.Empty)
  End Get
  Set(value As String)
   ViewState.SetValue("UserToken", value, String.Empty)
  End Set
 End Property

#End Region

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
   userForm.Items.Add(formItem)
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
   userForm.Items.Add(formItem)
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
   userForm.Items.Add(formItem)
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
    userForm.Items.Add(formItem)
   Else
    section.Items.Add(formItem)
   End If

  End If

 End Sub

 Private Sub BindLoginControl(authLoginControl As AuthenticationLoginBase, authSystem As AuthenticationInfo)
  'set the control ID to the resource file name ( ie. controlname.ascx = controlname )
  'this is necessary for the Localization in PageBase
  authLoginControl.AuthenticationType = authSystem.AuthenticationType
  authLoginControl.ID = IO.Path.GetFileNameWithoutExtension(authSystem.LoginControlSrc) & "_" & Convert.ToString(authSystem.AuthenticationType)
  authLoginControl.LocalResourceFile = (Convert.ToString(authLoginControl.TemplateSourceDirectory) & "/") + Localization.LocalResourceDirectory & "/" & IO.Path.GetFileNameWithoutExtension(authSystem.LoginControlSrc)
  authLoginControl.RedirectURL = RedirectURL
  authLoginControl.ModuleConfiguration = ModuleConfiguration

  AddHandler authLoginControl.UserAuthenticated, AddressOf UserAuthenticated
 End Sub

 Private Sub CreateUser()
  'Update DisplayName to conform to Format
  UpdateDisplayName()

  User.Membership.Approved = PortalSettings.UserRegistration = CInt(DotNetNuke.Common.Globals.PortalRegistrationType.PublicRegistration)
  Dim user__1 As UserInfo = User
  CreateStatus = UserController.CreateUser(user__1)

  DataCache.ClearPortalCache(PortalId, True)

  Try
   If CreateStatus = UserCreateStatus.Success Then
    'hide the succesful captcha
    captchaRow.Visible = False

    'Assocate alternate Login with User and proceed with Login
    If Not [String].IsNullOrEmpty(AuthenticationType) Then
     AuthenticationController.AddUserAuthentication(User.UserID, AuthenticationType, UserToken)
    End If

    Dim strMessage As String = CompleteUserCreation(CreateStatus, user__1, True, IsRegister)

    If (String.IsNullOrEmpty(strMessage)) Then
     Response.Redirect(RedirectURL, True)
    End If
   Else
    AddLocalizedModuleMessage(UserController.GetUserCreateStatus(CreateStatus), ModuleMessage.ModuleMessageType.RedError, True)
   End If
  Catch exc As Exception
   'Module failed to load
   Exceptions.ProcessModuleLoadException(Me, exc)
  End Try
 End Sub

 Private Function GetSettingValue(key As String) As String
  Dim value As String = [String].Empty
  Dim setting As Object = GetSetting(UserPortalID, key)
  If (setting IsNot Nothing) AndAlso (Not [String].IsNullOrEmpty(Convert.ToString(setting))) Then
   value = Convert.ToString(setting)
  End If
  Return value

 End Function

 Private Sub UpdateDisplayName()
  'Update DisplayName to conform to Format
  Dim setting As Object = GetSetting(UserPortalID, "Security_DisplayNameFormat")
  If (setting IsNot Nothing) AndAlso (Not String.IsNullOrEmpty(Convert.ToString(setting))) Then
   User.UpdateDisplayName(Convert.ToString(setting))
  End If
 End Sub

 Private Function Validate() As Boolean
  CreateStatus = UserCreateStatus.AddUser
  Dim portalSecurity__1 As New PortalSecurity

  'Check User Editor
  Dim _IsValid As Boolean = userForm.IsValid

  If RegistrationFormType = 0 Then
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
  Else
   'Set Username to Email
   If [String].IsNullOrEmpty(User.Username) Then
    User.Username = User.Email
   End If

   'Set DisplayName
   If [String].IsNullOrEmpty(User.DisplayName) Then
    User.DisplayName = If([String].IsNullOrEmpty(User.FirstName + " " + User.LastName), User.Email.Substring(0, User.Email.IndexOf("@", StringComparison.Ordinal)), User.FirstName + " " + User.LastName)
   End If

   'Random Password
   If [String].IsNullOrEmpty(User.Membership.Password) Then
    'Generate a random password for the user
    User.Membership.Password = UserController.GeneratePassword()
   End If

   'Password Confirm
   If Not [String].IsNullOrEmpty(User.Membership.PasswordConfirm) Then
    If User.Membership.Password <> User.Membership.PasswordConfirm Then
     CreateStatus = UserCreateStatus.PasswordMismatch
    End If
   End If
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
     userName = User.Username + "0" & i.ToString(CultureInfo.InvariantCulture)
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
     displayName = User.DisplayName + " 0" & i.ToString(CultureInfo.InvariantCulture)
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

 Protected Overrides Sub OnInit(e As EventArgs)
  MyBase.OnInit(e)

  jQuery.RequestDnnPluginsRegistration()

  ClientResourceManager.RegisterScript(Page, "~/Resources/Shared/scripts/dnn.jquery.extensions.js")
  ClientResourceManager.RegisterScript(Page, "~/Resources/Shared/scripts/dnn.jquery.tooltip.js")
  ClientResourceManager.RegisterScript(Page, "~/DesktopModules/Admin/Security/Scripts/dnn.PasswordComparer.js")

  rpRoles.DataSource = Settings.RolesToShow.Values
  rpRoles.DataBind()

  Dim mainSection As New DnnFormSection
  mainSection.ResourceKey = "secLogin"
  userForm.Sections.Add(mainSection)

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

  Dim nameSection As New DnnFormSection
  nameSection.ResourceKey = "secName"
  userForm.Sections.Add(nameSection)

  'DisplayName
  If [String].IsNullOrEmpty(DisplayNameFormat) Then
   AddField(nameSection, "DisplayName", [String].Empty, True, [String].Empty, TextBoxMode.SingleLine)
  Else
   AddField(nameSection, "FirstName", [String].Empty, True, [String].Empty, TextBoxMode.SingleLine)
   AddField(nameSection, "LastName", [String].Empty, True, [String].Empty, TextBoxMode.SingleLine)
  End If

  'Email
  AddField(nameSection, "Email", [String].Empty, True, EmailValidator, TextBoxMode.SingleLine)

  If RequireValidProfile Then

   Dim otherSections As New Dictionary(Of String, DnnFormSection)
   For Each [property] As ProfilePropertyDefinition In User.Profile.ProfileProperties
    If [property].Required And Not AddedFields.Contains([property].PropertyName.ToLower) Then
     If Not otherSections.ContainsKey([property].PropertyCategory) Then
      otherSections.Add([property].PropertyCategory, New DnnFormSection With {.ResourceKey = [property].PropertyCategory})
     End If
    End If
   Next

   For Each [property] As ProfilePropertyDefinition In User.Profile.ProfileProperties
    If [property].Required And Not AddedFields.Contains([property].PropertyName.ToLower) Then
     AddProperty(otherSections([property].PropertyCategory), [property])
     AddedFields.Add([property].PropertyName.ToLower)
    End If
   Next

   For Each section As DnnFormSection In otherSections.Values
    userForm.Sections.Add(section)
   Next

  End If

  ' Dim fields As List(Of String) = RegistrationFields.Split(","c).ToList()

  'Verify that the current user has access to this page
  If PortalSettings.UserRegistration = CInt(DotNetNuke.Common.Globals.PortalRegistrationType.NoRegistration) AndAlso Request.IsAuthenticated = False Then
   Response.Redirect(DotNetNuke.Common.Globals.NavigateURL("Access Denied"), True)
  End If

  AddHandler cancelButton.Click, AddressOf cancelButton_Click
  AddHandler registerButton.Click, AddressOf registerButton_Click

  If UseAuthProviders Then
   Dim authSystems As List(Of AuthenticationInfo) = AuthenticationController.GetEnabledAuthenticationServices()
   For Each authSystem As AuthenticationInfo In authSystems
    Try
     Dim authLoginControl As AuthenticationLoginBase = DirectCast(LoadControl("~/" + authSystem.LoginControlSrc), AuthenticationLoginBase)
     If authSystem.AuthenticationType <> "DNN" Then
      BindLoginControl(authLoginControl, authSystem)
      'Check if AuthSystem is Enabled
      If authLoginControl.Enabled AndAlso authLoginControl.SupportsRegistration Then
       authLoginControl.Mode = AuthMode.Register

       'Add Login Control to List
       _loginControls.Add(authLoginControl)
      End If
     End If
    Catch ex As Exception
     Exceptions.LogException(ex)
    End Try
   Next
  End If
 End Sub

 Protected Overrides Sub OnLoad(e As EventArgs)
  MyBase.OnLoad(e)

  If Request.IsAuthenticated Then
   'if a Login Page has not been specified for the portal
   If DotNetNuke.Common.Globals.IsAdminControl() Then
    'redirect to current page 
    Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(), True)
   Else
    'make module container invisible if user is not a page admin
    If Not TabPermissionController.CanAdminPage() Then
     ContainerControl.Visible = False
    End If
   End If
  End If

  If UseCaptcha Then
   captchaRow.Visible = True
   ctlCaptcha.ErrorMessage = Localization.GetString("InvalidCaptcha", LocalResourceFile)
   ctlCaptcha.Text = Localization.GetString("CaptchaText", LocalResourceFile)
  End If

  If UseAuthProviders AndAlso [String].IsNullOrEmpty(AuthenticationType) Then
   For Each authLoginControl As AuthenticationLoginBase In _loginControls
    socialLoginControls.Controls.Add(authLoginControl)
   Next
  End If

  'Display relevant message
  userHelpLabel.Text = Localization.GetSystemMessage(PortalSettings, "MESSAGE_REGISTRATION_INSTRUCTIONS")
  Select Case PortalSettings.UserRegistration
   Case CInt(DotNetNuke.Common.Globals.PortalRegistrationType.PrivateRegistration)
    userHelpLabel.Text += Localization.GetString("PrivateMembership", Localization.SharedResourceFile)
    Exit Select
   Case CInt(DotNetNuke.Common.Globals.PortalRegistrationType.PublicRegistration)
    userHelpLabel.Text += Localization.GetString("PublicMembership", Localization.SharedResourceFile)
    Exit Select
   Case CInt(DotNetNuke.Common.Globals.PortalRegistrationType.VerifiedRegistration)
    userHelpLabel.Text += Localization.GetString("VerifiedMembership", Localization.SharedResourceFile)
    Exit Select
  End Select
  userHelpLabel.Text += Localization.GetString("Required", LocalResourceFile)
  userHelpLabel.Text += Localization.GetString("RegisterWarning", LocalResourceFile)

  userForm.DataSource = User
  If Not Page.IsPostBack Then
   userForm.DataBind()
  End If
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

 Private Sub cancelButton_Click(sender As Object, e As EventArgs)
  Response.Redirect(RedirectURL, True)
 End Sub

 Private Sub registerButton_Click(sender As Object, e As EventArgs)
  If (UseCaptcha AndAlso ctlCaptcha.IsValid) OrElse Not UseCaptcha Then
   If IsValid Then
    CreateUser()
   Else
    If CreateStatus <> UserCreateStatus.AddUser Then
     AddLocalizedModuleMessage(UserController.GetUserCreateStatus(CreateStatus), ModuleMessage.ModuleMessageType.RedError, True)
    End If
   End If
  End If
 End Sub

 Private Sub UserAuthenticated(sender As Object, e As UserAuthenticatedEventArgs)
  Dim profileProperties As NameValueCollection = e.Profile

  User.Username = e.UserToken
  AuthenticationType = e.AuthenticationType
  UserToken = e.UserToken

  For Each key As String In profileProperties
   Select Case key
    Case "FirstName"
     User.FirstName = profileProperties(key)
     Exit Select
    Case "LastName"
     User.LastName = profileProperties(key)
     Exit Select
    Case "Email"
     User.Email = profileProperties(key)
     Exit Select
    Case "DisplayName"
     User.DisplayName = profileProperties(key)
     Exit Select
    Case Else
     User.Profile.SetProfileProperty(key, profileProperties(key))
     Exit Select
   End Select
  Next

  'Generate a random password for the user
  User.Membership.Password = UserController.GeneratePassword()

  If Not [String].IsNullOrEmpty(User.Email) Then
   CreateUser()
  Else
   AddLocalizedModuleMessage(LocalizeString("NoEmail"), ModuleMessage.ModuleMessageType.RedError, True)
   For Each formItem As DnnFormItemBase In userForm.Items
    formItem.Visible = formItem.DataField = "Email"
   Next
   userForm.DataBind()
  End If
 End Sub

 Private Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load

  ' Me.LocalResourceFile = "DesktopModules\Admin\Security\App_LocalResources\Profile"

 End Sub

#Region " IActionable "
 Public ReadOnly Property ModuleActions As Actions.ModuleActionCollection Implements IActionable.ModuleActions
  Get
   Dim MyActions As New Actions.ModuleActionCollection
   If Security.CanEdit Then
    MyActions.Add(GetNextActionID, Localization.GetString("ChooseFields", LocalResourceFile), ModuleActionType.EditContent, "", "", EditUrl("Fields"), False, DotNetNuke.Security.SecurityAccessLevel.Edit, True, False)
   End If
   Return MyActions
  End Get
 End Property
#End Region

End Class
