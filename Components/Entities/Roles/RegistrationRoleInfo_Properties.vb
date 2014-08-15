Imports System
Imports System.Runtime.Serialization

Namespace Entities.Roles
 Partial Public Class RegistrationRoleInfo
  Inherits DotNetNuke.Security.Roles.RoleInfo

#Region " Private Members "
#End Region

#Region " Constructors "
  Public Sub New()
   MyBase.New()
  End Sub
#End Region

#Region " Public Properties "
  <DataMember()>
  Public Property Locale As String = ""
  <DataMember()>
  Public Property ViewOrder As Integer = 0
  <DataMember()>
  Public Property Presentation As String = ""
#End Region

 End Class
End Namespace


