Imports DotNetNuke.Common.Utilities.DictionaryExtensions

Public Class RegistrationSettings

#Region " Properties "
 Public Property RandomPassword As Boolean = False
 Public Property RedirectAfterRegistration As Integer = -1
 Public Property RegistrationFields As String = ""
 Public Property ExcludeTerms As String = ""
 Public Property ExcludeTermsRegex As String = ""
 Public Property RegistrationFormType As Integer = -1
 Public Property RequirePasswordConfirm As Boolean = False
 Public Property RequireUniqueDisplayName As Boolean = False
 Public Property UseAuthProviders As Boolean = False
 Public Property UseEmailAsUserName As Boolean = False
 Public Property UseProfanityFilter As Boolean = False
#End Region

#Region " Constructor "
 Public Sub New(settings As Dictionary(Of String, String))
  RandomPassword = settings.GetValue(Of Boolean)("Registration_RandomPassword", RandomPassword)
  RedirectAfterRegistration = settings.GetValue(Of Integer)("Redirect_AfterRegistration", RedirectAfterRegistration)
  RegistrationFields = settings.GetValue(Of String)("Registration_RegistrationFields", RegistrationFields)
  ExcludeTerms = settings.GetValue(Of String)("Registration_ExcludeTerms", ExcludeTerms)
  RegistrationFormType = settings.GetValue(Of Integer)("Registration_RegistrationFormType", RegistrationFormType)
  RequirePasswordConfirm = settings.GetValue(Of Boolean)("Registration_RequireConfirmPassword", RequirePasswordConfirm)
  RequireUniqueDisplayName = settings.GetValue(Of Boolean)("Registration_RequireUniqueDisplayName", RequireUniqueDisplayName)
  UseAuthProviders = settings.GetValue(Of Boolean)("Registration_UseAuthProviders", UseAuthProviders)
  UseEmailAsUserName = settings.GetValue(Of Boolean)("Registration_UseEmailAsUserName", UseEmailAsUserName)
  UseProfanityFilter = settings.GetValue(Of Boolean)("Registration_UseProfanityFilter", UseProfanityFilter)

  ExcludeTermsRegex = "^(?:(?!" & ExcludeTerms.Replace(" ", "").Replace(",", "|") & ").)*$\r?\n?"

 End Sub
#End Region

End Class
