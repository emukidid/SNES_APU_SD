<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> Partial Class frmToP
#Region "Windows Form Designer generated code "
	<System.Diagnostics.DebuggerNonUserCode()> Public Sub New()
		MyBase.New()
		'This call is required by the Windows Form Designer.
		InitializeComponent()
	End Sub
	'Form overrides dispose to clean up the component list.
	<System.Diagnostics.DebuggerNonUserCode()> Protected Overloads Overrides Sub Dispose(ByVal Disposing As Boolean)
		If Disposing Then
			If Not components Is Nothing Then
				components.Dispose()
			End If
		End If
		MyBase.Dispose(Disposing)
	End Sub
	'Required by the Windows Form Designer
	Private components As System.ComponentModel.IContainer
	Public ToolTip1 As System.Windows.Forms.ToolTip
	Public WithEvents cboGEDSP As System.Windows.Forms.ComboBox
	Public WithEvents _optTACT_10 As System.Windows.Forms.RadioButton
	Public WithEvents _optTACT_9 As System.Windows.Forms.RadioButton
	Public WithEvents _optTACT_8 As System.Windows.Forms.RadioButton
	Public WithEvents _optTACT_7 As System.Windows.Forms.RadioButton
	Public WithEvents _optTACT_6 As System.Windows.Forms.RadioButton
	Public WithEvents _optTACT_5 As System.Windows.Forms.RadioButton
	Public WithEvents _optTACT_4 As System.Windows.Forms.RadioButton
	Public WithEvents _optTACT_3 As System.Windows.Forms.RadioButton
	Public WithEvents _optTACT_2 As System.Windows.Forms.RadioButton
	Public WithEvents _optTACT_1 As System.Windows.Forms.RadioButton
	Public WithEvents _optTACT_0 As System.Windows.Forms.RadioButton
	Public WithEvents Frame3 As System.Windows.Forms.GroupBox
	Public WithEvents Timer1 As System.Windows.Forms.Timer
	Public WithEvents Command1 As System.Windows.Forms.Button
	Public WithEvents Stop_Renamed As System.Windows.Forms.Button
	Public WithEvents cmdPlay As System.Windows.Forms.Button
	Public WithEvents cmdPause As System.Windows.Forms.Button
    'Public WithEvents sldPan As Axmscomctl.AxSlider
	Public WithEvents cmdFastForward As System.Windows.Forms.Button
	Public WithEvents optPanDelay As System.Windows.Forms.RadioButton
	Public WithEvents optDry As System.Windows.Forms.RadioButton
	Public WithEvents optReverb As System.Windows.Forms.RadioButton
	Public WithEvents Frame2 As System.Windows.Forms.GroupBox
	Public WithEvents _optOutput_2 As System.Windows.Forms.RadioButton
	Public WithEvents _optOutput_1 As System.Windows.Forms.RadioButton
	Public WithEvents _optOutput_0 As System.Windows.Forms.RadioButton
	Public WithEvents Frame1 As System.Windows.Forms.GroupBox
    'Public WithEvents _SldGraficEqualizer_0 As Axmscomctl.AxSlider
	Public WithEvents vsVOL As System.Windows.Forms.VScrollBar
    'Public WithEvents _SldGraficEqualizer_1 As Axmscomctl.AxSlider
    'Public WithEvents _SldGraficEqualizer_2 As Axmscomctl.AxSlider
    'Public WithEvents _SldGraficEqualizer_3 As Axmscomctl.AxSlider
    'Public WithEvents _SldGraficEqualizer_4 As Axmscomctl.AxSlider
    'Public WithEvents _SldGraficEqualizer_5 As Axmscomctl.AxSlider
    'Public WithEvents _SldGraficEqualizer_6 As Axmscomctl.AxSlider
    'Public WithEvents _SldGraficEqualizer_7 As Axmscomctl.AxSlider
    'Public WithEvents _SldGraficEqualizer_8 As Axmscomctl.AxSlider
    'Public WithEvents sldTrans As Axmscomctl.AxSlider
    'Public WithEvents sldPitch As Axmscomctl.AxSlider
	Public WithEvents lblPattern As System.Windows.Forms.Label
	Public WithEvents lblLoop As System.Windows.Forms.Label
	Public WithEvents Label6 As System.Windows.Forms.Label
	Public WithEvents Label5 As System.Windows.Forms.Label
	Public WithEvents Label4 As System.Windows.Forms.Label
	Public WithEvents Label3 As System.Windows.Forms.Label
	Public WithEvents Label2 As System.Windows.Forms.Label
	Public WithEvents Label1 As System.Windows.Forms.Label
	Public WithEvents lblVOL As System.Windows.Forms.Label
    'Public WithEvents SldGraficEqualizer As AxSliderArray
	Public WithEvents optOutput As Microsoft.VisualBasic.Compatibility.VB6.RadioButtonArray
	Public WithEvents optTACT As Microsoft.VisualBasic.Compatibility.VB6.RadioButtonArray
	'NOTE: The following procedure is required by the Windows Form Designer
	'It can be modified using the Windows Form Designer.
	'Do not modify it using the code editor.
	<System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.cboGEDSP = New System.Windows.Forms.ComboBox
        Me.Frame3 = New System.Windows.Forms.GroupBox
        Me._optTACT_10 = New System.Windows.Forms.RadioButton
        Me._optTACT_9 = New System.Windows.Forms.RadioButton
        Me._optTACT_8 = New System.Windows.Forms.RadioButton
        Me._optTACT_7 = New System.Windows.Forms.RadioButton
        Me._optTACT_6 = New System.Windows.Forms.RadioButton
        Me._optTACT_5 = New System.Windows.Forms.RadioButton
        Me._optTACT_4 = New System.Windows.Forms.RadioButton
        Me._optTACT_3 = New System.Windows.Forms.RadioButton
        Me._optTACT_2 = New System.Windows.Forms.RadioButton
        Me._optTACT_1 = New System.Windows.Forms.RadioButton
        Me._optTACT_0 = New System.Windows.Forms.RadioButton
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.Command1 = New System.Windows.Forms.Button
        Me.Stop_Renamed = New System.Windows.Forms.Button
        Me.cmdPlay = New System.Windows.Forms.Button
        Me.cmdPause = New System.Windows.Forms.Button
        Me.cmdFastForward = New System.Windows.Forms.Button
        Me.Frame2 = New System.Windows.Forms.GroupBox
        Me.optPanDelay = New System.Windows.Forms.RadioButton
        Me.optDry = New System.Windows.Forms.RadioButton
        Me.optReverb = New System.Windows.Forms.RadioButton
        Me.Frame1 = New System.Windows.Forms.GroupBox
        Me._optOutput_2 = New System.Windows.Forms.RadioButton
        Me._optOutput_1 = New System.Windows.Forms.RadioButton
        Me._optOutput_0 = New System.Windows.Forms.RadioButton
        Me.vsVOL = New System.Windows.Forms.VScrollBar
        Me.lblPattern = New System.Windows.Forms.Label
        Me.lblLoop = New System.Windows.Forms.Label
        Me.Label6 = New System.Windows.Forms.Label
        Me.Label5 = New System.Windows.Forms.Label
        Me.Label4 = New System.Windows.Forms.Label
        Me.Label3 = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.Label1 = New System.Windows.Forms.Label
        Me.lblVOL = New System.Windows.Forms.Label
        Me.optOutput = New Microsoft.VisualBasic.Compatibility.VB6.RadioButtonArray(Me.components)
        Me.optTACT = New Microsoft.VisualBasic.Compatibility.VB6.RadioButtonArray(Me.components)
        Me._SldGraficEqualizer_0 = New System.Windows.Forms.TrackBar
        Me._SldGraficEqualizer_1 = New System.Windows.Forms.TrackBar
        Me._SldGraficEqualizer_2 = New System.Windows.Forms.TrackBar
        Me._SldGraficEqualizer_3 = New System.Windows.Forms.TrackBar
        Me._SldGraficEqualizer_4 = New System.Windows.Forms.TrackBar
        Me._SldGraficEqualizer_5 = New System.Windows.Forms.TrackBar
        Me._SldGraficEqualizer_6 = New System.Windows.Forms.TrackBar
        Me._SldGraficEqualizer_7 = New System.Windows.Forms.TrackBar
        Me._SldGraficEqualizer_8 = New System.Windows.Forms.TrackBar
        Me.sldPan = New System.Windows.Forms.TrackBar
        Me.sldTrans = New System.Windows.Forms.TrackBar
        Me.sldPitch = New System.Windows.Forms.TrackBar
        Me.Frame3.SuspendLayout()
        Me.Frame2.SuspendLayout()
        Me.Frame1.SuspendLayout()
        CType(Me.optOutput, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.optTACT, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me._SldGraficEqualizer_0, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me._SldGraficEqualizer_1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me._SldGraficEqualizer_2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me._SldGraficEqualizer_3, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me._SldGraficEqualizer_4, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me._SldGraficEqualizer_5, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me._SldGraficEqualizer_6, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me._SldGraficEqualizer_7, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me._SldGraficEqualizer_8, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.sldPan, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.sldTrans, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.sldPitch, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'cboGEDSP
        '
        Me.cboGEDSP.BackColor = System.Drawing.SystemColors.Window
        Me.cboGEDSP.Cursor = System.Windows.Forms.Cursors.Default
        Me.cboGEDSP.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cboGEDSP.ForeColor = System.Drawing.SystemColors.WindowText
        Me.cboGEDSP.Items.AddRange(New Object() {"Flat", "Bass Boost", "Arena"})
        Me.cboGEDSP.Location = New System.Drawing.Point(352, 160)
        Me.cboGEDSP.Name = "cboGEDSP"
        Me.cboGEDSP.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.cboGEDSP.Size = New System.Drawing.Size(89, 22)
        Me.cboGEDSP.TabIndex = 47
        '
        'Frame3
        '
        Me.Frame3.BackColor = System.Drawing.SystemColors.Control
        Me.Frame3.Controls.Add(Me._optTACT_10)
        Me.Frame3.Controls.Add(Me._optTACT_9)
        Me.Frame3.Controls.Add(Me._optTACT_8)
        Me.Frame3.Controls.Add(Me._optTACT_7)
        Me.Frame3.Controls.Add(Me._optTACT_6)
        Me.Frame3.Controls.Add(Me._optTACT_5)
        Me.Frame3.Controls.Add(Me._optTACT_4)
        Me.Frame3.Controls.Add(Me._optTACT_3)
        Me.Frame3.Controls.Add(Me._optTACT_2)
        Me.Frame3.Controls.Add(Me._optTACT_1)
        Me.Frame3.Controls.Add(Me._optTACT_0)
        Me.Frame3.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Frame3.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Frame3.Location = New System.Drawing.Point(240, 184)
        Me.Frame3.Name = "Frame3"
        Me.Frame3.Padding = New System.Windows.Forms.Padding(0)
        Me.Frame3.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Frame3.Size = New System.Drawing.Size(193, 33)
        Me.Frame3.TabIndex = 33
        Me.Frame3.TabStop = False
        Me.Frame3.Text = "TACT"
        '
        '_optTACT_10
        '
        Me._optTACT_10.BackColor = System.Drawing.SystemColors.Control
        Me._optTACT_10.Cursor = System.Windows.Forms.Cursors.Default
        Me._optTACT_10.Enabled = False
        Me._optTACT_10.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me._optTACT_10.ForeColor = System.Drawing.SystemColors.ControlText
        Me.optTACT.SetIndex(Me._optTACT_10, CType(10, Short))
        Me._optTACT_10.Location = New System.Drawing.Point(168, 16)
        Me._optTACT_10.Name = "_optTACT_10"
        Me._optTACT_10.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me._optTACT_10.Size = New System.Drawing.Size(17, 13)
        Me._optTACT_10.TabIndex = 44
        Me._optTACT_10.TabStop = True
        Me._optTACT_10.UseVisualStyleBackColor = False
        '
        '_optTACT_9
        '
        Me._optTACT_9.BackColor = System.Drawing.SystemColors.Control
        Me._optTACT_9.Cursor = System.Windows.Forms.Cursors.Default
        Me._optTACT_9.Enabled = False
        Me._optTACT_9.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me._optTACT_9.ForeColor = System.Drawing.SystemColors.ControlText
        Me.optTACT.SetIndex(Me._optTACT_9, CType(9, Short))
        Me._optTACT_9.Location = New System.Drawing.Point(152, 16)
        Me._optTACT_9.Name = "_optTACT_9"
        Me._optTACT_9.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me._optTACT_9.Size = New System.Drawing.Size(17, 13)
        Me._optTACT_9.TabIndex = 43
        Me._optTACT_9.TabStop = True
        Me._optTACT_9.UseVisualStyleBackColor = False
        '
        '_optTACT_8
        '
        Me._optTACT_8.BackColor = System.Drawing.SystemColors.Control
        Me._optTACT_8.Cursor = System.Windows.Forms.Cursors.Default
        Me._optTACT_8.Enabled = False
        Me._optTACT_8.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me._optTACT_8.ForeColor = System.Drawing.SystemColors.ControlText
        Me.optTACT.SetIndex(Me._optTACT_8, CType(8, Short))
        Me._optTACT_8.Location = New System.Drawing.Point(136, 16)
        Me._optTACT_8.Name = "_optTACT_8"
        Me._optTACT_8.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me._optTACT_8.Size = New System.Drawing.Size(17, 13)
        Me._optTACT_8.TabIndex = 42
        Me._optTACT_8.TabStop = True
        Me._optTACT_8.UseVisualStyleBackColor = False
        '
        '_optTACT_7
        '
        Me._optTACT_7.BackColor = System.Drawing.SystemColors.Control
        Me._optTACT_7.Cursor = System.Windows.Forms.Cursors.Default
        Me._optTACT_7.Enabled = False
        Me._optTACT_7.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me._optTACT_7.ForeColor = System.Drawing.SystemColors.ControlText
        Me.optTACT.SetIndex(Me._optTACT_7, CType(7, Short))
        Me._optTACT_7.Location = New System.Drawing.Point(120, 16)
        Me._optTACT_7.Name = "_optTACT_7"
        Me._optTACT_7.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me._optTACT_7.Size = New System.Drawing.Size(17, 13)
        Me._optTACT_7.TabIndex = 41
        Me._optTACT_7.TabStop = True
        Me._optTACT_7.UseVisualStyleBackColor = False
        '
        '_optTACT_6
        '
        Me._optTACT_6.BackColor = System.Drawing.SystemColors.Control
        Me._optTACT_6.Cursor = System.Windows.Forms.Cursors.Default
        Me._optTACT_6.Enabled = False
        Me._optTACT_6.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me._optTACT_6.ForeColor = System.Drawing.SystemColors.ControlText
        Me.optTACT.SetIndex(Me._optTACT_6, CType(6, Short))
        Me._optTACT_6.Location = New System.Drawing.Point(104, 16)
        Me._optTACT_6.Name = "_optTACT_6"
        Me._optTACT_6.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me._optTACT_6.Size = New System.Drawing.Size(17, 13)
        Me._optTACT_6.TabIndex = 40
        Me._optTACT_6.TabStop = True
        Me._optTACT_6.UseVisualStyleBackColor = False
        '
        '_optTACT_5
        '
        Me._optTACT_5.BackColor = System.Drawing.SystemColors.Control
        Me._optTACT_5.Cursor = System.Windows.Forms.Cursors.Default
        Me._optTACT_5.Enabled = False
        Me._optTACT_5.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me._optTACT_5.ForeColor = System.Drawing.SystemColors.ControlText
        Me.optTACT.SetIndex(Me._optTACT_5, CType(5, Short))
        Me._optTACT_5.Location = New System.Drawing.Point(88, 16)
        Me._optTACT_5.Name = "_optTACT_5"
        Me._optTACT_5.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me._optTACT_5.Size = New System.Drawing.Size(17, 13)
        Me._optTACT_5.TabIndex = 39
        Me._optTACT_5.TabStop = True
        Me._optTACT_5.UseVisualStyleBackColor = False
        '
        '_optTACT_4
        '
        Me._optTACT_4.BackColor = System.Drawing.SystemColors.Control
        Me._optTACT_4.Cursor = System.Windows.Forms.Cursors.Default
        Me._optTACT_4.Enabled = False
        Me._optTACT_4.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me._optTACT_4.ForeColor = System.Drawing.SystemColors.ControlText
        Me.optTACT.SetIndex(Me._optTACT_4, CType(4, Short))
        Me._optTACT_4.Location = New System.Drawing.Point(72, 16)
        Me._optTACT_4.Name = "_optTACT_4"
        Me._optTACT_4.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me._optTACT_4.Size = New System.Drawing.Size(17, 13)
        Me._optTACT_4.TabIndex = 38
        Me._optTACT_4.TabStop = True
        Me._optTACT_4.UseVisualStyleBackColor = False
        '
        '_optTACT_3
        '
        Me._optTACT_3.BackColor = System.Drawing.SystemColors.Control
        Me._optTACT_3.Cursor = System.Windows.Forms.Cursors.Default
        Me._optTACT_3.Enabled = False
        Me._optTACT_3.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me._optTACT_3.ForeColor = System.Drawing.SystemColors.ControlText
        Me.optTACT.SetIndex(Me._optTACT_3, CType(3, Short))
        Me._optTACT_3.Location = New System.Drawing.Point(56, 16)
        Me._optTACT_3.Name = "_optTACT_3"
        Me._optTACT_3.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me._optTACT_3.Size = New System.Drawing.Size(17, 13)
        Me._optTACT_3.TabIndex = 37
        Me._optTACT_3.TabStop = True
        Me._optTACT_3.UseVisualStyleBackColor = False
        '
        '_optTACT_2
        '
        Me._optTACT_2.BackColor = System.Drawing.SystemColors.Control
        Me._optTACT_2.Cursor = System.Windows.Forms.Cursors.Default
        Me._optTACT_2.Enabled = False
        Me._optTACT_2.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me._optTACT_2.ForeColor = System.Drawing.SystemColors.ControlText
        Me.optTACT.SetIndex(Me._optTACT_2, CType(2, Short))
        Me._optTACT_2.Location = New System.Drawing.Point(40, 16)
        Me._optTACT_2.Name = "_optTACT_2"
        Me._optTACT_2.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me._optTACT_2.Size = New System.Drawing.Size(17, 13)
        Me._optTACT_2.TabIndex = 36
        Me._optTACT_2.TabStop = True
        Me._optTACT_2.UseVisualStyleBackColor = False
        '
        '_optTACT_1
        '
        Me._optTACT_1.BackColor = System.Drawing.SystemColors.Control
        Me._optTACT_1.Cursor = System.Windows.Forms.Cursors.Default
        Me._optTACT_1.Enabled = False
        Me._optTACT_1.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me._optTACT_1.ForeColor = System.Drawing.SystemColors.ControlText
        Me.optTACT.SetIndex(Me._optTACT_1, CType(1, Short))
        Me._optTACT_1.Location = New System.Drawing.Point(24, 16)
        Me._optTACT_1.Name = "_optTACT_1"
        Me._optTACT_1.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me._optTACT_1.Size = New System.Drawing.Size(17, 13)
        Me._optTACT_1.TabIndex = 35
        Me._optTACT_1.TabStop = True
        Me._optTACT_1.UseVisualStyleBackColor = False
        '
        '_optTACT_0
        '
        Me._optTACT_0.BackColor = System.Drawing.SystemColors.Control
        Me._optTACT_0.Cursor = System.Windows.Forms.Cursors.Default
        Me._optTACT_0.Enabled = False
        Me._optTACT_0.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me._optTACT_0.ForeColor = System.Drawing.SystemColors.ControlText
        Me.optTACT.SetIndex(Me._optTACT_0, CType(0, Short))
        Me._optTACT_0.Location = New System.Drawing.Point(8, 16)
        Me._optTACT_0.Name = "_optTACT_0"
        Me._optTACT_0.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me._optTACT_0.Size = New System.Drawing.Size(17, 13)
        Me._optTACT_0.TabIndex = 34
        Me._optTACT_0.TabStop = True
        Me._optTACT_0.UseVisualStyleBackColor = False
        '
        'Timer1
        '
        Me.Timer1.Interval = 25
        '
        'Command1
        '
        Me.Command1.BackColor = System.Drawing.SystemColors.Control
        Me.Command1.Cursor = System.Windows.Forms.Cursors.Default
        Me.Command1.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Command1.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Command1.Location = New System.Drawing.Point(72, 8)
        Me.Command1.Name = "Command1"
        Me.Command1.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Command1.Size = New System.Drawing.Size(49, 17)
        Me.Command1.TabIndex = 32
        Me.Command1.Text = "Reset"
        Me.Command1.UseVisualStyleBackColor = False
        '
        'Stop_Renamed
        '
        Me.Stop_Renamed.BackColor = System.Drawing.SystemColors.Control
        Me.Stop_Renamed.Cursor = System.Windows.Forms.Cursors.Default
        Me.Stop_Renamed.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Stop_Renamed.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Stop_Renamed.Location = New System.Drawing.Point(168, 184)
        Me.Stop_Renamed.Name = "Stop_Renamed"
        Me.Stop_Renamed.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Stop_Renamed.Size = New System.Drawing.Size(65, 25)
        Me.Stop_Renamed.TabIndex = 31
        Me.Stop_Renamed.Text = "Stop"
        Me.Stop_Renamed.UseVisualStyleBackColor = False
        '
        'cmdPlay
        '
        Me.cmdPlay.BackColor = System.Drawing.SystemColors.Control
        Me.cmdPlay.Cursor = System.Windows.Forms.Cursors.Default
        Me.cmdPlay.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmdPlay.ForeColor = System.Drawing.SystemColors.ControlText
        Me.cmdPlay.Location = New System.Drawing.Point(88, 184)
        Me.cmdPlay.Name = "cmdPlay"
        Me.cmdPlay.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.cmdPlay.Size = New System.Drawing.Size(73, 25)
        Me.cmdPlay.TabIndex = 30
        Me.cmdPlay.Text = "Play"
        Me.cmdPlay.UseVisualStyleBackColor = False
        '
        'cmdPause
        '
        Me.cmdPause.BackColor = System.Drawing.SystemColors.Control
        Me.cmdPause.Cursor = System.Windows.Forms.Cursors.Default
        Me.cmdPause.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmdPause.ForeColor = System.Drawing.SystemColors.ControlText
        Me.cmdPause.Location = New System.Drawing.Point(88, 152)
        Me.cmdPause.Name = "cmdPause"
        Me.cmdPause.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.cmdPause.Size = New System.Drawing.Size(73, 25)
        Me.cmdPause.TabIndex = 29
        Me.cmdPause.Text = "Pause"
        Me.cmdPause.UseVisualStyleBackColor = False
        '
        'cmdFastForward
        '
        Me.cmdFastForward.BackColor = System.Drawing.SystemColors.Control
        Me.cmdFastForward.Cursor = System.Windows.Forms.Cursors.Default
        Me.cmdFastForward.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmdFastForward.ForeColor = System.Drawing.SystemColors.ControlText
        Me.cmdFastForward.Location = New System.Drawing.Point(8, 184)
        Me.cmdFastForward.Name = "cmdFastForward"
        Me.cmdFastForward.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.cmdFastForward.Size = New System.Drawing.Size(73, 25)
        Me.cmdFastForward.TabIndex = 22
        Me.cmdFastForward.Text = "Fast Forward"
        Me.cmdFastForward.UseVisualStyleBackColor = False
        '
        'Frame2
        '
        Me.Frame2.BackColor = System.Drawing.SystemColors.Control
        Me.Frame2.Controls.Add(Me.optPanDelay)
        Me.Frame2.Controls.Add(Me.optDry)
        Me.Frame2.Controls.Add(Me.optReverb)
        Me.Frame2.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Frame2.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Frame2.Location = New System.Drawing.Point(112, 216)
        Me.Frame2.Name = "Frame2"
        Me.Frame2.Padding = New System.Windows.Forms.Padding(0)
        Me.Frame2.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Frame2.Size = New System.Drawing.Size(105, 98)
        Me.Frame2.TabIndex = 18
        Me.Frame2.TabStop = False
        Me.Frame2.Text = "DSP"
        '
        'optPanDelay
        '
        Me.optPanDelay.BackColor = System.Drawing.SystemColors.Control
        Me.optPanDelay.Cursor = System.Windows.Forms.Cursors.Default
        Me.optPanDelay.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.optPanDelay.ForeColor = System.Drawing.SystemColors.ControlText
        Me.optPanDelay.Location = New System.Drawing.Point(8, 71)
        Me.optPanDelay.Name = "optPanDelay"
        Me.optPanDelay.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.optPanDelay.Size = New System.Drawing.Size(73, 17)
        Me.optPanDelay.TabIndex = 21
        Me.optPanDelay.TabStop = True
        Me.optPanDelay.Text = "Pan Delay"
        Me.optPanDelay.UseVisualStyleBackColor = False
        '
        'optDry
        '
        Me.optDry.BackColor = System.Drawing.SystemColors.Control
        Me.optDry.Cursor = System.Windows.Forms.Cursors.Default
        Me.optDry.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.optDry.ForeColor = System.Drawing.SystemColors.ControlText
        Me.optDry.Location = New System.Drawing.Point(8, 43)
        Me.optDry.Name = "optDry"
        Me.optDry.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.optDry.Size = New System.Drawing.Size(57, 17)
        Me.optDry.TabIndex = 20
        Me.optDry.TabStop = True
        Me.optDry.Text = "Dry"
        Me.optDry.UseVisualStyleBackColor = False
        '
        'optReverb
        '
        Me.optReverb.BackColor = System.Drawing.SystemColors.Control
        Me.optReverb.Checked = True
        Me.optReverb.Cursor = System.Windows.Forms.Cursors.Default
        Me.optReverb.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.optReverb.ForeColor = System.Drawing.SystemColors.ControlText
        Me.optReverb.Location = New System.Drawing.Point(8, 18)
        Me.optReverb.Name = "optReverb"
        Me.optReverb.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.optReverb.Size = New System.Drawing.Size(65, 17)
        Me.optReverb.TabIndex = 19
        Me.optReverb.TabStop = True
        Me.optReverb.Text = "Reverb"
        Me.optReverb.UseVisualStyleBackColor = False
        '
        'Frame1
        '
        Me.Frame1.BackColor = System.Drawing.SystemColors.Control
        Me.Frame1.Controls.Add(Me._optOutput_2)
        Me.Frame1.Controls.Add(Me._optOutput_1)
        Me.Frame1.Controls.Add(Me._optOutput_0)
        Me.Frame1.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Frame1.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Frame1.Location = New System.Drawing.Point(8, 216)
        Me.Frame1.Name = "Frame1"
        Me.Frame1.Padding = New System.Windows.Forms.Padding(0)
        Me.Frame1.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Frame1.Size = New System.Drawing.Size(89, 98)
        Me.Frame1.TabIndex = 14
        Me.Frame1.TabStop = False
        Me.Frame1.Text = "OutPut"
        '
        '_optOutput_2
        '
        Me._optOutput_2.BackColor = System.Drawing.SystemColors.Control
        Me._optOutput_2.Checked = True
        Me._optOutput_2.Cursor = System.Windows.Forms.Cursors.Default
        Me._optOutput_2.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me._optOutput_2.ForeColor = System.Drawing.SystemColors.ControlText
        Me.optOutput.SetIndex(Me._optOutput_2, CType(2, Short))
        Me._optOutput_2.Location = New System.Drawing.Point(8, 72)
        Me._optOutput_2.Name = "_optOutput_2"
        Me._optOutput_2.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me._optOutput_2.Size = New System.Drawing.Size(78, 17)
        Me._optOutput_2.TabIndex = 17
        Me._optOutput_2.TabStop = True
        Me._optOutput_2.Text = "Surround"
        Me._optOutput_2.UseVisualStyleBackColor = False
        '
        '_optOutput_1
        '
        Me._optOutput_1.BackColor = System.Drawing.SystemColors.Control
        Me._optOutput_1.Cursor = System.Windows.Forms.Cursors.Default
        Me._optOutput_1.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me._optOutput_1.ForeColor = System.Drawing.SystemColors.ControlText
        Me.optOutput.SetIndex(Me._optOutput_1, CType(1, Short))
        Me._optOutput_1.Location = New System.Drawing.Point(8, 43)
        Me._optOutput_1.Name = "_optOutput_1"
        Me._optOutput_1.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me._optOutput_1.Size = New System.Drawing.Size(65, 16)
        Me._optOutput_1.TabIndex = 16
        Me._optOutput_1.TabStop = True
        Me._optOutput_1.Text = "Mono"
        Me._optOutput_1.UseVisualStyleBackColor = False
        '
        '_optOutput_0
        '
        Me._optOutput_0.BackColor = System.Drawing.SystemColors.Control
        Me._optOutput_0.Cursor = System.Windows.Forms.Cursors.Default
        Me._optOutput_0.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me._optOutput_0.ForeColor = System.Drawing.SystemColors.ControlText
        Me.optOutput.SetIndex(Me._optOutput_0, CType(0, Short))
        Me._optOutput_0.Location = New System.Drawing.Point(8, 18)
        Me._optOutput_0.Name = "_optOutput_0"
        Me._optOutput_0.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me._optOutput_0.Size = New System.Drawing.Size(65, 19)
        Me._optOutput_0.TabIndex = 15
        Me._optOutput_0.TabStop = True
        Me._optOutput_0.Text = "Stereo"
        Me._optOutput_0.UseVisualStyleBackColor = False
        '
        'vsVOL
        '
        Me.vsVOL.Cursor = System.Windows.Forms.Cursors.Default
        Me.vsVOL.LargeChange = 1
        Me.vsVOL.Location = New System.Drawing.Point(24, 40)
        Me.vsVOL.Maximum = 0
        Me.vsVOL.Minimum = -63
        Me.vsVOL.Name = "vsVOL"
        Me.vsVOL.Size = New System.Drawing.Size(17, 113)
        Me.vsVOL.TabIndex = 0
        Me.vsVOL.TabStop = True
        Me.vsVOL.Value = -63
        '
        'lblPattern
        '
        Me.lblPattern.AutoSize = True
        Me.lblPattern.BackColor = System.Drawing.SystemColors.Control
        Me.lblPattern.Cursor = System.Windows.Forms.Cursors.Default
        Me.lblPattern.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblPattern.ForeColor = System.Drawing.SystemColors.ControlText
        Me.lblPattern.Location = New System.Drawing.Point(272, 160)
        Me.lblPattern.Name = "lblPattern"
        Me.lblPattern.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.lblPattern.Size = New System.Drawing.Size(68, 14)
        Me.lblPattern.TabIndex = 46
        Me.lblPattern.Text = "Pattern #: 00"
        '
        'lblLoop
        '
        Me.lblLoop.AutoSize = True
        Me.lblLoop.BackColor = System.Drawing.SystemColors.Control
        Me.lblLoop.Cursor = System.Windows.Forms.Cursors.Default
        Me.lblLoop.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblLoop.ForeColor = System.Drawing.SystemColors.ControlText
        Me.lblLoop.Location = New System.Drawing.Point(176, 160)
        Me.lblLoop.Name = "lblLoop"
        Me.lblLoop.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.lblLoop.Size = New System.Drawing.Size(80, 14)
        Me.lblLoop.TabIndex = 45
        Me.lblLoop.Text = "Loop Count: 00"
        '
        'Label6
        '
        Me.Label6.BackColor = System.Drawing.SystemColors.Control
        Me.Label6.Cursor = System.Windows.Forms.Cursors.Default
        Me.Label6.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label6.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Label6.Location = New System.Drawing.Point(224, 288)
        Me.Label6.Name = "Label6"
        Me.Label6.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Label6.Size = New System.Drawing.Size(33, 17)
        Me.Label6.TabIndex = 28
        Me.Label6.Text = "Pitch"
        '
        'Label5
        '
        Me.Label5.BackColor = System.Drawing.SystemColors.Control
        Me.Label5.Cursor = System.Windows.Forms.Cursors.Default
        Me.Label5.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label5.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Label5.Location = New System.Drawing.Point(224, 256)
        Me.Label5.Name = "Label5"
        Me.Label5.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Label5.Size = New System.Drawing.Size(33, 17)
        Me.Label5.TabIndex = 27
        Me.Label5.Text = "Trans"
        '
        'Label4
        '
        Me.Label4.BackColor = System.Drawing.SystemColors.Control
        Me.Label4.Cursor = System.Windows.Forms.Cursors.Default
        Me.Label4.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Label4.Location = New System.Drawing.Point(224, 232)
        Me.Label4.Name = "Label4"
        Me.Label4.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Label4.Size = New System.Drawing.Size(25, 17)
        Me.Label4.TabIndex = 24
        Me.Label4.Text = "Pan"
        '
        'Label3
        '
        Me.Label3.BackColor = System.Drawing.SystemColors.Control
        Me.Label3.Cursor = System.Windows.Forms.Cursors.Default
        Me.Label3.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Label3.Location = New System.Drawing.Point(208, 8)
        Me.Label3.Name = "Label3"
        Me.Label3.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Label3.Size = New System.Drawing.Size(129, 17)
        Me.Label3.TabIndex = 13
        Me.Label3.Text = "9 Band Graphic Equalizer"
        '
        'Label2
        '
        Me.Label2.BackColor = System.Drawing.SystemColors.Control
        Me.Label2.Cursor = System.Windows.Forms.Cursors.Default
        Me.Label2.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Label2.Location = New System.Drawing.Point(128, 136)
        Me.Label2.Name = "Label2"
        Me.Label2.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Label2.Size = New System.Drawing.Size(297, 17)
        Me.Label2.TabIndex = 12
        Me.Label2.Text = " 30.................150...............550................2K..................9Khz" & _
            ""
        '
        'Label1
        '
        Me.Label1.BackColor = System.Drawing.SystemColors.Control
        Me.Label1.Cursor = System.Windows.Forms.Cursors.Default
        Me.Label1.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Label1.Location = New System.Drawing.Point(16, 16)
        Me.Label1.Name = "Label1"
        Me.Label1.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Label1.Size = New System.Drawing.Size(41, 17)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "Volume"
        '
        'lblVOL
        '
        Me.lblVOL.AutoSize = True
        Me.lblVOL.BackColor = System.Drawing.SystemColors.Control
        Me.lblVOL.Cursor = System.Windows.Forms.Cursors.Default
        Me.lblVOL.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblVOL.ForeColor = System.Drawing.SystemColors.ControlText
        Me.lblVOL.Location = New System.Drawing.Point(24, 160)
        Me.lblVOL.Name = "lblVOL"
        Me.lblVOL.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.lblVOL.Size = New System.Drawing.Size(13, 14)
        Me.lblVOL.TabIndex = 1
        Me.lblVOL.Text = "0"
        '
        'optOutput
        '
        '
        '_SldGraficEqualizer_0
        '
        Me._SldGraficEqualizer_0.Location = New System.Drawing.Point(127, 20)
        Me._SldGraficEqualizer_0.Maximum = 0
        Me._SldGraficEqualizer_0.Minimum = -15
        Me._SldGraficEqualizer_0.Name = "_SldGraficEqualizer_0"
        Me._SldGraficEqualizer_0.Orientation = System.Windows.Forms.Orientation.Vertical
        Me._SldGraficEqualizer_0.Size = New System.Drawing.Size(42, 113)
        Me._SldGraficEqualizer_0.TabIndex = 48
        Me._SldGraficEqualizer_0.Value = -7
        '
        '_SldGraficEqualizer_1
        '
        Me._SldGraficEqualizer_1.Location = New System.Drawing.Point(159, 20)
        Me._SldGraficEqualizer_1.Maximum = 0
        Me._SldGraficEqualizer_1.Minimum = -15
        Me._SldGraficEqualizer_1.Name = "_SldGraficEqualizer_1"
        Me._SldGraficEqualizer_1.Orientation = System.Windows.Forms.Orientation.Vertical
        Me._SldGraficEqualizer_1.Size = New System.Drawing.Size(42, 113)
        Me._SldGraficEqualizer_1.TabIndex = 49
        Me._SldGraficEqualizer_1.Value = -7
        '
        '_SldGraficEqualizer_2
        '
        Me._SldGraficEqualizer_2.Location = New System.Drawing.Point(191, 20)
        Me._SldGraficEqualizer_2.Maximum = 0
        Me._SldGraficEqualizer_2.Minimum = -15
        Me._SldGraficEqualizer_2.Name = "_SldGraficEqualizer_2"
        Me._SldGraficEqualizer_2.Orientation = System.Windows.Forms.Orientation.Vertical
        Me._SldGraficEqualizer_2.Size = New System.Drawing.Size(42, 113)
        Me._SldGraficEqualizer_2.TabIndex = 50
        Me._SldGraficEqualizer_2.Value = -7
        '
        '_SldGraficEqualizer_3
        '
        Me._SldGraficEqualizer_3.Location = New System.Drawing.Point(223, 20)
        Me._SldGraficEqualizer_3.Maximum = 0
        Me._SldGraficEqualizer_3.Minimum = -15
        Me._SldGraficEqualizer_3.Name = "_SldGraficEqualizer_3"
        Me._SldGraficEqualizer_3.Orientation = System.Windows.Forms.Orientation.Vertical
        Me._SldGraficEqualizer_3.Size = New System.Drawing.Size(42, 113)
        Me._SldGraficEqualizer_3.TabIndex = 51
        Me._SldGraficEqualizer_3.Value = -7
        '
        '_SldGraficEqualizer_4
        '
        Me._SldGraficEqualizer_4.Location = New System.Drawing.Point(255, 20)
        Me._SldGraficEqualizer_4.Maximum = 0
        Me._SldGraficEqualizer_4.Minimum = -15
        Me._SldGraficEqualizer_4.Name = "_SldGraficEqualizer_4"
        Me._SldGraficEqualizer_4.Orientation = System.Windows.Forms.Orientation.Vertical
        Me._SldGraficEqualizer_4.Size = New System.Drawing.Size(42, 113)
        Me._SldGraficEqualizer_4.TabIndex = 52
        Me._SldGraficEqualizer_4.Value = -7
        '
        '_SldGraficEqualizer_5
        '
        Me._SldGraficEqualizer_5.Location = New System.Drawing.Point(287, 20)
        Me._SldGraficEqualizer_5.Maximum = 0
        Me._SldGraficEqualizer_5.Minimum = -15
        Me._SldGraficEqualizer_5.Name = "_SldGraficEqualizer_5"
        Me._SldGraficEqualizer_5.Orientation = System.Windows.Forms.Orientation.Vertical
        Me._SldGraficEqualizer_5.Size = New System.Drawing.Size(42, 113)
        Me._SldGraficEqualizer_5.TabIndex = 53
        Me._SldGraficEqualizer_5.Value = -7
        '
        '_SldGraficEqualizer_6
        '
        Me._SldGraficEqualizer_6.Location = New System.Drawing.Point(319, 20)
        Me._SldGraficEqualizer_6.Maximum = 0
        Me._SldGraficEqualizer_6.Minimum = -15
        Me._SldGraficEqualizer_6.Name = "_SldGraficEqualizer_6"
        Me._SldGraficEqualizer_6.Orientation = System.Windows.Forms.Orientation.Vertical
        Me._SldGraficEqualizer_6.Size = New System.Drawing.Size(42, 113)
        Me._SldGraficEqualizer_6.TabIndex = 54
        Me._SldGraficEqualizer_6.Value = -7
        '
        '_SldGraficEqualizer_7
        '
        Me._SldGraficEqualizer_7.Location = New System.Drawing.Point(352, 20)
        Me._SldGraficEqualizer_7.Maximum = 0
        Me._SldGraficEqualizer_7.Minimum = -15
        Me._SldGraficEqualizer_7.Name = "_SldGraficEqualizer_7"
        Me._SldGraficEqualizer_7.Orientation = System.Windows.Forms.Orientation.Vertical
        Me._SldGraficEqualizer_7.Size = New System.Drawing.Size(42, 113)
        Me._SldGraficEqualizer_7.TabIndex = 55
        Me._SldGraficEqualizer_7.Value = -7
        '
        '_SldGraficEqualizer_8
        '
        Me._SldGraficEqualizer_8.Location = New System.Drawing.Point(383, 20)
        Me._SldGraficEqualizer_8.Maximum = 0
        Me._SldGraficEqualizer_8.Minimum = -15
        Me._SldGraficEqualizer_8.Name = "_SldGraficEqualizer_8"
        Me._SldGraficEqualizer_8.Orientation = System.Windows.Forms.Orientation.Vertical
        Me._SldGraficEqualizer_8.Size = New System.Drawing.Size(42, 113)
        Me._SldGraficEqualizer_8.TabIndex = 56
        Me._SldGraficEqualizer_8.Value = -7
        '
        'sldPan
        '
        Me.sldPan.Location = New System.Drawing.Point(264, 224)
        Me.sldPan.Maximum = 0
        Me.sldPan.Minimum = -63
        Me.sldPan.Name = "sldPan"
        Me.sldPan.Size = New System.Drawing.Size(169, 42)
        Me.sldPan.TabIndex = 57
        Me.sldPan.TickStyle = System.Windows.Forms.TickStyle.TopLeft
        Me.sldPan.Value = -32
        '
        'sldTrans
        '
        Me.sldTrans.Location = New System.Drawing.Point(264, 255)
        Me.sldTrans.Maximum = 15
        Me.sldTrans.Name = "sldTrans"
        Me.sldTrans.Size = New System.Drawing.Size(169, 42)
        Me.sldTrans.TabIndex = 58
        Me.sldTrans.TickStyle = System.Windows.Forms.TickStyle.TopLeft
        Me.sldTrans.Value = 8
        '
        'sldPitch
        '
        Me.sldPitch.Location = New System.Drawing.Point(264, 285)
        Me.sldPitch.Maximum = 63
        Me.sldPitch.Name = "sldPitch"
        Me.sldPitch.Size = New System.Drawing.Size(169, 42)
        Me.sldPitch.TabIndex = 59
        Me.sldPitch.TickStyle = System.Windows.Forms.TickStyle.TopLeft
        Me.sldPitch.Value = 32
        '
        'frmToP
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 14.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.Control
        Me.ClientSize = New System.Drawing.Size(446, 326)
        Me.Controls.Add(Me.sldPitch)
        Me.Controls.Add(Me.sldTrans)
        Me.Controls.Add(Me.sldPan)
        Me.Controls.Add(Me._SldGraficEqualizer_8)
        Me.Controls.Add(Me._SldGraficEqualizer_7)
        Me.Controls.Add(Me._SldGraficEqualizer_6)
        Me.Controls.Add(Me._SldGraficEqualizer_5)
        Me.Controls.Add(Me._SldGraficEqualizer_4)
        Me.Controls.Add(Me._SldGraficEqualizer_3)
        Me.Controls.Add(Me._SldGraficEqualizer_2)
        Me.Controls.Add(Me._SldGraficEqualizer_1)
        Me.Controls.Add(Me._SldGraficEqualizer_0)
        Me.Controls.Add(Me.cboGEDSP)
        Me.Controls.Add(Me.Frame3)
        Me.Controls.Add(Me.Command1)
        Me.Controls.Add(Me.Stop_Renamed)
        Me.Controls.Add(Me.cmdPlay)
        Me.Controls.Add(Me.cmdPause)
        Me.Controls.Add(Me.cmdFastForward)
        Me.Controls.Add(Me.Frame2)
        Me.Controls.Add(Me.Frame1)
        Me.Controls.Add(Me.vsVOL)
        Me.Controls.Add(Me.lblPattern)
        Me.Controls.Add(Me.lblLoop)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.lblVOL)
        Me.Cursor = System.Windows.Forms.Cursors.Default
        Me.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Location = New System.Drawing.Point(4, 23)
        Me.Name = "frmToP"
        Me.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Text = "Tales of Phantasia/Star Ocean Control Panel"
        Me.Frame3.ResumeLayout(False)
        Me.Frame2.ResumeLayout(False)
        Me.Frame1.ResumeLayout(False)
        CType(Me.optOutput, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.optTACT, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me._SldGraficEqualizer_0, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me._SldGraficEqualizer_1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me._SldGraficEqualizer_2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me._SldGraficEqualizer_3, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me._SldGraficEqualizer_4, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me._SldGraficEqualizer_5, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me._SldGraficEqualizer_6, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me._SldGraficEqualizer_7, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me._SldGraficEqualizer_8, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.sldPan, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.sldTrans, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.sldPitch, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents _SldGraficEqualizer_0 As System.Windows.Forms.TrackBar
    Friend WithEvents _SldGraficEqualizer_1 As System.Windows.Forms.TrackBar
    Friend WithEvents _SldGraficEqualizer_2 As System.Windows.Forms.TrackBar
    Friend WithEvents _SldGraficEqualizer_3 As System.Windows.Forms.TrackBar
    Friend WithEvents _SldGraficEqualizer_4 As System.Windows.Forms.TrackBar
    Friend WithEvents _SldGraficEqualizer_5 As System.Windows.Forms.TrackBar
    Friend WithEvents _SldGraficEqualizer_6 As System.Windows.Forms.TrackBar
    Friend WithEvents _SldGraficEqualizer_7 As System.Windows.Forms.TrackBar
    Friend WithEvents _SldGraficEqualizer_8 As System.Windows.Forms.TrackBar
    Friend WithEvents sldPan As System.Windows.Forms.TrackBar
    Friend WithEvents sldTrans As System.Windows.Forms.TrackBar
    Friend WithEvents sldPitch As System.Windows.Forms.TrackBar
#End Region 
End Class