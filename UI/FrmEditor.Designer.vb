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
            Me.Tb = New ScintillaNET.Scintilla()
            Me.PnlSettings = New System.Windows.Forms.Panel()
            Me.BtnSigFigs = New System.Windows.Forms.Button()
            Me.LbSettings = New System.Windows.Forms.Label()
            Me.PbSettingsLogo = New System.Windows.Forms.PictureBox()
            Me.BtnLog = New System.Windows.Forms.Button()
            Me.BtnExplicit = New System.Windows.Forms.Button()
            Me.CbAutoUpd = New System.Windows.Forms.CheckBox()
            Me.BtnUpdate = New System.Windows.Forms.Button()
            Me.LbAbout = New System.Windows.Forms.Label()
            Me.BtnAngleRepr = New System.Windows.Forms.Button()
            Me.BtnOutputFormat = New System.Windows.Forms.Button()
            Me.BtnY = New System.Windows.Forms.Button()
            Me.BtnX = New System.Windows.Forms.Button()
            Me.BtnEval = New System.Windows.Forms.Button()
            Me.LbResult = New System.Windows.Forms.Label()
            Me.PnlResults = New System.Windows.Forms.Panel()
            Me.BtnMin = New System.Windows.Forms.Button()
            Me.BtnMax = New System.Windows.Forms.Button()
            Me.BtnTranslucent = New System.Windows.Forms.Button()
            Me.BtnClose = New System.Windows.Forms.Button()
            Me.BtnSettings = New System.Windows.Forms.Button()
            Me.TTLetters = New System.Windows.Forms.ToolTip(Me.components)
            Me.BtnFunctions = New System.Windows.Forms.Button()
            Me.BtnSave = New System.Windows.Forms.Button()
            Me.BtnOpen = New System.Windows.Forms.Button()
            Me.BtnNew = New System.Windows.Forms.Button()
            Me.PnlTb = New System.Windows.Forms.Panel()
            Me.TmrLoad = New System.Windows.Forms.Timer(Me.components)
            Me.TmrAutoSave = New System.Windows.Forms.Timer(Me.components)
            Me.Editor = New System.Windows.Forms.Panel()
            Me.Viewer = New Cantus.UI.Viewer()
            Me.TmrAnim = New System.Windows.Forms.Timer(Me.components)
            Me.BtnKeyboard = New System.Windows.Forms.Button()
            Me.Split = New System.Windows.Forms.SplitContainer()
            Me.PnlSettings.SuspendLayout()
            CType(Me.PbSettingsLogo, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.PnlResults.SuspendLayout()
            Me.PnlTb.SuspendLayout()
            Me.Editor.SuspendLayout()
            Me.SuspendLayout()
            '
            'Split
            '
            Me.Split.Name = "Split"
            Me.Split.Location = New Point(4, 4)
            Me.Split.Size = New Size(1341, 700)
            Me.Split.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                                Or System.Windows.Forms.AnchorStyles.Left) _
                                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.Split.BackColor = System.Drawing.Color.FromArgb(45, 45, 45)
            Me.Split.Panel1.Controls.Add(Me.Viewer)
            Me.Split.Panel2.Controls.Add(Me.Editor)
            Me.Split.Panel2MinSize = 53
            Me.Split.Panel1MinSize = 0
            Me.Split.Panel1Collapsed = False
            Me.Split.Panel2Collapsed = False
            Me.Split.SplitterDistance = 614
            Me.Split.SplitterWidth = 2
            Me.Split.Cursor = Cursors.Arrow
            '
            'Tb
            '
            Me.Tb.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.Tb.BackColor = System.Drawing.Color.FromArgb(CType(CType(34, Byte), Integer), CType(CType(34, Byte), Integer), CType(CType(34, Byte), Integer))
            Me.Tb.BorderStyle = System.Windows.Forms.BorderStyle.None
            Me.Tb.CaretForeColor = System.Drawing.Color.GhostWhite
            Me.Tb.Cursor = System.Windows.Forms.Cursors.Arrow
            Me.Tb.EdgeColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.Tb.Font = New System.Drawing.Font("Lucida Console", 13.5!)
            Me.Tb.ForeColor = System.Drawing.Color.White
            Me.Tb.Location = New System.Drawing.Point(3, 18)
            Me.Tb.Name = "Tb"
            Me.Tb.Size = New System.Drawing.Size(675, 634)
            Me.Tb.TabIndex = 0
            '
            'PnlSettings
            '
            Me.PnlSettings.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.PnlSettings.BackColor = System.Drawing.Color.Black
            Me.PnlSettings.BackgroundImage = CType(resources.GetObject("PnlSettings.BackgroundImage"), System.Drawing.Image)
            Me.PnlSettings.Controls.Add(Me.BtnSigFigs)
            Me.PnlSettings.Controls.Add(Me.LbSettings)
            Me.PnlSettings.Controls.Add(Me.PbSettingsLogo)
            Me.PnlSettings.Controls.Add(Me.BtnLog)
            Me.PnlSettings.Controls.Add(Me.BtnExplicit)
            Me.PnlSettings.Controls.Add(Me.CbAutoUpd)
            Me.PnlSettings.Controls.Add(Me.BtnUpdate)
            Me.PnlSettings.Controls.Add(Me.LbAbout)
            Me.PnlSettings.Controls.Add(Me.BtnAngleRepr)
            Me.PnlSettings.Controls.Add(Me.BtnOutputFormat)
            Me.PnlSettings.Controls.Add(Me.BtnY)
            Me.PnlSettings.Controls.Add(Me.BtnX)
            Me.PnlSettings.Location = New System.Drawing.Point(680, 1)
            Me.PnlSettings.Name = "PnlSettings"
            Me.PnlSettings.Size = New System.Drawing.Size(678, 655)
            Me.PnlSettings.TabIndex = 6
            Me.PnlSettings.Visible = False
            '
            'BtnSigFigs
            '
            Me.BtnSigFigs.Anchor = System.Windows.Forms.AnchorStyles.None
            Me.BtnSigFigs.BackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.BtnSigFigs.Cursor = System.Windows.Forms.Cursors.Hand
            Me.BtnSigFigs.FlatAppearance.BorderSize = 0
            Me.BtnSigFigs.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(65, Byte), Integer), CType(CType(65, Byte), Integer), CType(CType(65, Byte), Integer))
            Me.BtnSigFigs.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.BtnSigFigs.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.BtnSigFigs.Font = New System.Drawing.Font(OpenSansLight, 12.75!)
            Me.BtnSigFigs.ForeColor = System.Drawing.Color.White
            Me.BtnSigFigs.Location = New System.Drawing.Point(315, 289)
            Me.BtnSigFigs.Name = "BtnSigFigs"
            Me.BtnSigFigs.Size = New System.Drawing.Size(86, 40)
            Me.BtnSigFigs.TabIndex = 15
            Me.BtnSigFigs.Tag = "-S"
            Me.BtnSigFigs.Text = "Sig Figs"
            Me.TTLetters.SetToolTip(Me.BtnSigFigs, resources.GetString("BtnSigFigs.ToolTip"))
            Me.BtnSigFigs.UseVisualStyleBackColor = False
            '
            'LbSettings
            '
            Me.LbSettings.Anchor = System.Windows.Forms.AnchorStyles.None
            Me.LbSettings.AutoSize = True
            Me.LbSettings.BackColor = System.Drawing.Color.Transparent
            Me.LbSettings.Cursor = System.Windows.Forms.Cursors.Arrow
            Me.LbSettings.Font = New System.Drawing.Font(OpenSansLight, 16.0!)
            Me.LbSettings.ForeColor = System.Drawing.Color.White
            Me.LbSettings.Location = New System.Drawing.Point(326, 249)
            Me.LbSettings.Name = "LbSettings"
            Me.LbSettings.Size = New System.Drawing.Size(91, 30)
            Me.LbSettings.TabIndex = 13
            Me.LbSettings.Tag = "-"
            Me.LbSettings.Text = "Settings"
            '
            'PbSettingsLogo
            '
            Me.PbSettingsLogo.Anchor = System.Windows.Forms.AnchorStyles.None
            Me.PbSettingsLogo.BackColor = System.Drawing.Color.Transparent
            Me.PbSettingsLogo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
            Me.PbSettingsLogo.Image = CType(resources.GetObject("PbSettingsLogo.Image"), System.Drawing.Image)
            Me.PbSettingsLogo.Location = New System.Drawing.Point(300, 248)
            Me.PbSettingsLogo.Name = "PbSettingsLogo"
            Me.PbSettingsLogo.Size = New System.Drawing.Size(32, 34)
            Me.PbSettingsLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
            Me.PbSettingsLogo.TabIndex = 14
            Me.PbSettingsLogo.TabStop = False
            '
            'BtnLog
            '
            Me.BtnLog.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.BtnLog.BackColor = System.Drawing.Color.Transparent
            Me.BtnLog.Cursor = System.Windows.Forms.Cursors.Hand
            Me.BtnLog.FlatAppearance.BorderSize = 0
            Me.BtnLog.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent
            Me.BtnLog.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent
            Me.BtnLog.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.BtnLog.Font = New System.Drawing.Font(OpenSansLight, 9.0!)
            Me.BtnLog.ForeColor = System.Drawing.Color.LightSteelBlue
            Me.BtnLog.Location = New System.Drawing.Point(393, 616)
            Me.BtnLog.Name = "BtnLog"
            Me.BtnLog.Size = New System.Drawing.Size(129, 29)
            Me.BtnLog.TabIndex = 12
            Me.BtnLog.Tag = "-"
            Me.BtnLog.Text = "What's New | Docs"
            Me.BtnLog.UseVisualStyleBackColor = False
            '
            'BtnExplicit
            '
            Me.BtnExplicit.Anchor = System.Windows.Forms.AnchorStyles.None
            Me.BtnExplicit.BackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.BtnExplicit.Cursor = System.Windows.Forms.Cursors.Hand
            Me.BtnExplicit.FlatAppearance.BorderSize = 0
            Me.BtnExplicit.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(65, Byte), Integer), CType(CType(65, Byte), Integer), CType(CType(65, Byte), Integer))
            Me.BtnExplicit.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.BtnExplicit.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.BtnExplicit.Font = New System.Drawing.Font(OpenSansLight, 12.75!)
            Me.BtnExplicit.ForeColor = System.Drawing.Color.White
            Me.BtnExplicit.Location = New System.Drawing.Point(223, 289)
            Me.BtnExplicit.Name = "BtnExplicit"
            Me.BtnExplicit.Size = New System.Drawing.Size(86, 40)
            Me.BtnExplicit.TabIndex = 0
            Me.BtnExplicit.Tag = "-E"
            Me.BtnExplicit.Text = "Explicit"
            Me.TTLetters.SetToolTip(Me.BtnExplicit, resources.GetString("BtnExplicit.ToolTip"))
            Me.BtnExplicit.UseVisualStyleBackColor = False
            '
            'CbAutoUpd
            '
            Me.CbAutoUpd.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.CbAutoUpd.AutoSize = True
            Me.CbAutoUpd.BackColor = System.Drawing.Color.Transparent
            Me.CbAutoUpd.Cursor = System.Windows.Forms.Cursors.Hand
            Me.CbAutoUpd.Font = New System.Drawing.Font(OpenSansLight, 9.0!)
            Me.CbAutoUpd.ForeColor = System.Drawing.Color.White
            Me.CbAutoUpd.Location = New System.Drawing.Point(530, 621)
            Me.CbAutoUpd.Name = "CbAutoUpd"
            Me.CbAutoUpd.Size = New System.Drawing.Size(123, 21)
            Me.CbAutoUpd.TabIndex = 11
            Me.CbAutoUpd.Tag = "-"
            Me.CbAutoUpd.Text = "Update on launch"
            Me.TTLetters.SetToolTip(Me.CbAutoUpd, "If this box is checked, Cantus will check for updates automatically" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "each time it" &
        " is launched")
            Me.CbAutoUpd.UseVisualStyleBackColor = False
            '
            'BtnUpdate
            '
            Me.BtnUpdate.Anchor = System.Windows.Forms.AnchorStyles.None
            Me.BtnUpdate.BackColor = System.Drawing.Color.DarkSlateBlue
            Me.BtnUpdate.Cursor = System.Windows.Forms.Cursors.Hand
            Me.BtnUpdate.FlatAppearance.BorderSize = 0
            Me.BtnUpdate.FlatAppearance.MouseDownBackColor = System.Drawing.Color.SlateBlue
            Me.BtnUpdate.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkSlateBlue
            Me.BtnUpdate.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.BtnUpdate.Font = New System.Drawing.Font(OpenSansLight, 10.0!)
            Me.BtnUpdate.ForeColor = System.Drawing.Color.White
            Me.BtnUpdate.Location = New System.Drawing.Point(406, 335)
            Me.BtnUpdate.Name = "BtnUpdate"
            Me.BtnUpdate.Size = New System.Drawing.Size(86, 50)
            Me.BtnUpdate.TabIndex = 9
            Me.BtnUpdate.Tag = "-"
            Me.BtnUpdate.Text = "Check For &Updates"
            Me.TTLetters.SetToolTip(Me.BtnUpdate, "Check for new versions of Cantus on the internet." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "If one is found, the program w" &
        "ill close and update automatically." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "The check is done in the background and you" &
        " may work while we perform it.")
            Me.BtnUpdate.UseVisualStyleBackColor = False
            '
            'LbAbout
            '
            Me.LbAbout.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
            Me.LbAbout.AutoSize = True
            Me.LbAbout.BackColor = System.Drawing.Color.Transparent
            Me.LbAbout.Cursor = System.Windows.Forms.Cursors.Hand
            Me.LbAbout.Font = New System.Drawing.Font(OpenSansLight, 9.0!)
            Me.LbAbout.ForeColor = System.Drawing.Color.White
            Me.LbAbout.Location = New System.Drawing.Point(28, 622)
            Me.LbAbout.Name = "LbAbout"
            Me.LbAbout.Size = New System.Drawing.Size(191, 17)
            Me.LbAbout.TabIndex = 10
            Me.LbAbout.Tag = "-"
            Me.LbAbout.Text = "Cantus v{VER} By Alex Yu 2015-16"
            '
            'BtnAngleRepr
            '
            Me.BtnAngleRepr.Anchor = System.Windows.Forms.AnchorStyles.None
            Me.BtnAngleRepr.BackColor = System.Drawing.Color.FromArgb(CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer))
            Me.BtnAngleRepr.Cursor = System.Windows.Forms.Cursors.Hand
            Me.BtnAngleRepr.FlatAppearance.BorderSize = 0
            Me.BtnAngleRepr.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer))
            Me.BtnAngleRepr.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer))
            Me.BtnAngleRepr.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.BtnAngleRepr.Font = New System.Drawing.Font(OpenSansLight, 12.75!)
            Me.BtnAngleRepr.ForeColor = System.Drawing.Color.White
            Me.BtnAngleRepr.Location = New System.Drawing.Point(223, 334)
            Me.BtnAngleRepr.Name = "BtnAngleRepr"
            Me.BtnAngleRepr.Size = New System.Drawing.Size(86, 50)
            Me.BtnAngleRepr.TabIndex = 7
            Me.BtnAngleRepr.Tag = "-"
            Me.BtnAngleRepr.Text = "Radian"
            Me.TTLetters.SetToolTip(Me.BtnAngleRepr, "Change the angle representation:" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Degree / Radian / Gradian" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "(Hotkey: Ctrl+Alt+P " &
        "or Ctrl+Alt+<first letter of mode name>)")
            Me.BtnAngleRepr.UseVisualStyleBackColor = False
            '
            'BtnOutputFormat
            '
            Me.BtnOutputFormat.Anchor = System.Windows.Forms.AnchorStyles.None
            Me.BtnOutputFormat.BackColor = System.Drawing.Color.FromArgb(CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer))
            Me.BtnOutputFormat.Cursor = System.Windows.Forms.Cursors.Hand
            Me.BtnOutputFormat.FlatAppearance.BorderSize = 0
            Me.BtnOutputFormat.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer))
            Me.BtnOutputFormat.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer))
            Me.BtnOutputFormat.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.BtnOutputFormat.Font = New System.Drawing.Font(OpenSansLight, 12.75!)
            Me.BtnOutputFormat.ForeColor = System.Drawing.Color.White
            Me.BtnOutputFormat.Location = New System.Drawing.Point(315, 335)
            Me.BtnOutputFormat.Name = "BtnOutputFormat"
            Me.BtnOutputFormat.Size = New System.Drawing.Size(86, 50)
            Me.BtnOutputFormat.TabIndex = 8
            Me.BtnOutputFormat.Tag = "-"
            Me.BtnOutputFormat.Text = "Math"
            Me.TTLetters.SetToolTip(Me.BtnOutputFormat, "Change the output format:" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Math: Output fractions, roots, etc. (Ctrl+Alt+M)" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Scie" &
        "ntific: Output scientific notation (Ctrl+Alt+S)" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Raw: Output full decimal number" &
        " (Ctrl+Alt+W)" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "(Hotkey: Ctrl+Alt+O)")
            Me.BtnOutputFormat.UseVisualStyleBackColor = False
            '
            'BtnY
            '
            Me.BtnY.Anchor = System.Windows.Forms.AnchorStyles.None
            Me.BtnY.BackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.BtnY.Cursor = System.Windows.Forms.Cursors.Hand
            Me.BtnY.FlatAppearance.BorderSize = 0
            Me.BtnY.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(65, Byte), Integer), CType(CType(65, Byte), Integer), CType(CType(65, Byte), Integer))
            Me.BtnY.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.BtnY.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.BtnY.Font = New System.Drawing.Font(OpenSansLight, 12.0!)
            Me.BtnY.ForeColor = System.Drawing.Color.White
            Me.BtnY.Location = New System.Drawing.Point(453, 289)
            Me.BtnY.Name = "BtnY"
            Me.BtnY.Size = New System.Drawing.Size(40, 40)
            Me.BtnY.TabIndex = 3
            Me.BtnY.Tag = "y"
            Me.BtnY.Text = "&y"
            Me.BtnY.UseVisualStyleBackColor = False
            '
            'BtnX
            '
            Me.BtnX.Anchor = System.Windows.Forms.AnchorStyles.None
            Me.BtnX.BackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.BtnX.Cursor = System.Windows.Forms.Cursors.Hand
            Me.BtnX.FlatAppearance.BorderSize = 0
            Me.BtnX.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(65, Byte), Integer), CType(CType(65, Byte), Integer), CType(CType(65, Byte), Integer))
            Me.BtnX.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.BtnX.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.BtnX.Font = New System.Drawing.Font(OpenSansLight, 12.0!)
            Me.BtnX.ForeColor = System.Drawing.Color.White
            Me.BtnX.Location = New System.Drawing.Point(407, 289)
            Me.BtnX.Name = "BtnX"
            Me.BtnX.Size = New System.Drawing.Size(40, 40)
            Me.BtnX.TabIndex = 2
            Me.BtnX.Tag = "x"
            Me.BtnX.Text = "&x"
            Me.BtnX.UseVisualStyleBackColor = False
            '
            'BtnEval
            '
            Me.BtnEval.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.BtnEval.BackColor = System.Drawing.Color.FromArgb(CType(CType(160, Byte), Integer), CType(CType(70, Byte), Integer), CType(CType(10, Byte), Integer))
            Me.BtnEval.Cursor = System.Windows.Forms.Cursors.Hand
            Me.BtnEval.FlatAppearance.BorderSize = 0
            Me.BtnEval.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(200, Byte), Integer), CType(CType(80, Byte), Integer), CType(CType(20, Byte), Integer))
            Me.BtnEval.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(160, Byte), Integer), CType(CType(70, Byte), Integer), CType(CType(10, Byte), Integer))
            Me.BtnEval.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.BtnEval.Font = New System.Drawing.Font(OpenSansLight, 13.0!)
            Me.BtnEval.ForeColor = System.Drawing.Color.White
            Me.BtnEval.Image = CType(resources.GetObject("BtnEval.Image"), System.Drawing.Image)
            Me.BtnEval.Location = New System.Drawing.Point(673, 648)
            Me.BtnEval.Name = "BtnEval"
            Me.BtnEval.Size = New System.Drawing.Size(56, 52)
            Me.BtnEval.TabIndex = 1
            Me.TTLetters.SetToolTip(Me.BtnEval, "Run (Alt+Enter or F5)")
            Me.BtnEval.UseVisualStyleBackColor = False
            '
            'LbResult
            '
            Me.LbResult.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.LbResult.BackColor = System.Drawing.Color.Transparent
            Me.LbResult.Font = New System.Drawing.Font(OpenSansLight, 14.0!)
            Me.LbResult.ForeColor = System.Drawing.Color.White
            Me.LbResult.Location = New System.Drawing.Point(11, 2)
            Me.LbResult.Name = "LbResult"
            Me.LbResult.Size = New System.Drawing.Size(619, 46)
            Me.LbResult.TabIndex = 1
            Me.LbResult.Text = "= "
            Me.LbResult.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            Me.TTLetters.SetToolTip(Me.LbResult, "=")
            Me.LbResult.UseMnemonic = False
            '
            'PnlResults
            '
            Me.PnlResults.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.PnlResults.BackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.PnlResults.Controls.Add(Me.BtnMin)
            Me.PnlResults.Controls.Add(Me.BtnMax)
            Me.PnlResults.Controls.Add(Me.BtnTranslucent)
            Me.PnlResults.Controls.Add(Me.BtnClose)
            Me.PnlResults.Controls.Add(Me.LbResult)
            Me.PnlResults.Location = New System.Drawing.Point(-1, -1)
            Me.PnlResults.Name = "PnlResults"
            Me.PnlResults.Size = New System.Drawing.Size(730, 49)
            Me.PnlResults.TabIndex = 4
            '
            'BtnMax
            '
            Me.BtnMax.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.BtnMax.BackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.BtnMax.Cursor = System.Windows.Forms.Cursors.Hand
            Me.BtnMax.FlatAppearance.BorderSize = 0
            Me.BtnMax.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(80, Byte), Integer), CType(CType(80, Byte), Integer), CType(CType(80, Byte), Integer))
            Me.BtnMax.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.BtnMax.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.BtnMax.ForeColor = System.Drawing.Color.White
            Me.BtnMax.Location = New System.Drawing.Point(623, 1)
            Me.BtnMax.Name = "BtnMax"
            Me.BtnMax.Size = New System.Drawing.Size(53, 48)
            Me.BtnMax.TabIndex = 0
            Me.BtnMax.Text = "□"
            Me.TTLetters.SetToolTip(Me.BtnMax, "Maximize (Win+Up)")
            Me.BtnMax.UseVisualStyleBackColor = False
            '
            'BtnMin
            '
            Me.BtnMin.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.BtnMin.BackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.BtnMin.Cursor = System.Windows.Forms.Cursors.Hand
            Me.BtnMin.FlatAppearance.BorderSize = 0
            Me.BtnMin.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(80, Byte), Integer), CType(CType(80, Byte), Integer), CType(CType(80, Byte), Integer))
            Me.BtnMin.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.BtnMin.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.BtnMin.ForeColor = System.Drawing.Color.White
            Me.BtnMin.Location = New System.Drawing.Point(570, 1)
            Me.BtnMin.Name = "BtnMin"
            Me.BtnMin.Size = New System.Drawing.Size(53, 48)
            Me.BtnMin.TabIndex = 0
            Me.BtnMin.Text = "-"
            Me.TTLetters.SetToolTip(Me.BtnMin, "Minimize (Win+Down)")
            Me.BtnMin.UseVisualStyleBackColor = False
            '
            'BtnTranslucent
            '
            Me.BtnTranslucent.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.BtnTranslucent.BackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.BtnTranslucent.Cursor = System.Windows.Forms.Cursors.Hand
            Me.BtnTranslucent.FlatAppearance.BorderSize = 0
            Me.BtnTranslucent.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(80, Byte), Integer), CType(CType(80, Byte), Integer), CType(CType(80, Byte), Integer))
            Me.BtnTranslucent.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.BtnTranslucent.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.BtnTranslucent.ForeColor = System.Drawing.Color.White
            Me.BtnTranslucent.Image = CType(resources.GetObject("BtnTranslucent.Image"), System.Drawing.Image)
            Me.BtnTranslucent.Location = New System.Drawing.Point(511, 1)
            Me.BtnTranslucent.Name = "BtnTranslucent"
            Me.BtnTranslucent.Size = New System.Drawing.Size(53, 48)
            Me.BtnTranslucent.TabIndex = 6
            Me.TTLetters.SetToolTip(Me.BtnTranslucent, "Cycle Translucency (Alt+T)")
            Me.BtnTranslucent.UseVisualStyleBackColor = False
            '
            'BtnClose
            '
            Me.BtnClose.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.BtnClose.BackColor = System.Drawing.Color.Brown
            Me.BtnClose.Cursor = System.Windows.Forms.Cursors.Hand
            Me.BtnClose.FlatAppearance.BorderSize = 0
            Me.BtnClose.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(170, Byte), Integer), CType(CType(60, Byte), Integer), CType(CType(60, Byte), Integer))
            Me.BtnClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Brown
            Me.BtnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.BtnClose.ForeColor = System.Drawing.Color.White
            Me.BtnClose.Image = CType(resources.GetObject("BtnClose.Image"), System.Drawing.Image)
            Me.BtnClose.Location = New System.Drawing.Point(675, 1)
            Me.BtnClose.Name = "BtnClose"
            Me.BtnClose.Size = New System.Drawing.Size(53, 49)
            Me.BtnClose.TabIndex = 5
            Me.BtnClose.UseVisualStyleBackColor = False
            '
            'BtnSettings
            '
            Me.BtnSettings.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.BtnSettings.BackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.BtnSettings.Cursor = System.Windows.Forms.Cursors.Hand
            Me.BtnSettings.FlatAppearance.BorderSize = 0
            Me.BtnSettings.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(80, Byte), Integer), CType(CType(80, Byte), Integer), CType(CType(80, Byte), Integer))
            Me.BtnSettings.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.BtnSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.BtnSettings.Font = New System.Drawing.Font(OpenSansLight, 1.0!)
            Me.BtnSettings.ForeColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.BtnSettings.Image = CType(resources.GetObject("BtnSettings.Image"), System.Drawing.Image)
            Me.BtnSettings.Location = New System.Drawing.Point(675, 150)
            Me.BtnSettings.Name = "BtnSettings"
            Me.BtnSettings.Size = New System.Drawing.Size(53, 47)
            Me.BtnSettings.TabIndex = 3
            Me.BtnSettings.TextAlign = System.Drawing.ContentAlignment.TopRight
            Me.TTLetters.SetToolTip(Me.BtnSettings, "Open the settings panel (Alt+S)")
            Me.BtnSettings.UseVisualStyleBackColor = False
            '
            'TTLetters
            '
            Me.TTLetters.ShowAlways = True
            '
            'BtnFunctions
            '
            Me.BtnFunctions.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.BtnFunctions.BackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.BtnFunctions.Cursor = System.Windows.Forms.Cursors.Hand
            Me.BtnFunctions.FlatAppearance.BorderSize = 0
            Me.BtnFunctions.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(80, Byte), Integer), CType(CType(80, Byte), Integer), CType(CType(80, Byte), Integer))
            Me.BtnFunctions.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.BtnFunctions.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.BtnFunctions.Font = New System.Drawing.Font(OpenSansLight, 13.0!)
            Me.BtnFunctions.ForeColor = System.Drawing.Color.White
            Me.BtnFunctions.Location = New System.Drawing.Point(675, 49)
            Me.BtnFunctions.Margin = New System.Windows.Forms.Padding(2)
            Me.BtnFunctions.Name = "BtnFunctions"
            Me.BtnFunctions.Size = New System.Drawing.Size(53, 47)
            Me.BtnFunctions.TabIndex = 27
            Me.BtnFunctions.TabStop = False
            Me.BtnFunctions.Text = "f(x)"
            Me.TTLetters.SetToolTip(Me.BtnFunctions, "Insert function from library (Alt+F)")
            Me.BtnFunctions.UseVisualStyleBackColor = False
            '
            'BtnSave
            '
            Me.BtnSave.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.BtnSave.BackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.BtnSave.Cursor = System.Windows.Forms.Cursors.Hand
            Me.BtnSave.FlatAppearance.BorderSize = 0
            Me.BtnSave.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(80, Byte), Integer), CType(CType(80, Byte), Integer), CType(CType(80, Byte), Integer))
            Me.BtnSave.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.BtnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.BtnSave.Font = New System.Drawing.Font(OpenSansLight, 13.0!)
            Me.BtnSave.ForeColor = System.Drawing.Color.White
            Me.BtnSave.Image = CType(resources.GetObject("BtnSave.Image"), System.Drawing.Image)
            Me.BtnSave.Location = New System.Drawing.Point(675, 599)
            Me.BtnSave.Margin = New System.Windows.Forms.Padding(2)
            Me.BtnSave.Name = "BtnSave"
            Me.BtnSave.Size = New System.Drawing.Size(53, 47)
            Me.BtnSave.TabIndex = 28
            Me.BtnSave.TabStop = False
            Me.TTLetters.SetToolTip(Me.BtnSave, "Save (Ctrl+S or F12 (Save As))" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Note that when a file is open, the editor saves" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) &
        "automatically when you make any changes." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "This button also saves all settings.")
            Me.BtnSave.UseVisualStyleBackColor = False
            '
            'BtnOpen
            '
            Me.BtnOpen.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.BtnOpen.BackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.BtnOpen.Cursor = System.Windows.Forms.Cursors.Hand
            Me.BtnOpen.FlatAppearance.BorderSize = 0
            Me.BtnOpen.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(80, Byte), Integer), CType(CType(80, Byte), Integer), CType(CType(80, Byte), Integer))
            Me.BtnOpen.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.BtnOpen.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.BtnOpen.Font = New System.Drawing.Font(OpenSansLight, 13.0!)
            Me.BtnOpen.ForeColor = System.Drawing.Color.White
            Me.BtnOpen.Image = CType(resources.GetObject("BtnOpen.Image"), System.Drawing.Image)
            Me.BtnOpen.Location = New System.Drawing.Point(675, 548)
            Me.BtnOpen.Margin = New System.Windows.Forms.Padding(2)
            Me.BtnOpen.Name = "BtnOpen"
            Me.BtnOpen.Size = New System.Drawing.Size(53, 47)
            Me.BtnOpen.TabIndex = 29
            Me.BtnOpen.TabStop = False
            Me.TTLetters.SetToolTip(Me.BtnOpen, "Open (Ctrl+O or F11)")
            Me.BtnOpen.UseVisualStyleBackColor = False
            '
            'BtnNew
            '
            Me.BtnNew.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.BtnNew.BackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.BtnNew.Cursor = System.Windows.Forms.Cursors.Hand
            Me.BtnNew.FlatAppearance.BorderSize = 0
            Me.BtnNew.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(80, Byte), Integer), CType(CType(80, Byte), Integer), CType(CType(80, Byte), Integer))
            Me.BtnNew.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.BtnNew.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.BtnNew.Font = New System.Drawing.Font(OpenSansLight, 19.0!)
            Me.BtnNew.ForeColor = System.Drawing.Color.White
            Me.BtnNew.Location = New System.Drawing.Point(675, 498)
            Me.BtnNew.Margin = New System.Windows.Forms.Padding(2)
            Me.BtnNew.Name = "BtnNew"
            Me.BtnNew.Size = New System.Drawing.Size(53, 47)
            Me.BtnNew.TabIndex = 30
            Me.BtnNew.TabStop = False
            Me.BtnNew.Text = "+"
            Me.TTLetters.SetToolTip(Me.BtnNew, "New (Ctrl+N)")
            Me.BtnNew.UseVisualStyleBackColor = False
            '
            'PnlTb
            '
            Me.PnlTb.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.PnlTb.BackColor = System.Drawing.Color.FromArgb(CType(CType(34, Byte), Integer), CType(CType(34, Byte), Integer), CType(CType(34, Byte), Integer))
            Me.PnlTb.Controls.Add(Me.PnlSettings)
            Me.PnlTb.Controls.Add(Me.Tb)
            Me.PnlTb.Location = New System.Drawing.Point(-3, 46)
            Me.PnlTb.Name = "PnlTb"
            Me.PnlTb.Size = New System.Drawing.Size(678, 661)
            Me.PnlTb.TabIndex = 0
            '
            'TmrLoad
            '
            Me.TmrLoad.Interval = 50
            '
            'TmrAutoSave
            '
            Me.TmrAutoSave.Interval = 60000
            '
            'Editor
            '
            Me.Editor.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.Editor.BackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.Editor.Controls.Add(Me.BtnKeyboard)
            Me.Editor.Controls.Add(Me.BtnNew)
            Me.Editor.Controls.Add(Me.BtnOpen)
            Me.Editor.Controls.Add(Me.BtnSave)
            Me.Editor.Controls.Add(Me.PnlTb)
            Me.Editor.Controls.Add(Me.BtnSettings)
            Me.Editor.Controls.Add(Me.BtnEval)
            Me.Editor.Controls.Add(Me.BtnFunctions)
            Me.Editor.Controls.Add(Me.PnlResults)
            Me.Editor.Location = New System.Drawing.Point(621, 0)
            Me.Editor.Name = "Editor"
            Me.Editor.Dock = DockStyle.Fill
            Me.Editor.TabIndex = 15
            '
            'Viewer
            '
            Me.Viewer.BackColor = System.Drawing.Color.FromArgb(CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer))
            Me.Viewer.Font = New System.Drawing.Font(OpenSansLight, 11.0!)
            Me.Viewer.Location = New System.Drawing.Point(0, 0)
            Me.Viewer.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
            Me.Viewer.Name = "Viewer"
            Me.Viewer.Dock = DockStyle.Fill
            Me.Viewer.Size = New System.Drawing.Size(621, 700)
            Me.Viewer.TabIndex = 17
            Me.Viewer.View = Cantus.UI.Viewer.ViewType.console
            '
            'TmrAnim
            '
            '
            'BtnKeyboard
            '
            Me.BtnKeyboard.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.BtnKeyboard.BackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.BtnKeyboard.Cursor = System.Windows.Forms.Cursors.Hand
            Me.BtnKeyboard.FlatAppearance.BorderSize = 0
            Me.BtnKeyboard.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(80, Byte), Integer), CType(CType(80, Byte), Integer), CType(CType(80, Byte), Integer))
            Me.BtnKeyboard.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.BtnKeyboard.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.BtnKeyboard.Font = New System.Drawing.Font(OpenSansLight, 15.0!)
            Me.BtnKeyboard.ForeColor = System.Drawing.Color.White
            Me.BtnKeyboard.Location = New System.Drawing.Point(675, 99)
            Me.BtnKeyboard.Name = "BtnKeyboard"
            Me.BtnKeyboard.Size = New System.Drawing.Size(53, 47)
            Me.BtnKeyboard.TabIndex = 31
            Me.BtnKeyboard.Text = "⌨"
            Me.TTLetters.SetToolTip(Me.BtnKeyboard, "Open the on-screen keyboard (Alt+K)")
            Me.BtnKeyboard.UseVisualStyleBackColor = False
            '
            'FrmEditor
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
            Me.AutoSize = False
            Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
            Me.BackColor = System.Drawing.Color.FromArgb(40, 40, 40)
            Me.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
            Me.ClientSize = New System.Drawing.Size(1349, 708)
            Me.ControlBox = False
            Me.Controls.Add(Me.Split)
            Me.Font = New System.Drawing.Font(OpenSansLight, 11.25!)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
            Me.KeyPreview = True
            Me.MaximizeBox = False
            Me.MinimumSize = New Size(150, 100)
            Me.Cursor = Cursors.SizeAll
            Me.Name = "FrmEditor"
            Me.StartPosition = System.Windows.Forms.FormStartPosition.Manual
            Me.Text = "Cantus"
            Me.PnlSettings.ResumeLayout(False)
            Me.PnlSettings.PerformLayout()
            CType(Me.PbSettingsLogo, System.ComponentModel.ISupportInitialize).EndInit()
            Me.PnlResults.ResumeLayout(False)
            Me.PnlTb.ResumeLayout(False)
            Me.Editor.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents Tb As ScintillaNET.Scintilla
        Friend WithEvents BtnEval As System.Windows.Forms.Button
        Friend WithEvents LbResult As System.Windows.Forms.Label
        Friend WithEvents PnlResults As System.Windows.Forms.Panel
        Friend WithEvents BtnSettings As System.Windows.Forms.Button
        Friend WithEvents PnlSettings As System.Windows.Forms.Panel
        Friend WithEvents BtnX As System.Windows.Forms.Button
        Friend WithEvents BtnY As System.Windows.Forms.Button
        Friend WithEvents BtnOutputFormat As System.Windows.Forms.Button
        Friend WithEvents TTLetters As System.Windows.Forms.ToolTip
        Friend WithEvents BtnClose As Button
        Friend WithEvents PnlTb As Panel
        Friend WithEvents BtnMin As Button
        Friend WithEvents BtnMax As Button
        Friend WithEvents BtnAngleRepr As Button
        Friend WithEvents LbAbout As Label
        Friend WithEvents CbAutoUpd As CheckBox
        Friend WithEvents BtnUpdate As Button
        Friend WithEvents BtnExplicit As Button
        Friend WithEvents BtnFunctions As Button
        Friend WithEvents BtnLog As Button
        Friend WithEvents LbSettings As Label
        Friend WithEvents BtnSave As Button
        Friend WithEvents BtnOpen As Button
        Friend WithEvents PbSettingsLogo As PictureBox
        Friend WithEvents TmrLoad As Timer
        Friend WithEvents TmrAutoSave As Timer
        Friend WithEvents BtnTranslucent As Button
        Friend WithEvents BtnNew As Button
        Friend WithEvents Editor As Panel
        Friend WithEvents Viewer As Viewer
        Friend WithEvents TmrAnim As Timer
        Friend WithEvents BtnSigFigs As Button
        Friend WithEvents BtnKeyboard As Button
        Friend WithEvents Split As SplitContainer
    End Class
End Namespace
