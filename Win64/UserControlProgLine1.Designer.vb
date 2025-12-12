<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class UserControlProgLine1
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
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
        Me.txtGotoTrue = New System.Windows.Forms.TextBox()
        Me.lblP322VUnits = New System.Windows.Forms.Label()
        Me.cboP322S = New System.Windows.Forms.ComboBox()
        Me.cboP321S = New System.Windows.Forms.ComboBox()
        Me.lblP82VUnits = New System.Windows.Forms.Label()
        Me.lblP81VUnits = New System.Windows.Forms.Label()
        Me.txtP322V = New System.Windows.Forms.TextBox()
        Me.cboOpType = New System.Windows.Forms.ComboBox()
        Me.lblP322S = New System.Windows.Forms.Label()
        Me.lblP321VUnits = New System.Windows.Forms.Label()
        Me.txtP321V = New System.Windows.Forms.TextBox()
        Me.lblP321S = New System.Windows.Forms.Label()
        Me.txtP82V = New System.Windows.Forms.TextBox()
        Me.lblP82S = New System.Windows.Forms.Label()
        Me.cboP82S = New System.Windows.Forms.ComboBox()
        Me.lblPolarity = New System.Windows.Forms.Label()
        Me.cboPolarity = New System.Windows.Forms.ComboBox()
        Me.txtP81V = New System.Windows.Forms.TextBox()
        Me.lblP81S = New System.Windows.Forms.Label()
        Me.cboP81S = New System.Windows.Forms.ComboBox()
        Me.lblChannel = New System.Windows.Forms.Label()
        Me.cboChannel = New System.Windows.Forms.ComboBox()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.cboCommand = New System.Windows.Forms.ComboBox()
        Me.lblLine = New System.Windows.Forms.Label()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.SuspendLayout()
        '
        'txtGotoTrue
        '
        Me.txtGotoTrue.Location = New System.Drawing.Point(11, 51)
        Me.txtGotoTrue.Name = "txtGotoTrue"
        Me.txtGotoTrue.Size = New System.Drawing.Size(54, 20)
        Me.txtGotoTrue.TabIndex = 19
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
        'cboP322S
        '
        Me.cboP322S.FormattingEnabled = True
        Me.cboP322S.Location = New System.Drawing.Point(404, 20)
        Me.cboP322S.Name = "cboP322S"
        Me.cboP322S.Size = New System.Drawing.Size(75, 21)
        Me.cboP322S.TabIndex = 24
        '
        'cboP321S
        '
        Me.cboP321S.FormattingEnabled = True
        Me.cboP321S.Location = New System.Drawing.Point(404, 0)
        Me.cboP321S.Name = "cboP321S"
        Me.cboP321S.Size = New System.Drawing.Size(75, 21)
        Me.cboP321S.TabIndex = 23
        '
        'lblP82VUnits
        '
        Me.lblP82VUnits.AutoSize = True
        Me.lblP82VUnits.Location = New System.Drawing.Point(330, 25)
        Me.lblP82VUnits.Name = "lblP82VUnits"
        Me.lblP82VUnits.Size = New System.Drawing.Size(15, 13)
        Me.lblP82VUnits.TabIndex = 22
        Me.lblP82VUnits.Text = "%"
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
        'txtP322V
        '
        Me.txtP322V.Location = New System.Drawing.Point(481, 21)
        Me.txtP322V.Name = "txtP322V"
        Me.txtP322V.Size = New System.Drawing.Size(54, 20)
        Me.txtP322V.TabIndex = 20
        '
        'cboOpType
        '
        Me.cboOpType.FormattingEnabled = True
        Me.cboOpType.Location = New System.Drawing.Point(40, 44)
        Me.cboOpType.Name = "cboOpType"
        Me.cboOpType.Size = New System.Drawing.Size(110, 21)
        Me.cboOpType.TabIndex = 24
        Me.cboOpType.Text = "Pause Program"
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
        'txtP321V
        '
        Me.txtP321V.Location = New System.Drawing.Point(481, 0)
        Me.txtP321V.Name = "txtP321V"
        Me.txtP321V.Size = New System.Drawing.Size(54, 20)
        Me.txtP321V.TabIndex = 17
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
        'txtP82V
        '
        Me.txtP82V.Location = New System.Drawing.Point(300, 21)
        Me.txtP82V.Name = "txtP82V"
        Me.txtP82V.Size = New System.Drawing.Size(30, 20)
        Me.txtP82V.TabIndex = 15
        Me.txtP82V.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'lblP82S
        '
        Me.lblP82S.Location = New System.Drawing.Point(171, 25)
        Me.lblP82S.Name = "lblP82S"
        Me.lblP82S.Size = New System.Drawing.Size(50, 13)
        Me.lblP82S.TabIndex = 14
        Me.lblP82S.Text = "End:"
        Me.lblP82S.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'cboP82S
        '
        Me.cboP82S.BackColor = System.Drawing.SystemColors.ControlLightLight
        Me.cboP82S.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboP82S.FormattingEnabled = True
        Me.cboP82S.Location = New System.Drawing.Point(223, 20)
        Me.cboP82S.Name = "cboP82S"
        Me.cboP82S.Size = New System.Drawing.Size(75, 21)
        Me.cboP82S.TabIndex = 13
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
        'cboPolarity
        '
        Me.cboPolarity.FormattingEnabled = True
        Me.cboPolarity.Location = New System.Drawing.Point(56, 0)
        Me.cboPolarity.Name = "cboPolarity"
        Me.cboPolarity.Size = New System.Drawing.Size(113, 21)
        Me.cboPolarity.TabIndex = 11
        Me.cboPolarity.Text = "Rev -Toggle Cycle"
        '
        'txtP81V
        '
        Me.txtP81V.Location = New System.Drawing.Point(300, 0)
        Me.txtP81V.MaxLength = 3
        Me.txtP81V.Name = "txtP81V"
        Me.txtP81V.Size = New System.Drawing.Size(30, 20)
        Me.txtP81V.TabIndex = 10
        Me.txtP81V.Text = "255"
        Me.txtP81V.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
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
        'cboP81S
        '
        Me.cboP81S.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple
        Me.cboP81S.FormattingEnabled = True
        Me.cboP81S.Location = New System.Drawing.Point(223, 0)
        Me.cboP81S.Name = "cboP81S"
        Me.cboP81S.Size = New System.Drawing.Size(75, 21)
        Me.cboP81S.TabIndex = 8
        Me.cboP81S.Text = "Sys Variable"
        '
        'lblChannel
        '
        Me.lblChannel.Location = New System.Drawing.Point(167, 52)
        Me.lblChannel.Name = "lblChannel"
        Me.lblChannel.Size = New System.Drawing.Size(55, 13)
        Me.lblChannel.TabIndex = 7
        Me.lblChannel.Text = "Channel:"
        Me.lblChannel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'cboChannel
        '
        Me.cboChannel.FormattingEnabled = True
        Me.cboChannel.Location = New System.Drawing.Point(223, 47)
        Me.cboChannel.Name = "cboChannel"
        Me.cboChannel.Size = New System.Drawing.Size(107, 21)
        Me.cboChannel.TabIndex = 6
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
        Me.SplitContainer1.Panel2.Controls.Add(Me.txtGotoTrue)
        Me.SplitContainer1.Panel2.Controls.Add(Me.lblP322VUnits)
        Me.SplitContainer1.Panel2.Controls.Add(Me.cboChannel)
        Me.SplitContainer1.Panel2.Controls.Add(Me.cboP322S)
        Me.SplitContainer1.Panel2.Controls.Add(Me.lblChannel)
        Me.SplitContainer1.Panel2.Controls.Add(Me.cboP321S)
        Me.SplitContainer1.Panel2.Controls.Add(Me.cboP81S)
        Me.SplitContainer1.Panel2.Controls.Add(Me.lblP82VUnits)
        Me.SplitContainer1.Panel2.Controls.Add(Me.lblP81S)
        Me.SplitContainer1.Panel2.Controls.Add(Me.lblP81VUnits)
        Me.SplitContainer1.Panel2.Controls.Add(Me.txtP81V)
        Me.SplitContainer1.Panel2.Controls.Add(Me.txtP322V)
        Me.SplitContainer1.Panel2.Controls.Add(Me.cboPolarity)
        Me.SplitContainer1.Panel2.Controls.Add(Me.cboOpType)
        Me.SplitContainer1.Panel2.Controls.Add(Me.lblPolarity)
        Me.SplitContainer1.Panel2.Controls.Add(Me.lblP322S)
        Me.SplitContainer1.Panel2.Controls.Add(Me.cboP82S)
        Me.SplitContainer1.Panel2.Controls.Add(Me.lblP321VUnits)
        Me.SplitContainer1.Panel2.Controls.Add(Me.lblP82S)
        Me.SplitContainer1.Panel2.Controls.Add(Me.txtP321V)
        Me.SplitContainer1.Panel2.Controls.Add(Me.txtP82V)
        Me.SplitContainer1.Panel2.Controls.Add(Me.lblP321S)
        Me.SplitContainer1.Size = New System.Drawing.Size(666, 246)
        Me.SplitContainer1.SplitterDistance = 110
        Me.SplitContainer1.TabIndex = 2
        '
        'cboCommand
        '
        Me.cboCommand.FormattingEnabled = True
        Me.cboCommand.Location = New System.Drawing.Point(37, 0)
        Me.cboCommand.Name = "cboCommand"
        Me.cboCommand.Size = New System.Drawing.Size(73, 21)
        Me.cboCommand.TabIndex = 1
        '
        'lblLine
        '
        Me.lblLine.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblLine.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblLine.Location = New System.Drawing.Point(0, 0)
        Me.lblLine.Name = "lblLine"
        Me.lblLine.Size = New System.Drawing.Size(37, 21)
        Me.lblLine.TabIndex = 0
        Me.lblLine.Text = "1234"
        Me.lblLine.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'UserControlProgLine
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.AutoSize = True
        Me.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Controls.Add(Me.SplitContainer1)
        Me.MaximumSize = New System.Drawing.Size(670, 250)
        Me.MinimumSize = New System.Drawing.Size(670, 21)
        Me.Name = "UserControlProgLine"
        Me.Size = New System.Drawing.Size(670, 246)
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        Me.SplitContainer1.Panel2.PerformLayout()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents SplitContainer1 As SplitContainer
    Friend WithEvents lblLine As Label
    Friend WithEvents cboCommand As ComboBox
    Friend WithEvents lblChannel As Label
    Friend WithEvents cboChannel As ComboBox
    Friend WithEvents lblPolarity As Label
    Friend WithEvents cboPolarity As ComboBox
    Friend WithEvents txtP81V As TextBox
    Friend WithEvents lblP81S As Label
    Friend WithEvents cboP81S As ComboBox
    Friend WithEvents lblP321VUnits As Label
    Friend WithEvents txtP321V As TextBox
    Friend WithEvents lblP321S As Label
    Friend WithEvents txtP82V As TextBox
    Friend WithEvents lblP82S As Label
    Friend WithEvents cboP82S As ComboBox
    Friend WithEvents lblP82VUnits As Label
    Friend WithEvents lblP81VUnits As Label
    Friend WithEvents txtP322V As TextBox
    Friend WithEvents lblP322S As Label
    Friend WithEvents cboP322S As ComboBox
    Friend WithEvents cboP321S As ComboBox
    Friend WithEvents txtGotoTrue As TextBox
    Friend WithEvents cboOpType As ComboBox
    Friend WithEvents lblP322VUnits As Label
End Class
