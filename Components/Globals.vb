Imports DotNetNuke.Security.Roles

Public Class Globals

 Public Shared Function GetRoleDictionary(portalId As Integer, roles As String) As Dictionary(Of Integer, RoleInfo)
  Dim res As New Dictionary(Of Integer, DotNetNuke.Security.Roles.RoleInfo)
  If Not String.IsNullOrEmpty(roles) Then
   For Each roleId As String In roles.Split(";"c)
    Dim r As RoleInfo = (New RoleController).GetRole(Integer.Parse(roleId), portalId)
    res.Add(r.RoleID, r)
   Next
  End If
  Return res
 End Function

End Class
