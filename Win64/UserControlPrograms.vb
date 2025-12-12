Imports System.Reflection

Public Class UserControlPrograms

    Dim _rows As New List(Of UserControlProgLine2)

    Dim _NumMotors = 0
    Dim _NumTens = 0
    Dim _NumSys = 0

    Dim _ChanType As enumChantype


    Dim _activeRow As Integer = -1 'indicates the last row that received user's focus

    Public Event ActiveRowChanged(ByVal NewActiveIndex As Integer, ByVal LastActiveIndex As Integer)
    Public Event ProgramRow_Leave(ByVal LineNum As Integer)
    Public Event reqAddRowToEnd()
    Public Event reqDeleteRow(ByVal LineNum As Integer)
    Public Event reqInsertRow(ByVal LineNum As Integer)
    Public Event reqMoveRowUp(ByVal LineNum As Integer)
    Public Event reqMoveRowDown(ByVal LineNum As Integer)


    ReadOnly Property Rows(ByVal Index As Integer) As UserControlProgLine2
        Get
            If (Index >= 0) And (Index < _rows.Count) Then
                Return _rows.Item(Index)
            Else
                Return Nothing
            End If
        End Get
    End Property
    ReadOnly Property RowCount As Integer
        Get
            'Return _rowCount
            Return _rows.Count
        End Get
    End Property

    Public Property NumMotorChannels As Integer
        Get
            Return _NumMotors
        End Get
        Set(value As Integer)
            _NumMotors = value
        End Set
    End Property
    Public Property NumTensChannels As Integer
        Get
            Return _NumTens
        End Get
        Set(value As Integer)
            _NumTens = value
        End Set
    End Property
    Public Property NumSysChannels As Integer
        Get
            Return _NumSys
        End Get
        Set(value As Integer)
            _NumSys = value
        End Set
    End Property

    Private Sub UserControlPrograms_Load(sender As Object, e As EventArgs) Handles Me.Load
        For Each tmpLine As UserControlProgLine2 In pnlProg.Controls
            If TypeOf tmpLine Is UserControlProgLine2 Then
                tmpLine.Dispose()
            End If
        Next
    End Sub

    Public Property ActiveRow As Integer
        Get
            Return _activeRow
        End Get
        Set(value As Integer)
            If value < _rows.Count - 1 Then
                '_activeRow = value
                ProgLine_GotFocus(value)
            End If
        End Set
    End Property

    Public Sub Clear()
        For n As Integer = 0 To _rows.Count - 1
            _rows.Item(n).Dispose()
        Next
        _rows.Clear()
        'For Each tmpProgLine As UserControlProgLine In pnlProg.Controls
        '    If TypeOf tmpProgLine Is UserControlProgLine Then
        '        pnlProg.Controls.Remove(tmpProgLine)
        '    End If
        'Next
    End Sub

    Public Function AddRow(ByVal ParamArray vals() As String) As Integer
        Dim newIndex As Integer = _rows.Count
        If vals.Length < 1 Then Return -1

        'find the y location of the lowest control:
        Dim yval = 0
        If newIndex > 0 Then
            Dim tmpPrevRow As UserControlProgLine2 = _rows.Item(newIndex - 1)
            yval = tmpPrevRow.Top + tmpPrevRow.Size.Height - 1
        End If

        Dim ucplTemp As UserControlProgLine2 = CreateNewProgLine(vals)
        ucplTemp.UnsavedChanges = False
        ucplTemp.Name = "Line" & vals(0)
        ucplTemp.ContextMenuStrip = contextProgLineList
        ucplTemp.Location = New Point(0, yval)
        _rows.Add(ucplTemp)
        pnlProg.Controls.Add(ucplTemp)
        Return _rows.Count
    End Function

    Private Function CreateNewProgLine(ByVal ParamArray vals() As String) As UserControlProgLine2
        'Create a new row(progLine), add return it
        'Dim tmpProgLine As UserControlProgLine = New UserControlProgLine With {.NumMotorChans = _NumMotors, .NumTensChans = _NumTens,
        '        .NumSysChans = _NumSys, .LineNum = vals(0), .Command = vals(1), .ChannelNum = vals(2),
        '        .GoToTrue = vals(3), .GoToFalse = vals(4), .P81S = vals(5), .P81V = vals(6), .P82S = vals(7),
        '        .P82V = vals(8), .Polarity = vals(9), .P321S = vals(10), .P321V = vals(11),
        '        .P322S = vals(12), .P322V = vals(13)}
        Dim tmpProgLine As UserControlProgLine2 = New UserControlProgLine2 With {
                .LineNum = vals(0), .Command = vals(1), .ChannelNum = vals(2), .GoToTrue = vals(3),
                .GoToFalse = vals(4), .P81S = vals(5), .P81V = vals(6), .P82S = vals(7), .P82V = vals(8),
                .Polarity = vals(9), .P321S = vals(10), .P321V = vals(11), .P322S = vals(12), .P322V = vals(13)}
        AddHandler tmpProgLine.ProgLineSizeChanged, AddressOf ProgLineSize_Changed
        AddHandler tmpProgLine.ProgLineGotFocus, AddressOf ProgLine_GotFocus
        AddHandler tmpProgLine.ProgLineLeaveWithLineNumber, AddressOf ProgLine_LeaveWithLineNumber
        Return tmpProgLine
    End Function

    Private Sub ResetYPositions(ByVal startingIndex As Integer, Optional ByVal lineNumAdjust As Integer = 0)
        'recalculate and reset the y positions of each row, starting with startingIndex
        'and continue to the end of the list
        'LineNumAdjust can be used to update the line number of the program line.  The value 
        'of "lineNumAdjust" will be added to each line control's lineNum.  So a lineNumAdjust value
        'of 1 will increment the line number and a value of -1 will dec.
        If (startingIndex < 0) Or (startingIndex >= _rows.Count) Then Exit Sub

        Dim newY As Integer = 0
        If startingIndex > 0 Then
            Dim tmpPrevRow As UserControlProgLine2 = _rows.Item(startingIndex - 1)
            newY = tmpPrevRow.Top + tmpPrevRow.Size.Height - 1
        End If

        For n As Integer = startingIndex To _rows.Count - 1
            _rows(n).Location = New Point(0, newY)
            _rows(n).LineNum += lineNumAdjust
            newY += _rows(n).Size.Height - 1
        Next

    End Sub

    Public Function DeleteRow(ByVal Index As Integer) As Boolean
        If (Index >= 0) And (Index < _rows.Count) Then
            pnlProg.Controls.Remove(_rows.Item(Index))
            _rows.RemoveAt(Index)
            ResetYPositions(Index, -1)
            Return True
        End If
        Return False
    End Function

    Public Function InsertRow(ByVal Index As Integer, ByVal ParamArray vals() As String) As Boolean
        'create a new row
        Dim ucplTemp As UserControlProgLine2 = CreateNewProgLine(vals)
        'insert the row
        Return InsertRow(Index, ucplTemp)
    End Function
    Public Function InsertRow(ByVal Index As Integer, ByRef ucProgLine As UserControlProgLine2) As Boolean
        If Index < 0 Then Return False
        If Index >= _rows.Count Then Index = _rows.Count - 1

        'find the y location of the control currently in this index position:
        Dim yval = _rows(Index).Location.Y

        ucProgLine.LineNum = Index
        ucProgLine.Name = "InsertedAt" & Index
        ucProgLine.Location = New Point(0, yval)

        'insert the new row into the list
        _rows.Insert(Index, ucProgLine)

        'add the new row to the controls panel
        pnlProg.Controls.Add(ucProgLine)

        'adjust the y position AND lineNumbers of the rows after the inserted row
        ResetYPositions(Index + 1, 1)

        'Unhighlight the original row and highlight the inserted row
        _rows(Index + 1).HighlightRow = False
        _rows(Index).HighlightRow = True

        Return True
    End Function


    Private Sub ProgLineSize_Changed(ByVal lineNum As Integer, ByVal yPos As Integer, ByVal newHeight As Integer)
        ResetYPositions(lineNum + 1, 0)
    End Sub
    Private Sub ProgLine_GotFocus(ByVal lineNum As Integer)
        Dim tmpLastActive As Integer = _activeRow
        _activeRow = lineNum
        'Unhighlight the now inactive row
        If (tmpLastActive >= 0) And (tmpLastActive < _rows.Count) Then
            _rows.Item(tmpLastActive).HighlightRow(False) = False
        End If

        'Highlight the new active row
        If (_activeRow >= 0) And (_activeRow < _rows.Count) Then
            _rows.Item(_activeRow).HighlightRow(True) = True
        End If
        RaiseEvent ActiveRowChanged(_activeRow, tmpLastActive)
    End Sub



    Private Sub ProgLine_LeaveWithLineNumber(ByVal lineNum As Integer)
        RaiseEvent ProgramRow_Leave(lineNum)
    End Sub

    Private Sub tsAddRow_Click(sender As Object, e As EventArgs) Handles tsAddRow.Click
        'AddRow(_rows.Count, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0) 'NoOp
        RaiseEvent reqAddRowToEnd()
    End Sub

    Private Sub tsInsertRow_Click(sender As Object, e As EventArgs) Handles tsInsertRow.Click
        'InsertRow(_activeRow, noOpParamArray)
        RaiseEvent reqInsertRow(_activeRow)
    End Sub

    Private Sub tsDeleteRow_Click(sender As Object, e As EventArgs) Handles tsDeleteRow.Click
        'DeleteRow(_activeRow)
        RaiseEvent reqDeleteRow(_activeRow)
    End Sub

    Private Sub tsMoveRowUp_Click(sender As Object, e As EventArgs) Handles tsMoveRowUp.Click
        RaiseEvent reqMoveRowUp(_activeRow)
    End Sub
    Public Sub MoveActiveRowUp()
        If _activeRow < 1 Then Exit Sub
        Dim baseRowNum As Integer = _activeRow
        'get a reference to the progLine item that needs to be moved up and then remove it from the list
        Dim rowToMoveUp As UserControlProgLine2 = _rows.Item(baseRowNum)
        _rows.RemoveAt(baseRowNum)

        'find the y location of the control currently in this index position:
        Dim yval = _rows(baseRowNum - 1).Location.Y
        rowToMoveUp.LineNum = baseRowNum - 1
        rowToMoveUp.Name = "MovedUpTo" & baseRowNum - 1
        rowToMoveUp.Location = New Point(0, yval)

        'insert the new row into the list
        _rows.Insert(baseRowNum - 1, rowToMoveUp)

        'adjust the y position and lineNumber of the single row that was moved down
        _rows.Item(baseRowNum).LineNum = baseRowNum
        _rows(baseRowNum).Location = New Point(0, rowToMoveUp.Top + rowToMoveUp.Height - 1)

        'Leave the (moved-up) cell highlighted, but update the _activeRow to match the row's new location
        _activeRow -= 1
    End Sub


    Private Sub tsMoveRowDown_Click(sender As Object, e As EventArgs) Handles tsMoveRowDown.Click
        RaiseEvent reqMoveRowDown(_activeRow)
    End Sub
    Public Sub MoveActiveRowDown()
        If _activeRow >= _rows.Count - 1 Then Exit Sub
        'Note: In order to keep the mechanism of this function almost identical to tsMoveRowUp,
        ' instead of moving *this* row down, we're moving the *next* row up.  So we're doing the same
        ' as we did in tsMoveRowUp but just starting 1 index later in the list.
        Dim baseRowNum As Integer = _activeRow + 1
        'get a reference to the progLine item that needs to be moved up and then remove it from the list
        Dim rowToMoveUp As UserControlProgLine2 = _rows.Item(baseRowNum)
        _rows.RemoveAt(baseRowNum)

        'find the y location of the control currently in this index position:
        Dim yval = _rows(baseRowNum - 1).Location.Y
        rowToMoveUp.LineNum = baseRowNum - 1
        rowToMoveUp.Name = "MovedUpTo" & baseRowNum - 1
        rowToMoveUp.Location = New Point(0, yval)

        'insert the new row into the list
        _rows.Insert(baseRowNum - 1, rowToMoveUp)

        'adjust the y position and lineNumber of the single row that was moved down
        _rows.Item(baseRowNum).LineNum = baseRowNum
        _rows(baseRowNum).Location = New Point(0, rowToMoveUp.Top + rowToMoveUp.Height - 1)

        'Leave the (moved-down) cell highlighted, but update the _activeRow to match the row's new location
        _activeRow += 1
    End Sub
End Class
