Imports System.Text
Imports Cantus.Calculator.Evaluator.CommonTypes
Imports Cantus.Calculator.Evaluator.Exceptions
Imports Cantus.Calculator.Evaluator.ObjectTypes
Imports System.Collections.Specialized
Imports System.Net
Imports System.Security.Cryptography
Imports System.IO
Imports System.Threading
Imports System.Text.RegularExpressions
Imports Cantus.Calculator.Evaluator.Evaluator

Namespace Calculator.Evaluator
    ' Define functions here (name is case insensitive)
    ' All public functions are directly accessible when evaluating an expressionession (though may be overrided by user functions)
    ' All private and friend functions are hidden
    Public Class InternalFunctions
        Private _eval As Evaluator
        Private _curThreadId As Integer

        Public Sub New(parent As Evaluator)
            Me._eval = parent
        End Sub

        ' evaluator management

        ''' <summary>
        ''' Reload all constants, clears all variables and userfunctions, and clears imports
        ''' </summary>
        Public Sub AllClear()
            _eval.Clear()
            _eval.ReloadDefault()
        End Sub

        ''' <summary>
        ''' Reload default constants. if name is specified, reloads constant with that name only.
        ''' </summary>
        Public Sub Reload(Optional name As String = "")
            _eval.ReloadDefault(name)
        End Sub

        ''' <summary>
        ''' Get the previous nth answer
        ''' </summary>
        Public Function PrevAns(index As Double) As Object
            Return _eval.PrevAns(Int(_eval.PrevAns.Count - index - 1))
        End Function

        ' modes
        ''' <summary>
        ''' Get or set the output format of this evaluator
        ''' </summary>
        Public Function _Output(Optional ByVal val As String = "") As String
            If val <> "" Then
                Try
                    val = val.ToLower()
                    ' shorthands
                    If val = "sci" Then
                        val = "Scientific"
                    End If
                    ' do not allow numbers
                    If Double.TryParse(val, Nothing) Then Throw New EvaluatorException()
                    _eval.OutputFormat = DirectCast([Enum].Parse(GetType(eOutputFormat), val, True), eOutputFormat)
                Catch
                    Throw New EvaluatorException(val & " is not a valid output mode. Choices are: 
                        Raw, Math, Scientific (Sci)")
                End Try
            End If
            Return _eval.OutputFormat.ToString()
        End Function

        ''' <summary>
        ''' Get or set the angle representation mode of this evaluator
        ''' </summary>
        Public Function _AngleRepr(Optional ByVal val As String = "") As String
            If val <> "" Then
                val = val.ToLower()
                ' shorthands
                If val = "deg" Then
                    val = "Degree"
                ElseIf val = "rad"
                    val = "Radian"
                ElseIf val = "grad"
                    val = "Gradian"
                End If
                Try
                    _eval.AngleMode = DirectCast([Enum].Parse(GetType(eAngleRepresentation), val, True), eAngleRepresentation)
                Catch
                    Throw New EvaluatorException(val & " is not a valid angle representation mode." &
                    "Choices are: Radian (Rad), Degree (Deg), Gradian (Grad)")
                End Try
            End If
            Return _eval.AngleMode.ToString()
        End Function

        ''' <summary>
        ''' Get or set the number of spaces per tab of this evaluator
        ''' </summary>
        Public Function _SpacesPerTab(Optional ByVal val As Double = Double.NaN) As Double
            If val >= 0 Then
                _eval.SpacesPerTab = Int(val)
            ElseIf val < 0 AndAlso Not Double.IsNaN(val) Then
                Throw New EvaluatorException("Number of spaces per tab may not be negative.")
            End If
            Return _eval.SpacesPerTab
        End Function

        ''' <summary>
        ''' Get or set the explicit mode
        ''' </summary>
        Public Function _Explicit(Optional ByVal val As Boolean? = Nothing) As Boolean
            If Not val Is Nothing Then
                _eval.ExplicitMode = CBool(val)
            End If
            Return _eval.ExplicitMode
        End Function

        ''' <summary>
        ''' Get the scope
        ''' </summary>
        Public Function _Scope() As String
            Return _eval.Scope
        End Function

        ''' <summary>
        ''' Read the definition of a User Functions
        ''' </summary>
        ''' <param name="name"></param>
        ''' <returns></returns>
        Public Function RecallUF(name As String) As String
            Try
                Return _eval.UserFunctions(name).ToString(_eval.Scope)
            Catch
                Return "Function ''" & name & "'' is undefined"
            End Try
        End Function

        ''' <summary>
        ''' Copy onto the clipboard the definition of a User Functions
        ''' </summary>
        ''' <param name="name"></param>
        ''' <returns></returns>
        Public Function CopyUF(name As String) As Object
            Try
                Return Clipboard(_eval.UserFunctions(name).ToString(_eval.Scope))
            Catch
                Return "Function ''" & name & "''is undefined"
            End Try
        End Function

        ''' <summary>
        ''' Set or get the clipboard object
        ''' </summary>
        Public Function Clipboard(Optional obj As Object = Nothing) As Object
            If obj Is Nothing Then
                Dim result As Object = Double.NaN
                Dim th As New Thread(Sub()
                                         Try
                                             result = ObjectTypes.StrDetectType(My.Computer.Clipboard.GetText(),
                                                                           _eval, True, False).GetValue()
                                         Catch
                                         End Try
                                     End Sub)
                th.SetApartmentState(ApartmentState.STA)
                th.Start()
                th.Join()
                Return result
            Else
                Dim th As New Thread(Sub()
                                         Try
                                             If String.IsNullOrEmpty(obj.ToString()) Then
                                                 My.Computer.Clipboard.Clear()
                                             Else
                                                 My.Computer.Clipboard.SetText(obj.ToString())
                                             End If
                                         Catch
                                         End Try
                                     End Sub)
                th.SetApartmentState(ApartmentState.STA)
                th.Start()

                Return obj.ToString() & ": copied to clipboard"
            End If
        End Function

        ' convert degree, radians, gradians
        Public Function DToR(ByVal v As Double) As Double
            Return v * Math.PI / 180
        End Function
        Public Function RToD(ByVal v As Double) As Double
            Return v / Math.PI * 180
        End Function
        Public Function RToG(ByVal v As Double) As Double
            Return v / Math.PI * 200
        End Function
        Public Function DToG(ByVal v As Double) As Double
            Return v / 9 * 10
        End Function
        Public Function GToD(ByVal v As Double) As Double
            Return v / 10 * 9
        End Function
        Public Function GToR(ByVal v As Double) As Double
            Return v / 200 * Math.PI
        End Function

        ' trig functions
        Public Function Sin(ByVal v As Object) As Object
            If TypeOf v Is Double Then
                Select Case _eval.AngleMode
                    Case Evaluator.eAngleRepresentation.Degree
                        Return SinD(CDbl(v))
                    Case Evaluator.eAngleRepresentation.Radian
                        Return SinR(CDbl(v))
                    Case Evaluator.eAngleRepresentation.Gradian
                        Return SinG(CDbl(v))
                    Case Else
                        Return Double.NaN
                End Select
            ElseIf TypeOf v Is Numerics.Complex
                Return Numerics.Complex.Sin(CType(v, Numerics.Complex))
            Else
                Return Double.NaN
            End If
        End Function
        Public Function Cos(ByVal v As Object) As Object
            If TypeOf v Is Double Then
                Select Case _eval.AngleMode
                    Case Evaluator.eAngleRepresentation.Degree
                        Return CosD(CDbl(v))
                    Case Evaluator.eAngleRepresentation.Radian
                        Return CosR(CDbl(v))
                    Case Evaluator.eAngleRepresentation.Gradian
                        Return CosG(CDbl(v))
                    Case Else
                        Return Double.NaN
                End Select
            ElseIf TypeOf v Is Numerics.Complex
                Return Numerics.Complex.Cos(CType(v, Numerics.Complex))
            Else
                Return Double.NaN
            End If
        End Function
        Public Function Tan(ByVal v As Object) As Object
            If TypeOf v Is Double Then
                Select Case _eval.AngleMode
                    Case Evaluator.eAngleRepresentation.Degree
                        Return TanD(CDbl(v))
                    Case Evaluator.eAngleRepresentation.Radian
                        Return TanR(CDbl(v))
                    Case Evaluator.eAngleRepresentation.Gradian
                        Return TanG(CDbl(v))
                    Case Else
                        Return Double.NaN
                End Select
            ElseIf TypeOf v Is Numerics.Complex
                Return Numerics.Complex.Tan(CType(v, Numerics.Complex))
            Else
                Return Double.NaN
            End If
        End Function
        Public Function Cot(ByVal v As Object) As Object
            If TypeOf v Is Double Then
                Return 1 / CDbl(Tan(CDbl(v)))
            ElseIf TypeOf v Is Numerics.Complex
                Return 1 / CDbl(Tan(CType(v, Numerics.Complex)))
            Else
                Return Double.NaN
            End If
        End Function
        Public Function Sec(ByVal v As Object) As Object
            If TypeOf v Is Double Then
                Return 1 / CDbl(Cos(CDbl(v)))
            ElseIf TypeOf v Is Numerics.Complex
                Return 1 / CDbl(Cos(CType(v, Numerics.Complex)))
            Else
                Return Double.NaN
            End If
        End Function
        Public Function Csc(ByVal v As Object) As Object
            If TypeOf v Is Double Then
                Return 1 / CDbl(Sin(CDbl(v)))
            ElseIf TypeOf v Is Numerics.Complex
                Return 1 / CDbl(Sin(CType(v, Numerics.Complex)))
            Else
                Return Double.NaN
            End If
        End Function

        ' specific trig functions
        Public Function SinD(ByVal v As Double) As Double
            Dim deg As Double = DToR(v)
            deg = Math.Sin(deg)
            Return Math.Round(deg, 11)
        End Function
        Public Function CosD(ByVal v As Double) As Double
            Dim deg As Double = DToR(v)
            deg = Math.Cos(deg)
            Return Math.Round(deg, 11)
        End Function
        Public Function TanD(ByVal v As Double) As Double
            Dim deg As Double = DToR(v)
            deg = Math.Tan(deg)
            Return Math.Round(deg, 11)
        End Function
        Public Function CotD(ByVal v As Double) As Double
            Return 1 / TanD(v)
        End Function
        Public Function SecD(ByVal v As Double) As Double
            Return 1 / CosD(v)
        End Function
        Public Function CscD(ByVal v As Double) As Double
            Return 1 / SinD(v)
        End Function
        Public Function SinR(ByVal v As Double) As Double
            Return Math.Round(Math.Sin(v), 9)
        End Function
        Public Function CosR(ByVal v As Double) As Double
            Return Math.Round(Math.Cos(v), 9)
        End Function
        Public Function TanR(ByVal v As Double) As Double
            Return Math.Round(Math.Tan(v), 9)
        End Function
        Public Function CotR(ByVal v As Double) As Double
            Return 1 / TanR(v)
        End Function
        Public Function SecR(ByVal v As Double) As Double
            Return 1 / CosR(v)
        End Function
        Public Function CscR(ByVal v As Double) As Double
            Return 1 / SinR(v)
        End Function
        Public Function SinG(ByVal v As Double) As Double
            Return Math.Round(Math.Sin(GToR(v)), 11)
        End Function
        Public Function CosG(ByVal v As Double) As Double
            Return Math.Round(Math.Cos(GToR(v)), 11)
        End Function
        Public Function TanG(ByVal v As Double) As Double
            Return Math.Round(Math.Tan(GToR(v)), 11)
        End Function
        Public Function CotG(ByVal v As Double) As Double
            Return 1 / TanG(v)
        End Function
        Public Function SecG(ByVal v As Double) As Double
            Return 1 / CosG(v)
        End Function
        Public Function CscG(ByVal v As Double) As Double
            Return 1 / SinG(v)
        End Function

        Public Function Asin(ByVal v As Object) As Object
            If TypeOf v Is Numerics.Complex Then
                Return Numerics.Complex.Asin(CType(v, Numerics.Complex))
            ElseIf TypeOf v Is Double
                Select Case _eval.AngleMode
                    Case Evaluator.eAngleRepresentation.Degree
                        Return Asind(CDbl(v))
                    Case Evaluator.eAngleRepresentation.Radian
                        Return Asinr(CDbl(v))
                    Case Evaluator.eAngleRepresentation.Gradian
                        Return Asing(CDbl(v))
                    Case Else
                        Return Double.NaN
                End Select
            Else
                Return Double.NaN
            End If
        End Function

        Public Function Acos(ByVal v As Object) As Object
            If TypeOf v Is Numerics.Complex Then
                Return Numerics.Complex.Acos(CType(v, Numerics.Complex))
            ElseIf TypeOf v Is Double
                Select Case _eval.AngleMode
                    Case Evaluator.eAngleRepresentation.Degree
                        Return Acosd(CDbl(v))
                    Case Evaluator.eAngleRepresentation.Radian
                        Return Acosr(CDbl(v))
                    Case Evaluator.eAngleRepresentation.Gradian
                        Return Acosg(CDbl(v))
                    Case Else
                        Return Double.NaN
                End Select
            Else
                Return Double.NaN
            End If
        End Function
        Public Function Asind(ByVal v As Double) As Double
            Dim deg As Double = Math.Asin(v) / Math.PI * 180
            Return Math.Round(deg, 11)
        End Function
        Public Function Acosd(ByVal v As Double) As Double
            Dim deg As Double = Math.Acos(v) / Math.PI * 180
            Return Math.Round(deg, 11)
        End Function
        Public Function Asinr(ByVal v As Double) As Double
            Return Math.Round(Math.Asin(v), 11)
        End Function
        Public Function Acosr(ByVal v As Double) As Double
            Return Math.Round(Math.Acos(v), 11)
        End Function
        Public Function Asing(ByVal v As Double) As Double
            Return Math.Round(RToG(Math.Asin(v)), 11)
        End Function
        Public Function Acosg(ByVal v As Double) As Double
            Return Math.Round(RToG(Math.Acos(v)), 11)
        End Function

        Public Function Atan(ByVal v As Object) As Object
            If TypeOf v Is Numerics.Complex Then
                Return Numerics.Complex.Atan(CType(v, Numerics.Complex))
            ElseIf TypeOf v Is Double
                Select Case _eval.AngleMode
                    Case Evaluator.eAngleRepresentation.Degree
                        Return Atand(CDbl(v))
                    Case Evaluator.eAngleRepresentation.Radian
                        Return Atanr(CDbl(v))
                    Case Evaluator.eAngleRepresentation.Gradian
                        Return Atang(CDbl(v))
                    Case Else
                        Return Double.NaN
                End Select
            Else
                Return Double.NaN
            End If
        End Function
        Public Function Atand(ByVal v As Double) As Double
            Return Math.Atan(v) / Math.PI * 180
        End Function
        Public Function Atanr(ByVal v As Double) As Double
            Return Math.Atan(v)
        End Function
        Public Function Atang(ByVal v As Double) As Double
            Return RToG(Math.Atan(v))
        End Function

        Public Function Sinh(ByVal v As Object) As Object
            If TypeOf v Is Numerics.Complex Then
                Return Numerics.Complex.Sinh(CType(v, Numerics.Complex))
            ElseIf TypeOf v Is Double
                Select Case _eval.AngleMode
                    Case Evaluator.eAngleRepresentation.Degree
                        Return SinhD(CDbl(v))
                    Case Evaluator.eAngleRepresentation.Radian
                        Return SinhR(CDbl(v))
                    Case Evaluator.eAngleRepresentation.Gradian
                        Return SinhG(CDbl(v))
                    Case Else
                        Return Double.NaN
                End Select
            Else
                Return Double.NaN
            End If
        End Function

        Public Function Tanh(ByVal v As Object) As Object
            If TypeOf v Is Numerics.Complex Then
                Return Numerics.Complex.Tanh(CType(v, Numerics.Complex))
            ElseIf TypeOf v Is Double
                Select Case _eval.AngleMode
                    Case Evaluator.eAngleRepresentation.Degree
                        Return TanhD(CDbl(v))
                    Case Evaluator.eAngleRepresentation.Radian
                        Return TanhR(CDbl(v))
                    Case Evaluator.eAngleRepresentation.Gradian
                        Return TanhG(CDbl(v))
                    Case Else
                        Return Double.NaN
                End Select
            Else
                Return Double.NaN
            End If
        End Function

        Public Function Cosh(ByVal v As Object) As Object
            If TypeOf v Is Numerics.Complex Then
                Return Numerics.Complex.Cosh(CType(v, Numerics.Complex))
            ElseIf TypeOf v Is Double
                Select Case _eval.AngleMode
                    Case Evaluator.eAngleRepresentation.Degree
                        Return CoshD(CDbl(v))
                    Case Evaluator.eAngleRepresentation.Radian
                        Return CoshR(CDbl(v))
                    Case Evaluator.eAngleRepresentation.Gradian
                        Return CoshG(CDbl(v))
                    Case Else
                        Return Double.NaN
                End Select
            Else
                Return Double.NaN
            End If
        End Function
        Public Function SinhD(ByVal v As Double) As Double
            Return Math.Sinh(DToR(v))
        End Function
        Public Function TanhD(ByVal v As Double) As Double
            Return Math.Tanh(DToR(v))
        End Function
        Public Function CoshD(ByVal v As Double) As Double
            Return Math.Cosh(DToR(v))
        End Function
        Public Function SinhR(ByVal v As Double) As Double
            Return Math.Sinh(v)
        End Function
        Public Function TanhR(ByVal v As Double) As Double
            Return Math.Tanh(v)
        End Function
        Public Function CoshR(ByVal v As Double) As Double
            Return Math.Cosh(v)
        End Function
        Public Function SinhG(ByVal v As Double) As Double
            Return Math.Sinh(GToR(v))
        End Function
        Public Function TanhG(ByVal v As Double) As Double
            Return Math.Tanh(GToR(v))
        End Function
        Public Function CoshG(ByVal v As Double) As Double
            Return Math.Cosh(GToR(v))
        End Function

        Public Function Exp(ByVal v As Object) As Object
            If TypeOf v Is Double Then
                Return Math.Exp(CDbl(v))
            ElseIf TypeOf v Is Numerics.Complex
                Return Numerics.Complex.Exp(CType(v, Numerics.Complex))
            Else
                Throw New EvaluatorException("Invalid exp")
            End If
        End Function

        Public Function Pow(ByVal value1 As Object, ByVal value2 As Object) As Object
            If TypeOf value1 Is Double AndAlso TypeOf value2 Is Double Then
                Return BigDecimal.Pow(CDbl(value1), CDbl(value2))

            ElseIf TypeOf value1 Is Numerics.Complex
                If TypeOf value2 Is BigDecimal OrElse TypeOf value2 Is Double Then value2 = New Numerics.Complex(CDbl(value2), 0)
                Return Numerics.Complex.Pow(CType(value1, Numerics.Complex), CType(value2, Numerics.Complex))

            ElseIf TypeOf value1 Is IEnumerable(Of Reference) AndAlso TypeOf value2 Is Double OrElse TypeOf value2 Is BigDecimal
                Return New Matrix(DirectCast(value1, IEnumerable(Of Reference))).Expo(Int(CDbl(value2))).GetValue()

            Else
                Throw New EvaluatorException("Invalid pow")
            End If
        End Function

        Public Function Sqrt(ByVal v As Object) As Object
            If TypeOf v Is Double Then
                If CDbl(v) >= 0 Then
                    Return Math.Sqrt(CDbl(v))
                Else
                    Return New Numerics.Complex(0, Numerics.Complex.Sqrt(CDbl(v)).Imaginary)
                End If
            ElseIf TypeOf v Is Numerics.Complex Then
                Return Numerics.Complex.Sqrt(CType(v, Numerics.Complex))
            Else
                Return Double.NaN
            End If
        End Function

        Public Function Cbrt(ByVal v As Object) As Object
            If TypeOf v Is Double Then
                Return If(CDbl(v) < 0, -1, 1) * Math.Pow(Math.Abs(CDbl(v)), 1 / 3)
            ElseIf TypeOf v Is Numerics.Complex Then
                Return Numerics.Complex.Pow(CType(v, Numerics.Complex), 1 / 3)
            Else
                Return Double.NaN
            End If
        End Function

        Public Function Root(ByVal value1 As Object, ByVal value2 As Object) As Object
            If TypeOf value1 Is Double AndAlso TypeOf value2 Is Double Then
                If CmpDbl(CDbl(value2) Mod 2, 0) = 1 Then ' handle negative odd roots which are otherwise undefined
                    Return If(CDbl(value1) < 0, -1, 1) * Math.Pow(Math.Abs(CDbl(value1)), 1 / CDbl(value2))
                Else
                    Return Math.Pow(CDbl(value1), 1 / CDbl(value2))
                End If
            ElseIf TypeOf value1 Is Numerics.Complex And TypeOf value2 Is Double Then
                Return Numerics.Complex.Pow(CType(value1, Numerics.Complex), 1 / CDbl(value2))
            Else
                Return Double.NaN
            End If
        End Function

        ''' <summary>
        ''' Calculates the factorial of a number using an approximation of the gamma function
        ''' </summary>
        Public Function Factorial(ByVal value As Double) As Double
            Return Math.Round(Gamma(value + 1), 3)
        End Function

        ''' Gamma function (Use Lanczos approximation)
        ''' From wikipedia "Lanczos approximation" page
        ''' Originally in Python, translated by author to VB.NET
        Public Function Gamma(ByVal z As Numerics.Complex) As Double
            Try
                Dim epsi As Double = 0.0000001

                Dim p As Double() = {
            676.5203681218851, -1259.1392167224028, 771.32342877765313,
            -176.61502916214059, 12.507343278686905, -0.13857109526572012,
            0.0000099843695780195716, 0.00000015056327351493116}
                Dim result As New Numerics.Complex()
                If z.Real < 0.5 Then
                    result = Math.PI / (Numerics.Complex.Sin(Math.PI * z) * Gamma(1 - z))
                Else
                    z -= 1
                    Dim x As New Numerics.Complex(0.99999999999980993, 0)

                    For i As Integer = 0 To p.Length - 1
                        x += p(i) / (z + i + 1)
                    Next

                    Dim t As Numerics.Complex = z + p.Length - 0.5
                    result = Math.Sqrt(2 * Math.PI) * Numerics.Complex.Pow(t, (z + 0.5)) _
                   * Numerics.Complex.Exp(-t) * x
                End If
                If CmpDbl(result.Imaginary, 0, epsi) = 0 Then
                    Return result.Real
                End If
            Catch
                Return Double.NaN
            End Try
            Return Double.NaN
        End Function

        ''' <summary>
        ''' Check if a number is prime. Deterministic for small numbers under 10^16
        ''' </summary>
        ''' <param name="n"></param>
        ''' <returns></returns>
        Public Function IsPrime(n As Double) As Boolean
            ' First test trivial cases
            If Double.IsNaN(n) OrElse Double.IsInfinity(n) OrElse n <= 1 OrElse n Mod 2 = 0 Then Return False

            Try
                ' Then try using the Miller-Rabin algorithm
                If Not MillerRabin(CLng(Math.Floor(n)), 20) Then Return False
            Catch
            End Try

            ' Try brute forcing 
            Dim max As Integer = 100000001
            If max > Math.Sqrt(n) Then max = Int(Math.Sqrt(n))
            For i As Integer = 3 To max
                If n Mod i = 0 Then Return False
            Next

            Return True
        End Function

        ''' <summary>
        ''' Miller-Rabin primality test, from RosettaCode
        ''' </summary>
        ''' <param name="n"></param>
        ''' <param name="k"></param>
        ''' <returns></returns>
        Private Function MillerRabin(n As Long, k As Integer) As Boolean
            If n < 2 Then
                Return False
            End If
            If n <> 2 AndAlso n Mod 2 = 0 Then
                Return False
            End If
            Dim s As Integer = Int(n - 1)
            While s Mod 2 = 0
                s >>= 1
            End While
            Dim r As New Random()
            For i As Integer = 0 To k - 1
                Dim a As Double = r.Next(Int(n - 1)) + 1
                Dim temp As Integer = s
                Dim modulo As Long = CLng(Math.Pow(a, temp)) Mod n
                While temp <> n - 1 AndAlso modulo <> 1 AndAlso modulo <> n - 1
                    modulo = (modulo * modulo) Mod n
                    temp = temp * 2
                End While
                If modulo <> n - 1 AndAlso temp Mod 2 = 0 Then
                    Return False
                End If
            Next
            Return True
        End Function

        ' combinatorics
        ''' <summary>
        ''' Compute combinations
        ''' </summary>
        Public Function Comb(ByVal n As Double, ByVal r As Double) As Double
            Dim sum As Double = 0
            n = Math.Truncate(n)
            If CmpDbl(n, 0) = 0 AndAlso r < 1 AndAlso r >= 0 Then Return 1 ' (0 0) = 1
            For i As Long = 0 To CLng(Math.Truncate(r - 1))
                sum += Math.Log10(n - i)
                sum -= Math.Log10(i + 1)
            Next
            Return Math.Round(Math.Pow(10, sum))
        End Function

        ''' <summary>
        ''' Compute combinations
        ''' </summary>
        Public Function Choose(ByVal n As Double, ByVal k As Double) As Double
            Return Comb(n, k)
        End Function

        ''' <summary>
        ''' Compute combinations
        ''' </summary>
        Public Function NCr(ByVal n As Double, ByVal k As Double) As Double
            Return Comb(n, k)
        End Function

        ''' <summary>
        ''' Compute permutations
        ''' </summary>
        Public Function Perm(ByVal n As Double, ByVal k As Double) As Double
            Dim sum As Double = 0
            n = Math.Truncate(n)
            If CmpDbl(n, 0) = 0 AndAlso k < 1 AndAlso k >= 0 Then Return 1 ' (0 0) = 1
            For i As Long = 0 To CLng(Math.Truncate(k - 1))
                sum += Math.Log10(n - i)
            Next
            Return Math.Round(Math.Pow(10, sum))
        End Function

        ''' <summary>
        ''' Compute permutations
        ''' </summary>
        Public Function nPr(ByVal n As Double, ByVal k As Double) As Double
            Return Perm(n, k)
        End Function
        Public Function GCF(ByVal v1 As Double, ByVal v2 As Double) As Double
            Try
                'convert to integers
                Dim i1 As ULong = Convert.ToUInt64(Abs(v1))
                Dim i2 As ULong = Convert.ToUInt64(Abs(v2))
                Dim r As ULong
                'euclid's algorithm
                Do
                    r = i1 Mod i2
                    i1 = i2
                    i2 = r
                Loop While r > 0
                Return i1
            Catch ex As Exception
                Return Double.NaN
            End Try
        End Function

        Public Function GCD(ByVal v1 As Double, ByVal v2 As Double) As Double 'alternate name
            Return GCF(v1, v2)
        End Function
        Public Function LCM(ByVal v1 As Double, ByVal v2 As Double) As Double
            Try
                Dim i1 As Integer = Int(v1)
                Dim i2 As Integer = Int(v2)
                Return i1 * i2 / GCF(i1, i2)
            Catch
                Return Double.NaN
            End Try
        End Function

        ' rounding
        ''' <summary>
        ''' Round the number to the nearest integer
        ''' </summary>
        Public Function Round(ByVal value As Double, Optional ByVal digits As Double = 0) As Double
            Return Math.Round(value, Int(digits))
        End Function

        ''' <summary>
        ''' Round the number to the integer above
        ''' </summary>
        Public Function Ceil(ByVal value As Double) As Double
            Return Math.Ceiling(value)
        End Function

        ''' <summary>
        ''' Round the number to the integer below
        ''' </summary>
        Public Function Floor(ByVal value As Double) As Double
            Return Math.Floor(value)
        End Function

        ''' <summary>
        ''' Round the number to the nearest integer to 0
        ''' </summary>
        Public Function Truncate(ByVal value As Double) As Double
            Return Math.Truncate(value)
        End Function

        ''' <summary>
        ''' Round the number to the specified number of significant figures
        ''' </summary>
        Public Function SigFig(ByVal value As Double, Optional ByVal digits As Double = 1) As Double
            Return Math.Sign(value) *
                Math.Round(Math.Abs(value) / 10 ^ Math.Ceiling(Math.Log10(Math.Abs(value))),
                           Int(digits)) * 10 ^ Math.Ceiling(Math.Log10(Math.Abs(value)))
        End Function

        ''' <summary>
        ''' Get the sign of a number
        ''' </summary>
        Public Function Sgn(ByVal value As Double) As Double
            Return Math.Sign(value)
        End Function

        ''' <summary>
        ''' Get the absolute value of a number, determinant of a matrix, magnitude of a vector
        ''' </summary>
        Public Function Abs(ByVal value As Object) As Object
            If TypeOf value Is Double Then
                Return Math.Abs(CDbl(value))
            ElseIf TypeOf value Is BigDecimal Then
                Dim bdec As BigDecimal = CType(value, BigDecimal)
                If bdec >= 0 Then
                    Return bdec
                Else
                    Return -bdec
                End If
            ElseIf TypeOf value Is Numerics.Complex Then
                Return CType(value, Numerics.Complex).Magnitude
            ElseIf TypeOf value Is IEnumerable(Of Reference) Then
                ' Find the determinant. If we cannot, try getting the magnitude. If that still fails, get the length of the list
                Try
                    Return Det(CType(value, IEnumerable(Of Reference)))
                Catch
                    Try
                        Return Magnitude(CType(value, IEnumerable(Of Reference)))
                    Catch
                        Return CType(value, IEnumerable(Of Reference)).Count
                    End Try
                End Try
            ElseIf TypeOf value Is IDictionary(Of Reference, Reference) Then
                Return CType(value, IDictionary(Of Reference, Reference)).Count
            ElseIf TypeOf value Is String
                Return CStr(value).Length
            Else
                Return 0
            End If
        End Function

        ''' <summary>
        ''' Get the natural logarithm of a number
        ''' </summary>
        Public Function Ln(ByVal value As Object) As Object
            If TypeOf value Is Double Then
                Return Math.Log(CDbl(value))
            ElseIf TypeOf value Is Numerics.Complex
                Return Numerics.Complex.Log(CType(value, Numerics.Complex))
            Else
                Return Double.NaN
            End If
        End Function

        ''' <summary>
        ''' Get the base [base] logarithm of a number. If base is not specified, defaults to 10.
        ''' </summary>
        Public Function Log(ByVal value As Object, Optional ByVal base As Object = 10) As Object
            If TypeOf value Is Double Then
                Return Math.Log(CDbl(value), CDbl(base))
            ElseIf TypeOf value Is Numerics.Complex
                Return Numerics.Complex.Log(CType(value, Numerics.Complex), CDbl(base))
            Else
                Return Double.NaN
            End If
        End Function

        ''' <summary>
        ''' Get the base 2 logarithm of a number
        ''' </summary>
        Public Function Log2(ByVal value As Object) As Object
            Return Me.Log(value, 2)
        End Function

        ''' <summary>
        ''' Get the base 2 logarithm of a number
        ''' </summary>
        Public Function Lg(ByVal value As Object) As Object
            Return Me.Log2(value)
        End Function

        ''' <summary>
        ''' Get the base 10 logarithm of a number
        ''' </summary>
        Public Function Log10(ByVal value As Object) As Object
            Return Me.Log(value)
        End Function

        ''' <summary>
        ''' Get the modulo of two numbers
        ''' </summary>
        Public Function Modulo(ByVal value1 As Double, ByVal value2 As Double) As Double
            If value2 < 0 Then Return -Modulo(-value1, -value2)
            If value1 < 0 Then
                Return (value2 + value1 Mod value2) Mod value2
            Else
                Return value1 Mod value2
            End If
        End Function

        ''' <summary>
        ''' Swap two references to objects
        ''' </summary>
        Public Function Swap(value1 As Reference, value2 As Reference) As Reference()
            Dim t As Object = value1.GetValue()
            value1.SetValue(value2.GetValue())
            value2.SetValue(t)
            Return {value1, value2}
        End Function

        ''' <summary>
        ''' Returns the mean of a set
        ''' </summary>
        ''' <param name="value1"></param>
        ''' <returns></returns>
        Public Function Average(ByVal value1 As Object) As Double
            Dim ct As Integer = 1
            Dim res As Double = RecursiveComputeLst({value1}, New Func(Of Double, Double, Double)(
                          Function(a As Double, b As Double)
                              ct += 1
                              Return a + b
                          End Function))
            Return res / ct
        End Function

        ''' <summary>
        ''' Returns the mean of a set (alias for average)
        ''' </summary>
        ''' <param name="value1"></param>
        ''' <returns></returns>
        Public Function Mean(ByVal value1 As Object) As Double
            Return Average(value1)
        End Function

        ''' <summary>
        ''' Returns the median value of a list. If the number of elements is even, returns the average of the middle two elements.
        ''' </summary>
        ''' <param name="value1"></param>
        ''' <returns></returns>
        Public Function Median(ByVal value1 As Object) As Double
            Dim lst As New List(Of Double)
            If TypeOf value1 Is IEnumerable(Of Reference) Then
                Dim tmp As IEnumerable(Of Reference) = CType(value1, IEnumerable(Of Reference))
                For Each r As Reference In tmp
                    lst.Add(CDbl(r.Resolve()))
                Next
            ElseIf TypeOf value1 Is Reference() Then
                Dim tmp As IEnumerable(Of Reference) = CType(value1, Reference()).ToList()
                For Each r As Reference In tmp
                    lst.Add(CDbl(r.Resolve()))
                Next
            ElseIf TypeOf value1 Is BigDecimal OrElse TypeOf value1 Is Double
                Return CDbl(value1)
            Else
                Return Double.NaN
            End If
            lst.Sort()
            If lst.Count Mod 2 = 1 Then
                Return lst(Int(lst.Count / 2))
            Else
                Return lst(Int(lst.Count / 2)) / 2 + lst(Int(lst.Count / 2 - 1)) / 2
            End If
        End Function

        ''' <summary>
        ''' Returns the mode of the list. If there are multiple modes (aka. no mode), returns undefined (NaN)
        ''' </summary>
        ''' <param name="value1"></param>
        ''' <returns></returns>
        Public Function Mode(ByVal value1 As Object) As Double
            Dim lst As New List(Of Double)
            Dim count As New Dictionary(Of Double, Integer)
            Dim countfreq As New Dictionary(Of Integer, Integer)

            If TypeOf value1 Is IEnumerable(Of Reference) Then
                Dim tmp As IEnumerable(Of Reference) = CType(value1, IEnumerable(Of Reference))
                For Each r As Reference In tmp
                    lst.Add(CDbl(r.Resolve()))
                Next
            ElseIf TypeOf value1 Is Reference() Then
                Dim tmp As IEnumerable(Of Reference) = CType(value1, Reference()).ToList()
                For Each r As Reference In tmp
                    lst.Add(CDbl(r.Resolve()))
                Next
            ElseIf TypeOf value1 Is BigDecimal OrElse TypeOf value1 Is Double
                Return CDbl(value1)
            Else
                Return Double.NaN
            End If

            If lst.Count = 0 Then Return Double.NaN

            count(0) = 0
            Dim highCount As Double = 0
            For Each v As Double In lst
                v = Math.Round(v, 10)
                If Not count.ContainsKey(v) Then count(v) = 0
                If Not countfreq.ContainsKey(count(v)) Then countfreq(count(v)) = 0
                countfreq(count(v)) -= 1
                count(v) += 1
                If Not countfreq.ContainsKey(count(v)) Then countfreq(count(v)) = 0
                countfreq(count(v)) += 1
                If count(v) > count(highCount) Then
                    highCount = v
                End If
            Next

            If countfreq(count(highCount)) > 1 Then
                Return Double.NaN ' nore than one mode
            Else
                Return highCount ' found!
            End If
        End Function

        Public Function Min(ByVal value1 As Object, Optional ByVal value2 As Double = Double.PositiveInfinity) As Double
            Return RecursiveComputeLst({value1, value2}, AddressOf Math.Min)
        End Function

        Public Function Max(ByVal value1 As Object, Optional ByVal value2 As Double = Double.NegativeInfinity) As Double
            Return RecursiveComputeLst({value1, value2}, AddressOf Math.Max)
        End Function

        Private Function RecursiveComputeLst(list() As Object, func As Func(Of Double, Double, Double)) As Double
            If list.Length = 0 Then Return Double.NaN

            If TypeOf list(0) Is IEnumerable(Of Reference) Then
                list(0) = RecursiveComputeLst(New List(Of Object)(CType(list(0), IEnumerable(Of Reference))).ToArray(), func)
            ElseIf TypeOf list(0) Is Reference() Then
                list(0) = RecursiveComputeLst(New List(Of Object)(CType(list(0), Reference())).ToArray(), func)
            End If
            While TypeOf list(0) Is Reference
                list(0) = CType(list(0), Reference).GetValue()
            End While
            If TypeOf list(0) IsNot Double Then Return Double.NaN

            Dim result As Double = CDbl(list(0))
            For i As Integer = 1 To list.Length - 1
                Dim obj As Object = list(i)
                If TypeOf list(0) Is IEnumerable(Of Reference) Then
                    obj = RecursiveComputeLst(CType(obj, IEnumerable(Of Reference)).ToArray(), func)
                ElseIf TypeOf list(0) Is Reference() Then
                    obj = RecursiveComputeLst(CType(obj, Reference()), func)
                End If
                While TypeOf obj Is Reference
                    obj = CType(obj, Reference).GetValue()
                End While
                If TypeOf obj Is BigDecimal OrElse TypeOf obj Is Double Then
                    If CDbl(obj) = Double.NaN Then Continue For
                    result = func(result, CType(obj, Double))
                End If
            Next
            Return result
        End Function



        ' calculus
        Public Function Dydx(ByVal func As Lambda) As Double
            Return Derivative(func)
        End Function
        Public Function DydxAt(ByVal func As Lambda, ByVal x As Double) As Double
            Return DerivativeAt(func, x)
        End Function
        Public Function DNydxN(ByVal func As Lambda, ByVal n As Double, Optional ByVal delta As String = "x"c) As Double
            Dim v As Double = CDbl(_eval.GetVariable(delta))
            Return DNydxNAt(func, n, v)
        End Function
        Public Function DNydxNAt(ByVal func As Lambda, ByVal n As Double, ByVal x As Double) As Double
            If n > 0 Then
                If CmpDbl(n, 1) > 0 Then
                    For i As Integer = 1 To Int(Math.Floor(n)) - 1
                        Dim newFn As String = func.ToString()
                        If newFn.StartsWith("`") Then
                            newFn = newFn.Trim("`"c)
                        Else
                            newFn = newFn & "()"
                        End If
                        func = New Lambda("`derivative(" & ControlChars.Quote & newFn &
                                          ControlChars.Quote & ")`")
                    Next
                End If
                Return DerivativeAt(func, x)
            Else
                Return Double.NaN
            End If
        End Function

        ''' <summary>
        ''' Take the derivative of a function at the current value of the variable
        ''' </summary>
        ''' <param name="func">The function, as a texting</param>
        ''' <param name="delta">The variable to take the derivative of</param>
        ''' <returns></returns>
        Public Function Derivative(ByVal func As Lambda, Optional ByVal delta As String = "x"c) As Double
            Dim v As Double = CDbl(_eval.GetVariable(delta))
            Return DerivativeAt(func, v)
        End Function

        ''' <summary>
        ''' Take the derivative of a function at x
        ''' </summary>
        Public Function DerivativeAt(ByVal func As Lambda, ByVal x As Double, Optional ByVal delta As String = "x"c) As Double
            Dim oldx As Object = _eval.GetVariable("x"c)
            _eval.SetVariable(delta, New ObjectTypes.Number(x - 0.0001))
            Dim l As Double = CDbl(func.Execute(_eval, {}))
            _eval.SetVariable(delta, New ObjectTypes.Number(x + 0.0001))
            Dim r As Double = CDbl(func.Execute(_eval, {}))
            _eval.SetVariable(delta, ObjectTypes.DetectType(oldx))
            Return Math.Round((r - l) / 0.0002, 5)
        End Function

        ' integration
        ''' <summary>
        ''' Takes the definite integral of a function between a and b
        ''' </summary>
        Public Function Integral(ByVal func As Lambda, ByVal a As Double, ByVal b As Double,
                                 Optional ByVal delta As String = "x"c) As Double
            ' use simpson's rule by default
            Return IntegralSimpson(func, a, b, delta)
        End Function

        ' integral from a certain point to the variable specified
        ''' <summary>
        ''' Takes the definite integral of a function between a and the current value of thevariable
        ''' </summary>
        Public Function IntegralFrom(ByVal func As Lambda, ByVal a As Double, Optional ByVal delta As String = "x"c) As Double
            ' use simpson's rule by default
            Return IntegralSimpson(func, a, CDbl(_eval.GetVariable(delta)), delta)
        End Function

        ''' <summary>
        '''  Integral estimation with simpson's rule
        ''' </summary>
        Public Function IntegralSimpson(ByVal func As Lambda, ByVal a As Double, ByVal b As Double, Optional ByVal delta As String = "x"c) As Double
            If CmpDbl(a, b) = 0 Then Return 0
            Dim oldx As Object = _eval.GetVariable(delta)
            Dim stepx As Decimal = CDec(b - a) / 2500
            Dim res As Decimal = 0
            Dim sw As Decimal = 1
            _eval.SetVariable(delta, New ObjectTypes.Number(a))
            For cx As Decimal = CDec(a) To CDec(b) - stepx Step stepx
                _eval.SetVariable(delta, New ObjectTypes.Number(cx))
                res += sw * CDec(func.Execute(_eval, {}))
                If sw = 2 OrElse sw = 1 Then
                    sw = 4
                Else
                    sw = 2
                End If
            Next
            _eval.SetVariable(delta, New ObjectTypes.Number(b))
            res += CDec(func.Execute(_eval, {}))
            _eval.SetVariable(delta, ObjectTypes.DetectType(oldx))
            Return Math.Round(res / 3 * stepx, 5)
        End Function

        ''' <summary>
        '''  Integral estimation with trapezoid sums
        ''' </summary>
        Public Function IntegralTrapezoid(ByVal func As Lambda, ByVal a As Double, ByVal b As Double, Optional ByVal delta As String = "x"c) As Double
            If CmpDbl(a, b) = 0 Then Return 0
            Dim oldx As Object = _eval.GetVariable(delta)
            Dim stepx As Decimal = CDec(b - a) / 25000
            Dim res As Decimal = 0
            _eval.SetVariable(delta, New ObjectTypes.Number(a))
            Dim py As Decimal = CDec(func.Execute(_eval, {}))
            For cx As Decimal = CDec(a) + stepx To CDec(b) Step stepx
                _eval.SetVariable(delta, New ObjectTypes.Number(cx))
                Dim cy As Decimal = CDec(func.Execute(_eval, {}))
                res += (py + (cy - py) / 2) * stepx
                py = cy
            Next
            _eval.SetVariable(delta, ObjectTypes.DetectType(oldx))
            Return Math.Round(res, 5)
        End Function

        ''' <summary>
        '''  Integral estimation with midpoint sums
        ''' </summary>
        Public Function IntegralMidpoint(ByVal func As Lambda, ByVal a As Double, ByVal b As Double, Optional ByVal delta As String = "x"c) As Double
            Return IntegralRiemann(func, a, b, 0, delta)
        End Function
        ''' <summary>
        ''' integral estimation with left riemann sums
        ''' </summary>
        Public Function IntegralLeft(ByVal func As Lambda, ByVal a As Double, ByVal b As Double, Optional ByVal delta As String = "x"c) As Double
            Return IntegralRiemann(func, a, b, -1, delta, 50000)
        End Function
        ''' <summary>
        ''' integral estimation with right riemann sums
        ''' </summary>
        Public Function IntegralRight(ByVal func As Lambda, ByVal a As Double, ByVal b As Double, Optional ByVal delta As String = "x"c) As Double
            Return IntegralRiemann(func, a, b, 1, delta, 50000)
        End Function

        ''' <summary>
        ''' helper function for integral estimations with riemann sums
        ''' </summary>
        Private Function IntegralRiemann(ByVal func As Lambda, ByVal a As Double, ByVal b As Double,
                             ByVal offset As Integer, Optional ByVal delta As String = "x"c, Optional ByVal intervals As Integer = 10000) As Double
            If CmpDbl(a, b) = 0 Then Return 0
            Dim oldx As Object = _eval.GetVariable(delta)
            Dim stepx As Decimal = CDec(b - a) / intervals
            Dim res As Decimal = 0
            _eval.SetVariable(delta, New ObjectTypes.Number(a))
            For cx As Decimal = CDec(a) + stepx / 2 * (offset + 1) To CDec(b) + stepx / 2 * (offset - 1) Step stepx
                _eval.SetVariable(delta, New ObjectTypes.Number(cx))
                res += stepx * CDec(func.Execute(_eval, {}))
            Next
            _eval.SetVariable(delta, ObjectTypes.DetectType(oldx))
            Return Math.Round(res, 5)
        End Function
        ' end calculus

        ''' <summary>
        ''' Summation: takes the sum of expression over the range between a and b, inclusive
        ''' </summary>
        Public Function Sigma(ByVal func As Lambda, ByVal a As Double, ByVal b As Double,
                              Optional ByVal [step] As Double = 1, Optional ByVal variable As String = "i") As BigDecimal
            Try
                ' step cannot be 0 or in the reverse direction
                If CmpDbl([step], 0) = 0 OrElse (b - a) / [step] < 0 OrElse
                Double.IsInfinity([step]) OrElse Double.IsNaN([step]) Then Return Double.NaN
                If b < a Then [step] = -1
                Dim prevvar As EvalObjectBase = _eval.GetVariableRef(variable).GetRefObject()
                Dim sum As BigDecimal = 0
                For i As Double = a To b Step [step]
                    _eval.SetVariable(variable, New ObjectTypes.Number(i))
                    sum += CType(func.Execute(_eval, {}), BigDecimal)
                Next
                _eval.SetVariable(variable, prevvar)
                Return sum
            Catch
                Return Double.NaN
            End Try
        End Function

        ''' <summary>
        ''' Summation: takes the sum of expression over the range between a and b, inclusive
        ''' Alias for sigma()
        ''' </summary>
        Public Function Sum(ByVal func As Lambda, ByVal a As Double, ByVal b As Double,
                            Optional ByVal [step] As Double = 1, Optional ByVal variable As String = "i") As BigDecimal
            Return Sigma(func, a, b, [step], variable)
        End Function

        ''' <summary>
        ''' Product: takes the product of expression over the range between l and r, inclusive
        ''' Alias for sigma()
        ''' </summary>
        Public Function product(ByVal func As Lambda, ByVal a As Double, ByVal b As Double,
                                Optional ByVal [step] As Double = 1, Optional ByVal variable As String = "i") As BigDecimal
            Try
                ' step cannot be 0 or in the reverse direction
                If CmpDbl([step], 0) = 0 OrElse (b - a) / [step] < 0 OrElse
                Double.IsInfinity([step]) OrElse Double.IsNaN([step]) Then Return Double.NaN
                If b < a Then [step] = -1
                Dim prevvar As EvalObjectBase = _eval.GetVariableRef(variable)
                Dim prod As BigDecimal = 1
                For i As Double = a To b Step [step]
                    _eval.SetVariable(variable, New ObjectTypes.Number(i))
                    prod *= CType(func.Execute(_eval, {}), BigDecimal)
                Next
                _eval.SetVariable(variable, prevvar)
                Return prod
            Catch
                Return Double.NaN
            End Try
        End Function

        ' encryption (just for fun)
        ' function for creating caesar ciphers
        Public Function Caesar(ByVal value As String, ByVal shift As Double) As String
            value = value.ToUpper()
            Dim ret As New StringBuilder
            For Each c As Char In value
                If c = " "c Then
                    ret.Append(" ")
                Else
                    Dim cid As Integer = AscW(c) + Int(shift)
                    If cid > AscW("Z"c) Then cid -= 26
                    If cid < AscW("A"c) Then cid += 26
                    ret.Append(ChrW(cid))
                End If
            Next
            Return ret.ToString()
        End Function

        ''' <summary>
        ''' Encodes a string into Xecryption (very bad encryption)
        ''' </summary>
        Public Function EncodeXecryption(ByVal value As String, Optional ByVal pwd As Double = 0) As String
            Dim res As String = ""
            Dim rand As New Random()
            For Each c As Char In value
                Dim val As Integer = AscW(c) + Int(Math.Truncate(pwd))
                Dim x As Double = rand.NextDouble()
                Dim y As Double = rand.NextDouble()
                Dim z As Double = rand.NextDouble()
                Dim sum As Double = x + y + z
                Dim xi As Integer = Int(val * (x / sum))
                Dim yi As Integer = Int(val * (y / sum))
                Dim zi As Integer = Int(val * (z / sum))
                If xi + yi + zi <> val Then
                    Dim modify As Integer = rand.Next(0, 3)
                    If modify = 0 Then
                        xi += val - xi - yi - zi
                    ElseIf modify = 1
                        yi += val - xi - yi - zi
                    Else
                        zi += val - xi - yi - zi
                    End If
                End If
                res &= String.Format(".{0}.{1}.{2}", xi, yi, zi)
            Next
            Return res
        End Function

        ''' <summary>
        ''' Decodes a string from Xecryption (very bad encryption)
        ''' </summary>
        Public Function DecodeXecryption(ByVal value As String, Optional ByVal pwd As Double = -1) As String
            Dim res As String = ""
            Dim rand As New Random()
            Dim spl As String() = value.Trim({"."c, vbCr(0), vbLf(0), " "c}).Split("."c)
            If spl.Length < 3 Then Return ""
            If pwd = -1 Then
                Dim totalini As Integer = Integer.Parse(spl(0)) + Integer.Parse(spl(1)) +
                Integer.Parse(spl(2))
                Dim subopt As String = ""
                For t As Integer = totalini - 126 To totalini - 10
                    Dim text As String = DecodeXecryption(value, t)
                    For i As Integer = 0 To text.Length - 1
                        Dim ascii As Integer = AscW(text(i))
                        If ascii >= AscW("{"c) OrElse (ascii < AscW(" "c) AndAlso ascii <> AscW(vbLf(0)) AndAlso
                        ascii <> AscW(vbCr(0))) Then
                            text = ""
                            Exit For
                        End If
                    Next
                    If text = "" Then Continue For
                    For i As Integer = 0 To text.Length - 1
                        Dim ascii As Integer = AscW(text(i))
                        If ((ascii >= AscW(" "c) AndAlso ascii < AscW("#"c)) OrElse (ascii >= AscW(","c) AndAlso
                        ascii <= AscW("."c)) OrElse (ascii >= AscW(":"c) AndAlso
                        ascii < AscW("<"c)) OrElse ascii = AscW("?"c)) Then
                            If subopt = "" Then subopt = text & vbCrLf
                            text = ""
                            Exit For
                        End If
                    Next

                    If text <> "" Then
                        Return text
                    End If
                Next
                If subopt <> "" Then Return subopt
                Return "XEcryption: Auto Decryption Failed"
            End If
            For i As Integer = 2 To spl.Length - 1 Step 3
                Dim total As Integer = Integer.Parse(spl(i - 2)) + Integer.Parse(spl(i - 1)) +
                Integer.Parse(spl(i)) - Int(Math.Truncate(pwd))
                res &= ChrW(total)
            Next
            Return res
        End Function

        ' encryption (actual)

        ''' <summary>
        ''' Encode a texting into base 64
        ''' </summary>
        ''' <param name="value"></param>
        ''' <returns></returns>
        Public Function EncodeBase64(ByVal value As String) As String
            Return Convert.ToBase64String(Encoding.ASCII.GetBytes(value))
        End Function
        Public Function Eb64(ByVal value As String) As String
            Return EncodeBase64(value)
        End Function

        ''' <summary>
        ''' Decode a texting from base 64
        ''' </summary>
        ''' <param name="value"></param>
        ''' <returns></returns>
        Public Function DecodeBase64(ByVal value As String) As String
            Return Encoding.ASCII.GetString(Convert.FromBase64String(value))
        End Function
        Public Function Db64(ByVal value As String) As String
            Return DecodeBase64(value)
        End Function

        ''' <summary>
        ''' Encrypts a texting using the AES/Rijndael symmetric key algorithm using the specified password
        ''' generates a random hash and IV and appends them before the actual cipher, so they can be used when decrypting
        ''' modified from http://www.obviex.com/samples/encryption.aspx
        ''' </summary>
        ''' <param name="value">The value to encrypt. Automatically converted to a texting.</param>
        ''' <param name="pwd">The password</param>
        ''' <param name="keySize">The size of the key (default is 256, please use a power of 2)</param>
        ''' <returns></returns>
        Public Function EncryptAES(ByVal value As Object, ByVal pwd As String,
                                    Optional ByVal keySize As Double = 256) As String
            Dim bytes As Byte() = Encoding.UTF8.GetBytes(value.ToString())
            Dim symmetricKey As RijndaelManaged
            symmetricKey = New RijndaelManaged()
            symmetricKey.Mode = CipherMode.CBC

            Dim saltBytes(32) As Byte
            Dim rand As New RNGCryptoServiceProvider()
            rand.GetNonZeroBytes(saltBytes)

            Dim password As Rfc2898DeriveBytes = New Rfc2898DeriveBytes(pwd, saltBytes, 2)

            Dim keyBytes As Byte()
            keyBytes = password.GetBytes(Int(keySize / 8))

            symmetricKey.GenerateIV()
            Dim encryptor As ICryptoTransform
            encryptor = symmetricKey.CreateEncryptor(keyBytes, symmetricKey.IV())

            Using memoryStream As New MemoryStream()
                Using cryptoStream As New CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write)
                    ' Start encrypting.
                    cryptoStream.Write(bytes, 0, bytes.Length)
                    cryptoStream.FlushFinalBlock()

                    Dim cipherTextBytes As Byte() = memoryStream.ToArray()
                    Dim ivTxt As String = Convert.ToBase64String(symmetricKey.IV)

                    ivTxt = ivTxt.Remove(ivTxt.Count - 2) ' save two bytes
                    Return Convert.ToBase64String(saltBytes) & ivTxt &
                        Convert.ToBase64String(cipherTextBytes)
                End Using
            End Using
        End Function

        ''' <summary>
        ''' Decode a AES/Rijndael-encrypted texting using the specified password
        ''' modified from http://www.obviex.com/samples/encryption.aspx
        ''' </summary>
        ''' <param name="value">The encrypted texting</param>
        ''' <param name="pwd">The password</param>
        ''' <param name="keySize">The size of the key (default is 256, please use a power of 2)</param>
        ''' <returns></returns>
        Public Function DecryptAES(ByVal value As String, ByVal pwd As String,
                                         Optional ByVal keySize As Double = 256) As String

            Dim salt As String = value.Remove(44)
            Dim iv As String = value.Remove(66).Substring(44) & "=="
            Dim cipher As String = value.Substring(66)

            Dim saltBytes As Byte() = Convert.FromBase64String(salt)
            Dim ivBytes As Byte() = Convert.FromBase64String(iv)
            Dim cipherTextBytes As Byte() = Convert.FromBase64String(cipher)

            Dim password As New Rfc2898DeriveBytes(pwd, saltBytes, 2)

            Dim keyBytes As Byte()
            keyBytes = password.GetBytes(Int(keySize / 8))

            Dim symmetricKey As RijndaelManaged
            symmetricKey = New RijndaelManaged()

            symmetricKey.Mode = CipherMode.CBC

            Dim decryptor As ICryptoTransform
            decryptor = symmetricKey.CreateDecryptor(keyBytes, ivBytes)

            Using MemoryStream As New MemoryStream(cipherTextBytes)
                Using cryptoStream As New CryptoStream(MemoryStream, decryptor, CryptoStreamMode.Read)
                    Dim plainTextBytes As Byte()
                    ReDim plainTextBytes(cipherTextBytes.Length)

                    Dim decryptedByteCount As Integer
                    decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length)

                    Return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount)
                End Using
            End Using
        End Function

        Public Function Hash(ByVal value As Object) As String
            Return SHA2Hash(value)
        End Function

        Public Function IntHash(ByVal value As Object) As Double
            Return value.GetHashCode()
        End Function

        Public Function MD5Hash(ByVal value As Object) As String
            Dim md5Crypt As MD5 = System.Security.Cryptography.MD5.Create()
            Dim bytes As Byte() = System.Text.Encoding.UTF8.GetBytes(ObjectTypes.DetectType(value.ToString(), True).ToString())
            Dim hash As Byte() = md5Crypt.ComputeHash(bytes)
            Dim sb As New StringBuilder()

            For i As Integer = 0 To hash.Length - 1
                sb.Append(hash(i).ToString("X2"))
            Next
            Return sb.ToString()
        End Function

        Public Function SHA1Hash(ByVal value As Object) As String
            Dim sha1Crypt As SHA1 = System.Security.Cryptography.SHA1.Create()
            Dim bytes As Byte() = System.Text.Encoding.UTF8.GetBytes(ObjectTypes.DetectType(value.ToString(), True).ToString())
            Dim hash As Byte() = sha1Crypt.ComputeHash(bytes)
            Dim sb As New StringBuilder()

            For i As Integer = 0 To hash.Length - 1
                sb.Append(hash(i).ToString("X2"))
            Next
            Return sb.ToString()
        End Function

        Public Function SHA2Hash(ByVal value As Object) As String
            Dim sha256Crypt As SHA256 = System.Security.Cryptography.SHA256.Create()
            Dim bytes As Byte() = System.Text.Encoding.UTF8.GetBytes(ObjectTypes.DetectType(value.ToString(), True).ToString())
            Dim hash As Byte() = sha256Crypt.ComputeHash(bytes)
            Dim sb As New StringBuilder()

            For i As Integer = 0 To hash.Length - 1
                sb.Append(hash(i).ToString("X2"))
            Next
            Return sb.ToString()
        End Function

        Public Function RandomPassword(Optional ByVal length As Double = 8, Optional ByVal allowSymbols As Boolean = False) As String
            Dim sb As New StringBuilder()
            Dim rand As New Random()
            Dim ltrs As New List(Of Char)
            If allowSymbols Then
                ltrs.AddRange({"="c, "+"c, "$"c, "#"c, "@"c, "!"c, "%"c, "^"c, "&"c})
            End If
            For i As Integer = AscW("0"c) To AscW("9"c)
                ltrs.Add(ChrW(i))
            Next
            For i As Integer = AscW("a"c) To AscW("z"c)
                ltrs.Add(ChrW(i))
            Next
            For i As Integer = AscW("A"c) To AscW("Z"c)
                ltrs.Add(ChrW(i))
            Next

            For i As Integer = 1 To Int(length)
                sb.Append(ltrs(rand.Next(0, ltrs.Count)))
            Next

            Return sb.ToString()
        End Function

        ' base conversion
        Public Function CBase(ByVal value As String, ByVal frombase As Double, ByVal base As Double) As String
            Dim ret As String = ""
            Dim rm As Double
            Dim qo As Double = 0
            For i As Integer = 0 To value.Length - 1
                Dim dgtc As Char = value(value.Length - 1 - i)
                Dim dgt As Double
                If Char.IsNumber(dgtc) Then
                    dgt = Double.Parse(dgtc)
                ElseIf Char.IsUpper(dgtc)
                    dgt = AscW(dgtc) - AscW("A"c) + 10
                ElseIf Char.IsLower(dgtc)
                    dgt = AscW(dgtc) - AscW("a"c) + 36
                Else
                    dgt = 0
                End If
                If dgt >= frombase Then Return "Undefined"
                qo += dgt * frombase ^ i
            Next
            While CmpDbl(qo, 0) <> 0
                rm = qo Mod base
                qo = Math.Floor(qo / base)
                If rm >= 36 Then
                    ret = ChrW(Int(rm) + AscW("a"c) - 36) & ret
                ElseIf rm >= 10 Then
                    ret = ChrW(Int(rm) + AscW("A"c) - 10) & ret
                Else
                    ret = Int(rm) & ret
                End If
            End While
            If ret = "" Then
                If base = 64 Then
                    Return "A"
                Else
                    Return "0"
                End If
            End If
            Return ret
        End Function

        ' binary repr   
        Public Function Bin(value As Double) As String
            Return Convert.ToString(Int(Math.Truncate(value)), 2)
        End Function
        ' octal repr
        Public Function Oct(value As Double) As String
            Return Convert.ToString(Int(Math.Truncate(value)), 7)
        End Function
        ' hex repr   
        Public Function Hex(value As Double) As String
            Return HexU(value)
        End Function
        Public Function HexL(value As Double) As String
            Return Convert.ToString(Int(Math.Truncate(value)), 16)
        End Function
        Public Function HexU(value As Double) As String
            Return Convert.ToString(Int(Math.Truncate(value)), 16).ToUpper()
        End Function

        Public Function IsUndefined(val As Double) As Boolean
            Return Double.IsNaN(val)
        End Function

        ' dates
        Public Function DateTime(ByVal first As Object, Optional ByVal month As Double = 0,
                                 Optional ByVal day As Double = 1, Optional ByVal hour As Double = 0,
                                 Optional ByVal minute As Double = 0, Optional ByVal second As Double = 0,
                                 Optional ByVal ms As Double = 0) As Object
            If TypeOf first Is String Then
                Return New DateTime(first.ToString()).GetValue()
            ElseIf TypeOf first Is Double OrElse TypeOf first Is BigDecimal OrElse TypeOf first Is Integer Then
                If month = 0 Then
                    Dim ts As New TimeSpan(CLng(first))
                    If ts.Days > ObjectTypes.DateTime.TIMESPAN_DIVIDER Then
                        Return ObjectTypes.DateTime.BASE_DATE.Add(ts)
                    Else
                        Return ts
                    End If
                Else
                    If Int(CDbl(first)) * 365 + Int(month) * 30 + Int(day) <= ObjectTypes.DateTime.TIMESPAN_DIVIDER Then
                        Return New TimeSpan(Int(CDbl(first)) * 365 + Int(month) * 30 + Int(day), Int(hour), Int(minute), Int(second), Int(ms))
                    Else
                        Return New Date(Int(CDbl(first)), Int(month), Int(day), Int(hour), Int(minute), Int(second), Int(ms))
                    End If
                End If
            Else
                Return Nothing
            End If
        End Function
        Public Function [Date](ByVal year As Double, Optional ByVal month As Double = 1,
                                 Optional ByVal day As Double = 1) As Date
            Return New Date(Int(year), Int(month), Int(day))
        End Function
        Public Function Time(Optional ByVal hour As Double = 0,
                                 Optional ByVal minute As Double = 0, Optional ByVal second As Double = 0,
                             Optional ByVal millisecond As Double = 0) As TimeSpan
            Return New TimeSpan(Int(hour), Int(minute), Int(second), Int(millisecond))
        End Function

        Public Function Now() As System.DateTime
            Return System.DateTime.Now
        End Function
        Public Function UTCNow() As System.DateTime
            Return System.DateTime.UtcNow
        End Function
        Public Function Today() As System.DateTime
            Return System.DateTime.Today
        End Function

        ' datetime modification
        Public Function Year(dt As Object) As Double
            If TypeOf dt Is System.DateTime Then
                Return DirectCast(dt, System.DateTime).Year
            ElseIf TypeOf dt Is System.TimeSpan Then
                Return DirectCast(dt, System.TimeSpan).Days / 365
            Else
                Return Double.NaN
            End If
        End Function
        Public Function Years(dt As Object) As Double
            Return Year(dt)
        End Function

        Public Function Month(dt As Object) As Double
            If TypeOf dt Is System.DateTime Then
                Return DirectCast(dt, System.DateTime).Month
            ElseIf TypeOf dt Is System.TimeSpan Then
                Return DirectCast(dt, System.TimeSpan).Days / 30
            Else
                Return Double.NaN
            End If
        End Function
        Public Function Months(dt As Object) As Double
            Return Month(dt)
        End Function

        Public Function Day(dt As Object) As Double
            If TypeOf dt Is System.DateTime Then
                Return DirectCast(dt, System.DateTime).Day
            ElseIf TypeOf dt Is System.TimeSpan Then
                Return DirectCast(dt, System.TimeSpan).Days
            Else
                Return Double.NaN
            End If
        End Function
        Public Function Days(dt As Object) As Double
            Return Day(dt)
        End Function

        Public Function DayOfWeekName(dt As Object) As String
            If TypeOf dt Is System.DateTime Then
                Return DirectCast(dt, System.DateTime).DayOfWeek.ToString()
            End If
            Return "Unknown"
        End Function
        Public Function DayOfWeek(dt As Object) As Double
            If TypeOf dt Is System.DateTime Then
                Return Int(DirectCast(dt, System.DateTime).DayOfWeek)
            End If
            Return Double.NaN
        End Function

        Public Function Hour(dt As Object) As Double
            If TypeOf dt Is System.DateTime Then
                Return DirectCast(dt, System.DateTime).Hour
            ElseIf TypeOf dt Is System.TimeSpan Then
                Return DirectCast(dt, System.TimeSpan).Hours
            Else
                Return Double.NaN
            End If
        End Function
        Public Function Hours(dt As Object) As Double
            Return Hour(dt)
        End Function

        Public Function Minute(dt As Object) As Double
            If TypeOf dt Is System.DateTime Then
                Return DirectCast(dt, System.DateTime).Minute
            ElseIf TypeOf dt Is System.TimeSpan Then
                Return DirectCast(dt, System.TimeSpan).Minutes
            Else
                Return Double.NaN
            End If
        End Function
        Public Function Minutes(dt As Object) As Double
            Return Minute(dt)
        End Function

        Public Function Second(dt As Object) As Double
            If TypeOf dt Is System.DateTime Then
                Return DirectCast(dt, System.DateTime).Second
            ElseIf TypeOf dt Is System.TimeSpan Then
                Return DirectCast(dt, System.TimeSpan).Seconds
            Else
                Return Double.NaN
            End If
        End Function
        Public Function Seconds(dt As Object) As Double
            Return Second(dt)
        End Function

        Public Function Millisecond(dt As Object) As Double
            If TypeOf dt Is System.DateTime Then
                Return DirectCast(dt, System.DateTime).Millisecond
            ElseIf TypeOf dt Is System.TimeSpan Then
                Return DirectCast(dt, System.TimeSpan).Seconds
            Else
                Return Double.NaN
            End If
        End Function
        Public Function Milliseconds(dt As Object) As Double
            Return Second(dt)
        End Function

        Public Function Ticks(dt As Object) As Double
            If TypeOf dt Is System.DateTime Then
                Return DirectCast(dt, System.DateTime).Ticks
            ElseIf TypeOf dt Is System.TimeSpan Then
                Return DirectCast(dt, System.TimeSpan).Ticks
            Else
                Return Double.NaN
            End If
        End Function

        Public Function AddYears(dt As Object, years As Double) As Object
            If TypeOf dt Is System.DateTime Then
                Return DirectCast(dt, System.DateTime).AddYears(Int(years))
            ElseIf TypeOf dt Is System.TimeSpan Then
                Return DirectCast(dt, System.TimeSpan).Add(New TimeSpan(Int(years) * 365, 0, 0, 0))
            Else
                Return Nothing
            End If
        End Function
        Public Function AddMonths(dt As Object, months As Double) As Object
            If TypeOf dt Is System.DateTime Then
                Return DirectCast(dt, System.DateTime).AddMonths(Int(months))
            ElseIf TypeOf dt Is System.TimeSpan Then
                Return DirectCast(dt, System.TimeSpan).Add(New TimeSpan(Int(months) * 30, 0, 0, 0))
            Else
                Return Nothing
            End If
        End Function
        Public Function AddWeeks(dt As Object, days As Double) As Object
            If TypeOf dt Is System.DateTime Then
                Return DirectCast(dt, System.DateTime).AddDays(Int(days) * 7)
            ElseIf TypeOf dt Is System.TimeSpan Then
                Return DirectCast(dt, System.TimeSpan).Add(New TimeSpan(Int(days) * 7, 0, 0, 0))
            Else
                Return Nothing
            End If
        End Function
        Public Function AddDays(dt As Object, days As Double) As Object
            If TypeOf dt Is System.DateTime Then
                Return DirectCast(dt, System.DateTime).AddDays(Int(days))
            ElseIf TypeOf dt Is System.TimeSpan Then
                Return DirectCast(dt, System.TimeSpan).Add(New TimeSpan(Int(days), 0, 0, 0))
            Else
                Return Nothing
            End If
        End Function
        Public Function AddHours(dt As Object, hours As Double) As Object
            If TypeOf dt Is System.DateTime Then
                Return DirectCast(dt, System.DateTime).AddHours(Int(hours))
            ElseIf TypeOf dt Is System.TimeSpan Then
                Return DirectCast(dt, System.TimeSpan).Add(New TimeSpan(Int(hours), 0, 0))
            Else
                Return Nothing
            End If
        End Function
        Public Function AddMinutes(dt As Object, minutes As Double) As Object
            If TypeOf dt Is System.DateTime Then
                Return DirectCast(dt, System.DateTime).AddMinutes(Int(minutes))
            ElseIf TypeOf dt Is System.TimeSpan Then
                Return DirectCast(dt, System.TimeSpan).Add(New TimeSpan(0, Int(minutes), 0))
            Else
                Return Nothing
            End If
        End Function
        Public Function AddSeconds(dt As Object, seconds As Double) As Object
            If TypeOf dt Is System.DateTime Then
                Return DirectCast(dt, System.DateTime).AddSeconds(Int(seconds))
            ElseIf TypeOf dt Is System.TimeSpan Then
                Return DirectCast(dt, System.TimeSpan).Add(New TimeSpan(0, 0, Int(seconds)))
            Else
                Return Nothing
            End If
        End Function
        Public Function AddMilliseconds(dt As Object, ms As Double) As Object
            If TypeOf dt Is System.DateTime Then
                Return DirectCast(dt, System.DateTime).AddMilliseconds(Int(ms))
            ElseIf TypeOf dt Is System.TimeSpan Then
                Return DirectCast(dt, System.TimeSpan).Add(New TimeSpan(0, 0, 0, 0, Int(ms)))
            Else
                Return Nothing
            End If
        End Function
        Public Function AddTicks(dt As Object, ticks As Double) As Object
            If TypeOf dt Is System.DateTime Then
                Return DirectCast(dt, System.DateTime).AddTicks(Int(ticks))
            ElseIf TypeOf dt Is System.TimeSpan Then
                Return DirectCast(dt, System.TimeSpan).Add(New TimeSpan(Int(ticks)))
            Else
                Return Nothing
            End If
        End Function

        Public Function Rand(Optional min As Double = 0, Optional max As Double = 1) As Double
            Dim r As New Random()
            Return r.NextDouble() * (max - min) + min
        End Function

        Public Function RandInt(min As Double, max As Double) As Double
            Dim rand As New Random()
            Return rand.Next(Int(min), Int(max))
        End Function

        Public Function GUID() As String
            Return System.Guid.NewGuid().ToString()
        End Function

        Public Function UUID() As String
            Return GUID()
        End Function

        ''' <summary>
        ''' Compare two double values with the precision
        ''' </summary>
        Friend Function CmpDbl(v1 As Double, v2 As Double, Optional ByVal epsi As Double = 0.000000000001) As Integer
            Dim diff As Double = Math.Abs(v1 - v2)
            If diff < epsi Then
                Return 0
            ElseIf v1 > v2 Then
                Return 1
            Else
                Return -1
            End If
        End Function

        ''' <summary>
        ''' Get the length of any collection or string
        ''' </summary>
        Public Function Len(ByVal obj As Object) As Double
            If TypeOf obj Is String Then
                Return CStr(obj).Length
            ElseIf TypeOf obj Is IEnumerable(Of Reference)
                Return DirectCast(obj, IEnumerable(Of Reference)).Count()
            ElseIf TypeOf obj Is Reference()
                Return DirectCast(obj, Reference()).Length
            ElseIf TypeOf obj Is Dictionary(Of Object, Object)
                Return DirectCast(obj, Dictionary(Of Object, Object)).Count()
            Else
                If obj Is Nothing Then
                    Return 0
                Else
                    Return 1
                End If
            End If
        End Function

        ''' <summary>
        ''' Get the length of a piece of text or a collection
        ''' </summary>
        Public Function Size(ByVal obj As Object) As Double
            Return Len(obj)
        End Function

        ''' <summary>
        ''' Get the length of a piece of text or a collection
        ''' </summary>
        Public Function Length(ByVal lst As Object) As Double
            Return Len(lst)
        End Function

        ''' <summary>
        ''' Get the type of the object
        ''' </summary>
        Public Function Type(ByVal obj As Object) As String
            If obj Is Nothing OrElse (TypeOf obj Is Double AndAlso
            Double.IsNaN(CDbl(obj))) OrElse TypeOf obj Is System.DBNull Then
                Return "Undefined"
            ElseIf TypeOf obj Is LinkedList(Of Reference) Then
                Return "LinkedList"
            ElseIf TypeOf obj Is IEnumerable(Of Reference) Then
                Return "Matrix"
            ElseIf TypeOf obj Is Reference() Then
                Return "Tuple"
            ElseIf TypeOf obj Is SortedDictionary(Of Reference, Reference) Then
                Return "Set"
            ElseIf TypeOf obj Is Dictionary(Of Reference, Reference) Then
                Return "HashSet"
            ElseIf TypeOf obj Is Double OrElse TypeOf obj Is Single OrElse
                TypeOf obj Is Decimal Then
                Return "Number"
            ElseIf TypeOf obj Is String Then
                Return "Text"
            ElseIf TypeOf obj Is System.DateTime Then
                Return "Date"
            ElseIf TypeOf obj Is Boolean Then
                Return "Boolean"
            ElseIf TypeOf obj Is ICollection Then
                Return "(Matrix/Set/Tuple)"
            ElseIf TypeOf obj Is Reference Then
                Return "Reference of " & Type(DirectCast(obj, Reference).GetValue())
            ElseIf TypeOf obj Is Lambda Then
                Return "Function"
            Else
                Return obj.GetType().Name
            End If
        End Function
        Public Shadows Function [GetType](ByVal obj As Object) As String
            Return Type(obj)
        End Function

        ''' <summary>
        ''' Automatically detect the type of an object and convert it to a char array
        ''' </summary>
        ''' <param name="chars"></param>
        ''' <returns></returns>
        Private Function ToCharArray(chars As Object) As Char()
            If TypeOf chars Is String Then
                Return CStr(chars).ToCharArray()
            Else
                Dim lst As New List(Of Char)
                Dim refLst As IEnumerable(Of Reference)
                If TypeOf chars Is IEnumerable(Of Reference) Then
                    refLst = DirectCast(chars, IEnumerable(Of Reference))
                ElseIf TypeOf chars Is IDictionary(Of Reference, Reference) Then
                    refLst = DirectCast(chars, IDictionary(Of Reference, Reference)).Keys
                Else
                    Throw New EvaluatorException("Invalid types: expecting list of characters")
                End If
                For Each r As Reference In refLst
                    Dim res As Object = r.Resolve()
                    If TypeOf res Is String Then
                        If CStr(res).Length <> 1 Then Throw New EvaluatorException("Character expected")
                        lst.Add(CStr(res)(0))
                    End If
                Next
                Return lst.ToArray()
            End If
        End Function

        ''' <summary>
        ''' Remove all instances of each character from both ends of the string
        ''' </summary>
        Public Function Strip(ByVal text As String, Optional ByVal chars As Object = Nothing) As String
            If chars Is Nothing Then
                Return text.Trim()
            Else
                Return text.Trim(ToCharArray(chars))
            End If
        End Function

        ''' <summary>
        ''' Remove all instances of each character from the start of the string
        ''' </summary>
        Public Function StripStart(ByVal text As String, Optional ByVal chars As Object = Nothing) As String
            If chars Is Nothing Then
                Return text.TrimStart()
            Else
                Return text.TrimStart(ToCharArray(chars))
            End If
        End Function

        ''' <summary>
        ''' Remove all instances of each character from the end of the string
        ''' </summary>
        Public Function StripEnd(ByVal text As String, Optional ByVal chars As Object = Nothing) As String
            If chars Is Nothing Then
                Return text.TrimEnd()
            Else
                Return text.TrimEnd(ToCharArray(chars))
            End If
        End Function

        ''' <summary>
        ''' Replace format {0} in text with 
        ''' </summary>
        ''' <returns></returns>
        Public Function Format(ByVal text As Object, ByVal formatPattern As Object) As String
            Return String.Format(text.ToString(), formatPattern)
        End Function

        Friend Function Int(ByVal value As Double) As Integer
            Return CInt(Math.Truncate(value))
        End Function

        ''' <summary>
        ''' Solve a quadratic equation with coefficients a, b, and c
        ''' </summary>
        Public Function Quadratic(ByVal a As Double, ByVal b As Double, ByVal c As Double) As IEnumerable(Of Reference)
            Dim tort As Double = b ^ 2 - 4 * a * c
            Dim resultLst As New HashSet(Of Reference)
            If tort >= 0 Then
                resultLst.Add(New Reference((-b + Math.Sqrt(tort)) / (2 * a)))
                resultLst.Add(New Reference((-b - Math.Sqrt(tort)) / (2 * a)))
            Else
                resultLst.Add(New Reference((-b + Numerics.Complex.Sqrt(tort)) / (2 * a)))
                resultLst.Add(New Reference((-b - Numerics.Complex.Sqrt(tort)) / (2 * a)))
            End If
            Return New List(Of Reference)(resultLst)
        End Function

        ''' <summary>
        ''' Solve a quadratic equation with coefficients a, b, and c (shorthand for quadratic())
        ''' </summary>
        Public Function Qdtc(ByVal a As Double, ByVal b As Double, ByVal c As Double) As IEnumerable(Of Reference) 'shorthand for quadratic
            Return Quadratic(a, b, c)
        End Function

        ''' <summary>
        ''' Simplify a radical with radicand d and index ind
        ''' </summary>
        Private Function SimpRadical(ByVal d As Double, ByVal ind As Double) As String
            Dim sign As String = ""
            If d < 0 Then
                sign = "-"
            End If
            Dim rad As Int64() = SRadical(Math.Abs(d), Int(ind))
            Dim textbefore As String = "[" & ind.ToString
            Dim textafter As String = "]"
            If ind < 4 Then
                textbefore = ""
                textafter = ""
            End If
            If rad(0) = 1 Then
                Return sign & textbefore & If(ind = 3, "∛", "√") & textafter & rad(1)
            ElseIf rad(1) = 1 Then
                Return sign & rad(0)
            Else
                Return sign & rad(0) & textbefore & "√" & textafter & rad(1)
            End If
        End Function

        ''' <summary>
        ''' Internal helper for simplifying radicals
        ''' </summary>
        Private Function SRadical(ByVal d As Double, ByVal ind As Integer) As Int64()
            Dim coe As Long = 1
            Dim rdc As Long = Convert.ToInt64(d)
            For i As Integer = 2 To 12
                For j As Integer = 12 To ind Step -ind
                    Dim pow As Long = Convert.ToInt64(i ^ j)
                    Dim modu As Long = rdc Mod pow

                    If modu = 0 Then
                        coe *= Convert.ToInt64(i * j \ ind)
                        rdc \= pow
                    End If
                Next
            Next
            Return {coe, rdc}
        End Function

        ''' <summary>
        ''' Convert a double value to a fraction
        ''' </summary>
        Private Function ToFrac(ByVal d As Double) As String
            Dim sign As Integer = Int(Sgn(d))
            Dim res() As Double = CFrac(Math.Abs(d), Math.Min(0.0000000000001 * 10 ^ Math.Round(Math.Abs(d) ^ 0.1), 0.001))
            If res(1) = 1 Then
                Return (sign * res(0)).ToString
            ElseIf res(1) = 0
                Throw New Exception("Fraction resulted in 0 denominator")
            ElseIf res(1) < 50000
                Return (sign * res(0)) & "/" & res(1)
            Else
                Return d.ToString()
            End If
        End Function

        ''' <summary>
        ''' Internal function for converting a double value to a fraction
        ''' </summary>
        Private Function CFrac(ByVal d As Double, Optional ByVal epsi As Double = 0.000000000001) As Double()
            Dim n As Double = Math.Floor(d)
            d -= n
            If d < epsi Then
                Return {n, 1}
            ElseIf 1 - epsi < d
                Return {n + 1, 1}
            End If
            Dim lower_n As Double = 0
            Dim lower_d As Double = 1

            Dim upper_n As Double = 1
            Dim upper_d As Double = 1

            Dim middle_n As Double
            Dim middle_d As Double

            Dim runtimes As Integer = 0
            While runtimes < 100000000
                middle_n = lower_n + upper_n
                middle_d = lower_d + upper_d
                If middle_d * (d + epsi) < middle_n Then
                    upper_n = middle_n
                    upper_d = middle_d
                ElseIf middle_n < (d - epsi) * middle_d
                    lower_n = middle_n
                    lower_d = middle_d
                Else
                    Return {n * middle_d + middle_n, middle_d}
                End If
                runtimes += 1
            End While
            Return {d, 1}
            Return {0, 1.0}
        End Function

        ''' <summary>
        ''' Tests if the double value is an interger
        ''' </summary>
        Private Function IsInteger(d As Double) As Boolean
            Return CmpDbl(d, Math.Round(d), 0.00000001) = 0
        End Function

        ''' <summary>
        ''' Trinomial factoring Function (needs improvement)
        ''' </summary>
        Public Function TriFact(ByVal a As Double, ByVal b As Double, ByVal c As Double) As String
            Dim fact As Double = GCF(GCF(a, b), c)
            If a < 0 Then
                a = -a
                b = -b
                c = -c
                fact = -fact
            End If
            a /= fact
            b /= fact
            c /= fact
            Dim ac As Double = a * c
            Dim stp As Integer = 1
            Dim fa1 As Integer = -1
            Dim fa2 As Integer = -1
            Dim found As Boolean = False
            If (b < 0) Then stp = -1
            For i As Integer = -Int(b) To Int(b) Step stp
                'trifact(2,9,-5)
                If i * Int(b - i) = Int(ac) Then
                    fa1 = i
                    fa2 = Int(b - i)
                    found = True
                    Exit For
                End If
            Next
            If Not found Then
                Return "Not Factorable"
            Else
                Dim ff1 As Integer = Int(GCF(fa1, a))
                Dim ff2 As Integer = Int(GCF(fa2, a))
                Dim fm1 As Integer = Int(a / ff1)
                Dim fm2 As Integer = Int(a / ff2)
                fa1 \= ff1
                fa2 \= ff2

                Dim facttxt As String
                If fact = 1 Then
                    facttxt = "("
                ElseIf fact = -1
                    facttxt = "-("
                Else
                    facttxt = fact & "("
                End If

                Dim fm1txt As String = SignText(fm1)
                Dim fm2txt As String = SignText(fm2)
                Dim fa1sgn As String = If(fa1 > 0, "+", "")
                Dim fa2sgn As String = If(fa2 > 0, "+", "")
                If fm2txt & fa2sgn & fa2 = fm1txt & fa1sgn & fa1 Then
                    Return facttxt & fm1txt & "x" & fa1sgn & fa1 & ")^2"
                Else
                    Return facttxt & fm1txt & "x" & fa1sgn & fa1 & ")(" & fm2txt & "x" & fa2sgn & fa2 & ")"
                End If
            End If
        End Function

        ''' <summary>
        ''' Convert to scientific notation
        ''' </summary>
        ''' <param name="value">The double to use</param>
        ''' <returns>Scientific notation representation</returns>
        Private Function Sci(ByVal value As Double) As String
            If value = 0 Then Return "0"
            If Double.IsNaN(value) OrElse Double.IsInfinity(value) Then Return value.ToString()
            Dim log As Integer = Int(CDbl(Log10(Math.Abs(value))))
            Return value / Math.Pow(10, log) & " x 10^" & log
        End Function

        ''' <summary>
        ''' Convert a value to scientific notation
        ''' </summary>
        Private Function Sci(ByVal value As BigDecimal) As String
            Return value.ToScientific()
        End Function

        ' output modes
        ''' <summary>
        ''' Output as scientific notation
        ''' </summary>
        Private Function SciO(value As Object) As String
            If TypeOf value Is Double Then
                Return Sci(CDbl(value))
            ElseIf TypeOf value Is BigDecimal
                Return Sci(CType(value, BigDecimal))
            Else
                Return value.ToString()
            End If
        End Function

        ''' <summary>
        ''' Output directly
        ''' </summary>
        Private Function LineO(value As Object) As String ' use this to see results in linear fashion when in mathio mode
            If TypeOf value Is Double Then
                If Math.Abs(CDbl(value)) < 0.0001 OrElse Math.Abs(CDbl(value)) >= 1.0E+15 Then
                    ' switch to scientific for extreme values
                    Return SciO(value)
                Else
                    Return CDec(value).ToString("F4")
                End If
            ElseIf TypeOf value Is BigDecimal
                Return CType(value, BigDecimal).ToString()
            Else
                Return value.ToString()
            End If
        End Function

        ''' <summary>
        ''' Output as mathematical notation
        ''' </summary>
        Private Function MathO(o As Object) As String ' use this to see results in mathio fashion when in lineio mode
            If TypeOf o Is BigDecimal OrElse TypeOf o Is Double Then
                Dim value As Double
                If TypeOf o Is BigDecimal Then
                    If CType(o, BigDecimal).IsOutsideDispRange() Then
                        Return CType(o, BigDecimal).ToString()
                    Else
                        value = CDbl(CType(o, BigDecimal))
                    End If
                Else
                    value = CDbl(o)
                End If

                ' eliminate integer and undefined/non-real values.
                If Double.IsNaN(value) OrElse Double.IsInfinity(value) OrElse
                        CmpDbl(value, Math.Floor(value)) = 0 OrElse Math.Abs(value) < 0.0000001 OrElse
                        Math.Abs(value) > 10000000 Then
                    Return value.ToString()
                End If

                ' multiples of PI
                For j As Integer = -100 To 100
                    If j = 0 Then Continue For
                    For k As Integer = -20 To 20
                        Dim s As String = SimpStr(value, Math.PI, j, "π", "+", k)
                        If s <> "" Then Return s
                    Next
                Next
                Try
                    ' radicals
                    For i As Integer = 2 To 3
                        Dim sq As Double = value ^ i
                        If sq > 15000 Then Exit For ' ignore excessively large roots
                        If i Mod 2 = 0 Then sq *= Sgn(value)
                        If IsInteger(sq) Then
                            Return SimpRadical(sq, i)
                        End If
                    Next
                    For i As Integer = 2 To 3
                        For j As Integer = 1 To 40
                            Dim sq As Double = (value - j) ^ i
                            If sq > 15000 Then Exit For ' ignore excessively large roots
                            If i Mod 2 = 0 Then sq *= Sgn(value - j)
                            If IsInteger(sq) Then
                                Return (j & " + " & SimpRadical(sq, i)).Replace("+ -", "- ")
                            End If
                            sq = (value + j) ^ i
                            If sq > 15000 Then Exit For ' ignore excessively large roots
                            If i Mod 2 = 0 Then sq *= Sgn(value + j)
                            If IsInteger(sq) Then
                                Return SimpRadical(sq, i) & " - " & j
                            End If
                        Next
                    Next
                    For i As Integer = 2 To 3
                        For j As Integer = 0 To 40
                            For k As Integer = 2 To 40
                                Dim ba As Double = (value * k - j)
                                If IsInteger(ba) Then Continue For
                                Dim sq As Double = ba ^ i
                                If sq > 15000 Then Exit For ' ignore excessively large roots
                                If i Mod 2 = 0 Then sq *= Sgn(ba)
                                If IsInteger(sq) Then
                                    If j = 0 Then
                                        Return (SimpRadical(sq, i)).Replace("+ -", "- ") & "/" & k
                                    Else
                                        Return ("(" & j & " + " & SimpRadical(sq, i)).Replace("+ -", "- ") & ")/" & k
                                    End If
                                End If
                                sq = (value * k + j)
                                If IsInteger(sq) Then Continue For 'ignore integer base 
                                sq ^= i
                                If sq > 15000 Then Exit For ' ignore excessively large roots
                                If i Mod 2 = 0 Then sq *= Sgn(value + j)
                                If IsInteger(sq) Then
                                    If j = 0 Then
                                        Return SimpRadical(sq, i) & "/" & k
                                    Else
                                        Return "(" & SimpRadical(sq, i) & " - " & j & ")/" & k
                                    End If
                                End If
                            Next
                        Next
                    Next
                Catch ex As Exception
                End Try

                ' fractions of PI
                For j As Integer = 1 To 100
                    For k As Integer = -20 To 20
                        Dim s As String = SimpStr(value, Math.PI, 1 / j, "π", "+", k)
                        If s <> "" Then Return s
                        s = SimpStr(value, Math.PI, -1 / j, "π", "+", k)
                        If s <> "" Then Return s
                    Next
                Next

                ' try converting to fraction
                Return ToFrac(value)

                ' return original texting
                'Return value.ToString
            Else
                Return o.ToString() 'not double, don't simplify
            End If
        End Function

        ''' <summary>
        ''' Crazy helper function that checks for equality and then returns a symbol to use if equal
        ''' </summary>
        Private Function SimpStr(val As Double, num As Double, mul As Double, symb As String,
                             Optional ByVal addop As String = "", Optional ByVal addval As Integer = 0, Optional ByVal divop As String = "",
                             Optional ByVal divval As Double = 1) As String

            If CmpDbl(val, (num * mul + addval) / divval) = 0 Then

                Dim addtext As String = ""
                Dim divtext As String = ""
                If addop <> "" And addval <> 0 Then addtext = addval & " " & addop & " "
                If divval <> 1 Then
                    divtext = divop & divval
                    If addval <> 0 Or mul > 1 Then
                        addtext = "(" & addtext
                        divtext = ")" & divtext
                    End If
                End If
                If (mul = 1) Then
                    Return (addtext & symb & divtext).Replace("+ -", "- ")
                ElseIf mul = -1
                    Return (addtext & "-" & symb & divtext).Replace("+ -", "- ")
                Else
                    Return (addtext & ToFrac(mul) & symb & divtext).Replace("+ -", "- ")
                End If
            End If
            Return ""
        End Function

        ''' <summary>
        ''' Output using the current output mode. Also deals with data structures.
        ''' </summary>
        Friend Function O(value As Object) As String
            ' use Undefined instead of NaN
            If value Is Nothing OrElse (TypeOf value Is BigDecimal AndAlso CType(value, BigDecimal).IsUndefined) OrElse
                             (TypeOf value Is Double AndAlso Double.IsNaN(CDbl(value))) Then Return "Undefined"

            If TypeOf value Is Reference Then ' reference: display ampersand
                Return "&" & DirectCast(value, Reference).ToString()

            ElseIf TypeOf value Is Lambda Then ' lambda: display ampersand if function pointer
                Dim result As String = DirectCast(value, Lambda).ToString()
                If result.StartsWith("`") Then Return result
                Return "&" & result

            ElseIf TypeOf value Is String Then ' text: put quotes
                Return ControlChars.Quote & CStr(value) & ControlChars.Quote ' put quotes around textings

            ElseIf TypeOf value Is Double OrElse TypeOf value Is BigDecimal Then ' numbers: process (detect fractions, etc.)
                Dim ret As String
                If _eval.OutputFormat = Evaluator.eOutputFormat.Math Then
                    ret = MathO(value)
                ElseIf _eval.OutputFormat = Evaluator.eOutputFormat.Scientific Then
                    ret = SciO(value)
                Else
                    ret = LineO(value)
                End If
                Return ret

            Else
                Return DetectType(value).ToString() ' other stuff like sets, lists: use type-specific serialization
            End If
        End Function

        ''' <summary>
        ''' Evaluate an expression
        ''' </summary>
        Public Function Eval(value As Object) As Object
            Dim ans As Object = _eval.EvalExprRaw(value.ToString(), True)
            Return ans
        End Function

        ''' <summary>
        ''' Loop over a collection and apply an operation
        ''' </summary>
        Public Function [Each](lst As Object, operation As Lambda) As Object
            Dim loopLst As New List(Of Reference)
            If TypeOf lst Is IEnumerable(Of Reference) Then
                loopLst.AddRange(DirectCast(lst, IEnumerable(Of Reference)))
            ElseIf TypeOf lst Is IDictionary(Of Reference, Reference) Then
                loopLst.AddRange(ToList(DirectCast(lst, IDictionary(Of Reference, Reference))))
            End If
            For Each v As Object In loopLst
                _eval.SetDefaultVariable(New Reference(v))
                If TypeOf v Is Reference() Then
                    operation.Execute(_eval, DirectCast(v, Reference()))
                ElseIf TypeOf v Is Reference
                    operation.Execute(_eval, {DirectCast(v, Reference)})
                Else
                    operation.Execute(_eval, {New Reference(v)})
                End If
            Next
            Return lst
        End Function

        ''' <summary>
        ''' Ternary operator: if(condition, [true], [false])
        ''' </summary>
        Public Function [If](condition As Object, a As Object, b As Object) As Object
            If IsTrue(Eval(condition)) Then Return a Else Return b
        End Function

        ' lines and points for graphing
        Public Function LineSeg(x1 As Double, y1 As Double, x2 As Double, y2 As Double) As Double
            Dim var As Double = CDbl(_eval.GetVariable("x"c))
            If (var < x1 AndAlso var < x2) OrElse (var > x1 AndAlso var > x2) Then
                Return Double.NaN
            End If
            Return (y2 - y1) * (var - x1) / (x2 - x1) + y1
        End Function

        Public Function Line(x1 As Double, y1 As Double, x2 As Double, y2 As Double) As Double
            Dim var As Double = CDbl(_eval.GetVariable("x"c))
            Return (y2 - y1) * (var - x1) / (x2 - x1) + y1
        End Function

        Public Function RayFrom(x1 As Double, y1 As Double, x2 As Double, y2 As Double) As Double
            Dim var As Double = CDbl(_eval.GetVariable("x"c))
            If (x2 - x1) * (x1 - var) < 0 Then
                Return Double.NaN
            End If
            Return (y2 - y1) * (var - x1) / (x2 - x1) + y1
        End Function

        Public Function RayTo(x1 As Double, y1 As Double, x2 As Double, y2 As Double) As Double
            Dim var As Double = CDbl(_eval.GetVariable("x"c))
            If (x1 - x2) * (x2 - var) < 0 Then
                Return Double.NaN
            End If
            Return (y2 - y1) * (var - x1) / (x2 - x1) + y1
        End Function

        Private Function SignText(coefficient As Integer, Optional ByVal showpositive As Boolean = False) As String 'helper
            If coefficient = 1 Then
                If showpositive Then
                    Return "+"
                Else
                    Return ""
                End If
            ElseIf coefficient = -1
                Return "-1"
            Else
                Return coefficient.ToString()
            End If
        End Function

        Public Function Text(ByVal obj As Object) As String
            Return obj.ToString()
        End Function
        Public Function [Boolean](ByVal obj As Object) As Boolean
            Return IsTrue(obj)
        End Function
        Public Function [Char](ByVal id As Double) As String
            Return ChrW(Int(id))
        End Function
        Public Function Ascii(ByVal c As String) As Integer
            Return AscW(c(0))
        End Function

        Public Shadows Function ToString(ByVal obj As Object) As String
            Return obj.ToString()
        End Function
        Public Function Parse(ByVal text As String) As Object
            Try
                Return ParseN(text)
            Catch 'ex As Exception
                Try
                    Return ParseD(text)
                Catch
                    Return text
                End Try
            End Try
        End Function
        Public Function ParseN(ByVal text As String) As Double
            Return CDbl(New Number(text).GetValue())
        End Function
        Public Function ParseD(ByVal text As String) As System.DateTime
            Return System.DateTime.Parse(text)
        End Function

        ' texting operations
        ''' <summary>
        ''' Concatenate (join) two text objects
        ''' </summary>
        Public Function Concat(ByVal a As Object, ByVal b As Object) As Object
            If TypeOf a Is IList(Of Reference) AndAlso TypeOf b Is IList(Of Reference) Then
                Dim ac As IList(Of Reference) = DirectCast(New Matrix(DirectCast(
                                                 a, IList(Of Reference))).GetDeepCopy().GetValue(), IList(Of Reference))
                Return Append(ac, DirectCast(b, IList(Of Reference)))
            Else
                Return a.ToString() & b.ToString()
            End If
        End Function

        ''' <summary>
        ''' Convert the entire texting to lower case
        ''' </summary>
        Public Function ToLower(ByVal text As String) As String
            Return text.ToLowerInvariant()
        End Function

        ''' <summary>
        ''' Convert the entire texting to upper case
        ''' </summary>
        Public Function ToUpper(ByVal text As String) As String
            Return text.ToUpperInvariant()
        End Function

        ''' <summary>
        ''' Make the first letter of the text capitalized, if necessary
        ''' </summary>
        Public Function Capitalize(ByVal text As String) As String
            If text.Length <= 0 Then Return text
            text = text.ToLowerInvariant()
            Dim c As Char = Char.ToUpperInvariant(text(0))
            If text.Length <= 1 Then Return c
            text = c & text.Substring(1)
            Return text
        End Function

        ''' <summary>
        ''' Get the subtexting of the texting starting at the point specified with the length specified
        ''' </summary>
        Public Function Substring(ByVal text As String, ByVal fi As Double, Optional ByVal len As Double = -1) As String
            If len < 0 Then
                Return text.Substring(Int(Math.Truncate(fi)))
            Else
                Return text.Substring(Int(Math.Truncate(fi)), Int(Math.Truncate(len)))
            End If
        End Function

        ''' <summary>
        ''' Get the subtexting of the texting starting at the point specified with the length specified (alias for Substring)
        ''' </summary>
        Public Function Subtext(ByVal text As String, ByVal fi As Double, Optional ByVal len As Double = -1) As String
            Return Substring(text, fi, len)
        End Function

        ''' <summary>
        ''' Returns true if the given texting is empty
        ''' </summary>
        Public Function IsEmpty(ByVal obj As Object) As Boolean
            If TypeOf obj Is String Then
                Return String.IsNullOrEmpty(CStr(obj))
            ElseIf TypeOf obj Is IEnumerable(Of Reference) Then
                Return DirectCast(obj, IEnumerable(Of Reference)).Count = 0
            ElseIf TypeOf obj Is IDictionary(Of Reference, Reference) Then
                Return DirectCast(obj, IDictionary(Of Reference, Reference)).Count = 0
            ElseIf TypeOf obj Is Reference() Then
                Return DirectCast(obj, Reference()).Length = 0
            Else
                Return False
            End If
        End Function

        ''' <summary>
        ''' Returns true if the given texting is empty or white space
        ''' </summary>
        ''' <param name="text"></param>
        ''' <returns></returns>
        Public Function IsEmptyOrSpace(ByVal text As String) As Boolean
            Return String.IsNullOrWhiteSpace(text) Or String.IsNullOrEmpty(text)
        End Function

        ''' <summary>
        ''' Returns true if the given texting starts with the pattern (regex enabled)
        ''' </summary>
        Public Function StartsWith(ByVal text As String, ByVal pattern As String) As Boolean
            If Not pattern.StartsWith("^") Then pattern = "^" & pattern
            Return RegexMatch(text, pattern).Count > 0
        End Function

        ''' <summary>
        ''' Returns true if the given texting ends with the pattern (regex enabled)
        ''' </summary>
        Public Function EndsWith(ByVal text As String, ByVal pattern As String) As Boolean
            If Not pattern.EndsWith("$") Then pattern &= "$"
            Return RegexMatch(text, pattern).Count > 0
        End Function

        ''' <summary>
        ''' Join the matrix, set, or tuple with the specified separator
        ''' </summary>
        ''' <returns></returns>
        Public Function Join(ByVal sep As String, ByVal lst As Object, Optional ByVal ignoreEmpty As Boolean = False) As String
            Dim res As New StringBuilder()

            Dim lstc As IEnumerable(Of Reference)
            If TypeOf lst Is IEnumerable(Of Reference) Then
                lstc = DirectCast(lst, IEnumerable(Of Reference))
            ElseIf TypeOf lst Is Reference()
                lstc = CType(lst, Reference())
            ElseIf TypeOf lst Is IDictionary(Of Reference, Reference)
                lstc = DirectCast(lst, IDictionary(Of Reference, Reference)).Keys
            Else
                Return ""
            End If

            Dim init As Boolean = True

            For Each r As Reference In lstc
                Dim cur As String
                If TypeOf r.Resolve() Is String Then
                    cur = CStr(r.Resolve())
                Else
                    cur = r.ToString()
                End If
                If Not ignoreEmpty OrElse Not String.IsNullOrWhiteSpace(cur) Then
                    If Not init Then res.Append(sep) Else init = False
                    res.Append(cur)
                End If
            Next
            Return res.ToString()
        End Function

        ' regex, wildcards
        ''' <summary>
        ''' Checks if the current text matches the regex pattern
        ''' </summary>
        ''' <param name="text"></param>
        ''' <param name="pattern"></param>
        ''' <returns></returns>
        Public Function RegexIsMatch(ByVal text As String, ByVal pattern As String) As Boolean
            Dim regex As New Regex(pattern)
            Return regex.IsMatch(text)
        End Function

        ''' <summary>
        ''' Find the first occurrence of the pattern in the texting using regex
        ''' </summary>
        ''' <param name="text"></param>
        ''' <param name="pattern"></param>
        ''' <returns></returns>
        Public Function RegexMatch(ByVal text As String, ByVal pattern As String) As IEnumerable(Of Reference)
            Dim regex As New Regex(pattern)
            Dim result As New List(Of Reference)
            Dim match As Match = regex.Match(text)
            If Not match.Success Then Return result ' if failed, return empty list
            If match.Groups.Count > 1 Then
                For i As Integer = 1 To match.Groups.Count - 1
                    result.Add(New Reference(match.Groups(i).Value))
                Next
            Else
                result.Add(New Reference(match.Value))
            End If
            Return result
        End Function

        ''' <summary>
        ''' Find all occurrences of the pattern in the texting using regex
        ''' </summary>
        ''' <param name="text"></param>
        ''' <param name="pattern"></param>
        ''' <returns></returns>
        Public Function RegexMatchAll(ByVal text As String, ByVal pattern As String) As IEnumerable(Of Reference)
            Dim regex As New Regex(pattern)
            Dim result As New List(Of Reference)
            For Each match As Match In regex.Matches(text)
                If Not match.Success Then Continue For ' if failed, skip
                If match.Groups.Count > 1 Then
                    Dim curlst As New SortedDictionary(Of Reference, Reference)
                    For i As Integer = 1 To match.Groups.Count - 1
                        If Not match.Groups(i).Success Then Continue For ' if failed, skip
                        curlst(New Reference(regex.GroupNameFromNumber(i))) =
                            (New Reference(match.Groups(i).Value))
                    Next
                    result.Add(New Reference(New [Set](curlst)))
                Else
                    result.Add(New Reference(match.Value))
                End If
            Next
            Return result
        End Function

        Public Function WildCardMatch(ByVal text As String, ByVal pattern As String) As Boolean
            Return text Like pattern
        End Function


        ' matrix/list stuff

        ''' <summary>
        ''' Get the item at the index, wrapping around if out of range
        ''' </summary>
        Public Function Index(ByVal lst As Object, val As Object) As Object
            If TypeOf lst Is IList(Of Reference) Then
                If (TypeOf val Is Double OrElse TypeOf val Is BigDecimal OrElse TypeOf val Is Integer) AndAlso
                                     (Len(lst) > CDbl(val) AndAlso -Len(lst) <= CDbl(val)) Then
                    If CDbl(val) < 0 Then val = CType(lst, IEnumerable(Of Reference)).Count + CDbl(val)
                    Return DirectCast(lst, IEnumerable(Of Reference))(Int(CDbl(val)))
                Else
                    Return "Index Is Out Of Range"
                End If
            ElseIf TypeOf lst Is LinkedList(Of Reference) Then ' WARNING: inefficient
                If (TypeOf val Is Double OrElse TypeOf val Is BigDecimal OrElse TypeOf val Is Integer) AndAlso
                                     (Len(lst) > CDbl(val) AndAlso -Len(lst) <= CDbl(val)) Then
                    If CDbl(val) < 0 Then val = CType(lst, LinkedList(Of Reference)).Count + CDbl(val)
                    Dim first As LinkedListNode(Of Reference) = DirectCast(lst, LinkedList(Of Reference)).First
                    For i As Integer = 1 To Int(CDbl(val))
                        first = first.Next
                    Next
                    Return New Reference(first)
                Else
                    Return "Index Is Out Of Range"
                End If
            ElseIf TypeOf lst Is Reference() Then
                If (TypeOf val Is Double OrElse TypeOf val Is BigDecimal OrElse TypeOf val Is Integer) AndAlso
                                     (Len(lst) > CDbl(val) AndAlso -Len(lst) <= CDbl(val)) Then
                    If CDbl(val) < 0 Then val = CType(lst, Reference()).Length + CDbl(val)
                    Return DirectCast(lst, Reference())(Int(CDbl(val)))
                Else
                    Return "Index Is Out Of Range"
                End If
            ElseIf TypeOf lst Is IDictionary(Of Reference, Reference)
                Try
                    Dim res As Object = DirectCast(lst, IDictionary(Of Reference, Reference))(New Reference(ObjectTypes.DetectType(val)))
                    If res Is Nothing Then Return Double.NaN
                    Return res
                Catch
                    Return Double.NaN
                End Try
            ElseIf TypeOf lst Is String
                If (TypeOf val Is Double OrElse TypeOf val Is BigDecimal OrElse TypeOf val Is Integer) AndAlso
                                     (Len(lst) > CDbl(val) AndAlso -Len(lst) <= CDbl(val)) Then
                    If CDbl(val) < 0 Then val = CType(lst, Reference()).Length + CDbl(val)
                    Return DirectCast(lst, String)(Int(CDbl(val))).ToString()
                Else
                    Return "Index Is Out Of Range"
                End If
            Else
                Return Double.NaN
            End If
        End Function

        ''' <summary>
        ''' Get the item at the index, wrapping around if out of range
        ''' </summary>
        Public Function IndexCircular(ByVal lst As Object, val As Object) As Object
            If TypeOf lst Is IList(Of Reference) Then
                If (TypeOf val Is Double OrElse TypeOf val Is BigDecimal OrElse TypeOf val Is Integer) Then
                    val = Modulo(Int(CDbl(val)), Len(lst))
                    Return DirectCast(lst, IEnumerable(Of Reference))(Int(CDbl(val)))
                Else
                    Return "Index Must Be A Number"
                End If
            ElseIf TypeOf lst Is LinkedList(Of Reference) Then ' WARNING: inefficient
                If (TypeOf val Is Double OrElse TypeOf val Is BigDecimal OrElse TypeOf val Is Integer) Then
                    val = Modulo(Int(CDbl(val)), Len(lst))
                    Dim first As LinkedListNode(Of Reference) = DirectCast(lst, LinkedList(Of Reference)).First
                    For i As Integer = 1 To Int(CDbl(val))
                        first = first.Next
                    Next
                    Return New Reference(first)
                Else
                    Return "Index Is Out Of Range"
                End If
            ElseIf TypeOf lst Is Reference() Then
                If (TypeOf val Is Double OrElse TypeOf val Is BigDecimal OrElse TypeOf val Is Integer) Then
                    val = Modulo(Int(CDbl(val)), Len(lst))
                    Return DirectCast(lst, Reference())(Int(CDbl(val)))
                Else
                    Return "Index Must Be A Number"
                End If
            ElseIf TypeOf lst Is IDictionary(Of Reference, Reference)
                Try
                    Dim res As Object = DirectCast(lst, IDictionary(Of Reference, Reference))(New Reference(ObjectTypes.DetectType(val)))
                    If res Is Nothing Then Return Double.NaN
                    Return res
                Catch
                    Return Double.NaN
                End Try
            ElseIf TypeOf lst Is String
                If (TypeOf val Is Double OrElse TypeOf val Is BigDecimal OrElse TypeOf val Is Integer) Then
                    val = Modulo(Int(CDbl(val)), Len(lst))
                    Return DirectCast(lst, String)(Int(CDbl(val))).ToString()
                Else
                    Return "Index Must Be A Number"
                End If
            Else
                Return Double.NaN
            End If
        End Function

        Public Function At(lst As Object, val As Object) As Object
            Return IndexCircular(lst, val)
        End Function

        Public Function SetAt(lst As Object, idx As Object, val As Object) As Object
            If TypeOf lst Is IList(Of Reference) Then
                If TypeOf idx Is Double AndAlso (Len(lst) > CDbl(idx) AndAlso CDbl(idx) >= 0) Then
                    DirectCast(lst, IList(Of Reference)).Item(Int(CDbl(idx))) = New Reference(ObjectTypes.DetectType(val))
                End If
            ElseIf TypeOf lst Is Reference() Then
                If TypeOf idx Is Double AndAlso (Len(lst) > CDbl(idx) AndAlso CDbl(idx) >= 0) Then
                    DirectCast(lst, Reference())(Int(CDbl(idx))) = New Reference(ObjectTypes.DetectType(val))
                End If
            ElseIf TypeOf lst Is IDictionary(Of Reference, Reference) Then
                DirectCast(lst, IDictionary(Of Reference, Reference))(New Reference(ObjectTypes.DetectType(idx))) = New Reference(ObjectTypes.DetectType(val))
            Else
                Return "Index Out Of Range"
            End If
            Return lst
        End Function

        Public Function SetAtCircular(lst As Object, idx As Object, val As Object) As Object
            If TypeOf lst Is IList(Of Reference) Then
                If TypeOf idx Is Double Then
                    idx = Modulo(CDbl(idx), DirectCast(lst, IList(Of Reference)).Count)
                    DirectCast(lst, IList(Of Reference)).Item(Int(CDbl(idx))) = New Reference(ObjectTypes.DetectType(val))
                End If
            ElseIf TypeOf lst Is Reference() Then
                If TypeOf idx Is Double AndAlso (Len(lst) > CDbl(idx) AndAlso CDbl(idx) >= 0) Then
                    DirectCast(lst, Reference())(Int(CDbl(idx))) = New Reference(ObjectTypes.DetectType(val))
                End If
            ElseIf TypeOf lst Is IDictionary(Of Reference, Reference) Then
                DirectCast(lst, IDictionary(Of Reference, Reference))(New Reference(ObjectTypes.DetectType(idx))) = New Reference(ObjectTypes.DetectType(val))
            Else
                Return "Index Out Of Range"
            End If
            Return lst
        End Function

        Public Function Slice(lst As Object, Optional a As Double = Double.NaN, Optional b As Double = Double.NaN) As Object
            Dim reverse As Boolean = False
            If Double.IsNaN(b) Then b = Len(lst)
            If Double.IsNaN(a) Then a = 0

            a = Math.Truncate(a)
            b = Math.Truncate(b)

            If b < 0 Then b = Len(lst) + b
            If a < 0 Then a = Len(lst) + a

            If b < a Then
                reverse = True
                Dim t As Double = a
                a = b
                b = t
            End If

            ' allow out of range
            If b > Len(lst) Then b = Len(lst)
            If a < 0 Then a = 0

            If TypeOf lst Is IEnumerable(Of Reference) Then
                Dim rlst As New List(Of Reference)(DirectCast(lst, IEnumerable(Of Reference)))
                rlst.RemoveRange(Int(b), Int(Len(rlst) - b))
                rlst.RemoveRange(0, Int(a))
                If reverse Then rlst.Reverse()
                Return rlst
            ElseIf TypeOf lst Is Reference() Then
                Dim lst2 As New List(Of Reference)(DirectCast(lst, Reference()))
                lst2.RemoveRange(Int(b), Int(Len(lst2) - b))
                lst2.RemoveRange(0, Int(a))
                If reverse Then lst2.Reverse()
                Return lst2
            ElseIf TypeOf lst Is String
                Dim text As String
                If b = CStr(lst).Length Then
                    text = CStr(lst).Substring(Int(a))
                Else
                    text = CStr(lst).Remove(Int(b)).Substring(Int(a))
                End If
                If reverse Then Return Me.Reverse(text)
                Return text
            Else
                Return Double.NaN
            End If
            Return lst
        End Function

        ''' <summary>
        ''' Add an item to a list-matrix or setionary-set
        ''' </summary>
        Public Function Add(lst As Object, val As Object) As Object
            If TypeOf lst Is IList(Of Reference) Then
                DirectCast(lst, IList(Of Reference)).Add(New Reference(ObjectTypes.DetectType(val)))
            ElseIf TypeOf lst Is LinkedList(Of Reference) Then
                DirectCast(lst, LinkedList(Of Reference)).AddLast(New Reference(ObjectTypes.DetectType(val)))
            ElseIf TypeOf lst Is IDictionary(Of Reference, Reference) Then
                If TypeOf val Is IEnumerable(Of Reference) AndAlso CType(val, IEnumerable(Of Reference)).Count = 2 Then
                    Dim valLst As IEnumerable(Of Reference) = CType(val, IEnumerable(Of Reference))
                    DirectCast(lst, IDictionary(Of Reference, Reference)).Add(valLst(0), valLst(1))
                ElseIf TypeOf val Is IDictionary(Of Reference, Reference)
                    lst = Union(CType(lst, IDictionary(Of Reference, Reference)), CType(val, IDictionary(Of Reference, Reference)))
                Else
                    DirectCast(lst, IDictionary(Of Reference, Reference))(New Reference(ObjectTypes.DetectType(val))) = Nothing
                End If
            Else
                Return Nothing
            End If
            Return lst
        End Function

        ''' <summary>
        ''' Add a blank row to the specified matrix
        ''' </summary>
        ''' <param name="lst"></param>
        ''' <returns></returns>
        Public Function AddRow(lst As IEnumerable(Of Reference)) As IEnumerable(Of Reference)
            Dim mat As New Matrix(DirectCast(lst, IEnumerable(Of Reference)))
            mat.Resize(mat.Height + 1, mat.Width)
            Return DirectCast(mat.GetValue(), IEnumerable(Of Reference))
            Return lst
        End Function

        ''' <summary>
        ''' Add a blank column to the specified matrix
        ''' </summary>
        ''' <param name="lst"></param>
        ''' <returns></returns>
        Public Function AddCol(lst As IEnumerable(Of Reference)) As IEnumerable(Of Reference)
            Dim mat As New Matrix(DirectCast(lst, IEnumerable(Of Reference)))
            mat.Resize(mat.Height, mat.Width + 1)
            Return DirectCast(mat.GetValue(), IEnumerable(Of Reference))
            Return lst
        End Function

        ''' <summary>
        ''' Append the second list onto the first list
        ''' </summary>
        ''' <param name="lst"></param>
        ''' <param name="val"></param>
        ''' <returns></returns>
        Public Function Append(lst As IEnumerable(Of Reference), val As IEnumerable(Of Reference)) As IEnumerable(Of Reference)
            If TypeOf lst Is List(Of Reference) Then
                DirectCast(lst, List(Of Reference)).AddRange(val.ToArray())
            ElseIf TypeOf lst Is LinkedList(Of Reference) Then
                For Each r As Reference In val
                    DirectCast(lst, LinkedList(Of Reference)).AddLast(r)
                Next
            Else
                Return Nothing ' not supported
            End If
            Return lst
        End Function

        ''' <summary>
        ''' Remove the first object matching the specified object within the list
        ''' </summary>
        Public Function Remove(lst As Object, Optional val As Object = Nothing) As Object
            If TypeOf lst Is IList(Of Reference) Then
                Dim i As Integer = 0
                Dim tmp As IList(Of Reference) = DirectCast(lst, IList(Of Reference))
                While i < tmp.Count
                    If GenericComparer.CompareObjs(tmp(i), val) = 0 Then
                        tmp.RemoveAt(i)
                        Exit While
                    End If
                    i += 1
                End While
                Return tmp
            ElseIf TypeOf lst Is LinkedList(Of Reference) Then
                Dim tmp As LinkedListNode(Of Reference) = DirectCast(lst, LinkedList(Of Reference)).First
                While Not tmp Is Nothing
                    If GenericComparer.CompareObjs(tmp.Value, val) = 0 Then
                        tmp.List.Remove(tmp)
                        Exit While
                    End If
                    tmp = tmp.Next
                End While
                Return tmp.List
            ElseIf TypeOf lst Is IDictionary(Of Reference, Reference) Then
                Dim tmp As IDictionary(Of Reference, Reference) = DirectCast(lst, IDictionary(Of Reference, Reference))
                tmp.Remove(New Reference(ObjectTypes.DetectType(val)))
                Return tmp
            ElseIf TypeOf lst Is String Then
                If CStr(lst).Contains(val.ToString()) Then
                    lst = CStr(lst).Remove(CStr(lst).IndexOf(val.ToString())) &
                    CStr(lst).Substring(CStr(lst).IndexOf(val.ToString()) + val.ToString().Length)
                End If
                Return lst
            ElseIf TypeOf lst Is Reference Then
                DirectCast(lst, Reference).NodeRemove()
                Return DirectCast(lst, Reference)
            Else
                Return Nothing
            End If
        End Function

        ''' <summary>
        ''' Remove all objects matching the specified object within the list
        ''' </summary>
        Public Function RemoveAll(lst As Object, val As Object) As Object
            If TypeOf lst Is IList(Of Reference) Then
                Dim i As Integer = 0
                Dim tmp As IList(Of Reference) = DirectCast(lst, IList(Of Reference))
                While i < tmp.Count
                    If GenericComparer.CompareObjs(tmp(i), val) = 0 Then
                        tmp.RemoveAt(i)
                        Continue While
                    End If
                    i += 1
                End While
            ElseIf TypeOf lst Is LinkedList(Of Reference) Then
                Dim tmp As LinkedListNode(Of Reference) = DirectCast(lst, LinkedList(Of Reference)).First
                While Not tmp Is Nothing
                    If GenericComparer.CompareObjs(tmp.Value, val) = 0 Then
                        Dim nxt As LinkedListNode(Of Reference) = tmp.Next
                        tmp.List.Remove(tmp)
                        tmp = nxt
                    End If
                    tmp = tmp.Next
                End While
                Return tmp.List
            ElseIf TypeOf lst Is IDictionary(Of Reference, Reference)
                Return Remove(lst, val)
            ElseIf TypeOf lst Is String
                While CStr(lst).Contains(val.ToString())
                    lst = CStr(lst).Remove(CStr(lst).IndexOf(val.ToString())) & CStr(lst).Substring(CStr(lst).IndexOf(val.ToString()) + 1)
                End While
            Else
                Return Nothing
            End If
            Return lst
        End Function

        ''' <summary>
        ''' Reverse the entire dimension of the matrix
        ''' </summary>
        ''' <returns></returns>
        Public Function Reverse(lst As Object) As Object
            If TypeOf lst Is IEnumerable(Of Reference) Then
                DirectCast(lst, IEnumerable(Of Reference)).Reverse()
            ElseIf TypeOf lst Is String
                Dim slst As IEnumerable(Of Char) = CStr(lst).ToList()
                slst.Reverse()
                Dim s As New StringBuilder()
                For Each c As Char In slst
                    s.Append(c)
                Next
                Return s.ToString()
            Else
                Return Nothing
            End If
            Return lst
        End Function

        ''' <summary>
        ''' Return a reference to the first item in the linked list
        ''' </summary>
        Public Function First(lst As LinkedList(Of Reference)) As Reference
            Return New Reference(DirectCast(lst, LinkedList(Of Reference)).First)
        End Function

        ''' <summary>
        ''' Return a reference to the last item in the linked list
        ''' </summary>
        Public Function Last(lst As LinkedList(Of Reference)) As Reference
            Return New Reference(DirectCast(lst, LinkedList(Of Reference)).Last)
        End Function

        ''' <summary>
        ''' Tries to move the reference to the next place on the linkedlist the specified number of times
        ''' </summary>
        Public Function [Next](ref As Reference, Optional ByVal times As Double = 1) As Reference
            For i As Double = 0 To times - 1
                ref.NodeNext()
            Next
            Return ref
        End Function

        ''' <summary>
        ''' Tries to move the reference to the previous place on the linkedlist the specified number of times
        ''' </summary>
        Public Function Prev(ref As Reference, Optional ByVal times As Double = 1) As Reference
            For i As Double = 0 To times - 1
                ref.NodePrevious()
            Next
            Return ref
        End Function

        ''' <summary>
        ''' Tries to add an item after the one pointed to by the reference on the linked list 
        ''' </summary>
        Public Function AddAfter(ref As Reference, ByVal val As Object) As Reference
            ref.NodeAddAfter(New Reference(val))
            Return ref
        End Function

        ''' <summary>
        ''' Tries to add an item before the one pointed to by the reference on the linked list 
        ''' </summary>
        Public Function AddBefore(ref As Reference, ByVal val As Object) As Reference
            ref.NodeAddBefore(New Reference(val))
            Return ref
        End Function

        ''' <summary>
        ''' Take an item from the end of the list, remove it and return it.
        ''' </summary>
        Public Function Pop(lst As Object) As Object
            If TypeOf lst Is IList(Of Reference) Then
                Dim lr As IList(Of Reference) = DirectCast(lst, IList(Of Reference))
                If lr.Count = 0 Then Return Double.NaN
                Dim last As Object = lr(lr.Count - 1)
                lr.RemoveAt(lr.Count - 1)
                Return last
            ElseIf TypeOf lst Is LinkedList(Of Reference) Then
                Dim lr As LinkedList(Of Reference) = DirectCast(lst, LinkedList(Of Reference))
                If lr.Count = 0 Then Return Double.NaN
                Dim last As Object = lr.Last.Value.Resolve()
                lr.RemoveLast()
                Return last
            ElseIf TypeOf lst Is String Then
                If CStr(lst).Length = 0 Then Return Double.NaN
                Dim last As String = CStr(lst).Substring(CStr(lst).Length - 1)
                lst = CStr(lst).Remove(CStr(lst).Length - 1)
                Return last
            Else
                Return Double.NaN
            End If
        End Function

        ''' <summary>
        ''' Take an item from the end of the list, remove it and return it.
        ''' </summary>
        Public Function PopLast(lst As Object) As Object
            Return Pop(lst)
        End Function

        ''' <summary>
        ''' Take an item from the start of the list, remove it and return it.
        ''' </summary>
        Public Function PopFirst(lst As Object) As Object
            If TypeOf lst Is IList(Of Reference) Then
                Dim lr As IList(Of Reference) = DirectCast(lst, IList(Of Reference))
                If lr.Count = 0 Then Return Double.NaN
                Dim first As Object = lr(0)
                lr.RemoveAt(0)
                Return first
            ElseIf TypeOf lst Is LinkedList(Of Reference) Then
                Dim lr As LinkedList(Of Reference) = DirectCast(lst, LinkedList(Of Reference))
                If lr.Count = 0 Then Return Double.NaN
                Dim first As Object = lr.First.Value.Resolve()
                lr.RemoveFirst()
                Return first
            ElseIf TypeOf lst Is String Then
                If CStr(lst).Length = 0 Then Return Double.NaN
                Dim first As String = CStr(lst).Remove(1)
                lst = CStr(lst).Substring(1)
                Return first
            Else
                Return Double.NaN
            End If
        End Function

        ''' <summary>
        ''' Push an item to the end of the list
        ''' </summary>
        Public Function Push(lst As Object, obj As Object) As Object
            If TypeOf lst Is IList(Of Reference) Then
                Dim lr As IList(Of Reference) = DirectCast(lst, IList(Of Reference))
                lr.Add(New Reference(obj))
                Return lr
            ElseIf TypeOf lst Is LinkedList(Of Reference) Then
                Dim lr As LinkedList(Of Reference) = DirectCast(lst, LinkedList(Of Reference))
                lr.AddLast(New Reference(obj))
                Return lr
            ElseIf TypeOf lst Is String Then
                lst = CStr(lst) & obj.ToString()
                Return lst
            Else
                Return Nothing
            End If
        End Function

        ''' <summary>
        ''' Push an item to the end of the list
        ''' </summary>
        Public Function PushLast(lst As Object, obj As Object) As Object
            Return Push(lst, obj)
        End Function

        ''' <summary>
        ''' Push an item to the beginning of the list
        ''' </summary>
        Public Function PushFirst(lst As Object, obj As Object) As Object
            If TypeOf lst Is IList(Of Reference) Then
                Dim lr As IList(Of Reference) = DirectCast(lst, IList(Of Reference))
                lr.Insert(0, New Reference(obj))
                Return lr
            ElseIf TypeOf lst Is LinkedList(Of Reference) Then
                Dim lr As LinkedList(Of Reference) = DirectCast(lst, LinkedList(Of Reference))
                lr.AddFirst(New Reference(obj))
                Return lr
            ElseIf TypeOf lst Is String Then
                lst = obj.ToString() & CStr(lst)
                Return lst
            Else
                Return Nothing
            End If
        End Function

        ''' <summary>
        ''' Cycle the list forwards: [1,2,3]->[3,1,2]
        ''' </summary>
        Public Function Cycle(lst As Object, Optional times As Double = 1) As Object
            Dim reverse As Boolean = False
            If times < 0 Then
                reverse = True
                times = -times
            End If
            For i As Integer = 0 To Int(times) - 1
                If reverse Then
                    Push(lst, PopFirst(lst))
                Else
                    PushFirst(lst, Pop(lst))
                End If
            Next
            Return lst
        End Function

        ''' <summary>
        ''' Cycle the list backwards: [1,2,3]->[2,3,1]
        ''' </summary>
        Public Function CycleReverse(lst As Object, Optional times As Double = 1) As Object
            Return Cycle(lst, -times)
        End Function

        ''' <summary>
        ''' Remove at the index in the matrix-list
        ''' </summary>
        Public Function RemoveAt(lst As Object, val As Double) As Object
            If TypeOf lst Is IList(Of Reference) Then
                DirectCast(lst, IList(Of Reference)).RemoveAt(Int(val))
            ElseIf TypeOf lst Is linkedList(Of Reference) Then
                Dim tmp As LinkedListNode(Of Reference) = DirectCast(lst, LinkedList(Of Reference)).First
                For i As Integer = 1 To Int(val)
                    tmp = tmp.Next
                Next
                tmp.List.Remove(tmp)
            ElseIf TypeOf lst Is String Then
                Dim idx As Integer = CStr(lst).IndexOf(val.ToString())
                lst = CStr(lst).Remove(idx) + CStr(lst).Substring(idx + 1)
            Else
                Return Nothing
            End If
            Return lst
        End Function

        ''' <summary>
        ''' Count the number of times the given value occurs within the matrix, set, or texting.
        ''' Note: for sets, if the set is in setionary form ({a:b,c:d}), 
        ''' then this counts the number of times the value, not key occurs, in the set (i.e. b and d are checked)
        ''' Otherwise, this simply returns one because elements of the set are unique.
        ''' Note2: Regex enabled for textings
        ''' </summary>
        Public Function Count(lst As Object, val As Object) As Double
            Dim ct As Integer = 0
            If TypeOf lst Is IEnumerable(Of Reference) Then
                For Each i As Object In DirectCast(lst, IEnumerable)
                    If O(i) = O(val) Then
                        ct += 1
                    End If
                Next
            ElseIf TypeOf lst Is LinkedList(Of Reference) Then
                For Each i As Object In DirectCast(lst, LinkedList(Of Reference))
                    If O(i) = O(val) Then
                        ct += 1
                    End If
                Next
            ElseIf TypeOf lst Is IDictionary(Of Reference, Reference)
                Dim notNullCt As Integer = 0
                For Each i As KeyValuePair(Of Reference, Reference) In DirectCast(lst, IDictionary(Of Reference, Reference))
                    If i.Value Is Nothing Then Continue For
                    If O(i.Value) = O(val) Then ct += 1
                    notNullCt += 1
                Next
                If notNullCt <= 0 Then Return 1
            ElseIf TypeOf lst Is String
                For i As Integer = 0 To CStr(lst).Length
                    If StartsWith(CStr(lst).Substring(i), val.ToString) Then ct += 1
                Next
            Else
                Return 1
            End If
            Return ct
        End Function

        ''' <summary>
        ''' Clear the matrix or set
        ''' </summary>
        Public Function Clear(lst As Object) As Object
            If TypeOf lst Is IList(Of Reference) Then
                DirectCast(lst, IList(Of Reference)).Clear()
            ElseIf TypeOf lst Is LinkedList(Of Reference) Then
                DirectCast(lst, LinkedList(Of Reference)).Clear()
            ElseIf TypeOf lst Is IDictionary(Of Reference, Reference) Then
                DirectCast(lst, IDictionary(Of Reference, Reference)).Clear()
            Else
                Return Nothing
            End If
            Return lst
        End Function

        ''' <summary>
        ''' Returns true if the specified matrix, text, or set contains the value. (regex enabled for texting)
        ''' </summary>
        Public Function Contains(lst As Object, val As Object) As Boolean
            If TypeOf lst Is IEnumerable(Of Reference) Then
                Return DirectCast(lst, IEnumerable(Of Reference)).Contains(New Reference(ObjectTypes.DetectType(val)))
            ElseIf TypeOf lst Is IDictionary(Of Reference, Reference)
                Return ContainsKey(DirectCast(lst, IDictionary(Of Reference, Reference)), val)
            ElseIf TypeOf lst Is String
                Return Not RegexMatch(CStr(lst), val.ToString()).Count = 0
            Else
                Return False
            End If
        End Function

        ''' <summary>
        ''' find the specified object or subtexting within the array or texting from the beginning (regex enabled for textings)
        ''' </summary>
        ''' <param name="lst"></param>
        ''' <param name="val"></param>
        ''' <returns></returns>
        Public Function Find(lst As Object, val As Object) As Double
            Dim res As Double
            If TypeOf lst Is IList(Of Reference) Then
                res = DirectCast(lst, IList(Of Reference)).IndexOf(New Reference(ObjectTypes.DetectType(val)))
            ElseIf TypeOf lst Is String
                Dim regex As New Regex(val.ToString())
                res = regex.Match(CStr(lst)).Index
                If res < 0 Then res = Double.NaN
            Else
                Return Double.NaN
            End If
            If res < 0 Then Return Double.NaN
            Return res
        End Function

        ''' <summary>
        ''' find the specified object or subtexting within the array or texting from the end (regex enabled for textings)
        ''' </summary>
        ''' <param name="lst"></param>
        ''' <param name="val"></param>
        ''' <returns></returns>
        Public Function FindEnd(lst As Object, val As Object) As Double
            Dim res As Double
            If TypeOf lst Is List(Of Reference) Then
                res = DirectCast(lst, List(Of Reference)).LastIndexOf(New Reference(ObjectTypes.DetectType(val)))
            ElseIf TypeOf lst Is String
                Dim regex As New Regex(val.ToString(), RegexOptions.RightToLeft)
                res = regex.Match(CStr(lst)).Index
                If res < 0 Then res = Double.NaN
            Else
                Return Double.NaN
            End If
            If res < 0 Then Return Double.NaN
            Return res
        End Function

        ''' <summary>
        ''' replace first instance of the value with in the texting with the new value
        ''' </summary>
        ''' <returns></returns>
        Public Function ReplaceFirst(text As String, oldVal As String, newVal As String) As String
            Dim regex As New Regex(oldVal.ToString())
            Dim m As Match = regex.Match(text)
            Dim sb As New StringBuilder(text)
            If m.Groups.Count > 1 Then
                ' replace captures
                For j As Integer = m.Groups.Count - 1 To 1 Step -1
                    sb.Remove(m.Groups(j).Index, m.Groups(j).Length)
                    sb.Insert(m.Groups(j).Index, newVal)
                Next
            Else
                'replace everything
                sb.Remove(m.Index, m.Length)
                sb.Insert(m.Index, newVal)
            End If
            Return sb.ToString()
        End Function

        ''' <summary>
        ''' replace last instance of the value with in the texting with the new value
        ''' </summary>
        ''' <returns></returns>
        Public Function ReplaceLast(text As String, oldVal As String, newVal As String) As String
            Dim regex As New Regex(oldVal.ToString(), RegexOptions.RightToLeft)
            Dim m As Match = regex.Match(text)
            Dim sb As New StringBuilder(text)
            If m.Groups.Count > 1 Then
                ' replace captures
                For j As Integer = m.Groups.Count - 1 To 1 Step -1
                    sb.Remove(m.Groups(j).Index, m.Groups(j).Length)
                    sb.Insert(m.Groups(j).Index, newVal)
                Next
            Else
                'replace everything
                sb.Remove(m.Index, m.Length)
                sb.Insert(m.Index, newVal)
            End If
            Return sb.ToString()
        End Function

        ''' <summary>
        ''' replace all visible instances of the value with in the texting with the new value
        ''' </summary>
        ''' <returns></returns>
        Public Function Replace(text As String, oldVal As String, newVal As String) As String
            Dim regex As New Regex(oldVal.ToString())
            Dim res As MatchCollection = regex.Matches(text)
            Dim sb As New StringBuilder(text)
            For i As Integer = res.Count - 1 To 0 Step -1
                Dim m As Match = res(i)
                If m.Groups.Count > 1 Then
                    ' replace captures
                    For j As Integer = m.Groups.Count - 1 To 1 Step -1
                        sb.Remove(m.Groups(j).Index, m.Groups(j).Length)
                        sb.Insert(m.Groups(j).Index, newVal)
                    Next
                Else
                    'replace everything
                    sb.Remove(m.Index, m.Length)
                    sb.Insert(m.Index, newVal)
                End If
            Next
            Return sb.ToString()
        End Function

        ''' <summary>
        ''' replace all instances of the value with in the texting with the new value
        ''' repeats until the value no longer exists at all
        ''' e.g. replace("1233333","123","312") will return 3123333 while this will return 33333`12
        ''' </summary>
        ''' <returns></returns>
        Public Function ReplaceAll(text As String, oldVal As String, newVal As String) As String
            While Contains(text, oldVal)
                text = Replace(text, oldVal, newVal)
            End While
            Return text
        End Function

        ''' <summary>
        ''' add to the left side of the texting or matrix/list until it is at least the specified length.
        ''' </summary>
        ''' <param name="lst"></param>
        ''' <param name="length"></param>
        ''' <returns></returns>
        Public Function Pad(lst As Object, length As Double, Optional item As Object = Nothing) As Object
            If TypeOf lst Is IList(Of Reference) Then
                If item Is Nothing Then item = 0.0
                Dim rlst As IList(Of Reference) = DirectCast(lst, IList(Of Reference))
                While Len(rlst) < Int(length)
                    rlst.Insert(0, New Reference(item))
                End While
                Return rlst
            ElseIf TypeOf lst Is String
                If item Is Nothing Then item = " "
                Return CStr(lst).PadLeft(Int(length), item.ToString()(0))
            Else
                Return Double.NaN
            End If
        End Function

        ''' <summary>
        ''' add to the right side of the texting or matrix/list until it is at least the specified length.
        ''' </summary>
        ''' <param name="lst"></param>
        ''' <param name="length"></param>
        ''' <returns></returns>
        Public Function PadEnd(lst As Object, length As Double, Optional ByVal item As Object = Nothing) As Object
            If TypeOf lst Is IList(Of Reference) Then
                If item Is Nothing Then item = 0.0
                Dim rlst As IList(Of Reference) = DirectCast(lst, IList(Of Reference))
                While Len(rlst) < Int(length)
                    rlst.Add(New Reference(item))
                End While
                Return rlst
            ElseIf TypeOf lst Is String
                If item Is Nothing Then item = " "
                Return CStr(lst).PadRight(Int(length), item.ToString()(0))
            Else
                Return Double.NaN
            End If
        End Function

        ''' <summary>
        ''' Get the levenshtein edit distance between two strings, optionally with custom weights
        ''' </summary>
        ''' <returns></returns>
        Public Function EditDist(a As String, b As String,
                                     Optional costAdd As Double = 1,
                                     Optional costRem As Double = 1, Optional costChange As Double = 1) As Double

            costAdd = Math.Min(costAdd, costRem)

            Dim dp(a.Length + 1, b.Length + 1) As Double

            ' initialize to 0
            For i As Integer = 0 To a.Length
                dp(i, 0) = i * costAdd
            Next
            For i As Integer = 1 To b.Length
                dp(0, i) = i * costAdd
            Next

            For i As Integer = 1 To a.Length
                For j As Integer = 1 To b.Length
                    If a(i - 1) = b(j - 1) Then
                        dp(i, j) = dp(i - 1, j - 1)
                    Else
                        dp(i, j) = Math.Min(Math.Min(dp(i - 1, j) + costAdd,
                                     dp(i, j - 1) + costAdd), dp(i - 1, j - 1) + costChange)
                    End If
                Next
            Next

            Return dp(a.Length, b.Length)
        End Function

        ''' <summary>
        ''' Swap two items in a list without creating a matrix, used internally by qsort
        ''' </summary>
        Private Sub InplaceSwap(list As IList(Of Reference), a As Integer, b As Integer)
            Dim tmp As Reference = list(a)
            list(a) = list(b)
            list(b) = tmp
        End Sub

        ''' <summary>
        ''' Internal function using quicksort to sort a list in-place
        ''' </summary>
        ''' <param name="lst">List to sort</param>
        ''' <param name="l">Left limit of range to sort (inclusive)</param>
        ''' <param name="r">Right limit of range to sort (exclusive)</param>
        Private Sub QSort(lst As IList(Of Reference), l As Integer, r As Integer)
            If l + 1 >= r Then Return

            'MsgBox(String.Join(",", lst))
            Dim pivot As Integer = Int(l + (r - l) / 2) ' choose middle element as pivot
            InplaceSwap(lst, pivot, r - 1) ' move pivot to end

            pivot = r - 1 ' new location for pivot, set for convenience

            Dim mid As Integer = l ' the index we are moving things less than the pivot to
            For i As Integer = l To pivot - 1 ' do not loop onto pivot
                ' if less than pivot, swap
                If GenericComparer.CompareObjs(lst(i), lst(pivot)) < 0 Then
                    InplaceSwap(lst, i, mid)
                    mid += 1 ' this index is full, go to next
                End If
            Next

            InplaceSwap(lst, mid, pivot) ' swap pivot back to where it should belong

            ' divide and conquer
            QSort(lst, l, mid)
            QSort(lst, mid + 1, r)
        End Sub

        ''' <summary>
        ''' Sort a list using the generic comparer, returning true on success
        ''' </summary>
        ''' <param name="lst"></param>
        ''' <returns></returns>
        Public Function Sort(lst As IList(Of Reference)) As IList(Of Reference)
            QSort(lst, 0, lst.Count)
            Return lst
        End Function

        ''' <summary>
        ''' Randomly shuffle a list
        ''' </summary>
        ''' <param name="lst"></param>
        ''' <returns></returns>
        Public Function Shuffle(lst As IList(Of Reference)) As IList(Of Reference)
            Dim lst2 As New List(Of Reference)(lst)
            lst.Clear()
            Dim rnd As New Random()
            While lst2.Count > 0
                Dim i As Integer = rnd.Next(0, lst2.Count)
                lst.Add(lst2(i))
                lst2.RemoveAt(i)
            End While
            Return lst
        End Function

        ''' <summary>
        ''' Get the height of the matrix
        ''' </summary>
        ''' <param name="lst"></param>
        ''' <returns></returns>
        Public Function Height(lst As IEnumerable(Of Reference)) As Double
            Return New Matrix(lst).Height
        End Function

        ''' <summary>
        ''' Alias for matrix height
        ''' </summary>
        ''' <param name="lst"></param>
        ''' <returns></returns>
        Public Function Rows(lst As IEnumerable(Of Reference)) As Double
            Return Height(lst)
        End Function

        ''' <summary>
        ''' Get the width of the matrix
        ''' </summary>
        ''' <param name="lst"></param>
        ''' <returns></returns>
        Public Function Width(lst As IEnumerable(Of Reference)) As Double
            Return New Matrix(lst).Width
        End Function

        ''' <summary>
        ''' Alias for matrix width
        ''' </summary>
        ''' <param name="lst"></param>
        ''' <returns></returns>
        Public Function Cols(lst As IEnumerable(Of Reference)) As Double
            Return Width(lst)
        End Function

        ''' <summary>
        ''' Give the matrix a standard width and height
        ''' </summary>
        ''' <param name="lst"></param>
        ''' <returns></returns>
        Public Function Normalize(lst As IEnumerable(Of Reference)) As IEnumerable(Of Reference)
            Return CType(New Matrix(lst).GetValue(), IEnumerable(Of Reference))
        End Function

        ''' <summary>
        ''' Get a row of the matrix as a 1-column matrix/vector
        ''' </summary>
        ''' <returns></returns>
        Public Function Row(lst As IEnumerable(Of Reference), id As Double) As IEnumerable(Of Reference)
            Return DirectCast(New Matrix(lst).Row(Int(id)).GetValue(), IEnumerable(Of Reference))
        End Function

        ''' <summary>
        ''' Get a column of the matrix as a 1-column matrix/vector
        ''' </summary>
        ''' <returns></returns>
        Public Function Col(lst As IEnumerable(Of Reference), id As Double) As IEnumerable(Of Reference)
            Return DirectCast(New Matrix(lst).Col(Int(id)).GetValue(), IEnumerable(Of Reference))
        End Function

        ''' <summary>
        ''' Resize
        ''' </summary>
        ''' <param name="lst"></param>
        ''' <returns></returns>
        Public Function Resize(lst As IEnumerable(Of Reference), height As Double, Optional width As Double = Double.NaN) As IEnumerable(Of Reference)
            Dim mat As New Matrix(lst)
            If width = Double.NaN Then width = mat.Width
            mat.Resize(Int(height), Int(width))
            Return CType(mat.GetValue(), IEnumerable(Of Reference))
        End Function

        ''' <summary>
        ''' Multiply two matrices
        ''' </summary>
        ''' <param name="a"></param>
        ''' <param name="b"></param>
        ''' <returns></returns>
        Public Function Multiply(a As IEnumerable(Of Reference), b As IEnumerable(Of Reference)) As Object
            Dim ma As New Matrix(a)
            Dim mb As New Matrix(b)
            Try
                Return ma.Multiply(mb).GetValue()
            Catch ex As MathException
                Return Dot(a, b)
            End Try
        End Function

        ''' <summary>
        ''' Get the dot product of two column vectors
        ''' </summary>
        ''' <returns></returns>
        Public Function Dot(a As IEnumerable(Of Reference), b As IEnumerable(Of Reference)) As Object
            Return New Matrix(a).Dot(New Matrix(b))
        End Function

        ''' <summary>
        ''' Get the cross product of two column vectors
        ''' </summary>
        ''' <returns></returns>
        Public Function Cross(a As IEnumerable(Of Reference), b As IEnumerable(Of Reference)) As IEnumerable(Of Reference)
            Return DirectCast(New Matrix(a).Cross(New Matrix(b)).GetValue(), IEnumerable(Of Reference))
        End Function

        ''' <summary>
        ''' Multiply a matrix and a scalar
        ''' </summary>
        ''' <returns></returns>
        Public Function Scale(a As IEnumerable(Of Reference), b As Object) As IEnumerable(Of Reference)
            Return DirectCast(New Matrix(a).MultiplyScalar(b).GetValue(), IEnumerable(Of Reference))
        End Function

        ''' <summary>
        ''' Swap two rows in a matrix
        ''' </summary>
        ''' <returns></returns>
        Public Function SwapRows(mat As IList(Of Reference), a As Double, b As Double) As IList(Of Reference)
            Return DirectCast(New Matrix(mat).SwapRows(Int(a), Int(b)).GetValue(), IList(Of Reference))
        End Function

        ''' <summary>
        ''' Swap two columns in a matrix
        ''' </summary>
        ''' <returns></returns>
        Public Function SwapCols(mat As IList(Of Reference), a As Double, b As Double) As IList(Of Reference)
            Return DirectCast(New Matrix(mat).SwapCols(Int(a), Int(b)).GetValue(), IList(Of Reference))
        End Function

        ''' <summary>
        ''' Find the determinant of a matrix
        ''' </summary>
        ''' <returns></returns>
        Public Function Det(a As IEnumerable(Of Reference)) As Object
            Return New Matrix(a).Determinant()
        End Function

        ''' <summary>
        ''' Find the reduced row echelon form of a matrix
        ''' </summary>
        ''' <returns></returns>
        Public Function Rref(mat As IEnumerable(Of Reference), Optional aug As IEnumerable(Of Reference) = Nothing) As IEnumerable(Of Reference)
            If Not aug Is Nothing Then
                Return DirectCast(New Matrix(mat).Rref(New Matrix(aug)).GetValue(), IEnumerable(Of Reference))
            Else
                Return DirectCast(New Matrix(mat).Rref().GetValue(), IEnumerable(Of Reference))
            End If
        End Function

        ''' <summary>
        ''' Find the norm of a column vector (gives the square of the magnitude)
        ''' </summary>
        ''' <returns></returns>
        Public Function Norm(a As IEnumerable(Of Reference)) As Object
            Return New Matrix(a).Norm()
        End Function

        ''' <summary>
        ''' Find the transpose of a matrix
        ''' </summary>
        ''' <param name="a"></param>
        ''' <returns></returns>
        Public Function Transpose(a As IEnumerable(Of Reference)) As IEnumerable(Of Reference)
            Return DirectCast(New Matrix(a).Transpose().GetValue(), IEnumerable(Of Reference))
        End Function

        ''' <summary>
        ''' Find the inverse of a matrix
        ''' </summary>
        ''' <returns></returns>
        Public Function Inverse(a As IEnumerable(Of Reference)) As IEnumerable(Of Reference)
            Return DirectCast(New Matrix(a).Inverse().GetValue(), IEnumerable(Of Reference))
        End Function

        ''' <summary>
        ''' Get the identity matrix (filled with zeros except the primary diagonal which is filled with ones) 
        ''' with the specified number of rows and cols
        ''' </summary>
        ''' <returns></returns>
        Public Function IdentityMatrix(rows As Double, Optional cols As Double = -1) As IEnumerable(Of Reference)
            Return DirectCast(ObjectTypes.Matrix.IdentityMatrix(Int(rows), Int(cols)).GetValue(), IEnumerable(Of Reference))
        End Function

        ''' <summary>
        ''' Convert to set
        ''' </summary>
        Public Function ToSet(lst As Object) As SortedDictionary(Of Reference, Reference)
            Dim tmp As [Set]
            If TypeOf lst Is IEnumerable(Of Reference) Then
                tmp = New [Set](CType(lst, IEnumerable(Of Reference)))
            ElseIf TypeOf lst Is IDictionary(Of Reference, Reference)
                tmp = New [Set](CType(lst, IDictionary(Of Reference, Reference)))
            Else
                tmp = New [Set]({New Reference(lst)}.ToList())
            End If
            Return CType(tmp.GetValue(), SortedDictionary(Of Reference, Reference))
        End Function

        ''' <summary>
        ''' Create a new set from any object
        ''' </summary>
        Public Function [Set](Optional lst As Object = Nothing) As IDictionary(Of Reference, Reference)
            If lst Is Nothing Then Return New SortedDictionary(Of Reference, Reference)
            Return ToSet(lst)
        End Function

        ''' <summary>
        ''' Convert to hashset
        ''' </summary>
        Public Function ToHashSet(lst As Object) As Dictionary(Of Reference, Reference)
            Dim tmp As HashSet
            If TypeOf lst Is IEnumerable(Of Reference) Then
                tmp = New HashSet(CType(lst, IEnumerable(Of Reference)))
            ElseIf TypeOf lst Is IDictionary(Of Reference, Reference)
                tmp = New HashSet(CType(lst, IDictionary(Of Reference, Reference)))
            Else
                tmp = New HashSet({New Reference(lst)}.ToList())
            End If
            Return CType(tmp.GetValue(), Dictionary(Of Reference, Reference))
        End Function

        ''' <summary>
        ''' Create a new hashset from any object
        ''' </summary>
        Public Function [HashSet](Optional lst As Object = Nothing) As IDictionary(Of Reference, Reference)
            If lst Is Nothing Then Return New Dictionary(Of Reference, Reference)
            Return ToHashSet(lst)
        End Function

        ' setionary/set

        ''' <summary>
        ''' Computes the union of two sets
        ''' </summary>
        Public Function Union(set1 As IDictionary(Of Reference, Reference), set2 As IDictionary(Of Reference, Reference)) As _
                IDictionary(Of Reference, Reference)
            For Each kv As KeyValuePair(Of Reference, Reference) In set2
                set1(New Reference(kv.Key)) = kv.Value
            Next
            Return set1
        End Function

        ''' <summary>
        ''' Computes the intersection of two sets
        ''' </summary>
        Public Function Intersect(set1 As IDictionary(Of Reference, Reference), set2 As IDictionary(Of Reference, Reference)) As _
                IDictionary(Of Reference, Reference)
            Dim toRem As New List(Of Reference)
            For Each kv As KeyValuePair(Of Reference, Reference) In set1
                If Not set2.ContainsKey(kv.Key) Then toRem.Add(kv.Key)
            Next
            For Each r As Reference In toRem
                set1.Remove(r)
            Next
            Return set1
        End Function

        ''' <summary>
        ''' Computes the difference of two sets
        ''' </summary>
        Public Function Difference(set1 As IDictionary(Of Reference, Reference), set2 As IDictionary(Of Reference, Reference)) As _
                IDictionary(Of Reference, Reference)
            For Each kv As KeyValuePair(Of Reference, Reference) In set2
                If set1.ContainsKey(kv.Key) Then set1.Remove(kv.Key)
            Next
            Return set1
        End Function

        ''' <summary>
        ''' Computes the symmetric difference of two sets
        ''' </summary>
        Public Function SymmetricDifference(set1 As IDictionary(Of Reference, Reference), set2 As IDictionary(Of Reference, Reference)) As _
                IDictionary(Of Reference, Reference)
            For Each kv As KeyValuePair(Of Reference, Reference) In set2
                If set1.ContainsKey(kv.Key) Then
                    set1.Remove(kv.Key)
                Else
                    set1(kv.Key) = kv.Value
                End If
            Next
            Return set1
        End Function

        ''' <summary>
        ''' Returns true if the specified setionary (set) contains the key
        ''' </summary>
        Public Function ContainsKey([set] As IDictionary(Of Reference, Reference), val As Object) As Boolean
            Return [set].ContainsKey(New Reference(ObjectTypes.DetectType(val)))
        End Function

        ''' <summary>
        ''' Returns true if the specified setionary (set) contains the value
        ''' </summary>
        Public Function ContainsValue([set] As IDictionary(Of Reference, Reference), val As Object) As Boolean
            Dim valRef As Reference = New Reference(ObjectTypes.DetectType(val))
            For Each r As Reference In [set].Values
                If GenericComparer.CompareObjs(r, valRef) = 0 Then Return True
            Next
            Return False
        End Function

        ''' <summary>
        ''' Convert to a matrix
        ''' </summary>
        ''' <param name="lst"></param>
        ''' <returns></returns>
        Public Function ToMatrix(lst As Object) As List(Of Reference)
            Dim ret As New List(Of Reference)
            If TypeOf lst Is IDictionary Then
                For Each k As KeyValuePair(Of Reference, Reference) In DirectCast(lst, IDictionary(Of Reference, Reference))
                    If k.Value Is Nothing Then
                        ret.Add(k.Key)
                    Else
                        ret.Add(New Reference({k.Key, k.Value}))
                    End If
                Next
            ElseIf TypeOf lst Is IEnumerable(Of Reference)
                ret.AddRange(DirectCast(lst, IEnumerable(Of Reference)))
            Else
                ret.Add(New Reference(lst))
            End If
            Return ret
        End Function

        ''' <summary>
        ''' Alias for ToMatrix(lst); Creates a matrix from another collection or object
        ''' </summary>
        ''' <param name="lst"></param>
        ''' <returns></returns>
        Public Function ToList(lst As Object) As List(Of Reference)
            Return ToMatrix(lst)
        End Function

        ''' <summary>
        ''' Convert to a matrix or create a new matrix with a rows and b columns filled with 0
        ''' </summary>
        ''' <param name="lstOrRows">either a collection or the number of rows</param>
        ''' <param name="cols">number of columns</param>
        ''' <returns></returns>
        Public Function Matrix(Optional lstOrRows As Object = Nothing, Optional cols As Object = Nothing) As List(Of Reference)
            If lstOrRows Is Nothing Then Return New List(Of Reference)
            If cols Is Nothing Then
                Return ToMatrix(lstOrRows)
            Else
                Return DirectCast(New Matrix(Int(CDbl(lstOrRows)), Int(CDbl(cols))).GetValue(), List(Of Reference))
            End If
        End Function

        ''' <summary>
        ''' Convert to a linked list
        ''' </summary>
        ''' <param name="lst"></param>
        ''' <returns></returns>
        Public Function ToLinkedList(lst As Object) As LinkedList(Of Reference)
            Dim ret As New LinkedList(Of Reference)
            If TypeOf lst Is IDictionary Then
                For Each k As KeyValuePair(Of Reference, Reference) In DirectCast(lst, IDictionary(Of Reference, Reference))
                    If k.Value Is Nothing Then
                        ret.AddLast(k.Key)
                    Else
                        ret.AddLast(New Reference(ToLinkedList({k.Key, k.Value})))
                    End If
                Next
            ElseIf TypeOf lst Is IEnumerable(Of Reference)
                For Each r As Reference In DirectCast(lst, IEnumerable(Of Reference))
                    ret.AddLast(r)
                Next
            Else
                ret.AddLast(New Reference(lst))
            End If
            Return ret
        End Function

        ''' <summary>
        ''' Alias for ToLinkedList(lst); Creates a new linked list 
        ''' </summary>
        ''' <param name="lst"></param>
        ''' <returns></returns>
        Public Function LinkedList(Optional lst As Object = Nothing) As LinkedList(Of Reference)
            If lst Is Nothing Then Return New LinkedList(Of Reference)
            Return ToLinkedList(lst)
        End Function

        ''' <summary>
        ''' Initialize an array with the specified number of dimensions, with only one item at 0.
        ''' </summary>
        ''' <returns></returns>
        Public Function Array(dimensions As Double) As IEnumerable(Of Reference)
            Dim d As Integer = Int(dimensions)
            If d < 0 Then Throw New EvaluatorException("Array dimensions cannot be negative")
            If d > 25 Then Throw New EvaluatorException("Array dimensions too large: please keep under 15 dimensions")
            If d = 0 Then Return New List(Of Reference) ' empty

            Dim lst As New List(Of Reference)({New Reference(New Number(0.0))})
            For i As Integer = 2 To d
                lst = New List(Of Reference)({New Reference(New Matrix(lst))})
            Next
            Return lst
        End Function

        ''' <summary>
        ''' Convert to a tuple
        ''' </summary>
        ''' <param name="lst"></param>
        ''' <returns></returns>
        Public Function ToTuple(lst As Object) As Reference()
            Dim ret As New List(Of Reference)
            If TypeOf lst Is IDictionary Then
                Try
                    For Each k As KeyValuePair(Of Reference, Reference) In CType(lst, IDictionary(Of Reference, Reference))
                        If k.Value Is Nothing Then
                            ret.Add(k.Key)
                        Else
                            ret.Add(New Reference({k.Key, k.Value}))
                        End If
                    Next
                Catch
                End Try
            ElseIf TypeOf lst Is IEnumerable(Of Reference)
                ret.AddRange(DirectCast(lst, IEnumerable(Of Reference)))
            Else
                ret.Add(New Reference(lst))
            End If
            Return ret.ToArray()
        End Function

        ''' <summary>
        ''' Convert to a tuple; Alias for ToTuple(lst)
        ''' </summary>
        ''' <returns></returns>
        Public Function Tuple(Optional lst As Object = Nothing) As Reference()
            If lst Is Nothing Then Return {}
            Return ToTuple(lst)
        End Function

        ' complex number stuff
        ''' <summary>
        ''' Create a new complex number from real and imaginary parts
        ''' </summary>
        ''' <returns></returns>
        Public Function ToComplex(real As Double, Optional imag As Double = 0) As Numerics.Complex
            Return New Numerics.Complex(real, imag)
        End Function

        ''' <summary>
        ''' Create a new complex number from real and imaginary parts
        ''' </summary>
        ''' <returns></returns>
        Public Function Complex(Optional real As Double = 0, Optional imag As Double = 0) As Numerics.Complex
            Return ToComplex(real, imag)
        End Function

        ''' <summary>
        ''' Get the conjugate of the complex number
        ''' </summary>
        Public Function Conjugate(val As Numerics.Complex) As Numerics.Complex
            Return Numerics.Complex.Conjugate(val)
        End Function

        ''' <summary>
        ''' Get the reciprocal of the complex number
        ''' </summary>
        Public Function Reciprocal(val As Numerics.Complex) As Numerics.Complex
            Return Numerics.Complex.Reciprocal(val)
        End Function

        ''' <summary>
        ''' Get the real part of the complex number
        ''' </summary>
        Public Function Real(val As Numerics.Complex) As Double
            Return val.Real
        End Function

        ''' <summary>
        ''' Get the imaginary part of the complex number
        ''' </summary>
        Public Function Imag(val As Numerics.Complex) As Double
            Return val.Imaginary
        End Function

        ''' <summary>
        ''' Get the magnitude of a complex number or a column vector
        ''' </summary>
        Public Function Magnitude(val As Object) As Object
            If TypeOf val Is Numerics.Complex Then
                Return DirectCast(val, Numerics.Complex).Magnitude
            ElseIf TypeOf val Is IEnumerable(Of Reference)
                Return New Matrix(DirectCast(val, IEnumerable(Of Reference))).Magnitude()
            Else
                Return Double.NaN
            End If
        End Function

        ''' <summary>
        ''' Get the phase of a complex number 
        ''' </summary>
        Public Function Phase(val As Numerics.Complex) As Double
            Return val.Phase
        End Function

        ''' <summary>
        ''' Create a new complex number from polar coordinates.
        ''' </summary>
        Public Function FromPolar(magnitude As Double, phase As Double) As Numerics.Complex
            Return Numerics.Complex.FromPolarCoordinates(magnitude, phase)
        End Function

        ''' <summary>
        ''' find out If the Object Is 'truthy' (e.g. for numbers, all numbers other than 0 are considered truthy), for use with conditions
        ''' </summary>
        Public Function IsTrue(ByVal obj As Object) As Boolean
            If TypeOf obj Is Boolean Then
                Return CBool(obj) = True
            ElseIf TypeOf obj Is Integer OrElse TypeOf obj Is Int32 Then
                Return Int(CDbl(obj)) <> 0
            ElseIf TypeOf obj Is Double OrElse TypeOf obj Is Single Then
                Return (CDbl(obj) - 0) > 0.00000001
            ElseIf TypeOf obj Is BigDecimal Then
                Return (DirectCast(obj, BigDecimal) - 0) > 0.00000001
            ElseIf TypeOf obj Is String Then
                Return obj.ToString().ToLower() = "true"
            ElseIf TypeOf obj Is System.DateTime OrElse TypeOf obj Is Date Then
                Return CDate(obj) = System.DateTime.Now
            Else
                Return False
            End If
        End Function

        ' limits for functions
        ''' <summary>
        ''' If the variable specified (default x) is between l and r, then evaluates func and returns the value
        ''' Otherwise returns undefined.
        ''' </summary>
        Public Function SliceFunction(ByVal text As Object, ByVal left As Double, ByVal right As Double, Optional ByVal var As String = "x") As Object
            Try
                Dim varval As Double = CDbl(_eval.GetVariable(var))
                If varval >= left AndAlso varval <= right Then
                    Dim ans As Double = CDbl(_eval.EvalExprRaw(text.ToString()))
                    Return ans
                Else
                    Return Double.NaN
                End If
            Catch
                Return Double.NaN
            End Try
        End Function

        ''' <summary>
        ''' If dom evaluates to a 'truthy' value, then evaluates func and returns the value
        ''' Otherwise returns undefined.
        ''' </summary>
        Public Function Domain(ByVal func As Object, ByVal dom As String) As Object
            Try
                If IsTrue(_eval.EvalExprRaw(dom)) Then
                    Dim ans As Double = CDbl(_eval.EvalExprRaw(func.ToString()))
                    Return ans
                Else
                    Return Double.NaN
                End If
            Catch
                Return Double.NaN
            End Try
        End Function

        ' input / output
        ' command line
        ''' <summary>
        ''' Prints to the console, if available
        ''' </summary>
        Public Function Print(ByVal text As Object) As String
            Console.Write(text.ToString())
            Return text.ToString()
        End Function

        ''' <summary>
        ''' Prints the text to the console, if available, followed immediately by a line break
        ''' </summary>
        Public Function WriteLine(ByVal text As Object) As Boolean
            Console.WriteLine(text.ToString())
            Return True
        End Function

        ''' <summary>
        ''' Read a line from the console, if available
        ''' </summary>
        Public Function ReadLine() As String
            Return Console.ReadLine()
        End Function

        ''' <summary>
        ''' Read a character from the console, if available
        ''' </summary>
        Public Function Read() As String
            Return ChrW(Console.Read()).ToString()
        End Function

        ''' <summary>
        ''' Clear the console, if available
        ''' </summary>
        Public Function Cls() As Boolean
            Console.Clear()
            Return True
        End Function

        ' file IO
        ''' <summary>
        ''' Read text from a file
        ''' </summary>
        ''' <param name="path"></param>
        ''' <returns></returns>
        Public Function ReadFile(ByVal path As String) As String
            Try
                Return FileIO.FileSystem.ReadAllText(path)
            Catch 'ex As Exception
                Return ""
            End Try
        End Function

        ''' <summary>
        ''' Read the specified line of text from a file
        ''' </summary>
        ''' <param name="path"></param>
        ''' <param name="line"></param>
        ''' <returns></returns>
        Public Function ReadLine(ByVal path As String, ByVal line As Double) As Object
            Try
                Return IO.File.ReadAllLines(path)(Int(line - 1))
            Catch 'ex As Exception
                Return Double.NaN
            End Try
        End Function

        ''' <summary>
        ''' Overwrite a file; or, if append is true, appends to the file
        ''' </summary>
        Public Function WriteFile(ByVal path As String, ByVal content As Object, Optional ByVal append As Boolean = False) As String
            Try
                FileIO.FileSystem.WriteAllText(path, content.ToString(), append)
                Return "Write Success"
            Catch 'ex As Exception
                Return "Write Failed"
            End Try
        End Function

        ''' <summary>
        ''' Write to the specified line in the file
        ''' </summary>
        Public Function WriteFileLine(ByVal path As String, ByVal line As Integer, ByVal content As Object) As String
            Try
                Dim lines As String() = IO.File.ReadAllLines(path)
                lines(Int(line - 1)) = content.ToString()
                FileIO.FileSystem.WriteAllText(path, String.Join(vbNewLine, lines), False)
                Return "Write Success"
            Catch 'ex As Exception
                Return "Write Failed"
            End Try
        End Function

        ''' <summary>
        ''' Append to file, equal to Write(path, content, true)
        ''' </summary>
        Public Function AppendFile(ByVal path As String, ByVal content As Object) As String
            Return WriteFile(path, content, True)
        End Function

        ''' <summary>
        ''' Move a file
        ''' </summary>
        Public Function MoveFile(ByVal path As String, ByVal path2 As String) As Boolean
            File.Move(path, path2)
            Return True
        End Function

        ''' <summary>
        ''' Rename a file
        ''' </summary>
        Public Function RenameFile(ByVal path As String, ByVal newname As String) As Boolean
            FileSystem.Rename(path, newname)
            Return True
        End Function

        ''' <summary>
        ''' Delete a file
        ''' </summary>
        Public Function DeleteFile(ByVal path As String) As Boolean
            File.Delete(path)
            Return True
        End Function

        ''' <summary>
        ''' Move a directory
        ''' </summary>
        Public Function MoveDirectory(ByVal path As String, ByVal path2 As String) As Boolean
            Directory.Move(path, path2)
            Return True
        End Function

        ''' <summary>
        ''' Rename a directory
        ''' </summary>
        Public Function RenameDirectory(ByVal path As String, ByVal newname As String) As Boolean
            FileIO.FileSystem.RenameDirectory(path, newname)
            Return True
        End Function

        ''' <summary>
        ''' Delete a directory
        ''' </summary>
        Public Function DeleteDir(ByVal path As String) As Boolean
            Directory.Delete(path, True)
            Return True
        End Function

        ''' <summary>
        ''' IEnumerable the files and directories at the specified file system path
        ''' </summary>
        Public Function IEnumerableDir(ByVal path As String) As IEnumerable(Of Reference)
            Return DirectCast(New Matrix(Directory.EnumerateFileSystemEntries(path).ToList()).GetValue(),
                IEnumerable(Of Reference))
        End Function

        ''' <summary>
        ''' IEnumerable the files at the specified file system path
        ''' </summary>
        Public Function IEnumerableFiles(ByVal path As String) As IEnumerable(Of Reference)
            Return DirectCast(New Matrix(Directory.EnumerateFiles(path).ToList()).GetValue(),
                IEnumerable(Of Reference))
        End Function

        ''' <summary>
        ''' IEnumerable the directories at the specified file system path
        ''' </summary>
        Public Function IEnumerableDirs(ByVal path As String) As IEnumerable(Of Reference)
            Return DirectCast(New Matrix(Directory.EnumerateDirectories(path).ToList()).GetValue(),
                IEnumerable(Of Reference))
        End Function

        ''' <summary>
        ''' Checks if the specified file exists in the filesystem
        ''' </summary>
        Public Function FileExists(ByVal path As String) As Boolean
            Return File.Exists(path)
        End Function

        ''' <summary>
        ''' Checks if the specified file exists in the filesystem
        ''' </summary>
        Public Function DirExists(ByVal path As String) As Boolean
            Return Directory.Exists(path)
        End Function

        ''' <summary>
        ''' Given the full path, gets the file name 
        ''' </summary>
        Public Function GetFileName(ByVal path As String) As String
            Return IO.Path.GetFileName(path)
        End Function

        ''' <summary>
        ''' Given the full path, gets the directory name 
        ''' </summary>
        Public Function GetDirName(ByVal path As String) As String
            Return IO.Path.GetDirectoryName(path)
        End Function

        ''' <summary>
        ''' Combines two paths into one
        ''' </summary>
        Public Function JoinPath(ByVal path1 As String, ByVal path2 As String) As String
            Return IO.Path.Combine(path1, path2)
        End Function

        ''' <summary>
        ''' Get the startup directory
        ''' </summary>
        Public Function BaseDir() As String
            Return Environment.CurrentDirectory
        End Function

        ''' <summary>
        ''' Get the desktop path
        ''' </summary>
        Public Function DesktopDir() As String
            Return Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
        End Function

        ''' <summary>
        ''' Get the desktop path
        ''' </summary>
        Public Function PublicDesktopDir() As String
            Return Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory)
        End Function

        ''' <summary>
        ''' Get the program files directory
        ''' </summary>
        Public Function ProgramFilesPath() As String
            Return Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)
        End Function

        ''' <summary>
        ''' Get the program files (x86) directory
        ''' </summary>
        Public Function ProgramFilesX86Path() As String
            Return Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86)
        End Function

        ''' <summary>
        ''' Get the temp directory
        ''' </summary>
        Public Function TempDir() As String
            Return Path.GetTempPath()
        End Function

        ''' <summary>
        ''' Get the appdata directory
        ''' </summary>
        Public Function AppDataDir() As String
            Return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
        End Function

        ''' <summary>
        ''' Get the appdata directory for all users
        ''' </summary>
        Public Function PublicAppDataDir() As String
            Return Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)
        End Function

        ''' <summary>
        ''' Get the path of the cantus executable
        ''' </summary>
        Public Function CantusPath() As String
            Return Application.ExecutablePath
        End Function

        ''' <summary>
        ''' Get the system's path separator (/ for linux, \ for windows)
        ''' </summary>
        Public Function GetPathSeparator() As String
            Return IO.Path.DirectorySeparatorChar
        End Function

        ' GUI Only
        ''' <summary>
        ''' Show a message on the screen (if gui available)
        ''' </summary>
        Public Sub Alert(ByVal text As Object, Optional ByVal title As Object = "Alert")
            MsgBox(text.ToString(), MsgBoxStyle.SystemModal, title.ToString())
        End Sub

        ''' <summary>
        ''' Show a confirmation box (Ok/Cancel) on the screen (if gui available)
        ''' </summary>
        Public Function Confirm(ByVal text As Object, Optional ByVal title As Object = "Message") As Boolean
            Return MsgBox(text.ToString(), MsgBoxStyle.Information Or MsgBoxStyle.OkCancel, title.ToString()) =
                MsgBoxResult.Ok
        End Function

        ''' <summary>
        ''' Show a confirmation box (Yes/No) on the screen (if gui available)
        ''' </summary>
        Public Function YesNo(ByVal text As Object, Optional ByVal title As Object = "Message") As Boolean
            Return MsgBox(text.ToString(), MsgBoxStyle.Information Or MsgBoxStyle.YesNo, title.ToString()) =
                MsgBoxResult.Yes
        End Function

        ''' <summary>
        ''' Show a input box on the screen (if gui available)
        ''' </summary>
        Public Function Input(ByVal text As Object, Optional ByVal title As Object = "Enter a Value", Optional ByVal deftext As Object = "") As String
            Return InputBox(text.ToString(), title.ToString(), deftext.ToString(), 50, 50)
        End Function

        ' threading 

        ''' <summary>
        ''' Start an asynchroneous task.
        ''' </summary>
        ''' <param name="text"></param>
        ''' <param name="after"></param>
        ''' <returns></returns>
        Public Function Async(ByVal text As String, Optional ByVal after As String = "",
                              Optional runAfter As String = "", Optional var As String = "result") As String
            Try
                Dim tmp As Evaluator = _eval.DeepCopy()
                AddHandler tmp.EvalComplete, Sub(sender As Object, result As Object)
                                                 AsyncCallBack(tmp, result, var, runAfter)
                                             End Sub
                tmp.EvalRawAsync(text)
            Catch
            End Try
            Return ""
        End Function

        Private Sub AsyncCallBack(tmp As Evaluator, result As Object, var As String, runAfter As String)
            Try
                _eval.SetVariable(var, result)
                If runAfter = "" Then
                    ' if no callback is defined then return the variable
                    _eval.EvalAsync(var)
                Else
                    _eval.EvalAsync(runAfter)
                End If

                tmp.Dispose()
            Catch
            End Try
        End Sub

        ''' <summary>
        ''' Cause the current thread to wait the specified number of seconds
        ''' </summary>
        ''' <param name="seconds"></param>
        ''' <returns></returns>
        Public Function Wait(seconds As Double) As Boolean
            Thread.Sleep(Int(seconds * 1000))
            Return True
        End Function

        ''' <summary>
        ''' Start a process from the specified filesystem path, wait for completion, and get the return value
        ''' </summary>
        Public Function StartWait(ByVal path As String, Optional ByVal args As String = "") As String
            Dim p As New Process()
            Dim si As New ProcessStartInfo(path, args)
            si.UseShellExecute = False
            si.RedirectStandardOutput = True
            p.StartInfo = si
            p.Start()

            Dim ret As String
            Using oStreamReader As System.IO.StreamReader = p.StandardOutput
                ret = oStreamReader.ReadToEnd()
            End Using
            Return ret
        End Function

        ''' <summary>
        ''' Start a process from the specified filesystem path without waiting for completion
        ''' </summary>
        Public Function Start(ByVal path As String, Optional ByVal args As String = "") As Double
            Process.Start(path, args)
            Return Double.NaN
        End Function

        ''' <summary>
        ''' Kill all subthreads (except this one) spawned from the evaluator
        ''' </summary>
        Public Sub StopAll()
            _eval.StopAll(Thread.CurrentThread.ManagedThreadId)
        End Sub

        ''' <summary>
        ''' Execute a script at the specified path, saves the result into var and executes runAfter
        ''' </summary>
        Public Function Run(ByVal path As String, Optional runAfter As String = "", Optional var As String = "result") As String
            Dim tmp As Evaluator = _eval.DeepCopy()
            AddHandler tmp.EvalComplete, Sub(sender As Object, result As Object)
                                             RunCallBack(tmp, result, var, runAfter)
                                         End Sub
            tmp.EvalRawAsync(File.ReadAllText(path))
            Return ""
        End Function

        Private Sub RunCallBack(tmp As Evaluator, result As Object, var As String, runAfter As String)
            Try
                _eval.SetVariable(var, result)
                If runAfter = "" Then
                    ' if no callback is defined then return the variable
                    _eval.EvalRawAsync(var)
                Else
                    _eval.EvalAsync(runAfter)
                End If
                tmp.Dispose()
            Catch
            End Try
        End Sub

        ''' <summary>
        ''' Execute the script at the specified path, wait, and return the result
        ''' </summary>
        Public Function RunWait(ByVal path As String) As Object
            Return _eval.EvalRaw(File.ReadAllText(path), noSaveAns:=True)
        End Function

        ''' <summary>
        ''' Download from the specified url to the specified path and wait for completion
        ''' </summary>
        Public Function DownloadWait(ByVal url As String, ByVal path As String) As Boolean
            Using wc As New WebClient
                wc.DownloadFile(New Uri(url), path)
            End Using
            Return IO.File.Exists(path)
        End Function

        ''' <summary>
        ''' Download from the specified url to the specified path without waiting for completion
        ''' </summary>
        Public Function Download(ByVal url As String, ByVal path As String) As Boolean
            Using wc As New WebClient
                wc.DownloadFileAsync(New Uri(url), path)
            End Using
            Return True
        End Function

        Public Function UploadWait(ByVal url As String, ByVal path As String) As Boolean
            Using wc As New WebClient
                wc.UploadFile(New Uri(url), path)
            End Using
            Return True
        End Function

        Public Function Upload(ByVal url As String, ByVal path As String) As Boolean
            Using wc As New WebClient
                wc.UploadFileAsync(New Uri(url), path)
            End Using
            Return True
        End Function

        Public Function DownloadText(ByVal url As String) As String
            Using wc As New WebClient
                Return wc.DownloadString(New Uri(url))
            End Using
        End Function

        Public Function DownloadSource(ByVal url As String) As String
            Return DownloadText(url)
        End Function

        Public Function WebGet(ByVal url As String, Optional ByVal params As IDictionary(Of Reference, Reference) = Nothing) As String
            If params IsNot Nothing Then
                url &= "?"
                For Each k As KeyValuePair(Of Reference, Reference) In params
                    If url <> "?" Then url &= "&"
                    url &= k.Key.ToString() & "=" & k.Value.ToString()
                Next
            End If
            Return DownloadText(url)
        End Function

        Public Function WebPost(ByVal url As String, Optional ByVal params As IDictionary(Of Reference, Reference) = Nothing) As String
            Using wc As New WebClient()
                If params Is Nothing Then params = New SortedDictionary(Of Reference, Reference)(New GenericComparer())
                Dim nvc As New NameValueCollection()
                For Each k As KeyValuePair(Of Reference, Reference) In params
                    nvc.Add(k.Key.ToString(), k.Value.ToString())
                Next
                Dim response As Byte() = wc.UploadValues(url, nvc)
                Return System.Text.Encoding.UTF8.GetString(response)
            End Using
        End Function

        Public Function UserGroup() As String
            If My.User.Name.Contains("\") Then
                Return My.User.Name.Remove(My.User.Name.IndexOf("\"))
            Else
                Return ""
            End If
        End Function

        Public Function Username() As String
            If My.User.Name.Contains("\") Then
                Return My.User.Name.Substring(My.User.Name.IndexOf("\") + 1)
            Else
                Return My.User.Name
            End If
        End Function

        Public Function Ver() As String
            Return My.Application.Info.Version.ToString()
        End Function

        Public Function OsName() As String
            Return My.Computer.Info.OSFullName()
        End Function

        Public Function OsVer() As String
            Return My.Computer.Info.OSVersion()
        End Function

        ' removed functions, kept to prevent upgrade errors. Will be deleted eventually
        Public Function OMode(Optional val As String = "") As String
            _Output(val)
            Return "Removed in version 2.1. Please use _Output() instead"
        End Function
        Public Function AngleRep(Optional val As String = "") As String
            _AngleRepr(val)
            Return "Removed in version 2.1. Please use _AngleRepr() instead"
        End Function
        Public Function SpacesPerTab(Optional val As Double = Double.NaN) As String
            _SpacesPerTab(val)
            Return "Deprecated in version 2.1. Please use _SpacesPerTab() instead"
        End Function

    End Class
End Namespace