Imports System
Imports System.Runtime.Serialization

Namespace Entities.RoleLocalizations
  Partial Public Class RoleLocalizationInfo

#Region " Private Members "
#End Region
	
#Region " Constructors "
  Public Sub New()
  End Sub

  Public Sub New(roleId As Int32, locale As String, description As String, iconFile As String, presentation As String, roleName As String)
   Me.Description = description
   Me.IconFile = iconFile
   Me.Locale = locale
   Me.Presentation = presentation
   Me.RoleId = roleId
   Me.RoleName = roleName
  End Sub
#End Region
	
#Region " Public Properties "
  <DataMember()>
  Public Property Description As String = ""
  <DataMember()>
  Public Property IconFile As String = ""
  <DataMember()>
  Public Property Locale As String = ""
  <DataMember()>
  Public Property Presentation As String = ""
  <DataMember()>
  Public Property RoleId As Int32 = -1
  <DataMember()>
  Public Property RoleName As String = ""
#End Region

 End Class
End Namespace


