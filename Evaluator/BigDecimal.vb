Imports System.Globalization
Imports System.Numerics
Imports System.Text
Imports Cantus.Calculator.Evaluator.Exceptions
Namespace Calculator.Evaluator.CommonTypes
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
        Public Shared ReadOnly Property Undefined As BigDecimal = New BigDecimal(0, 0, True)

        ''' <summary>
        ''' If true, this bigdecimal represents an undefined value
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property IsUndefined As Boolean

        Public Property Mantissa() As BigInteger
            Get
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

        Public Sub New(mantissa__1 As BigInteger, exponent__2 As Integer, Optional undefined__3 As Boolean = False)
            Me.New()
            Mantissa = mantissa__1
            Exponent = exponent__2
            IsUndefined = undefined__3
            Normalize()
            If AlwaysTruncate Then
                Truncate()
            End If
        End Sub

        Public Sub New(value As Double)
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
            While NumberOfDigits(shortened.Mantissa) > precision
                shortened.Mantissa /= 10
                shortened.Exponent += 1
            End While
            ' normalize again to make sure there are no trailing zeros left
            shortened.Normalize()
            Return shortened
        End Function

        Public Function Truncate() As BigDecimal
            If IsUndefined Then Return Me
            Return Truncate(PRECISION)
        End Function

        Private Shared Function NumberOfDigits(value As BigInteger) As Integer
            ' do not count the sign
            'return (value * value.Sign).ToString().Length;
            ' faster version
            Return CInt(Math.Ceiling(BigInteger.Log10(value * value.Sign)))
        End Function

#Region "Conversions"

        Public Shared Widening Operator CType(value As Integer) As BigDecimal
            Return New BigDecimal(value, 0)
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
            Return If(left.Exponent > right.Exponent, New BigDecimal(AlignExponent(left, right) + right.Mantissa, right.Exponent), New BigDecimal(AlignExponent(right, left) + left.Mantissa, left.Exponent))
        End Function

        Public Shared Operator *(left As BigDecimal, right As BigDecimal) As BigDecimal
            If left.IsUndefined Then Return left
            If right.IsUndefined Then Return right
            Return New BigDecimal(left.Mantissa * right.Mantissa, left.Exponent + right.Exponent)
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
                Return New BigDecimal(dividend.Mantissa / divisor.Mantissa, dividend.Exponent - divisor.Exponent - exponentChange)
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

        Public Shared Function Pow(basis As Double, exponent As Double) As BigDecimal
            Dim tmp As BigDecimal = CType(1, BigDecimal)
            While Math.Abs(exponent) > 100
                Dim diff As Integer = If(exponent > 0, 100, -100)
                tmp *= Math.Pow(basis, diff)
                exponent -= diff
            End While
            Return tmp * Math.Pow(basis, exponent)
        End Function

#End Region

        ''' <summary>
        ''' Returns string containing the full decimal representation of this number
        ''' </summary>
        ''' <returns></returns>
        Public Function ToFullNumber() As String
            Me.Normalize()
            If Me.Mantissa = 0 Then Return "0" ' special case: for 0 nothing needs to be inserted

            Dim neg As Boolean = False
            Dim str As StringBuilder
            If Me.Mantissa < 0 Then
                str = New StringBuilder((-Me.Mantissa).ToString())
                neg = True
            Else
                str = New StringBuilder(Me.Mantissa.ToString())
            End If

            If Me.Exponent < 0 AndAlso str.Length + Me.Exponent <= 0 Then ' we need to add 0's to left
                Dim left As String = "".PadLeft(-Me.Exponent - str.Length, "0"c) ' generate string of zeros
                str.Insert(0, left)
                str.Insert(0, "0" & CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)
            ElseIf Me.Exponent < 0 Then ' just insert point
                str.Insert(str.Length + Me.Exponent, CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)
            Else ' we need to append 0's to right
                str.Append("".PadLeft(Me.Exponent, "0"c)) ' generate string of zeros
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
            If Me.Mantissa = 0 Then Return "0"

            Dim tmp As BigDecimal = Me.Truncate(10)

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
            Return String.Concat(If(neg, "-", ""), val.ToString(), " E ", expo)
        End Function

        Public Function IsOutsideDispRange() As Boolean
            Return Me > New BigDecimal(MAX_FULL_DISP) OrElse Me < New BigDecimal(-MAX_FULL_DISP) OrElse
                Math.Abs(CDbl(Me)) < MIN_FULL_DISP
        End Function

        Public Overrides Function ToString() As String
            If Me.IsUndefined Then Return "Undefined"
            If Me.Mantissa = 0 Then Return "0"
            If Me.IsOutsideDispRange() Then
                Return Me.ToScientific()
            Else
                Return Me.Truncate(10).ToFullNumber()
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
