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
            If My.Settings.LeftKbdPos <> "" Then
                Dim spl() As String = My.Settings.LeftKbdPos.Split(","c)
                Me.Location = New Point(CInt(spl(0)), CInt(spl(1)))
            Else
                Me.Left = FrmCalc.Left
                Me.Top = FrmCalc.Bottom
                My.Settings.LeftKbdPos = Me.Left & "," & Me.Top
                My.Settings.Save()
            End If
        End Sub

        Private Sub btnT_Click(sender As Object, e As EventArgs) Handles btnY.Click, btnX.Click, btn9.Click, btn8.Click, btn7.Click,
            btn6.Click, btn5.Click, btn4.Click, btn3.Click, btn2.Click, btn1.Click, btn0.Click, btnPt.Click,
            btnZ.Click, btnPi.Click, btnE.Click, btnImagUnit.Click
            Dim btn As Button = DirectCast(sender, Button)
            'pnl.Focus()
            Dim start As Integer = FrmCalc.tb.SelectionStart
            FrmCalc.tb.Text = FrmCalc.tb.Text.Remove(FrmCalc.tb.SelectionStart, FrmCalc.tb.SelectionEnd - FrmCalc.tb.SelectionStart)
            FrmCalc.tb.Focus()
            FrmCalc.tb.Text = FrmCalc.tb.Text.Insert(start, btn.Text)
            FrmCalc.tb.SelectionStart = start + btn.Text.Length
        End Sub
        Private Sub WriteFunction(ByVal s As String)
            FrmCalc.tb.Focus()
            Dim start As Integer = FrmCalc.tb.SelectionStart
            FrmCalc.tb.Text = FrmCalc.tb.Text.Remove(FrmCalc.tb.SelectionStart, FrmCalc.tb.SelectionEnd - FrmCalc.tb.SelectionStart).Insert(start, s)
            If s.Contains("(") Then
                FrmCalc.tb.SelectionStart = start + s.IndexOf("(") + 1
            Else
                FrmCalc.tb.SelectionStart = start + s.Count()
            End If
        End Sub

        Private Sub btnAlert_Click(sender As Object, e As EventArgs)
            WriteFunction("alert()")
        End Sub

        Private Sub btnConfirm_Click(sender As Object, e As EventArgs)
            WriteFunction("confirm()")
        End Sub

        Private Sub btnInput_Click(sender As Object, e As EventArgs)
            WriteFunction("input()")
        End Sub

        Private Sub btnChoose_Click(sender As Object, e As EventArgs)
            WriteFunction(" choose ")
        End Sub

        Private Sub btnPerm_Click(sender As Object, e As EventArgs)
            WriteFunction("nPr()")
        End Sub

        Private Sub OskLeft_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
            FrmCalc.Close()
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
            My.Settings.MainPos = FrmCalc.Left & "," & FrmCalc.Top
            My.Settings.LeftKbdPos = Me.Left & "," & Me.Top
            My.Settings.RightKbdPos = OskRight.Left & "," & OskRight.Top
            My.Settings.OskLock = Me.Snap
            My.Settings.LOskSnap = FrmCalc.LSnap
            My.Settings.ROskSnap = FrmCalc.RSnap
            My.Settings.ROskSnap = FrmCalc.RSnap
            My.Settings.Save()
        End Sub

        Private Sub osk_Enter(sender As Object, e As EventArgs) Handles btnPi.Enter, btnE.Enter, btnZ.Enter, btnY.Enter,
        btnX.Enter, btnPt.Enter, btnImagUnit.Enter, btn9.Enter, btn8.Enter, btn7.Enter, btn6.Enter, btn5.Enter,
        btn4.Enter, btn3.Enter, btn2.Enter, btn1.Enter, btn0.Enter
            If (Not DirectCast(sender, Control).Name = "pnl") Then pnl.Focus()
            FrmCalc.tb.Focus()
            'frmCalc.BringToFront()
        End Sub
    End Class
End Namespace