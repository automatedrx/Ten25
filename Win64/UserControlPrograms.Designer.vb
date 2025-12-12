<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class UserControlPrograms
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.pnlProg = New System.Windows.Forms.Panel()
        Me.contextProgLineList = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.tsAddRow = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsInsertRow = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsDeleteRow = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsMoveRowUp = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsMoveRowDown = New System.Windows.Forms.ToolStripMenuItem()
        Me.UserControlProgLine21 = New Tens25.UserControlProgLine2()
        Me.pnlProg.SuspendLayout()
        Me.contextProgLineList.SuspendLayout()
        Me.SuspendLayout()
        '
        'pnlProg
        '
        Me.pnlProg.AutoScroll = True
        Me.pnlProg.Controls.Add(Me.UserControlProgLine21)
        Me.pnlProg.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlProg.Location = New System.Drawing.Point(0, 0)
        Me.pnlProg.Name = "pnlProg"
        Me.pnlProg.Size = New System.Drawing.Size(689, 320)
        Me.pnlProg.TabIndex = 3
        '
        'contextProgLineList
        '
        Me.contextProgLineList.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsAddRow, Me.tsInsertRow, Me.tsDeleteRow, Me.tsMoveRowUp, Me.tsMoveRowDown})
        Me.contextProgLineList.Name = "contextProgLineList"
        Me.contextProgLineList.Size = New System.Drawing.Size(266, 114)
        '
        'tsAddRow
        '
        Me.tsAddRow.Name = "tsAddRow"
        Me.tsAddRow.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.[End]), System.Windows.Forms.Keys)
        Me.tsAddRow.Size = New System.Drawing.Size(265, 22)
        Me.tsAddRow.Text = "Add New Row to End"
        '
        'tsInsertRow
        '
        Me.tsInsertRow.Name = "tsInsertRow"
        Me.tsInsertRow.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Insert), System.Windows.Forms.Keys)
        Me.tsInsertRow.Size = New System.Drawing.Size(265, 22)
        Me.tsInsertRow.Text = "Insert New Row"
        '
        'tsDeleteRow
        '
        Me.tsDeleteRow.Name = "tsDeleteRow"
        Me.tsDeleteRow.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Delete), System.Windows.Forms.Keys)
        Me.tsDeleteRow.Size = New System.Drawing.Size(265, 22)
        Me.tsDeleteRow.Text = "Delete Active Row"
        '
        'tsMoveRowUp
        '
        Me.tsMoveRowUp.Name = "tsMoveRowUp"
        Me.tsMoveRowUp.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Up), System.Windows.Forms.Keys)
        Me.tsMoveRowUp.Size = New System.Drawing.Size(265, 22)
        Me.tsMoveRowUp.Text = "Move Active Row Up"
        '
        'tsMoveRowDown
        '
        Me.tsMoveRowDown.Name = "tsMoveRowDown"
        Me.tsMoveRowDown.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Down), System.Windows.Forms.Keys)
        Me.tsMoveRowDown.Size = New System.Drawing.Size(265, 22)
        Me.tsMoveRowDown.Text = "Move Active Row Down"
        '
        'UserControlProgLine21
        '
        Me.UserControlProgLine21.ChannelNum = -1
        Me.UserControlProgLine21.Command = 0
        Me.UserControlProgLine21.GoToFalse = -1
        Me.UserControlProgLine21.GoToTrue = -1
        Me.UserControlProgLine21.LineNum = -1
        Me.UserControlProgLine21.Location = New System.Drawing.Point(0, 0)
        Me.UserControlProgLine21.Name = "UserControlProgLine21"
        Me.UserControlProgLine21.P321S = 0
        Me.UserControlProgLine21.P321V = 0
        Me.UserControlProgLine21.P322S = 0
        Me.UserControlProgLine21.P322V = 0
        Me.UserControlProgLine21.P81S = 0
        Me.UserControlProgLine21.P81V = 255
        Me.UserControlProgLine21.P82S = 0
        Me.UserControlProgLine21.P82V = 0
        Me.UserControlProgLine21.Polarity = 0
        Me.UserControlProgLine21.Size = New System.Drawing.Size(670, 250)
        Me.UserControlProgLine21.TabIndex = 1
        Me.UserControlProgLine21.UnsavedChanges = True
        '
        'UserControlPrograms
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Controls.Add(Me.pnlProg)
        Me.MinimumSize = New System.Drawing.Size(689, 30)
        Me.Name = "UserControlPrograms"
        Me.Size = New System.Drawing.Size(689, 320)
        Me.pnlProg.ResumeLayout(False)
        Me.contextProgLineList.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents pnlProg As Panel
    Friend WithEvents contextProgLineList As ContextMenuStrip
    Friend WithEvents tsAddRow As ToolStripMenuItem
    Friend WithEvents tsInsertRow As ToolStripMenuItem
    Friend WithEvents tsDeleteRow As ToolStripMenuItem
    Friend WithEvents tsMoveRowUp As ToolStripMenuItem
    Friend WithEvents tsMoveRowDown As ToolStripMenuItem
    Friend WithEvents UserControlProgLine21 As UserControlProgLine2
End Class
