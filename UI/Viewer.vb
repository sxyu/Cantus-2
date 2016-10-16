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
        '' Max number of lines to display in console
        '' </summary>
        Private Const MAX_LINES As Integer = 150
        Dim _defaultPromptText As String = String.Format("{0}@{1}> ", Environment.UserName, Application.ProductName)
        Dim _promptText As String = String.Format("{0}@{1}> ", Environment.UserName, Application.ProductName)
        Dim _prevPrompt As New List(Of String)
        Dim _idx As Integer = 0
        Dim _line As Integer = 0

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
        Public Enum ViewType
            none = -1
            console = 0
            graphing = 1
        End Enum

        Friend GraphingControl As Graphing.GraphingSystem
        Friend ConsoleControl As ScintillaNET.Scintilla

        Private _view As ViewType = ViewType.none
        Private _ctl As ScintillaController
        Private _feeder As New ScriptFeeder("", False, Globals.RootEvaluator)

        '' <summary>
        '' Get or set the current view of the viewer
        '' </summary>
        '' <returns></returns>
        Public Property View As ViewType
            Get
                Return _view
            End Get
            Set(view As ViewType)
                _view = view
                ' set tab colors
                Dim inactiveColor As Color = Me.BackColor
                Dim activeColor As Color = Pnl.BackColor
                For Each c As Control In Me.Controls
                    If TypeOf c Is Button AndAlso Not c.Tag Is Nothing Then
                        c.BackColor = If(c.Tag.ToString() = view.ToString(), activeColor, inactiveColor)
                        Dim Btn As Button = DirectCast(c, Button)
                        Btn.FlatAppearance.MouseOverBackColor = Btn.BackColor
                        If c.Tag.ToString() = view.ToString() Then
                            Btn.FlatAppearance.MouseDownBackColor = Btn.BackColor
                            Btn.Cursor = Cursors.Default
                        Else
                            Btn.FlatAppearance.MouseDownBackColor =
                                Color.FromArgb(Btn.BackColor.R + 15, Btn.BackColor.G + 15, Btn.BackColor.B + 15)
                            Btn.Cursor = Cursors.Hand
                        End If
                    End If
                Next

                Pnl.Controls.Clear()
                Select Case view
                    Case ViewType.console
                        If ConsoleControl Is Nothing Then Exit Property
                        Pnl.Controls.Add(ConsoleControl)
                        ConsoleControl.Select()
                    Case ViewType.graphing
                        If GraphingControl Is Nothing Then Exit Property
                        Pnl.Controls.Add(GraphingControl)
                        GraphingControl.Tb.Select()
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
            _line = ConsoleControl.Lines.Count
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
            ConsoleControl.AppendText("".PadRight(ConsoleControl.Width \ 11, "-"c))
            WritePromptLine(appendPrevious)
        End Sub

        Private Sub TruncateConsole()
            If ConsoleControl.Lines.Count > MAX_LINES Then
                Dim tc1 As String = "'''"
                Dim tc2 As String = """"""""
                Dim txt As String = ConsoleControl.GetTextRange(0,
                            ConsoleControl.Lines(ConsoleControl.Lines.Count - MAX_LINES).EndPosition)

                ConsoleControl.DeleteRange(0, ConsoleControl.Lines(ConsoleControl.Lines.Count - MAX_LINES).EndPosition)

                Dim tc1o As Boolean = New InternalFunctions(Nothing).Count(txt, tc1) Mod 2 = 1
                Dim tc2o As Boolean = New InternalFunctions(Nothing).Count(txt, tc2) Mod 2 = 1

                While (tc1o OrElse tc2o) AndAlso ConsoleControl.Lines.Count > 0
                    txt = ConsoleControl.GetTextRange(0, ConsoleControl.Lines(0).EndPosition)
                    tc1o = (tc1o Xor (New InternalFunctions(Nothing).Count(txt, tc1) Mod 2 = 1))
                    tc2o = (tc2o Xor (New InternalFunctions(Nothing).Count(txt, tc2) Mod 2 = 1))

                    ConsoleControl.DeleteRange(0, ConsoleControl.Lines(0).EndPosition)
                End While

            End If
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
                    ConsoleControl.AppendText("".PadRight(ConsoleControl.Width \ 11, "-"c))
                End If
            End If
            AutoAddLine()

            ConsoleControl.AppendText(text & vbLf)
            TruncateConsole()
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
            TruncateConsole()
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
            TruncateConsole()
            ConsoleControl.SelectionStart = ConsoleControl.TextLength
            ConsoleControl.ScrollCaret()

            'WritePromptLine(appendPrevious)
        End Sub

        ''' <summary>
        ''' Clear this console
        ''' </summary>
        Public Sub ClearConsole()
            ConsoleControl.Text = ""
            WritePromptLine()
        End Sub

        ''' <summary>
        ''' Scroll console to bottom
        ''' </summary>
        Public Sub ConsoleScrollToBottom()
            ConsoleControl.SelectionStart = ConsoleControl.TextLength
            ConsoleControl.ScrollCaret()
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
                ConsoleControl.Focus()
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
                                             WritePromptLine()
                                         End While
                                         ReadWord()
                                     End Sub)
                PromptRead()
                th.IsBackground = True
                th.Start()
            End If
        End Sub

        Private Sub BtnMin_Click(sender As Object, e As EventArgs) Handles BtnMin.Click
            Pnl.Focus()
            'Me.WindowState = FormWindowState.Minimized
        End Sub

        Private Sub FrmViewer_MouseDown(sender As Object, e As MouseEventArgs) Handles MyBase.MouseDown, LbTitle.MouseDown,
                PbLogo.MouseDown
            FrmEditor.FrmEditor_MouseDown(FrmEditor, e)
        End Sub

        Private Sub LbTitle_MouseMove(sender As Object, e As MouseEventArgs) Handles MyBase.MouseMove, LbTitle.MouseMove,
                PbLogo.MouseMove
            FrmEditor.FrmEditor_MouseMove(FrmEditor, e)
        End Sub

        Private Sub LbTitle_MouseUp(sender As Object, e As MouseEventArgs) Handles MyBase.MouseUp, LbTitle.MouseUp, PbLogo.MouseUp
            FrmEditor.FrmEditor_MouseUp(FrmEditor, e)
        End Sub

        Private Sub BtnTabs_Click(sender As Object, e As EventArgs)
            Pnl.Focus()
            Me.Text = DirectCast(sender, Button).Text & " - Cantus"
            Me.View = DirectCast([Enum].Parse(GetType(ViewType), DirectCast(sender, Button).Tag.ToString()), ViewType)
        End Sub

        Protected Overrides Function ProcessCmdKey(ByRef msg As Message, keyData As Keys) As Boolean
            If _view <> ViewType.console Then Return False
            Dim lastLineStart As Integer = ConsoleControl.Lines(_line - 1).Position + _promptText.Length
            If keyData = Keys.Home OrElse keyData = (Keys.Shift Or Keys.Home) OrElse
                keyData = (Keys.Control Or Keys.Home) OrElse keyData = Keys.Enter OrElse keyData = (Keys.Control Or Keys.Z) OrElse keyData = (Keys.Control Or Keys.Y) Then
                Return True
            End If
            If ConsoleControl.SelectionStart > lastLineStart AndAlso ConsoleControl.SelectionEnd > lastLineStart Then
                If keyData = Keys.Up AndAlso Not ConsoleControl.AutoCActive Then
                    Return True
                End If
                If keyData = (Keys.Control Or Keys.A) Then
                    ConsoleControl.SelectionStart = lastLineStart
                    ConsoleControl.SelectionEnd = ConsoleControl.TextLength
                    Return True
                End If
            ElseIf Not (ConsoleControl.SelectionStart = lastLineStart AndAlso ConsoleControl.SelectionEnd > lastLineStart) Then
                If keyData = (Keys.Control Or Keys.X) OrElse
                    (keyData Or Keys.Control Or Keys.Alt Or Keys.Shift) = (Keys.Back Or Keys.Control Or Keys.Alt Or Keys.Shift) OrElse
                   (((keyData Or Keys.Control Or Keys.Alt Or Keys.Shift) = (Keys.Delete Or Keys.Alt Or Keys.Control Or Keys.Shift) OrElse
                   keyData = (Keys.Control Or Keys.V)) AndAlso
                   Not (ConsoleControl.SelectionStart = lastLineStart AndAlso ConsoleControl.SelectionEnd = lastLineStart)) Then
                    Return True
                End If
                If ConsoleControl.SelectionStart = lastLineStart Then
                    If keyData = Keys.Up AndAlso Not ConsoleControl.AutoCActive Then
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

        Private Sub SetupScintilla(Tb As Scintilla)
            Dim backColor As Color = Color.FromArgb(37, 37, 37)
            _ctl = New ScintillaController(Tb, RootEvaluator,
                                           _defaultPromptText.Length)
            Tb.CaretForeColor = Color.GhostWhite
            Tb.IndentWidth = 0
        End Sub

        Private Sub FrmViewer_Load(sender As Object, e As EventArgs) Handles MyBase.Load
            For Each c As Control In Me.Controls
                If TypeOf c Is Button AndAlso Not c.Tag Is Nothing Then
                    AddHandler DirectCast(c, Button).Click, AddressOf BtnTabs_Click
                End If
            Next
            AddHandler Me.KeyDown, AddressOf Control_KeyDown
            LbTitle.Text = LbTitle.Text.Replace("{VER}", Version.ToString())

            GraphingControl = New Graphing.GraphingSystem
            GraphingControl.Dock = DockStyle.Fill
            AddHandler GraphingControl.KeyDown, AddressOf Control_KeyDown
            AddHandler GraphingControl.Tb.KeyDown, AddressOf Control_KeyDown

            ConsoleControl = New Scintilla
            ConsoleControl.Dock = DockStyle.Fill
            ConsoleControl.BorderStyle = BorderStyle.None

            SetupScintilla(ConsoleControl)
            AddHandler ConsoleControl.KeyDown, AddressOf Control_KeyDown
            AddHandler ConsoleControl.KeyUp, AddressOf ConsoleControl_KeyUp
            AddHandler ConsoleControl.KeyPress, AddressOf ConsoleControl_KeyPress

            Me.View = ViewType.console

            WriteConsoleSection(
                String.Format("# Welcome to Cantus version {0}!" & vbLf & "# Copyright © Alex Yu 2016." & vbLf & "# http://github.com/sxyu/Cantus-GUI" & vbLf & vbLf & """""""" & vbLf &
                "---Virtual Console Help---" & vbLf & "Basic Input/Output" & vbLf & "Printing: print() or printline()" & vbLf & "Reading: read() readline() or readchar()" & vbLf & vbLf & "You can directly use this console for simple calculations:" & vbLf &
                "Simply enter mathematical expressions to begin." & vbLf & "Note: End the line with an extra ':' character to run blocks. Enter an empty line to end a block." & vbLf & vbLf & "Press Alt + Enter or click the 'Run' button on the" & vbLf & "bottom right " & "of the editor to print out the current result." & vbLf & """""""",
                                   Version.ToString))

            Pnl.Select()
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
                    ElseIf e.KeyCode = Keys.P Then
                        FrmEditor.BtnAngleRepr.PerformClick()
                    ElseIf e.KeyCode = Keys.D OrElse e.KeyCode = Keys.R OrElse e.KeyCode = Keys.G Then
                        If e.KeyCode = Keys.D Then
                            RootEvaluator.AngleMode = AngleRepresentation.Degree
                        ElseIf e.KeyCode = Keys.R Then
                            RootEvaluator.AngleMode = AngleRepresentation.Radian
                        Else
                            RootEvaluator.AngleMode = AngleRepresentation.Gradian
                        End If
                        FrmEditor.BtnAngleRepr.Text = RootEvaluator.AngleMode.ToString()
                        FrmEditor.EvaluateExpr(True)
                    ElseIf e.KeyCode = Keys.O Then
                        FrmEditor.BtnOutputFormat.PerformClick()
                    ElseIf e.KeyCode = Keys.M OrElse e.KeyCode = Keys.S OrElse e.KeyCode = Keys.W Then
                        If e.KeyCode = Keys.M Then
                            RootEvaluator.OutputMode = OutputFormat.Math
                            e.SuppressKeyPress = True
                        ElseIf e.KeyCode = Keys.W Then
                            RootEvaluator.OutputMode = OutputFormat.Raw
                        Else
                            RootEvaluator.OutputMode = OutputFormat.Scientific
                        End If
                        FrmEditor.BtnOutputFormat.Text = RootEvaluator.OutputMode.ToString()
                        FrmEditor.EvaluateExpr(True)
                    End If
                Else
                    If e.KeyCode = Keys.G Then
                        BtnGraph.PerformClick()
                    ElseIf e.KeyCode = Keys.C Then
                        BtnConsole.PerformClick()
                    ElseIf e.KeyCode = Keys.E Then
                        FrmEditor.BringToFront()
                    ElseIf e.KeyCode = Keys.S Then
                        FrmEditor.BtnSettings.PerformClick()
                    ElseIf e.KeyCode = Keys.F Then
                        FrmEditor.BtnFunctions.PerformClick()
                    ElseIf e.KeyCode = Keys.T Then
                        FrmEditor.BtnTranslucent.PerformClick()
                        DirectCast(sender, Control).Focus()
                    ElseIf e.KeyCode = keys.K Then
                        FrmEditor.BtnKeyboard.PerformClick()
                        DirectCast(sender, Control).Focus()

                    End If
                End If
            ElseIf e.KeyCode = Keys.F5 Then
                If e.Control Then
                    FrmEditor.OpenRunScript()
                Else
                    FrmEditor.BtnEval.PerformClick()
                End If
            ElseIf e.KeyCode = Keys.F6 Then
                FrmEditor.OpenImportScript()
            ElseIf e.KeyCode = Keys.F11 OrElse e.KeyCode = Keys.O AndAlso e.Control Then
                FrmEditor.BtnOpen.PerformClick()
            ElseIf e.KeyCode = Keys.F12 OrElse e.KeyCode = Keys.S AndAlso e.Control Then
                FrmEditor.OpenSaveAs()
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
            ElseIf e.KeyCode = Keys.Up AndAlso Not ConsoleControl.AutoCActive Then
                Dim lastline As Line = ConsoleControl.Lines(ConsoleControl.Lines.Count - 1)
                If ConsoleControl.SelectionStart < lastline.Position Then Exit Sub
                If _idx > 0 Then
                    ConsoleControl.DeleteRange(lastline.Position + _promptText.Length, lastline.Length)
                    ConsoleControl.AppendText(_prevPrompt(_idx - 1))
                    _idx -= 1
                End If
                ConsoleControl.SelectionStart = ConsoleControl.TextLength
            ElseIf e.KeyCode = Keys.Down AndAlso Not ConsoleControl.AutoCActive Then
                Dim lastline As Line = ConsoleControl.Lines(ConsoleControl.Lines.Count - 1)
                If ConsoleControl.SelectionStart < lastline.Position Then Exit Sub
                If _idx < _prevPrompt.Count Then
                    ConsoleControl.DeleteRange(lastline.Position + _promptText.Length, lastline.Length)
                    If (_idx + 1 < _prevPrompt.Count) Then ConsoleControl.AppendText(_prevPrompt(_idx + 1))
                    _idx += 1
                End If
                ConsoleControl.SelectionStart = ConsoleControl.TextLength

            ElseIf e.KeyCode = Keys.Enter Then
                Dim lastLineText As String = ConsoleControl.Lines(_line - 1).Text
                For i As Integer = _line To ConsoleControl.Lines.Count - 1
                    lastLineText += ConsoleControl.Lines(i).Text
                Next

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
                                _idx += 2
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
                                                             Dim res As String =
                                                                RootEvaluator.Eval(lastLineText)
                                                             ConsoleControl.BeginInvoke(
                                                             Sub()
                                                                 WriteConsoleLine(res)
                                                             End Sub)
                                                         Catch ex As Exception
                                                             ConsoleControl.BeginInvoke(
                                                             Sub()
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
    End Class
End Namespace