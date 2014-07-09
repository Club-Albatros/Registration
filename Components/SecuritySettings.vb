Imports DotNetNuke.Common.Utilities.DictionaryExtensions

Public Class SecuritySettings

#Region " Properties "
 Public Property RequireValidProfile As Boolean = False
 Public Property UseCaptcha As Boolean = False
 Public Property UserNameValidator As String = ""
 Public Property DisplayNameFormat As String = ""
 Public Property EmailValidator As String = ""
#End Region

#Region " Constructor "
 Public Sub New(settings As Dictionary(Of String, String))
  RequireValidProfile = settings.GetValue(Of Boolean)("Security_RequireValidProfile", RequireValidProfile)
  UseCaptcha = settings.GetValue(Of Boolean)("Security_CaptchaRegister", UseCaptcha)
  UserNameValidator = settings.GetValue(Of String)("Security_UserNameValidation", UserNameValidator)
  DisplayNameFormat = settings.GetValue(Of String)("Security_DisplayNameFormat", DisplayNameFormat)
  EmailValidator = settings.GetValue(Of String)("Security_EmailValidation", EmailValidator)
 End Sub
#End Region

End Class
