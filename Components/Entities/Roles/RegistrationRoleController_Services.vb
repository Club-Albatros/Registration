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

Namespace Entities.Roles

 Partial Public Class RegistrationRoleController
  Inherits DnnApiController
  Implements IServiceRouteMapper

#Region " IServiceRouteMapper "
  Public Sub RegisterRoutes(mapRouteManager As DotNetNuke.Web.Api.IMapRoute) Implements DotNetNuke.Web.Api.IServiceRouteMapper.RegisterRoutes
   mapRouteManager.MapHttpRoute("Albatros/Registration", "Reorder", "Reorder", New With {.Controller = "RegistrationRole", .Action = "Reorder"}, Nothing, New String() {"Albatros.DNN.Modules.Registration.Entities.Roles"})
  End Sub
#End Region

#Region " Service Methods "
  Public Class orderDTO
   Public Property order As String
  End Class

  <HttpPost()>
  <DnnModuleAuthorize(AccessLevel:=DotNetNuke.Security.SecurityAccessLevel.Edit)>
  <ValidateAntiForgeryToken()>
  Public Function Reorder(postData As orderDTO) As HttpResponseMessage
   Dim newOrder As String = postData.order.Replace("role[]=", "")
   Data.DataProvider.Instance().ReorderRoles(ActiveModule.PortalID, newOrder)
   Return Request.CreateResponse(HttpStatusCode.OK, "")
  End Function
#End Region

 End Class
End Namespace
