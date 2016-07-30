Imports Cantus.Calculator.Evaluator
Namespace Calculator.Keyboards
    Public Class OskRight
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
            Me.Show()
            TmrLoad.Stop()
            Me.Snap = My.Settings.OskLock
            Me.SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.OptimizedDoubleBuffer, True)

            For Each c As Control In Me.pnl.Controls
                AddHandler c.Enter, AddressOf osk_Enter
            Next

            If (My.Settings.RightKbdPos <> "") Then
                Dim spl() As String = My.Settings.RightKbdPos.Split(","c)
                Me.Left = CInt(spl(0))
                Me.Top = CInt(spl(1))
            Else
                Me.Left = FrmCalc.Right - Me.Width
                Me.Top = FrmCalc.Bottom
                My.Settings.RightKbdPos = Me.Left & "," & Me.Top
                My.Settings.Save()
            End If
        End Sub

        Private Sub GenericButtonClick(sender As Object, e As EventArgs) Handles btnFact.Click, btnMul.Click, btnDiv.Click, btnAdd.Click, btnMin.Click,
            btnEquals.Click, btnLessThan.Click, btnMoreThan.Click, btnAns.Click, btnExp.Click, btnPt.Click,
            btnCBL.Click, btnCBR.Click, btnSqBrL.Click, btnSqBrR.Click, btnA.Click, btnB.Click, btnC.Click,
            btnT.Click, btnM.Click, btnX.Click, btnY.Click, btnComma.Click, btnN.Click, btnTick.Click

            Dim btn As Button = DirectCast(sender, Button)
            FrmCalc.tb.Focus()
            Dim start As Integer = FrmCalc.tb.SelectionStart
            FrmCalc.tb.Text = FrmCalc.tb.Text.Remove(FrmCalc.tb.SelectionStart, FrmCalc.tb.SelectionEnd - FrmCalc.tb.SelectionStart)
            FrmCalc.tb.Text = FrmCalc.tb.Text.Insert(start, btn.Text)
            FrmCalc.tb.SelectionStart = start + btn.Text.Length
        End Sub

        ''' <summary>
        ''' Write a string in function notation to the evaluator textbox
        ''' </summary>
        ''' <param name="s"></param>
        ''' <param name="sep"></param>
        Private Sub WriteFunction(ByVal s As String, Optional ByVal sep As String = "(")
            FrmCalc.tb.Focus()
            Dim start As Integer = FrmCalc.tb.SelectionStart
            FrmCalc.tb.Text = FrmCalc.tb.Text.Remove(FrmCalc.tb.SelectionStart, FrmCalc.tb.SelectionEnd - FrmCalc.tb.SelectionStart).Insert(start, s)

            If s.Contains(sep) Then
                FrmCalc.tb.SelectionStart = start + s.IndexOf(sep) + 1
            Else
                FrmCalc.tb.SelectionStart = start + s.Count()
            End If
            FrmCalc.tb.Focus()
        End Sub

        Private Sub btnCalc_Click(sender As Object, e As EventArgs) Handles btnCalc.Click
            FrmCalc.tb.Focus()
            WriteFunction(vbNewLine)
        End Sub

        Private Sub btnNext_Click(sender As Object, e As EventArgs) Handles btnNext.Click
            FrmCalc.tb.Focus()
            If FrmCalc.tb.SelectionStart < FrmCalc.tb.Text.Length Then
                FrmCalc.tb.SelectionStart += 1
            End If
        End Sub

        Private Sub btnPrev_Click(sender As Object, e As EventArgs) Handles btnPrev.Click
            FrmCalc.tb.Focus()
            If (FrmCalc.tb.SelectionStart > 0) Then
                FrmCalc.tb.SelectionStart -= 1
            End If
        End Sub

        Private Sub btnEnd_Click(sender As Object, e As EventArgs) Handles btnEnd.Click
            FrmCalc.tb.Focus()
            FrmCalc.tb.SelectionEnd = 0
            FrmCalc.tb.SelectionStart = FrmCalc.tb.Text.Length
            FrmCalc.VerticalScroll.Value = FrmCalc.VerticalScroll.Maximum
        End Sub
        Private Sub btnRet_Click(sender As Object, e As EventArgs) Handles btnRet.Click
            FrmCalc.tb.Focus()
            FrmCalc.tb.SelectionStart = 0
            FrmCalc.tb.SelectionEnd = 0
            FrmCalc.VerticalScroll.Value = 0
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
            FrmCalc.tb.Focus()
            If FrmCalc.tb.Text.Length > 0 Then
                Dim start As Integer = FrmCalc.tb.SelectionStart
                If FrmCalc.tb.SelectionEnd - FrmCalc.tb.SelectionStart > 0 Then
                    FrmCalc.tb.Text = FrmCalc.tb.Text.Substring(0, FrmCalc.tb.SelectionStart) &
                        FrmCalc.tb.Text.Substring(FrmCalc.tb.SelectionStart + FrmCalc.tb.SelectionEnd - FrmCalc.tb.SelectionStart)
                    FrmCalc.tb.SelectionStart = start
                ElseIf FrmCalc.tb.SelectionStart > 0 Then
                    FrmCalc.tb.Text = FrmCalc.tb.Text.Substring(0, FrmCalc.tb.SelectionStart - 1) &
                        FrmCalc.tb.Text.Substring(FrmCalc.tb.SelectionStart)
                    FrmCalc.tb.SelectionStart = start - 1
                Else
                    FrmCalc.tb.Text = FrmCalc.tb.Text.Substring(1)
                    'frmCalc.tb.SelectionStart = start
                End If
            End If
        End Sub

        Private Sub btnPans_Click(sender As Object, e As EventArgs) Handles btnFunctions.Click
            Using diag As New DiagFunctions
                If diag.ShowDialog() = DialogResult.OK Then
                    WriteFunction(diag.Result)
                End If
            End Using
        End Sub

        Private Sub btnAC_Click(sender As Object, e As EventArgs) Handles btnAC.Click
            pnl.Focus()
            'frmCalc.BringToFront()
            FrmCalc.tb.Text = ""
            FrmCalc.lbResult.Text = "= "
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
            FrmCalc.Close()
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
                If FrmCalc.RSnap Then
                    FrmCalc.Left += e.X - _movingPrevPt.X
                    FrmCalc.Top += e.Y - _movingPrevPt.Y
                    If FrmCalc.LSnap Then
                        OskLeft.Left += e.X - _movingPrevPt.X
                        OskLeft.Top += e.Y - _movingPrevPt.Y
                    End If
                ElseIf OskLeft.Snap Then
                    OskLeft.Left += e.X - _movingPrevPt.X
                    OskLeft.Top += e.Y - _movingPrevPt.Y
                End If
            End If
        End Sub

        Private Sub osk_MouseUp(sender As Object, e As MouseEventArgs) Handles MyBase.MouseUp, pnl.MouseUp, lbResult.MouseUp
            If e.Button = MouseButtons.Right Then
                If FrmCalc.RSnap Then
                    FrmCalc.RSnap = False
                    Me.Top += 6
                    If Snap Then
                        FrmCalc.LSnap = False
                        OskLeft.Top += 6
                    End If
                End If
                If Snap Then
                    Me.Snap = False
                    OskLeft.Snap = False
                    If Me.Left < OskLeft.Left Then
                        Me.Left -= 9
                        OskLeft.Left += 9
                    Else
                        Me.Left += 9
                        OskLeft.Left -= 9
                    End If
                End If
            Else
                _isMoving = False
                If Me.Left < OskLeft.Right AndAlso Me.Left > OskLeft.Left AndAlso Me.Bottom > OskLeft.Top - 40 AndAlso Me.Top < OskLeft.Bottom + 100 Then
                    Me.Left = OskLeft.Right - 12
                    Me.Top = OskLeft.Top
                    Me.Snap = True
                    OskLeft.Snap = True
                    If FrmCalc.LSnap Then FrmCalc.RSnap = True
                ElseIf Me.Right > OskLeft.Left AndAlso Me.Right < OskLeft.Right AndAlso (Me.Bottom < OskLeft.Top + 100 AndAlso Me.Top > OskLeft.Bottom - 100) Then
                    Me.Left = OskLeft.Left - Me.Width + 12
                    Me.Top = OskLeft.Top
                    Me.Snap = True
                    OskLeft.Snap = True
                    If FrmCalc.LSnap Then FrmCalc.RSnap = True
                ElseIf Me.Right > Screen.PrimaryScreen.WorkingArea.Right
                    Me.Left = Screen.PrimaryScreen.WorkingArea.Right - Me.Width + 12
                ElseIf Me.Left < 0
                    Me.Left = -12
                End If
                If Me.Top < FrmCalc.Bottom AndAlso Me.Top > FrmCalc.Top AndAlso Me.Left < FrmCalc.Right - 47 AndAlso Me.Right > FrmCalc.Left - 1 Then
                    Me.Left = FrmCalc.Right - Me.Width
                    Me.Top = FrmCalc.Bottom
                    FrmCalc.RSnap = True
                    If Me.Snap Then
                        OskLeft.Left = FrmCalc.Left
                        OskLeft.Top = FrmCalc.Bottom
                        FrmCalc.LSnap = True
                    End If
                End If
            End If
            My.Settings.MainPos = FrmCalc.Left & "," & FrmCalc.Top
            My.Settings.RightKbdPos = Me.Left & "," & Me.Top
            My.Settings.LeftKbdPos = OskLeft.Left & "," & OskLeft.Top
            My.Settings.OskLock = Me.Snap
            My.Settings.LOskSnap = FrmCalc.LSnap
            My.Settings.ROskSnap = FrmCalc.RSnap
            My.Settings.Save()
        End Sub

        Private Sub btnMod_Click(sender As Object, e As EventArgs) Handles btnMod.Click
            WriteFunction(" mod ")
        End Sub

        Private Sub osk_Enter(sender As Object, e As EventArgs)
            pnl.Focus()
            FrmCalc.tb.Focus()
            'frmCalc.BringToFront()
        End Sub

        Private Sub btnBrLft_MouseUp(sender As Object, e As MouseEventArgs) Handles btnBrLft.MouseUp
            Dim startSign As Char = "("c
            Dim endSign As Char = ")"c
            If e.Button = MouseButtons.Right Then
                startSign = "["c
                endSign = "]"c
            ElseIf e.Button = MouseButtons.Middle Then
                startSign = "{"c
                endSign = "}"c
            End If

            Dim start As Integer = FrmCalc.tb.SelectionStart
            Dim ct As Integer = 0

            For Each c As Char In FrmCalc.tb.Text
                If c = startSign Then
                    ct += 1
                ElseIf c = endSign
                    ct -= 1
                End If
            Next

            FrmCalc.tb.Text = FrmCalc.tb.Text.Remove(FrmCalc.tb.SelectionStart, FrmCalc.tb.SelectionEnd - FrmCalc.tb.SelectionStart).Insert(start, startSign)
            If ct >= 0 Then
                FrmCalc.tb.Text = FrmCalc.tb.Text.Insert(start + 1, endSign)
            End If
            FrmCalc.tb.SelectionStart = start + 1
        End Sub

        Private Sub btnBrRt_MouseUp(sender As Object, e As MouseEventArgs) Handles btnBrRt.MouseUp
            FrmCalc.tb.Focus()
            Dim openBr As Char = "("c
            Dim closeBr As Char = ")"c

            If e.Button = MouseButtons.Right Then
                openBr = "["c
                closeBr = "]"c
            ElseIf e.Button = MouseButtons.Middle
                openBr = "{"c
                closeBr = "}"c
            End If

            Dim start As Integer = FrmCalc.tb.SelectionStart
            FrmCalc.tb.Text = FrmCalc.tb.Text.Remove(FrmCalc.tb.SelectionStart, FrmCalc.tb.SelectionEnd - FrmCalc.tb.SelectionStart)

            Dim insertStart As Boolean = True
            For i As Integer = Math.Min(start, FrmCalc.tb.Text.Length - 1) To 0 Step -1
                If FrmCalc.tb.Text(i) = closeBr Then
                    Exit For
                ElseIf FrmCalc.tb.Text(i) = openBr
                    insertStart = False
                    Exit For
                End If
            Next

            If insertStart Then
                For i As Integer = start - 1 To -1 Step -1
                    If i = -1 OrElse
                        FrmCalc.tb.Text.Length > i AndAlso (FrmCalc.tb.Text(i) = ControlChars.Lf OrElse
                                                                     FrmCalc.tb.Text(i) = ControlChars.Cr) Then
                        FrmCalc.tb.Text = FrmCalc.tb.Text.Insert(i + 1, openBr)
                        Exit For
                    End If
                Next
                FrmCalc.tb.Text = FrmCalc.tb.Text.Insert(start + 1, closeBr)
            Else
                FrmCalc.tb.Text = FrmCalc.tb.Text.Insert(start, closeBr)
            End If

            FrmCalc.tb.SelectionStart = start + 2
            FrmCalc.tb.SelectionEnd = start + 2
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
    End Class
End Namespace