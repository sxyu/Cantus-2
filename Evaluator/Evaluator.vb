Imports System.Collections.ObjectModel
Imports System.IO
Imports System.Reflection
Imports System.Text
Imports System.Threading
Imports Cantus.Calculator.Evaluator.CommonTypes
Imports Cantus.Calculator.Evaluator.Exceptions
Imports Cantus.Calculator.Evaluator.ObjectTypes
Imports Cantus.Calculator.Evaluator.StatementRegistar

Namespace Calculator.Evaluator
    Public Module Globals
        Friend ReadOnly Property Evaluator As New Evaluator()
    End Module

    Public NotInheritable Class Evaluator
        Implements IDisposable
#Region "Enums"

        ''' <summary>
        ''' Represents a represenatation of an angle (degree/radian/gradian)
        ''' </summary>
        Public Enum eAngleRepresentation
            Degree = 0
            Radian
            Gradian
        End Enum

        ''' <summary>
        ''' Represents an output format
        ''' </summary>
        Public Enum eOutputFormat
            ''' <summary>
            ''' Directly outputs as a decimal number (switches to scientific notation for very large/very small numbers)
            ''' </summary>
            Raw = 0
            ''' <summary>
            ''' Attempts to format output as a fraction, root, or multiple of pi
            ''' </summary>
            Math
            ''' <summary>
            ''' Formats output in scientific notation
            ''' </summary>
            Scientific
        End Enum
#End Region

#Region "Structs"
        ''' <summary>
        ''' Represents an user-defined function
        ''' </summary>
        Public NotInheritable Class UserFunction
            ''' <summary>
            ''' The name of the function
            ''' </summary>
            ''' <returns></returns>
            Public Property Name As String

            ''' <summary>
            ''' The body of the function
            ''' </summary>
            ''' <returns></returns>
            Public Property Body As String

            ''' <summary>
            ''' Hashset of modifiers, such as private, applied to the function (NI)
            ''' </summary>
            Public Property Modifiers As HashSet(Of String)

            ''' <summary>
            '''  Return type of the function (NI)
            ''' </summary>
            Public Property ReturnType As String

            ''' <summary>
            ''' Names of the function arguments
            ''' </summary>
            Public Property Args As List(Of String)

            ''' <summary>
            ''' Types of the function arguments (NI)
            ''' </summary>
            Public Property ArgTypes As List(Of String)

            ''' <summary>
            ''' Gets the scope in which this function was declared
            ''' </summary>
            Public Property DeclaringScope As String

            ''' <summary>
            ''' Gets the full name of this function, including the scope
            ''' </summary>
            ''' <returns></returns>
            Public ReadOnly Property FullName As String
                Get
                    Return DeclaringScope & SCOPE_SEP & Name
                End Get
            End Property

            ''' <summary>
            ''' Create a new user function
            ''' </summary>
            Public Sub New(name As String, body As String,
                           args As List(Of String), declaredScope As String,
                            Optional modifiers As IEnumerable(Of String) = Nothing,
                           Optional argTypes As List(Of String) = Nothing,
                           Optional returnType As String = "")
                If modifiers Is Nothing Then
                    Me.Modifiers = New HashSet(Of String)()
                Else
                    Me.Modifiers = New HashSet(Of String)(modifiers)
                End If
                Me.Name = name
                Me.Body = body
                Me.Args = args
                Me.DeclaringScope = declaredScope
                Me.ArgTypes = argTypes
                If Me.ArgTypes Is Nothing Then
                    Me.ArgTypes = New List(Of String)
                    For Each a As String In Me.Args
                        Me.ArgTypes.Add("")
                    Next
                End If
                Me.ReturnType = returnType
            End Sub

            ''' <summary>
            ''' Get the full definition of this function as a string
            ''' </summary>
            Public Shadows Function ToString(Optional ByVal ignoreScope As String = ROOT_NAMESPACE) As String
                Dim result As New StringBuilder()
                For Each m As String In Modifiers
                    result.Append(m).Append(" ")
                Next
                result.Append("function ")

                Dim scope As String = Evaluator.RemoveRedundantScope(DeclaringScope, ignoreScope)
                result.Append(scope.Trim())
                If Not String.IsNullOrWhiteSpace(scope) Then result.Append(SCOPE_SEP)

                result.Append(Name).Append("(").
                Append(String.Join(", ", Args)).AppendLine(")")
                result.Append(Body)
                Return result.ToString()
            End Function
        End Class

        ''' <summary>
        ''' Represents an evaluator variable
        ''' </summary>
        Public NotInheritable Class Variable
            ''' <summary>
            ''' The name of the variable
            ''' </summary>
            Public Property Name As String

            ''' <summary>
            ''' Gets or sets a reference to the value of the variable
            ''' </summary>
            ''' <returns></returns>
            Public Property Reference As ObjectTypes.Reference

            ''' <summary>
            ''' Gets or sets a hashset of modifiers for the variable
            ''' </summary>
            ''' <returns></returns>
            Public Property Modifiers As HashSet(Of String)

            ''' <summary>
            ''' Gets or sets the value of the variable without changing refernce
            ''' </summary>
            ''' <returns></returns>
            Public Property Value As Object
                Get
                    Return Reference.Resolve()
                End Get
                Set(value As Object)
                    Reference.ResolveRef().SetValue(value)
                End Set
            End Property

            ''' <summary>
            ''' Gets the scope in which this variable was last assigned to
            ''' </summary>
            ''' <returns></returns>
            Public Property DeclaringScope As String

            ''' <summary>
            ''' Gets the full name of this variable, including the scope
            ''' </summary>
            ''' <returns></returns>
            Public ReadOnly Property FullName As String
                Get
                    Return DeclaringScope & SCOPE_SEP & Name
                End Get
            End Property

            ''' <summary>
            ''' Create a empty variable (internal)
            ''' </summary>
            Private Sub New(name As String, declaringScope As String, Optional modifiers As IEnumerable(Of String) = Nothing)
                Me.Name = name
                Me.DeclaringScope = declaringScope
                If Not modifiers Is Nothing Then
                    Me.Modifiers = New HashSet(Of String)(modifiers)
                Else
                    Me.Modifiers = New HashSet(Of String)()
                End If
            End Sub

            ''' <summary>
            ''' Create a new variable from a value
            ''' </summary>
            Public Sub New(name As String, value As Object, declaringScope As String,
                           Optional modifiers As IEnumerable(Of String) = Nothing)
                Me.New(name, declaringScope, modifiers)
                Me.Reference = New ObjectTypes.Reference(value)
            End Sub

            ''' <summary>
            ''' Create a new variable from an evaluator object
            ''' </summary>
            Public Sub New(name As String, value As ObjectTypes.EvalObjectBase,
                           declaringScope As String, Optional modifiers As IEnumerable(Of String) = Nothing)
                Me.New(name, declaringScope, modifiers)
                Me.Reference = New ObjectTypes.Reference(value)
            End Sub

            ''' <summary>
            ''' Create a new variable from an existing reference
            ''' </summary>
            Public Sub New(name As String, ref As ObjectTypes.Reference,
                           declaringScope As String, Optional modifiers As IEnumerable(Of String) = Nothing)
                Me.New(name, declaringScope, modifiers)
                Me.Reference = ref
            End Sub

            ''' <summary>
            ''' Convert the variable to a (human-readable) string
            ''' </summary>
            ''' <returns></returns>
            Public Overrides Function ToString() As String
                Return Me.FullName & " = " & Me.Reference.ToString
            End Function
        End Class

        ''' <summary>
        ''' Represents an user-defined class with OOP support
        ''' </summary>
        Public NotInheritable Class UserClass
            Implements IDisposable

            Private _disposed As Boolean

            ''' <summary>
            ''' The name of the class
            ''' </summary>
            Public Property Name As String

            ''' <summary>
            ''' Gets or sets a dictionary of fields in this class, in the format (name, reference)
            ''' </summary>
            ''' <returns></returns>
            Public Property Fields As Dictionary(Of String, Variable)

            ''' <summary>
            ''' Gets a dictionary of all fields in this class, including inherited ones.
            ''' </summary>
            ''' <returns></returns>
            Public ReadOnly Property AllFields As Dictionary(Of String, Variable)
                Get
                    Dim res As New Dictionary(Of String, Variable)
                    For i As Integer = Me.BaseClasses.Count - 1 To 0 Step -1
                        Dim b As UserClass = Me.Evaluator.UserClasses(Me.BaseClasses(i))
                        For Each f As String In b.AllFields.Keys
                            res(f) = b.AllFields(f)
                        Next
                    Next
                    For Each f As String In Me.Fields.Keys
                        res(f) = Me.Fields(f)
                    Next
                    Return res
                End Get
            End Property

            ''' <summary>
            ''' Gets the constructor function for this class
            ''' </summary>
            ''' <returns></returns>
            Public ReadOnly Property Constructor As ObjectTypes.Lambda
                Get
                    Return DirectCast(AllFields("init").Reference.ResolveObj(), ObjectTypes.Lambda)
                End Get
            End Property

            ''' <summary>
            ''' Gets or sets the body of the class declaration
            ''' </summary>
            ''' <returns></returns>
            Public Property Body As String

            ''' <summary>
            ''' Gets or sets a hashset of modifiers for the class
            ''' </summary>
            ''' <returns></returns>
            Public Property Modifiers As HashSet(Of String)

            ''' <summary>
            ''' Gets or sets a list of classes this class inherits from.
            ''' </summary>
            ''' <returns></returns>
            Public Property BaseClasses As IEnumerable(Of String)

            ''' <summary>
            ''' Get a list of all classes that this class inherits from, directly or indirectly
            ''' </summary>
            ''' <returns></returns>
            Public ReadOnly Property AllParentClasses As IEnumerable(Of String)
                Get
                    Dim lst As New List(Of String)()

                    For Each b As String In BaseClasses
                        lst.Add(b)
                        If Not Evaluator.UserClasses.ContainsKey(b) Then Continue For
                        Dim bc As UserClass = Evaluator.UserClasses(b)
                        lst.AddRange(bc.AllParentClasses)
                    Next

                    Return lst
                End Get
            End Property

            ''' <summary>
            ''' Gets the scope in which this variable was last assigned to
            ''' </summary>
            ''' <returns></returns>
            Public Property DeclaringScope As String

            ''' <summary>
            ''' List of instances of this class
            ''' </summary>
            ''' <returns></returns>
            Public ReadOnly Property Instances As List(Of ClassInstance)

            ''' <summary>
            ''' The inner scope used to register functions, etc. of this class
            ''' </summary>
            ''' <returns></returns>
            Public ReadOnly Property InnerScope As String

            ''' <summary>
            ''' Gets the full name of this variable, including the scope
            ''' </summary>
            ''' <returns></returns>
            Public ReadOnly Property FullName As String
                Get
                    If Me._disposed Then Return Nothing
                    Return DeclaringScope & SCOPE_SEP & Name
                End Get
            End Property

            ''' <summary>
            ''' Gets the shortest name of the class that can be directly used to access it in the evaluator
            ''' </summary>
            ''' <returns></returns>
            Public ReadOnly Property ShortestAccessibleName As String
                Get
                    If Me._disposed Then Return Nothing
                    Return Evaluator.RemoveRedundantScope(Me.FullName, Evaluator.Scope)
                End Get
            End Property

            ''' <summary>
            ''' The evaluator used with this user class
            ''' </summary>
            Public ReadOnly Property Evaluator As Evaluator

            ''' <summary>
            ''' Create a empty class with the specified name, definition, evaluator, scope, and modifiers
            ''' </summary>
            ''' <param name="declaringScope">Optional. If not specified, uses the scope of the specified evaluator.</param>
            Public Sub New(name As String, def As String, eval As Evaluator,
                           Optional modifiers As IEnumerable(Of String) = Nothing,
                           Optional inheritedClasses As IEnumerable(Of String) = Nothing,
                           Optional declaringScope As String = "")

                Me._disposed = False
                Me.Instances = New List(Of ClassInstance)

                If Not inheritedClasses Is Nothing Then
                    Me.BaseClasses = New List(Of String)(inheritedClasses)
                Else
                    Me.BaseClasses = New List(Of String)
                End If

                Me.Name = name
                If declaringScope = "" Then
                    Me.DeclaringScope = eval.Scope
                Else
                    Me.DeclaringScope = declaringScope
                End If

                Me.Evaluator = eval
                Me.Body = def

                Me.Fields = New Dictionary(Of String, Variable)()
                If Not modifiers Is Nothing Then
                    Me.Modifiers = New HashSet(Of String)(modifiers)
                Else
                    Me.Modifiers = New HashSet(Of String)()
                End If

                Dim tmpScope As String = "__class_" & name & "_" &
                                     Guid.NewGuid().ToString().Replace("-", "") & Now.Millisecond

                Dim tmpEval As Evaluator = eval.SubEvaluator(0, tmpScope)
                tmpEval.Variables.Clear()
                tmpEval.UserFunctions.Clear()

                tmpScope = tmpEval.Scope()
                Dim nsScope As String = eval.Scope & SCOPE_SEP & Me.Name
                Me.InnerScope = tmpScope

                tmpEval.Eval(def, True)

                ' add back newly declared variables
                For Each var As Variable In tmpEval.Variables.Values
                    If var.DeclaringScope <> tmpScope Then Continue For
                    If var.Modifiers.Contains("static") Then ' static variables : declare in namespace with class name
                        var.DeclaringScope = nsScope
                        var.Modifiers.Add("internal")
                        eval.Variables(var.FullName) = var
                        Me.Fields(var.Name) = New Variable(var.Name, var.Reference, tmpScope, var.Modifiers)
                    Else
                        var.Modifiers.Add("internal")
                        Me.Fields(var.Name) = New Variable(var.Name, var.Reference.GetDeepCopy(), tmpScope, var.Modifiers)
                    End If
                Next

                ' add back newly declared functions
                For Each fn As UserFunction In tmpEval.UserFunctions.Values
                    If fn.DeclaringScope <> tmpScope Then Continue For
                    If fn.Modifiers.Contains("static") Then ' static functions
                        fn.DeclaringScope = nsScope
                        fn.Modifiers.Add("internal")
                        eval.UserFunctions(fn.FullName) = fn
                        Me.Fields(fn.Name) = New Variable(fn.Name, New ObjectTypes.Lambda(fn),
                          tmpScope, fn.Modifiers)
                    Else
                        fn.Modifiers.Add("internal")
                        Me.Fields(fn.Name) = New Variable(fn.Name, New ObjectTypes.Lambda(fn, True),
                          tmpScope, fn.Modifiers)
                    End If
                Next

                ' add back newly declared classes
                For Each uc As UserClass In tmpEval.UserClasses.Values
                    If uc.DeclaringScope <> tmpScope Then Continue For
                    uc.DeclaringScope = nsScope
                    eval.UserClasses(uc.FullName) = uc
                Next

                ' add empty constructor, if none exists
                If Not Me.Fields.ContainsKey("init") Then
                    Dim fn As New UserFunction("init", "", New List(Of String), tmpScope)
                    fn.Modifiers.Add("internal")
                    Me.Fields(fn.Name) = New Variable(fn.Name, New ObjectTypes.Lambda(fn, True), Me.FullName, fn.Modifiers)
                End If

                ' add 'type' function
                Dim typeFn As New UserFunction("type", String.Format("return {0}{1}type(this)", ROOT_NAMESPACE, SCOPE_SEP),
                                               New List(Of String), tmpScope)
                typeFn.Modifiers.Add("internal")
                Me.Fields(typeFn.Name) = New Variable(typeFn.Name,
                                      New ObjectTypes.Lambda(typeFn, True), Me.FullName, typeFn.Modifiers)
            End Sub

            Public Shared Operator =(a As UserClass, b As UserClass) As Boolean
                Return a.FullName = b.FullName
            End Operator
            Public Shared Operator <>(a As UserClass, b As UserClass) As Boolean
                Return a.FullName <> b.FullName
            End Operator

            ''' <summary>
            ''' Register an new instance of the class
            ''' </summary>
            Public Sub RegisterInstance(instance As ClassInstance)
                If Me._disposed Then Return
                If instance.UserClass = Me Then
                    Me._Instances.Add(instance)
                End If
            End Sub

            ''' <summary>
            ''' Convert the class to a (human-readable) string
            ''' </summary>
            ''' <returns></returns>
            Public Shadows Function ToString(Optional ignoreScope As String = "") As String
                Dim result As New StringBuilder()
                For Each m As String In Modifiers
                    result.Append(m).Append(" ")
                Next
                result.Append("class ")

                Dim scope As String = Evaluator.RemoveRedundantScope(DeclaringScope, ignoreScope)
                result.Append(scope.Trim())
                If Not String.IsNullOrWhiteSpace(scope) Then result.Append(SCOPE_SEP)
                result.Append(Name)
                If Not BaseClasses.Count = 0 Then
                    result.Append(":")
                    Dim first As Boolean = True
                    For Each b As String In BaseClasses
                        If Not first Then result.Append(",") Else first = False
                        result.Append(Evaluator.RemoveRedundantScope(Evaluator.UserClasses(b).FullName, ignoreScope))
                    Next
                End If
                result.AppendLine()

                result.Append(Body)
                Return result.ToString()
            End Function

            Public Sub Dispose() Implements IDisposable.Dispose
                If Me._disposed Then Return
                Me._disposed = True
                For Each ins As ClassInstance In Me.Instances
                    ins.Dispose()
                Next

                For Each f As String In Me.AllFields.Keys
                    Evaluator.SetVariable(Me.InnerScope & SCOPE_SEP & f, Double.NaN)
                Next
                ' prevent access
                Me.Modifiers.Clear()
                Me.Fields.Clear()
                Me._Name = ""
            End Sub
        End Class

        ''' <summary>
        ''' Represents a segment in the expression obtained after it is tokenized, consisting of 
        ''' an object and an operator
        ''' If either is unavailable they are set to null.
        ''' </summary>
        Private Structure Token
            Public [Object] As ObjectTypes.EvalObjectBase
            Public [Operator] As OperatorRegistar.Operator
            Public Sub New(evalObject As ObjectTypes.EvalObjectBase, operatorBefore As OperatorRegistar.Operator)
                Me.Object = evalObject
                Me.Operator = operatorBefore
            End Sub
        End Structure

        ''' <summary>
        ''' A special data structure used to store tokens
        ''' Allows for indexing, removal, appending, lookup of tokens with a certain precedence
        ''' </summary>
        Private NotInheritable Class TokenList
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
                For Each i As Integer In [Enum].GetValues(GetType(OperatorRegistar.ePrecedence))
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
            Public Function OperatorsWithPrecedenceCount(prec As OperatorRegistar.ePrecedence) As Integer
                Return _opsByPrecedence(prec).Count
            End Function

            ''' <summary>
            ''' Get a list of token indecess with the precedence specified
            ''' </summary>
            ''' <param name="prec"></param>
            ''' <returns></returns>
            Public Function OperatorsWithPrecedence(prec As OperatorRegistar.ePrecedence) As List(Of Integer)
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

            '''' <summary>
            '''' For debugging purposes
            '''' Converts the list to a human-readable string.
            '''' Each list represents a token, with
            '''' the first row indicating if the token is NOT removed (pointing at another token), 
            '''' the second indicating the operator, and the third the object.
            '''' The final row contains a count of remaining valid (not removed) objects.
            '''' </summary>
            '''' <returns></returns>
            'Public Overrides Function ToString() As String
            '    Dim str As New StringBuilder("OPER" & vbTab)
            '    For i As Integer = 0 To Me.OperatorCount - 1

            '        If Me.IsRemoved(i) Then Continue For

            '        If i > 0 Then str.Append(vbTab)

            '        If Me.OperatorAt(i) Is Nothing Then
            '            str.Append("Null")
            '        Else
            '            str.Append(Me.OperatorAt(i).Signs(0))
            '        End If

            '    Next

            '    str.Append(vbCrLf & "VALUE" & vbTab)
            '    For i As Integer = 0 To Me.ObjectCount - 1

            '        If Me.IsRemoved(i) Then Continue For
            '        If i > 0 Then str.Append(vbTab)

            '        If Me.ObjectAt(i) Is Nothing Then
            '            str.Append("Null")
            '        Else
            '            str.Append(Me.ObjectAt(i).ToString())
            '        End If

            '    Next
            '    str.Append(vbCrLf & "TOTAL ITEMS " & vbTab & Me.ValidCount)
            '    Return str.ToString()
            'End Function
        End Class
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

        ''' <summary>
        ''' The root namespace
        ''' </summary>
        Public Const ROOT_NAMESPACE As String = "cantus"

        ''' <summary>
        ''' Character separating namespaces, etc.; For example, '.' in 'cantus.abc'
        ''' </summary>
        Public Const SCOPE_SEP As Char = "."c

#End Region
#Region "Variables"
        ' modes
        ''' <summary>
        ''' The output mode of the evaluator
        ''' </summary>
        ''' <returns></returns>
        Public Property OutputFormat As eOutputFormat

        ''' <summary>
        ''' The angle representation mode of the evaluator (radians, degrees, gradians)
        ''' </summary>
        ''' <returns></returns>
        Public Property AngleMode As eAngleRepresentation

        ''' <summary>
        ''' The number of spaces that would represent a tab. Default is 4.
        ''' </summary>
        ''' <returns></returns>
        Public Property SpacesPerTab As Integer

        ''' <summary>
        ''' If true, force explicit declaration of variables
        ''' </summary>
        Public Property ExplicitMode As Boolean

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
        ''' Dictionary of user function definitions
        ''' </summary>
        Public ReadOnly Property UserFunctions As New Dictionary(Of String, UserFunction)

        ''' <summary>
        ''' Dictionary for storing variables
        ''' </summary>
        Public ReadOnly Property Variables As New Dictionary(Of String, Variable)

        ''' <summary>
        ''' Dictionary of user class definitions
        ''' </summary>
        Public ReadOnly Property UserClasses As New Dictionary(Of String, UserClass)

        ''' <summary>
        ''' Stores the names of imported scopes
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Imported As New HashSet(Of String)

        ''' <summary>
        ''' Stores the names of loaded scopes
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Loaded As New HashSet(Of String)

        ''' <summary>
        ''' Records the current scope of this evaluator
        ''' </summary>
        Public Property Scope As String

        ''' <summary>
        ''' Get the line number the evaluator started from, used for error reporting
        ''' </summary>
        Private _baseLine As Integer

        ''' <summary>
        ''' Get the line number the evaluator is currently processing, used for error reporting
        ''' </summary>
        Private _curLine As Integer

        ''' <summary>
        ''' A list of threads managed by this evaluator
        ''' </summary>
        Private _threads As New List(Of Thread)

        ''' <summary>
        ''' The id used for the next unnamed scope created from this evaluator
        ''' </summary>
        Private _anonymousScopeID As Integer = 0
#End Region

#Region "Events"
        ''' <summary>
        ''' Event raised when any async evauation is complete.
        ''' </summary>
        ''' <param name="sender">The evaluator that sent this result.</param>
        ''' <param name="result">The value of the result</param>
        Public Event EvalComplete(sender As Object, result As Object)
#End Region

#Region "Evaluator Constants"
        ''' <summary>
        ''' List of predefined constants, as a dictionary
        ''' By default this includes some often used math, physics, and chemistry constants. 
        ''' </summary>
        ''' <returns></returns>
        Private Shared ReadOnly Property _default As New Dictionary(Of String, Object) From
        {
            {"e", Math.E},
            {"pi", Math.PI},
            {"π", Math.PI},
            {"phi", 1.61803398875},
            {"φ", 1.61803398875},
            {"avogadro", 6.0221409E+23},
            {"na", 6.0221409E+23},
            {"G", 0.0000000000667408},
            {"gravity", 0.0000000000667408},
            {"g", 9.807},
            {"i", Numerics.Complex.ImaginaryOne},
            {"imaginaryunit", Numerics.Complex.ImaginaryOne},
            {"c", 299792458.0},
            {"lightspeed", 299792458.0},
            {"h", 6.6260755E-34},
            {"planck", 6.6260755E-34},
            {"hbar", 1.05457266E-34},
            {"planckreduced", 1.05457266E-34},
            {"e0", 0.000000000008854187817},
            {"permittivity", 0.000000000008854187817},
            {"mu0", 4.0 * Math.PI * 0.0000001},
            {"permeability", 4.0 * Math.PI * 0.0000001},
            {"F", 96485.3329},
            {"faraday", 96485.3329},
            {"me", 9.10938356E-31},
            {"electronmass", 9.10938356E-31},
            {"mp", 1.6726219E-27},
            {"protonmass", 1.6726219E-27},
            {"q", 1.60217662E-19},
            {"elemcharge", 1.60217662E-19},
            {"soundspeed", 343.2},
            {"vs", 343.2},
            {"R", 8.3144598},
            {"gas", 8.3144598},
            {"cmperinch", 2.54},
            {"torrsperatm", 760.0},
            {"torrsperkpa", 7.50062},
            {"prime", 2 ^ 31 - 1}
        }


        ''' <summary>
        ''' List of reserved names not to be used in function or variable names
        ''' </summary>
        ''' <returns></returns>
        Private Shared ReadOnly Property _reserved As New HashSet(Of String)(
        {
            DEFAULT_VAR_NAME, "if", "else", "not", "and", "or", "xor",
            "while", "for", "in", "to", "step", "until", "repeat", "run", "import", "function", "let", "global",
            "undefined", "null", "switch", "case", "load", "prototype", "namespace"
        })

        ''' <summary>
        ''' Reload the default constants into variable storage (accessible via Reload() in execution)
        ''' <param name="name">Optional: if specified, only reloads constant with name specified</param>
        ''' </summary>
        Public Sub ReloadDefault(Optional ByVal name As String = "")
            If name <> "" Then
                If _default.ContainsKey(name) Then
                    SetVariable(name, _default(name), _Scope)
                Else
                    Throw New EvaluatorException(name & " is not a valid default variable")
                End If
            Else
                For Each kvp As KeyValuePair(Of String, Object) In _default
                    SetVariable(kvp.Key, kvp.Value, _Scope)
                Next
            End If
        End Sub
#End Region
#End Region

#Region "Evaluator Logic"

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
        ''' <param name="outputFormat">The output mode of this evaluator. E.g.: MathO: 0.5->1/2; SciO: 0.5->5 E -1; LineO: 0.5->0.500</param>
        ''' <param name="angleRepr">The angle representation mode of this evaluator (Radians, Degrees, etc.)</param>
        ''' <param name="spacesPerTab">The number of spaces per tab, default is 4</param>
        ''' <param name="prevAns">List of previous answers</param>
        ''' <param name="vars">Variable definitions to start</param>
        ''' <param name="userFunctions">Dictioanry of user function definitions</param>
        ''' <param name="baseLine">The line number that this evaluator started at, used for error reporting</param>
        ''' <param name="scope">The name of the scope of this evaluator</param>
        Public Sub New(Optional outputFormat As eOutputFormat = eOutputFormat.Math,
                       Optional angleRepr As eAngleRepresentation = eAngleRepresentation.Radian,
                       Optional spacesPerTab As Integer = 4,
                       Optional explicit As Boolean = False,
                       Optional prevAns As List(Of ObjectTypes.EvalObjectBase) = Nothing,
                       Optional vars As Dictionary(Of String, Variable) = Nothing,
                       Optional userFunctions As Dictionary(Of String, UserFunction) = Nothing,
                       Optional baseLine As Integer = 0,
                       Optional scope As String = ROOT_NAMESPACE, Optional reloadDefault As Boolean = True
                       )

            Me.New()

            Me.OutputFormat = outputFormat
            Me.AngleMode = angleRepr
            Me.SpacesPerTab = spacesPerTab
            Me.ExplicitMode = explicit

            If Not prevAns Is Nothing Then Me.PrevAns = prevAns
            If Not vars Is Nothing Then Me.Variables = vars
            If Not userFunctions Is Nothing Then Me.UserFunctions = userFunctions

            Me._baseLine = baseLine
            Me._curLine = baseLine
            Me._Scope = scope

            Loaded.Add(ROOT_NAMESPACE)
            If IsExternalScope(scope, ROOT_NAMESPACE) Then Me.Import(ROOT_NAMESPACE)

            If reloadDefault Then
                ' reload default variable values
                Me.ReloadDefault()
            End If
        End Sub

        ''' <summary>
        ''' Load all files within a directory, for internal uses only (handled by Load())
        ''' </summary>
        Private Sub LoadDir(path As String, Optional asInternal As Boolean = False, Optional import As Boolean = False)
            Dim dir As New IO.DirectoryInfo(path)
            For Each fi As FileInfo In dir.GetFiles("*.can", SearchOption.AllDirectories)
                Dim curDir As String = Environment.CurrentDirectory
                Dim fp As String = fi.FullName
                If fp.StartsWith(curDir) Then fp = fp.Substring(curDir.Count + 1)
                Load(fp, asInternal, False)
            Next
            Dim newScope As String = path
            If newScope.EndsWith(".can") Then newScope = newScope.Remove(newScope.Length - 4)
            If newScope.StartsWith("include") Then newScope = newScope.Substring("include".Length)
            If newScope <> IO.Path.GetFullPath(newScope) Then
                newScope = IO.Path.GetFileName(IO.Path.GetDirectoryName(newScope)) & SCOPE_SEP & IO.Path.GetFileName(newScope)
            End If
            newScope = newScope.Replace("/", SCOPE_SEP).Replace("\", SCOPE_SEP).Trim({SCOPE_SEP})
            If import Then Me.Import(newScope.Trim())
        End Sub

        ''' <summary>
        ''' Make available the specified package for use inside the evaluator (files in plugin/ and init/ are imported by default)
        ''' Accepts: 
        ''' 1. Absolute path to file (uses parent directory + file name as package name)
        ''' 2. Relative path to directory/file from current directory
        ''' 3. Relative path to directory/file from include/ 
        ''' 4. Relative path from current directory or include, using SCOPE_SEP (usually ".") as separator
        ''' The extension .can can be ignored
        ''' </summary>
        ''' <param name="path">Path of the script to evaluate</param>
        ''' <param name="asInternal">If true, the script is exceuted in the current scope</param>
        ''' <param name="import">If true, imports the package into the evaluator after loading</param>
        Public Sub Load(path As String, Optional asInternal As Boolean = False, Optional import As Boolean = False)
            path = path.Trim()

            ' if file does not exist, see if it is using SCOPE_SEP notation instead of absolute path
            If Not IO.File.Exists(path) Then
                '
                If IO.Directory.Exists(path) Then ' it's a directory, so load entire directory and exit
                    LoadDir(path, asInternal, import)
                    Return
                End If

                path = path.Replace(SCOPE_SEP, IO.Path.DirectorySeparatorChar)
                If path.EndsWith(IO.Path.DirectorySeparatorChar & "can") Then path = path.Remove(path.Length - 4)
                If Not path.EndsWith(".can") Then path &= ".can"

                If Not IO.File.Exists(path) Then
                    If IO.Directory.Exists(path) Then ' load entire directory, with SCORE_SEP notation
                        LoadDir(path, asInternal, import)
                        Return
                    End If
                    ' if still not found look in the include directory
                    If Not path.StartsWith("include" & IO.Path.DirectorySeparatorChar) Then
                        path = "include" & IO.Path.DirectorySeparatorChar & path
                    End If
                End If
            End If

            If IO.Directory.Exists(path) Then ' load entire directory, with include
                LoadDir(path, asInternal, import)
                Return
            End If

            Dim newScope As String = _Scope
            If Not asInternal Then
                ' get the scope name for the file
                If IO.Path.GetFullPath(path) <> path Then
                    newScope = path.Replace("/", SCOPE_SEP).Replace("\", SCOPE_SEP)
                    If newScope.StartsWith("include.") Then newScope = newScope.Substring("include.".Count) ' do not include 'include'
                Else
                    newScope = IO.Path.GetDirectoryName(path) & SCOPE_SEP & IO.Path.GetFileName(path)
                    newScope = IO.Path.GetFileName(IO.Path.GetDirectoryName(newScope)) & SCOPE_SEP & IO.Path.GetFileName(newScope)
                End If
                If newScope.EndsWith(".can") Then newScope = newScope.Remove(newScope.Length - 4)
                newScope = newScope.Trim({SCOPE_SEP})
            End If

            Dim tmpEval As Evaluator = DeepCopy(newScope)

            Dim except As Exception = Nothing
            Try
                tmpEval.EvalRaw(File.ReadAllText(path), noSaveAns:=True)
            Catch ex As Exception
                except = ex ' first ensure all things we currently have are loaded. Throw the error in the end.
            End Try

            ' load new user functions
            For Each uf As UserFunction In tmpEval.UserFunctions.Values
                If uf.Modifiers.Contains("private") OrElse uf.Modifiers.Contains("internal") Then Continue For ' ignore private functions
                UserFunctions(uf.FullName) = (uf)
            Next

            ' load new variables
            For Each var As Variable In tmpEval.Variables.Values
                If var.Modifiers.Contains("private") OrElse var.Modifiers.Contains("internal") Then Continue For ' ignore private variables
                Variables(var.FullName) = var
            Next

            ' load new classes
            For Each uc As UserClass In tmpEval.UserClasses.Values
                If uc.Modifiers.Contains("private") OrElse uc.Modifiers.Contains("internal") Then Continue For ' ignore private variables
                UserClasses(uc.FullName) = uc
            Next

            If asInternal Then
                OutputFormat = tmpEval.OutputFormat
                AngleMode = tmpEval.AngleMode
                SpacesPerTab = tmpEval.SpacesPerTab
                ExplicitMode = tmpEval.ExplicitMode
            End If

            tmpEval.Dispose()

            Loaded.Add(newScope)

            ' import the scope if we need to
            If import Then Me.Import(newScope.Trim())

            While newScope.Contains(SCOPE_SEP)
                newScope = newScope.Remove(newScope.LastIndexOf(SCOPE_SEP))
                Loaded.Add(newScope)
            End While

            ' if there was an error, throw it now
            If Not except Is Nothing Then Throw except
        End Sub

        ''' <summary>
        ''' Import a scope so items declared in it may be accessed directly
        ''' </summary>
        Public Sub Import(scope As String)
            Me.Imported.Add(scope)
        End Sub

        ''' <summary>
        ''' Un-import a scope imported with import
        ''' </summary>
        Public Sub Unimport(scope As String)
            If Me.Imported.Contains(scope) Then Me.Imported.Remove(scope)
        End Sub

        ''' <summary>
        ''' Evauate a multi-line script and return the result as a string
        ''' </summary>
        ''' <param name="script">The script to evaluate</param>
        ''' <param name="noSaveAns">If true, evaluates without saving answers</param>
        ''' <param name="declarative">If true, disallows all expressions other than declarations</param>
        ''' <param name="internal">If true, returns a internal statementresult object with information on return code</param>
        Public Function Eval(script As String, Optional noSaveAns As Boolean = False, Optional declarative As Boolean = False,
            Optional internal As Boolean = False) As String
            Return InternalFunctions.O(EvalRaw(script, noSaveAns, declarative, internal))
        End Function

        ''' <summary>
        ''' Evauate a multi-line script asynchroneously and raises the EvalComplete event when done
        ''' </summary>
        ''' <param name="script">The script to evaluate</param>
        ''' <param name="noSaveAns">If true, evaluates without saving answers</param>
        ''' <param name="declarative">If true, disallows all expressions other than declarations</param>
        ''' <param name="internal">If true, returns a internal statementresult object with information on return code</param>
        Public Sub EvalAsync(script As String, Optional noSaveAns As Boolean = False, Optional declarative As Boolean = False,
            Optional internal As Boolean = False)
            Dim th As New Thread(Sub()
                                     Try
                                         RaiseEvent EvalComplete(Me, Eval(script, noSaveAns, declarative, internal))
                                     Catch ex As Exception
                                         RaiseEvent EvalComplete(Me, ex.Message.Trim())
                                     End Try
                                     Me._threads.Remove(Threading.Thread.CurrentThread)
                                 End Sub)
            Me._threads.Add(th)
            th.Start()
        End Sub

        ''' <summary>
        ''' Evauate a multi-line script and return the result as a system object
        ''' </summary>
        ''' <param name="script">The script to evaluate</param>
        ''' <param name="noSaveAns">If true, evaluates without saving answers</param>
        ''' <param name="declarative">If true, disallows all expressions other than declarations</param>
        ''' <param name="internal">If true, returns a internal statementresult object with information on return code</param>
        Public Function EvalRaw(script As String, Optional noSaveAns As Boolean = False,
            Optional declarative As Boolean = False, Optional internal As Boolean = False) As Object

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

                        ' multiline lambda
                        If fullLine.TrimEnd.EndsWith("=>") OrElse
                            (fullLine.TrimEnd().EndsWith("`") AndAlso
                            fullLine.IndexOf("`") = fullLine.LastIndexOf("`")) Then
                            lineNum += 1
                            While lineNum < lines.Count
                                fullLine = fullLine & vbNewLine & lines(lineNum)
                                lineNum += 1
                            End While
                        End If

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

                        If Not curSM Is Nothing AndAlso Not (declarative AndAlso Not curSM.Declarative) Then ' if matches

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

                            If String.IsNullOrWhiteSpace(l) Then Continue For
                            If declarative Then Throw New Exception("Declarative mode disallows non-declarative statements.")

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
                    ' save answer
                    If Not noSaveAns Then
                        ' do not save if undefined
                        If (Not TypeOf lastVal Is BigDecimal OrElse Not DirectCast(lastVal, BigDecimal).IsUndefined) AndAlso
                             (Not TypeOf lastVal Is Double OrElse Not Double.IsNaN(CDbl(lastVal))) Then
                            PrevAns.Add(ObjectTypes.DetectType(lastVal))
                        End If
                    End If
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
                'MsgBox(ex.ToString)
                Throw New EvaluatorException(ex.Message, _curLine + 1)
            End Try
        End Function

        ''' <summary>
        ''' Evauate a multi-line script asynchroneously and raises the EvalComplete event when done
        ''' returning the result as a system object
        ''' </summary>
        ''' <param name="script">The script to evaluate</param>
        ''' <param name="noSaveAns">If true, evaluates without saving answers</param>
        ''' <param name="declarative">If true, disallows all expressions other than declarations</param>
        ''' <param name="internal">If true, returns a internal statementresult object with information on return code</param>
        Public Sub EvalRawAsync(script As String, Optional noSaveAns As Boolean = False, Optional declarative As Boolean = False,
            Optional internal As Boolean = False)
            Dim th As New Thread(Sub()
                                     Try
                                         RaiseEvent EvalComplete(Me, EvalRaw(script, noSaveAns, declarative, internal))
                                     Catch ex As Exception
                                         RaiseEvent EvalComplete(Me, ex.Message.Trim())
                                     End Try
                                     Me._threads.Remove(Threading.Thread.CurrentThread)
                                 End Sub)
            Me._threads.Add(th)
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
        ''' <param name="noSaveAns">If true, evaluates without saving answers</param>
        ''' <param name="conditionMode">If true, the = operator is always used for comparison 
        ''' (otherwise both assignment and comparison)</param>
        Public Function EvalExpr(expr As String, noSaveAns As Boolean, conditionMode As Boolean) As String
            Return InternalFunctions.O(EvalExprRaw(expr, noSaveAns, conditionMode))
        End Function

        ''' <summary>
        ''' Evauate a mathematical expression asynchroneously and raises the EvalComplete event when done
        ''' </summary>
        ''' <param name="expr">The expression to evaluate</param>
        ''' <param name="noSaveAns">If true, evaluates without saving answers</param>
        ''' <param name="conditionMode">If true, the = operator is always used for comparison 
        ''' (otherwise both assignment and comparison)</param>
        Public Sub EvalExprAsync(expr As String, noSaveAns As Boolean, conditionMode As Boolean)
            Dim th As New Thread(Sub()
                                     Try
                                         RaiseEvent EvalComplete(Me, EvalExpr(expr, noSaveAns, conditionMode))
                                     Catch ex As Exception
                                         RaiseEvent EvalComplete(Me, ex.Message.Trim())
                                     End Try
                                     Me._threads.Remove(Threading.Thread.CurrentThread)
                                 End Sub)
            Me._threads.Add(th)
            th.Start()
        End Sub

        ''' <summary>
        ''' Evauate a mathematical expression and return the resulting object
        ''' </summary>
        ''' <param name="expr">The expression to evaluate</param>
        ''' <param name="noSaveAns">If true, evaluates without saving answers</param>
        ''' <param name="conditionMode">If true, the = operator is always used for comparison 
        ''' (otherwise both assignment and comparison)</param>
        Public Function EvalExprRaw(expr As String, Optional noSaveAns As Boolean = False,
                                    Optional conditionMode As Boolean = False) As Object

            Dim oldmode As Boolean = conditionMode
            OperatorRegistar.ConditionMode = conditionMode
            Dim resultObj As ObjectTypes.EvalObjectBase = ResolveOperators(Tokenize(expr))
            OperatorRegistar.ConditionMode = oldmode

            Dim result As Object = BigDecimal.Undefined

            If Not resultObj Is Nothing Then

                If TypeOf resultObj Is ObjectTypes.Reference AndAlso
                    Not TypeOf DirectCast(resultObj, ObjectTypes.Reference).GetRefObject() Is
                    ObjectTypes.Reference Then
                    resultObj = DirectCast(resultObj, ObjectTypes.Reference).GetRefObject()
                End If

                If TypeOf resultObj Is ObjectTypes.Number Then
                    result = CType(resultObj, ObjectTypes.Number).BigDecValue()
                ElseIf TypeOf resultObj Is ObjectTypes.Reference
                    result = resultObj
                Else
                    result = resultObj.GetValue()
                End If

                If result Is Nothing Then result = BigDecimal.Undefined
            End If

            If Not noSaveAns Then
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
        ''' <param name="noSaveAns">If true, evaluates without saving answers</param>
        ''' <param name="conditionMode">If true, the = operator is always used for comparison 
        ''' (otherwise both assignment and comparison)</param>
        Public Sub EvalExprRawAsync(expr As String, noSaveAns As Boolean, conditionMode As Boolean)
            Dim th As New Thread(Sub()
                                     Try
                                         RaiseEvent EvalComplete(Me, EvalExprRaw(expr, noSaveAns, conditionMode))
                                     Catch ex As Exception
                                         RaiseEvent EvalComplete(Me, ex.Message.Trim())
                                     End Try
                                     Me._threads.Remove(Threading.Thread.CurrentThread)
                                 End Sub)
            Me._threads.Add(th)
            th.Start()
        End Sub

        ''' <summary>
        ''' Goes through a list of tokens and evaluates all operators by precedence
        ''' </summary>
        ''' <param name="tokens">The list of tokens to evaluate</param>
        ''' <returns></returns>
        Private Function ResolveOperators(tokens As TokenList) As ObjectTypes.EvalObjectBase
            ' start from operators with highest precedence, skipping the brackets (already evaluated when tokenizing)
            For i As Integer = [Enum].GetValues(GetType(OperatorRegistar.ePrecedence)).Length - 1 To 0 Step -1
                Dim cur_precedence As OperatorRegistar.ePrecedence = CType(i, OperatorRegistar.ePrecedence)

                Dim prevct As Integer
                ' keep looping until all operators are done
                While tokens.OperatorsWithPrecedenceCount(cur_precedence) > 0
                    Dim preclst As List(Of Integer) = tokens.OperatorsWithPrecedence(cur_precedence)
                    prevct = tokens.OperatorsWithPrecedenceCount(cur_precedence)

                    ' RTL evaluation for assignment operators so you can chain them
                    If cur_precedence = OperatorRegistar.ePrecedence.assignment Then preclst.Reverse()

                    For Each opid As Integer In preclst

                        ' check if the operator has not already been executed and is of the correct precedence
                        If tokens.IsRemoved(opid) OrElse tokens.OperatorAt(opid).Precedence <> cur_precedence Then Continue For

                        Dim prevtoken As Token = If(opid > 0, tokens(opid - 1), New Token(Nothing, Nothing))
                        Dim curtoken As Token = tokens(opid)
                        Dim result As ObjectTypes.EvalObjectBase
                        If TypeOf curtoken.Operator Is OperatorRegistar.UnaryOperatorBefore Then ' operators like x!
                            Dim op As OperatorRegistar.UnaryOperatorBefore = CType(curtoken.Operator, OperatorRegistar.UnaryOperatorBefore)

                            ' if we're not passing by reference then copy and "dereference" the references before passing
                            If Not op.ByReference AndAlso Not prevtoken.Object Is Nothing Then
                                prevtoken.Object = prevtoken.Object.GetDeepCopy()
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
                                prevtoken.Object = prevtoken.Object.GetDeepCopy()
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

                            If curtoken.Object Is Nothing AndAlso opid < tokens.OperatorCount - 1 Then
                                ' allow for chaining of binary operators with same precedence:
                                ' defer evaluation until next pass if the right side Is null.
                                If tokens(opid + 1).Operator.Precedence = cur_precedence Then
                                    ' for same precedence, just continue
                                    Continue For
                                Else
                                    ' for different precedence, we'll have to evaluate separately and join back
                                    tokens.SetObject(opid, ObjectTypes.DetectType(EvalExprRaw(
                                                                  tokens.OperatorAt(opid + 1).Signs(0).ToString() &
                                                                  tokens.ObjectAt(opid + 1).ToString(), True)))
                                    tokens.RemoveAt(opid + 1)
                                    prevct += 1 ' cheat: continue even though we didn't make progress
                                    Continue For
                                End If
                            End If

                            ' if we're not passing by reference then "dereference" the references before passing
                            If Not op.ByReference Then
                                If Not prevtoken.Object Is Nothing Then
                                    If TypeOf prevtoken.Object Is ObjectTypes.Reference Then
                                        prevtoken.Object = CType(prevtoken.Object, ObjectTypes.Reference).ResolveObj()
                                    End If
                                    prevtoken.Object = prevtoken.Object.GetDeepCopy()
                                End If
                                If Not curtoken.Object Is Nothing Then
                                    If TypeOf curtoken.Object Is ObjectTypes.Reference Then
                                        curtoken.Object = CType(curtoken.Object, ObjectTypes.Reference).ResolveObj()
                                    End If
                                    curtoken.Object = curtoken.Object.GetDeepCopy()
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

            Return tokens.ObjectAt(tokens.ObjectCount - 1)
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
                                        Dim endIdx As Integer = DirectCast(OperatorRegistar.OperatorWithSign("("),
                                                                       OperatorRegistar.Bracket).
                                                                       FindCloseBracket(funcargs, OperatorRegistar)
                                        If endIdx < funcargs.Length Then
                                            funcargs = funcargs.Remove(endIdx)
                                        End If
                                    Else
                                        Throw New EvaluatorException("(: No close bracket found")
                                    End If

                                    If lst.ObjectCount > 0 AndAlso lst.OperatorCount >= lst.ObjectCount AndAlso
                                                    eo.ToString().Trim().StartsWith(SCOPE_SEP) Then
                                        left = lst.ObjectAt(lst.ObjectCount - 1)
                                    End If
                                    varlist = ResolveFunctions(eo.ToString(), funcargs, left)

                                    ' advance past this function
                                    idx = j + funcargs.Count + 1
                                    i = idx - 1

                                Else ' this consists of variables only, so only resolve variables / function pointers
                                    If op.AssignmentOperator Then
                                        ' for assignment operators, do not resolve the variables
                                        varlist = New List(Of ObjectTypes.EvalObjectBase)({GetVariableRef(eo.ToString())})
                                    Else
                                        ' try resolving a function pointer

                                        varlist = New List(Of EvalObjectBase)()
                                        Dim fn As String = eo.ToString()
                                        If HasUserFunction(fn) Then
                                            varlist.Add(New ObjectTypes.Lambda(fn,
                                                       GetUserFunction(fn).Args,
                                                       True))
                                        ElseIf HasFunction(fn)
                                            varlist.Clear()
                                            If fn.StartsWith(ROOT_NAMESPACE) Then fn = fn.Remove(ROOT_NAMESPACE.Length).Trim({SCOPE_SEP})
                                            Dim info As MethodInfo = GetType(InternalFunctions).GetMethod(fn.ToLowerInvariant(),
                                                        Reflection.BindingFlags.IgnoreCase Or
                                                        Reflection.BindingFlags.Public Or Reflection.BindingFlags.Instance Or
                                                        Reflection.BindingFlags.DeclaredOnly)

                                            varlist.Add(New ObjectTypes.Lambda(fn,
                                                    (From param As ParameterInfo In info.GetParameters()
                                                     Select param.Name),
                                                               True))
                                        Else
                                            If lst.ObjectCount > 0 AndAlso Not lst.ObjectAt(lst.ObjectCount - 1) Is Nothing AndAlso
                                                eo.ToString().StartsWith(SCOPE_SEP) Then
                                                varlist = ResolveFunctions(eo.ToString(), "", lst.ObjectAt(lst.ObjectCount - 1))
                                            End If
                                        End If
                                        If varlist.Count = 0 Then
                                            varlist = ResolveVariables(eo.ToString())
                                        Else
                                            ' advance past this function
                                            idx = j + 1
                                            i = idx - 1
                                        End If
                                    End If
                                End If

                                If varlist.Count > 0 Then ' good we found variables/functions, let's add them

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

                        ' if we find an operator with brackets type
                        ' we evaluate the bracket and continue after it.
                        ' If the value before is an identifier we recognize it as a function so we skip this
                        If TypeOf op Is OperatorRegistar.Bracket AndAlso
                                    (op.Signs(0) <> "( " OrElse eo Is Nothing OrElse Not ObjectTypes.Identifier.IsType(eo)) Then

                            Dim inner As String = expr.Substring(j)
                            Dim endIdx As Integer

                            If op.Signs.Count > 0 Then
                                endIdx = DirectCast(op, OperatorRegistar.Bracket).FindCloseBracket(inner, OperatorRegistar)
                            Else
                                Throw New EvaluatorException("Invalid bracket operator: must have at least 1 sign")
                            End If

                            If endIdx >= 0 Then
                                If endIdx < inner.Length Then inner = inner.Remove(endIdx)

                                Dim brkt As OperatorRegistar.Bracket = DirectCast(op, OperatorRegistar.Bracket)
                                Dim left As ObjectTypes.EvalObjectBase = Nothing
                                Dim orig As ObjectTypes.EvalObjectBase = Nothing

                                If lst.ObjectCount > 0 Then
                                    left = lst.ObjectAt(lst.ObjectCount - 1)
                                    ' if we're not passing by reference then "dereference" the references before passing
                                    If Not brkt.ByReference AndAlso Not left Is Nothing Then
                                        left = left.GetDeepCopy()
                                        If TypeOf left Is ObjectTypes.Reference Then
                                            left = CType(left, ObjectTypes.Reference).GetRefObject()
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
            If idx < expr.Length AndAlso Not expr.Substring(idx, expr.Length - idx).Trim() = "" Then
                Dim eo As ObjectTypes.EvalObjectBase = ObjectTypes.StrDetectType(expr.Substring(idx, expr.Length - idx).Trim())

                ' if the object we get is an identifier, we try to break it into variables which are resolved using ResolveVariables
                If ObjectTypes.Identifier.IsType(eo) Then
                    Dim varlist As New List(Of ObjectTypes.EvalObjectBase)

                    ' try resolving a function pointer

                    varlist = New List(Of EvalObjectBase)()
                    Dim fn As String = eo.ToString()
                    If HasUserFunction(fn) Then
                        varlist.Add(New ObjectTypes.Lambda(fn,
                                   GetUserFunction(fn).Args,
                                   True))
                    ElseIf HasFunction(fn)
                        varlist.Clear()
                        If fn.StartsWith(ROOT_NAMESPACE) Then fn = fn.Remove(ROOT_NAMESPACE.Length).Trim({SCOPE_SEP})
                        Dim info As MethodInfo = GetType(InternalFunctions).GetMethod(fn.ToLowerInvariant(),
                                    Reflection.BindingFlags.IgnoreCase Or
                                    Reflection.BindingFlags.Public Or Reflection.BindingFlags.Instance Or
                                    Reflection.BindingFlags.DeclaredOnly)

                        varlist.Add(New ObjectTypes.Lambda(fn,
                                (From param As ParameterInfo In info.GetParameters()
                                 Select param.Name),
                                           True))
                    Else
                        If lst.ObjectCount > 0 AndAlso Not lst.ObjectAt(lst.ObjectCount - 1) Is Nothing AndAlso
                            eo.ToString().StartsWith(SCOPE_SEP) Then
                            varlist = ResolveFunctions(eo.ToString(), "", lst.ObjectAt(lst.ObjectCount - 1))
                        End If
                    End If

                    If varlist.Count = 0 Then
                        varlist = ResolveVariables(eo.ToString())
                    End If
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
            If str.Contains(SCOPE_SEP) AndAlso Not HasFunction(str) Then
                Dim baseTxt As String = str.Remove(str.IndexOf(SCOPE_SEP))
                If max = min AndAlso baseTxt.Length + 1 <> min Then
                    Throw New EvaluatorException("Member function is undefined")
                End If

                str = str.Substring(str.IndexOf(SCOPE_SEP) + 1)
                If Not String.IsNullOrEmpty(baseTxt) Then
                    Try
                        baseObj = GetVariableRef(baseTxt)
                    Catch
                    End Try
                    Dim br As ObjectTypes.Reference = DirectCast(baseObj, ObjectTypes.Reference)
                    Try
                        If baseObj Is Nothing OrElse (TypeOf br.Resolve() Is Double AndAlso Double.IsNaN(CDbl(br.Resolve())) OrElse
                            TypeOf br.Resolve() Is BigDecimal AndAlso CType(br.Resolve(), BigDecimal).IsUndefined) Then
                            baseObj = StrDetectType(baseTxt, Me, True)
                            br = New Reference(baseObj)
                        End If
                    Catch
                    End Try

                    If baseObj Is Nothing OrElse (TypeOf br.Resolve() Is Double AndAlso Double.IsNaN(CDbl(br.Resolve())) OrElse
                        TypeOf br.Resolve() Is BigDecimal AndAlso CType(br.Resolve(), BigDecimal).IsUndefined) Then
                        str = baseTxt & SCOPE_SEP & str ' try full name
                        baseObj = Nothing
                    Else
                        min = 0
                        max = 0
                    End If
                Else
                    If left Is Nothing Then
                        baseObj = GetDefaultVariableRef()
                        If baseObj Is Nothing Then
                            If baseObj Is Nothing Then str = baseTxt & SCOPE_SEP & str ' try full name
                        Else
                            min = 0
                            max = 0
                        End If
                    Else
                        baseObj = left
                    End If
                End If

            Else
                left = Nothing
            End If

            Dim argLst As New List(Of Object)
            If Not baseObj Is Nothing Then
                If TypeOf baseObj Is ObjectTypes.Tuple Then ' if a tuple is used, supplies multiple parameters
                    For Each r As ObjectTypes.Reference In CType(CType(baseObj, ObjectTypes.Tuple).GetValue(),
                        ObjectTypes.Reference())
                        argLst.Add(r.Resolve())
                    Next
                ElseIf TypeOf baseObj Is Reference
                    If TypeOf DirectCast(baseObj, Reference).ResolveObj() Is Reference Then
                        argLst.Add(baseObj)
                    Else
                        argLst.Add(DirectCast(baseObj, Reference).Resolve())
                    End If
                Else
                    argLst.Add(baseObj.GetValue())
                End If
            End If

            If Not String.IsNullOrWhiteSpace(args) Then
                Dim tuple As Object = EvalExprRaw("(" & args & ")", True)
                Dim otherarglst As New List(Of ObjectTypes.Reference)

                If TypeOf tuple Is ObjectTypes.Reference() Then
                    otherarglst.AddRange(DirectCast(tuple, ObjectTypes.Reference()))
                ElseIf TypeOf tuple Is ObjectTypes.Reference
                    otherarglst.Add(DirectCast(tuple, ObjectTypes.Reference))
                Else
                    otherarglst.Add(New ObjectTypes.Reference(tuple))
                End If

                For Each ref As ObjectTypes.Reference In otherarglst
                    If ref Is Nothing Then
                        argLst.Add(Double.NaN)
                    Else
                        If TypeOf ref.ResolveObj() Is Reference Then
                            argLst.Add(ref)
                        Else
                            argLst.Add(ref.Resolve())
                        End If
                    End If
                Next
            End If

            ' loop through string from left to right and look for functions on right
            For i As Integer = min To max
                If i >= str.Length Then Exit For
                Dim fn As String = str.Substring(i).Trim()
                Dim varstr As String = str.Remove(i)

                Dim lst As List(Of EvalObjectBase)
                Try
                    lst = ResolveVariables(varstr)
                Catch
                    Exit For
                End Try

                ' for class instances, try looking for members
                If Not baseObj Is Nothing AndAlso TypeOf baseObj.GetValue() Is ClassInstance Then
                    Dim ci As ClassInstance = DirectCast(baseObj.GetValue(), ClassInstance)
                    Dim ref As Reference = ci.ResolveField(fn, Scope)

                    If TypeOf ref.ResolveObj() Is Lambda Then
                        ' remove the first item in the arguments, which is set to the lambda expression itself
                        argLst.RemoveAt(0)
                        Dim lambda As ObjectTypes.Lambda = DirectCast(ref.ResolveObj(), ObjectTypes.Lambda)

                        If lambda.Args.Count <> argLst.Count Then
                            ' incorrect parameter count
                            Throw New EvaluatorException(ci.UserClass.Name & SCOPE_SEP & fn &
                                                          ": " & lambda.Args.Count & " parameter(s) expected")
                        Else
                            ' execute
                            Dim tmpEval As Evaluator = ci.UserClass.Evaluator.SubEvaluator()
                            tmpEval.Scope = ci.InnerScope
                            tmpEval.SubScope()
                            tmpEval.SetDefaultVariable(New Reference(ci))
                            lst.Add(ObjectTypes.DetectType(lambda.Execute(tmpEval, argLst, tmpEval.Scope)))
                        End If
                        Return lst
                    Else
                        If TypeOf ref.GetRefObject() Is Reference Then
                            lst.Add(ref)
                        Else
                            lst.Add(ref.ResolveObj())
                        End If
                        Return lst
                    End If
                ElseIf HasUserClass(fn) Then ' user class constructors

                    Dim uc As UserClass = GetUserClass(fn)
                    If uc.Constructor.Args.Count > 0 AndAlso argLst.Count = 0 Then
                        lst.Add(New ClassInstance(uc)) ' support creating empty objects without running constructor
                    Else
                        lst.Add(New ClassInstance(uc, argLst))
                    End If
                    Return lst

                ElseIf HasUserFunction(fn) Then ' user functions

                    Dim execResult As Object = ExecUserFunction(fn, argLst)
                    lst.Add(ObjectTypes.DetectType(execResult, True))
                    Return lst

                ElseIf HasVariable(fn) AndAlso ObjectTypes.Lambda.IsType(GetVariableRef(fn).ResolveObj()) Then
                    ' lambda expression/function pointer

                    Dim lambda As ObjectTypes.Lambda = DirectCast(GetVariableRef(fn).ResolveObj(), ObjectTypes.Lambda)
                    If lambda.Args.Count <> argLst.Count Then
                        Throw New EvaluatorException(fn & ": " & lambda.Args.Count & " parameter(s) expected" &
                                                    If(Not baseObj Is Nothing, "(self-referring resolution on)", ""))
                    Else
                        lst.Add(ObjectTypes.DetectType(lambda.Execute(Me, argLst)))
                    End If
                    Return lst

                Else ' internal functions defined in EvalFunctions
                    lst.Add(ObjectTypes.DetectType(ExecInternalFunction(fn, argLst)))
                    Return lst
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
                    If ObjectTypes.Number.StrIsType(cur) Then
                        ret.Add(New ObjectTypes.Number(cur))
                        i += j
                        Continue While

                    Else
                        Try
                            ret.Add(GetVariableRef(cur, True))
                            i += j
                            Continue While
                        Catch ex As Exception
                            If j = 1 Then ' really can't find anything
                                If ExplicitMode Then
                                    Throw New EvaluatorException("Variable " & str &
                                     " is undefined. (Explicit mode disallows implicit declaration)")
                                Else
                                    ret.Clear()
                                    SetVariable(str, Double.NaN)
                                    ret.Add(GetVariableRef(str))
                                    Exit While
                                End If
                            End If
                        End Try
                    End If
                Next
                i += 1
            End While
            Return ret
        End Function
#End Region

#Region "Variables, User Functions, Past Answers"

        ''' <summary>
        ''' Moves all namespaces to the scope and leaves only the name of the variable or function as name
        ''' e.g. name=a.b scope=cantus -> name=b scope=cantus.a
        ''' </summary>
        Friend Shared Sub NormalizeScope(ByRef name As String, ByRef scope As String)
            If name.Contains(SCOPE_SEP) Then
                Dim varscope As String = name.Remove(name.LastIndexOf(SCOPE_SEP))
                ' remove duplicate scope: name=cantus.a.b.c scope=cantus.a -> name=c scope=cantus.a.b
                If varscope.StartsWith(scope) Then varscope = varscope.Substring(scope.Length).Trim({SCOPE_SEP})
                scope &= SCOPE_SEP & varscope
                scope = scope.Trim({SCOPE_SEP})
                name = name.Substring(name.LastIndexOf(SCOPE_SEP) + 1)
            End If
        End Sub

        ''' <summary>
        ''' Removes redundant scope on a name relative to a scope: cantus.abc -> abc
        ''' </summary>
        Friend Shared Function RemoveRedundantScope(ByVal name As String, ByVal scope As String) As String
            If scope = name Then Return "" ' same scope, none required
            If name.StartsWith(scope & SCOPE_SEP) Then name = name.Substring(scope.Length + 1)
            Return name
        End Function

        ''' <summary>
        ''' Combine a scope and a name, removing redundancies
        ''' </summary>
        Friend Shared Function CombineScope(ByVal scope As String, ByVal name As String) As String
            Return scope & SCOPE_SEP & RemoveRedundantScope(name, scope)
        End Function

        ''' <summary>
        ''' Get the value of the variable with the name specified as an IEvalObject
        ''' </summary>
        ''' <param name="name">Name of the variable</param>
        ''' <param name="explicit">If true, simulates explicit mode even when not set on the evaluator</param>
        ''' <returns></returns>
        Friend Function GetVariableRef(ByVal name As String,
                                       Optional explicit As Boolean = False
                                       ) As ObjectTypes.Reference
            If name = "ans" Then Return New ObjectTypes.Reference(GetLastAns())
            Dim scope As String = _Scope
            name = RemoveRedundantScope(name, scope)

            If Variables.ContainsKey(name) Then Return Variables(name).Reference

            For Each s As String In GetAllAccessibleScopes()
                Dim temp As String = name
                While temp.Contains(SCOPE_SEP)
                    temp = temp.Remove(temp.LastIndexOf(SCOPE_SEP))
                    If HasVariable(s & SCOPE_SEP & temp) Then
                        Dim v As Reference = Variables(s & SCOPE_SEP & temp).Reference
                        If TypeOf v.ResolveObj() Is ClassInstance Then
                            Return DirectCast(v.ResolveObj(), ClassInstance).
                                ResolveField(name.Substring(temp.Length + 1), scope)
                        End If
                    End If
                End While

                temp = name
                NormalizeScope(temp, s)

                If Variables.ContainsKey(s & SCOPE_SEP & temp) Then
                    ' ignore if private
                    If Not IsParentScopeOf(s, scope) AndAlso
                            Variables(s & SCOPE_SEP & temp).Modifiers.Contains("private") Then Continue For
                    If scope <> s AndAlso IsParentScopeOf(s, scope) Then
                        SetVariable(temp, Variables(s & SCOPE_SEP & temp).Reference)
                        Return Variables(scope & SCOPE_SEP & temp).Reference
                    Else
                        Return Variables(s & SCOPE_SEP & temp).Reference
                    End If
                End If
            Next

            ' variable not found, implicit declaration?

            ' explicit mode: disallow
            If ExplicitMode OrElse explicit Then Throw New EvaluatorException(
                "Variable " & name & " is undefined. (Explicit mode disallows implicit declaration)")

            NormalizeScope(name, scope)

            ' classes: disallow any declarations within a class scope (unless specified in the class)
            Dim tmp As String = scope

            While True
                If HasUserClass(tmp) Then
                    Dim uc As UserClass = GetUserClass(scope)
                    Dim subName As String = name
                    If subName.Contains(SCOPE_SEP) Then subName = subName.Remove(subName.IndexOf(SCOPE_SEP))
                    If Not uc.AllFields.ContainsKey(subName) Then
                        Throw New EvaluatorException(
                            "Cannot declare variable " & name & " inside class " & UserClasses(scope).Name)
                        Exit While
                    End If
                End If

                If Not tmp.Contains(SCOPE_SEP) Then Exit While
                tmp = tmp.Remove(tmp.LastIndexOf(SCOPE_SEP))
            End While

            Dim var As New Variable(name, New ObjectTypes.Reference(Double.NaN), scope)
            Variables(var.FullName) = var

            Return Variables(scope & SCOPE_SEP & name).Reference
        End Function

        ''' <summary>
        ''' Get the value of the variable with the name specified as a system object
        ''' </summary>
        ''' <param name="name">Name of the variable</param>
        ''' <param name="explicit">If true, simulates explicit mode even when not set on the evaluator</param>
        Public Function GetVariable(ByVal name As String,
                                   Optional explicit As Boolean = False
                                    ) As Object
            If name = "ans" Then Return GetLastAns()
            Dim scope As String = _Scope
            name = RemoveRedundantScope(name, scope)

            If Variables.ContainsKey(name) Then Return Variables(name).Reference

            For Each s As String In GetAllAccessibleScopes()
                Dim temp As String = name
                While temp.Contains(SCOPE_SEP)
                    temp = temp.Remove(temp.LastIndexOf(SCOPE_SEP))
                    If HasVariable(scope & SCOPE_SEP & temp) Then
                        ' ignore if private
                        If Not IsParentScopeOf(s, scope) AndAlso
                            Variables(s & SCOPE_SEP & name).Modifiers.Contains("private") Then Continue While
                        Dim v As Reference = Variables(scope & SCOPE_SEP & temp).Reference
                        If TypeOf v.ResolveObj() Is ClassInstance Then
                            Return DirectCast(v.ResolveObj(), ClassInstance).
                                ResolveField(name.Substring(temp.Length + 1), scope).Resolve()
                        End If
                    End If
                End While
                temp = name
                NormalizeScope(temp, s)
                If Variables.ContainsKey(s & SCOPE_SEP & temp) Then
                    ' ignore if private
                    If Not IsParentScopeOf(s, scope) AndAlso
                            Variables(s & SCOPE_SEP & temp).Modifiers.Contains("private") Then Continue For
                    If scope <> s AndAlso IsParentScopeOf(s, scope) Then
                        SetVariable(temp, Variables(s & SCOPE_SEP & temp).Reference)
                        Return Variables(scope & SCOPE_SEP & temp).Reference
                    Else
                        Return Variables(s & SCOPE_SEP & temp).Reference
                    End If
                End If
            Next

            ' variable not found, implicit declaration?

            ' explicit mode: disallow
            If ExplicitMode OrElse explicit Then Throw New EvaluatorException("Variable " & name & " is undefined. (Explicit mode disallows implicit declaration)")

            NormalizeScope(name, scope)

            ' classes: disallow any declarations within a class scope (unless specified in the class)
            Dim tmp As String = scope

            While True
                If HasUserClass(tmp) Then
                    Dim uc As UserClass = GetUserClass(scope)
                    Dim subName As String = name
                    If subName.Contains(SCOPE_SEP) Then subName = subName.Remove(subName.IndexOf(SCOPE_SEP))
                    If Not uc.AllFields.ContainsKey(subName) Then
                        Throw New EvaluatorException(
                    "Cannot declare variable " & name & " inside class " & UserClasses(scope).Name)
                        Exit While
                    End If
                End If
                If Not tmp.Contains(SCOPE_SEP) Then Exit While
                tmp = tmp.Remove(tmp.LastIndexOf(SCOPE_SEP))
            End While

            Variables(scope & SCOPE_SEP & name) = New Variable(name, New ObjectTypes.Reference(Double.NaN), scope)
            Return Variables(scope & SCOPE_SEP & name).Value
        End Function

        ''' <summary>
        ''' Set the value of the variable with the name specified to the object referenced
        ''' </summary>
        ''' <param name="name">Name of the variable</param>
        ''' <param name="ref">Value of the variable as a Reference</param>
        Public Sub SetVariable(ByVal name As String, ByVal ref As ObjectTypes.Reference,
                               Optional ByVal scope As String = "", Optional ByVal modifiers As IEnumerable(Of String) = Nothing)
            ' set declaring scope
            If String.IsNullOrWhiteSpace(name) Then Throw New EvaluatorException("Variable name cannot be empty")
            If _reserved.Contains(name.Trim().ToLower()) Then
                Throw New EvaluatorException("Variable name ''" & name & "'' is reserved by Cantus and may not be assigned to")
            End If
            If Not IsValidIdentifier(name) Then Throw New EvaluatorException("Invalid Variable Name: " & name)

            If String.IsNullOrWhiteSpace(scope) Then scope = _Scope

            NormalizeScope(name, scope)
            Dim var As New Variable(name, ref, scope, modifiers)
            Variables(var.FullName) = var
        End Sub

        ''' <summary>
        ''' Set the value of the variable with the name specified
        ''' </summary>
        ''' <param name="name">Name of the variable</param>
        ''' <param name="value">Value of the variable as an IEvalObject</param>
        Public Sub SetVariable(ByVal name As String,
                               ByVal value As ObjectTypes.EvalObjectBase,
                               Optional ByVal scope As String = "", Optional ByVal modifiers As IEnumerable(Of String) = Nothing)
            NormalizeScope(name, scope)

            If String.IsNullOrWhiteSpace(name) Then Throw New EvaluatorException("Variable name cannot be empty")

            If _reserved.Contains(name.Trim().ToLower()) Then
                Throw New EvaluatorException("Variable name ''" & name & "'' is reserved by Cantus and may not be assigned to")
            End If
            If Not IsValidIdentifier(name) Then Throw New EvaluatorException("Invalid Variable Name: " & name)
            If String.IsNullOrEmpty(scope) Then scope = _Scope

            Dim var As New Variable(name, value, scope, modifiers)
            Variables(var.FullName) = var
        End Sub

        ''' <summary>
        ''' Set the value of the variable with the name specified
        ''' </summary>
        ''' <param name="name">Name of the variable</param>
        ''' <param name="value">Value of the variable as a system object</param>
        Public Sub SetVariable(ByVal name As String, ByVal value As Object,
                               Optional ByVal scope As String = "", Optional ByVal modifiers As IEnumerable(Of String) = Nothing)
            SetVariable(name, New ObjectTypes.Reference(value), scope, modifiers)
        End Sub

        ''' <summary>
        ''' Set the value of the default variable (i.e. this) used when no name is specified in a self-referring function call (.xxyy())
        ''' </summary>
        ''' <param name="ref">Value of the variable as a Reference</param>
        Friend Sub SetDefaultVariable(ByVal ref As ObjectTypes.Reference)
            Variables(CombineScope(Scope, DEFAULT_VAR_NAME)) = New Variable(DEFAULT_VAR_NAME, ref, _Scope)
        End Sub

        ''' <summary>
        ''' Get the value of the default variable (i.e. this) used when no name is specified in a self-referring function call (.xxyy())
        ''' </summary>
        Friend Function GetDefaultVariableRef() As ObjectTypes.Reference
            If Variables.ContainsKey(CombineScope(Scope, DEFAULT_VAR_NAME)) Then
                Return Variables(CombineScope(Scope, DEFAULT_VAR_NAME)).Reference
            Else
                ' default variable not set, we'll complain about the variable name
                Throw New EvaluatorException("Variable name ''" & DEFAULT_VAR_NAME &
                                             "'' is reserved by Cantus and may not be assigned to")
            End If
        End Function

        ''' <summary>
        ''' Returns true if the variable with the specified name is defined
        ''' </summary>
        Public Function HasVariable(ByVal name As String) As Boolean
            If name = "ans" Then Return True
            If Variables.ContainsKey(name) Then Return True
            For Each scope As String In GetAllAccessibleScopes()
                If Variables.ContainsKey(scope & SCOPE_SEP & name) AndAlso
                     (IsParentScopeOf(_Scope, scope) OrElse
                     Not Variables(scope & SCOPE_SEP & name).Modifiers.Contains("private")) Then ' do not return if private
                    Return True
                End If
            Next
            Return False
        End Function

        ''' <summary>
        ''' Returns true if the name given is a valid identifier (variable/function/class/namespace) name 
        ''' (i.e. is not empty, does not contain any of &amp;+-*/{}[]()';^$@#!%=&lt;&gt;,:|\` and does not start with a number)
        ''' </summary>
        Public Shared Function IsValidIdentifier(ByVal name As String) As Boolean
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
            Variables.Clear()
        End Sub

        ''' <summary>
        ''' Clears all variables, user functions, and previous answers on this evaluator
        ''' </summary>
        Public Sub Clear()
            Me._Scope = ROOT_NAMESPACE
            ClearVariables()
            UserFunctions.Clear()
            UserClasses.Clear()
            PrevAns.Clear()
            Imported.Clear()
            Loaded.Clear()
            Loaded.Add(ROOT_NAMESPACE)
            If IsExternalScope(Scope, ROOT_NAMESPACE) Then Me.Import(ROOT_NAMESPACE)
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
        ''' Add or set a user function
        ''' </summary>
        ''' <param name="name">The name of the function</param>
        ''' <param name="args">A list of argument names</param>
        ''' <param name="def">The function definition</param>
        Private Sub InternalDefineUserFunction(name As String, ByVal args As List(Of String),
                                   def As String, Optional modifiers As IEnumerable(Of String) = Nothing)

            Dim scope As String = _Scope
            NormalizeScope(name, scope)

            If name.Length = 0 OrElse Not IsValidIdentifier(name(0)) Then
                Throw New EvaluatorException("Error: Invalid Function Name")
            End If

            For i As Integer = 0 To args.Count - 1
                args(i) = args(i).Trim()
                If Not IsValidIdentifier(args(i)) Then Throw New EvaluatorException("Invalid Argument Name: " & args(i))
            Next

            If String.IsNullOrWhiteSpace(def) Then RemUserFunction(name)

            UserFunctions(scope & SCOPE_SEP & name) = New UserFunction(name, def, args, scope, modifiers)
        End Sub

        ''' <summary>
        ''' Add/set a user function
        ''' </summary>
        ''' <param name="fmtstr">The function in function notation e.g. name(a,b)</param>
        ''' <param name="def">The function definition</param>
        Public Function DefineUserFunction(fmtstr As String, def As String,
                                           Optional modifiers As IEnumerable(Of String) = Nothing) As Boolean

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

            Dim l As New List(Of String)(
                    fmtstr.Substring(openBracket + 1, closeBracket - openBracket - 1).Split(","c))

            If l.Count = 1 AndAlso String.IsNullOrWhiteSpace(l(0)) Then l.Clear()
            InternalDefineUserFunction(name, l, def)

            Return True
        End Function

        ''' <summary>
        ''' Remove the user function with the name
        ''' </summary>
        ''' <param name="name"></param>
        Public Sub RemUserFunction(name As String)
            If UserFunctions.ContainsKey(name) Then
                UserFunctions.Remove(name)
            Else
                name = CombineScope(_Scope, name)
                If (UserFunctions.ContainsKey(name)) Then UserFunctions.Remove(name)
            End If
        End Sub

        ''' <summary>
        ''' Return true if a user function with the given name exists
        ''' </summary>
        ''' <param name="name"></param>
        ''' <returns></returns>
        Public Function HasUserFunction(name As String) As Boolean
            If UserFunctions.ContainsKey(name) Then Return True
            name = RemoveRedundantScope(name, _Scope)
            For Each s As String In Me.GetAllAccessibleScopes()
                If UserFunctions.ContainsKey(s & SCOPE_SEP & name) Then Return True
            Next
            Return False
        End Function

        ''' <summary>
        ''' Return true if an internal or user function with the given name exists
        ''' </summary>
        ''' <param name="name"></param>
        ''' <returns></returns>
        Public Function HasFunction(name As String) As Boolean
            If HasUserFunction(name) Then Return True
            If name.StartsWith(ROOT_NAMESPACE) Then
                If name = ROOT_NAMESPACE Then Return False
                name = name.Remove(ROOT_NAMESPACE.Length).Trim({SCOPE_SEP})
            End If

            name = RemoveRedundantScope(name, ROOT_NAMESPACE)
            Dim info As MethodInfo = GetType(InternalFunctions).GetMethod(name.ToLowerInvariant(),
                    Reflection.BindingFlags.IgnoreCase Or
                    Reflection.BindingFlags.Public Or Reflection.BindingFlags.Instance Or
                    Reflection.BindingFlags.DeclaredOnly)

            Return Not info Is Nothing
        End Function

        ''' <summary>
        ''' Get the function with the name as a UserFunction object
        ''' </summary>
        ''' <param name="name"></param>
        Public Function GetUserFunction(name As String) As UserFunction
            Dim scope As String = _Scope
            name = RemoveRedundantScope(name, scope)
            If HasUserFunction(name) Then
                If UserFunctions.ContainsKey(name) Then
                    Return UserFunctions(name)
                Else
                    For Each s As String In Me.GetAllAccessibleScopes()
                        If UserFunctions.ContainsKey(s & SCOPE_SEP & name) Then
                            Return UserFunctions(s & SCOPE_SEP & name)
                        End If
                    Next
                End If
            End If
            Return Nothing
        End Function

        ''' <summary>
        ''' Execute the function with the given arguments
        ''' </summary>
        ''' <param name="name"></param>
        Public Function ExecUserFunction(name As String, args As IEnumerable(Of Object)) As Object
            Dim scope As String = _Scope
            name = RemoveRedundantScope(name, scope)
            If HasUserFunction(name) Then

                Dim uf As UserFunction = Nothing
                If UserFunctions.ContainsKey(name) Then
                    uf = UserFunctions(name)
                Else
                    For Each s As String In Me.GetAllAccessibleScopes()
                        If UserFunctions.ContainsKey(s & SCOPE_SEP & name) Then
                            uf = UserFunctions(s & SCOPE_SEP & name)
                            Exit For
                        End If
                    Next
                End If

                Dim tmpEval As Evaluator

                ' use a scoped evaluator for function call
                tmpEval = SubEvaluator(0)
                tmpEval.Scope = uf.DeclaringScope

                Dim argnames As List(Of String) = uf.Args

                If args.Count = argnames.Count Then
                    For i As Integer = 0 To args.Count - 1
                        tmpEval.SetVariable(argnames(i), args(i))
                    Next
                Else
                    Throw New EvaluatorException(name & ": " & argnames.Count & " parameter(s) expected")
                End If

                ' execute the function in a new scope
                Try
                    Return tmpEval.EvalRaw(uf.Body, noSaveAns:=True)
                Catch ex As EvaluatorException
                    ' append current function name & internal to exception 'stack trace'
                    Dim newMsg As String = ex.Message & " [In function " & name & " (" & scope & "), line " &
                            ex.Line & "]" & vbNewLine
                    If TypeOf ex Is MathException Then
                        Throw New MathException(newMsg, ex.Line)
                    ElseIf TypeOf ex Is SyntaxException
                        Throw New SyntaxException(newMsg, ex.Line)
                    Else
                        Throw New EvaluatorException(newMsg, ex.Line)
                    End If
                End Try
            Else
                Throw New EvaluatorException("User Function " & name & " is Undefined")
            End If
        End Function

        Public Function ExecInternalFunction(name As String, args As List(Of Object)) As Object
            Dim info As Reflection.MethodInfo

            name = RemoveRedundantScope(name, ROOT_NAMESPACE)
            info = GetType(InternalFunctions).GetMethod(name.ToLowerInvariant(),
                    Reflection.BindingFlags.IgnoreCase Or
                    Reflection.BindingFlags.Public Or Reflection.BindingFlags.Instance Or
                    Reflection.BindingFlags.DeclaredOnly)

            If Not info Is Nothing Then
                Dim minParamCt As Integer = 0
                Dim maxParamCt As Integer = 0
                Dim parameterMismatch As Boolean = False

                For Each paraminfo As Reflection.ParameterInfo In info.GetParameters()
                    If Not paraminfo.IsOptional Then minParamCt += 1
                    If maxParamCt >= args.Count Then
                        If paraminfo.IsOptional Then
                            args.Add(paraminfo.DefaultValue)
                        Else
                            parameterMismatch = True
                        End If

                    ElseIf Not paraminfo.ParameterType().IsAssignableFrom(args(maxParamCt).GetType()) Then
                        Dim paramTypeName As String = GetEvaluatorTypeName(paraminfo.ParameterType())

                        If paramTypeName.Contains("`") Then paramTypeName = paramTypeName.Remove(paramTypeName.IndexOf("`"))

                        Throw New EvaluatorException(name.ToLowerInvariant() & ": Parameter " & maxParamCt + 1 &
                                                    ": '" & paramTypeName & "' Type Expected")
                    End If
                    maxParamCt += 1
                Next

                If parameterMismatch OrElse args.Count > maxParamCt Then
                    Throw New EvaluatorException(name.ToLowerInvariant() & ": " &
                                                   If(minParamCt = maxParamCt, minParamCt.ToString(),
                                                   minParamCt & " to " & maxParamCt) &
                                                    " parameter(s) expected ")
                End If
                Try
                    ' execute the internal function
                    Dim execResult As Object = info.Invoke(InternalFunctions, args.ToArray())
                    If execResult Is Nothing Then ' if is null then we should return NaN
                        Return Double.NaN
                    Else
                        Return execResult
                    End If
                Catch ex As EvaluatorException
                    Throw ex
                Catch ex As Exception
                    If TypeOf ex.InnerException Is EvaluatorException Then
                        Throw New EvaluatorException("In " & name.ToLowerInvariant() & ": " & ex.InnerException.Message,
                                                             _curLine)
                    Else
                        Throw New EvaluatorException("In " & name.ToLowerInvariant() & ": Unknown error", _curLine)
                    End If
                End Try
            Else
                Throw New EvaluatorException("Function " & name.ToLowerInvariant() & " is undefined", _curLine)
            End If
        End Function

        ''' <summary>
        ''' Return true if a user class with the given name exists
        ''' </summary>
        ''' <param name="name"></param>
        ''' <returns></returns>
        Public Function HasUserClass(name As String) As Boolean
            If UserClasses.ContainsKey(name) Then Return True
            For Each s As String In Me.GetAllAccessibleScopes()
                If UserClasses.ContainsKey(s & SCOPE_SEP & name) Then Return True
            Next
            Return False
        End Function

        ''' <summary>
        ''' Returns the UserClass with the name specified
        ''' </summary>
        ''' <param name="name"></param>
        ''' <returns></returns>
        Public Function GetUserClass(name As String) As UserClass
            If UserClasses.ContainsKey(name) Then Return UserClasses(name)
            For Each s As String In Me.GetAllAccessibleScopes()
                If UserClasses.ContainsKey(s & SCOPE_SEP & name) Then Return UserClasses(s & SCOPE_SEP & name)
            Next
            Return Nothing
        End Function

        ''' <summary>
        ''' Define a UserClass with the name specified and the definition specified, 
        ''' inheriting from the classes specified
        ''' </summary>
        Public Sub DefineUserClass(name As String, def As String,
                                   Optional inherit As IEnumerable(Of String) = Nothing,
                                   Optional modifiers As IEnumerable(Of String) = Nothing)
            name = name.Trim()
            If Not IsValidIdentifier(name) Then Throw New SyntaxException("''" & name & "'' is not a valid class name.")

            Dim key As String = CombineScope(Me.Scope, name)

            Try
                If Me.UserClasses.ContainsKey(key) Then Me.UserClasses(key).Dispose()

                Dim baseClasses As New List(Of String)

                If Not inherit Is Nothing Then
                    For i As Integer = 0 To inherit.Count() - 1
                        Dim inh As String = inherit(i).Trim()
                        If inh = name Then Throw New EvaluatorException(inh & " may not inherit itself")
                        If Not HasUserClass(inh) Then Throw New EvaluatorException(inh & " is not a valid base class name")
                        Dim baseClass As UserClass = GetUserClass(inh)
                        If baseClass.AllParentClasses.Contains(key) Then Throw New EvaluatorException(inh &
                                                              ": circular inheritance detected")
                        baseClasses.Add(baseClass.FullName)
                    Next
                End If

                Dim uc As UserClass = New UserClass(name, def, Me, modifiers, baseClasses)
                Me.UserClasses(key) = uc
            Catch ex As Exception
                If Me.UserClasses.ContainsKey(key) Then Me.UserClasses.Remove(key)
                Throw ex
            End Try
        End Sub

        ''' <summary>
        ''' Gets the base scope of the current scope: for cantus.foo.bar
        ''' that would be cantus
        ''' </summary>
        Friend Shared Function GetScopeBase(scope As String) As String
            If scope.Contains(SCOPE_SEP) Then
                Return scope.Remove(scope.IndexOf(SCOPE_SEP)).Trim()
            Else
                Return scope.Trim()
            End If
        End Function

        ''' <summary>
        ''' If scope1 is a parent scope of scope2, returns true
        ''' that would be cantus
        ''' </summary>
        Friend Shared Function IsParentScopeOf(scope1 As String, scope2 As String, Optional baseScope As String = "") As Boolean
            scope1 = RemoveRedundantScope(scope1, baseScope)
            scope2 = RemoveRedundantScope(scope2, baseScope)
            Return scope2.StartsWith(scope1 & SCOPE_SEP) OrElse scope1 = scope2
        End Function

        ''' <summary>
        ''' Get a list of parent scopes of the scope specified, from closest to furthest
        ''' that would be cantus
        ''' </summary>
        Friend Shared Function GetParentScopes(scope As String) As List(Of String)
            Dim scopes As New List(Of String)()

            While scope.Contains(SCOPE_SEP)
                scope = scope.Remove(scope.LastIndexOf(SCOPE_SEP))
                scopes.Add(scope)
            End While

            Return scopes
        End Function

        ''' <summary>
        ''' Get a list of scopes accessible from the current scope
        ''' </summary>
        ''' <returns></returns>
        Private Function GetAllAccessibleScopes() As List(Of String)
            Dim checkScopeLst As New List(Of String)
            checkScopeLst.Add(Scope)
            checkScopeLst.AddRange(GetParentScopes(Scope).ToArray())
            checkScopeLst.AddRange(Imported.ToArray())
            Return checkScopeLst
        End Function

        ''' <summary>
        ''' Checks if the first scope is 'external' (that is, 
        ''' does not have the same base scope) in relation to the second scope.
        ''' If the second scope is not specified, tests with the scope of this evaluator.
        ''' </summary>
        Friend Function IsExternalScope(scope1 As String, Optional scope2 As String = "") As Boolean
            If scope2 = "" Then scope2 = _Scope
            Return GetScopeBase(scope1) <> GetScopeBase(scope2)
        End Function

        Private _visClass As HashSet(Of UserClass)
        ''' <summary>
        ''' Serialize a user class in a way that preserves inheritance
        ''' </summary>
        ''' <returns></returns>
        Private Function SerializeRelatedClasses(uc As UserClass) As StringBuilder
            Dim result As New StringBuilder

            _visClass.Add(uc)

            For Each b As String In uc.BaseClasses
                If Not UserClasses.ContainsKey(b) Then Continue For
                Dim baseUC As UserClass = UserClasses(b)
                If _visClass.Contains(baseUC) Then Continue For
                result.Append(SerializeRelatedClasses(baseUC))
            Next

            result.AppendLine(uc.ToString(Scope))
            Return result
        End Function

        ''' <summary>
        ''' Convert the evaluator's user functions, variables, and configuration into a
        '''  script that can be ran again for storage
        ''' </summary>
        ''' <returns></returns>
        Public Function ToScript() As String
            Dim serialized As New StringBuilder()
            serialized.AppendLine("# Cantus " & Application.ProductVersion &
                                  " auto-generated initialization script")
            serialized.AppendLine("# Use caution when modifying manually").Append(vbNewLine)
            serialized.AppendLine("# Modes")

            serialized.Append("_Output(").Append(ControlChars.Quote).Append(OutputFormat.ToString()).Append(ControlChars.Quote).Append(")").
                Append(vbNewLine)
            serialized.Append("_AngleRepr(").Append(ControlChars.Quote).Append(AngleMode.ToString()).Append(ControlChars.Quote).
                Append(")").Append(vbNewLine)
            serialized.Append("_SpacesPerTab(").Append(SpacesPerTab.ToString()).Append(")").Append(vbNewLine)

            serialized.AppendLine().AppendLine("# Class Definitions")
            _visClass = New HashSet(Of UserClass)()

            For Each uc As KeyValuePair(Of String, UserClass) In UserClasses
                ' Do not output if it is from an external scope -- it is already saved there somewhere
                If Not _visClass.Contains(uc.Value) AndAlso
                                          Not IsExternalScope(uc.Value.DeclaringScope) AndAlso Not uc.Value.Modifiers.Contains("internal") Then
                    serialized.Append(SerializeRelatedClasses(uc.Value))
                End If
            Next

            serialized.AppendLine().AppendLine("# Function Definitions")
            For Each func As KeyValuePair(Of String, UserFunction) In UserFunctions
                ' Do not output if it is from an external scope -- it is already saved there somewhere.
                ' Also do not save 'internal' variables and functions - they are already saved with classes
                If Not IsExternalScope(func.Value.DeclaringScope) AndAlso Not func.Value.Modifiers.Contains("internal") Then
                    serialized.AppendLine(func.Value.ToString(_Scope))
                End If
            Next

            serialized.AppendLine().AppendLine("# Variable Definitions")
            '
            For Each var As KeyValuePair(Of String, Variable) In Variables.ToArray()
                Dim def As ObjectTypes.EvalObjectBase = var.Value.Reference.ResolveObj()

                If Not def Is Nothing AndAlso (
                    Not TypeOf def Is ObjectTypes.Number OrElse Not Double.IsNaN(CDbl(def.GetValue()))) AndAlso
                     Not TypeOf def Is ObjectTypes.Reference AndAlso Not var.Value.Modifiers.Contains("internal") AndAlso
                    Not var.Key = DEFAULT_VAR_NAME AndAlso
                    Not (IsExternalScope(var.Value.DeclaringScope)) Then

                    Dim defs As String

                    Dim fullName As String = RemoveRedundantScope(var.Key, _Scope)

                    If TypeOf def Is ClassInstance Then ' special treatment for class instances
                        Dim ci As ClassInstance = DirectCast(def, ClassInstance)
                        Dim sb As New StringBuilder()
                        sb.Append(ci.UserClass.Name)
                        sb.AppendLine("()")
                        For Each f As String In ci.Fields.Keys
                            sb.Append(fullName).Append(SCOPE_SEP).Append(f).Append(" = ").AppendLine(ci.Fields(f).ToString())
                        Next
                        defs = sb.ToString()
                    Else
                        defs = def.ToString()
                    End If

                    serialized.Append(fullName).Append("=").AppendLine(defs)
                End If
            Next


            If ExplicitMode Then
                serialized.AppendLine().AppendLine("# Explicit mode switch")
                serialized.Append("_explicit(").Append(ExplicitMode.ToString()).Append(")").Append(vbNewLine)
            End If

            serialized.AppendLine().AppendLine("# End of Cantus auto-generated initialization script. DO NOT modify this comment.")
            serialized.AppendLine("# You may write additional initialization code below this line.")

            Return serialized.ToString()
        End Function

        ''' <summary>
        ''' Given a system type, returns the name of the equivalent type used inside Cantus
        ''' </summary>
        Public Shared Function GetEvaluatorTypeName(type As Type) As String
            Select Case type
                Case GetType(String)
                    Return "Text"
                Case GetType(Double), GetType(BigDecimal)
                    Return "Number"
                Case GetType(SortedDictionary(Of ObjectTypes.Reference, ObjectTypes.Reference))
                    Return "Set"
                Case GetType(Dictionary(Of ObjectTypes.Reference, ObjectTypes.Reference))
                    Return "HashSet"
                Case GetType(List(Of ObjectTypes.Reference))
                    Return "Matrix"
                Case GetType(LinkedList(Of ObjectTypes.Reference))
                    Return "LinkedList"
                Case GetType(ObjectTypes.Reference())
                    Return "Tuple"
                Case GetType(ObjectTypes.Reference)
                    Return "Reference"
                Case GetType(ObjectTypes.Lambda)
                    Return "Function"

                Case GetType(ICollection(Of ObjectTypes.Reference))
                    Return "(Matrix, Set, HashSet, Tuple, LinkedList)"
                Case GetType(IEnumerable(Of ObjectTypes.Reference)), GetType(IList(Of ObjectTypes.Reference))
                    Return "(Matrix, LinkedList)"
                Case GetType(IDictionary(Of ObjectTypes.Reference, ObjectTypes.Reference))
                    Return "(Set, HashSet)"

                Case GetType(Object)
                    Return "(Variable)"
                Case Else
                    Return type.Name
            End Select
        End Function

        ''' <summary>
        ''' Move this evaluator down to a subscope of the current scope. 
        ''' If no subscope is specified, gets an anonymous subscope name.
        ''' </summary>
        Public Sub SubScope(Optional subScopeName As String = "")
            If subScopeName = "" Then subScopeName = GetAnonymousSubscope()
            _Scope &= SCOPE_SEP & subScopeName
        End Sub

        ''' <summary>
        ''' Move this evaluator up to the parent scope
        ''' </summary>
        Public Sub ParentScope()
            If _Scope.Contains(SCOPE_SEP) Then _Scope = _Scope.Remove(_Scope.LastIndexOf(SCOPE_SEP))
        End Sub

        ''' <summary>
        ''' Get an anonymous subscope name
        ''' </summary>
        Private Function GetAnonymousSubscope() As String
            _anonymousScopeID += 1

            If _anonymousScopeID = Int32.MaxValue Then _anonymousScopeID = 0
            Return "__anonymous_" & _anonymousScopeID - 1
        End Function

        ''' <summary>
        ''' Create a copy of the evaluator containing the same user functions, variables,
        ''' and functions starting at the current line
        ''' </summary>
        ''' <returns></returns>
        Public Function SubEvaluator(Optional ByVal lineNumber As Integer = -1, Optional ByVal subScopeName As String = "") As Evaluator
            If lineNumber < 0 Then lineNumber = _curLine

            Dim varsCopy As New Dictionary(Of String, Variable)

            Dim tmp As KeyValuePair(Of String, Variable)() = Me.Variables.ToArray()
            For i As Integer = 0 To tmp.Count - 1
                If TypeOf tmp(i).Value.Reference.Resolve() Is Double AndAlso
                    Double.IsNaN(CDbl(tmp(i).Value.Reference.Resolve())) Then Continue For ' skip undefined vars
                varsCopy(tmp(i).Key) = tmp(i).Value
            Next

            Dim funcCopy As New Dictionary(Of String, UserFunction)(Me.UserFunctions)

            ' if no scope name is given then give it the next anonymous scope
            If subScopeName = "" Then subScopeName = GetAnonymousSubscope()

            Dim res As New Evaluator(Me.OutputFormat, Me.AngleMode,
                                 Me.SpacesPerTab, Me.ExplicitMode, Me.PrevAns, varsCopy, funcCopy,
                                          lineNumber, _Scope & SCOPE_SEP & subScopeName, False)
            For Each import As String In GetAllAccessibleScopes()
                res.Import(import)
            Next

            Return res
        End Function

        ''' <summary>
        ''' Create an identical copy of the evaluator containing COPIES of the same user functions, variables, and functions
        ''' </summary>
        ''' <returns></returns>
        Public Function DeepCopy(Optional scopeName As String = "") As Evaluator
            Dim varsCopy As New Dictionary(Of String, Variable)
            Try
                For Each k As KeyValuePair(Of String, Variable) In
                    Variables.ToArray()
                    If TypeOf k.Value.Reference.Resolve() Is Double AndAlso
                    Double.IsNaN(CDbl(k.Value.Reference.Resolve())) Then Continue For ' skip undefined vars
                    varsCopy.Add(k.Key, New Variable(k.Value.Name, DirectCast(k.Value.
                                                     Reference.GetDeepCopy(),
                                 ObjectTypes.Reference), k.Value.DeclaringScope))
                Next
            Catch
            End Try

            Dim funcCopy As New Dictionary(Of String, UserFunction)(Me.UserFunctions)

            Dim res As New Evaluator(Me.OutputFormat, Me.AngleMode, Me.SpacesPerTab, Me.ExplicitMode,
                                 Me.PrevAns, varsCopy, funcCopy, Me._baseLine, If(scopeName = "", Me._Scope, scopeName), False)
            For Each import As String In GetAllAccessibleScopes()
                res.Import(import)
            Next
            Return res
        End Function

        ''' <summary>
        ''' Stop all threads, optionally sparing the thread marked as haveMercy
        ''' </summary>
        Public Sub StopAll(Optional haveMercy As Integer = -1)
            Me.StatementRegistar.StopAll()
            For i As Integer = 0 To Me._threads.Count - 1
                If Me._threads(i).ManagedThreadId = haveMercy Then Continue For
                Try
                    Me._threads(i).Abort()
                    Me._threads.RemoveAt(i)
                    i -= 1
                Catch
                End Try
            Next
        End Sub

        ''' <summary>
        ''' Cleans up threads spawned by this evaluator. Unneeded if no threads spawned.
        ''' </summary>
        Friend Sub Dispose() Implements IDisposable.Dispose
            StopAll()
            Me.StatementRegistar.Dispose()
        End Sub
    End Class
#End Region

End Namespace
