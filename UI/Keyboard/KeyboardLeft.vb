Namespace UI.Keyboards
    Public Class KeyboardLeft
        Protected Overloads Overrides ReadOnly Property CreateParams() As CreateParams
            Get
                Dim cp As CreateParams = MyBase.CreateParams
                ' turn on WS_EX_TOOLWINDOW style bit
                cp.ExStyle = cp.ExStyle Or &H80
                Return cp
            End Get
        End Property

        Public Snap As Boolean = False

        Private Sub btnT_Click(sender As Object, e As EventArgs) Handles btnY.Click, btnX.Click, btn9.Click, btn8.Click, btn7.Click,
            btn6.Click, btn5.Click, btn4.Click, btn3.Click, btn2.Click, btn1.Click, btn0.Click, btnPt.Click,
            btnZ.Click, btnPi.Click, btnE.Click, btnImagUnit.Click
            Dim btn As Button = DirectCast(sender, Button)
            'pnl.Focus()
            Dim start As Integer = FrmEditor.tb.SelectionStart
            FrmEditor.tb.Text = FrmEditor.tb.Text.Remove(FrmEditor.tb.SelectionStart, FrmEditor.tb.SelectionEnd - FrmEditor.tb.SelectionStart)
            FrmEditor.tb.Focus()
            FrmEditor.tb.Text = FrmEditor.tb.Text.Insert(start, btn.Text)
            FrmEditor.tb.SelectionStart = start + btn.Text.Length
        End Sub
        Private Sub WriteFunction(ByVal s As String)
            FrmEditor.tb.Focus()
            Dim start As Integer = FrmEditor.tb.SelectionStart
            FrmEditor.tb.Text = FrmEditor.tb.Text.Remove(FrmEditor.tb.SelectionStart, FrmEditor.tb.SelectionEnd - FrmEditor.tb.SelectionStart).Insert(start, s)
            If s.Contains("(") Then
                FrmEditor.tb.SelectionStart = start + s.IndexOf("(") + 1
            Else
                FrmEditor.tb.SelectionStart = start + s.Count()
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
            FrmEditor.Close()
        End Sub
        Private _isMoving As Boolean
        Private _movingPrevPt As Point

        Private Sub osk_MouseDown(sender As Object, e As MouseEventArgs) Handles MyBase.MouseDown, pnl.MouseDown
            If e.Button <> MouseButtons.Right Then
                _movingPrevPt = e.Location
                _isMoving = True
                'FrmEditor.BringToFront()
            End If
        End Sub

        Private Sub osk_MouseMove(sender As Object, e As MouseEventArgs) Handles MyBase.MouseMove, pnl.MouseMove
            If _isMoving Then
                Me.Left += e.X - _movingPrevPt.X
                Me.Top += e.Y - _movingPrevPt.Y
                If FrmEditor.LSnap Then
                    FrmEditor.Left += e.X - _movingPrevPt.X
                    FrmEditor.Top += e.Y - _movingPrevPt.Y
                    If FrmViewer.Snap Then
                        FrmViewer.Left += e.X - _movingPrevPt.X
                        FrmViewer.Top += e.Y - _movingPrevPt.Y
                    End If
                End If
                If KeyboardRight.Snap Then
                    KeyboardRight.Left += e.X - _movingPrevPt.X
                    KeyboardRight.Top += e.Y - _movingPrevPt.Y
                End If
            End If
        End Sub

        Private Sub osk_MouseUp(sender As Object, e As MouseEventArgs) Handles MyBase.MouseUp, pnl.MouseUp
            If e.Button = MouseButtons.Right Then
                If FrmEditor.LSnap Then
                    FrmEditor.LSnap = False
                    Me.Top += 6
                    If Snap Then
                        KeyboardRight.Top += 6
                        FrmEditor.RSnap = False
                    End If
                End If
                If Snap Then
                    Me.Snap = False
                    KeyboardRight.Snap = False
                    If Me.Left < KeyboardRight.Left Then
                        Me.Left -= 9
                        KeyboardRight.Left += 9
                    Else
                        Me.Left += 9
                        KeyboardRight.Left -= 9
                    End If
                End If
            Else
                _isMoving = False
                If Me.Right > KeyboardRight.Left AndAlso Me.Right < KeyboardRight.Right AndAlso (Me.Bottom > KeyboardRight.Top - 100 AndAlso Me.Top < KeyboardRight.Bottom + 100) Then
                    Me.Left = KeyboardRight.Left - Me.Width + 12
                    Me.Top = KeyboardRight.Top
                    Me.Snap = True
                    KeyboardRight.Snap = True
                    If FrmEditor.RSnap Then FrmEditor.LSnap = True
                ElseIf Me.Left < KeyboardRight.Right AndAlso Me.Left > KeyboardRight.Left AndAlso (Me.Bottom > KeyboardRight.Top - 100 AndAlso Me.Top < KeyboardRight.Bottom + 100) Then
                    Me.Left = KeyboardRight.Right - 12
                    Me.Top = KeyboardRight.Top
                    Me.Snap = True
                    KeyboardRight.Snap = True
                    If FrmEditor.RSnap Then FrmEditor.LSnap = True
                ElseIf Me.Right > Screen.PrimaryScreen.WorkingArea.Right AndAlso Not FrmEditor.LSnap
                    Me.Left = Screen.PrimaryScreen.WorkingArea.Right - Me.Width + 12
                ElseIf Me.Left < 0 AndAlso Not FrmEditor.LSnap
                    Me.Left = -12
                End If
                If Me.Top < FrmEditor.Bottom AndAlso Me.Top > FrmEditor.Top AndAlso
                    (Me.Left < FrmEditor.Right - 47 AndAlso Me.Right > FrmEditor.Left - 1) Then
                    Me.Left = FrmEditor.Left
                    Me.Top = FrmEditor.Bottom
                    FrmEditor.LSnap = True
                    If Me.Snap Then
                        KeyboardRight.Left = Me.Right - 12
                        KeyboardRight.Top = FrmEditor.Bottom
                        FrmEditor.RSnap = True
                    End If
                End If
            End If
            If FrmEditor.LSnap Then
                Me.Left = FrmEditor.Left
                Me.Top = FrmEditor.Bottom
                If Me.Snap Then
                    KeyboardRight.Left = Me.Right - 12
                    KeyboardRight.Top = FrmEditor.Bottom
                    FrmEditor.RSnap = True
                End If
            End If
            My.Settings.MainPos = FrmEditor.Left & "," & FrmEditor.Top
            My.Settings.LeftKbdPos = Me.Left & "," & Me.Top
            My.Settings.RightKbdPos = KeyboardRight.Left & "," & KeyboardRight.Top
            My.Settings.OskLock = Me.Snap
            My.Settings.LOskSnap = FrmEditor.LSnap
            My.Settings.ROskSnap = FrmEditor.RSnap
            My.Settings.ROskSnap = FrmEditor.RSnap
            My.Settings.Save()
        End Sub

        Private Sub osk_Enter(sender As Object, e As EventArgs) Handles btnPi.Enter, btnE.Enter, btnZ.Enter, btnY.Enter,
        btnX.Enter, btnPt.Enter, btnImagUnit.Enter, btn9.Enter, btn8.Enter, btn7.Enter, btn6.Enter, btn5.Enter,
        btn4.Enter, btn3.Enter, btn2.Enter, btn1.Enter, btn0.Enter
            If (Not DirectCast(sender, Control).Name = "pnl") Then pnl.Focus()
            FrmEditor.tb.Focus()
            'FrmEditor.BringToFront()
        End Sub

        Private Sub KeyboardLeft_Load(sender As Object, e As EventArgs) Handles Me.Load
            Me.SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.OptimizedDoubleBuffer, True)
            Me.Snap = My.Settings.OskLock
        End Sub
        Public Sub InitPosition()
            If My.Settings.LeftKbdPos <> "" AndAlso Not FrmEditor.LSnap Then
                Dim spl() As String = My.Settings.LeftKbdPos.Split(","c)
                Me.Location = New Point(CInt(spl(0)), CInt(spl(1)))
            Else
                Me.Left = FrmEditor.Left
                Me.Top = FrmEditor.Bottom
                My.Settings.LeftKbdPos = Me.Left & "," & Me.Top
                My.Settings.Save()
            End If
        End Sub
    End Class
End Namespace