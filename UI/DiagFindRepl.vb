Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Threading
Imports ScintillaNET
Imports Cantus.Core
Imports Cantus.Core.CommonTypes
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
            Matches.Clear()

            If Not regexp Then find = Regex.Escape(find)

            Tb.Text = Regex.Replace(Tb.Text, find, repl)

            LbMatchCount.Text = "All matches replaced"
            LbMatchCount.Show()
        End Sub

        Private Sub FindAll(find As String, Optional regexp As Boolean = False)
            Tb.ClearSelections()
            Matches.Clear()

            Dim escaped As String = CStr(New Text(find).Escape().GetValue())
            If Not regexp Then find = Regex.Escape(find)
            Dim lines As Integer = CInt(New CantusEvaluator.InternalFunctions(Nothing).Count(escaped, vbLf)) + 1

            Dim start As Integer = Tb.CurrentLine
            Dim endline As Integer = Tb.Lines.Count
            Dim i As Integer = start

            While True
                Dim matchTxt As String = ""
                For j As Integer = i To i + lines - 1
                    If j <> i Then matchTxt += vbLf
                    matchTxt += Tb.Lines(j).Text
                Next

                Dim matchset As MatchCollection = Regex.Matches(matchTxt, find)
                For Each match As RegularExpressions.Match In matchset
                    Matches.Add(New Match(match.Index + Tb.Lines(i).Position, match.Length))

                    Tb.AddSelection(match.Index + Tb.Lines(i).Position + match.Length, match.Index + Tb.Lines(i).Position)
                Next

                i += 1
                If i = start Then Exit While
                If i > endline - lines Then i = 0
                If start = i AndAlso start = 0 Then Exit While
            End While

            CurMatch = 0

            LbMatchCount.Text = Matches.Count & " matches found"
            LbMatchCount.Show()
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
    End Class
End Namespace