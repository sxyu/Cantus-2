Namespace UI.Updater
    Public Class DiagUpdateAvailable
        '' <summary>
        '' Create a new Update Available dialog
        '' </summary>
        '' <param name="newVersion">The new version of the application we're updating to, to be displayed on the dialog</param>
        Public Sub New(newVersion As String)

            ' This call is required by the designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            Me.LbNewVer.Text = newVersion
            Me.LbOldVer.Text = Version.ToString()
        End Sub

        Private Sub BtnOK_Click(sender As Object, e As EventArgs) Handles BtnOK.Click
            Me.DialogResult = DialogResult.OK
            Me.Close()
        End Sub

        Private Sub BtnCancel_Click(sender As Object, e As EventArgs) Handles BtnCancel.Click
            Me.DialogResult = DialogResult.Cancel
            Me.Close()
        End Sub

        Private Sub Tmr_Tick(sender As Object, e As EventArgs) Handles Tmr.Tick
            If Me.Opacity + 0.05 < 1 Then
                Me.Opacity += 0.05
            Else
                Tmr.Stop()
                Me.Opacity = 1
            End If
        End Sub

        Private Sub DiagUpdateAvailable_KeyUp(sender As Object, e As KeyEventArgs) Handles MyBase.KeyUp, BtnOK.KeyUp, BtnCancel.KeyUp
            If e.KeyCode = Keys.Enter Then
                BtnOK.PerformClick()
            End If
        End Sub

        Private _isMoving As Boolean
        Private _movingPrevPt As Point

        Private Sub Label1_MouseDown(sender As Object, e As MouseEventArgs) Handles PbLogo.MouseDown,
            LbOldVer.MouseDown, LbNewVer.MouseDown, Label5.MouseDown, Label3.MouseDown, Label2.MouseDown, Label1.MouseDown,
            MyBase.MouseDown
            _movingPrevPt = e.Location
            _isMoving = True
        End Sub

        Private Sub PbLogo_MouseMove(sender As Object, e As MouseEventArgs) Handles PbLogo.MouseMove,
            LbOldVer.MouseMove, LbNewVer.MouseMove, Label5.MouseMove, Label3.MouseMove, Label2.MouseMove, Label1.MouseMove,
            MyBase.MouseMove

            If _isMoving Then
                Me.Left = Me.Left + e.X - _movingPrevPt.X
                Me.Top = Me.Top + e.Y - _movingPrevPt.Y
            End If
        End Sub

        Private Sub PbLogo_MouseUp(sender As Object, e As MouseEventArgs) Handles PbLogo.MouseUp,
            LbOldVer.MouseUp, LbNewVer.MouseUp, Label5.MouseUp, Label3.MouseUp, Label2.MouseUp, Label1.MouseUp,
            MyBase.MouseUp

            _isMoving = False
        End Sub

        Private Sub DiagUpdateAvailable_Leave(sender As Object, e As EventArgs) Handles MyBase.Leave
            Me.DialogResult = DialogResult.Cancel
            Me.Close()
        End Sub
    End Class
End Namespace