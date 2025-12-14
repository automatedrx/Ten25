Imports System.ComponentModel
Imports System.Net

Public Class ucValueControl

    'Dim _progRef As List(Of Program)
    Dim devRef As Device_t
    Dim _curProgNum As Integer

    Dim _curChanNumForVarNames = -1


    Dim controlPadding = 3

    Dim srcIndex As DataSourceEnum = DataSourceEnum.dsDirect
    Dim _val1 As Integer = 0
    Dim _val2 As Integer = 0
    Dim _maxVal As Integer = 255

    Dim _progVarCount As Integer = -1
    Dim _progTimerCount As Integer = -1
    'Dim _sysVarCount As Integer = -1
    Dim _digInCount As Integer = -1
    Dim _digOutCount As Integer = -1
    Dim _chanSettingCount As Integer = dataSourceChanSettingString.Length
    Dim _sysSettingCount As Integer = dataSourceSysSettingString.Length

    Dim strSourceLabel As String = ""
    Dim strUnitsLabel As String = ""

    Dim _lastSrcIndexForCboVal = -1

    Public Event Source_Changed(sender As Object, newSourceIndex As Integer)
    Public Event Val_Changed(sender As Object, newVal As Integer)
    Public Event Val2_Changed(sender As Object, newVal As Integer)



    '<DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    'Public WriteOnly Property ProgRef As Object
    '    Set(value As Object)
    '        'This should be set to a reference of prog(list of Program) from form1.
    '        _progRef = value
    '    End Set
    'End Property
    Friend Sub SetDeviceRef(ByRef Dev As Device_t)
        devRef = Dev
    End Sub

    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property ProgNum As Integer
        Get
            Return _curProgNum
        End Get
        Set(value As Integer)
            _curProgNum = value
        End Set
    End Property
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property ProgVarCount As Integer
        Get
            Return _progVarCount
        End Get
        Set(value As Integer)
            _progVarCount = value
        End Set
    End Property

    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property ProgTimerCount As Integer
        Get
            Return _progTimerCount
        End Get
        Set(value As Integer)
            _progTimerCount = value
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

    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public ReadOnly Property SourceItems As ComboBox.ObjectCollection
        Get
            Return cboSource.Items
        End Get
    End Property

    Public ReadOnly Property Val1Items As ComboBox.ObjectCollection
        Get
            Return cboVal1.Items
        End Get
    End Property

    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property SourceSelectedIndex As Integer
        Get
            Return cboSource.SelectedIndex
        End Get
        Set(value As Integer)
            cboSource.SelectedIndex = value
        End Set
    End Property

    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property SourceSelectedItem As Object
        Get
            Return cboSource.SelectedItem
        End Get
        Set(value As Object)
            cboSource.SelectedItem = value
        End Set
    End Property

    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property Val1 As Integer
        Get
            Return _val1
        End Get
        Set(value As Integer)
            _val1 = value
            RemoveHandler cboVal1.SelectedIndexChanged, AddressOf cboVal1_SelectedIndexChanged
            If _val1 < cboVal1.Items.Count Then
                cboVal1.SelectedIndex = _val1
            Else
                cboVal1.Text = ""
            End If
            AddHandler cboVal1.SelectedIndexChanged, AddressOf cboVal1_SelectedIndexChanged
            RemoveHandler txtVal1.TextChanged, AddressOf txtVal1_TextChanged
            txtVal1.Text = _val1
            AddHandler txtVal1.TextChanged, AddressOf txtVal1_TextChanged

        End Set
    End Property

    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property Val2 As Integer
        Get
            Return _val2
        End Get
        Set(value As Integer)
            _val2 = value
            RemoveHandler txtVal2.TextChanged, AddressOf txtVal2_TextChanged
            txtVal2.Text = _val2
            AddHandler txtVal2.TextChanged, AddressOf txtVal2_TextChanged
        End Set
    End Property

    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property MaxVal As Integer
        Get
            Return _maxVal
        End Get
        Set(value As Integer)
            _maxVal = value
            Dim tmpStr As String = value.ToString
            Dim maxDigits = tmpStr.Length
            Dim txtWidth = 15 + ((maxDigits - 1) * 6)
            txtVal1.Width = txtWidth
            txtVal2.Width = txtWidth

            lblMax.Left = txtVal1.Left + txtVal1.Width + 3
            txtVal2.Left = lblMax.Left + 33
            lblUnits.Left = txtVal1.Left + txtVal1.Width + 3
            cboChannel.Left = txtVal2.Left
        End Set
    End Property

    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property SourceLabel As String
        Get
            Return strSourceLabel
        End Get
        Set(value As String)
            strSourceLabel = value
            If (lblSource.Text = "") Or (lblSource.Text <> value) Then lblSource.Text = value
        End Set
    End Property

    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property UnitsLabel As String
        Get
            Return strUnitsLabel
        End Get
        Set(value As String)
            strUnitsLabel = value
            If (lblUnits.Text = "") Or (lblUnits.Text <> value) Then lblUnits.Text = value
        End Set
    End Property

    Public Sub Clear()
        RemoveHandler cboSource.SelectedIndexChanged, AddressOf cboSource_SelectedIndexChanged
        cboSource.SelectedIndex = -1
        cboSource.Text = ""
        AddHandler cboSource.SelectedIndexChanged, AddressOf cboSource_SelectedIndexChanged

        RemoveHandler cboVal1.SelectedIndexChanged, AddressOf cboVal1_SelectedIndexChanged
        cboVal1.Visible = False
        cboVal1.SelectedIndex = -1
        cboVal1.Text = ""
        AddHandler cboVal1.SelectedIndexChanged, AddressOf cboVal1_SelectedIndexChanged

        RemoveHandler cboChannel.SelectedIndexChanged, AddressOf cboChannel_selectedIndexChanged
        cboChannel.Visible = False
        cboChannel.SelectedIndex = -1
        cboChannel.Text = ""

        lblMin.Visible = False
        txtVal1.Visible = False
        RemoveHandler txtVal1.TextChanged, AddressOf txtVal1_TextChanged
        txtVal1.Text = ""

        lblMax.Visible = False
        txtVal2.Visible = False
        RemoveHandler txtVal2.TextChanged, AddressOf txtVal2_TextChanged
        txtVal2.Text = ""

        lblUnits.Visible = False
        cmdEditVarName.Visible = False

    End Sub

    Private Sub ucValueControl_Load(sender As Object, e As EventArgs) Handles Me.Load
        cboSource.Visible = True
        lblMin.Visible = False
        cboVal1.Visible = False
        txtVal1.Visible = False
        lblMax.Visible = False
        txtVal2.Visible = False
        lblUnits.Visible = False

        AddHandler cboSource.SelectedIndexChanged, AddressOf cboSource_SelectedIndexChanged
    End Sub

    Private Sub cboSource_SelectedIndexChanged(sender As Object, e As EventArgs) 'Handles cboSource.SelectedIndexChanged
        srcIndex = cboSource.SelectedIndex

        RemoveHandler cboVal1.SelectedIndexChanged, AddressOf cboVal1_SelectedIndexChanged
        RemoveHandler txtVal1.TextChanged, AddressOf txtVal1_TextChanged
        RemoveHandler txtVal2.TextChanged, AddressOf txtVal2_TextChanged

        If srcIndex = DataSourceEnum.dsChanSetting Then
            PopulateCboVal1List(srcIndex)

            If (_val1 < 0) Or (_val1 >= dataSourceChanSettingString.Length) Then
                _val1 = dataSourceChanSettingString.Length - 1
            End If
            cboVal1.SelectedIndex = Val1
            AddHandler cboVal1.SelectedIndexChanged, AddressOf cboVal1_SelectedIndexChanged

            'If cboChannel.Items.Count <> allChannelsStringPlusThisChan.Length Then
            RemoveHandler cboChannel.SelectedIndexChanged, AddressOf cboChannel_selectedIndexChanged
                cboChannel.Items.Clear()
            cboChannel.Items.AddRange(devRef.allChannelsStringPlusThisChan)
            'End If
            AddHandler cboChannel.SelectedIndexChanged, AddressOf cboChannel_selectedIndexChanged
            If _val2 < devRef.allChannelsStringPlusThisChan.Length Then
                cboChannel.SelectedIndex = _val2
                RaiseEvent Val2_Changed(Nothing, _val2)
            Else
                cboChannel.SelectedIndex = -1
            End If

            lblMin.Visible = False
            txtVal1.Visible = False
            cboVal1.Left = cboSource.Left + cboSource.Width + controlPadding + 5
            cboVal1.Visible = True
            lblMax.Left = cboVal1.Left + cboVal1.Width + controlPadding
            lblMax.Text = "Chan:"
            lblMax.Visible = True
            txtVal2.Visible = False
            cboChannel.Left = lblMax.Left + lblMax.Width + controlPadding
            cboChannel.Visible = True
            lblUnits.Visible = False

        Else
            RemoveHandler cboChannel.SelectedIndexChanged, AddressOf cboChannel_selectedIndexChanged
            cboChannel.Visible = False

        End If

        If srcIndex = DataSourceEnum.dsDirect Then
            lblMin.Visible = False
            lblMax.Visible = False
            cboVal1.Visible = False
            txtVal2.Visible = False

            txtVal1.Left = cboSource.Left + cboSource.Width + controlPadding + lblMin.Width + controlPadding
            txtVal1.Text = _val1
            txtVal1.Visible = True
            AddHandler txtVal1.TextChanged, AddressOf txtVal1_TextChanged

            lblUnits.Left = txtVal1.Left + txtVal1.Width + controlPadding
            lblUnits.Visible = True

        ElseIf (srcIndex = DataSourceEnum.dsProgramVar) Or (srcIndex = DataSourceEnum.dsSysVar) Or
                 (srcIndex = DataSourceEnum.dsSysSetting) Or (srcIndex = DataSourceEnum.dsDigIn) Or
                 (srcIndex = DataSourceEnum.dsDigOut) Or (srcIndex = DataSourceEnum.dsTimer) Then

            lblMin.Visible = False
            lblMax.Visible = False
            txtVal1.Visible = False
            txtVal2.Visible = False
            cboVal1.Left = cboSource.Left + cboSource.Width + controlPadding + lblMin.Width + controlPadding
            cboVal1.Visible = True

            'Set up the Val combobox with the corresponding items for the selected source:
            PopulateCboVal1List(srcIndex)
            If Not IsNothing(cboVal1.Items) Then
                If _val1 < cboVal1.Items.Count Then
                    cboVal1.SelectedIndex = _val1
                Else
                    '_val1 = 0
                End If

            End If
            AddHandler cboVal1.SelectedIndexChanged, AddressOf cboVal1_SelectedIndexChanged

            lblUnits.Left = cboVal1.Left + cboVal1.Width + controlPadding
            lblUnits.Visible = False 'True

        ElseIf srcIndex = DataSourceEnum.dsRandom Then
            cboVal1.Visible = False
            lblMin.Left = cboSource.Left + cboSource.Width + controlPadding
            lblMin.Visible = True

            txtVal1.Left = lblMin.Left + lblMin.Width + controlPadding
            If txtVal1.Text <> _val1.ToString Then
                txtVal1.Text = _val1
            End If
            txtVal1.Visible = True
            AddHandler txtVal1.TextChanged, AddressOf txtVal1_TextChanged
            lblMax.Text = "Max:"

            lblMax.Left = txtVal1.Left + txtVal1.Width + controlPadding
            lblMax.Visible = True

            If txtVal2.Text = "" Then txtVal2.Text = _val2
            If txtVal2.Text <> _val2.ToString Then txtVal2.Text = _val2
            txtVal2.Left = lblMax.Left + lblMax.Width + controlPadding
            txtVal2.Visible = True
            AddHandler txtVal2.TextChanged, AddressOf txtVal2_TextChanged

            If lblUnits.Text <> strUnitsLabel Then
                lblUnits.Text = strUnitsLabel
            End If
            lblUnits.Left = txtVal2.Left + txtVal2.Width + controlPadding
            lblUnits.Visible = True

        End If

        RaiseEvent Source_Changed(Me, cboSource.SelectedIndex)
    End Sub

    Private Sub PopulateCboVal1List(ByVal srcIndex As DataSourceEnum)
        Dim tmpEditVarNameButtonVisible As Boolean = False

        cboVal1.Items.Clear()
        cboVal1.Text = ""
        If (srcIndex = DataSourceEnum.dsDigIn) Or (srcIndex = DataSourceEnum.dsDigOut) Then
            Dim tmpCount As Integer = 0
            If srcIndex = DataSourceEnum.dsDigIn Then tmpCount = _digInCount
            If srcIndex = DataSourceEnum.dsDigOut Then tmpCount = _digOutCount

            cboVal1.Width = 45
            For n As Integer = 0 To tmpCount - 1
                cboVal1.Items.Add(n)
            Next
        ElseIf srcIndex = DataSourceEnum.dsProgramVar Then
            RemoveHandler cboVal1.SelectedIndexChanged, AddressOf cboVal1_SelectedIndexChanged
            cboVal1.Items.Clear()
            For n As Integer = 0 To _progVarCount - 1
                cboVal1.Items.Add("(" & n & ")  " & devRef.Prog(_curProgNum).varName(n))
            Next
            If (_val1 >= 0) And (_val1 < _progVarCount) Then
                cboVal1.SelectedIndex = _val1
                tmpEditVarNameButtonVisible = True
            End If
            AddHandler cboVal1.SelectedIndexChanged, AddressOf cboVal1_SelectedIndexChanged
            cboVal1.Width = 157 '194

        ElseIf srcIndex = DataSourceEnum.dsSysVar Then
            RemoveHandler cboVal1.SelectedIndexChanged, AddressOf cboVal1_SelectedIndexChanged
            cboVal1.Items.Clear()
            For n As Integer = 0 To _progVarCount - 1
                cboVal1.Items.Add("(" & n & ")  " & devRef.Prog(0).varName(n))
            Next
            If (_val1 >= 0) And (_val1 < _progVarCount) Then
                cboVal1.SelectedIndex = _val1
                tmpEditVarNameButtonVisible = True
            End If
            AddHandler cboVal1.SelectedIndexChanged, AddressOf cboVal1_SelectedIndexChanged
            cboVal1.Width = 157 '194

        ElseIf srcIndex = DataSourceEnum.dsChanSetting Then
            cboVal1.Items.AddRange(dataSourceChanSettingString)
            cboVal1.Width = 90
        ElseIf srcIndex = DataSourceEnum.dsSysSetting Then
            cboVal1.Items.AddRange(dataSourceSysSettingString)
            cboVal1.Width = 90
        ElseIf srcIndex = DataSourceEnum.dsTimer Then
            RemoveHandler cboVal1.SelectedIndexChanged, AddressOf cboVal1_SelectedIndexChanged
            cboVal1.Items.Clear()
            For n As Integer = 0 To _progTimerCount - 1
                cboVal1.Items.Add("(" & n & ")  " & devRef.Prog(_curProgNum).timerName(n))
            Next
            If (_val1 >= 0) And (_val1 < _progTimerCount) Then
                cboVal1.SelectedIndex = _val1
                'tmpEditVarNameButtonVisible = True
            End If
            AddHandler cboVal1.SelectedIndexChanged, AddressOf cboVal1_SelectedIndexChanged
            cboVal1.Width = 157 '194
        End If
        cmdEditVarName.Visible = tmpEditVarNameButtonVisible
        _lastSrcIndexForCboVal = srcIndex
    End Sub



    Private Sub cboVal1_SelectedIndexChanged(sender As Object, e As EventArgs)
        _val1 = cboVal1.SelectedIndex

        If (cboSource.SelectedIndex = DataSourceEnum.dsSysVar) Or (cboSource.SelectedIndex = DataSourceEnum.dsProgramVar) Then
            cmdEditVarName.Visible = True
        Else
            cmdEditVarName.Visible = False
        End If

        RaiseEvent Val_Changed(Me, cboVal1.SelectedIndex)
    End Sub

    Private Sub cboChannel_selectedIndexChanged(sender As Object, e As EventArgs)
        _val2 = cboChannel.SelectedIndex
        RaiseEvent Val2_Changed(Me, cboChannel.SelectedIndex)
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

    Private Sub txtVal1_TextChanged(sender As Object, e As EventArgs)
        If txtVal1.Text = "" Then
            _val1 = 0
        ElseIf IsNumeric(txtVal1.Text) = False Then
            _val1 = RemoveNonNumeric(txtVal1.Text)
            txtVal1.Text = _val1
            txtVal1.SelectionStart = txtVal1.Text.Length
            Beep()
        ElseIf CInt(txtVal1.Text) > _maxVal Then
            txtVal1.Text = _maxVal
            txtVal1.SelectionStart = txtVal1.Text.Length
            Beep()
        Else
            _val1 = CInt(txtVal1.Text)
        End If

        RaiseEvent Val_Changed(Me, _val1)
    End Sub

    Private Sub txtVal2_TextChanged(sender As Object, e As EventArgs)
        If txtVal2.Text = "" Then
            _val2 = 0
        ElseIf IsNumeric(txtVal2.Text) = False Then
            _val2 = RemoveNonNumeric(txtVal2.Text)
            txtVal2.Text = _val2
            txtVal2.SelectionStart = txtVal2.Text.Length
            Beep()
        ElseIf CInt(txtVal2.Text) > _maxVal Then
            txtVal2.Text = _maxVal
            txtVal2.SelectionStart = txtVal2.Text.Length
            Beep()
        Else
            _val2 = CInt(txtVal2.Text)
        End If

        RaiseEvent Val2_Changed(Me, _val2)
    End Sub

    Private Sub cmdEditVarName_Click(sender As Object, e As EventArgs) Handles cmdEditVarName.Click
        Dim progIndex As Integer = 0
        Dim varIndex As Integer = cboVal1.SelectedIndex
        Dim srcIndex As Integer = cboSource.SelectedIndex

        If srcIndex = DataSourceEnum.dsSysVar Then
            progIndex = 0
        ElseIf srcIndex = DataSourceEnum.dsProgramVar Then
            progIndex = _curProgNum
        Else
            cmdEditVarName.Visible = False
            Exit Sub
        End If

        Dim tmpDialog As New DialogVarNamesSingle
        tmpDialog.ProgNum = progIndex
        tmpDialog.VarNum = varIndex
        tmpDialog.VarName = devRef.Prog(progIndex).varName(varIndex)
        If tmpDialog.ShowDialog = DialogResult.OK Then
            'save the name
            devRef.Prog(progIndex).varName(varIndex) = tmpDialog.VarName
            PopulateCboVal1List(srcIndex)
        End If
    End Sub
End Class
