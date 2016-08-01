Imports Cantus.Calculator.Evaluator.Exceptions
Imports Cantus.Calculator.Evaluator.CommonTypes
Namespace Calculator.Evaluator
    Public Class OperatorRegistar
#Region "Enums"
        ''' <summary>
        ''' Represents the precedence of an operator. The higher, the earlier the operator is executed.
        ''' Note that brackets do not have precedence. They are pre-evaluated during tokenization.
        ''' </summary>
        Public Enum ePrecedence
            ''' <summary>
            ''' Represents the precedence of assignment operators like =.
            ''' Note: assignment operators are evaluate RTL so you can do a=b=c chaining, whereas all others are LTR
            ''' </summary>
            assignment = 0
            ''' <summary>
            ''' Tupling opera
            ''' </summary>
            tupling
            ''' <summary>
            ''' represents the precedence of the logical and operator
            ''' </summary>
            [and]
            ''' <summary>
            ''' represents the precedence of the logical or operator
            ''' </summary>
            [or]
            ''' <summary>
            ''' represents the precedence of the logical not operator
            ''' </summary>
            [not]
            ''' <summary>
            ''' A comparison operator like =, &lt;
            ''' </summary>
            comparison
            ''' <summary>
            ''' Represents the precedence of the operators &lt;&lt; &amp; and \ (below +- but above comparison)
            ''' </summary>
            bitshift_concat_frac
            ''' <summary>
            ''' Represents the precedence of the operators + and -
            ''' </summary>
            add_sub
            ''' <summary>
            ''' Represents the precedence of the operators *, /, and mod, as well as most bitwise operators like || &amp;&amp;
            ''' </summary>
            mul_div
            ''' <summary>
            ''' Represents the precedence of the ^ operator
            ''' </summary>
            exponent
            ''' <summary>
            ''' Represents the precedence of some very high precedence operators like !, %, and E, evaluated first
            ''' </summary>
            fact_pct
        End Enum
#End Region

#Region "Operator Types"

        ''' <summary>
        ''' Base class for all operators
        ''' </summary>
        Public MustInherit Class [Operator]
            MustOverride ReadOnly Property Signs As List(Of String)
            MustOverride ReadOnly Property Precedence As ePrecedence
            ''' <summary>
            ''' If true, values are passed by the Reference class which allows the value within to be manipulated
            ''' </summary>
            Property ByReference As Boolean = False

            ''' <summary>
            ''' If true, the identifier before are evaluated as a single variable and not multiplied i.e. abc -> abc instead of a*b*c
            ''' </summary>
            Property AssignmentOperator As Boolean = False
        End Class

        ''' <summary>
        ''' An operator involving two values
        ''' </summary>
        Public Class BinaryOperator : Inherits [Operator]
            Public Overrides ReadOnly Property Signs As List(Of String)
            Public Overrides ReadOnly Property Precedence As ePrecedence
            Public ReadOnly Property Execute As Func(Of ObjectTypes.EvalObjectBase, ObjectTypes.EvalObjectBase, ObjectTypes.EvalObjectBase)
            ''' <summary>
            ''' Initialize a new binary operator
            ''' </summary>
            ''' <param name="signs">The list of signs to register for the operator</param>
            ''' <param name="precedence">The precedence of the operator</param>
            ''' <param name="execute">The operator definition, specified as a function (use AddressOf ...)</param>
            Public Sub New(signs As String(), precedence As ePrecedence, execute As Func(Of ObjectTypes.EvalObjectBase, ObjectTypes.EvalObjectBase, ObjectTypes.EvalObjectBase))
                Me.Signs = New List(Of String)(signs)
                Me.Precedence = precedence
                Me.Execute = execute
            End Sub
        End Class

        ''' <summary>
        ''' An operator involving a single value
        ''' </summary>
        Public MustInherit Class UnaryOperator : Inherits [Operator]
            Public Overrides ReadOnly Property Signs As List(Of String)
            Public Overrides ReadOnly Property Precedence As ePrecedence
            Public ReadOnly Property Execute As Func(Of ObjectTypes.EvalObjectBase, ObjectTypes.EvalObjectBase)
            ''' <summary>
            ''' Initialize a new unary operator
            ''' </summary>
            ''' <param name="signs">The list of signs to register for the operator</param>
            ''' <param name="execute">The operator definition, specified as a function (use AddressOf ...)</param>
            Public Sub New(signs As String(), precedence As ePrecedence, execute As Func(Of ObjectTypes.EvalObjectBase, ObjectTypes.EvalObjectBase))
                Me.Signs = New List(Of String)(signs)
                Me.Precedence = precedence
                Me.Execute = execute
            End Sub
        End Class

        ''' <summary>
        ''' An operator involving a single value before it (for example, x!)
        ''' </summary>
        Public Class UnaryOperatorBefore
            Inherits UnaryOperator
            Public Sub New(signs As String(), precedence As ePrecedence, execute As Func(Of ObjectTypes.EvalObjectBase, ObjectTypes.EvalObjectBase))
                MyBase.New(signs, precedence, execute)
            End Sub
        End Class

        ''' <summary>
        ''' An operator involving a single value after it (for example, 'not x')
        ''' </summary>
        Public Class UnaryOperatorAfter
            Inherits UnaryOperator
            Public Sub New(signs As String(), precedence As ePrecedence, execute As Func(Of ObjectTypes.EvalObjectBase, ObjectTypes.EvalObjectBase))
                MyBase.New(signs, precedence, execute)
            End Sub
        End Class

        ''' <summary>
        ''' A 'bracket' type operator (e.g. (), [], ||)
        ''' Evaluated before all other operators, on tokenization
        ''' </summary>
        Public Class Bracket : Inherits [Operator]
            Public Overrides ReadOnly Property Signs As New List(Of String)
            Public Overrides ReadOnly Property Precedence As ePrecedence
                Get
                    Return Nothing ' not used
                End Get
            End Property
            Public Delegate Function BracketDelegate(inner As String, ByRef left As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase

            ''' <summary>
            ''' If true, allow stacking (like ((1)+2)). Operators with a single sign cannot stack.
            ''' </summary>
            ''' <returns></returns>
            Public ReadOnly Property Stackable As Boolean

            Public ReadOnly Property Execute As BracketDelegate

            ''' <summary>
            ''' Create a new bracket operator with two signs (for example '(x)')
            ''' </summary>
            ''' <param name="signStart">The sign that begins the operator (eg. '(')</param>
            ''' <param name="signEnd">The sign that ends the bracket (eg. ')')</param>
            ''' <param name="execute">The operator definition (use AddressOf)</param>
            Public Sub New(signStart As String, signEnd As String, execute As BracketDelegate, Optional stackable As Boolean = True)
                Me.Signs = New List(Of String)
                Me.Signs.Add(signStart)
                Me.Signs.Add(signEnd)
                Me.Execute = execute
                Me.Stackable = stackable
            End Sub

            ''' <summary>
            ''' Create a new bracket operator with one start sign and many end signs
            ''' </summary>
            ''' <param name="signStart">The sign that begins the operator (eg. '(')</param>
            ''' <param name="signsEnd">A list of signs that end the bracket (eg. ')')</param>
            ''' <param name="execute">The operator definition (use AddressOf)</param>
            Public Sub New(signStart As String, signsEnd As IEnumerable(Of String), execute As BracketDelegate, Optional stackable As Boolean = True)
                Me.Signs = New List(Of String)
                Me.Signs.Add(signStart)
                Me.Signs.AddRange(signsEnd)
                Me.Execute = execute
                Me.Stackable = stackable
            End Sub

            ''' <summary>
            ''' Create a new bracket operator with one sign (for example '|x|')
            ''' </summary>
            ''' <param name="sign">The sign that begins and ends the operator (eg. '|')</param>
            ''' <param name="execute">The operator definition (use AddressOf)</param>
            Public Sub New(sign As String, execute As BracketDelegate)
                Me.Signs = New List(Of String)
                Me.Signs.Add(sign)
                Me.Execute = execute
                Me.Stackable = False
            End Sub

            ''' <summary>
            ''' Given the remaining part of the expression not including the open bracket,
            ''' finds the index of the matching close bracket.
            ''' </summary>
            ''' <param name="expr">The part of the expression after (not including) the open bracket</param>
            ''' <returns></returns>
            Public Function FindCloseBracket(expr As String, opReg As OperatorRegistar) As Integer
                Dim startSign As String = Me.Signs(0)
                Dim endSign As New HashSet(Of String)({If(Me.Signs.Count < 2, startSign, Me.Signs(1))})
                For i As Integer = 2 To Me.Signs.Count - 1
                    endSign.Add(Me.Signs(i))
                Next

                Dim endIdx As Integer
                Dim stackHeight As Integer = 0
                Dim lastFound As Integer = -1

                For endIdx = 0 To expr.Length - 1
                    Dim foundEndSign As Boolean = False
                    ' try to find end sign
                    For Each s As String In endSign
                        If expr.Substring(endIdx).StartsWith(s) Then
                            stackHeight -= 1
                            If stackHeight < 0 Then Return endIdx
                            endIdx += s.Length - 1
                            foundEndSign = True
                            Exit For
                        End If
                    Next

                    ' end sign not found, try to find start sign
                    If Not foundEndSign Then
                        If expr(endIdx) = "\"c Then
                            lastFound = endIdx + 1 ' save the time we last found an escaped end sign
                            endIdx += 1
                        ElseIf Stackable Then
                            If expr.Substring(endIdx).StartsWith(startSign) Then
                                stackHeight += 1
                                endIdx += startSign.Length - 1

                            Else ' try to find other brackets
                                For Each b As Bracket In opReg.Brackets
                                    If expr.Substring(endIdx).StartsWith(b.Signs(0)) Then
                                        ' start escape sequence
                                        If b.Signs.Count = 1 Then
                                            endIdx += b.FindCloseBracket(expr.Substring(endIdx + b.Signs(0).Length), opReg) +
                                                b.Signs(0).Length
                                        Else ' >1
                                            endIdx += b.FindCloseBracket(expr.Substring(endIdx + b.Signs(0).Length), opReg) +
                                                    b.Signs(1).Length
                                        End If
                                        Exit For
                                    End If
                                Next
                            End If
                        End If
                    End If
                Next

                ' if we can't find anything unescaped we'll ignore the last escape sequence
                If lastFound = -1 Then
                        Return expr.Length
                    Else
                        Return lastFound
                    End If
            End Function
        End Class

#End Region

#Region "Constants & Variable Declarations"
        ''' <summary>
        ''' Maximum length in characters of a single operator
        ''' </summary>
        Public Const MAX_OPERATOR_LENGTH As Integer = 9
        Private _eval As Evaluator

        ''' <summary>
        ''' A list of lists operators, with each inner list containing all operators of the precedence level of its index
        ''' </summary>
        Public Operators As New List(Of List(Of [Operator]))

        ''' <summary>
        ''' A list of brackets
        ''' </summary>
        Public Brackets As New List(Of Bracket)

        ''' <summary>
        ''' Condition mode: if on, the = operator is always seen as a comparison operator
        ''' </summary>
        Public Property ConditionMode As Boolean = False

        ''' <summary>
        ''' The default operator to use when none is specified, normally *
        ''' </summary>
        ''' <returns></returns>
        Public Property DefaultOperator As [Operator]

        ''' <summary>
        ''' A hashset of operator signs to quickly check if a given name is an operator
        ''' </summary>
        Private _operatorSigns As New Dictionary(Of String, [Operator])
#End Region

#Region "Public Methods"
        ''' <summary>
        ''' Create a new Operator Registar for registering and accessing operators like if/else
        ''' </summary>
        Public Sub New(parent As Evaluator, Optional ByVal conditional As Boolean = False)
            Me._eval = parent
            Me.ConditionMode = conditional
            RegisterOperators()
        End Sub

        ''' <summary>
        ''' Tests if there is an operator with the specified sign
        ''' </summary>
        ''' <param name="sign"></param>
        ''' <returns></returns>
        Public Function OperatorExists(sign As String) As Boolean
            Return _operatorSigns.ContainsKey(sign)
        End Function

        ''' <summary>
        ''' Returns the operator that registered the sign
        ''' </summary>
        ''' <param name="sign"></param>
        ''' <returns></returns>
        Public Function OperatorWithSign(sign As String) As [Operator]
            Return _operatorSigns(sign)
        End Function
#End Region

#Region "Operator Registration"
        ''' <summary>
        ''' Register all operators
        ''' </summary>
        Private Sub RegisterOperators()
            Operators.Clear()
            For Each lvl As Integer In [Enum].GetValues(GetType(ePrecedence))
                Operators.Add(New List(Of [Operator]))
            Next

            ' Register operators here 
            ' FORMAT: 
            ' Register(New [Type]Operator({[List of signs to register]}, Precedence.[Precedence], AddressOf [Definition]))

            Register(New BinaryOperator({"+"}, ePrecedence.add_sub, AddressOf BinaryOperatorAdd))
            Register(New BinaryOperator({"-"}, ePrecedence.add_sub, AddressOf BinaryOperatorSubtract))

            Register(New BinaryOperator({"*"}, ePrecedence.mul_div, AddressOf BinaryOperatorMultiply))
            Register(New BinaryOperator({"/"}, ePrecedence.mul_div, AddressOf BinaryOperatorDivide))
            Register(New BinaryOperator({"**"}, ePrecedence.mul_div, AddressOf BinaryOperatorDuplicateCross))
            Register(New BinaryOperator({"//"}, ePrecedence.mul_div, AddressOf BinaryOperatorDivideFloor))
            Register(New BinaryOperator({" mod "}, ePrecedence.mul_div, AddressOf BinaryOperatorModulo))

            Register(New BinaryOperator({"^"}, ePrecedence.exponent, AddressOf BinaryOperatorExponent))
            Register(New BinaryOperator({"&"}, ePrecedence.bitshift_concat_frac, AddressOf BinaryOperatorConcat))
            Register(New BinaryOperator({"\"}, ePrecedence.bitshift_concat_frac, AddressOf BinaryOperatorDivide))
            Register(New BinaryOperator({"<<"}, ePrecedence.bitshift_concat_frac, AddressOf BinaryOperatorShl))
            Register(New BinaryOperator({">>"}, ePrecedence.bitshift_concat_frac, AddressOf BinaryOperatorShr))

            Register(New BinaryOperator({" or "}, ePrecedence.or, AddressOf BinaryOperatorOr))
            Register(New BinaryOperator({"||"}, ePrecedence.mul_div, AddressOf BinaryOperatorBitwiseOr))
            Register(New BinaryOperator({" and "}, ePrecedence.and, AddressOf BinaryOperatorAnd))
            Register(New BinaryOperator({"&&"}, ePrecedence.mul_div, AddressOf BinaryOperatorBitwiseAnd))
            Register(New BinaryOperator({" xor "}, ePrecedence.or, AddressOf BinaryOperatorXor))
            Register(New BinaryOperator({"^^"}, ePrecedence.mul_div, AddressOf BinaryOperatorBitwiseXor))

            ' use number group separator for ,
            RegisterByRef(New BinaryOperator({","}, ePrecedence.tupling, AddressOf BinaryOperatorCommaTuple))

            RegisterByRef(New BinaryOperator({":"}, ePrecedence.tupling, AddressOf BinaryOperatorColon))

            Register(New BinaryOperator({" choose ", " c "}, ePrecedence.mul_div, AddressOf BinaryOperatorChoose))
            Register(New BinaryOperator({" e "}, ePrecedence.fact_pct, AddressOf BinaryOperatorExp10))

            Register(New BinaryOperator({"=="}, ePrecedence.comparison, AddressOf BinaryOperatorEqualTo))
            Register(New BinaryOperator({"!=", "<>"}, ePrecedence.comparison, AddressOf BinaryOperatorNotEqualTo))
            Register(New BinaryOperator({">"}, ePrecedence.comparison, AddressOf BinaryOperatorGreaterThan))
            Register(New BinaryOperator({">="}, ePrecedence.comparison, AddressOf BinaryOperatorGreaterThanOrEqualTo))
            Register(New BinaryOperator({"<"}, ePrecedence.comparison, AddressOf BinaryOperatorLessThan))
            Register(New BinaryOperator({"<="}, ePrecedence.comparison, AddressOf BinaryOperatorLessThanOrEqualTo))

            ' assignment (use RegisterByRef, with same format)

            RegisterAssignment(New BinaryOperator({"="}, ePrecedence.assignment, AddressOf BinaryOperatorAutoEqual))
            RegisterAssignment(New BinaryOperator({":="}, ePrecedence.assignment, AddressOf BinaryOperatorAssign))
            RegisterAssignment(New BinaryOperator({"+="}, ePrecedence.assignment, AddressOf BinaryOperatorAddAssign))
            RegisterAssignment(New BinaryOperator({"-="}, ePrecedence.assignment, AddressOf BinaryOperatorSubtractAssign))
            RegisterAssignment(New BinaryOperator({"*="}, ePrecedence.assignment, AddressOf BinaryOperatorMultiplyAssign))
            RegisterAssignment(New BinaryOperator({"/="}, ePrecedence.assignment, AddressOf BinaryOperatorDivideAssign))
            RegisterAssignment(New BinaryOperator({"**="}, ePrecedence.assignment, AddressOf BinaryOperatorDuplicateAssign))
            RegisterAssignment(New BinaryOperator({"//="}, ePrecedence.assignment, AddressOf BinaryOperatorDivideFloorAssign))
            RegisterAssignment(New BinaryOperator({" mod=", " mod ="}, ePrecedence.assignment, AddressOf BinaryOperatorModuloAssign))
            RegisterAssignment(New BinaryOperator({"^="}, ePrecedence.assignment, AddressOf BinaryOperatorExponentAssign))

            RegisterAssignment(New BinaryOperator({"&="}, ePrecedence.assignment, AddressOf BinaryOperatorConcatAssign))

            RegisterAssignment(New BinaryOperator({"||="}, ePrecedence.assignment, AddressOf BinaryOperatorBitwiseOrAssign))
            RegisterAssignment(New BinaryOperator({"&&="}, ePrecedence.assignment, AddressOf BinaryOperatorBitwiseAndAssign))
            RegisterAssignment(New BinaryOperator({"^^="}, ePrecedence.assignment, AddressOf BinaryOperatorBitwiseXorAssign))
            RegisterAssignment(New BinaryOperator({"<<="}, ePrecedence.assignment, AddressOf BinaryOperatorShlAssign))
            RegisterAssignment(New BinaryOperator({">>="}, ePrecedence.assignment, AddressOf BinaryOperatorShrAssign))

            RegisterAssignment(New BinaryOperator({"++"}, ePrecedence.add_sub, AddressOf BinaryOperatorIncrement))
            RegisterAssignment(New BinaryOperator({"--"}, ePrecedence.add_sub, AddressOf BinaryOperatorDecrement))

            ' unary

            Register(New UnaryOperatorBefore({"!"}, ePrecedence.fact_pct, AddressOf UnaryOperatorFactorial))
            Register(New UnaryOperatorBefore({"%"}, ePrecedence.fact_pct, AddressOf UnaryOperatorPercent))
            Register(New UnaryOperatorAfter({"not "}, ePrecedence.not, AddressOf UnaryOperatorNot))
            Register(New UnaryOperatorAfter({"~"}, ePrecedence.fact_pct, AddressOf UnaryOperatorBitwiseNot))

            ' ref keyword: create reference to object (reference not saved after session)
            RegisterByRef(New UnaryOperatorAfter({"ref "}, ePrecedence.fact_pct, AddressOf UnaryOperatorReference))

            ' deref keyword: dereference the reference
            RegisterByRef(New UnaryOperatorAfter({"deref "}, ePrecedence.fact_pct, AddressOf UnaryOperatorDereference))

            ' Brackets:
            ' Register(New Bracket([start], [end], AddressOf [Definition]))
            ' OR
            ' Register(New Bracket([sign], AddressOf [Definition]))

            Register(New Bracket("$(", ")", AddressOf BracketOperatorAsync))
            Register(New Bracket("`", AddressOf BracketOperatorLambdaExpression))

            RegisterByRef(New Bracket("[", "]", AddressOf BracketOperatorListIndexSlice))
            Register(New Bracket("{", "}", AddressOf BracketOperatorDictionary))

            Register(New Bracket("(", ")", AddressOf BracketOperatorRoundBracket))
            Register(New Bracket("|", AddressOf BracketOperatorAbsoluteValue))

            Register(New Bracket("r" & ControlChars.Quote, ControlChars.Quote, AddressOf BracketOperatorRawText, False))
            Register(New Bracket(ControlChars.Quote, AddressOf BracketOperatorQuotedText))
            Register(New Bracket("'", AddressOf BracketOperatorQuotedText))

            Me.DefaultOperator = OperatorWithSign("*")
        End Sub

        ''' <summary>
        ''' Register an operator
        ''' </summary>
        ''' <param name="op"></param>
        Public Sub Register(op As [Operator])
            If TypeOf op Is Bracket Then
                Brackets.Add(DirectCast(op, Bracket)) ' for brackets, add to list of brackets instead
            Else
                Operators(op.Precedence).Add(op)
            End If
            For Each sign As String In op.Signs
                _operatorSigns(sign) = op
            Next
        End Sub

        ''' <summary>
        ''' Register an operator that passes values by a 'reference' class
        ''' </summary>
        ''' <param name="op"></param>
        Public Sub RegisterByRef(op As [Operator])
            op.ByReference = True
            Register(op)
        End Sub

        ''' <summary>
        ''' Register an assignment operator
        ''' </summary>
        ''' <param name="op"></param>
        Public Sub RegisterAssignment(op As [Operator])
            op.AssignmentOperator = True
            RegisterByRef(op)
        End Sub
#End Region

#Region "Bracket Operator Definitions"
        ' Define BRACKET OPERATORS here:
        ' format is always: Private Function BracketOperator[name](inner As String, left As EvalTypes.IEvalObject) As EvalTypes.IEvalObject
        Private Function BracketOperatorRoundBracket(inner As String, ByRef left As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            Dim res As Object = _eval.EvalExprRaw(inner, True)
            Return ObjectTypes.DetectType(res, True)
        End Function

        Private Function BracketOperatorAbsoluteValue(inner As String, ByRef left As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            Try
                Dim result As Object = _eval.EvalExprRaw(inner, True)
                Return ObjectTypes.DetectType(_eval.InternalFunctions.Abs(result))
            Catch ex As Exception
                Throw New SyntaxException("Invalid use of the || (absolute value) operator")
            End Try
        End Function

        Private Function BracketOperatorAsync(inner As String, ByRef left As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            _eval.InternalFunctions.Async(inner)
            Return New ObjectTypes.Identifier("")
        End Function

        Private Function BracketOperatorQuotedText(inner As String, ByRef left As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            Return New ObjectTypes.Text(inner).Escape()
        End Function

        Private Function BracketOperatorRawText(inner As String, ByRef left As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            Return New ObjectTypes.Text(inner).Escape(True)
        End Function

        Private Function BracketOperatorListIndexSlice(inner As String, ByRef left As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            Try
                ' first dereference
                If ObjectTypes.Reference.IsType(left) Then left = DirectCast(left, ObjectTypes.Reference).ResolveObj()
                If ObjectTypes.Matrix.IsType(left) Then ' lists
                    If String.IsNullOrWhiteSpace(inner) Then Throw New EvaluatorException("No Index Specified")
                    If inner.Contains(":") Then ' slicing 
                        Try
                            Dim resultL As Double = CDbl(CType(_eval.EvalExprRaw(inner.Remove(inner.IndexOf(":")), True), BigDecimal))
                            Dim resultR As Double = CDbl(CType(_eval.EvalExprRaw(inner.Substring(inner.IndexOf(":") + 1), True), BigDecimal))
                            left = New ObjectTypes.Matrix(DirectCast(_eval.InternalFunctions.Slice(
                                      CType(left.GetValue(), List(Of ObjectTypes.Reference)),
                                      resultL, resultR), List(Of ObjectTypes.Reference)))
                        Catch
                            Throw New EvaluatorException("Illegal Slicing Operation")
                        End Try
                    Else
                        Dim result As Object = _eval.EvalExprRaw(inner, True)
                        If Not TypeOf result Is BigDecimal Then Throw New EvaluatorException("Invalid index")
                        left = ObjectTypes.DetectType(_eval.InternalFunctions.Index(DirectCast(left.GetValue(),
                                                                                    List(Of ObjectTypes.Reference)), CInt(
                                                                                      CType(result, BigDecimal))), True)
                    End If

                ElseIf ObjectTypes.LinkedList.IsType(left) Then ' lists
                    If String.IsNullOrWhiteSpace(inner) Then Throw New EvaluatorException("No Index Specified")
                    Dim result As Object = _eval.EvalExprRaw(inner, True)
                    left = ObjectTypes.DetectType(_eval.InternalFunctions.Index(DirectCast(left.GetValue(),
                                                                                LinkedList(Of ObjectTypes.Reference)), CInt(
                                                                                          CType(result, BigDecimal))), True)
                ElseIf ObjectTypes.Tuple.IsType(left) Then ' tuples
                    If String.IsNullOrWhiteSpace(inner) Then Throw New EvaluatorException("No Index Specified")
                    If inner.Contains(":") Then ' slicing 
                        Try
                            Dim resultL As Double = Math.Truncate(CDbl(CType(_eval.EvalExprRaw(inner.Remove(inner.IndexOf(":")), True), BigDecimal)))
                            Dim resultR As Double = Math.Truncate(CDbl(CType(_eval.EvalExprRaw(inner.Substring(inner.IndexOf(":") + 1), True), BigDecimal)))
                            left = New ObjectTypes.Tuple(DirectCast(
                                       _eval.InternalFunctions.Slice(CType(left.GetValue(), ObjectTypes.Reference()), resultL, resultR), List(Of ObjectTypes.Reference)))
                        Catch
                            Throw New EvaluatorException("Illegal Slicing Operation")
                        End Try
                    Else
                        Dim result As Object = _eval.EvalExprRaw(inner, True)
                        If Not TypeOf result Is BigDecimal Then Throw New EvaluatorException("Invalid index")
                        left = ObjectTypes.DetectType(_eval.InternalFunctions.Index(DirectCast(left.GetValue(), ObjectTypes.Reference()), CInt(
                                                                                          CType(result, BigDecimal))), True)
                    End If

                ElseIf ObjectTypes.Set.IsType(left) OrElse ObjectTypes.HashSet.IsType(left)
                    Dim result As Object = _eval.EvalExprRaw(inner, True)
                    left = ObjectTypes.DetectType(_eval.InternalFunctions.Index(DirectCast(left.GetValue(), IDictionary(Of ObjectTypes.Reference, ObjectTypes.Reference)),
                                                                                            result), True)
                ElseIf ObjectTypes.Text.IsType(left) Then ' strings
                    If String.IsNullOrWhiteSpace(inner) Then Throw New EvaluatorException("No index specified")
                    If inner.Contains(":") Then ' slicing 
                        Try
                            Dim resultL As Double = Math.Truncate(CDbl(CType(_eval.EvalExprRaw(inner.Remove(inner.IndexOf(":")), True), BigDecimal)))
                            Dim resultR As Double = Math.Truncate(CDbl(CType(_eval.EvalExprRaw(inner.Substring(inner.IndexOf(":") + 1), True), BigDecimal)))
                            left = New ObjectTypes.Text(_eval.InternalFunctions.Slice(
                                                          CStr(left.GetValue()), resultL, resultR).ToString())
                        Catch
                            Throw New EvaluatorException("Illegal Slicing Operation")
                        End Try
                    Else ' normal indexing
                        Dim result As Object = _eval.EvalExprRaw(inner, True)
                        If Not TypeOf result Is BigDecimal Then Throw New EvaluatorException("Invalid index")
                        left = New ObjectTypes.Text(CStr(left.GetValue())(CInt(CType(result, BigDecimal))).ToString())
                    End If
                Else
                    Try
                        Return New ObjectTypes.Matrix("[" & inner & "]", _eval)
                    Catch 'ex2 As Exception
                        Throw New SyntaxException("Invalid list format")
                    End Try
                End If
                Return left
            Catch ex As Exception
                If TypeOf ex Is ArgumentOutOfRangeException Then
                    Throw New EvaluatorException("Index is out of range")
                Else
                    Throw New EvaluatorException("Operator [] Error: " & ex.Message)
                End If
            End Try
        End Function

        Private Function BracketOperatorDictionary(inner As String, ByRef left As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            Try
                Return New ObjectTypes.Set("{" & inner & "}", _eval)
            Catch
                Throw New SyntaxException("Invalid dictionary format")
            End Try
        End Function

        Private Function BracketOperatorLambdaExpression(inner As String, ByRef left As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            Return New ObjectTypes.Lambda("`" & inner & "`")
        End Function
#End Region

#Region "Unary Operator Definitions"
        ' Define UNARY OPERATORS here:
        ' format is always: Private Function UnaryOperator[name](value As EvalTypes.IEvalObject) As EvalTypes.IEvalObject

        Private Function UnaryOperatorFactorial(value As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            Dim v As Object = value.GetValue()
            If ObjectTypes.Number.IsType(value) Then
                Return New ObjectTypes.Number(_eval.InternalFunctions.Factorial(CDbl(v)))
            Else
                Throw New SyntaxException("Only Number types may be used with the ! (factorial) operator")
            End If
        End Function

        Private Function UnaryOperatorPercent(value As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            Dim v As Object = value.GetValue()
            If ObjectTypes.Number.IsType(value) Then
                Return New ObjectTypes.Number(CDbl(v) / 100)
            Else
                Throw New SyntaxException("Only Number types may be used with the ! (factorial) operator")
            End If
        End Function

        Private Function UnaryOperatorNot(value As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            Dim v As Object = value.GetValue()
            If ObjectTypes.Boolean.IsType(value) Then
                Try
                    Return New ObjectTypes.Boolean(Not CBool(_eval.EvalExprRaw(v.ToString(), True)))
                Catch
                    Throw New SyntaxException("Operator not: Expression must produce a boolean value")
                End Try
            ElseIf ObjectTypes.Number.IsType(value) AndAlso Not Double.IsNaN(CDbl(v)) Then
                Return New ObjectTypes.Boolean(Math.Round(CLng(v), 15) = 0)
            Else
                Throw New SyntaxException("Invalid type for the logical not operator (only booleans and numbers allowed)")
            End If
        End Function

        Private Function UnaryOperatorBitwiseNot(value As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            Dim v As Object = value.GetValue()
            If ObjectTypes.Boolean.IsType(value) Then
                Try
                    Return New ObjectTypes.Boolean(Not CBool(_eval.EvalExprRaw(v.ToString(), True)))
                Catch
                    Throw New SyntaxException("Invalid type for the bitwise not operator")
                End Try
            ElseIf ObjectTypes.Number.IsType(value) AndAlso Not Double.IsNaN(CDbl(v)) Then
                Return New ObjectTypes.Number(Not CLng(v))
            Else
                Throw New SyntaxException("Invalid type for the bitwise not operator")
            End If
        End Function

        ''' <summary>
        ''' Operator to create a new reference
        ''' </summary>
        Private Function UnaryOperatorReference(value As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            Return New ObjectTypes.Reference(value)
        End Function

        ''' <summary>
        ''' Operator to dereference a reference
        ''' </summary>
        Private Function UnaryOperatorDereference(value As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            If ObjectTypes.Reference.IsType(value) Then
                Return DirectCast(value, ObjectTypes.Reference).GetRefObject()
            Else
                Throw New SyntaxException("Dereference (deref) operator works only for references")
            End If
        End Function
#End Region

#Region "Binary Operator Definitions"
        ' Define BINARY OPERATORS here:
        ' format is always: Private Function BinaryOperator[name](left As EvalTypes.IEvalObject, 
        '                                                   right As EvalTypes.IEvalObject) As EvalTypes.IEvalObject

        Private Function BinaryOperatorAdd(left As ObjectTypes.EvalObjectBase, right As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            If left Is Nothing Then left = New ObjectTypes.Number(0)
            If ObjectTypes.Number.IsType(left) And ObjectTypes.Complex.IsType(right) Then Return BinaryOperatorAdd(right, left)
            Dim lv As Object = left.GetValue()
            Dim rv As Object = right.GetValue()

            If ObjectTypes.Number.IsType(left) And ObjectTypes.Number.IsType(right) Then
                Return New ObjectTypes.Number(CType(left, ObjectTypes.Number).BigDecValue() +
                                              CType(right, ObjectTypes.Number).BigDecValue())

            ElseIf ObjectTypes.Complex.IsType(left) And ObjectTypes.Complex.IsType(right) Then
                Return New ObjectTypes.Complex(CType(lv, Numerics.Complex) + CType(lv, Numerics.Complex))
            ElseIf ObjectTypes.Complex.IsType(left) And ObjectTypes.Number.IsType(right) Then
                Return New ObjectTypes.Complex(CType(lv, Numerics.Complex) + CDbl(rv))

            ElseIf ObjectTypes.DateTime.IsType(left) And ObjectTypes.DateTime.IsType(right) Then
                If TypeOf lv Is DateTime Then lv = New TimeSpan(CDate(lv).Ticks)
                If TypeOf rv Is DateTime Then rv = New TimeSpan(CDate(rv).Ticks)
                Return New ObjectTypes.DateTime(CType(lv, TimeSpan) + (CType(rv, TimeSpan)))

            ElseIf ObjectTypes.Text.IsType(left) OrElse ObjectTypes.Text.IsType(right) Then
                ' do not append "NaN"
                If lv.ToString() = "NaN" Then lv = ""
                If rv.ToString() = "NaN" Then rv = ""
                Return New ObjectTypes.Text(lv.ToString() & rv.ToString())

            ElseIf ObjectTypes.Matrix.IsType(left) And ObjectTypes.Matrix.IsType(right) Then
                Dim lstl As List(Of ObjectTypes.Reference) = DirectCast(lv, List(Of ObjectTypes.Reference))
                Dim lstr As List(Of ObjectTypes.Reference) = DirectCast(rv, List(Of ObjectTypes.Reference))
                Dim newLst As New List(Of ObjectTypes.Reference)
                For i As Integer = 0 To Math.Max(lstl.Count, lstr.Count) - 1

                    Dim objL As ObjectTypes.EvalObjectBase = If(i >= lstl.Count, New ObjectTypes.Number(0), lstl(i).ResolveObj())
                    Dim objR As ObjectTypes.EvalObjectBase = If(i >= lstr.Count, New ObjectTypes.Number(0), lstr(i).ResolveObj())

                    If Not (DirectCast(left, ObjectTypes.Matrix).Height = 1 AndAlso
                        DirectCast(right, ObjectTypes.Matrix).Height = 1) Then
                        If Not ObjectTypes.Matrix.IsType(objL) Then objL = New ObjectTypes.Matrix({objL})
                        If Not ObjectTypes.Matrix.IsType(objR) Then objR = New ObjectTypes.Matrix({objR})
                    End If

                    Dim sum As ObjectTypes.EvalObjectBase = BinaryOperatorAdd(objL, objR)

                    If ObjectTypes.Reference.IsType(sum) Then
                        newLst.Add(DirectCast(sum, ObjectTypes.Reference))
                    Else
                        newLst.Add(New ObjectTypes.Reference(sum))
                    End If
                Next
                Return New ObjectTypes.Matrix(newLst)

            ElseIf ObjectTypes.Matrix.IsType(left) And (ObjectTypes.Set.IsType(right) OrElse ObjectTypes.HashSet.IsType(right)) Then
                Dim lst As List(Of ObjectTypes.Reference) = DirectCast(lv, List(Of ObjectTypes.Reference))
                lst.AddRange(_eval.InternalFunctions.ToMatrix(DirectCast(
                              rv, IDictionary(Of ObjectTypes.Reference, ObjectTypes.Reference))))
                Return New ObjectTypes.Matrix(lst)

            ElseIf ObjectTypes.Matrix.IsType(left) Then
                Dim lst As List(Of ObjectTypes.Reference) = DirectCast(lv, List(Of ObjectTypes.Reference))
                lst.Add(New ObjectTypes.Reference(right))
                Return New ObjectTypes.Matrix(lst)

            ElseIf ObjectTypes.LinkedList.IsType(left) Then
                Dim lst As LinkedList(Of ObjectTypes.Reference) = DirectCast(lv, LinkedList(Of ObjectTypes.Reference))
                lst.AddLast(New ObjectTypes.Reference(right))
                Return New ObjectTypes.LinkedList(lst)

            ElseIf ObjectTypes.Set.IsType(left) OrElse ObjectTypes.HashSet.IsType(left) Then
                Dim dict As IDictionary(Of ObjectTypes.Reference, ObjectTypes.Reference) =
                CType(lv, IDictionary(Of ObjectTypes.Reference, ObjectTypes.Reference))

                If ObjectTypes.Set.IsType(right) OrElse ObjectTypes.HashSet.IsType(right) Then
                    ' union
                    dict = _eval.InternalFunctions.Union(
                        dict, CType(rv, IDictionary(Of ObjectTypes.Reference, ObjectTypes.Reference)))
                ElseIf ObjectTypes.Matrix.IsType(right) Then
                    dict = _eval.InternalFunctions.Union(dict, _eval.InternalFunctions.ToSet(CType(rv, List(Of ObjectTypes.Reference))))
                Else
                    dict(New ObjectTypes.Reference(rv)) = Nothing
                End If
                Return ObjectTypes.DetectType(dict)

            ElseIf ObjectTypes.Matrix.IsType(right) OrElse TypeOf right Is ObjectTypes.LinkedList
                Return BinaryOperatorAdd(right, left)

            ElseIf ObjectTypes.Boolean.IsType(left) And ObjectTypes.Boolean.IsType(right) Then
                Return BinaryOperatorOr(left, right) ' + = OR

            Else
                Throw New SyntaxException("Invalid Addition")
            End If
        End Function

        Private Function BinaryOperatorSubtract(left As ObjectTypes.EvalObjectBase, right As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            If left Is Nothing Then left = New ObjectTypes.Number(0)

            Dim lv As Object = left.GetValue()
            Dim rv As Object = right.GetValue()
            If ObjectTypes.Number.IsType(left) And ObjectTypes.Number.IsType(right) Then
                Return New ObjectTypes.Number(CType(left, ObjectTypes.Number).BigDecValue() -
                                              CType(right, ObjectTypes.Number).BigDecValue())

            ElseIf ObjectTypes.Complex.IsType(left) And ObjectTypes.Complex.IsType(right) Then
                Return New ObjectTypes.Complex(CType(lv, Numerics.Complex) - CType(lv, Numerics.Complex))
            ElseIf ObjectTypes.Complex.IsType(left) And ObjectTypes.Number.IsType(right) Then
                Return New ObjectTypes.Complex(CType(lv, Numerics.Complex) - CDbl(rv))
            ElseIf ObjectTypes.Number.IsType(left) And ObjectTypes.Complex.IsType(right) Then
                Return New ObjectTypes.Complex(CDbl(lv) - CType(rv, Numerics.Complex))

            ElseIf ObjectTypes.Set.IsType(left) OrElse ObjectTypes.HashSet.IsType(left) Then
                Dim dict As IDictionary(Of ObjectTypes.Reference, ObjectTypes.Reference) =
                    CType(lv, IDictionary(Of ObjectTypes.Reference, ObjectTypes.Reference))
                If ObjectTypes.Set.IsType(right) OrElse ObjectTypes.HashSet.IsType(right) Then
                    ' difference
                    dict = _eval.InternalFunctions.Difference(dict, CType(rv, IDictionary(Of ObjectTypes.Reference, ObjectTypes.Reference)))
                ElseIf ObjectTypes.Matrix.IsType(right) Then
                    dict = _eval.InternalFunctions.Difference(dict, _eval.InternalFunctions.ToSet(CType(rv, List(Of ObjectTypes.Reference))))
                Else
                    dict(New ObjectTypes.Reference(rv)) = Nothing
                End If
                Return New ObjectTypes.Set(dict)

            ElseIf ObjectTypes.Matrix.IsType(left) And ObjectTypes.Matrix.IsType(right) Then
                Dim lstl As List(Of ObjectTypes.Reference) = DirectCast(lv, List(Of ObjectTypes.Reference))
                Dim lstr As List(Of ObjectTypes.Reference) = DirectCast(rv, List(Of ObjectTypes.Reference))
                For i As Integer = 0 To Math.Min(lstl.Count, lstr.Count) - 1
                    Dim sum As ObjectTypes.EvalObjectBase = BinaryOperatorSubtract(lstl(i).ResolveObj(), lstr(i).ResolveObj())
                    If ObjectTypes.Reference.IsType(sum) Then
                        lstl(i) = DirectCast(sum, ObjectTypes.Reference)
                    Else
                        lstl(i) = New ObjectTypes.Reference(sum)
                    End If
                Next
                Dim mat As New ObjectTypes.Matrix(lstl)
                Return mat

            ElseIf ObjectTypes.DateTime.IsType(left) And ObjectTypes.DateTime.IsType(right) Then
                If TypeOf lv Is DateTime Then lv = New TimeSpan(CDate(lv).Ticks)
                If TypeOf rv Is DateTime Then rv = New TimeSpan(CDate(rv).Ticks)
                Return New ObjectTypes.DateTime(CType(lv, TimeSpan) - CType(rv, TimeSpan))

            Else
                Throw New SyntaxException("Invalid Subtraction")
            End If
        End Function

        Private Function BinaryOperatorMultiply(left As ObjectTypes.EvalObjectBase, right As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            If TypeOf left Is ObjectTypes.Lambda Then
                Dim lambda As ObjectTypes.Lambda = DirectCast(left, ObjectTypes.Lambda)
                If right Is Nothing OrElse TypeOf right Is ObjectTypes.Number AndAlso Double.IsNaN(CDbl(right.GetValue())) Then
                    If lambda.Args.Count > 0 Then Throw New EvaluatorException("(Lambda): " & lambda.Args.Count & " parameters expected")
                    Return ObjectTypes.DetectType(lambda.Execute(_eval, New List(Of ObjectTypes.Reference)))
                ElseIf TypeOf right Is ObjectTypes.Tuple
                    If lambda.Args.Count <> DirectCast(right.GetValue(), ObjectTypes.Reference()).Length Then
                        Throw New EvaluatorException("(Lambda): " & lambda.Args.Count & " parameters expected")
                    End If
                    Return ObjectTypes.DetectType(lambda.Execute(_eval, New List(Of ObjectTypes.Reference)(
                                                                   DirectCast(right.GetValue(), ObjectTypes.Reference()))))
                ElseIf TypeOf right Is ObjectTypes.Reference
                    If lambda.Args.Count > 1 Then Throw New EvaluatorException("(Lambda): " & lambda.Args.Count & " parameters expected")
                    Return ObjectTypes.DetectType(lambda.Execute(_eval, New List(Of ObjectTypes.Reference)({
                                                   DirectCast(right, ObjectTypes.Reference)})))
                Else
                    If lambda.Args.Count > 1 Then Throw New EvaluatorException("(Lambda): " & lambda.Args.Count & " parameters expected")
                    Return ObjectTypes.DetectType(lambda.Execute(_eval, New List(Of ObjectTypes.Reference)({
                                                   New ObjectTypes.Reference(right)})))
                End If
            End If

            If left Is Nothing Then Throw New SyntaxException("Invalid Multiplication")
            If (ObjectTypes.Number.IsType(left) And ObjectTypes.DateTime.IsType(right)) OrElse
               (ObjectTypes.Number.IsType(left) And ObjectTypes.Matrix.IsType(right)) OrElse
               (ObjectTypes.Number.IsType(left) And ObjectTypes.Complex.IsType(right)) OrElse
               (ObjectTypes.Number.IsType(left) And ObjectTypes.Text.IsType(right)) Then
                Return BinaryOperatorMultiply(right, left)
            End If
            Dim lv As Object = left.GetValue()
            Dim rv As Object = right.GetValue()
            If ObjectTypes.Number.IsType(left) And ObjectTypes.Number.IsType(right) Then
                Return New ObjectTypes.Number(CType(left, ObjectTypes.Number).BigDecValue() *
                                              CType(right, ObjectTypes.Number).BigDecValue())

            ElseIf ObjectTypes.Complex.IsType(left) And ObjectTypes.Complex.IsType(right) Then
                Return New ObjectTypes.Complex(CType(lv, Numerics.Complex) * CType(lv, Numerics.Complex))
            ElseIf ObjectTypes.Complex.IsType(left) And ObjectTypes.Number.IsType(right) Then
                Return New ObjectTypes.Complex(CType(lv, Numerics.Complex) * CDbl(rv))

            ElseIf ObjectTypes.DateTime.IsType(left) And ObjectTypes.Number.IsType(right) Then
                If TypeOf lv Is DateTime Then lv = New TimeSpan(CDate(lv).Ticks)
                Return New ObjectTypes.DateTime(New TimeSpan(CLng(CDbl(rv) * CType(lv, TimeSpan).Ticks)))

            ElseIf ObjectTypes.Text.IsType(left) And ObjectTypes.Number.IsType(right) Then
                Dim origstr As String = CStr(lv)
                Dim strcat As String = origstr
                While strcat.Length * 2 < origstr.Length * CDbl(rv)
                    strcat &= strcat
                End While
                While strcat.Length < origstr.Length * CDbl(rv)
                    strcat &= origstr
                End While
                Return New ObjectTypes.Text(strcat)

            ElseIf ObjectTypes.Matrix.IsType(left) And ObjectTypes.Number.IsType(right) Then
                ' scalar multiply, use ** to duplicate

                Return ObjectTypes.DetectType(_eval.InternalFunctions.Scale(CType(lv, List(Of ObjectTypes.Reference)), rv))

            ElseIf ObjectTypes.Matrix.IsType(left) And ObjectTypes.Matrix.IsType(right) Then
                ' matrix multiplication (for appropriate matrices) or dot product (for vectors)
                Return ObjectTypes.DetectType(_eval.InternalFunctions.Multiply(CType(left.GetValue(), List(Of ObjectTypes.Reference)),
                                                CType(right.GetValue(), List(Of ObjectTypes.Reference))))

            ElseIf ObjectTypes.Set.IsType(left) And ObjectTypes.Matrix.IsType(right) OrElse ObjectTypes.HashSet.IsType(left) And ObjectTypes.Matrix.IsType(right) Then
                Return New ObjectTypes.Set(_eval.InternalFunctions.Intersect(CType(
                       lv, IDictionary(Of ObjectTypes.Reference, ObjectTypes.Reference)), _eval.InternalFunctions.ToSet(CType(rv, List(Of ObjectTypes.Reference)))))

            ElseIf ObjectTypes.Matrix.IsType(left) And ObjectTypes.Set.IsType(right) OrElse ObjectTypes.HashSet.IsType(right) Then
                Return BinaryOperatorMultiply(right, left)

            ElseIf ObjectTypes.Set.IsType(left) And ObjectTypes.Set.IsType(right) OrElse ObjectTypes.HashSet.IsType(left) And ObjectTypes.Set.IsType(right) Then
                Return New ObjectTypes.Set(_eval.InternalFunctions.Intersect(CType(lv, IDictionary(Of ObjectTypes.Reference, ObjectTypes.Reference)),
                       CType(rv, IDictionary(Of ObjectTypes.Reference, ObjectTypes.Reference))))

            ElseIf ObjectTypes.Boolean.IsType(left) And ObjectTypes.Boolean.IsType(right) Then
                Return BinaryOperatorAnd(left, right) ' * = AND

            Else
                Throw New SyntaxException("Invalid Multiplication")
            End If
        End Function

        ' used for duplication
        Private Function BinaryOperatorDuplicateCross(left As ObjectTypes.EvalObjectBase, right As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            If (ObjectTypes.Number.IsType(left) And ObjectTypes.Matrix.IsType(right)) OrElse
             (ObjectTypes.Complex.IsType(left) And ObjectTypes.Matrix.IsType(right)) Then
                Return BinaryOperatorDuplicateCross(right, left)
            End If

            If ObjectTypes.Matrix.IsType(left) And (ObjectTypes.Number.IsType(right) OrElse ObjectTypes.Complex.IsType(right)) Then
                ' duplicate
                Dim lst As New List(Of ObjectTypes.Reference)(CType(left.GetValue(), List(Of ObjectTypes.Reference)))
                Dim lv As Object = left.GetDeepCopy().GetValue()
                Dim rv As Object = right.GetValue()
                Dim origlst As New List(Of ObjectTypes.Reference)(lst)
                While lst.Count * 2 < origlst.Count * CDbl(rv)
                    lst.AddRange(DirectCast(New ObjectTypes.Matrix(lst).GetDeepCopy().GetValue(), List(Of ObjectTypes.Reference)))
                End While
                While lst.Count < origlst.Count * CDbl(rv)
                    lst.AddRange(DirectCast(New ObjectTypes.Matrix(origlst).GetDeepCopy().GetValue(), List(Of ObjectTypes.Reference)))
                End While
                Return New ObjectTypes.Matrix(lst)

            ElseIf ObjectTypes.Matrix.IsType(left) And ObjectTypes.Matrix.IsType(right) Then
                ' cross product
                Return New ObjectTypes.Matrix(CType(_eval.InternalFunctions.Cross(CType(left.GetValue(), List(Of ObjectTypes.Reference)),
                                                    CType(right.GetValue(), List(Of ObjectTypes.Reference))), List(Of ObjectTypes.Reference)))
            Else
                Return BinaryOperatorMultiply(left, right)
            End If
        End Function

        Private Function BinaryOperatorDivide(left As ObjectTypes.EvalObjectBase, right As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            If left Is Nothing OrElse right Is Nothing Then Throw New SyntaxException("Invalid Division")
            Dim lv As Object = left.GetValue()
            Dim rv As Object = right.GetValue()
            If ObjectTypes.Number.IsType(left) And ObjectTypes.Number.IsType(right) Then
                Return New ObjectTypes.Number(CType(left, ObjectTypes.Number).BigDecValue() / CType(right, ObjectTypes.Number).BigDecValue())

            ElseIf ObjectTypes.Complex.IsType(left) And ObjectTypes.Complex.IsType(right) Then
                Return New ObjectTypes.Complex(CType(lv, Numerics.Complex) / CType(lv, Numerics.Complex))
            ElseIf ObjectTypes.Complex.IsType(left) And ObjectTypes.Number.IsType(right) Then
                Return New ObjectTypes.Complex(CType(lv, Numerics.Complex) / CDbl(rv))
            ElseIf ObjectTypes.Number.IsType(left) And ObjectTypes.Complex.IsType(right) Then
                Return New ObjectTypes.Complex(CDbl(lv) / CType(rv, Numerics.Complex))

            ElseIf ObjectTypes.Matrix.IsType(left) And ObjectTypes.Number.IsType(right) Then
                ' scalar division

                Return ObjectTypes.DetectType(_eval.InternalFunctions.Scale(CType(lv, List(Of ObjectTypes.Reference)),
                                                        BinaryOperatorDivide(New ObjectTypes.Number(1), right).GetValue()))

            ElseIf ObjectTypes.DateTime.IsType(left) And ObjectTypes.Number.IsType(right) Then
                If TypeOf lv Is DateTime Then lv = New TimeSpan(CDate(lv).Ticks)
                Return New ObjectTypes.DateTime(New TimeSpan(CLng(CType(lv, TimeSpan).Ticks / CDbl(rv))))

            Else
                Throw New SyntaxException("Invalid Division")
            End If
        End Function

        Private Function BinaryOperatorDivideFloor(left As ObjectTypes.EvalObjectBase, right As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            If left Is Nothing OrElse right Is Nothing Then Throw New SyntaxException("Invalid Floor Div.")
            Dim lv As Object = left.GetValue()
            Dim rv As Object = right.GetValue()
            If ObjectTypes.Number.IsType(left) And ObjectTypes.Number.IsType(right) Then
                Return New ObjectTypes.Number(Math.Floor(CDbl(CType(left, ObjectTypes.Number).BigDecValue() /
                                                         CType(right, ObjectTypes.Number).BigDecValue())))

            ElseIf ObjectTypes.Complex.IsType(left) Or ObjectTypes.Complex.IsType(right) Then
                Dim cplx As ObjectTypes.Complex = CType(BinaryOperatorDivide(left, right), ObjectTypes.Complex)
                Return New ObjectTypes.Complex(Math.Floor(cplx.Real), Math.Floor(cplx.Imag))

            Else
                Throw New SyntaxException("Invalid Floor Div.")
            End If
        End Function

        Private Function BinaryOperatorModulo(left As ObjectTypes.EvalObjectBase, right As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            If left Is Nothing Then Throw New SyntaxException("Invalid Modulo")
            Dim lv As Object = left.GetValue()
            Dim rv As Object = right.GetValue()
            If ObjectTypes.Number.IsType(left) And ObjectTypes.Number.IsType(right) Then
                Return New ObjectTypes.Number(_eval.InternalFunctions.Modulo(CDbl(lv), CDbl(rv)))

            ElseIf ObjectTypes.DateTime.IsType(left) And ObjectTypes.Number.IsType(right) Then
                If TypeOf lv Is DateTime Then lv = New TimeSpan(CDate(lv).Ticks)
                Return New ObjectTypes.DateTime(New TimeSpan(CLng(CType(lv, TimeSpan).Ticks Mod CDbl(rv))))
            Else
                Throw New SyntaxException("Invalid Modulo")
            End If
        End Function

        Private Function BinaryOperatorExponent(left As ObjectTypes.EvalObjectBase, right As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            If left Is Nothing Then Throw New SyntaxException("Invalid Exponent")
            Try
                Return ObjectTypes.DetectType(_eval.InternalFunctions.Pow(left.GetValue(), right.GetValue()))
            Catch 'ex As Exception
                Throw New SyntaxException("Invalid Exponent")
            End Try
        End Function

        Private Function BinaryOperatorOr(left As ObjectTypes.EvalObjectBase, right As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            Dim lv As Object = left.GetValue()
            Dim rv As Object = right.GetValue()
            If ObjectTypes.Boolean.IsType(left) And ObjectTypes.Boolean.IsType(right) Then
                Try
                    Return New ObjectTypes.Boolean(CBool(_eval.InternalFunctions.Eval(lv.ToString())) OrElse
                                             CBool(_eval.InternalFunctions.Eval(rv.ToString())))
                Catch
                    Throw New SyntaxException("Operator or: Expression must produce a boolean value")
                End Try

            ElseIf ObjectTypes.Number.IsType(left) AndAlso ObjectTypes.Number.IsType(right) AndAlso
                Not Double.IsNaN(CDbl(lv)) AndAlso Not Double.IsNaN(CDbl(rv)) Then
                Return New ObjectTypes.Boolean(Math.Round(CDbl(lv), 15) <> 0 Or Math.Round(CDbl(rv), 15) <> 0)
            Else
                Throw New SyntaxException("Invalid logical or operation")
            End If
        End Function

        Private Function BinaryOperatorBitwiseOr(left As ObjectTypes.EvalObjectBase, right As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            If left Is Nothing OrElse right Is Nothing Then Return New ObjectTypes.Number(Double.NaN)
            Dim lv As Object = left.GetValue()
            Dim rv As Object = right.GetValue()
            If ObjectTypes.Boolean.IsType(left) And ObjectTypes.Boolean.IsType(right) Then
                Try
                    Return New ObjectTypes.Boolean(CBool(_eval.InternalFunctions.Eval(lv.ToString())) OrElse
                                             CBool(_eval.InternalFunctions.Eval(rv.ToString())))
                Catch
                    Throw New SyntaxException("Invalid bitwise or operation")
                End Try

            ElseIf ObjectTypes.Number.IsType(left) AndAlso ObjectTypes.Number.IsType(right) AndAlso
                Not Double.IsNaN(CDbl(lv)) AndAlso Not Double.IsNaN(CDbl(rv)) Then
                Return New ObjectTypes.Number(CLng(lv) Or CLng(rv))
            Else
                Throw New SyntaxException("Invalid bitwise or operation")
            End If
        End Function

        Private Function BinaryOperatorAnd(left As ObjectTypes.EvalObjectBase, right As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            Dim lv As Object = left.GetValue()
            Dim rv As Object = right.GetValue()
            If ObjectTypes.Boolean.IsType(left) AndAlso ObjectTypes.Boolean.IsType(right) Then
                Try
                    Return New ObjectTypes.Boolean(CBool(_eval.InternalFunctions.Eval(lv.ToString())) AndAlso
                                             CBool(_eval.InternalFunctions.Eval(rv.ToString())))
                Catch
                    Throw New SyntaxException("Operator and: Expression must produce a boolean value")
                End Try
            ElseIf ObjectTypes.Number.IsType(left) AndAlso ObjectTypes.Number.IsType(right) AndAlso
                Not Double.IsNaN(CDbl(lv)) AndAlso Not Double.IsNaN(CDbl(rv)) Then
                Return New ObjectTypes.Boolean(Math.Round(CDbl(lv), 15) <> 0 And Math.Round(CDbl(rv), 15) <> 0)
            Else
                Throw New SyntaxException("Invalid logical and operation")
            End If
        End Function

        Private Function BinaryOperatorBitwiseAnd(left As ObjectTypes.EvalObjectBase, right As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            Dim lv As Object = left.GetValue()
            Dim rv As Object = right.GetValue()
            If ObjectTypes.Boolean.IsType(left) AndAlso ObjectTypes.Boolean.IsType(right) Then
                Try
                    Return New ObjectTypes.Boolean(CBool(_eval.InternalFunctions.Eval(lv.ToString())) AndAlso
                                             CBool(_eval.InternalFunctions.Eval(rv.ToString())))
                Catch
                    Throw New SyntaxException("Invalid bitwise and operation")
                End Try
            ElseIf ObjectTypes.Number.IsType(left) AndAlso ObjectTypes.Number.IsType(right) AndAlso
                Not Double.IsNaN(CDbl(lv)) AndAlso Not Double.IsNaN(CDbl(rv)) Then
                Return New ObjectTypes.Number(CLng(lv) And CLng(rv))
            Else
                Throw New SyntaxException("Invalid bitwise and operation")
            End If
        End Function

        Private Function BinaryOperatorXor(left As ObjectTypes.EvalObjectBase, right As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            Dim lv As Object = left.GetValue()
            Dim rv As Object = right.GetValue()
            If ObjectTypes.Boolean.IsType(left) And ObjectTypes.Boolean.IsType(right) Then
                Try
                    Return New ObjectTypes.Boolean(CBool(_eval.InternalFunctions.Eval(lv.ToString())) Xor
                                             CBool(_eval.InternalFunctions.Eval(rv.ToString())))
                Catch
                    Throw New SyntaxException("Operator xor: Expression must produce a boolean value")
                End Try
            ElseIf ObjectTypes.Number.IsType(left) AndAlso ObjectTypes.Number.IsType(right) AndAlso
                Not Double.IsNaN(CDbl(lv)) AndAlso Not Double.IsNaN(CDbl(rv)) Then
                Return New ObjectTypes.Boolean(Math.Round(CDbl(lv), 15) <> 0 Xor Math.Round(CDbl(rv), 15) <> 0)
            Else
                Throw New SyntaxException("Invalid logical xor operation")
            End If
        End Function

        Private Function BinaryOperatorBitwiseXor(left As ObjectTypes.EvalObjectBase, right As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            Dim lv As Object = left.GetValue()
            Dim rv As Object = right.GetValue()
            If ObjectTypes.Boolean.IsType(left) And ObjectTypes.Boolean.IsType(right) Then
                Try
                    Return New ObjectTypes.Boolean(CBool(_eval.InternalFunctions.Eval(lv.ToString())) Xor
                                             CBool(_eval.InternalFunctions.Eval(rv.ToString())))
                Catch
                    Throw New SyntaxException("Invalid bitwise xor operation")
                End Try
            ElseIf ObjectTypes.Number.IsType(left) AndAlso ObjectTypes.Number.IsType(right) AndAlso
                Not Double.IsNaN(CDbl(lv)) AndAlso Not Double.IsNaN(CDbl(rv)) Then
                Return New ObjectTypes.Number(CLng(lv) Xor CLng(rv))
            ElseIf ObjectTypes.Set.IsType(left) OrElse ObjectTypes.HashSet.IsType(left) Then ' this operator doubles as the symmetric difference operator for sets
                Dim dict As IDictionary(Of ObjectTypes.Reference, ObjectTypes.Reference) =
                CType(lv, IDictionary(Of ObjectTypes.Reference, ObjectTypes.Reference))

                If ObjectTypes.Set.IsType(right) OrElse ObjectTypes.HashSet.IsType(right) Then
                    ' symmetric difference
                    dict = _eval.InternalFunctions.SymmetricDifference(dict, CType(rv,
                           IDictionary(Of ObjectTypes.Reference, ObjectTypes.Reference)))
                ElseIf ObjectTypes.Matrix.IsType(right) Then
                    dict = _eval.InternalFunctions.SymmetricDifference(dict, _eval.InternalFunctions.ToSet(CType(rv, List(Of ObjectTypes.Reference))))
                Else
                    dict(New ObjectTypes.Reference(rv)) = Nothing
                End If
                Return New ObjectTypes.Set(dict)
            Else
                Throw New SyntaxException("Invalid bitwise xor operation")
            End If
        End Function

        Private Function BinaryOperatorShl(left As ObjectTypes.EvalObjectBase, right As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            Dim lv As Object = left.GetValue()
            Dim rv As Object = right.GetValue()
            If ObjectTypes.Number.IsType(left) AndAlso ObjectTypes.Number.IsType(right) AndAlso
                Not Double.IsNaN(CDbl(lv)) AndAlso Not Double.IsNaN(CDbl(rv)) Then
                Return New ObjectTypes.Number(CLng(lv) << CInt(rv))
            Else
                Throw New SyntaxException("Invalid << (bitwise shl) operation")
            End If
        End Function

        Private Function BinaryOperatorShr(left As ObjectTypes.EvalObjectBase, right As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            Dim lv As Object = left.GetValue()
            Dim rv As Object = right.GetValue()
            If ObjectTypes.Number.IsType(left) AndAlso ObjectTypes.Number.IsType(right) AndAlso
                Not Double.IsNaN(CDbl(lv)) AndAlso Not Double.IsNaN(CDbl(rv)) Then
                Return New ObjectTypes.Number(CLng(lv) >> CInt(rv))
            Else
                Throw New SyntaxException("Invalid >> (bitwise shl) operation")
            End If
        End Function

        Private Function BinaryOperatorChoose(left As ObjectTypes.EvalObjectBase, right As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            Dim lv As Object = left.GetValue()
            Dim rv As Object = right.GetValue()
            If ObjectTypes.Number.IsType(left) And ObjectTypes.Number.IsType(right) Then
                Return New ObjectTypes.Number(_eval.InternalFunctions.Comb(CDbl(lv), CDbl(rv)))
            Else
                Throw New SyntaxException("Invalid types for the choose (combinations) operator")
            End If
        End Function

        Private Function BinaryOperatorAssign(left As ObjectTypes.EvalObjectBase, right As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            Try
                If ObjectTypes.Reference.IsType(left) AndAlso ObjectTypes.Reference.IsType(right) AndAlso
                    ObjectTypes.Reference.IsType(DirectCast(right, ObjectTypes.Reference).GetRefObject()) Then
                    Dim lr As ObjectTypes.Reference = DirectCast(left, ObjectTypes.Reference)
                    Dim rr As ObjectTypes.Reference = DirectCast(right, ObjectTypes.Reference)
                    ' if we are assigning a reference

                    If Not lr Is rr Then ' try to avoid circular references
                        If Not rr.Node Is Nothing Then
                            ' set node
                            lr.SetNode(rr.Node)
                            lr.SetValue(New ObjectTypes.Reference(rr.Node))
                        Else
                            ' set object
                            left.SetValue(New ObjectTypes.Reference(rr.ResolveObj()))
                        End If
                    End If
                Else
                    If ObjectTypes.Reference.IsType(right) Then right = DirectCast(right, ObjectTypes.Reference).ResolveObj()
                    ' if we are assigning a plain value
                    left.SetValue(right.GetDeepCopy().GetValue())
                End If
                Return left
            Catch 'ex As Exception
                Throw New EvaluatorException("Assignment operation failed")
            End Try
        End Function

        Private Function BinaryOperatorOpAssign(left As ObjectTypes.EvalObjectBase, right As ObjectTypes.EvalObjectBase,
                                                op As Func(Of ObjectTypes.EvalObjectBase, ObjectTypes.EvalObjectBase, ObjectTypes.EvalObjectBase)) As ObjectTypes.EvalObjectBase
            Try

                If ObjectTypes.Reference.IsType(left) OrElse ObjectTypes.Tuple.IsType(left) Then
                    Dim lr As ObjectTypes.Reference = DirectCast(left, ObjectTypes.Reference)
                    ' assign object
                    If ObjectTypes.Reference.IsType(right) Then right = DirectCast(right, ObjectTypes.Reference).ResolveObj()
                    Return BinaryOperatorAssign(left, op(lr.ResolveObj().GetDeepCopy(), right.GetDeepCopy()))

                Else
                    Return op(left, right) ' not a reference? just use normal operator
                End If
            Catch 'ex As Exception
                Throw New EvaluatorException("Operator and assignment ([op]=) operation failed")
            End Try
        End Function


        Private Function BinaryOperatorAddAssign(left As ObjectTypes.EvalObjectBase, right As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            Return BinaryOperatorOpAssign(left, right, AddressOf BinaryOperatorAdd)
        End Function

        Private Function BinaryOperatorSubtractAssign(left As ObjectTypes.EvalObjectBase, right As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            Return BinaryOperatorOpAssign(left, right, AddressOf BinaryOperatorSubtract)
        End Function

        Private Function BinaryOperatorMultiplyAssign(left As ObjectTypes.EvalObjectBase, right As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            Return BinaryOperatorOpAssign(left, right, AddressOf BinaryOperatorMultiply)
        End Function

        Private Function BinaryOperatorDivideAssign(left As ObjectTypes.EvalObjectBase, right As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            Return BinaryOperatorOpAssign(left, right, AddressOf BinaryOperatorDivide)
        End Function

        Private Function BinaryOperatorDuplicateAssign(left As ObjectTypes.EvalObjectBase, right As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            Return BinaryOperatorOpAssign(left, right, AddressOf BinaryOperatorDuplicateCross)
        End Function

        Private Function BinaryOperatorDivideFloorAssign(left As ObjectTypes.EvalObjectBase, right As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            Return BinaryOperatorOpAssign(left, right, AddressOf BinaryOperatorDivideFloor)
        End Function

        Private Function BinaryOperatorModuloAssign(left As ObjectTypes.EvalObjectBase, right As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            Return BinaryOperatorOpAssign(left, right, AddressOf BinaryOperatorModulo)
        End Function

        Private Function BinaryOperatorExponentAssign(left As ObjectTypes.EvalObjectBase, right As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            Return BinaryOperatorOpAssign(left, right, AddressOf BinaryOperatorExponent)
        End Function

        Private Function BinaryOperatorBitwiseOrAssign(left As ObjectTypes.EvalObjectBase, right As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            Return BinaryOperatorOpAssign(left, right, AddressOf BinaryOperatorBitwiseOr)
        End Function

        Private Function BinaryOperatorBitwiseAndAssign(left As ObjectTypes.EvalObjectBase, right As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            Return BinaryOperatorOpAssign(left, right, AddressOf BinaryOperatorBitwiseAnd)
        End Function

        Private Function BinaryOperatorBitwiseXorAssign(left As ObjectTypes.EvalObjectBase, right As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            Return BinaryOperatorOpAssign(left, right, AddressOf BinaryOperatorBitwiseXor)
        End Function

        Private Function BinaryOperatorShlAssign(left As ObjectTypes.EvalObjectBase, right As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            Return BinaryOperatorOpAssign(left, right, AddressOf BinaryOperatorShl)
        End Function

        Private Function BinaryOperatorShrAssign(left As ObjectTypes.EvalObjectBase, right As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            Return BinaryOperatorOpAssign(left, right, AddressOf BinaryOperatorShr)
        End Function

        Private Function BinaryOperatorConcatAssign(left As ObjectTypes.EvalObjectBase, right As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            Return BinaryOperatorOpAssign(left, right, AddressOf BinaryOperatorConcat)
        End Function

        Private Function BinaryOperatorIncrement(left As ObjectTypes.EvalObjectBase, right As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            Try
                If ObjectTypes.Reference.IsType(right) Then right = DirectCast(right, ObjectTypes.Reference).GetRefObject()
                If ObjectTypes.Reference.IsType(left) AndAlso (right Is Nothing OrElse right.ToString() = "NaN") Then
                    Dim lv As Object = CType(left, ObjectTypes.Reference).Resolve()
                    If TypeOf lv Is Double OrElse TypeOf lv Is BigDecimal Then
                        left.SetValue(CDbl(lv) + 1)  ' add one to numbers
                    ElseIf TypeOf lv Is BigDecimal Then
                        left.SetValue(CType(lv, BigDecimal) + 1) ' add one to numbers
                    ElseIf TypeOf lv Is Date
                        left.SetValue(CDate(lv).AddDays(1)) ' add one day to dates
                    ElseIf TypeOf lv Is TimeSpan
                        left.SetValue(CType(lv, TimeSpan).Add(New TimeSpan(0, 1, 0))) ' add one minute to timespans
                    Else
                        'otherwise ??? we don't know what to do, so we'll try the normal add operation.
                        Return BinaryOperatorAdd(left, right)
                    End If
                    Return left
                Else
                    If ObjectTypes.Reference.IsType(left) Then left = DirectCast(left, ObjectTypes.Reference).ResolveObj()
                    If ObjectTypes.Reference.IsType(right) Then right = DirectCast(right, ObjectTypes.Reference).ResolveObj()
                    ' if it is not a reference we see the operation as ++, for example in 1++1 = 2
                    Return BinaryOperatorAdd(left, right)
                End If
                Return left
            Catch
                Throw New EvaluatorException("Invalid increment (++) operation")
            End Try
        End Function

        Private Function BinaryOperatorDecrement(left As ObjectTypes.EvalObjectBase, right As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            Try
                If ObjectTypes.Reference.IsType(right) Then right = DirectCast(right, ObjectTypes.Reference).GetRefObject()
                If ObjectTypes.Reference.IsType(left) AndAlso (right Is Nothing OrElse right.ToString() = "NaN") Then
                    Dim lv As Object = CType(left, ObjectTypes.Reference).Resolve()
                    If TypeOf lv Is Double Then
                        left.SetValue(CDbl(lv) - 1)  ' subtract one from numbers
                    ElseIf TypeOf lv Is BigDecimal Then
                        left.SetValue(CType(lv, BigDecimal) - 1)  ' subtract one from numbers
                    ElseIf TypeOf lv Is Date
                        left.SetValue(CDate(lv).AddDays(-1)) ' subtract one day from dates
                    ElseIf TypeOf lv Is TimeSpan
                        left.SetValue(CType(lv, TimeSpan).Add(New TimeSpan(0, -1, 0))) ' subtract one minute from timespans
                    Else
                        'otherwise ??? we don't know what to do, so we'll try the normal subtract operation.
                        Return BinaryOperatorSubtract(left, right)
                    End If
                    Return left
                Else
                    ' if it is not a reference we see the operation as --, for example in 1--1 = 2
                    Return BinaryOperatorAdd(left, right)
                End If
            Catch
                Throw New EvaluatorException("Invalid decrement (--) operation")
            End Try
        End Function

        ''' <summary>
        ''' 'Smart' equals operator that functions as an assignment or equalTo operator as needed
        ''' </summary>
        ''' <param name="left"></param>
        ''' <param name="right"></param>
        ''' <returns></returns>
        Private Function BinaryOperatorAutoEqual(left As ObjectTypes.EvalObjectBase, right As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            ' if it is a reference, tuple, or identifier and we are not in conditional mode then we assign to it
            If (ObjectTypes.Reference.IsType(left) OrElse ObjectTypes.Tuple.IsType(left) OrElse
                ObjectTypes.Tuple.IsType(right)) AndAlso Not ConditionMode Then
                Return BinaryOperatorAssign(left, right)
            Else
                Return BinaryOperatorEqualTo(left, right) ' otherwise we compare to the right side
            End If
        End Function

        Private Function BinaryOperatorEqualTo(left As ObjectTypes.EvalObjectBase, right As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            Return New ObjectTypes.Boolean(GenericComparer.CompareObjs(left, right) = 0)
        End Function

        Private Function BinaryOperatorNotEqualTo(left As ObjectTypes.EvalObjectBase, right As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            Return UnaryOperatorNot(BinaryOperatorEqualTo(left, right))
        End Function

        Private Function BinaryOperatorGreaterThan(left As ObjectTypes.EvalObjectBase, right As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            Return New ObjectTypes.Boolean(GenericComparer.CompareObjs(left, right) = 1)
        End Function

        Private Function BinaryOperatorGreaterThanOrEqualTo(left As ObjectTypes.EvalObjectBase, right As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            Return New ObjectTypes.Boolean(GenericComparer.CompareObjs(left, right) >= 0)
        End Function


        Private Function BinaryOperatorLessThan(left As ObjectTypes.EvalObjectBase, right As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            Return UnaryOperatorNot(BinaryOperatorGreaterThanOrEqualTo(left, right))
        End Function

        Private Function BinaryOperatorLessThanOrEqualTo(left As ObjectTypes.EvalObjectBase, right As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            Return UnaryOperatorNot(BinaryOperatorGreaterThan(left, right))
        End Function


        Private Function BinaryOperatorConcat(left As ObjectTypes.EvalObjectBase, right As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            Dim lv As Object = left.GetValue()
            Dim rv As Object = right.GetValue()
            Try
                ' do not append NaN
                If rv.ToString() = "NaN" Then rv = ""
                If lv.ToString() = "NaN" Then lv = ""
                Return New ObjectTypes.Text(_eval.InternalFunctions.O(lv).Trim().Trim(ControlChars.Quote) &
                                            _eval.InternalFunctions.O(rv).Trim().Trim(ControlChars.Quote))
            Catch
                Throw New SyntaxException("Invalid concatenation")
            End Try
        End Function

        Private Function BinaryOperatorCommaTuple(left As ObjectTypes.EvalObjectBase, right As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            If left Is Nothing Then Return right
            If right Is Nothing Then Return left

            Dim lv As Object = left.GetValue()
            Dim rv As Object = right.GetValue()

            Try
                If ObjectTypes.Tuple.IsType(left) Then
                    Return New ObjectTypes.Tuple((DirectCast(lv, Object()).Concat({right})))
                Else
                    Return New ObjectTypes.Tuple({left, right})
                End If
            Catch 'ex As Exception
                Throw New SyntaxException("Invalid comma concatenation")
            End Try
        End Function

        Private Function BinaryOperatorColon(left As ObjectTypes.EvalObjectBase, right As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            If left Is Nothing Then Return right
            If right Is Nothing Then Return left
            Dim lv As Object = left.GetValue()
            Dim rv As Object = right.GetValue()
            Try
                If rv Is Nothing OrElse rv.ToString() = "NaN" Then Return left
                If ObjectTypes.Tuple.IsType(left) Then
                    Dim lst As New List(Of ObjectTypes.Reference)(DirectCast(lv, ObjectTypes.Reference()))
                    lst(lst.Count - 1) = New ObjectTypes.Reference(BinaryOperatorCommaTuple(lst(lst.Count - 1).GetRefObject(), right))
                    Return New ObjectTypes.Tuple(lst)
                Else
                    Dim ref As New ObjectTypes.Reference(New ObjectTypes.Tuple({lv, rv}))
                    Return New ObjectTypes.Tuple({ref})
                End If
            Catch 'ex As Exception
                Throw New SyntaxException("Invalid comma concatenation")
            End Try
        End Function

        Private Function BinaryOperatorExp10(left As ObjectTypes.EvalObjectBase, right As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            Dim lv As Object = left.GetValue()
            Dim rv As Object = right.GetValue()
            If ObjectTypes.Number.IsType(left) And ObjectTypes.Number.IsType(right) Then
                Return New ObjectTypes.Number(CDbl(lv) * BigDecimal.Pow(10, CLng(rv)))
            Else
                Throw New SyntaxException("Invalid types for the E (exp10) operator")
            End If
        End Function
#End Region
    End Class
End Namespace