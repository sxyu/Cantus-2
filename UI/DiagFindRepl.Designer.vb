﻿Namespace UI
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class DiagFindRepl
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
            Me.TbFind = New System.Windows.Forms.TextBox()
            Me.Label1 = New System.Windows.Forms.Label()
            Me.Label2 = New System.Windows.Forms.Label()
            Me.TbReplace = New System.Windows.Forms.TextBox()
            Me.btnCancel = New System.Windows.Forms.Button()
            Me.BtnFind = New System.Windows.Forms.Button()
            Me.BtnRepl = New System.Windows.Forms.Button()
            Me.CbRegex = New System.Windows.Forms.CheckBox()
            Me.LbMatchCount = New System.Windows.Forms.Label()
            Me.SuspendLayout()
            '
            'TbFind
            '
            Me.TbFind.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.TbFind.Location = New System.Drawing.Point(21, 39)
            Me.TbFind.Name = "TbFind"
            Me.TbFind.Size = New System.Drawing.Size(472, 28)
            Me.TbFind.TabIndex = 0
            '
            'Label1
            '
            Me.Label1.AutoSize = True
            Me.Label1.Location = New System.Drawing.Point(17, 13)
            Me.Label1.Name = "Label1"
            Me.Label1.Size = New System.Drawing.Size(39, 20)
            Me.Label1.TabIndex = 1
            Me.Label1.Text = "Find"
            '
            'Label2
            '
            Me.Label2.AutoSize = True
            Me.Label2.Location = New System.Drawing.Point(17, 83)
            Me.Label2.Name = "Label2"
            Me.Label2.Size = New System.Drawing.Size(98, 20)
            Me.Label2.TabIndex = 3
            Me.Label2.Text = "Replace With"
            '
            'TbReplace
            '
            Me.TbReplace.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.TbReplace.Location = New System.Drawing.Point(21, 109)
            Me.TbReplace.Name = "TbReplace"
            Me.TbReplace.Size = New System.Drawing.Size(472, 28)
            Me.TbReplace.TabIndex = 1
            '
            'btnCancel
            '
            Me.btnCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnCancel.BackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand
            Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.btnCancel.FlatAppearance.BorderSize = 0
            Me.btnCancel.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(65, Byte), Integer), CType(CType(65, Byte), Integer), CType(CType(65, Byte), Integer))
            Me.btnCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnCancel.Font = New System.Drawing.Font("Open Sans Light", 11.0!)
            Me.btnCancel.ForeColor = System.Drawing.Color.White
            Me.btnCancel.Location = New System.Drawing.Point(202, 205)
            Me.btnCancel.Name = "btnCancel"
            Me.btnCancel.Size = New System.Drawing.Size(93, 40)
            Me.btnCancel.TabIndex = 3
            Me.btnCancel.Tag = "b"
            Me.btnCancel.Text = "&Cancel"
            Me.btnCancel.UseVisualStyleBackColor = False
            '
            'BtnFind
            '
            Me.BtnFind.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.BtnFind.BackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.BtnFind.Cursor = System.Windows.Forms.Cursors.Hand
            Me.BtnFind.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.BtnFind.FlatAppearance.BorderSize = 0
            Me.BtnFind.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(65, Byte), Integer), CType(CType(65, Byte), Integer), CType(CType(65, Byte), Integer))
            Me.BtnFind.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.BtnFind.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.BtnFind.Font = New System.Drawing.Font("Open Sans Light", 11.0!)
            Me.BtnFind.ForeColor = System.Drawing.Color.White
            Me.BtnFind.Location = New System.Drawing.Point(301, 205)
            Me.BtnFind.Name = "BtnFind"
            Me.BtnFind.Size = New System.Drawing.Size(93, 40)
            Me.BtnFind.TabIndex = 4
            Me.BtnFind.Tag = "b"
            Me.BtnFind.Text = "&Find"
            Me.BtnFind.UseVisualStyleBackColor = False
            '
            'BtnRepl
            '
            Me.BtnRepl.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.BtnRepl.BackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.BtnRepl.Cursor = System.Windows.Forms.Cursors.Hand
            Me.BtnRepl.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.BtnRepl.FlatAppearance.BorderSize = 0
            Me.BtnRepl.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(65, Byte), Integer), CType(CType(65, Byte), Integer), CType(CType(65, Byte), Integer))
            Me.BtnRepl.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.BtnRepl.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.BtnRepl.Font = New System.Drawing.Font("Open Sans Light", 11.0!)
            Me.BtnRepl.ForeColor = System.Drawing.Color.White
            Me.BtnRepl.Location = New System.Drawing.Point(400, 205)
            Me.BtnRepl.Name = "BtnRepl"
            Me.BtnRepl.Size = New System.Drawing.Size(93, 40)
            Me.BtnRepl.TabIndex = 5
            Me.BtnRepl.Tag = "b"
            Me.BtnRepl.Text = "&Replace"
            Me.BtnRepl.UseVisualStyleBackColor = False
            '
            'CbRegex
            '
            Me.CbRegex.AutoSize = True
            Me.CbRegex.Location = New System.Drawing.Point(21, 157)
            Me.CbRegex.Name = "CbRegex"
            Me.CbRegex.Size = New System.Drawing.Size(216, 24)
            Me.CbRegex.TabIndex = 2
            Me.CbRegex.Text = "Enable Regular E&xpressions"
            Me.CbRegex.UseVisualStyleBackColor = True
            '
            'LbMatchCount
            '
            Me.LbMatchCount.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
            Me.LbMatchCount.AutoSize = True
            Me.LbMatchCount.ForeColor = System.Drawing.Color.Gainsboro
            Me.LbMatchCount.Location = New System.Drawing.Point(17, 215)
            Me.LbMatchCount.Name = "LbMatchCount"
            Me.LbMatchCount.Size = New System.Drawing.Size(115, 20)
            Me.LbMatchCount.TabIndex = 6
            Me.LbMatchCount.Text = "Matches Found"
            Me.LbMatchCount.Visible = False
            '
            'DiagFindRepl
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
            Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(44, Byte), Integer), CType(CType(44, Byte), Integer), CType(CType(44, Byte), Integer))
            Me.CancelButton = Me.btnCancel
            Me.ClientSize = New System.Drawing.Size(509, 257)
            Me.Controls.Add(Me.LbMatchCount)
            Me.Controls.Add(Me.CbRegex)
            Me.Controls.Add(Me.BtnRepl)
            Me.Controls.Add(Me.BtnFind)
            Me.Controls.Add(Me.btnCancel)
            Me.Controls.Add(Me.TbReplace)
            Me.Controls.Add(Me.TbFind)
            Me.Controls.Add(Me.Label2)
            Me.Controls.Add(Me.Label1)
            Me.Font = New System.Drawing.Font("Open Sans", 11.25!)
            Me.ForeColor = System.Drawing.Color.White
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
            Me.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
            Me.Name = "DiagFindRepl"
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
            Me.Text = "Find and Replace"
            Me.TopMost = True
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub

        Friend WithEvents TbFind As TextBox
        Friend WithEvents Label1 As Label
        Friend WithEvents Label2 As Label
        Friend WithEvents TbReplace As TextBox
        Friend WithEvents btnCancel As Button
        Friend WithEvents BtnFind As Button
        Friend WithEvents BtnRepl As Button
        Friend WithEvents CbRegex As CheckBox
        Friend WithEvents LbMatchCount As Label
    End Class
End Namespace
