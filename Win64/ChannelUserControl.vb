Imports System.ComponentModel
Imports Tens25.comDef

Public Class ChannelUserControl

    'Private Enum enumChantype As Integer
    '    Unknown = -1
    '    Tens = 0
    '    Motor = 1
    '    System = 2
    'End Enum

    'Private Enum CommandEnum As Integer
    '    cmdNoop = 0
    '    cmdTensOutput = 1
    '    cmdMotorOutput = 2
    '    cmdGoTo = 3
    '    cmdEnd = 4
    '    cmdTest = 5
    '    cmdSet = 6
    '    cmdDelay = 7
    'End Enum


    Public Event Enabled_Click(ByVal index As Integer, ByVal NewState As Boolean)
    Public Event Speed_Changed(ByVal index As Integer, ByVal NewSpeed As Integer)
    Public Event OutputIntensity_Changed(ByVal index As Integer, ByVal NewOutput As Integer)
    Public Event PulseWidth_Changed(ByVal index As Integer, ByVal NewOutput As Integer)

    Dim _chanIndex As Integer = -1
    Dim _chanEnabled As Boolean = False

    Dim _chanName As String

    Dim _minSpeedVal As Integer = 0
    Dim _maxSpeedVal As Integer = 0
    Dim _speedVal As Integer = 0
    'TODO: speed values need a serious overhaul.  TensUI remote sends out a value of -100 (slow) to +100 (fast).
    ' The tens device uses [minSpeed]=50 to [maxSpeed]=500, and those are variable.  
    ' The tens device sends out during sendParamArray a value between [minSpeed] to [maxSpeed]
    ' Here in vb, the slider control uses ???, the _speedVal uses ???, and the Tens device expects to be sent ???

    Dim _progState As Integer = -1
    Dim _lineNum As Integer = -1
    Dim _lineCommand As Integer = -1
    Dim _duration As Integer = -1
    Dim _polarity As Integer = -1
    Dim _repeatsRemaining As Integer = -1
    Dim _percentComplete As Integer = -1

    Dim _outputIntensityMaxPct As Integer = 0
    Dim _outputIntensityPct As Integer = 0
    'Dim _maxOutputIntensityPct As Integer = 0

    Dim _pulseWidthPct As Integer = 0
    Dim _startPulseWidth As Integer = -1
    Dim _endPulseWidth As Integer = -1

    Dim _chanType As enumChantype = enumChantype.Unknown
    Dim _origWidth As Integer = 177 'need to update this if the control gets wider.  


    Dim ControlColorGreen As Color = Color.Lime
    Dim ControlColorNone As Color = Me.BackColor
    Dim ControlColorRed As Color = Color.HotPink

    Dim ignoreSpeedSlider As Boolean = False
    Dim ignoreOutputSlider As Boolean = False
    Dim ignorePulseWidthSlider As Boolean = False


    <Browsable(True)>
    Public Property ChanIndex() As Integer
        Get
            Return _chanIndex
        End Get
        Set(value As Integer)
            _chanIndex = value
        End Set
    End Property

    Public Property ChanType As Integer
        Get
            Return _chanType
        End Get
        Set(value As Integer)
            If _chanType <> value Then
                _chanType = value
                If _chanType = enumChantype.Tens Then
                    grpOutputIntensity.Visible = True
                    grpProgInfo.Visible = True
                    grpPulseWidth.Visible = True
                ElseIf _chanType = enumChantype.Motor Then
                    grpOutputIntensity.Visible = False
                    Width -= (grpOutputIntensity.Width + 2)
                    grpProgInfo.Visible = True
                    grpPulseWidth.Visible = True
                ElseIf _chanType = enumChantype.Master Then
                    grpOutputIntensity.Visible = False
                    Width -= (grpOutputIntensity.Width + 2)
                    grpProgInfo.Visible = False
                    grpPulseWidth.Visible = False
                ElseIf _chanType = enumChantype.Aux Then
                    grpOutputIntensity.Visible = False
                    Width -= (grpOutputIntensity.Width + 2)
                    grpProgInfo.Visible = False
                    grpPulseWidth.Visible = False
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
    Public Property ChanEnabled() As Integer
        Get
            Return _chanEnabled
        End Get
        Set(value As Integer)
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
                Case 0
                    txtVal = "Unknown"
                Case 1
                    txtVal = "Empty"
                Case 2
                    txtVal = "Stopped"
                Case 3
                    txtVal = "Paused"
                Case 4
                    txtVal = "Running"
                Case 5
                    txtVal = "Line Complete"
                Case 6
                    txtVal = "End"
            End Select
            If lblProgState.Text <> txtVal Then
                lblProgState.Text = txtVal
            End If

        End Set
    End Property
    Public Property LineCommand As Integer
        Get
            Return _lineCommand
        End Get
        Set(value As Integer)
            If _lineCommand <> value Then
                _lineCommand = value
                Select Case _lineCommand
                    Case CommandEnum.cmdNoop
                        lblCommand.Text = "-No Op-"
                    Case CommandEnum.cmdTensOutput
                        lblCommand.Text = "Tens Output"
                    Case CommandEnum.cmdMotorOutput
                        lblCommand.Text = "Motor Output"
                    Case CommandEnum.cmdGoTo
                        lblCommand.Text = "GoTo"
                    Case CommandEnum.cmdEnd
                        lblCommand.Text = "End"
                    Case CommandEnum.cmdTest
                        lblCommand.Text = "Test"
                    Case CommandEnum.cmdSet
                        lblCommand.Text = "Set"
                    Case CommandEnum.cmdDelay
                        lblCommand.Text = "Delay"
                    Case Else
                        lblCommand.Text = ""
                End Select
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
                ignoreSpeedSlider = True
                sldSpeed.Value = convertSpeedTensToSlider()
                ignoreSpeedSlider = False
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
                ignoreSpeedSlider = True
                sldSpeed.Value = convertSpeedTensToSlider()
                ignoreSpeedSlider = False
            End If
        End Set
    End Property
    Public Property Speed As Integer
        Get
            Return _speedVal
        End Get
        Set(value As Integer)
            'If _speedVal <> value Then
            _speedVal = value 'Tens-based system [minSpeed] to [maxSpeed]  (approx. 20 to 500)
            Dim tmpVal = convertSpeedTensToSlider()
            If sldSpeed.Value <> tmpVal Then
                ignoreSpeedSlider = True
                sldSpeed.Value = tmpVal
                ignoreSpeedSlider = False
                lblSpeed.Text = _speedVal & "%"
            End If
            'If lblSpeed.Text <> _speedVal & "%" Then
            'lblSpeed.Text = _speedVal & "%"
            'End If
        End Set
    End Property

    Public Property OutputIntensityPct As Integer
        Get
            Return _outputIntensityPct
        End Get
        Set(value As Integer)
            'Dim tmpVal As Integer = If((value > 0), If((value <= _outputIntensityMax), value, _outputIntensityMax), 0)
            Dim tmpVal As Integer = If(value > 0, If(value <= 100, value, 100), 0)
            'If _outputIntensityPct <> tmpVal Then
            _outputIntensityPct = tmpVal
            If grpOutputIntensity.Visible = True Then
                If sldOutputIntensityPct.Value <> _outputIntensityPct Then
                    ignoreOutputSlider = True
                    sldOutputIntensityPct.Value = _outputIntensityPct
                    lblOutputIntensityPct.Text = _outputIntensityPct & "%"
                    ignoreOutputSlider = False
                End If
            End If
            'End If
        End Set
    End Property
    Public Property OutputIntensityMax As Integer
        Get
            Return _outputIntensityMaxPct
        End Get
        Set(value As Integer)
            _outputIntensityMaxPct = value
        End Set
    End Property

    Public Property LineNum As Integer
        Get
            Return _lineNum
        End Get
        Set(value As Integer)
            If _lineNum <> value Then
                _lineNum = value
                If grpProgInfo.Visible = True Then
                    lblLineNum.Text = value
                End If
            End If
        End Set
    End Property

    Public Property Polarity As Integer
        Get
            Return _polarity
        End Get
        Set(value As Integer)
            'If _polarity <> value Then
            _polarity = value
                If grpProgInfo.Visible = True Then
                    If (_lineCommand = CommandEnum.cmdTensOutput) Or (_lineCommand = CommandEnum.cmdMotorOutput) Then
                        Select Case _polarity
                            Case 0
                                lblPolarity.Text = "Fwd"
                            Case 1
                                lblPolarity.Text = "Rev"
                            Case 2
                                lblPolarity.Text = "Tog Pul"
                            Case 3
                                lblPolarity.Text = "Tog Pul"
                            Case 4
                                lblPolarity.Text = "Tog Cyc"
                            Case 5
                                lblPolarity.Text = "Tog Cyc"
                            Case Else
                                lblPolarity.Text = "---"
                        End Select
                    Else
                        lblPolarity.Text = ""
                    End If
                End If
            'End If
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
                    ignorePulseWidthSlider = True
                    sldPulseWidth.Value = _pulseWidthPct
                    lblPulseWidth.Text = _pulseWidthPct & "%"
                    ignorePulseWidthSlider = False
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
                    'lblStartPW.Text = value & "%"
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
                    'lblEndPW.Text = value & "%"
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
            If _percentComplete <> value Then
                _percentComplete = value
                If grpProgInfo.Visible = True Then
                    'If _duration >= 200 Then
                    'barPercentComplete.Value = _percentComplete
                    sbarComplete.PercentComplete = _percentComplete
                    'Else
                    'barPercentComplete.Value = If(_percentComplete < 50, 0, 100)
                    'End If
                End If
            End If
        End Set
    End Property


    Private Sub cmdEnable_Click(sender As Object, e As EventArgs) Handles cmdEnable.Click
        If _chanEnabled = True Then
            _chanEnabled = False
            'cmdEnable.BackColor = ControlColorNone
        Else
            _chanEnabled = True
            'cmdEnable.BackColor = ControlColorGreen
        End If
        RaiseEvent Enabled_Click(ChanIndex, _chanEnabled)
    End Sub

    Private Sub ChannelUserControl_Load(sender As Object, e As EventArgs) Handles Me.Load
        _chanEnabled = False
        'cmdEnable.BackColor = ControlColorNone
        lblSpeed.Text = _speedVal & "%"
        lblRepeatsRemaining.Text = "--"
        lblProgState.Text = ""
        lblCommand.Text = ""
        lblLineNum.Text = "--"
        'lblStartPW.Text = "--"
        'lblEndPW.Text = "--"
        lblDuration.Text = "--"
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
    'Private Function convertSpeedToLabelPercent() As Integer
    '    'The _speedPct value is read and converted to a label text value as follows:
    '    '0 to 99      -->  0 to 99% 
    '    '100          -->  100%
    '    '101 to 1000  -->  101 to 1000%
    '    Dim retVal As Integer = 0
    '    If _speedVal < 100 Then
    '        retVal = map(_speedVal, 0, 100, 0, 100, True)
    '    ElseIf _speedVal = 100 Then
    '        retVal = 100
    '    Else
    '        retVal = map(_speedVal, 100, 1000, 100, 1000, True)
    '    End If
    '    Return retVal
    'End Function

    Private Sub sldSpeed_ValueChanged(sender As Object, e As EventArgs) Handles sldSpeed.ValueChanged
        If ignoreSpeedSlider = False Then
            _speedVal = convertSpeedSliderToTens() '_speedPct = sldSpeed.Value
            lblSpeed.Text = _speedVal & "%" ' convertSpeedToLabelPercent() & "%" 'lblSpeed.Text = sldSpeed.Value
            RaiseEvent Speed_Changed(_chanIndex, _speedVal) 'RaiseEvent Speed_Changed(_chanIndex, sldSpeed.Value)
        End If
    End Sub

    Private Sub sldOutputIntensityPct_ValueChanged(sender As Object, e As EventArgs) Handles sldOutputIntensityPct.ValueChanged
        If ignoreOutputSlider = False Then
            If sldOutputIntensityPct.Value > _outputIntensityMaxPct Then
                sldOutputIntensityPct.Value = _outputIntensityMaxPct
                Exit Sub
            End If
            _outputIntensityPct = sldOutputIntensityPct.Value
            lblOutputIntensityPct.Text = sldOutputIntensityPct.Value & "%"
            RaiseEvent OutputIntensity_Changed(_chanIndex, _outputIntensityPct)

            End If
        If _outputIntensityPct < 40 Then
            If sldOutputIntensityPct.BackColor <> ControlColorNone Then
                sldOutputIntensityPct.BackColor = ControlColorNone
            End If
        ElseIf _outputIntensityPct < 60 Then
            If sldOutputIntensityPct.BackColor <> Color.Yellow Then
                sldOutputIntensityPct.BackColor = Color.Yellow
            End If
        ElseIf _outputIntensityPct >= 60 Then
            If sldOutputIntensityPct.BackColor <> Color.Red Then
                sldOutputIntensityPct.BackColor = Color.Red
            End If
        End If
    End Sub

    Private Sub sldPulseWidth_ValueChanged(sender As Object, e As EventArgs) Handles sldPulseWidth.ValueChanged
        If ignorePulseWidthSlider = False Then
            _pulseWidthPct = sldPulseWidth.Value
            lblPulseWidth.Text = sldPulseWidth.Value & "%"
            RaiseEvent PulseWidth_Changed(_chanIndex, _pulseWidthPct)
        End If
    End Sub

    Private Sub lblSpeed_DoubleClick(sender As Object, e As EventArgs) Handles lblSpeed.DoubleClick
        sldSpeed.Value = 50
    End Sub
End Class
