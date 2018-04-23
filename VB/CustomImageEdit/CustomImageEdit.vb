Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Text
Imports DevExpress.XtraEditors
Imports DevExpress.XtraEditors.Repository
Imports DevExpress.XtraEditors.Registrator
Imports DevExpress.XtraEditors.ViewInfo
Imports DevExpress.XtraEditors.Drawing
Imports DevExpress.XtraEditors.Controls
Imports DevExpress.XtraEditors.Popup
Imports System.ComponentModel
Imports System.Drawing
Imports System.Windows.Forms
Imports System.Reflection

Namespace MyCustomEdit
	<UserRepositoryItem("RegisterCustomImageEdit")> _
	Public Class RepositoryItemCustomImageEdit
		Inherits RepositoryItemPopupBase
		Protected loadButtonIndex_Renamed As Integer
		Private openFile As OpenFileDialog
		Public Sub New()
			MyBase.New()
			ActionButtonIndex = 1
			loadButtonIndex_Renamed = 0
		End Sub
		Public Overridable ReadOnly Property LoadButtonIndex() As Integer
			Get
				Return loadButtonIndex_Renamed
			End Get
		End Property

		Public Overrides Sub CreateDefaultButton()
			Dim btn As New EditorButton()
			btn.IsDefaultButton = True
			btn.Kind = ButtonPredefines.Plus
			Buttons.Add(btn)
			MyBase.CreateDefaultButton()
		End Sub

		Public Overridable Sub LoadFile()
			If openFile Is Nothing Then
				openFile = New OpenFileDialog()
			End If
			If openFile.ShowDialog() = System.Windows.Forms.DialogResult.OK Then
				OwnerEdit.EditValue = openFile.FileName
			End If
		End Sub

		Shared Sub New()
			RegisterCustomImageEdit()
		End Sub

		Public Const CustomImageEditName As String = "CustomImageEdit"

		Public Overrides ReadOnly Property EditorTypeName() As String
			Get
				Return CustomImageEditName
			End Get
		End Property


		Public Shared Sub RegisterCustomImageEdit()
			EditorRegistrationInfo.Default.Editors.Add(New EditorClassInfo(CustomImageEditName, GetType(CustomImageEdit), GetType(RepositoryItemCustomImageEdit), GetType(PopupBaseEditViewInfo), New ButtonEditPainter(), True))
		End Sub


	End Class

	Public Class CustomImageEdit
		Inherits PopupBaseEdit
		Public Sub New()
			MyBase.New()
		End Sub

		Shared Sub New()
			RepositoryItemCustomImageEdit.RegisterCustomImageEdit()
		End Sub

		Public Overrides ReadOnly Property EditorTypeName() As String
			Get
				Return RepositoryItemCustomImageEdit.CustomImageEditName
			End Get
		End Property

		<DesignerSerializationVisibility(DesignerSerializationVisibility.Content)> _
		Public Shadows ReadOnly Property Properties() As RepositoryItemCustomImageEdit
			Get
				Return TryCast(MyBase.Properties, RepositoryItemCustomImageEdit)
			End Get
		End Property

		Protected Overrides Function CreatePopupForm() As DevExpress.XtraEditors.Popup.PopupBaseForm
			Return New CustomPopupBaseSizeableForm(Me)
		End Function

		Protected Overrides Sub OnPressButton(ByVal buttonInfo As EditorButtonObjectInfoArgs)
			If IsLoadButton(buttonInfo) Then
				Properties.LoadFile()
			End If
			MyBase.OnPressButton(buttonInfo)
		End Sub

		Protected Overridable Function IsLoadButton(ByVal buttonInfo As EditorButtonObjectInfoArgs) As Boolean
			Return Properties.LoadButtonIndex >= 0 AndAlso Properties.Buttons.IndexOf(buttonInfo.Button) = Properties.LoadButtonIndex
		End Function
	End Class

	Public Class CustomPopupBaseSizeableForm
		Inherits PopupBaseSizeableForm
		Public myImage As Image
		Private openButton, okButton, cancelButton, clearButton As SimpleButton
		Private openFile As OpenFileDialog
		Private resultFileName As String
		Private formContentSize As Size

		Public Sub New(ByVal ownerEdit As PopupBaseEdit)
			MyBase.New(ownerEdit)
			CreateButtons()
			openFile = New OpenFileDialog()
			formContentSize = New Size(200, 200)
		End Sub

		Protected Overridable Sub CreateButtons()
			okButton = New SimpleButton()
			okButton.Text = "Ok"
			okButton.AllowFocus = False
			AddHandler okButton.Click, AddressOf okButton_Click
			Controls.Add(okButton)

			cancelButton = New SimpleButton()
			cancelButton.Text = "Cancel"
			cancelButton.AllowFocus = False
			AddHandler cancelButton.Click, AddressOf cancelButton_Click
			Controls.Add(cancelButton)

			openButton = New SimpleButton()
			openButton.Text = "Open"
			openButton.AllowFocus = False
			AddHandler openButton.Click, AddressOf openButton_Click
			Controls.Add(openButton)

			clearButton = New SimpleButton()
			clearButton.Text = "Clear"
			clearButton.AllowFocus = False
			AddHandler clearButton.Click, AddressOf clearButton_Click
			Controls.Add(clearButton)
		End Sub
		Protected Overrides ReadOnly Property MinFormSize() As Size
			Get
				Dim newSize As Size = MyBase.MinFormSize
				newSize.Width = ViewInfo.ButtonSize.Width * 5
				Return newSize
			End Get
		End Property
		Protected Overridable Sub ChangeButtonsPositions()
			okButton.Bounds = ViewInfo.OkButtonRect
			cancelButton.Bounds = ViewInfo.CancelButtonRect
			openButton.Bounds = ViewInfo.OpenButtonRect
			clearButton.Bounds = ViewInfo.ClearButtonRect
		End Sub

		Private Sub okButton_Click(ByVal sender As Object, ByVal e As EventArgs)
			ClosePopup(PopupCloseMode.Normal)
		End Sub

		Private Sub cancelButton_Click(ByVal sender As Object, ByVal e As EventArgs)
			ClosePopup(PopupCloseMode.Cancel)
		End Sub

		Private Sub clearButton_Click(ByVal sender As Object, ByVal e As EventArgs)
			myImage = Nothing
			openFile.FileName = ""
			resultFileName = ""
			LayoutChanged()
		End Sub

		Private Sub openButton_Click(ByVal sender As Object, ByVal e As EventArgs)
			Dim res As System.Windows.Forms.DialogResult = openFile.ShowDialog()
			OwnerEdit.ShowPopup()
			If res = System.Windows.Forms.DialogResult.OK Then
				LoadImage(openFile.FileName)
				Invalidate()
			End If
		End Sub
		Protected Overrides Sub LayoutChanged()
			MyBase.LayoutChanged()
			ChangeButtonsPositions()
		End Sub

		Public Overridable Function LoadImage(ByVal fileName As String) As Boolean
			If fileName IsNot Nothing AndAlso fileName <> String.Empty Then
				Try
					myImage = Image.FromFile(fileName)
				Catch
					resultFileName = String.Empty
					ClearImage()
					Return False
				End Try
				Me.resultFileName = fileName
				Return True
			End If
			resultFileName = String.Empty
			ClearImage()
			Return False

		End Function

		Public Sub ClearImage()
			If myImage IsNot Nothing Then
				myImage.Dispose()
				myImage = Nothing
			End If
		End Sub

		Public Overrides Overloads Function CalcFormSize(ByVal contentSize As Size) As Size
			Return MyBase.CalcFormSize(formContentSize)
		End Function

		Public Overrides ReadOnly Property ResultValue() As Object
			Get
				Return resultFileName
			End Get
		End Property

		Protected Overrides Overloads Sub ClosePopup(ByVal closeMode As PopupCloseMode)
			ClearImage()
			MyBase.ClosePopup(closeMode)
			resultFileName = ""
		End Sub

		Public Overrides Sub ShowPopupForm()
			Dim fileName As String = TryCast(OwnerEdit.EditValue, String)
			LoadImage(fileName)
			MyBase.ShowPopupForm()
		End Sub

		Protected Overrides Function CreatePainter() As PopupBaseFormPainter
			Return New CustomPopupBaseFormPainter()
		End Function
		Protected Overrides Function CreateViewInfo() As PopupBaseFormViewInfo
			Return New CustomPopupBaseSizeableFormViewInfo(Me)
		End Function
		Protected Shadows ReadOnly Property ViewInfo() As CustomPopupBaseSizeableFormViewInfo
			Get
				Return TryCast(MyBase.ViewInfo, CustomPopupBaseSizeableFormViewInfo)
			End Get
		End Property
		Protected Overrides Sub OnSizeChanged(ByVal e As EventArgs)
			formContentSize = ViewInfo.ContentRect.Size
			MyBase.OnSizeChanged(e)
		End Sub

	End Class
	Public Class CustomPopupBaseFormPainter
		Inherits PopupBaseSizeableFormPainter
		Public Sub New()
			MyBase.New()

		End Sub

		Public Overrides Sub Draw(ByVal info As PopupFormGraphicsInfoArgs)
			MyBase.Draw(info)
			DrawImage(info)
		End Sub

		Private Sub DrawImage(ByVal info As PopupFormGraphicsInfoArgs)
			Dim vi As CustomPopupBaseSizeableFormViewInfo = TryCast(info.ViewInfo, CustomPopupBaseSizeableFormViewInfo)
			Dim form As CustomPopupBaseSizeableForm = TryCast(vi.Form, CustomPopupBaseSizeableForm)
			If form.myImage IsNot Nothing Then
				info.Graphics.DrawImage(form.myImage, vi.ImageRect)
			End If
		End Sub
	End Class
	Public Class CustomPopupBaseSizeableFormViewInfo
		Inherits PopupBaseSizeableFormViewInfo
		Private okButtonRect_Renamed, openButtonRect_Renamed, clearButtonRect_Renamed, cancelButtonRect_Renamed, imageRect_Renamed As Rectangle
		Private Const borderWidth As Integer = 2, buttonWidth As Integer = 40, buttonHeight As Integer = 18
		Private buttonSize_Renamed As Size

		Public Sub New(ByVal form As PopupBaseForm)
			MyBase.New(form)
			buttonSize_Renamed = New Size(buttonWidth, buttonHeight)
			imageRect_Renamed = Rectangle.Empty
			okButtonRect_Renamed = Rectangle.Empty
			openButtonRect_Renamed = Rectangle.Empty
			clearButtonRect_Renamed = Rectangle.Empty
			cancelButtonRect_Renamed = Rectangle.Empty
		End Sub
		Public Overridable ReadOnly Property OkButtonRect() As Rectangle
			Get
				Return okButtonRect_Renamed
			End Get
		End Property
		Public Overridable ReadOnly Property OpenButtonRect() As Rectangle
			Get
				Return openButtonRect_Renamed
			End Get
		End Property
		Public Overridable ReadOnly Property ClearButtonRect() As Rectangle
			Get
				Return clearButtonRect_Renamed
			End Get
		End Property
		Public Overridable ReadOnly Property CancelButtonRect() As Rectangle
			Get
				Return cancelButtonRect_Renamed
			End Get
		End Property
		Public Overridable ReadOnly Property ImageRect() As Rectangle
			Get
				Return imageRect_Renamed
			End Get
		End Property

		Public Overridable ReadOnly Property ButtonSize() As Size
			Get
				Return buttonSize_Renamed
			End Get
		End Property
		Protected Overrides Sub CalcRects()
			MyBase.CalcRects()
			imageRect_Renamed = ContentRect

			okButtonRect_Renamed = New Rectangle(imageRect_Renamed.Left + borderWidth, imageRect_Renamed.Bottom + borderWidth, ButtonSize.Width, ButtonSize.Height)
			cancelButtonRect_Renamed = okButtonRect_Renamed
			cancelButtonRect_Renamed.Offset(borderWidth * 2 + buttonSize_Renamed.Width, 0)
			openButtonRect_Renamed = cancelButtonRect_Renamed
			openButtonRect_Renamed.Offset(borderWidth * 6 + buttonSize_Renamed.Width, 0)
			clearButtonRect_Renamed = openButtonRect_Renamed
			clearButtonRect_Renamed.Offset(borderWidth * 2 + buttonSize_Renamed.Width, 0)
		End Sub
	End Class
End Namespace
