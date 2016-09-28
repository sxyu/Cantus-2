
Namespace UI.Dialogs
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class DiagFunctions
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
            Me.lv = New System.Windows.Forms.ListView()
            Me.ColName = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
            Me.ColDescription = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
            Me.BtnOK = New System.Windows.Forms.Button()
            Me.BtnCancel = New System.Windows.Forms.Button()
            Me.TbSearch = New System.Windows.Forms.TextBox()
            Me.PnlSearch = New System.Windows.Forms.Panel()
            Me.PnlSearch.SuspendLayout()
            Me.SuspendLayout()
            '
            'lv
            '
            Me.lv.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.lv.BackColor = System.Drawing.Color.FromArgb(CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer))
            Me.lv.BorderStyle = System.Windows.Forms.BorderStyle.None
            Me.lv.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColName, Me.ColDescription})
            Me.lv.Font = New System.Drawing.Font("Open Sans", 11.0!)
            Me.lv.ForeColor = System.Drawing.Color.White
            Me.lv.FullRowSelect = True
            Me.lv.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None
            Me.lv.HideSelection = False
            Me.lv.Location = New System.Drawing.Point(11, 9)
            Me.lv.MultiSelect = False
            Me.lv.Name = "lv"
            Me.lv.Size = New System.Drawing.Size(1222, 533)
            Me.lv.TabIndex = 0
            Me.lv.UseCompatibleStateImageBehavior = False
            Me.lv.View = System.Windows.Forms.View.Details
            '
            'ColName
            '
            Me.ColName.Text = "Name"
            Me.ColName.Width = 550
            '
            'ColDescription
            '
            Me.ColDescription.Text = "Description"
            Me.ColDescription.Width = 600
            '
            'BtnOK
            '
            Me.BtnOK.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.BtnOK.BackColor = System.Drawing.Color.FromArgb(CType(CType(160, Byte), Integer), CType(CType(70, Byte), Integer), CType(CType(10, Byte), Integer))
            Me.BtnOK.Cursor = System.Windows.Forms.Cursors.Hand
            Me.BtnOK.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.BtnOK.FlatAppearance.BorderSize = 0
            Me.BtnOK.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(210, Byte), Integer), CType(CType(80, Byte), Integer), CType(CType(20, Byte), Integer))
            Me.BtnOK.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(160, Byte), Integer), CType(CType(70, Byte), Integer), CType(CType(10, Byte), Integer))
            Me.BtnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.BtnOK.ForeColor = System.Drawing.Color.White
            Me.BtnOK.Location = New System.Drawing.Point(1153, -1)
            Me.BtnOK.Name = "BtnOK"
            Me.BtnOK.Size = New System.Drawing.Size(80, 49)
            Me.BtnOK.TabIndex = 3
            Me.BtnOK.Text = "&Insert"
            Me.BtnOK.UseVisualStyleBackColor = False
            '
            'BtnCancel
            '
            Me.BtnCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.BtnCancel.BackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.BtnCancel.Cursor = System.Windows.Forms.Cursors.Hand
            Me.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.BtnCancel.FlatAppearance.BorderSize = 0
            Me.BtnCancel.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(65, Byte), Integer), CType(CType(65, Byte), Integer), CType(CType(65, Byte), Integer))
            Me.BtnCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer), CType(CType(55, Byte), Integer))
            Me.BtnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.BtnCancel.Font = New System.Drawing.Font("Open Sans Light", 11.0!)
            Me.BtnCancel.ForeColor = System.Drawing.Color.White
            Me.BtnCancel.Location = New System.Drawing.Point(1071, 0)
            Me.BtnCancel.Name = "BtnCancel"
            Me.BtnCancel.Size = New System.Drawing.Size(83, 49)
            Me.BtnCancel.TabIndex = 4
            Me.BtnCancel.Tag = "b"
            Me.BtnCancel.Text = "&Cancel"
            Me.BtnCancel.UseVisualStyleBackColor = False
            '
            'TbSearch
            '
            Me.TbSearch.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.TbSearch.BackColor = System.Drawing.Color.FromArgb(CType(CType(45, Byte), Integer), CType(CType(45, Byte), Integer), CType(CType(45, Byte), Integer))
            Me.TbSearch.BorderStyle = System.Windows.Forms.BorderStyle.None
            Me.TbSearch.Font = New System.Drawing.Font("Open Sans", 14.0!)
            Me.TbSearch.ForeColor = System.Drawing.Color.Gainsboro
            Me.TbSearch.Location = New System.Drawing.Point(11, 9)
            Me.TbSearch.Name = "TbSearch"
            Me.TbSearch.Size = New System.Drawing.Size(1054, 26)
            Me.TbSearch.TabIndex = 2
            Me.TbSearch.Text = "Type to Filter Functions (Regex Enabled) ..."
            '
            'PnlSearch
            '
            Me.PnlSearch.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.PnlSearch.BackColor = System.Drawing.Color.FromArgb(CType(CType(45, Byte), Integer), CType(CType(45, Byte), Integer), CType(CType(45, Byte), Integer))
            Me.PnlSearch.Controls.Add(Me.TbSearch)
            Me.PnlSearch.Controls.Add(Me.BtnCancel)
            Me.PnlSearch.Controls.Add(Me.BtnOK)
            Me.PnlSearch.Cursor = System.Windows.Forms.Cursors.IBeam
            Me.PnlSearch.Location = New System.Drawing.Point(1, 545)
            Me.PnlSearch.Name = "PnlSearch"
            Me.PnlSearch.Size = New System.Drawing.Size(1231, 52)
            Me.PnlSearch.TabIndex = 5
            '
            'DiagFunctions
            '
            Me.AcceptButton = Me.BtnOK
            Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
            Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer))
            Me.CancelButton = Me.BtnCancel
            Me.ClientSize = New System.Drawing.Size(1231, 591)
            Me.Controls.Add(Me.PnlSearch)
            Me.Controls.Add(Me.lv)
            Me.Font = New System.Drawing.Font("Open Sans Light", 11.0!)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
            Me.Margin = New System.Windows.Forms.Padding(4, 6, 4, 6)
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "DiagFunctions"
            Me.ShowIcon = False
            Me.ShowInTaskbar = False
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
            Me.Text = "Insert Function"
            Me.PnlSearch.ResumeLayout(False)
            Me.PnlSearch.PerformLayout()
            Me.ResumeLayout(False)

        End Sub

        Friend WithEvents lv As ListView
        Friend WithEvents BtnOK As Button
        Friend WithEvents BtnCancel As Button
        Friend WithEvents ColName As ColumnHeader
        Friend WithEvents ColDescription As ColumnHeader
        Friend WithEvents TbSearch As TextBox
        Friend WithEvents PnlSearch As Panel
    End Class
End Namespace
