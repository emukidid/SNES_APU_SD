<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> Partial Class Form1
#Region "Windows Form Designer generated code "
    <System.Diagnostics.DebuggerNonUserCode()> Public Sub New()
        MyBase.New()
        'This call is required by the Windows Form Designer.
        InitializeComponent()
    End Sub
    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> Protected Overloads Overrides Sub Dispose(ByVal Disposing As Boolean)
        If Disposing Then
            Static fTerminateCalled As Boolean
            If Not fTerminateCalled Then
                Form_Terminate_Renamed()
                fTerminateCalled = True
            End If
            If Not components Is Nothing Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(Disposing)
    End Sub
    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer
    Public ToolTip1 As System.Windows.Forms.ToolTip
    Public WithEvents Command2 As System.Windows.Forms.Button
    Public WithEvents Combo1 As System.Windows.Forms.ComboBox
    Public WithEvents PauseUpload As System.Windows.Forms.Button
    Public WithEvents cmdLoadPrev As System.Windows.Forms.Button
    'Public WithEvents Slider3 As Axmscomctl.AxSlider
    'Public WithEvents Slider4 As Axmscomctl.AxSlider
    '	Public WithEvents Slider1 As Axmscomctl.AxSlider
    '	Public WithEvents Slider2 As Axmscomctl.AxSlider
    Public WithEvents Option3 As System.Windows.Forms.RadioButton
    Public WithEvents Option2 As System.Windows.Forms.RadioButton
    Public WithEvents Option1 As System.Windows.Forms.RadioButton
    Public WithEvents Frame1 As System.Windows.Forms.GroupBox
    Public WithEvents Text1 As System.Windows.Forms.TextBox
    Public WithEvents cmdOpenPort As System.Windows.Forms.Button
    Public WithEvents txtUploadSpeed As System.Windows.Forms.TextBox
    Public WithEvents Initialize As System.Windows.Forms.Button
    Public WithEvents txtReadSpeed As System.Windows.Forms.TextBox
    Public WithEvents Timer1 As System.Windows.Forms.Timer
    Public WithEvents apuLoad As System.Windows.Forms.ProgressBar
    Public WithEvents cmdLoadAPU As System.Windows.Forms.Button
    Public WithEvents HScroll1 As System.Windows.Forms.HScrollBar
    Public WithEvents cmdReset As System.Windows.Forms.Button
    Public SPCbrowseOpen As System.Windows.Forms.OpenFileDialog
    Public WithEvents _txtIn_3 As System.Windows.Forms.TextBox
    Public WithEvents _txtIn_2 As System.Windows.Forms.TextBox
    Public WithEvents _txtIn_1 As System.Windows.Forms.TextBox
    Public WithEvents _txtIn_0 As System.Windows.Forms.TextBox
    Public WithEvents _txtOut_3 As System.Windows.Forms.TextBox
    Public WithEvents _txtOut_2 As System.Windows.Forms.TextBox
    Public WithEvents _txtOut_1 As System.Windows.Forms.TextBox
    Public WithEvents _txtOut_0 As System.Windows.Forms.TextBox
    Public WithEvents tmrReadport As System.Windows.Forms.Timer
    Public WithEvents Image1 As System.Windows.Forms.PictureBox
    Public WithEvents lblArtist As System.Windows.Forms.Label
    Public WithEvents lblSong As System.Windows.Forms.Label
    Public WithEvents lblGame As System.Windows.Forms.Label
    Public WithEvents Label7 As System.Windows.Forms.Label
    Public WithEvents Label6 As System.Windows.Forms.Label
    Public WithEvents Label5 As System.Windows.Forms.Label
    Public WithEvents Label4 As System.Windows.Forms.Label
    Public WithEvents Label3 As System.Windows.Forms.Label
    Public WithEvents Label2 As System.Windows.Forms.Label
    Public WithEvents Label1 As System.Windows.Forms.Label
    Public WithEvents txtIn As Microsoft.VisualBasic.Compatibility.VB6.TextBoxArray
    Public WithEvents txtOut As Microsoft.VisualBasic.Compatibility.VB6.TextBoxArray
    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.Command2 = New System.Windows.Forms.Button()
        Me.Combo1 = New System.Windows.Forms.ComboBox()
        Me.PauseUpload = New System.Windows.Forms.Button()
        Me.cmdLoadPrev = New System.Windows.Forms.Button()
        Me.Frame1 = New System.Windows.Forms.GroupBox()
        Me.Option3 = New System.Windows.Forms.RadioButton()
        Me.Option2 = New System.Windows.Forms.RadioButton()
        Me.Option1 = New System.Windows.Forms.RadioButton()
        Me.Text1 = New System.Windows.Forms.TextBox()
        Me.cmdOpenPort = New System.Windows.Forms.Button()
        Me.txtUploadSpeed = New System.Windows.Forms.TextBox()
        Me.Initialize = New System.Windows.Forms.Button()
        Me.txtReadSpeed = New System.Windows.Forms.TextBox()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.apuLoad = New System.Windows.Forms.ProgressBar()
        Me.cmdLoadAPU = New System.Windows.Forms.Button()
        Me.HScroll1 = New System.Windows.Forms.HScrollBar()
        Me.cmdReset = New System.Windows.Forms.Button()
        Me.SPCbrowseOpen = New System.Windows.Forms.OpenFileDialog()
        Me._txtIn_3 = New System.Windows.Forms.TextBox()
        Me._txtIn_2 = New System.Windows.Forms.TextBox()
        Me._txtIn_1 = New System.Windows.Forms.TextBox()
        Me._txtIn_0 = New System.Windows.Forms.TextBox()
        Me._txtOut_3 = New System.Windows.Forms.TextBox()
        Me._txtOut_2 = New System.Windows.Forms.TextBox()
        Me._txtOut_1 = New System.Windows.Forms.TextBox()
        Me._txtOut_0 = New System.Windows.Forms.TextBox()
        Me.tmrReadport = New System.Windows.Forms.Timer(Me.components)
        Me.Image1 = New System.Windows.Forms.PictureBox()
        Me.lblArtist = New System.Windows.Forms.Label()
        Me.lblSong = New System.Windows.Forms.Label()
        Me.lblGame = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.txtIn = New Microsoft.VisualBasic.Compatibility.VB6.TextBoxArray(Me.components)
        Me._TxtIn_4 = New System.Windows.Forms.TextBox()
        Me.txtOut = New Microsoft.VisualBasic.Compatibility.VB6.TextBoxArray(Me.components)
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.RadioButton4 = New System.Windows.Forms.RadioButton()
        Me.RadioButton3 = New System.Windows.Forms.RadioButton()
        Me.RadioButton2 = New System.Windows.Forms.RadioButton()
        Me.RadioButton1 = New System.Windows.Forms.RadioButton()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.ListBox1 = New System.Windows.Forms.ListBox()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.Button3 = New System.Windows.Forms.Button()
        Me.Panel3 = New System.Windows.Forms.Panel()
        Me.lblComment = New System.Windows.Forms.TextBox()
        Me.chkAutoPlay = New System.Windows.Forms.CheckBox()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.lblFileName = New System.Windows.Forms.Label()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.lblPublisher = New System.Windows.Forms.Label()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.lblOST = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.lblDumper = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.ListBox3 = New System.Windows.Forms.ListBox()
        Me.ListBox2 = New System.Windows.Forms.ListBox()
        Me.tmrAutoPlay = New System.Windows.Forms.Timer(Me.components)
        Me.Frame1.SuspendLayout()
        CType(Me.Image1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.txtIn, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.txtOut, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox1.SuspendLayout()
        Me.Panel1.SuspendLayout()
        Me.Panel2.SuspendLayout()
        Me.Panel3.SuspendLayout()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.SuspendLayout()
        '
        'Command2
        '
        Me.Command2.BackColor = System.Drawing.SystemColors.Control
        Me.Command2.Cursor = System.Windows.Forms.Cursors.Default
        Me.Command2.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Command2.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Command2.Location = New System.Drawing.Point(243, 170)
        Me.Command2.Name = "Command2"
        Me.Command2.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Command2.Size = New System.Drawing.Size(144, 29)
        Me.Command2.TabIndex = 41
        Me.Command2.Text = "Refresh Port List"
        Me.Command2.UseVisualStyleBackColor = False
        '
        'Combo1
        '
        Me.Combo1.BackColor = System.Drawing.SystemColors.Window
        Me.Combo1.Cursor = System.Windows.Forms.Cursors.Default
        Me.Combo1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.Combo1.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Combo1.ForeColor = System.Drawing.SystemColors.WindowText
        Me.Combo1.Location = New System.Drawing.Point(243, 115)
        Me.Combo1.Name = "Combo1"
        Me.Combo1.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Combo1.Size = New System.Drawing.Size(142, 22)
        Me.Combo1.TabIndex = 40
        '
        'PauseUpload
        '
        Me.PauseUpload.BackColor = System.Drawing.SystemColors.Control
        Me.PauseUpload.Cursor = System.Windows.Forms.Cursors.Default
        Me.PauseUpload.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.PauseUpload.ForeColor = System.Drawing.SystemColors.ControlText
        Me.PauseUpload.Location = New System.Drawing.Point(151, 58)
        Me.PauseUpload.Name = "PauseUpload"
        Me.PauseUpload.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.PauseUpload.Size = New System.Drawing.Size(90, 25)
        Me.PauseUpload.TabIndex = 39
        Me.PauseUpload.Text = "Pause Upload"
        Me.PauseUpload.UseVisualStyleBackColor = False
        Me.PauseUpload.Visible = False
        '
        'cmdLoadPrev
        '
        Me.cmdLoadPrev.BackColor = System.Drawing.SystemColors.Control
        Me.cmdLoadPrev.Cursor = System.Windows.Forms.Cursors.Default
        Me.cmdLoadPrev.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmdLoadPrev.ForeColor = System.Drawing.SystemColors.ControlText
        Me.cmdLoadPrev.Location = New System.Drawing.Point(82, 31)
        Me.cmdLoadPrev.Name = "cmdLoadPrev"
        Me.cmdLoadPrev.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.cmdLoadPrev.Size = New System.Drawing.Size(65, 25)
        Me.cmdLoadPrev.TabIndex = 37
        Me.cmdLoadPrev.Text = "Load Curr"
        Me.cmdLoadPrev.UseVisualStyleBackColor = False
        '
        'Frame1
        '
        Me.Frame1.BackColor = System.Drawing.SystemColors.Control
        Me.Frame1.Controls.Add(Me.Option3)
        Me.Frame1.Controls.Add(Me.Option2)
        Me.Frame1.Controls.Add(Me.Option1)
        Me.Frame1.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Frame1.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Frame1.Location = New System.Drawing.Point(4, 117)
        Me.Frame1.Name = "Frame1"
        Me.Frame1.Padding = New System.Windows.Forms.Padding(0)
        Me.Frame1.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Frame1.Size = New System.Drawing.Size(111, 88)
        Me.Frame1.TabIndex = 27
        Me.Frame1.TabStop = False
        Me.Frame1.Text = "Echo Region Clear"
        '
        'Option3
        '
        Me.Option3.BackColor = System.Drawing.SystemColors.Control
        Me.Option3.Checked = True
        Me.Option3.Cursor = System.Windows.Forms.Cursors.Default
        Me.Option3.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Option3.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Option3.Location = New System.Drawing.Point(9, 16)
        Me.Option3.Name = "Option3"
        Me.Option3.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Option3.Size = New System.Drawing.Size(57, 17)
        Me.Option3.TabIndex = 30
        Me.Option3.TabStop = True
        Me.Option3.Text = "Auto"
        Me.Option3.UseVisualStyleBackColor = False
        '
        'Option2
        '
        Me.Option2.BackColor = System.Drawing.SystemColors.Control
        Me.Option2.Cursor = System.Windows.Forms.Cursors.Default
        Me.Option2.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Option2.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Option2.Location = New System.Drawing.Point(9, 47)
        Me.Option2.Name = "Option2"
        Me.Option2.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Option2.Size = New System.Drawing.Size(57, 18)
        Me.Option2.TabIndex = 29
        Me.Option2.Text = "No"
        Me.Option2.UseVisualStyleBackColor = False
        '
        'Option1
        '
        Me.Option1.BackColor = System.Drawing.SystemColors.Control
        Me.Option1.Cursor = System.Windows.Forms.Cursors.Default
        Me.Option1.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Option1.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Option1.Location = New System.Drawing.Point(9, 32)
        Me.Option1.Name = "Option1"
        Me.Option1.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Option1.Size = New System.Drawing.Size(55, 17)
        Me.Option1.TabIndex = 28
        Me.Option1.TabStop = True
        Me.Option1.Text = "Yes"
        Me.Option1.UseVisualStyleBackColor = False
        '
        'Text1
        '
        Me.Text1.AcceptsReturn = True
        Me.Text1.BackColor = System.Drawing.SystemColors.Window
        Me.Text1.Cursor = System.Windows.Forms.Cursors.IBeam
        Me.Text1.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Text1.ForeColor = System.Drawing.SystemColors.WindowText
        Me.Text1.Location = New System.Drawing.Point(0, 444)
        Me.Text1.MaxLength = 0
        Me.Text1.Multiline = True
        Me.Text1.Name = "Text1"
        Me.Text1.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Text1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.Text1.Size = New System.Drawing.Size(173, 73)
        Me.Text1.TabIndex = 26
        '
        'cmdOpenPort
        '
        Me.cmdOpenPort.BackColor = System.Drawing.SystemColors.Control
        Me.cmdOpenPort.Cursor = System.Windows.Forms.Cursors.Default
        Me.cmdOpenPort.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmdOpenPort.ForeColor = System.Drawing.SystemColors.ControlText
        Me.cmdOpenPort.Location = New System.Drawing.Point(243, 140)
        Me.cmdOpenPort.Name = "cmdOpenPort"
        Me.cmdOpenPort.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.cmdOpenPort.Size = New System.Drawing.Size(70, 25)
        Me.cmdOpenPort.TabIndex = 25
        Me.cmdOpenPort.Text = "Open Port"
        Me.cmdOpenPort.UseVisualStyleBackColor = False
        '
        'txtUploadSpeed
        '
        Me.txtUploadSpeed.AcceptsReturn = True
        Me.txtUploadSpeed.BackColor = System.Drawing.SystemColors.Window
        Me.txtUploadSpeed.Cursor = System.Windows.Forms.Cursors.IBeam
        Me.txtUploadSpeed.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtUploadSpeed.ForeColor = System.Drawing.SystemColors.WindowText
        Me.txtUploadSpeed.Location = New System.Drawing.Point(95, 423)
        Me.txtUploadSpeed.MaxLength = 0
        Me.txtUploadSpeed.Name = "txtUploadSpeed"
        Me.txtUploadSpeed.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.txtUploadSpeed.Size = New System.Drawing.Size(287, 20)
        Me.txtUploadSpeed.TabIndex = 24
        '
        'Initialize
        '
        Me.Initialize.BackColor = System.Drawing.SystemColors.Control
        Me.Initialize.Cursor = System.Windows.Forms.Cursors.Default
        Me.Initialize.Enabled = False
        Me.Initialize.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Initialize.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Initialize.Location = New System.Drawing.Point(315, 140)
        Me.Initialize.Name = "Initialize"
        Me.Initialize.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Initialize.Size = New System.Drawing.Size(70, 25)
        Me.Initialize.TabIndex = 23
        Me.Initialize.Text = "Close Port"
        Me.Initialize.UseVisualStyleBackColor = False
        '
        'txtReadSpeed
        '
        Me.txtReadSpeed.AcceptsReturn = True
        Me.txtReadSpeed.BackColor = System.Drawing.SystemColors.Window
        Me.txtReadSpeed.Cursor = System.Windows.Forms.Cursors.IBeam
        Me.txtReadSpeed.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtReadSpeed.ForeColor = System.Drawing.SystemColors.WindowText
        Me.txtReadSpeed.Location = New System.Drawing.Point(0, 423)
        Me.txtReadSpeed.MaxLength = 0
        Me.txtReadSpeed.Name = "txtReadSpeed"
        Me.txtReadSpeed.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.txtReadSpeed.Size = New System.Drawing.Size(89, 20)
        Me.txtReadSpeed.TabIndex = 22
        Me.txtReadSpeed.Text = "Text1"
        '
        'Timer1
        '
        Me.Timer1.Enabled = True
        Me.Timer1.Interval = 1000
        '
        'apuLoad
        '
        Me.apuLoad.Location = New System.Drawing.Point(2, 92)
        Me.apuLoad.Maximum = 65535
        Me.apuLoad.Name = "apuLoad"
        Me.apuLoad.Size = New System.Drawing.Size(385, 17)
        Me.apuLoad.TabIndex = 15
        '
        'cmdLoadAPU
        '
        Me.cmdLoadAPU.BackColor = System.Drawing.SystemColors.Control
        Me.cmdLoadAPU.Cursor = System.Windows.Forms.Cursors.Default
        Me.cmdLoadAPU.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmdLoadAPU.ForeColor = System.Drawing.SystemColors.ControlText
        Me.cmdLoadAPU.Location = New System.Drawing.Point(82, 58)
        Me.cmdLoadAPU.Name = "cmdLoadAPU"
        Me.cmdLoadAPU.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.cmdLoadAPU.Size = New System.Drawing.Size(65, 25)
        Me.cmdLoadAPU.TabIndex = 14
        Me.cmdLoadAPU.Text = "Load APU"
        Me.cmdLoadAPU.UseVisualStyleBackColor = False
        '
        'HScroll1
        '
        Me.HScroll1.Cursor = System.Windows.Forms.Cursors.Default
        Me.HScroll1.LargeChange = 1
        Me.HScroll1.Location = New System.Drawing.Point(0, 2)
        Me.HScroll1.Maximum = 1000
        Me.HScroll1.Name = "HScroll1"
        Me.HScroll1.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.HScroll1.Size = New System.Drawing.Size(241, 22)
        Me.HScroll1.TabIndex = 13
        Me.HScroll1.TabStop = True
        '
        'cmdReset
        '
        Me.cmdReset.BackColor = System.Drawing.SystemColors.Control
        Me.cmdReset.Cursor = System.Windows.Forms.Cursors.Default
        Me.cmdReset.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmdReset.ForeColor = System.Drawing.SystemColors.ControlText
        Me.cmdReset.Location = New System.Drawing.Point(0, 58)
        Me.cmdReset.Name = "cmdReset"
        Me.cmdReset.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.cmdReset.Size = New System.Drawing.Size(77, 25)
        Me.cmdReset.TabIndex = 12
        Me.cmdReset.Text = "Reset APU"
        Me.cmdReset.UseVisualStyleBackColor = False
        '
        'SPCbrowseOpen
        '
        Me.SPCbrowseOpen.Filter = "SPC Files|*.sp?"
        Me.SPCbrowseOpen.Multiselect = True
        '
        '_txtIn_3
        '
        Me._txtIn_3.AcceptsReturn = True
        Me._txtIn_3.BackColor = System.Drawing.SystemColors.Window
        Me._txtIn_3.Cursor = System.Windows.Forms.Cursors.IBeam
        Me._txtIn_3.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me._txtIn_3.ForeColor = System.Drawing.SystemColors.WindowText
        Me.txtIn.SetIndex(Me._txtIn_3, CType(3, Short))
        Me._txtIn_3.Location = New System.Drawing.Point(46, 65)
        Me._txtIn_3.MaxLength = 2
        Me._txtIn_3.Name = "_txtIn_3"
        Me._txtIn_3.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me._txtIn_3.Size = New System.Drawing.Size(37, 20)
        Me._txtIn_3.TabIndex = 11
        '
        '_txtIn_2
        '
        Me._txtIn_2.AcceptsReturn = True
        Me._txtIn_2.BackColor = System.Drawing.SystemColors.Window
        Me._txtIn_2.Cursor = System.Windows.Forms.Cursors.IBeam
        Me._txtIn_2.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me._txtIn_2.ForeColor = System.Drawing.SystemColors.WindowText
        Me.txtIn.SetIndex(Me._txtIn_2, CType(2, Short))
        Me._txtIn_2.Location = New System.Drawing.Point(46, 44)
        Me._txtIn_2.MaxLength = 2
        Me._txtIn_2.Name = "_txtIn_2"
        Me._txtIn_2.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me._txtIn_2.Size = New System.Drawing.Size(37, 20)
        Me._txtIn_2.TabIndex = 10
        '
        '_txtIn_1
        '
        Me._txtIn_1.AcceptsReturn = True
        Me._txtIn_1.BackColor = System.Drawing.SystemColors.Window
        Me._txtIn_1.Cursor = System.Windows.Forms.Cursors.IBeam
        Me._txtIn_1.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me._txtIn_1.ForeColor = System.Drawing.SystemColors.WindowText
        Me.txtIn.SetIndex(Me._txtIn_1, CType(1, Short))
        Me._txtIn_1.Location = New System.Drawing.Point(46, 23)
        Me._txtIn_1.MaxLength = 2
        Me._txtIn_1.Name = "_txtIn_1"
        Me._txtIn_1.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me._txtIn_1.Size = New System.Drawing.Size(37, 20)
        Me._txtIn_1.TabIndex = 9
        '
        '_txtIn_0
        '
        Me._txtIn_0.AcceptsReturn = True
        Me._txtIn_0.BackColor = System.Drawing.SystemColors.Window
        Me._txtIn_0.Cursor = System.Windows.Forms.Cursors.IBeam
        Me._txtIn_0.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me._txtIn_0.ForeColor = System.Drawing.SystemColors.WindowText
        Me.txtIn.SetIndex(Me._txtIn_0, CType(0, Short))
        Me._txtIn_0.Location = New System.Drawing.Point(46, 2)
        Me._txtIn_0.MaxLength = 2
        Me._txtIn_0.Name = "_txtIn_0"
        Me._txtIn_0.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me._txtIn_0.Size = New System.Drawing.Size(37, 20)
        Me._txtIn_0.TabIndex = 8
        '
        '_txtOut_3
        '
        Me._txtOut_3.AcceptsReturn = True
        Me._txtOut_3.BackColor = System.Drawing.SystemColors.Window
        Me._txtOut_3.Cursor = System.Windows.Forms.Cursors.IBeam
        Me._txtOut_3.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me._txtOut_3.ForeColor = System.Drawing.SystemColors.WindowText
        Me.txtOut.SetIndex(Me._txtOut_3, CType(3, Short))
        Me._txtOut_3.Location = New System.Drawing.Point(89, 65)
        Me._txtOut_3.MaxLength = 0
        Me._txtOut_3.Name = "_txtOut_3"
        Me._txtOut_3.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me._txtOut_3.Size = New System.Drawing.Size(35, 20)
        Me._txtOut_3.TabIndex = 3
        '
        '_txtOut_2
        '
        Me._txtOut_2.AcceptsReturn = True
        Me._txtOut_2.BackColor = System.Drawing.SystemColors.Window
        Me._txtOut_2.Cursor = System.Windows.Forms.Cursors.IBeam
        Me._txtOut_2.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me._txtOut_2.ForeColor = System.Drawing.SystemColors.WindowText
        Me.txtOut.SetIndex(Me._txtOut_2, CType(2, Short))
        Me._txtOut_2.Location = New System.Drawing.Point(89, 44)
        Me._txtOut_2.MaxLength = 0
        Me._txtOut_2.Name = "_txtOut_2"
        Me._txtOut_2.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me._txtOut_2.Size = New System.Drawing.Size(35, 20)
        Me._txtOut_2.TabIndex = 2
        '
        '_txtOut_1
        '
        Me._txtOut_1.AcceptsReturn = True
        Me._txtOut_1.BackColor = System.Drawing.SystemColors.Window
        Me._txtOut_1.Cursor = System.Windows.Forms.Cursors.IBeam
        Me._txtOut_1.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me._txtOut_1.ForeColor = System.Drawing.SystemColors.WindowText
        Me.txtOut.SetIndex(Me._txtOut_1, CType(1, Short))
        Me._txtOut_1.Location = New System.Drawing.Point(89, 23)
        Me._txtOut_1.MaxLength = 0
        Me._txtOut_1.Name = "_txtOut_1"
        Me._txtOut_1.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me._txtOut_1.Size = New System.Drawing.Size(36, 20)
        Me._txtOut_1.TabIndex = 1
        '
        '_txtOut_0
        '
        Me._txtOut_0.AcceptsReturn = True
        Me._txtOut_0.BackColor = System.Drawing.SystemColors.Window
        Me._txtOut_0.Cursor = System.Windows.Forms.Cursors.IBeam
        Me._txtOut_0.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me._txtOut_0.ForeColor = System.Drawing.SystemColors.WindowText
        Me.txtOut.SetIndex(Me._txtOut_0, CType(0, Short))
        Me._txtOut_0.Location = New System.Drawing.Point(89, 2)
        Me._txtOut_0.MaxLength = 0
        Me._txtOut_0.Name = "_txtOut_0"
        Me._txtOut_0.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me._txtOut_0.Size = New System.Drawing.Size(36, 20)
        Me._txtOut_0.TabIndex = 0
        '
        'tmrReadport
        '
        Me.tmrReadport.Interval = 1
        '
        'Image1
        '
        Me.Image1.Cursor = System.Windows.Forms.Cursors.Default
        Me.Image1.Location = New System.Drawing.Point(0, 0)
        Me.Image1.Name = "Image1"
        Me.Image1.Size = New System.Drawing.Size(32, 32)
        Me.Image1.TabIndex = 42
        Me.Image1.TabStop = False
        '
        'lblArtist
        '
        Me.lblArtist.BackColor = System.Drawing.SystemColors.Control
        Me.lblArtist.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblArtist.Cursor = System.Windows.Forms.Cursors.Default
        Me.lblArtist.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblArtist.ForeColor = System.Drawing.SystemColors.ControlText
        Me.lblArtist.Location = New System.Drawing.Point(64, 253)
        Me.lblArtist.Name = "lblArtist"
        Me.lblArtist.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.lblArtist.Size = New System.Drawing.Size(315, 14)
        Me.lblArtist.TabIndex = 21
        Me.lblArtist.Text = "Game:"
        '
        'lblSong
        '
        Me.lblSong.BackColor = System.Drawing.SystemColors.Control
        Me.lblSong.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblSong.Cursor = System.Windows.Forms.Cursors.Default
        Me.lblSong.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblSong.ForeColor = System.Drawing.SystemColors.ControlText
        Me.lblSong.Location = New System.Drawing.Point(64, 232)
        Me.lblSong.Name = "lblSong"
        Me.lblSong.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.lblSong.Size = New System.Drawing.Size(315, 14)
        Me.lblSong.TabIndex = 20
        Me.lblSong.Text = "Game:"
        '
        'lblGame
        '
        Me.lblGame.BackColor = System.Drawing.SystemColors.Control
        Me.lblGame.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblGame.Cursor = System.Windows.Forms.Cursors.Default
        Me.lblGame.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblGame.ForeColor = System.Drawing.SystemColors.ControlText
        Me.lblGame.Location = New System.Drawing.Point(64, 211)
        Me.lblGame.Name = "lblGame"
        Me.lblGame.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.lblGame.Size = New System.Drawing.Size(315, 14)
        Me.lblGame.TabIndex = 19
        Me.lblGame.Text = "Game:"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.BackColor = System.Drawing.SystemColors.Control
        Me.Label7.Cursor = System.Windows.Forms.Cursors.Default
        Me.Label7.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label7.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Label7.Location = New System.Drawing.Point(8, 253)
        Me.Label7.Name = "Label7"
        Me.Label7.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Label7.Size = New System.Drawing.Size(36, 14)
        Me.Label7.TabIndex = 18
        Me.Label7.Text = "Artist:"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.BackColor = System.Drawing.SystemColors.Control
        Me.Label6.Cursor = System.Windows.Forms.Cursors.Default
        Me.Label6.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label6.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Label6.Location = New System.Drawing.Point(10, 232)
        Me.Label6.Name = "Label6"
        Me.Label6.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Label6.Size = New System.Drawing.Size(35, 14)
        Me.Label6.TabIndex = 17
        Me.Label6.Text = "Song:"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.BackColor = System.Drawing.SystemColors.Control
        Me.Label5.Cursor = System.Windows.Forms.Cursors.Default
        Me.Label5.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label5.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Label5.Location = New System.Drawing.Point(10, 211)
        Me.Label5.Name = "Label5"
        Me.Label5.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Label5.Size = New System.Drawing.Size(38, 14)
        Me.Label5.TabIndex = 16
        Me.Label5.Text = "Game:"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.BackColor = System.Drawing.SystemColors.Control
        Me.Label4.Cursor = System.Windows.Forms.Cursors.Default
        Me.Label4.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Label4.Location = New System.Drawing.Point(3, 71)
        Me.Label4.Name = "Label4"
        Me.Label4.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Label4.Size = New System.Drawing.Size(37, 14)
        Me.Label4.TabIndex = 7
        Me.Label4.Text = "$2143"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.BackColor = System.Drawing.SystemColors.Control
        Me.Label3.Cursor = System.Windows.Forms.Cursors.Default
        Me.Label3.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Label3.Location = New System.Drawing.Point(3, 29)
        Me.Label3.Name = "Label3"
        Me.Label3.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Label3.Size = New System.Drawing.Size(37, 14)
        Me.Label3.TabIndex = 6
        Me.Label3.Text = "$2141"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.BackColor = System.Drawing.SystemColors.Control
        Me.Label2.Cursor = System.Windows.Forms.Cursors.Default
        Me.Label2.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Label2.Location = New System.Drawing.Point(3, 8)
        Me.Label2.Name = "Label2"
        Me.Label2.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Label2.Size = New System.Drawing.Size(37, 14)
        Me.Label2.TabIndex = 5
        Me.Label2.Text = "$2140"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.BackColor = System.Drawing.SystemColors.Control
        Me.Label1.Cursor = System.Windows.Forms.Cursors.Default
        Me.Label1.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Label1.Location = New System.Drawing.Point(3, 50)
        Me.Label1.Name = "Label1"
        Me.Label1.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Label1.Size = New System.Drawing.Size(37, 14)
        Me.Label1.TabIndex = 4
        Me.Label1.Text = "$2142"
        '
        'txtIn
        '
        '
        '_TxtIn_4
        '
        Me._TxtIn_4.AcceptsReturn = True
        Me._TxtIn_4.BackColor = System.Drawing.SystemColors.Window
        Me._TxtIn_4.Cursor = System.Windows.Forms.Cursors.IBeam
        Me._TxtIn_4.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me._TxtIn_4.ForeColor = System.Drawing.SystemColors.WindowText
        Me.txtIn.SetIndex(Me._TxtIn_4, CType(4, Short))
        Me._TxtIn_4.Location = New System.Drawing.Point(310, 448)
        Me._TxtIn_4.MaxLength = 2
        Me._TxtIn_4.Name = "_TxtIn_4"
        Me._TxtIn_4.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me._TxtIn_4.Size = New System.Drawing.Size(37, 20)
        Me._TxtIn_4.TabIndex = 51
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.RadioButton4)
        Me.GroupBox1.Controls.Add(Me.RadioButton3)
        Me.GroupBox1.Controls.Add(Me.RadioButton2)
        Me.GroupBox1.Controls.Add(Me.RadioButton1)
        Me.GroupBox1.Location = New System.Drawing.Point(119, 117)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(118, 89)
        Me.GroupBox1.TabIndex = 43
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Free Space Search"
        '
        'RadioButton4
        '
        Me.RadioButton4.AutoSize = True
        Me.RadioButton4.Location = New System.Drawing.Point(11, 48)
        Me.RadioButton4.Name = "RadioButton4"
        Me.RadioButton4.Size = New System.Drawing.Size(85, 18)
        Me.RadioButton4.TabIndex = 3
        Me.RadioButton4.Text = "Echo Region"
        Me.RadioButton4.UseVisualStyleBackColor = True
        '
        'RadioButton3
        '
        Me.RadioButton3.AutoSize = True
        Me.RadioButton3.Location = New System.Drawing.Point(11, 65)
        Me.RadioButton3.Name = "RadioButton3"
        Me.RadioButton3.Size = New System.Drawing.Size(88, 18)
        Me.RadioButton3.TabIndex = 2
        Me.RadioButton3.Text = "32xFF 32x00"
        Me.RadioButton3.UseVisualStyleBackColor = True
        '
        'RadioButton2
        '
        Me.RadioButton2.AutoSize = True
        Me.RadioButton2.Location = New System.Drawing.Point(11, 33)
        Me.RadioButton2.Name = "RadioButton2"
        Me.RadioButton2.Size = New System.Drawing.Size(87, 18)
        Me.RadioButton2.TabIndex = 1
        Me.RadioButton2.TabStop = True
        Me.RadioButton2.Text = "FF to 00 only"
        Me.RadioButton2.UseVisualStyleBackColor = True
        '
        'RadioButton1
        '
        Me.RadioButton1.AutoSize = True
        Me.RadioButton1.Checked = True
        Me.RadioButton1.Location = New System.Drawing.Point(11, 16)
        Me.RadioButton1.Name = "RadioButton1"
        Me.RadioButton1.Size = New System.Drawing.Size(48, 18)
        Me.RadioButton1.TabIndex = 0
        Me.RadioButton1.TabStop = True
        Me.RadioButton1.Text = "Auto"
        Me.RadioButton1.UseVisualStyleBackColor = True
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(153, 30)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(88, 26)
        Me.Button1.TabIndex = 44
        Me.Button1.Text = "Load Next"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'Button2
        '
        Me.Button2.Location = New System.Drawing.Point(0, 30)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(76, 26)
        Me.Button2.TabIndex = 45
        Me.Button2.Text = "Load Prev"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'ListBox1
        '
        Me.ListBox1.FormattingEnabled = True
        Me.ListBox1.HorizontalScrollbar = True
        Me.ListBox1.ItemHeight = 14
        Me.ListBox1.Location = New System.Drawing.Point(4, 192)
        Me.ListBox1.Name = "ListBox1"
        Me.ListBox1.Size = New System.Drawing.Size(571, 186)
        Me.ListBox1.TabIndex = 46
        Me.ListBox1.Visible = False
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.Label2)
        Me.Panel1.Controls.Add(Me._txtIn_0)
        Me.Panel1.Controls.Add(Me._txtOut_0)
        Me.Panel1.Controls.Add(Me.Label3)
        Me.Panel1.Controls.Add(Me._txtIn_1)
        Me.Panel1.Controls.Add(Me._txtOut_1)
        Me.Panel1.Controls.Add(Me.Label1)
        Me.Panel1.Controls.Add(Me.Label4)
        Me.Panel1.Controls.Add(Me._txtIn_2)
        Me.Panel1.Controls.Add(Me._txtOut_2)
        Me.Panel1.Controls.Add(Me._txtIn_3)
        Me.Panel1.Controls.Add(Me._txtOut_3)
        Me.Panel1.Location = New System.Drawing.Point(4, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(128, 89)
        Me.Panel1.TabIndex = 47
        '
        'Panel2
        '
        Me.Panel2.Controls.Add(Me.HScroll1)
        Me.Panel2.Controls.Add(Me.Button3)
        Me.Panel2.Controls.Add(Me.Button2)
        Me.Panel2.Controls.Add(Me.cmdReset)
        Me.Panel2.Controls.Add(Me.Button1)
        Me.Panel2.Controls.Add(Me.cmdLoadPrev)
        Me.Panel2.Controls.Add(Me.cmdLoadAPU)
        Me.Panel2.Controls.Add(Me.PauseUpload)
        Me.Panel2.Location = New System.Drawing.Point(138, 0)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(248, 83)
        Me.Panel2.TabIndex = 48
        '
        'Button3
        '
        Me.Button3.Location = New System.Drawing.Point(151, 59)
        Me.Button3.Name = "Button3"
        Me.Button3.Size = New System.Drawing.Size(89, 24)
        Me.Button3.TabIndex = 46
        Me.Button3.Text = "Add to List"
        Me.Button3.UseVisualStyleBackColor = True
        '
        'Panel3
        '
        Me.Panel3.AutoSize = True
        Me.Panel3.Controls.Add(Me.lblComment)
        Me.Panel3.Controls.Add(Me.chkAutoPlay)
        Me.Panel3.Controls.Add(Me.Label10)
        Me.Panel3.Controls.Add(Me.Label15)
        Me.Panel3.Controls.Add(Me.lblFileName)
        Me.Panel3.Controls.Add(Me.Label13)
        Me.Panel3.Controls.Add(Me.lblPublisher)
        Me.Panel3.Controls.Add(Me.Label11)
        Me.Panel3.Controls.Add(Me.lblOST)
        Me.Panel3.Controls.Add(Me.Label9)
        Me.Panel3.Controls.Add(Me.lblDumper)
        Me.Panel3.Controls.Add(Me._TxtIn_4)
        Me.Panel3.Controls.Add(Me.Label8)
        Me.Panel3.Controls.Add(Me.Text1)
        Me.Panel3.Controls.Add(Me.Label7)
        Me.Panel3.Controls.Add(Me.lblArtist)
        Me.Panel3.Controls.Add(Me.txtUploadSpeed)
        Me.Panel3.Controls.Add(Me.txtReadSpeed)
        Me.Panel3.Controls.Add(Me.Label6)
        Me.Panel3.Controls.Add(Me.Label5)
        Me.Panel3.Controls.Add(Me.lblSong)
        Me.Panel3.Controls.Add(Me.GroupBox1)
        Me.Panel3.Controls.Add(Me.lblGame)
        Me.Panel3.Controls.Add(Me.Panel1)
        Me.Panel3.Controls.Add(Me.Command2)
        Me.Panel3.Controls.Add(Me.Panel2)
        Me.Panel3.Controls.Add(Me.apuLoad)
        Me.Panel3.Controls.Add(Me.Combo1)
        Me.Panel3.Controls.Add(Me.Frame1)
        Me.Panel3.Controls.Add(Me.cmdOpenPort)
        Me.Panel3.Controls.Add(Me.Initialize)
        Me.Panel3.Dock = System.Windows.Forms.DockStyle.Left
        Me.Panel3.Location = New System.Drawing.Point(0, 0)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Size = New System.Drawing.Size(390, 520)
        Me.Panel3.TabIndex = 49
        '
        'lblComment
        '
        Me.lblComment.Location = New System.Drawing.Point(64, 358)
        Me.lblComment.Multiline = True
        Me.lblComment.Name = "lblComment"
        Me.lblComment.ReadOnly = True
        Me.lblComment.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.lblComment.Size = New System.Drawing.Size(315, 59)
        Me.lblComment.TabIndex = 63
        '
        'chkAutoPlay
        '
        Me.chkAutoPlay.AutoSize = True
        Me.chkAutoPlay.Location = New System.Drawing.Point(190, 480)
        Me.chkAutoPlay.Name = "chkAutoPlay"
        Me.chkAutoPlay.Size = New System.Drawing.Size(72, 18)
        Me.chkAutoPlay.TabIndex = 62
        Me.chkAutoPlay.Text = "Auto Play"
        Me.chkAutoPlay.UseVisualStyleBackColor = True
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.BackColor = System.Drawing.SystemColors.Control
        Me.Label10.Cursor = System.Windows.Forms.Cursors.Default
        Me.Label10.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label10.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Label10.Location = New System.Drawing.Point(7, 358)
        Me.Label10.Name = "Label10"
        Me.Label10.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Label10.Size = New System.Drawing.Size(54, 14)
        Me.Label10.TabIndex = 60
        Me.Label10.Text = "Comment:"
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.BackColor = System.Drawing.SystemColors.Control
        Me.Label15.Cursor = System.Windows.Forms.Cursors.Default
        Me.Label15.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label15.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Label15.Location = New System.Drawing.Point(8, 337)
        Me.Label15.Name = "Label15"
        Me.Label15.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Label15.Size = New System.Drawing.Size(52, 14)
        Me.Label15.TabIndex = 58
        Me.Label15.Text = "Filename:"
        '
        'lblFileName
        '
        Me.lblFileName.BackColor = System.Drawing.SystemColors.Control
        Me.lblFileName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblFileName.Cursor = System.Windows.Forms.Cursors.Default
        Me.lblFileName.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblFileName.ForeColor = System.Drawing.SystemColors.ControlText
        Me.lblFileName.Location = New System.Drawing.Point(64, 337)
        Me.lblFileName.Name = "lblFileName"
        Me.lblFileName.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.lblFileName.Size = New System.Drawing.Size(315, 14)
        Me.lblFileName.TabIndex = 59
        Me.lblFileName.Text = "Game:"
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.BackColor = System.Drawing.SystemColors.Control
        Me.Label13.Cursor = System.Windows.Forms.Cursors.Default
        Me.Label13.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label13.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Label13.Location = New System.Drawing.Point(8, 316)
        Me.Label13.Name = "Label13"
        Me.Label13.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Label13.Size = New System.Drawing.Size(54, 14)
        Me.Label13.TabIndex = 56
        Me.Label13.Text = "Publisher:"
        '
        'lblPublisher
        '
        Me.lblPublisher.BackColor = System.Drawing.SystemColors.Control
        Me.lblPublisher.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblPublisher.Cursor = System.Windows.Forms.Cursors.Default
        Me.lblPublisher.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblPublisher.ForeColor = System.Drawing.SystemColors.ControlText
        Me.lblPublisher.Location = New System.Drawing.Point(64, 316)
        Me.lblPublisher.Name = "lblPublisher"
        Me.lblPublisher.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.lblPublisher.Size = New System.Drawing.Size(315, 14)
        Me.lblPublisher.TabIndex = 57
        Me.lblPublisher.Text = "Game:"
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.BackColor = System.Drawing.SystemColors.Control
        Me.Label11.Cursor = System.Windows.Forms.Cursors.Default
        Me.Label11.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label11.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Label11.Location = New System.Drawing.Point(8, 295)
        Me.Label11.Name = "Label11"
        Me.Label11.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Label11.Size = New System.Drawing.Size(30, 14)
        Me.Label11.TabIndex = 54
        Me.Label11.Text = "OST:"
        '
        'lblOST
        '
        Me.lblOST.BackColor = System.Drawing.SystemColors.Control
        Me.lblOST.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblOST.Cursor = System.Windows.Forms.Cursors.Default
        Me.lblOST.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblOST.ForeColor = System.Drawing.SystemColors.ControlText
        Me.lblOST.Location = New System.Drawing.Point(64, 295)
        Me.lblOST.Name = "lblOST"
        Me.lblOST.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.lblOST.Size = New System.Drawing.Size(315, 14)
        Me.lblOST.TabIndex = 55
        Me.lblOST.Text = "Game:"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.BackColor = System.Drawing.SystemColors.Control
        Me.Label9.Cursor = System.Windows.Forms.Cursors.Default
        Me.Label9.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label9.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Label9.Location = New System.Drawing.Point(8, 274)
        Me.Label9.Name = "Label9"
        Me.Label9.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Label9.Size = New System.Drawing.Size(47, 14)
        Me.Label9.TabIndex = 52
        Me.Label9.Text = "Dumper:"
        '
        'lblDumper
        '
        Me.lblDumper.BackColor = System.Drawing.SystemColors.Control
        Me.lblDumper.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblDumper.Cursor = System.Windows.Forms.Cursors.Default
        Me.lblDumper.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblDumper.ForeColor = System.Drawing.SystemColors.ControlText
        Me.lblDumper.Location = New System.Drawing.Point(64, 274)
        Me.lblDumper.Name = "lblDumper"
        Me.lblDumper.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.lblDumper.Size = New System.Drawing.Size(315, 14)
        Me.lblDumper.TabIndex = 53
        Me.lblDumper.Text = "Game:"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(187, 454)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(120, 14)
        Me.Label8.TabIndex = 50
        Me.Label8.Text = "SPC Driver Song Select"
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
        Me.SplitContainer1.Panel1.Controls.Add(Me.Panel3)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.ListBox3)
        Me.SplitContainer1.Panel2.Controls.Add(Me.ListBox2)
        Me.SplitContainer1.Panel2.Controls.Add(Me.ListBox1)
        Me.SplitContainer1.Size = New System.Drawing.Size(1087, 520)
        Me.SplitContainer1.SplitterDistance = 388
        Me.SplitContainer1.TabIndex = 50
        '
        'ListBox3
        '
        Me.ListBox3.FormattingEnabled = True
        Me.ListBox3.HorizontalScrollbar = True
        Me.ListBox3.ItemHeight = 14
        Me.ListBox3.Location = New System.Drawing.Point(112, 191)
        Me.ListBox3.Name = "ListBox3"
        Me.ListBox3.Size = New System.Drawing.Size(571, 186)
        Me.ListBox3.TabIndex = 48
        Me.ListBox3.Visible = False
        '
        'ListBox2
        '
        Me.ListBox2.Dock = System.Windows.Forms.DockStyle.Top
        Me.ListBox2.FormattingEnabled = True
        Me.ListBox2.ItemHeight = 14
        Me.ListBox2.Location = New System.Drawing.Point(0, 0)
        Me.ListBox2.Name = "ListBox2"
        Me.ListBox2.Size = New System.Drawing.Size(695, 186)
        Me.ListBox2.TabIndex = 47
        '
        'tmrAutoPlay
        '
        Me.tmrAutoPlay.Enabled = True
        Me.tmrAutoPlay.Interval = 250
        '
        'Form1
        '
        Me.AllowDrop = True
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 14.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.Control
        Me.ClientSize = New System.Drawing.Size(1087, 520)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Controls.Add(Me.Image1)
        Me.Cursor = System.Windows.Forms.Cursors.Default
        Me.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Location = New System.Drawing.Point(4, 23)
        Me.Name = "Form1"
        Me.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Text = "APU Play Win"
        Me.Frame1.ResumeLayout(False)
        CType(Me.Image1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.txtIn, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.txtOut, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.Panel2.ResumeLayout(False)
        Me.Panel3.ResumeLayout(False)
        Me.Panel3.PerformLayout()
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel1.PerformLayout()
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        Me.SplitContainer1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents RadioButton4 As System.Windows.Forms.RadioButton
    Friend WithEvents RadioButton3 As System.Windows.Forms.RadioButton
    Friend WithEvents RadioButton2 As System.Windows.Forms.RadioButton
    Friend WithEvents RadioButton1 As System.Windows.Forms.RadioButton
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents Button2 As System.Windows.Forms.Button
    Friend WithEvents ListBox1 As System.Windows.Forms.ListBox
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents Panel2 As System.Windows.Forms.Panel
    Friend WithEvents Panel3 As System.Windows.Forms.Panel
    Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
    Friend WithEvents ListBox2 As System.Windows.Forms.ListBox
    Friend WithEvents Button3 As System.Windows.Forms.Button
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Public WithEvents _TxtIn_4 As System.Windows.Forms.TextBox
    Friend WithEvents ListBox3 As System.Windows.Forms.ListBox
    Public WithEvents Label15 As System.Windows.Forms.Label
    Public WithEvents lblFileName As System.Windows.Forms.Label
    Public WithEvents Label13 As System.Windows.Forms.Label
    Public WithEvents lblPublisher As System.Windows.Forms.Label
    Public WithEvents Label11 As System.Windows.Forms.Label
    Public WithEvents lblOST As System.Windows.Forms.Label
    Public WithEvents Label9 As System.Windows.Forms.Label
    Public WithEvents lblDumper As System.Windows.Forms.Label
    Public WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents chkAutoPlay As System.Windows.Forms.CheckBox
    Friend WithEvents tmrAutoPlay As System.Windows.Forms.Timer
    Friend WithEvents lblComment As System.Windows.Forms.TextBox
#End Region
End Class