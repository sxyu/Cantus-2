Namespace UI
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class Viewer
        Inherits System.Windows.Forms.UserControl

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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Viewer))
            Me.LbTitle = New System.Windows.Forms.Label()
            Me.BtnMin = New System.Windows.Forms.Button()
            Me.BtnConsole = New System.Windows.Forms.Button()
            Me.BtnGraph = New System.Windows.Forms.Button()
            Me.Pnl = New System.Windows.Forms.Panel()
            Me.tt = New System.Windows.Forms.ToolTip(Me.components)
            Me.PbLogo = New System.Windows.Forms.PictureBox()
            CType(Me.PbLogo, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'LbTitle
            '
            Me.LbTitle.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.LbTitle.AutoSize = True
            Me.LbTitle.ForeColor = System.Drawing.Color.LightGray
            Me.LbTitle.Location = New System.Drawing.Point(447, 15)
            Me.LbTitle.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
            Me.LbTitle.Name = "LbTitle"
            Me.LbTitle.Size = New System.Drawing.Size(96, 20)
            Me.LbTitle.TabIndex = 0
            Me.LbTitle.Text = "Cantus {VER}"
            '
            'BtnMin
            '
            Me.BtnMin.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.BtnMin.BackColor = System.Drawing.Color.FromArgb(CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer))
            Me.BtnMin.Cursor = System.Windows.Forms.Cursors.Hand
            Me.BtnMin.FlatAppearance.BorderSize = 0
            Me.BtnMin.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(65, Byte), Integer), CType(CType(65, Byte), Integer), CType(CType(65, Byte), Integer))
            Me.BtnMin.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer))
            Me.BtnMin.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.BtnMin.ForeColor = System.Drawing.Color.White
            Me.BtnMin.Location = New System.Drawing.Point(568, -1)
            Me.BtnMin.Name = "BtnMin"
            Me.BtnMin.Size = New System.Drawing.Size(53, 50)
            Me.BtnMin.TabIndex = 6
            Me.BtnMin.TabStop = False
            Me.BtnMin.Text = "-"
            Me.BtnMin.UseVisualStyleBackColor = False
            Me.BtnMin.Visible = False
            '
            'BtnConsole
            '
            Me.BtnConsole.BackColor = System.Drawing.Color.FromArgb(CType(CType(37, Byte), Integer), CType(CType(37, Byte), Integer), CType(CType(37, Byte), Integer))
            Me.BtnConsole.Cursor = System.Windows.Forms.Cursors.Hand
            Me.BtnConsole.FlatAppearance.BorderSize = 0
            Me.BtnConsole.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(80, Byte), Integer), CType(CType(80, Byte), Integer), CType(CType(80, Byte), Integer))
            Me.BtnConsole.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.BtnConsole.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.BtnConsole.ForeColor = System.Drawing.Color.White
            Me.BtnConsole.Image = CType(resources.GetObject("BtnConsole.Image"), System.Drawing.Image)
            Me.BtnConsole.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
            Me.BtnConsole.Location = New System.Drawing.Point(1, 0)
            Me.BtnConsole.Name = "BtnConsole"
            Me.BtnConsole.Padding = New System.Windows.Forms.Padding(0, 0, 10, 0)
            Me.BtnConsole.Size = New System.Drawing.Size(135, 48)
            Me.BtnConsole.TabIndex = 1
            Me.BtnConsole.Tag = "console"
            Me.BtnConsole.Text = "Console"
            Me.BtnConsole.TextAlign = System.Drawing.ContentAlignment.MiddleRight
            Me.tt.SetToolTip(Me.BtnConsole, "See saved results (Alt+R)")
            Me.BtnConsole.UseVisualStyleBackColor = False
            '
            'BtnGraph
            '
            Me.BtnGraph.BackColor = System.Drawing.Color.FromArgb(CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer))
            Me.BtnGraph.Cursor = System.Windows.Forms.Cursors.Hand
            Me.BtnGraph.FlatAppearance.BorderSize = 0
            Me.BtnGraph.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(65, Byte), Integer), CType(CType(65, Byte), Integer), CType(CType(65, Byte), Integer))
            Me.BtnGraph.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer))
            Me.BtnGraph.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.BtnGraph.ForeColor = System.Drawing.Color.White
            Me.BtnGraph.Image = CType(resources.GetObject("BtnGraph.Image"), System.Drawing.Image)
            Me.BtnGraph.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
            Me.BtnGraph.Location = New System.Drawing.Point(137, 1)
            Me.BtnGraph.Name = "BtnGraph"
            Me.BtnGraph.Padding = New System.Windows.Forms.Padding(0, 0, 10, 0)
            Me.BtnGraph.Size = New System.Drawing.Size(124, 48)
            Me.BtnGraph.TabIndex = 2
            Me.BtnGraph.Tag = "graphing"
            Me.BtnGraph.Text = "Graph"
            Me.BtnGraph.TextAlign = System.Drawing.ContentAlignment.MiddleRight
            Me.tt.SetToolTip(Me.BtnGraph, "Draw graphs (Alt+G)")
            Me.BtnGraph.UseVisualStyleBackColor = False
            '
            'Pnl
            '
            Me.Pnl.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.Pnl.BackColor = System.Drawing.Color.FromArgb(CType(CType(37, Byte), Integer), CType(CType(37, Byte), Integer), CType(CType(37, Byte), Integer))
            Me.Pnl.Location = New System.Drawing.Point(1, 48)
            Me.Pnl.Name = "Pnl"
            Me.Pnl.Size = New System.Drawing.Size(621, 651)
            Me.Pnl.TabIndex = 0
            '
            'PbLogo
            '
            Me.PbLogo.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.PbLogo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
            Me.PbLogo.Image = Global.Cantus.My.Resources.Resources.Cantus_Logo
            Me.PbLogo.Location = New System.Drawing.Point(411, 10)
            Me.PbLogo.Name = "PbLogo"
            Me.PbLogo.Size = New System.Drawing.Size(29, 29)
            Me.PbLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
            Me.PbLogo.TabIndex = 15
            Me.PbLogo.TabStop = False
            '
            'Viewer
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
            Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer))
            Me.Controls.Add(Me.Pnl)
            Me.Controls.Add(Me.BtnGraph)
            Me.Controls.Add(Me.BtnConsole)
            Me.Controls.Add(Me.BtnMin)
            Me.Controls.Add(Me.LbTitle)
            Me.Controls.Add(Me.PbLogo)
            Me.Font = New System.Drawing.Font(OpenSansLight, 11.0!)
            Me.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
            Me.Name = "Viewer"
            Me.Size = New System.Drawing.Size(621, 700)
            CType(Me.PbLogo, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub

        Friend WithEvents LbTitle As Label
        Friend WithEvents BtnMin As Button
        Friend WithEvents BtnConsole As Button
        Friend WithEvents BtnGraph As Button
        Friend WithEvents Pnl As Panel
        Friend WithEvents tt As ToolTip
        Friend WithEvents PbLogo As PictureBox
    End Class
End Namespace
