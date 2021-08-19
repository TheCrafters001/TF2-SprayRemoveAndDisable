Imports System.IO
Imports Gameloop.Vdf
Imports Gameloop.Vdf.JsonConverter
Imports Gameloop.Vdf.Linq
Imports Microsoft.Win32
Imports Newtonsoft.Json
Imports TF2_SprayRemoveAndDisable.Models

Module Module1
    Public Property Result() As Category

    Sub Main()
        ' Check for Steam
        ' Solution: harrymc - https:\\stackoverflow.com\a\47746130\7799766
        Dim rk1 As RegistryKey
        Dim rk2 As RegistryKey

        Debug.WriteLine(Environment.Is64BitProcess)

        If Environment.Is64BitProcess = True Then
            rk1 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)
            rk2 = rk1.OpenSubKey("SOFTWARE\Wow6432Node\Valve\Steam")
            Console.WriteLine("64-Bit Detected")
        ElseIf Environment.Is64BitProcess = False Then
            rk1 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32)
            rk2 = rk1.OpenSubKey("SOFTWARE\Valve\Steam")
            Console.WriteLine("32-Bit Detected")
        End If
        Dim PID As String = rk2.GetValue("InstallPath").ToString

        ' Write Debug Line (For Testing Purposes
        Debug.WriteLine(PID)

        ' Display Information to user
        Console.WriteLine("Steam found at: {0}", PID)

        'Look for TF2 Directory
        Console.WriteLine("Looking for Team Fortress 2...")
        If My.Computer.FileSystem.DirectoryExists(PID + "\steamapps\common\Team Fortress 2") Then
            Debug.WriteLine(PID + "\steamapps\common\Team Fortress 2\ detected.")

            Console.WriteLine("Team Fortress 2 found at {0}{1}", PID, "\steamapps\common\Team Fortress 2")
        ElseIf Not My.Computer.FileSystem.DirectoryExists(PID + "\steamapps\common\Team Fortress 2") Then
            Debug.WriteLine(PID + "\steamapps\common\Team Fortress 2\ NOT detected.")

            Console.WriteLine("Team Fortress 2 is NOT in Default Directory.")

            ' Look for libraryfolders.vdf
            ' This file is a JSON file that contains the
            ' actual location of Team Fortress 2. If Not Found,
            ' TF2 is not installed.
            Console.WriteLine("Looking for 'libraryfolders.vdf'...")

            Debug.WriteLine(My.Computer.FileSystem.FileExists(PID + "\steamapps\libraryfolders.vdf"))
            If My.Computer.FileSystem.FileExists(PID + "\steamapps\libraryfolders.vdf") Then
                Debug.WriteLine(PID + "\steamapps\libraryfolders.vdf detected.")

                Console.WriteLine("Library Folders File found.")

                Dim volvo As VProperty = VdfConvert.Deserialize(File.ReadAllText(PID + "\steamapps\libraryfolders.vdf"))
                Console.WriteLine(volvo.ToJson().ToObject(Of Category)())

            ElseIf Not My.Computer.FileSystem.FileExists(PID + "\steamapps\libraryfolders.vdf") Then
                Debug.WriteLine(PID + "\steamapps\libraryfolders.vdf not detected.")

                Console.WriteLine("libraryfolders.vdf could not be found. This could indicate that your steam directory is corrupt or missing.")
                Console.WriteLine("Press enter to exit.")
                Console.ReadLine()

                End
            End If
        End If
        Console.Read()
    End Sub

End Module



Namespace Models

    Public Class Category

        <JsonProperty("path")>
        Public Property steamPaths As String

        <JsonProperty("label")>
        Public Property steamLabel As String

        <JsonProperty("mounted")>
        Public Property steamMounted As Integer

        <JsonProperty("contentid")>
        Public Property steamContentID As Integer

    End Class

End Namespace