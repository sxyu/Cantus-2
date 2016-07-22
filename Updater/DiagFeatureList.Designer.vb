<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class DiagFeatureList
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.tb = New System.Windows.Forms.TextBox()
        Me.pnlBottom = New System.Windows.Forms.Panel()
        Me.pnlNote = New System.Windows.Forms.Panel()
        Me.lbNote = New System.Windows.Forms.Label()
        Me.btnClose = New System.Windows.Forms.Button()
        Me.pnlBottom.SuspendLayout()
        Me.pnlNote.SuspendLayout()
        Me.SuspendLayout()
        '
        'tb
        '
        Me.tb.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.tb.BackColor = System.Drawing.Color.FromArgb(CType(CType(35, Byte), Integer), CType(CType(35, Byte), Integer), CType(CType(35, Byte), Integer))
        Me.tb.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.tb.ForeColor = System.Drawing.Color.White
        Me.tb.Location = New System.Drawing.Point(13, 14)
        Me.tb.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.tb.Multiline = True
        Me.tb.Name = "tb"
        Me.tb.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.tb.Size = New System.Drawing.Size(748, 481)
        Me.tb.TabIndex = 0
        '
        'pnlBottom
        '
        Me.pnlBottom.BackColor = System.Drawing.Color.FromArgb(CType(CType(45, Byte), Integer), CType(CType(45, Byte), Integer), CType(CType(45, Byte), Integer))
        Me.pnlBottom.Controls.Add(Me.pnlNote)
        Me.pnlBottom.Controls.Add(Me.btnClose)
        Me.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.pnlBottom.Location = New System.Drawing.Point(0, 505)
        Me.pnlBottom.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.pnlBottom.Name = "pnlBottom"
        Me.pnlBottom.Size = New System.Drawing.Size(761, 41)
        Me.pnlBottom.TabIndex = 1
        '
        'pnlNote
        '
        Me.pnlNote.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.pnlNote.Controls.Add(Me.lbNote)
        Me.pnlNote.Location = New System.Drawing.Point(0, -4)
        Me.pnlNote.Name = "pnlNote"
        Me.pnlNote.Size = New System.Drawing.Size(634, 100)
        Me.pnlNote.TabIndex = 22
        '
        'lbNote
        '
        Me.lbNote.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.lbNote.AutoSize = True
        Me.lbNote.ForeColor = System.Drawing.Color.Gainsboro
        Me.lbNote.Location = New System.Drawing.Point(12, 15)
        Me.lbNote.Name = "lbNote"
        Me.lbNote.Size = New System.Drawing.Size(531, 20)
        Me.lbNote.TabIndex = 0
        Me.lbNote.Text = "Tip: Click the version number in the setings view to reopen this window later on"
        '
        'btnClose
        '
        Me.btnClose.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnClose.BackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
        Me.btnClose.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnClose.FlatAppearance.BorderSize = 0
        Me.btnClose.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(70, Byte), Integer), CType(CType(70, Byte), Integer), CType(CType(70, Byte), Integer))
        Me.btnClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
        Me.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnClose.ForeColor = System.Drawing.Color.White
        Me.btnClose.Location = New System.Drawing.Point(632, -4)
        Me.btnClose.Margin = New System.Windows.Forms.Padding(32, 14, 32, 14)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(130, 50)
        Me.btnClose.TabIndex = 21
        Me.btnClose.Text = "Close"
        Me.btnClose.UseVisualStyleBackColor = False
        '
        'DiagFeatureList
        '
        Me.AcceptButton = Me.btnClose
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(35, Byte), Integer), CType(CType(35, Byte), Integer), CType(CType(35, Byte), Integer))
        Me.CancelButton = Me.btnClose
        Me.ClientSize = New System.Drawing.Size(761, 546)
        Me.Controls.Add(Me.pnlBottom)
        Me.Controls.Add(Me.tb)
        Me.Font = New System.Drawing.Font("Segoe UI", 11.0!)
        Me.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "DiagFeatureList"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Welcome to Cantus"
        Me.TopMost = True
        Me.pnlBottom.ResumeLayout(False)
        Me.pnlNote.ResumeLayout(False)
        Me.pnlNote.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents tb As TextBox
    Friend WithEvents pnlBottom As Panel
    Friend WithEvents btnClose As Button
    Friend WithEvents pnlNote As Panel
    Friend WithEvents lbNote As Label
End Class
