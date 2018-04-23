Imports Microsoft.VisualBasic
Imports System
Namespace CustomImageEdit_NS
	Partial Public Class Form1
		''' <summary>
		''' Required designer variable.
		''' </summary>
		Private components As System.ComponentModel.IContainer = Nothing

		''' <summary>
		''' Clean up any resources being used.
		''' </summary>
		''' <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		Protected Overrides Sub Dispose(ByVal disposing As Boolean)
			If disposing AndAlso (components IsNot Nothing) Then
				components.Dispose()
			End If
			MyBase.Dispose(disposing)
		End Sub

		#Region "Windows Form Designer generated code"

		''' <summary>
		''' Required method for Designer support - do not modify
		''' the contents of this method with the code editor.
		''' </summary>
		Private Sub InitializeComponent()
			Me.customImageEdit2 = New MyCustomEdit.CustomImageEdit()
			Me.customImageEdit1 = New MyCustomEdit.CustomImageEdit()
			CType(Me.customImageEdit2.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
			CType(Me.customImageEdit1.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
			Me.SuspendLayout()
			' 
			' customImageEdit2
			' 
			Me.customImageEdit2.Location = New System.Drawing.Point(12, 38)
			Me.customImageEdit2.Name = "customImageEdit2"
			Me.customImageEdit2.Properties.ActionButtonIndex = 1
			Me.customImageEdit2.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() { New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Plus), New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
			Me.customImageEdit2.Size = New System.Drawing.Size(100, 20)
			Me.customImageEdit2.TabIndex = 1
			' 
			' customImageEdit1
			' 
			Me.customImageEdit1.Location = New System.Drawing.Point(12, 12)
			Me.customImageEdit1.Name = "customImageEdit1"
			Me.customImageEdit1.Properties.ActionButtonIndex = 1
			Me.customImageEdit1.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() { New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Plus), New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
			Me.customImageEdit1.Size = New System.Drawing.Size(100, 20)
			Me.customImageEdit1.TabIndex = 0
			' 
			' Form1
			' 
			Me.AutoScaleDimensions = New System.Drawing.SizeF(6F, 13F)
			Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
			Me.ClientSize = New System.Drawing.Size(284, 262)
			Me.Controls.Add(Me.customImageEdit2)
			Me.Controls.Add(Me.customImageEdit1)
			Me.Name = "Form1"
			Me.Text = "Form1"
'			Me.Load += New System.EventHandler(Me.Form1_Load);
			CType(Me.customImageEdit2.Properties, System.ComponentModel.ISupportInitialize).EndInit()
			CType(Me.customImageEdit1.Properties, System.ComponentModel.ISupportInitialize).EndInit()
			Me.ResumeLayout(False)

		End Sub

		#End Region

		Private customImageEdit1 As MyCustomEdit.CustomImageEdit
		Private customImageEdit2 As MyCustomEdit.CustomImageEdit

	End Class
End Namespace

