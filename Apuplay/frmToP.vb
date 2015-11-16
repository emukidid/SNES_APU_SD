Option Strict Off
Option Explicit On
Imports VB = Microsoft.VisualBasic


Friend Class frmToP
	Inherits System.Windows.Forms.Form
	
    Private Declare Function WriteSPC700 Lib "APU_DLL.DLL" (ByVal Address As Integer, ByVal data As Integer) As Integer
	
    Private Declare Function ReadSPC700 Lib "APU_DLL.DLL" (ByVal Address As Integer) As Integer
	
	
	Dim nPort0State As Byte
	Dim nPrevPort0 As Byte
	Dim eventfired As Boolean
    Dim commandbyte As Byte

    Private top_events_allowed As Boolean = False

    Sub allow_top_events(ByVal enable_top_events As Boolean)
        top_events_allowed = enable_top_events
    End Sub

    Sub grafic_change(ByVal Index As Integer, ByVal CValue As Integer)
        Dim portstate As Byte
        portstate = ReadSPC700(0)
        Select Case form1.lblGame.Text
            '        Case "  Tales of Phantasia  "
            '            WriteSPC700 1, Index * 4
            '            WriteSPC700 2, (SldGraficEqualizer(Index).Value * -1 * 8) + &H87
            '            WriteSPC700 3, 1
            '            form1.debugprint "ToP"
            Case "  Star Ocean  ", "  Tales of Phantasia  "
                WriteSPC700(1, Index * 2)
                WriteSPC700(2, (CValue * -1 * 8) + &H87)
                WriteSPC700(3, &HB7)
                form1.debugprint("SO")
        End Select
        sendCommand((&H1D))
        eventfired = Not eventfired
        WaitforStatechange(portstate)
    End Sub
	
	
	'UPGRADE_WARNING: Event cboGEDSP.SelectedIndexChanged may fire when form is initialized. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="88B12AE1-6DE0-48A0-86F1-60C0686C026A"'
	Private Sub cboGEDSP_SelectedIndexChanged(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles cboGEDSP.SelectedIndexChanged
        On Error Resume Next
        If (Not top_events_allowed) Then Exit Sub
        Dim i As Short
		Dim portstate As Byte
		portstate = ReadSPC700(0)
		Select Case cboGEDSP.Text
			Case "Flat"
                _SldGraficEqualizer_0.Value = ((&HBF - &H87) / 8) * -1
                _SldGraficEqualizer_1.Value = ((&HBF - &H87) / 8) * -1
                _SldGraficEqualizer_2.Value = ((&HBF - &H87) / 8) * -1
                _SldGraficEqualizer_3.Value = ((&HBF - &H87) / 8) * -1
                _SldGraficEqualizer_4.Value = ((&HBF - &H87) / 8) * -1
                _SldGraficEqualizer_5.Value = ((&HBF - &H87) / 8) * -1
                _SldGraficEqualizer_6.Value = ((&HBF - &H87) / 8) * -1
                _SldGraficEqualizer_7.Value = ((&HBF - &H87) / 8) * -1
                _SldGraficEqualizer_8.Value = ((&HBF - &H87) / 8) * -1
                form1.debugprint("Flat")
            Case "Bass Boost"

                _SldGraficEqualizer_0.Value = ((&HFF - &H87) / 8) * -1
                _SldGraficEqualizer_1.Value = ((&HFF - &H87) / 8) * -1
                _SldGraficEqualizer_2.Value = ((&HFF - &H87) / 8) * -1
                _SldGraficEqualizer_3.Value = ((&HF7 - &H87) / 8) * -1
                _SldGraficEqualizer_4.Value = ((&HCF - &H87) / 8) * -1
                _SldGraficEqualizer_5.Value = ((&HAF - &H87) / 8) * -1
                _SldGraficEqualizer_6.Value = ((&H9F - &H87) / 8) * -1
                _SldGraficEqualizer_7.Value = ((&H97 - &H87) / 8) * -1
                _SldGraficEqualizer_8.Value = ((&H8F - &H87) / 8) * -1

                form1.debugprint("Bass Boost")
			Case "Arena"
                _SldGraficEqualizer_0.Value = ((&HFF - &H87) / 8) * -1
                _SldGraficEqualizer_1.Value = ((&HFF - &H87) / 8) * -1
                _SldGraficEqualizer_2.Value = ((&HF7 - &H87) / 8) * -1
                _SldGraficEqualizer_3.Value = ((&HD7 - &H87) / 8) * -1
                _SldGraficEqualizer_4.Value = ((&HA7 - &H87) / 8) * -1
                _SldGraficEqualizer_5.Value = ((&H8F - &H87) / 8) * -1
                _SldGraficEqualizer_6.Value = ((&HA7 - &H87) / 8) * -1
                _SldGraficEqualizer_7.Value = ((&HEF - &H87) / 8) * -1
                _SldGraficEqualizer_8.Value = ((&HFF - &H87) / 8) * -1
				
                form1.debugprint("Arena")
        End Select
        grafic_change(0, _SldGraficEqualizer_0.Value)
        grafic_change(1, _SldGraficEqualizer_1.Value)
        grafic_change(2, _SldGraficEqualizer_2.Value)
        grafic_change(3, _SldGraficEqualizer_3.Value)
        grafic_change(4, _SldGraficEqualizer_4.Value)
        grafic_change(5, _SldGraficEqualizer_5.Value)
        grafic_change(6, _SldGraficEqualizer_6.Value)
        grafic_change(7, _SldGraficEqualizer_7.Value)
        grafic_change(8, _SldGraficEqualizer_8.Value)
	End Sub
	
	Private Sub cmdFastForward_MouseDown(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.MouseEventArgs) Handles cmdFastForward.MouseDown
		Dim Button As Short = eventArgs.Button \ &H100000
		Dim Shift As Short = System.Windows.Forms.Control.ModifierKeys \ &H10000
		Dim X As Single = VB6.PixelsToTwipsX(eventArgs.X)
		Dim Y As Single = VB6.PixelsToTwipsY(eventArgs.Y)
		Dim portstate As Byte
		portstate = ReadSPC700(0)
		WriteSPC700(1, &H78)
		WriteSPC700(2, &HFF)
		WriteSPC700(3, &HFC)
        sendCommand((&H11))
        eventfired = Not eventfired
		WaitforStatechange(portstate)
	End Sub
	
	Private Sub cmdFastForward_MouseUp(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.MouseEventArgs) Handles cmdFastForward.MouseUp
		Dim Button As Short = eventArgs.Button \ &H100000
		Dim Shift As Short = System.Windows.Forms.Control.ModifierKeys \ &H10000
		Dim X As Single = VB6.PixelsToTwipsX(eventArgs.X)
		Dim Y As Single = VB6.PixelsToTwipsY(eventArgs.Y)
		Dim portstate As Byte
		portstate = ReadSPC700(0)
		WriteSPC700(1, &H40)
		WriteSPC700(2, &HFF)
		WriteSPC700(3, &HFC)
        sendCommand((&H11))
        eventfired = Not eventfired
		WaitforStatechange(portstate)
	End Sub
	
	Private Sub Command1_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles Command1.Click
        'Dim i As Short
		
		'   checkStateType
		
		
		
		vsVOL.Value = 63 * -1
        grafic_change(0, _SldGraficEqualizer_0.Value)
        grafic_change(1, _SldGraficEqualizer_1.Value)
        grafic_change(2, _SldGraficEqualizer_2.Value)
        grafic_change(3, _SldGraficEqualizer_3.Value)
        grafic_change(4, _SldGraficEqualizer_4.Value)
        grafic_change(5, _SldGraficEqualizer_5.Value)
        grafic_change(6, _SldGraficEqualizer_6.Value)
        grafic_change(7, _SldGraficEqualizer_7.Value)
        grafic_change(8, _SldGraficEqualizer_8.Value)
		optOutput(2).Checked = True
		sldPitch.Value = 32
		sldTrans.Value = 8
        sldPan.Value = 32 * -1
        sldPan_Change(eventSender, New EventArgs())
        sldTrans_Change(eventSender, New EventArgs())
        sldPitch_Change(eventSender, New EventArgs())
        optReverb.Checked = True
        Timer1.Enabled = True
        Timer1.Interval = 50
		
	End Sub
	
	Private Sub cmdPause_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles cmdPause.Click
		Dim portstate As Byte
		portstate = ReadSPC700(0)
		WriteSPC700(1, &H12)
		WriteSPC700(2, &HBF)
		WriteSPC700(3, &H1)
		If cmdPause.Text = "Pause" Then
			cmdPause.Text = "Resume"
			sendCommand((&H12))
		Else
			cmdPause.Text = "Pause"
			sendCommand((&H13))
        End If
        eventfired = Not eventfired
		WaitforStatechange(portstate)
	End Sub
	
	Private Sub cmdPlay_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles cmdPlay.Click
		Dim portstate As Byte
		Dim i As Short
		portstate = ReadSPC700(0)
		WriteSPC700(1, 0)
		WriteSPC700(2, 0)
		WriteSPC700(3, &H8E)
        sendCommand((&H6))
        eventfired = Not eventfired
        WaitforStatechange(portstate)
        vsVOL_Change(vsVOL.Value)

        grafic_change(0, _SldGraficEqualizer_0.Value)
        grafic_change(1, _SldGraficEqualizer_1.Value)
        grafic_change(2, _SldGraficEqualizer_2.Value)
        grafic_change(3, _SldGraficEqualizer_3.Value)
        grafic_change(4, _SldGraficEqualizer_4.Value)
        grafic_change(5, _SldGraficEqualizer_5.Value)
        grafic_change(6, _SldGraficEqualizer_6.Value)
        grafic_change(7, _SldGraficEqualizer_7.Value)
        grafic_change(8, _SldGraficEqualizer_8.Value)
		For i = 0 To 2
			If optOutput(i).Checked Then optOutput_CheckedChanged(optOutput.Item(i), New System.EventArgs())
		Next i
		sldPitch_Change(sldPitch, New System.EventArgs())
		sldTrans_Change(sldTrans, New System.EventArgs())
		sldPan_Change(sldPan, New System.EventArgs())
		If optReverb.Checked Then optReverb_CheckedChanged(optReverb, New System.EventArgs())
		If optDry.Checked Then optDry_CheckedChanged(optDry, New System.EventArgs())
		If optPanDelay.Checked Then optPanDelay_CheckedChanged(optPanDelay, New System.EventArgs())
		
	End Sub
	
	'UPGRADE_WARNING: Event optDry.CheckedChanged may fire when form is initialized. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="88B12AE1-6DE0-48A0-86F1-60C0686C026A"'
	Private Sub optDry_CheckedChanged(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles optDry.CheckedChanged
        If (Not top_events_allowed) Then Exit Sub
        If eventSender.Checked Then
            Dim portstate As Byte
            portstate = ReadSPC700(0)
            WriteSPC700(1, &HFF)
            WriteSPC700(2, &HFF)
            WriteSPC700(3, &H0)
            sendCommand((&H19))
            eventfired = Not eventfired
            WaitforStatechange(portstate)
        End If
	End Sub
	
	'UPGRADE_WARNING: Event optOutput.CheckedChanged may fire when form is initialized. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="88B12AE1-6DE0-48A0-86F1-60C0686C026A"'
	Private Sub optOutput_CheckedChanged(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles optOutput.CheckedChanged
        If (Not top_events_allowed) Then Exit Sub
        If eventSender.Checked Then
            Dim Index As Short = optOutput.GetIndex(eventSender)
            Dim portstate As Byte
            portstate = ReadSPC700(0)
            WriteSPC700(1, Index)
            WriteSPC700(2, 1)
            WriteSPC700(3, 2)
            sendCommand((&HB))
            eventfired = Not eventfired
            WaitforStatechange(portstate)
        End If
	End Sub
	
	'UPGRADE_WARNING: Event optPanDelay.CheckedChanged may fire when form is initialized. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="88B12AE1-6DE0-48A0-86F1-60C0686C026A"'
	Private Sub optPanDelay_CheckedChanged(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles optPanDelay.CheckedChanged
        If (Not top_events_allowed) Then Exit Sub
        If eventSender.Checked Then
            Dim portstate As Byte
            portstate = ReadSPC700(0)
            WriteSPC700(1, &H74)
            WriteSPC700(2, &H1)
            WriteSPC700(3, &H5)
            sendCommand((&H19))
            eventfired = Not eventfired
            WaitforStatechange(portstate)
        End If
	End Sub
	
	'UPGRADE_WARNING: Event optReverb.CheckedChanged may fire when form is initialized. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="88B12AE1-6DE0-48A0-86F1-60C0686C026A"'
	Private Sub optReverb_CheckedChanged(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles optReverb.CheckedChanged
        If (Not top_events_allowed) Then Exit Sub
        If eventSender.Checked Then
            Dim portstate As Byte
            portstate = ReadSPC700(0)
            WriteSPC700(1, &H74)
            WriteSPC700(2, &H1)
            WriteSPC700(3, &H2)
            sendCommand((&H19))
            eventfired = Not eventfired
            WaitforStatechange(portstate)
        End If
	End Sub
	
    Private Sub sldPan_Change(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles sldPan.Scroll
        Dim portstate As Byte
        portstate = ReadSPC700(0)
        WriteSPC700(1, &H3F)
        WriteSPC700(2, &HFF)
        WriteSPC700(3, (sldPan.Value * -1 * 2) + 1)
        sendCommand((&HE))
        eventfired = Not eventfired
        WaitforStatechange(portstate)
    End Sub
	
	Private Sub Slider1_Change()
		
	End Sub
	
	
	
    Private Sub sldPitch_Change(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles sldPitch.Scroll
        Dim portstate As Byte
        portstate = ReadSPC700(0)
        WriteSPC700(1, &H3F)
        WriteSPC700(2, &HFF)
        WriteSPC700(3, sldPitch.Value * 2)
        sendCommand((&H10))
        eventfired = Not eventfired
        WaitforStatechange(portstate)
    End Sub
	
    Private Sub sldTrans_Change(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles sldTrans.Scroll
        Dim portstate As Byte
        portstate = ReadSPC700(0)
        WriteSPC700(1, &H3F)
        WriteSPC700(2, &HFF)
        WriteSPC700(3, (sldTrans.Value) - 8)
        sendCommand((&HF))
        eventfired = Not eventfired
        WaitforStatechange(portstate)
    End Sub
	
	Private Sub Stop_Renamed_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles Stop_Renamed.Click
		Dim portstate As Byte
		portstate = ReadSPC700(0)
		WriteSPC700(1, &H4)
		WriteSPC700(2, &H10)
		WriteSPC700(3, &H1)
        sendCommand((&H8))
        eventfired = Not eventfired
		WaitforStatechange(portstate)
		
	End Sub
	
	Private Sub Timer1_Tick(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles Timer1.Tick
		Dim portstate As Byte
		On Error Resume Next
		If Me.Visible Then
			If commandbyte <> &H1A Then
				WriteSPC700(1, &H3)
				WriteSPC700(2, &H4)
				WriteSPC700(3, &H5)
				portstate = ReadSPC700(0)
				sendCommand((&H1A))
				eventfired = Not eventfired
				WaitforStatechange(portstate)
			End If
			lblPattern.Text = "Pattern #: " & ReadSPC700(1)
			optTACT(ReadSPC700(2) - 1).Checked = True
			lblLoop.Text = "Loop Count: " & ReadSPC700(3)
			
		End If
	End Sub
	
	'UPGRADE_NOTE: vsVOL.Change was changed from an event to a procedure. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="4E2DC008-5EDA-4547-8317-C9316952674F"'
	'UPGRADE_WARNING: VScrollBar event vsVOL.Change has a new behavior. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"'
    Private Sub vsVOL_Change(ByVal newScrollValue As Integer)
        Dim portstate As Byte
        WriteSPC700(1, &H3F)
        WriteSPC700(2, &HFF)
        WriteSPC700(3, newScrollValue * 4 * -1)
        portstate = ReadSPC700(0)
        lblVOL.Text = CStr(newScrollValue * -1)
        sendCommand((&HD))
        eventfired = Not eventfired
        WaitforStatechange(portstate)
    End Sub
	
	Public Sub sendCommand(ByRef bCommand As Byte)
		commandbyte = bCommand
		nPort0State = ReadSPC700(0)
		If nPrevPort0 = 0 Then
			If (nPort0State And 128) Then
				WriteSPC700(0, bCommand)
			Else
				WriteSPC700(0, bCommand + 128)
			End If
		Else
			If (nPort0State And 128) Then
				WriteSPC700(0, bCommand + 128)
			Else
				WriteSPC700(0, bCommand)
			End If
		End If
	End Sub
	
	Public Sub checkStateType()
		Dim ftimer As Single
		nPort0State = ReadSPC700(0)
		WriteSPC700(1, &H0)
		WriteSPC700(2, &H0)
		WriteSPC700(3, &H0)
		If (nPort0State And 128) Then
			WriteSPC700(0, &H1A)
		Else
			WriteSPC700(0, &H9A)
		End If
		ftimer = VB.Timer()
		Do Until (VB.Timer() - ftimer) > 0.25
		Loop 
		nPrevPort0 = ReadSPC700(0)
		If nPrevPort0 = nPort0State Then
			nPrevPort0 = 1
		Else
			nPrevPort0 = 0
		End If
        Timer1.Enabled = True
        For i = 0 To 2
            If optOutput(i).Checked Then optOutput_CheckedChanged(optOutput.Item(i), New System.EventArgs())
        Next i
        If optReverb.Checked Then optReverb_CheckedChanged(optReverb, New System.EventArgs())
        If optDry.Checked Then optDry_CheckedChanged(optDry, New System.EventArgs())
        If optPanDelay.Checked Then optPanDelay_CheckedChanged(optPanDelay, New System.EventArgs())
        sldPitch_Change(sldPitch, New System.EventArgs())
        sldTrans_Change(sldTrans, New System.EventArgs())
        sldPan_Change(sldPan, New System.EventArgs())
        vsVOL_Change(vsVOL.Value)
        grafic_change(0, _SldGraficEqualizer_0.Value)
        grafic_change(1, _SldGraficEqualizer_1.Value)
        grafic_change(2, _SldGraficEqualizer_2.Value)
        grafic_change(3, _SldGraficEqualizer_3.Value)
        grafic_change(4, _SldGraficEqualizer_4.Value)
        grafic_change(5, _SldGraficEqualizer_5.Value)
        grafic_change(6, _SldGraficEqualizer_6.Value)
        grafic_change(7, _SldGraficEqualizer_7.Value)
        grafic_change(8, _SldGraficEqualizer_8.Value)
        
        
        

	End Sub
	
	Public Sub WaitforStatechange(ByRef portstate As Byte)
		Static eventhandled As Boolean
		If eventfired = eventhandled Then
		Else
			eventhandled = eventfired
			If (portstate And 128) Then
				Do Until (portstate And 128) = False
					portstate = ReadSPC700(0)
					If portstate = &HAA Then
						If ReadSPC700(1) = &HBB Then
							Timer1.Enabled = False
							Exit Do
						End If
					End If
				Loop 
			Else
                Do Until (portstate And 128)
                    portstate = ReadSPC700(0)
                    If portstate = &HAA Then
                        If ReadSPC700(1) = &HBB Then
                            Timer1.Enabled = False
                            Exit Do
                        End If
                    End If
                Loop
			End If
		End If
	End Sub
	Private Sub vsVOL_Scroll(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.ScrollEventArgs) Handles vsVOL.Scroll
		Select Case eventArgs.type
			Case System.Windows.Forms.ScrollEventType.EndScroll
				vsVOL_Change(eventArgs.newValue)
		End Select
	End Sub

    Private Sub frmToP_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

    End Sub

    Private Sub _SldGraficEqualizer_0_Change(ByVal sender As Object, ByVal e As System.EventArgs) Handles _SldGraficEqualizer_0.Scroll
        grafic_change(0, _SldGraficEqualizer_0.Value)
    End Sub

    Private Sub _SldGraficEqualizer_1_Change(ByVal sender As Object, ByVal e As System.EventArgs) Handles _SldGraficEqualizer_1.Scroll
        grafic_change(1, _SldGraficEqualizer_1.Value)
    End Sub

    Private Sub _SldGraficEqualizer_2_Change(ByVal sender As Object, ByVal e As System.EventArgs) Handles _SldGraficEqualizer_2.Scroll
        grafic_change(2, _SldGraficEqualizer_2.Value)
    End Sub

    Private Sub _SldGraficEqualizer_3_Change(ByVal sender As Object, ByVal e As System.EventArgs) Handles _SldGraficEqualizer_3.Scroll
        grafic_change(3, _SldGraficEqualizer_3.Value)
    End Sub

    Private Sub _SldGraficEqualizer_4_Change(ByVal sender As Object, ByVal e As System.EventArgs) Handles _SldGraficEqualizer_4.Scroll
        grafic_change(4, _SldGraficEqualizer_4.Value)
    End Sub

    Private Sub _SldGraficEqualizer_5_Change(ByVal sender As Object, ByVal e As System.EventArgs) Handles _SldGraficEqualizer_5.Scroll
        grafic_change(5, _SldGraficEqualizer_5.Value)
    End Sub

    Private Sub _SldGraficEqualizer_6_Change(ByVal sender As Object, ByVal e As System.EventArgs) Handles _SldGraficEqualizer_6.Scroll
        grafic_change(6, _SldGraficEqualizer_6.Value)
    End Sub

    Private Sub _SldGraficEqualizer_7_Change(ByVal sender As Object, ByVal e As System.EventArgs) Handles _SldGraficEqualizer_7.Scroll
        grafic_change(7, _SldGraficEqualizer_7.Value)
    End Sub

    Private Sub _SldGraficEqualizer_8_Change(ByVal sender As Object, ByVal e As System.EventArgs) Handles _SldGraficEqualizer_8.Scroll
        grafic_change(8, _SldGraficEqualizer_8.Value)
    End Sub


End Class