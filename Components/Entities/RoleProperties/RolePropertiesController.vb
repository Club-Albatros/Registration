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
Imports DotNetNuke.Entities.Profile

Namespace Entities.RoleProperties

 Partial Public Class RolePropertiesController

  Public Shared Function GetRolesPropertiesByPortalId(portalId As Int32) As List(Of RolePropertyInfo)
   Return CBO.FillCollection(Of RolePropertyInfo)(Data.DataProvider.Instance().GetRolesPropertiesByPortalId(portalId))
  End Function

  Public Shared Function GetProfileProperties(portalId As Int32, roles As String) As List(Of ProfilePropertyDefinition)
   Return CBO.FillCollection(Of ProfilePropertyDefinition)(Data.DataProvider.Instance().GetProfileProperties(portalId, roles))
  End Function

 End Class
End Namespace

