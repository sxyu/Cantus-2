Namespace UI.Updater
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class FrmUpdate
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
            Me.bw = New System.ComponentModel.BackgroundWorker()
            Me.BtnCancel = New System.Windows.Forms.Button()
            Me.LbStep = New System.Windows.Forms.Label()
            Me.LbTitle = New System.Windows.Forms.Label()
            Me.LbFile = New System.Windows.Forms.Label()
            Me.logo = New System.Windows.Forms.PictureBox()
            Me.LbVer = New System.Windows.Forms.Label()
            Me.LbDlSize = New System.Windows.Forms.Label()
            Me.LbSpeed = New System.Windows.Forms.Label()
            Me.LbTSize = New System.Windows.Forms.Label()
            Me.pb = New Cantus.UI.UserControls.FlatProgressBar()
            CType(Me.logo, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'bw
            '
            Me.bw.WorkerReportsProgress = True
            Me.bw.WorkerSupportsCancellation = True
            '
            'BtnCancel
            '
            Me.BtnCancel.BackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.BtnCancel.Cursor = System.Windows.Forms.Cursors.Hand
            Me.BtnCancel.FlatAppearance.BorderSize = 0
            Me.BtnCancel.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(70, Byte), Integer), CType(CType(70, Byte), Integer), CType(CType(70, Byte), Integer))
            Me.BtnCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.BtnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.BtnCancel.ForeColor = System.Drawing.Color.White
            Me.BtnCancel.Location = New System.Drawing.Point(625, 257)
            Me.BtnCancel.Margin = New System.Windows.Forms.Padding(32, 14, 32, 14)
            Me.BtnCancel.Name = "BtnCancel"
            Me.BtnCancel.Size = New System.Drawing.Size(130, 40)
            Me.BtnCancel.TabIndex = 20
            Me.BtnCancel.Text = "Cancel"
            Me.BtnCancel.UseVisualStyleBackColor = False
            '
            'LbStep
            '
            Me.LbStep.AutoSize = True
            Me.LbStep.Font = New System.Drawing.Font(OpenSansLight, 12.0!)
            Me.LbStep.ForeColor = System.Drawing.Color.White
            Me.LbStep.Location = New System.Drawing.Point(447, 142)
            Me.LbStep.Margin = New System.Windows.Forms.Padding(32, 0, 32, 0)
            Me.LbStep.Name = "LbStep"
            Me.LbStep.Size = New System.Drawing.Size(50, 22)
            Me.LbStep.TabIndex = 22
            Me.LbStep.Text = "1 of 1"
            '
            'LbTitle
            '
            Me.LbTitle.AutoSize = True
            Me.LbTitle.Font = New System.Drawing.Font(OpenSans, 14.25!)
            Me.LbTitle.ForeColor = System.Drawing.Color.White
            Me.LbTitle.Location = New System.Drawing.Point(99, 30)
            Me.LbTitle.Margin = New System.Windows.Forms.Padding(32, 0, 32, 0)
            Me.LbTitle.Name = "LbTitle"
            Me.LbTitle.Size = New System.Drawing.Size(155, 26)
            Me.LbTitle.TabIndex = 14
            Me.LbTitle.Text = "Cantus Updater"
            '
            'LbFile
            '
            Me.LbFile.AutoSize = True
            Me.LbFile.Font = New System.Drawing.Font(OpenSansLight, 12.0!)
            Me.LbFile.ForeColor = System.Drawing.Color.White
            Me.LbFile.Location = New System.Drawing.Point(447, 113)
            Me.LbFile.Margin = New System.Windows.Forms.Padding(32, 0, 32, 0)
            Me.LbFile.Name = "LbFile"
            Me.LbFile.Size = New System.Drawing.Size(86, 22)
            Me.LbFile.TabIndex = 21
            Me.LbFile.Text = "cantus.exe"
            '
            'logo
            '
            Me.logo.Location = New System.Drawing.Point(41, 23)
            Me.logo.Margin = New System.Windows.Forms.Padding(32, 14, 32, 14)
            Me.logo.Name = "logo"
            Me.logo.Size = New System.Drawing.Size(44, 43)
            Me.logo.TabIndex = 15
            Me.logo.TabStop = False
            '
            'LbVer
            '
            Me.LbVer.Font = New System.Drawing.Font(OpenSansLight, 12.0!)
            Me.LbVer.ForeColor = System.Drawing.Color.White
            Me.LbVer.Location = New System.Drawing.Point(63, 91)
            Me.LbVer.Margin = New System.Windows.Forms.Padding(32, 0, 32, 0)
            Me.LbVer.Name = "LbVer"
            Me.LbVer.Size = New System.Drawing.Size(259, 21)
            Me.LbVer.TabIndex = 16
            Me.LbVer.Text = "Version"
            Me.LbVer.TextAlign = System.Drawing.ContentAlignment.MiddleRight
            '
            'LbDlSize
            '
            Me.LbDlSize.Font = New System.Drawing.Font(OpenSansLight, 12.0!)
            Me.LbDlSize.ForeColor = System.Drawing.Color.White
            Me.LbDlSize.Location = New System.Drawing.Point(63, 138)
            Me.LbDlSize.Margin = New System.Windows.Forms.Padding(32, 0, 32, 0)
            Me.LbDlSize.Name = "LbDlSize"
            Me.LbDlSize.Size = New System.Drawing.Size(259, 29)
            Me.LbDlSize.TabIndex = 19
            Me.LbDlSize.Text = "Downloaded Size"
            Me.LbDlSize.TextAlign = System.Drawing.ContentAlignment.MiddleRight
            '
            'LbSpeed
            '
            Me.LbSpeed.Font = New System.Drawing.Font(OpenSansLight, 12.0!)
            Me.LbSpeed.ForeColor = System.Drawing.Color.White
            Me.LbSpeed.Location = New System.Drawing.Point(67, 113)
            Me.LbSpeed.Margin = New System.Windows.Forms.Padding(32, 0, 32, 0)
            Me.LbSpeed.Name = "LbSpeed"
            Me.LbSpeed.Size = New System.Drawing.Size(255, 29)
            Me.LbSpeed.TabIndex = 17
            Me.LbSpeed.Text = "Speed"
            Me.LbSpeed.TextAlign = System.Drawing.ContentAlignment.MiddleRight
            '
            'LbTSize
            '
            Me.LbTSize.Font = New System.Drawing.Font(OpenSansLight, 12.0!)
            Me.LbTSize.ForeColor = System.Drawing.Color.White
            Me.LbTSize.Location = New System.Drawing.Point(59, 163)
            Me.LbTSize.Margin = New System.Windows.Forms.Padding(32, 0, 32, 0)
            Me.LbTSize.Name = "LbTSize"
            Me.LbTSize.Size = New System.Drawing.Size(263, 29)
            Me.LbTSize.TabIndex = 18
            Me.LbTSize.Text = "Total Size"
            Me.LbTSize.TextAlign = System.Drawing.ContentAlignment.MiddleRight
            '
            'pb
            '
            Me.pb.BackColor = System.Drawing.Color.Gainsboro
            Me.pb.DrawBorder = False
            Me.pb.Location = New System.Drawing.Point(0, 212)
            Me.pb.Margin = New System.Windows.Forms.Padding(32, 14, 32, 14)
            Me.pb.Name = "pb"
            Me.pb.ProgressColor = System.Drawing.Color.MediumSeaGreen
            Me.pb.ProgressText = ""
            Me.pb.Size = New System.Drawing.Size(771, 35)
            Me.pb.TabIndex = 23
            Me.pb.Vertical = False
            '
            'FrmUpdate
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
            Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer))
            Me.ClientSize = New System.Drawing.Size(770, 309)
            Me.Controls.Add(Me.pb)
            Me.Controls.Add(Me.BtnCancel)
            Me.Controls.Add(Me.LbStep)
            Me.Controls.Add(Me.LbTitle)
            Me.Controls.Add(Me.LbFile)
            Me.Controls.Add(Me.logo)
            Me.Controls.Add(Me.LbVer)
            Me.Controls.Add(Me.LbDlSize)
            Me.Controls.Add(Me.LbSpeed)
            Me.Controls.Add(Me.LbTSize)
            Me.Font = New System.Drawing.Font(OpenSans, 12.0!)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
            Me.Margin = New System.Windows.Forms.Padding(43, 23, 43, 23)
            Me.Name = "FrmUpdate"
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
            Me.Text = "Entity System Patcher"
            CType(Me.logo, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Friend WithEvents bw As System.ComponentModel.BackgroundWorker
        Friend WithEvents pb As UI.UserControls.FlatProgressBar
        Friend WithEvents BtnCancel As System.Windows.Forms.Button
        Friend WithEvents LbStep As System.Windows.Forms.Label
        Friend WithEvents LbTitle As System.Windows.Forms.Label
        Friend WithEvents LbFile As System.Windows.Forms.Label
        Friend WithEvents logo As System.Windows.Forms.PictureBox
        Friend WithEvents LbVer As System.Windows.Forms.Label
        Friend WithEvents LbDlSize As System.Windows.Forms.Label
        Friend WithEvents LbSpeed As System.Windows.Forms.Label
        Friend WithEvents LbTSize As System.Windows.Forms.Label

    End Class
End Namespace
