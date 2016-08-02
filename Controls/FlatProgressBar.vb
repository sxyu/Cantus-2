Namespace UI.UserControls
    ''' <summary>
    ''' A flat progress bar
    ''' </summary>
    Public Class FlatProgressBar : Inherits ProgressBar
        ''' <summary>
        ''' The color of the progress bar
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ProgressColor As Color
        ''' <summary>
        ''' Text to display on progressbar
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ProgressText As String
        ''' <summary>
        ''' Specifies if a border should be drawn
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property DrawBorder As Boolean
        ''' <summary>
        ''' specifies if the control should be drawn vertically
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Vertical As Boolean
        Public Sub New()
            Me.SetStyle(ControlStyles.UserPaint, True)
            Me.SetStyle(ControlStyles.AllPaintingInWmPaint, True)
            Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
            Me.ProgressColor = Color.FromArgb(80, 80, 80)
            Me.BackColor = Color.WhiteSmoke
            Me.Vertical = False
            Me.ProgressText = ""
        End Sub
        Protected Overrides Sub onPaint(e As PaintEventArgs)
            'set to high speed, since we are only painting rectangles anyways
            e.Graphics.CompositingQuality = Drawing2D.CompositingQuality.HighSpeed
            e.Graphics.SmoothingMode = Drawing2D.SmoothingMode.HighSpeed
            'well, and text
            e.Graphics.TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAlias
            Dim rec As Rectangle = e.ClipRectangle
            If Vertical Then
                rec.Height = CInt(rec.Height * CDbl(Value / Maximum))
            Else
                rec.Width = CInt(rec.Width * CDbl(Value / Maximum))
            End If
            If (ProgressBarRenderer.IsSupported) Then
                ProgressBarRenderer.DrawHorizontalBar(e.Graphics, e.ClipRectangle)
                If Vertical Then
                    rec.Width = rec.Width - 4
                    If DrawBorder Then
                        rec.Height = CInt(rec.Height * (e.ClipRectangle.Height - 4 / e.ClipRectangle.Height))
                    End If
                Else
                    rec.Height = rec.Height - 4
                    If DrawBorder Then
                        rec.Width = CInt(rec.Width * (e.ClipRectangle.Width - 4 / e.ClipRectangle.Width))
                    End If
                End If
                Using progressBrush As New SolidBrush(ProgressColor)
                    Using forecolorBrush As New SolidBrush(Me.ForeColor)
                        If Vertical Then
                            If DrawBorder Then
                                e.Graphics.FillRectangle(progressBrush, 2, 2, rec.Width, rec.Height)
                                e.Graphics.DrawString(ProgressText, New Font("Segoe UI SemiLight", CSng(Me.Width / 5 * 2)), forecolorBrush, 4, 4)
                            Else
                                Using backBrush As New SolidBrush(Me.BackColor)
                                    e.Graphics.FillRectangle(backBrush, 0, 0, e.ClipRectangle.Width, e.ClipRectangle.Height)
                                End Using
                                e.Graphics.FillRectangle(progressBrush, 0, e.ClipRectangle.Height - rec.Height, e.ClipRectangle.Width, rec.Height)
                                Dim drawFormat As New System.Drawing.StringFormat
                                drawFormat.FormatFlags = StringFormatFlags.DirectionVertical
                                e.Graphics.DrawString(ProgressText, New Font("Segoe UI SemiLight", CSng(Me.Width / 5 * 2)), forecolorBrush, 4, 4, drawFormat)
                            End If
                        Else
                            If DrawBorder Then
                                e.Graphics.FillRectangle(progressBrush, 2, 2, rec.Width, rec.Height)
                                e.Graphics.DrawString(ProgressText, New Font("Segoe UI SemiLight", CSng(Me.Height / 5 * 2)), forecolorBrush, 4, 4)
                            Else
                                Using backBrush As New SolidBrush(Me.BackColor)
                                    e.Graphics.FillRectangle(backBrush, 0, 0, e.ClipRectangle.Width, e.ClipRectangle.Height)
                                End Using
                                e.Graphics.FillRectangle(progressBrush, 0, 0, rec.Width, e.ClipRectangle.Height)
                                e.Graphics.DrawString(ProgressText, New Font("Segoe UI SemiLight", CSng(Me.Height / 5 * 2)), forecolorBrush, 4, 4)
                            End If
                        End If
                    End Using
                End Using
            End If
        End Sub
    End Class
End Namespace
