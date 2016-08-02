Namespace UI
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class FrmEditor
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmEditor))
            Me.tb = New ScintillaNET.Scintilla()
            Me.pnlSettings = New System.Windows.Forms.Panel()
            Me.lbSettings = New System.Windows.Forms.Label()
            Me.PictureBox1 = New System.Windows.Forms.PictureBox()
            Me.btnLog = New System.Windows.Forms.Button()
            Me.btnExplicit = New System.Windows.Forms.Button()
            Me.cbAutoUpd = New System.Windows.Forms.CheckBox()
            Me.btnUpdate = New System.Windows.Forms.Button()
            Me.lbAbout = New System.Windows.Forms.Label()
            Me.btnAngleRepr = New System.Windows.Forms.Button()
            Me.btnT = New System.Windows.Forms.Button()
            Me.btnM = New System.Windows.Forms.Button()
            Me.btnOutputFormat = New System.Windows.Forms.Button()
            Me.btnY = New System.Windows.Forms.Button()
            Me.btnX = New System.Windows.Forms.Button()
            Me.btnEval = New System.Windows.Forms.Button()
            Me.lbResult = New System.Windows.Forms.Label()
            Me.pnlResults = New System.Windows.Forms.Panel()
            Me.btnClose = New System.Windows.Forms.Button()
            Me.btnMin = New System.Windows.Forms.Button()
            Me.btnSettings = New System.Windows.Forms.Button()
            Me.TTLetters = New System.Windows.Forms.ToolTip(Me.components)
            Me.btnFunctions = New System.Windows.Forms.Button()
            Me.btnSave = New System.Windows.Forms.Button()
            Me.btnOpen = New System.Windows.Forms.Button()
            Me.pnlTb = New System.Windows.Forms.Panel()
            Me.TmrReCalc = New System.Windows.Forms.Timer(Me.components)
            Me.TmrLoad = New System.Windows.Forms.Timer(Me.components)
            Me.pnlSettings.SuspendLayout()
            CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.pnlResults.SuspendLayout()
            Me.pnlTb.SuspendLayout()
            Me.SuspendLayout()
            '
            'tb
            '
            Me.tb.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.tb.BackColor = System.Drawing.Color.FromArgb(CType(CType(34, Byte), Integer), CType(CType(34, Byte), Integer), CType(CType(34, Byte), Integer))
            Me.tb.BorderStyle = System.Windows.Forms.BorderStyle.None
            Me.tb.CaretForeColor = System.Drawing.Color.GhostWhite
            Me.tb.Cursor = System.Windows.Forms.Cursors.Arrow
            Me.tb.EdgeColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.tb.Font = New System.Drawing.Font("Lucida Console", 13.5!)
            Me.tb.ForeColor = System.Drawing.Color.White
            Me.tb.Location = New System.Drawing.Point(3, 18)
            Me.tb.Name = "tb"
            Me.tb.Size = New System.Drawing.Size(675, 334)
            Me.tb.TabIndex = 0
            '
            'pnlSettings
            '
            Me.pnlSettings.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.pnlSettings.BackColor = System.Drawing.Color.Black
            Me.pnlSettings.Controls.Add(Me.lbSettings)
            Me.pnlSettings.Controls.Add(Me.PictureBox1)
            Me.pnlSettings.Controls.Add(Me.btnLog)
            Me.pnlSettings.Controls.Add(Me.btnExplicit)
            Me.pnlSettings.Controls.Add(Me.cbAutoUpd)
            Me.pnlSettings.Controls.Add(Me.btnUpdate)
            Me.pnlSettings.Controls.Add(Me.lbAbout)
            Me.pnlSettings.Controls.Add(Me.btnAngleRepr)
            Me.pnlSettings.Controls.Add(Me.btnT)
            Me.pnlSettings.Controls.Add(Me.btnM)
            Me.pnlSettings.Controls.Add(Me.btnOutputFormat)
            Me.pnlSettings.Controls.Add(Me.btnY)
            Me.pnlSettings.Controls.Add(Me.btnX)
            Me.pnlSettings.Location = New System.Drawing.Point(0, 1)
            Me.pnlSettings.Name = "pnlSettings"
            Me.pnlSettings.Size = New System.Drawing.Size(678, 355)
            Me.pnlSettings.TabIndex = 6
            Me.pnlSettings.Visible = False
            '
            'lbSettings
            '
            Me.lbSettings.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
            Me.lbSettings.AutoSize = True
            Me.lbSettings.BackColor = System.Drawing.Color.Transparent
            Me.lbSettings.Cursor = System.Windows.Forms.Cursors.Arrow
            Me.lbSettings.Font = New System.Drawing.Font("Segoe UI Light", 16.0!)
            Me.lbSettings.ForeColor = System.Drawing.Color.White
            Me.lbSettings.Location = New System.Drawing.Point(326, 99)
            Me.lbSettings.Name = "lbSettings"
            Me.lbSettings.Size = New System.Drawing.Size(87, 30)
            Me.lbSettings.TabIndex = 13
            Me.lbSettings.Tag = "-"
            Me.lbSettings.Text = "Settings"
            '
            'PictureBox1
            '
            Me.PictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
            Me.PictureBox1.Image = CType(resources.GetObject("PictureBox1.Image"), System.Drawing.Image)
            Me.PictureBox1.Location = New System.Drawing.Point(300, 98)
            Me.PictureBox1.Name = "PictureBox1"
            Me.PictureBox1.Size = New System.Drawing.Size(32, 34)
            Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
            Me.PictureBox1.TabIndex = 14
            Me.PictureBox1.TabStop = False
            '
            'btnLog
            '
            Me.btnLog.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnLog.BackColor = System.Drawing.Color.Transparent
            Me.btnLog.Cursor = System.Windows.Forms.Cursors.Hand
            Me.btnLog.FlatAppearance.BorderSize = 0
            Me.btnLog.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent
            Me.btnLog.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent
            Me.btnLog.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnLog.Font = New System.Drawing.Font("Segoe UI Semilight", 9.0!)
            Me.btnLog.ForeColor = System.Drawing.Color.DarkSalmon
            Me.btnLog.Location = New System.Drawing.Point(408, 316)
            Me.btnLog.Name = "btnLog"
            Me.btnLog.Size = New System.Drawing.Size(114, 29)
            Me.btnLog.TabIndex = 12
            Me.btnLog.Tag = "-"
            Me.btnLog.Text = "What's New | Docs"
            Me.btnLog.UseVisualStyleBackColor = False
            '
            'btnExplicit
            '
            Me.btnExplicit.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnExplicit.BackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.btnExplicit.Cursor = System.Windows.Forms.Cursors.Hand
            Me.btnExplicit.FlatAppearance.BorderSize = 0
            Me.btnExplicit.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(65, Byte), Integer), CType(CType(65, Byte), Integer), CType(CType(65, Byte), Integer))
            Me.btnExplicit.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.btnExplicit.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnExplicit.Font = New System.Drawing.Font("Segoe UI Semilight", 12.75!)
            Me.btnExplicit.ForeColor = System.Drawing.Color.White
            Me.btnExplicit.Location = New System.Drawing.Point(223, 139)
            Me.btnExplicit.Name = "btnExplicit"
            Me.btnExplicit.Size = New System.Drawing.Size(86, 40)
            Me.btnExplicit.TabIndex = 0
            Me.btnExplicit.Tag = "-"
            Me.btnExplicit.Text = "Explicit"
            Me.TTLetters.SetToolTip(Me.btnExplicit, resources.GetString("btnExplicit.ToolTip"))
            Me.btnExplicit.UseVisualStyleBackColor = False
            '
            'cbAutoUpd
            '
            Me.cbAutoUpd.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.cbAutoUpd.AutoSize = True
            Me.cbAutoUpd.BackColor = System.Drawing.Color.Transparent
            Me.cbAutoUpd.Cursor = System.Windows.Forms.Cursors.Hand
            Me.cbAutoUpd.Font = New System.Drawing.Font("Segoe UI Semilight", 9.0!)
            Me.cbAutoUpd.ForeColor = System.Drawing.Color.White
            Me.cbAutoUpd.Location = New System.Drawing.Point(534, 323)
            Me.cbAutoUpd.Name = "cbAutoUpd"
            Me.cbAutoUpd.Size = New System.Drawing.Size(119, 19)
            Me.cbAutoUpd.TabIndex = 11
            Me.cbAutoUpd.Tag = "-"
            Me.cbAutoUpd.Text = "Update on launch"
            Me.TTLetters.SetToolTip(Me.cbAutoUpd, "If this box is checked, Cantus will check for updates automatically" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "each time it" &
        " is launched")
            Me.cbAutoUpd.UseVisualStyleBackColor = False
            '
            'btnUpdate
            '
            Me.btnUpdate.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnUpdate.BackColor = System.Drawing.Color.DarkSlateBlue
            Me.btnUpdate.Cursor = System.Windows.Forms.Cursors.Hand
            Me.btnUpdate.FlatAppearance.BorderSize = 0
            Me.btnUpdate.FlatAppearance.MouseDownBackColor = System.Drawing.Color.SlateBlue
            Me.btnUpdate.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkSlateBlue
            Me.btnUpdate.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnUpdate.Font = New System.Drawing.Font("Segoe UI Semilight", 10.0!)
            Me.btnUpdate.ForeColor = System.Drawing.Color.White
            Me.btnUpdate.Location = New System.Drawing.Point(406, 185)
            Me.btnUpdate.Name = "btnUpdate"
            Me.btnUpdate.Size = New System.Drawing.Size(86, 50)
            Me.btnUpdate.TabIndex = 9
            Me.btnUpdate.Tag = "-"
            Me.btnUpdate.Text = "Check For &Updates"
            Me.TTLetters.SetToolTip(Me.btnUpdate, "Check for new versions of Cantus on the internet." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "If one is found, the program w" &
        "ill close and update automatically." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "The check is done in the background and you" &
        " may work while we perform it.")
            Me.btnUpdate.UseVisualStyleBackColor = False
            '
            'lbAbout
            '
            Me.lbAbout.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
            Me.lbAbout.AutoSize = True
            Me.lbAbout.BackColor = System.Drawing.Color.Transparent
            Me.lbAbout.Cursor = System.Windows.Forms.Cursors.Hand
            Me.lbAbout.Font = New System.Drawing.Font("Segoe UI Semilight", 9.0!)
            Me.lbAbout.ForeColor = System.Drawing.Color.White
            Me.lbAbout.Location = New System.Drawing.Point(28, 322)
            Me.lbAbout.Name = "lbAbout"
            Me.lbAbout.Size = New System.Drawing.Size(179, 15)
            Me.lbAbout.TabIndex = 10
            Me.lbAbout.Tag = "-"
            Me.lbAbout.Text = "Cantus v{VER} By Alex Yu 2015-16"
            '
            'btnAngleRepr
            '
            Me.btnAngleRepr.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnAngleRepr.BackColor = System.Drawing.Color.FromArgb(CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer))
            Me.btnAngleRepr.Cursor = System.Windows.Forms.Cursors.Hand
            Me.btnAngleRepr.FlatAppearance.BorderSize = 0
            Me.btnAngleRepr.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer))
            Me.btnAngleRepr.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer))
            Me.btnAngleRepr.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnAngleRepr.Font = New System.Drawing.Font("Segoe UI Semilight", 12.75!)
            Me.btnAngleRepr.ForeColor = System.Drawing.Color.White
            Me.btnAngleRepr.Location = New System.Drawing.Point(223, 184)
            Me.btnAngleRepr.Name = "btnAngleRepr"
            Me.btnAngleRepr.Size = New System.Drawing.Size(86, 50)
            Me.btnAngleRepr.TabIndex = 7
            Me.btnAngleRepr.Tag = "-"
            Me.btnAngleRepr.Text = "Radian"
            Me.TTLetters.SetToolTip(Me.btnAngleRepr, "Change the angle representation:" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Degree / Radian / Gradian" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "(Hotkey: Ctrl+Alt+P " &
        "or Ctrl+Alt+<first letter of mode name>)")
            Me.btnAngleRepr.UseVisualStyleBackColor = False
            '
            'btnT
            '
            Me.btnT.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnT.BackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.btnT.Cursor = System.Windows.Forms.Cursors.Hand
            Me.btnT.FlatAppearance.BorderSize = 0
            Me.btnT.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(65, Byte), Integer), CType(CType(65, Byte), Integer), CType(CType(65, Byte), Integer))
            Me.btnT.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.btnT.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnT.Font = New System.Drawing.Font("Segoe UI Semilight", 12.0!)
            Me.btnT.ForeColor = System.Drawing.Color.White
            Me.btnT.Location = New System.Drawing.Point(406, 139)
            Me.btnT.Name = "btnT"
            Me.btnT.Size = New System.Drawing.Size(40, 40)
            Me.btnT.TabIndex = 4
            Me.btnT.Tag = "t"
            Me.btnT.Text = "&t"
            Me.btnT.UseVisualStyleBackColor = False
            '
            'btnM
            '
            Me.btnM.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnM.BackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.btnM.Cursor = System.Windows.Forms.Cursors.Hand
            Me.btnM.FlatAppearance.BorderSize = 0
            Me.btnM.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(65, Byte), Integer), CType(CType(65, Byte), Integer), CType(CType(65, Byte), Integer))
            Me.btnM.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.btnM.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnM.Font = New System.Drawing.Font("Segoe UI Semilight", 12.0!)
            Me.btnM.ForeColor = System.Drawing.Color.White
            Me.btnM.Location = New System.Drawing.Point(452, 139)
            Me.btnM.Name = "btnM"
            Me.btnM.Size = New System.Drawing.Size(40, 40)
            Me.btnM.TabIndex = 5
            Me.btnM.Tag = "m"
            Me.btnM.Text = "&m"
            Me.btnM.UseVisualStyleBackColor = False
            '
            'btnOutputFormat
            '
            Me.btnOutputFormat.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnOutputFormat.BackColor = System.Drawing.Color.FromArgb(CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer))
            Me.btnOutputFormat.Cursor = System.Windows.Forms.Cursors.Hand
            Me.btnOutputFormat.FlatAppearance.BorderSize = 0
            Me.btnOutputFormat.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer))
            Me.btnOutputFormat.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer))
            Me.btnOutputFormat.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnOutputFormat.Font = New System.Drawing.Font("Segoe UI Semilight", 12.75!)
            Me.btnOutputFormat.ForeColor = System.Drawing.Color.White
            Me.btnOutputFormat.Location = New System.Drawing.Point(315, 185)
            Me.btnOutputFormat.Name = "btnOutputFormat"
            Me.btnOutputFormat.Size = New System.Drawing.Size(86, 50)
            Me.btnOutputFormat.TabIndex = 8
            Me.btnOutputFormat.Tag = "-"
            Me.btnOutputFormat.Text = "Math"
            Me.TTLetters.SetToolTip(Me.btnOutputFormat, "Change the output format:" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Math: Output fractions, roots, etc." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Scientific: Outpu" &
        "t scientific notation " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Raw: Output full decimal number" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "(Hotkey: Ctrl+Alt+O or " &
        "Ctrl+Alt+<first letter of mode name>)")
            Me.btnOutputFormat.UseVisualStyleBackColor = False
            '
            'btnY
            '
            Me.btnY.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnY.BackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.btnY.Cursor = System.Windows.Forms.Cursors.Hand
            Me.btnY.FlatAppearance.BorderSize = 0
            Me.btnY.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(65, Byte), Integer), CType(CType(65, Byte), Integer), CType(CType(65, Byte), Integer))
            Me.btnY.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.btnY.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnY.Font = New System.Drawing.Font("Segoe UI Semilight", 12.0!)
            Me.btnY.ForeColor = System.Drawing.Color.White
            Me.btnY.Location = New System.Drawing.Point(361, 139)
            Me.btnY.Name = "btnY"
            Me.btnY.Size = New System.Drawing.Size(40, 40)
            Me.btnY.TabIndex = 3
            Me.btnY.Tag = "y"
            Me.btnY.Text = "&y"
            Me.btnY.UseVisualStyleBackColor = False
            '
            'btnX
            '
            Me.btnX.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnX.BackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.btnX.Cursor = System.Windows.Forms.Cursors.Hand
            Me.btnX.FlatAppearance.BorderSize = 0
            Me.btnX.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(65, Byte), Integer), CType(CType(65, Byte), Integer), CType(CType(65, Byte), Integer))
            Me.btnX.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.btnX.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnX.Font = New System.Drawing.Font("Segoe UI Semilight", 12.0!)
            Me.btnX.ForeColor = System.Drawing.Color.White
            Me.btnX.Location = New System.Drawing.Point(315, 139)
            Me.btnX.Name = "btnX"
            Me.btnX.Size = New System.Drawing.Size(40, 40)
            Me.btnX.TabIndex = 2
            Me.btnX.Tag = "x"
            Me.btnX.Text = "&x"
            Me.btnX.UseVisualStyleBackColor = False
            '
            'btnEval
            '
            Me.btnEval.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnEval.BackColor = System.Drawing.Color.FromArgb(CType(CType(160, Byte), Integer), CType(CType(70, Byte), Integer), CType(CType(10, Byte), Integer))
            Me.btnEval.Cursor = System.Windows.Forms.Cursors.Hand
            Me.btnEval.FlatAppearance.BorderSize = 0
            Me.btnEval.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(210, Byte), Integer), CType(CType(80, Byte), Integer), CType(CType(20, Byte), Integer))
            Me.btnEval.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(160, Byte), Integer), CType(CType(70, Byte), Integer), CType(CType(10, Byte), Integer))
            Me.btnEval.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnEval.Font = New System.Drawing.Font("Segoe UI Semilight", 13.0!)
            Me.btnEval.ForeColor = System.Drawing.Color.White
            Me.btnEval.Image = CType(resources.GetObject("btnEval.Image"), System.Drawing.Image)
            Me.btnEval.Location = New System.Drawing.Point(673, 348)
            Me.btnEval.Name = "btnEval"
            Me.btnEval.Size = New System.Drawing.Size(56, 52)
            Me.btnEval.TabIndex = 1
            Me.TTLetters.SetToolTip(Me.btnEval, "Run & Record (Alt+Enter)")
            Me.btnEval.UseVisualStyleBackColor = False
            '
            'lbResult
            '
            Me.lbResult.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.lbResult.BackColor = System.Drawing.Color.Transparent
            Me.lbResult.Font = New System.Drawing.Font("Segoe UI Semilight", 14.0!)
            Me.lbResult.ForeColor = System.Drawing.Color.White
            Me.lbResult.Location = New System.Drawing.Point(11, 2)
            Me.lbResult.Name = "lbResult"
            Me.lbResult.Size = New System.Drawing.Size(619, 46)
            Me.lbResult.TabIndex = 1
            Me.lbResult.Text = "= "
            Me.lbResult.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            Me.TTLetters.SetToolTip(Me.lbResult, "=")
            Me.lbResult.UseMnemonic = False
            '
            'pnlResults
            '
            Me.pnlResults.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.pnlResults.BackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.pnlResults.Controls.Add(Me.btnClose)
            Me.pnlResults.Controls.Add(Me.btnMin)
            Me.pnlResults.Controls.Add(Me.lbResult)
            Me.pnlResults.Location = New System.Drawing.Point(-1, -1)
            Me.pnlResults.Name = "pnlResults"
            Me.pnlResults.Size = New System.Drawing.Size(730, 49)
            Me.pnlResults.TabIndex = 4
            '
            'btnClose
            '
            Me.btnClose.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnClose.BackColor = System.Drawing.Color.Brown
            Me.btnClose.Cursor = System.Windows.Forms.Cursors.Hand
            Me.btnClose.FlatAppearance.BorderSize = 0
            Me.btnClose.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(170, Byte), Integer), CType(CType(60, Byte), Integer), CType(CType(60, Byte), Integer))
            Me.btnClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Brown
            Me.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnClose.ForeColor = System.Drawing.Color.White
            Me.btnClose.Image = CType(resources.GetObject("btnClose.Image"), System.Drawing.Image)
            Me.btnClose.Location = New System.Drawing.Point(677, 2)
            Me.btnClose.Name = "btnClose"
            Me.btnClose.Size = New System.Drawing.Size(53, 49)
            Me.btnClose.TabIndex = 5
            Me.btnClose.UseVisualStyleBackColor = False
            '
            'btnMin
            '
            Me.btnMin.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnMin.BackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.btnMin.Cursor = System.Windows.Forms.Cursors.Hand
            Me.btnMin.FlatAppearance.BorderSize = 0
            Me.btnMin.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(80, Byte), Integer), CType(CType(80, Byte), Integer), CType(CType(80, Byte), Integer))
            Me.btnMin.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.btnMin.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnMin.ForeColor = System.Drawing.Color.White
            Me.btnMin.Location = New System.Drawing.Point(623, 1)
            Me.btnMin.Name = "btnMin"
            Me.btnMin.Size = New System.Drawing.Size(53, 48)
            Me.btnMin.TabIndex = 0
            Me.btnMin.Text = "-"
            Me.TTLetters.SetToolTip(Me.btnMin, "Minimize (Win+Down)")
            Me.btnMin.UseVisualStyleBackColor = False
            '
            'btnSettings
            '
            Me.btnSettings.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnSettings.BackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.btnSettings.Cursor = System.Windows.Forms.Cursors.Hand
            Me.btnSettings.FlatAppearance.BorderSize = 0
            Me.btnSettings.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(80, Byte), Integer), CType(CType(80, Byte), Integer), CType(CType(80, Byte), Integer))
            Me.btnSettings.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.btnSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnSettings.Font = New System.Drawing.Font("Segoe UI Semilight", 1.0!)
            Me.btnSettings.ForeColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.btnSettings.Image = CType(resources.GetObject("btnSettings.Image"), System.Drawing.Image)
            Me.btnSettings.Location = New System.Drawing.Point(674, 101)
            Me.btnSettings.Name = "btnSettings"
            Me.btnSettings.Size = New System.Drawing.Size(53, 47)
            Me.btnSettings.TabIndex = 3
            Me.btnSettings.TextAlign = System.Drawing.ContentAlignment.TopRight
            Me.TTLetters.SetToolTip(Me.btnSettings, "Open the settings panel (Alt+S)")
            Me.btnSettings.UseVisualStyleBackColor = False
            '
            'TTLetters
            '
            Me.TTLetters.ShowAlways = True
            '
            'btnFunctions
            '
            Me.btnFunctions.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnFunctions.BackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.btnFunctions.Cursor = System.Windows.Forms.Cursors.Hand
            Me.btnFunctions.FlatAppearance.BorderSize = 0
            Me.btnFunctions.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(80, Byte), Integer), CType(CType(80, Byte), Integer), CType(CType(80, Byte), Integer))
            Me.btnFunctions.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.btnFunctions.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnFunctions.Font = New System.Drawing.Font("Segoe UI Semilight", 13.0!)
            Me.btnFunctions.ForeColor = System.Drawing.Color.White
            Me.btnFunctions.Location = New System.Drawing.Point(675, 49)
            Me.btnFunctions.Margin = New System.Windows.Forms.Padding(2)
            Me.btnFunctions.Name = "btnFunctions"
            Me.btnFunctions.Size = New System.Drawing.Size(53, 47)
            Me.btnFunctions.TabIndex = 27
            Me.btnFunctions.TabStop = False
            Me.btnFunctions.Text = "f(x)"
            Me.TTLetters.SetToolTip(Me.btnFunctions, "Insert function from library (Alt+F)")
            Me.btnFunctions.UseVisualStyleBackColor = False
            '
            'btnSave
            '
            Me.btnSave.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnSave.BackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.btnSave.Cursor = System.Windows.Forms.Cursors.Hand
            Me.btnSave.FlatAppearance.BorderSize = 0
            Me.btnSave.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(80, Byte), Integer), CType(CType(80, Byte), Integer), CType(CType(80, Byte), Integer))
            Me.btnSave.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnSave.Font = New System.Drawing.Font("Segoe UI Semilight", 13.0!)
            Me.btnSave.ForeColor = System.Drawing.Color.White
            Me.btnSave.Image = CType(resources.GetObject("btnSave.Image"), System.Drawing.Image)
            Me.btnSave.Location = New System.Drawing.Point(675, 299)
            Me.btnSave.Margin = New System.Windows.Forms.Padding(2)
            Me.btnSave.Name = "btnSave"
            Me.btnSave.Size = New System.Drawing.Size(53, 47)
            Me.btnSave.TabIndex = 28
            Me.btnSave.TabStop = False
            Me.TTLetters.SetToolTip(Me.btnSave, "Save (Ctrl+S or F12 (Save As))" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Note that when a file is open, the editor saves" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) &
        "automatically when you make any changes." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "This button also saves all settings.")
            Me.btnSave.UseVisualStyleBackColor = False
            '
            'btnOpen
            '
            Me.btnOpen.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnOpen.BackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.btnOpen.Cursor = System.Windows.Forms.Cursors.Hand
            Me.btnOpen.FlatAppearance.BorderSize = 0
            Me.btnOpen.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(80, Byte), Integer), CType(CType(80, Byte), Integer), CType(CType(80, Byte), Integer))
            Me.btnOpen.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.btnOpen.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnOpen.Font = New System.Drawing.Font("Segoe UI Semilight", 13.0!)
            Me.btnOpen.ForeColor = System.Drawing.Color.White
            Me.btnOpen.Image = CType(resources.GetObject("btnOpen.Image"), System.Drawing.Image)
            Me.btnOpen.Location = New System.Drawing.Point(675, 248)
            Me.btnOpen.Margin = New System.Windows.Forms.Padding(2)
            Me.btnOpen.Name = "btnOpen"
            Me.btnOpen.Size = New System.Drawing.Size(53, 47)
            Me.btnOpen.TabIndex = 29
            Me.btnOpen.TabStop = False
            Me.TTLetters.SetToolTip(Me.btnOpen, "Open (Ctrl+O or F11)")
            Me.btnOpen.UseVisualStyleBackColor = False
            '
            'pnlTb
            '
            Me.pnlTb.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.pnlTb.BackColor = System.Drawing.Color.FromArgb(CType(CType(34, Byte), Integer), CType(CType(34, Byte), Integer), CType(CType(34, Byte), Integer))
            Me.pnlTb.Controls.Add(Me.pnlSettings)
            Me.pnlTb.Controls.Add(Me.tb)
            Me.pnlTb.Location = New System.Drawing.Point(-3, 46)
            Me.pnlTb.Name = "pnlTb"
            Me.pnlTb.Size = New System.Drawing.Size(678, 361)
            Me.pnlTb.TabIndex = 0
            '
            'TmrReCalc
            '
            Me.TmrReCalc.Interval = 150
            '
            'TmrLoad
            '
            Me.TmrLoad.Interval = 50
            '
            'FrmEditor
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
            Me.AutoSize = True
            Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
            Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
            Me.ClientSize = New System.Drawing.Size(728, 400)
            Me.ControlBox = False
            Me.Controls.Add(Me.btnOpen)
            Me.Controls.Add(Me.btnSave)
            Me.Controls.Add(Me.pnlResults)
            Me.Controls.Add(Me.pnlTb)
            Me.Controls.Add(Me.btnSettings)
            Me.Controls.Add(Me.btnEval)
            Me.Controls.Add(Me.btnFunctions)
            Me.Font = New System.Drawing.Font("Segoe UI Semilight", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
            Me.KeyPreview = True
            Me.MaximizeBox = False
            Me.MaximumSize = New System.Drawing.Size(728, 400)
            Me.MinimumSize = New System.Drawing.Size(728, 400)
            Me.Name = "FrmEditor"
            Me.StartPosition = System.Windows.Forms.FormStartPosition.Manual
            Me.Text = "Editor - Cantus"
            Me.TopMost = True
            Me.pnlSettings.ResumeLayout(False)
            Me.pnlSettings.PerformLayout()
            CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
            Me.pnlResults.ResumeLayout(False)
            Me.pnlTb.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents tb As ScintillaNET.Scintilla
        Friend WithEvents btnEval As System.Windows.Forms.Button
        Friend WithEvents lbResult As System.Windows.Forms.Label
        Friend WithEvents pnlResults As System.Windows.Forms.Panel
        Friend WithEvents btnSettings As System.Windows.Forms.Button
        Friend WithEvents pnlSettings As System.Windows.Forms.Panel
        Friend WithEvents btnX As System.Windows.Forms.Button
        Friend WithEvents btnY As System.Windows.Forms.Button
        Friend WithEvents btnOutputFormat As System.Windows.Forms.Button
        Friend WithEvents btnM As System.Windows.Forms.Button
        Friend WithEvents btnT As System.Windows.Forms.Button
        Friend WithEvents TTLetters As System.Windows.Forms.ToolTip
        Friend WithEvents btnClose As Button
        Friend WithEvents pnlTb As Panel
        Friend WithEvents btnMin As Button
        Friend WithEvents btnAngleRepr As Button
        Friend WithEvents lbAbout As Label
        Friend WithEvents cbAutoUpd As CheckBox
        Friend WithEvents btnUpdate As Button
        Friend WithEvents TmrReCalc As Timer
        Friend WithEvents btnExplicit As Button
        Friend WithEvents btnFunctions As Button
        Friend WithEvents btnLog As Button
        Friend WithEvents lbSettings As Label
        Friend WithEvents btnSave As Button
        Friend WithEvents btnOpen As Button
        Friend WithEvents PictureBox1 As PictureBox
        Friend WithEvents TmrLoad As Timer
    End Class
End Namespace
