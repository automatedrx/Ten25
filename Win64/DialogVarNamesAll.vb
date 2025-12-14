Imports System.Net
Imports System.Reflection
Imports System.Windows.Forms

Public Class DialogVarNamesAll

    Public Enum NameList_t As Integer
        VarNames = 0
        TimNames
    End Enum

    Dim _progNum As Integer = -1
    Dim _progRef As List(Of Program)
    'Dim _listRef As List(Of String)
    'Dim _numVars As Integer

    Dim _numItems As Integer
    Dim _curListType As NameList_t
    Dim _LabelAbbrev As String = "" '"Var " or "Tim "
    Dim _formDataHasChanged As Boolean = False

    Dim TitleEditVarNames As String = "Edit Program Variable Names"
    Dim TitleEditTimNames As String = "Edit Program Timer Names"
    'Dim _WindowTitle As String = "Edit Names"

    'Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
    '    'Save the var/timer names
    '    Dim tmpCtl As TextBox
    '    For n As Integer = 0 To _numVars - 1

    '        tmpCtl = Me.Panel1.Controls.Item("txt" & n)
    '        Dim tmpName As String = "Var " & n
    '        If tmpCtl.Text.Length > 0 Then
    '            tmpName = New String(tmpCtl.Text)
    '        End If

    '        _progRef(_progNum).varName(n) = tmpName
    '    Next

    '    Me.DialogResult = System.Windows.Forms.DialogResult.OK
    '    Me.Close()
    'End Sub
    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        SaveFormDataToList()

        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub SaveFormDataToList()
        'Save the var/timer names
        Dim tmpCtl As TextBox
        Dim tmpNameList As New List(Of String)
        For n As Integer = 0 To _numItems - 1

            tmpCtl = Me.Panel1.Controls.Item("txt" & n)
            Dim tmpName As String = _LabelAbbrev & n
            If tmpCtl.Text.Length > 0 Then
                'tmpName = New String(tmpCtl.Text)
                tmpName = tmpCtl.Text
            End If
            tmpNameList.Add(tmpName)
        Next

        Dim tmpProg As Program = _progRef(_progNum)
        If _curListType = NameList_t.VarNames Then
            tmpProg.varName = tmpNameList
        ElseIf _curListType = NameList_t.TimNames Then
            tmpProg.timerName = tmpNameList
        Else
            Exit Sub
        End If
        _progRef(_progNum) = tmpProg

        _formDataHasChanged = False
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub


    'Public Function ShowVarEditDialog(ProgRef As Object, ProgNum As Integer, VariableCount As Integer) As MsgBoxResult
    '    _progRef = ProgRef
    '    _progNum = ProgNum
    '    _numVars = VariableCount

    '    If (_progNum < 0) Or (_progNum >= _progRef.Count) Then Return MsgBoxResult.Cancel
    '    If _numVars > _progRef(_progNum).varName.Count Then _numVars = _progRef(_progNum).varName.Count

    '    Dim lblLeft As Integer = Label1.Left
    '    Dim lblTop As Integer = Label1.Top
    '    Dim lblW As Integer = Label1.Width
    '    Dim lblH As Integer = Label1.Height

    '    Dim txtLeft As Integer = TextBox1.Left
    '    Dim txtTop As Integer = TextBox1.Top
    '    Dim txtW As Integer = TextBox1.Width
    '    Dim txtH As Integer = TextBox1.Height

    '    Dim Inc As Integer = 26

    '    Panel1.SuspendLayout()

    '    Dim ctlList(0) As String
    '    Dim listCount As Integer = 0
    '    For Each tmpCtl As Control In Me.Panel1.Controls
    '        If (tmpCtl.Name.StartsWith("txt")) Or (tmpCtl.Name.StartsWith("lbl")) Then
    '            ReDim Preserve ctlList(0 To listCount)
    '            ctlList(listCount) = tmpCtl.Name
    '            listCount += 1
    '        End If
    '    Next

    '    For x As Integer = 0 To listCount - 1
    '        Me.Panel1.Controls.RemoveByKey(ctlList(x))
    '    Next

    '    'Create new labels and textboxes for each variable:
    '    For n As Integer = 0 To _numVars - 1
    '        Dim tmpLbl As New Label
    '        tmpLbl.Name = "lbl" & n
    '        tmpLbl.AutoSize = False
    '        tmpLbl.Width = lblW
    '        tmpLbl.Height = lblH
    '        tmpLbl.Left = lblLeft
    '        tmpLbl.Top = lblTop + (Inc * n)
    '        tmpLbl.Text = n
    '        Panel1.Controls.Add(tmpLbl)

    '        Dim tmpTxt As New TextBox
    '        tmpTxt.Name = "txt" & n
    '        tmpTxt.Width = txtW
    '        tmpTxt.Height = txtH
    '        tmpTxt.Left = txtLeft
    '        tmpTxt.Top = txtTop + (Inc * n)
    '        tmpTxt.Text = _progRef(_progNum).varName(n)
    '        Panel1.Controls.Add(tmpTxt)
    '    Next

    '    Panel1.ResumeLayout()
    '    Return Me.ShowDialog()
    'End Function
    Public Function ShowEditDialog(ByRef ProgRef As Object, ProgNum As Integer, ListType As NameList_t) As MsgBoxResult
        _progRef = ProgRef
        _progNum = ProgNum
        _curListType = ListType

        If (_progNum < 0) Or (_progNum >= _progRef.Count) Then Return MsgBoxResult.Cancel

        RemoveHandler cboProgNum.SelectedIndexChanged, AddressOf cboProgNum_SelectedIndexChanged
        cboProgNum.Items.Clear()
        For n As Integer = 0 To _progRef.Count - 1
            Dim tmpProgName As String = "(" & n & ") " & _progRef(n).progName
            cboProgNum.Items.Add(tmpProgName)
        Next
        AddHandler cboProgNum.SelectedIndexChanged, AddressOf cboProgNum_SelectedIndexChanged

        cboProgNum.SelectedIndex = _progNum
        Select Case ListType
            Case NameList_t.VarNames
                rdoVars.Checked = True
            Case NameList_t.TimNames
                rdoTimers.Checked = True
        End Select

        Return Me.ShowDialog()
    End Function

    Private Sub LoadFormData()
        If _curListType = NameList_t.VarNames Then
            _numItems = _progRef(_progNum).varName.Count
            _LabelAbbrev = "Var "
            Me.Text = TitleEditVarNames
        ElseIf _curListType = NameList_t.TimNames Then
            _numItems = _progRef(_progNum).timerName.Count
            _LabelAbbrev = "Tim "
            Me.Text = TitleEditTimNames
        Else
            Exit Sub
        End If

        Dim lblLeft As Integer = Label1.Left
        Dim lblTop As Integer = Label1.Top
        Dim lblW As Integer = Label1.Width
        Dim lblH As Integer = Label1.Height

        Dim txtLeft As Integer = TextBox1.Left
        Dim txtTop As Integer = TextBox1.Top
        Dim txtW As Integer = TextBox1.Width
        Dim txtH As Integer = TextBox1.Height

        Dim Inc As Integer = 26

        Panel1.SuspendLayout()

        Dim ctlList(0) As String
        Dim listCount As Integer = 0
        For Each tmpCtl As Control In Me.Panel1.Controls
            If (tmpCtl.Name.StartsWith("txt")) Or (tmpCtl.Name.StartsWith("lbl")) Then
                ReDim Preserve ctlList(0 To listCount)
                ctlList(listCount) = tmpCtl.Name
                If tmpCtl.Name.StartsWith("txt") Then
                    RemoveHandler DirectCast(tmpCtl, TextBox).TextChanged, AddressOf TextBox_TextChanged
                End If
                listCount += 1
                End If
        Next

        For x As Integer = 0 To listCount - 1
            Me.Panel1.Controls.RemoveByKey(ctlList(x))
        Next

        Dim tmpNameList As List(Of String)
        If _curListType = NameList_t.VarNames Then
            tmpNameList = _progRef(_progNum).varName
        ElseIf _curListType = NameList_t.TimNames Then
            tmpNameList = _progRef(_progNum).timerName
        Else
            Exit Sub
        End If

        'Create new labels and textboxes for each variable:
        For n As Integer = 0 To _numItems - 1
            Dim tmpLbl As New Label
            tmpLbl.Name = "lbl" & n
            tmpLbl.AutoSize = False
            tmpLbl.Width = lblW
            tmpLbl.Height = lblH
            tmpLbl.Left = lblLeft
            tmpLbl.Top = lblTop + (Inc * n)
            tmpLbl.Text = n
            Panel1.Controls.Add(tmpLbl)

            Dim tmpTxt As New TextBox
            tmpTxt.Name = "txt" & n
            tmpTxt.Width = txtW
            tmpTxt.Height = txtH
            tmpTxt.Left = txtLeft
            tmpTxt.Top = txtTop + (Inc * n)
            'tmpTxt.Text = _progRef(_progNum).varName(n)
            tmpTxt.Text = tmpNameList(n)
            Panel1.Controls.Add(tmpTxt)
            AddHandler tmpTxt.TextChanged, AddressOf TextBox_TextChanged
        Next

        Panel1.ResumeLayout()

    End Sub

    Private Sub cboProgNum_SelectedIndexChanged(sender As Object, e As EventArgs)
        If _formDataHasChanged Then
            SaveFormDataToList()
        End If

        _progNum = cboProgNum.SelectedIndex
        LoadFormData()
    End Sub

    Private Sub TextBox_TextChanged(sender As Object, e As EventArgs)
        _formDataHasChanged = True
    End Sub

    Private Sub rdoVars_CheckedChanged(sender As Object, e As EventArgs) Handles rdoVars.CheckedChanged
        If _formDataHasChanged Then
            SaveFormDataToList()
        End If

        Me.Text = TitleEditVarNames
        _curListType = NameList_t.VarNames
        LoadFormData()
    End Sub

    Private Sub rdoTimers_CheckedChanged(sender As Object, e As EventArgs) Handles rdoTimers.CheckedChanged
        If _formDataHasChanged Then
            SaveFormDataToList()
        End If

        Me.Text = TitleEditTimNames
        _curListType = NameList_t.TimNames
        LoadFormData()
    End Sub
End Class
