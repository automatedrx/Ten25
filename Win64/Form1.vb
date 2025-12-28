Imports System.ComponentModel
Imports System.Drawing.Text
Imports System.IO
Imports System.IO.Ports
Imports System.Net
Imports System.Reflection
Imports System.Runtime.Intrinsics
Imports System.Windows.Forms.AxHost
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports Tens2502.comDef
Imports Tens2502.My

Public Class Form1

    Public Enum eAutoStatus As Integer
        asOff = 0
        asOn = 1
        asPaused = 2
    End Enum


    '==Device==
    Dim editorDev As Device_t
    Dim monitorDev As Device_t

    Dim uploadInProgress As Boolean = False
    Dim downloadInProgress As Boolean = False
    Dim updownAttemptCount As Integer = 0


    Dim validDevice As Boolean = False  'Flag that indicates true when a successful response has been received from a tens device
    'Dim radioId As Integer = -1
    'Dim devNumVarsPerProgram = 50 ' 20
    'Dim devNumTimersPerProgram = 50 '20

    'Dim devDigInCount As Integer = -1
    'Dim devDigOutCount As Integer = -1

    Dim devNumProgramsLastReported As Integer = 0   'This is only used during program download from device.
    Dim devTotalLines As Integer = 0    'This is only used for status reporting during upload/download.

    '==Communications==
    Dim WithEvents ComPort As New System.IO.Ports.SerialPort
    Dim rxBuff As String = ""
    Dim newMsgReceived As Boolean = False

    Dim autoStatus As eAutoStatus = eAutoStatus.asOff
    Dim autoStatusLast As eAutoStatus = eAutoStatus.asOff
    Dim nextParamArrayAutoStatus = 0
    Dim lastChanStatsReceivedTime As DateTime = Now

    Dim currentDownloadingDevice As Device_t

    Dim lastCommandTypeReceived As Integer
    Dim lastProgSingleParamSent As Integer
    Dim lastProgSingleParamReceived As Integer
    Dim lastProgParamArraySent As Integer
    Dim lastProgParamArrayReceived As Integer


    '==Channel Controls==
    Dim chanControls(1) As ucChanControl
    Dim chanControlProg As New List(Of Program) 'This is only used by channel controls so they can update the current program names, line numbers, etc.
    Dim tmpMinSpeed As Integer = 20
    Dim tmpMaxSpeed As Integer = 500


    Dim autoCommitProgLineChanges As Boolean = True 'Maybe make a my.settings for this entry?

    '==Programs==
    Friend Event UnsavedProgramChanges(ByRef Dev As Device_t, ByVal newState As Boolean)
    Private Event UndownloadedProgramChanges(ByRef Dev As Device_t, ByVal newState As Boolean)
    Private Event RequestAutostatusStateChange(newState As eAutoStatus)

    Dim progLineEditEnabled As Boolean = True


    '=== Offline Simulator ===
    Private Enum eSimState
        Stopped
        Running
        Paused
        Stepping  ' One step requested while paused
    End Enum

    Private simState As eSimState = eSimState.Stopped
    Private simChannelStates() As ChannelSimState  ' One per channel
    Private tmrSim As New Timer With {.Interval = 50}  ' 20 Hz simulation tick
    Private simBreakpoints As New HashSet(Of Integer)  ' Line numbers with breakpoints (0-based)

    Private Class ChannelSimState
        Public CurProgNum As Integer = 0
        Public CurLineNum As Integer = 0
        Public Variables() As Integer  ' Size = NumVarsPerProgram
        Public Timers() As Integer     ' Size = NumTimersPerProgram, in ms
        Public TimerStartTicks() As Integer  ' When timer started
        Public TimerRunning() As Boolean
        Public ProgState As Integer = 0
        Public RepeatsRemaining As Integer = 0
        Public OutputActive As Boolean = False
        Public OutputStartTime As Integer = 0
        Public OutputDuration As Integer = 0
        Public OutputStartVal As Integer = 0
        Public OutputEndVal As Integer = 0
        Public OutputCurVal As Integer = 0
        Public DelayDuration As Integer = 0

        Public OutputMaxPulsewidthPercent As Integer = 50


        Public Speed As Integer = 100
        Public Polarity As ePolarity = ePolarity.polForward
        Public PolaritySwapped As Boolean = False
        Public TensIntensityCurLevel As Integer = 20
        Public TensIntensityMinLevel As Integer = 0
        Public TensIntensityMaxLevel As Integer = 40

    End Class










    Private Class JsonData
        Public Property Dev As Device_t
    End Class

    Private Sub CreateNewProject(ByRef Dev As Device_t, Optional numProgramsToCreate As Integer = 1, Optional RefreshDisplay As Boolean = True)
        'MsgBox("Set up a dialog to select what devicetype you're creating a new project for.")

        Dim tmpNewDeviceType As DeviceType_t = Settings.DefaultNewDeviceType '= DeviceType_t.Tens2410E1
        If tmpNewDeviceType < 0 Then
            Dim tmpDevTypeSelector As New DialogDeviceType
            If tmpDevTypeSelector.ShowDevTypeDialog(Settings.LastNewDeviceType) = MsgBoxResult.Cancel Then
                Exit Sub
            End If
            tmpNewDeviceType = tmpDevTypeSelector.SelectedDeviceType
            Settings.DefaultNewDeviceType = tmpDevTypeSelector.DefaultSelection
            Settings.LastNewDeviceType = tmpDevTypeSelector.SelectedDeviceType
            Settings.Save()
            tmpDevTypeSelector.Close()
            tmpDevTypeSelector.Dispose()
        End If

        Dev = GetDeviceTemplate(tmpNewDeviceType)

        For n As Integer = 1 To numProgramsToCreate
            AddBlankProgram(Dev)
        Next
        lstProgDisplay.Items.Clear()
        lstProgDisplay.SelectedIndex = -1
        'curProg = 0
        If RefreshDisplay = True Then
            InitProgLineEditControl()
            PopulateProgramListDisplay()
            AddHandler lstProgDisplay.SelectedIndexChanged, AddressOf lstProgDisplay_SelectedIndexChanged
            lstPrograms.SelectedIndex = 0 'Dev.CurProgNum
        End If

        'unsavedProgChanges = False
        RaiseEvent UnsavedProgramChanges(Dev, False) ' It's a brand new project, no changes have been made to it yet.
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles Me.Load
        RefreshComPortList()
        comportIsOpen(False)
        CreateNewProject(editorDev)
        InitializeDebugTabs()
        UpdateDebugButtonStates()
    End Sub


#Region "ComPort"

    Private Sub cmdRefreshComportList_Click_1(sender As Object, e As EventArgs) Handles cmdRefreshComportList.Click
        RefreshComPortList()
    End Sub

    Public Sub RefreshComPortList()
        If IsNothing(cboComPorts) Then
            cboComPorts = New ToolStripComboBox()
        Else
            cboComPorts.Items.Clear()
        End If
        Dim availPorts As String() = SerialPort.GetPortNames
        Array.Sort(availPorts)
        cboComPorts.Items.Clear()
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

    Private Sub cmdConnect_Click_1(sender As Object, e As EventArgs) Handles cmdConnect.Click
        If ComPort.IsOpen = True Then
            CloseComPort()
        Else
            OpenComPort()
            requestParameter(pStatEnum.TensProgCurVer)
        End If
    End Sub
    Private Sub cmdResetDevice_Click(sender As Object, e As EventArgs) Handles cmdResetDevice.Click
        If ComPort.IsOpen = False Then
            OpenComPort()
            If ComPort.IsOpen = True Then
                Dim dataBuff As String
                dataBuff = Chr(eCommandType.ResetDevice) & "=YES" & Chr(10)
                dataSend(dataBuff, dataBuff.Length)
                CloseComPort()
            End If
        End If
    End Sub

    Private Sub cmdEraseDevice_Click(sender As Object, e As EventArgs) Handles cmdEraseDevice.Click
        If ComPort.IsOpen = False Then
            OpenComPort()
            If ComPort.IsOpen = True Then
                Dim dataBuff As String
                dataBuff = Chr(eCommandType.EraseDevice) & "=YES" & Chr(10)
                dataSend(dataBuff, dataBuff.Length)
                CloseComPort()
            End If
        End If
    End Sub
    Private Sub cmdStop_Click(sender As Object, e As EventArgs) Handles cmdStop.Click
        Chan_Enabled_Click(0, False)
    End Sub

    Private Sub OpenComPort()
        If SerialPort.GetPortNames.Contains(cboComPorts.Text) Then
            ComPort.PortName = cboComPorts.Text
        Else
            RefreshComPortList()
            Exit Sub
        End If

        Try
            ComPort.Open()
            ComPort.DiscardInBuffer()
            ComPort.DiscardOutBuffer()
            rxBuff = ""
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
            'cmdConnect.BackColor = ControlColorNone
            lblDevStatus.Text = "Initiating connection to a device..."
            cmdConnect.Text = "Disconnect"
            cboComPorts.Enabled = False
            cmdRefreshComportList.Enabled = False
            tmrCheckComportStatus.Enabled = True
            UpdateDebugButtonStates()
        Else
            'cmdConnect.BackColor = ControlColorNone
            tmrCheckComportStatus.Enabled = False
            lblDevStatus.Text = "No device connected."
            cmdConnect.Text = "Connect"
            cboComPorts.Enabled = True
            cmdRefreshComportList.Enabled = True
            validDevice = False 'reset the flag so that when a comport is opened again, the program will initiate communications.
        End If
    End Sub

    Private Sub UpdateDebugButtonStates()
        Dim enabled As Boolean

        If tsbtnRealOrSimulated.Checked Then
            ' Simulator mode — always enable debug controls
            enabled = True
        Else
            ' Device mode — only when connected and valid
            enabled = ComPort.IsOpen AndAlso validDevice
        End If

        tsbtnDebugRun.Enabled = enabled AndAlso simState <> eSimState.Running
        tsbtnDebugPause.Enabled = enabled AndAlso simState = eSimState.Running
        tsbtnDebugStep.Enabled = enabled AndAlso (simState = eSimState.Paused OrElse simState = eSimState.Stopped)
    End Sub
    Private Sub tmrCheckComportStatus_Tick(sender As Object, e As EventArgs) Handles tmrCheckComportStatus.Tick
        'This timer monitors the comport for any unplanned disconnection (device unplugged, powered off, etc.).

        If ComPort.IsOpen = False Then
            tmrCheckComportStatus.Enabled = False
            CloseComPort()
        End If
    End Sub

    Private Sub ComPort_DataReceived(sender As Object, e As SerialDataReceivedEventArgs) Handles ComPort.DataReceived
        Dim rxData As String = ComPort.ReadExisting
        rxBuff &= rxData
        If rxBuff.Contains(Chr(10)) Then
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
                SetAutoStatusMode(eAutoStatus.asOff)
            End Try
        Else
            RaiseEvent RequestAutostatusStateChange(eAutoStatus.asOff)
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
            newMsgReceived = True
        End If
        tmrNewMessages.Enabled = True
    End Sub


#End Region

#Region "AutoStatus"
    Private Sub SetAutoStatusMode(ByVal newMode As eAutoStatus)
        If Me.InvokeRequired Then
            Me.Invoke(Sub()
                          SetAutoStatusMode(newMode)
                      End Sub)
        Else
            If newMode = eAutoStatus.asOff Then
                autoStatus = eAutoStatus.asOff
            ElseIf validDevice = True Then
                autoStatus = newMode
            Else
                autoStatus = eAutoStatus.asOff
            End If

            Select Case autoStatus
                Case eAutoStatus.asOff
                    cmdtsChanMonAutoStatusOff.Checked = True
                    cmdtsChanMonAutoStatusOn.Checked = False
                Case eAutoStatus.asOn
                    cmdtsChanMonAutoStatusOff.Checked = False
                    cmdtsChanMonAutoStatusOn.Checked = True
                    nextParamArrayAutoStatus = 0
                    requestParamArray(eParamArray.chanStats, nextParamArrayAutoStatus)
            End Select

            UpdateDebugButtonStates()
        End If
    End Sub

    Private Sub Form1_RequestAutostatusStateChange(newState As eAutoStatus) Handles Me.RequestAutostatusStateChange
        SetAutoStatusMode(newState)
    End Sub

    Private Sub cmdtsChanMonAutoStatusOff_Click_1(sender As Object, e As EventArgs) Handles cmdtsChanMonAutoStatusOff.Click
        SetAutoStatusMode(eAutoStatus.asOff)
        My.Settings.Item("lastAutoStatus") = CInt(eAutoStatus.asOff)
        tmrCheckAutostatus.Enabled = False
    End Sub

    Private Sub cmdtsChanMonAutoStatusOn_Click(sender As Object, e As EventArgs) Handles cmdtsChanMonAutoStatusOn.Click
        If uploadInProgress Or downloadInProgress Then
            cmdtsChanMonAutoStatusOff.PerformClick()
            Exit Sub
        End If
        SetAutoStatusMode(eAutoStatus.asOn)
        My.Settings.Item("lastAutoStatus") = CInt(eAutoStatus.asOn)
        tmrCheckAutostatus.Enabled = True
    End Sub

    Private Sub tmrCheckAutostatus_Tick(sender As Object, e As EventArgs) Handles tmrCheckAutostatus.Tick
        'This timer checks to see if autostatus has stalled out.  If so, it sends a paramArray request to the device to try
        'to restart communications.
        If autoStatus = eAutoStatus.asOn Then
            If Now > lastChanStatsReceivedTime.AddSeconds(2) Then
                nextParamArrayAutoStatus = 0
                requestParamArray(eParamArray.chanStats, nextParamArrayAutoStatus)
            End If
        Else
            tmrCheckAutostatus.Enabled = False
        End If
    End Sub

#End Region

#Region "Device Communications"

    Private Sub parseRxMessage()
        Dim eomIndx As Integer = rxBuff.IndexOf(Chr(10))
        Dim msg As String = rxBuff.Substring(0, eomIndx)
        Dim val As String = ""
        rxBuff = rxBuff.Remove(0, eomIndx + 1)

        Dim cmd As eCommandType = Asc(msg.Chars(0))
        Dim param As pStatEnum = pStatEnum.None
        If msg.Length > 1 Then
            param = Asc(msg.Chars(1))
        End If

        Dim eqIndex = msg.IndexOf("=")
        Dim paramIndex() As Integer = {-1, -1, -1, -1, -1}
        Dim paramIndexCount As Integer = 0
        Dim tmpIndexLen = eqIndex - 2       ' jb32=xyz

        Dim strParamIndex As String = ""
        If (msg.Length > 2) And (tmpIndexLen > 0) Then
            strParamIndex = msg.Substring(2, tmpIndexLen)
        End If
        If strParamIndex.Length Then
            Dim strParamIndexArray As String() = strParamIndex.Split(",")
            For n As Integer = 0 To strParamIndexArray.Length - 1
                If IsNumeric(strParamIndexArray(n)) Then
                    paramIndex(n) = CInt(strParamIndexArray(n))
                    paramIndexCount += 1
                Else
                    Exit Sub
                End If
            Next
        End If


        If eqIndex > 0 Then
            val = msg.Substring(eqIndex + 1, msg.Length - eqIndex - 1)
        End If

        lastCommandTypeReceived = cmd

        Select Case cmd
            Case eCommandType.ACK

            Case eCommandType.NAK

            Case eCommandType.SetParamArray        '//Followed by an index num
                setParamArray(param, val, paramIndex)
            Case eCommandType.SetSingleParam       '//Followed by a pStatEnum
                setSingleParam(param, val, paramIndex)
            Case eCommandType.GetParamArray        '//Followed by an index num

            Case eCommandType.GetSingleParam       '//Followed by a pStatEnum

            Case eCommandType.SetFileData

            Case eCommandType.GetFileData

            Case eCommandType.ResetDevice

            Case eCommandType.EraseDevice

        End Select
    End Sub
    Private Sub setSingleParam(ByVal param As pStatEnum, ByVal val As String, ByVal paramIndex() As Integer)
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
            Case pStatEnum.NumProgVariables
            Case pStatEnum.NumProgTimers
            Case pStatEnum.TensProgCurVer
                If val.Contains(tensCurVersion) Then
                    'correct version!
                    validDevice = True

                    My.Settings.Item("lastComPort") = cboComPorts.Text
                    monitorDev = New Device_t
                    OpenProjectFromDevice(monitorDev)
                Else
                    Dim tmpText As String = "Sorry, This software only communicates with devices running version:" & tensCurVersion & "."
                    tmpText &= vbCrLf & "The connected device is running version:" & val.ToString & "."
                    tmpText &= vbCrLf & "Unable to proceed."
                    MsgBox(tmpText, MsgBoxStyle.ApplicationModal, "Wrong SW Version")
                    CloseComPort()
                End If
            Case pStatEnum.TensProgMinVer

            Case pStatEnum.ChanType
            Case pStatEnum.ChanEnabled
            Case pStatEnum.ProgNumber
            Case pStatEnum.ProgState
            Case pStatEnum.CurLineNum
            Case pStatEnum.ChanCurIntensityPct
            Case pStatEnum.CurSpeed
            Case pStatEnum.MinIntensity
            Case pStatEnum.MaxIntensity
            Case pStatEnum.SwapPolarity

            Case pStatEnum.MaxPulsewidthOutputPct

            Case pStatEnum.NumberOfPrograms

            Case pStatEnum.ProgramLength
                'Set up the specified program with the specified number of blank program lines
                Dim tmpProgNum As Integer = paramIndex(0)
                Dim tmpNumLines As Integer = CInt(val)
                If (tmpProgNum >= 0) And (tmpProgNum < monitorDev.Prog.Count) Then
                    devTotalLines += tmpNumLines
                    monitorDev.Prog(tmpProgNum).progLine.Clear()
                    AddBlankProgramLine(monitorDev.Prog(tmpProgNum), tmpProgNum = monitorDev.CurProgNum, tmpNumLines)
                End If

            Case pStatEnum.ProgramName
                If (paramIndex(0) >= 0) And (monitorDev.Prog.Count > paramIndex(0)) Then

                    Dim tmpProg As Program = monitorDev.Prog.Item(paramIndex(0))
                    tmpProg.progName = val.PadRight(10)
                    monitorDev.Prog(paramIndex(0)) = tmpProg
                End If

        End Select
    End Sub
    Private Sub setParamArray(ByVal pArrayNum As eParamArray, ByVal val As String, ByVal paramIndex() As Integer)
        lastProgParamArrayReceived = pArrayNum

        'Split the val into an array of values
        Dim valArray As String() = val.Split(",")

        Select Case pArrayNum
            Case eParamArray.DeviceInfo
                '// DeviceType, RadioId, NumberOfPrograms, DeviceName, SerialNumber
                If valArray.Length = 5 Then
                    monitorDev = GetDeviceTemplate(CInt(valArray(0)))
                    monitorDev.RadioId = CInt(valArray(1))
                    devNumProgramsLastReported = CInt(valArray(2))  'This is used during the OpenProjectFromDevice background thread.
                    monitorDev.Name = valArray(3)
                    monitorDev.serialNumber = valArray(4)
                    lblDevStatus.Text = "Connected: " & monitorDev.Name & vbTab & "RadioId: " & monitorDev.RadioId &
                        vbTab & "model:" & monitorDev.DeviceType & vbTab & "sn:" & monitorDev.serialNumber
                    AddBlankProgram(monitorDev, devNumProgramsLastReported)
                    initChannels()
                End If
            Case eParamArray.chanMinMaxInfo
                '// minSpeed, maxSpeed, minIntensity, maxIntensity
                Dim tmpChan As Channel_t = monitorDev.Channel(paramIndex(0))
                tmpChan.SpeedMin = CInt(valArray(0))
                tmpChan.SpeedMax = CInt(valArray(1))
                monitorDev.Channel(paramIndex(0)) = tmpChan

                chanControls(paramIndex(0)).MinSpeed = tmpChan.SpeedMin 'CInt(valArray(0))
                chanControls(paramIndex(0)).MaxSpeed = tmpChan.SpeedMax
                chanControls(paramIndex(0)).OutputIntensityMin = CInt(valArray(2))
                chanControls(paramIndex(0)).OutputIntensityMax = CInt(valArray(3))

            Case eParamArray.chanStats
                '// chan0-3
                '// 0 chanEnabled, 1 chanSpeed, 2 curLineNum, 3 startVal, 4 endVal, 5 modDuration,
                '// 6 percentComplete, 7 RepeatsRemaining, 8 chanCurVal, 9 chanCurIntensity,
                '// 10 progState, 11 polaritySwapped, 12 polarity, 13 curProgNumber, 14 minIntensity, 15 maxIntensity, 16 maxOutputPulsewidthPct

                Dim tmpCurProgNum As Integer = CInt(valArray(13))
                Dim tmpCurLineNum As Integer = CInt(valArray(2))

                'If IsNothing(chanControls(paramIndex(0)).ProgRef) Then
                '    chanControls(paramIndex(0)).ProgRef = monitorDev.Prog
                'End If
                If IsNothing(chanControls(paramIndex(0)).GetProgRef()) Then
                    chanControls(paramIndex(0)).SetProgRef(monitorDev.Prog)
                End If

                If Not IsNothing(chanControls(paramIndex(0))) Then
                    Dim tmpProg As List(Of Program) = chanControls(paramIndex(0)).GetProgRef
                    If tmpProg.Count > tmpCurProgNum Then
                        If tmpProg(tmpCurProgNum).progLine.Count > tmpCurLineNum Then
                            chanControls(paramIndex(0)).ProgNum = tmpCurProgNum
                            chanControls(paramIndex(0)).LineNum = tmpCurLineNum
                        End If
                    End If
                End If


                chanControls(paramIndex(0)).ChanEnabled = If(valArray(0) = "1", True, False)
                chanControls(paramIndex(0)).Speed = CInt(valArray(1))
                chanControls(paramIndex(0)).PulseWidthStart = CInt(valArray(3))
                chanControls(paramIndex(0)).PulseWidthEnd = CInt(valArray(4))
                chanControls(paramIndex(0)).Duration = CInt(valArray(5))
                '//==pulseWidth must be set BEFORE percentComplete!
                chanControls(paramIndex(0)).PulseWidth = CInt(valArray(8))
                chanControls(paramIndex(0)).PercentComplete = CInt(valArray(6))
                '//==
                chanControls(paramIndex(0)).RepeatsRemaining = CInt(valArray(7))

                chanControls(paramIndex(0)).OutputIntensityPct = CInt(valArray(9))
                chanControls(paramIndex(0)).ProgState = CInt(valArray(10))
                chanControls(paramIndex(0)).PolaritySwapped = If(CInt(valArray(11)) = 1, True, False)
                chanControls(paramIndex(0)).Polarity = CInt(valArray(12))

                chanControls(paramIndex(0)).OutputIntensityMin = CInt(valArray(14))
                chanControls(paramIndex(0)).OutputIntensityMax = CInt(valArray(15))
                chanControls(paramIndex(0)).MaxOutputPulsewidthPct = CInt(valArray(16))

                lastChanStatsReceivedTime = Now
                If autoStatus = eAutoStatus.asOn Then
                    nextParamArrayAutoStatus = paramIndex(0) + 1
                    If nextParamArrayAutoStatus >= monitorDev.Channel.Count Then
                        nextParamArrayAutoStatus = 0
                    End If
                    requestParamArray(eParamArray.chanStats, nextParamArrayAutoStatus)
                End If

            Case eParamArray.progLineData
                Dim tmpProg = paramIndex(0)
                Dim tmpLine = paramIndex(1)
                If (tmpProg >= 0) And (tmpProg < monitorDev.Prog.Count) Then
                    If (tmpLine >= 0) And (tmpLine < monitorDev.Prog(tmpProg).progLine.Count) Then
                        'Need to "remove" the first element (line number) from valArray for the paramArray we pass to the program list.
                        Dim shortValArray(valArray.Length - 2) As Integer
                        For c As Integer = 1 To (valArray.Length - 1)
                            shortValArray(c - 1) = CInt(valArray(c))
                        Next
                        monitorDev.Prog(tmpProg).progLine(tmpLine) = shortValArray
                    End If

                    'If this is the final line of the final program, update the program list:
                    If (tmpProg = (monitorDev.Prog.Count - 1)) And (tmpLine = (monitorDev.Prog(tmpProg).progLine.Count - 1)) Then
                        RemoveHandler lstPrograms.SelectedIndexChanged, AddressOf lstPrograms_SelectedIndexChanged
                        PopulateProgramListDisplay()
                        lstPrograms.SelectedIndex = 0
                    End If
                End If

            Case eParamArray.chanDebugData
                '// 0 curProgNum, 1 curLineNum, 2 progState, 3 number of program variables, 4 number of program timers,
                '// 5 variable values,  5 + (number of program variables * 4) start of timer values


        End Select
    End Sub
    Private Sub sendParameter(ByVal param As pStatEnum, ByVal newVal As Integer, Optional ByVal index As Integer = -1)
        sendParameter(param, newVal.ToString, index)
    End Sub
    Private Sub sendParameter(ByVal param As pStatEnum, ByVal newVal As String, Optional ByVal index As Integer = -1)
        Dim dataBuff As String
        dataBuff = Chr(eCommandType.SetSingleParam) & Chr(param)

        If index >= 0 Then
            dataBuff &= index
        End If
        dataBuff &= "=" & newVal & Chr(10)
        dataSend(dataBuff, dataBuff.Length)
    End Sub
    Private Sub requestParameter(ByVal param As pStatEnum, Optional ByVal index As Integer = -1)
        Dim dataBuff As String
        dataBuff = Chr(eCommandType.GetSingleParam) & Chr(param)

        If index >= 0 Then
            dataBuff &= index
        End If
        dataBuff &= Chr(10)
        dataSend(dataBuff, dataBuff.Length)
    End Sub
    Private Sub sendParamArray(ByRef Dev As Device_t, ByVal paNum As eParamArray, Optional ByVal paramIndex1 As Integer = -1,
                               Optional ByVal paramIndex2 As Integer = -1)
        Dim dataBuff As String
        dataBuff = Chr(eCommandType.SetParamArray) & Chr(paNum)

        Select Case paNum
            Case eParamArray.progLineData
                'paramIndex1 = prog number, paramIndex2 = line number
                If (paramIndex1 < 0) Or (paramIndex1 >= Dev.Prog.Count) Then Return
                If (paramIndex2 < 0) Or (paramIndex2 >= Dev.Prog(paramIndex1).progLine.Count) Then Return

                dataBuff &= paramIndex1 & "," & paramIndex2 & "=" & paramIndex2 & ","
                Dim tmpLine(DataFieldLen) As Integer
                tmpLine = Dev.Prog(paramIndex1).progLine(paramIndex2)
                For n As Integer = 0 To DataFieldLen - 1
                    If n > 0 Then dataBuff &= ","
                    dataBuff &= tmpLine(n)
                Next
            Case Else
                Return
        End Select

        dataBuff &= Chr(10)
        dataSend(dataBuff, dataBuff.Length)
    End Sub
    Private Sub requestParamArray(ByVal paNum As eParamArray, Optional ByVal paramIndex As Integer = -1,
                                  Optional ByVal index2 As Integer = -1)
        Dim dataBuff As String
        dataBuff = Chr(eCommandType.GetParamArray) & Chr(paNum)
        If paramIndex > -1 Then
            dataBuff &= paramIndex
            If index2 > -1 Then
                dataBuff &= "," & index2
            End If
        End If
        dataBuff &= Chr(10)
        dataSend(dataBuff, dataBuff.Length)
    End Sub

#End Region

#Region "Channel Controls"
    Private Sub initChannels()
        'remove any existing channel controls:
        For x As Integer = 0 To chanControls.Length - 1
            If Not IsNothing(chanControls(x)) Then
                chanControls(x).Dispose()
            End If
        Next

        Dim tmpControlWithTensW As Integer = UcChanControl1.WidthWithTens
        Dim tmpControlWithoutTensW As Integer = UcChanControl1.WidthWithoutTens
        Dim tmpControlH As Integer = UcChanControl1.Height
        Dim tmpPadding As Integer = 5

        'create new controls:
        Dim tmpNextX As Integer = 0
        ReDim chanControls(monitorDev.Channel.Count - 1)
        For n As Integer = 0 To (monitorDev.Channel.Count - 1)
            Dim tmpX As Integer = tmpPadding
            If n > 0 Then
                tmpX += chanControls(n - 1).Left + chanControls(n - 1).Width
            End If
            Dim tmpY As Integer = tmpPadding

            chanControls(n) = New ucChanControl With {.ChanIndex = n,
                        .ChanType = monitorDev.Channel(n).Type,
                        .ChanEnabled = False,
                        .Location = New Point(tmpX, tmpY),
                        .MinSpeed = tmpMinSpeed, .MaxSpeed = tmpMaxSpeed, .Speed = 100,
                        .ChanName = monitorDev.allChannelsString(n),
                        .Duration = 0,
                        .OutputIntensityMax = My.Settings.TensMaxOutputLow,
                        .MaxOutputPulsewidthPct = My.Settings.TensMaxPulsewidthOutputPctInitial,
                        .ProgState = 0,
                        .PulseWidth = 0, .PulseWidthStart = 0, .PulseWidthEnd = 0,
                        .Polarity = -1}

            AddHandler chanControls(n).Enabled_Click, AddressOf Chan_Enabled_Click
            AddHandler chanControls(n).Speed_Changed, AddressOf Chan_Speed_Changed
            AddHandler chanControls(n).OutputIntensity_Changed, AddressOf Chan_OutputIntensity_Changed
            AddHandler chanControls(n).IntensityMin_Changed, AddressOf Chan_IntensityMin_Changed
            AddHandler chanControls(n).IntensityMax_Changed, AddressOf Chan_IntensityMax_Changed
            AddHandler chanControls(n).SwapPolarity_Changed, AddressOf UcChanControl1_SwapPolarity_Changed
            AddHandler chanControls(n).MaxPulseWidthOutputPct_Changed, AddressOf Chan_MaxOutputPulsewidthPct_Changed
            SplitContainer6.Panel2.Controls.Add(chanControls(n))
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
    Private Sub Chan_MaxOutputPulsewidthPct_Changed(ByVal index As Integer, ByVal NewVal As Integer)
        sendParameter(pStatEnum.MaxPulsewidthOutputPct, NewVal, index)
    End Sub
    Private Sub Chan_IntensityMin_Changed(ByVal index As Integer, ByVal newMin As Integer)
        sendParameter(pStatEnum.MinIntensity, newMin, index)
    End Sub
    Private Sub Chan_IntensityMax_Changed(ByVal index As Integer, ByVal newMax As Integer)
        sendParameter(pStatEnum.MaxIntensity, newMax, index)
    End Sub
    Private Sub UcChanControl1_SwapPolarity_Changed(index As Integer, PolarityIsSwapped As Integer)
        sendParameter(pStatEnum.SwapPolarity, PolarityIsSwapped, index)
    End Sub
#End Region

#Region "Debug & Simulator"
    Private Sub InitializeSimulator()
        If IsNothing(editorDev) Then Return

        ReDim simChannelStates(editorDev.Channel.Count - 1)
        For i As Integer = 0 To editorDev.Channel.Count - 1
            simChannelStates(i) = New ChannelSimState
            ReDim simChannelStates(i).Variables(editorDev.NumVarsPerProgram - 1)
            ReDim simChannelStates(i).Timers(editorDev.NumTimersPerProgram - 1)
            ReDim simChannelStates(i).TimerStartTicks(editorDev.NumTimersPerProgram - 1)
            ReDim simChannelStates(i).TimerRunning(editorDev.NumTimersPerProgram - 1)
            ' All values initialized to 0 by default
        Next

        AddHandler tmrSim.Tick, AddressOf tmrSim_Tick
        tmrSim.Enabled = False
    End Sub

    Private Sub InitializeDebugTabs()
        ' Called once or on project/device change
        PopulateChannelCombo(cboVariablesChannel)
        PopulateChannelCombo(cboTimersChannel)

        ' Setup grid columns (only once)
        SetupVariablesGrid()
        SetupTimersGrid()

        ' Default to Master channel
        If cboVariablesChannel.Items.Count > 0 Then
            cboVariablesChannel.SelectedIndex = 0
            cboTimersChannel.SelectedIndex = 0
        End If

        InitializeSimulator()
    End Sub

    Private Sub PopulateChannelCombo(cbo As ComboBox)
        cbo.Items.Clear()
        If IsNothing(editorDev) Then Return

        For i As Integer = 0 To editorDev.Channel.Count - 1
            Dim chanName As String = editorDev.allChannelsString(i).TrimEnd(Chr(0))
            If chanName = "" Then chanName = "Channel " & i
            cbo.Items.Add(chanName)
        Next
    End Sub

    Private Sub SetupVariablesGrid()
        dgvVariables.Columns.Clear()
        dgvVariables.Columns.Add("VarName", "Variable")
        dgvVariables.Columns.Add("VarValue", "Value")
        dgvVariables.Columns(0).Width = 220
        dgvVariables.Columns(1).Width = 100
        dgvVariables.Columns(1).AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        dgvVariables.Columns(1).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
        dgvVariables.AllowUserToAddRows = False
        dgvVariables.AllowUserToDeleteRows = False
        dgvVariables.ReadOnly = True
        dgvVariables.RowHeadersVisible = False
    End Sub

    Private Sub SetupTimersGrid()
        dgvTimers.Columns.Clear()
        dgvTimers.Columns.Add("TimerName", "Timer")
        dgvTimers.Columns.Add("TimerValue", "Value (ms)")
        dgvTimers.Columns(0).Width = 220
        dgvTimers.Columns(1).Width = 100
        dgvTimers.Columns(1).AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        dgvTimers.Columns(1).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
        dgvTimers.AllowUserToAddRows = False
        dgvTimers.AllowUserToDeleteRows = False
        dgvTimers.ReadOnly = True
        dgvTimers.RowHeadersVisible = False
    End Sub

    Private Sub cboVariablesChannel_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboVariablesChannel.SelectedIndexChanged
        UpdateVariablesGrid()
    End Sub

    Private Sub cboTimersChannel_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboTimersChannel.SelectedIndexChanged
        UpdateTimersGrid()
    End Sub

    Private Sub UpdateVariablesGrid()
        dgvVariables.Rows.Clear()
        If IsNothing(editorDev) OrElse cboVariablesChannel.SelectedIndex < 0 Then Return

        Dim progNum As Integer = editorDev.CurProgNum
        If progNum < 0 OrElse progNum >= editorDev.Prog.Count Then Return

        Dim prog As Program = editorDev.Prog(progNum)

        For i As Integer = 0 To Math.Min(prog.varName.Count - 1, editorDev.NumVarsPerProgram - 1)
            Dim varName As String = prog.varName(i).TrimEnd(Chr(0))
            If varName = "" Then varName = "Var " & i
            dgvVariables.Rows.Add(varName, 0)  ' Initial value = 0 (will be updated by simulator later)
        Next
    End Sub

    Private Sub UpdateTimersGrid()
        dgvTimers.Rows.Clear()
        If IsNothing(editorDev) OrElse cboTimersChannel.SelectedIndex < 0 Then Return

        Dim progNum As Integer = editorDev.CurProgNum
        If progNum < 0 OrElse progNum >= editorDev.Prog.Count Then Return

        Dim prog As Program = editorDev.Prog(progNum)

        For i As Integer = 0 To Math.Min(prog.timerName.Count - 1, editorDev.NumTimersPerProgram - 1)
            Dim timerName As String = prog.timerName(i).TrimEnd(Chr(0))
            If timerName = "" Then timerName = "Tim " & i
            dgvTimers.Rows.Add(timerName, 0)  ' Initial value = 0 ms
        Next
    End Sub


    Private Sub tmrSim_Tick(sender As Object, e As EventArgs)
        If simState = eSimState.Running OrElse simState = eSimState.Stepping Then
            ' Advance one line on every channel (simple demo)
            For i = 0 To simChannelStates.Length - 1
                simChannelStates(i).CurLineNum += 1
                If simChannelStates(i).CurLineNum >= editorDev.Prog(simChannelStates(i).CurProgNum).progLine.Count Then
                    simChannelStates(i).CurLineNum = 0
                End If
            Next

            UpdateAllDebugGridsAndHighlight()

            ' If we were stepping, pause after one cycle
            If simState = eSimState.Stepping Then
                simState = eSimState.Paused
                tmrSim.Enabled = False
                tsbtnDebugRun.Enabled = True
                tsbtnDebugPause.Enabled = False
                tsbtnDebugStep.Enabled = True
            End If
        End If
    End Sub

    Private Sub ExecuteOneSimulationCycle()
        For chanIdx As Integer = 0 To editorDev.Channel.Count - 1
            Dim state As ChannelSimState = simChannelStates(chanIdx)
            Dim prog As Program = editorDev.Prog(state.CurProgNum)

            If state.CurLineNum >= prog.progLine.Count Then
                state.CurLineNum = 0  ' Loop or stop? For now loop
            End If

            Dim line() As Integer = prog.progLine(state.CurLineNum)
            Dim cmd As Integer = line(eDataField.dfCommand)

            ' Check breakpoint
            If simBreakpoints.Contains(state.CurLineNum) Then
                simState = eSimState.Paused
                tmrSim.Enabled = False
                tsbtnDebugRun.Enabled = True
                tsbtnDebugPause.Enabled = False
                tsbtnDebugStep.Enabled = True
                Continue For
            End If

            Select Case cmd
                Case eCommand.cmdNoop
                    ' Do nothing

                Case eCommand.cmdSet
                    SimulatorSetCommand(chanIdx, line)

                Case eCommand.cmdGoTo
                    state.CurLineNum = line(eDataField.dfGotoTrue) - 1
                    Continue For  ' Skip increment

                Case eCommand.cmdTest
                    Dim testResult As Boolean = SimulatorTestCommand(chanIdx, line)
                    If testResult Then
                        state.CurLineNum = line(eDataField.dfGotoTrue) - 1
                    Else
                        state.CurLineNum = line(eDataField.dfGotoFalse) - 1
                    End If
                    Continue For

                Case eCommand.cmdTenMotOutput
                    ' Simulate output activation
                    SimulatorTenMotOutputCommand(chanIdx, state, line)
            End Select

            ' Increment line
            state.CurLineNum += 1
            If state.CurLineNum >= prog.progLine.Count Then
                state.CurLineNum = 0
            End If
        Next
    End Sub

    Private Sub SimulatorSetCommand(chanNum As Integer, curProgLine As Integer())
        If chanNum >= editorDev.Channel.Count Then Exit Sub

        'Determine the math result (answer), then determine where to put it (variable, dig output value, etc.)
        Dim maxResult = 2000000000
        Dim setResult = SimulatorGetValue(curProgLine(eDataField.df321S), curProgLine(eDataField.df321V1), curProgLine(eDataField.df321V2), chanNum, 0)
        If curProgLine(eDataField.df82V2) > eMathOp.mathOpNone Then
            'Determine the modifier value:
            Dim setModifierVal = SimulatorGetValue(curProgLine(eDataField.df322S), curProgLine(eDataField.df322V1), curProgLine(eDataField.df322V2), chanNum, 0)

            Select Case curProgLine(eDataField.df82V2)
                Case eMathOp.mathOpAdd
                    If maxResult - setResult > setModifierVal Then
                        setResult += setModifierVal
                    Else
                        setResult = maxResult
                    End If
                Case eMathOp.mathOpSubtract
                    If setResult > setModifierVal Then
                        setResult -= setModifierVal
                    Else
                        setResult = 0
                    End If
                Case eMathOp.mathOpMultiply
                    If setResult = 0 OrElse setModifierVal = 0 Then
                        setResult = 0
                    ElseIf maxResult / setResult > setModifierVal Then
                        setResult *= setModifierVal
                    Else
                        setResult = maxResult
                    End If
                Case eMathOp.mathOpDivide
                    If setResult >= setModifierVal AndAlso setModifierVal > 0 Then
                        setResult = setResult / setModifierVal
                    Else
                        setResult = 0
                    End If
                Case eMathOp.mathOpRemainder
                    If setModifierVal > 0 Then
                        setResult += setResult Mod setModifierVal
                    Else
                        setResult = 0
                    End If

                Case Else
                    Exit Sub
            End Select

            'Now figure out what to do with the result we just calculated:
            Dim tmpIndex = curProgLine(eDataField.df81V1)
            Select Case curProgLine(eDataField.df81S)
                Case eDataSource.dsDirect
                    'Nothing to do here.
                Case eDataSource.dsProgramVar
                    simChannelStates(chanNum).Variables(tmpIndex) = setResult
                Case eDataSource.dsSysVar
                    simChannelStates(0).Variables(tmpIndex) = setResult
                Case eDataSource.dsChanSetting
                    'Any program can be run on any channel.  So if the program needs to specifically reference the channel that it's running
                    'on, it will specify the channel index == NUM_CHANNELS.  So if dataVal2 (requested channel) = NUM_CHANNELS then it's looking for this channel.
                    Dim targetChan As Integer = curProgLine(eDataField.df81V2)
                    If targetChan = editorDev.Channel.Count Then
                        targetChan = chanNum
                    End If

                    Select Case curProgLine(eDataField.df81V1)
                        Case eDataSourceChanSetting.dscsSpeed
                            simChannelStates(targetChan).Speed = setResult

                            'The commented options below can be skipped:
                        'Case eDataSourceChanSetting.dscsOrigOpDuration
                        'Case eDataSourceChanSetting.dscsOrigPdDuration
                        'Case eDataSourceChanSetting.dscsOrigTotalDuration
                        'Case eDataSourceChanSetting.dscsModOpDuration
                        'Case eDataSourceChanSetting.dscsModPdDuration
                        'Case eDataSourceChanSetting.dscsModTotalDuration
                        'Case eDataSourceChanSetting.dscsElapsedOpDuration
                        'Case eDataSourceChanSetting.dscsElapsedPdDuration
                        'Case eDataSourceChanSetting.dscsElapsedTotalDuration
                        'Case eDataSourceChanSetting.dscsRemainingOpDuration
                        'Case eDataSourceChanSetting.dscsRemainingPdDuration
                        'Case eDataSourceChanSetting.dscsRemainingTotalDuration
                        'Case eDataSourceChanSetting.dscsPercentComplete
                        'Case eDataSourceChanSetting.dscsStartVal
                        'Case eDataSourceChanSetting.dscsEndVal
                        'Case eDataSourceChanSetting.dscsCurVal

                        Case eDataSourceChanSetting.dscsPolarity
                            'This will change the polarity override of the channel.
                            Select Case setResult
                                Case 0 'Normal polarity
                                    simChannelStates(targetChan).PolaritySwapped = False
                                Case 1 'Reverse polarity
                                    simChannelStates(targetChan).PolaritySwapped = True
                                Case Else 'Toggle polarity
                                    simChannelStates(targetChan).PolaritySwapped = Not simChannelStates(targetChan).PolaritySwapped
                            End Select
                        Case eDataSourceChanSetting.dscsIntensity
                            'Set the current voltage level of the tens unit (0-100%)
                            simChannelStates(targetChan).TensIntensityCurLevel = setResult

                        Case eDataSourceChanSetting.dscsIntensityMin
                            'Set the minimum voltage level of the tens unit (0-100%)
                            simChannelStates(targetChan).TensIntensityMinLevel = setResult

                        Case eDataSourceChanSetting.dscsIntensityMax
                            'Set the maximum voltage level of the tens unit (0-100%)
                            simChannelStates(targetChan).TensIntensityMaxLevel = setResult

                        'Case eDataSourceChanSetting.dscsCurProgNum
                        'Case eDataSourceChanSetting.dscsCurLineNum
                        'Case eDataSourceChanSetting.dscsCurChanNum
                        Case eDataSourceChanSetting.dscsOutputPulsewidthMaxPercent
                            'Set the max pulsewidth (output volume control for pulsewidth) for scaling:
                            simChannelStates(targetChan).OutputMaxPulsewidthPercent = setResult

                    End Select

                Case eDataSource.dsSysSetting
                    Select Case curProgLine(eDataField.df81V1)
                        Case eDataSourceSysSetting.dsssZRotation
                            'Grok, we need to set our simulated imu Z rotation value.  ZRotation = setResult
                        'Case eDataSourceSysSetting.dsssUpDirection
                        Case eDataSourceSysSetting.dsssStepCount
                            'Grok, we need to set our fake step counter = setResult.

                            'We can igore these commented cases:
                            'Case eDataSourceSysSetting.dsssAudioTotal
                            'Case eDataSourceSysSetting.dsssAudioLow
                            'Case eDataSourceSysSetting.dsssAudioMid
                            'Case eDataSourceSysSetting.dsssAudioHigh

                    End Select
                Case eDataSource.dsDigIn
                    'Ignore, we can't "set" a digital input.
                Case eDataSource.dsDigOut
                    'Grok, if we wanted to simulate digital outputs, here's where we'd set an output.
                Case eDataSource.dsRandom
                    'Ignore, nothing to do here.
                Case eDataSource.dsTimer
                    If tmpIndex < simChannelStates(chanNum).Timers.Count Then
                        'simChannelStates(chanNum).Timers(tmpIndex) = 'Grok- here's where we need to set the timer variable = HAL_GetTicks().  How could we do this in our simulation?
                    End If
            End Select
        End If
    End Sub

    Private Function SimulatorGetValue(datasource As eDataSource, dataVal1 As Integer, dataval2 As Integer, chanNum As Integer, maxVal As Integer) As Integer
        Dim retVal As Integer = 0

        Select Case datasource
            Case eDataSource.dsDirect
                retVal = dataVal1
            Case eDataSource.dsProgramVar
                If dataVal1 >= 0 AndAlso dataVal1 < simChannelStates(chanNum).Variables.Length Then
                    retVal = simChannelStates(chanNum).Variables(dataVal1)
                End If
            Case eDataSource.dsSysVar
                If dataVal1 >= 0 AndAlso dataVal1 < simChannelStates(0).Variables.Length Then
                    retVal = simChannelStates(0).Variables(dataVal1)
                End If
            Case eDataSource.dsChanSetting
                'Any program can be run on any channel.  So if the program needs to specifically reference the channel that it's running
                'on, it will specify the channel index == NUM_CHANNELS.  So if dataVal2 (requested channel) = NUM_CHANNELS then it's looking for this channel.
                Dim targetChan As Integer = dataval2
                If targetChan = editorDev.Channel.Count Then
                    targetChan = chanNum
                End If
                Select Case dataVal1
                    Case eDataSourceChanSetting.dscsSpeed
                        retVal = simChannelStates(targetChan).Speed
                    Case eDataSourceChanSetting.dscsOrigOpDuration
                        retVal = simChannelStates(targetChan).OutputDuration
                    Case eDataSourceChanSetting.dscsOrigPdDuration
                        retVal = simChannelStates(targetChan).DelayDuration
                    'Case eDataSourceChanSetting.dscsOrigTotalDuration
                    'Case eDataSourceChanSetting.dscsModOpDuration
                    'Case eDataSourceChanSetting.dscsModPdDuration
                    'Case eDataSourceChanSetting.dscsModTotalDuration
                    'Case eDataSourceChanSetting.dscsElapsedOpDuration
                    'Case eDataSourceChanSetting.dscsElapsedPdDuration
                    'Case eDataSourceChanSetting.dscsElapsedTotalDuration
                    'Case eDataSourceChanSetting.dscsRemainingOpDuration
                    'Case eDataSourceChanSetting.dscsRemainingPdDuration
                    'Case eDataSourceChanSetting.dscsRemainingTotalDuration
                    'Case eDataSourceChanSetting.dscsPercentComplete

                    Case eDataSourceChanSetting.dscsStartVal
                        retVal = simChannelStates(targetChan).OutputStartVal
                    Case eDataSourceChanSetting.dscsEndVal
                        retVal = simChannelStates(targetChan).OutputEndVal
                    Case eDataSourceChanSetting.dscsCurVal
                        retVal = simChannelStates(targetChan).OutputCurVal
                    Case eDataSourceChanSetting.dscsPolarity
                        retVal = simChannelStates(targetChan).Polarity
                    Case eDataSourceChanSetting.dscsIntensity
                        retVal = simChannelStates(targetChan).TensIntensityCurLevel
                    Case eDataSourceChanSetting.dscsIntensityMin
                        retVal = simChannelStates(targetChan).TensIntensityMinLevel
                    Case eDataSourceChanSetting.dscsIntensityMax
                        retVal = simChannelStates(targetChan).TensIntensityMaxLevel
                    Case eDataSourceChanSetting.dscsCurProgNum
                        retVal = simChannelStates(targetChan).CurProgNum
                    Case eDataSourceChanSetting.dscsCurLineNum
                        retVal = simChannelStates(targetChan).CurLineNum
                    Case eDataSourceChanSetting.dscsCurChanNum
                        retVal = chanNum
                    Case eDataSourceChanSetting.dscsOutputPulsewidthMaxPercent
                        retVal = simChannelStates(targetChan).OutputMaxPulsewidthPercent
                    Case Else
                        retVal = 0
                End Select

            Case eDataSource.dsSysSetting
                Select Case dataVal1
                    Case eDataSourceSysSetting.dsssZRotation
                        ' Simulate drifting compass (0-360)
                        Static lastZ As Integer = 180
                        lastZ += New Random().Next(-10, 11)
                        If lastZ < 0 Then lastZ = 360 + lastZ 'adding a negative number to subtract from 360.
                        If lastZ > 360 Then lastZ = lastZ - 360
                        retVal = lastZ
                    Case eDataSourceSysSetting.dsssUpDirection
                        ' Simulate which axis is up (0-5)
                        Static lastUp As Integer = 2
                        If New Random().Next(0, 10) = 0 Then ' Occasional drift
                            lastUp = (lastUp + New Random().Next(-1, 2) + 6) Mod 6
                        End If
                        retVal = lastUp

                    Case eDataSourceSysSetting.dsssStepCount
                        ' Increment occasionally
                        Static steps As Integer = 0
                        If New Random().Next(0, 20) = 0 Then steps += 1
                        retVal = steps

                    Case eDataSourceSysSetting.dsssAudioTotal,
                         eDataSourceSysSetting.dsssAudioLow,
                         eDataSourceSysSetting.dsssAudioMid,
                         eDataSourceSysSetting.dsssAudioHigh
                        retVal = New Random().Next(0, 101) ' 0-100
                    Case Else
                        retVal = 0
                End Select

            Case eDataSource.dsDigIn
                'We can ignore this for now.
                retVal = 0
            Case eDataSource.dsDigOut
                'We can ignore this for now.
                retVal = 0
            Case eDataSource.dsRandom
                If dataVal1 <= dataval2 Then
                    retVal = New Random().Next(dataVal1, dataval2 + 1)
                Else
                    retVal = New Random().Next(dataval2, dataVal1 + 1)
                End If

            Case eDataSource.dsTimer
                If dataVal1 >= 0 AndAlso dataVal1 < simChannelStates(chanNum).Timers.Length Then
                    If simChannelStates(chanNum).TimerRunning(dataVal1) Then
                        retVal = Environment.TickCount - simChannelStates(chanNum).TimerStartTicks(dataVal1)
                    Else
                        retVal = simChannelStates(chanNum).Timers(dataVal1)
                    End If
                End If
            Case Else
                retVal = 0
        End Select

        ' Apply max limit if requested
        If maxVal > 0 AndAlso retVal > maxVal Then
            retVal = maxVal
        End If
        Return retVal
    End Function

    Private Function SimulatorTestCommand(chanNum As Integer, curProgLine() As Integer) As Boolean
        If chanNum >= editorDev.Channel.Count Then Return False

        Dim testValLeft = SimulatorGetValue(curProgLine(eDataField.df81S), curProgLine(eDataField.df81V1), curProgLine(eDataField.df81V2), chanNum, 0)
        Dim testValRight = SimulatorGetValue(curProgLine(eDataField.df321S), curProgLine(eDataField.df321V1), curProgLine(eDataField.df321V2), chanNum, 0)
        Dim testValRight2 = SimulatorGetValue(curProgLine(eDataField.df322S), curProgLine(eDataField.df322V1), curProgLine(eDataField.df322V2), chanNum, 0)

        'Figure out if the Right value gets modified, and if so then how:
        Select Case curProgLine(eDataField.df82V2)
            Case eMathOp.mathOpNone
                'testValRight = testValRight
            Case eMathOp.mathOpAdd
                If testValRight < MAX_32BIT AndAlso testValRight2 < MAX_32BIT Then
                    testValRight += testValRight2
                Else
                    testValRight = MAX_32BIT
                End If
            Case eMathOp.mathOpSubtract
                If testValRight >= testValRight2 Then
                    testValRight -= testValRight2
                Else
                    testValRight = 0
                End If
            Case eMathOp.mathOpMultiply
                testValRight *= testValRight2
            Case eMathOp.mathOpDivide
                If testValRight >= testValRight2 AndAlso testValRight2 > 0 Then
                    testValRight = testValRight / testValRight2
                Else
                    testValRight = 0
                End If
            Case eMathOp.mathOpRemainder
                If testValRight2 > 0 Then
                    testValRight = testValRight Mod testValRight2
                Else
                    testValRight = 0
                End If
        End Select

        'Now perform the test:
        Select Case curProgLine(eDataField.df82V1)
            Case eCompare.tensCompare_LessThan
                Return testValLeft < testValRight
            Case eCompare.tensCompare_LessThanOrEqual
                Return testValLeft <= testValRight
            Case eCompare.tensCompare_Equal
                Return testValLeft = testValRight
            Case eCompare.tensCompare_NotEqual
                Return testValLeft <> testValRight
            Case eCompare.tensCompare_GreaterThanOrEqual
                Return testValLeft >= testValRight
            Case eCompare.tensCompare_GreaterThan
                Return testValLeft > testValRight
            Case eCompare.tensCompare_IsBetween
                Return (testValLeft > testValRight) And (testValLeft < testValRight2)
            Case eCompare.tensCompare_IsBetweenOrEqual
                Return (testValLeft >= testValRight) And (testValLeft <= testValRight2)
            Case eCompare.tensCompare_IsNotBetween
                Return (testValLeft < testValRight) Or (testValLeft > testValRight2)
        End Select
        Return False
    End Function

    Private Sub SimulatorTenMotOutputCommand(chanNum As Integer, State As ChannelSimState, line As Integer())
        If chanNum >= editorDev.Channel.Count Then Exit Sub

        State.OutputActive = True
        State.OutputStartTime = Environment.TickCount
        State.OutputDuration = SimulatorGetValue(line(eDataField.df321S), line(eDataField.df321V1), line(eDataField.df321V2), chanNum, 0)
        State.DelayDuration = SimulatorGetValue(line(eDataField.df323S), line(eDataField.df323V1), line(eDataField.df323V2), chanNum, 0)
        State.RepeatsRemaining = SimulatorGetValue(line(eDataField.df322S), line(eDataField.df322V1), line(eDataField.df322V2), chanNum, 0)
        State.OutputStartVal = SimulatorGetValue(line(eDataField.df81S), line(eDataField.df81V1), line(eDataField.df81V2), chanNum, 0)
        State.OutputEndVal = SimulatorGetValue(line(eDataField.df82S), line(eDataField.df82V1), line(eDataField.df82V2), chanNum, 0)

    End Sub

    Private Sub UpdateAllDebugGridsAndHighlight()
        If IsNothing(editorDev) Then Exit Sub

        Dim chanIdx As Integer = cboVariablesChannel.SelectedIndex
        If chanIdx < 0 OrElse chanIdx >= simChannelStates.Length Then Exit Sub

        Dim state As ChannelSimState = simChannelStates(chanIdx)
        Dim progNum As Integer = state.CurProgNum

        If progNum < 0 OrElse progNum >= editorDev.Prog.Count Then Exit Sub

        ' Highlight current line in lstProgDisplay
        Dim lineCount As Integer = editorDev.Prog(progNum).progLine.Count
        If lineCount > 0 Then
            Dim displayLine As Integer = state.CurLineNum
            If displayLine >= lineCount Then displayLine = 0

            If displayLine >= 0 AndAlso displayLine < lstProgDisplay.Items.Count Then
                lstProgDisplay.SelectedIndex = displayLine
                lstProgDisplay.TopIndex = Math.Max(0, displayLine - 5)
            End If
        End If

        ' Refresh grids (even if just showing 0s — proves update works)
        UpdateVariablesGrid()
        UpdateTimersGrid()
    End Sub

#End Region

#Region "Program Lines"
    Private Sub InitProgLineEditControl()
        UcProgLineEdit1.SetDeviceRef(editorDev) 'Do this first, then set the other properties.

        UcProgLineEdit1.ChannelCount = editorDev.Channel.Count
        UcProgLineEdit1.DigInCount = editorDev.NumDigInputs
        UcProgLineEdit1.DigOutCount = editorDev.NumDigOutputs
        UcProgLineEdit1.ProgVarCount = editorDev.NumVarsPerProgram
        UcProgLineEdit1.ProgTimerCount = editorDev.NumTimersPerProgram
        UcProgLineEdit1.Clear()
    End Sub

    Private Function GetFormattedLineNumber(lineNumZeroBased As Integer) As String
        'This function formats the line number display for the lstProgramSentence display.
        Dim retVal As String = lineNumZeroBased
        retVal = retVal.PadRight(5) ' & ": "
        Return retVal
    End Function

    Private Sub LoadProgramSentenceDisplay()
        lstProgDisplay.Items.Clear()
        Dim tmpStr As String
        Dim curProg As Integer = editorDev.CurProgNum

        For n As Integer = 1 To editorDev.Prog(curProg).progLine.Count
            tmpStr = GetFormattedLineNumber(n - 1) & UcProgLineEdit1.CreateProgLineSentence(curProg, n - 1)
            lstProgDisplay.Items.Add(tmpStr)
        Next
    End Sub

    Private Sub lstProgDisplay_SelectedIndexChanged(sender As Object, e As EventArgs)
        If UcProgLineEdit1.UnsavedChanges = True Then
            Dim res As MsgBoxResult
            If autoCommitProgLineChanges = True Then
                res = vbYes
            Else
                res = MsgBox("Save Line Changes (Yes), Discard changes (No), Or Continue Editing (Cancel)?", vbYesNoCancel, "Unsaved Line Changes")
            End If

            If res = vbYes Then
                RaiseEvent UnsavedProgramChanges(editorDev, True)
                UcProgLineEdit1_RequestSave(UcProgLineEdit1.ProgramNumber, UcProgLineEdit1.LineNumber, UcProgLineEdit1.ProgLine)
            ElseIf res = vbCancel Then
                RemoveHandler lstProgDisplay.SelectedIndexChanged, AddressOf lstProgDisplay_SelectedIndexChanged
                lstProgDisplay.ClearSelected()
                lstProgDisplay.SelectedIndex = editorDev.curLineNum
                AddHandler lstProgDisplay.SelectedIndexChanged, AddressOf lstProgDisplay_SelectedIndexChanged
                Exit Sub
            Else
                'Discard changes.  Move on.
            End If
        End If

        'If multiple items are selected then we can detect that here:
        If lstProgDisplay.SelectedIndices.Count > 1 Then
            Dim abc As Integer = lstProgDisplay.SelectedIndices.Count
        End If
        Dim tmpIndex As Integer = lstProgDisplay.SelectedIndex
        If tmpIndex < 0 Then
            Exit Sub
        End If

        UcProgLineEdit1.Clear()
        editorDev.curLineNum = lstProgDisplay.SelectedIndex
        UcProgLineEdit1.LoadProgLine(editorDev.CurProgNum, editorDev.curLineNum, progLineEditEnabled)

    End Sub

    Private Sub UcProgLineEdit1_RequestSave(ProgramNumber As Integer, LineNumber As Integer, updatedProgLine As Integer())
        'Commit changes to progs list, update sentence in progDisplay, accept changes in ucProgline Edit control
        Dim tmpIndex = LineNumber
        Dim prog = editorDev.Prog

        'Save to progs list:
        If ProgramNumber < 0 Or LineNumber < 0 Then
            'ignore.
            Exit Sub
        ElseIf ProgramNumber >= prog.Count Then
            MsgBox("The original program could Not be found.  Failed to save.", MsgBoxStyle.Critical, "Failed to Save :(")
        ElseIf LineNumber >= prog(ProgramNumber).progLine.Count Then
            MsgBox("The original program line could Not be found.  Failed to save.", MsgBoxStyle.Critical, "Failed to Save :(")
        Else
            updatedProgLine.CopyTo(prog(ProgramNumber).progLine(LineNumber), 0)
            RaiseEvent UnsavedProgramChanges(editorDev, True)
            'Update sentence:
            Dim tmpStr As String
            tmpStr = GetFormattedLineNumber(tmpIndex) & UcProgLineEdit1.CreateProgLineSentence(editorDev.CurProgNum, tmpIndex)
            RemoveHandler lstProgDisplay.SelectedIndexChanged, AddressOf lstProgDisplay_SelectedIndexChanged
            lstProgDisplay.Items(tmpIndex) = tmpStr
            AddHandler lstProgDisplay.SelectedIndexChanged, AddressOf lstProgDisplay_SelectedIndexChanged
        End If

        'Accept changes in ucProgLineEdit
        UcProgLineEdit1.AcceptChanges()
    End Sub


    Private Sub CheckGotoNumber(TargetProgNumber As Integer, oldIndex As Integer, ByVal newIndex As Integer)
        'If a line was moved, inserted, or deleted, then we need to go through the whole program and update
        'the GotoTrue and GotoFalse line numbers.
        'Key:   OldIndex    NewIndex    Description
        '       -1          >=0         New line was added (newIndex = progs().count-1) or inserted (newIndex < progs().count-1)
        '       >=0         -1          Line was deleted
        '       >=0         >=0         Line was moved

        'Adjustments required:
        'Line added to end:  None.
        'Line inserted:  Any GTT or GTF that points at the newIndex or greater needs to be incremented.
        'Line Deleted:  Any GTT or GTF that points to a line greater that oldIndex needs to be decremented
        'Line Moved: This is tricky and could go either way (adjust or not to adjust).
        '==For now, don't make changes to line moved.

        If (oldIndex < 0) And (newIndex >= 0) Then
            'Line added or inserted
            If newIndex < editorDev.Prog(TargetProgNumber).progLine.Count - 1 Then
                'Line inserted.
                AdjustGotoNumbers(TargetProgNumber, newIndex, 1)
            Else
                'Line added at end.  Nothing to do.
            End If
        ElseIf (oldIndex >= 0) And (newIndex < 0) Then
            'Line deleted.
            AdjustGotoNumbers(TargetProgNumber, oldIndex + 1, -1)
        End If

    End Sub

    Private Sub AdjustGotoNumbers(TargetProgNumber As Integer, startIndex As Integer, offset As Integer)
        Dim tmpLine(DataFieldLen - 1) As Integer
        For n As Integer = 0 To editorDev.Prog(TargetProgNumber).progLine.Count - 1
            tmpLine = editorDev.Prog(TargetProgNumber).progLine(n)
            If (tmpLine(eDataField.dfCommand) = eCommand.cmdGoTo) Or (tmpLine(eDataField.dfCommand) = eCommand.cmdTest) Then
                If tmpLine(eDataField.dfGotoTrue) >= startIndex Then
                    tmpLine(eDataField.dfGotoTrue) += offset
                    RaiseEvent UnsavedProgramChanges(editorDev, True)
                End If
                If tmpLine(eDataField.dfGotoFalse) >= startIndex Then
                    tmpLine(eDataField.dfGotoFalse) += offset
                    RaiseEvent UnsavedProgramChanges(editorDev, True)
                End If
            End If
        Next
    End Sub


    Private Sub AddBlankProgramLine(ByRef TargetProg As Program, Optional isCurProg As Boolean = False, Optional numLinesToAdd As Integer = 1)
        For n As Integer = 1 To numLinesToAdd
            Dim tmpProgLine(DataFieldLen - 1) As Integer
            TargetProg.progLine.Add(tmpProgLine)
        Next
        'NOTE:  Adding blank lines to the end of a program won't trigger an UnsavedChanges event.  But inserting lines will trigger it.

        If isCurProg = True Then
            'Update the program sentence display:
            Dim Prog As List(Of Program) = editorDev.Prog
            Dim tmpLineNum As Integer = Prog(editorDev.CurProgNum).progLine.Count - 1
            Dim tmpStr As String
            tmpStr = GetFormattedLineNumber(tmpLineNum) & UcProgLineEdit1.CreateProgLineSentence(editorDev.CurProgNum, tmpLineNum)
            lstProgDisplay.Items.Add(tmpStr)
            lstProgDisplay.SelectedIndex = lstProgDisplay.Items.Count - 1
        End If
    End Sub
    Private Sub cmdAddRow_Click(sender As Object, e As EventArgs) Handles cmdAddRow.Click
        If editorDev.CurProgNum < 0 Then
            lstPrograms.SelectedIndex = 0
            Exit Sub
        End If

        AddBlankProgramLine(editorDev.Prog(editorDev.CurProgNum), True)
    End Sub
    Private Sub cmsPsAddLine_Click(sender As Object, e As EventArgs) Handles cmsPsAddLine.Click
        AddBlankProgramLine(editorDev.Prog(editorDev.CurProgNum), True)
    End Sub

    Private Sub InsertBlankProgramLine(TargetProgNumber As Integer, IndexForNewLine As Integer)
        Dim tmpProgLine(DataFieldLen - 1) As Integer
        If IndexForNewLine < 0 Then IndexForNewLine = 0
        If IndexForNewLine >= editorDev.Prog(TargetProgNumber).progLine.Count - 1 Then
            AddBlankProgramLine(editorDev.Prog(TargetProgNumber), TargetProgNumber = editorDev.CurProgNum)
            Exit Sub
        End If
        editorDev.Prog(TargetProgNumber).progLine.Insert(IndexForNewLine, tmpProgLine)
        RaiseEvent UnsavedProgramChanges(editorDev, True)

        CheckGotoNumber(TargetProgNumber, -1, IndexForNewLine)

        If TargetProgNumber = editorDev.CurProgNum Then
            'Update the program sentence display:
            lstProgDisplay.BeginUpdate()
            LoadProgramSentenceDisplay()
            lstProgDisplay.EndUpdate()
            lstProgDisplay.SelectedIndex = IndexForNewLine
        End If
    End Sub
    Private Sub cmdInsertRow_Click(sender As Object, e As EventArgs) Handles cmdInsertRow.Click
        InsertBlankProgramLine(editorDev.CurProgNum, lstProgDisplay.SelectedIndex)
    End Sub
    Private Sub cmsPsInsertLine_Click(sender As Object, e As EventArgs) Handles cmsPsInsertLine.Click
        InsertBlankProgramLine(editorDev.CurProgNum, lstProgDisplay.SelectedIndex)
    End Sub

    Private Sub DeleteProgramLine(TargetProgNumber As Integer, LineIndex As Integer)
        Dim Prog As List(Of Program) = editorDev.Prog
        If (TargetProgNumber < 0) Or (TargetProgNumber >= Prog.Count) Then Exit Sub
        If (LineIndex < 0) Or (LineIndex >= Prog(TargetProgNumber).progLine.Count) Then Exit Sub

        Prog(TargetProgNumber).progLine.RemoveAt(LineIndex)
        RaiseEvent UnsavedProgramChanges(editorDev, True)
        CheckGotoNumber(TargetProgNumber, LineIndex, -1)

        If TargetProgNumber = editorDev.CurProgNum Then
            'Update the program sentence display:
            lstProgDisplay.BeginUpdate()
            LoadProgramSentenceDisplay()
            If LineIndex < Prog(editorDev.CurProgNum).progLine.Count Then
                lstProgDisplay.SelectedIndex = LineIndex
            ElseIf Prog(editorDev.CurProgNum).progLine.Count > 0 Then
                lstProgDisplay.SelectedIndex = Prog(editorDev.CurProgNum).progLine.Count - 1
            Else
                lstProgDisplay.SelectedIndex = -1
            End If
            lstProgDisplay.EndUpdate()
        End If
    End Sub
    Private Sub cmdDeleteRow_Click(sender As Object, e As EventArgs) Handles cmdDeleteRow.Click
        DeleteProgramLine(editorDev.CurProgNum, lstProgDisplay.SelectedIndex)
    End Sub
    Private Sub cmsPsDeleteLine_Click(sender As Object, e As EventArgs) Handles cmsPsDeleteLine.Click
        DeleteProgramLine(editorDev.CurProgNum, lstProgDisplay.SelectedIndex)
    End Sub

    Private Sub lstPrograms_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lstPrograms.SelectedIndexChanged
        editorDev.CurProgNum = lstPrograms.SelectedIndex
        UcProgLineEdit1.Clear()
        UcProgLineEdit1.ProgramNumber = editorDev.CurProgNum

        If editorDev.CurProgNum >= 0 And editorDev.CurProgNum < editorDev.Prog.Count Then
            lstProgDisplay.BeginUpdate()
            LoadProgramSentenceDisplay()
            lstProgDisplay.EndUpdate()
            lblProgName.Text = "Prog " & editorDev.CurProgNum
            txtProgName.Text = editorDev.Prog(editorDev.CurProgNum).progName.TrimEnd
            txtProgName.Enabled = True
        Else
            lblProgName.Text = "No Program Loaded."
            txtProgName.Text = ""
            txtProgName.Enabled = False
        End If

        UpdateVariablesGrid()
    End Sub

    Private Sub MoveProgramLineUp(TargetProgNumber As Integer, ByVal OrigIndex As Integer)
        If (OrigIndex < 1) Or (TargetProgNumber >= editorDev.Prog.Count) Then Exit Sub
        If OrigIndex >= editorDev.Prog(TargetProgNumber).progLine.Count Then Exit Sub

        Dim tmpLine(DataFieldLen - 1) As Integer
        editorDev.Prog(TargetProgNumber).progLine(OrigIndex).CopyTo(tmpLine, 0)
        editorDev.Prog(TargetProgNumber).progLine.RemoveAt(OrigIndex)
        editorDev.Prog(TargetProgNumber).progLine.Insert(OrigIndex - 1, tmpLine)

        RaiseEvent UnsavedProgramChanges(editorDev, True)

        If TargetProgNumber = editorDev.CurProgNum Then
            'Update the program sentence display:
            lstProgDisplay.BeginUpdate()
            LoadProgramSentenceDisplay()
            lstProgDisplay.SelectedIndex = OrigIndex - 1
            lstProgDisplay.EndUpdate()
        End If

    End Sub
    Private Sub cmdMoveRowUp_Click(sender As Object, e As EventArgs) Handles cmdMoveRowUp.Click
        MoveProgramLineUp(editorDev.CurProgNum, lstProgDisplay.SelectedIndex)
    End Sub
    Private Sub cmsPsMoveLineUp_Click(sender As Object, e As EventArgs) Handles cmsPsMoveLineUp.Click
        MoveProgramLineUp(editorDev.CurProgNum, lstProgDisplay.SelectedIndex)
    End Sub


    Private Sub MoveProgramLineDown(TargetProgNumber As Integer, ByVal OrigIndex As Integer)
        If (OrigIndex < 0) Or (TargetProgNumber >= editorDev.Prog.Count) Then Exit Sub
        If OrigIndex + 1 >= editorDev.Prog(TargetProgNumber).progLine.Count Then Exit Sub

        Dim tmpLine(DataFieldLen - 1) As Integer
        editorDev.Prog(TargetProgNumber).progLine(OrigIndex + 1).CopyTo(tmpLine, 0)
        editorDev.Prog(TargetProgNumber).progLine.RemoveAt(OrigIndex + 1)
        editorDev.Prog(TargetProgNumber).progLine.Insert(OrigIndex, tmpLine)

        RaiseEvent UnsavedProgramChanges(editorDev, True)

        If TargetProgNumber = editorDev.CurProgNum Then
            'Update the program sentence display:
            lstProgDisplay.BeginUpdate()
            LoadProgramSentenceDisplay()
            lstProgDisplay.SelectedIndex = OrigIndex + 1
            lstProgDisplay.EndUpdate()
        End If
    End Sub
    Private Sub cmdMoveDown_Click(sender As Object, e As EventArgs) Handles cmdMoveDown.Click
        MoveProgramLineDown(editorDev.CurProgNum, lstProgDisplay.SelectedIndex)
    End Sub
    Private Sub cmsPsMoveLineDown_Click(sender As Object, e As EventArgs) Handles cmsPsMoveLineDown.Click
        MoveProgramLineDown(editorDev.CurProgNum, lstProgDisplay.SelectedIndex)
    End Sub

    Private Sub DuplicateProgramLine(TargetProgNumber As Integer, ByVal OrigIndex As Integer)
        If (OrigIndex < 0) Or (TargetProgNumber >= editorDev.Prog.Count) Then Exit Sub
        If OrigIndex >= editorDev.Prog(TargetProgNumber).progLine.Count Then Exit Sub

        Dim tmpLine(DataFieldLen - 1) As Integer
        editorDev.Prog(TargetProgNumber).progLine(OrigIndex).CopyTo(tmpLine, 0)
        editorDev.Prog(TargetProgNumber).progLine.Insert(OrigIndex + 1, tmpLine)

        RaiseEvent UnsavedProgramChanges(editorDev, True)

        CheckGotoNumber(TargetProgNumber, -1, OrigIndex + 1)

        If TargetProgNumber = editorDev.CurProgNum Then
            'Update the program sentence display:
            lstProgDisplay.BeginUpdate()
            LoadProgramSentenceDisplay()
            lstProgDisplay.SelectedIndex = OrigIndex
            lstProgDisplay.EndUpdate()
        End If

    End Sub
    Private Sub cmdDuplicateProgLine_Click(sender As Object, e As EventArgs) Handles cmdDuplicateProgLine.Click
        DuplicateProgramLine(editorDev.CurProgNum, lstProgDisplay.SelectedIndex)
    End Sub
    Private Sub cmsPsDuplicateLine_Click(sender As Object, e As EventArgs) Handles cmsPsDuplicateLine.Click
        DuplicateProgramLine(editorDev.CurProgNum, lstProgDisplay.SelectedIndex)
    End Sub
#End Region

#Region "Program List"

    Private Function OkToProceedAndDiscardUnsavedChanges(ByRef Dev As Device_t) As Boolean
        'If there are any unsaved changes, prompt the user to save them, discard them, or abort.
        If Dev.UnsavedChanges = True Then
            Dim tmpRes As MsgBoxResult = MsgBox("There are unsaved changes." & vbCrLf & "Do you want to save your changes (Yes)," &
                                                vbCrLf & "Discard your changes And continue (No)," & vbCrLf &
                                                "Or cancel this operation And resume making changes (Cancel)?", MsgBoxStyle.YesNoCancel, "Save Changes?")
            If tmpRes = vbYes Then
                Return SaveProjectToFile(editorDev.FileName)
            ElseIf tmpRes = vbNo Then
                'Dev.UnsavedChanges = False
                Return True
            Else
                Return False
            End If
        Else
            Return True
        End If
    End Function

    Private Function GetNewBlankProgram(ByRef Dev As Device_t) As Program

        Dim tmpProg As New Program
        Dim tmpName As String = "NewProg"
        tmpProg.progName = tmpName.PadRight(10)
        tmpProg.progLine = New List(Of Integer())
        tmpProg.varName = New List(Of String)
        tmpProg.timerName = New List(Of String)

        For n As Integer = 0 To Dev.NumVarsPerProgram
            tmpProg.varName.Add("Var " & n)
        Next
        For n As Integer = 0 To Dev.NumTimersPerProgram
            tmpProg.timerName.Add("Tim " & n)
        Next

        'Add one blank prog line for the new program:
        AddBlankProgramLine(tmpProg, False)

        Return tmpProg
    End Function

    Private Sub PopulateProgramListDisplay()
        RemoveHandler lstPrograms.SelectedIndexChanged, AddressOf lstPrograms_SelectedIndexChanged
        lstPrograms.Items.Clear()
        Dim tmpStr As String = ""
        For p As Integer = 0 To editorDev.Prog.Count - 1
            tmpStr = "(" & p & ") " & editorDev.Prog(p).progName
            lstPrograms.Items.Add(tmpStr)
        Next
        AddHandler lstPrograms.SelectedIndexChanged, AddressOf lstPrograms_SelectedIndexChanged

        RemoveHandler txtProjectName.TextChanged, AddressOf txtProjectName_TextChanged
        txtProjectName.Text = editorDev.ProjectName.Trim
        AddHandler txtProjectName.TextChanged, AddressOf txtProjectName_TextChanged

        RemoveHandler txtDeviceName.TextChanged, AddressOf txtDeviceName_TextChanged
        txtDeviceName.Text = editorDev.Name.Trim
        AddHandler txtDeviceName.TextChanged, AddressOf txtDeviceName_TextChanged

        lblDeviceType.Text = deviceTypeString(editorDev.DeviceType)
    End Sub

    Private Sub AddBlankProgram(ByRef Dev As Device_t, Optional ByVal NumToAdd As Integer = 1)
        Dim tmpIndex As Integer = Dev.Prog.Count

        For n As Integer = 1 To NumToAdd
            Dev.Prog.Add(GetNewBlankProgram(Dev))
        Next

        RaiseEvent UnsavedProgramChanges(editorDev, True)

        If Object.Equals(Dev, editorDev) Then
            lstPrograms.BeginUpdate()
            PopulateProgramListDisplay()
            lstPrograms.EndUpdate()
        End If
    End Sub
    Private Sub cmsProgAddBlankProgramToEnd_Click(sender As Object, e As EventArgs) Handles cmsProgAddBlankProgramToEnd.Click
        AddBlankProgram(editorDev)
    End Sub

    Private Sub InsertBlankProgram(ByRef Dev As Device_t, Index As Integer, Optional ByVal RefreshDisplay As Boolean = True)
        If Index < 0 Then Index = 0
        If Index >= Dev.Prog.Count Then
            AddBlankProgram(Dev)
            Exit Sub
        End If

        Dev.Prog.Insert(Index, GetNewBlankProgram(Dev))
        RaiseEvent UnsavedProgramChanges(editorDev, True)

        If (Object.Equals(Dev, editorDev)) And (RefreshDisplay = True) Then
            RemoveHandler lstPrograms.SelectedIndexChanged, AddressOf lstPrograms_SelectedIndexChanged
            PopulateProgramListDisplay()
            lstPrograms.SelectedIndex = Index
        End If
    End Sub
    Private Sub cmsProgInsertBlankProgram_Click(sender As Object, e As EventArgs) Handles cmsProgInsertBlankProgram.Click
        InsertBlankProgram(editorDev, lstPrograms.SelectedIndex)
    End Sub

    Private Sub DuplicateProgram(ByRef Dev As Device_t, TargetProgNumber As Integer)
        If (TargetProgNumber < 0) Or (TargetProgNumber >= Dev.Prog.Count) Then Exit Sub

        'Insert a new entry in progs just after the one to be duplicated
        Dev.Prog.Insert(TargetProgNumber + 1, GetNewBlankProgram(Dev))

        'A single blank line is added during GetNewBlankProgram by default, but we don't want it in this instance.
        Dev.Prog(TargetProgNumber + 1).progLine.Clear()

        'Copy all of the data from the target index to the new item just added.
        Dim tmpProgForName As Program = Dev.Prog(TargetProgNumber + 1)
        tmpProgForName.progName = Dev.Prog(TargetProgNumber).progName
        Dev.Prog(TargetProgNumber + 1) = tmpProgForName

        For p As Integer = 0 To Dev.Prog(TargetProgNumber).progLine.Count - 1
            Dim tmpLine(DataFieldLen - 1) As Integer
            Dev.Prog(TargetProgNumber).progLine(p).CopyTo(tmpLine, 0)
            Dev.Prog(TargetProgNumber + 1).progLine.Add(tmpLine)
        Next
        For vn As Integer = 0 To Dev.Prog(TargetProgNumber).varName.Count - 1
            Dim tmpProg As Program = Dev.Prog(TargetProgNumber + 1)
            tmpProg.varName(vn) = Dev.Prog(TargetProgNumber).varName(vn)
            Dev.Prog(TargetProgNumber + 1) = tmpProg
        Next
        For tn As Integer = 0 To Dev.Prog(TargetProgNumber).timerName.Count - 1
            Dim tmpProg As Program = Dev.Prog(TargetProgNumber + 1)
            tmpProg.timerName(tn) = Dev.Prog(TargetProgNumber).timerName(tn)
            Dev.Prog(TargetProgNumber + 1) = tmpProg
        Next

        RaiseEvent UnsavedProgramChanges(editorDev, True)
        RemoveHandler lstPrograms.SelectedIndexChanged, AddressOf lstPrograms_SelectedIndexChanged
        PopulateProgramListDisplay()
        lstPrograms.SelectedIndex = TargetProgNumber

    End Sub
    Private Sub cmsProgDuplicateProgram_Click(sender As Object, e As EventArgs) Handles cmsProgDuplicateProgram.Click
        DuplicateProgram(editorDev, lstPrograms.SelectedIndex)
    End Sub


    Private Sub MoveProgramTowardsBeginning(TargetProgNumber As Integer)
        If (TargetProgNumber < 1) Or (TargetProgNumber >= editorDev.Prog.Count) Then Exit Sub

        Dim tmpProg As Program = editorDev.Prog(TargetProgNumber - 1)
        editorDev.Prog(TargetProgNumber - 1) = editorDev.Prog(TargetProgNumber)
        editorDev.Prog(TargetProgNumber) = tmpProg

        RaiseEvent UnsavedProgramChanges(editorDev, True)

        PopulateProgramListDisplay()
        RemoveHandler lstPrograms.SelectedIndexChanged, AddressOf lstPrograms_SelectedIndexChanged
        lstPrograms.SelectedIndex = TargetProgNumber - 1
        AddHandler lstPrograms.SelectedIndexChanged, AddressOf lstPrograms_SelectedIndexChanged
    End Sub
    Private Sub cmsProgMoveProgramTowardsBeginning_Click(sender As Object, e As EventArgs) Handles cmsProgMoveProgramTowardsBeginning.Click
        MoveProgramTowardsBeginning(lstPrograms.SelectedIndex)
    End Sub

    Private Sub MoveProgramTowardsEnd(TargetProgNumber As Integer)
        If (TargetProgNumber < 0) Or (TargetProgNumber >= editorDev.Prog.Count - 1) Then Exit Sub

        Dim tmpProg As Program = editorDev.Prog(TargetProgNumber)
        editorDev.Prog(TargetProgNumber) = editorDev.Prog(TargetProgNumber + 1)
        editorDev.Prog(TargetProgNumber + 1) = tmpProg

        RaiseEvent UnsavedProgramChanges(editorDev, True)

        PopulateProgramListDisplay()
        RemoveHandler lstPrograms.SelectedIndexChanged, AddressOf lstPrograms_SelectedIndexChanged
        lstPrograms.SelectedIndex = TargetProgNumber + 1
        AddHandler lstPrograms.SelectedIndexChanged, AddressOf lstPrograms_SelectedIndexChanged
    End Sub
    Private Sub cmsProgMoveProgramTowardsEnd_Click(sender As Object, e As EventArgs) Handles cmsProgMoveProgramTowardsEnd.Click
        MoveProgramTowardsEnd(lstPrograms.SelectedIndex)
    End Sub


    Private Sub DeleteProgram(TargetProgNumber As Integer)
        If (TargetProgNumber < 0) Or (TargetProgNumber >= editorDev.Prog.Count) Then Exit Sub
        Dim tmpIndex As Integer = TargetProgNumber

        RemoveHandler lstPrograms.SelectedIndexChanged, AddressOf lstPrograms_SelectedIndexChanged
        editorDev.Prog.RemoveAt(tmpIndex)
        RaiseEvent UnsavedProgramChanges(editorDev, True)
        AddHandler lstPrograms.SelectedIndexChanged, AddressOf lstPrograms_SelectedIndexChanged

        PopulateProgramListDisplay()

        If tmpIndex < lstPrograms.Items.Count Then
            lstPrograms.SelectedIndex = tmpIndex
        ElseIf lstPrograms.Items.Count > 0 Then
            lstPrograms.SelectedIndex = lstPrograms.Items.Count - 1
        Else
            lstPrograms.SelectedIndex = -1
        End If
    End Sub
    Private Sub cmsProgDeleteProgram_Click(sender As Object, e As EventArgs) Handles cmsProgDeleteProgram.Click
        DeleteProgram(lstPrograms.SelectedIndex)
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        If UcProgLineEdit1.Enabled = True Then
            UcProgLineEdit1.Enabled = False
            progLineEditEnabled = False
        Else
            chkLiveTrackLines.Checked = False
            UcProgLineEdit1.Enabled = True
            progLineEditEnabled = True
            UcProgLineEdit1.LoadProgLine(editorDev.CurProgNum, editorDev.curLineNum, progLineEditEnabled)
        End If
    End Sub

    Private Sub txtProgName_TextChanged(sender As Object, e As EventArgs) Handles txtProgName.TextChanged
        Dim curProg = editorDev.CurProgNum
        If curProg < 0 Then Exit Sub
        Dim tmpProg = editorDev.Prog(curProg)
        tmpProg.progName = txtProgName.Text.PadRight(10)
        editorDev.Prog(curProg) = tmpProg
        RaiseEvent UnsavedProgramChanges(editorDev, True)
        'Update the program list to show the new name:
        RemoveHandler lstPrograms.SelectedIndexChanged, AddressOf lstPrograms_SelectedIndexChanged
        lstPrograms.Items.Item(curProg) = "(" & curProg & ") " & editorDev.Prog(curProg).progName
        AddHandler lstPrograms.SelectedIndexChanged, AddressOf lstPrograms_SelectedIndexChanged
    End Sub

    Private Sub EditVarNames()
        If lstPrograms.SelectedIndex < 0 Then Exit Sub
        'DialogVarNamesAll.ShowVarEditDialog(editorDev.Prog, lstPrograms.SelectedIndex, devNumVarsPerProgram)
        Dim tmpProg As List(Of Program) = editorDev.Prog
        If DialogVarNamesAll.ShowEditDialog(tmpProg, lstPrograms.SelectedIndex, DialogVarNamesAll.NameList_t.VarNames) = MsgBoxResult.Ok Then
            editorDev.Prog = tmpProg
            RaiseEvent UnsavedProgramChanges(editorDev, True)
            UpdateVariablesGrid()
        End If
    End Sub
    Private Sub cmdEditVarNames_Click(sender As Object, e As EventArgs) Handles cmdEditVarNames.Click
        EditVarNames()
    End Sub
    Private Sub cmsProgEditVarNames_Click(sender As Object, e As EventArgs) Handles cmsProgEditVarNames.Click
        EditVarNames()
    End Sub

    Private Sub EditTimerNames()
        If lstPrograms.SelectedIndex < 0 Then Exit Sub
        Dim tmpProg As List(Of Program) = editorDev.Prog
        If DialogVarNamesAll.ShowEditDialog(tmpProg, lstPrograms.SelectedIndex, DialogVarNamesAll.NameList_t.TimNames) = MsgBoxResult.Ok Then
            editorDev.Prog = tmpProg
            RaiseEvent UnsavedProgramChanges(editorDev, True)
        End If
    End Sub
    Private Sub cmsProgEditTimerNames_Click(sender As Object, e As EventArgs) Handles cmsProgEditTimerNames.Click
        EditTimerNames()
    End Sub

#End Region

#Region "Save Project/Program"

    Private Sub Form1_UnsavedProgramChanges(ByRef Dev As Device_t, newState As Boolean) Handles Me.UnsavedProgramChanges
        Dev.UnsavedChanges = newState
        If Object.Equals(Dev, editorDev) Then
            tscmdSaveProjectToFile.Enabled = newState
            SaveProjectToolStripMenuItem.Enabled = newState
            If newState = True Then RaiseEvent UndownloadedProgramChanges(Dev, True)
        End If
    End Sub

    Private Sub Form1_UndownloadedProgramChanges(ByRef Dev As Device_t, newState As Boolean) Handles Me.UndownloadedProgramChanges
        Dev.UndownloadedChanges = newState
        If Object.Equals(Dev, editorDev) Then
            DownloadProjectToDeviceToolStripMenuItem.Enabled = newState
            tscmdSaveProgramToDevice.Enabled = newState
        End If
    End Sub

    Private Sub Save()
        SaveProjectToFile(editorDev.FileName)
    End Sub
    Private Sub SaveAs()
        Dim emptyStr As String = ""
        SaveProjectToFile(emptyStr)
    End Sub


    Private Function SaveProjectToFile(filePath As String) As Boolean
        'return true if successful, false if fail

        ' Create an anonymous type to hold both prog and FileVersion
        'Dim dataToSave = New With {
        '    .ProjectName = editorDev.ProjectName,
        '    .FileVersion = editorDev.Version,' fileVersion,
        '    .Programs = editorDev.Prog,
        '    .Channel = tmpChanNames
        '}
        Dim dataToSave = New With {
            .Dev = editorDev
        }

        If filePath.Length < 1 Then
            If sfdProject.ShowDialog() = DialogResult.Cancel Then
                Return False
            Else

                filePath = sfdProject.FileName
                If filePath.Contains("\"c) Then
                    My.Settings.Item("projectPath") = filePath.Remove(filePath.LastIndexOf("\"))
                    My.Settings.Save()
                End If
            End If
        End If

        Try
            ' Serialize to JSON with indented formatting
            Dim jsonString As String = JsonConvert.SerializeObject(dataToSave, Formatting.None)

            ' Write to file (you can change the path as needed)
            File.WriteAllText(filePath, jsonString)
            editorDev.FileName = filePath
            RaiseEvent UnsavedProgramChanges(editorDev, False)

            Console.WriteLine("Data successfully saved to " & filePath)
            Return True
        Catch ex As Exception
            Console.WriteLine("Error saving to JSON file: " & ex.Message)
            MsgBox("Failed to save file.  Sorry.", vbCritical, "Whoops")
            Return False
        End Try
    End Function
#End Region
#Region "Load Project/Program From File"
    '1. Choose file
    '2. Choose whether to load the whole project or just a single program
    '3. Read file, load into a temp list(of Program)
    '4a. If loading the entire project, assign the temp list to the prog pointer
    '4b. If loading a single program, add a new program to prog() and then copy over just the new program.

    Private Function GetFileNameToOpen() As String
        If My.Settings.projectPath.Length Then ofdProject.InitialDirectory = My.Settings.projectPath
        If ofdProject.ShowDialog() = vbCancel Then
            Return ""
        Else
            Return ofdProject.FileName
        End If
    End Function

    ' New method to load from JSON file
    Private Function OpenProjectFile(filePath As String) As Device_t
        Try
            ' Check if file exists
            If Not File.Exists(filePath) Then
                Throw New FileNotFoundException("JSON file Not found", filePath)
            End If

            ' Read the JSON file
            Dim jsonString As String = File.ReadAllText(filePath)

            ' Deserialize into our JsonData class
            Dim loadedData As JsonData = JsonConvert.DeserializeObject(Of JsonData)(jsonString)

            ' Check the FileVersion
            If loadedData.Dev.Version.ToUpper <> tensCurVersion.ToUpper Then
                Throw New FileNotFoundException("Wrong File Version", "Expected Ver:" & tensCurVersion & ", Got Ver:" & loadedData.Dev.Version)
            End If

            ' Grab the project name for future reference
            'If Not IsNothing(loadedData.Dev.ProjectName) Then
            '    openFileProjectName = loadedData.Dev.ProjectName
            'Else
            '    openFileProjectName = " "
            'End If

            'Check the program names to be sure they didn't pick up extra characters
            For n As Integer = 0 To loadedData.Dev.Prog.Count - 1
                If loadedData.Dev.Prog(n).progName.Length > 10 Then
                    Dim tmpProg As Program = loadedData.Dev.Prog(n)
                    tmpProg.progName = tmpProg.progName.Substring(0, 10)
                    loadedData.Dev.Prog(n) = tmpProg
                End If
            Next


            ' Return the list of programs
            Console.WriteLine("Data successfully loaded from " & filePath)
            Return loadedData.Dev

        Catch ex As Exception
            Console.WriteLine("Error loading JSON file: " & ex.Message)
            MsgBox("Error loading JSON file: " & ex.Message, MsgBoxStyle.Critical, "Error Loading File")
            'Return New List(Of Program) ' Return empty list on error
            Return GetDeviceTemplate(DeviceType_t.Tens2410E1) 'return empty device on error
        End Try
    End Function

    Private Sub RefreshProgramDisplays()
        InitProgLineEditControl()
        PopulateProgramListDisplay()
        InitializeDebugTabs()
    End Sub

    Private Sub OpenProjectFromFile()
        Dim filePath As String = GetFileNameToOpen()
        If filePath = "" Then Exit Sub

        editorDev = OpenProjectFile(filePath)
        RefreshProgramDisplays()

        If lstPrograms.Items.Count > 0 Then
            lstPrograms.SelectedIndex = 0
        End If

        'If newProg is good, capture the filename.
        editorDev.FileName = ofdProject.FileName
        If editorDev.FileName.Contains("\"c) Then
            My.Settings.Item("projectPath") = editorDev.FileName.Remove(editorDev.FileName.LastIndexOf("\"))
            My.Settings.Save()
        End If

        RaiseEvent UnsavedProgramChanges(editorDev, False)
        RaiseEvent UndownloadedProgramChanges(editorDev, True)
    End Sub

    Private Sub ImportProgramFromFile()
        Dim filePath As String = GetFileNameToOpen()
        If filePath = "" Then Exit Sub

        Dim tmpDev As Device_t = OpenProjectFile(filePath)

        'Find out which program the user wants to import and whether we're inserting it or adding it to the end of prog:
        If dialogImportProgram.ShowWithData(editorDev.Prog, editorDev.ProjectName, tmpDev.Prog, tmpDev.ProjectName) = vbCancel Then
            'abort.
            'openFileProjectName = ""
            tmpDev = Nothing
            Exit Sub
        End If
        Dim indexOfTmpProg As Integer = dialogImportProgram.SourceIndex
        Dim targetIndex As Integer = dialogImportProgram.TargetIndex

        'Copy the imported program to prog:
        InsertBlankProgram(editorDev, targetIndex, False)
        editorDev.Prog(targetIndex) = tmpDev.Prog(indexOfTmpProg)

        RemoveHandler lstPrograms.SelectedIndexChanged, AddressOf lstPrograms_SelectedIndexChanged
        RefreshProgramDisplays()

        If lstPrograms.Items.Count > targetIndex Then
            lstPrograms.SelectedIndex = targetIndex
        End If
        'RaiseEvent UnsavedProgramChanges(editorDev, True) --already covered by InsertBlankProgram above.
    End Sub


    Private Sub cmsProgImportProgram_Click(sender As Object, e As EventArgs) Handles cmsProgImportProgram.Click
        ImportProgramFromFile()
    End Sub

    Private Sub txtDeviceName_TextChanged(sender As Object, e As EventArgs) Handles txtDeviceName.TextChanged
        If editorDev.Name <> txtDeviceName.Text.PadRight(txtDeviceName.MaxLength) Then
            editorDev.Name = txtDeviceName.Text.PadRight(txtDeviceName.MaxLength)
            RaiseEvent UnsavedProgramChanges(editorDev, True)
        End If
    End Sub

    Private Sub txtProjectName_TextChanged(sender As Object, e As EventArgs) 'Handles txtProjectName.TextChanged
        If editorDev.ProjectName <> txtProjectName.Text.PadRight(txtProjectName.MaxLength) Then
            editorDev.ProjectName = txtProjectName.Text.PadRight(txtProjectName.MaxLength)
            RaiseEvent UnsavedProgramChanges(editorDev, True)
        End If
    End Sub

#End Region

#Region "Load Project From Device"

    Private Sub bwDLProgFromDevice_DoWork(sender As Object, e As DoWorkEventArgs) Handles bwDLProgFromDevice.DoWork
        'This runs as a background thread to retrieve a project from the device.
        '1. Capture the start time so we can monitor for a timeout.
        '2. Find out how many programs are on the device
        '3. Find out how many lines each program has
        '4. Get the program lines for each program
        '5. Get the names of each program
        '6  Get the progVar names for each program


        Dim tmpStartTime = Now()
        Dim tmpTimeoutinMs = 5000
        Dim tmpTimedOut As Boolean = False

        Dim tmpProgress = 0.0
        Dim tmpProgressStep = 0.0
        Dim tmpTotalMsg As Integer = 1 'Will be changed as soon as we start getting info.
        'Progress step calculation is based on the total number of messages required from the device for the download.
        'Each program requires: 1 to get prog length, 1 to get prog name, 30 to get progVar names, (prog length) to get progline data.
        'Also, retrieving the first message from device (# of programs) indicates communication success, so that counts as the first 5%.


        'Get the number of programs on device.  
        lastProgParamArrayReceived = eParamArray.None
        devNumProgramsLastReported = -1
        requestParamArray(eParamArray.DeviceInfo)
        While lastProgParamArrayReceived <> eParamArray.DeviceInfo
            If Now() > tmpStartTime.AddMilliseconds(tmpTimeoutinMs) Then
                bwDLProgFromDevice.ReportProgress(CInt(tmpProgress), "Timed out retrieving device info.")
                e.Cancel = True
                Exit While
            End If
            Threading.Thread.Sleep(10)
            Application.DoEvents()
        End While

        If e.Cancel = True Then Return

        tmpStartTime = Now
        'Wait for the prog object to be setup by the main thread.
        Dim tmpDone = False
        While tmpDone = False
            If Not IsNothing(monitorDev.Prog) Then
                If monitorDev.Prog.Count = devNumProgramsLastReported Then
                    tmpDone = True
                Else
                    If Now() > tmpStartTime.AddMilliseconds(tmpTimeoutinMs) Then
                        bwDLProgFromDevice.ReportProgress(CInt(tmpProgress), "Timed out while setting up local Prog object.")
                        e.Cancel = True
                        Exit While
                    End If
                    Threading.Thread.Sleep(10)
                    Application.DoEvents()
                End If
            End If

        End While

        tmpProgress = 5
        bwDLProgFromDevice.ReportProgress(CInt(tmpProgress), "Found " & devNumProgramsLastReported & " programs on device.  Retrieving program lengths...")

        'Get the number of lines for each program.
        'NOTE: This also creates blank program lines in the prog list for each program line on the device.
        For n As Integer = 1 To monitorDev.Prog.Count
            tmpStartTime = Now()
            lastProgSingleParamReceived = pStatEnum.None
            requestParameter(pStatEnum.ProgramLength, n - 1)
            While lastProgSingleParamReceived <> pStatEnum.ProgramLength
                If Now() > tmpStartTime.AddMilliseconds(tmpTimeoutinMs) Then
                    bwDLProgFromDevice.ReportProgress(CInt(tmpProgress), "Timed out retrieving the length of Program " & n - 1 & ".")
                    e.Cancel = True
                    Exit While
                End If
                Threading.Thread.Sleep(10)
                Application.DoEvents()
            End While

            If e.Cancel = True Then Return
        Next

        Threading.Thread.Sleep(5)
        'We now should know the total # of msgs required for the entire download.
        tmpProgressStep = (100 - 5) / (devTotalLines + 1) 'adding 1 so that we don't end up with a div by zero if there are no programs loaded

        'Retrieve the program line data:
        For p As Integer = 1 To monitorDev.Prog.Count
            For l As Integer = 0 To monitorDev.Prog(p - 1).progLine.Count - 1
                tmpStartTime = Now()
                lastProgParamArrayReceived = eParamArray.None
                requestParamArray(eParamArray.progLineData, p - 1, l)

                While lastProgParamArrayReceived <> eParamArray.progLineData
                    If Now() > tmpStartTime.AddMilliseconds(tmpTimeoutinMs) Then
                        bwDLProgFromDevice.ReportProgress(CInt(tmpProgress), "Timed out retrieving Prog " & p - 1 & " Line " & l & ".")
                        e.Cancel = True
                        Exit While
                    End If
                End While

                If e.Cancel = True Then
                    Return
                Else
                    tmpProgress += tmpProgressStep
                    bwDLProgFromDevice.ReportProgress(CInt(tmpProgress), "Downloading line data...")
                End If
            Next
        Next

        'Get the name of each program.
        For n As Integer = 1 To monitorDev.Prog.Count
            tmpStartTime = Now()
            lastProgSingleParamReceived = pStatEnum.None
            requestParameter(pStatEnum.ProgramName, n - 1)
            While lastProgSingleParamReceived <> pStatEnum.ProgramName
                If Now() > tmpStartTime.AddMilliseconds(tmpTimeoutinMs) Then
                    bwDLProgFromDevice.ReportProgress(CInt(tmpProgress), "Timed out retrieving Program " & n - 1 & " name.")
                    e.Cancel = True
                    Exit While
                End If
            End While

            If e.Cancel = True Then
                Return
            Else
                tmpProgress += tmpProgressStep
                bwDLProgFromDevice.ReportProgress(CInt(tmpProgress), "Downloading program names...")
            End If
        Next

        'Get the min/max settings of each channel.
        For n As Integer = 1 To monitorDev.Channel.Count
            tmpStartTime = Now()
            lastProgParamArrayReceived = eParamArray.None
            requestParamArray(eParamArray.chanMinMaxInfo, n - 1)
            While lastProgParamArrayReceived <> eParamArray.chanMinMaxInfo
                If Now() > tmpStartTime.AddMilliseconds(tmpTimeoutinMs) Then
                    bwDLProgFromDevice.ReportProgress(CInt(tmpProgress), "Timed out retrieving Min/Max Data for channel " & n - 1 & ".")
                    e.Cancel = True
                    Exit While
                End If
                Threading.Thread.Sleep(10)
                Application.DoEvents()
            End While

            If e.Cancel = True Then
                Return
            Else
                tmpProgress += tmpProgressStep
                bwDLProgFromDevice.ReportProgress(CInt(tmpProgress), "Downloading channel min/max settings...")
            End If
        Next

    End Sub

    Private Sub bwDLProgFromDevice_ProgressChanged(sender As Object, e As ProgressChangedEventArgs) Handles bwDLProgFromDevice.ProgressChanged
        ssmProgressBar.Value = If(e.ProgressPercentage > 100, 100, e.ProgressPercentage)
        ssmStatusText.Text = e.UserState
    End Sub

    Private Sub bwDLProgFromDevice_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs) Handles bwDLProgFromDevice.RunWorkerCompleted
        ' This event is fired when your BackgroundWorker exits.
        ' It may have exitted Normally after completing its task, 
        ' or because of Cancellation, or due to any Error.

        If e.Error IsNot Nothing Then
            ' if BackgroundWorker terminated due to error
            MsgBox(e.Error.Message, MsgBoxStyle.Critical, "Download Error")
            ssmStatusText.Text = "Error occurred during download."

        ElseIf e.Cancelled Then
            ' otherwise if it was cancelled
            MsgBox("Download was cancelled Or has timed out.", MsgBoxStyle.Critical, "Download Failed")
            ssmStatusText.Text = If(Not IsNothing(e.UserState), e.UserState.ToString, "Download Failed.")

        Else
            ' otherwise it completed normally
            ssmProgressBar.Value = 100
            ssmStatusText.Text = "Download complete."
        End If

        ProjectFinishedDownloading()
        downloadInProgress = False
        tmrClearStatusbar.Interval = 5000
        tmrClearStatusbar.Enabled = True
    End Sub

    Private Sub UpdateProjectName(newName As String)
        RemoveHandler txtProjectName.TextChanged, AddressOf txtProjectName_TextChanged
        txtProjectName.Text = newName.Trim
        AddHandler txtProjectName.TextChanged, AddressOf txtProjectName_TextChanged
    End Sub

    Private Sub OpenProjectFromDevice(ByRef Dev As Device_t)
        'Make sure an upload or download isn't already in process:
        If uploadInProgress Or downloadInProgress Then
            MsgBox("An upload or download is already in progress.  Please try again after that operation completes.")
            Exit Sub
        End If

        If bwDLProgFromDevice.IsBusy Then
            bwDLProgFromDevice.CancelAsync()
        End If
        'downloadInProgress = True

        'Make sure autostatus is off
        autoStatusLast = autoStatus
        autoStatus = eAutoStatus.asOff

        currentDownloadingDevice = Dev

        If Object.Equals(Dev, editorDev) = True Then
            If OkToProceedAndDiscardUnsavedChanges(Dev) = False Then
                Exit Sub
            Else
                UpdateProjectName("Downloading...")
            End If
        End If
        downloadInProgress = True
        bwDLProgFromDevice.RunWorkerAsync()
    End Sub

    Private Sub ProjectFinishedDownloading()

        If Object.Equals(currentDownloadingDevice, editorDev) Then
            'If MsgBox("Load project into the editor?", MsgBoxStyle.YesNo, "Download Complete") = vbYes Then
            LoadDownloadedProjectIntoEditor()
        End If

        'Restore autostatus mode:
        Select Case My.Settings.lastAutoStatus
            Case eAutoStatus.asOn
                cmdtsChanMonAutoStatusOn.PerformClick()
            Case Else
                'leave autostatus off
                cmdtsChanMonAutoStatusOff.PerformClick()
        End Select
    End Sub

    Private Sub LoadDownloadedProjectIntoEditor()
        If Me.InvokeRequired Then
            Me.Invoke(Sub()
                          LoadDownloadedProjectIntoEditor()
                      End Sub)
        Else
            editorDev = CopyDevice(monitorDev)
            RaiseEvent UnsavedProgramChanges(editorDev, False)
            RaiseEvent UndownloadedProgramChanges(editorDev, False)
            InitProgLineEditControl() 're-initialize so that it will have the correct channel count, channel names, etc. based on the current editorDev.
            RemoveHandler lstProgDisplay.SelectedIndexChanged, AddressOf lstProgDisplay_SelectedIndexChanged
            PopulateProgramListDisplay()
            AddHandler lstProgDisplay.SelectedIndexChanged, AddressOf lstProgDisplay_SelectedIndexChanged
            If lstPrograms.Items.Count > 0 Then
                lstPrograms.SelectedIndex = 0
            End If

            editorDev.FileName = ""
            autoStatus = autoStatusLast
        End If
    End Sub

    Private Function CopyChannel(ByRef source As Channel_t) As Channel_t
        Dim newChan As New Channel_t
        newChan.Name = New String(" ", 20)
        source.Name.CopyTo(newChan.Name)
        newChan.Type = source.Type
        newChan.SpeedMin = source.SpeedMin
        newChan.SpeedMax = source.SpeedMax
        Return newChan
    End Function

    Private Function CopyProg(ByRef source As Program) As Program
        Dim newProg As New Program

        newProg.progLine = New List(Of Integer())
        For pl As Integer = 1 To source.progLine.Count
            Dim newProgLine(DataFieldLen - 1) As Integer
            source.progLine(pl - 1).CopyTo(newProgLine, 0)
            newProg.progLine.Add(newProgLine)
        Next

        newProg.progName = New String(source.progName.PadRight(20))
        ' source.progName.CopyTo(newProg.progName)

        newProg.varName = New List(Of String)
        For vn As Integer = 1 To source.varName.Count
            Dim newVarName As String = New String(source.varName(vn - 1))
            newProg.varName.Add(newVarName)
        Next

        newProg.timerName = New List(Of String)
        For tn As Integer = 1 To source.timerName.Count
            Dim newTimName As String = New String(source.timerName(tn - 1))
            newProg.timerName.Add(newTimName)
        Next

        Return newProg
    End Function
    Private Function CopyDevice(ByRef source As Device_t) As Device_t
        Dim newDev As Device_t = GetDeviceTemplate(source.DeviceType)

        source.Name.CopyTo(newDev.Name)
        newDev.RadioId = source.RadioId
        source.serialNumber.CopyTo(newDev.serialNumber)
        source.Version.CopyTo(newDev.Version)

        newDev.Channel = New List(Of Channel_t)
        For n As Integer = 0 To source.Channel.Count - 1
            newDev.Channel.Add(CopyChannel(source.Channel(n)))
        Next

        newDev.NumMotorChans = source.NumMotorChans
        newDev.NumTensChans = source.NumTensChans
        newDev.NumAuxChans = source.NumAuxChans
        newDev.NumDigInputs = source.NumDigInputs
        newDev.NumDigOutputs = source.NumDigOutputs

        newDev.Prog = New List(Of Program)
        For p As Integer = 1 To source.Prog.Count
            newDev.Prog.Add(CopyProg(source.Prog(p - 1)))
        Next

        newDev.NumVarsPerProgram = source.NumVarsPerProgram
        newDev.NumTimersPerProgram = source.NumTimersPerProgram
        newDev.FileName = New String(source.FileName)

        newDev.UnsavedChanges = source.UnsavedChanges
        newDev.UndownloadedChanges = source.UndownloadedChanges

        newDev.ProjectName = New String(source.ProjectName)
        newDev.CurProgNum = source.CurProgNum
        newDev.curLineNum = source.curLineNum

        ReDim newDev.motorChanNameString(source.motorChanNameString.Length - 1)
        Array.Copy(source.motorChanNameString, newDev.motorChanNameString, source.motorChanNameString.Length)

        ReDim newDev.tensChanNameString(source.tensChanNameString.Length - 1)
        Array.Copy(source.tensChanNameString, newDev.tensChanNameString, source.tensChanNameString.Length)

        ReDim newDev.auxChanNameString(source.auxChanNameString.Length - 1)
        Array.Copy(source.auxChanNameString, newDev.auxChanNameString, source.auxChanNameString.Length)

        ReDim newDev.allChannelsString(source.allChannelsString.Length - 1)
        Array.Copy(source.allChannelsString, newDev.allChannelsString, source.allChannelsString.Length)

        ReDim newDev.allChannelsStringPlusThisChan(source.allChannelsStringPlusThisChan.Length - 1)
        Array.Copy(source.allChannelsStringPlusThisChan, newDev.allChannelsStringPlusThisChan, source.allChannelsStringPlusThisChan.Length)

        Return newDev
    End Function

    Private Sub bwULProgToDevice_DoWork(sender As Object, e As DoWorkEventArgs) Handles bwULProgToDevice.DoWork
        ''Send the number of programs to the device and that will trigger it to start requesting all project data from us.
        ''When it has successfully downloaded all data then it will send us a message letting us know it succeeded or failed.
        ''sendParameter(pStatEnum.NumberOfPrograms, prog.Count)
        ''bwULProgToDevice.RunWorkerAsync()

        'Download all program lines to the device.
        'Send data one line at a time and wait for an ack from the device before sending the next line.
        Dim timoutLenInSeconds = 3 '1
        Dim retryCounter = 0
        Dim maxRetries = 3

        Dim tmpProgress = 0.0
        Dim tmpProgressStep = 0.0

        'Gather the total number of messages that will be sent:
        Dim tmpTotalMsg As Integer = 1 'initial message
        'Number of lines for each program
        tmpTotalMsg += editorDev.Prog.Count
        'Program Name for each program:
        tmpTotalMsg += editorDev.Prog.Count
        'Line data for each program:
        For n As Integer = 1 To editorDev.Prog.Count
            tmpTotalMsg += editorDev.Prog(n - 1).progLine.Count
        Next
        tmpProgressStep = 100 / tmpTotalMsg

        'Autostatus was already shut off before we came into this thread worker. No need to turn it off again.

        'The device will stop itself as soon as the upload begins, so no need to explicitly stop the device from here.

        bwULProgToDevice.ReportProgress(CInt(tmpProgress), "Uploading program to device...")

        'Initiate the download by sending the number of programs to the device.
        lastCommandTypeReceived = 0
        'capture the timeout time for our timeout timer
        Dim progComTimeoutTime = Now.AddSeconds(timoutLenInSeconds)
        sendParameter(pStatEnum.NumberOfPrograms, editorDev.Prog.Count)

        'wait for the data to be returned.
        While lastCommandTypeReceived <> eCommandType.ACK
            If (lastCommandTypeReceived = eCommandType.NAK) Or (Now() > progComTimeoutTime) Then
                retryCounter += 1
                If retryCounter <= maxRetries Then
                    'retry
                    progComTimeoutTime = Now.AddSeconds(timoutLenInSeconds)
                    lastCommandTypeReceived = 0
                    bwULProgToDevice.ReportProgress(CInt(tmpProgress), "Uploading program to device... Retry " & retryCounter)
                    sendParameter(pStatEnum.NumberOfPrograms, editorDev.Prog.Count)
                Else
                    bwULProgToDevice.ReportProgress(CInt(tmpProgress), "Uploading program to device... TimedOut! " & retryCounter)
                    e.Cancel = True
                    Exit Sub
                End If
            End If
            Threading.Thread.Sleep(10)
            Application.DoEvents()
        End While

        tmpProgress += tmpProgressStep
        bwULProgToDevice.ReportProgress(CInt(tmpProgress), "Sending number of lines for each program...")

        'Send the the number of lines for each program:
        retryCounter = 0
        For n As Integer = 0 To (editorDev.Prog.Count - 1)
            lastCommandTypeReceived = 0
            'capture the timeout time for our timeout timer
            progComTimeoutTime = Now.AddSeconds(timoutLenInSeconds)
            sendParameter(pStatEnum.ProgramLength, editorDev.Prog(n).progLine.Count, n)

            'wait for the data to be returned.
            While lastCommandTypeReceived <> eCommandType.ACK
                If (lastCommandTypeReceived = eCommandType.NAK) Or (Now() > progComTimeoutTime) Then
                    retryCounter += 1
                    If retryCounter <= maxRetries Then
                        'retry
                        progComTimeoutTime = Now.AddSeconds(timoutLenInSeconds)
                        lastCommandTypeReceived = 0
                        bwULProgToDevice.ReportProgress(CInt(tmpProgress), "Sending number of lines for each program... " & n & "/" & (editorDev.Prog.Count - 1) & " Retry " & retryCounter)
                        sendParameter(pStatEnum.ProgramLength, editorDev.Prog(n).progLine.Count, n)
                    Else
                        bwULProgToDevice.ReportProgress(CInt(tmpProgress), "Sending number of lines for each program... " & n & "/" & (editorDev.Prog.Count - 1) & " TimedOut!")
                        e.Cancel = True
                        Exit Sub
                    End If
                End If
                Threading.Thread.Sleep(10)
                Application.DoEvents()
            End While
            tmpProgress += tmpProgressStep
            bwULProgToDevice.ReportProgress(CInt(tmpProgress), "Sending number of lines for each program... " & n & "/" & (editorDev.Prog.Count - 1))
        Next


        'For each program, send the program name and once the client ack's that then send each line of data for the program.
        'Send the program names:
        retryCounter = 0
        For p As Integer = 0 To (editorDev.Prog.Count - 1)
            Dim fullName As String = editorDev.Prog(p).progName.PadRight(10)
            If fullName.Length > 10 Then
                fullName = fullName.Substring(0, 10)
            End If

            lastCommandTypeReceived = 0
            'capture the timeout time for our timeout timer
            progComTimeoutTime = Now.AddSeconds(timoutLenInSeconds)
            bwULProgToDevice.ReportProgress(CInt(tmpProgress), "Sending program name of prog " & p)
            sendParameter(pStatEnum.ProgramName, fullName, p)

            'wait for the data to be returned.
            While lastCommandTypeReceived <> eCommandType.ACK
                If (lastCommandTypeReceived = eCommandType.NAK) Or (Now() > progComTimeoutTime) Then
                    retryCounter += 1
                    If retryCounter <= maxRetries Then
                        'retry
                        progComTimeoutTime = Now.AddSeconds(timoutLenInSeconds)
                        lastCommandTypeReceived = 0
                        bwULProgToDevice.ReportProgress(CInt(tmpProgress), "Sending program name of prog " & p & "  Retry " & retryCounter)
                        sendParameter(pStatEnum.ProgramName, fullName, p)
                    Else
                        bwULProgToDevice.ReportProgress(CInt(tmpProgress), "Sending program name of prog " & p & "  TimedOut!")
                        e.Cancel = True
                        Exit Sub
                    End If
                End If
                Threading.Thread.Sleep(1)
                Application.DoEvents()
            End While

            tmpProgress += tmpProgressStep

            'Now we're ready to send the program lines (one line at a time) to the device.
            For l As Integer = 0 To (editorDev.Prog(p).progLine.Count - 1)
                lastCommandTypeReceived = 0
                'capture the timeout time for our timeout timer
                progComTimeoutTime = Now.AddSeconds(timoutLenInSeconds)
                bwULProgToDevice.ReportProgress(CInt(tmpProgress), "Sending program line data of prog " & p & ": " & l & "/" & (editorDev.Prog(p).progLine.Count - 1))
                sendParamArray(editorDev, eParamArray.progLineData, p, l)

                While lastCommandTypeReceived <> eCommandType.ACK
                    If (lastCommandTypeReceived = eCommandType.NAK) Or (Now() > progComTimeoutTime) Then
                        retryCounter += 1
                        If retryCounter <= maxRetries Then
                            'retry
                            progComTimeoutTime = Now.AddSeconds(timoutLenInSeconds)
                            lastCommandTypeReceived = 0
                            bwULProgToDevice.ReportProgress(CInt(tmpProgress), "Sending program line data of prog " & p & ": " & l & "/" & (editorDev.Prog(p).progLine.Count - 1) & "  Retry " & retryCounter)
                            sendParamArray(editorDev, eParamArray.progLineData, p, l)
                        Else
                            bwULProgToDevice.ReportProgress(CInt(tmpProgress), "Sending program line data of prog " & p & ": " & l & "/" & (editorDev.Prog(p).progLine.Count - 1) & "  TimedOut!")
                            e.Cancel = True
                            Exit Sub
                        End If
                    End If
                    Threading.Thread.Sleep(1)
                    Application.DoEvents()
                End While
                tmpProgress += tmpProgressStep
            Next
        Next

        'Upload complete!
    End Sub

    Private Sub bwULProgToDevice_ProgressChanged(sender As Object, e As ProgressChangedEventArgs) Handles bwULProgToDevice.ProgressChanged
        ssmProgressBar.Value = e.ProgressPercentage
        ssmStatusText.Text = e.UserState
    End Sub

    Private Sub bwULProgToDevice_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs) Handles bwULProgToDevice.RunWorkerCompleted
        '' This event is fired when your BackgroundWorker exits.
        '' It may have exitted Normally after completing its task, 
        '' or because of Cancellation, or due to any Error.

        If e.Error IsNot Nothing Then
            '' if BackgroundWorker terminated due to error
            MessageBox.Show(e.Error.Message)
            ssmStatusText.Text = "Error occurred during upload."

        ElseIf e.Cancelled Then
            '' otherwise if it was cancelled
            MessageBox.Show("Upload cancelled!")
            ssmStatusText.Text = "Upload was cancelled or an error occurred."
        Else
            '' otherwise it completed normally

            MsgBox("Upload Complete.")
            RaiseEvent UndownloadedProgramChanges(editorDev, False)

            uploadInProgress = False
            'Restore autostatus
            If autoStatusLast = eAutoStatus.asOn Then
                cmdtsChanMonAutoStatusOn.PerformClick()
            End If

            'MessageBox.Show("Upload completed!")

            'Copy the editorDev to the monitorDev
            monitorDev = CopyDevice(editorDev)
            'Update the devRef in all the channel controls
            For n As Integer = 0 To chanControls.Length - 1
                chanControls(n).SetProgRef(monitorDev.Prog)
            Next


            ssmStatusText.Text = "Upload completed!"
        End If

        uploadInProgress = False
        tmrClearStatusbar.Interval = 10000
        tmrClearStatusbar.Enabled = True
    End Sub

    Private Sub SaveProjectToDevice()
        'Make sure an upload or download isn't already in process:
        If uploadInProgress Or downloadInProgress Then
            If updownAttemptCount > 0 Then
                bwULProgToDevice.CancelAsync()
                uploadInProgress = False
            Else
                updownAttemptCount += 1
            End If
            MsgBox("An upload or download is already in progress.  Please try again after that operation completes.")
            Exit Sub
        End If
        updownAttemptCount = 0

        If bwULProgToDevice.IsBusy Then
            bwULProgToDevice.CancelAsync()
        End If
        uploadInProgress = True

        'Make sure autostatus is off
        autoStatusLast = autoStatus
        cmdtsChanMonAutoStatusOff.PerformClick()

        bwULProgToDevice.RunWorkerAsync()
    End Sub

#End Region
#Region "Menus"


    Private Sub OpenProjectToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OpenProjectFromFileToolStripMenuItem.Click
        OpenProjectFromFile()
    End Sub

    Private Sub CreateNewProjectToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CreateNewProjectToolStripMenuItem.Click
        CreateNewProject(editorDev)
    End Sub

    Private Sub tsmDeviceType_Click(sender As Object, e As EventArgs) Handles tsmDeviceType.Click
        Dim tmpDevTypeSelector As New DialogDeviceType
        If tmpDevTypeSelector.ShowSetDefaultDevTypeDialog(Settings.DefaultNewDeviceType) = MsgBoxResult.Ok Then
            Settings.DefaultNewDeviceType = tmpDevTypeSelector.SelectedDeviceType
            Settings.LastNewDeviceType = tmpDevTypeSelector.SelectedDeviceType
            Settings.Save()
        End If
        tmpDevTypeSelector.Close()
        tmpDevTypeSelector.Dispose()
    End Sub


    Private Sub SaveProjectToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveProjectToolStripMenuItem.Click
        SaveProjectToFile(editorDev.FileName)
    End Sub

    Private Sub SaveProjectAsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveProjectAsToolStripMenuItem.Click
        Dim emptyStr As String = ""
        SaveProjectToFile(emptyStr)
    End Sub

    Private Sub AddNewBlankProgramToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AddNewBlankProgramToolStripMenuItem.Click
        InsertBlankProgram(editorDev, editorDev.Prog.Count)
    End Sub

    Private Sub mnuAddBlankProgramAtIndex_Click(sender As Object, e As EventArgs) Handles mnuAddBlankProgramAtIndex.Click
        InsertBlankProgram(editorDev, lstPrograms.SelectedIndex)
    End Sub

    Private Sub mnuAddBlankProgramAtEnd_Click(sender As Object, e As EventArgs) Handles mnuAddBlankProgramAtEnd.Click
        InsertBlankProgram(editorDev, editorDev.Prog.Count)
    End Sub


    Private Sub ImportExistingProgramToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ImportExistingProgramToolStripMenuItem.Click
        ImportProgramFromFile()
    End Sub

    Private Sub MoveProgramTowardsBeginningToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MoveProgramTowardsBeginningToolStripMenuItem.Click
        MoveProgramTowardsBeginning(lstPrograms.SelectedIndex)
    End Sub

    Private Sub MoveProgramTowardsEndToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MoveProgramTowardsEndToolStripMenuItem.Click
        MoveProgramTowardsEnd(lstPrograms.SelectedIndex)
    End Sub

    Private Sub DeleteProgramToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DeleteProgramToolStripMenuItem.Click
        DeleteProgram(lstPrograms.SelectedIndex)
    End Sub

    Private Sub mnuDuplicateProgram_Click(sender As Object, e As EventArgs) Handles mnuDuplicateProgram.Click
        DuplicateProgram(editorDev, lstPrograms.SelectedIndex)
    End Sub

    Private Sub mnuAddProgLine_Click(sender As Object, e As EventArgs) Handles mnuAddProgLine.Click
        AddBlankProgramLine(editorDev.Prog(editorDev.CurProgNum), True)
    End Sub

    Private Sub mnuInsertProgLine_Click(sender As Object, e As EventArgs) Handles mnuInsertProgLine.Click
        InsertBlankProgramLine(editorDev.CurProgNum, lstProgDisplay.SelectedIndex)
    End Sub

    Private Sub mnuDuplicateProgLine_Click(sender As Object, e As EventArgs) Handles mnuDuplicateProgLine.Click
        DuplicateProgramLine(editorDev.CurProgNum, lstProgDisplay.SelectedIndex)
    End Sub

    Private Sub mnuMoveProgLineUp_Click(sender As Object, e As EventArgs) Handles mnuMoveProgLineUp.Click
        MoveProgramLineUp(editorDev.CurProgNum, lstProgDisplay.SelectedIndex)
    End Sub

    Private Sub mnuMoveProgramLineDown_Click(sender As Object, e As EventArgs) Handles mnuMoveProgramLineDown.Click
        MoveProgramLineDown(editorDev.CurProgNum, lstProgDisplay.SelectedIndex)
    End Sub

    Private Sub mnuDeleteProgramLine_Click(sender As Object, e As EventArgs) Handles mnuDeleteProgramLine.Click
        DeleteProgramLine(editorDev.CurProgNum, lstProgDisplay.SelectedIndex)
    End Sub

    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        My.Settings.Save()
    End Sub

    Private Sub OpenProjectFromDeviceToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OpenProjectFromDeviceToolStripMenuItem.Click
        If ComPort.IsOpen Then
            OpenProjectFromDevice(editorDev)
        Else
            MsgBox("No device connected.", MsgBoxStyle.ApplicationModal, "Not Connected")
        End If
    End Sub

    Private Sub DownloadProjectToDeviceToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DownloadProjectToDeviceToolStripMenuItem.Click
        If ComPort.IsOpen Then
            SaveProjectToDevice()
        Else
            MsgBox("No device connected.", MsgBoxStyle.ApplicationModal, "Not Connected")
        End If
    End Sub

    Private Sub tmrClearStatusbar_Tick(sender As Object, e As EventArgs) Handles tmrClearStatusbar.Tick
        tmrClearStatusbar.Enabled = False
        ssmProgressBar.Value = 0
        ssmStatusText.Text = ""
    End Sub

    Private Sub tscmdNewProject_Click(sender As Object, e As EventArgs) Handles tscmdNewProject.Click
        CreateNewProject(editorDev)
    End Sub

    Private Sub tscmdOpenProjectFromFile_Click(sender As Object, e As EventArgs) Handles tscmdOpenProjectFromFile.Click
        OpenProjectFromFile()
    End Sub

    Private Sub tscmdSaveProjectToFile_Click(sender As Object, e As EventArgs) Handles tscmdSaveProjectToFile.Click
        SaveProjectToFile(editorDev.FileName)
    End Sub

    Private Sub tscmdOpenProgramFromDevice_Click(sender As Object, e As EventArgs) Handles tscmdOpenProgramFromDevice.Click
        If ComPort.IsOpen Then
            OpenProjectFromDevice(editorDev)
        Else
            MsgBox("No device connected.", MsgBoxStyle.ApplicationModal, "Not Connected")
        End If
    End Sub

    Private Sub tscmdSaveProgramToDevice_Click(sender As Object, e As EventArgs) Handles tscmdSaveProgramToDevice.Click
        If ComPort.IsOpen Then
            SaveProjectToDevice()
        Else
            MsgBox("No device connected.", MsgBoxStyle.ApplicationModal, "Not Connected")
        End If
    End Sub


    Private Sub tsbtnDebugRun_Click(sender As Object, e As EventArgs) Handles tsbtnDebugRun.Click
        If simState = eSimState.Stopped Then
            InitializeSimulator()
        End If

        simState = eSimState.Running
        tmrSim.Enabled = True

        tsbtnDebugRun.Enabled = False
        tsbtnDebugPause.Enabled = True
        tsbtnDebugStep.Enabled = False
    End Sub

    Private Sub tsbtnDebugPause_Click(sender As Object, e As EventArgs) Handles tsbtnDebugPause.Click
        simState = eSimState.Paused
        tmrSim.Enabled = False

        tsbtnDebugRun.Enabled = True
        tsbtnDebugPause.Enabled = False
        tsbtnDebugStep.Enabled = True
    End Sub

    Private Sub tsbtnDebugStep_Click(sender As Object, e As EventArgs) Handles tsbtnDebugStep.Click
        If simState = eSimState.Stopped Then
            InitializeSimulator()
        End If
        simState = eSimState.Stepping
        tmrSim.Enabled = True  ' One tick will run and then pause
    End Sub


    Private Sub tsbtnRealOrSimulated_CheckedChanged(sender As Object, e As EventArgs) Handles tsbtnRealOrSimulated.CheckedChanged
        If tsbtnRealOrSimulated.Checked Then
            ' Entering simulator mode
            lblDevStatus.Text = "Simulator Mode (Offline)"
            ' Optionally disable COM controls
            cboComPorts.Enabled = False
            cmdRefreshComportList.Enabled = False
            cmdConnect.Enabled = False
        Else
            ' Back to device mode
            comportIsOpen(ComPort.IsOpen) ' Restore normal status
            cboComPorts.Enabled = True
            cmdRefreshComportList.Enabled = True
            cmdConnect.Enabled = True
        End If

        UpdateDebugButtonStates()
    End Sub

#End Region





End Class
