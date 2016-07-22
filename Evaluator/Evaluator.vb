Imports System.Collections.ObjectModel

Imports System.Text
Imports System.Threading
Imports Cantus.Calculator.Evaluator.CommonTypes
Imports Cantus.Calculator.Evaluator.Exceptions
Imports Cantus.Calculator.Evaluator.StatementRegistar

Namespace Calculator.Evaluator
    Public Module Globals
        Friend ReadOnly Property Evaluator As New Evaluator()
        Friend ReadOnly Property RootThreadId As Integer = Thread.CurrentThread.ManagedThreadId
    End Module

    Public Class Evaluator
        Implements IDisposable
#Region "Enums"
        Public Enum AngleRep
            Degree = 0
            Radian
            Gradian
        End Enum
        Public Enum IOMode
            LineO = 0
            MathO
            SciO
        End Enum
#End Region

#Region "Declarations"
#Region "Constants"
        ''' <summary>
        ''' Comment pattern: The (script) evaluator ignores all characters in each line after this pattern is found
        ''' </summary>
        Public Const COMMENT_START_PTN As String = "#"

        ''' <summary>
        ''' The default variable name used when none is specified (when using self-referring functions)
        ''' </summary>
        Public Const DEFAULT_VAR_NAME As String = "this"
#End Region
#Region "Variables"
        ' modes
        ''' <summary>
        ''' The output mode of the evaluator
        ''' </summary>
        ''' <returns></returns>
        Public Property OMode As IOMode

        ''' <summary>
        ''' The angle representation mode of the evaluator (radians, degrees, gradians)
        ''' </summary>
        ''' <returns></returns>
        Public Property AngleRepMode As AngleRep

        ''' <summary>
        ''' The number of spaces that would represent a tab. Default is 4.
        ''' </summary>
        ''' <returns></returns>
        Public Property SpacesPerTab As Integer

        ''' <summary>
        ''' A list of previous answers (last item is the last answer)
        ''' </summary>
        Public ReadOnly Property PrevAns As New List(Of ObjectTypes.EvalObjectBase)

        ' composition
        ''' <summary>
        ''' Object for registering and accessing operators (like +, -, *) usable in the evaluator
        ''' </summary>
        Friend ReadOnly Property OperatorRegistar As OperatorRegistar

        ''' <summary>
        ''' Object for registering and accessing statements (like if, else, while, function) usable in the evaluator
        ''' </summary>
        Friend ReadOnly Property StatementRegistar As StatementRegistar

        ''' <summary>
        ''' Object containing pre-defined functions usable in the evaluator
        ''' </summary>
        Friend ReadOnly Property InternalFunctions As InternalFunctions

        ''' <summary>
        ''' Dictionary for storing variables
        ''' </summary>
        Private _vars As New Dictionary(Of String, ObjectTypes.Reference)

        ''' <summary>
        ''' Records if the variable with the name is declared in an outside file
        ''' </summary>
        Private _varExtern As New Dictionary(Of String, Object)
        ''' <summary>
        ''' List of user function definitions
        ''' </summary>
        Private _userFunctions As New Dictionary(Of String, String)

        ''' <summary>
        ''' List of argument names of user functions
        ''' </summary>
        Private _userFunctionArgs As New Dictionary(Of String, List(Of String))

        ''' <summary>
        ''' Records if the user function with the name is declared in an outside file
        ''' </summary>
        Private _userFunctionExtern As New Dictionary(Of String, Boolean)

        ''' <summary>
        ''' Get the line number the evaluator started from
        ''' </summary>
        Private _baseLine As Integer

        ''' <summary>
        ''' Get the line number the evaluator is currently processing
        ''' </summary>
        Private _curLine As Integer

        ''' <summary>
        ''' If true, all newly defined functions should be considered outside declarations
        ''' </summary>
        Private _externalExec As Boolean
#End Region

#Region "Events"
        Public Event EvalComplete(sender As Object, result As Object)
#End Region

#Region "Evaluator Constants"
        ''' <summary>
        ''' List of predefined constants, format: {{name 1, name 2, value},}
        ''' By default this includes some often used math, physics, and chemistry constants. 
        ''' </summary>
        ''' <returns></returns>
        Private Shared ReadOnly Property _default As Object(,) =
        {
            {"e", Nothing, Math.E},
            {"pi", "π", Math.PI},
            {"phi", "φ", 1.61803398875},
            {"avogadro", "A", 6.0221409E+23},
            {"G", "gravity", 0.0000000000667408},
            {"g", Nothing, 9.807},
            {"i", "imaginaryone", Numerics.Complex.ImaginaryOne},
            {"c", "lightspeed", 299792458.0},
            {"h", "planck", 6.6260755E-34},
            {"hbar", "planckreduced", 1.05457266E-34},
            {"e0", "permittivity", 0.000000000008854187817},
            {"mu0", "permeability", 4.0 * Math.PI * 0.0000001},
            {"F", "faraday", 96485.3329},
            {"me", "electronmass", 9.10938356E-31},
            {"mp", "protonmass", 1.6726219E-27},
            {"q", "elemcharge", 1.60217662E-19},
            {"soundspeed", "vs", 343.2},
            {"R", "gas", 8.3144598},
            {"cmperinch", Nothing, 2.54},
            {"torrsperatm", Nothing, 760.0},
            {"torrsperkpa", Nothing, 7.50062}
        }

        ''' <summary>
        ''' Reload the default constants into variable storage (accessible via Reload() in execution)
        ''' </summary>
        Public Sub ReloadDefault()
            For i As Integer = 0 To _default.GetLength(0) - 1
                For j As Integer = 0 To _default.GetLength(1) - 2
                    If Not _default(i, j) Is Nothing Then
                        SetVariable(_default(i, j).ToString(), _default(i, _default.GetLength(1) - 1))
                    End If
                Next
            Next
        End Sub
#End Region
#End Region

#Region "Evaluator Logic"

        ''' <summary>
        ''' Represents a segment in the expression obtained after it is tokenized, consisting of 
        ''' an object and an operator
        ''' If either is unavailable they are set to null.
        ''' </summary>
        Private Class Token
            Public [Object] As ObjectTypes.EvalObjectBase
            Public [Operator] As OperatorRegistar.Operator
            Public Sub New(evalObject As ObjectTypes.EvalObjectBase, operatorBefore As OperatorRegistar.Operator)
                Me.Object = evalObject
                Me.Operator = operatorBefore
            End Sub
        End Class

        ''' <summary>
        ''' A special data structure used to store tokens
        ''' Allows for indexing, removal, appending, lookup of tokens with a certain precedence
        ''' </summary>
        Private Class TokenList
            Implements IEnumerator
            Implements IEnumerable
            ''' <summary>
            ''' A list of objects used with _operators to store tokens
            ''' </summary>
            Private _objects As New List(Of ObjectTypes.EvalObjectBase)
            ''' <summary>
            ''' A list of operators used with _objects to store tokens
            ''' </summary>
            Private _operators As New List(Of OperatorRegistar.Operator)
            ''' <summary>
            ''' A list of operator signs
            ''' </summary>
            Private _signs As New List(Of String)
            ''' <summary>
            ''' A list of integers, one for each index in the list, indicating what it points to. Updated by Resolve.
            ''' </summary>
            Private _pointers As New List(Of Integer)
            ''' <summary>
            ''' A list of sorted sets storing operators at each precedence level
            ''' </summary>
            Private _opsByPrecedence As New List(Of SortedSet(Of Integer))
            ''' <summary>
            ''' Private variable storing the number of remaining tokens still pointing at themselves.
            ''' </summary>
            Private _validCount As Integer = 0
            ''' <summary>
            ''' The position of the enumerator
            ''' </summary>
            Private _position As Integer = 0
            ''' <summary>
            ''' Create a new TokenList for storing tokens.
            ''' </summary>
            Public Sub New()
                For Each i As Integer In [Enum].GetValues(GetType(OperatorRegistar.Precedence))
                    _opsByPrecedence.Add(New SortedSet(Of Integer))
                Next
            End Sub

            Default Public Property Item(ByVal index As Integer) As Token
                Get
                    Return New Token(_objects(Resolve(index)), _operators(Resolve(index)))
                End Get
                Set(value As Token)
                    _objects(_pointers(index)) = value.Object
                    _operators(Resolve(index)) = value.Operator
                    _signs(Resolve(index)) = value.Operator.Signs(0)
                    If Not value.Operator Is Nothing Then
                        _opsByPrecedence(value.Operator.Precedence).Add(Resolve(index))
                    End If
                End Set
            End Property

            Public Function MoveNext() As Boolean Implements IEnumerator.MoveNext
                _position += 1
                Return (_position < Me.Count)
            End Function

            Public Sub Reset() Implements IEnumerator.Reset
                _position = -1
            End Sub

            Public ReadOnly Property Current() As Object Implements IEnumerator.Current
                Get
                    Return Me.Item(_position)
                End Get
            End Property

            Public Function GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
                Return CType(Me, IEnumerator)
            End Function

            ''' <summary>
            ''' The number of TokenList objects not currently marked as removed (not pointing at another object)
            ''' </summary>
            ''' <returns></returns>
            Public ReadOnly Property ValidCount As Integer
                Get
                    Return _validCount
                End Get
            End Property

            ''' <summary>
            ''' The total number of indices in the TokenList
            ''' </summary>
            ''' <returns></returns>
            Public ReadOnly Property Count As Integer
                Get
                    Return Math.Min(_operators.Count, _objects.Count)
                End Get
            End Property

            Public ReadOnly Property Capacity As Integer
                Get
                    Return Count
                End Get
            End Property

            ''' <summary>
            ''' Balance the operator and object lists
            ''' </summary>
            Public Sub BalanceLists()
                While _operators.Count > _objects.Count
                    _objects.Add(Nothing)
                End While
                While _operators.Count < _objects.Count
                    _operators.Add(Nothing)
                    _signs.Add(Nothing)
                End While
                While _pointers.Count < _objects.Count
                    _pointers.Add(_pointers.Count)
                End While
            End Sub

            ''' <summary>
            ''' Remove the token at the specified index by pointing it at the previous index
            ''' </summary>
            ''' <param name="idx"></param>
            Public Sub RemoveAt(idx As Integer)
                idx = Resolve(idx)
                _opsByPrecedence(_operators(idx).Precedence).Remove(idx)
                _pointers(idx) = _pointers(idx - 1)
                _validCount -= 1
            End Sub

            ''' <summary>
            ''' The total number of operators in this TokenList
            ''' </summary>
            ''' <returns></returns>
            Public ReadOnly Property OperatorCount() As Integer
                Get
                    Return _operators.Count
                End Get
            End Property
            ''' <summary>
            ''' The number of items in the object list
            ''' </summary>
            ''' <returns></returns>
            Public ReadOnly Property ObjectCount() As Integer
                Get
                    Return _objects.Count
                End Get
            End Property

            ''' <summary>
            ''' Get the operator at the specified index
            ''' </summary>
            ''' <param name="idx"></param>
            ''' <returns></returns>
            Public Function OperatorAt(idx As Integer) As OperatorRegistar.Operator
                Return _operators(Resolve(idx))
            End Function

            ''' <summary>
            ''' Get the sign of the operator at the specified index
            ''' </summary>
            ''' <param name="idx"></param>
            ''' <returns></returns>
            Public Function OperatorSignAt(idx As Integer) As String
                Return _signs(Resolve(idx))
            End Function

            ''' <summary>
            ''' Get the number of token indices with the precedence specified
            ''' </summary>
            ''' <param name="prec">The precedence</param>
            ''' <returns></returns>
            Public Function OperatorsWithPrecedenceCount(prec As OperatorRegistar.Precedence) As Integer
                Return _opsByPrecedence(prec).Count
            End Function
            ''' <summary>
            ''' Get a list of token indecess with the precedence specified
            ''' </summary>
            ''' <param name="prec"></param>
            ''' <returns></returns>
            Public Function OperatorsWithPrecedence(prec As OperatorRegistar.Precedence) As List(Of Integer)
                Return New List(Of Integer)(_opsByPrecedence(prec))
            End Function

            ''' <summary>
            ''' Checks if the token is marked as removed (i.e. it points to another item other than itself)
            ''' </summary>
            ''' <param name="idx">The index</param>
            ''' <returns></returns>
            Public Function IsRemoved(idx As Integer) As Boolean
                Return _pointers(idx) <> idx
            End Function

            ''' <summary>
            ''' Lookup the read index of the item
            ''' </summary>
            ''' <param name="idx">The index</param>
            ''' <returns></returns>
            Public Function Resolve(idx As Integer) As Integer
                Dim updatelst As New List(Of Integer)
                While (_pointers(idx) <> idx)
                    updatelst.Add(idx)
                    idx = _pointers(idx)
                End While
                For Each i As Integer In updatelst
                    _pointers(i) = idx
                Next
                Return idx
            End Function

            ''' <summary>
            ''' Add a new operator
            ''' </summary>
            ''' <param name="op">The operator</param>
            ''' <param name="sign">The sign of the operator 
            ''' (if not specified, the first one froom the operator object is used)</param>
            Public Sub AddOperator(op As OperatorRegistar.Operator, Optional sign As String = "")
                If Not op Is Nothing Then
                    _opsByPrecedence(op.Precedence).Add(_operators.Count)
                End If
                _operators.Add(op)
                If sign = "" Then
                    If Not op Is Nothing Then
                        _signs.Add(op.Signs(0))
                    Else
                        _signs.Add(Nothing)
                    End If
                Else
                    _signs.Add(sign)
                End If
                While _pointers.Count < _operators.Count
                    _pointers.Add(_pointers.Count)
                    _validCount += 1
                End While
            End Sub
            ''' <summary>
            ''' Set the operator at the index specified
            ''' </summary>
            ''' <param name="idx">The index at which to change the operator</param>
            ''' <param name="op">The operator</param>
            ''' <param name="sign">The sign of the operator </param>
            Public Sub SetOperator(idx As Integer, op As OperatorRegistar.Operator, Optional sign As String = "")
                idx = Resolve(idx)

                ' modify precedence lists
                If Not _operators(idx) Is Nothing Then
                    _opsByPrecedence(_operators(idx).Precedence).Remove(idx)
                End If

                _opsByPrecedence(op.Precedence).Add(idx)
                _operators(idx) = op

                If sign = "" Then
                    If Not op Is Nothing Then
                        _signs(idx) = op.Signs(0)
                    Else
                        _signs(idx) = Nothing
                    End If
                Else
                    _signs(idx) = sign
                End If
            End Sub

            ''' <summary>
            ''' Get the object at the specified index
            ''' </summary>
            ''' <param name="idx">The index</param>
            ''' <returns></returns>
            Public Function ObjectAt(idx As Integer) As ObjectTypes.EvalObjectBase
                Return _objects(Resolve(idx))
            End Function

            ''' <summary>
            ''' Set the object at the specified index
            ''' </summary>
            ''' <param name="idx">The index at which to set the object</param>
            ''' <param name="obj">The object</param>
            Public Sub SetObject(idx As Integer, obj As ObjectTypes.EvalObjectBase)
                _objects(Resolve(idx)) = obj
            End Sub

            ''' <summary>
            ''' Set the object at the specified index
            ''' </summary>
            ''' <param name="obj">The object</param>
            Public Sub AddObject(obj As ObjectTypes.EvalObjectBase)
                _objects.Add(obj)
                While _pointers.Count < _objects.Count
                    _pointers.Add(_pointers.Count)
                    _validCount += 1
                End While
            End Sub

            ''' <summary>
            ''' Converts the list to a human-readable string.
            ''' Each list represents a token, with
            ''' the first row indicating if the token is NOT removed (pointing at another token), 
            ''' the second indicating the operator, and the third the object.
            ''' The final row contains a count of remaining valid (not removed) objects.
            ''' </summary>
            ''' <returns></returns>
            Public Overrides Function ToString() As String
                Dim str As New StringBuilder("OPER" & vbTab)
                For i As Integer = 0 To Me.OperatorCount - 1

                    If Me.IsRemoved(i) Then Continue For

                    If i > 0 Then str.Append(vbTab)

                    If Me.OperatorAt(i) Is Nothing Then
                        str.Append("Null")
                    Else
                        str.Append(Me.OperatorAt(i).Signs(0))
                    End If

                Next

                str.Append(vbCrLf & "VALUE" & vbTab)
                For i As Integer = 0 To Me.ObjectCount - 1

                    If Me.IsRemoved(i) Then Continue For
                    If i > 0 Then str.Append(vbTab)

                    If Me.ObjectAt(i) Is Nothing Then
                        str.Append("Null")
                    Else
                        str.Append(Me.ObjectAt(i).ToString())
                    End If

                Next
                str.Append(vbCrLf & "TOTAL ITEMS " & vbTab & Me.ValidCount)
                Return str.ToString()
            End Function
        End Class

        ''' <summary>
        ''' Not publicly accessible, for internal initialization only
        ''' </summary>
        Private Sub New()
            Me.InternalFunctions = New InternalFunctions(Me)
            Me.OperatorRegistar = New OperatorRegistar(Me)
            Me.StatementRegistar = New StatementRegistar(Me)
            Me._baseLine = 0
            Me._curLine = 0
        End Sub

        ''' <summary>
        ''' Create a new Evaluator for evaluating mathematical expressions &amp; .can scripts
        ''' </summary>
        ''' <param name="oMode">The output mode of this evaluator. E.g.: MathO: 0.5->1/2; SciO: 0.5->5 E -1; LineO: 0.5->0.500</param>
        ''' <param name="angleRepMode">The angle representation mode of this evaluator (Radians, Degrees, etc.)</param>
        ''' <param name="spacesPerTab">The number of spaces per tab, default is 4</param>
        ''' <param name="prevAns">List of previous answers</param>
        ''' <param name="vars">Dictioanry of variable definitions</param>
        ''' <param name="userFunctions">Dictioanry of user function definitions</param>
        ''' <param name="userFunctionArgs">Dictioanry of user function arguments</param>
        Public Sub New(Optional oMode As IOMode = IOMode.MathO,
                       Optional angleRepMode As AngleRep = AngleRep.Radian,
                       Optional spacesPerTab As Integer = 4,
                       Optional prevAns As List(Of ObjectTypes.EvalObjectBase) = Nothing,
                       Optional vars As Dictionary(Of String, ObjectTypes.Reference) = Nothing,
                       Optional userFunctions As Dictionary(Of String, String) = Nothing,
                       Optional userFunctionArgs As Dictionary(Of String, List(Of String)) = Nothing,
                       Optional baseLine As Integer = 0)
            Me.New()
            Me.OMode = oMode
            Me.AngleRepMode = angleRepMode
            Me.SpacesPerTab = spacesPerTab

            If Not prevAns Is Nothing Then Me.PrevAns = prevAns
            If Not vars Is Nothing Then Me._vars = vars
            If Not userFunctions Is Nothing Then Me._userFunctions = userFunctions
            If Not userFunctionArgs Is Nothing Then Me._userFunctionArgs = userFunctionArgs

            Me._baseLine = baseLine
            Me._curLine = baseLine
        End Sub

        ''' <summary>
        ''' Evauate a multi-line script and return the result as a string
        ''' </summary>
        ''' <param name="script">The script to evaluate</param>
        Public Function Eval(script As String) As String
            Return InternalFunctions.O(EvalRaw(script))
        End Function

        ''' <summary>
        ''' Evauate the file at the path
        ''' </summary>
        ''' <param name="path">Path of the script to evaluate</param>
        ''' <param name="asInternal">if true, the script is exceuted as if it were in the evaluator.</param>
        Public Function Include(path As String, Optional asInternal As Boolean = False) As String
            If Not asInternal Then _externalExec = True
            Dim res As String = InternalFunctions.O(EvalRaw(IO.File.ReadAllText(path)))
            Return res
            If Not asInternal Then _externalExec = False
        End Function

        ''' <summary>
        ''' Evauate a multi-line script asynchroneously and raises the EvalComplete event when done
        ''' </summary>
        ''' <param name="script">The script to evaluate</param>
        Public Sub EvalAsync(script As String)
            Dim th As New Thread(Sub()
                                     Try
                                         RaiseEvent EvalComplete(Me, Eval(script))
                                     Catch ex As Exception
                                         RaiseEvent EvalComplete(Me, ex.Message.Trim())
                                     End Try
                                 End Sub)
            th.Start()
        End Sub

        ''' <summary>
        ''' Evauate a multi-line script and return the result as a system object
        ''' </summary>
        ''' <param name="script">The script to evaluate</param>
        ''' <param name="internal">If true, the evaluator does not save answers and returns a StatementResult
        '''  (for internal evaluations)</param>
        Public Function EvalRaw(script As String, Optional ByVal internal As Boolean = False) As Object
            Dim lineNum As Integer = 0
            Dim fullLine As String = ""
            Try
                Dim lines As String() = script.Replace(vbCrLf, vbLf).Split({ControlChars.Cr, ControlChars.Lf})
                If lines.Length = 0 Then Return Double.NaN

                ' initial indentation level
                Dim rootIndentLevel As Integer = -1

                Dim curSM As Statement = Nothing ' current statement (if)
                Dim curBlock As Block = Nothing ' current block within the statement (if, elif, elif, else)
                Dim nextBlock As Block = Nothing ' next block in chain when 'then' chaining is used (run then while x<3000) 
                Dim curBlockBegin As Integer = 0 ' the line at which the current block began at
                Dim curBlockLst As New List(Of Block) ' list of blocks for this statement
                Dim curBlockInner As New StringBuilder() ' the content of the block
                Dim curBlockIndent As Integer = rootIndentLevel ' the indentation level of this block
                Dim lastVal As Object = Double.NaN ' the last value we got in an evaluation, returned if no return statement is found

                For lineNum = 0 To lines.Length ' last line is an extra blank line to end blocks
                    fullLine = ""

                    If lineNum < lines.Length Then
                        fullLine = lines(lineNum)

                        ' remove comments
                        If fullLine.Contains(COMMENT_START_PTN) Then
                            fullLine = fullLine.Remove(fullLine.IndexOf(COMMENT_START_PTN))
                        End If

                        ' ignore blank lines
                        If String.IsNullOrWhiteSpace(fullLine) Then
                            If Not curBlock Is Nothing Then
                                curBlockInner.Append(vbLf)
                            End If
                            Continue For
                        End If

                        ' while line ends with " _" then keep joining the next line
                        While fullLine.TrimEnd().EndsWith(" _")
                            lineNum += 1
                            fullLine = fullLine.TrimEnd().TrimEnd("_"c).TrimEnd() & lines(lineNum)
                        End While


                        ' update global line number (only update for non-blank lines and does not update within blocks)
                        If curBlock Is Nothing Then Me._curLine = Me._baseLine + lineNum
                    End If

                    Dim indent As Integer = LineIndentLevel(fullLine)
                    If rootIndentLevel < 0 Then rootIndentLevel = indent

                    ' allow inline expressions with ; (assume same indent level)
                    Dim spl As String() = fullLine.Split({";"c})

                    For i As Integer = 0 To spl.Count - 1
                        Dim l As String = spl(i)
                        If String.IsNullOrWhiteSpace(l) AndAlso lineNum < lines.Length AndAlso i > 0 Then Continue For ' skip empty

                        If Not curSM Is Nothing AndAlso Not curBlock Is Nothing Then
                            ' if we are appending to a statement block
                            If curBlockIndent = -1 Then
                                If indent <= rootIndentLevel Then
                                    ' go back and cancel this block
                                    curBlockLst.Add(curBlock)
                                    lineNum -= 1
                                    curBlock = Nothing
                                    Continue For

                                    'If indent <= rootIndentLevel Then Throw New EvaluatorException(
                                    '"Higher indentation level expected")
                                Else
                                    _curLine += 1 ' increment current global line to first line of block 
                                End If

                                ' Set indentation level
                                curBlockIndent = indent
                            End If

                            If indent < curBlockIndent Then ' end of block found
                                curBlock.Content = curBlockInner.ToString().TrimEnd()

                                ' 'Then' chaining
                                nextBlock = Nothing
                                If curBlock.Argument.ToLowerInvariant().Contains(" then ") Then
                                    Dim right As String = curBlock.Argument.
                                        Substring(curBlock.Argument.ToLowerInvariant().IndexOf(" then ") + 6)

                                    curBlock.Argument = curBlock.Argument.
                                        Remove(curBlock.Argument.ToLowerInvariant().IndexOf(" then "))

                                    Dim newKwd As String = StatementRegistar.KeywordFromExpr(right)
                                    nextBlock = New Block(newKwd, right, "")
                                End If

                                ' Add this block to the list of blocks for this statement
                                curBlockLst.Add(curBlock)

                                ' Clear to make new block
                                curBlock = Nothing

                            Else ' just append the line and skip the rest
                                If curBlockInner.Length > 0 AndAlso
                                    Not (curBlockInner.Length = 1 AndAlso curBlockInner(0) = vbLf) Then curBlockInner.Append(vbLf) ' new line
                                ' if we are not at the end append this line to the block
                                curBlockInner.Append(l)
                                Continue For ' do not execute the rest of the code
                            End If
                        End If

                        If indent > rootIndentLevel Then Throw New SyntaxException("Invalid indentation (Check if tabs/spaces match: 1 tab=" &
                                                                                   SpacesPerTab & " spaces (change with SpacesPerTab())")
                        Dim kwd As String = ""

                        ' if we just finished executing the block, check if there is a continuation e.g. else for an if block
                        If Not curSM Is Nothing Then
                            kwd = StatementRegistar.KeywordFromExpr(l)

                            If kwd = "" OrElse Not curSM.AuxKeywords.Contains(kwd) Then ' found end of statement
                                If curBlockLst.Count > 0 Then ' don't execute if there are no blocks at all
                                    ' inline operators: actually on the previous line
                                    If Not curSM.BlockLevel OrElse lineNum <= curBlockBegin + 1 Then _curLine -= 1
                                    Dim res As StatementResult = curSM.Execute()(curBlockLst) ' process the statement 

                                    If internal Then
                                        Select Case res.Code
                                            Case StatementResult.ExecCode.break, StatementResult.ExecCode.continue,
                                             StatementResult.ExecCode.return
                                                ' return the code with the value
                                                Return res

                                            Case StatementResult.ExecCode.breakLevel
                                                ' break level and resume
                                                Return New StatementResult(res.Value, StatementResult.ExecCode.resume)

                                            Case Else ' if Resume
                                                ' continue executing otherwise 
                                                lastVal = res.Value
                                        End Select
                                    Else
                                        ' if we're at the top (non-internal) level and get the return code, directly return the value
                                        If res.Code = StatementResult.ExecCode.return Then
                                            Return res.Value
                                        ElseIf Not res.Code = StatementResult.ExecCode.resume
                                            Throw New SyntaxException("Continue and break statements may only be used within loops")
                                        End If

                                        ' other codes are invalid at this level
                                        lastVal = res.Value
                                    End If
                                End If

                                curBlockLst.Clear()
                                curSM = StatementRegistar.StatementWithKeyword(kwd, True)

                                ' then chaining
                                If Not nextBlock Is Nothing Then
                                    ' set block and go back
                                    curBlock = nextBlock
                                    curSM = StatementRegistar.StatementWithKeyword(curBlock.Keyword, True)
                                    lineNum = curBlockBegin
                                    curBlockInner.Clear()
                                    Continue For
                                End If
                            End If
                        Else
                            ' see if the current line matches any statement 
                            ' note that this function also modifies l so that the keyword is removed (becomes the argument)
                            kwd = StatementRegistar.KeywordFromExpr(l)
                            curSM = StatementRegistar.StatementWithKeyword(kwd, True)
                        End If

                        If Not curSM Is Nothing Then ' if matches

                            ' check if we have an argument that we are not supposed to have
                            If curSM.ArgumentExpected.ContainsKey(kwd) AndAlso
                                Not curSM.ArgumentExpected(kwd) AndAlso Not String.IsNullOrWhiteSpace(l) AndAlso
                                Not l.TrimStart().ToLowerInvariant().StartsWith("then ") Then
                                Throw New EvaluatorException("''" & curSM.MainKeywords(0) &
                                                             "'' statement does not accept any arguments")
                            End If

                            curBlock = New Block(kwd, l, "")
                            If curSM.BlockLevel Then
                                ' make a new block object out of the keyword and the argument
                                ' if this is a block level statement, start a new block
                                curBlockIndent = -1 ' expect a higher indentation level ( automatically set on next iteration )
                                curBlockInner = New StringBuilder()
                                curBlockBegin = lineNum
                            Else
                                ' if this is an inline statement, we don't expect a block, so we use an empty one
                                curBlockLst.Add(curBlock)
                                curBlock = Nothing
                            End If
                        Else
                            If StatementRegistar.HasKeyword(kwd) Then
                                Dim tmp As String() = StatementRegistar.StatementWithKeyword(kwd).MainKeywords
                                Throw New SyntaxException("The ''" & kwd & "'' statement must be paired with " & If(tmp.Length <> 1,
                                                          "one of: " & String.Join(",", tmp), " the ''" & tmp(0) & "'' statement"))
                            End If
                            Dim res As Object = EvalExprRaw(l, True)
                                If Not (TypeOf res Is Double AndAlso Double.IsNaN(CDbl(res))) AndAlso
                               Not (TypeOf res Is BigDecimal AndAlso DirectCast(res, BigDecimal).IsUndefined) Then
                                lastVal = res
                            End If
                        End If
                    Next
                Next

                If internal Then
                    Return New StatementResult(lastVal)
                Else
                    Return lastVal
                End If

            Catch ex As EvaluatorException ' append line numbers to errors
                If ex.Line = 0 Then
                    If TypeOf ex Is MathException Then
                        Throw New MathException(ex.Message, _curLine + 1)
                    ElseIf TypeOf ex Is SyntaxException
                        Throw New SyntaxException(ex.Message, _curLine + 1)
                    Else
                        Throw New EvaluatorException(ex.Message, _curLine + 1)
                    End If
                Else
                    Throw ex
                End If

            Catch ex As Exception
                Throw New EvaluatorException(ex.Message, _curLine + 1)
            End Try
        End Function

        ''' <summary>
        ''' Evauate a multi-line script asynchroneously and raises the EvalComplete event when done
        ''' returning the result as a system object
        ''' </summary>
        ''' <param name="script">The script to evaluate</param>
        Public Sub EvalRawAsync(script As String, Optional internal As Boolean = False)
            Dim th As New Thread(Sub()
                                     Try
                                         RaiseEvent EvalComplete(Me, EvalRaw(script, internal))
                                     Catch ex As Exception
                                         RaiseEvent EvalComplete(Me, ex.Message.Trim())
                                     End Try
                                 End Sub)
            th.Start()
        End Sub

        ''' <summary>
        ''' Given a line of text, finds the level of indentation. 
        ''' </summary>
        ''' <returns></returns>
        Private Function LineIndentLevel(line As String) As Integer
            Dim res As Integer = 0
            For i As Integer = 0 To line.Length - 1
                If line(i) = " " Then
                    res += 1
                ElseIf line(i) = ControlChars.Tab Then
                    res += SpacesPerTab
                Else
                    Return res
                End If
            Next
            Return 0
        End Function

        ''' <summary>
        ''' Evauate a mathematical expression and return the result as a processed string
        ''' </summary>
        ''' <param name="expr">The expression to evaluate</param>
        Public Function EvalExpr(expr As String) As String
            Return InternalFunctions.O(EvalExprRaw(expr))
        End Function

        ''' <summary>
        ''' Evauate a mathematical expression asynchroneously and raises the EvalComplete event when done
        ''' </summary>
        ''' <param name="expr">The expression to evaluate</param>
        Public Sub EvalExprAsync(expr As String)
            Dim th As New Thread(Sub()
                                     Try
                                         RaiseEvent EvalComplete(Me, EvalExpr(expr))
                                     Catch ex As Exception
                                         RaiseEvent EvalComplete(Me, ex.Message.Trim())
                                     End Try
                                 End Sub)
            th.Start()
        End Sub

        ''' <summary>
        ''' Evauate a mathematical expression and return the resulting object
        ''' </summary>
        ''' <param name="expr">The expression to evaluate</param>
        ''' <param name="internal">If true, evaluates without saving answers ''' (for internal uses)</param>
        Public Function EvalExprRaw(expr As String, Optional ByVal internal As Boolean = False,
                                    Optional ByVal condition As Boolean = False) As Object
            Dim resultObj As ObjectTypes.EvalObjectBase = ResolveOperators(Tokenize(expr))
            Dim result As Object = BigDecimal.Undefined

            If Not resultObj Is Nothing Then

                If TypeOf resultObj Is ObjectTypes.Reference Then
                    resultObj = DirectCast(resultObj, ObjectTypes.Reference).ResolveObj()
                End If
                If TypeOf resultObj Is ObjectTypes.Number Then
                    result = CType(resultObj, ObjectTypes.Number).BigDecValue()
                Else
                    result = resultObj.GetValue()
                End If

                If result Is Nothing Then result = BigDecimal.Undefined
            End If

            If Not internal Then
                ' do not save if undefined
                If (Not TypeOf result Is BigDecimal OrElse Not DirectCast(result, BigDecimal).IsUndefined) AndAlso
                     (Not TypeOf result Is Double OrElse Not Double.IsNaN(CDbl(result))) Then PrevAns.Add(resultObj)
            End If
            Return result
        End Function

        ''' <summary>
        ''' Evauate a mathematical expression asynchroneously and raises the EvalComplete event when done,
        ''' returning the result as a system object
        ''' </summary>
        ''' <param name="expr">The expression to evaluate</param>
        ''' <param name="internal">If true, evaluates without saving answers ''' (for internal uses)</param>
        Public Sub EvalExprRawAsync(expr As String, internal As Boolean)
            Dim th As New Thread(Sub()
                                     Try
                                         RaiseEvent EvalComplete(Me, EvalExprRaw(expr, internal))
                                     Catch ex As Exception
                                         RaiseEvent EvalComplete(Me, ex.Message.Trim())
                                     End Try
                                 End Sub)
            th.Start()
        End Sub

        ''' <summary>
        ''' Goes through a list of tokens and evaluates all operators by precedence
        ''' </summary>
        ''' <param name="tokens">The list of tokens to evaluate</param>
        ''' <returns></returns>
        Private Function ResolveOperators(tokens As TokenList, Optional ByVal left As Integer = 0,
                                        Optional ByVal right As Integer = -1) As ObjectTypes.EvalObjectBase
            If right < 0 Then right = tokens.Count - 1

            ' start from operators with highest precedence, skipping the brackets (already evaluated when tokenizing)
            For i As Integer = [Enum].GetValues(GetType(OperatorRegistar.Precedence)).Length - 1 To 0 Step -1
                Dim cur_precedence As OperatorRegistar.Precedence = CType(i, OperatorRegistar.Precedence)

                Dim prevct As Integer
                ' keep looping until all operators are done
                While tokens.OperatorsWithPrecedenceCount(cur_precedence) > 0
                    Dim preclst As List(Of Integer) = tokens.OperatorsWithPrecedence(cur_precedence)
                    prevct = tokens.OperatorsWithPrecedenceCount(cur_precedence)

                    ' RTL evaluation for assignment operators so you can chain them
                    If cur_precedence = OperatorRegistar.Precedence.assignment Then preclst.Reverse()

                    For Each opid As Integer In preclst
                        ' check that the operator is within range
                        If opid < left OrElse opid > right Then Continue For

                        ' check if the operator has not already been executed and is of the correct precedence
                        If tokens.IsRemoved(opid) OrElse tokens.OperatorAt(opid).Precedence <> cur_precedence Then Continue For

                        Dim prevtoken As Token = If(opid > 0, tokens(opid - 1), New Token(Nothing, Nothing))
                        Dim curtoken As Token = tokens(opid)
                        Dim result As ObjectTypes.EvalObjectBase
                        If TypeOf curtoken.Operator Is OperatorRegistar.UnaryOperatorBefore Then ' operators like x!
                            Dim op As OperatorRegistar.UnaryOperatorBefore = CType(curtoken.Operator, OperatorRegistar.UnaryOperatorBefore)

                            ' if we're not passing by reference then copy and "dereference" the references before passing
                            If Not op.ByReference AndAlso Not prevtoken.Object Is Nothing Then
                                prevtoken.Object = prevtoken.Object.DeepCopy()
                                If TypeOf prevtoken.Object Is ObjectTypes.Reference Then
                                    prevtoken.Object = CType(prevtoken.Object, ObjectTypes.Reference).ResolveObj()
                                End If
                            End If

                            Try
                                result = op.Execute(prevtoken.Object)
                            Catch ex As NullReferenceException
                                Throw New EvaluatorException("Operator " & op.Signs(0) & " disallows empty operands")
                            End Try

                            tokens.SetObject(opid - 1, result)
                            If tokens.ObjectAt(opid) Is Nothing OrElse
                                        (TypeOf tokens.ObjectAt(opid).GetValue() Is Double AndAlso
                                        Double.IsNaN(CDbl(tokens.ObjectAt(opid).GetValue()))) Then
                                tokens.RemoveAt(opid)
                            Else
                                tokens.SetOperator(opid, OperatorRegistar.DefaultOperator) ' default to multiply
                            End If

                        ElseIf TypeOf curtoken.Operator Is OperatorRegistar.UnaryOperatorAfter Then ' operators like ~x
                            Dim op As OperatorRegistar.UnaryOperatorAfter = CType(curtoken.Operator, OperatorRegistar.UnaryOperatorAfter)
                            ' allow for chaining of unary after operators with same precedence:
                            ' skip evaluation if the target of execution Is null.
                            ' and execute after the other unary after operator has filled in the target
                            If curtoken.Object Is Nothing AndAlso opid < tokens.OperatorCount - 1 Then
                                If tokens.OperatorAt(opid + 1).Precedence = cur_precedence Then Continue For
                            End If
                            ' if we're not passing by reference then "dereference" the references before passing
                            If Not op.ByReference AndAlso Not prevtoken.Object Is Nothing Then
                                prevtoken.Object = prevtoken.Object.DeepCopy()
                                If TypeOf prevtoken.Object Is ObjectTypes.Reference Then
                                    prevtoken.Object = CType(prevtoken.Object, ObjectTypes.Reference).ResolveObj()
                                End If
                            End If

                            Try
                                result = op.Execute(curtoken.Object)
                            Catch ex As NullReferenceException
                                Throw New EvaluatorException("Operator " & op.Signs(0) & " disallows empty operands")
                            End Try

                            tokens.SetObject(opid, result)

                            If tokens.ObjectAt(opid - 1) Is Nothing OrElse
                                   (TypeOf tokens.ObjectAt(opid - 1).GetValue() Is Double AndAlso
                                    Double.IsNaN(CDbl(tokens.ObjectAt(opid - 1).GetValue()))) Then
                                tokens.SetObject(opid - 1, result)
                                tokens.RemoveAt(opid)
                            Else
                                tokens.SetOperator(opid, OperatorRegistar.DefaultOperator) ' default to multiply
                            End If

                        Else ' if binary
                            Dim op As OperatorRegistar.BinaryOperator = CType(curtoken.Operator, OperatorRegistar.BinaryOperator)

                            If curtoken.Object Is Nothing AndAlso
                                opid < tokens.OperatorCount - 1 AndAlso curtoken.Object Is Nothing Then
                                ' allow for chaining of binary operators with same precedence:
                                ' skip evaluation if the right side Is null.
                                If tokens(opid + 1).Operator.Precedence = cur_precedence Then Continue For
                            End If

                            ' if we're not passing by reference then "dereference" the references before passing
                            If Not op.ByReference Then
                                If Not prevtoken.Object Is Nothing Then
                                    prevtoken.Object = prevtoken.Object.DeepCopy()
                                    If TypeOf prevtoken.Object Is ObjectTypes.Reference Then
                                        prevtoken.Object = CType(prevtoken.Object, ObjectTypes.Reference).ResolveObj()
                                    End If
                                End If
                                If Not curtoken.Object Is Nothing Then
                                    curtoken.Object = curtoken.Object.DeepCopy()
                                    If TypeOf curtoken.Object Is ObjectTypes.Reference Then
                                        curtoken.Object = CType(curtoken.Object, ObjectTypes.Reference).ResolveObj()
                                    End If
                                End If
                            End If

                            Try
                                result = op.Execute(prevtoken.Object, curtoken.Object)
                            Catch ex As NullReferenceException
                                Throw New EvaluatorException("Operator " & op.Signs(0) & " disallows empty operands")
                            End Try

                            tokens.SetObject(opid - 1, result)
                            tokens.RemoveAt(opid)
                        End If
                    Next

                    ' if we don't make any progresss then stop trying
                    If prevct <= tokens.OperatorsWithPrecedenceCount(cur_precedence) Then Exit While
                End While
            Next

            Return tokens.ObjectAt(tokens.Count - 1)
        End Function

        ''' <summary>
        ''' Parse the expression into a TokenList object, which can then be used to evaluate the expression
        ''' </summary>
        ''' <param name="expr"></param>
        ''' <returns></returns>
        Private Function Tokenize(expr As String) As TokenList
            Dim lst As New TokenList()

            ' beginning has no operator
            lst.AddOperator(Nothing)

                Dim idx As Integer = 0
                Dim value As Boolean = False

            For i As Integer = 0 To expr.Length - 1
                For j As Integer = Math.Min(expr.Length, i + OperatorRegistar.MAX_OPERATOR_LENGTH) To i Step -1
                    Dim valueL As String = expr.Substring(i, j - i).Replace("  ", " ").ToLowerInvariant()

                    If OperatorRegistar.OperatorExists(valueL) Then
                        Dim op As OperatorRegistar.Operator = OperatorRegistar.OperatorWithSign(valueL)
                        Dim objstr As String = expr.Substring(idx, i - idx).Trim()
                        Dim eo As ObjectTypes.EvalObjectBase = Nothing

                        ' if the object is not empty we try to detect its type
                        If Not objstr = "" Then eo = ObjectTypes.StrDetectType(objstr, Me)

                        If Not objstr = "" Then ' if the object is not empty

                            ' we already detected the type of the object earlier

                            ' if the object is an identifier, we try to resolve it.

                            If ObjectTypes.Identifier.IsType(eo) Then

                                Dim varlist As List(Of ObjectTypes.EvalObjectBase)
                                Dim left As ObjectTypes.EvalObjectBase = Nothing

                                If valueL = "(" Then ' this ends with a function, so try resolving the function

                                    Dim funcargs As String = expr.Substring(j)
                                    If funcargs.Contains(")") Then
                                        funcargs = funcargs.Remove(DirectCast(OperatorRegistar.OperatorWithSign("("),
                                                                       OperatorRegistar.Bracket).
                                                                       FindCloseBracket(funcargs, OperatorRegistar))
                                    Else
                                        Throw New EvaluatorException("(: No close bracket found")
                                    End If

                                    If lst.ObjectCount > 0 AndAlso lst.OperatorCount >= lst.ObjectCount AndAlso
                                                    eo.ToString().Trim().StartsWith(".") Then
                                        left = lst.ObjectAt(lst.ObjectCount - 1)
                                    End If
                                    varlist = ResolveFunctions(eo.ToString(), funcargs, left)

                                    ' advance past this function
                                    idx = j + funcargs.Count + 1
                                    i = idx - 1

                                Else ' this consists of variables only, so only resolve variables
                                    If op.AssignmentOperator Then
                                        ' for assignment operators, do not resolve the variables
                                        varlist = New List(Of ObjectTypes.EvalObjectBase)({GetVariableRef(eo.ToString())})
                                    Else
                                        varlist = ResolveVariables(eo.ToString())
                                        Try
                                            If varlist.Count = 0 OrElse (varlist.Count = 1 AndAlso
                                                    (TypeOf varlist(0) Is ObjectTypes.Number AndAlso
                                                    Double.IsNaN(CDbl(varlist(0).GetValue()))) OrElse
                                                    (TypeOf varlist(0) Is ObjectTypes.Reference AndAlso
                                                    Double.IsNaN(CDbl(DirectCast(varlist(0), ObjectTypes.Reference).Resolve()
                                                    )))) Then 'could not properly resolve the variable, try resolving a function

                                                Dim varlist2 As List(Of ObjectTypes.EvalObjectBase) =
                                                            ResolveFunctions(eo.ToString(), "",
                                                             If(lst.ObjectCount > 0, lst.ObjectAt(lst.ObjectCount - 1), Nothing))
                                                varlist = varlist2

                                                ' advance past this function
                                                idx = j + 1
                                                i = idx - 1

                                            End If
                                        Catch
                                            ' do nothing
                                        End Try
                                    End If
                                End If

                                If varlist.Count > 0 Then ' good we found variables, let's add them

                                    If left Is Nothing Then
                                        lst.AddObject(varlist(0))
                                    Else ' if this is a self-referring function call then we need to replace the previous value
                                        lst.SetObject(lst.ObjectCount - 1, varlist(0))
                                    End If

                                    For k As Integer = 1 To varlist.Count - 1
                                        ' default operation is *; we add this operator between each variable
                                        lst.AddOperator(OperatorRegistar.DefaultOperator)
                                        'End If
                                        lst.AddObject(varlist(k))
                                    Next

                                Else ' we couldn't resolve this identifier
                                    lst.AddObject(Nothing)
                                End If
                                ' if it was a function then we don't continue since we're already
                                ' done adding And advancing the counters
                                If valueL = ("(") Then Exit For

                            Else ' if the object is not a identifier (if it is a number, etc.) we just add it
                                lst.AddObject(eo)
                            End If

                        Else ' if the object is empty we just add the operator
                            ' if the operator count is too high we will add a null object to balance it
                            If lst.OperatorCount - lst.ObjectCount >= 1 Then lst.AddObject(Nothing) ''
                        End If

                        If Not TypeOf op Is OperatorRegistar.Bracket Then lst.AddOperator(op, valueL)

                        ' if we find an operator with brackets precedence
                        ' we evaluate the bracket and continue after it.
                        ' If the value before is an identifier we recognize it as a function so we skip this
                        If TypeOf op Is OperatorRegistar.Bracket AndAlso
                                    (op.Signs(0) <> "( " OrElse eo Is Nothing OrElse Not ObjectTypes.Identifier.IsType(eo)) Then

                            Dim inner As String = expr.Substring(j)
                            Dim endIdx As Integer

                            If op.Signs.Count <= 2 AndAlso op.Signs.Count > 0 Then
                                endIdx = DirectCast(op, OperatorRegistar.Bracket).FindCloseBracket(inner, OperatorRegistar)
                            Else
                                Throw New EvaluatorException("Invalid bracket operator: must have 1 or 2 signs")
                            End If

                            If endIdx >= 0 Then
                                inner = inner.Remove(endIdx)

                                Dim brkt As OperatorRegistar.Bracket = DirectCast(op, OperatorRegistar.Bracket)
                                Dim left As ObjectTypes.EvalObjectBase = Nothing
                                Dim orig As ObjectTypes.EvalObjectBase = Nothing

                                If lst.ObjectCount > 0 Then
                                    left = lst.ObjectAt(lst.ObjectCount - 1)
                                    ' if we're not passing by reference then "dereference" the references before passing
                                    If Not brkt.ByReference AndAlso Not left Is Nothing Then
                                        left = left.DeepCopy()
                                        If TypeOf left Is ObjectTypes.Reference Then
                                            left = CType(left, ObjectTypes.Reference).ResolveObj()
                                        End If
                                    End If
                                    orig = left
                                End If
                                Dim result As ObjectTypes.EvalObjectBase = brkt.Execute(inner, left)

                                If left Is Nothing Then
                                    lst.SetObject(lst.ObjectCount - 1, result)
                                Else
                                    If Not left Is orig Then
                                        lst.SetObject(lst.ObjectCount - 1, left)
                                    Else
                                        If Not valueL.Trim() = "" Then lst.AddOperator(OperatorRegistar.DefaultOperator)
                                        lst.AddObject(result)
                                        If lst.ObjectCount > lst.OperatorCount Then lst.AddOperator(OperatorRegistar.DefaultOperator)
                                    End If
                                End If

                                'advance the counters past the entire bracket set
                                i += brkt.Signs(0).Length - 1 + inner.Length +
                                             If(brkt.Signs.Count = 1, brkt.Signs(0).Length, brkt.Signs(1).Length)

                                idx = i + 1
                                Exit For
                            Else
                                Throw New EvaluatorException(op.Signs(0) & ": No close bracket found")
                            End If
                        End If

                        ' advance the counters past this identifier
                        idx = j
                        i = j - 1
                        Exit For
                    End If
                Next
            Next

            ' add remaining bit at the end
            If Not expr.Substring(idx, expr.Length - idx).Trim() = "" Then
                    Dim eo As ObjectTypes.EvalObjectBase = ObjectTypes.StrDetectType(expr.Substring(idx, expr.Length - idx).Trim())

                ' if the object we get is an identifier, we try to break it into variables which are resolved using ResolveVariables
                If ObjectTypes.Identifier.IsType(eo) Then
                    Dim varlist As List(Of ObjectTypes.EvalObjectBase)
                    varlist = ResolveVariables(eo.ToString())

                    Try
                        If varlist.Count = 0 OrElse (varlist.Count = 1 AndAlso
                                                    (TypeOf varlist(0) Is ObjectTypes.Number AndAlso
                                                    Double.IsNaN(CDbl(varlist(0).GetValue()))) OrElse
                                                    (TypeOf varlist(0) Is ObjectTypes.Reference AndAlso
                                                    Double.IsNaN(CDbl(DirectCast(varlist(0), ObjectTypes.Reference).Resolve()
                                                    )))) Then 'could not properly resolve the variable, try resolving a function

                            Dim varlist2 As List(Of ObjectTypes.EvalObjectBase) =
                                                ResolveFunctions(eo.ToString(), "",
                                                 If(lst.ObjectCount > 0, lst.ObjectAt(lst.ObjectCount - 1), Nothing))

                            varlist = varlist2
                        End If
                    Catch
                        ' do nothing
                    End Try

                    If varlist.Count > 0 Then
                        lst.AddObject(varlist(0))
                        For k As Integer = 1 To varlist.Count - 1

                            lst.AddObject(varlist(k))
                            If lst.OperatorCount < lst.ObjectCount Then

                                ' default operation is *
                                lst.AddOperator(OperatorRegistar.DefaultOperator, "*")
                            End If
                        Next
                    Else
                        lst.AddObject(Nothing)
                    End If
                Else ' otherwise we just add it
                    lst.AddObject(eo)
                        While lst.OperatorCount < lst.ObjectCount
                            lst.AddOperator(OperatorRegistar.DefaultOperator, "*")
                        End While
                    End If
                End If

                lst.BalanceLists()

                Return lst
        End Function

        ''' <summary>
        ''' Tries to break a string into a function on the right and valid variables and constants on the left
        ''' For example: xsin(x) should be x*sin(x), a variable times a function
        ''' Note: Prioritizes longer functions and variables so if both hello() and he llo() are defined hello() will be used
        ''' </summary>
        ''' <param name="str">The identifier string to parse</param>
        ''' <param name="args">The string containing the function arguments</param>
        ''' <param name="left">The object to the left of the function, needed to resolve self-referring function calls</param>
        ''' <returns></returns>
        Private Function ResolveFunctions(str As String, args As String, ByRef left As ObjectTypes.EvalObjectBase) As List(Of ObjectTypes.EvalObjectBase)
            Dim min As Integer = 0
            Dim max As Integer = str.Length - 1

            Dim baseObj As ObjectTypes.EvalObjectBase = Nothing

            ' deal with self-referring (.) notation
            If str.Contains(".") Then
                Dim baseTxt As String = str.Remove(str.LastIndexOf("."))
                If max = min AndAlso baseTxt.Length + 1 <> min Then
                    Throw New EvaluatorException("Member function is undefined")
                End If
                min = 0
                max = 0
                str = str.Substring(str.IndexOf(".") + 1)
                If Not String.IsNullOrEmpty(baseTxt) Then
                    Try
                        baseObj = GetVariableRef(baseTxt)
                    Catch
                        baseObj = ObjectTypes.DetectType(EvalExprRaw(baseTxt, True), True)
                        If baseObj Is Nothing Then Throw New EvaluatorException("Cannot call self-referring function call of undefined")
                    End Try
                Else
                    If left Is Nothing Then
                        baseObj = GetDefaultVariableRef()
                        If baseObj Is Nothing Then Throw New EvaluatorException("Cannot call self-referring function call of undefined")
                    Else
                        baseObj = left
                    End If
                End If
            Else
                left = Nothing
            End If

            Dim arglst As New List(Of Object)
            If Not baseObj Is Nothing Then
                If TypeOf baseObj Is ObjectTypes.Tuple Then ' if a tuple is used, supplies multiple parameters
                    For Each r As ObjectTypes.Reference In CType(CType(baseObj, ObjectTypes.Tuple).GetValue(), ObjectTypes.Reference())
                        arglst.Add(r.GetValue())
                    Next
                Else
                    arglst.Add(baseObj.GetValue())
                End If
            End If

            If Not String.IsNullOrWhiteSpace(args) Then
                Dim tuple As Object = EvalExprRaw("(" & args & ")", True)
                Dim otherarglst As New List(Of ObjectTypes.Reference)
                If TypeOf tuple Is ObjectTypes.Reference() Then
                    otherarglst.AddRange(CType(tuple, ObjectTypes.Reference()))
                ElseIf TypeOf tuple Is ObjectTypes.Reference
                    otherarglst.Add(CType(tuple, ObjectTypes.Reference))
                Else
                    otherarglst.Add(New ObjectTypes.Reference(tuple))
                End If
                For Each ref As ObjectTypes.Reference In otherarglst
                    If ref Is Nothing Then arglst.Add(Double.NaN)
                    arglst.Add(ref.GetValue())
                Next
            End If

            For i As Integer = min To max
                Dim fn As String = str.Substring(i).Trim()
                Dim varstr As String = str.Remove(i)
                Dim lst As List(Of ObjectTypes.EvalObjectBase)
                Try
                    lst = ResolveVariables(varstr)
                Catch
                    Exit For
                End Try
                If _userFunctions.Keys.Contains(fn) Then ' user functions
                    Dim execResult As Object = ExecUserFunction(fn, arglst)
                    lst.Add(ObjectTypes.DetectType(execResult, True))
                    Return lst
                Else ' functions defined in EvalFunctions
                    Dim info As Reflection.MethodInfo

                    info = GetType(InternalFunctions).GetMethod(fn.ToLowerInvariant(),
                    Reflection.BindingFlags.IgnoreCase Or
                    Reflection.BindingFlags.Public Or Reflection.BindingFlags.Instance Or
                    Reflection.BindingFlags.DeclaredOnly)

                    If Not info Is Nothing Then
                        Dim minParamCt As Integer = 0
                        Dim maxParamCt As Integer = 0
                        Dim parameterMismatch As Boolean = False
                        For Each paraminfo As Reflection.ParameterInfo In info.GetParameters()
                            If Not paraminfo.IsOptional Then minParamCt += 1
                            If maxParamCt >= arglst.Count Then
                                If paraminfo.IsOptional Then
                                    arglst.Add(paraminfo.DefaultValue)
                                Else
                                    parameterMismatch = True
                                End If
                            ElseIf Not paraminfo.ParameterType().IsAssignableFrom(arglst(maxParamCt).GetType()) Then
                                Dim paramTypeName As String = paraminfo.ParameterType().
                                                    Name.Replace("String", "Text").Replace("Double", "Number").
                                                    Replace("SortedDictionary", "Set").Replace("BigDecimal", "Number").
                                                    Replace("List", "Matrix").Replace("Reference[]", "Tuple").Replace("Object", "(Variable)").
                                                    Replace("ICollection", "(Matrix/Set/Tuple)").Trim()

                                If paramTypeName.Contains("`") Then paramTypeName = paramTypeName.Remove(paramTypeName.IndexOf("`"))
                                Throw New EvaluatorException(fn.ToLowerInvariant() & ": Parameter " & maxParamCt + 1 &
                                                    ": '" & paramTypeName & "' Type Expected")
                            End If
                            maxParamCt += 1
                        Next

                        If parameterMismatch OrElse arglst.Count > maxParamCt Then
                            Throw New EvaluatorException(fn.ToLowerInvariant() & ": " &
                                                   If(minParamCt = maxParamCt, minParamCt.ToString(),
                                                   minParamCt & " to " & maxParamCt) &
                                                    " parameter(s) expected")
                        End If
                        Try
                            Dim execResult As Object = info.Invoke(InternalFunctions, arglst.ToArray())
                            lst.Add(ObjectTypes.DetectType(execResult, True))
                            Return lst
                        Catch ex As Exception
                            Throw New EvaluatorException(fn.ToLowerInvariant() & ": Error running function")
                        End Try
                    End If
                End If
            Next
            Throw New EvaluatorException("Function ''" & str.ToLowerInvariant().Trim() & "'' is undefined")
        End Function

        ''' <summary>
        ''' Tries to break the string into valid variables and constants
        ''' i.e. the string abc can either represent the variable abc or a * b * c
        ''' And then resolves the variables
        ''' Note: Prioritizes longer variables so if both xy and x y are defined xy will be used
        ''' </summary>
        ''' <param name="str">The string to parse</param>
        ''' <returns></returns>
        Private Function ResolveVariables(str As String) As List(Of ObjectTypes.EvalObjectBase)
            Dim ret As New List(Of ObjectTypes.EvalObjectBase)

            Dim i As Integer = 0

            While i < str.Length
                For j As Integer = str.Length - i To 1 Step -1
                    Dim cur As String = str.Substring(i, j)
                    If VariableExists(cur) Then
                        ret.Add(GetVariableRef(cur))
                        i += j
                        Continue While
                    Else
                        Dim dbl As Double
                        If Double.TryParse(cur, dbl) Then
                            ret.Add(New ObjectTypes.Number(dbl))
                            i += j
                            Continue While

                        ElseIf j = 1 ' really can't find anything
                            ret.Clear()
                            SetVariable(str, Double.NaN)
                            ret.Add(GetVariableRef(str))
                            Exit While
                        End If
                    End If
                Next
                i += 1
            End While
            Return ret
        End Function
#End Region

#Region "Variables, User Functions, Past Answers"

        ''' <summary>
        ''' Get the value of the variable with the name specified as an IEvalObject
        ''' </summary>
        ''' <param name="name">Name of the variable</param>
        ''' <returns></returns>
        Friend Function GetVariableRef(ByVal name As String) As ObjectTypes.Reference
            If name = "ans" Then Return New ObjectTypes.Reference(GetLastAns())
            If Not VariableExists(name) Then SetVariable(name, Double.NaN)
            Return _vars(name)
        End Function

        ''' <summary>
        ''' Get the value of the variable with the name specified as a system object
        ''' </summary>
        ''' <param name="name">Name of the variable</param>
        ''' <returns></returns>
        Public Function GetVariable(ByVal name As String) As Object
            If name = "ans" Then Return GetLastAns()

            If Not VariableExists(name) Then Return Double.NaN
            If _vars(name) Is Nothing Then Return Double.NaN
            Dim value As ObjectTypes.EvalObjectBase = _vars(name)
            Return value.GetValue()
        End Function

        ''' <summary>
        ''' Set the value of the variable with the name specified to the object referenced
        ''' </summary>
        ''' <param name="name">Name of the variable</param>
        ''' <param name="ref">Value of the variable as a Reference</param>
        Public Sub SetVariable(ByVal name As String, ByVal ref As ObjectTypes.Reference)
            If _externalExec Then _varExtern(name) = ref.Resolve()
            If String.IsNullOrWhiteSpace(name) Then Throw New EvaluatorException("Variable name cannot be empty")
            If name.Trim() = DEFAULT_VAR_NAME Then Throw New EvaluatorException("Variable name ''" & DEFAULT_VAR_NAME & "'' is reserved")
            If Not IsValidVariableName(name) Then Throw New EvaluatorException("Invalid Variable Name: " & name)
            _vars(name) = ref
        End Sub

        ''' <summary>
        ''' Set the value of the variable with the name specified
        ''' </summary>
        ''' <param name="name">Name of the variable</param>
        ''' <param name="value">Value of the variable as an IEvalObject</param>
        Public Sub SetVariable(ByVal name As String, ByVal value As ObjectTypes.EvalObjectBase)
            If String.IsNullOrWhiteSpace(name) Then Throw New EvaluatorException("Variable name cannot be empty")
            If name.Trim() = DEFAULT_VAR_NAME Then Throw New EvaluatorException("Variable name ''" & DEFAULT_VAR_NAME & "'' is reserved")
            If Not IsValidVariableName(name) Then Throw New EvaluatorException("Invalid Variable Name: " & name)
            If _externalExec Then _varExtern(name) = value.GetValue()
            _vars(name) = New ObjectTypes.Reference(value)
        End Sub

        ''' <summary>
        ''' Set the value of the variable with the name specified
        ''' </summary>
        ''' <param name="name">Name of the variable</param>
        ''' <param name="value">Value of the variable as a system object</param>
        Public Sub SetVariable(ByVal name As String, ByVal value As Object)
            If _externalExec Then _varExtern(name) = value
            If String.IsNullOrWhiteSpace(name) Then Throw New EvaluatorException("Variable name cannot be empty")
            If name.Trim() = DEFAULT_VAR_NAME Then Throw New EvaluatorException("Variable name ''" & DEFAULT_VAR_NAME & "'' is reserved")
            If Not IsValidVariableName(name) Then Throw New EvaluatorException("Invalid Variable Name: " & name)
            SetVariable(name, ObjectTypes.DetectType(value, True))
        End Sub

        ''' <summary>
        ''' Set the value of the default variable (i.e. this) used when no name is specified in a self-referring function call (.xxyy())
        ''' </summary>
        ''' <param name="ref">Value of the variable as a Reference</param>
        Friend Sub SetDefaultVariable(ByVal ref As ObjectTypes.Reference)
            _vars(DEFAULT_VAR_NAME) = ref
        End Sub

        ''' <summary>
        ''' Get the value of the default variable (i.e. this) used when no name is specified in a self-referring function call (.xxyy())
        ''' </summary>
        Friend Function GetDefaultVariableRef() As ObjectTypes.Reference
            If _vars.ContainsKey("") Then
                Return _vars(DEFAULT_VAR_NAME)
            Else
                ' default variable not set, we'll complain about the variable name
                Throw New EvaluatorException("Variable name cannot be empty")
            End If
        End Function

        Private _varBackup As New Dictionary(Of String, ObjectTypes.Reference)

        ''' <summary>
        ''' Make a backup of current variables. Restore with RestoreVariableBackup()
        ''' </summary>
        Private Sub BackupVariables()
            _varBackup = New Dictionary(Of String, ObjectTypes.Reference)(_vars)
        End Sub

        ''' <summary>
        ''' Restore the last variable backup made with BackupVariables()
        ''' </summary>
        Private Sub RestoreVariableBackup()
            _vars = New Dictionary(Of String, ObjectTypes.Reference)(_varBackup)
        End Sub

        ''' <summary>
        ''' Returns true if the variable with the specified name is defined
        ''' </summary>
        Public Function VariableExists(ByVal name As String) As Boolean
            If name = "ans" Then Return True
            Return _vars.ContainsKey(name)
        End Function

        ''' <summary>
        ''' Returns true if the name given is a valid variable name 
        ''' (i.e. is not empty, does not contain any of &amp;+-*/{}[]()';^$@#!%=&lt;&gt;,:|\` and does not start with a number)
        ''' </summary>
        Public Shared Function IsValidVariableName(ByVal name As String) As Boolean
            Try
                Return ObjectTypes.Identifier.StrIsType(name)
            Catch
                Return False
            End Try
        End Function

        ''' <summary>
        ''' Clear all variables defined on this evaluator
        ''' </summary>
        Public Sub ClearVariables()
            _vars.Clear()
        End Sub

        ''' <summary>
        ''' Clears all variables, user functions, and previous answers on this evaluator
        ''' </summary>
        Public Sub Clear()
            ClearVariables()
            _userFunctions.Clear()
            PrevAns.Clear()
        End Sub

        ''' <summary>
        ''' Get the last answer
        ''' </summary>
        ''' <returns></returns>
        Public Function GetLastAns() As Object
            If PrevAns.Count > 0 Then Return PrevAns(PrevAns.Count - 1).GetValue()
            Return 0
        End Function

        ''' <summary>
        ''' Add a user function
        ''' </summary>
        ''' <param name="name">The name of the function</param>
        ''' <param name="args">A list of argument names</param>
        ''' <param name="def">The function definition</param>
        Public Sub AddUserFunction(name As String, ByVal args As List(Of String), def As String)
            If name.Length = 0 OrElse Not IsValidVariableName(name(0)) Then
                Throw New EvaluatorException("Error: Invalid Function Name")
            End If

            _userFunctions(name) = def
            For i As Integer = 0 To args.Count - 1
                args(i) = args(i).Trim()
                If Not IsValidVariableName(args(i)) Then Throw New EvaluatorException("Invalid Argument Name: " & args(i))
            Next
            _userFunctionArgs(name) = args
            _userFunctionExtern(name) = _externalExec
        End Sub

        ''' <summary>
        ''' Add a user function
        ''' </summary>
        ''' <param name="fmtstr">The function in function notation e.g. name(a,b)</param>
        ''' <param name="def">The function definition</param>
        Public Function AddUserFunction(fmtstr As String, def As String) As Boolean
            Dim openBracket As Integer
            Dim closeBracket As Integer
            Dim name As String
            If fmtstr.Contains("(") Then
                openBracket = fmtstr.IndexOf("(")
                If Not fmtstr.Contains(")") Then Throw New SyntaxException("No close bracket found")
                closeBracket = fmtstr.IndexOf(")")
                name = fmtstr.Remove(openBracket).Trim()
            Else
                openBracket = fmtstr.Length - 1
                closeBracket = fmtstr.Length
                name = fmtstr.Trim()
            End If

            If String.IsNullOrEmpty(def) Then
                RemUserFunction(name)
            Else
                Dim l As New List(Of String)(
                    fmtstr.Substring(openBracket + 1, closeBracket - openBracket - 1).Split(","c))
                If l.Count = 1 AndAlso String.IsNullOrWhiteSpace(l(0)) Then l.Clear()
                AddUserFunction(name, l, def)
            End If
            Return True
        End Function

        ''' <summary>
        ''' Remove the user function with the name
        ''' </summary>
        ''' <param name="name"></param>
        Public Sub RemUserFunction(name As String)
            If (_userFunctions.ContainsKey(name)) Then
                _userFunctions.Remove(name)
                _userFunctionArgs.Remove(name)
            End If
        End Sub

        ''' <summary>
        ''' Return the definition of the user function with the given name
        ''' </summary>
        ''' <param name="name"></param>
        ''' <returns></returns>
        Public Function GetUserFunction(name As String) As String
            Return _userFunctions(name)
        End Function

        ''' <summary>
        ''' Return the arguments of the user function with the given name
        ''' </summary>
        ''' <param name="name"></param>
        ''' <returns></returns>
        Public Function GetUserFunctionArgs(name As String) As String()
            Return _userFunctionArgs(name).ToArray()
        End Function

        ''' <summary>
        ''' Returns a list of user functions defined in this evaluator
        ''' </summary>
        Public Function ListUserFunctions() As String()
            Return _userFunctions.Keys.ToArray()
        End Function

        ''' <summary>
        ''' Return true if a user function with the given name exists
        ''' </summary>
        ''' <param name="name"></param>
        ''' <returns></returns>
        Public Function HasUserFunction(name As String) As Boolean
            Return _userFunctions.ContainsKey(name)
        End Function

        ''' <summary>
        ''' Execute the function with the given arguments
        ''' </summary>
        ''' <param name="name"></param>
        Public Function ExecUserFunction(name As String, args As List(Of Object)) As Object
            If (_userFunctions.ContainsKey(name)) Then
                BackupVariables()
                Dim argnames As List(Of String) = _userFunctionArgs(name)
                If args.Count = argnames.Count Then
                    For i As Integer = 0 To args.Count - 1
                        SetVariable(argnames(i), args(i))
                    Next
                Else
                    Throw New EvaluatorException(name & ": " & argnames.Count & " parameter(s) expected")
                End If
                Dim result As Object = EvalRaw(_userFunctions(name))
                RestoreVariableBackup()
                Return result
            Else
                Throw New EvaluatorException("User Function " & name & " is Undefined")
            End If
        End Function

        ''' <summary>
        ''' Convert the evaluator's user functions, variables, and configuration into a
        '''  script that can be ran again for storage
        ''' </summary>
        ''' <returns></returns>
        Public Function ToScript() As String
            Dim serialized As New StringBuilder()
            serialized.AppendLine("# Cantus " & Application.ProductVersion &
                                  " Auto-generated initialization script")
            serialized.AppendLine("# Use caution when modifying manually").Append(vbNewLine)
            serialized.AppendLine("# Modes")

            serialized.Append("OMode(").Append(ControlChars.Quote).Append(OMode.ToString()).Append(ControlChars.Quote).Append(")").
                Append(vbNewLine)
            serialized.Append("AngleRep(").Append(ControlChars.Quote).Append(AngleRepMode.ToString()).Append(ControlChars.Quote).
                Append(")").Append(vbNewLine)
            serialized.Append("SpacesPerTab(").Append(SpacesPerTab.ToString()).Append(")").Append(vbNewLine)

            serialized.AppendLine().AppendLine("# Function Definitions")
            For Each func As KeyValuePair(Of String, List(Of String)) In _userFunctionArgs
                If HasUserFunction(func.Key) AndAlso Not _userFunctionExtern(func.Key) Then
                    serialized.Append("function ").Append(func.Key).Append("(")
                    serialized.Append(String.Join(", ", func.Value)).Append(")").Append(vbNewLine)
                    serialized.AppendLine(GetUserFunction(func.Key).TrimEnd())
                End If
            Next

            serialized.AppendLine().AppendLine("# Variable Definitions")
            For Each var As KeyValuePair(Of String, ObjectTypes.Reference) In _vars.ToArray()
                Dim def As ObjectTypes.EvalObjectBase = var.Value.ResolveObj()

                If Not def Is Nothing AndAlso (
                    Not TypeOf def Is ObjectTypes.Number OrElse Not Double.IsNaN(CDbl(def.GetValue()))) AndAlso
                     Not TypeOf def Is ObjectTypes.Reference AndAlso
                    Not var.Key = DEFAULT_VAR_NAME AndAlso
                    Not (_varExtern.ContainsKey(var.Key) AndAlso
                    _varExtern(var.Key).GetType() = var.Value.GetType() AndAlso
                    _varExtern(var.Key).ToString() = var.Value.ToString()) Then ' note: cannot save references

                    Dim defs As String = def.ToString()
                    serialized.Append(var.Key).Append("=").Append(defs).Append(vbNewLine)
                End If
            Next
            serialized.AppendLine().AppendLine("# End of Cantus auto-generated initialization script. DO NOT modify this comment.")
            serialized.AppendLine("# You may write additional initialization code below this line.")

            Return serialized.ToString()
        End Function


        ''' <summary>
        ''' Create a copy of the evaluator containing the same user functions, variables, and functions starting at the current line
        ''' </summary>
        ''' <returns></returns>
        Public Function ScopedEvaluator() As Evaluator
            Dim varsCopy As New Dictionary(Of String, ObjectTypes.Reference)(Me._vars)
            Dim funcCopy As New Dictionary(Of String, String)(Me._userFunctions)
            Dim funcargsCopy As New Dictionary(Of String, List(Of String))(Me._userFunctionArgs)
            Return New Evaluator(Me.OMode, Me.AngleRepMode, Me.SpacesPerTab, Me.PrevAns, varsCopy, funcCopy,
                                         funcargsCopy, Me._curLine)
        End Function

        ''' <summary>
        ''' Create an identical copy of the evaluator containing COPIES of the same user functions, variables, and functions
        ''' </summary>
        ''' <returns></returns>
        Public Function Clone() As Evaluator
            Dim varsCopy As New Dictionary(Of String, ObjectTypes.Reference)
            Try
                For Each k As KeyValuePair(Of String, ObjectTypes.Reference) In _vars.ToArray()
                    varsCopy.Add(k.Key, DirectCast(k.Value.DeepCopy(), ObjectTypes.Reference))
                Next
            Catch
            End Try

            Dim funcCopy As New Dictionary(Of String, String)(Me._userFunctions)
            Dim funcargsCopy As New Dictionary(Of String, List(Of String))(Me._userFunctionArgs)

            Return New Evaluator(Me.OMode, Me.AngleRepMode, Me.SpacesPerTab, Me.PrevAns, varsCopy, funcCopy,
                                         funcargsCopy, Me._baseLine)
        End Function

        ''' <summary>
        ''' Cleans up threads spawned by this evaluator. Unneeded if no threads spawned.
        ''' </summary>
        Friend Sub Dispose() Implements IDisposable.Dispose
            Me.StatementRegistar.Dispose()
        End Sub
    End Class
#End Region

End Namespace
