Public Class DiagFeatureList
    Dim _featureList As String
    Public Sub New(featureList As String)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me._featureList = featureList
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

    Private Sub DiagFeatureList_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Icon = My.Resources.Calculator
        tb.Text = _featureList
        tb.SelectionStart = 0
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
End Class