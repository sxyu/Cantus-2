Imports System.Text
Imports System.Text.RegularExpressions
Imports Cantus.Evaluator.CommonTypes
Imports ScintillaNET

Namespace UI.ScintillaForCantus
    ''' <summary>
    ''' Custom lexer for the Cantus language for Scintilla.Net. Code adapted from Scintilla wiki
    ''' https://github.com/jacobslusser/ScintillaNET/wiki/Custom-Syntax-Highlighting
    ''' </summary>
    Friend Class CantusLexer
        ''' <summary>
        ''' Default style used when none other (even identifier) is applicable.
        ''' </summary>
        Public Const StyleDefault As Integer = 0

        ''' <summary>
        ''' Style used for block-level keywords like while
        ''' </summary>
        Public Const StyleKeyword As Integer = 1

        ''' <summary>
        ''' Style used for inline keywords like let or continue
        ''' </summary>
        Public Const StyleInlineKeyword As Integer = 2

        ''' <summary>
        ''' Style used for arbitrary identifiers like abc
        ''' </summary>
        Public Const StyleIdentifier As Integer = 3

        ''' <summary>
        ''' Style used for apparently incorrect identifiers
        ''' </summary>
        Public Const StyleError As Integer = 4

        ''' <summary>
        ''' Style used for numbers
        ''' </summary>
        Public Const StyleNumberBoolean As Integer = 5

        ''' <summary>
        ''' Style used for strings (both single and double quoted as well as raw strings)
        ''' </summary>
        Public Const StyleString As Integer = 6

        ''' <summary>
        ''' Style used for comments
        ''' </summary>
        Public Const StyleComment As Integer = 7

        ''' <summary>
        ''' Decimal point
        ''' </summary>
        Private Shared ReadOnly Property DecPt As String = Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator

        ''' <summary>
        ''' The type of token we are currently styling
        ''' </summary>
        Private Enum eState
            unknown = 0
            identifier
            number
            [string]
            [singleQuotedString]
            comment
        End Enum

        ''' <summary>
        ''' Indicates if we are currently styling a raw string
        ''' </summary>
        Private _rawString As Boolean = False

        ''' <summary>
        ''' A hashset of block keywords
        ''' </summary>
        Private _keywords As New HashSet(Of String)(("class function namespace if else elif for in to step repeat " &
                                                "switch case run try catch finally while until with").Split(" "c))

        ''' <summary>
        ''' A hashset of inline keywords
        ''' </summary>
        Private _inlineKeywords As New HashSet(Of String)(
                                                "import load static let public private static global continue return break".Split(" "c))

        Public Sub Style(scintilla As Scintilla, startPos As Integer, endPos As Integer)
            Try
                ' Back up to the line start
                Dim line As Integer = scintilla.LineFromPosition(startPos)
                Dim lineTextSb As New StringBuilder(scintilla.Lines(line).Text)
                Dim initPos As Integer = startPos
                Dim styleText As String = scintilla.GetTextRange(startPos, endPos)

                Dim uline As Integer = line
                While uline > 0 AndAlso scintilla.Lines(uline - 1).Text.EndsWith(" _")
                    uline -= 1
                    lineTextSb.Insert(0, scintilla.Lines(uline).Text.Remove(scintilla.Lines(uline).Text.Length - 2))
                End While

                Dim lineText As String = lineTextSb.ToString()

                Dim state As Integer = eState.unknown

                Dim tripleQuote As String = ControlChars.Quote & ControlChars.Quote & ControlChars.Quote
                Dim upperText As String = scintilla.GetTextRange(0, scintilla.Lines(line).Position)
                If Evaluator.Globals.RootEvaluator.InternalFunctions.Count(
                upperText, tripleQuote) Mod 2 = 1 OrElse
                Evaluator.Globals.RootEvaluator.InternalFunctions.Count(
                   upperText, "'''") Mod 2 = 1 Then
                    state = eState.string
                End If

                startPos = scintilla.Lines(line).Position

                Dim length As Integer = 0
                Dim flag As Boolean = False

                Dim c As Char = Nothing

                ' Start styling
                scintilla.StartStyling(startPos)
                While startPos < endPos
                    Dim prevC As Char = c
                    c = ChrW(scintilla.GetCharAt(startPos))

                    Select Case state
                        Case eState.unknown
                            If c = ControlChars.Quote OrElse c = "'"c Then
                                ' Start of double-quoted string
                                scintilla.SetStyling(1, StyleString)
                                If prevC = "r"c Then scintilla.SetStyling(1, StyleString)
                                state = If(c = "'", eState.singleQuotedString, eState.string)
                            ElseIf c = "r"c Then
                                ' detect raw string qualifier (or if detected incorrectly, change after)
                                Exit Select
                            ElseIf c = "#"c Then
                                ' Start of comment
                                scintilla.SetStyling(1, StyleComment)
                                state = eState.comment
                            ElseIf [Char].IsDigit(c) OrElse c = DecPt Then
                                state = eState.number
                                If c = DecPt Then
                                    length = 0
                                Else
                                    If c = "0"c Then flag = True
                                    length = 1
                                    Continue While
                                End If
                            ElseIf [Char].IsLetter(c) Then
                                state = eState.identifier
                                If prevC = "r"c Then length += 1
                                Continue While
                            Else
                                ' Everything else
                                scintilla.SetStyling(1, StyleDefault)
                                'If prevC = "r"c Then scintilla.SetStyling(1, StyleDefault)
                            End If

                            Exit Select

                        Case eState.string, eState.singleQuotedString
                            If c = If(state = eState.string, ControlChars.Quote, "'") Then
                                scintilla.SetStyling(1, StyleString)
                                If prevC = "\" Then
                                    Dim endChar As Char = If(state = eState.string, ControlChars.Quote, "'"c)
                                    Dim restOfLine As String = lineText
                                    If lineText.Length > startPos - initPos + 1 Then restOfLine = lineText.Remove(startPos - initPos + 1)

                                    If _rawString OrElse prevC <> "\" OrElse Not restOfLine.Contains(endChar) Then
                                        state = eState.unknown
                                        _rawString = False
                                    End If
                                Else
                                    state = eState.unknown
                                End If
                            Else
                                scintilla.SetStyling(1, StyleString)
                            End If
                            Exit Select

                        Case eState.number
                            If [Char].IsDigit(c) OrElse c = DecPt OrElse (c = "x" AndAlso flag AndAlso length = 2) Then
                                scintilla.SetStyling(1, StyleNumberBoolean)
                                length += 1
                                If length = 1 Then scintilla.SetStyling(1, StyleNumberBoolean)
                            Else
                                length = 0
                                state = eState.unknown
                                Continue While
                            End If
                            Exit Select

                        Case eState.identifier
                            If [Char].IsLetterOrDigit(c) OrElse c = "_"c Then
                                length += 1
                            Else
                                Dim identifierStyle As Integer = StyleError
                                Dim identifier As String = scintilla.GetTextRange(startPos - length, length)

                                If identifier.ToLowerInvariant() = "true" OrElse identifier.ToLowerInvariant() = "false" OrElse
                                 identifier.ToLowerInvariant() = "undefined" Then
                                    identifierStyle = StyleNumberBoolean
                                ElseIf _keywords.Contains(identifier) Then
                                    identifierStyle = StyleKeyword
                                ElseIf _inlineKeywords.Contains(identifier)
                                    identifierStyle = StyleInlineKeyword
                                Else
                                    Dim restOfLine As String = lineText
                                    If lineText.Length > startPos + 1 Then restOfLine = lineText.Remove(startPos + 1).Trim()

                                    If Not restOfLine.Contains("=") Then
                                        Try
                                            Dim res As Object = Evaluator.Globals.RootEvaluator.EvalExprRaw(identifier, True)
                                            If (Not TypeOf res Is Double OrElse Not Double.IsNaN(CDbl(res))) AndAlso
                                       (Not TypeOf res Is BigDecimal OrElse Not DirectCast(res, BigDecimal).IsUndefined) Then
                                                identifierStyle = StyleIdentifier
                                            End If
                                        Catch ' do nothing, display error style
                                        End Try
                                    End If
                                End If

                                scintilla.SetStyling(length, identifierStyle)
                                length = 0
                                state = eState.unknown
                                Continue While
                            End If
                            Exit Select

                        Case eState.comment
                            If vbCrLf.Contains(c) Then
                                length = 0
                                state = eState.unknown
                                Continue While
                            Else
                                scintilla.SetStyling(1, StyleComment)
                            End If
                            Exit Select
                    End Select

                    startPos += 1
                End While
            Catch
            End Try
        End Sub
    End Class

    ''' <summary>
    ''' Comparer used for the autoComplete
    ''' </summary>
    Friend Class AutoCompleteComparer
        Implements IComparer(Of String)
        Public Function Compare(x As String, y As String) As Integer Implements IComparer(Of String).Compare
            Return String.Compare(x, y, StringComparison.OrdinalIgnoreCase)
        End Function
    End Class
End Namespace
