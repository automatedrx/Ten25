<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ucChanControl
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
        cmdEnable = New Button()
        lblProgName = New Label()
        lblProgState = New Label()
        lblCommand = New Label()
        grpIntensity = New GroupBox()
        cboIntensityMax = New ComboBox()
        Label2 = New Label()
        cboIntensityMin = New ComboBox()
        Label1 = New Label()
        grpOutputIntensity = New GroupBox()
        sldOutputIntensityPct = New TrackBar()
        lblOutputIntensityPct = New Label()
        grpProgInfo = New GroupBox()
        pnlPolarity = New Panel()
        lblPolarityLabel = New Label()
        lblPolarityVal = New Label()
        sbarComplete = New ucSBar()
        lblRepeatsRemaining = New Label()
        Label7 = New Label()
        lblDuration = New Label()
        Label5 = New Label()
        lblLineNum = New Label()
        Label3 = New Label()
        grpSpeed = New GroupBox()
        sldSpeed = New TrackBar()
        lblSpeed = New Label()
        grpPulseWidth = New GroupBox()
        sldPulseWidth = New TrackBar()
        lblPulseWidth = New Label()
        grpIntensity.SuspendLayout()
        grpOutputIntensity.SuspendLayout()
        CType(sldOutputIntensityPct, ComponentModel.ISupportInitialize).BeginInit()
        grpProgInfo.SuspendLayout()
        pnlPolarity.SuspendLayout()
        grpSpeed.SuspendLayout()
        CType(sldSpeed, ComponentModel.ISupportInitialize).BeginInit()
        grpPulseWidth.SuspendLayout()
        CType(sldPulseWidth, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' cmdEnable
        ' 
        cmdEnable.BackColor = Color.HotPink
        cmdEnable.BackgroundImageLayout = ImageLayout.None
        cmdEnable.Dock = DockStyle.Top
        cmdEnable.Location = New Point(0, 0)
        cmdEnable.Name = "cmdEnable"
        cmdEnable.Size = New Size(177, 23)
        cmdEnable.TabIndex = 0
        cmdEnable.Text = "cmdEnable"
        cmdEnable.UseVisualStyleBackColor = False
        ' 
        ' lblProgName
        ' 
        lblProgName.Font = New Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0)
        lblProgName.Location = New Point(5, 23)
        lblProgName.Name = "lblProgName"
        lblProgName.Size = New Size(119, 15)
        lblProgName.TabIndex = 1
        lblProgName.Text = "Unknown"
        lblProgName.TextAlign = ContentAlignment.MiddleCenter
        ' 
        ' lblProgState
        ' 
        lblProgState.Font = New Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0)
        lblProgState.Location = New Point(5, 39)
        lblProgState.Name = "lblProgState"
        lblProgState.Size = New Size(119, 15)
        lblProgState.TabIndex = 2
        lblProgState.Text = "Unknown"
        lblProgState.TextAlign = ContentAlignment.MiddleCenter
        ' 
        ' lblCommand
        ' 
        lblCommand.Font = New Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0)
        lblCommand.Location = New Point(5, 55)
        lblCommand.Name = "lblCommand"
        lblCommand.Size = New Size(119, 15)
        lblCommand.TabIndex = 3
        lblCommand.Text = "Motor Output"
        lblCommand.TextAlign = ContentAlignment.MiddleCenter
        ' 
        ' grpIntensity
        ' 
        grpIntensity.Controls.Add(cboIntensityMax)
        grpIntensity.Controls.Add(Label2)
        grpIntensity.Controls.Add(cboIntensityMin)
        grpIntensity.Controls.Add(Label1)
        grpIntensity.Location = New Point(124, 23)
        grpIntensity.Name = "grpIntensity"
        grpIntensity.Size = New Size(48, 93)
        grpIntensity.TabIndex = 4
        grpIntensity.TabStop = False
        grpIntensity.Text = "PWR"
        ' 
        ' cboIntensityMax
        ' 
        cboIntensityMax.FormattingEnabled = True
        cboIntensityMax.Location = New Point(3, 63)
        cboIntensityMax.Name = "cboIntensityMax"
        cboIntensityMax.Size = New Size(42, 23)
        cboIntensityMax.TabIndex = 3
        cboIntensityMax.Text = "100"
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.Font = New Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0)
        Label2.Location = New Point(6, 51)
        Label2.Name = "Label2"
        Label2.Size = New Size(28, 13)
        Label2.TabIndex = 2
        Label2.Text = "Max"
        ' 
        ' cboIntensityMin
        ' 
        cboIntensityMin.FormattingEnabled = True
        cboIntensityMin.Location = New Point(3, 27)
        cboIntensityMin.Name = "cboIntensityMin"
        cboIntensityMin.Size = New Size(42, 23)
        cboIntensityMin.TabIndex = 1
        cboIntensityMin.Text = "100"
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Font = New Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0)
        Label1.Location = New Point(6, 14)
        Label1.Name = "Label1"
        Label1.Size = New Size(27, 13)
        Label1.TabIndex = 0
        Label1.Text = "Min"
        ' 
        ' grpOutputIntensity
        ' 
        grpOutputIntensity.Controls.Add(sldOutputIntensityPct)
        grpOutputIntensity.Controls.Add(lblOutputIntensityPct)
        grpOutputIntensity.Location = New Point(124, 118)
        grpOutputIntensity.Name = "grpOutputIntensity"
        grpOutputIntensity.Size = New Size(48, 151)
        grpOutputIntensity.TabIndex = 5
        grpOutputIntensity.TabStop = False
        grpOutputIntensity.Text = "Volts"
        ' 
        ' sldOutputIntensityPct
        ' 
        sldOutputIntensityPct.Location = New Point(8, 33)
        sldOutputIntensityPct.Maximum = 100
        sldOutputIntensityPct.Name = "sldOutputIntensityPct"
        sldOutputIntensityPct.Orientation = Orientation.Vertical
        sldOutputIntensityPct.Size = New Size(45, 114)
        sldOutputIntensityPct.TabIndex = 9
        sldOutputIntensityPct.TickFrequency = 10
        sldOutputIntensityPct.TickStyle = TickStyle.TopLeft
        ' 
        ' lblOutputIntensityPct
        ' 
        lblOutputIntensityPct.Font = New Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0)
        lblOutputIntensityPct.Location = New Point(6, 20)
        lblOutputIntensityPct.Margin = New Padding(0)
        lblOutputIntensityPct.Name = "lblOutputIntensityPct"
        lblOutputIntensityPct.Size = New Size(40, 15)
        lblOutputIntensityPct.TabIndex = 8
        lblOutputIntensityPct.Text = "100%"
        lblOutputIntensityPct.TextAlign = ContentAlignment.MiddleCenter
        ' 
        ' grpProgInfo
        ' 
        grpProgInfo.Controls.Add(pnlPolarity)
        grpProgInfo.Controls.Add(sbarComplete)
        grpProgInfo.Controls.Add(lblRepeatsRemaining)
        grpProgInfo.Controls.Add(Label7)
        grpProgInfo.Controls.Add(lblDuration)
        grpProgInfo.Controls.Add(Label5)
        grpProgInfo.Controls.Add(lblLineNum)
        grpProgInfo.Controls.Add(Label3)
        grpProgInfo.Location = New Point(4, 73)
        grpProgInfo.Name = "grpProgInfo"
        grpProgInfo.Size = New Size(115, 101)
        grpProgInfo.TabIndex = 6
        grpProgInfo.TabStop = False
        grpProgInfo.Text = "Program Info"
        ' 
        ' pnlPolarity
        ' 
        pnlPolarity.Controls.Add(lblPolarityLabel)
        pnlPolarity.Controls.Add(lblPolarityVal)
        pnlPolarity.Location = New Point(0, 60)
        pnlPolarity.Margin = New Padding(0)
        pnlPolarity.Name = "pnlPolarity"
        pnlPolarity.Size = New Size(112, 17)
        pnlPolarity.TabIndex = 9
        ' 
        ' lblPolarityLabel
        ' 
        lblPolarityLabel.AutoSize = True
        lblPolarityLabel.BackColor = SystemColors.Control
        lblPolarityLabel.Font = New Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0)
        lblPolarityLabel.ForeColor = SystemColors.ControlText
        lblPolarityLabel.Location = New Point(5, 2)
        lblPolarityLabel.Name = "lblPolarityLabel"
        lblPolarityLabel.Size = New Size(48, 13)
        lblPolarityLabel.TabIndex = 6
        lblPolarityLabel.Text = "Polarity:"
        ' 
        ' lblPolarityVal
        ' 
        lblPolarityVal.Font = New Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0)
        lblPolarityVal.ForeColor = SystemColors.ControlText
        lblPolarityVal.Location = New Point(61, 2)
        lblPolarityVal.Name = "lblPolarityVal"
        lblPolarityVal.Size = New Size(48, 15)
        lblPolarityVal.TabIndex = 7
        lblPolarityVal.Text = "Tog Pul"
        ' 
        ' sbarComplete
        ' 
        sbarComplete.Location = New Point(6, 80)
        sbarComplete.MaxValue = 50
        sbarComplete.MinMaxVisible = True
        sbarComplete.MinValue = 0
        sbarComplete.Name = "sbarComplete"
        sbarComplete.PercentComplete = -1
        sbarComplete.PulseWidthCurVal = -1
        sbarComplete.Size = New Size(107, 19)
        sbarComplete.TabIndex = 8
        ' 
        ' lblRepeatsRemaining
        ' 
        lblRepeatsRemaining.Font = New Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0)
        lblRepeatsRemaining.Location = New Point(61, 46)
        lblRepeatsRemaining.Name = "lblRepeatsRemaining"
        lblRepeatsRemaining.Size = New Size(48, 15)
        lblRepeatsRemaining.TabIndex = 5
        lblRepeatsRemaining.Text = "200"
        ' 
        ' Label7
        ' 
        Label7.AutoSize = True
        Label7.Font = New Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0)
        Label7.Location = New Point(5, 46)
        Label7.Name = "Label7"
        Label7.Size = New Size(51, 13)
        Label7.TabIndex = 4
        Label7.Text = "Repeats:"
        ' 
        ' lblDuration
        ' 
        lblDuration.Font = New Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0)
        lblDuration.Location = New Point(61, 32)
        lblDuration.Name = "lblDuration"
        lblDuration.Size = New Size(48, 15)
        lblDuration.TabIndex = 3
        lblDuration.Text = "2000000"
        ' 
        ' Label5
        ' 
        Label5.AutoSize = True
        Label5.Font = New Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0)
        Label5.Location = New Point(5, 32)
        Label5.Name = "Label5"
        Label5.Size = New Size(56, 13)
        Label5.TabIndex = 2
        Label5.Text = "Duration:"
        ' 
        ' lblLineNum
        ' 
        lblLineNum.Font = New Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0)
        lblLineNum.Location = New Point(61, 18)
        lblLineNum.Name = "lblLineNum"
        lblLineNum.Size = New Size(48, 15)
        lblLineNum.TabIndex = 1
        lblLineNum.Text = "2000000"
        ' 
        ' Label3
        ' 
        Label3.AutoSize = True
        Label3.Font = New Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0)
        Label3.Location = New Point(5, 18)
        Label3.Name = "Label3"
        Label3.Size = New Size(58, 13)
        Label3.TabIndex = 0
        Label3.Text = "Line Num:"
        ' 
        ' grpSpeed
        ' 
        grpSpeed.Controls.Add(sldSpeed)
        grpSpeed.Controls.Add(lblSpeed)
        grpSpeed.Location = New Point(5, 224)
        grpSpeed.Name = "grpSpeed"
        grpSpeed.Size = New Size(115, 45)
        grpSpeed.TabIndex = 7
        grpSpeed.TabStop = False
        grpSpeed.Text = "Speed"
        ' 
        ' sldSpeed
        ' 
        sldSpeed.AutoSize = False
        sldSpeed.Location = New Point(34, 14)
        sldSpeed.Maximum = 100
        sldSpeed.Name = "sldSpeed"
        sldSpeed.Size = New Size(80, 25)
        sldSpeed.TabIndex = 8
        sldSpeed.TickFrequency = 25
        sldSpeed.Value = 50
        ' 
        ' lblSpeed
        ' 
        lblSpeed.Font = New Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0)
        lblSpeed.Location = New Point(1, 17)
        lblSpeed.Margin = New Padding(0)
        lblSpeed.Name = "lblSpeed"
        lblSpeed.Size = New Size(40, 15)
        lblSpeed.TabIndex = 7
        lblSpeed.Text = "2000%"
        ' 
        ' grpPulseWidth
        ' 
        grpPulseWidth.Controls.Add(sldPulseWidth)
        grpPulseWidth.Controls.Add(lblPulseWidth)
        grpPulseWidth.Location = New Point(4, 177)
        grpPulseWidth.Name = "grpPulseWidth"
        grpPulseWidth.Size = New Size(115, 45)
        grpPulseWidth.TabIndex = 8
        grpPulseWidth.TabStop = False
        grpPulseWidth.Text = "PulseWidth"
        ' 
        ' sldPulseWidth
        ' 
        sldPulseWidth.AutoSize = False
        sldPulseWidth.Location = New Point(34, 14)
        sldPulseWidth.Maximum = 100
        sldPulseWidth.Name = "sldPulseWidth"
        sldPulseWidth.Size = New Size(80, 25)
        sldPulseWidth.TabIndex = 8
        sldPulseWidth.TickFrequency = 25
        sldPulseWidth.Value = 50
        ' 
        ' lblPulseWidth
        ' 
        lblPulseWidth.Font = New Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0)
        lblPulseWidth.Location = New Point(1, 17)
        lblPulseWidth.Margin = New Padding(0)
        lblPulseWidth.Name = "lblPulseWidth"
        lblPulseWidth.Size = New Size(40, 15)
        lblPulseWidth.TabIndex = 7
        lblPulseWidth.Text = "2000%"
        ' 
        ' ucChanControl
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BorderStyle = BorderStyle.FixedSingle
        Controls.Add(grpPulseWidth)
        Controls.Add(grpSpeed)
        Controls.Add(grpProgInfo)
        Controls.Add(grpOutputIntensity)
        Controls.Add(grpIntensity)
        Controls.Add(lblCommand)
        Controls.Add(lblProgState)
        Controls.Add(lblProgName)
        Controls.Add(cmdEnable)
        Name = "ucChanControl"
        Size = New Size(177, 270)
        grpIntensity.ResumeLayout(False)
        grpIntensity.PerformLayout()
        grpOutputIntensity.ResumeLayout(False)
        grpOutputIntensity.PerformLayout()
        CType(sldOutputIntensityPct, ComponentModel.ISupportInitialize).EndInit()
        grpProgInfo.ResumeLayout(False)
        grpProgInfo.PerformLayout()
        pnlPolarity.ResumeLayout(False)
        pnlPolarity.PerformLayout()
        grpSpeed.ResumeLayout(False)
        CType(sldSpeed, ComponentModel.ISupportInitialize).EndInit()
        grpPulseWidth.ResumeLayout(False)
        CType(sldPulseWidth, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
    End Sub

    Friend WithEvents cmdEnable As Button
    Friend WithEvents lblProgName As Label
    Friend WithEvents lblProgState As Label
    Friend WithEvents lblCommand As Label
    Friend WithEvents grpIntensity As GroupBox
    Friend WithEvents cboIntensityMin As ComboBox
    Friend WithEvents Label1 As Label
    Friend WithEvents grpOutputIntensity As GroupBox
    Friend WithEvents cboIntensityMax As ComboBox
    Friend WithEvents Label2 As Label
    Friend WithEvents grpProgInfo As GroupBox
    Friend WithEvents Label3 As Label
    Friend WithEvents lblPolarityVal As Label
    Friend WithEvents lblPolarityLabel As Label
    Friend WithEvents lblRepeatsRemaining As Label
    Friend WithEvents Label7 As Label
    Friend WithEvents lblDuration As Label
    Friend WithEvents Label5 As Label
    Friend WithEvents lblLineNum As Label
    Friend WithEvents grpSpeed As GroupBox
    Friend WithEvents sldSpeed As TrackBar
    Friend WithEvents lblSpeed As Label
    Friend WithEvents sldOutputIntensityPct As TrackBar
    Friend WithEvents lblOutputIntensityPct As Label
    Friend WithEvents grpPulseWidth As GroupBox
    Friend WithEvents sldPulseWidth As TrackBar
    Friend WithEvents lblPulseWidth As Label
    Friend WithEvents sbarComplete As ucSBar
    Friend WithEvents pnlPolarity As Panel

End Class
