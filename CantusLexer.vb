Imports System.Text.RegularExpressions
Imports ScintillaNET

Namespace Calculator.ScintillaForCantus
    ''' <summary>
    ''' Custom lexer for the Cantus language for Scintilla.Net. Code adapted from Scintilla wiki
    ''' https://github.com/jacobslusser/ScintillaNET/wiki/Custom-Syntax-Highlighting
    ''' </summary>
    Friend Class CantusLexer
        Public Const StyleDefault As Integer = 0
        Public Const StyleKeyword As Integer = 1
        Public Const StyleIdentifier As Integer = 2
        Public Const StyleNumber As Integer = 3
        Public Const StyleString As Integer = 4
        Public Const StyleInlineKeyword As Integer = 5
        Public Const StyleComment As Integer = 6

        Private Const STATE_UNKNOWN As Integer = 0
        Private Const STATE_IDENTIFIER As Integer = 1
        Private Const STATE_NUMBER As Integer = 2
        Private Const STATE_STRING As Integer = 3
        Private Const STATE_COMMENT As Integer = 4

        Private keywords As New HashSet(Of String)(("class function namespace if else elif for in to step repeat " &
                                                "switch case run import load try catch finally while until with").Split(" "c))
        Private inlineKeywords As New HashSet(Of String)(
                                                "static let public private static global continue return break".Split(" "c))

        Public Sub Style(scintilla As Scintilla, startPos As Integer, endPos As Integer)
            ' Back up to the line start
            Dim line As Integer = scintilla.LineFromPosition(startPos)
            startPos = scintilla.Lines(line).Position

            Dim length As Integer = 0
            Dim state As Integer = STATE_UNKNOWN

            Dim c As Char = Nothing
            ' Start styling
            scintilla.StartStyling(startPos)
            While startPos < endPos
                Dim prevC As Char = c
                c = ChrW(scintilla.GetCharAt(startPos))
REPROCESS:

                Select Case state
                    Case STATE_UNKNOWN
                        If c = ControlChars.Quote Then
                            ' Start of "string"
                            scintilla.SetStyling(1, StyleString)
                            state = STATE_STRING
                        ElseIf c = "#"c Then
                            ' Start of comment
                            scintilla.SetStyling(1, StyleComment)
                            state = STATE_COMMENT
                        ElseIf [Char].IsDigit(c) OrElse c = "."c Then
                            state = STATE_NUMBER
                            GoTo REPROCESS
                        ElseIf [Char].IsLetter(c) Then
                            state = STATE_IDENTIFIER
                            GoTo REPROCESS
                        Else
                            ' Everything else
                            scintilla.SetStyling(1, StyleDefault)
                        End If
                        Exit Select

                    Case STATE_STRING
                        If c = ControlChars.Quote Then
                            length += 1
                            scintilla.SetStyling(length, StyleString)
                            length = 0
                            state = STATE_UNKNOWN
                        Else
                            length += 1
                        End If
                        Exit Select

                    Case STATE_NUMBER
                        If [Char].IsDigit(c) OrElse c = "."c Then
                            length += 1
                        Else
                            scintilla.SetStyling(length, StyleNumber)
                            length = 0
                            state = STATE_UNKNOWN
                            GoTo REPROCESS
                        End If
                        Exit Select

                    Case STATE_IDENTIFIER
                        If [Char].IsLetterOrDigit(c) OrElse c = "_"c Then
                            length += 1
                        Else
                            Dim style__1 As Integer = StyleIdentifier
                            Dim identifier As String = scintilla.GetTextRange(startPos - length, length)
                            If keywords.Contains(identifier) Then
                                style__1 = StyleKeyword
                            ElseIf inlineKeywords.Contains(identifier)
                                style__1 = StyleInlineKeyword
                            End If

                            scintilla.SetStyling(length, style__1)
                            length = 0
                            state = STATE_UNKNOWN
                            GoTo REPROCESS
                        End If
                        Exit Select
                    Case STATE_COMMENT
                        If vbCrLf.Contains(c) Then
                            scintilla.SetStyling(length, StyleComment)
                            length = 0
                            state = STATE_UNKNOWN
                            GoTo REPROCESS
                        Else
                            length += 1
                        End If
                        Exit Select
                End Select

                startPos += 1
            End While
        End Sub
    End Class
End Namespace
