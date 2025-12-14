Imports System.ComponentModel
Imports System.ComponentModel.Design
Imports System.Windows.Forms
Imports System.Windows.Forms.Design
Imports Tens2502.DialogVarNamesAll

Public Class DialogDeviceType

    Public SelectedDeviceType As Integer = -1
    Public DefaultSelection As Integer = -1

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub lstDeviceType_SelectedIndexChanged_CreateNewDevice(sender As Object, e As EventArgs)
        'This function is used when the dialog is in the "Create new device" mode.
        SelectedDeviceType = lstDeviceType.SelectedIndex
        If SelectedDeviceType < 0 Then
            RichTextBox1.Text = "Select a Device..."
            Exit Sub
        End If
        FillStatsLabel(SelectedDeviceType)
    End Sub

    Private Sub lstDeviceType_SelectedIndexChanged_SetDefaultDevice(sender As Object, e As EventArgs)
        'This function is used when the dialog is in the "Set Default Device" mode.  The listbox
        'has an extra entry at the beginning compared to when the dialog is opened in the "create new device" mode.
        'Therefore, real device choices are shifted from the selected index of the list box.

        SelectedDeviceType = If(lstDeviceType.SelectedIndex = 0, -1, lstDeviceType.SelectedIndex - 1)
        If SelectedDeviceType = -1 Then
            RichTextBox1.Text = "Ask every time."
        Else
            FillStatsLabel(SelectedDeviceType)
        End If


    End Sub

    Private Sub FillStatsLabel(devType As DeviceType_t)
        Dim dev As Device_t = GetDeviceTemplate(devType)
        Dim txt As String = deviceTypeString(SelectedDeviceType) & vbCrLf & vbCrLf
        txt &= "Motor Chans: " & vbTab & dev.NumMotorChans & vbCrLf
        txt &= "Tens Chans: " & vbTab & dev.NumTensChans & vbCrLf
        txt &= "Aux Chans: " & vbTab & dev.NumAuxChans & vbCrLf
        txt &= "Dig Inputs: " & vbTab & dev.NumDigInputs & vbCrLf
        txt &= "Dig Outputs: " & vbTab & dev.NumDigOutputs & vbCrLf
        txt &= "Radio: " & vbTab & If(dev.RadioId > 0, "Yes", "No") & vbCrLf
        'txt &= "BlueTooth: " & dev.NumMotorChans & vbCrLf

        RichTextBox1.Text = txt
    End Sub
    Public Function ShowDevTypeDialog(Optional initType As Integer = -1) As MsgBoxResult

        Me.Text = "Choose New Device Type"
        RemoveHandler lstDeviceType.SelectedIndexChanged, AddressOf lstDeviceType_SelectedIndexChanged_CreateNewDevice
        RemoveHandler lstDeviceType.SelectedIndexChanged, AddressOf lstDeviceType_SelectedIndexChanged_SetDefaultDevice
        lstDeviceType.Items.Clear()
        For n As Integer = 0 To deviceTypeString.Length - 1
            lstDeviceType.Items.Add(deviceTypeString(n))
        Next

        RemoveHandler chkDefault.CheckedChanged, AddressOf chkDefault_CheckedChanged
        chkDefault.Checked = False
        chkDefault.Visible = True
        AddHandler chkDefault.CheckedChanged, AddressOf chkDefault_CheckedChanged


        AddHandler lstDeviceType.SelectedIndexChanged, AddressOf lstDeviceType_SelectedIndexChanged_CreateNewDevice
        RichTextBox1.Text = "Choose a device"
        lstDeviceType.SelectedIndex = initType

        Return Me.ShowDialog
    End Function

    Public Function ShowSetDefaultDevTypeDialog(curDefaultType As Integer) As MsgBoxResult

        RemoveHandler lstDeviceType.SelectedIndexChanged, AddressOf lstDeviceType_SelectedIndexChanged_SetDefaultDevice
        RemoveHandler lstDeviceType.SelectedIndexChanged, AddressOf lstDeviceType_SelectedIndexChanged_CreateNewDevice
        lstDeviceType.Items.Clear()
        Me.Text = "Select Default Device Type"
        Dim tmpAsk As String = "Ask every time."
        lstDeviceType.Items.Add(tmpAsk)
        For n As Integer = 0 To deviceTypeString.Length - 1
            lstDeviceType.Items.Add(deviceTypeString(n))
        Next

        chkDefault.Visible = False

        AddHandler lstDeviceType.SelectedIndexChanged, AddressOf lstDeviceType_SelectedIndexChanged_SetDefaultDevice
        RichTextBox1.Text = "Choose a device"
        lstDeviceType.SelectedIndex = If(curDefaultType < 0, 0, curDefaultType + 1)

        Return Me.ShowDialog
    End Function

    Private Sub chkDefault_CheckedChanged(sender As Object, e As EventArgs) Handles chkDefault.CheckedChanged
        DefaultSelection = If(chkDefault.Checked = True, SelectedDeviceType, -1)
    End Sub
End Class
