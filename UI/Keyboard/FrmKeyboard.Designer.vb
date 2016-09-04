Namespace UI.Keyboards
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class FrmKeyboard
        Inherits System.Windows.Forms.Form

        'Form overrides dispose to clean up the component list.
        <System.Diagnostics.DebuggerNonUserCode()>
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            Try
                If disposing AndAlso components IsNot Nothing Then
                    components.Dispose()
                End If
            Finally
                MyBase.Dispose(disposing)
            End Try
        End Sub

        'Required by the Windows Form Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.  
        'Do not modify it using the code editor.
        <System.Diagnostics.DebuggerStepThrough()>
        Private Sub InitializeComponent()
            Me.MainKeyboard = New Cantus.UI.Keyboards.Keyboard()
            Me.SuspendLayout()
            '
            'MainKeyboard
            '
            Me.MainKeyboard.AutoSize = True
            Me.MainKeyboard.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
            Me.MainKeyboard.BackColor = System.Drawing.Color.FromArgb(CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer))
            Me.MainKeyboard.Cursor = System.Windows.Forms.Cursors.Hand
            Me.MainKeyboard.Font = New System.Drawing.Font(OpenSansLight, 12.25!)
            Me.MainKeyboard.Location = New System.Drawing.Point(0, 0)
            Me.MainKeyboard.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
            Me.MainKeyboard.MaximumSize = New System.Drawing.Size(728, 300)
            Me.MainKeyboard.Name = "MainKeyboard"
            Me.MainKeyboard.Size = New System.Drawing.Size(728, 300)
            Me.MainKeyboard.TabIndex = 0
            '
            'FrmKeyboard
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
            Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer))
            Me.ClientSize = New System.Drawing.Size(728, 300)
            Me.Controls.Add(Me.MainKeyboard)
            Me.Font = New System.Drawing.Font(OpenSansLight, 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.ForeColor = System.Drawing.Color.White
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
            Me.MaximizeBox = False
            Me.MaximumSize = New System.Drawing.Size(728, 300)
            Me.MinimizeBox = False
            Me.MinimumSize = New System.Drawing.Size(728, 300)
            Me.Name = "FrmKeyboard"
            Me.ShowInTaskbar = False
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
            Me.Text = "Cantus Keyboard"
            Me.TopMost = True
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub

        Friend WithEvents MainKeyboard As UI.Keyboards.Keyboard
    End Class
End Namespace
