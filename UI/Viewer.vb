Imports System.Reflection
Imports System.Text
Imports System.Threading
Imports Cantus.Core
Imports Cantus.Core.CantusEvaluator
Imports Cantus.Core.CantusEvaluator.ObjectTypes
Imports Cantus.Core.Scoping
Imports Cantus.Core.CommonTypes
Imports Cantus.UI.ScintillaForCantus
Imports ScintillaNET

Namespace UI
    Public Class Viewer
        '' <summary>
        '' Max number of lines to display in log
        '' </summary>
        Private Const MAX_LINES As Integer = 500
        Dim _defaultPromptText As String = String.Format("{0}@{1}> ", Environment.UserName, Application.ProductName)
        Dim _promptText As String = String.Format("{0}@{1}> ", Environment.UserName, Application.ProductName)
        Dim _prevPrompt As New List(Of String)
        Dim _idx As Integer = 0

        '' <summary>
        '' If true, the console is reading user input, rather than taking commands
        '' </summary>
        Dim _reading As Boolean = False

        '' <summary>
        '' If true, executes blocks
        '' </summary>
        Dim _block As Boolean = False

        '' <summary>
        '' Event used to synchronize threads
        '' </summary>
        Dim _ev As New ManualResetEventSlim(False)

        '' <summary>
        '' The remaining text that the user entered
        '' </summary>
        Private _buf As New StringBuilder

        Public Class InputEventArgs
            Inherits EventArgs
            Public ReadOnly Property Text As String
            Public Sub New(text As String)
                Me.Text = text
            End Sub
        End Class

        '' <summary>
        '' Raised when input is read
        '' </summary>
        Public Event InputRead(ByVal sender As Object, ByVal e As InputEventArgs)


        '' <summary>
        '' Represents a view in the viewer
        '' </summary>
        Public Enum eView
            none = -1
            console = 0
            graphing = 1
        End Enum

        Friend GraphingControl As Graphing.GraphingSystem
        Friend ConsoleControl As ScintillaNET.Scintilla

        Private _view As eView = eView.none
        Private _cantusLexer As New CantusLexer()
        Private _feeder As New ScriptFeeder("", False, Globals.RootEvaluator)

        '' <summary>
        '' Get or set the current view of the viewer
        '' </summary>
        '' <returns></returns>
        Public Property View As eView
            Get
                Return _view
            End Get
            Set(view As eView)
                _view = view
                ' set tab colors
                Dim inactiveColor As Color = Me.BackColor
                Dim activeColor As Color = pnl.BackColor
                For Each c As Control In Me.Controls
                    If TypeOf c Is Button AndAlso Not c.Tag Is Nothing Then
                        c.BackColor = If(c.Tag.ToString() = view.ToString(), activeColor, inactiveColor)
                        Dim btn As Button = DirectCast(c, Button)
                        btn.FlatAppearance.MouseOverBackColor = btn.BackColor
                        If c.Tag.ToString() = view.ToString() Then
                            btn.FlatAppearance.MouseDownBackColor = btn.BackColor
                            btn.Cursor = Cursors.Default
                        Else
                            btn.FlatAppearance.MouseDownBackColor =
                                Color.FromArgb(btn.BackColor.R + 15, btn.BackColor.G + 15, btn.BackColor.B + 15)
                            btn.Cursor = Cursors.Hand
                        End If
                    End If
                Next

                pnl.Controls.Clear()
                Select Case view
                    Case eView.console
                        If ConsoleControl Is Nothing Then Exit Property
                        pnl.Controls.Add(ConsoleControl)
                        ConsoleControl.Select()
                    Case eView.graphing
                        If GraphingControl Is Nothing Then Exit Property
                        pnl.Controls.Add(GraphingControl)
                        GraphingControl.tb.Select()
                End Select
            End Set
        End Property

        Public Sub New()
            ' This call is required by the designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            AddHandler _feeder.ResultReceived, AddressOf ReceiveAnswer
        End Sub

        Dim _lastPromptLine As String = ""

        Private Sub DeletePromptLine()
            Dim lastline As Line = ConsoleControl.Lines(ConsoleControl.Lines.Count - 1)
            If lastline.Text.StartsWith(_promptText) Then
                _lastPromptLine = ConsoleControl.GetTextRange(lastline.Position, lastline.Length).Trim({ControlChars.Lf, ControlChars.Cr})
                ConsoleControl.DeleteRange(lastline.Position - 1, lastline.Length + 1)
                If ConsoleControl.Lines(ConsoleControl.Lines.Count - 1).Text.StartsWith(_promptText) Then
                    ConsoleControl.AppendText(ControlChars.Lf)
                End If
            End If
        End Sub

        Private Sub WritePromptLine(Optional appendPrevious As Boolean = False)
            AutoAddLine()
            If Not String.IsNullOrEmpty(_lastPromptLine) AndAlso appendPrevious Then
                ConsoleControl.AppendText(_lastPromptLine.Trim({ControlChars.Lf, ControlChars.Cr}))
            Else
                ConsoleControl.AppendText(_promptText)
            End If
            ConsoleControl.SelectionStart = ConsoleControl.TextLength
        End Sub

        Friend Sub AutoAddLine()
            If ConsoleControl.GetCharAt(ConsoleControl.TextLength - 1) <> AscW(ControlChars.Lf) Then
                ConsoleControl.AppendText(vbLf)
            End If
        End Sub

        '' <summary>
        '' Write a separator to the log
        '' </summary>
        '' <param name="appendPrevious">If true, sets the text on the new line to the text that was being edited on the previous line.</param>
        Public Sub WriteLogSeparator(Optional appendPrevious As Boolean = True)
            DeletePromptLine()
            AutoAddLine()
            ConsoleControl.AppendText("".PadRight(63, "-"c) & vbLf)
            WritePromptLine(appendPrevious)
        End Sub

        '' <summary>
        '' Write the specified text to the log, appending a separator before if appropriate
        '' </summary>
        '' <param name="appendPrevious">If true, sets the text on the new line to the text that was being edited on the previous line.</param>
        Public Sub WriteConsoleSection(text As String, Optional appendPrevious As Boolean = True)
            DeletePromptLine()

            If ConsoleControl.TextLength > 0 Then
                AutoAddLine()
                If ConsoleControl.Lines(ConsoleControl.Lines.Count - 2).Text.Trim().Replace("-", "") <> "" Then
                    ConsoleControl.AppendText("".PadRight(63, "-"c) & vbLf)
                End If
            End If

            ConsoleControl.AppendText(text & vbLf)
            If ConsoleControl.Lines.Count >= MAX_LINES Then ConsoleControl.DeleteRange(0, ConsoleControl.Lines(MAX_LINES).EndPosition)
            ConsoleControl.SelectionStart = ConsoleControl.TextLength
            ConsoleControl.ScrollCaret()

            WritePromptLine(appendPrevious)
        End Sub

        '' <summary>
        '' Write the specified text followed by a line break to the log
        '' </summary>
        '' <param name="appendPrevious">If true, sets the text on the new line to the text that was being edited on the previous line.</param>
        Public Sub WriteConsoleLine(text As String, Optional appendPrevious As Boolean = True)
            DeletePromptLine()

            If ConsoleControl.GetCharAt(ConsoleControl.TextLength - 1) <> AscW(ControlChars.Lf) Then
                ConsoleControl.AppendText(vbLf)
            End If

            ConsoleControl.AppendText(text & vbLf)
            If ConsoleControl.Lines.Count > MAX_LINES Then ConsoleControl.DeleteRange(0, ConsoleControl.Lines(MAX_LINES).Position)
            ConsoleControl.SelectionStart = ConsoleControl.TextLength
            ConsoleControl.ScrollCaret()

            WritePromptLine(appendPrevious)
        End Sub

        '' <summary>
        '' Write the specified text to the log
        '' </summary>
        '' <param name="appendPrevious">If true, sets the text on the new line to the text that was being edited on the previous line.</param>
        Public Sub WriteConsole(text As String, Optional appendPrevious As Boolean = True)
            DeletePromptLine()

            ConsoleControl.AppendText(text)
            If ConsoleControl.Lines.Count > MAX_LINES Then ConsoleControl.DeleteRange(0, ConsoleControl.Lines(MAX_LINES).Position)
            ConsoleControl.SelectionStart = ConsoleControl.TextLength
            ConsoleControl.ScrollCaret()

            WritePromptLine(appendPrevious)
        End Sub

        '' <summary>
        '' Clear this console
        '' </summary>
        Public Sub ClearConsole()
            ConsoleControl.Text = ""
            WritePromptLine()
        End Sub

        '' <summary>
        '' Ask the user to enter text
        '' </summary>
        Private Sub PromptRead()
            If ConsoleControl.InvokeRequired Then
                ConsoleControl.BeginInvoke(Sub() PromptRead())
            Else
                DeletePromptLine()
                _promptText = ""
                WritePromptLine()
                _reading = True
            End If
        End Sub

        '' <summary>
        '' Read one character from the console. Raises InputRead event when done.
        '' </summary>
        Public Sub ReadChar()
            While _buf.Length > 0 AndAlso AscW(_buf(0)) <= AscW(" ")
                _buf.Remove(0, 1)
            End While
            If _buf.Length > 0 Then
                RaiseEvent InputRead(Me, New InputEventArgs(_buf(0)))
                _buf.Remove(0, 1)
            Else
                Dim th As New Thread(Sub()
                                         While _reading OrElse _buf.Length = 0
                                             _reading = True
                                             _ev.Wait()
                                             _ev.Reset()
                                         End While
                                         ReadChar()
                                     End Sub)
                th.IsBackground = True
                PromptRead()
                th.Start()
            End If
        End Sub

        '' <summary>
        '' Read one line from the console. Raises InputRead event when done.
        '' </summary>
        Public Sub ReadLine()
            While _buf.Length > 0 AndAlso AscW(_buf(0)) <= AscW(" ")
                _buf.Remove(0, 1)
            End While
            If _buf.Length > 0 Then
                Dim line As String = _buf.ToString()
                Dim remove As Boolean = False
                If line.Contains(Environment.NewLine) Then
                    line = line.Remove(line.IndexOf(Environment.NewLine))
                    remove = True
                End If
                RaiseEvent InputRead(Me, New InputEventArgs(line))
                If remove Then
                    _buf = New StringBuilder(_buf.ToString().Remove(0, line.Length).TrimStart({ControlChars.Lf, ControlChars.Cr}))
                Else
                    _buf.Clear()
                End If
            Else
                Dim th As New Thread(Sub()
                                         While _reading OrElse _buf.Length = 0
                                             _reading = True
                                             _ev.Wait()
                                             _ev.Reset()
                                         End While
                                         ReadLine()
                                     End Sub)
                PromptRead()
                th.IsBackground = True
                th.Start()
            End If
        End Sub

        '' <summary>
        '' Read one word from the console. Raises InputRead event when done.
        '' </summary>
        Public Sub ReadWord()
            While _buf.Length > 0 AndAlso AscW(_buf(0)) <= AscW(" ")
                _buf.Remove(0, 1)
            End While
            If _buf.Length > 0 Then
                Dim word As New StringBuilder
                Dim started As Boolean = False
                For i As Integer = 0 To _buf.Length - 1
                    If AscW(_buf(i)) <= AscW(" "c) Then
                        If started Then
                            RaiseEvent InputRead(Me, New InputEventArgs(word.ToString()))
                            _buf.Remove(0, i + 1)
                            Return
                        End If
                    Else
                        started = True
                        word.Append(_buf(i))
                    End If
                Next
            Else
                Dim th As New Thread(Sub()
                                         While _reading OrElse _buf.Length = 0
                                             _reading = True
                                             _ev.Wait()
                                             _ev.Reset()
                                         End While
                                         ReadWord()
                                     End Sub)
                PromptRead()
                th.IsBackground = True
                th.Start()
            End If
        End Sub

        Private Sub btnMin_Click(sender As Object, e As EventArgs) Handles btnMin.Click
            pnl.Focus()
            'Me.WindowState = FormWindowState.Minimized
        End Sub

        Private Sub FrmViewer_MouseDown(sender As Object, e As MouseEventArgs) Handles MyBase.MouseDown, lbTitle.MouseDown,
                PbLogo.MouseDown
            FrmEditor.FrmEditor_MouseDown(FrmEditor, e)
        End Sub

        Private Sub lbTitle_MouseMove(sender As Object, e As MouseEventArgs) Handles MyBase.MouseMove, lbTitle.MouseMove,
                PbLogo.MouseMove
            FrmEditor.FrmEditor_MouseMove(FrmEditor, e)
        End Sub

        Private Sub lbTitle_MouseUp(sender As Object, e As MouseEventArgs) Handles MyBase.MouseUp, lbTitle.MouseUp, PbLogo.MouseUp
            FrmEditor.FrmEditor_MouseUp(FrmEditor, e)
        End Sub

        Private Sub btnTabs_Click(sender As Object, e As EventArgs)
            pnl.Focus()
            Me.Text = DirectCast(sender, Button).Text & " - Cantus"
            Me.View = DirectCast([Enum].Parse(GetType(eView), DirectCast(sender, Button).Tag.ToString()), eView)
        End Sub

        Protected Overrides Function ProcessCmdKey(ByRef msg As Message, keyData As Keys) As Boolean
            If _view <> eView.console Then Return False
            Dim lastLineStart As Integer = ConsoleControl.Lines(ConsoleControl.Lines.Count - 1).Position + _promptText.Length
            If keyData = Keys.Home OrElse keyData = (Keys.Shift Or Keys.Home) OrElse
                keyData = (Keys.Control Or Keys.Home) OrElse keyData = Keys.Enter OrElse keyData = (Keys.Control Or Keys.Z) OrElse keyData = (Keys.Control Or Keys.Y) Then
                Return True
            End If
            If ConsoleControl.SelectionStart > lastLineStart AndAlso ConsoleControl.SelectionEnd > lastLineStart Then
                If keyData = Keys.Up Then
                    Return True
                End If
                If keyData = (Keys.Control Or Keys.A) Then
                    ConsoleControl.SelectionStart = lastLineStart
                    ConsoleControl.SelectionEnd = ConsoleControl.TextLength
                    Return True
                End If
            ElseIf Not (ConsoleControl.SelectionStart = lastLineStart AndAlso ConsoleControl.SelectionEnd > lastLineStart) Then
                If keyData = (Keys.Control Or Keys.X) OrElse keyData = Keys.Back OrElse
                   ((keyData = Keys.Delete OrElse keyData = (Keys.Control Or Keys.V)) AndAlso
                   Not (ConsoleControl.SelectionStart = lastLineStart AndAlso ConsoleControl.SelectionEnd = lastLineStart)) Then

                    Return True
                End If
                If ConsoleControl.SelectionStart = lastLineStart Then
                    If keyData = Keys.Up Then
                        Return True
                    End If
                    If keyData = (Keys.Control Or Keys.A) Then
                        ConsoleControl.SelectionStart = lastLineStart
                        ConsoleControl.SelectionEnd = ConsoleControl.TextLength
                        Return True
                    End If
                End If
            End If
            Return MyBase.ProcessCmdKey(msg, keyData)
        End Function

        Private Sub SetupScintilla(tb As Scintilla)
            Dim backColor As Color = Color.FromArgb(37, 37, 37)
            tb.CaretForeColor = Color.GhostWhite

            With tb.Styles(Style.Default)
                .BackColor = backColor
                .Font = "Consolas"
                .Size = 13
            End With

            tb.SetSelectionBackColor(True, Color.GhostWhite)
            tb.SetSelectionForeColor(True, Color.Black)

            tb.StyleClearAll()
            tb.WrapMode = WrapMode.Word

            tb.Styles(CantusLexer.StyleDefault).ForeColor = Color.LightGray

            tb.Styles(CantusLexer.StyleKeyword).ForeColor = Color.FromArgb(147, 199, 99)
            tb.Styles(CantusLexer.StyleInlineKeyword).ForeColor = Color.FromArgb(103, 140, 177)

            tb.Styles(CantusLexer.StyleIdentifier).ForeColor = Color.FromArgb(241, 242, 243)
            tb.Styles(CantusLexer.StyleError).ForeColor = Color.LightGray

            tb.Styles(CantusLexer.StyleNumberBoolean).ForeColor = Color.FromArgb(255, 205, 34)
            tb.Styles(CantusLexer.StyleString).ForeColor = Color.FromArgb(236, 118, 0)

            tb.Styles(CantusLexer.StyleComment).ForeColor = Color.FromArgb(153, 163, 138)

            tb.Lexer = Lexer.Container

            tb.IndentWidth = 4

            With tb.Styles(Style.LineNumber)
                .BackColor = backColor
                .ForeColor = Color.DarkGray
                .Size = 13
            End With

            tb.IndentationGuides = IndentView.Real

            tb.Styles(Style.BraceLight).ForeColor = Color.BlueViolet
            tb.Styles(Style.BraceLight).BackColor = Color.LightGray

            tb.Styles(Style.BraceBad).ForeColor = Color.White
            tb.Styles(Style.BraceBad).BackColor = Color.IndianRed

            tb.Styles(Style.IndentGuide).ForeColor = Color.Gray

            tb.TabWidth = 4

            tb.ScrollWidth = tb.Width

            tb.AutoCIgnoreCase = True
        End Sub

        Private Sub FrmViewer_Load(sender As Object, e As EventArgs) Handles MyBase.Load
            For Each c As Control In Me.Controls
                If TypeOf c Is Button AndAlso Not c.Tag Is Nothing Then
                    AddHandler DirectCast(c, Button).Click, AddressOf btnTabs_Click
                End If
            Next
            AddHandler Me.KeyDown, AddressOf Control_KeyDown
            lbTitle.Text = lbTitle.Text.Replace("{VER}", Version.ToString())

            GraphingControl = New Graphing.GraphingSystem
            GraphingControl.Dock = DockStyle.Fill
            AddHandler GraphingControl.KeyDown, AddressOf Control_KeyDown
            AddHandler GraphingControl.tb.KeyDown, AddressOf Control_KeyDown

            ConsoleControl = New Scintilla
            ConsoleControl.Dock = DockStyle.Fill
            ConsoleControl.BorderStyle = BorderStyle.None

            SetupScintilla(ConsoleControl)
            AddHandler ConsoleControl.KeyDown, AddressOf Control_KeyDown
            AddHandler ConsoleControl.KeyUp, AddressOf ConsoleControl_KeyUp
            AddHandler ConsoleControl.KeyPress, AddressOf ConsoleControl_KeyPress
            AddHandler ConsoleControl.CharAdded, AddressOf ConsoleControl_CharAdded
            AddHandler ConsoleControl.AutoCCompleted, AddressOf ConsoleControl_AutoCCompleted
            AddHandler ConsoleControl.StyleNeeded, AddressOf ConsoleControl_StyleNeeded

            Me.View = eView.console

            WriteConsoleSection(
                String.Format(vbLf & "# Welcome to Cantus version {0}!" & vbLf & "# Copyright © Alex Yu 2016." & vbLf & "# http://github.com/sxyu/Cantus-GUI" & vbLf & vbLf & """""""" & vbLf &
                "---Virtual Console Help---" & vbLf & "Basic Input/Output" & vbLf & "Printing: print() or printline()" & vbLf & "Reading: read() readline() or readchar()" & vbLf & vbLf & "You can directly use this console for simple calculations:" & vbLf &
                "Simply enter mathematical expressions to begin." & vbLf & "Note: End the line with an extra ':' character to run blocks. Enter an empty line to end a block." & vbLf & vbLf & "Press Alt + Enter or click the 'Run' button on the" & vbLf & "bottom right " & "of the editor to print out the current result." & vbLf & """""""",
                                   Version.ToString))

            pnl.Select()
            _feeder.BeginExecution()
        End Sub

        Dim allowKey As Boolean = False
        Private Sub Control_KeyDown(sender As Object, e As KeyEventArgs)
            If e.Alt Then
                If e.Control Then
                    If e.KeyCode = Keys.F Then
                        FrmEditor.BtnSigFigs.PerformClick()
                    ElseIf e.KeyCode = Keys.T Then
                        FrmEditor.BtnExplicit.PerformClick()
                    End If
                Else
                    If e.KeyCode = Keys.G Then
                        btnGraph.PerformClick()
                    ElseIf e.KeyCode = Keys.C Then
                        btnConsole.PerformClick()
                    ElseIf e.KeyCode = Keys.E Then
                        FrmEditor.BringToFront()
                    ElseIf e.KeyCode = Keys.S Then
                        FrmEditor.BtnSettings.PerformClick()
                    ElseIf e.KeyCode = Keys.F Then
                        FrmEditor.BtnFunctions.PerformClick()
                    ElseIf e.KeyCode = Keys.T Then
                        FrmEditor.BtnTranslucent.PerformClick()
                        DirectCast(sender, Control).Focus()
                    ElseIf e.KeyCode = Keys.P Then
                        FrmEditor.BtnAngleRepr.PerformClick()
                    ElseIf e.KeyCode = Keys.D OrElse e.KeyCode = Keys.R OrElse e.KeyCode = Keys.G Then
                        If e.KeyCode = Keys.D Then
                            RootEvaluator.AngleMode = eAngleRepresentation.Degree
                        ElseIf e.KeyCode = Keys.R
                            RootEvaluator.AngleMode = eAngleRepresentation.Radian
                        Else
                            RootEvaluator.AngleMode = eAngleRepresentation.Gradian
                        End If
                        FrmEditor.BtnAngleRepr.Text = RootEvaluator.AngleMode.ToString()
                        FrmEditor.EvaluateExpr(True)
                    ElseIf e.KeyCode = Keys.O Then
                        FrmEditor.BtnOutputFormat.PerformClick()
                    ElseIf e.KeyCode = Keys.M OrElse e.KeyCode = Keys.S OrElse e.KeyCode = Keys.W Then
                        If e.KeyCode = Keys.M Then
                            RootEvaluator.OutputFormat = eOutputFormat.Math
                            e.SuppressKeyPress = True
                        ElseIf e.KeyCode = Keys.W
                            RootEvaluator.OutputFormat = eOutputFormat.Raw
                        Else
                            RootEvaluator.OutputFormat = eOutputFormat.Scientific
                        End If
                        FrmEditor.BtnOutputFormat.Text = RootEvaluator.OutputFormat.ToString()
                        FrmEditor.EvaluateExpr(True)
                    End If
                End If
            End If
        End Sub

        Private Sub ConsoleControl_KeyDown(sender As Object, e As KeyEventArgs)
            If e.Control AndAlso (e.KeyCode = Keys.A OrElse e.KeyCode = Keys.C) AndAlso Not e.Shift Then
                allowKey = True
            Else
                allowKey = False
                e.SuppressKeyPress = True
            End If
        End Sub

        Private Sub ConsoleControl_KeyUp(sender As Object, e As KeyEventArgs)
            If e.KeyCode = Keys.Home Then
                ConsoleControl.SelectionStart = ConsoleControl.Lines(ConsoleControl.Lines.Count - 1).Position + _promptText.Length
                If Not e.Shift OrElse e.Control Then
                    ConsoleControl.SelectionEnd = ConsoleControl.Lines(ConsoleControl.Lines.Count - 1).Position + _promptText.Length
                End If
            ElseIf e.KeyCode = Keys.Up Then
                Dim lastline As Line = ConsoleControl.Lines(ConsoleControl.Lines.Count - 1)
                If ConsoleControl.SelectionStart < lastline.Position Then Exit Sub
                If _idx > 0 Then
                    ConsoleControl.DeleteRange(lastline.Position + _promptText.Length, lastline.Length)
                    ConsoleControl.AppendText(_prevPrompt(_idx - 1))
                    _idx -= 1
                End If
                ConsoleControl.SelectionStart = ConsoleControl.TextLength
            ElseIf e.KeyCode = Keys.Down Then
                Dim lastline As Line = ConsoleControl.Lines(ConsoleControl.Lines.Count - 1)
                If ConsoleControl.SelectionStart < lastline.Position Then Exit Sub
                If _idx < _prevPrompt.Count Then
                    ConsoleControl.DeleteRange(lastline.Position + _promptText.Length, lastline.Length)
                    If (_idx + 1 < _prevPrompt.Count) Then ConsoleControl.AppendText(_prevPrompt(_idx + 1))
                    _idx += 1
                End If
                ConsoleControl.SelectionStart = ConsoleControl.TextLength

            ElseIf e.KeyCode = Keys.Enter Then
                Dim lastLineText As String = ConsoleControl.Lines(ConsoleControl.Lines.Count - 1).Text

                If lastLineText.Length >= _promptText.Length Then
                    lastLineText = lastLineText.Substring(_promptText.Length)
                    If _reading Then
                        _buf.AppendLine(lastLineText)
                        _reading = False
                        _promptText = _defaultPromptText
                        _ev.Set()

                    Else
                        If Not String.IsNullOrWhiteSpace(lastLineText) Then
                            If _idx >= _prevPrompt.Count Then
                                _prevPrompt.Add(lastLineText)
                                _idx = _prevPrompt.Count
                            Else
                                _prevPrompt.Insert(_idx + 1, lastLineText)
                                _idx += 1
                            End If
                        End If
                        ConsoleControl.AutoCCancel()

                        If lastLineText.EndsWith(":") Then
                            lastLineText = lastLineText.Remove(lastLineText.Length - 1)
                            _block = True
                        End If

                        If _block Then
                            _feeder.Append(lastLineText, True)
                            If String.IsNullOrWhiteSpace(lastLineText) Then
                                _block = False
                                _feeder.EndAfterQueueDone()
                            End If
                        Else
                            WritePromptLine()
                            Try
                                Dim th As New Thread(Sub()
                                                         Try
                                                             Dim res As String = RootEvaluator.Eval(lastLineText)
                                                             ConsoleControl.BeginInvoke(Sub()
                                                                                            WriteConsoleLine(res)
                                                                                        End Sub)
                                                         Catch ex As Exception
                                                             ConsoleControl.BeginInvoke(Sub()
                                                                                            WriteConsoleLine(ex.Message)
                                                                                        End Sub)
                                                         End Try
                                                     End Sub)
                                th.IsBackground = True
                                th.Start()
                            Catch ex As Exception
                                WriteConsoleLine(ex.Message)
                            End Try
                        End If
                    End If
                End If

                If _block OrElse _reading Then
                    ConsoleControl.AppendText(vbLf)
                    WritePromptLine()
                End If
                e.Handled = True
            End If
            e.SuppressKeyPress = True
        End Sub

        Private Sub ReceiveAnswer(sender As Object, result As String)
            _block = False
            If Me.InvokeRequired Then
                Me.BeginInvoke(Sub() ReceiveAnswer(sender, result))
            Else
                DirectCast(sender, ScriptFeeder).BeginExecution()
                WriteConsoleLine(result, False)
            End If
        End Sub

        Private Sub ConsoleControl_KeyPress(sender As Object, e As KeyPressEventArgs)
            If allowKey Then
                allowKey = False
            Else
                Dim lastLineStart As Integer = ConsoleControl.Lines(ConsoleControl.Lines.Count - 1).Position + _promptText.Length
                If ConsoleControl.SelectionStart >= lastLineStart Then
                Else
                    e.Handled = True
                End If
            End If
        End Sub

        ' syntax highlighting
        Private Sub ConsoleControl_StyleNeeded(sender As Object, e As StyleNeededEventArgs)
            Dim startPos As Integer = ConsoleControl.GetEndStyled()
            Dim endPos As Integer = e.Position
            _cantusLexer.Style(ConsoleControl, startPos, endPos)
        End Sub

        Private Sub ConsoleControl_CharAdded(sender As Object, e As CharAddedEventArgs)

            ' autocomplete
            Dim currentPos As Integer = ConsoleControl.CurrentPosition
            Dim wordStartPos As Integer = ConsoleControl.CurrentPosition

            While wordStartPos - 1 >= 0 AndAlso (
                  ConsoleControl.GetCharAt(wordStartPos - 1) >= AscW("0"c) AndAlso ConsoleControl.GetCharAt(wordStartPos - 1) <= AscW("9"c) OrElse ConsoleControl.GetCharAt(wordStartPos - 1) >= AscW("a"c) AndAlso ConsoleControl.GetCharAt(wordStartPos - 1) <= AscW("z"c) OrElse ConsoleControl.GetCharAt(wordStartPos - 1) >= AscW("A"c) AndAlso ConsoleControl.GetCharAt(wordStartPos - 1) <= AscW("Z"c) OrElse ConsoleControl.GetCharAt(wordStartPos - 1) = AscW("_"c) OrElse ConsoleControl.GetCharAt(wordStartPos - 1) = AscW("."c))
                wordStartPos -= 1
            End While

            If ConsoleControl.GetCharAt(wordStartPos) = AscW("."c) Then wordStartPos += 1

            Dim enteredWord As String = ConsoleControl.GetTextRange(wordStartPos, currentPos)

            Dim lenEntered As Integer = currentPos - wordStartPos

            If lenEntered > 0 AndAlso Not _reading Then

                Dim curLineText As String = ConsoleControl.GetTextRange(ConsoleControl.Lines(ConsoleControl.CurrentLine).Position, ConsoleControl.CurrentPosition)

                Dim singleQuote As Boolean = False
                Dim doubleQuote As Boolean = False
                For i As Integer = 0 To curLineText.Count - 1
                    If curLineText(i) = "'"c Then singleQuote = Not singleQuote
                    If curLineText(i) = """"c Then doubleQuote = Not doubleQuote
                Next
                If singleQuote OrElse doubleQuote Then Return ' do not autocomplete string

                curLineText = ConsoleControl.Lines(ConsoleControl.CurrentLine).Text
                Dim keyword As String = curLineText

                Dim blockKwd As String() = ("class function namespace").Split(" "c)
                keyword = keyword.Trim()
                If keyword.Contains(" ") Then keyword = keyword.Remove(keyword.IndexOf(" "))

                If blockKwd.Contains(keyword) Then Return ' do not autocomplete class, function, namespace names 

                ' do not autocomplete variable declarations unless after keyword
                If (keyword = "let" OrElse keyword = "global") AndAlso Not curLineText.Contains("=") Then Return

                Dim keywords As String() = "function global let private public static".Split(" "c)

                Dim autoCList As New List(Of String)

                If keywords.Contains(keyword) AndAlso Not curLineText.Contains("=") Then
                    autoCList.AddRange(keywords)
                Else
                    Dim nsMode As Boolean = enteredWord.Contains(".")

                    If Not nsMode Then
                        autoCList.AddRange(("class function namespace if else elif for repeat return continue private public " &
                                           "let static global ref undefined null " &
                                           "switch case run try catch finally while until with in step to choose").Split(" "c))

                        autoCList.Add(ROOT_NAMESPACE)
                    End If

                    For Each v As Variable In RootEvaluator.Variables.Values.ToArray()
                        ' ignore private
                        If v.Modifiers.Contains("internal") OrElse (v.Modifiers.Contains("private") AndAlso
                            Not IsParentScopeOf(v.DeclaringScope, RootEvaluator.Scope)) Then Continue For

                        ' ignore null
                        If v.Value Is Nothing OrElse TypeOf v.Value Is Double AndAlso Double.IsNaN(CDbl(v.Value)) OrElse
                            TypeOf v.Value Is BigDecimal AndAlso DirectCast(v.Value, BigDecimal).IsUndefined Then Continue For

                        If nsMode Then ' filter namespace
                            Dim partialName As String = RemoveRedundantScope(v.FullName, Globals.RootEvaluator.Scope)

                            If v.FullName.ToLower().StartsWith(enteredWord.ToLower()) Then
                                autoCList.Add(v.FullName)

                            ElseIf partialName.ToLower().StartsWith(enteredWord.ToLower()) Then
                                autoCList.Add(partialName.ToLower())

                            ElseIf enteredWord.ToLower().StartsWith(partialName.ToLower()) OrElse enteredWord.ToLower().StartsWith(v.FullName.ToLower())
                                If enteredWord.ToLower().StartsWith(v.FullName.ToLower()) Then partialName = v.FullName
                                If TypeOf v.Reference.Resolve() Is ClassInstance Then
                                    Dim ci As ClassInstance = DirectCast(v.Reference.Resolve(), ClassInstance)
                                    For Each f As String In ci.Fields.Keys.ToArray()
                                        autoCList.Add(CombineScope(partialName,
                                                   f & If(TypeOf ci.Fields(f).ResolveObj() Is Lambda, "(" &
                                                   If(DirectCast(ci.Fields(f).ResolveObj(), Lambda).Args.Count = 0, "", "_") & ")",
                                              "")))
                                    Next
                                End If
                            Else
                                Continue For
                            End If
                        Else
                            autoCList.Add(RemoveRedundantScope(v.FullName, Globals.RootEvaluator.Scope) &
                                          If(TypeOf v.Value Is Lambda, "(" & If(DirectCast(v.Value, Lambda).Args.Count = 0, "", "_") & ")", ""))
                        End If
                    Next

                    ' internal functions
                    Dim varname As String = ""
                    Dim type As Type = Nothing
                    If nsMode AndAlso enteredWord.IndexOf(".") < enteredWord.Length AndAlso Globals.RootEvaluator.HasVariable(enteredWord.Remove(enteredWord.IndexOf("."))) Then

                        varname = enteredWord.Remove(enteredWord.IndexOf("."))

                        Dim var As Object = Nothing
                        var = Globals.RootEvaluator.GetVariable(enteredWord.Remove(enteredWord.IndexOf(".")))
                        If TypeOf var Is Reference AndAlso Not TypeOf DirectCast(var, Reference).GetRefObject() Is Reference Then
                            type = DirectCast(var, Reference).Resolve().GetType
                        Else
                            type = var.GetType()
                        End If
                    End If

                    For Each fn As MethodInfo In InternalFunctions.Methods
                        If nsMode Then
                            If enteredWord.StartsWith("cantus") Then
                                autoCList.Add(ROOT_NAMESPACE & SCOPE_SEP & fn.Name.ToLower() & "(" & If(fn.GetParameters().Count = 0, "", "_") & ")")

                            ElseIf fn.GetParameters().Count > 0 AndAlso Not String.IsNullOrEmpty(varname) Then
                                If fn.GetParameters(0).ParameterType.IsAssignableFrom(type) Then
                                    autoCList.Add(varname & SCOPE_SEP & fn.Name.ToLower() & "(" & If(fn.GetParameters().Count <= 1, "", "_") & ")")
                                End If
                            End If
                        Else
                            autoCList.Add(fn.Name.ToLower() & "(" & If(fn.GetParameters().Count = 0, "", "_") & ")")
                        End If
                    Next

                    For Each fn As UserFunction In Globals.RootEvaluator.UserFunctions.Values.ToArray()
                        If nsMode Then
                            If Not fn.FullName.ToLower().StartsWith(enteredWord.ToLower()) AndAlso Not fn.FullName.ToLower().StartsWith(RemoveRedundantScope(fn.FullName, Globals.RootEvaluator.Scope).ToLower()) Then
                                Continue For
                            End If
                        End If
                        ' ignore private
                        If fn.Modifiers.Contains("internal") OrElse (fn.Modifiers.Contains("private") AndAlso Not IsParentScopeOf(fn.DeclaringScope, Globals.RootEvaluator.Scope)) Then Continue For

                        If nsMode Then ' filter namespace
                            If fn.FullName.ToLower().StartsWith(enteredWord.ToLower()) Then
                                autoCList.Add(fn.FullName & "(" & If(fn.Args.Count = 0, "", "_") & ")")
                            ElseIf RemoveRedundantScope(fn.FullName, Globals.RootEvaluator.Scope).ToLower().StartsWith(enteredWord.ToLower()) Then
                                autoCList.Add(RemoveRedundantScope(fn.FullName, Globals.RootEvaluator.Scope) & "(" & If(fn.Args.Count = 0, "", "_") & ")")
                            Else
                                Continue For
                            End If
                        Else
                            autoCList.Add(RemoveRedundantScope(fn.FullName, Globals.RootEvaluator.Scope) & "(" & If(fn.Args.Count = 0, "", "_") & ")")
                        End If
                    Next

                    For Each uc As UserClass In Globals.RootEvaluator.UserClasses.Values.ToArray()
                        If nsMode Then
                            If Not uc.FullName.ToLower().StartsWith(enteredWord.ToLower()) AndAlso Not uc.FullName.ToLower().StartsWith(RemoveRedundantScope(uc.FullName, Globals.RootEvaluator.Scope).ToLower()) Then
                                Continue For
                            End If
                        End If
                        ' ignore private
                        If uc.Modifiers.Contains("internal") OrElse (uc.Modifiers.Contains("private") AndAlso Not IsParentScopeOf(uc.DeclaringScope, Globals.RootEvaluator.Scope)) Then Continue For

                        If nsMode Then ' filter namespace
                            If uc.FullName.ToLower().StartsWith(enteredWord.ToLower()) Then
                                autoCList.Add(uc.FullName & "(" & If(uc.Constructor.Args.Count = 0, "", "_") & ")")
                            ElseIf RemoveRedundantScope(uc.FullName, Globals.RootEvaluator.Scope).ToLower().StartsWith(enteredWord.ToLower()) Then
                                autoCList.Add(RemoveRedundantScope(uc.FullName, Globals.RootEvaluator.Scope) & "(" & If(uc.Constructor.Args.Count = 0, "", "_") & ")")
                            Else
                                Continue For
                            End If
                        Else
                            autoCList.Add(RemoveRedundantScope(uc.FullName, Globals.RootEvaluator.Scope) & "(" & If(uc.Constructor.Args.Count = 0, "", "_") & ")")
                        End If
                    Next
                    autoCList.Sort(New AutoCompleteComparer())
                End If

                If autoCList.Count = 0 Then Return
                ConsoleControl.AutoCShow(lenEntered, String.Join(" ", autoCList))
            End If

            ' brace completion
            If e.Char = AscW("(") OrElse e.Char = AscW("[") OrElse e.Char = AscW("{") OrElse e.Char = AscW(")") OrElse e.Char = AscW("]") OrElse e.Char = AscW("}") Then

                Dim startPos As Integer
                Dim curLine As Integer = ConsoleControl.CurrentLine
                Dim curText As String = ConsoleControl.Lines(curLine).Text

                While curLine > 0 AndAlso ConsoleControl.Lines(curLine - 1).Text.EndsWith(" _") ' connect _
                    curLine -= 1
                    curText = ConsoleControl.Lines(curLine).Text.Remove(ConsoleControl.Lines(curLine).Text.Length - 2) & curText
                End While
                startPos = ConsoleControl.Lines(curLine).Position

                Dim startBr As Char = ChrW(e.Char)
                Dim endBr As Char
                Dim reverse As Boolean = False
                Dim ct As Integer = 0

                Select Case ChrW(e.Char)
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
                        Dim len As Integer = ConsoleControl.CurrentPosition - startPos
                        If curText.Length > len Then curText = curText.Remove(len)

                        Dim braceList As Char() = {"["c, "("c, "{"c}
                        Dim endBraceList As Char() = {"]"c, ")"c, "}"c}
                        Dim lvl As List(Of Integer)() = {New List(Of Integer)({0}),
                            New List(Of Integer)({0}), New List(Of Integer)({0})}
                        Dim pos As Integer = _promptText.Length

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

                        ConsoleControl.InsertText(ConsoleControl.Lines(ConsoleControl.CurrentLine).Position + pos, endBr)
                    Else
                        ConsoleControl.InsertText(ConsoleControl.CurrentPosition, endBr)
                    End If
                End If

            ElseIf e.Char = AscW("|") OrElse e.Char = AscW(""""c) OrElse e.Char = AscW("'"c) OrElse e.Char = AscW("`"c) Then
                If e.Char = AscW(""""c) AndAlso ConsoleControl.CurrentPosition > 1 AndAlso ConsoleControl.GetTextRange(ConsoleControl.CurrentPosition - 2, 2) = """""" OrElse e.Char = AscW("'"c) AndAlso ConsoleControl.CurrentPosition > 1 AndAlso ConsoleControl.GetTextRange(ConsoleControl.CurrentPosition - 2, 2) = "'" & "'" Then

                    ' if there were already two quotes before, do not add another: user probably wanted to type triple quotes
                    ConsoleControl.SelectionStart += 1
                    Return
                End If

                Dim curText As String = ConsoleControl.Lines(ConsoleControl.CurrentLine).Text
                Dim ct As Boolean = False

                For i As Integer = 0 To curText.Length - 1
                    If curText(i) = ChrW(e.Char) Then ct = Not ct
                Next

                If ct Then ConsoleControl.InsertText(ConsoleControl.CurrentPosition, ChrW(e.Char))

            End If
        End Sub

        Private Sub ConsoleControl_AutoCCompleted(sender As Object, e As AutoCSelectionEventArgs)
            If e.Text.EndsWith("(_)") Then
                ConsoleControl.DeleteRange(ConsoleControl.SelectionStart - 2, 1)
                ConsoleControl.SelectionStart -= 1
                ConsoleControl.SelectionEnd -= 1
            End If
        End Sub
    End Class
End Namespace