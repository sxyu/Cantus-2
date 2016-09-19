Namespace UI.Updater
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class DiagUpdateAvailable
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
            Me.Label1 = New System.Windows.Forms.Label()
            Me.Label2 = New System.Windows.Forms.Label()
            Me.PbLogo = New System.Windows.Forms.PictureBox()
            Me.Label3 = New System.Windows.Forms.Label()
            Me.LbNewVer = New System.Windows.Forms.Label()
            Me.BtnCancel = New System.Windows.Forms.Button()
            Me.BtnOK = New System.Windows.Forms.Button()
            Me.LbOldVer = New System.Windows.Forms.Label()
            Me.Label5 = New System.Windows.Forms.Label()
            Me.Tmr = New System.Windows.Forms.Timer(Me.components)
            CType(Me.PbLogo, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'Label1
            '
            Me.Label1.AutoSize = True
            Me.Label1.Font = New System.Drawing.Font(OpenSans, 13.25!)
            Me.Label1.ForeColor = System.Drawing.Color.White
            Me.Label1.Location = New System.Drawing.Point(70, 23)
            Me.Label1.Name = "Label1"
            Me.Label1.Size = New System.Drawing.Size(152, 24)
            Me.Label1.TabIndex = 0
            Me.Label1.Text = "Update Available"
            '
            'Label2
            '
            Me.Label2.AutoSize = True
            Me.Label2.Font = New System.Drawing.Font(OpenSansLight, 11.25!)
            Me.Label2.ForeColor = System.Drawing.Color.FromArgb(CType(CType(230, Byte), Integer), CType(CType(230, Byte), Integer), CType(CType(230, Byte), Integer))
            Me.Label2.Location = New System.Drawing.Point(30, 68)
            Me.Label2.Name = "Label2"
            Me.Label2.Size = New System.Drawing.Size(331, 100)
            Me.Label2.TabIndex = 1
            Me.Label2.Text = "An update for Cantus is available for download." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Would you like to update now?" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Warning: Theditor will need to be closed." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Please save all your changes befor" &
    "e proceeding."
            '
            'PbLogo
            '
            Me.PbLogo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
            Me.PbLogo.Image = Global.Cantus.My.Resources.Resources.Cantus_Logo
            Me.PbLogo.Location = New System.Drawing.Point(30, 17)
            Me.PbLogo.Name = "PbLogo"
            Me.PbLogo.Size = New System.Drawing.Size(34, 35)
            Me.PbLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
            Me.PbLogo.TabIndex = 16
            Me.PbLogo.TabStop = False
            '
            'Label3
            '
            Me.Label3.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
            Me.Label3.AutoSize = True
            Me.Label3.Font = New System.Drawing.Font(OpenSans, 11.25!)
            Me.Label3.ForeColor = System.Drawing.Color.FromArgb(CType(CType(250, Byte), Integer), CType(CType(250, Byte), Integer), CType(CType(250, Byte), Integer))
            Me.Label3.Location = New System.Drawing.Point(30, 217)
            Me.Label3.Name = "Label3"
            Me.Label3.Size = New System.Drawing.Size(100, 20)
            Me.Label3.TabIndex = 17
            Me.Label3.Text = "New Version:"
            '
            'LbNewVer
            '
            Me.LbNewVer.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
            Me.LbNewVer.AutoSize = True
            Me.LbNewVer.Font = New System.Drawing.Font(OpenSans, 11.25!)
            Me.LbNewVer.ForeColor = System.Drawing.Color.FromArgb(CType(CType(250, Byte), Integer), CType(CType(250, Byte), Integer), CType(CType(250, Byte), Integer))
            Me.LbNewVer.Location = New System.Drawing.Point(128, 217)
            Me.LbNewVer.Name = "LbNewVer"
            Me.LbNewVer.Size = New System.Drawing.Size(57, 20)
            Me.LbNewVer.TabIndex = 18
            Me.LbNewVer.Text = "2.3.0.0"
            '
            'BtnCancel
            '
            Me.BtnCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.BtnCancel.BackColor = System.Drawing.Color.FromArgb(CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer))
            Me.BtnCancel.Cursor = System.Windows.Forms.Cursors.Hand
            Me.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.BtnCancel.FlatAppearance.BorderSize = 0
            Me.BtnCancel.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(65, Byte), Integer), CType(CType(65, Byte), Integer), CType(CType(65, Byte), Integer))
            Me.BtnCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer))
            Me.BtnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.BtnCancel.Font = New System.Drawing.Font(OpenSansLight, 11.25!)
            Me.BtnCancel.ForeColor = System.Drawing.Color.White
            Me.BtnCancel.Location = New System.Drawing.Point(258, 197)
            Me.BtnCancel.Name = "BtnCancel"
            Me.BtnCancel.Size = New System.Drawing.Size(126, 50)
            Me.BtnCancel.TabIndex = 1
            Me.BtnCancel.Text = "Maybe Later"
            Me.BtnCancel.UseVisualStyleBackColor = False
            '
            'BtnOK
            '
            Me.BtnOK.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.BtnOK.BackColor = System.Drawing.Color.FromArgb(CType(CType(160, Byte), Integer), CType(CType(70, Byte), Integer), CType(CType(10, Byte), Integer))
            Me.BtnOK.Cursor = System.Windows.Forms.Cursors.Hand
            Me.BtnOK.FlatAppearance.BorderSize = 0
            Me.BtnOK.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(200, Byte), Integer), CType(CType(80, Byte), Integer), CType(CType(20, Byte), Integer))
            Me.BtnOK.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(160, Byte), Integer), CType(CType(70, Byte), Integer), CType(CType(10, Byte), Integer))
            Me.BtnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.BtnOK.Font = New System.Drawing.Font(OpenSansLight, 11.25!)
            Me.BtnOK.ForeColor = System.Drawing.Color.White
            Me.BtnOK.Location = New System.Drawing.Point(381, 197)
            Me.BtnOK.Name = "BtnOK"
            Me.BtnOK.Size = New System.Drawing.Size(100, 52)
            Me.BtnOK.TabIndex = 0
            Me.BtnOK.Text = "Update"
            Me.BtnOK.UseVisualStyleBackColor = False
            '
            'LbOldVer
            '
            Me.LbOldVer.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
            Me.LbOldVer.AutoSize = True
            Me.LbOldVer.Font = New System.Drawing.Font(OpenSans, 11.25!)
            Me.LbOldVer.ForeColor = System.Drawing.Color.FromArgb(CType(CType(250, Byte), Integer), CType(CType(250, Byte), Integer), CType(CType(250, Byte), Integer))
            Me.LbOldVer.Location = New System.Drawing.Point(128, 194)
            Me.LbOldVer.Name = "LbOldVer"
            Me.LbOldVer.Size = New System.Drawing.Size(57, 20)
            Me.LbOldVer.TabIndex = 23
            Me.LbOldVer.Text = "2.3.0.0"
            '
            'Label5
            '
            Me.Label5.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
            Me.Label5.AutoSize = True
            Me.Label5.Font = New System.Drawing.Font(OpenSans, 11.25!)
            Me.Label5.ForeColor = System.Drawing.Color.FromArgb(CType(CType(250, Byte), Integer), CType(CType(250, Byte), Integer), CType(CType(250, Byte), Integer))
            Me.Label5.Location = New System.Drawing.Point(30, 194)
            Me.Label5.Name = "Label5"
            Me.Label5.Size = New System.Drawing.Size(94, 20)
            Me.Label5.TabIndex = 22
            Me.Label5.Text = "Old Version:"
            '
            'Tmr
            '
            Me.Tmr.Enabled = True
            Me.Tmr.Interval = 50
            '
            'DiagUpdateAvailable
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(9.0!, 20.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(45, Byte), Integer), CType(CType(45, Byte), Integer), CType(CType(45, Byte), Integer))
            Me.CancelButton = Me.BtnCancel
            Me.ClientSize = New System.Drawing.Size(479, 247)
            Me.Controls.Add(Me.LbOldVer)
            Me.Controls.Add(Me.Label5)
            Me.Controls.Add(Me.BtnOK)
            Me.Controls.Add(Me.BtnCancel)
            Me.Controls.Add(Me.LbNewVer)
            Me.Controls.Add(Me.Label3)
            Me.Controls.Add(Me.PbLogo)
            Me.Controls.Add(Me.Label2)
            Me.Controls.Add(Me.Label1)
            Me.Font = New System.Drawing.Font(OpenSans, 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
            Me.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "DiagUpdateAvailable"
            Me.Opacity = 0R
            Me.ShowIcon = False
            Me.ShowInTaskbar = False
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
            Me.Text = "Update Available"
            Me.TopMost = True
            CType(Me.PbLogo, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub

        Friend WithEvents Label1 As Label
        Friend WithEvents Label2 As Label
        Friend WithEvents PbLogo As PictureBox
        Friend WithEvents Label3 As Label
        Friend WithEvents LbNewVer As Label
        Friend WithEvents BtnCancel As Button
        Friend WithEvents BtnOK As Button
        Friend WithEvents LbOldVer As Label
        Friend WithEvents Label5 As Label
        Friend WithEvents Tmr As Timer
    End Class
End Namespace
