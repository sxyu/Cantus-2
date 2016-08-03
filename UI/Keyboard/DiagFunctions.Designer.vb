
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
            Me.btnOK = New System.Windows.Forms.Button()
            Me.btnCancel = New System.Windows.Forms.Button()
            Me.tbSearch = New System.Windows.Forms.TextBox()
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
            Me.lv.Font = New System.Drawing.Font(OpenSans, 11.0!)
            Me.lv.ForeColor = System.Drawing.Color.White
            Me.lv.FullRowSelect = True
            Me.lv.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None
            Me.lv.HideSelection = False
            Me.lv.Location = New System.Drawing.Point(11, 9)
            Me.lv.MultiSelect = False
            Me.lv.Name = "lv"
            Me.lv.Size = New System.Drawing.Size(1150, 530)
            Me.lv.TabIndex = 0
            Me.lv.UseCompatibleStateImageBehavior = False
            Me.lv.View = System.Windows.Forms.View.Details
            '
            'ColName
            '
            Me.ColName.Text = "Name"
            Me.ColName.Width = 450
            '
            'ColDescription
            '
            Me.ColDescription.Text = "Description"
            Me.ColDescription.Width = 600
            '
            'btnOK
            '
            Me.btnOK.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnOK.BackColor = System.Drawing.Color.FromArgb(CType(CType(160, Byte), Integer), CType(CType(70, Byte), Integer), CType(CType(10, Byte), Integer))
            Me.btnOK.Cursor = System.Windows.Forms.Cursors.Hand
            Me.btnOK.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.btnOK.FlatAppearance.BorderSize = 0
            Me.btnOK.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(210, Byte), Integer), CType(CType(80, Byte), Integer), CType(CType(20, Byte), Integer))
            Me.btnOK.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(160, Byte), Integer), CType(CType(70, Byte), Integer), CType(CType(10, Byte), Integer))
            Me.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnOK.ForeColor = System.Drawing.Color.White
            Me.btnOK.Location = New System.Drawing.Point(1081, -1)
            Me.btnOK.Name = "btnOK"
            Me.btnOK.Size = New System.Drawing.Size(80, 49)
            Me.btnOK.TabIndex = 3
            Me.btnOK.Text = "&Insert"
            Me.btnOK.UseVisualStyleBackColor = False
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
            Me.btnCancel.Font = New System.Drawing.Font(OpenSansLight, 11.0!)
            Me.btnCancel.ForeColor = System.Drawing.Color.White
            Me.btnCancel.Location = New System.Drawing.Point(999, 0)
            Me.btnCancel.Name = "btnCancel"
            Me.btnCancel.Size = New System.Drawing.Size(83, 49)
            Me.btnCancel.TabIndex = 4
            Me.btnCancel.Tag = "b"
            Me.btnCancel.Text = "&Cancel"
            Me.btnCancel.UseVisualStyleBackColor = False
            '
            'tbSearch
            '
            Me.tbSearch.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.tbSearch.BackColor = System.Drawing.Color.FromArgb(CType(CType(45, Byte), Integer), CType(CType(45, Byte), Integer), CType(CType(45, Byte), Integer))
            Me.tbSearch.BorderStyle = System.Windows.Forms.BorderStyle.None
            Me.tbSearch.Font = New System.Drawing.Font(OpenSans, 14.0!)
            Me.tbSearch.ForeColor = System.Drawing.Color.Gainsboro
            Me.tbSearch.Location = New System.Drawing.Point(11, 9)
            Me.tbSearch.Name = "tbSearch"
            Me.tbSearch.Size = New System.Drawing.Size(982, 26)
            Me.tbSearch.TabIndex = 2
            Me.tbSearch.Text = "Type to Filter Functions (Regex Enabled) ..."
            '
            'PnlSearch
            '
            Me.PnlSearch.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.PnlSearch.BackColor = System.Drawing.Color.FromArgb(CType(CType(45, Byte), Integer), CType(CType(45, Byte), Integer), CType(CType(45, Byte), Integer))
            Me.PnlSearch.Controls.Add(Me.tbSearch)
            Me.PnlSearch.Controls.Add(Me.btnCancel)
            Me.PnlSearch.Controls.Add(Me.btnOK)
            Me.PnlSearch.Cursor = System.Windows.Forms.Cursors.IBeam
            Me.PnlSearch.Location = New System.Drawing.Point(1, 542)
            Me.PnlSearch.Name = "PnlSearch"
            Me.PnlSearch.Size = New System.Drawing.Size(1159, 52)
            Me.PnlSearch.TabIndex = 5
            '
            'DiagFunctions
            '
            Me.AcceptButton = Me.btnOK
            Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
            Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer))
            Me.CancelButton = Me.btnCancel
            Me.ClientSize = New System.Drawing.Size(1159, 588)
            Me.Controls.Add(Me.PnlSearch)
            Me.Controls.Add(Me.lv)
            Me.Font = New System.Drawing.Font(OpenSansLight, 11.0!)
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
        Friend WithEvents btnOK As Button
        Friend WithEvents btnCancel As Button
        Friend WithEvents ColName As ColumnHeader
        Friend WithEvents ColDescription As ColumnHeader
        Friend WithEvents tbSearch As TextBox
        Friend WithEvents PnlSearch As Panel
    End Class
End Namespace
