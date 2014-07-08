Imports System.Globalization
Imports System.IO
Imports System.Linq
Imports System.Net
Imports System.Net.Http
Imports System.Runtime.Serialization
Imports System.Runtime.Serialization.Json
Imports System.Web.Http
Imports System.Web.Script.Serialization

Imports DotNetNuke.Web.Api
Imports DotNetNuke.Security.Permissions
Imports DotNetNuke.Common.Lists

Public Class CountryRegionsController
 Inherits DnnApiController
 Implements IServiceRouteMapper

#Region " IServiceRouteMapper "
 Public Sub RegisterRoutes(mapRouteManager As DotNetNuke.Web.Api.IMapRoute) Implements DotNetNuke.Web.Api.IServiceRouteMapper.RegisterRoutes
  mapRouteManager.MapHttpRoute("Albatros/Registration", "ListCountries", "Countries", New With {.Controller = "CountryRegions", .Action = "ListCountries"}, Nothing, New String() {"Albatros.DNN.Modules.Registration"})
  mapRouteManager.MapHttpRoute("Albatros/Registration", "ListRegions", "Country/{country}/Regions", New With {.Controller = "CountryRegions", .Action = "ListRegions"}, New With {.country = "\w*"}, New String() {"Albatros.DNN.Modules.Registration"})
  mapRouteManager.MapHttpRoute("Albatros/Registration", "ListSiblingRegions", "Region/{region}/Siblings", New With {.Controller = "CountryRegions", .Action = "ListSiblingRegions"}, Nothing, New String() {"Albatros.DNN.Modules.Registration"})
 End Sub
#End Region

#Region " Service Methods "
 <HttpGet()>
 <DnnModuleAuthorize(AccessLevel:=DotNetNuke.Security.SecurityAccessLevel.View)>
 Public Function ListCountries() As HttpResponseMessage
  Dim searchString As String = HttpContext.Current.Request.Params("SearchString").NormalizeString
  Dim countries As CachedCountryList = CachedCountryList.GetCountryList(Threading.Thread.CurrentThread.CurrentCulture.Name)
  Return Request.CreateResponse(HttpStatusCode.OK, countries.Values.Where(Function(x) x.NormalizedFullName.IndexOf(searchString) > -1).OrderBy(Function(x) x.NormalizedFullName))
 End Function

 Public Structure Region
  Public Text As String
  Public Value As String
 End Structure

 <HttpGet()>
 <DnnModuleAuthorize(AccessLevel:=DotNetNuke.Security.SecurityAccessLevel.View)>
 Public Function ListRegions(country As String) As HttpResponseMessage
  Dim res As New List(Of Region)
  For Each r As ListEntryInfo In (New ListController).GetListEntryInfoItems("Region", "Country." & country, ActiveModule.PortalID)
   res.Add(New Region With {.Text = r.Text, .Value = r.Value})
  Next
  Return Request.CreateResponse(HttpStatusCode.OK, res)
 End Function

 <HttpGet()>
 <DnnModuleAuthorize(AccessLevel:=DotNetNuke.Security.SecurityAccessLevel.View)>
 Public Function ListSiblingRegions(region As String) As HttpResponseMessage
  Dim res As New List(Of Region)
  For Each r As ListEntryInfo In GetSiblingRegions(ActiveModule.PortalID, region)
   res.Add(New Region With {.Text = r.Text, .Value = r.Value})
  Next
  Return Request.CreateResponse(HttpStatusCode.OK, res)
 End Function
#End Region

#Region " Other Methods "
 Public Enum ListType
  Country
  Region
 End Enum

 Public Shared Function GetLocalizedString(type As ListType, input As String, locale As String) As String
  Dim res As String = input
  Select Case type
   Case ListType.Region
    res = DotNetNuke.Services.Localization.Localization.GetString(input, "~/App_GlobalResources/Region.resx", locale)
    If String.IsNullOrEmpty(res) Then
     Dim lei As ListEntryInfo = (New ListController).GetListEntryInfo("Region", input)
     If lei IsNot Nothing Then
      res = lei.Value
     Else
      res = input
     End If
    End If
   Case Else
    Dim countries As CachedCountryList = CachedCountryList.GetCountryList(locale)
    If countries.ContainsKey(input) Then
     res = countries(input).Name
    End If
  End Select
  Return res
 End Function

 Public Shared Function GetSiblingRegions(portalId As Integer, region As String) As List(Of ListEntryInfo)
  Return DotNetNuke.Common.Utilities.CBO.FillCollection(Of ListEntryInfo)(Data.DataProvider.Instance().GetSiblingRegions(portalId, region))
 End Function
#End Region

End Class
