<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class sbarUserControl
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
        Me.lblMin = New System.Windows.Forms.Label()
        Me.lblMax = New System.Windows.Forms.Label()
        Me.pnlBackgroundBar = New System.Windows.Forms.Panel()
        Me.lblForegroundBar = New System.Windows.Forms.Label()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.lblLeftMarker = New System.Windows.Forms.Label()
        Me.lblRightMarker = New System.Windows.Forms.Label()
        Me.lblCurVal = New System.Windows.Forms.Label()
        Me.pnlBackgroundBar.SuspendLayout()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'lblMin
        '
        Me.lblMin.AutoSize = True
        Me.lblMin.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblMin.Location = New System.Drawing.Point(3, 0)
        Me.lblMin.Name = "lblMin"
        Me.lblMin.Size = New System.Drawing.Size(30, 15)
        Me.lblMin.TabIndex = 0
        Me.lblMin.Text = "100"
        Me.lblMin.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblMax
        '
        Me.lblMax.AutoSize = True
        Me.lblMax.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblMax.Location = New System.Drawing.Point(74, 0)
        Me.lblMax.Name = "lblMax"
        Me.lblMax.Size = New System.Drawing.Size(30, 15)
        Me.lblMax.TabIndex = 1
        Me.lblMax.Text = "100"
        Me.lblMax.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'pnlBackgroundBar
        '
        Me.pnlBackgroundBar.BackColor = System.Drawing.SystemColors.ControlLight
        Me.pnlBackgroundBar.Controls.Add(Me.lblRightMarker)
        Me.pnlBackgroundBar.Controls.Add(Me.lblLeftMarker)
        Me.pnlBackgroundBar.Controls.Add(Me.lblForegroundBar)
        Me.pnlBackgroundBar.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlBackgroundBar.Location = New System.Drawing.Point(0, 0)
        Me.pnlBackgroundBar.Name = "pnlBackgroundBar"
        Me.pnlBackgroundBar.Size = New System.Drawing.Size(107, 3)
        Me.pnlBackgroundBar.TabIndex = 4
        '
        'lblForegroundBar
        '
        Me.lblForegroundBar.BackColor = System.Drawing.SystemColors.HotTrack
        Me.lblForegroundBar.Location = New System.Drawing.Point(20, 0)
        Me.lblForegroundBar.Margin = New System.Windows.Forms.Padding(0)
        Me.lblForegroundBar.Name = "lblForegroundBar"
        Me.lblForegroundBar.Size = New System.Drawing.Size(20, 3)
        Me.lblForegroundBar.TabIndex = 0
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 3
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 35.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.lblCurVal, 1, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.lblMax, 2, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.lblMin, 0, 0)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 3)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 1
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(107, 15)
        Me.TableLayoutPanel1.TabIndex = 5
        '
        'lblLeftMarker
        '
        Me.lblLeftMarker.BackColor = System.Drawing.SystemColors.ControlDarkDark
        Me.lblLeftMarker.Location = New System.Drawing.Point(0, 0)
        Me.lblLeftMarker.Name = "lblLeftMarker"
        Me.lblLeftMarker.Size = New System.Drawing.Size(3, 3)
        Me.lblLeftMarker.TabIndex = 1
        '
        'lblRightMarker
        '
        Me.lblRightMarker.BackColor = System.Drawing.SystemColors.ControlDarkDark
        Me.lblRightMarker.Location = New System.Drawing.Point(104, 0)
        Me.lblRightMarker.Name = "lblRightMarker"
        Me.lblRightMarker.Size = New System.Drawing.Size(3, 3)
        Me.lblRightMarker.TabIndex = 2
        '
        'lblCurVal
        '
        Me.lblCurVal.AutoSize = True
        Me.lblCurVal.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblCurVal.Location = New System.Drawing.Point(39, 0)
        Me.lblCurVal.Name = "lblCurVal"
        Me.lblCurVal.Size = New System.Drawing.Size(29, 15)
        Me.lblCurVal.TabIndex = 2
        Me.lblCurVal.Text = "100"
        Me.lblCurVal.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'sbarUserControl
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Controls.Add(Me.pnlBackgroundBar)
        Me.Name = "sbarUserControl"
        Me.Size = New System.Drawing.Size(107, 19)
        Me.pnlBackgroundBar.ResumeLayout(False)
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents lblMin As Label
    Friend WithEvents lblMax As Label
    Friend WithEvents pnlBackgroundBar As Panel
    Friend WithEvents lblForegroundBar As Label
    Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
    Friend WithEvents lblLeftMarker As Label
    Friend WithEvents lblRightMarker As Label
    Friend WithEvents lblCurVal As Label
End Class
