Imports System
Imports System.Data
Imports System.Xml
Imports System.Xml.Schema
Imports System.Xml.Serialization

Imports DotNetNuke
Imports DotNetNuke.Common
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Services.Tokens

Imports Albatros.DNN.Modules.Registration.Data

Namespace Entities.RoleLocalizations

 Partial Public Class RoleLocalizationsController

  Public Shared Function GetRoleLocalizationsByRole(portalId As Integer, roleID As Int32) As List(Of RoleLocalizationInfo)

   Return DotNetNuke.Common.Utilities.CBO.FillCollection(Of RoleLocalizationInfo)(DataProvider.Instance().GetRoleLocalizations(portalId, roleID))

  End Function

  Public Shared Sub SetRoleLocalization(objRoleLocalization As RoleLocalizationInfo)

   ' DataProvider.Instance().UpdateRoleLocalization(objRoleLocalization.Description, objRoleLocalization.IconFile, objRoleLocalization.Locale, objRoleLocalization.RoleId, objRoleLocalization.RoleName)

  End Sub

 End Class
End Namespace

