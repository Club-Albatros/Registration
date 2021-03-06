Imports System
Imports DotNetNuke

Namespace Data

 Partial Public MustInherit Class DataProvider

  Public MustOverride Function GetProfileProperties(portalId As Int32, roles As String) As IDataReader
  Public MustOverride Function GetRoleLocalizations(portalId As Int32, roleId As Int32) As IDataReader
  Public MustOverride Function GetRolesByPortalId(portalId As Int32, locale As String) As IDataReader
  Public MustOverride Function GetRolesPropertiesByPortalId(portalId As Int32) As IDataReader
  Public MustOverride Function GetSiblingRegions(portalId As Int32, regionKey As String) As IDataReader
  Public MustOverride Function ListProperties(portalId As Int32, propertyName As String, searchString As String) As IDataReader
  Public MustOverride Sub RemoveRoleLocalizations(roleId As Int32)
  Public MustOverride Sub ReorderRoles(portalId As Int32, roleOrder As String)
  Public MustOverride Sub SetRoleLocalization(description As String, iconFile As String, locale As String, presentation As String, roleId As Int32, roleName As String)
  Public MustOverride Sub SetRoleProperty(propertyDefinitionID As Int32, roleId As Int32, required As Boolean, remove As Boolean)
  Public MustOverride Sub UpdateRoleSetting(roleID As Int32, redirectTab As Int32)

 End Class

End Namespace

