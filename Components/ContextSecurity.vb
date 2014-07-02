Imports DotNetNuke.Entities.Users
Imports DotNetNuke.Security.Permissions
Imports DotNetNuke.Entities.Modules

Public Class ContextSecurity

 Public Property CanEdit As Boolean = False

#Region " Constructor "
 Public Sub New(objModule As ModuleInfo, user As UserInfo)
  CanEdit = ModulePermissionController.HasModulePermission(objModule.ModulePermissions, "EDIT")
 End Sub
#End Region

End Class
