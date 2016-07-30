Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Threading
Imports Cantus.Calculator.Evaluator
Imports Cantus.Calculator.Evaluator.CommonTypes

Namespace Calculator
    Public Class FrmCalc
#Region "Declarations"
        ''' <summary>
        ''' Maximum length of text to display on the tooltip
        ''' </summary>
        Private Const TT_LEN_LIMIT As Integer = 500
        Private Const PREV_EXPRESSION_LIMIT As Integer = 15

        Private Const RELEASE_TYPE As String = "Alpha"

        ' the main evaluator
        Private _eval As Evaluator.Evaluator

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

        ' Win32 API to change tab width in textbox
        Const EM_SETTABSTOPS As Integer = &HCB
        <DllImport("user32.dll")>
        Private Shared Function SendMessageA(ByVal TBHandle As IntPtr,
                                              ByVal EM_SETTABSTOPS As Integer,
                                              ByVal wParam As Integer,
                                              ByRef lParam As Integer) As Boolean
        End Function

#End Region
#Region "Form Events"
        Private Sub FrmCalc_Load(sender As Object, e As EventArgs) Handles MyBase.Load

            ' new version? upgrade settings from previous version + show message
            If My.Settings.ReqUpgrade Then
                My.Settings.Upgrade()
                My.Settings.ReqUpgrade = False
                Me.Hide()
                _displayUpdateMessage = True
            End If

            ' icon
            Me.Icon = My.Resources.Calculator

            ' set location
            If (My.Settings.MainPos <> "") Then
                Dim spl() As String = My.Settings.MainPos.Split(","c)
                Me.Location = New Point(CInt(spl(0)), CInt(spl(1)))
                If Not (Me.Left >= -100 And Me.Top >= -300 And Me.Right <= Screen.PrimaryScreen.WorkingArea.Width + 100 And
                Me.Bottom <= Screen.PrimaryScreen.WorkingArea.Height) Then
                    Me.Left = CInt(Screen.PrimaryScreen.WorkingArea.Width / 2 - Me.Width / 2)
                    Me.Top = CInt(Screen.PrimaryScreen.WorkingArea.Height / 4 - Me.Height / 3)
                End If
            Else
                Me.Left = CInt(Screen.PrimaryScreen.WorkingArea.Width / 2 - Me.Width / 2)
                Me.Top = CInt(Screen.PrimaryScreen.WorkingArea.Height / 4 - Me.Height / 3)
            End If

            ' setup evaluator
            Me._eval = Evaluator.Globals.Evaluator
            AddHandler Evaluator.Globals.Evaluator.EvalComplete, AddressOf EvalComplete

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

            ' delay remaining tasks to avoid flickering
            TmrLoad.Start()

            def = def.Trim().Trim({ControlChars.Quote, "'"c})
            If opengraph Then
                Graphing.FrmGraph.tb.Text = "0"
                Graphing.FrmGraph.Show()
                Graphing.FrmGraph.BringToFront()
                Graphing.FrmGraph.btnGraph.PerformClick()
                btnMin.PerformClick()
                Graphing.FrmGraph.tb.Text = def
                Graphing.FrmGraph.tb.Focus()
                Graphing.FrmGraph.tb.SelectAll()
            Else
                tb.Text = def
            End If
        End Sub

        Dim ct As Integer = 0
        Public Sub TmrLoad_Tick(sender As Object, e As EventArgs) Handles TmrLoad.Tick
            TmrLoad.Stop()

            ' setup keyboards
            Me.RSnap = My.Settings.ROskSnap
            Me.LSnap = My.Settings.LOskSnap
            Me.BringToFront()

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
                Using diag As New DiagFeatureList(
                 My.Resources.UpdateMsg.Replace("{ver}", Application.ProductVersion))
                    diag.ShowDialog()
                End Using
                Me.Show()
            End If

            ' save location
            My.Settings.MainPos = Me.Left & "," & Me.Top

            ' update tooltips
            UpdateLetterTT()

            ' set up UI
            pnlSettings.BackColor = Color.FromArgb(50, 30, 30, 30)
            cbAutoUpd.Checked = My.Settings.AutoUpdate
            lbAbout.Text = lbAbout.Text.Replace("{VER}", Application.ProductVersion & " " & RELEASE_TYPE)

            ' defaults
            Me.SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.OptimizedDoubleBuffer, True)

            ' change tab size
            Dim TabStop As Integer = 8
            SendMessageA(tb.Handle, EM_SETTABSTOPS, 1, TabStop)

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
        Private Sub FrmCalc_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
            Try
                SaveSettings()
                _eval.Dispose()
                Thread.Sleep(250)
            Catch
            End Try
        End Sub
        Private Sub FrmCalc_MouseUp(sender As Object, e As MouseEventArgs) Handles MyBase.MouseUp, tb.MouseUp, lbResult.MouseUp, pnlTb.MouseUp
            tb.Focus()
            pnlSettings.Hide()
        End Sub
        Private Sub FrmCalc_Leave(sender As Object, e As EventArgs) Handles MyBase.Deactivate
            pnlSettings.Hide()
        End Sub
        Private Sub FrmCalc_SizeChanged(sender As Object, e As EventArgs) Handles MyBase.SizeChanged
            If Me.WindowState = FormWindowState.Normal AndAlso My.Settings.ShowKbd Then
                If LSnap Then Keyboards.OskLeft.Show()
                If RSnap Then Keyboards.OskRight.Show()
            End If
        End Sub
        Private Sub tb_KeyUp(sender As Object, e As KeyEventArgs) Handles tb.KeyUp
            Try
                If e.KeyCode = Keys.Enter And e.Alt Then
                    EvaluateExpr()
                ElseIf e.KeyCode = Keys.F12
                    Using diag As New SaveFileDialog()
                        diag.Filter = "Cantus Script (.can)|*.can"
                        diag.RestoreDirectory = True
                        diag.Title = "Save To Script"
                        If diag.ShowDialog = DialogResult.OK Then
                            IO.File.WriteAllText(diag.FileName, tb.Text, System.Text.Encoding.UTF8)
                        End If
                    End Using

                ElseIf e.KeyCode = Keys.F11
                    Using diag As New OpenFileDialog()
                        diag.Filter = "Cantus Script (.can)|*.can"
                        diag.RestoreDirectory = True
                        diag.Multiselect = False
                        diag.Title = "Load Script"
                        If diag.ShowDialog = DialogResult.OK Then
                            tb.Text = IO.File.ReadAllText(diag.FileName).Replace(vbCrLf, vbLf).Replace(vbCr, vbLf).
                                Replace(vbLf, vbNewLine) ' fix line endings
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
                ElseIf e.KeyCode = Keys.V AndAlso e.Control AndAlso e.Alt AndAlso e.Shift
                    Dim sb As New StringBuilder
                    For Each v As Evaluator.Evaluator.Variable In _eval.Variables.Values
                        sb.Append(v.ToString).Append(vbNewLine)
                    Next
                    My.Computer.Clipboard.SetText(sb.ToString)
                End If
            Catch ex As Exception
                MsgBox(ex.Message, MsgBoxStyle.Exclamation Or MsgBoxStyle.MsgBoxSetForeground, "File Read/Save Operation Failed")
            End Try
        End Sub

        Private Sub FrmCalc_KeyUp(sender As Object, e As KeyEventArgs) Handles MyBase.KeyUp
            If e.Control Then
                If e.KeyCode = Keys.P Then
                    btnAngleRep_Click(btnAngleRepr, New EventArgs)
                ElseIf e.KeyCode = Keys.D OrElse e.KeyCode = Keys.R OrElse e.KeyCode = Keys.G Then
                    If e.KeyCode = Keys.D Then
                        _eval.AngleMode = Evaluator.Evaluator.eAngleRepresentation.Degree
                    ElseIf e.KeyCode = Keys.R
                        _eval.AngleMode = Evaluator.Evaluator.eAngleRepresentation.Radian
                    Else
                        _eval.AngleMode = Evaluator.Evaluator.eAngleRepresentation.Gradian
                    End If
                    btnAngleRepr.Text = _eval.AngleMode.ToString()
                    EvaluateExpr()
                ElseIf e.KeyCode = Keys.O Then
                    btnOMode_Click(btnOutputFormat, New EventArgs)
                ElseIf e.KeyCode = Keys.M OrElse e.KeyCode = Keys.S OrElse e.KeyCode = Keys.L Then
                    If e.KeyCode = Keys.M Then
                        _eval.OutputFormat = Evaluator.Evaluator.eOutputFormat.Math
                        e.SuppressKeyPress = True
                    ElseIf e.KeyCode = Keys.L
                        _eval.OutputFormat = Evaluator.Evaluator.eOutputFormat.Raw
                    Else
                        _eval.OutputFormat = Evaluator.Evaluator.eOutputFormat.Scientific
                    End If
                    btnOutputFormat.Text = _eval.OutputFormat.ToString()
                    EvaluateExpr()
                ElseIf e.KeyCode = Keys.T Then
                    btnExplicit.PerformClick()
                End If
            End If
        End Sub

        Private Sub tb_KeyPress(sender As Object, e As KeyPressEventArgs) Handles tb.KeyPress
            ' auto-insert brackets
            If e.KeyChar = "(" OrElse e.KeyChar = "[" OrElse e.KeyChar = "{" Then
                Dim endSign As Char = ")"c
                If e.KeyChar = "["c Then
                    endSign = "]"c
                ElseIf e.KeyChar = "{"c
                    endSign = "}"c
                End If

                Dim start As Integer = tb.SelectionStart
                Dim ct As Integer = 0

                For Each c As Char In tb.Text
                    If c = e.KeyChar Then
                        ct += 1
                    ElseIf c = endSign
                        ct -= 1
                    End If
                Next

                tb.Text = tb.Text.Remove(tb.SelectionStart, tb.SelectionLength).Insert(start, e.KeyChar)
                If ct >= 0 Then
                    tb.Text = tb.Text.Insert(start + 1, endSign)
                End If
                tb.SelectionStart = start + 1
                tb.ScrollToCaret()
                e.Handled = True

            ElseIf e.KeyChar = ")" OrElse e.KeyChar = "]" OrElse e.KeyChar = "}"
                tb.Focus()
                Dim openBr As Char = "("c
                Dim closeBr As Char = e.KeyChar

                If e.KeyChar = "]" Then
                    openBr = "["c
                ElseIf e.KeyChar = "}" Then
                    openBr = "{"c
                End If

                Dim start As Integer = tb.SelectionStart
                tb.Text = tb.Text.Remove(tb.SelectionStart, tb.SelectionLength)

                Dim insertStart As Boolean = True
                For i As Integer = Math.Min(start, tb.Text.Length - 1) To 0 Step -1
                    If tb.Text(i) = closeBr Then
                        Exit For
                    ElseIf tb.Text(i) = openBr
                        insertStart = False
                        Exit For
                    End If
                Next
                If insertStart Then
                    For i As Integer = start To -1 Step -1
                        If i = -1 OrElse
                            tb.Text.Length > i AndAlso (tb.Text(i) = ControlChars.Lf OrElse tb.Text(i) = ControlChars.Cr) Then
                            tb.Text = tb.Text.Insert(i + 1, openBr)
                            Exit For
                        End If
                    Next
                    tb.Text = tb.Text.Insert(start + 1, closeBr)
                Else
                    tb.Text = tb.Text.Insert(start, closeBr)
                End If
                tb.Select(start + 2, 0)
                tb.ScrollToCaret()
                e.Handled = True
            End If
        End Sub
#End Region
#Region "Shared functions"
        ''' <summary>
        ''' Send the event to asynchroneously evalute the expression
        ''' </summary>
        Private Sub EvaluateExpr()
            tb.Focus()
            lbResult.Text = "="

            ' evaluae
            _eval.EvalAsync(tb.Text)
        End Sub

        Private Sub EvalComplete(sender As Object, result As Object)
            If lbResult.InvokeRequired Then
                lbResult.BeginInvoke(Sub() EvalComplete(sender, result))
            Else
                Dim ans As String = result.ToString()

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

                ' display answer
                lbResult.Text = AutoTrimDisplayText(ans)

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
        Private Sub btnCalc_click(sender As Object, e As System.EventArgs) Handles btnCalc.Click
            EvaluateExpr()
        End Sub

        Private Sub btnGraph_click(sender As Object, e As EventArgs) Handles btnGraph.Click
            ' open graphing window
            tb.Focus()
            If Graphing.FrmGraph.Visible Then
                Graphing.FrmGraph.BringToFront()
            Else
                Graphing.FrmGraph.Show()
            End If
            btnMin.PerformClick()
        End Sub

#Region "textbox & labels"

        Private Sub tb_keydown(sender As Object, e As KeyEventArgs) Handles tb.KeyDown
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
                End If
            End If

            If e.Control Then
                If e.KeyCode = Keys.A Then
                    tb.SelectAll()
                    e.SuppressKeyPress = True
                ElseIf e.KeyCode = Keys.E Then
                    Keyboards.OskRight.btnPow10.PerformClick()
                    e.SuppressKeyPress = True
                ElseIf e.KeyCode = Keys.M
                    e.SuppressKeyPress = True
                End If
            End If

        End Sub

        Private Sub lbResult_textchanged(sender As Object, e As EventArgs) Handles lbResult.TextChanged
            Keyboards.OskRight.lbResult.Text = lbResult.Text
        End Sub
#End Region
#End Region
#Region "memory ui"
        Private Sub btnSettings_click(sender As Object, e As EventArgs) Handles btnSettings.Click
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
                EvaluateExpr()
                pnlSettings.Hide()
            End If
        End Sub

        Private Sub btnLetters_click(sender As Object, e As EventArgs) Handles btnY.Click, btnX.Click, btnT.Click, btnM.Click
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
                If c.Tag.ToString() = "-" Then Continue For

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
                    If FileIO.FileSystem.FileExists(Application.StartupPath & "\calculator.backup") Then FileIO.FileSystem.DeleteFile(Application.StartupPath & "\calculator.backup")
                Catch 'ex2 As Exception
                End Try

                Try
                    FileIO.FileSystem.RenameFile(Application.ExecutablePath, "calculator.backup")
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
            Keyboards.OskLeft.Visible = False
            Keyboards.OskRight.Visible = False
            Me.Visible = False
            Using fup As New Updater.FrmUpdate
                Updater.FrmUpdate.ShowDialog()
                Updater.FrmUpdate.BringToFront()
                Keyboards.OskLeft.SendToBack()
                Keyboards.OskRight.SendToBack()
            End Using
        End Sub
#End Region
#Region "Command Buttons & Aesthetic"
        Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
            Me.Close()
        End Sub

        Private Sub btnMin_Click(sender As Object, e As EventArgs) Handles btnMin.Click
            Me.WindowState = FormWindowState.Minimized
            If LSnap Then Keyboards.OskLeft.Hide()
            If RSnap Then Keyboards.OskRight.Hide()
        End Sub

        Private Sub btnMem_Enter(sender As Object, e As EventArgs) Handles btnGraph.Enter, btnSettings.Enter, btnCalc.Enter, btnMin.Enter, btnClose.Enter
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
                If e.Y > 0 Then
                    Me.Left = newLft
                    Me.Top = newTop
                    If LSnap Then
                        Keyboards.OskLeft.Left = newLft
                        Keyboards.OskLeft.Top = newTop + Me.Height - 1
                    End If
                    If RSnap Then
                        Keyboards.OskRight.Left = newLft + Me.Width - Keyboards.OskRight.Width
                        Keyboards.OskRight.Top = newTop + Me.Height - 1
                    End If
                Else
                    If LSnap Then
                        Keyboards.OskLeft.Left = newLft
                        Keyboards.OskLeft.Top = newTop + Me.Height - 1
                    End If
                    If RSnap Then
                        Keyboards.OskRight.Left = newLft + Me.Width - Keyboards.OskRight.Width
                        Keyboards.OskRight.Top = newTop + Me.Height - 1
                    End If
                    Me.Left = newLft
                    Me.Top = newTop
                    Keyboards.OskLeft.BringToFront()
                    Keyboards.OskRight.BringToFront()
                End If
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
                    My.Settings.Save()
                End If
            Else
                _isMoving = False
                If Me.Bottom > Keyboards.OskRight.Top AndAlso Me.Bottom < Keyboards.OskRight.Bottom AndAlso Me.Left > Keyboards.OskRight.Left - 400 AndAlso Me.Right < Keyboards.OskRight.Right + 400 AndAlso Not RSnap Then
                    Me.Left = Keyboards.OskRight.Right - Me.Width
                    Me.Top = Keyboards.OskRight.Top - Me.Height
                    Me.RSnap = True
                    If LSnap Then
                        Keyboards.OskLeft.Left = Me.Left
                        Keyboards.OskLeft.Top = Me.Bottom
                    End If
                End If
                If Me.Bottom >= Keyboards.OskLeft.Top AndAlso Me.Bottom < Keyboards.OskLeft.Bottom AndAlso (Me.Left > Keyboards.OskLeft.Left - 400 AndAlso Me.Right < Keyboards.OskLeft.Right + 400) AndAlso Not LSnap Then
                    Me.Left = Keyboards.OskLeft.Left
                    Me.Top = Keyboards.OskLeft.Top - Me.Height
                    Me.LSnap = True
                    If RSnap Then
                        Keyboards.OskRight.Left = Me.Right - Keyboards.OskRight.Width
                        Keyboards.OskRight.Top = Me.Bottom
                    End If
                End If
                If LSnap AndAlso RSnap Then
                    Keyboards.OskRight.Snap = True
                    Keyboards.OskLeft.Snap = True
                    My.Settings.OskLock = True
                End If
                My.Settings.MainPos = Me.Left & "," & Me.Top
                My.Settings.LeftKbdPos = Keyboards.OskLeft.Left & "," & Keyboards.OskLeft.Top
                My.Settings.RightKbdPos = Keyboards.OskRight.Left & "," & Keyboards.OskRight.Top
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
            EvaluateExpr()
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
            EvaluateExpr()
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
            Using diag As New DiagFeatureList(
                 My.Resources.UpdateMsg.Replace("{ver}", Application.ProductVersion))
                diag.ShowDialog()
            End Using
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
        Private Sub FrmCalc_LocationChanged(sender As Object, e As EventArgs) Handles Me.LocationChanged
            If Me.WindowState = FormWindowState.Maximized Then Me.WindowState = FormWindowState.Normal
            If Keyboards.OskRight.WindowState = FormWindowState.Maximized Then
                Keyboards.OskRight.WindowState = FormWindowState.Normal
                If RSnap Then
                    Keyboards.OskRight.Left = Me.Right - Keyboards.OskRight.Width
                    Keyboards.OskRight.Top = Me.Bottom
                End If
            End If
            If Keyboards.OskLeft.WindowState = FormWindowState.Maximized Then
                Keyboards.OskLeft.WindowState = FormWindowState.Normal
                If LSnap Then
                    Keyboards.OskLeft.Left = Me.Left
                    Keyboards.OskLeft.Top = Me.Bottom
                End If
            End If
        End Sub

        Private Sub tb_TextChanged(sender As Object, e As EventArgs) Handles tb.TextChanged
            ct = 2
            TmrReCalc.Start()
        End Sub

        Private Sub TmrReCalc_Tick(sender As Object, e As EventArgs) Handles TmrReCalc.Tick
            If ct <= 0 Then
                TmrReCalc.Stop()
                btnCalc.PerformClick()
            End If
            ct -= 1
        End Sub

        Private Sub btnExplicit_Click(sender As Object, e As EventArgs) Handles btnExplicit.Click
            _eval.ExplicitMode = Not _eval.ExplicitMode
            If _eval.ExplicitMode Then
                btnExplicit.BackColor = btnCalc.BackColor
                btnExplicit.ForeColor = btnCalc.ForeColor
                btnExplicit.FlatAppearance.MouseOverBackColor = btnCalc.FlatAppearance.MouseOverBackColor
                btnExplicit.FlatAppearance.MouseDownBackColor = btnCalc.FlatAppearance.MouseDownBackColor
            Else
                btnExplicit.BackColor = btnX.BackColor
                btnExplicit.ForeColor = btnX.ForeColor
                btnExplicit.FlatAppearance.MouseOverBackColor = btnX.FlatAppearance.MouseOverBackColor
                btnExplicit.FlatAppearance.MouseDownBackColor = btnX.FlatAppearance.MouseDownBackColor
            End If
        End Sub

        Private Sub btnFunctions_Click(sender As Object, e As EventArgs) Handles btnFunctions.Click
            tb.Focus()
            Using diag As New DiagFunctions
                If diag.ShowDialog() = DialogResult.OK Then
                    Dim start As Integer = tb.SelectionStart
                    tb.Text = tb.Text.Remove(
                                tb.SelectionStart, tb.SelectionLength).Insert(start, diag.Result)

                    If diag.Result.Contains("(") Then
                        tb.SelectionStart = start + diag.Result.IndexOf("(") + 1
                    Else
                        tb.SelectionStart = start + diag.Result.Count
                    End If
                End If
            End Using
        End Sub

        Private Sub btnLog_MouseEnter(sender As Object, e As EventArgs) Handles btnLog.MouseEnter
            btnLog.ForeColor = Color.LightSalmon
        End Sub

        Private Sub btnLog_MouseLeave(sender As Object, e As EventArgs) Handles btnLog.MouseLeave
            btnLog.ForeColor = Color.DarkSalmon
        End Sub

#End Region
    End Class
End Namespace
