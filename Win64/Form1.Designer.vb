<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
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
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
        Me.SerialPort1 = New System.IO.Ports.SerialPort(Me.components)
        Me.TabControlMain = New System.Windows.Forms.TabControl()
        Me.tabChannelMonitor = New System.Windows.Forms.TabPage()
        Me.tsTabChanMon = New System.Windows.Forms.ToolStrip()
        Me.ToolStripLabelAutoStatus = New System.Windows.Forms.ToolStripLabel()
        Me.cmdtsChanMonAutoStatusOff = New System.Windows.Forms.ToolStripButton()
        Me.cmdtsChanMon_AutoStatusFast = New System.Windows.Forms.ToolStripButton()
        Me.cmdtsChanMon_AutoStatusSlow = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripLabel1 = New System.Windows.Forms.ToolStripLabel()
        Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        Me.tsChanMonDDCmdTensMaxV = New System.Windows.Forms.ToolStripDropDownButton()
        Me.tsCMTensMax_Low = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsCMTensMax_Med = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsCMTensMax_High = New System.Windows.Forms.ToolStripMenuItem()
        Me.tabPrograms = New System.Windows.Forms.TabPage()
        Me.splitConProgramsTab = New System.Windows.Forms.SplitContainer()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.lblProgsTotalNumProgs = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.txtProgName = New System.Windows.Forms.TextBox()
        Me.cboProgNumSelect = New System.Windows.Forms.ComboBox()
        Me.msTabPrograms = New System.Windows.Forms.MenuStrip()
        Me.FileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CreateNewProjectToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.OpenProjectFromComputerToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.GetProjectFromDeviceToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SaveProjectToDeviceToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SaveProjectToComputerToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ProgramToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AddNewProgramToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ImportProgramFromFileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ExportCurrentProgramToFileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator()
        Me.MoveCurrentProgramTowardsBeginningToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.MoveCurrentProgramTowardsEndToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator5 = New System.Windows.Forms.ToolStripSeparator()
        Me.DeleteCurrentProgramToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.EditToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AddNewRowToEndToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.InsertNewRowToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.MoveActiveRowUpToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.MoveActiveRowDownToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator4 = New System.Windows.Forms.ToolStripSeparator()
        Me.DeleteActiveRowToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ucProg = New Tens25.UserControlPrograms()
        Me.tabSettings = New System.Windows.Forms.TabPage()
        Me.tmrNewMessages = New System.Windows.Forms.Timer(Me.components)
        Me.tmrStatusUpdates = New System.Windows.Forms.Timer(Me.components)
        Me.SplitContainerMain = New System.Windows.Forms.SplitContainer()
        Me.cmdStop = New System.Windows.Forms.Button()
        Me.lblDevStatus = New System.Windows.Forms.Label()
        Me.cmdConnect = New System.Windows.Forms.Button()
        Me.cmdRefreshComportList = New System.Windows.Forms.Button()
        Me.cboComPorts = New System.Windows.Forms.ComboBox()
        Me.OpenFileDialogProject = New System.Windows.Forms.OpenFileDialog()
        Me.SaveFileDialogProject = New System.Windows.Forms.SaveFileDialog()
        Me.chkOffline = New System.Windows.Forms.CheckBox()
        Me.TabControlMain.SuspendLayout()
        Me.tabChannelMonitor.SuspendLayout()
        Me.tsTabChanMon.SuspendLayout()
        Me.tabPrograms.SuspendLayout()
        CType(Me.splitConProgramsTab, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.splitConProgramsTab.Panel1.SuspendLayout()
        Me.splitConProgramsTab.Panel2.SuspendLayout()
        Me.splitConProgramsTab.SuspendLayout()
        Me.msTabPrograms.SuspendLayout()
        CType(Me.SplitContainerMain, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainerMain.Panel1.SuspendLayout()
        Me.SplitContainerMain.Panel2.SuspendLayout()
        Me.SplitContainerMain.SuspendLayout()
        Me.SuspendLayout()
        '
        'TabControlMain
        '
        Me.TabControlMain.Controls.Add(Me.tabChannelMonitor)
        Me.TabControlMain.Controls.Add(Me.tabPrograms)
        Me.TabControlMain.Controls.Add(Me.tabSettings)
        Me.TabControlMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TabControlMain.Location = New System.Drawing.Point(0, 0)
        Me.TabControlMain.Name = "TabControlMain"
        Me.TabControlMain.SelectedIndex = 0
        Me.TabControlMain.Size = New System.Drawing.Size(952, 508)
        Me.TabControlMain.TabIndex = 9
        '
        'tabChannelMonitor
        '
        Me.tabChannelMonitor.AutoScroll = True
        Me.tabChannelMonitor.BackColor = System.Drawing.SystemColors.Control
        Me.tabChannelMonitor.Controls.Add(Me.tsTabChanMon)
        Me.tabChannelMonitor.Location = New System.Drawing.Point(4, 22)
        Me.tabChannelMonitor.Name = "tabChannelMonitor"
        Me.tabChannelMonitor.Padding = New System.Windows.Forms.Padding(3)
        Me.tabChannelMonitor.Size = New System.Drawing.Size(1070, 482)
        Me.tabChannelMonitor.TabIndex = 0
        Me.tabChannelMonitor.Text = "Channel Monitor"
        '
        'tsTabChanMon
        '
        Me.tsTabChanMon.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripLabelAutoStatus, Me.cmdtsChanMonAutoStatusOff, Me.cmdtsChanMon_AutoStatusFast, Me.cmdtsChanMon_AutoStatusSlow, Me.ToolStripSeparator1, Me.ToolStripLabel1, Me.ToolStripSeparator2, Me.tsChanMonDDCmdTensMaxV})
        Me.tsTabChanMon.Location = New System.Drawing.Point(3, 3)
        Me.tsTabChanMon.Name = "tsTabChanMon"
        Me.tsTabChanMon.Size = New System.Drawing.Size(1064, 25)
        Me.tsTabChanMon.TabIndex = 0
        Me.tsTabChanMon.Text = "ToolStrip1"
        '
        'ToolStripLabelAutoStatus
        '
        Me.ToolStripLabelAutoStatus.Name = "ToolStripLabelAutoStatus"
        Me.ToolStripLabelAutoStatus.Size = New System.Drawing.Size(68, 22)
        Me.ToolStripLabelAutoStatus.Text = "AutoStatus:"
        '
        'cmdtsChanMonAutoStatusOff
        '
        Me.cmdtsChanMonAutoStatusOff.Checked = True
        Me.cmdtsChanMonAutoStatusOff.CheckState = System.Windows.Forms.CheckState.Checked
        Me.cmdtsChanMonAutoStatusOff.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.cmdtsChanMonAutoStatusOff.Image = CType(resources.GetObject("cmdtsChanMonAutoStatusOff.Image"), System.Drawing.Image)
        Me.cmdtsChanMonAutoStatusOff.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.cmdtsChanMonAutoStatusOff.Name = "cmdtsChanMonAutoStatusOff"
        Me.cmdtsChanMonAutoStatusOff.Size = New System.Drawing.Size(28, 22)
        Me.cmdtsChanMonAutoStatusOff.Text = "Off"
        Me.cmdtsChanMonAutoStatusOff.ToolTipText = "Turn  Autostatus Off"
        '
        'cmdtsChanMon_AutoStatusFast
        '
        Me.cmdtsChanMon_AutoStatusFast.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.cmdtsChanMon_AutoStatusFast.ImageTransparentColor = System.Drawing.Color.Transparent
        Me.cmdtsChanMon_AutoStatusFast.Name = "cmdtsChanMon_AutoStatusFast"
        Me.cmdtsChanMon_AutoStatusFast.Size = New System.Drawing.Size(32, 22)
        Me.cmdtsChanMon_AutoStatusFast.Text = "Fast"
        Me.cmdtsChanMon_AutoStatusFast.ToolTipText = "Turn AutoStatus on, FAST mode"
        '
        'cmdtsChanMon_AutoStatusSlow
        '
        Me.cmdtsChanMon_AutoStatusSlow.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.cmdtsChanMon_AutoStatusSlow.ImageTransparentColor = System.Drawing.Color.Transparent
        Me.cmdtsChanMon_AutoStatusSlow.Name = "cmdtsChanMon_AutoStatusSlow"
        Me.cmdtsChanMon_AutoStatusSlow.Size = New System.Drawing.Size(36, 22)
        Me.cmdtsChanMon_AutoStatusSlow.Text = "Slow"
        Me.cmdtsChanMon_AutoStatusSlow.ToolTipText = "Turn Autostatus on, SLOW mode"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(6, 25)
        '
        'ToolStripLabel1
        '
        Me.ToolStripLabel1.Name = "ToolStripLabel1"
        Me.ToolStripLabel1.Size = New System.Drawing.Size(19, 22)
        Me.ToolStripLabel1.Text = "    "
        '
        'ToolStripSeparator2
        '
        Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
        Me.ToolStripSeparator2.Size = New System.Drawing.Size(6, 25)
        '
        'tsChanMonDDCmdTensMaxV
        '
        Me.tsChanMonDDCmdTensMaxV.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.tsChanMonDDCmdTensMaxV.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsCMTensMax_Low, Me.tsCMTensMax_Med, Me.tsCMTensMax_High})
        Me.tsChanMonDDCmdTensMaxV.Image = CType(resources.GetObject("tsChanMonDDCmdTensMaxV.Image"), System.Drawing.Image)
        Me.tsChanMonDDCmdTensMaxV.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsChanMonDDCmdTensMaxV.Name = "tsChanMonDDCmdTensMaxV"
        Me.tsChanMonDDCmdTensMaxV.Size = New System.Drawing.Size(135, 22)
        Me.tsChanMonDDCmdTensMaxV.Text = "Max Tens Output:  0%"
        '
        'tsCMTensMax_Low
        '
        Me.tsCMTensMax_Low.Name = "tsCMTensMax_Low"
        Me.tsCMTensMax_Low.Size = New System.Drawing.Size(181, 22)
        Me.tsCMTensMax_Low.Text = "ToolStripMenuItem1"
        '
        'tsCMTensMax_Med
        '
        Me.tsCMTensMax_Med.Name = "tsCMTensMax_Med"
        Me.tsCMTensMax_Med.Size = New System.Drawing.Size(181, 22)
        Me.tsCMTensMax_Med.Text = "ToolStripMenuItem2"
        '
        'tsCMTensMax_High
        '
        Me.tsCMTensMax_High.Name = "tsCMTensMax_High"
        Me.tsCMTensMax_High.Size = New System.Drawing.Size(181, 22)
        Me.tsCMTensMax_High.Text = "ToolStripMenuItem3"
        '
        'tabPrograms
        '
        Me.tabPrograms.BackColor = System.Drawing.SystemColors.Control
        Me.tabPrograms.Controls.Add(Me.splitConProgramsTab)
        Me.tabPrograms.Location = New System.Drawing.Point(4, 22)
        Me.tabPrograms.Name = "tabPrograms"
        Me.tabPrograms.Padding = New System.Windows.Forms.Padding(3)
        Me.tabPrograms.Size = New System.Drawing.Size(944, 482)
        Me.tabPrograms.TabIndex = 1
        Me.tabPrograms.Text = "Programs"
        '
        'splitConProgramsTab
        '
        Me.splitConProgramsTab.Dock = System.Windows.Forms.DockStyle.Fill
        Me.splitConProgramsTab.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
        Me.splitConProgramsTab.Location = New System.Drawing.Point(3, 3)
        Me.splitConProgramsTab.Name = "splitConProgramsTab"
        Me.splitConProgramsTab.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'splitConProgramsTab.Panel1
        '
        Me.splitConProgramsTab.Panel1.Controls.Add(Me.Label3)
        Me.splitConProgramsTab.Panel1.Controls.Add(Me.lblProgsTotalNumProgs)
        Me.splitConProgramsTab.Panel1.Controls.Add(Me.Label2)
        Me.splitConProgramsTab.Panel1.Controls.Add(Me.Label1)
        Me.splitConProgramsTab.Panel1.Controls.Add(Me.txtProgName)
        Me.splitConProgramsTab.Panel1.Controls.Add(Me.cboProgNumSelect)
        Me.splitConProgramsTab.Panel1.Controls.Add(Me.msTabPrograms)
        '
        'splitConProgramsTab.Panel2
        '
        Me.splitConProgramsTab.Panel2.Controls.Add(Me.ucProg)
        Me.splitConProgramsTab.Size = New System.Drawing.Size(938, 476)
        Me.splitConProgramsTab.SplitterDistance = 63
        Me.splitConProgramsTab.TabIndex = 4
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(200, 40)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(38, 13)
        Me.Label3.TabIndex = 9
        Me.Label3.Text = "Name:"
        '
        'lblProgsTotalNumProgs
        '
        Me.lblProgsTotalNumProgs.Location = New System.Drawing.Point(154, 40)
        Me.lblProgsTotalNumProgs.Name = "lblProgsTotalNumProgs"
        Me.lblProgsTotalNumProgs.Size = New System.Drawing.Size(34, 13)
        Me.lblProgsTotalNumProgs.TabIndex = 8
        Me.lblProgsTotalNumProgs.Text = "0"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(129, 40)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(19, 13)
        Me.Label2.TabIndex = 7
        Me.Label2.Text = "of:"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(13, 40)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(49, 13)
        Me.Label1.TabIndex = 6
        Me.Label1.Text = "Program:"
        '
        'txtProgName
        '
        Me.txtProgName.Location = New System.Drawing.Point(244, 37)
        Me.txtProgName.MaxLength = 10
        Me.txtProgName.Name = "txtProgName"
        Me.txtProgName.Size = New System.Drawing.Size(122, 20)
        Me.txtProgName.TabIndex = 5
        '
        'cboProgNumSelect
        '
        Me.cboProgNumSelect.FormattingEnabled = True
        Me.cboProgNumSelect.Location = New System.Drawing.Point(68, 37)
        Me.cboProgNumSelect.Name = "cboProgNumSelect"
        Me.cboProgNumSelect.Size = New System.Drawing.Size(55, 21)
        Me.cboProgNumSelect.TabIndex = 2
        '
        'msTabPrograms
        '
        Me.msTabPrograms.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FileToolStripMenuItem, Me.ProgramToolStripMenuItem, Me.EditToolStripMenuItem})
        Me.msTabPrograms.Location = New System.Drawing.Point(0, 0)
        Me.msTabPrograms.Name = "msTabPrograms"
        Me.msTabPrograms.Size = New System.Drawing.Size(938, 24)
        Me.msTabPrograms.TabIndex = 15
        Me.msTabPrograms.Text = "MenuStrip1"
        '
        'FileToolStripMenuItem
        '
        Me.FileToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.CreateNewProjectToolStripMenuItem, Me.OpenProjectFromComputerToolStripMenuItem, Me.GetProjectFromDeviceToolStripMenuItem, Me.SaveProjectToDeviceToolStripMenuItem, Me.SaveProjectToComputerToolStripMenuItem})
        Me.FileToolStripMenuItem.Name = "FileToolStripMenuItem"
        Me.FileToolStripMenuItem.Size = New System.Drawing.Size(37, 20)
        Me.FileToolStripMenuItem.Text = "File"
        '
        'CreateNewProjectToolStripMenuItem
        '
        Me.CreateNewProjectToolStripMenuItem.Name = "CreateNewProjectToolStripMenuItem"
        Me.CreateNewProjectToolStripMenuItem.Size = New System.Drawing.Size(231, 22)
        Me.CreateNewProjectToolStripMenuItem.Text = "Create New Project"
        '
        'OpenProjectFromComputerToolStripMenuItem
        '
        Me.OpenProjectFromComputerToolStripMenuItem.Name = "OpenProjectFromComputerToolStripMenuItem"
        Me.OpenProjectFromComputerToolStripMenuItem.Size = New System.Drawing.Size(231, 22)
        Me.OpenProjectFromComputerToolStripMenuItem.Text = "Open Project From Computer"
        '
        'GetProjectFromDeviceToolStripMenuItem
        '
        Me.GetProjectFromDeviceToolStripMenuItem.Name = "GetProjectFromDeviceToolStripMenuItem"
        Me.GetProjectFromDeviceToolStripMenuItem.Size = New System.Drawing.Size(231, 22)
        Me.GetProjectFromDeviceToolStripMenuItem.Text = "Get Project From Device"
        '
        'SaveProjectToDeviceToolStripMenuItem
        '
        Me.SaveProjectToDeviceToolStripMenuItem.Name = "SaveProjectToDeviceToolStripMenuItem"
        Me.SaveProjectToDeviceToolStripMenuItem.Size = New System.Drawing.Size(231, 22)
        Me.SaveProjectToDeviceToolStripMenuItem.Text = "Save Project To Device"
        '
        'SaveProjectToComputerToolStripMenuItem
        '
        Me.SaveProjectToComputerToolStripMenuItem.Name = "SaveProjectToComputerToolStripMenuItem"
        Me.SaveProjectToComputerToolStripMenuItem.Size = New System.Drawing.Size(231, 22)
        Me.SaveProjectToComputerToolStripMenuItem.Text = "Save Project To Computer"
        '
        'ProgramToolStripMenuItem
        '
        Me.ProgramToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.AddNewProgramToolStripMenuItem, Me.ImportProgramFromFileToolStripMenuItem, Me.ExportCurrentProgramToFileToolStripMenuItem, Me.ToolStripSeparator3, Me.MoveCurrentProgramTowardsBeginningToolStripMenuItem, Me.MoveCurrentProgramTowardsEndToolStripMenuItem, Me.ToolStripSeparator5, Me.DeleteCurrentProgramToolStripMenuItem})
        Me.ProgramToolStripMenuItem.Name = "ProgramToolStripMenuItem"
        Me.ProgramToolStripMenuItem.Size = New System.Drawing.Size(65, 20)
        Me.ProgramToolStripMenuItem.Text = "Program"
        '
        'AddNewProgramToolStripMenuItem
        '
        Me.AddNewProgramToolStripMenuItem.Name = "AddNewProgramToolStripMenuItem"
        Me.AddNewProgramToolStripMenuItem.Size = New System.Drawing.Size(299, 22)
        Me.AddNewProgramToolStripMenuItem.Text = "Add New (Blank) Program"
        '
        'ImportProgramFromFileToolStripMenuItem
        '
        Me.ImportProgramFromFileToolStripMenuItem.Name = "ImportProgramFromFileToolStripMenuItem"
        Me.ImportProgramFromFileToolStripMenuItem.Size = New System.Drawing.Size(299, 22)
        Me.ImportProgramFromFileToolStripMenuItem.Text = "Import Program from File"
        '
        'ExportCurrentProgramToFileToolStripMenuItem
        '
        Me.ExportCurrentProgramToFileToolStripMenuItem.Name = "ExportCurrentProgramToFileToolStripMenuItem"
        Me.ExportCurrentProgramToFileToolStripMenuItem.Size = New System.Drawing.Size(299, 22)
        Me.ExportCurrentProgramToFileToolStripMenuItem.Text = "Export Current Program to File"
        '
        'ToolStripSeparator3
        '
        Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
        Me.ToolStripSeparator3.Size = New System.Drawing.Size(296, 6)
        '
        'MoveCurrentProgramTowardsBeginningToolStripMenuItem
        '
        Me.MoveCurrentProgramTowardsBeginningToolStripMenuItem.Name = "MoveCurrentProgramTowardsBeginningToolStripMenuItem"
        Me.MoveCurrentProgramTowardsBeginningToolStripMenuItem.Size = New System.Drawing.Size(299, 22)
        Me.MoveCurrentProgramTowardsBeginningToolStripMenuItem.Text = "Move Current Program Towards Beginning"
        '
        'MoveCurrentProgramTowardsEndToolStripMenuItem
        '
        Me.MoveCurrentProgramTowardsEndToolStripMenuItem.Name = "MoveCurrentProgramTowardsEndToolStripMenuItem"
        Me.MoveCurrentProgramTowardsEndToolStripMenuItem.Size = New System.Drawing.Size(299, 22)
        Me.MoveCurrentProgramTowardsEndToolStripMenuItem.Text = "Move Current Program Towards End"
        '
        'ToolStripSeparator5
        '
        Me.ToolStripSeparator5.Name = "ToolStripSeparator5"
        Me.ToolStripSeparator5.Size = New System.Drawing.Size(296, 6)
        '
        'DeleteCurrentProgramToolStripMenuItem
        '
        Me.DeleteCurrentProgramToolStripMenuItem.Name = "DeleteCurrentProgramToolStripMenuItem"
        Me.DeleteCurrentProgramToolStripMenuItem.Size = New System.Drawing.Size(299, 22)
        Me.DeleteCurrentProgramToolStripMenuItem.Text = "Delete Current Program"
        '
        'EditToolStripMenuItem
        '
        Me.EditToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.AddNewRowToEndToolStripMenuItem, Me.InsertNewRowToolStripMenuItem, Me.MoveActiveRowUpToolStripMenuItem, Me.MoveActiveRowDownToolStripMenuItem, Me.ToolStripSeparator4, Me.DeleteActiveRowToolStripMenuItem})
        Me.EditToolStripMenuItem.Name = "EditToolStripMenuItem"
        Me.EditToolStripMenuItem.Size = New System.Drawing.Size(39, 20)
        Me.EditToolStripMenuItem.Text = "Edit"
        '
        'AddNewRowToEndToolStripMenuItem
        '
        Me.AddNewRowToEndToolStripMenuItem.Name = "AddNewRowToEndToolStripMenuItem"
        Me.AddNewRowToEndToolStripMenuItem.Size = New System.Drawing.Size(200, 22)
        Me.AddNewRowToEndToolStripMenuItem.Text = "Add New Row to End"
        '
        'InsertNewRowToolStripMenuItem
        '
        Me.InsertNewRowToolStripMenuItem.Name = "InsertNewRowToolStripMenuItem"
        Me.InsertNewRowToolStripMenuItem.Size = New System.Drawing.Size(200, 22)
        Me.InsertNewRowToolStripMenuItem.Text = "Insert New Row"
        '
        'MoveActiveRowUpToolStripMenuItem
        '
        Me.MoveActiveRowUpToolStripMenuItem.Name = "MoveActiveRowUpToolStripMenuItem"
        Me.MoveActiveRowUpToolStripMenuItem.Size = New System.Drawing.Size(200, 22)
        Me.MoveActiveRowUpToolStripMenuItem.Text = "Move Active Row Up"
        '
        'MoveActiveRowDownToolStripMenuItem
        '
        Me.MoveActiveRowDownToolStripMenuItem.Name = "MoveActiveRowDownToolStripMenuItem"
        Me.MoveActiveRowDownToolStripMenuItem.Size = New System.Drawing.Size(200, 22)
        Me.MoveActiveRowDownToolStripMenuItem.Text = "Move Active Row Down"
        '
        'ToolStripSeparator4
        '
        Me.ToolStripSeparator4.Name = "ToolStripSeparator4"
        Me.ToolStripSeparator4.Size = New System.Drawing.Size(197, 6)
        '
        'DeleteActiveRowToolStripMenuItem
        '
        Me.DeleteActiveRowToolStripMenuItem.Name = "DeleteActiveRowToolStripMenuItem"
        Me.DeleteActiveRowToolStripMenuItem.Size = New System.Drawing.Size(200, 22)
        Me.DeleteActiveRowToolStripMenuItem.Text = "Delete Active Row"
        '
        'ucProg
        '
        Me.ucProg.ActiveRow = -1
        Me.ucProg.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.ucProg.Dock = System.Windows.Forms.DockStyle.Left
        Me.ucProg.Location = New System.Drawing.Point(0, 0)
        Me.ucProg.MinimumSize = New System.Drawing.Size(689, 30)
        Me.ucProg.Name = "ucProg"
        Me.ucProg.NumMotorChannels = 2
        Me.ucProg.NumSysChannels = 1
        Me.ucProg.NumTensChannels = 2
        Me.ucProg.Size = New System.Drawing.Size(689, 409)
        Me.ucProg.TabIndex = 4
        '
        'tabSettings
        '
        Me.tabSettings.BackColor = System.Drawing.SystemColors.Control
        Me.tabSettings.Location = New System.Drawing.Point(4, 22)
        Me.tabSettings.Name = "tabSettings"
        Me.tabSettings.Size = New System.Drawing.Size(1070, 482)
        Me.tabSettings.TabIndex = 2
        Me.tabSettings.Text = "Settings"
        '
        'tmrNewMessages
        '
        Me.tmrNewMessages.Enabled = True
        Me.tmrNewMessages.Interval = 1
        '
        'tmrStatusUpdates
        '
        Me.tmrStatusUpdates.Interval = 50
        '
        'SplitContainerMain
        '
        Me.SplitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainerMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
        Me.SplitContainerMain.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainerMain.Name = "SplitContainerMain"
        Me.SplitContainerMain.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainerMain.Panel1
        '
        Me.SplitContainerMain.Panel1.Controls.Add(Me.chkOffline)
        Me.SplitContainerMain.Panel1.Controls.Add(Me.cmdStop)
        Me.SplitContainerMain.Panel1.Controls.Add(Me.lblDevStatus)
        Me.SplitContainerMain.Panel1.Controls.Add(Me.cmdConnect)
        Me.SplitContainerMain.Panel1.Controls.Add(Me.cmdRefreshComportList)
        Me.SplitContainerMain.Panel1.Controls.Add(Me.cboComPorts)
        '
        'SplitContainerMain.Panel2
        '
        Me.SplitContainerMain.Panel2.Controls.Add(Me.TabControlMain)
        Me.SplitContainerMain.Size = New System.Drawing.Size(952, 537)
        Me.SplitContainerMain.SplitterDistance = 25
        Me.SplitContainerMain.TabIndex = 11
        '
        'cmdStop
        '
        Me.cmdStop.BackColor = System.Drawing.Color.Red
        Me.cmdStop.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmdStop.ForeColor = System.Drawing.Color.White
        Me.cmdStop.Location = New System.Drawing.Point(546, 2)
        Me.cmdStop.Name = "cmdStop"
        Me.cmdStop.Size = New System.Drawing.Size(75, 20)
        Me.cmdStop.TabIndex = 4
        Me.cmdStop.Text = "STOP"
        Me.cmdStop.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.cmdStop.UseVisualStyleBackColor = False
        '
        'lblDevStatus
        '
        Me.lblDevStatus.AutoSize = True
        Me.lblDevStatus.Location = New System.Drawing.Point(248, 6)
        Me.lblDevStatus.Name = "lblDevStatus"
        Me.lblDevStatus.Size = New System.Drawing.Size(122, 13)
        Me.lblDevStatus.TabIndex = 3
        Me.lblDevStatus.Text = "No Device Connected..."
        '
        'cmdConnect
        '
        Me.cmdConnect.Location = New System.Drawing.Point(154, 2)
        Me.cmdConnect.Name = "cmdConnect"
        Me.cmdConnect.Size = New System.Drawing.Size(75, 20)
        Me.cmdConnect.TabIndex = 2
        Me.cmdConnect.Text = "Connect"
        Me.cmdConnect.UseVisualStyleBackColor = True
        '
        'cmdRefreshComportList
        '
        Me.cmdRefreshComportList.BackgroundImage = Global.Tens25.My.Resources.Resources._159061
        Me.cmdRefreshComportList.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.cmdRefreshComportList.FlatAppearance.BorderSize = 0
        Me.cmdRefreshComportList.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cmdRefreshComportList.Location = New System.Drawing.Point(130, 3)
        Me.cmdRefreshComportList.Name = "cmdRefreshComportList"
        Me.cmdRefreshComportList.Size = New System.Drawing.Size(18, 18)
        Me.cmdRefreshComportList.TabIndex = 1
        Me.cmdRefreshComportList.UseVisualStyleBackColor = True
        '
        'cboComPorts
        '
        Me.cboComPorts.FormattingEnabled = True
        Me.cboComPorts.Location = New System.Drawing.Point(3, 3)
        Me.cboComPorts.Name = "cboComPorts"
        Me.cboComPorts.Size = New System.Drawing.Size(121, 21)
        Me.cboComPorts.TabIndex = 0
        '
        'OpenFileDialogProject
        '
        Me.OpenFileDialogProject.DefaultExt = "tp"
        Me.OpenFileDialogProject.FileName = "OpenFileDialog1"
        Me.OpenFileDialogProject.Filter = "Tens Project Files|*.tp|All Files|*.*"
        '
        'SaveFileDialogProject
        '
        Me.SaveFileDialogProject.DefaultExt = "tp"
        Me.SaveFileDialogProject.Filter = "Tens Project Files|*.tp|All Files|*.*"
        '
        'chkOffline
        '
        Me.chkOffline.AutoSize = True
        Me.chkOffline.Location = New System.Drawing.Point(640, 4)
        Me.chkOffline.Name = "chkOffline"
        Me.chkOffline.Size = New System.Drawing.Size(56, 17)
        Me.chkOffline.TabIndex = 5
        Me.chkOffline.Text = "Offline"
        Me.chkOffline.UseVisualStyleBackColor = True
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(952, 537)
        Me.Controls.Add(Me.SplitContainerMain)
        Me.MainMenuStrip = Me.msTabPrograms
        Me.Name = "Form1"
        Me.Text = "Ten 25"
        Me.TabControlMain.ResumeLayout(False)
        Me.tabChannelMonitor.ResumeLayout(False)
        Me.tabChannelMonitor.PerformLayout()
        Me.tsTabChanMon.ResumeLayout(False)
        Me.tsTabChanMon.PerformLayout()
        Me.tabPrograms.ResumeLayout(False)
        Me.splitConProgramsTab.Panel1.ResumeLayout(False)
        Me.splitConProgramsTab.Panel1.PerformLayout()
        Me.splitConProgramsTab.Panel2.ResumeLayout(False)
        CType(Me.splitConProgramsTab, System.ComponentModel.ISupportInitialize).EndInit()
        Me.splitConProgramsTab.ResumeLayout(False)
        Me.msTabPrograms.ResumeLayout(False)
        Me.msTabPrograms.PerformLayout()
        Me.SplitContainerMain.Panel1.ResumeLayout(False)
        Me.SplitContainerMain.Panel1.PerformLayout()
        Me.SplitContainerMain.Panel2.ResumeLayout(False)
        CType(Me.SplitContainerMain, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainerMain.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents SerialPort1 As IO.Ports.SerialPort
    Friend WithEvents TabControlMain As TabControl
    Friend WithEvents tabChannelMonitor As TabPage
    Friend WithEvents tabPrograms As TabPage
    Friend WithEvents tabSettings As TabPage
    Friend WithEvents tmrNewMessages As Timer
    Friend WithEvents tmrStatusUpdates As Timer
    Friend WithEvents tsTabChanMon As ToolStrip
    Friend WithEvents cmdtsChanMon_AutoStatusFast As ToolStripButton
    Friend WithEvents cmdtsChanMon_AutoStatusSlow As ToolStripButton
    Friend WithEvents ToolStripLabel1 As ToolStripLabel
    Friend WithEvents ToolStripLabelAutoStatus As ToolStripLabel
    Friend WithEvents cmdtsChanMonAutoStatusOff As ToolStripButton
    Friend WithEvents ToolStripSeparator1 As ToolStripSeparator
    Friend WithEvents tsCMTensMax_Low As ToolStripMenuItem
    Friend WithEvents tsCMTensMax_Med As ToolStripMenuItem
    Friend WithEvents tsCMTensMax_High As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator2 As ToolStripSeparator
    Friend WithEvents tsChanMonDDCmdTensMaxV As ToolStripDropDownButton
    Friend WithEvents SplitContainerMain As SplitContainer
    Friend WithEvents cmdRefreshComportList As Button
    Friend WithEvents cboComPorts As ComboBox
    Friend WithEvents cmdConnect As Button
    Friend WithEvents lblDevStatus As Label
    Friend WithEvents cmdStop As Button
    Friend WithEvents cboProgNumSelect As ComboBox
    Friend WithEvents splitConProgramsTab As SplitContainer
    Friend WithEvents Label2 As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents txtProgName As TextBox
    Friend WithEvents lblProgsTotalNumProgs As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents OpenFileDialogProject As OpenFileDialog
    Friend WithEvents SaveFileDialogProject As SaveFileDialog
    Friend WithEvents ucProg As UserControlPrograms
    Friend WithEvents msTabPrograms As MenuStrip
    Friend WithEvents FileToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ProgramToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents CreateNewProjectToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents OpenProjectFromComputerToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents GetProjectFromDeviceToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents SaveProjectToDeviceToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents SaveProjectToComputerToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents AddNewProgramToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ImportProgramFromFileToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ExportCurrentProgramToFileToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator3 As ToolStripSeparator
    Friend WithEvents DeleteCurrentProgramToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents EditToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents AddNewRowToEndToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents InsertNewRowToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents DeleteActiveRowToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents MoveActiveRowUpToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents MoveActiveRowDownToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents MoveCurrentProgramTowardsBeginningToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents MoveCurrentProgramTowardsEndToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator5 As ToolStripSeparator
    Friend WithEvents ToolStripSeparator4 As ToolStripSeparator
    Friend WithEvents chkOffline As CheckBox
End Class
