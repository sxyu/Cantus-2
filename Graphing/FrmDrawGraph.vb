Public Class FrmDrawGraph

    Dim enable(36) As Boolean
    Dim graph(36) As List(Of Integer)
    Private Sub FrmDrawGraph_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Icon = My.Resources.Calculator
        btnVis.PerformClick()
    End Sub
    Function BuildGraph() As Boolean
        Try
            For i As Integer = 1 To 35
                enable(i) = False
                If graph(i) IsNot Nothing Then graph(i).Clear()
                graph(i) = New List(Of Integer)
            Next
            Dim spl() As String = tbEdges.Text.Trim.Replace(vbCrLf, " ").Replace(vbCr, " ").Replace(vbLf, " ").Replace("  ", "").Split(" "c)
            If spl.Length Mod 2 = 1 Then Return False
            For i As Integer = 0 To spl.Length - 2 Step 2
                Dim a As Integer = Integer.Parse(spl(i))
                Dim b As Integer = Integer.Parse(spl(i + 1))
                enable(a) = True
                enable(b) = True
                graph(a).Add(b)
            Next
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function
    Private Sub btnVis_Click(sender As Object, e As EventArgs) Handles btnVis.Click
        If (BuildGraph()) Then
            canvas.Invalidate()
        End If
    End Sub
    Private Sub canvas_Paint(sender As Object, e As PaintEventArgs) Handles canvas.Paint
        Dim x As Integer = 100
        Dim y As Integer = 100
        Dim ct As Integer = 0
        Dim nrow As Integer = 0
        Dim _buf As New Bitmap(canvas.Width, canvas.Height)
        Dim g As Graphics = Graphics.FromImage(_buf)
        g.SmoothingMode = Drawing2D.SmoothingMode.HighQuality
        For i As Integer = 1 To 35
            If enable(i) Then nrow += 1
        Next
        nrow = CInt(Math.Sqrt(nrow))
        Dim pt(36) As Point
        For i As Integer = 1 To 35
            If enable(i) Then
                pt(i) = New Point(x + CInt(Rnd() * 10 - 5), y + CInt(Rnd() * 20 - 10) + If(ct Mod 2 = 1, 30, -30))
                x += canvas.Width \ (nrow + 1)
                If x > canvas.Width OrElse ct >= nrow - 1 Then
                    x = 100 + CInt(Rnd() * 30 - 15)
                    y += canvas.Height \ (nrow + 2)
                    ct = 0
                    Continue For
                End If
                ct += 1
                'GraphVisualizer()
            End If
        Next
        For i As Integer = 1 To 35
            If enable(i) Then
                For j As Integer = 0 To graph(i).Count - 1
                    Dim t As Integer = graph(i)(j)
                    If (t < i) Then
                        If (pt(i).Y > pt(t).Y) Then
                            pt(i).Y -= (pt(i).Y - pt(t).Y) \ 16
                            pt(t).Y += (pt(i).Y - pt(t).Y) \ 12
                        End If
                        If (Math.Abs(pt(i).X - pt(t).X) < 50) Then
                            pt(i).X -= (pt(i).X - pt(t).X) \ 10
                            pt(t).X -= (pt(i).X - pt(t).X) \ 12
                            For k As Integer = 1 To 35
                                If enable(k) And k <> i And k <> t Then
                                    If (Not ((pt(k).Y < pt(i).Y And pt(k).Y < pt(t).Y) Or (pt(k).Y > pt(i).Y And pt(k).Y > pt(t).Y))) And
                                        (Math.Abs(pt(k).X - pt(i).X) < 100 OrElse Math.Abs(pt(t).X - pt(k).X) < 100) Then
                                        pt(k).X += 40
                                    End If
                                End If
                            Next
                        Else
                            pt(i).X -= (pt(i).X - pt(t).X) \ 12
                            pt(t).X += (pt(i).X - pt(t).X) \ 12
                        End If
                    Else
                        If (pt(i).Y < pt(t).Y) Then
                            pt(i).Y += (pt(t).Y - pt(i).Y) \ 16
                            pt(t).Y -= (pt(t).Y - pt(i).Y) \ 12
                        End If
                        If (Math.Abs(pt(i).X - pt(t).X) < 50) Then
                            pt(i).X -= (pt(i).X - pt(t).X) \ 8
                            pt(t).X -= (pt(i).X - pt(t).X) \ 10
                            For k As Integer = 1 To 35
                                If enable(k) And k <> i And k <> t Then
                                    If (Not ((pt(k).Y < pt(i).Y And pt(k).Y < pt(t).Y) Or (pt(k).Y > pt(i).Y And pt(k).Y > pt(t).Y))) And
                                        (Math.Abs(pt(k).X - pt(i).X) < 100 OrElse Math.Abs(pt(t).X - pt(k).X) < 100) Then
                                        pt(k).X += 40
                                    End If
                                End If
                            Next
                        Else
                            pt(i).X -= (pt(i).X - pt(t).X) \ 12
                            pt(t).X += (pt(i).X - pt(t).X) \ 12
                        End If
                    End If
                        Next
                    End If
        Next
        For i As Integer = 1 To 35
            If enable(i) Then
                For j As Integer = 0 To graph(i).Count - 1
                    Dim t As Integer = graph(i)(j)
                    If pt(i).X < 26 Then pt(i).X = 26
                    If pt(i).Y < 26 Then pt(i).Y = 26
                    If pt(i).X > canvas.Width - 26 Then pt(i).X = canvas.Width - 26
                    If pt(i).Y > canvas.Height - 26 Then pt(i).Y = canvas.Height - 26
                    g.DrawLine(Pens.White, pt(i), pt(t))
                Next
            End If
        Next
        For i As Integer = 1 To 35
            If enable(i) Then
                g.FillEllipse(Brushes.SlateBlue, pt(i).X - 25, pt(i).Y - 25, 50, 50)
                g.DrawString(i.ToString, New Font("Segoe UI Semilight", 25), Brushes.White, pt(i).X - 15, pt(i).Y - 22)
            End If
        Next
        e.Graphics.DrawImageUnscaled(_buf, 0, 0)
    End Sub

    Private Sub FrmDrawGraph_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize
        canvas.Invalidate()
    End Sub

    Private Sub tbEdges_KeyUp(sender As Object, e As KeyEventArgs) Handles tbEdges.KeyUp
        If e.Control AndAlso e.KeyCode = Keys.A Then
            tbEdges.SelectAll()
        End If
    End Sub
End Class