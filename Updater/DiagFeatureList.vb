Namespace UI.Dialogs
    Public Class DiagFeatureList
        Public Sub New()
            ' This call is required by the designer.
            InitializeComponent()
        End Sub

        Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
            Me.Close()
        End Sub

        Private Sub DiagFeatureList_Load(sender As Object, e As EventArgs) Handles MyBase.Load
            Me.Opacity = 0
            Me.Icon = My.Resources.Calculator
            tb.Text = My.Resources.UpdateMsg.Replace("{ver}", Application.ProductVersion).Replace(vbLf, vbCrLf)
            tb.SelectionStart = 0
            Dim cCode As String = Globalization.CultureInfo.CurrentCulture.Name

            If cCode.StartsWith("en") AndAlso cCode <> "en-US" Then btnLicense.Text = "Licence" ' spelling for non-american english...
            TmrAnim.Start()
        End Sub

        Dim ctrl As Boolean = False

        Private Sub tb_KeyPress(sender As Object, e As KeyPressEventArgs) Handles tb.KeyPress
            If Not ctrl Then e.Handled = True
        End Sub

        Private Sub tb_KeyDown(sender As Object, e As KeyEventArgs) Handles tb.KeyDown
            If e.Control Then
                If e.KeyCode = Keys.A Then
                    tb.SelectAll()
                ElseIf e.KeyCode = Keys.C
                    ctrl = True
                End If
            End If
        End Sub

        Private Sub tb_KeyUp(sender As Object, e As KeyEventArgs) Handles tb.KeyUp
            ctrl = False
        End Sub

        Private Sub SwitchTab(btnName As String)
            Dim curBtnColor As Color = Color.FromArgb(70, 70, 70)
            Dim backBtnColor As Color = Color.FromArgb(55, 55, 55)
            For Each c As Control In pnlNote.Controls
                If TypeOf c Is Button AndAlso Not c.Tag Is Nothing Then
                    Dim btn As Button = DirectCast(c, Button)
                    If btn.Tag.ToString = btnName Then
                        btn.BackColor = curBtnColor
                        btn.FlatAppearance.MouseOverBackColor = curBtnColor
                        btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(85, 85, 85)
                    Else
                        btn.BackColor = backBtnColor
                        btn.FlatAppearance.MouseOverBackColor = backBtnColor
                        btn.FlatAppearance.MouseDownBackColor = curBtnColor
                    End If
                End If
            Next
        End Sub

        Private Sub btnDocs_Click(sender As Object, e As EventArgs) Handles btnDocs.Click
            If DirectCast(sender, Button).BackColor.G = 70 Then Return ' if selected, do not reload
            _allowNavigation = True
            SwitchTab("docs")
            wb.Navigate("https://github.com/sxyu/Cantus-2/blob/master/README.md")
            wb.Show()
            wb.Focus()
        End Sub

        Private Sub btnLicense_Click(sender As Object, e As EventArgs) Handles btnLicense.Click
            If DirectCast(sender, Button).BackColor.G = 70 Then Return ' if selected, do not reload
            _allowNavigation = True
            SwitchTab("license")
            wb.Hide()
            tb.Text = My.Resources.LICENSE.Replace(vbLf, vbCrLf)
        End Sub

        Private Sub btnLog_Click(sender As Object, e As EventArgs) Handles btnLog.Click
            If DirectCast(sender, Button).BackColor.G = 70 Then Return ' if selected, do not reload
            SwitchTab("log")
            wb.Hide()
            tb.Text = My.Resources.UpdateMsg.Replace("{ver}", Application.ProductVersion).Replace(vbLf, vbCrLf)
        End Sub

        Private _allowNavigation As Boolean = False
        Private Sub wb_Navigating(sender As Object, e As WebBrowserNavigatingEventArgs) Handles wb.Navigating
            If Not _allowNavigation Then e.Cancel = True Else _allowNavigation = False
        End Sub

        Private Sub TmrAnim_Tick(sender As Object, e As EventArgs) Handles TmrAnim.Tick
            If Me.Opacity >= 1 Then TmrAnim.Stop()
            Me.Opacity += 0.05
        End Sub
    End Class
End Namespace