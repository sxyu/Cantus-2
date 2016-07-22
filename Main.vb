Imports System.Runtime.InteropServices
Imports System.Text
Imports Cantus.Calculator.Evaluator

Namespace Calculator
    Module MainModule
        ' Win32 API to use console
        <DllImport("kernel32.dll")>
        Private Function AttachConsole(dwProcessId As Integer) As Boolean
        End Function
        <DllImport("kernel32.dll", SetLastError:=True)>
        Private Function FreeConsole() As Boolean
        End Function

        Private Sub PrintWelcomeMessage()
            Console.WriteLine()
            Console.WriteLine("Welcome to Cantus v." & Application.ProductVersion & " Alpha")
            Console.WriteLine("By Alex Yu 2016")
        End Sub

        Private Sub EvalWrite(script As String, Optional ByVal path As String = "", Optional ByVal clone As Boolean = False)
            Try
                Dim res As StatementRegistar.StatementResult =
                            DirectCast(If(clone, Globals.Evaluator.Clone(), Globals.Evaluator).
                                         EvalRaw(script, True), StatementRegistar.StatementResult)

                Console.WriteLine()
                If res.Code = StatementRegistar.StatementResult.ExecCode.return Then
                    ' prints to the user the result only if a value is returned
                    Console.WriteLine(Globals.Evaluator.InternalFunctions.O(res.Value))
                ElseIf res.Code = StatementRegistar.StatementResult.ExecCode.resume
                    ' else if no result is returned we tell the user this executed successfully
                    Console.WriteLine("Finished executing " & IO.Path.GetFileName(path) & ", 0 errors.")
                Else
                    ' else if the user tried to break or continue at the top level then we complain
                    Console.WriteLine("Invalid " & res.Code.ToString() & " statement")
                End If

            Catch ex As Exception
                Console.WriteLine()
                If Not path = "" Then
                    Console.WriteLine("In " & IO.Path.GetFileName(path) & ":")
                End If
                Console.WriteLine(ex.Message)
            End Try
        End Sub

        Private Sub InitConsole()
            AttachConsole(-1)
        End Sub


        <STAThread>
        Public Sub Main()

            ' add initialization, plugin scripts
            Dim initScripts As New List(Of String)
            If IO.Directory.Exists("plugin") Then initScripts.AddRange(
                IO.Directory.GetFiles("plugin", "*.can", IO.SearchOption.AllDirectories))

            If IO.File.Exists("init.can") Then initScripts.Add("init.can")

            If IO.Directory.Exists("init") Then initScripts.AddRange(
                IO.Directory.GetFiles("init", "*.can", IO.SearchOption.AllDirectories))

            For Each file As String In initScripts
                Try
                    ' Evaluate each file. On error, ignore.
                    If file = "init.can" Then
                        Globals.Evaluator.Include(file, True)
                    Else
                        Globals.Evaluator.Include(file)
                    End If
                Catch
                End Try
            Next

            ' if init.can not found, restore constants and try restoring from Settings.State
            If Not IO.File.Exists("init.can") Then
                Globals.Evaluator.ReloadDefault() ' reload constants
                Try
                    Globals.Evaluator.Eval(My.Settings.State)
                Catch
                    ' clear the state if it is invalid
                    My.Settings.State = ""
                    My.Settings.Save()
                End Try
            End If

            ' process command line args
            Dim args As String() = Environment.GetCommandLineArgs()
            Dim def As String = ""

            Dim runFiles As Boolean = False
            Dim curExpr As New StringBuilder()
            For i As Integer = 1 To args.Length - 1

                Dim s As String = args(i)
                If IO.File.Exists(s) Then ' run files
                    ' for first file, open console and write welcome message
                    If Not runFiles Then
                        InitConsole()
                        runFiles = True
                        PrintWelcomeMessage()
                    End If
                End If

                EvalWrite(IO.File.ReadAllText(s), s, True)
            Next

            If runFiles Then
                ' free the console and exit
                FreeConsole()
                System.Windows.Forms.SendKeys.SendWait("{ENTER}")
                Exit Sub
            Else
                '  open form
                Application.EnableVisualStyles()
                Application.Run(FrmCalc)
            End If
        End Sub
    End Module
End Namespace
