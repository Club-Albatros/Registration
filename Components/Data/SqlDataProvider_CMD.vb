Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports Microsoft.ApplicationBlocks.Data

Namespace Data

 Partial Public Class SqlDataProvider

  Public Overrides Function GetProfileProperties(portalId As Int32, roles As String) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetProfileProperties", portalId, roles), IDataReader)
  End Function

  Public Overrides Function GetRolesByPortalId(portalId As Int32, locale As String) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetRolesByPortalId", portalId, locale), IDataReader)
  End Function

  Public Overrides Function GetRolesPropertiesByPortalId(portalId As Int32) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetRolesPropertiesByPortalId", portalId), IDataReader)
  End Function

  Public Overrides Function GetSiblingRegions(portalId As Int32, regionKey As String) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetSiblingRegions", portalId, regionKey), IDataReader)
  End Function

  Public Overrides Sub SetRoleProperty(propertyDefinitionID As Int32, roleId As Int32, required As Boolean, remove As Boolean)
   SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "SetRoleProperty", propertyDefinitionID, roleId, required, remove)
  End Sub


 End Class

End Namespace
