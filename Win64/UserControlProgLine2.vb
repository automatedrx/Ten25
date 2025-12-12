

Imports System.ComponentModel
Imports Tens25.comDef

Public Class UserControlProgLine2

    Dim _width = 670
    Dim _hSmall = 23 '21
    Dim _hBig = 44
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
    Dim _chanType As enumChantype = enumChantype.Unknown

    Dim _highlightRow As Boolean = False
    Dim _allowEdit As Boolean = False

    Dim _unsavedChanges As Boolean = False

    Public Event ProgLineSizeChanged(ByVal lineNum As Integer, ByVal yPos As Integer, ByVal newHeight As Integer)
    Public Event ProgLineGotFocus(ByVal lineNum As Integer)
    Public Event ProgLineLeaveWithLineNumber(ByVal lineNum As Integer)


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

            If (_lineCommand = CommandEnum.cmdNoop) Or (_lineCommand = CommandEnum.cmdEnd) Then
                _sizeCur = _sizeSmall
                lblLine.Height = _hSmall
                pnlNoOpEnd.Visible = True
                pnlTenMot.Visible = False
                pnlGoto.Visible = False
                pnlProgOp.Visible = False
                pnlDelay.Visible = False
                pnlTest.Visible = False
                pnlSet.Visible = False

            ElseIf (_lineCommand = CommandEnum.cmdTensOutput) Or (_lineCommand = CommandEnum.cmdMotorOutput) Then
                _sizeCur = _sizeBig
                lblLine.Height = _hBig
                pnlNoOpEnd.Visible = False
                pnlTenMot.Visible = True
                pnlGoto.Visible = False
                pnlProgOp.Visible = False
                pnlDelay.Visible = False
                pnlTest.Visible = False
                pnlSet.Visible = False

            ElseIf (_lineCommand = CommandEnum.cmdGoTo) Then
                _sizeCur = _sizeSmall
                lblLine.Height = _hSmall
                pnlNoOpEnd.Visible = False
                pnlTenMot.Visible = False
                pnlGoto.Visible = True
                pnlProgOp.Visible = False
                pnlDelay.Visible = False
                pnlTest.Visible = False
                pnlSet.Visible = False

            ElseIf _lineCommand = CommandEnum.cmdTest Then
                _sizeCur = _sizeBig
                lblLine.Height = _hBig
                pnlNoOpEnd.Visible = False
                pnlTenMot.Visible = False
                pnlGoto.Visible = False
                pnlProgOp.Visible = False
                pnlDelay.Visible = False
                pnlTest.Visible = True
                pnlSet.Visible = False

            ElseIf _lineCommand = CommandEnum.cmdSet Then
                _sizeCur = _sizeSmall
                lblLine.Height = _hSmall
                pnlNoOpEnd.Visible = False
                pnlTenMot.Visible = False
                pnlGoto.Visible = False
                pnlProgOp.Visible = False
                pnlDelay.Visible = False
                pnlTest.Visible = False
                pnlSet.Visible = True

            ElseIf (_lineCommand = CommandEnum.cmdDelay) Then
                _sizeCur = _sizeSmall
                lblLine.Height = _hSmall
                pnlNoOpEnd.Visible = False
                pnlTenMot.Visible = False
                pnlGoto.Visible = False
                pnlProgOp.Visible = False
                pnlDelay.Visible = True
                pnlTest.Visible = False
                pnlSet.Visible = False

            ElseIf (_lineCommand = CommandEnum.cmdProgControl) Then
                _sizeCur = _sizeSmall
                lblLine.Height = _hSmall
                pnlNoOpEnd.Visible = False
                pnlTenMot.Visible = False
                pnlGoto.Visible = False
                pnlProgOp.Visible = True
                pnlDelay.Visible = False
                pnlTest.Visible = False
                pnlSet.Visible = False

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
                cboTenMotPolarity.DropDownStyle = ComboBoxStyle.DropDownList
                cboTenMotP81S.DropDownStyle = ComboBoxStyle.DropDownList
                cboTenMotP82S.DropDownStyle = ComboBoxStyle.DropDownList
                cboTenMotP321S.DropDownStyle = ComboBoxStyle.DropDownList
                cboTenMotP322S.DropDownStyle = ComboBoxStyle.DropDownList

                cboProgOpP81S.DropDownStyle = ComboBoxStyle.DropDownList
                cboProgOpChannel.DropDownStyle = ComboBoxStyle.DropDownList
                cboProgOpP321S.DropDownStyle = ComboBoxStyle.DropDownList

                cboDelayP321S.DropDownStyle = ComboBoxStyle.DropDownList

                cboTestP81S.DropDownStyle = ComboBoxStyle.DropDownList
                cboTestP82V.DropDownStyle = ComboBoxStyle.DropDownList
                cboTestP321S.DropDownStyle = ComboBoxStyle.DropDownList
                cboTestP322S.DropDownStyle = ComboBoxStyle.DropDownList

                cboSetP81S.DropDownStyle = ComboBoxStyle.DropDownList
                cboSetP82S.DropDownStyle = ComboBoxStyle.DropDownList
                cboSetP321S.DropDownStyle = ComboBoxStyle.DropDownList
                cboSetP322S.DropDownStyle = ComboBoxStyle.DropDownList


            Else
                cboCommand.DropDownStyle = ComboBoxStyle.Simple
                cboTenMotPolarity.DropDownStyle = ComboBoxStyle.Simple
                cboTenMotP81S.DropDownStyle = ComboBoxStyle.Simple
                cboTenMotP82S.DropDownStyle = ComboBoxStyle.Simple
                cboTenMotP321S.DropDownStyle = ComboBoxStyle.Simple
                cboTenMotP322S.DropDownStyle = ComboBoxStyle.Simple

                cboProgOpP81S.DropDownStyle = ComboBoxStyle.Simple
                cboProgOpChannel.DropDownStyle = ComboBoxStyle.Simple
                cboProgOpP321S.DropDownStyle = ComboBoxStyle.Simple

                cboDelayP321S.DropDownStyle = ComboBoxStyle.Simple

                cboTestP81S.DropDownStyle = ComboBoxStyle.Simple
                cboTestP82V.DropDownStyle = ComboBoxStyle.Simple
                cboTestP321S.DropDownStyle = ComboBoxStyle.Simple
                cboTestP322S.DropDownStyle = ComboBoxStyle.Simple

                cboSetP81S.DropDownStyle = ComboBoxStyle.Simple
                cboSetP82S.DropDownStyle = ComboBoxStyle.Simple
                cboSetP321S.DropDownStyle = ComboBoxStyle.Simple
                cboSetP322S.DropDownStyle = ComboBoxStyle.Simple


            End If
        End Set
    End Property

    Private Sub InitializeValuesForCommand()
        If Not IsNothing(cboCommand) Then
            If Not IsNothing(cboCommand.Items) Then
                If cboCommand.Items.Count = 0 Then
                    cboCommand.Items.Clear()
                    cboCommand.Items.AddRange(commandVals)
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

            If cboTenMotP81S.Items.Count < dataSourceString.Length Then
                cboTenMotP81S.Items.Clear()
                cboTenMotP81S.Items.AddRange(dataSourceString)
            End If
            cboTenMotP81S.SelectedIndex = _lineP81S 'DataSourceEnum.dsDirect
            txtTenMotP81V.Text = _lineP81V '"0"

            If cboTenMotP82S.Items.Count < dataSourceString.Length Then
                cboTenMotP82S.Items.Clear()
                cboTenMotP82S.Items.AddRange(dataSourceString)
            End If
            cboTenMotP82S.SelectedIndex = _lineP82S 'DataSourceEnum.dsDirect
            txtTenMotP82V.Text = _lineP82V '"0"

            If (cboTenMotPolarity.Items.Count < 1) Or (IsNothing(cboTenMotPolarity.SelectedItem)) Then
                cboTenMotPolarity.Items.Clear()
                cboTenMotPolarity.Items.AddRange(polarityString)
            End If
            cboTenMotPolarity.SelectedIndex = _linePolarity '0

            If (cboTenMotP321S.Items.Count < 1) Or (IsNothing(cboTenMotP321S.SelectedItem)) Then
                cboTenMotP321S.Items.Clear()
                cboTenMotP321S.Items.AddRange(dataSourceString)
            End If
            cboTenMotP321S.SelectedIndex = _lineP321S 'DataSourceEnum.dsDirect
            txtTenMotP321V.Text = _lineP321V '"1000" 

            If (cboTenMotP322S.Items.Count < 1) Or IsNothing(cboTenMotP322S.SelectedItem) Then
                cboTenMotP322S.Items.Clear()
                cboTenMotP322S.Items.AddRange(dataSourceString)
            End If
            cboTenMotP322S.SelectedIndex = _lineP322S 'DataSourceEnum.dsDirect
            txtTenMotP322V.Text = _lineP322V '"0"

        ElseIf _lineCommand = CommandEnum.cmdGoTo Then
            txtGotoGotoTrue.Text = _lineGotoTrue '"0"

        ElseIf _lineCommand = CommandEnum.cmdDelay Then
            If (cboDelayP321S.Items.Count < 1) Or (IsNothing(cboDelayP321S.SelectedItem)) Then
                cboDelayP321S.Items.Clear()
                cboDelayP321S.Items.AddRange(dataSourceString)
            End If
            cboDelayP321S.SelectedIndex = _lineP321S 'DataSourceEnum.dsDirect
            txtDelayP321V.Text = _lineP321V '"100"

        ElseIf _lineCommand = CommandEnum.cmdTest Then
            If (cboTestP81S.Items.Count < 1) Or (IsNothing(cboTestP81S.SelectedItem)) Then
                cboTestP81S.Items.Clear()
                cboTestP81S.Items.AddRange(dataSourceString)
            End If
            cboTestP81S.SelectedIndex = _lineP81S 'DataSourceEnum.dsDirect
            txtTestP81V.Text = _lineP81V '"100"

            If (cboTestP82V.Items.Count < 1) Or (IsNothing(cboTestP82V.SelectedItem)) Then
                cboTestP82V.Items.Clear()
                cboTestP82V.Items.AddRange(testOperationString)
            End If
            cboTestP82V.SelectedIndex = _lineP82V 'DataSourceEnum.dsDirect

            If (cboTestP321S.Items.Count < 1) Or (IsNothing(cboTestP321S.SelectedItem)) Then
                cboTestP321S.Items.Clear()
                cboTestP321S.Items.AddRange(dataSourceString)
            End If
            cboTestP321S.SelectedIndex = _lineP321S 'DataSourceEnum.dsDirect
            txtTestP321V.Text = _lineP321V '"1000" 

            If (cboTestP322S.Items.Count < 1) Or IsNothing(cboTestP322S.SelectedItem) Then
                cboTestP322S.Items.Clear()
                cboTestP322S.Items.AddRange(dataSourceString)
            End If
            cboTestP322S.SelectedIndex = _lineP322S 'DataSourceEnum.dsDirect
            txtTestP322V.Text = _lineP322V '"0"

            txtTestGotoTrue.Text = _lineGotoTrue
            txtTestGotoFalse.Text = _lineGotoFalse

        ElseIf _lineCommand = CommandEnum.cmdSet Then
            If (cboSetP81S.Items.Count < 1) Or (IsNothing(cboSetP81S.SelectedItem)) Then
                cboSetP81S.Items.Clear()
                cboSetP81S.Items.AddRange(dataSourceString)
            End If
            cboSetP81S.SelectedIndex = _lineP81S 'DataSourceEnum.dsDirect
            txtSetP81V.Text = _lineP81V '"100"

            If (cboSetP321S.Items.Count < 1) Or (IsNothing(cboSetP321S.SelectedItem)) Then
                cboSetP321S.Items.Clear()
                cboSetP321S.Items.AddRange(dataSourceString)
            End If
            cboSetP321S.SelectedIndex = _lineP321S 'DataSourceEnum.dsDirect
            txtSetP321V.Text = _lineP321V '"1000" 

            If (cboSetP82S.Items.Count < 1) Or (IsNothing(cboSetP82S.SelectedItem)) Then
                cboSetP82S.Items.Clear()
                cboSetP82S.Items.AddRange(mathFunctionString)
            End If
            cboSetP82S.SelectedIndex = _lineP81V 'DataSourceEnum.dsDirect

            If (cboSetP322S.Items.Count < 1) Or IsNothing(cboSetP322S.SelectedItem) Then
                cboSetP322S.Items.Clear()
                cboSetP322S.Items.AddRange(dataSourceString)
            End If
            cboSetP322S.SelectedIndex = _lineP322S 'DataSourceEnum.dsDirect
            txtSetP322V.Text = _lineP322V '"0"



        ElseIf _lineCommand = CommandEnum.cmdProgControl Then
            If (cboProgOpP81S.Items.Count < 1) Or IsNothing(cboProgOpP81S.SelectedItem) Then
                cboProgOpP81S.Items.Clear()
                cboProgOpP81S.Items.AddRange(programOpTypeString)
            End If
            cboProgOpP81S.SelectedIndex = _lineP81V


            If (cboProgOpChannel.Items.Count < 1) Or IsNothing(cboProgOpChannel.SelectedItem) Then
                cboProgOpChannel.Items.Clear()
                cboProgOpChannel.Items.AddRange(allChannelsString)
            End If
            cboProgOpChannel.SelectedIndex = _lineChan

            If (cboProgOpP321S.Items.Count < 1) Or (IsNothing(cboProgOpP321S.SelectedItem)) Then
                cboProgOpP321S.Items.Clear()
                cboProgOpP321S.Items.AddRange(dataSourceString)
            End If
            cboProgOpP321S.SelectedIndex = _lineP321S 'DataSourceEnum.dsDirect
            txtProgOpP321V.Text = _lineP321V '"1000" 

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
        Command = cboCommand.SelectedIndex
    End Sub



    Private Sub cboTenMotP81S_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboTenMotP81S.SelectedIndexChanged
        _lineP81S = cboTenMotP81S.SelectedIndex
    End Sub

    Private Sub cboTenMotP82S_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboTenMotP82S.SelectedIndexChanged
        _lineP82S = cboTenMotP82S.SelectedIndex
    End Sub

    Private Sub cboTenMotPolarity_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboTenMotPolarity.SelectedIndexChanged
        _linePolarity = cboTenMotPolarity.SelectedIndex
    End Sub



    Private Sub cboTenMotP321S_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboTenMotP321S.SelectedIndexChanged
        _lineP321S = cboTenMotP321S.SelectedIndex
    End Sub

    Private Sub cboTenMotP322S_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboTenMotP322S.SelectedIndexChanged
        _lineP322S = cboTenMotP322S.SelectedIndex
    End Sub

    Private Sub txtTenMotP81V_TextChanged(sender As Object, e As EventArgs) Handles txtTenMotP81V.TextChanged
        If IsNumeric(txtTenMotP81V.Text) = False Then
            txtTenMotP81V.Text = _lineP81V
        Else
            _lineP81V = txtTenMotP81V.Text
        End If
    End Sub


    Private Sub txtTenMotP82V_TextChanged(sender As Object, e As EventArgs) Handles txtTenMotP82V.TextChanged
        If IsNumeric(txtTenMotP82V.Text) = False Then
            txtTenMotP82V.Text = _lineP82V
        Else
            _lineP82V = txtTenMotP82V.Text
        End If
    End Sub

    Private Sub txtTenMotP321V_TextChanged(sender As Object, e As EventArgs) Handles txtTenMotP321V.TextChanged
        If IsNumeric(txtTenMotP321V.Text) = False Then
            txtTenMotP321V.Text = _lineP321V
        Else
            _lineP321V = txtTenMotP321V.Text
        End If
    End Sub

    Private Sub txtTenMotP322V_TextChanged(sender As Object, e As EventArgs) Handles txtTenMotP322V.TextChanged
        If IsNumeric(txtTenMotP322V.Text) = False Then
            txtTenMotP322V.Text = _lineP322V
        Else
            _lineP322V = txtTenMotP322V.Text
        End If
    End Sub





    Private Sub UserControlProgLine_Leave(sender As Object, e As EventArgs) Handles Me.Leave
        RaiseEvent ProgLineLeaveWithLineNumber(_LineNum)
    End Sub

    Private Sub txtGotoGotoTrue_TextChanged(sender As Object, e As EventArgs) Handles txtGotoGotoTrue.TextChanged
        If IsNumeric(txtGotoGotoTrue.Text) = False Then
            txtGotoGotoTrue.Text = _lineGotoTrue
        Else
            _lineGotoTrue = txtGotoGotoTrue.Text
        End If
    End Sub

    Private Sub cboProgOpP81S_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboProgOpP81S.SelectedIndexChanged
        _lineP81V = cboProgOpP81S.SelectedIndex
        If _lineP81V = OpTypeEnum.opLoadProgram Then
            lblProgOpLineSource.Visible = True
            If Not IsNothing(cboProgOpP321S) Then
                If IsNothing(cboProgOpP321S.SelectedIndex) Or (cboProgOpP321S.Items.Count < 1) Then
                    cboProgOpP321S.Items.Clear()
                    cboProgOpP321S.Items.AddRange(dataSourceString)
                End If
            End If
            cboProgOpP321S.Visible = True
            cboProgOpP321S.SelectedIndex = _lineP321S
            txtProgOpP321V.Visible = True
            txtProgOpP321V.Text = _lineP321V
        Else
            lblProgOpLineSource.Visible = False
            cboProgOpP321S.Visible = False
            txtProgOpP321V.Visible = False
        End If
    End Sub

    Private Sub txtProgOpP321V_TextChanged(sender As Object, e As EventArgs) Handles txtProgOpP321V.TextChanged
        If IsNumeric(txtProgOpP321V.Text) = False Then
            txtProgOpP321V.Text = _lineP321V
        Else
            _lineP321V = txtProgOpP321V.Text
        End If
    End Sub

    Private Sub cboDelayP321S_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboDelayP321S.SelectedIndexChanged
        _lineP321S = cboDelayP321S.SelectedIndex
    End Sub

    Private Sub txtDelayP321V_TextChanged(sender As Object, e As EventArgs) Handles txtDelayP321V.TextChanged
        If IsNumeric(txtDelayP321V.Text) = False Then
            txtDelayP321V.Text = _lineP321V
        Else
            _lineP321V = txtDelayP321V.Text
        End If
    End Sub

    Private Sub cboTestP81S_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboTestP81S.SelectedIndexChanged
        _lineP81S = cboTestP81S.SelectedIndex
    End Sub

    Private Sub txtTestP81V_TextChanged(sender As Object, e As EventArgs) Handles txtTestP81V.TextChanged
        If IsNumeric(txtTestP81V.Text) = False Then
            txtTestP81V.Text = _lineP81V
        Else
            _lineP81V = txtTestP81V.Text
        End If
    End Sub

    Private Sub cboTestP82V_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboTestP82V.SelectedIndexChanged
        _lineP82V = cboTestP82V.SelectedIndex
        If (cboTestP82V.SelectedIndex = 4) Or (cboTestP82V.SelectedIndex = 5) Or (cboTestP82V.SelectedIndex = 6) Then
            lblTestP322S.Visible = True

            If Not IsNothing(cboTestP322S) Then
                If IsNothing(cboTestP322S.SelectedIndex) Or (cboTestP322S.Items.Count < 1) Then
                    cboTestP322S.Items.Clear()
                    cboTestP322S.Items.AddRange(dataSourceString)
                End If
            End If
            cboTestP322S.SelectedIndex = _lineP322S
            cboTestP322S.Visible = True
            txtTestP322V.Text = _lineP322V
            txtTestP322V.Visible = True
            lblTestThen.Top = 14
        Else
            lblTestP322S.Visible = False
            cboTestP322S.Visible = False
            txtTestP322V.Visible = False
            lblTestThen.Top = 5

        End If
    End Sub

    Private Sub cboTestP321S_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboTestP321S.SelectedIndexChanged
        _lineP321S = cboTestP321S.SelectedIndex
    End Sub

    Private Sub cboTestP322S_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboTestP322S.SelectedIndexChanged
        _lineP322S = cboTestP322S.SelectedIndex
    End Sub

    Private Sub txtTestP321V_TextChanged(sender As Object, e As EventArgs) Handles txtTestP321V.TextChanged
        If IsNumeric(txtTestP321V.Text) = False Then
            txtTestP321V.Text = _lineP321V
        Else
            _lineP321V = txtTestP321V.Text
        End If
    End Sub

    Private Sub txtTestP322V_TextChanged(sender As Object, e As EventArgs) Handles txtTestP322V.TextChanged
        If IsNumeric(txtTestP322V.Text) = False Then
            txtTestP322V.Text = _lineP322V
        Else
            _lineP322V = txtTestP322V.Text
        End If
    End Sub

    Private Sub txtTestGotoTrue_TextChanged(sender As Object, e As EventArgs) Handles txtTestGotoTrue.TextChanged
        If IsNumeric(txtTestGotoTrue.Text) = False Then
            txtTestGotoTrue.Text = _lineGotoTrue
        Else
            _lineGotoTrue = txtTestGotoTrue.Text
        End If
    End Sub

    Private Sub txtTestGotoFalse_TextChanged(sender As Object, e As EventArgs) Handles txtTestGotoFalse.TextChanged
        If IsNumeric(txtTestGotoFalse.Text) = False Then
            txtTestGotoFalse.Text = _lineGotoFalse
        Else
            _lineGotoFalse = txtTestGotoFalse.Text
        End If
    End Sub

    Private Sub cboSetP81S_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboSetP81S.SelectedIndexChanged
        _lineP81S = cboSetP81S.SelectedIndex
    End Sub

    Private Sub txtSetP81V_TextChanged(sender As Object, e As EventArgs) Handles txtSetP81V.TextChanged
        If IsNumeric(txtSetP81V.Text) = False Then
            txtSetP81V.Text = _lineP81V
        Else
            _lineP81V = txtSetP81V.Text
        End If
    End Sub

    Private Sub cboSetP321S_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboSetP321S.SelectedIndexChanged
        _lineP321S = cboSetP321S.SelectedIndex
    End Sub

    Private Sub cboSetP82S_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboSetP82S.SelectedIndexChanged
        _lineP82S = cboSetP82S.SelectedIndex
        If (cboSetP82S.SelectedIndex <> 0) Then

            If Not IsNothing(cboSetP322S) Then
                If IsNothing(cboSetP322S.SelectedIndex) Or (cboSetP322S.Items.Count < 1) Then
                    cboSetP322S.Items.Clear()
                    cboSetP322S.Items.AddRange(dataSourceString)
                End If
            End If
            cboSetP322S.SelectedIndex = _lineP322S
            cboSetP322S.Visible = True
            txtSetP322V.Text = _lineP322V
            txtSetP322V.Visible = True
        Else
            cboSetP322S.Visible = False
            txtSetP322V.Visible = False
        End If
    End Sub

    Private Sub cboSetP322S_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboSetP322S.SelectedIndexChanged
        _lineP322S = cboSetP322S.SelectedIndex
    End Sub

    Private Sub txtSetP321V_TextChanged(sender As Object, e As EventArgs) Handles txtSetP321V.TextChanged
        If IsNumeric(txtSetP321V.Text) = False Then
            txtSetP321V.Text = _lineP321V
        Else
            _lineP321V = txtSetP321V.Text
        End If
    End Sub

    Private Sub txtSetP322V_TextChanged(sender As Object, e As EventArgs) Handles txtSetP322V.TextChanged
        If IsNumeric(txtSetP322V.Text) = False Then
            txtSetP322V.Text = _lineP322V
        Else
            _lineP322V = txtSetP322V.Text
        End If
    End Sub
End Class

