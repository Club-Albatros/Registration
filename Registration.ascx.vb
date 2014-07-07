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

 Protected Overrides Sub OnInit(e As EventArgs)
  MyBase.OnInit(e)

  jQuery.RequestDnnPluginsRegistration()

  ClientResourceManager.RegisterScript(Page, "~/Resources/Shared/scripts/dnn.jquery.extensions.js")
  ClientResourceManager.RegisterScript(Page, "~/Resources/Shared/scripts/dnn.jquery.tooltip.js")
  ClientResourceManager.RegisterScript(Page, "~/DesktopModules/Admin/Security/Scripts/dnn.PasswordComparer.js")

  rpRoles.DataSource = Settings.RolesToShow.Values
  rpRoles.DataBind()

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

  Dim roles As New List(Of String)
  If Me.IsPostBack Then ' todo
   For Each item As RepeaterItem In rpRoles.Items
    Dim chk As CheckBox = CType(item.FindControl("chkActive"), CheckBox)
    If chk.Checked Then
     Dim hid As HiddenField = CType(item.FindControl("hidRoleID"), HiddenField)
     roles.Add(hid.Value)
    End If
   Next
  End If
  If Not roles.Contains(PortalSettings.RegisteredRoleId.ToString) Then roles.Add(PortalSettings.RegisteredRoleId.ToString)
  profileForm.SelectedRoles = String.Join(";", roles)
  profileForm.DataBind()

 End Sub

 Private Sub cancelButton_Click(sender As Object, e As EventArgs)
  Response.Redirect(RedirectURL, True)
 End Sub

 Private Sub registerButton_Click(sender As Object, e As EventArgs)
  If (UseCaptcha AndAlso ctlCaptcha.IsValid) OrElse Not UseCaptcha Then
   If profileForm.IsValid Then
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

  profileForm.User.Username = e.UserToken
  AuthenticationType = e.AuthenticationType
  UserToken = e.UserToken

  For Each key As String In profileProperties
   Select Case key
    Case "FirstName"
     profileForm.User.FirstName = profileProperties(key)
     Exit Select
    Case "LastName"
     profileForm.User.LastName = profileProperties(key)
     Exit Select
    Case "Email"
     profileForm.User.Email = profileProperties(key)
     Exit Select
    Case "DisplayName"
     profileForm.User.DisplayName = profileProperties(key)
     Exit Select
    Case Else
     profileForm.User.Profile.SetProfileProperty(key, profileProperties(key))
     Exit Select
   End Select
  Next

  'Generate a random password for the user
  profileForm.User.Membership.Password = UserController.GeneratePassword()

  If Not [String].IsNullOrEmpty(profileForm.User.Email) Then
   CreateUser()
  Else
   AddLocalizedModuleMessage(LocalizeString("NoEmail"), ModuleMessage.ModuleMessageType.RedError, True)
   For Each formItem As DnnFormItemBase In profileForm.Items
    formItem.Visible = formItem.DataField = "Email"
   Next
   profileForm.DataBind()
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
