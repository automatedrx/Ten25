Imports System.ComponentModel
Imports System.ComponentModel.Design
Imports System.Data.Common
Imports System.Drawing.Configuration
Imports System.Drawing.Text
Imports System.IO
Imports System.IO.Ports
Imports System.Reflection
Imports System.Runtime.InteropServices
Imports System.Runtime.InteropServices.ComTypes
Imports System.Security.AccessControl
Imports System.Windows.Forms.VisualStyles
Imports System.Xml.Schema
Imports Microsoft.SqlServer.Server
Imports Microsoft.VisualBasic.Devices
Imports Tens25.comDef

Public Class Form1

    Dim tensCurVersion As String = "TENS2501"

    Public Enum AutoStatusenum As Integer
        Off = 0
        SlowChanStatus = 1
        FastChanStatus = 2
    End Enum

    Public Enum Direction As Integer
        Down = 1
        DontMove = 0
        Up = -1
        TowardsStart = -1
        TowardsEnd = 1
    End Enum

    Dim WithEvents ComPort As New System.IO.Ports.SerialPort

    Dim frmWidth_ChanControl = 0
    Dim frmWidth_Progs = 725
    Dim frmWidth_Settings = 1100

    Dim ControlColorGreen As Color = Color.Lime
    Dim ControlColorNone As Color = Me.BackColor
    Dim ControlColorRed As Color = Color.HotPink


    Dim chanControls(1) As ChannelUserControl

    Dim validDevice As Boolean = False  'Flag that indicates true when a successful response has been received from a tens device
    'Dim devDiscoveryComplete As Boolean = False  'Flag that indicates whether the device has been fully set up in vb code via initial messages back-and-forth

    Dim radioId As Integer = -1
    Dim chanType As Integer() = {enumChantype.Unknown}
    Dim numMasterChannels As Integer = 0 '1
    Dim numMotorChannels As Integer = 0 '2
    Dim numTensChannels As Integer = 0 '2
    Dim numChannels As Integer = 0 '5
    Dim numAuxChannels As Integer = 0

    Dim tmpMinSpeed As Integer = 10
    Dim tmpMaxSpeed As Integer = 1000

    Dim maxTensOutputPct = My.Settings.TensMaxOutputLow

    Dim batLevel As Integer = -1
    Dim charging As Boolean = False
    Dim charged As Boolean = False

    'Dim autoStatusUpdate As Boolean = False
    Dim autoStatus As AutoStatusenum = AutoStatusenum.Off
    Dim autoStatusLast As AutoStatusenum = AutoStatusenum.Off
    Dim nextParamArrayAutoStatus = 0



    Dim rxBuff As String = ""
    Dim newMsgReceived As Boolean = False

    '==Programs==
    Dim progTable() As DataTable
    'Dim progData(0, 0, 12) As Integer '[program] [Line] [field]
    'Dim numChanProgs As Integer = 0
    'Dim numSysProgs As Integer = 0
    Dim numPrograms As Integer = 0      'NOTE: This list is 1-based.  So a value of 0 means there are no programs present.
    Dim numProgLines() As Integer = {0}
    Dim progName() As String = {""}

    Dim curProg As Integer = -1 'used in the Programs tab

    'Dim progComStartTime As DateTime
    Dim progComTimeoutTime As DateTime
    Dim lastCommandTypeReceived As Integer
    Dim lastProgSingleParamSent As Integer
    Dim lastProgSingleParamReceived As Integer
    Dim lastProgParamArraySent As Integer
    Dim lastProgParamArraayReceived As Integer

    Dim activeProgType As progType = progType.Unknown

    Dim commandTable As New DataTable


    Dim unsavedProgChanges As Boolean = False

    'These are used for comboboxes in the datagridview.
    Dim cb As ComboBox
    Dim cbRow As Integer = -1
    Dim cbCol As Integer = -1

    Dim tmpRowCount = 0


#Region "Form Related"



    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles Me.Load
        RefreshComPortList()
        comportIsOpen(False)
        SetTensMaxOutputValues(My.Settings.TensMaxOutputLow, My.Settings.TensMaxOutputMedium, My.Settings.TensMaxOutputHigh)
        tsChanMonDDCmdTensMaxV.Text = "Max Tens Output: " & My.Settings.TensMaxOutputLow & "%"

    End Sub

    'Private Sub SetNumChannels(ByVal motChans, ByVal tensChans, ByVal sysChans)
    '    numMotorChannels = motChans
    '    numTensChannels = tensChans
    '    numSysChannels = sysChans

    '    Dim m As Integer = 0
    '    Dim t As Integer = 0
    '    Dim s As Integer = 0

    '    ReDim allChannelsString(motChans + tensChans + sysChans - 1) 'numMotorChannels + numTensChannels + numSysChannels)

    '    If numMotorChannels > 0 Then
    '        ReDim motorChanNameString(numMotorChannels - 1)
    '        For m = 0 To numMotorChannels - 1
    '            motorChanNameString(m) = New String("Motor " & (m + 1))
    '            allChannelsString(m) = New String("Motor" & (m + 1))
    '        Next
    '    End If

    '    If numTensChannels > 0 Then
    '        ReDim tensChanNameString(numTensChannels - 1)
    '        For t = 0 To numTensChannels - 1
    '            tensChanNameString(t) = New String("Tens " & (t + 1))
    '            allChannelsString(m + t) = New String("Tens " & (t + 1))
    '        Next
    '    End If

    '    If numSysChannels > 0 Then
    '        ReDim sysChanNameString(numSysChannels - 1)
    '        For s = 0 To numSysChannels - 1
    '            sysChanNameString(s) = New String("System " & (s + 1))
    '            allChannelsString(m + t + s) = New String("System " & (s + 1))
    '        Next
    '    End If

    'End Sub
    Private Sub SetNumChannels(ByVal masterChans As Integer, ByVal motChans As Integer,
                               ByVal tensChans As Integer, ByVal auxChans As Integer)
        numMasterChannels = masterChans
        numMotorChannels = motChans
        numTensChannels = tensChans
        numAuxChannels = auxChans

        Dim s As Integer = 0
        Dim m As Integer = 0
        Dim t As Integer = 0
        Dim a As Integer = 0

        ReDim allChannelsString(masterChans + motChans + tensChans + auxChans - 1) 'numMotorChannels + numTensChannels + numSysChannels)

        If numMasterChannels > 0 Then
            ReDim masterChanNameString(numMasterChannels - 1)
            For s = 0 To numMasterChannels - 1
                If s = 0 Then
                    masterChanNameString(s) = New String("Master")
                Else
                    masterChanNameString(s) = New String("Master " & (s + 1))
                End If
                allChannelsString(s) = masterChanNameString(s)
            Next
        End If

        If numMotorChannels > 0 Then
            ReDim motorChanNameString(numMotorChannels - 1)
            For m = 0 To numMotorChannels - 1
                motorChanNameString(m) = New String("Motor " & (m + 1))
                allChannelsString(s + m) = New String("Motor" & (m + 1))
            Next
        End If

        If numTensChannels > 0 Then
            ReDim tensChanNameString(numTensChannels - 1)
            For t = 0 To numTensChannels - 1
                tensChanNameString(t) = New String("Tens " & (t + 1))
                allChannelsString(s + m + t) = New String("Tens " & (t + 1))
            Next
        End If

        If numAuxChannels > 0 Then
            ReDim auxChanNameString(numAuxChannels - 1)
            For a = 0 To numAuxChannels - 1
                auxChanNameString(a) = New String("Aux " & (a + 1))
                allChannelsString(s + m + t + a) = New String("Aux " & (a + 1))
            Next
        End If

    End Sub

    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        My.Settings.Save()
    End Sub

    Private Sub TabControlMain_SelectedIndexChanged(sender As Object, e As EventArgs) Handles TabControlMain.SelectedIndexChanged
        If (TabControlMain.SelectedTab.Text = "Channel Monitor") Or (TabControlMain.SelectedIndex) = 0 Then
            SetAutoStatusMode(autoStatusLast)
            Width = frmWidth_ChanControl
        Else
            autoStatus = AutoStatusenum.Off
            Width = frmWidth_Progs
        End If
    End Sub

    Private Sub cmdRefreshComportList_Click(sender As Object, e As EventArgs) Handles cmdRefreshComportList.Click
        RefreshComPortList()
    End Sub

    Public Sub RefreshComPortList()
        cboComPorts.Items.Clear()
        'cboComports.Items.AddRange(My.Computer.Ports.SerialPortNames.ToArray)
        Dim availPorts As String() = My.Computer.Ports.SerialPortNames.ToArray
        Array.Copy(My.Computer.Ports.SerialPortNames.ToArray, availPorts, My.Computer.Ports.SerialPortNames.ToArray.Length)
        Array.Sort(availPorts)
        cboComPorts.Items.AddRange(availPorts)
        If My.Settings.lastComPort.Length Then
            If cboComPorts.Items.Contains(My.Settings.lastComPort.ToString) Then
                For Each tmpItem As String In cboComPorts.Items
                    If tmpItem.ToString = My.Settings.lastComPort.ToString Then
                        cboComPorts.SelectedItem = tmpItem
                        Exit For
                    End If
                Next
            End If
        End If

    End Sub

    Private Sub cmdConnect_Click(sender As Object, e As EventArgs) Handles cmdConnect.Click
        If ComPort.IsOpen = True Then
            CloseComPort()
        Else
            OpenComPort()
            requestParameter(pStatEnum.TensProgCurVer)
        End If
    End Sub

    Private Sub cmdStop_Click(sender As Object, e As EventArgs) Handles cmdStop.Click
        Chan_Enabled_Click(numChannels, False)
    End Sub

    Private Sub OpenComPort()

        'If ComPort.PortName.Length < 1 Then

        If My.Computer.Ports.SerialPortNames.Contains(cboComPorts.Text) Then
            ComPort.PortName = cboComPorts.Text
        Else
            RefreshComPortList()
            Exit Sub
        End If

        Try
            ComPort.Open()
            'ComPort.
            comportIsOpen(True)
        Catch ex As Exception
            MsgBox("Could not open Com Port." & vbCrLf & "Error: " & ex.ToString)
            comportIsOpen(False)
        End Try

    End Sub

    Private Sub CloseComPort()
        Try
            ComPort.Close()
        Catch ex As Exception

        End Try
        comportIsOpen(False)
    End Sub

    Private Sub comportIsOpen(ByVal isOpen As Boolean)
        If isOpen Then
            'cmdConnect.BackColor = ControlColorGreen
            cmdConnect.BackColor = ControlColorNone
            lblDevStatus.Text = "Initiating connection to a device..."
            cmdConnect.Text = "Disconnect"
            cboComPorts.Enabled = False
            cmdRefreshComportList.Enabled = False
        Else
            'cmdConnect.BackColor = ControlColorRed
            cmdConnect.BackColor = ControlColorNone
            lblDevStatus.Text = "No device connected."
            cmdConnect.Text = "Connect"
            cboComPorts.Enabled = True
            cmdRefreshComportList.Enabled = True
            validDevice = False 'reset the flag so that when a comport is opened again, the program will initiate communications.
            TabControlMain.Enabled = False 'controls are disabled here.  The are enabled at the same time validDevice is set to true.
        End If
    End Sub

#End Region
#Region "Communications Related"


    Private Sub ComPort_DataReceived(sender As Object, e As SerialDataReceivedEventArgs) Handles ComPort.DataReceived
        Dim rxData As String = ComPort.ReadExisting
        'rxBuff = rxBuff.Concat(rxData)
        rxBuff &= rxData
        If rxBuff.Contains(Chr(10)) Then
            'tmrNewMessages.Enabled = True
            newMsgReceived = True
        End If
    End Sub

    Private Sub dataSend(ByRef buff As String, ByVal len As Integer)
        If ComPort.IsOpen Then

            Try
                ComPort.Write(buff)
            Catch ex As Exception
                MsgBox("Error!  " & ex.Message)
                CloseComPort()
                SetAutoStatusMode(AutoStatusenum.Off)
            End Try
        Else
            SetAutoStatusMode(AutoStatusenum.Off)
        End If
    End Sub

    Private Sub tmrNewMessages_Tick(sender As Object, e As EventArgs) Handles tmrNewMessages.Tick
        'The timer is enabled by the comPort rx isr when a cr is detected in the rx buffer.  We're disabling
        'the timer here at the start of the timer routine so that it works like a 1-shot timer.
        tmrNewMessages.Enabled = False

        If newMsgReceived = True Then
            newMsgReceived = False
            parseRxMessage()
        End If

        'If there are additional line endings in the buffer then reenable the timer so that it'll parse them semi-recursively.
        If rxBuff.Contains(Chr(10)) Then
            'tmrNewMessages.Enabled = True
            newMsgReceived = True
        End If
        tmrNewMessages.Enabled = True
    End Sub

    Private Sub parseRxMessage()
        Dim eomIndx As Integer = rxBuff.IndexOf(Chr(10))
        Dim msg As String = rxBuff.Substring(0, eomIndx)
        Dim val As String = ""
        rxBuff = rxBuff.Remove(0, eomIndx + 1)

        Dim cmd As commandTypeEnum = Asc(msg.Chars(0))
        Dim param As pStatEnum = pStatEnum.None
        If msg.Length > 1 Then
            param = Asc(msg.Chars(1))
        End If

        Dim eqIndex = msg.IndexOf("=")
        Dim paramIndex As Integer = -1
        Dim tmpIndexLen = eqIndex - 2       ' jb32=xyz
        'For n As Integer = 2 To (eqIndex - 1)
        'paramIndex = AscW(msg.Chars(n)) * (10 ^ (tmpIndexLen - 1))
        'tmpIndexLen -= 1
        Dim strParamIndex As String = ""
        If msg.Length > 2 Then
            strParamIndex = msg.Substring(2, tmpIndexLen)
        End If
        If strParamIndex.Length Then paramIndex = CInt(strParamIndex)
        'Next

        If eqIndex > 0 Then
            val = msg.Substring(eqIndex + 1, msg.Length - eqIndex - 1)
        End If

        lastCommandTypeReceived = cmd

        Select Case cmd
            Case commandTypeEnum.ACK

            Case commandTypeEnum.NAK

            Case commandTypeEnum.SetParamArray        '//Followed by an index num
                setParamArray(param, val, paramIndex)
            Case commandTypeEnum.SetSingleParam       '//Followed by a pStatEnum
                setSingleParam(param, val, paramIndex)
            Case commandTypeEnum.GetParamArray        '//Followed by an index num

            Case commandTypeEnum.GetSingleParam       '//Followed by a pStatEnum

            Case commandTypeEnum.SetFileData

            Case commandTypeEnum.GetFileData

        End Select
    End Sub



    Private Sub setSingleParam(ByVal param As pStatEnum, ByVal val As String, Optional ByVal paramIndex As Integer = -1)
        lastProgSingleParamReceived = param

        Select Case param
            Case pStatEnum.BattLevel
            Case pStatEnum.Charging
            Case pStatEnum.Charged

            Case pStatEnum.NumMotorChannels
            Case pStatEnum.NumTensChannels
            Case pStatEnum.NumChannels
            Case pStatEnum.NumInputs
            Case pStatEnum.NumOutputs
            Case pStatEnum.MotIndexStart
            Case pStatEnum.TensIndexStart
            'Case pStatEnum.ChanProglinesMax
            'Case pStatEnum.SysProgLinesMax
            Case pStatEnum.TensProgCurVer
                'If val.Contains("TENS2501") Then
                If val.Contains(tensCurVersion) Then
                    'correct version!
                    If validDevice = False Then
                        validDevice = True
                        TabControlMain.Enabled = True
                        cmdConnect.BackColor = ControlColorGreen
                        My.Settings.lastComPort = cboComPorts.Text
                        requestParamArray(paramArrayEnum.DeviceInfo)
                    End If
                End If
            Case pStatEnum.TensProgMinVer

            Case pStatEnum.ChanType
            Case pStatEnum.ChanEnabled
            Case pStatEnum.ProgNumber
            Case pStatEnum.ProgState
            Case pStatEnum.CurLineNum
            Case pStatEnum.ChanCurIntensityPct
            Case pStatEnum.CurSpeed

            Case pStatEnum.NumberOfPrograms
                SetCurrentNumberOfPrograms(CInt(val), True)

            Case pStatEnum.ProgramLength
                If (paramIndex >= 0) And (numProgLines.Length > paramIndex) Then
                    numProgLines(paramIndex) = CInt(val)
                End If
            Case pStatEnum.ProgramName
                If (paramIndex >= 0) And (progName.Length > paramIndex) Then
                    progName(paramIndex) = val
                End If

        End Select
    End Sub

    Private Sub setParamArray(ByVal pArrayNum As paramArrayEnum, ByVal val As String, ByVal paramIndex As Integer)
        'Split the val into an array of values
        Dim valArray As String() = val.Split(",")

        Select Case pArrayNum
            Case paramArrayEnum.DeviceInfo
                '// RadioId, NumChannels, NumMotorChannels, NumTensChannels, NumSysChannels, minSpeed, maxSpeed
                'If valArray.Length = 5 Then
                If valArray.Length >= 2 Then
                    radioId = CInt(valArray(0))
                    lblDevStatus.Text = "DevId: " & radioId

                    numChannels = CInt(valArray(1))
                    ReDim chanType(numChannels - 1)

                    Dim masterChan As Integer = 0
                    Dim mChans As Integer = 0
                    Dim tChans As Integer = 0
                    Dim auxChans As Integer = 0
                    For n As Integer = 0 To numChannels - 1
                        Dim curType As Integer = CInt(valArray(n + 2))
                        chanType(n) = curType
                        Select Case curType
                            Case enumChantype.Master
                                masterChan += 1
                            Case enumChantype.Motor
                                mChans += 1
                            Case enumChantype.Tens
                                tChans += 1
                            Case enumChantype.Aux
                                auxChans += 1
                        End Select

                    Next



                    'SetNumMotorChannels(CInt(valArray(2))) 'numMotorChannels = CInt(valArray(2))
                    'SetNumTensChannels(CInt(valArray(3))) 'numTensChannels = CInt(valArray(3))
                    'SetNumSysChannels(CInt(valArray(4))) 'numSysChannels = CInt(valArray(4))
                    'SetNumChannels(CInt(valArray(2)), CInt(valArray(3)), CInt(valArray(4)))
                    'tmpMinSpeed = CInt(valArray(5))
                    'tmpMaxSpeed = CInt(valArray(6))

                    SetNumChannels(masterChan, mChans, tChans, auxChans)

                    initChannels()

                    'chkAutostatus.Checked = True
                    'Start requesting channel min/max paramArrays.  Start by requesting the first channel.  When
                    ' that paramArray is received, it will request the next channel's min/max.  This repeats until
                    ' all channels have been received.
                    requestParamArray(paramArrayEnum.chanMinMaxInfo, 0)
                End If
            Case paramArrayEnum.chanMinMaxInfo
                '// minSpeed, maxSpeed
                chanControls(paramIndex).MinSpeed = CInt(valArray(0))
                chanControls(paramIndex).MaxSpeed = CInt(valArray(1))

                paramIndex += 1
                If paramIndex < numChannels Then
                    requestParamArray(paramArrayEnum.chanMinMaxInfo, paramIndex)
                Else
                    'chkAutostatus.Checked = True
                    Select Case My.Settings.lastAutoStatus
                        Case AutoStatusenum.FastChanStatus
                            cmdtsChanMon_AutoStatusFast.PerformClick()
                            'autoStatus = AutoStatusenum.FastChanStatus
                        Case AutoStatusenum.SlowChanStatus
                            'autoStatus = AutoStatusenum.SlowChanStatus
                            cmdtsChanMon_AutoStatusSlow.PerformClick()
                        Case Else
                            'leave autostatus off
                            'autoStatus = AutoStatusenum.Off
                            cmdtsChanMonAutoStatusOff.PerformClick()
                    End Select
                End If
            Case paramArrayEnum.chanStats
                '// chan0-3
                '// 0 chanEnabled, 1 chanSpeed, 2 curLineNum, 3 startVal, 4 endVal, 5 modDuration,
                '// 6 percentComplete, 7 RepeatsRemaining, 8 chanCurVal, 9 chanCurIntensity,
                '// 10 progState, 11 lineCommand, 12 polarity
                chanControls(paramIndex).ChanEnabled = If(valArray(0) = "1", True, False)
                chanControls(paramIndex).Speed = CInt(valArray(1))
                chanControls(paramIndex).LineNum = CInt(valArray(2))
                chanControls(paramIndex).PulseWidthStart = CInt(valArray(3))
                chanControls(paramIndex).PulseWidthEnd = CInt(valArray(4))
                chanControls(paramIndex).Duration = CInt(valArray(5))
                '//==pulseWidth must be set BEFORE percentComplete!
                chanControls(paramIndex).PulseWidth = CInt(valArray(8))
                chanControls(paramIndex).PercentComplete = CInt(valArray(6))
                '//==
                chanControls(paramIndex).RepeatsRemaining = CInt(valArray(7))

                chanControls(paramIndex).OutputIntensityPct = CInt(valArray(9))
                chanControls(paramIndex).ProgState = CInt(valArray(10))
                chanControls(paramIndex).LineCommand = CInt(valArray(11))
                chanControls(paramIndex).Polarity = CInt(valArray(12))

                If autoStatus = AutoStatusenum.FastChanStatus Then
                    nextParamArrayAutoStatus = paramIndex + 1
                    If nextParamArrayAutoStatus >= numChannels Then
                        nextParamArrayAutoStatus = 0
                    End If
                    requestParamArray(paramArrayEnum.chanStats, nextParamArrayAutoStatus)
                End If

                'Case paramArrayEnum.sysStats
                '    '// sys channel
                '    '// sysEnabled, sysProgState, curLineNum, masterSpeed
                '    chanControls(numChannels).ChanEnabled = If(valArray(0) = "1", True, False)
                '    chanControls(numChannels).ProgState = CInt(valArray(1))
                '    chanControls(numChannels).LineNum = CInt(valArray(2))
                '    chanControls(numChannels).Speed = CInt(valArray(3))

                '    If autoStatus = AutoStatusenum.FastChanStatus Then
                '        nextParamArrayAutoStatus = 0
                '        requestParamArray(paramArrayEnum.chanStats, nextParamArrayAutoStatus)
                '    End If


        End Select
    End Sub




    Private Sub sendParameter(ByVal param As pStatEnum, ByVal newVal As Integer, Optional ByVal index As Integer = -1)
        'Dim dataBuff As String
        ''Dim tmpInt As Integer = 0
        'dataBuff = Chr(commandTypeEnum.SetSingleParam) & Chr(param)

        'If index >= 0 Then
        '    dataBuff &= index
        'End If
        'dataBuff &= "=" & newVal & Chr(10)
        'dataSend(dataBuff, dataBuff.Length)
        sendParameter(param, newVal.ToString, index)
    End Sub
    Private Sub sendParameter(ByVal param As pStatEnum, ByVal newVal As String, Optional ByVal index As Integer = -1)
        Dim dataBuff As String
        'Dim tmpInt As Integer = 0
        dataBuff = Chr(commandTypeEnum.SetSingleParam) & Chr(param)

        If index >= 0 Then
            dataBuff &= index
        End If
        dataBuff &= "=" & newVal & Chr(10)
        dataSend(dataBuff, dataBuff.Length)
    End Sub

    Private Sub requestParameter(ByVal param As pStatEnum, Optional ByVal index As Integer = -1)
        Dim dataBuff As String
        dataBuff = Chr(commandTypeEnum.GetSingleParam) & Chr(param)

        If index >= 0 Then
            dataBuff &= index
        End If
        dataBuff &= Chr(10)
        dataSend(dataBuff, dataBuff.Length)
    End Sub

    Private Sub sendParamArray(ByVal paNum As paramArrayEnum, Optional ByVal paramIndex As Integer = -1,
                               Optional ByVal paramIndex2 As Integer = -1)
        Dim dataBuff As String
        dataBuff = Chr(commandTypeEnum.SetParamArray) & Chr(paNum)

        Select Case paNum
            Case paramArrayEnum.progLineData
                If (paramIndex < 0) Or (paramIndex2 < 0) Then Return
                If IsNothing(progTable(paramIndex)) Then Return
                If IsNothing(progTable(paramIndex).Rows(paramIndex2)) Then Return
                dataBuff &= paramIndex & "," & paramIndex2 & "="
                For n As Integer = 0 To 14 - 1
                    If n > 0 Then dataBuff &= ","
                    Dim tmpVal As String = "0"
                    If Not IsNothing(progTable(paramIndex).Rows(paramIndex2).Item(n)) Then
                        tmpVal = progTable(paramIndex).Rows(paramIndex2).Item(n)
                    End If
                    dataBuff &= tmpVal
                Next
            Case Else
                Return
        End Select

        dataBuff &= Chr(10)
        dataSend(dataBuff, dataBuff.Length)
    End Sub
    Private Sub requestParamArray(ByVal paNum As paramArrayEnum, Optional ByVal paramIndex As Integer = -1,
                                  Optional ByVal index2 As Integer = -1)
        Dim dataBuff As String
        dataBuff = Chr(commandTypeEnum.GetParamArray) & Chr(paNum)
        If paramIndex >= 0 Then
            dataBuff &= paramIndex
            If index2 > 0 Then
                dataBuff &= "," & index2
            End If
        End If
        dataBuff &= Chr(10)
        dataSend(dataBuff, dataBuff.Length)
    End Sub

#End Region











#Region "Channel Controls"



    '============= Channel Controls '=============

    Private Sub initChannels()
        'remove any existing channel controls:
        For x As Integer = 0 To chanControls.Length - 1
            If Not IsNothing(chanControls(x)) Then
                chanControls(x).Dispose()
            End If
        Next

        'size the form so that the controls will fit:
        Dim numOfControls As Integer = (numChannels)
        Dim numPerLine As Integer = 5
        Dim tmpCC As New ChannelUserControl
        Dim tmpControlWithTensW As Integer = tmpCC.WidthWithTens
        Dim tmpControlWithoutTensW As Integer = tmpCC.WidthWithoutTens
        Dim tmpControlH As Integer = tmpCC.Height
        Dim tmpPadding As Integer = 5
        tmpCC.Dispose()
        Dim tmpCCperRow As Integer = 0
        Dim tmpFormHeight As Integer = 91 + tsTabChanMon.Height

        'Just use 1 row of controls for now.  If there's a tens device later that has more
        'controls then this will need to be reworked.
        frmWidth_ChanControl = 23 + tmpPadding + (numMasterChannels * tmpControlWithoutTensW) +
            (numMotorChannels * tmpControlWithoutTensW) + (numTensChannels * tmpControlWithTensW) +
            (numOfControls * tmpPadding) + (numAuxChannels * tmpControlWithoutTensW)
        Width = frmWidth_ChanControl
        Height = tmpFormHeight + tmpPadding + tmpControlH + tmpPadding

        ''create new controls:
        'Dim tmpNextX As Integer = 0
        'ReDim chanControls(numOfControls) '(numChannels + numSysChannels)
        'For n As Integer = 0 To (numOfControls - 1) '(numChannels + numSysChannels - 1)
        '    Dim tmpX As Integer = tmpNextX + tmpPadding
        '    tmpNextX += If((n < numMotorChannels) Or (n >= (numMotorChannels + numTensChannels)),
        '        tmpControlWithoutTensW, tmpControlWithTensW) + tmpPadding
        '    Dim tmpY As Integer = tsTabChanMon.Height + tmpPadding

        '    chanControls(n) = New ChannelUserControl With {.ChanIndex = n,
        '                .ChanType = If(n < numMotorChannels, 1, If(n < numMotorChannels + numTensChannels, 0, 2)),
        '                .ChanEnabled = False,
        '                .Location = New Point(tmpX, tmpY),'.Location = New Point((n * (.Width + 10)) + 10, 50),
        '                .MinSpeed = tmpMinSpeed, .MaxSpeed = tmpMaxSpeed, .Speed = 100,
        '                .ChanName = If((n < numMotorChannels), "Motor " & (n + 1), If((n < (numMotorChannels + numTensChannels)), "Tens " & (n - numMotorChannels + 1), "System " & (n - numChannels + 1))),
        '                .Duration = 0,
        '                .OutputIntensityMax = My.Settings.TensMaxOutputLow,
        '                .ProgState = 0,
        '                .PulseWidth = 0, .PulseWidthStart = 0, .PulseWidthEnd = 0,
        '                .Polarity = -1, .LineCommand = -1}

        '    AddHandler chanControls(n).Enabled_Click, AddressOf Chan_Enabled_Click
        '    AddHandler chanControls(n).Speed_Changed, AddressOf Chan_Speed_Changed
        '    AddHandler chanControls(n).OutputIntensity_Changed, AddressOf Chan_OutputIntensity_Changed
        '    tabChannelMonitor.Controls.Add(chanControls(n))

        'create new controls:
        Dim tmpNextX As Integer = 0
        ReDim chanControls(numOfControls) '(numChannels + numSysChannels)
        For n As Integer = 0 To (numOfControls - 1) '(numChannels + numSysChannels - 1)
            Dim tmpX As Integer = tmpNextX + tmpPadding
            tmpNextX += If(chanType(n) = enumChantype.Tens, tmpControlWithTensW, tmpControlWithoutTensW) + tmpPadding
            Dim tmpY As Integer = tsTabChanMon.Height + tmpPadding

            chanControls(n) = New ChannelUserControl With {.ChanIndex = n,
                        .ChanType = chanType(n),
                        .ChanEnabled = False,
                        .Location = New Point(tmpX, tmpY),'.Location = New Point((n * (.Width + 10)) + 10, 50),
                        .MinSpeed = tmpMinSpeed, .MaxSpeed = tmpMaxSpeed, .Speed = 100,
                        .ChanName = allChannelsString(n),' If((n < numMotorChannels), "Motor " & (n + 1), If((n < (numMotorChannels + numTensChannels)), "Tens " & (n - numMotorChannels + 1), "System " & (n - numChannels + 1))),
                        .Duration = 0,
                        .OutputIntensityMax = My.Settings.TensMaxOutputLow,
                        .ProgState = 0,
                        .PulseWidth = 0, .PulseWidthStart = 0, .PulseWidthEnd = 0,
                        .Polarity = -1, .LineCommand = -1}

            AddHandler chanControls(n).Enabled_Click, AddressOf Chan_Enabled_Click
            AddHandler chanControls(n).Speed_Changed, AddressOf Chan_Speed_Changed
            AddHandler chanControls(n).OutputIntensity_Changed, AddressOf Chan_OutputIntensity_Changed
            tabChannelMonitor.Controls.Add(chanControls(n))
        Next
    End Sub

    Private Sub Chan_Enabled_Click(index As Integer, NewState As Boolean)
        Dim tmpVal As Integer = If((NewState = True), 1, 0)
        sendParameter(pStatEnum.ChanEnabled, tmpVal, index)
    End Sub

    Private Sub Chan_Speed_Changed(ByVal index As Integer, ByVal NewSpeed As Integer)
        sendParameter(pStatEnum.CurSpeed, NewSpeed, index)
    End Sub
    Private Sub Chan_OutputIntensity_Changed(ByVal index As Integer, ByVal NewVal As Integer)
        sendParameter(pStatEnum.ChanCurIntensityPct, NewVal, index)
    End Sub






    Private Sub tmrStatusUpdates_Tick(sender As Object, e As EventArgs) Handles tmrStatusUpdates.Tick
        'This timer will request paramArrays from the device for status updates.
        tmrStatusUpdates.Enabled = False

        'If nextParamArrayAutoStatus < numChannels Then
        requestParamArray(paramArrayEnum.chanStats, nextParamArrayAutoStatus)
        'ElseIf nextParamArrayAutoStatus < (numChannels + numSysChannels) Then
        'requestParamArray(paramArrayEnum.sysStats, nextParamArrayAutoStatus)
        'End If

        nextParamArrayAutoStatus += 1
        If nextParamArrayAutoStatus >= numChannels Then '(numChannels + numSysChannels) Then
            nextParamArrayAutoStatus = 0
        End If
        tmrStatusUpdates.Enabled = If(autoStatus = AutoStatusenum.SlowChanStatus, True, False) 'autoStatusUpdate
    End Sub

    Private Sub SetAutoStatusMode(ByVal newMode As AutoStatusenum)
        If newMode = AutoStatusenum.Off Then
            autoStatus = AutoStatusenum.Off
        ElseIf validDevice = True Then
            autoStatus = newMode
        Else
            autoStatus = AutoStatusenum.Off
        End If

        Select Case autoStatus
            Case AutoStatusenum.Off
                cmdtsChanMonAutoStatusOff.Checked = True
                cmdtsChanMon_AutoStatusFast.Checked = False
                cmdtsChanMon_AutoStatusSlow.Checked = False
            Case AutoStatusenum.FastChanStatus
                cmdtsChanMonAutoStatusOff.Checked = False
                cmdtsChanMon_AutoStatusFast.Checked = True
                cmdtsChanMon_AutoStatusSlow.Checked = False

                nextParamArrayAutoStatus = 0
                requestParamArray(paramArrayEnum.chanStats, nextParamArrayAutoStatus)
            Case AutoStatusenum.SlowChanStatus
                cmdtsChanMonAutoStatusOff.Checked = False
                cmdtsChanMon_AutoStatusFast.Checked = False
                cmdtsChanMon_AutoStatusSlow.Checked = True

                tmrStatusUpdates.Enabled = True
        End Select
        autoStatusLast = autoStatus
    End Sub

    Private Sub cmdtsChanMonAutoStatusOff_Click(sender As Object, e As EventArgs) Handles cmdtsChanMonAutoStatusOff.Click
        SetAutoStatusMode(AutoStatusenum.Off)
        My.Settings.lastAutoStatus = AutoStatusenum.Off
    End Sub

    Private Sub cmdtsChanMon_AutoStatusFast_Click(sender As Object, e As EventArgs) Handles cmdtsChanMon_AutoStatusFast.Click
        SetAutoStatusMode(AutoStatusenum.FastChanStatus)
        My.Settings.lastAutoStatus = AutoStatusenum.FastChanStatus
    End Sub

    Private Sub cmdtsChanMon_AutoStatusSlow_Click(sender As Object, e As EventArgs) Handles cmdtsChanMon_AutoStatusSlow.Click
        SetAutoStatusMode(AutoStatusenum.SlowChanStatus)
        My.Settings.lastAutoStatus = AutoStatusenum.SlowChanStatus
    End Sub



    Private Sub SetTensMaxOutputValues(ByVal lowVal As Integer, ByVal medVal As Integer, ByVal hiVal As Integer)
        lowVal = If(lowVal < 0, 0, If(lowVal > 100, 100, lowVal))
        My.Settings.TensMaxOutputLow = lowVal
        tsCMTensMax_Low.Text = lowVal

        medVal = If(medVal < 0, 0, If(medVal > 100, 100, medVal))
        My.Settings.TensMaxOutputMedium = medVal
        tsCMTensMax_Med.Text = medVal

        hiVal = If(hiVal < 0, 0, If(hiVal > 100, 100, hiVal))
        My.Settings.TensMaxOutputHigh = hiVal
        tsCMTensMax_High.Text = hiVal
    End Sub
    Private Sub SendTensMaxOutputToChanControls(ByVal newMaxOutput As Integer)
        For n As Integer = 0 To (numChannels - 1) 'chanControls.Length - 1
            If chanControls(n).ChanType = enumChantype.Tens Then
                chanControls(n).OutputIntensityMax = newMaxOutput
            End If
        Next
    End Sub
    Private Sub tsCMTensMax_Low_Click(sender As Object, e As EventArgs) Handles tsCMTensMax_Low.Click
        tsChanMonDDCmdTensMaxV.Text = "Max Tens Output: " & My.Settings.TensMaxOutputLow & "%"
        SendTensMaxOutputToChanControls(My.Settings.TensMaxOutputLow)
    End Sub
    Private Sub tsCMTensMax_Med_Click(sender As Object, e As EventArgs) Handles tsCMTensMax_Med.Click
        tsChanMonDDCmdTensMaxV.Text = "Max Tens Output: " & My.Settings.TensMaxOutputMedium & "%"
        SendTensMaxOutputToChanControls(My.Settings.TensMaxOutputMedium)
    End Sub
    Private Sub tsCMTensMax_High_Click(sender As Object, e As EventArgs) Handles tsCMTensMax_High.Click
        tsChanMonDDCmdTensMaxV.Text = "Max Tens Output: " & My.Settings.TensMaxOutputHigh & "%"
        SendTensMaxOutputToChanControls(My.Settings.TensMaxOutputHigh)
    End Sub


#End Region



    '============= Programs '=============
#Region "Programs"

    'Private Function ClearProgramData() As Boolean
    Private Function ProceedWithUnsavedChanges() As Boolean
        'This function checks for any unsaved changes, and if there are any, give the user the option
        'to Cancel so that they can save their change.
        'If there are no unsaved changes, return true.
        'If there are unsaved changes and the user clicks OK then return true.
        'If there are unsaved changes and the user clicks Cancel then return false.

        If unsavedProgChanges Then
            If MsgBox("There are unsaved program changes.  Discard changes and proceed?", vbOKCancel) = vbCancel Then
                Return False
            Else
                Return True
            End If
        Else
            Return True
        End If
    End Function

    'Private Sub cmdGetAllProgramsFromDevice_Click(sender As Object, e As EventArgs) Handles cmdGetAllProgramsFromDevice.Click
    '    OpenProjectFromDevice()
    'End Sub
    Private Sub OpenProjectFromDevice()
        If ProceedWithUnsavedChanges() = False Then Return
        Dim timoutLenInSeconds As Integer = 10 '1

        'capture the timeout time for our timeout timer
        progComTimeoutTime = Now.AddSeconds(timoutLenInSeconds)
        'prep the flags for indicating when our message is received
        lastProgSingleParamSent = pStatEnum.NumberOfPrograms
        lastProgSingleParamReceived = 0
        requestParameter(pStatEnum.NumberOfPrograms)

        'wait for the data to be returned.
        While lastProgSingleParamSent <> lastProgSingleParamReceived
            If Now() > progComTimeoutTime Then
                MsgBox("Timed Out waiting for response.  [NumberOfPrograms]")
                Exit Sub
            End If
            Application.DoEvents()
        End While

        'We should now know the number of programs on the device.
        'lblProgsTotalNumProgs.Text = numPrograms - 1

        If numPrograms = 0 Then
            'No programs on device.  
            MsgBox("No Programs on Device")
            Exit Sub
        End If

        ''Prepare data storage for all the program data we'll retrieve soon:

        ''TODO: Alex: maybe use SetCurNumPrograms( [numPrograms received from device] )??
        ''OR--- find out what happens when we receive the single paramSet from the device that tells us how many progs it has.
        ''Maybe we don't need to do anything with storage here...

        'ReDim progName(numPrograms - 1)
        'ReDim numProgLines(numPrograms - 1)
        'For n As Integer = 0 To numPrograms - 1
        '    progName(n) = New String("")
        '    numProgLines(n) = 0
        'Next


        'Get the program names:
        For n As Integer = 0 To (numPrograms - 1)
            lastProgSingleParamSent = pStatEnum.ProgramName
            lastProgSingleParamReceived = 0
            progComTimeoutTime = Now.AddSeconds(timoutLenInSeconds)
            requestParameter(pStatEnum.ProgramName, n)

            While lastProgSingleParamSent <> lastProgSingleParamReceived
                If Now() > progComTimeoutTime Then
                    MsgBox("Timed Out waiting for response.  [Program Names(" & n & "]")
                    Exit Sub
                End If
                Application.DoEvents()
            End While
        Next

        'Get the number of lines for each program:
        For n As Integer = 0 To (numPrograms - 1)
            lastProgSingleParamSent = pStatEnum.ProgramLength
            lastProgSingleParamReceived = 0
            progComTimeoutTime = Now.AddSeconds(timoutLenInSeconds)
            requestParameter(pStatEnum.ProgramLength, n)

            While lastProgSingleParamSent <> lastProgSingleParamReceived
                If Now() > progComTimeoutTime Then
                    MsgBox("Timed Out waiting for response.  [Number of program lines(" & n & "]")
                    Exit Sub
                End If
                Application.DoEvents()
            End While
        Next

        'Now we're ready to get the program lines (one line at a time) from the device.
        For p As Integer = 0 To (numPrograms - 1)
            For l As Integer = 0 To (numProgLines(p) - 1)
                lastProgParamArraySent = paramArrayEnum.progLineData
                lastProgParamArraayReceived = 0
                progComTimeoutTime = Now.AddSeconds(timoutLenInSeconds)
                requestParamArray(paramArrayEnum.progLineData, p, l)

                While lastProgSingleParamSent <> lastProgSingleParamReceived
                    If Now() > progComTimeoutTime Then
                        MsgBox("Timed Out waiting for response.  [Get Prog(" & p & ") Line(" & l & ")")
                        Exit Sub
                    End If
                    Application.DoEvents()
                End While
            Next
        Next


        'Load the first program into the dgv:
        'TODO: finish this.
    End Sub

    Private Sub ClearAllProgramData()
        progTable = Nothing
        numProgLines = Nothing
        numPrograms = 0
        progName = Nothing
        ucProg.Clear()
        curProg = -1


        RemoveHandler cboProgNumSelect.SelectedValueChanged, AddressOf cboProgNumSelect_SelectedValueChanged
        cboProgNumSelect.Items.Clear()
        cboProgNumSelect.SelectedValue = Nothing
        'cboProgNumSelect.SelectedIndex = Nothing
        AddHandler cboProgNumSelect.SelectedValueChanged, AddressOf cboProgNumSelect_SelectedValueChanged
        lblProgsTotalNumProgs.Text = 0

    End Sub

    Private Sub SetCurrentNumberOfPrograms(ByVal newNum As Integer, ByVal ClearAllRegardless As Boolean)
        'dgvProg.Rows.Clear()
        If curProg >= newNum - 1 Then
            ucProg.Clear()
        End If


        'NOTE: newNum is 1-based and numPrograms is 1-based!
        If newNum < 1 Then
            ClearAllProgramData()
            Return
        End If

        'At least 1 program... Set up all data storage variables

        RemoveHandler cboProgNumSelect.SelectedValueChanged, AddressOf cboProgNumSelect_SelectedValueChanged
        cboProgNumSelect.Items.Clear()
        For n As Integer = 1 To (newNum)
            cboProgNumSelect.Items.Add(n & " (" & n - 1 & ")")
        Next
        lblProgsTotalNumProgs.Text = newNum
        cboProgNumSelect.SelectedValue = Nothing 'cboProgNumSelect.SelectedValue = tmpPrevSelected
        AddHandler cboProgNumSelect.SelectedValueChanged, AddressOf cboProgNumSelect_SelectedValueChanged

        'Make sure we have a reference to storage arrays:
        If numPrograms = 0 Then
            ReDim progTable(newNum - 1)
            ReDim progName(newNum - 1)
            ReDim numProgLines(newNum - 1)
        End If

        'configure the arrays for the new number of programs:
        'If newNum >= numPrograms Then
        If (newNum >= numPrograms) Or (ClearAllRegardless = True) Then
            Dim startIndx As Integer = If(ClearAllRegardless = True, 0, numPrograms)
            'For n As Integer = (numPrograms) To (newNum - 1)
            For n As Integer = (startIndx) To (newNum - 1)
                'add more tables
                ReDim Preserve progTable(n)
                AddNewProgramDataTable(n)
                ReDim Preserve progName(n)
                progName(n) = New String(" ")
                ReDim Preserve numProgLines(n)
                numProgLines(n) = 0
            Next
        Else
            ReDim Preserve progTable(newNum - 1)
            ReDim Preserve progName(newNum - 1)
            ReDim Preserve numProgLines(newNum - 1)
        End If

        numPrograms = newNum
        lblProgsTotalNumProgs.Text = numPrograms
    End Sub
    'Private Sub cmdCreateNewProgram_Click(sender As Object, e As EventArgs) Handles cmdCreateNewProgram.Click
    '    CreateNewProgram()
    'End Sub
    Private Sub CreateNewProgram()

        If numPrograms < 1 Then
            'We need to know how many programs are currently on the device so we can set our new program number to +1.
            If chkOffline.Checked = False Then
                OpenProjectFromDevice()
            End If
        End If

        If numPrograms < 1 Then numPrograms = 0
        SetCurrentNumberOfPrograms(numPrograms + 1, False) 'NOTE: This function expects a 1-based count of programs.  So "1" = 1 program.
        Dim newProgNumber = numPrograms - 1

        cboProgNumSelect.SelectedIndex = newProgNumber
        'add 1 row of data and then display it on the datagridview.
        AddDataRowToProgram(newProgNumber)

        'Dim unsavedProgChanges As Boolean = False
    End Sub

    Private Sub DeleteProgram(ByVal progNum As Integer)
        '1. Move all subsequent Array elements forward 1 element
        '2. Resize all arrays (-1)
        '3. Refresh the display

        'Disqualifiers:
        If IsNothing(progTable) Then Return
        If IsNothing(progTable(progNum)) Then Return
        If (progNum < 0) Or (progNum >= numPrograms) Then Return

        'Copy all array elements 1 position forward
        For n As Integer = progNum To numPrograms - 2
            progTable(n) = progTable(n + 1)
            progName(n) = progName(n + 1)
            numProgLines(n) = numProgLines(n + 1)
        Next

        'Resize
        SetCurrentNumberOfPrograms(numPrograms - 1, False)

        'Refresh display
        Dim tmpNum = progNum
        If tmpNum >= numPrograms Then
            tmpNum -= 1
        End If
        If tmpNum >= 0 Then
            cboProgNumSelect.SelectedIndex = tmpNum
        End If
    End Sub

    Private Sub MoveProgram(ByVal progNum As Integer, ByVal moveDirection As Direction)
        'This is effectively an element swap.
        '1. Create temp variable
        '2. Place the array element from the target location into the temp
        '3. Set the target element = source element
        '4. Set the source element = temp

        'Disqualifiers:
        If (moveDirection = Direction.TowardsStart) And (progNum < 1) Then Return
        If (moveDirection = Direction.TowardsEnd) And (progNum >= numPrograms - 1) Then Return

        'Create swap variables and capture the target element's data
        Dim tarNum As Integer = progNum + moveDirection

        Dim swapTable As DataTable = progTable(tarNum)
        Dim swapName As String = progName(tarNum)
        Dim swapNumLines As Integer = numProgLines(tarNum)

        'Move the source data
        progTable(tarNum) = progTable(progNum)
        progName(tarNum) = progName(progNum)
        numProgLines(tarNum) = numProgLines(progNum)

        'Restore the swap data to the source element
        progTable(progNum) = swapTable
        progName(progNum) = swapName
        numProgLines(progNum) = swapNumLines

        'Refresh the display
        cboProgNumSelect.SelectedIndex = tarNum

    End Sub


    Private Function AddDataRowToProgram(ByVal progNum As Integer, ByVal ParamArray vals() As String)
        Dim retVal = progTable(progNum).Rows.Count
        progTable(progNum).Rows.Add(vals)
        numProgLines(progNum) += 1
        If progNum = curProg Then
            ucProg.AddRow(vals)
        End If
        Return retVal
    End Function
    Public Function AddDataRowToProgram(ByVal progNum As Integer) As Integer
        'Add a row of data to the progTable specified by progNum.
        'Return the 0-based row number of the new row.
        Dim retVal = progTable(progNum).Rows.Count
        progTable(progNum).Rows.Add(retVal, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
        'dgvProg.Rows.Add()
        Dim srcRow As DataRow = progTable(progNum).Rows(retVal)
        'Dim tarRow As DataGridViewRow = dgvProg.Rows.Item(retVal)

        'FormatDGVRow(tarRow, srcRow.Item(0), srcRow.Item(1), srcRow.Item(2), srcRow.Item(3), srcRow.Item(4),
        '                 srcRow.Item(5), srcRow.Item(6), srcRow.Item(7), srcRow.Item(8),
        '                 srcRow.Item(9), srcRow.Item(10), srcRow.Item(11), srcRow.Item(12), srcRow.Item(13))
        ucProg.AddRow(srcRow.Item(0), srcRow.Item(1), srcRow.Item(2), srcRow.Item(3), srcRow.Item(4), srcRow.Item(5),
                      srcRow.Item(6), srcRow.Item(7), srcRow.Item(8), srcRow.Item(9), srcRow.Item(10),
                      srcRow.Item(11), srcRow.Item(12), srcRow.Item(13))
        numProgLines(progNum) += 1
        Return retVal
    End Function

    Public Sub MoveDataRowUpOrDown(ByVal progNum As Integer, ByVal rowNum As Integer, ByVal dirToMove As Direction)
        '1. Change the LineNum values for the two rows affected in the data table (the row being moved and the row previously
        '   sitting at the target location).   
        '2. "Sort" the dataTable.  
        '3. If this is the active program then reload the ucProg control. 

        'Disqualifiers:
        If IsNothing(progTable) Then Return
        If IsNothing(progTable(progNum)) Then Return
        If dirToMove = Direction.DontMove Then Return
        If (dirToMove = Direction.Up) And (rowNum < 1) Then Return
        If (dirToMove = Direction.Down) And (rowNum >= progTable(progNum).Rows.Count - 1) Then Return

        'Adjust lineNumbers:
        Dim srcRow As Integer = rowNum
        Dim tarRow As Integer = rowNum + dirToMove
        progTable(progNum).Rows(srcRow).Item("LineNum") = tarRow
        progTable(progNum).Rows(tarRow).Item("LineNum") = srcRow

        'Sort the data:
        progTable(progNum) = progTable(progNum).Select("", "LineNum ASC").CopyToDataTable

        'refresh the display
        If progNum = curProg Then
            If dirToMove = Direction.Up Then
                ucProg.MoveActiveRowUp()
            ElseIf dirToMove = Direction.Down Then
                ucProg.MoveActiveRowDown()
            End If
        End If
    End Sub

    Public Sub DeleteDataRow(ByVal progNum As Integer, ByVal rowNum As Integer)
        '1. Delete the row
        '2. Adjust the line numbers after the deletion to close the gap
        '3. Update the program's line count
        '4. Update the display

        'Disqualifiers:
        If IsNothing(progTable) Then Return
        If IsNothing(progTable(progNum)) Then Return
        If (progNum < 0) Or (progNum >= numPrograms) Then Return
        If (rowNum < 0) Or (rowNum >= progTable(progNum).Rows.Count) Then Return

        'Delete the row
        progTable(progNum).Rows(rowNum).Delete()
        progTable(progNum).AcceptChanges()

        'Adjust the lineNum of every row afterwards
        For n As Integer = rowNum To progTable(progNum).Rows.Count - 1
            progTable(progNum).Rows(n).Item("LineNum") -= 1
        Next

        'Update the line count
        numProgLines(progNum) -= 1

        'Update the display:
        If progNum = curProg Then
            ucProg.DeleteRow(rowNum)
        End If
    End Sub
    Public Sub InsertDataRow(ByVal progNum As Integer, ByVal rowNum As Integer)
        '1. Adjust the lineNum of every row past where the new line will end up (+1)
        '2. Create a new row at the end of the table (but with the correct lineNum)
        '3. Sort the data table (this will put the new line in the correct position)
        '4. Update the program's line count
        '5. Update the display

        'Disqualifiers:
        If IsNothing(progTable) Then Return
        If IsNothing(progTable(progNum)) Then Return
        If (progNum < 0) Or (progNum >= numPrograms) Then Return
        If (rowNum < 0) Or (rowNum >= progTable(progNum).Rows.Count) Then Return

        'Adjust the lineNum of every row past where the inserted row will be located
        '(including the current inhabitant of that destination row)
        For n As Integer = rowNum To progTable(progNum).Rows.Count - 1
            progTable(progNum).Rows(n).Item("LineNum") += 1
        Next

        'Create a new row at the end
        progTable(progNum).Rows.Add(rowNum, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)

        'Sort the data:
        progTable(progNum) = progTable(progNum).Select("", "LineNum ASC").CopyToDataTable

        'Update the line count
        numProgLines(progNum) += 1

        'Update the display:
        If progNum = curProg Then
            Dim tmpObj() As Object = progTable(progNum).Rows(rowNum).ItemArray
            Dim tmpVals(tmpObj.Length - 1) As String
            For n As Integer = 0 To tmpVals.Length - 1
                tmpVals(n) = tmpObj(n).ToString
            Next
            ucProg.InsertRow(rowNum, tmpVals)
        End If
    End Sub

    Public Sub AddNewProgramDataTable(ByVal newProgNum As Integer)
        'newProgNum is 0-based.

        'If IsNothing(progTable) Then
        '    ReDim progTable(newProgNum)
        'Else
        '    If newProgNum <= (progTable.Length - 1) Then
        '        If Not IsNothing(progTable(newProgNum)) Then
        '            'there is already a table for this progNumber.  Clear it so that a fresh table can be created below.
        '            progTable(newProgNum).Clear()
        '            progTable(newProgNum) = Nothing
        '        End If
        '    Else
        '        ReDim Preserve progTable(newProgNum)
        '    End If
        'End If
        If Not IsNothing(progTable) Then
            If Not IsNothing(progTable(newProgNum)) Then
                progTable(newProgNum).Clear()
            End If
        End If

        progTable(newProgNum) = New DataTable
        progTable(newProgNum).Columns.Add("LineNum", Type.GetType("System.Int32")) '0
        progTable(newProgNum).Columns.Add("Command", Type.GetType("System.Int32"))
        progTable(newProgNum).Columns.Add("Channel", Type.GetType("System.Int32")) '2
        progTable(newProgNum).Columns.Add("GoTo True", Type.GetType("System.Int32"))
        progTable(newProgNum).Columns.Add("GoTo False", Type.GetType("System.Int32")) '4
        progTable(newProgNum).Columns.Add("P81 Source", Type.GetType("System.Int32"))
        progTable(newProgNum).Columns.Add("P81 Val", Type.GetType("System.Int32")) '6
        progTable(newProgNum).Columns.Add("P82 Source", Type.GetType("System.Int32"))
        progTable(newProgNum).Columns.Add("P82 Val", Type.GetType("System.Int32")) '8
        progTable(newProgNum).Columns.Add("Polarity", Type.GetType("System.Int32"))
        progTable(newProgNum).Columns.Add("P321 Source", Type.GetType("System.Int32")) '10
        progTable(newProgNum).Columns.Add("P321 Val", Type.GetType("System.Int32"))
        progTable(newProgNum).Columns.Add("P322 Source", Type.GetType("System.Int32")) '12
        progTable(newProgNum).Columns.Add("P322 Val", Type.GetType("System.Int32")) '13
    End Sub


    'Private Sub cmdSaveChanges_Click(sender As Object, e As EventArgs) Handles cmdSaveChanges.Click
    '    SaveProjectToDevice()
    'End Sub
    Private Sub SaveProjectToDevice()
        'Download all program lines to the device.
        'Send data one line at a time and wait for an ack from the device before sending the next line.
        Dim timoutLenInSeconds = 3 '1
        Dim retryCounter = 0
        Dim maxRetries = 3


        'Initiate the download by sending the number of programs to the device.
        lastCommandTypeReceived = 0
        'capture the timeout time for our timeout timer
        progComTimeoutTime = Now.AddSeconds(timoutLenInSeconds)
        sendParameter(pStatEnum.NumberOfPrograms, numPrograms)

        'wait for the data to be returned.
        While lastCommandTypeReceived <> commandTypeEnum.ACK
            If lastCommandTypeReceived = commandTypeEnum.NAK Then
                retryCounter += 1
                If retryCounter <= maxRetries Then
                    'retry
                    progComTimeoutTime = Now.AddSeconds(timoutLenInSeconds)
                    lastCommandTypeReceived = 0
                    sendParameter(pStatEnum.NumberOfPrograms, numPrograms)
                End If
            End If
            If Now() > progComTimeoutTime Then
                MsgBox("Timed Out waiting for response from device.  [Initiate download: NumPrograms]")
                Exit Sub
            End If
            Application.DoEvents()
        End While

        'Send the program names:
        retryCounter = 0
        For n As Integer = 0 To (numPrograms - 1)
            Dim fullName As String = progName(n).PadRight(10)

            lastCommandTypeReceived = 0
            'capture the timeout time for our timeout timer
            progComTimeoutTime = Now.AddSeconds(timoutLenInSeconds)
            sendParameter(pStatEnum.ProgramName, fullName, n)

            'wait for the data to be returned.
            While lastCommandTypeReceived <> commandTypeEnum.ACK
                If lastCommandTypeReceived = commandTypeEnum.NAK Then
                    retryCounter += 1
                    If retryCounter <= maxRetries Then
                        'retry
                        progComTimeoutTime = Now.AddSeconds(timoutLenInSeconds)
                        lastCommandTypeReceived = 0

                        sendParameter(pStatEnum.ProgramName, fullName, n)
                    End If
                End If
                If Now() > progComTimeoutTime Then
                    MsgBox("Timed Out waiting for response from device.  [Program Name (" & n & "]")
                    Exit Sub
                End If
                Application.DoEvents()
            End While
        Next

        'Send the the number of lines for each program:
        retryCounter = 0
        For n As Integer = 0 To (numPrograms - 1)
            lastCommandTypeReceived = 0
            'capture the timeout time for our timeout timer
            progComTimeoutTime = Now.AddSeconds(timoutLenInSeconds)
            sendParameter(pStatEnum.ProgramLength, numProgLines(n), n)

            'wait for the data to be returned.
            While lastCommandTypeReceived <> commandTypeEnum.ACK
                If lastCommandTypeReceived = commandTypeEnum.NAK Then
                    retryCounter += 1
                    If retryCounter <= maxRetries Then
                        'retry
                        progComTimeoutTime = Now.AddSeconds(timoutLenInSeconds)
                        lastCommandTypeReceived = 0
                        sendParameter(pStatEnum.ProgramLength, numProgLines(n), n)
                    End If
                End If
                If Now() > progComTimeoutTime Then
                    MsgBox("Timed Out waiting for response from device.  [Number of progLines(" & n & "]")
                    Exit Sub
                End If
                Application.DoEvents()
            End While
        Next


        'Now we're ready to send the program lines (one line at a time) to the device.
        For p As Integer = 0 To (numPrograms - 1)
            For l As Integer = 0 To (numProgLines(p) - 1)
                lastCommandTypeReceived = 0
                'capture the timeout time for our timeout timer
                progComTimeoutTime = Now.AddSeconds(timoutLenInSeconds)
                sendParamArray(paramArrayEnum.progLineData, p, l)

                While lastCommandTypeReceived <> commandTypeEnum.ACK
                    If lastCommandTypeReceived = commandTypeEnum.NAK Then
                        retryCounter += 1
                        If retryCounter <= maxRetries Then
                            'retry
                            progComTimeoutTime = Now.AddSeconds(timoutLenInSeconds)
                            lastCommandTypeReceived = 0
                            sendParamArray(paramArrayEnum.progLineData, p, l)
                        End If
                    End If
                    If Now() > progComTimeoutTime Then
                        MsgBox("Timed Out waiting for response.  [Send Prog(" & p & ") Line(" & l & ")")
                        Exit Sub
                    End If
                    Application.DoEvents()
                End While
            Next
        Next

        MsgBox("Download Complete.")
        unsavedProgChanges = False
    End Sub

    Private Sub cboProgNumSelect_SelectedValueChanged(sender As Object, e As EventArgs) Handles cboProgNumSelect.SelectedValueChanged
        FillUCProgControl(cboProgNumSelect.SelectedIndex)
    End Sub
    Private Sub FillUCProgControl(ByVal progNum As Integer)
        'If unsavedProgChanges = True Then
        '    Dim tmpRes = MsgBox("Save Changes?", MsgBoxStyle.YesNoCancel, "Save Changes?")
        '    If tmpRes = MsgBoxResult.Yes Then

        '    ElseIf tmpRes = MsgBoxResult.Cancel Then
        '        'revert to the previously selected program:

        '    End If
        'End If

        'Load the program into ucProg user control
        curProg = progNum
        ucProg.Clear()

        For r As Integer = 0 To progTable(curProg).Rows.Count - 1 'tmpLineCount - 1 'progTable(t).Rows.Count - 1
            Dim srcRow As DataRow = progTable(curProg).Rows(r)
            ucProg.AddRow(srcRow.Item(0), srcRow.Item(1), srcRow.Item(2), srcRow.Item(3), srcRow.Item(4),
                         srcRow.Item(5), srcRow.Item(6), srcRow.Item(7), srcRow.Item(8),
                         srcRow.Item(9), srcRow.Item(10), srcRow.Item(11), srcRow.Item(12), srcRow.Item(13))
        Next

        RemoveHandler txtProgName.TextChanged, AddressOf txtProgName_TextChanged
        txtProgName.Text = progName(curProg)
        AddHandler txtProgName.TextChanged, AddressOf txtProgName_TextChanged
    End Sub

    Private Sub txtProgName_TextChanged(sender As Object, e As EventArgs) Handles txtProgName.TextChanged
        If txtProgName.Text.Length > 10 Then txtProgName.Text = txtProgName.Text.Substring(0, 10)
        If curProg >= 0 Then
            progName(curProg) = txtProgName.Text
            unsavedProgChanges = True
        End If
    End Sub


    Private Sub ImportProjectFileFromIni()
        If ProceedWithUnsavedChanges() = False Then Exit Sub

        If OpenFileDialogProject.ShowDialog() = DialogResult.Cancel Then Exit Sub

        If OpenFileDialogProject.FileName.Length = 0 Then Exit Sub

        'at this point we should have a file selected.  Read it.
        Dim lines() As String
        Dim strSection As String = "None"
        lines = File.ReadAllLines(OpenFileDialogProject.FileName)

        Dim tmpProgCount As Integer = 0
        Dim tmpExpectedLineCount As Integer() = {0}

        For Each txtline In lines
            If txtline.StartsWith("VERSION=") Then

            ElseIf txtline.Trim.StartsWith("PROGCOUNT=") Then
                txtline = txtline.Remove(0, txtline.IndexOf("=") + 1)
                If IsNumeric(txtline) Then
                    'tmpProgCount = CInt(txtline)
                    SetCurrentNumberOfPrograms(CInt(txtline), True)
                    If numPrograms < 1 Then
                        Exit Sub
                    End If
                    ReDim tmpExpectedLineCount(numPrograms - 1)
                Else
                    Exit Sub
                End If
            ElseIf txtline.Trim.StartsWith("NAMES=") Then
                txtline = txtline.Remove(0, txtline.IndexOf("=") + 1)
                If txtline.Length Then
                    Dim tmpName() As String = txtline.Split(",")
                    For n As Integer = 0 To progName.Length - 1
                        progName(n) = If(Not IsNothing(tmpName(n)), tmpName(n), " ")
                    Next
                End If
            ElseIf txtline.Trim.StartsWith("PROGLEN=") Then
                txtline = txtline.Remove(0, txtline.IndexOf("=") + 1)
                Dim tmpProgLen() As String = txtline.Split(",")
                If Not IsNothing(tmpExpectedLineCount) Then
                    If tmpProgLen.Length <> tmpExpectedLineCount.Length Then
                        'Error in the file.  The number of progLength entries doesn't match the number of programs specified.
                        MsgBox("Error 47.")
                        SetCurrentNumberOfPrograms(0, True)
                        Return
                    Else
                        For n As Integer = 0 To tmpExpectedLineCount.Length - 1
                            tmpExpectedLineCount(n) = If(Not IsNothing(tmpProgLen(n)), If(IsNumeric(tmpProgLen(n)), CInt(tmpProgLen(n)), 0), 0)
                        Next
                    End If
                End If
            ElseIf IsNumeric(txtline.Trim.Chars(0)) And (txtline.IndexOf("=") > 0) Then
                Dim strProgNum As String = txtline.Trim.Substring(0, txtline.IndexOf("="))
                If IsNumeric(strProgNum) And (CInt(strProgNum) < numPrograms) Then
                    Dim tmpCurProgNum As Integer = CInt(strProgNum)
                    txtline = txtline.Remove(0, txtline.IndexOf("=") + 1)
                    Dim tmpLineData() As String = txtline.Split(",")
                    AddDataRowToProgram(tmpCurProgNum, tmpLineData)
                End If
            End If
        Next

        'Check to make sure the line counts match:
        If numProgLines.Length <> tmpExpectedLineCount.Length Then
            'That ain't right.
            MsgBox("Error 48")
            SetCurrentNumberOfPrograms(0, True)
            Return
        Else
            For n As Integer = 0 To numProgLines.Length - 1
                If numProgLines(n) <> tmpExpectedLineCount(n) Then
                    'That ain't right either.
                    MsgBox("Error 49")
                    SetCurrentNumberOfPrograms(0, True)
                    Return
                End If
            Next
        End If

        'Set the current program to the first program retrieved from the project file.
        cboProgNumSelect.SelectedIndex = 0
    End Sub

    Private Sub ExportProjectFileAsInI()
        'Write all program data to a file.  This is essentially the same data that gets downloaded to the tens device.
        Dim result As Integer = -1

        If SaveFileDialogProject.ShowDialog() <> DialogResult.OK Then
            Exit Sub
        End If

        Dim writer As New StreamWriter(SaveFileDialogProject.FileName)

        'File Version
        Dim tmpLine As String = "VERSION=" & tensCurVersion
        writer.WriteLine(tmpLine)

        'Program Count
        tmpLine = "PROGCOUNT=" & numPrograms
        writer.WriteLine(tmpLine)

        'Program Names
        tmpLine = "NAMES="
        If numPrograms > 0 Then
            tmpLine &= progName(0)
            For n As Integer = 1 To numPrograms - 1
                tmpLine &= "," & progName(n)
            Next
        Else
            tmpLine &= ""
        End If
        writer.WriteLine(tmpLine)

        'Program(s) Length
        tmpLine = "PROGLEN="
        If numPrograms > 0 Then
            tmpLine &= numProgLines(0)
            For n As Integer = 1 To numPrograms - 1
                tmpLine &= "," & numProgLines(n)
            Next
        Else
            tmpLine &= "0"
        End If
        writer.WriteLine(tmpLine)

        'Program Lines
        writer.WriteLine("LINES")
        For p As Integer = 0 To numPrograms - 1
            For l As Integer = 0 To numProgLines(p) - 1
                tmpLine = p & "="
                For n As Integer = 0 To 14 - 1
                    If n > 0 Then tmpLine &= ","
                    Dim tmpVal As String = "0"
                    If Not IsNothing(progTable(p).Rows(l).Item(n)) Then
                        tmpVal = progTable(p).Rows(l).Item(n)
                    End If
                    tmpLine &= tmpVal
                Next
                writer.WriteLine(tmpLine)
            Next
        Next
        writer.Close()
    End Sub

    Private Sub ExportProjectAsXml()
        If SaveFileDialogProject.ShowDialog() <> DialogResult.OK Then
            Exit Sub
        End If

        Dim doc As XDocument =
            <?xml version="1.0" encoding="utf-8"?>
            <Root><FileInfo>Version="TENS2501" MotorChans="0,1" TensChans="2,3" SysChans="4"</FileInfo>
                <prog>ProgId="0" 
                    <line>LineData="0,1,5,0,0,4,45,0,54,5,3,65,1,56"</line>
                    <line>1,2,0,0,0,1,45,5,5,1,1,77,2,55</line>
                    <line>2,0,0,0,0,0,0,0,0,0,0,0,0,0</line>
                    <line>3,5,0,0,0,0,0,0,0,0,0,0,0,0</line>
                </prog>
            </Root>
        doc.Save(SaveFileDialogProject.FileName)
    End Sub


    Public Sub UpdateProgTableRow(ByVal progNum As Integer, ByVal rowNum As Integer, ByVal ParamArray vals() As String)
        With progTable(progNum).Rows(rowNum)
            For n As Integer = 0 To vals.Count - 1
                .Item(n) = vals(n)
            Next
        End With
    End Sub

    Public Sub UpdateProgTableRow(ByVal progNum As Integer, ByVal rowNum As Integer, ByVal newLine As Integer,
                                  ByVal newCmd As Integer, ByVal newChan As Integer, ByVal newGTT As Integer,
                                  ByVal newGTF As Integer, ByVal newP81S As Integer, ByVal newP81V As Integer,
                                  ByVal newP82S As Integer, ByVal newP82V As Integer, ByVal newPol As Integer,
                                  ByVal newP321S As Integer, ByVal newP321V As Integer, ByVal newP322S As Integer,
                                  ByVal newP322V As Integer)
        With progTable(progNum).Rows(rowNum)
            .Item(0) = newLine
            .Item(1) = newCmd
            .Item(2) = newChan
            .Item(3) = newGTT
            .Item(4) = newGTF
            .Item(5) = newP81S
            .Item(6) = newP81V
            .Item(7) = newP82S
            .Item(8) = newP82V
            .Item(9) = newPol
            .Item(10) = newP321S
            .Item(11) = newP321V
            .Item(12) = newP322S
            .Item(13) = newP322V
        End With
    End Sub

    Private Function GetStringIndex(ByVal strToFind As String, ByRef srcArray As String()) As Integer
        For n As Integer = 0 To srcArray.Length - 1
            If srcArray(n) = strToFind Then
                Return n
            End If
        Next
        Return -1
    End Function






    Private Sub UserControlPrograms1_ActiveRowChanged(NewActiveIndex As Integer, LastActiveIndex As Integer) Handles ucProg.ActiveRowChanged
        If (LastActiveIndex >= 0) And (LastActiveIndex < ucProg.RowCount) Then
            CommitUCProgChangesToTableData(LastActiveIndex)
        End If
    End Sub
    Private Sub ucProg_ProgramRow_Leave(LineNum As Integer) Handles ucProg.ProgramRow_Leave
        CommitUCProgChangesToTableData(LineNum)
    End Sub
    Private Sub CommitUCProgChangesToTableData(LineNum As Integer)
        If ucProg.Rows(LineNum).UnsavedChanges = True Then
            UpdateProgTableRow(curProg, LineNum, ucProg.Rows(LineNum).GetCellValues())
            ucProg.Rows(LineNum).UnsavedChanges = False
        End If
    End Sub






    Private Sub ucProg_reqAddRowToEnd() Handles ucProg.reqAddRowToEnd
        AddDataRowToProgram(curProg)
    End Sub

    Private Sub ucProg_reqDeleteRow(LineNum As Integer) Handles ucProg.reqDeleteRow
        DeleteDataRow(curProg, ucProg.ActiveRow)
    End Sub

    Private Sub ucProg_reqInsertRow(LineNum As Integer) Handles ucProg.reqInsertRow
        InsertDataRow(curProg, ucProg.ActiveRow)
    End Sub

    Private Sub ucProg_reqMoveRowDown(LineNum As Integer) Handles ucProg.reqMoveRowDown
        MoveDataRowUpOrDown(curProg, ucProg.ActiveRow, Direction.Down)
    End Sub

    Private Sub ucProg_reqMoveRowUp(LineNum As Integer) Handles ucProg.reqMoveRowUp
        MoveDataRowUpOrDown(curProg, ucProg.ActiveRow, Direction.Up)
    End Sub

    Private Sub OpenProjectFromComputerToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OpenProjectFromComputerToolStripMenuItem.Click
        ImportProjectFileFromIni()
    End Sub

    Private Sub CreateNewProjectToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CreateNewProjectToolStripMenuItem.Click
        If ProceedWithUnsavedChanges() = False Then Exit Sub
        'SetCurrentNumberOfPrograms(0) 'Clear all data
        SetCurrentNumberOfPrograms(1, True)
        AddDataRowToProgram(0)
        cboProgNumSelect.SelectedIndex = 0
        ucProg.ActiveRow = 0
    End Sub

    Private Sub SaveProjectToComputerToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveProjectToComputerToolStripMenuItem.Click
        ExportProjectFileAsInI()
    End Sub

    Private Sub GetProjectFromDeviceToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles GetProjectFromDeviceToolStripMenuItem.Click
        OpenProjectFromDevice()
    End Sub

    Private Sub SaveProjectToDeviceToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveProjectToDeviceToolStripMenuItem.Click
        SaveProjectToDevice()
    End Sub

    Private Sub AddNewProgramToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AddNewProgramToolStripMenuItem.Click
        CreateNewProgram()
    End Sub

    Private Sub ImportProgramFromFileToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ImportProgramFromFileToolStripMenuItem.Click

    End Sub

    Private Sub ExportCurrentProgramToFileToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExportCurrentProgramToFileToolStripMenuItem.Click

    End Sub

    Private Sub DeleteCurrentProgramToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DeleteCurrentProgramToolStripMenuItem.Click
        DeleteProgram(curProg)
    End Sub

    Private Sub MoveCurrentProgramTowardsBeginningToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MoveCurrentProgramTowardsBeginningToolStripMenuItem.Click
        MoveProgram(curProg, Direction.TowardsStart)
    End Sub

    Private Sub MoveCurrentProgramTowardsEndToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MoveCurrentProgramTowardsEndToolStripMenuItem.Click
        MoveProgram(curProg, Direction.TowardsEnd)
    End Sub

    Private Sub AddNewRowToEndToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AddNewRowToEndToolStripMenuItem.Click
        AddDataRowToProgram(curProg)
    End Sub

    Private Sub DeleteActiveRowToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DeleteActiveRowToolStripMenuItem.Click
        DeleteDataRow(curProg, ucProg.ActiveRow)
    End Sub

    Private Sub InsertNewRowToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles InsertNewRowToolStripMenuItem.Click
        InsertDataRow(curProg, ucProg.ActiveRow)
    End Sub

    Private Sub MoveActiveRowUpToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MoveActiveRowUpToolStripMenuItem.Click
        MoveDataRowUpOrDown(curProg, ucProg.ActiveRow, Direction.Up)
    End Sub

    Private Sub MoveActiveRowDownToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MoveActiveRowDownToolStripMenuItem.Click
        MoveDataRowUpOrDown(curProg, ucProg.ActiveRow, Direction.Down)
    End Sub

    Private Sub chkOffline_CheckedChanged(sender As Object, e As EventArgs) Handles chkOffline.CheckedChanged
        If chkOffline.Checked = True Then
            TabControlMain.Enabled = True

        End If
    End Sub










#End Region

End Class
