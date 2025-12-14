<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ucSBar
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
        TableLayoutPanel1 = New TableLayoutPanel()
        lblMax = New Label()
        lblCurVal = New Label()
        lblMin = New Label()
        pnlBackgroundBar = New Panel()
        lblForegroundBar = New Label()
        lblLeftMarker = New Label()
        lblRightMarker = New Label()
        TableLayoutPanel1.SuspendLayout()
        pnlBackgroundBar.SuspendLayout()
        SuspendLayout()
        ' 
        ' TableLayoutPanel1
        ' 
        TableLayoutPanel1.ColumnCount = 3
        TableLayoutPanel1.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50F))
        TableLayoutPanel1.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 35F))
        TableLayoutPanel1.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50F))
        TableLayoutPanel1.Controls.Add(lblMax, 2, 0)
        TableLayoutPanel1.Controls.Add(lblCurVal, 1, 0)
        TableLayoutPanel1.Controls.Add(lblMin, 0, 0)
        TableLayoutPanel1.Location = New Point(0, 3)
        TableLayoutPanel1.Name = "TableLayoutPanel1"
        TableLayoutPanel1.RowCount = 1
        TableLayoutPanel1.RowStyles.Add(New RowStyle(SizeType.Percent, 100F))
        TableLayoutPanel1.Size = New Size(107, 15)
        TableLayoutPanel1.TabIndex = 0
        ' 
        ' lblMax
        ' 
        lblMax.AutoSize = True
        lblMax.Dock = DockStyle.Fill
        lblMax.Font = New Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        lblMax.Location = New Point(74, 0)
        lblMax.Name = "lblMax"
        lblMax.Size = New Size(30, 15)
        lblMax.TabIndex = 2
        lblMax.Text = "100"
        lblMax.TextAlign = ContentAlignment.MiddleCenter
        ' 
        ' lblCurVal
        ' 
        lblCurVal.AutoSize = True
        lblCurVal.Dock = DockStyle.Fill
        lblCurVal.Font = New Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        lblCurVal.Location = New Point(39, 0)
        lblCurVal.Name = "lblCurVal"
        lblCurVal.Size = New Size(29, 15)
        lblCurVal.TabIndex = 1
        lblCurVal.Text = "100"
        lblCurVal.TextAlign = ContentAlignment.MiddleCenter
        ' 
        ' lblMin
        ' 
        lblMin.AutoSize = True
        lblMin.Dock = DockStyle.Fill
        lblMin.Font = New Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        lblMin.Location = New Point(3, 0)
        lblMin.Name = "lblMin"
        lblMin.Size = New Size(30, 15)
        lblMin.TabIndex = 0
        lblMin.Text = "100"
        lblMin.TextAlign = ContentAlignment.MiddleCenter
        ' 
        ' pnlBackgroundBar
        ' 
        pnlBackgroundBar.BackColor = SystemColors.ControlLight
        pnlBackgroundBar.Controls.Add(lblRightMarker)
        pnlBackgroundBar.Controls.Add(lblLeftMarker)
        pnlBackgroundBar.Controls.Add(lblForegroundBar)
        pnlBackgroundBar.Dock = DockStyle.Top
        pnlBackgroundBar.Location = New Point(0, 0)
        pnlBackgroundBar.Name = "pnlBackgroundBar"
        pnlBackgroundBar.Size = New Size(107, 3)
        pnlBackgroundBar.TabIndex = 1
        ' 
        ' lblForegroundBar
        ' 
        lblForegroundBar.BackColor = SystemColors.HotTrack
        lblForegroundBar.Location = New Point(20, 0)
        lblForegroundBar.Margin = New Padding(0)
        lblForegroundBar.Name = "lblForegroundBar"
        lblForegroundBar.Size = New Size(20, 3)
        lblForegroundBar.TabIndex = 0
        ' 
        ' lblLeftMarker
        ' 
        lblLeftMarker.BackColor = SystemColors.ControlDarkDark
        lblLeftMarker.Location = New Point(0, 0)
        lblLeftMarker.Name = "lblLeftMarker"
        lblLeftMarker.Size = New Size(3, 3)
        lblLeftMarker.TabIndex = 1
        ' 
        ' lblRightMarker
        ' 
        lblRightMarker.BackColor = SystemColors.ControlDarkDark
        lblRightMarker.Location = New Point(104, 0)
        lblRightMarker.Name = "lblRightMarker"
        lblRightMarker.Size = New Size(3, 3)
        lblRightMarker.TabIndex = 2
        ' 
        ' ucSBar
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        Controls.Add(pnlBackgroundBar)
        Controls.Add(TableLayoutPanel1)
        Name = "ucSBar"
        Size = New Size(107, 19)
        TableLayoutPanel1.ResumeLayout(False)
        TableLayoutPanel1.PerformLayout()
        pnlBackgroundBar.ResumeLayout(False)
        ResumeLayout(False)
    End Sub

    Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
    Friend WithEvents lblMin As Label
    Friend WithEvents pnlBackgroundBar As Panel
    Friend WithEvents lblMax As Label
    Friend WithEvents lblCurVal As Label
    Friend WithEvents lblForegroundBar As Label
    Friend WithEvents lblRightMarker As Label
    Friend WithEvents lblLeftMarker As Label

End Class
