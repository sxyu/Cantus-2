Imports System.Text
Imports System.Text.RegularExpressions
Imports Cantus.Core.CommonTypes
Imports Cantus.Core.Scoping
Imports Cantus.Core.CantusEvaluator
Imports ScintillaNET
Imports Cantus.Core
Imports Cantus.Core.CantusEvaluator.ObjectTypes
Imports System.Reflection

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
        ''' Style used for types
        ''' </summary>
        Public Const StyleType As Integer = 8

        ''' <summary>
        ''' Decimal point
        ''' </summary>
        Private Shared ReadOnly Property DecPt As String = Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator

        ''' <summary>
        ''' The evaluator
        ''' </summary>
        Private _eval As CantusEvaluator

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
        Private _keywords As New HashSet(Of String)(("class function namespace if else elif for" &
                                                    " in to step repeat " &
                                                    "switch case run try catch finally" &
                                                    " while until with").Split(" "c))

        ''' <summary>
        ''' A hashset of inline keywords
        ''' </summary>
        Private _inlineKeywords As New HashSet(Of String)((
                          "import load static let public private static global as is" &
                        " continue return break throw or and xor not this ans prevans then").Split(" "c))

        ''' <summary>
        ''' A hashset of internal types
        ''' </summary>
        Private _internalTypes As New HashSet(Of String)((
                          "text number date boolean matrix tuple set hashset linkedlist" &
                          " reference function").Split(" "c))

        Public Sub New(eval As CantusEvaluator)
            _eval = eval
        End Sub

        Public Sub Style(scintilla As Scintilla, startPos As Integer, endPos As Integer)
            Try
                ' Back up to the line start
                Dim line As Integer = scintilla.LineFromPosition(startPos)
                Dim lineTextSb As New StringBuilder(scintilla.Lines(line).Text)
                Dim initPos As Integer = startPos
                Dim styleText As String = scintilla.GetTextRange(startPos, endPos)

                Dim uline As Integer = line
                While uline > 0 AndAlso scintilla.Lines(uline - 1).Text.EndsWith("\")
                    uline -= 1
                    lineTextSb.Insert(0, scintilla.Lines(uline).Text.Remove(scintilla.Lines(uline).Text.Length - 2))
                End While

                Dim lineText As String = lineTextSb.ToString()

                Dim state As Integer = eState.unknown

                Dim tripleQuote As String = ControlChars.Quote & ControlChars.Quote & ControlChars.Quote
                Dim upperText As String = scintilla.GetTextRange(0, scintilla.Lines(line).Position)
                If New Core.CantusEvaluator.InternalFunctions(Nothing).Count(
                upperText, tripleQuote) Mod 2 = 1 OrElse New Core.CantusEvaluator.InternalFunctions(Nothing).Count(
                   upperText, "'") Mod 2 = 1 Then
                    state = eState.string
                Else
                    tripleQuote = "'''"
                    If New Core.CantusEvaluator.InternalFunctions(Nothing).Count(
                    upperText, tripleQuote) Mod 2 = 1 OrElse New Core.CantusEvaluator.InternalFunctions(Nothing).Count(
                       upperText, "'") Mod 2 = 1 Then
                        state = eState.string
                    End If
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
                                ' start of double-quoted string
                                scintilla.SetStyling(1, StyleString)
                                If prevC = "r"c Then scintilla.SetStyling(1, StyleString)
                                state = If(c = "'", eState.singleQuotedString, eState.string)
                            ElseIf c = "r"c Then
                                ' detect raw string qualifier (or if detected incorrectly, change after)
                                Exit Select
                            ElseIf c = "#"c Then
                                ' start of comment
                                scintilla.SetStyling(1, StyleComment)
                                state = eState.comment
                            ElseIf c = "$"c Then
                                ' silent mode
                                scintilla.SetStyling(1, StyleInlineKeyword)
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
                                    If lineText.Length > startPos - initPos + 1 Then _
                                        restOfLine = lineText.Remove(startPos - initPos + 1)

                                    If _rawString Then 'OrElse Not restOfLine.Contains(endChar) Then
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
                                If length = 0 Then scintilla.SetStyling(1, StyleIdentifier)
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
                                    identifier.ToLowerInvariant() = "undefined" OrElse identifier.ToLowerInvariant() = "null" Then
                                    identifierStyle = StyleNumberBoolean
                                ElseIf _keywords.Contains(identifier) Then
                                    identifierStyle = StyleKeyword
                                ElseIf _inlineKeywords.Contains(identifier) Then
                                    identifierStyle = StyleInlineKeyword
                                ElseIf _internalTypes.Contains(identifier.ToLowerInvariant()) OrElse
                                    _eval.HasUserClass(identifier) Then
                                    identifierStyle = StyleType
                                Else
                                    Dim restOfLine As String = lineText
                                    If lineText.Length > startPos + 1 Then restOfLine = lineText.Remove(startPos + 1).Trim()

                                    If Not restOfLine.Contains("=") Then
                                        Try
                                            Dim res As Object = _eval.EvalExprRaw(identifier, True)
                                            If (Not TypeOf res Is Double OrElse Not Double.IsNaN(CDbl(res))) AndAlso (Not TypeOf res Is BigDecimal OrElse Not DirectCast(res, BigDecimal).IsUndefined) Then
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

    ''' <summary>
    ''' Class for providing custom functionality to Scintilla
    ''' </summary>
    Friend Class ScintillaController
        ''' <summary>
        ''' The Scintilla editor
        ''' </summary>
        Private WithEvents _tb As Scintilla

        ''' <summary>
        ''' The Cantus Evaluator from which we get autocompletion info
        ''' </summary>
        Private _eval As CantusEvaluator

        ''' The offset from the start Of
        ''' the line where start braces should be added. 
        ''' This is used in consoles where the start brace must be added
        ''' after some prompt text.
        Private _lineOffset As Integer

        ''' <summary>
        ''' Custom lexer for scintilla
        ''' </summary>
        Private _lexer As CantusLexer

        Private Sub AutoCCompleted(sender As Object, e As AutoCSelectionEventArgs) Handles _tb.AutoCCompleted
            Try
                If e.Text.EndsWith("(_)") Then
                    _tb.DeleteRange(_tb.SelectionStart - 2, 1)
                    _tb.SelectionStart -= 1
                    _tb.SelectionEnd -= 1
                End If
            Catch
            End Try
        End Sub

        Private Shared Function IsBrace(c As Integer) As Boolean
            Select Case ChrW(c)
                Case "("c, ")"c, "["c, "]"c, "{"c, "}"c
                    Return True
            End Select

            Return False
        End Function
        Private lastCaretPos As Integer = 0

        ' brace pairing
        Private Sub UpdateUI(sender As Object, e As UpdateUIEventArgs) Handles _tb.UpdateUI
            ' Has the caret changed position?
            Dim caretPos As Integer = _tb.CurrentPosition
            If lastCaretPos <> caretPos Then
                lastCaretPos = caretPos
                Dim bracePos1 As Integer = -1
                Dim bracePos2 As Integer = -1

                ' Is there a brace to the left or right?
                If caretPos > 0 AndAlso IsBrace(_tb.GetCharAt(caretPos - 1)) Then
                    bracePos1 = (caretPos - 1)
                ElseIf IsBrace(_tb.GetCharAt(caretPos)) Then
                    bracePos1 = caretPos
                End If

                If bracePos1 >= 0 Then
                    ' Find the matching brace
                    bracePos2 = _tb.BraceMatch(bracePos1)
                    If bracePos2 = Scintilla.InvalidPosition Then
                        _tb.BraceBadLight(bracePos1)
                    Else
                        _tb.BraceHighlight(bracePos1, bracePos2)
                    End If
                Else
                    ' Turn off brace matching
                    _tb.BraceHighlight(Scintilla.InvalidPosition, Scintilla.InvalidPosition)
                End If
            End If
        End Sub

        ' syntax highlighting
        Private Sub Tb_StyleNeeded(sender As Object, e As StyleNeededEventArgs) Handles _tb.StyleNeeded
            Dim startPos As Integer = _tb.GetEndStyled()
            Dim endPos As Integer = e.Position
            _lexer.Style(_tb, startPos, endPos)
        End Sub

        ' autoindent
        Private Sub Tb_InsertCheck(sender As Object, e As InsertCheckEventArgs) Handles _tb.InsertCheck
            If _lineOffset > 0 Then Return
            If e.Text.EndsWith(ControlChars.Cr) OrElse e.Text.EndsWith(ControlChars.Lf) Then
                Dim curLine As Integer = _tb.LineFromPosition(e.Position)
                Dim curLineText As String = _tb.Lines(curLine).Text

                Dim indent As Match = Regex.Match(curLineText, "^\s*")
                e.Text += indent.Value


                Dim blockKwd As New HashSet(Of String)(("class function namespace if else elif for repeat " & "switch case run try catch finally while until with").Split(" "c))
                curLineText = curLineText.Trim()
                If curLineText.Contains(" ") Then curLineText = curLineText.Remove(curLineText.IndexOf(" "))

                If blockKwd.Contains(curLineText) Then e.Text += ControlChars.Tab

            End If
        End Sub

        Private Sub Tb_CharAdded(sender As Object, e As CharAddedEventArgs) Handles _tb.CharAdded
            Try

                ' autocomplete
                Dim currentPos As Integer = _tb.CurrentPosition
                Dim wordStartPos As Integer = _tb.CurrentPosition

                While wordStartPos - 1 >= 0 AndAlso (
                  _tb.GetCharAt(wordStartPos - 1) >= AscW("0"c) AndAlso _tb.GetCharAt(wordStartPos - 1) <= AscW("9"c) OrElse _tb.GetCharAt(wordStartPos - 1) >= AscW("a"c) AndAlso _tb.GetCharAt(wordStartPos - 1) <= AscW("z"c) OrElse _tb.GetCharAt(wordStartPos - 1) >= AscW("A"c) AndAlso _tb.GetCharAt(wordStartPos - 1) <= AscW("Z"c) OrElse _tb.GetCharAt(wordStartPos - 1) = AscW("_"c) OrElse _tb.GetCharAt(wordStartPos - 1) = AscW(SCOPE_SEP))
                    wordStartPos -= 1
                End While

                If _tb.GetCharAt(wordStartPos) = AscW(SCOPE_SEP) Then wordStartPos += 1

                Dim enteredWord As String = _tb.GetTextRange(wordStartPos, currentPos)
                If enteredWord.Length > 0 AndAlso Char.IsDigit(enteredWord(0)) Then Return

                Dim lenEntered As Integer = currentPos - wordStartPos

                If lenEntered > 0 Then

                    Dim curLineText As String = _tb.GetTextRange(_tb.Lines(_tb.CurrentLine).Position, _tb.CurrentPosition)

                    Dim singleQuote As Boolean = False
                    Dim doubleQuote As Boolean = False
                    For i As Integer = 0 To curLineText.Count - 1
                        If curLineText(i) = "'"c Then singleQuote = Not singleQuote
                        If curLineText(i) = """"c Then doubleQuote = Not doubleQuote
                    Next
                    If singleQuote OrElse doubleQuote Then Return ' do not autocomplete string

                    curLineText = _tb.Lines(_tb.CurrentLine).Text
                    Dim keyword As String = curLineText

                    Dim blockKwd As String() = ("class function namespace").Split(" "c)
                    keyword = keyword.Trim()
                    If keyword.Contains(" ") Then keyword = keyword.Remove(keyword.IndexOf(" "))

                    If blockKwd.Contains(keyword) Then Return ' do not autocomplete class, function, namespace names 

                    ' do not autocomplete variable declarations unless after keyword
                    If (keyword = "let" OrElse keyword = "global") AndAlso Not curLineText.Contains("=") Then Return

                    Dim keywords As String() = "function global let private public static".Split(" "c)

                    Dim autoCList As New List(Of String)

                    If keywords.Contains(keyword) AndAlso Not curLineText.Contains("=") Then
                        autoCList.AddRange(keywords)
                    Else
                        Dim nsMode As Boolean = enteredWord.Contains(SCOPE_SEP)

                        If Not nsMode Then
                            autoCList.AddRange(("class function namespace if else elif for repeat return throw continue private public " &
                                           "let static global ref deref undefined null " &
                                           "switch case run try catch finally while until then " &
                                           "with in step to choose and or xor not ans prevans as is " &
                                           "text number date boolean matrix tuple set hashset linkedlist reference function " &
                                           "true false OUTPUT ANGLEREPR SPACESPERTAB EXPLICIT SIGFIGS").Split(" "c))

                            autoCList.Add(ROOT_NAMESPACE)
                        End If

                        ' variables
                        For Each v As Variable In _eval.Variables.Values.ToArray()
                            ' ignore private
                            If v.Modifiers.Contains("hidden") OrElse (v.Modifiers.Contains("private") AndAlso
                                Not IsParentScopeOf(v.DeclaringScope, _eval.Scope)) Then Continue For

                            ' ignore null
                            If v.Value Is Nothing OrElse TypeOf v.Value Is Double AndAlso Double.IsNaN(CDbl(v.Value)) OrElse TypeOf v.Value Is BigDecimal AndAlso DirectCast(v.Value, BigDecimal).IsUndefined Then Continue For

                            If nsMode Then ' filter namespace
                                Dim partialName As String = RemoveRedundantScope(v.FullName, _eval.Scope)

                                If v.FullName.ToLower().StartsWith(enteredWord.ToLower()) Then
                                    autoCList.Add(v.FullName)

                                ElseIf partialName.ToLower().StartsWith(enteredWord.ToLower()) Then
                                    autoCList.Add(partialName)

                                ElseIf enteredWord.ToLower().StartsWith(partialName.ToLower()) OrElse enteredWord.ToLower().StartsWith(v.FullName.ToLower()) Then
                                    If enteredWord.ToLower().StartsWith(v.FullName.ToLower()) Then partialName = v.FullName
                                    If TypeOf v.Reference.Resolve() Is ClassInstance Then
                                        Dim ci As ClassInstance = DirectCast(v.Reference.Resolve(), ClassInstance)
                                        For Each f As String In ci.Fields.Keys.ToArray()
                                            autoCList.Add(CombineScope(partialName,
                                                   f & If(TypeOf ci.Fields(f).ResolveObj() Is Lambda, "(" & If(DirectCast(ci.Fields(f).ResolveObj(), Lambda).Args.Count = 0, "", "_") & ")",
                                              "")))
                                        Next
                                    End If
                                Else
                                    Continue For
                                End If
                            Else
                                Dim varn As String = RemoveRedundantScope(v.FullName, _eval.Scope)
                                autoCList.Add(varn & If(TypeOf v.Value Is Lambda, "(" & If(DirectCast(v.Value, Lambda).Args.Count = 0, "", "_") & ")", ""))
                                If varn.ToLower().StartsWith("plugin") Then
                                    autoCList.Add(varn.Substring("plugin".Length + 1) & If(TypeOf v.Value Is Lambda, "(" & If(DirectCast(v.Value, Lambda).Args.Count = 0, "", "_") & ")", ""))
                                End If
                            End If
                        Next

                        ' internal functions
                        Dim varname As String = ""
                        Dim type As Type = Nothing
                        If nsMode AndAlso
                            enteredWord.LastIndexOf(SCOPE_SEP) >= 0 AndAlso
                            _eval.HasVariable(enteredWord.Remove(enteredWord.LastIndexOf(SCOPE_SEP))) Then

                            varname = enteredWord.Remove(enteredWord.LastIndexOf(SCOPE_SEP))

                            Dim var As Object = Nothing
                            var = _eval.GetVariableObj(varname)
                            If TypeOf var Is Reference AndAlso Not TypeOf DirectCast(var, Reference).GetRefObject() Is Reference Then
                                type = DirectCast(var, Reference).Resolve().GetType
                            Else
                                type = var.GetType()
                            End If
                        End If

                        For Each fn As MethodInfo In InternalFunctions.Methods
                            If nsMode Then
                                If enteredWord.StartsWith(ROOT_NAMESPACE) Then
                                    autoCList.Add(ROOT_NAMESPACE & SCOPE_SEP & fn.Name.ToLower() &
                                          "(" & If(fn.GetParameters().Count = 0, "", "_") & ")")

                                ElseIf fn.GetParameters().Count > 0 AndAlso
                                Not String.IsNullOrEmpty(varname) AndAlso varname.EndsWith(")") OrElse
                                varname.Length > 0 Then
                                    If fn.GetParameters().Length > 0 AndAlso fn.GetParameters(0).ParameterType.IsAssignableFrom(type) Then
                                        autoCList.Add(varname & SCOPE_SEP & fn.Name.ToLower() & "(" &
                                                  If(fn.GetParameters().Count <= 1, "", "_") & ")")
                                    End If
                                End If
                            Else
                                autoCList.Add(fn.Name.ToLower() & "(" & If(fn.GetParameters().Count = 0, "", "_") & ")")
                            End If
                        Next

                        ' user functions
                        For Each fn As UserFunction In _eval.UserFunctions.Values.ToArray()
                            If nsMode Then
                                Dim fullName = fn.FullName
                                If fullName.ToLower().StartsWith("plugin.") Then fullName = fullName.Remove("plugin.".Length)
                                If Not fullName.ToLower().StartsWith(enteredWord.ToLower()) AndAlso
                                    Not fullName.ToLower().StartsWith(RemoveRedundantScope(fullName, _eval.Scope).
                                    ToLower()) Then
                                    Continue For
                                End If
                            End If
                            ' ignore private
                            If fn.Modifiers.Contains("hidden") OrElse (fn.Modifiers.Contains("private") AndAlso Not IsParentScopeOf(fn.DeclaringScope, _eval.Scope)) Then Continue For

                            If nsMode Then ' filter namespace
                                Dim partialname As String = RemoveRedundantScope(fn.FullName, _eval.Scope)
                                If fn.FullName.ToLower().StartsWith(enteredWord.ToLower()) Then
                                    autoCList.Add(fn.FullName & "(" & If(fn.Args.Count = 0, "", "_") & ")")
                                ElseIf varname.Length > 0 AndAlso Not partialname.Contains(SCOPE_SEP) Then
                                    autoCList.Add(varname + SCOPE_SEP + partialname &
                                                  "(" & If(fn.Args.Count = 0, "", "_") & ")")
                                ElseIf RemoveRedundantScope(fn.FullName, _eval.Scope).ToLower().StartsWith(enteredWord.ToLower()) Then
                                    autoCList.Add(RemoveRedundantScope(partialname, _eval.Scope) & "(" & If(fn.Args.Count = 0, "", "_") & ")")
                                Else
                                    Continue For
                                End If
                            Else
                                Dim fnName As String = RemoveRedundantScope(fn.FullName, _eval.Scope)
                                autoCList.Add(fnName & "(" & If(fn.Args.Count = 0, "", "_") & ")")
                                If fnName.ToLower().StartsWith("plugin.") Then
                                    autoCList.Add(fnName.Substring("plugin.".Length) &
                                                  "(" & If(fn.Args.Count = 0, "", "_") & ")")
                                End If
                            End If
                        Next

                        For Each uc As UserClass In _eval.UserClasses.Values.ToArray()
                            If nsMode Then
                                If Not uc.FullName.ToLower().StartsWith(enteredWord.ToLower()) AndAlso Not uc.FullName.ToLower().StartsWith(RemoveRedundantScope(uc.FullName, _eval.Scope).ToLower()) Then
                                    Continue For
                                End If
                            End If
                            ' ignore private
                            If uc.Modifiers.Contains("hidden") OrElse (uc.Modifiers.Contains("private") AndAlso Not IsParentScopeOf(uc.DeclaringScope, _eval.Scope)) Then Continue For

                            If nsMode Then ' filter namespace
                                If uc.FullName.ToLower().StartsWith(enteredWord.ToLower()) Then
                                    autoCList.Add(uc.FullName & "(" & If(uc.Constructor.Args.Count = 0, "", "_") & ")")
                                ElseIf RemoveRedundantScope(uc.FullName, _eval.Scope).ToLower().StartsWith(enteredWord.ToLower()) Then
                                    autoCList.Add(RemoveRedundantScope(uc.FullName, _eval.Scope) & "(" & If(uc.Constructor.Args.Count = 0, "", "_") & ")")
                                Else
                                    Continue For
                                End If
                            Else
                                autoCList.Add(RemoveRedundantScope(uc.FullName, _eval.Scope) & "(" & If(uc.Constructor.Args.Count = 0, "", "_") & ")")
                            End If
                        Next
                        autoCList.Sort(New AutoCompleteComparer())
                    End If

                    Dim lst As String = String.Join(" ", autoCList)
                    If String.IsNullOrWhiteSpace(lst) Then Return
                    _tb.AutoCShow(lenEntered, lst)
                End If

                ' brace completion
                If e.Char = AscW("(") OrElse e.Char = AscW("[") OrElse e.Char = AscW("{") OrElse
                   e.Char = AscW(")") OrElse e.Char = AscW("]") OrElse e.Char = AscW("}") Then

                    Dim startPos As Integer
                    Dim curLine As Integer = _tb.CurrentLine
                    Dim curText As String = _tb.Lines(curLine).Text

                    While curLine > 0 AndAlso _tb.Lines(curLine - 1).Text.EndsWith("\") ' connect \
                        curLine -= 1
                        curText = _tb.Lines(curLine).Text.Remove(_tb.Lines(curLine).Text.Length - 1) & curText
                    End While
                    startPos = _tb.Lines(curLine).Position

                    Dim startBr As Char = ChrW(e.Char)
                    Dim endBr As Char
                    Dim reverse As Boolean = False
                    Dim ct As Integer = 0

                    Select Case ChrW(e.Char)
                        Case "("c
                            endBr = ")"c
                        Case "["c
                            endBr = "]"c
                        Case "{"c
                            endBr = "}"c
                        Case ")"c
                            endBr = "("c
                            reverse = True
                        Case "]"c
                            endBr = "["c
                            reverse = True
                        Case "}"c
                            endBr = "{"c
                            reverse = True
                    End Select

                    For i As Integer = 0 To curText.Length - 1
                        If curText(i) = startBr Then ct += 1
                        If curText(i) = endBr Then ct -= 1
                    Next

                    If ct > 0 Then
                        If reverse Then
                            Dim len As Integer = _tb.CurrentPosition - startPos
                            If curText.Length > len Then curText = curText.Remove(len)

                            Dim braceList As Char() = {"["c, "("c, "{"c}
                            Dim endBraceList As Char() = {"]"c, ")"c, "}"c}
                            Dim lvl As List(Of Integer)() = {New List(Of Integer)({0}),
                            New List(Of Integer)({0}), New List(Of Integer)({0})}
                            Dim pos As Integer = 0

                            For i As Integer = 0 To curText.Length - 2
                                For j As Integer = 0 To braceList.Count - 1
                                    If braceList(j) = curText(i) Then
                                        lvl(j).Add(i + 1)
                                    ElseIf endBraceList(j) = curText(i) Then
                                        If Not lvl(j).Count <= 1 Then lvl(j).RemoveAt(lvl(j).Count - 1)
                                    End If
                                Next
                            Next

                            For j As Integer = 0 To lvl.Count - 1
                                pos = Math.Max(lvl(j)(lvl(j).Count - 1), pos)
                            Next

                            _tb.InsertText(_tb.Lines(_tb.CurrentLine).Position + _lineOffset + pos, endBr)
                        Else
                            _tb.InsertText(_tb.CurrentPosition, endBr)
                        End If
                    End If

                ElseIf e.Char = AscW("|") OrElse e.Char = AscW(""""c) OrElse e.Char = AscW("'"c) OrElse e.Char = AscW("`"c) Then
                    If e.Char = AscW(""""c) AndAlso _tb.CurrentPosition > 1 AndAlso _tb.GetTextRange(_tb.CurrentPosition - 2, 2) = """""" OrElse
                    e.Char = AscW("'"c) AndAlso _tb.CurrentPosition > 1 AndAlso _tb.GetTextRange(_tb.CurrentPosition - 2, 2) = "'" & "'" Then

                        ' if there were already two quotes before, do not add another: user probably wanted to type triple quotes
                        _tb.SelectionStart += 1
                        Return
                    End If

                    Dim curText As String = _tb.Lines(_tb.CurrentLine).Text
                    Dim ct As Boolean = False

                    For i As Integer = 0 To curText.Length - 1
                        If curText(i) = ChrW(e.Char) Then ct = Not ct
                    Next

                    If ct Then _tb.InsertText(_tb.CurrentPosition, ChrW(e.Char))

                End If
            Catch
            End Try
        End Sub

        Private Sub SetTheme()
            _tb.StyleResetDefault()

            With _tb.Styles(Style.Default)
                .BackColor = Color.FromArgb(34, 34, 34)
                .Font = "Consolas"
                .Size = 13
            End With

            _tb.SetSelectionBackColor(True, Color.GhostWhite)
            _tb.SetSelectionForeColor(True, Color.Black)

            _tb.StyleClearAll()
            _tb.WrapMode = WrapMode.Word

            _tb.Styles(CantusLexer.StyleDefault).ForeColor = Color.LightGray

            _tb.Styles(CantusLexer.StyleKeyword).ForeColor = Color.FromArgb(147, 199, 99)
            _tb.Styles(CantusLexer.StyleInlineKeyword).ForeColor = Color.FromArgb(103, 140, 177)

            _tb.Styles(CantusLexer.StyleIdentifier).ForeColor = Color.FromArgb(241, 242, 243)
            _tb.Styles(CantusLexer.StyleError).ForeColor = Color.LightGray

            _tb.Styles(CantusLexer.StyleNumberBoolean).ForeColor = Color.FromArgb(255, 205, 34)
            _tb.Styles(CantusLexer.StyleString).ForeColor = Color.FromArgb(236, 118, 0)

            _tb.Styles(CantusLexer.StyleComment).ForeColor = Color.FromArgb(153, 163, 138)
            _tb.Styles(CantusLexer.StyleType).ForeColor = Color.FromArgb(113, 175, 143)

            _tb.Lexer = Lexer.Container

            _tb.IndentWidth = 4

            With _tb.Styles(Style.LineNumber)
                .BackColor = Color.FromArgb(34, 34, 34)
                .ForeColor = Color.DarkGray
                .Size = 13
            End With

            _tb.IndentationGuides = IndentView.Real

            _tb.Styles(Style.BraceLight).ForeColor = Color.BlueViolet
            _tb.Styles(Style.BraceLight).BackColor = Color.LightGray

            _tb.Styles(Style.BraceBad).ForeColor = Color.White
            _tb.Styles(Style.BraceBad).BackColor = Color.IndianRed

            _tb.Styles(Style.IndentGuide).ForeColor = Color.Gray

            Dim margin As Margin = _tb.Margins(0)
            margin.Width = 45

            _tb.TabWidth = 4

            _tb.ScrollWidth = _tb.Width - 2 * margin.Width - 5

            _tb.AutoCIgnoreCase = True
        End Sub

        ''' <summary>
        ''' Create a new Scintilla controller. 
        ''' Immediately adds styles and events to the Scintilla editor.
        ''' </summary>
        ''' <param name="eval">The evaluator from which to get autocomplete info.</param>
        ''' <param name="lineOffset">The offset from the start of
        ''' the line where start braces should be added. 
        ''' This is used in consoles where the start brace must be added
        ''' after some prompt text.</param>
        Public Sub New(tb As Scintilla, eval As CantusEvaluator,
                       Optional lineOffset As Integer = 0)
            Me._tb = tb
            Me._eval = eval
            Me._lexer = New CantusLexer(eval)
            Me._lineOffset = lineOffset
            SetTheme()
        End Sub

    End Class
End Namespace
