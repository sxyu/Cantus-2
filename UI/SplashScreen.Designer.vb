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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(SplashScreen))
            Me.lbName = New System.Windows.Forms.Label()
            Me.lbLoading = New System.Windows.Forms.Label()
            Me.lbVer = New System.Windows.Forms.Label()
            Me.lbCredits = New System.Windows.Forms.Label()
            Me.lbVerText = New System.Windows.Forms.Label()
            Me.lbDescription = New System.Windows.Forms.Label()
            Me.lbWelcome = New System.Windows.Forms.Label()
            Me.Progress = New Cantus.UI.UserControls.FlatProgressBar()
            Me.SuspendLayout()
            '
            'lbName
            '
            Me.lbName.AutoSize = True
            Me.lbName.BackColor = System.Drawing.Color.Transparent
            Me.lbName.Font = New System.Drawing.Font("Segoe UI Light", 40.0!)
            Me.lbName.ForeColor = System.Drawing.Color.White
            Me.lbName.Location = New System.Drawing.Point(97, 148)
            Me.lbName.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
            Me.lbName.Name = "lbName"
            Me.lbName.Size = New System.Drawing.Size(186, 72)
            Me.lbName.TabIndex = 0
            Me.lbName.Text = "Cantus"
            '
            'lbLoading
            '
            Me.lbLoading.AutoSize = True
            Me.lbLoading.BackColor = System.Drawing.Color.Transparent
            Me.lbLoading.Font = New System.Drawing.Font("Segoe UI Semilight", 12.0!)
            Me.lbLoading.ForeColor = System.Drawing.Color.White
            Me.lbLoading.Location = New System.Drawing.Point(495, 305)
            Me.lbLoading.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
            Me.lbLoading.Name = "lbLoading"
            Me.lbLoading.Size = New System.Drawing.Size(77, 21)
            Me.lbLoading.TabIndex = 1
            Me.lbLoading.Text = "Loading..."
            '
            'lbVer
            '
            Me.lbVer.AutoSize = True
            Me.lbVer.BackColor = System.Drawing.Color.Transparent
            Me.lbVer.Font = New System.Drawing.Font("Segoe UI Semilight", 12.0!)
            Me.lbVer.ForeColor = System.Drawing.Color.LightGray
            Me.lbVer.Location = New System.Drawing.Point(205, 305)
            Me.lbVer.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
            Me.lbVer.Name = "lbVer"
            Me.lbVer.Size = New System.Drawing.Size(54, 21)
            Me.lbVer.TabIndex = 2
            Me.lbVer.Text = "2.0.0.0"
            '
            'lbCredits
            '
            Me.lbCredits.AutoSize = True
            Me.lbCredits.BackColor = System.Drawing.Color.Transparent
            Me.lbCredits.Font = New System.Drawing.Font("Segoe UI Semilight", 12.0!)
            Me.lbCredits.ForeColor = System.Drawing.Color.LightGray
            Me.lbCredits.Location = New System.Drawing.Point(13, 305)
            Me.lbCredits.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
            Me.lbCredits.Name = "lbCredits"
            Me.lbCredits.Size = New System.Drawing.Size(114, 21)
            Me.lbCredits.TabIndex = 4
            Me.lbCredits.Text = "Alex Yu 2015-16"
            '
            'lbVerText
            '
            Me.lbVerText.AutoSize = True
            Me.lbVerText.BackColor = System.Drawing.Color.Transparent
            Me.lbVerText.Font = New System.Drawing.Font("Segoe UI Semilight", 12.0!)
            Me.lbVerText.ForeColor = System.Drawing.Color.LightGray
            Me.lbVerText.Location = New System.Drawing.Point(149, 305)
            Me.lbVerText.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
            Me.lbVerText.Name = "lbVerText"
            Me.lbVerText.Size = New System.Drawing.Size(61, 21)
            Me.lbVerText.TabIndex = 5
            Me.lbVerText.Text = "Version"
            '
            'lbDescription
            '
            Me.lbDescription.BackColor = System.Drawing.Color.Transparent
            Me.lbDescription.Font = New System.Drawing.Font("Segoe UI Semilight", 12.0!)
            Me.lbDescription.ForeColor = System.Drawing.Color.White
            Me.lbDescription.Location = New System.Drawing.Point(314, 123)
            Me.lbDescription.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
            Me.lbDescription.Name = "lbDescription"
            Me.lbDescription.Size = New System.Drawing.Size(153, 120)
            Me.lbDescription.TabIndex = 6
            Me.lbDescription.Text = "A powerful mathematical expression evaluator and graphing calculator."
            '
            'lbWelcome
            '
            Me.lbWelcome.AutoSize = True
            Me.lbWelcome.BackColor = System.Drawing.Color.Transparent
            Me.lbWelcome.Font = New System.Drawing.Font("Segoe UI Semilight", 12.0!)
            Me.lbWelcome.ForeColor = System.Drawing.Color.White
            Me.lbWelcome.Location = New System.Drawing.Point(177, 127)
            Me.lbWelcome.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
            Me.lbWelcome.Name = "lbWelcome"
            Me.lbWelcome.Size = New System.Drawing.Size(92, 21)
            Me.lbWelcome.TabIndex = 7
            Me.lbWelcome.Text = "Welcome to"
            '
            'Progress
            '
            Me.Progress.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.Progress.BackColor = System.Drawing.Color.FromArgb(CType(CType(45, Byte), Integer), CType(CType(45, Byte), Integer), CType(CType(45, Byte), Integer))
            Me.Progress.DrawBorder = False
            Me.Progress.Location = New System.Drawing.Point(0, 0)
            Me.Progress.Name = "Progress"
            Me.Progress.ProgressColor = System.Drawing.Color.DarkOliveGreen
            Me.Progress.ProgressText = ""
            Me.Progress.Size = New System.Drawing.Size(592, 12)
            Me.Progress.Style = System.Windows.Forms.ProgressBarStyle.Continuous
            Me.Progress.TabIndex = 3
            Me.Progress.Vertical = False
            '
            'SplashScreen
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(9.0!, 21.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer))
            Me.BackgroundImage = CType(resources.GetObject("$this.BackgroundImage"), System.Drawing.Image)
            Me.ClientSize = New System.Drawing.Size(585, 335)
            Me.Controls.Add(Me.lbWelcome)
            Me.Controls.Add(Me.lbDescription)
            Me.Controls.Add(Me.lbVerText)
            Me.Controls.Add(Me.lbCredits)
            Me.Controls.Add(Me.Progress)
            Me.Controls.Add(Me.lbVer)
            Me.Controls.Add(Me.lbLoading)
            Me.Controls.Add(Me.lbName)
            Me.Font = New System.Drawing.Font("Segoe UI Semilight", 12.0!)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
            Me.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "SplashScreen"
            Me.ShowIcon = False
            Me.ShowInTaskbar = False
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
            Me.Text = "SplashScreen"
            Me.TopMost = False
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub

        Friend WithEvents lbName As Label
        Friend WithEvents lbLoading As Label
        Friend WithEvents lbVer As Label
        Friend WithEvents Progress As UserControls.FlatProgressBar
        Friend WithEvents lbCredits As Label
        Friend WithEvents lbVerText As Label
        Friend WithEvents lbDescription As Label
        Friend WithEvents lbWelcome As Label
    End Class
End Namespace
