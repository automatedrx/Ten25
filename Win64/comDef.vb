Module comDef

    Public Enum enumChantype As Integer
        Unknown = 0
        Master = 1
        Motor = 2
        Tens = 3
        Aux = 4
    End Enum

    Public Enum CommandEnum As Integer
        cmdNoop = 0
        cmdTensOutput = 1
        cmdMotorOutput = 2
        cmdGoTo = 3
        cmdEnd = 4
        cmdTest = 5
        cmdSet = 6
        cmdDelay = 7
        cmdProgControl = 8
    End Enum

    Public Enum DataSourceEnum As Integer
        ' = {"Direct", "Prog Var", "Sys Var", "Dig Input", "Dig Output", "Math"}
        dsDirect = 0
        dsProgramVar = 1
        dsSysVar = 2
        dsDigIn = 3
        dsDigOut = 4
        dsMath = 5
    End Enum

    Public Enum OpTypeEnum As Integer
        opUnknown = 0
        opLoadProgram = 1
        opStart = 2
        opStop = 3
        opPause = 4
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
        'ChanProglinesMax = AscW("h")
        'SysProgLinesMax = AscW("i")
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

    Public Enum fileDataEnum As Byte
        numChannelProgs = AscW("a")
        numSysProgs = AscW("b")

    End Enum

    Public Enum paramArrayEnum As Byte
        DeviceInfo = AscW("1")                '[RadioId, NumChannels, NumMotorChannels, NumTensChannels, NumSysChannels]
        chanMinMaxInfo = AscW("2") 'Index req'd
        chanStats = AscW("3") 'Index req'd    ' chanEnabled, progState, curLineNum, startVal, endVal, modDuration, percentComplete, RepeatsRemaining, chanCurVal, chanCurIntensity
        'sysStats = AscW("4")
        progLineData = AscW("5") 'TWO indexes req'd: one for progNum and one for lineNum
    End Enum

    Public commandVals As String() = {"No Op", "Tens", "Motor", "GoTo", "End", "Test", "Set", "Delay", "ProgControl"}
    Public progStateString As String() ' = {"Unknown", "Empty", "Stopped", "Paused", "Running", "Line Complete", "End"}
    Public dataSourceString As String() = {"Direct", "Prog Var", "Sys Var", "Dig Input", "Dig Output", "Math"}
    Public masterChanNameString As String() = {"Master"}
    Public tensChanNameString As String() = {"Tens 1", "Tens 2"}
    Public auxChanNameString() As String
    Public allChannelsString As String() = {"Motor 1", "Motor 2", "Tens 1", "Tens 2", "System 1"}
    Public motorChanNameString As String() = {"Motor 1", "Motor 2"}
    Public polarityString As String() = {"Forward", "Reverse", "Fwd -Toggle Pulse", "Rev -Toggle Pulse", "Fwd -Toggle Cycle", "Rev -Toggle Cycle"}
    Public testOperationString As String() = {" < ", " <= ", " = ", " >= ", "Is Between", "Eq or Between", "Outside Of"} '{"<", "<=", "=", ">=", ">", "> | <", "=>|<=", "<|>"}
    Public mathFunctionString As String() = {" ", "+", "-", "x", "/", "mod"}

    Public programOpTypeString As String() = {" ", "Load Program", "Start Program", "Stop Program", "Pause Program"}


    Public noOpParamArray As String() = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
End Module
