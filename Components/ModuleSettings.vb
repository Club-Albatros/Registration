Imports System.Linq
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Common.Utilities.DictionaryExtensions
Imports DotNetNuke.Security.Roles

Public Class ModuleSettings

#Region " Properties "
 Private Property ModuleId As Integer = -1
 Private Property PortalId As Integer = -1
 Private Property Settings As Hashtable

 Private _rolesToShow As String = ""
 Private _rolesToShowBuff As Dictionary(Of Integer, RoleInfo)
 Public Property RolesToShow() As Dictionary(Of Integer, RoleInfo)
  Get
   If _rolesToShowBuff Is Nothing Then
    _rolesToShowBuff = Globals.GetRoleDictionary(PortalId, _rolesToShow)
   End If
   Return _rolesToShowBuff
  End Get
  Set(ByVal value As Dictionary(Of Integer, RoleInfo))
   _rolesToShowBuff = value
   _rolesToShow = String.Join(";", _rolesToShowBuff.Select(Function(x) x.Value.RoleID.ToString).ToArray)
  End Set
 End Property
#End Region

#Region " Constructors "
 Public Sub New(moduleId As Integer, portalId As Integer)

  _ModuleId = moduleId
  _PortalId = portalId
  _Settings = (New DotNetNuke.Entities.Modules.ModuleController).GetModuleSettings(moduleId)
  _rolesToShow = _Settings.GetValue(Of String)("RolesToShow", _rolesToShow)

 End Sub
#End Region

#Region " Public Members "
 Public Sub SaveSettings()

  Dim objModules As New ModuleController
  objModules.UpdateModuleSetting(ModuleId, "RolesToShow", _rolesToShow)
  DotNetNuke.Common.Utilities.DataCache.SetCache(CacheKey(ModuleId), Me)

 End Sub

 Public Shared Function GetSettings(moduleId As Integer, portalId As Integer) As ModuleSettings

  Dim res As ModuleSettings = Nothing
  Try
   res = CType(DotNetNuke.Common.Utilities.DataCache.GetCache(CacheKey(moduleId)), ModuleSettings)
  Catch ex As Exception
  End Try
  If res Is Nothing Then
   res = New ModuleSettings(moduleId, portalId)
   DotNetNuke.Common.Utilities.DataCache.SetCache(CacheKey(moduleId), res)
  End If
  Return res

 End Function

 Public Shared Function CacheKey(moduleId As Integer) As String
  Return String.Format("SettingsModule{0}", moduleId)
 End Function
#End Region

End Class
