Imports System.Collections.ObjectModel
Imports System.Linq.Expressions
Imports System.Numerics
Imports System.Reflection
Imports System.Reflection.Emit
Imports System.Text
Imports Cantus.Evaluator.CommonTypes
Imports Cantus.Evaluator.ObjectTypes
Imports Cantus.Evaluator.Evaluator
Imports Cantus.Evaluator.Exceptions

Namespace Evaluator
    ' To define a new type for use with the evaluator, add a class implementing IEvalType in this namespace, 
    '    change the StrDetectType function and add converter functions as necessary

    Public Class ObjectTypes
        ' a pre-computed list of valid object types
        Private Shared _types As IEnumerable(Of Type) = From t As Type In GetType(ObjectTypes).GetNestedTypes(BindingFlags.Public)
                                                        Select t Where (Not t.IsInterface AndAlso Not t.IsAbstract AndAlso
                                                        t.IsSubclassOf(GetType(EvalObjectBase)))
        ''' <summary>
        ''' Automatically converts the object to a IEvalObject
        ''' </summary>
        ''' <param name="obj">The string</param>
        ''' <param name="identifierAsText">If true, parses strings into Text's instead of Identifiers</param>
        ''' <returns></returns>
        Public Shared Function DetectType(obj As Object, Optional identifierAsText As Boolean = False) As EvalObjectBase
            If obj Is Nothing Then Return Nothing ' null

            If obj.GetType().ToString().StartsWith("Cantus.Evaluator.ObjectTypes") AndAlso
                Not obj.GetType().ToString().EndsWith("[]") Then
                Return DirectCast(obj, EvalObjectBase)
            End If

            For Each t As Type In _types
                ' identifiers & text are both strings, so we need to look at the identifierAsText flag
                If identifierAsText AndAlso t = GetType(Identifier) Then Continue For
                If Not identifierAsText AndAlso t = GetType(Text) Then Continue For

                If CBool(t.GetMethod("IsType").Invoke(t, {obj})) Then
                    Return DirectCast(Activator.CreateInstance(t, {obj}), EvalObjectBase)
                End If
            Next

            Throw New EvaluatorException("Type " & obj.GetType().Name & " is not understood by the evaluator.")
        End Function

        ''' <summary>
        ''' Automatically parses the string into an IEvalObject
        ''' </summary>
        ''' <param name="str">The string</param>
        ''' <param name="identifierAsText">If true, parses remaining into Text's instead of Identifiers</param>
        ''' <param name="primitiveOnly">If true, only checks number, boolean, text types</param>
        ''' <returns></returns>
        Public Shared Function StrDetectType(str As String, Optional eval As Evaluator = Nothing,
                                                 Optional identifierAsText As Boolean = False,
                                                      Optional primitiveOnly As Boolean = True) As EvalObjectBase
            If Number.StrIsType(str) Then
                Return New Number(str)
            ElseIf [Boolean].StrIsType(str) Then
                Return New [Boolean](str)
            Else
                If Not primitiveOnly Then
                    For Each t As Type In _types
                        ' identifiers & text are both strings, so we need to look at the identifierAsText flag
                        If identifierAsText AndAlso t = GetType(Identifier) Then Continue For
                        If Not identifierAsText AndAlso t = GetType(Text) Then Continue For

                        If CBool(t.GetMethod("StrIsType").Invoke(t, {str})) Then
                            Return DirectCast(Activator.CreateInstance(t, {CObj(str)}), EvalObjectBase)
                        End If
                    Next
                Else
                    ' if none work then try text
                    If identifierAsText AndAlso Text.StrIsType(str) AndAlso
                        str.StartsWith(ControlChars.Quote) AndAlso str.EndsWith(ControlChars.Quote) Then
                        Return New Text(str)
                    ElseIf Not identifierAsText Then 'AndAlso ObjectTypes.Identifier.StrIsType(str) Then
                        Return New Identifier(str)
                    End If
                End If

                Return Nothing
            End If
        End Function

        Public MustInherit Class EvalObjectBase
            Inherits Object
            Implements IEquatable(Of EvalObjectBase)
            Implements IComparable
            Implements IComparable(Of EvalObjectBase)

            ''' <summary>
            ''' Get the system type value represented by this object. 
            ''' If the object represents no specific system type, then this should return the object itself.
            ''' </summary>
            Public MustOverride Function GetValue() As Object

            ''' <summary>
            ''' Set the value represented by this object.
            ''' If the object represents no specific system type, this should be used to copy from another object of the same type
            ''' </summary>
            Public MustOverride Sub SetValue(obj As Object)

            ''' <summary>
            ''' Convert this object to a human readable string
            ''' </summary>
            Public Overrides Function ToString() As String
                Return GetValue().ToString()
            End Function

            ''' <summary>
            ''' Function used to detect if an object is of or is represented by the type. Used in DetectType()
            ''' </summary>
            Public Shared Function IsType(obj As Object) As Boolean
                Return False
            End Function

            ''' <summary>
            ''' Function used to detect if a string is of or is represented by the type. Used in StrDetectType()
            ''' </summary>
            Public Shared Function StrIsType(str As String) As Boolean
                Return False
            End Function

            ''' <summary>
            ''' Generate a (usually) unique integer identifying the object
            ''' </summary>
            Public MustOverride Overrides Function GetHashCode() As Integer

            ''' <summary>
            ''' Create a brand new copy of this object, so that the new copy will not affect the old
            ''' </summary>
            ''' <returns></returns>
            Public Overridable Function GetDeepCopy() As EvalObjectBase
                Return DeepCopy()
            End Function

            ''' <summary>
            ''' Create a brand new copy of this object, so that the new copy will not affect the old
            ''' </summary>
            Protected MustOverride Function DeepCopy() As EvalObjectBase

            Public MustOverride Shadows Function Equals(other As EvalObjectBase) As Boolean Implements IEquatable(Of EvalObjectBase).Equals

            Public Overridable Function CompareTo(other As Object) As Integer Implements IComparable.CompareTo
                Return CType(GetValue(), IComparable).CompareTo(other)
            End Function

            Public Overridable Function CompareTo(other As EvalObjectBase) As Integer Implements IComparable(Of EvalObjectBase).CompareTo
                Return CType(GetValue(), IComparable).CompareTo(other.GetValue())
            End Function
        End Class

        Public NotInheritable Class Number : Inherits EvalObjectBase
            Private _value As BigDecimal

            Public Overrides Function GetValue() As Object
                Return CDbl(_value)
            End Function

            Public Function BigDecValue() As BigDecimal
                Return _value
            End Function

            Public Overrides Sub SetValue(obj As Object)
                If TypeOf obj Is BigDecimal Then
                    _value = CType(obj, BigDecimal)
                Else
                    _value = CDbl(obj)
                End If
            End Sub

            Public Shared Shadows Function IsType(obj As Object) As Boolean
                Return TypeOf obj Is Number Or TypeOf obj Is Double Or TypeOf obj Is Integer Or TypeOf obj Is BigDecimal
            End Function

            Public Shared Shadows Function StrIsType(str As String) As Boolean
                str = str.ToLowerInvariant()
                Return str = "null" OrElse str = "undefined" OrElse
                    (Double.TryParse(str.Trim(), Nothing) AndAlso Not str.Contains("e"))
            End Function

            Public Overrides Function Equals(other As EvalObjectBase) As Boolean
                If IsType(other) Then
                    Return CType(other, Number).BigDecValue() = _value
                Else
                    Return False
                End If
            End Function

            Public Overrides Function ToString() As String
                Return _value.ToString()
            End Function

            Protected Overrides Function DeepCopy() As EvalObjectBase
                Me._value.Normalize()
                Return New Number(New BigDecimal(Me._value.Mantissa, Me._value.Exponent, Me._value.IsUndefined))
            End Function

            Public Sub New(value As Double)
                Me._value = value
            End Sub

            Public Sub New(value As BigDecimal)
                Me._value = value
            End Sub

            Public Sub New(str As String)
                str = str.ToLowerInvariant()
                If str.ToLower() = "undefined" Then
                    Me._value = BigDecimal.Undefined
                Else
                    Try
                        Me._value = Double.Parse(str.Trim())
                    Catch
                        Me._value = BigDecimal.Undefined
                    End Try
                End If
            End Sub

            Public Overrides Function GetHashCode() As Integer
                Return _value.GetHashCode()
            End Function
        End Class

        Public NotInheritable Class Complex : Inherits EvalObjectBase
            Private _value As Numerics.Complex
            Public Overrides Function GetValue() As Object
                Return _value
            End Function
            Public Property Real() As Double
                Get
                    Return _value.Real
                End Get
                Set(value As Double)
                    Me._value = New Numerics.Complex(value, Me.Imag)
                End Set
            End Property
            Public Property Imag() As Double
                Get
                    Return _value.Imaginary
                End Get
                Set(value As Double)
                    Me._value = New Numerics.Complex(Me.Real, value)
                End Set
            End Property
            Public Overrides Sub SetValue(obj As Object)
                If Matrix.IsType(obj) Then
                    Dim lst As List(Of Reference) = CType(obj, List(Of Reference))
                    Me._value = New Numerics.Complex(CDbl(lst(0).GetValue()), CDbl(lst(1).GetValue()))
                ElseIf IsType(obj) Then
                    Me._value = CType(obj, Numerics.Complex)
                ElseIf Number.IsType(obj)
                    Me._value = CDbl(obj)
                End If
            End Sub

            Protected Overrides Function DeepCopy() As EvalObjectBase
                Return New Complex(New Numerics.Complex(Real, Imag))
            End Function

            Public Shared Shadows Function IsType(obj As Object) As Boolean
                Return TypeOf obj Is Complex Or TypeOf obj Is Numerics.Complex
            End Function

            Public Shared Shadows Function StrIsType(str As String) As Boolean
                str = str.Trim()
                Return str.EndsWith(")") AndAlso str.StartsWith("(") AndAlso str.Contains("i") AndAlso
                    (str.Contains("+") OrElse str.Contains("-"))
            End Function

            Public Overrides Function ToString() As String
                Return String.Format("({0} {1} {2}i)", _value.Real, If(_value.Imaginary >= 0, "+", "-"),
                                    Math.Abs(_value.Imaginary))
            End Function

            Public Overrides Function Equals(other As EvalObjectBase) As Boolean
                If IsType(other) Then
                    Return CType(other.GetValue(), Numerics.Complex) = _value
                Else
                    Return False
                End If
            End Function

            Public Sub New(real As Double, Optional imag As Double = 0)
                Me._value = New Numerics.Complex(real, imag)
            End Sub
            Public Sub New(value As Numerics.Complex)
                Me._value = value
            End Sub
            Public Sub New(str As String, eval As Evaluator)
                If StrIsType(str) Then
                    str = str.Trim().Remove(str.Length - 1).Substring(1).Trim()
                    Dim split As String() = str.Split({" "c}, StringSplitOptions.RemoveEmptyEntries)

                    If split.Length <> 3 Then Throw New EvaluatorException("Invalid complex format")

                    Dim neg As Integer = If(split(1).Contains("-"), -1, 1)

                    Me._value = New Numerics.Complex(CDbl(CType(eval.EvalExprRaw(split(0), True), BigDecimal)),
                                                CDbl(neg * New Number(split(2).Replace("i", "").Trim()).BigDecValue()))
                End If
            End Sub

            Public Overrides Function GetHashCode() As Integer
                Return _value.GetHashCode()
            End Function
        End Class

        Public NotInheritable Class [Boolean] : Inherits EvalObjectBase
            Private _value As Boolean
            Public Overrides Function GetValue() As Object
                Return _value
            End Function
            Public Overrides Sub SetValue(obj As Object)
                _value = CBool(obj)
            End Sub
            Public Shared Shadows Function IsType(obj As Object) As Boolean
                Return TypeOf obj Is [Boolean] Or TypeOf obj Is Boolean
            End Function
            Public Shared Shadows Function StrIsType(str As String) As Boolean
                str = str.Trim().ToLower()
                Return str = "true" OrElse str = "false"
            End Function

            Public Overrides Function Equals(other As EvalObjectBase) As Boolean
                If IsType(other) Then
                    Return CBool(other.GetValue()) = _value
                Else
                    Return False
                End If
            End Function

            Protected Overrides Function DeepCopy() As EvalObjectBase
                Return New ObjectTypes.Boolean(Me._value)
            End Function

            Public Sub New(value As Boolean)
                Me._value = value
            End Sub
            Public Sub New(str As String)
                Me._value = Boolean.Parse(str.Trim())
            End Sub

            Public Overrides Function GetHashCode() As Integer
                Return _value.GetHashCode()
            End Function
        End Class

        ''' <summary>
        ''' A piece of text
        ''' </summary>
        Public NotInheritable Class Text : Inherits EvalObjectBase
            Private _value As String
            Public Overrides Function GetValue() As Object
                Return _value
            End Function
            Public Overrides Sub SetValue(obj As Object)
                _value = CStr(obj)
            End Sub
            Public Shared Shadows Function IsType(obj As Object) As Boolean
                Return TypeOf obj Is String Or TypeOf obj Is Text
            End Function
            Public Shared Shadows Function StrIsType(str As String) As Boolean
                Return True
            End Function

            ''' <summary>
            ''' If the index is within bounds then set the character. Otherwise append the character to the end.
            ''' </summary>
            Private Sub SetOrAppend(ByRef sb As StringBuilder, chr As Char, Optional idx As Integer = -1)
                If idx < sb.Length AndAlso idx >= 0 Then
                    sb(idx) = chr
                Else
                    sb.Append(chr)
                End If
            End Sub

            ''' <summary>
            ''' Resolve all escape sequences in this Text object
            ''' </summary>
            ''' <param name="raw">If true, only escapes \ \' and \"</param>
            Public Function Escape(Optional raw As Boolean = False) As Text
                Dim newstr As New StringBuilder
                Dim escNxt As Boolean = False
                Dim idx As Integer = 0

                For i As Integer = 0 To _value.Length - 1
                    If escNxt Then
                        If raw Then
                            Select Case Char.ToLowerInvariant(_value(i))
                                Case ControlChars.Quote, "'"c
                                    SetOrAppend(newstr, _value(i), idx)
                                Case Else
                                    SetOrAppend(newstr, "\"c, idx)
                                    idx += 1
                                    SetOrAppend(newstr, _value(i), idx)
                            End Select
                        Else
                            ' c-like escape sequence
                            Select Case Char.ToLowerInvariant(_value(i))
                                Case "a"c
                                    SetOrAppend(newstr, ChrW(7), idx)
                                Case "b"c
                                    If idx > 1 Then idx -= 3 ' non-destructive backspace
                                Case "f"c
                                    SetOrAppend(newstr, ChrW(12), idx)
                                Case "n"c
                                    SetOrAppend(newstr, ControlChars.Lf, idx)
                                Case "r"c
                                    SetOrAppend(newstr, ControlChars.Cr, idx)
                                Case "t"c
                                    SetOrAppend(newstr, ControlChars.Tab, idx)
                                Case "v"c
                                    SetOrAppend(newstr, ControlChars.VerticalTab, idx)
                                Case "x"c
                                    i += 1
                                    Dim charId As String = "&H"
                                    While i < _value.Length AndAlso (
                                        (AscW(_value(i)) >= AscW("0"c) AndAlso AscW(_value(i)) <= AscW("9"c)) OrElse
                                        (AscW(_value(i)) >= AscW("a"c) AndAlso AscW(_value(i)) <= AscW("f"c)) OrElse
                                        (AscW(_value(i)) >= AscW("A"c) AndAlso AscW(_value(i)) <= AscW("F"c))) AndAlso
                                        charId.Length < 7
                                        charId &= Char.ToUpperInvariant(_value(i))
                                        i += 1
                                    End While
                                    i -= 1
                                    SetOrAppend(newstr, ChrW(CInt(charId)), idx)
                                Case "0"c To "9"c
                                    Dim charId As String = "&O"
                                    While i < _value.Length AndAlso charId.Length < 5 AndAlso
                                    (AscW(_value(i)) >= AscW("0"c) AndAlso AscW(_value(i)) <= AscW("7"c))
                                        charId &= Char.ToUpperInvariant(_value(i))
                                        i += 1
                                    End While
                                    i -= 1
                                    SetOrAppend(newstr, ChrW(CInt(charId)), idx)
                                Case "d"c
                                    i += 1
                                    Dim charId As Integer = 0
                                    While i < _value.Length AndAlso
                                    AscW(_value(i)) >= AscW("0"c) AndAlso AscW(_value(i)) <= AscW("9"c)
                                        charId = charId * 10 + AscW(_value(i)) - AscW("0"c)
                                        i += 1
                                    End While
                                    i -= 1
                                    SetOrAppend(newstr, ChrW(charId), idx)
                                Case "u"c
                                    i += 1
                                    Dim charId As Integer = 0
                                    While i < _value.Length AndAlso
                                    AscW(_value(i)) >= AscW("0"c) AndAlso AscW(_value(i)) <= AscW("9"c)
                                        charId = charId * 10 + AscW(_value(i)) - AscW("0"c)
                                        i += 1
                                    End While
                                    i -= 1
                                    SetOrAppend(newstr, Char.ConvertFromUtf32(charId)(0), idx)
                                Case "\"c, ControlChars.Quote, "'"c, "?"c
                                    SetOrAppend(newstr, _value(i), idx)
                                Case Else
                                    Throw New EvaluatorException("Invalid escape sequence")
                            End Select
                        End If
                        escNxt = False
                    ElseIf _value(i) = "\" Then
                        If i = _value.Length - 1 Then SetOrAppend(newstr, "\"c, idx) ' do not escape if this is the last character
                        escNxt = True
                    Else
                        SetOrAppend(newstr, _value(i), idx)
                    End If
                    idx += 1
                Next
                Me._value = newstr.ToString()
                Return Me
            End Function

            Public Overrides Function ToString() As String
                Return ControlChars.Quote & _value & ControlChars.Quote
            End Function

            Public Overrides Function Equals(other As EvalObjectBase) As Boolean
                If IsType(other) Then
                    Return CStr(other.GetValue()) = _value
                Else
                    Return False
                End If
            End Function

            Protected Overrides Function DeepCopy() As EvalObjectBase
                Return New ObjectTypes.Text(Me._value)
            End Function


            Public Sub New(value As String)
                If value.Length > 1 AndAlso (value.StartsWith(ControlChars.Quote) AndAlso
                        value.EndsWith(ControlChars.Quote) OrElse value.StartsWith("'") AndAlso value.EndsWith("'")) Then
                    Me._value = value.Substring(1, value.Length - 2)
                Else
                    Me._value = value
                End If
            End Sub

            Public Overrides Function GetHashCode() As Integer
                Return _value.GetHashCode()
            End Function
        End Class

        ''' <summary>
        ''' A piece of text that represents a function or variable
        ''' </summary>
        Public NotInheritable Class Identifier : Inherits EvalObjectBase
            Private _value As String

            Public Overrides Function GetValue() As Object
                Return _value
            End Function

            Public Overrides Sub SetValue(obj As Object)
                _value = CStr(obj)
            End Sub

            Public Shared Shadows Function IsType(obj As Object) As Boolean
                Return TypeOf obj Is String Or TypeOf obj Is Identifier
            End Function

            Public Shared Shadows Function StrIsType(str As String) As Boolean
                If String.IsNullOrWhiteSpace(str.Trim()) Then Return False ' check if empty
                If Char.IsDigit(str(0)) Then Return False ' check if starts with number

                Dim disallowed As Char() = "&+-*/{}[]()';^$@#!%=<>,:|\`~ ".ToCharArray()
                For Each c As Char In str
                    If disallowed.Contains(c) Then Return False
                Next
                Return True
            End Function

            Protected Overrides Function DeepCopy() As EvalObjectBase
                Return New ObjectTypes.Identifier(Me._value)
            End Function

            Public Overrides Function Equals(other As EvalObjectBase) As Boolean
                If IsType(other) Then
                    Return CStr(other.GetValue()) = _value
                Else
                    Return False
                End If
            End Function

            Public Sub New(value As String)
                Me._value = value.Trim()
            End Sub

            Public Overrides Function GetHashCode() As Integer
                Return _value.GetHashCode()
            End Function
        End Class

        ''' <summary>
        ''' A single class that is able to represent both absolute points in time and time spans
        ''' </summary>
        Public NotInheritable Class DateTime : Inherits EvalObjectBase
            Private _value As System.TimeSpan
            ''' <summary>
            ''' The date from which absolute datetimes are calculated
            ''' </summary>
            ''' <returns></returns>
            Public Shared ReadOnly Property BASE_DATE As System.DateTime = System.DateTime.MinValue
            ''' <summary>
            ''' The length of time in days after which absolute datetimes are returned instead of timespans
            ''' </summary>
            ''' <returns></returns>
            Public Shared ReadOnly Property TIMESPAN_DIVIDER As Integer = 36500

            Public Overrides Function GetValue() As Object
                If _value.Days > TIMESPAN_DIVIDER Then
                    Return BASE_DATE.Add(_value)
                End If
                Return _value
            End Function

            Public Overrides Sub SetValue(obj As Object)
                If TypeOf obj Is DateTime Then
                    _value = CType(obj, System.DateTime).Subtract(BASE_DATE)
                ElseIf TypeOf obj Is TimeSpan
                    _value = CType(obj, TimeSpan)
                End If
            End Sub

            Public Overrides Function GetDeepCopy() As EvalObjectBase
                If TypeOf Me.GetValue() Is DateTime Then
                    Return New DateTime(New System.DateTime(CDate(Me.GetValue()).Ticks))
                Else
                    Return New DateTime(New System.TimeSpan(CType(Me.GetValue(), TimeSpan).Ticks))
                End If
            End Function

            Public Shared Shadows Function IsType(obj As Object) As Boolean
                Return TypeOf obj Is TimeSpan Or TypeOf obj Is System.DateTime OrElse TypeOf obj Is Date Or TypeOf obj Is DateTime
            End Function

            Public Shared Shadows Function StrIsType(str As String) As Boolean
                Return System.TimeSpan.TryParse(str.Trim(), Nothing) Or System.DateTime.TryParse(str.Trim(), Nothing)
            End Function

            Public Overrides Function ToString() As String
                If _value.Days > TIMESPAN_DIVIDER Then
                    Return BASE_DATE.Add(_value).ToString()
                End If
                Return _value.ToString()
            End Function

            Protected Overrides Function DeepCopy() As EvalObjectBase
                Return New ObjectTypes.DateTime(Me._value)
            End Function

            Public Overrides Function Equals(other As EvalObjectBase) As Boolean
                If IsType(other) Then
                    Dim val As Object = other.GetValue()
                    If TypeOf val Is TimeSpan Then
                        Return CType(val, TimeSpan) = _value
                    ElseIf TypeOf val Is DateTime Then
                        Return CDate(val) = BASE_DATE.Add(_value)
                    Else
                        Return False
                    End If
                Else
                    Return False
                End If
            End Function

            Public Sub New(value As System.TimeSpan)
                Me._value = value
            End Sub
            Public Sub New(value As System.DateTime)
                Me._value = value.Subtract(BASE_DATE)
            End Sub
            Public Sub New(str As String)
                If Not System.TimeSpan.TryParse(str.Trim(), Me._value) Then
                    Dim tmp As System.DateTime
                    System.DateTime.TryParse(str.Trim(), tmp)
                    Me._value = tmp.Subtract(BASE_DATE)
                End If
            End Sub

            Public Overrides Function GetHashCode() As Integer
                Return _value.GetHashCode()
            End Function
        End Class

        ''' <summary>
        ''' A fixed list of numbers
        ''' </summary>
        Public NotInheritable Class Tuple : Inherits EvalObjectBase
            Private _value As List(Of Reference)

            Public Overrides Function GetValue() As Object
                Return _value.ToArray()
            End Function

            Public Overrides Sub SetValue(obj As Object)
                If TypeOf obj Is List(Of Reference) Then
                    _value = DirectCast(obj, List(Of Reference))
                Else
                    If TypeOf obj Is Tuple Then obj = DirectCast(obj, Tuple).GetValue()
                    If Not TypeOf obj Is Reference() Then obj = {New Reference(obj)}
                    Dim reflst As Reference() = DirectCast(obj, Reference())
                    For i As Integer = 0 To _value.Count - 1
                        If reflst.Length <= i Then
                            _value(i).SetValue(reflst(reflst.Length - 1))
                        Else
                            _value(i).SetValue(reflst(i))
                        End If
                    Next
                End If
            End Sub

            Protected Overrides Function DeepCopy() As EvalObjectBase
                Dim lst As New List(Of Reference)
                For Each r As Reference In _value
                    lst.Add(DirectCast(r.GetDeepCopy(), Reference))
                Next
                Return New Tuple(lst)
            End Function

            Public Shared Shadows Function IsType(obj As Object) As Boolean
                Return TypeOf obj Is Tuple Or TypeOf obj Is Reference()
            End Function
            Public Shared Shadows Function StrIsType(str As String) As Boolean
                str = str.Trim()
                Return str.StartsWith("(") AndAlso str.EndsWith(")")
            End Function

            Public Overrides Function ToString() As String
                Dim str As New StringBuilder("(")
                For Each k As Reference In _value
                    If Not str.Length = 1 Then str.Append(", ")
                    Dim ef As InternalFunctions = New InternalFunctions(New Evaluator(reloadDefault:=False))
                    str.Append(ef.O(k.GetRefObject()))
                Next
                str.Append(")")
                Return str.ToString()
            End Function

            Public Overrides Function Equals(other As EvalObjectBase) As Boolean
                Return False
            End Function

            Public Sub New(value As List(Of Reference))
                Me._value = value
            End Sub
            Public Sub New(value As Reference())
                Me._value = value.ToList()
            End Sub
            Public Sub New(value As IEnumerable(Of Object))
                Me._value = New List(Of Reference)()
                For Each v As Object In value
                    If TypeOf v Is Reference AndAlso Not TypeOf DirectCast(v, Reference).GetRefObject() Is Reference Then
                        Me._value.Add(DirectCast(v, Reference))
                    Else
                        Me._value.Add(New Reference(DetectType(v, True)))
                    End If
                Next
            End Sub

            ''' <summary>
            ''' Create a new tuple
            ''' </summary>
            ''' <param name="conditions">If true, all members are evaluated in condition mode into booleans</param>
            Public Sub New(str As String, eval As Evaluator, Optional ByVal conditions As Boolean = True)
                'Try
                If StrIsType(str) Then
                    str = str.Trim().Remove(str.Length - 1).Substring(1).Trim(","c)
                    Me._value = New List(Of Reference)
                    If Not String.IsNullOrWhiteSpace(str) Then
                        Dim res As EvalObjectBase = ObjectTypes.DetectType(eval.EvalExprRaw("0," & str, True, conditions), True)
                        If IsType(res) Then
                            Dim lst As List(Of Reference) = CType(res.GetValue(), Reference()).ToList()
                            Me._value.AddRange(lst.GetRange(1, lst.Count - 1))
                        Else
                            Me._value.Add(New Reference(res))
                        End If
                    End If
                End If
                'Catch ex As Exception
                '    MsgBox(ex.ToString)
                'End Try
            End Sub

            Public Overrides Function GetHashCode() As Integer
                Return _value.GetHashCode()
            End Function
        End Class

        ''' <summary>
        ''' A vector or matrix 
        ''' </summary>
        Public NotInheritable Class Matrix : Inherits EvalObjectBase
            Private _value As List(Of Reference)
            Public ReadOnly Property Height As Integer
                Get
                    Return _value.Count
                End Get
            End Property
            Public ReadOnly Property Width As Integer
            Public ReadOnly Property Size As Size
                Get
                    Return New Size(Width, Height)
                End Get
            End Property

            Public Overrides Function GetValue() As Object
                Return _value
            End Function

            Public Overrides Sub SetValue(obj As Object)
                If TypeOf obj Is Reference() Then ' tuple
                    For i As Integer = 0 To Math.Min(_value.Count, DirectCast(obj, Reference()).Length) - 1
                        _value(i).ResolveRef().SetValue(DirectCast(obj, Reference())(i).ResolveObj())
                    Next
                Else
                    _value = DirectCast(obj, List(Of Reference))
                End If
                Me.Normalize()
            End Sub

            ''' <summary>
            ''' Get the object at the specified row and column in the matrix
            ''' </summary>
            ''' <returns></returns>
            Public Function GetCoord(row As Integer, col As Integer) As Object
                Dim r As EvalObjectBase = _value(row).GetRefObject()
                If TypeOf r Is Matrix Then
                    Return CType(CType(r, Matrix).GetValue(), List(Of Reference))(col).Resolve()
                Else
                    If col = 0 Then
                        If TypeOf r Is Reference Then Return DirectCast(r, Reference).Resolve()
                        If TypeOf r Is Number Then Return DirectCast(r, Number).BigDecValue
                        Return r.GetValue()
                    Else
                        Return Double.NaN
                    End If
                End If
            End Function

            ''' <summary>
            ''' Get the object at the specified row and column in the matrix as a reference
            ''' </summary>
            ''' <returns></returns>
            Public Function GetCoordRef(row As Integer, col As Integer) As Reference
                Dim r As EvalObjectBase = _value(row).GetRefObject()
                If TypeOf r Is Matrix Then
                    Return CType(CType(r, Matrix).GetValue(), List(Of Reference))(col)
                Else
                    If col = 0 Then
                        Return New Reference(r)
                    Else
                        Return New Reference(Double.NaN)
                    End If
                End If
            End Function

            ''' <summary>
            ''' Set the object at the specified row and column in the matrix
            ''' </summary>
            Public Sub SetCoord(row As Integer, col As Integer, obj As Object)
                Dim r As EvalObjectBase = _value(row).ResolveObj()
                If TypeOf obj Is Reference Then
                    If TypeOf r Is Matrix Then
                        CType(CType(r, Matrix).GetValue(), List(Of Reference))(col) = DirectCast(obj, Reference)
                    Else
                        If col = 0 Then _value(row) = DirectCast(obj, Reference)
                    End If
                Else
                    If TypeOf r Is Matrix Then
                        CType(CType(r, Matrix).GetValue(), List(Of Reference))(col).SetValue(obj)
                    Else
                        If col = 0 Then r.SetValue(obj)
                    End If
                End If
            End Sub

            ''' <summary>
            ''' Get the transpose of this matrix
            ''' </summary>
            ''' <returns></returns>
            Public Function Transpose() As Matrix
                Me.Normalize()
                Dim mat As Matrix = DirectCast(Me.GetDeepCopy(), Matrix)
                Dim m2 As New Matrix(DirectCast(DirectCast(Me.GetDeepCopy(), Matrix).GetValue(), List(Of Reference)))
                mat.Resize(Width, Height)
                For i As Integer = 0 To mat.Height - 1
                    For j As Integer = 0 To mat.Width - 1
                        mat.SetCoord(i, j, m2.GetCoord(j, i))
                    Next
                Next
                Return mat
            End Function

            Public Function Determinant() As Object
                ' can only calculate det for square matrices
                If Width <> Height Then Throw New MathException("Can only calculate determinant for square matrices")

                ' base cases
                If Width = 0 AndAlso Height = 0 Then Return 1.0
                If Width = 1 AndAlso Height = 1 Then Return GetCoord(0, 0)

                Dim ans As Object = 0

                Dim coeff As Integer = 1
                For i As Integer = 0 To Me.Width - 1
                    Dim newmat As New Matrix(Height - 1, Width - 1)
                    For j As Integer = 1 To Me.Height - 1
                        For k As Integer = 0 To Me.Width - 1
                            If i = k Then Continue For
                            newmat.SetCoord(j - 1, If(k > i, k - 1, k), GetCoord(j, k))
                        Next
                    Next
                    Dim i1 As Object = GetCoord(0, i)
                    Dim i2 As Object = newmat.Determinant()
                    If TypeOf ans Is Numerics.Complex OrElse TypeOf i1 Is Numerics.Complex OrElse TypeOf i2 Is Numerics.Complex Then
                        If Not TypeOf ans Is Numerics.Complex Then ans = New Numerics.Complex(CDbl(ans), 0)
                        If Not TypeOf i1 Is Numerics.Complex Then i1 = New Numerics.Complex(CDbl(i1), 0)
                        If Not TypeOf i2 Is Numerics.Complex Then i2 = New Numerics.Complex(CDbl(i2), 0)
                        ans = CType(ans, Numerics.Complex) + coeff * CType(i1, Numerics.Complex) * CType(i2, Numerics.Complex)
                    ElseIf TypeOf i1 Is Double AndAlso TypeOf i2 Is Double
                        ans = CDbl(ans) + coeff * CDbl(i1) * CDbl(i2)
                    ElseIf TypeOf i1 Is BigDecimal AndAlso TypeOf i2 Is BigDecimal
                        ans = CType(ans, BigDecimal) + coeff * CType(i1, BigDecimal) * CType(i2, BigDecimal)
                    Else
                        Return Double.NaN
                    End If
                    coeff = -coeff
                Next
                Return ans
            End Function

            ''' <summary>
            ''' Multiply two matrices, if the width of the first is equal to the height of the second.
            ''' </summary>
            ''' <param name="mb"></param>
            ''' <returns></returns>
            Public Function Multiply(mb As Matrix) As Matrix
                If Width = mb.Height Then
                    Dim res As New List(Of Reference)
                    For row As Integer = 0 To Height - 1
                        Dim currow As New List(Of Reference)
                        For col As Integer = 0 To mb.Width - 1
                            Dim curitm As Object = 0.0
                            For i As Integer = 0 To Width - 1
                                Dim i1 As Object = GetCoord(row, i)
                                Dim i2 As Object = mb.GetCoord(i, col)
                                If TypeOf curitm Is Numerics.Complex OrElse TypeOf i1 Is Numerics.Complex OrElse TypeOf i2 Is Numerics.Complex Then
                                    If Not TypeOf curitm Is Numerics.Complex Then curitm = New Numerics.Complex(CDbl(curitm), 0)
                                    If Not TypeOf i1 Is Numerics.Complex Then i1 = New Numerics.Complex(CDbl(i1), 0)
                                    If Not TypeOf i2 Is Numerics.Complex Then i2 = New Numerics.Complex(CDbl(i2), 0)
                                    curitm = CType(curitm, Numerics.Complex) + CType(i1, Numerics.Complex) * CType(i2, Numerics.Complex)
                                ElseIf TypeOf i1 Is Double AndAlso TypeOf i2 Is Double
                                    curitm = CDbl(curitm) + CDbl(i1) * CDbl(i2)
                                ElseIf TypeOf i1 Is BigDecimal AndAlso TypeOf i2 Is BigDecimal
                                    curitm = CType(curitm, BigDecimal) + CType(i1, BigDecimal) * CType(i2, BigDecimal)
                                Else
                                    Throw New EvaluatorException("Invalid type in matrix for multiplication. Only numbers and complex values are allowed.")
                                End If
                            Next
                            currow.Add(New Reference(curitm))
                        Next
                        res.Add(New Reference(New Matrix(currow)))
                    Next
                    Me._value = res
                    Return Me
                Else
                    Throw New MathException("Width of first matrix must equal height of second")
                End If
            End Function

            ''' <summary>
            ''' Multiply the matrix by a scalar quantity
            ''' </summary>
            ''' <param name="b"></param>
            ''' <returns></returns>
            Public Function MultiplyScalar(b As Object) As Matrix
                If TypeOf b Is Double Then b = CType(CDbl(b), BigDecimal)

                For row As Integer = 0 To Height - 1
                    Dim currow As New List(Of Reference)
                    For col As Integer = 0 To Width - 1
                        Dim cur As Object = GetCoord(row, col)
                        If TypeOf cur Is Numerics.Complex OrElse TypeOf b Is Numerics.Complex Then
                            If Not TypeOf cur Is Numerics.Complex Then cur = New Numerics.Complex(CDbl(cur), 0)
                            If Not TypeOf b Is Numerics.Complex Then b = New Numerics.Complex(CDbl(b), 0)
                            SetCoord(row, col, CType(cur, Numerics.Complex) * CType(b, Numerics.Complex))
                        ElseIf TypeOf cur Is BigDecimal AndAlso TypeOf b Is BigDecimal Then
                            SetCoord(row, col, CType(cur, BigDecimal) * DirectCast(b, BigDecimal))
                        ElseIf TypeOf cur Is Double AndAlso TypeOf b Is BigDecimal Then
                            SetCoord(row, col, CType(CDbl(cur), BigDecimal) * DirectCast(b, BigDecimal))
                        Else
                            Throw New EvaluatorException("Invalid type in matrix for scalar multiplication. Only numbers and complex values are allowed.")
                        End If
                    Next
                Next
                Return Me
            End Function

            Public Function Dot(other As Matrix) As Object
                If Width <> 1 OrElse other.Width <> 1 Then
                    Throw New MathException("Can only calculate dot product of two column vectors")
                End If
                Dim valueR As List(Of Reference) = DirectCast(other.GetValue(), List(Of Reference))
                Dim ans As Object = 0

                For i As Integer = 0 To Math.Min(Height, other.Height) - 1
                    Dim a As Object = _value(i).Resolve()
                    Dim b As Object = valueR(i).Resolve()
                    If TypeOf a Is Numerics.Complex OrElse
                        TypeOf b Is Numerics.Complex OrElse TypeOf ans Is Numerics.Complex Then
                        If TypeOf a Is Double OrElse TypeOf a Is BigDecimal Then a = New Numerics.Complex(CDbl(a), 0)
                        If TypeOf b Is Double OrElse TypeOf b Is BigDecimal Then b = New Numerics.Complex(CDbl(b), 0)
                        If TypeOf ans Is Double OrElse TypeOf ans Is BigDecimal Then ans = New Numerics.Complex(CDbl(ans), 0)
                        ans = CType(ans, Numerics.Complex) + CType(a, Numerics.Complex) * CType(b, Numerics.Complex)
                    ElseIf TypeOf a Is Double AndAlso TypeOf b Is Double
                        ans = CDbl(ans) + CDbl(a) * CDbl(b)
                    ElseIf TypeOf a Is BigDecimal AndAlso TypeOf b Is BigDecimal
                        ans = CType(ans, BigDecimal) + CType(a, BigDecimal) * CType(b, BigDecimal)
                    End If
                Next
                Return ans
            End Function

            ''' <summary>
            ''' Sets the current vector to the cross product with another vector. 
            ''' Only works for vectors in R3 (lower dimension vectors are padded with 0 in unspecified dimensions).
            ''' </summary>
            ''' <param name="other"></param>
            ''' <returns></returns>
            Public Function Cross(other As Matrix) As Matrix
                If Width <> 1 OrElse other.Width <> 1 OrElse Height > 3 OrElse other.Height > 3 Then
                    Throw New MathException("Can only calculate cross product of two column vectors in R3")
                End If

                Resize(3, 1)
                other.Resize(3, 1)

                Dim newValue As New List(Of Reference)(3)
                For i As Integer = 0 To 2

                    Dim j As Integer = (i + 1) Mod 3
                    Dim k As Integer = (j + 1) Mod 3
                    If k = i Then k += 1
                    Dim evalStr As String = GetCoord(j, 0).ToString() & "*" & other.GetCoord(k, 0).ToString() & "-" &
                    GetCoord(k, 0).ToString() & "*" & other.GetCoord(j, 0).ToString()

                    Dim result As EvalObjectBase = DetectType(New Evaluator(reloadDefault:=False).EvalExprRaw(
                                                              evalStr.ToString(), True))
                    If TypeOf result Is Reference Then
                        newValue.Add(DirectCast(result, Reference))
                    Else
                        newValue.Add(New Reference(result))
                    End If
                Next
                _value = newValue
                Return Me
            End Function

            ''' <summary>
            ''' Finds the norm of the vector (actually gives square of the magnitude)
            ''' </summary>
            ''' <returns></returns>
            Public Function Norm() As Object
                If Me.Width <> 1 Then Throw New MathException("Can only get norm of column vectors")
                Dim result As Object = 0.0
                For i As Integer = 0 To Me.Height - 1
                    Dim a As Object = GetCoord(i, 0)
                    If TypeOf result Is Numerics.Complex OrElse TypeOf a Is Numerics.Complex Then

                        If Not TypeOf a Is Numerics.Complex Then a = New Numerics.Complex(CDbl(a), 0)
                        If Not TypeOf result Is Numerics.Complex Then a = New Numerics.Complex(CDbl(result), 0)

                        result = DirectCast(result, Numerics.Complex) +
                            DirectCast(a, Numerics.Complex) * DirectCast(a, Numerics.Complex)

                    ElseIf TypeOf result Is Double AndAlso TypeOf a Is Double
                        result = CDbl(result) + CDbl(a) * CDbl(a)

                    ElseIf TypeOf result Is BigDecimal AndAlso TypeOf a Is BigDecimal
                        result = DirectCast(result, BigDecimal) + DirectCast(a, BigDecimal) * DirectCast(a, BigDecimal)

                    End If
                Next

                Return result
            End Function

            Public Function Magnitude() As Object
                Dim norm As Object = Me.Norm()
                If TypeOf norm Is Numerics.Complex Then
                    Return Numerics.Complex.Sqrt(DirectCast(norm, Numerics.Complex))
                Else
                    Return Math.Sqrt(CDbl(norm))
                End If
            End Function

            ''' <summary>
            ''' Retrieve the specified column as a column vector
            ''' </summary>
            ''' <returns></returns>
            Public Function Col(id As Integer) As Matrix
                Dim tmp As New List(Of Reference)

                For i As Integer = 0 To Height - 1
                    If TypeOf Me._value(i).ResolveObj() Is Matrix Then
                        tmp.Add(DirectCast(Me._value(i).Resolve(), List(Of Reference))(id))
                    ElseIf TypeOf Me._value(i).ResolveObj() Is Reference Then
                        tmp.Add(DirectCast(Me._value(i).ResolveObj(), Reference))
                    Else
                        tmp.Add(New Reference(Me._value(i).ResolveObj()))
                    End If
                Next

                Return New Matrix(tmp)
            End Function

            ''' <summary>
            ''' Retrieve the specied row as a column vector
            ''' </summary>
            ''' <param name="id"></param>
            ''' <returns></returns>
            Public Function Row(id As Integer) As Matrix
                If TypeOf Me._value(id).ResolveObj() Is Matrix Then
                    Return DirectCast(Me._value(id).ResolveObj(), Matrix)
                Else
                    Return New Matrix({Me._value(id).ResolveObj()})
                End If
            End Function

            ''' <summary>
            ''' Swap two matrix rows and return the matrix
            ''' </summary>
            ''' <param name="aug">Matrix representing right side of augmented matrix, if available</param>
            ''' <returns></returns>
            Public Function SwapRows(a As Integer, b As Integer, Optional ByRef aug As Matrix = Nothing) As Matrix
                Dim tmp As Reference = _value(a)
                _value(a) = _value(b)
                _value(b) = tmp
                If Not aug Is Nothing Then aug.SwapRows(a, b)
                Return Me
            End Function

            ''' <summary>
            ''' Swap two matrix columns and return the matrix
            ''' </summary>
            ''' <returns></returns>
            Public Function SwapCols(a As Integer, b As Integer) As Matrix
                For i As Integer = 0 To Height - 1
                    If TypeOf Me._value(i).ResolveObj() Is Matrix Then
                        Dim tmp As Reference
                        tmp = DirectCast(Me._value(i).Resolve(), List(Of Reference))(b)
                        DirectCast(Me._value(i).Resolve(), List(Of Reference))(b) =
                            DirectCast(Me._value(i).Resolve(), List(Of Reference))(a)
                        DirectCast(Me._value(i).Resolve(), List(Of Reference))(a) = tmp
                    Else
                        If a <> 0 OrElse b <> 0 Then Throw New MathException("Index is out of bounds")
                    End If
                Next
                Return Me
            End Function

            ''' <summary>
            ''' Scale the specified row by the specified factor
            ''' </summary>
            ''' <param name="aug">Right side of augmented matrix, if applicable</param>
            Public Sub ScaleRow(row As Integer, scale As Object, Optional ByRef aug As Matrix = Nothing)
                For i As Integer = 0 To Width - 1
                    Dim orig As Object = GetCoord(row, i)
                    If TypeOf scale Is Numerics.Complex OrElse TypeOf orig Is Numerics.Complex Then
                        If Not TypeOf scale Is Numerics.Complex Then scale = New Numerics.Complex(CDbl(scale), 0)
                        If Not TypeOf orig Is Numerics.Complex Then orig = New Numerics.Complex(CDbl(orig), 0)
                        SetCoord(row, i, New Reference(DirectCast(orig, Numerics.Complex) * DirectCast(scale, Numerics.Complex)))
                    ElseIf TypeOf orig Is Double
                        SetCoord(row, i, New Reference((CDbl(orig) * CType(scale, BigDecimal))))
                    ElseIf TypeOf orig Is BigDecimal
                        SetCoord(row, i, New Reference((DirectCast(orig, BigDecimal) * DirectCast(scale, BigDecimal))))
                    End If
                Next
                If Not aug Is Nothing Then aug.ScaleRow(row, scale)
            End Sub

            ''' <summary>
            ''' Subtract row b from row a and assign the values to row a
            ''' </summary>
            ''' <param name="aug">Right side of augmented matrix, if applicable</param>
            Public Sub SubtractRow(a As Integer, b As Integer, Optional ByRef aug As Matrix = Nothing)
                For i As Integer = 0 To Width - 1
                    Dim av As Object = GetCoord(a, i)
                    Dim bv As Object = GetCoord(b, i)
                    If TypeOf av Is Numerics.Complex OrElse TypeOf bv Is Numerics.Complex Then
                        If Not TypeOf av Is Numerics.Complex Then av = New Numerics.Complex(CDbl(av), 0)
                        If Not TypeOf bv Is Numerics.Complex Then bv = New Numerics.Complex(CDbl(bv), 0)
                        SetCoord(a, i, DirectCast(av, Numerics.Complex) - DirectCast(bv, Numerics.Complex))
                    ElseIf TypeOf av Is Double AndAlso TypeOf bv Is Double
                        SetCoord(a, i, New Reference(CType(CDbl(av) - CDbl(bv), BigDecimal)))
                    ElseIf TypeOf av Is BigDecimal AndAlso TypeOf bv Is BigDecimal
                        SetCoord(a, i, New Reference((DirectCast(av, BigDecimal) - DirectCast(bv, BigDecimal))))
                    End If
                Next
                If Not aug Is Nothing Then aug.SubtractRow(a, b)
            End Sub

            ''' <summary>
            ''' Helper function for getting the reciprocal of various types
            ''' </summary>
            ''' <returns></returns>
            Private Function AutoReciprocal(a As Object) As Object
                If TypeOf a Is Double Then
                    Return 1 / CType(CDbl(a), BigDecimal)
                ElseIf TypeOf a Is BigDecimal Then
                    Return 1 / DirectCast(a, BigDecimal)
                ElseIf TypeOf a Is Numerics.Complex Then
                    Return 1 / DirectCast(a, Numerics.Complex)
                Else
                    Return 1
                End If
            End Function

            ''' <summary>
            ''' Find the reduced row echelon form of the matrix
            ''' </summary>
            ''' <param name="augmented">Matrix to modify along with the current matrix as an augmented matrix</param>
            ''' <returns></returns>
            Public Function Rref(Optional ByRef augmented As Matrix = Nothing) As Matrix

                ' deep copy everything before doing anything to avoid messing up due to references
                Dim mat As Matrix = DirectCast(Me.GetDeepCopy(), Matrix)

                '' convert all rows to matrices
                'If mat.Width = 1 Then
                '    Dim lst As List(Of Reference) = DirectCast(mat.GetValue(), List(Of Reference))
                '    For i As Integer = 0 To mat.Height - 1
                '        lst(i) = New Reference(New Matrix({lst(i).ResolveObj()}))
                '    Next
                'End If

                Dim pivot As New Dictionary(Of Integer, Integer)
                Dim curRow As Integer = 0
                For col As Integer = 0 To mat.Width - 1
                    If curRow >= mat.Height Then Return mat
                    Dim success As Boolean = True

                    For swapRow As Integer = curRow To mat.Height
                        If swapRow = mat.Height Then ' reached end, failed to find an appropriate row
                            success = False
                            Exit For
                        End If
                        Dim val As Object = mat.GetCoord(swapRow, col)
                        If (TypeOf val Is Double AndAlso CDbl(val) <> 0.0) OrElse (TypeOf val Is BigDecimal AndAlso
                            CType(val, BigDecimal) <> 0.0) OrElse
                            TypeOf val Is Numerics.Complex AndAlso Math.Round(DirectCast(val, Numerics.Complex).Magnitude, 12) <> 0 Then
                            mat.SwapRows(curRow, swapRow, augmented)
                            mat.ScaleRow(curRow, AutoReciprocal(val), augmented)
                            pivot(curRow) = col
                            Exit For
                        End If
                    Next

                    If Not success Then Continue For

                    For zeroOutRow As Integer = 0 To Height - 1
                        If zeroOutRow = curRow Then Continue For
                        Dim val As Object = mat.GetCoord(zeroOutRow, col)
                        If (TypeOf val Is Double AndAlso CDbl(val) <> 0.0) OrElse (TypeOf val Is BigDecimal AndAlso
                            CType(val, BigDecimal) <> 0.0) OrElse
                            TypeOf val Is Numerics.Complex AndAlso Math.Round(DirectCast(val, Numerics.Complex).Magnitude, 12) <> 0 Then
                            mat.ScaleRow(zeroOutRow, AutoReciprocal(val), augmented)
                            mat.SubtractRow(zeroOutRow, curRow, augmented)

                            ' if the row was already processed then we need to scale its pivot back to one
                            If zeroOutRow < curRow Then
                                mat.ScaleRow(zeroOutRow, AutoReciprocal(mat.GetCoord(zeroOutRow, pivot(zeroOutRow))), augmented)
                            End If
                        End If
                    Next
                    curRow += 1
                Next

                While curRow < mat.Height
                    mat.ScaleRow(curRow, 0, augmented)
                    curRow += 1
                End While

                Return mat
            End Function

            ''' <summary>
            ''' Invert this matrix and return it
            ''' </summary>
            ''' <returns></returns>
            Public Function Inverse() As Matrix
                Dim r As Matrix = Matrix.IdentityMatrix(Height, Width)
                Dim l As Matrix = DirectCast(Me.GetDeepCopy(), Matrix)
                l.Rref(r)
                If Not l.IsIdentityMatrix() Then Return New Matrix({Double.NaN})
                Me._value = DirectCast(r.GetValue(), List(Of Reference))
                Return Me
            End Function

            ''' <summary>
            ''' Exponentiate the matrix. Or, if the exponent is -1, inverts the matrix. 
            ''' </summary>
            ''' <param name="p">The exponent</param>
            ''' <returns></returns>
            Public Function Expo(p As Integer) As Matrix
                If p = -1 Then Return Inverse()
                If p < 0 Then Throw New MathException("Negative exponents of matrices not defined (except -1" &
                    "which is interpreted as matrix inverse)")
                If Width <> Height Then Throw New MathException("Only square matrices may be exponenciated.")

                Dim curp As Integer = 2
                Dim origmat As Matrix = DirectCast(Me.GetDeepCopy(), Matrix)

                While curp <= p
                    Me.Multiply(Me)
                    curp *= 2
                End While
                curp \= 2

                While curp < p
                    Me.Multiply(origmat)
                    curp += 1
                End While

                Return Me
            End Function

            Protected Overrides Function DeepCopy() As EvalObjectBase
                Dim lst As New List(Of Reference)
                For Each r As Reference In _value
                    lst.Add(DirectCast(r.GetDeepCopy(), Reference))
                Next
                Return New Matrix(lst)
            End Function

            Public Shared Shadows Function IsType(obj As Object) As Boolean
                Return TypeOf obj Is Matrix Or TypeOf obj Is System.Collections.Generic.List(Of Reference)
            End Function

            Public Shared Shadows Function StrIsType(str As String) As Boolean
                str = str.Trim()
                Return str.StartsWith("[") AndAlso str.EndsWith("]")
            End Function

            Public Overrides Function ToString() As String
                Dim str As New StringBuilder("[")
                For Each k As Reference In _value
                    If Not str.Length = 1 Then str.Append(", ")
                    Dim ef As InternalFunctions = New InternalFunctions(New Evaluator(reloadDefault:=False))
                    str.Append(ef.O(k.GetRefObject()))
                Next
                str.Append("]")
                Return str.ToString()
            End Function

            Public Overrides Function Equals(other As EvalObjectBase) As Boolean
                If TypeOf other Is Matrix Then
                    Return GenericComparer.CompareLists(_value, CType(other.GetValue(), List(Of Reference))) = 0
                Else
                    Return False
                End If
            End Function

            ''' <summary>
            ''' Make this a proper matrix by making all the rows and columns the same length, respectively
            ''' </summary>
            Public Sub Normalize()
                Dim max_len As Integer = 0
                For Each r As Reference In _value
                    If TypeOf r.GetRefObject() Is Matrix Then
                        Dim ct As Integer = CType(CType(r.GetRefObject(), Matrix).GetValue(), List(Of Reference)).Count
                        If ct > max_len Then max_len = ct
                    Else
                        If 1 > max_len Then max_len = 1
                    End If
                Next
                Resize(Height, max_len)
            End Sub

            ''' <summary>
            ''' Resize the matrix by cropping out extra rows/columns and adding empty rows/columns filled with zeros as necessary
            ''' </summary>
            Public Sub Resize(size As Size)
                Resize(size.Height, size.Width)
            End Sub

            ''' <summary>
            ''' Resize the matrix by cropping out extra rows/columns and adding empty rows/columns filled with zeros as necessary
            ''' </summary>
            Public Sub Resize(height As Integer, width As Integer)
                _Width = width

                ' fit height
                While Me.Height < height
                    _value.Add(New Reference(0.0))
                End While
                While Me.Height > height
                    _value.RemoveAt(_value.Count - 1)
                End While

                ' fit width
                For i As Integer = 0 To height - 1
                    Dim r As Reference = _value(i)
                    If width > 1 Then
                        If Not TypeOf r.ResolveObj() Is Matrix Then
                            r.SetValue(New Matrix({r.GetValue()}))
                        End If
                        Dim inner As List(Of Reference) = CType(CType(r.GetRefObject(), Matrix).GetValue(), List(Of Reference))
                        While inner.Count < width
                            inner.Add(New Reference(0.0))
                        End While
                        While inner.Count > width
                            inner.RemoveAt(inner.Count - 1)
                        End While
                    ElseIf width = 1 Then
                        ' if single column, expand to column vector
                        If TypeOf r.ResolveObj() Is Matrix Then
                            Dim lst As List(Of Reference) = DirectCast(r.Resolve(), List(Of Reference))
                            If lst.Count = 0 Then r.SetValue(Double.NaN) Else r.SetValue(lst(0).ResolveObj())
                        End If
                    End If
                Next
            End Sub

            ''' <summary>
            ''' Returns true if the current matrix represents an identity matrix
            ''' </summary>
            Public Function IsIdentityMatrix() As Boolean
                For i As Integer = 0 To Height - 1
                    For j As Integer = 0 To Width - 1
                        Dim expected As Integer = If(i = j, 1, 0)
                        Dim obj As Object = GetCoord(i, j)
                        If TypeOf obj Is Numerics.Complex Then
                            If Math.Round(DirectCast(obj, Numerics.Complex).Magnitude, 12) <> expected Then Return False
                        ElseIf TypeOf obj Is Double OrElse TypeOf obj Is BigDecimal
                            If CType(obj, BigDecimal).Truncate(12) <> expected Then Return False
                        End If
                    Next
                Next
                Return True
            End Function

            ''' <summary>
            ''' Get the identity matrix (matrix with all zeros except on the main diagonal, which contains all ones) 
            ''' with the specified number of rows and cols (if cols is not specified, a square matrix is returned)
            ''' </summary>
            Public Shared Function IdentityMatrix(rows As Integer, Optional cols As Integer = -1) As Matrix
                If cols = -1 Then cols = rows

                Dim mat As New Matrix(rows, cols)
                For i As Integer = 0 To Math.Min(rows, cols) - 1
                    mat.SetCoord(i, i, 1.0)
                Next
                Return mat
            End Function

            ''' <summary>
            ''' Create a new matrix with the specified number of rows and columns, filled with 0.
            ''' If cols is not specified, then a square matrix is returned.
            ''' </summary>
            Public Sub New(rows As Integer, Optional cols As Integer = -1)
                If cols = -1 Then cols = rows
                Me._value = New List(Of Reference)(rows)
                Me.Resize(rows, cols)
            End Sub

            ''' <summary>
            ''' Create a new matrix from a list of references
            ''' </summary>
            Public Sub New(value As IEnumerable(Of Reference))
                If TypeOf value Is List(Of Reference) Then
                    Me._value = DirectCast(value, List(Of Reference))
                Else
                    Me._value = value.ToList()
                End If
                Me.Normalize()
            End Sub

            ''' <summary>
            ''' Create a new matrix from a list of evaluator objects
            ''' </summary>
            Public Sub New(value As IEnumerable(Of EvalObjectBase))
                Me._value = New List(Of Reference)()
                For Each v As EvalObjectBase In value
                    If TypeOf v Is Reference Then
                        Me._value.Add(DirectCast(v, Reference))
                    Else
                        Me._value.Add(New Reference(v))
                    End If
                Next
                Me.Normalize()
            End Sub

            ''' <summary>
            ''' Create a new matrix from a list of system objects
            ''' </summary>
            Public Sub New(value As IEnumerable(Of Object))
                Me._value = New List(Of Reference)()
                For Each v As Object In value
                    Me._value.Add(New Reference(ObjectTypes.DetectType(v, True)))
                Next
                Me.Normalize()
            End Sub

            ''' <summary>
            ''' Create a new matrix from a string in matrix format: [[1,2,3],[2,3,4],[3,4,5]]
            ''' </summary>
            Public Sub New(str As String, eval As Evaluator)
                Try
                    If StrIsType(str) Then
                        str = str.Trim().Remove(str.Length - 1).Substring(1).Trim()

                        Me._value = New List(Of Reference)
                        If Not String.IsNullOrWhiteSpace(str) Then
                            ' add zeros to fill blanks for convenience
                            str = "," & str & ","
                            While str.Contains(", ")
                                str = str.Replace(", ", ",") ' prepare so that we can detect ,,'s later
                            End While
                            While str.Contains(",,")
                                str = str.Replace(",,", ",0,") ' add zeros to fill blanks for convenience
                            End While

                            ' ignore hanging commas
                            If str.EndsWith(",") Then str = str.Remove(str.Length - 1)
                            If str.StartsWith(",") Then str = str.Substring(1)

                            Dim res As Object = eval.EvalExprRaw("0," & str, True, True)

                            If Tuple.IsType(res) Then
                                Dim lst As List(Of Reference) = CType(res, Reference()).ToList()
                                Me._value.AddRange(lst.GetRange(1, lst.Count - 1))
                            Else
                                Me._value.Add(New Reference(res))
                            End If
                        End If
                    End If

                    Me.Normalize()
                Catch 'ex As Exception
                    'MsgBox(ex.ToString)
                End Try
            End Sub

            Public Overrides Function GetHashCode() As Integer
                Return _value.GetHashCode()
            End Function
        End Class

        ''' <summary>
        ''' A dictionary/set of objects
        ''' </summary>
        Public NotInheritable Class [Set] : Inherits EvalObjectBase
            Private _value As System.Collections.Generic.SortedDictionary(Of Reference, Reference)
            Public Overrides Function GetValue() As Object
                Return _value
            End Function
            Public Overrides Sub SetValue(obj As Object)
                _value = DirectCast(obj, SortedDictionary(Of Reference, Reference))
            End Sub
            Public Shared Shadows Function IsType(obj As Object) As Boolean
                Return TypeOf obj Is [Set] Or
                    TypeOf obj Is System.Collections.Generic.SortedDictionary(Of Reference, Reference)
            End Function
            Public Shared Shadows Function StrIsType(str As String) As Boolean
                str = str.Trim()
                Return str.StartsWith("{") AndAlso str.EndsWith("}")
            End Function

            Protected Overrides Function DeepCopy() As EvalObjectBase
                Dim dict As New SortedDictionary(Of Reference, Reference)(New GenericComparer())
                For Each k As KeyValuePair(Of Reference, Reference) In _value
                    Dim key As Reference = DirectCast(k.Key.GetDeepCopy(), Reference)
                    If k.Value Is Nothing Then
                        dict(key) = Nothing
                    Else
                        dict(key) = DirectCast(k.Value.GetDeepCopy(), Reference)
                    End If
                Next
                Return New [Set](dict)
            End Function

            Private Sub ConvertFrom(obj As EvalObjectBase)
                If TypeOf obj Is [Set] Then
                    _value = New SortedDictionary(Of Reference, Reference)(
                        CType(obj.GetValue(), SortedDictionary(Of Reference, Reference)), New GenericComparer())
                ElseIf TypeOf obj Is Matrix Then
                    _value = New SortedDictionary(Of Reference, Reference)(New GenericComparer())
                    For Each o As EvalObjectBase In CType(obj.GetValue(), List(Of Reference))
                        _value(New Reference(o)) = Nothing
                    Next
                Else
                    _value = New SortedDictionary(Of Reference, Reference)(New GenericComparer())
                    _value(New Reference(obj)) = Nothing
                End If
            End Sub

            Public Overrides Function ToString() As String
                Dim str As New StringBuilder("{")
                For Each k As KeyValuePair(Of Reference, Reference) In _value
                    If Not str.Length = 1 Then str.Append(", ")
                    Dim ef As InternalFunctions = New InternalFunctions(New Evaluator(reloadDefault:=False))
                    str.Append(ef.O(k.Key.GetRefObject()))
                    If Not k.Value Is Nothing Then
                        str.Append(":" & ef.O(k.Value.GetRefObject()))
                    End If
                Next
                str.Append("}")
                Return str.ToString()
            End Function

            Public Overrides Function Equals(other As EvalObjectBase) As Boolean
                Return False
            End Function

            Public Sub New(value As System.Collections.Generic.IEnumerable(Of Reference))
                Me.ConvertFrom(New Matrix(value))
            End Sub
            Public Sub New(value As System.Collections.Generic.IDictionary(Of Reference, Reference))
                Me._value = New SortedDictionary(Of Reference, Reference)(value, New GenericComparer())
            End Sub
            Public Sub New(value As System.Collections.Generic.IDictionary(Of Object, Object))
                Me._value = New SortedDictionary(Of Reference, Reference)()
                For Each k As KeyValuePair(Of Object, Object) In value
                    Me._value(New Reference(ObjectTypes.DetectType(k.Key, True))) = New Reference(ObjectTypes.DetectType(k.Value, True))
                Next
            End Sub
            Public Sub New(str As String, eval As Evaluator)
                Try
                    If StrIsType(str) Then
                        str = str.Trim().Remove(str.Length - 1).Substring(1).Trim(","c)
                        Me._value = New SortedDictionary(Of Reference, Reference)(New GenericComparer())
                        If String.IsNullOrWhiteSpace(str) Then Return
                        Dim lst As New List(Of Reference)(CType(New Tuple("(" & str & ")", eval).GetValue(), Reference()))
                        For Each o As EvalObjectBase In lst
                            If TypeOf o Is Reference Then o = CType(o, Reference).GetRefObject()
                            If Tuple.IsType(o) Then
                                Dim innerlst As Reference() = CType(o.GetValue(), Reference())
                                If innerlst.Count = 2 Then
                                    Me._value(innerlst(0)) = innerlst(1)
                                    Continue For
                                End If
                            End If
                            Me._value(New Reference(o)) = Nothing
                        Next
                    End If
                Catch 'ex As Exception
                End Try
            End Sub

            Public Overrides Function GetHashCode() As Integer
                Return _value.GetHashCode()
            End Function
        End Class

        ''' <summary>
        ''' A hashed dictionary/set of objects
        ''' </summary>
        Public NotInheritable Class HashSet : Inherits EvalObjectBase
            Private _value As System.Collections.Generic.Dictionary(Of Reference, Reference)
            Public Overrides Function GetValue() As Object
                Return _value
            End Function
            Public Overrides Sub SetValue(obj As Object)
                _value = DirectCast(obj, Dictionary(Of Reference, Reference))
            End Sub
            Public Shared Shadows Function IsType(obj As Object) As Boolean
                Return TypeOf obj Is HashSet Or
                    TypeOf obj Is System.Collections.Generic.Dictionary(Of Reference, Reference)
            End Function
            Public Shared Shadows Function StrIsType(str As String) As Boolean
                str = str.Trim()
                Return str.StartsWith("HashSet({") AndAlso str.EndsWith("})")
            End Function

            Protected Overrides Function DeepCopy() As EvalObjectBase
                Dim dict As New Dictionary(Of Reference, Reference)(New GenericComparer())
                For Each k As KeyValuePair(Of Reference, Reference) In _value
                    Dim key As Reference = DirectCast(k.Key.GetDeepCopy(), Reference)
                    If k.Value Is Nothing Then
                        dict(key) = Nothing
                    Else
                        dict(key) = DirectCast(k.Value.GetDeepCopy(), Reference)
                    End If
                Next
                Return New HashSet(dict)
            End Function

            Private Sub ConvertFrom(obj As EvalObjectBase)
                If TypeOf obj Is [Set] Then
                    _value = New Dictionary(Of Reference, Reference)(
                        CType(obj.GetValue(), Dictionary(Of Reference, Reference)), New GenericComparer())
                ElseIf TypeOf obj Is Matrix Then
                    _value = New Dictionary(Of Reference, Reference)(New GenericComparer())
                    For Each o As EvalObjectBase In CType(obj.GetValue(), List(Of Reference))
                        _value(New Reference(o)) = Nothing
                    Next
                Else
                    _value = New Dictionary(Of Reference, Reference)(New GenericComparer())
                    _value(New Reference(obj)) = Nothing
                End If
            End Sub

            Public Overrides Function ToString() As String
                Dim str As New StringBuilder("HashSet({")
                For Each k As KeyValuePair(Of Reference, Reference) In _value
                    If Not str.Chars(str.Length - 1) = "{" Then str.Append(", ")
                    Dim ef As InternalFunctions = New InternalFunctions(New Evaluator(reloadDefault:=False))
                    str.Append(ef.O(k.Key.GetRefObject()))
                    If Not k.Value Is Nothing Then
                        str.Append(":" & ef.O(k.Value.GetRefObject()))
                    End If
                Next
                str.Append("})")
                Return str.ToString()
            End Function

            Public Overrides Function Equals(other As EvalObjectBase) As Boolean
                Return False
            End Function

            Public Sub New(value As System.Collections.Generic.IEnumerable(Of Reference))
                Me.ConvertFrom(New Matrix(value))
            End Sub
            Public Sub New(value As System.Collections.Generic.IDictionary(Of Reference, Reference))
                Me._value = New Dictionary(Of Reference, Reference)(value, New GenericComparer())
            End Sub
            Public Sub New(value As System.Collections.Generic.IDictionary(Of Object, Object))
                Me._value = New Dictionary(Of Reference, Reference)()
                For Each k As KeyValuePair(Of Object, Object) In value
                    Me._value(New Reference(ObjectTypes.DetectType(k.Key, True))) = New Reference(ObjectTypes.DetectType(k.Value, True))
                Next
            End Sub
            Public Sub New(str As String, eval As Evaluator)
                Try
                    If StrIsType(str) Then
                        str = str.Trim().Remove(str.Length - 2).Substring("HashSet({".Length).Trim(","c)
                        Me._value = New Dictionary(Of Reference, Reference)(New GenericComparer())
                        If String.IsNullOrWhiteSpace(str) Then Return
                        Dim lst As New List(Of Reference)(CType(New Tuple("(" & str & ")", eval).GetValue(), Reference()))
                        For Each o As EvalObjectBase In lst
                            If TypeOf o Is Reference Then o = CType(o, Reference).GetRefObject()
                            If Tuple.IsType(o) Then
                                Dim innerlst As Reference() = CType(o.GetValue(), Reference())
                                If innerlst.Count = 2 Then
                                    Me._value(innerlst(0)) = innerlst(1)
                                    Continue For
                                End If
                            End If
                            Me._value(New Reference(o)) = Nothing
                        Next
                    End If
                Catch 'ex As Exception
                End Try
            End Sub

            Public Overrides Function GetHashCode() As Integer
                Return _value.GetHashCode()
            End Function
        End Class

        ''' <summary>
        ''' A linked list of objects
        ''' </summary>
        Public NotInheritable Class LinkedList : Inherits EvalObjectBase
            Private _value As System.Collections.Generic.LinkedList(Of Reference)

            Public ReadOnly Property Index As Integer = 0
            Public ReadOnly Property Count As Integer
                Get
                    Return _value.Count
                End Get
            End Property

            Private _node As LinkedListNode(Of Reference)

            Public Overrides Function GetValue() As Object
                Return _value
            End Function

            Public Overrides Sub SetValue(obj As Object)
                Me._value = New LinkedList(Of Reference)
                For Each r As Reference In DirectCast(obj, LinkedList(Of Reference))
                    Me._value.AddLast(New Reference(r))
                Next
                GoToFirst()
            End Sub

            ''' <summary>
            ''' Go to the first item in the linked list
            ''' </summary>
            Public Sub GoToFirst()
                _Index = 0
                If Count = 0 Then Return
                _node = _value.First
            End Sub

            ''' <summary>
            ''' Go to the last item in the linked list
            ''' </summary>
            Public Sub GoToLast()
                _Index = 0
                If Count = 0 Then Return
                _node = _value.Last
                _Index = _value.Count - 1
            End Sub

            ''' <summary>
            ''' Go to the next item in the linked list
            ''' </summary>
            Public Sub [Next]()
                If Count = 0 Then Return
                If _Index < _value.Count - 1 Then
                    _node = _node.Next
                    _Index += 1
                End If
            End Sub

            ''' <summary>
            ''' Go to the previous item in the linked list
            ''' </summary>
            Public Sub Previous()
                If Count = 0 Then Return
                If _Index > 0 Then
                    _node = _node.Previous
                    _Index -= 1
                End If
            End Sub

            ''' <summary>
            ''' Remove the current item from the linked list
            ''' </summary>
            Public Sub Remove()
                If Count > 0 Then
                    _value.Remove(_node)
                    If Count > 0 Then _node = _node.Next
                End If
            End Sub

            ''' <summary>
            ''' Remove the last item from the linked list
            ''' </summary>
            Public Sub RemoveLast()
                If Count > 0 Then _value.RemoveLast()
            End Sub

            ''' <summary>
            ''' Remove the first item from the linked list
            ''' </summary>
            Public Sub RemoveFirst()
                If Count > 0 Then _value.RemoveFirst()
            End Sub

            ''' <summary>
            ''' Get the current item in the linked list
            ''' </summary>
            Public Function Current() As Reference
                Return _node.Value
            End Function

            Public Shared Shadows Function IsType(obj As Object) As Boolean
                Return TypeOf obj Is LinkedList Or
                        TypeOf obj Is System.Collections.Generic.LinkedList(Of Reference)
            End Function

            Public Shared Shadows Function StrIsType(str As String) As Boolean
                str = str.Trim()
                Return str.StartsWith("linkedlist(") AndAlso str.EndsWith(")")
            End Function

            Protected Overrides Function DeepCopy() As EvalObjectBase
                Dim lst As New LinkedList(Of Reference)
                For Each r As Reference In _value
                    lst.AddLast(DirectCast(r.GetDeepCopy(), Reference))
                Next
                Return New LinkedList(lst)
            End Function

            Public Overrides Function ToString() As String
                Dim str As New StringBuilder("linkedlist([")
                Dim init As Boolean = True
                For Each r As Reference In _value
                    If Not init Then str.Append(", ") Else init = False
                    Dim ef As InternalFunctions = New InternalFunctions(New Evaluator(reloadDefault:=False))
                    str.Append(ef.O(r.GetRefObject()))
                Next
                str.Append("])")
                Return str.ToString()
            End Function

            Public Overrides Function Equals(other As EvalObjectBase) As Boolean
                Return False
            End Function

            Public Sub New(value As System.Collections.Generic.IList(Of Object))
                Me._value = New LinkedList(Of Reference)
                For Each obj As Object In value
                    Me._value.AddLast(New Reference(obj))
                Next
                GoToFirst()
            End Sub

            Public Sub New(value As System.Collections.Generic.IList(Of Reference))
                Me._value = New LinkedList(Of Reference)
                For Each r As Reference In value
                    Me._value.AddLast(New Reference(r))
                Next
                GoToFirst()
            End Sub

            Public Sub New(value As System.Collections.Generic.LinkedList(Of Reference))
                SetValue(value)
            End Sub

            Public Overrides Function GetHashCode() As Integer
                Return _value.GetHashCode()
            End Function

            ' to create from text, just use the normal linkedlist() internal function
        End Class

        ''' <summary>
        ''' A reference to another object
        ''' </summary>
        Public NotInheritable Class Reference : Inherits EvalObjectBase

            ''' <summary>
            ''' The linked list node that this refers to. Only used for linked lists.
            ''' </summary>
            Public ReadOnly Property Node As LinkedListNode(Of Reference) = Nothing

            Private _value As EvalObjectBase

            ''' <summary>
            ''' Get the value of the object pointed to by the reference
            ''' </summary>
            ''' <returns></returns>
            Public Overrides Function GetValue() As Object
                If _value Is Nothing Then Return Double.NaN
                Return _value.GetValue()
            End Function

            ''' <summary>
            ''' Get the object pointed to by the reference
            ''' </summary>
            ''' <returns></returns>
            Public Function GetRefObject() As EvalObjectBase
                Return _value
            End Function

            ''' <summary>
            ''' Get the final value pointed to by the reference, resolving any multiple indirection
            ''' </summary>
            ''' <returns></returns>
            Public Function Resolve() As Object
                Dim res As Object = _value
                While TypeOf res Is Reference AndAlso Not res Is DirectCast(res, Reference).GetValue()
                    res = CType(res, Reference).GetValue()
                End While
                If res Is Nothing Then Return Double.NaN
                If TypeOf res Is EvalObjectBase Then
                    res = CType(res, EvalObjectBase).GetValue()
                End If
                Return res
            End Function

            ''' <summary>
            ''' Get the final evaluator type value pointed to by the reference, resolves any multiple indirection
            ''' </summary>
            ''' <returns></returns>
            Public Function ResolveObj() As EvalObjectBase
                Dim res As EvalObjectBase = _value
                While TypeOf res Is Reference
                    res = CType(res, Reference).GetRefObject()
                End While
                Return res
            End Function

            ''' <summary>
            ''' Resolves any multiple indirection and returns the child reference that does not point at another reference
            ''' </summary>
            ''' <returns></returns>
            Public Function ResolveRef() As Reference
                Dim res As EvalObjectBase = _value
                If Not TypeOf res Is Reference Then Return Me
                While TypeOf CType(res, Reference).GetRefObject() Is Reference
                    res = CType(res, Reference).GetRefObject()
                End While
                Return CType(res, Reference)
            End Function

            ''' <summary>
            ''' Set the value pointed to by the reference, resolving any multiple indirection
            ''' </summary>
            Public Overrides Sub SetValue(obj As Object)
                Me.ResolveRef().SetRefObj(obj)
            End Sub

            ''' <summary>
            ''' Set the linked list node pointed to by the reference and the value to the value of that node
            ''' </summary>
            Public Sub SetNode(node As LinkedListNode(Of Reference))
                Me._Node = node
                Me._value = node.Value.ResolveObj()
            End Sub

            ''' <summary>
            ''' Adds an item after the node in the linked list, if available
            ''' </summary>
            Public Sub NodeAddAfter(value As Reference)
                Me._Node.List.AddAfter(_Node, value)
            End Sub

            ''' <summary>
            ''' Adds an item before the node in the linked list, if available
            ''' </summary>
            Public Sub NodeAddBefore(value As Reference)
                Me._Node.List.AddBefore(_Node, value)
            End Sub

            ''' <summary>
            ''' Create a new reference containing a new copy of the referenced object
            ''' </summary>
            ''' <returns></returns>
            Protected Overrides Function DeepCopy() As EvalObjectBase
                Dim obj As EvalObjectBase = _value.GetDeepCopy()
                If TypeOf obj Is Reference Then
                    Return obj
                Else
                    Return New Reference(obj)
                End If
            End Function


            ''' <summary>
            ''' Set the value pointed to by the reference
            ''' </summary>
            ''' <param name="obj"></param>
            Public Sub SetRefObj(obj As Object)
                If Not _value Is Nothing AndAlso _value.GetValue().GetType() = obj.GetType() Then
                    _value.SetValue(obj)
                Else
                    _value = ObjectTypes.DetectType(obj, True)
                End If
            End Sub

            ' linkedlist special stuff
            ''' <summary>
            ''' If this node is within a linked list, then this moves this reference forwards in the list
            ''' </summary>
            Public Sub NodeNext()
                Try
                    If Not Node Is Nothing Then
                        _Node = Node.Next
                        _value = Node.Value.GetRefObject()
                    End If
                Catch
                    _value = New Number(Double.NaN)
                End Try
            End Sub

            ''' <summary>
            ''' If this node is within a linked list, then this moves this reference backwards in the list
            ''' </summary>
            Public Sub NodePrevious()
                Try
                    If Not Node Is Nothing Then
                        _Node = Node.Previous
                        _value = Node.Value.GetRefObject()
                    End If
                Catch
                    _value = New Number(Double.NaN)
                End Try
            End Sub

            ''' <summary>
            ''' If this node is within a linked list, then this moves this reference to the front of the list
            ''' </summary>
            Public Sub NodeFirst()
                Try
                    If Not Node Is Nothing Then
                        While Not Node.Previous Is Nothing
                            _Node = Node.Previous
                        End While
                        _value = Node.Value.GetRefObject()
                    End If
                Catch
                    _value = New Number(Double.NaN)
                End Try
            End Sub

            ''' <summary>
            ''' If this node is within a linked list, then this moves this reference to the back of the list
            ''' </summary>
            Public Sub NodeLast()
                Try
                    If Not Node Is Nothing Then
                        While Not Node.Next Is Nothing
                            _Node = Node.Next
                        End While
                        _value = Node.Value.GetRefObject()
                    End If
                Catch
                    _value = New Number(Double.NaN)
                End Try
            End Sub

            ''' <summary>
            ''' If this node is within a linked list, then this removes the node referenced
            ''' </summary>
            Public Sub NodeRemove()
                Try
                    If Not Node Is Nothing Then
                        Dim tmp As LinkedListNode(Of Reference) = _Node.Next
                        _Node.List.Remove(Node)
                        _Node = tmp
                        _value = Node.Value.GetRefObject()
                    End If
                Catch
                    _value = New Number(Double.NaN)
                End Try
            End Sub

            ''' <summary>
            ''' If this node is within a linked list, then gets the list linked to by the node
            ''' </summary>
            Public Function NodeList() As LinkedList(Of Reference)
                Try
                    Return Node.List
                Catch
                    Return Nothing
                End Try
            End Function

            Public Shared Shadows Function IsType(obj As Object) As Boolean
                Return TypeOf obj Is Reference Or TypeOf obj Is Double Or TypeOf obj Is BigDecimal
            End Function
            Public Shared Shadows Function StrIsType(str As String) As Boolean
                Return False
            End Function

            Public Overrides Function ToString() As String
                If _value Is Nothing OrElse _value Is Me Then Return "Undefined"
                Return _value.ToString()
            End Function
            Public Overrides Function Equals(other As EvalObjectBase) As Boolean
                Return _value.Equals(other)
            End Function

            Public Sub New(value As EvalObjectBase)
                Me.Node = Nothing
                Me._value = value
            End Sub

            Public Sub New(value As Object)
                Me.Node = Nothing
                Me._value = ObjectTypes.DetectType(value, True)
            End Sub

            Public Sub New(node As LinkedListNode(Of Reference))
                Me.Node = node
                Me._value = New Reference(node.Value)
            End Sub

            Public Overrides Function GetHashCode() As Integer
                Return Me._value.GetHashCode()
            End Function
        End Class

        ''' <summary>
        ''' A lambda function/function pointer
        ''' </summary>
        Public NotInheritable Class Lambda : Inherits EvalObjectBase

            Public ReadOnly Property Args As IEnumerable(Of String)

            Private _value As String
            Private _fnPtr As Boolean
            Private _dispText As String

            ''' <summary>
            ''' Returns self
            ''' </summary>
            ''' <returns></returns>
            Public Overrides Function GetValue() As Object
                Return Me
            End Function

            Public Overrides Sub SetValue(obj As Object)
                If TypeOf obj Is Lambda Then
                    If obj.ToString().StartsWith("`") Then
                        Me.SetLambdaExprRaw(obj.ToString())
                    Else
                        Me.SetFunctionPtr(obj.ToString(), DirectCast(obj, Lambda).Args)
                    End If
                Else
                    Me.SetLambdaExprRaw(obj.ToString())
                End If
            End Sub

            ''' <summary>
            ''' Run this function on the specified evaluator and return the result
            ''' </summary>
            ''' <returns></returns>
            Public Function Execute(eval As Evaluator, args As IEnumerable(Of Object), Optional executingScope As String = "") As Object

                Dim tmpEval As Evaluator = eval.SubEvaluator()
                If executingScope <> "" Then tmpEval.Scope = executingScope

                For i As Integer = 0 To _Args.Count - 1
                    tmpEval.SetVariable(_Args(i), args(i))
                Next

                Dim res As Object = tmpEval.EvalRaw(_value, noSaveAns:=True)

                If TypeOf res Is Reference Then
                    Dim ref As Reference = DirectCast(res, Reference)
                    Return ref.GetRefObject()
                Else
                    Return res
                End If
            End Function

            ''' <summary>
            ''' Sets this lambda object to the function specified
            ''' </summary>
            Public Sub SetFunctionPtr(uf As String, args As IEnumerable(Of String))
                If uf.Contains("(") Then uf = uf.Remove(uf.IndexOf("("))
                Me._Args = args
                Me._value = uf & "("
                For Each a As String In args
                    If Not Me._value.EndsWith("(") Then Me._value &= ","
                    Me._value &= a
                Next
                Me._value &= ")"
                Me._fnPtr = True
            End Sub

            ''' <summary>
            ''' Sets this lambda object to the lambda expression specified
            ''' </summary>
            Public Sub SetLambdaExpr(expr As String, args As IEnumerable(Of String))
                Me._Args = args
                Me._value = expr
                Me._fnPtr = False
            End Sub

            ''' <summary>
            ''' Sets this lambda object to the lambda expression specified, in raw lambda expression notation
            ''' </summary>
            Public Sub SetLambdaExprRaw(lambda As String)
                If StrIsType(lambda) Then

                    lambda = lambda.Trim().Remove(lambda.Length - 1).Substring(1)
                    Dim args As String = ""
                    Dim expr As String = ""

                    If lambda.Contains("=>") Then
                        args = lambda.Remove(lambda.IndexOf("=>")).ToLowerInvariant()
                        expr = lambda.Substring(lambda.IndexOf("=>") + 2)
                    Else
                        expr = lambda
                    End If

                    If String.IsNullOrWhiteSpace(args) Then
                        Me.SetLambdaExpr(expr, New List(Of String))
                    Else
                        args = args.Trim().Trim({"("c, ")"c})
                        Me.SetLambdaExpr(expr, args.Split(","c))
                    End If
                    Me._fnPtr = False
                Else
                    Throw New SyntaxException(
                                "Invalid lambda expression (correct format: `var, var2, ...:expression` OR" &
                                " `var: expression` OR `expression`")
                End If
            End Sub

            Public Shared Shadows Function IsType(obj As Object) As Boolean
                Return TypeOf obj Is Lambda OrElse TypeOf obj Is UserFunction
            End Function

            Public Shared Shadows Function StrIsType(str As String) As Boolean
                str = str.Trim()
                Return str.StartsWith("`") AndAlso str.EndsWith("`")
            End Function

            Public Overrides Function ToString() As String
                If Me._fnPtr Then
                    Return Me._value.Remove(Me._value.IndexOf("("))
                Else
                    If Me.Args.Count = 0 Then
                        Return "`" & Me._value & "`"
                    Else
                        Return "`" & String.Join(",", Me.Args) & " => " & Me._value & "`"
                    End If
                End If
            End Function

            Public Overrides Function Equals(other As EvalObjectBase) As Boolean
                Return _value.Equals(other)
            End Function

            ''' <summary>
            ''' Create a new lambda expression directly from an expression and some arguments
            ''' </summary>
            ''' <param name="expr">Either the lambda expression or the function name</param>
            ''' <param name="args">The list of argument names</param>
            ''' <param name="fnptr">If true, intreprets as function pointer</param>
            Public Sub New(expr As String, args As IEnumerable(Of String), Optional fnptr As Boolean = False)
                If fnptr Then
                    Me.SetFunctionPtr(expr, args)
                Else
                    Me.SetLambdaExpr(expr, args)
                End If
            End Sub

            ''' <summary>
            ''' Create a lambda expression that functions as a function pointer to a user function. 
            ''' If flatten is set to true, creates a lambda expression
            ''' with the user function's content instead.
            ''' </summary>
            ''' <param name="uf"></param>
            ''' <param name="flatten"></param>
            Public Sub New(uf As UserFunction, Optional ByVal flatten As Boolean = True)
                If flatten Then
                    Me.SetLambdaExpr(uf.Body, uf.Args)
                Else
                    Me.SetFunctionPtr(uf.FullName, uf.Args)
                End If
            End Sub

            ''' <summary>
            ''' Create a new lambda expression from syntax
            ''' </summary>
            Public Sub New(lambda As String)
                Me.SetLambdaExprRaw(lambda)
            End Sub

            Public Overrides Function GetHashCode() As Integer
                Return Me._value.GetHashCode()
            End Function

            Protected Overrides Function DeepCopy() As EvalObjectBase
                Return New Lambda(Me._value, Me.Args, Me._fnPtr)
            End Function
        End Class

        ''' <summary>
        ''' An instance of a user-defined class
        ''' </summary>
        Public NotInheritable Class ClassInstance
            Inherits EvalObjectBase
            Implements IDisposable

            ''' <summary>
            ''' Class of this object
            ''' </summary>
            Public ReadOnly Property UserClass As UserClass

            ''' <summary>
            ''' Values of the object's fields
            ''' </summary>
            Public ReadOnly Property Fields As Dictionary(Of String, Reference)

            ''' <summary>
            ''' Gets the internal scope of this instance used to store fields, etc.
            ''' </summary>
            Public ReadOnly Property InnerScope As String

            ''' <summary>
            ''' Indicates if the instance is disposed
            ''' </summary>
            Private _disposed As Boolean

            ''' <summary>
            ''' Returns self
            ''' </summary>
            ''' <returns></returns>
            Public Overrides Function GetValue() As Object
                Return Me
            End Function

            ''' <summary>
            ''' Copy from another ClassInstance
            ''' </summary>
            Public Overrides Sub SetValue(obj As Object)
                If TypeOf obj Is ClassInstance Then
                    Me.Dispose()
                    Dim ci As ClassInstance = DirectCast(obj, ClassInstance)
                    Me._disposed = False
                    Me._UserClass = ci.UserClass
                    ci.UserClass.RegisterInstance(Me)

                    Me._Fields = New Dictionary(Of String, Reference)

                    Me.UserClass.Evaluator.SubScope("__instance_" & Me.UserClass.Name & "_" & RandomInstanceId())

                    Dim imported As Boolean = Me.UserClass.Evaluator.Imported.Contains(UserClass.InnerScope)
                    If Not imported Then Me.UserClass.Evaluator.Import(UserClass.InnerScope)

                    Try
                        Me._InnerScope = Me.UserClass.Evaluator.Scope

                        For Each f As KeyValuePair(Of String, Reference) In ci.Fields
                            Dim newVal As Reference
                            If UserClass.AllFields(f.Key).Modifiers.Contains("static") Then
                                newVal = DirectCast(f.Value, Reference)
                            Else
                                newVal = DirectCast(f.Value.GetDeepCopy(), Reference)
                            End If
                            Me.UserClass.Evaluator.SetVariable(Me.InnerScope & SCOPE_SEP & f.Key, newVal,
                                                               modifiers:={"internal"})
                            Me._Fields(f.Key) = Me.UserClass.Evaluator.GetVariableRef(Me.InnerScope & SCOPE_SEP & f.Key)
                        Next
                    Catch
                    End Try

                    If Not imported Then Me.UserClass.Evaluator.Unimport(UserClass.InnerScope)

                    Me.UserClass.Evaluator.ParentScope()
                    InitInstanceId()
                End If
            End Sub

            ''' <summary>
            ''' Recursively get the value of the field with the specified name under this instance
            ''' </summary>
            Public Function ResolveField(fieldName As String, scope As String) As Reference
                If String.IsNullOrWhiteSpace(fieldName) Then Throw New Exception("Field name cannot be blank")
                Try
                    Dim spl As String() = fieldName.Split(Evaluator.SCOPE_SEP)
                    Dim curVar As Reference = New Reference(Me)

                    For i As Integer = 0 To spl.Length - 1
                        If TypeOf curVar.GetRefObject() Is ClassInstance Then
                            Dim ci As ClassInstance = DirectCast(curVar.ResolveObj(), ClassInstance)
                            If Not ci.UserClass.AllFields(spl(i)).Modifiers.Contains("private") OrElse
                                Evaluator.IsParentScopeOf(ci.InnerScope, scope) Then
                                curVar = ci.Fields(spl(i))
                            Else
                                Throw New EvaluatorException("Field " & fieldName & " is private.")
                            End If
                        End If
                    Next

                    If TypeOf curVar.GetRefObject() Is Reference Then Return DirectCast(curVar.GetRefObject(), Reference)
                    Return curVar

                Catch ex As EvaluatorException
                    Throw ex
                Catch ex As Exception
                    Throw New Exception(fieldName & " is not a field of " & UserClass.Name)
                End Try
            End Function

            ''' <summary>
            ''' Get a random instance id
            ''' </summary>
            Private Function RandomInstanceId() As String
                Return Guid.NewGuid().ToString().Replace("-", "") & Now.Millisecond
            End Function
            Private Function GenerateSubscope() As String
                Return RandomInstanceId()
            End Function


            Public Shared Shadows Function IsType(obj As Object) As Boolean
                Return TypeOf obj Is ClassInstance OrElse TypeOf obj Is UserClass
            End Function

            Public Shared Shadows Function StrIsType(str As String) As Boolean
                Return False
            End Function

            Public Overrides Function ToString() As String
                If Me._disposed Then Return ""
                If Me.Fields.ContainsKey("text") AndAlso TypeOf Me.Fields("text").ResolveObj() Is Lambda Then
                    ' use the "text" function within the class definition
                    Dim tmpEval As Evaluator = UserClass.Evaluator.SubEvaluator()
                    tmpEval.Scope = Me.InnerScope
                    'tmpEval.Import(UserClass.InnerScope)
                    tmpEval.SubScope()
                    tmpEval.SetDefaultVariable(New Reference(Me))
                    Return UserClass.Evaluator.InternalFunctions.O(
                        DirectCast(Me.Fields("text").ResolveObj(), Lambda).Execute(tmpEval, {}, tmpEval.Scope))
                Else
                    ' default instance info
                    Return "<instance of ''" & Me.UserClass.Name & "'' with id " & Me.InnerScope & ">"
                End If
            End Function

            Public Overrides Function Equals(other As EvalObjectBase) As Boolean
                Return Me.UserClass.Name.Equals(other)
            End Function

            ''' <summary>
            ''' Set up the 'instanceid' member function
            ''' </summary>
            Private Sub InitInstanceId()
                ' add 'instaneid' function
                Dim iidFn As New UserFunction("type", String.Format("return " & ControlChars.Quote & Me.InnerScope &
                                                                    ControlChars.Quote,
                                                                    Evaluator.ROOT_NAMESPACE, Evaluator.SCOPE_SEP),
                                               New List(Of String), Me.InnerScope)
                iidFn.Modifiers.Add("internal")
                Me.Fields(iidFn.Name) = New Reference(New Lambda(iidFn, True))
            End Sub

            ''' <summary>
            ''' Create an identical class instance from another class instance
            ''' </summary>
            Public Sub New(ci As ClassInstance)
                Me.Fields = New Dictionary(Of String, Reference)()
                Me.SetValue(ci)
                InitInstanceId()
            End Sub

            ''' <summary>
            ''' Create a new instance of a class
            ''' </summary>
            Public Sub New(uc As UserClass)
                Me.Fields = New Dictionary(Of String, Reference)()
                Me.UserClass = uc
                uc.RegisterInstance(Me)

                uc.Evaluator.SubScope(
                    "__instance_" & Me.UserClass.Name & "_" & RandomInstanceId())
                Try
                    Me.InnerScope = uc.Evaluator.Scope

                    ' load default values 
                    For Each kvp As KeyValuePair(Of String, Variable) In uc.AllFields
                        Me.Fields(kvp.Key) = kvp.Value.Reference()
                        uc.Evaluator.SetVariable(kvp.Key, kvp.Value.Reference())
                    Next
                Catch
                End Try
                uc.Evaluator.ParentScope()
                InitInstanceId()
            End Sub

            Public Sub New(uc As UserClass, args As IEnumerable(Of Object))
                Me.Fields = New Dictionary(Of String, Reference)()
                Me.UserClass = uc
                uc.RegisterInstance(Me)

                Dim constructor As Lambda = uc.Constructor

                Dim tmpEval As Evaluator = uc.Evaluator.SubEvaluator()
                tmpEval.Scope = uc.Evaluator.Scope & SCOPE_SEP &
                                                          "__instance_" & Me.UserClass.Name & "_" & RandomInstanceId()
                Me.InnerScope = tmpEval.Scope

                tmpEval.Import(uc.InnerScope)

                ' load default values 
                For Each kvp As KeyValuePair(Of String, Variable) In uc.AllFields
                    Me.Fields(kvp.Key) = kvp.Value.Reference()
                    tmpEval.SetVariable(kvp.Key, kvp.Value.Reference())
                Next

                ' run constructor
                If constructor.Args.Count <> args.Count Then
                    Throw New EvaluatorException(
                            String.Format("{0} parameters expected For ''{1}'' constructor", constructor.Args.Count, uc.Name))
                End If

                tmpEval.SubScope()
                tmpEval.SetDefaultVariable(New Reference(Me))

                constructor.Execute(tmpEval, args, tmpEval.Scope)

                For Each var As Variable In tmpEval.Variables.Values
                    If var.DeclaringScope = Me.InnerScope Then
                        var.Modifiers.Add("internal")
                        uc.Evaluator.Variables(var.FullName) = var
                        Me.Fields(var.Name) = var.Reference
                    End If
                Next
                InitInstanceId()
            End Sub

            Public Overrides Function GetHashCode() As Integer
                If Me._disposed Then Throw New EvaluatorException("This user class instance is disposed")
                Return Me._disposed.GetHashCode()
            End Function

            ''' <summary>
            ''' Create a deep copy of this class instance
            ''' </summary>
            ''' <returns></returns>
            Protected Overrides Function DeepCopy() As EvalObjectBase
                If Me._disposed Then Throw New EvaluatorException("This user class instance is disposed")
                Return New ClassInstance(Me)
            End Function

            Public Sub Dispose() Implements IDisposable.Dispose
                If Me._disposed Then Return
                Me._disposed = True
                For Each f As String In Me.Fields.Keys
                    Me.UserClass.Evaluator.SetVariable(Me.InnerScope & SCOPE_SEP & f, Double.NaN)
                Next
                Me._UserClass = Nothing
            End Sub
        End Class
    End Class
End Namespace

Namespace Evaluator.CommonTypes
    ''' <summary>
    ''' Custom comparer that works for all types, used for our non type-specific sets, dictionaries, and sorts
    ''' </summary>
    Public NotInheritable Class GenericComparer
        Implements IComparer(Of Object)
        Implements IEqualityComparer(Of Object)
        Private Shared Function TypeToId(obj As Object) As Integer
            Select Case obj.GetType().Name
                Case "BigDecimal"
                    Return 0
                Case "Double"
                    Return 1
                Case "String"
                    Return 2
                Case "DateTime", "Date"
                    Return 3
                Case "TimeSpan"
                    Return 4
                Case Else
                    If obj.GetType().Name.StartsWith("List") Then
                        Return 5
                    ElseIf obj.GetType().Name.StartsWith("Dictionary") Then
                        Return 6
                    End If
                    Return 7
            End Select
        End Function

        ''' <summary>
        ''' Compare two lists by element
        ''' </summary>
        Public Shared Function CompareLists(a As IEnumerable(Of Reference), b As IEnumerable(Of Reference)) As Integer
            For i As Integer = 0 To Math.Min(a.Count, b.Count) - 1
                If CompareObjs(a(i), b(i)) <> 0 Then Return CompareObjs(a(i), b(i))
            Next
            Return CompareObjs(a.Count, b.Count)
        End Function

        ''' <summary>
        ''' Compare two linked lists by element
        ''' </summary>
        Public Shared Function CompareLinkedLists(a As LinkedList(Of Reference),
                                                  b As LinkedList(Of Reference)) As Integer
            For i As Integer = 0 To Math.Min(a.Count, b.Count) - 1
                If CompareObjs(a(i), b(i)) <> 0 Then Return CompareObjs(a(i), b(i))
            Next
            Return CompareObjs(a.Count, b.Count)
        End Function

        ''' <summary>
        ''' Compare two generic objects
        ''' </summary>
        ''' <param name="x"></param>
        ''' <param name="y"></param>
        ''' <returns></returns>
        Public Shared Function CompareObjs(x As Object, y As Object) As Integer
            While x.GetType().ToString().StartsWith("Cantus.Calculator.Evaluator.ObjectTypes") AndAlso
                Not x.GetType().ToString().EndsWith("[]")
                x = DirectCast(x, EvalObjectBase).GetValue()
            End While
            While y.GetType().ToString().StartsWith("Cantus.Calculator.Evaluator.ObjectTypes") AndAlso
                Not y.GetType().ToString().EndsWith("[]")
                y = DirectCast(y, EvalObjectBase).GetValue()
            End While

            If TypeToId(x) <> TypeToId(y) Then
                Return If(TypeToId(x) > TypeToId(y), 1, -1)

            ElseIf TypeOf x Is BigDecimal AndAlso TypeOf y Is BigDecimal
                Return CType(x, BigDecimal).CompareTo(CType(y, BigDecimal))
            ElseIf TypeOf x Is Double AndAlso TypeOf y Is Double
                Return CDbl(x).CompareTo(CDbl(y))
            ElseIf TypeOf x Is Integer AndAlso TypeOf y Is Integer
                Return CDbl(x).CompareTo(CDbl(y))
            ElseIf TypeOf x Is Boolean AndAlso TypeOf y Is Boolean
                Return CBool(x).CompareTo(CBool(y))

            ElseIf TypeOf x Is String AndAlso TypeOf y Is String
                Return CStr(x).CompareTo(CStr(y))
            ElseIf TypeOf x Is DateTime AndAlso TypeOf y Is DateTime
                Return CDate(x).CompareTo(CDate(y))
            ElseIf TypeOf x Is TimeSpan AndAlso TypeOf y Is TimeSpan
                Return CType(x, TimeSpan).CompareTo(CType(y, TimeSpan))

            ElseIf TypeOf x Is IList(Of Reference) AndAlso TypeOf y Is IList(Of Reference)
                Return CompareLists(CType(x, IList(Of Reference)), CType(y, IList(Of Reference)))

            ElseIf TypeOf x Is LinkedList(Of Reference) AndAlso TypeOf y Is LinkedList(Of Reference)
                Return CompareLinkedLists(CType(x, LinkedList(Of Reference)), CType(y, LinkedList(Of Reference)))

            ElseIf TypeOf x Is Reference() AndAlso TypeOf y Is Reference()
                Return CompareLists(CType(x, Reference()), CType(y, Reference()))

            ElseIf TypeOf x Is ReadOnlyCollection(Of Reference) AndAlso TypeOf y Is ReadOnlyCollection(Of Reference)
                Return CompareLists(CType(x, ReadOnlyCollection(Of Reference)), CType(y, ReadOnlyCollection(Of Reference)))

            ElseIf TypeOf x Is KeyValuePair(Of Object, Object) AndAlso TypeOf y Is KeyValuePair(Of Object, Object)
                Dim cmpKv As Integer = CompareObjs(CType(x, KeyValuePair(Of Object, Object)).Key,
                                                      CType(y, KeyValuePair(Of Object, Object)).Key)
                If cmpKv <> 0 Then Return cmpKv
                Return CompareObjs(CType(x, KeyValuePair(Of Object, Object)).Value,
                                      CType(y, KeyValuePair(Of Object, Object)).Value)

            ElseIf TypeOf x Is IDictionary(Of Reference, Reference) AndAlso
                TypeOf y Is IDictionary(Of Reference, Reference)

                Dim cmpDict As Integer = CompareLists(CType(x, IDictionary(Of Reference, Reference)).Keys.ToList(),
                     CType(y, IDictionary(Of Reference, Reference)).Keys.ToList())
                If cmpDict <> 0 Then Return cmpDict

                Try
                    Return CompareLists(CType(x, IDictionary(Of Reference, Reference)).Values.ToList(),
                     CType(y, IDictionary(Of Reference, Reference)).Values.ToList())
                Catch
                    Return 0
                End Try
            Else
                Return 1
            End If
        End Function

        ''' <summary>
        ''' Compare two generic objects
        ''' </summary>
        ''' <param name="x"></param>
        ''' <param name="y"></param>
        ''' <returns></returns>
        Public Function Compare(x As Object, y As Object) As Integer Implements IComparer(Of Object).Compare
            Return CompareObjs(x, y)
        End Function

        Private Shadows Function Equals(x As Object, y As Object) As Boolean Implements IEqualityComparer(Of Object).Equals
            Return CompareObjs(x, y) = 0
        End Function

        Public Shadows Function GetHashCode(obj As Object) As Integer Implements IEqualityComparer(Of Object).GetHashCode
            If TypeOf obj Is EvalObjectBase Then
                Return DirectCast(obj, EvalObjectBase).GetHashCode()
            Else
                Return obj.GetHashCode()
            End If
        End Function
    End Class
End Namespace
