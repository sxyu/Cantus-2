Imports System.Text
Imports Cantus.Core.CantusEvaluator
Imports Cantus.Core.CommonTypes
Imports Cantus.Core.Scoping

Namespace UI.Dialogs
    '' <summary>
    '' Dialog for listing all available functions
    '' </summary>
    Public Class DiagFunctions

        Public Property Result As String = ""
        Private Shared _lastResult As String = ""

        Private Sub DiagFunctions_Load(sender As Object, e As EventArgs) Handles MyBase.Load
            LoadLv()
        End Sub

        Private Sub LoadLv(Optional filter As String = "")
            lv.Items.Clear()
            Dim toAdd As New List(Of ListViewItem)
            Dim info As IEnumerable(Of Reflection.MethodInfo)
            info = InternalFunctions.Methods

            ' add internal functions
            For Each i As Reflection.MethodInfo In info
                Try
                    ' regex filter
                    If Not New InternalFunctions(Nothing).Contains(ROOT_NAMESPACE & SCOPE_SEP & i.Name.ToLowerInvariant(),
                                                                    filter.ToLowerInvariant()) Then Continue For
                Catch
                End Try

                Dim name As New StringBuilder(ROOT_NAMESPACE)
                Dim tag As New StringBuilder(i.Name)
                Dim types As New StringBuilder()

                Dim init As Boolean = True
                name.Append(SCOPE_SEP).Append(i.Name.ToLowerInvariant()).Append(" (")
                tag.Append("(")

                For Each p As Reflection.ParameterInfo In i.GetParameters()
                    If Not init Then
                        name.Append(", ")
                        If Not p.IsOptional Then
                            tag.Append(",")
                        End If
                        types.Append(", ")
                    Else
                        init = False
                    End If

                    Dim typeName As String = GetEvaluatorTypeName(p.ParameterType)

                    If typeName.Contains("`") Then typeName = typeName.Remove(typeName.IndexOf("`"c))

                    If p.IsOptional Then
                        types.Append("[")
                    End If

                    types.Append(typeName)
                    If typeName <> "" Then types.Append(" ")

                    If p.Name = "dt" AndAlso typeName = "(Variable)" Then
                        types.Append("DateTime dt")
                        name.Append("dt")
                    Else
                        types.Append(p.Name)
                        name.Append(p.Name)
                    End If

                    If p.IsOptional Then
                        If p.DefaultValue Is Nothing OrElse
                            TypeOf p.DefaultValue Is Double AndAlso Double.IsNaN(CDbl(p.DefaultValue)) OrElse
                            TypeOf p.DefaultValue Is BigDecimal AndAlso CType(p.DefaultValue, BigDecimal).IsUndefined Then
                            name.Append(" = Undefined")
                        ElseIf TypeOf p.DefaultValue Is String Then
                            name.Append(" = '" & p.DefaultValue.ToString() & "'")
                        Else
                            name.Append(" = " & p.DefaultValue.ToString())
                        End If
                        types.Append("]")
                    End If
                Next

                Dim returnType As String = GetEvaluatorTypeName(i.ReturnType)

                If returnType.Contains("`") Then returnType = returnType.Remove(returnType.IndexOf("`"c))

                If types.Length = 0 Then types.Append("(No Params)")

                types.Append(" → ").Append(returnType)

                name.Append(")")
                tag.Append(")")

                Dim lvI As New ListViewItem(name.ToString())

                lvI.Tag = tag.ToString().ToLowerInvariant()
                lvI.SubItems.Add(types.ToString())

                lvI.SubItems.Add(i.MetadataToken.ToString()) ' hidden item for sorting based on declaration order

                If lvI.Tag.ToString() = _lastResult Then
                    lvI.Selected = True
                End If

                toAdd.Add(lvI)
            Next

            ' add user functions
            For Each uf As UserFunction In RootEvaluator.UserFunctions.Values
                Try
                    If uf.Modifiers.Contains("private") OrElse
                       uf.Modifiers.Contains("hidden") Then Continue For ' ignore private methods

                    ' regex filter
                    If Not New InternalFunctions(Nothing).Contains(uf.FullName.ToLowerInvariant(),
                                                                filter.ToLowerInvariant()) Then Continue For
                Catch
                End Try

                Dim name As New StringBuilder(uf.FullName)
                Dim tag As New StringBuilder(RemoveRedundantScope(uf.FullName, Globals.RootEvaluator.Scope))
                Dim types As New StringBuilder()

                Dim init As Boolean = True
                name.Append(" (")
                tag.Append("(")

                Dim ct = 0
                For Each p As String In uf.Args
                    If Not init Then
                        tag.Append(",")
                        types.Append(", ")
                        name.Append(", ")
                    Else
                        init = False
                    End If

                    If uf.Defaults.Count >= ct AndAlso Not uf.Defaults(ct) Is Nothing AndAlso
                        Not (TypeOf uf.Defaults(ct) Is Double AndAlso
                        Double.IsNaN(DirectCast(uf.Defaults(ct), Double))) Then
                        types.Append("[")
                    End If

                    types.Append(p)
                    name.Append(p)

                    If uf.Defaults.Count >= ct AndAlso
                        Not uf.Defaults(ct) Is Nothing AndAlso
                        Not (TypeOf uf.Defaults(ct) Is Double AndAlso
                        Double.IsNaN(DirectCast(uf.Defaults(ct), Double))) Then
                        types.Append("]")
                        name.Append(" = " & uf.Defaults(ct).ToString())
                    End If
                    ct += 1
                Next

                name.Append(")")

                tag.Append(")")

                If types.Length = 0 Then types.Append("(No Params)")
                types.Append(" → (Variable)")

                Dim lvI As New ListViewItem(name.ToString())

                lvI.Tag = tag.ToString()
                lvI.SubItems.Add(types.ToString())

                lvI.SubItems.Add("0") ' order at beginning

                If IsExternalScope(uf.DeclaringScope, Globals.RootEvaluator.Scope) Then
                    lvI.BackColor = Color.FromArgb(35, 35, 35) ' color for user functions
                Else
                    lvI.BackColor = Color.FromArgb(50, 50, 35) ' color for plugins
                End If

                If lvI.Tag.ToString() = _lastResult Then
                    lvI.Selected = True
                End If

                toAdd.Add(lvI)
            Next

            toAdd.Sort(New ListViewItemComparer())
            lv.Items.AddRange(toAdd.ToArray())

            If lv.Items.Count > 0 AndAlso lv.SelectedItems.Count = 0 Then lv.Items(0).Selected = True

            If lv.SelectedIndices.Count > 0 Then lv.EnsureVisible(lv.SelectedIndices(0))
        End Sub

        Private Sub BtnOK_Click(sender As Object, e As EventArgs) Handles BtnOK.Click

            If lv.SelectedItems.Count > 0 Then
                Me.DialogResult = DialogResult.OK
                Me.Result = lv.SelectedItems(0).Tag.ToString()
                _lastResult = Me.Result
            Else
                Me.DialogResult = DialogResult.Cancel
                Me.Result = ""
            End If

            Me.Close()
        End Sub

        Private Sub BtnCancel_Click(sender As Object, e As EventArgs) Handles BtnCancel.Click
            Me.DialogResult = DialogResult.Cancel
            Me.Result = ""
            Me.Close()
        End Sub

        Private Sub TbSearch_Enter(sender As Object, e As EventArgs) Handles TbSearch.Enter
            If TbSearch.ForeColor <> Color.White Then
                TbSearch.Text = ""
                TbSearch.ForeColor = Color.White
            End If
        End Sub

        Private Sub TbSearch_Leave(sender As Object, e As EventArgs) Handles TbSearch.Leave
            If String.IsNullOrWhiteSpace(TbSearch.Text) Then
                TbSearch.ForeColor = Color.Gainsboro
                TbSearch.Text = "Type to Filter Functions (Regex Enabled) ..."
            End If
        End Sub

        Private Sub TbSearch_TextChanged(sender As Object, e As EventArgs) Handles TbSearch.TextChanged
            If TbSearch.ForeColor = Color.White Then
                LoadLv(TbSearch.Text)
            End If
        End Sub

        Private Sub PnlSearch_Click(sender As Object, e As EventArgs) Handles PnlSearch.Click
            TbSearch.Focus()
        End Sub

        Private Sub lv_KeyPress(sender As Object, e As KeyPressEventArgs) Handles lv.KeyPress
            TbSearch.Focus()
            SendKeys.Send(e.KeyChar)
            e.Handled = True
        End Sub

        Private Sub lv_ItemActivate(sender As Object, e As EventArgs) Handles lv.ItemActivate
            BtnOK.PerformClick()
        End Sub
    End Class
    Public Class ListViewItemComparer
        Implements IComparer(Of ListViewItem)

        Public Function Compare(x As ListViewItem, y As ListViewItem) As Integer Implements IComparer(Of ListViewItem).Compare
            Return Integer.Parse(x.SubItems(x.SubItems.Count - 1).Text).CompareTo(Integer.Parse(y.SubItems(y.SubItems.Count - 1).Text))
        End Function
    End Class
End Namespace