Namespace UI
    Public Class SplashScreen
        Private Sub SplashScreen_Load(sender As Object, e As EventArgs) Handles MyBase.Load
            Keyboards.KeyboardLeft.Location = New Point(-Keyboards.KeyboardLeft.Width - 1, 10000)
            Keyboards.KeyboardRight.Location = New Point(-Keyboards.KeyboardRight.Width - 1, 10000)
            Me.SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.OptimizedDoubleBuffer, True)
            lbVer.Text = Application.ProductVersion
            Me.BringToFront()
            FrmEditor.Location = New Point(-FrmEditor.Width - 1, 0)
            FrmEditor.Show()
            FrmViewer.Location = New Point(-FrmViewer.Width - 1, -FrmViewer.Height - 1)
            FrmViewer.Show()
        End Sub
    End Class
End Namespace