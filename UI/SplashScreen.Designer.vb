Namespace UI
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class SplashScreen
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(SplashScreen))
            Me.lbName = New System.Windows.Forms.Label()
            Me.lbVer = New System.Windows.Forms.Label()
            Me.lbCredits = New System.Windows.Forms.Label()
            Me.lbVerText = New System.Windows.Forms.Label()
            Me.lbWelcome = New System.Windows.Forms.Label()
            Me.PbSettingsLogo = New System.Windows.Forms.PictureBox()
            Me.TmrAnim = New System.Windows.Forms.Timer(Me.components)
            Me.Progress = New Cantus.UI.UserControls.FlatProgressBar()
            CType(Me.PbSettingsLogo, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'lbName
            '
            Me.lbName.Anchor = System.Windows.Forms.AnchorStyles.None
            Me.lbName.AutoSize = True
            Me.lbName.BackColor = System.Drawing.Color.Transparent
            Me.lbName.Font = New System.Drawing.Font(OpenSansLight, 40.0!)
            Me.lbName.ForeColor = System.Drawing.Color.White
            Me.lbName.Location = New System.Drawing.Point(284, 132)
            Me.lbName.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
            Me.lbName.Name = "lbName"
            Me.lbName.Size = New System.Drawing.Size(202, 74)
            Me.lbName.TabIndex = 0
            Me.lbName.Text = "Cantus"
            '
            'lbVer
            '
            Me.lbVer.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
            Me.lbVer.AutoSize = True
            Me.lbVer.BackColor = System.Drawing.Color.Transparent
            Me.lbVer.Font = New System.Drawing.Font(OpenSansLight, 12.0!)
            Me.lbVer.ForeColor = System.Drawing.Color.LightGray
            Me.lbVer.Location = New System.Drawing.Point(77, 322)
            Me.lbVer.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
            Me.lbVer.Name = "lbVer"
            Me.lbVer.Size = New System.Drawing.Size(58, 22)
            Me.lbVer.TabIndex = 2
            Me.lbVer.Text = "2.0.0.0"
            '
            'lbCredits
            '
            Me.lbCredits.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
            Me.lbCredits.AutoSize = True
            Me.lbCredits.BackColor = System.Drawing.Color.Transparent
            Me.lbCredits.Font = New System.Drawing.Font(OpenSansLight, 12.0!)
            Me.lbCredits.ForeColor = System.Drawing.Color.LightGray
            Me.lbCredits.Location = New System.Drawing.Point(447, 322)
            Me.lbCredits.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
            Me.lbCredits.Name = "lbCredits"
            Me.lbCredits.Size = New System.Drawing.Size(125, 22)
            Me.lbCredits.TabIndex = 4
            Me.lbCredits.Text = "Alex Yu 2015-16"
            '
            'lbVerText
            '
            Me.lbVerText.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
            Me.lbVerText.AutoSize = True
            Me.lbVerText.BackColor = System.Drawing.Color.Transparent
            Me.lbVerText.Font = New System.Drawing.Font(OpenSansLight, 12.0!)
            Me.lbVerText.ForeColor = System.Drawing.Color.LightGray
            Me.lbVerText.Location = New System.Drawing.Point(13, 322)
            Me.lbVerText.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
            Me.lbVerText.Name = "lbVerText"
            Me.lbVerText.Size = New System.Drawing.Size(63, 22)
            Me.lbVerText.TabIndex = 5
            Me.lbVerText.Text = "Version"
            '
            'lbWelcome
            '
            Me.lbWelcome.Anchor = System.Windows.Forms.AnchorStyles.None
            Me.lbWelcome.AutoSize = True
            Me.lbWelcome.BackColor = System.Drawing.Color.Transparent
            Me.lbWelcome.Font = New System.Drawing.Font(OpenSansLight, 12.0!)
            Me.lbWelcome.ForeColor = System.Drawing.Color.White
            Me.lbWelcome.Location = New System.Drawing.Point(296, 202)
            Me.lbWelcome.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
            Me.lbWelcome.Name = "lbWelcome"
            Me.lbWelcome.Size = New System.Drawing.Size(177, 22)
            Me.lbWelcome.TabIndex = 7
            Me.lbWelcome.Text = "Mathematical Language"
            '
            'PbSettingsLogo
            '
            Me.PbSettingsLogo.BackColor = System.Drawing.Color.Transparent
            Me.PbSettingsLogo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
            Me.PbSettingsLogo.Image = Global.Cantus.My.Resources.Resources.Cantus_Logo_Large
            Me.PbSettingsLogo.Location = New System.Drawing.Point(133, 122)
            Me.PbSettingsLogo.Name = "PbSettingsLogo"
            Me.PbSettingsLogo.Size = New System.Drawing.Size(133, 123)
            Me.PbSettingsLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
            Me.PbSettingsLogo.TabIndex = 16
            Me.PbSettingsLogo.TabStop = False
            '
            'TmrAnim
            '
            Me.TmrAnim.Interval = 10
            '
            'Progress
            '
            Me.Progress.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.Progress.BackColor = System.Drawing.Color.FromArgb(CType(CType(45, Byte), Integer), CType(CType(45, Byte), Integer), CType(CType(45, Byte), Integer))
            Me.Progress.DrawBorder = False
            Me.Progress.Location = New System.Drawing.Point(0, 0)
            Me.Progress.Name = "Progress"
            Me.Progress.ProgressColor = System.Drawing.Color.DarkOliveGreen
            Me.Progress.ProgressText = ""
            Me.Progress.Size = New System.Drawing.Size(592, 26)
            Me.Progress.Style = System.Windows.Forms.ProgressBarStyle.Continuous
            Me.Progress.TabIndex = 3
            Me.Progress.Vertical = False
            '
            'SplashScreen
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
            Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer))
            Me.BackgroundImage = CType(resources.GetObject("$this.BackgroundImage"), System.Drawing.Image)
            Me.ClientSize = New System.Drawing.Size(585, 355)
            Me.Controls.Add(Me.PbSettingsLogo)
            Me.Controls.Add(Me.lbWelcome)
            Me.Controls.Add(Me.lbVerText)
            Me.Controls.Add(Me.lbCredits)
            Me.Controls.Add(Me.Progress)
            Me.Controls.Add(Me.lbVer)
            Me.Controls.Add(Me.lbName)
            Me.Font = New System.Drawing.Font(OpenSansLight, 12.0!)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
            Me.Margin = New System.Windows.Forms.Padding(4, 6, 4, 6)
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "SplashScreen"
            Me.Opacity = 0R
            Me.ShowIcon = False
            Me.ShowInTaskbar = False
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
            Me.Text = "SplashScreen"
            CType(Me.PbSettingsLogo, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub

        Friend WithEvents lbName As Label
        Friend WithEvents lbVer As Label
        Friend WithEvents Progress As UserControls.FlatProgressBar
        Friend WithEvents lbCredits As Label
        Friend WithEvents lbVerText As Label
        Friend WithEvents lbWelcome As Label
        Friend WithEvents PbSettingsLogo As PictureBox
        Friend WithEvents TmrAnim As Timer
    End Class
End Namespace
