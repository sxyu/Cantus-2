Namespace UI.Keyboards
    Public Class FrmKeyboard
        Dim _ppt As Point
        Dim _dragging As Boolean
        Private Sub MainKeyboard_MouseDown(sender As Object, e As MouseEventArgs) Handles MainKeyboard.MouseDown, MyBase.MouseDown
            _ppt = e.Location
            _dragging = True
        End Sub

        Private Sub MainKeyboard_MouseMove(sender As Object, e As MouseEventArgs) Handles MainKeyboard.MouseMove, MyBase.MouseMove
            If _dragging Then
                Me.Left += e.X - _ppt.X
                Me.Top += e.Y - _ppt.Y
            End If
        End Sub

        Private Sub MainKeyboard_MouseUp(sender As Object, e As MouseEventArgs) Handles MainKeyboard.MouseUp, MyBase.MouseUp
            _dragging = False
        End Sub

        Private Sub MainKeyboard_CloseKeyboard(sender As Object, e As EventArgs) Handles MainKeyboard.CloseKeyboard
            Me.Close()
        End Sub

        Private Sub FrmKeyboard_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
            My.Settings.ShowKeyboard = False
            My.Settings.Save()
            If Not _animateReverse Then
                e.Cancel = True
                _animateReverse = True
                TmrAnim.Start()
            End If
        End Sub

        Private Sub FrmKeyboard_Load(sender As Object, e As EventArgs) Handles MyBase.Load
            Me.Top = Screen.PrimaryScreen.Bounds.Height
            Me.Left = FrmEditor.Right - Me.Width - 53
            TmrAnim.Start()
        End Sub

        Private _animateReverse As Boolean = False

        Private Sub TmrAnim_Tick(sender As Object, e As EventArgs) Handles TmrAnim.Tick
            If _animateReverse Then
                If Me.Top < Screen.PrimaryScreen.Bounds.Height Then
                    Me.Top += Me.Top \ 4 + 8
                Else
                    Me.Top = Screen.PrimaryScreen.Bounds.Height
                    TmrAnim.Stop()
                    Me.Close()
                End If
            Else
                If Me.Top > FrmEditor.Bottom - Me.Height Then
                    Me.Top -= (Me.Top - FrmEditor.Bottom + Me.Height) \ 3 + 2
                Else
                    Me.Top = FrmEditor.Bottom - Me.Height
                    TmrAnim.Stop()
                End If
            End If
        End Sub
    End Class
End Namespace