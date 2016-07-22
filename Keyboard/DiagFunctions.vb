Imports System.Text
Imports Cantus.Calculator.Evaluator

Public Class DiagFunctions

    Public Property Result As String = ""
    Private Shared _lastResult As String = ""

    Private Sub DiagFunctions_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadLv()
    End Sub

    Private Sub LoadLv(Optional filter As String = "")
        lv.Items.Clear()
        Dim toAdd As New List(Of ListViewItem)
        Dim info As Reflection.MethodInfo()
        info = GetType(InternalFunctions).GetMethods(
                        Reflection.BindingFlags.Public Or Reflection.BindingFlags.Instance Or
                    Reflection.BindingFlags.DeclaredOnly)

        ' add internal functions
        For Each i As Reflection.MethodInfo In info
            Try
                If Not Globals.Evaluator.InternalFunctions.Contains(i.Name.ToLowerInvariant(),
                                                                    filter.ToLowerInvariant()) Then Continue For
            Catch
            End Try

            Dim name As New StringBuilder(i.Name)
            Dim tag As New StringBuilder(i.Name)
            Dim types As New StringBuilder()

            Dim init As Boolean = True
            name.Append(" (")
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

                Dim typeName As String = p.ParameterType.Name().Replace("Double", "Number").Replace("BigDecimal", "Number").Replace("String", "Text").
                    Replace("List", "Matrix").Replace("SortedDictionary", "Set").Replace("Reference[]", "Tuple").
                    Replace("Object", "(Variable)").Replace("ICollection", "(Matrix/Set/Tuple)")

                If typeName.Contains("`") Then typeName = typeName.Remove(typeName.IndexOf("`"c))

                If p.IsOptional Then
                    types.Append("[")
                    name.Append("[")
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
                    types.Append("]")
                    name.Append("]")
                End If
            Next

            Dim returnType As String = i.ReturnType.Name.Replace("Double", "Number").Replace("BigDecimal", "Number").Replace("String", "Text").
                    Replace("List", "Matrix").Replace("SortedDictionary", "Set").Replace("Reference[]", "Tuple").
                    Replace("Object", "(Variable)").Replace("ICollection", "(Matrix/Set/Tuple)")

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
        For Each uf As String In Globals.Evaluator.ListUserFunctions()
            Try
                If Not Globals.Evaluator.InternalFunctions.Contains(uf.ToLowerInvariant(),
                                                                filter.ToLowerInvariant()) Then Continue For
            Catch
            End Try

            Dim name As New StringBuilder(uf)
            Dim tag As New StringBuilder(uf)
            Dim types As New StringBuilder()

            Dim init As Boolean = True
            tag.Append("(")
            name.Append(" (")

            For Each p As String In Globals.Evaluator.GetUserFunctionArgs(uf)
                If Not init Then
                    tag.Append(",")
                    types.Append(", ")
                Else
                    init = False
                End If

                types.Append("(Variable) ").Append(p)
                name.Append(p)
            Next

            name.Append(")")

            tag.Append(")")
            types.Append(" → (Variable)")

            Dim lvI As New ListViewItem(name.ToString())

            lvI.Tag = tag.ToString()
            lvI.SubItems.Add(types.ToString())

            lvI.SubItems.Add("0") ' order at beginning

            lvI.BackColor = Color.FromArgb(35, 35, 35)

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

    Private Sub btnOK_Click(sender As Object, e As EventArgs) Handles btnOK.Click

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

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.DialogResult = DialogResult.Cancel
        Me.result = ""
        Me.Close()
    End Sub

    Private Sub tbSearch_Enter(sender As Object, e As EventArgs) Handles tbSearch.Enter
        If tbSearch.ForeColor <> Color.White Then
            tbSearch.Text = ""
            tbSearch.ForeColor = Color.White
        End If
    End Sub

    Private Sub tbSearch_Leave(sender As Object, e As EventArgs) Handles tbSearch.Leave
        If String.IsNullOrWhiteSpace(tbSearch.Text) Then
            tbSearch.ForeColor = Color.Gainsboro
            tbSearch.Text = "Filter Functions (Regex)"
        End If
    End Sub

    Private Sub tbSearch_TextChanged(sender As Object, e As EventArgs) Handles tbSearch.TextChanged
        If tbSearch.ForeColor = Color.White Then
            LoadLv(tbSearch.Text)
        End If
    End Sub

    Private Sub PnlSearch_Click(sender As Object, e As EventArgs) Handles PnlSearch.Click
        tbSearch.Focus()
    End Sub

    Private Sub lv_KeyPress(sender As Object, e As KeyPressEventArgs) Handles lv.KeyPress
        tbSearch.Focus()
        SendKeys.Send(e.KeyChar)
        e.Handled = True
    End Sub

    Private Sub lv_ItemActivate(sender As Object, e As EventArgs) Handles lv.ItemActivate
        btnOK.PerformClick()
    End Sub
End Class
Public Class ListViewItemComparer
    Implements IComparer(Of ListViewItem)

    Public Function Compare(x As ListViewItem, y As ListViewItem) As Integer Implements IComparer(Of ListViewItem).Compare
        Return Integer.Parse(x.SubItems(2).Text).CompareTo(Integer.Parse(y.SubItems(2).Text))
    End Function
End Class