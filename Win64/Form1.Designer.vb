<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        components = New ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
        lstProgDisplay = New ListBox()
        cmsProgSentences = New ContextMenuStrip(components)
        cmsPsInsertLine = New ToolStripMenuItem()
        cmsPsDuplicateLine = New ToolStripMenuItem()
        cmsPsAddLine = New ToolStripMenuItem()
        ToolStripSeparator2 = New ToolStripSeparator()
        cmsPsMoveLineUp = New ToolStripMenuItem()
        cmsPsMoveLineDown = New ToolStripMenuItem()
        ToolStripSeparator1 = New ToolStripSeparator()
        cmsPsDeleteLine = New ToolStripMenuItem()
        cmdAddRow = New Button()
        cmdDeleteRow = New Button()
        cmdMoveRowUp = New Button()
        cmdInsertRow = New Button()
        lstPrograms = New ListBox()
        cmsPrograms = New ContextMenuStrip(components)
        cmsProgAddBlankProgramToEnd = New ToolStripMenuItem()
        cmsProgInsertBlankProgram = New ToolStripMenuItem()
        cmsProgImportProgram = New ToolStripMenuItem()
        cmsProgDuplicateProgram = New ToolStripMenuItem()
        ToolStripSeparator3 = New ToolStripSeparator()
        cmsProgMoveProgramTowardsBeginning = New ToolStripMenuItem()
        cmsProgMoveProgramTowardsEnd = New ToolStripMenuItem()
        ToolStripSeparator4 = New ToolStripSeparator()
        cmsProgEditVarNames = New ToolStripMenuItem()
        cmsProgEditTimerNames = New ToolStripMenuItem()
        ToolStripSeparator13 = New ToolStripSeparator()
        cmsProgDeleteProgram = New ToolStripMenuItem()
        cmdMoveDown = New Button()
        cmdDuplicateProgLine = New Button()
        Button3 = New Button()
        lblProgName = New Label()
        txtProgName = New TextBox()
        cmdEditVarNames = New Button()
        tabBottom = New TabControl()
        TabBottomProgram = New TabPage()
        SplitContainer8 = New SplitContainer()
        ToolStripContainer1 = New ToolStripContainer()
        ToolStrip2 = New ToolStrip()
        tscmdNewProject = New ToolStripButton()
        ToolStripSeparator14 = New ToolStripSeparator()
        tscmdOpenProjectFromFile = New ToolStripButton()
        tscmdSaveProjectToFile = New ToolStripButton()
        ToolStripSeparator15 = New ToolStripSeparator()
        tscmdOpenProgramFromDevice = New ToolStripButton()
        tscmdSaveProgramToDevice = New ToolStripButton()
        ToolStripSeparator16 = New ToolStripSeparator()
        ToolStripSeparator17 = New ToolStripSeparator()
        tsbtnDebugRun = New ToolStripButton()
        tsbtnDebugPause = New ToolStripButton()
        tsbtnDebugStep = New ToolStripButton()
        Panel1 = New Panel()
        SplitContainer1 = New SplitContainer()
        SplitContainer5 = New SplitContainer()
        chkLiveTrackLines = New CheckBox()
        SplitContainer4 = New SplitContainer()
        TabControl1 = New TabControl()
        tabpageLineDetails = New TabPage()
        UcProgLineEdit1 = New ucProgLineEdit()
        tabpageVariables = New TabPage()
        SplitContainerVariables = New SplitContainer()
        cboVariablesChannel = New ComboBox()
        dgvVariables = New DataGridView()
        tabpageTimers = New TabPage()
        SplitContainerTimers = New SplitContainer()
        cboTimersChannel = New ComboBox()
        dgvTimers = New DataGridView()
        pnlProgramList = New Panel()
        SplitContainer2 = New SplitContainer()
        txtDeviceName = New TextBox()
        Label4 = New Label()
        lblDeviceType = New Label()
        Label2 = New Label()
        Label1 = New Label()
        txtProjectName = New TextBox()
        mnuTabProgram = New MenuStrip()
        mnuMainProject = New ToolStripMenuItem()
        CreateNewProjectToolStripMenuItem = New ToolStripMenuItem()
        tsmDeviceType = New ToolStripMenuItem()
        ToolStripSeparator11 = New ToolStripSeparator()
        OpenProjectFromFileToolStripMenuItem = New ToolStripMenuItem()
        SaveProjectToolStripMenuItem = New ToolStripMenuItem()
        SaveProjectAsToolStripMenuItem = New ToolStripMenuItem()
        ToolStripSeparator12 = New ToolStripSeparator()
        OpenProjectFromDeviceToolStripMenuItem = New ToolStripMenuItem()
        DownloadProjectToDeviceToolStripMenuItem = New ToolStripMenuItem()
        mnuMainPrograms = New ToolStripMenuItem()
        AddNewBlankProgramToolStripMenuItem = New ToolStripMenuItem()
        mnuAddBlankProgramAtIndex = New ToolStripMenuItem()
        mnuAddBlankProgramAtEnd = New ToolStripMenuItem()
        ImportExistingProgramToolStripMenuItem = New ToolStripMenuItem()
        mnuDuplicateProgram = New ToolStripMenuItem()
        ToolStripSeparator5 = New ToolStripSeparator()
        MoveProgramTowardsBeginningToolStripMenuItem = New ToolStripMenuItem()
        MoveProgramTowardsEndToolStripMenuItem = New ToolStripMenuItem()
        ToolStripSeparator6 = New ToolStripSeparator()
        DeleteProgramToolStripMenuItem = New ToolStripMenuItem()
        mnuProgLines = New ToolStripMenuItem()
        mnuAddProgLine = New ToolStripMenuItem()
        mnuInsertProgLine = New ToolStripMenuItem()
        mnuDuplicateProgLine = New ToolStripMenuItem()
        ToolStripSeparator7 = New ToolStripSeparator()
        mnuMoveProgLineUp = New ToolStripMenuItem()
        mnuMoveProgramLineDown = New ToolStripMenuItem()
        ToolStripSeparator8 = New ToolStripSeparator()
        mnuDeleteProgramLine = New ToolStripMenuItem()
        ToolStripMenuItem2 = New ToolStripMenuItem()
        TabPage2 = New TabPage()
        SplitContainer3 = New SplitContainer()
        SplitContainer6 = New SplitContainer()
        SplitContainer7 = New SplitContainer()
        ToolStrip1 = New ToolStrip()
        cmdResets = New ToolStripDropDownButton()
        cmdResetDevice = New ToolStripMenuItem()
        cmdEraseDevice = New ToolStripMenuItem()
        cboComPorts = New ToolStripComboBox()
        cmdRefreshComportList = New ToolStripButton()
        cmdConnect = New ToolStripButton()
        ToolStripSeparator10 = New ToolStripSeparator()
        ToolStripLabelAutoStatus = New ToolStripLabel()
        cmdtsChanMonAutoStatusOff = New ToolStripButton()
        cmdtsChanMonAutoStatusOn = New ToolStripButton()
        ToolStripSeparator9 = New ToolStripSeparator()
        lblDevStatus = New ToolStripLabel()
        cmdStop = New Button()
        UcChanControl1 = New ucChanControl()
        cmdAutostatus = New ToolStripDropDownButton()
        ofdProject = New OpenFileDialog()
        sfdProject = New SaveFileDialog()
        tmrNewMessages = New Timer(components)
        StatusStrip1 = New StatusStrip()
        ssmProgressBar = New ToolStripProgressBar()
        ssmStatusText = New ToolStripStatusLabel()
        bwDLProgFromDevice = New ComponentModel.BackgroundWorker()
        bwULProgToDevice = New ComponentModel.BackgroundWorker()
        tmrClearStatusbar = New Timer(components)
        tmrCheckComportStatus = New Timer(components)
        tmrCheckAutostatus = New Timer(components)
        SplitContainer9 = New SplitContainer()
        cmsProgSentences.SuspendLayout()
        cmsPrograms.SuspendLayout()
        tabBottom.SuspendLayout()
        TabBottomProgram.SuspendLayout()
        CType(SplitContainer8, ComponentModel.ISupportInitialize).BeginInit()
        SplitContainer8.Panel1.SuspendLayout()
        SplitContainer8.Panel2.SuspendLayout()
        SplitContainer8.SuspendLayout()
        ToolStripContainer1.TopToolStripPanel.SuspendLayout()
        ToolStripContainer1.SuspendLayout()
        ToolStrip2.SuspendLayout()
        Panel1.SuspendLayout()
        CType(SplitContainer1, ComponentModel.ISupportInitialize).BeginInit()
        SplitContainer1.Panel1.SuspendLayout()
        SplitContainer1.Panel2.SuspendLayout()
        SplitContainer1.SuspendLayout()
        CType(SplitContainer5, ComponentModel.ISupportInitialize).BeginInit()
        SplitContainer5.Panel1.SuspendLayout()
        SplitContainer5.Panel2.SuspendLayout()
        SplitContainer5.SuspendLayout()
        CType(SplitContainer4, ComponentModel.ISupportInitialize).BeginInit()
        SplitContainer4.Panel1.SuspendLayout()
        SplitContainer4.Panel2.SuspendLayout()
        SplitContainer4.SuspendLayout()
        TabControl1.SuspendLayout()
        tabpageLineDetails.SuspendLayout()
        tabpageVariables.SuspendLayout()
        CType(SplitContainerVariables, ComponentModel.ISupportInitialize).BeginInit()
        SplitContainerVariables.Panel1.SuspendLayout()
        SplitContainerVariables.Panel2.SuspendLayout()
        SplitContainerVariables.SuspendLayout()
        CType(dgvVariables, ComponentModel.ISupportInitialize).BeginInit()
        tabpageTimers.SuspendLayout()
        CType(SplitContainerTimers, ComponentModel.ISupportInitialize).BeginInit()
        SplitContainerTimers.Panel1.SuspendLayout()
        SplitContainerTimers.Panel2.SuspendLayout()
        SplitContainerTimers.SuspendLayout()
        CType(dgvTimers, ComponentModel.ISupportInitialize).BeginInit()
        pnlProgramList.SuspendLayout()
        CType(SplitContainer2, ComponentModel.ISupportInitialize).BeginInit()
        SplitContainer2.Panel1.SuspendLayout()
        SplitContainer2.Panel2.SuspendLayout()
        SplitContainer2.SuspendLayout()
        mnuTabProgram.SuspendLayout()
        CType(SplitContainer3, ComponentModel.ISupportInitialize).BeginInit()
        SplitContainer3.Panel1.SuspendLayout()
        SplitContainer3.Panel2.SuspendLayout()
        SplitContainer3.SuspendLayout()
        CType(SplitContainer6, ComponentModel.ISupportInitialize).BeginInit()
        SplitContainer6.Panel1.SuspendLayout()
        SplitContainer6.Panel2.SuspendLayout()
        SplitContainer6.SuspendLayout()
        CType(SplitContainer7, ComponentModel.ISupportInitialize).BeginInit()
        SplitContainer7.Panel1.SuspendLayout()
        SplitContainer7.Panel2.SuspendLayout()
        SplitContainer7.SuspendLayout()
        ToolStrip1.SuspendLayout()
        StatusStrip1.SuspendLayout()
        CType(SplitContainer9, ComponentModel.ISupportInitialize).BeginInit()
        SplitContainer9.Panel1.SuspendLayout()
        SplitContainer9.Panel2.SuspendLayout()
        SplitContainer9.SuspendLayout()
        SuspendLayout()
        ' 
        ' lstProgDisplay
        ' 
        lstProgDisplay.BackColor = SystemColors.Window
        lstProgDisplay.ContextMenuStrip = cmsProgSentences
        lstProgDisplay.Dock = DockStyle.Fill
        lstProgDisplay.FormattingEnabled = True
        lstProgDisplay.HorizontalScrollbar = True
        lstProgDisplay.ItemHeight = 15
        lstProgDisplay.Items.AddRange(New Object() {"Line 0: blah", "Line 1: foo", "Line 2: Bar"})
        lstProgDisplay.Location = New Point(0, 0)
        lstProgDisplay.Name = "lstProgDisplay"
        lstProgDisplay.ScrollAlwaysVisible = True
        lstProgDisplay.Size = New Size(730, 442)
        lstProgDisplay.TabIndex = 3
        ' 
        ' cmsProgSentences
        ' 
        cmsProgSentences.ImageScalingSize = New Size(32, 32)
        cmsProgSentences.Items.AddRange(New ToolStripItem() {cmsPsInsertLine, cmsPsDuplicateLine, cmsPsAddLine, ToolStripSeparator2, cmsPsMoveLineUp, cmsPsMoveLineDown, ToolStripSeparator1, cmsPsDeleteLine})
        cmsProgSentences.Name = "cmsProgSentences"
        cmsProgSentences.Size = New Size(245, 148)
        ' 
        ' cmsPsInsertLine
        ' 
        cmsPsInsertLine.Name = "cmsPsInsertLine"
        cmsPsInsertLine.ShortcutKeys = Keys.Control Or Keys.Insert
        cmsPsInsertLine.Size = New Size(244, 22)
        cmsPsInsertLine.Text = "Insert Blank Line"
        ' 
        ' cmsPsDuplicateLine
        ' 
        cmsPsDuplicateLine.Name = "cmsPsDuplicateLine"
        cmsPsDuplicateLine.ShortcutKeys = Keys.Control Or Keys.D
        cmsPsDuplicateLine.Size = New Size(244, 22)
        cmsPsDuplicateLine.Text = "Duplicate Line"
        ' 
        ' cmsPsAddLine
        ' 
        cmsPsAddLine.Name = "cmsPsAddLine"
        cmsPsAddLine.ShortcutKeys = Keys.Control Or Keys.End
        cmsPsAddLine.Size = New Size(244, 22)
        cmsPsAddLine.Text = "Add Blank Line to End"
        ' 
        ' ToolStripSeparator2
        ' 
        ToolStripSeparator2.Name = "ToolStripSeparator2"
        ToolStripSeparator2.Size = New Size(241, 6)
        ' 
        ' cmsPsMoveLineUp
        ' 
        cmsPsMoveLineUp.Name = "cmsPsMoveLineUp"
        cmsPsMoveLineUp.ShortcutKeys = Keys.Control Or Keys.Up
        cmsPsMoveLineUp.Size = New Size(244, 22)
        cmsPsMoveLineUp.Text = "Move Line Up"
        ' 
        ' cmsPsMoveLineDown
        ' 
        cmsPsMoveLineDown.Name = "cmsPsMoveLineDown"
        cmsPsMoveLineDown.ShortcutKeys = Keys.Control Or Keys.Down
        cmsPsMoveLineDown.Size = New Size(244, 22)
        cmsPsMoveLineDown.Text = "Move Line Down"
        ' 
        ' ToolStripSeparator1
        ' 
        ToolStripSeparator1.Name = "ToolStripSeparator1"
        ToolStripSeparator1.Size = New Size(241, 6)
        ' 
        ' cmsPsDeleteLine
        ' 
        cmsPsDeleteLine.Name = "cmsPsDeleteLine"
        cmsPsDeleteLine.ShortcutKeys = Keys.Control Or Keys.Delete
        cmsPsDeleteLine.Size = New Size(244, 22)
        cmsPsDeleteLine.Text = "Delete Line"
        ' 
        ' cmdAddRow
        ' 
        cmdAddRow.Location = New Point(428, 3)
        cmdAddRow.Name = "cmdAddRow"
        cmdAddRow.Size = New Size(93, 23)
        cmdAddRow.TabIndex = 4
        cmdAddRow.Text = "add blank line"
        cmdAddRow.UseVisualStyleBackColor = True
        ' 
        ' cmdDeleteRow
        ' 
        cmdDeleteRow.Location = New Point(527, 24)
        cmdDeleteRow.Name = "cmdDeleteRow"
        cmdDeleteRow.Size = New Size(75, 23)
        cmdDeleteRow.TabIndex = 5
        cmdDeleteRow.Text = "delete"
        cmdDeleteRow.UseVisualStyleBackColor = True
        ' 
        ' cmdMoveRowUp
        ' 
        cmdMoveRowUp.Location = New Point(608, 3)
        cmdMoveRowUp.Name = "cmdMoveRowUp"
        cmdMoveRowUp.Size = New Size(89, 23)
        cmdMoveRowUp.TabIndex = 6
        cmdMoveRowUp.Text = "move up"
        cmdMoveRowUp.UseVisualStyleBackColor = True
        ' 
        ' cmdInsertRow
        ' 
        cmdInsertRow.Location = New Point(527, 3)
        cmdInsertRow.Name = "cmdInsertRow"
        cmdInsertRow.Size = New Size(75, 23)
        cmdInsertRow.TabIndex = 7
        cmdInsertRow.Text = "insert"
        cmdInsertRow.UseVisualStyleBackColor = True
        ' 
        ' lstPrograms
        ' 
        lstPrograms.ContextMenuStrip = cmsPrograms
        lstPrograms.Dock = DockStyle.Fill
        lstPrograms.FormattingEnabled = True
        lstPrograms.ItemHeight = 15
        lstPrograms.Location = New Point(0, 0)
        lstPrograms.Name = "lstPrograms"
        lstPrograms.ScrollAlwaysVisible = True
        lstPrograms.Size = New Size(136, 340)
        lstPrograms.TabIndex = 8
        ' 
        ' cmsPrograms
        ' 
        cmsPrograms.ImageScalingSize = New Size(32, 32)
        cmsPrograms.Items.AddRange(New ToolStripItem() {cmsProgAddBlankProgramToEnd, cmsProgInsertBlankProgram, cmsProgImportProgram, cmsProgDuplicateProgram, ToolStripSeparator3, cmsProgMoveProgramTowardsBeginning, cmsProgMoveProgramTowardsEnd, ToolStripSeparator4, cmsProgEditVarNames, cmsProgEditTimerNames, ToolStripSeparator13, cmsProgDeleteProgram})
        cmsPrograms.Name = "cmsProgSentences"
        cmsPrograms.Size = New Size(306, 220)
        ' 
        ' cmsProgAddBlankProgramToEnd
        ' 
        cmsProgAddBlankProgramToEnd.Name = "cmsProgAddBlankProgramToEnd"
        cmsProgAddBlankProgramToEnd.Size = New Size(305, 22)
        cmsProgAddBlankProgramToEnd.Text = "Add Blank Program to then End"
        ' 
        ' cmsProgInsertBlankProgram
        ' 
        cmsProgInsertBlankProgram.Name = "cmsProgInsertBlankProgram"
        cmsProgInsertBlankProgram.Size = New Size(305, 22)
        cmsProgInsertBlankProgram.Text = "Insert Blank Program"
        ' 
        ' cmsProgImportProgram
        ' 
        cmsProgImportProgram.Name = "cmsProgImportProgram"
        cmsProgImportProgram.Size = New Size(305, 22)
        cmsProgImportProgram.Text = "Import Existing Program"
        ' 
        ' cmsProgDuplicateProgram
        ' 
        cmsProgDuplicateProgram.Name = "cmsProgDuplicateProgram"
        cmsProgDuplicateProgram.Size = New Size(305, 22)
        cmsProgDuplicateProgram.Text = "Duplicate Program"
        ' 
        ' ToolStripSeparator3
        ' 
        ToolStripSeparator3.Name = "ToolStripSeparator3"
        ToolStripSeparator3.Size = New Size(302, 6)
        ' 
        ' cmsProgMoveProgramTowardsBeginning
        ' 
        cmsProgMoveProgramTowardsBeginning.Name = "cmsProgMoveProgramTowardsBeginning"
        cmsProgMoveProgramTowardsBeginning.ShortcutKeys = Keys.Control Or Keys.Left
        cmsProgMoveProgramTowardsBeginning.Size = New Size(305, 22)
        cmsProgMoveProgramTowardsBeginning.Text = "Move Program Towards Start"
        ' 
        ' cmsProgMoveProgramTowardsEnd
        ' 
        cmsProgMoveProgramTowardsEnd.Name = "cmsProgMoveProgramTowardsEnd"
        cmsProgMoveProgramTowardsEnd.ShortcutKeys = Keys.Control Or Keys.Right
        cmsProgMoveProgramTowardsEnd.Size = New Size(305, 22)
        cmsProgMoveProgramTowardsEnd.Text = "Move Program Towards the End"
        ' 
        ' ToolStripSeparator4
        ' 
        ToolStripSeparator4.Name = "ToolStripSeparator4"
        ToolStripSeparator4.Size = New Size(302, 6)
        ' 
        ' cmsProgEditVarNames
        ' 
        cmsProgEditVarNames.Name = "cmsProgEditVarNames"
        cmsProgEditVarNames.Size = New Size(305, 22)
        cmsProgEditVarNames.Text = "Prog Variable Names..."
        ' 
        ' cmsProgEditTimerNames
        ' 
        cmsProgEditTimerNames.Name = "cmsProgEditTimerNames"
        cmsProgEditTimerNames.Size = New Size(305, 22)
        cmsProgEditTimerNames.Text = "Prog Timer Names..."
        ' 
        ' ToolStripSeparator13
        ' 
        ToolStripSeparator13.Name = "ToolStripSeparator13"
        ToolStripSeparator13.Size = New Size(302, 6)
        ' 
        ' cmsProgDeleteProgram
        ' 
        cmsProgDeleteProgram.Name = "cmsProgDeleteProgram"
        cmsProgDeleteProgram.Size = New Size(305, 22)
        cmsProgDeleteProgram.Text = "Delete Program"
        ' 
        ' cmdMoveDown
        ' 
        cmdMoveDown.Location = New Point(608, 25)
        cmdMoveDown.Name = "cmdMoveDown"
        cmdMoveDown.Size = New Size(89, 23)
        cmdMoveDown.TabIndex = 9
        cmdMoveDown.Text = "move down"
        cmdMoveDown.UseVisualStyleBackColor = True
        ' 
        ' cmdDuplicateProgLine
        ' 
        cmdDuplicateProgLine.Location = New Point(428, 24)
        cmdDuplicateProgLine.Name = "cmdDuplicateProgLine"
        cmdDuplicateProgLine.Size = New Size(93, 23)
        cmdDuplicateProgLine.TabIndex = 10
        cmdDuplicateProgLine.Text = "duplicate line"
        cmdDuplicateProgLine.UseVisualStyleBackColor = True
        ' 
        ' Button3
        ' 
        Button3.Location = New Point(16, 11)
        Button3.Name = "Button3"
        Button3.Size = New Size(75, 23)
        Button3.TabIndex = 11
        Button3.Text = "Enable"
        Button3.UseVisualStyleBackColor = True
        ' 
        ' lblProgName
        ' 
        lblProgName.Location = New Point(20, 14)
        lblProgName.Name = "lblProgName"
        lblProgName.Size = New Size(117, 15)
        lblProgName.TabIndex = 12
        lblProgName.Text = "No Program Loaded."
        lblProgName.TextAlign = ContentAlignment.MiddleRight
        ' 
        ' txtProgName
        ' 
        txtProgName.Location = New Point(143, 11)
        txtProgName.MaxLength = 10
        txtProgName.Name = "txtProgName"
        txtProgName.Size = New Size(100, 23)
        txtProgName.TabIndex = 13
        ' 
        ' cmdEditVarNames
        ' 
        cmdEditVarNames.Location = New Point(311, 10)
        cmdEditVarNames.Name = "cmdEditVarNames"
        cmdEditVarNames.Size = New Size(75, 23)
        cmdEditVarNames.TabIndex = 15
        cmdEditVarNames.Text = "Var Names"
        cmdEditVarNames.UseVisualStyleBackColor = True
        ' 
        ' tabBottom
        ' 
        tabBottom.Controls.Add(TabBottomProgram)
        tabBottom.Controls.Add(TabPage2)
        tabBottom.Dock = DockStyle.Fill
        tabBottom.Location = New Point(0, 0)
        tabBottom.Name = "tabBottom"
        tabBottom.SelectedIndex = 0
        tabBottom.Size = New Size(1310, 593)
        tabBottom.TabIndex = 16
        ' 
        ' TabBottomProgram
        ' 
        TabBottomProgram.Controls.Add(SplitContainer8)
        TabBottomProgram.Controls.Add(mnuTabProgram)
        TabBottomProgram.Location = New Point(4, 24)
        TabBottomProgram.Name = "TabBottomProgram"
        TabBottomProgram.Padding = New Padding(3)
        TabBottomProgram.Size = New Size(1302, 565)
        TabBottomProgram.TabIndex = 0
        TabBottomProgram.Text = "Program"
        TabBottomProgram.UseVisualStyleBackColor = True
        ' 
        ' SplitContainer8
        ' 
        SplitContainer8.Dock = DockStyle.Fill
        SplitContainer8.FixedPanel = FixedPanel.Panel1
        SplitContainer8.IsSplitterFixed = True
        SplitContainer8.Location = New Point(3, 27)
        SplitContainer8.Name = "SplitContainer8"
        SplitContainer8.Orientation = Orientation.Horizontal
        ' 
        ' SplitContainer8.Panel1
        ' 
        SplitContainer8.Panel1.Controls.Add(ToolStripContainer1)
        ' 
        ' SplitContainer8.Panel2
        ' 
        SplitContainer8.Panel2.Controls.Add(Panel1)
        SplitContainer8.Size = New Size(1296, 535)
        SplitContainer8.SplitterDistance = 35
        SplitContainer8.TabIndex = 21
        ' 
        ' ToolStripContainer1
        ' 
        ToolStripContainer1.BottomToolStripPanelVisible = False
        ' 
        ' ToolStripContainer1.ContentPanel
        ' 
        ToolStripContainer1.ContentPanel.Size = New Size(1296, 3)
        ToolStripContainer1.Dock = DockStyle.Fill
        ToolStripContainer1.LeftToolStripPanelVisible = False
        ToolStripContainer1.Location = New Point(0, 0)
        ToolStripContainer1.Name = "ToolStripContainer1"
        ToolStripContainer1.RightToolStripPanelVisible = False
        ToolStripContainer1.Size = New Size(1296, 35)
        ToolStripContainer1.TabIndex = 20
        ToolStripContainer1.Text = "ToolStripContainer1"
        ' 
        ' ToolStripContainer1.TopToolStripPanel
        ' 
        ToolStripContainer1.TopToolStripPanel.Controls.Add(ToolStrip2)
        ' 
        ' ToolStrip2
        ' 
        ToolStrip2.Dock = DockStyle.None
        ToolStrip2.ImageScalingSize = New Size(25, 25)
        ToolStrip2.Items.AddRange(New ToolStripItem() {tscmdNewProject, ToolStripSeparator14, tscmdOpenProjectFromFile, tscmdSaveProjectToFile, ToolStripSeparator15, tscmdOpenProgramFromDevice, tscmdSaveProgramToDevice, ToolStripSeparator16, ToolStripSeparator17, tsbtnDebugRun, tsbtnDebugPause, tsbtnDebugStep})
        ToolStrip2.LayoutStyle = ToolStripLayoutStyle.Flow
        ToolStrip2.Location = New Point(3, 0)
        ToolStrip2.Name = "ToolStrip2"
        ToolStrip2.Size = New Size(257, 32)
        ToolStrip2.TabIndex = 0
        ToolStrip2.Text = "Project"
        ' 
        ' tscmdNewProject
        ' 
        tscmdNewProject.DisplayStyle = ToolStripItemDisplayStyle.Image
        tscmdNewProject.Image = CType(resources.GetObject("tscmdNewProject.Image"), Image)
        tscmdNewProject.ImageTransparentColor = Color.Magenta
        tscmdNewProject.Name = "tscmdNewProject"
        tscmdNewProject.Size = New Size(29, 29)
        tscmdNewProject.Text = "ToolStripButton1"
        tscmdNewProject.ToolTipText = "Create New Project"
        ' 
        ' ToolStripSeparator14
        ' 
        ToolStripSeparator14.Name = "ToolStripSeparator14"
        ToolStripSeparator14.Size = New Size(6, 23)
        ' 
        ' tscmdOpenProjectFromFile
        ' 
        tscmdOpenProjectFromFile.DisplayStyle = ToolStripItemDisplayStyle.Image
        tscmdOpenProjectFromFile.Image = CType(resources.GetObject("tscmdOpenProjectFromFile.Image"), Image)
        tscmdOpenProjectFromFile.ImageTransparentColor = Color.Magenta
        tscmdOpenProjectFromFile.Name = "tscmdOpenProjectFromFile"
        tscmdOpenProjectFromFile.Size = New Size(29, 29)
        tscmdOpenProjectFromFile.Text = "ToolStripButton1"
        tscmdOpenProjectFromFile.ToolTipText = "Open Project From File"
        ' 
        ' tscmdSaveProjectToFile
        ' 
        tscmdSaveProjectToFile.DisplayStyle = ToolStripItemDisplayStyle.Image
        tscmdSaveProjectToFile.Image = CType(resources.GetObject("tscmdSaveProjectToFile.Image"), Image)
        tscmdSaveProjectToFile.ImageTransparentColor = Color.Magenta
        tscmdSaveProjectToFile.Name = "tscmdSaveProjectToFile"
        tscmdSaveProjectToFile.Size = New Size(29, 29)
        tscmdSaveProjectToFile.Text = "ToolStripButton1"
        tscmdSaveProjectToFile.ToolTipText = "Save Program To File"
        ' 
        ' ToolStripSeparator15
        ' 
        ToolStripSeparator15.Name = "ToolStripSeparator15"
        ToolStripSeparator15.Size = New Size(6, 23)
        ' 
        ' tscmdOpenProgramFromDevice
        ' 
        tscmdOpenProgramFromDevice.DisplayStyle = ToolStripItemDisplayStyle.Image
        tscmdOpenProgramFromDevice.Image = CType(resources.GetObject("tscmdOpenProgramFromDevice.Image"), Image)
        tscmdOpenProgramFromDevice.ImageTransparentColor = Color.Magenta
        tscmdOpenProgramFromDevice.Name = "tscmdOpenProgramFromDevice"
        tscmdOpenProgramFromDevice.Size = New Size(29, 29)
        tscmdOpenProgramFromDevice.Text = "ToolStripButton1"
        tscmdOpenProgramFromDevice.ToolTipText = "Download Project FROM Device"
        ' 
        ' tscmdSaveProgramToDevice
        ' 
        tscmdSaveProgramToDevice.DisplayStyle = ToolStripItemDisplayStyle.Image
        tscmdSaveProgramToDevice.Image = CType(resources.GetObject("tscmdSaveProgramToDevice.Image"), Image)
        tscmdSaveProgramToDevice.ImageTransparentColor = Color.Magenta
        tscmdSaveProgramToDevice.Name = "tscmdSaveProgramToDevice"
        tscmdSaveProgramToDevice.Size = New Size(29, 29)
        tscmdSaveProgramToDevice.Text = "ToolStripButton1"
        tscmdSaveProgramToDevice.ToolTipText = "Upload Project TO Device"
        ' 
        ' ToolStripSeparator16
        ' 
        ToolStripSeparator16.Name = "ToolStripSeparator16"
        ToolStripSeparator16.Size = New Size(6, 23)
        ' 
        ' ToolStripSeparator17
        ' 
        ToolStripSeparator17.Name = "ToolStripSeparator17"
        ToolStripSeparator17.Size = New Size(6, 23)
        ' 
        ' tsbtnDebugRun
        ' 
        tsbtnDebugRun.DisplayStyle = ToolStripItemDisplayStyle.Image
        tsbtnDebugRun.Enabled = False
        tsbtnDebugRun.Image = My.Resources.Resources.playGreen
        tsbtnDebugRun.ImageTransparentColor = Color.Magenta
        tsbtnDebugRun.Name = "tsbtnDebugRun"
        tsbtnDebugRun.Size = New Size(29, 29)
        tsbtnDebugRun.Text = "Run"
        tsbtnDebugRun.ToolTipText = "Run"
        ' 
        ' tsbtnDebugPause
        ' 
        tsbtnDebugPause.DisplayStyle = ToolStripItemDisplayStyle.Image
        tsbtnDebugPause.Enabled = False
        tsbtnDebugPause.Image = My.Resources.Resources.pauseOrange
        tsbtnDebugPause.ImageTransparentColor = Color.Magenta
        tsbtnDebugPause.Name = "tsbtnDebugPause"
        tsbtnDebugPause.Size = New Size(29, 29)
        tsbtnDebugPause.Text = "Pause"
        ' 
        ' tsbtnDebugStep
        ' 
        tsbtnDebugStep.DisplayStyle = ToolStripItemDisplayStyle.Image
        tsbtnDebugStep.Enabled = False
        tsbtnDebugStep.Image = My.Resources.Resources.skipBlack
        tsbtnDebugStep.ImageTransparentColor = Color.Magenta
        tsbtnDebugStep.Name = "tsbtnDebugStep"
        tsbtnDebugStep.Size = New Size(29, 29)
        tsbtnDebugStep.Text = "Step"
        tsbtnDebugStep.ToolTipText = "Single Step"
        ' 
        ' Panel1
        ' 
        Panel1.Controls.Add(SplitContainer1)
        Panel1.Controls.Add(pnlProgramList)
        Panel1.Dock = DockStyle.Fill
        Panel1.Location = New Point(0, 0)
        Panel1.Name = "Panel1"
        Panel1.Size = New Size(1296, 496)
        Panel1.TabIndex = 19
        ' 
        ' SplitContainer1
        ' 
        SplitContainer1.Dock = DockStyle.Fill
        SplitContainer1.FixedPanel = FixedPanel.Panel1
        SplitContainer1.Location = New Point(136, 0)
        SplitContainer1.Name = "SplitContainer1"
        SplitContainer1.Orientation = Orientation.Horizontal
        ' 
        ' SplitContainer1.Panel1
        ' 
        SplitContainer1.Panel1.Controls.Add(SplitContainer5)
        ' 
        ' SplitContainer1.Panel2
        ' 
        SplitContainer1.Panel2.Controls.Add(SplitContainer4)
        SplitContainer1.Size = New Size(1160, 496)
        SplitContainer1.TabIndex = 18
        ' 
        ' SplitContainer5
        ' 
        SplitContainer5.Dock = DockStyle.Fill
        SplitContainer5.FixedPanel = FixedPanel.Panel2
        SplitContainer5.IsSplitterFixed = True
        SplitContainer5.Location = New Point(0, 0)
        SplitContainer5.Name = "SplitContainer5"
        ' 
        ' SplitContainer5.Panel1
        ' 
        SplitContainer5.Panel1.Controls.Add(cmdEditVarNames)
        SplitContainer5.Panel1.Controls.Add(lblProgName)
        SplitContainer5.Panel1.Controls.Add(cmdMoveDown)
        SplitContainer5.Panel1.Controls.Add(cmdMoveRowUp)
        SplitContainer5.Panel1.Controls.Add(cmdDeleteRow)
        SplitContainer5.Panel1.Controls.Add(cmdAddRow)
        SplitContainer5.Panel1.Controls.Add(cmdDuplicateProgLine)
        SplitContainer5.Panel1.Controls.Add(txtProgName)
        SplitContainer5.Panel1.Controls.Add(cmdInsertRow)
        ' 
        ' SplitContainer5.Panel2
        ' 
        SplitContainer5.Panel2.Controls.Add(chkLiveTrackLines)
        SplitContainer5.Panel2.Controls.Add(Button3)
        SplitContainer5.Panel2MinSize = 410
        SplitContainer5.Size = New Size(1160, 50)
        SplitContainer5.SplitterDistance = 730
        SplitContainer5.TabIndex = 16
        ' 
        ' chkLiveTrackLines
        ' 
        chkLiveTrackLines.AutoSize = True
        chkLiveTrackLines.Location = New Point(135, 16)
        chkLiveTrackLines.Name = "chkLiveTrackLines"
        chkLiveTrackLines.Size = New Size(110, 19)
        chkLiveTrackLines.TabIndex = 12
        chkLiveTrackLines.Text = "Live Track Line#"
        chkLiveTrackLines.UseVisualStyleBackColor = True
        ' 
        ' SplitContainer4
        ' 
        SplitContainer4.Dock = DockStyle.Fill
        SplitContainer4.FixedPanel = FixedPanel.Panel2
        SplitContainer4.IsSplitterFixed = True
        SplitContainer4.Location = New Point(0, 0)
        SplitContainer4.Name = "SplitContainer4"
        ' 
        ' SplitContainer4.Panel1
        ' 
        SplitContainer4.Panel1.Controls.Add(lstProgDisplay)
        ' 
        ' SplitContainer4.Panel2
        ' 
        SplitContainer4.Panel2.Controls.Add(TabControl1)
        SplitContainer4.Panel2MinSize = 410
        SplitContainer4.Size = New Size(1160, 442)
        SplitContainer4.SplitterDistance = 730
        SplitContainer4.TabIndex = 4
        ' 
        ' TabControl1
        ' 
        TabControl1.Controls.Add(tabpageLineDetails)
        TabControl1.Controls.Add(tabpageVariables)
        TabControl1.Controls.Add(tabpageTimers)
        TabControl1.Dock = DockStyle.Fill
        TabControl1.Location = New Point(0, 0)
        TabControl1.Name = "TabControl1"
        TabControl1.SelectedIndex = 0
        TabControl1.Size = New Size(426, 442)
        TabControl1.TabIndex = 1
        ' 
        ' tabpageLineDetails
        ' 
        tabpageLineDetails.Controls.Add(UcProgLineEdit1)
        tabpageLineDetails.Location = New Point(4, 24)
        tabpageLineDetails.Name = "tabpageLineDetails"
        tabpageLineDetails.Padding = New Padding(3)
        tabpageLineDetails.Size = New Size(418, 414)
        tabpageLineDetails.TabIndex = 0
        tabpageLineDetails.Text = "Line Details"
        tabpageLineDetails.UseVisualStyleBackColor = True
        ' 
        ' UcProgLineEdit1
        ' 
        UcProgLineEdit1.Dock = DockStyle.Fill
        UcProgLineEdit1.Location = New Point(3, 3)
        UcProgLineEdit1.Margin = New Padding(6)
        UcProgLineEdit1.Name = "UcProgLineEdit1"
        UcProgLineEdit1.Size = New Size(412, 408)
        UcProgLineEdit1.TabIndex = 0
        ' 
        ' tabpageVariables
        ' 
        tabpageVariables.Controls.Add(SplitContainerVariables)
        tabpageVariables.Location = New Point(4, 24)
        tabpageVariables.Name = "tabpageVariables"
        tabpageVariables.Padding = New Padding(3)
        tabpageVariables.Size = New Size(418, 414)
        tabpageVariables.TabIndex = 1
        tabpageVariables.Text = "Variables"
        tabpageVariables.UseVisualStyleBackColor = True
        ' 
        ' SplitContainerVariables
        ' 
        SplitContainerVariables.Dock = DockStyle.Fill
        SplitContainerVariables.FixedPanel = FixedPanel.Panel1
        SplitContainerVariables.Location = New Point(3, 3)
        SplitContainerVariables.Name = "SplitContainerVariables"
        SplitContainerVariables.Orientation = Orientation.Horizontal
        ' 
        ' SplitContainerVariables.Panel1
        ' 
        SplitContainerVariables.Panel1.Controls.Add(cboVariablesChannel)
        ' 
        ' SplitContainerVariables.Panel2
        ' 
        SplitContainerVariables.Panel2.Controls.Add(dgvVariables)
        SplitContainerVariables.Size = New Size(412, 408)
        SplitContainerVariables.SplitterDistance = 36
        SplitContainerVariables.TabIndex = 0
        ' 
        ' cboVariablesChannel
        ' 
        cboVariablesChannel.FormattingEnabled = True
        cboVariablesChannel.Location = New Point(9, 10)
        cboVariablesChannel.Name = "cboVariablesChannel"
        cboVariablesChannel.Size = New Size(164, 23)
        cboVariablesChannel.TabIndex = 0
        ' 
        ' dgvVariables
        ' 
        dgvVariables.AllowUserToAddRows = False
        dgvVariables.AllowUserToDeleteRows = False
        dgvVariables.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        dgvVariables.Dock = DockStyle.Fill
        dgvVariables.Location = New Point(0, 0)
        dgvVariables.Name = "dgvVariables"
        dgvVariables.ReadOnly = True
        dgvVariables.RowHeadersVisible = False
        dgvVariables.Size = New Size(412, 368)
        dgvVariables.TabIndex = 0
        ' 
        ' tabpageTimers
        ' 
        tabpageTimers.Controls.Add(SplitContainerTimers)
        tabpageTimers.Location = New Point(4, 24)
        tabpageTimers.Name = "tabpageTimers"
        tabpageTimers.Padding = New Padding(3)
        tabpageTimers.Size = New Size(418, 414)
        tabpageTimers.TabIndex = 2
        tabpageTimers.Text = "Timers"
        tabpageTimers.UseVisualStyleBackColor = True
        ' 
        ' SplitContainerTimers
        ' 
        SplitContainerTimers.Dock = DockStyle.Fill
        SplitContainerTimers.FixedPanel = FixedPanel.Panel1
        SplitContainerTimers.Location = New Point(3, 3)
        SplitContainerTimers.Name = "SplitContainerTimers"
        SplitContainerTimers.Orientation = Orientation.Horizontal
        ' 
        ' SplitContainerTimers.Panel1
        ' 
        SplitContainerTimers.Panel1.Controls.Add(cboTimersChannel)
        ' 
        ' SplitContainerTimers.Panel2
        ' 
        SplitContainerTimers.Panel2.Controls.Add(dgvTimers)
        SplitContainerTimers.Size = New Size(412, 408)
        SplitContainerTimers.SplitterDistance = 36
        SplitContainerTimers.TabIndex = 1
        ' 
        ' cboTimersChannel
        ' 
        cboTimersChannel.FormattingEnabled = True
        cboTimersChannel.Location = New Point(9, 10)
        cboTimersChannel.Name = "cboTimersChannel"
        cboTimersChannel.Size = New Size(164, 23)
        cboTimersChannel.TabIndex = 0
        ' 
        ' dgvTimers
        ' 
        dgvTimers.AllowUserToAddRows = False
        dgvTimers.AllowUserToDeleteRows = False
        dgvTimers.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        dgvTimers.Dock = DockStyle.Fill
        dgvTimers.Location = New Point(0, 0)
        dgvTimers.Name = "dgvTimers"
        dgvTimers.ReadOnly = True
        dgvTimers.RowHeadersVisible = False
        dgvTimers.Size = New Size(412, 368)
        dgvTimers.TabIndex = 1
        ' 
        ' pnlProgramList
        ' 
        pnlProgramList.Controls.Add(SplitContainer2)
        pnlProgramList.Dock = DockStyle.Left
        pnlProgramList.Location = New Point(0, 0)
        pnlProgramList.MinimumSize = New Size(0, 240)
        pnlProgramList.Name = "pnlProgramList"
        pnlProgramList.Size = New Size(136, 496)
        pnlProgramList.TabIndex = 16
        ' 
        ' SplitContainer2
        ' 
        SplitContainer2.Dock = DockStyle.Fill
        SplitContainer2.FixedPanel = FixedPanel.Panel1
        SplitContainer2.IsSplitterFixed = True
        SplitContainer2.Location = New Point(0, 0)
        SplitContainer2.Name = "SplitContainer2"
        SplitContainer2.Orientation = Orientation.Horizontal
        ' 
        ' SplitContainer2.Panel1
        ' 
        SplitContainer2.Panel1.Controls.Add(txtDeviceName)
        SplitContainer2.Panel1.Controls.Add(Label4)
        SplitContainer2.Panel1.Controls.Add(lblDeviceType)
        SplitContainer2.Panel1.Controls.Add(Label2)
        SplitContainer2.Panel1.Controls.Add(Label1)
        SplitContainer2.Panel1.Controls.Add(txtProjectName)
        ' 
        ' SplitContainer2.Panel2
        ' 
        SplitContainer2.Panel2.Controls.Add(lstPrograms)
        SplitContainer2.Panel2MinSize = 65
        SplitContainer2.Size = New Size(136, 496)
        SplitContainer2.SplitterDistance = 152
        SplitContainer2.TabIndex = 21
        ' 
        ' txtDeviceName
        ' 
        txtDeviceName.Location = New Point(16, 57)
        txtDeviceName.MaxLength = 20
        txtDeviceName.Name = "txtDeviceName"
        txtDeviceName.Size = New Size(105, 23)
        txtDeviceName.TabIndex = 24
        ' 
        ' Label4
        ' 
        Label4.Location = New Point(3, 40)
        Label4.Name = "Label4"
        Label4.Size = New Size(129, 15)
        Label4.TabIndex = 23
        Label4.Text = "Device Name:"
        Label4.TextAlign = ContentAlignment.MiddleLeft
        ' 
        ' lblDeviceType
        ' 
        lblDeviceType.Location = New Point(16, 20)
        lblDeviceType.Name = "lblDeviceType"
        lblDeviceType.Size = New Size(113, 15)
        lblDeviceType.TabIndex = 22
        lblDeviceType.Text = "lblDeviceType"
        lblDeviceType.TextAlign = ContentAlignment.MiddleLeft
        ' 
        ' Label2
        ' 
        Label2.Location = New Point(5, 5)
        Label2.Name = "Label2"
        Label2.Size = New Size(129, 15)
        Label2.TabIndex = 21
        Label2.Text = "Device Type:"
        Label2.TextAlign = ContentAlignment.MiddleLeft
        ' 
        ' Label1
        ' 
        Label1.Location = New Point(3, 80)
        Label1.Name = "Label1"
        Label1.Size = New Size(103, 15)
        Label1.TabIndex = 19
        Label1.Text = "Project Name:"
        Label1.TextAlign = ContentAlignment.MiddleLeft
        ' 
        ' txtProjectName
        ' 
        txtProjectName.Location = New Point(18, 98)
        txtProjectName.MaxLength = 20
        txtProjectName.Name = "txtProjectName"
        txtProjectName.Size = New Size(105, 23)
        txtProjectName.TabIndex = 20
        txtProjectName.Text = "OneTwoThreeWRGUWQZMH"
        ' 
        ' mnuTabProgram
        ' 
        mnuTabProgram.ImageScalingSize = New Size(32, 32)
        mnuTabProgram.Items.AddRange(New ToolStripItem() {mnuMainProject, mnuMainPrograms, mnuProgLines, ToolStripMenuItem2})
        mnuTabProgram.Location = New Point(3, 3)
        mnuTabProgram.Name = "mnuTabProgram"
        mnuTabProgram.Size = New Size(1296, 24)
        mnuTabProgram.TabIndex = 18
        mnuTabProgram.Text = "MenuStrip1"
        ' 
        ' mnuMainProject
        ' 
        mnuMainProject.DropDownItems.AddRange(New ToolStripItem() {CreateNewProjectToolStripMenuItem, tsmDeviceType, ToolStripSeparator11, OpenProjectFromFileToolStripMenuItem, SaveProjectToolStripMenuItem, SaveProjectAsToolStripMenuItem, ToolStripSeparator12, OpenProjectFromDeviceToolStripMenuItem, DownloadProjectToDeviceToolStripMenuItem})
        mnuMainProject.Name = "mnuMainProject"
        mnuMainProject.Size = New Size(56, 20)
        mnuMainProject.Text = "Project"
        ' 
        ' CreateNewProjectToolStripMenuItem
        ' 
        CreateNewProjectToolStripMenuItem.Name = "CreateNewProjectToolStripMenuItem"
        CreateNewProjectToolStripMenuItem.Size = New Size(222, 22)
        CreateNewProjectToolStripMenuItem.Text = "Create New Project"
        ' 
        ' tsmDeviceType
        ' 
        tsmDeviceType.Name = "tsmDeviceType"
        tsmDeviceType.Size = New Size(222, 22)
        tsmDeviceType.Text = "Default Device Type..."
        ' 
        ' ToolStripSeparator11
        ' 
        ToolStripSeparator11.Name = "ToolStripSeparator11"
        ToolStripSeparator11.Size = New Size(219, 6)
        ' 
        ' OpenProjectFromFileToolStripMenuItem
        ' 
        OpenProjectFromFileToolStripMenuItem.Name = "OpenProjectFromFileToolStripMenuItem"
        OpenProjectFromFileToolStripMenuItem.Size = New Size(222, 22)
        OpenProjectFromFileToolStripMenuItem.Text = "Open Project From File"
        ' 
        ' SaveProjectToolStripMenuItem
        ' 
        SaveProjectToolStripMenuItem.Name = "SaveProjectToolStripMenuItem"
        SaveProjectToolStripMenuItem.Size = New Size(222, 22)
        SaveProjectToolStripMenuItem.Text = "Save Project"
        ' 
        ' SaveProjectAsToolStripMenuItem
        ' 
        SaveProjectAsToolStripMenuItem.Name = "SaveProjectAsToolStripMenuItem"
        SaveProjectAsToolStripMenuItem.Size = New Size(222, 22)
        SaveProjectAsToolStripMenuItem.Text = "Save Project As..."
        ' 
        ' ToolStripSeparator12
        ' 
        ToolStripSeparator12.Name = "ToolStripSeparator12"
        ToolStripSeparator12.Size = New Size(219, 6)
        ' 
        ' OpenProjectFromDeviceToolStripMenuItem
        ' 
        OpenProjectFromDeviceToolStripMenuItem.Name = "OpenProjectFromDeviceToolStripMenuItem"
        OpenProjectFromDeviceToolStripMenuItem.Size = New Size(222, 22)
        OpenProjectFromDeviceToolStripMenuItem.Text = "Get Project From Device"
        ' 
        ' DownloadProjectToDeviceToolStripMenuItem
        ' 
        DownloadProjectToDeviceToolStripMenuItem.Name = "DownloadProjectToDeviceToolStripMenuItem"
        DownloadProjectToDeviceToolStripMenuItem.Size = New Size(222, 22)
        DownloadProjectToDeviceToolStripMenuItem.Text = "Download Project To Device"
        ' 
        ' mnuMainPrograms
        ' 
        mnuMainPrograms.DropDownItems.AddRange(New ToolStripItem() {AddNewBlankProgramToolStripMenuItem, ImportExistingProgramToolStripMenuItem, mnuDuplicateProgram, ToolStripSeparator5, MoveProgramTowardsBeginningToolStripMenuItem, MoveProgramTowardsEndToolStripMenuItem, ToolStripSeparator6, DeleteProgramToolStripMenuItem})
        mnuMainPrograms.Name = "mnuMainPrograms"
        mnuMainPrograms.Size = New Size(70, 20)
        mnuMainPrograms.Text = "Programs"
        ' 
        ' AddNewBlankProgramToolStripMenuItem
        ' 
        AddNewBlankProgramToolStripMenuItem.DropDownItems.AddRange(New ToolStripItem() {mnuAddBlankProgramAtIndex, mnuAddBlankProgramAtEnd})
        AddNewBlankProgramToolStripMenuItem.Name = "AddNewBlankProgramToolStripMenuItem"
        AddNewBlankProgramToolStripMenuItem.Size = New Size(257, 22)
        AddNewBlankProgramToolStripMenuItem.Text = "Add New (Blank) Program To End"
        ' 
        ' mnuAddBlankProgramAtIndex
        ' 
        mnuAddBlankProgramAtIndex.Name = "mnuAddBlankProgramAtIndex"
        mnuAddBlankProgramAtIndex.Size = New Size(210, 22)
        mnuAddBlankProgramAtIndex.Text = "Insert At Current Location"
        ' 
        ' mnuAddBlankProgramAtEnd
        ' 
        mnuAddBlankProgramAtEnd.Name = "mnuAddBlankProgramAtEnd"
        mnuAddBlankProgramAtEnd.Size = New Size(210, 22)
        mnuAddBlankProgramAtEnd.Text = "Add to End"
        ' 
        ' ImportExistingProgramToolStripMenuItem
        ' 
        ImportExistingProgramToolStripMenuItem.Name = "ImportExistingProgramToolStripMenuItem"
        ImportExistingProgramToolStripMenuItem.Size = New Size(257, 22)
        ImportExistingProgramToolStripMenuItem.Text = "Import Existing Program"
        ' 
        ' mnuDuplicateProgram
        ' 
        mnuDuplicateProgram.Name = "mnuDuplicateProgram"
        mnuDuplicateProgram.Size = New Size(257, 22)
        mnuDuplicateProgram.Text = "Duplicate Program"
        ' 
        ' ToolStripSeparator5
        ' 
        ToolStripSeparator5.Name = "ToolStripSeparator5"
        ToolStripSeparator5.Size = New Size(254, 6)
        ' 
        ' MoveProgramTowardsBeginningToolStripMenuItem
        ' 
        MoveProgramTowardsBeginningToolStripMenuItem.Name = "MoveProgramTowardsBeginningToolStripMenuItem"
        MoveProgramTowardsBeginningToolStripMenuItem.Size = New Size(257, 22)
        MoveProgramTowardsBeginningToolStripMenuItem.Text = "Move Program Towards Beginning"
        ' 
        ' MoveProgramTowardsEndToolStripMenuItem
        ' 
        MoveProgramTowardsEndToolStripMenuItem.Name = "MoveProgramTowardsEndToolStripMenuItem"
        MoveProgramTowardsEndToolStripMenuItem.Size = New Size(257, 22)
        MoveProgramTowardsEndToolStripMenuItem.Text = "Move Program Towards End"
        ' 
        ' ToolStripSeparator6
        ' 
        ToolStripSeparator6.Name = "ToolStripSeparator6"
        ToolStripSeparator6.Size = New Size(254, 6)
        ' 
        ' DeleteProgramToolStripMenuItem
        ' 
        DeleteProgramToolStripMenuItem.Name = "DeleteProgramToolStripMenuItem"
        DeleteProgramToolStripMenuItem.Size = New Size(257, 22)
        DeleteProgramToolStripMenuItem.Text = "Delete Program"
        ' 
        ' mnuProgLines
        ' 
        mnuProgLines.DropDownItems.AddRange(New ToolStripItem() {mnuAddProgLine, mnuInsertProgLine, mnuDuplicateProgLine, ToolStripSeparator7, mnuMoveProgLineUp, mnuMoveProgramLineDown, ToolStripSeparator8, mnuDeleteProgramLine})
        mnuProgLines.Name = "mnuProgLines"
        mnuProgLines.Size = New Size(95, 20)
        mnuProgLines.Text = "Program Lines"
        ' 
        ' mnuAddProgLine
        ' 
        mnuAddProgLine.Name = "mnuAddProgLine"
        mnuAddProgLine.Size = New Size(212, 22)
        mnuAddProgLine.Text = "Add Blank Program Line"
        ' 
        ' mnuInsertProgLine
        ' 
        mnuInsertProgLine.Name = "mnuInsertProgLine"
        mnuInsertProgLine.Size = New Size(212, 22)
        mnuInsertProgLine.Text = "Insert Blank Program Line"
        ' 
        ' mnuDuplicateProgLine
        ' 
        mnuDuplicateProgLine.Name = "mnuDuplicateProgLine"
        mnuDuplicateProgLine.Size = New Size(212, 22)
        mnuDuplicateProgLine.Text = "Duplicate Program Line"
        ' 
        ' ToolStripSeparator7
        ' 
        ToolStripSeparator7.Name = "ToolStripSeparator7"
        ToolStripSeparator7.Size = New Size(209, 6)
        ' 
        ' mnuMoveProgLineUp
        ' 
        mnuMoveProgLineUp.Name = "mnuMoveProgLineUp"
        mnuMoveProgLineUp.Size = New Size(212, 22)
        mnuMoveProgLineUp.Text = "Move Program Line Up"
        ' 
        ' mnuMoveProgramLineDown
        ' 
        mnuMoveProgramLineDown.Name = "mnuMoveProgramLineDown"
        mnuMoveProgramLineDown.Size = New Size(212, 22)
        mnuMoveProgramLineDown.Text = "Move Program Line Down"
        ' 
        ' ToolStripSeparator8
        ' 
        ToolStripSeparator8.Name = "ToolStripSeparator8"
        ToolStripSeparator8.Size = New Size(209, 6)
        ' 
        ' mnuDeleteProgramLine
        ' 
        mnuDeleteProgramLine.Name = "mnuDeleteProgramLine"
        mnuDeleteProgramLine.Size = New Size(212, 22)
        mnuDeleteProgramLine.Text = "Delete Program Line"
        ' 
        ' ToolStripMenuItem2
        ' 
        ToolStripMenuItem2.Name = "ToolStripMenuItem2"
        ToolStripMenuItem2.Size = New Size(127, 20)
        ToolStripMenuItem2.Text = "ToolStripMenuItem2"
        ' 
        ' TabPage2
        ' 
        TabPage2.Location = New Point(4, 24)
        TabPage2.Name = "TabPage2"
        TabPage2.Padding = New Padding(3)
        TabPage2.Size = New Size(1302, 565)
        TabPage2.TabIndex = 1
        TabPage2.Text = "TabPage2"
        TabPage2.UseVisualStyleBackColor = True
        ' 
        ' SplitContainer3
        ' 
        SplitContainer3.BorderStyle = BorderStyle.Fixed3D
        SplitContainer3.Dock = DockStyle.Fill
        SplitContainer3.FixedPanel = FixedPanel.Panel1
        SplitContainer3.Location = New Point(0, 0)
        SplitContainer3.Name = "SplitContainer3"
        SplitContainer3.Orientation = Orientation.Horizontal
        ' 
        ' SplitContainer3.Panel1
        ' 
        SplitContainer3.Panel1.Controls.Add(SplitContainer6)
        ' 
        ' SplitContainer3.Panel2
        ' 
        SplitContainer3.Panel2.Controls.Add(tabBottom)
        SplitContainer3.Size = New Size(1314, 912)
        SplitContainer3.SplitterDistance = 311
        SplitContainer3.TabIndex = 17
        ' 
        ' SplitContainer6
        ' 
        SplitContainer6.Dock = DockStyle.Fill
        SplitContainer6.FixedPanel = FixedPanel.Panel1
        SplitContainer6.Location = New Point(0, 0)
        SplitContainer6.Name = "SplitContainer6"
        SplitContainer6.Orientation = Orientation.Horizontal
        ' 
        ' SplitContainer6.Panel1
        ' 
        SplitContainer6.Panel1.Controls.Add(SplitContainer7)
        ' 
        ' SplitContainer6.Panel2
        ' 
        SplitContainer6.Panel2.AutoScroll = True
        SplitContainer6.Panel2.Controls.Add(UcChanControl1)
        SplitContainer6.Size = New Size(1310, 307)
        SplitContainer6.SplitterDistance = 25
        SplitContainer6.TabIndex = 11
        ' 
        ' SplitContainer7
        ' 
        SplitContainer7.Dock = DockStyle.Fill
        SplitContainer7.FixedPanel = FixedPanel.Panel1
        SplitContainer7.Location = New Point(0, 0)
        SplitContainer7.Name = "SplitContainer7"
        ' 
        ' SplitContainer7.Panel1
        ' 
        SplitContainer7.Panel1.Controls.Add(ToolStrip1)
        ' 
        ' SplitContainer7.Panel2
        ' 
        SplitContainer7.Panel2.Controls.Add(cmdStop)
        SplitContainer7.Size = New Size(1310, 25)
        SplitContainer7.SplitterDistance = 721
        SplitContainer7.TabIndex = 3
        ' 
        ' ToolStrip1
        ' 
        ToolStrip1.Dock = DockStyle.Fill
        ToolStrip1.ImageScalingSize = New Size(32, 32)
        ToolStrip1.Items.AddRange(New ToolStripItem() {cmdResets, cboComPorts, cmdRefreshComportList, cmdConnect, ToolStripSeparator10, ToolStripLabelAutoStatus, cmdtsChanMonAutoStatusOff, cmdtsChanMonAutoStatusOn, ToolStripSeparator9, lblDevStatus})
        ToolStrip1.Location = New Point(0, 0)
        ToolStrip1.Name = "ToolStrip1"
        ToolStrip1.Padding = New Padding(0, 0, 2, 0)
        ToolStrip1.Size = New Size(721, 25)
        ToolStrip1.TabIndex = 4
        ToolStrip1.Text = "ToolStrip1"
        ' 
        ' cmdResets
        ' 
        cmdResets.AutoSize = False
        cmdResets.DisplayStyle = ToolStripItemDisplayStyle.Image
        cmdResets.DropDownItems.AddRange(New ToolStripItem() {cmdResetDevice, cmdEraseDevice})
        cmdResets.Image = CType(resources.GetObject("cmdResets.Image"), Image)
        cmdResets.ImageTransparentColor = Color.Magenta
        cmdResets.Name = "cmdResets"
        cmdResets.Size = New Size(40, 22)
        cmdResets.Text = "ToolStripDropDownButton1"
        ' 
        ' cmdResetDevice
        ' 
        cmdResetDevice.Name = "cmdResetDevice"
        cmdResetDevice.Size = New Size(102, 22)
        cmdResetDevice.Text = "Reset"
        cmdResetDevice.ToolTipText = "Reset Device"
        ' 
        ' cmdEraseDevice
        ' 
        cmdEraseDevice.Name = "cmdEraseDevice"
        cmdEraseDevice.Size = New Size(102, 22)
        cmdEraseDevice.Text = "Erase"
        ' 
        ' cboComPorts
        ' 
        cboComPorts.Name = "cboComPorts"
        cboComPorts.Size = New Size(121, 25)
        ' 
        ' cmdRefreshComportList
        ' 
        cmdRefreshComportList.DisplayStyle = ToolStripItemDisplayStyle.Text
        cmdRefreshComportList.Image = CType(resources.GetObject("cmdRefreshComportList.Image"), Image)
        cmdRefreshComportList.ImageTransparentColor = Color.Magenta
        cmdRefreshComportList.Name = "cmdRefreshComportList"
        cmdRefreshComportList.Size = New Size(50, 22)
        cmdRefreshComportList.Text = "Refresh"
        ' 
        ' cmdConnect
        ' 
        cmdConnect.DisplayStyle = ToolStripItemDisplayStyle.Text
        cmdConnect.Image = CType(resources.GetObject("cmdConnect.Image"), Image)
        cmdConnect.ImageTransparentColor = Color.Magenta
        cmdConnect.Name = "cmdConnect"
        cmdConnect.Size = New Size(56, 22)
        cmdConnect.Text = "Connect"
        ' 
        ' ToolStripSeparator10
        ' 
        ToolStripSeparator10.Name = "ToolStripSeparator10"
        ToolStripSeparator10.Size = New Size(6, 25)
        ' 
        ' ToolStripLabelAutoStatus
        ' 
        ToolStripLabelAutoStatus.Name = "ToolStripLabelAutoStatus"
        ToolStripLabelAutoStatus.Size = New Size(68, 22)
        ToolStripLabelAutoStatus.Text = "AutoStatus:"
        ' 
        ' cmdtsChanMonAutoStatusOff
        ' 
        cmdtsChanMonAutoStatusOff.DisplayStyle = ToolStripItemDisplayStyle.Text
        cmdtsChanMonAutoStatusOff.Image = CType(resources.GetObject("cmdtsChanMonAutoStatusOff.Image"), Image)
        cmdtsChanMonAutoStatusOff.ImageTransparentColor = Color.Magenta
        cmdtsChanMonAutoStatusOff.Name = "cmdtsChanMonAutoStatusOff"
        cmdtsChanMonAutoStatusOff.Size = New Size(28, 22)
        cmdtsChanMonAutoStatusOff.Text = "Off"
        ' 
        ' cmdtsChanMonAutoStatusOn
        ' 
        cmdtsChanMonAutoStatusOn.DisplayStyle = ToolStripItemDisplayStyle.Text
        cmdtsChanMonAutoStatusOn.Image = CType(resources.GetObject("cmdtsChanMonAutoStatusOn.Image"), Image)
        cmdtsChanMonAutoStatusOn.ImageTransparentColor = Color.Magenta
        cmdtsChanMonAutoStatusOn.Name = "cmdtsChanMonAutoStatusOn"
        cmdtsChanMonAutoStatusOn.Size = New Size(27, 22)
        cmdtsChanMonAutoStatusOn.Text = "On"
        ' 
        ' ToolStripSeparator9
        ' 
        ToolStripSeparator9.Name = "ToolStripSeparator9"
        ToolStripSeparator9.Size = New Size(6, 25)
        ' 
        ' lblDevStatus
        ' 
        lblDevStatus.Name = "lblDevStatus"
        lblDevStatus.Size = New Size(88, 22)
        lblDevStatus.Text = "ToolStripLabel1"
        ' 
        ' cmdStop
        ' 
        cmdStop.Dock = DockStyle.Fill
        cmdStop.Location = New Point(0, 0)
        cmdStop.Name = "cmdStop"
        cmdStop.Size = New Size(585, 25)
        cmdStop.TabIndex = 0
        cmdStop.Text = "STOP"
        cmdStop.UseVisualStyleBackColor = True
        ' 
        ' UcChanControl1
        ' 
        UcChanControl1.BorderStyle = BorderStyle.FixedSingle
        UcChanControl1.ChanEnabled = False
        UcChanControl1.ChanIndex = -1
        UcChanControl1.ChanName = Nothing
        UcChanControl1.ChanType = 0
        UcChanControl1.Duration = -1
        UcChanControl1.LineNum = -1
        UcChanControl1.Location = New Point(5, 5)
        UcChanControl1.MaxOutputPulsewidthPct = 10
        UcChanControl1.MaxSpeed = 0
        UcChanControl1.MinSpeed = 0
        UcChanControl1.Name = "UcChanControl1"
        UcChanControl1.OutputIntensityMax = 0
        UcChanControl1.OutputIntensityMin = 0
        UcChanControl1.OutputIntensityPct = 0
        UcChanControl1.PercentComplete = -1
        UcChanControl1.Polarity = -1
        UcChanControl1.PolaritySwapped = False
        UcChanControl1.ProgNum = -1
        UcChanControl1.ProgState = -1
        UcChanControl1.PulseWidth = 0
        UcChanControl1.PulseWidthEnd = -1
        UcChanControl1.PulseWidthStart = -1
        UcChanControl1.RepeatsRemaining = -1
        UcChanControl1.Size = New Size(177, 270)
        UcChanControl1.Speed = 0
        UcChanControl1.TabIndex = 1
        UcChanControl1.TabStop = False
        UcChanControl1.Visible = False
        ' 
        ' cmdAutostatus
        ' 
        cmdAutostatus.DisplayStyle = ToolStripItemDisplayStyle.Image
        cmdAutostatus.Image = CType(resources.GetObject("cmdAutostatus.Image"), Image)
        cmdAutostatus.ImageTransparentColor = Color.Magenta
        cmdAutostatus.Name = "cmdAutostatus"
        cmdAutostatus.Size = New Size(29, 22)
        cmdAutostatus.Text = "ToolStripDropDownButton1"
        ' 
        ' ofdProject
        ' 
        ofdProject.DefaultExt = "tp"
        ofdProject.Filter = "Tens Project Files|*.tp|All Files|*.*"
        ' 
        ' sfdProject
        ' 
        sfdProject.DefaultExt = "tp"
        sfdProject.Filter = "Tens Project Files|*.tp|All Files|*.*"
        ' 
        ' tmrNewMessages
        ' 
        tmrNewMessages.Enabled = True
        tmrNewMessages.Interval = 1
        ' 
        ' StatusStrip1
        ' 
        StatusStrip1.Dock = DockStyle.Fill
        StatusStrip1.ImageScalingSize = New Size(32, 32)
        StatusStrip1.Items.AddRange(New ToolStripItem() {ssmProgressBar, ssmStatusText})
        StatusStrip1.Location = New Point(0, 0)
        StatusStrip1.Name = "StatusStrip1"
        StatusStrip1.Size = New Size(1314, 25)
        StatusStrip1.TabIndex = 18
        StatusStrip1.Text = "statusStripMain"
        ' 
        ' ssmProgressBar
        ' 
        ssmProgressBar.Name = "ssmProgressBar"
        ssmProgressBar.Size = New Size(100, 19)
        ' 
        ' ssmStatusText
        ' 
        ssmStatusText.Name = "ssmStatusText"
        ssmStatusText.Size = New Size(81, 20)
        ssmStatusText.Text = "ssmStatusText"
        ' 
        ' bwDLProgFromDevice
        ' 
        bwDLProgFromDevice.WorkerReportsProgress = True
        ' 
        ' bwULProgToDevice
        ' 
        bwULProgToDevice.WorkerReportsProgress = True
        bwULProgToDevice.WorkerSupportsCancellation = True
        ' 
        ' tmrClearStatusbar
        ' 
        tmrClearStatusbar.Interval = 3000
        ' 
        ' tmrCheckComportStatus
        ' 
        ' 
        ' tmrCheckAutostatus
        ' 
        tmrCheckAutostatus.Interval = 1000
        ' 
        ' SplitContainer9
        ' 
        SplitContainer9.Dock = DockStyle.Fill
        SplitContainer9.FixedPanel = FixedPanel.Panel2
        SplitContainer9.IsSplitterFixed = True
        SplitContainer9.Location = New Point(0, 0)
        SplitContainer9.Name = "SplitContainer9"
        SplitContainer9.Orientation = Orientation.Horizontal
        ' 
        ' SplitContainer9.Panel1
        ' 
        SplitContainer9.Panel1.Controls.Add(SplitContainer3)
        ' 
        ' SplitContainer9.Panel2
        ' 
        SplitContainer9.Panel2.Controls.Add(StatusStrip1)
        SplitContainer9.Size = New Size(1314, 941)
        SplitContainer9.SplitterDistance = 912
        SplitContainer9.TabIndex = 19
        ' 
        ' Form1
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(1314, 941)
        Controls.Add(SplitContainer9)
        MinimumSize = New Size(840, 507)
        Name = "Form1"
        StartPosition = FormStartPosition.CenterScreen
        Text = "Tens2503"
        cmsProgSentences.ResumeLayout(False)
        cmsPrograms.ResumeLayout(False)
        tabBottom.ResumeLayout(False)
        TabBottomProgram.ResumeLayout(False)
        TabBottomProgram.PerformLayout()
        SplitContainer8.Panel1.ResumeLayout(False)
        SplitContainer8.Panel2.ResumeLayout(False)
        CType(SplitContainer8, ComponentModel.ISupportInitialize).EndInit()
        SplitContainer8.ResumeLayout(False)
        ToolStripContainer1.TopToolStripPanel.ResumeLayout(False)
        ToolStripContainer1.TopToolStripPanel.PerformLayout()
        ToolStripContainer1.ResumeLayout(False)
        ToolStripContainer1.PerformLayout()
        ToolStrip2.ResumeLayout(False)
        ToolStrip2.PerformLayout()
        Panel1.ResumeLayout(False)
        SplitContainer1.Panel1.ResumeLayout(False)
        SplitContainer1.Panel2.ResumeLayout(False)
        CType(SplitContainer1, ComponentModel.ISupportInitialize).EndInit()
        SplitContainer1.ResumeLayout(False)
        SplitContainer5.Panel1.ResumeLayout(False)
        SplitContainer5.Panel1.PerformLayout()
        SplitContainer5.Panel2.ResumeLayout(False)
        SplitContainer5.Panel2.PerformLayout()
        CType(SplitContainer5, ComponentModel.ISupportInitialize).EndInit()
        SplitContainer5.ResumeLayout(False)
        SplitContainer4.Panel1.ResumeLayout(False)
        SplitContainer4.Panel2.ResumeLayout(False)
        CType(SplitContainer4, ComponentModel.ISupportInitialize).EndInit()
        SplitContainer4.ResumeLayout(False)
        TabControl1.ResumeLayout(False)
        tabpageLineDetails.ResumeLayout(False)
        tabpageVariables.ResumeLayout(False)
        SplitContainerVariables.Panel1.ResumeLayout(False)
        SplitContainerVariables.Panel2.ResumeLayout(False)
        CType(SplitContainerVariables, ComponentModel.ISupportInitialize).EndInit()
        SplitContainerVariables.ResumeLayout(False)
        CType(dgvVariables, ComponentModel.ISupportInitialize).EndInit()
        tabpageTimers.ResumeLayout(False)
        SplitContainerTimers.Panel1.ResumeLayout(False)
        SplitContainerTimers.Panel2.ResumeLayout(False)
        CType(SplitContainerTimers, ComponentModel.ISupportInitialize).EndInit()
        SplitContainerTimers.ResumeLayout(False)
        CType(dgvTimers, ComponentModel.ISupportInitialize).EndInit()
        pnlProgramList.ResumeLayout(False)
        SplitContainer2.Panel1.ResumeLayout(False)
        SplitContainer2.Panel1.PerformLayout()
        SplitContainer2.Panel2.ResumeLayout(False)
        CType(SplitContainer2, ComponentModel.ISupportInitialize).EndInit()
        SplitContainer2.ResumeLayout(False)
        mnuTabProgram.ResumeLayout(False)
        mnuTabProgram.PerformLayout()
        SplitContainer3.Panel1.ResumeLayout(False)
        SplitContainer3.Panel2.ResumeLayout(False)
        CType(SplitContainer3, ComponentModel.ISupportInitialize).EndInit()
        SplitContainer3.ResumeLayout(False)
        SplitContainer6.Panel1.ResumeLayout(False)
        SplitContainer6.Panel2.ResumeLayout(False)
        CType(SplitContainer6, ComponentModel.ISupportInitialize).EndInit()
        SplitContainer6.ResumeLayout(False)
        SplitContainer7.Panel1.ResumeLayout(False)
        SplitContainer7.Panel1.PerformLayout()
        SplitContainer7.Panel2.ResumeLayout(False)
        CType(SplitContainer7, ComponentModel.ISupportInitialize).EndInit()
        SplitContainer7.ResumeLayout(False)
        ToolStrip1.ResumeLayout(False)
        ToolStrip1.PerformLayout()
        StatusStrip1.ResumeLayout(False)
        StatusStrip1.PerformLayout()
        SplitContainer9.Panel1.ResumeLayout(False)
        SplitContainer9.Panel2.ResumeLayout(False)
        SplitContainer9.Panel2.PerformLayout()
        CType(SplitContainer9, ComponentModel.ISupportInitialize).EndInit()
        SplitContainer9.ResumeLayout(False)
        ResumeLayout(False)
    End Sub

    'Friend WithEvents UcProgLineEdit1 As ucProgLineEdit
    Friend WithEvents lstProgDisplay As ListBox
    Friend WithEvents cmdAddRow As Button
    Friend WithEvents cmdDeleteRow As Button
    Friend WithEvents cmdMoveRowUp As Button
    Friend WithEvents cmdInsertRow As Button
    Friend WithEvents lstPrograms As ListBox
    Friend WithEvents cmdMoveDown As Button
    Friend WithEvents cmdDuplicateProgLine As Button
    Friend WithEvents cmsProgSentences As ContextMenuStrip
    Friend WithEvents cmsPsAddLine As ToolStripMenuItem
    Friend WithEvents cmsPsInsertLine As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator2 As ToolStripSeparator
    Friend WithEvents cmsPsDeleteLine As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator1 As ToolStripSeparator
    Friend WithEvents cmsPsMoveLineUp As ToolStripMenuItem
    Friend WithEvents cmsPsMoveLineDown As ToolStripMenuItem
    Friend WithEvents cmsPsDuplicateLine As ToolStripMenuItem
    Friend WithEvents cmsPrograms As ContextMenuStrip
    Friend WithEvents cmsProgInsertBlankProgram As ToolStripMenuItem
    Friend WithEvents cmsProgDuplicateProgram As ToolStripMenuItem
    Friend WithEvents cmsProgAddBlankProgramToEnd As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator3 As ToolStripSeparator
    Friend WithEvents cmsProgMoveProgramTowardsBeginning As ToolStripMenuItem
    Friend WithEvents cmsProgMoveProgramTowardsEnd As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator4 As ToolStripSeparator
    Friend WithEvents cmsProgDeleteProgram As ToolStripMenuItem
    Friend WithEvents Button3 As Button
    Friend WithEvents lblProgName As Label
    Friend WithEvents txtProgName As TextBox
    Friend WithEvents cmdEditVarNames As Button
    Friend WithEvents tabBottom As TabControl
    Friend WithEvents TabBottomProgram As TabPage
    Friend WithEvents TabPage2 As TabPage
    Friend WithEvents SplitContainer3 As SplitContainer
    Friend WithEvents ofdProject As OpenFileDialog
    Friend WithEvents sfdProject As SaveFileDialog
    Friend WithEvents Label1 As Label
    Friend WithEvents txtProjectName As TextBox
    Friend WithEvents cmsProgImportProgram As ToolStripMenuItem
    Friend WithEvents pnlProgramList As Panel
    Friend WithEvents mnuTabProgram As MenuStrip
    Friend WithEvents mnuMainProject As ToolStripMenuItem
    Friend WithEvents mnuMainPrograms As ToolStripMenuItem
    Friend WithEvents mnuProgLines As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem2 As ToolStripMenuItem
    Friend WithEvents Panel1 As Panel
    Friend WithEvents SplitContainer1 As SplitContainer
    Friend WithEvents SplitContainer4 As SplitContainer
    Friend WithEvents SplitContainer5 As SplitContainer
    Friend WithEvents OpenProjectFromFileToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents CreateNewProjectToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents SaveProjectToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents SaveProjectAsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents AddNewBlankProgramToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ImportExistingProgramToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents MoveProgramTowardsBeginningToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents MoveProgramTowardsEndToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents mnuAddBlankProgramAtIndex As ToolStripMenuItem
    Friend WithEvents mnuAddBlankProgramAtEnd As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator5 As ToolStripSeparator
    Friend WithEvents ToolStripSeparator6 As ToolStripSeparator
    Friend WithEvents DeleteProgramToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents mnuAddProgLine As ToolStripMenuItem
    Friend WithEvents mnuInsertProgLine As ToolStripMenuItem
    Friend WithEvents mnuDuplicateProgLine As ToolStripMenuItem
    Friend WithEvents mnuMoveProgLineUp As ToolStripMenuItem
    Friend WithEvents mnuDuplicateProgram As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator7 As ToolStripSeparator
    Friend WithEvents mnuMoveProgramLineDown As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator8 As ToolStripSeparator
    Friend WithEvents mnuDeleteProgramLine As ToolStripMenuItem
    Friend WithEvents SplitContainer2 As SplitContainer
    Friend WithEvents tmrNewMessages As Timer
    Friend WithEvents SplitContainer6 As SplitContainer
    'Friend WithEvents cboComPorts As ComboBox
    Friend WithEvents SplitContainer7 As SplitContainer
    Friend WithEvents cmdStop As Button
    Friend WithEvents ToolStrip1 As ToolStrip
    Friend WithEvents ToolStripComboBox1 As ToolStripComboBox
    Friend WithEvents cmdRefreshComportList As ToolStripButton
    Friend WithEvents cmdConnect As ToolStripButton
    Friend WithEvents cmdAutostatus As ToolStripDropDownButton
    Friend WithEvents lblDevStatus As ToolStripLabel
    Friend WithEvents cboComPorts As ToolStripComboBox
    Friend WithEvents ToolStripLabelAutoStatus As ToolStripLabel
    Friend WithEvents cmdtsChanMonAutoStatusOff As ToolStripButton
    Friend WithEvents cmdtsChanMonAutoStatusOn As ToolStripButton
    Friend WithEvents ToolStripSeparator10 As ToolStripSeparator
    Friend WithEvents ToolStripSeparator9 As ToolStripSeparator
    Friend WithEvents UcProgLineEdit1 As ucProgLineEdit
    Friend WithEvents ToolStripSeparator11 As ToolStripSeparator
    Friend WithEvents ToolStripSeparator12 As ToolStripSeparator
    Friend WithEvents OpenProjectFromDeviceToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents DownloadProjectToDeviceToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents StatusStrip1 As StatusStrip
    Friend WithEvents ssmProgressBar As ToolStripProgressBar
    Friend WithEvents ssmStatusText As ToolStripStatusLabel
    Friend WithEvents bwDLProgFromDevice As System.ComponentModel.BackgroundWorker
    Friend WithEvents bwULProgToDevice As System.ComponentModel.BackgroundWorker
    Friend WithEvents tmrClearStatusbar As Timer
    Friend WithEvents chkLiveTrackLines As CheckBox
    Friend WithEvents cmsProgEditVarNames As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator13 As ToolStripSeparator
    Friend WithEvents ToolStripContainer1 As ToolStripContainer
    Friend WithEvents ToolStrip2 As ToolStrip
    Friend WithEvents tscmdNewProject As ToolStripButton
    Friend WithEvents ToolStripSeparator14 As ToolStripSeparator
    Friend WithEvents tscmdOpenProjectFromFile As ToolStripButton
    Friend WithEvents tscmdSaveProjectToFile As ToolStripButton
    Friend WithEvents ToolStripSeparator15 As ToolStripSeparator
    Friend WithEvents tscmdOpenProgramFromDevice As ToolStripButton
    Friend WithEvents tscmdSaveProgramToDevice As ToolStripButton
    Friend WithEvents SplitContainer8 As SplitContainer
    Friend WithEvents tmrCheckComportStatus As Timer
    Friend WithEvents tmrCheckAutostatus As Timer
    Friend WithEvents cmsProgEditTimerNames As ToolStripMenuItem
    Friend WithEvents tsmDeviceType As ToolStripMenuItem
    Friend WithEvents lblDeviceType As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents txtDeviceName As TextBox
    Friend WithEvents Label4 As Label
    Friend WithEvents cmdResets As ToolStripDropDownButton
    Friend WithEvents cmdResetDevice As ToolStripMenuItem
    Friend WithEvents cmdEraseDevice As ToolStripMenuItem
    Friend WithEvents SplitContainer9 As SplitContainer
    Friend WithEvents UcChanControl1 As ucChanControl
    Friend WithEvents TabControl1 As TabControl
    Friend WithEvents tabpageLineDetails As TabPage
    Friend WithEvents tabpageVariables As TabPage
    Friend WithEvents ToolStripSeparator16 As ToolStripSeparator
    Friend WithEvents ToolStripSeparator17 As ToolStripSeparator
    Friend WithEvents tsbtnDebugRun As ToolStripButton
    Friend WithEvents tsbtnDebugPause As ToolStripButton
    Friend WithEvents tsbtnDebugStep As ToolStripButton
    Friend WithEvents tabpageTimers As TabPage
    Friend WithEvents SplitContainerVariables As SplitContainer
    Friend WithEvents cboVariablesChannel As ComboBox
    Friend WithEvents dgvVariables As DataGridView
    Friend WithEvents SplitContainerTimers As SplitContainer
    Friend WithEvents cboTimersChannel As ComboBox
    Friend WithEvents dgvTimers As DataGridView

End Class
