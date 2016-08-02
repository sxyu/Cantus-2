﻿Namespace UI
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class FrmViewer
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmViewer))
            Me.lbTitle = New System.Windows.Forms.Label()
            Me.btnMin = New System.Windows.Forms.Button()
            Me.btnLog = New System.Windows.Forms.Button()
            Me.btnGraph = New System.Windows.Forms.Button()
            Me.pnl = New System.Windows.Forms.Panel()
            Me.SuspendLayout()
            '
            'lbTitle
            '
            Me.lbTitle.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.lbTitle.AutoSize = True
            Me.lbTitle.ForeColor = System.Drawing.Color.LightGray
            Me.lbTitle.Location = New System.Drawing.Point(460, 14)
            Me.lbTitle.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
            Me.lbTitle.Name = "lbTitle"
            Me.lbTitle.Size = New System.Drawing.Size(91, 20)
            Me.lbTitle.TabIndex = 0
            Me.lbTitle.Text = "Cantus {VER}"
            '
            'btnMin
            '
            Me.btnMin.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnMin.BackColor = System.Drawing.Color.FromArgb(CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer))
            Me.btnMin.Cursor = System.Windows.Forms.Cursors.Hand
            Me.btnMin.FlatAppearance.BorderSize = 0
            Me.btnMin.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(65, Byte), Integer), CType(CType(65, Byte), Integer), CType(CType(65, Byte), Integer))
            Me.btnMin.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer))
            Me.btnMin.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnMin.ForeColor = System.Drawing.Color.White
            Me.btnMin.Location = New System.Drawing.Point(568, -1)
            Me.btnMin.Name = "btnMin"
            Me.btnMin.Size = New System.Drawing.Size(53, 50)
            Me.btnMin.TabIndex = 6
            Me.btnMin.TabStop = False
            Me.btnMin.Text = "-"
            Me.btnMin.UseVisualStyleBackColor = False
            '
            'btnLog
            '
            Me.btnLog.BackColor = System.Drawing.Color.FromArgb(CType(CType(37, Byte), Integer), CType(CType(37, Byte), Integer), CType(CType(37, Byte), Integer))
            Me.btnLog.Cursor = System.Windows.Forms.Cursors.Hand
            Me.btnLog.FlatAppearance.BorderSize = 0
            Me.btnLog.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(80, Byte), Integer), CType(CType(80, Byte), Integer), CType(CType(80, Byte), Integer))
            Me.btnLog.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.btnLog.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnLog.ForeColor = System.Drawing.Color.White
            Me.btnLog.Image = CType(resources.GetObject("btnLog.Image"), System.Drawing.Image)
            Me.btnLog.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
            Me.btnLog.Location = New System.Drawing.Point(1, 0)
            Me.btnLog.Name = "btnLog"
            Me.btnLog.Padding = New System.Windows.Forms.Padding(0, 0, 10, 0)
            Me.btnLog.Size = New System.Drawing.Size(151, 48)
            Me.btnLog.TabIndex = 1
            Me.btnLog.Tag = "log"
            Me.btnLog.Text = "My Results"
            Me.btnLog.TextAlign = System.Drawing.ContentAlignment.MiddleRight
            Me.btnLog.UseVisualStyleBackColor = False
            '
            'btnGraph
            '
            Me.btnGraph.BackColor = System.Drawing.Color.FromArgb(CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer))
            Me.btnGraph.Cursor = System.Windows.Forms.Cursors.Hand
            Me.btnGraph.FlatAppearance.BorderSize = 0
            Me.btnGraph.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(65, Byte), Integer), CType(CType(65, Byte), Integer), CType(CType(65, Byte), Integer))
            Me.btnGraph.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer))
            Me.btnGraph.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnGraph.ForeColor = System.Drawing.Color.White
            Me.btnGraph.Image = CType(resources.GetObject("btnGraph.Image"), System.Drawing.Image)
            Me.btnGraph.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
            Me.btnGraph.Location = New System.Drawing.Point(153, 1)
            Me.btnGraph.Name = "btnGraph"
            Me.btnGraph.Padding = New System.Windows.Forms.Padding(0, 0, 10, 0)
            Me.btnGraph.Size = New System.Drawing.Size(124, 48)
            Me.btnGraph.TabIndex = 2
            Me.btnGraph.Tag = "graphing"
            Me.btnGraph.Text = "Graph"
            Me.btnGraph.TextAlign = System.Drawing.ContentAlignment.MiddleRight
            Me.btnGraph.UseVisualStyleBackColor = False
            '
            'pnl
            '
            Me.pnl.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.pnl.BackColor = System.Drawing.Color.FromArgb(CType(CType(37, Byte), Integer), CType(CType(37, Byte), Integer), CType(CType(37, Byte), Integer))
            Me.pnl.Location = New System.Drawing.Point(1, 48)
            Me.pnl.Name = "pnl"
            Me.pnl.Size = New System.Drawing.Size(620, 650)
            Me.pnl.TabIndex = 0
            '
            'FrmViewer
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
            Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer))
            Me.ClientSize = New System.Drawing.Size(621, 700)
            Me.Controls.Add(Me.pnl)
            Me.Controls.Add(Me.btnGraph)
            Me.Controls.Add(Me.btnLog)
            Me.Controls.Add(Me.btnMin)
            Me.Controls.Add(Me.lbTitle)
            Me.Font = New System.Drawing.Font("Segoe UI Semilight", 11.0!)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
            Me.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
            Me.Name = "FrmViewer"
            Me.StartPosition = System.Windows.Forms.FormStartPosition.Manual
            Me.Text = "Cantus"
            Me.TopMost = True
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub

        Friend WithEvents lbTitle As Label
        Friend WithEvents btnMin As Button
        Friend WithEvents btnLog As Button
        Friend WithEvents btnGraph As Button
        Friend WithEvents pnl As Panel
    End Class
End Namespace
