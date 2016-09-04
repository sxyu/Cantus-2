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
            Try
                RootEvaluator = New CantusEvaluator()
            Catch ex As Exception
                MsgBox(ex.Message, MsgBoxStyle.Exclamation Or MsgBoxStyle.ApplicationModal, "Initialization Error")
            End Try

            RootEvaluator.ThreadController.MaxThreads = 5

            Dim cantusPath As String = IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) & IO.Path.DirectorySeparatorChar

            ' setup folders, etc.
            Dim requiredFolders As String() = {cantusPath + "plugin", cantusPath + "include", cantusPath + "init"}
            For Each dir As String In requiredFolders
                If Not IO.Directory.Exists(dir) Then
                    Try
                        IO.Directory.CreateDirectory(dir)
                    Catch
                    End Try
                End If
            Next

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
                    Globals.RootEvaluator.AngleMode = Core.CantusEvaluator.AngleRepresentation.Radian
                ElseIf s = "--anglerepr=deg" Then
                    Globals.RootEvaluator.AngleMode = Core.CantusEvaluator.AngleRepresentation.Degree
                ElseIf s = "--anglerepr=grad" Then
                    Globals.RootEvaluator.AngleMode = Core.CantusEvaluator.AngleRepresentation.Gradian
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
