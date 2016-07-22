Imports Cantus.Calculator.Evaluator.Exceptions
Imports Cantus.Calculator.Evaluator
Imports Cantus.Calculator.Evaluator.StatementRegistar.StatementResult
Imports Cantus.Calculator.Evaluator.ObjectTypes
Imports Cantus.Calculator.Evaluator.CommonTypes

Namespace Calculator.Evaluator

    Public Class StatementRegistar
        Implements IDisposable

#Region "Statement Classes & Structs"
        ''' <summary>
        ''' Class representing a block like if or else
        ''' </summary>
        Public Class Block
            ''' <summary>
            ''' The keyword that started the block, e.g. if
            ''' </summary>
            ''' <returns></returns>
            Public Property Keyword As String

            ''' <summary>
            ''' The arguments to the block
            ''' </summary>
            ''' <returns></returns>
            Public Property Argument As String

            ''' <summary>
            ''' The content of the block as a string
            ''' </summary>
            ''' <returns></returns>
            Public Property Content As String

            Public Sub New(keyword As String, argument As String, content As String)
                Me.Keyword = keyword
                Me.Argument = argument
                Me.Content = content
            End Sub
        End Class

        ''' <summary>
        ''' Structure representing the result of processing a statement
        ''' </summary>
        Public Structure StatementResult
            Public Enum ExecCode
                ''' <summary>
                ''' continue executing script
                ''' </summary>
                [resume] = 0
                ''' <summary>
                ''' stop executing and directly return the value to the highest level
                ''' </summary>
                [return] = 1
                ''' <summary>
                ''' stop executing and continue executing the parent loop
                ''' </summary>
                [continue] = 2
                ''' <summary>
                ''' stop executing and break out of the parent loop
                ''' </summary>
                [break] = 3
                ''' <summary>
                ''' stop executing and break out of this level only 
                ''' </summary>
                [breakLevel] = 4
            End Enum

            ''' <summary>
            ''' Code determining what the evaluator does after executing this statement
            ''' </summary>
            Public ReadOnly Property Code As ExecCode

            ''' <summary>
            ''' Value representing the value obtained by executing this statement
            ''' </summary>
            Public ReadOnly Property Value As Object

            Public Sub New(value As Object, Optional code As ExecCode = ExecCode.resume)
                Me.Code = code
                Me.Value = value
            End Sub
        End Structure

        Public Class Statement
            ''' <summary>
            ''' The main keywords (e.g. if) for this statement. These keywords start off the statement. 
            ''' All keywords must be lower case.
            ''' </summary>
            ''' <returns></returns>
            Public ReadOnly Property MainKeywords As String() ' e.g. if

            ''' <summary>
            ''' The auxiliary keywords (e.g. elif, else) for this statement. These keywords continue the statement. 
            ''' All keywords must be lower case.
            ''' </summary>
            ''' <returns></returns>
            Public ReadOnly Property AuxKeywords As String() ' e.g. elif, else

            ''' <summary>
            ''' If true, this keyword will be processed as a block
            ''' </summary>
            ''' <returns></returns>
            Public ReadOnly Property BlockLevel As Boolean

            ''' <summary>
            ''' Dictionary specifying whether each key needs a argument
            ''' </summary>
            ''' <returns></returns>
            Public Property ArgumentExpected As Dictionary(Of String, Boolean)

            ''' <summary>
            ''' Process this keyword
            ''' </summary>
            ''' <returns></returns>
            Public ReadOnly Property Execute As Func(Of List(Of Block), StatementResult)

            ''' <summary>
            ''' Create a new Statement
            ''' </summary>
            ''' <param name="mainKeywords">The lmain keywords (e.g. if) for this statement. 
            ''' These keywords start off the statement. All keywords must be lower case.</param>
            ''' <param name="auxKeywords">The auxiliary keywords (e.g. elif, else) for this statement.
            ''' These keywords continue the statement. All keywords must be lower case.</param>
            ''' <param name="execute">The definition of the statement to execute when processing</param>
            '''
            ''' <param name="blockLevel">If true, allows attaching of indented blocks to this statement 
            ''' (e.g. for if statement; not required for statements like return)</param>
            ''' <param name="argumentExpected">If the value of this dictionary with 
            '''  key name equal to the block name
            '''  is false, forbids arguments to this block
            '''  (raises an error when an argument is found) for use with statements like continue, try</param>
            Public Sub New(mainKeywords As IEnumerable(Of String), auxKeywords As IEnumerable(Of String),
                           execute As Func(Of List(Of Block), StatementResult), Optional blockLevel As Boolean = True,
                           Optional argumentExpected As Dictionary(Of String, Boolean) = Nothing)
                Me.MainKeywords = mainKeywords.ToArray()
                Me.AuxKeywords = auxKeywords.ToArray()
                Me.Execute = execute
                Me.BlockLevel = blockLevel
                If argumentExpected Is Nothing Then
                    Me.ArgumentExpected = New Dictionary(Of String, Boolean)
                Else
                    Me.ArgumentExpected = argumentExpected
                End If
            End Sub
            ''' <summary>
            ''' Create a new Statement
            ''' </summary>
            ''' <param name="mainKeywords">The lmain keywords (e.g. if) for this statement. 
            ''' These keywords start off the statement. All keywords must be lower case.</param>
            ''' <param name="execute">The definition of the statement to execute when processing</param>
            '''
            ''' <param name="blockLevel">If true, allows attaching of indented blocks to this statement 
            ''' (e.g. for if statement; not required for statements like return)</param>
            ''' <param name="argumentExpected">If the value of this dictionary with 
            '''  key name equal to the block name
            '''  is false, forbids arguments to this block
            '''  (raises an error when an argument is found) for use with statements like continue, try</param>
            Public Sub New(mainKeywords As IEnumerable(Of String),
                       execute As Func(Of List(Of Block), StatementResult), Optional blockLevel As Boolean = True,
                       Optional argumentExpected As Dictionary(Of String, Boolean) = Nothing)
                Me.MainKeywords = mainKeywords.ToArray()
                Me.AuxKeywords = {}
                Me.Execute = execute
                Me.BlockLevel = blockLevel
                If argumentExpected Is Nothing Then
                    Me.ArgumentExpected = New Dictionary(Of String, Boolean)
                Else
                    Me.ArgumentExpected = argumentExpected
                End If
            End Sub
        End Class

#End Region

#Region "Variable & Const Declarations"

        ''' <summary>
        ''' Maximum length in characters of a single statement
        ''' </summary>
        Public Const MAX_STATEMENT_LENGTH As Integer = 8

        ''' <summary>
        ''' Maximum times to loop
        ''' </summary>
        Public Property LoopLimit As Integer = 10000

        ''' <summary>
        ''' If true, limits all loops to at most 10000 repetitions
        ''' </summary>
        Public Property LimitLoops As Boolean = True

        Private _eval As Evaluator
        Private _keywords As Dictionary(Of String, Statement)
        Private _mainKeywords As Dictionary(Of String, Statement)

        Private _die As Boolean = False
#End Region

#Region "Registration"

        ''' <summary>
        ''' Create a new Statement Registar for registering and accessing statements like if/else
        ''' </summary>
        Public Sub New(parent As Evaluator)
            _eval = parent
            _keywords = New Dictionary(Of String, Statement)
            _mainKeywords = New Dictionary(Of String, Statement)
            RegisterStatements()
            If Threading.Thread.CurrentThread.ManagedThreadId <> Globals.RootThreadId Then LimitLoops = False
        End Sub

        ''' <summary>
        ''' Register all statements
        ''' </summary>
        Private Sub RegisterStatements()
            ' register statements here
            ' all keywords must be lower case

            ' FORMAT Register(New Statement({[main keywords]}, {[aux keywords]},
            '                {[allowed argument counts]}, AddressOf [definition], [is block level], 
            '                 [dictionary: argument expected for each keyword?]))
            ' or     Register(New Statement({[main keywords]}, {[allowed argument counts]}, 
            '                 AddressOf [definition], [is block level], 
            '                 [dictionary: argument expected for each keyword?]))

            Register(New Statement({"if"}, {"elif", "else if", "else"}, AddressOf StatementIfElifElse))
            Register(New Statement({"while"}, AddressOf StatementWhile))
            Register(New Statement({"until"}, AddressOf StatementUntil))
            Register(New Statement({"repeat"}, AddressOf StatementRepeat))
            Register(New Statement({"run"}, AddressOf StatementRun, True,
                                   New Dictionary(Of String, Boolean) From {{"run", False}}))
            Register(New Statement({"for"}, AddressOf StatementFor))

            Register(New Statement({"try"}, {"catch", "finally"}, AddressOf StatementTryCatchFinally, True,
                       New Dictionary(Of String, Boolean) From {{"try", False},
                       {"catch", True}, {"finally", False}}))
            Register(New Statement({"with"}, AddressOf StatementWith))

            Register(New Statement({"switch"}, AddressOf StatementSwitch, True))
            Register(New Statement({"case"}, AddressOf StatementCase, True))

            Register(New Statement({"return"}, AddressOf StatementReturn, False))
            Register(New Statement({"break"}, AddressOf StatementBreak, False,
                     New Dictionary(Of String, Boolean) From {{"break", False}}))
            Register(New Statement({"continue"}, AddressOf StatementBreak, False,
                     New Dictionary(Of String, Boolean) From {{"continue", False}}))

            ' use to declare local scoped variables or override global variable names
            Register(New Statement({"let"}, AddressOf StatementDeclare, False))
            Register(New Statement({"global"}, AddressOf StatementDeclareGlobal, False))

            Register(New Statement({"function"}, AddressOf StatementDeclareFunction))
        End Sub

        ''' <summary>
        ''' Register a statement
        ''' </summary>
        Public Sub Register(statement As Statement)
            For Each kwd As String In statement.MainKeywords
                _keywords.Add(kwd, statement)
                _mainKeywords.Add(kwd, statement)
            Next
            For Each kwd As String In statement.AuxKeywords
                _keywords.Add(kwd, statement)
            Next
        End Sub
#End Region

#Region "Methods"
        ''' <summary>
        ''' Returns true if the specified keyword is registered as a main keyword
        ''' </summary>
        Public Function HasMainKeyword(kwd As String) As Boolean
            Return _mainKeywords.ContainsKey(kwd.ToLowerInvariant())
        End Function

        ''' <summary>
        ''' Returns true if the specified keyword is registered (as any type of keyword)
        ''' </summary>
        ''' <returns></returns>
        Public Function HasKeyword(kwd As String) As Boolean
            Return _keywords.ContainsKey(kwd.ToLowerInvariant())
        End Function

        ''' <summary>
        ''' Returns the statement with the keyword, or nothing if it is not registered.
        ''' </summary>
        ''' <param name="mainOnly">If true, only returns if the keyword is a main keyword</param>
        Public Function StatementWithKeyword(kwd As String, Optional mainOnly As Boolean = False) As Statement
            If HasMainKeyword(kwd) OrElse HasKeyword(kwd) AndAlso Not mainOnly Then
                Return _keywords(kwd)
            Else
                Return Nothing
            End If
        End Function

        ''' <summary>
        ''' Returns the keyword that the specified expression starts with, or blank if it does not match any statements
        ''' Also sets expr to the rest of the expression (the argument)
        ''' </summary>
        ''' <param name="mainOnly">If true, only returns if the keyword is a main keyword</param>
        Public Function KeywordFromExpr(ByRef expr As String, Optional mainOnly As Boolean = False) As String
            expr = expr.Trim()
            For i As Integer = Math.Min(expr.Length, MAX_STATEMENT_LENGTH) To 1 Step -1
                ' only try to resolve if this takes up the whole expression or ends with a space
                If i <> expr.Length AndAlso expr(i) <> " " Then Continue For
                Dim kwd As String = expr
                If i < kwd.Length Then kwd = kwd.Remove(i).Trim()
                If HasMainKeyword(kwd) OrElse HasKeyword(kwd) AndAlso Not mainOnly Then
                    expr = " " & expr.Substring(kwd.Length).Trim()
                    Return kwd.ToLowerInvariant()
                End If
            Next
            Return ""
        End Function
#End Region

#Region "Statement Declarations"

#Region "Helpers"
        ''' <summary>
        ''' Evaluates the given expression and determines if it is truthy
        ''' </summary>
        Private Function TestCond(expr As String) As Boolean
            Return _eval.InternalFunctions.IsTrue(_eval.EvalExprRaw(expr, True, True))
        End Function

        ''' <summary>
        ''' Evaluates the given script in a child evaluator and returns the result
        ''' </summary>
        Private Function Run(expr As String, Optional vars As Dictionary(Of String, Object) = Nothing,
                             Optional [default] As Reference = Nothing) As StatementResult
            Dim newEval As Evaluator = _eval.ScopedEvaluator()
            If Not vars Is Nothing Then
                For Each k As String In vars.Keys
                    newEval.SetVariable(k, vars(k))
                Next
            End If
            If Not [default] Is Nothing Then newEval.SetDefaultVariable([default])
            Return DirectCast(newEval.EvalRaw(expr, True), StatementResult)
        End Function
#End Region

        ' statement definitions here
        ' FORMAT Private Function Statement[main keywords][aux keywords](blocks As List(Of Block)) As StatementResult

#Region "Block-Level Flow Control Statements"
        Private Function StatementIfElifElse(blocks As List(Of Block)) As StatementResult
            Dim ifBlock As Block = Nothing
            Dim elifBlocks As New List(Of Block)
            Dim elseBlock As Block = Nothing
            For Each block As Block In blocks
                If block.Keyword = "if" Then
                    If ifBlock Is Nothing Then ifBlock = block
                ElseIf block.Keyword = "else" Then
                    If elseBlock Is Nothing Then elseBlock = block
                Else ' elif
                    elifBlocks.Add(block)
                End If
            Next

            If TestCond(ifBlock.Argument) Then ' original if true
                Return Run(ifBlock.Content)
            Else
                For Each elifBlock As Block In elifBlocks
                    If TestCond(elifBlock.Argument) Then ' elif true
                        Return Run(elifBlock.Content)
                    End If
                Next
                If elseBlock IsNot Nothing Then
                    Return Run(elseBlock.Content)
                End If
            End If

            Return New StatementResult(Double.NaN)
        End Function

        Private Function LoopWhile(blocks As List(Of Block), Optional until As Boolean = False) As StatementResult
            If blocks.Count <> 1 Then Throw New SyntaxException("While statement is invalid")
            Dim result As New StatementResult(Double.NaN)
            Dim ct As Integer = 0

            While If(until, Not TestCond(blocks(0).Argument), TestCond(blocks(0).Argument))
                If LimitLoops AndAlso ct > LoopLimit Then Throw New EvaluatorException("Loop limit reached")
                If _die Then Throw New EvaluatorException("")

                result = Run(blocks(0).Content)
                Select Case result.Code
                    Case ExecCode.break, ExecCode.return, ExecCode.breakLevel
                        Return New StatementResult(result.Value)
                    Case ExecCode.continue
                        Continue While
                End Select
                ct += 1
            End While
            Return result
        End Function

        Private Function StatementWhile(blocks As List(Of Block)) As StatementResult
            Return LoopWhile(blocks)
        End Function

        Private Function StatementUntil(blocks As List(Of Block)) As StatementResult
            Return LoopWhile(blocks, True)
        End Function

        Private Function StatementRepeat(blocks As List(Of Block)) As StatementResult
            Dim times As BigDecimal = BigDecimal.Undefined
            Dim obj As Object = _eval.EvalExprRaw(blocks(0).Argument, True)
            If TypeOf obj Is Double Then
                times = CDbl(obj)
            ElseIf TypeOf obj Is BigDecimal
                times = CType(obj, BigDecimal)
            End If
            If times < 1 Then Throw New EvaluatorException("Repeat statement: cannot repeat a negative number of times")
            Dim result As StatementResult = New StatementResult(Double.NaN)
            Dim ct As Integer = 0

            For i As BigDecimal = 1 To times
                If LimitLoops AndAlso ct > LoopLimit Then Throw New EvaluatorException("Loop limit reached")
                If _die Then Throw New EvaluatorException("")

                result = Run(blocks(0).Content)
                Select Case result.Code
                    Case ExecCode.break, ExecCode.return, ExecCode.breakLevel
                        Return New StatementResult(result.Value)
                    Case ExecCode.continue
                        Continue For
                End Select
                ct += 1
            Next
            Return result
        End Function

        Private Function StatementRun(blocks As List(Of Block)) As StatementResult
            If blocks.Count <> 1 Then Throw New SyntaxException("Run statement is invalid")
            Return Run(blocks(0).Content)
        End Function

        Private Function StatementFor(blocks As List(Of Block)) As StatementResult
            If blocks.Count <> 1 Then Throw New SyntaxException("For statement is invalid")

            Dim arg As String = blocks(0).Argument
            Dim result As New StatementResult(Double.NaN)
            Dim ct As Integer = 0

            If arg.ToLowerInvariant().Contains(" in ") Then
                Dim vars As String() = arg.Remove(arg.ToLowerInvariant().IndexOf(" in ")).Split(","c)
                For i As Integer = 0 To vars.Length - 1
                    vars(i) = vars(i).Trim()
                Next

                Dim lstName As Object =
                    _eval.EvalExprRaw(arg.Substring(arg.ToLowerInvariant().IndexOf(" in ") + 4), True)
                Dim lstNames As New List(Of List(Of EvalObjectBase))

                If TypeOf lstName Is IEnumerable(Of Reference) Then
                    For Each r As Reference In DirectCast(
                    lstName, IEnumerable(Of Reference))
                        lstNames.Add(New List(Of EvalObjectBase)({r.ResolveObj()}))
                    Next
                ElseIf TypeOf lstName Is IDictionary(Of Reference, Reference) Then
                    For Each k As KeyValuePair(Of Reference, Reference) In DirectCast(
                        lstName, IDictionary(Of Reference, Reference))
                        If k.Value Is Nothing Then
                            lstNames.Add(New List(Of EvalObjectBase)({k.Key.ResolveObj()}))
                        Else
                            lstNames.Add(New List(Of EvalObjectBase)({k.Key.ResolveObj(), k.Value.ResolveObj()}))
                        End If
                    Next
                Else
                    lstNames.Add(New List(Of EvalObjectBase)({DetectType(lstName)}))
                End If

                For Each lst As List(Of EvalObjectBase) In lstNames
                    If LimitLoops AndAlso ct > LoopLimit Then Throw New EvaluatorException("Loop limit reached")
                    If _die Then Throw New EvaluatorException("")

                    Dim tmpEval As Evaluator = _eval.ScopedEvaluator()
                    For i As Integer = 0 To vars.Count - 1
                        If i >= lst.Count Then
                            tmpEval.SetVariable(vars(i), Double.NaN)
                        Else
                            tmpEval.SetVariable(vars(i), lst(i))
                        End If
                    Next

                    result = DirectCast(tmpEval.EvalRaw(blocks(0).Content, True), StatementResult)

                    Select Case result.Code
                        Case ExecCode.break, ExecCode.return, ExecCode.breakLevel
                            Return New StatementResult(result.Value)
                        Case ExecCode.continue
                            Continue For
                    End Select
                    ct += 1
                Next
            ElseIf arg.ToLowerInvariant().Contains(" to ") Then
                Dim varname As String = arg.Remove(
                                      arg.ToLowerInvariant().IndexOf(" to "))

                Dim var As BigDecimal = DirectCast(
                        _eval.EvalExprRaw(varname, True), BigDecimal)

                If varname.Contains("=") Then varname = varname.Remove(varname.IndexOf("=")).Trim()

                Dim lim As BigDecimal = 0
                Dim delta As BigDecimal = 1
                If lim < var Then delta = -1

                If arg.ToLowerInvariant.Contains(" step ") AndAlso
                    arg.ToLowerInvariant().IndexOf(" to ") < arg.ToLowerInvariant().IndexOf(" step ") Then

                    delta = New Number(arg.Substring(arg.ToLowerInvariant().IndexOf(" step ") + 6)).BigDecValue()
                    lim = New Number(arg.Remove(arg.ToLowerInvariant().IndexOf(" step ")).
                                        Substring(arg.ToLowerInvariant().IndexOf(" to ") + 4)).BigDecValue()
                Else
                    lim = New Number(arg.Substring(arg.ToLowerInvariant().IndexOf(" to ") + 4)).BigDecValue()
                End If

                If delta = 0 Then Throw New SyntaxException("Step of 0 not allowed")

                For i As BigDecimal = var To lim Step delta
                    If i = lim Then Exit For ' exclusive
                    If LimitLoops AndAlso ct > LoopLimit Then Throw New EvaluatorException("Loop limit reached")
                    If _die Then Throw New EvaluatorException("")

                    Dim tmpEval As Evaluator = _eval.ScopedEvaluator()
                    tmpEval.SetVariable(varname, i)

                    result = DirectCast(tmpEval.EvalRaw(blocks(0).Content, True), StatementResult)

                    Select Case result.Code
                        Case ExecCode.break, ExecCode.return, ExecCode.breakLevel
                            Return New StatementResult(result.Value)
                        Case ExecCode.continue
                            Continue For
                    End Select
                    ct += 1
                Next
            Else
                Throw New SyntaxException("Invalid ''for'' statement syntax")
            End If
            Return result
        End Function

        Private Function StatementTryCatchFinally(blocks As List(Of Block)) As StatementResult
            Dim tryBlock As Block = Nothing
            Dim catchBlock As Block = Nothing
            Dim catchVar As String = "error"
            Dim finallyBlock As Block = Nothing
            For Each block As Block In blocks
                If block.Keyword = "try" Then
                    If tryBlock Is Nothing Then tryBlock = block
                ElseIf block.Keyword = "catch" Then
                    If catchBlock Is Nothing Then
                        catchBlock = block
                        If Not String.IsNullOrWhiteSpace(block.Argument) Then catchVar = block.Argument
                    End If
                ElseIf block.Keyword = "finally" Then
                    If finallyBlock Is Nothing Then finallyBlock = block
                End If
            Next

            Dim result As StatementResult = New StatementResult(Double.NaN)
            Try
                result = Run(tryBlock.Content)
            Catch ex As Exception
                If Not catchBlock Is Nothing Then result = Run(catchBlock.Content,
                           New Dictionary(Of String, Object) From {{catchVar, ex.Message}})
            End Try

            If Not finallyBlock Is Nothing Then result = Run(finallyBlock.Content)
            Return result
        End Function

        Private Function StatementSwitch(blocks As List(Of Block)) As StatementResult
            If blocks.Count <> 1 Then Throw New SyntaxException("Switch statement is invalid")
            Return Run(blocks(0).Content, New Dictionary(Of String, Object) From {{"__switch",
                       _eval.EvalExprRaw(blocks(0).Argument, True)}})
        End Function

        Private Function StatementCase(blocks As List(Of Block)) As StatementResult
            If blocks.Count <> 1 Then Throw New SyntaxException("Case statement is invalid")
            _eval.SetVariable("__case", _eval.EvalExprRaw(blocks(0).Argument, True))
            Dim varcheck As Object = _eval.GetVariable("__case")
            Dim varval As Object = _eval.GetVariable("__switch")
            If varcheck.GetType() = varval.GetType() AndAlso varcheck.ToString() = varval.ToString() Then
                Dim res As StatementResult = Run(blocks(0).Content)
                If res.Code = ExecCode.return Then Return res
                Return New StatementResult(res.Value, ExecCode.breakLevel)
            Else
                Return New StatementResult(Double.NaN)
            End If
        End Function

        Private Function StatementWith(blocks As List(Of Block)) As StatementResult
            If blocks.Count <> 1 Then Throw New SyntaxException("With statement is invalid")
            Return Run(blocks(0).Content, Nothing, _eval.GetVariableRef(blocks(0).Argument.Trim()))
        End Function

#End Region

#Region "Inline Flow Control Statements"
        Private Function StatementReturn(blocks As List(Of Block)) As StatementResult
            If blocks.Count <> 1 Then Throw New SyntaxException("Return statement is invalid")
            Return New StatementResult(_eval.EvalExprRaw(blocks(0).Argument, True), ExecCode.return)
        End Function

        Private Function StatementBreak(blocks As List(Of Block)) As StatementResult
            If blocks.Count <> 1 Then Throw New SyntaxException("Break statement is invalid ")
            Return New StatementResult(Double.NaN, ExecCode.break)
        End Function

        Private Function StatementContinue(blocks As List(Of Block)) As StatementResult
            If blocks.Count <> 1 Then Throw New SyntaxException("Continue statement is invalid ")
            Return New StatementResult(Double.NaN, ExecCode.continue)
        End Function
#End Region

#Region "Variable/Function Declaration Statements"
        Private Function StatementDeclare(blocks As List(Of Block)) As StatementResult
            If blocks.Count <> 1 Then Throw New SyntaxException("Variable declaration is invalid ")
            Dim var As String = blocks(0).Argument.Trim()
            Dim def As Object = Double.NaN
            If var.Contains("=") Then
                Try
                    def = ObjectTypes.DetectType(_eval.EvalExprRaw(var.Substring(var.IndexOf("="c) + 1).Trim(), True))
                Catch
                    ' do nothing
                End Try
                var = var.Remove(var.IndexOf("="c)).Trim()
                If var.EndsWith(":") Then var = var.Remove(var.Length - 1)
            End If
            var = var.Trim()
            _eval.SetVariable(var, def)
            Return New StatementResult(_eval.GetVariableRef(var).GetValue(), ExecCode.resume)
        End Function

        Private Function StatementDeclareGlobal(blocks As List(Of Block)) As StatementResult
            If blocks.Count <> 1 Then Throw New SyntaxException("Global declaration is invalid ")
            Dim var As String = blocks(0).Argument.Trim()
            Dim def As Object = Double.NaN
            If var.Contains("=") Then
                Try
                    def = ObjectTypes.DetectType(_eval.EvalExprRaw(var.Substring(var.IndexOf("="c) + 1).Trim(), True))
                Catch
                    ' do nothing
                End Try
                var = var.Remove(var.IndexOf("="c)).Trim().Trim({":"c})
                If var.EndsWith(":") Then var = var.Remove(var.Length - 1)
            End If
            var = var.Trim()

            Dim ref As Reference = DirectCast(Globals.Evaluator.GetVariableRef(var), Reference).ResolveRef()
            ref.SetValue(def)
            _eval.SetVariable(var, ref)

            Return New StatementResult(Globals.Evaluator.GetVariableRef(var), ExecCode.resume)
        End Function

        Private Function StatementDeclareFunction(blocks As List(Of Block)) As StatementResult
            _eval.AddUserFunction(blocks(0).Argument, blocks(0).Content)
            Return New StatementResult("Function " & blocks(0).Argument & " : ...", ExecCode.resume)
        End Function

        Public Sub Dispose() Implements IDisposable.Dispose
            Me._die = True
            Me._keywords.Clear()
            Me._mainKeywords.Clear()
        End Sub

        Public Sub KillAll()
            Me._die = True
            Threading.Thread.Sleep(50)
            Me._die = False
        End Sub
#End Region
#End Region

    End Class
End Namespace
