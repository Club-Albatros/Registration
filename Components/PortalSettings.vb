Imports DotNetNuke.Entities.Portals

Public Class PortalSettings

#Region " Properties "
 Private Property PortalId As Integer = -1
 Private Property Settings As Dictionary(Of String, String)
 Public Property Security As SecuritySettings
 Public Property Registration As RegistrationSettings
#End Region

#Region " Constructors "
 Public Sub New(portalId As Integer)

  _PortalId = portalId
  _Settings = PortalController.GetPortalSettingsDictionary(portalId)
  Security = New SecuritySettings(_Settings)
  Registration = New RegistrationSettings(_Settings)

 End Sub
#End Region

#Region " Public Members "
 Public Shared Function GetSettings(portalId As Integer) As PortalSettings

  Dim res As PortalSettings = Nothing
  Try
   res = CType(DotNetNuke.Common.Utilities.DataCache.GetCache(CacheKey(portalId)), PortalSettings)
  Catch ex As Exception
  End Try
  If res Is Nothing Then
   res = New PortalSettings(portalId)
   DotNetNuke.Common.Utilities.DataCache.SetCache(CacheKey(portalId), res)
  End If
  Return res

 End Function

 Public Shared Function CacheKey(portalId As Integer) As String
  Return String.Format("PortalSettings{0}", portalId)
 End Function
#End Region


End Class
