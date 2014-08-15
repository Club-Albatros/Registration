#Region "Copyright"
' 
' DotNetNuke® - http://www.dotnetnuke.com
' Copyright (c) 2002-2013
' by DotNetNuke Corporation
' 
' Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
' documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
' the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
' to permit persons to whom the Software is furnished to do so, subject to the following conditions:
' 
' The above copyright notice and this permission notice shall be included in all copies or substantial portions 
' of the Software.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
' DEALINGS IN THE SOFTWARE.
#End Region
#Region "Usings"

Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Framework
Imports DotNetNuke.Web.Client.ClientResourceManagement
Imports DotNetNuke.Web.UI.WebControls

#End Region

Public Class DnnFormPasswordItem
 Inherits DnnFormItemBase
 Private _password As TextBox

 Public Property TextBoxCssClass() As String
  Get
   Return ViewState.GetValue("TextBoxCssClass", String.Empty)
  End Get
  Set(value As String)
   ViewState.SetValue("TextBoxCssClass", value, String.Empty)
  End Set
 End Property

 Public Property ContainerCssClass() As String
  Get
   Return ViewState.GetValue("ContainerCssClass", String.Empty)
  End Get
  Set(value As String)
   ViewState.SetValue("ContainerCssClass", value, String.Empty)
  End Set
 End Property

 Private Sub TextChanged(sender As Object, e As EventArgs)
  'UpdateDataSource(Value, _password.Text, DataField)
  RaiseEvent UpdateSource(DataField, DataMember, Value, _password.Text)
 End Sub
 Public Event UpdateSource(dataField As String, dataMember As String, oldValue As Object, newValue As String)

 ''' <summary>
 ''' Use container to add custom control hierarchy to
 ''' </summary>
 ''' <param name="container"></param>
 ''' <returns>An "input" control that can be used for attaching validators</returns>
 Protected Overrides Function CreateControlInternal(container As Control) As WebControl
  'ensure password cannot be cut if too long
  ' Load from ControlState
  _password = New TextBox() With {
    .ID = ID & "_TextBox",
    .TextMode = TextBoxMode.Password,
    .CssClass = TextBoxCssClass,
    .MaxLength = 20,
    .Text = Convert.ToString(Value)
  }
  AddHandler _password.TextChanged, AddressOf TextChanged

  Dim passwordContainer As New Panel() With {
    .ID = "passwordContainer",
    .CssClass = ContainerCssClass
  }

  ' add control hierarchy to the container
  container.Controls.Add(passwordContainer)

  passwordContainer.Controls.Add(_password)

  ' return input control that can be used for validation
  Return _password
 End Function

 Protected Overrides Sub OnInit(e As EventArgs)
  MyBase.OnInit(e)
  ClientResourceManager.RegisterScript(Page, "~/Resources/Shared/scripts/dnn.jquery.extensions.js")
  ClientResourceManager.RegisterScript(Page, "~/Resources/Shared/scripts/dnn.jquery.tooltip.js")
  ClientResourceManager.RegisterScript(Page, "~/Resources/Shared/scripts/dnn.PasswordStrength.js")

  jQuery.RequestDnnPluginsRegistration()
 End Sub

 Protected Overrides Sub OnPreRender(e As EventArgs)
  MyBase.OnPreRender(e)

  Dim options As New DnnPaswordStrengthOptions()
  Dim optionsAsJsonString As String = Json.Serialize(options)

  Dim script As String = String.Format("dnn.initializePasswordStrength('.{0}', {1});{2}", TextBoxCssClass, optionsAsJsonString, Environment.NewLine)

  If ScriptManager.GetCurrent(Page) IsNot Nothing Then
   ' respect MS AJAX
   ScriptManager.RegisterStartupScript(Page, [GetType](), "PasswordStrength", script, True)
  Else
   Page.ClientScript.RegisterStartupScript([GetType](), "PasswordStrength", script, True)
  End If

 End Sub

End Class

