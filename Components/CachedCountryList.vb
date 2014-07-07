Imports DotNetNuke.Common.Lists

Public Class CachedCountryList
 Inherits Dictionary(Of String, Country)

#Region " Structures "
 Public Structure Country
  Public Name As String
  Public Code As String
  Public FullName As String
  Public NormalizedFullName As String
 End Structure
#End Region

#Region " Constructors "
 Public Sub New(locale As String)
  MyBase.New()

  For Each li As ListEntryInfo In (New ListController).GetListEntryInfoItems("Country")
   'Dim text As String = DotNetNuke.Services.Localization.Localization.GetString(li.Value, "~/App_GlobalResources/Country.resx", locale)
   'If String.IsNullOrEmpty(text) Then text = li.Text
   Dim text As String = li.Text
   Dim c As New Country With {.Code = li.Value, .FullName = String.Format("{0} ({1})", text, li.Value), .Name = text}
   c.NormalizedFullName = c.FullName.NormalizeString
   Add(li.Value, c)
  Next

 End Sub
#End Region

#Region " Static Methods "
 Public Shared Function GetCountryList(locale As String) As CachedCountryList

  Dim res As CachedCountryList = Nothing
  Try
   res = CType(DotNetNuke.Common.Utilities.DataCache.GetCache(CacheKey(locale)), CachedCountryList)
  Catch ex As Exception
  End Try
  If res Is Nothing Then
   res = New CachedCountryList(locale)
   DotNetNuke.Common.Utilities.DataCache.SetCache(CacheKey(locale), res)
  End If
  Return res

 End Function

 Public Shared Function CacheKey(locale As String) As String
  Return String.Format("CountryList:{0}", locale)
 End Function
#End Region

End Class
