Namespace UI.Dialogs
    Public Class DiagFeatureList
        Public Sub New()
            ' This call is required by the designer.
            InitializeComponent()
        End Sub

        Private Sub BtnClose_Click(sender As Object, e As EventArgs) Handles BtnClose.Click
            Me.Close()
        End Sub

        Private Sub DiagFeatureList_Load(sender As Object, e As EventArgs) Handles MyBase.Load
            Me.Opacity = 0
            Me.Icon = My.Resources.Cantus
            Tb.Text = My.Resources.UpdateMsg.Replace("{ver}", Version).Replace(vbLf, vbCrLf)
            Tb.SelectionStart = 0
            Dim cCode As String = Globalization.CultureInfo.CurrentCulture.Name

            If cCode.StartsWith("en") AndAlso cCode <> "en-US" Then BtnLicense.Text = "Licence" ' spelling for non-americannglish...
            TmrAnim.Start()
        End Sub

        Dim ctrl As Boolean = False

        Private Sub Tb_KeyPress(sender As Object, e As KeyPressEventArgs) Handles Tb.KeyPress
            If Not ctrl Then e.Handled = True
        End Sub

        Private Sub Tb_KeyDown(sender As Object, e As KeyEventArgs) Handles Tb.KeyDown
            If e.Control Then
                If e.KeyCode = Keys.A Then
                    Tb.SelectAll()
                ElseIf e.KeyCode = Keys.C
                    ctrl = True
                End If
            End If
        End Sub

        Private Sub Tb_KeyUp(sender As Object, e As KeyEventArgs) Handles Tb.KeyUp
            ctrl = False
        End Sub

        Private Sub SwitchTab(BtnName As String)
            Dim curBtnColor As Color = Color.FromArgb(70, 70, 70)
            Dim backBtnColor As Color = Color.FromArgb(55, 55, 55)
            For Each c As Control In PnlNote.Controls
                If TypeOf c Is Button AndAlso Not c.Tag Is Nothing Then
                    Dim Btn As Button = DirectCast(c, Button)
                    If Btn.Tag.ToString = BtnName Then
                        Btn.BackColor = curBtnColor
                        Btn.FlatAppearance.MouseOverBackColor = curBtnColor
                        Btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(85, 85, 85)
                    Else
                        Btn.BackColor = backBtnColor
                        Btn.FlatAppearance.MouseOverBackColor = backBtnColor
                        Btn.FlatAppearance.MouseDownBackColor = curBtnColor
                    End If
                End If
            Next
        End Sub

        Private Sub BtnDocs_Click(sender As Object, e As EventArgs) Handles BtnDocs.Click
            If DirectCast(sender, Button).BackColor.G = 70 Then Return ' if selected, do not reload
            _allowNavigation = True
            SwitchTab("docs")
            Wb.Navigate("https://github.com/sxyu/Cantus-GUI/blob/master/README.md")
            Wb.Show()
            Wb.Focus()
        End Sub

        Private Sub BtnLicense_Click(sender As Object, e As EventArgs) Handles BtnLicense.Click
            If DirectCast(sender, Button).BackColor.G = 70 Then Return ' if selected, do not reload
            _allowNavigation = True
            SwitchTab("license")
            Wb.Hide()
            Tb.Text = My.Resources.LICENSE.Replace(vbLf, vbCrLf)
        End Sub

        Private Sub BtnLog_Click(sender As Object, e As EventArgs) Handles BtnLog.Click
            If DirectCast(sender, Button).BackColor.G = 70 Then Return ' if selected, do not reload
            SwitchTab("log")
            Wb.Hide()
            Tb.Text = My.Resources.UpdateMsg.Replace("{ver}", Version).Replace(vbLf, vbCrLf)
        End Sub

        Private _allowNavigation As Boolean = False
        Private Sub wb_Navigating(sender As Object, e As WebBrowserNavigatingEventArgs) Handles Wb.Navigating
            If Not _allowNavigation Then e.Cancel = True Else _allowNavigation = False
        End Sub

        Private Sub TmrAnim_Tick(sender As Object, e As EventArgs) Handles TmrAnim.Tick
            If Me.Opacity >= 1 Then TmrAnim.Stop()
            Me.Opacity += 0.05
        End Sub
    End Class
End Namespace