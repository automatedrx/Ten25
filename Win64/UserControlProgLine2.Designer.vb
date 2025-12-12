<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UserControlProgLine2
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.cboCommand = New System.Windows.Forms.ComboBox()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.lblLine = New System.Windows.Forms.Label()
        Me.pnlSet = New System.Windows.Forms.Panel()
        Me.cboSetP82S = New System.Windows.Forms.ComboBox()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.cboSetP322S = New System.Windows.Forms.ComboBox()
        Me.txtSetP322V = New System.Windows.Forms.TextBox()
        Me.txtSetP321V = New System.Windows.Forms.TextBox()
        Me.cboSetP321S = New System.Windows.Forms.ComboBox()
        Me.txtSetP81V = New System.Windows.Forms.TextBox()
        Me.cboSetP81S = New System.Windows.Forms.ComboBox()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.pnlTest = New System.Windows.Forms.Panel()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.txtTestGotoFalse = New System.Windows.Forms.TextBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.txtTestGotoTrue = New System.Windows.Forms.TextBox()
        Me.cboTestP322S = New System.Windows.Forms.ComboBox()
        Me.txtTestP322V = New System.Windows.Forms.TextBox()
        Me.txtTestP321V = New System.Windows.Forms.TextBox()
        Me.cboTestP321S = New System.Windows.Forms.ComboBox()
        Me.txtTestP81V = New System.Windows.Forms.TextBox()
        Me.cboTestP82V = New System.Windows.Forms.ComboBox()
        Me.cboTestP81S = New System.Windows.Forms.ComboBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.pnlDelay = New System.Windows.Forms.Panel()
        Me.txtDelayP321V = New System.Windows.Forms.TextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.cboDelayP321S = New System.Windows.Forms.ComboBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.pnlProgOp = New System.Windows.Forms.Panel()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.lblProgOpLineSource = New System.Windows.Forms.Label()
        Me.cboProgOpChannel = New System.Windows.Forms.ComboBox()
        Me.txtProgOpP321V = New System.Windows.Forms.TextBox()
        Me.lblChannel = New System.Windows.Forms.Label()
        Me.cboProgOpP81S = New System.Windows.Forms.ComboBox()
        Me.cboProgOpP321S = New System.Windows.Forms.ComboBox()
        Me.pnlGoto = New System.Windows.Forms.Panel()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.txtGotoGotoTrue = New System.Windows.Forms.TextBox()
        Me.pnlNoOpEnd = New System.Windows.Forms.Panel()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.pnlTenMot = New System.Windows.Forms.Panel()
        Me.lblPolarity = New System.Windows.Forms.Label()
        Me.lblP321S = New System.Windows.Forms.Label()
        Me.lblP322VUnits = New System.Windows.Forms.Label()
        Me.txtTenMotP82V = New System.Windows.Forms.TextBox()
        Me.txtTenMotP321V = New System.Windows.Forms.TextBox()
        Me.cboTenMotP322S = New System.Windows.Forms.ComboBox()
        Me.lblP82S = New System.Windows.Forms.Label()
        Me.lblP321VUnits = New System.Windows.Forms.Label()
        Me.cboTenMotP321S = New System.Windows.Forms.ComboBox()
        Me.cboTenMotP82S = New System.Windows.Forms.ComboBox()
        Me.cboTenMotP81S = New System.Windows.Forms.ComboBox()
        Me.lblP322S = New System.Windows.Forms.Label()
        Me.lblP82VUnits = New System.Windows.Forms.Label()
        Me.cboTenMotPolarity = New System.Windows.Forms.ComboBox()
        Me.lblP81S = New System.Windows.Forms.Label()
        Me.txtTenMotP322V = New System.Windows.Forms.TextBox()
        Me.lblP81VUnits = New System.Windows.Forms.Label()
        Me.txtTenMotP81V = New System.Windows.Forms.TextBox()
        Me.lblTestP322S = New System.Windows.Forms.Label()
        Me.lblTestThen = New System.Windows.Forms.Label()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.pnlSet.SuspendLayout()
        Me.pnlTest.SuspendLayout()
        Me.pnlDelay.SuspendLayout()
        Me.pnlProgOp.SuspendLayout()
        Me.pnlGoto.SuspendLayout()
        Me.pnlNoOpEnd.SuspendLayout()
        Me.pnlTenMot.SuspendLayout()
        Me.SuspendLayout()
        '
        'cboCommand
        '
        Me.cboCommand.FormattingEnabled = True
        Me.cboCommand.Location = New System.Drawing.Point(37, 1)
        Me.cboCommand.Name = "cboCommand"
        Me.cboCommand.Size = New System.Drawing.Size(73, 21)
        Me.cboCommand.TabIndex = 1
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
        Me.SplitContainer1.IsSplitterFixed = True
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer1.Name = "SplitContainer1"
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.cboCommand)
        Me.SplitContainer1.Panel1.Controls.Add(Me.lblLine)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.pnlSet)
        Me.SplitContainer1.Panel2.Controls.Add(Me.pnlTest)
        Me.SplitContainer1.Panel2.Controls.Add(Me.pnlDelay)
        Me.SplitContainer1.Panel2.Controls.Add(Me.pnlProgOp)
        Me.SplitContainer1.Panel2.Controls.Add(Me.pnlGoto)
        Me.SplitContainer1.Panel2.Controls.Add(Me.pnlNoOpEnd)
        Me.SplitContainer1.Panel2.Controls.Add(Me.pnlTenMot)
        Me.SplitContainer1.Size = New System.Drawing.Size(670, 250)
        Me.SplitContainer1.SplitterDistance = 110
        Me.SplitContainer1.TabIndex = 3
        '
        'lblLine
        '
        Me.lblLine.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblLine.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblLine.Location = New System.Drawing.Point(0, 1)
        Me.lblLine.Name = "lblLine"
        Me.lblLine.Size = New System.Drawing.Size(37, 21)
        Me.lblLine.TabIndex = 0
        Me.lblLine.Text = "1234"
        Me.lblLine.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'pnlSet
        '
        Me.pnlSet.Controls.Add(Me.cboSetP82S)
        Me.pnlSet.Controls.Add(Me.Label12)
        Me.pnlSet.Controls.Add(Me.cboSetP322S)
        Me.pnlSet.Controls.Add(Me.txtSetP322V)
        Me.pnlSet.Controls.Add(Me.txtSetP321V)
        Me.pnlSet.Controls.Add(Me.cboSetP321S)
        Me.pnlSet.Controls.Add(Me.txtSetP81V)
        Me.pnlSet.Controls.Add(Me.cboSetP81S)
        Me.pnlSet.Controls.Add(Me.Label11)
        Me.pnlSet.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlSet.Location = New System.Drawing.Point(0, 180)
        Me.pnlSet.Name = "pnlSet"
        Me.pnlSet.Size = New System.Drawing.Size(556, 23)
        Me.pnlSet.TabIndex = 32
        '
        'cboSetP82S
        '
        Me.cboSetP82S.FormattingEnabled = True
        Me.cboSetP82S.Location = New System.Drawing.Point(325, 0)
        Me.cboSetP82S.Name = "cboSetP82S"
        Me.cboSetP82S.Size = New System.Drawing.Size(75, 21)
        Me.cboSetP82S.TabIndex = 38
        '
        'Label12
        '
        Me.Label12.Location = New System.Drawing.Point(169, 5)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(14, 13)
        Me.Label12.TabIndex = 37
        Me.Label12.Text = "="
        Me.Label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'cboSetP322S
        '
        Me.cboSetP322S.FormattingEnabled = True
        Me.cboSetP322S.Location = New System.Drawing.Point(404, 1)
        Me.cboSetP322S.Name = "cboSetP322S"
        Me.cboSetP322S.Size = New System.Drawing.Size(75, 21)
        Me.cboSetP322S.TabIndex = 32
        '
        'txtSetP322V
        '
        Me.txtSetP322V.Location = New System.Drawing.Point(481, 2)
        Me.txtSetP322V.Name = "txtSetP322V"
        Me.txtSetP322V.Size = New System.Drawing.Size(54, 20)
        Me.txtSetP322V.TabIndex = 31
        '
        'txtSetP321V
        '
        Me.txtSetP321V.Location = New System.Drawing.Point(266, 0)
        Me.txtSetP321V.Name = "txtSetP321V"
        Me.txtSetP321V.Size = New System.Drawing.Size(54, 20)
        Me.txtSetP321V.TabIndex = 29
        '
        'cboSetP321S
        '
        Me.cboSetP321S.FormattingEnabled = True
        Me.cboSetP321S.Location = New System.Drawing.Point(189, 0)
        Me.cboSetP321S.Name = "cboSetP321S"
        Me.cboSetP321S.Size = New System.Drawing.Size(75, 21)
        Me.cboSetP321S.TabIndex = 30
        '
        'txtSetP81V
        '
        Me.txtSetP81V.Location = New System.Drawing.Point(133, 1)
        Me.txtSetP81V.MaxLength = 3
        Me.txtSetP81V.Name = "txtSetP81V"
        Me.txtSetP81V.Size = New System.Drawing.Size(30, 20)
        Me.txtSetP81V.TabIndex = 28
        Me.txtSetP81V.Text = "255"
        Me.txtSetP81V.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'cboSetP81S
        '
        Me.cboSetP81S.FormattingEnabled = True
        Me.cboSetP81S.Location = New System.Drawing.Point(56, 1)
        Me.cboSetP81S.Name = "cboSetP81S"
        Me.cboSetP81S.Size = New System.Drawing.Size(75, 21)
        Me.cboSetP81S.TabIndex = 26
        '
        'Label11
        '
        Me.Label11.Location = New System.Drawing.Point(3, 5)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(52, 13)
        Me.Label11.TabIndex = 13
        Me.Label11.Text = "Set "
        Me.Label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'pnlTest
        '
        Me.pnlTest.Controls.Add(Me.lblTestThen)
        Me.pnlTest.Controls.Add(Me.lblTestP322S)
        Me.pnlTest.Controls.Add(Me.Label8)
        Me.pnlTest.Controls.Add(Me.txtTestGotoFalse)
        Me.pnlTest.Controls.Add(Me.Label6)
        Me.pnlTest.Controls.Add(Me.txtTestGotoTrue)
        Me.pnlTest.Controls.Add(Me.cboTestP322S)
        Me.pnlTest.Controls.Add(Me.txtTestP322V)
        Me.pnlTest.Controls.Add(Me.txtTestP321V)
        Me.pnlTest.Controls.Add(Me.cboTestP321S)
        Me.pnlTest.Controls.Add(Me.txtTestP81V)
        Me.pnlTest.Controls.Add(Me.cboTestP82V)
        Me.pnlTest.Controls.Add(Me.cboTestP81S)
        Me.pnlTest.Controls.Add(Me.Label7)
        Me.pnlTest.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlTest.Location = New System.Drawing.Point(0, 136)
        Me.pnlTest.Name = "pnlTest"
        Me.pnlTest.Size = New System.Drawing.Size(556, 44)
        Me.pnlTest.TabIndex = 31
        '
        'Label8
        '
        Me.Label8.Location = New System.Drawing.Point(432, 27)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(63, 13)
        Me.Label8.TabIndex = 35
        Me.Label8.Text = "Goto False:"
        Me.Label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'txtTestGotoFalse
        '
        Me.txtTestGotoFalse.Location = New System.Drawing.Point(496, 23)
        Me.txtTestGotoFalse.Name = "txtTestGotoFalse"
        Me.txtTestGotoFalse.Size = New System.Drawing.Size(39, 20)
        Me.txtTestGotoFalse.TabIndex = 36
        '
        'Label6
        '
        Me.Label6.Location = New System.Drawing.Point(435, 5)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(60, 13)
        Me.Label6.TabIndex = 33
        Me.Label6.Text = "Goto True:"
        Me.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'txtTestGotoTrue
        '
        Me.txtTestGotoTrue.Location = New System.Drawing.Point(496, 2)
        Me.txtTestGotoTrue.Name = "txtTestGotoTrue"
        Me.txtTestGotoTrue.Size = New System.Drawing.Size(39, 20)
        Me.txtTestGotoTrue.TabIndex = 34
        '
        'cboTestP322S
        '
        Me.cboTestP322S.FormattingEnabled = True
        Me.cboTestP322S.Location = New System.Drawing.Point(274, 22)
        Me.cboTestP322S.Name = "cboTestP322S"
        Me.cboTestP322S.Size = New System.Drawing.Size(75, 21)
        Me.cboTestP322S.TabIndex = 32
        '
        'txtTestP322V
        '
        Me.txtTestP322V.Location = New System.Drawing.Point(351, 23)
        Me.txtTestP322V.Name = "txtTestP322V"
        Me.txtTestP322V.Size = New System.Drawing.Size(39, 20)
        Me.txtTestP322V.TabIndex = 31
        '
        'txtTestP321V
        '
        Me.txtTestP321V.Location = New System.Drawing.Point(351, 1)
        Me.txtTestP321V.Name = "txtTestP321V"
        Me.txtTestP321V.Size = New System.Drawing.Size(39, 20)
        Me.txtTestP321V.TabIndex = 29
        Me.txtTestP321V.Text = "32768"
        '
        'cboTestP321S
        '
        Me.cboTestP321S.FormattingEnabled = True
        Me.cboTestP321S.Location = New System.Drawing.Point(274, 1)
        Me.cboTestP321S.Name = "cboTestP321S"
        Me.cboTestP321S.Size = New System.Drawing.Size(75, 21)
        Me.cboTestP321S.TabIndex = 30
        '
        'txtTestP81V
        '
        Me.txtTestP81V.Location = New System.Drawing.Point(133, 1)
        Me.txtTestP81V.MaxLength = 3
        Me.txtTestP81V.Name = "txtTestP81V"
        Me.txtTestP81V.Size = New System.Drawing.Size(30, 20)
        Me.txtTestP81V.TabIndex = 28
        Me.txtTestP81V.Text = "255"
        Me.txtTestP81V.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'cboTestP82V
        '
        Me.cboTestP82V.FormattingEnabled = True
        Me.cboTestP82V.Location = New System.Drawing.Point(165, 1)
        Me.cboTestP82V.Name = "cboTestP82V"
        Me.cboTestP82V.Size = New System.Drawing.Size(107, 21)
        Me.cboTestP82V.TabIndex = 27
        Me.cboTestP82V.Text = "Is Not Between"
        '
        'cboTestP81S
        '
        Me.cboTestP81S.FormattingEnabled = True
        Me.cboTestP81S.Location = New System.Drawing.Point(56, 1)
        Me.cboTestP81S.Name = "cboTestP81S"
        Me.cboTestP81S.Size = New System.Drawing.Size(75, 21)
        Me.cboTestP81S.TabIndex = 26
        '
        'Label7
        '
        Me.Label7.Location = New System.Drawing.Point(32, 5)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(23, 13)
        Me.Label7.TabIndex = 13
        Me.Label7.Text = "If "
        Me.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'pnlDelay
        '
        Me.pnlDelay.Controls.Add(Me.txtDelayP321V)
        Me.pnlDelay.Controls.Add(Me.Label5)
        Me.pnlDelay.Controls.Add(Me.cboDelayP321S)
        Me.pnlDelay.Controls.Add(Me.Label4)
        Me.pnlDelay.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlDelay.Location = New System.Drawing.Point(0, 113)
        Me.pnlDelay.Name = "pnlDelay"
        Me.pnlDelay.Size = New System.Drawing.Size(556, 23)
        Me.pnlDelay.TabIndex = 30
        '
        'txtDelayP321V
        '
        Me.txtDelayP321V.Location = New System.Drawing.Point(133, 1)
        Me.txtDelayP321V.Name = "txtDelayP321V"
        Me.txtDelayP321V.Size = New System.Drawing.Size(54, 20)
        Me.txtDelayP321V.TabIndex = 24
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(187, 5)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(20, 13)
        Me.Label5.TabIndex = 25
        Me.Label5.Text = "ms"
        Me.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'cboDelayP321S
        '
        Me.cboDelayP321S.FormattingEnabled = True
        Me.cboDelayP321S.Location = New System.Drawing.Point(56, 1)
        Me.cboDelayP321S.Name = "cboDelayP321S"
        Me.cboDelayP321S.Size = New System.Drawing.Size(75, 21)
        Me.cboDelayP321S.TabIndex = 26
        '
        'Label4
        '
        Me.Label4.Location = New System.Drawing.Point(0, 5)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(55, 13)
        Me.Label4.TabIndex = 13
        Me.Label4.Text = "Delay:"
        Me.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.Label4.Visible = False
        '
        'pnlProgOp
        '
        Me.pnlProgOp.Controls.Add(Me.Label3)
        Me.pnlProgOp.Controls.Add(Me.lblProgOpLineSource)
        Me.pnlProgOp.Controls.Add(Me.cboProgOpChannel)
        Me.pnlProgOp.Controls.Add(Me.txtProgOpP321V)
        Me.pnlProgOp.Controls.Add(Me.lblChannel)
        Me.pnlProgOp.Controls.Add(Me.cboProgOpP81S)
        Me.pnlProgOp.Controls.Add(Me.cboProgOpP321S)
        Me.pnlProgOp.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlProgOp.Location = New System.Drawing.Point(0, 90)
        Me.pnlProgOp.Name = "pnlProgOp"
        Me.pnlProgOp.Size = New System.Drawing.Size(556, 23)
        Me.pnlProgOp.TabIndex = 29
        '
        'Label3
        '
        Me.Label3.Location = New System.Drawing.Point(0, 5)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(55, 13)
        Me.Label3.TabIndex = 12
        Me.Label3.Text = "ProgOp:"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblProgOpLineSource
        '
        Me.lblProgOpLineSource.Location = New System.Drawing.Point(347, 5)
        Me.lblProgOpLineSource.Name = "lblProgOpLineSource"
        Me.lblProgOpLineSource.Size = New System.Drawing.Size(57, 13)
        Me.lblProgOpLineSource.TabIndex = 16
        Me.lblProgOpLineSource.Text = "Line:"
        Me.lblProgOpLineSource.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'cboProgOpChannel
        '
        Me.cboProgOpChannel.FormattingEnabled = True
        Me.cboProgOpChannel.Location = New System.Drawing.Point(223, 1)
        Me.cboProgOpChannel.Name = "cboProgOpChannel"
        Me.cboProgOpChannel.Size = New System.Drawing.Size(107, 21)
        Me.cboProgOpChannel.TabIndex = 6
        '
        'txtProgOpP321V
        '
        Me.txtProgOpP321V.Location = New System.Drawing.Point(481, 1)
        Me.txtProgOpP321V.Name = "txtProgOpP321V"
        Me.txtProgOpP321V.Size = New System.Drawing.Size(54, 20)
        Me.txtProgOpP321V.TabIndex = 17
        '
        'lblChannel
        '
        Me.lblChannel.Location = New System.Drawing.Point(171, 5)
        Me.lblChannel.Name = "lblChannel"
        Me.lblChannel.Size = New System.Drawing.Size(50, 13)
        Me.lblChannel.TabIndex = 7
        Me.lblChannel.Text = "Channel:"
        Me.lblChannel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'cboProgOpP81S
        '
        Me.cboProgOpP81S.FormattingEnabled = True
        Me.cboProgOpP81S.Location = New System.Drawing.Point(56, 1)
        Me.cboProgOpP81S.Name = "cboProgOpP81S"
        Me.cboProgOpP81S.Size = New System.Drawing.Size(113, 21)
        Me.cboProgOpP81S.TabIndex = 24
        Me.cboProgOpP81S.Text = "Pause Program"
        '
        'cboProgOpP321S
        '
        Me.cboProgOpP321S.FormattingEnabled = True
        Me.cboProgOpP321S.Location = New System.Drawing.Point(404, 1)
        Me.cboProgOpP321S.Name = "cboProgOpP321S"
        Me.cboProgOpP321S.Size = New System.Drawing.Size(75, 21)
        Me.cboProgOpP321S.TabIndex = 23
        '
        'pnlGoto
        '
        Me.pnlGoto.Controls.Add(Me.Label2)
        Me.pnlGoto.Controls.Add(Me.txtGotoGotoTrue)
        Me.pnlGoto.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlGoto.Location = New System.Drawing.Point(0, 67)
        Me.pnlGoto.Name = "pnlGoto"
        Me.pnlGoto.Size = New System.Drawing.Size(556, 23)
        Me.pnlGoto.TabIndex = 28
        '
        'Label2
        '
        Me.Label2.Location = New System.Drawing.Point(0, 5)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(55, 13)
        Me.Label2.TabIndex = 13
        Me.Label2.Text = "Line:"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'txtGotoGotoTrue
        '
        Me.txtGotoGotoTrue.Location = New System.Drawing.Point(56, 1)
        Me.txtGotoGotoTrue.Name = "txtGotoGotoTrue"
        Me.txtGotoGotoTrue.Size = New System.Drawing.Size(54, 20)
        Me.txtGotoGotoTrue.TabIndex = 19
        '
        'pnlNoOpEnd
        '
        Me.pnlNoOpEnd.Controls.Add(Me.Label1)
        Me.pnlNoOpEnd.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlNoOpEnd.Location = New System.Drawing.Point(0, 44)
        Me.pnlNoOpEnd.Name = "pnlNoOpEnd"
        Me.pnlNoOpEnd.Size = New System.Drawing.Size(556, 23)
        Me.pnlNoOpEnd.TabIndex = 27
        '
        'Label1
        '
        Me.Label1.Location = New System.Drawing.Point(0, 5)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(84, 13)
        Me.Label1.TabIndex = 13
        Me.Label1.Text = "(NoOp / End)"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.Label1.Visible = False
        '
        'pnlTenMot
        '
        Me.pnlTenMot.Controls.Add(Me.lblPolarity)
        Me.pnlTenMot.Controls.Add(Me.lblP321S)
        Me.pnlTenMot.Controls.Add(Me.lblP322VUnits)
        Me.pnlTenMot.Controls.Add(Me.txtTenMotP82V)
        Me.pnlTenMot.Controls.Add(Me.txtTenMotP321V)
        Me.pnlTenMot.Controls.Add(Me.cboTenMotP322S)
        Me.pnlTenMot.Controls.Add(Me.lblP82S)
        Me.pnlTenMot.Controls.Add(Me.lblP321VUnits)
        Me.pnlTenMot.Controls.Add(Me.cboTenMotP321S)
        Me.pnlTenMot.Controls.Add(Me.cboTenMotP82S)
        Me.pnlTenMot.Controls.Add(Me.cboTenMotP81S)
        Me.pnlTenMot.Controls.Add(Me.lblP322S)
        Me.pnlTenMot.Controls.Add(Me.lblP82VUnits)
        Me.pnlTenMot.Controls.Add(Me.cboTenMotPolarity)
        Me.pnlTenMot.Controls.Add(Me.lblP81S)
        Me.pnlTenMot.Controls.Add(Me.txtTenMotP322V)
        Me.pnlTenMot.Controls.Add(Me.lblP81VUnits)
        Me.pnlTenMot.Controls.Add(Me.txtTenMotP81V)
        Me.pnlTenMot.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlTenMot.Location = New System.Drawing.Point(0, 0)
        Me.pnlTenMot.Name = "pnlTenMot"
        Me.pnlTenMot.Size = New System.Drawing.Size(556, 44)
        Me.pnlTenMot.TabIndex = 26
        '
        'lblPolarity
        '
        Me.lblPolarity.Location = New System.Drawing.Point(0, 5)
        Me.lblPolarity.Name = "lblPolarity"
        Me.lblPolarity.Size = New System.Drawing.Size(55, 13)
        Me.lblPolarity.TabIndex = 12
        Me.lblPolarity.Text = "Polarity:"
        Me.lblPolarity.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblP321S
        '
        Me.lblP321S.Location = New System.Drawing.Point(347, 5)
        Me.lblP321S.Name = "lblP321S"
        Me.lblP321S.Size = New System.Drawing.Size(57, 13)
        Me.lblP321S.TabIndex = 16
        Me.lblP321S.Text = "Duration:"
        Me.lblP321S.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblP322VUnits
        '
        Me.lblP322VUnits.AutoSize = True
        Me.lblP322VUnits.Location = New System.Drawing.Point(535, 25)
        Me.lblP322VUnits.Name = "lblP322VUnits"
        Me.lblP322VUnits.Size = New System.Drawing.Size(20, 13)
        Me.lblP322VUnits.TabIndex = 25
        Me.lblP322VUnits.Text = "ms"
        Me.lblP322VUnits.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'txtTenMotP82V
        '
        Me.txtTenMotP82V.Location = New System.Drawing.Point(300, 22)
        Me.txtTenMotP82V.Name = "txtTenMotP82V"
        Me.txtTenMotP82V.Size = New System.Drawing.Size(30, 20)
        Me.txtTenMotP82V.TabIndex = 15
        Me.txtTenMotP82V.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'txtTenMotP321V
        '
        Me.txtTenMotP321V.Location = New System.Drawing.Point(481, 1)
        Me.txtTenMotP321V.Name = "txtTenMotP321V"
        Me.txtTenMotP321V.Size = New System.Drawing.Size(54, 20)
        Me.txtTenMotP321V.TabIndex = 17
        '
        'cboTenMotP322S
        '
        Me.cboTenMotP322S.FormattingEnabled = True
        Me.cboTenMotP322S.Location = New System.Drawing.Point(404, 21)
        Me.cboTenMotP322S.Name = "cboTenMotP322S"
        Me.cboTenMotP322S.Size = New System.Drawing.Size(75, 21)
        Me.cboTenMotP322S.TabIndex = 24
        '
        'lblP82S
        '
        Me.lblP82S.Location = New System.Drawing.Point(171, 26)
        Me.lblP82S.Name = "lblP82S"
        Me.lblP82S.Size = New System.Drawing.Size(50, 13)
        Me.lblP82S.TabIndex = 14
        Me.lblP82S.Text = "End:"
        Me.lblP82S.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblP321VUnits
        '
        Me.lblP321VUnits.AutoSize = True
        Me.lblP321VUnits.Location = New System.Drawing.Point(535, 5)
        Me.lblP321VUnits.Name = "lblP321VUnits"
        Me.lblP321VUnits.Size = New System.Drawing.Size(20, 13)
        Me.lblP321VUnits.TabIndex = 18
        Me.lblP321VUnits.Text = "ms"
        Me.lblP321VUnits.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'cboTenMotP321S
        '
        Me.cboTenMotP321S.FormattingEnabled = True
        Me.cboTenMotP321S.Location = New System.Drawing.Point(404, 1)
        Me.cboTenMotP321S.Name = "cboTenMotP321S"
        Me.cboTenMotP321S.Size = New System.Drawing.Size(75, 21)
        Me.cboTenMotP321S.TabIndex = 23
        '
        'cboTenMotP82S
        '
        Me.cboTenMotP82S.BackColor = System.Drawing.SystemColors.ControlLightLight
        Me.cboTenMotP82S.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboTenMotP82S.FormattingEnabled = True
        Me.cboTenMotP82S.Location = New System.Drawing.Point(223, 21)
        Me.cboTenMotP82S.Name = "cboTenMotP82S"
        Me.cboTenMotP82S.Size = New System.Drawing.Size(75, 21)
        Me.cboTenMotP82S.TabIndex = 13
        '
        'cboTenMotP81S
        '
        Me.cboTenMotP81S.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple
        Me.cboTenMotP81S.FormattingEnabled = True
        Me.cboTenMotP81S.Location = New System.Drawing.Point(223, 1)
        Me.cboTenMotP81S.Name = "cboTenMotP81S"
        Me.cboTenMotP81S.Size = New System.Drawing.Size(75, 21)
        Me.cboTenMotP81S.TabIndex = 8
        Me.cboTenMotP81S.Text = "Sys Variable"
        '
        'lblP322S
        '
        Me.lblP322S.Location = New System.Drawing.Point(347, 25)
        Me.lblP322S.Name = "lblP322S"
        Me.lblP322S.Size = New System.Drawing.Size(57, 13)
        Me.lblP322S.TabIndex = 19
        Me.lblP322S.Text = "Repeats:"
        Me.lblP322S.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblP82VUnits
        '
        Me.lblP82VUnits.AutoSize = True
        Me.lblP82VUnits.Location = New System.Drawing.Point(330, 26)
        Me.lblP82VUnits.Name = "lblP82VUnits"
        Me.lblP82VUnits.Size = New System.Drawing.Size(15, 13)
        Me.lblP82VUnits.TabIndex = 22
        Me.lblP82VUnits.Text = "%"
        '
        'cboTenMotPolarity
        '
        Me.cboTenMotPolarity.FormattingEnabled = True
        Me.cboTenMotPolarity.Location = New System.Drawing.Point(56, 1)
        Me.cboTenMotPolarity.Name = "cboTenMotPolarity"
        Me.cboTenMotPolarity.Size = New System.Drawing.Size(113, 21)
        Me.cboTenMotPolarity.TabIndex = 11
        Me.cboTenMotPolarity.Text = "Rev -Toggle Cycle"
        '
        'lblP81S
        '
        Me.lblP81S.Location = New System.Drawing.Point(171, 5)
        Me.lblP81S.Name = "lblP81S"
        Me.lblP81S.Size = New System.Drawing.Size(50, 13)
        Me.lblP81S.TabIndex = 9
        Me.lblP81S.Text = "Channel:"
        Me.lblP81S.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'txtTenMotP322V
        '
        Me.txtTenMotP322V.Location = New System.Drawing.Point(481, 22)
        Me.txtTenMotP322V.Name = "txtTenMotP322V"
        Me.txtTenMotP322V.Size = New System.Drawing.Size(54, 20)
        Me.txtTenMotP322V.TabIndex = 20
        '
        'lblP81VUnits
        '
        Me.lblP81VUnits.AutoSize = True
        Me.lblP81VUnits.Location = New System.Drawing.Point(330, 5)
        Me.lblP81VUnits.Name = "lblP81VUnits"
        Me.lblP81VUnits.Size = New System.Drawing.Size(15, 13)
        Me.lblP81VUnits.TabIndex = 21
        Me.lblP81VUnits.Text = "%"
        '
        'txtTenMotP81V
        '
        Me.txtTenMotP81V.Location = New System.Drawing.Point(300, 1)
        Me.txtTenMotP81V.MaxLength = 3
        Me.txtTenMotP81V.Name = "txtTenMotP81V"
        Me.txtTenMotP81V.Size = New System.Drawing.Size(30, 20)
        Me.txtTenMotP81V.TabIndex = 10
        Me.txtTenMotP81V.Text = "255"
        Me.txtTenMotP81V.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'lblTestP322S
        '
        Me.lblTestP322S.Location = New System.Drawing.Point(240, 26)
        Me.lblTestP322S.Name = "lblTestP322S"
        Me.lblTestP322S.Size = New System.Drawing.Size(28, 13)
        Me.lblTestP322S.TabIndex = 38
        Me.lblTestP322S.Text = "And"
        Me.lblTestP322S.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblTestThen
        '
        Me.lblTestThen.Location = New System.Drawing.Point(396, 14)
        Me.lblTestThen.Name = "lblTestThen"
        Me.lblTestThen.Size = New System.Drawing.Size(35, 13)
        Me.lblTestThen.TabIndex = 39
        Me.lblTestThen.Text = "Then:"
        Me.lblTestThen.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'UserControlProgLine2
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.SplitContainer1)
        Me.Name = "UserControlProgLine2"
        Me.Size = New System.Drawing.Size(670, 250)
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        Me.pnlSet.ResumeLayout(False)
        Me.pnlSet.PerformLayout()
        Me.pnlTest.ResumeLayout(False)
        Me.pnlTest.PerformLayout()
        Me.pnlDelay.ResumeLayout(False)
        Me.pnlDelay.PerformLayout()
        Me.pnlProgOp.ResumeLayout(False)
        Me.pnlProgOp.PerformLayout()
        Me.pnlGoto.ResumeLayout(False)
        Me.pnlGoto.PerformLayout()
        Me.pnlNoOpEnd.ResumeLayout(False)
        Me.pnlTenMot.ResumeLayout(False)
        Me.pnlTenMot.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents cboCommand As ComboBox
    Friend WithEvents SplitContainer1 As SplitContainer
    Friend WithEvents lblLine As Label
    Friend WithEvents txtGotoGotoTrue As TextBox
    Friend WithEvents lblP322VUnits As Label
    Friend WithEvents cboProgOpChannel As ComboBox
    Friend WithEvents cboTenMotP322S As ComboBox
    Friend WithEvents lblChannel As Label
    Friend WithEvents cboTenMotP321S As ComboBox
    Friend WithEvents cboTenMotP81S As ComboBox
    Friend WithEvents lblP82VUnits As Label
    Friend WithEvents lblP81S As Label
    Friend WithEvents lblP81VUnits As Label
    Friend WithEvents txtTenMotP81V As TextBox
    Friend WithEvents txtTenMotP322V As TextBox
    Friend WithEvents cboTenMotPolarity As ComboBox
    Friend WithEvents cboProgOpP81S As ComboBox
    Friend WithEvents lblPolarity As Label
    Friend WithEvents lblP322S As Label
    Friend WithEvents cboTenMotP82S As ComboBox
    Friend WithEvents lblP321VUnits As Label
    Friend WithEvents lblP82S As Label
    Friend WithEvents txtTenMotP321V As TextBox
    Friend WithEvents txtTenMotP82V As TextBox
    Friend WithEvents lblP321S As Label
    Friend WithEvents pnlNoOpEnd As Panel
    Friend WithEvents Label1 As Label
    Friend WithEvents pnlTenMot As Panel
    Friend WithEvents pnlGoto As Panel
    Friend WithEvents Label2 As Label
    Friend WithEvents pnlProgOp As Panel
    Friend WithEvents Label3 As Label
    Friend WithEvents lblProgOpLineSource As Label
    Friend WithEvents txtProgOpP321V As TextBox
    Friend WithEvents cboProgOpP321S As ComboBox
    Friend WithEvents pnlDelay As Panel
    Friend WithEvents Label4 As Label
    Friend WithEvents txtDelayP321V As TextBox
    Friend WithEvents Label5 As Label
    Friend WithEvents cboDelayP321S As ComboBox
    Friend WithEvents pnlTest As Panel
    Friend WithEvents cboTestP81S As ComboBox
    Friend WithEvents Label7 As Label
    Friend WithEvents txtTestP81V As TextBox
    Friend WithEvents cboTestP82V As ComboBox
    Friend WithEvents txtTestP321V As TextBox
    Friend WithEvents cboTestP321S As ComboBox
    Friend WithEvents Label8 As Label
    Friend WithEvents txtTestGotoFalse As TextBox
    Friend WithEvents Label6 As Label
    Friend WithEvents txtTestGotoTrue As TextBox
    Friend WithEvents cboTestP322S As ComboBox
    Friend WithEvents txtTestP322V As TextBox
    Friend WithEvents pnlSet As Panel
    Friend WithEvents cboSetP322S As ComboBox
    Friend WithEvents txtSetP322V As TextBox
    Friend WithEvents txtSetP321V As TextBox
    Friend WithEvents cboSetP321S As ComboBox
    Friend WithEvents txtSetP81V As TextBox
    Friend WithEvents cboSetP81S As ComboBox
    Friend WithEvents Label11 As Label
    Friend WithEvents Label12 As Label
    Friend WithEvents cboSetP82S As ComboBox
    Friend WithEvents lblTestP322S As Label
    Friend WithEvents lblTestThen As Label
End Class
