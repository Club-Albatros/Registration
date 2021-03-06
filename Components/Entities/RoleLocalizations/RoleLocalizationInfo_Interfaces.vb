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

Namespace Entities.RoleLocalizations
  <Serializable(), XmlRoot("RoleLocalization"), DataContract()>
  Partial Public Class RoleLocalizationInfo
   Implements IHydratable
   Implements IPropertyAccess
   Implements IXmlSerializable

#Region " IHydratable Implementation "
 ''' -----------------------------------------------------------------------------
 ''' <summary>
 ''' Fill hydrates the object from a Datareader
 ''' </summary>
 ''' <remarks>The Fill method is used by the CBO method to hydrtae the object
 ''' rather than using the more expensive Refection  methods.</remarks>
 ''' <history>
 ''' 	[pdonker]	08/12/2014  Created
 ''' </history>
 ''' -----------------------------------------------------------------------------
 Public Sub Fill(dr As IDataReader) Implements IHydratable.Fill

  Description = Convert.ToString(Null.SetNull(dr.Item("Description"), Description))
  IconFile = Convert.ToString(Null.SetNull(dr.Item("IconFile"), IconFile))
  Locale = Convert.ToString(Null.SetNull(dr.Item("Locale"), Locale))
  Presentation = Convert.ToString(Null.SetNull(dr.Item("Presentation"), Presentation))
  RoleId = Convert.ToInt32(Null.SetNull(dr.Item("RoleId"), RoleId))
  RoleName = Convert.ToString(Null.SetNull(dr.Item("RoleName"), RoleName))

 End Sub
 ''' -----------------------------------------------------------------------------
 ''' <summary>
 ''' Gets and sets the Key ID
 ''' </summary>
 ''' <remarks>The KeyID property is part of the IHydratble interface.  It is used
 ''' as the key property when creating a Dictionary</remarks>
 ''' <history>
 ''' 	[pdonker]	08/12/2014  Created
 ''' </history>
 ''' -----------------------------------------------------------------------------
 Public Property KeyID() As Integer Implements IHydratable.KeyID
  Get
   Return Nothing
  End Get
  Set(value As Integer)
  End Set
 End Property
#End Region

#Region " IPropertyAccess Implementation "
 Public Function GetProperty(strPropertyName As String, strFormat As String, formatProvider As System.Globalization.CultureInfo, AccessingUser As DotNetNuke.Entities.Users.UserInfo, AccessLevel As DotNetNuke.Services.Tokens.Scope, ByRef PropertyNotFound As Boolean) As String Implements DotNetNuke.Services.Tokens.IPropertyAccess.GetProperty
  Dim OutputFormat As String = String.Empty
  Dim portalSettings As DotNetNuke.Entities.Portals.PortalSettings = DotNetNuke.Entities.Portals.PortalController.GetCurrentPortalSettings()
  If strFormat = String.Empty Then
   OutputFormat = "D"
  Else
   OutputFormat = strFormat
  End If
  Select Case strPropertyName.ToLower
   Case "description"
    Return PropertyAccess.FormatString(Me.Description, strFormat)
   Case "iconfile"
    Return PropertyAccess.FormatString(Me.IconFile, strFormat)
   Case "locale"
    Return PropertyAccess.FormatString(Me.Locale, strFormat)
   Case "presentation"
    Return PropertyAccess.FormatString(Me.Presentation, strFormat)
   Case "roleid"
    Return (Me.RoleId.ToString(OutputFormat, formatProvider))
   Case "rolename"
    Return PropertyAccess.FormatString(Me.RoleName, strFormat)
   Case Else
    PropertyNotFound = True
  End Select

  Return Null.NullString
 End Function

 Public ReadOnly Property Cacheability() As DotNetNuke.Services.Tokens.CacheLevel Implements DotNetNuke.Services.Tokens.IPropertyAccess.Cacheability
  Get
   Return CacheLevel.fullyCacheable
  End Get
 End Property
#End Region

#Region " IXmlSerializable Implementation "
  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' GetSchema returns the XmlSchema for this class
  ''' </summary>
  ''' <remarks>GetSchema is implemented as a stub method as it is not required</remarks>
  ''' <history>
  ''' 	[pdonker]	08/12/2014  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public Function GetSchema() As XmlSchema Implements IXmlSerializable.GetSchema
   Return Nothing
  End Function

  Private Function readElement(reader As XmlReader, ElementName As String) As String
   If (Not reader.NodeType = XmlNodeType.Element) OrElse reader.Name <> ElementName Then
    reader.ReadToFollowing(ElementName)
   End If
   If reader.NodeType = XmlNodeType.Element Then
    Return reader.ReadElementContentAsString
   Else
    Return ""
   End If
  End Function

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' ReadXml fills the object (de-serializes it) from the XmlReader passed
  ''' </summary>
  ''' <remarks></remarks>
  ''' <param name="reader">The XmlReader that contains the xml for the object</param>
  ''' <history>
  ''' 	[pdonker]	08/12/2014  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public Sub ReadXml(reader As XmlReader) Implements IXmlSerializable.ReadXml
   Try

    Description = readElement(reader, "Description")
    IconFile = readElement(reader, "IconFile")
    Presentation = readElement(reader, "Presentation")
    RoleName = readElement(reader, "RoleName")
   Catch ex As Exception
    ' log exception as DNN import routine does not do that
    DotNetNuke.Services.Exceptions.LogException(ex)
    ' re-raise exception to make sure import routine displays a visible error to the user
    Throw New Exception("An error occured during import of an RoleLocalization", ex)
   End Try

  End Sub

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' WriteXml converts the object to Xml (serializes it) and writes it using the XmlWriter passed
  ''' </summary>
  ''' <remarks></remarks>
  ''' <param name="writer">The XmlWriter that contains the xml for the object</param>
  ''' <history>
  ''' 	[pdonker]	08/12/2014  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public Sub WriteXml(writer As XmlWriter) Implements IXmlSerializable.WriteXml
   writer.WriteStartElement("RoleLocalization")
   writer.WriteElementString("RoleId",  RoleId.ToString())
   writer.WriteElementString("Locale",  Locale.ToString())
   writer.WriteElementString("Description",  Description)
   writer.WriteElementString("IconFile",  IconFile)
   writer.WriteElementString("Presentation",  Presentation)
   writer.WriteElementString("RoleName",  RoleName)
   writer.WriteEndElement()
  End Sub
#End Region

#Region " ToXml Methods "
  Public Function ToXml() As String
   Return ToXml("RoleLocalization")
  End Function

  Public Function ToXml(elementName As String) As String
   Dim xml As New StringBuilder
   xml.Append("<")
   xml.Append(elementName)
   AddAttribute(xml, "RoleId", RoleId.ToString())
   AddAttribute(xml, "Locale", Locale.ToString())
   AddAttribute(xml, "Description", Description)
   AddAttribute(xml, "IconFile", IconFile)
   AddAttribute(xml, "Presentation", Presentation)
   AddAttribute(xml, "RoleName", RoleName)
   xml.Append(" />")
   Return xml.ToString
  End Function

  Private Sub AddAttribute(ByRef xml As StringBuilder, attributeName As String, attributeValue As String)
   xml.Append(" " & attributeName)
   xml.Append("=""" & attributeValue & """")
  End Sub
#End Region

#Region " JSON Serialization "
  Public Sub WriteJSON(ByRef s As Stream)
   Dim ser As New DataContractJsonSerializer(GetType(RoleLocalizationInfo))
   ser.WriteObject(s, Me)
  End Sub
#End Region

 End Class
End Namespace


