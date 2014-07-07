Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.UI.WebControls
Imports DotNetNuke.Entities.Portals

<ToolboxData("<{0}:DnnRegionControl runat=server></{0}:DnnRegionControl>")>
Public Class DnnRegionControl
 Inherits EditControl

#Region " Controls "
 Private _Regions As DropDownList
 Private ReadOnly Property Regions As DropDownList
  Get
   If _Regions Is Nothing Then
    _Regions = New DropDownList
   End If
   Return _Regions
  End Get
 End Property

 Private _Region As TextBox
 Private ReadOnly Property Region As TextBox
  Get
   If _Region Is Nothing Then
    _Region = New TextBox
   End If
   Return _Region
  End Get
 End Property

 Private _InitialValue As HiddenField
 Private ReadOnly Property InitialValue As HiddenField
  Get
   If _InitialValue Is Nothing Then
    _InitialValue = New HiddenField
   End If
   Return _InitialValue
  End Get
 End Property
#End Region

#Region " Properties "
 Protected Overrides Property StringValue() As String
  Get
   Dim strValue As String = Null.NullString
   If Value IsNot Nothing Then
    strValue = Convert.ToString(Value)
   End If
   Return strValue
  End Get
  Set(value As String)
   value = value
  End Set
 End Property

 Protected ReadOnly Property OldStringValue() As String
  Get
   Return Convert.ToString(OldValue)
  End Get
 End Property
#End Region

#Region " Constructors "
 Public Sub New()
 End Sub
 Public Sub New(type As String)
  SystemType = type
 End Sub
#End Region

#Region " Overrides "
 Protected Overrides Sub OnDataChanged(e As EventArgs)
  Dim args As New PropertyEditorEventArgs(Name)
  args.Value = StringValue
  args.OldValue = OldStringValue
  args.StringValue = StringValue
  MyBase.OnValueChanged(args)
 End Sub

 Protected Overrides Sub CreateChildControls()
  MyBase.CreateChildControls()

  Regions.ControlStyle.CopyFrom(ControlStyle)
  Regions.ID = ID + "_dropdown"
  Controls.Add(Regions)

  Region.ControlStyle.CopyFrom(ControlStyle)
  Region.ID = ID + "_text"
  Controls.Add(Region)

  InitialValue.ID = ID + "_value"
  Controls.Add(InitialValue)

 End Sub

 Public Overrides Function LoadPostData(postDataKey As String, postCollection As System.Collections.Specialized.NameValueCollection) As Boolean
  EnsureChildControls()
  Dim dataChanged As Boolean = False
  Dim presentValue As String = StringValue
  Dim postedValue As String = postCollection(postDataKey & "_dropdown")
  If String.IsNullOrEmpty(postedValue) Then postedValue = postCollection(postDataKey & "_value")
  If Not presentValue.Equals(postedValue) Then
   Value = postedValue
   dataChanged = True
  End If
  Return dataChanged
 End Function

 Protected Overrides Sub OnPreRender(e As System.EventArgs)
  MyBase.OnPreRender(e)

  LoadControls()

  If Page IsNot Nothing And EditMode = PropertyEditorMode.Edit Then
   Page.RegisterRequiresPostBack(Me)
   Page.RegisterRequiresPostBack(Regions)
  End If

 End Sub

 Protected Overrides Sub RenderEditMode(writer As HtmlTextWriter)
  RenderChildren(writer)
 End Sub
#End Region

#Region " Page Events "
 Private Sub DnnCountryRegionControl_Init(sender As Object, e As System.EventArgs) Handles Me.Init
  DotNetNuke.Web.Client.ClientResourceManagement.ClientResourceManager.RegisterScript(Page, ResolveUrl("~/DesktopModules/Albatros/Registration/js/countryregionbox.js"), 70)
  DotNetNuke.Framework.jQuery.RequestRegistration()
  DotNetNuke.Framework.jQuery.RequestUIRegistration()
 End Sub

 Private Sub DnnCountryRegionControl_Load(sender As Object, e As System.EventArgs) Handles Me.Load

  Dim script As String = String.Format(
  <![CDATA[
  <script type='text/javascript'>
   var dnnRegionBoxId = '{0}';
  </script>
  ]]>.Value, Me.ClientID)
  DotNetNuke.UI.Utilities.ClientAPI.RegisterStartUpScript(Page, "DnnRegionControl", script)

 End Sub
#End Region

#Region " Private Methods "
 Private Sub LoadControls()
  ' todo
  InitialValue.Value = OldStringValue
 End Sub
#End Region

End Class
