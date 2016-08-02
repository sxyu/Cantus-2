Namespace UI.Dialogs
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class DiagFeatureList
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
            Me.components = New System.ComponentModel.Container()
            Me.tb = New System.Windows.Forms.TextBox()
            Me.pnlBottom = New System.Windows.Forms.Panel()
            Me.pnlNote = New System.Windows.Forms.Panel()
            Me.btnLog = New System.Windows.Forms.Button()
            Me.btnLicense = New System.Windows.Forms.Button()
            Me.btnDocs = New System.Windows.Forms.Button()
            Me.lbNote = New System.Windows.Forms.Label()
            Me.btnClose = New System.Windows.Forms.Button()
            Me.wb = New System.Windows.Forms.WebBrowser()
            Me.TmrAnim = New System.Windows.Forms.Timer(Me.components)
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
            Me.tb.Size = New System.Drawing.Size(1186, 656)
            Me.tb.TabIndex = 0
            '
            'pnlBottom
            '
            Me.pnlBottom.BackColor = System.Drawing.Color.FromArgb(CType(CType(45, Byte), Integer), CType(CType(45, Byte), Integer), CType(CType(45, Byte), Integer))
            Me.pnlBottom.Controls.Add(Me.pnlNote)
            Me.pnlBottom.Controls.Add(Me.btnClose)
            Me.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom
            Me.pnlBottom.Location = New System.Drawing.Point(0, 680)
            Me.pnlBottom.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
            Me.pnlBottom.Name = "pnlBottom"
            Me.pnlBottom.Size = New System.Drawing.Size(1199, 41)
            Me.pnlBottom.TabIndex = 1
            '
            'pnlNote
            '
            Me.pnlNote.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.pnlNote.Controls.Add(Me.btnLog)
            Me.pnlNote.Controls.Add(Me.btnLicense)
            Me.pnlNote.Controls.Add(Me.btnDocs)
            Me.pnlNote.Controls.Add(Me.lbNote)
            Me.pnlNote.Location = New System.Drawing.Point(0, -4)
            Me.pnlNote.Name = "pnlNote"
            Me.pnlNote.Size = New System.Drawing.Size(1072, 100)
            Me.pnlNote.TabIndex = 22
            '
            'btnLog
            '
            Me.btnLog.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnLog.BackColor = System.Drawing.Color.FromArgb(CType(CType(70, Byte), Integer), CType(CType(70, Byte), Integer), CType(CType(70, Byte), Integer))
            Me.btnLog.Cursor = System.Windows.Forms.Cursors.Hand
            Me.btnLog.FlatAppearance.BorderSize = 0
            Me.btnLog.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(85, Byte), Integer), CType(CType(85, Byte), Integer), CType(CType(85, Byte), Integer))
            Me.btnLog.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(70, Byte), Integer), CType(CType(70, Byte), Integer), CType(CType(70, Byte), Integer))
            Me.btnLog.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnLog.ForeColor = System.Drawing.Color.White
            Me.btnLog.Location = New System.Drawing.Point(688, -1)
            Me.btnLog.Margin = New System.Windows.Forms.Padding(32, 14, 32, 14)
            Me.btnLog.Name = "btnLog"
            Me.btnLog.Size = New System.Drawing.Size(128, 50)
            Me.btnLog.TabIndex = 0
            Me.btnLog.Tag = "log"
            Me.btnLog.Text = "What's New"
            Me.btnLog.UseVisualStyleBackColor = False
            '
            'btnLicense
            '
            Me.btnLicense.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnLicense.BackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.btnLicense.Cursor = System.Windows.Forms.Cursors.Hand
            Me.btnLicense.FlatAppearance.BorderSize = 0
            Me.btnLicense.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(70, Byte), Integer), CType(CType(70, Byte), Integer), CType(CType(70, Byte), Integer))
            Me.btnLicense.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.btnLicense.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnLicense.ForeColor = System.Drawing.Color.White
            Me.btnLicense.Location = New System.Drawing.Point(816, -1)
            Me.btnLicense.Margin = New System.Windows.Forms.Padding(32, 14, 32, 14)
            Me.btnLicense.Name = "btnLicense"
            Me.btnLicense.Size = New System.Drawing.Size(128, 50)
            Me.btnLicense.TabIndex = 1
            Me.btnLicense.Tag = "license"
            Me.btnLicense.Text = "License"
            Me.btnLicense.UseVisualStyleBackColor = False
            '
            'btnDocs
            '
            Me.btnDocs.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnDocs.BackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.btnDocs.Cursor = System.Windows.Forms.Cursors.Hand
            Me.btnDocs.FlatAppearance.BorderSize = 0
            Me.btnDocs.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(70, Byte), Integer), CType(CType(70, Byte), Integer), CType(CType(70, Byte), Integer))
            Me.btnDocs.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.btnDocs.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnDocs.ForeColor = System.Drawing.Color.White
            Me.btnDocs.Location = New System.Drawing.Point(944, -1)
            Me.btnDocs.Margin = New System.Windows.Forms.Padding(32, 14, 32, 14)
            Me.btnDocs.Name = "btnDocs"
            Me.btnDocs.Size = New System.Drawing.Size(128, 50)
            Me.btnDocs.TabIndex = 2
            Me.btnDocs.Tag = "docs"
            Me.btnDocs.Text = "Docs"
            Me.btnDocs.UseVisualStyleBackColor = False
            '
            'lbNote
            '
            Me.lbNote.Anchor = System.Windows.Forms.AnchorStyles.Left
            Me.lbNote.AutoSize = True
            Me.lbNote.ForeColor = System.Drawing.Color.Gainsboro
            Me.lbNote.Location = New System.Drawing.Point(12, 14)
            Me.lbNote.Name = "lbNote"
            Me.lbNote.Size = New System.Drawing.Size(577, 20)
            Me.lbNote.TabIndex = 0
            Me.lbNote.Text = "Tip: Click the ""What's New | Docs"" link  in the settings pane later on to view th" &
    "is dialog"
            '
            'btnClose
            '
            Me.btnClose.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnClose.BackColor = System.Drawing.Color.Brown
            Me.btnClose.Cursor = System.Windows.Forms.Cursors.Hand
            Me.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.btnClose.FlatAppearance.BorderSize = 0
            Me.btnClose.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(170, Byte), Integer), CType(CType(60, Byte), Integer), CType(CType(60, Byte), Integer))
            Me.btnClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Brown
            Me.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnClose.ForeColor = System.Drawing.Color.White
            Me.btnClose.Location = New System.Drawing.Point(1070, -4)
            Me.btnClose.Margin = New System.Windows.Forms.Padding(32, 14, 32, 14)
            Me.btnClose.Name = "btnClose"
            Me.btnClose.Size = New System.Drawing.Size(130, 50)
            Me.btnClose.TabIndex = 0
            Me.btnClose.Text = "Close"
            Me.btnClose.UseVisualStyleBackColor = False
            '
            'wb
            '
            Me.wb.AllowWebBrowserDrop = False
            Me.wb.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.wb.IsWebBrowserContextMenuEnabled = False
            Me.wb.Location = New System.Drawing.Point(0, 0)
            Me.wb.MinimumSize = New System.Drawing.Size(20, 20)
            Me.wb.Name = "wb"
            Me.wb.ScriptErrorsSuppressed = True
            Me.wb.Size = New System.Drawing.Size(1199, 678)
            Me.wb.TabIndex = 0
            Me.wb.Url = New System.Uri("", System.UriKind.Relative)
            Me.wb.Visible = False
            Me.wb.WebBrowserShortcutsEnabled = False
            '
            'TmrAnim
            '
            Me.TmrAnim.Interval = 50
            '
            'DiagFeatureList
            '
            Me.AcceptButton = Me.btnClose
            Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 20.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(35, Byte), Integer), CType(CType(35, Byte), Integer), CType(CType(35, Byte), Integer))
            Me.CancelButton = Me.btnClose
            Me.ClientSize = New System.Drawing.Size(1199, 721)
            Me.Controls.Add(Me.wb)
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
        Friend WithEvents btnDocs As Button
        Friend WithEvents wb As WebBrowser
        Friend WithEvents btnLicense As Button
        Friend WithEvents btnLog As Button
        Friend WithEvents TmrAnim As Timer
    End Class
End Namespace
