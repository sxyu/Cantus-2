Public Class DiagFeatureList
    Dim _featureList As String
    Public Sub New(featureList As String)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me._featureList = featureList.Replace(vbCrLf, vbLf).Replace(vbCr, vbLf).Replace(vbLf, vbNewLine)
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

    Private Sub DiagFeatureList_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Icon = My.Resources.Calculator
        tb.Text = _featureList
        tb.SelectionStart = 0
        Dim cCode As String = Globalization.CultureInfo.CurrentCulture.Name

        If cCode.StartsWith("en") AndAlso cCode <> "en-US" Then btnLicense.Text = "Licence" ' spelling for non-american english...
    End Sub

    Dim ctrl As Boolean = False

    Private Sub tb_KeyPress(sender As Object, e As KeyPressEventArgs) Handles tb.KeyPress
        If Not ctrl Then e.Handled = True
    End Sub

    Private Sub tb_KeyDown(sender As Object, e As KeyEventArgs) Handles tb.KeyDown
        If e.Control Then
            If e.KeyCode = Keys.A Then
                tb.SelectAll()
            Else
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
        _allowNavigation = True
        SwitchTab("docs")
        wb.Navigate("https://github.com/sxyu/Cantus-2/blob/master/README.md")
        wb.Show()
        wb.Focus()
    End Sub

    Private Sub btnLicense_Click(sender As Object, e As EventArgs) Handles btnLicense.Click
        _allowNavigation = True
        SwitchTab("license")
        wb.Navigate("https://github.com/sxyu/Cantus-2/blob/master/LICENSE")
        wb.Show()
        wb.Focus()
    End Sub

    Private Sub btnLog_Click(sender As Object, e As EventArgs) Handles btnLog.Click
        SwitchTab("log")
        wb.Hide()
    End Sub

    Private _allowNavigation As Boolean = False
    Private Sub wb_Navigating(sender As Object, e As WebBrowserNavigatingEventArgs) Handles wb.Navigating
        'If Not _allowNavigation Then e.Cancel = True Else _allowNavigation = False
    End Sub
End Class