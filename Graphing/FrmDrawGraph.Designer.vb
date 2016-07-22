<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FrmDrawGraph
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.tbEdges = New System.Windows.Forms.TextBox()
        Me.canvas = New System.Windows.Forms.PictureBox()
        Me.lbEdges = New System.Windows.Forms.Label()
        Me.btnVis = New System.Windows.Forms.Button()
        CType(Me.canvas, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'tbEdges
        '
        Me.tbEdges.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.tbEdges.BackColor = System.Drawing.Color.FromArgb(CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer))
        Me.tbEdges.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.tbEdges.ForeColor = System.Drawing.Color.White
        Me.tbEdges.Location = New System.Drawing.Point(12, 57)
        Me.tbEdges.Multiline = True
        Me.tbEdges.Name = "tbEdges"
        Me.tbEdges.Size = New System.Drawing.Size(223, 419)
        Me.tbEdges.TabIndex = 0
        Me.tbEdges.Text = "1 2" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "2 3" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "3 4" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "4 1"
        '
        'canvas
        '
        Me.canvas.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.canvas.BackColor = System.Drawing.Color.FromArgb(CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer))
        Me.canvas.Location = New System.Drawing.Point(255, -3)
        Me.canvas.Name = "canvas"
        Me.canvas.Size = New System.Drawing.Size(743, 546)
        Me.canvas.TabIndex = 1
        Me.canvas.TabStop = False
        '
        'lbEdges
        '
        Me.lbEdges.AutoSize = True
        Me.lbEdges.ForeColor = System.Drawing.Color.White
        Me.lbEdges.Location = New System.Drawing.Point(8, 23)
        Me.lbEdges.Name = "lbEdges"
        Me.lbEdges.Size = New System.Drawing.Size(230, 20)
        Me.lbEdges.TabIndex = 2
        Me.lbEdges.Text = "Digraph Edges (src dest src dest...)" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10)
        '
        'btnVis
        '
        Me.btnVis.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.btnVis.BackColor = System.Drawing.Color.FromArgb(CType(CType(160, Byte), Integer), CType(CType(70, Byte), Integer), CType(CType(10, Byte), Integer))
        Me.btnVis.FlatAppearance.BorderSize = 0
        Me.btnVis.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(210, Byte), Integer), CType(CType(80, Byte), Integer), CType(CType(20, Byte), Integer))
        Me.btnVis.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(160, Byte), Integer), CType(CType(70, Byte), Integer), CType(CType(10, Byte), Integer))
        Me.btnVis.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnVis.ForeColor = System.Drawing.Color.White
        Me.btnVis.Location = New System.Drawing.Point(12, 489)
        Me.btnVis.Name = "btnVis"
        Me.btnVis.Size = New System.Drawing.Size(223, 41)
        Me.btnVis.TabIndex = 5
        Me.btnVis.Text = "Visualize"
        Me.btnVis.UseVisualStyleBackColor = False
        '
        'FrmDrawGraph
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoSize = True
        Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer))
        Me.ClientSize = New System.Drawing.Size(997, 542)
        Me.Controls.Add(Me.btnVis)
        Me.Controls.Add(Me.lbEdges)
        Me.Controls.Add(Me.canvas)
        Me.Controls.Add(Me.tbEdges)
        Me.Font = New System.Drawing.Font("Segoe UI Semilight", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.MinimumSize = New System.Drawing.Size(1013, 581)
        Me.Name = "FrmDrawGraph"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Calculatorˣ Graph Visualizer Beta"
        CType(Me.canvas, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents tbEdges As TextBox
    Friend WithEvents canvas As PictureBox
    Friend WithEvents lbEdges As Label
    Friend WithEvents btnVis As Button
End Class
