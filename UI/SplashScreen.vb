Namespace UI
    Public Class SplashScreen
        Private Sub SplashScreen_Load(sender As Object, e As EventArgs) Handles MyBase.Load
            Me.SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.OptimizedDoubleBuffer, True)
            TmrAnim.Start()
            LbVer.Text = Version
        End Sub

        Private Sub TmrAnim_Tick(sender As Object, e As EventArgs) Handles TmrAnim.Tick
            If Me.Opacity = 0 Then
                Me.BringToFront()
                FrmEditor.Location = New Point(1000, 8000)
                FrmEditor.Show()
            End If
            If Me.Opacity + 0.1 < 1 Then
                Me.Opacity += 0.06
            Else
                TmrAnim.Stop()
                Me.Opacity = 1
            End If
        End Sub
    End Class
End Namespace