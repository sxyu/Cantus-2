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
        'Do not modify it using the codeditor.
        <System.Diagnostics.DebuggerStepThrough()>
        Private Sub InitializeComponent()
            Me.components = New System.ComponentModel.Container()
            Me.Tb = New System.Windows.Forms.TextBox()
            Me.PnlBottom = New System.Windows.Forms.Panel()
            Me.PnlNote = New System.Windows.Forms.Panel()
            Me.BtnLog = New System.Windows.Forms.Button()
            Me.BtnLicense = New System.Windows.Forms.Button()
            Me.BtnDocs = New System.Windows.Forms.Button()
            Me.LbNote = New System.Windows.Forms.Label()
            Me.BtnClose = New System.Windows.Forms.Button()
            Me.Wb = New System.Windows.Forms.WebBrowser()
            Me.TmrAnim = New System.Windows.Forms.Timer(Me.components)
            Me.PnlBottom.SuspendLayout()
            Me.PnlNote.SuspendLayout()
            Me.SuspendLayout()
            '
            'Tb
            '
            Me.Tb.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.Tb.BackColor = System.Drawing.Color.FromArgb(CType(CType(35, Byte), Integer), CType(CType(35, Byte), Integer), CType(CType(35, Byte), Integer))
            Me.Tb.BorderStyle = System.Windows.Forms.BorderStyle.None
            Me.Tb.Font = New System.Drawing.Font("Consolas", 11.0!)
            Me.Tb.ForeColor = System.Drawing.Color.White
            Me.Tb.Location = New System.Drawing.Point(13, 14)
            Me.Tb.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
            Me.Tb.Multiline = True
            Me.Tb.Name = "Tb"
            Me.Tb.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
            Me.Tb.Size = New System.Drawing.Size(1186, 624)
            Me.Tb.TabIndex = 0
            '
            'PnlBottom
            '
            Me.PnlBottom.BackColor = System.Drawing.Color.FromArgb(CType(CType(45, Byte), Integer), CType(CType(45, Byte), Integer), CType(CType(45, Byte), Integer))
            Me.PnlBottom.Controls.Add(Me.PnlNote)
            Me.PnlBottom.Controls.Add(Me.BtnClose)
            Me.PnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom
            Me.PnlBottom.Location = New System.Drawing.Point(0, 648)
            Me.PnlBottom.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
            Me.PnlBottom.Name = "PnlBottom"
            Me.PnlBottom.Size = New System.Drawing.Size(1199, 41)
            Me.PnlBottom.TabIndex = 1
            '
            'PnlNote
            '
            Me.PnlNote.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.PnlNote.Controls.Add(Me.BtnLog)
            Me.PnlNote.Controls.Add(Me.BtnLicense)
            Me.PnlNote.Controls.Add(Me.BtnDocs)
            Me.PnlNote.Controls.Add(Me.LbNote)
            Me.PnlNote.Location = New System.Drawing.Point(0, -4)
            Me.PnlNote.Name = "PnlNote"
            Me.PnlNote.Size = New System.Drawing.Size(1072, 100)
            Me.PnlNote.TabIndex = 22
            '
            'BtnLog
            '
            Me.BtnLog.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.BtnLog.BackColor = System.Drawing.Color.FromArgb(CType(CType(70, Byte), Integer), CType(CType(70, Byte), Integer), CType(CType(70, Byte), Integer))
            Me.BtnLog.Cursor = System.Windows.Forms.Cursors.Hand
            Me.BtnLog.FlatAppearance.BorderSize = 0
            Me.BtnLog.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(85, Byte), Integer), CType(CType(85, Byte), Integer), CType(CType(85, Byte), Integer))
            Me.BtnLog.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(70, Byte), Integer), CType(CType(70, Byte), Integer), CType(CType(70, Byte), Integer))
            Me.BtnLog.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.BtnLog.ForeColor = System.Drawing.Color.White
            Me.BtnLog.Location = New System.Drawing.Point(688, -1)
            Me.BtnLog.Margin = New System.Windows.Forms.Padding(32, 14, 32, 14)
            Me.BtnLog.Name = "BtnLog"
            Me.BtnLog.Size = New System.Drawing.Size(128, 50)
            Me.BtnLog.TabIndex = 0
            Me.BtnLog.Tag = "log"
            Me.BtnLog.Text = "What's New"
            Me.BtnLog.UseVisualStyleBackColor = False
            '
            'BtnLicense
            '
            Me.BtnLicense.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.BtnLicense.BackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.BtnLicense.Cursor = System.Windows.Forms.Cursors.Hand
            Me.BtnLicense.FlatAppearance.BorderSize = 0
            Me.BtnLicense.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(70, Byte), Integer), CType(CType(70, Byte), Integer), CType(CType(70, Byte), Integer))
            Me.BtnLicense.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.BtnLicense.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.BtnLicense.ForeColor = System.Drawing.Color.White
            Me.BtnLicense.Location = New System.Drawing.Point(816, -1)
            Me.BtnLicense.Margin = New System.Windows.Forms.Padding(32, 14, 32, 14)
            Me.BtnLicense.Name = "BtnLicense"
            Me.BtnLicense.Size = New System.Drawing.Size(128, 50)
            Me.BtnLicense.TabIndex = 1
            Me.BtnLicense.Tag = "license"
            Me.BtnLicense.Text = "License"
            Me.BtnLicense.UseVisualStyleBackColor = False
            '
            'BtnDocs
            '
            Me.BtnDocs.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.BtnDocs.BackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.BtnDocs.Cursor = System.Windows.Forms.Cursors.Hand
            Me.BtnDocs.FlatAppearance.BorderSize = 0
            Me.BtnDocs.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(70, Byte), Integer), CType(CType(70, Byte), Integer), CType(CType(70, Byte), Integer))
            Me.BtnDocs.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.BtnDocs.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.BtnDocs.ForeColor = System.Drawing.Color.White
            Me.BtnDocs.Location = New System.Drawing.Point(944, -1)
            Me.BtnDocs.Margin = New System.Windows.Forms.Padding(32, 14, 32, 14)
            Me.BtnDocs.Name = "BtnDocs"
            Me.BtnDocs.Size = New System.Drawing.Size(128, 50)
            Me.BtnDocs.TabIndex = 2
            Me.BtnDocs.Tag = "docs"
            Me.BtnDocs.Text = "Docs"
            Me.BtnDocs.UseVisualStyleBackColor = False
            '
            'LbNote
            '
            Me.LbNote.Anchor = System.Windows.Forms.AnchorStyles.Left
            Me.LbNote.AutoSize = True
            Me.LbNote.ForeColor = System.Drawing.Color.Gainsboro
            Me.LbNote.Location = New System.Drawing.Point(12, 14)
            Me.LbNote.Name = "LbNote"
            Me.LbNote.Size = New System.Drawing.Size(559, 17)
            Me.LbNote.TabIndex = 0
            Me.LbNote.Text = "Tip: Click the ""What's New | Docs"" link  in the settings pane later on to view th" &
    "is dialog"
            '
            'BtnClose
            '
            Me.BtnClose.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.BtnClose.BackColor = System.Drawing.Color.Brown
            Me.BtnClose.Cursor = System.Windows.Forms.Cursors.Hand
            Me.BtnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.BtnClose.FlatAppearance.BorderSize = 0
            Me.BtnClose.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(170, Byte), Integer), CType(CType(60, Byte), Integer), CType(CType(60, Byte), Integer))
            Me.BtnClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Brown
            Me.BtnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.BtnClose.ForeColor = System.Drawing.Color.White
            Me.BtnClose.Location = New System.Drawing.Point(1070, -4)
            Me.BtnClose.Margin = New System.Windows.Forms.Padding(32, 14, 32, 14)
            Me.BtnClose.Name = "BtnClose"
            Me.BtnClose.Size = New System.Drawing.Size(130, 50)
            Me.BtnClose.TabIndex = 0
            Me.BtnClose.Text = "Close"
            Me.BtnClose.UseVisualStyleBackColor = False
            '
            'Wb
            '
            Me.Wb.AllowWebBrowserDrop = False
            Me.Wb.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.Wb.IsWebBrowserContextMenuEnabled = False
            Me.Wb.Location = New System.Drawing.Point(0, 0)
            Me.Wb.MinimumSize = New System.Drawing.Size(20, 20)
            Me.Wb.Name = "Wb"
            Me.Wb.ScriptErrorsSuppressed = True
            Me.Wb.Size = New System.Drawing.Size(1199, 646)
            Me.Wb.TabIndex = 0
            Me.Wb.Url = New System.Uri("", System.UriKind.Relative)
            Me.Wb.Visible = False
            Me.Wb.WebBrowserShortcutsEnabled = False
            '
            'TmrAnim
            '
            Me.TmrAnim.Interval = 50
            '
            'DiagFeatureList
            '
            Me.AcceptButton = Me.BtnClose
            Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 17.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(35, Byte), Integer), CType(CType(35, Byte), Integer), CType(CType(35, Byte), Integer))
            Me.CancelButton = Me.BtnClose
            Me.ClientSize = New System.Drawing.Size(1199, 689)
            Me.Controls.Add(Me.Wb)
            Me.Controls.Add(Me.PnlBottom)
            Me.Controls.Add(Me.Tb)
            Me.Font = New System.Drawing.Font(OpenSans, 11.0!)
            Me.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "DiagFeatureList"
            Me.Opacity = 0R
            Me.ShowInTaskbar = False
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
            Me.Text = "Welcome to Cantus"
            Me.PnlBottom.ResumeLayout(False)
            Me.PnlNote.ResumeLayout(False)
            Me.PnlNote.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub

        Friend WithEvents Tb As TextBox
        Friend WithEvents PnlBottom As Panel
        Friend WithEvents BtnClose As Button
        Friend WithEvents PnlNote As Panel
        Friend WithEvents LbNote As Label
        Friend WithEvents BtnDocs As Button
        Friend WithEvents Wb As WebBrowser
        Friend WithEvents BtnLicense As Button
        Friend WithEvents BtnLog As Button
        Friend WithEvents TmrAnim As Timer
    End Class
End Namespace
