Imports System.Linq
Imports DotNetNuke.Security.Roles
Imports DotNetNuke.Common.Utilities
Imports Albatros.DNN.Modules.Registration.Entities.Roles

Public Class Globals

 Public Const RoleDictionaryCacheKey As String = "RoleDictionary:{0}-{1}"

 Public Shared Function GetRoleDictionary(portalId As Integer, roles As String, locale As String) As Dictionary(Of Integer, RegistrationRoleInfo)
  Dim res As New Dictionary(Of Integer, RegistrationRoleInfo)
  If Not String.IsNullOrEmpty(roles) Then
   Dim pRoles As Dictionary(Of Integer, RegistrationRoleInfo) = GetRoles(portalId, locale)
   For Each roleId As String In roles.Split(";"c)
    If pRoles.ContainsKey(Integer.Parse(roleId)) Then
     res.Add(Integer.Parse(roleId), pRoles(Integer.Parse(roleId)))
    End If
   Next
  End If
  Return res
 End Function

 Public Shared Function GetRoles(portalId As Integer, locale As String) As Dictionary(Of Integer, RegistrationRoleInfo)
  Dim cacheKey As String = [String].Format(RoleDictionaryCacheKey, portalId, locale)
  Return CBO.GetCachedObject(Of Dictionary(Of Integer, RegistrationRoleInfo))(New CacheItemArgs(cacheKey, DataCache.RolesCacheTimeOut, DataCache.RolesCachePriority), Function(c) GetRolesInternal(portalId, locale))
 End Function

 Public Shared Sub ClearRolesCache(portalId As Integer)
  For Each l As DotNetNuke.Services.Localization.Locale In DotNetNuke.Services.Localization.LocaleController.Instance.GetLocales(portalId).Values
   Dim cacheKey As String = [String].Format(RoleDictionaryCacheKey, portalId, l.Code)
   DotNetNuke.Common.Utilities.DataCache.ClearCache(cacheKey)
  Next
 End Sub

 Private Shared Function GetRolesInternal(portalId As Integer, locale As String) As Dictionary(Of Integer, RegistrationRoleInfo)
  Return CBO.FillDictionary(Of Integer, RegistrationRoleInfo)("RoleID", Data.DataProvider.Instance().GetRolesByPortalId(portalId, locale))
 End Function

End Class
