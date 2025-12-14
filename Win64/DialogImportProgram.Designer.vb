<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class dialogImportProgram
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
        lstSource = New ListBox()
        lstTarget = New ListBox()
        SplitContainer1 = New SplitContainer()
        Label1 = New Label()
        Label2 = New Label()
        Label3 = New Label()
        lblSourceName = New Label()
        lblTargetName = New Label()
        Label6 = New Label()
        TableLayoutPanel1.SuspendLayout()
        CType(SplitContainer1, ComponentModel.ISupportInitialize).BeginInit()
        SplitContainer1.Panel1.SuspendLayout()
        SplitContainer1.Panel2.SuspendLayout()
        SplitContainer1.SuspendLayout()
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
        TableLayoutPanel1.Location = New Point(557, 666)
        TableLayoutPanel1.Margin = New Padding(7, 6, 7, 6)
        TableLayoutPanel1.Name = "TableLayoutPanel1"
        TableLayoutPanel1.RowCount = 1
        TableLayoutPanel1.RowStyles.Add(New RowStyle(SizeType.Percent, 50F))
        TableLayoutPanel1.Size = New Size(316, 70)
        TableLayoutPanel1.TabIndex = 0
        ' 
        ' OK_Button
        ' 
        OK_Button.Anchor = AnchorStyles.None
        OK_Button.Enabled = False
        OK_Button.Location = New Point(7, 6)
        OK_Button.Margin = New Padding(7, 6, 7, 6)
        OK_Button.Name = "OK_Button"
        OK_Button.Size = New Size(143, 58)
        OK_Button.TabIndex = 0
        OK_Button.Text = "OK"
        ' 
        ' Cancel_Button
        ' 
        Cancel_Button.Anchor = AnchorStyles.None
        Cancel_Button.Location = New Point(165, 6)
        Cancel_Button.Margin = New Padding(7, 6, 7, 6)
        Cancel_Button.Name = "Cancel_Button"
        Cancel_Button.Size = New Size(143, 58)
        Cancel_Button.TabIndex = 1
        Cancel_Button.Text = "Cancel"
        ' 
        ' lstSource
        ' 
        lstSource.Dock = DockStyle.Bottom
        lstSource.FormattingEnabled = True
        lstSource.Location = New Point(0, 111)
        lstSource.Margin = New Padding(6)
        lstSource.Name = "lstSource"
        lstSource.Size = New Size(428, 420)
        lstSource.TabIndex = 1
        ' 
        ' lstTarget
        ' 
        lstTarget.Dock = DockStyle.Bottom
        lstTarget.FormattingEnabled = True
        lstTarget.Location = New Point(0, 111)
        lstTarget.Margin = New Padding(6)
        lstTarget.Name = "lstTarget"
        lstTarget.Size = New Size(430, 420)
        lstTarget.TabIndex = 2
        ' 
        ' SplitContainer1
        ' 
        SplitContainer1.BorderStyle = BorderStyle.FixedSingle
        SplitContainer1.Dock = DockStyle.Top
        SplitContainer1.IsSplitterFixed = True
        SplitContainer1.Location = New Point(0, 0)
        SplitContainer1.Margin = New Padding(6)
        SplitContainer1.Name = "SplitContainer1"
        ' 
        ' SplitContainer1.Panel1
        ' 
        SplitContainer1.Panel1.Controls.Add(Label1)
        SplitContainer1.Panel1.Controls.Add(lstSource)
        ' 
        ' SplitContainer1.Panel2
        ' 
        SplitContainer1.Panel2.Controls.Add(Label2)
        SplitContainer1.Panel2.Controls.Add(lstTarget)
        SplitContainer1.Size = New Size(899, 533)
        SplitContainer1.SplitterDistance = 430
        SplitContainer1.SplitterWidth = 37
        SplitContainer1.TabIndex = 3
        ' 
        ' Label1
        ' 
        Label1.Location = New Point(6, 19)
        Label1.Margin = New Padding(6, 0, 6, 0)
        Label1.Name = "Label1"
        Label1.Size = New Size(420, 70)
        Label1.TabIndex = 2
        Label1.Text = "Import this program from the selected project:"
        Label1.TextAlign = ContentAlignment.TopCenter
        ' 
        ' Label2
        ' 
        Label2.Location = New Point(6, 19)
        Label2.Margin = New Padding(6, 0, 6, 0)
        Label2.Name = "Label2"
        Label2.Size = New Size(420, 70)
        Label2.TabIndex = 3
        Label2.Text = "And place it before the selected program in the current project:"
        Label2.TextAlign = ContentAlignment.TopCenter
        ' 
        ' Label3
        ' 
        Label3.Location = New Point(0, 558)
        Label3.Margin = New Padding(6, 0, 6, 0)
        Label3.Name = "Label3"
        Label3.Size = New Size(431, 41)
        Label3.TabIndex = 4
        Label3.Text = "Importing From:"
        Label3.TextAlign = ContentAlignment.MiddleCenter
        ' 
        ' lblSourceName
        ' 
        lblSourceName.Location = New Point(0, 599)
        lblSourceName.Margin = New Padding(6, 0, 6, 0)
        lblSourceName.Name = "lblSourceName"
        lblSourceName.Size = New Size(431, 41)
        lblSourceName.TabIndex = 5
        lblSourceName.Text = "lblSourceName"
        lblSourceName.TextAlign = ContentAlignment.MiddleCenter
        ' 
        ' lblTargetName
        ' 
        lblTargetName.Location = New Point(468, 599)
        lblTargetName.Margin = New Padding(6, 0, 6, 0)
        lblTargetName.Name = "lblTargetName"
        lblTargetName.Size = New Size(431, 41)
        lblTargetName.TabIndex = 7
        lblTargetName.Text = "lblTargetName"
        lblTargetName.TextAlign = ContentAlignment.MiddleCenter
        ' 
        ' Label6
        ' 
        Label6.Location = New Point(468, 558)
        Label6.Margin = New Padding(6, 0, 6, 0)
        Label6.Name = "Label6"
        Label6.Size = New Size(431, 41)
        Label6.TabIndex = 6
        Label6.Text = "Importing To:"
        Label6.TextAlign = ContentAlignment.MiddleCenter
        ' 
        ' dialogImportProgram
        ' 
        AcceptButton = OK_Button
        AutoScaleDimensions = New SizeF(13F, 32F)
        AutoScaleMode = AutoScaleMode.Font
        CancelButton = Cancel_Button
        ClientSize = New Size(899, 766)
        ControlBox = False
        Controls.Add(lblTargetName)
        Controls.Add(Label6)
        Controls.Add(lblSourceName)
        Controls.Add(Label3)
        Controls.Add(SplitContainer1)
        Controls.Add(TableLayoutPanel1)
        FormBorderStyle = FormBorderStyle.FixedDialog
        Margin = New Padding(7, 6, 7, 6)
        MaximizeBox = False
        MinimizeBox = False
        Name = "dialogImportProgram"
        ShowInTaskbar = False
        StartPosition = FormStartPosition.CenterParent
        Text = "Import Program from another Project"
        TableLayoutPanel1.ResumeLayout(False)
        SplitContainer1.Panel1.ResumeLayout(False)
        SplitContainer1.Panel2.ResumeLayout(False)
        CType(SplitContainer1, ComponentModel.ISupportInitialize).EndInit()
        SplitContainer1.ResumeLayout(False)
        ResumeLayout(False)

    End Sub
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents OK_Button As System.Windows.Forms.Button
    Friend WithEvents Cancel_Button As System.Windows.Forms.Button
    Friend WithEvents lstSource As ListBox
    Friend WithEvents lstTarget As ListBox
    Friend WithEvents SplitContainer1 As SplitContainer
    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents lblSourceName As Label
    Friend WithEvents lblTargetName As Label
    Friend WithEvents Label6 As Label

End Class
