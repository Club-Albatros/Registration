Imports System
Imports System.Runtime.Serialization

Namespace Entities.RoleProperties
  Partial Public Class RolePropertyInfo

#Region " Private Members "
#End Region
	
#Region " Constructors "
  Public Sub New()
  End Sub

  Public Sub New(roleId As Int32, propertyDefinitionID As Int32, required As Boolean)
   Me.PropertyDefinitionID = propertyDefinitionID
   Me.Required = required
   Me.RoleId = roleId
  End Sub
#End Region
	
#Region " Public Properties "
  <DataMember()>
  Public Property PropertyDefinitionID As Int32 = -1
  <DataMember()>
  Public Property Required As Boolean = False
  <DataMember()>
  Public Property RoleId As Int32 = -1
#End Region

 End Class
End Namespace


