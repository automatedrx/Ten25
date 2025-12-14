<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class DialogVarNamesAll
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
        TableLayoutPanel1 = New TableLayoutPanel()
        OK_Button = New Button()
        Cancel_Button = New Button()
        Panel1 = New Panel()
        TextBox1 = New TextBox()
        Label1 = New Label()
        cboProgNum = New ComboBox()
        Label2 = New Label()
        rdoVars = New RadioButton()
        rdoTimers = New RadioButton()
        TableLayoutPanel1.SuspendLayout()
        Panel1.SuspendLayout()
        SuspendLayout()
        ' 
        ' TableLayoutPanel1
        ' 
        TableLayoutPanel1.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        TableLayoutPanel1.ColumnCount = 2
        TableLayoutPanel1.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50F))
        TableLayoutPanel1.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50F))
        TableLayoutPanel1.Controls.Add(OK_Button, 0, 0)
        TableLayoutPanel1.Controls.Add(Cancel_Button, 1, 0)
        TableLayoutPanel1.Location = New Point(58, 509)
        TableLayoutPanel1.Margin = New Padding(4, 3, 4, 3)
        TableLayoutPanel1.Name = "TableLayoutPanel1"
        TableLayoutPanel1.RowCount = 1
        TableLayoutPanel1.RowStyles.Add(New RowStyle(SizeType.Percent, 50F))
        TableLayoutPanel1.Size = New Size(170, 33)
        TableLayoutPanel1.TabIndex = 0
        ' 
        ' OK_Button
        ' 
        OK_Button.Anchor = AnchorStyles.None
        OK_Button.Location = New Point(4, 3)
        OK_Button.Margin = New Padding(4, 3, 4, 3)
        OK_Button.Name = "OK_Button"
        OK_Button.Size = New Size(77, 27)
        OK_Button.TabIndex = 0
        OK_Button.Text = "OK"
        ' 
        ' Cancel_Button
        ' 
        Cancel_Button.Anchor = AnchorStyles.None
        Cancel_Button.Location = New Point(89, 3)
        Cancel_Button.Margin = New Padding(4, 3, 4, 3)
        Cancel_Button.Name = "Cancel_Button"
        Cancel_Button.Size = New Size(77, 27)
        Cancel_Button.TabIndex = 1
        Cancel_Button.Text = "Cancel"
        ' 
        ' Panel1
        ' 
        Panel1.AutoScroll = True
        Panel1.Controls.Add(TextBox1)
        Panel1.Controls.Add(Label1)
        Panel1.Location = New Point(12, 73)
        Panel1.Name = "Panel1"
        Panel1.Size = New Size(216, 416)
        Panel1.TabIndex = 1
        ' 
        ' TextBox1
        ' 
        TextBox1.Location = New Point(34, 3)
        TextBox1.Name = "TextBox1"
        TextBox1.Size = New Size(160, 23)
        TextBox1.TabIndex = 1
        TextBox1.Visible = False
        ' 
        ' Label1
        ' 
        Label1.Location = New Point(3, 6)
        Label1.Name = "Label1"
        Label1.Size = New Size(25, 15)
        Label1.TabIndex = 0
        Label1.Text = "222"
        Label1.Visible = False
        ' 
        ' cboProgNum
        ' 
        cboProgNum.FormattingEnabled = True
        cboProgNum.Location = New Point(87, 5)
        cboProgNum.Name = "cboProgNum"
        cboProgNum.Size = New Size(141, 23)
        cboProgNum.TabIndex = 2
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.Location = New Point(15, 8)
        Label2.Name = "Label2"
        Label2.Size = New Size(66, 15)
        Label2.TabIndex = 3
        Label2.Text = "Program #:"
        ' 
        ' rdoVars
        ' 
        rdoVars.AutoSize = True
        rdoVars.Location = New Point(32, 40)
        rdoVars.Name = "rdoVars"
        rdoVars.Size = New Size(71, 19)
        rdoVars.TabIndex = 4
        rdoVars.TabStop = True
        rdoVars.Text = "Variables"
        rdoVars.UseVisualStyleBackColor = True
        ' 
        ' rdoTimers
        ' 
        rdoTimers.AutoSize = True
        rdoTimers.Location = New Point(135, 40)
        rdoTimers.Name = "rdoTimers"
        rdoTimers.Size = New Size(60, 19)
        rdoTimers.TabIndex = 5
        rdoTimers.TabStop = True
        rdoTimers.Text = "Timers"
        rdoTimers.UseVisualStyleBackColor = True
        ' 
        ' DialogVarNamesAll
        ' 
        AcceptButton = OK_Button
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        CancelButton = Cancel_Button
        ClientSize = New Size(242, 556)
        Controls.Add(rdoTimers)
        Controls.Add(rdoVars)
        Controls.Add(Label2)
        Controls.Add(cboProgNum)
        Controls.Add(Panel1)
        Controls.Add(TableLayoutPanel1)
        FormBorderStyle = FormBorderStyle.FixedDialog
        Margin = New Padding(4, 3, 4, 3)
        MaximizeBox = False
        MinimizeBox = False
        Name = "DialogVarNamesAll"
        ShowInTaskbar = False
        StartPosition = FormStartPosition.CenterParent
        Text = "DialogVarNamesAll"
        TableLayoutPanel1.ResumeLayout(False)
        Panel1.ResumeLayout(False)
        Panel1.PerformLayout()
        ResumeLayout(False)
        PerformLayout()

    End Sub
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents OK_Button As System.Windows.Forms.Button
    Friend WithEvents Cancel_Button As System.Windows.Forms.Button
    Friend WithEvents Panel1 As Panel
    Friend WithEvents TextBox1 As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents cboProgNum As ComboBox
    Friend WithEvents Label2 As Label
    Friend WithEvents rdoVars As RadioButton
    Friend WithEvents rdoTimers As RadioButton

End Class
