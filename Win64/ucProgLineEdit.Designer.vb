<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ucProgLineEdit
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ucProgLineEdit))
        pnlTenMot = New Panel()
        grpTenMotVals = New GroupBox()
        ucvcTenMotV323 = New ucValueControl()
        ucvcTenMotV322 = New ucValueControl()
        ucvcTenMotV321 = New ucValueControl()
        ucvcTenMotEnd = New ucValueControl()
        ucvcTenMotStart = New ucValueControl()
        txtTenMotDutyCycle = New TextBox()
        lblTenMotDutyCycle = New Label()
        chkTenMotPostDelay = New CheckBox()
        cboPolarity = New ComboBox()
        lblPolarity = New Label()
        grpWaveform = New GroupBox()
        rdoWaveformRamp = New RadioButton()
        rdoWaveformTriangle = New RadioButton()
        rdoWaveformSine = New RadioButton()
        grpSineQuadrant = New GroupBox()
        rdoSineQuadHiLowHi = New RadioButton()
        rdoSineQuadLowHiLow = New RadioButton()
        rdoSineQuadLowHi = New RadioButton()
        rdoSineQuadLowMid = New RadioButton()
        rdoSineQuadMidLowMid = New RadioButton()
        rdoSineQuadMidLow = New RadioButton()
        rdoSineQuadHiMid = New RadioButton()
        rdoSineQuadMidHi = New RadioButton()
        rdoSineQuadMidHiMid = New RadioButton()
        Label1 = New Label()
        cboCommand = New ComboBox()
        grpGoto = New GroupBox()
        txtGotoLine = New TextBox()
        lblGotoLabel = New Label()
        grpDelay = New GroupBox()
        ucvcDelay = New ucValueControl()
        grpTest = New GroupBox()
        ucvcTestValRight322 = New ucValueControl()
        ucvcTestValRight321 = New ucValueControl()
        ucvcTestValLeft81 = New ucValueControl()
        Label6 = New Label()
        txtTestGTF = New TextBox()
        Label3 = New Label()
        txtTestGTT = New TextBox()
        Label2 = New Label()
        cboTestRightModOperator82V2 = New ComboBox()
        lblTestRightModOperator82S = New Label()
        cboTestType82V1 = New ComboBox()
        lblTestOperatorLabel = New Label()
        grpSet = New GroupBox()
        ucvcSetModifierVal322 = New ucValueControl()
        ucvcSetSource321 = New ucValueControl()
        ucvcSetTarget81 = New ucValueControl()
        cboSetModOperator82V2 = New ComboBox()
        lblSetModOperator82 = New Label()
        grpModifier = New GroupBox()
        grpProgControl = New GroupBox()
        ucvcProgControlProgNum321 = New ucValueControl()
        cboProgControlChannel = New ComboBox()
        lblProgControlChannel = New Label()
        cboProgControlOperation = New ComboBox()
        Label4 = New Label()
        grpDisplay = New GroupBox()
        grpSendMsg = New GroupBox()
        cmdUndo = New Button()
        cmdSave = New Button()
        pnlCommand = New Panel()
        rdoSineQuadMidHiLowMid = New RadioButton()
        rdoSineQuadHiLow = New RadioButton()
        rdoSineQuadMidLowHiMid = New RadioButton()
        pnlTenMot.SuspendLayout()
        grpTenMotVals.SuspendLayout()
        grpWaveform.SuspendLayout()
        grpSineQuadrant.SuspendLayout()
        grpGoto.SuspendLayout()
        grpDelay.SuspendLayout()
        grpTest.SuspendLayout()
        grpSet.SuspendLayout()
        grpProgControl.SuspendLayout()
        pnlCommand.SuspendLayout()
        SuspendLayout()
        ' 
        ' pnlTenMot
        ' 
        pnlTenMot.Controls.Add(grpTenMotVals)
        pnlTenMot.Controls.Add(grpWaveform)
        pnlTenMot.Controls.Add(grpSineQuadrant)
        pnlTenMot.Location = New Point(5, 50)
        pnlTenMot.Name = "pnlTenMot"
        pnlTenMot.Size = New Size(400, 405)
        pnlTenMot.TabIndex = 0
        ' 
        ' grpTenMotVals
        ' 
        grpTenMotVals.Controls.Add(ucvcTenMotV323)
        grpTenMotVals.Controls.Add(ucvcTenMotV322)
        grpTenMotVals.Controls.Add(ucvcTenMotV321)
        grpTenMotVals.Controls.Add(ucvcTenMotEnd)
        grpTenMotVals.Controls.Add(ucvcTenMotStart)
        grpTenMotVals.Controls.Add(txtTenMotDutyCycle)
        grpTenMotVals.Controls.Add(lblTenMotDutyCycle)
        grpTenMotVals.Controls.Add(chkTenMotPostDelay)
        grpTenMotVals.Controls.Add(cboPolarity)
        grpTenMotVals.Controls.Add(lblPolarity)
        grpTenMotVals.Location = New Point(0, 164)
        grpTenMotVals.Name = "grpTenMotVals"
        grpTenMotVals.Size = New Size(400, 236)
        grpTenMotVals.TabIndex = 2
        grpTenMotVals.TabStop = False
        grpTenMotVals.Text = "Ten/Mot Values"
        ' 
        ' ucvcTenMotV323
        ' 
        ucvcTenMotV323.Location = New Point(3, 170)
        ucvcTenMotV323.Name = "ucvcTenMotV323"
        ucvcTenMotV323.Size = New Size(395, 23)
        ucvcTenMotV323.TabIndex = 28
        ' 
        ' ucvcTenMotV322
        ' 
        ucvcTenMotV322.Location = New Point(3, 120)
        ucvcTenMotV322.Name = "ucvcTenMotV322"
        ucvcTenMotV322.Size = New Size(395, 23)
        ucvcTenMotV322.TabIndex = 27
        ' 
        ' ucvcTenMotV321
        ' 
        ucvcTenMotV321.Location = New Point(3, 95)
        ucvcTenMotV321.Name = "ucvcTenMotV321"
        ucvcTenMotV321.Size = New Size(395, 23)
        ucvcTenMotV321.TabIndex = 26
        ' 
        ' ucvcTenMotEnd
        ' 
        ucvcTenMotEnd.Location = New Point(3, 45)
        ucvcTenMotEnd.Name = "ucvcTenMotEnd"
        ucvcTenMotEnd.Size = New Size(395, 23)
        ucvcTenMotEnd.TabIndex = 25
        ' 
        ' ucvcTenMotStart
        ' 
        ucvcTenMotStart.Location = New Point(3, 20)
        ucvcTenMotStart.Name = "ucvcTenMotStart"
        ucvcTenMotStart.Size = New Size(395, 23)
        ucvcTenMotStart.TabIndex = 24
        ' 
        ' txtTenMotDutyCycle
        ' 
        txtTenMotDutyCycle.Location = New Point(292, 70)
        txtTenMotDutyCycle.MaxLength = 3
        txtTenMotDutyCycle.Name = "txtTenMotDutyCycle"
        txtTenMotDutyCycle.Size = New Size(27, 23)
        txtTenMotDutyCycle.TabIndex = 23
        ' 
        ' lblTenMotDutyCycle
        ' 
        lblTenMotDutyCycle.Location = New Point(216, 73)
        lblTenMotDutyCycle.Name = "lblTenMotDutyCycle"
        lblTenMotDutyCycle.Size = New Size(73, 15)
        lblTenMotDutyCycle.TabIndex = 22
        lblTenMotDutyCycle.Text = "Duty Cycle:"
        lblTenMotDutyCycle.TextAlign = ContentAlignment.MiddleRight
        ' 
        ' chkTenMotPostDelay
        ' 
        chkTenMotPostDelay.AutoSize = True
        chkTenMotPostDelay.Location = New Point(29, 145)
        chkTenMotPostDelay.Name = "chkTenMotPostDelay"
        chkTenMotPostDelay.Size = New Size(78, 19)
        chkTenMotPostDelay.TabIndex = 21
        chkTenMotPostDelay.Text = "PostDelay"
        chkTenMotPostDelay.UseVisualStyleBackColor = True
        ' 
        ' cboPolarity
        ' 
        cboPolarity.FormattingEnabled = True
        cboPolarity.Location = New Point(71, 70)
        cboPolarity.Name = "cboPolarity"
        cboPolarity.Size = New Size(121, 23)
        cboPolarity.TabIndex = 17
        cboPolarity.Text = "Fwd -Toggle Pulse"
        ' 
        ' lblPolarity
        ' 
        lblPolarity.Location = New Point(3, 73)
        lblPolarity.Name = "lblPolarity"
        lblPolarity.Size = New Size(65, 15)
        lblPolarity.TabIndex = 16
        lblPolarity.Text = "Polarity:"
        lblPolarity.TextAlign = ContentAlignment.MiddleRight
        ' 
        ' grpWaveform
        ' 
        grpWaveform.Controls.Add(rdoWaveformRamp)
        grpWaveform.Controls.Add(rdoWaveformTriangle)
        grpWaveform.Controls.Add(rdoWaveformSine)
        grpWaveform.Location = New Point(0, 0)
        grpWaveform.Name = "grpWaveform"
        grpWaveform.Size = New Size(400, 60)
        grpWaveform.TabIndex = 1
        grpWaveform.TabStop = False
        grpWaveform.Text = "Waveform"
        ' 
        ' rdoWaveformRamp
        ' 
        rdoWaveformRamp.Image = My.Resources.Resources.GraphRampSmall
        rdoWaveformRamp.Location = New Point(15, 22)
        rdoWaveformRamp.Name = "rdoWaveformRamp"
        rdoWaveformRamp.Size = New Size(50, 25)
        rdoWaveformRamp.TabIndex = 0
        rdoWaveformRamp.TextImageRelation = TextImageRelation.TextBeforeImage
        rdoWaveformRamp.UseVisualStyleBackColor = True
        ' 
        ' rdoWaveformTriangle
        ' 
        rdoWaveformTriangle.Image = My.Resources.Resources.GraphTriangleSmall
        rdoWaveformTriangle.Location = New Point(88, 22)
        rdoWaveformTriangle.Name = "rdoWaveformTriangle"
        rdoWaveformTriangle.Size = New Size(50, 25)
        rdoWaveformTriangle.TabIndex = 1
        rdoWaveformTriangle.UseVisualStyleBackColor = True
        ' 
        ' rdoWaveformSine
        ' 
        rdoWaveformSine.Image = My.Resources.Resources.GraphSineSmall
        rdoWaveformSine.Location = New Point(161, 22)
        rdoWaveformSine.Name = "rdoWaveformSine"
        rdoWaveformSine.Size = New Size(50, 25)
        rdoWaveformSine.TabIndex = 3
        rdoWaveformSine.UseVisualStyleBackColor = True
        ' 
        ' grpSineQuadrant
        ' 
        grpSineQuadrant.Controls.Add(rdoSineQuadMidLowHiMid)
        grpSineQuadrant.Controls.Add(rdoSineQuadHiLow)
        grpSineQuadrant.Controls.Add(rdoSineQuadMidHiLowMid)
        grpSineQuadrant.Controls.Add(rdoSineQuadHiLowHi)
        grpSineQuadrant.Controls.Add(rdoSineQuadLowHiLow)
        grpSineQuadrant.Controls.Add(rdoSineQuadLowHi)
        grpSineQuadrant.Controls.Add(rdoSineQuadLowMid)
        grpSineQuadrant.Controls.Add(rdoSineQuadMidLowMid)
        grpSineQuadrant.Controls.Add(rdoSineQuadMidLow)
        grpSineQuadrant.Controls.Add(rdoSineQuadHiMid)
        grpSineQuadrant.Controls.Add(rdoSineQuadMidHi)
        grpSineQuadrant.Controls.Add(rdoSineQuadMidHiMid)
        grpSineQuadrant.Location = New Point(0, 65)
        grpSineQuadrant.Name = "grpSineQuadrant"
        grpSineQuadrant.Size = New Size(400, 88)
        grpSineQuadrant.TabIndex = 1
        grpSineQuadrant.TabStop = False
        grpSineQuadrant.Text = "Quadrant"
        ' 
        ' rdoSineQuadHiLowHi
        ' 
        rdoSineQuadHiLowHi.Image = CType(resources.GetObject("rdoSineQuadHiLowHi.Image"), Image)
        rdoSineQuadHiLowHi.Location = New Point(204, 51)
        rdoSineQuadHiLowHi.Name = "rdoSineQuadHiLowHi"
        rdoSineQuadHiLowHi.Size = New Size(50, 25)
        rdoSineQuadHiLowHi.TabIndex = 10
        rdoSineQuadHiLowHi.TextImageRelation = TextImageRelation.TextBeforeImage
        rdoSineQuadHiLowHi.UseVisualStyleBackColor = True
        ' 
        ' rdoSineQuadLowHiLow
        ' 
        rdoSineQuadLowHiLow.Image = CType(resources.GetObject("rdoSineQuadLowHiLow.Image"), Image)
        rdoSineQuadLowHiLow.Location = New Point(141, 51)
        rdoSineQuadLowHiLow.Name = "rdoSineQuadLowHiLow"
        rdoSineQuadLowHiLow.Size = New Size(50, 25)
        rdoSineQuadLowHiLow.TabIndex = 9
        rdoSineQuadLowHiLow.TextImageRelation = TextImageRelation.TextBeforeImage
        rdoSineQuadLowHiLow.UseVisualStyleBackColor = True
        ' 
        ' rdoSineQuadLowHi
        ' 
        rdoSineQuadLowHi.Image = CType(resources.GetObject("rdoSineQuadLowHi.Image"), Image)
        rdoSineQuadLowHi.Location = New Point(15, 51)
        rdoSineQuadLowHi.Name = "rdoSineQuadLowHi"
        rdoSineQuadLowHi.Size = New Size(50, 25)
        rdoSineQuadLowHi.TabIndex = 8
        rdoSineQuadLowHi.TextImageRelation = TextImageRelation.TextBeforeImage
        rdoSineQuadLowHi.UseVisualStyleBackColor = True
        ' 
        ' rdoSineQuadLowMid
        ' 
        rdoSineQuadLowMid.Image = CType(resources.GetObject("rdoSineQuadLowMid.Image"), Image)
        rdoSineQuadLowMid.Location = New Point(204, 20)
        rdoSineQuadLowMid.Name = "rdoSineQuadLowMid"
        rdoSineQuadLowMid.Size = New Size(50, 25)
        rdoSineQuadLowMid.TabIndex = 7
        rdoSineQuadLowMid.TextImageRelation = TextImageRelation.TextBeforeImage
        rdoSineQuadLowMid.UseVisualStyleBackColor = True
        ' 
        ' rdoSineQuadMidLowMid
        ' 
        rdoSineQuadMidLowMid.Image = CType(resources.GetObject("rdoSineQuadMidLowMid.Image"), Image)
        rdoSineQuadMidLowMid.Location = New Point(330, 20)
        rdoSineQuadMidLowMid.Name = "rdoSineQuadMidLowMid"
        rdoSineQuadMidLowMid.Size = New Size(50, 25)
        rdoSineQuadMidLowMid.TabIndex = 6
        rdoSineQuadMidLowMid.TextImageRelation = TextImageRelation.TextBeforeImage
        rdoSineQuadMidLowMid.UseVisualStyleBackColor = True
        ' 
        ' rdoSineQuadMidLow
        ' 
        rdoSineQuadMidLow.Image = CType(resources.GetObject("rdoSineQuadMidLow.Image"), Image)
        rdoSineQuadMidLow.Location = New Point(141, 20)
        rdoSineQuadMidLow.Name = "rdoSineQuadMidLow"
        rdoSineQuadMidLow.Size = New Size(50, 25)
        rdoSineQuadMidLow.TabIndex = 5
        rdoSineQuadMidLow.TextImageRelation = TextImageRelation.TextBeforeImage
        rdoSineQuadMidLow.UseVisualStyleBackColor = True
        ' 
        ' rdoSineQuadHiMid
        ' 
        rdoSineQuadHiMid.Image = CType(resources.GetObject("rdoSineQuadHiMid.Image"), Image)
        rdoSineQuadHiMid.Location = New Point(78, 20)
        rdoSineQuadHiMid.Name = "rdoSineQuadHiMid"
        rdoSineQuadHiMid.Size = New Size(50, 25)
        rdoSineQuadHiMid.TabIndex = 4
        rdoSineQuadHiMid.TextImageRelation = TextImageRelation.TextBeforeImage
        rdoSineQuadHiMid.UseVisualStyleBackColor = True
        ' 
        ' rdoSineQuadMidHi
        ' 
        rdoSineQuadMidHi.Image = CType(resources.GetObject("rdoSineQuadMidHi.Image"), Image)
        rdoSineQuadMidHi.Location = New Point(15, 20)
        rdoSineQuadMidHi.Name = "rdoSineQuadMidHi"
        rdoSineQuadMidHi.Size = New Size(50, 25)
        rdoSineQuadMidHi.TabIndex = 1
        rdoSineQuadMidHi.TextImageRelation = TextImageRelation.TextBeforeImage
        rdoSineQuadMidHi.UseVisualStyleBackColor = True
        ' 
        ' rdoSineQuadMidHiMid
        ' 
        rdoSineQuadMidHiMid.BackgroundImageLayout = ImageLayout.Center
        rdoSineQuadMidHiMid.Image = CType(resources.GetObject("rdoSineQuadMidHiMid.Image"), Image)
        rdoSineQuadMidHiMid.Location = New Point(267, 20)
        rdoSineQuadMidHiMid.Name = "rdoSineQuadMidHiMid"
        rdoSineQuadMidHiMid.Size = New Size(50, 25)
        rdoSineQuadMidHiMid.TabIndex = 3
        rdoSineQuadMidHiMid.TextImageRelation = TextImageRelation.TextBeforeImage
        rdoSineQuadMidHiMid.UseVisualStyleBackColor = True
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Location = New Point(3, 8)
        Label1.Name = "Label1"
        Label1.Size = New Size(67, 15)
        Label1.TabIndex = 0
        Label1.Text = "Command:"
        ' 
        ' cboCommand
        ' 
        cboCommand.FormattingEnabled = True
        cboCommand.Items.AddRange(New Object() {"No Op", "Delay", "TenMot"})
        cboCommand.Location = New Point(76, 6)
        cboCommand.Name = "cboCommand"
        cboCommand.Size = New Size(121, 23)
        cboCommand.TabIndex = 1
        ' 
        ' grpGoto
        ' 
        grpGoto.Controls.Add(txtGotoLine)
        grpGoto.Controls.Add(lblGotoLabel)
        grpGoto.Location = New Point(5, 456)
        grpGoto.Name = "grpGoto"
        grpGoto.Size = New Size(400, 65)
        grpGoto.TabIndex = 2
        grpGoto.TabStop = False
        grpGoto.Text = "GoTo"
        ' 
        ' txtGotoLine
        ' 
        txtGotoLine.Location = New Point(71, 20)
        txtGotoLine.MaxLength = 3
        txtGotoLine.Name = "txtGotoLine"
        txtGotoLine.Size = New Size(27, 23)
        txtGotoLine.TabIndex = 18
        ' 
        ' lblGotoLabel
        ' 
        lblGotoLabel.Location = New Point(3, 23)
        lblGotoLabel.Name = "lblGotoLabel"
        lblGotoLabel.Size = New Size(65, 15)
        lblGotoLabel.TabIndex = 17
        lblGotoLabel.Text = "Line:"
        lblGotoLabel.TextAlign = ContentAlignment.MiddleRight
        ' 
        ' grpDelay
        ' 
        grpDelay.Controls.Add(ucvcDelay)
        grpDelay.Location = New Point(5, 527)
        grpDelay.Name = "grpDelay"
        grpDelay.Size = New Size(400, 65)
        grpDelay.TabIndex = 3
        grpDelay.TabStop = False
        grpDelay.Text = "Delay"
        ' 
        ' ucvcDelay
        ' 
        ucvcDelay.Location = New Point(0, 20)
        ucvcDelay.Name = "ucvcDelay"
        ucvcDelay.Size = New Size(395, 23)
        ucvcDelay.TabIndex = 29
        ' 
        ' grpTest
        ' 
        grpTest.Controls.Add(ucvcTestValRight322)
        grpTest.Controls.Add(ucvcTestValRight321)
        grpTest.Controls.Add(ucvcTestValLeft81)
        grpTest.Controls.Add(Label6)
        grpTest.Controls.Add(txtTestGTF)
        grpTest.Controls.Add(Label3)
        grpTest.Controls.Add(txtTestGTT)
        grpTest.Controls.Add(Label2)
        grpTest.Controls.Add(cboTestRightModOperator82V2)
        grpTest.Controls.Add(lblTestRightModOperator82S)
        grpTest.Controls.Add(cboTestType82V1)
        grpTest.Controls.Add(lblTestOperatorLabel)
        grpTest.Location = New Point(5, 602)
        grpTest.Name = "grpTest"
        grpTest.Size = New Size(400, 209)
        grpTest.TabIndex = 4
        grpTest.TabStop = False
        grpTest.Text = "Test"
        ' 
        ' ucvcTestValRight322
        ' 
        ucvcTestValRight322.Location = New Point(3, 120)
        ucvcTestValRight322.Name = "ucvcTestValRight322"
        ucvcTestValRight322.Size = New Size(395, 23)
        ucvcTestValRight322.TabIndex = 32
        ' 
        ' ucvcTestValRight321
        ' 
        ucvcTestValRight321.Location = New Point(3, 70)
        ucvcTestValRight321.Name = "ucvcTestValRight321"
        ucvcTestValRight321.Size = New Size(395, 23)
        ucvcTestValRight321.TabIndex = 31
        ' 
        ' ucvcTestValLeft81
        ' 
        ucvcTestValLeft81.Location = New Point(0, 20)
        ucvcTestValLeft81.Name = "ucvcTestValLeft81"
        ucvcTestValLeft81.Size = New Size(395, 23)
        ucvcTestValLeft81.TabIndex = 30
        ' 
        ' Label6
        ' 
        Label6.Location = New Point(11, 148)
        Label6.Name = "Label6"
        Label6.Size = New Size(65, 15)
        Label6.TabIndex = 28
        Label6.Text = "Then:"
        Label6.TextAlign = ContentAlignment.MiddleRight
        ' 
        ' txtTestGTF
        ' 
        txtTestGTF.Location = New Point(187, 170)
        txtTestGTF.MaxLength = 3
        txtTestGTF.Name = "txtTestGTF"
        txtTestGTF.Size = New Size(27, 23)
        txtTestGTF.TabIndex = 27
        ' 
        ' Label3
        ' 
        Label3.Location = New Point(121, 173)
        Label3.Name = "Label3"
        Label3.Size = New Size(65, 15)
        Label3.TabIndex = 26
        Label3.Text = "Goto False:"
        Label3.TextAlign = ContentAlignment.MiddleRight
        ' 
        ' txtTestGTT
        ' 
        txtTestGTT.Location = New Point(71, 170)
        txtTestGTT.MaxLength = 3
        txtTestGTT.Name = "txtTestGTT"
        txtTestGTT.Size = New Size(27, 23)
        txtTestGTT.TabIndex = 25
        ' 
        ' Label2
        ' 
        Label2.Location = New Point(3, 173)
        Label2.Name = "Label2"
        Label2.Size = New Size(65, 15)
        Label2.TabIndex = 24
        Label2.Text = "Goto True:"
        Label2.TextAlign = ContentAlignment.MiddleRight
        ' 
        ' cboTestRightModOperator82V2
        ' 
        cboTestRightModOperator82V2.FormattingEnabled = True
        cboTestRightModOperator82V2.Location = New Point(71, 95)
        cboTestRightModOperator82V2.MaxDropDownItems = 10
        cboTestRightModOperator82V2.Name = "cboTestRightModOperator82V2"
        cboTestRightModOperator82V2.Size = New Size(50, 23)
        cboTestRightModOperator82V2.TabIndex = 23
        cboTestRightModOperator82V2.Text = "mod"
        ' 
        ' lblTestRightModOperator82S
        ' 
        lblTestRightModOperator82S.Location = New Point(3, 98)
        lblTestRightModOperator82S.Name = "lblTestRightModOperator82S"
        lblTestRightModOperator82S.Size = New Size(65, 15)
        lblTestRightModOperator82S.TabIndex = 22
        lblTestRightModOperator82S.Text = "Modifier:"
        lblTestRightModOperator82S.TextAlign = ContentAlignment.MiddleRight
        ' 
        ' cboTestType82V1
        ' 
        cboTestType82V1.FormattingEnabled = True
        cboTestType82V1.Location = New Point(71, 45)
        cboTestType82V1.MaxDropDownItems = 10
        cboTestType82V1.Name = "cboTestType82V1"
        cboTestType82V1.Size = New Size(121, 23)
        cboTestType82V1.TabIndex = 19
        cboTestType82V1.Text = "Equal Or Between"
        ' 
        ' lblTestOperatorLabel
        ' 
        lblTestOperatorLabel.Location = New Point(3, 48)
        lblTestOperatorLabel.Name = "lblTestOperatorLabel"
        lblTestOperatorLabel.Size = New Size(65, 15)
        lblTestOperatorLabel.TabIndex = 18
        lblTestOperatorLabel.Text = "is"
        lblTestOperatorLabel.TextAlign = ContentAlignment.MiddleRight
        ' 
        ' grpSet
        ' 
        grpSet.Controls.Add(ucvcSetModifierVal322)
        grpSet.Controls.Add(ucvcSetSource321)
        grpSet.Controls.Add(ucvcSetTarget81)
        grpSet.Controls.Add(cboSetModOperator82V2)
        grpSet.Controls.Add(lblSetModOperator82)
        grpSet.Location = New Point(5, 840)
        grpSet.Name = "grpSet"
        grpSet.Size = New Size(400, 145)
        grpSet.TabIndex = 5
        grpSet.TabStop = False
        grpSet.Text = "Set"
        ' 
        ' ucvcSetModifierVal322
        ' 
        ucvcSetModifierVal322.Location = New Point(3, 113)
        ucvcSetModifierVal322.Name = "ucvcSetModifierVal322"
        ucvcSetModifierVal322.Size = New Size(395, 23)
        ucvcSetModifierVal322.TabIndex = 35
        ' 
        ' ucvcSetSource321
        ' 
        ucvcSetSource321.Location = New Point(3, 51)
        ucvcSetSource321.Name = "ucvcSetSource321"
        ucvcSetSource321.Size = New Size(395, 23)
        ucvcSetSource321.TabIndex = 34
        ' 
        ' ucvcSetTarget81
        ' 
        ucvcSetTarget81.Location = New Point(3, 22)
        ucvcSetTarget81.Name = "ucvcSetTarget81"
        ucvcSetTarget81.Size = New Size(395, 23)
        ucvcSetTarget81.TabIndex = 33
        ' 
        ' cboSetModOperator82V2
        ' 
        cboSetModOperator82V2.FormattingEnabled = True
        cboSetModOperator82V2.Location = New Point(71, 84)
        cboSetModOperator82V2.MaxDropDownItems = 10
        cboSetModOperator82V2.Name = "cboSetModOperator82V2"
        cboSetModOperator82V2.Size = New Size(121, 23)
        cboSetModOperator82V2.TabIndex = 25
        cboSetModOperator82V2.Text = "Equal Or Between"
        ' 
        ' lblSetModOperator82
        ' 
        lblSetModOperator82.Location = New Point(3, 87)
        lblSetModOperator82.Name = "lblSetModOperator82"
        lblSetModOperator82.Size = New Size(65, 15)
        lblSetModOperator82.TabIndex = 24
        lblSetModOperator82.Text = "Modifier:"
        lblSetModOperator82.TextAlign = ContentAlignment.MiddleRight
        ' 
        ' grpModifier
        ' 
        grpModifier.Location = New Point(5, 991)
        grpModifier.Name = "grpModifier"
        grpModifier.Size = New Size(400, 35)
        grpModifier.TabIndex = 6
        grpModifier.TabStop = False
        grpModifier.Text = "Set Modifier"
        ' 
        ' grpProgControl
        ' 
        grpProgControl.Controls.Add(ucvcProgControlProgNum321)
        grpProgControl.Controls.Add(cboProgControlChannel)
        grpProgControl.Controls.Add(lblProgControlChannel)
        grpProgControl.Controls.Add(cboProgControlOperation)
        grpProgControl.Controls.Add(Label4)
        grpProgControl.Location = New Point(5, 1032)
        grpProgControl.Name = "grpProgControl"
        grpProgControl.Size = New Size(400, 123)
        grpProgControl.TabIndex = 7
        grpProgControl.TabStop = False
        grpProgControl.Text = "Program Control"
        ' 
        ' ucvcProgControlProgNum321
        ' 
        ucvcProgControlProgNum321.Location = New Point(3, 84)
        ucvcProgControlProgNum321.Name = "ucvcProgControlProgNum321"
        ucvcProgControlProgNum321.Size = New Size(395, 23)
        ucvcProgControlProgNum321.TabIndex = 34
        ' 
        ' cboProgControlChannel
        ' 
        cboProgControlChannel.FormattingEnabled = True
        cboProgControlChannel.Location = New Point(71, 55)
        cboProgControlChannel.MaxDropDownItems = 10
        cboProgControlChannel.Name = "cboProgControlChannel"
        cboProgControlChannel.Size = New Size(143, 23)
        cboProgControlChannel.TabIndex = 29
        cboProgControlChannel.Text = "This Channel"
        ' 
        ' lblProgControlChannel
        ' 
        lblProgControlChannel.Location = New Point(3, 58)
        lblProgControlChannel.Name = "lblProgControlChannel"
        lblProgControlChannel.Size = New Size(65, 15)
        lblProgControlChannel.TabIndex = 28
        lblProgControlChannel.Text = "Channel:"
        lblProgControlChannel.TextAlign = ContentAlignment.MiddleRight
        ' 
        ' cboProgControlOperation
        ' 
        cboProgControlOperation.FormattingEnabled = True
        cboProgControlOperation.Location = New Point(71, 26)
        cboProgControlOperation.MaxDropDownItems = 10
        cboProgControlOperation.Name = "cboProgControlOperation"
        cboProgControlOperation.Size = New Size(143, 23)
        cboProgControlOperation.TabIndex = 27
        cboProgControlOperation.Text = "Load Program & Wait"
        ' 
        ' Label4
        ' 
        Label4.Location = New Point(3, 29)
        Label4.Name = "Label4"
        Label4.Size = New Size(65, 15)
        Label4.TabIndex = 26
        Label4.Text = "Operation:"
        Label4.TextAlign = ContentAlignment.MiddleRight
        ' 
        ' grpDisplay
        ' 
        grpDisplay.Location = New Point(5, 1161)
        grpDisplay.Name = "grpDisplay"
        grpDisplay.Size = New Size(400, 35)
        grpDisplay.TabIndex = 8
        grpDisplay.TabStop = False
        grpDisplay.Text = "Display"
        ' 
        ' grpSendMsg
        ' 
        grpSendMsg.Location = New Point(5, 1202)
        grpSendMsg.Name = "grpSendMsg"
        grpSendMsg.Size = New Size(400, 35)
        grpSendMsg.TabIndex = 9
        grpSendMsg.TabStop = False
        grpSendMsg.Text = "Send Message"
        ' 
        ' cmdUndo
        ' 
        cmdUndo.Location = New Point(234, 5)
        cmdUndo.Name = "cmdUndo"
        cmdUndo.Size = New Size(50, 23)
        cmdUndo.TabIndex = 10
        cmdUndo.Text = "Undo"
        cmdUndo.UseVisualStyleBackColor = True
        ' 
        ' cmdSave
        ' 
        cmdSave.Location = New Point(307, 5)
        cmdSave.Name = "cmdSave"
        cmdSave.Size = New Size(50, 23)
        cmdSave.TabIndex = 11
        cmdSave.Text = "Save"
        cmdSave.UseVisualStyleBackColor = True
        ' 
        ' pnlCommand
        ' 
        pnlCommand.Controls.Add(Label1)
        pnlCommand.Controls.Add(cmdSave)
        pnlCommand.Controls.Add(cboCommand)
        pnlCommand.Controls.Add(cmdUndo)
        pnlCommand.Location = New Point(5, 5)
        pnlCommand.Name = "pnlCommand"
        pnlCommand.Size = New Size(400, 40)
        pnlCommand.TabIndex = 12
        ' 
        ' rdoSineQuadMidHiLowMid
        ' 
        rdoSineQuadMidHiLowMid.Image = CType(resources.GetObject("rdoSineQuadMidHiLowMid.Image"), Image)
        rdoSineQuadMidHiLowMid.Location = New Point(267, 51)
        rdoSineQuadMidHiLowMid.Name = "rdoSineQuadMidHiLowMid"
        rdoSineQuadMidHiLowMid.Size = New Size(50, 25)
        rdoSineQuadMidHiLowMid.TabIndex = 11
        rdoSineQuadMidHiLowMid.TextImageRelation = TextImageRelation.TextBeforeImage
        rdoSineQuadMidHiLowMid.UseVisualStyleBackColor = True
        ' 
        ' rdoSineQuadHiLow
        ' 
        rdoSineQuadHiLow.Image = CType(resources.GetObject("rdoSineQuadHiLow.Image"), Image)
        rdoSineQuadHiLow.Location = New Point(78, 51)
        rdoSineQuadHiLow.Name = "rdoSineQuadHiLow"
        rdoSineQuadHiLow.Size = New Size(50, 25)
        rdoSineQuadHiLow.TabIndex = 12
        rdoSineQuadHiLow.TextImageRelation = TextImageRelation.TextBeforeImage
        rdoSineQuadHiLow.UseVisualStyleBackColor = True
        ' 
        ' rdoSineQuadMidLowHiMid
        ' 
        rdoSineQuadMidLowHiMid.Image = CType(resources.GetObject("rdoSineQuadMidLowHiMid.Image"), Image)
        rdoSineQuadMidLowHiMid.Location = New Point(330, 51)
        rdoSineQuadMidLowHiMid.Name = "rdoSineQuadMidLowHiMid"
        rdoSineQuadMidLowHiMid.Size = New Size(50, 25)
        rdoSineQuadMidLowHiMid.TabIndex = 13
        rdoSineQuadMidLowHiMid.TextImageRelation = TextImageRelation.TextBeforeImage
        rdoSineQuadMidLowHiMid.UseVisualStyleBackColor = True
        ' 
        ' ucProgLineEdit
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        Controls.Add(pnlCommand)
        Controls.Add(grpSendMsg)
        Controls.Add(grpDisplay)
        Controls.Add(grpProgControl)
        Controls.Add(grpModifier)
        Controls.Add(grpSet)
        Controls.Add(grpTest)
        Controls.Add(grpDelay)
        Controls.Add(grpGoto)
        Controls.Add(pnlTenMot)
        Name = "ucProgLineEdit"
        Size = New Size(410, 1466)
        pnlTenMot.ResumeLayout(False)
        grpTenMotVals.ResumeLayout(False)
        grpTenMotVals.PerformLayout()
        grpWaveform.ResumeLayout(False)
        grpSineQuadrant.ResumeLayout(False)
        grpGoto.ResumeLayout(False)
        grpGoto.PerformLayout()
        grpDelay.ResumeLayout(False)
        grpTest.ResumeLayout(False)
        grpTest.PerformLayout()
        grpSet.ResumeLayout(False)
        grpProgControl.ResumeLayout(False)
        pnlCommand.ResumeLayout(False)
        pnlCommand.PerformLayout()
        ResumeLayout(False)
    End Sub

    Friend WithEvents pnlTenMot As Panel
    Friend WithEvents Label1 As Label
    Friend WithEvents cboCommand As ComboBox
    Friend WithEvents rdoWaveformRamp As RadioButton
    Friend WithEvents rdoWaveformSine As RadioButton
    Friend WithEvents rdoWaveformTriangle As RadioButton
    Friend WithEvents pnlSineQuadrant As Panel
    Friend WithEvents rdoSineQuadMidHiMid As RadioButton
    Friend WithEvents rdoSineQuadMidHi As RadioButton
    Friend WithEvents rdoSineQuadHiMid As RadioButton
    Friend WithEvents grpSineQuadrant As GroupBox
    Friend WithEvents grpWaveform As GroupBox
    Friend WithEvents grpTenMotVals As GroupBox
    'Friend WithEvents ucvcTenMotStart As ucValueControl
    'Friend WithEvents ucvcTenMotEnd As ucValueControl
    Friend WithEvents lblPolarity As Label
    Friend WithEvents cboPolarity As ComboBox
    'Friend WithEvents ucvcTenMotV323 As ucValueControl
    'Friend WithEvents ucvcTenMotV322 As ucValueControl
    'Friend WithEvents ucvcTenMotV321 As ucValueControl
    Friend WithEvents rdoSineQuadLowHiLow As RadioButton
    Friend WithEvents rdoSineQuadLowHi As RadioButton
    Friend WithEvents rdoSineQuadLowMid As RadioButton
    Friend WithEvents rdoSineQuadMidLowMid As RadioButton
    Friend WithEvents rdoSineQuadMidLow As RadioButton
    Friend WithEvents rdoSineQuadHiLowHi As RadioButton
    Friend WithEvents grpGoto As GroupBox
    Friend WithEvents txtGotoLine As TextBox
    Friend WithEvents lblGotoLabel As Label
    Friend WithEvents grpDelay As GroupBox
    'Friend WithEvents ucvcDelay As ucValueControl
    Friend WithEvents grpTest As GroupBox
    Friend WithEvents grpSet As GroupBox
    Friend WithEvents grpModifier As GroupBox
    Friend WithEvents grpProgControl As GroupBox
    Friend WithEvents grpDisplay As GroupBox
    Friend WithEvents grpSendMsg As GroupBox
    'Friend WithEvents ucvcTestValLeft81 As ucValueControl
    Friend WithEvents cboTestType82V1 As ComboBox
    Friend WithEvents lblTestOperatorLabel As Label
    'Friend WithEvents ucvcTestValRight321 As ucValueControl
    Friend WithEvents cboTestRightModOperator82V2 As ComboBox
    Friend WithEvents lblTestRightModOperator82S As Label
    'Friend WithEvents ucvcTestValRight322 As ucValueControl
    Friend WithEvents txtTestGTF As TextBox
    Friend WithEvents Label3 As Label
    Friend WithEvents txtTestGTT As TextBox
    Friend WithEvents Label2 As Label
    'Friend WithEvents ucvcSetTarget81 As ucValueControl
    'Friend WithEvents ucvcSetSource321 As ucValueControl
    Friend WithEvents cboSetModOperator82V2 As ComboBox
    Friend WithEvents lblSetModOperator82 As Label
    'Friend WithEvents ucvcSetModifierVal322 As ucValueControl
    'Friend WithEvents ucvcProgControlProgNum321 As ucValueControl
    Friend WithEvents cboProgControlChannel As ComboBox
    Friend WithEvents lblProgControlChannel As Label
    Friend WithEvents cboProgControlOperation As ComboBox
    Friend WithEvents Label4 As Label
    Friend WithEvents Label6 As Label
    Friend WithEvents cmdUndo As Button
    Friend WithEvents cmdSave As Button
    Friend WithEvents lblTenMotRepeatTimes As Label
    Friend WithEvents txtTenMotRepeats As TextBox
    Friend WithEvents chkTenMotPostDelay As CheckBox
    Friend WithEvents pnlCommand As Panel
    Friend WithEvents txtTenMotDutyCycle As TextBox
    Friend WithEvents lblTenMotDutyCycle As Label
    Friend WithEvents ucvcTenMotStart As ucValueControl
    Friend WithEvents ucvcTenMotEnd As ucValueControl
    Friend WithEvents ucvcTenMotV323 As ucValueControl
    Friend WithEvents ucvcTenMotV322 As ucValueControl
    Friend WithEvents ucvcTenMotV321 As ucValueControl
    Friend WithEvents ucvcDelay As ucValueControl
    Friend WithEvents ucvcTestValLeft81 As ucValueControl
    Friend WithEvents ucvcTestValRight322 As ucValueControl
    Friend WithEvents ucvcTestValRight321 As ucValueControl
    Friend WithEvents ucvcSetTarget81 As ucValueControl
    Friend WithEvents ucvcSetModifierVal322 As ucValueControl
    Friend WithEvents ucvcSetSource321 As ucValueControl
    Friend WithEvents ucvcProgControlProgNum321 As ucValueControl
    Friend WithEvents rdoSineQuadMidHiLowMid As RadioButton
    Friend WithEvents rdoSineQuadHiLow As RadioButton
    Friend WithEvents rdoSineQuadMidLowHiMid As RadioButton



End Class
