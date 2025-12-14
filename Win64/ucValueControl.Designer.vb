<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ucValueControl
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
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
        cboSource = New ComboBox()
        lblSource = New Label()
        lblMin = New Label()
        txtVal1 = New TextBox()
        txtVal2 = New TextBox()
        lblMax = New Label()
        cboVal1 = New ComboBox()
        lblUnits = New Label()
        cboChannel = New ComboBox()
        cmdEditVarName = New Button()
        SuspendLayout()
        ' 
        ' cboSource
        ' 
        cboSource.FormattingEnabled = True
        cboSource.Location = New Point(68, 0)
        cboSource.Name = "cboSource"
        cboSource.Size = New Size(92, 23)
        cboSource.TabIndex = 0
        ' 
        ' lblSource
        ' 
        lblSource.Location = New Point(0, 3)
        lblSource.Name = "lblSource"
        lblSource.Size = New Size(65, 15)
        lblSource.TabIndex = 1
        lblSource.Text = "DutyCycle:"
        lblSource.TextAlign = ContentAlignment.MiddleRight
        ' 
        ' lblMin
        ' 
        lblMin.Location = New Point(163, 0)
        lblMin.Margin = New Padding(0)
        lblMin.Name = "lblMin"
        lblMin.Size = New Size(32, 21)
        lblMin.TabIndex = 2
        lblMin.Text = "Min:"
        lblMin.TextAlign = ContentAlignment.MiddleRight
        ' 
        ' txtVal1
        ' 
        txtVal1.Location = New Point(196, 0)
        txtVal1.MaxLength = 10
        txtVal1.Name = "txtVal1"
        txtVal1.Size = New Size(44, 23)
        txtVal1.TabIndex = 3
        txtVal1.Text = "628000"
        txtVal1.WordWrap = False
        ' 
        ' txtVal2
        ' 
        txtVal2.Location = New Point(285, 0)
        txtVal2.MaxLength = 10
        txtVal2.Name = "txtVal2"
        txtVal2.Size = New Size(45, 23)
        txtVal2.TabIndex = 5
        txtVal2.WordWrap = False
        ' 
        ' lblMax
        ' 
        lblMax.Location = New Point(244, 3)
        lblMax.Margin = New Padding(0)
        lblMax.Name = "lblMax"
        lblMax.Size = New Size(38, 15)
        lblMax.TabIndex = 4
        lblMax.Text = "Chan:"
        lblMax.TextAlign = ContentAlignment.MiddleRight
        ' 
        ' cboVal1
        ' 
        cboVal1.FormattingEnabled = True
        cboVal1.Location = New Point(196, 0)
        cboVal1.Name = "cboVal1"
        cboVal1.Size = New Size(45, 23)
        cboVal1.TabIndex = 6
        ' 
        ' lblUnits
        ' 
        lblUnits.Location = New Point(327, 5)
        lblUnits.Name = "lblUnits"
        lblUnits.Size = New Size(37, 15)
        lblUnits.TabIndex = 7
        lblUnits.Text = "%"
        lblUnits.TextAlign = ContentAlignment.MiddleLeft
        ' 
        ' cboChannel
        ' 
        cboChannel.FormattingEnabled = True
        cboChannel.Location = New Point(297, 0)
        cboChannel.Name = "cboChannel"
        cboChannel.Size = New Size(93, 23)
        cboChannel.TabIndex = 8
        ' 
        ' cmdEditVarName
        ' 
        cmdEditVarName.Font = New Font("Segoe UI", 7F)
        cmdEditVarName.Location = New Point(359, 2)
        cmdEditVarName.Name = "cmdEditVarName"
        cmdEditVarName.Size = New Size(33, 19)
        cmdEditVarName.TabIndex = 9
        cmdEditVarName.Text = "Edit"
        cmdEditVarName.UseVisualStyleBackColor = True
        cmdEditVarName.Visible = False
        ' 
        ' ucValueControl
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        Controls.Add(cmdEditVarName)
        Controls.Add(cboChannel)
        Controls.Add(lblUnits)
        Controls.Add(cboVal1)
        Controls.Add(txtVal2)
        Controls.Add(lblMax)
        Controls.Add(txtVal1)
        Controls.Add(lblMin)
        Controls.Add(lblSource)
        Controls.Add(cboSource)
        Name = "ucValueControl"
        Size = New Size(395, 23)
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents cboSource As ComboBox
    Friend WithEvents lblSource As Label
    Friend WithEvents lblMin As Label
    Friend WithEvents txtVal1 As TextBox
    Friend WithEvents txtVal2 As TextBox
    Friend WithEvents lblMax As Label
    Friend WithEvents cboVal1 As ComboBox
    Friend WithEvents lblUnits As Label
    Friend WithEvents cboChannel As ComboBox
    Friend WithEvents cmdEditVarName As Button

End Class
