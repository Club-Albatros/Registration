Imports System
Imports System.Data
Imports System.IO
Imports System.Runtime.Serialization
Imports System.Runtime.Serialization.Json
Imports System.Text
Imports System.Xml
Imports System.Xml.Schema
Imports System.Xml.Serialization

Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Services.Tokens

Namespace Entities.Roles
 <Serializable(), XmlRoot("Role"), DataContract()>
 Partial Public Class RegistrationRoleInfo
  Implements IHydratable
  Implements IPropertyAccess
  Implements IXmlSerializable

#Region " IHydratable Implementation "
  Public Overrides Sub Fill(dr As IDataReader) Implements IHydratable.Fill

   MyBase.Fill(dr)

   Locale = Convert.ToString(Null.SetNull(dr.Item("Locale"), Locale))
   Presentation = Convert.ToString(Null.SetNull(dr.Item("Presentation"), Presentation))
   ViewOrder = Convert.ToInt32(Null.SetNull(dr.Item("ViewOrder"), ViewOrder))
   RedirectTab = Convert.ToInt32(Null.SetNull(dr.Item("RedirectTab"), RedirectTab))

  End Sub
#End Region

#Region " IPropertyAccess Implementation "
  Public Shadows Function GetProperty(strPropertyName As String, strFormat As String, formatProvider As System.Globalization.CultureInfo, AccessingUser As DotNetNuke.Entities.Users.UserInfo, AccessLevel As DotNetNuke.Services.Tokens.Scope, ByRef PropertyNotFound As Boolean) As String Implements DotNetNuke.Services.Tokens.IPropertyAccess.GetProperty
   Dim OutputFormat As String = String.Empty
   Dim portalSettings As DotNetNuke.Entities.Portals.PortalSettings = DotNetNuke.Entities.Portals.PortalController.GetCurrentPortalSettings()
   If strFormat = String.Empty Then
    OutputFormat = "D"
   Else
    OutputFormat = strFormat
   End If
   Select Case strPropertyName.ToLower
    Case "locale"
     Return Me.Locale
    Case "presentation"
     Return Me.Presentation
    Case "vieworder"
     Return (Me.ViewOrder.ToString(OutputFormat, formatProvider))
    Case "redirecttab"
     Return (Me.RedirectTab.ToString(OutputFormat, formatProvider))
    Case Else
     Return MyBase.GetProperty(strPropertyName, strFormat, formatProvider, AccessingUser, AccessLevel, PropertyNotFound)
   End Select

   Return Null.NullString
  End Function
#End Region

 End Class
End Namespace


