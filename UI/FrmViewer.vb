
Imports Cantus.UI.ScintillaForCantus
Imports ScintillaNET

Namespace UI
    Public Class FrmViewer
        ''' <summary>
        ''' Max number of lines to display in log
        ''' </summary>
        Private Const MAX_LINES As Integer = 500

        ''' <summary>
        ''' Represents a view in the viewer
        ''' </summary>
        Public Enum eView
            none = -1
            log = 0
            graphing = 1
        End Enum

        Private graphingControl As Graphing.GraphingSystem
        Private logControl As ScintillaNET.Scintilla

        Private _view As eView = eView.none
        Private _cantusLexer As New CantusLexer()

        ''' <summary>
        ''' Get or set the current view of the viewer
        ''' </summary>
        ''' <returns></returns>
        Public Property View As eView
            Get
                Return _view
            End Get
            Set(view As eView)
                If view = _view Then Return ' do not set
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
                    Case eView.log
                        pnl.Controls.Add(logControl)
                        logControl.Select()
                    Case eView.graphing
                        pnl.Controls.Add(graphingControl)
                        graphingControl.tb.Select()
                End Select
            End Set
        End Property

        ''' <summary>
        ''' If true, snaps to the editor on move
        ''' </summary>
        Public Property Snap As Boolean

        ''' <summary>
        ''' Write the specified text to the log, appending a separator before if appropriate
        ''' </summary>
        Public Sub WriteLogSection(text As String)
            If logControl.TextLength > 0 Then logControl.AppendText("".PadRight(63, "-"c) & vbLf)
            logControl.AppendText(text & vbLf)
            If logControl.Lines.Count >= MAX_LINES Then logControl.DeleteRange(0, logControl.Lines(MAX_LINES).EndPosition)
            logControl.SelectionStart = logControl.TextLength
            logControl.ScrollCaret()
        End Sub

        ''' <summary>
        ''' Write the specified text to the log
        ''' </summary>
        Public Sub WriteLog(text As String)
            logControl.AppendText(text & vbLf)
            If logControl.Lines.Count > MAX_LINES Then logControl.DeleteRange(0, logControl.Lines(MAX_LINES).Position)
            logControl.SelectionStart = logControl.TextLength
            logControl.ScrollCaret()
        End Sub

        Private Sub btnMin_Click(sender As Object, e As EventArgs) Handles btnMin.Click
            pnl.Focus()
            Me.WindowState = FormWindowState.Minimized
        End Sub

        ' form movement
        Dim _dragging As Boolean
        Dim _ppt As Point
        Private Sub FrmViewer_MouseDown(sender As Object, e As MouseEventArgs) Handles MyBase.MouseDown, lbTitle.MouseDown
            _dragging = True
            _ppt = e.Location
        End Sub

        Private Sub lbTitle_MouseMove(sender As Object, e As MouseEventArgs) Handles MyBase.MouseMove, lbTitle.MouseMove
            If _dragging Then
                Me.Left += e.X - _ppt.X
                Me.Top += e.Y - _ppt.Y
                If Snap Then
                    FrmEditor.Left = Me.Right
                    FrmEditor.Top = Me.Top
                    If FrmEditor.LSnap Then
                        Keyboards.KeyboardLeft.Top = FrmEditor.Bottom
                        Keyboards.KeyboardLeft.Left = FrmEditor.Left
                    End If
                    If FrmEditor.RSnap Then
                        Keyboards.KeyboardRight.Top = FrmEditor.Bottom
                        Keyboards.KeyboardRight.Left = FrmEditor.Right - Keyboards.KeyboardRight.Width
                    End If
                End If
            End If
        End Sub

        Private Sub lbTitle_MouseUp(sender As Object, e As MouseEventArgs) Handles MyBase.MouseUp, lbTitle.MouseUp
            _dragging = False
            My.Settings.ViewerPos = Me.Left & "," & Me.Top
            If e.Button = MouseButtons.Right Then
                If Snap Then
                    Me.Left -= 6
                    Snap = False
                    My.Settings.VSnap = False
                    Me.ShowInTaskbar = True
                    btnMin.Show()
                End If
            Else
                If Me.Right > FrmEditor.Left - 40 AndAlso Me.Left < FrmEditor.Right + 40 AndAlso
                    (Me.Bottom > FrmEditor.Top AndAlso Me.Top < FrmEditor.Bottom) Then
                    Snap = True
                    My.Settings.VSnap = True
                    Me.Left = FrmEditor.Left - Me.Width
                    Me.Top = FrmEditor.Top

                    Me.ShowInTaskbar = False
                    btnMin.Hide()
                End If
            End If
            My.Settings.Save()
        End Sub

        Private Sub btnTabs_Click(sender As Object, e As EventArgs)
            pnl.Focus()
            Me.Text = DirectCast(sender, Button).Text & " - Cantus"
            Me.View = DirectCast([Enum].Parse(GetType(eView), DirectCast(sender, Button).Tag.ToString()), eView)
        End Sub

        Private Sub SetupScintilla(tb As Scintilla)
            Dim backColor As Color = Color.FromArgb(37, 37, 37)
            tb.CaretForeColor = backColor

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

            tb.Styles(CantusLexer.StyleNumber).ForeColor = Color.FromArgb(255, 205, 34)
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
            Me.Icon = My.Resources.Calculator
            Me.Snap = My.Settings.VSnap

            For Each c As Control In Me.Controls
                If TypeOf c Is Button AndAlso Not c.Tag Is Nothing Then
                    AddHandler DirectCast(c, Button).Click, AddressOf btnTabs_Click
                End If
            Next
            AddHandler Me.KeyDown, AddressOf Control_KeyDown
            lbTitle.Text = lbTitle.Text.Replace("{VER}", Application.ProductVersion.ToString())

            graphingControl = New Graphing.GraphingSystem
            graphingControl.Dock = DockStyle.Fill
            AddHandler graphingControl.KeyDown, AddressOf Control_KeyDown
            AddHandler graphingControl.tb.KeyDown, AddressOf Control_KeyDown

            logControl = New Scintilla
            logControl.Dock = DockStyle.Fill
            logControl.BorderStyle = BorderStyle.None

            SetupScintilla(logControl)
            AddHandler logControl.KeyDown, AddressOf Control_KeyDown
            AddHandler logControl.KeyUp, AddressOf LogControl_KeyUp
            AddHandler logControl.KeyPress, AddressOf LogControl_KeyPress
            AddHandler logControl.StyleNeeded, AddressOf LogControl_StyleNeeded

            Me.View = eView.log
            WriteLogSection(
                String.Format(vbLf & "# Welcome to Cantus version {0}!" & vbLf &
                "# Press Alt + Enter or click the ''Run & Record'' button on the" & vbLf & "# bottom right " &
                "of the editor to record an answer in My Results.",
                                   Application.ProductVersion.ToString))

            pnl.Select()
        End Sub
        Public Sub InitPosition()
            If My.Settings.ViewerPos <> "" AndAlso Not Me.Snap Then
                Dim spl() As String = My.Settings.ViewerPos.Split(","c)
                Me.Location = New Point(CInt(spl(0)), CInt(spl(1)))
            Else
                Me.Left = FrmEditor.Left - Me.Width
                Me.Top = FrmEditor.Top
                My.Settings.ViewerPos = Me.Left & "," & Me.Top
                My.Settings.Save()
            End If
        End Sub

        Dim allowKey As Boolean = False
        Private Sub Control_KeyDown(sender As Object, e As KeyEventArgs)
            If e.Alt Then
                If e.KeyCode = Keys.G Then
                    btnGraph.PerformClick()
                ElseIf e.KeyCode = Keys.R Then
                    btnLog.PerformClick()
                ElseIf e.KeyCode = Keys.E Then
                    FrmEditor.BringToFront()
                    FrmEditor.tb.Select()
                ElseIf e.KeyCode = Keys.S Then
                    FrmEditor.BringToFront()
                    FrmEditor.btnSettings.PerformClick()
                ElseIf e.KeyCode = Keys.F Then
                    FrmEditor.BringToFront()
                    FrmEditor.btnFunctions.PerformClick()
                ElseIf e.KeyCode = Keys.T Then
                    FrmEditor.BtnTranslucent.PerformClick()
                    DirectCast(sender, Control).Focus()
                End If
            End If
        End Sub
        Private Sub LogControl_KeyDown(sender As Object, e As KeyEventArgs)
            If e.Control AndAlso (e.KeyCode = Keys.A OrElse e.KeyCode = Keys.C) Then
                allowKey = True
            Else
                allowKey = False
                e.SuppressKeyPress = True
            End If
        End Sub
        Private Sub LogControl_KeyUp(sender As Object, e As KeyEventArgs)
            e.SuppressKeyPress = True
        End Sub
        Private Sub LogControl_KeyPress(sender As Object, e As KeyPressEventArgs)
            If allowKey Then
                allowKey = False
            Else
                e.Handled = True
            End If
        End Sub

        ' syntax highlighting
        Private Sub LogControl_StyleNeeded(sender As Object, e As StyleNeededEventArgs)
            Dim startPos As Integer = logControl.GetEndStyled()
            Dim endPos As Integer = e.Position
            _cantusLexer.Style(logControl, startPos, endPos)
        End Sub

        Private Sub FrmViewer_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
            If FrmEditor.Visible Then
                FrmEditor.Close()
            End If
        End Sub
    End Class
End Namespace