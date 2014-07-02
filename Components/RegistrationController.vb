Imports System.Xml
Imports System.Text
Imports System.Collections.Generic
Imports System.IO

Imports DotNetNuke.Common.Utilities.XmlUtils
Imports DotNetNuke.Common.Utilities

Public Class RegistrationController
 Implements DotNetNuke.Entities.Modules.IPortable
 Implements DotNetNuke.Entities.Modules.IUpgradeable

#Region " IPortable Implementation "
 Public Function ExportModule(ByVal ModuleID As Integer) As String Implements DotNetNuke.Entities.Modules.IPortable.ExportModule
  Dim sb As New StringBuilder()
  Dim settings As New XmlWriterSettings()
  settings.ConformanceLevel = ConformanceLevel.Fragment
  settings.OmitXmlDeclaration = True

  sb.Append("<Registration><Settings>")
  Dim moduleSettings As Hashtable
  Dim objModules As New DotNetNuke.Entities.Modules.ModuleController
  moduleSettings = objModules.GetModuleSettings(ModuleID)
  For Each name As String In moduleSettings.Keys
   sb.Append("<Setting Name=""" + name + """>")
   sb.Append("<Value>" + XMLEncode(moduleSettings(name).ToString) + "</Value>")
   sb.Append("</Setting>")
  Next
  sb.Append("</Settings>")

  ' content items go here

  sb.Append("</Registration>")
  Return sb.ToString()

 End Function

 Public Sub ImportModule(ByVal ModuleID As Integer, ByVal Content As String, ByVal Version As String, ByVal UserID As Integer) Implements DotNetNuke.Entities.Modules.IPortable.ImportModule

  Dim xmlDoc As New XmlDocument
  xmlDoc.LoadXml(Content)

  Dim objModules As New DotNetNuke.Entities.Modules.ModuleController
  For Each xSetting As XmlNode In xmlDoc.SelectNodes("Registration/Settings/Setting")
   Dim settingName As String = xSetting.Attributes("Name").InnerText
   Dim settingValue As String = xSetting.SelectSingleNode("Value").InnerText
   objModules.UpdateModuleSetting(ModuleID, settingName, settingValue)
  Next

  Using reader As XmlReader = XmlReader.Create(New StringReader(Content))

   If reader.Read() Then
    reader.ReadStartElement("Registration")
    'reader.ReadToFollowing("Feeds")
    'reader.Read()
    'If reader.ReadState <> ReadState.EndOfFile And reader.NodeType <> XmlNodeType.None And reader.LocalName <> "" Then
    ' Do
    '  reader.ReadStartElement("Feed")
    '  Dim Feed As New FeedInfo

    'Deserialize Feed
    '  Feed.ReadXml(reader)

    'initialize values of the new Feed to this module and this user
    '  Feed.FeedID = Null.NullInteger
    '  Feed.ModuleID = ModuleID

    'Save Feed
    '  FeedController.AddFeed(Feed)
    ' Loop While reader.ReadToNextSibling("Feed")
    'End If
   End If

   reader.Close()
  End Using

 End Sub
#End Region

#Region " IUpgradeable Implementation "
 Public Function UpgradeModule(ByVal Version As String) As String Implements DotNetNuke.Entities.Modules.IUpgradeable.UpgradeModule
  Dim strResults As String = ""
  Try
   Select Case Version
    Case "01.00.00"
   End Select
  Catch ex As Exception
   strResults += "Error: " & ex.Message & vbCrLf
   Try
    DotNetNuke.Services.Exceptions.LogException(ex)
   Catch
    ' ignore
   End Try
  End Try
  Return strResults
 End Function
#End Region

End Class

