Namespace Calculator
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class FrmCalc
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmCalc))
            Me.tb = New System.Windows.Forms.TextBox()
            Me.pnlSettings = New System.Windows.Forms.Panel()
            Me.cbAutoUpd = New System.Windows.Forms.CheckBox()
            Me.btnUpdate = New System.Windows.Forms.Button()
            Me.lbAbout = New System.Windows.Forms.Label()
            Me.btnAngleRep = New System.Windows.Forms.Button()
            Me.btnT = New System.Windows.Forms.Button()
            Me.btnB = New System.Windows.Forms.Button()
            Me.btnA = New System.Windows.Forms.Button()
            Me.btnM = New System.Windows.Forms.Button()
            Me.btnOMode = New System.Windows.Forms.Button()
            Me.btnY = New System.Windows.Forms.Button()
            Me.btnX = New System.Windows.Forms.Button()
            Me.btnCalc = New System.Windows.Forms.Button()
            Me.lbResult = New System.Windows.Forms.Label()
            Me.pnlResults = New System.Windows.Forms.Panel()
            Me.btnMin = New System.Windows.Forms.Button()
            Me.btnClose = New System.Windows.Forms.Button()
            Me.btnGraph = New System.Windows.Forms.Button()
            Me.btnSettings = New System.Windows.Forms.Button()
            Me.TTLetters = New System.Windows.Forms.ToolTip(Me.components)
            Me.pnlTb = New System.Windows.Forms.Panel()
            Me.TmrLoad = New System.Windows.Forms.Timer(Me.components)
            Me.TmrReCalc = New System.Windows.Forms.Timer(Me.components)
            Me.pnlSettings.SuspendLayout()
            Me.pnlResults.SuspendLayout()
            Me.pnlTb.SuspendLayout()
            Me.SuspendLayout()
            '
            'tb
            '
            Me.tb.AcceptsTab = True
            Me.tb.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.tb.BackColor = System.Drawing.Color.FromArgb(CType(CType(34, Byte), Integer), CType(CType(34, Byte), Integer), CType(CType(34, Byte), Integer))
            Me.tb.BorderStyle = System.Windows.Forms.BorderStyle.None
            Me.tb.Cursor = System.Windows.Forms.Cursors.Arrow
            Me.tb.Font = New System.Drawing.Font("Segoe UI Semilight", 15.75!)
            Me.tb.ForeColor = System.Drawing.Color.White
            Me.tb.HideSelection = False
            Me.tb.Location = New System.Drawing.Point(13, 9)
            Me.tb.Multiline = True
            Me.tb.Name = "tb"
            Me.tb.Size = New System.Drawing.Size(314, 124)
            Me.tb.TabIndex = 0
            '
            'pnlSettings
            '
            Me.pnlSettings.BackColor = System.Drawing.Color.Black
            Me.pnlSettings.Controls.Add(Me.cbAutoUpd)
            Me.pnlSettings.Controls.Add(Me.btnUpdate)
            Me.pnlSettings.Controls.Add(Me.lbAbout)
            Me.pnlSettings.Controls.Add(Me.btnAngleRep)
            Me.pnlSettings.Controls.Add(Me.btnT)
            Me.pnlSettings.Controls.Add(Me.btnB)
            Me.pnlSettings.Controls.Add(Me.btnA)
            Me.pnlSettings.Controls.Add(Me.btnM)
            Me.pnlSettings.Controls.Add(Me.btnOMode)
            Me.pnlSettings.Controls.Add(Me.btnY)
            Me.pnlSettings.Controls.Add(Me.btnX)
            Me.pnlSettings.Location = New System.Drawing.Point(0, 0)
            Me.pnlSettings.Name = "pnlSettings"
            Me.pnlSettings.Size = New System.Drawing.Size(337, 144)
            Me.pnlSettings.TabIndex = 6
            Me.pnlSettings.Visible = False
            '
            'cbAutoUpd
            '
            Me.cbAutoUpd.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
            Me.cbAutoUpd.AutoSize = True
            Me.cbAutoUpd.BackColor = System.Drawing.Color.Transparent
            Me.cbAutoUpd.Cursor = System.Windows.Forms.Cursors.Hand
            Me.cbAutoUpd.Font = New System.Drawing.Font("Segoe UI Semilight", 9.0!)
            Me.cbAutoUpd.ForeColor = System.Drawing.Color.White
            Me.cbAutoUpd.Location = New System.Drawing.Point(212, 119)
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
            Me.btnUpdate.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
            Me.btnUpdate.BackColor = System.Drawing.Color.DarkSlateBlue
            Me.btnUpdate.Cursor = System.Windows.Forms.Cursors.Hand
            Me.btnUpdate.FlatAppearance.BorderSize = 0
            Me.btnUpdate.FlatAppearance.MouseDownBackColor = System.Drawing.Color.SlateBlue
            Me.btnUpdate.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkSlateBlue
            Me.btnUpdate.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnUpdate.Font = New System.Drawing.Font("Segoe UI Semilight", 10.0!)
            Me.btnUpdate.ForeColor = System.Drawing.Color.White
            Me.btnUpdate.Location = New System.Drawing.Point(221, 61)
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
            Me.lbAbout.AutoSize = True
            Me.lbAbout.BackColor = System.Drawing.Color.Transparent
            Me.lbAbout.Cursor = System.Windows.Forms.Cursors.Hand
            Me.lbAbout.Font = New System.Drawing.Font("Segoe UI Semilight", 9.0!)
            Me.lbAbout.ForeColor = System.Drawing.Color.White
            Me.lbAbout.Location = New System.Drawing.Point(14, 121)
            Me.lbAbout.Name = "lbAbout"
            Me.lbAbout.Size = New System.Drawing.Size(137, 15)
            Me.lbAbout.TabIndex = 8
            Me.lbAbout.Tag = "-"
            Me.lbAbout.Text = "Cantus v{VER} By Alex Yu"
            '
            'btnAngleRep
            '
            Me.btnAngleRep.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnAngleRep.BackColor = System.Drawing.Color.FromArgb(CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer))
            Me.btnAngleRep.Cursor = System.Windows.Forms.Cursors.Hand
            Me.btnAngleRep.FlatAppearance.BorderSize = 0
            Me.btnAngleRep.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer))
            Me.btnAngleRep.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer))
            Me.btnAngleRep.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnAngleRep.Font = New System.Drawing.Font("Segoe UI Semilight", 12.75!)
            Me.btnAngleRep.ForeColor = System.Drawing.Color.White
            Me.btnAngleRep.Location = New System.Drawing.Point(29, 61)
            Me.btnAngleRep.Name = "btnAngleRep"
            Me.btnAngleRep.Size = New System.Drawing.Size(86, 50)
            Me.btnAngleRep.TabIndex = 7
            Me.btnAngleRep.Tag = "-"
            Me.btnAngleRep.Text = "Radian"
            Me.TTLetters.SetToolTip(Me.btnAngleRep, "Change the angle representation mode:" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Degree / Radian / Gradian" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "(Hotkey: Ctrl+P" &
        " or Ctrl+<first letter of mode name>)")
            Me.btnAngleRep.UseVisualStyleBackColor = False
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
            Me.btnT.Location = New System.Drawing.Point(125, 13)
            Me.btnT.Name = "btnT"
            Me.btnT.Size = New System.Drawing.Size(40, 40)
            Me.btnT.TabIndex = 2
            Me.btnT.Tag = "t"
            Me.btnT.Text = "&t"
            Me.btnT.UseVisualStyleBackColor = False
            '
            'btnB
            '
            Me.btnB.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnB.BackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.btnB.Cursor = System.Windows.Forms.Cursors.Hand
            Me.btnB.FlatAppearance.BorderSize = 0
            Me.btnB.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(65, Byte), Integer), CType(CType(65, Byte), Integer), CType(CType(65, Byte), Integer))
            Me.btnB.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.btnB.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnB.Font = New System.Drawing.Font("Segoe UI Semilight", 12.0!)
            Me.btnB.ForeColor = System.Drawing.Color.White
            Me.btnB.Location = New System.Drawing.Point(267, 13)
            Me.btnB.Name = "btnB"
            Me.btnB.Size = New System.Drawing.Size(40, 40)
            Me.btnB.TabIndex = 5
            Me.btnB.Tag = "b"
            Me.btnB.Text = "&b"
            Me.btnB.UseVisualStyleBackColor = False
            '
            'btnA
            '
            Me.btnA.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnA.BackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.btnA.Cursor = System.Windows.Forms.Cursors.Hand
            Me.btnA.FlatAppearance.BorderSize = 0
            Me.btnA.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(65, Byte), Integer), CType(CType(65, Byte), Integer), CType(CType(65, Byte), Integer))
            Me.btnA.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.btnA.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnA.Font = New System.Drawing.Font("Segoe UI Semilight", 12.0!)
            Me.btnA.ForeColor = System.Drawing.Color.White
            Me.btnA.Location = New System.Drawing.Point(221, 13)
            Me.btnA.Name = "btnA"
            Me.btnA.Size = New System.Drawing.Size(40, 40)
            Me.btnA.TabIndex = 4
            Me.btnA.Tag = "a"
            Me.btnA.Text = "&a"
            Me.btnA.UseVisualStyleBackColor = False
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
            Me.btnM.Location = New System.Drawing.Point(171, 13)
            Me.btnM.Name = "btnM"
            Me.btnM.Size = New System.Drawing.Size(40, 40)
            Me.btnM.TabIndex = 3
            Me.btnM.Tag = "m"
            Me.btnM.Text = "&m"
            Me.btnM.UseVisualStyleBackColor = False
            '
            'btnOMode
            '
            Me.btnOMode.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnOMode.BackColor = System.Drawing.Color.FromArgb(CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer))
            Me.btnOMode.Cursor = System.Windows.Forms.Cursors.Hand
            Me.btnOMode.FlatAppearance.BorderSize = 0
            Me.btnOMode.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer))
            Me.btnOMode.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer))
            Me.btnOMode.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnOMode.Font = New System.Drawing.Font("Segoe UI Semilight", 12.75!)
            Me.btnOMode.ForeColor = System.Drawing.Color.White
            Me.btnOMode.Location = New System.Drawing.Point(125, 61)
            Me.btnOMode.Name = "btnOMode"
            Me.btnOMode.Size = New System.Drawing.Size(86, 50)
            Me.btnOMode.TabIndex = 8
            Me.btnOMode.Tag = "-"
            Me.btnOMode.Text = "MathO"
            Me.TTLetters.SetToolTip(Me.btnOMode, "Change the output mode:" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "MathO: Output fractions, roots, etc." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "SciO: Output scien" &
        "tific notation " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "LineO: Output full decimal number" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "(Hotkey: Ctrl+O or Ctrl+<fir" &
        "st letter of mode name>)")
            Me.btnOMode.UseVisualStyleBackColor = False
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
            Me.btnY.Location = New System.Drawing.Point(75, 13)
            Me.btnY.Name = "btnY"
            Me.btnY.Size = New System.Drawing.Size(40, 40)
            Me.btnY.TabIndex = 1
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
            Me.btnX.Location = New System.Drawing.Point(29, 13)
            Me.btnX.Name = "btnX"
            Me.btnX.Size = New System.Drawing.Size(40, 40)
            Me.btnX.TabIndex = 0
            Me.btnX.Tag = "x"
            Me.btnX.Text = "&x"
            Me.btnX.UseVisualStyleBackColor = False
            '
            'btnCalc
            '
            Me.btnCalc.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnCalc.BackColor = System.Drawing.Color.FromArgb(CType(CType(160, Byte), Integer), CType(CType(70, Byte), Integer), CType(CType(10, Byte), Integer))
            Me.btnCalc.Cursor = System.Windows.Forms.Cursors.Hand
            Me.btnCalc.FlatAppearance.BorderSize = 0
            Me.btnCalc.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(210, Byte), Integer), CType(CType(80, Byte), Integer), CType(CType(20, Byte), Integer))
            Me.btnCalc.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(160, Byte), Integer), CType(CType(70, Byte), Integer), CType(CType(10, Byte), Integer))
            Me.btnCalc.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnCalc.ForeColor = System.Drawing.Color.White
            Me.btnCalc.Location = New System.Drawing.Point(330, 139)
            Me.btnCalc.Name = "btnCalc"
            Me.btnCalc.Size = New System.Drawing.Size(60, 53)
            Me.btnCalc.TabIndex = 1
            Me.btnCalc.Text = "="
            Me.TTLetters.SetToolTip(Me.btnCalc, "Evaluate the expression (Alt+Enter)")
            Me.btnCalc.UseVisualStyleBackColor = False
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
            Me.lbResult.Size = New System.Drawing.Size(278, 46)
            Me.lbResult.TabIndex = 1
            Me.lbResult.Text = "= "
            Me.lbResult.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            Me.TTLetters.SetToolTip(Me.lbResult, "=")
            '
            'pnlResults
            '
            Me.pnlResults.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.pnlResults.BackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.pnlResults.Controls.Add(Me.btnMin)
            Me.pnlResults.Controls.Add(Me.lbResult)
            Me.pnlResults.Location = New System.Drawing.Point(-1, -1)
            Me.pnlResults.Name = "pnlResults"
            Me.pnlResults.Size = New System.Drawing.Size(389, 49)
            Me.pnlResults.TabIndex = 4
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
            Me.btnMin.Location = New System.Drawing.Point(282, 1)
            Me.btnMin.Name = "btnMin"
            Me.btnMin.Size = New System.Drawing.Size(53, 48)
            Me.btnMin.TabIndex = 0
            Me.btnMin.Text = "-"
            Me.TTLetters.SetToolTip(Me.btnMin, "Minimize (Win+Down)")
            Me.btnMin.UseVisualStyleBackColor = False
            '
            'btnClose
            '
            Me.btnClose.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnClose.BackColor = System.Drawing.Color.Brown
            Me.btnClose.Cursor = System.Windows.Forms.Cursors.Hand
            Me.btnClose.FlatAppearance.BorderSize = 0
            Me.btnClose.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(170, Byte), Integer), CType(CType(60, Byte), Integer), CType(CType(60, Byte), Integer))
            Me.btnClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Brown
            Me.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnClose.ForeColor = System.Drawing.Color.White
            Me.btnClose.Image = CType(resources.GetObject("btnClose.Image"), System.Drawing.Image)
            Me.btnClose.Location = New System.Drawing.Point(334, -1)
            Me.btnClose.Name = "btnClose"
            Me.btnClose.Size = New System.Drawing.Size(53, 49)
            Me.btnClose.TabIndex = 5
            Me.btnClose.UseVisualStyleBackColor = False
            '
            'btnGraph
            '
            Me.btnGraph.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnGraph.BackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.btnGraph.Cursor = System.Windows.Forms.Cursors.Hand
            Me.btnGraph.FlatAppearance.BorderSize = 0
            Me.btnGraph.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(80, Byte), Integer), CType(CType(80, Byte), Integer), CType(CType(80, Byte), Integer))
            Me.btnGraph.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.btnGraph.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnGraph.Font = New System.Drawing.Font("Segoe UI Semilight", 1.0!)
            Me.btnGraph.ForeColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.btnGraph.Image = CType(resources.GetObject("btnGraph.Image"), System.Drawing.Image)
            Me.btnGraph.Location = New System.Drawing.Point(334, 95)
            Me.btnGraph.Name = "btnGraph"
            Me.btnGraph.Size = New System.Drawing.Size(53, 47)
            Me.btnGraph.TabIndex = 2
            Me.btnGraph.Text = "&G"
            Me.btnGraph.TextAlign = System.Drawing.ContentAlignment.TopRight
            Me.TTLetters.SetToolTip(Me.btnGraph, "Open the graphing window (Alt+G)")
            Me.btnGraph.UseVisualStyleBackColor = False
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
            Me.btnSettings.Location = New System.Drawing.Point(334, 48)
            Me.btnSettings.Name = "btnSettings"
            Me.btnSettings.Size = New System.Drawing.Size(53, 47)
            Me.btnSettings.TabIndex = 3
            Me.btnSettings.Text = "&S"
            Me.btnSettings.TextAlign = System.Drawing.ContentAlignment.TopRight
            Me.TTLetters.SetToolTip(Me.btnSettings, "Open the settings panel (Alt+S)")
            Me.btnSettings.UseVisualStyleBackColor = False
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
            Me.pnlTb.Size = New System.Drawing.Size(337, 152)
            Me.pnlTb.TabIndex = 0
            '
            'TmrLoad
            '
            Me.TmrLoad.Interval = 50
            '
            'TmrReCalc
            '
            Me.TmrReCalc.Interval = 150
            '
            'FrmCalc
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
            Me.AutoSize = True
            Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
            Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer))
            Me.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
            Me.ClientSize = New System.Drawing.Size(387, 191)
            Me.ControlBox = False
            Me.Controls.Add(Me.btnClose)
            Me.Controls.Add(Me.pnlResults)
            Me.Controls.Add(Me.pnlTb)
            Me.Controls.Add(Me.btnGraph)
            Me.Controls.Add(Me.btnSettings)
            Me.Controls.Add(Me.btnCalc)
            Me.Font = New System.Drawing.Font("Segoe UI Semilight", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
            Me.KeyPreview = True
            Me.MaximizeBox = False
            Me.MaximumSize = New System.Drawing.Size(387, 191)
            Me.MinimumSize = New System.Drawing.Size(387, 191)
            Me.Name = "FrmCalc"
            Me.StartPosition = System.Windows.Forms.FormStartPosition.Manual
            Me.Text = "Cantus"
            Me.TopMost = True
            Me.pnlSettings.ResumeLayout(False)
            Me.pnlSettings.PerformLayout()
            Me.pnlResults.ResumeLayout(False)
            Me.pnlTb.ResumeLayout(False)
            Me.pnlTb.PerformLayout()
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents tb As System.Windows.Forms.TextBox
        Friend WithEvents btnCalc As System.Windows.Forms.Button
        Friend WithEvents lbResult As System.Windows.Forms.Label
        Friend WithEvents pnlResults As System.Windows.Forms.Panel
        Friend WithEvents btnSettings As System.Windows.Forms.Button
        Friend WithEvents pnlSettings As System.Windows.Forms.Panel
        Friend WithEvents btnX As System.Windows.Forms.Button
        Friend WithEvents btnY As System.Windows.Forms.Button
        Friend WithEvents btnOMode As System.Windows.Forms.Button
        Friend WithEvents btnM As System.Windows.Forms.Button
        Friend WithEvents btnT As System.Windows.Forms.Button
        Friend WithEvents btnB As System.Windows.Forms.Button
        Friend WithEvents btnA As System.Windows.Forms.Button
        Friend WithEvents TTLetters As System.Windows.Forms.ToolTip
        Friend WithEvents btnGraph As Button
        Friend WithEvents btnClose As Button
        Friend WithEvents pnlTb As Panel
        Friend WithEvents btnMin As Button
        Friend WithEvents btnAngleRep As Button
        Friend WithEvents lbAbout As Label
        Friend WithEvents cbAutoUpd As CheckBox
        Friend WithEvents btnUpdate As Button
        Friend WithEvents TmrLoad As Timer
        Friend WithEvents TmrReCalc As Timer
    End Class
End Namespace
