Imports System.ComponentModel
Imports System.Diagnostics.Eventing.Reader
'Imports System.Net

Public Class ucProgLineEdit

    Dim devRef As Device_t

    Dim TopPanelY As Integer = 50
    Dim grpBuffer As Integer = 5
    Dim errorColor As Color = Color.Yellow ' Color.LightYellow
    Dim normalControlColor As Color = SystemColors.ControlLightLight

    Dim waveformString As String = "Not Selected"

    Dim _ProgLine(DataFieldLen - 1) As Integer
    Dim _ProgLineOrig(DataFieldLen - 1) As Integer 'This holds a copy of the original data and it is not changed by the controls on the form.  Therefore it can be compared with the _ProgLine array at any time to see if the _ProgLine data has been changed in any way by the user.
    Dim _digInCount As Integer = -1
    Dim _digOutCount As Integer = -1
    Dim _progVarCount As Integer = -1
    Dim _progTimerCount As Integer = -1

    Dim _channelCount As Integer = -1
    'Dim _ProgLineCount As Integer = -1  'used to verify the values for GotoTrue & GotoFalse fields

    Dim _curProgNum As Integer = -1 'used when saving.  Not really used internally to this control, only used for the calling code
    Dim _curLineNum As Integer = -1 'used when saving.  Not really used internally to this control, only used for the calling code

    Dim _enabled As Boolean = False 'If false, no data is displayed on the control.

    Public Event RequestSave(ByVal ProgramNum As Integer, ByVal LineNum As Integer, ByVal updatedProgLine() As Integer)


    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Overloads Property Enabled As Boolean
        Get
            Return _enabled
        End Get
        Set(value As Boolean)
            _enabled = value
            If _enabled = False Then
                HideControlGroups()
                pnlCommand.Visible = False
            Else
                pnlCommand.Visible = True
                cboCommand.SelectedIndex = _ProgLine(DataFieldEnum.dfCommand)
            End If
        End Set
    End Property

    Friend Sub SetDeviceRef(ByRef Dev As Device_t)
        devRef = Dev
    End Sub
    Friend Function GetDeviceRef() As Device_t
        Return devRef
    End Function

    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property ProgLine() As Integer()
        Get
            Return _ProgLine
        End Get
        Set(value() As Integer)
            value.CopyTo(_ProgLine, 0)
            '_ProgLine(Index) = value
        End Set
    End Property
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property DigInCount As Integer
        Get
            Return _digInCount
        End Get
        Set(value As Integer)
            _digInCount = value
        End Set
    End Property
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property DigOutCount As Integer
        Get
            Return _digOutCount
        End Get
        Set(value As Integer)
            _digOutCount = value
        End Set
    End Property
    '<DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    'Public Property SysVarCount As Integer
    '    Get
    '        Return _sysVarCount
    '    End Get
    '    Set(value As Integer)
    '        _sysVarCount = value
    '    End Set
    'End Property
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property ProgVarCount As Integer
        Get
            Return _progVarCount
        End Get
        Set(value As Integer)
            _progVarCount = value
            UpdateProgVarCountInAllValControls(Me.Controls)
        End Set
    End Property
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property ProgTimerCount As Integer
        Get
            Return _progTimerCount
        End Get
        Set(value As Integer)
            _progTimerCount = value
            UpdateTimerVarCountInAllValControls(Me.Controls)
        End Set
    End Property
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property ChannelCount As Integer
        Get
            Return _channelCount
        End Get
        Set(value As Integer)
            _channelCount = value
        End Set
    End Property
    '<DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    'Public Property ProgLineCount As Integer
    '    Get
    '        Return _ProgLineCount
    '    End Get
    '    Set(value As Integer)
    '        _ProgLineCount = value
    '    End Set
    'End Property
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property LineNumber As Integer
        Get
            Return _curLineNum
        End Get
        Set(value As Integer)
            _curLineNum = value
        End Set
    End Property
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property ProgramNumber As Integer
        Get
            Return _curProgNum
        End Get
        Set(value As Integer)
            _curProgNum = value
            UpdateCurProgramNumberInAllValControls(Me.Controls)
        End Set
    End Property

#Region "ProgramLineSentences"
    Public Function CreateProgLineSentence(ByVal ProgNum As Integer, ByVal LineNum As Integer) As String
        'Accepts a programLine array and converts it to a readable sentence.
        Dim retVal As String = ""

        _curProgNum = ProgNum
        _curLineNum = LineNum
        Dim pLine() As Integer = devRef.Prog(ProgNum).progLine(LineNum)

        If pLine.Length <> DataFieldLen Then
            retVal = "Error 1: Invalid data length provided."
            Return retVal
        End If

        Select Case pLine(DataFieldEnum.dfCommand)
            Case CommandEnum.cmdNoop
                retVal = "No Op."
            Case CommandEnum.cmdTenMotOutput
                retVal = ProgLineSentenceTenMot(pLine)
            Case CommandEnum.cmdGoTo
                retVal = ProgLineSentenceGoto(pLine)
            Case CommandEnum.cmdEnd
                retVal = "End Program"
            Case CommandEnum.cmdTest
                retVal = ProgLineSentenceTest(pLine)
            Case CommandEnum.cmdSet
                retVal = ProgLineSentenceSet(pLine)
            Case CommandEnum.cmdDelay
                retVal = ProgLineSentenceDelay(pLine)
            Case CommandEnum.cmdProgControl
                retVal = ProgLineSentenceProgControl(pLine)
                'Case CommandEnum.cmdSetModifier
            '    retVal = ProgLineSentenceSetModifier(pLine)
            Case CommandEnum.cmdDisplay
                retVal = ProgLineSentenceDisplay(pLine)
            Case CommandEnum.cmdSendMsg
                retVal = ProgLineSentenceSendMsg(pLine)
            Case Else
                retVal = "Error 2: Invalid command."
        End Select

        Return retVal
    End Function

    Private Function GetValueSentence(ByVal source As DataSourceEnum, ByVal val1 As Integer, Optional ByVal val2 As Integer = 0) As String
        Dim retVal As String
        If (source < 0) Or (val1 < 0) Then
            retVal = "ERROR"
            Return retVal
        End If
        Select Case source
            Case DataSourceEnum.dsDirect
                retVal = val1.ToString
            Case DataSourceEnum.dsProgramVar
                retVal = "[ProgVar " & val1 & ": " & devRef.Prog(_curProgNum).varName(val1) & "]"
            Case DataSourceEnum.dsSysVar
                retVal = "[SysVar " & val1 & ": " & devRef.Prog(0).varName(val1) & "]"
            Case DataSourceEnum.dsChanSetting
                retVal = "[Chan Setting: "
                'Channel will be in val2, setting will be in val1.
                If val2 < devRef.allChannelsStringPlusThisChan.Length Then
                    retVal &= devRef.allChannelsStringPlusThisChan(val2) & " "
                Else
                    retVal &= "<ERROR! Invalid Channel> "
                End If

                If val1 < dataSourceChanSettingString.Length Then
                    retVal &= dataSourceChanSettingString(val1)
                Else
                    retVal &= "<ERROR! Invalid Setting> "
                End If
                retVal &= "]"
            Case DataSourceEnum.dsSysSetting
                retVal = "[Sys Setting: "
                If val1 < dataSourceSysSettingString.Length Then
                    retVal &= dataSourceSysSettingString(val1)
                Else
                    retVal &= "<ERROR! Invalid Setting> "
                End If
                retVal &= "]"
            Case DataSourceEnum.dsDigIn
                retVal = "[Dig Input " & val1 & "]"
            Case DataSourceEnum.dsDigOut
                retVal = "[Dig Output " & val1 & "]"
            Case DataSourceEnum.dsRandom
                retVal = "[Rand # from " & val1 & " to " & val2 & "]"
            Case DataSourceEnum.dsTimer
                retVal = "[Timer " & val1 & ": " & devRef.Prog(_curProgNum).timerName(val1) & "]"
            Case Else
                retVal = "[ERROR! Invalid Source]"
        End Select
        Return retVal
    End Function
    Private Function ProgLineSentenceTenMot(ByRef pLine() As Integer) As String
        Dim retVal As String = "PWM Output.  "
        Select Case pLine(DataFieldEnum.dfGotoTrue)
            Case WaveformEnum.wfRamp
                If (pLine(DataFieldEnum.df81S) = pLine(DataFieldEnum.df82S)) And (pLine(DataFieldEnum.df81V1) = pLine(DataFieldEnum.df82V1)) And (pLine(DataFieldEnum.df81V2) = pLine(DataFieldEnum.df82V2)) Then
                    'Constant output
                    retVal &= "Constant output at " & GetValueSentence(pLine(DataFieldEnum.df81S), pLine(DataFieldEnum.df81V1), pLine(DataFieldEnum.df81V2)) & "%"
                Else
                    'Ramp output
                    retVal &= "Ramp from " & GetValueSentence(pLine(DataFieldEnum.df81S), pLine(DataFieldEnum.df81V1), pLine(DataFieldEnum.df81V2))
                    retVal &= "% to " & GetValueSentence(pLine(DataFieldEnum.df82S), pLine(DataFieldEnum.df82V1), pLine(DataFieldEnum.df82V2))
                    retVal &= "%"
                End If
            Case WaveformEnum.wfTriangle
                retVal &= "Triangle from " & GetValueSentence(pLine(DataFieldEnum.df81S), pLine(DataFieldEnum.df81V1), pLine(DataFieldEnum.df81V2))
                retVal &= "% to " & GetValueSentence(pLine(DataFieldEnum.df82S), pLine(DataFieldEnum.df82V1), pLine(DataFieldEnum.df82V2))
                retVal &= "% then back to " & GetValueSentence(pLine(DataFieldEnum.df81S), pLine(DataFieldEnum.df81V1), pLine(DataFieldEnum.df81V2))
                retVal &= "%, DutyCycle: " & pLine(DataFieldEnum.dfGotoFalse) & "%" ' & GetValueSentence(pLine(DataFieldEnum.df323S), pLine(DataFieldEnum.df323V1), pLine(DataFieldEnum.df323V2))
            Case WaveformEnum.wfSine
                Dim quad As Integer = pLine(DataFieldEnum.dfGotoFalse)
                retVal &= "Sine (" & quadrantString(quad)
                retVal &= ") from " & GetValueSentence(pLine(DataFieldEnum.df81S), pLine(DataFieldEnum.df81V1), pLine(DataFieldEnum.df81V2))
                retVal &= "% to " & GetValueSentence(pLine(DataFieldEnum.df82S), pLine(DataFieldEnum.df82V1), pLine(DataFieldEnum.df82V2))
                If (quad = QuadrantEnum.quadMidHiMid) Or (quad = QuadrantEnum.quadMidLowMid) Or (quad = QuadrantEnum.quadLowHiLow) Or (quad = QuadrantEnum.quadHiLowHi) Then
                    retVal &= "% to " & GetValueSentence(pLine(DataFieldEnum.df81S), pLine(DataFieldEnum.df81V1), pLine(DataFieldEnum.df81V2))
                    'else quadMidHi, quadHiMid, quadMidLow, quadLowMid, quadLowHi
                End If
                retVal &= "%"
            Case Else
                retVal = "<ERROR! UNKNOWN WAVEFORM>"
        End Select

        retVal &= ", Polarity: " & polarityString(pLine(DataFieldEnum.dfPolarity))
        retVal &= ", Duration: " & GetValueSentence(pLine(DataFieldEnum.df321S), pLine(DataFieldEnum.df321V1), pLine(DataFieldEnum.df321V2))
        retVal &= ". Repeats: " & GetValueSentence(pLine(DataFieldEnum.df322S), pLine(DataFieldEnum.df322V1), pLine(DataFieldEnum.df322V2))

        If (pLine(DataFieldEnum.df323S) = 0) And (pLine(DataFieldEnum.df323V1) = 0) Then
            'No delay
            retVal &= ". No Delay."
        Else
            'with delay.
            retVal &= ". Delay: " & GetValueSentence(pLine(DataFieldEnum.df323S), pLine(DataFieldEnum.df323V1), pLine(DataFieldEnum.df323V2))
            retVal &= " mSec"
        End If
        Return retVal
    End Function
    Private Function ProgLineSentenceGoto(ByRef pLine() As Integer) As String
        Dim retVal As String = ""
        retVal = "Goto Line " & pLine(DataFieldEnum.dfGotoTrue)
        Return retVal
    End Function
    Private Function ProgLineSentenceTest(ByRef pLine() As Integer) As String
        Dim retVal As String = "Test: "
        retVal &= "If " & GetValueSentence(pLine(DataFieldEnum.df81S), pLine(DataFieldEnum.df81V1), pLine(DataFieldEnum.df81V2))
        retVal &= " " & testOperationString(pLine(DataFieldEnum.df82V1)) & " "
        'Any modifier?
        If pLine(DataFieldEnum.df82V2) > MathOpEnum.mathOpNone Then
            retVal &= "( "
        End If
        retVal &= GetValueSentence(pLine(DataFieldEnum.df321S), pLine(DataFieldEnum.df321V1), pLine(DataFieldEnum.df321V2)) & " "
        If pLine(DataFieldEnum.df82S) > MathOpEnum.mathOpNone Then
            retVal &= " " & mathFunctionString(pLine(DataFieldEnum.df82V2)) & " "
            retVal &= GetValueSentence(pLine(DataFieldEnum.df322S), pLine(DataFieldEnum.df322V1), pLine(DataFieldEnum.df322V2)) & " )"
        End If

        retVal &= " Then Goto: " & pLine(DataFieldEnum.dfGotoTrue)
        retVal &= ", Else Goto: " & pLine(DataFieldEnum.dfGotoFalse)
        Return retVal
    End Function
    Private Function ProgLineSentenceSet(ByRef pLine() As Integer) As String
        Dim retVal As String = "Set "
        Dim tmpModOp As MathOpEnum = pLine(DataFieldEnum.df82V2)

        retVal &= GetValueSentence(pLine(DataFieldEnum.df81S), pLine(DataFieldEnum.df81V1), pLine(DataFieldEnum.df81V2))
        retVal &= " = "
        If tmpModOp > MathOpEnum.mathOpNone Then
            retVal &= "( "
        End If
        retVal &= GetValueSentence(pLine(DataFieldEnum.df321S), pLine(DataFieldEnum.df321V1), pLine(DataFieldEnum.df321V2))
        If tmpModOp > MathOpEnum.mathOpNone Then
            retVal &= " " & mathFunctionString(tmpModOp) & " "
            retVal &= GetValueSentence(pLine(DataFieldEnum.df322S), pLine(DataFieldEnum.df322V1), pLine(DataFieldEnum.df322V2))
            retVal &= " )"
        End If
        Return retVal
    End Function
    Private Function ProgLineSentenceDelay(ByRef pLine() As Integer) As String
        Dim retVal As String = ""
        retVal &= "Delay: " & GetValueSentence(pLine(DataFieldEnum.df323S), pLine(DataFieldEnum.df323V1), pLine(DataFieldEnum.df323V2))
        retVal &= " mSec"
        Return retVal
    End Function
    Private Function ProgLineSentenceProgControl(ByRef pLine() As Integer) As String
        Dim retVal As String = "Program Control: "
        Dim tmpOp = pLine(DataFieldEnum.df81V1)

        If (tmpOp = OpTypeEnum.opLoadProgramAndPause) Or (tmpOp = OpTypeEnum.opLoadProgramAndRun) Then
            'load prog
            retVal &= "Load Program # " & GetValueSentence(pLine(DataFieldEnum.df321S), pLine(DataFieldEnum.df321V1), pLine(DataFieldEnum.df321V2))
            If pLine(DataFieldEnum.dfChannel) < devRef.allChannelsStringPlusThisChan.Length Then
                retVal &= " on " & devRef.allChannelsStringPlusThisChan(pLine(DataFieldEnum.dfChannel)) & " Channel and "
            Else
                retVal &= " on --OUT OF RANGE: Channel " & pLine(DataFieldEnum.dfChannel) & "-- and "
            End If
            retVal &= If(tmpOp = OpTypeEnum.opLoadProgramAndRun, "run.", "wait.")
        ElseIf pLine(DataFieldEnum.df81V1) >= OpTypeEnum.opStart Then
            'start / stop / pause
            retVal &= programOpTypeString(pLine(DataFieldEnum.df81V1))
            retVal &= " Program on " & devRef.allChannelsStringPlusThisChan(pLine(DataFieldEnum.dfChannel)) & " Channel."
        Else
            'unknown
            retVal &= "ERROR: Unknown operation."
        End If
        Return retVal
    End Function
    Private Function ProgLineSentenceSetModifier(ByRef pLine() As Integer) As String
        Dim retVal As String = ""

        Return retVal
    End Function
    Private Function ProgLineSentenceDisplay(ByRef pLine() As Integer) As String
        Dim retVal As String = ""

        Return retVal
    End Function
    Private Function ProgLineSentenceSendMsg(ByRef pLine() As Integer) As String
        Dim retVal As String = ""

        Return retVal
    End Function

#End Region


    Private Sub UpdateProgVarCountInAllValControls(tmpControlCollection As ControlCollection)
        For Each tmpControl As Control In tmpControlCollection
            If TypeOf tmpControl Is ucValueControl Then
                DirectCast(tmpControl, ucValueControl).ProgVarCount = _progVarCount
            ElseIf tmpControl.HasChildren Then
                UpdateProgVarCountInAllValControls(tmpControl.Controls)
            End If
        Next
    End Sub
    Private Sub UpdateTimerVarCountInAllValControls(tmpControlCollection As ControlCollection)
        For Each tmpControl As Control In tmpControlCollection
            If TypeOf tmpControl Is ucValueControl Then
                DirectCast(tmpControl, ucValueControl).ProgTimerCount = _progTimerCount
            ElseIf tmpControl.HasChildren Then
                UpdateTimerVarCountInAllValControls(tmpControl.Controls)
            End If
        Next
    End Sub
    Private Sub UpdateCurProgramNumberInAllValControls(tmpControlCollection As ControlCollection)
        For Each tmpControl As Control In tmpControlCollection
            If TypeOf tmpControl Is ucValueControl Then
                DirectCast(tmpControl, ucValueControl).ProgNum = _curProgNum
                DirectCast(tmpControl, ucValueControl).SetDeviceRef(devRef)
            ElseIf tmpControl.HasChildren Then
                UpdateCurProgramNumberInAllValControls(tmpControl.Controls)
            End If
        Next
    End Sub



    Private Function CheckLimits(ByVal num As Integer, ByVal min As Integer, ByVal max As Integer) As Boolean
        'Verify whether min <= num >= max
        Return ((num >= min) And (num <= max))
    End Function

    Private Sub cmdUndo_Click(sender As Object, e As EventArgs) Handles cmdUndo.Click
        'Reload the control with the original data
        'Dim tmpLine(DataFieldLen - 1) As Integer
        '_ProgLineOrig.CopyTo(tmpLine, 0)
        Clear()
        'LoadProgLine(tmpLine, _curProgNum, _curLineNum)
        LoadProgLine(_curProgNum, _curLineNum, True)
    End Sub
    Private Sub cmdSave_Click(sender As Object, e As EventArgs) Handles cmdSave.Click
        InitiateSave()
    End Sub
    Public Sub InitiateSave()
        Dim tmpProgLine(DataFieldLen - 1) As Integer
        _ProgLine.CopyTo(tmpProgLine, 0)
        RaiseEvent RequestSave(_curProgNum, _curLineNum, tmpProgLine)
    End Sub
    Public Function UnsavedChanges() As Boolean
        'Compare the _ProgLine and _ProgLineOriginal arrays.  If they are different then return true.
        For n As Integer = 0 To _ProgLine.Length - 1
            If _ProgLine(n) <> _ProgLineOrig(n) Then
                Return True
            End If
        Next
        Return False
    End Function
    Public Sub AcceptChanges()
        'This acknowledges the changes that have been made to the progLine data by the user.
        For n As Integer = 0 To _ProgLine.Length - 1
            _ProgLineOrig(n) = _ProgLine(n)
        Next
    End Sub

    Public Sub Clear()
        'Clear all the data from the control
        For n As Integer = 0 To DataFieldLen - 1
            _ProgLine(n) = 0
            _ProgLineOrig(n) = 0
        Next

        Enabled = False
        'HideControlGroups() -- Covered in the Enabled sub above.

        'Command
        RemoveHandler cboCommand.SelectedIndexChanged, AddressOf cboCommand_SelectedIndexChanged
        cboCommand.SelectedIndex = -1
        cboCommand.Text = ""
        AddHandler cboCommand.SelectedIndexChanged, AddressOf cboCommand_SelectedIndexChanged

        ClearWaveFormGroup()
        ClearSineQuadrantGroup()
        ClearTenMotGroup()
        ClearGotoGroup()
        ClearDelayGroup()

        ClearTestGroup()
        ClearSetGroup()
        ClearModifierGroup()
        ClearProgControlGroup()
        ClearDisplayGroup()
        ClearSendMsgGroup()



    End Sub

    Private Sub ClearWaveFormGroup()
        For Each tmpRdo As RadioButton In grpWaveform.Controls
            If TypeOf tmpRdo Is RadioButton Then
                RemoveHandler tmpRdo.CheckedChanged, AddressOf WaveformShapeRadioCheckChanged
                tmpRdo.Checked = False
                AddHandler tmpRdo.CheckedChanged, AddressOf WaveformShapeRadioCheckChanged
            End If
        Next
    End Sub
    Private Sub ClearSineQuadrantGroup()
        For Each tmpRdo As RadioButton In grpSineQuadrant.Controls
            If TypeOf tmpRdo Is RadioButton Then
                RemoveHandler tmpRdo.CheckedChanged, AddressOf WaveformQuadrantRadioCheckChanged
                tmpRdo.Checked = False
                AddHandler tmpRdo.CheckedChanged, AddressOf WaveformQuadrantRadioCheckChanged
            End If
        Next
    End Sub
    Private Sub ClearTenMotGroup()
        RemoveHandler cboPolarity.SelectedIndexChanged, AddressOf CboPolarity_SelectedIndexChanged
        cboPolarity.SelectedIndex = -1
        cboPolarity.Text = ""
        AddHandler cboPolarity.SelectedIndexChanged, AddressOf CboPolarity_SelectedIndexChanged

        RemoveHandler txtTenMotDutyCycle.TextChanged, AddressOf txtTenMotDutyCycle_TextChanged
        txtTenMotDutyCycle.Text = ""
        lblTenMotDutyCycle.Visible = False
        txtTenMotDutyCycle.Visible = False
        AddHandler txtTenMotDutyCycle.TextChanged, AddressOf txtTenMotDutyCycle_TextChanged

        ucvcTenMotStart.Clear()
        ucvcTenMotEnd.Clear()
        ucvcTenMotV321.Clear()
        ucvcTenMotV322.Clear()
        ucvcTenMotV323.Clear()
    End Sub
    Private Sub ClearGotoGroup()
        RemoveHandler txtGotoLine.TextChanged, AddressOf txtGotoLine_TextChanged
        txtGotoLine.Text = ""
        AddHandler txtGotoLine.TextChanged, AddressOf txtGotoLine_TextChanged
    End Sub
    Private Sub ClearDelayGroup()
        ucvcDelay.Clear()
    End Sub
    Private Sub ClearTestGroup()
        ucvcTestValLeft81.Clear()

        RemoveHandler cboTestType82V1.SelectedIndexChanged, AddressOf cboTestType82V1_SelectedIndexChanged
        cboTestType82V1.SelectedIndex = -1
        cboTestType82V1.Text = ""
        AddHandler cboTestType82V1.SelectedIndexChanged, AddressOf cboTestType82V1_SelectedIndexChanged

        ucvcTestValRight321.Clear()

        RemoveHandler cboTestRightModOperator82V2.SelectedIndexChanged, AddressOf cboTestRightModOperator82V2_SelectedIndexChanged
        cboTestRightModOperator82V2.SelectedIndex = -1
        cboTestRightModOperator82V2.Text = ""
        AddHandler cboTestRightModOperator82V2.SelectedIndexChanged, AddressOf cboTestRightModOperator82V2_SelectedIndexChanged

        ucvcTestValRight322.Clear()

        RemoveHandler txtTestGTT.TextChanged, AddressOf txtTestGTT_TextChanged
        txtTestGTT.Text = ""
        AddHandler txtTestGTT.TextChanged, AddressOf txtTestGTT_TextChanged

        RemoveHandler txtTestGTF.TextChanged, AddressOf txtTestGTF_TextChanged
        txtTestGTF.Text = ""
        AddHandler txtTestGTF.TextChanged, AddressOf txtTestGTF_TextChanged

    End Sub
    Private Sub ClearSetGroup()
        ucvcSetTarget81.Clear()
        ucvcSetSource321.Clear()

        RemoveHandler cboSetModOperator82V2.SelectedIndexChanged, AddressOf cboSetModOperator82V2_SelectedIndexChanged
        cboSetModOperator82V2.SelectedIndex = -1
        cboSetModOperator82V2.Text = ""
        AddHandler cboSetModOperator82V2.SelectedIndexChanged, AddressOf cboSetModOperator82V2_SelectedIndexChanged

        ucvcSetModifierVal322.Clear()

    End Sub
    Private Sub ClearModifierGroup()

    End Sub
    Private Sub ClearProgControlGroup()
        RemoveHandler cboProgControlOperation.SelectedIndexChanged, AddressOf cboProgControlOperation_SelectedIndexChanged
        cboProgControlOperation.SelectedIndex = -1
        cboProgControlOperation.Text = ""
        AddHandler cboProgControlOperation.SelectedIndexChanged, AddressOf cboProgControlOperation_SelectedIndexChanged

        RemoveHandler cboProgControlChannel.SelectedIndexChanged, AddressOf cboProgControlChannel_SelectedIndexChanged
        cboProgControlChannel.SelectedIndex = -1
        cboProgControlChannel.Text = ""
        AddHandler cboProgControlChannel.SelectedIndexChanged, AddressOf cboProgControlChannel_SelectedIndexChanged

        ucvcProgControlProgNum321.Clear()
    End Sub
    Private Sub ClearDisplayGroup()

    End Sub
    Private Sub ClearSendMsgGroup()

    End Sub
    Private Sub HideControlGroups()
        ShowSineQuadrant(False)
        ShowTenMotGroup(False)
        pnlTenMot.Visible = False

        grpGoto.Visible = False
        ShowDelayGroup(False)
        ShowTestGroup(False)
        ShowSetGroup(False)
        ShowModifierGroup(False)
        ShowProgControlGroup(False)
        ShowDisplayGroup(False)
        ShowSendMsgGroup(False)

    End Sub


    'Public Function LoadProgLine(newProgLineData As Integer(), CurProgramNumber As Integer, CurLineNumber As Integer, EditEnabled As Boolean) As Integer ',
    Public Function LoadProgLine(CurProgramNumber As Integer, CurLineNumber As Integer, EditEnabled As Boolean) As Integer ',
        'Optional newChanCount As Integer = -1,
        'Optional newProgVarCount As Integer = -1, Optional newSysVarCount As Integer = -1,
        'Optional newDigInCount As Integer = -1, Optional newDigOutCount As Integer = -1) As Integer
        'Verify the incoming data.  If correct, return 0.  
        'If there's an issue with one of the data fields, return the 1-based index of the data field.

        '_progRef = ProgRef
        If CurProgramNumber < 0 Then Return -1
        If CurLineNumber < 0 Then Return -2


        _curProgNum = CurProgramNumber
        _curLineNum = CurLineNumber

        If _enabled <> EditEnabled Then
            Enabled = EditEnabled
        End If
        If _enabled = False Then
            Return -3  'Progline edit control is not enabled.
        End If
        'If newProgLineData.Length <> DataFieldLen Then Return -1    'Wrong length of data

        'Dim cmd As Integer = newProgLineData(DataFieldEnum.dfCommand)

        ''TODO: this would be a great place to verify the data.
        'If CheckLimits(newProgLineData(DataFieldEnum.dfCommand), 0, commandVals.Length - 1) = False Then Return DataFieldEnum.dfCommand + 1
        'If CheckLimits(newProgLineData(DataFieldEnum.dfChannel), 0, _channelCount - 1) = False Then Return DataFieldEnum.dfChannel + 1
        'If (cmd = CommandEnum.cmdGoTo) Or (cmd = CommandEnum.cmdTest) Then
        '    'If CheckLimits(newProgLineData(DataFieldEnum.dfGotoTrue), 0, _ProgLineCount - 1) = False Then Return DataFieldEnum.dfGotoTrue + 1
        '    'If CheckLimits(newProgLineData(DataFieldEnum.dfGotoFalse), 0, _ProgLineCount - 1) = False Then Return DataFieldEnum.dfGotoFalse + 1
        'End If

        ''Sources:
        'If CheckLimits(newProgLineData(DataFieldEnum.df81S), 0, dataSourceString.Length - 1) = False Then Return DataFieldEnum.df81S + 1
        'If CheckLimits(newProgLineData(DataFieldEnum.df82S), 0, dataSourceString.Length - 1) = False Then Return DataFieldEnum.df82S + 1
        'If CheckLimits(newProgLineData(DataFieldEnum.df321S), 0, dataSourceString.Length - 1) = False Then Return DataFieldEnum.df321S + 1
        'If CheckLimits(newProgLineData(DataFieldEnum.df322S), 0, dataSourceString.Length - 1) = False Then Return DataFieldEnum.df322S + 1
        'If CheckLimits(newProgLineData(DataFieldEnum.df323S), 0, dataSourceString.Length - 1) = False Then Return DataFieldEnum.df323S + 1

        ''Polarity
        'If cmd = CommandEnum.cmdTenMotOutput Then
        '    If CheckLimits(newProgLineData(DataFieldEnum.dfPolarity), 0, polarityString.Length - 1) = False Then Return DataFieldEnum.dfPolarity + 1
        'End If

        ''Incoming data validation complete.
        '_curProgNum = CurProgramNumber
        '_curLineNum = CurLineNumber

        'newProgLineData.CopyTo(_ProgLine, 0)
        'newProgLineData.CopyTo(_ProgLineOrig, 0)

        'If cboCommand.Items.Count <> commandVals.Length - 1 Then
        '    cboCommand.Items.Clear()
        '    cboCommand.Items.AddRange(commandVals)
        'End If

        'If _enabled = True Then
        '    cboCommand.SelectedIndex = _ProgLine(DataFieldEnum.dfCommand)
        '    Return 0
        'Else
        '    Return 1  'Progline edit control is not enabled.
        'End If

        devRef.Prog(CurProgramNumber).progLine(CurLineNumber).CopyTo(_ProgLine, 0) ' newProgLineData.Length <> DataFieldLen Then Return -1    'Wrong length of data
        _ProgLine.CopyTo(_ProgLineOrig, 0)

        Dim cmd As Integer = _ProgLine(DataFieldEnum.dfCommand)

        'TODO: this would be a great place to verify the data.
        Dim checkLimitsErrorNum = 0
        If CheckLimits(_ProgLine(DataFieldEnum.dfCommand), 0, commandVals.Length - 1) = False Then checkLimitsErrorNum = DataFieldEnum.dfCommand + 1
        If CheckLimits(_ProgLine(DataFieldEnum.dfChannel), 0, _channelCount) = False Then checkLimitsErrorNum = DataFieldEnum.dfChannel + 1
        If (cmd = CommandEnum.cmdGoTo) Or (cmd = CommandEnum.cmdTest) Then
            'If CheckLimits(newProgLineData(DataFieldEnum.dfGotoTrue), 0, _ProgLineCount - 1) = False Then checkLimitsErrorNum = DataFieldEnum.dfGotoTrue + 1
            'If CheckLimits(newProgLineData(DataFieldEnum.dfGotoFalse), 0, _ProgLineCount - 1) = False Then checkLimitsErrorNum = DataFieldEnum.dfGotoFalse + 1
        End If

        'Sources:
        If CheckLimits(_ProgLine(DataFieldEnum.df81S), 0, dataSourceString.Length - 1) = False Then checkLimitsErrorNum = DataFieldEnum.df81S + 1
        If CheckLimits(_ProgLine(DataFieldEnum.df82S), 0, dataSourceString.Length - 1) = False Then checkLimitsErrorNum = DataFieldEnum.df82S + 1
        If CheckLimits(_ProgLine(DataFieldEnum.df321S), 0, dataSourceString.Length - 1) = False Then checkLimitsErrorNum = DataFieldEnum.df321S + 1
        If CheckLimits(_ProgLine(DataFieldEnum.df322S), 0, dataSourceString.Length - 1) = False Then checkLimitsErrorNum = DataFieldEnum.df322S + 1
        If CheckLimits(_ProgLine(DataFieldEnum.df323S), 0, dataSourceString.Length - 1) = False Then checkLimitsErrorNum = DataFieldEnum.df323S + 1

        'Polarity
        If cmd = CommandEnum.cmdTenMotOutput Then
            If CheckLimits(_ProgLine(DataFieldEnum.dfPolarity), 0, polarityString.Length - 1) = False Then checkLimitsErrorNum = DataFieldEnum.dfPolarity + 1
        End If

        If checkLimitsErrorNum > 0 Then
            MsgBox("Error detected in data.  Maybe modify the ucProgLineEdit control to highlight these errors.")
        End If

        'Incoming data validation complete.
        '_curProgNum = CurProgramNumber
        '_curLineNum = CurLineNumber

        'newProgLineData.CopyTo(_ProgLine, 0)
        'newProgLineData.CopyTo(_ProgLineOrig, 0)

        If cboCommand.Items.Count <> commandVals.Length - 1 Then
            cboCommand.Items.Clear()
            cboCommand.Items.AddRange(commandVals)
        End If

        If _enabled = True Then
            cboCommand.SelectedIndex = _ProgLine(DataFieldEnum.dfCommand)
            Return 0
        Else
            Return 1  'Progline edit control is not enabled.
        End If

    End Function

    Private Sub WaveformShapeRadioCheckChanged(sender As Object, e As EventArgs)
        Dim newWaveform As WaveformEnum = WaveformEnum.wfNone

        Dim quadrantVisible As Boolean = False
        'Dim V323Visible As Boolean = False
        'Dim V323Label As String = ""
        'Dim V323Units As String = ""


        If rdoWaveformRamp.Checked Then
            waveformString = "Ramp"
            newWaveform = WaveformEnum.wfRamp
        ElseIf rdoWaveformTriangle.Checked Then
            waveformString = "Triangle"
            newWaveform = WaveformEnum.wfTriangle
        ElseIf rdoWaveformSine.Checked Then
            waveformString = "Sine"
            newWaveform = WaveformEnum.wfSine
            quadrantVisible = True
        Else
            waveformString = "Not Selected"
            newWaveform = WaveformEnum.wfNone
        End If

        'If (newWaveform = WaveformEnum.wfTriangle) Then
        '    ucvcTenMotStart.SourceLabel = "Start:"
        '    ucvcTenMotEnd.SourceLabel = "Peak:"
        'Else
        '    ucvcTenMotStart.SourceLabel = "Start:"
        '    ucvcTenMotEnd.SourceLabel = "End:"
        'End If
        'ucvcTenMotStart.MaxVal = 100
        'ucvcTenMotEnd.MaxVal = 100

        _ProgLine(DataFieldEnum.dfGotoTrue) = newWaveform
        grpWaveform.Text = "Waveform - " & waveformString
        ShowSineQuadrant(quadrantVisible)
        ShowTenMotGroup(True)
    End Sub

    Private Sub WaveformQuadrantRadioCheckChanged(sender As Object, e As EventArgs)
        Dim tmpQuadrant As Integer = 0
        Dim tmpStartLabel = "Start:"
        Dim tmpEndLabel = "End:"

        If rdoSineQuadMidHi.Checked = True Then
            tmpQuadrant = QuadrantEnum.quadMidHi
            tmpStartLabel = "Low:"
            tmpEndLabel = "High:"
        ElseIf rdoSineQuadMidHiMid.Checked = True Then
            tmpQuadrant = QuadrantEnum.quadMidHiMid
            tmpStartLabel = "Low:"
            tmpEndLabel = "High:"
        ElseIf rdoSineQuadHiMid.Checked = True Then
            tmpQuadrant = QuadrantEnum.quadHiMid
            tmpStartLabel = "High:"
            tmpEndLabel = "Low:"
        ElseIf rdoSineQuadMidLow.Checked = True Then
            tmpQuadrant = QuadrantEnum.quadMidLow
        ElseIf rdoSineQuadMidLowMid.Checked = True Then
            tmpQuadrant = QuadrantEnum.quadMidLowMid
        ElseIf rdoSineQuadLowMid.Checked = True Then
            tmpQuadrant = QuadrantEnum.quadLowMid
        ElseIf rdoSineQuadLowHi.Checked = True Then
            tmpQuadrant = QuadrantEnum.quadLowHi
        ElseIf rdoSineQuadLowHiLow.Checked = True Then
            tmpQuadrant = QuadrantEnum.quadLowHiLow
            tmpStartLabel = "Low:"
            tmpEndLabel = "High:"
        ElseIf rdoSineQuadHiLowHi.Checked = True Then
            tmpQuadrant = QuadrantEnum.quadHiLowHi
            tmpStartLabel = "High:"
            tmpEndLabel = "Low:"

        ElseIf rdoSineQuadHiLow.Checked = True Then
            tmpQuadrant = QuadrantEnum.quadHiLow
            tmpStartLabel = "High:"
            tmpEndLabel = "Low:"
        ElseIf rdoSineQuadMidHiLowMid.Checked = True Then
            tmpQuadrant = QuadrantEnum.quadMidHiLowMid
            tmpStartLabel = "Start:"
            tmpEndLabel = "High:"
        ElseIf rdoSineQuadMidLowHiMid.Checked = True Then
            tmpQuadrant = QuadrantEnum.quadMidLowHiMid
            tmpStartLabel = "Start:"
            tmpEndLabel = "Low:"
        End If
        ucvcTenMotStart.SourceLabel = tmpStartLabel
        ucvcTenMotEnd.SourceLabel = tmpEndLabel

        _ProgLine(DataFieldEnum.dfGotoFalse) = tmpQuadrant
    End Sub

    Private Sub ShowSineQuadrant(ByVal newVal As Boolean)
        Dim nextTopLocation = grpWaveform.Top + grpWaveform.Height + grpBuffer

        If newVal = True Then
            grpSineQuadrant.Top = nextTopLocation
            grpSineQuadrant.Left = 0
            grpSineQuadrant.Width = pnlTenMot.Width
            grpSineQuadrant.Visible = True

            If CheckLimits(_ProgLine(DataFieldEnum.dfGotoFalse), 0, quadrantString.Length - 1) = False Then
                'The current quadrant value of _progLine is not valid.  Check to see
                'if any quadrant rdo's are currently checked. If so then go with that one.
                Dim tmpVal As Integer = 0
                If rdoSineQuadMidHi.Checked = True Then tmpVal = QuadrantEnum.quadMidHi
                If rdoSineQuadMidHiMid.Checked = True Then tmpVal = QuadrantEnum.quadMidHiMid
                If rdoSineQuadHiMid.Checked = True Then tmpVal = QuadrantEnum.quadHiMid
                If rdoSineQuadMidLow.Checked = True Then tmpVal = QuadrantEnum.quadMidLow
                If rdoSineQuadMidLowMid.Checked = True Then tmpVal = QuadrantEnum.quadMidLowMid
                If rdoSineQuadLowMid.Checked = True Then tmpVal = QuadrantEnum.quadLowMid
                If rdoSineQuadLowHi.Checked = True Then tmpVal = QuadrantEnum.quadLowHi
                If rdoSineQuadLowHiLow.Checked = True Then tmpVal = QuadrantEnum.quadLowHiLow
                If rdoSineQuadHiLowHi.Checked = True Then tmpVal = QuadrantEnum.quadHiLowHi

                If rdoSineQuadHiLow.Checked = True Then tmpVal = QuadrantEnum.quadHiLow
                If rdoSineQuadMidHiLowMid.Checked = True Then tmpVal = QuadrantEnum.quadMidHiLowMid
                If rdoSineQuadMidLowHiMid.Checked = True Then tmpVal = QuadrantEnum.quadMidLowHiMid
                _ProgLine(DataFieldEnum.dfGotoFalse) = tmpVal
            End If

            'check the appropriate quadrant rdo:
            Select Case _ProgLine(DataFieldEnum.dfGotoFalse)
                Case QuadrantEnum.quadMidHi
                    rdoSineQuadMidHi.Checked = True
                Case QuadrantEnum.quadMidHiMid
                    rdoSineQuadMidHiMid.Checked = True
                Case QuadrantEnum.quadHiMid
                    rdoSineQuadHiMid.Checked = True
                Case QuadrantEnum.quadMidLow
                    rdoSineQuadMidLow.Checked = True
                Case QuadrantEnum.quadMidLowMid
                    rdoSineQuadMidLowMid.Checked = True
                Case QuadrantEnum.quadLowMid
                    rdoSineQuadLowMid.Checked = True
                Case QuadrantEnum.quadLowHi
                    rdoSineQuadLowHi.Checked = True
                Case QuadrantEnum.quadLowHiLow
                    rdoSineQuadLowHiLow.Checked = True
                Case QuadrantEnum.quadHiLowHi
                    rdoSineQuadHiLowHi.Checked = True
                Case QuadrantEnum.quadHiLow
                    rdoSineQuadHiLow.Checked = True
                Case QuadrantEnum.quadMidHiLowMid
                    rdoSineQuadMidHiLowMid.Checked = True
                Case QuadrantEnum.quadMidLowHiMid
                    rdoSineQuadMidLowHiMid.Checked = True
            End Select

            'Next group box needs to be repositioned:
            nextTopLocation += grpSineQuadrant.Height + grpBuffer
        Else
            grpSineQuadrant.Visible = False
            grpTenMotVals.Visible = False

        End If
    End Sub

    Private Sub ShowTenMotGroup(ByVal Visible As Boolean)
        If Visible = True Then
            'If ucvcTenMotStart.SourceItems.Count < dataSourceString.Length Then
            InitUcvcControl(ucvcTenMotStart, dataSourceString, 100)
            'End If
            'If ucvcTenMotEnd.SourceItems.Count < dataSourceString.Length Then
            InitUcvcControl(ucvcTenMotEnd, dataSourceString, 100)
            'End If
            'If ucvcTenMotV321.SourceItems.Count < dataSourceString.Length Then
            InitUcvcControl(ucvcTenMotV321, dataSourceString, MAX_32BIT)
            'End If
            'If ucvcTenMotV322.SourceItems.Count < dataSourceString.Length Then
            InitUcvcControl(ucvcTenMotV322, dataSourceString, MAX_32BIT)
            'End If
            'If ucvcTenMotV323.SourceItems.Count < dataSourceString.Length Then
            InitUcvcControl(ucvcTenMotV323, dataSourceString, MAX_32BIT)
            'End If

            'figure out where to place the group box
            If grpSineQuadrant.Visible = True Then
                grpTenMotVals.Top = grpSineQuadrant.Top + grpSineQuadrant.Height + grpBuffer
            Else
                grpTenMotVals.Top = grpWaveform.Top + grpWaveform.Height + grpBuffer
            End If


            'Load data into the tenMot controls:
            ucvcTenMotStart.SourceSelectedIndex = _ProgLine(DataFieldEnum.df81S)
            ucvcTenMotStart.Val1 = _ProgLine(DataFieldEnum.df81V1)
            ucvcTenMotStart.Val2 = _ProgLine(DataFieldEnum.df81V2)
            AddHandler ucvcTenMotStart.Source_Changed, AddressOf ucvcTenMotStart_Source_Changed
            AddHandler ucvcTenMotStart.Val_Changed, AddressOf ucvcTenMotStart_Val_Changed
            AddHandler ucvcTenMotStart.Val2_Changed, AddressOf ucvcTenMotStart_Val2_Changed

            ucvcTenMotEnd.SourceSelectedIndex = _ProgLine(DataFieldEnum.df82S)
            ucvcTenMotEnd.Val1 = _ProgLine(DataFieldEnum.df82V1)
            ucvcTenMotEnd.Val2 = _ProgLine(DataFieldEnum.df82V2)
            AddHandler ucvcTenMotEnd.Source_Changed, AddressOf ucvcTenMotEnd_Source_Changed
            AddHandler ucvcTenMotEnd.Val_Changed, AddressOf ucvcTenMotEnd_Val_Changed
            AddHandler ucvcTenMotEnd.Val2_Changed, AddressOf ucvcTenMotEnd_Val2_Changed

            RemoveHandler cboPolarity.SelectedIndexChanged, AddressOf CboPolarity_SelectedIndexChanged
            If cboPolarity.Items.Count < polarityString.Length Then
                cboPolarity.Items.Clear()
                cboPolarity.Items.AddRange(polarityString)
            End If
            If _ProgLine(DataFieldEnum.dfPolarity) >= polarityString.Length Then
                _ProgLine(DataFieldEnum.dfPolarity) = 0
            End If
            cboPolarity.SelectedIndex = _ProgLine(DataFieldEnum.dfPolarity)
            AddHandler cboPolarity.SelectedIndexChanged, AddressOf CboPolarity_SelectedIndexChanged

            ucvcTenMotV321.SourceLabel = "Duration:"
            ucvcTenMotV321.UnitsLabel = "mSec"
            ucvcTenMotV321.SourceSelectedIndex = _ProgLine(DataFieldEnum.df321S)
            ucvcTenMotV321.Val1 = _ProgLine(DataFieldEnum.df321V1)
            ucvcTenMotV321.Val2 = _ProgLine(DataFieldEnum.df321V2)
            AddHandler ucvcTenMotV321.Source_Changed, AddressOf ucvcTenMotV321_Source_Changed
            AddHandler ucvcTenMotV321.Val_Changed, AddressOf ucvcTenMotV321_Val_Changed
            AddHandler ucvcTenMotV321.Val2_Changed, AddressOf ucvcTenMotV321_Val2_Changed

            ucvcTenMotV322.SourceLabel = "Repeats:"
            ucvcTenMotV322.UnitsLabel = "x"
            ucvcTenMotV322.SourceSelectedIndex = _ProgLine(DataFieldEnum.df322S)
            ucvcTenMotV322.Val1 = _ProgLine(DataFieldEnum.df322V1)
            ucvcTenMotV322.Val2 = _ProgLine(DataFieldEnum.df322V2)
            AddHandler ucvcTenMotV322.Source_Changed, AddressOf ucvcTenMotV322_Source_Changed
            AddHandler ucvcTenMotV322.Val_Changed, AddressOf ucvcTenMotV322_Val_Changed
            AddHandler ucvcTenMotV322.Val2_Changed, AddressOf ucvcTenMotV322_Val2_Changed


            ucvcTenMotV323.SourceLabel = "Delay:"
            ucvcTenMotV323.UnitsLabel = "mSec"
            ucvcTenMotV323.SourceSelectedIndex = _ProgLine(DataFieldEnum.df323S)
            ucvcTenMotV323.Val1 = _ProgLine(DataFieldEnum.df323V1)
            ucvcTenMotV323.Val2 = _ProgLine(DataFieldEnum.df323V2)
            AddHandler ucvcTenMotV323.Source_Changed, AddressOf ucvcTenMotV323_Source_Changed
            AddHandler ucvcTenMotV323.Val_Changed, AddressOf ucvcTenMotV323_Val_Changed
            AddHandler ucvcTenMotV323.Val2_Changed, AddressOf ucvcTenMotV323_Val2_Changed


            RemoveHandler chkTenMotPostDelay.CheckedChanged, AddressOf chkTenMotPostDelay_CheckedChanged
            If (_ProgLine(DataFieldEnum.df323S) = 0) And (_ProgLine(DataFieldEnum.df323V1) = 0) Then
                chkTenMotPostDelay.Checked = False
                ucvcTenMotV323.Visible = False
            Else
                chkTenMotPostDelay.Checked = True
                ucvcTenMotV323.Visible = True
            End If
            AddHandler chkTenMotPostDelay.CheckedChanged, AddressOf chkTenMotPostDelay_CheckedChanged


            If (_ProgLine(DataFieldEnum.dfGotoTrue) = WaveformEnum.wfTriangle) Then
                ucvcTenMotStart.SourceLabel = "Start:"
                ucvcTenMotEnd.SourceLabel = "Peak:"
                lblTenMotDutyCycle.Visible = True
                txtTenMotDutyCycle.Visible = True
                Dim tmpDutyCycle = _ProgLine(DataFieldEnum.dfGotoFalse)
                'If tmpDutyCycle < 1 Then tmpDutyCycle = 50
                'If tmpDutyCycle > 100 Then tmpDutyCycle = 100
                AddHandler txtTenMotDutyCycle.TextChanged, AddressOf txtTenMotDutyCycle_TextChanged
                txtTenMotDutyCycle.Text = tmpDutyCycle
                'RemoveHandler txtTenMotDutyCycle.TextChanged, AddressOf txtTenMotDutyCycle_textchanged

            Else
                ucvcTenMotStart.SourceLabel = "Start:"
                ucvcTenMotEnd.SourceLabel = "End:"
                lblTenMotDutyCycle.Visible = False
                txtTenMotDutyCycle.Visible = False
            End If
            ucvcTenMotStart.MaxVal = 100
            ucvcTenMotEnd.MaxVal = 100

            grpTenMotVals.Visible = True
        Else

            ucvcTenMotV323.Visible = False
            lblTenMotDutyCycle.Visible = False
            txtTenMotDutyCycle.Visible = False

            grpTenMotVals.Visible = False

            RemoveHandler ucvcTenMotStart.Source_Changed, AddressOf ucvcTenMotStart_Source_Changed
            RemoveHandler ucvcTenMotStart.Val_Changed, AddressOf ucvcTenMotStart_Val_Changed
            RemoveHandler ucvcTenMotStart.Val2_Changed, AddressOf ucvcTenMotStart_Val2_Changed

            RemoveHandler ucvcTenMotEnd.Source_Changed, AddressOf ucvcTenMotEnd_Source_Changed
            RemoveHandler ucvcTenMotEnd.Val_Changed, AddressOf ucvcTenMotEnd_Val_Changed
            RemoveHandler ucvcTenMotEnd.Val2_Changed, AddressOf ucvcTenMotEnd_Val2_Changed

            RemoveHandler cboPolarity.SelectedIndexChanged, AddressOf CboPolarity_SelectedIndexChanged

            RemoveHandler ucvcTenMotV321.Source_Changed, AddressOf ucvcTenMotV321_Source_Changed
            RemoveHandler ucvcTenMotV321.Val_Changed, AddressOf ucvcTenMotV321_Val_Changed
            RemoveHandler ucvcTenMotV321.Val2_Changed, AddressOf ucvcTenMotV321_Val2_Changed

            RemoveHandler ucvcTenMotV322.Source_Changed, AddressOf ucvcTenMotV322_Source_Changed
            RemoveHandler ucvcTenMotV322.Val_Changed, AddressOf ucvcTenMotV322_Val_Changed
            RemoveHandler ucvcTenMotV322.Val2_Changed, AddressOf ucvcTenMotV322_Val2_Changed

            RemoveHandler chkTenMotPostDelay.CheckedChanged, AddressOf chkTenMotPostDelay_CheckedChanged

            RemoveHandler ucvcTenMotV323.Source_Changed, AddressOf ucvcTenMotV323_Source_Changed
            RemoveHandler ucvcTenMotV323.Val_Changed, AddressOf ucvcTenMotV323_Val_Changed
            RemoveHandler ucvcTenMotV323.Val2_Changed, AddressOf ucvcTenMotV323_Val2_Changed

            'RemoveHandler rdoWaveformRamp.CheckedChanged, AddressOf WaveformShapeRadioCheckChanged
            'RemoveHandler rdoWaveformTriangle.CheckedChanged, AddressOf WaveformShapeRadioCheckChanged
            'RemoveHandler rdoWaveformSine.CheckedChanged, AddressOf WaveformShapeRadioCheckChanged
            'rdoWaveformRamp.Checked = False
            'rdoWaveformTriangle.Checked = False
            'rdoWaveformSine.Checked = False
            'AddHandler rdoWaveformRamp.CheckedChanged, AddressOf WaveformShapeRadioCheckChanged
            'AddHandler rdoWaveformTriangle.CheckedChanged, AddressOf WaveformShapeRadioCheckChanged
            'AddHandler rdoWaveformSine.CheckedChanged, AddressOf WaveformShapeRadioCheckChanged

        End If
    End Sub
    Private Sub ShowGotoGroup(ByVal Visible As Boolean)
        If Visible = True Then
            AddHandler txtGotoLine.TextChanged, AddressOf txtGotoLine_TextChanged
            txtGotoLine.Text = _ProgLine(DataFieldEnum.dfGotoTrue)
            grpGoto.Visible = True
        Else
            RemoveHandler txtGotoLine.TextChanged, AddressOf txtGotoLine_TextChanged
            grpGoto.Visible = False
        End If
    End Sub
    Private Sub ShowDelayGroup(ByVal Visible As Boolean)
        If Visible = True Then
            If ucvcDelay.SourceItems.Count < dataSourceString.Length Then
                InitUcvcControl(ucvcDelay, dataSourceString, MAX_32BIT)
            End If

            ucvcDelay.SourceLabel = "Duration:"
            ucvcDelay.UnitsLabel = "mSec"
            ucvcDelay.SourceSelectedIndex = _ProgLine(DataFieldEnum.df323S)
            ucvcDelay.Val1 = _ProgLine(DataFieldEnum.df323V1)
            ucvcDelay.Val2 = _ProgLine(DataFieldEnum.df323V2)

            AddHandler ucvcDelay.Source_Changed, AddressOf ucvcDelay_Source_Changed
            AddHandler ucvcDelay.Val_Changed, AddressOf ucvcDelay_Val_Changed
            AddHandler ucvcDelay.Val2_Changed, AddressOf ucvcDelay_Val2_Changed

            grpDelay.Visible = True
        Else
            grpDelay.Visible = False
            RemoveHandler ucvcDelay.Source_Changed, AddressOf ucvcDelay_Source_Changed
            RemoveHandler ucvcDelay.Val_Changed, AddressOf ucvcDelay_Val_Changed
            RemoveHandler ucvcDelay.Val2_Changed, AddressOf ucvcDelay_Val2_Changed
        End If
    End Sub
    Private Sub ShowTestGroup(ByVal Visible As Boolean)
        If Visible = True Then
            If ucvcTestValLeft81.SourceItems.Count < dataSourceString.Length Then
                InitUcvcControl(ucvcTestValLeft81, dataSourceString, MAX_8BIT)
            End If
            ucvcTestValLeft81.SourceLabel = "If "
            ucvcTestValLeft81.UnitsLabel = ""
            ucvcTestValLeft81.SourceSelectedIndex = _ProgLine(DataFieldEnum.df81S)
            ucvcTestValLeft81.Val1 = _ProgLine(DataFieldEnum.df81V1)
            ucvcTestValLeft81.Val2 = _ProgLine(DataFieldEnum.df81V2)
            AddHandler ucvcTestValLeft81.Source_Changed, AddressOf ucvcTestValLeft81_Source_Changed
            AddHandler ucvcTestValLeft81.Val_Changed, AddressOf ucvcTestValLeft81_Val_Changed
            AddHandler ucvcTestValLeft81.Val2_Changed, AddressOf ucvcTestValLeft81_Val2_Changed

            RemoveHandler cboTestType82V1.SelectedIndexChanged, AddressOf cboTestType82V1_SelectedIndexChanged
            If cboTestType82V1.Items.Count < testOperationString.Length Then
                cboTestType82V1.Items.Clear()
                cboTestType82V1.Items.AddRange(testOperationString)
            End If
            If _ProgLine(DataFieldEnum.df82V1) >= testOperationString.Length Then
                _ProgLine(DataFieldEnum.df82V1) = 0
            End If
            cboTestType82V1.SelectedIndex = _ProgLine(DataFieldEnum.df82V1)
            AddHandler cboTestType82V1.SelectedIndexChanged, AddressOf cboTestType82V1_SelectedIndexChanged

            If ucvcTestValRight321.SourceItems.Count < dataSourceString.Length Then
                InitUcvcControl(ucvcTestValRight321, dataSourceString, MAX_32BIT)
            End If
            ucvcTestValRight321.SourceLabel = ""
            ucvcTestValRight321.UnitsLabel = ""
            ucvcTestValRight321.SourceSelectedIndex = _ProgLine(DataFieldEnum.df321S)
            ucvcTestValRight321.Val1 = _ProgLine(DataFieldEnum.df321V1)
            ucvcTestValRight321.Val2 = _ProgLine(DataFieldEnum.df321V2)
            AddHandler ucvcTestValRight321.Source_Changed, AddressOf ucvcTestValRight321_Source_Changed
            AddHandler ucvcTestValRight321.Val_Changed, AddressOf ucvcTestValRight321_Val_Changed
            AddHandler ucvcTestValRight321.Val2_Changed, AddressOf ucvcTestValRight321_Val2_Changed


            RemoveHandler cboTestRightModOperator82V2.SelectedIndexChanged, AddressOf cboTestRightModOperator82V2_SelectedIndexChanged
            If cboTestRightModOperator82V2.Items.Count < mathFunctionString.Length Then
                cboTestRightModOperator82V2.Items.Clear()
                cboTestRightModOperator82V2.Items.AddRange(mathFunctionString)
            End If
            If _ProgLine(DataFieldEnum.df82V2) >= testOperationString.Length Then
                _ProgLine(DataFieldEnum.df82V2) = 0
            End If
            cboTestRightModOperator82V2.SelectedIndex = _ProgLine(DataFieldEnum.df82V2)
            If cboTestRightModOperator82V2.SelectedIndex = 0 Then
                ucvcTestValRight322.Visible = False
            Else
                ucvcTestValRight322.Visible = True
            End If
            AddHandler cboTestRightModOperator82V2.SelectedIndexChanged, AddressOf cboTestRightModOperator82V2_SelectedIndexChanged

            If ucvcTestValRight322.SourceItems.Count < dataSourceString.Length Then
                InitUcvcControl(ucvcTestValRight322, dataSourceString, MAX_32BIT)
            End If
            ucvcTestValRight322.SourceLabel = mathFunctionString(cboTestRightModOperator82V2.SelectedIndex) + " "
            ucvcTestValRight322.UnitsLabel = ""
            ucvcTestValRight322.SourceSelectedIndex = _ProgLine(DataFieldEnum.df322S)
            ucvcTestValRight322.Val1 = _ProgLine(DataFieldEnum.df322V1)
            ucvcTestValRight322.Val2 = _ProgLine(DataFieldEnum.df322V2)
            AddHandler ucvcTestValRight322.Source_Changed, AddressOf ucvcTestValRight322_Source_Changed
            AddHandler ucvcTestValRight322.Val_Changed, AddressOf ucvcTestValRight322_Val_Changed
            AddHandler ucvcTestValRight322.Val2_Changed, AddressOf ucvcTestValRight322_Val2_Changed

            RemoveHandler txtTestGTT.TextChanged, AddressOf txtTestGTT_TextChanged
            txtTestGTT.Text = _ProgLine(DataFieldEnum.dfGotoTrue)
            HighlightGotoTextbox(txtTestGTT)
            AddHandler txtTestGTT.TextChanged, AddressOf txtTestGTT_TextChanged

            RemoveHandler txtTestGTF.TextChanged, AddressOf txtTestGTF_TextChanged
            txtTestGTF.Text = _ProgLine(DataFieldEnum.dfGotoFalse)
            HighlightGotoTextbox(txtTestGTF)
            AddHandler txtTestGTF.TextChanged, AddressOf txtTestGTF_TextChanged

            grpTest.Visible = True
        Else
            ucvcTestValRight322.Visible = False
            grpTest.Visible = False

            RemoveHandler ucvcTestValLeft81.Source_Changed, AddressOf ucvcTestValLeft81_Source_Changed
            RemoveHandler ucvcTestValLeft81.Val_Changed, AddressOf ucvcTestValLeft81_Val_Changed
            RemoveHandler ucvcTestValLeft81.Val2_Changed, AddressOf ucvcTestValLeft81_Val2_Changed

            RemoveHandler ucvcTestValRight321.Source_Changed, AddressOf ucvcTestValRight321_Source_Changed
            RemoveHandler ucvcTestValRight321.Val_Changed, AddressOf ucvcTestValRight321_Val_Changed
            RemoveHandler ucvcTestValRight321.Val2_Changed, AddressOf ucvcTestValRight321_Val2_Changed

            RemoveHandler ucvcTestValRight322.Source_Changed, AddressOf ucvcTestValRight322_Source_Changed
            RemoveHandler ucvcTestValRight322.Val_Changed, AddressOf ucvcTestValRight322_Val_Changed
            RemoveHandler ucvcTestValRight322.Val2_Changed, AddressOf ucvcTestValRight322_Val2_Changed

            RemoveHandler txtTestGTT.TextChanged, AddressOf txtTestGTT_TextChanged
            RemoveHandler txtTestGTF.TextChanged, AddressOf txtTestGTF_TextChanged
        End If
    End Sub
    Private Sub ShowSetGroup(ByVal Visible As Boolean)
        If Visible = True Then
            If ucvcSetTarget81.SourceItems.Count < dataSourceString.Length Then
                InitUcvcControl(ucvcSetTarget81, dataSourceString, MAX_8BIT)
            End If
            ucvcSetTarget81.SourceLabel = "Set "
            ucvcSetTarget81.UnitsLabel = ""
            ucvcSetTarget81.SourceSelectedIndex = _ProgLine(DataFieldEnum.df81S)
            ucvcSetTarget81.Val1 = _ProgLine(DataFieldEnum.df81V1)
            ucvcSetTarget81.Val2 = _ProgLine(DataFieldEnum.df81V2)
            AddHandler ucvcSetTarget81.Source_Changed, AddressOf ucvcSetTarget81_Source_Changed
            AddHandler ucvcSetTarget81.Val_Changed, AddressOf ucvcSetTarget81_Val1_Changed
            AddHandler ucvcSetTarget81.Val2_Changed, AddressOf ucvcSetTarget81_Val2_Changed

            If ucvcSetSource321.SourceItems.Count < dataSourceString.Length Then
                InitUcvcControl(ucvcSetSource321, dataSourceString, MAX_32BIT)
            End If
            ucvcSetSource321.SourceLabel = "= "
            ucvcSetSource321.UnitsLabel = ""
            ucvcSetSource321.SourceSelectedIndex = _ProgLine(DataFieldEnum.df321S)
            ucvcSetSource321.Val1 = _ProgLine(DataFieldEnum.df321V1)
            ucvcSetSource321.Val2 = _ProgLine(DataFieldEnum.df321V2)
            AddHandler ucvcSetSource321.Source_Changed, AddressOf ucvcSetSource321_Source_Changed
            AddHandler ucvcSetSource321.Val_Changed, AddressOf ucvcSetSource321_Val1_Changed
            AddHandler ucvcSetSource321.Val2_Changed, AddressOf ucvcSetSource321_Val2_Changed

            RemoveHandler cboSetModOperator82V2.SelectedIndexChanged, AddressOf cboSetModOperator82V2_SelectedIndexChanged
            If cboSetModOperator82V2.Items.Count < mathFunctionString.Length Then
                cboSetModOperator82V2.Items.Clear()
                cboSetModOperator82V2.Items.AddRange(mathFunctionString)
            End If
            If _ProgLine(DataFieldEnum.df82V2) >= mathFunctionString.Length Then
                _ProgLine(DataFieldEnum.df82V2) = 0
            Else
                cboSetModOperator82V2.SelectedIndex = _ProgLine(DataFieldEnum.df82V2)
            End If
            If cboSetModOperator82V2.SelectedIndex = 0 Then
                ucvcSetModifierVal322.Visible = False
            Else
                ucvcSetModifierVal322.Visible = True
            End If
            AddHandler cboSetModOperator82V2.SelectedIndexChanged, AddressOf cboSetModOperator82V2_SelectedIndexChanged

            If ucvcSetModifierVal322.SourceItems.Count < dataSourceString.Length Then
                InitUcvcControl(ucvcSetModifierVal322, dataSourceString, MAX_32BIT)
            End If
            ucvcSetModifierVal322.SourceLabel = mathFunctionString(cboSetModOperator82V2.SelectedIndex) + " "
            ucvcSetModifierVal322.UnitsLabel = ""
            ucvcSetModifierVal322.SourceSelectedIndex = _ProgLine(DataFieldEnum.df322S)
            ucvcSetModifierVal322.Val1 = _ProgLine(DataFieldEnum.df322V1)
            ucvcSetModifierVal322.Val2 = _ProgLine(DataFieldEnum.df322V2)
            AddHandler ucvcSetModifierVal322.Source_Changed, AddressOf ucvcSetModifierVal322_Source_Changed
            AddHandler ucvcSetModifierVal322.Val_Changed, AddressOf ucvcSetModifierVal322_Val_Changed
            AddHandler ucvcSetModifierVal322.Val2_Changed, AddressOf ucvcSetModifierVal322_Val2_Changed

            grpSet.Visible = True
        Else
            grpSet.Visible = False

            RemoveHandler ucvcSetTarget81.Source_Changed, AddressOf ucvcSetTarget81_Source_Changed
            RemoveHandler ucvcSetTarget81.Val_Changed, AddressOf ucvcSetTarget81_Val1_Changed
            RemoveHandler ucvcSetTarget81.Val2_Changed, AddressOf ucvcSetTarget81_Val2_Changed

            RemoveHandler ucvcSetSource321.Source_Changed, AddressOf ucvcSetSource321_Source_Changed
            RemoveHandler ucvcSetSource321.Val_Changed, AddressOf ucvcSetSource321_Val1_Changed
            RemoveHandler ucvcSetSource321.Val2_Changed, AddressOf ucvcSetSource321_Val2_Changed

            RemoveHandler cboSetModOperator82V2.SelectedIndexChanged, AddressOf cboSetModOperator82V2_SelectedIndexChanged

            RemoveHandler ucvcSetModifierVal322.Source_Changed, AddressOf ucvcSetModifierVal322_Source_Changed
            RemoveHandler ucvcSetModifierVal322.Val_Changed, AddressOf ucvcSetModifierVal322_Val_Changed
            RemoveHandler ucvcSetModifierVal322.Val2_Changed, AddressOf ucvcSetModifierVal322_Val2_Changed
        End If
    End Sub
    Private Sub ShowModifierGroup(ByVal Visible As Boolean)
        If Visible = True Then

            grpModifier.Visible = True
        Else
            grpModifier.Visible = False
        End If
    End Sub
    Private Sub ShowProgControlGroup(ByVal Visible As Boolean)
        If Visible = True Then
            RemoveHandler cboProgControlOperation.SelectedIndexChanged, AddressOf cboProgControlOperation_SelectedIndexChanged
            If cboProgControlOperation.Items.Count < programOpTypeString.Length Then
                cboProgControlOperation.Items.Clear()
                cboProgControlOperation.Items.AddRange(programOpTypeString)
            End If
            If _ProgLine(DataFieldEnum.df81V1) >= programOpTypeString.Length Then
                _ProgLine(DataFieldEnum.df81V1) = 0
            End If
            Dim tmpVal As Integer = _ProgLine(DataFieldEnum.df81V1)
            cboProgControlOperation.SelectedIndex = tmpVal
            If tmpVal > 0 Then
                lblProgControlChannel.Visible = True
                cboProgControlChannel.Visible = True
                If (tmpVal = OpTypeEnum.opLoadProgramAndPause) Or (tmpVal = OpTypeEnum.opLoadProgramAndRun) Then
                    ucvcProgControlProgNum321.Visible = True
                Else
                    ucvcProgControlProgNum321.Visible = False
                End If
            Else
                lblProgControlChannel.Visible = False
                cboProgControlChannel.Visible = False
                ucvcProgControlProgNum321.Visible = False
            End If
            AddHandler cboProgControlOperation.SelectedIndexChanged, AddressOf cboProgControlOperation_SelectedIndexChanged

            RemoveHandler cboProgControlChannel.SelectedIndexChanged, AddressOf cboProgControlChannel_SelectedIndexChanged
            If cboProgControlChannel.Items.Count < devRef.allChannelsStringPlusThisChan.Length Then
                cboProgControlChannel.Items.Clear()
                cboProgControlChannel.Items.AddRange(devRef.allChannelsStringPlusThisChan)
            End If
            If _ProgLine(DataFieldEnum.dfChannel) >= devRef.allChannelsStringPlusThisChan.Length Then
                _ProgLine(DataFieldEnum.dfChannel) = 0
            End If
            cboProgControlChannel.SelectedIndex = _ProgLine(DataFieldEnum.dfChannel)
            AddHandler cboProgControlChannel.SelectedIndexChanged, AddressOf cboProgControlChannel_SelectedIndexChanged

            If ucvcProgControlProgNum321.SourceItems.Count < dataSourceString.Length Then
                InitUcvcControl(ucvcProgControlProgNum321, dataSourceString, MAX_16BIT)
            End If
            ucvcProgControlProgNum321.SourceLabel = "Prog #:"
            ucvcProgControlProgNum321.UnitsLabel = ""
            ucvcProgControlProgNum321.SourceSelectedIndex = _ProgLine(DataFieldEnum.df321S)
            ucvcProgControlProgNum321.Val1 = _ProgLine(DataFieldEnum.df321V1)
            ucvcProgControlProgNum321.Val2 = _ProgLine(DataFieldEnum.df321V2)
            AddHandler ucvcProgControlProgNum321.Source_Changed, AddressOf ucvcProgControlProgNum321_Source_Changed
            AddHandler ucvcProgControlProgNum321.Val_Changed, AddressOf ucvcProgControlProgNum321_Val_Changed
            AddHandler ucvcProgControlProgNum321.Val2_Changed, AddressOf ucvcProgControlProgNum321_Val2_Changed

            grpProgControl.Visible = True
        Else
            grpProgControl.Visible = False

            RemoveHandler cboProgControlOperation.SelectedIndexChanged, AddressOf cboProgControlOperation_SelectedIndexChanged

            RemoveHandler cboProgControlChannel.SelectedIndexChanged, AddressOf cboProgControlChannel_SelectedIndexChanged

            RemoveHandler ucvcProgControlProgNum321.Source_Changed, AddressOf ucvcProgControlProgNum321_Source_Changed
            RemoveHandler ucvcProgControlProgNum321.Val_Changed, AddressOf ucvcProgControlProgNum321_Val_Changed
            RemoveHandler ucvcProgControlProgNum321.Val2_Changed, AddressOf ucvcProgControlProgNum321_Val2_Changed
        End If
    End Sub
    Private Sub ShowDisplayGroup(ByVal Visible As Boolean)
        If Visible = True Then

            grpDisplay.Visible = True
        Else
            grpDisplay.Visible = False
        End If
    End Sub
    Private Sub ShowSendMsgGroup(ByVal Visible As Boolean)
        If Visible = True Then

            grpSendMsg.Visible = True
        Else
            grpSendMsg.Visible = False
        End If
    End Sub


    Private Sub InitUcvcControl(ByRef ctrl As ucValueControl, ByRef itemArray() As String, ByVal maxVal As Integer)
        ctrl.Clear()
        ctrl.SetDeviceRef(devRef)
        ctrl.ProgNum = _curProgNum
        ctrl.SourceItems.Clear()
        ctrl.SourceItems.AddRange(itemArray)
        ctrl.DigInCount = _digInCount
        ctrl.DigOutCount = _digOutCount
        'ctrl.SysVarCount = _sysVarCount
        ctrl.ProgVarCount = _progVarCount
        ctrl.MaxVal = maxVal
    End Sub


    Private Function RemoveNonNumeric(ByVal orig As String) As Integer
        'Accept a string input and start removing chars from the right until what's left is numeric.
        'Return the numeric remains.
        While orig.Length > 0
            If IsNumeric(orig) Then
                Return CInt(orig)
            Else
                orig = orig.Remove(orig.Length - 1)
            End If
        End While
        Return 0
    End Function
    Private Function GetGotoTextBoxValue(ByRef ctrl As TextBox) As Integer
        'This is called by the textbox_TextChanged event handler for all GTT and GTF text boxes.
        '1. If the box is empty, interpret line # as 0
        '2. If there's a non-numeric char in the textbox, remove it.
        '3. If the line number specified in the box is higher than the number of line nums in the program, highlight the box.

        Dim tmpVal As Integer = 0
        Dim highlight As Boolean = False

        If ctrl.Text = "" Then
            tmpVal = 0
        ElseIf IsNumeric(ctrl.Text) = False Then
            tmpVal = RemoveNonNumeric(ctrl.Text)
            ctrl.Text = tmpVal
            ctrl.SelectionStart = ctrl.Text.Length
            Beep()
        Else
            tmpVal = CInt(ctrl.Text)
        End If

        HighlightGotoTextbox(ctrl)
        Return tmpVal
    End Function
    Private Sub HighlightGotoTextbox(ByRef ctrl As TextBox)
        'If the line number specified in the box is higher than the number of line nums in the program, highlight the box.

        Dim tmpVal As Integer = -1
        If ctrl.Text <> "" Then
            If IsNumeric(ctrl.Text) Then
                tmpVal = CInt(ctrl.Text)
            End If
        End If

        'If (tmpVal < 0) Or (tmpVal >= _ProgLineCount) Then
        If (tmpVal < 0) Or (tmpVal >= devRef.Prog(_curProgNum).progLine.Count) Then
            If ctrl.BackColor <> errorColor Then
                ctrl.BackColor = errorColor
            End If
        Else
            If ctrl.BackColor <> normalControlColor Then
                ctrl.BackColor = normalControlColor
            End If
        End If
    End Sub



    Private Sub ucProgLineEdit_Load(sender As Object, e As EventArgs) Handles Me.Load
        Enabled = False
        'HideControlGroups()

        AddHandler cboCommand.SelectedIndexChanged, AddressOf cboCommand_SelectedIndexChanged

        AddHandler rdoWaveformRamp.CheckedChanged, AddressOf WaveformShapeRadioCheckChanged
        AddHandler rdoWaveformTriangle.CheckedChanged, AddressOf WaveformShapeRadioCheckChanged
        AddHandler rdoWaveformSine.CheckedChanged, AddressOf WaveformShapeRadioCheckChanged

        AddHandler rdoSineQuadMidHi.CheckedChanged, AddressOf WaveformQuadrantRadioCheckChanged
        AddHandler rdoSineQuadHiMid.CheckedChanged, AddressOf WaveformQuadrantRadioCheckChanged
        AddHandler rdoSineQuadMidHiMid.CheckedChanged, AddressOf WaveformQuadrantRadioCheckChanged
        AddHandler rdoSineQuadMidLow.CheckedChanged, AddressOf WaveformQuadrantRadioCheckChanged
        AddHandler rdoSineQuadMidLowMid.CheckedChanged, AddressOf WaveformQuadrantRadioCheckChanged
        AddHandler rdoSineQuadLowMid.CheckedChanged, AddressOf WaveformQuadrantRadioCheckChanged
        AddHandler rdoSineQuadLowHi.CheckedChanged, AddressOf WaveformQuadrantRadioCheckChanged
        AddHandler rdoSineQuadLowHiLow.CheckedChanged, AddressOf WaveformQuadrantRadioCheckChanged
        AddHandler rdoSineQuadHiLowHi.CheckedChanged, AddressOf WaveformQuadrantRadioCheckChanged

        AddHandler rdoSineQuadHiLow.CheckedChanged, AddressOf WaveformQuadrantRadioCheckChanged
        AddHandler rdoSineQuadMidHiLowMid.CheckedChanged, AddressOf WaveformQuadrantRadioCheckChanged
        AddHandler rdoSineQuadMidLowHiMid.CheckedChanged, AddressOf WaveformQuadrantRadioCheckChanged

    End Sub

    Private Sub cboCommand_SelectedIndexChanged(sender As Object, e As EventArgs)
        'If cboCommand.SelectedIndex = _ProgLine(DataFieldEnum.dfCommand) Then Exit Sub

        HideControlGroups()

        If cboCommand.SelectedIndex = CommandEnum.cmdTenMotOutput Then
            pnlTenMot.Top = TopPanelY
            pnlTenMot.Visible = True
            grpWaveform.Visible = True

            'Set the waveform rdo button
            Select Case _ProgLine(DataFieldEnum.dfGotoTrue) 'Waveform field for tenMot output
                Case WaveformEnum.wfRamp
                    If rdoWaveformRamp.Checked = False Then
                        rdoWaveformRamp.Checked = True
                    Else
                        WaveformShapeRadioCheckChanged(Nothing, Nothing)
                    End If
                Case WaveformEnum.wfTriangle
                    If rdoWaveformTriangle.Checked = False Then
                        rdoWaveformTriangle.Checked = True
                    Else
                        WaveformShapeRadioCheckChanged(Nothing, Nothing)
                    End If
                Case WaveformEnum.wfSine
                    If rdoWaveformSine.Checked = False Then
                        rdoWaveformSine.Checked = True
                    Else
                        WaveformShapeRadioCheckChanged(Nothing, Nothing)
                    End If
                Case Else
                    _ProgLine(DataFieldEnum.dfGotoTrue) = 0 'Waveform field for tenMot output
                    ShowTenMotGroup(False)
            End Select

        ElseIf cboCommand.SelectedIndex = CommandEnum.cmdGoTo Then
            grpGoto.Top = TopPanelY
            ShowGotoGroup(True)
        ElseIf cboCommand.SelectedIndex = CommandEnum.cmdDelay Then
            grpDelay.Top = TopPanelY
            ShowDelayGroup(True)
        ElseIf cboCommand.SelectedIndex = CommandEnum.cmdTest Then
            grpTest.Top = TopPanelY
            ShowTestGroup(True)
        ElseIf cboCommand.SelectedIndex = CommandEnum.cmdSet Then
            grpSet.Top = TopPanelY
            ShowSetGroup(True)
            'ElseIf cboCommand.SelectedIndex = CommandEnum.cmdSetModifier Then
            '    grpModifier.Top = TopPanelY
            '    ShowModifierGroup(True)
        ElseIf cboCommand.SelectedIndex = CommandEnum.cmdProgControl Then
            grpProgControl.Top = TopPanelY
            ShowProgControlGroup(True)
        ElseIf cboCommand.SelectedIndex = CommandEnum.cmdDisplay Then
            grpDisplay.Top = TopPanelY
            ShowDisplayGroup(True)
        ElseIf cboCommand.SelectedIndex = CommandEnum.cmdSendMsg Then
            grpSendMsg.Top = TopPanelY
            ShowSendMsgGroup(True)
        End If

        _ProgLine(DataFieldEnum.dfCommand) = cboCommand.SelectedIndex
    End Sub


#Region "TenMot_Events"

    Private Sub ucvcTenMotStart_Source_Changed(sender As Object, newSourceIndex As Integer)
        _ProgLine(DataFieldEnum.df81S) = newSourceIndex
    End Sub
    Private Sub ucvcTenMotStart_Val_Changed(sender As Object, newVal As Integer)
        _ProgLine(DataFieldEnum.df81V1) = newVal
    End Sub
    Private Sub ucvcTenMotStart_Val2_Changed(sender As Object, newVal As Integer)
        _ProgLine(DataFieldEnum.df81V2) = newVal
    End Sub


    Private Sub ucvcTenMotEnd_Source_Changed(sender As Object, newSourceIndex As Integer)
        _ProgLine(DataFieldEnum.df82S) = newSourceIndex
    End Sub
    Private Sub ucvcTenMotEnd_Val_Changed(sender As Object, newVal As Integer)
        _ProgLine(DataFieldEnum.df82V1) = newVal
    End Sub
    Private Sub ucvcTenMotEnd_Val2_Changed(sender As Object, newVal As Integer)
        _ProgLine(DataFieldEnum.df82V2) = newVal
    End Sub

    Private Sub CboPolarity_SelectedIndexChanged(sender As Object, e As EventArgs)
        _ProgLine(DataFieldEnum.dfPolarity) = cboPolarity.SelectedIndex
    End Sub

    Private Sub txtTenMotDutyCycle_TextChanged(sender As Object, e As EventArgs)
        Dim tmpVal As Integer = 50

        If txtTenMotDutyCycle.Text = "" Then
            tmpVal = 50
        ElseIf IsNumeric(txtTenMotDutyCycle.Text) = False Then
            tmpVal = RemoveNonNumeric(txtTenMotDutyCycle.Text)
            If tmpVal < 1 Then tmpVal = 50
            txtTenMotDutyCycle.Text = tmpVal
            txtTenMotDutyCycle.SelectionStart = txtTenMotDutyCycle.Text.Length
            Beep()
        ElseIf CInt(txtTenMotDutyCycle.Text) < 1 Then
            tmpVal = 50
            txtTenMotDutyCycle.Text = tmpVal
            txtTenMotDutyCycle.SelectionStart = txtTenMotDutyCycle.Text.Length
            'Beep()
        ElseIf CInt(txtTenMotDutyCycle.Text) > 100 Then
            tmpVal = 100
            txtTenMotDutyCycle.Text = tmpVal
            txtTenMotDutyCycle.SelectionStart = txtTenMotDutyCycle.Text.Length
            Beep()
        Else
            tmpVal = CInt(txtTenMotDutyCycle.Text)
        End If

        _ProgLine(DataFieldEnum.dfGotoFalse) = tmpVal
    End Sub


    Private Sub ucvcTenMotV321_Source_Changed(sender As Object, newSourceIndex As Integer)
        _ProgLine(DataFieldEnum.df321S) = newSourceIndex
    End Sub
    Private Sub ucvcTenMotV321_Val_Changed(sender As Object, newVal As Integer)
        _ProgLine(DataFieldEnum.df321V1) = newVal
    End Sub
    Private Sub ucvcTenMotV321_Val2_Changed(sender As Object, newVal As Integer)
        _ProgLine(DataFieldEnum.df321V2) = newVal
    End Sub

    Private Sub ucvcTenMotV322_Source_Changed(sender As Object, newSourceIndex As Integer)
        _ProgLine(DataFieldEnum.df322S) = newSourceIndex
    End Sub
    Private Sub ucvcTenMotV322_Val_Changed(sender As Object, newVal As Integer)
        _ProgLine(DataFieldEnum.df322V1) = newVal
    End Sub
    Private Sub ucvcTenMotV322_Val2_Changed(sender As Object, newVal As Integer)
        _ProgLine(DataFieldEnum.df322V2) = newVal
    End Sub

    Private Sub chkTenMotPostDelay_CheckedChanged(sender As Object, e As EventArgs)
        Dim tmpVal = If(chkTenMotPostDelay.Checked = True, 1, 0)
        If tmpVal = 1 Then
            ucvcTenMotV323.Visible = True
        Else
            _ProgLine(DataFieldEnum.df323S) = 0
            _ProgLine(DataFieldEnum.df323V1) = 0
            _ProgLine(DataFieldEnum.df323V2) = 0
            ucvcTenMotV323.Visible = False
        End If
        ucvcTenMotV323.SourceSelectedIndex = _ProgLine(DataFieldEnum.df323S)
        ucvcTenMotV323.Val1 = _ProgLine(DataFieldEnum.df323V1)
        ucvcTenMotV323.Val2 = _ProgLine(DataFieldEnum.df323V2)
    End Sub

    Private Sub ucvcTenMotV323_Source_Changed(sender As Object, newSourceIndex As Integer)
        _ProgLine(DataFieldEnum.df323S) = newSourceIndex
    End Sub
    Private Sub ucvcTenMotV323_Val_Changed(sender As Object, newVal As Integer)
        _ProgLine(DataFieldEnum.df323V1) = newVal
    End Sub
    Private Sub ucvcTenMotV323_Val2_Changed(sender As Object, newVal As Integer)
        _ProgLine(DataFieldEnum.df323V2) = newVal
    End Sub

#End Region
#Region "Delay_Events"

    Private Sub ucvcDelay_Source_Changed(sender As Object, newSourceIndex As Integer)
        _ProgLine(DataFieldEnum.df323S) = newSourceIndex
    End Sub
    Private Sub ucvcDelay_Val_Changed(sender As Object, newVal As Integer)
        _ProgLine(DataFieldEnum.df323V1) = newVal
    End Sub
    Private Sub ucvcDelay_Val2_Changed(sender As Object, newVal As Integer)
        _ProgLine(DataFieldEnum.df323V2) = newVal
    End Sub

#End Region
#Region "Goto_Events"
    Private Sub txtGotoLine_TextChanged(sender As Object, e As EventArgs)
        Dim tmpVal As Integer = GetGotoTextBoxValue(txtGotoLine)
        _ProgLine(DataFieldEnum.dfGotoTrue) = tmpVal
    End Sub
#End Region
#Region "Test_Events"

    Private Sub ucvcTestValLeft81_Source_Changed(sender As Object, newSourceIndex As Integer)
        _ProgLine(DataFieldEnum.df81S) = newSourceIndex
    End Sub
    Private Sub ucvcTestValLeft81_Val_Changed(sender As Object, newVal As Integer)
        _ProgLine(DataFieldEnum.df81V1) = newVal
    End Sub
    Private Sub ucvcTestValLeft81_Val2_Changed(sender As Object, newVal As Integer)
        _ProgLine(DataFieldEnum.df81V2) = newVal
    End Sub

    Private Sub cboTestType82V1_SelectedIndexChanged(sender As Object, e As EventArgs)
        _ProgLine(DataFieldEnum.df82V1) = cboTestType82V1.SelectedIndex
    End Sub

    Private Sub ucvcTestValRight321_Source_Changed(sender As Object, newSourceIndex As Integer)
        _ProgLine(DataFieldEnum.df321S) = newSourceIndex
    End Sub
    Private Sub ucvcTestValRight321_Val_Changed(sender As Object, newVal As Integer)
        _ProgLine(DataFieldEnum.df321V1) = newVal
    End Sub
    Private Sub ucvcTestValRight321_Val2_Changed(sender As Object, newVal As Integer)
        _ProgLine(DataFieldEnum.df321V2) = newVal
    End Sub

    Private Sub cboTestRightModOperator82V2_SelectedIndexChanged(sender As Object, e As EventArgs)
        Dim tmpVal = cboTestRightModOperator82V2.SelectedIndex
        _ProgLine(DataFieldEnum.df82V2) = tmpVal
        ucvcTestValRight322.SourceLabel = mathFunctionString(cboTestRightModOperator82V2.SelectedIndex) + " "
        If tmpVal = 0 Then 'Public mathFunctionString As String() = {" ", "+", "-", "x", "/", "mod"}
            ucvcTestValRight322.Visible = False
        Else
            ucvcTestValRight322.Visible = True
        End If
    End Sub

    Private Sub ucvcTestValRight322_Source_Changed(sender As Object, newSourceIndex As Integer)
        _ProgLine(DataFieldEnum.df322S) = newSourceIndex
    End Sub
    Private Sub ucvcTestValRight322_Val_Changed(sender As Object, newVal As Integer)
        _ProgLine(DataFieldEnum.df322V1) = newVal
    End Sub
    Private Sub ucvcTestValRight322_Val2_Changed(sender As Object, newVal As Integer)
        _ProgLine(DataFieldEnum.df322V2) = newVal
    End Sub

    Private Sub txtTestGTT_TextChanged(sender As Object, e As EventArgs) Handles txtTestGTT.TextChanged
        Dim tmpVal As Integer = GetGotoTextBoxValue(txtTestGTT)
        _ProgLine(DataFieldEnum.dfGotoTrue) = tmpVal
    End Sub
    Private Sub txtTestGTF_TextChanged(sender As Object, e As EventArgs) Handles txtTestGTF.TextChanged
        Dim tmpVal As Integer = GetGotoTextBoxValue(txtTestGTF)
        _ProgLine(DataFieldEnum.dfGotoFalse) = tmpVal
    End Sub

#End Region
#Region "Set_Events"

    Private Sub ucvcSetTarget81_Source_Changed(sender As Object, newSourceIndex As Integer)
        _ProgLine(DataFieldEnum.df81S) = newSourceIndex
    End Sub
    Private Sub ucvcSetTarget81_Val1_Changed(sender As Object, newVal As Integer)
        _ProgLine(DataFieldEnum.df81V1) = newVal
    End Sub
    Private Sub ucvcSetTarget81_Val2_Changed(sender As Object, newVal As Integer)
        _ProgLine(DataFieldEnum.df81V2) = newVal
    End Sub

    Private Sub ucvcSetSource321_Source_Changed(sender As Object, newSourceIndex As Integer)
        _ProgLine(DataFieldEnum.df321S) = newSourceIndex
    End Sub
    Private Sub ucvcSetSource321_Val1_Changed(sender As Object, newVal As Integer)
        _ProgLine(DataFieldEnum.df321V1) = newVal
    End Sub
    Private Sub ucvcSetSource321_Val2_Changed(sender As Object, newVal As Integer)
        _ProgLine(DataFieldEnum.df321V2) = newVal
    End Sub

    Private Sub cboSetModOperator82V2_SelectedIndexChanged(sender As Object, e As EventArgs)
        Dim tmpVal As Integer = cboSetModOperator82V2.SelectedIndex
        _ProgLine(DataFieldEnum.df82V2) = tmpVal
        ucvcSetModifierVal322.SourceLabel = mathFunctionString(cboSetModOperator82V2.SelectedIndex) + " "
        If tmpVal = 0 Then 'Public mathFunctionString As String() = {" ", "+", "-", "x", "/", "mod"}
            ucvcSetModifierVal322.Visible = False
        Else
            ucvcSetModifierVal322.Visible = True
        End If
    End Sub

    Private Sub ucvcSetModifierVal322_Source_Changed(sender As Object, newSourceIndex As Integer)
        _ProgLine(DataFieldEnum.df322S) = newSourceIndex
    End Sub
    Private Sub ucvcSetModifierVal322_Val_Changed(sender As Object, newVal As Integer)
        _ProgLine(DataFieldEnum.df322V1) = newVal
    End Sub
    Private Sub ucvcSetModifierVal322_Val2_Changed(sender As Object, newVal As Integer)
        _ProgLine(DataFieldEnum.df322V2) = newVal
    End Sub

#End Region
#Region "SetModifier_Events"

#End Region
#Region "ProgramControl_Events"

    Private Sub cboProgControlOperation_SelectedIndexChanged(sender As Object, e As EventArgs)
        Dim tmpVal As Integer = cboProgControlOperation.SelectedIndex
        _ProgLine(DataFieldEnum.df81V1) = tmpVal
        If tmpVal > 0 Then
            lblProgControlChannel.Visible = True
            cboProgControlChannel.Visible = True
            If (tmpVal = OpTypeEnum.opLoadProgramAndPause) Or (tmpVal = OpTypeEnum.opLoadProgramAndRun) Then
                ucvcProgControlProgNum321.Visible = True
            Else
                ucvcProgControlProgNum321.Visible = False
            End If
        Else
            lblProgControlChannel.Visible = False
            cboProgControlChannel.Visible = False
            ucvcProgControlProgNum321.Visible = False
        End If
    End Sub

    Private Sub cboProgControlChannel_SelectedIndexChanged(sender As Object, e As EventArgs)
        _ProgLine(DataFieldEnum.dfChannel) = cboProgControlChannel.SelectedIndex
    End Sub

    Private Sub ucvcProgControlProgNum321_Source_Changed(sender As Object, newSourceIndex As Integer)
        _ProgLine(DataFieldEnum.df321S) = newSourceIndex
    End Sub
    Private Sub ucvcProgControlProgNum321_Val_Changed(sender As Object, newVal As Integer)
        _ProgLine(DataFieldEnum.df321V1) = newVal
    End Sub
    Private Sub ucvcProgControlProgNum321_Val2_Changed(sender As Object, newVal As Integer)
        _ProgLine(DataFieldEnum.df321V2) = newVal
    End Sub







#End Region

#Region "Display_Events"

#End Region
#Region "SendMsg_Events"

#End Region
    'Private Sub _Source_Changed(sender As Object, newSourceIndex As Integer)
    '    _ProgLine(DataFieldEnum.df) = newSourceIndex
    'End Sub
    'Private Sub _Val_Changed(sender As Object, newVal As Integer)
    '    _ProgLine(DataFieldEnum.df) = newVal
    'End Sub
    'Private Sub _Val2_Changed(sender As Object, newVal As Integer)
    '    _ProgLine(DataFieldEnum.df) = newVal
    'End Sub
End Class
