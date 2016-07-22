Namespace Calculator.Keyboards
    Public Class OskLeft
        Protected Overloads Overrides ReadOnly Property CreateParams() As CreateParams
            Get
                Dim cp As CreateParams = MyBase.CreateParams
                ' turn on WS_EX_TOOLWINDOW style bit
                cp.ExStyle = cp.ExStyle Or &H80
                Return cp
            End Get
        End Property

        Public Snap As Boolean = False

        Private Sub TmrLoad_Tick(sender As Object, e As EventArgs) Handles TmrLoad.Tick
            TmrLoad.Stop()
            Me.Show()
            Me.SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.OptimizedDoubleBuffer, True)
            Me.Snap = My.Settings.OskLock
            If My.Settings.LeftOskPos <> "" Then
                Dim spl() As String = My.Settings.LeftOskPos.Split(","c)
                Me.Location = New Point(CInt(spl(0)), CInt(spl(1)))
                If Me.Left >= -100 And Me.Top >= 0 And Me.Right <= Screen.PrimaryScreen.WorkingArea.Width + 100 And
                Me.Bottom <= Screen.PrimaryScreen.WorkingArea.Height + 500 Then Exit Sub
            End If
            Me.Left = CInt(Screen.PrimaryScreen.WorkingArea.Width / 2 - FrmCalc.Width / 2)
            Me.Top = CInt(Screen.PrimaryScreen.WorkingArea.Height / 6 + FrmCalc.Height / 2)
            Me.Width = btn3.Right + 12
            My.Settings.LeftOskPos = Me.Left & "," & Me.Top
            My.Settings.Save()
        End Sub

        Private Sub btnT_Click(sender As Object, e As EventArgs) Handles btnY.Click, btnX.Click, btnT.Click, btnM.Click, btnD.Click, btnC.Click, btnB.Click, btnA.Click, btn9.Click, btn8.Click, btn7.Click, btn6.Click, btn5.Click, btn4.Click, btn3.Click, btn2.Click, btn1.Click, btn0.Click, btnPt.Click, btnZ.Click, btnPi.Click, btnE.Click, btnSpace.Click
            Dim btn As Button = DirectCast(sender, Button)
            'pnl.Focus()
            Dim start As Integer = FrmCalc.tb.SelectionStart
            FrmCalc.tb.Text = FrmCalc.tb.Text.Remove(FrmCalc.tb.SelectionStart, FrmCalc.tb.SelectionLength)
            FrmCalc.tb.Focus()
            FrmCalc.tb.Text = frmCalc.tb.Text.Insert(start, btn.Text)
            frmCalc.tb.SelectionStart = start + btn.Text.Length
        End Sub
        Private Sub WriteFunction(ByVal s As String)
            frmCalc.tb.Focus()
            Dim start As Integer = FrmCalc.tb.SelectionStart
            FrmCalc.tb.Text = FrmCalc.tb.Text.Remove(FrmCalc.tb.SelectionStart, FrmCalc.tb.SelectionLength).Insert(start, s)
            If s.Contains("(") Then
                frmCalc.tb.SelectionStart = start + s.IndexOf("(") + 1
            Else
                frmCalc.tb.SelectionStart = start + s.Count()
            End If
        End Sub

        Private Sub btnFileLine_Click(sender As Object, e As EventArgs) Handles btnFileLine.Click
            WriteFunction(" comb ")
        End Sub

        Private Sub btnFile_Click(sender As Object, e As EventArgs) Handles btnFile.Click
            WriteFunction("write()")
        End Sub

        Private Sub btnClip_Click(sender As Object, e As EventArgs) Handles btnClip.Click
            WriteFunction("clipboard()")
        End Sub

        Private Sub btnFileAppend_Click(sender As Object, e As EventArgs) Handles btnFileAppend.Click
            WriteFunction("read()")
        End Sub

        Private Sub OskLeft_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
            frmCalc.Close()
        End Sub
        Private _isMoving As Boolean
        Private _movingPrevPt As Point

        Private Sub osk_MouseDown(sender As Object, e As MouseEventArgs) Handles MyBase.MouseDown, pnl.MouseDown
            If e.Button <> MouseButtons.Right Then
                _movingPrevPt = e.Location
                _isMoving = True
                'frmCalc.BringToFront()
            End If
        End Sub

        Private Sub osk_MouseMove(sender As Object, e As MouseEventArgs) Handles MyBase.MouseMove, pnl.MouseMove
            If _isMoving Then
                Me.Left += e.X - _movingPrevPt.X
                Me.Top += e.Y - _movingPrevPt.Y
                If FrmCalc.LSnap Then
                    FrmCalc.Left += e.X - _movingPrevPt.X
                    FrmCalc.Top += e.Y - _movingPrevPt.Y
                    If FrmCalc.RSnap Then
                        OskRight.Left += e.X - _movingPrevPt.X
                        OskRight.Top += e.Y - _movingPrevPt.Y
                    End If
                ElseIf OskRight.Snap Then
                    OskRight.Left += e.X - _movingPrevPt.X
                    OskRight.Top += e.Y - _movingPrevPt.Y
                End If
            End If
        End Sub

        Private Sub osk_MouseUp(sender As Object, e As MouseEventArgs) Handles MyBase.MouseUp, pnl.MouseUp
            If e.Button = MouseButtons.Right Then
                If FrmCalc.LSnap Then
                    FrmCalc.LSnap = False
                    Me.Top += 6
                    If Snap Then
                        OskRight.Top += 6
                        FrmCalc.RSnap = False
                    End If
                End If
                If Snap Then
                    Me.Snap = False
                    OskRight.Snap = False
                    If Me.Left < OskRight.Left Then
                        Me.Left -= 9
                        OskRight.Left += 9
                    Else
                        Me.Left += 9
                        OskRight.Left -= 9
                    End If
                End If
            Else
                _isMoving = False
                If Me.Right > OskRight.Left AndAlso Me.Right < OskRight.Right AndAlso (Me.Bottom > OskRight.Top - 100 AndAlso Me.Top < OskRight.Bottom + 100) Then
                    Me.Left = OskRight.Left - Me.Width + 12
                    Me.Top = OskRight.Top
                    Me.Snap = True
                    OskRight.Snap = True
                    If FrmCalc.RSnap Then FrmCalc.LSnap = True
                ElseIf Me.Left < OskRight.Right AndAlso Me.Left > OskRight.Left AndAlso (Me.Bottom > OskRight.Top - 100 AndAlso Me.Top < OskRight.Bottom + 100) Then
                    Me.Left = OskRight.Right - 12
                    Me.Top = OskRight.Top
                    Me.Snap = True
                    OskRight.Snap = True
                    If FrmCalc.RSnap Then FrmCalc.LSnap = True
                ElseIf Me.Right > Screen.PrimaryScreen.WorkingArea.Right
                    Me.Left = Screen.PrimaryScreen.WorkingArea.Right - Me.Width + 12
                ElseIf Me.Left < 0
                    Me.Left = -12
                End If
                If Me.Top < FrmCalc.Bottom AndAlso Me.Top > FrmCalc.Top AndAlso (Me.Left < FrmCalc.Right - 47 AndAlso Me.Right > FrmCalc.Left - 1) Then
                    Me.Left = FrmCalc.Left
                    Me.Top = FrmCalc.Bottom
                    FrmCalc.LSnap = True
                    If Me.Snap Then
                        OskRight.Left = FrmCalc.Right - 53 - OskRight.Width
                        OskRight.Top = FrmCalc.Bottom
                        FrmCalc.RSnap = True
                    End If
                End If
            End If
            My.Settings.MainPos = frmCalc.Left & "," & frmCalc.Top
            My.Settings.LeftOskPos = Me.Left & "," & Me.Top
            My.Settings.RightOskPos = OskRight.Left & "," & OskRight.Top
            My.Settings.OskLock = Me.Snap
            My.Settings.LOskSnap = frmCalc.LSnap
            My.Settings.ROskSnap = frmCalc.RSnap
            My.Settings.ROskSnap = frmCalc.RSnap
            My.Settings.Save()
        End Sub

        Private Sub osk_Enter(sender As Object, e As EventArgs) Handles btnPi.Enter, btnE.Enter, btnZ.Enter, btnY.Enter,
        btnX.Enter, btnT.Enter, btnSpace.Enter, btnPt.Enter, btnM.Enter, btnFileLine.Enter, btnFileAppend.Enter, btnFile.Enter,
        btnD.Enter, btnClip.Enter, btnC.Enter, btnB.Enter, btnA.Enter, btn9.Enter, btn8.Enter, btn7.Enter, btn6.Enter, btn5.Enter,
        btn4.Enter, btn3.Enter, btn2.Enter, btn1.Enter, btn0.Enter
            If (Not DirectCast(sender, Control).Name = "pnl") Then pnl.Focus()
            frmCalc.tb.Focus()
            'frmCalc.BringToFront()
        End Sub
    End Class
End Namespace