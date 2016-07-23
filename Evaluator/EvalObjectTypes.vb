Imports System.Collections.ObjectModel
Imports System.Numerics
Imports System.Text
Imports Cantus.Calculator.Evaluator.CommonTypes
Imports Cantus.Calculator.Evaluator.Exceptions
Imports Cantus.Calculator.Evaluator.ObjectTypes

Namespace Calculator.Evaluator
    ' To define a new type for use with the evaluator, add a class implementing IEvalType in this namespace, 
    '    change the StrDetectType function and add converter functions as necessary

    Public Class ObjectTypes
        ''' <summary>
        ''' Automatically converts the object to a IEvalObject
        ''' </summary>
        ''' <param name="obj">The string</param>
        ''' <param name="identifierAsText">If true, parses strings into Text's instead of Identifiers</param>
        ''' <returns></returns>
        Public Shared Function DetectType(obj As Object, Optional identifierAsText As Boolean = False) As EvalObjectBase
            If obj.GetType().ToString().StartsWith("Cantus.Calculator.Evaluator.ObjectTypes") AndAlso Not obj.GetType().ToString().EndsWith("[]") Then
                Return DirectCast(obj, EvalObjectBase)
            End If
            If TypeOf obj Is BigDecimal Then
                Return New ObjectTypes.Number(CType(obj, BigDecimal))
            ElseIf TypeOf obj Is System.DateTime Then
                Return New ObjectTypes.DateTime(CDate(obj))
            ElseIf TypeOf obj Is TimeSpan Then
                Return New ObjectTypes.DateTime(CType(obj, TimeSpan))
            ElseIf TypeOf obj Is Double Then
                Return New ObjectTypes.Number(CDbl(obj))
            ElseIf ObjectTypes.Complex.IsType(obj) Then
                Return New ObjectTypes.Complex(CType(obj, Numerics.Complex))
            ElseIf ObjectTypes.Boolean.IsType(obj) Then
                Return New ObjectTypes.Boolean(CBool(obj))
            ElseIf ObjectTypes.Matrix.IsType(obj) Then
                Return New ObjectTypes.Matrix(CType(obj, List(Of Reference)))
            ElseIf ObjectTypes.Set.IsType(obj) Then
                Return New ObjectTypes.Set(CType(obj, SortedDictionary(Of Reference, Reference)))
            ElseIf ObjectTypes.Tuple.IsType(obj) Then
                Return New ObjectTypes.Tuple(CType(obj, Reference()))
            ElseIf identifierAsText AndAlso ObjectTypes.Text.IsType(obj) Then
                Return New ObjectTypes.Text(obj.ToString())
            ElseIf identifierAsText = False AndAlso ObjectTypes.Identifier.IsType(obj)
                Return New ObjectTypes.Identifier(obj.ToString())
            Else
                Return Nothing
            End If
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
                    If Matrix.StrIsType(str) Then
                        Return New Matrix(str, eval)
                    ElseIf [Set].StrIsType(str) Then
                        Return New [Set](str, eval)
                    ElseIf Complex.StrIsType(str) Then
                        Return New Complex(str, eval)
                    ElseIf Tuple.StrIsType(str) Then
                        Return New Tuple(str, eval)
                    ElseIf DateTime.StrIsType(str) Then
                        Return New Tuple(str, eval)
                    End If
                End If
                ' if none work then try text
                If identifierAsText AndAlso Text.StrIsType(str) AndAlso
                str.StartsWith(ControlChars.Quote) AndAlso str.EndsWith(ControlChars.Quote) Then
                    Return New Text(str)
                ElseIf Not identifierAsText Then 'AndAlso ObjectTypes.Identifier.StrIsType(str) Then
                    Return New Identifier(str)
                End If
                Return Nothing
            End If
        End Function

        Public Interface IEvalObject
            Function GetValue() As Object
            Sub SetValue(obj As Object)
            Function ToString() As String
            Function GetHashCode() As Integer
        End Interface

        Public MustInherit Class EvalObjectBase
            Inherits Object
            Implements IEvalObject
            Implements IEquatable(Of EvalObjectBase)
            Implements IComparable
            Implements IComparable(Of EvalObjectBase)

            Public MustOverride Function GetValue() As Object Implements IEvalObject.GetValue
            Public MustOverride Sub SetValue(obj As Object) Implements IEvalObject.SetValue

            Public Overrides Function ToString() As String Implements IEvalObject.ToString
                Return GetValue().ToString()
            End Function
            Public Shared Function IsType(obj As Object) As Boolean
                Return False
            End Function

            Public Shared Function StrType(str As String) As Boolean
                Return False
            End Function

            ''' <summary>
            ''' Generate a (usually) unique integer identifying the object
            ''' </summary>
            ''' <returns></returns>
            Public Overrides Function GetHashCode() As Integer Implements IEvalObject.GetHashCode
                Return GetValue().GetHashCode()
            End Function

            ''' <summary>
            ''' Create a brand new copy of this object, so that the new copy will not affect the old
            ''' </summary>
            ''' <returns></returns>
            Public Overridable Function DeepCopy() As EvalObjectBase
                Return ObjectTypes.DetectType(GetValue(), True)
            End Function

            Public MustOverride Shadows Function Equals(other As EvalObjectBase) As Boolean Implements IEquatable(Of EvalObjectBase).Equals

            Public Overridable Function CompareTo(other As Object) As Integer Implements IComparable.CompareTo
                Return CType(GetValue(), IComparable).CompareTo(other)
            End Function

            Public Overridable Function CompareTo(other As EvalObjectBase) As Integer Implements IComparable(Of EvalObjectBase).CompareTo
                Return CType(GetValue(), IComparable).CompareTo(other.GetValue())
            End Function
        End Class

        Public Class Number : Inherits EvalObjectBase
            Public value As BigDecimal
            Public Overrides Function GetValue() As Object
                Return CDbl(value)
            End Function
            Public Function BigDecValue() As BigDecimal
                Return value
            End Function
            Public Overrides Sub SetValue(obj As Object)
                value = CDbl(obj)
            End Sub
            Public Shared Shadows Function IsType(obj As Object) As Boolean
                Return TypeOf obj Is Number Or TypeOf obj Is Double Or TypeOf obj Is Integer Or TypeOf obj Is BigDecimal
            End Function
            Public Shared Function StrIsType(str As String) As Boolean
                Return str.ToLower() = "null" OrElse str.ToLower() = "undefined" OrElse Double.TryParse(str.Trim(), Nothing) OrElse str.Contains("e") AndAlso
                    Double.TryParse(str.Remove(str.IndexOf("e")).Trim(), Nothing) AndAlso
                    Double.TryParse(str.Substring(str.IndexOf("e") + 1).Trim(), Nothing)
            End Function

            Public Overrides Function Equals(other As EvalObjectBase) As Boolean
                If IsType(other) Then
                    Return CType(other, Number).BigDecValue() = value
                Else
                    Return False
                End If
            End Function

            Public Sub New(value As Double)
                Me.value = value
            End Sub
            Public Sub New(value As BigDecimal)
                Me.value = value
            End Sub
            Public Sub New(str As String)
                str = str.ToLowerInvariant()
                If str.ToLower() = "undefined" Then
                    Me.value = BigDecimal.Undefined
                ElseIf str.Contains("e") Then
                    Me.value = New BigDecimal(Double.Parse(str.Remove(str.IndexOf("e")))) * BigDecimal.Pow(10,
                                              Double.Parse(str.Substring(str.IndexOf("e") + 1)))
                Else
                    Try
                        Me.value = Double.Parse(str.Trim())
                    Catch
                        Me.value = BigDecimal.Undefined
                    End Try
                End If
            End Sub
        End Class

        Public Class Complex : Inherits EvalObjectBase
            Public value As Numerics.Complex
            Public Overrides Function GetValue() As Object
                Return value
            End Function
            Public Property Real() As Double
                Get
                    Return value.Real
                End Get
                Set(value As Double)
                    Me.value = New Numerics.Complex(value, Me.Imag)
                End Set
            End Property
            Public Property Imag() As Double
                Get
                    Return value.Imaginary
                End Get
                Set(value As Double)
                    Me.value = New Numerics.Complex(Me.Real, value)
                End Set
            End Property
            Public Overrides Sub SetValue(obj As Object)
                If Matrix.IsType(obj) Then
                    Dim lst As List(Of Reference) = CType(obj, List(Of Reference))
                    Me.value = New Numerics.Complex(CDbl(lst(0).GetValue()), CDbl(lst(1).GetValue()))
                ElseIf IsType(obj) Then
                    Me.value = CType(obj, Numerics.Complex)
                ElseIf Number.IsType(obj)
                    Me.value = CDbl(obj)
                End If
            End Sub

            Public Overrides Function DeepCopy() As EvalObjectBase
                Return New Complex(New Numerics.Complex(Real, Imag))
            End Function

            Public Shared Shadows Function IsType(obj As Object) As Boolean
                Return TypeOf obj Is Complex Or TypeOf obj Is Numerics.Complex
            End Function
            Public Shared Function StrIsType(str As String) As Boolean
                str = str.Trim()
                Return str.EndsWith(")") AndAlso str.StartsWith("(") AndAlso str.Contains("i") AndAlso
                    (str.Contains("+") OrElse str.Contains("-"))
            End Function
            Public Overrides Function ToString() As String
                Return String.Format("({0} {1} {2}i)", value.Real, If(value.Imaginary >= 0, "+", "-"),
                                    Math.Abs(value.Imaginary))
            End Function

            Public Overrides Function Equals(other As EvalObjectBase) As Boolean
                If IsType(other) Then
                    Return CType(other.GetValue(), Numerics.Complex) = value
                Else
                    Return False
                End If
            End Function

            Public Sub New(real As Double, Optional imag As Double = 0)
                Me.value = New Numerics.Complex(real, imag)
            End Sub
            Public Sub New(value As Numerics.Complex)
                Me.value = value
            End Sub
            Public Sub New(str As String, eval As Evaluator)
                If StrIsType(str) Then
                    str = str.Trim().Remove(str.Length - 1).Substring(1).Trim()
                    Dim split As String() = str.Split({" "c}, StringSplitOptions.RemoveEmptyEntries)

                    If split.Length <> 3 Then Throw New EvaluatorException("Invalid complex format")

                    Dim neg As Integer = If(split(1).Contains("-"), -1, 1)

                    Me.value = New Numerics.Complex(CDbl(CType(eval.EvalExprRaw(split(0), True), BigDecimal)),
                                                CDbl(neg * New Number(split(2).Replace("i", "").Trim()).BigDecValue()))
                End If
            End Sub
        End Class

        Public Class [Boolean] : Inherits EvalObjectBase
            Private value As Boolean
            Public Overrides Function GetValue() As Object
                Return value
            End Function
            Public Overrides Sub SetValue(obj As Object)
                value = CBool(obj)
            End Sub
            Public Shared Shadows Function IsType(obj As Object) As Boolean
                Return TypeOf obj Is [Boolean] Or TypeOf obj Is Boolean
            End Function
            Public Shared Function StrIsType(str As String) As Boolean
                str = str.Trim().ToLower()
                Return str = "true" OrElse str = "false"
            End Function

            Public Overrides Function Equals(other As EvalObjectBase) As Boolean
                If IsType(other) Then
                    Return CBool(other.GetValue()) = value
                Else
                    Return False
                End If
            End Function

            Public Sub New(value As Boolean)
                Me.value = value
            End Sub
            Public Sub New(str As String)
                Me.value = Boolean.Parse(str.Trim())
            End Sub
        End Class

        ''' <summary>
        ''' A piece of text
        ''' </summary>
        Public Class Text : Inherits EvalObjectBase
            Private value As String
            Public Overrides Function GetValue() As Object
                Return value
            End Function
            Public Overrides Sub SetValue(obj As Object)
                value = CStr(obj)
            End Sub
            Public Shared Shadows Function IsType(obj As Object) As Boolean
                Return TypeOf obj Is String Or TypeOf obj Is Text
            End Function
            Public Shared Function StrIsType(str As String) As Boolean
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

                For i As Integer = 0 To value.Length - 1
                    If escNxt Then
                        If raw Then
                            Select Case Char.ToLowerInvariant(value(i))
                                Case ControlChars.Quote, "'"c
                                    SetOrAppend(newstr, value(i), idx)
                                Case Else
                                    SetOrAppend(newstr, "\"c, idx)
                                    idx += 1
                                    SetOrAppend(newstr, value(i), idx)
                            End Select
                        Else
                            ' c-like escape sequence
                            Select Case Char.ToLowerInvariant(value(i))
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
                                    While i < value.Length AndAlso (
                                        (AscW(value(i)) >= AscW("0"c) AndAlso AscW(value(i)) <= AscW("9"c)) OrElse
                                        (AscW(value(i)) >= AscW("a"c) AndAlso AscW(value(i)) <= AscW("f"c)) OrElse
                                        (AscW(value(i)) >= AscW("A"c) AndAlso AscW(value(i)) <= AscW("F"c))) AndAlso
                                        charId.Length < 7
                                        charId &= Char.ToUpperInvariant(value(i))
                                        i += 1
                                    End While
                                    i -= 1
                                    SetOrAppend(newstr, ChrW(CInt(charId)), idx)
                                Case "0"c To "9"c
                                    Dim charId As String = "&O"
                                    While i < value.Length AndAlso charId.Length < 5 AndAlso
                                    (AscW(value(i)) >= AscW("0"c) AndAlso AscW(value(i)) <= AscW("7"c))
                                        charId &= Char.ToUpperInvariant(value(i))
                                        i += 1
                                    End While
                                    i -= 1
                                    SetOrAppend(newstr, ChrW(CInt(charId)), idx)
                                Case "d"c
                                    i += 1
                                    Dim charId As Integer = 0
                                    While i < value.Length AndAlso
                                    AscW(value(i)) >= AscW("0"c) AndAlso AscW(value(i)) <= AscW("9"c)
                                        charId = charId * 10 + AscW(value(i)) - AscW("0"c)
                                        i += 1
                                    End While
                                    i -= 1
                                    SetOrAppend(newstr, ChrW(charId), idx)
                                Case "u"c
                                    i += 1
                                    Dim charId As Integer = 0
                                    While i < value.Length AndAlso
                                    AscW(value(i)) >= AscW("0"c) AndAlso AscW(value(i)) <= AscW("9"c)
                                        charId = charId * 10 + AscW(value(i)) - AscW("0"c)
                                        i += 1
                                    End While
                                    i -= 1
                                    SetOrAppend(newstr, Char.ConvertFromUtf32(charId)(0), idx)
                                Case "\"c, ControlChars.Quote, "'"c, "?"c
                                    SetOrAppend(newstr, value(i), idx)
                                Case Else
                                    Throw New EvaluatorException("Invalid escape sequence")
                            End Select
                        End If
                        escNxt = False
                    ElseIf value(i) = "\" Then
                        If i = value.Length - 1 Then SetOrAppend(newstr, "\"c, idx) ' do not escape if this is the last character
                        escNxt = True
                    Else
                        SetOrAppend(newstr, value(i), idx)
                    End If
                    idx += 1
                Next
                Me.value = newstr.ToString()
                Return Me
            End Function

            Public Overrides Function ToString() As String
                Return ControlChars.Quote & value & ControlChars.Quote
            End Function

            Public Overrides Function Equals(other As EvalObjectBase) As Boolean
                If IsType(other) Then
                    Return CStr(other.GetValue()) = value
                Else
                    Return False
                End If
            End Function

            Public Sub New(value As String)
                If value.Length > 1 AndAlso (value.StartsWith(ControlChars.Quote) AndAlso
                        value.EndsWith(ControlChars.Quote) OrElse value.StartsWith("'") AndAlso value.EndsWith("'")) Then
                    Me.value = value.Substring(1, value.Length - 2)
                Else
                    Me.value = value
                End If
            End Sub
        End Class

        ''' <summary>
        ''' A piece of text that represents a function or variable
        ''' </summary>
        Public Class Identifier : Inherits EvalObjectBase
            Private value As String

            Public Overrides Function GetValue() As Object
                Return value
            End Function

            Public Overrides Sub SetValue(obj As Object)
                value = CStr(obj)
            End Sub

            Public Shared Shadows Function IsType(obj As Object) As Boolean
                Return TypeOf obj Is String Or TypeOf obj Is Identifier
            End Function

            Public Shared Function StrIsType(str As String) As Boolean
                If String.IsNullOrWhiteSpace(str.Trim()) Then Return False ' check if empty
                If Char.IsDigit(str(0)) Then Return False ' check if starts with number

                Dim disallowed As New HashSet(Of Char)("&+-*/{}[]()';^$@#!%=<>,:|\`~ ")
                For Each c As Char In str
                    If disallowed.Contains(c) Then Return False
                Next
                Return True
            End Function

            Public Overrides Function Equals(other As EvalObjectBase) As Boolean
                If IsType(other) Then
                    Return CStr(other.GetValue()) = value
                Else
                    Return False
                End If
            End Function

            Public Sub New(value As String)
                Me.value = value.Trim()
            End Sub
        End Class

        ''' <summary>
        ''' A single class that is able to represent both absolute points in time and time spans
        ''' </summary>
        Public Class DateTime : Inherits EvalObjectBase
            Private value As System.TimeSpan
            ''' <summary>
            ''' The date from which absolute datetimes are calculated
            ''' </summary>
            ''' <returns></returns>
            Public Shared ReadOnly Property BASE_DATE As System.DateTime = System.DateTime.MinValue
            ''' <summary>
            ''' The length of time in days after which absolute datetimes are returned instead of timespans
            ''' </summary>
            ''' <returns></returns>
            Public Shared ReadOnly Property TIMESPAN_DIVIDER As Integer = 365000
            Public Overrides Function GetValue() As Object
                If value.Days > TIMESPAN_DIVIDER Then
                    Return BASE_DATE.Add(value)
                End If
                Return value
            End Function
            Public Overrides Sub SetValue(obj As Object)
                If TypeOf obj Is DateTime Then
                    value = CType(obj, System.DateTime).Subtract(BASE_DATE)
                ElseIf TypeOf obj Is TimeSpan
                    value = CType(obj, TimeSpan)
                End If
            End Sub

            Public Overrides Function DeepCopy() As EvalObjectBase
                If TypeOf Me.GetValue() Is DateTime Then
                    Return New DateTime(New System.DateTime(CDate(Me.GetValue()).Ticks))
                Else
                    Return New DateTime(New System.TimeSpan(CType(Me.GetValue(), TimeSpan).Ticks))
                End If
            End Function

            Public Shared Shadows Function IsType(obj As Object) As Boolean
                Return TypeOf obj Is TimeZone Or TypeOf obj Is System.DateTime Or TypeOf obj Is DateTime
            End Function
            Public Shared Function StrIsType(str As String) As Boolean
                Return System.TimeSpan.TryParse(str.Trim(), Nothing) Or System.DateTime.TryParse(str.Trim(), Nothing)
            End Function

            Public Overrides Function ToString() As String
                If value.Days > TIMESPAN_DIVIDER Then
                    Return BASE_DATE.Add(value).ToString()
                End If
                Return value.ToString()
            End Function

            Public Overrides Function Equals(other As EvalObjectBase) As Boolean
                If IsType(other) Then
                    Dim val As Object = other.GetValue()
                    If TypeOf val Is TimeSpan Then
                        Return CType(val, TimeSpan) = value
                    ElseIf TypeOf val Is DateTime Then
                        Return CDate(val) = BASE_DATE.Add(value)
                    Else
                        Return False
                    End If
                Else
                    Return False
                End If
            End Function

            Public Sub New(value As System.TimeSpan)
                Me.value = value
            End Sub
            Public Sub New(value As System.DateTime)
                Me.value = value.Subtract(BASE_DATE)
            End Sub
            Public Sub New(str As String)
                If Not System.TimeSpan.TryParse(str.Trim(), Me.value) Then
                    Dim tmp As System.DateTime
                    System.DateTime.TryParse(str.Trim(), tmp)
                    Me.value = tmp.Subtract(BASE_DATE)
                End If
            End Sub
        End Class

        ''' <summary>
        ''' A fixed list of numbers
        ''' </summary>
        Public Class Tuple : Inherits EvalObjectBase
            Private value As List(Of Reference)
            Public Overrides Function GetValue() As Object
                Return value.ToArray()
            End Function
            Public Overrides Sub SetValue(obj As Object)
                If TypeOf obj Is List(Of Reference) Then
                    value = DirectCast(obj, List(Of Reference))
                Else
                    If TypeOf obj Is Tuple Then obj = DirectCast(obj, Tuple).GetValue()
                    If Not TypeOf obj Is Reference() Then obj = {New Reference(obj)}
                    Dim reflst As Reference() = DirectCast(obj, Reference())
                    For i As Integer = 0 To value.Count - 1
                        If reflst.Length <= i Then
                            value(i).SetValue(reflst(reflst.Length - 1))
                        Else
                            value(i).SetValue(reflst(i))
                        End If
                    Next
                End If
            End Sub

            Public Overrides Function DeepCopy() As EvalObjectBase
                Dim lst As New List(Of Reference)
                For Each r As Reference In value
                    lst.Add(DirectCast(r.DeepCopy(), Reference))
                Next
                Return New Tuple(lst)
            End Function

            Public Shared Shadows Function IsType(obj As Object) As Boolean
                Return TypeOf obj Is Tuple Or TypeOf obj Is Reference()
            End Function
            Public Shared Function StrIsType(str As String) As Boolean
                str = str.Trim()
                Return str.StartsWith("(") AndAlso str.EndsWith(")")
            End Function

            Public Overrides Function ToString() As String
                Dim str As String = "("
                For Each k As Object In value
                    If Not str = "(" Then str &= ", "
                    str &= k.ToString()
                Next
                str &= ")"
                Return str
            End Function

            Public Overrides Function Equals(other As EvalObjectBase) As Boolean
                Return False
            End Function

            Public Sub New(value As List(Of Reference))
                Me.value = value
            End Sub
            Public Sub New(value As Reference())
                Me.value = value.ToList()
            End Sub
            Public Sub New(value As IEnumerable(Of Object))
                Me.value = New List(Of Reference)()
                For Each v As Object In value
                    If TypeOf v Is Reference Then
                        Me.value.Add(DirectCast(v, Reference))
                    Else
                        Me.value.Add(New Reference(ObjectTypes.DetectType(v, True)))
                    End If
                Next
            End Sub
            Public Sub New(str As String, eval As Evaluator)
                'Try
                If StrIsType(str) Then
                    str = str.Trim().Remove(str.Length - 1).Substring(1).Trim(","c)
                    Me.value = New List(Of Reference)
                    If Not String.IsNullOrWhiteSpace(str) Then
                        Dim res As EvalObjectBase = ObjectTypes.DetectType(eval.EvalExprRaw("0," & str, True), True)
                        If IsType(res) Then
                            Dim lst As List(Of Reference) = CType(res.GetValue(), Reference()).ToList()
                            Me.value.AddRange(lst.GetRange(1, lst.Count - 1))
                        Else
                            Me.value.Add(New Reference(res))
                        End If
                    End If
                End If
                'Catch ex As Exception
                '    MsgBox(ex.ToString)
                'End Try
            End Sub
        End Class

        ''' <summary>
        ''' A vector or matrix 
        ''' </summary>
        Public Class Matrix : Inherits EvalObjectBase
            Private value As List(Of Reference)
            Public ReadOnly Property Height As Integer
                Get
                    Return value.Count
                End Get
            End Property
            Public ReadOnly Property Width As Integer
            Public ReadOnly Property Size As Size
                Get
                    Return New Size(Width, Height)
                End Get
            End Property

            Public Overrides Function GetValue() As Object
                Return value
            End Function

            Public Overrides Sub SetValue(obj As Object)
                value = DirectCast(obj, List(Of Reference))
                Me.Normalize()
            End Sub

            ''' <summary>
            ''' Get the object at the specified row and column in the matrix as a system type
            ''' </summary>
            ''' <returns></returns>
            Public Function GetCoord(row As Integer, col As Integer) As Object
                Dim r As EvalObjectBase = value(row).GetRefObject()
                If TypeOf r Is Matrix Then
                    Return CType(CType(r, Matrix).GetValue(), List(Of Reference))(col).GetValue()
                Else
                    If col = 0 Then
                        Return r.GetValue()
                    Else
                        Return Double.NaN
                    End If
                End If
            End Function

            ''' <summary>
            ''' Get the object at the specified row and column in the matrix as an evaluator type
            ''' </summary>
            ''' <returns></returns>
            Public Function GetCoordObj(row As Integer, col As Integer) As EvalObjectBase
                Dim r As EvalObjectBase = value(row).GetRefObject()
                If TypeOf r Is Matrix Then
                    Return CType(CType(r, Matrix).GetValue(), List(Of Reference))(col)
                Else
                    If col = 0 Then
                        Return r
                    Else
                        Return New Number(Double.NaN)
                    End If
                End If
            End Function

            ''' <summary>
            ''' Set the object at the specified row and column in the matrix
            ''' </summary>
            Public Sub SetCoord(row As Integer, col As Integer, obj As Object)
                Dim r As EvalObjectBase = value(row).ResolveObj()
                If TypeOf r Is Matrix Then
                    CType(CType(r, Matrix).GetValue(), List(Of Reference))(col).SetValue(obj)
                Else
                    If TypeOf obj Is Reference Then obj = DirectCast(obj, Reference).Resolve()
                    If col = 0 Then
                        r.SetValue(obj)
                    End If
                End If
            End Sub

            ''' <summary>
            ''' Get the transpose of this matrix
            ''' </summary>
            ''' <returns></returns>
            Public Function Transpose() As Matrix
                Me.Normalize()
                Dim mat As Matrix = DirectCast(Me.DeepCopy(), Matrix)
                Dim m2 As New Matrix(DirectCast(DirectCast(Me.DeepCopy(), Matrix).GetValue(), List(Of Reference)))
                mat.Resize(Width, Height)
                For i As Integer = 0 To mat.Height - 1
                    For j As Integer = 0 To mat.Width - 1
                        mat.SetCoord(i, j, m2.GetCoord(j, i))
                    Next
                Next
                Return mat
            End Function

            Public Function Determinant() As Object
                ' base cases
                If Width <> Height Then Return Double.NaN ' can only calculate det for square matrices
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
                    Me.value = res
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
                For row As Integer = 0 To Height - 1
                    Dim currow As New List(Of Reference)
                    For col As Integer = 0 To Width - 1
                        Dim cur As Object = GetCoord(row, col)
                        If TypeOf cur Is Numerics.Complex OrElse TypeOf b Is Numerics.Complex Then
                            If Not TypeOf cur Is Numerics.Complex Then cur = New Numerics.Complex(CDbl(cur), 0)
                            If Not TypeOf b Is Numerics.Complex Then b = New Numerics.Complex(CDbl(b), 0)
                            SetCoord(row, col, CType(cur, Numerics.Complex) * CType(b, Numerics.Complex))
                        ElseIf TypeOf cur Is Double OrElse TypeOf b Is Double Then
                            SetCoord(row, col, CDbl(cur) * CDbl(b))
                        ElseIf TypeOf cur Is BigDecimal OrElse TypeOf b Is BigDecimal Then
                            SetCoord(row, col, CType(cur, BigDecimal) * CType(b, BigDecimal))
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
                    Dim a As Object = value(i).Resolve()
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

                    Dim result As EvalObjectBase = ObjectTypes.DetectType(New Evaluator().EvalExprRaw(
                                                              evalStr.ToString(), True))
                    If TypeOf result Is Reference Then
                        newValue.Add(DirectCast(result, Reference))
                    Else
                        newValue.Add(New Reference(result))
                    End If
                Next
                value = newValue
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
                    Dim a As Object = GetCoord(0, i)
                    If TypeOf result Is Numerics.Complex OrElse TypeOf a Is Numerics.Complex Then
                        If Not TypeOf a Is Numerics.Complex Then a = New Numerics.Complex(CDbl(a), 0)
                        If Not TypeOf result Is Numerics.Complex Then a = New Numerics.Complex(CDbl(result), 0)
                        result = DirectCast(result, Numerics.Complex) + DirectCast(a, Numerics.Complex) * DirectCast(a, Numerics.Complex)

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
                    If TypeOf Me.value(i).ResolveObj() Is Matrix Then
                        tmp.Add(DirectCast(Me.value(i).Resolve(), List(Of Reference))(id))
                    ElseIf TypeOf Me.value(i).ResolveObj() Is Reference Then
                        tmp.Add(DirectCast(Me.value(i).ResolveObj(), Reference))
                    Else
                        tmp.Add(New Reference(Me.value(i).ResolveObj()))
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
                If TypeOf Me.value(id).ResolveObj() Is Matrix Then
                    Return DirectCast(Me.value(id).ResolveObj(), Matrix)
                Else
                    Return New Matrix({Me.value(id).ResolveObj()})
                End If
            End Function

            ''' <summary>
            ''' Swap two matrix rows and return the matrix
            ''' </summary>
            ''' <param name="aug">Matrix representing right side of augmented matrix, if available</param>
            ''' <returns></returns>
            Public Function SwapRows(a As Integer, b As Integer, Optional aug As Matrix = Nothing) As Matrix
                Dim tmp As Reference = value(a)
                value(a) = value(b)
                value(b) = tmp
                If Not aug Is Nothing Then aug.SwapRows(a, b)
                Return Me
            End Function

            ''' <summary>
            ''' Swap two matrix columns and return the matrix
            ''' </summary>
            ''' <returns></returns>
            Public Function SwapCols(a As Integer, b As Integer) As Matrix
                For i As Integer = 0 To Height - 1
                    If TypeOf Me.value(i).ResolveObj() Is Matrix Then
                        Dim tmp As Reference
                        tmp = DirectCast(Me.value(i).Resolve(), List(Of Reference))(b)
                        DirectCast(Me.value(i).Resolve(), List(Of Reference))(b) = DirectCast(Me.value(i).Resolve(), List(Of Reference))(a)
                        DirectCast(Me.value(i).Resolve(), List(Of Reference))(a) = tmp
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
            Public Sub ScaleRow(row As Integer, scale As Object, Optional aug As Matrix = Nothing)
                For i As Integer = 0 To Width - 1
                    Dim orig As Object = GetCoord(row, i)
                    If TypeOf scale Is Numerics.Complex OrElse TypeOf orig Is Numerics.Complex Then
                        If Not TypeOf scale Is Numerics.Complex Then scale = New Numerics.Complex(CDbl(scale), 0)
                        If Not TypeOf orig Is Numerics.Complex Then orig = New Numerics.Complex(CDbl(orig), 0)
                        SetCoord(row, i, New Reference(DirectCast(orig, Numerics.Complex) * DirectCast(scale, Numerics.Complex)))
                    ElseIf TypeOf scale Is Double AndAlso TypeOf orig Is Double
                        SetCoord(row, i, New Reference(CDbl(orig) * CDbl(scale)))
                    ElseIf TypeOf scale Is BigDecimal AndAlso TypeOf orig Is BigDecimal
                        SetCoord(row, i, New Reference(DirectCast(orig, BigDecimal) * DirectCast(scale, BigDecimal)))
                    End If
                Next
                If Not aug Is Nothing Then aug.ScaleRow(row, scale)
            End Sub

            ''' <summary>
            ''' Subtract row b from row a and assign the values to row a
            ''' </summary>
            ''' <param name="aug">Right side of augmented matrix, if applicable</param>
            Public Sub SubtractRow(a As Integer, b As Integer, Optional aug As Matrix = Nothing)
                For i As Integer = 0 To Width - 1
                    Dim av As Object = GetCoord(a, i)
                    Dim bv As Object = GetCoord(b, i)
                    If TypeOf av Is Numerics.Complex OrElse TypeOf bv Is Numerics.Complex Then
                        If Not TypeOf av Is Numerics.Complex Then av = New Numerics.Complex(CDbl(av), 0)
                        If Not TypeOf bv Is Numerics.Complex Then bv = New Numerics.Complex(CDbl(bv), 0)
                        SetCoord(a, i, DirectCast(av, Numerics.Complex) - DirectCast(bv, Numerics.Complex))
                    ElseIf TypeOf av Is Double AndAlso TypeOf bv Is Double
                        SetCoord(a, i, New Reference(Math.Round(CDbl(av) - CDbl(bv), 12)))
                    ElseIf TypeOf av Is BigDecimal AndAlso TypeOf bv Is BigDecimal
                        SetCoord(a, i, New Reference(DirectCast(av, BigDecimal) - DirectCast(bv, BigDecimal)))
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
                    Return 1 / CDbl(a)
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
            ''' <param name="aug">Matrix to modify along with the current matrix as an augmented matrix</param>
            ''' <returns></returns>
            Public Function Rref(Optional aug As Matrix = Nothing) As Matrix
                ' deep copy everything before doing anything to avoid messing up due to references
                Dim mat As Matrix = DirectCast(Me.DeepCopy(), Matrix)

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
                        If ((TypeOf val Is Double OrElse TypeOf val Is BigDecimal) AndAlso CDbl(val) <> 0.0) OrElse
                                TypeOf val Is Numerics.Complex AndAlso Math.Round(DirectCast(val, Numerics.Complex).Magnitude, 12) <> 0 Then
                            mat.SwapRows(curRow, swapRow, aug)
                            mat.ScaleRow(curRow, AutoReciprocal(val), aug)
                            pivot(curRow) = col
                            Exit For
                        End If
                    Next

                    If Not success Then Continue For

                    For zeroOutRow As Integer = 0 To Height - 1
                        If zeroOutRow = curRow Then Continue For
                        Dim val As Object = mat.GetCoord(zeroOutRow, col)
                        If ((TypeOf val Is Double OrElse TypeOf val Is BigDecimal) AndAlso CDbl(val) <> 0.0) OrElse
                                TypeOf val Is Numerics.Complex AndAlso Math.Round(DirectCast(val, Numerics.Complex).Magnitude, 12) <> 0 Then
                            mat.ScaleRow(zeroOutRow, AutoReciprocal(val), aug)
                            mat.SubtractRow(zeroOutRow, curRow, aug)

                            ' if the row was already processed then we need to scale its pivot back to one
                            If zeroOutRow < curRow Then
                                mat.ScaleRow(zeroOutRow, AutoReciprocal(mat.GetCoord(zeroOutRow, pivot(zeroOutRow))), aug)
                            End If
                        End If
                    Next
                    curRow += 1
                Next
                While curRow < mat.Height
                    mat.ScaleRow(curRow, 0, aug)
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
                Dim l As Matrix = DirectCast(Me.DeepCopy(), Matrix)
                l.Rref(r)
                If Not l.IsIdentityMatrix() Then Return New Matrix({Double.NaN})
                Me.value = DirectCast(r.GetValue(), List(Of Reference))
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
                Dim origmat As Matrix = DirectCast(Me.DeepCopy(), Matrix)

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

            Public Overrides Function DeepCopy() As EvalObjectBase
                Dim lst As New List(Of Reference)
                For Each r As Reference In value
                    lst.Add(DirectCast(r.DeepCopy(), Reference))
                Next
                Return New Matrix(lst)
            End Function

            Public Shared Shadows Function IsType(obj As Object) As Boolean
                Return TypeOf obj Is Matrix Or TypeOf obj Is System.Collections.Generic.List(Of Reference)
            End Function

            Public Shared Function StrIsType(str As String) As Boolean
                str = str.Trim()
                Return str.StartsWith("[") AndAlso str.EndsWith("]")
            End Function

            Public Overrides Function ToString() As String
                Dim str As String = "["
                For Each k As Reference In value
                    If Not str = "[" Then str &= ", "
                    str &= k.ToString()
                Next
                str &= "]"
                Return str
            End Function

            Public Overrides Function Equals(other As EvalObjectBase) As Boolean
                If TypeOf other Is Matrix Then
                    Return GenericComparer.CompareLists(value, CType(other.GetValue(), List(Of Reference))) = 0
                Else
                    Return False
                End If
            End Function

            ''' <summary>
            ''' Make this a proper matrix by making all the rows and columns the same length, respectively
            ''' </summary>
            Public Sub Normalize()
                Dim max_len As Integer = 0
                For Each r As Reference In value
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
                    value.Add(New Reference(0.0))
                End While
                While Me.Height > height
                    value.RemoveAt(value.Count - 1)
                End While

                ' fit width
                If width > 1 Then
                    For i As Integer = 0 To value.Count - 1
                        Dim r As Reference = value(i)
                        If Not TypeOf r.GetRefObject() Is Matrix Then
                            r.SetValue(New Matrix({r.GetValue()}))
                        End If
                        Dim inner As List(Of Reference) = CType(CType(r.GetRefObject(), Matrix).GetValue(), List(Of Reference))
                        While inner.Count < width
                            inner.Add(New Reference(0.0))
                        End While
                        While inner.Count > width
                            inner.RemoveAt(inner.Count - 1)
                        End While
                    Next
                End If
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
                            If Math.Round(CDbl(obj), 12) <> expected Then Return False
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
                Me.value = New List(Of Reference)
                Me.Resize(rows, cols)
            End Sub

            ''' <summary>
            ''' Create a new matrix from a list of references
            ''' </summary>
            Public Sub New(value As System.Collections.Generic.IEnumerable(Of Reference))
                Me.value = value.ToList()
                Me.Normalize()
            End Sub

            ''' <summary>
            ''' Create a new matrix from a list of evaluator objects
            ''' </summary>
            Public Sub New(value As System.Collections.Generic.IEnumerable(Of EvalObjectBase))
                Me.value = New List(Of Reference)()
                For Each v As EvalObjectBase In value
                    If TypeOf v Is Reference Then
                        Me.value.Add(DirectCast(v, Reference))
                    Else
                        Me.value.Add(New Reference(v))
                    End If
                Next
                Me.Normalize()
            End Sub

            ''' <summary>
            ''' Create a new matrix from a list of system objects
            ''' </summary>
            Public Sub New(value As System.Collections.Generic.IEnumerable(Of Object))
                Me.value = New List(Of Reference)()
                For Each v As Object In value
                    Me.value.Add(New Reference(ObjectTypes.DetectType(v, True)))
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
                        Me.value = New List(Of Reference)
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

                            Dim res As Object = eval.EvalExprRaw("0," & str, True)

                            If Tuple.IsType(res) Then
                                Dim lst As List(Of Reference) = CType(res, Reference()).ToList()
                                Me.value.AddRange(lst.GetRange(1, lst.Count - 1))
                            Else
                                Me.value.Add(New Reference(res))
                            End If
                        End If
                    End If

                    Me.Normalize()
                Catch 'ex As Exception
                    'MsgBox(ex.ToString)
                End Try
            End Sub
        End Class

        ''' <summary>
        ''' A dictionary of objects
        ''' </summary>
        Public Class [Set] : Inherits EvalObjectBase
            Private value As System.Collections.Generic.SortedDictionary(Of Reference, Reference)
            Public Overrides Function GetValue() As Object
                Return value
            End Function
            Public Overrides Sub SetValue(obj As Object)
                value = DirectCast(obj, SortedDictionary(Of Reference, Reference))
            End Sub
            Public Shared Shadows Function IsType(obj As Object) As Boolean
                Return TypeOf obj Is [Set] Or
                    TypeOf obj Is System.Collections.Generic.SortedDictionary(Of Reference, Reference) Or
                    TypeOf obj Is System.Collections.Generic.Dictionary(Of Object, Object)
            End Function
            Public Shared Function StrIsType(str As String) As Boolean
                str = str.Trim()
                Return str.StartsWith("{") AndAlso str.EndsWith("}")
            End Function

            Public Overrides Function DeepCopy() As EvalObjectBase
                Dim dict As New SortedDictionary(Of Reference, Reference)(New GenericComparer())
                For Each k As KeyValuePair(Of Reference, Reference) In value
                    Dim key As Reference = DirectCast(k.Key.DeepCopy(), Reference)
                    If k.Value Is Nothing Then
                        dict(key) = Nothing
                    Else
                        dict(key) = DirectCast(k.Value.DeepCopy(), Reference)
                    End If
                Next
                Return New [Set](dict)
            End Function

            Private Sub ConvertFrom(obj As EvalObjectBase)
                If TypeOf obj Is [Set] Then
                    value = New SortedDictionary(Of Reference, Reference)(
                        CType(obj.GetValue(), SortedDictionary(Of Reference, Reference)), New GenericComparer())
                ElseIf TypeOf obj Is Matrix Then
                    value = New SortedDictionary(Of Reference, Reference)(New GenericComparer())
                    For Each o As EvalObjectBase In CType(obj.GetValue(), List(Of Reference))
                        value(New Reference(o)) = Nothing
                    Next
                Else
                    value = New SortedDictionary(Of Reference, Reference)(New GenericComparer())
                    value(New Reference(obj)) = Nothing
                End If
            End Sub
            Public Overrides Function ToString() As String
                Dim str As String = "{"
                For Each k As KeyValuePair(Of Reference, Reference) In value
                    If Not str = "{" Then str &= ", "
                    Dim ef As InternalFunctions = New InternalFunctions(New Evaluator())
                    str &= ef.O(k.Key)
                    If Not k.Value Is Nothing Then
                        str &= ":" & ef.O(k.Value)
                    End If
                Next
                str &= "}"
                Return str
            End Function

            Public Overrides Function Equals(other As EvalObjectBase) As Boolean
                Return False
            End Function

            Public Sub New(value As System.Collections.Generic.IEnumerable(Of Reference))
                Me.ConvertFrom(New Matrix(value))
            End Sub
            Public Sub New(value As System.Collections.Generic.IDictionary(Of Reference, Reference))
                Me.value = New SortedDictionary(Of Reference, Reference)(value, New GenericComparer())
            End Sub
            Public Sub New(value As System.Collections.Generic.IDictionary(Of Object, Object))
                Me.value = New SortedDictionary(Of Reference, Reference)()
                For Each k As KeyValuePair(Of Object, Object) In value
                    Me.value(New Reference(ObjectTypes.DetectType(k.Key, True))) = New Reference(ObjectTypes.DetectType(k.Value, True))
                Next
            End Sub
            Public Sub New(str As String, eval As Evaluator)
                Try
                    If StrIsType(str) Then
                        str = str.Trim().Remove(str.Length - 1).Substring(1).Trim(","c)
                        Me.value = New SortedDictionary(Of Reference, Reference)(New GenericComparer())
                        If String.IsNullOrWhiteSpace(str) Then Return
                        Dim lst As New List(Of Reference)(CType(New Tuple("(" & str & ")", eval).GetValue(), Reference()))
                        For Each o As EvalObjectBase In lst
                            If TypeOf o Is Reference Then o = CType(o, Reference).GetRefObject()
                            If Tuple.IsType(o) Then
                                Dim innerlst As Reference() = CType(o.GetValue(), Reference())
                                If innerlst.Count = 2 Then
                                    Me.value(innerlst(0)) = innerlst(1)
                                    Continue For
                                End If
                            End If
                            Me.value(New Reference(o)) = Nothing
                        Next
                    End If
                Catch 'ex As Exception
                End Try
            End Sub
        End Class

        ' a reference to another object
        Public Class Reference : Inherits EvalObjectBase
            Public value As EvalObjectBase

            ''' <summary>
            ''' Get the (system type) value pointed to by the reference
            ''' </summary>
            ''' <returns></returns>
            Public Overrides Function GetValue() As Object
                If value Is Nothing Then Return Double.NaN
                Return value.GetValue()
            End Function

            ''' <summary>
            ''' Get the (evaluator type) value pointed to by the reference
            ''' </summary>
            ''' <returns></returns>
            Public Function GetRefObject() As EvalObjectBase
                Return value
            End Function

            ''' <summary>
            ''' Get the (system type) value pointed to by the reference and resolves any multiple indirection
            ''' </summary>
            ''' <returns></returns>
            Public Function Resolve() As Object
                Dim res As Object = value
                While TypeOf res Is Reference
                    res = CType(res, Reference).GetValue()
                End While
                If res Is Nothing Then Return Double.NaN
                If TypeOf res Is EvalObjectBase Then
                    res = CType(res, EvalObjectBase).GetValue()
                End If
                Return res
            End Function

            ''' <summary>
            ''' Get the (evaluator type) value pointed to by the reference and resolves any multiple indirection
            ''' </summary>
            ''' <returns></returns>
            Public Function ResolveObj() As EvalObjectBase
                Dim res As EvalObjectBase = value
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
                Dim res As EvalObjectBase = value
                If Not TypeOf res Is Reference Then Return Me
                While TypeOf CType(res, Reference).GetRefObject() Is Reference
                    res = CType(res, Reference).GetRefObject()
                End While
                Return CType(res, Reference)
            End Function

            ''' <summary>
            ''' Create a new reference containing a new copy of the referenced object
            ''' </summary>
            ''' <returns></returns>
            Public Overrides Function DeepCopy() As EvalObjectBase
                Dim obj As EvalObjectBase = value.DeepCopy()
                If TypeOf obj Is Reference Then
                    Return obj
                Else
                    Return New Reference(obj)
                End If
            End Function

            ''' <summary>
            ''' Set the value pointed to by the reference, resolving any multiple indirection
            ''' </summary>
            ''' <param name="obj"></param>
            Public Overrides Sub SetValue(obj As Object)
                Me.ResolveRef().SetRefObj(obj)
            End Sub

            ''' <summary>
            ''' Set the value pointed to by the reference
            ''' </summary>
            ''' <param name="obj"></param>
            Public Sub SetRefObj(obj As Object)
                If Not value Is Nothing AndAlso value.GetValue().GetType() = obj.GetType() Then
                    value.SetValue(obj)
                Else
                    value = ObjectTypes.DetectType(obj, True)
                End If
            End Sub

            Public Shared Shadows Function IsType(obj As Object) As Boolean
                Return TypeOf obj Is Reference Or TypeOf obj Is Double Or TypeOf obj Is BigDecimal
            End Function
            Public Shared Function StrIsType(str As String) As Boolean
                Return False
            End Function

            Public Overrides Function ToString() As String
                If value Is Nothing Then Return "NaN"
                Return value.ToString()
            End Function
            Public Overrides Function Equals(other As EvalObjectBase) As Boolean
                Return value.Equals(other)
            End Function

            Public Sub New(value As EvalObjectBase)
                Me.value = value
            End Sub
            Public Sub New(value As Object)
                Me.value = ObjectTypes.DetectType(value, True)
            End Sub
        End Class
    End Class
End Namespace

Namespace Calculator.Evaluator.CommonTypes
    ''' <summary>
    ''' Custom comparer that works for all types used for the dictionary
    ''' </summary>
    Public Class GenericComparer
        Implements IComparer(Of Object)
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
        ''' <param name="a"></param>
        ''' <param name="b"></param>
        ''' <returns></returns>
        Public Shared Function CompareLists(a As IEnumerable(Of EvalObjectBase), b As IEnumerable(Of EvalObjectBase)) As Integer
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

            ElseIf TypeOf x Is List(Of Reference) AndAlso TypeOf y Is List(Of Reference)
                Return CompareLists(CType(x, List(Of Reference)), CType(y, List(Of Reference)))

            ElseIf TypeOf x Is Reference() AndAlso TypeOf y Is Reference()
                Return CompareLists(CType(x, Reference()), CType(y, Reference()))

            ElseIf TypeOf x Is ReadOnlyCollection(Of EvalObjectBase) AndAlso TypeOf y Is ReadOnlyCollection(Of EvalObjectBase)
                Return CompareLists(CType(x, ReadOnlyCollection(Of EvalObjectBase)), CType(y, ReadOnlyCollection(Of EvalObjectBase)))

            ElseIf TypeOf x Is KeyValuePair(Of Object, Object) AndAlso TypeOf y Is KeyValuePair(Of Object, Object)
                Dim cmpKv As Integer = CompareObjs(CType(x, KeyValuePair(Of Object, Object)).Key,
                                                      CType(y, KeyValuePair(Of Object, Object)).Key)
                If cmpKv <> 0 Then Return cmpKv
                Return CompareObjs(CType(x, KeyValuePair(Of Object, Object)).Value,
                                      CType(y, KeyValuePair(Of Object, Object)).Value)

            ElseIf TypeOf x Is SortedDictionary(Of Reference, Reference) AndAlso
                TypeOf y Is SortedDictionary(Of Reference, Reference)

                Dim cmpDict As Integer = CompareLists(CType(x, SortedDictionary(Of Reference, Reference)).Keys.ToList(),
                     CType(y, SortedDictionary(Of Reference, Reference)).Keys.ToList())
                If cmpDict <> 0 Then Return cmpDict

                Return CompareLists(CType(x, SortedDictionary(Of Reference, Reference)).Values.ToList(),
                     CType(y, SortedDictionary(Of Reference, Reference)).Values.ToList())

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
    End Class
End Namespace
