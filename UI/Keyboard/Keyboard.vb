Namespace UI.Keyboards
    Public Class Keyboard
        ''' <summary>
        ''' Raised when the user has requested to close the keyboard
        ''' </summary>
        Public Event CloseKeyboard As EventHandler(Of EventArgs)
        Dim _minimized As Boolean = False
        Dim _init As Boolean = True

        Private Sub GenericButtonClick(sender As Object, e As EventArgs) Handles BtnFact.Click, BtnMul.Click,
            BtnDiv.Click, BtnAdd.Click, BtnMinus.Click,
            BtnEquals.Click, BtnLessThan.Click, BtnMoreThan.Click, BtnAns.Click, BtnExp.Click, BtnPt.Click,
           BtnA.Click, BtnB.Click, BtnC.Click,
            BtnT.Click, BtnM.Click, BtnX.Click, BtnY.Click, BtnComma.Click, BtnN.Click,
            Btn9.Click, Btn8.Click, Btn7.Click,
            Btn6.Click, Btn5.Click, Btn4.Click, Btn3.Click, Btn2.Click, Btn1.Click, Btn0.Click, BtnPt.Click,
            BtnZ.Click, BtnPi.Click, BtnE.Click, BtnImagUnit.Click, BtnTripleQuote.Click, BtnComment.Click

            Dim Btn As Button = DirectCast(sender, Button)
            FrmEditor.Tb.Focus()
            FrmEditor.Tb.DeleteRange(FrmEditor.Tb.SelectionStart, FrmEditor.Tb.SelectionEnd - FrmEditor.Tb.SelectionStart)
            FrmEditor.Tb.AddText(Btn.Text)
        End Sub

        '' <summary>
        '' Write a string in function notation to the evaluator texTbox
        '' </summary>
        '' <param name="s"></param>
        '' <param name="sep"></param>
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

        Private Sub BtnCalc_Click(sender As Object, e As EventArgs) Handles BtnCalc.Click
            FrmEditor.Tb.Focus()
            WriteFunction(vbNewLine)
        End Sub

        Private Sub BtnNext_Click(sender As Object, e As EventArgs) Handles BtnNext.Click
            FrmEditor.Tb.Focus()
            If FrmEditor.Tb.SelectionStart < FrmEditor.Tb.Text.Length Then
                FrmEditor.Tb.SelectionStart += 1
                FrmEditor.Tb.SelectionEnd += 1
            End If
        End Sub

        Private Sub BtnPrev_Click(sender As Object, e As EventArgs) Handles BtnPrev.Click
            FrmEditor.Tb.Focus()
            If (FrmEditor.Tb.SelectionStart > 0) Then
                FrmEditor.Tb.SelectionStart -= 1
                FrmEditor.Tb.SelectionEnd -= 1
            End If
        End Sub

        Private Sub BtnEnd_Click(sender As Object, e As EventArgs) Handles BtnEnd.Click
            FrmEditor.Tb.Focus()
            FrmEditor.Tb.SelectionEnd = FrmEditor.Tb.TextLength
            FrmEditor.Tb.SelectionStart = FrmEditor.Tb.TextLength
            FrmEditor.Tb.ScrollCaret()
        End Sub
        Private Sub BtnRet_Click(sender As Object, e As EventArgs) Handles BtnRet.Click
            FrmEditor.Tb.Focus()
            FrmEditor.Tb.SelectionStart = 0
            FrmEditor.Tb.SelectionEnd = 0
            FrmEditor.Tb.ScrollCaret()
        End Sub

        Private Sub BtnSqrt_MouseUp(sender As Object, e As MouseEventArgs) Handles BtnSqrt.MouseUp
            If e.Button = MouseButtons.Left Then
                WriteFunction("sqrt()")
            ElseIf e.Button = MouseButtons.Middle Then
                WriteFunction("cbrt()")
            Else
                WriteFunction("root(,)")
            End If
        End Sub

        Private Sub BtnDel_Click(sender As Object, e As EventArgs) Handles BtnDel.Click
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
                    'FrmEditor.Tb.SelectionStart = start
                End If
            End If
        End Sub

        Private Sub BtnPans_Click(sender As Object, e As EventArgs) Handles BtnFunctions.Click
            Using diag As New Dialogs.DiagFunctions
                If diag.ShowDialog() = DialogResult.OK Then
                    WriteFunction(diag.Result)
                End If
            End Using
        End Sub

        Private Sub BtnAC_Click(sender As Object, e As EventArgs) Handles BtnAC.Click
            Pnl.Focus()
            'FrmEditor.BringToFront()
            FrmEditor.Tb.Text = ""
            FrmEditor.LbResult.Text = "= "
        End Sub

        Private Sub BtnFn_Click(sender As Object, e As EventArgs) Handles BtnAsin.Click, BtnAcos.Click,
            BtnSin.Click, BtnCos.Click, BtnTan.Click, BtnAtan.Click, BtnLn.Click, BtnRand.Click,
            BtnPrint.Click, BtnRead.Click, BtnAvg.Click, BtnMedian.Click

            WriteFunction(DirectCast(sender, Button).Text & "()")
        End Sub

        Private Sub BtnRt_Click(sender As Object, e As EventArgs) Handles BtnQdtc.Click
            WriteFunction("quadratic(,,)")
        End Sub

        Private Sub BtnGcd_Click(sender As Object, e As EventArgs) Handles BtnPow10.Click
            WriteFunction(" E ")
        End Sub

        Private Sub BtnMod_Click(sender As Object, e As EventArgs) Handles BtnMod.Click
            WriteFunction(" mod ")
        End Sub

        Private Sub osk_Enter(sender As Object, e As EventArgs)
            Pnl.Focus()
            'FrmEditor.Tb.Focus()
        End Sub

        Private Sub CompleteBrace(chr As Char)
            If chr = "(" OrElse chr = "[" OrElse chr = "{" OrElse chr = ")" OrElse chr = "]" OrElse chr = "}" Then

                Dim startPos As Integer
                Dim curLine As Integer = FrmEditor.Tb.CurrentLine
                Dim curText As String = FrmEditor.Tb.Lines(curLine).Text

                While curLine > 0 AndAlso FrmEditor.Tb.Lines(curLine - 1).Text.EndsWith("\") ' connect \
                    curLine -= 1
                    curText = FrmEditor.Tb.Lines(curLine).Text.Remove(FrmEditor.Tb.Lines(curLine).Text.Length - 1) & curText
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

        Private Sub BtnBrLft_MouseUp(sender As Object, e As MouseEventArgs) Handles BtnBrLft.MouseUp,
            BtnSqBrL.MouseUp, BtnCBL.MouseUp, BtnCBR.MouseUp, BtnBrRt.MouseUp, BtnSqBrR.MouseUp, BtnAbs.MouseUp, BtnQuote.MouseUp,
            BtnTick.MouseUp

            FrmEditor.Tb.DeleteRange(FrmEditor.Tb.SelectionStart, FrmEditor.Tb.SelectionEnd - FrmEditor.Tb.SelectionStart)
            Dim startSign As Char = DirectCast(sender, Button).Text(0)
            FrmEditor.Tb.InsertText(FrmEditor.Tb.CurrentPosition, startSign)
            FrmEditor.Tb.SelectionStart += 1
            CompleteBrace(startSign)
        End Sub

        Private Sub Btnln_MouseUp(sender As Object, e As MouseEventArgs) Handles Btnlog.MouseUp
            If e.Button = MouseButtons.Left Then
                WriteFunction("log(,)")
            ElseIf e.Button = MouseButtons.Middle
                WriteFunction("lg()")
            Else
                WriteFunction("ln()")
            End If
        End Sub

        Private Sub BtnExp_MouseUp(sender As Object, e As MouseEventArgs) Handles BtnExp.MouseUp
            If e.Button = MouseButtons.Right Then
                WriteFunction(" E ")
            ElseIf e.Button = MouseButtons.Middle
                WriteFunction("^2")
            End If
        End Sub

        Private Sub BtnIntegral_Click(sender As Object, e As EventArgs) Handles BtnIntegral.Click
            WriteFunction("integral(,,)")
        End Sub

        Private Sub BtnDyDx_Click(sender As Object, e As EventArgs) Handles BtnDyDx.Click
            WriteFunction("dydx(,)")
        End Sub

        Private Sub BtnUnaryRight_Click(sender As Object, e As EventArgs) Handles BtnIf.Click, BtnWhile.Click, BtnNot.Click
            WriteFunction(DirectCast(sender, Button).Text & " ")
        End Sub

        Private Sub BtnBinary_Click(sender As Object, e As EventArgs) Handles BtnOr.Click, BtnAnd.Click
            WriteFunction(" " & DirectCast(sender, Button).Text & " ")
        End Sub

        Private Sub BtnSigma_Click(sender As Object, e As EventArgs) Handles BtnSigma.Click
            WriteFunction("sigma(,,)")
        End Sub

        Private Sub BtnPerm_Click(sender As Object, e As EventArgs) Handles BtnPerm.Click
            WriteFunction("perm(,)")
        End Sub

        Private Sub BtnChoose_Click(sender As Object, e As EventArgs) Handles BtnChoose.Click
            WriteFunction(" choose ")
        End Sub

        Private Sub BtnConfirm_Click(sender As Object, e As EventArgs) Handles BtnConfirm.Click
            WriteFunction("confirm()")
        End Sub

        Private Sub BtnYesNo_Click(sender As Object, e As EventArgs) Handles BtnYesNo.Click
            WriteFunction("yesno()")
        End Sub

        Private Sub KeyboardRight_Load(sender As Object, e As EventArgs) Handles Me.Load
            Me.SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.OptimizedDoubleBuffer, True)

            For Each c As Control In Me.Pnl.Controls
                AddHandler c.Enter, AddressOf osk_Enter
            Next
        End Sub

        Private Sub Pnl_MouseDown(sender As Object, e As MouseEventArgs) Handles Pnl.MouseDown
            Me.OnMouseDown(e)
        End Sub
        Private Sub Pnl_MouseMove(sender As Object, e As MouseEventArgs) Handles Pnl.MouseMove
            Me.OnMouseMove(e)
        End Sub
        Private Sub Pnl_MouseUp(sender As Object, e As MouseEventArgs) Handles Pnl.MouseUp
            Me.OnMouseUp(e)
        End Sub

        Private Sub BtnClose_Click(sender As Object, e As EventArgs) Handles BtnClose.Click
            If Not CloseKeyboardEvent Is Nothing Then RaiseEvent CloseKeyboard(Me, e)
        End Sub
    End Class
End Namespace