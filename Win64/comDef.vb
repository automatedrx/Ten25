Imports System.ComponentModel.DataAnnotations
Imports System.DirectoryServices.ActiveDirectory
Imports System.Net

Module comDef

    Public Const DataFieldLen = 20
    Public Const MAX_32BIT As Integer = 2000000000 '(2 ^ 32) - 1
    Public Const MAX_16BIT As Integer = 65535
    Public Const MAX_8BIT As Integer = 255

    Public tensCurVersion As String = "TENS2503"

    Public Structure Program
        Dim progLine As List(Of Integer())
        Dim progName As String
        Dim varName As List(Of String)
        Dim timerName As List(Of String)
    End Structure


    Public Enum DataFieldEnum As Integer 'index number for fields in each line of program data
        dfCommand = 0
        dfChannel = 1
        dfGotoTrue = 2
        dfGotoFalse = 3
        df81S = 4
        df81V1 = 5
        df81V2 = 6
        df82S = 7
        df82V1 = 8
        df82V2 = 9
        dfPolarity = 10
        df321S = 11
        df321V1 = 12
        df321V2 = 13
        df322S = 14
        df322V1 = 15
        df322V2 = 16
        df323S = 17
        df323V1 = 18
        df323V2 = 19
        'dfRepeats = 20
    End Enum

    Public Enum enumChantype As Integer
        Unknown = 0
        Master = 1
        Motor = 2
        Tens = 3
        Aux = 4
    End Enum

    Public Enum ProgStateEnum As Integer
        progState_Unknown = 0
        progState_Empty = 1
        progState_Stopped = 2
        progState_Paused = 3
        progState_Running = 4
        progState_LineComplete = 5
        progState_End = 6
    End Enum

    Public Enum CommandEnum As Integer
        cmdNoop = 0
        cmdTenMotOutput = 1
        'cmdMotorOutput = 2
        cmdGoTo = 2
        cmdEnd = 3
        cmdTest = 4
        cmdSet = 5
        cmdDelay = 6
        cmdProgControl = 7
        'cmdSetModifier = 8
        cmdDisplay = 8
        cmdSendMsg = 9
        cmdLastEntry = 10
    End Enum

    Public Enum DataSourceEnum As Integer
        ' = {"Direct", "Prog Var", "Sys Var", "Chan Setting", "Sys Setting", "Dig Input", "Dig Output", "Math"}
        dsDirect = 0
        dsProgramVar = 1
        dsSysVar = 2
        dsChanSetting = 3
        dsSysSetting = 4
        dsDigIn = 5
        dsDigOut = 6
        dsRandom = 7
        dsTimer = 8
    End Enum

    Public Enum DataSourceChanSettingEnum As Integer
        dscsSpeed = 0
        dscsOrigOpDuration = 1
        dscsOrigPdDuration = 2
        dscsOrigTotalDuration = 3
        dscsModOpDuration = 4
        dscsModPdDuration = 5
        dscsModTotalDuration = 6
        dscsElapsedOpDuration = 7
        dscsElapsedPdDuration = 8
        dscsElapsedTotalDuration = 9
        dscsRemainingOpDuration = 10
        dscsRemainingPdDuration = 11
        dscsRemainingTotalDuration = 12
        dscsPercentComplete = 13
        dscsStartVal = 14
        dscsEndVal = 15
        dscsCurVal = 16
        dscsPolarity = 17
        dscsIntensity = 18
        dscsIntensityMin = 19
        dscsIntensityMax = 20
        dscsCurProgNum = 21
        dscsCurLineNum = 22
        dscsCurChanNum = 23
    End Enum

    Public Enum DataSourceSysSettingEnum As Integer
        dsssZRotation = 0
        dsssUpDirection
        dsssStepCount
        dsssAudioTotal
        dsssAudioLow
        dsssAudioMid
        dsssAudioHigh
    End Enum


    Public Enum WaveformEnum As Integer
        wfNone = 0
        wfRamp = 1
        wfTriangle = 2
        'wfTrapezoid = 3
        wfSine = 3
        'wfPulse = 5
    End Enum

    Public Enum QuadrantEnum As Integer
        quadMidHi = 0
        quadMidHiMid = 1
        quadHiMid = 2
        quadMidLow = 3
        quadMidLowMid = 4
        quadLowMid = 5
        quadLowHi = 6
        quadLowHiLow = 7
        quadHiLowHi = 8

        quadHiLow = 9
        quadMidHiLowMid = 10
        quadMidLowHiMid = 11
    End Enum

    Public Enum OpTypeEnum As Integer
        opUnknown = 0
        opLoadProgramAndPause = 1
        opLoadProgramAndRun = 2
        opStart = 3
        opStop = 4
        opPause = 5
    End Enum

    Public Enum commandTypeEnum As Byte
        ACK = AscW("A") '65 'A'
        NAK = AscW("n")
        SetParamArray = AscW("b")       '//Followed by an index num
        SetSingleParam = AscW("d")      '//Followed by a pStatEnum
        GetParamArray = AscW("e")       '//Followed by an index num
        GetSingleParam = AscW("g")      '//Followed by a pStatEnum
        SetFileData = AscW("k")
        GetFileData = AscW("p")

        ResetDevice = AscW("R")
        EraseDevice = AscW("X")
    End Enum

    Public Enum pStatEnum As Byte
        None = 0
        BattLevel = AscW("1")
        Charging = AscW("2")
        Charged = AscW("3")

        NumMotorChannels = AscW("a")
        NumTensChannels = AscW("b")
        NumChannels = AscW("c")
        NumInputs = AscW("d")
        NumOutputs = AscW("e")
        MotIndexStart = AscW("f")
        TensIndexStart = AscW("g")
        NumProgVariables = AscW("h")
        NumProgTimers = AscW("i")
        TensProgCurVer = AscW("j")
        TensProgMinVer = AscW("k")

        ChanType = AscW("l")
        ChanEnabled = AscW("m")
        ProgNumber = AscW("n")
        ProgState = AscW("o")
        CurLineNum = AscW("p")
        ChanCurPWidthPct = AscW("q")
        ChanCurIntensityPct = AscW("r")
        CurSpeed = AscW("s")
        MinIntensity = AscW("t")
        MaxIntensity = AscW("u")
        SwapPolarity = AscW("v")

        NumberOfPrograms = AscW("A")    'Includes chanProgs and sysProgs
        ProgramLength = AscW("B")       'Index Required for program number
        ProgramName = AscW("C")         'Index Required for program name
    End Enum

    Public Enum progType As Integer
        Unknown = 0
        'ChanProgram = 1
        'SysProgram = 2

        SystemMaster = 0
        Motor = 1
        Tens = 2
        Aux = 3
    End Enum

    Public Enum MathOpEnum As Integer
        mathOpNone = 0
        mathOpPlus = 1
        mathOpMinus = 2
        mathOpMultiply = 3
        mathOpDivide = 4
        mathOpModulo = 5
    End Enum


    Public Enum fileDataEnum As Byte
        numChannelProgs = AscW("a")
        numSysProgs = AscW("b")

    End Enum

    Public Enum paramArrayEnum As Byte
        None = 0
        DeviceInfo = AscW("1")                '[RadioId, NumChannels, NumMotorChannels, NumTensChannels, NumSysChannels]
        chanMinMaxInfo = AscW("2") 'Index req'd
        chanStats = AscW("3") 'Index req'd    ' chanEnabled, progState, curLineNum, startVal, endVal, modDuration, percentComplete, RepeatsRemaining, chanCurVal, chanCurIntensity
        'sysStats = AscW("4")
        progLineData = AscW("5") 'TWO indexes req'd: one for progNum and one for lineNum
    End Enum

    Public commandVals As String() = {"No Op", "Tens/Motor", "GoTo", "End", "Test", "Set", "Delay", "ProgControl", "Display", "Send Msg"}

    Public progStateString As String() ' = {"Unknown", "Empty", "Stopped", "Paused", "Running", "Line Complete", "End"}
    Public dataSourceString As String() = {"Direct", "Prog Var", "Sys Var", "Chan Setting", "Sys Setting", "Dig Input", "Dig Output", "Rand Num", "Timer"}
    Public dataSourceChanSettingString As String() = {"Speed", "Org OPTime", "Org PdTime", "Org TotTime",
                                                        "Mod OpTime", "Mod PdTime", "Mod TotTime",
                                                        "Elps OpTime", "Elps PdTime", "ElpsTotTime",
                                                        "Rem OpTime", "Rem PdTime", "Rem TotTime", "% Complete",
                                                        "Start Val", "End Val", "Cur Val", "Polarity",
                                                        "Intensity", "Intens Min", "Intens Max",
                                                        "CurProg Num", "CurLine Num", "CurChan Num"}

    Public dataSourceSysSettingString As String() = {"Z Rotation", "Up Direction", "Step Count", "Audio-Total", "Audio-Low", "Audio-Mid", "Audio-High"}

    'Public masterChanNameString As String() = {"Master"}
    'Public tensChanNameString As String() = {"Tens 1", "Tens 2"}
    'Public auxChanNameString() As String
    'Public allChannelsString As String() = {"Master", "Motor 1", "Motor 2", "Tens 1", "Tens 2", "Aux 1", "Aux 2"}
    'Public allChannelsStringPlusThisChan As String() = {"Master", "Motor 1", "Motor 2", "Tens 1", "Tens 2", "Aux 1", "Aux 2", "This Channel"}
    'Public motorChanNameString As String() = {"Motor 1", "Motor 2"}
    Public waveformString As String() = {"None", "Ramp", "Triangle", "Sine"}
    Public quadrantString As String() = {"Mid-Hi", "Mid-Hi-Mid", "Hi-Mid", "Mid-Low", "Mid-Low-Mid", "Low-Mid", "Low-Hi", "Low-Hi-Low", "Hi-Low-Hi", "Hi-Low", "Mid-HiLowHi", "Mid-LowHiLo"}
    Public polarityString As String() = {"Forward", "Reverse", "Fwd -Toggle Pulse", "Rev -Toggle Pulse", "Fwd -Toggle Cycle", "Rev -Toggle Cycle"}
    Public testOperationString As String() = {" < ", " <= ", " = ", "<>", " >= ", " > ", "Between", "Equal or Between", "Outside Of"} '{"<", "<=", "=", ">=", ">", "> | <", "=>|<=", "<|>"}
    Public mathFunctionString As String() = {" ", "+", "-", "x", "/", "mod"}



    Public programOpTypeString As String() = {" ", "Load Prog & Wait", "Load Prog & Run", "Start Program", "Stop Program", "Pause Program"}

    Public noOpParamArray As String() = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}



    Public Structure Channel_t
        Dim Name As String
        Dim Type As enumChantype
        Dim SpeedMin As Integer
        Dim SpeedMax As Integer
    End Structure

    Public Structure Device_t
        Dim Name As String
        Dim DeviceType As DeviceType_t
        Dim RadioId As Integer
        Dim serialNumber As String
        Dim Version As String
        Dim Channel As List(Of Channel_t)
        Dim NumMotorChans As Integer
        Dim NumTensChans As Integer
        Dim NumAuxChans As Integer
        Dim NumDigInputs As Integer
        Dim NumDigOutputs As Integer
        Dim Prog As List(Of Program)
        Dim NumVarsPerProgram
        Dim NumTimersPerProgram
        Dim FileName As String
        Dim UnsavedChanges As Boolean
        Dim UndownloadedChanges As Boolean

        Dim ProjectName As String
        Dim CurProgNum As Integer
        Dim curLineNum As Integer

        Dim motorChanNameString() As String
        Dim tensChanNameString() As String
        Dim auxChanNameString() As String
        Dim allChannelsString() As String
        Dim allChannelsStringPlusThisChan() As String
    End Structure



    Public Enum DeviceType_t As Integer
        Tens2410B1
        Tens2410E1
        Tens2503F1
    End Enum

    Public deviceTypeString As String() = {"2410B1", "2410E1", "2503F1"}

    Public Sub SetupChannels(ByRef Dev As Device_t, ByVal motChans As Integer,
                               ByVal tensChans As Integer, ByVal auxChans As Integer)

        Dev.NumMotorChans = motChans
        Dev.NumTensChans = tensChans
        Dev.NumAuxChans = auxChans

        Dim s As Integer = 1
        Dim m As Integer = 0
        Dim t As Integer = 0
        Dim a As Integer = 0

        ReDim Dev.allChannelsString(1 + motChans + tensChans + auxChans - 1) 'Entries for actual channels (does not include extra at the end for "This Chan".
        ReDim Dev.allChannelsStringPlusThisChan(1 + motChans + tensChans + auxChans + 1 - 1) 'Includes an entry at the end for "This Chan"

        Dev.allChannelsString(0) = New String("Master")
        Dev.allChannelsStringPlusThisChan(0) = Dev.allChannelsString(0)
        Dev.Channel = New List(Of Channel_t)
        Dim MasterChan As New Channel_t
        MasterChan.Name = "Master"
        MasterChan.Type = enumChantype.Master
        Dev.Channel.Add(MasterChan)

        If Dev.NumMotorChans > 0 Then
            ReDim Dev.motorChanNameString(Dev.NumMotorChans - 1)
            For m = 0 To Dev.NumMotorChans - 1
                Dev.motorChanNameString(m) = New String("Motor " & (m + 1))
                Dev.allChannelsString(s + m) = New String("Motor" & (m + 1))
                Dev.allChannelsStringPlusThisChan(s + m) = Dev.allChannelsString(s + m)

                Dim tmpChan As New Channel_t
                tmpChan.Name = Dev.motorChanNameString(m)
                tmpChan.Type = enumChantype.Motor
                Dev.Channel.Add(tmpChan)
            Next
        End If

        If Dev.NumTensChans > 0 Then
            ReDim Dev.tensChanNameString(Dev.NumTensChans - 1)
            For t = 0 To Dev.NumTensChans - 1
                Dev.tensChanNameString(t) = New String("Tens " & (t + 1))
                Dev.allChannelsString(s + m + t) = New String("Tens " & (t + 1))
                Dev.allChannelsStringPlusThisChan(s + m + t) = Dev.allChannelsString(s + m + t)

                Dim tmpChan As New Channel_t
                tmpChan.Name = Dev.tensChanNameString(t)
                tmpChan.Type = enumChantype.Tens
                Dev.Channel.Add(tmpChan)
            Next
        End If

        If Dev.NumAuxChans > 0 Then
            ReDim Dev.auxChanNameString(Dev.NumAuxChans - 1)
            For a = 0 To Dev.NumAuxChans - 1
                Dev.auxChanNameString(a) = New String("Aux " & (a + 1))
                Dev.allChannelsString(s + m + t + a) = New String("Aux " & (a + 1))
                Dev.allChannelsStringPlusThisChan(s + m + t + a) = Dev.allChannelsString(s + m + t + a)

                Dim tmpChan As New Channel_t
                tmpChan.Name = Dev.auxChanNameString(a)
                tmpChan.Type = enumChantype.Aux
                Dev.Channel.Add(tmpChan)
            Next
        End If

        Dev.allChannelsStringPlusThisChan(Dev.allChannelsString.Length) = "This Channel"

    End Sub

    Public Function GetDeviceTemplate(ModelType As DeviceType_t) As Device_t
        Dim dev As New Device_t

        dev.Name = New String(" ", 20)
        dev.RadioId = 1
        dev.serialNumber = New String(" ", 32)
        dev.Prog = New List(Of Program)
        dev.FileName = New String(" ", 256)
        dev.UnsavedChanges = False
        dev.UndownloadedChanges = False
        dev.ProjectName = New String("NewProject").PadRight(20)
        dev.CurProgNum = -1
        dev.curLineNum = -1


        Select Case ModelType
            Case DeviceType_t.Tens2410B1
                dev.DeviceType = DeviceType_t.Tens2410B1
                dev.Version = tensCurVersion ' New String("TENS2503")
                SetupChannels(dev, 2, 2, 2)
                dev.NumDigInputs = 0
                dev.NumDigOutputs = 0
                dev.NumVarsPerProgram = 30
                dev.NumTimersPerProgram = 20

            Case DeviceType_t.Tens2410E1
                dev.DeviceType = DeviceType_t.Tens2410E1
                dev.Version = tensCurVersion ' New String("TENS2503")
                SetupChannels(dev, 2, 3, 2)
                dev.NumDigInputs = 0
                dev.NumDigOutputs = 0
                dev.NumVarsPerProgram = 30
                dev.NumTimersPerProgram = 20
            Case DeviceType_t.Tens2503F1
                dev.DeviceType = DeviceType_t.Tens2503F1
                dev.Version = tensCurVersion 'New String("TENS2503")
                SetupChannels(dev, 2, 3, 2)
                dev.NumDigInputs = 0
                dev.NumDigOutputs = 0
                dev.NumVarsPerProgram = 30
                dev.NumTimersPerProgram = 20

        End Select
        Return dev
    End Function
End Module
