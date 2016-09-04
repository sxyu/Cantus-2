Namespace UI.KeyBoards
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
        End Sub

        Private Sub FrmKeyboard_Load(sender As Object, e As EventArgs) Handles MyBase.Load
            Me.Top = FrmEditor.Bottom - Me.Height
            Me.Left = FrmEditor.Right - Me.Width - 53
        End Sub
    End Class
End Namespace