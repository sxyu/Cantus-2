Imports System.Text
Imports System.Text.RegularExpressions
Imports ScintillaNET
Imports Cantus.Core
Imports Cantus.Core.CantusEvaluator.ObjectTypes

Namespace UI
    Public Class DiagFindRepl
        ''' <summary>
        ''' The texTbox to replace in
        ''' </summary>
        Friend Tb As Scintilla

        ''' <summary>
        ''' Represents a match found by the findall function
        ''' </summary>
        Friend Structure Match
            ''' <summary>
            ''' The starting index of the match
            ''' </summary>
            Public Property Start As Long

            ''' <summary>
            ''' The length of the match
            ''' </summary>
            ''' <returns></returns>
            Public Property Length As Long
            Public Sub New(start As Long, len As Long)
                Me.Start = start
                Me.Length = len
            End Sub
        End Structure

        ''' <summary>
        ''' All matches found so far
        ''' </summary>
        Friend Matches As List(Of Match) = New List(Of Match)

        ''' <summary>
        ''' The index of the current match
        ''' </summary>
        Friend CurMatch As Integer = 0

        Public Sub New(Tb As Scintilla)
            ' This call is required by the designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            Me.Tb = Tb
        End Sub

        Private Sub ReplaceAll(find As String, repl As String, Optional regexp As Boolean = False)
            Try
                Matches.Clear()
                If Not regexp Then find = Regex.Escape(find)
                Dim ignoreCase As Boolean = Not CbCase.Checked

                If Tb.Selections.Count > 0 AndAlso Tb.SelectionStart <> Tb.SelectionEnd Then
                    Dim sels As Selection() = Tb.Selections.ToArray()
                    For i As Integer = sels.Count - 1 To 0 Step -1
                        Dim s As Integer = sels(i).Start
                        Dim l As Integer = sels(i).End - s
                        Dim replaced As String =
                            Regex.Replace(Tb.GetTextRange(s, l), find, repl,
                                         If(ignoreCase, RegexOptions.IgnoreCase,
                                            RegexOptions.None))
                        Tb.DeleteRange(s, l)
                        Tb.InsertText(s, replaced)
                    Next
                    LbMatchCount.Text = "Selected matches replaced."
                Else
                    Tb.Text = Regex.Replace(Tb.Text, find, repl,
                                            If(ignoreCase, RegexOptions.IgnoreCase,
                                            RegexOptions.None))
                    LbMatchCount.Text = "All matches replaced."
                End If

                LbMatchCount.Show()
            Catch
                LbMatchCount.Text = "No matches found."
            End Try
        End Sub

        Private Sub FindAll(find As String, Optional regexp As Boolean = False)
            Try
                Dim escaped As String = CStr(New Text(find).Escape().GetValue())
                If Not regexp Then find = Regex.Escape(find)
                Dim lines As Integer = CInt(New CantusEvaluator.InternalFunctions(Nothing).Count(escaped, vbLf)) + 1

                Dim ignoreCase As Boolean = Not CbCase.Checked

                Matches.Clear()
                If Tb.Selections.Count > 0 AndAlso Tb.SelectionStart <> Tb.SelectionEnd Then
                    For Each sel As Selection In Tb.Selections
                        Dim matchset As MatchCollection =
                            Regex.Matches(Tb.GetTextRange(sel.Start, sel.End - sel.Start),
                                          find, If(ignoreCase, RegexOptions.IgnoreCase,
                                            RegexOptions.None))
                        For Each match As RegularExpressions.Match In matchset
                            Matches.Add(New Match(sel.Start + match.Index, match.Length))
                        Next
                    Next
                    Tb.ClearSelections()
                    For Each m As Match In Matches
                        Tb.AddSelection(CInt(m.Start + m.Length), CInt(m.Start))
                    Next
                    Tb.ScrollCaret()
                    Return
                End If

                Tb.ClearSelections()

                Dim start As Integer = Tb.CurrentLine
                Dim endline As Integer = Tb.Lines.Count
                Dim i As Integer = start

                While True
                    Dim matchTxt As String = ""
                    For j As Integer = i To i + lines - 1
                        If j <> i Then matchTxt += vbLf
                        matchTxt += Tb.Lines(j).Text
                    Next

                    Dim matchset As MatchCollection = Regex.Matches(matchTxt, find,
                                            If(ignoreCase, RegexOptions.IgnoreCase,
                                            RegexOptions.None))
                    For Each match As RegularExpressions.Match In matchset
                        Matches.Add(New Match(match.Index + Tb.Lines(i).Position, match.Length))

                        Tb.AddSelection(match.Index + Tb.Lines(i).Position + match.Length, match.Index + Tb.Lines(i).Position)
                    Next

                    i += 1
                    If i = start Then Exit While
                    If i > endline - lines Then i = 0
                    If start = i AndAlso start = 0 Then Exit While
                End While
                If Tb.Selections.Count > 0 Then
                    Tb.ScrollCaret()
                End If

                CurMatch = 0

                LbMatchCount.Text = Matches.Count & " matches found."
                LbMatchCount.Show()
            Catch
                LbMatchCount.Text = "0 matches found."
            End Try
        End Sub

        Private Sub BtnCancel_Click(sender As Object, e As EventArgs) Handles BtnCancel.Click
            Me.Close()
        End Sub

        Private Sub BtnFind_Click(sender As Object, e As EventArgs) Handles BtnFind.Click
            FindAll(TbFind.Text, CbRegex.Checked)
        End Sub

        Private Sub BtnRepl_Click(sender As Object, e As EventArgs) Handles BtnRepl.Click
            ReplaceAll(TbFind.Text, TbReplace.Text, CbRegex.Checked)
        End Sub

        Private Sub DiagFindRepl_Load(sender As Object, e As EventArgs) Handles MyBase.Load
            If My.Settings.FindReplRegex Then CbRegex.Checked = True
            If My.Settings.FindReplCase Then CbCase.Checked = True
        End Sub

        Private Sub DiagFindRepl_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
            My.Settings.FindReplRegex = CbRegex.Checked
            My.Settings.FindReplCase = CbCase.Checked
            My.Settings.Save()
        End Sub
    End Class
End Namespace