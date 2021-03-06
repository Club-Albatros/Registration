Imports System.Globalization
Imports System.IO
Imports System.Net
Imports System.Net.Http
Imports System.Runtime.Serialization
Imports System.Runtime.Serialization.Json
Imports System.Web.Http
Imports System.Web.Script.Serialization

Imports DotNetNuke.Web.Api
Imports DotNetNuke.Security.Permissions

Namespace Entities.RoleProperties

 Partial Public Class RolePropertiesController
  Inherits DnnApiController
  Implements IServiceRouteMapper

#Region " IServiceRouteMapper "
  Public Sub RegisterRoutes(mapRouteManager As DotNetNuke.Web.Api.IMapRoute) Implements DotNetNuke.Web.Api.IServiceRouteMapper.RegisterRoutes
   mapRouteManager.MapHttpRoute("Albatros/Registration", "GetRoleProperties", "Props/GetRoleProperties", New With {.Controller = "RoleProperties", .Action = "GetRoleProperties"}, Nothing, New String() {"Albatros.DNN.Modules.Registration.Entities.RoleProperties"})
   mapRouteManager.MapHttpRoute("Albatros/Registration", "SetRoleProperty", "Props/SetRoleProperty", New With {.Controller = "RoleProperties", .Action = "SetRoleProperty"}, Nothing, New String() {"Albatros.DNN.Modules.Registration.Entities.RoleProperties"})
  End Sub
#End Region

#Region " Service Methods "
  <HttpGet()>
  <DnnModuleAuthorize(AccessLevel:=DotNetNuke.Security.SecurityAccessLevel.Edit)>
  Public Function GetRoleProperties() As HttpResponseMessage
   Dim res As List(Of RolePropertyInfo) = GetRolesPropertiesByPortalId(ActiveModule.PortalID)
   Return Request.CreateResponse(HttpStatusCode.OK, res)
  End Function

  Public Class SetRolePropertyDTO
   Public Property propertyDefinitionID As Integer
   Public Property roleId As Integer
   Public Property state As String
  End Class

  <HttpPost()>
  <DnnModuleAuthorize(AccessLevel:=DotNetNuke.Security.SecurityAccessLevel.Edit)>
  Public Function SetRoleProperty(postData As SetRolePropertyDTO) As HttpResponseMessage
   Select Case postData.state.ToLower
    Case "selected"
     Data.DataProvider.Instance().SetRoleProperty(postData.propertyDefinitionID, postData.roleId, False, False)
    Case "required"
     Data.DataProvider.Instance().SetRoleProperty(postData.propertyDefinitionID, postData.roleId, True, False)
    Case Else
     Data.DataProvider.Instance().SetRoleProperty(postData.propertyDefinitionID, postData.roleId, False, True)
   End Select
   Return Request.CreateResponse(HttpStatusCode.OK, True)
  End Function
#End Region

 End Class
End Namespace
