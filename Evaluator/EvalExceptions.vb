Namespace Calculator.Evaluator.Exceptions
    ''' <summary>
    ''' Represents any exception that occurs during evaluation
    ''' </summary>
    Public Class EvaluatorException : Inherits Exception
        Public ReadOnly Property Line As Integer
        Public Sub New()
            MyBase.New()
        End Sub
        Public Sub New(message As String, Optional line As Integer = 0)
            MyBase.New(New Evaluator().InternalFunctions.Replace(message, " \[Line [0-9]*\]", "") &
                                                     If(line > 0, " [Line " & line & "]", ""))
            Me.Line = line
        End Sub
    End Class

    ''' <summary>
    ''' Represents any exception caused by incorrect operator, statement, or statement syntax
    ''' </summary>
    Public Class SyntaxException : Inherits EvaluatorException
        Public Sub New()
            MyBase.New("Syntax Error")
        End Sub
        Public Sub New(message As String, Optional line As Integer = 0)
            MyBase.New(If(Not message.StartsWith("Syntax Error"), "Syntax Error: " & message, message), line)
        End Sub
    End Class

    ''' <summary>
    ''' Represents any exception caused by invalid math operations 
    ''' </summary>
    Public Class MathException : Inherits EvaluatorException
        Public Sub New()
            MyBase.New("Math Error")
        End Sub
        Public Sub New(message As String, Optional line As Integer = 0)
            MyBase.New(If(Not message.StartsWith("Math Error"), "Math Error: " & message, message), line)
        End Sub
    End Class
End Namespace
