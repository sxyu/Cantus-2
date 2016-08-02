Namespace UI.Keyboards
    Public Class KeyboardRight
        Protected Overloads Overrides ReadOnly Property CreateParams() As CreateParams
            Get
                Dim cp As CreateParams = MyBase.CreateParams
                ' turn on WS_EX_TOOLWINDOW style bit
                cp.ExStyle = cp.ExStyle Or &H80
                Return cp
            End Get
        End Property

        Public Snap As Boolean = False

        Private Sub GenericButtonClick(sender As Object, e As EventArgs) Handles btnFact.Click, btnMul.Click,
            btnDiv.Click, btnAdd.Click, btnMin.Click,
            btnEquals.Click, btnLessThan.Click, btnMoreThan.Click, btnAns.Click, btnExp.Click, btnPt.Click,
           btnA.Click, btnB.Click, btnC.Click,
            btnT.Click, btnM.Click, btnX.Click, btnY.Click, btnComma.Click, btnN.Click, btnTick.Click

            Dim btn As Button = DirectCast(sender, Button)
            FrmEditor.tb.Focus()
            FrmEditor.tb.DeleteRange(FrmEditor.tb.SelectionStart, FrmEditor.tb.SelectionEnd - FrmEditor.tb.SelectionStart)
            FrmEditor.tb.AddText(btn.Text)
        End Sub

        ''' <summary>
        ''' Write a string in function notation to the evaluator textbox
        ''' </summary>
        ''' <param name="s"></param>
        ''' <param name="sep"></param>
        Private Sub WriteFunction(ByVal s As String, Optional ByVal sep As String = "(")
            FrmEditor.tb.Focus()
            Dim start As Integer = FrmEditor.tb.SelectionStart
            FrmEditor.tb.DeleteRange(start, FrmEditor.tb.SelectionEnd - FrmEditor.tb.SelectionStart)
            FrmEditor.tb.InsertText(start, s)

            If s.Contains(sep) Then
                FrmEditor.tb.SelectionStart = start + s.IndexOf(sep) + 1
            Else
                FrmEditor.tb.SelectionStart = start + s.Count()
            End If
            FrmEditor.tb.Focus()
        End Sub

        Private Sub btnCalc_Click(sender As Object, e As EventArgs) Handles btnCalc.Click
            FrmEditor.tb.Focus()
            WriteFunction(vbNewLine)
        End Sub

        Private Sub btnNext_Click(sender As Object, e As EventArgs) Handles btnNext.Click
            FrmEditor.tb.Focus()
            If FrmEditor.tb.SelectionStart < FrmEditor.tb.Text.Length Then
                FrmEditor.tb.SelectionStart += 1
            End If
        End Sub

        Private Sub btnPrev_Click(sender As Object, e As EventArgs) Handles btnPrev.Click
            FrmEditor.tb.Focus()
            If (FrmEditor.tb.SelectionStart > 0) Then
                FrmEditor.tb.SelectionStart -= 1
            End If
        End Sub

        Private Sub btnEnd_Click(sender As Object, e As EventArgs) Handles btnEnd.Click
            FrmEditor.tb.Focus()
            FrmEditor.tb.SelectionEnd = 0
            FrmEditor.tb.SelectionStart = FrmEditor.tb.Text.Length
            FrmEditor.VerticalScroll.Value = FrmEditor.VerticalScroll.Maximum
        End Sub
        Private Sub btnRet_Click(sender As Object, e As EventArgs) Handles btnRet.Click
            FrmEditor.tb.Focus()
            FrmEditor.tb.SelectionStart = 0
            FrmEditor.tb.SelectionEnd = 0
            FrmEditor.VerticalScroll.Value = 0
        End Sub

        Private Sub btnSqrt_MouseUp(sender As Object, e As MouseEventArgs) Handles btnSqrt.MouseUp
            If e.Button = MouseButtons.Left Then
                WriteFunction("sqrt()")
            ElseIf e.Button = MouseButtons.Middle Then
                WriteFunction("cbrt()")
            Else
                WriteFunction("root(,)")
            End If
        End Sub

        Private Sub btnDel_Click(sender As Object, e As EventArgs) Handles btnDel.Click
            FrmEditor.tb.Focus()
            If FrmEditor.tb.Text.Length > 0 Then
                Dim start As Integer = FrmEditor.tb.SelectionStart
                If FrmEditor.tb.SelectionEnd - FrmEditor.tb.SelectionStart > 0 Then
                    FrmEditor.tb.Text = FrmEditor.tb.Text.Substring(0, FrmEditor.tb.SelectionStart) &
                        FrmEditor.tb.Text.Substring(FrmEditor.tb.SelectionStart + FrmEditor.tb.SelectionEnd - FrmEditor.tb.SelectionStart)
                    FrmEditor.tb.SelectionStart = start
                ElseIf FrmEditor.tb.SelectionStart > 0 Then
                    FrmEditor.tb.Text = FrmEditor.tb.Text.Substring(0, FrmEditor.tb.SelectionStart - 1) &
                        FrmEditor.tb.Text.Substring(FrmEditor.tb.SelectionStart)
                    FrmEditor.tb.SelectionStart = start - 1
                Else
                    FrmEditor.tb.Text = FrmEditor.tb.Text.Substring(1)
                    'FrmEditor.tb.SelectionStart = start
                End If
            End If
        End Sub

        Private Sub btnPans_Click(sender As Object, e As EventArgs) Handles btnFunctions.Click
            Using diag As New Dialogs.DiagFunctions
                If diag.ShowDialog() = DialogResult.OK Then
                    WriteFunction(diag.Result)
                End If
            End Using
        End Sub

        Private Sub btnAC_Click(sender As Object, e As EventArgs) Handles btnAC.Click
            pnl.Focus()
            'FrmEditor.BringToFront()
            FrmEditor.tb.Text = ""
            FrmEditor.lbResult.Text = "= "
        End Sub

        Private Sub btnFn_Click(sender As Object, e As EventArgs) Handles btnAsin.Click, btnAcos.Click,
            btnSin.Click, btnCos.Click, btnTan.Click, btnAtan.Click, btnLn.Click, btnRand.Click,
            btnAlert.Click, btnInput.Click, btnAvg.Click, btnMedian.Click

            WriteFunction(DirectCast(sender, Button).Text & "()")
        End Sub

        Private Sub btnRt_Click(sender As Object, e As EventArgs) Handles btnQdtc.Click
            WriteFunction("quadratic(,,)")
        End Sub

        Private Sub btnGcd_Click(sender As Object, e As EventArgs) Handles btnPow10.Click
            WriteFunction(" E ")
        End Sub

        Private Sub OskRight_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
            FrmEditor.Close()
        End Sub
        Private _isMoving As Boolean
        Private _movingPrevPt As Point

        Private Sub osk_MouseDown(sender As Object, e As MouseEventArgs) Handles MyBase.MouseDown, pnl.MouseDown, lbResult.MouseDown
            If e.Button <> MouseButtons.Right Then
                _movingPrevPt = e.Location
                _isMoving = True
            End If
        End Sub

        Private Sub osk_MouseMove(sender As Object, e As MouseEventArgs) Handles MyBase.MouseMove, pnl.MouseMove, lbResult.MouseMove
            If _isMoving Then
                Me.Left += e.X - _movingPrevPt.X
                Me.Top += e.Y - _movingPrevPt.Y
                If FrmEditor.RSnap Then
                    FrmEditor.Left += e.X - _movingPrevPt.X
                    FrmEditor.Top += e.Y - _movingPrevPt.Y
                    If FrmViewer.Snap Then
                        FrmViewer.Left += e.X - _movingPrevPt.X
                        FrmViewer.Top += e.Y - _movingPrevPt.Y
                    End If
                End If
                If KeyboardLeft.Snap Then
                    KeyboardLeft.Left += e.X - _movingPrevPt.X
                    KeyboardLeft.Top += e.Y - _movingPrevPt.Y
                End If
            End If
        End Sub

        Private Sub osk_MouseUp(sender As Object, e As MouseEventArgs) Handles MyBase.MouseUp, pnl.MouseUp, lbResult.MouseUp
            If e.Button = MouseButtons.Right Then
                If FrmEditor.RSnap Then
                    FrmEditor.RSnap = False
                    Me.Top += 6
                    If Snap Then
                        FrmEditor.LSnap = False
                        KeyboardLeft.Top += 6
                    End If
                End If
                If Snap Then
                    Me.Snap = False
                    KeyboardLeft.Snap = False
                    If Me.Left < KeyboardLeft.Left Then
                        Me.Left -= 9
                        KeyboardLeft.Left += 9
                    Else
                        Me.Left += 9
                        KeyboardLeft.Left -= 9
                    End If
                End If
            Else
                _isMoving = False
                If Me.Left < KeyboardLeft.Right AndAlso Me.Left > KeyboardLeft.Left AndAlso
                    Me.Bottom > KeyboardLeft.Top - 40 AndAlso Me.Top < KeyboardLeft.Bottom + 100 Then
                    Me.Left = KeyboardLeft.Right - 12
                    Me.Top = KeyboardLeft.Top
                    Me.Snap = True
                    KeyboardLeft.Snap = True
                    If FrmEditor.LSnap Then FrmEditor.RSnap = True
                ElseIf Me.Right > KeyboardLeft.Left AndAlso Me.Right < KeyboardLeft.Right AndAlso
                    (Me.Bottom < KeyboardLeft.Top + 100 AndAlso Me.Top > KeyboardLeft.Bottom - 100) Then
                    Me.Left = KeyboardLeft.Left - Me.Width + 12
                    Me.Top = KeyboardLeft.Top
                    Me.Snap = True
                    KeyboardLeft.Snap = True
                    If FrmEditor.LSnap Then FrmEditor.RSnap = True
                ElseIf Me.Right > Screen.PrimaryScreen.WorkingArea.Right AndAlso Not FrmEditor.RSnap
                    Me.Left = Screen.PrimaryScreen.WorkingArea.Right - Me.Width + 12
                ElseIf Me.Left < 0 AndAlso Not FrmEditor.RSnap
                    Me.Left = -12
                End If
                If Me.Top <= FrmEditor.Bottom AndAlso Me.Top > FrmEditor.Top AndAlso
                    Me.Left < FrmEditor.Right - 47 AndAlso Me.Right > FrmEditor.Left - 1 Then
                    Me.Left = FrmEditor.Right - Me.Width
                    Me.Top = FrmEditor.Bottom
                    FrmEditor.RSnap = True
                End If
            End If
            If FrmEditor.RSnap Then
                Me.Left = FrmEditor.Right - Me.Width
                Me.Top = FrmEditor.Bottom
                If Me.Snap Then
                    KeyboardLeft.Left = FrmEditor.Left
                    KeyboardLeft.Top = FrmEditor.Bottom
                    FrmEditor.LSnap = True
                End If
            End If
            My.Settings.MainPos = FrmEditor.Left & "," & FrmEditor.Top
            My.Settings.RightKbdPos = Me.Left & "," & Me.Top
            My.Settings.LeftKbdPos = KeyboardLeft.Left & "," & KeyboardLeft.Top
            My.Settings.OskLock = Me.Snap
            My.Settings.LOskSnap = FrmEditor.LSnap
            My.Settings.ROskSnap = FrmEditor.RSnap
            My.Settings.Save()
        End Sub

        Private Sub btnMod_Click(sender As Object, e As EventArgs) Handles btnMod.Click
            WriteFunction(" mod ")
        End Sub

        Private Sub osk_Enter(sender As Object, e As EventArgs)
            pnl.Focus()
            FrmEditor.tb.Focus()
        End Sub

        Private Sub btnBrLft_MouseUp(sender As Object, e As MouseEventArgs) Handles btnBrLft.MouseUp, btnSqBrL.MouseUp, btnCBL.MouseUp
            Dim startSign As Char = DirectCast(sender, Button).Text(0)
            Dim endSign As Char = ")"c
            If startSign = "["c Then
                endSign = "]"c
            ElseIf startSign = "{"c
                endSign = "}"c
            End If

            Dim start As Integer = FrmEditor.tb.SelectionStart
            Dim ct As Integer = 0

            For Each c As Char In FrmEditor.tb.Text
                If c = startSign Then
                    ct += 1
                ElseIf c = endSign
                    ct -= 1
                End If
            Next

            FrmEditor.tb.DeleteRange(start, FrmEditor.tb.SelectionEnd - FrmEditor.tb.SelectionStart)
            FrmEditor.tb.AddText(startSign)
            If ct >= 0 Then
                FrmEditor.tb.AddText(endSign)
                FrmEditor.tb.SelectionStart -= 1
                FrmEditor.tb.SelectionEnd -= 1
            End If
        End Sub

        Private Sub btnBrRt_MouseUp(sender As Object, e As MouseEventArgs) Handles btnBrRt.MouseUp
            FrmEditor.tb.Focus()
            Dim openBr As Char = "("c
            Dim closeBr As Char = ")"c

            If e.Button = MouseButtons.Right Then
                openBr = "["c
                closeBr = "]"c
            ElseIf e.Button = MouseButtons.Middle
                openBr = "{"c
                closeBr = "}"c
            End If

            Dim start As Integer = FrmEditor.tb.SelectionStart
            FrmEditor.tb.Text = FrmEditor.tb.Text.Remove(FrmEditor.tb.SelectionStart, FrmEditor.tb.SelectionEnd - FrmEditor.tb.SelectionStart)

            Dim insertStart As Boolean = True
            For i As Integer = Math.Min(start, FrmEditor.tb.Text.Length - 1) To 0 Step -1
                If FrmEditor.tb.Text(i) = closeBr Then
                    Exit For
                ElseIf FrmEditor.tb.Text(i) = openBr
                    insertStart = False
                    Exit For
                End If
            Next

            If insertStart Then
                For i As Integer = start - 1 To -1 Step -1
                    If i = -1 OrElse
                        FrmEditor.tb.Text.Length > i AndAlso (FrmEditor.tb.Text(i) = ControlChars.Lf OrElse
                                                                     FrmEditor.tb.Text(i) = ControlChars.Cr) Then
                        FrmEditor.tb.Text = FrmEditor.tb.Text.Insert(i + 1, openBr)
                        Exit For
                    End If
                Next
                FrmEditor.tb.Text = FrmEditor.tb.Text.Insert(start + 1, closeBr)
            Else
                FrmEditor.tb.Text = FrmEditor.tb.Text.Insert(start, closeBr)
            End If

            FrmEditor.tb.SelectionStart = start + 2
            FrmEditor.tb.SelectionEnd = start + 2
        End Sub

        Private Sub btnln_MouseUp(sender As Object, e As MouseEventArgs) Handles btnlog.MouseUp
            If e.Button = MouseButtons.Left Then
                WriteFunction("log(,)")
            ElseIf e.Button = MouseButtons.Middle
                WriteFunction("lg()")
            Else
                WriteFunction("ln()")
            End If
        End Sub

        Private Sub btnAbs_Click(sender As Object, e As EventArgs) Handles btnAbs.Click
            WriteFunction("||", "|")
        End Sub

        Private Sub btnExp_MouseUp(sender As Object, e As MouseEventArgs) Handles btnExp.MouseUp
            If e.Button = MouseButtons.Right Then
                WriteFunction(" E ")
            ElseIf e.Button = MouseButtons.Middle
                WriteFunction("^2")
            End If
        End Sub

        Private Sub btnIntegral_Click(sender As Object, e As EventArgs) Handles btnIntegral.Click
            WriteFunction("integral(,,)")
        End Sub

        Private Sub btnDyDx_Click(sender As Object, e As EventArgs) Handles btnDyDx.Click
            WriteFunction("dydx(,)")
        End Sub

        Private Sub btnIf_Click(sender As Object, e As EventArgs) Handles btnIf.Click
            WriteFunction("if ")
        End Sub

        Private Sub btnWhile_Click(sender As Object, e As EventArgs) Handles btnWhile.Click
            WriteFunction("while ")
        End Sub

        Private Sub btnSigma_Click(sender As Object, e As EventArgs) Handles btnSigma.Click
            WriteFunction("sigma(,,)")
        End Sub

        Private Sub btnPerm_Click(sender As Object, e As EventArgs) Handles btnPerm.Click
            WriteFunction("perm(,)")
        End Sub

        Private Sub btnChoose_Click(sender As Object, e As EventArgs) Handles btnChoose.Click
            WriteFunction(" choose ")
        End Sub

        Private Sub btnConfirm_Click(sender As Object, e As EventArgs) Handles btnConfirm.Click
            WriteFunction("confirm()")
        End Sub

        Private Sub btnYesNo_Click(sender As Object, e As EventArgs) Handles btnYesNo.Click
            WriteFunction("yesno()")
        End Sub

        Private Sub KeyboardRight_Load(sender As Object, e As EventArgs) Handles Me.Load
            Me.Hide()
            Me.Snap = My.Settings.OskLock
            Me.SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.OptimizedDoubleBuffer, True)

            For Each c As Control In Me.pnl.Controls
                AddHandler c.Enter, AddressOf osk_Enter
            Next
        End Sub
        Public Sub InitPosition()
            If My.Settings.RightKbdPos <> "" AndAlso Not FrmEditor.RSnap Then
                Dim spl() As String = My.Settings.RightKbdPos.Split(","c)
                Me.Left = CInt(spl(0))
                Me.Top = CInt(spl(1))
            Else
                Me.Left = FrmEditor.Right - Me.Width
                Me.Top = FrmEditor.Bottom
                My.Settings.RightKbdPos = Me.Left & "," & Me.Top
                My.Settings.Save()
            End If
        End Sub
    End Class
End Namespace