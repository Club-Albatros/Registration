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
Imports DotNetNuke.Services.Social.Notifications
Imports ClientDependency.Core.StringExtensions

Partial Public Class Registration
 Inherits ModuleBase
 Implements IActionable

 Protected Const PasswordStrengthTextBoxCssClass As String = "password-strength"
 Protected Const ConfirmPasswordTextBoxCssClass As String = "password-confirm"

 Private ReadOnly _loginControls As New List(Of AuthenticationLoginBase)()
 Private AddedFields As New List(Of String)

#Region "Protected Properties"
 Private ReadOnly Property VerificationKey As String
  Get
   Return (Session.SessionID & DotNetNuke.Entities.Host.Host.GUID.ToString).GenerateMd5
  End Get
 End Property

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

 Protected Property RedirectTabId As Integer = -1

 Protected ReadOnly Property HomeURL As String
  Get
   Return DotNetNuke.Common.NavigateURL(PortalSettings.HomeTabId)
  End Get
 End Property

 Protected ReadOnly Property RedirectURL() As String
  Get
   Dim _RedirectURL As String = ""

   'Dim setting As Object = GetSetting(PortalId, "Redirect_AfterRegistration")

   If PSettings.Registration.RedirectAfterRegistration > 0 Then
    'redirect to after registration page
    _RedirectURL = DotNetNuke.Common.Globals.NavigateURL(PSettings.Registration.RedirectAfterRegistration)
   Else

    If PSettings.Registration.RedirectAfterRegistration <= 0 Then
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
      _RedirectURL = HomeURL
     End If
    Else
     'redirect to after registration page
     _RedirectURL = DotNetNuke.Common.Globals.NavigateURL(PSettings.Registration.RedirectAfterRegistration)
    End If
   End If

   Return _RedirectURL
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

 Private Sub CreateOrUpdateUser(user As UserInfo)

  If user.UserID = -1 Then ' Create User

   If Not String.IsNullOrEmpty(PSettings.Security.DisplayNameFormat) Then
    user.UpdateDisplayName(PSettings.Security.DisplayNameFormat)
   End If

   user.Membership.Approved = PortalSettings.UserRegistration = CInt(DotNetNuke.Common.Globals.PortalRegistrationType.PublicRegistration)
   CreateStatus = UserController.CreateUser(user)

   DataCache.ClearPortalCache(PortalId, True)

   Try
    If CreateStatus = UserCreateStatus.Success Then
     'hide the succesful captcha
     captchaRow.Visible = False

     'Assocate alternate Login with User and proceed with Login
     If Not [String].IsNullOrEmpty(AuthenticationType) Then
      AuthenticationController.AddUserAuthentication(user.UserID, AuthenticationType, UserToken)
     End If

     Dim strMessage As String = CompleteUserCreation(CreateStatus, user, True)
     AddUserToSelectedRoles(user)

     If (String.IsNullOrEmpty(strMessage)) Then
      Response.Redirect(RedirectURL, False)
     End If
    Else
     DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, UserController.GetUserCreateStatus(CreateStatus), ModuleMessage.ModuleMessageType.RedError)
    End If
   Catch exc As Exception
    'Module failed to load
    Exceptions.ProcessModuleLoadException(Me, exc)
   End Try

  Else ' Update User

   UserController.UpdateUser(PortalId, user)
   AddUserToSelectedRoles(user)

  End If

 End Sub

 Protected Overrides Sub OnInit(e As EventArgs)
  MyBase.OnInit(e)

  jQuery.RequestDnnPluginsRegistration()

  ClientResourceManager.RegisterScript(Page, "~/Resources/Shared/scripts/dnn.jquery.extensions.js")
  ClientResourceManager.RegisterScript(Page, "~/Resources/Shared/scripts/dnn.jquery.tooltip.js")
  ClientResourceManager.RegisterScript(Page, "~/DesktopModules/Admin/Security/Scripts/dnn.PasswordComparer.js")

  rpRoles.DataSource = Settings.RolesToShow.Values.OrderBy(Function(r) r.ViewOrder)
  rpRoles.DataBind()

  pnlRoles.Visible = False
  If Me.UserInfo IsNot Nothing And Not Me.IsPostBack Then
   Dim userRoles As List(Of UserRoleInfo) = CType((New Roles.RoleController).GetUserRoles(UserInfo, True), Global.System.Collections.Generic.List(Of Global.DotNetNuke.Entities.Users.UserRoleInfo))
   For Each item As RepeaterItem In rpRoles.Items
    Dim hid As HiddenField = CType(item.FindControl("hidRoleID"), HiddenField)
    If userRoles.Where(Function(x) x.RoleID.ToString = hid.Value).Count > 0 Then
     Dim chk As CheckBox = CType(item.FindControl("chkActive"), CheckBox)
     chk.Checked = True
    End If
    pnlRoles.Visible = True
   Next
  End If

  pnlSocialLogin.Visible = False
  If UserInfo.UserID = -1 Then
   If PSettings.Registration.UseAuthProviders Then
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
      pnlSocialLogin.Visible = True
     Catch ex As Exception
      Exceptions.LogException(ex)
     End Try
    Next
   End If
   registerButton.Visible = True
   cmdUpdate.Visible = False
  Else
   registerButton.Visible = False
   cmdUpdate.Visible = True
  End If

 End Sub

 Protected Overrides Sub OnLoad(e As EventArgs)
  MyBase.OnLoad(e)

  If PSettings.Security.UseCaptcha Then
   captchaRow.Visible = True
   ctlCaptcha.ErrorMessage = Localization.GetString("InvalidCaptcha", LocalResourceFile)
   ctlCaptcha.Text = Localization.GetString("CaptchaText", LocalResourceFile)
  End If

  If PSettings.Registration.UseAuthProviders AndAlso [String].IsNullOrEmpty(AuthenticationType) Then
   For Each authLoginControl As AuthenticationLoginBase In _loginControls
    socialLoginControls.Controls.Add(authLoginControl)
   Next
  End If

  Dim roles As New List(Of String)
  For Each item As RepeaterItem In rpRoles.Items
   Dim chk As CheckBox = CType(item.FindControl("chkActive"), CheckBox)
   If chk.Checked Then
    Dim hid As HiddenField = CType(item.FindControl("hidRoleID"), HiddenField)
    roles.Add(hid.Value)
   End If
  Next
  If Not roles.Contains(PortalSettings.RegisteredRoleId.ToString) Then roles.Add(PortalSettings.RegisteredRoleId.ToString)
  profileForm.SelectedRoles = String.Join(";", roles)
  profileForm.DataBind()

  hidVerToken.Value = VerificationKey

 End Sub

 Private Sub cancelButton_Click(sender As Object, e As EventArgs) Handles cancelButton.Click
  Response.Redirect(HomeURL, False)
 End Sub

 Private Sub registerButton_Click(sender As Object, e As EventArgs) Handles registerButton.Click
  RegisterOrUpdateUser()
  If RedirectTabId <> -1 Then
   Response.Redirect(DotNetNuke.Common.NavigateURL(RedirectTabId), False)
  Else
   Response.Redirect(RedirectURL, False)
  End If
 End Sub

 Private Sub cmdUpdate_Click(sender As Object, e As System.EventArgs) Handles cmdUpdate.Click
  RegisterOrUpdateUser()
 End Sub

 Private Sub UserAuthenticated(sender As Object, e As UserAuthenticatedEventArgs)

  Dim User As UserInfo = e.User

  Select Case e.LoginStatus

   Case UserLoginStatus.LOGIN_FAILURE ' we need to create the user

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
     CreateOrUpdateUser(User)
    Else
     DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, LocalizeString("NoEmail"), ModuleMessage.ModuleMessageType.RedError)
     For Each formItem As DnnFormItemBase In profileForm.Items
      formItem.Visible = formItem.DataField = "Email"
     Next
    End If

   Case UserLoginStatus.LOGIN_USERLOCKEDOUT
    DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, String.Format(Localization.GetString("UserLockedOut", LocalResourceFile), Host.AutoAccountUnlockDuration), ModuleMessage.ModuleMessageType.RedError)
   Case UserLoginStatus.LOGIN_USERNOTAPPROVED
    DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, LocalizeString(e.Message), ModuleMessage.ModuleMessageType.RedError)
   Case Else

    ' user is getting logged in with an existing account

  End Select

  profileForm.User = User
  profileForm.DataBind()

 End Sub

#Region " IActionable "
 Public ReadOnly Property ModuleActions As Actions.ModuleActionCollection Implements IActionable.ModuleActions
  Get
   Dim MyActions As New Actions.ModuleActionCollection
   If Security.CanEdit Then
    MyActions.Add(GetNextActionID, Localization.GetString("ChooseFields", LocalResourceFile), ModuleActionType.EditContent, "", "", EditUrl("Fields"), False, DotNetNuke.Security.SecurityAccessLevel.Edit, True, False)
    MyActions.Add(GetNextActionID, Localization.GetString("RoleEditor", LocalResourceFile), ModuleActionType.EditContent, "", "", EditUrl("Roles"), False, DotNetNuke.Security.SecurityAccessLevel.Edit, True, False)
   End If
   Return MyActions
  End Get
 End Property
#End Region

#Region " Private Methods "
 Private Sub RegisterOrUpdateUser()
  If hidVerToken.Value <> VerificationKey Then Exit Sub
  If (PSettings.Security.UseCaptcha AndAlso ctlCaptcha.IsValid) OrElse Not PSettings.Security.UseCaptcha Then
   If profileForm.IsValid Then
    CreateOrUpdateUser(profileForm.User)
   Else
    If CreateStatus <> UserCreateStatus.AddUser Then
     DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, UserController.GetUserCreateStatus(CreateStatus), ModuleMessage.ModuleMessageType.RedError)
    End If
   End If
  End If
 End Sub

 Private Sub AddUserToSelectedRoles(user As UserInfo)

  For Each item As RepeaterItem In rpRoles.Items
   Dim chk As CheckBox = CType(item.FindControl("chkActive"), CheckBox)
   Dim hid As HiddenField = CType(item.FindControl("hidRoleID"), HiddenField)
   Dim roleId As Integer = Integer.Parse(hid.Value)
   Dim role As Roles.RoleInfo = (New Roles.RoleController).GetRole(roleId, PortalSettings.PortalId)
   If chk.Checked Then
    JoinGroup(user, role)
    If RedirectTabId = -1 AndAlso Settings.RolesToShow(role.RoleID).RedirectTab <> -1 Then
     RedirectTabId = Settings.RolesToShow(role.RoleID).RedirectTab
    End If
   Else
    Roles.RoleController.DeleteUserRole(UserInfo, role, PortalSettings, False)
   End If
  Next

 End Sub

 Private Const MemberPendingNotification As String = "GroupMemberPendingNotification"
 Private Const GroupModuleSharedResourcesPath As String = "~/DesktopModules/SocialGroups/App_LocalResources/SharedResources.resx"

 Private Sub JoinGroup(user As UserInfo, role As Roles.RoleInfo)
  Try
   If user.UserID >= 0 AndAlso role.RoleID > 0 Then
    Dim roleController As New Roles.RoleController
    If role IsNot Nothing Then

     Dim requireApproval As Boolean = False

     If role.Settings.ContainsKey("ReviewMembers") Then
      requireApproval = Convert.ToBoolean(role.Settings("ReviewMembers"))
     End If

     If role.IsPublic AndAlso Not requireApproval Then
      roleController.AddUserRole(PortalSettings.PortalId, UserInfo.UserID, role.RoleID, Null.NullDate)
      roleController.UpdateRole(role)
     End If

     If role.IsPublic AndAlso requireApproval Then
      roleController.AddUserRole(PortalSettings.PortalId, UserInfo.UserID, role.RoleID, Roles.RoleStatus.Pending, False, Null.NullDate, Null.NullDate)
      AddGroupOwnerNotification(MemberPendingNotification, TabId, ModuleId, role, UserInfo)
     End If
    End If
   End If
  Catch exc As Exception
  End Try
 End Sub

 Friend Overridable Function AddGroupOwnerNotification(notificationTypeName As String, tabId As Integer, moduleId As Integer, group As Roles.RoleInfo, initiatingUser As UserInfo) As Notification
  Dim notificationType As NotificationType = NotificationsController.Instance.GetNotificationType(notificationTypeName)

  Dim tokenReplace As New GroupItemTokenReplace(group)

  Dim subject As String = Localization.GetString(notificationTypeName & ".Subject", GroupModuleSharedResourcesPath)
  Dim body As String = Localization.GetString(notificationTypeName & ".Body", GroupModuleSharedResourcesPath)
  subject = subject.Replace("[DisplayName]", initiatingUser.DisplayName)
  subject = subject.Replace("[ProfileUrl]", DotNetNuke.Common.Globals.UserProfileURL(initiatingUser.UserID))
  subject = tokenReplace.ReplaceGroupItemTokens(subject)
  body = body.Replace("[DisplayName]", initiatingUser.DisplayName)
  body = body.Replace("[ProfileUrl]", DotNetNuke.Common.Globals.UserProfileURL(initiatingUser.UserID))
  body = tokenReplace.ReplaceGroupItemTokens(body)
  Dim roleCreator As UserInfo = UserController.GetUserById(group.PortalID, group.CreatedByUserID)
  Dim roleOwners As New List(Of UserInfo)
  Dim rc As New Roles.RoleController
  For Each userInfo As UserInfo In rc.GetUsersByRoleName(group.PortalID, group.RoleName)
   Dim userRoleInfo As DotNetNuke.Entities.Users.UserRoleInfo = rc.GetUserRole(group.PortalID, userInfo.UserID, group.RoleID)
   If userRoleInfo.IsOwner AndAlso userRoleInfo.UserID <> group.CreatedByUserID Then
    roleOwners.Add(UserController.GetUserById(group.PortalID, userRoleInfo.UserID))
   End If
  Next
  roleOwners.Add(roleCreator)

  'Need to add from sender details
  Dim notification As New Notification() With {
    .NotificationTypeID = notificationType.NotificationTypeId,
    .Subject = subject,
    .Body = body,
    .IncludeDismissAction = True,
    .SenderUserID = initiatingUser.UserID,
    .Context = [String].Format("{0}:{1}:{2}:{3}", tabId, moduleId, group.RoleID, initiatingUser.UserID)
  }
  NotificationsController.Instance.SendNotification(notification, initiatingUser.PortalID, Nothing, roleOwners)

  Return notification
 End Function

 Private Function CompleteUserCreation(createStatus As UserCreateStatus, newUser As UserInfo, notify As Boolean) As String

  Dim strMessage As String = ""
  Dim message As ModuleMessage.ModuleMessageType = ModuleMessage.ModuleMessageType.RedError

  Dim register As Boolean = Not Request.IsAuthenticated

  If register Then
   'send notification to portal administrator of new user registration
   'check the receive notification setting first, but if register type is Private, we will always send the notification email.
   'because the user need administrators to do the approve action so that he can continue use the website.
   If PortalSettings.EnableRegisterNotification OrElse PortalSettings.UserRegistration = CInt(DotNetNuke.Common.Globals.PortalRegistrationType.PrivateRegistration) Then
    strMessage += DotNetNuke.Services.Mail.Mail.SendMail(newUser, DotNetNuke.Services.Mail.MessageType.UserRegistrationAdmin, PortalSettings)
    SendAdminNotification(newUser, "NewUserRegistration")
   End If

   Dim loginStatus As UserLoginStatus = UserLoginStatus.LOGIN_FAILURE

   'complete registration
   Select Case PortalSettings.UserRegistration
    Case CInt(DotNetNuke.Common.Globals.PortalRegistrationType.PrivateRegistration)
     strMessage += DotNetNuke.Services.Mail.Mail.SendMail(newUser, DotNetNuke.Services.Mail.MessageType.UserRegistrationPrivate, PortalSettings)

     'show a message that a portal administrator has to verify the user credentials
     If String.IsNullOrEmpty(strMessage) Then
      strMessage += String.Format(Localization.GetString("PrivateConfirmationMessage", Localization.SharedResourceFile), newUser.Email)
      message = ModuleMessage.ModuleMessageType.GreenSuccess
     End If
     Exit Select
    Case CInt(DotNetNuke.Common.Globals.PortalRegistrationType.PublicRegistration)
     DotNetNuke.Services.Mail.Mail.SendMail(newUser, DotNetNuke.Services.Mail.MessageType.UserRegistrationPublic, PortalSettings)
     UserController.UserLogin(PortalSettings.PortalId, newUser.Username, newUser.Membership.Password, "", PortalSettings.PortalName, "", loginStatus, False)
     Exit Select
    Case CInt(DotNetNuke.Common.Globals.PortalRegistrationType.VerifiedRegistration)
     DotNetNuke.Services.Mail.Mail.SendMail(newUser, DotNetNuke.Services.Mail.MessageType.UserRegistrationVerified, PortalSettings)
     UserController.UserLogin(PortalSettings.PortalId, newUser.Username, newUser.Membership.Password, "", PortalSettings.PortalName, "", loginStatus, False)
     Exit Select
   End Select
   'affiliate
   If Not Null.IsNull(newUser.AffiliateID) Then
    Dim objAffiliates As New DotNetNuke.Services.Vendors.AffiliateController()
    objAffiliates.UpdateAffiliateStats(newUser.AffiliateID, 0, 1)
   End If
   'store preferredlocale in cookie
   Localization.SetLanguage(newUser.Profile.PreferredLocale)
   If register AndAlso message = ModuleMessage.ModuleMessageType.RedError Then
    DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, String.Format(Localization.GetString("SendMail.Error", Localization.SharedResourceFile), strMessage), message)
   Else
    DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, strMessage, message)
   End If
  Else
   If notify Then
    'Send Notification to User
    If PortalSettings.UserRegistration = CInt(DotNetNuke.Common.Globals.PortalRegistrationType.VerifiedRegistration) Then
     strMessage += DotNetNuke.Services.Mail.Mail.SendMail(newUser, DotNetNuke.Services.Mail.MessageType.UserRegistrationVerified, PortalSettings)
    Else
     strMessage += DotNetNuke.Services.Mail.Mail.SendMail(newUser, DotNetNuke.Services.Mail.MessageType.UserRegistrationPublic, PortalSettings)
    End If
   End If
  End If

  Return strMessage
 End Function

 Private Sub SendAdminNotification(newUser As UserInfo, notificationType As String)
  Dim notification As New DotNetNuke.Services.Social.Notifications.Notification() With {.NotificationTypeID = DotNetNuke.Services.Social.Notifications.NotificationsController.Instance.GetNotificationType(notificationType).NotificationTypeId, .IncludeDismissAction = True, .SenderUserID = PortalSettings.AdministratorId}
  notification.Subject = GetNotificationSubject(notificationType, newUser.Profile.PreferredLocale, newUser)
  notification.Body = GetNotificationBody(notificationType, newUser.Profile.PreferredLocale, newUser)
  Dim adminrole As Roles.RoleInfo = (New Roles.RoleController).GetRole(PortalSettings.AdministratorRoleId, PortalSettings.PortalId)
  Dim roles As New List(Of Roles.RoleInfo)
  roles.Add(adminrole)
  DotNetNuke.Services.Social.Notifications.NotificationsController.Instance.SendNotification(notification, PortalSettings.PortalId, roles, New List(Of UserInfo)())
 End Sub

 Private Function GetNotificationBody(notificationType As String, locale As String, newUser As UserInfo) As String
  Dim text As String = ""
  Select Case notificationType
   Case "NewUserRegistration"
    text = "EMAIL_USER_REGISTRATION_ADMINISTRATOR_BODY"
    Exit Select
  End Select
  Return LocalizeNotificationText(text, locale, newUser)
 End Function

 Private Function LocalizeNotificationText(text As String, locale As String, user As UserInfo) As String
  'This method could need a custom ArrayList in future notification types. Currently it is null
  Return Localization.GetSystemMessage(locale, PortalSettings, text, user, Localization.GlobalResourceFile, Nothing, "", PortalSettings.AdministratorId)
 End Function

 Private Function GetNotificationSubject(notificationType As String, locale As String, newUser As UserInfo) As String
  Dim text As String = ""
  Select Case notificationType
   Case "NewUserRegistration"
    text = "EMAIL_USER_REGISTRATION_ADMINISTRATOR_SUBJECT"
    Exit Select
  End Select
  Return LocalizeNotificationText(text, locale, newUser)
 End Function
#End Region

End Class
