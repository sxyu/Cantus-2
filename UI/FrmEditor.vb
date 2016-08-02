Imports System.Reflection
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Threading
Imports Cantus.Evaluator
Imports Cantus.Evaluator.CommonTypes
Imports Cantus.Evaluator.Evaluator
Imports Cantus.Evaluator.Exceptions
Imports Cantus.Evaluator.ObjectTypes
Imports Cantus.UI.ScintillaForCantus
Imports ScintillaNET

Namespace UI
    Public Class FrmEditor
#Region "Declarations"
        ''' <summary>
        ''' Maximum length of text to display on the tooltip
        ''' </summary>
        Private Const TT_LEN_LIMIT As Integer = 500
        Private Const PREV_EXPRESSION_LIMIT As Integer = 15

        Private Const RELEASE_TYPE As String = "Alpha"

        ''' <summary>
        ''' The main evaluator
        ''' </summary>
        Private _eval As Evaluator.Evaluator

        ''' <summary>
        ''' The path to the file currently open in the editor. 
        ''' If no file is open, stores an empty string.
        ''' </summary>
        Public ReadOnly Property File As String = ""

        ' expression memory (up/down arrow keys)
        Private _prevExp As New List(Of String)
        Private _lastExp As String = ""
        Private _curExpId As Integer = 0

        ' keyboard snapping
        Public RSnap As Boolean = False
        Public LSnap As Boolean = False

        ' update checking thread
        Private _updTh As Thread

        ''' <summary>
        '''  if true, allows user to press enter in textbox
        ''' </summary>
        Private _allowEnter As Boolean = False

        ''' <summary>
        ''' If true, displays the update message after load
        ''' </summary>
        Private _displayUpdateMessage As Boolean = False

        ''' <summary>
        ''' Custom lexer for scintilla
        ''' </summary>
        Private _cantusLexer As New CantusLexer()

        ''' <summary>
        ''' Counter used for lazy evaluation of expressions
        ''' </summary>
        Private _editCt As Integer = 0

#End Region
#Region "Form Events"
        Private Sub FrmEditor_Load(sender As Object, e As EventArgs) Handles MyBase.Load
            SplashScreen.Show()
            SplashScreen.BringToFront()
            TmrLoad.Start()

            ' new version? upgrade settings from previous version + show message
            If My.Settings.ReqUpgrade Then
                My.Settings.Upgrade()
                My.Settings.ReqUpgrade = False
                _displayUpdateMessage = True
            End If

            ' icon
            Me.Icon = My.Resources.Calculator

            ' setup evaluator
            Me._eval = Globals.Evaluator
            AddHandler Globals.Evaluator.EvalComplete, AddressOf EvalComplete

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
                End If
            Next

            def = def.Trim().Trim({ControlChars.Quote, "'"c})
            If opengraph Then
                btnMin.PerformClick()
                ' GRAPH
            Else
                tb.Text = def
            End If

            ' setup keyboards
            Me.RSnap = My.Settings.ROskSnap
            Me.LSnap = My.Settings.LOskSnap

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
                        My.Settings.State = ""
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

            If _displayUpdateMessage Then
                Try
                    Process.Start(
                    "cmd", "/c Assoc .can=Cantus.CBool  && Ftype Cantus.CantusScript=" &
                    ControlChars.Quote & Application.ExecutablePath & ControlChars.Quote & " ""%1""")
                Catch
                End Try
                Keyboards.KeyboardLeft.Hide()
                Keyboards.KeyboardRight.Hide()
                TmrLoad.Stop()
                Using diag As New Dialogs.DiagFeatureList()
                    diag.ShowDialog()
                End Using
                TmrLoad.Start()
            End If

            ' update tooltips
            UpdateLetterTT()

            ' set up UI
            pnlSettings.BackColor = Color.FromArgb(50, 30, 30, 30)
            cbAutoUpd.Checked = My.Settings.AutoUpdate
            lbAbout.Text = lbAbout.Text.Replace("{VER}", Application.ProductVersion & " " & RELEASE_TYPE)

            ' load other forms
            Keyboards.KeyboardLeft.Show()
            Keyboards.KeyboardRight.Show()
            FrmViewer.Show()

            ' faster drawing
            Me.SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.OptimizedDoubleBuffer, True)

            ' set up modes
            If _eval.OutputFormat = Evaluator.Evaluator.eOutputFormat.Math Then
                btnOutputFormat.Text = "MathO"
            ElseIf _eval.OutputFormat = Evaluator.Evaluator.eOutputFormat.Scientific Then
                btnOutputFormat.Text = "SciO"
            Else
                btnOutputFormat.Text = "LineO"
            End If
            If _eval.AngleMode = Evaluator.Evaluator.eAngleRepresentation.Radian Then
                btnAngleRepr.Text = "Radian"
            ElseIf _eval.AngleMode = Evaluator.Evaluator.eAngleRepresentation.Degree Then
                btnAngleRepr.Text = "Degree"
            Else
                btnAngleRepr.Text = "Gradian"
            End If

            ' set up scintilla
            SetTheme("dark")

            ' check for update
            If My.Settings.AutoUpdate Then
                _updTh = New Thread(CType(Sub()
                                              CheckUpdate()
                                          End Sub, ThreadStart))
                _updTh.Start()
            End If

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
        Private Sub FrmEditor_MouseUp(sender As Object, e As MouseEventArgs) Handles MyBase.MouseUp, tb.MouseUp, lbResult.MouseUp, pnlTb.MouseUp
            tb.Focus()
            pnlSettings.Hide()
        End Sub
        Private Sub FrmEditor_Leave(sender As Object, e As EventArgs) Handles MyBase.Deactivate
            pnlSettings.Hide()
        End Sub
        Private Sub FrmEditor_SizeChanged(sender As Object, e As EventArgs) Handles MyBase.SizeChanged
            If Me.WindowState = FormWindowState.Normal AndAlso My.Settings.ShowKbd Then
                If LSnap Then Keyboards.KeyboardLeft.Show()
                If RSnap Then Keyboards.KeyboardRight.Show()
            End If
        End Sub

        Dim _ignoreNextKey As Boolean = False
        Private Sub FrmEditor_KeyUp(sender As Object, e As KeyEventArgs) Handles MyBase.KeyUp, tb.KeyUp
            If e.KeyCode = Keys.Enter And e.Alt Then
                If sender Is Me Then Return
                EvaluateExpr()
            ElseIf e.Control AndAlso e.Alt Then
                If e.KeyCode = Keys.P Then
                    btnAngleRep_Click(btnAngleRepr, New EventArgs)
                ElseIf e.KeyCode = Keys.D OrElse e.KeyCode = Keys.R OrElse e.KeyCode = Keys.G Then
                    If e.KeyCode = Keys.D Then
                        _eval.AngleMode = eAngleRepresentation.Degree
                    ElseIf e.KeyCode = Keys.R
                        _eval.AngleMode = eAngleRepresentation.Radian
                    Else
                        _eval.AngleMode = eAngleRepresentation.Gradian
                    End If
                    btnAngleRepr.Text = _eval.AngleMode.ToString()
                    EvaluateExpr(True)
                ElseIf e.KeyCode = Keys.O Then
                    btnOMode_Click(btnOutputFormat, New EventArgs)
                ElseIf e.KeyCode = Keys.M OrElse e.KeyCode = Keys.S OrElse e.KeyCode = Keys.L Then
                    If e.KeyCode = Keys.M Then
                        _eval.OutputFormat = eOutputFormat.Math
                        e.SuppressKeyPress = True
                    ElseIf e.KeyCode = Keys.L
                        _eval.OutputFormat = Evaluator.Evaluator.eOutputFormat.Raw
                    Else
                        _eval.OutputFormat = Evaluator.Evaluator.eOutputFormat.Scientific
                    End If
                    btnOutputFormat.Text = _eval.OutputFormat.ToString()
                    EvaluateExpr(True)
                ElseIf e.KeyCode = Keys.T Then
                    btnExplicit_Click(btnExplicit, New EventArgs())
                End If
            End If
        End Sub

        Private Sub FrmEditor_KeyPress(sender As Object, e As KeyPressEventArgs) Handles tb.KeyPress
            If _ignoreNextKey Then
                _ignoreNextKey = False
                e.Handled = True
            End If
        End Sub
#End Region
#Region "Shared functions"
        ''' <summary>
        ''' Send the event to asynchroneously evalute the expression
        ''' </summary>
        Private Sub EvaluateExpr(Optional noSaveAns As Boolean = False)
            _eval.EvalAsync(tb.Text, noSaveAns) ' evaluate & wait for event

            If Not noSaveAns Then
                ' save previous expressions 
                If _prevExp.Count = 0 OrElse _prevExp(_prevExp.Count - 1) <> tb.Text Then
                    _prevExp.Add(tb.Text)
                    _curExpId += 1
                End If

                ' remove previous expressions past limit
                _lastExp = tb.Text
                If _prevExp.Count > PREV_EXPRESSION_LIMIT Then
                    _prevExp.RemoveAt(0) 'max expressions
                    _curExpId -= 1
                End If

                Dim logText As String = "'"
                If tb.TextLength > 103 Then
                    logText = tb.GetTextRange(0, 100) & "..."
                Else
                    logText = tb.Text
                End If

                ' write expression to log
                FrmViewer.WriteLogSection(logText)
            End If
        End Sub

        Private _lastAnsCount As Integer = 0
        Private Sub EvalComplete(sender As Object, result As Object)
            If lbResult.InvokeRequired Then
                lbResult.BeginInvoke(Sub() EvalComplete(sender, result))
            Else
                Dim ans As String = result.ToString()

                ' display answer
                lbResult.Text = AutoTrimDisplayText(ans)
                If _eval.PrevAns.Count > _lastAnsCount Then
                    FrmViewer.WriteLog("= " & ans)
                End If
                _lastAnsCount = _eval.PrevAns.Count

                tb.Focus()
            End If
        End Sub

        Private Function AutoTrimDisplayText(txt As String) As String
            Dim g As Graphics = Graphics.FromHwnd(Me.Handle)
            Dim i As Integer = 0
            Dim res As String = "= "
            Dim wid As Single = g.MeasureString(res, lbResult.Font).Width
            While i < txt.Length AndAlso wid < lbResult.Width - 1
                res &= txt(i)
                i += 1
                wid = g.MeasureString(res, lbResult.Font).Width
            End While

            If wid > lbResult.Width - 1 Then
                While wid > lbResult.Width - 1 AndAlso res.Length > 0
                    res = res.Remove(res.Length - 1)
                    wid = g.MeasureString(res & "...", lbResult.Font).Width
                End While
                res &= "..."
                If txt.Length <= TT_LEN_LIMIT Then
                    TTLetters.SetToolTip(lbResult, txt)
                Else
                    TTLetters.SetToolTip(lbResult, txt.Remove(TT_LEN_LIMIT - 3) & "...")
                End If
            Else
                TTLetters.SetToolTip(lbResult, "")
            End If

            Return res
        End Function


        Public Sub SetTheme(name As String)
            tb.StyleResetDefault()

            Select Case name
                Case "dark"
                    With tb.Styles(Style.Default)
                        .BackColor = Color.FromArgb(34, 34, 34)
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

                    tb.Styles(CantusLexer.StyleNumber).ForeColor = Color.FromArgb(255, 205, 34)
                    tb.Styles(CantusLexer.StyleString).ForeColor = Color.FromArgb(236, 118, 0)

                    tb.Styles(CantusLexer.StyleComment).ForeColor = Color.FromArgb(153, 163, 138)

                    tb.Lexer = Lexer.Container

                    tb.IndentWidth = 4

                    With tb.Styles(Style.LineNumber)
                        .BackColor = Color.FromArgb(34, 34, 34)
                        .ForeColor = Color.DarkGray
                        .Size = 13
                    End With

                    tb.IndentationGuides = IndentView.Real

                    tb.Styles(Style.BraceLight).ForeColor = Color.BlueViolet
                    tb.Styles(Style.BraceLight).BackColor = Color.LightGray

                    tb.Styles(Style.BraceBad).ForeColor = Color.White
                    tb.Styles(Style.BraceBad).BackColor = Color.IndianRed

                    tb.Styles(Style.IndentGuide).ForeColor = Color.Gray

                    Dim margin As Margin = tb.Margins(0)
                    margin.Width = 45

                    tb.TabWidth = 4

                    tb.ScrollWidth = tb.Width - 2 * margin.Width - 5

                    tb.AutoCIgnoreCase = True
            End Select
        End Sub


        ''' <summary>
        ''' Save all settings to the init.can file
        ''' </summary>
        Public Sub SaveSettings()
            My.Settings.ROskSnap = Me.RSnap
            My.Settings.LOskSnap = Me.LSnap

            Try
                Dim curText As String = ""
                If IO.File.Exists("init.can") Then
                    curText = IO.File.ReadAllText("init.can")

                    ' cut everything up to the end comment
                    Dim endComment As String = "# end of cantus auto-generated initialization script." &
                        " do not modify this comment."
                    If curText.ToLower().Contains(endComment) Then
                        curText = curText.Substring(curText.ToLower().LastIndexOf(endComment) + endComment.Length +
                                                    ControlChars.NewLine.Length +
                                                     "# You may write additional initialization code below this line.".Length)
                    End If
                End If
                curText = curText.TrimEnd() & vbNewLine

                ' try writing to init.can
                IO.File.WriteAllText("init.can", _eval.ToScript() & curText)
                My.Settings.State = ""
            Catch ex As Exception
                ' we weren't able to write to init.can, so write to Settings.State
                My.Settings.State = _eval.ToScript()
            End Try

            My.Settings.Save()
        End Sub
#End Region
#Region "main buttons"
        Private Sub btnCalc_Click(sender As Object, e As System.EventArgs) Handles btnEval.Click
            tb.Focus()
            EvaluateExpr()
        End Sub

        Private Sub btnGraph_Click(sender As Object, e As EventArgs)
            ' open graphing window
            tb.Focus()
            ' GRAPH
            btnMin.PerformClick()
        End Sub

        Private Sub btnFunctions_Click(sender As Object, e As EventArgs) Handles btnFunctions.Click
            tb.Focus()
            Using diag As New Dialogs.DiagFunctions
                If diag.ShowDialog() = DialogResult.OK Then
                    Dim start As Integer = tb.SelectionStart
                    tb.Text = tb.Text.Remove(
                                tb.SelectionStart, tb.SelectionEnd - tb.SelectionStart).Insert(start, diag.Result)

                    If diag.Result.Contains("(") Then
                        tb.SelectionStart = start + diag.Result.IndexOf("(") + 1
                    Else
                        tb.SelectionStart = start + diag.Result.Count
                    End If
                End If
            End Using
        End Sub

        ''' <summary>
        ''' Open a 'save as' dialogue to save as another file
        ''' </summary>
        Private Sub OpenSaveAs()
            Using diag As New SaveFileDialog()
                diag.Filter = "Cantus Script (.can)|*.can"
                diag.RestoreDirectory = True
                diag.Title = "Save As Script"
                If diag.ShowDialog = DialogResult.OK Then
                    IO.File.WriteAllText(diag.FileName, tb.Text, Encoding.UTF8)
                    _File = diag.FileName ' set new file name
                End If
            End Using
        End Sub

        ''' <summary>
        ''' Open the 'open script' dialog to open a script
        ''' </summary>
        Private Sub Open()
            Using diag As New OpenFileDialog()
                diag.Filter = "Cantus Script (.can)|*.can"
                diag.RestoreDirectory = True
                diag.Multiselect = False
                diag.Title = "Open Script"
                If diag.ShowDialog = DialogResult.OK Then
                    tb.Text = IO.File.ReadAllText(diag.FileName).Replace(vbCrLf, vbLf).Replace(vbCr, vbLf).
                                Replace(vbLf, vbNewLine) ' fix line endings
                    _File = diag.FileName
                End If
            End Using
        End Sub

        ''' <summary>
        ''' Save the file; if the editor is not linked to any file, opens the save as dialogue.
        ''' </summary>
        Private Sub Save()
            If String.IsNullOrEmpty(Me.File) Then
                OpenSaveAs()
            Else
                IO.File.WriteAllText(File, tb.Text, Encoding.UTF8)
            End If
        End Sub

        Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
            tb.Focus()
            Save()
            SaveSettings()
        End Sub

        Private Sub btnOpen_Click(sender As Object, e As EventArgs) Handles btnOpen.Click
            tb.Focus()
            Open()
        End Sub
#Region "textbox & labels"

        Private Sub Tb_KeyDown(sender As Object, e As KeyEventArgs) Handles tb.KeyDown
            If e.Alt Then
                If e.KeyCode = Keys.Up Then
                    If _curExpId > 1 And _prevExp.Count > 1 Then
                        If (tb.Text = _lastExp) Then
                            _curExpId -= 1
                        Else
                            _prevExp.Add(tb.Text)
                        End If
                        _lastExp = _prevExp(_curExpId - 1)
                        tb.Text = _lastExp
                        tb.SelectionStart = tb.Text.Length
                    End If
                ElseIf e.KeyCode = Keys.Down Then
                    If _curExpId < _prevExp.Count And _prevExp.Count > 1 Then
                        _curExpId += 1
                        _lastExp = _prevExp(_curExpId - 1)
                        tb.Text = _lastExp
                        tb.SelectionStart = tb.Text.Length
                    End If
                ElseIf e.KeyCode = Keys.F Then
                    btnFunctions.PerformClick()
                ElseIf e.KeyCode = Keys.S Then
                    btnSettings.PerformClick()
                ElseIf e.KeyCode = Keys.G Then
                    FrmViewer.View = FrmViewer.eView.graphing
                    Try
                        FrmViewer.pnl.Controls(0).Select()
                    Catch
                    End Try
                ElseIf e.KeyCode = Keys.R Then
                    FrmViewer.View = FrmViewer.eView.log
                    Try
                        FrmViewer.pnl.Controls(0).Select()
                    Catch
                    End Try
                End If
            ElseIf e.KeyCode = Keys.F12
                OpenSaveAs()
            ElseIf e.KeyCode = Keys.S AndAlso e.Control
                _ignoreNextKey = True
                Save()
            ElseIf e.KeyCode = Keys.F11 OrElse e.KeyCode = Keys.O AndAlso e.Control
                _ignoreNextKey = True
                Open()

            ElseIf e.KeyCode = Keys.F6
                Using diag As New OpenFileDialog()
                    diag.Filter = "Cantus Script (.can)|*.can"
                    diag.RestoreDirectory = True
                    diag.Multiselect = False
                    diag.Title = "Import Script"
                    If diag.ShowDialog = DialogResult.OK Then
                        _eval.Load(diag.FileName, False, True)
                    End If
                End Using

            ElseIf e.KeyCode = Keys.F5
                Using diag As New OpenFileDialog()
                    diag.Filter = "Cantus Script (.can)|*.can"
                    diag.RestoreDirectory = True
                    diag.Multiselect = False
                    diag.Title = "Run Script"
                    If diag.ShowDialog = DialogResult.OK Then
                        _eval.EvalAsync(IO.File.ReadAllText(diag.FileName))
                    End If
                End Using
            End If

        End Sub

        Private Sub lbResult_TextChanged(sender As Object, e As EventArgs) Handles lbResult.TextChanged
            Keyboards.KeyboardRight.lbResult.Text = lbResult.Text
        End Sub
#End Region
#End Region
#Region "memory ui"
        Private Sub btnSettings_Click(sender As Object, e As EventArgs) Handles btnSettings.Click
            If Not pnlSettings.Visible Then
                If pnlSettings.BackgroundImage Is Nothing Then pnlSettings.BackgroundImage = New Bitmap(pnlSettings.Width, pnlSettings.Height)

                Try
                    pnlTb.DrawToBitmap(CType(pnlSettings.BackgroundImage, Bitmap), New Rectangle(0, 0, pnlSettings.Width, pnlSettings.Height))
                Catch ex As Exception
                End Try
                pnlTb.Focus()
                btnAngleRepr.Text = _eval.AngleMode.ToString()
                btnOutputFormat.Text = _eval.OutputFormat.ToString()
                pnlSettings.Show()
            Else
                EvaluateExpr(True)
                pnlSettings.Hide()
            End If
        End Sub

        Private Sub btnLetters_Click(sender As Object, e As EventArgs) Handles btnY.Click, btnX.Click, btnT.Click, btnM.Click
            Dim btn As Button = DirectCast(sender, Button)
            SetVariable(btn.Tag.ToString()(0), _eval.GetLastAns())
            UpdateLetterTT()
        End Sub

        Private Sub SetVariable(varnm As Char, data As Object)
            _eval.SetVariable(varnm, ObjectTypes.DetectType(data))
        End Sub

        ''' <summary>
        ''' Update tooltips for variable buttons
        ''' </summary>
        Private Sub UpdateLetterTT()
            For Each c As Control In pnlSettings.Controls
                If c.Tag Is Nothing OrElse c.Tag.ToString() = "-" Then Continue For

                Dim val As String
                Try
                    val = _eval.GetVariableRef(c.Text.Remove(0, 1)(0)).ToString()
                Catch
                    val = "Undefined"
                End Try

                TTLetters.SetToolTip(c, c.Text.Remove(0, 1) & " = " & val)
            Next
        End Sub
#End Region
#Region "Update"
        Private _updateStarted As Boolean = False
        Private Sub CheckUpdate(Optional ByVal promptuser As Boolean = False)
            If _updateStarted Then
                If (Me.InvokeRequired) Then
                    Me.BeginInvoke(Sub()
                                       MsgBox("Please wait, we're already checking for updates.", MsgBoxStyle.Exclamation, "Checking For Updates")
                                   End Sub)
                Else
                    MsgBox("Please wait, we're already checking for updates.", MsgBoxStyle.Exclamation, "Checking For Updates")
                End If
                Exit Sub
            End If
            _updateStarted = True
            Dim nv As String = ""
            Try
                Using wc As New System.Net.WebClient()
                    nv = wc.DownloadString("https://drive.google.com/uc?export=download&id=0B314tJw3ioySY0k1THVWZFV6S00")
                End Using
                Dim spl() As String = nv.Split("."c)
                Dim curverspl() As String = Application.ProductVersion.Split("."c)
                For i As Integer = 0 To spl.Length - 1
                    If CInt(spl(i)) > CInt(curverspl(i)) Then
                        If Not promptuser OrElse MessageBox.Show(Me, "New version of Cantus: " & nv & " found." & vbCrLf & "Update now?", "Update Found",
                              MessageBoxButtons.YesNo, MessageBoxIcon.Information,
                              MessageBoxDefaultButton.Button1) = DialogResult.Yes Then
                            Exit For ' needs update
                        Else
                            Exit Sub
                        End If
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
                    If FileIO.FileSystem.FileExists(Application.StartupPath & "\calculator.backup") Then
                        FileIO.FileSystem.DeleteFile(Application.StartupPath & "\calculator.backup")
                    End If
                Catch 'ex2 As Exception
                End Try

                Try
                    FileIO.FileSystem.RenameFile(Application.ExecutablePath, "cantus.backup")
                Catch 'ex As Exception
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
            Keyboards.KeyboardLeft.Visible = False
            Keyboards.KeyboardRight.Visible = False
            Me.Visible = False
            Using fup As New Updater.FrmUpdate
                Updater.FrmUpdate.ShowDialog()
                Updater.FrmUpdate.BringToFront()
                Keyboards.KeyboardLeft.SendToBack()
                Keyboards.KeyboardRight.SendToBack()
            End Using
        End Sub
#End Region
#Region "Command Buttons & Aesthetic"
        Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
            Me.Close()
        End Sub

        Private Sub btnMin_Click(sender As Object, e As EventArgs) Handles btnMin.Click
            Me.WindowState = FormWindowState.Minimized
            If LSnap Then Keyboards.KeyboardLeft.Hide()
            If RSnap Then Keyboards.KeyboardRight.Hide()
        End Sub

        Private Sub btnMem_Enter(sender As Object, e As EventArgs) Handles btnSettings.Enter, btnEval.Enter, btnMin.Enter, btnClose.Enter
            tb.Focus()
        End Sub
        Private Sub pnlMemLtrs_VisibleChanged(sender As Object, e As EventArgs) Handles pnlSettings.VisibleChanged
            If pnlSettings.Visible Then
                UpdateLetterTT()
                btnSettings.BackColor = Color.FromArgb(60, 60, 60)
                btnSettings.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 60, 60)
            Else
                btnSettings.BackColor = Color.FromArgb(55, 55, 55)
                btnSettings.FlatAppearance.MouseOverBackColor = Color.FromArgb(55, 55, 55)
            End If
        End Sub
#End Region
#Region "Form Moving"
        Private _isMoving As Boolean
        Private _movingPrevPt As Point

        Private Sub me_MouseDown(sender As Object, e As MouseEventArgs) Handles MyBase.MouseDown, pnlResults.MouseDown, lbResult.MouseDown
            If e.Button <> MouseButtons.Right Then
                _movingPrevPt = e.Location
                _isMoving = True
            End If
        End Sub

        Private Sub me_MouseMove(sender As Object, e As MouseEventArgs) Handles MyBase.MouseMove, pnlResults.MouseMove, lbResult.MouseMove
            If _isMoving Then
                Dim newLft As Integer = Me.Left + e.X - _movingPrevPt.X
                Dim newTop As Integer = Me.Top + e.Y - _movingPrevPt.Y
                Me.Left = newLft
                Me.Top = newTop
                If LSnap Then
                    Keyboards.KeyboardLeft.Left = newLft
                    Keyboards.KeyboardLeft.Top = newTop + Me.Height - 1
                End If
                If RSnap Then
                    Keyboards.KeyboardRight.Left = newLft + Me.Width - Keyboards.KeyboardRight.Width
                    Keyboards.KeyboardRight.Top = newTop + Me.Height - 1
                End If
                If FrmViewer.Snap Then
                    FrmViewer.Left = newLft - FrmViewer.Width
                    FrmViewer.Top = newTop
                End If
                Keyboards.KeyboardLeft.BringToFront()
                Keyboards.KeyboardRight.BringToFront()
            End If
        End Sub

        Private Sub me_MouseUp(sender As Object, e As MouseEventArgs) Handles MyBase.MouseUp, pnlResults.MouseUp, lbResult.MouseUp
            If e.Button = MouseButtons.Right Then
                If Me.LSnap Or Me.RSnap Then
                    Me.RSnap = False
                    Me.LSnap = False
                    Me.Top -= 6
                    My.Settings.ROskSnap = RSnap
                    My.Settings.LOskSnap = LSnap
                End If
                If FrmViewer.Snap Then
                    Me.Left += 6
                    FrmViewer.Snap = False
                    My.Settings.VSnap = False
                End If
                My.Settings.Save()
            Else
                ' these parts are quite messy, and will need cleanup eventually
                _isMoving = False
                If Me.Right > FrmViewer.Left - 40 AndAlso Me.Left < FrmViewer.Right + 40 AndAlso
                    (Me.Bottom > FrmViewer.Top AndAlso Me.Top < FrmViewer.Bottom) Then
                    FrmViewer.Snap = True
                    My.Settings.VSnap = True
                    FrmViewer.Left = Me.Left - FrmViewer.Width
                    FrmViewer.Top = Me.Top
                End If
                If Me.Bottom > Keyboards.KeyboardRight.Top AndAlso
                    Me.Bottom < Keyboards.KeyboardRight.Top + Keyboards.KeyboardRight.Height / 2 AndAlso
                    Me.Left > Keyboards.KeyboardRight.Left - 400 AndAlso
                    Me.Right < Keyboards.KeyboardRight.Right + 400 AndAlso Not RSnap Then
                    Me.Left = Keyboards.KeyboardRight.Right - Me.Width
                    Me.Top = Keyboards.KeyboardRight.Top - Me.Height
                    Me.RSnap = True
                    If LSnap Then
                        Keyboards.KeyboardLeft.Left = Me.Left
                        Keyboards.KeyboardLeft.Top = Me.Bottom
                    End If
                End If
                If Me.Bottom >= Keyboards.KeyboardLeft.Top AndAlso
                    Me.Bottom < Keyboards.KeyboardLeft.Top + Keyboards.KeyboardLeft.Height / 2 AndAlso
                    (Me.Left > Keyboards.KeyboardLeft.Left - 400 AndAlso Me.Left < Keyboards.KeyboardLeft.Right + 400) AndAlso
                    Not LSnap Then
                    Me.Left = Keyboards.KeyboardLeft.Left
                    Me.Top = Keyboards.KeyboardLeft.Top - Me.Height
                    Me.LSnap = True
                    If RSnap Then
                        Keyboards.KeyboardRight.Left = Me.Right - Keyboards.KeyboardRight.Width
                        Keyboards.KeyboardRight.Top = Me.Bottom
                    End If
                End If
                If LSnap AndAlso RSnap Then
                    Keyboards.KeyboardRight.Snap = True
                    Keyboards.KeyboardLeft.Snap = True
                    My.Settings.OskLock = True
                End If
                If FrmViewer.Snap Then
                    FrmViewer.Left = Me.Left - FrmViewer.Width
                    FrmViewer.Top = Me.Top
                End If
                My.Settings.MainPos = Me.Left & "," & Me.Top
                My.Settings.LeftKbdPos = Keyboards.KeyboardLeft.Left & "," & Keyboards.KeyboardLeft.Top
                My.Settings.RightKbdPos = Keyboards.KeyboardRight.Left & "," & Keyboards.KeyboardRight.Top
                My.Settings.ROskSnap = RSnap
                My.Settings.LOskSnap = LSnap
            End If
            My.Settings.Save()
        End Sub

        Private Sub btnOMode_Click(sender As Object, e As EventArgs) Handles btnOutputFormat.Click
            If _eval.OutputFormat = Evaluator.Evaluator.eOutputFormat.Scientific Then
                _eval.OutputFormat = Evaluator.Evaluator.eOutputFormat.Raw
            Else
                _eval.OutputFormat = CType(CInt(_eval.OutputFormat) + 1, Evaluator.Evaluator.eOutputFormat)
            End If
            If _eval.OutputFormat = Evaluator.Evaluator.eOutputFormat.Math Then
                btnOutputFormat.Text = "Math"
            ElseIf _eval.OutputFormat = Evaluator.Evaluator.eOutputFormat.Scientific Then
                btnOutputFormat.Text = "Scientific"
            Else
                btnOutputFormat.Text = "Raw"
            End If
            EvaluateExpr(True)
        End Sub

        Private Sub btnAngleRep_Click(sender As Object, e As EventArgs) Handles btnAngleRepr.Click
            If _eval.AngleMode = Evaluator.Evaluator.eAngleRepresentation.Gradian Then
                _eval.AngleMode = Evaluator.Evaluator.eAngleRepresentation.Degree
            Else
                _eval.AngleMode = CType(CInt(_eval.AngleMode) + 1, Evaluator.Evaluator.eAngleRepresentation)
            End If
            If _eval.AngleMode = Evaluator.Evaluator.eAngleRepresentation.Radian Then
                btnAngleRepr.Text = "Radian"
            ElseIf _eval.AngleMode = Evaluator.Evaluator.eAngleRepresentation.Degree Then
                btnAngleRepr.Text = "Degree"
            Else
                btnAngleRepr.Text = "Gradian"
            End If
            EvaluateExpr(True)
        End Sub

        Private Sub pnlSettings_Paint(sender As Object, e As PaintEventArgs) Handles pnlSettings.Paint
            e.Graphics.TextRenderingHint = Drawing.Text.TextRenderingHint.ClearTypeGridFit
            Using backbr As New SolidBrush(Color.FromArgb(150, 20, 20, 20))
                e.Graphics.FillRectangle(backbr, 0, 0, pnlSettings.Width, pnlSettings.Height)
            End Using
        End Sub

        Private Sub pnlSettings_Click(sender As Object, e As EventArgs) Handles pnlSettings.Click, lbSettings.Click
            btnSettings.PerformClick()
        End Sub

        Private Sub lbAbout_Click(sender As Object, e As EventArgs) Handles lbAbout.Click, btnLog.Click
            Using diag As New Dialogs.DiagFeatureList()
                diag.ShowDialog()
            End Using
            tb.Focus()
        End Sub

        Private Sub cbAutoUpd_CheckedChanged(sender As Object, e As EventArgs) Handles cbAutoUpd.CheckedChanged
            My.Settings.AutoUpdate = cbAutoUpd.Checked
            My.Settings.Save()
        End Sub

        Private Sub btnUpdate_Click(sender As Object, e As EventArgs) Handles btnUpdate.Click
            _updTh = New Thread(CType(Sub()
                                          CheckUpdate(True)
                                      End Sub, ThreadStart))
            _updTh.Start()
        End Sub
        ' make sure the window doesn't get maximized - that would cause weird behaviour
        Private Sub FrmEditor_LocationChanged(sender As Object, e As EventArgs) Handles Me.LocationChanged
            If Me.WindowState = FormWindowState.Maximized Then Me.WindowState = FormWindowState.Normal
            If Keyboards.KeyboardRight.WindowState = FormWindowState.Maximized Then
                Keyboards.KeyboardRight.WindowState = FormWindowState.Normal
                If RSnap Then
                    Keyboards.KeyboardRight.Left = Me.Right - Keyboards.KeyboardRight.Width
                    Keyboards.KeyboardRight.Top = Me.Bottom
                End If
            End If
            If Keyboards.KeyboardLeft.WindowState = FormWindowState.Maximized Then
                Keyboards.KeyboardLeft.WindowState = FormWindowState.Normal
                If LSnap Then
                    Keyboards.KeyboardLeft.Left = Me.Left
                    Keyboards.KeyboardLeft.Top = Me.Bottom
                End If
            End If
        End Sub

        Private Sub tb_TextChanged(sender As Object, e As EventArgs) Handles tb.TextChanged
            _editCt = 2
            TmrReCalc.Start()
        End Sub

        Private Sub TmrReCalc_Tick(sender As Object, e As EventArgs) Handles TmrReCalc.Tick
            If _editCt <= 0 Then
                TmrReCalc.Stop()
                If Not String.IsNullOrEmpty(File) Then Save() ' save if file available
                EvaluateExpr(True)
            End If
            _editCt -= 1
        End Sub

        Private Sub btnExplicit_Click(sender As Object, e As EventArgs) Handles btnExplicit.Click
            _eval.ExplicitMode = Not _eval.ExplicitMode
            If _eval.ExplicitMode Then
                btnExplicit.BackColor = btnEval.BackColor
                btnExplicit.ForeColor = btnEval.ForeColor
                btnExplicit.FlatAppearance.MouseOverBackColor = btnEval.FlatAppearance.MouseOverBackColor
                btnExplicit.FlatAppearance.MouseDownBackColor = btnEval.FlatAppearance.MouseDownBackColor
            Else
                btnExplicit.BackColor = btnX.BackColor
                btnExplicit.ForeColor = btnX.ForeColor
                btnExplicit.FlatAppearance.MouseOverBackColor = btnX.FlatAppearance.MouseOverBackColor
                btnExplicit.FlatAppearance.MouseDownBackColor = btnX.FlatAppearance.MouseDownBackColor
            End If
        End Sub

        Private Sub btnLog_MouseEnter(sender As Object, e As EventArgs) Handles btnLog.MouseEnter
            btnLog.ForeColor = Color.LightSalmon
        End Sub

        Private Sub btnLog_MouseLeave(sender As Object, e As EventArgs) Handles btnLog.MouseLeave
            btnLog.ForeColor = Color.DarkSalmon
        End Sub

#End Region
#Region "Scintilla"
        Private Shared Function IsBrace(c As Integer) As Boolean
            Select Case ChrW(c)
                Case "("c, ")"c, "["c, "]"c, "{"c, "}"c,
            "<"c, ">"c
                    Return True
            End Select

            Return False
        End Function
        Private lastCaretPos As Integer = 0

        ' brace pairing
        Private Sub scintilla_UpdateUI(sender As Object, e As UpdateUIEventArgs) Handles tb.UpdateUI
            ' Has the caret changed position?
            Dim caretPos As Integer = tb.CurrentPosition
            If lastCaretPos <> caretPos Then
                lastCaretPos = caretPos
                Dim bracePos1 As Integer = -1
                Dim bracePos2 As Integer = -1

                ' Is there a brace to the left or right?
                If caretPos > 0 AndAlso IsBrace(tb.GetCharAt(caretPos - 1)) Then
                    bracePos1 = (caretPos - 1)
                ElseIf IsBrace(tb.GetCharAt(caretPos)) Then
                    bracePos1 = caretPos
                End If

                If bracePos1 >= 0 Then
                    ' Find the matching brace
                    bracePos2 = tb.BraceMatch(bracePos1)
                    If bracePos2 = Scintilla.InvalidPosition Then
                        tb.BraceBadLight(bracePos1)
                    Else
                        tb.BraceHighlight(bracePos1, bracePos2)
                    End If
                Else
                    ' Turn off brace matching
                    tb.BraceHighlight(Scintilla.InvalidPosition, Scintilla.InvalidPosition)
                End If
            End If
        End Sub

        ' syntax highlighting
        Private Sub tb_StyleNeeded(sender As Object, e As StyleNeededEventArgs) Handles tb.StyleNeeded
            Dim startPos As Integer = tb.GetEndStyled()
            Dim endPos As Integer = e.Position
            _cantusLexer.Style(tb, startPos, endPos)
        End Sub

        ' autoindent
        Private Sub tb_InsertCheck(sender As Object, e As InsertCheckEventArgs) Handles tb.InsertCheck
            If e.Text.EndsWith(ControlChars.Cr) OrElse e.Text.EndsWith(ControlChars.Lf) Then
                Dim curLine As Integer = tb.LineFromPosition(e.Position)
                Dim curLineText As String = tb.Lines(curLine).Text

                Dim indent As Match = Regex.Match(curLineText, "^\s*")
                e.Text += indent.Value


                Dim blockKwd As New HashSet(Of String)(("class function namespace if else elif for repeat " &
                                                "switch case run try catch finally while until with").Split(" "c))
                curLineText = curLineText.Trim()
                If curLineText.Contains(" ") Then curLineText = curLineText.Remove(curLineText.IndexOf(" "))

                If blockKwd.Contains(curLineText) Then e.Text += ControlChars.Tab

            End If
        End Sub

        Private Sub tb_CharAdded(sender As Object, e As CharAddedEventArgs) Handles tb.CharAdded

            ' autocomplete
            Dim currentPos As Integer = tb.CurrentPosition
            Dim wordStartPos As Integer = tb.CurrentPosition

            While wordStartPos - 1 >= 0 AndAlso (
                  tb.GetCharAt(wordStartPos - 1) >= AscW("0"c) AndAlso tb.GetCharAt(wordStartPos - 1) <= AscW("9"c) OrElse
                  tb.GetCharAt(wordStartPos - 1) >= AscW("a"c) AndAlso tb.GetCharAt(wordStartPos - 1) <= AscW("z"c) OrElse
                  tb.GetCharAt(wordStartPos - 1) >= AscW("A"c) AndAlso tb.GetCharAt(wordStartPos - 1) <= AscW("Z"c) OrElse
                  tb.GetCharAt(wordStartPos - 1) = AscW("_"c) OrElse tb.GetCharAt(wordStartPos - 1) = AscW("."c))
                wordStartPos -= 1
            End While

            If tb.GetCharAt(wordStartPos) = AscW("."c) Then wordStartPos += 1

            Dim enteredWord As String = tb.GetTextRange(wordStartPos, currentPos)

            Dim lenEntered As Integer = currentPos - wordStartPos

            If lenEntered > 0 Then

                Dim curLineText As String = tb.GetTextRange(tb.Lines(tb.CurrentLine).Position, tb.CurrentPosition)

                Dim singleQuote As Boolean = False
                Dim doubleQuote As Boolean = False
                For i As Integer = 0 To curLineText.Count - 1
                    If curLineText(i) = "'"c Then singleQuote = Not singleQuote
                    If curLineText(i) = """"c Then doubleQuote = Not doubleQuote
                Next
                If singleQuote OrElse doubleQuote Then Return ' do not autocomplete string

                curLineText = tb.Lines(tb.CurrentLine).Text
                Dim keyword As String = curLineText

                Dim blockKwd As String() = ("class function namespace").Split(" "c)
                keyword = keyword.Trim()
                If keyword.Contains(" ") Then keyword = keyword.Remove(keyword.IndexOf(" "))

                If blockKwd.Contains(keyword) Then Return ' do not autocomplete class, function, namespace names 

                ' do not autocomplete variable declarations unless after keyword
                If (keyword = "let" OrElse keyword = "global") AndAlso
                    Not curLineText.Contains("=") Then Return

                Dim keywords As String() = "function global let private public static".Split(" "c)

                Dim autoCList As New List(Of String)

                If keywords.Contains(keyword) AndAlso Not curLineText.Contains("=") Then
                    autoCList.AddRange(keywords)
                Else
                    Dim nsMode As Boolean = enteredWord.Contains(".")

                    If Not nsMode Then
                        autoCList.AddRange(("class function namespace if else elif for repeat return continue private public " &
                                   "let static global ref " &
                                   "switch case run try catch finally while until with in step to choose").Split(" "c))

                        autoCList.Add(ROOT_NAMESPACE)
                    End If

                    For Each v As Variable In _eval.Variables.Values
                        ' ignore private
                        If v.Modifiers.Contains("internal") OrElse (v.Modifiers.Contains("private") AndAlso
                        Not IsParentScopeOf(v.DeclaringScope, _eval.Scope)) Then Continue For

                        ' ignore null
                        If v.Value Is Nothing OrElse TypeOf v.Value Is Double AndAlso Double.IsNaN(CDbl(v.Value)) OrElse
                        TypeOf v.Value Is BigDecimal AndAlso
                            DirectCast(v.Value, BigDecimal).IsUndefined Then Continue For

                        If nsMode Then ' filter namespace
                            Dim partialName As String = RemoveRedundantScope(v.FullName, _eval.Scope)

                            If v.FullName.ToLower().StartsWith(enteredWord.ToLower()) Then
                                autoCList.Add(v.FullName)

                            ElseIf partialName.ToLower().StartsWith(enteredWord.ToLower()) Then
                                autoCList.Add(partialName.ToLower())

                            ElseIf enteredWord.ToLower().StartsWith(partialName.ToLower()) OrElse
                            enteredWord.ToLower().StartsWith(v.FullName.ToLower())
                                If enteredWord.ToLower().StartsWith(v.FullName.ToLower()) Then partialName = v.FullName
                                If TypeOf v.Reference.Resolve() Is ClassInstance Then
                                    Dim ci As ClassInstance = DirectCast(v.Reference.Resolve(), ClassInstance)
                                    For Each f As String In ci.Fields.Keys
                                        autoCList.Add(CombineScope(partialName,
                                                   f &
                                            If(TypeOf ci.Fields(f).ResolveObj() Is Lambda, "(" &
                                                  If(DirectCast(ci.Fields(f).ResolveObj(), Lambda).Args.Count = 0, "", "_") & ")",
                                              "")))
                                    Next
                                End If
                            Else
                                Continue For
                            End If
                        Else
                            autoCList.Add(RemoveRedundantScope(v.FullName, _eval.Scope) &
                                          If(TypeOf v.Value Is Lambda, "(" &
                                          If(DirectCast(v.Value, Lambda).Args.Count = 0, "", "_") & ")", ""))
                        End If
                    Next

                    ' internal functions
                    Dim info As MethodInfo() = GetType(InternalFunctions).GetMethods(
                        Reflection.BindingFlags.Public Or Reflection.BindingFlags.Instance Or
                        Reflection.BindingFlags.DeclaredOnly)

                    Dim varname As String = ""
                    Dim type As Type = Nothing
                    If nsMode AndAlso enteredWord.IndexOf(".") < enteredWord.Length AndAlso
                        _eval.HasVariable(enteredWord.Remove(enteredWord.IndexOf("."))) Then

                        varname = enteredWord.Remove(enteredWord.IndexOf("."))

                        Dim var As Object = Nothing
                        var = _eval.GetVariable(enteredWord.Remove(enteredWord.IndexOf(".")))
                        If TypeOf var Is Reference AndAlso Not TypeOf DirectCast(var, Reference).GetRefObject() Is Reference Then
                            type = DirectCast(var, Reference).Resolve().GetType
                        Else
                            type = var.GetType()
                        End If
                    End If

                    For Each fn As MethodInfo In info
                        If nsMode Then
                            If enteredWord.StartsWith("cantus") Then
                                autoCList.Add(ROOT_NAMESPACE & SCOPE_SEP & fn.Name.ToLower() & "(" &
                                              If(fn.GetParameters().Count = 0, "", "_") & ")")

                            ElseIf fn.GetParameters().Count > 0 AndAlso Not String.IsNullOrEmpty(varname) Then
                                If fn.GetParameters(0).ParameterType.IsAssignableFrom(type) Then
                                    autoCList.Add(varname & SCOPE_SEP & fn.Name.ToLower() & "(" &
                                          If(fn.GetParameters().Count <= 1, "", "_") & ")")
                                End If
                            End If
                        Else
                            autoCList.Add(fn.Name.ToLower() & "(" &
                                          If(fn.GetParameters().Count = 0, "", "_") & ")")
                        End If
                    Next

                    For Each fn As UserFunction In _eval.UserFunctions.Values
                        If nsMode Then
                            If Not fn.FullName.ToLower().StartsWith(enteredWord.ToLower()) AndAlso
                           Not fn.FullName.ToLower().StartsWith(RemoveRedundantScope(fn.FullName, _eval.Scope).ToLower()) Then
                                Continue For
                            End If
                        End If
                        ' ignore private
                        If fn.Modifiers.Contains("internal") OrElse (fn.Modifiers.Contains("private") AndAlso
                        Not IsParentScopeOf(fn.DeclaringScope, _eval.Scope)) Then Continue For

                        If nsMode Then ' filter namespace
                            If fn.FullName.ToLower().StartsWith(enteredWord.ToLower()) Then
                                autoCList.Add(fn.FullName & "(" &
                                          If(fn.Args.Count = 0, "", "_") & ")")
                            ElseIf RemoveRedundantScope(fn.FullName, _eval.Scope).ToLower().StartsWith(enteredWord.ToLower()) Then
                                autoCList.Add(RemoveRedundantScope(fn.FullName, _eval.Scope) & "(" &
                                          If(fn.Args.Count = 0, "", "_") & ")")
                            Else
                                Continue For
                            End If
                        Else
                            autoCList.Add(RemoveRedundantScope(fn.FullName, _eval.Scope) & "(" &
                                          If(fn.Args.Count = 0, "", "_") & ")")
                        End If
                    Next

                    For Each uc As UserClass In _eval.UserClasses.Values
                        If nsMode Then
                            If Not uc.FullName.ToLower().StartsWith(enteredWord.ToLower()) AndAlso
                               Not uc.FullName.ToLower().StartsWith(RemoveRedundantScope(uc.FullName, _eval.Scope).ToLower()) Then
                                Continue For
                            End If
                        End If
                        ' ignore private
                        If uc.Modifiers.Contains("internal") OrElse (uc.Modifiers.Contains("private") AndAlso
                            Not IsParentScopeOf(uc.DeclaringScope, _eval.Scope)) Then Continue For

                        If nsMode Then ' filter namespace
                            If uc.FullName.ToLower().StartsWith(enteredWord.ToLower()) Then
                                autoCList.Add(uc.FullName & "(" & If(uc.Constructor.Args.Count = 0, "", "_") & ")")
                            ElseIf RemoveRedundantScope(uc.FullName, _eval.Scope).ToLower().StartsWith(enteredWord.ToLower()) Then
                                autoCList.Add(RemoveRedundantScope(uc.FullName, _eval.Scope) & "(" &
                                              If(uc.Constructor.Args.Count = 0, "", "_") & ")")
                            Else
                                Continue For
                            End If
                        Else
                            autoCList.Add(RemoveRedundantScope(uc.FullName, _eval.Scope) & "(" &
                                          If(uc.Constructor.Args.Count = 0, "", "_") & ")")
                        End If
                    Next
                    autoCList.Sort(New AutoCompleteComparer())
                End If

                If autoCList.Count = 0 Then Return
                tb.AutoCShow(lenEntered, String.Join(" ", autoCList))
            End If

            ' brace completion
            If e.Char = AscW("(") OrElse e.Char = AscW("[") OrElse e.Char = AscW("{") OrElse
               e.Char = AscW(")") OrElse e.Char = AscW("]") OrElse e.Char = AscW("}") Then

                Dim startPos As Integer
                Dim curLine As Integer = tb.CurrentLine
                Dim curText As String = tb.Lines(curLine).Text

                While curLine > 0 AndAlso tb.Lines(curLine - 1).Text.EndsWith(" _") ' connect _
                    curLine -= 1
                    curText = tb.Lines(curLine).Text.Remove(tb.Lines(curLine).Text.Length - 2) & curText
                End While
                startPos = tb.Lines(curLine).Position

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
                        Dim len As Integer = tb.CurrentPosition - startPos
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

                        tb.InsertText(tb.Lines(tb.CurrentLine).Position + pos, endBr)
                    Else
                        tb.InsertText(tb.CurrentPosition, endBr)
                    End If
                End If

            ElseIf e.Char = AscW("|") OrElse e.Char = AscW(""""c) OrElse e.Char = AscW("'"c) Then

                Dim curText As String = tb.Lines(tb.CurrentLine).Text
                Dim ct As Boolean = False

                For i As Integer = 0 To curText.Length - 1
                    If curText(i) = ChrW(e.Char) Then ct = Not ct
                Next

                If ct Then tb.InsertText(tb.CurrentPosition, ChrW(e.Char))
            End If
        End Sub

        Private Sub tb_AutoCCompleted(sender As Object, e As AutoCSelectionEventArgs) Handles tb.AutoCCompleted
            If e.Text.EndsWith("(_)") Then
                tb.DeleteRange(tb.SelectionStart - 2, 1)
                tb.SelectionStart -= 1
                tb.SelectionEnd -= 1
            End If
        End Sub

        Private Sub TmrLoad_Tick(sender As Object, e As EventArgs) Handles TmrLoad.Tick
            SplashScreen.Progress.Value += 4
            If SplashScreen.Progress.Value < 100 Then Return
            TmrLoad.Stop()
            SplashScreen.Hide()

            ' set location
            If (My.Settings.MainPos <> "") Then
                Dim spl() As String = My.Settings.MainPos.Split(","c)
                Me.Location = New Point(CInt(spl(0)), CInt(spl(1)))
            Else
                Me.Left = CInt(Screen.PrimaryScreen.WorkingArea.Width / 2 - Me.Width / 2)
                Me.Top = CInt(Screen.PrimaryScreen.WorkingArea.Height / 4 - Me.Height / 3)
            End If

            ' save location
            My.Settings.MainPos = Me.Left & "," & Me.Top

            'set location for other forms
            Keyboards.KeyboardLeft.Show()
            Keyboards.KeyboardRight.Show()
            FrmViewer.InitPosition()
            Keyboards.KeyboardLeft.InitPosition()
            Keyboards.KeyboardRight.InitPosition()
        End Sub
#End Region
    End Class
End Namespace
