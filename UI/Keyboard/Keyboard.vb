Namespace UI.Keyboards
    Public Class MainKeyboard
        Dim _minimized As Boolean = False
        Dim _init As Boolean = True
        Public Property Minimized As Boolean
            Get
                Return _minimized
            End Get
            Set(value As Boolean)
                If _init Then
                    _init = False
                    Return
                End If

                _minimized = value
                Me.Top = 400
                If _minimized Then
                    FrmEditor.Editor.Height = Me.Top + LbKbd.Top - 10
                    pnl.Hide()
                    LbKbd.Show()

                Else
                    pnl.Show()
                    LbKbd.Hide()
                    FrmEditor.Editor.Height = 400
                End If
                My.Settings.ShowKeyboard = Not _minimized
                My.Settings.Save()
            End Set
        End Property

        Private Sub GenericButtonClick(sender As Object, e As EventArgs) Handles btnFact.Click, btnMul.Click,
            btnDiv.Click, btnAdd.Click, btnMin.Click,
            btnEquals.Click, btnLessThan.Click, btnMoreThan.Click, btnAns.Click, btnExp.Click, btnPt.Click,
           btnA.Click, btnB.Click, btnC.Click,
            btnT.Click, btnM.Click, btnX.Click, btnY.Click, btnComma.Click, btnN.Click,
            btnY.Click, btnX.Click, btn9.Click, btn8.Click, btn7.Click,
            btn6.Click, btn5.Click, btn4.Click, btn3.Click, btn2.Click, btn1.Click, btn0.Click, btnPt.Click,
            btnZ.Click, btnPi.Click, btnE.Click, btnImagUnit.Click, btnTripleQuote.Click, btnComment.Click

            Dim btn As Button = DirectCast(sender, Button)
            FrmEditor.Tb.Focus()
            FrmEditor.Tb.DeleteRange(FrmEditor.Tb.SelectionStart, FrmEditor.Tb.SelectionEnd - FrmEditor.Tb.SelectionStart)
            FrmEditor.Tb.AddText(btn.Text)
        End Sub

        ''' <summary>
        ''' Write a string in function notation to the evaluator textbox
        ''' </summary>
        ''' <param name="s"></param>
        ''' <param name="sep"></param>
        Private Sub WriteFunction(ByVal s As String, Optional ByVal sep As String = "(")
            FrmEditor.Tb.Focus()
            Dim start As Integer = FrmEditor.Tb.SelectionStart
            FrmEditor.Tb.DeleteRange(start, FrmEditor.Tb.SelectionEnd - FrmEditor.Tb.SelectionStart)
            FrmEditor.Tb.InsertText(start, s)

            If s.Contains(sep) Then
                FrmEditor.Tb.SelectionStart = start + s.IndexOf(sep) + 1
            Else
                FrmEditor.Tb.SelectionStart = start + s.Count()
            End If
            FrmEditor.Tb.Focus()
        End Sub

        Private Sub btnCalc_Click(sender As Object, e As EventArgs) Handles btnCalc.Click
            FrmEditor.Tb.Focus()
            WriteFunction(vbNewLine)
        End Sub

        Private Sub btnNext_Click(sender As Object, e As EventArgs) Handles btnNext.Click
            FrmEditor.Tb.Focus()
            If FrmEditor.Tb.SelectionStart < FrmEditor.Tb.Text.Length Then
                FrmEditor.Tb.SelectionStart += 1
                FrmEditor.Tb.SelectionEnd += 1
            End If
        End Sub

        Private Sub btnPrev_Click(sender As Object, e As EventArgs) Handles btnPrev.Click
            FrmEditor.Tb.Focus()
            If (FrmEditor.Tb.SelectionStart > 0) Then
                FrmEditor.Tb.SelectionStart -= 1
                FrmEditor.Tb.SelectionEnd -= 1
            End If
        End Sub

        Private Sub btnEnd_Click(sender As Object, e As EventArgs) Handles btnEnd.Click
            FrmEditor.Tb.Focus()
            FrmEditor.Tb.SelectionEnd = FrmEditor.Tb.TextLength
            FrmEditor.Tb.SelectionStart = FrmEditor.Tb.TextLength
            FrmEditor.Tb.ScrollCaret()
        End Sub
        Private Sub btnRet_Click(sender As Object, e As EventArgs) Handles btnRet.Click
            FrmEditor.Tb.Focus()
            FrmEditor.Tb.SelectionStart = 0
            FrmEditor.Tb.SelectionEnd = 0
            FrmEditor.Tb.ScrollCaret()
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
            FrmEditor.Tb.Focus()
            If FrmEditor.Tb.Text.Length > 0 Then
                Dim start As Integer = FrmEditor.Tb.SelectionStart
                If FrmEditor.Tb.SelectionEnd - FrmEditor.Tb.SelectionStart > 0 Then
                    FrmEditor.Tb.Text = FrmEditor.Tb.Text.Substring(0, FrmEditor.Tb.SelectionStart) & FrmEditor.Tb.Text.Substring(FrmEditor.Tb.SelectionStart + FrmEditor.Tb.SelectionEnd - FrmEditor.Tb.SelectionStart)
                    FrmEditor.Tb.SelectionStart = start
                ElseIf FrmEditor.Tb.SelectionStart > 0 Then
                    FrmEditor.Tb.Text = FrmEditor.Tb.Text.Substring(0, FrmEditor.Tb.SelectionStart - 1) & FrmEditor.Tb.Text.Substring(FrmEditor.Tb.SelectionStart)
                    FrmEditor.Tb.SelectionStart = start - 1
                Else
                    FrmEditor.Tb.Text = FrmEditor.Tb.Text.Substring(1)
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
            FrmEditor.Tb.Text = ""
            FrmEditor.LbResult.Text = "= "
        End Sub

        Private Sub btnFn_Click(sender As Object, e As EventArgs) Handles btnAsin.Click, btnAcos.Click,
            btnSin.Click, btnCos.Click, btnTan.Click, btnAtan.Click, btnLn.Click, btnRand.Click,
            btnPrint.Click, btnRead.Click, btnAvg.Click, btnMedian.Click

            WriteFunction(DirectCast(sender, Button).Text & "()")
        End Sub

        Private Sub btnRt_Click(sender As Object, e As EventArgs) Handles btnQdtc.Click
            WriteFunction("quadratic(,,)")
        End Sub

        Private Sub btnGcd_Click(sender As Object, e As EventArgs) Handles btnPow10.Click
            WriteFunction(" E ")
        End Sub

        Private Sub btnMod_Click(sender As Object, e As EventArgs) Handles btnMod.Click
            WriteFunction(" mod ")
        End Sub

        Private Sub osk_Enter(sender As Object, e As EventArgs)
            pnl.Focus()
            'FrmEditor.Tb.Focus()
        End Sub

        Private Sub CompleteBrace(chr As Char)
            If chr = "(" OrElse chr = "[" OrElse chr = "{" OrElse chr = ")" OrElse chr = "]" OrElse chr = "}" Then

                Dim startPos As Integer
                Dim curLine As Integer = FrmEditor.Tb.CurrentLine
                Dim curText As String = FrmEditor.Tb.Lines(curLine).Text

                While curLine > 0 AndAlso FrmEditor.Tb.Lines(curLine - 1).Text.EndsWith(" _") ' connect _
                    curLine -= 1
                    curText = FrmEditor.Tb.Lines(curLine).Text.Remove(FrmEditor.Tb.Lines(curLine).Text.Length - 2) & curText
                End While
                startPos = FrmEditor.Tb.Lines(curLine).Position

                Dim startBr As Char = chr
                Dim endBr As Char
                Dim reverse As Boolean = False
                Dim ct As Integer = 0

                Select Case chr
                    Case "("c
                        endBr = ")"c
                    Case "["c
                        endBr = "]"c
                    Case "{"c
                        endBr = "}"c
                    Case ")"c
                        endBr = "("c
                        reverse = True
                    Case "]"c
                        endBr = "["c
                        reverse = True
                    Case "}"c
                        endBr = "{"c
                        reverse = True
                End Select

                For i As Integer = 0 To curText.Length - 1
                    If curText(i) = startBr Then ct += 1
                    If curText(i) = endBr Then ct -= 1
                Next

                If ct > 0 Then
                    If reverse Then
                        Dim len As Integer = FrmEditor.Tb.CurrentPosition - startPos
                        If curText.Length > len Then curText = curText.Remove(len)

                        Dim braceList As Char() = {"["c, "("c, "{"c}
                        Dim endBraceList As Char() = {"]"c, ")"c, "}"c}
                        Dim lvl As List(Of Integer)() = {New List(Of Integer)({0}),
                            New List(Of Integer)({0}), New List(Of Integer)({0})}
                        Dim pos As Integer = 0

                        For i As Integer = 0 To curText.Length - 2
                            For j As Integer = 0 To braceList.Count - 1
                                If braceList(j) = curText(i) Then
                                    lvl(j).Add(i + 1)
                                ElseIf endBraceList(j) = curText(i) Then
                                    If Not lvl(j).Count <= 1 Then lvl(j).RemoveAt(lvl(j).Count - 1)
                                End If
                            Next
                        Next

                        For j As Integer = 0 To lvl.Count - 1
                            pos = Math.Max(lvl(j)(lvl(j).Count - 1), pos)
                        Next

                        FrmEditor.Tb.InsertText(FrmEditor.Tb.Lines(FrmEditor.Tb.CurrentLine).Position + pos, endBr)
                    Else
                        FrmEditor.Tb.InsertText(FrmEditor.Tb.CurrentPosition, endBr)
                    End If
                End If

            ElseIf chr = "|" OrElse chr = """"c OrElse chr = "'"c OrElse chr = "`"c Then
                If chr = """"c AndAlso FrmEditor.Tb.CurrentPosition > 1 AndAlso FrmEditor.Tb.GetTextRange(FrmEditor.Tb.CurrentPosition - 2, 2) = """""" OrElse chr = "'"c AndAlso FrmEditor.Tb.CurrentPosition > 1 AndAlso FrmEditor.Tb.GetTextRange(FrmEditor.Tb.CurrentPosition - 2, 2) = "'" & "'" Then

                    ' if there were already two quotes before, do not add another: user probably wanted to type triple quotes
                    FrmEditor.Tb.SelectionStart += 1
                    Return
                End If

                Dim curText As String = FrmEditor.Tb.Lines(FrmEditor.Tb.CurrentLine).Text
                Dim ct As Boolean = False

                For i As Integer = 0 To curText.Length - 1
                    If curText(i) = chr Then ct = Not ct
                Next

                If ct Then FrmEditor.Tb.InsertText(FrmEditor.Tb.CurrentPosition, chr)

            End If
        End Sub

        Private Sub btnBrLft_MouseUp(sender As Object, e As MouseEventArgs) Handles btnBrLft.MouseUp,
            btnSqBrL.MouseUp, btnCBL.MouseUp, btnCBR.MouseUp, btnBrRt.MouseUp, btnSqBrR.MouseUp, btnAbs.MouseUp, btnQuote.MouseUp,
            btnTick.MouseUp

            FrmEditor.Tb.DeleteRange(FrmEditor.Tb.SelectionStart, FrmEditor.Tb.SelectionEnd - FrmEditor.Tb.SelectionStart)
            Dim startSign As Char = DirectCast(sender, Button).Text(0)
            FrmEditor.Tb.InsertText(FrmEditor.Tb.CurrentPosition, startSign)
            FrmEditor.Tb.SelectionStart += 1
            CompleteBrace(startSign)
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

        Private Sub btnUnaryRight_Click(sender As Object, e As EventArgs) Handles btnIf.Click, btnWhile.Click, btnNot.Click
            WriteFunction(DirectCast(sender, Button).Text & " ")
        End Sub

        Private Sub btnBinary_Click(sender As Object, e As EventArgs) Handles btnOr.Click, btnAnd.Click
            WriteFunction(" " & DirectCast(sender, Button).Text & " ")
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
            Me.SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.OptimizedDoubleBuffer, True)

            For Each c As Control In Me.pnl.Controls
                AddHandler c.Enter, AddressOf osk_Enter
            Next
        End Sub

        Private Sub pnl_MouseDown(sender As Object, e As MouseEventArgs) Handles pnl.MouseDown, Me.MouseDown, LbKbd.MouseDown
            FrmEditor.FrmEditor_MouseDown(FrmEditor, e)
        End Sub
        Private Sub pnl_MouseMove(sender As Object, e As MouseEventArgs) Handles pnl.MouseMove, Me.MouseMove, LbKbd.MouseMove
            FrmEditor.FrmEditor_MouseMove(FrmEditor, e)
        End Sub
        Private Sub pnl_MouseUp(sender As Object, e As MouseEventArgs) Handles pnl.MouseUp, Me.MouseUp, LbKbd.MouseUp
            FrmEditor.FrmEditor_MouseUp(FrmEditor, e)
        End Sub

        Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
            Me.Minimized = True
        End Sub

        Private Sub LbKbd_Click(sender As Object, e As EventArgs) Handles LbKbd.Click, Me.Click
            Me.Minimized = False
        End Sub
    End Class
End Namespace