Imports System
Imports DotNetNuke

Namespace Data

 Partial Public MustInherit Class DataProvider

  Public MustOverride Function GetProfileProperties(portalId As Int32, roles As String) As IDataReader
  Public MustOverride Function GetRolesByPortalId(portalId As Int32, locale As String) As IDataReader
  Public MustOverride Function GetRolesPropertiesByPortalId(portalId As Int32) As IDataReader
  Public MustOverride Function GetSiblingRegions(portalId As Int32, regionKey As String) As IDataReader
  Public MustOverride Sub SetRoleProperty(propertyDefinitionID As Int32, roleId As Int32, required As Boolean, remove As Boolean)

 End Class

End Namespace

