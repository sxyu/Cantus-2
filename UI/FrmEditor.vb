Imports System.Reflection
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Threading

Imports Cantus.Core
Imports Cantus.Core.CommonTypes
Imports Cantus.Core.CantusEvaluator
Imports Cantus.Core.CantusEvaluator.IOEventArgs
Imports Cantus.Core.CantusEvaluator.ObjectTypes
Imports Cantus.UI.ScintillaForCantus

Imports Cantus.Core.Scoping

Imports ScintillaNET
Imports Cantus.UI.Keyboards

Namespace UI
    Public Class FrmEditor
#Region "Declarations"
        '' <summary>
        '' Maximum length of text to display on the tooltip
        '' </summary>
        Private Const TT_LEN_LIMIT As Integer = 500

        '' <summary>
        '' Max number of previous expressions to store
        '' </summary>
        Private Const PREV_EXPRESSION_LIMIT As Integer = 15

        '' <summary>
        '' URL to get info about the latest version number of cantus
        '' </summary>
        Private Const VERSION_URL As String = "https://raw.githubusercontent.com/sxyu/Cantus-Core/master/meta/ver"

        '' <summary>
        '' The main evaluator
        '' </summary>
        Private _eval As Core.CantusEvaluator

        '' <summary>
        '' The path to the file currently open in the editor. 
        '' If no file is open, stores an empty string.
        '' </summary>
        Public ReadOnly Property File As String = ""

        ' expression memory (up/down arrow keys)
        Private _prevExp As New List(Of String)
        Private _lastExp As String = ""
        Private _curExpId As Integer = 0

        ' update checking thread
        Private _updTh As Thread

        '' <summary>
        ''  if true, allows user to press enter in text box
        '' </summary>
        Private _allowEnter As Boolean = False

        '' <summary>
        '' If true, displays the update message after load
        '' </summary>
        Private _displayUpdateMessage As Boolean = False

        '' <summary>
        '' Custom lexer for scintilla
        '' </summary>
        Private _cantusLexer As New CantusLexer()

        '' <summary>
        '' Counter used for lazy evaluation of expressions
        '' </summary>
        Private _editCt As Integer = 0

#End Region
#Region "Form Events"
        Private Sub FrmEditor_Load(sender As Object, e As EventArgs) Handles MyBase.Load
            SplashScreen.BringToFront()
            TmrLoad.Start()

            ' new version? upgrade settings from previous version + show message
            If My.Settings.NewInstall Then
                My.Settings.Upgrade()
                My.Settings.NewInstall = False
                _displayUpdateMessage = True
            End If

            ' icon
            Me.Icon = My.Resources.Cantus

            ' process additional command line args involving UI
            Dim args As String() = Environment.GetCommandLineArgs()
            Dim def As String = ""
            Dim opengraph As Boolean = False

            For i As Integer = 1 To args.Length - 1
                Dim s As String = args(i)
                If s = "-g" OrElse s = "--graphing" Then
                    opengraph = True
                ElseIf (s = "-d" OrElse s = "--default") AndAlso i < args.Length - 1 Then
                    i += 1
                    def = args(i)
                ElseIf IO.File.Exists(s) Then
                    OpenFile(s)
                End If
            Next

            def = def.Trim().Trim({ControlChars.Quote, "'"c})
            If opengraph Then
                Viewer.View = Viewer.ViewType.graphing
                Viewer.GraphingControl.tb.Text = def
            ElseIf Not def = "" Then
                Tb.Text = def
            End If

            Try
                ' delete updater backups if found
                If FileIO.FileSystem.FileExists(Application.StartupPath & " \cantus.backup") Then
                    IO.File.Delete(Application.StartupPath & "\cantus.backup")
                End If

                If FileIO.FileSystem.FileExists(Application.StartupPath & " \calculator.backup") Then
                    IO.File.Delete(Application.StartupPath & "\calculator.backup")
                End If

                If IO.Path.GetFileName(Application.ExecutablePath).ToLower() = "calculator.exe" Then
                    Try
                        ' update from legacy CalculatorX: Clear state
                        My.Settings.Save()
                        FileSystem.Rename(Application.ExecutablePath, "cantus.exe")
                        Process.Start(IO.Path.Combine("cantus.exe"))
                        Me.Close()
                        Exit Sub
                    Catch
                    End Try
                End If
            Catch
            End Try

            ' update tooltips
            UpdateLetterTT()

            ' set up UI
            CbAutoUpd.Checked = My.Settings.AutoUpdate
            LbAbout.Text = LbAbout.Text.Replace("{VER}", Version)

            ' faster drawing
            Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)

            _eval = Globals.RootEvaluator
            ' set up modes
            If _eval.OutputMode = OutputFormat.Math Then
                BtnOutputFormat.Text = "MathO"
            ElseIf _eval.OutputMode = OutputFormat.Scientific Then
                BtnOutputFormat.Text = "SciO"
            Else
                BtnOutputFormat.Text = "LineO"
            End If
            If _eval.AngleMode = AngleRepresentation.Radian Then
                BtnAngleRepr.Text = "Radian"
            ElseIf _eval.AngleMode = AngleRepresentation.Degree Then
                BtnAngleRepr.Text = "Degree"
            Else
                BtnAngleRepr.Text = "Gradian"
            End If

            ' attach events to evaluator
            AddHandler RootEvaluator.EvalComplete, AddressOf EvalComplete
            AddHandler RootEvaluator.WriteOutput, AddressOf WriteOutput
            AddHandler RootEvaluator.ReadInput, AddressOf ReadInput
            AddHandler RootEvaluator.ClearConsole, AddressOf ClearConsole

            ' set up scintilla
            SetTheme("dark")

            Me.PnlResults.BringToFront()
            My.Settings.Save()
        End Sub

        ' form events
        Private Sub FrmEditor_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
            Try
                SaveSettings()
                _eval.Dispose()
                Thread.Sleep(250)
            Catch
            End Try
            SplashScreen.Close()
        End Sub

        Dim deactivated As Boolean = False
        Private Sub FrmEditor_Deactivate(sender As Object, e As EventArgs) Handles MyBase.Deactivate
            ShowSettings(False)
        End Sub

        Dim _ignoreNextKey As Boolean = False
        Private Sub FrmEditor_KeyUp(sender As Object, e As KeyEventArgs) Handles MyBase.KeyUp, Tb.KeyUp,
            BtnAngleRepr.KeyUp, PnlSettings.KeyUp, Viewer.KeyUp
            If e.KeyCode = Keys.Enter And e.Alt Then
                If sender Is Me Then Return
                EvaluateExpr()
            ElseIf e.Control AndAlso e.Alt Then
                If e.KeyCode = Keys.P Then
                    btnAngleRep_Click(BtnAngleRepr, New EventArgs)
                ElseIf e.KeyCode = Keys.D OrElse e.KeyCode = Keys.R OrElse e.KeyCode = Keys.G Then
                    If e.KeyCode = Keys.D Then
                        _eval.AngleMode = AngleRepresentation.Degree
                    ElseIf e.KeyCode = Keys.R
                        _eval.AngleMode = AngleRepresentation.Radian
                    Else
                        _eval.AngleMode = AngleRepresentation.Gradian
                    End If
                    BtnAngleRepr.Text = _eval.AngleMode.ToString()
                    EvaluateExpr(True)
                ElseIf e.KeyCode = Keys.O Then
                    btnOutputFormat_Click(BtnOutputFormat, New EventArgs)
                ElseIf e.KeyCode = Keys.M OrElse e.KeyCode = Keys.S OrElse e.KeyCode = Keys.W Then
                    If e.KeyCode = Keys.M Then
                        _eval.OutputMode = OutputFormat.Math
                        e.SuppressKeyPress = True
                    ElseIf e.KeyCode = Keys.W
                        _eval.OutputMode = OutputFormat.Raw
                    Else
                        _eval.OutputMode = OutputFormat.Scientific
                    End If
                    BtnOutputFormat.Text = _eval.OutputMode.ToString()
                    EvaluateExpr(True)
                End If
            End If
        End Sub

        Private Sub FrmEditor_KeyPress(sender As Object, e As KeyPressEventArgs) Handles Tb.KeyPress
            If _ignoreNextKey Then
                _ignoreNextKey = False
                e.Handled = True
            End If
        End Sub
#End Region
#Region "Shared functions"
        '' <summary>
        '' Send the event to asynchronously evaluate the expression
        '' </summary>
        Friend Sub EvaluateExpr(Optional noSaveAns As Boolean = False,
                                Optional text As String = "",
                                Optional file As String = "")
            If Not _eval Is Nothing Then _eval.Dispose()
            Dim fromTb As Boolean = False
            If text = "" Then
                text = Tb.Text
                fromTb = True
            End If
            If file = "" Then file = Me.File

            ' set up evaluator
            Me._eval = New CantusEvaluator(scope:=If(String.IsNullOrWhiteSpace(file), "cantus",
                                           Scoping.GetFileScopeName(file)), reloadDefault:=False)
            Me._eval.ReloadDefault()
            Dim cantusPath As String = IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) & IO.Path.DirectorySeparatorChar

            If String.IsNullOrWhiteSpace(file) OrElse Not IO.Path.GetFullPath(file).StartsWith(cantusPath + "plugin") Then
                Try
                    Me._eval.ReInitialize()
                Catch ex As Exception
                    MsgBox(ex.Message, MsgBoxStyle.Exclamation Or MsgBoxStyle.ApplicationModal, "Reinitialization Error")
                End Try
            End If

            AddHandler _eval.EvalComplete, AddressOf EvalComplete
            AddHandler _eval.WriteOutput, AddressOf WriteOutput
            AddHandler _eval.ReadInput, AddressOf ReadInput
            AddHandler _eval.ClearConsole, AddressOf ClearConsole

            _eval.EvalAsync(text, noSaveAns) ' evaluate & wait for event

            If Not noSaveAns Then
                ' save previous expressions 
                If _prevExp.Count = 0 OrElse _prevExp(_prevExp.Count - 1) <> text Then
                    _prevExp.Add(text)
                    _curExpId += 1
                End If

                ' remove previous expressions past limit
                _lastExp = text
                If _prevExp.Count > PREV_EXPRESSION_LIMIT Then
                    _prevExp.RemoveAt(0) 'max expressions
                    _curExpId -= 1
                End If

                Dim logText As String = ""
                If fromTb Then
                    If Tb.Lines.Count > 4 Then
                        logText = Tb.GetTextRange(0, Tb.Lines(4).Position) & "..."
                    Else
                        If Tb.Lines.Count > 1 Then logText = vbLf
                        logText += Tb.Text
                    End If
                End If

                If New InternalFunctions(Nothing).Count(logText, """""""") Mod 2 = 1 Then logText &= """"""""
                If New InternalFunctions(Nothing).Count(logText, "'''") Mod 2 = 1 Then logText &= "'''"

                While New InternalFunctions(Nothing).Count(logText, """") Mod 2 = 1
                    logText &= """"
                End While
                While New InternalFunctions(Nothing).Count(logText, "'") Mod 2 = 1
                    logText &= "'"
                End While

                logText &= vbLf

                ' write expression to console
                Viewer.WriteConsoleSection(logText)
            End If
        End Sub

        Private Sub EvalComplete(sender As Object, e As AnswerEventArgs)
            If LbResult.InvokeRequired Then
                LbResult.BeginInvoke(Sub() EvalComplete(sender, e))
            Else
                ' display & print answer
                LbResult.Text = AutoTrimDisplayText(e.ResultString)
                If Not e.NoSaveAns Then
                    Viewer.WriteConsoleLine("Result: " & vbLf & e.ResultString)
                    Viewer.WriteLogSeparator()
                    Tb.Focus()
                End If
            End If
        End Sub

        Private Function AutoTrimDisplayText(txt As String) As String
            Dim g As Graphics = Graphics.FromHwnd(Me.Handle)
            Dim i As Integer = 0
            Dim res As String = "= "
            Dim wid As Single = g.MeasureString(res, LbResult.Font).Width
            While i < txt.Length AndAlso wid < LbResult.Width - 1
                res &= txt(i)
                i += 1
                wid = g.MeasureString(res, LbResult.Font).Width
            End While

            If wid > LbResult.Width - 1 Then
                While wid > LbResult.Width - 1 AndAlso res.Length > 0
                    res = res.Remove(res.Length - 1)
                    wid = g.MeasureString(res & "...", LbResult.Font).Width
                End While
                res &= "..."
                If txt.Length <= TT_LEN_LIMIT Then
                    TTLetters.SetToolTip(LbResult, txt)
                Else
                    TTLetters.SetToolTip(LbResult, txt.Remove(TT_LEN_LIMIT - 3) & "...")
                End If
            Else
                TTLetters.SetToolTip(LbResult, "")
            End If

            Return res
        End Function

        Private Sub WriteOutput(sender As Object, e As IOEventArgs)
            If Me.InvokeRequired Then
                Me.BeginInvoke(Sub() WriteOutput(sender, e))
            Else
                Viewer.WriteConsole(e.Content)
            End If
        End Sub

        Private _ev As New ManualResetEventSlim(False)

        Private Sub ReadInput(sender As Object, e As IOEventArgs, ByRef [return] As Object)
            Dim ret As String = ""
            Dim method As Viewer.InputReadEventHandler = Sub(senderR As Object, eR As Viewer.InputEventArgs)
                                                             ret = eR.Text
                                                             _ev.Set()
                                                         End Sub
            AddHandler Viewer.InputRead, method
            If e.Message = IOMessage.readLine Then
                Viewer.ReadLine()
            ElseIf e.Message = IOMessage.readChar
                Viewer.ReadChar()
            ElseIf e.Message = IOMessage.readWord
                Viewer.ReadWord()
            Else
                If e.Args("yes").ToString() = "yes" Then
                    Viewer.WriteConsoleLine("Please enter 'Y', 'N', 'yes', or 'no'")
                Else
                    Viewer.WriteConsoleLine("Please enter 'Y', 'N', 'ok', or 'cancel'")
                End If
                Viewer.ReadWord()
            End If

            _ev.Wait()
            _ev.Reset()
            RemoveHandler Viewer.InputRead, method

            If e.Message = IOMessage.confirm Then
                If e.Args("yes").ToString() = "yes" Then
                    ret = ret.ToLowerInvariant().Trim()
                    If ret = "yes" OrElse ret = "y" Then
                        [return] = True
                    ElseIf ret = "no" OrElse ret = "n" Then
                        [return] = False
                    Else
                        ' ask again
                        ReadInput(sender, e, [return])
                    End If
                Else
                    ret = ret.ToLowerInvariant().Trim()
                    If ret = "ok" OrElse ret = "y" Then
                        [return] = True
                    ElseIf ret = "cancel" OrElse ret = "n" Then
                        [return] = False
                    Else
                        ' ask again
                        ReadInput(sender, e, [return])
                    End If
                End If
            Else
                [return] = ret
            End If
        End Sub

        Private Sub ClearConsole(sender As Object, e As EventArgs)
            Viewer.ClearConsole()
        End Sub

        Public Sub SetTheme(name As String)
            Tb.StyleResetDefault()

            Select Case name
                Case "dark"
                    With Tb.Styles(Style.Default)
                        .BackColor = Color.FromArgb(34, 34, 34)
                        .Font = "Consolas"
                        .Size = 13
                    End With

                    Tb.SetSelectionBackColor(True, Color.GhostWhite)
                    Tb.SetSelectionForeColor(True, Color.Black)

                    Tb.StyleClearAll()
                    Tb.WrapMode = WrapMode.Word

                    Tb.Styles(CantusLexer.StyleDefault).ForeColor = Color.LightGray

                    Tb.Styles(CantusLexer.StyleKeyword).ForeColor = Color.FromArgb(147, 199, 99)
                    Tb.Styles(CantusLexer.StyleInlineKeyword).ForeColor = Color.FromArgb(103, 140, 177)

                    Tb.Styles(CantusLexer.StyleIdentifier).ForeColor = Color.FromArgb(241, 242, 243)
                    Tb.Styles(CantusLexer.StyleError).ForeColor = Color.LightGray

                    Tb.Styles(CantusLexer.StyleNumberBoolean).ForeColor = Color.FromArgb(255, 205, 34)
                    Tb.Styles(CantusLexer.StyleString).ForeColor = Color.FromArgb(236, 118, 0)

                    Tb.Styles(CantusLexer.StyleComment).ForeColor = Color.FromArgb(153, 163, 138)

                    Tb.Lexer = Lexer.Container

                    Tb.IndentWidth = 4

                    With Tb.Styles(Style.LineNumber)
                        .BackColor = Color.FromArgb(34, 34, 34)
                        .ForeColor = Color.DarkGray
                        .Size = 13
                    End With

                    Tb.IndentationGuides = IndentView.Real

                    Tb.Styles(Style.BraceLight).ForeColor = Color.BlueViolet
                    Tb.Styles(Style.BraceLight).BackColor = Color.LightGray

                    Tb.Styles(Style.BraceBad).ForeColor = Color.White
                    Tb.Styles(Style.BraceBad).BackColor = Color.IndianRed

                    Tb.Styles(Style.IndentGuide).ForeColor = Color.Gray

                    Dim margin As Margin = Tb.Margins(0)
                    margin.Width = 45

                    Tb.TabWidth = 4

                    Tb.ScrollWidth = Tb.Width - 2 * margin.Width - 5

                    Tb.AutoCIgnoreCase = True
            End Select
        End Sub

        '' <summary>
        '' Save all settings to the init.can file
        '' </summary>
        Public Sub SaveSettings()
            Try
                My.Settings.Position = String.Join(",", DirectCast({Me.Left, Me.Top, Me.Width, Me.Height,
                                                   Me.WindowState, Split.SplitterDistance}, Object()))
                My.Settings.Save()

                Dim curText As String = ""
                Dim cantusPath As String = IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) & IO.Path.DirectorySeparatorChar
                If IO.File.Exists(cantusPath + "init.can") Then
                    curText = IO.File.ReadAllText(cantusPath + "init.can")

                    ' cut everything up to the end comment
                    Dim endComment As String = "# end of cantus auto-generated initialization script." & " do not modify this comment."
                    If curText.ToLower().Contains(endComment) Then
                        curText = curText.Substring(curText.ToLower().LastIndexOf(endComment) + endComment.Length +
                                                    ControlChars.NewLine.Length +
                                                     "# You may write additional initialization code below this line.".Length)
                    End If
                End If
                curText = curText.TrimEnd() & vbNewLine

                ' try writing to init.can
                IO.File.WriteAllText(cantusPath + "init.can", _eval.ToScript() & curText)
            Catch 'ex As Exception
            End Try

            My.Settings.Save()
        End Sub

        ''' <summary>
        ''' Open a 'save as' dialogue to save as another file
        ''' </summary>
        Friend Sub OpenSaveAs()
            Using diag As New SaveFileDialog()
                diag.Filter = "Cantus Script (.can)|*.can|Any File (*)|*"
                diag.RestoreDirectory = True
                diag.Title = "Save As Script"
                If diag.ShowDialog = DialogResult.OK Then
                    IO.File.WriteAllText(diag.FileName, Tb.Text, Encoding.UTF8)
                    _File = diag.FileName ' set new file name
                    _eval.Scope = GetFileScopeName(_File)
                End If
            End Using
        End Sub

        ''' <summary>
        ''' Open a dialog to select a script to run
        ''' </summary>
        Friend Sub OpenRunScript()
            Using diag As New OpenFileDialog()
                diag.Filter = "Cantus Script (.can)|*.can"
                diag.RestoreDirectory = True
                diag.Multiselect = False
                diag.Title = "Run Script"
                If diag.ShowDialog = DialogResult.OK Then
                    EvaluateExpr(False, IO.File.ReadAllText(diag.FileName), diag.FileName)
                End If
            End Using
        End Sub

        ''' <summary>
        ''' Open a dialog to select a script to import
        ''' </summary>
        Friend Sub OpenImportScript()
            Using diag As New OpenFileDialog()
                diag.Filter = "Cantus Script (.can)|*.can"
                diag.RestoreDirectory = True
                diag.Multiselect = False
                diag.Title = "Import Script"
                If diag.ShowDialog = DialogResult.OK Then
                    _eval.Load(diag.FileName, False, True)
                End If
            End Using
        End Sub

        ''' <summary>
        ''' Open a file at the specified path
        ''' </summary>
        ''' <param name="path"></param>
        Private Sub OpenFile(path As String)
            Tb.Text = IO.File.ReadAllText(path).Replace(vbCrLf, vbLf).Replace(vbCr, vbLf).
                        Replace(vbLf, vbNewLine) ' fix line endings
            _File = path
        End Sub


        '' <summary>
        '' Open the 'open script' dialog to open a script
        '' </summary>
        Private Sub Open()
            Using diag As New OpenFileDialog()
                diag.Filter = "Cantus Script (.can)|*.can"
                diag.RestoreDirectory = True
                diag.Multiselect = False
                diag.Title = "Open Script"
                If diag.ShowDialog = DialogResult.OK Then
                    OpenFile(diag.FileName)
                End If
            End Using
        End Sub

        '' <summary>
        '' Save the file; if the editor is not linked to any file, opens the save as dialogue.
        '' </summary>
        Private Sub Save()
            If String.IsNullOrEmpty(Me.File) Then
                OpenSaveAs()
            Else
                IO.File.WriteAllText(File, Tb.Text, Encoding.UTF8)
            End If
        End Sub

        '' <summary>
        '' Create and edit a new, empty script, closing the currently opened script
        '' </summary>
        Private Sub NewScript()
            If String.IsNullOrEmpty(Me.File) Then
                If (Tb.TextLength < 10 AndAlso String.IsNullOrWhiteSpace(Tb.Text)) OrElse
                           MsgBox("All unsaved changes will be lost. " & ControlChars.NewLine &
                                  "Are you sure you want to create a new script?",
                           MsgBoxStyle.YesNo Or MsgBoxStyle.MsgBoxSetForeground Or MsgBoxStyle.Exclamation, "New Script") =
                           MsgBoxResult.Yes Then
                    Tb.Text = ""
                    _eval.Scope = ROOT_NAMESPACE
                End If
            Else
                Save()
                Me._File = ""
                Tb.Text = ""
            End If
        End Sub
#End Region
#Region "main buttons"
        Private Sub btnCalc_Click(sender As Object, e As System.EventArgs) Handles BtnEval.Click
            Tb.Focus()
            EvaluateExpr()
        End Sub

        Private Sub btnFunctions_Click(sender As Object, e As EventArgs) Handles BtnFunctions.Click
            Tb.Focus()
            Using diag As New Dialogs.DiagFunctions
                If diag.ShowDialog() = DialogResult.OK Then
                    Dim start As Integer = Tb.SelectionStart
                    Tb.Text = Tb.Text.Remove(
                                Tb.SelectionStart, Tb.SelectionEnd - Tb.SelectionStart).Insert(start, diag.Result)

                    If diag.Result.Contains("(") Then
                        Tb.SelectionStart = start + diag.Result.IndexOf("(") + 1
                    Else
                        Tb.SelectionStart = start + diag.Result.Count
                    End If
                End If
            End Using
        End Sub

        Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles BtnSave.Click
            Tb.Focus()
            Save()
            SaveSettings()
        End Sub

        Private Sub btnOpen_Click(sender As Object, e As EventArgs) Handles BtnOpen.Click
            Tb.Focus()
            Open()
        End Sub

        Private Sub BtnTranslucent_Click(sender As Object, e As EventArgs) Handles BtnTranslucent.Click
            Tb.Focus()
            If Me.Opacity = 1 Then
                Me.Opacity = 0.75
                BtnTranslucent.BackColor = Color.FromArgb(45, 45, 45)
                BtnTranslucent.FlatAppearance.MouseDownBackColor = Color.FromArgb(55, 55, 55)
            ElseIf Me.Opacity = 0.75
                Me.Opacity = 0.25
                BtnTranslucent.BackColor = Color.FromArgb(30, 30, 30)
                BtnTranslucent.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 40, 40)
            Else
                Me.Opacity = 1
                BtnTranslucent.BackColor = BtnMin.BackColor
                BtnTranslucent.FlatAppearance.MouseDownBackColor = BtnMin.FlatAppearance.MouseDownBackColor
            End If
            BtnTranslucent.FlatAppearance.MouseOverBackColor = BtnTranslucent.BackColor
            'Viewer.Opacity = Me.Opacity
        End Sub

        Private Sub btnNew_Click(sender As Object, e As EventArgs) Handles BtnNew.Click
            Tb.Focus()
            NewScript()
        End Sub

#Region "Textbox & Labels"

        Private Sub Tb_KeyDown(sender As Object, e As KeyEventArgs) Handles Tb.KeyDown
            If e.Alt AndAlso Not e.Control Then
                If e.KeyCode = Keys.Up Then
                    If _curExpId > 1 And _prevExp.Count > 1 Then
                        If (Tb.Text = _lastExp) Then
                            _curExpId -= 1
                        Else
                            _prevExp.Add(Tb.Text)
                        End If
                        _lastExp = _prevExp(_curExpId - 1)
                        Tb.Text = _lastExp
                        Tb.SelectionStart = Tb.Text.Length
                    End If
                ElseIf e.KeyCode = Keys.Down Then
                    If _curExpId < _prevExp.Count And _prevExp.Count > 1 Then
                        _curExpId += 1
                        _lastExp = _prevExp(_curExpId - 1)
                        Tb.Text = _lastExp
                        Tb.SelectionStart = Tb.Text.Length
                    End If
                ElseIf e.KeyCode = Keys.F Then
                    BtnFunctions.PerformClick()
                ElseIf e.KeyCode = Keys.S Then
                    BtnSettings.PerformClick()
                ElseIf e.KeyCode = Keys.G Then
                    Viewer.View = Viewer.ViewType.graphing
                    Try
                        Viewer.pnl.Controls(0).Select()
                    Catch
                    End Try
                ElseIf e.KeyCode = Keys.C Then
                    Viewer.View = Viewer.ViewType.console
                    Try
                        Viewer.pnl.Controls(0).Select()
                    Catch
                    End Try
                ElseIf e.KeyCode = Keys.T Then
                    BtnTranslucent.PerformClick()
                ElseIf e.KeyCode = Keys.K
                    BtnKeyboard.PerformClick()
                End If
            ElseIf e.KeyCode = Keys.F12
                OpenSaveAs()
            ElseIf e.KeyCode = Keys.S AndAlso e.Control AndAlso Not e.Alt
                _ignoreNextKey = True
                Save()
            ElseIf e.KeyCode = Keys.N AndAlso e.Control AndAlso Not e.Alt
                _ignoreNextKey = True
                NewScript()
            ElseIf e.KeyCode = Keys.F11 OrElse e.KeyCode = Keys.O AndAlso e.Control AndAlso Not e.Alt
                _ignoreNextKey = True
                Open()

            ElseIf e.KeyCode = Keys.F6
                OpenImportScript()

            ElseIf e.KeyCode = Keys.F5
                If e.Control Then
                    OpenRunScript()
                Else
                    BtnEval.PerformClick()
                End If
            ElseIf e.Control AndAlso e.Alt
                If e.KeyCode = Keys.T Then
                    BtnExplicit.PerformClick()
                ElseIf e.KeyCode = Keys.F Then
                    BtnSigFigs.PerformClick()
                End If
            End If
        End Sub
#End Region
#End Region
#Region "Settings"
        Friend Sub ShowSettings(Optional show As Boolean = True)
            If TmrAnim.Enabled Then Return
            If show Then
                PnlSettings.Show()
                PnlSettings.Left = Tb.Right
            End If
            _showSettings = show
            PnlSettings.BringToFront()
            _slideRate = 50
            TmrAnim.Start()
        End Sub

        Dim _showSettings As Boolean = True
        Dim _slideRate As Double
        Private Sub TmrAnim_tick(sender As Object, e As EventArgs) Handles TmrAnim.Tick
            If _showSettings Then
                If PnlSettings.Left - _slideRate <= 0 Then
                    TmrAnim.Stop()
                    PnlSettings.Left = 0

                    UpdateLetterTT()
                    BtnSettings.BackColor = Color.FromArgb(60, 60, 60)
                    BtnSettings.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 60, 60)
                    Exit Sub
                End If
                PnlSettings.Left -= CInt(_slideRate)
                _slideRate *= 1.2
            Else
                If PnlSettings.Left + _slideRate > PnlTb.Width Then
                    TmrAnim.Stop()
                    PnlSettings.Left = PnlTb.Width + 1
                    PnlSettings.Hide()

                    BtnSettings.BackColor = Color.FromArgb(55, 55, 55)
                    BtnSettings.FlatAppearance.MouseOverBackColor = Color.FromArgb(55, 55, 55)
                End If
                PnlSettings.Left += CInt(_slideRate)
                _slideRate *= 1.2
            End If
        End Sub

        Private Sub btnSettings_Click(sender As Object, e As EventArgs) Handles BtnSettings.Click
            If PnlSettings.Left > 0 Then
                PnlTb.Focus()
                BtnAngleRepr.Text = _eval.AngleMode.ToString()
                BtnOutputFormat.Text = _eval.OutputMode.ToString()
                If _eval.ExplicitMode Then
                    BtnExplicit.BackColor = BtnEval.BackColor
                    BtnExplicit.FlatAppearance.MouseDownBackColor = BtnEval.FlatAppearance.MouseDownBackColor
                Else
                    BtnExplicit.BackColor = BtnX.BackColor
                    BtnExplicit.FlatAppearance.MouseDownBackColor = BtnX.FlatAppearance.MouseDownBackColor
                End If
                BtnExplicit.FlatAppearance.MouseOverBackColor = BtnExplicit.BackColor
                If _eval.SignificantMode Then
                    BtnSigFigs.BackColor = BtnEval.BackColor
                    BtnSigFigs.FlatAppearance.MouseDownBackColor = BtnEval.FlatAppearance.MouseDownBackColor
                Else
                    BtnSigFigs.BackColor = BtnX.BackColor
                    BtnSigFigs.FlatAppearance.MouseDownBackColor = BtnX.FlatAppearance.MouseDownBackColor
                End If
                BtnSigFigs.FlatAppearance.MouseOverBackColor = BtnSigFigs.BackColor
                ShowSettings()
            Else
                EvaluateExpr(True)
                ShowSettings(False)
            End If
        End Sub

        Private Sub btnLetters_Click(sender As Object, e As EventArgs) Handles BtnY.Click, BtnX.Click
            Dim btn As Button = DirectCast(sender, Button)
            SetVariable(btn.Tag.ToString()(0), _eval.GetLastAns())
            UpdateLetterTT()
        End Sub

        Private Sub SetVariable(varnm As Char, data As Object)
            _eval.SetVariable(varnm, ObjectTypes.DetectType(data))
        End Sub

        '' <summary>
        '' Update tooltips for variable buttons
        '' </summary>
        Private Sub UpdateLetterTT()
            For Each c As Control In PnlSettings.Controls
                If c.Tag Is Nothing OrElse c.Tag.ToString().StartsWith("-") Then Continue For

                Dim val As String
                Try
                    val = _eval.GetVariableRepr(c.Text.Remove(0, 1)(0))
                Catch
                    val = "Undefined"
                End Try

                TTLetters.SetToolTip(c, c.Text.Remove(0, 1) & " = " & val)
            Next
        End Sub
#End Region
#Region "Update"
        Private _updateStarted As Boolean = False
        ''' <summary>
        ''' Check for updates
        ''' </summary>
        ''' <param name="promptuser">If true, shows a message to the user when done, even if no update is found.</param>
        Private Sub CheckUpdate(Optional ByVal promptuser As Boolean = False)
            If _updateStarted Then
                If (Me.InvokeRequired) Then
                    Me.BeginInvoke(Sub()
                                       MsgBox("Please wait, we're already checking for updates.",
                                              MsgBoxStyle.Exclamation, "Checking For Updates")
                                   End Sub)
                Else
                    MsgBox("Please wait, we're already checking for updates.", MsgBoxStyle.Exclamation, "Checking For Updates")
                End If
                Exit Sub
            End If
            _updateStarted = True
            Try
                Dim newVer As String = ""
                Using wc As New System.Net.WebClient()
                    newVer = wc.DownloadString(VERSION_URL).Trim()
                End Using
                Dim nv As String = newVer
                If nv.Contains(" ") Then nv = nv.Remove(nv.IndexOf(" "))
                Dim spl() As String = nv.Split("."c)
                Dim curVer As String = Version
                If curVer.Contains(" ") Then curVer = curVer.Remove(curVer.IndexOf(" "))
                Dim curverspl() As String = curVer.Split("."c)
                For i As Integer = 0 To spl.Length - 1
                    If CInt(spl(i)) > CInt(curverspl(i)) Then
                        Using upd As New Updater.DiagUpdateAvailable(newVer)
                            If upd.ShowDialog() = DialogResult.OK Then
                                Exit For ' needs update
                            Else
                                _updateStarted = False
                                Exit Sub ' does not need update
                            End If
                        End Using
                    ElseIf CInt(spl(i)) < CInt(curverspl(i)) OrElse i = spl.Length - 1 Then
                        If promptuser Then MessageBox.Show(Me, "You are running the latest version of Cantus.", "No Update Found",
                              MessageBoxButtons.OK, MessageBoxIcon.Information)
                        _updateStarted = False
                        Exit Sub ' don't update
                    End If
                Next

                Try
                    If FileIO.FileSystem.FileExists(Application.StartupPath & "\cantus.backup") Then
                        FileIO.FileSystem.DeleteFile(Application.StartupPath & "\cantus.backup")
                    End If
                    If FileIO.FileSystem.FileExists(Application.StartupPath & "\cantus.core.backup") Then
                        FileIO.FileSystem.DeleteFile(Application.StartupPath & "\calculator.backup")
                    End If
                    If FileIO.FileSystem.FileExists(Application.StartupPath & "\calculator.backup") Then
                        FileIO.FileSystem.DeleteFile(Application.StartupPath & "\calculator.backup")
                    End If
                Catch 'ex2 As Exception
                End Try

                If (Me.InvokeRequired) Then
                    Me.BeginInvoke(Sub()
                                       ShowUpdForm()
                                   End Sub)
                Else
                    ShowUpdForm()
                End If
            Catch 'ex As Exception
                'ignore
            End Try
            _updateStarted = False
        End Sub
        Private Sub ShowUpdForm()
            Me.Visible = False
            Using fup As New Updater.FrmUpdate
                Updater.FrmUpdate.ShowDialog()
                Updater.FrmUpdate.BringToFront()
            End Using
        End Sub
#End Region
#Region "Command Buttons & Aesthetic"
        Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles BtnClose.Click
            Me.Close()
        End Sub

        Private Sub btnMin_Click(sender As Object, e As EventArgs) Handles BtnMin.Click
            Me.WindowState = FormWindowState.Minimized
        End Sub

        Private Sub btnMax_Click(sender As Object, e As EventArgs) Handles BtnMax.Click
            If Me.WindowState = FormWindowState.Normal Then
                Me.WindowState = FormWindowState.Maximized
            Else
                Me.WindowState = FormWindowState.Normal
            End If
        End Sub

        Private Sub btnMem_Enter(sender As Object, e As EventArgs) Handles BtnSettings.Enter, BtnEval.Enter, BtnMin.Enter, BtnClose.Enter
            Tb.Focus()
        End Sub
#End Region
#Region "Form Moving"
        Private _isMoving As Boolean
        Private _movingPrevPt As Point

        Friend Sub FrmEditor_MouseDown(sender As Object, e As MouseEventArgs) Handles PnlResults.MouseDown, LbResult.MouseDown
            If e.Button <> MouseButtons.Right And Not _isMoving Then
                _movingPrevPt = e.Location
                _isMoving = True
            End If
        End Sub

        Friend Sub FrmEditor_MouseMove(sender As Object, e As MouseEventArgs) Handles PnlResults.MouseMove, LbResult.MouseMove
            If _isMoving Then
                Me.Left = Me.Left + e.X - _movingPrevPt.X
                Me.Top = Me.Top + e.Y - _movingPrevPt.Y
            End If
        End Sub

        Friend Sub FrmEditor_MouseUp(sender As Object, e As MouseEventArgs) Handles PnlResults.MouseUp, LbResult.MouseUp, Tb.MouseUp, PnlTb.MouseUp
            Tb.Focus()
            ShowSettings(False)
            _isMoving = False

            My.Settings.Position = String.Join(",", DirectCast({Me.Left, Me.Top, Me.Width, Me.Height, Me.WindowState,
                                               Split.SplitterDistance}, Object()))
            My.Settings.Save()
        End Sub

        ''' <summary>
        ''' Specifies the side of the window being scaled
        ''' </summary>
        Private Enum ScaleSide
            Top = 1
            Bottom = 2
            Left = 4
            Right = 8
            None = 0
        End Enum
        Dim _scaleSide As ScaleSide = ScaleSide.None
        Friend Sub FrmEditor_scale_MouseDown(sender As Object, e As MouseEventArgs) Handles MyBase.MouseDown
            Dim dist As Integer() = {e.Y, Me.Height - e.Y, e.X, Me.Width - e.X}
            Dim MAX_DIST As Integer = 15
            _scaleSide = ScaleSide.None
            For i As Integer = 0 To dist.Length - 1
                If dist(i) < MAX_DIST Then
                    _scaleSide = _scaleSide Or CType(2 ^ i, ScaleSide)
                End If
            Next
        End Sub

        Friend Sub FrmEditor_scale_MouseMove(sender As Object, e As MouseEventArgs) Handles MyBase.MouseMove
            If _scaleSide = ScaleSide.None Then Return

            If (_scaleSide And ScaleSide.Top) <> 0 Then
                Me.Height = Me.Bottom - Cursor.Position.Y
                Me.Top = Cursor.Position.Y
            End If

            If (_scaleSide And ScaleSide.Bottom) <> 0 Then
                Me.Height = Cursor.Position.Y - Me.Top
            End If

            If (_scaleSide And ScaleSide.Left) <> 0 Then
                Me.Width = Me.Right - Cursor.Position.X
                Me.Left = Cursor.Position.X
            End If

            If (_scaleSide And ScaleSide.Right) <> 0 Then
                Me.Width = Cursor.Position.X - Me.Left
            End If
        End Sub

        Friend Sub FrmEditor_scale_MouseUp(sender As Object, e As MouseEventArgs) Handles MyBase.MouseUp
            _scaleSide = ScaleSide.None
            My.Settings.Position = String.Join(",", DirectCast({Me.Left, Me.Top, Me.Width, Me.Height, Me.WindowState,
                                               Split.SplitterDistance}, Object()))
            My.Settings.Save()
        End Sub

        Private Sub btnOutputFormat_Click(sender As Object, e As EventArgs) Handles BtnOutputFormat.Click
            If _eval.OutputMode = Core.CantusEvaluator.OutputFormat.Scientific Then
                _eval.OutputMode = Core.CantusEvaluator.OutputFormat.Raw
            Else
                _eval.OutputMode = CType(CInt(_eval.OutputMode) + 1, Core.CantusEvaluator.OutputFormat)
            End If
            If _eval.OutputMode = Core.CantusEvaluator.OutputFormat.Math Then
                BtnOutputFormat.Text = "Math"
            ElseIf _eval.OutputMode = Core.CantusEvaluator.OutputFormat.Scientific Then
                BtnOutputFormat.Text = "Scientific"
            Else
                BtnOutputFormat.Text = "Raw"
            End If
            EvaluateExpr(True)
        End Sub

        Private Sub btnAngleRep_Click(sender As Object, e As EventArgs) Handles BtnAngleRepr.Click
            If _eval.AngleMode = Core.CantusEvaluator.AngleRepresentation.Gradian Then
                _eval.AngleMode = Core.CantusEvaluator.AngleRepresentation.Degree
            Else
                _eval.AngleMode = CType(CInt(_eval.AngleMode) + 1, Core.CantusEvaluator.AngleRepresentation)
            End If
            If _eval.AngleMode = Core.CantusEvaluator.AngleRepresentation.Radian Then
                BtnAngleRepr.Text = "Radian"
            ElseIf _eval.AngleMode = Core.CantusEvaluator.AngleRepresentation.Degree Then
                BtnAngleRepr.Text = "Degree"
            Else
                BtnAngleRepr.Text = "Gradian"
            End If
            EvaluateExpr(True)
        End Sub

        Private Sub pnlSettings_Click(sender As Object, e As EventArgs) Handles PnlSettings.Click, LbSettings.Click
            BtnSettings.PerformClick()
        End Sub

        Private Sub lbAbout_Click(sender As Object, e As EventArgs) Handles LbAbout.Click, BtnLog.Click
            Tb.Focus()
            Using diag As New Dialogs.DiagFeatureList()
                diag.ShowDialog()
            End Using
        End Sub

        Private Sub cbAutoUpd_CheckedChanged(sender As Object, e As EventArgs) Handles CbAutoUpd.CheckedChanged
            My.Settings.AutoUpdate = CbAutoUpd.Checked
            My.Settings.Save()
        End Sub

        Private Sub btnUpdate_Click(sender As Object, e As EventArgs) Handles BtnUpdate.Click
            _updTh = New Thread(CType(Sub()
                                          CheckUpdate(True)
                                      End Sub, ThreadStart))
            _updTh.IsBackground = True
            _updTh.Start()
        End Sub

        Private Sub BtnOptions_Click(sender As Object, e As EventArgs) Handles BtnExplicit.Click, BtnSigFigs.Click
            Dim mode As Boolean
            Dim btn As Button = DirectCast(sender, Button)

            If btn.Tag.ToString().EndsWith("E") Then
                _eval.ExplicitMode = Not _eval.ExplicitMode
                mode = _eval.ExplicitMode
            Else
                _eval.SignificantMode = Not _eval.SignificantMode
                mode = _eval.SignificantMode
                If mode AndAlso _eval.OutputMode = OutputFormat.Math Then
                    _eval.OutputMode = OutputFormat.Raw
                    BtnOutputFormat.Text = "Raw"
                End If
            End If

            If mode Then
                btn.BackColor = BtnEval.BackColor
                btn.ForeColor = BtnEval.ForeColor
                btn.FlatAppearance.MouseOverBackColor = BtnEval.FlatAppearance.MouseOverBackColor
                btn.FlatAppearance.MouseDownBackColor = BtnEval.FlatAppearance.MouseDownBackColor
            Else
                btn.BackColor = BtnX.BackColor
                btn.ForeColor = BtnX.ForeColor
                btn.FlatAppearance.MouseOverBackColor = BtnX.FlatAppearance.MouseOverBackColor
                btn.FlatAppearance.MouseDownBackColor = BtnX.FlatAppearance.MouseDownBackColor
            End If

            EvaluateExpr(True)
            SaveSettings()
        End Sub

        Private Sub btnLog_MouseEnter(sender As Object, e As EventArgs) Handles BtnLog.MouseEnter
            BtnLog.ForeColor = Color.Lavender
        End Sub

        Private Sub btnLog_MouseLeave(sender As Object, e As EventArgs) Handles BtnLog.MouseLeave
            BtnLog.ForeColor = Color.LightSteelBlue
        End Sub

#End Region
#Region "Scintilla"
        Private Shared Function IsBrace(c As Integer) As Boolean
            Select Case ChrW(c)
                Case "("c, ")"c, "["c, "]"c, "{"c, "}"c
                    Return True
            End Select

            Return False
        End Function
        Private lastCaretPos As Integer = 0

        ' brace pairing
        Private Sub scintilla_UpdateUI(sender As Object, e As UpdateUIEventArgs) Handles Tb.UpdateUI
            ' Has the caret changed position?
            Dim caretPos As Integer = Tb.CurrentPosition
            If lastCaretPos <> caretPos Then
                lastCaretPos = caretPos
                Dim bracePos1 As Integer = -1
                Dim bracePos2 As Integer = -1

                ' Is there a brace to the left or right?
                If caretPos > 0 AndAlso IsBrace(Tb.GetCharAt(caretPos - 1)) Then
                    bracePos1 = (caretPos - 1)
                ElseIf IsBrace(Tb.GetCharAt(caretPos)) Then
                    bracePos1 = caretPos
                End If

                If bracePos1 >= 0 Then
                    ' Find the matching brace
                    bracePos2 = Tb.BraceMatch(bracePos1)
                    If bracePos2 = Scintilla.InvalidPosition Then
                        Tb.BraceBadLight(bracePos1)
                    Else
                        Tb.BraceHighlight(bracePos1, bracePos2)
                    End If
                Else
                    ' Turn off brace matching
                    Tb.BraceHighlight(Scintilla.InvalidPosition, Scintilla.InvalidPosition)
                End If
            End If
        End Sub

        ' syntax highlighting
        Private Sub tb_StyleNeeded(sender As Object, e As StyleNeededEventArgs) Handles Tb.StyleNeeded
            Dim startPos As Integer = Tb.GetEndStyled()
            Dim endPos As Integer = e.Position
            _cantusLexer.Style(Tb, startPos, endPos)
        End Sub

        ' autoindent
        Private Sub tb_InsertCheck(sender As Object, e As InsertCheckEventArgs) Handles Tb.InsertCheck
            If e.Text.EndsWith(ControlChars.Cr) OrElse e.Text.EndsWith(ControlChars.Lf) Then
                Dim curLine As Integer = Tb.LineFromPosition(e.Position)
                Dim curLineText As String = Tb.Lines(curLine).Text

                Dim indent As Match = Regex.Match(curLineText, "^\s*")
                e.Text += indent.Value


                Dim blockKwd As New HashSet(Of String)(("class function namespace if else elif for repeat " & "switch case run try catch finally while until with").Split(" "c))
                curLineText = curLineText.Trim()
                If curLineText.Contains(" ") Then curLineText = curLineText.Remove(curLineText.IndexOf(" "))

                If blockKwd.Contains(curLineText) Then e.Text += ControlChars.Tab

            End If
        End Sub

        Private Sub tb_CharAdded(sender As Object, e As CharAddedEventArgs) Handles Tb.CharAdded

            ' autocomplete
            Dim currentPos As Integer = Tb.CurrentPosition
            Dim wordStartPos As Integer = Tb.CurrentPosition

            While wordStartPos - 1 >= 0 AndAlso (
                  Tb.GetCharAt(wordStartPos - 1) >= AscW("0"c) AndAlso Tb.GetCharAt(wordStartPos - 1) <= AscW("9"c) OrElse Tb.GetCharAt(wordStartPos - 1) >= AscW("a"c) AndAlso Tb.GetCharAt(wordStartPos - 1) <= AscW("z"c) OrElse Tb.GetCharAt(wordStartPos - 1) >= AscW("A"c) AndAlso Tb.GetCharAt(wordStartPos - 1) <= AscW("Z"c) OrElse Tb.GetCharAt(wordStartPos - 1) = AscW("_"c) OrElse Tb.GetCharAt(wordStartPos - 1) = AscW("."c))
                wordStartPos -= 1
            End While

            If Tb.GetCharAt(wordStartPos) = AscW("."c) Then wordStartPos += 1

            Dim enteredWord As String = Tb.GetTextRange(wordStartPos, currentPos)

            Dim lenEntered As Integer = currentPos - wordStartPos

            If lenEntered > 0 Then

                Dim curLineText As String = Tb.GetTextRange(Tb.Lines(Tb.CurrentLine).Position, Tb.CurrentPosition)

                Dim singleQuote As Boolean = False
                Dim doubleQuote As Boolean = False
                For i As Integer = 0 To curLineText.Count - 1
                    If curLineText(i) = "'"c Then singleQuote = Not singleQuote
                    If curLineText(i) = """"c Then doubleQuote = Not doubleQuote
                Next
                If singleQuote OrElse doubleQuote Then Return ' do not autocomplete string

                curLineText = Tb.Lines(Tb.CurrentLine).Text
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

                    For Each v As Variable In _eval.Variables.Values.ToArray()
                        ' ignore private
                        If v.Modifiers.Contains("internal") OrElse (v.Modifiers.Contains("private") AndAlso Not IsParentScopeOf(v.DeclaringScope, _eval.Scope)) Then Continue For

                        ' ignore null
                        If v.Value Is Nothing OrElse TypeOf v.Value Is Double AndAlso Double.IsNaN(CDbl(v.Value)) OrElse TypeOf v.Value Is BigDecimal AndAlso DirectCast(v.Value, BigDecimal).IsUndefined Then Continue For

                        If nsMode Then ' filter namespace
                            Dim partialName As String = RemoveRedundantScope(v.FullName, _eval.Scope)

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
                                                   f & If(TypeOf ci.Fields(f).ResolveObj() Is Lambda, "(" & If(DirectCast(ci.Fields(f).ResolveObj(), Lambda).Args.Count = 0, "", "_") & ")",
                                              "")))
                                    Next
                                End If
                            Else
                                Continue For
                            End If
                        Else
                            autoCList.Add(RemoveRedundantScope(v.FullName, _eval.Scope) & If(TypeOf v.Value Is Lambda, "(" & If(DirectCast(v.Value, Lambda).Args.Count = 0, "", "_") & ")", ""))
                        End If
                    Next

                    ' internal functions
                    Dim varname As String = ""
                    Dim type As Type = Nothing
                    If nsMode AndAlso enteredWord.IndexOf(".") < enteredWord.Length AndAlso _eval.HasVariable(enteredWord.Remove(enteredWord.IndexOf("."))) Then

                        varname = enteredWord.Remove(enteredWord.IndexOf("."))

                        Dim var As Object = Nothing
                        var = _eval.GetVariable(enteredWord.Remove(enteredWord.IndexOf(".")))
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

                    For Each fn As UserFunction In _eval.UserFunctions.Values.ToArray()
                        If nsMode Then
                            If Not fn.FullName.ToLower().StartsWith(enteredWord.ToLower()) AndAlso Not fn.FullName.ToLower().StartsWith(RemoveRedundantScope(fn.FullName, _eval.Scope).ToLower()) Then
                                Continue For
                            End If
                        End If
                        ' ignore private
                        If fn.Modifiers.Contains("internal") OrElse (fn.Modifiers.Contains("private") AndAlso Not IsParentScopeOf(fn.DeclaringScope, _eval.Scope)) Then Continue For

                        If nsMode Then ' filter namespace
                            If fn.FullName.ToLower().StartsWith(enteredWord.ToLower()) Then
                                autoCList.Add(fn.FullName & "(" & If(fn.Args.Count = 0, "", "_") & ")")
                            ElseIf RemoveRedundantScope(fn.FullName, _eval.Scope).ToLower().StartsWith(enteredWord.ToLower()) Then
                                autoCList.Add(RemoveRedundantScope(fn.FullName, _eval.Scope) & "(" & If(fn.Args.Count = 0, "", "_") & ")")
                            Else
                                Continue For
                            End If
                        Else
                            autoCList.Add(RemoveRedundantScope(fn.FullName, _eval.Scope) & "(" & If(fn.Args.Count = 0, "", "_") & ")")
                        End If
                    Next

                    For Each uc As UserClass In _eval.UserClasses.Values.ToArray()
                        If nsMode Then
                            If Not uc.FullName.ToLower().StartsWith(enteredWord.ToLower()) AndAlso Not uc.FullName.ToLower().StartsWith(RemoveRedundantScope(uc.FullName, _eval.Scope).ToLower()) Then
                                Continue For
                            End If
                        End If
                        ' ignore private
                        If uc.Modifiers.Contains("internal") OrElse (uc.Modifiers.Contains("private") AndAlso Not IsParentScopeOf(uc.DeclaringScope, _eval.Scope)) Then Continue For

                        If nsMode Then ' filter namespace
                            If uc.FullName.ToLower().StartsWith(enteredWord.ToLower()) Then
                                autoCList.Add(uc.FullName & "(" & If(uc.Constructor.Args.Count = 0, "", "_") & ")")
                            ElseIf RemoveRedundantScope(uc.FullName, _eval.Scope).ToLower().StartsWith(enteredWord.ToLower()) Then
                                autoCList.Add(RemoveRedundantScope(uc.FullName, _eval.Scope) & "(" & If(uc.Constructor.Args.Count = 0, "", "_") & ")")
                            Else
                                Continue For
                            End If
                        Else
                            autoCList.Add(RemoveRedundantScope(uc.FullName, _eval.Scope) & "(" & If(uc.Constructor.Args.Count = 0, "", "_") & ")")
                        End If
                    Next
                    autoCList.Sort(New AutoCompleteComparer())
                End If

                If autoCList.Count = 0 Then Return
                Tb.AutoCShow(lenEntered, String.Join(" ", autoCList))
            End If

            ' brace completion
            If e.Char = AscW("(") OrElse e.Char = AscW("[") OrElse e.Char = AscW("{") OrElse e.Char = AscW(")") OrElse e.Char = AscW("]") OrElse e.Char = AscW("}") Then

                Dim startPos As Integer
                Dim curLine As Integer = Tb.CurrentLine
                Dim curText As String = Tb.Lines(curLine).Text

                While curLine > 0 AndAlso Tb.Lines(curLine - 1).Text.EndsWith(" _") ' connect _
                    curLine -= 1
                    curText = Tb.Lines(curLine).Text.Remove(Tb.Lines(curLine).Text.Length - 2) & curText
                End While
                startPos = Tb.Lines(curLine).Position

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
                        Dim len As Integer = Tb.CurrentPosition - startPos
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

                        Tb.InsertText(Tb.Lines(Tb.CurrentLine).Position + pos, endBr)
                    Else
                        Tb.InsertText(Tb.CurrentPosition, endBr)
                    End If
                End If

            ElseIf e.Char = AscW("|") OrElse e.Char = AscW(""""c) OrElse e.Char = AscW("'"c) OrElse e.Char = AscW("`"c) Then
                If e.Char = AscW(""""c) AndAlso Tb.CurrentPosition > 1 AndAlso Tb.GetTextRange(Tb.CurrentPosition - 2, 2) = """""" OrElse
                    e.Char = AscW("'"c) AndAlso Tb.CurrentPosition > 1 AndAlso Tb.GetTextRange(Tb.CurrentPosition - 2, 2) = "'" & "'" Then

                    ' if there were already two quotes before, do not add another: user probably wanted to type triple quotes
                    Tb.SelectionStart += 1
                    Return
                End If

                Dim curText As String = Tb.Lines(Tb.CurrentLine).Text
                Dim ct As Boolean = False

                For i As Integer = 0 To curText.Length - 1
                    If curText(i) = ChrW(e.Char) Then ct = Not ct
                Next

                If ct Then Tb.InsertText(Tb.CurrentPosition, ChrW(e.Char))

            End If
        End Sub

        Private Sub tb_AutoCCompleted(sender As Object, e As AutoCSelectionEventArgs) Handles Tb.AutoCCompleted
            If e.Text.EndsWith("(_)") Then
                Tb.DeleteRange(Tb.SelectionStart - 2, 1)
                Tb.SelectionStart -= 1
                Tb.SelectionEnd -= 1
            End If
        End Sub

        Private Sub TmrLoad_Tick(sender As Object, e As EventArgs) Handles TmrLoad.Tick
            Dim newProgress As Integer = SplashScreen.Progress.Value + 4
            If SplashScreen.Progress.Value < 100 Then
                SplashScreen.Progress.Value = newProgress
                Return
            End If

            TmrLoad.Stop()

            If _displayUpdateMessage Then
                SplashScreen.SendToBack()
                Try
                    Process.Start(
                    "cmd", "/c Assoc .can=Cantus.CBool  && Ftype Cantus.CantusScript=" & ControlChars.Quote & Application.ExecutablePath & ControlChars.Quote & " ""%1""")
                Catch
                End Try
                TmrLoad.Stop()
                Using diag As New Dialogs.DiagFeatureList()
                    diag.ShowDialog()
                End Using
            End If

            ' check for update
            If My.Settings.AutoUpdate Then
                _updTh = New Thread(CType(Sub()
                                              CheckUpdate()
                                          End Sub, ThreadStart))
                _updTh.IsBackground = True
                _updTh.Start()
            End If

            SplashScreen.Hide()

            ' set location
            If My.Settings.Position <> "" Then
                Dim spl() As String = My.Settings.Position.Split(","c)
                If spl.Length > 1 Then Me.Location = New Point(CInt(spl(0)), CInt(spl(1)))
                If spl.Length > 3 Then Me.Size = New Size(CInt(spl(2)), CInt(spl(3)))
                If spl.Length > 4 Then Me.WindowState = DirectCast([Enum].Parse(GetType(FormWindowState), spl(4)), FormWindowState)
                If spl.Length > 5 Then Split.SplitterDistance = CInt(spl(5))
                If Me.Location.X <= -Me.Width OrElse Me.Location.Y <= -Me.Height OrElse Me.Right >= Screen.PrimaryScreen.WorkingArea.Width + Me.Width OrElse Me.Bottom >= Screen.PrimaryScreen.WorkingArea.Height + Me.Height Then
                    Me.Left = CInt(Screen.PrimaryScreen.WorkingArea.Width / 2 - Me.Width / 2)
                    Me.Top = CInt(Screen.PrimaryScreen.WorkingArea.Height / 2 - Me.Height / 2)
                End If
            Else
                Me.Left = CInt(Screen.PrimaryScreen.WorkingArea.Width / 2 - Me.Width / 2)
                Me.Top = CInt(Screen.PrimaryScreen.WorkingArea.Height / 2 - Me.Height / 2)
            End If

            ' Minimize/show Keyboard 
            If My.Settings.ShowKeyboard Then
                FrmKeyboard.Show()
            End If

            ' save location
            My.Settings.Position = String.Join(",", DirectCast({Me.Left, Me.Top, Me.Width, Me.Height, Me.WindowState,
                                               Split.SplitterDistance}, Object()))

            Me.Tb.Select()
        End Sub

        Private Sub BtnKeyboard_Click(sender As Object, e As EventArgs) Handles BtnKeyboard.Click
            Tb.Focus()
            If FrmKeyboard.Visible Then
                FrmKeyboard.Hide()
                My.Settings.ShowKeyboard = False
            Else
                FrmKeyboard.Show()
                My.Settings.ShowKeyboard = True
                Tb.Focus()
            End If
            My.Settings.Save()
        End Sub

        Private Sub FrmEditor_SizeChanged(sender As Object, e As EventArgs) Handles Me.SizeChanged
            If Me.WindowState = FormWindowState.Normal Then
                Me.TTLetters.SetToolTip(Me.BtnMax, "Maximize (Win+Up)")
            Else
                Me.TTLetters.SetToolTip(Me.BtnMax, "Restore (Win+Down)")
            End If
        End Sub
#End Region
    End Class

    Public Module Globals
        '' <summary>
        '' The default evaluator in the root namespace
        '' </summary>
        Friend Property RootEvaluator As CantusEvaluator
        Public Const ReleaseType As String = "Alpha"
        Public ReadOnly Property Version As String = Assembly.GetAssembly(GetType(CantusEvaluator)).GetName().
                                                        Version.ToString() & " " & ReleaseType
    End Module
End Namespace