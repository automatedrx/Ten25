<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ChannelUserControl
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
        Me.sldSpeed = New System.Windows.Forms.TrackBar()
        Me.lblSpeed = New System.Windows.Forms.Label()
        Me.sldOutputIntensityPct = New System.Windows.Forms.TrackBar()
        Me.cmdEnable = New System.Windows.Forms.Button()
        Me.lblOutputIntensityPct = New System.Windows.Forms.Label()
        Me.lblPulseWidth = New System.Windows.Forms.Label()
        Me.grpPulseWidth = New System.Windows.Forms.GroupBox()
        Me.sldPulseWidth = New System.Windows.Forms.TrackBar()
        Me.grpSpeed = New System.Windows.Forms.GroupBox()
        Me.grpOutputIntensity = New System.Windows.Forms.GroupBox()
        Me.grpProgInfo = New System.Windows.Forms.GroupBox()
        Me.lblRepeatsRemaining = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.lblPolarity = New System.Windows.Forms.Label()
        Me.lblLabelPolarity = New System.Windows.Forms.Label()
        Me.lblLineNum = New System.Windows.Forms.Label()
        Me.lblDuration = New System.Windows.Forms.Label()
        Me.lblLabelDuration = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.lblProgState = New System.Windows.Forms.Label()
        Me.lblCommand = New System.Windows.Forms.Label()
        Me.sbarComplete = New Tens25.sbarUserControl()
        CType(Me.sldSpeed, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.sldOutputIntensityPct, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.grpPulseWidth.SuspendLayout()
        CType(Me.sldPulseWidth, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.grpSpeed.SuspendLayout()
        Me.grpOutputIntensity.SuspendLayout()
        Me.grpProgInfo.SuspendLayout()
        Me.SuspendLayout()
        '
        'sldSpeed
        '
        Me.sldSpeed.AutoSize = False
        Me.sldSpeed.BackColor = System.Drawing.SystemColors.Control
        Me.sldSpeed.LargeChange = 10
        Me.sldSpeed.Location = New System.Drawing.Point(34, 14)
        Me.sldSpeed.Margin = New System.Windows.Forms.Padding(0)
        Me.sldSpeed.Maximum = 100
        Me.sldSpeed.Name = "sldSpeed"
        Me.sldSpeed.Size = New System.Drawing.Size(80, 25)
        Me.sldSpeed.TabIndex = 2
        Me.sldSpeed.TickFrequency = 25
        Me.sldSpeed.Value = 50
        '
        'lblSpeed
        '
        Me.lblSpeed.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblSpeed.Location = New System.Drawing.Point(1, 17)
        Me.lblSpeed.Margin = New System.Windows.Forms.Padding(0)
        Me.lblSpeed.Name = "lblSpeed"
        Me.lblSpeed.Size = New System.Drawing.Size(39, 15)
        Me.lblSpeed.TabIndex = 4
        Me.lblSpeed.Text = "1000%"
        Me.lblSpeed.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'sldOutputIntensityPct
        '
        Me.sldOutputIntensityPct.AutoSize = False
        Me.sldOutputIntensityPct.BackColor = System.Drawing.SystemColors.Control
        Me.sldOutputIntensityPct.Location = New System.Drawing.Point(8, 33)
        Me.sldOutputIntensityPct.Maximum = 100
        Me.sldOutputIntensityPct.Name = "sldOutputIntensityPct"
        Me.sldOutputIntensityPct.Orientation = System.Windows.Forms.Orientation.Vertical
        Me.sldOutputIntensityPct.Size = New System.Drawing.Size(35, 182)
        Me.sldOutputIntensityPct.TabIndex = 5
        Me.sldOutputIntensityPct.TickFrequency = 10
        Me.sldOutputIntensityPct.TickStyle = System.Windows.Forms.TickStyle.TopLeft
        '
        'cmdEnable
        '
        Me.cmdEnable.BackColor = System.Drawing.Color.HotPink
        Me.cmdEnable.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.cmdEnable.Dock = System.Windows.Forms.DockStyle.Top
        Me.cmdEnable.Location = New System.Drawing.Point(0, 0)
        Me.cmdEnable.Name = "cmdEnable"
        Me.cmdEnable.Size = New System.Drawing.Size(177, 23)
        Me.cmdEnable.TabIndex = 6
        Me.cmdEnable.Text = "cmdEnable"
        Me.cmdEnable.UseVisualStyleBackColor = False
        '
        'lblOutputIntensityPct
        '
        Me.lblOutputIntensityPct.Location = New System.Drawing.Point(6, 20)
        Me.lblOutputIntensityPct.Name = "lblOutputIntensityPct"
        Me.lblOutputIntensityPct.Size = New System.Drawing.Size(39, 15)
        Me.lblOutputIntensityPct.TabIndex = 7
        Me.lblOutputIntensityPct.Text = "100%"
        Me.lblOutputIntensityPct.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblPulseWidth
        '
        Me.lblPulseWidth.Location = New System.Drawing.Point(1, 17)
        Me.lblPulseWidth.Name = "lblPulseWidth"
        Me.lblPulseWidth.Size = New System.Drawing.Size(39, 15)
        Me.lblPulseWidth.TabIndex = 8
        Me.lblPulseWidth.Text = "100%"
        Me.lblPulseWidth.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'grpPulseWidth
        '
        Me.grpPulseWidth.Controls.Add(Me.sldPulseWidth)
        Me.grpPulseWidth.Controls.Add(Me.lblPulseWidth)
        Me.grpPulseWidth.Location = New System.Drawing.Point(5, 158)
        Me.grpPulseWidth.Name = "grpPulseWidth"
        Me.grpPulseWidth.Size = New System.Drawing.Size(115, 45)
        Me.grpPulseWidth.TabIndex = 9
        Me.grpPulseWidth.TabStop = False
        Me.grpPulseWidth.Text = "PulseWidth"
        '
        'sldPulseWidth
        '
        Me.sldPulseWidth.AutoSize = False
        Me.sldPulseWidth.BackColor = System.Drawing.SystemColors.Control
        Me.sldPulseWidth.LargeChange = 10
        Me.sldPulseWidth.Location = New System.Drawing.Point(34, 14)
        Me.sldPulseWidth.Maximum = 100
        Me.sldPulseWidth.Name = "sldPulseWidth"
        Me.sldPulseWidth.Size = New System.Drawing.Size(80, 25)
        Me.sldPulseWidth.TabIndex = 3
        Me.sldPulseWidth.TickFrequency = 25
        Me.sldPulseWidth.Value = 50
        '
        'grpSpeed
        '
        Me.grpSpeed.Controls.Add(Me.lblSpeed)
        Me.grpSpeed.Controls.Add(Me.sldSpeed)
        Me.grpSpeed.Location = New System.Drawing.Point(5, 203)
        Me.grpSpeed.Name = "grpSpeed"
        Me.grpSpeed.Size = New System.Drawing.Size(115, 45)
        Me.grpSpeed.TabIndex = 10
        Me.grpSpeed.TabStop = False
        Me.grpSpeed.Text = "Speed"
        '
        'grpOutputIntensity
        '
        Me.grpOutputIntensity.Controls.Add(Me.sldOutputIntensityPct)
        Me.grpOutputIntensity.Controls.Add(Me.lblOutputIntensityPct)
        Me.grpOutputIntensity.Location = New System.Drawing.Point(124, 27)
        Me.grpOutputIntensity.Name = "grpOutputIntensity"
        Me.grpOutputIntensity.Size = New System.Drawing.Size(48, 221)
        Me.grpOutputIntensity.TabIndex = 11
        Me.grpOutputIntensity.TabStop = False
        Me.grpOutputIntensity.Text = "Volts"
        '
        'grpProgInfo
        '
        Me.grpProgInfo.Controls.Add(Me.sbarComplete)
        Me.grpProgInfo.Controls.Add(Me.lblRepeatsRemaining)
        Me.grpProgInfo.Controls.Add(Me.Label3)
        Me.grpProgInfo.Controls.Add(Me.lblPolarity)
        Me.grpProgInfo.Controls.Add(Me.lblLabelPolarity)
        Me.grpProgInfo.Controls.Add(Me.lblLineNum)
        Me.grpProgInfo.Controls.Add(Me.lblDuration)
        Me.grpProgInfo.Controls.Add(Me.lblLabelDuration)
        Me.grpProgInfo.Controls.Add(Me.Label2)
        Me.grpProgInfo.Location = New System.Drawing.Point(5, 63)
        Me.grpProgInfo.Name = "grpProgInfo"
        Me.grpProgInfo.Size = New System.Drawing.Size(115, 94)
        Me.grpProgInfo.TabIndex = 12
        Me.grpProgInfo.TabStop = False
        Me.grpProgInfo.Text = "Program Info"
        '
        'lblRepeatsRemaining
        '
        Me.lblRepeatsRemaining.ImageAlign = System.Drawing.ContentAlignment.TopLeft
        Me.lblRepeatsRemaining.Location = New System.Drawing.Point(63, 42)
        Me.lblRepeatsRemaining.Name = "lblRepeatsRemaining"
        Me.lblRepeatsRemaining.Size = New System.Drawing.Size(48, 15)
        Me.lblRepeatsRemaining.TabIndex = 17
        Me.lblRepeatsRemaining.Text = "200"
        Me.lblRepeatsRemaining.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(7, 43)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(52, 13)
        Me.Label3.TabIndex = 16
        Me.Label3.Text = "Rpt Rem:"
        '
        'lblPolarity
        '
        Me.lblPolarity.ImageAlign = System.Drawing.ContentAlignment.TopLeft
        Me.lblPolarity.Location = New System.Drawing.Point(62, 57)
        Me.lblPolarity.Name = "lblPolarity"
        Me.lblPolarity.Size = New System.Drawing.Size(48, 15)
        Me.lblPolarity.TabIndex = 15
        Me.lblPolarity.Text = "Tog Pul"
        Me.lblPolarity.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblLabelPolarity
        '
        Me.lblLabelPolarity.AutoSize = True
        Me.lblLabelPolarity.Location = New System.Drawing.Point(6, 58)
        Me.lblLabelPolarity.Name = "lblLabelPolarity"
        Me.lblLabelPolarity.Size = New System.Drawing.Size(44, 13)
        Me.lblLabelPolarity.TabIndex = 14
        Me.lblLabelPolarity.Text = "Polarity:"
        '
        'lblLineNum
        '
        Me.lblLineNum.ImageAlign = System.Drawing.ContentAlignment.TopLeft
        Me.lblLineNum.Location = New System.Drawing.Point(61, 16)
        Me.lblLineNum.Name = "lblLineNum"
        Me.lblLineNum.Size = New System.Drawing.Size(48, 15)
        Me.lblLineNum.TabIndex = 13
        Me.lblLineNum.Text = "200000"
        Me.lblLineNum.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblDuration
        '
        Me.lblDuration.ImageAlign = System.Drawing.ContentAlignment.TopLeft
        Me.lblDuration.Location = New System.Drawing.Point(62, 29)
        Me.lblDuration.Name = "lblDuration"
        Me.lblDuration.Size = New System.Drawing.Size(48, 15)
        Me.lblDuration.TabIndex = 10
        Me.lblDuration.Text = "200000"
        Me.lblDuration.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblLabelDuration
        '
        Me.lblLabelDuration.AutoSize = True
        Me.lblLabelDuration.Location = New System.Drawing.Point(6, 30)
        Me.lblLabelDuration.Name = "lblLabelDuration"
        Me.lblLabelDuration.Size = New System.Drawing.Size(50, 13)
        Me.lblLabelDuration.TabIndex = 4
        Me.lblLabelDuration.Text = "Duration:"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(5, 17)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(55, 13)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "Line Num:"
        '
        'lblProgState
        '
        Me.lblProgState.Location = New System.Drawing.Point(11, 27)
        Me.lblProgState.Name = "lblProgState"
        Me.lblProgState.Size = New System.Drawing.Size(97, 15)
        Me.lblProgState.TabIndex = 8
        Me.lblProgState.Text = "Unknown"
        Me.lblProgState.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblCommand
        '
        Me.lblCommand.Location = New System.Drawing.Point(9, 43)
        Me.lblCommand.Name = "lblCommand"
        Me.lblCommand.Size = New System.Drawing.Size(99, 15)
        Me.lblCommand.TabIndex = 16
        Me.lblCommand.Text = "Motor Output"
        Me.lblCommand.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'sbarComplete
        '
        Me.sbarComplete.Location = New System.Drawing.Point(6, 74)
        Me.sbarComplete.MaxValue = 50
        Me.sbarComplete.MinMaxVisible = True
        Me.sbarComplete.MinValue = 0
        Me.sbarComplete.Name = "sbarComplete"
        Me.sbarComplete.PercentComplete = -1
        Me.sbarComplete.Size = New System.Drawing.Size(107, 15)
        Me.sbarComplete.TabIndex = 17
        '
        'ChannelUserControl
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.Control
        Me.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Controls.Add(Me.lblCommand)
        Me.Controls.Add(Me.grpProgInfo)
        Me.Controls.Add(Me.grpOutputIntensity)
        Me.Controls.Add(Me.grpSpeed)
        Me.Controls.Add(Me.grpPulseWidth)
        Me.Controls.Add(Me.cmdEnable)
        Me.Controls.Add(Me.lblProgState)
        Me.Name = "ChannelUserControl"
        Me.Size = New System.Drawing.Size(177, 252)
        CType(Me.sldSpeed, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.sldOutputIntensityPct, System.ComponentModel.ISupportInitialize).EndInit()
        Me.grpPulseWidth.ResumeLayout(False)
        CType(Me.sldPulseWidth, System.ComponentModel.ISupportInitialize).EndInit()
        Me.grpSpeed.ResumeLayout(False)
        Me.grpOutputIntensity.ResumeLayout(False)
        Me.grpProgInfo.ResumeLayout(False)
        Me.grpProgInfo.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents sldSpeed As TrackBar
    Friend WithEvents lblSpeed As Label
    Friend WithEvents sldOutputIntensityPct As TrackBar
    Friend WithEvents cmdEnable As Button
    Friend WithEvents lblOutputIntensityPct As Label
    Friend WithEvents lblPulseWidth As Label
    Friend WithEvents grpPulseWidth As GroupBox
    Friend WithEvents sldPulseWidth As TrackBar
    Friend WithEvents grpSpeed As GroupBox
    Friend WithEvents grpOutputIntensity As GroupBox
    Friend WithEvents grpProgInfo As GroupBox
    Friend WithEvents lblProgState As Label
    Friend WithEvents lblLabelDuration As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents lblLineNum As Label
    Friend WithEvents lblDuration As Label
    Friend WithEvents lblPolarity As Label
    Friend WithEvents lblLabelPolarity As Label
    Friend WithEvents lblCommand As Label
    Friend WithEvents lblRepeatsRemaining As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents sbarComplete As sbarUserControl
End Class
