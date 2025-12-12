Imports System.ComponentModel
Imports Tens25.comDef

Public Class UserControlProgLine1

    Dim _width = 670
    Dim _hSmall = 22 '21
    Dim _hBig = 42
    Dim _sizeSmall = New Size(670, _hSmall) 'Dim _sizeSmall = New Size(670, 21)
    Dim _sizeBig = New Size(670, _hBig) 'Dim _sizeBig = New Size(670, 42)
    Dim _sizeCur As Size

    Dim _fontNormal As Font
    Dim _fontBold As Font


    Dim posLblP1X As Integer = 0
    Dim posControl1X As Integer = 56
    Dim posLblP2X As Integer = 167
    Dim posControl2X As Integer = 223
    Dim posControl3X As Integer = 300
    Dim posLblP3UnitX As Integer = 330
    Dim posLblP4X As Integer = 347
    Dim posControl4X As Integer = 404
    Dim posControl5X As Integer = 481
    Dim posLblP5UnitX As Integer = 535

    Dim posLblYR1 As Integer = 5
    Dim posLblYR2 As Integer = 25
    Dim posCboYR1 As Integer = 1 '0
    Dim posCboYR2 As Integer = 20
    Dim posTxtYR1 As Integer = 1 '0
    Dim posTxtYR2 As Integer = 21


    Dim _startupComplete = False

    Dim _LineNum As Integer = -1
    Dim _lineCommand As Integer = 0
    Dim _lineChan As Integer = -1
    Dim _lineGotoTrue As Integer = -1
    Dim _lineGotoFalse As Integer = -1
    Dim _lineP81S As Integer = 0
    Dim _lineP81V As Integer = 0
    Dim _lineP82S As Integer = 0
    Dim _lineP82V As Integer = 0
    Dim _linePolarity As Integer = 0
    Dim _lineP321S As Integer = 0
    Dim _lineP321V As Integer = 0
    Dim _lineP322S As Integer = 0
    Dim _lineP322V As Integer = 0

    Dim _origValues(13) As Integer

    'Dim _lineType As Integer = enumChantype.Unknown

    'Dim _numMotorChans As Integer = 2
    ''Dim _tensChanListString As String()
    'Dim _numTensChans As Integer = 2
    ''Dim _motorChanListString As String()
    'Dim _numsysChans As Integer = 1
    ''Dim _sysChanListString As String()
    Dim _chanType As enumChantype = enumChantype.Unknown

    Dim _highlightRow As Boolean = False
    Dim _allowEdit As Boolean = False

    Dim _unsavedChanges As Boolean = False

    Public Event ProgLineSizeChanged(ByVal lineNum As Integer, ByVal yPos As Integer, ByVal newHeight As Integer)
    Public Event ProgLineGotFocus(ByVal lineNum As Integer)
    Public Event ProgLineLeaveWithLineNumber(ByVal lineNum As Integer)


    'Public Enum LineCommand As Integer
    '    cmdNoOp = 0
    '    cmdTens = 1
    '    cmdMotor = 2
    '    cmdGoTo = 3
    '    cmdEnd = 4
    '    cmdTest = 5
    '    cmdSet = 6
    '    cmdDelay = 7
    'End Enum

    'Public Property NumMotorChans As Integer
    '    Get
    '        Return _numMotorChans
    '    End Get
    '    Set(value As Integer)
    '        _numMotorChans = value
    '        If _startupComplete = True Then InitializeChanListStrings()
    '    End Set
    'End Property
    'Public Property NumTensChans As Integer
    '    Get
    '        Return _numTensChans
    '    End Get
    '    Set(value As Integer)
    '        _numTensChans = value
    '        If _startupComplete = True Then InitializeChanListStrings()
    '    End Set
    'End Property
    'Public Property NumSysChans As Integer
    '    Get
    '        Return _numsysChans
    '    End Get
    '    Set(value As Integer)
    '        _numsysChans = value
    '        If _startupComplete = True Then InitializeChanListStrings()
    '    End Set
    'End Property

    Public Property UnsavedChanges As Boolean
        Get
            _unsavedChanges = ((_origValues(0) <> _LineNum) Or
                    (_origValues(1) <> _lineCommand) Or
                    (_origValues(2) <> _lineChan) Or
                    (_origValues(3) <> _lineGotoTrue) Or
                    (_origValues(4) <> _lineGotoFalse) Or
                    (_origValues(5) <> _lineP81S) Or
                    (_origValues(6) <> _lineP81V) Or
                    (_origValues(7) <> _lineP82S) Or
                    (_origValues(8) <> _lineP82V) Or
                    (_origValues(9) <> _linePolarity) Or
                    (_origValues(10) <> _lineP321S) Or
                    (_origValues(11) <> _lineP321V) Or
                    (_origValues(12) <> _lineP322S) Or
                    (_origValues(13) <> _lineP322V))
            Return _unsavedChanges

        End Get
        Set(value As Boolean)
            If value = False Then
                _origValues(0) = _LineNum
                _origValues(1) = _lineCommand
                _origValues(2) = _lineChan
                _origValues(3) = _lineGotoTrue
                _origValues(4) = _lineGotoFalse
                _origValues(5) = _lineP81S
                _origValues(6) = _lineP81V
                _origValues(7) = _lineP82S
                _origValues(8) = _lineP82V
                _origValues(9) = _linePolarity
                _origValues(10) = _lineP321S
                _origValues(11) = _lineP321V
                _origValues(12) = _lineP322S
                _origValues(13) = _lineP322V
            Else
                _unsavedChanges = True
            End If
        End Set
    End Property

    Public Property LineNum As Integer
        Get
            Return _LineNum
        End Get
        Set(value As Integer)
            _LineNum = value
            If _startupComplete Then
                lblLine.Text = _LineNum
            End If
        End Set
    End Property

    Public Property Command As Integer
        Get
            Return _lineCommand
        End Get
        Set(value As Integer)
            _lineCommand = value

            'Me.SuspendLayout()

            lblChannel.Visible = False
            cboChannel.Visible = False
            lblPolarity.Visible = False
            cboPolarity.Visible = False
            lblP81S.Visible = False
            cboP81S.Visible = False
            txtP81V.Visible = False
            lblP81VUnits.Visible = False
            cboP82S.Visible = False
            txtP82V.Visible = False
            lblP82VUnits.Visible = False
            lblP321S.Visible = False
            cboP321S.Visible = False
            txtP321V.Visible = False
            lblP321VUnits.Visible = False
            lblP322S.Visible = False
            cboP322S.Visible = False
            txtP322V.Visible = False
            lblP322VUnits.Visible = False
            cboOpType.Visible = False

            If (_lineCommand = CommandEnum.cmdNoop) Or (_lineCommand = CommandEnum.cmdEnd) Then
                _sizeCur = _sizeSmall
                lblLine.Height = _hSmall
                'pnlNoOp_End.Visible = True
                'pnlTensMotor.Visible = False
                'pnlDelay.Visible = False
                'pnlGoto.Visible = False
                'pnlTest.Visible = False
                'pnlSet.Visible = False




            ElseIf (_lineCommand = CommandEnum.cmdTensOutput) Or (_lineCommand = CommandEnum.cmdMotorOutput) Then
                _sizeCur = _sizeBig
                lblLine.Height = _hBig
                'pnlNoOp_End.Visible = False
                'pnlTensMotor.Visible = True
                'pnlDelay.Visible = False
                'pnlGoto.Visible = False
                'pnlTest.Visible = False
                'pnlSet.Visible = False

                lblChannel.Visible = False
                cboChannel.Visible = False

                lblPolarity.Location = New Point(posLblP1X, posLblYR1)
                lblPolarity.Visible = True

                cboPolarity.Location = New Point(posControl1X, posCboYR1)
                cboPolarity.Visible = True

                lblP81S.Text = "Start:"
                lblP81S.Location = New Point(posLblP2X, posLblYR1)
                lblP81S.Visible = True

                cboP81S.Location = New Point(posControl2X, posCboYR1)
                cboP81S.Visible = True
                txtP81V.Location = New Point(posControl3X, posTxtYR1)
                txtP81V.Visible = True
                lblP81VUnits.Location = New Point(posLblP3UnitX, posLblYR1)
                lblP81VUnits.Text = "%"
                lblP81VUnits.Visible = True

                lblP82S.Text = "End:"
                lblP82S.Location = New Point(posLblP2X, posLblYR2)
                lblP82S.Visible = True

                cboP82S.Location = New Point(posControl2X, posCboYR2)
                cboP82S.Visible = True
                txtP82V.Location = New Point(posControl3X, posTxtYR2)
                txtP82V.Visible = True
                lblP82VUnits.Location = New Point(posLblP3UnitX, posLblYR2)
                lblP82VUnits.Text = "%"
                lblP82VUnits.Visible = True

                lblP321S.Location = New Point(posLblP4X, posLblYR1)
                lblP321S.Text = "Duration:"
                lblP321S.Visible = True
                cboP321S.Location = New Point(posControl4X, posCboYR1)
                cboP321S.Visible = True
                txtP321V.Location = New Point(posControl5X, posTxtYR1)
                txtP321V.Visible = True
                lblP321VUnits.Location = New Point(posLblP5UnitX, posLblYR1)
                lblP321VUnits.Text = "ms"
                lblP321VUnits.Visible = True

                lblP322S.Location = New Point(posLblP4X, posLblYR2)
                lblP322S.Text = "Repeats:"
                lblP322S.Visible = True
                cboP322S.Location = New Point(posControl4X, posCboYR2)
                cboP322S.Visible = True
                txtP322V.Location = New Point(posControl5X, posTxtYR2)
                txtP322V.Visible = True
                lblP322VUnits.Location = New Point(posLblP5UnitX, posLblYR2)


            ElseIf (_lineCommand = CommandEnum.cmdGoTo) Then
                _sizeCur = _sizeSmall
                lblLine.Height = _hSmall
                'pnlNoOp_End.Visible = False
                'pnlTensMotor.Visible = False
                'pnlDelay.Visible = False
                'pnlGoto.Visible = True
                'pnlTest.Visible = False
                'pnlSet.Visible = False

                lblPolarity.Text = "Line:"
                lblPolarity.Location = New Point(posLblP1X, posLblYR1)
                lblPolarity.Visible = True
                txtGotoTrue.Location = New Point(posControl1X, posTxtYR1)
                txtGotoTrue.Visible = True

            ElseIf _lineCommand = CommandEnum.cmdTest Then
                _sizeCur = _sizeSmall
                lblLine.Height = _hSmall
                'pnlNoOp_End.Visible = False
                'pnlTensMotor.Visible = False
                'pnlDelay.Visible = False
                'pnlGoto.Visible = False
                'pnlTest.Visible = True
                'pnlSet.Visible = False
            ElseIf _lineCommand = CommandEnum.cmdSet Then
                _sizeCur = _sizeSmall
                lblLine.Height = _hSmall
                'pnlNoOp_End.Visible = False
                'pnlTensMotor.Visible = False
                'pnlDelay.Visible = False
                'pnlGoto.Visible = False
                'pnlTest.Visible = False
                'pnlSet.Visible = True

            ElseIf (_lineCommand = CommandEnum.cmdDelay) Then
                _sizeCur = _sizeSmall
                lblLine.Height = _hSmall
                'pnlNoOp_End.Visible = False
                'pnlTensMotor.Visible = False
                'pnlDelay.Visible = True
                'pnlGoto.Visible = False
                'pnlTest.Visible = False
                'pnlSet.Visible = False

                lblPolarity.Text = "Delay:"
                lblPolarity.Location = New Point(posLblP1X, posLblYR1)
                lblPolarity.Visible = True

                cboP321S.Location = New Point(posControl1X, posCboYR1)
                cboP321S.Visible = True
                txtP321V.Location = New Point(cboP321S.Location.X + cboP321S.Width + 2, posTxtYR1)
                txtP321V.Visible = True
                lblP321VUnits.Location = New Point(txtP321V.Location.X + txtP321V.Width + 1, posLblYR1)
                lblP321VUnits.Text = "ms"
                lblP321VUnits.Visible = True

            ElseIf (_lineCommand = CommandEnum.cmdProgControl) Then
                lblPolarity.Text = "Op Type:"
                lblPolarity.Location = New Point(posLblP1X, posLblYR1)
                lblPolarity.Visible = True

                cboOpType.Location = New Point(posControl1X, posCboYR1)
                cboOpType.Visible = True

                lblChannel.Text = "Channel:"
                lblChannel.Location = New Point(posLblP2X, posLblYR1)
                lblChannel.Visible = True
                cboChannel.Location = New Point(posControl2X, posCboYR1)
                cboChannel.Visible = True

                lblP321S.Location = New Point(posLblP4X, posLblYR1)
                lblP321S.Text = "Prog Num:"

                cboP321S.Location = New Point(posControl4X, posCboYR1)
                txtP321V.Location = New Point(posControl5X, posTxtYR1)

            End If

            Size = _sizeCur

            If _startupComplete = True Then
                InitializeValuesForCommand()
            End If

            'Me.ResumeLayout()
        End Set
    End Property

    Public Property ChannelNum As Integer
        Get
            'If _lineCommand = LineCommand.cmdTens Then
            'Return _lineChan + NumMotorChans
            'Else
            Return _lineChan
            'End If
        End Get
        Set(value As Integer)
            _lineChan = value
        End Set
    End Property

    'Public Property ChannelType As Integer
    '    Get
    '        Return _lineType
    '    End Get
    '    Set(value As Integer)
    '        _lineType = value
    '        'TODO: here is where control lists could be tailored specifically for tens, motor, system channels
    '    End Set
    'End Property

    Public Property GoToTrue As Integer
        Get
            Return _lineGotoTrue
        End Get
        Set(value As Integer)
            _lineGotoTrue = value
        End Set
    End Property

    Public Property GoToFalse As Integer
        Get
            Return _lineGotoFalse
        End Get
        Set(value As Integer)
            _lineGotoFalse = value
        End Set
    End Property

    Public Property P81S As Integer
        Get
            Return _lineP81S
        End Get
        Set(value As Integer)
            _lineP81S = value
        End Set
    End Property
    Public Property P81V As Integer
        Get
            Return _lineP81V
        End Get
        Set(value As Integer)
            _lineP81V = value
        End Set
    End Property
    Public Property P82S As Integer
        Get
            Return _lineP82S
        End Get
        Set(value As Integer)
            _lineP82S = value
        End Set
    End Property
    Public Property P82V As Integer
        Get
            Return _lineP82V
        End Get
        Set(value As Integer)
            _lineP82V = value
        End Set
    End Property
    Public Property Polarity As Integer
        Get
            Return _linePolarity
        End Get
        Set(value As Integer)
            _linePolarity = value
        End Set
    End Property

    Public Property P321S As Integer
        Get
            Return _lineP321S
        End Get
        Set(value As Integer)
            _lineP321S = value
        End Set
    End Property
    Public Property P321V As Integer
        Get
            Return _lineP321V
        End Get
        Set(value As Integer)
            _lineP321V = value
        End Set
    End Property
    Public Property P322S As Integer
        Get
            Return _lineP322S
        End Get
        Set(value As Integer)
            _lineP322S = value
        End Set
    End Property
    Public Property P322V As Integer
        Get
            Return _lineP322V
        End Get
        Set(value As Integer)
            _lineP322V = value
        End Set
    End Property


    Public ReadOnly Property GetCellValues() As String()
        Get
            'Dim tmpChan As Integer = _lineChan
            'If _lineCommand = LineCommand.cmdTens Then
            '    tmpChan += NumMotorChans
            'End If
            'Dim retVal() As String = {_LineNum.ToString, _lineCommand.ToString, tmpChan.ToString, _lineGotoTrue.ToString,
            '                  _lineGotoFalse.ToString, _lineP81S.ToString, _lineP81V.ToString,
            '                  _lineP82S.ToString, _lineP82V.ToString, _linePolarity.ToString,
            '                  _lineP321S.ToString, _lineP321V.ToString, _lineP322S.ToString, _lineP322V.ToString}
            'Return retVal
            Dim retVal() As String = {_LineNum.ToString, _lineCommand.ToString, _lineChan.ToString, _lineGotoTrue.ToString,
                              _lineGotoFalse.ToString, _lineP81S.ToString, _lineP81V.ToString,
                              _lineP82S.ToString, _lineP82V.ToString, _linePolarity.ToString,
                              _lineP321S.ToString, _lineP321V.ToString, _lineP322S.ToString, _lineP322V.ToString}
            Return retVal
        End Get
    End Property


    'Public Property ValArray(ByVal NumMotorChans As Integer, ByVal NumTensChans As Integer,
    '                         ByVal NumSysChans As Integer, ByVal ParamArray vals() As String) As String()
    '    Get
    '        Dim tmpChan As Integer = _lineChan
    '        If _lineCommand = LineCommand.cmdTens Then
    '            tmpChan += NumMotorChans
    '        End If
    '        Dim retVal() As String = {_LineNum.ToString, _lineCommand.ToString, tmpChan.ToString, _lineGotoTrue.ToString,
    '                          _lineGotoFalse.ToString, _lineP81S.ToString, _lineP81V.ToString,
    '                          _lineP82S.ToString, _lineP82V.ToString, _linePolarity.ToString,
    '                          _lineP321S.ToString, _lineP321V.ToString, _lineP322S.ToString, _lineP322V.ToString}
    '        Return retVal
    '    End Get
    '    Set(value As String())
    '        If vals.Length = 14 Then
    '            _numMotorChans = NumMotorChans
    '            _numTensChans = NumTensChans
    '            _numsysChans = NumSysChans
    '            _LineNum = vals(0)
    '            _lineCommand = vals(1)
    '            _lineChan = vals(2)
    '            _lineGotoTrue = vals(3)
    '            _lineGotoFalse = vals(4)
    '            _lineP81S = vals(5)
    '            _lineP81V = vals(6)
    '            _lineP82S = vals(7)
    '            _lineP82V = vals(8)
    '            _linePolarity = vals(9)
    '            _lineP321S = vals(10)
    '            _lineP321V = vals(11)
    '            _lineP322S = vals(12)
    '            _lineP322V = vals(13)
    '        End If
    '    End Set
    'End Property
    Public Property ValArray(ByVal LineType As Integer, ByVal ParamArray vals() As String) As String()
        Get
            Dim retVal() As String = {_LineNum.ToString, _lineCommand.ToString, _lineChan.ToString, _lineGotoTrue.ToString,
                              _lineGotoFalse.ToString, _lineP81S.ToString, _lineP81V.ToString,
                              _lineP82S.ToString, _lineP82V.ToString, _linePolarity.ToString,
                              _lineP321S.ToString, _lineP321V.ToString, _lineP322S.ToString, _lineP322V.ToString}
            Return retVal
        End Get
        Set(value As String())
            If vals.Length = 14 Then
                '_lineType = LineType
                _LineNum = vals(0)
                _lineCommand = vals(1)
                _lineChan = vals(2)
                _lineGotoTrue = vals(3)
                _lineGotoFalse = vals(4)
                _lineP81S = vals(5)
                _lineP81V = vals(6)
                _lineP82S = vals(7)
                _lineP82V = vals(8)
                _linePolarity = vals(9)
                _lineP321S = vals(10)
                _lineP321V = vals(11)
                _lineP322S = vals(12)
                _lineP322V = vals(13)
            End If
        End Set
    End Property

    Public Property HighlightRow(Optional ByVal AllowEdit As Boolean = False) As Boolean
        Get
            Return _highlightRow
        End Get
        Set(value As Boolean)
            _highlightRow = value
            If _highlightRow = True Then
                BorderStyle = BorderStyle.Fixed3D
                lblLine.Font = _fontBold

            Else
                BorderStyle = BorderStyle.None
                lblLine.Font = _fontNormal
            End If

            If AllowEdit = True Then
                cboCommand.DropDownStyle = ComboBoxStyle.DropDownList
                cboChannel.DropDownStyle = ComboBoxStyle.DropDownList
                cboP81S.DropDownStyle = ComboBoxStyle.DropDownList
                cboP82S.DropDownStyle = ComboBoxStyle.DropDownList
                cboPolarity.DropDownStyle = ComboBoxStyle.DropDownList
                cboP321S.DropDownStyle = ComboBoxStyle.DropDownList
                cboP322S.DropDownStyle = ComboBoxStyle.DropDownList
                cboOpType.DropDownStyle = ComboBoxStyle.DropDownList

            Else
                cboCommand.DropDownStyle = ComboBoxStyle.Simple
                cboChannel.DropDownStyle = ComboBoxStyle.Simple
                cboP81S.DropDownStyle = ComboBoxStyle.Simple
                cboP82S.DropDownStyle = ComboBoxStyle.Simple
                cboPolarity.DropDownStyle = ComboBoxStyle.Simple
                cboP321S.DropDownStyle = ComboBoxStyle.Simple
                cboP322S.DropDownStyle = ComboBoxStyle.Simple
                cboOpType.DropDownStyle = ComboBoxStyle.Simple
            End If
        End Set
    End Property

    'Private Sub InitializeChanListStrings()
    '    If _numMotorChans > 0 Then
    '        ReDim motorChanNameString(_numMotorChans - 1)
    '        For n As Integer = 0 To _numMotorChans - 1
    '            motorChanNameString(n) = New String("Motor " & (n + 1))
    '        Next
    '    End If
    '    If _numTensChans > 0 Then
    '        ReDim tensChanNameString(_numTensChans - 1)
    '        For n As Integer = 0 To _numTensChans - 1
    '            tensChanNameString(n) = New String("Tens " & (n + 1))
    '        Next
    '    End If

    '    If _numsysChans > 0 Then
    '        ReDim sysChanNameString(_numsysChans - 1)
    '        For n As Integer = 0 To _numsysChans - 1
    '            sysChanNameString(n) = New String("Sys " & (n + 1))
    '        Next
    '    End If

    'End Sub

    Private Sub InitializeValuesForCommand()
        If Not IsNothing(cboCommand) Then
            If Not IsNothing(cboCommand.Items) Then
                If cboCommand.Items.Count = 0 Then
                    cboCommand.Items.Clear()
                    'If Not IsNothing(commandVals) Then
                    cboCommand.Items.AddRange(commandVals)
                    'Else
                    'cboCommand.Items.AddRange(commandVals) ' New String("No Op,Tens,Motor,GoTo,End,Test,Set,Delay").Split(","))
                    'End If
                End If
            End If
        End If


        If cboCommand.SelectedIndex <> _lineCommand Then
            'This happens when the "Command" property is set by an outside object, rather than the combobox being changed
            'by the user or software.  We just need to set the combobox to match the _lineCommand value.
            'RemoveHandler cboCommand.SelectedIndexChanged, AddressOf cboCommand_SelectedIndexChanged
            cboCommand.SelectedIndex = _lineCommand
            Exit Sub
        End If


        If (_lineCommand = CommandEnum.cmdNoop) Or (_lineCommand = CommandEnum.cmdEnd) Then
            'Nothing to do.
        ElseIf (_lineCommand = CommandEnum.cmdTensOutput) Or (_lineCommand = CommandEnum.cmdMotorOutput) Then
            'Set up for Tens or Motor
            'If _lineCommand = CommandEnum.cmdTensOutput Then
            '    If (cboChannel.Items.Count <> _numTensChans) Or (IsNothing(cboChannel.SelectedItem)) Then
            '        cboChannel.Items.Clear()
            '        cboChannel.Items.AddRange(tensChanNameString)
            '        If (_lineChan < _numMotorChans) Or (_lineChan >= (_numMotorChans + _numTensChans)) Then
            '            _lineChan = _numMotorChans 'reset the chan number into the Tens channel realm.
            '        End If
            '        cboChannel.SelectedIndex = _lineChan - _numMotorChans
            '    ElseIf Not cboChannel.SelectedItem.ToString.StartsWith(tensChanNameString(0).Substring(0, 3)) Then
            '        cboChannel.Items.Clear()
            '        cboChannel.Items.AddRange(tensChanNameString)
            '        If (_lineChan < _numMotorChans) Or (_lineChan >= (_numMotorChans + _numTensChans)) Then
            '            _lineChan = _numMotorChans 'reset the chan number into the Tens channel realm.
            '        End If
            '        cboChannel.SelectedIndex = _lineChan - _numMotorChans
            '    End If
            'Else 'Motor
            '    If (cboChannel.Items.Count <> _numMotorChans) Or (IsNothing(cboChannel.SelectedItem)) Then
            '        cboChannel.Items.Clear()
            '        cboChannel.Items.AddRange(motorChanNameString)
            '        If _lineChan >= _numMotorChans Then
            '            _lineChan = 0 'reset the chan number into the Motor channel realm.
            '        End If
            '        cboChannel.SelectedIndex = _lineChan
            '    ElseIf Not cboChannel.SelectedItem.ToString.StartsWith(motorChanNameString(0).Substring(0, 3)) Then
            '        cboChannel.Items.Clear()
            '        cboChannel.Items.AddRange(motorChanNameString)
            '        If _lineChan >= _numMotorChans Then
            '            _lineChan = 0 'reset the chan number into the Motor channel realm.
            '        End If
            '        cboChannel.SelectedIndex = _lineChan
            '    End If
            'End If

            If cboP81S.Items.Count < dataSourceString.Length Then
                cboP81S.Items.Clear()
                cboP81S.Items.AddRange(dataSourceString)
            End If
            cboP81S.SelectedIndex = _lineP81S 'DataSourceEnum.dsDirect




            If cboP82S.Items.Count < dataSourceString.Length Then
                cboP82S.Items.Clear()
                cboP82S.Items.AddRange(dataSourceString)
            End If
            cboP82S.SelectedIndex = _lineP82S 'DataSourceEnum.dsDirect

            'If txtP81V.Text.Length = 0 Then
            txtP81V.Text = _lineP81V '"0"
            'End If

            'If txtP82V.Text.Length = 0 Then
            txtP82V.Text = _lineP82V '"0"
            'End If

            If (cboPolarity.Items.Count < 1) Or (IsNothing(cboPolarity.SelectedItem)) Then
                cboPolarity.Items.Clear()
                cboPolarity.Items.AddRange(polarityString)
            End If
            cboPolarity.SelectedIndex = _linePolarity '0

            If (cboP321S.Items.Count < 1) Or (IsNothing(cboP321S.SelectedItem)) Then
                cboP321S.Items.Clear()
                cboP321S.Items.AddRange(dataSourceString)
            End If
            cboP321S.SelectedIndex = _lineP321S 'DataSourceEnum.dsDirect


            'If txtP321V.Text.Length = 0 Then
            txtP321V.Text = _lineP321V '"1000" 
            'End If


            If (cboP322S.Items.Count < 1) Or IsNothing(cboP322S.SelectedItem) Then
                cboP322S.Items.Clear()
                cboP322S.Items.AddRange(dataSourceString)
            End If
            cboP322S.SelectedIndex = _lineP322S 'DataSourceEnum.dsDirect


            'If txtP322V.Text.Length = 0 Then
            txtP322V.Text = _lineP322V '"0"
            'End If

        ElseIf _lineCommand = CommandEnum.cmdGoTo Then
            'If txtGotoTrue.Text.Length = 0 Then
            txtGotoTrue.Text = _lineGotoTrue '"0"
            'End If

        ElseIf _lineCommand = CommandEnum.cmdDelay Then
            If (cboP321S.Items.Count < 1) Or (IsNothing(cboP321S.SelectedItem)) Then
                cboP321S.Items.Clear()
                cboP321S.Items.AddRange(dataSourceString)
            End If
            cboP321S.SelectedIndex = _lineP321S 'DataSourceEnum.dsDirect
            txtP321V.Text = _lineP321V '"100"

        ElseIf _lineCommand = CommandEnum.cmdTest Then

        ElseIf _lineCommand = CommandEnum.cmdSet Then

        ElseIf _lineCommand = CommandEnum.cmdProgControl Then
            If (cboOpType.Items.Count < 1) Or IsNothing(cboOpType.SelectedItem) Then
                cboOpType.Items.Clear()
                cboOpType.Items.AddRange(programOpTypeString)
            End If
            cboOpType.SelectedIndex = _lineP81V


            If (cboChannel.Items.Count < 1) Then
                cboChannel.Items.Clear()
                cboChannel.Items.AddRange(allChannelsString)
            End If
            cboChannel.SelectedIndex = _lineChan

            If (cboP321S.Items.Count < 1) Or (IsNothing(cboP321S.SelectedItem)) Then
                cboP321S.Items.Clear()
                cboP321S.Items.AddRange(dataSourceString)
            End If
            cboP321S.SelectedIndex = _lineP321S 'DataSourceEnum.dsDirect

            txtP321V.Text = _lineP321V
        End If

    End Sub




    Private Sub UserControlProgLine_Load(sender As Object, e As EventArgs) Handles Me.Load
        _startupComplete = True
        lblLine.Text = _LineNum
        InitializeValuesForCommand()

        _fontNormal = New Font(lblLine.Font, FontStyle.Regular)
        _fontBold = New Font(lblLine.Font, FontStyle.Bold)

        HighlightRow(False) = False


        SetFocusEventHandlers(Me.Controls)

    End Sub

    Private Sub SetFocusEventHandlers(ByRef tmpContainer As ControlCollection)
        For Each tmpControl As Control In tmpContainer
            If TypeOf tmpControl Is Control Then
                If tmpControl.Controls.Count > 0 Then
                    SetFocusEventHandlers(tmpControl.Controls)
                End If
                If (TypeOf tmpControl Is Panel) Or (TypeOf tmpControl Is TextBox) Or
                    (TypeOf tmpControl Is Label) Then
                    AddHandler tmpControl.Click, AddressOf ProgramLine_GotFocus
                End If
            End If
        Next
    End Sub

    Private Sub UserControlProgLine_SizeChanged(sender As Object, e As EventArgs) Handles Me.SizeChanged
        RaiseEvent ProgLineSizeChanged(_LineNum, Location.Y, Size.Height)
    End Sub

    Private Sub ProgramLine_GotFocus(sender As Object, e As EventArgs)
        RaiseEvent ProgLineGotFocus(_LineNum)
    End Sub



    Private Sub cboCommand_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboCommand.SelectedIndexChanged
        '        If cboCommand.SelectedIndex <> _origValues(1) Then _unsavedChanges = True
        Command = cboCommand.SelectedIndex
    End Sub

    Private Sub cboChannel_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboChannel.SelectedIndexChanged
        _lineChan = cboChannel.SelectedIndex
        'If _lineCommand = LineCommand.cmdTens Then
        '    _lineChan += _numMotorChans
        'End If
    End Sub

    Private Sub cboStartSource_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboP81S.SelectedIndexChanged
        '       If cboStartSource.SelectedIndex <> _origValues() Then _unsavedChanges = True
        _lineP81S = cboP81S.SelectedIndex
    End Sub

    Private Sub cboEndSource_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboP82S.SelectedIndexChanged
        '       If cboEndSource.SelectedIndex <> _origValues() Then _unsavedChanges = True
        _lineP82S = cboP82S.SelectedIndex
    End Sub

    Private Sub cboPolarity_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboPolarity.SelectedIndexChanged
        '       If cboPolarity.SelectedIndex <> _origValues() Then _unsavedChanges = True
        _linePolarity = cboPolarity.SelectedIndex
    End Sub

    Private Sub cboOpType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboOpType.SelectedIndexChanged
        _lineP81V = cboOpType.SelectedIndex
        If _lineP81V = OpTypeEnum.opLoadProgram Then
            lblP321S.Visible = True
            cboP321S.Visible = True
            txtP321V.Visible = True
        Else
            lblP321S.Visible = False
            cboP321S.Visible = False
            txtP321V.Visible = False
        End If
    End Sub

    Private Sub cboP321S_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboP321S.SelectedIndexChanged
        _lineP321S = cboP321S.SelectedIndex
    End Sub

    Private Sub cboP322S_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboP322S.SelectedIndexChanged
        _lineP322S = cboP322S.SelectedIndex
    End Sub

    Private Sub txtP81V_TextChanged(sender As Object, e As EventArgs) Handles txtP81V.TextChanged
        If IsNumeric(txtP81V.Text) = False Then
            txtP81V.Text = _lineP81V
        Else
            _lineP81V = txtP81V.Text
        End If
    End Sub


    Private Sub txtP82V_TextChanged(sender As Object, e As EventArgs) Handles txtP82V.TextChanged
        If IsNumeric(txtP82V.Text) = False Then
            txtP82V.Text = _lineP82V
        Else
            _lineP82V = txtP82V.Text
        End If
    End Sub

    Private Sub txtP321V_TextChanged(sender As Object, e As EventArgs) Handles txtP321V.TextChanged
        If IsNumeric(txtP321V.Text) = False Then
            txtP321V.Text = _lineP321V
        Else
            _lineP321V = txtP321V.Text
        End If
    End Sub

    Private Sub txtP322V_TextChanged(sender As Object, e As EventArgs) Handles txtP322V.TextChanged
        If IsNumeric(txtP322V.Text) = False Then
            txtP322V.Text = _lineP322V
        Else
            _lineP322V = txtP322V.Text
        End If
    End Sub



    Private Sub txtGotoTrue_TextChanged(sender As Object, e As EventArgs) Handles txtGotoTrue.TextChanged
        If IsNumeric(txtGotoTrue.Text) = False Then
            txtGotoTrue.Text = _lineGotoTrue
        Else
            _lineGotoTrue = txtGotoTrue.Text
        End If
    End Sub


    Private Sub UserControlProgLine_Leave(sender As Object, e As EventArgs) Handles Me.Leave
        RaiseEvent ProgLineLeaveWithLineNumber(_LineNum)
    End Sub


End Class
