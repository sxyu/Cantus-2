Imports System.Globalization
Imports System.Numerics
Imports System.Text
Imports Cantus.Evaluator.Exceptions
Namespace Evaluator.CommonTypes
    ''' <summary>
    ''' Arbitrary precision decimal.
    ''' All operations are exact, except for division. Division never determines more digits than the given precision.
    ''' Based on http://stackoverflow.com/a/4524254
    ''' Author: Jan Christoph Bernack (contact: jc.bernack at googlemail.com)
    ''' (Slightly modified for the evaluator)
    ''' Converted to VB using Telerik
    ''' </summary>
    Public Structure BigDecimal
        Implements IComparable
        Implements IComparable(Of BigDecimal)
        ''' <summary>
        ''' Specifies whether the significant digits should be truncated to the given precision after each operation.
        ''' </summary>
        Public Shared AlwaysTruncate As Boolean = False

        ''' <summary>
        ''' Sets the maximum precision of division operations.
        ''' If AlwaysTruncate is set to true all operations are affected.
        ''' </summary>
        Public Const PRECISION As Integer = 50

        ''' <summary>
        ''' Sets the maximum order of magnitude at which we display the full value of the BigDecimal
        ''' </summary>
        Public Const MAX_FULL_DISP As Double = 10000000000.0

        ''' <summary>
        ''' Sets the minimum order of magnitude at which we display the full value of the BigDecimal
        ''' </summary>
        Public Const MIN_FULL_DISP As Double = 0.000000001

        ''' <summary>
        ''' A BigDecimal representing the undefined (NaN) value
        ''' </summary>
        ''' <returns></returns>
        Public Shared ReadOnly Property Undefined As BigDecimal = New BigDecimal(0, 0, undefined:=True)

        ''' <summary>
        ''' If true, this bigdecimal represents an undefined value
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property IsUndefined As Boolean

        ''' <summary>
        ''' The number of sig figs to preserve after operations. Integer.MaxValue if not to be used
        ''' </summary>
        ''' <returns></returns>
        Public Property SigFigs As Integer

        ''' <summary>
        ''' Decimal separator
        ''' </summary>
        Private Shared ReadOnly Property DecimalSep As String =
            Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator

        ''' <summary>
        ''' Get the index of the lowest sig fig in the number. 
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property LeastSigFig As Integer
            Get
                If IsUndefined Then Return 0
                If SigFigs = Integer.MaxValue Then Return Integer.MinValue
                Return HighestDigit() - SigFigs + 1
            End Get
        End Property

        Public Property Mantissa() As BigInteger
            Get
                If IsUndefined Then Return 0
                Return m_Mantissa
            End Get
            Set
                m_Mantissa = Value
            End Set
        End Property

        Private m_Mantissa As BigInteger
        Public Property Exponent() As Integer
            Get
                Return m_Exponent
            End Get
            Set
                m_Exponent = Value
            End Set
        End Property

        Private m_Exponent As Integer

        Public Sub New(mantissa As BigInteger, exponent As Integer,
                       Optional undefined As Boolean = False, Optional sigFigs As Integer = Integer.MaxValue)
            Me.New()
            Me.Mantissa = mantissa
            Me.Exponent = exponent
            Me.IsUndefined = undefined
            Me.SigFigs = sigFigs
            Normalize()
            If AlwaysTruncate Then
                Truncate()
            End If
        End Sub

        Public Sub New(value As Double,
                       Optional sigFigs As Integer = Integer.MaxValue)
            Me.New()
            If Double.IsNaN(value) OrElse Double.IsInfinity(value) Then
                Me.IsUndefined = True
                Return
            End If
            Dim mantissa As BigInteger = CType(value, BigInteger)
            Dim exponent As Integer = 0
            Dim scaleFactor As Double = 1
            While Math.Abs(value * scaleFactor - CDbl(mantissa)) > 0
                exponent -= 1
                scaleFactor *= 10
                mantissa = CType(value * scaleFactor, BigInteger)
            End While
            Me.Mantissa = mantissa
            Me.Exponent = exponent
            Me.SigFigs = sigFigs
        End Sub

        ''' <summary>
        ''' Removes trailing zeros on the mantissa
        ''' </summary>
        Public Sub Normalize()
            If Mantissa.IsZero Then
                Exponent = 0
            Else
                Dim remainder As BigInteger = 0
                While remainder = 0
                    Dim shortened As BigInteger = BigInteger.DivRem(Mantissa, 10, remainder)
                    If remainder = 0 Then
                        Mantissa = shortened
                        Exponent += 1
                    End If
                End While
            End If
        End Sub

        ''' <summary>
        ''' Truncate the number to the given precision by removing the least significant digits.
        ''' </summary>
        ''' <returns>The truncated number</returns>
        Public Function Truncate(precision As Integer) As BigDecimal
            If IsUndefined Then Return Me
            ' copy this instance (remember its a struct)
            Dim shortened As BigDecimal = Me
            ' save some time because the number of digits is not needed to remove trailing zeros
            shortened.Normalize()

            ' remove the least significant digits, as long as the number of digits is higher than the given Precision
            Dim noD As Integer = NumberOfDigits(shortened.Mantissa)
            Dim expo As Integer = 0
            While noD > precision
                If noD - 1 = precision Then
                    Dim mod1 As Integer = CInt(BigInteger.Abs(shortened.Mantissa) Mod 10)
                    If mod1 > 5 Then
                        shortened.Mantissa += shortened.Mantissa.Sign * 10
                    ElseIf mod1 = 5 Then ' decide if to round up or down
                        If BigInteger.Abs(Me.Mantissa) Mod BigInteger.Pow(10, expo) > 0 OrElse
                            CInt(shortened.Mantissa / 10 Mod 2) = 1 Then
                            shortened.Mantissa += shortened.Mantissa.Sign * 10
                        End If
                    End If
                End If
                shortened.Mantissa /= 10
                expo += 1
                noD = NumberOfDigits(shortened.Mantissa)

            End While
            shortened.Exponent += expo

            ' normalize again to make sure there are no trailing zeros left
            shortened.Normalize()
            Return shortened
        End Function

        Public Function Truncate() As BigDecimal
            If IsUndefined Then Return Me
            Return Truncate(PRECISION)
        End Function

        Private Shared Function NumberOfDigits(value As BigInteger) As Integer
            If value = 0 Then Return 0 ' deal with zero (prevent Log(0))
            ' do not count the sign
            ' faster version
            Return CInt(Math.Ceiling(BigInteger.Log10(value * value.Sign)))
        End Function

        ''' <summary>
        ''' Get the number of digits in this BigDecimal
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Digits As Integer
            Get
                Return NumberOfDigits(Me.Mantissa)
            End Get
        End Property

#Region "Conversions"

        Public Shared Widening Operator CType(value As Integer) As BigDecimal
            Return New BigDecimal(value, exponent:=0)
        End Operator

        Public Shared Widening Operator CType(value As Double) As BigDecimal
            Return New BigDecimal(value)
        End Operator

        Public Shared Widening Operator CType(value As Decimal) As BigDecimal
            Dim mantissa As BigInteger = CType(value, BigInteger)
            Dim exponent As Integer = 0
            Dim scaleFactor As Decimal = 1
            While CDec(mantissa) <> value * scaleFactor
                exponent -= 1
                scaleFactor *= 10
                mantissa = CType(value * scaleFactor, BigInteger)
            End While
            Return New BigDecimal(mantissa, exponent)
        End Operator

        Public Shared Narrowing Operator CType(value As BigDecimal) As Double
            If value.IsUndefined Then Return Double.NaN
            Return CDbl(value.Mantissa) * Math.Pow(10, value.Exponent)
        End Operator

        Public Shared Narrowing Operator CType(value As BigDecimal) As Single
            If value.IsUndefined Then Return Single.NaN
            Return Convert.ToSingle(CDbl(value))
        End Operator

        Public Shared Narrowing Operator CType(value As BigDecimal) As Decimal
            If value.IsUndefined Then Return 0
            Return CDec(value.Mantissa) * CDec(Math.Pow(10, value.Exponent))
        End Operator

        Public Shared Narrowing Operator CType(value As BigDecimal) As Integer
            If value.IsUndefined Then Return 0
            Return CInt(value.Mantissa * BigInteger.Pow(10, value.Exponent))
        End Operator

        Public Shared Narrowing Operator CType(value As BigDecimal) As UInteger
            If value.IsUndefined Then Return 0
            Return CUInt(value.Mantissa * BigInteger.Pow(10, value.Exponent))
        End Operator

        Public Shared Narrowing Operator CType(value As BigDecimal) As Long
            If value.IsUndefined Then Return 0
            Return CLng(CDbl(value.Mantissa) * Math.Pow(10, value.Exponent))
        End Operator
#End Region

#Region "Operators"

        Public Shared Operator +(value As BigDecimal) As BigDecimal
            Return value
        End Operator

        Public Shared Operator -(value As BigDecimal) As BigDecimal
            value.Mantissa *= -1
            Return value
        End Operator

        Public Shared Operator +(left As BigDecimal, right As BigDecimal) As BigDecimal
            Return Add(left, right)
        End Operator

        Public Shared Operator -(left As BigDecimal, right As BigDecimal) As BigDecimal
            Return Add(left, -right)
        End Operator

        Private Shared Function Add(left As BigDecimal, right As BigDecimal) As BigDecimal
            If left.IsUndefined Then Return left
            If right.IsUndefined Then Return right
            left = left.Truncate(left.SigFigs)
            right = right.Truncate(left.SigFigs)

            Dim digit As Integer = Math.Max(left.LeastSigFig, right.LeastSigFig)

            Dim bn As BigDecimal = If(left.Exponent > right.Exponent,
                New BigDecimal(AlignExponent(left, right) + right.Mantissa, right.Exponent),
                New BigDecimal(AlignExponent(right, left) + left.Mantissa, left.Exponent))


            If digit = Integer.MinValue Then
                bn.SigFigs = Integer.MaxValue
                Return bn
            Else
                bn.SigFigs = bn.HighestDigit() - digit + 1
                Return bn.Truncate(bn.HighestDigit() - digit + 1)
            End If
        End Function

        Public Shared Operator *(left As BigDecimal, right As BigDecimal) As BigDecimal
            If left.IsUndefined Then Return left
            If right.IsUndefined Then Return right
            Dim prod As New BigDecimal(left.Mantissa * right.Mantissa, left.Exponent + right.Exponent, False,
                                       Math.Min(left.SigFigs, right.SigFigs))
            Return prod
        End Operator

        Public Shared Operator /(dividend As BigDecimal, divisor As BigDecimal) As BigDecimal
            If dividend.IsUndefined Then Return dividend
            If divisor.IsUndefined Then Return divisor
            Try
                Dim exponentChange As Integer = PRECISION - (NumberOfDigits(dividend.Mantissa) - NumberOfDigits(divisor.Mantissa))
                If exponentChange < 0 Then
                    exponentChange = 0
                End If
                dividend.Mantissa *= BigInteger.Pow(10, exponentChange)
                Dim quotient As New BigDecimal(dividend.Mantissa / divisor.Mantissa,
                                                   dividend.Exponent - divisor.Exponent - exponentChange,
                        False, Math.Min(dividend.SigFigs, divisor.SigFigs))
                Return quotient
            Catch ex As ArithmeticException
                Throw New MathException("Division by Zero")
            End Try
        End Operator

        Public Shared Operator =(left As BigDecimal, right As BigDecimal) As Boolean
            Return left.Exponent = right.Exponent AndAlso left.Mantissa = right.Mantissa
        End Operator

        Public Shared Operator <>(left As BigDecimal, right As BigDecimal) As Boolean
            Return left.Exponent <> right.Exponent OrElse left.Mantissa <> right.Mantissa
        End Operator

        Public Shared Operator <(left As BigDecimal, right As BigDecimal) As Boolean
            Return If(left.Exponent > right.Exponent, AlignExponent(left, right) < right.Mantissa, left.Mantissa < AlignExponent(right, left))
        End Operator

        Public Shared Operator >(left As BigDecimal, right As BigDecimal) As Boolean
            Return If(left.Exponent > right.Exponent, AlignExponent(left, right) > right.Mantissa, left.Mantissa > AlignExponent(right, left))
        End Operator

        Public Shared Operator <=(left As BigDecimal, right As BigDecimal) As Boolean
            Return If(left.Exponent > right.Exponent, AlignExponent(left, right) <= right.Mantissa, left.Mantissa <= AlignExponent(right, left))
        End Operator

        Public Shared Operator >=(left As BigDecimal, right As BigDecimal) As Boolean
            Return If(left.Exponent > right.Exponent, AlignExponent(left, right) >= right.Mantissa, left.Mantissa >= AlignExponent(right, left))
        End Operator

        ''' <summary>
        ''' Returns the mantissa of value, aligned to the exponent of reference.
        ''' Assumes the exponent of value is larger than of reference.
        ''' </summary>
        Private Shared Function AlignExponent(value As BigDecimal, reference As BigDecimal) As BigInteger
            Return value.Mantissa * BigInteger.Pow(10, value.Exponent - reference.Exponent)
        End Function

        ''' <summary>
        ''' Get the base 10 logarithm of one on the highest digit of a bigdecimal
        ''' </summary>
        Public Function HighestDigit() As Integer
            If Mantissa = 0 Then Return 0
            Return Exponent + CInt(Math.Floor(BigInteger.Log10(BigInteger.Abs(Mantissa))))
        End Function
#End Region

#Region "Additional mathematical functions"

        Public Shared Function Exp(exponent As Double) As BigDecimal
            Dim tmp As BigDecimal = CType(1, BigDecimal)
            While Math.Abs(exponent) > 100
                Dim diff As Integer = If(exponent > 0, 100, -100)
                tmp *= Math.Exp(diff)
                exponent -= diff
            End While
            Return tmp * Math.Exp(exponent)
        End Function

        ''' <summary>
        ''' Calculate the basis raised to the exponent. Note that this does not
        ''' actually allow bases/exponents above the double precision range.
        ''' Significant figures are supported.
        ''' </summary>
        Public Shared Function Pow(basis As BigDecimal, exponent As BigDecimal) As BigDecimal
            Dim tmp As BigDecimal = CType(1, BigDecimal)

            Dim expo As Double = CDbl(exponent)
            Dim base As Double = CDbl(basis)

            While Math.Abs(expo) > 100
                Dim diff As Integer = If(exponent > 0, 100, -100)
                tmp *= Math.Pow(base, diff)
                exponent -= diff
            End While

            tmp *= Math.Pow(base, expo)
            If basis.SigFigs = Integer.MaxValue AndAlso exponent.SigFigs < Integer.MaxValue Then
                tmp.SigFigs = exponent.SigFigs - exponent.HighestDigit - 1
            Else
                tmp.SigFigs = basis.SigFigs
            End If

            Return tmp
        End Function

#End Region

        ''' <summary>
        ''' Returns string containing the full decimal representation of this number
        ''' </summary>
        ''' <returns></returns>
        Public Function FullDecimalRepr() As String
            Me.Normalize()
            ' special case: for 0 nothing needs to be inserted
            If Me.Mantissa = 0 Then
                Dim zeroStr As String = "0"
                If Me.SigFigs > 1 AndAlso Me.SigFigs < Integer.MaxValue Then
                    zeroStr &= DecimalSep
                    zeroStr &= "".PadRight(Me.SigFigs - 1, "0"c)
                End If
                Return zeroStr
            End If

            Dim neg As Boolean = False
            Dim str As StringBuilder
            Dim trunc As BigDecimal = Me.Truncate(SigFigs)

            If trunc.Mantissa < 0 Then
                str = New StringBuilder((-trunc.Mantissa).ToString())
                neg = True
            Else
                str = New StringBuilder(trunc.Mantissa.ToString())
            End If

            Dim curlen As Integer = 0
            If trunc.Exponent < 0 AndAlso str.Length + trunc.Exponent <= 0 Then ' we need to add 0's to left
                Dim left As String = "".PadLeft(-trunc.Exponent - str.Length, "0"c) ' generate string of zeros
                str.Insert(0, left)
                str.Insert(0, "0" & CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)
                curlen = str.Length - 2
            ElseIf trunc.Exponent < 0 Then ' just insert point
                str.Insert(str.Length + trunc.Exponent, CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)
                curlen = str.Length - 1
            Else ' we need to append 0's to right
                str.Append("".PadLeft(trunc.Exponent, "0"c)) ' generate string of zeros
                curlen = str.Length
                If curlen < SigFigs AndAlso SigFigs < Integer.MaxValue Then str.Append(DecimalSep)
            End If

            If SigFigs < Integer.MaxValue Then
                str.Append("".PadLeft(Math.Max(SigFigs - curlen, 0), "0"c))
                If New InternalFunctions(Nothing).CountSigFigs(str.ToString()) <> SigFigs Then
                    ' we tried, this is unrepresentable. use scientific notation.
                    Return ToScientific()
                End If
            End If

            If neg Then str.Insert(0, "-")

            Return str.ToString()
        End Function

        ''' <summary>
        ''' Convert the BigDecimal to scientific notation
        ''' </summary>
        ''' <returns></returns>
        Public Function ToScientific() As String
            If Me.IsUndefined Then Return "Undefined"

            Me.Normalize()
            If Me.Mantissa = 0 Then
                Dim zeroStr As String = "0"
                If Me.SigFigs > 1 AndAlso Me.SigFigs < Integer.MaxValue Then
                    zeroStr &= DecimalSep
                    zeroStr &= "".PadRight(Me.SigFigs - 1, "0"c)
                End If
                Return zeroStr
            End If

            Dim tmp As BigDecimal
            If Me.SigFigs = Integer.MaxValue Then
                tmp = Me.Truncate()
            Else
                tmp = Me.Truncate(SigFigs)
            End If

            Dim neg As Boolean = False
            Dim val As StringBuilder
            If tmp.Mantissa < 0 Then
                val = New StringBuilder((-tmp.Mantissa).ToString())
                tmp.Mantissa = -tmp.Mantissa
                neg = True
            Else
                val = New StringBuilder(tmp.Mantissa.ToString())
            End If

            If val.Length > 1 Then val.Insert(1, CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)

            Dim expo As Integer = tmp.Exponent + CInt(Math.Floor(BigInteger.Log10(tmp.Mantissa)))

            Dim valRepr As String = val.ToString()
            If SigFigs < Integer.MaxValue Then
                Dim zeros As String = "".PadLeft(Math.Max(CInt(SigFigs - New InternalFunctions(Nothing).CountSigFigs(valRepr)), 0), "0"c)
                If zeros.Length > 0 AndAlso Not valRepr.Contains(DecimalSep) Then val.Append(DecimalSep)
                val.Append(zeros)
            End If

            Return String.Concat(If(neg, "-", ""), val.ToString(), " E ", expo)
        End Function

        Public Function IsOutsideDispRange() As Boolean
            Return Me > New BigDecimal(MAX_FULL_DISP) OrElse Me < New BigDecimal(-MAX_FULL_DISP) OrElse
                Math.Abs(CDbl(Me)) < MIN_FULL_DISP
        End Function

        Public Overrides Function ToString() As String
            If Me.IsUndefined Then Return "Undefined"
            If Me.IsOutsideDispRange() Then
                Return Me.ToScientific()
            Else
                Return Me.FullDecimalRepr()
            End If
        End Function

        Public Overloads Function Equals(other As BigDecimal) As Boolean
            Return other.Mantissa.Equals(Mantissa) AndAlso other.Exponent = Exponent
        End Function

        Public Overrides Function Equals(obj As Object) As Boolean
            If ReferenceEquals(Nothing, obj) Then
                Return False
            End If
            Return TypeOf obj Is BigDecimal AndAlso Equals(CType(obj, BigDecimal))
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return (Mantissa.GetHashCode() * 397) Xor Exponent
        End Function

        Public Function CompareTo(obj As Object) As Integer Implements IComparable.CompareTo
            If ReferenceEquals(obj, Nothing) OrElse Not (TypeOf obj Is BigDecimal) Then
                Throw New ArgumentException()
            End If
            Return CompareTo(CType(obj, BigDecimal))
        End Function

        Public Function CompareTo(other As BigDecimal) As Integer Implements IComparable(Of BigDecimal).CompareTo
            Return If(Me < other, -1, (If(Me > other, 1, 0)))
        End Function
    End Structure
End Namespace
