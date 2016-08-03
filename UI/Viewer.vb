
Imports Cantus.UI.ScintillaForCantus
Imports ScintillaNET

Namespace UI
    Public Class Viewer
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

        Friend GraphingControl As Graphing.GraphingSystem
        Friend LogControl As ScintillaNET.Scintilla

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
                        pnl.Controls.Add(LogControl)
                        LogControl.Select()
                    Case eView.graphing
                        pnl.Controls.Add(GraphingControl)
                        GraphingControl.tb.Select()
                End Select
            End Set
        End Property

        Public Sub New()
            ' This call is required by the designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
        End Sub

        ''' <summary>
        ''' Write the specified text to the log, appending a separator before if appropriate
        ''' </summary>
        Public Sub WriteLogSection(text As String)
            If LogControl.TextLength > 0 Then
                If LogControl.GetCharAt(LogControl.TextLength - 1) <> AscW(ControlChars.Lf) Then
                    LogControl.AppendText(vbLf)
                End If
                LogControl.AppendText("".PadRight(63, "-"c) & vbLf)
            End If
            LogControl.AppendText(text & vbLf)
            If LogControl.Lines.Count >= MAX_LINES Then LogControl.DeleteRange(0, LogControl.Lines(MAX_LINES).EndPosition)
            LogControl.SelectionStart = LogControl.TextLength
            LogControl.ScrollCaret()
        End Sub

        ''' <summary>
        ''' Write the specified text followed by a line break to the log
        ''' </summary>
        Public Sub WriteLogLine(text As String)
            If LogControl.GetCharAt(LogControl.TextLength - 1) <> AscW(ControlChars.Lf) Then
                LogControl.AppendText(vbLf)
            End If
            LogControl.AppendText(text & vbLf)
            If LogControl.Lines.Count > MAX_LINES Then LogControl.DeleteRange(0, LogControl.Lines(MAX_LINES).Position)
            LogControl.SelectionStart = LogControl.TextLength
            LogControl.ScrollCaret()
        End Sub

        ''' <summary>
        ''' Write the specified text to the log
        ''' </summary>
        ''' <param name="text"></param>
        Public Sub WriteLog(text As String)
            LogControl.AppendText(text)
            If LogControl.Lines.Count > MAX_LINES Then LogControl.DeleteRange(0, LogControl.Lines(MAX_LINES).Position)
            LogControl.SelectionStart = LogControl.TextLength
            LogControl.ScrollCaret()
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
            For Each c As Control In Me.Controls
                If TypeOf c Is Button AndAlso Not c.Tag Is Nothing Then
                    AddHandler DirectCast(c, Button).Click, AddressOf btnTabs_Click
                End If
            Next
            AddHandler Me.KeyDown, AddressOf Control_KeyDown
            lbTitle.Text = lbTitle.Text.Replace("{VER}", Application.ProductVersion.ToString())

            GraphingControl = New Graphing.GraphingSystem
            GraphingControl.Dock = DockStyle.Fill
            AddHandler GraphingControl.KeyDown, AddressOf Control_KeyDown
            AddHandler GraphingControl.tb.KeyDown, AddressOf Control_KeyDown

            LogControl = New Scintilla
            LogControl.Dock = DockStyle.Fill
            LogControl.BorderStyle = BorderStyle.None

            SetupScintilla(LogControl)
            AddHandler LogControl.KeyDown, AddressOf Control_KeyDown
            AddHandler LogControl.KeyUp, AddressOf LogControl_KeyUp
            AddHandler LogControl.KeyPress, AddressOf LogControl_KeyPress
            AddHandler LogControl.StyleNeeded, AddressOf LogControl_StyleNeeded

            Me.View = eView.log
            WriteLogSection(
                String.Format(vbLf & "# Welcome to Cantus version {0}!" & vbLf &
                "# Everything you print with print() will appear in this console." & vbLf &
                "# Press Alt + Enter or click the ''Run & Record'' button on the" & vbLf & "# bottom right " &
                "of the editor to print out the current result.",
                                   Application.ProductVersion.ToString))

            pnl.Select()
        End Sub

        Dim allowKey As Boolean = False
        Private Sub Control_KeyDown(sender As Object, e As KeyEventArgs)
            If e.Alt Then
                If e.KeyCode = Keys.G Then
                    btnGraph.PerformClick()
                ElseIf e.KeyCode = Keys.C Then
                    btnLog.PerformClick()
                ElseIf e.KeyCode = Keys.E Then
                    FrmEditor.BringToFront()
                    FrmEditor.Tb.Select()
                ElseIf e.KeyCode = Keys.S Then
                    FrmEditor.BringToFront()
                    FrmEditor.BtnSettings.PerformClick()
                ElseIf e.KeyCode = Keys.F Then
                    FrmEditor.BringToFront()
                    FrmEditor.BtnFunctions.PerformClick()
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
            Dim startPos As Integer = LogControl.GetEndStyled()
            Dim endPos As Integer = e.Position
            _cantusLexer.Style(LogControl, startPos, endPos)
        End Sub
    End Class
End Namespace