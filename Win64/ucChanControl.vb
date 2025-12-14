Imports System.ComponentModel
Imports Accessibility
Imports Tens2502.comDef
Public Class ucChanControl

    Public Event Enabled_Click(ByVal index As Integer, ByVal NewState As Boolean)
    Public Event Speed_Changed(ByVal index As Integer, ByVal NewSpeed As Integer)
    Public Event OutputIntensity_Changed(ByVal index As Integer, ByVal NewOutput As Integer)
    Public Event PulseWidth_Changed(ByVal index As Integer, ByVal NewOutput As Integer)
    Public Event IntensityMin_Changed(ByVal index As Integer, ByVal newMin As Integer)
    Public Event IntensityMax_Changed(ByVal index As Integer, ByVal newMax As Integer)
    Public Event SwapPolarity_Changed(ByVal index As Integer, ByVal PolarityIsSwapped As Integer)

    Dim _chanIndex As Integer = -1
    Dim _chanEnabled As Boolean = False

    Dim _progRef As List(Of Program)


    'Dim _progName As String
    Dim _chanName As String

    Dim _minSpeedVal As Integer = 0
    Dim _maxSpeedVal As Integer = 0
    Dim _speedVal As Integer = 0
    'TODO: speed values need a serious overhaul.  TensUI remote sends out a value of -100 (slow) to +100 (fast).
    ' The tens device uses [minSpeed]=50 to [maxSpeed]=500, and those are variable.  
    ' The tens device sends out during sendParamArray a value between [minSpeed] to [maxSpeed]
    ' Here in vb, the slider control uses ???, the _speedVal uses ???, and the Tens device expects to be sent ???

    Dim _progState As Integer = -1
    Dim _progNum As Integer = -1
    Dim _lineNum As Integer = -1
    Dim _duration As Integer = -1
    Dim _polarity As Integer = -1

    Dim _polaritySwapped As Integer = False
    Dim _polarityNotSwappedBackColor As Color = SystemColors.Control
    Dim _polarityNotSwappedForeColor As Color = SystemColors.ControlText
    Dim _polaritySwappedBackColor As Color = Color.LightGreen
    Dim _polaritySwappedForeColor As Color = Color.Blue

    Dim _repeatsRemaining As Integer = -1
    Dim _percentComplete As Integer = -1

    Dim _outputIntensityMinPct As Integer = 0
    Dim _outputIntensityMaxPct As Integer = 0
    Dim _outputIntensityPct As Integer = 0

    Dim _pulseWidthPct As Integer = 0
    Dim _startPulseWidth As Integer = -1
    Dim _endPulseWidth As Integer = -1

    Dim _chanType As enumChantype = enumChantype.Unknown
    Dim _origWidth As Integer = 177 'need to update this if the control gets wider.  


    Dim ControlColorGreen As Color = Color.Lime
    Dim ControlColorNone As Color = Me.BackColor
    Dim ControlColorRed As Color = Color.HotPink

    'Dim ignoreSpeedSlider As Boolean = False
    'Dim ignoreOutputSlider As Boolean = False
    'Dim ignorePulseWidthSlider As Boolean = False


    <Browsable(True)>
    Public Property ChanIndex() As Integer
        Get
            Return _chanIndex
        End Get
        Set(value As Integer)
            _chanIndex = value
        End Set
    End Property

    'Public Property ProgRef() As Object
    '    Get
    '        Return _progRef
    '    End Get
    '    Set(value As Object)
    '        _progRef = value
    '    End Set
    'End Property
    Friend Sub SetProgRef(ByRef ListOProg As List(Of Program))
        _progRef = ListOProg
    End Sub
    Friend Function GetProgRef() As List(Of Program)
        Return _progRef
    End Function

    Public Property ChanType As Integer
        Get
            Return _chanType
        End Get
        Set(value As Integer)
            If _chanType <> value Then
                _chanType = value
                If _chanType = enumChantype.Tens Then
                    grpIntensity.Visible = True
                    grpOutputIntensity.Visible = True
                    grpProgInfo.Visible = True
                    grpPulseWidth.Visible = True
                    Me.Width = WidthWithTens
                ElseIf _chanType = enumChantype.Motor Then
                    grpIntensity.Visible = True
                    grpOutputIntensity.Visible = False
                    Width -= (grpOutputIntensity.Width + 2)
                    grpProgInfo.Visible = True
                    grpPulseWidth.Visible = True
                    Me.Width = WidthWithTens
                ElseIf _chanType = enumChantype.Master Then
                    grpIntensity.Visible = False
                    grpOutputIntensity.Visible = False
                    Width -= (grpOutputIntensity.Width + 2)
                    grpProgInfo.Visible = False
                    grpPulseWidth.Visible = False
                    Me.Width = WidthWithoutTens
                ElseIf _chanType = enumChantype.Aux Then
                    grpIntensity.Visible = False
                    grpOutputIntensity.Visible = False
                    Width -= (grpOutputIntensity.Width + 2)
                    grpProgInfo.Visible = False
                    grpPulseWidth.Visible = False
                    Me.Width = WidthWithoutTens
                End If
            End If
        End Set
    End Property
    Public ReadOnly Property WidthWithTens As Integer
        Get
            Return _origWidth
        End Get
    End Property
    Public ReadOnly Property WidthWithoutTens As Integer
        Get
            Return _origWidth - (grpOutputIntensity.Width + 2)
        End Get
    End Property

    <Browsable(True)>
    Public Property ChanEnabled() As Boolean
        Get
            Return _chanEnabled
        End Get
        Set(value As Boolean)
            _chanEnabled = value
            If value = True Then
                cmdEnable.BackColor = ControlColorGreen
            Else
                cmdEnable.BackColor = ControlColorNone
            End If
        End Set
    End Property

    Public Property ProgState As Integer
        Get
            Return _progState
        End Get
        Set(value As Integer)
            If _progState <> value Then
                _progState = value
            End If
            Dim txtVal As String = ""
            Select Case _progState
                Case ProgStateEnum.progState_Unknown
                    txtVal = "Unknown"
                Case ProgStateEnum.progState_Empty
                    txtVal = "Empty"
                Case ProgStateEnum.progState_Stopped
                    txtVal = "Stopped"
                Case ProgStateEnum.progState_Paused
                    txtVal = "Paused"
                Case ProgStateEnum.progState_Running
                    txtVal = "Running"
                Case ProgStateEnum.progState_LineComplete
                    txtVal = "Line Complete"
                Case ProgStateEnum.progState_End
                    txtVal = "End"
            End Select
            If lblProgState.Text <> txtVal Then
                lblProgState.Text = txtVal
            End If

        End Set
    End Property

    <Browsable(True)>
    Public Property ChanName As String
        Get
            Return _chanName
        End Get
        Set(value As String)
            _chanName = value
            cmdEnable.Text = value
        End Set
    End Property

    Public Property MinSpeed As Integer
        Get
            Return _minSpeedVal
        End Get
        Set(value As Integer)
            If _minSpeedVal <> value Then
                _minSpeedVal = value
                RemoveHandler sldSpeed.ValueChanged, AddressOf sldSpeed_ValueChanged
                sldSpeed.Value = convertSpeedTensToSlider()
                AddHandler sldSpeed.ValueChanged, AddressOf sldSpeed_ValueChanged
            End If
        End Set
    End Property

    Public Property MaxSpeed As Integer
        Get
            Return _maxSpeedVal
        End Get
        Set(value As Integer)
            If _maxSpeedVal <> value Then
                _maxSpeedVal = value
                RemoveHandler sldSpeed.ValueChanged, AddressOf sldSpeed_ValueChanged
                sldSpeed.Value = convertSpeedTensToSlider()
                AddHandler sldSpeed.ValueChanged, AddressOf sldSpeed_ValueChanged
            End If
        End Set
    End Property
    Public Property Speed As Integer
        Get
            Return _speedVal
        End Get
        Set(value As Integer)
            _speedVal = value 'Tens-based system [minSpeed] to [maxSpeed]  (approx. 20 to 500)
            Dim tmpVal = convertSpeedTensToSlider()
            If sldSpeed.Value <> tmpVal Then
                RemoveHandler sldSpeed.ValueChanged, AddressOf sldSpeed_ValueChanged
                sldSpeed.Value = tmpVal
                AddHandler sldSpeed.ValueChanged, AddressOf sldSpeed_ValueChanged
                lblSpeed.Text = _speedVal & "%"
            End If
        End Set
    End Property

    Public Property OutputIntensityPct As Integer
        Get
            Return _outputIntensityPct
        End Get
        Set(value As Integer)
            Dim tmpVal As Integer = If(value > 0, If(value <= 100, value, 100), 0)
            _outputIntensityPct = tmpVal
            'If grpOutputIntensity.Visible = True Then
            If sldOutputIntensityPct.Value <> _outputIntensityPct Then
                RemoveHandler sldOutputIntensityPct.ValueChanged, AddressOf sldOutputIntensityPct_ValueChanged
                sldOutputIntensityPct.Value = _outputIntensityPct
                lblOutputIntensityPct.Text = _outputIntensityPct & "%"
                AddHandler sldOutputIntensityPct.ValueChanged, AddressOf sldOutputIntensityPct_ValueChanged
            End If
            'End If
            UpdateOutputIntensitySliderColor()
        End Set
    End Property

    Public Property OutputIntensityMin As Integer
        Get
            Return _outputIntensityMinPct
        End Get
        Set(value As Integer)
            _outputIntensityMinPct = value
            'If cboIntensityMin.SelectedIndex >= 0 Then
            '    If cboIntensityMin.SelectedIndex <> value Then
            RemoveHandler cboIntensityMin.SelectedIndexChanged, AddressOf cboIntensityMin_SelectedIndexChanged
            If cboIntensityMin.Items.Count < 100 Then
                AddIntensityMinMaxItems()
            End If
            cboIntensityMin.SelectedIndex = value
            AddHandler cboIntensityMin.SelectedIndexChanged, AddressOf cboIntensityMin_SelectedIndexChanged
            '    End If
            'End If
        End Set
    End Property

    Private Sub AddIntensityMinMaxItems()
        RemoveHandler cboIntensityMin.SelectedIndexChanged, AddressOf cboIntensityMin_SelectedIndexChanged
        RemoveHandler cboIntensityMax.SelectedIndexChanged, AddressOf cboIntensityMin_SelectedIndexChanged
        cboIntensityMin.Items.Clear()
        cboIntensityMax.Items.Clear()
        For n As Integer = 0 To 100
            cboIntensityMin.Items.Add(n)
            cboIntensityMax.Items.Add(n)
        Next
        cboIntensityMin.SelectedIndex = 0
        cboIntensityMax.SelectedIndex = 0
        AddHandler cboIntensityMin.SelectedIndexChanged, AddressOf cboIntensityMin_SelectedIndexChanged
        AddHandler cboIntensityMax.SelectedIndexChanged, AddressOf cboIntensityMin_SelectedIndexChanged
    End Sub

    Public Property OutputIntensityMax As Integer
        Get
            Return _outputIntensityMaxPct
        End Get
        Set(value As Integer)
            _outputIntensityMaxPct = value
            'If cboIntensityMax.SelectedIndex >= 0 Then
            '    If cboIntensityMax.SelectedIndex <> value Then
            RemoveHandler cboIntensityMax.SelectedIndexChanged, AddressOf cboIntensityMax_SelectedIndexChanged
            If cboIntensityMax.Items.Count < 100 Then
                AddIntensityMinMaxItems()
            End If
            cboIntensityMax.SelectedIndex = value
            AddHandler cboIntensityMax.SelectedIndexChanged, AddressOf cboIntensityMax_SelectedIndexChanged
            '    End If
            'End If

        End Set
    End Property

    Public Property ProgNum As Integer
        Get
            Return _progNum
        End Get
        Set(value As Integer)
            _progNum = value
        End Set
    End Property

    Public Property LineNum As Integer
        Get
            Return _lineNum
        End Get
        Set(value As Integer)
            'Line Number
            If _lineNum <> value Then
                _lineNum = value
                If grpProgInfo.Visible = True Then
                    lblLineNum.Text = value
                End If
            End If

            If _progNum >= 0 Then

                'Program Name:
                If Not IsNothing(_progRef) Then
                    If String.Compare(lblProgName.Text, _progRef(_progNum).progName) <> 0 Then
                        lblProgName.Text = _progRef(_progNum).progName
                    End If
                End If

                Dim tmpCmdText As String = ""
                Dim pLine() As Integer = _progRef(_progNum).progLine(_lineNum)
                Select Case pLine(DataFieldEnum.dfCommand)
                    Case CommandEnum.cmdNoop
                        tmpCmdText = "-No Op-"
                    Case CommandEnum.cmdTenMotOutput
                        tmpCmdText = "Ten/Mot Output"
                    Case CommandEnum.cmdGoTo
                        tmpCmdText = "GoTo"
                    Case CommandEnum.cmdEnd
                        tmpCmdText = "End"
                    Case CommandEnum.cmdTest
                        tmpCmdText = "Test"
                    Case CommandEnum.cmdSet
                        tmpCmdText = "Set"
                    Case CommandEnum.cmdDelay
                        tmpCmdText = "Delay"
                    Case Else
                        tmpCmdText = ""
                End Select
                If String.Compare(tmpCmdText, lblCommand.Text) <> 0 Then
                    lblCommand.Text = tmpCmdText
                End If

            End If
        End Set
    End Property

    Public Property Polarity As Integer
        Get
            Return _polarity
        End Get
        Set(value As Integer)
            _polarity = value
            If (_progNum >= 0) And (_lineNum >= 0) Then
                If _progNum >= _progRef.Count Then Exit Property
                If _lineNum >= _progRef(_progNum).progLine.Count Then Exit Property
                If grpProgInfo.Visible = True Then
                    Dim tmpPolText As String = ""
                    Dim pLine() As Integer = _progRef(_progNum).progLine(_lineNum)
                    If pLine(DataFieldEnum.dfCommand) = CommandEnum.cmdTenMotOutput Then
                        Select Case _polarity
                            Case 0
                                tmpPolText = "Fwd"
                            Case 1
                                tmpPolText = "Rev"
                            Case 2
                                tmpPolText = "Tog Pul"
                            Case 3
                                tmpPolText = "Tog Pul"
                            Case 4
                                tmpPolText = "Tog Cyc"
                            Case 5
                                tmpPolText = "Tog Cyc"
                            Case Else
                                tmpPolText = "---"
                        End Select
                    Else
                        tmpPolText = ""
                    End If
                    If String.Compare(tmpPolText, tmpPolText) <> 0 Then
                        lblPolarityVal.Text = tmpPolText
                    End If
                End If
            Else
                lblPolarityVal.Text = "---"
            End If
        End Set
    End Property

    Public Property PolaritySwapped As Boolean
        Get
            Return _polaritySwapped
        End Get
        Set(value As Boolean)
            _polaritySwapped = value
            SwapPolarity(value, False)
        End Set
    End Property

    Public Property PulseWidth As Integer
        Get
            Return _pulseWidthPct
        End Get
        Set(value As Integer)
            If _pulseWidthPct <> value Then
                _pulseWidthPct = value
                If grpPulseWidth.Visible = True Then
                    sbarComplete.PulseWidthCurVal = _pulseWidthPct
                    RemoveHandler sldPulseWidth.ValueChanged, AddressOf sldPulseWidth_ValueChanged
                    sldPulseWidth.Value = _pulseWidthPct
                    lblPulseWidth.Text = _pulseWidthPct & "%"
                    AddHandler sldPulseWidth.ValueChanged, AddressOf sldPulseWidth_ValueChanged
                End If
            End If
        End Set
    End Property

    Public Property PulseWidthStart As Integer
        Get
            Return _startPulseWidth
        End Get
        Set(value As Integer)
            If _startPulseWidth <> value Then
                _startPulseWidth = value
                If grpProgInfo.Visible = True Then
                    sbarComplete.MinValue = _startPulseWidth
                End If
            End If
        End Set
    End Property
    Public Property PulseWidthEnd As Integer
        Get
            Return _endPulseWidth
        End Get
        Set(value As Integer)
            If _endPulseWidth <> value Then
                _endPulseWidth = value
                If grpProgInfo.Visible = True Then
                    sbarComplete.MaxValue = _endPulseWidth
                End If
            End If
        End Set
    End Property

    Public Property Duration As Integer
        Get
            Return _duration
        End Get
        Set(value As Integer)
            If _duration <> value Then
                _duration = value
                If grpProgInfo.Visible = True Then
                    lblDuration.Text = _duration
                End If
            End If
            If (_progNum >= 0) And (_lineNum >= 0) Then
                If _progNum >= _progRef.Count Then Exit Property
                If _lineNum >= _progRef(_progNum).progLine.Count Then Exit Property

                Dim pLine() As Integer = _progRef(_progNum).progLine(_lineNum)
                Dim tmpCmd As CommandEnum = pLine(DataFieldEnum.dfCommand)
                If (tmpCmd = CommandEnum.cmdDelay) And (lblCommand.Text <> "Delay (" & value & ")") Then
                    lblCommand.Text = "Delay (" & value & ")"
                End If
            Else
                lblCommand.Text = "---"
            End If
        End Set
    End Property

    Public Property RepeatsRemaining As Integer
        Get
            Return _repeatsRemaining
        End Get
        Set(value As Integer)
            If _repeatsRemaining <> value Then
                _repeatsRemaining = value
                If grpProgInfo.Visible = True Then
                    If _repeatsRemaining > 0 Then
                        lblRepeatsRemaining.Text = _repeatsRemaining
                    Else
                        lblRepeatsRemaining.Text = ""
                    End If
                End If
            End If
        End Set
    End Property

    Public Property PercentComplete As Integer
        Get
            Return _percentComplete
        End Get
        Set(value As Integer)
            'If _percentComplete <> value Then
            '    _percentComplete = value
            '    If grpProgInfo.Visible = True Then
            '        sbarComplete.PercentComplete = _percentComplete
            '    End If
            'End If
            If _percentComplete <> value Then
                _percentComplete = value
                If grpProgInfo.Visible = True Then
                    sbarComplete.PercentComplete = _percentComplete
                End If
            End If
        End Set
    End Property


    Private Sub cmdEnable_Click(sender As Object, e As EventArgs) Handles cmdEnable.Click
        If _chanEnabled = True Then
            _chanEnabled = False
        Else
            _chanEnabled = True
        End If
        RaiseEvent Enabled_Click(ChanIndex, _chanEnabled)
    End Sub

    Private Sub ChannelUserControl_Load(sender As Object, e As EventArgs) Handles Me.Load
        _chanEnabled = False
        lblSpeed.Text = _speedVal & "%"
        lblRepeatsRemaining.Text = "--"
        lblProgName.Text = ""
        lblProgState.Text = ""
        lblCommand.Text = ""
        lblLineNum.Text = "--"
        lblDuration.Text = "--"

        If (cboIntensityMax.Items.Count < 100) Or (cboIntensityMin.Items.Count < 100) Then
            AddIntensityMinMaxItems()
        End If
        cboIntensityMin.SelectedIndex = _outputIntensityMinPct
        cboIntensityMax.SelectedIndex = _outputIntensityMaxPct
        AddHandler cboIntensityMin.SelectedIndexChanged, AddressOf cboIntensityMin_SelectedIndexChanged
        AddHandler cboIntensityMax.SelectedIndexChanged, AddressOf cboIntensityMax_SelectedIndexChanged

        AddHandler sldSpeed.ValueChanged, AddressOf sldSpeed_ValueChanged
        AddHandler sldPulseWidth.ValueChanged, AddressOf sldPulseWidth_ValueChanged
        AddHandler sldOutputIntensityPct.ValueChanged, AddressOf sldOutputIntensityPct_ValueChanged

    End Sub

    Private Function map(ByVal inVal As Integer, ByVal inMin As Integer, ByVal inMax As Integer,
                         ByVal outMin As Integer, ByVal outMax As Integer, Optional ByVal clampLimits As Boolean = False) As Integer
        Dim retVal As Integer = (inVal - inMin) * (outMax - outMin) / (inMax - inMin) + outMin
        If clampLimits Then
            retVal = If(retVal < outMin, outMin, If(retVal > outMax, outMax, retVal))
        End If
        Return retVal
    End Function
    Private Function convertSpeedSliderToTens() As Integer
        'The curVal of the slider is read and converted as follows:
        '0 to 49   -->  minSpeed to 99
        '50        -->  100
        '51 to 100 -->  101 to maxSpeed
        Dim retVal As Integer = 0
        If sldSpeed.Value < 49 Then
            retVal = map(sldSpeed.Value, 0, 50, _minSpeedVal, 100, True)
        ElseIf sldSpeed.Value = 50 Then
            retVal = 100
        Else
            retVal = map(sldSpeed.Value, 50, 100, 100, _maxSpeedVal, True)
        End If
        Return retVal
    End Function
    Private Function convertSpeedTensToSlider() As Integer
        'The _speedPct value is read and converted to a slider value as follows:
        '_minSpeedVal - 99   -->  0 to 49
        '100                 -->  50
        '101 to _maxSpeedVal -->  51 to 100
        Dim retVal As Integer = 0
        If _speedVal < 100 Then
            retVal = map(_speedVal, _minSpeedVal, 100, 0, 50, True)
        ElseIf _speedVal = 100 Then
            retVal = 50
        Else
            retVal = map(_speedVal, 100, _maxSpeedVal, 50, 100, True)
        End If
        Return retVal
    End Function

    Private Sub sldSpeed_ValueChanged(sender As Object, e As EventArgs)
        _speedVal = convertSpeedSliderToTens() '_speedPct = sldSpeed.Value
        lblSpeed.Text = _speedVal & "%" ' convertSpeedToLabelPercent() & "%" 'lblSpeed.Text = sldSpeed.Value
        RaiseEvent Speed_Changed(_chanIndex, _speedVal) 'RaiseEvent Speed_Changed(_chanIndex, sldSpeed.Value)
    End Sub

    Private Sub sldOutputIntensityPct_ValueChanged(sender As Object, e As EventArgs)
        _outputIntensityPct = sldOutputIntensityPct.Value
        lblOutputIntensityPct.Text = sldOutputIntensityPct.Value & "%"
        RaiseEvent OutputIntensity_Changed(_chanIndex, _outputIntensityPct)

        'If _outputIntensityPct < 51 Then
        '    If sldOutputIntensityPct.BackColor <> ControlColorNone Then
        '        sldOutputIntensityPct.BackColor = ControlColorNone
        '    End If
        'ElseIf _outputIntensityPct < 75 Then
        '    If sldOutputIntensityPct.BackColor <> Color.Yellow Then
        '        sldOutputIntensityPct.BackColor = Color.Yellow
        '    End If
        'Else
        '    If sldOutputIntensityPct.BackColor <> Color.Red Then
        '        sldOutputIntensityPct.BackColor = Color.Red
        '    End If
        'End If
        UpdateOutputIntensitySliderColor()
    End Sub

    Private Sub UpdateOutputIntensitySliderColor()
        If _outputIntensityPct < 51 Then
            If sldOutputIntensityPct.BackColor <> ControlColorNone Then
                sldOutputIntensityPct.BackColor = ControlColorNone
            End If
        ElseIf _outputIntensityPct < 75 Then
            If sldOutputIntensityPct.BackColor <> Color.Yellow Then
                sldOutputIntensityPct.BackColor = Color.Yellow
            End If
        Else
            If sldOutputIntensityPct.BackColor <> Color.Red Then
                sldOutputIntensityPct.BackColor = Color.Red
            End If
        End If
    End Sub

    Private Sub sldPulseWidth_ValueChanged(sender As Object, e As EventArgs)
        'If ignorePulseWidthSlider = False Then
        _pulseWidthPct = sldPulseWidth.Value
        lblPulseWidth.Text = sldPulseWidth.Value & "%"
        RaiseEvent PulseWidth_Changed(_chanIndex, _pulseWidthPct)
        'End If
    End Sub

    Private Sub lblSpeed_DoubleClick(sender As Object, e As EventArgs) Handles lblSpeed.DoubleClick
        sldSpeed.Value = 50
    End Sub

    Private Sub cboIntensityMin_SelectedIndexChanged(sender As Object, e As EventArgs)
        RaiseEvent IntensityMin_Changed(_chanIndex, cboIntensityMin.SelectedIndex)
    End Sub

    Private Sub cboIntensityMax_SelectedIndexChanged(sender As Object, e As EventArgs)
        RaiseEvent IntensityMax_Changed(_chanIndex, cboIntensityMax.SelectedIndex)
    End Sub

    Private Sub lblPolarityLabel_Click(sender As Object, e As EventArgs) Handles lblPolarityLabel.Click
        SwapPolarity(Not _polaritySwapped, True)
    End Sub

    Private Sub lblPolarityVal_Click(sender As Object, e As EventArgs) Handles lblPolarityVal.Click
        SwapPolarity(Not _polaritySwapped, True)
    End Sub

    Private Sub SwapPolarity(PolIsSwapped As Boolean, ByVal AllowRaiseEvent As Boolean)
        _polaritySwapped = PolIsSwapped
        If _polaritySwapped = True Then
            pnlPolarity.BackColor = _polaritySwappedBackColor
            lblPolarityLabel.BackColor = _polaritySwappedBackColor
            lblPolarityLabel.ForeColor = _polaritySwappedForeColor
            lblPolarityLabel.BackColor = _polaritySwappedBackColor
            lblPolarityVal.ForeColor = _polaritySwappedForeColor
        Else
            pnlPolarity.BackColor = _polarityNotSwappedBackColor
            lblPolarityLabel.BackColor = _polarityNotSwappedBackColor
            lblPolarityLabel.ForeColor = _polarityNotSwappedForeColor
            lblPolarityLabel.BackColor = _polarityNotSwappedBackColor
            lblPolarityVal.ForeColor = _polarityNotSwappedForeColor
        End If

        If AllowRaiseEvent Then
            RaiseEvent SwapPolarity_Changed(_chanIndex, If(_polaritySwapped = False, 0, 1))
        End If
    End Sub

    Private Sub lblSpeed_Click(sender As Object, e As EventArgs) Handles lblSpeed.Click
        RaiseEvent Speed_Changed(_chanIndex, 100)
    End Sub

    Private Sub lblOutputIntensityPct_Click(sender As Object, e As EventArgs) Handles lblOutputIntensityPct.Click
        If _outputIntensityPct > 0 Then
            RaiseEvent OutputIntensity_Changed(_chanIndex, _outputIntensityPct / 2)
        End If
    End Sub

    Private Sub lblOutputIntensityPct_DoubleClick(sender As Object, e As EventArgs) Handles lblOutputIntensityPct.DoubleClick
        RaiseEvent OutputIntensity_Changed(_chanIndex, 0)
    End Sub


End Class

