Imports Cantus.Calculator.Evaluator.Exceptions
Imports Cantus.Calculator.Evaluator.CommonTypes
Namespace Calculator.Evaluator
    Public Class OperatorRegistar
#Region "Enums"
        ''' <summary>
        ''' Operator precedence. The higher, the earlier the operator is executed.
        ''' </summary>
        Public Enum Precedence
            assignment = 0 ' Note: assignment operators are evaluate RTL so you can do a=b=c chaining, whereas all others are LTR
            tupling
            [or]
            [and]
            [not]
            comparison
            bitshift_concat_frac
            add_sub
            mul_div
            exponent
            fact_pct
        End Enum
#End Region

#Region "Operator Types"

        ''' <summary>
        ''' Base class for all operators
        ''' </summary>
        Public MustInherit Class [Operator]
            MustOverride ReadOnly Property Signs As List(Of String)
            MustOverride ReadOnly Property Precedence As Precedence
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
            Public Overrides ReadOnly Property Precedence As Precedence
            Public ReadOnly Property Execute As Func(Of ObjectTypes.EvalObjectBase, ObjectTypes.EvalObjectBase, ObjectTypes.EvalObjectBase)
            ''' <summary>
            ''' Initialize a new binary operator
            ''' </summary>
            ''' <param name="signs">The list of signs to register for the operator</param>
            ''' <param name="precedence">The precedence of the operator</param>
            ''' <param name="execute">The operator definition, specified as a function (use AddressOf ...)</param>
            Public Sub New(signs As String(), precedence As Precedence, execute As Func(Of ObjectTypes.EvalObjectBase, ObjectTypes.EvalObjectBase, ObjectTypes.EvalObjectBase))
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
            Public Overrides ReadOnly Property Precedence As Precedence
            Public ReadOnly Property Execute As Func(Of ObjectTypes.EvalObjectBase, ObjectTypes.EvalObjectBase)
            ''' <summary>
            ''' Initialize a new unary operator
            ''' </summary>
            ''' <param name="signs">The list of signs to register for the operator</param>
            ''' <param name="execute">The operator definition, specified as a function (use AddressOf ...)</param>
            Public Sub New(signs As String(), precedence As Precedence, execute As Func(Of ObjectTypes.EvalObjectBase, ObjectTypes.EvalObjectBase))
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
            Public Sub New(signs As String(), precedence As Precedence, execute As Func(Of ObjectTypes.EvalObjectBase, ObjectTypes.EvalObjectBase))
                MyBase.New(signs, precedence, execute)
            End Sub
        End Class

        ''' <summary>
        ''' An operator involving a single value after it (for example, 'not x')
        ''' </summary>
        Public Class UnaryOperatorAfter
            Inherits UnaryOperator
            Public Sub New(signs As String(), precedence As Precedence, execute As Func(Of ObjectTypes.EvalObjectBase, ObjectTypes.EvalObjectBase))
                MyBase.New(signs, precedence, execute)
            End Sub
        End Class

        ''' <summary>
        ''' A 'bracket' type operator (e.g. (), [], ||)
        ''' Evaluated before all other operators, on tokenization
        ''' </summary>
        Public Class Bracket : Inherits [Operator]
            Public Overrides ReadOnly Property Signs As New List(Of String)
            Public Overrides ReadOnly Property Precedence As Precedence
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
                Dim endSign As String = If(Me.Signs.Count < 2, startSign, Me.Signs(1))
                Dim endIdx As Integer
                Dim stackHeight As Integer = 0
                Dim lastFound As Integer = -1
                For endIdx = 0 To expr.Length - endSign.Length
                    If expr.Substring(endIdx).StartsWith(endSign) Then
                        stackHeight -= 1
                        If stackHeight < 0 Then Return endIdx
                        endIdx += endSign.Length - 1
                    ElseIf Stackable
                        If expr.Substring(endIdx).StartsWith(startSign) Then
                            stackHeight += 1
                            endIdx += startSign.Length - 1
                        Else
                            For Each b As Bracket In opReg.Brackets
                                If expr.Substring(endIdx).StartsWith(b.Signs(0)) Then
                                    ' start escape sequence
                                    If expr(endIdx) = "\"c Then
                                        lastFound = endIdx + 1 ' save the time we last found an escaped end sign
                                        endIdx += endSign.Length
                                    Else
                                        If b.Signs.Count = 1 Then
                                            endIdx += b.FindCloseBracket(expr.Substring(endIdx + b.Signs(0).Length), opReg) +
                                                b.Signs(0).Length
                                        Else ' >1
                                            endIdx += b.FindCloseBracket(expr.Substring(endIdx + b.Signs(0).Length), opReg) +
                                                b.Signs(1).Length
                                        End If
                                    End If
                                    Exit For
                                End If
                            Next
                        End If
                    End If
                Next

                ' if we can't find anything unescaped we'll ignore the last escape sequence
                Return lastFound
            End Function
        End Class

#End Region

#Region "Constants & Variable Declarations"
        ''' <summary>
        ''' Maximum length in characters of a single operator
        ''' </summary>
        Public Const MAX_OPERATOR_LENGTH As Integer = 8
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
        ''' The default operator to use when none is specified, normally *
        ''' </summary>
        ''' <returns></returns>
        Public Property DefaultOperator As [Operator]

        ''' <summary>
        ''' A hashset of operator signs to quickly check if a given name is an operator
        ''' </summary>
        Private _operatorSigns As New Dictionary(Of String, [Operator])

        ''' <summary>
        ''' Conditional mode: if on, the = operator is seen as a comparison operator always
        ''' </summary>
        Private _conditional As Boolean = False
#End Region

#Region "Public Methods"
        ''' <summary>
        ''' Create a new Operator Registar for registering and accessing operators like if/else
        ''' </summary>
        Public Sub New(parent As Evaluator, Optional ByVal conditional As Boolean = False)
            Me._eval = parent
            Me._conditional = conditional
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
            For Each lvl As Integer In [Enum].GetValues(GetType(Precedence))
                Operators.Add(New List(Of [Operator]))
            Next

            ' Register operators here 
            ' FORMAT: 
            ' Register(New [Type]Operator({[List of signs to register]}, Precedence.[Precedence], AddressOf [Definition]))

            Register(New BinaryOperator({"+"}, Precedence.add_sub, AddressOf BinaryOperatorAdd))
            Register(New BinaryOperator({"-"}, Precedence.add_sub, AddressOf BinaryOperatorSubtract))

            Register(New BinaryOperator({"*"}, Precedence.mul_div, AddressOf BinaryOperatorMultiply))
            Register(New BinaryOperator({"/"}, Precedence.mul_div, AddressOf BinaryOperatorDivide))
            Register(New BinaryOperator({"**"}, Precedence.mul_div, AddressOf BinaryOperatorDuplicateCross))
            Register(New BinaryOperator({"//"}, Precedence.mul_div, AddressOf BinaryOperatorDivideFloor))
            Register(New BinaryOperator({" mod "}, Precedence.mul_div, AddressOf BinaryOperatorModulo))

            Register(New BinaryOperator({"^"}, Precedence.exponent, AddressOf BinaryOperatorExponent))
            Register(New BinaryOperator({"&"}, Precedence.bitshift_concat_frac, AddressOf BinaryOperatorConcat))
            Register(New BinaryOperator({"\"}, Precedence.bitshift_concat_frac, AddressOf BinaryOperatorDivide))
            Register(New BinaryOperator({"<<"}, Precedence.bitshift_concat_frac, AddressOf BinaryOperatorShl))
            Register(New BinaryOperator({">>"}, Precedence.bitshift_concat_frac, AddressOf BinaryOperatorShr))

            Register(New BinaryOperator({" or "}, Precedence.or, AddressOf BinaryOperatorOr))
            Register(New BinaryOperator({"||"}, Precedence.mul_div, AddressOf BinaryOperatorBitwiseOr))
            Register(New BinaryOperator({" and "}, Precedence.and, AddressOf BinaryOperatorAnd))
            Register(New BinaryOperator({"&&"}, Precedence.mul_div, AddressOf BinaryOperatorBitwiseAnd))
            Register(New BinaryOperator({" xor "}, Precedence.or, AddressOf BinaryOperatorXor))
            Register(New BinaryOperator({"^^"}, Precedence.mul_div, AddressOf BinaryOperatorBitwiseXor))
            RegisterByRef(New BinaryOperator({","}, Precedence.tupling, AddressOf BinaryOperatorComma))
            RegisterByRef(New BinaryOperator({":"}, Precedence.tupling, AddressOf BinaryOperatorColon))

            Register(New BinaryOperator({" choose ", " c "}, Precedence.mul_div, AddressOf BinaryOperatorChoose))
            Register(New BinaryOperator({" e "}, Precedence.exponent, AddressOf BinaryOperatorExp10))

            Register(New BinaryOperator({"=="}, Precedence.comparison, AddressOf BinaryOperatorEqualTo))
            Register(New BinaryOperator({"!=", "<>"}, Precedence.comparison, AddressOf BinaryOperatorNotEqualTo))
            Register(New BinaryOperator({">"}, Precedence.comparison, AddressOf BinaryOperatorGreaterThan))
            Register(New BinaryOperator({">="}, Precedence.comparison, AddressOf BinaryOperatorGreaterThanOrEqualTo))
            Register(New BinaryOperator({"<"}, Precedence.comparison, AddressOf BinaryOperatorLessThan))
            Register(New BinaryOperator({"<="}, Precedence.comparison, AddressOf BinaryOperatorLessThanOrEqualTo))

            ' assignment (use RegisterByRef, with same format)

            RegisterAssignment(New BinaryOperator({"="}, Precedence.assignment, AddressOf BinaryOperatorAutoEqual))
            RegisterAssignment(New BinaryOperator({":="}, Precedence.assignment, AddressOf BinaryOperatorAssign))
            RegisterAssignment(New BinaryOperator({"+="}, Precedence.assignment, AddressOf BinaryOperatorAddAssign))
            RegisterAssignment(New BinaryOperator({"-="}, Precedence.assignment, AddressOf BinaryOperatorSubtractAssign))
            RegisterAssignment(New BinaryOperator({"*="}, Precedence.assignment, AddressOf BinaryOperatorMultiplyAssign))
            RegisterAssignment(New BinaryOperator({"/="}, Precedence.assignment, AddressOf BinaryOperatorDivideAssign))
            RegisterAssignment(New BinaryOperator({"**="}, Precedence.assignment, AddressOf BinaryOperatorDuplicateAssign))
            RegisterAssignment(New BinaryOperator({"//="}, Precedence.assignment, AddressOf BinaryOperatorDivideFloorAssign))
            RegisterAssignment(New BinaryOperator({" mod=", " mod ="}, Precedence.assignment, AddressOf BinaryOperatorModuloAssign))
            RegisterAssignment(New BinaryOperator({"^="}, Precedence.assignment, AddressOf BinaryOperatorExponentAssign))

            RegisterAssignment(New BinaryOperator({"&="}, Precedence.assignment, AddressOf BinaryOperatorConcatAssign))

            RegisterAssignment(New BinaryOperator({"||="}, Precedence.assignment, AddressOf BinaryOperatorBitwiseOrAssign))
            RegisterAssignment(New BinaryOperator({"&&="}, Precedence.assignment, AddressOf BinaryOperatorBitwiseAndAssign))
            RegisterAssignment(New BinaryOperator({"^^="}, Precedence.assignment, AddressOf BinaryOperatorBitwiseXorAssign))
            RegisterAssignment(New BinaryOperator({"<<="}, Precedence.assignment, AddressOf BinaryOperatorShlAssign))
            RegisterAssignment(New BinaryOperator({">>="}, Precedence.assignment, AddressOf BinaryOperatorShrAssign))

            RegisterAssignment(New BinaryOperator({"++"}, Precedence.add_sub, AddressOf BinaryOperatorIncrement))
            RegisterAssignment(New BinaryOperator({"--"}, Precedence.add_sub, AddressOf BinaryOperatorDecrement))

            ' unary

            Register(New UnaryOperatorBefore({"!"}, Precedence.fact_pct, AddressOf UnaryOperatorFactorial))
            Register(New UnaryOperatorBefore({"%"}, Precedence.fact_pct, AddressOf UnaryOperatorPercent))
            Register(New UnaryOperatorAfter({"not "}, Precedence.not, AddressOf UnaryOperatorNot))
            Register(New UnaryOperatorAfter({"~"}, Precedence.fact_pct, AddressOf UnaryOperatorBitwiseNot))

            ' ref keyword: create reference to object (not saved after session)
            RegisterByRef(New UnaryOperatorAfter({"ref "}, Precedence.fact_pct, AddressOf UnaryOperatorReference))

            ' Brackets:
            ' Register(New Bracket([start], [end], AddressOf [Definition]))
            ' OR
            ' Register(New Bracket([sign], AddressOf [Definition]))

            Register(New Bracket("$(", ")", AddressOf BracketOperatorAsync))

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
            Return ObjectTypes.DetectType(_eval.EvalExprRaw(inner, True), True)
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
                        left = ObjectTypes.DetectType(_eval.InternalFunctions.Index(DirectCast(left.GetValue(), List(Of ObjectTypes.Reference)), CInt(
                                                                                      CType(result, BigDecimal))), True)
                    End If

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
                        left = ObjectTypes.DetectType(_eval.InternalFunctions.Index(DirectCast(left.GetValue(), ObjectTypes.Reference()), CInt(
                                                                                      CType(result, BigDecimal))), True)
                    End If
                ElseIf ObjectTypes.Set.IsType(left) Then ' dictionaries
                    Dim result As Object = _eval.EvalExprRaw(inner, True)
                    left = ObjectTypes.DetectType(_eval.InternalFunctions.Index(DirectCast(left.GetValue(), SortedDictionary(Of ObjectTypes.Reference, ObjectTypes.Reference)),
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
#End Region

#Region "Unary Operator Definitions"
        ' Define UNARY OPERATORS here:
        ' format is always: Private Function UnaryOperator[name](value As EvalTypes.IEvalObject) As EvalTypes.IEvalObject

        Private Function UnaryOperatorFactorial(value As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            Dim v As Object = value.GetValue()
            If TypeOf value Is ObjectTypes.Number Then
                Return New ObjectTypes.Number(_eval.InternalFunctions.Factorial(CDbl(v)))
            Else
                Throw New SyntaxException("Only Number types may be used with the ! (factorial) operator")
            End If
        End Function

        Private Function UnaryOperatorPercent(value As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            Dim v As Object = value.GetValue()
            If TypeOf value Is ObjectTypes.Number Then
                Return New ObjectTypes.Number(CDbl(v) / 100)
            Else
                Throw New SyntaxException("Only Number types may be used with the ! (factorial) operator")
            End If
        End Function

        Private Function UnaryOperatorNot(value As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            Dim v As Object = value.GetValue()
            If TypeOf value Is ObjectTypes.Boolean Then
                Try
                    Return New ObjectTypes.Boolean(Not CBool(_eval.EvalExprRaw(v.ToString(), True)))
                Catch
                    Throw New SyntaxException("Operator not: Expression must produce a boolean value")
                End Try
            ElseIf TypeOf value Is ObjectTypes.Number AndAlso Not Double.IsNaN(CDbl(v)) Then
                Return New ObjectTypes.Boolean(Math.Round(CLng(v), 15) = 0)
            Else
                Throw New SyntaxException("Invalid type for the logical not operator (only booleans and numbers allowed)")
            End If
        End Function

        Private Function UnaryOperatorBitwiseNot(value As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            Dim v As Object = value.GetValue()
            If TypeOf value Is ObjectTypes.Boolean Then
                Try
                    Return New ObjectTypes.Boolean(Not CBool(_eval.EvalExprRaw(v.ToString(), True)))
                Catch
                    Throw New SyntaxException("Invalid type for the bitwise not operator")
                End Try
            ElseIf TypeOf value Is ObjectTypes.Number AndAlso Not Double.IsNaN(CDbl(v)) Then
                Return New ObjectTypes.Number(Not CLng(v))
            Else
                Throw New SyntaxException("Invalid type for the bitwise not operator")
            End If
        End Function

        Private Function UnaryOperatorReference(value As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            Return New ObjectTypes.Reference(value)
        End Function
#End Region

#Region "Binary Operator Definitions"
        ' Define BINARY OPERATORS here:
        ' format is always: Private Function BinaryOperator[name](left As EvalTypes.IEvalObject, 
        '                                                   right As EvalTypes.IEvalObject) As EvalTypes.IEvalObject

        Private Function BinaryOperatorAdd(left As ObjectTypes.EvalObjectBase, right As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            If left Is Nothing Then left = New ObjectTypes.Number(0)
            If TypeOf left Is ObjectTypes.Number And TypeOf right Is ObjectTypes.Complex Then Return BinaryOperatorAdd(right, left)
            Dim lv As Object = left.GetValue()
            Dim rv As Object = right.GetValue()

            If TypeOf left Is ObjectTypes.Number And TypeOf right Is ObjectTypes.Number Then
                Return New ObjectTypes.Number(CType(left, ObjectTypes.Number).BigDecValue() +
                                              CType(right, ObjectTypes.Number).BigDecValue())

            ElseIf TypeOf left Is ObjectTypes.Complex And TypeOf right Is ObjectTypes.Complex Then
                Return New ObjectTypes.Complex(CType(lv, Numerics.Complex) + CType(lv, Numerics.Complex))
            ElseIf TypeOf left Is ObjectTypes.Complex And TypeOf right Is ObjectTypes.Number Then
                Return New ObjectTypes.Complex(CType(lv, Numerics.Complex) + CDbl(rv))

            ElseIf TypeOf left Is ObjectTypes.DateTime And TypeOf right Is ObjectTypes.DateTime Then
                If TypeOf lv Is DateTime Then lv = New TimeSpan(CDate(lv).Ticks)
                If TypeOf rv Is DateTime Then rv = New TimeSpan(CDate(rv).Ticks)
                Return New ObjectTypes.DateTime(CType(lv, TimeSpan) + (CType(rv, TimeSpan)))

            ElseIf TypeOf left Is ObjectTypes.Text OrElse TypeOf right Is ObjectTypes.Text Then
                ' do not append "NaN"
                If lv.ToString() = "NaN" Then lv = ""
                If rv.ToString() = "NaN" Then rv = ""
                Return New ObjectTypes.Text(lv.ToString() & rv.ToString())

            ElseIf TypeOf left Is ObjectTypes.Matrix And TypeOf right Is ObjectTypes.Matrix Then
                Dim lstl As List(Of ObjectTypes.Reference) = DirectCast(lv, List(Of ObjectTypes.Reference))
                Dim lstr As List(Of ObjectTypes.Reference) = DirectCast(rv, List(Of ObjectTypes.Reference))
                For i As Integer = 0 To Math.Min(lstl.Count, lstr.Count) - 1
                    Dim sum As ObjectTypes.EvalObjectBase = BinaryOperatorAdd(lstl(i).ResolveObj(), lstr(i).ResolveObj())
                    If TypeOf sum Is ObjectTypes.Reference Then
                        lstl(i) = DirectCast(sum, ObjectTypes.Reference)
                    Else
                        lstl(i) = New ObjectTypes.Reference(sum)
                    End If
                Next
                Dim mat As New ObjectTypes.Matrix(lstl)
                Return mat

            ElseIf TypeOf left Is ObjectTypes.Matrix And TypeOf right Is ObjectTypes.Set Then
                Dim lst As New List(Of ObjectTypes.Reference)(DirectCast(lv, List(Of ObjectTypes.Reference)))
                lst.AddRange(_eval.InternalFunctions.ToMatrix(DirectCast(
                              rv, SortedDictionary(Of ObjectTypes.Reference, ObjectTypes.Reference))))
                Return New ObjectTypes.Matrix(lst)

            ElseIf TypeOf left Is ObjectTypes.Matrix Then
                Dim lst As New List(Of ObjectTypes.Reference)(CType(lv, List(Of ObjectTypes.Reference)))
                lst.Add(New ObjectTypes.Reference(right))
                Return New ObjectTypes.Matrix(lst)

            ElseIf TypeOf left Is ObjectTypes.Set Then
                Dim dict As New SortedDictionary(Of ObjectTypes.Reference, ObjectTypes.Reference)(
                    CType(lv, SortedDictionary(Of ObjectTypes.Reference, ObjectTypes.Reference)), New GenericComparer())
                If TypeOf right Is ObjectTypes.Set Then
                    ' union
                    dict = _eval.InternalFunctions.Union(dict, CType(rv, SortedDictionary(Of ObjectTypes.Reference, ObjectTypes.Reference)))
                ElseIf TypeOf right Is ObjectTypes.Matrix Then
                    dict = _eval.InternalFunctions.Union(dict, _eval.InternalFunctions.ToSet(CType(rv, List(Of ObjectTypes.Reference))))
                Else
                    dict(New ObjectTypes.Reference(rv)) = Nothing
                End If
                Return New ObjectTypes.Set(dict)

            ElseIf TypeOf right Is ObjectTypes.Matrix Then
                Return BinaryOperatorAdd(right, left)

            ElseIf TypeOf left Is ObjectTypes.Boolean And TypeOf right Is ObjectTypes.Boolean Then
                Return BinaryOperatorOr(left, right) ' + = OR

            Else
                Throw New SyntaxException("Invalid Addition")
            End If
        End Function

        Private Function BinaryOperatorSubtract(left As ObjectTypes.EvalObjectBase, right As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            If left Is Nothing Then left = New ObjectTypes.Number(0)

            Dim lv As Object = left.GetValue()
            Dim rv As Object = right.GetValue()
            If TypeOf left Is ObjectTypes.Number And TypeOf right Is ObjectTypes.Number Then
                Return New ObjectTypes.Number(CType(left, ObjectTypes.Number).BigDecValue() -
                                              CType(right, ObjectTypes.Number).BigDecValue())

            ElseIf TypeOf left Is ObjectTypes.Complex And TypeOf right Is ObjectTypes.Complex Then
                Return New ObjectTypes.Complex(CType(lv, Numerics.Complex) - CType(lv, Numerics.Complex))
            ElseIf TypeOf left Is ObjectTypes.Complex And TypeOf right Is ObjectTypes.Number Then
                Return New ObjectTypes.Complex(CType(lv, Numerics.Complex) - CDbl(rv))
            ElseIf TypeOf left Is ObjectTypes.Number And TypeOf right Is ObjectTypes.Complex Then
                Return New ObjectTypes.Complex(CDbl(lv) - CType(rv, Numerics.Complex))

            ElseIf TypeOf left Is ObjectTypes.Set Then
                Dim dict As SortedDictionary(Of ObjectTypes.Reference, ObjectTypes.Reference) =
                    CType(lv, SortedDictionary(Of ObjectTypes.Reference, ObjectTypes.Reference))
                If TypeOf right Is ObjectTypes.Set Then
                    ' difference
                    dict = _eval.InternalFunctions.Difference(dict, CType(rv, SortedDictionary(Of ObjectTypes.Reference, ObjectTypes.Reference)))
                ElseIf TypeOf right Is ObjectTypes.Matrix Then
                    dict = _eval.InternalFunctions.Difference(dict, _eval.InternalFunctions.ToSet(CType(rv, List(Of ObjectTypes.Reference))))
                Else
                    dict(New ObjectTypes.Reference(rv)) = Nothing
                End If
                Return New ObjectTypes.Set(dict)

            ElseIf TypeOf left Is ObjectTypes.Matrix And TypeOf right Is ObjectTypes.Matrix Then
                Dim lstl As List(Of ObjectTypes.Reference) = DirectCast(lv, List(Of ObjectTypes.Reference))
                Dim lstr As List(Of ObjectTypes.Reference) = DirectCast(rv, List(Of ObjectTypes.Reference))
                For i As Integer = 0 To Math.Min(lstl.Count, lstr.Count) - 1
                    Dim sum As ObjectTypes.EvalObjectBase = BinaryOperatorSubtract(lstl(i).ResolveObj(), lstr(i).ResolveObj())
                    If TypeOf sum Is ObjectTypes.Reference Then
                        lstl(i) = DirectCast(sum, ObjectTypes.Reference)
                    Else
                        lstl(i) = New ObjectTypes.Reference(sum)
                    End If
                Next
                Dim mat As New ObjectTypes.Matrix(lstl)
                Return mat

            ElseIf TypeOf left Is ObjectTypes.DateTime And TypeOf right Is ObjectTypes.DateTime Then
                If TypeOf lv Is DateTime Then lv = New TimeSpan(CDate(lv).Ticks)
                If TypeOf rv Is DateTime Then rv = New TimeSpan(CDate(rv).Ticks)
                Return New ObjectTypes.DateTime(CType(lv, TimeSpan) - CType(rv, TimeSpan))
            Else
                Throw New SyntaxException("Invalid Subtraction")
            End If
        End Function

        Private Function BinaryOperatorMultiply(left As ObjectTypes.EvalObjectBase, right As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            If left Is Nothing Then Throw New SyntaxException("Invalid Multiplication")
            If (TypeOf left Is ObjectTypes.Number And TypeOf right Is ObjectTypes.DateTime) OrElse
               (TypeOf left Is ObjectTypes.Number And TypeOf right Is ObjectTypes.Matrix) OrElse
               (TypeOf left Is ObjectTypes.Number And TypeOf right Is ObjectTypes.Complex) OrElse
               (TypeOf left Is ObjectTypes.Number And TypeOf right Is ObjectTypes.Text) Then
                Return BinaryOperatorMultiply(right, left)
            End If
            Dim lv As Object = left.GetValue()
            Dim rv As Object = right.GetValue()
            If TypeOf left Is ObjectTypes.Number And TypeOf right Is ObjectTypes.Number Then
                Return New ObjectTypes.Number(CType(left, ObjectTypes.Number).BigDecValue() *
                                              CType(right, ObjectTypes.Number).BigDecValue())

            ElseIf TypeOf left Is ObjectTypes.Complex And TypeOf right Is ObjectTypes.Complex Then
                Return New ObjectTypes.Complex(CType(lv, Numerics.Complex) * CType(lv, Numerics.Complex))
            ElseIf TypeOf left Is ObjectTypes.Complex And TypeOf right Is ObjectTypes.Number Then
                Return New ObjectTypes.Complex(CType(lv, Numerics.Complex) * CDbl(rv))

            ElseIf TypeOf left Is ObjectTypes.DateTime And TypeOf right Is ObjectTypes.Number Then
                If TypeOf lv Is DateTime Then lv = New TimeSpan(CDate(lv).Ticks)
                Return New ObjectTypes.DateTime(New TimeSpan(CLng(CDbl(rv) * CType(lv, TimeSpan).Ticks)))

            ElseIf TypeOf left Is ObjectTypes.Text And TypeOf right Is ObjectTypes.Number Then
                Dim origstr As String = CStr(lv)
                Dim strcat As String = origstr
                While strcat.Length * 2 < origstr.Length * CDbl(rv)
                    strcat &= strcat
                End While
                While strcat.Length < origstr.Length * CDbl(rv)
                    strcat &= origstr
                End While
                Return New ObjectTypes.Text(strcat)

            ElseIf TypeOf left Is ObjectTypes.Matrix And TypeOf right Is ObjectTypes.Number Then
                ' scalar multiply, use ** to duplicate

                Return ObjectTypes.DetectType(_eval.InternalFunctions.Scale(CType(lv, List(Of ObjectTypes.Reference)), rv))

            ElseIf TypeOf left Is ObjectTypes.Matrix And TypeOf right Is ObjectTypes.Matrix Then
                ' matrix multiplication (for appropriate matrices) or dot product (for vectors)
                Return ObjectTypes.DetectType(_eval.InternalFunctions.Multiply(CType(left.GetValue(), List(Of ObjectTypes.Reference)),
                                                CType(right.GetValue(), List(Of ObjectTypes.Reference))))

            ElseIf TypeOf left Is ObjectTypes.Set And TypeOf right Is ObjectTypes.Matrix Then
                Return New ObjectTypes.Set(_eval.InternalFunctions.Intersect(CType(
                       lv, SortedDictionary(Of ObjectTypes.Reference, ObjectTypes.Reference)), _eval.InternalFunctions.ToSet(CType(rv, List(Of ObjectTypes.Reference)))))

            ElseIf TypeOf left Is ObjectTypes.Matrix And TypeOf right Is ObjectTypes.Set Then
                Return BinaryOperatorMultiply(right, left)

            ElseIf TypeOf left Is ObjectTypes.Set And TypeOf right Is ObjectTypes.Set Then
                Return New ObjectTypes.Set(_eval.InternalFunctions.Intersect(CType(lv, SortedDictionary(Of ObjectTypes.Reference, ObjectTypes.Reference)),
                       CType(rv, SortedDictionary(Of ObjectTypes.Reference, ObjectTypes.Reference))))

            ElseIf TypeOf left Is ObjectTypes.Boolean And TypeOf right Is ObjectTypes.Boolean Then
                Return BinaryOperatorAnd(left, right) ' * = AND

            Else
                Throw New SyntaxException("Invalid Multiplication")
            End If
        End Function

        ' used for duplication
        Private Function BinaryOperatorDuplicateCross(left As ObjectTypes.EvalObjectBase, right As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            If (TypeOf left Is ObjectTypes.Number And TypeOf right Is ObjectTypes.Matrix) OrElse
             (TypeOf left Is ObjectTypes.Complex And TypeOf right Is ObjectTypes.Matrix) Then
                Return BinaryOperatorDuplicateCross(right, left)
            End If

            If TypeOf left Is ObjectTypes.Matrix And (TypeOf right Is ObjectTypes.Number OrElse TypeOf right Is ObjectTypes.Complex) Then
                ' duplicate
                Dim lst As New List(Of ObjectTypes.Reference)(CType(left.GetValue(), List(Of ObjectTypes.Reference)))
                Dim lv As Object = left.DeepCopy().GetValue()
                Dim rv As Object = right.GetValue()
                Dim origlst As New List(Of ObjectTypes.Reference)(lst)
                While lst.Count * 2 < origlst.Count * CDbl(rv)
                    lst.AddRange(DirectCast(New ObjectTypes.Matrix(lst).DeepCopy().GetValue(), List(Of ObjectTypes.Reference)))
                End While
                While lst.Count < origlst.Count * CDbl(rv)
                    lst.AddRange(DirectCast(New ObjectTypes.Matrix(lst).DeepCopy().GetValue(), List(Of ObjectTypes.Reference)))
                End While
                Return New ObjectTypes.Matrix(lst)

            ElseIf TypeOf left Is ObjectTypes.Matrix And TypeOf right Is ObjectTypes.Matrix Then
                ' cross product
                Return New ObjectTypes.Matrix(CType(_eval.InternalFunctions.Cross(CType(left.GetValue(), List(Of ObjectTypes.Reference)),
                                                    CType(right.GetValue(), List(Of ObjectTypes.Reference))), List(Of ObjectTypes.Reference)))
            Else
                Return BinaryOperatorMultiply(left, right)
            End If
        End Function

        Private Function BinaryOperatorDivide(left As ObjectTypes.EvalObjectBase, right As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            If left Is Nothing Then Throw New SyntaxException("Invalid Division")
            Dim lv As Object = left.GetValue()
            Dim rv As Object = right.GetValue()
            If TypeOf left Is ObjectTypes.Number And TypeOf right Is ObjectTypes.Number Then
                Return New ObjectTypes.Number(CType(left, ObjectTypes.Number).BigDecValue() / CType(right, ObjectTypes.Number).BigDecValue())

            ElseIf TypeOf left Is ObjectTypes.Complex And TypeOf right Is ObjectTypes.Complex Then
                Return New ObjectTypes.Complex(CType(lv, Numerics.Complex) / CType(lv, Numerics.Complex))
            ElseIf TypeOf left Is ObjectTypes.Complex And TypeOf right Is ObjectTypes.Number Then
                Return New ObjectTypes.Complex(CType(lv, Numerics.Complex) / CDbl(rv))
            ElseIf TypeOf left Is ObjectTypes.Number And TypeOf right Is ObjectTypes.Complex Then
                Return New ObjectTypes.Complex(CDbl(lv) / CType(rv, Numerics.Complex))

            ElseIf TypeOf left Is ObjectTypes.Matrix And TypeOf right Is ObjectTypes.Number Then
                ' scalar division

                Return ObjectTypes.DetectType(_eval.InternalFunctions.Scale(CType(lv, List(Of ObjectTypes.Reference)),
                                                        BinaryOperatorDivide(New ObjectTypes.Number(1), right).GetValue()))

            ElseIf TypeOf left Is ObjectTypes.DateTime And TypeOf right Is ObjectTypes.Number Then
                If TypeOf lv Is DateTime Then lv = New TimeSpan(CDate(lv).Ticks)
                Return New ObjectTypes.DateTime(New TimeSpan(CLng(CType(lv, TimeSpan).Ticks / CDbl(rv))))

            Else
                Throw New SyntaxException("Invalid Division")
            End If
        End Function

        Private Function BinaryOperatorDivideFloor(left As ObjectTypes.EvalObjectBase, right As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            If left Is Nothing Then Throw New SyntaxException("Invalid Floor Div.")
            Dim lv As Object = left.GetValue()
            Dim rv As Object = right.GetValue()
            If TypeOf left Is ObjectTypes.Number And TypeOf right Is ObjectTypes.Number Then
                Return New ObjectTypes.Number(Math.Floor(CDbl(CType(left, ObjectTypes.Number).BigDecValue() /
                                                         CType(right, ObjectTypes.Number).BigDecValue())))

            ElseIf TypeOf left Is ObjectTypes.Complex Or TypeOf right Is ObjectTypes.Complex Then
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
            If TypeOf left Is ObjectTypes.Number And TypeOf right Is ObjectTypes.Number Then
                Return New ObjectTypes.Number(_eval.InternalFunctions.Modulo(CDbl(lv), CDbl(rv)))

            ElseIf TypeOf left Is ObjectTypes.DateTime And TypeOf right Is ObjectTypes.Number Then
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
            If TypeOf left Is ObjectTypes.Boolean And TypeOf right Is ObjectTypes.Boolean Then
                Try
                    Return New ObjectTypes.Boolean(CBool(_eval.InternalFunctions.Eval(lv.ToString())) OrElse
                                             CBool(_eval.InternalFunctions.Eval(rv.ToString())))
                Catch
                    Throw New SyntaxException("Operator or: Expression must produce a boolean value")
                End Try

            ElseIf TypeOf left Is ObjectTypes.Number AndAlso TypeOf right Is ObjectTypes.Number AndAlso
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
            If TypeOf left Is ObjectTypes.Boolean And TypeOf right Is ObjectTypes.Boolean Then
                Try
                    Return New ObjectTypes.Boolean(CBool(_eval.InternalFunctions.Eval(lv.ToString())) OrElse
                                             CBool(_eval.InternalFunctions.Eval(rv.ToString())))
                Catch
                    Throw New SyntaxException("Invalid bitwise or operation")
                End Try

            ElseIf TypeOf left Is ObjectTypes.Number AndAlso TypeOf right Is ObjectTypes.Number AndAlso
                Not Double.IsNaN(CDbl(lv)) AndAlso Not Double.IsNaN(CDbl(rv)) Then
                Return New ObjectTypes.Number(CLng(lv) Or CLng(rv))
            Else
                Throw New SyntaxException("Invalid bitwise or operation")
            End If
        End Function

        Private Function BinaryOperatorAnd(left As ObjectTypes.EvalObjectBase, right As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            Dim lv As Object = left.GetValue()
            Dim rv As Object = right.GetValue()
            If TypeOf left Is ObjectTypes.Boolean AndAlso TypeOf right Is ObjectTypes.Boolean Then
                Try
                    Return New ObjectTypes.Boolean(CBool(_eval.InternalFunctions.Eval(lv.ToString())) AndAlso
                                             CBool(_eval.InternalFunctions.Eval(rv.ToString())))
                Catch
                    Throw New SyntaxException("Operator and: Expression must produce a boolean value")
                End Try
            ElseIf TypeOf left Is ObjectTypes.Number AndAlso TypeOf right Is ObjectTypes.Number AndAlso
                Not Double.IsNaN(CDbl(lv)) AndAlso Not Double.IsNaN(CDbl(rv)) Then
                Return New ObjectTypes.Boolean(Math.Round(CDbl(lv), 15) <> 0 And Math.Round(CDbl(rv), 15) <> 0)
            Else
                Throw New SyntaxException("Invalid logical and operation")
            End If
        End Function

        Private Function BinaryOperatorBitwiseAnd(left As ObjectTypes.EvalObjectBase, right As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            Dim lv As Object = left.GetValue()
            Dim rv As Object = right.GetValue()
            If TypeOf left Is ObjectTypes.Boolean AndAlso TypeOf right Is ObjectTypes.Boolean Then
                Try
                    Return New ObjectTypes.Boolean(CBool(_eval.InternalFunctions.Eval(lv.ToString())) AndAlso
                                             CBool(_eval.InternalFunctions.Eval(rv.ToString())))
                Catch
                    Throw New SyntaxException("Invalid bitwise and operation")
                End Try
            ElseIf TypeOf left Is ObjectTypes.Number AndAlso TypeOf right Is ObjectTypes.Number AndAlso
                Not Double.IsNaN(CDbl(lv)) AndAlso Not Double.IsNaN(CDbl(rv)) Then
                Return New ObjectTypes.Number(CLng(lv) And CLng(rv))
            Else
                Throw New SyntaxException("Invalid bitwise and operation")
            End If
        End Function

        Private Function BinaryOperatorXor(left As ObjectTypes.EvalObjectBase, right As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            Dim lv As Object = left.GetValue()
            Dim rv As Object = right.GetValue()
            If TypeOf left Is ObjectTypes.Boolean And TypeOf right Is ObjectTypes.Boolean Then
                Try
                    Return New ObjectTypes.Boolean(CBool(_eval.InternalFunctions.Eval(lv.ToString())) Xor
                                             CBool(_eval.InternalFunctions.Eval(rv.ToString())))
                Catch
                    Throw New SyntaxException("Operator xor: Expression must produce a boolean value")
                End Try
            ElseIf TypeOf left Is ObjectTypes.Number AndAlso TypeOf right Is ObjectTypes.Number AndAlso
                Not Double.IsNaN(CDbl(lv)) AndAlso Not Double.IsNaN(CDbl(rv)) Then
                Return New ObjectTypes.Boolean(Math.Round(CDbl(lv), 15) <> 0 Xor Math.Round(CDbl(rv), 15) <> 0)
            Else
                Throw New SyntaxException("Invalid logical xor operation")
            End If
        End Function

        Private Function BinaryOperatorBitwiseXor(left As ObjectTypes.EvalObjectBase, right As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            Dim lv As Object = left.GetValue()
            Dim rv As Object = right.GetValue()
            If TypeOf left Is ObjectTypes.Boolean And TypeOf right Is ObjectTypes.Boolean Then
                Try
                    Return New ObjectTypes.Boolean(CBool(_eval.InternalFunctions.Eval(lv.ToString())) Xor
                                             CBool(_eval.InternalFunctions.Eval(rv.ToString())))
                Catch
                    Throw New SyntaxException("Invalid bitwise xor operation")
                End Try
            ElseIf TypeOf left Is ObjectTypes.Number AndAlso TypeOf right Is ObjectTypes.Number AndAlso
                Not Double.IsNaN(CDbl(lv)) AndAlso Not Double.IsNaN(CDbl(rv)) Then
                Return New ObjectTypes.Number(CLng(lv) Xor CLng(rv))
            ElseIf TypeOf left Is ObjectTypes.Set Then ' this operator doubles as the symmetric difference operator for sets
                Dim dict As SortedDictionary(Of ObjectTypes.Reference, ObjectTypes.Reference) =
                    CType(lv, SortedDictionary(Of ObjectTypes.Reference, ObjectTypes.Reference))
                If TypeOf right Is ObjectTypes.Set Then
                    ' symmetric difference
                    dict = _eval.InternalFunctions.SymmetricDifference(dict, CType(rv, SortedDictionary(Of ObjectTypes.Reference, ObjectTypes.Reference)))
                ElseIf TypeOf right Is ObjectTypes.Matrix Then
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
            If TypeOf left Is ObjectTypes.Number AndAlso TypeOf right Is ObjectTypes.Number AndAlso
                Not Double.IsNaN(CDbl(lv)) AndAlso Not Double.IsNaN(CDbl(rv)) Then
                Return New ObjectTypes.Number(CLng(lv) << CInt(rv))
            Else
                Throw New SyntaxException("Invalid << (bitwise shl) operation")
            End If
        End Function

        Private Function BinaryOperatorShr(left As ObjectTypes.EvalObjectBase, right As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            Dim lv As Object = left.GetValue()
            Dim rv As Object = right.GetValue()
            If TypeOf left Is ObjectTypes.Number AndAlso TypeOf right Is ObjectTypes.Number AndAlso
                Not Double.IsNaN(CDbl(lv)) AndAlso Not Double.IsNaN(CDbl(rv)) Then
                Return New ObjectTypes.Number(CLng(lv) >> CInt(rv))
            Else
                Throw New SyntaxException("Invalid >> (bitwise shl) operation")
            End If
        End Function

        Private Function BinaryOperatorChoose(left As ObjectTypes.EvalObjectBase, right As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            Dim lv As Object = left.GetValue()
            Dim rv As Object = right.GetValue()
            If TypeOf left Is ObjectTypes.Number And TypeOf right Is ObjectTypes.Number Then
                Return New ObjectTypes.Number(_eval.InternalFunctions.Combinations(CLng(lv), CLng(rv)))
            Else
                Throw New SyntaxException("Invalid types for the choose (combinations) operator")
            End If
        End Function

        Private Function BinaryOperatorAssign(left As ObjectTypes.EvalObjectBase, right As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            Try
                If TypeOf left Is ObjectTypes.Reference OrElse TypeOf left Is ObjectTypes.Tuple Then
                    ' if it is a reference or tuple then we copy the right value and assign the left to it
                    If TypeOf right Is ObjectTypes.Reference AndAlso
                        TypeOf DirectCast(right, ObjectTypes.Reference).GetRefObject() Is ObjectTypes.Reference Then
                        ' if we are assigning a reference
                        left.SetValue(DirectCast(right, ObjectTypes.Reference).ResolveObj())
                    Else
                        ' if we are assigning a plain value
                        left.SetValue(right.DeepCopy().GetValue())
                    End If
                    Return left
                Else
                    ' otherwise we don't actually assign anything
                    Return right
                End If
            Catch 'ex As Exception
                Throw New EvaluatorException("Assignment operation failed")
            End Try
        End Function

        Private Function BinaryOperatorOpAssign(left As ObjectTypes.EvalObjectBase, right As ObjectTypes.EvalObjectBase,
                                                op As Func(Of ObjectTypes.EvalObjectBase, ObjectTypes.EvalObjectBase, ObjectTypes.EvalObjectBase)) As ObjectTypes.EvalObjectBase
            Try
                If TypeOf right Is ObjectTypes.Reference Then right = DirectCast(right, ObjectTypes.Reference).ResolveObj()
                If TypeOf left Is ObjectTypes.Reference OrElse TypeOf left Is ObjectTypes.Tuple Then
                    left.SetValue(op(CType(left, ObjectTypes.Reference).ResolveObj().DeepCopy(), right.DeepCopy()))
                Else
                    Return op(left, right) ' not a reference? just use normal operator
                End If
                Return left
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
                If TypeOf right Is ObjectTypes.Reference Then right = DirectCast(right, ObjectTypes.Reference).GetRefObject()
                If TypeOf left Is ObjectTypes.Reference AndAlso (right Is Nothing OrElse right.ToString() = "NaN") Then
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
                    If TypeOf left Is ObjectTypes.Reference Then left = DirectCast(left, ObjectTypes.Reference).ResolveObj()
                    If TypeOf right Is ObjectTypes.Reference Then right = DirectCast(right, ObjectTypes.Reference).ResolveObj()
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
                If TypeOf right Is ObjectTypes.Reference Then right = DirectCast(right, ObjectTypes.Reference).GetRefObject()
                If TypeOf left Is ObjectTypes.Reference AndAlso (right Is Nothing OrElse right.ToString() = "NaN") Then
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
            If TypeOf left Is ObjectTypes.Reference OrElse TypeOf left Is ObjectTypes.Tuple AndAlso Not _conditional Then
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

        Private Function BinaryOperatorComma(left As ObjectTypes.EvalObjectBase, right As ObjectTypes.EvalObjectBase) As ObjectTypes.EvalObjectBase
            If left Is Nothing Then Return right
            If right Is Nothing Then Return left
            Dim lv As Object = left.GetValue()
            Dim rv As Object = right.GetValue()
            Try
                If TypeOf left Is ObjectTypes.Tuple Then
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
                If TypeOf left Is ObjectTypes.Tuple Then
                    Dim lst As New List(Of ObjectTypes.Reference)(DirectCast(lv, ObjectTypes.Reference()))
                    lst(lst.Count - 1) = New ObjectTypes.Reference(BinaryOperatorComma(lst(lst.Count - 1).GetRefObject(), right))
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
            If TypeOf left Is ObjectTypes.Number And TypeOf right Is ObjectTypes.Number Then
                Return New ObjectTypes.Number(CDbl(lv) * BigDecimal.Pow(10, CLng(rv)))
            Else
                Throw New SyntaxException("Invalid types for the E (exp10) operator")
            End If
        End Function
#End Region
    End Class
End Namespace