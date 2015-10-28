Option Strict Off
Option Explicit On
Imports VB = Microsoft.VisualBasic
Friend Class frmSongSelect
	Inherits System.Windows.Forms.Form
    Private Declare Function WriteSPC700 Lib "APU_DLL.DLL" (ByVal Address As Integer, ByVal data As Integer) As Integer
	
    Private Declare Function ReadSPC700 Lib "APU_DLL.DLL" (ByVal Address As Integer) As Integer
	
	
	Dim gamenumber As Short
	
	Public Sub checkgame()
		Select Case Form1.lblGame.Text
			Case "  Super Mario World  "
				
				' Me.Show
			Case Else
				
				'Me.Show
		End Select
	End Sub
	
	
	
	
    'Private Sub Slider1_Change(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles Slider1.Change
    '	Text1_TextChanged(Text1, New System.EventArgs())
    'End Sub

    'UPGRADE_WARNING: Event Text1.TextChanged may fire when form is initialized. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="88B12AE1-6DE0-48A0-86F1-60C0686C026A"'
    Private Sub Text1_TextChanged(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles Text1.TextChanged
        Dim i As Short
        Dim addr As Short
        Dim data As Short
        'UPGRADE_NOTE: command was upgraded to command_Renamed. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
        Dim command_Renamed As String
        On Error GoTo handleerror
        i = 0
        If VB.Right(Text1.Text, 5) = "START" Then
            Do Until Mid(Text1.Text, i + 1, 5) = "START"
                Text1.SelectionStart = i
                Text1.SelectionLength = 7
                command_Renamed = Mid(Text1.SelectedText, 3, 1)
                If command_Renamed <> ":" Then
                    Text1.SelectionStart = Len(Text1.Text)
                    Exit Sub
                End If
                i = i + 7
            Loop
            i = 0
            Do Until Mid(Text1.Text, i + 1, 7) = "START"
                Text1.SelectionStart = i
                Text1.SelectionLength = 7
                form1.debugprint(Text1.SelectedText)
                command_Renamed = Mid(Text1.SelectedText, 1, 1)
                If command_Renamed = "R" Then
                    addr = CByte(Mid(Text1.SelectedText, 2, 1))
                    data = CByte("&H" & Mid(Text1.SelectedText, 4, 2))
                    Do Until ReadSPC700(addr) = data
                    Loop
                ElseIf command_Renamed = "W" Then
                    addr = CByte(Mid(Text1.SelectedText, 2, 1))
                    data = CByte("&H" & Mid(Text1.SelectedText, 4, 2))
                    WriteSPC700(addr, data)
                ElseIf command_Renamed = "P" Then
                    addr = CByte(Mid(Text1.SelectedText, 2, 1))
                    Do Until ReadSPC700(addr) = data
                    Loop
                ElseIf command_Renamed = "Q" Then
                    Do Until ReadSPC700(addr) = data
                    Loop
                ElseIf command_Renamed = "S" Then
                    addr = CByte(Mid(Text1.SelectedText, 2, 1))
                    data = 250 'Slider1.Value
                    WriteSPC700(addr, data)
                ElseIf command_Renamed = "M" Then
                    'Slider1.Max = CByte("&H" & Mid(Text1.SelectedText, 4, 2))
                ElseIf command_Renamed = "N" Then
                    'Slider1.Min = CByte("&H" & Mid(Text1.SelectedText, 4, 2))

                End If
                i = i + 7
            Loop
            Text1.SelectionStart = Len(Text1.Text)
            '   Text1.Text = ""
        End If
        Exit Sub
handleerror:
        If Err.Number = 380 Then MsgBox("Reverse the Min/Max setting")
    End Sub
End Class