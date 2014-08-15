Imports System.Web.UI
Imports System.Web.UI.WebControls

Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Web.UI.WebControls

Public Class DnnFormTextBoxItem
 Inherits DnnFormItemBase
 Private _textBox As TextBox

 Public Property AutoCompleteType As AutoCompleteType
 Public Property Columns As Integer
 Public Property Rows As Integer
 Public Property TextMode As TextBoxMode

 Public Property TextBoxCssClass() As String
  Get
   Return ViewState.GetValue("TextBoxCssClass", String.Empty)
  End Get
  Set(value As String)
   ViewState.SetValue("TextBoxCssClass", value, String.Empty)
  End Set
 End Property

 Private Sub TextChanged(sender As Object, e As EventArgs)
  'UpdateDataSource(Value, _textBox.Text, DataField)
  RaiseEvent UpdateSource(DataField, DataMember, Value, _textBox.Text)
 End Sub
 Public Event UpdateSource(dataField As String, dataMember As String, oldValue As Object, newValue As String)

 Protected Overrides Function CreateControlInternal(container As Control) As WebControl

  _textBox = New TextBox() With {.ID = ID & "_TextBox"}

  _textBox.Rows = Rows
  _textBox.Columns = Columns
  _textBox.TextMode = TextMode
  _textBox.CssClass = TextBoxCssClass
  _textBox.AutoCompleteType = AutoCompleteType
  AddHandler _textBox.TextChanged, AddressOf TextChanged

  'Load from ControlState
  _textBox.Text = Convert.ToString(Value)
  If TextMode = TextBoxMode.Password Then
   _textBox.Attributes.Add("autocomplete", "off")
  End If
  container.Controls.Add(_textBox)

  Return _textBox
 End Function

 Protected Overrides Sub OnPreRender(e As EventArgs)
  MyBase.OnPreRender(e)

  If TextMode = TextBoxMode.Password Then
   _textBox.Attributes.Add("value", Convert.ToString(Value))
  End If
 End Sub

End Class
