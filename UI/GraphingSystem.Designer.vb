Namespace UI.Graphing
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class GraphingSystem
        Inherits System.Windows.Forms.UserControl

        'Form overrides dispose to clean up the component list.
        <System.Diagnostics.DebuggerNonUserCode()>
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            Try
                My.Settings.GraphSplitter = Me.Height - split.SplitterDistance
                My.Settings.Save()

                _buffer.Dispose()
                _tmpbuffer.Dispose()

                If disposing AndAlso components IsNot Nothing Then
                    components.Dispose()
                End If
                MyBase.Dispose(disposing)
            Catch
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(GraphingSystem))
            Me.split = New System.Windows.Forms.SplitContainer()
            Me.btnScale = New System.Windows.Forms.Button()
            Me.pnlWindow = New System.Windows.Forms.Panel()
            Me.tbWBot = New System.Windows.Forms.TextBox()
            Me.tbWLft = New System.Windows.Forms.TextBox()
            Me.tbWTop = New System.Windows.Forms.TextBox()
            Me.tbWRht = New System.Windows.Forms.TextBox()
            Me.btnWOK = New System.Windows.Forms.Button()
            Me.btnWCancel = New System.Windows.Forms.Button()
            Me.lbWLft = New System.Windows.Forms.Label()
            Me.lbWRht = New System.Windows.Forms.Label()
            Me.lbWBot = New System.Windows.Forms.Label()
            Me.lbWTop = New System.Windows.Forms.Label()
            Me.pnlWHeader = New System.Windows.Forms.Panel()
            Me.lbWLogo = New System.Windows.Forms.Label()
            Me.btnWClose = New System.Windows.Forms.Button()
            Me.lbWindow = New System.Windows.Forms.Label()
            Me.pnlTrace = New System.Windows.Forms.Panel()
            Me.lbTrace = New System.Windows.Forms.Label()
            Me.lbTVal = New System.Windows.Forms.Label()
            Me.npdTVal = New System.Windows.Forms.TextBox()
            Me.btnTNext = New System.Windows.Forms.Button()
            Me.btnTrace = New System.Windows.Forms.Button()
            Me.canvas = New System.Windows.Forms.PictureBox()
            Me.btnGraph = New System.Windows.Forms.Button()
            Me.btnNextFn = New System.Windows.Forms.Button()
            Me.btnPrevFn = New System.Windows.Forms.Button()
            Me.btnAdd = New System.Windows.Forms.Button()
            Me.pnlInput = New System.Windows.Forms.Panel()
            Me.tb = New System.Windows.Forms.TextBox()
            Me.lbFx = New System.Windows.Forms.Label()
            Me.tmrTraceUpdate = New System.Windows.Forms.Timer(Me.components)
            Me.tmrDrag = New System.Windows.Forms.Timer(Me.components)
            Me.tt = New System.Windows.Forms.ToolTip(Me.components)
            Me.btnFnDel = New System.Windows.Forms.Button()
            Me.pnlFnType = New System.Windows.Forms.Panel()
            Me.pnlFnTypeSelector = New System.Windows.Forms.Panel()
            Me.pnlOptInverse = New System.Windows.Forms.Panel()
            Me.lbOptInverseR = New System.Windows.Forms.Label()
            Me.lbOptInverseL = New System.Windows.Forms.Label()
            Me.pnlOptOriginRay = New System.Windows.Forms.Panel()
            Me.lbOptOriginRayR = New System.Windows.Forms.Label()
            Me.lbOptOriginRayL = New System.Windows.Forms.Label()
            Me.pnlOptPolar = New System.Windows.Forms.Panel()
            Me.lbOptPolarR = New System.Windows.Forms.Label()
            Me.lbOptPolarL = New System.Windows.Forms.Label()
            Me.pnlOptParametric = New System.Windows.Forms.Panel()
            Me.lbOptParametricR = New System.Windows.Forms.Label()
            Me.lbOptParametricL = New System.Windows.Forms.Label()
            Me.pnlOptCartesian = New System.Windows.Forms.Panel()
            Me.lbOptCartesianR = New System.Windows.Forms.Label()
            Me.lbOptCartesianL = New System.Windows.Forms.Label()
            Me.pnlOptDifferential = New System.Windows.Forms.Panel()
            Me.lbOptDifferentialR = New System.Windows.Forms.Label()
            Me.lbOptDifferentialL = New System.Windows.Forms.Label()
            Me.lbFnType = New System.Windows.Forms.Label()
            Me.tmrDelayRedraw = New System.Windows.Forms.Timer(Me.components)
            Me.DrawWorker = New System.ComponentModel.BackgroundWorker()
            Me.TmrStart = New System.Windows.Forms.Timer(Me.components)
            Me.TmrHighQuality = New System.Windows.Forms.Timer(Me.components)
            CType(Me.split, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.split.Panel1.SuspendLayout()
            Me.split.Panel2.SuspendLayout()
            Me.split.SuspendLayout()
            Me.pnlWindow.SuspendLayout()
            Me.pnlWHeader.SuspendLayout()
            Me.pnlTrace.SuspendLayout()
            CType(Me.canvas, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.pnlInput.SuspendLayout()
            Me.pnlFnType.SuspendLayout()
            Me.pnlFnTypeSelector.SuspendLayout()
            Me.pnlOptInverse.SuspendLayout()
            Me.pnlOptOriginRay.SuspendLayout()
            Me.pnlOptPolar.SuspendLayout()
            Me.pnlOptParametric.SuspendLayout()
            Me.pnlOptCartesian.SuspendLayout()
            Me.pnlOptDifferential.SuspendLayout()
            Me.SuspendLayout()
            '
            'split
            '
            Me.split.BackColor = System.Drawing.Color.FromArgb(CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer))
            Me.split.Dock = System.Windows.Forms.DockStyle.Fill
            Me.split.FixedPanel = System.Windows.Forms.FixedPanel.Panel2
            Me.split.Location = New System.Drawing.Point(0, 0)
            Me.split.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
            Me.split.Name = "split"
            Me.split.Orientation = System.Windows.Forms.Orientation.Horizontal
            '
            'split.Panel1
            '
            Me.split.Panel1.Controls.Add(Me.btnScale)
            Me.split.Panel1.Controls.Add(Me.pnlWindow)
            Me.split.Panel1.Controls.Add(Me.pnlTrace)
            Me.split.Panel1.Controls.Add(Me.btnTrace)
            Me.split.Panel1.Controls.Add(Me.canvas)
            '
            'split.Panel2
            '
            Me.split.Panel2.BackColor = System.Drawing.Color.FromArgb(CType(CType(45, Byte), Integer), CType(CType(45, Byte), Integer), CType(CType(45, Byte), Integer))
            Me.split.Panel2.Controls.Add(Me.btnGraph)
            Me.split.Panel2.Controls.Add(Me.btnNextFn)
            Me.split.Panel2.Controls.Add(Me.btnPrevFn)
            Me.split.Panel2.Controls.Add(Me.btnAdd)
            Me.split.Panel2.Controls.Add(Me.pnlInput)
            Me.split.Panel2.Controls.Add(Me.lbFx)
            Me.split.Size = New System.Drawing.Size(984, 661)
            Me.split.SplitterDistance = 600
            Me.split.SplitterWidth = 3
            Me.split.TabIndex = 0
            '
            'btnScale
            '
            Me.btnScale.BackColor = System.Drawing.Color.DarkSlateBlue
            Me.btnScale.BackgroundImage = CType(resources.GetObject("btnScale.BackgroundImage"), System.Drawing.Image)
            Me.btnScale.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
            Me.btnScale.Cursor = System.Windows.Forms.Cursors.Hand
            Me.btnScale.FlatAppearance.BorderSize = 0
            Me.btnScale.FlatAppearance.MouseDownBackColor = System.Drawing.Color.SlateBlue
            Me.btnScale.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkSlateBlue
            Me.btnScale.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnScale.Font = New System.Drawing.Font(OpenSansLight, 14.0!)
            Me.btnScale.ForeColor = System.Drawing.Color.White
            Me.btnScale.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
            Me.btnScale.Location = New System.Drawing.Point(11, 11)
            Me.btnScale.Margin = New System.Windows.Forms.Padding(2)
            Me.btnScale.Name = "btnScale"
            Me.btnScale.Size = New System.Drawing.Size(115, 40)
            Me.btnScale.TabIndex = 4
            Me.btnScale.TabStop = False
            Me.btnScale.Text = "Window"
            Me.btnScale.TextAlign = System.Drawing.ContentAlignment.MiddleRight
            Me.tt.SetToolTip(Me.btnScale, "Left click to adjust the range graphed on the window" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Right click to reset to def" &         "ault (centered) view" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Middle click to go to first quadrant view")
            Me.btnScale.UseVisualStyleBackColor = False
            '
            'pnlWindow
            '
            Me.pnlWindow.BackColor = System.Drawing.Color.FromArgb(CType(CType(45, Byte), Integer), CType(CType(45, Byte), Integer), CType(CType(45, Byte), Integer))
            Me.pnlWindow.Controls.Add(Me.tbWBot)
            Me.pnlWindow.Controls.Add(Me.tbWLft)
            Me.pnlWindow.Controls.Add(Me.tbWTop)
            Me.pnlWindow.Controls.Add(Me.tbWRht)
            Me.pnlWindow.Controls.Add(Me.btnWOK)
            Me.pnlWindow.Controls.Add(Me.btnWCancel)
            Me.pnlWindow.Controls.Add(Me.lbWLft)
            Me.pnlWindow.Controls.Add(Me.lbWRht)
            Me.pnlWindow.Controls.Add(Me.lbWBot)
            Me.pnlWindow.Controls.Add(Me.lbWTop)
            Me.pnlWindow.Controls.Add(Me.pnlWHeader)
            Me.pnlWindow.Location = New System.Drawing.Point(280, 160)
            Me.pnlWindow.Name = "pnlWindow"
            Me.pnlWindow.Size = New System.Drawing.Size(469, 305)
            Me.pnlWindow.TabIndex = 9
            Me.pnlWindow.Visible = False
            '
            'tbWBot
            '
            Me.tbWBot.Anchor = System.Windows.Forms.AnchorStyles.Bottom
            Me.tbWBot.Font = New System.Drawing.Font(OpenSansLight, 15.0!)
            Me.tbWBot.Location = New System.Drawing.Point(156, 212)
            Me.tbWBot.Name = "tbWBot"
            Me.tbWBot.Size = New System.Drawing.Size(156, 35)
            Me.tbWBot.TabIndex = 12
            Me.tt.SetToolTip(Me.tbWBot, "Minimum Y (Expression)")
            '
            'tbWLft
            '
            Me.tbWLft.Anchor = System.Windows.Forms.AnchorStyles.Left
            Me.tbWLft.Font = New System.Drawing.Font(OpenSansLight, 15.0!)
            Me.tbWLft.Location = New System.Drawing.Point(25, 147)
            Me.tbWLft.Name = "tbWLft"
            Me.tbWLft.Size = New System.Drawing.Size(156, 35)
            Me.tbWLft.TabIndex = 14
            Me.tt.SetToolTip(Me.tbWLft, "Minimum X (Expression)")
            '
            'tbWTop
            '
            Me.tbWTop.Anchor = System.Windows.Forms.AnchorStyles.Top
            Me.tbWTop.Font = New System.Drawing.Font(OpenSansLight, 15.0!)
            Me.tbWTop.Location = New System.Drawing.Point(156, 85)
            Me.tbWTop.Name = "tbWTop"
            Me.tbWTop.Size = New System.Drawing.Size(156, 35)
            Me.tbWTop.TabIndex = 10
            Me.tt.SetToolTip(Me.tbWTop, "Maximum Y (Expression)")
            '
            'tbWRht
            '
            Me.tbWRht.Anchor = System.Windows.Forms.AnchorStyles.Right
            Me.tbWRht.Font = New System.Drawing.Font(OpenSansLight, 15.0!)
            Me.tbWRht.Location = New System.Drawing.Point(289, 147)
            Me.tbWRht.Name = "tbWRht"
            Me.tbWRht.Size = New System.Drawing.Size(156, 35)
            Me.tbWRht.TabIndex = 16
            Me.tt.SetToolTip(Me.tbWRht, "Maximum X (Expression)")
            '
            'btnWOK
            '
            Me.btnWOK.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnWOK.BackColor = System.Drawing.Color.FromArgb(CType(CType(160, Byte), Integer), CType(CType(70, Byte), Integer), CType(CType(10, Byte), Integer))
            Me.btnWOK.Cursor = System.Windows.Forms.Cursors.Hand
            Me.btnWOK.FlatAppearance.BorderSize = 0
            Me.btnWOK.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(210, Byte), Integer), CType(CType(80, Byte), Integer), CType(CType(20, Byte), Integer))
            Me.btnWOK.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(160, Byte), Integer), CType(CType(70, Byte), Integer), CType(CType(10, Byte), Integer))
            Me.btnWOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnWOK.Font = New System.Drawing.Font(OpenSans, 11.0!)
            Me.btnWOK.ForeColor = System.Drawing.Color.White
            Me.btnWOK.Location = New System.Drawing.Point(368, 256)
            Me.btnWOK.Margin = New System.Windows.Forms.Padding(2)
            Me.btnWOK.Name = "btnWOK"
            Me.btnWOK.Size = New System.Drawing.Size(99, 47)
            Me.btnWOK.TabIndex = 20
            Me.btnWOK.TabStop = False
            Me.btnWOK.Text = "Update"
            Me.tt.SetToolTip(Me.btnWOK, "Update the Window Bounds (Enter)")
            Me.btnWOK.UseVisualStyleBackColor = False
            '
            'btnWCancel
            '
            Me.btnWCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnWCancel.BackColor = System.Drawing.Color.Transparent
            Me.btnWCancel.Cursor = System.Windows.Forms.Cursors.Hand
            Me.btnWCancel.FlatAppearance.BorderSize = 0
            Me.btnWCancel.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer))
            Me.btnWCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent
            Me.btnWCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnWCancel.Font = New System.Drawing.Font(OpenSansLight, 11.0!)
            Me.btnWCancel.ForeColor = System.Drawing.Color.White
            Me.btnWCancel.Location = New System.Drawing.Point(272, 253)
            Me.btnWCancel.Margin = New System.Windows.Forms.Padding(2)
            Me.btnWCancel.Name = "btnWCancel"
            Me.btnWCancel.Size = New System.Drawing.Size(92, 47)
            Me.btnWCancel.TabIndex = 18
            Me.btnWCancel.TabStop = False
            Me.btnWCancel.Text = "Cancel"
            Me.tt.SetToolTip(Me.btnWCancel, "Cancel Changes (Esc)")
            Me.btnWCancel.UseVisualStyleBackColor = False
            '
            'lbWLft
            '
            Me.lbWLft.Anchor = System.Windows.Forms.AnchorStyles.Left
            Me.lbWLft.AutoSize = True
            Me.lbWLft.Font = New System.Drawing.Font(OpenSansLight, 12.0!)
            Me.lbWLft.Location = New System.Drawing.Point(80, 117)
            Me.lbWLft.Name = "lbWLft"
            Me.lbWLft.Size = New System.Drawing.Size(49, 22)
            Me.lbWLft.TabIndex = 17
            Me.lbWLft.Text = "Min x"
            '
            'lbWRht
            '
            Me.lbWRht.Anchor = System.Windows.Forms.AnchorStyles.Right
            Me.lbWRht.AutoSize = True
            Me.lbWRht.Font = New System.Drawing.Font(OpenSansLight, 12.0!)
            Me.lbWRht.Location = New System.Drawing.Point(341, 119)
            Me.lbWRht.Name = "lbWRht"
            Me.lbWRht.Size = New System.Drawing.Size(52, 22)
            Me.lbWRht.TabIndex = 15
            Me.lbWRht.Text = "Max x"
            '
            'lbWBot
            '
            Me.lbWBot.Anchor = System.Windows.Forms.AnchorStyles.Bottom
            Me.lbWBot.AutoSize = True
            Me.lbWBot.Font = New System.Drawing.Font(OpenSansLight, 12.0!)
            Me.lbWBot.Location = New System.Drawing.Point(211, 182)
            Me.lbWBot.Name = "lbWBot"
            Me.lbWBot.Size = New System.Drawing.Size(48, 22)
            Me.lbWBot.TabIndex = 13
            Me.lbWBot.Text = "Min y"
            '
            'lbWTop
            '
            Me.lbWTop.Anchor = System.Windows.Forms.AnchorStyles.Top
            Me.lbWTop.AutoSize = True
            Me.lbWTop.Font = New System.Drawing.Font(OpenSansLight, 12.0!)
            Me.lbWTop.Location = New System.Drawing.Point(211, 56)
            Me.lbWTop.Name = "lbWTop"
            Me.lbWTop.Size = New System.Drawing.Size(51, 22)
            Me.lbWTop.TabIndex = 11
            Me.lbWTop.Text = "Max y"
            '
            'pnlWHeader
            '
            Me.pnlWHeader.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.pnlWHeader.BackColor = System.Drawing.Color.FromArgb(CType(CType(35, Byte), Integer), CType(CType(35, Byte), Integer), CType(CType(35, Byte), Integer))
            Me.pnlWHeader.Controls.Add(Me.lbWLogo)
            Me.pnlWHeader.Controls.Add(Me.btnWClose)
            Me.pnlWHeader.Controls.Add(Me.lbWindow)
            Me.pnlWHeader.Location = New System.Drawing.Point(0, 0)
            Me.pnlWHeader.Name = "pnlWHeader"
            Me.pnlWHeader.Size = New System.Drawing.Size(469, 47)
            Me.pnlWHeader.TabIndex = 19
            '
            'lbWLogo
            '
            Me.lbWLogo.Font = New System.Drawing.Font(OpenSansLight, 16.0!)
            Me.lbWLogo.Image = CType(resources.GetObject("lbWLogo.Image"), System.Drawing.Image)
            Me.lbWLogo.Location = New System.Drawing.Point(16, 7)
            Me.lbWLogo.Name = "lbWLogo"
            Me.lbWLogo.Size = New System.Drawing.Size(32, 30)
            Me.lbWLogo.TabIndex = 22
            Me.lbWLogo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            '
            'btnWClose
            '
            Me.btnWClose.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnWClose.BackColor = System.Drawing.Color.Transparent
            Me.btnWClose.Cursor = System.Windows.Forms.Cursors.Hand
            Me.btnWClose.FlatAppearance.BorderSize = 0
            Me.btnWClose.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer))
            Me.btnWClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent
            Me.btnWClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnWClose.Font = New System.Drawing.Font(OpenSansLight, 9.0!)
            Me.btnWClose.ForeColor = System.Drawing.Color.White
            Me.btnWClose.Location = New System.Drawing.Point(423, 2)
            Me.btnWClose.Margin = New System.Windows.Forms.Padding(2)
            Me.btnWClose.Name = "btnWClose"
            Me.btnWClose.Size = New System.Drawing.Size(44, 43)
            Me.btnWClose.TabIndex = 21
            Me.btnWClose.TabStop = False
            Me.btnWClose.Text = "X"
            Me.tt.SetToolTip(Me.btnWClose, "Close (Esc)")
            Me.btnWClose.UseVisualStyleBackColor = False
            '
            'lbWindow
            '
            Me.lbWindow.AutoSize = True
            Me.lbWindow.Font = New System.Drawing.Font(OpenSansLight, 12.0!)
            Me.lbWindow.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
            Me.lbWindow.Location = New System.Drawing.Point(48, 12)
            Me.lbWindow.Margin = New System.Windows.Forms.Padding(0, 0, 3, 0)
            Me.lbWindow.Name = "lbWindow"
            Me.lbWindow.Size = New System.Drawing.Size(193, 22)
            Me.lbWindow.TabIndex = 9
            Me.lbWindow.Text = "Window Bounds + Scaling"
            '
            'pnlTrace
            '
            Me.pnlTrace.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.pnlTrace.BackColor = System.Drawing.Color.FromArgb(CType(CType(60, Byte), Integer), CType(CType(60, Byte), Integer), CType(CType(60, Byte), Integer))
            Me.pnlTrace.Controls.Add(Me.lbTrace)
            Me.pnlTrace.Controls.Add(Me.lbTVal)
            Me.pnlTrace.Controls.Add(Me.npdTVal)
            Me.pnlTrace.Controls.Add(Me.btnTNext)
            Me.pnlTrace.Location = New System.Drawing.Point(676, 11)
            Me.pnlTrace.Name = "pnlTrace"
            Me.pnlTrace.Size = New System.Drawing.Size(228, 136)
            Me.pnlTrace.TabIndex = 10
            Me.pnlTrace.Visible = False
            '
            'lbTrace
            '
            Me.lbTrace.AutoSize = True
            Me.lbTrace.Font = New System.Drawing.Font(OpenSansLight, 12.0!)
            Me.lbTrace.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
            Me.lbTrace.Location = New System.Drawing.Point(10, 9)
            Me.lbTrace.Margin = New System.Windows.Forms.Padding(0, 0, 3, 0)
            Me.lbTrace.Name = "lbTrace"
            Me.lbTrace.Size = New System.Drawing.Size(114, 22)
            Me.lbTrace.TabIndex = 10
            Me.lbTrace.Text = "Trace Function"
            '
            'lbTVal
            '
            Me.lbTVal.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.lbTVal.BackColor = System.Drawing.Color.FromArgb(CType(CType(60, Byte), Integer), CType(CType(60, Byte), Integer), CType(CType(60, Byte), Integer))
            Me.lbTVal.Location = New System.Drawing.Point(-1, 74)
            Me.lbTVal.Name = "lbTVal"
            Me.lbTVal.Padding = New System.Windows.Forms.Padding(0, 3, 0, 0)
            Me.lbTVal.Size = New System.Drawing.Size(230, 62)
            Me.lbTVal.TabIndex = 3
            Me.lbTVal.Text = "f(x) = 0"
            Me.lbTVal.TextAlign = System.Drawing.ContentAlignment.TopRight
            '
            'npdTVal
            '
            Me.npdTVal.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.npdTVal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.npdTVal.Font = New System.Drawing.Font(OpenSansLight, 15.0!)
            Me.npdTVal.Location = New System.Drawing.Point(0, 41)
            Me.npdTVal.Multiline = True
            Me.npdTVal.Name = "npdTVal"
            Me.npdTVal.Size = New System.Drawing.Size(156, 46)
            Me.npdTVal.TabIndex = 4
            Me.tt.SetToolTip(Me.npdTVal, "Value to Trace At (Expression)")
            '
            'btnTNext
            '
            Me.btnTNext.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnTNext.BackColor = System.Drawing.Color.DimGray
            Me.btnTNext.Cursor = System.Windows.Forms.Cursors.Hand
            Me.btnTNext.FlatAppearance.BorderSize = 0
            Me.btnTNext.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DimGray
            Me.btnTNext.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gray
            Me.btnTNext.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnTNext.Font = New System.Drawing.Font(OpenSansLight, 7.0!)
            Me.btnTNext.ForeColor = System.Drawing.Color.White
            Me.btnTNext.Location = New System.Drawing.Point(156, 41)
            Me.btnTNext.Margin = New System.Windows.Forms.Padding(2)
            Me.btnTNext.Name = "btnTNext"
            Me.btnTNext.Size = New System.Drawing.Size(75, 34)
            Me.btnTNext.TabIndex = 5
            Me.btnTNext.TabStop = False
            Me.btnTNext.Text = "Find Crit Pts"
            Me.tt.SetToolTip(Me.btnTNext, "Find zeros, maxima, minima, and intercepts" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "(Enter)")
            Me.btnTNext.UseVisualStyleBackColor = False
            '
            'btnTrace
            '
            Me.btnTrace.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnTrace.BackColor = System.Drawing.Color.FromArgb(CType(CType(160, Byte), Integer), CType(CType(70, Byte), Integer), CType(CType(10, Byte), Integer))
            Me.btnTrace.Cursor = System.Windows.Forms.Cursors.Hand
            Me.btnTrace.FlatAppearance.BorderSize = 0
            Me.btnTrace.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(210, Byte), Integer), CType(CType(80, Byte), Integer), CType(CType(20, Byte), Integer))
            Me.btnTrace.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(160, Byte), Integer), CType(CType(70, Byte), Integer), CType(CType(10, Byte), Integer))
            Me.btnTrace.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnTrace.ForeColor = System.Drawing.Color.White
            Me.btnTrace.Location = New System.Drawing.Point(904, 11)
            Me.btnTrace.Margin = New System.Windows.Forms.Padding(2)
            Me.btnTrace.Name = "btnTrace"
            Me.btnTrace.Size = New System.Drawing.Size(68, 40)
            Me.btnTrace.TabIndex = 1
            Me.btnTrace.TabStop = False
            Me.btnTrace.Text = "Trace"
            Me.tt.SetToolTip(Me.btnTrace, "Trace this graph (Alt+T)")
            Me.btnTrace.UseVisualStyleBackColor = False
            '
            'canvas
            '
            Me.canvas.BackColor = System.Drawing.Color.FromArgb(CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer))
            Me.canvas.Dock = System.Windows.Forms.DockStyle.Fill
            Me.canvas.Location = New System.Drawing.Point(0, 0)
            Me.canvas.Name = "canvas"
            Me.canvas.Size = New System.Drawing.Size(984, 600)
            Me.canvas.TabIndex = 0
            Me.canvas.TabStop = False
            '
            'btnGraph
            '
            Me.btnGraph.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnGraph.BackColor = System.Drawing.Color.FromArgb(CType(CType(160, Byte), Integer), CType(CType(70, Byte), Integer), CType(CType(10, Byte), Integer))
            Me.btnGraph.Cursor = System.Windows.Forms.Cursors.Hand
            Me.btnGraph.FlatAppearance.BorderSize = 0
            Me.btnGraph.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(210, Byte), Integer), CType(CType(80, Byte), Integer), CType(CType(20, Byte), Integer))
            Me.btnGraph.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(160, Byte), Integer), CType(CType(70, Byte), Integer), CType(CType(10, Byte), Integer))
            Me.btnGraph.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnGraph.ForeColor = System.Drawing.Color.White
            Me.btnGraph.Location = New System.Drawing.Point(837, -1)
            Me.btnGraph.Margin = New System.Windows.Forms.Padding(2)
            Me.btnGraph.Name = "btnGraph"
            Me.btnGraph.Size = New System.Drawing.Size(105, 61)
            Me.btnGraph.TabIndex = 0
            Me.btnGraph.TabStop = False
            Me.btnGraph.Text = "Graph"
            Me.tt.SetToolTip(Me.btnGraph, "Update the graph of this function (Alt+Enter)")
            Me.btnGraph.UseVisualStyleBackColor = False
            '
            'btnNextFn
            '
            Me.btnNextFn.BackColor = System.Drawing.Color.FromArgb(CType(CType(60, Byte), Integer), CType(CType(60, Byte), Integer), CType(CType(60, Byte), Integer))
            Me.btnNextFn.Cursor = System.Windows.Forms.Cursors.Hand
            Me.btnNextFn.Enabled = False
            Me.btnNextFn.FlatAppearance.BorderSize = 0
            Me.btnNextFn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(35, Byte), Integer), CType(CType(35, Byte), Integer), CType(CType(35, Byte), Integer))
            Me.btnNextFn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent
            Me.btnNextFn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnNextFn.Font = New System.Drawing.Font(OpenSansLight, 11.0!)
            Me.btnNextFn.ForeColor = System.Drawing.Color.Silver
            Me.btnNextFn.Location = New System.Drawing.Point(0, 30)
            Me.btnNextFn.Margin = New System.Windows.Forms.Padding(2)
            Me.btnNextFn.Name = "btnNextFn"
            Me.btnNextFn.Size = New System.Drawing.Size(30, 30)
            Me.btnNextFn.TabIndex = 9
            Me.btnNextFn.TabStop = False
            Me.btnNextFn.Text = "▼"
            Me.tt.SetToolTip(Me.btnNextFn, "Go to next function (Alt+Down)")
            Me.btnNextFn.UseVisualStyleBackColor = False
            '
            'btnPrevFn
            '
            Me.btnPrevFn.BackColor = System.Drawing.Color.FromArgb(CType(CType(60, Byte), Integer), CType(CType(60, Byte), Integer), CType(CType(60, Byte), Integer))
            Me.btnPrevFn.Cursor = System.Windows.Forms.Cursors.Hand
            Me.btnPrevFn.Enabled = False
            Me.btnPrevFn.FlatAppearance.BorderSize = 0
            Me.btnPrevFn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(35, Byte), Integer), CType(CType(35, Byte), Integer), CType(CType(35, Byte), Integer))
            Me.btnPrevFn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent
            Me.btnPrevFn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnPrevFn.Font = New System.Drawing.Font(OpenSansLight, 11.0!)
            Me.btnPrevFn.ForeColor = System.Drawing.Color.Silver
            Me.btnPrevFn.Location = New System.Drawing.Point(0, 0)
            Me.btnPrevFn.Margin = New System.Windows.Forms.Padding(2)
            Me.btnPrevFn.Name = "btnPrevFn"
            Me.btnPrevFn.Size = New System.Drawing.Size(30, 30)
            Me.btnPrevFn.TabIndex = 8
            Me.btnPrevFn.TabStop = False
            Me.btnPrevFn.Text = "▲"
            Me.tt.SetToolTip(Me.btnPrevFn, "Go to previous function (Alt+Up)")
            Me.btnPrevFn.UseVisualStyleBackColor = False
            '
            'btnAdd
            '
            Me.btnAdd.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnAdd.BackColor = System.Drawing.Color.FromArgb(CType(CType(45, Byte), Integer), CType(CType(45, Byte), Integer), CType(CType(45, Byte), Integer))
            Me.btnAdd.Cursor = System.Windows.Forms.Cursors.Hand
            Me.btnAdd.FlatAppearance.BorderSize = 0
            Me.btnAdd.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(65, Byte), Integer), CType(CType(65, Byte), Integer), CType(CType(65, Byte), Integer))
            Me.btnAdd.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(45, Byte), Integer), CType(CType(45, Byte), Integer), CType(CType(45, Byte), Integer))
            Me.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnAdd.ForeColor = System.Drawing.Color.White
            Me.btnAdd.Location = New System.Drawing.Point(941, 1)
            Me.btnAdd.Margin = New System.Windows.Forms.Padding(2)
            Me.btnAdd.Name = "btnAdd"
            Me.btnAdd.Size = New System.Drawing.Size(43, 57)
            Me.btnAdd.TabIndex = 2
            Me.btnAdd.TabStop = False
            Me.btnAdd.Text = "+"
            Me.tt.SetToolTip(Me.btnAdd, "Add a new function (Alt+A or Alt++)")
            Me.btnAdd.UseVisualStyleBackColor = False
            '
            'pnlInput
            '
            Me.pnlInput.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.pnlInput.BackColor = System.Drawing.Color.Gainsboro
            Me.pnlInput.Controls.Add(Me.tb)
            Me.pnlInput.Location = New System.Drawing.Point(159, -3)
            Me.pnlInput.Name = "pnlInput"
            Me.pnlInput.Size = New System.Drawing.Size(682, 84)
            Me.pnlInput.TabIndex = 1
            '
            'tb
            '
            Me.tb.AcceptsTab = True
            Me.tb.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.tb.BackColor = System.Drawing.Color.Gainsboro
            Me.tb.BorderStyle = System.Windows.Forms.BorderStyle.None
            Me.tb.Cursor = System.Windows.Forms.Cursors.Arrow
            Me.tb.Font = New System.Drawing.Font(OpenSansLight, 14.0!)
            Me.tb.ForeColor = System.Drawing.Color.FromArgb(CType(CType(20, Byte), Integer), CType(CType(20, Byte), Integer), CType(CType(20, Byte), Integer))
            Me.tb.HideSelection = False
            Me.tb.Location = New System.Drawing.Point(7, 8)
            Me.tb.Multiline = True
            Me.tb.Name = "tb"
            Me.tb.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal
            Me.tb.Size = New System.Drawing.Size(673, 53)
            Me.tb.TabIndex = 0
            '
            'lbFx
            '
            Me.lbFx.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
            Me.lbFx.AutoEllipsis = True
            Me.lbFx.BackColor = System.Drawing.Color.FromArgb(CType(CType(45, Byte), Integer), CType(CType(45, Byte), Integer), CType(CType(45, Byte), Integer))
            Me.lbFx.Cursor = System.Windows.Forms.Cursors.Hand
            Me.lbFx.Font = New System.Drawing.Font(OpenSansLight, 14.0!)
            Me.lbFx.Location = New System.Drawing.Point(32, 0)
            Me.lbFx.Name = "lbFx"
            Me.lbFx.Size = New System.Drawing.Size(123, 60)
            Me.lbFx.TabIndex = 3
            Me.lbFx.Text = "f(x) = "
            Me.lbFx.TextAlign = System.Drawing.ContentAlignment.MiddleRight
            Me.tt.SetToolTip(Me.lbFx, "Click to change function type")
            '
            'tmrTraceUpdate
            '
            Me.tmrTraceUpdate.Interval = 80
            '
            'tmrDrag
            '
            Me.tmrDrag.Interval = 80
            '
            'btnFnDel
            '
            Me.btnFnDel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnFnDel.BackColor = System.Drawing.Color.Transparent
            Me.btnFnDel.Cursor = System.Windows.Forms.Cursors.Hand
            Me.btnFnDel.FlatAppearance.BorderSize = 0
            Me.btnFnDel.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer))
            Me.btnFnDel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent
            Me.btnFnDel.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnFnDel.Font = New System.Drawing.Font(OpenSansLight, 11.0!)
            Me.btnFnDel.ForeColor = System.Drawing.Color.Salmon
            Me.btnFnDel.Location = New System.Drawing.Point(171, 270)
            Me.btnFnDel.Margin = New System.Windows.Forms.Padding(2)
            Me.btnFnDel.Name = "btnFnDel"
            Me.btnFnDel.Size = New System.Drawing.Size(55, 47)
            Me.btnFnDel.TabIndex = 11
            Me.btnFnDel.TabStop = False
            Me.btnFnDel.Text = "✗"
            Me.tt.SetToolTip(Me.btnFnDel, "Delete Function")
            Me.btnFnDel.UseVisualStyleBackColor = False
            '
            'pnlFnType
            '
            Me.pnlFnType.BackColor = System.Drawing.Color.FromArgb(CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer))
            Me.pnlFnType.Controls.Add(Me.pnlFnTypeSelector)
            Me.pnlFnType.Controls.Add(Me.lbFnType)
            Me.pnlFnType.Controls.Add(Me.btnFnDel)
            Me.pnlFnType.Location = New System.Drawing.Point(30, 232)
            Me.pnlFnType.Name = "pnlFnType"
            Me.pnlFnType.Size = New System.Drawing.Size(226, 315)
            Me.pnlFnType.TabIndex = 8
            Me.pnlFnType.Visible = False
            '
            'pnlFnTypeSelector
            '
            Me.pnlFnTypeSelector.BackColor = System.Drawing.Color.FromArgb(CType(CType(60, Byte), Integer), CType(CType(60, Byte), Integer), CType(CType(60, Byte), Integer))
            Me.pnlFnTypeSelector.Controls.Add(Me.pnlOptInverse)
            Me.pnlFnTypeSelector.Controls.Add(Me.pnlOptOriginRay)
            Me.pnlFnTypeSelector.Controls.Add(Me.pnlOptPolar)
            Me.pnlFnTypeSelector.Controls.Add(Me.pnlOptParametric)
            Me.pnlFnTypeSelector.Controls.Add(Me.pnlOptCartesian)
            Me.pnlFnTypeSelector.Controls.Add(Me.pnlOptDifferential)
            Me.pnlFnTypeSelector.Dock = System.Windows.Forms.DockStyle.Top
            Me.pnlFnTypeSelector.Location = New System.Drawing.Point(0, 0)
            Me.pnlFnTypeSelector.Name = "pnlFnTypeSelector"
            Me.pnlFnTypeSelector.Size = New System.Drawing.Size(226, 270)
            Me.pnlFnTypeSelector.TabIndex = 8
            '
            'pnlOptInverse
            '
            Me.pnlOptInverse.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.pnlOptInverse.Controls.Add(Me.lbOptInverseR)
            Me.pnlOptInverse.Controls.Add(Me.lbOptInverseL)
            Me.pnlOptInverse.Cursor = System.Windows.Forms.Cursors.Hand
            Me.pnlOptInverse.Location = New System.Drawing.Point(0, 45)
            Me.pnlOptInverse.Name = "pnlOptInverse"
            Me.pnlOptInverse.Size = New System.Drawing.Size(226, 45)
            Me.pnlOptInverse.TabIndex = 15
            Me.pnlOptInverse.Tag = "2"
            '
            'lbOptInverseR
            '
            Me.lbOptInverseR.AutoSize = True
            Me.lbOptInverseR.BackColor = System.Drawing.Color.Transparent
            Me.lbOptInverseR.Font = New System.Drawing.Font(OpenSansLight, 14.0!)
            Me.lbOptInverseR.Location = New System.Drawing.Point(98, 9)
            Me.lbOptInverseR.Name = "lbOptInverseR"
            Me.lbOptInverseR.Size = New System.Drawing.Size(118, 26)
            Me.lbOptInverseR.TabIndex = 11
            Me.lbOptInverseR.Tag = "2"
            Me.lbOptInverseR.Text = "Inverse Cart."
            '
            'lbOptInverseL
            '
            Me.lbOptInverseL.AutoSize = True
            Me.lbOptInverseL.BackColor = System.Drawing.Color.Transparent
            Me.lbOptInverseL.Font = New System.Drawing.Font(OpenSans, 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.lbOptInverseL.Location = New System.Drawing.Point(7, 7)
            Me.lbOptInverseL.Name = "lbOptInverseL"
            Me.lbOptInverseL.Size = New System.Drawing.Size(75, 28)
            Me.lbOptInverseL.TabIndex = 10
            Me.lbOptInverseL.Tag = "2"
            Me.lbOptInverseL.Text = "x = f(y)"
            '
            'pnlOptOriginRay
            '
            Me.pnlOptOriginRay.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.pnlOptOriginRay.Controls.Add(Me.lbOptOriginRayR)
            Me.pnlOptOriginRay.Controls.Add(Me.lbOptOriginRayL)
            Me.pnlOptOriginRay.Cursor = System.Windows.Forms.Cursors.Hand
            Me.pnlOptOriginRay.Location = New System.Drawing.Point(0, 225)
            Me.pnlOptOriginRay.Name = "pnlOptOriginRay"
            Me.pnlOptOriginRay.Size = New System.Drawing.Size(226, 45)
            Me.pnlOptOriginRay.TabIndex = 14
            Me.pnlOptOriginRay.Tag = "6"
            '
            'lbOptOriginRayR
            '
            Me.lbOptOriginRayR.AutoSize = True
            Me.lbOptOriginRayR.BackColor = System.Drawing.Color.Transparent
            Me.lbOptOriginRayR.Font = New System.Drawing.Font(OpenSansLight, 14.0!)
            Me.lbOptOriginRayR.Location = New System.Drawing.Point(107, 9)
            Me.lbOptOriginRayR.Name = "lbOptOriginRayR"
            Me.lbOptOriginRayR.Size = New System.Drawing.Size(104, 26)
            Me.lbOptOriginRayR.TabIndex = 14
            Me.lbOptOriginRayR.Tag = "6"
            Me.lbOptOriginRayR.Text = "Ray / Angle"
            '
            'lbOptOriginRayL
            '
            Me.lbOptOriginRayL.AutoSize = True
            Me.lbOptOriginRayL.BackColor = System.Drawing.Color.Transparent
            Me.lbOptOriginRayL.Font = New System.Drawing.Font(OpenSans, 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.lbOptOriginRayL.Location = New System.Drawing.Point(6, 7)
            Me.lbOptOriginRayL.Name = "lbOptOriginRayL"
            Me.lbOptOriginRayL.Size = New System.Drawing.Size(28, 28)
            Me.lbOptOriginRayL.TabIndex = 11
            Me.lbOptOriginRayL.Tag = "6"
            Me.lbOptOriginRayL.Text = "ϴ"
            '
            'pnlOptPolar
            '
            Me.pnlOptPolar.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.pnlOptPolar.Controls.Add(Me.lbOptPolarR)
            Me.pnlOptPolar.Controls.Add(Me.lbOptPolarL)
            Me.pnlOptPolar.Cursor = System.Windows.Forms.Cursors.Hand
            Me.pnlOptPolar.Location = New System.Drawing.Point(0, 135)
            Me.pnlOptPolar.Name = "pnlOptPolar"
            Me.pnlOptPolar.Size = New System.Drawing.Size(226, 45)
            Me.pnlOptPolar.TabIndex = 10
            Me.pnlOptPolar.Tag = "4"
            '
            'lbOptPolarR
            '
            Me.lbOptPolarR.AutoSize = True
            Me.lbOptPolarR.BackColor = System.Drawing.Color.Transparent
            Me.lbOptPolarR.Font = New System.Drawing.Font(OpenSansLight, 14.0!)
            Me.lbOptPolarR.Location = New System.Drawing.Point(164, 9)
            Me.lbOptPolarR.Name = "lbOptPolarR"
            Me.lbOptPolarR.Size = New System.Drawing.Size(55, 26)
            Me.lbOptPolarR.TabIndex = 11
            Me.lbOptPolarR.Tag = "4"
            Me.lbOptPolarR.Text = "Polar"
            '
            'lbOptPolarL
            '
            Me.lbOptPolarL.AutoSize = True
            Me.lbOptPolarL.BackColor = System.Drawing.Color.Transparent
            Me.lbOptPolarL.Font = New System.Drawing.Font(OpenSans, 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.lbOptPolarL.Location = New System.Drawing.Point(7, 7)
            Me.lbOptPolarL.Name = "lbOptPolarL"
            Me.lbOptPolarL.Size = New System.Drawing.Size(40, 28)
            Me.lbOptPolarL.TabIndex = 10
            Me.lbOptPolarL.Tag = "4"
            Me.lbOptPolarL.Text = "r(t)"
            '
            'pnlOptParametric
            '
            Me.pnlOptParametric.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.pnlOptParametric.Controls.Add(Me.lbOptParametricR)
            Me.pnlOptParametric.Controls.Add(Me.lbOptParametricL)
            Me.pnlOptParametric.Cursor = System.Windows.Forms.Cursors.Hand
            Me.pnlOptParametric.Location = New System.Drawing.Point(0, 90)
            Me.pnlOptParametric.Name = "pnlOptParametric"
            Me.pnlOptParametric.Size = New System.Drawing.Size(226, 45)
            Me.pnlOptParametric.TabIndex = 9
            Me.pnlOptParametric.Tag = "3"
            '
            'lbOptParametricR
            '
            Me.lbOptParametricR.AutoSize = True
            Me.lbOptParametricR.BackColor = System.Drawing.Color.Transparent
            Me.lbOptParametricR.Font = New System.Drawing.Font(OpenSansLight, 14.0!)
            Me.lbOptParametricR.Location = New System.Drawing.Point(113, 9)
            Me.lbOptParametricR.Name = "lbOptParametricR"
            Me.lbOptParametricR.Size = New System.Drawing.Size(103, 26)
            Me.lbOptParametricR.TabIndex = 10
            Me.lbOptParametricR.Tag = "3"
            Me.lbOptParametricR.Text = "Parametric"
            '
            'lbOptParametricL
            '
            Me.lbOptParametricL.AutoSize = True
            Me.lbOptParametricL.BackColor = System.Drawing.Color.Transparent
            Me.lbOptParametricL.Font = New System.Drawing.Font(OpenSans, 13.75!)
            Me.lbOptParametricL.Location = New System.Drawing.Point(5, 10)
            Me.lbOptParametricL.Name = "lbOptParametricL"
            Me.lbOptParametricL.Size = New System.Drawing.Size(102, 26)
            Me.lbOptParametricL.TabIndex = 9
            Me.lbOptParametricL.Tag = "3"
            Me.lbOptParametricL.Text = "<x(t), y(t)>"
            '
            'pnlOptCartesian
            '
            Me.pnlOptCartesian.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.pnlOptCartesian.BackColor = System.Drawing.Color.FromArgb(CType(CType(160, Byte), Integer), CType(CType(70, Byte), Integer), CType(CType(10, Byte), Integer))
            Me.pnlOptCartesian.Controls.Add(Me.lbOptCartesianR)
            Me.pnlOptCartesian.Controls.Add(Me.lbOptCartesianL)
            Me.pnlOptCartesian.Cursor = System.Windows.Forms.Cursors.Arrow
            Me.pnlOptCartesian.Location = New System.Drawing.Point(0, 0)
            Me.pnlOptCartesian.Name = "pnlOptCartesian"
            Me.pnlOptCartesian.Size = New System.Drawing.Size(226, 45)
            Me.pnlOptCartesian.TabIndex = 8
            Me.pnlOptCartesian.Tag = "1"
            '
            'lbOptCartesianR
            '
            Me.lbOptCartesianR.AutoSize = True
            Me.lbOptCartesianR.BackColor = System.Drawing.Color.Transparent
            Me.lbOptCartesianR.Font = New System.Drawing.Font(OpenSansLight, 14.0!)
            Me.lbOptCartesianR.Location = New System.Drawing.Point(124, 9)
            Me.lbOptCartesianR.Name = "lbOptCartesianR"
            Me.lbOptCartesianR.Size = New System.Drawing.Size(91, 26)
            Me.lbOptCartesianR.TabIndex = 9
            Me.lbOptCartesianR.Tag = "1"
            Me.lbOptCartesianR.Text = "Cartesian"
            '
            'lbOptCartesianL
            '
            Me.lbOptCartesianL.AutoSize = True
            Me.lbOptCartesianL.BackColor = System.Drawing.Color.Transparent
            Me.lbOptCartesianL.Font = New System.Drawing.Font(OpenSans, 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.lbOptCartesianL.Location = New System.Drawing.Point(7, 7)
            Me.lbOptCartesianL.Name = "lbOptCartesianL"
            Me.lbOptCartesianL.Size = New System.Drawing.Size(75, 28)
            Me.lbOptCartesianL.TabIndex = 8
            Me.lbOptCartesianL.Tag = "1"
            Me.lbOptCartesianL.Text = "y = f(x)"
            '
            'pnlOptDifferential
            '
            Me.pnlOptDifferential.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.pnlOptDifferential.Controls.Add(Me.lbOptDifferentialR)
            Me.pnlOptDifferential.Controls.Add(Me.lbOptDifferentialL)
            Me.pnlOptDifferential.Cursor = System.Windows.Forms.Cursors.Hand
            Me.pnlOptDifferential.Location = New System.Drawing.Point(0, 180)
            Me.pnlOptDifferential.Name = "pnlOptDifferential"
            Me.pnlOptDifferential.Size = New System.Drawing.Size(226, 45)
            Me.pnlOptDifferential.TabIndex = 11
            Me.pnlOptDifferential.Tag = "5"
            '
            'lbOptDifferentialR
            '
            Me.lbOptDifferentialR.AutoSize = True
            Me.lbOptDifferentialR.BackColor = System.Drawing.Color.Transparent
            Me.lbOptDifferentialR.Font = New System.Drawing.Font(OpenSansLight, 14.0!)
            Me.lbOptDifferentialR.Location = New System.Drawing.Point(113, 10)
            Me.lbOptDifferentialR.Name = "lbOptDifferentialR"
            Me.lbOptDifferentialR.Size = New System.Drawing.Size(102, 26)
            Me.lbOptDifferentialR.TabIndex = 12
            Me.lbOptDifferentialR.Tag = "5"
            Me.lbOptDifferentialR.Text = "Differential"
            '
            'lbOptDifferentialL
            '
            Me.lbOptDifferentialL.AutoSize = True
            Me.lbOptDifferentialL.BackColor = System.Drawing.Color.Transparent
            Me.lbOptDifferentialL.Font = New System.Drawing.Font(OpenSans, 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.lbOptDifferentialL.Location = New System.Drawing.Point(6, 8)
            Me.lbOptDifferentialL.Name = "lbOptDifferentialL"
            Me.lbOptDifferentialL.Size = New System.Drawing.Size(73, 28)
            Me.lbOptDifferentialL.TabIndex = 11
            Me.lbOptDifferentialL.Tag = "5"
            Me.lbOptDifferentialL.Text = "dy/dx "
            '
            'lbFnType
            '
            Me.lbFnType.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
            Me.lbFnType.AutoSize = True
            Me.lbFnType.Location = New System.Drawing.Point(8, 284)
            Me.lbFnType.Name = "lbFnType"
            Me.lbFnType.Size = New System.Drawing.Size(117, 19)
            Me.lbFnType.TabIndex = 8
            Me.lbFnType.Text = "Function Options"
            '
            'tmrDelayRedraw
            '
            '
            'DrawWorker
            '
            Me.DrawWorker.WorkerReportsProgress = True
            '
            'TmrStart
            '
            Me.TmrStart.Interval = 20
            '
            'TmrHighQuality
            '
            Me.TmrHighQuality.Interval = 600
            '
            'GraphingSystem
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
            Me.AutoSize = True
            Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer))
            Me.Controls.Add(Me.pnlFnType)
            Me.Controls.Add(Me.split)
            Me.Font = New System.Drawing.Font(OpenSansLight, 10.0!)
            Me.ForeColor = System.Drawing.Color.White
            Me.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
            Me.Name = "GraphingSystem"
            Me.Size = New System.Drawing.Size(984, 661)
            Me.split.Panel1.ResumeLayout(False)
            Me.split.Panel2.ResumeLayout(False)
            CType(Me.split, System.ComponentModel.ISupportInitialize).EndInit()
            Me.split.ResumeLayout(False)
            Me.pnlWindow.ResumeLayout(False)
            Me.pnlWindow.PerformLayout()
            Me.pnlWHeader.ResumeLayout(False)
            Me.pnlWHeader.PerformLayout()
            Me.pnlTrace.ResumeLayout(False)
            Me.pnlTrace.PerformLayout()
            CType(Me.canvas, System.ComponentModel.ISupportInitialize).EndInit()
            Me.pnlInput.ResumeLayout(False)
            Me.pnlInput.PerformLayout()
            Me.pnlFnType.ResumeLayout(False)
            Me.pnlFnType.PerformLayout()
            Me.pnlFnTypeSelector.ResumeLayout(False)
            Me.pnlOptInverse.ResumeLayout(False)
            Me.pnlOptInverse.PerformLayout()
            Me.pnlOptOriginRay.ResumeLayout(False)
            Me.pnlOptOriginRay.PerformLayout()
            Me.pnlOptPolar.ResumeLayout(False)
            Me.pnlOptPolar.PerformLayout()
            Me.pnlOptParametric.ResumeLayout(False)
            Me.pnlOptParametric.PerformLayout()
            Me.pnlOptCartesian.ResumeLayout(False)
            Me.pnlOptCartesian.PerformLayout()
            Me.pnlOptDifferential.ResumeLayout(False)
            Me.pnlOptDifferential.PerformLayout()
            Me.ResumeLayout(False)

        End Sub

        Friend WithEvents split As SplitContainer
        Friend WithEvents btnAdd As Button
        Friend WithEvents btnGraph As Button
        Friend WithEvents tb As TextBox
        Friend WithEvents pnlInput As Panel
        Friend WithEvents lbFx As Label
        Friend WithEvents canvas As PictureBox
        Friend WithEvents btnTrace As Button
        Friend WithEvents lbTVal As Label
        Friend WithEvents npdTVal As TextBox
        Friend WithEvents tmrTraceUpdate As Timer
        Friend WithEvents btnTNext As Button
        Friend WithEvents btnScale As Button
        Friend WithEvents tmrDrag As Timer
        Friend WithEvents tt As ToolTip
        Friend WithEvents btnNextFn As Button
        Friend WithEvents btnPrevFn As Button
        Friend WithEvents pnlFnType As Panel
        Friend WithEvents pnlFnTypeSelector As Panel
        Friend WithEvents lbFnType As Label
        Friend WithEvents btnFnDel As Button
        Friend WithEvents pnlOptPolar As Panel
        Friend WithEvents pnlOptParametric As Panel
        Friend WithEvents pnlOptCartesian As Panel
        Friend WithEvents lbOptPolarR As Label
        Friend WithEvents lbOptPolarL As Label
        Friend WithEvents lbOptParametricR As Label
        Friend WithEvents lbOptParametricL As Label
        Friend WithEvents lbOptCartesianR As Label
        Friend WithEvents lbOptCartesianL As Label
        Friend WithEvents pnlOptDifferential As Panel
        Friend WithEvents lbOptDifferentialR As Label
        Friend WithEvents lbOptDifferentialL As Label
        Friend WithEvents pnlWindow As Panel
        Friend WithEvents lbWLft As Label
        Friend WithEvents tbWLft As TextBox
        Friend WithEvents lbWRht As Label
        Friend WithEvents tbWRht As TextBox
        Friend WithEvents lbWBot As Label
        Friend WithEvents tbWBot As TextBox
        Friend WithEvents lbWTop As Label
        Friend WithEvents tbWTop As TextBox
        Friend WithEvents lbWindow As Label
        Friend WithEvents btnWOK As Button
        Friend WithEvents btnWCancel As Button
        Friend WithEvents pnlWHeader As Panel
        Friend WithEvents btnWClose As Button
        Friend WithEvents lbWLogo As Label
        Friend WithEvents pnlTrace As Panel
        Friend WithEvents pnlOptOriginRay As Panel
        Friend WithEvents lbOptOriginRayL As Label
        Friend WithEvents lbOptOriginRayR As Label
        Friend WithEvents pnlOptInverse As Panel
        Friend WithEvents lbOptInverseR As Label
        Friend WithEvents lbOptInverseL As Label
        Friend WithEvents lbTrace As Label
        Friend WithEvents tmrDelayRedraw As Timer
        Friend WithEvents DrawWorker As System.ComponentModel.BackgroundWorker
        Friend WithEvents TmrStart As Timer
        Friend WithEvents TmrHighQuality As Timer
    End Class
End Namespace
