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
            Me.LbName = New System.Windows.Forms.Label()
            Me.LbVer = New System.Windows.Forms.Label()
            Me.LbCredits = New System.Windows.Forms.Label()
            Me.PbSettingsLogo = New System.Windows.Forms.PictureBox()
            Me.TmrAnim = New System.Windows.Forms.Timer(Me.components)
            Me.LbWelcome = New System.Windows.Forms.Label()
            Me.Progress = New Cantus.UI.UserControls.FlatProgressBar()
            CType(Me.PbSettingsLogo, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'LbName
            '
            Me.LbName.Anchor = System.Windows.Forms.AnchorStyles.None
            Me.LbName.AutoSize = True
            Me.LbName.BackColor = System.Drawing.Color.Transparent
            Me.LbName.Font = New System.Drawing.Font(OpenSansLight, 40.0!)
            Me.LbName.ForeColor = System.Drawing.Color.White
            Me.LbName.Location = New System.Drawing.Point(250, 130)
            Me.LbName.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
            Me.LbName.Name = "LbName"
            Me.LbName.Size = New System.Drawing.Size(202, 74)
            Me.LbName.TabIndex = 0
            Me.LbName.Text = "Cantus"
            '
            'LbVer
            '
            Me.LbVer.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
            Me.LbVer.AutoSize = True
            Me.LbVer.BackColor = System.Drawing.Color.Transparent
            Me.LbVer.Font = New System.Drawing.Font(OpenSansLight, 11.0!)
            Me.LbVer.ForeColor = System.Drawing.Color.LightGray
            Me.LbVer.Location = New System.Drawing.Point(13, 320)
            Me.LbVer.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
            Me.LbVer.Name = "LbVer"
            Me.LbVer.Size = New System.Drawing.Size(57, 20)
            Me.LbVer.TabIndex = 2
            Me.LbVer.Text = "2.0.0.0"
            '
            'LbCredits
            '
            Me.LbCredits.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
            Me.LbCredits.AutoSize = True
            Me.LbCredits.BackColor = System.Drawing.Color.Transparent
            Me.LbCredits.Font = New System.Drawing.Font(OpenSans, 11.0!)
            Me.LbCredits.ForeColor = System.Drawing.Color.Gray
            Me.LbCredits.Location = New System.Drawing.Point(450, 320)
            Me.LbCredits.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
            Me.LbCredits.Name = "LbCredits"
            Me.LbCredits.Size = New System.Drawing.Size(122, 20)
            Me.LbCredits.TabIndex = 4
            Me.LbCredits.Text = "Alex Yu 2015-16"
            '
            'PbSettingsLogo
            '
            Me.PbSettingsLogo.BackColor = System.Drawing.Color.Transparent
            Me.PbSettingsLogo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
            Me.PbSettingsLogo.Image = Global.Cantus.My.Resources.Resources.Cantus_Logo_Large
            Me.PbSettingsLogo.Location = New System.Drawing.Point(94, 120)
            Me.PbSettingsLogo.Name = "PbSettingsLogo"
            Me.PbSettingsLogo.Size = New System.Drawing.Size(151, 127)
            Me.PbSettingsLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
            Me.PbSettingsLogo.TabIndex = 16
            Me.PbSettingsLogo.TabStop = False
            '
            'TmrAnim
            '
            Me.TmrAnim.Interval = 10
            '
            'LbWelcome
            '
            Me.LbWelcome.Anchor = System.Windows.Forms.AnchorStyles.None
            Me.LbWelcome.AutoSize = True
            Me.LbWelcome.BackColor = System.Drawing.Color.Transparent
            Me.LbWelcome.Font = New System.Drawing.Font(OpenSansLight, 10.0!)
            Me.LbWelcome.ForeColor = System.Drawing.Color.White
            Me.LbWelcome.Location = New System.Drawing.Point(261, 204)
            Me.LbWelcome.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
            Me.LbWelcome.Name = "LbWelcome"
            Me.LbWelcome.Size = New System.Drawing.Size(242, 19)
            Me.LbWelcome.TabIndex = 17
            Me.LbWelcome.Text = "Mathematical Programming Language"
            '
            'Progress
            '
            Me.Progress.BackColor = System.Drawing.Color.FromArgb(CType(CType(45, Byte), Integer), CType(CType(45, Byte), Integer), CType(CType(45, Byte), Integer))
            Me.Progress.Dock = System.Windows.Forms.DockStyle.Top
            Me.Progress.DrawBorder = False
            Me.Progress.ForeColor = System.Drawing.Color.White
            Me.Progress.Location = New System.Drawing.Point(0, 0)
            Me.Progress.Name = "Progress"
            Me.Progress.ProgressColor = System.Drawing.Color.DarkOliveGreen
            Me.Progress.ProgressText = ""
            Me.Progress.Size = New System.Drawing.Size(585, 30)
            Me.Progress.Style = System.Windows.Forms.ProgressBarStyle.Continuous
            Me.Progress.TabIndex = 3
            Me.Progress.Tag = ""
            Me.Progress.Vertical = False
            '
            'SplashScreen
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
            Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer))
            Me.BackgroundImage = CType(resources.GetObject("$this.BackgroundImage"), System.Drawing.Image)
            Me.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
            Me.ClientSize = New System.Drawing.Size(585, 349)
            Me.Controls.Add(Me.Progress)
            Me.Controls.Add(Me.LbWelcome)
            Me.Controls.Add(Me.LbVer)
            Me.Controls.Add(Me.LbCredits)
            Me.Controls.Add(Me.PbSettingsLogo)
            Me.Controls.Add(Me.LbName)
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

        Friend WithEvents LbName As Label
        Friend WithEvents LbVer As Label
        Friend WithEvents Progress As UserControls.FlatProgressBar
        Friend WithEvents LbCredits As Label
        Friend WithEvents PbSettingsLogo As PictureBox
        Friend WithEvents TmrAnim As Timer
        Friend WithEvents LbWelcome As Label
    End Class
End Namespace
