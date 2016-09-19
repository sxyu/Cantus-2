Namespace UI.Graphing
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class GraphingSystem
        Inherits System.Windows.Forms.UserControl

        'Form overrides dispose to clean up the component list.
        <System.Diagnostics.DebuggerNonUserCode()>
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            Try
                My.Settings.GraphSplitter = Me.Height - Split.SplitterDistance
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
            Me.Split = New System.Windows.Forms.SplitContainer()
            Me.BtnScale = New System.Windows.Forms.Button()
            Me.PnlWindow = New System.Windows.Forms.Panel()
            Me.TbWBot = New System.Windows.Forms.TextBox()
            Me.TbWLft = New System.Windows.Forms.TextBox()
            Me.TbWTop = New System.Windows.Forms.TextBox()
            Me.TbWRht = New System.Windows.Forms.TextBox()
            Me.BtnWOK = New System.Windows.Forms.Button()
            Me.BtnWCancel = New System.Windows.Forms.Button()
            Me.LbWLft = New System.Windows.Forms.Label()
            Me.LbWRht = New System.Windows.Forms.Label()
            Me.LbWBot = New System.Windows.Forms.Label()
            Me.LbWTop = New System.Windows.Forms.Label()
            Me.PnlWHeader = New System.Windows.Forms.Panel()
            Me.LbWLogo = New System.Windows.Forms.Label()
            Me.BtnWClose = New System.Windows.Forms.Button()
            Me.LbWindow = New System.Windows.Forms.Label()
            Me.PnlTrace = New System.Windows.Forms.Panel()
            Me.LbTrace = New System.Windows.Forms.Label()
            Me.LbTVal = New System.Windows.Forms.Label()
            Me.NpdTVal = New System.Windows.Forms.TextBox()
            Me.BtnTNext = New System.Windows.Forms.Button()
            Me.BtnTrace = New System.Windows.Forms.Button()
            Me.Canvas = New System.Windows.Forms.PictureBox()
            Me.BtnGraph = New System.Windows.Forms.Button()
            Me.BtnNextFn = New System.Windows.Forms.Button()
            Me.BtnPrevFn = New System.Windows.Forms.Button()
            Me.BtnAdd = New System.Windows.Forms.Button()
            Me.PnlInput = New System.Windows.Forms.Panel()
            Me.Tb = New System.Windows.Forms.TextBox()
            Me.LbFx = New System.Windows.Forms.Label()
            Me.TmrTraceUpdate = New System.Windows.Forms.Timer(Me.components)
            Me.TmrDrag = New System.Windows.Forms.Timer(Me.components)
            Me.tt = New System.Windows.Forms.ToolTip(Me.components)
            Me.BtnFnDel = New System.Windows.Forms.Button()
            Me.PnlFnType = New System.Windows.Forms.Panel()
            Me.PnlFnTypeSelector = New System.Windows.Forms.Panel()
            Me.PnlOptInverse = New System.Windows.Forms.Panel()
            Me.LbOptInverseR = New System.Windows.Forms.Label()
            Me.LbOptInverseL = New System.Windows.Forms.Label()
            Me.PnlOptOriginRay = New System.Windows.Forms.Panel()
            Me.LbOptOriginRayR = New System.Windows.Forms.Label()
            Me.LbOptOriginRayL = New System.Windows.Forms.Label()
            Me.PnlOptPolar = New System.Windows.Forms.Panel()
            Me.LbOptPolarR = New System.Windows.Forms.Label()
            Me.LbOptPolarL = New System.Windows.Forms.Label()
            Me.PnlOptParametric = New System.Windows.Forms.Panel()
            Me.LbOptParametricR = New System.Windows.Forms.Label()
            Me.LbOptParametricL = New System.Windows.Forms.Label()
            Me.PnlOptCartesian = New System.Windows.Forms.Panel()
            Me.LbOptCartesianR = New System.Windows.Forms.Label()
            Me.LbOptCartesianL = New System.Windows.Forms.Label()
            Me.PnlOptDifferential = New System.Windows.Forms.Panel()
            Me.LbOptDifferentialR = New System.Windows.Forms.Label()
            Me.LbOptDifferentialL = New System.Windows.Forms.Label()
            Me.LbFnType = New System.Windows.Forms.Label()
            Me.tmrDelayRedraw = New System.Windows.Forms.Timer(Me.components)
            Me.DrawWorker = New System.ComponentModel.BackgroundWorker()
            Me.TmrStart = New System.Windows.Forms.Timer(Me.components)
            Me.TmrHighQuality = New System.Windows.Forms.Timer(Me.components)
            CType(Me.Split, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.Split.Panel1.SuspendLayout()
            Me.Split.Panel2.SuspendLayout()
            Me.Split.SuspendLayout()
            Me.PnlWindow.SuspendLayout()
            Me.PnlWHeader.SuspendLayout()
            Me.PnlTrace.SuspendLayout()
            CType(Me.Canvas, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.PnlInput.SuspendLayout()
            Me.PnlFnType.SuspendLayout()
            Me.PnlFnTypeSelector.SuspendLayout()
            Me.PnlOptInverse.SuspendLayout()
            Me.PnlOptOriginRay.SuspendLayout()
            Me.PnlOptPolar.SuspendLayout()
            Me.PnlOptParametric.SuspendLayout()
            Me.PnlOptCartesian.SuspendLayout()
            Me.PnlOptDifferential.SuspendLayout()
            Me.SuspendLayout()
            '
            'Split
            '
            Me.Split.BackColor = System.Drawing.Color.FromArgb(CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer))
            Me.Split.Dock = System.Windows.Forms.DockStyle.Fill
            Me.Split.FixedPanel = System.Windows.Forms.FixedPanel.Panel2
            Me.Split.Location = New System.Drawing.Point(0, 0)
            Me.Split.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
            Me.Split.Name = "Split"
            Me.Split.Orientation = System.Windows.Forms.Orientation.Horizontal
            '
            'Split.Panel1
            '
            Me.Split.Panel1.Controls.Add(Me.BtnScale)
            Me.Split.Panel1.Controls.Add(Me.PnlWindow)
            Me.Split.Panel1.Controls.Add(Me.PnlTrace)
            Me.Split.Panel1.Controls.Add(Me.BtnTrace)
            Me.Split.Panel1.Controls.Add(Me.Canvas)
            '
            'Split.Panel2
            '
            Me.Split.Panel2.BackColor = System.Drawing.Color.FromArgb(CType(CType(45, Byte), Integer), CType(CType(45, Byte), Integer), CType(CType(45, Byte), Integer))
            Me.Split.Panel2.Controls.Add(Me.BtnGraph)
            Me.Split.Panel2.Controls.Add(Me.BtnNextFn)
            Me.Split.Panel2.Controls.Add(Me.BtnPrevFn)
            Me.Split.Panel2.Controls.Add(Me.BtnAdd)
            Me.Split.Panel2.Controls.Add(Me.PnlInput)
            Me.Split.Panel2.Controls.Add(Me.LbFx)
            Me.Split.Size = New System.Drawing.Size(984, 661)
            Me.Split.SplitterDistance = 601
            Me.Split.SplitterWidth = 3
            Me.Split.TabIndex = 0
            '
            'BtnScale
            '
            Me.BtnScale.BackColor = System.Drawing.Color.DarkSlateBlue
            Me.BtnScale.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
            Me.BtnScale.Cursor = System.Windows.Forms.Cursors.Hand
            Me.BtnScale.FlatAppearance.BorderSize = 0
            Me.BtnScale.FlatAppearance.MouseDownBackColor = System.Drawing.Color.SlateBlue
            Me.BtnScale.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkSlateBlue
            Me.BtnScale.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.BtnScale.Font = New System.Drawing.Font(OpenSansLight, 14.0!)
            Me.BtnScale.ForeColor = System.Drawing.Color.White
            Me.BtnScale.Image = CType(resources.GetObject("BtnScale.Image"), System.Drawing.Image)
            Me.BtnScale.Location = New System.Drawing.Point(11, 11)
            Me.BtnScale.Margin = New System.Windows.Forms.Padding(2)
            Me.BtnScale.Name = "BtnScale"
            Me.BtnScale.Size = New System.Drawing.Size(115, 40)
            Me.BtnScale.TabIndex = 4
            Me.BtnScale.TabStop = False
            Me.BtnScale.TextAlign = System.Drawing.ContentAlignment.MiddleRight
            Me.tt.SetToolTip(Me.BtnScale, "Left click to adjust the range graphed on the window" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Right click to reset to def" &
        "ault (centered) view" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Middle click to go to first quadrant view")
            Me.BtnScale.UseVisualStyleBackColor = False
            '
            'PnlWindow
            '
            Me.PnlWindow.BackColor = System.Drawing.Color.FromArgb(CType(CType(45, Byte), Integer), CType(CType(45, Byte), Integer), CType(CType(45, Byte), Integer))
            Me.PnlWindow.Controls.Add(Me.TbWBot)
            Me.PnlWindow.Controls.Add(Me.TbWLft)
            Me.PnlWindow.Controls.Add(Me.TbWTop)
            Me.PnlWindow.Controls.Add(Me.TbWRht)
            Me.PnlWindow.Controls.Add(Me.BtnWOK)
            Me.PnlWindow.Controls.Add(Me.BtnWCancel)
            Me.PnlWindow.Controls.Add(Me.LbWLft)
            Me.PnlWindow.Controls.Add(Me.LbWRht)
            Me.PnlWindow.Controls.Add(Me.LbWBot)
            Me.PnlWindow.Controls.Add(Me.LbWTop)
            Me.PnlWindow.Controls.Add(Me.PnlWHeader)
            Me.PnlWindow.Location = New System.Drawing.Point(280, 160)
            Me.PnlWindow.Name = "PnlWindow"
            Me.PnlWindow.Size = New System.Drawing.Size(469, 305)
            Me.PnlWindow.TabIndex = 9
            Me.PnlWindow.Visible = False
            '
            'TbWBot
            '
            Me.TbWBot.Anchor = System.Windows.Forms.AnchorStyles.Bottom
            Me.TbWBot.Font = New System.Drawing.Font(OpenSansLight, 15.0!)
            Me.TbWBot.Location = New System.Drawing.Point(156, 212)
            Me.TbWBot.Name = "TbWBot"
            Me.TbWBot.Size = New System.Drawing.Size(156, 35)
            Me.TbWBot.TabIndex = 12
            Me.tt.SetToolTip(Me.TbWBot, "Minimum Y (Expression)")
            '
            'TbWLft
            '
            Me.TbWLft.Anchor = System.Windows.Forms.AnchorStyles.Left
            Me.TbWLft.Font = New System.Drawing.Font(OpenSansLight, 15.0!)
            Me.TbWLft.Location = New System.Drawing.Point(25, 147)
            Me.TbWLft.Name = "TbWLft"
            Me.TbWLft.Size = New System.Drawing.Size(156, 35)
            Me.TbWLft.TabIndex = 14
            Me.tt.SetToolTip(Me.TbWLft, "Minimum X (Expression)")
            '
            'TbWTop
            '
            Me.TbWTop.Anchor = System.Windows.Forms.AnchorStyles.Top
            Me.TbWTop.Font = New System.Drawing.Font(OpenSansLight, 15.0!)
            Me.TbWTop.Location = New System.Drawing.Point(156, 85)
            Me.TbWTop.Name = "TbWTop"
            Me.TbWTop.Size = New System.Drawing.Size(156, 35)
            Me.TbWTop.TabIndex = 10
            Me.tt.SetToolTip(Me.TbWTop, "Maximum Y (Expression)")
            '
            'TbWRht
            '
            Me.TbWRht.Anchor = System.Windows.Forms.AnchorStyles.Right
            Me.TbWRht.Font = New System.Drawing.Font(OpenSansLight, 15.0!)
            Me.TbWRht.Location = New System.Drawing.Point(289, 147)
            Me.TbWRht.Name = "TbWRht"
            Me.TbWRht.Size = New System.Drawing.Size(156, 35)
            Me.TbWRht.TabIndex = 16
            Me.tt.SetToolTip(Me.TbWRht, "Maximum X (Expression)")
            '
            'BtnWOK
            '
            Me.BtnWOK.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.BtnWOK.BackColor = System.Drawing.Color.FromArgb(CType(CType(160, Byte), Integer), CType(CType(70, Byte), Integer), CType(CType(10, Byte), Integer))
            Me.BtnWOK.Cursor = System.Windows.Forms.Cursors.Hand
            Me.BtnWOK.FlatAppearance.BorderSize = 0
            Me.BtnWOK.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(210, Byte), Integer), CType(CType(80, Byte), Integer), CType(CType(20, Byte), Integer))
            Me.BtnWOK.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(160, Byte), Integer), CType(CType(70, Byte), Integer), CType(CType(10, Byte), Integer))
            Me.BtnWOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.BtnWOK.Font = New System.Drawing.Font(OpenSans, 11.0!)
            Me.BtnWOK.ForeColor = System.Drawing.Color.White
            Me.BtnWOK.Location = New System.Drawing.Point(368, 256)
            Me.BtnWOK.Margin = New System.Windows.Forms.Padding(2)
            Me.BtnWOK.Name = "BtnWOK"
            Me.BtnWOK.Size = New System.Drawing.Size(99, 47)
            Me.BtnWOK.TabIndex = 20
            Me.BtnWOK.TabStop = False
            Me.BtnWOK.Text = "Update"
            Me.tt.SetToolTip(Me.BtnWOK, "Update the Window Bounds (Enter)")
            Me.BtnWOK.UseVisualStyleBackColor = False
            '
            'BtnWCancel
            '
            Me.BtnWCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.BtnWCancel.BackColor = System.Drawing.Color.Transparent
            Me.BtnWCancel.Cursor = System.Windows.Forms.Cursors.Hand
            Me.BtnWCancel.FlatAppearance.BorderSize = 0
            Me.BtnWCancel.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer))
            Me.BtnWCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent
            Me.BtnWCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.BtnWCancel.Font = New System.Drawing.Font(OpenSansLight, 11.0!)
            Me.BtnWCancel.ForeColor = System.Drawing.Color.White
            Me.BtnWCancel.Location = New System.Drawing.Point(272, 253)
            Me.BtnWCancel.Margin = New System.Windows.Forms.Padding(2)
            Me.BtnWCancel.Name = "BtnWCancel"
            Me.BtnWCancel.Size = New System.Drawing.Size(92, 47)
            Me.BtnWCancel.TabIndex = 18
            Me.BtnWCancel.TabStop = False
            Me.BtnWCancel.Text = "Cancel"
            Me.tt.SetToolTip(Me.BtnWCancel, "Cancel Changes (Esc)")
            Me.BtnWCancel.UseVisualStyleBackColor = False
            '
            'LbWLft
            '
            Me.LbWLft.Anchor = System.Windows.Forms.AnchorStyles.Left
            Me.LbWLft.AutoSize = True
            Me.LbWLft.Font = New System.Drawing.Font(OpenSansLight, 12.0!)
            Me.LbWLft.Location = New System.Drawing.Point(80, 117)
            Me.LbWLft.Name = "LbWLft"
            Me.LbWLft.Size = New System.Drawing.Size(49, 22)
            Me.LbWLft.TabIndex = 17
            Me.LbWLft.Text = "Min x"
            '
            'LbWRht
            '
            Me.LbWRht.Anchor = System.Windows.Forms.AnchorStyles.Right
            Me.LbWRht.AutoSize = True
            Me.LbWRht.Font = New System.Drawing.Font(OpenSansLight, 12.0!)
            Me.LbWRht.Location = New System.Drawing.Point(341, 119)
            Me.LbWRht.Name = "LbWRht"
            Me.LbWRht.Size = New System.Drawing.Size(52, 22)
            Me.LbWRht.TabIndex = 15
            Me.LbWRht.Text = "Max x"
            '
            'LbWBot
            '
            Me.LbWBot.Anchor = System.Windows.Forms.AnchorStyles.Bottom
            Me.LbWBot.AutoSize = True
            Me.LbWBot.Font = New System.Drawing.Font(OpenSansLight, 12.0!)
            Me.LbWBot.Location = New System.Drawing.Point(211, 182)
            Me.LbWBot.Name = "LbWBot"
            Me.LbWBot.Size = New System.Drawing.Size(48, 22)
            Me.LbWBot.TabIndex = 13
            Me.LbWBot.Text = "Min y"
            '
            'LbWTop
            '
            Me.LbWTop.Anchor = System.Windows.Forms.AnchorStyles.Top
            Me.LbWTop.AutoSize = True
            Me.LbWTop.Font = New System.Drawing.Font(OpenSansLight, 12.0!)
            Me.LbWTop.Location = New System.Drawing.Point(211, 56)
            Me.LbWTop.Name = "LbWTop"
            Me.LbWTop.Size = New System.Drawing.Size(51, 22)
            Me.LbWTop.TabIndex = 11
            Me.LbWTop.Text = "Max y"
            '
            'PnlWHeader
            '
            Me.PnlWHeader.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.PnlWHeader.BackColor = System.Drawing.Color.FromArgb(CType(CType(35, Byte), Integer), CType(CType(35, Byte), Integer), CType(CType(35, Byte), Integer))
            Me.PnlWHeader.Controls.Add(Me.LbWLogo)
            Me.PnlWHeader.Controls.Add(Me.BtnWClose)
            Me.PnlWHeader.Controls.Add(Me.LbWindow)
            Me.PnlWHeader.Location = New System.Drawing.Point(0, 0)
            Me.PnlWHeader.Name = "PnlWHeader"
            Me.PnlWHeader.Size = New System.Drawing.Size(469, 47)
            Me.PnlWHeader.TabIndex = 19
            '
            'LbWLogo
            '
            Me.LbWLogo.Font = New System.Drawing.Font(OpenSansLight, 16.0!)
            Me.LbWLogo.Image = CType(resources.GetObject("LbWLogo.Image"), System.Drawing.Image)
            Me.LbWLogo.Location = New System.Drawing.Point(16, 7)
            Me.LbWLogo.Name = "LbWLogo"
            Me.LbWLogo.Size = New System.Drawing.Size(32, 30)
            Me.LbWLogo.TabIndex = 22
            Me.LbWLogo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            '
            'BtnWClose
            '
            Me.BtnWClose.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.BtnWClose.BackColor = System.Drawing.Color.Transparent
            Me.BtnWClose.Cursor = System.Windows.Forms.Cursors.Hand
            Me.BtnWClose.FlatAppearance.BorderSize = 0
            Me.BtnWClose.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer))
            Me.BtnWClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent
            Me.BtnWClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.BtnWClose.Font = New System.Drawing.Font(OpenSansLight, 9.0!)
            Me.BtnWClose.ForeColor = System.Drawing.Color.White
            Me.BtnWClose.Location = New System.Drawing.Point(423, 2)
            Me.BtnWClose.Margin = New System.Windows.Forms.Padding(2)
            Me.BtnWClose.Name = "BtnWClose"
            Me.BtnWClose.Size = New System.Drawing.Size(44, 43)
            Me.BtnWClose.TabIndex = 21
            Me.BtnWClose.TabStop = False
            Me.BtnWClose.Text = "X"
            Me.tt.SetToolTip(Me.BtnWClose, "Close (Esc)")
            Me.BtnWClose.UseVisualStyleBackColor = False
            '
            'LbWindow
            '
            Me.LbWindow.AutoSize = True
            Me.LbWindow.Font = New System.Drawing.Font(OpenSansLight, 12.0!)
            Me.LbWindow.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
            Me.LbWindow.Location = New System.Drawing.Point(48, 12)
            Me.LbWindow.Margin = New System.Windows.Forms.Padding(0, 0, 3, 0)
            Me.LbWindow.Name = "LbWindow"
            Me.LbWindow.Size = New System.Drawing.Size(193, 22)
            Me.LbWindow.TabIndex = 9
            Me.LbWindow.Text = "Window Bounds + Scaling"
            '
            'PnlTrace
            '
            Me.PnlTrace.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.PnlTrace.BackColor = System.Drawing.Color.FromArgb(CType(CType(60, Byte), Integer), CType(CType(60, Byte), Integer), CType(CType(60, Byte), Integer))
            Me.PnlTrace.Controls.Add(Me.LbTrace)
            Me.PnlTrace.Controls.Add(Me.LbTVal)
            Me.PnlTrace.Controls.Add(Me.NpdTVal)
            Me.PnlTrace.Controls.Add(Me.BtnTNext)
            Me.PnlTrace.Location = New System.Drawing.Point(676, 11)
            Me.PnlTrace.Name = "PnlTrace"
            Me.PnlTrace.Size = New System.Drawing.Size(228, 136)
            Me.PnlTrace.TabIndex = 10
            Me.PnlTrace.Visible = False
            '
            'LbTrace
            '
            Me.LbTrace.AutoSize = True
            Me.LbTrace.Font = New System.Drawing.Font(OpenSansLight, 12.0!)
            Me.LbTrace.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
            Me.LbTrace.Location = New System.Drawing.Point(10, 9)
            Me.LbTrace.Margin = New System.Windows.Forms.Padding(0, 0, 3, 0)
            Me.LbTrace.Name = "LbTrace"
            Me.LbTrace.Size = New System.Drawing.Size(114, 22)
            Me.LbTrace.TabIndex = 10
            Me.LbTrace.Text = "Trace Function"
            '
            'LbTVal
            '
            Me.LbTVal.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.LbTVal.BackColor = System.Drawing.Color.FromArgb(CType(CType(60, Byte), Integer), CType(CType(60, Byte), Integer), CType(CType(60, Byte), Integer))
            Me.LbTVal.Location = New System.Drawing.Point(-1, 74)
            Me.LbTVal.Name = "LbTVal"
            Me.LbTVal.Padding = New System.Windows.Forms.Padding(0, 3, 0, 0)
            Me.LbTVal.Size = New System.Drawing.Size(230, 62)
            Me.LbTVal.TabIndex = 3
            Me.LbTVal.Text = "f(x) = 0"
            Me.LbTVal.TextAlign = System.Drawing.ContentAlignment.TopRight
            '
            'NpdTVal
            '
            Me.NpdTVal.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.NpdTVal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.NpdTVal.Font = New System.Drawing.Font(OpenSansLight, 15.0!)
            Me.NpdTVal.Location = New System.Drawing.Point(0, 41)
            Me.NpdTVal.Multiline = True
            Me.NpdTVal.Name = "NpdTVal"
            Me.NpdTVal.Size = New System.Drawing.Size(156, 46)
            Me.NpdTVal.TabIndex = 4
            Me.tt.SetToolTip(Me.NpdTVal, "Value to Trace At (Expression)")
            '
            'BtnTNext
            '
            Me.BtnTNext.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.BtnTNext.BackColor = System.Drawing.Color.DimGray
            Me.BtnTNext.Cursor = System.Windows.Forms.Cursors.Hand
            Me.BtnTNext.FlatAppearance.BorderSize = 0
            Me.BtnTNext.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DimGray
            Me.BtnTNext.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gray
            Me.BtnTNext.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.BtnTNext.Font = New System.Drawing.Font(OpenSansLight, 7.0!)
            Me.BtnTNext.ForeColor = System.Drawing.Color.White
            Me.BtnTNext.Location = New System.Drawing.Point(156, 41)
            Me.BtnTNext.Margin = New System.Windows.Forms.Padding(2)
            Me.BtnTNext.Name = "BtnTNext"
            Me.BtnTNext.Size = New System.Drawing.Size(75, 34)
            Me.BtnTNext.TabIndex = 5
            Me.BtnTNext.TabStop = False
            Me.BtnTNext.Text = "Find Crit Pts"
            Me.tt.SetToolTip(Me.BtnTNext, "Find zeros, maxima, minima, and intercepts" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "(Enter)")
            Me.BtnTNext.UseVisualStyleBackColor = False
            '
            'BtnTrace
            '
            Me.BtnTrace.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.BtnTrace.BackColor = System.Drawing.Color.FromArgb(CType(CType(160, Byte), Integer), CType(CType(70, Byte), Integer), CType(CType(10, Byte), Integer))
            Me.BtnTrace.Cursor = System.Windows.Forms.Cursors.Hand
            Me.BtnTrace.FlatAppearance.BorderSize = 0
            Me.BtnTrace.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(210, Byte), Integer), CType(CType(80, Byte), Integer), CType(CType(20, Byte), Integer))
            Me.BtnTrace.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(160, Byte), Integer), CType(CType(70, Byte), Integer), CType(CType(10, Byte), Integer))
            Me.BtnTrace.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.BtnTrace.ForeColor = System.Drawing.Color.White
            Me.BtnTrace.Location = New System.Drawing.Point(904, 11)
            Me.BtnTrace.Margin = New System.Windows.Forms.Padding(2)
            Me.BtnTrace.Name = "BtnTrace"
            Me.BtnTrace.Size = New System.Drawing.Size(68, 40)
            Me.BtnTrace.TabIndex = 1
            Me.BtnTrace.TabStop = False
            Me.BtnTrace.Text = "Trace"
            Me.tt.SetToolTip(Me.BtnTrace, "Trace this graph (Alt+T)")
            Me.BtnTrace.UseVisualStyleBackColor = False
            '
            'Canvas
            '
            Me.Canvas.BackColor = System.Drawing.Color.FromArgb(CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer))
            Me.Canvas.Dock = System.Windows.Forms.DockStyle.Fill
            Me.Canvas.Location = New System.Drawing.Point(0, 0)
            Me.Canvas.Name = "Canvas"
            Me.Canvas.Size = New System.Drawing.Size(984, 601)
            Me.Canvas.TabIndex = 0
            Me.Canvas.TabStop = False
            '
            'BtnGraph
            '
            Me.BtnGraph.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.BtnGraph.BackColor = System.Drawing.Color.FromArgb(CType(CType(160, Byte), Integer), CType(CType(70, Byte), Integer), CType(CType(10, Byte), Integer))
            Me.BtnGraph.Cursor = System.Windows.Forms.Cursors.Hand
            Me.BtnGraph.FlatAppearance.BorderSize = 0
            Me.BtnGraph.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(210, Byte), Integer), CType(CType(80, Byte), Integer), CType(CType(20, Byte), Integer))
            Me.BtnGraph.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(160, Byte), Integer), CType(CType(70, Byte), Integer), CType(CType(10, Byte), Integer))
            Me.BtnGraph.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.BtnGraph.ForeColor = System.Drawing.Color.White
            Me.BtnGraph.Location = New System.Drawing.Point(837, -1)
            Me.BtnGraph.Margin = New System.Windows.Forms.Padding(2)
            Me.BtnGraph.Name = "BtnGraph"
            Me.BtnGraph.Size = New System.Drawing.Size(105, 61)
            Me.BtnGraph.TabIndex = 0
            Me.BtnGraph.TabStop = False
            Me.BtnGraph.Text = "Graph"
            Me.tt.SetToolTip(Me.BtnGraph, "Update the graph of this function (Alt+Enter)")
            Me.BtnGraph.UseVisualStyleBackColor = False
            '
            'BtnNextFn
            '
            Me.BtnNextFn.BackColor = System.Drawing.Color.FromArgb(CType(CType(60, Byte), Integer), CType(CType(60, Byte), Integer), CType(CType(60, Byte), Integer))
            Me.BtnNextFn.Cursor = System.Windows.Forms.Cursors.Hand
            Me.BtnNextFn.Enabled = False
            Me.BtnNextFn.FlatAppearance.BorderSize = 0
            Me.BtnNextFn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(35, Byte), Integer), CType(CType(35, Byte), Integer), CType(CType(35, Byte), Integer))
            Me.BtnNextFn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent
            Me.BtnNextFn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.BtnNextFn.Font = New System.Drawing.Font(OpenSansLight, 11.0!)
            Me.BtnNextFn.ForeColor = System.Drawing.Color.Silver
            Me.BtnNextFn.Location = New System.Drawing.Point(0, 30)
            Me.BtnNextFn.Margin = New System.Windows.Forms.Padding(2)
            Me.BtnNextFn.Name = "BtnNextFn"
            Me.BtnNextFn.Size = New System.Drawing.Size(30, 30)
            Me.BtnNextFn.TabIndex = 9
            Me.BtnNextFn.TabStop = False
            Me.BtnNextFn.Text = "▼"
            Me.tt.SetToolTip(Me.BtnNextFn, "Go to next function (Alt+Down)")
            Me.BtnNextFn.UseVisualStyleBackColor = False
            '
            'BtnPrevFn
            '
            Me.BtnPrevFn.BackColor = System.Drawing.Color.FromArgb(CType(CType(60, Byte), Integer), CType(CType(60, Byte), Integer), CType(CType(60, Byte), Integer))
            Me.BtnPrevFn.Cursor = System.Windows.Forms.Cursors.Hand
            Me.BtnPrevFn.Enabled = False
            Me.BtnPrevFn.FlatAppearance.BorderSize = 0
            Me.BtnPrevFn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(35, Byte), Integer), CType(CType(35, Byte), Integer), CType(CType(35, Byte), Integer))
            Me.BtnPrevFn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent
            Me.BtnPrevFn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.BtnPrevFn.Font = New System.Drawing.Font(OpenSansLight, 11.0!)
            Me.BtnPrevFn.ForeColor = System.Drawing.Color.Silver
            Me.BtnPrevFn.Location = New System.Drawing.Point(0, 0)
            Me.BtnPrevFn.Margin = New System.Windows.Forms.Padding(2)
            Me.BtnPrevFn.Name = "BtnPrevFn"
            Me.BtnPrevFn.Size = New System.Drawing.Size(30, 30)
            Me.BtnPrevFn.TabIndex = 8
            Me.BtnPrevFn.TabStop = False
            Me.BtnPrevFn.Text = "▲"
            Me.tt.SetToolTip(Me.BtnPrevFn, "Go to previous function (Alt+Up)")
            Me.BtnPrevFn.UseVisualStyleBackColor = False
            '
            'BtnAdd
            '
            Me.BtnAdd.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.BtnAdd.BackColor = System.Drawing.Color.FromArgb(CType(CType(45, Byte), Integer), CType(CType(45, Byte), Integer), CType(CType(45, Byte), Integer))
            Me.BtnAdd.Cursor = System.Windows.Forms.Cursors.Hand
            Me.BtnAdd.FlatAppearance.BorderSize = 0
            Me.BtnAdd.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(65, Byte), Integer), CType(CType(65, Byte), Integer), CType(CType(65, Byte), Integer))
            Me.BtnAdd.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(45, Byte), Integer), CType(CType(45, Byte), Integer), CType(CType(45, Byte), Integer))
            Me.BtnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.BtnAdd.ForeColor = System.Drawing.Color.White
            Me.BtnAdd.Location = New System.Drawing.Point(941, 1)
            Me.BtnAdd.Margin = New System.Windows.Forms.Padding(2)
            Me.BtnAdd.Name = "BtnAdd"
            Me.BtnAdd.Size = New System.Drawing.Size(43, 57)
            Me.BtnAdd.TabIndex = 2
            Me.BtnAdd.TabStop = False
            Me.BtnAdd.Text = "+"
            Me.tt.SetToolTip(Me.BtnAdd, "Add a new function (Alt+A or Alt++)")
            Me.BtnAdd.UseVisualStyleBackColor = False
            '
            'PnlInput
            '
            Me.PnlInput.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.PnlInput.BackColor = System.Drawing.Color.Gainsboro
            Me.PnlInput.Controls.Add(Me.Tb)
            Me.PnlInput.Location = New System.Drawing.Point(159, -3)
            Me.PnlInput.Name = "PnlInput"
            Me.PnlInput.Size = New System.Drawing.Size(682, 84)
            Me.PnlInput.TabIndex = 1
            '
            'Tb
            '
            Me.Tb.AcceptsTab = True
            Me.Tb.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.Tb.BackColor = System.Drawing.Color.Gainsboro
            Me.Tb.BorderStyle = System.Windows.Forms.BorderStyle.None
            Me.Tb.Cursor = System.Windows.Forms.Cursors.Arrow
            Me.Tb.Font = New System.Drawing.Font(OpenSansLight, 14.0!)
            Me.Tb.ForeColor = System.Drawing.Color.FromArgb(CType(CType(20, Byte), Integer), CType(CType(20, Byte), Integer), CType(CType(20, Byte), Integer))
            Me.Tb.HideSelection = False
            Me.Tb.Location = New System.Drawing.Point(7, 8)
            Me.Tb.Multiline = True
            Me.Tb.Name = "Tb"
            Me.Tb.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal
            Me.Tb.Size = New System.Drawing.Size(673, 53)
            Me.Tb.TabIndex = 0
            '
            'LbFx
            '
            Me.LbFx.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
            Me.LbFx.AutoEllipsis = True
            Me.LbFx.BackColor = System.Drawing.Color.FromArgb(CType(CType(45, Byte), Integer), CType(CType(45, Byte), Integer), CType(CType(45, Byte), Integer))
            Me.LbFx.Cursor = System.Windows.Forms.Cursors.Hand
            Me.LbFx.Font = New System.Drawing.Font(OpenSansLight, 14.0!)
            Me.LbFx.Location = New System.Drawing.Point(32, 0)
            Me.LbFx.Name = "LbFx"
            Me.LbFx.Size = New System.Drawing.Size(123, 60)
            Me.LbFx.TabIndex = 3
            Me.LbFx.Text = "f(x) = "
            Me.LbFx.TextAlign = System.Drawing.ContentAlignment.MiddleRight
            Me.tt.SetToolTip(Me.LbFx, "Click to change function type")
            '
            'TmrTraceUpdate
            '
            Me.TmrTraceUpdate.Interval = 80
            '
            'TmrDrag
            '
            Me.TmrDrag.Interval = 80
            '
            'BtnFnDel
            '
            Me.BtnFnDel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.BtnFnDel.BackColor = System.Drawing.Color.Transparent
            Me.BtnFnDel.Cursor = System.Windows.Forms.Cursors.Hand
            Me.BtnFnDel.FlatAppearance.BorderSize = 0
            Me.BtnFnDel.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer))
            Me.BtnFnDel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent
            Me.BtnFnDel.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.BtnFnDel.Font = New System.Drawing.Font(OpenSansLight, 11.0!)
            Me.BtnFnDel.ForeColor = System.Drawing.Color.Salmon
            Me.BtnFnDel.Location = New System.Drawing.Point(171, 270)
            Me.BtnFnDel.Margin = New System.Windows.Forms.Padding(2)
            Me.BtnFnDel.Name = "BtnFnDel"
            Me.BtnFnDel.Size = New System.Drawing.Size(55, 47)
            Me.BtnFnDel.TabIndex = 11
            Me.BtnFnDel.TabStop = False
            Me.BtnFnDel.Text = "✗"
            Me.tt.SetToolTip(Me.BtnFnDel, "Delete Function")
            Me.BtnFnDel.UseVisualStyleBackColor = False
            '
            'PnlFnType
            '
            Me.PnlFnType.BackColor = System.Drawing.Color.FromArgb(CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer))
            Me.PnlFnType.Controls.Add(Me.PnlFnTypeSelector)
            Me.PnlFnType.Controls.Add(Me.LbFnType)
            Me.PnlFnType.Controls.Add(Me.BtnFnDel)
            Me.PnlFnType.Location = New System.Drawing.Point(30, 232)
            Me.PnlFnType.Name = "PnlFnType"
            Me.PnlFnType.Size = New System.Drawing.Size(226, 315)
            Me.PnlFnType.TabIndex = 8
            Me.PnlFnType.Visible = False
            '
            'PnlFnTypeSelector
            '
            Me.PnlFnTypeSelector.BackColor = System.Drawing.Color.FromArgb(CType(CType(60, Byte), Integer), CType(CType(60, Byte), Integer), CType(CType(60, Byte), Integer))
            Me.PnlFnTypeSelector.Controls.Add(Me.PnlOptInverse)
            Me.PnlFnTypeSelector.Controls.Add(Me.PnlOptOriginRay)
            Me.PnlFnTypeSelector.Controls.Add(Me.PnlOptPolar)
            Me.PnlFnTypeSelector.Controls.Add(Me.PnlOptParametric)
            Me.PnlFnTypeSelector.Controls.Add(Me.PnlOptCartesian)
            Me.PnlFnTypeSelector.Controls.Add(Me.PnlOptDifferential)
            Me.PnlFnTypeSelector.Dock = System.Windows.Forms.DockStyle.Top
            Me.PnlFnTypeSelector.Location = New System.Drawing.Point(0, 0)
            Me.PnlFnTypeSelector.Name = "PnlFnTypeSelector"
            Me.PnlFnTypeSelector.Size = New System.Drawing.Size(226, 270)
            Me.PnlFnTypeSelector.TabIndex = 8
            '
            'PnlOptInverse
            '
            Me.PnlOptInverse.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.PnlOptInverse.Controls.Add(Me.LbOptInverseR)
            Me.PnlOptInverse.Controls.Add(Me.LbOptInverseL)
            Me.PnlOptInverse.Cursor = System.Windows.Forms.Cursors.Hand
            Me.PnlOptInverse.Location = New System.Drawing.Point(0, 45)
            Me.PnlOptInverse.Name = "PnlOptInverse"
            Me.PnlOptInverse.Size = New System.Drawing.Size(226, 45)
            Me.PnlOptInverse.TabIndex = 15
            Me.PnlOptInverse.Tag = "2"
            '
            'LbOptInverseR
            '
            Me.LbOptInverseR.AutoSize = True
            Me.LbOptInverseR.BackColor = System.Drawing.Color.Transparent
            Me.LbOptInverseR.Font = New System.Drawing.Font(OpenSansLight, 14.0!)
            Me.LbOptInverseR.Location = New System.Drawing.Point(98, 9)
            Me.LbOptInverseR.Name = "LbOptInverseR"
            Me.LbOptInverseR.Size = New System.Drawing.Size(118, 26)
            Me.LbOptInverseR.TabIndex = 11
            Me.LbOptInverseR.Tag = "2"
            Me.LbOptInverseR.Text = "Inverse Cart."
            '
            'LbOptInverseL
            '
            Me.LbOptInverseL.AutoSize = True
            Me.LbOptInverseL.BackColor = System.Drawing.Color.Transparent
            Me.LbOptInverseL.Font = New System.Drawing.Font(OpenSans, 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.LbOptInverseL.Location = New System.Drawing.Point(7, 7)
            Me.LbOptInverseL.Name = "LbOptInverseL"
            Me.LbOptInverseL.Size = New System.Drawing.Size(75, 28)
            Me.LbOptInverseL.TabIndex = 10
            Me.LbOptInverseL.Tag = "2"
            Me.LbOptInverseL.Text = "x = f(y)"
            '
            'PnlOptOriginRay
            '
            Me.PnlOptOriginRay.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.PnlOptOriginRay.Controls.Add(Me.LbOptOriginRayR)
            Me.PnlOptOriginRay.Controls.Add(Me.LbOptOriginRayL)
            Me.PnlOptOriginRay.Cursor = System.Windows.Forms.Cursors.Hand
            Me.PnlOptOriginRay.Location = New System.Drawing.Point(0, 225)
            Me.PnlOptOriginRay.Name = "PnlOptOriginRay"
            Me.PnlOptOriginRay.Size = New System.Drawing.Size(226, 45)
            Me.PnlOptOriginRay.TabIndex = 14
            Me.PnlOptOriginRay.Tag = "6"
            '
            'LbOptOriginRayR
            '
            Me.LbOptOriginRayR.AutoSize = True
            Me.LbOptOriginRayR.BackColor = System.Drawing.Color.Transparent
            Me.LbOptOriginRayR.Font = New System.Drawing.Font(OpenSansLight, 14.0!)
            Me.LbOptOriginRayR.Location = New System.Drawing.Point(107, 9)
            Me.LbOptOriginRayR.Name = "LbOptOriginRayR"
            Me.LbOptOriginRayR.Size = New System.Drawing.Size(104, 26)
            Me.LbOptOriginRayR.TabIndex = 14
            Me.LbOptOriginRayR.Tag = "6"
            Me.LbOptOriginRayR.Text = "Ray / Angle"
            '
            'LbOptOriginRayL
            '
            Me.LbOptOriginRayL.AutoSize = True
            Me.LbOptOriginRayL.BackColor = System.Drawing.Color.Transparent
            Me.LbOptOriginRayL.Font = New System.Drawing.Font(OpenSans, 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.LbOptOriginRayL.Location = New System.Drawing.Point(6, 7)
            Me.LbOptOriginRayL.Name = "LbOptOriginRayL"
            Me.LbOptOriginRayL.Size = New System.Drawing.Size(28, 28)
            Me.LbOptOriginRayL.TabIndex = 11
            Me.LbOptOriginRayL.Tag = "6"
            Me.LbOptOriginRayL.Text = "ϴ"
            '
            'PnlOptPolar
            '
            Me.PnlOptPolar.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.PnlOptPolar.Controls.Add(Me.LbOptPolarR)
            Me.PnlOptPolar.Controls.Add(Me.LbOptPolarL)
            Me.PnlOptPolar.Cursor = System.Windows.Forms.Cursors.Hand
            Me.PnlOptPolar.Location = New System.Drawing.Point(0, 135)
            Me.PnlOptPolar.Name = "PnlOptPolar"
            Me.PnlOptPolar.Size = New System.Drawing.Size(226, 45)
            Me.PnlOptPolar.TabIndex = 10
            Me.PnlOptPolar.Tag = "4"
            '
            'LbOptPolarR
            '
            Me.LbOptPolarR.AutoSize = True
            Me.LbOptPolarR.BackColor = System.Drawing.Color.Transparent
            Me.LbOptPolarR.Font = New System.Drawing.Font(OpenSansLight, 14.0!)
            Me.LbOptPolarR.Location = New System.Drawing.Point(164, 9)
            Me.LbOptPolarR.Name = "LbOptPolarR"
            Me.LbOptPolarR.Size = New System.Drawing.Size(55, 26)
            Me.LbOptPolarR.TabIndex = 11
            Me.LbOptPolarR.Tag = "4"
            Me.LbOptPolarR.Text = "Polar"
            '
            'LbOptPolarL
            '
            Me.LbOptPolarL.AutoSize = True
            Me.LbOptPolarL.BackColor = System.Drawing.Color.Transparent
            Me.LbOptPolarL.Font = New System.Drawing.Font(OpenSans, 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.LbOptPolarL.Location = New System.Drawing.Point(7, 7)
            Me.LbOptPolarL.Name = "LbOptPolarL"
            Me.LbOptPolarL.Size = New System.Drawing.Size(40, 28)
            Me.LbOptPolarL.TabIndex = 10
            Me.LbOptPolarL.Tag = "4"
            Me.LbOptPolarL.Text = "r(t)"
            '
            'PnlOptParametric
            '
            Me.PnlOptParametric.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.PnlOptParametric.Controls.Add(Me.LbOptParametricR)
            Me.PnlOptParametric.Controls.Add(Me.LbOptParametricL)
            Me.PnlOptParametric.Cursor = System.Windows.Forms.Cursors.Hand
            Me.PnlOptParametric.Location = New System.Drawing.Point(0, 90)
            Me.PnlOptParametric.Name = "PnlOptParametric"
            Me.PnlOptParametric.Size = New System.Drawing.Size(226, 45)
            Me.PnlOptParametric.TabIndex = 9
            Me.PnlOptParametric.Tag = "3"
            '
            'LbOptParametricR
            '
            Me.LbOptParametricR.AutoSize = True
            Me.LbOptParametricR.BackColor = System.Drawing.Color.Transparent
            Me.LbOptParametricR.Font = New System.Drawing.Font(OpenSansLight, 14.0!)
            Me.LbOptParametricR.Location = New System.Drawing.Point(113, 9)
            Me.LbOptParametricR.Name = "LbOptParametricR"
            Me.LbOptParametricR.Size = New System.Drawing.Size(103, 26)
            Me.LbOptParametricR.TabIndex = 10
            Me.LbOptParametricR.Tag = "3"
            Me.LbOptParametricR.Text = "Parametric"
            '
            'LbOptParametricL
            '
            Me.LbOptParametricL.AutoSize = True
            Me.LbOptParametricL.BackColor = System.Drawing.Color.Transparent
            Me.LbOptParametricL.Font = New System.Drawing.Font(OpenSans, 13.75!)
            Me.LbOptParametricL.Location = New System.Drawing.Point(5, 10)
            Me.LbOptParametricL.Name = "LbOptParametricL"
            Me.LbOptParametricL.Size = New System.Drawing.Size(102, 26)
            Me.LbOptParametricL.TabIndex = 9
            Me.LbOptParametricL.Tag = "3"
            Me.LbOptParametricL.Text = "<x(t), y(t)>"
            '
            'PnlOptCartesian
            '
            Me.PnlOptCartesian.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.PnlOptCartesian.BackColor = System.Drawing.Color.FromArgb(CType(CType(160, Byte), Integer), CType(CType(70, Byte), Integer), CType(CType(10, Byte), Integer))
            Me.PnlOptCartesian.Controls.Add(Me.LbOptCartesianR)
            Me.PnlOptCartesian.Controls.Add(Me.LbOptCartesianL)
            Me.PnlOptCartesian.Cursor = System.Windows.Forms.Cursors.Arrow
            Me.PnlOptCartesian.Location = New System.Drawing.Point(0, 0)
            Me.PnlOptCartesian.Name = "PnlOptCartesian"
            Me.PnlOptCartesian.Size = New System.Drawing.Size(226, 45)
            Me.PnlOptCartesian.TabIndex = 8
            Me.PnlOptCartesian.Tag = "1"
            '
            'LbOptCartesianR
            '
            Me.LbOptCartesianR.AutoSize = True
            Me.LbOptCartesianR.BackColor = System.Drawing.Color.Transparent
            Me.LbOptCartesianR.Font = New System.Drawing.Font(OpenSansLight, 14.0!)
            Me.LbOptCartesianR.Location = New System.Drawing.Point(124, 9)
            Me.LbOptCartesianR.Name = "LbOptCartesianR"
            Me.LbOptCartesianR.Size = New System.Drawing.Size(91, 26)
            Me.LbOptCartesianR.TabIndex = 9
            Me.LbOptCartesianR.Tag = "1"
            Me.LbOptCartesianR.Text = "Cartesian"
            '
            'LbOptCartesianL
            '
            Me.LbOptCartesianL.AutoSize = True
            Me.LbOptCartesianL.BackColor = System.Drawing.Color.Transparent
            Me.LbOptCartesianL.Font = New System.Drawing.Font(OpenSans, 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.LbOptCartesianL.Location = New System.Drawing.Point(7, 7)
            Me.LbOptCartesianL.Name = "LbOptCartesianL"
            Me.LbOptCartesianL.Size = New System.Drawing.Size(75, 28)
            Me.LbOptCartesianL.TabIndex = 8
            Me.LbOptCartesianL.Tag = "1"
            Me.LbOptCartesianL.Text = "y = f(x)"
            '
            'PnlOptDifferential
            '
            Me.PnlOptDifferential.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.PnlOptDifferential.Controls.Add(Me.LbOptDifferentialR)
            Me.PnlOptDifferential.Controls.Add(Me.LbOptDifferentialL)
            Me.PnlOptDifferential.Cursor = System.Windows.Forms.Cursors.Hand
            Me.PnlOptDifferential.Location = New System.Drawing.Point(0, 180)
            Me.PnlOptDifferential.Name = "PnlOptDifferential"
            Me.PnlOptDifferential.Size = New System.Drawing.Size(226, 45)
            Me.PnlOptDifferential.TabIndex = 11
            Me.PnlOptDifferential.Tag = "5"
            '
            'LbOptDifferentialR
            '
            Me.LbOptDifferentialR.AutoSize = True
            Me.LbOptDifferentialR.BackColor = System.Drawing.Color.Transparent
            Me.LbOptDifferentialR.Font = New System.Drawing.Font(OpenSansLight, 14.0!)
            Me.LbOptDifferentialR.Location = New System.Drawing.Point(113, 10)
            Me.LbOptDifferentialR.Name = "LbOptDifferentialR"
            Me.LbOptDifferentialR.Size = New System.Drawing.Size(102, 26)
            Me.LbOptDifferentialR.TabIndex = 12
            Me.LbOptDifferentialR.Tag = "5"
            Me.LbOptDifferentialR.Text = "Differential"
            '
            'LbOptDifferentialL
            '
            Me.LbOptDifferentialL.AutoSize = True
            Me.LbOptDifferentialL.BackColor = System.Drawing.Color.Transparent
            Me.LbOptDifferentialL.Font = New System.Drawing.Font(OpenSans, 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.LbOptDifferentialL.Location = New System.Drawing.Point(6, 8)
            Me.LbOptDifferentialL.Name = "LbOptDifferentialL"
            Me.LbOptDifferentialL.Size = New System.Drawing.Size(73, 28)
            Me.LbOptDifferentialL.TabIndex = 11
            Me.LbOptDifferentialL.Tag = "5"
            Me.LbOptDifferentialL.Text = "dy/dx "
            '
            'LbFnType
            '
            Me.LbFnType.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
            Me.LbFnType.AutoSize = True
            Me.LbFnType.Location = New System.Drawing.Point(8, 284)
            Me.LbFnType.Name = "LbFnType"
            Me.LbFnType.Size = New System.Drawing.Size(117, 19)
            Me.LbFnType.TabIndex = 8
            Me.LbFnType.Text = "Function Options"
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
            Me.Controls.Add(Me.PnlFnType)
            Me.Controls.Add(Me.Split)
            Me.Font = New System.Drawing.Font(OpenSansLight, 10.0!)
            Me.ForeColor = System.Drawing.Color.White
            Me.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
            Me.Name = "GraphingSystem"
            Me.Size = New System.Drawing.Size(984, 661)
            Me.Split.Panel1.ResumeLayout(False)
            Me.Split.Panel2.ResumeLayout(False)
            CType(Me.Split, System.ComponentModel.ISupportInitialize).EndInit()
            Me.Split.ResumeLayout(False)
            Me.PnlWindow.ResumeLayout(False)
            Me.PnlWindow.PerformLayout()
            Me.PnlWHeader.ResumeLayout(False)
            Me.PnlWHeader.PerformLayout()
            Me.PnlTrace.ResumeLayout(False)
            Me.PnlTrace.PerformLayout()
            CType(Me.Canvas, System.ComponentModel.ISupportInitialize).EndInit()
            Me.PnlInput.ResumeLayout(False)
            Me.PnlInput.PerformLayout()
            Me.PnlFnType.ResumeLayout(False)
            Me.PnlFnType.PerformLayout()
            Me.PnlFnTypeSelector.ResumeLayout(False)
            Me.PnlOptInverse.ResumeLayout(False)
            Me.PnlOptInverse.PerformLayout()
            Me.PnlOptOriginRay.ResumeLayout(False)
            Me.PnlOptOriginRay.PerformLayout()
            Me.PnlOptPolar.ResumeLayout(False)
            Me.PnlOptPolar.PerformLayout()
            Me.PnlOptParametric.ResumeLayout(False)
            Me.PnlOptParametric.PerformLayout()
            Me.PnlOptCartesian.ResumeLayout(False)
            Me.PnlOptCartesian.PerformLayout()
            Me.PnlOptDifferential.ResumeLayout(False)
            Me.PnlOptDifferential.PerformLayout()
            Me.ResumeLayout(False)

        End Sub

        Friend WithEvents Split As SplitContainer
        Friend WithEvents BtnAdd As Button
        Friend WithEvents BtnGraph As Button
        Friend WithEvents Tb As TextBox
        Friend WithEvents PnlInput As Panel
        Friend WithEvents LbFx As Label
        Friend WithEvents Canvas As PictureBox
        Friend WithEvents BtnTrace As Button
        Friend WithEvents LbTVal As Label
        Friend WithEvents NpdTVal As TextBox
        Friend WithEvents TmrTraceUpdate As Timer
        Friend WithEvents BtnTNext As Button
        Friend WithEvents BtnScale As Button
        Friend WithEvents TmrDrag As Timer
        Friend WithEvents tt As ToolTip
        Friend WithEvents BtnNextFn As Button
        Friend WithEvents BtnPrevFn As Button
        Friend WithEvents PnlFnType As Panel
        Friend WithEvents PnlFnTypeSelector As Panel
        Friend WithEvents LbFnType As Label
        Friend WithEvents BtnFnDel As Button
        Friend WithEvents PnlOptPolar As Panel
        Friend WithEvents PnlOptParametric As Panel
        Friend WithEvents PnlOptCartesian As Panel
        Friend WithEvents LbOptPolarR As Label
        Friend WithEvents LbOptPolarL As Label
        Friend WithEvents LbOptParametricR As Label
        Friend WithEvents LbOptParametricL As Label
        Friend WithEvents LbOptCartesianR As Label
        Friend WithEvents LbOptCartesianL As Label
        Friend WithEvents PnlOptDifferential As Panel
        Friend WithEvents LbOptDifferentialR As Label
        Friend WithEvents LbOptDifferentialL As Label
        Friend WithEvents PnlWindow As Panel
        Friend WithEvents LbWLft As Label
        Friend WithEvents TbWLft As TextBox
        Friend WithEvents LbWRht As Label
        Friend WithEvents TbWRht As TextBox
        Friend WithEvents LbWBot As Label
        Friend WithEvents TbWBot As TextBox
        Friend WithEvents LbWTop As Label
        Friend WithEvents TbWTop As TextBox
        Friend WithEvents LbWindow As Label
        Friend WithEvents BtnWOK As Button
        Friend WithEvents BtnWCancel As Button
        Friend WithEvents PnlWHeader As Panel
        Friend WithEvents BtnWClose As Button
        Friend WithEvents LbWLogo As Label
        Friend WithEvents PnlTrace As Panel
        Friend WithEvents PnlOptOriginRay As Panel
        Friend WithEvents LbOptOriginRayL As Label
        Friend WithEvents LbOptOriginRayR As Label
        Friend WithEvents PnlOptInverse As Panel
        Friend WithEvents LbOptInverseR As Label
        Friend WithEvents LbOptInverseL As Label
        Friend WithEvents LbTrace As Label
        Friend WithEvents tmrDelayRedraw As Timer
        Friend WithEvents DrawWorker As System.ComponentModel.BackgroundWorker
        Friend WithEvents TmrStart As Timer
        Friend WithEvents TmrHighQuality As Timer
    End Class
End Namespace
