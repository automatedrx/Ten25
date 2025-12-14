<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class DialogVarNamesSingle
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Cancel_Button = New Button()
        txtName = New TextBox()
        OK_Button = New Button()
        SuspendLayout()
        ' 
        ' Cancel_Button
        ' 
        Cancel_Button.Location = New Point(97, 41)
        Cancel_Button.Margin = New Padding(4, 3, 4, 3)
        Cancel_Button.Name = "Cancel_Button"
        Cancel_Button.Size = New Size(77, 27)
        Cancel_Button.TabIndex = 2
        Cancel_Button.Text = "Cancel"
        ' 
        ' txtName
        ' 
        txtName.Location = New Point(12, 12)
        txtName.Name = "txtName"
        txtName.Size = New Size(162, 23)
        txtName.TabIndex = 0
        ' 
        ' OK_Button
        ' 
        OK_Button.Location = New Point(12, 41)
        OK_Button.Margin = New Padding(4, 3, 4, 3)
        OK_Button.Name = "OK_Button"
        OK_Button.Size = New Size(77, 27)
        OK_Button.TabIndex = 1
        OK_Button.Text = "OK"
        ' 
        ' DialogVarNames
        ' 
        AcceptButton = OK_Button
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        CancelButton = Cancel_Button
        ClientSize = New Size(189, 81)
        ControlBox = False
        Controls.Add(Cancel_Button)
        Controls.Add(OK_Button)
        Controls.Add(txtName)
        FormBorderStyle = FormBorderStyle.FixedDialog
        Margin = New Padding(4, 3, 4, 3)
        MaximizeBox = False
        MinimizeBox = False
        Name = "DialogVarNames"
        ShowInTaskbar = False
        StartPosition = FormStartPosition.CenterParent
        Text = "Variable Names"
        ResumeLayout(False)
        PerformLayout()

    End Sub
    Friend WithEvents Cancel_Button As System.Windows.Forms.Button
    Friend WithEvents txtName As TextBox
    Friend WithEvents OK_Button As Button

End Class
