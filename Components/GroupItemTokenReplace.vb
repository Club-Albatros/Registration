Imports DotNetNuke.Services.Tokens
Imports DotNetNuke.Security.Roles

Public Class GroupItemTokenReplace
 Inherits BaseCustomTokenReplace
 Public Sub New(groupInfo As RoleInfo)
  PropertySource("groupitem") = groupInfo
 End Sub
 Public Function ReplaceGroupItemTokens(source As String) As String
  Return MyBase.ReplaceTokens(source)
 End Function
End Class
