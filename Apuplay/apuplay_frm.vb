Option Strict Off
Option Explicit On

Imports System.IO
Imports System.Text
Imports VB = Microsoft.VisualBasic
Friend Class Form1
    Inherits Form

    Private ReadOnly _onBits(31) As Integer


    Private Declare Function ReadSPC700 Lib "APU_DLL.DLL" (ByVal Address As Integer) As Integer
    Private Declare Function WriteSPC700 Lib "APU_DLL.DLL" (ByVal Address As Integer, ByVal data As Integer) As Integer
    Private Declare Function WriteSPC700_WP0I Lib "APU_DLL.DLL" (ByVal Address As Integer, ByVal data As Integer) As Integer
    Private Declare Function SetPort0 Lib "APU_DLL.DLL" (ByVal data As Short) As Integer



    Private Declare Function Write16bytes Lib "APU_DLL.DLL" (ByVal byte1 As Byte, ByVal byte2 As Byte, ByVal byte3 As Byte, ByVal byte4 As Byte, ByVal byte5 As Byte, ByVal byte6 As Byte, ByVal byte7 As Byte, ByVal byte8 As Byte, ByVal byte9 As Byte, ByVal byte10 As Byte, ByVal byte11 As Byte, ByVal byte12 As Byte, ByVal byte13 As Byte, ByVal byte14 As Byte, ByVal byte15 As Byte, ByVal byte16 As Byte) As Integer
    Private Declare Function StartWrite16bytes Lib "APU_DLL.DLL" (ByVal byte1 As Byte, ByVal byte2 As Byte, ByVal byte3 As Byte, ByVal byte4 As Byte) As Integer
    Private Declare Function FinishWrite16bytes Lib "APU_DLL.DLL" () As Integer
    Private Declare Function ResetAPU Lib "APU_DLL.DLL" () As Integer

    Private Declare Function init_port Lib "APU_DLL.DLL" (ByVal port As String) As Integer
    Private Declare Function OpenPort_VB Lib "APU_DLL.DLL" (ByVal port As String) As Integer
    Private Declare Function ClosePort Lib "APU_DLL.DLL" () As Integer

    Private Declare Function StartSPC700 Lib "APU_DLL.DLL" (ByVal byte1 As Byte) As Integer
    Private Declare Function FlushRead Lib "APU_DLL.DLL" () As Integer

    Private Declare Function UploadSPC Lib "APU_DLL.DLL" (ByRef spcramdata As Byte, ByRef DSPdata As Byte, ByRef SPCExtraRam As Byte, ByVal A_reg As Byte, ByVal X_reg As Byte, ByVal Y_reg As Byte, ByVal SP_reg As Byte, ByVal SW_reg As Byte, ByVal PCL_reg As Byte, ByVal PCH_reg As Byte, ByVal Echo_Clear As Byte) As Integer

    Private Declare Function CreateID666 Lib "APU_DLL.DLL" () As Long
    Private Declare Sub DestroyID666 Lib "APU_DLL.DLL" (ByRef objptr As Long)
    Private Declare Function ID666_LoadTag Lib "APU_DLL.DLL" (ByRef objptr As Long, ByRef filedata() As Byte, ByVal filesize As Long) As Byte
    Private Declare Function ID666_ToStr Lib "APU_DLL.DLL" (ByRef objptr As Long, ByRef tagstr() As Byte, ByRef TagFormat() As Byte) As Byte


    Public Enum ID6Type
        ID6Unk = -1                            'Unknown file type
        ID6Err                                 'Error opening file
        ID6SPC                                 'Normal SPC
        ID6Ext                                 'SPC with extended ID666 tag
        ID6Zst                                 'ZSNES saved state
        ID6Sp2                                 'Kevtris SPC2 format. http://blog.kevtris.org/blogfiles/spc2_file_specification_v1.txt
        ID6Rom = 64                                'SNES ROM image
        ID6Swc                                 '  " " Super WildCard
        ID6Fig                                 '  " " Pro Fighter
        ID6Sf3                                 '  " " Game Doctor SF III
        ID6Romh                                    '  " " dumper header unknown
    End Enum

    Public Enum IDX6TagType
        Song_Name = 1
        Game_Name
        Artist_Name
        Dumper_Name
        Date_Dumped
        Emulator_Used
        Comment
        OST_Title = 16
        OST_Disc
        OST_Track
        Publisher_Name
        Copyright_Year
        Intro_Len = 48
        Loop_Len
        End_Len
        Fade_Len
        Muted_Channels
        Loop_Count
        Amp_Val
    End Enum

    Public Enum SPC2TagType
        EOD = 0
        Song_Name
        Game_Name
        Artist_Name
        Dumper_Name
        Comment
        OST_Title
        Publisher_Name
        SPC_Filename
    End Enum
    'Private Declare Function WriteCompressedByte Lib "APU_DLL.DLL" (ByVal byte1 As Byte, ByVal byte2 As Byte, ByVal byte3 As Byte, ByVal byte4 As Byte, ByVal compressedlen As Integer) As Integer


    ReadOnly _hackbytes(3) As Byte
    Dim _readcount As Integer
    Dim _echoclear As Integer
    ' ReSharper disable once InconsistentNaming
    Dim _DSPdata(127) As Byte
    Dim _strselectedspc As String
    Dim _intselectedspctrack As Integer
    Dim _strcurrentspc As String
    Dim _intcurrentspctrack As Integer
    Dim _listcleared As Boolean = False
    Dim _freespacesearch As Byte = 0

    Dim _uploadstate As Boolean
    Dim _usedBootcode As Boolean
    Dim _uploadcomplete As Boolean
    Dim _justUploaded As Boolean

    Dim _playtime As Integer

    Dim _spcinportiszero As Boolean

    Sub Debugprint(ByVal debugstr As String)
        Debug.Print(debugstr)
    End Sub

    Private Shared Sub RaiseHwError(i As Integer)
        If i = 1 Then Err.Raise(-2000002, , "Communication port to APU hardware not opened")
        If i = 2 Then Err.Raise(-2000001, , "Error Loading SPC")
    End Sub

    Private Shared Function ResetAPU_internal() As Integer
        RaiseHwError(ResetAPU)
    End Function


    Private Shared Function WriteSPC700_internal(ByVal Address As Integer, ByVal data As Integer) As Integer
        RaiseHwError(WriteSPC700(Address, data))
    End Function

    ' ReSharper disable once UnusedMember.Local
    Private Function WriteSPC700_internal_WP0I(ByVal Address As Integer, ByVal data As Integer) As Integer
        RaiseHwError(WriteSPC700_WP0I(Address, data))
    End Function

    Private Function ReadSPC700_internal(ByVal Address As Integer) As Integer
        Dim data As Integer
        data = ReadSPC700(Address)
        If (data < 0) Or (data > 255) Then
            If data = -1 Then RaiseHwError(1)
            RaiseHwError(2)
        End If
        ReadSPC700_internal = data
    End Function



    Private Sub cmdOpenPort_Click(ByVal eventSender As Object, ByVal eventArgs As EventArgs) Handles cmdOpenPort.Click
        Dim i As Integer
        i = init_port(Combo1.Text)
        Select Case i
            Case 0  'Sccessful open
                txtUploadSpeed.Text = "Port " & Combo1.Text & " opened successfully"
                cmdOpenPort.Enabled = False
                Command2.Enabled = False
                Combo1.Enabled = False
                Initialize.Enabled = True
            Case 1  'COM port in use or doesn't exist
                txtUploadSpeed.Text = "Error opening port " & Combo1.Text
            Case 2  'APU hardware not on Com port
                txtUploadSpeed.Text = "No APU interface on " & Combo1.Text
            Case 3  'Error communicating with APU hardware.
                txtUploadSpeed.Text = "Error communication with APU interface on " & Combo1.Text
            Case Is > 3
                txtUploadSpeed.Text = "Error on byte " & i
        End Select
    End Sub

    Dim LoadAPU_Cancelled As Boolean
    Private Sub cmdLoadAPU_Click(ByVal eventSender As Object, ByVal eventArgs As EventArgs) Handles cmdLoadAPU.Click
        On Error GoTo handleerror
        _listcleared = True
        LoadAPU_Cancelled = False
        Button3_Click(eventSender, eventArgs)
        If (LoadAPU_Cancelled) Then
            _listcleared = False
            Exit Sub
        End If
        _strselectedspc = ListBox1.Items.Item(0).ToString()
        _intselectedspctrack = Int32.Parse(ListBox3.Items.Item(0).ToString())
        ListBox2.SelectedIndex = 0
        'frmToP.allow_top_events(False)
        LoadAPU(_strselectedspc, _intselectedspctrack)
handleerror:
    End Sub

    Private Sub cmdLoadPrev_Click(ByVal eventSender As Object, ByVal eventArgs As EventArgs) Handles cmdLoadPrev.Click
        If (_strcurrentspc = "") Then
            _strcurrentspc = _strselectedspc
            _intcurrentspctrack = _intselectedspctrack
            If (_strcurrentspc = "") Then
                Exit Sub
            End If
        End If
        LoadAPU(_strcurrentspc, _intcurrentspctrack)
    End Sub

    Private Sub cmdReset_Click(ByVal eventSender As Object, ByVal eventArgs As EventArgs) Handles cmdReset.Click
        On Error Resume Next
        ResetAPU_internal()
        tmrAutoPlay.Enabled = False
        _playtime = 0
    End Sub

    Private Sub Command1_Click(ByVal eventSender As Object, ByVal eventArgs As EventArgs)
        'If DSPdata(&HC) >= 0 Then
        '	Slider1.Value = DSPdata(&HC)
        'End If
        'If DSPdata(&H1C) >= 0 Then
        '	Slider2.Value = DSPdata(&H1C)
        'End If
        'If DSPdata(&H2C) >= 0 Then
        '	Slider3.Value = DSPdata(&H2C)
        'End If
        'If DSPdata(&H3C) >= 0 Then
        '	Slider4.Value = DSPdata(&H3C)
        'End If

    End Sub

    Private Sub Command2_Click(ByVal eventSender As Object, ByVal eventArgs As EventArgs) Handles Command2.Click
        On Error GoTo errorhandler
        Dim i As Short
        Combo1.Items.Clear()
        For i = 1 To 256
            If OpenPort_VB("COM" & i) = 0 Then
                ClosePort()
                Combo1.Items.Add("COM" & i)
            End If
        Next  'i
        Combo1.SelectedIndex = 0
        Combo1.Enabled = True
        cmdOpenPort.Enabled = True
        Exit Sub
errorhandler:
        Combo1.Enabled = False
        cmdOpenPort.Enabled = False
        MsgBox("Error: No COM ports available. Close any applications that may be using them.", , "APU Play win")
    End Sub

    Private Sub FinishUpload()
        Dim i As Integer
        If _uploadcomplete Then
            PauseUpload.Hide()
            i = ReadSPC700_internal(0)
            i = i + 2
            WriteSPC700_internal(0, i)
            If _usedBootcode Then
                If Not _spcinportiszero Then
                    If Not waitInport(0, &H53, 512) Then Err.Raise(-2000004, , "Error Loading SPC")
                Else
                    WriteSPC700(3, 1)
                    WriteSPC700(0, 1)
                    If Not waitInport(0, &H53, 512) Then Err.Raise(-2000004, , "Error Loading SPC")
                End If
                WriteSPC700(0, _hackbytes(0))
                WriteSPC700(1, _hackbytes(1))
                WriteSPC700(2, _hackbytes(2))
                WriteSPC700(3, _hackbytes(3))
            End If
            _justUploaded = True
            _uploadcomplete = False
            Text1_TextChanged(Text1, New EventArgs())
            hackgame()
            enablebuttons(True)
            'frmSongSelect.checkgame()
        End If
    End Sub

    Private Sub PauseUpload_Click(ByVal eventSender As Object, ByVal eventArgs As EventArgs) Handles PauseUpload.Click
        On Error GoTo errorhandler
        _uploadstate = Not _uploadstate
        If _uploadstate = True Then
            PauseUpload.Text = "Pause Upload"
            FinishUpload()
        Else
            PauseUpload.Text = "Resume Upload"
        End If
        Exit Sub
errorhandler:
        MsgBox("SPC file upload failed", , "APU Play win")
        enablebuttons(True)
    End Sub

    Private Sub form1_Load(ByVal eventSender As Object, ByVal eventArgs As EventArgs) Handles MyBase.Load
        On Error GoTo errorhandler
        Dim i As Short
        'Slider1.Value = -1
        'Slider2.Value = -1
        'Slider3.Value = -1
        'Slider4.Value = -1
        If Option1.Checked = True Then _echoclear = 1
        If Option2.Checked = True Then _echoclear = 2
        If Option3.Checked = True Then _echoclear = 0

        If (ListBox2.Dock() = DockStyle.None) Then
            ListBox1.Visible = True
        Else
            ListBox2.Dock = DockStyle.Fill
        End If

        'If (init_port("COM3") <> 0) Then Unload form1
        For i = 1 To 256
            If OpenPort_VB("COM" & i) = 0 Then
                ClosePort()
                Combo1.Items.Add("COM" & i)
            End If
        Next  'i
        Combo1.SelectedIndex = 0
        Exit Sub
errorhandler:
        Combo1.Enabled = False
        cmdOpenPort.Enabled = False
        MsgBox("Error: No COM ports available. Close any applications that may be using them.", , "APU Play win")
    End Sub

    'UPGRADE_ISSUE: VBRUN.DataObject type was not upgraded. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6B85A2A7-FE9F-4FBE-AA0C-CF11AC86A305"'
    'UPGRADE_ISSUE: Form event Form.OLEDragDrop was not upgraded. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="ABD9AF39-7E24-4AFF-AD8D-3675C1AA3054"'
    Private Sub Form_OLEDragDrop(ByRef data As Object, ByRef Effect As Integer, ByRef Button As Short, ByRef Shift As Short, ByRef X As Single, ByRef Y As Single)
        'UPGRADE_ISSUE: DataObject property data.Files was not upgraded. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="076C26E5-B7A9-4E77-B69C-B4448DF39E58"'
        'UPGRADE_ISSUE: DataObjectFiles property Files.count was not upgraded. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="076C26E5-B7A9-4E77-B69C-B4448DF39E58"'
        debugprint(data.Files.count)
        'UPGRADE_ISSUE: DataObject property data.Files was not upgraded. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="076C26E5-B7A9-4E77-B69C-B4448DF39E58"'
        'UPGRADE_ISSUE: DataObjectFiles property Files.Item was not upgraded. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="076C26E5-B7A9-4E77-B69C-B4448DF39E58"'
        debugprint(data.Files.Item(1))
        'UPGRADE_ISSUE: DataObject property data.Files was not upgraded. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="076C26E5-B7A9-4E77-B69C-B4448DF39E58"'
        'UPGRADE_ISSUE: DataObjectFiles property Files.count was not upgraded. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="076C26E5-B7A9-4E77-B69C-B4448DF39E58"'
        If data.Files.count >= 1 Then
            'UPGRADE_ISSUE: DataObject property data.Files was not upgraded. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="076C26E5-B7A9-4E77-B69C-B4448DF39E58"'
            'UPGRADE_ISSUE: DataObjectFiles property Files.Item was not upgraded. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="076C26E5-B7A9-4E77-B69C-B4448DF39E58"'
            If LCase(VB.Right(data.Files.Item(1), 4)) = ".spc" Then
                'UPGRADE_ISSUE: DataObject property data.Files was not upgraded. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="076C26E5-B7A9-4E77-B69C-B4448DF39E58"'
                'UPGRADE_ISSUE: DataObjectFiles property Files.Item was not upgraded. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="076C26E5-B7A9-4E77-B69C-B4448DF39E58"'
                LoadAPU(data.Files.Item(1))
            End If
            If LCase(VB.Right(data.Files.Item(1), 4)) = ".sp2" Then
                LoadAPU(data.Files.Item(1), 0)
            End If
        End If

    End Sub

    'UPGRADE_NOTE: Form_Terminate was upgraded to Form_Terminate_Renamed. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
    'UPGRADE_WARNING: form1 event Form.Terminate has a new behavior. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"'
    Private Sub Form_Terminate_Renamed()
        ClosePort()
    End Sub

    Private Sub form1_FormClosed(ByVal eventSender As Object, ByVal eventArgs As FormClosedEventArgs) Handles Me.FormClosed
        'frmToP.Close()
        ClosePort()
    End Sub



    'UPGRADE_NOTE: HScroll1.Change was changed from an event to a procedure. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="4E2DC008-5EDA-4547-8317-C9316952674F"'
    'UPGRADE_WARNING: HScrollBar event HScroll1.Change has a new behavior. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"'
    Private Sub HScroll1_Change(ByVal newScrollValue As Integer)
        'UPGRADE_WARNING: Timer property tmrReadport.Interval cannot have a value of 0. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="169ECF4A-1968-402D-B243-16603CC08604"'
        If (newScrollValue > 0) Then
            tmrReadport.Enabled = True
            tmrReadport.Interval = newScrollValue
        Else
            tmrReadport.Enabled = False
        End If
    End Sub




    Private Sub Initialize_Click(ByVal eventSender As Object, ByVal eventArgs As EventArgs) Handles Initialize.Click
        'Dim timerinit As Single

        '    timerinit = Timer
        ClosePort()
        txtUploadSpeed.Text = "Port closed successfully"
        cmdOpenPort.Enabled = True
        Command2.Enabled = True
        Combo1.Enabled = True
        Initialize.Enabled = False
        enablebuttons(True)
        'Do Until (Timer - timerinit) > 120
        '        If ReadSPC700_internal(0) <> &HAA Then

        '            ResetAPU_internal
        '            timerinit = Timer
        '        Else
        '            readcount = readcount + 1
        '        End If

        '    txtUploadSpeed.Text = 120 - (Timer - timerinit) & "  seconds to go."
        '    DoEvents

        'Loop
        'cmdHide_Click
        'txtUploadSpeed.Text = "Parallel port Initialized."
    End Sub



    'UPGRADE_WARNING: Event Option1.CheckedChanged may fire when form is initialized. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="88B12AE1-6DE0-48A0-86F1-60C0686C026A"'
    Private Sub Option1_CheckedChanged(ByVal eventSender As Object, ByVal eventArgs As EventArgs) Handles Option1.CheckedChanged
        If eventSender.Checked Then
            _echoclear = 1
        End If
    End Sub

    'UPGRADE_WARNING: Event Option2.CheckedChanged may fire when form is initialized. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="88B12AE1-6DE0-48A0-86F1-60C0686C026A"'
    Private Sub Option2_CheckedChanged(ByVal eventSender As Object, ByVal eventArgs As EventArgs) Handles Option2.CheckedChanged
        If eventSender.Checked Then
            _echoclear = 2
        End If
    End Sub

    'UPGRADE_WARNING: Event Option3.CheckedChanged may fire when form is initialized. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="88B12AE1-6DE0-48A0-86F1-60C0686C026A"'
    Private Sub Option3_CheckedChanged(ByVal eventSender As Object, ByVal eventArgs As EventArgs) Handles Option3.CheckedChanged
        If eventSender.Checked Then
            _echoclear = 0
        End If
    End Sub

    'UPGRADE_WARNING: Event Text1.TextChanged may fire when form is initialized. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="88B12AE1-6DE0-48A0-86F1-60C0686C026A"'
    Private Sub Text1_TextChanged(ByVal eventSender As Object, ByVal eventArgs As EventArgs) Handles Text1.TextChanged
        Dim i As Integer
        Dim location As Integer
        Dim addr As Short
        Dim data As Short

        Dim timeout As Double

        location = Text1.SelectionStart()

        'UPGRADE_NOTE: command was upgraded to command_Renamed. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
        Dim command_Renamed As String
        On Error GoTo handleerror
        i = 0
        Dim debugdata As Byte
        If VB.Right(Text1.Text, 5) = "START" Then
            Do Until Mid(Text1.Text, i + 1, 5) = "START"
                Text1.SelectionStart = i
                Text1.SelectionLength = 7
                If (VB.Right(Text1.SelectedText, 1) <> Chr(10)) Then
                    i = i + 7
                    Do Until (Mid(Text1.Text, i + 1, 1) = Chr(13)) And (i > 0) And (i < Len(Text1.Text))
                        i = i - 1
                    Loop
                    Text1.SelectionStart = i
                    Text1.SelectionLength = 0
                    Exit Sub
                End If

                command_Renamed = Mid(Text1.SelectedText, 3, 1)
                If command_Renamed <> ":" Then
                    Text1.SelectionStart = i
                    Text1.SelectionLength = 0
                    Exit Sub
                End If
                i = i + 7
            Loop
            i = 0
            txtUploadSpeed.Text = "Script execution Started"
            Do Until Mid(Text1.Text, i + 1, 7) = "START"
                Application.DoEvents()
                Text1.SelectionStart = i
                Text1.SelectionLength = 7
                debugprint(Text1.SelectedText)
                command_Renamed = Mid(Text1.SelectedText, 1, 1)
                If command_Renamed = "R" Then
                    timeout = VB.Timer
                    addr = CByte(Mid(Text1.SelectedText, 2, 1))
                    data = CByte("&H" & Mid(Text1.SelectedText, 4, 2))
                    debugdata = ReadSPC700_internal(addr)

                    Do Until debugdata = data
                        If (VB.Timer - timeout) > 3 Then Exit Do
                        debugprint(debugdata)
                        debugdata = ReadSPC700_internal(addr)
                        If (debugdata = data) Then timeout = VB.Timer
                    Loop
                    If debugdata <> data Then
                        txtUploadSpeed.Text = "Script execution timeout"
                        Exit Do
                    End If
                ElseIf command_Renamed = "W" Then
                    addr = CByte(Mid(Text1.SelectedText, 2, 1))
                    data = CByte("&H" & Mid(Text1.SelectedText, 4, 2))
                    WriteSPC700_internal(addr, data)
                ElseIf command_Renamed = "P" Then
                    addr = CByte(Mid(Text1.SelectedText, 2, 1))
                    Do Until ReadSPC700_internal(addr) = data
                    Loop
                ElseIf command_Renamed = "Q" Then
                    Do Until ReadSPC700_internal(addr) = data
                    Loop
                End If
                i = i + 7
            Loop
            If txtUploadSpeed.Text <> "Script execution timeout" Then
                txtUploadSpeed.Text = "Script execution complete"
            End If
            Text1.SelectionStart = location
            Text1.SelectionLength = 0
            '   Text1.Text = ""
        End If
handleerror:

    End Sub

    Private Sub Timer1_Tick(ByVal eventSender As Object, ByVal eventArgs As EventArgs) Handles Timer1.Tick
        Static prevreadcount(9) As Short
        Dim i As Short
        prevreadcount(0) = prevreadcount(1)
        prevreadcount(1) = prevreadcount(2)
        prevreadcount(2) = prevreadcount(3)
        prevreadcount(3) = _readcount
        prevreadcount(4) = prevreadcount(5)
        prevreadcount(5) = prevreadcount(6)
        prevreadcount(6) = prevreadcount(7)
        prevreadcount(7) = prevreadcount(8)
        prevreadcount(8) = prevreadcount(9)
        prevreadcount(9) = _readcount
        _readcount = 0

        For i = 0 To 3
            _readcount = _readcount + prevreadcount(i)
        Next i

        txtReadSpeed.Text = _readcount / 4 & " B/s"
        _readcount = 0

    End Sub

    Private Sub tmrReadport_Tick(ByVal eventSender As Object, ByVal eventArgs As EventArgs) Handles tmrReadport.Tick
        On Error GoTo errorhandler
        Dim i As Short
        For i = 0 To 3
            txtOut(i).Text = Hex(ReadSPC700_internal(i + 4))
            If (txtOut(i).Text.Length = 1) Then
                txtOut(i).Text = "0" & txtOut(i).Text
            End If
        Next i
        _readcount = _readcount + 4
        Exit Sub
errorhandler:
        'UPGRADE_WARNING: Timer property tmrReadport.Interval cannot have a value of 0. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="169ECF4A-1968-402D-B243-16603CC08604"'
        'tmrReadport.Interval = 0
        tmrReadport.Enabled = False
    End Sub

    Private Sub txtIn_KeyPress(ByVal eventSender As Object, ByVal eventArgs As KeyPressEventArgs) Handles txtIn.KeyPress
        Dim KeyAscii As Short = Asc(eventArgs.KeyChar)
        Dim Index As Short = txtIn.GetIndex(eventSender)
        Dim writeports As Boolean = False
        On Error Resume Next

        Select Case KeyAscii
            Case 13
                debugprint(WriteSPC700_internal(Index, CInt("&h" & txtIn(Index).Text)))
                KeyAscii = 0
            Case 97 To 102
                KeyAscii = KeyAscii - 32
                writeports = True
            Case 65 To 70
                writeports = True
            Case 48 To 57
                writeports = True
            Case 8

            Case Else
                KeyAscii = 0

        End Select
        If (writeports) Then
            If ((Len(txtIn(Index).Text) = 1) And Not ((txtIn(Index).SelectionStart = 0) And (txtIn(Index).SelectionLength = 1))) Then
                If (Index = 4) Then
                    Select Case lblGame.Text
                        Case "  Blazeon  "
                            debugprint(WriteSPC700_internal(Index + 0, CInt("&h" & txtIn(Index).Text & Chr(KeyAscii))))
                        Case "  Uchuu Race Astro Go Go  "
                            debugprint(WriteSPC700_internal(Index + 1, CInt("&h" & txtIn(Index).Text & Chr(KeyAscii))))
                        Case "  UFO Kamen Yakisoban  "
                            debugprint(WriteSPC700_internal(Index + 1, CInt("&h" & txtIn(Index).Text & Chr(KeyAscii))))
                        Case "  Maka Maka  "
                            debugprint(WriteSPC700_internal(Index + 2, CInt("&h" & txtIn(Index).Text & Chr(KeyAscii))))
                    End Select
                Else
                    debugprint(WriteSPC700_internal(Index, CInt("&h" & txtIn(Index).Text & Chr(KeyAscii))))
                End If
            End If
        End If


        eventArgs.KeyChar = Chr(KeyAscii)
        If KeyAscii = 0 Then
            eventArgs.Handled = True
        End If
    End Sub

    Public Sub InitSPC700()
        Dim i As Short
        i = ReadSPC700_internal(0)
        Do Until i = &HAA
            i = ReadSPC700_internal(0)
            debugprint(i)
        Loop

        WriteSPC700_internal(1, 1)
        WriteSPC700_internal(2, 2)
        WriteSPC700_internal(3, 0)
        WriteSPC700_internal(0, &HCC)

        Do Until ReadSPC700_internal(0) = &HCC

        Loop
    End Sub

    Public Sub hackgame()
        Dim i As Short
        Dim ftimer As Single
        Select Case lblGame.Text
            Case "  Tales of Phantasia  ", "  Star Ocean  "
                ftimer = VB.Timer()
                Do Until (VB.Timer() - ftimer) > 0.25
                Loop

                frmToP.allow_top_events(True)
                frmToP.checkStateType()
                frmToP.Show()
            Case Else
                frmToP.allow_top_events(False)
                frmToP.Hide()
        End Select
        For i = 0 To 3
            If (Hex(_hackbytes(i)).Length = 1) Then
                txtIn(i).Text = "0" & Hex(_hackbytes(i))
            Else
                txtIn(i).Text = Hex(_hackbytes(i))
            End If
        Next i
        tmrReadport_Tick(tmrReadport, New EventArgs())
    End Sub

    Public Sub gameIPLsequence()
        'Select Case lblGame
        '    Case "  Bahamut Lagoon  "
        '        WriteSPC700_internal 0, &HCC
        '        Do Until ReadSPC700_internal(0) = &HAA
        '        Loop
        '    Case "  Seiken Densetsu 3  "
        '        WriteSPC700_internal 0, &HCC
        '        Do Until ReadSPC700_internal(0) = &HAA
        '        Loop
        '    Case "  Chrono Trigger  "
        '        WriteSPC700_internal 0, &HFF
        '        WriteSPC700_internal 1, &HF0
        '        Do Until ReadSPC700_internal(0) = &HAA
        '        Loop
        '    Case "  Hu Tengai Makyo Zero  "
        '        WriteSPC700_internal 1, &H10
        '        Do Until ReadSPC700_internal(0) = &HAA
        '        Loop
        'End Select
    End Sub



    Public Function LShiftLong(ByVal Value As Integer, ByVal Shift As Short) As Integer
        MakeOnBits()
        If (Value And (2 ^ (31 - Shift))) Then GoTo OverFlow
        LShiftLong = (CShort(Value And _onBits(31 - Shift)) * (2 ^ Shift))
        Exit Function

OverFlow:
        LShiftLong = (CShort(Value And _onBits(31 - (Shift + 1))) * (2 ^ (Shift))) Or &H80000000
    End Function

    Public Function RShiftLong(ByVal Value As Integer, ByVal Shift As Short) As Integer
        Dim hi As Integer
        MakeOnBits()
        If (Value And &H80000000) Then hi = &H40000000

        RShiftLong = (Value And &H7FFFFFFE) \ (2 ^ Shift)
        RShiftLong = (RShiftLong Or (hi \ (2 ^ (Shift - 1))))
    End Function



    Private Sub MakeOnBits()
        Dim j As Short
        Dim v As Integer
        For j = 0 To 30
            v = v + (2 ^ j)
            _onBits(j) = v
        Next j
        _onBits(j) = v + &H80000000
    End Sub

    Dim stop_00_search As Boolean
    Dim stop_ff_search As Boolean
    Dim stop_00_ff_search As Boolean
    Dim last_to_stop As Integer = -1
    Dim spcdata(65535) As Byte
    Dim spc2data(255) As Byte
    Dim spc2dataoffset As UInt16
    Public Function IsAPUSpaceFree(ByVal Address As Integer) As Integer
        On Error GoTo handler
        Dim i As Integer
        Dim j As Integer
        Dim _00_free As Integer = 0
        Dim _ff_free As Integer = 0
        Dim _00_ff_free As Integer = 0
        Dim min_search As Integer = 64

        If (Address = 256) Then
            last_to_stop = -1
            stop_00_search = True
            stop_ff_search = False
            stop_00_ff_search = False
        Else
            stop_00_search = True
            stop_ff_search = True
            stop_00_ff_search = True
            If (last_to_stop = -1) Then
                stop_00_search = True
                stop_ff_search = False
                stop_00_ff_search = False
            End If
            If (last_to_stop = 0) Then
                stop_00_search = False
            ElseIf (last_to_stop = 1) Then
                stop_ff_search = False
            Else
                stop_00_ff_search = False
            End If
        End If

        If (spcdata(Address) <> 0) And (spcdata(Address) <> 255) Then
            IsAPUSpaceFree = 0
            Exit Function
        End If
        For i = Address To 65535 Step 1
            If (Not stop_ff_search) Then
                If (spcdata(i) = 255) Then
                    _ff_free = _ff_free + 1
                Else
                    If (stop_00_ff_search) Then
                        If (i - Address) >= min_search Then
                            last_to_stop = 1
                        End If
                        stop_ff_search = True
                    End If
                End If
            End If
            If (Not stop_00_search) Then
                If (spcdata(i) = 0) Then
                    _00_free = _00_free + 1
                Else
                    stop_00_search = True
                End If
            End If
            If (Not stop_00_ff_search) Then
                j = 0
                If ((i - j) Mod 64 > 31) And (spcdata(i - j) = &HFF) Then
                    _00_ff_free = _00_ff_free + 1
                ElseIf ((i - j) Mod 64 <= 31) And (spcdata(i - j) = 0) Then
                    _00_ff_free = _00_ff_free + 1
                Else
                    If (stop_ff_search) Then
                        If (i - Address) >= min_search Then
                            last_to_stop = 2
                        End If
                        stop_ff_search = True
                    End If
                    stop_00_ff_search = True
                End If
            End If
            If (stop_ff_search And stop_00_search And stop_00_ff_search) Then
                If ((i - Address) >= min_search) Then
                    IsAPUSpaceFree = (i - Address) - ((i - Address) Mod 16)
                    Exit Function
                Else
                    IsAPUSpaceFree = 0
                    Exit Function
                End If
            End If
        Next
handler:
        If ((i - Address) >= min_search) Then
            IsAPUSpaceFree = (i - Address) - ((i - Address) Mod 16)
            Exit Function
        Else
            IsAPUSpaceFree = 0
            Exit Function
        End If
    End Function

    Public Sub LoadAPU(ByRef strSPCFile As String, Optional ByVal intSPCTrack As Integer = -1)
        On Error GoTo handleerror
        PauseUpload.Visible = True
        _uploadstate = True
        _uploadcomplete = False
        frmToP.allow_top_events(False)
        frmToP.Hide()
        Application.DoEvents()
        gameIPLsequence()
        enablebuttons(False)
        Dim timerinit As Single
        Dim xfer_error As Integer

        Dim songcount As UInt16
        Dim temp As Byte
        Dim spc_a As Byte
        Dim spc_x As Byte
        Dim spc_y As Byte
        Dim spc_sp As Byte
        Dim spc_sw As Byte
        Dim spc_pcl As Byte
        Dim spc_pch As Byte
        Dim spcram(63) As Byte

        Dim echosize As Integer
        Dim echoregion As Integer

        Dim bootcode() As Byte
        Dim dsploader() As Byte


        Dim bootptr As Integer
        Dim bootbyte As Byte
        Dim bootsize As Integer = 77
        Dim count As Short
        Dim i As Integer
        Dim j As Integer
        Dim k As Integer
        Dim l As Integer

        Dim boot_code As Integer = 0

        Dim compspcdata(80000) As Byte
        Dim mask As Byte
        Dim mask_left As Byte
        Dim mask_pointer As Integer
        Dim last_byte As Byte
        Dim comp_left As Integer


        'frmToP.Timer1.Enabled = False
        PauseUpload.Text = "Pause Upload"
        _strcurrentspc = strSPCFile
        _intcurrentspctrack = intSPCTrack


        FileOpen(1, strSPCFile, OpenMode.Binary, OpenAccess.Read, OpenShare.Shared)
        'FileOpen(1, strSPCFile, OpenMode.Binary)


        If intSPCTrack < 0 Then
            lblSong.Text = "  " & GetTag(IDX6TagType.Song_Name) & "  "
            lblGame.Text = "  " & GetTag(IDX6TagType.Game_Name) & "  "
            lblArtist.Text = "  " & GetTag(IDX6TagType.Artist_Name) & "  "

            Dim testFile As FileInfo
            testFile = My.Computer.FileSystem.GetFileInfo(strSPCFile)
            lblFileName.Text = " " & testFile.Name & " "
            lblDumper.Text = " " & GetTag(IDX6TagType.Dumper_Name) & " "
            lblOST.Text = " " & GetTag(IDX6TagType.OST_Title) & " "
            lblPublisher.Text = " " & GetTag(IDX6TagType.Publisher_Name) & " "
            lblComment.Text = " " & GetTag(IDX6TagType.Comment) & " "

            _playtime = GetTime(0) + GetTime(1)

            FileGet(1, spc_pcl, &H25 + 1)
            FileGet(1, spc_pch, &H26 + 1)
            FileGet(1, spc_a, &H27 + 1)
            FileGet(1, spc_x, &H28 + 1)
            FileGet(1, spc_y, &H29 + 1)
            FileGet(1, spc_sw, &H2A + 1)
            FileGet(1, spc_sp, &H2B + 1)
            FileGet(1, spcdata, &H100 + 1)
            FileGet(1, _DSPdata, &H10100 + 1)
            FileGet(1, spcram, &H101C0 + 1)
            FileClose(1)
        Else
            lblSong.Text = "  " & GetSP2Tag(SPC2TagType.Song_Name, intSPCTrack) & "  "
            lblGame.Text = "  " & GetSP2Tag(SPC2TagType.Game_Name, intSPCTrack) & "  "
            lblArtist.Text = "  " & GetSP2Tag(SPC2TagType.Artist_Name, intSPCTrack) & "  "
            lblFileName.Text = " " & GetSP2Tag(SPC2TagType.SPC_Filename, intSPCTrack) & ".spc "
            lblDumper.Text = " " & GetSP2Tag(SPC2TagType.Dumper_Name, intSPCTrack) & " "
            lblOST.Text = " " & GetSP2Tag(SPC2TagType.OST_Title, intSPCTrack) & " "
            lblPublisher.Text = " " & GetSP2Tag(SPC2TagType.Publisher_Name, intSPCTrack) & " "
            lblComment.Text = " " & GetSP2Tag(SPC2TagType.Comment, intSPCTrack) & " "

            _playtime = GetSP2Time(0, intSPCTrack) + GetSP2Time(1, intSPCTrack)

            FileGet(1, spc_pcl, 1 + 16 + (1024 * intSPCTrack) + 704)
            FileGet(1, spc_pch, 1 + 16 + (1024 * intSPCTrack) + 705)
            FileGet(1, spc_a, 1 + 16 + (1024 * intSPCTrack) + 706)
            FileGet(1, spc_x, 1 + 16 + (1024 * intSPCTrack) + 707)
            FileGet(1, spc_y, 1 + 16 + (1024 * intSPCTrack) + 708)
            FileGet(1, spc_sw, 1 + 16 + (1024 * intSPCTrack) + 709)
            FileGet(1, spc_sp, 1 + 16 + (1024 * intSPCTrack) + 710)
            FileGet(1, _DSPdata, 1 + 16 + (1024 * intSPCTrack) + 512)
            FileGet(1, spcram, 1 + 16 + (1024 * intSPCTrack) + 640)
            FileGet(1, temp, 1 + 16 + (1024 * intSPCTrack) + 734)
            boot_code = temp
            FileGet(1, temp, 1 + 16 + (1024 * intSPCTrack) + 735)
            boot_code = boot_code + (temp * 256)
            FileGet(1, temp, 1 + 7)
            songcount = temp
            FileGet(1, temp, 1 + 8)
            songcount = songcount + (temp * 256)
            For i = 0 To 255 Step 1
                FileGet(1, temp, 1 + 16 + (1024 * intSPCTrack) + (i * 2))
                spc2dataoffset = temp
                FileGet(1, temp, 1 + 16 + (1024 * intSPCTrack) + (i * 2) + 1)
                spc2dataoffset = spc2dataoffset + (temp * 256)
                FileGet(1, spc2data, 1 + 16 + (1024 * songcount) + (256 * spc2dataoffset))
                For j = 0 To 255 Step 1
                    spcdata((i * 256) + j) = spc2data(j)
                Next
            Next
            FileClose(1)
        End If

        dsploader = My.Resources.SPCCODE_DSP
        timerinit = VB.Timer()

        If spcdata(&HF1) And &H80 Then
            For i = 0 To 63
                spcdata(&HFFC0 + i) = spcram(i)
            Next
        End If

        echoregion = _DSPdata(&H6D) * CLng(256)
        echosize = _DSPdata(&H7D) * CLng(2048)

        If (((_DSPdata(&H6C) And &H20) = 0)) Then
            If (echoregion + echosize) > 65792 Then
                Err.Raise(-2000005, , "SPC FIle Echo region wraps around and corrupts stack. This is NOT supported on real hardware")
            End If
            If (echoregion + echosize) > 65536 Then
                If MessageBox.Show("SPC file Echo region wraps around, and will corrupt PAGE 0.  This SPC as a result might crash on real hardware. Load anyways?", "APUplay", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) = DialogResult.No Then
                    Err.Raise(-2000005, , "SPC file Echo region wrapped around.")
                End If
            End If
            If (echoregion = 0) And (echosize > 0) Then
                Err.Raise(-2000005, , "SPC File Echo region corrupts stack. This is NOT supported on real hardware.")
            End If
        End If

        If echosize = 0 Then echosize = 4
        For i = echoregion To 65535 Step 1
            If i >= echoregion And i <= (echoregion + echosize - 1) Then
                If ((_DSPdata(&H6C) And &H20) = 0) And (_echoclear = 0) Then
                    spcdata(i) = 0
                ElseIf _echoclear = 1 Then
                    spcdata(i) = 0
                End If
            End If
        Next i

        If (boot_code = 2) Then bootsize = 0
        If (bootsize = 77) Then
            bootcode = My.Resources.SPCCODE_BOOT

            _hackbytes(0) = spcdata(&HF4) 'Inports 0-3 to be written upon load completion
            _hackbytes(1) = spcdata(&HF5) 'Load Completion is Acknowledged by a read of 0x53 on Port 0.
            _hackbytes(2) = spcdata(&HF6)
            _hackbytes(3) = spcdata(&HF7)

            _spcinportiszero = False
            If (spcdata(&HF4) = 0) Then
                If (spcdata(&HF5) = 0) Then
                    If (spcdata(&HF6) = 0) Then
                        If (spcdata(&HF7) = 0) Then
                            _spcinportiszero = True
                        End If
                    End If
                End If
            End If

            If Not _spcinportiszero Then
                bootcode(&H19) = spcdata(&HF4)          'Inport 0
            Else
                bootcode(&H19) = spcdata(&HF4) + 1      'Inport 0
            End If
            bootcode(&H1F) = spcdata(&HF5)          'Inport 1
            bootcode(&H25) = spcdata(&HF6)          'Inport 2
            bootcode(&H2B) = spcdata(&HF7)          'Inport 3
            bootcode(&H1) = spcdata(&H0)            'SPCRam Address 0x0000
            bootcode(&H4) = spcdata(&H1)            'SPCRam Address 0x0001
            bootcode(&H7) = spcdata(&HFC)           'Timer 2
            bootcode(&HA) = spcdata(&HFB)           'Timer 1
            bootcode(&HD) = spcdata(&HFA)           'Timer 0
            bootcode(&H10) = spcdata(&HF1)          'Control Register
            bootcode(&H38) = _DSPdata(&H6C)          'DSP Echo Control Register
            _DSPdata(&H6C) = &H60                        'And Set it off to start
            bootcode(&H3E) = _DSPdata(&H4C)          'DSP KeyON Register
            _DSPdata(&H4C) = &H0                         'And Set it off to start
            bootcode(&H41) = spcdata(&HF2)          'Current DSP Register address

            bootcode(&H44) = (spc_sp + &H100 - 3) Mod 256
            spcdata(&H100 + ((spc_sp + &H100 - 0) Mod 256)) = spc_pch   'Program Counter High Address
            spcdata(&H100 + ((spc_sp + &H100 - 1) Mod 256)) = spc_pcl   'Program Counter Low Address
            spcdata(&H100 + ((spc_sp + &H100 - 2) Mod 256)) = spc_sw    'Program Status Word

            bootcode(&H47) = spc_a                  'A Register
            bootcode(&H49) = spc_y                  'Y Register
            bootcode(&H4B) = spc_x                  'X Register

            count = 0
            If (boot_code = 0) Then
                If (_freespacesearch < 2) Then
                    For j = 255 To 0 Step -1
                        '    bootbyte = spcdata(65471)
                        For bootptr = 65471 To &H100 Step -1
                            If (bootptr > echoregion + echosize) _
                                  Or (bootptr < echoregion) Then
                                If spcdata(bootptr) = j Then
                                    count = count + 1
                                Else
                                    count = 0
                                End If
                                If count = bootsize Then Exit For
                            Else
                                count = 0
                                '            bootbyte = spcdata(bootptr)
                                '            spcdata(bootptr) = 0
                            End If
                        Next bootptr
                        If count = bootsize Then Exit For
                    Next j
                End If
                If (_freespacesearch = 0) Or (_freespacesearch = 2) Then
                    If (count <> bootsize) Then
                        If (echosize < bootsize) Or (echoregion = 0) Then
                            count = 0
                        Else
                            count = bootsize
                            bootptr = echoregion
                            'bootcode(&H38) = &H60   'Bootloader will not survive echo being turned on, if loaded here.
                            count = bootsize  'Stupid bug, where the bootloader is loaded, either incompletely, or not at all, sometimes
                            'causing the spc to not load/play at all.  Amazing that this bug survived for a few years.
                        End If
                    End If
                End If
                If (_freespacesearch = 0) Or (_freespacesearch = 3) Then
                    If (count <> bootsize) Then
                        For bootptr = 65471 To &H100 Step -1
                            k = 0
                            If ((bootptr - k) Mod 64 > 31) And (spcdata(bootptr - k) = &HFF) Then
                                count = count + 1
                            ElseIf ((bootptr - k) Mod 64 <= 31) And (spcdata(bootptr - k) = 0) Then
                                count = count + 1
                            Else
                                count = 0
                            End If
                            If count = bootsize Then Exit For
                        Next
                    End If
                End If
                If count <> bootsize Then
                    bootptr = &HFF00 'Last resort,  free space not found any other way.
                    'not guaranteed to work correctly.
                    'Err.Raise(-2000003, , "This spc file does not have sufficient ram to be loaded with the selected free space strategy")
                End If
            ElseIf (boot_code = 1) Then
                bootptr = &HFF00 'Last resort,  free space not found any other way.
            Else
                bootptr = boot_code
            End If

            For i = bootptr To bootptr + bootsize - 1
                spcdata(i) = bootcode(i - bootptr)
            Next i
        End If

        Dim resetdelay As Double
        Dim retrycount As Integer = 0
        ResetAPU_internal()
        resetdelay = VB.Timer
        Do While (VB.Timer - resetdelay) < 0.05
            Application.DoEvents()
        Loop
        Do While (ReadSPC700_internal(0) <> &HAA) Or (ReadSPC700_internal(1) <> &HBB) Or _
              (ReadSPC700_internal(2) <> &H0) Or (ReadSPC700_internal(3) <> &H0)
            ResetAPU_internal()
            resetdelay = VB.Timer
            Do While (VB.Timer - resetdelay) < 0.05
                Application.DoEvents()
            Loop
            retrycount = retrycount + 1
            If (retrycount > 20) Then Err.Raise(-2000010, , "Error resetting the APU")
        Loop

        For i = 0 To 127
            StartSPC700(_DSPdata(i))
        Next i
        For i = 0 To 255
            StartSPC700(spcdata(i))
        Next i

        '    InitSPC700
        '    For i = 0 To 15
        '        WriteSPC700_internal 1, dsploader(i)
        '        WriteSPC700_internal 0, i
        '        Do Until ReadSPC700_internal(0) = i
        '        Loop
        '    Next i
        '    WriteSPC700_internal 1, 0
        '    i = ReadSPC700_internal(0)
        '    i = i + 2
        '    WriteSPC700_internal 0, i
        ''   Do Until ReadSPC700_internal(0) = i
        ''       Loop
        '    If Not waitInport(0, i, 512) Then Err.Raise -2000000, , "Error Loading SPC"
        '    For i = 0 To 127
        '        WriteSPC700_internal 1, dspdata(i)
        '        WriteSPC700_internal 0, i
        '        'Do Until ReadSPC700_internal(0) = i
        '            If i = 127 Then
        '                If Not waitInport(0, &HAA, 512) Then
        '                    Err.Raise -2000000, , "Error Loading SPC"
        '                End If
        '            Else
        '                If Not waitInport(0, i, 512) Then
        '                    Err.Raise -2000000, , "Error Loading SPC"
        '                End If
        '            End If
        '
        '        'Loop
        '    Next i
        ''    Do Until ReadSPC700_internal(0) = &HAA
        ''    Loop
        '    If Not waitInport(0, &HAA, 512) Then Err.Raise -2000000, , "Error Loading SPC"
        '    InitSPC700
        '    For i = 2 To &HEF
        '
        '        WriteSPC700_internal 1, spcdata(i)
        '        WriteSPC700_internal 0, i - 2
        '        j = 0
        '        'Do Until ReadSPC700_internal(0) = i - 2
        '        'tmrReadport_Timer
        '        'DoEvents
        '        'j = j + 1
        '        'If (j > 128) Then Err.Raise -2000000, , "Error Loading SPC"
        '        'Loop
        '        If Not waitInport(0, i - 2, 64) Then Err.Raise -2000000, , "Error Loading SPC, Address = " & Hex(i - 2) & " Data = " & Hex(spcdata(i))
        '        apuLoad.Value = i
        '    Next i

        'WriteSPC700_internal(1, 1)
        'WriteSPC700_internal(2, &H0)
        'WriteSPC700_internal(3, &H1)
        'k = ReadSPC700_internal(0)
        'k = k + 2
        'WriteSPC700_internal(0, k)
        'If Not waitInport(0, k, 512) Then Err.Raise(-2000000, , "Error Loading SPC")
        'SetPort0(0)
        Dim quickupload As Boolean = True
        Dim datauploaded As Boolean = False
        Dim dataskip As Boolean = True


        For i = &H100 To 65535 Step 16
            If quickupload Then
                For j = i To 65535 Step 16
                    If (IsAPUSpaceFree(j) > 0) Or ((j >= echoregion) And (j < (echoregion + echosize)) And (echosize <> 4)) And ((j < bootptr) Or (j >= (bootptr + bootsize))) Then
                        'If (spcdata(j + 0) = &HFF And spcdata(j + 1) = &HFF And spcdata(j + 2) = &HFF And spcdata(j + 3) = &HFF And spcdata(j + 4) = &HFF And spcdata(j + 5) = &HFF And spcdata(j + 6) = &HFF And spcdata(j + 7) = &HFF And spcdata(j + 8) = &HFF And spcdata(j + 9) = &HFF And spcdata(j + 10) = &HFF And spcdata(j + 11) = &HFF And spcdata(j + 12) = &HFF And spcdata(j + 13) = &HFF And spcdata(j + 14) = &HFF And spcdata(j + 15) = &HFF) Then
                        If ((j - i) <> 0) Then
                            datauploaded = True
                            xfer_error = StartWrite16bytes(i >> 8, i And &HFF, (j - i) >> 8, (j - i) And &HFF)
                            If xfer_error > 0 Then
                                Err.Raise(-2000000, , "Error Loading SPC")
                            End If
                            For k = i To (j - 1) Step 16
                                If (k Mod 256) = 0 Then
                                    apuLoad.Value = k
                                    Application.DoEvents()
                                    'readcount = readcount + 256
                                End If
                                _readcount = _readcount + 16
                                xfer_error = Write16bytes(spcdata(k + 0), spcdata(k + 1), spcdata(k + 2), spcdata(k + 3), spcdata(k + 4), spcdata(k + 5), spcdata(k + 6), spcdata(k + 7), spcdata(k + 8), spcdata(k + 9), spcdata(k + 10), spcdata(k + 11), spcdata(k + 12), spcdata(k + 13), spcdata(k + 14), spcdata(k + 15))
                                If xfer_error > 0 Then
                                    Err.Raise(-2000000, , "Error Loading SPC")
                                End If
                            Next
                            xfer_error = FinishWrite16bytes()
                            If xfer_error > 0 Then
                                Err.Raise(-2000000, , "Error Loading SPC")
                            End If
                        Else
                            If (dataskip) Then
                                datauploaded = True
                                dataskip = False
                            End If
                        End If
                        'For k = j To 65535 Step 16
                        '    If (spcdata(k + 0) <> &HFF Or spcdata(k + 1) <> &HFF Or spcdata(k + 2) <> &HFF Or spcdata(k + 3) <> &HFF Or spcdata(k + 4) <> &HFF Or spcdata(k + 5) <> &HFF Or spcdata(k + 6) <> &HFF Or spcdata(k + 7) <> &HFF Or spcdata(k + 8) <> &HFF Or spcdata(k + 9) <> &HFF Or spcdata(k + 10) <> &HFF Or spcdata(k + 11) <> &HFF Or spcdata(k + 12) <> &HFF Or spcdata(k + 13) <> &HFF Or spcdata(k + 14) <> &HFF Or spcdata(k + 15) <> &HFF) Then
                        '        i = (k - 16)
                        '        Exit For
                        '    Else
                        '        i = k
                        '    End If
                        '    If (k Mod 256) = 0 Then
                        '        apuLoad.Value = k
                        '        System.Windows.Forms.Application.DoEvents()
                        '        'readcount = readcount + 256
                        '    End If
                        'Next
                        k = IsAPUSpaceFree(j)
                        If ((j >= echoregion) And (j < (echoregion + echosize)) And (echosize <> 4)) Then
                            k = k + (echosize - (j - echoregion))
                        End If
                        i = k + j - 16
                        If (k + j) = 65536 Then
                            i = 65536
                            datauploaded = True
                        End If
                        Exit For
                    End If
                Next
                If (datauploaded = False) And (i < (j - 1)) Then
                    xfer_error = StartWrite16bytes(i >> 8, i And &HFF, (65536 - i) >> 8, (65536 - i) And &HFF)
                    If xfer_error > 0 Then
                        Err.Raise(-2000000, , "Error Loading SPC")
                    End If
                    For k = i To (j - 1) Step 16
                        If (k Mod 256) = 0 Then
                            apuLoad.Value = k
                            Application.DoEvents()
                            'readcount = readcount + 256
                        End If
                        _readcount = _readcount + 16
                        xfer_error = Write16bytes(spcdata(k + 0), spcdata(k + 1), spcdata(k + 2), spcdata(k + 3), spcdata(k + 4), spcdata(k + 5), spcdata(k + 6), spcdata(k + 7), spcdata(k + 8), spcdata(k + 9), spcdata(k + 10), spcdata(k + 11), spcdata(k + 12), spcdata(k + 13), spcdata(k + 14), spcdata(k + 15))
                        If xfer_error > 0 Then
                            Err.Raise(-2000000, , "Error Loading SPC")
                        End If
                    Next
                    xfer_error = FinishWrite16bytes()
                    If xfer_error > 0 Then
                        Err.Raise(-2000000, , "Error Loading SPC")
                    End If
                    Exit For
                Else
                    datauploaded = False
                End If
                Continue For
            Else

                'apuLoad.Value = i
                'System.Windows.Forms.Application.DoEvents()


                'l = l + 4
                'For i = 0 To l Step 4
                ' WriteSPC700_internal_WP0I 1, spcdata(i)
                '  WriteSPC700_internal 0, i Mod 256
                '   debugprint ReadSPC700_internal(0)
                'debugprint Hex(i)
                'debugprint Hex(i And &H7F)
                'If ((i And &H7F) = 0) Then
                If i Mod 256 = 0 Then
                    'WriteSPC700_internal 1, 1
                    'WriteSPC700_internal 2, (i And &HFF)
                    'WriteSPC700_internal 3, (RShiftLong(i, 8) And &HFF)

                    'k = ReadSPC700_internal(0)
                    'k = k + 2
                    'WriteSPC700_internal 0, k
                    '    Do Until ReadSPC700_internal(0) = i
                    '        Loop
                    'If Not waitInport(0, k, 512) Then Err.Raise -2000000, , "Error Loading SPC"
                    'SetPort0 0
                    apuLoad.Value = i
                    Application.DoEvents()
                    _readcount = _readcount + 256
                End If
            End If

            xfer_error = Write16bytes(spcdata(i + 0), spcdata(i + 1), spcdata(i + 2), spcdata(i + 3), spcdata(i + 4), spcdata(i + 5), spcdata(i + 6), spcdata(i + 7), spcdata(i + 8), spcdata(i + 9), spcdata(i + 10), spcdata(i + 11), spcdata(i + 12), spcdata(i + 13), spcdata(i + 14), spcdata(i + 15))
            'xfer_error = WriteCompressedByte(compspcdata(i), compspcdata(i + 1), compspcdata(i + 2), compspcdata(i + 3), l)
            If xfer_error > 0 Then Err.Raise(-2000000, , "Error Loading SPC")

            ' debugprint i, xfer_error
        Next i

        ''For i = 65472 To 65535 Step 16
        ''    If spcdata(&HF1) And &H80 Then
        ''        '            WriteSPC700_internal 1, spcram(i - 65472)
        ''        'If WriteSPC700_internal_WP0I(1, spcram(i - 65472)) = 1 Then Err.Raise -2000000, , "Error Loading SPC"
        ''        xfer_error = Write16bytes(spcram((i - 65472) + 0), spcram((i - 65472) + 1), spcram((i - 65472) + 2), spcram((i - 65472) + 3), spcram((i - 65472) + 4), spcram((i - 65472) + 5), spcram((i - 65472) + 6), spcram((i - 65472) + 7), spcram((i - 65472) + 8), spcram((i - 65472) + 9), spcram((i - 65472) + 10), spcram((i - 65472) + 11), spcram((i - 65472) + 12), spcram((i - 65472) + 13), spcram((i - 65472) + 14), spcram((i - 65472) + 15))
        ''    Else
        ''        '            WriteSPC700_internal 1, spcdata(i)
        ''        'If WriteSPC700_internal_WP0I(1, spcdata(i)) = 1 Then Err.Raise -2000000, , "Error Loading SPC"
        ''        xfer_error = Write16bytes(spcdata(i + 0), spcdata(i + 1), spcdata(i + 2), spcdata(i + 3), spcdata(i + 4), spcdata(i + 5), spcdata(i + 6), spcdata(i + 7), spcdata(i + 8), spcdata(i + 9), spcdata(i + 10), spcdata(i + 11), spcdata(i + 12), spcdata(i + 13), spcdata(i + 14), spcdata(i + 15))
        ''    End If
        ''    If xfer_error > 0 Then Err.Raise(-2000000, , "Error Loading SPC")
        ''    '  WriteSPC700_internal 0, i Mod 256
        ''    If i Mod 256 = 0 Then
        ''        apuLoad.Value = i
        ''        System.Windows.Forms.Application.DoEvents()
        ''    End If
        ''Next i
        FlushRead()
        If (bootptr > 0) Then
            WriteSPC700_internal(3, bootptr >> 8)
            WriteSPC700_internal(2, bootptr And &HFF)
            WriteSPC700_internal(1, 0)
            _usedBootcode = True
        Else
            WriteSPC700_internal(3, spc_pch)
            WriteSPC700_internal(2, spc_pcl)
            WriteSPC700_internal(1, 0)
            _usedBootcode = False
        End If
        _uploadcomplete = True
        If _uploadstate Then FinishUpload()

        txtUploadSpeed.Text = "Time to Upload: " & Math.Round(VB.Timer() - timerinit, 3) & " seconds"

        Debugprint("Boot Pointer Location = " & Hex(bootptr))
        Debugprint("Echo Pointer Location = " & Hex(echoregion))
        Debugprint("Echo Size = " & Hex(echosize))

        _strselectedspc = strSPCFile
        _intselectedspctrack = intSPCTrack
        If Not _uploadcomplete Then
            PauseUpload.Visible = False
        End If
        _uploadstate = False
        Exit Sub
handleerror:
        Dim error_i As Integer
        Select Case Err.Number
            Case 32755
                PauseUpload.Hide()
            Case -2000005
                error_i = MsgBox("This SPC file has an echo region that wraps to the beginning" & Chr(13) & _
                                 "and may or may not play correctly, do you wish to try anyways?", MsgBoxStyle.YesNo, "APU Play Win")
                If (error_i = vbYes) Then Resume Next
            Case -2000002
                Dim tryinit As Integer
                For tryinit = 0 To Combo1.Items.Count - 1 Step 1
                    Combo1.SelectedIndex = tryinit
                    If init_port(Combo1.Text) = 0 Then
                        cmdOpenPort.Enabled = False
                        Command2.Enabled = False
                        Combo1.Enabled = False
                        Initialize.Enabled = True
                        Resume
                    End If
                Next
                PauseUpload.Hide()
                MsgBox("Communcation port to APU hardware not open, and attempts to open it, have failed.", , "APU Play win")
            Case -2000000
                PauseUpload.Hide()
                MsgBox("Communication port to APU hardware closed unexpectedly", , "APU Play win")
            Case 9
                Resume
            Case 55
                FileClose(1)
                Resume
            Case Else
                PauseUpload.Hide()
                MsgBox(Err.Number & " - " & Err.Description, , "APU Play win")
        End Select
        'Resume
        enablebuttons(True)
        Debugprint(Err.Number & " - " & Err.Description)
    End Sub

    Public Function IsSPC(ByRef filename As String) As Integer
        On Error GoTo errorhandler
        Dim filesize As Long
        Dim headerstring As String = "SNES-SPC700 Sound File Data v0"
        Dim headerstring2 As String = "KSPC"
        Dim enc As New ASCIIEncoding()
        Dim headerbytes(30) As Byte
        Dim headerbytes2(4) As Byte
        Dim taginfo(3) As Byte
        Dim headerlen As Integer
        filesize = FileLen(filename)
        FileOpen(1, filename, OpenMode.Binary, OpenAccess.Read, OpenShare.Shared)
        FileGet(1, headerbytes, 1)
        headerbytes(30) = 0

        If (String.Compare(headerstring, enc.GetString(headerbytes)) = 0) Then
            If (filesize < &H1020C) Then
                FileClose(1)
                IsSPC = ID6Type.ID6SPC
                Exit Function
            End If
            FileGet(1, taginfo, &H10200 + 1)
            'If Chr(taginfo(0)) = "x" And Chr(taginfo(1)) = "i" And Chr(taginfo(2)) = "d" And Chr(taginfo(3)) = "6" Then
            If String.Compare("xid6", enc.GetString(taginfo)) = 0 Then
                FileGet(1, headerlen, &H10204 + 1)
                If (headerlen >= 4) Then
                    FileClose(1)
                    IsSPC = ID6Type.ID6Ext
                    Exit Function
                End If
            End If
            FileClose(1)
            IsSPC = ID6Type.ID6SPC
            Exit Function
        Else
            FileClose(1)
            headerbytes2(0) = headerbytes(0)
            headerbytes2(1) = headerbytes(1)
            headerbytes2(2) = headerbytes(2)
            headerbytes2(3) = headerbytes(3)
            headerbytes2(4) = 0
            If (String.Compare(headerstring2, enc.GetString(headerbytes2)) = 0) Then
                IsSPC = ID6Type.ID6Sp2
                Exit Function
            Else
                GoTo Done
            End If
        End If

        Exit Function
Done:
        IsSPC = ID6Type.ID6Unk
        Exit Function
errorhandler:
        IsSPC = ID6Type.ID6Err
    End Function

    Public Function GetSP2Tag(ByRef tagID As Short, ByVal intSPCTrack As UShort) As String
        On Error GoTo errorhandler
        Dim enc As New ASCIIEncoding()
        Dim tagoffset As UInt32 = 0
        Dim temp As Byte
        Dim chunktype As Byte
        Dim chunklength As Byte
        Dim tagstring As String
        Dim tagstringbytes(31) As Byte
        Dim i As Integer
        FileGet(1, temp, 1 + 16 + (1024 * intSPCTrack) + 1020)
        tagoffset = temp
        FileGet(1, temp, 1 + 16 + (1024 * intSPCTrack) + 1021)
        tagoffset = tagoffset + (temp * 256)
        FileGet(1, temp, 1 + 16 + (1024 * intSPCTrack) + 1022)
        tagoffset = tagoffset + (temp * 65536)
        FileGet(1, temp, 1 + 16 + (1024 * intSPCTrack) + 1023)
        tagoffset = tagoffset + (temp * 16777216)


        If (tagID < 1) Then
            GetSP2Tag = ""
            Exit Function
        End If
        If (tagID > 8) Then
            GetSP2Tag = ""
            Exit Function
        End If
        FileGet(1, tagstringbytes, 1 + 16 + (1024 * intSPCTrack) + 768 + ((tagID - 1) * 32))
        If (tagID = 8) Then
            tagstringbytes(28) = 0
        End If
        tagstring = ""
        For i = 0 To 31 Step 1
            If tagstringbytes(i) = 0 Then Exit For
            tagstring = tagstring & enc.GetChars(tagstringbytes, i, 1)
        Next
        If (tagoffset > 0) Then
            Do
                FileGet(1, chunktype, 1 + tagoffset + 0)
                If (chunktype = 0) Then Exit Do
                If (chunktype > 8) Then Exit Do 'Chunks > 8 not defined.
                FileGet(1, chunklength, 1 + tagoffset + 1)
                If (chunklength > 0) Then
                    ReDim tagstringbytes(chunklength - 1)
                    FileGet(1, tagstringbytes, 1 + tagoffset + 2)
                    tagoffset = tagoffset + 2 + chunklength
                    If chunktype = tagID Then
                        For i = 0 To chunklength - 1 Step 1
                            If tagstringbytes(i) = 0 Then Exit For
                            tagstring = tagstring & enc.GetChars(tagstringbytes, i, 1)
                        Next
                    End If
                Else
                    tagoffset = tagoffset + 2
                End If
            Loop
        End If
        GetSP2Tag = tagstring
        Exit Function
errorhandler:
        GetSP2Tag = ""
    End Function

    Public Function IsText(ByRef offset As Integer, ByRef length As Integer) As Integer
        Dim data(length - 1) As Byte
        Dim c As Integer = 0
        FileGet(1, data, offset + 1)
        While (c < length)  'Rewrote this function due to a stupid bug where ALL conditions are checked, 
            'even when the first one evaluated causes the statement to be false.
            If ((data(c) >= &H2F) And (data(c) <= &H39)) Then
                c = c + 1
            Else
                Exit While
            End If
        End While
        If (c = length) Then 'Ditto here
            IsText = c
        ElseIf (data(c) = 0) Then
            IsText = c
        Else
            IsText = -1
        End If
    End Function

    Public Function IsSPCBinary() As Boolean
        On Error GoTo IsSPCBinary_error_handler
        Dim spcheader(255) As Byte
        Dim i As Integer, j As Integer, k As Integer
        i = IsText(&HA9, 3)
        j = IsText(&HAC, 5)
        k = IsText(&H9E, 11)
        'Assume the SPC is binary, prove that the SPC is text.
        FileGet(1, spcheader, 1)
        If (spcheader(&H1F) = &H30) And (spcheader(&H20) = &H31) Then
            IsSPCBinary = True
            Exit Function
        End If
        If ((i = 0) And (j = 0) And (k = 0)) Then
            If (spcheader(&HD1) = 1) And (spcheader(&HD2) = 0) Then
                IsSPCBinary = False
            Else
                IsSPCBinary = True
            End If
            Exit Function
        End If
        If (i <> -1) And (j <> -1) Then
            If (k > 0) Then
                IsSPCBinary = False
            End If
            If (k = 0) Then
                If (spcheader(&HB0) = 0) And (spcheader(&HB1) > 0) Then
                    IsSPCBinary = False
                Else
                    IsSPCBinary = True
                End If
            End If
            If (k = -1) Then
                If (spcheader(&HA2) = 0) And (spcheader(&HA3) = 0) And (spcheader(&HA4) = 0) And (spcheader(&HA5) = 0) Then
                    IsSPCBinary = True
                ElseIf (i = 3) Or (j = 5) Then
                    IsSPCBinary = True
                Else
                    IsSPCBinary = False
                End If
            End If
        End If
        Exit Function
IsSPCBinary_error_handler:
        IsSPCBinary = False
    End Function

    Public Function GetSP2Time(ByVal type As Integer, ByVal intSPCTrack As Integer) As UInteger
        Dim tagdatabytes(3) As Byte
        Dim i As Integer, j As UInteger
        FileGet(1, tagdatabytes, 1 + 16 + (1024 * intSPCTrack) + 716 + (type * 4))
        j = 0
        For i = 3 To 0 Step -1
            j = j * 256
            j = j + tagdatabytes(i)
        Next
        j = j \ 64
        GetSP2Time = j
    End Function

    Public Function GetTime(ByVal type As Integer) As UInteger
        On Error GoTo GetTime_error_handler
        Dim enc As New ASCIIEncoding()
        Dim taginfo(3) As Byte
        Dim headerlen As Integer
        Dim tagdata As Short
        Dim spcbinary As Boolean
        Dim data(3) As Byte
        Dim time As UInteger, fade As UInteger
        Dim tagID As Integer

        If (type = 0) Then
            tagID = IDX6TagType.Intro_Len
        Else
            tagID = IDX6TagType.Fade_Len
        End If

        FileGet(1, taginfo, &H10200 + 1)
        If Chr(taginfo(0)) = "x" And Chr(taginfo(1)) = "i" And Chr(taginfo(2)) = "d" And Chr(taginfo(3)) = "6" Then

            FileGet(1, headerlen, &H10204 + 1)
            ReDim taginfo(headerlen - 1)

            FileGet(1, taginfo, &H10208 + 1)
            For i = 0 To headerlen - 1 Step 4
                If tagID = taginfo(i) Then
                    tagdata = taginfo(i + 2) + (taginfo(i + 3) * 256)
                    time = 0
                    For j = i + 4 To tagdata + i + 4 - 1
                        time = time * 256
                        time = time + taginfo(j)
                    Next j
                    time = time \ 64
                    GetTime = time
                    Exit Function
                Else
                    If taginfo(i + 1) <> 0 Then
                        i = i + taginfo(i + 2) + (taginfo(i + 3) * 256)
                        i = i + 3
                        i = i \ 4    'Strings HAVE to be aligned to the next 32 bit boundary.
                        i = i * 4
                    End If
                End If
            Next i
        End If


        spcbinary = IsSPCBinary()
        If spcbinary Then
            FileGet(1, data, 1 + &HA9)
            time = data(0) + (data(1) * 256) + (data(2) * 65536)
            FileGet(1, data, 1 + &HAC)
            fade = data(0) + (data(1) * 256) + (data(2) * 65536) + (data(3) * 16777216)
        Else
            ReDim data(2)
            FileGet(1, data, 1 + &HA9)
            time = CUInt(enc.GetString(data))
            ReDim data(4)
            FileGet(1, data, 1 + &HAC)
            fade = CUInt(enc.GetString(data))
        End If
        time = time * 1000  'Convert to milliseconds.
        If type = 0 Then
            GetTime = time
        Else
            GetTime = fade
        End If
        Exit Function
GetTime_error_handler:
        GetTime = 0
    End Function

    Public Function GetTag(ByRef tagID As Short) As String
        On Error Resume Next
        Dim taginfo(3) As Byte
        Dim headerlen As Integer
        Dim i As Integer
        Dim tagdata As Short
        Dim j As Short
        Dim strTAG As String
        Dim text_format As Integer = 0
        strTAG = ""

        FileGet(1, taginfo, &H10200 + 1)
        If Chr(taginfo(0)) = "x" And Chr(taginfo(1)) = "i" And Chr(taginfo(2)) = "d" And Chr(taginfo(3)) = "6" Then

            FileGet(1, headerlen, &H10204 + 1)
            ReDim taginfo(headerlen - 1)

            FileGet(1, taginfo, &H10208 + 1)
            strTAG = ""
            For i = 0 To headerlen - 1 Step 4
                If tagID = taginfo(i) Then
                    tagdata = taginfo(i + 2) + (taginfo(i + 3) * 256)
                    For j = i + 4 To tagdata + i + 4 - 1
                        If taginfo(j) = 0 Then Exit For
                        strTAG = strTAG & Chr(taginfo(j))
                    Next j
                    Exit For
                Else
                    If taginfo(i + 1) <> 0 Then
                        i = i + taginfo(i + 2) + (taginfo(i + 3) * 256)
                        i = i + 3
                        i = i \ 4    'Strings HAVE to be aligned to the next 32 bit boundary.
                        i = i * 4
                    End If
                End If
            Next i
        Else
            '
        End If

        'Now prove that the spc is NOT binary format. Artist info hinges on that.
        ReDim taginfo(255)  'Read out the entire SPC file header.
        FileGet(1, taginfo, &H0 + 1)

        If ((taginfo(&H9E) > 0) And (taginfo(&H9E) < &H30)) Or (taginfo(&H9E) > &H39) Then
            If ((taginfo(&H9F) > 0) And (taginfo(&H9F) < &H30)) Or (taginfo(&H9F) > &H39) Then
                text_format = 0 'Date was proven to be not 0, and not numeric text characters.
            End If
        End If



        If strTAG = "" Then
            Select Case tagID
                Case 1
                    ReDim taginfo(31)

                    FileGet(1, taginfo, &H2E + 1)
                    If taginfo(0) <> 0 Then
                        For i = 0 To 31
                            If taginfo(i) = 0 Then Exit For
                            strTAG = strTAG & Chr(taginfo(i))
                        Next i
                    End If
                Case 2
                    ReDim taginfo(31)

                    FileGet(1, taginfo, &H4E + 1)
                    If taginfo(0) <> 0 Then
                        For i = 0 To 31
                            If taginfo(i) = 0 Then Exit For
                            strTAG = strTAG & Chr(taginfo(i))
                        Next i
                    End If
                Case 3
                    ReDim taginfo(31)

                    FileGet(1, taginfo, &HB0 + text_format + 1)
                    If taginfo(1) <> 0 Then
                        For i = 0 To 31
                            If taginfo(i) = 0 Then Exit For
                            strTAG = strTAG & Chr(taginfo(i))
                        Next i
                    End If
                Case 4
                    ReDim taginfo(15)

                    FileGet(1, taginfo, &H6E + 1)
                    If taginfo(1) <> 0 Then
                        For i = 0 To 15
                            If taginfo(i) = 0 Then Exit For
                            strTAG = strTAG & Chr(taginfo(i))
                        Next i
                    End If
                Case 7
                    ReDim taginfo(31)

                    FileGet(1, taginfo, &H7E + 1)
                    If taginfo(1) <> 0 Then
                        For i = 0 To 31
                            If taginfo(i) = 0 Then Exit For
                            strTAG = strTAG & Chr(taginfo(i))
                        Next i
                    End If
            End Select
        End If
        GetTag = strTAG
    End Function

    Public Sub testcode()
        Dim btestcode(&H1D) As Byte
        Dim i As Integer
        FileOpen(1, "c:\testcode.bin", OpenMode.Binary)

        FileGet(1, btestcode, &H0 + 1)
        FileClose(1)
        ResetAPU_internal()
        Do Until ReadSPC700_internal(0) = &HAA
        Loop
        WriteSPC700_internal(1, 1)
        WriteSPC700_internal(2, 3)
        WriteSPC700_internal(3, 0)
        WriteSPC700_internal(0, &HCC)
        Do Until ReadSPC700_internal(0) = &HCC
        Loop

        For i = 0 To &H1D
            WriteSPC700_internal(1, btestcode(i))
            WriteSPC700_internal(0, i)
            Do Until ReadSPC700_internal(0) = i
            Loop
        Next i
        WriteSPC700_internal(1, 0)
        i = ReadSPC700_internal(0)
        i = i + 2
        WriteSPC700_internal(0, i)
        FileOpen(1, "c:\spc.ram", OpenMode.Binary)
        Dim delaytime As Single
        For i = 0 To 65535
            delaytime = VB.Timer()
            'Do Until Timer - delaytime > 0.004
            'Loop

            If i Mod 1000 = 0 Then debugprint(i)

            ReadByte()
        Next i
        FileClose(1)

    End Sub

    Public Sub ReadByte()
        Dim i As Byte
        i = ReadSPC700_internal(0)
        'UPGRADE_WARNING: Put was upgraded to FilePut and has a new behavior. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
        FilePut(1, i)
        i = ReadSPC700_internal(1)
        i = i Xor 1
        WriteSPC700_internal(0, i)
        Do Until ReadSPC700_internal(1) = i
        Loop
    End Sub


    Public Function waitInport(ByVal InPort As Short, ByVal InData As Short, ByVal timeoutvalue As Short) As Boolean
        Dim timeout As Short
        'debugprint InPort, InData
        Do Until ReadSPC700_internal(InPort) = InData
            timeout = timeout + 1
            If timeout > timeoutvalue Then
                waitInport = False
                Exit Function
            End If
        Loop
        waitInport = True
    End Function

    'UPGRADE_NOTE: enabled was upgraded to enabled_Renamed. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
    Public Sub enablebuttons(ByRef enabled_Renamed As Boolean)
        tmrAutoPlay.Enabled = enabled_Renamed
        Button3.Visible = enabled_Renamed
        Button1.Enabled = enabled_Renamed
        Button2.Enabled = enabled_Renamed
        ListBox2.Enabled = enabled_Renamed
        txtIn(0).Enabled = enabled_Renamed
        txtIn(1).Enabled = enabled_Renamed
        txtIn(2).Enabled = enabled_Renamed
        txtIn(3).Enabled = enabled_Renamed
        HScroll1.Enabled = enabled_Renamed
        Text1.Enabled = enabled_Renamed
        cmdReset.Enabled = enabled_Renamed
        cmdLoadAPU.Enabled = enabled_Renamed
        cmdLoadPrev.Enabled = enabled_Renamed
        'tmrReadport.Enabled = enabled_Renamed
        If (enabled_Renamed) Then
            HScroll1_Change(HScroll1.Value)
        Else
            tmrReadport.Enabled = False
        End If
        'frmToP.Timer1.Enabled = False
    End Sub
    Private Sub HScroll1_Scroll(ByVal eventSender As Object, ByVal eventArgs As ScrollEventArgs) Handles HScroll1.Scroll
        Select Case eventArgs.Type
            Case ScrollEventType.EndScroll
                HScroll1_Change(eventArgs.NewValue)
        End Select
    End Sub

    Private Sub RadioButton1_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles RadioButton1.CheckedChanged
        If RadioButton1.Checked Then _freespacesearch = 0
    End Sub

    Private Sub RadioButton2_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles RadioButton2.CheckedChanged
        If RadioButton2.Checked Then _freespacesearch = 1
    End Sub

    Private Sub RadioButton4_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles RadioButton4.CheckedChanged
        If RadioButton4.Checked Then _freespacesearch = 2
    End Sub

    Private Sub RadioButton3_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles RadioButton3.CheckedChanged
        If RadioButton3.Checked Then _freespacesearch = 3
    End Sub

    Private Sub Button1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Button1.Click
        Dim curstr As String
        On Error GoTo button1_click_error
        Dim count As Integer = ListBox1.Items.Count()
        If (count = 0) Then
            Exit Sub
        End If
        If ((ListBox1.SelectedIndex + 1) = count) Then
            ListBox2.SelectedIndex = 0
            _strselectedspc = ListBox1.Items.Item(0).ToString()
            _intselectedspctrack = Int32.Parse(ListBox3.Items.Item(0).ToString())
        Else
            ListBox2.SelectedIndex = ListBox1.SelectedIndex + 1
            _strselectedspc = ListBox1.Items.Item(ListBox1.SelectedIndex).ToString()
            _intselectedspctrack = Int32.Parse(ListBox3.Items.Item(ListBox3.SelectedIndex).ToString())
        End If
        _strcurrentspc = _strselectedspc
        _intcurrentspctrack = _intselectedspctrack
        cmdLoadPrev_Click(sender, e)
        Exit Sub
button1_click_error:
        _strselectedspc = curstr
        _intselectedspctrack = -1
    End Sub

    Private Sub ListBox1_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles ListBox1.SelectedIndexChanged
        If (ListBox1.SelectedIndex >= 0) Then
            _strselectedspc = ListBox1.Items.Item(ListBox1.SelectedIndex).ToString()
        End If
    End Sub

    Private Sub ListBox1_DoubleClick(ByVal sender As Object, ByVal e As EventArgs) Handles ListBox1.DoubleClick, ListBox2.DoubleClick
        _strcurrentspc = _strselectedspc
        _intcurrentspctrack = _intselectedspctrack
        cmdLoadPrev_Click(sender, e)
    End Sub

    Private Sub Button2_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Button2.Click
        Dim curstr As String
        On Error GoTo button1_click_error
        Dim count As Integer = ListBox1.Items.Count()
        If (count = 0) Then
            Exit Sub
        End If
        If ((ListBox1.SelectedIndex) = 0) Then
            ListBox2.SelectedIndex = (count - 1)
            _strselectedspc = ListBox1.Items.Item(count - 1).ToString()
            _intselectedspctrack = Int32.Parse(ListBox3.Items.Item(count - 1).ToString())
        Else
            ListBox2.SelectedIndex = ListBox1.SelectedIndex - 1
            _strselectedspc = ListBox1.Items.Item(ListBox1.SelectedIndex).ToString()
            _intselectedspctrack = Int32.Parse(ListBox3.Items.Item(ListBox3.SelectedIndex).ToString())
        End If
        _strcurrentspc = _strselectedspc
        _intcurrentspctrack = _intselectedspctrack
        cmdLoadPrev_Click(sender, e)
        Exit Sub
button1_click_error:
        _strselectedspc = curstr
        _intselectedspctrack = -1
    End Sub

    Private Shared Sub SplitContainer1_Panel2_Paint(ByVal sender As Object, ByVal e As PaintEventArgs) Handles SplitContainer1.Panel2.Paint

    End Sub

    Private Sub ListBox2_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles ListBox2.SelectedIndexChanged
        ListBox1.SelectedIndex = ListBox2.SelectedIndex
        ListBox3.SelectedIndex = ListBox2.SelectedIndex
    End Sub

    Private Sub Button3_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Button3.Click
        On Error GoTo errorhandler
        Dim i As Integer
        Dim j As Integer = -1
        Dim k As Integer
        i = SPCbrowseOpen.ShowDialog()
        If (i = vbCancel) Then
            LoadAPU_Cancelled = True
            Exit Sub
        End If
        If (_listcleared) Then
            ListBox1.Items.Clear()
            ListBox2.Items.Clear()
            ListBox3.Items.Clear()
            _listcleared = False
        End If
        For i = 0 To (SPCbrowseOpen.FileNames.Length - 1)
            AddSPCtoList(SPCbrowseOpen.FileNames(i))
        Next
        Exit Sub
errorhandler:
        Resume Next
    End Sub

    Private Shared Sub form1_DragEnter(ByVal sender As Object, ByVal e As DragEventArgs) Handles MyBase.DragEnter
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            e.Effect = DragDropEffects.Copy
        Else
            e.Effect = DragDropEffects.None
        End If
    End Sub

    Private Sub AddSPCtoList(ByVal filename As String)
        Dim id666type As Integer
        Dim k As Integer = ListBox2.Items.Count()
        id666type = IsSPC(filename)
        If (id666type = ID6Type.ID6Sp2) Then
            GoTo AddSP2
        End If
        If (id666type <> ID6Type.ID6SPC) And (id666type <> ID6Type.ID6Ext) Then
            Exit Sub
        End If
        FileOpen(1, filename, OpenMode.Binary, OpenAccess.Read, OpenShare.Shared)
        'ReDim id666bytes(FileLen(SPCbrowseOpen.FileNames(i)))
        'FileGet(1, id666bytes, 0)
        'FileClose(1)
        'id666type = ID666_LoadTag(id666, id666bytes, FileLen(SPCbrowseOpen.FileNames(i)))
        'If ((id666type <> ID6Type.ID6_SPC) And (id666type <> ID6Type.ID6_EXT)) Then
        ListBox1.Items.Add(filename)
        'ID666_ToStr(id666, id666tag, encText.GetBytes("%3 - %2 - %4"))
        'ListBox2.Items.Add((i + k + 1).ToString() & ". " & id666tag.ToString())
        'FileOpen(1, strSPCFile, OpenMode.Binary)
        'lblSong.Text = "  " & GetTag(1) & "  "
        'lblGame.Text = "  " & GetTag(2) & "  "
        'lblArtist.Text = "  " & GetTag(3) & "  "
        Dim testFile As FileInfo
        testFile = My.Computer.FileSystem.GetFileInfo(filename)
        ListBox2.Items.Add((k + 1).ToString() & ". (" & testFile.Name & ") " & GetTag(IDX6TagType.Game_Name) & " - " & GetTag(IDX6TagType.Song_Name) & " - " & GetTag(IDX6TagType.Artist_Name))
        ListBox3.Items.Add((-1).ToString()) 'SPC Files only have one song.  -1 on the load handler means we are loading an spc.
        FileClose(1)
        Exit Sub
AddSP2:
        'SPC2 format file. This was devised by kevtris for his FPGA spc player, which does NOT have the ability to decompress rar files.
        FileOpen(1, filename, OpenMode.Binary, OpenAccess.Read, OpenShare.Shared)
        Dim spcCount As UShort
        Dim temp As Byte
        Dim i As Integer
        Dim songstring As String
        FileGet(1, temp, 7 + 1)
        spcCount = temp
        FileGet(1, temp, 8 + 1)
        spcCount = spcCount + (temp * 256)
        For i = 0 To spcCount - 1 Step 1
            ListBox1.Items.Add(filename)
            songstring = (i + k + 1).ToString() & ". (" & GetSP2Tag(SPC2TagType.SPC_Filename, i) & ".spc) " & GetSP2Tag(SPC2TagType.Game_Name, i)
            songstring = songstring & " - " & GetSP2Tag(SPC2TagType.Song_Name, i)
            songstring = songstring & " - " & GetSP2Tag(SPC2TagType.Artist_Name, i)
            ListBox2.Items.Add(songstring)
            ListBox3.Items.Add(i.ToString())
        Next
        FileClose(1)
    End Sub

    Private Sub form1_DragDrop(ByVal sender As Object, ByVal e As DragEventArgs) Handles MyBase.DragDrop
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            Dim filePaths As String() = CType(e.Data.GetData(DataFormats.FileDrop), String())
            For Each fileLoc As String In filePaths
                ' Code to read the contents of the text file
                If File.Exists(fileLoc) Then
                    AddSPCtoList(fileLoc)
                End If
            Next fileLoc
        End If
    End Sub

    Private Sub ListBox2_KeyPress(ByVal sender As Object, ByVal e As KeyPressEventArgs) Handles ListBox2.KeyPress
        Dim i As Integer = ListBox1.SelectedIndex
        If (e.KeyChar() = Chr(Keys.Back)) Then
            ListBox1.Items.RemoveAt(i)
            ListBox2.Items.RemoveAt(i)
            i = i - 1
            If (i < 0) Then
                i = 0
            End If
            If (ListBox2.Items.Count > 0) Then
                ListBox2.SelectedIndex = i
            End If
        End If
    End Sub

    Private Sub ListBox3_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles ListBox3.SelectedIndexChanged
        If (ListBox3.SelectedIndex >= 0) Then
            _intselectedspctrack = Int32.Parse(ListBox3.Items.Item(ListBox3.SelectedIndex).ToString())
        End If
    End Sub

    Private Shared Sub Panel3_Paint(ByVal sender As Object, ByVal e As PaintEventArgs) Handles Panel3.Paint

    End Sub

    Private Sub chkAutoPlay_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles chkAutoPlay.CheckedChanged
        If (chkAutoPlay.Checked()) Then
            If (_playtime <= 0) Then
                _playtime = tmrAutoPlay.Interval
            End If
        End If
    End Sub

    Private Sub tmrAutoPlay_Tick(ByVal sender As Object, ByVal e As EventArgs) Handles tmrAutoPlay.Tick
        Static timeout As Boolean = False
        Static timeoutticks As Integer = 5000

        If _justUploaded Then
            timeoutticks = 5000
            _justUploaded = False
            timeout = False
        End If

        If _playtime > 0 Then
            timeoutticks = timeoutticks - tmrAutoPlay.Interval
            _playtime = _playtime - tmrAutoPlay.Interval
            If timeoutticks <= 0 Then
                timeout = True
            End If

            If (timeout) Then
                Dim hours As Integer = 0, minutes As Integer = 0, seconds As Integer = 0, milliseconds As Integer = 0
                milliseconds = _playtime Mod 1000
                seconds = _playtime \ 1000
                minutes = seconds \ 60
                hours = minutes \ 60
                minutes = minutes Mod 60
                seconds = seconds Mod 60
                If Not chkAutoPlay.Checked() Then
                    txtUploadSpeed.Text = "Play Time Left: Infinite :)"
                Else
                    If (hours > 0) Then
                        txtUploadSpeed.Text = "Play Time Left: " & hours.ToString() & ":" & minutes.ToString("00") & ":" & seconds.ToString("00")
                    Else
                        txtUploadSpeed.Text = "Play Time Left: " & minutes.ToString() & ":" & seconds.ToString("00")
                    End If
                End If
            End If

            If (_playtime <= 0) And chkAutoPlay.Checked() Then
                timeout = False
                timeoutticks = 5000
                If ListBox2.SelectedIndex < (ListBox2.Items.Count - 1) Then
                    Button1_Click(sender, e)
                Else
                    cmdReset_Click(sender, e)
                End If
            End If
        End If
    End Sub

    Private Shared Sub Label15_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Label15.Click

    End Sub

    Private Shared Sub Label10_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Label10.Click

    End Sub

    Private Shared Sub Label5_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Label5.Click

    End Sub

    Private Shared Sub Label6_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Label6.Click

    End Sub

    Private Shared Sub Label7_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Label7.Click

    End Sub

    Private Shared Sub Label9_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Label9.Click

    End Sub

    Private Shared Sub Label11_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Label11.Click

    End Sub

    Private Sub txtUploadSpeed_TextChanged(sender As System.Object, e As System.EventArgs) Handles txtUploadSpeed.TextChanged

    End Sub
End Class