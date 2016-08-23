Imports System.Drawing.Text
Imports System.Reflection
Imports System.Runtime.InteropServices
Imports System.Text
Imports Cantus.Core
Imports Cantus.Core.CommonTypes
Imports Cantus.Core.Scoping
Namespace UI
    Module MainModule
        Public ReadOnly Property OpenSans As FontFamily
            Get
                If Fonts Is Nothing Then LoadFonts()
                Return Fonts.Families(0)
            End Get
        End Property

        Public ReadOnly Property OpenSansLight As FontFamily
            Get
                If Fonts Is Nothing Then LoadFonts()
                Return Fonts.Families(1)
            End Get
        End Property

        Private Fonts As PrivateFontCollection

        Private FontBuffer As IntPtr

        ' Win32 API to use console
        <DllImport("kernel32.dll")>
        Private Function AttachConsole(dwProcessId As Integer) As Boolean
        End Function
        <DllImport("kernel32.dll", SetLastError:=True)>
        Private Function FreeConsole() As Boolean
        End Function

        Private Sub PrintWelcomeMessage()
            Console.WriteLine()
            Console.WriteLine("Welcome to Cantus v." & Version)
            Console.WriteLine("By Alex Yu 2016")
        End Sub

        '' <summary>
        '' Load all private fonts
        '' </summary>
        Friend Sub LoadFonts()
            If Fonts Is Nothing Then
                Fonts = New PrivateFontCollection()
                Dim font As Byte() = My.Resources.OpenSans_Regular
                FontBuffer = Marshal.AllocCoTaskMem(font.Length)
                Marshal.Copy(font, 0, FontBuffer, font.Length)
                Fonts.AddMemoryFont(FontBuffer, font.Length)

                Dim font2 As Byte() = My.Resources.OpenSans_Light
                FontBuffer = Marshal.AllocCoTaskMem(font2.Length)
                Marshal.Copy(font2, 0, FontBuffer, font2.Length)
                Fonts.AddMemoryFont(FontBuffer, font2.Length)
            End If
        End Sub

        Private Sub EvalWrite(script As String, Optional ByVal path As String = "")
            Try
                Dim res As String = Globals.RootEvaluator.DeepCopy().
                                         Eval(script, noSaveAns:=True, returnedOnly:=True)

                Console.WriteLine()
                If Not res Is Nothing Then
                    ' prints to the user the result only if a value is returned
                    Console.WriteLine(res)
                Else
                    ' else if no result is returned we tell the user this executed successfully
                    Console.WriteLine("Finished executing " & IO.Path.GetFileName(path) & ", 0 errors.")
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
            ' setup folders, etc.
            Dim requiredFolders As String() = {"plugin", "include", "init"}
            For Each dir As String In requiredFolders
                If Not IO.Directory.Exists(dir) Then
                    Try
                        IO.Directory.CreateDirectory(dir)
                    Catch
                    End Try
                End If
            Next

            ' load initialization, plugin scripts
            Dim initScripts As New List(Of String)
            If IO.Directory.Exists("plugin/") Then initScripts.AddRange(
                IO.Directory.GetFiles("plugin/", "*.can", IO.SearchOption.AllDirectories))

            ' initialization files: init.can and init/* ran in root scope on startup
            If IO.File.Exists("init.can") Then initScripts.Add("init.can")
            If IO.Directory.Exists("init/") Then initScripts.AddRange(
                IO.Directory.GetFiles("init/", "*.can", IO.SearchOption.AllDirectories))

            For Each file As String In initScripts
                Try
                    ' Evaluate each file. On error, ignore.
                    Globals.RootEvaluator.Load(file, file = "init.can" OrElse file.ToLower().
                                           StartsWith("init" & IO.Path.DirectorySeparatorChar))
                Catch ex As Exception
                    If file = "init.can" Then
                        MsgBox("Error occurred while processing init.can." & vbNewLine & "Variables and functions may not load." & vbNewLine & vbNewLine & "Message:" & vbNewLine & ex.Message,
                               MsgBoxStyle.MsgBoxSetForeground Or MsgBoxStyle.Critical, "Initialization Error")
                    Else
                        MsgBox("Error occurred while loading """ & file.Replace(IO.Path.DirectorySeparatorChar,
                                                                                SCOPE_SEP).
                               Remove(file.LastIndexOf(".")) & """" & vbNewLine & ex.Message,
                               MsgBoxStyle.MsgBoxSetForeground Or MsgBoxStyle.Exclamation, "Initialization Error")
                    End If
                End Try
            Next

            ' if init.can not found, restore constants and try restoring from Settings.State
            If My.Settings IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(My.Settings.ErrorSaveState) Then
                Try
                    Globals.RootEvaluator.Eval(My.Settings.ErrorSaveState)
                Catch
                    ' clear the state if it is invalid
                    My.Settings.ErrorSaveState = ""
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
                If s = "-i" OrElse s = "--input" Then
                    AttachConsole(-1)
                    runFiles = True
                    PrintWelcomeMessage()
                ElseIf s = "-h" OrElse s = "--help" Then
                    PrintWelcomeMessage()
                    Console.WriteLine("Available commands:")
                    Console.WriteLine("-i --input [file1] [file2]..." & vbTab & "Run scripts at the specified paths")
                    Console.WriteLine()
                    Console.WriteLine("--sigfigs/--nosigfigs        " & vbTab & "SigFig mode on/off")
                    Console.WriteLine("--explicit/--implicit        " & vbTab & "Explicit mode on/off")
                    Console.WriteLine("--anglerepr=[deg/rad/grad]   " & vbTab & "Set angle representation")
                    Console.WriteLine("--output=[raw/math/sci]      " & vbTab & "Set output format")
                    Console.WriteLine("--output=[raw/math/sci]      " & vbTab & "Set output format")
                    Console.WriteLine()
                    Console.WriteLine("-g --graphing                " & vbTab & "Default graphing view")
                    Console.WriteLine("-d [text] --default [text]   " & vbTab & "Set initial editor/graphing view editor text (depending on -g)")
                    Console.WriteLine(" [file]                      " & vbTab & "Open the file in the editor")
                    Console.WriteLine()
                    Console.WriteLine("-h --help                    " & vbTab & "Show this help")
                    FreeConsole()
                    SendKeys.SendWait("{ENTER}")
                    Exit Sub
                ElseIf s = "--sigfigs" Then
                    Globals.RootEvaluator.SignificantMode = True
                ElseIf s = "--nosigfigs" Then
                    Globals.RootEvaluator.SignificantMode = False
                ElseIf s = "--explicit" Then
                    Globals.RootEvaluator.ExplicitMode = True
                ElseIf s = "--implicit" Then
                    Globals.RootEvaluator.ExplicitMode = False
                ElseIf s = "--anglerepr=rad" Then
                    Globals.RootEvaluator.AngleMode = Core.CantusEvaluator.eAngleRepresentation.Radian
                ElseIf s = "--anglerepr=deg" Then
                    Globals.RootEvaluator.AngleMode = Core.CantusEvaluator.eAngleRepresentation.Degree
                ElseIf s = "--anglerepr=grad" Then
                    Globals.RootEvaluator.AngleMode = Core.CantusEvaluator.eAngleRepresentation.Gradian
                ElseIf s = "--output=raw" Then
                    Globals.RootEvaluator.OutputFormat = Core.CantusEvaluator.eOutputFormat.Raw
                ElseIf s = "--output=math" Then
                    Globals.RootEvaluator.OutputFormat = Core.CantusEvaluator.eOutputFormat.Math
                ElseIf s = "--output=sci" Then
                    Globals.RootEvaluator.OutputFormat = Core.CantusEvaluator.eOutputFormat.Scientific
                End If

                EvalWrite(IO.File.ReadAllText(s), s)
            Next

            If runFiles Then
                ' free the console and exit
                FreeConsole()
                SendKeys.SendWait("{ENTER}")
                Exit Sub
            Else
                LoadFonts()
                '  open form
                Application.EnableVisualStyles()
                Application.Run(SplashScreen)
            End If
        End Sub
    End Module
End Namespace
