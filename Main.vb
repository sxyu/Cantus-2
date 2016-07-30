﻿Imports System.Reflection
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
                            DirectCast(If(clone, Globals.Evaluator.DeepCopy(), Globals.Evaluator).
                                         EvalRaw(script, noSaveAns:=True, internal:=True), StatementRegistar.StatementResult)

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

        <STAThread>
        Public Sub Main()
            EmbeddedAssembly.Load("Cantus.ScintillaNET.dll", "ScintillaNET.dll")
            AddHandler AppDomain.CurrentDomain.AssemblyResolve, AddressOf CurrentDomain_AssemblyResolve

            ' setup folders, etc.
            Dim requiredFolders As String() = {"plugin", "include", "init"}
            For Each dir As String In requiredFolders
                If Not IO.Directory.Exists(Dir) Then
                    Try
                        IO.Directory.CreateDirectory(Dir)
                    Catch
                    End Try
                End If
            Next

            ' load initialization, plugin scripts
            Dim initScripts As New List(Of String)
            If IO.Directory.Exists("plugin") Then initScripts.AddRange(
                IO.Directory.GetFiles("plugin", "*.can", IO.SearchOption.AllDirectories))

            If IO.File.Exists("init.can") Then initScripts.Add("init.can")

            If IO.Directory.Exists("init") Then initScripts.AddRange(
                IO.Directory.GetFiles("init", "*.can", IO.SearchOption.AllDirectories))

            For Each file As String In initScripts
                Try
                    ' Evaluate each file. On error, ignore.
                    Globals.Evaluator.Load(file, file = "init.can")
                Catch ex As Exception
                    If file = "init.can" Then
                        MsgBox("Error occurred while processing init.can." & vbNewLine & "Variables and functions may not load." &
                               vbNewLine & vbNewLine & "Message:" & vbNewLine & ex.Message,
                               MsgBoxStyle.MsgBoxSetForeground Or MsgBoxStyle.Critical, "Initialization Error")
                    Else
                        MsgBox("Error occurred while loading ''" & file.Replace(IO.Path.DirectorySeparatorChar,
                                                                                Evaluator.Evaluator.SCOPE_SEP).
                               Remove(file.LastIndexOf(".")) & "''" & vbNewLine & ex.Message,
                               MsgBoxStyle.MsgBoxSetForeground Or MsgBoxStyle.Exclamation, "Initialization Error")
                    End If
                End Try
            Next

            ' if init.can not found, restore constants and try restoring from Settings.State
            If Not String.IsNullOrWhiteSpace(My.Settings.State) Then
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
                        AttachConsole(-1)
                        runFiles = True
                        PrintWelcomeMessage()
                    End If
                End If

                EvalWrite(IO.File.ReadAllText(s), s, True)
            Next

            If runFiles Then
                ' free the console and exit
                FreeConsole()
                SendKeys.SendWait("{ENTER}")
                Exit Sub
            Else
                '  open form
                Application.EnableVisualStyles()
                Application.Run(FrmCalc)
            End If
        End Sub

        Private Function CurrentDomain_AssemblyResolve(sender As Object, args As ResolveEventArgs) As Assembly
            Return EmbeddedAssembly.Get(args.Name)
        End Function
    End Module
End Namespace
